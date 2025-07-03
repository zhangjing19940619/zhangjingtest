using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;
using Deduce.DMIP.ResourceSync.Server;
using Deduce.DMIP.Sys.SysData;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Deduce.DMIP.ResourceSync
{
    /// <summary>
    /// 三板公告查询和修改的方法集
    /// </summary>
    public class SBQueryHelper : DataClass
    {
        private readonly SBSyncHelper _syncHelper;
        private readonly ILogger<SBQueryHelper> _logger;

        public SBQueryHelper(SBSyncHelper syncHelper, ILogger<SBQueryHelper> logger)
        {
            _syncHelper = syncHelper ?? throw new ArgumentNullException(nameof(syncHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        /// <summary>
        /// 选择添加数据
        /// </summary>
        /// <param name="dic">判断重复项</param>
        /// <param name="dt">数据添加表来源</param>
        /// <param name="txt">添加项</param>
        /// <param name="showMsg">提示信息</param>
        public void AddSelectedData(Dictionary<string, string> dic, DataTable dt, string txt, string showMsg, DataGridView grid)
        {
            txt = txt.Trim();
            if (Utils.IsEmpty(txt))
            {
                MessageBox.Show("不能添加空的代码！!");
                return;
            }
            int index = txt.IndexOf(' ');
            string dm = txt.Substring(0, index).ToString();
            int index2 = txt.IndexOf('-');
            string mc = index2 == -1 ? txt.Substring(index + 1).Replace(" ", "") :
                txt.Substring(index + 1, index2 - index - 1).Replace(" ", "");
            if (dic.ContainsKey(dm))
            {
                MessageBox.Show(showMsg);
                return;
            }
            dic.Add(dm, mc);
            DataRow dr = dt.NewRow();
            dr["代码"] = dm;
            dr["名称"] = mc;
            dt.Rows.Add(dr);
            grid.DataSource = dt;
        }

        public string GetGGLBBM(DataRow dr)
        {
            string sixLBBM = dr["LJGGBM"].ToString();
            string fiveLBBM = dr["WJGGBM"].ToString();
            string fourLBBM = dr["SIJGGBM"].ToString();
            string threeLBBM = dr["SAJGGBM"].ToString();
            string twoLBBM = dr["EJGGBM"].ToString();
            string oneLBBM = dr["YJGGBM"].ToString();

            string lbbm = !Utils.IsEmpty(sixLBBM) ? sixLBBM : !Utils.IsEmpty(fiveLBBM) ?
                fiveLBBM : !Utils.IsEmpty(fourLBBM) ? fourLBBM :
                !Utils.IsEmpty(threeLBBM) ? threeLBBM : !Utils.IsEmpty(twoLBBM) ?
                twoLBBM : !Utils.IsEmpty(oneLBBM) ? oneLBBM : "";

            return lbbm;
        }

        public string GetDMByDes(string code)
        {
            if (Utils.IsEmpty(code))
                return "";

            code = code.Trim();
            int index = code.IndexOf(' ');
            return index != -1 ? code.Substring(0, index).ToString() : code;
        }
        /// <summary>
        /// 解析查询语句中的in条件
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public string AnalyseQueryFields(DataTable dt, string field)
        {
            if (Utils.IsEmpty(dt))
                return "";

            string result = @" and  " + field + " in (";
            foreach (DataRow dr in dt.Rows)
            {
                result += (dr[0].ToString() + ",");
            }
            result = result.Substring(0, result.Length - 1) + ")";
            return result;
        }
        public string GetQueryScript(List<string> datas, string field)
        {
            if (datas == null || datas.Count == 0)
                return "";

            string result = field + " in (";
            foreach (string s in datas)
            {
                result += ("'" + s + "'" + ",");
            }
            result = result.Substring(0, result.Length - 1) + ")";
            return result;
        }
        public string GetQueryScripts(List<string> datas, List<string> fields)
        {
            string result = "";
            if (datas == null || datas.Count == 0)
                return "";

            for (int i = 0; i < fields.Count - 1; i++)
            {
                result += GetQueryScript(datas, fields[i]) + " or ";
            }
            result += GetQueryScript(datas, fields[fields.Count - 1]);
            return result;
        }

        /// <summary>
        /// 通过指定分隔找到字符串
        /// </summary>
        /// <param name="data">待处理字符串</param>
        /// <param name="split">分隔符</param>
        /// <param name="after">取分隔符之前或者之后的字符串</param>
        /// <returns></returns>
        public string GetFindString(string data, string split, bool after)
        {
            if (Utils.IsEmpty(data))
                return "";

            int index = data.IndexOf(split);
            if (index != -1)
                return after ? data.Substring(index + 1) : data.Substring(0, index);

            return data;
        }

        public string GetGKBZName(string gkbzCode)
        {
            return (Utils.IsEmpty(gkbzCode))?
                "":ResourceTypes.GKBZData.GetGKBZ.ContainsKey(gkbzCode) ? ResourceTypes.GKBZData.GetGKBZ[gkbzCode] : gkbzCode;
        }
        public string GetGKBZCode(string gkbzName)
        {
            if (Utils.IsEmpty(gkbzName))
                return "";

            return ResourceTypes.GKBZData.GetGKBZ.Values.Contains(gkbzName) ?
                    ResourceTypes.GKBZData.GetGKBZ.Where(item => item.Value == gkbzName).First().Key : gkbzName;
        }
        public string GetRQRange(string start, string end, string field)
        {
            if (Utils.IsEmpty(field))
                return "";

            if (!Utils.IsEmpty(end))
                end += " 23:59:59";

            if (Utils.IsEmpty(start) && Utils.IsEmpty(end))
                return "";

            if (!Utils.IsEmpty(start) && Utils.IsEmpty(end))
                return @" and  " + field + " >= '" + start + "'";

            if (Utils.IsEmpty(start) && !Utils.IsEmpty(end))
                return @" and  " + field + " <= '" + end + "'";

            return @" and  " + field + " between '" + start + "' and '" + end + "'";
        }
        public void GetOssFile(SBGGData sd, string toPath)
        {
            try
            {
                Utils.CreateDir(toPath);
                string objKey = _syncHelper.CreateObjectKey(sd.ID, sd.WJGS.ToUpper(), sd.GGRQ);

                if (Utils.IsEmpty(objKey))
                {
                    MessageBox.Show("该文件资源不存在或者生成OSS的objectKey过程中出错。请检查！");
                    return;
                }
                //去除文件名中的非法字符
                foreach (char rInvalidChar in Path.GetInvalidFileNameChars())
                    sd.GGBT = sd.GGBT.Replace(rInvalidChar.ToString(), string.Empty);
                toPath += (sd.GGBT + sd.ID + sd.WJGS.ToUpper());
                if (!File.Exists(toPath))
                {
                    //先从共享目录中获取，如果没有则在OSS上下载
                    string shareFile = GlobalData.BackupPath + objKey;
                    byte[] obj = _data.GetResourceFile(shareFile);
                    if (obj == null || obj.Length == 0)
                    {
                        obj = _data.GetOssContent(objKey);
                    }
                    Utils.WriteBinary(toPath, obj);
                }
                System.Diagnostics.Process.Start(toPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("文件获取失败!报错信息：\n" + ex.Message + ex.StackTrace);
            }
        }
    }
}
