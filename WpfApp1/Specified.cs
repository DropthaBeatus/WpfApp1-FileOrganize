using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Specified
    {
        public string name;
       // public List<string> locations = new List<string>();
       public List<string> locations = new List<string>();
        public List<DateTime> createTime = new List<DateTime>();
        public bool used = false;
        //need to create a bool that can act as a UI for this stuff
    public Specified(string mainName, List<string> locate)
        {
                foreach (string s in locate)
                {
                    if(s != null || s != "")
                    {
                    //will need long path tool for exception catch I imagine
                    createTime.Add(File.GetCreationTime(s));
                    used = false;
                    locations.Add(s);
                    }
                }



            
            name = mainName;
        }
    }



}
