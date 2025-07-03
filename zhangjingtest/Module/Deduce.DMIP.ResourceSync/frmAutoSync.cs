using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;
using Deduce.DMIP.ResourceManage;
using Deduce.DMIP.ResourceSync.Server;
using Deduce.DMIP.Sys.SysData;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Deduce.DMIP.ResourceSync
{
    public partial class frmAutoSync : frmResourceService
    {
        BaseSyncService _syncSvc = null;

        // 属性注入 SBSyncService
        public SBSyncService SyncSvc { get; set; }

        public frmAutoSync()
        {
            InitializeComponent();
            base.BoxMessage = txtMsg;
        }

        private bool InitData()
        {
            _sysException = new CaptureException();
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(_sysException.UnhandledException);
            Application.ThreadException += new ThreadExceptionEventHandler(_sysException.ThreadException);
            try
            {
                UploadResourceType uploadType = ServiceSetting.UploadType;
                if (uploadType == UploadResourceType.ThirdBoard || uploadType == UploadResourceType.Nothing)
                {
                    this.Text = "三板资源同步服务";
                }
                CheckQuit();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
            return false;
        }

        private void frmAutoSync_Shown(object sender, EventArgs e)
        {
            if (!InitData())
                return;

            SyncSvc?.Startup();
        }

        private void frmAutoSync_FormClosing(object sender, FormClosingEventArgs e)
        {
            Quit(e);
        }
    }
}