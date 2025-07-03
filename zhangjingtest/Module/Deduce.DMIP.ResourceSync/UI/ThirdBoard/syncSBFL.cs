using Deduce.DMIP.Sys.OperateData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using Deduce.Common.Utility;

namespace Deduce.DMIP.ResourceSync
{
    public class SyncSBFL
    {
        protected static OperateData _data = OperateData.Instance;
        /// <summary>
        /// 获取SBFL表数据
        /// </summary>
        /// <param name="menuID">表ID</param>
        /// <returns></returns>
        public static DataTable GetDatatable(string menuID)
        {
            try
            {
                return _data.GetDataTable("select * from usrSBGGFLZYB", menuID);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
