using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Office.Interop.Word;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {

        TreeNode folderNode;
        List<FIFO> docs = new List<FIFO>();
        List<Specified> sorts = new List<Specified>();
        string path = @"\\depot\ITDEPT\RTS";
        public MainWindow()
        {
            
            InitializeComponent();


     

                try
                {

                    if (Directory.Exists(path))
                    {
                        // This path is a directory
                        ProcessDirectory(path);
                    }
                    else if (File.Exists(path))
                    {
                        // This path is a file
                        ProcessFile(path);
                    }
                    else
                    {
                        Console.WriteLine("{0} is not a valid file or directory.", path);
                    }
                
                    
                }
                catch (UnauthorizedAccessException)
                {
                  //  MessageBox.Show("Access denied");
                }
                
            List<FIFO> Sorted = docs.OrderBy(o => o.name).ToList();
            float DocCount = docs.Count;
            docs.Clear();
            sorts = Sort(Sorted);
            System.Text.StringBuilder coll = new System.Text.StringBuilder();
            UpdateList();
            DocCount = (sorts.Count / DocCount) * 100;
            System.Windows.MessageBox.Show("Files reduced to " + DocCount + "%");
        }



        /*
         * reference https://msdn.microsoft.com/en-us/library/07wt70x2(v=vs.110).aspx
         */


        public void UpdateList()
        {
            System.Windows.MessageBox.Show("Clearing NameBox");
            NameBox.Items.Clear();
            foreach (Specified temp in sorts)
            {
                if (temp.used == false)
                {
                    NameBox.Items.Add(temp.name);
                    /*      System.Windows.Forms.ListViewItem li = new System.Windows.Forms.ListViewItem(temp.name);

                          li.UseItemStyleForSubItems = false;

                          this.ListV.Items.Add(li);
                         */

                }
            }
        }

        public void ProcessDirectory(string targetDirectory)
        {
            try
            {
                // Process the list of files found in the directory. Need to only process files that hold info not information 
                string[] fileEntries = Directory.GetFiles(targetDirectory);
                foreach (string fileName in fileEntries)
                    ProcessFile(fileName);
            }
            catch (UnauthorizedAccessException)
            {
               Console.WriteLine("Access denied");
            }

            // Recurse into subdirectories of this directory.
            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach (string subdirectory in subdirectoryEntries)
                    ProcessDirectory(subdirectory);
            }
        catch (UnauthorizedAccessException)
            {
                  //  MessageBox.Show("Access denied");
            }
        }
        
    // Insert logic for processing found files here.
    private void ProcessFile(string path)
        {
            FIFO doc1 = new FIFO(path);
            docs.Add(doc1);
        }
        public void ProcessDirectoryTree(string targetDirectory)
        {
            try
            {
                
                // Process the list of files found in the directory. Need to only process files that hold info not information 
                string[] fileEntries = Directory.GetFiles(targetDirectory);
                foreach (string fileName in fileEntries)
                    ProcessNode(fileName);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Access denied");
            }

            // Recurse into subdirectories of this directory.
            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach (string subdirectory in subdirectoryEntries)
                    ProcessNode(subdirectory);
            }
            catch (UnauthorizedAccessException)
            {
                //  MessageBox.Show("Access denied");
            }
        }

        // Insert logic for processing found files here.
        private void ProcessNode(string path)
        {
            string[] nameParts = path.Split(System.IO.Path.DirectorySeparatorChar);
            if (nameParts.Length > 1)
            {
                string nameTEST = nameParts[nameParts.Length - 1];

               
                TreeNode tempNode = new TreeNode(nameTEST);
                folderNode.Nodes.Add(tempNode);
                folderNode.ExpandAll();
            }
            else
            {
                System.Windows.MessageBox.Show("Error here");
            }

           
        }

        public static List<Specified> Sort(List<FIFO> Sorted)
        {
            List<Specified> sorts = new List<Specified>();
            List<string> paths = new List<string>();
            int index = 0;
            string tempS;
            foreach(FIFO doc1 in Sorted) {
                try
                {
                    if (Sorted[index].name == Sorted[index + 1].name)
                    {
                        tempS = Sorted[index].path;
                        paths.Add(tempS);
                    }
                    else if(index < Sorted.Count)
                    {
                        tempS = Sorted[index].path;
                        paths.Add(tempS);
                        Specified temp = new Specified(Sorted[index].name, paths);
                        sorts.Add(temp);
                        paths.Clear();
                    }
                    index++;
                }
                catch(ArgumentOutOfRangeException)
                {
                    return sorts;
                }
               
            
            }
            return sorts;
        }

        private void NameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PathBox.Items.Clear();
           
            try
            {
                foreach (Specified temp in sorts)
                {
                    if (NameBox.SelectedValue.ToString() == temp.name)
                    {
                        int i = 0;
                        foreach (string s in temp.locations)
                        {
                            
                            PathBox.Items.Add(s + "\nCreated on:" + temp.createTime[i++]);
                            
                        }
                    }
                }
            }
            catch(NullReferenceException)
            {
                return;

            }
        }

        private void SearchBTN_Click(object sender, RoutedEventArgs e)
        {
            //need a reverse update list
            NameBox.Items.Clear();
            string s = SearchBox.Text;
            foreach (Specified temp in sorts)
            {
                if((temp.name).Contains(s) == true)
                NameBox.Items.Add(temp.name);
               
                
            }
        }

        private void PathBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FolderView.Items.Clear();
            try
            {
                folderNode.Nodes.Clear();
            }
            catch (NullReferenceException) { }

            string[] nameParts = Convert.ToString(PathBox.SelectedItem).Split(System.IO.Path.DirectorySeparatorChar);
            //create catch statement nameParts Length because index will be out of range
           
            if (nameParts.Length > 2)
            {
                string nameTEST = nameParts[nameParts.Length - 2];
               folderNode = new TreeNode(nameTEST);

               
                string fullPath = "";
                for (int y = 0; y < nameParts.Length - 1; y++)
                {
                    fullPath = fullPath + nameParts[y] + @"\";
                }
                ProcessDirectoryTree(fullPath);

            }
            else
            {
                folderNode = new TreeNode(nameParts[nameParts.Length - 1]);
                FolderView.Items.Add(nameParts[nameParts.Length - 1]);
            }

            if(folderNode != null)
            {
                FolderView.Items.Add(folderNode);
                for (int i = 0; i < folderNode.Nodes.Count - 1; i++)
                {
                    FolderView.Items.Add(folderNode.Nodes[i]);
                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string s = PathBox.SelectedItem.ToString();
               
                foreach (Specified temp in sorts)
                {
                    if (NameBox.SelectedValue.ToString() == temp.name)
                    {
                        int i = 0;
                        foreach (string y in temp.locations)
                        {
                            if(s == y + "\nCreated on:" + temp.createTime[i++])
                            {
                                s = y;
                                break;
                            }
                           //need to create a way if the string cant be found
                        }
                    }
                }
                
                if (s.Contains(".docx"))
                {
                    /*
                    Microsoft.Office.Interop.Word.Application ap = new Microsoft.Office.Interop.Word.Application();
                    Document doc = ap.Documents.Open(s);
                    //Make read only with , ReadOnly:true

                    ap.Quit();
                    */
                    System.Diagnostics.Process.Start(s);
                }
                if (s.Contains(".pdf") || s.Contains(".jpg")|| s.Contains(".gif") || s.Contains(".png")|| s.Contains(".jpeg") || s.Contains(".txt"))
                {
                    System.Diagnostics.Process.Start(s);


                   
                   // FolderView.//.Nodes.Add("test");
                }
                //Need to catch for people trying to launch .exe, .dll, .htm, and other executable files
            }
            catch (NullReferenceException)
            {
                System.Windows.MessageBox.Show("Please Select a path to view");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //need to attach transfer box items to a class for safe keeping and saving
            try
            {
                string[] current = (PathBox.SelectedItem.ToString()).Split('\n');
                TransferBox.Items.Add(current[0]+ "\n" +current[1]);
                foreach (Specified t in sorts)
                {
                    for(int i = 0; i < t.locations.Count; i++)
                    {
                       
                        if (t.locations[i] == current[0])
                        {
                            t.used =true;
                            System.Windows.MessageBox.Show("got here to remove from name box");
                        }
                    }
                }
                UpdateList();

            }
            catch(NullReferenceException)
            {
                System.Windows.MessageBox.Show("Please make sure a file and a file path are selected!");
            }
        }

        private void RemoveFile_Click(object sender, RoutedEventArgs e)
        {
            if (TransferBox.SelectedIndex >= 0)
            {
                string[] current = (TransferBox.SelectedItem.ToString()).Split('\n');
                TransferBox.Items.RemoveAt(TransferBox.SelectedIndex);
               
                foreach (Specified t in sorts)
                {
                    for (int i = 0; i < t.locations.Count; i++)
                    {

                        if (t.locations[i] == current[0])
                        {
                            
                            t.used = false;
                            System.Windows.MessageBox.Show("got here to remove at transbox");
                        }
                    }
                }
                
                UpdateList();


            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Where would you like to transfer these files?";
            saveFileDialog1.InitialDirectory = "c:\\";
            saveFileDialog1.RestoreDirectory = true;
            //might need to get rid of the restore dir read more about it
            System.Windows.Forms.DialogResult savePath = saveFileDialog1.ShowDialog();
            if (savePath == System.Windows.Forms.DialogResult.OK)
            {
                List<string> path = new List<string>();
                foreach(var item in TransferBox.Items)
                {
                    string con = (Convert.ToString(item));
                    //need to fix the parse here with something better this leaves a huge hole in the program
                     string[] tempStr = con.Split('\n');
                    
                    path.Add(tempStr[1]);
                }

                string targetDir = System.IO.Path.GetDirectoryName(saveFileDialog1.FileName);
                foreach (string file in path)
                {
                    try
                    {
                        File.Copy(file, System.IO.Path.Combine(targetDir, System.IO.Path.GetFileName(file)));
                    }
                    catch(IOException)
                    {
                        //need to catch this later this is another big issue
                        //probably can throw a new while loop and and number to end of file while it keeps going up
                        Console.WriteLine("File already exist");
                    }
                }
            }
        }
    }
}

