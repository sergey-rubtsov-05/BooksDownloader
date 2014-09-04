using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace BooksDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("BOOKS READER");

            string url = "http://knigosite.org/library/genres/110/page/1";
            string html = String.Empty;
            HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader stream = new StreamReader(myResponse.GetResponseStream(), Encoding.GetEncoding(1251));
            html = stream.ReadToEnd();
            //File.Create(@"C:\Users\Сергей\Desktop\1.xml");
            
            File.WriteAllText(@"C:\Users\Сергей\Desktop\1.xml", html);
            Console.WriteLine(html);

            Console.Read();
        }
    }
}
