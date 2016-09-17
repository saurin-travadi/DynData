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

                clsLog.LogInfo("Service started");

                _timer.Interval = 500;
                _timer.Enabled = true;
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
            clsLog.LogInfo("Service stopped");
        }

        private void Process()
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
                    new DynData.LKQ.LKQClient().PushData();


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
