using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

using HtmlAgilityPack;

namespace HtmlAgilityPackTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "./url.list";
            StreamWriter sw = new StreamWriter(path);

            WebClient web = new WebClient();
            string html = web.DownloadString("http://travel.cnn.com/activities?page=1");

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id='block-system-main']/div/div[2]/div/div[1]/a");
            foreach(HtmlNode node in nodes)
            {
                string url = node.Attributes["href"].Value;
                if (url.StartsWith("http://edition.cnn.com/video/")){}
                else if (url.StartsWith("http://"))
                    sw.WriteLine(url);
                else
                    sw.WriteLine("http://travel.cnn.com" + url);
            }
            sw.Close();
            Console.WriteLine("URL GET! Please press any key to continute...");
            Console.ReadKey();
            readandcrew();
        }
        static void readandcrew()
        {
            if (!Directory.Exists(@"./news"))
            {
                DirectoryInfo di = new DirectoryInfo(@".\");
                di.CreateSubdirectory("news");
            }

            string read_path = "./url.list";
            char[] delimiterChars = { '\\' , '/' , ':' , '*' , '?' , ';' , '「' , '<' , '>' , '|' , '\n' , ' '};
            StreamReader sr = new StreamReader(read_path);
            
            while(!sr.EndOfStream){
                string url = sr.ReadLine();

                WebClient img_link_web = new WebClient();
                string img_link_html = img_link_web.DownloadString(url);

                HtmlDocument img_link_doc = new HtmlDocument();
                img_link_doc.LoadHtml(img_link_html);
                
                

                string path,title_path="",title;

                if (url.StartsWith("http://travel.cnn.com/"))
                {
                    path = "//*[@class='node-body']/div/div/div/p";
                    title_path = "//*[@class='title']";
                }
                else if (url.StartsWith("http://edition.cnn.com/") && url.Contains("/travel/gallery/"))
                {
                    path = "//*[@class='cnn_dyncntnt2']/p";
                    title_path = "//*[@class='cnn_storyarea']/h1";
                }
                else
                {
                    path = "//*[@class='cnn_strycntntlft']/p";
                    title_path = "//*[@id='cnnContentContainer']/h1";
                }

                HtmlNode title_node;
                title_node = img_link_doc.DocumentNode.SelectSingleNode(title_path);
                title = "";
                string[] words = title_node.InnerText.Split(delimiterChars);
                foreach (string s in words)
                {
                    if(s!="")
                        title = title + s;
                    if (s != words.Last())
                        title = title + " ";
                }

                string write_path = "./news/" + title + ".txt";
                StreamWriter sw = new StreamWriter(write_path);

                HtmlNodeCollection  img_link_nodes;
                img_link_nodes = img_link_doc.DocumentNode.SelectNodes(path);
                foreach (HtmlNode img_link_node in img_link_nodes)
                {
                    string text = img_link_node.InnerText;
                    sw.WriteLine(text);
                }
                sw.Close();
            }
            Console.WriteLine("Context GET! Please press any key to end this...");
            Console.ReadKey();
            mackdictory();
        }
        static void mackdictory()
        {
            List<one_word> dict = new List<one_word>();
            string[] filePaths = Directory.GetFiles(@"./news");
            foreach(string file in filePaths)
            {
                StreamReader sr = new StreamReader(file);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    char[] delimiterChars = { ' ', ',', '.', '?', '!' , '\n'};
                    string[] words = line.Split(delimiterChars);
                    foreach (string s in words)
                    {
                        if (s == "")
                            continue;
                        int flag = 0;
                        foreach(one_word insi in dict)
                        {
                            if (s.ToLower() == insi.word)
                            {
                                flag = 1;
                                insi.count++;
                                break;
                            }
                        }
                        if (flag == 0)
                        {
                            one_word temp = new one_word();
                            temp.word = s.ToLower();
                            temp.count = 1;
                            dict.Add(temp);
                        }
                    }
                }
            }
            dict.Sort((x, y) => (y.count - x.count));
            string dic_path = "./result.dict";
            StreamWriter sw = new StreamWriter(dic_path);
            foreach (one_word insi in dict)
            {
                string text = insi.word + "\t" + insi.count;
                sw.WriteLine(text);
            }
            sw.Close();
            Console.WriteLine("Complete! Please press any key to end this...");
            Console.ReadKey();
        }
        class one_word
        {
            public string word;
            public int count;
        }
    }
}
