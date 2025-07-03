using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;

namespace Deduce.DMIP.ResourceSync
{
    public partial class frmSBHYFLNode : DataForm
    {
        int _type = -1;
        private TreeNode _node;
        private bool _modifyStatus;
        private TreeNode _parNode = null;

        public frmSBHYFLNode()
        {
            InitializeComponent();
            BindComboBox();
        }

        public void DisplayWindow(TreeNode node, string menuID, bool modifyStatus, TreeNode parNode, int type)
        {
            this.txtName.Text = "";
            this.txtExplain.Text = "";
            this.dtpAfficfe.Text = "";
            this.dtpEffect.Text = "";
            this.cbxFlag.Text = "";
            this.cbxFBJG.Text = "";
            _type = type;
            _node = node;
            _menuID = menuID;
            _modifyStatus = modifyStatus;
            _parNode = parNode;
            if (!_modifyStatus)
            {
                string q = " Select * From usrSBHYFLZYB where HYFLBZ=" + _type + " and ID=" + _node.Tag + "";
                if (_node.Tag == null)
                    return;
                DataTable dt = _data.GetDataTable(q, _menuID);
                this.txtName.Text = _node.Text;
                this.txtExplain.Text = _node.Name;
                this.dtpAfficfe.Text = dt.Rows[0]["GGRQ"].ToString();
                this.dtpEffect.Text = dt.Rows[0]["SXRQ"].ToString();
                this.cbxFlag.Text = (int.Parse(dt.Rows[0]["GKBZ"].ToString()) == 1) ? "否" : "是";
                this.cbxFBJG.Text = dt.Rows[0]["FBJGDM"].ToString();
            }
            this.ShowDialog();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Utils.IsEmpty(txtName.Text))
            {
                MessageBox.Show("'行业名称'不能为空！");
                return;
            }
            else if (Utils.IsEmpty(cbxFlag.Text))
            {
                MessageBox.Show("'公开标志'不能为空！");
                return;
            }

            if (_node.Tag == null && _modifyStatus == false)
            {
                this.Close();
                return;
            }

            string q = "SELECT * From usrSBHYFLZYB";
            q += (_node.Tag == null) ? " " : " WHERE ID=" + _node.Tag + "";
            DataTable dt = _data.GetDataTable(q, _menuID);
            string fhybm = dt.Rows[0]["HYBM"].ToString();//父行业编码
            if (_node.Tag != null && Utils.IsEmpty(dt))
                return;

            DataTable dtable = (_modifyStatus) ? dt.Clone() : dt.Copy();
            DataRow dr = dtable.NewRow();
            ModifyType mType = ModifyType.Insert;
            if (_modifyStatus)
            {
                dr["ID"] = _data.GetIDs(1, "usrSBHYFLZYB")[0];
                dr["HYJB"] = _node.Level + 1;
                dr["FQHYBM"] = _node.Level == 0 ? "0" : fhybm;
                dr["HYBM"] = fhybm.Substring(0, 1) + GetMaxHYBM(fhybm);
                if (cbxFBJG.SelectedValue != null)
                {
                    dr["FBJGDM"] = cbxFBJG.SelectedValue;
                }
                dr["HYFLBZ"] = _type;
                dtable.Rows.Add(dr);
            }
            else
            {
                mType = ModifyType.Update;
                dr = dtable.Rows[0];
                if (cbxFlag.SelectedValue != null)
                {
                    dr["GKBZ"] = cbxFlag.SelectedValue;
                }
                if (cbxFBJG.SelectedValue != null)
                {
                    dr["FBJGDM"] = cbxFBJG.SelectedValue;
                }
            }
            GlobalData.SetDefaultFieldsValue(dr, RunWay.Manual, true, GKBZ.已检验);
            dr["HYMC"] = txtName.Text;
            dr["ZSSM"] = txtExplain.Text;
            dr["GGRQ"] = dtpAfficfe.Text;
            dr["SXRQ"] = dtpEffect.Text;
            _node.Name = txtExplain.Text;
            if (!_data.DataImport("usrSBHYFLZYB", dtable, mType, GlobalData.CommonMenuID))
            {
                MessageBox.Show("保存数据失败！" + GetLastMessage());
                return;
            }

            if (_modifyStatus)
            {
                TreeNode tn = new TreeNode(txtName.Text);
                tn.Tag = dr["ID"];
                _parNode.Nodes.Add(tn);
            }
            else
            {
                _parNode.Text = txtName.Text;
            }
            this.Close();
        }

        //获取最大的行业编码加1
        private int GetMaxHYBM(string fhybm)
        {
            List<int> maxs = new List<int>();
            string q = " Select HYBM From usrSBHYFLZYB where HYFLBZ=" + _type + " and HYBM like '%" + fhybm.Substring(0, 1) + "%'";
            DataTable dt = _data.GetDataTable(q, _menuID);
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                string hybm = dt.Rows[i][0].ToString();
                if (hybm.Length > 1)
                {
                    maxs.Add(int.Parse(hybm.Substring(1, hybm.Length - 1)));
                }
            }
            return maxs.Max() + 1;
        }

        //初始化窗体下拉框
        private void BindComboBox()
        {
            Dictionary<int, string> dFlag = new Dictionary<int, string>();
            dFlag.Add(1, "否");
            dFlag.Add(3, "是");
            BindingSource bs2 = new BindingSource();
            bs2.DataSource = dFlag;
            cbxFlag.DataSource = bs2;
            cbxFlag.DisplayMember = "Value";
            cbxFlag.ValueMember = "Key";
            cbxFlag.SelectedValue = "";

            Dictionary<int, string> dFBJG = new Dictionary<int, string>();
            dFBJG.Add(169142, "中国证监会");
            dFBJG.Add(201717, "全国中小企业股份转让系统");
            BindingSource bs3 = new BindingSource();
            bs3.DataSource = dFBJG;
            cbxFBJG.DataSource = bs3;
            cbxFBJG.DisplayMember = "Value";
            cbxFBJG.ValueMember = "Key";
            cbxFBJG.SelectedValue = "";
        }
    }
}
