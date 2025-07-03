using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace Deduce.DMIP.ResourceSync
{
    [Serializable]
    public class SBGGData
    {
        public SBGGData()
        { }
        public SBGGData(DataGridViewRow dr)
        {
            ID = dr.Cells["ID"].Value.ToString();
            GGBT = dr.Cells["公告标题"].Value.ToString();
            GGLY = dr.Cells["公告来源"].Value.ToString();
            GGLB = dr.Cells["公告类别"].Value.ToString();
            GKBZ = dr.Cells["公开标识"].Value.ToString();
            GPDM = dr.Cells["证券代码"].Value.ToString();
            IGSDM = dr.Cells["IGSDM"].Value.ToString();
            INBBM = dr.Cells["INBBM"].Value.ToString();
            TBSJ = dr.Cells["同步时间"].Value.ToString();
            WJGS = dr.Cells["文件格式"].Value.ToString();
            GGRQ = dr.Cells["公告日期"].Value.ToString();
            LINK = dr.Cells["公告链接"].Value.ToString();
            TBZT = dr.Cells["同步状态"].Value.ToString();
            MD5 = dr.Cells["md5"].Value.ToString();
        }
        public string ID { get; set; }
        public string GGRQ { get; set; }
        public string GGBT { get; set; }
        public string GGLY { get; set; }
        public string GGLB { get; set; }
        public string GKBZ { get; set; }
        public string GPDM { get; set; }
        public string TBSJ { get; set; }
        public string WJGS { get; set; }
        public string IGSDM { get; set; }
        public string INBBM { get; set; }
        public string LINK { get; set; }
        public string XGRY { get; set; }
        public string XGSJ { get; set; }
        public string TBZT { get; set; }
        public string MD5 { get; set; }
    }
}
