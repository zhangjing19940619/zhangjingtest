using Aliyun.OSS.Common;
using Deduce.Common.Entity;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;
using Deduce.DMIP.Business.WebAnalyze;
using Deduce.DMIP.ResourceManage;
using Deduce.DMIP.Sys.OperateData;
using Deduce.DMIP.Sys.SysData;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Deduce.DMIP.ResourceSync.Server
{
    public class SBSyncToOssService
    {
        OssOperate _toOSS = null;
        Dictionary<string, List<SBGGFLZYB>> _gglb = new Dictionary<string, List<SBGGFLZYB>>();
        Dictionary<string, ResourceSyncRule> _formulas = new Dictionary<string, ResourceSyncRule>();
        Dictionary<string, SBGGSync> _queues = new Dictionary<string, SBGGSync>();        
        int _runNums = 0;
        object _objLock = new object();
        List<string> _repeats = new List<string>();
        private readonly SBSyncHelper _sbSync;
        private readonly frmAutoSync _frmSvc;
        private readonly ILogger<SBSyncToOssService> _logger;
        private readonly CaptureException _sysException;

        public SBSyncToOssService(CaptureException ex, SBSyncHelper sbSync, frmAutoSync frmSvc, ILogger<SBSyncToOssService> logger)
        {
            _sysException = ex;
            _sbSync = sbSync;
            _frmSvc = frmSvc;
            _logger = logger;
            _toOSS = new OssOperate();
        }

        public void Startup()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    CheckOssExist();//检测oss文件存在性
                    //_sbSync.CheckBackupResource(false);
                    _sbSync.CheckResourceSyncStatus();
                    Thread.Sleep(60 * 1123);
                }
            });
           
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Circulation();
                }
            });
        }

        private void Circulation()
        {
            _repeats.Clear();
            if (_sbSync.Suspend)
                return;

            try
            {
                Display("正加载需上传的清单，请稍等...");
                _gglb = _sbSync.BuildFLZYB();
                _formulas = _sbSync.LoadFormulas("3", "");
                //先处理上传成功，但分类不成功的资源
                RunCategory();
                DataTable dtSource = _sbSync.LoadToOssResource(SyncStatus.Nothing);
                if (dtSource == null)
                {
                    _sbSync.Suspend = AutoHelper.Quit = true;
                    return;
                }
                int recordNum = dtSource.Rows.Count;
                ToOSS(dtSource);
                dtSource = _sbSync.LoadToOssResource(SyncStatus.Runing);
                if (dtSource != null)
                {
                    recordNum += dtSource.Rows.Count;
                    ToOSS(dtSource);
                }
                _sbSync.CheckResourceSync(recordNum.ToString());
                Display("本次上传全部结束!");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("ToOSS 出现异常！" + ex.Message + ex.StackTrace);
            }
            AutoHelper.SetSleep(311, 3222);
        }      

        /// <summary>
        /// 检查 OSS 中的资源是否存在
        /// </summary>
        public void CheckOssExist()
        {
            string sql = GetQuerySql();
            DataTable dtSource = OperateData.Instance.GetDataTable(sql);

            if (Utils.IsEmpty(dtSource))
                return;

            Parallel.ForEach(dtSource.AsEnumerable(), new ParallelOptions { MaxDegreeOfParallelism = 10 }, CheckResourceExistence);
        }

        /// <summary>
        /// 构建查询 SQL 语句
        /// </summary>
        /// <returns>SQL 语句</returns>
        private string GetQuerySql()
        {
            return $@"select t2.ggrq,t2.extension,t1.* from cfg.dmip_ResourceSync t1 
                inner join cfg.dmip_Resource t2 on t1.ob_object_id=t2.ob_object_id 
                where t1.tableName='usrSBGGB' and t1.status='2' and (t1.ossExist is null or t1.ossExist=0) 
                and t1.publishDate >= '{DateTime.Now.AddDays(-1).ToString(GlobalData.DateFormat)}' 
                order by t1.publishDate";
        }

        /// <summary>
        /// 检查单个资源在 OSS 中是否存在
        /// </summary>
        /// <param name="drRes">数据行</param>
        private void CheckResourceExistence(DataRow drRes)
        {
            string objKey = _sbSync.CreateObjectKey(drRes["GGID"].ToString(), drRes["extension"].ToString().ToUpper(), drRes["ggrq"].ToString());
            bool exist = _toOSS.Exist(objKey);

            if (exist)
            {
                UpdateResourceSyncStatus(drRes["ob_object_id"].ToString(), 1);
            }
            else
            {
                HandleMissingResource(drRes);
            }
        }

        /// <summary>
        /// 更新资源同步状态
        /// </summary>
        /// <param name="objectId">资源对象ID</param>
        /// <param name="status">状态值</param>
        private void UpdateResourceSyncStatus(string objectId, int status)
        {
            string sql = $"update cfg.dmip_ResourceSync set ossExist = {status} where ob_object_id = '{objectId}'";
            OperateData.Instance.ExecuteNonQuery(sql, GlobalData.CommonMenuID, ConnectType.Rule);
        }

        /// <summary>
        /// 处理在 OSS 中缺失的资源
        /// </summary>
        /// <param name="drRes">数据行</param>
        private void HandleMissingResource(DataRow drRes)
        {
            Utils.WriteLog($"检测到 OSS 公告不存在，执行补公告：{drRes["GGID"]}", true);

            string sql = $"select 0 from usrSBGGB where ID = {drRes["GGID"]}";
            DataTable dtSBGG = OperateData.Instance.GetDataTable(sql);

            if (dtSBGG != null && dtSBGG.Rows.Count > 0)
            {
                RcHelper.SyncFastDfsToOss(
                    drRes["GGID"].ToString(),
                    drRes["ob_object_id"].ToString(),
                    "JYNQ", "usrSBGGB",
                    Convert.ToDateTime(drRes["ggrq"]),
                    drRes["extension"].ToString().ToUpper().Trim('.')
                );
            }
        }

        public bool ToOSSExtend(DataTable dtSource)
        {
            if (Utils.IsEmpty(dtSource))
                return false;

            List<string> ids = new List<string>();
            if (ids.Count == 0)
            {
                ids = _sbSync.GetIDs(dtSource.Rows.Count);
            }
            if (ids.Count == 0)
                return false;

            try
            {
                DataTable dtRes = dtSource.Clone();
                DataRow dr = dtSource.Rows[0];
                DataRow drRes = dtRes.NewRow();
                drRes.ItemArray = dr.ItemArray;
                dtRes.Rows.Add(drRes);
                dtSource.Rows.RemoveAt(0);

                bool success = CheckFile(drRes);
                if (!success)
                {
                    return false;
                }
                string md5 = GetMD5(drRes);
                if (_queues.ContainsKey(md5))
                    return false;

                drRes["GGID"] = ids[0];
                ids.RemoveAt(0);

                SBGGSync sync = new SBGGSync(drRes, SyncStatus.Runing, md5);
                ToOSS(dtRes, sync);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        private void ToOSS(DataTable dtSource)
        {
            if (Utils.IsEmpty(dtSource))
                return;

            List<string> ids = new List<string>();
            while (dtSource.Rows.Count > 0)
            {
                if (ids.Count == 0)
                {
                    ids = _sbSync.GetIDs(dtSource.Rows.Count);
                }
                if (ids.Count == 0)
                    return;

                try
                {
                    DataTable dtRes = dtSource.Clone();
                    DataRow dr = dtSource.Rows[0];
                    DataRow drRes = dtRes.NewRow();
                    drRes.ItemArray = dr.ItemArray;
                    dtRes.Rows.Add(drRes);
                    dtSource.Rows.RemoveAt(0);

                    bool success = CheckFile(drRes);
                    if (!success)
                    {
                        Display(drRes["storePath"].ToString() + ",上传失败,文件不存在！");
                        continue;
                    }
                    string md5 = GetMD5(drRes);
                    if (_queues.ContainsKey(md5))
                        continue;

                    drRes["GGID"] = ids[0];
                    ids.RemoveAt(0);
                    Display(drRes["storePath"].ToString() + ",正上传... " + dtSource.Rows.Count.ToString() + " 需上传");
                    SBGGSync sync = new SBGGSync(drRes, SyncStatus.Runing, md5);
                    ToOSS(dtRes, sync);
                }
                catch (Exception ex)
                {
                    Utils.WriteLog("SyncToOss ToOSS错误:" + ex.Message);
                }
            }
            //Waiting(1);
        }

        private bool CheckFile(DataRow drRes)
        {
            string storePath = drRes["storePath"].ToString();
            var result = RCFastDfs.Init.DownLoad(drRes["ob_object_id"].ToString());
            if (result.Success)
            {
                Utils.CreateDir(storePath);
                if (File.Exists(storePath))
                    return true;
                File.Move(result.Result, storePath);
            }
            UriResource res = new UriResource();
            res.DownloadUri = new Uri(drRes["resourceURL"].ToString());
            res.Extension = drRes["extension"].ToString();
            string error = "";
            res.ResourceType = WebParser.ParserHead(res, "");
            object obj = WebParser.GetData(res, HeadersType.Content_Disposition, false, null, ref error);
            if (obj == null)
                return false;

            res.FullName = storePath;
            res.Content = obj;
            if (res.ResourceType == ResourceType.Html)
            {
                return res.Store();
            }
            else if (res.ResourceType == ResourceType.Attachment)
            {
                //如果为空，则默认为PDF，下载的资源大多数是PDF格式的文档,如果确实没有扩展名的，
                //强制设置为PDF，则会存在问题。
                if (Utils.IsEmpty(res.Extension))
                    res.Extension = ".pdf";

                string fullName = res.FullName.ToLower();
                if (!fullName.EndsWith(res.Extension))
                {
                    res.FullName = res.FullName + res.Extension;
                }
                return res.Store();
            }
            return false;
        }


        private void Waiting(int maxNum)
        {
            while (_runNums > maxNum)
            {
                Thread.Sleep(77);
            }
        }

        private void ToOSS(DataTable dtRes, SBGGSync sync)
        {
            DataRow drRes = dtRes.Rows[0];
            string code = "";
            string inbbm = "";
            string igsdm = "";
            sync.Status = _sbSync.CheckCode(drRes, ref inbbm, ref igsdm, ref code);
            if (sync.Status == SyncStatus.CodeNull)
            {
                _sbSync.WriteSync(sync.Status, sync.Row, null);
                return;
            }
            if (_sbSync.QueryMD5(sync.MD5))
            {
                sync.Status = SyncStatus.MD5Exists;
                _sbSync.WriteSync(sync.Status, sync.Row, null);
                return;
            }

            code = Convert.ToDateTime(drRes["ggrq"].ToString()).ToString(GlobalData.DateFormat)
                + igsdm + inbbm + drRes["title"];

            if (_repeats.Contains(code))
                return;

            _repeats.Add(code);
            Dictionary<string, List<SBGGFLZYB>> cates = _sbSync.BuildCategory(drRes, _gglb, _formulas);
            bool used = true;
            DataTable dtTemp = dtRes.Copy();
            DataRow drTemp = dtTemp.Rows[0];
            _queues.Add(sync.MD5, sync);

            if (!used)
            {
                PutOSS(drTemp, sync, cates);
            }
            else
            {
                Thread t = new Thread((ThreadStart)delegate ()
                {
                    PutOSS(drTemp, sync, cates);
                });
                t.IsBackground = true;
                t.Start();
            }
            Counter(true);
            //Thread.Sleep(333);
            Waiting(10);
        }

        private void Counter(bool isAdd)
        {
            lock (_objLock)
            {
                _runNums = (isAdd) ? _runNums++ : _runNums--;
            }
        }

        /// <summary>
        /// 将数据行上传到OSS
        /// </summary>
        /// <param name="drRes">数据行</param>
        /// <param name="sync">同步对象</param>
        /// <param name="cates">资源分类字典</param>
        private void PutOSS(DataRow drRes, SBGGSync sync, Dictionary<string, List<SBGGFLZYB>> cates)
        {
            string md5 = sync.MD5;
            try
            {
                Dictionary<string, List<SBGGFLZYB>> cloneCates = Utils.CreateDeepCopy(cates);

                string objKey = GenerateOSSObjectKey(drRes);
                string formPath = drRes["storePath"].ToString();

                if (!UploadToOSS(drRes, formPath, objKey, sync))
                    return;

                sync.Status = SyncStatus.Success;
                _sbSync.WriteSync(sync.Status, sync.Row, cloneCates);

                if (IsTestEnvironment())
                {
                    return;
                }

                // 共享目录后面会废弃，是否继续保留此功能需确定
                // BackupFile(formPath, objKey);

                //_sbSync.SendToResourceCenterKafka(drRes);
            }
            catch (Exception ex)
            {
                Utils.WriteLog($"PutOSS 错误！{ex.Message} {ex.StackTrace}");
            }
            finally
            {
                CleanupAfterUpload(md5);
            }
        }

        /// <summary>
        /// 根据数据行生成OSS对象键
        /// </summary>
        /// <param name="drRes">数据行</param>
        /// <returns>OSS对象键</returns>
        private string GenerateOSSObjectKey(DataRow drRes)
        {
            string ggrq = drRes["ggrq"].ToString();
            string ggId = drRes["GGID"].ToString();
            string extension = drRes["extension"].ToString().ToUpper();
            return _sbSync.CreateObjectKey(ggId, extension, ggrq);
        }

        /// <summary>
        /// 上传文件到OSS
        /// </summary>
        /// <param name="drRes">数据行</param>
        /// <param name="formPath">文件路径</param>
        /// <param name="objKey">OSS对象键</param>
        /// <param name="sync">同步对象</param>
        /// <returns>是否成功</returns>
        private bool UploadToOSS(DataRow drRes, string formPath, string objKey, SBGGSync sync)
        {
            bool success = _sbSync.SyncFast2Oss(drRes);

            if (!success)
            {
                RCFastDfs.Init.Upload(formPath, drRes["ob_object_id"].ToString());
                success = _sbSync.SyncFast2Oss(drRes);
            }

            return success;
        }

        /// <summary>
        /// 判断当前环境是否为测试环境
        /// </summary>
        /// <returns>是否为测试环境</returns>
        private bool IsTestEnvironment() =>
            ServiceSetting.City == ServerCity.Nothing ||
            ServiceSetting.City == ServerCity.ShTest ||
            ServiceSetting.City == ServerCity.HzTest;

        /// <summary>
        /// 在文件上传完成后进行清理工作
        /// </summary>
        /// <param name="md5">文件的MD5值</param>
        private void CleanupAfterUpload(string md5)
        {
            Counter(false);
            if (_queues.ContainsKey(md5))
            {
                _queues.Remove(md5);
            }
        }


        private void Backup(string formPath, string toFile)
        {
            try
            {
                File.Copy(formPath, toFile);
            }
            catch (Exception ex)
            {
                Utils.WriteLog("ToOSS 本地备份错误！" + ex.Message + ex.StackTrace);
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

        public void RunCategory()
        {
            DataTable dt = _sbSync.QuerySBGGLB();
            if (Utils.IsEmpty(dt))
                return;

            List<int> status = new List<int>()
            {
                2,
                4,
                0
            };

            foreach (DataRow dr in dt.Rows)
            {
                DataTable dt1 = null;
                foreach (int s in status)
                {
                    dt1 = _sbSync.LoadToOssResource((SyncStatus)s, dr["HASHCODE"].ToString());
                    if (dt1 != null && dt1.Rows.Count > 0)
                        break;
                }
                if (Utils.IsEmpty(dt1))
                    continue;

                DataRow drRes = dt1.Rows[0];
                string ggid = dr["ID"].ToString();
                drRes["GGID"] = ggid;
                Dictionary<string, List<SBGGFLZYB>> cates = _sbSync.BuildCategory(drRes, _gglb, _formulas);
                string lbbm = "";
                if (_sbSync.WriteLBB(drRes, ggid, cates, ref lbbm))
                {
                    _sbSync.WriteResourceSync(SyncStatus.Success, drRes, lbbm);
                }
            }
        }

        private void Display(string msg)
        {
            _frmSvc.WriteMsg(msg, LogOut.ToScreen);
        }

    }
}