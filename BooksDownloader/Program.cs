using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ionic.Zip;

namespace BooksDownloader
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("BOOKS DOWNLOADER");
            for (int pageNumber = 1; pageNumber <= 117; pageNumber++)
            {
                Console.WriteLine($"PAGE: {pageNumber}");
                var html = GetPageHtmlSource(pageNumber);
                var elements = GetBookElements(html);
                Dictionary<string, string> links = new Dictionary<string, string>();
                foreach (var xElement in elements)
                {
                    var linkData = GetLinkData(xElement);
                    links.Add(linkData[0], linkData[1]);
                }

                foreach (var link in links)
                {
                    Console.WriteLine(link);
                    try
                    {
                        WebClient wc = new WebClient();
                        Uri uri = new Uri(link.Value);
                        string unpackDir = @"C:\Users\Sergey Rubtsov\Downloads\Книги\";
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
                                Console.WriteLine("Прочитано {0} байт", len);
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

        private static string[] GetLinkData(XElement xElement)
        {
            var aElement = xElement.Element("div").Element("a");
            char[] urlSeparator = {'/'};
            var urlParts = aElement.Attribute("href").Value.Split(urlSeparator);
            var authorName =
                xElement.Element("div")
                    .Elements("span")
                    .FirstOrDefault(span => span.HasAttributes && span.Attribute("class").Value.EndsWith("aut")).Value;
            authorName = authorName.Replace("#160;", " ");
            var title = $"{aElement.Attribute("title").Value} - {authorName}";
            string urlTemplate = "http://knigosite.org/download/{0}.fb2.zip";
            var link = string.Format(urlTemplate, urlParts[urlParts.Length - 1]);
            var linkData = new[] {title, link};
            return linkData;
        }

        private static IEnumerable<XElement> GetBookElements(string html)
        {
            XDocument doc = XDocument.Parse(html);
            var elements = doc.Root.Element("body").Element("div").Elements();
            XElement mainDiv = elements.FirstOrDefault(xElement => xElement.Attribute("class").Value == "main  library genres");
            elements = mainDiv.Element("div").Element("div").Elements();
            XElement ul =
                elements.FirstOrDefault(
                    xElement => xElement.HasAttributes && xElement.Attribute("class").Value == "lib_books_list books");
            elements = ul.Elements();
            return elements;
        }

        private static string GetPageHtmlSource(int i)
        {
            string url = "http://knigosite.org/library/genres/110/page/" + i;
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            string html;
            using (StreamReader stream = new StreamReader(myResponse.GetResponseStream()))
            {
                html = stream.ReadToEnd();
            }
            while (html.IndexOf('&') != -1)
            {
                int startIndex = html.IndexOf('&');
                html = html.Remove(startIndex, 1);
            }
            return html;
        }
    }
}
