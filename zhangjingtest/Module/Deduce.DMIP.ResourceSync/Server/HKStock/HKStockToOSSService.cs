using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Deduce.Common.Entity;
using Deduce.Common.Utility;
using Deduce.DMIP.ResourceManage;
using Deduce.DMIP.Sys.SysData;

namespace Deduce.DMIP.ResourceSync
{
    public class HKStockToOSSService
    {
        HKStockSyncHelper _hkSyncHelper = null;
        OssOperate _toOSS = new OssOperate();
        Dictionary<string, TableDesign> _formulas = new Dictionary<string, TableDesign>();
        frmAutoSync _frmAutoSync = null;
        int _runNums = 0;
        object _objLock = new object();

        public HKStockToOSSService(HKStockSyncHelper helper, frmAutoSync frm)
        {
            this._hkSyncHelper = helper;
            this._frmAutoSync = frm;
        }
        public void Startup()
        {
            Thread oss = new Thread(Circulation);
            oss.IsBackground = true;
            oss.Start();
        }
        private void Circulation()
        {
            while (true)
            {
                try
                {
                    _formulas = _hkSyncHelper.LoadFormulas(((int)ArchivingType.ManualUploadTableRule).ToString());
                    Display("正加载需上传的清单，请稍等...");
                    DataTable dtResource = _hkSyncHelper.LoadToOssResource(SyncStatus.Nothing, ToTextFlag.Original);
                    ToOSS(dtResource);

                    dtResource = _hkSyncHelper.LoadToOssResource(SyncStatus.Runing, ToTextFlag.Original);
                    ToOSS(dtResource);

                    Display("正在加载需转换的历史记录，请稍等...");
                    DataTable dtHisRes = _hkSyncHelper.LoadToOssResource(SyncStatus.Success, ToTextFlag.Original);
                    if (dtHisRes != null && dtHisRes.Rows.Count != 0)
                    {
                        int count = dtHisRes.Rows.Count;
                        foreach (DataRow dr in dtHisRes.Rows)
                        {
                            _hkSyncHelper.LoadHisResource(dr);
                            Display(dr["storePath"].ToString() + ",正在转换... " + "还有 " + (--count) + " 个需上传");
                        }
                    }
                    Display("本次上传全部结束!");
                }
                catch (Exception ex)
                {
                    Utils.WriteLog("ToOSS 出现异常！" + ex.Message + ex.StackTrace);
                }
                _hkSyncHelper.SetSleep();
            }
        }
        private List<string> GetIDs(int maxNums)
        {
            List<string> ids = new List<string>();
            int numIDs = 20;
            if (maxNums <= numIDs)
            {
                ids = _hkSyncHelper.GetIDs(maxNums);
            }
            else
            {
                ids = _hkSyncHelper.GetIDs(numIDs);
            }
            return ids;
        }
        private void ToOSS(DataTable dtResource)
        {
            if (Utils.IsEmpty(dtResource))
                return;

            DataTable dtTemp = dtResource.Clone();
            List<string> ids = new List<string>();
            while (dtResource.Rows.Count > 0)
            {
                if (ids.Count == 0)
                {
                    ids = GetIDs(dtResource.Rows.Count);
                }
                if (ids.Count == 0)
                    return;

                try
                {
                    DataRow dr = dtResource.Rows[0];
                    DataRow drRes = dtTemp.NewRow();
                    drRes.ItemArray = dr.ItemArray;
                    dtTemp.Rows.Add(drRes);
                    dtResource.Rows.RemoveAt(0);
                    string md5 = GetMD5(drRes);
                    drRes["GGID"] = ids[0];
                    ids.RemoveAt(0);
                    Display(drRes["storePath"].ToString() + ",正在上传... " + "还有 " + dtResource.Rows.Count.ToString() + " 个需上传");
                    SBGGSync syncData = new SBGGSync(drRes, SyncStatus.Runing, md5);
                    PutOSS(drRes, syncData, _formulas);
                }
                catch (Exception ex)
                {
                    Utils.WriteLog("SyncToOss ToOSS错误:" + ex.Message);
                }
            }
            Waiting(1);
        }
        private void Waiting(int maxNum)
        {
            while (_runNums > maxNum)
            {
                Thread.Sleep(77);
            }
        }
        private string GetMD5(DataRow drResource)
        {
            string md5 = drResource["md5"].ToString();
            if (Utils.IsEmpty(md5))
            {
                drResource["md5"] = md5 = Utils.GetFileMD5(drResource["storePath"].ToString());
            }
            return md5;
        }

        private void PutOSS(DataRow drRes, SBGGSync syncData, Dictionary<string, TableDesign> formula)
        {
            try
            {
                string ggrq = drRes["ggrq"].ToString();
                string objKey = _hkSyncHelper.CreateObjectKey(drRes["GGID"].ToString(), drRes["extension"].ToString().ToUpper(), ggrq);
                string fromPath = drRes["storePath"].ToString();
                string ossMd5 = "";
                bool success = _toOSS.Put(fromPath, objKey, out ossMd5, syncData);
                if (!success)
                    return;

                ossMd5 = ossMd5.ToUpper();
                if (ossMd5 != syncData.MD5)
                    return;

                syncData.Status = SyncStatus.Success;
                DataTable dtMsg = new DataTable();
                _hkSyncHelper.WriteTable(syncData.Row, formula, true, ref dtMsg);
            }
            catch (Exception ex)
            {
                Utils.WriteLog("ToOSS 错误！" + ex.Message + ex.StackTrace);
            }
            finally
            {
                Counter(false);
            }
        }
        private void Display(string msg)
        {
            _frmAutoSync.WriteMsg(msg, LogOut.ToScreen);
        }
        private void Counter(bool isAdd)
        {
            lock (_objLock)
            {
                if (isAdd)
                {
                    _runNums++;
                }
                else
                {
                    _runNums--;
                }
            }
        }
    }
}
