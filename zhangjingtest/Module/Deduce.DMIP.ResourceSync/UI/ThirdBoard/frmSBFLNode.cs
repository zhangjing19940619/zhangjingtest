using System;
using System.Data;
using System.Windows.Forms;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;

namespace Deduce.DMIP.ResourceSync
{
    public partial class frmSBFLNode : DataForm
    {
        TreeNode _node;
        bool _modifyStatus;
        TreeNode _parNode = null;

        public frmSBFLNode()
        {
            InitializeComponent();
        }

        public void DisplayWindow(TreeNode node, string menuID, bool modifyStatus, TreeNode parNode)
        {
            this.txtName.Text = "";
            this.txtExplain.Text = "";
            _node = node;
            _menuID = menuID;
            _modifyStatus = modifyStatus;
            _parNode = parNode;
            if (!_modifyStatus)
            {
                this.txtName.Text = _node.Text;
                this.txtExplain.Text = _node.Name;
            }
            this.ShowDialog();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Utils.IsEmpty(txtName.Text))
            {
                MessageBox.Show("'类别名称'不能为空！");
                return;
            }

            if (_node.Tag == null && _modifyStatus == false)
            {
                this.Close();
                return;
            }

            string q = "SELECT * From usrSBGGFLZYB";
            q += (_node.Tag == null) ? " " : " WHERE ID=" + _node.Tag + "";
            DataTable dt = _data.GetDataTable(q, _menuID);
            if (Utils.IsEmpty(dt) && _node.Tag != null)
                return;

            DataTable dtNew = (_modifyStatus) ? dt.Clone() : dt.Copy();
            DataRow dr = dtNew.NewRow();
            ModifyType mType = ModifyType.Insert;
            if (_modifyStatus)
            {
                GlobalData.SetDefaultFieldsValue(dr, RunWay.Manual, true, GKBZ.已检验);
                dr["ID"] = _data.GetIDs(1, "usrSBGGFLZYB")[0];
                dr["LBJB"] = _node.Level + 1;
                dr["FQLBBM"] = _node.Level == 0 ? "0" : dt.Rows[0]["LBBM"].ToString();
                dr["LBBM"] = Utils.BuildID;
                dtNew.Rows.Add(dr);
            }
            else
            {
                mType = ModifyType.Update;
                dr = dtNew.Rows[0];
                GlobalData.SetDefaultFieldsValue(dr, RunWay.Manual, true, GKBZ.已检验);
            }

            dr["LBMC"] = txtName.Text;
            dr["ZSSM"] = txtExplain.Text;
            _node.Name = txtExplain.Text;
            if (!_data.DataImport("usrSBGGFLZYB", dtNew, mType, GlobalData.CommonMenuID))
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
    }
}
