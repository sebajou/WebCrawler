using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    using System.IO;
    using System.Net;

    /// <summary>
    /// Définit les constantes.
    /// </summary>
    static class Const
    {
        /// <summary>
        /// 2^8 -1.
        /// </summary>
        public const int ADDRESS_MAX = 255;

        /// <summary>
        /// {0}: DateTime.Now.
        /// {1}: URL.
        /// </summary>
        public const string INTRODUCTION_MESSAGE = "{0}: Recherche {1}";

        /// <summary>
        /// {0}: DateTime.Now.
        /// {1}: URL.
        /// {2}: Message error.
        /// </summary>
        public const string ERROR_CONNECTION_MESSAGE = "{0}: Erreur de connexion {1} {2}";


        public const string ALPHA_CHARS = "abcdefghijklmnopqrstuvwxyz";
    }


    /// <summary>
    /// Point d'entré de l'application console.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string url;
            string searchString;

            if (args.Count() == 2)
            {
                url = AddressBuilder.GetNextIpv4Address(args[0]);
                searchString = args[1];
            }
            else
            {
                url = Console.ReadLine();
                searchString = Console.ReadLine();
            }

            string result = PageContains(url, searchString);
            AppendInFile(@"C:\Users\clement\output.txt", result);

            Main(new string[] { url, searchString });
        }

        /// <summary>
        /// Ecrit dans le fichier txt les logs.
        /// </summary>
        /// <param name="path">Chemin d'accès au fichier.</param>
        /// <param name="lineToAppend">Ligne à ajouter.</param>
        public static void AppendInFile(string path, string lineToAppend)
        {
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(lineToAppend);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(lineToAppend);
                }
            }
        }

        /// <summary>
        /// Détermine si la page à l'url contient le terme recherché.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="searchString">Terme recherché.</param>
        /// <returns>Les lignes qui contiennent le terme recherché.</returns>
        public static string PageContains(string url, string searchString)
        {
            try
            {
                WebRequest webRequest = WebRequest.Create(url);

                //WebProxy webProxy = WebProxy.GetDefaultProxy();
                //webRequest.Proxy = webProxy;

                Stream objStream = webRequest.GetResponse().GetResponseStream();
                StreamReader objReader = new StreamReader(objStream);

                string line = string.Empty;
                int i = 0;

                StringBuilder foundString = new StringBuilder(string.Format(Const.INTRODUCTION_MESSAGE, DateTime.Now, url));

                while (!objReader.EndOfStream)
                {
                    i++;
                    line = objReader.ReadLine();
                    if (line.Contains(searchString))
                    {
                        foundString.AppendLine(line);
                    }
                }

                return foundString.ToString();
            }
            catch (Exception e)
            {
                return string.Format(Const.ERROR_CONNECTION_MESSAGE, DateTime.Now, url, e.Message);
            }
        }
    }
}