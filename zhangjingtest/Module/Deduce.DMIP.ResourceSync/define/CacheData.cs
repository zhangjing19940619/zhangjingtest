using System.Collections.Generic;
using System.Data;
using Deduce.DMIP.Sys.SysData;

namespace Deduce.DMIP.ResourceSync
{
    /// <summary>
    /// 编辑用到的缓存数据 
    /// </summary>
    public class CacheData
    {
        public CacheData(DataTable dtGPDM, DataTable dtGGLY, Dictionary<string, StockCode> sc, Dictionary<string, string> ggly)
        {
            this.dtGPDM = dtGPDM;
            this.dtGGLY = dtGGLY;
            this.stockCode = sc;
            this.GGLYPair = ggly;
        }
        public DataTable dtGPDM { get; set; }
        public DataTable dtGGLY { get; set; }
        public Dictionary<string, StockCode> stockCode { get; set; }
        public Dictionary<string, string> GGLYPair { get; set; }
    }
}
