using Aliyun.OSS.Common;
using Deduce.Common.Utility;
using Deduce.DMIP.ResourceManage;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Deduce.DMIP.ResourceSync.Server
{
    public class SBSyncService : BaseSyncService
    {
        private readonly SyncSBFL _sySBFL;
        private readonly SBSyncHelper _sbSync;
        private readonly SBSyncToOssService _toOss;
        private readonly CaptureException _sysException;
        private readonly ILogger<SBSyncService> _logger;

        // 属性注入 frmAutoSync
        public frmAutoSync FrmSvc { get; set; }

        public SBSyncService(CaptureException ex, SBSyncHelper sbSync, SBSyncToOssService toOss, ILogger<SBSyncService> logger)
        {
            _sysException = ex;
            _sbSync = sbSync;
            _toOss = toOss;
            _logger = logger;
            _sySBFL = new SyncSBFL();
        }

        public override void Startup()
        {
            try
            {
                _logger.LogInformationWithProps(new { type = "三板资源同步"}, "SBSyncService.Startup");

                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Circulation();
                    }
                });
                _toOss.Startup();

                //同步三板公告文本表
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        var fileName = (Convert.ToInt32(Math.Floor(DateTime.Now.ToOADate())) - 2).ToString();
                        var path = @"D:\XMLFile\" + fileName;
                        if (Directory.Exists(path))
                        {
                            DirectoryInfo di = new DirectoryInfo(path);
                            di.Delete(true);
                        }
                        SBWBSync sync = new SBWBSync();
                        sync.SyncExecute(FrmSvc);
                        Thread.Sleep(8000);
                    }
                });
            }
            catch (Exception ex)
            {
                Utils.WriteLog("SBSyncService Satrup 异常！" + ex.Message + ex.StackTrace);
            }
        }

        private void MatchCode(DataTable dt, bool isCalcMd5)
        {
            if (Utils.IsEmpty(dt))
                return;

            foreach (DataRow dr in dt.Rows)
            {
                _sbSync.SaveSyncData(dr, isCalcMd5);
                Thread.Sleep(13);
            }
        }

        private void Circulation()
        {
            try
            {
                DataTable dt = _sbSync.LoadResource();
                if (_sbSync.Suspend)
                    return;

                MatchCode(dt, true);
                _sbSync.LoadZQZB();
                dt = _sbSync.LoadResourceSync();
                MatchCode(dt, false);
                AutoHelper.SetSleep(7311, 35222);
            }
            catch (Exception ex)
            {
                Utils.WriteLog("SBSyncService Circulation异常：" + ex.Message + ex.StackTrace);
            }
        }

        private void Circulation_WBB()
        {
            try
            {
                //var condition = "";
                //SBGGWBBSyncService service = new SBGGWBBSyncService();
                //service.GetCondition(out condition);
                //service.SaveDataNew(condition);
            }
            catch (Exception ex)
            {
                Utils.WriteLog("SBSyncService Circulation_WBB异常：" + ex.Message + ex.StackTrace);
            }
        }
    }
}