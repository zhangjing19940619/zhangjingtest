using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Deduce.Common.Components;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;
using Deduce.DMIP.ResourceManage;

namespace Deduce.DMIP.ResourceSync
{
    public delegate void LBClickDelegateHander(TreeNode NdMessage);
    public partial class frmLBModify : ExtendDataForm
    {
        public event LBClickDelegateHander ClickEvent;
        private readonly SBQueryHelper _helper;
        private readonly frmSearchBox _frmFind;
        private readonly ModuleTree _mTree;
        private TreeNode _root = null;
        private DataTable _dtNew = null;
        protected string _menuID = "";
        private static TreeNode _selectedLB = null;
        private static Dictionary<string, string> _checkedParents = new Dictionary<string, string>();

        public frmLBModify(string menuID, string gglbMC, SBQueryHelper helper, frmSearchBox frmFind, ModuleTree mTree)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _frmFind = frmFind ?? throw new ArgumentNullException(nameof(frmFind));
            _mTree = mTree ?? throw new ArgumentNullException(nameof(mTree));

            InitializeComponent();
            _isEscapeClose = true;
            InitData(menuID, gglbMC);
        }
        private void InitData(string menuID, string gglbMC)
        {
            if (_checkedParents != null || _checkedParents.Keys.Count != 0)
                _checkedParents.Clear();

            _dtNew = SyncSBFL.GetDatatable(_menuID);
            if (_dtNew == null)
            {
                MessageBox.Show("无法加载三板公告分类数据");
                return;
            }
            _menuID = menuID;
            BuildTree(menuID);
            //重新打开时，将已选择的进行标注
            if (gglbMC != "")
            {
                foreach (TreeNode ckNode in treeGGLB.Nodes)
                {
                    CheckBefor(ckNode, gglbMC);
                }
            }
            frmSearchBox.BindData(_frmFind, _dtNew, "LBMC", "ID");
            this.Show();
        }

        private void CheckBefor(TreeNode treeNode, string ckNodes)
        {
            foreach (TreeNode tn in treeNode.Nodes)
            {
                if (ckNodes.Contains(tn.Text))
                    tn.Checked = true;
                CheckBefor(tn, ckNodes);
            }
        }
        public void BuildTree(string menuID)
        {
            this.treeGGLB.Nodes.Clear();
            _root = new TreeNode("三板公告分类指引");
            treeGGLB.Nodes.Add(_root);
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
            treeGGLB.ExpandAll();
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

        private void treeArchive_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckLBNode(e.Node);
        }
        /// <summary>
        /// 选中替代节点
        /// </summary>
        /// <param name="curNode"></param>
        private void CheckLBNode(TreeNode curNode)
        {
            if (curNode == null)
                return;

            if (curNode.Nodes.Count > 0)
            {
                MessageBox.Show(curNode.Text + " 包含子节点，请重新选择类别!");
                return;
            }

            try
            {
                treeGGLB.BeforeCheck -= new TreeViewCancelEventHandler(treeGGLB_BeforeCheck);
                _selectedLB = null;
                if (!curNode.Checked)
                    return;

                foreach (TreeNode ckNode in treeGGLB.Nodes)
                {
                    Check(ckNode, curNode.Text);
                    _selectedLB = curNode;
                    GetChkNodeParent(curNode);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Check(TreeNode treeNode, string lbName)
        {
            foreach (TreeNode tn in treeNode.Nodes)
            {
                if (tn.Checked && tn.Text != lbName)
                {
                    tn.Checked = false;
                    return;
                }
                Check(tn, lbName);
            }
        }

        private void GetChkNodeParent(TreeNode treeNode)
        {
            if (treeNode == null)
                return;

            if (!_checkedParents.ContainsKey(treeNode.ToolTipText))
                _checkedParents.Add(treeNode.ToolTipText, treeNode.Text + "/" + treeNode.Level);

            while (treeNode.Parent != null)
            {
                if (!_checkedParents.ContainsKey(treeNode.Parent.ToolTipText)
                    && !Utils.IsEmpty(treeNode.Parent.ToolTipText))
                {
                    _checkedParents.Add(treeNode.Parent.ToolTipText, treeNode.Parent.Text + "/" + treeNode.Parent.Level);
                }
                treeNode = treeNode.Parent;
            }
        }
        private void btnOK_Click(object sender, System.EventArgs e)
        {
            if (_selectedLB == null && DialogResult.No != MessageBox.Show("不选择任何一个类别吗?", "更改类别", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                return;

            if (ClickEvent != null)
                ClickEvent(_selectedLB);

            this.Close();
        }
        public static TreeNode SelectedLBNode
        {
            get { return _selectedLB; }
        }

        public static Dictionary<string, string> CheckedParents
        {
            get { return _checkedParents; }
        }
        private void treeGGLB_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            _selectedLB = e.Node;
            CategoryNode node = e.Node as CategoryNode;
            if (node == null || node.Parent == null || !node.Parent.Checked)
                return;

            e.Cancel = true;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F3)
            {
                _frmFind.DisplayWindow();
                if (_frmFind.DialogResult != DialogResult.OK)
                    return true;
                string temp = _frmFind.QueryValue;
                _mTree.FindNodeValue(temp, treeGGLB);
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void frmLBModify_Shown(object sender, EventArgs e)
        {
            if (_checkedParents != null || _checkedParents.Keys.Count != 0)
                _checkedParents.Clear();
        }

        private void treeGGLB_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            e.Node.Checked = true;
            CheckLBNode(e.Node);
        }
    }
}
