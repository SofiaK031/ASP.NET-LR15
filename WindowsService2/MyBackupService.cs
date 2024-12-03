using Quartz;
using Quartz.Impl;
using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace WindowsService2
{
    public partial class MyBackupService : ServiceBase
    {
        private static void LogEvent(string msg, EventLogEntryType eventLogType)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "MyBackupTest";
                eventLog.WriteEntry(msg, eventLogType, 101, 1);
            }
        }

        class MyJob : IJob
        {
            private void BackupTxtFromDesktop()
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string[] files = Directory.GetFiles(desktopPath, "*.txt");
                string backupDir = "D:/BackupDir";
                Directory.CreateDirectory(backupDir);

                foreach (string file in files)
                {
                    File.Copy(file, Path.Combine(backupDir, Path.GetFileName(file)), true);
                }

                LogEvent($"{files.Length} file(s) backed up. Directory for backup is '{backupDir}'.", EventLogEntryType.Information);
            }

            public Task Execute(IJobExecutionContext context)
            {
                BackupTxtFromDesktop();
                return Task.CompletedTask;
            }
        }

        IScheduler scheduler = new StdSchedulerFactory().GetScheduler().Result;

        public MyBackupService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            IJobDetail revokeJob = JobBuilder.Create<MyJob>()
            .WithIdentity("MyJob", "MyJobGroup")
            .Build();

            ITrigger revokeTrigger = TriggerBuilder.Create()
            .WithIdentity("MyTrigger", "MyJobGroup")
            .StartNow()
            .WithCronSchedule("0 0/1 * * * ?")
            .Build();

            scheduler.ScheduleJob(revokeJob, revokeTrigger);
            scheduler.Start();
        }

        protected override void OnStop()
        {
            scheduler.Shutdown();
        }
    }
}