using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using Deduce.Common.Entity;
using Deduce.Common.Utility;
using Deduce.DMIP.ResourceManage;
using Deduce.DMIP.Sys.SysData;

namespace Deduce.DMIP.ResourceSync
{
    public class AStockToOssService
    {
        AStockSyncHelper _uploadHelper = null;
        OssOperate _toOSS = new OssOperate();
        Dictionary<string, TableDesign> _formulas = new Dictionary<string, TableDesign>();
        Dictionary<string, SBGGSync> _queues = new Dictionary<string, SBGGSync>();
        frmAutoSync _frmSvc = null;
        int _runNums = 0;
        object _objLock = new object();

        public AStockToOssService(AStockSyncHelper helper, frmAutoSync frm)
        {
            this._uploadHelper = helper;
            this._frmSvc = frm;
        }
        public void Startup()
        {
            Thread oss = new Thread(new ThreadStart(Circulation));
            oss.IsBackground = true;
            oss.Start();
        }
        private void Circulation()
        {
            while (true)
            {
                try
                {
                    _uploadHelper.GetILeagalData();
                    _formulas = _uploadHelper.LoadFormulas(((int)ArchivingType.ManualUploadTableRule).ToString());
                    Display("正加载需上传的清单，请稍等...");
                    DataTable dtSource = _uploadHelper.LoadToOssResource(SyncStatus.Nothing);
                    if (dtSource == null)
                        continue;

                    int recordNum = dtSource.Rows.Count;
                    ToOSS(dtSource);

                    dtSource = _uploadHelper.LoadToOssResource(SyncStatus.Runing);
                    if (dtSource != null && dtSource.Rows.Count != 0)
                    {
                        recordNum += dtSource.Rows.Count;
                        ToOSS(dtSource);
                    }
                    //_uploadHelper.CheckResourceSync(recordNum.ToString());暂时不需要检查，MD5是否已经存在
                    Display("正加载需转换的历史记录，请稍等...");
                    dtSource = _uploadHelper.LoadConvertResource(SyncStatus.Success, ToTextFlag.Original);
                    if (dtSource != null && dtSource.Rows.Count != 0)
                    {
                        int conNum = dtSource.Rows.Count;
                        foreach (DataRow drRes in dtSource.Rows)
                        {
                            _uploadHelper.ConvertHisTxt(drRes);
                            Display(drRes["storePath"].ToString() + ",正在转换...，还有" + (--conNum) + "个 ");
                        }
                    }
                    Display("本次上传全部结束!");
                }
                catch (Exception ex)
                {
                    Utils.WriteLog("ToOSS 出现异常！" + ex.Message + ex.StackTrace);
                }
                _uploadHelper.SetSleep();
            }
        }
        private List<string> GetIDs(int maxNums)
        {
            List<string> ids = new List<string>();
            int numIDs = 20;
            if (maxNums <= numIDs)
            {
                ids = _uploadHelper.GetIDs(maxNums);
            }
            else
            {
                ids = _uploadHelper.GetIDs(numIDs);
            }
            return ids;
        }

        private void ToOSS(DataTable dtSource)
        {
            if (Utils.IsEmpty(dtSource))
                return;

            DataTable dtTemp = dtSource.Clone();
            List<string> ids = new List<string>();
            while (dtSource.Rows.Count > 0)
            {
                if (ids.Count == 0)
                {
                    ids = GetIDs(dtSource.Rows.Count);
                }
                if (ids.Count == 0)
                    return;

                try
                {
                    DataRow dr = dtSource.Rows[0];
                    string objID = dr["ob_object_id"].ToString();
                    DataRow drRes = dtTemp.NewRow();
                    drRes.ItemArray = dr.ItemArray;
                    dtTemp.Rows.Add(drRes);
                    dtSource.Rows.RemoveAt(0);
                    string md5 = GetMD5(drRes);
                    if (_queues.ContainsKey(md5))
                        continue;

                    drRes["GGID"] = ids[0];
                    ids.RemoveAt(0);
                    Display(drRes["storePath"].ToString() + ",正上传... " + dtSource.Rows.Count.ToString() + " 需上传");
                    SBGGSync syncData = new SBGGSync(drRes, SyncStatus.Runing, md5);
                    ToOSS(drRes, syncData, _formulas);
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
        private void ToOSS(DataRow drRes, SBGGSync syncData, Dictionary<string, TableDesign> formula)
        {
            //Thread t = new Thread((ThreadStart)delegate()
            //{
            PutOSS(drRes, syncData, formula);
            //});
            //t.IsBackground = true;
            //t.Start();
            Counter(true);
            Thread.Sleep(33);
            Waiting(6);
        }
        private void PutOSS(DataRow drRes, SBGGSync syncData, Dictionary<string, TableDesign> formula)
        {
            try
            {
                string ggrq = drRes["ggrq"].ToString();
                string objKey = _uploadHelper.CreateObjectKey(drRes["GGID"].ToString(), drRes["extension"].ToString().ToUpper(), ggrq);
                string fromPath = drRes["storePath"].ToString();
                string ossMd5 = "";
                bool success = _toOSS.Put(fromPath, objKey, out ossMd5, syncData);
                if (!success)
                    return;

                ossMd5 = ossMd5.ToUpper();
                if (ossMd5 != syncData.MD5)
                {
                    drRes["MD5"] = syncData.MD5 = Utils.GetHash(File.ReadAllBytes(fromPath), "x2").ToUpper();
                }
                if (ossMd5 != syncData.MD5)
                    return;

                syncData.Status = SyncStatus.Success;
                _uploadHelper.WriteSync(syncData.Status, syncData.Row, formula);

                //下面是将 文件写入共享目录-写入需要权限--2016-07-22
                string toFile = GlobalData.BackupPath + objKey.Replace("/", @"\");
                if (!Utils.CreateDir(toFile))
                {
                    Utils.WriteLog("创建 " + toFile + " 路径失败，请检查共享目录的权限设置！");
                    return;
                }
                File.Copy(fromPath, toFile);
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
            _frmSvc.WriteMsg(msg, LogOut.ToScreen);
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
