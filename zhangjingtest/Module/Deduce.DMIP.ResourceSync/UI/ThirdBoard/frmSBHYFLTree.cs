using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;
using Deduce.Common.Components;

namespace Deduce.DMIP.ResourceSync
{
    public partial class frmSBHYFLTree : ExtendDataForm
    {
        private frmSearchBox frmFind = new frmSearchBox();
        private ModuleTree _mTree = new ModuleTree();
        private TreeNode _root = null;
        private TreeNode _tr = null;
        private int _index = 2;
        private DataTable _dtNew = null;
        int _type = -1;
        private bool _moidfyStatus;
        private frmSBHYFLNode frmEditNode = null;
        private TreeNode _currNode = null;  //当前操作节点

        public frmSBHYFLTree()
        {
            InitializeComponent();
            _isEscapeClose = true;
        }

        public void DisplayWindow(string type, string menuID)
        {
            _type = Convert.ToInt32(type);
            _menuID = menuID;
            GetAllNodes();
            BuildTree(menuID);
            frmEditNode = new frmSBHYFLNode();
            //this.treeView.AutoScrollOffset = new System.Drawing.Point(100, 100);
            this.treeView.Select();
            this.MdiParent = null;
            frmSearchBox.BindData(frmFind, _dtNew, "HYMC", "ID");            
            this.ShowDialog();
        }

        private void BuildTree(string menuID)
        {
            this.treeView.Nodes.Clear();
            TreeNode _root = new TreeNode("三板行业分类指引");
            treeView.Nodes.Add(_root);
            _root.Expand();
            DataRow[] rs = _dtNew.Select("HYJB='1' AND HYFLBZ='" + _type.ToString() + "'");
            foreach (DataRow dr in rs)
            {
                TreeNode tr = new TreeNode(dr["HYMC"].ToString());
                tr.Tag = dr["ID"].ToString();
                tr.Name = dr["ZSSM"].ToString();
                tr.Expand();
                _root.Nodes.Add(tr);
                TreeView_AddChildNodes(tr, dr["HYBM"].ToString(), menuID);

            }
        }

        private void TreeView_AddChildNodes(TreeNode node, string fhybm, string menuID)
        {
            DataRow[] rs = _dtNew.Select("FQHYBM='" + fhybm + "'");
            foreach (DataRow dr in rs)
            {
                TreeNode tr = new TreeNode(dr["HYMC"].ToString());
                tr.Tag = dr["ID"].ToString();
                tr.Name = dr["ZSSM"].ToString();
                node.Nodes.Add(tr);
                TreeView_AddChildNodes(tr, dr["HYBM"].ToString(), menuID);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _moidfyStatus = true;
            if (treeView.SelectedNode == null)
            {
                MessageBox.Show("请选择节点！");
                return;
            }
            _currNode = (_currNode == null) ? _root : _currNode;
            frmEditNode.DisplayWindow(treeView.SelectedNode, _menuID, _moidfyStatus, _currNode, _type);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditNode();
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
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
            _currNode = (_currNode == null) ? _root : _currNode;            
            frmEditNode.DisplayWindow(treeView.SelectedNode, _menuID, _moidfyStatus, _currNode, _type);
        }

        private void treeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = (e.Data.GetDataPresent(typeof(TreeNode))) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void treeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        //拖放完成
        private void treeView_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode myNode = (TreeNode)(e.Data.GetData(typeof(TreeNode)));
            TreeNode DropNode = GetDropNode(e);
            if (!e.Data.GetDataPresent(typeof(TreeNode)))
            {
                MessageBox.Show("error");
                return;
            }


            if (DropNode == null)
            {
                MessageBox.Show("请选择目标节点！");
                return;
            }
            if (!myNode.Checked)
            {
                MessageBox.Show("请选择要拖动的节点！");
                return;
            }


            // 1.目标节点不是空。2.目标节点不是被拖拽接点的字节点。3.目标节点不是被拖拽节点本身
            if (DropNode.Parent != myNode && DropNode != myNode)
            {
                if (MessageBox.Show("确定要拖动到“" + DropNode.Text + "”节点下吗？", "提示", MessageBoxButtons.OKCancel) != DialogResult.OK)
                    return;
                try
                {
                    string q = "SELECT * From usrSBHYFLZYB WHERE ID=" + DropNode.Tag;
                    DataTable dtParent = _data.GetDataTable(q, _menuID);
                    string FQHYBM = (dtParent == null) ? "0" : dtParent.Rows[0]["HYBM"].ToString();
                    q = " SELECT * From usrSBHYFLZYB WHERE ID=" + myNode.Tag;
                    DataTable dt = _data.GetDataTable(q, _menuID);
                    if (Utils.IsEmpty(dt))
                        return;
                    DataRow dr = dt.Rows[0];
                    GlobalData.SetDefaultFieldsValue(dr, RunWay.Auto, true, GKBZ.已检验);
                    dr["ID"] = dt.Rows[0]["ID"];
                    dr["HYJB"] = DropNode.Level + 1;
                    dr["FQHYBM"] = FQHYBM;
                    _data.DataImport("usrSBHYFLZYB", dt, ModifyType.Update, GlobalData.CommonMenuID);
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
                myNode.Checked = false;
            }
        }

        //获取目标节点
        private TreeNode GetDropNode(DragEventArgs e)
        {
            Point position = new Point(0, 0);
            position.X = e.X;
            position.Y = e.Y;
            position = treeView.PointToClient(position);
            return this.treeView.GetNodeAt(position);
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _currNode = treeView.SelectedNode;
        }

        private void GetAllNodes()
        {
            string q = "select * from usrSBHYFLZYB where HYFLBZ=" + _type;
            DataTable dtTree = _data.GetDataTable(q, _menuID);
            if (Utils.IsEmpty(dtTree))
                return;
            _dtNew = dtTree.Copy();
        }

        private void UpdateLevel(TreeNode DropNode)
        {
            try
            {
                string q = "SELECT * From usrSBHYFLZYB WHERE ID=" + _tr.Tag;
                DataTable dt = _data.GetDataTable(q, _menuID);
                if (Utils.IsEmpty(dt))
                    return;

                string hybm = dt.Rows[0]["HYBM"].ToString();
                q = "SELECT * From usrSBHYFLZYB WHERE HYFLBZ=" + _type + " and FQHYBM='" + hybm + "'";
                DataTable dtChild = _data.GetDataTable(q, _menuID);
                if (Utils.IsEmpty(dtChild))
                    return;
                foreach (DataRow dr in dtChild.Rows)
                {
                    GlobalData.SetDefaultFieldsValue(dr, RunWay.Manual, true, GKBZ.已检验);
                    dr["HYJB"] = DropNode.Level + _index;//选定节点下子节点深度加一
                    _data.DataImport("usrSBHYFLZYB", dtChild, ModifyType.Update, GlobalData.CommonMenuID);
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

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            Point pt = ((TreeView)(sender)).PointToClient(new Point(e.X, e.Y));
            TreeNode objNode = (TreeNode)((TreeView)(sender)).GetNodeAt(pt);
            treeView.SelectedNode = objNode;
        }

        private void btnDrag_Click(object sender, EventArgs e)
        {
            treeView.CheckBoxes = !treeView.CheckBoxes; ;
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
            _currNode = e.Node;
            DataTools.SetTreeNodesBackColor(treeView, _currNode);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F3)
            {
                frmFind.DisplayWindow();
                if (frmFind.DialogResult != DialogResult.OK)
                    return true;
                string temp = frmFind.QueryValue;
                _mTree.FindNodeValue(temp, treeView);
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}
