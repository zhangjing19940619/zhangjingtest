using Deduce.Common.Components;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;
using Deduce.DMIP.ResourceManage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace Deduce.DMIP.ResourceSync
{
    public delegate void ClickDelegateHander(string message);
    public partial class frmSelectGGLB : ExtendDataForm
    {
        public event ClickDelegateHander ClickEvent;
        List<string> _checkedNodes = new List<string>();
        DataTable _dtSelectedGGLB = null;
        DataTable _dtNew = null;
        TreeNode _root = null;
        static List<string> _ckNodes = new List<string>();
        private frmSearchBox frmFind = new frmSearchBox();
        private ModuleTree _mTree = new ModuleTree();
        public frmSelectGGLB(string menuID)
        {
            InitializeComponent();
            _isEscapeClose = true;
            InitData(menuID);
        }
        private void InitData(string menuID)
        {
            _dtSelectedGGLB = Utils.TableAddColumns("类别");
            _dtNew = SyncSBFL.GetDatatable(_menuID);
            if (_dtNew == null)
            {
                MessageBox.Show("无法加载三板公告分类数据");
                return;
            }
            _menuID = menuID;
            BuildTree(menuID);
            //重新打开时，将已选择的节点进行标注
            if (_ckNodes.Count != 0)
            {
                foreach (TreeNode ckNode in treeArchive.Nodes)
                {
                    Check(ckNode, _ckNodes);
                }
            }
            frmSearchBox.BindData(frmFind,_dtNew, "LBMC", "ID");
            this.Show();
        }

        private void Check(TreeNode treeNode, List<string> ckNodes)
        {
            foreach (TreeNode tn in treeNode.Nodes)
            {
                if (ckNodes.Contains(tn.Text))
                    tn.Checked = true;
                Check(tn, ckNodes);
            }
        }
        public void BuildTree(string menuID)
        {
            this.treeArchive.Nodes.Clear();
            _root = new TreeNode("三板公告分类指引");
            treeArchive.Nodes.Add(_root);
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
            treeArchive.ExpandAll();
        }
        public void TreeView_AddChildNodes(TreeNode node, string lbbm, string menuID)
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
        private void SetChildNodesStatus(TreeNodeCollection collection, bool enable)
        {
            if (collection == null || collection.Count <= 0)
                return;

            foreach (TreeNode node in collection)
            {
                node.Checked = false;
                node.ForeColor = (enable) ? Color.Black : Color.Gray;
                SetChildNodesStatus(node.Nodes, enable);
            }
        }
        private void setChildNodeCheckedState(TreeNode currNode, bool state)
        {
            TreeNodeCollection nodes = currNode.Nodes;
            if (nodes.Count <= 0)
                return;

            foreach (TreeNode tn in nodes)
            {
                tn.Checked = state;
                tn.ForeColor = (state == false) ? Color.Black : Color.Gray;
                setChildNodeCheckedState(tn, state);
            }
        }

        private void treeArchive_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            CategoryNode node = e.Node as CategoryNode;
            if (node == null || node.Parent == null || !node.Parent.Checked)
                return;

            setChildNodeCheckedState(e.Node, e.Node.Checked);
            e.Cancel = true;
        }
        private void treeArchive_AfterCheck(object sender, TreeViewEventArgs e)
        {
            SelectLBNodes(e.Node);
        }

        private void SelectLBNodes(TreeNode curNode)
        {
            if (curNode == null)
                return;

            try
            {
                string nodeValue = curNode.Text;
                treeArchive.BeforeCheck -= new TreeViewCancelEventHandler(treeArchive_BeforeCheck);
                if (curNode.Checked)
                {
                    GetCheckedNodes(curNode, true);
                    setChildNodeCheckedState(curNode, true);
                }
                else
                {
                    GetCheckedNodes(curNode, false);
                    setChildNodeCheckedState(curNode, false);
                }
                treeArchive.BeforeCheck += new TreeViewCancelEventHandler(treeArchive_BeforeCheck);
                GetSelectedLBTable(_checkedNodes);
                gridSelectFileType.DataSource = _dtSelectedGGLB;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void GetCheckedNodes(TreeNode node, bool IsChecked)
        {
            if (IsChecked)
            {
                if (node.Nodes.Count == 0)
                {
                    if (!_checkedNodes.Contains(node.Text))
                        _checkedNodes.Add(node.Text);
                    return;
                }
                foreach (TreeNode nd in node.Nodes)
                {
                    if (nd.Nodes.Count == 0)
                    {
                        if (!_checkedNodes.Contains(nd.Text))
                            _checkedNodes.Add(nd.Text);
                    }
                    else
                        GetCheckedNodes(nd, true);
                }
            }
            else
            {
                if (node.Nodes.Count == 0)
                {
                    if (_checkedNodes.Contains(node.Text))
                        _checkedNodes.Remove(node.Text);
                    return;
                }
                foreach (TreeNode nd in node.Nodes)
                {
                    if (nd.Nodes.Count == 0)
                    {
                        if (_checkedNodes.Contains(nd.Text))
                            _checkedNodes.Remove(nd.Text);
                    }
                    else
                        GetCheckedNodes(nd, false);
                }
            }
        }
        private void GetSelectedLBTable(List<string> lbs)
        {
            if (lbs == null || lbs.Count == 0)
            {
                _dtSelectedGGLB.Clear();
                return;
            }
            _dtSelectedGGLB.Clear();
            foreach (string lb in lbs)
            {
                DataRow dr = _dtSelectedGGLB.NewRow();
                dr["类别"] = lb;
                DataRow[] drs = _dtSelectedGGLB.Select("类别='" + lb.Trim() + "'");
                if (drs.Count() == 0)
                    _dtSelectedGGLB.Rows.Add(dr);
            }
        }
        public static List<string> CheckedLB
        {
            get { return _ckNodes; }
            set { _ckNodes = value; }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            string msg = "";
            _checkedNodes.Distinct().ToList().ForEach(type => { msg += (type + ";"); });
            if (ClickEvent != null) //判断事件是否被注册
                ClickEvent(msg);
            _ckNodes = _checkedNodes;
            this.Close(DialogResult.OK);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F3)
            {
                frmFind.DisplayWindow();
                if (frmFind.DialogResult != DialogResult.OK)
                    return true;
                string temp = frmFind.QueryValue;
                _mTree.FindNodeValue(temp, treeArchive);
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void treeArchive_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            e.Node.Checked = true;
            SelectLBNodes(e.Node);
        }
    }
}

