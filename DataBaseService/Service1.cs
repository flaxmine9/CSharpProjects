using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;


namespace DataBaseService
{
    public partial class Service1 : ServiceBase
    {
        DataBase Database = new DataBase();

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {      
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 5000;
            timer.Enabled = true;
            timer.Elapsed += this.timer1_Tick;
            timer.Start();               
        }

        protected override void OnStop()
        {
           // workWtihData.StopService();
        }

        public void Start()
        {
            System.IO.File.Create(@"E:\Files\" + "OnStop" + DateTime.Now.ToString().Replace('.', '_').Replace(':', '_') + ".txt");

            Database.AddToDataBase();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Start();
        }
    }
}
