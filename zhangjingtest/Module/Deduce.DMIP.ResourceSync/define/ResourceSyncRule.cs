using System;
using Deduce.Common.Formula;
using Deduce.Common.Utility;

namespace Deduce.DMIP.ResourceSync
{
    /// <summary>
    /// 资源同步规则设定类
    /// 1、归类规则设定
    /// 2、内码查询规则设定
    /// 3、字段值的SQL查询脚本
    /// </summary>
    [Serializable]
    public class ResourceSyncRule
    {
        public ResourceSyncRule()
        {
        }

        public string CategoryID { get; set; }
        
        string _categoryFormula = "";
        public string CategoryFormula
        {
            get { return _categoryFormula; }
            set
            {
                _categoryFormula = value;
                _categoryExpress = new Expression();
                if (!Utils.IsEmpty(_categoryFormula))
                {
                    _categoryExpress = Expression.FromString(_categoryFormula);
                }
            }
        }

        Expression _categoryExpress = null;
        public Expression CategoryExpress
        {
            get { return _categoryExpress; }
            set
            {
                _categoryExpress = value;
                _categoryFormula = value.ToString();
            }
        }
    }
}
