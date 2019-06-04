using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using ClassLibrary1;
using DataBaseService;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

namespace Parse_VK
{
    public partial class Form1 : Form
    {
        Vkontakte vk { get; set; }
        Thread thread { get; set; }
        Writer_Reader writer_Reader { get; set; }
        DataBase db { get; set; }

        List<string>[] news_from_VK { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread mythread = new Thread(() =>
            {               
                vk = new Vkontakte(Description.GetLogin(), Description.GetPassWord());
                vk.Authorization();
            });

            mythread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            writer_Reader = new Writer_Reader();

            //вторичный поток
            thread = new Thread(() =>
            {
                Stopwatch st = new Stopwatch();
                st.Start();

                news_from_VK = vk.GetNews(); // получаем новости(текст, время и т.д)
                List<string>[] urls = vk.PicturesFromGroups(news_from_VK[6]);

                Thread thread2 = new Thread(() =>
                {
                    writer_Reader.WriteToJson(news_from_VK, urls); // записываем в Json
                });

                thread2.Start();
                thread2.Join();

                st.Stop();
                MessageBox.Show($"Время выполнения: {st.ElapsedMilliseconds.ToString()} миллисекунд");
            });           
            thread.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataBase db = new DataBase();
            db.AddToDataBase();

            
        }
    }  
}
