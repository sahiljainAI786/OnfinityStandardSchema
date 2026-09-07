using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DBase;
using VIS.Helpers;


namespace VA005.Models
{
    public class VA005_ProductHelper
    {
        Ctx ctx;
        public VA005_ProductHelper(Ctx ctx)
        {
            this.ctx = ctx;
        }

        public MTree GetMenuTree(int AD_Tree_ID, bool editable, bool onDemandTree, int nodeID)
        {
            MTree vTree = null;
            if (onDemandTree)
            {
                vTree = new MTree(ctx, AD_Tree_ID, editable, true, null, true, 180, 0);
            }

            return vTree;
        }

        public List<SetTree> GetMenuTreeUI(SetTree trees, VTreeNode vObject, string tbname)
        {
            List<SetTree> obj2 = new List<SetTree>();
            SetTree treees = new SetTree()
            {
                text = vObject.SetName,
                TableName = tbname,
                IsSummary = true,
                NodeID = vObject.Node_ID,
                IsActive = true,
                ImageSource = "Cmb Text",
                ParentID = vObject.Parent_ID,
                TreeParentID = vObject.Parent_ID,
                imageIndegator = vObject.ImageKey
            };
            obj2.Add(treees);
            CreateTree1(treees, vObject.Nodes);
            return obj2;
        }

        private string CreateTree1(SetTree trees, System.Windows.Forms.TreeNodeCollection treeNodeCollection)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in treeNodeCollection)
            {
                VTreeNode vt = (VTreeNode)item;
                if (vt.IsSummary)
                {
                    SetTree newTrees = new SetTree();
                    if (trees.items == null)
                    {
                        trees.expanded = false;
                        trees.items = new List<SetTree>();
                    }
                    trees.items.Add(newTrees);
                    sb.Append(GetSummaryItemStart(newTrees, vt.Parent_ID, vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName)));
                    sb.Append(CreateTree1(newTrees, ((System.Windows.Forms.TreeNode)vt).Nodes));
                }
                else
                {
                    sb.Append(GetTreeItem(trees, vt.Parent_ID, vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), vt.ImageKey, vt.GetAction(), vt.GetActionID(), vt.OnBar));
                }
            }

            return sb.ToString();
        }

        private string GetSummaryItemStart(SetTree newTrees, int parentID, int id, string text, string windowNo = "")
        {
            var h = "";
            //h += " { text: '" + text + "', issummary: true , nodeid:" + id + ",items:[";
            newTrees.text = text;
            newTrees.IsSummary = true;
            newTrees.NodeID = id;
            newTrees.ParentID = parentID;
            newTrees.TreeParentID = parentID;
            newTrees.ImageSource = "Areas/VA003/Images/orgstr-org.png";

            return h;
        }

        /// <summary>
        /// get leaf node html string
        /// </summary>
        /// <param name="id">id of node</param>
        /// <param name="text">text to display</param>
        /// <param name="img">img to display gainst node</param>
        /// <param name="action">action of node (window , form etc)</param>
        /// <param name="aid">data attribute id</param>
        /// <param name="baseUrl">app url</param>
        /// <returns>html string </returns>
        private string GetTreeItem(SetTree newTrees, int parent_ID, int id, string text, string img, string action, int aid, bool onBar = false)
        {
            if (action.Trim() == "") { action = "W"; img = "W"; }
            var h = "";
            if (newTrees.items == null)
            {
                newTrees.items = new List<SetTree>();
            }
            SetTree nTree = new SetTree();
            nTree.text = text;
            nTree.ParentID = parent_ID;
            nTree.TreeParentID = parent_ID;
            nTree.IsSummary = false;

            nTree.NodeID = id;

            newTrees.items.Add(nTree);

            return h;
        }


    }
    public class SetTree
    {
        public int id { get; set; }
        public string text { get; set; }
        public int AD_Tree_ID { get; set; }
        public string TableName { get; set; }
        public bool IsSummary { get; set; }
        public int NodeID { get; set; }
        public bool IsActive { get; set; }
        public string ImageSource { get; set; }
        public string imageIndegator { get; set; }
        public int ParentID { get; set; }
        public int TreeParentID { get; set; }
        public List<SetTree> items { get; set; }
        public bool expanded { get; set; }
    }
    public class Tree
    {
        public List<SetTree> settree { get; set; }
    }
}