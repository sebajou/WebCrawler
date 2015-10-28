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
        public const string INTRODUCTION_MESSAGE = "{0}: Recherche {1}\n";

        /// <summary>
        /// {0}: DateTime.Now.
        /// {1}: URL.
        /// {2}: Message error.
        /// </summary>
        public const string ERROR_CONNECTION_MESSAGE = "{0}: Erreur de connexion {1} {2}";

        /// <summary>
        /// Caractères disponibles pour les adresses internet.
        /// </summary>
        public const string ALPHA_CHARS = "abcdefghijklmnopqrstuvwxyz";
    }

    /// <summary>
    /// Point d'entré de l'application console.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string mainHost;
            string searchString;

            if (args.Count() == 2)
            {
                mainHost = AddressBuilder.GetNextAlphaNumAddress(args[0]);
                searchString = args[1];
            }
            else
            {
                mainHost = Console.ReadLine();
                searchString = Console.ReadLine();
            }

            UriBuilder uri = new UriBuilder() { Host = mainHost + ".fr", Scheme = Uri.UriSchemeHttp };

            string result = PageContains(uri.Uri.ToString(), searchString);
            string robotFile = PageContains(uri.Uri.ToString() + "/robots.txt");

            AppendInFile(@"C:\Users\clement\output.txt", result, robotFile);

            Main(new string[] { mainHost, searchString });
        }

        /// <summary>
        /// Ecrit dans le fichier txt les logs.
        /// </summary>
        /// <param name="path">Chemin d'accès au fichier.</param>
        /// <param name="lineToAppend">Ligne à ajouter.</param>
        public static void AppendInFile(string path, params string[] lineToAppend)
        {
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    foreach (var line in lineToAppend)
                    {
                        sw.WriteLine(line);
                    }
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    foreach (var line in lineToAppend)
                    {
                        sw.WriteLine(line);
                    }
                }
            }
        }

        /// <summary>
        /// Détermine si la page à l'url contient le terme recherché.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="searchString">Terme recherché.</param>
        /// <returns>Les lignes qui contiennent le terme recherché.</returns>
        public static string PageContains(string url, string searchString = null)
        {
            try
            {
                WebRequest webRequest = WebRequest.Create(url);

                //WebProxy webProxy = WebProxy.GetDefaultProxy();
                //webRequest.Proxy = webProxy;
                webRequest.Timeout = 10000;
                Stream objStream = webRequest.GetResponse().GetResponseStream();
                StreamReader objReader = new StreamReader(objStream);

                string line = string.Empty;
                int i = 0;

                string introduction = string.Format(Const.INTRODUCTION_MESSAGE, DateTime.Now, url);
                StringBuilder foundString = new StringBuilder(introduction);
                ColoredConsoleWrite(ConsoleColor.Green, introduction, true);

                if (searchString == null)
                {
                    line = objReader.ReadToEnd();
                    foundString.AppendLine(line);
                    ColoredConsoleWrite(ConsoleColor.Cyan, line, true);
                }
                else
                {
                    while (!objReader.EndOfStream)
                    {
                        i++;
                        line = objReader.ReadLine();
                        if (line.Contains(searchString))
                        {
                            foundString.AppendLine(line);
                            ColoredConsoleWrite(ConsoleColor.White, line, true);
                        }
                    }
                }

                return foundString.ToString();
            }
            catch (Exception e)
            {
                string errorMessage = string.Format(Const.ERROR_CONNECTION_MESSAGE, DateTime.Now, url, e.Message);
                ColoredConsoleWrite(ConsoleColor.Red, errorMessage, true);
                return errorMessage;
            }
        }

        public static void ColoredConsoleWrite(ConsoleColor color, string text, bool writeLine = false)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = originalColor;
            if (writeLine)
            {
                Console.Write("\n");
            }
        }
    }
}