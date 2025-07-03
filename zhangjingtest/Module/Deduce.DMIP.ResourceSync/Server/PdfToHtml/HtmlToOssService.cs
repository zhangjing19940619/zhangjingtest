using Aliyun.OSS;
using Deduce.Common.Entity;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;
using Deduce.DMIP.ResourceManage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Deduce.DMIP.ResourceSync
{
    /// <summary>
    /// 功能：将pdf文件转换成的html文件上传到oss中（目前为在小梵中展示服务）
    /// </summary>
    class HtmlToOssService : BaseSyncService
    {
        HtmlSyncHelper _htmlSync = null;
        OssClient _oss;
        OssSetting _set = null;
        string _fromPath = @"\\vm-fileserver\FILEDATA\JYPRIME\usrGSGGYWFWB\";
        string _toPath = @"\\vm-zdhjg64\resource\json\";
        frmAutoSync frmSvc = null;

        public HtmlToOssService(frmAutoSync frm)
        {
            _htmlSync = new HtmlSyncHelper();
            Dictionary<string, string> oss = GetOss();
            _set = new OssSetting(oss);
            _oss = new OssClient(_set.Address, _set.UserID, _set.UserKey);
            frmSvc = frm;
        }

        private Dictionary<string,string> GetOss()
        {
            Dictionary<string, string> oss = new Dictionary<string, string>();
            oss.Add("userID", "");
            oss.Add("userKey", "");
            oss.Add("address", "");
            oss.Add("bucketName", "");
            return oss;
        }

        private string ParsePathFromDateTime(string publishDate,bool format=false)
        {
            DateTime time = DateTime.Parse(publishDate);
            return (format)?
                time.ToString(GlobalData.DateFormat):time.Year + @"\" + time.Month + @"\" + time.Day;
        }
        /// <summary>
        /// 将pdf文件解析成html-json文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns>解析后生成的json文件的路径地址</returns>
        public int PdfToHtml(string fromPath, string toPath,string datetime)
        {
            string startPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName;
            Process p = new Process();
            string fileName = startPath + "PdfToXml.exe";
            string para = "\"" + fromPath + "\" " + "\"" + toPath + "\" json \"" + ParsePathFromDateTime(datetime, true)+"\"";
            ProcessStartInfo pInfo = new ProcessStartInfo(fileName, para);
            p.StartInfo = pInfo;
            p.Start();
            while (!p.HasExited)
            {
                p.WaitForExit();
                Thread.Sleep(50);
            }
            return p.ExitCode;
        }
        public override void Startup()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        ParseAndUpLoad();
                    }
                    catch (Exception ex)
                    {
                        Utils.WriteLog("HtmlToOssService Startup 异常：" + ex.Message + ex.StackTrace);
                    }
                    AutoHelper.SetSleep(33 * 1115, 3 * 60 * 1237);
                }
            });
        }
        public void ParseAndUpLoad()
        {
            string publishDate = "";
            //扫描发布日期从现在起往前推的天数
            int before = ServiceSetting.MaxTasks * (-1);
            if(before<0)
            {
                publishDate = DateTime.Now.AddDays(before).ToString(GlobalData.DateFormat);
            }
            DataTable dtDate= _htmlSync.GetXXFBRQ(publishDate,false);
            if (Utils.IsEmpty(dtDate))
                return;
                        
            foreach (DataRow drRq in dtDate.Rows)
            {
                string resRq = drRq[0].ToString();
                frmSvc.DisplayMsg("加载：" + resRq + " 数据中,请稍等...");
                DataTable dt = _htmlSync.GetResourceData(resRq);
                if (Utils.IsEmpty(dt))
                    continue;

                frmSvc.DisplayMsg("转换并上传：" + resRq +" 共："+ dt.Rows.Count.ToString()+ " 文件,请稍等...");
                foreach (DataRow dr in dt.Rows)
                {
                    string id = dr["id"].ToString();
                    if (_htmlSync.ConvertEndIDs.ContainsKey(id))
                        continue;

                    if (_htmlSync.QueryNonConvert(id))
                        continue;

                    DateTime t = Convert.ToDateTime(dr["XXFBRQ"].ToString());
                    publishDate =t.ToString(GlobalData.DateFormat);                    
                    string filePath = _fromPath + ParsePathFromDateTime(publishDate) + @"\" + id + ".pdf";
                    string toFile = _toPath + ParsePathFromDateTime(publishDate) + @"\" + id + ".json";
                    if (!File.Exists(filePath))
                    {
                        filePath = _htmlSync.GetDmpResource(dr["HASHCODE"].ToString());
                    }
                    if (Utils.IsEmpty(filePath) || !File.Exists(filePath))
                        continue;

                    frmSvc.DisplayMsg("生成JSON：" + dr["XXBT"].ToString() + " ,请稍等...");
                    int exitCode = PdfToHtml(filePath, toFile, dr["XXFBRQ"].ToString());
                    if (exitCode == 0 || exitCode == 6)
                    {
                        //访问生成的json文件
                        //http://gildata-attachment.oss-cn-hangzhou.aliyuncs.com/JYPRIME/usrGSGGYWFWB/2018/03/21/574877756752JSON

                        if (ToOss(toFile, id, publishDate))
                        {
                            _htmlSync.WriteHtmlConvert(id, exitCode);
                        }
                    }
                    else
                    {
                        _htmlSync.WriteHtmlConvert(id, exitCode);
                    }
                    if (!_htmlSync.ConvertEndIDs.ContainsKey(id))
                    {
                        _htmlSync.ConvertEndIDs.Add(id, null);
                    }                    
                }
            }
        }
        /// <summary>
        /// 将文件上传到oss
        /// </summary>
        public bool ToOss(string jsonPath, string id, string xxfbrq)
        {
            if (!File.Exists(jsonPath))
            {
                Utils.WriteLog(jsonPath + " 文件不存在，上传失败！");
                return false;
            }
            //上传到OSS时，文件的扩展名全部为大写
            string objKey = _htmlSync.CreateObjectKey(id, "JSON", xxfbrq);
            string formPath = jsonPath;
            string ossMd5 = "";
            bool success = false;

            if (ServiceSetting.City == ServerCity.Nothing || ServiceSetting.City == ServerCity.ShTest
                || ServiceSetting.City == ServerCity.HzTest)
            {
                //测试环境没有OSS的存储空间，可以考虑不上传来处理。
                return !success;
            }
            success = Put(formPath, objKey, out ossMd5);
            if (!success)
                return false;
            ossMd5 = ossMd5.ToUpper();
            string localMd5 = Utils.GetHash(File.ReadAllBytes(formPath), "x2").ToUpper();
            return ossMd5 == localMd5;
        }

        public bool Put(string formPath, string objKey, out string md5)
        {
            md5 = "";
            try
            {
                byte[] b = File.ReadAllBytes(formPath);
                return Put(b, objKey, out md5);
            }
            catch (Exception ex)
            {
                Utils.WriteLog("ToOSS put错误！\r\nMD5:" + md5 + "\r\n文件路径:" + formPath + "\r\n"
                    + ex.Message + ex.StackTrace);
                GC.Collect();
                GC.Collect();
            }
            return false;
        }

        public bool Put(byte[] content, string objKey, out string md5)
        {
            md5 = "";
            if (content == null || Utils.IsEmpty(objKey))
                return false;
            using (MemoryStream ms = new MemoryStream(content))
            {
                PutObjectResult result = _oss.PutObject(_set.BucketName, objKey, ms);
                md5 = result.ETag;
            }
            return true;
        }

    }
}
