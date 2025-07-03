using Deduce.Common.Components;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;
using Deduce.DMIP.Sys.SysData;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Deduce.DMIP.ResourceSync.Server
{
    public class SBWBSync: DataClass
    {

        static string _apiUrl = GlobalData.GetSystemParameters(SystemParaType.AutoServiceWebApi)[0].ParaValue;


        frmAutoSync frmSvc = null;
        /// <summary>
        /// 获取需要解析的pdf公告相关信息
        /// </summary>
        public void SyncExecute(frmAutoSync frm)
        {
            
            try
            {
                frmSvc = frm;
                var today = DateTime.Now.ToShortDateString();
                var todayFileName = (Math.Floor(DateTime.Now.ToOADate())).ToString();
                if (!Directory.Exists(@"D:\XMLFile\"+ todayFileName))
                    Directory.CreateDirectory(@"D:\XMLFile\" + todayFileName);
                var GGList = _data.GetDataTable("SELECT * FROM usrSBGGB a where a.GGRQ >= '"+today+"' and NOT EXISTS(select 1 FROM usrSBGGWBB b WHERE b.RID = a.ID)");
                if(GGList ==null || GGList.Rows.Count == 0)
                {
                    return;
                }
                foreach (DataRow item in GGList.Rows)
                {
                    if (File.Exists(@"D:\XMLFile\"+ todayFileName + "\\" + item["ID"].ToString() + ".xml") || File.Exists(@"D:\XMLFile\" + todayFileName + "\\" + item["ID"].ToString() + ".txt"))
                        continue;
                    var resource = _data.GetDataTable("select top 1 a.*,b.MediaSource from cfg.dmip_Resource(nolock) a left join cfg.dmip_GrabSite(nolock) b on a.sourceID=b.OB_OBJECT_ID where resMD5 ='" + item["HASHCODE"] + "'");
                    AutoResource autoResource = new AutoResource();
                    autoResource.ObjectID = resource.Rows[0]["ob_object_id"].ToString();
                    autoResource.StorePath = resource.Rows[0]["storePath"].ToString();
                    autoResource.Extension = resource.Rows[0]["extension"].ToString();
                    autoResource.MediaSource = resource.Rows[0]["MediaSource"].ToString();
                    var apiRes = ConvertFileApi(autoResource);
                    //将数据读入流中
                    if (apiRes.Code == 0)
                    {
                        byte[] array = Encoding.Default.GetBytes(apiRes.Data);
                        MemoryStream stream = new MemoryStream(array);
                        using (stream)
                        {
                            using (var fs = File.Open(@"D:\XMLFile\"+ todayFileName + "\\" + item["ID"].ToString() + ".xml", FileMode.CreateNew))
                            {
                                int length = 100 * 1024;
                                var byteLength = new byte[length];
                                do
                                {
                                    length = stream.Read(byteLength, 0, length);
                                    fs.Write(byteLength, 0, length);
                                } while (length != 0);
                            }
                        }
                        //判断pdf解析的数据是否异常
                        var IsNormal = IsChinaStr(apiRes.Data);

                        //判断content是否乱码
                        var codeRes = IsUnableRead(@"D:\XMLFile\" + todayFileName + "\\" + item["ID"].ToString() + ".xml");

                        if (IsNormal && !codeRes)
                        {
                            var content = XmlRead.GetAllDatasFromContent(apiRes.Data)[0].Content;
                            //string content = ReadXML(@"D:\XMLFile\" + item["ID"].ToString() + ".xml");
                            if (!string.IsNullOrEmpty(content) && !content.StartsWith("4-1"))
                            {
                                InsertToWBB(item, content);
                                Display(apiRes.Code + "---" + apiRes.Msg + "---" + item["ID"].ToString());
                                Utils.WriteInfoLog(apiRes.Code + "---" + apiRes.Msg + "---" + item["ID"].ToString());
                            }
                        }
                    }
                    else
                    {
                        File.Create(@"D:\XMLFile\" + todayFileName + "\\" + item["ID"].ToString() + ".txt");
                        Display(apiRes.Code + "---" + apiRes.Msg + "---" + item["ID"].ToString());
                        Utils.WriteInfoLog(apiRes.Code + "---" + apiRes.Msg + "---" + item["ID"].ToString());
                    }
                }
            }
            catch (IOException ex)
            {
                Utils.WriteErrorLog("文件读写错误：" + ex.Message + "位置：" + ex.StackTrace + "错误时间：" + DateTime.Now);
            }
            catch (Exception ex)
            {
                Utils.WriteErrorLog(ex.Message + "位置：" + ex.StackTrace + "错误时间：" + DateTime.Now);
            }
        }

        /// <summary>
        /// 读取xml文件的文本信息
        /// </summary>
        /// <returns></returns>
        public static string ReadXML(string xmlPath)
        {
            var resultStr = "";
            var content = "";
            XmlDocument xmlDocument = new XmlDocument();
            //读取文件的内容
            using (FileStream fileStream = new FileStream(xmlPath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader streamReader = new StreamReader(fileStream, Encoding.GetEncoding("gb2312")))
                {
                    //获取文件中的内容
                    resultStr = streamReader.ReadToEnd();
                }
            }
            try
            {
                //将XML字串读到xmlDocument中
                xmlDocument.LoadXml(resultStr);
                //将xmlDocument读取到内存
                MemoryStream memoryStream = new MemoryStream();
                xmlDocument.Save(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                GetXmlChildNodes(xmlDocument.FirstChild.NextSibling, ref content);
            }
            catch (Exception ex)
            {
                Utils.WriteErrorLog("xml文件读取失败" + DateTime.Now + "  错误信息：" + ex.Message + " 堆栈的直接帧的字符串： " + ex.StackTrace);
            }
            return content;
        }

        /// <summary>
        /// 判读content的中文
        /// </summary>
        /// <returns></returns>
        public static bool IsChinaStr(string CString)
        {
            string rt = "";
            for (int i = 0; i < CString.Length; i++)
            {
                if (Convert.ToInt32(Convert.ToChar(CString.Substring(i, 1))) > Convert.ToInt32(Convert.ToChar(128)))
                {
                    rt += CString.Substring(i, 1).ToString();
                }
            }
            //float percent = (float)rt.Length /CString.Length ;

            //[^%&’,;=?$\x22]+
            //Regex reg = new Regex("[^%&’,;=?$\x22]+");
            //string result = reg.Replace(rt,string.Empty);


            foreach (char rInvalidChar in Path.GetInvalidFileNameChars())
                rt = rt.Replace(rInvalidChar.ToString(), string.Empty);
            if (rt.Length > 100)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断是否乱码
        /// </summary>
        /// <returns></returns>
        public static bool IsUnableRead(string filepath)
        {
            string content = ReadXML(filepath);
            string result = Regex.Replace(content, "[\\s\\p{P}\n\r=+$￥<>^`~|]", string.Empty);
            Double percent = Convert.ToDouble(result.Length) / Convert.ToDouble(content.Length);
            if (percent < 0.51)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="row"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool InsertToWBB(DataRow row, string content)
        {

            DataTable dt = _data.GetStructure("usrSBGGWBB", "");
            var insertRow = dt.NewRow();
            insertRow["ID"] = Convert.ToInt64(_data.GetIDs(1, "usrSBGGWBB")[0]);
            insertRow["RID"] = Convert.ToInt64(row["ID"]);
            insertRow["GGNR"] = content;
            insertRow["GKBZ"] = 3;
            insertRow["XGRY"] = 11111;
            insertRow["XGRY2"] = 999;
            insertRow["XGSJ"] = DateTime.Now;
            insertRow["FBSJ"] = DateTime.Now;
            if (!string.IsNullOrEmpty(row["SHRY"].ToString()))
                insertRow["SHRY"] = Convert.ToInt32(row["SHRY"]);
            insertRow["JSID"] = Convert.ToInt64(row["JSID"]);
            dt.Rows.Add(insertRow);
            var resultDb = _data.DataImport("usrSBGGWBB", dt, ModifyType.Insert, GlobalData.CommonMenuID);
            return resultDb;
        }

        private static List<string> _needOcrCategoryIDList = new List<string>()
        {
            //"{713456FE-D4D9-4C7D-A0B3-4FEC8FDD23C5}",//新股配售公告自动解析（基金、法人配售与战略投资者）--测试环境--生产环境
            //"{F82F765E-9115-4744-9E07-52567348D30F}",//IPO投资者报价信息表--测试环境--生产环境
        };
        public ApiResponse<string> ConvertFileApi(AutoResource resource)
        {
            string host = _apiUrl;//GlobalData.GetSystemParameters(SystemParaType.AutoServiceWebApi)[0].ParaValue;
            string hostForImageParse = GlobalData.GetSystemParameters(SystemParaType.WebApiForImageParse)[0].ParaValue;

            var isImageParse = false;
            var attachParas = new { Removefooter = "1", OCRType = "0" };
            string fileType = "";
            if (resource.MediaSource == "44444444")
            {
                fileType = "pdf_noline";
            }
            else if (resource.Extension == ".pdf")
            {
                fileType = "pdf";
                if (_needOcrCategoryIDList.Contains(resource.CategoryID))
                {
                    isImageParse = true;
                    attachParas = new { Removefooter = "1", OCRType = "1" };
                }
            }
            //......

            string requestString = new
            {
                BusinessID = "DMP",
                ObjectID = resource.ObjectID,
                SourceType = "1",
                FilePath = resource.StorePath,
                FileType = fileType,
                AttachParas = attachParas
            }.ToJson();
            string responseString = new HttpHelper().GetHtml(new HttpItem
            {
                Method = "POST",
                URL = (isImageParse ? hostForImageParse : host) + "/api/CollectData/GetXmlString",
                ContentType = "application/json",
                Timeout = 1000 * 60 * 30,
                Postdata = requestString,
                PostEncoding = Encoding.UTF8
            }).Html;

            ApiResponse<string> response;
            try
            {
                response = ExtractJson.ToObject<ApiResponse<string>>(responseString);
            }
            catch (Exception ex)
            {
                Utils.WriteLogServer("PDF解析结果异常", responseString + "-----" + resource.ObjectID);
                throw ex;
            }

            Utils.WriteInfoLog(string.Format("文件类型={0},resourceID={1},code={2},msg={3}", fileType, resource.ObjectID, response.Code, response.Msg));
            Utils.WriteLogServer("接口解析-" + fileType, string.Format("文件类型={0},code={1},msg={2}", fileType, response.Code, response.Msg) +"-----"+resource.ObjectID);

            //判断解析xml长度，如果太短则认为是失败，再调用单字符解析
            if (!string.IsNullOrEmpty(response.Data) && response.Data.Length < 50)
            {
                Utils.WriteLogServer("PDF解析", "解析失败后采用单字符解析"+"-----"+ resource.ObjectID);
                response = ConvertSingleChar(resource);
                Utils.WriteLogServer("单字符接口解析-" + fileType, string.Format("文件类型={0},code={1},msg={2}", fileType, response.Code, response.Msg)+"-----"+ resource.ObjectID);
            }

            return response;
        }


        /// <summary>
        /// PDF单字符解析
        /// </summary>
        private static ApiResponse<string> ConvertSingleChar(AutoResource resource)
        {
            string host = GlobalData.GetSystemParameters(SystemParaType.AutoServiceWebApi)[0].ParaValue;
            string requestString = ExtractJson.ToJson(new
            {
                BusinessID = "DMP",
                ObjectID = "test",
                SourceType = "1",
                FilePath = resource.StorePath,
                FileType = "pdf_noline",
                AttachParas = new { IsSingleChar = "1" }
            });
            string responseString = new HttpHelper().GetHtml(new HttpItem
            {
                Method = "POST",
                URL = host + "/api/CollectData/GetXmlString",
                ContentType = "application/json",
                Timeout = 1000 * 60 * 30,
                Postdata = requestString,
                PostEncoding = Encoding.UTF8
            }).Html;
            ApiResponse<string> response;
            try
            {
                response = ExtractJson.ToObject<ApiResponse<string>>(responseString);
            }
            catch (Exception ex)
            {
                Utils.WriteLogServer("PDF单字符解析结果异常", responseString+"-----"+ resource.ObjectID);
                throw ex;
            }

            return response;
        }


        /// <summary>
        /// 对xml文件节点的子节点遍历
        /// </summary>
        /// <param name="xmlNode">xml节点</param>
        /// <param name="dataTable">DataTable表格</param>
        /// <returns>操作后的dataTable表格</returns> 
        public static void GetXmlChildNodes(XmlNode xmlNode, ref string content)
        {
            for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
            {
                if (xmlNode.ChildNodes[i].HasChildNodes && xmlNode.ChildNodes[i].Name != "#text")
                {
                    //通过递归调用遍历节点树
                    GetXmlChildNodes(xmlNode.ChildNodes[i], ref content);
                }
                else if (xmlNode.ChildNodes[i].Name == "#text")
                {
                    content += xmlNode.ChildNodes[i].Value.ToString() + "      ";
                }
            }
        }

        private void Display(string msg)
        {
            frmSvc.WriteMsg(msg, LogOut.ToScreen);
        }
    }


    /// <summary>
    /// API接口返回消息体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }

        public ApiResponse()
        {
            Code = 0;
            Msg = "成功";
        }

        public ApiResponse(int code, string msg)
        {
            Code = code;
            Msg = msg;
        }

        public ApiResponse(T data)
        {
            Code = 0;
            Msg = "成功";
            Data = data;
        }
    }
}
