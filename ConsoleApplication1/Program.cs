using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    using System.IO;
    using System.Net;

    class Program
    {
        static void Main(string[] args)
        {
            string url  = Console.ReadLine();

            WebRequest webRequest = WebRequest.Create(url);

            Stream objStream = webRequest.GetResponse().GetResponseStream();

            StreamReader objReader = new StreamReader(objStream);

            string line = string.Empty;
            int i = 0;

            while (line != null)
            {
                i++;
                line = objReader.ReadLine();
                if (line != null)
                {
                    Console.WriteLine("{0}:{1}", i, line);
                }
            }

            Main(args);
        }
    }
}
