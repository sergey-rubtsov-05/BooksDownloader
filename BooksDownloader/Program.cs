using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Xml.Linq;
using Ionic.Zip;

namespace BooksDownloader
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("BOOKS DOWNLOADER");
            for (int i = 1; i <= 50; i++)
            {
                string url = "http://knigosite.org/library/genres/110/page/" + i;
                HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                StreamReader stream = new StreamReader(myResponse.GetResponseStream());
                string html = stream.ReadToEnd();
                //File.Create(@"C:\Users\Сергей\Desktop\1.xml");
                while (html.IndexOf('&') != -1)
                {
                    int startIndex = html.IndexOf('&');
                    html = html.Remove(startIndex, 1);
                }
                File.WriteAllText(@"\1.xml", html);
                //Console.WriteLine(html);

                XDocument doc = XDocument.Load(@"\1.xml");
                var els = doc.Root.Element("body").Element("div").Elements();
                XElement mainDiv = null;
                foreach (var xElement in els)
                {
                    if (xElement.Attribute("class").Value == "main  library genres")
                    {
                        mainDiv = xElement;
                        break;
                    }
                }
                els = mainDiv.Element("div").Element("div").Elements();
                XElement ul = null;
                foreach (var xElement in els)
                {
                    if (xElement.HasAttributes && xElement.Attribute("class").Value == "lib_books_list books")
                    {
                        ul = xElement;
                        break;
                    }
                }
                els = ul.Elements();
                Dictionary<string, string> links = new Dictionary<string, string>();
                char[] urlSeparator = { '/' };
                string[] urlParts = null;
                string urlTemplate = "http://108.160.149.68/valera/{0}.fb2.zip";
                foreach (var xElement in els)
                {
                    var link = xElement.Element("p").Element("a");
                    urlParts = link.Attribute("href").Value.Split(urlSeparator);
                    links.Add(link.Attribute("title").Value, string.Format(urlTemplate, urlParts[urlParts.Length - 1]));
                }

                foreach (var link in links)
                {
                    Console.WriteLine(link);
                    try
                    {
                        WebClient wc = new WebClient();
                        Uri uri = new Uri(link.Value);
                        string unpackDir = @"C:\Users\Сергей\Downloads\Книги\";
                        string zipToUnpack = unpackDir + link.Key + ".zip";
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Downloading begin");
                        Console.ResetColor();
                        wc.DownloadFile(uri, zipToUnpack);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Book succeseful download");
                        Console.ResetColor();

                        using (ZipFile zip = ZipFile.Read(zipToUnpack))
                        {
                            Stream s = new MemoryStream();
                            foreach (ZipEntry e in zip)
                            {
                                e.Extract(s);
                                byte[] bytes = new byte[s.Length];
                                s.Seek(0, SeekOrigin.Begin);
                                int len = s.Read(bytes, 0, (int)s.Length);
                                Console.WriteLine("Прочитано {0} байт",len);
                                File.WriteAllBytes(unpackDir + link.Key + ".fb2", bytes);
                            }
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Unpack succeseful");
                            Console.ResetColor();
                        }
                        File.Delete(zipToUnpack);

                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(e);
                        Console.ResetColor();
                    }
                }
            }
            Console.WriteLine("Done");
            Console.Read();
        }
    }
}
