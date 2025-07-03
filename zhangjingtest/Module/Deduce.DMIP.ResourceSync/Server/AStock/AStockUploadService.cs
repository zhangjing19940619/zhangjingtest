using System;
using System.Data;
using System.Threading;
using Deduce.Common.Utility;
using Deduce.DMIP.ResourceManage;

namespace Deduce.DMIP.ResourceSync
{
    public class AStockUploadService : BaseSyncService
    {
        frmAutoSync _frmSrv = null;
        CaptureException _sysException = null;
        AStockSyncHelper _aSyncHelper = new AStockSyncHelper();
        AStockToOssService _toOss = null;
        public AStockUploadService(frmAutoSync frm, CaptureException ex)
        {
            _frmSrv = frm;
            _sysException = ex;
            _toOss = new AStockToOssService(_aSyncHelper, frm);
        }
        public override void Startup()
        {
            try
            {
                Execute();
                _toOss.Startup();
            }
            catch (Exception ex)
            {
                Utils.WriteLog("SBSyncService Satrup 异常！" + ex.Message + ex.StackTrace);
            }
        }
        private void Execute()
        {
            Thread t = new Thread(Circulation);
            t.IsBackground = true;
            t.Start();
        }

        private void Circulation()
        {
            while (true)
            {
                _aSyncHelper.GetILeagalData();
                DataTable dtSource = _aSyncHelper.LoadResource();
                if (dtSource != null && dtSource.Rows.Count != 0)
                {
                    MatchCode(dtSource, true);
                }
                _aSyncHelper.LoadZQZB();
                //加载代码为空的记录
                dtSource = _aSyncHelper.LoadResourceSync();
                if (dtSource != null && dtSource.Rows.Count > 0)
                {
                    MatchCode(dtSource, false);
                }
                _aSyncHelper.SetSleep();
            }
        }
        private void MatchCode(DataTable dt, bool isCalcMd5)
        {
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    _aSyncHelper.SaveSyncData(dr, isCalcMd5);
                    Thread.Sleep(13);
                }
                catch (Exception ex)
                {
                    Utils.WriteLog("Circulation出现异常：" + ex.StackTrace);
                }
            }
        }
    }
}
