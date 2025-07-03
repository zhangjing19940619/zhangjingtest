using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Deduce.Common.Components;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;
using Deduce.DMIP.Sys.SysData;

namespace Deduce.DMIP.ResourceSync
{
    public partial class frmSBFLTree : ExtendDataForm
    {
        frmSearchBox frmFind = new frmSearchBox();
        ModuleTree _mTree = new ModuleTree();
        TreeNode _root = null;
        DataTable _dtNew = null;
        TreeNode _tr = null;
        int _index = 2;
        bool _moidfyStatus;
        frmSBFLNode frmEditNode = null;
        TreeNode _curNode = null;  //当前操作节点

        FormType _frmType = FormType.SBFL;
        frmSBFLRuleDesign frmSBFLRule = null;

        public frmSBFLTree()
        {
            InitializeComponent();
            _isEscapeClose = true;
        }

        public void DisplayWindow(string type, string menuID)
        {
            InitData(type, menuID);
        }

        private void InitData(string type, string menuID)
        {
            if (Utils.IsEmpty(type) || type == "10")
            {
                _frmType = FormType.SBFL;
            }
            else if (type == "11")
            {
                _frmType = FormType.SBFLDesign;
            }
            else if (type == "12")
            {
                _frmType = FormType.SBFLQuery;
            }
            toolBar.Visible = (_frmType == FormType.SBFL);
            _dtNew = SyncSBFL.GetDatatable(_menuID);
            if (_dtNew == null)
            {
                MessageBox.Show("无法加载三板公告分类数据");
                return;
            }

            _menuID = menuID;
            BuildTree(menuID);
            frmSearchBox.BindData(frmFind, _dtNew, "LBMC", "ID");

            if (_frmType == FormType.SBFL)
            {
                frmEditNode = new frmSBFLNode();
                this.MdiParent = null;
                pnlForm.Visible = false;
                treeView.Dock = DockStyle.Fill;
                this.ShowDialog();
                return;
            }

            if (_frmType == FormType.SBFLDesign)
            {
                frmSBFLRule = new frmSBFLRuleDesign();
                DataForm.SetInsideForm(frmSBFLRule, pnlForm.Controls);
                frmSBFLRule.MenuID = base.MenuID;
            }
            this.WindowState = FormWindowState.Maximized;
            this.Show();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _moidfyStatus = true;
            if (treeView.SelectedNode == null)
            {
                MessageBox.Show("请选择节点！");
                return;
            }
            _curNode = (_curNode == null) ? _root : _curNode;
            frmEditNode.DisplayWindow(treeView.SelectedNode, _menuID, _moidfyStatus, _curNode);
        }

        private void BuildTree(string menuID)
        {
            this.treeView.Nodes.Clear();
            _root = new TreeNode("三板公告分类指引");
            treeView.Nodes.Add(_root);
            _root.Expand();
            DataRow[] rs = _dtNew.Select("LBJB=1");
            foreach (DataRow dr in rs)
            {
                TreeNode tr = new TreeNode(dr["LBMC"].ToString());
                tr.Tag = dr["ID"].ToString();
                tr.Name = dr["ZSSM"].ToString();
                tr.ToolTipText = dr["LBBM"].ToString();
                tr.Expand();
                _root.Nodes.Add(tr);
                TreeView_AddChildNodes(tr, dr["LBBM"].ToString(), menuID);
            }
        }

        private void TreeView_AddChildNodes(TreeNode node, string lbbm, string menuID)
        {
            DataRow[] rs = _dtNew.Select("FQLBBM='" + lbbm + "'");
            foreach (DataRow dr in rs)
            {
                TreeNode tr = new TreeNode(dr["LBMC"].ToString());
                tr.Tag = dr["ID"].ToString();
                tr.Name = dr["ZSSM"].ToString();
                tr.ToolTipText = dr["LBBM"].ToString();
                node.Nodes.Add(tr);
                TreeView_AddChildNodes(tr, dr["LBBM"].ToString(), menuID);
            }
        }

        private void treeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void treeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = (e.Data.GetDataPresent(typeof(TreeNode))) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            Point pt = ((TreeView)(sender)).PointToClient(new Point(e.X, e.Y));
            TreeNode objNode = (TreeNode)((TreeView)(sender)).GetNodeAt(pt);
            treeView.SelectedNode = objNode;
        }

        private void treeView_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode myNode = (TreeNode)(e.Data.GetData(typeof(TreeNode)));
            TreeNode DropNode = GetDropNode(e);
            if (!e.Data.GetDataPresent(typeof(TreeNode)))
            {
                MessageBox.Show("error");
                return;
            }


            //目标节点不存在
            if (DropNode == null)
            {
                MessageBox.Show("请选择目标节点！");
                return;
            }

            // 1.目标节点不是空。2.目标节点不是被拖拽接点的字节点。3.目标节点不是被拖拽节点本身
            if (DropNode.Parent != myNode && DropNode != myNode)
            {
                if (MessageBox.Show("确定要拖动到" + DropNode.Text + "节点下吗？", "提示", MessageBoxButtons.OKCancel) != DialogResult.OK)
                    return;
                try
                {
                    string q = "SELECT * From usrSBGGFLZYB WHERE ID=" + DropNode.Tag;
                    DataTable dtParent = _data.GetDataTable(q, _menuID);
                    string fqlbbm = (dtParent == null) ? "0" : dtParent.Rows[0]["LBBM"].ToString();

                    q = " SELECT * From usrSBGGFLZYB WHERE ID=" + myNode.Tag;
                    DataTable dt = _data.GetDataTable(q, _menuID);
                    if (Utils.IsEmpty(dt))
                        return;
                    DataRow dr = dt.Rows[0];
                    GlobalData.SetDefaultFieldsValue(dr, RunWay.Manual, true, GKBZ.已检验);
                    dr["LBJB"] = DropNode.Level + 1;//选定节点深度加一
                    dr["FQLBBM"] = fqlbbm;
                    _data.DataImport("usrSBGGFLZYB", dt, ModifyType.Update, GlobalData.CommonMenuID);
                    _tr = myNode;
                    UpdateLevel(DropNode);
                    _index = 2;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                TreeNode DragNode = myNode;
                // 将被拖拽节点从原来位置删除。
                myNode.Remove();
                // 在目标节点下增加被拖拽节点
                DropNode.Nodes.Add(DragNode);
            }
        }

        private TreeNode GetDropNode(DragEventArgs e)
        {
            Point position = new Point(0, 0);
            position.X = e.X;
            position.Y = e.Y;
            position = treeView.PointToClient(position);
            return this.treeView.GetNodeAt(position);
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (_frmType != FormType.SBFL)
                return;

            EditNode();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditNode();
        }

        private void EditNode()
        {
            _moidfyStatus = false;
            if (treeView.SelectedNode == null)
            {
                MessageBox.Show("请选择节点！");
                return;
            }
            _curNode = (_curNode == null) ? _root : _curNode;
            frmEditNode.DisplayWindow(treeView.SelectedNode, _menuID, _moidfyStatus, _curNode);
        }

        private void UpdateLevel(TreeNode DropNode)
        {
            string q = "SELECT * From usrSBGGFLZYB WHERE ID=" + _tr.Tag;
            DataTable dt = _data.GetDataTable(q, _menuID);
            if (Utils.IsEmpty(dt))
                return;

            try
            {
                string lbbm = dt.Rows[0]["LBBM"].ToString();
                q = "SELECT * From usrSBGGFLZYB WHERE FQLBBM='" + lbbm + "'";
                DataTable dtChild = _data.GetDataTable(q, _menuID);
                if (Utils.IsEmpty(dtChild))
                    return;
                for (int i = 0; i < dtChild.Rows.Count; i++)
                {
                    DataRow drChild = dtChild.Rows[i];
                    GlobalData.SetDefaultFieldsValue(drChild, RunWay.Auto, true, GKBZ.已检验);
                    drChild["LBJB"] = DropNode.Level + _index;//选定节点下子节点深度加一
                    _data.DataImport("usrSBGGFLZYB", dtChild, ModifyType.Update, GlobalData.CommonMenuID);
                }
                _index++;
                _tr = _tr.FirstNode;//获取子节点
                UpdateLevel(DropNode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDrag_Click(object sender, EventArgs e)
        {
            treeView.CheckBoxes = !treeView.CheckBoxes;
            this.treeView.ItemDrag -= new System.Windows.Forms.ItemDragEventHandler(this.treeView_ItemDrag);
            this.treeView.DragDrop -= new System.Windows.Forms.DragEventHandler(this.treeView_DragDrop);
            this.treeView.DragEnter -= new System.Windows.Forms.DragEventHandler(this.treeView_DragEnter);
            this.treeView.DragOver -= new System.Windows.Forms.DragEventHandler(this.treeView_DragOver);

            if (treeView.CheckBoxes)
            {
                this.treeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView_ItemDrag);
                this.treeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView_DragDrop);
                this.treeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView_DragEnter);
                this.treeView.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView_DragOver);
            }
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            _curNode = e.Node;
            DataTools.SetTreeNodesBackColor(treeView, _curNode);
            if ((_frmType == FormType.SBFL) || _curNode == null)
                return;
            frmSBFLRule.DisplayWindow(_curNode.ToolTipText);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F3)
            {
                frmFind.DisplayWindow();
                if (frmFind.DialogResult != DialogResult.OK)
                    return true;
                string temp = frmFind.QueryValue;
                _mTree.FindNodeValue(temp,treeView);
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void frmSBFLTree_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_frmType == FormType.SBFLDesign)
            {
                frmSBFLRule.CheckModifyStatus();
            }
        }
    }
}