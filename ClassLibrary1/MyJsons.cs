using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibrary1
{
    [DataContract]
    public class MyJsons
    {
        [DataMember]
        public string Id_Group { get; set; }
        [DataMember]
        public string News { get; set; }
        [DataMember]
        public News[] Informations { get; set; }
        
        public MyJsons() { }

        public MyJsons(string id_group, string NewsT, News[] newsT)
        {
            News = NewsT;
            Informations = newsT;
            Id_Group = id_group;
        }
    }

    [DataContract]
    public class Pictures
    {
        [DataMember]
        public Picture[] pictures { get; set; }
        [DataMember]
        public string id_group { get; set; }

        public Pictures() { }

        public Pictures(Picture[] picturesT, string id_groupT)
        {
            pictures = picturesT;
            id_group = id_groupT;
        }
    }

    [DataContract]
    public class Names
    {
        [DataMember]
        public string id_groups { get; set; }
        [DataMember]
        public NameGroup nameGroup { get; set; }
        
        public Names() { }

        public Names(NameGroup nameGroupsT, string id_groupT)
        {
            id_groups = id_groupT;
            nameGroup = nameGroupsT;           
        }
    }

    public class News
    {
        public string Times { get; set; }
        public string Text { get; set; }
        public string Quantity_likes { get; set; }
        public string Quantity_reposts { get; set; }
        public string Views { get; set; }

        public News() { }

        public News(List<string> text, List<string> times, List<string> likes, List<string> reposts, List<string> views, int count)
        {
            Text = text[count];
            Times = times[count];
            Quantity_likes = likes[count];
            Quantity_reposts = reposts[count];
            Views = views[count];
        }      
    }

    public class NameGroup
    {
        public string Name { get; set; }

        public NameGroup() { }

        public NameGroup(List<string> name, int count)
        {
            Name = name[count];
        }
    }

    public class Picture
    {       
        public string path { get; set; }
        public List<string> links = new List<string>();   
       // public Enumerator enumerator { get; set; }
       // public Links[] links { get; set; }
        public Picture() { }


        /*public Picture(Links[] linksT)
        {
            links = linksT;
        }*/

        public Picture(List<string> linksT, string pathT, int count, List<string> url)
        {
            if (url.Any())
            {
                for (int i = 0; i < url.Count; i++)
                {
                    links.Add(linksT[i]);  
                }
                path = pathT;
            }
            else
            {
                links.Add("null");
                path = null;
            }        
        }
       
        public class Links
        {
            [DataMember(EmitDefaultValue = false)]
           // public Enumerator enumerator;
            public string link { get; set; }

            public Links() { }           
            
            public Links(string g)
            {
                link = g;
            }          
        }     
    }

    /*[DataContract(Namespace = "Serialization")]
    public enum Enumerator
    {
        [EnumMember(Value = "1")]
        First = 1,
        [EnumMember(Value = "2")]
        Second = 2,
        [EnumMember(Value = "3")]
        Third = 3,
        [EnumMember(Value = "4")]
        Fourth = 4,
        [EnumMember(Value = "5")]
        Fifth = 5,
        [EnumMember(Value = "6")]
        Sixth = 6
    }*/

    public class Json<T>
    {
        public T[] json;

        static object locker = new object();

        public Json() { }

        public void open(string name_file)
        {
            lock (locker)
            {
                DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(T[]));
                using (FileStream fs = new FileStream(name_file, FileMode.OpenOrCreate))
                {
                    json = (T[])jsonFormatter.ReadObject(fs);
                }
            }
        }

        public void save(string name_file)
        {
            lock (locker)
            {
                DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(T[]));
                using (FileStream fs = new FileStream(name_file, FileMode.OpenOrCreate))
                {
                    jsonFormatter.WriteObject(fs, json);
                }
            }
        }     
    }
    
    public class Writer_Reader
    {
        Json<Pictures> js3 { get; set; }
        Json<MyJsons> js { get; set; }
        Json<Names> js2 { get; set; }

        public Writer_Reader() { }

        public void WriteToJson(List<string>[] news, List<string>[] urls)
        {
            MyJsons[] work = new MyJsons[news[1].Count];
            Names[] work2 = new Names[news[1].Count];
            Pictures[] work3 = new Pictures[news[1].Count];

            js = new Json<MyJsons>();
            js2 = new Json<Names>();
            js3 = new Json<Pictures>();

            int count = 0;

            while (count < news[1].Count)
            {
                work[count] = new MyJsons(news[6][count], "Новость " + (count + 1).ToString(),
                    new News[] { new News(news[3], news[0], news[2], news[1], news[5], count) });
                work2[count] = new Names(new NameGroup(news[4], count), news[6][count]);
                work3[count] = new Pictures(new Picture[] { new Picture(urls[count], @"E:\Pictures\VK_Parse", count, urls[count]) }, news[6][count]);
               // work3[count] = new Pictures(new Picture[] { new Picture(new Picture.Links[] { new Picture.Links("kgjhgkjhgkjhgk"){ enumerator = Enumerator.Second } }) }, news[6][count]);
                count++;
            }

            js.json = work;
            js.save("News.json");

            js2.json = work2;
            js2.save("Name.json");

            js3.json = work3;
            js3.save("Path.json");
        }

        public List<string>[] ReadFromJson()
        {
            List<string>[] list = new List<string>[8];

            js3 = new Json<Pictures>();
            js3.open("Path.json");
         
            list[7] = (from a in js3.json select a.pictures.First().path).ToList();

            ////////////////////////////////////////////

            js2 = new Json<Names>();
            js2.open("Name.json");

            list[5] = (from a in js2.json select a.nameGroup.Name).ToList();
            list[6] = (from a in js2.json select a.id_groups).ToList();

            ////////////////////////////////////////////

            js = new Json<MyJsons>();
            js.open("News.json");
          
            List<MyJsons> myJson = (from a in js.json select a).ToList();
            List<News[]> list2 = (from a in myJson select a.Informations).ToList();

            list[0] = (from a in list2 select a.First().Quantity_likes).ToList();
            list[1] = (from a in list2 select a.First().Quantity_reposts).ToList();
            list[2] = (from a in list2 select a.First().Text).ToList();
            list[3] = (from a in list2 select a.First().Times).ToList();
            list[4] = (from a in list2 select a.First().Views).ToList();

            return list;
        }

        public List<List<string>> ReadLinks()
        {
            js3 = new Json<Pictures>();
            js3.open("Path.json");

            return (from a in js3.json select a.pictures.First().links).ToList();
        }
    }
}