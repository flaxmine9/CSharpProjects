using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using System.Net;
using Parse_VK.Bot;

namespace Parse_VK
{
    public class Vkontakte
    {
        ChromeDriver chrome { get; set; }
        MyPicture picture { get; set; }

        string login { get; set; }
        string password { get; set; }
      
        public Vkontakte(string login, string password)
        {
            this.login = login;
            this.password = password;

            chrome = new ChromeDriver();
            picture = new MyPicture(chrome, @"E:\Page");
        }

        public Vkontakte() { }

        private int RandomTime(int min, int max)
        {
            Random random = new Random();

            return random.Next(min, max);
        }

        public void Authorization()
        {
            chrome.Navigate().GoToUrl("https://www.vk.com/");
            chrome.Manage().Window.Maximize();
            SetTextInField("index_email", login);
            Thread.Sleep(500);
            SetTextInField("index_pass", password);
            Thread.Sleep(500);
            SetClickOnField("index_login_button");
        }

        private void SetTextInField(string id, string values)
        {
            IWebElement login_field = chrome.FindElementById(id);
            char[] symbols = values.ToArray();
            
            for (int i = 0; i < symbols.Length; i++)
            {
                Thread.Sleep(RandomTime(100, 250));
                login_field.SendKeys(symbols[i].ToString());
            }
            
        }

        private void SetClickOnField(string id)
        {
            List<IWebElement> list = chrome.FindElementsById(id).ToList();
            foreach (IWebElement item in list)
            {
                item.Click();
                return;
            }
        }

        public List<string>[] GetNews()
        {
            List<string>[] news = new List<string>[8];
            List<string> id_groups = new List<string>();
            List<IWebElement> feed_row = new List<IWebElement>();

            List<string> reposts = new List<string>();
            List<string> likes = new List<string>();
            List<string> names = new List<string>();
            List<string> texts = new List<string>();
            List<string> views = new List<string>();

            int counter = 0;
            //int y = 0;

            while (id_groups.Count <= 20)
            {
                feed_row = (from a in (chrome.FindElements(By.ClassName("feed_row"))) where a.Location.Y != 0 select a).ToList();
                id_groups = (from id in (from a in feed_row select a.FindElements(By.TagName("div")))
                             where id.First().GetAttribute("id") != "ads_feed_placeholder"
                             select id.First().GetAttribute("id")).ToList();
               
                counter = names.Count;
                for (int j = counter; j < id_groups.Count; j++)
                {                     
                    ((IJavaScriptExecutor)chrome).ExecuteScript("arguments[0].scrollIntoView();", feed_row[j]);
                   // DownloadPictures(feed_row[j], id_groups[j], y);
                   // y = feed_row[j + 1].Location.Y;

                    reposts.AddRange(Reposts(feed_row[j]));
                    likes.AddRange(Likes(feed_row[j]));
                    texts.AddRange(Text(feed_row[j]));
                    names.AddRange(Names(feed_row[j]));
                    views.AddRange(Views(feed_row[j]));
                                      
                    Thread.Sleep(RandomTime(200, 400));
                }
            }

            news[0] = Times();
            news[1] = reposts;
            news[2] = likes;
            news[3] = texts;
            news[4] = names;
            news[5] = views;
            news[6] = id_groups;

            return news;
        }
  
        private List<string> Names(IWebElement feed_row)
        {
            List<IWebElement> post_author = (feed_row.FindElements(By.CssSelector("h5[class='post_author']"))).ToList();
            return (from names in post_author select names.Text).ToList();
        }

        private List<string> Times()
        {
            List<IWebElement> post_link = (chrome.FindElements(By.CssSelector("span[class='rel_date rel_date_needs_update']"))).ToList();
            return (from time in post_link select time.GetAttribute("abs_time")).ToList();         
        }

        private List<string> Text(IWebElement feed_row)
        {
            List<IWebElement> wall_post_text = (feed_row.FindElements(By.CssSelector("div[class='wall_text']"))).ToList();
            return (from text in wall_post_text select text.Text).ToList();
        }

        public List<string> Likes(IWebElement feed_row)
        {
            return (from attribute in (feed_row.FindElements(By.CssSelector("a[class='like_btn like _like']")))
                    where attribute.GetAttribute("onmouseover").IndexOf("wall_reply") == -1
                    select attribute.GetAttribute("data-count")).ToList();                                      
        }

        private List<string> Reposts(IWebElement feed_row)
        {
            return (from reposts in (feed_row.FindElements(By.CssSelector("a[class='like_btn share _share']")))
                    select reposts.GetAttribute("data-count")).ToList();
        }

        public List<string> Views(IWebElement feed_row)
        {
            return (from views in (feed_row.FindElements(By.CssSelector("div[class='like_views _views']")))
                    select views.GetAttribute("data-count")).ToList();         
        }

        public void DownloadPicture(string path, List<string>[] lst)
        {
            using (WebClient client = new WebClient())
            {
                foreach (List<string> url in lst)
                {
                    for (int i = 0; i < url.Count; i++)
                    {
                        string name = GetNameFile(url[i]);
                        client.DownloadFile(url[i], path + @"\" + name);
                    }                  
                }
            }
        }

        public string GetNameFile(string link)
        {
            string stroka = "";

            for (int i = link.Length - 1; i > 0; i--)
            {
                if (link[i] != '/')
                {
                    stroka += link[i];
                }

                else { break; }
            }

            return new string(stroka.ToCharArray().Reverse().ToArray());
        }

        public List<string> FindPircture()
        {        
            List<IWebElement> post_header = chrome.FindElements(By.CssSelector("div[class='post_header']")).ToList();

            List<string> Url = (from image in post_header select image.FindElement(By.TagName("img")).GetAttribute("src")).ToList();

            return Url;
        }

        public List<string>[] PicturesFromGroups(List<string> id_groups)
        {
            List<int> count = new List<int>();
            List<string>[] urls = new List<string>[id_groups.Count];

            for (int i = 0; i < id_groups.Count; i++)
            {
                List<IWebElement> url = (chrome.FindElements(By.CssSelector($"#{id_groups[i]} div[class='page_post_sized_thumbs  clear_fix'] a"))).ToList();
                urls[i] = (from id in url select id.GetAttribute("style")).ToList();
            }

            int counter = 0;
            foreach (List<string> item in urls)
            {
                for (int k = 0; k < item.Count; k++)
                {
                    int indexOfhttps = item[k].IndexOf("https");
                    if (indexOfhttps != -1)
                    {
                        string delete_left = item[k].Remove(0, indexOfhttps);
                        int indexOfjpg = delete_left.IndexOf("jpg");
                        string delete_right = delete_left.Remove(indexOfjpg + 3, delete_left.Length - 3 - indexOfjpg);
                        urls[counter][k] = delete_right;
                    }
                    else
                    {
                        urls[counter].RemoveAt(k);
                    }
                }
                counter++;
            }
            return urls;
        }            

        public List<string> Id_Gropus()
        {
            List<IWebElement> feed_row = (chrome.FindElements(By.ClassName("feed_row"))).ToList();
            List<string> id_groups = (from id in (from a in feed_row select a.FindElements(By.TagName("div")))
                                      select id.First().GetAttribute("id")).ToList();
            return id_groups;
        }
     
        public void DownloadPictures(IWebElement element, string id, int y)
        {
            picture.DownloadPictures(element, id, y);
        }

    }
}
