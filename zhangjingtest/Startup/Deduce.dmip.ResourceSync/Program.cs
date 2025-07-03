using Deduce.Common.Entity;
using Deduce.Common.Utility;
using Deduce.DMIP.ResourceSync.Server;
using DMP.Common.Framework;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Deduce.DMIP.ResourceSync
{
    static class Program
    {
        internal static IBootStrapper bootStrapper;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        static void Main(string[] args)
        {
            try
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                if (Aide.IsRunning())
                    return;

                ThreadPool.SetMaxThreads(1000, 10000);
                Utils.WriteLog("启动AutoSync,请等候...");
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
                Application.ThreadException += new ThreadExceptionEventHandler(ThreadException);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                bootStrapper = new AutofacBootStrapper(args);
                bootStrapper.StartServiceAsync().Wait();            
                var sbSyncService = bootStrapper.ServiceProvider.GetRequiredService<SBSyncService>();
                var frmAutoSync = bootStrapper.ServiceProvider.GetRequiredService<frmAutoSync>();
                frmAutoSync.SyncSvc = sbSyncService;
                sbSyncService.FrmSvc = frmAutoSync;
                Application.Run(frmAutoSync);
            }
            catch (Exception ex)
            {
                Utils.WriteLog("SEH异常：" + ex.Message + ex.StackTrace);
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        private static void ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            StringBuilder sb = new StringBuilder();
            sb.Append(ex.StackTrace + Environment.NewLine);
            sb.Append("InnerException" + Environment.NewLine);
            sb.Append(ex.InnerException.StackTrace + Environment.NewLine);
            sb.Append("Source" + Environment.NewLine);
            sb.Append(ex.Source + Environment.NewLine);
            Utils.WriteLog("AutoSync main ThreadException:" + sb.ToString());
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            StringBuilder sb = new StringBuilder();
            sb.Append(ex.StackTrace + Environment.NewLine);
            sb.Append("InnerException" + Environment.NewLine);
            sb.Append(ex.InnerException.StackTrace + Environment.NewLine);
            sb.Append("Source" + Environment.NewLine);
            sb.Append(ex.Source + Environment.NewLine);
            Utils.WriteLog("AutoSync main UnhandledException:" + sb.ToString());
        }
    }
}