using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Service
{
    public partial class DownloadData : ServiceBase
    {
        private System.Timers.Timer _timer;
        System.Threading.Thread threadGet;
        System.Threading.Thread threadPush;
        System.Threading.Thread[] threadArray;

        public DownloadData()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _timer = new System.Timers.Timer();
                _timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);

                clsLog.Log("Service started");

                _timer.Interval = 500;
                _timer.Enabled = true;

                //ProcessImageGet();
                //ProcessImagePush();
                RunImageThreads();

            }
            catch (Exception ex)
            {
                clsLog.LogError(ex.ToString());
            }
        }

        protected void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                clsLog.LogInfo("Timer Fired " + DateTime.Now.ToString());
                _timer.Enabled = false;

                Process();

                _timer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["TimeInterval"]);
                _timer.Enabled = true;
                clsLog.LogInfo("Timer will check back at " + DateTime.Now.AddMilliseconds(_timer.Interval).ToString());
            }
            catch (Exception ex)
            {
                clsLog.LogError(ex.ToString());
            }
        }

        protected override void OnStop()
        {
            //threadGet.Abort();
            //threadPush.Abort();
            for (int i = 0; i < threadArray.Count(); i++)
            {
                threadArray[i].Abort();
            }
            clsLog.Log("Service stopped");
        }

        public void ProcessImageGet()
        {
            clsLog.Log("Starting a new thread...");

            threadGet = new System.Threading.Thread(th =>
            {
                var data = new DynData.LKQ.LKQImageClient();
                while (true)
                {
                    clsLog.LogInfo("Starting Image Get thread...");

                    data.GetImageData();

                    clsLog.LogInfo("Sleeping Image Get thread...");
                    System.Threading.Thread.Sleep(10000);
                }
            });
        }

        public void ProcessImagePush()
        {
            threadPush = new System.Threading.Thread(th =>
            {
                var data = new DynData.LKQ.LKQImageClient();

                while (true)
                {
                    clsLog.LogInfo("Starting Image Push thread...");

                    data.PushImageData();

                    clsLog.LogInfo("Sleeping Image Push thread...");
                    System.Threading.Thread.Sleep(2000);
                }
            });
            threadPush.Start();
        }

        public void RunImageThreads()
        {
            threadArray = new System.Threading.Thread[10];
            for (int i = 0; i < threadArray.Count(); i++)
            {
                threadArray[i] = new System.Threading.Thread(th =>
                {
                    var data = new DynData.LKQ.LKQImageClient();
                    while (true)
                    {
                        clsLog.LogInfo("Starting a thread " + i + "...");

                        data.ProcessImage();

                        clsLog.LogInfo("Sleeping a thread " + i + "...");
                        System.Threading.Thread.Sleep(1000);
                    }
                });
            }
            for (int i = 0; i < threadArray.Count(); i++)
            {
                threadArray[i].Start();
            }

        }

        public void Process()
        {
            try
            {
                //You don't start process, here
                //Read next run date and time from local DB and set timer elapse time

                //check next download date for Data & Image from config and run download
                clsLog.LogInfo("Reading next run date and time ...");
                if (IsNextRun("LKQGet") == 1)
                    new DynData.LKQ.LKQClient().GetData();

                if (IsNextRun("IAAGet") == 1)
                    new DynData.IAA.IAAClient().GetData();

                if (IsNextRun("LKQPush") == 1)
                {
                    new DynData.LKQ.LKQClient().PushData();
                    new DynData.LKQ.LKQImageClient().SetHighResData();
                }

                if (IsNextRun("Cleanup") == 1)
                    new DynData.LKQ.LKQClient().Cleanup();

                clsLog.LogInfo("Sleeping....");

            }
            catch (Exception ex)
            {
                clsLog.LogError(ex.ToString());
            }
        }

        private int IsNextRun(string job)
        {
            try
            {
                var objRet = clsDB.funcExecuteSQLScalar("SP_NextJob '" + job + "'", ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
                return Convert.ToInt16(objRet);
            }
            catch (Exception ex)
            {
                clsLog.LogError(ex.ToString());
                return 0;
            }
        }

    }
}
