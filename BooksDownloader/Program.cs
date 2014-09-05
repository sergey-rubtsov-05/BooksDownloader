using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Linq;

namespace BooksDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("BOOKS READER");

            string url = "http://knigosite.org/library/genres/113/page/1";
            string html = String.Empty;
            HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader stream = new StreamReader(myResponse.GetResponseStream());
            html = stream.ReadToEnd();
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
            foreach (var xElement in els)
            {
                var link = xElement.Element("p").Element("a");
                links.Add(link.Attribute("title").Value, link.Attribute("href").Value);
            }

            foreach (var link in links)
            {
                Console.WriteLine(link);
            }

            Console.Read();
        }
    }
}
