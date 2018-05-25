using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class FIFO
    {

        public string name;
        public string path;
        public bool completed;
       

        public FIFO (string pth)
        {
            name = FindName(pth);
            path = pth;
            completed = false;
        }


        public string FindName(string pathen)
        {
            string[] nameParts = pathen.Split(System.IO.Path.DirectorySeparatorChar);
               
            string nameTEST = nameParts[nameParts.Length - 1];

            return nameTEST;
        }


    }
}
