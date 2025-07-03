using System;
using System.Data;
using System.Threading;
using Deduce.Common.Utility;
using Deduce.DMIP.ResourceManage;

namespace Deduce.DMIP.ResourceSync
{
    public class HKStockUploadService : BaseSyncService
    {
        frmAutoSync _frmAutoSync = null;
        CaptureException _sysException = null;
        HKStockSyncHelper _hkSyncHelper = new HKStockSyncHelper();
        HKStockToOSSService _toOss = null;
        public HKStockUploadService(frmAutoSync frm, CaptureException ex)
        {
            _frmAutoSync = frm;
            _sysException = ex;
            _toOss = new HKStockToOSSService(_hkSyncHelper, frm);
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
                Utils.WriteLog("HKSyncService Satrup 异常！" + ex.Message + ex.StackTrace);
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
                _hkSyncHelper.LoadGGZQZB();
                DataTable dtResource = _hkSyncHelper.LoadResource();
                if (dtResource != null && dtResource.Rows.Count != 0)
                {
                    MatchCode(dtResource);
                }
                _hkSyncHelper.SetSleep();
            }
        }
        private void MatchCode(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    _hkSyncHelper.SaveSyncData(dr);
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
