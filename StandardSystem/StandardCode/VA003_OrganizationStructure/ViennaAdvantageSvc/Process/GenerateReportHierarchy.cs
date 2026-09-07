using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    public class GenerateReportHierarchy : SvrProcess
    {

        private int seqCounter = 0;
        private List<int> lstLegalEntities = new List<int>();
        private List<int> lstSummary = new List<int>();
        private List<TreeStructure> LstTrees = new List<TreeStructure>();
        private List<TreeStructure> finalTree = new List<TreeStructure>();
        private List<TreeHierarchy> lstOrgHie = new List<TreeHierarchy>();
        private List<int> lstInsertedItems = new List<int>();
        private List<int> lstNewTreeInsertedItems = new List<int>();
        private List<int> lstInActiveOrg = new List<int>();
        private Dictionary<string, string> orgValues = new Dictionary<string, string>();
        private DataSet dsOrgInfo = null;
        private int Ref_Tree_ID = 0;
        private int heirarchyID = 0;
        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
            }
        }

        protected override string DoIt()
        {
            string sql = @"SELECT AD_OrgInfo.AD_Org_ID,
                                              AD_OrgInfo.parent_org_id 
                                            FROM AD_OrgInfo
                                            JOIN AD_Org
                                            ON AD_Org.AD_Org_ID =AD_OrgInfo.AD_Org_ID
                                            WHERE AD_Org.IsCostCenter='N' AND AD_Org.IsProfitCenter='N' 
                                            ORDER BY ad_org.issummary DESC , AD_OrgInfo.AD_Org_ID ";
            sql = MRole.GetDefault(GetCtx()).AddAccessSQL(sql, "AD_Org", true, true);

            dsOrgInfo = DB.ExecuteDataset(sql);

            //            if (ds != null && ds.Tables[0].Rows.Count > 0)
            //            {

            //                sql = "SELECT AD_Tree_ID FROM AD_TREE WHERE Name='Standard " + GetCtx().GetAD_Client_Name() + " Organization'";

            //                object treeID = DB.ExecuteScalar(sql);

            //                //MTree newTree = null;
            //                //int parentID = 0;
            //                //if (treeID == null || treeID == DBNull.Value)           //check if tree already exist, if not create new
            //                //{
            //                //    newTree = new MTree(GetCtx(), 0, null);
            //                //    string treeName = "Standard " + GetCtx().GetAD_Client_Name() + " Organization";
            //                //    newTree.SetName(treeName);
            //                //    newTree.SetAD_Table_ID(MTable.Get_Table_ID("AD_Org"));
            //                //    newTree.SetTreeType("OO");
            //                //    newTree.Save();
            //                //    parentID = InsertHeaderNode(newTree);
            //                //}
            //                //else
            //                //{
            //                //    parentID = Convert.ToInt32(DB.ExecuteScalar("SELECT AD_Org_ID FROM AD_Org WHERE Name='HeaderSummaryGroupNode'"));
            //                //    newTree = new MTree(GetCtx(), Convert.ToInt32(treeID), null);
            //                //}
            //                //headerParentID = parentID;
            //                //CreateUpdateTree(ds, newTree, parentID);

            //            }


            GetTree();

            return "Generated Hierarchy";
        }

        private void LoadOrgValues()
        {
            DataSet ds = DB.ExecuteDataset("SELECT AD_ORG_ID, Value FROM AD_Org");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    orgValues[ds.Tables[0].Rows[i]["AD_Org_ID"].ToString()] = ds.Tables[0].Rows[i]["Value"].ToString();
                }
            }
        }

        private void LoadInActiveOrgs()
        {
            DataSet ds = DB.ExecuteDataset("SELECT AD_Org_ID FROM AD_ORg WHERE IsActive='N'");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lstInActiveOrg.Add(Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Org_ID"]));
                }
            }

        }

        public List<TreeStructure> GetTree()
        {
            LoadLegalEntities();
            LoadInActiveOrgs();
            LoadOrgValues();

            int orgWindowID = Convert.ToInt32(DB.ExecuteScalar("SELECT AD_Window_ID from AD_Window WHERE Name='Organization'", null, null));

            if (!(bool)MRole.GetDefault(GetCtx()).GetWindowAccess(orgWindowID))
            {
                return null;
            }

            int orgTableID = MTable.Get_Table_ID("AD_Org");

            string sql = "SELECT AD_Tree_ID FROM AD_Tree "
          + "WHERE AD_Client_ID=" + GetCtx().GetAD_Client_ID() + " AND AD_Table_ID=" + orgTableID + " AND IsActive='Y' AND IsAllNodes='Y' "
          + "ORDER BY IsDefault DESC, AD_Tree_ID";

            object AD_Tree_ID = DB.ExecuteScalar(sql, null, null);

            if (AD_Tree_ID != null && AD_Tree_ID != DBNull.Value)
            {

                MTree tree = new MTree(GetCtx(), Convert.ToInt32(AD_Tree_ID), true, true, null);


                TreeStructure trees = new TreeStructure();
                trees.AD_Tree_ID = Convert.ToInt32(AD_Tree_ID);
                LstTrees.Add(trees);
                string html = GetMenuTreeUI(trees, tree.GetRootNode(), tree.GetNodeTableName());
                //  m.dispose();
                //return LstTrees;
                List<TreeStructure> retFinalTree = new List<TreeStructure>();
                LoadOrgStructure();

                retFinalTree.Add(LstTrees[0]);

                if (dsOrgInfo != null && dsOrgInfo.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsOrgInfo.Tables[0].Rows.Count; i++)
                    {
                        TreeStructure ts = LstTrees.Find(a => a.NodeID == Convert.ToInt32(dsOrgInfo.Tables[0].Rows[i]["AD_Org_ID"]));

                        if (ts == null)
                        {
                            continue;
                        }

                        if (Util.GetValueOfInt(dsOrgInfo.Tables[0].Rows[i]["parent_org_id"]) > 0)
                        {
                            ts.ParentOrgInfoID = Util.GetValueOfInt(dsOrgInfo.Tables[0].Rows[i]["parent_org_id"]);
                            ts.ParentID = 0;
                        }
                    }
                }


                MakeTree();

                if (finalTree.Count > 0)
                {
                    for (int i = 0; i < finalTree.Count; i++)
                    {
                        retFinalTree[0].items.Add(finalTree[i]);
                    }
                }


                CreateNewTree(finalTree);

                return retFinalTree;
            }

            return null;

        }

        private void LoadOrgStructure()
        {


            string sql = "SELECT AD_Org_ID, parent_org_id FROM AD_OrgInfo WHERE IsActive='Y'";
            // DataSet ds = DB.ExecuteDataset(MRole.GetDefault(GetCtx()).AddAccessSQL(sql, "AD_OrgInfo", true, true));
            DataSet ds = DB.ExecuteDataset(sql);


            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lstOrgHie.Add(new TreeHierarchy() { NodeID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Org_ID"]), ParentNodeID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["Parent_Org_ID"]) });
                }
            }




        }

        private void MakeTree()
        {
            for (int i = 0; i < lstSummary.Count; i++)
            {
                List<TreeStructure> lstSumaaryTree = LstTrees.FindAll(a => a.ParentID == lstSummary[i]);

                List<TreeStructure> lstparentLess = lstSumaaryTree.FindAll(a => a.ParentID == 0);

                TreeStructure tss = LstTrees.Find(a => a.NodeID == lstSummary[i]);

                if (tss == null || tss.IsSummary && tss.TreeParentID > 0)
                {
                    continue;
                }

                if (lstparentLess != null && lstparentLess.Count > 0)
                {
                    for (int l = 0; l < lstparentLess.Count; l++)
                    {
                        TreeStructure tt = finalTree.Find(aa => aa.NodeID == lstparentLess[l].NodeID);
                        finalTree.Remove(tt);
                        tss.items.Add(tt);
                    }
                }
                lstInsertedItems.Add(tss.NodeID);

                LstTrees.Remove(tss);
                finalTree.Add(tss);

                CreateFinalTree(tss, lstSumaaryTree, true);
            }

            if (LstTrees.Count > 0)
            {
                for (int i = 0; i < LstTrees.Count; i++)
                {
                    if (LstTrees[i].NodeID > 0 && LstTrees[i].ParentID == 0)
                    {
                        List<TreeStructure> newLstParentLess = new List<TreeStructure>();
                        newLstParentLess.Add(LstTrees[i]);
                        var pTree = LstTrees.Find(a => a.NodeID == LstTrees[i].NodeID);
                        insertCurrentnonSummaryNode(finalTree, pTree);
                        InsertNonSummary(newLstParentLess, pTree, 0);
                    }
                }
            }

        }

        private void insertCurrentnonSummaryNode(List<TreeStructure> finaltre, TreeStructure pTree)
        {
            TreeHierarchy th = lstOrgHie.Find(aa => aa.NodeID == pTree.NodeID);
            if (th != null && th.ParentNodeID == 0)
            {
                finalTree.Add(pTree);
                lstInsertedItems.Add(pTree.NodeID);
            }
        }

        private void CreateFinalTree(TreeStructure ts, List<TreeStructure> lstparentLess, bool isSummary)
        {
            for (int j = 0; j < lstparentLess.Count; j++)
            {
                //if (lstInActiveOrg.IndexOf(lstparentLess[j].NodeID) > -1)
                //{
                //    continue;
                //}

                var pTree = LstTrees.Find(a => a.NodeID == lstparentLess[j].NodeID);
                if (pTree != null)
                {
                    // LstTrees.Remove(pTree);
                    if (ts.items == null)
                    {
                        ts.items = new List<TreeStructure>();
                    }
                    ts.items.Add(pTree);
                    lstInsertedItems.Add(pTree.NodeID);
                }

                if (isSummary)
                {
                    List<TreeStructure> newChild = LstTrees.FindAll(aa => aa.TreeParentID == lstparentLess[j].NodeID);
                    List<TreeStructure> newLstParentLess = new List<TreeStructure>();
                    if (pTree != null)
                    {
                        if (newChild != null && newChild.Count > 0)
                        {
                            for (int k = 0; k < newChild.Count; k++)
                            {
                                if (!newChild[k].IsSummary && lstOrgHie.Find(aa => aa.NodeID == newChild[k].NodeID).ParentNodeID > 0 && lstInsertedItems.IndexOf((lstOrgHie.Find(aa => aa.NodeID == newChild[k].NodeID)).ParentNodeID) == -1)
                                {
                                    continue;
                                }


                                newLstParentLess.Add(newChild[k]);
                                if (newChild[k].IsSummary)
                                {
                                    CreateFinalTree(pTree, newLstParentLess, true);
                                }
                                else
                                {
                                    CreateFinalTree(pTree, newLstParentLess, false);
                                }
                            }
                        }
                    }

                    InsertNonSummary(lstparentLess, pTree, j);
                }
                else
                {
                    InsertNonSummary(lstparentLess, pTree, j);
                }
            }
        }

        private void InsertNonSummary(List<TreeStructure> lstparentLess, TreeStructure pTree, int j)
        {
            TreeHierarchy th = lstOrgHie.Find(aa => aa.ParentNodeID == lstparentLess[j].NodeID);

            if (th != null)
            {
                List<TreeHierarchy> newChild = lstOrgHie.FindAll(aa => aa.ParentNodeID == lstparentLess[j].NodeID);

                List<TreeStructure> newLstParentLess = new List<TreeStructure>();
                if (pTree != null)
                {
                    if (newChild != null && newChild.Count > 0)
                    {
                        for (int k = 0; k < newChild.Count; k++)
                        {
                            if (lstInsertedItems.IndexOf(newChild[k].ParentNodeID) == -1)
                            {
                                continue;
                            }
                            TreeStructure tss = LstTrees.Find(a => a.NodeID == newChild[k].NodeID);
                            lstInsertedItems.Add(newChild[k].NodeID);
                            if (tss != null)
                            {
                                // LstTrees.Remove(tss);
                                newLstParentLess.Add(tss);
                                lstInsertedItems.Add(tss.NodeID);
                                if (pTree.items == null)
                                {
                                    pTree.items = new List<TreeStructure>();
                                }
                            }
                        }
                        CreateFinalTree(pTree, newLstParentLess, false);
                    }
                }
            }
        }

        /// <summary>
        /// get Menu Tree html String 
        /// </summary>
        /// <param name="root">Root of tree</param>
        /// <param name="baseUrl">application url</param>
        /// <returns>html string</returns>
        private string GetMenuTreeUI(TreeStructure trees, VTreeNode root, string tableName = "table")
        {
            StringBuilder sb = new StringBuilder("");
            //sb.Append("[{text: '" + root.SetName + "', expanded: true, 'tableName':'" + tableName + "', nodeid: " + root.Node_ID + ", items: [");
            trees.text = root.SetName;
            trees.TableName = tableName;
            trees.NodeID = root.Node_ID;
            trees.ParentID = root.Parent_ID;
            trees.TreeParentID = root.Parent_ID;
            trees.SeqNo = root.SeqNo;
            sb.Append(CreateTree(trees, root.Nodes));

            return sb.ToString();
        }

        /// <summary>
        /// Create Tree
        /// </summary>
        /// <param name="trees">Tree</param>
        /// <param name="treeNodeCollection">Tree Nodes</param>
        /// <returns>String as Message</returns>
        private string CreateTree(TreeStructure trees, System.Windows.Forms.TreeNodeCollection treeNodeCollection)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in treeNodeCollection)
            {
                VTreeNode vt = (VTreeNode)item;

                // Applied  check on tree not to show/create org unit
                MOrg Org = new MOrg(GetCtx(), vt.Node_ID, null);

                if (Org.Get_ColumnIndex("IsOrgUnit") > 0 && Org.IsOrgUnit())
                {
                    continue;
                }

                if (vt.IsSummary)
                {
                    TreeStructure newTrees = new TreeStructure();
                    if (trees.items == null)
                    {
                        trees.items = new List<TreeStructure>();
                    }

                    //trees.items.Add(newTrees);

                    LstTrees.Add(newTrees);

                    sb.Append(GetSummaryItemStart(newTrees, vt.Parent_ID, vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), vt.SeqNo));
                    sb.Append(CreateTree(newTrees, ((System.Windows.Forms.TreeNode)vt).Nodes));
                }
                else
                {

                    sb.Append(GetTreeItem(vt.SeqNo, trees, vt.Parent_ID, vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), vt.ImageKey, vt.GetAction(), vt.GetActionID(), vt.OnBar));
                }
            }

            return sb.ToString();
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
        private string GetTreeItem(int seqNo, TreeStructure newTrees, int parent_ID, int id, string text, string img, string action, int aid, bool onBar = false)
        {
            var h = "";
            //h += " { text: '" + text + "', issummary: false, nodeid:" + id + " },";
            if (newTrees.items == null)
            {
                newTrees.items = new List<TreeStructure>();
            }
            TreeStructure nTree = new TreeStructure();
            LstTrees.Add(nTree);
            //newTrees.items.Add(nTree);
            nTree.text = text;
            nTree.ParentID = parent_ID;
            nTree.TreeParentID = parent_ID;
            nTree.IsSummary = false;
            nTree.SeqNo = seqNo;
            if (lstLegalEntities.IndexOf(id) > -1)
            {
                nTree.IsLegal = true;
            }
            nTree.NodeID = id;

            return h;
        }

        /// <summary>
        /// summary node start html string 
        /// </summary>
        /// <param name="id">id of node</param>
        /// <param name="text">text display</param>
        /// <returns>html string</returns>
        private string GetSummaryItemStart(TreeStructure newTrees, int parentID, int id, string text, int SeqNo)
        {
            var h = "";

            newTrees.text = text;
            newTrees.IsSummary = true;
            newTrees.NodeID = id;
            newTrees.SeqNo = SeqNo;
            newTrees.ParentID = parentID;
            newTrees.TreeParentID = parentID;
            lstSummary.Add(id);

            return h;
        }


        private void LoadLegalEntities()
        {
            DataSet ds = DB.ExecuteDataset("SELECT AD_Org_ID FROM AD_ORg WHERE IsActive='Y' AND islegalentity='Y'");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lstLegalEntities.Add(Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Org_ID"]));
                }
            }

        }

        private void CreateNewTree(List<TreeStructure> retFinalTree)
        {
            int orgTableID = MTable.Get_Table_ID("AD_Org");

            string sql = @"SELECT AD_Tree_ID FROM AD_Tree "
              + "WHERE AD_Client_ID=" + GetCtx().GetAD_Client_ID() + " AND AD_Table_ID=" + orgTableID + " AND IsActive='Y' AND IsAllNodes='Y' "
              + "ORDER BY IsDefault DESC, AD_Tree_ID";

            object treeID = DB.ExecuteScalar(sql);

            if (treeID != null && treeID != DBNull.Value)
            {
                Ref_Tree_ID = Convert.ToInt32(treeID);
                sql = "select ad_tree_org_id , created, PA_hierarchy_ID from PA_hierarchy where IsActive='Y' AND ref_tree_org_id=" + treeID;
                DataSet ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    heirarchyID = Convert.ToInt32(ds.Tables[0].Rows[0]["PA_hierarchy_ID"]);


                    sql = "SELECT Created FROM AD_TRee WHERE AD_Tree_ID=" + Convert.ToInt32(ds.Tables[0].Rows[0]["ad_tree_org_id"]);
                    object val = DB.ExecuteScalar(sql);

                    if (val != null && val != DBNull.Value && Convert.ToDateTime(val) <= new DateTime(2015, 10, 10))
                    {
                        sql = "UPDATE AD_Tree Set IsActive='N',name=name||'_NN' WHERE AD_Tree_ID=" + Convert.ToInt32(ds.Tables[0].Rows[0]["ad_tree_org_id"]);
                        DB.ExecuteQuery(sql);

                        treeID = null;
                    }
                    else
                    {
                        treeID = Convert.ToInt32(ds.Tables[0].Rows[0]["ad_tree_org_id"]);
                    }
                }
                else
                {
                    treeID = null;
                }

            }
            else
            {
                return;
            }




            MTree newTree = null;
            int parentID = 0;
            if (treeID == null || treeID == DBNull.Value)           //check if tree already exist, if not create new
            {
                string treeName = Msg.GetMsg(GetCtx(), "VA003_Standard") + " " + GetCtx().GetAD_Client_Name() + Msg.GetMsg(GetCtx(), "Organization");

                object tName = DB.ExecuteScalar("SELECT AD_Tree_ID FROM AD_Tree WHERE IsActive='Y' AND AD_Client_ID=" + GetCtx().GetAD_Client_ID() + " AND name ='" + treeName + "'");
                if (tName != null && tName != DBNull.Value && tName != "")
                {
                    newTree = new MTree(GetCtx(), Convert.ToInt32(tName), null);
                }
                else
                {
                    newTree = new MTree(GetCtx(), 0, null);
                }

                newTree.SetName(treeName);
                newTree.SetAD_Table_ID(MTable.Get_Table_ID("AD_Org"));
                newTree.SetTreeType("OO");
                newTree.Save();
                parentID = InsertHeaderNode(newTree);
            }
            else
            {
                newTree = new MTree(GetCtx(), Convert.ToInt32(treeID), null);

                object val = DB.ExecuteScalar("SELECT AD_Org_ID FROM AD_Org WHERE AD_Client_ID=" + GetCtx().GetAD_Client_ID() + " AND  value='" + GetCtx().GetAD_Client_ID().ToString() + "HeaderOrgVA003_Consolidate" + "'");

                if (val != null && val != DBNull.Value)
                {
                    parentID = Convert.ToInt32(val);

                    MTreeNode node = new MTreeNode(newTree, parentID);
                    node.SetParent_ID(0);
                    node.SetSeqNo(seqCounter);
                    seqCounter++;
                    node.Save();
                }
                else
                {
                    parentID = InsertHeaderNode(newTree);
                }

            }
            CreateUpdateTree(newTree, parentID, retFinalTree);

            CreateReportHeirarchy(newTree);


            DeleteExtraNodes(newTree.GetAD_Tree_ID());

        }

        private bool IsNodeExistinTree(MTree tree, int nodeID)
        {
            MTreeNode node = MTreeNode.Get(tree, nodeID);
            return !(node == null);
        }

        /// <summary>
        /// Create or Update Tree
        /// </summary>
        /// <param name="tree">Tree</param>
        /// <param name="parentID">Parenet ID</param>
        /// <param name="retFinalTree">Tree Nodes</param>
        private void CreateUpdateTree(MTree tree, int parentID, List<TreeStructure> retFinalTree)
        {
            for (int i = 0; i < retFinalTree.Count; i++)
            {
                int parentIdForChild = retFinalTree[i].NodeID;


                MTreeNode node = null;
                if (!IsNodeExistinTree(tree, Convert.ToInt32(retFinalTree[i].NodeID)))
                {
                    node = new MTreeNode(tree, Convert.ToInt32(retFinalTree[i].NodeID));
                    node.SetParent_ID(parentID);
                    node.SetSeqNo(seqCounter);
                    seqCounter++;
                    node.Save();
                }
                else
                {
                    node = MTreeNode.Get(tree, retFinalTree[i].NodeID);

                    node.SetSeqNo(seqCounter);
                    seqCounter++;
                    node.Save();

                }
                var orgVal = orgValues[retFinalTree[i].NodeID.ToString()];


                if (lstInActiveOrg.Contains(parentIdForChild))
                {
                    if (IsNodeExistinTree(tree, parentIdForChild))
                    {
                        DB.ExecuteQuery("UPDATE AD_TreeNode Set isActive='N' WHERE AD_Tree_ID=" + tree.GetAD_Tree_ID() + " AND Node_ID=" + parentIdForChild);
                        SetInActiveDependent(orgVal, tree.GetAD_Tree_ID(), false);
                        parentIdForChild = createSettings(retFinalTree, i, node, parentID, tree, orgVal);
                        node.Save();
                        InsertChildren(retFinalTree[i].items, tree, parentIdForChild);
                    }

                    continue;
                }
                else
                {
                    DB.ExecuteQuery("UPDATE AD_TreeNode Set isActive='Y' WHERE AD_Tree_ID=" + tree.GetAD_Tree_ID() + " AND Node_ID=" + parentIdForChild);
                    SetInActiveDependent(orgVal, tree.GetAD_Tree_ID(), true);
                }


                int parentIDs = parentID;



                parentIdForChild = createSettings(retFinalTree, i, node, parentID, tree, orgVal);

                node.Save();
                InsertChildren(retFinalTree[i].items, tree, parentIdForChild);

                // Handled issue when no Organization for Summary Level org
                if (retFinalTree[i].IsSummary && retFinalTree[i].items != null && retFinalTree[i].items.Count > 0)
                {
                    int inActvieCount = 0;
                    for (int a = 0; a < retFinalTree[i].items.Count; a++)
                    {
                        if (lstInActiveOrg.IndexOf(retFinalTree[i].items[a].NodeID) > -1)
                        {
                            inActvieCount++;
                        }
                    }
                    if (inActvieCount == retFinalTree[i].items.Count)
                    {
                        DB.ExecuteQuery(" UPDATE AD_TreeNode Set isActive='N' WHERE AD_Tree_ID=" + tree.GetAD_Tree_ID() + " AND Node_ID=" + retFinalTree[i].NodeID);
                    }
                    else
                    {
                        DB.ExecuteQuery(" UPDATE AD_TreeNode Set isActive='Y' WHERE AD_Tree_ID=" + tree.GetAD_Tree_ID() + " AND Node_ID=" + retFinalTree[i].NodeID);
                    }
                }
            }
        }

        private int createSettings(List<TreeStructure> retFinalTree, int i, MTreeNode node, int parentID, MTree tree, string orgVal)
        {
            int parentIDs = 0;
            int parentIdForChild = 0;


            if (!retFinalTree[i].IsSummary)
            {
                parentIDs = CreateUpdateNewSummaryLevel(System.Net.WebUtility.HtmlDecode(retFinalTree[i].text).Replace("'", "''"), parentID, tree, orgVal, retFinalTree[i].NodeID);
                node.SetParent_ID(parentIDs);
                parentIdForChild = parentIDs;
                InsertUpdateCurrentNode(tree, retFinalTree[i].NodeID, parentIdForChild);
            }
            else
            {
                parentIdForChild = node.GetNode_ID();
                node.SetParent_ID(parentID);
            }


            if (!retFinalTree[i].IsSummary && retFinalTree[i].items != null && retFinalTree[i].items.Count > 0)
            {
                parentIdForChild = InsertCostCenter(tree, System.Net.WebUtility.HtmlDecode(retFinalTree[i].text).Replace("'", "''"), parentIDs, orgVal);
            }
            return parentIdForChild;
        }

        private void InsertChildren(List<TreeStructure> items, MTree tree, int parentID)
        {
            int inActiveChildCount = 0;
            if (items != null && items.Count > 0)
            {
                for (int i = 0; i < items.Count; i++)
                {

                    var orgVal = orgValues[items[i].NodeID.ToString()];

                    if (lstInActiveOrg.Contains(items[i].NodeID))
                    {
                        if (IsNodeExistinTree(tree, items[i].NodeID))
                        {
                            DB.ExecuteQuery("UPDATE AD_TreeNode Set isActive='N' WHERE AD_Tree_ID=" + tree.GetAD_Tree_ID() + " AND Node_ID=" + items[i].NodeID);
                            //    DB.ExecuteQuery("DELETE FROM AD_TreeNode WHERE AD_Tree_ID=" + tree.GetAD_Tree_ID() + " AND Node_ID=" + items[i].NodeID);
                            SetInActiveDependent(orgVal, tree.GetAD_Tree_ID(), false);
                        }
                        inActiveChildCount++;
                        //    continue;
                    }
                    else
                    {
                        DB.ExecuteQuery("UPDATE AD_TreeNode Set isActive='Y' WHERE AD_Tree_ID=" + tree.GetAD_Tree_ID() + " AND Node_ID=" + items[i].NodeID);
                        SetInActiveDependent(orgVal, tree.GetAD_Tree_ID(), true);
                    }
                    if (lstNewTreeInsertedItems.IndexOf(items[i].NodeID) > -1)
                    {
                        continue;
                    }



                    int PID = parentID;
                    if (items[i].ParentOrgInfoID == 0 && !items[i].IsSummary)
                    {
                        PID = CreateUpdateNewSummaryLevel(System.Net.WebUtility.HtmlDecode(items[i].text).Replace("'", "''"), parentID, tree, orgVal, items[i].NodeID);
                    }

                    InsertUpdateCurrentNode(tree, items[i].NodeID, PID);
                    lstNewTreeInsertedItems.Add(items[i].NodeID);

                    if (items[i].items != null && !IsCostCenterRequired(items[i].items))
                    {
                        string sql = "UPDATE  AD_TreeNode Set isActive='N' WHERE AD_Tree_ID=" + tree.GetAD_Tree_ID() + " AND Node_ID=(SELECT AD_ORg_ID FROM AD_Org WHERE AD_Client_ID=" + GetCtx().GetAD_Client_ID() + " AND  value='" + orgVal + " " + "VA003_orgUnit" + "')";
                        DB.ExecuteQuery(sql);
                        continue;
                    }

                    if (items[i].items != null && items[i].items.Count > 0 && !items[i].IsSummary)
                    {
                        PID = InsertCostCenter(tree, System.Net.WebUtility.HtmlDecode(items[i].text).Replace("'", "''"), PID, orgVal);
                    }
                    else    // this case occur when we remove childnode of a cost center then sequence not updating properly
                    {
                        DeleteCostCenter(tree, orgVal, PID);
                    }
                    if (PID == parentID && items[i].IsSummary)
                    {
                        PID = items[i].NodeID;
                    }
                    else if (PID == parentID && !items[i].IsSummary)
                    {
                        PID = items[i].ParentID;
                    }

                    InsertChildren(items[i].items, tree, PID);
                }
            }

            if (items != null && items.Count > 0)
            {
                if (inActiveChildCount == items.Count)
                {
                    string sql = "UPDATE  AD_TreeNode Set isActive='N' WHERE AD_Tree_ID=" + tree.GetAD_Tree_ID() + " AND Node_ID=(SELECT Parent_ID FROM AD_TreeNode WHERE Node_ID=" + items[0].NodeID + " AND AD_Tree_ID=" + tree.GetAD_Tree_ID() + ")";// ;
                    DB.ExecuteQuery(sql);
                }
                //else
                //{
                //    string sql = "UPDATE  AD_TreeNode Set isActive='Y' WHERE AD_Tree_ID=" + tree.GetAD_Tree_ID() + " AND Node_ID=(SELECT Parent_ID FROM AD_TreeNode WHERE Node_ID=" + items[0].NodeID + " AND AD_Tree_ID=" + tree.GetAD_Tree_ID() + ")";// ;
                //    DB.ExecuteQuery(sql);
                //}
            }
        }

        private bool IsCostCenterRequired(List<TreeStructure> items)
        {
            bool result = false;
            if (items.Count > 0)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (!lstInActiveOrg.Contains(items[i].NodeID))
                    {
                        result = true;
                        break;
                    }
                }
                return result;
            }
            return false;
        }

        private void SetInActiveDependent(string Name, int treeID, bool isActive)
        {
            if (isActive)
            {
                string sql = "UPDATE  AD_TreeNode Set isActive='Y' WHERE AD_Tree_ID=" + treeID + " AND Node_ID=(SELECT AD_ORg_ID FROM AD_Org WHERE AD_Client_ID=" + GetCtx().GetAD_Client_ID() + " AND value='" + Name + " " + "VA003_Consolidate" + "')";
                DB.ExecuteQuery(sql);

                //sql = "UPDATE  AD_TreeNode Set isActive='Y' WHERE AD_Tree_ID=" + treeID + " AND Node_ID=(SELECT AD_ORg_ID FROM AD_Org WHERE value='" + Name + " " + "VA003_orgUnit" + "')";
                //DB.ExecuteQuery(sql);
            }
            else
            {
                string sql = "UPDATE  AD_TreeNode Set isActive='N' WHERE AD_Tree_ID=" + treeID + " AND Node_ID=(SELECT AD_ORg_ID FROM AD_Org WHERE AD_Client_ID=" + GetCtx().GetAD_Client_ID() + " AND value='" + Name + " " + "VA003_Consolidate" + "')";
                DB.ExecuteQuery(sql);

                //sql = "UPDATE  AD_TreeNode Set isActive='N' WHERE AD_Tree_ID=" + treeID + " AND Node_ID=(SELECT AD_ORg_ID FROM AD_Org WHERE value='" + Name + " " + "VA003_orgUnit" + "')";
                //DB.ExecuteQuery(sql);
            }
        }


        private void DeleteCostCenter(MTree tree, string name, int parent_ID)
        {
            string sql = "SELECT AD_ORg_ID FROM AD_Org WHERE AD_Client_ID=" + GetCtx().GetAD_Client_ID() + " AND  value='" + name + " " + "VA003_orgUnit" + "'";
            var orgID = DB.ExecuteScalar(sql);
            int neworgID = 0;
            if (orgID != null && orgID != DBNull.Value)
            {
                neworgID = Convert.ToInt32(orgID);

                //MTreeNode node = MTreeNode.Get(tree, neworgID);
                //node.Save();
                //node.SetParent_ID(parent_ID);
                //node.SetSeqNo(seqCounter);
                //seqCounter++;
                //node.Save();

                sql = "DELETE FROM AD_TreeNode WHERE AD_Tree_ID=" + tree.GetAD_Tree_ID() + " AND Node_ID=" + neworgID;
                DB.ExecuteQuery(sql);

                sql = "DELETE FROM AD_OrgInfo WHERE AD_ORg_ID=" + neworgID;
                DB.ExecuteQuery(sql);

                sql = "DELETE FROM AD_Org WHERE AD_ORg_ID=" + neworgID;
                DB.ExecuteQuery(sql);

            }
            else
            {
                return;
            }
        }

        private int CreateUpdateNewSummaryLevel(string Name, int parentID, MTree tree, string value, int nodeID)
        {
            string sql = "SELECT AD_ORg_ID FROM AD_Org WHERE AD_Client_ID=" + GetCtx().GetAD_Client_ID() + " AND  value='" + value + " " + "VA003_Consolidate" + "'";
            object orgID = DB.ExecuteScalar(sql);
            int neworgID = 0;
            if (orgID != null && orgID != DBNull.Value)
            {
                neworgID = Convert.ToInt32(orgID);
            }
            else
            {
                neworgID = 0;
            }

            //MOrg newOrg = new MOrg(GetCtx(), Convert.ToInt32(orgID), null);
            //newOrg.SetName(Name + "SummaryGroup");
            //newOrg.SetIsSummary(true);
            //newOrg.Save();
            if (neworgID == 0)
            {
                neworgID = InsertNewOrg((Name + " " + Msg.GetMsg(GetCtx(), "VA003_Consolidate")), "Y", (value + " " + "VA003_Consolidate"));
            }

            MTreeNode node = null;
            if (!IsNodeExistinTree(tree, neworgID))
            {
                node = new MTreeNode(tree, neworgID);

            }
            else
            {
                node = MTreeNode.Get(tree, neworgID);
            }

            node.SetParent_ID(parentID);
            node.SetSeqNo(seqCounter);


            if (lstInActiveOrg.IndexOf(nodeID) > -1)
            {
                node.SetIsActive(false);
            }
            else
            {
                node.SetIsActive(true);
            }

            seqCounter++;
            node.Save();

            return neworgID;
        }

        private void InsertUpdateCurrentNode(MTree tree, int nodeID, int ParentID)
        {

            MTreeNode node = null;
            if (!IsNodeExistinTree(tree, nodeID))
            {
                node = new MTreeNode(tree, nodeID);
            }
            else
            {
                node = MTreeNode.Get(tree, nodeID);

            }
            node.SetParent_ID(ParentID);
            node.SetSeqNo(seqCounter);
            seqCounter++;

            if (lstInActiveOrg.IndexOf(nodeID) > -1)
            {
                node.SetIsActive(false);
            }
            else
            {
                node.SetIsActive(true);
            }
            node.Save(); ;

        }

        private int InsertCostCenter(MTree tree, string name, int parent_ID, string value)
        {
            string sql = "SELECT AD_ORg_ID FROM AD_Org WHERE AD_Client_ID=" + GetCtx().GetAD_Client_ID() + " AND value='" + value + " " + "VA003_orgUnit" + "'";
            var orgID = DB.ExecuteScalar(sql);
            int neworgID = 0;
            if (orgID != null && orgID != DBNull.Value)
            {
                neworgID = Convert.ToInt32(orgID);
            }
            else
            {
                neworgID = 0;
            }

            //MOrg newOrg = new MOrg(GetCtx(), Convert.ToInt32(orgID), null);
            //newOrg.SetName(name + "CostCenter");
            //newOrg.SetIsSummary(true);
            //newOrg.Save();
            if (neworgID == 0)
            {
                neworgID = InsertNewOrg((name + " " + Msg.GetMsg(GetCtx(), "VA003_orgUnit")), "Y", (value + " " + "VA003_orgUnit"));
            }

            if (!IsNodeExistinTree(tree, neworgID))
            {
                MTreeNode node = new MTreeNode(tree, neworgID);
                node.Save();
                node.SetParent_ID(parent_ID);
                node.SetSeqNo(seqCounter);
                node.SetIsActive(true);
                seqCounter++;
                node.Save();
            }
            else
            {
                MTreeNode node = MTreeNode.Get(tree, neworgID);
                node.Save();
                node.SetParent_ID(parent_ID);
                node.SetIsActive(true);
                node.SetSeqNo(seqCounter);
                seqCounter++;
                node.Save();
            }
            return neworgID;
        }


        //private void insertChildren(List<TreeStructure> items, MTree tree, int parentID)
        //{

        //    DataRow[] rows = ds.Tables[0].Select("parent_org_id=" + Convert.ToInt32(dr["AD_Org_ID"]));
        //    if (rows.Count() > 0)
        //    {
        //        int PID = InsertCostCenter(tree, dr, parentID);
        //        insertChildren1(ds, rows, PID, tree);
        //    }
        //}

        //private int InsertCostCenter(MTree tree, DataRow dr, int parent_ID)
        //{
        //    MOrg newOrg = new MOrg(GetCtx(), 0, null);
        //    newOrg.SetName(dr["Name"].ToString() + "CostCenter");
        //    newOrg.SetIsSummary(true);
        //    newOrg.Save();

        //    MTreeNode node = new MTreeNode(tree, newOrg.GetAD_Org_ID());
        //    node.Save();
        //    node.SetParent_ID(parent_ID);
        //    node.SetSeqNo(seqCounter);
        //    seqCounter++;
        //    node.Save();

        //    return newOrg.GetAD_Org_ID();
        //}

        //private void insertChildren1(DataSet ds, DataRow[] rows, int ParentTreeID, MTree tree)
        //{
        //    foreach (var dr in rows)
        //    {
        //        DataRow[] newRows = ds.Tables[0].Select("parent_org_id=" + dr["AD_Org_ID"]);
        //        if (newRows.Count() > 0)
        //        {
        //            if (!IsNodeExistinTree(tree, Convert.ToInt32(dr["AD_Org_ID"])))
        //            {
        //                //   int PID = InsertNewSummryNode(dr["Name"].ToString(), tree, ParentTreeID);
        //                InsertCurrentNode(tree, dr, Convert.ToInt32(dr["AD_Org_ID"]));
        //                InsertCostCenter(tree, dr, Convert.ToInt32(dr["AD_Org_ID"]));
        //            }
        //            else
        //            {
        //                UpdateCurrentNode(ParentTreeID, tree, Convert.ToInt32(dr["AD_Org_ID"]));
        //            }
        //            insertChildren1(ds, newRows, Convert.ToInt32(dr["AD_Org_ID"]), tree);
        //        }
        //        else
        //        {
        //            if (!IsNodeExistinTree(tree, Convert.ToInt32(dr["AD_Org_ID"])))
        //            {
        //                InsertCurrentNode(tree, dr, ParentTreeID);
        //            }
        //            else
        //            {
        //                UpdateCurrentNode(ParentTreeID, tree, Convert.ToInt32(dr["AD_Org_ID"]));
        //            }
        //        }
        //    }
        //}

        //private void UpdateCurrentNode(int ParentTreeID, MTree tree, int NodeID)
        //{
        //    MTreeNode node = MTreeNode.Get(tree, NodeID);
        //    node.SetParent_ID(ParentTreeID);
        //    node.SetSeqNo(seqCounter);
        //    seqCounter++;
        //    node.Save();
        //}

        //private void InsertCurrentNode(MTree tree, DataRow dr, int ParentID)
        //{
        //    MTreeNode node = new MTreeNode(tree, Convert.ToInt32(dr["AD_Org_ID"]));
        //    node.SetParent_ID(ParentID);
        //    node.SetSeqNo(seqCounter);
        //    seqCounter++;
        //    node.Save();
        //}


        //private int InsertNewSummryNode(string Name, MTree tree, int parentID)
        //{
        //    MOrg newOrg = new MOrg(GetCtx(), 0, null);
        //    newOrg.SetName("SummaryGroupNode" + Name);
        //    newOrg.SetIsSummary(true);
        //    newOrg.Save();

        //    MTreeNode node = new MTreeNode(tree, newOrg.GetAD_Org_ID());
        //    node.SetParent_ID(parentID);
        //    node.SetSeqNo(seqCounter);
        //    seqCounter++;
        //    node.Save();

        //    return newOrg.GetAD_Org_ID();
        //}

        private int InsertHeaderNode(MTree tree)
        {
            //MOrg newOrg = new MOrg(GetCtx(), 0, null);
            //newOrg.SetName("HeaderSummaryGroupNode");
            //newOrg.SetIsSummary(true);
            //newOrg.Save();


            object val = DB.ExecuteScalar("SELECT AD_Org_ID FROM AD_Org WHERE AD_Client_ID=" + GetCtx().GetAD_Client_ID() + " AND  value='" + GetCtx().GetAD_Client_ID().ToString() + "HeaderOrgVA003_Consolidate" + "'");
            int neworgID = 0;
            if (val != null && val != DBNull.Value && val != "")
            {
                neworgID = Convert.ToInt32(val);
            }
            else
            {
                neworgID = InsertNewOrg(GetCtx().GetAD_Client_Name() + " " + Msg.GetMsg(GetCtx(), "VA003_Consolidate"), "Y", GetCtx().GetAD_Client_ID().ToString() + "HeaderOrgVA003_Consolidate");
            }



            MTreeNode node = new MTreeNode(tree, neworgID);
            node.SetParent_ID(0);
            node.SetSeqNo(seqCounter);
            seqCounter++;
            node.Save();

            return neworgID;
        }

        private int InsertNewOrg(string name, string summary, string value)
        {
            int newOrgIDD = MSequence.GetNextID(GetCtx().GetAD_Client_ID(), "AD_Org", null);
            string sql = @"INSERT
                                INTO AD_Org
                                  (
                                    AD_CLIENT_ID,
                                    AD_ORG_ID ,
                                    CREATED ,
                                    CREATEDBY ,
                                    ISACTIVE ,
                                    ISSUMMARY ,
                                    NAME ,
                                    UPDATED ,
                                    UPDATEDBY ,
                                    VALUE
                                  )
                                  VALUES (
                        " + GetCtx().GetAD_Client_ID() + "," + Convert.ToInt32(newOrgIDD) + ",sysdate," + GetCtx().GetAD_User_ID() + ",'Y','" + summary + "','" + name + "',sysdate," + GetCtx().GetAD_User_ID() + ",'" + value + "')";

            int result = DB.ExecuteQuery(sql);

            MOrg org = new MOrg(GetCtx(), newOrgIDD, null);
            MOrgInfo info = new MOrgInfo(org);
            info.Save();

            return newOrgIDD;
        }

        private void CreateReportHeirarchy(MTree newTree)
        {
            //string sql = "SELECT PA_Hierarchy_ID from PA_Hierarchy WHERE AD_Tree_Org_ID=" + newTree.GetAD_Tree_ID();
            //object id = DB.ExecuteScalar(sql);
            try
            {
                MHierarchy hie = null;

                //if (id != null && id != DBNull.Value)
                //{
                hie = new MHierarchy(GetCtx(), Convert.ToInt32(heirarchyID), null);
                //}
                //else
                //{
                //    hie = new MHierarchy(GetCtx(), 0, null);
                //}


                MClientInfo cInfo = new MClientInfo(GetCtx(), GetAD_Client_ID(), null);

                hie.SetName(newTree.GetName());
                hie.SetAD_Tree_Org_ID(newTree.GetAD_Tree_ID());
                hie.SetAD_Tree_Activity_ID(cInfo.GetAD_Tree_Activity_ID());
                hie.SetAD_Tree_BPartner_ID(cInfo.GetAD_Tree_BPartner_ID());
                hie.SetAD_Tree_Campaign_ID(cInfo.GetAD_Tree_Campaign_ID());
                hie.SetAD_Tree_Product_ID(cInfo.GetAD_Tree_Product_ID());
                hie.SetAD_Tree_Project_ID(cInfo.GetAD_Tree_Project_ID());
                hie.SetAD_Tree_SalesRegion_ID(cInfo.GetAD_Tree_SalesRegion_ID());

                string sql = "SELECT AD_Tree_ID FROM AD_Tree WHERE isActive='Y' AND AD_Client_ID=" + GetAD_Client_ID() + @" AND AD_Table_ID=
                                                                       (SELECT AD_TAble_ID FROM AD_Table WHERE TableName='C_ElementValue')";
                object accountTree = DB.ExecuteScalar(sql);
                if (accountTree == null || accountTree == DBNull.Value)
                {


                }

                hie.SetRef_Tree_Org_ID(Ref_Tree_ID);

                hie.SetAD_Tree_Account_ID(Convert.ToInt32(accountTree));

                hie.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                return;
            }
        }

        private void DeleteExtraNodes(int AD_Tree_ID)
        {
            int count = DB.ExecuteQuery(@"DELETE
                        FROM AD_TreeNode
                        WHERE AD_Tree_ID=" + AD_Tree_ID + @"
                        AND Node_ID    IN
                          (SELECT node_ID
                          FROM AD_TreeNode
                          JOIN AD_Org
                          ON AD_TreeNode.Node_ID=AD_Org.AD_Org_ID
                          WHERE AD_Tree_ID      =" + AD_Tree_ID + @"
                          AND Node_ID NOT      IN
                            (SELECT node_ID
                            FROM AD_TreeNode
                            WHERE AD_Tree_ID=" + AD_Tree_ID + @"
                            AND node_ID    IN
                              (SELECT parent_ID FROM AD_TreeNode WHERE AD_Tree_ID=" + AD_Tree_ID + @"
                              )
                            )
                          AND AD_Org.IsSummary='Y'
                          )");

            if (count > 0)
            {
                DeleteExtraNodes(AD_Tree_ID);
            }

        }

    }


    public class TreeStructure
    {

        public string text { get; set; }
        public int NodeID { get; set; }
        public int ParentID { get; set; }
        public bool IsSummary { get; set; }
        public bool IsLegal { get; set; }
        public bool expanded { get; set; }
        public string TableName { get; set; }
        public string color { get; set; }
        public string bColor { get; set; }
        public string ImageSource { get; set; }
        public int AD_Tree_ID { get; set; }
        public List<TreeStructure> items { get; set; }
        public int ParentOrgInfoID { get; set; }
        public int SeqNo { get; set; }
        public int TreeParentID { get; set; }
    }
    public class TreeHierarchy
    {
        public int NodeID { get; set; }
        public int ParentNodeID { get; set; }
        public int SeqNo { get; set; }
    }



}
