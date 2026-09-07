/********************************************************
 * Module Name    : VA003
 * Purpose        : Create Organization Structure
 * Class Used     : 
 * Chronological Development
 * Karan         2 July 2015
 ******************************************************/


using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VIS.DBase;
using VIS.Helpers;

namespace VIS.Models
{
    public class OrgStructure
    {
        Ctx ctx = null;

        private List<int> lstLegalEntities = new List<int>();
        private List<int> lstOrgUnits = new List<int>();
        //VIS_427 BugId 5473 Created object of list
        private List<int> lstNonLegalEntity = new List<int>();
        private List<int> lstSummary = new List<int>();
        private List<int> lstInsertedItems = new List<int>();
        List<TreeStructure> LstTrees = new List<TreeStructure>();
        List<TreeStructure> finalTree = new List<TreeStructure>();
        List<TreeHierarchy> lstOrgHie = new List<TreeHierarchy>();
        Dictionary<string, int> lstChangeLogColumns = new Dictionary<string, int>();
        private List<int> lstInActiveOrg = new List<int>();
        private List<int> lstInActiveOrgInTree = new List<int>();

        bool canRemoveCurrentFromList = true;// Used when there is no parent of non summary is being added to main tree. in this case if we remove nodes from list, then values in list decreses.
        private bool displayOrgUnits = false;

        public OrgStructure(Ctx ctx)
        {
            this.ctx = ctx;
        }




        private void LoadLegalEntities()
        {
            string sql = "SELECT AD_Org_ID FROM AD_ORg WHERE islegalentity='Y'";
            //VIS_427 BugId 5473 Applied role based condition to fetch only those records which has access of particular role
            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "AD_Org", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lstLegalEntities.Add(Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Org_ID"]));
                }
            }

        }
        /// <summary>
        /// 21/03/2024 BugId 5473 This Function used to fetch those Non legal entity which has Role access
        /// </summary>
        /// <returns>list of non legal entity</returns>
        /// <author>VIS_427</author>
        private void LoadNonLegalEntities()
        {
            string sql = "SELECT AD_Org_ID FROM AD_Org WHERE IsSummary='N' AND islegalentity='N' AND IsCostCenter='N' AND IsProfitCenter='N'";
            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "AD_Org", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lstNonLegalEntity.Add(Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Org_ID"]));
                }
            }

        }
        /// <summary>
        /// Load Organization Units
        /// VIS0060: 28 Sep 2022 
        /// </summary>
        /// <param name="LegalEntityIds">Legal Entity IDs for filter data</param>
        private void LoadLOrganizationUnits(string LegalEntityIds)
        {
            string sql = "SELECT AD_Org_ID FROM AD_Org WHERE " + (string.IsNullOrEmpty(LegalEntityIds) ? "  IsOrgUnit='Y' " :
                         " IsOrgUnit='Y' AND " + VAdvantage.DataBase.DBFunctionCollection.TypecastColumnAsInt("LegalEntityOrg") + " IN (" + LegalEntityIds + ")");
            //VIS_427 BugId 5473 Applied role based condition to fetch only those records which has access of particular role
            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "AD_Org", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lstOrgUnits.Add(Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Org_ID"]));
                }
            }
        }

        private void LoadInActiveOrgs()
        {
            string sql = "SELECT AD_Org_ID FROM AD_Org WHERE IsActive='N'";
            sql= MRole.GetDefault(ctx).AddAccessSQL(sql, "AD_Org", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
            //VIS_427 BugId 5473 Applied role based condition to fetch only those records which has access of particular role
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lstInActiveOrg.Add(Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Org_ID"]));
                }
            }

        }

        private void LoadInActiveOrgsInTree(int AD_Tree_ID)
        {
            DataSet ds = DB.ExecuteDataset("SELECT Node_ID FROM AD_TreeNode WHERE IsActive='N' AND AD_Tree_ID=" + AD_Tree_ID);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lstInActiveOrgInTree.Add(Convert.ToInt32(ds.Tables[0].Rows[i]["Node_ID"]));
                }
            }

        }

        private void LoadOrgStructure()
        {


            // string sql = "SELECT AD_Org_ID, parent_org_id FROM AD_OrgInfo";
            string sql = @"SELECT ad_org.AD_Org_ID,  AD_OrgInfo.parent_org_id
                        FROM AD_OrgInfo INNER JOIN AD_Org ON AD_OrgInfo.AD_Org_ID  =AD_Org.AD_Org_ID";
            if (!displayOrgUnits)
            {
                MOrg Org = new MOrg(ctx, ctx.GetAD_Org_ID(), null);
                // Applied check not to show Cost Centers in tree.
                if (Org.Get_ColumnIndex("IsOrgUnit") > -1)
                {
                    sql += " WHERE AD_Org.IsCostCenter='N' AND AD_Org.IsProfitCenter='N'";
                }
            }
            // DataSet ds = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql, "AD_OrgInfo", true, true));
            DataSet ds = DB.ExecuteDataset(sql);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lstOrgHie.Add(new TreeHierarchy() { NodeID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Org_ID"]), ParentNodeID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["Parent_Org_ID"]) });
                }
            }




        }




        private void GetChangeLogColumn()
        {
            string sql = "select UPPER(columnname) as colName,AD_Column_ID from AD_Column where ad_table_id =(SELECT AD_Table_ID FROM AD_Table WHERE TableName='AD_Org')";

            DataSet ds = DB.ExecuteDataset(sql);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lstChangeLogColumns[ds.Tables[0].Rows[i]["colName"].ToString()] = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Column_ID"].ToString());
                }
            }


        }

        private void MakeTree()
        {

            for (int i = 0; i < lstSummary.Count; i++)
            {

                List<TreeStructure> lstSumaaryTree = LstTrees.FindAll(a => a.ParentID == lstSummary[i]);

                //if (lstSummary[i] > 0 && isSummary)
                //{
                //    continue;
                //}



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


                LstTrees.Remove(tss);
                finalTree.Add(tss);
                lstInsertedItems.Add(tss.NodeID);
                CreateFinalTree(tss, lstSumaaryTree, true);
            }

            if (LstTrees.Count > 0)
            {
                canRemoveCurrentFromList = false;
                for (int i = 0; i < LstTrees.Count; i++)
                {
                    if (LstTrees[i].NodeID > 0 && LstTrees[i].ParentID == 0)// && lstInsertedItems.IndexOf(LstTrees[i].NodeID) == -1)
                    {
                        List<TreeStructure> newLstParentLess = new List<TreeStructure>();
                        newLstParentLess.Add(LstTrees[i]);
                        var pTree = LstTrees.Find(a => a.NodeID == LstTrees[i].NodeID);
                        insertCurrentnonSummaryNode(finalTree, pTree);
                        InsertNonSummary(newLstParentLess, pTree, 0);
                        //i = 0;
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

                var pTree = LstTrees.Find(a => a.NodeID == lstparentLess[j].NodeID);

                //if (lstInsertedItems.IndexOf(LstTrees[j].NodeID) > -1)
                //{
                //    continue;
                //}

                if (pTree != null)
                {
                    if (canRemoveCurrentFromList)
                    {
                        LstTrees.Remove(pTree);
                    }
                    lstInsertedItems.Add(pTree.NodeID);
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
                            List<TreeStructure> newLstParentLess = new List<TreeStructure>();
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
                                CreateFinalTree(pTree, newLstParentLess, tss.IsSummary);

                            }

                        }
                    }
                }
            }
        }

        //private void AddChildToTree( List<TreeHierarchy> newChild)
        //{
        //    List<TreeStructure> newLstParentLess = new List<TreeStructure>();
        //    if (newChild != null && newChild.Count > 0)
        //    {
        //        for (int k = 0; k < newChild.Count; k++)
        //        {
        //            TreeStructure tss = LstTrees.Find(a => a.NodeID == newChild[k].NodeID);
        //            if (tss != null)
        //            {
        //                LstTrees.Remove(tss);
        //                newLstParentLess.Add(tss);
        //                if (pTree.items == null)
        //                {
        //                    pTree.items = new List<TreeStructure>();
        //                }
        //            }
        //        }
        //        CreateFinalTree(pTree, newLstParentLess);
        //    }

        //}



        public List<TreeStructure> GetTree(int windowNo, string url, string tree_ID, bool showOrgUnits)
        {
            LoadLegalEntities();
            //VIS_427 BugId 5473 Called function to fetch non legal entity
            LoadNonLegalEntities();
            LoadLOrganizationUnits(null);
            LoadInActiveOrgs();
            displayOrgUnits = showOrgUnits;

            int orgWindowID = Convert.ToInt32(DB.ExecuteScalar("SELECT AD_Window_ID from AD_Window WHERE Name='Organization'", null, null));

            if (!(bool)MRole.GetDefault(ctx).GetWindowAccess(orgWindowID))
            {
                return null;
            }

            int orgTableID = MTable.Get_Table_ID("AD_Org");

            object AD_Tree_ID = null;
            string sql = null;
            if (String.IsNullOrEmpty(tree_ID))
            {

                sql = "SELECT AD_Tree_ID FROM AD_Tree "
              + "WHERE AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND AD_Table_ID=" + orgTableID + " AND IsActive='Y' AND IsAllNodes='Y' "
              + "ORDER BY IsDefault DESC, AD_Tree_ID";

                AD_Tree_ID = DB.ExecuteScalar(sql, null, null);
            }
            else
            {
                AD_Tree_ID = tree_ID;
            }

            if (AD_Tree_ID != null && AD_Tree_ID != DBNull.Value)
            {
                //var m = new MenuHelper(ctx);

                MTree tree = new MTree(ctx, Convert.ToInt32(AD_Tree_ID), true, true, null);

                //List<TreeHierarchy> LstTrees = new List<TreeHierarchy>();

                //TreeHierarchy th = new TreeHierarchy();
                //LstTrees.Add(th);

                TreeStructure trees = new TreeStructure();
                trees.AD_Tree_ID = Convert.ToInt32(AD_Tree_ID);
                LstTrees.Add(trees);
                string html = GetMenuTreeUI(trees, tree.GetRootNode(), url, windowNo.ToString(), tree.GetNodeTableName());
                //  m.dispose();
                //return LstTrees;
                List<TreeStructure> retFinalTree = new List<TreeStructure>();
                LoadOrgStructure();

                retFinalTree.Add(LstTrees[0]);
                //if (retFinalTree[0].items == null)
                //{
                //    retFinalTree[0].items = new List<TreeStructure>();
                //}


                sql = @"SELECT AD_OrgInfo.AD_Org_ID,
                                              AD_OrgInfo.parent_org_id 
                                            FROM AD_OrgInfo
                                            JOIN AD_Org
                                            ON AD_Org.AD_Org_ID =AD_OrgInfo.AD_Org_ID";
                if (!displayOrgUnits)
                {
                    MOrg Org = new MOrg(ctx, ctx.GetAD_Org_ID(), null);
                    // Applied check not to show Cost Centers in tree.
                    if (Org.Get_ColumnIndex("IsOrgUnit") > -1)
                    {
                        sql += " WHERE AD_Org.IsCostCenter='N' AND AD_Org.IsProfitCenter='N' ";
                    }
                }
                sql += " ORDER BY ad_org.issummary DESC , AD_OrgInfo.AD_Org_ID ";

                sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "AD_Org", true, true);

                DataSet dsOrgInfo = DB.ExecuteDataset(sql);


                if (dsOrgInfo != null && dsOrgInfo.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsOrgInfo.Tables[0].Rows.Count; i++)
                    {
                        TreeStructure ts = LstTrees.Find(a => a.NodeID == Convert.ToInt32(dsOrgInfo.Tables[0].Rows[i]["AD_Org_ID"]));

                        if (ts != null && Util.GetValueOfInt(dsOrgInfo.Tables[0].Rows[i]["parent_org_id"]) > 0)
                        {
                            ts.ParentID = Util.GetValueOfInt(dsOrgInfo.Tables[0].Rows[i]["parent_org_id"]);
                            ts.OrgParentID = Util.GetValueOfInt(dsOrgInfo.Tables[0].Rows[i]["parent_org_id"]);
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

                return retFinalTree;
            }

            return null;

        }

        private void PrepareList(TreeHierarchy tree, VTreeNode root)
        {
            tree.NodeID = root.Node_ID;
            tree.ParentNodeID = root.Parent_ID;
            tree.SeqNo = root.SeqNo;
        }

        private void CreateChildItems(TreeHierarchy tree, System.Windows.Forms.TreeNodeCollection treeNodeCollection)
        {
            foreach (var item in treeNodeCollection)
            {
                VTreeNode vt = (VTreeNode)item;
                if (vt.IsSummary)
                {
                    TreeHierarchy th = new TreeHierarchy();
                }
                else
                {


                }
            }
        }


        /// <summary>
        /// get Menu Tree html String 
        /// </summary>
        /// <param name="root">Root of tree</param>
        /// <param name="baseUrl">application url</param>
        /// <returns>html string</returns>
        private string GetMenuTreeUI(TreeStructure trees, VTreeNode root, string baseUrl, string windowNo = "", string tableName = "table")
        {
            baseUrl = baseUrl.Replace('.', ' ');
            StringBuilder sb = new StringBuilder("");
            if (windowNo != "")
            {
                //sb.Append("[{text: '" + root.SetName + "', expanded: true, 'tableName':'" + tableName + "', nodeid: " + root.Node_ID + ", items: [");
                trees.text = root.SetName;
                trees.expanded = true;
                trees.TableName = tableName;
                trees.IsSummary = true;
                trees.NodeID = root.Node_ID;
                trees.IsActive = true;
                trees.color = "rgba(var(--v-c-on-primary), 1)";
                //trees.ImageSource = "Areas/VA003/Images/orgstr-root-folder.png";
                trees.ImageSource = "fa fa-folder-open-o";
                trees.ParentID = root.Parent_ID;
                trees.TreeParentID = root.Parent_ID;
            }

            sb.Append(CreateTree(trees, root.Nodes, baseUrl, windowNo));

            return sb.ToString();
        }

        /// <summary>
        /// Create Tree 
        /// </summary>
        /// <param name="treeNodeCollection"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        private string CreateTree(TreeStructure trees, System.Windows.Forms.TreeNodeCollection treeNodeCollection, string baseUrl, string windowNo = "")
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in treeNodeCollection)
            {
                VTreeNode vt = (VTreeNode)item;
                if (!displayOrgUnits)
                {
                    MOrg Org = new MOrg(ctx, vt.Node_ID, null);
                    // Restrict showing the summary level organization unit on left tree.
                    if (Org.Get_ColumnIndex("IsOrgUnit") > 0 && Org.IsOrgUnit())
                    {
                        continue;
                    }
                }
                /*VIS_427 BugId 5473 if role don't have access of non legal entity in which
                user is logged in then it will continue the loop*/
                if (!lstNonLegalEntity.Contains(vt.Node_ID) && !vt.IsSummary && !lstOrgUnits.Contains(vt.Node_ID) && !lstLegalEntities.Contains(vt.Node_ID))
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

                    sb.Append(GetSummaryItemStart(newTrees, vt.Parent_ID, vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), windowNo));
                    sb.Append(CreateTree(newTrees, ((System.Windows.Forms.TreeNode)vt).Nodes, baseUrl, windowNo));
                }
                else
                {

                    sb.Append(GetTreeItem(trees, vt.Parent_ID, vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), vt.ImageKey, vt.GetAction(), vt.GetActionID(), baseUrl, windowNo, vt.OnBar));
                }
            }
            sb.Append(GetSummaryItemEnd());

            return sb.ToString();
        }

        /// <summary>
        /// summary node start html string 
        /// </summary>
        /// <param name="id">id of node</param>
        /// <param name="text">text display</param>
        /// <returns>html string</returns>
        private string GetSummaryItemStart(TreeStructure newTrees, int parentID, int id, string text, string windowNo = "")
        {
            var h = "";
            //h += " { text: '" + text + "', issummary: true , nodeid:" + id + ",items:[";

            newTrees.text = text;
            newTrees.IsSummary = true;
            newTrees.NodeID = id;
            newTrees.ParentID = parentID;
            newTrees.TreeParentID = parentID;
            newTrees.color = "white";
            if (lstInActiveOrg.IndexOf(id) > -1)
            {
                newTrees.IsActive = false;
                newTrees.bColor = "rgba(0, 132, 196, 0.7)";
            }
            else
            {
                newTrees.IsActive = true;
                newTrees.bColor = "#0084c4";
            }
            newTrees.ImageSource = "Areas/VA003/Images/orgstr-org.png";
            lstSummary.Add(id);

            return h;
        }

        /// <summary>
        /// summary node end html string 
        /// </summary>
        /// <returns></returns>
        private string GetSummaryItemEnd()
        {
            return "]},";
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
        private string GetTreeItem(TreeStructure newTrees, int parent_ID, int id, string text, string img, string action, int aid, string baseUrl, string windowNo = "", bool onBar = false)
        {
            if (action.Trim() == "") { action = "W"; img = "W"; }
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
            if (lstLegalEntities.IndexOf(id) > -1)
            {
                nTree.IsLegal = true;
                if (lstInActiveOrg.IndexOf(id) > -1)
                {
                    nTree.IsActive = false;
                    nTree.bColor = "#F4C993";
                }
                else
                {
                    nTree.IsActive = true;
                    nTree.bColor = "#dc8a20";
                }
                nTree.ImageSource = "Areas/VA003/Images/orgstr-legal-entity.PNG";
            }
            else if (lstOrgUnits.IndexOf(id) > -1)
            {
                nTree.IsOrgUnit = true;
                if (lstInActiveOrg.IndexOf(id) > -1)
                {
                    nTree.IsActive = false;
                    nTree.bColor = "rgba(86, 186, 109, 0.6)";
                }
                else
                {
                    nTree.IsActive = true;
                    nTree.bColor = "rgba(86, 186, 109, 1)";
                }

                nTree.ImageSource = "Areas/VA003/Images/orgstr-store.PNG";
            }
            else
            {
                nTree.IsOrgUnit = false;
                if (lstInActiveOrg.IndexOf(id) > -1)
                {
                    nTree.IsActive = false;
                    nTree.bColor = "rgba(166, 222, 255, 1)";
                }
                else
                {
                    nTree.IsActive = true;
                    nTree.bColor = "rgba(43, 174, 250, 0.78)";
                }

                nTree.ImageSource = "Areas/VA003/Images/orgstr-store.PNG";
            }
            nTree.NodeID = id;
            nTree.color = "white";

            return h;
        }


        ///// <summary>
        ///// get Menu Tree html String 
        ///// </summary>
        ///// <param name="root">Root of tree</param>
        ///// <param name="baseUrl">application url</param>
        ///// <returns>html string</returns>
        //private string GetMenuTreeUI(VTreeNode root, string baseUrl, string windowNo = "", string tableName = "table")
        //{
        //    baseUrl = baseUrl.Replace('.', ' ');
        //    StringBuilder sb = new StringBuilder("");
        //    if (windowNo != "")
        //    {
        //        //sb.Append("<ul data-tableName='" + tableName + "'>");
        //        //sb.Append("<li data-value='" + root.Node_ID + "'>").Append(GetRootItem(root.Node_ID, root.SetName, windowNo));

        //        sb.Append("[{text: '" + root.SetName + "', expanded: true, 'tableName':'" + tableName + "', nodeid: " + root.Node_ID + ", items: [");

        //    }

        //    //sb.Append("<ul>");

        //    sb.Append(CreateTree(root.Nodes, baseUrl, windowNo));

        //    //sb.Append("</ul>");

        //    //sb.Append("</li></ul>");

        //    return sb.ToString();
        //}

        ///// <summary>
        ///// Return Root item 
        ///// </summary>
        ///// <param name="id">id of node</param>
        ///// <param name="text">text to display</param>
        ///// <returns>root item hmnl string</returns>
        ////private string GetRootItem(int id, string text, string windowNo = "")
        ////{
        ////    var h = "<input type='checkbox' data-value='" + id + "'  id='" + windowNo + id + "' checked='checked' /><label for='" + windowNo + id + "'>" + text + "</label>";
        ////    if (windowNo != "")
        ////    {
        ////        h += "<span></span>";
        ////    }
        ////    return h;
        ////}


        ///// <summary>
        ///// Create Tree 
        ///// </summary>
        ///// <param name="treeNodeCollection"></param>
        ///// <param name="baseUrl"></param>
        ///// <returns></returns>
        //private string CreateTree(System.Windows.Forms.TreeNodeCollection treeNodeCollection, string baseUrl, string windowNo = "")
        //{
        //    StringBuilder sb = new StringBuilder();

        //    foreach (var item in treeNodeCollection)
        //    {
        //        VTreeNode vt = (VTreeNode)item;
        //        if (vt.IsSummary)
        //        {
        //            sb.Append(GetSummaryItemStart(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), windowNo));
        //            sb.Append(CreateTree(((System.Windows.Forms.TreeNode)vt).Nodes, baseUrl, windowNo));
        //        }
        //        else
        //        {
        //            sb.Append(GetTreeItem(vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), vt.ImageKey, vt.GetAction(), vt.GetActionID(), baseUrl, windowNo, vt.OnBar));
        //        }
        //    }
        //    sb.Append(GetSummaryItemEnd());

        //    return sb.ToString();
        //}

        ///// <summary>
        ///// summary node start html string 
        ///// </summary>
        ///// <param name="id">id of node</param>
        ///// <param name="text">text display</param>
        ///// <returns>html string</returns>
        //private string GetSummaryItemStart(int id, string text, string windowNo = "")
        //{
        //    var h = "";
        //    //if (windowNo != "")
        //    //{
        //    //    h += "<li data-value='" + id + "' data-summary='Y' class='vis-hasSubMenu'><input type='checkbox'  id='" + windowNo + id + "' /><label for='" + windowNo + id + "'>" + text + "</label>";
        //    //    h += "<span class='vis-treewindow-span'><span class='vis-css-treewindow-arrow-up'></span></span>";
        //    //    h += "<ul>";

        //    h += " { text: '" + text + "', issummary: true , nodeid:" + id + ",items:[";
        //    //}
        //    //else
        //    //{
        //    //    h += "<li  data-value='" + id + "' data-summary='Y' class='vis-hasSubMenu'> " +
        //    //         "<input type='checkbox'  id='" + windowNo + id + "' /><label data-target='#ul_" + id + "' data-toggle='collapse' for='" + windowNo + id + "'>" + text + "</label>";
        //    //    h += "<ul class='collapse'  id='ul_" + id + "'>";
        //    //}

        //    return h;
        //}

        ///// <summary>
        ///// summary node end html string 
        ///// </summary>
        ///// <returns></returns>
        //private string GetSummaryItemEnd()
        //{
        //    return "]},";
        //}

        ///// <summary>
        ///// get leaf node html string
        ///// </summary>
        ///// <param name="id">id of node</param>
        ///// <param name="text">text to display</param>
        ///// <param name="img">img to display gainst node</param>
        ///// <param name="action">action of node (window , form etc)</param>
        ///// <param name="aid">data attribute id</param>
        ///// <param name="baseUrl">app url</param>
        ///// <returns>html string </returns>
        //private string GetTreeItem(int id, string text, string img, string action, int aid, string baseUrl, string windowNo = "", bool onBar = false)
        //{
        //    if (action.Trim() == "") { action = "W"; img = "W"; }
        //    var h = "";
        //    //if (windowNo != "")
        //    //{
        //    //    h += "<li  data-value='" + id + "' data-summary='N'><img src='" + GetImageURI(img, baseUrl) + "' />" +
        //    //        "<a href='javascript:void(0)' data-value='" + id + "' data-action='" + action + "' data-actionid =" + aid + "> " + text + "</a>";
        //    //    h += "<span class='vis-treewindow-span'><span class='vis-css-treewindow-arrow-up'></span></span>";
        //    //    h += "</li>";
        //    //}
        //    //else
        //    //{
        //    //    h += "<li style='min-height: 40px;overflow: auto;' data-value='" + id + "' data-summary='N'>" +
        //    //         "<a class='vis-menuitm-with-favItm' href='javascript:void(0)'  data-value='" + id + "' data-action='" + action + "' data-actionid ='" + aid + "'>" +
        //    //         "<span " + GetSpanClass(img);
        //    //    if (_ctx.GetIsRightToLeft())
        //    //    {
        //    //        h += " Style='float:right;margin:1px 0px 0px 10px;' ";
        //    //    }

        //    //    h += " ></span>" + text + "</a>";

        //    //    if (onBar)
        //    //    {
        //    //        h += "<a data-isfavbtn='yes' data-value='" + id + "' data-isfav='yes' data-action='" + action + "' data-actionid ='" + aid + "' data-name ='" + text + "'   class='vis-menufavitm vis-favitmchecked'></a>";
        //    //    }
        //    //    else
        //    //    {
        //    //        h += "<a data-isfavbtn='yes' data-value='" + id + "' data-isfav='no' data-action='" + action + "' data-actionid ='" + aid + "'  data-name ='" + text + "' class='vis-menufavitm vis-favitmunchecked'></a>";
        //    //    }
        //    //}

        //    h += " { text: '" + text + "', issummary: false, nodeid:" + id + " },";

        //    return h;
        //}


        public string GetInitSettings()
        {
            int orgWindowID = Convert.ToInt32(DB.ExecuteScalar("SELECT AD_Window_ID from AD_Window WHERE Name='Organization'", null, null));

            if (!(bool)MRole.GetDefault(ctx).GetWindowAccess(orgWindowID))
            {
                return null;
            }

            int orgTableID = MTable.Get_Table_ID("AD_Org");

            string sql = "SELECT AD_Tree_ID FROM AD_Tree "
         + "WHERE AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND AD_Table_ID=" + orgTableID + " AND IsActive='Y' AND IsAllNodes='Y' "
         + "ORDER BY IsDefault DESC, AD_Tree_ID";

            object AD_Tree_ID = DB.ExecuteScalar(sql, null, null);

            if (AD_Tree_ID != null && AD_Tree_ID != DBNull.Value)
            {
                return AD_Tree_ID.ToString();
            }

            return null;

        }

        public object InsertOrUpdateOrg(OrgStructureData data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.SearchKey))
                {
                    data.SearchKey = MSequence.GetDocumentNo(ctx.GetAD_Client_ID(), "AD_Org", null, ctx);
                }

                int newOrgIDD = 0;
                if (data != null)
                {
                    if (data.OrgID == 0)
                    {
                        object res = DB.ExecuteScalar("SELECT Count(*) FROM AD_Org WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND upper(value)=upper('" + data.SearchKey + "')", null, null);
                        if (res != null && res != DBNull.Value && Util.GetValueOfInt(res) > 0)
                        {
                            return "VIS_OrgExist";
                        }
                    }
                    else
                    {
                        object res = DB.ExecuteScalar("SELECT AD_Org_ID FROM AD_Org WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND upper(value)=upper('" + data.SearchKey + "')", null, null);
                        if (res != null && res != DBNull.Value && Util.GetValueOfInt(res) != data.OrgID)
                        {
                            return "VIS_OrgExist";
                        }
                    }

                    MOrg org = null;// new MOrg(ctx, newOrgIDD, null);
                    int insertedCount = 0;
                    if (data.OrgID == 0)
                    {


                        newOrgIDD = InsertNewOrg(org, data.Description, data.IsSummary ? 'Y' : 'N', data.IsLegalEntity ? 'Y' : 'N', data.Name, data.SearchKey, data.IsActive ? 'Y' : 'N', data.costCenter ? 'Y' : 'N', data.profitCenter ? 'Y' : 'N', "'N'", Util.GetValueOfInt(data.ParentOrg));
                    }
                    else
                    {
                        newOrgIDD = data.OrgID;
                        org = new MOrg(ctx, newOrgIDD, null);
                        org.SetName(data.Name);
                        org.SetDescription(data.Description);
                        if (!string.IsNullOrEmpty(data.SearchKey))
                        {
                            org.SetValue(data.SearchKey);
                        }
                        org.SetIsLegalEntity(data.IsLegalEntity);
                        org.SetIsSummary(data.IsSummary);
                        org.SetIsActive(data.IsActive);
                        //cost center and profit center
                        org.SetIsCostCenter(data.costCenter);
                        org.SetIsProfitCenter(data.profitCenter);
                        if (org.Save())
                        {
                            insertedCount++;
                        }


                        string sql = @"SELECT AD_Tree.AD_Tree_ID,PA_Hierarchy.ref_tree_org_id
                                                                    FROM AD_Tree JOIN 
                                                                    PA_Hierarchy ON AD_Tree.ad_tree_id       =PA_Hierarchy.AD_Tree_Org_ID"
                                        + " WHERE AD_Tree.AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND AD_Tree.AD_Table_ID=" + MTable.Get_Table_ID("AD_Org") + " AND AD_Tree.IsActive='Y' AND AD_Tree.IsAllNodes='Y' "
                                        + "ORDER BY AD_Tree.AD_Tree_ID DESC";

                        DataSet dsorgs = DB.ExecuteDataset(sql);
                        if (dsorgs != null && dsorgs.Tables[0].Rows.Count > 0)
                        {
                            StringBuilder strTrees = new StringBuilder();
                            for (int o = 0; o < dsorgs.Tables[0].Rows.Count; o++)
                            {
                                if (dsorgs.Tables[0].Rows[o]["ref_tree_org_id"] != null && dsorgs.Tables[0].Rows[o]["ref_tree_org_id"] != DBNull.Value && Convert.ToInt32(dsorgs.Tables[0].Rows[o]["ref_tree_org_id"]) > 0)
                                {
                                    continue;
                                }


                                if (strTrees.Length > 0)
                                {
                                    strTrees.Append(",");
                                }
                                strTrees.Append(dsorgs.Tables[0].Rows[o]["AD_Tree_ID"]);
                            }
                            if (strTrees.Length > 0)
                            {
                                if (data.IsActive)
                                {
                                    DB.ExecuteQuery(" UPDATE AD_TreeNode Set isActive='Y' WHERE AD_Tree_ID IN (" + strTrees + ") AND Node_ID=" + data.OrgID, null, null);
                                }
                                else
                                {
                                    DB.ExecuteQuery(" UPDATE AD_TreeNode Set isActive='N' WHERE AD_Tree_ID IN (" + strTrees + ") AND Node_ID=" + data.OrgID, null, null);
                                }
                            }

                            //DB.ExecuteQuery(" UPDATE AD_TreeNode Set isActive='N' WHERE AD_Tree_ID IN (" + strTrees + ") AND Node_ID=" + data.OrgID, null, null);
                        }

                    }




                    //if (data.IsActive)
                    //{
                    //    DB.ExecuteQuery(" UPDATE AD_TreeNode Set isActive='Y' WHERE AD_Tree_ID IN (" + data.AD_Tree_ID + ") AND Node_ID=" + data.OrgID, null, null);
                    //}
                    //else
                    //{
                    //    DB.ExecuteQuery(" UPDATE AD_TreeNode Set isActive='N' WHERE AD_Tree_ID IN (" + data.AD_Tree_ID + ") AND Node_ID=" + data.OrgID, null, null);
                    //}

                    //}

                    if (newOrgIDD > 0)
                    {
                        if (org == null)
                        {
                            org = new MOrg(ctx, newOrgIDD, null);
                        }

                        MOrgInfo info = new MOrgInfo(ctx, newOrgIDD, null);

                        info.SetIsActive(data.IsActive);

                        if (!data.IsSummary && data.C_Location_ID != null)
                        {
                            info.SetC_Location_ID(Util.GetValueOfInt(data.C_Location_ID));
                        }
                        else if (data.IsSummary)
                        {
                            info.SetC_Location_ID(Util.GetValueOfInt(0));
                        }
                        if (data.OrgType != null && data.OrgType.Length > 0 && data.OrgType != "-1")
                        {
                            info.SetAD_OrgType_ID(Util.GetValueOfInt(data.OrgType));
                        }
                        if (data.ParentOrg != null && data.ParentOrg.Length > 0 && !data.IsSummary)
                        {
                            info.SetParent_Org_ID(Util.GetValueOfInt(data.ParentOrg));
                        }
                        if (data.TaxID != null && data.TaxID.Length > 0)
                        {
                            info.SetTaxID(Util.GetValueOfString(data.TaxID));
                        }
                        if (data.EmailAddess != null && data.EmailAddess.Length > 0)
                        {
                            info.SetEMail(Util.GetValueOfString(data.EmailAddess));
                        }
                        if (data.Phone != null && data.Phone.Length > 0)
                        {
                            info.SetPhone(Util.GetValueOfString(data.Phone));
                        }
                        if (data.Fax != null && data.Fax.Length > 0)
                        {
                            info.SetFax(Util.GetValueOfString(data.Fax));
                        }
                        if (data.OrgSupervisor != null && data.OrgSupervisor.Length > 0)
                        {
                            info.SetSupervisor_ID(Util.GetValueOfInt(data.OrgSupervisor));
                        }
                        if (data.Warehouse != null && data.Warehouse.Length > 0)
                        {
                            info.SetM_Warehouse_ID(Util.GetValueOfInt(data.Warehouse));
                        }
                        info.Save();

                    }

                    if (data.OrgID == 0)
                    {
                        MTree tree = new MTree(ctx, data.AD_Tree_ID, null);

                        MTreeNode nodes = new MTreeNode(tree, Convert.ToInt32(newOrgIDD));
                        if (!String.IsNullOrEmpty(data.ParentIDForSummary))
                        {
                            nodes.SetParent_ID(Convert.ToInt32(data.ParentIDForSummary));
                        }
                        int childCount = 0;
                        var sql = "SELECT  max(seqNo) FROM AD_TreeNode WHERE AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND AD_Tree_ID=" + data.AD_Tree_ID;
                        try
                        {
                            childCount = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        }
                        catch (Exception ex)
                        {

                        }

                        childCount = childCount + 10;
                        nodes.Save();


                        return newOrgIDD.ToString();
                    }
                    else
                    {
                        string sql = "Update AD_TreeNode Set Parent_ID=" + data.ParentIDForSummary + " WHERE AD_Tree_ID=" + data.AD_Tree_ID + " AND NODE_ID=" + newOrgIDD;
                        int i = DB.ExecuteQuery(sql, null, null);
                    }

                    if (data.IsSummary)
                    {
                        if (data.OrgID > 0)
                        {
                            DB.ExecuteQuery("Update AD_OrgInfo Set Parent_Org_ID = 0 WHERE Parent_Org_ID=" + newOrgIDD, null, null);
                        }
                    }
                    else
                    {
                        string sql = "Update AD_TreeNode Set Parent_ID=" + data.ParentIDForSummary + " WHERE AD_Tree_ID=" + data.AD_Tree_ID + " AND NODE_ID=" + newOrgIDD;
                        int i = DB.ExecuteQuery(sql, null, null);
                    }

                    return GetTree(0, "", data.AD_Tree_ID.ToString(), data.showOrgUnit);
                }
            }
            catch (Exception ex)
            {
                return "Error " + ex.Message;
            }

            return null;
        }

        /// <summary>
        /// Insert New Organization
        /// </summary>
        /// <param name="org">Organization</param>
        /// <param name="description">Description</param>
        /// <param name="summary">IsSummary Level</param>
        /// <param name="legal">Is Legal Entiry</param>
        /// <param name="name">Name</param>
        /// <param name="value">Search Key</param>
        /// <param name="active">Is Active</param>
        /// <param name="costCenter">Is Cost Center</param>
        /// <param name="profitCenter">Is Profit Center</param>
        /// <param name="orgUnit">Is Org Unit</param>
        /// <param name="parentOrg">Parent Organization</param>
        /// <returns>Organization ID</returns>
        private int InsertNewOrg(MOrg org, string description, char summary, char legal, string name, string value, char active, char costCenter, char profitCenter, string orgUnit, int parentOrg)
        {
            char isOrgUnit = 'N';
            bool insertLegalEnt = false;

            if (costCenter == 'Y' || profitCenter == 'Y')
            {
                isOrgUnit = 'Y';
            }

            // VIS0060: Work done to add new node/org as Organization Unit
            if (orgUnit == "'Y'")
            {
                isOrgUnit = 'Y';
            }

            // if insert organization unit
            if (costCenter.ToString().Equals("Y") || profitCenter.ToString().Equals("Y"))
            {
                insertLegalEnt = true;
            }

            // insert organization wit legal entity
            else if (summary.ToString().Equals("N") && legal.ToString().Equals("N"))
            {
                insertLegalEnt = true;
            }

            int newOrgIDD = MSequence.GetNextID(ctx.GetAD_Client_ID(), "AD_Org", null);
            string sql = @"INSERT
                                INTO AD_Org
                                  (
                                    AD_CLIENT_ID,
                                    AD_ORG_ID ,
                                    CREATED ,
                                    CREATEDBY ,
                                    DESCRIPTION ,
                                    ISACTIVE ,
                                    ISSUMMARY ,
                                    NAME ,
                                    UPDATED ,
                                    UPDATEDBY ,
                                    VALUE,
                                    IsLegalEntity,
                                    IsCostCenter,
                                    IsProfitCenter,
                                    IsOrgUnit,
                                    LegalEntityOrg
                                  )
                                  VALUES (
                        " + ctx.GetAD_Client_ID() + "," + Convert.ToInt32(newOrgIDD) + ",sysdate," + ctx.GetAD_User_ID() + ",'" + description + "','" + active + "','" + summary + "','" + name + "',sysdate," + ctx.GetAD_User_ID() + ",'" + value + "','" + legal + "','" + costCenter + "','" + profitCenter + "','" + isOrgUnit + "',";
            if (insertLegalEnt)
            {
                sql += parentOrg;
            }
            else
            {
                sql += "NULL";
            }
            sql += ")";
            int insertedCount = DB.ExecuteQuery(sql, null, null);

            org = new MOrg(ctx, newOrgIDD, null);


            int AD_Table_ID = MTable.Get_Table_ID("AD_Org");

            string type = X_AD_ChangeLog.CHANGELOGTYPE_Insert;

            GetChangeLogColumn();

            if (!MChangeLog.IsNotLogged(AD_Table_ID, "AD_Org", 0, type))
            {
                MRole role = MRole.GetDefault(ctx, false);
                //	Do we need to log
                if (MChangeLog.IsLogged(AD_Table_ID, type)		//	im/explicit log
                    || (role != null && role.IsChangeLog()))//	Role Logging
                {
                    CreateLog(org, "AD_CLIENT_ID", ctx.GetAD_Client_ID().ToString(), AD_Table_ID);
                    CreateLog(org, "AD_ORG_ID", newOrgIDD.ToString(), AD_Table_ID);
                    CreateLog(org, "CREATED", org.GetCreated().ToShortDateString(), AD_Table_ID);
                    CreateLog(org, "CREATEDBY", ctx.GetAD_User_ID().ToString(), AD_Table_ID);
                    CreateLog(org, "DESCRIPTION", description, AD_Table_ID);
                    CreateLog(org, "ISACTIVE", "Y", AD_Table_ID);
                    CreateLog(org, "ISSUMMARY", summary.ToString(), AD_Table_ID);
                    CreateLog(org, "NAME", name, AD_Table_ID);
                    CreateLog(org, "UPDATED", org.GetUpdated().ToShortDateString(), AD_Table_ID);
                    CreateLog(org, "UPDATEDBY", ctx.GetAD_User_ID().ToString(), AD_Table_ID);
                    CreateLog(org, "VALUE", value, AD_Table_ID);
                    CreateLog(org, "IsLegalEntity", legal.ToString(), AD_Table_ID);
                }
            }

            org.PublicAfterSave(true, true);

            return org.GetAD_Org_ID();

        }


        private void CreateLog(MOrg org, string ColumnName, string value, int AD_Table_ID)
        {
            int colID = lstChangeLogColumns[ColumnName.ToUpper()];
            MChangeLog clog = new MChangeLog(ctx, 0, null);
            MSession session = MSession.Get(ctx);
            clog.SetAD_Session_ID(session.GetAD_Session_ID());
            clog.SetAD_Table_ID(AD_Table_ID);
            clog.SetRecord_ID(org.GetAD_Org_ID());
            clog.SetOldValue(null);
            clog.SetNewValue(value);
            clog.SetAD_Column_ID(colID);
            clog.Save();
        }

        public OrgStructureData GetOrgInfo(int orgID, bool loadLookups)
        {
            OrgStructureData data = new OrgStructureData();
            if (orgID > 0)
            {
                MOrg org = new MOrg(ctx, orgID, null);
                data.Tenant = org.GetAD_Client_ID();
                data.SearchKey = org.GetValue();
                data.Name = org.GetName();
                data.Description = org.GetDescription();
                data.IsSummary = org.IsSummary();
                data.IsLegalEntity = org.IsLegalEntity();
                data.OrgID = orgID;
                data.IsActive = org.IsActive();
                data.profitCenter = org.IsProfitCenter();
                data.costCenter = org.IsCostCenter();

                MOrgInfo info = new MOrgInfo(ctx, org.GetAD_Org_ID(), null);
                data.C_Location_ID = Util.GetValueOfInt(info.GetC_Location_ID());
                data.OrgType = Util.GetValueOfString(info.GetAD_OrgType_ID());
                data.ParentOrg = Util.GetValueOfString(info.GetParent_Org_ID());
                data.TaxID = Util.GetValueOfString(info.GetTaxID());
                data.EmailAddess = Util.GetValueOfString(info.GetEMail());
                data.Phone = Util.GetValueOfString(info.GetPhone());
                data.Fax = Util.GetValueOfString(info.GetFax());
                data.OrgSupervisor = Util.GetValueOfString(info.GetSupervisor_ID());
                data.Warehouse = Util.GetValueOfString(info.GetM_Warehouse_ID());

                byte[] logo = info.GetLogo();
                //if (logo != null && logo.Length > 0)
                //{
                //    data.OrgImage = Convert.ToBase64String(logo);
                //}
                //else
                //{
                //    logo = File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "Areas\\VIS\\Images\\login\\logo.png");
                //    data.OrgImage = Convert.ToBase64String(logo);
                //}


                CheckDirectoryExist();

                if (logo != null && logo.Length > 0)
                {

                    string path = "TempDownload\\orgInfo\\ogrima" + DateTime.Now.Ticks + ".png";
                    File.WriteAllBytes(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + path, logo);
                    data.OrgImage = path;
                }
                else
                {
                    data.OrgImage = "";
                }
            }


            CheckDirectoryExist();

            string[] files = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "TempDownload\\orgInfo");
            if (files != null && files.Length > 0)
            {
                for (int r = 0; r < files.Length; r++)
                {
                    FileInfo f = new FileInfo(files[r]);
                    if (DateTime.Now - f.CreationTime > TimeSpan.FromHours(1))
                    {
                        File.Delete(files[r]);
                    }
                }
            }


            //if (loadLookups)
            //{

            //}
            return data;
        }

        private void CheckDirectoryExist()
        {
            if (!Directory.Exists(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "TempDownload\\orgInfo"))
            {
                Directory.CreateDirectory(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "TempDownload\\orgInfo");
            }
        }

        public OrgStructureLookup LoadLookups()
        {
            OrgStructureLookup data = new OrgStructureLookup();

            //string sql = "SELECT Name, AD_Client_ID FROM AD_Client WHERE IsActive='Y'";
            //DataSet ds = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql, "AD_Client", true, true));
            //data.AllTenent = new List<OrgKeyVal>();
            //data.AllTenent.Add(new OrgKeyVal { Key = -1, Name = "" });
            //if (ds != null && ds.Tables[0].Rows.Count > 0)
            //{
            //    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            //    {
            //        data.AllTenent.Add(new OrgKeyVal { Key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Client_ID"]), Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]) });
            //    }
            //}

            string sql = "SELECT AD_OrgType_ID, Name FROM ad_orgtype WHERE IsActive='Y'";
            DataSet ds = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql, "AD_OrgType", true, true));
            data.AllOrgType = new List<OrgKeyVal>();
            data.AllOrgType.Add(new OrgKeyVal { Key = -1, Name = "" });
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    data.AllOrgType.Add(new OrgKeyVal { Key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_OrgType_ID"]), Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]) });
                }
            }


            //sql = "SELECT AD_Tree_ID,Name FROM AD_Tree "
            //          + "WHERE AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND AD_Table_ID=" + MTable.Get_Table_ID("AD_Org") + " AND IsActive='Y' AND IsAllNodes='Y' "
            //          + "ORDER BY AD_Tree_ID desc";


            sql = @"SELECT AD_Tree.AD_Tree_ID,
                          AD_Tree.Name,PA_Hierarchy.ref_tree_org_id
                        FROM AD_Tree JOIN 
                        PA_Hierarchy ON AD_Tree.ad_tree_id       =PA_Hierarchy.AD_Tree_Org_ID"
                 + " WHERE AD_Tree.AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND AD_Tree.AD_Table_ID=" + MTable.Get_Table_ID("AD_Org") + " AND AD_Tree.IsActive='Y' AND AD_Tree.IsAllNodes='Y' AND PA_Hierarchy.IsActive='Y' "
                      + "ORDER BY AD_Tree.AD_Tree_ID DESC";


            ds = DB.ExecuteDataset(sql);
            data.AllReportHierarchy = new List<OrgKeyVal>();
            data.AllReportHierarchy.Add(new OrgKeyVal { Key = -1, Name = "" });
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    bool ref_tree_org_id = Util.GetValueOfInt(ds.Tables[0].Rows[i]["ref_tree_org_id"]) > 0 ? true : false;
                    data.AllReportHierarchy.Add(new OrgKeyVal { Key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Tree_ID"]), Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]), Selected = ref_tree_org_id, IsDefault = ref_tree_org_id });
                }
            }

            sql = @"SELECT fieldlength,
                              ColumnName
                            FROM AD_Column
                            WHERE ColumnName IN ('Value','Name')
                            AND AD_Table_ID   =
                              (SELECT AD_Table_ID FROM AD_Table WHERE tableName='AD_Org'
                              )";

            ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i]["ColumnName"].ToString() == "Name")
                    {
                        data.NameLength = Convert.ToInt32(ds.Tables[0].Rows[i]["fieldlength"]);
                    }
                    else
                    {
                        data.ValueLength = Convert.ToInt32(ds.Tables[0].Rows[i]["fieldlength"]);
                    }
                }
            }




            sql = @"SELECT fieldlength,
                              ColumnName
                            FROM AD_Column
                            WHERE ColumnName IN ('TaxID','Phone','EMail','Fax')
                            AND AD_Table_ID   =
                              (SELECT AD_Table_ID FROM AD_Table WHERE tableName='AD_OrgInfo'
                              )";

            ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i]["ColumnName"].ToString() == "TaxID")
                    {
                        data.TaxIDLength = Convert.ToInt32(ds.Tables[0].Rows[i]["fieldlength"]);
                    }
                    else if (ds.Tables[0].Rows[i]["ColumnName"].ToString() == "Fax")
                    {
                        data.FaxLength = Convert.ToInt32(ds.Tables[0].Rows[i]["fieldlength"]);
                    }
                    else if (ds.Tables[0].Rows[i]["ColumnName"].ToString() == "Phone")
                    {
                        data.PhoneLength = Convert.ToInt32(ds.Tables[0].Rows[i]["fieldlength"]);
                    }
                    else
                    {
                        data.EMailLength = Convert.ToInt32(ds.Tables[0].Rows[i]["fieldlength"]);
                    }
                }
            }


            sql = @"SELECT fieldlength,
                          ColumnName
                        FROM AD_Column
                        WHERE ColumnName IN ('Name')
                        AND AD_Table_ID   =
                          (SELECT AD_Table_ID FROM AD_Table WHERE tableName='AD_Tree'
                          )";


            ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i]["ColumnName"].ToString() == "Name")
                    {
                        data.TreeNameLength = Convert.ToInt32(ds.Tables[0].Rows[i]["fieldlength"]);
                    }
                }
            }

            //sql = "SELECT AD_Org_id, Name FROM AD_Org WHERE IsActive='Y'";
            //ds = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql, "AD_Org", true, true));
            //data.AllParentOrg = new List<OrgKeyVal>();
            //data.AllParentOrg.Add(new OrgKeyVal { Key = -1, Name = "" });
            //if (ds != null && ds.Tables[0].Rows.Count > 0)
            //{
            //    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            //    {
            //        data.AllParentOrg.Add(new OrgKeyVal { Key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Org_ID"]), Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]) });
            //    }
            //}

            //sql = "SELECT m_warehouse_id, Name FROM M_Warehouse WHERE IsActive='Y'";
            //ds = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql, "M_Warehouse", true, true));
            //data.AllWarehouse = new List<OrgKeyVal>();
            //data.AllWarehouse.Add(new OrgKeyVal { Key = -1, Name = "" });
            //if (ds != null && ds.Tables[0].Rows.Count > 0)
            //{
            //    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            //    {
            //        data.AllWarehouse.Add(new OrgKeyVal { Key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Warehouse_ID"]), Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]) });
            //    }
            //}

            return data;
        }

        public List<OrgKeyVal> RefreshOrgType()
        {
            List<OrgKeyVal> AllOrgType = new List<OrgKeyVal>();

            string sql = "SELECT AD_OrgType_ID, Name FROM ad_orgtype WHERE IsActive='Y'";
            DataSet ds = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql, "AD_OrgType", true, true));
            AllOrgType = new List<OrgKeyVal>();
            AllOrgType.Add(new OrgKeyVal { Key = -1, Name = "" });
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AllOrgType.Add(new OrgKeyVal { Key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_OrgType_ID"]), Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]) });
                }
            }
            return AllOrgType;
        }

        public string UploadPic(byte[] pic, int orgID)
        {
            MOrgInfo ifo = new MOrgInfo(ctx, orgID, null);
            ifo.SetLogo(pic);
            ifo.Save();

            if (pic != null && pic.Length > 0)
            {
                if (!Directory.Exists(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "TempDownload"))
                {
                    Directory.CreateDirectory(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "TempDownload");
                }
                string path = "TempDownload\\ogrima" + DateTime.Now.Ticks + ".png";
                File.WriteAllBytes(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + path, pic);
                return path;
            }
            else
            {
                return System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "Areas\\VIS\\Images\\login\\logo.png";
            }

            // return Convert.ToBase64String(pic);
        }

        public object GenerateHierarchy(string url, int windowNo)
        {
            object ad_process_id = DB.ExecuteScalar("select ad_process_id from ad_process where value = 'VA003_Generate Report Hierarchy'", null, null);

            if (ad_process_id == null || ad_process_id == DBNull.Value)
            {
                return "ProcessFailed";
            }

            MPInstance instance = new MPInstance(ctx, Convert.ToInt32(ad_process_id), 0);
            if (!instance.Save())
            {
                return "ProcessNoInstance";
            }

            VAdvantage.ProcessEngine.ProcessInfo inf = new VAdvantage.ProcessEngine.ProcessInfo("GenerateTreeNodes", Convert.ToInt32(ad_process_id), 0, 0);
            inf.SetAD_PInstance_ID(instance.GetAD_PInstance_ID());
            inf.SetAD_Client_ID(ctx.GetAD_Client_ID());


            //try
            //{

            ProcessCtl worker = new ProcessCtl(ctx, null, inf, null);
            worker.Run();
            //}
            //catch(Exception ex)
            //{
            //    return "Error "+ ex.Message;
            //}

            if (inf.IsError())
            {
                return "Error " + inf.GetSummary();
            }


            ReportHierarchy Hie = new ReportHierarchy();

            int orgTableID = MTable.Get_Table_ID("AD_Org");

            //  string sql = "SELECT AD_Tree_ID,Name FROM AD_Tree "
            //+ "WHERE AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND AD_Table_ID=" + orgTableID + " AND IsActive='Y' AND IsAllNodes='Y'  "
            //+ "ORDER BY AD_Tree_ID desc";
            //  sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "AD_Tree", true, true);
            int tree_ID = 0;




            string sql = @"SELECT AD_Tree.AD_Tree_ID,
                          AD_Tree.Name,PA_Hierarchy.ref_tree_org_id
                        FROM AD_Tree JOIN 
                        PA_Hierarchy ON AD_Tree.ad_tree_id       =PA_Hierarchy.AD_Tree_Org_ID"
                 + " WHERE AD_Tree.AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND AD_Tree.AD_Table_ID=" + MTable.Get_Table_ID("AD_Org") + " AND AD_Tree.IsActive='Y' AND AD_Tree.IsAllNodes='Y' AND PA_Hierarchy.IsActive='Y' "
                      + "ORDER BY AD_Tree.AD_Tree_ID DESC";






            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                Hie.AllReportHierarchy = new List<OrgKeyVal>();
                //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                //{
                //    OrgKeyVal key = new OrgKeyVal();
                //    key.Key = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Tree_ID"]);
                //    if (i == 0)
                //    {
                //        tree_ID = key.Key;
                //    }
                //    key.Name = Convert.ToString(ds.Tables[0].Rows[i]["Name"]);
                //    Hie.AllReportHierarchy.Add(key);
                //}

                Hie.AllReportHierarchy.Add(new OrgKeyVal { Key = -1, Name = "" });
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        tree_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Tree_ID"]);
                    }
                    bool ref_tree_org_id = Util.GetValueOfInt(ds.Tables[0].Rows[i]["ref_tree_org_id"]) > 0 ? true : false;
                    Hie.AllReportHierarchy.Add(new OrgKeyVal { Key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Tree_ID"]), Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]), Selected = ref_tree_org_id, IsDefault = ref_tree_org_id });
                }

                MTree tree = new MTree(ctx, Convert.ToInt32(tree_ID), true, true, null);

                Hie.Tree = CreateTree1(tree_ID, url, windowNo);

                return Hie;
            }
            return "";
        }

        public List<TreeStructure> CreateTree(int AD_Tree_ID, string url, int windowNo)
        {
            MTree tree = new MTree(ctx, Convert.ToInt32(AD_Tree_ID), true, true, null);
            List<TreeStructure> LstTrees = new List<TreeStructure>();
            TreeStructure trees = new TreeStructure();
            trees.AD_Tree_ID = Convert.ToInt32(AD_Tree_ID);
            LstTrees.Add(trees);
            GetMenuTreeUI(trees, tree.GetRootNode(), url, windowNo.ToString(), tree.GetNodeTableName());

            return LstTrees;
        }

        public List<TreeStructure> CreateTree1(int AD_Tree_ID, string url, int windowNo)
        {
            return CreateTree1(AD_Tree_ID, url, windowNo, null);
        }

        /// <summary>
        /// Create Tree
        /// </summary>
        /// <param name="AD_Tree_ID">Tree ID</param>
        /// <param name="url"></param>
        /// <param name="windowNo"></param>
        /// <param name="LegalEntityIds">Legal Entity IDs</param>
        /// <returns>List<TreeStructure></returns>
        public List<TreeStructure> CreateTree1(int AD_Tree_ID, string url, int windowNo, string LegalEntityIds)
        {
            LoadLegalEntities();
            //VIS_427 Called function to fetch non legal entity
            LoadNonLegalEntities();
            LoadLOrganizationUnits(LegalEntityIds);
            LoadInActiveOrgsInTree(AD_Tree_ID);
            MTree tree = new MTree(ctx, Convert.ToInt32(AD_Tree_ID), true, true, null);
            List<TreeStructure> LstTrees = new List<TreeStructure>();
            TreeStructure trees = new TreeStructure();
            trees.AD_Tree_ID = Convert.ToInt32(AD_Tree_ID);

            // VIS0060: Get Where Clause from the selected Tree.
            if (tree.Get_ColumnIndex("WhereClause") >= 0)
            {
                trees.WhereClause = Util.GetValueOfString(tree.Get_Value("WhereClause"));
            }
            LstTrees.Add(trees);
            GetMenuTreeUI1(trees, tree.GetRootNode(), url, windowNo.ToString(), tree.GetNodeTableName());

            return LstTrees;
        }

        public ReportHierarchy AddOrgNode(int treeID, string name, string description, string value, int windowNo, string url, string parentID)
        {
            ReportHierarchy rep = new ReportHierarchy();

            if (string.IsNullOrEmpty(value))
            {
                value = MSequence.GetDocumentNo(ctx.GetAD_Client_ID(), "AD_Org", null, ctx);
            }

            object res = DB.ExecuteScalar("SELECT Count(*) FROM AD_Org WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND  upper(value)=upper('" + value + "')", null, null);
            if (res != null && res != DBNull.Value && Util.GetValueOfInt(res) > 0)
            {
                rep.ErrorMsg = "VA003_OrgExist";
                return rep;
            }



            //            int newOrgIDD = MSequence.GetNextID(ctx.GetAD_Client_ID(), "AD_Org", null);

            //            string sql = @"INSERT
            //                                INTO AD_Org
            //                                  (
            //                                    AD_CLIENT_ID,
            //                                    AD_ORG_ID ,
            //                                    CREATED ,
            //                                    CREATEDBY ,
            //                                    DESCRIPTION ,
            //                                    ISACTIVE ,
            //                                    ISSUMMARY ,
            //                                    NAME ,
            //                                    UPDATED ,
            //                                    UPDATEDBY ,
            //                                    VALUE
            //                                  )
            //                                  VALUES (
            //                        " + ctx.GetAD_Client_ID() + "," + Convert.ToInt32(newOrgIDD) + ",sysdate," + ctx.GetAD_User_ID() + ",'" + description + "','Y','Y','" + name + "',sysdate," + ctx.GetAD_User_ID() + ",'" + value + "')";
            //            int insertedCount = DB.ExecuteQuery(sql, null, null);
            //            if (insertedCount <= 0)
            //            {
            //                rep.ErrorMsg = "VA003_NewOrgNotInserted";
            //                return rep;
            //            }

            //            MOrg org = new MOrg(ctx, newOrgIDD, null);

            //            MOrgInfo info = new MOrgInfo(org);
            //            //info.SetAD_Client_ID(ctx.GetAD_Client_ID());
            //            //info.SetAD_Org_ID(ctx.GetAD_Org_ID());
            //            //info.SetTaxID("0");
            //            //info.SetDUNS("0");
            //            //info.SetIsActive(true);


            //            info.Save();

            MOrg org = null;
            MTree tree = new MTree(ctx, treeID, null);

            // VIS0060: Work done to add new node/org as Organization Unit
            string orgunit = "'N'";
            string whereClause = Util.GetValueOfString(tree.Get_Value("WhereClause"));

            if (whereClause.Length > 0 && whereClause.Contains("IsOrgUnit"))
            {
                orgunit = whereClause.Substring(whereClause.IndexOf("=") + 1);
            }

            int newOrgIDD = InsertNewOrg(org, description, 'Y', 'N', name, value, 'Y', 'N', 'N', orgunit, 0);

            org = new MOrg(ctx, newOrgIDD, null);

            MOrgInfo info = new MOrgInfo(org);
            info.SetAD_Client_ID(ctx.GetAD_Client_ID());
            info.SetAD_Org_ID(ctx.GetAD_Org_ID());
            info.SetTaxID("0");
            info.SetDUNS("0");
            info.SetIsActive(true);
            info.Save();


            int childCount = 0;
            var sql = "SELECT  max(seqNo) FROM AD_TreeNode WHERE AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND AD_Tree_ID=" + treeID;
            try
            {
                childCount = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            }
            catch (Exception ex)
            {

            }

            childCount = childCount + 10;

            MTreeNode nodes = new MTreeNode(tree, Convert.ToInt32(newOrgIDD));
            if (!String.IsNullOrEmpty(parentID))
            {
                nodes.SetParent_ID(Convert.ToInt32(parentID));
            }
            nodes.SetSeqNo(childCount);
            nodes.Save();

            //rep.Tree = CreateTree(treeID, url, windowNo);

            LoadLegalEntities();
            //VIS_427 Called function to fetch non legal entity
            LoadNonLegalEntities();
            LoadLOrganizationUnits(null);
            MTree tre = new MTree(ctx, Convert.ToInt32(tree.GetAD_Tree_ID()), true, true, null);
            List<TreeStructure> LstTrees = new List<TreeStructure>();
            TreeStructure trees = new TreeStructure();
            trees.AD_Tree_ID = Convert.ToInt32(tree.GetAD_Tree_ID());

            // VIS0060: Get Where Clause from the selected Tree.
            if (tree.Get_ColumnIndex("WhereClause") >= 0)
            {
                trees.WhereClause = Util.GetValueOfString(tree.Get_Value("WhereClause"));
            }

            LstTrees.Add(trees);
            GetMenuTreeUI1(trees, tre.GetRootNode(), url, windowNo.ToString(), tre.GetNodeTableName());

            rep.Tree = LstTrees;
            return rep;

        }






        /// <summary>
        /// get Menu Tree html String 
        /// </summary>
        /// <param name="root">Root of tree</param>
        /// <param name="baseUrl">application url</param>
        /// <returns>html string</returns>
        private string GetMenuTreeUI1(TreeStructure trees, VTreeNode root, string baseUrl, string windowNo = "", string tableName = "table")
        {
            baseUrl = baseUrl.Replace('.', ' ');
            StringBuilder sb = new StringBuilder("");
            if (windowNo != "")
            {
                //sb.Append("[{text: '" + root.SetName + "', expanded: true, 'tableName':'" + tableName + "', nodeid: " + root.Node_ID + ", items: [");
                trees.text = HttpUtility.HtmlEncode(root.SetName);
                trees.expanded = true;
                trees.TableName = tableName;
                trees.NodeID = root.Node_ID;
                trees.IsSummary = true;
                trees.color = "rgba(var(--v-c-on-primary), 1)";
                trees.TreeParentID = root.Parent_ID;
                //trees.ImageSource = "Areas/VA003/Images/orgstr-root-folder.png";
                trees.ImageSource = "fa fa-folder-open-o";
                trees.Visibility = "none";
                trees.DeleteVisibility = "none";
            }

            sb.Append(CreateTree1(trees, root.Nodes, baseUrl, windowNo));

            return sb.ToString();
        }



        /// <summary>
        /// Create Tree 
        /// </summary>
        /// <param name="treeNodeCollection"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        private string CreateTree1(TreeStructure trees, System.Windows.Forms.TreeNodeCollection treeNodeCollection, string baseUrl, string windowNo = "")
        {
            StringBuilder sb = new StringBuilder();
            string orgunit = "'N'";

            if (trees.WhereClause.Length > 0 && trees.WhereClause.Contains("IsOrgUnit"))
            {
                orgunit = trees.WhereClause.Substring(trees.WhereClause.IndexOf("=") + 1);
            }

            foreach (var item in treeNodeCollection)
            {
                VTreeNode vt = (VTreeNode)item;
                if (lstInActiveOrgInTree.Contains(vt.Node_ID))
                {
                    continue;
                }
                /*VIS_427 BugId 5473 if role don't have access of non legal entity in which
                user is logged in then it will continue the loop*/
                if (!lstNonLegalEntity.Contains(vt.Node_ID) && !vt.IsSummary && !lstOrgUnits.Contains(vt.Node_ID) && !lstLegalEntities.Contains(vt.Node_ID))
                {
                    continue;
                }

                //VIS_0045: When Load Organization Unit, check it contains in the list or not
                if (orgunit.Equals("'Y'") && !vt.IsSummary && !lstOrgUnits.Contains(vt.Node_ID))
                {
                    continue;
                }

                // Applied  check on tree not to show/create org unit
                MOrg Org = new MOrg(ctx, vt.Node_ID, null);

                if (Org.Get_ColumnIndex("IsOrgUnit") > 0 && (Org.IsOrgUnit() ? "'Y'" : "'N'") != orgunit)
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
                    newTrees.WhereClause = trees.WhereClause;
                    trees.items.Add(newTrees);

                    sb.Append(GetSummaryItemStart1(newTrees, vt.Parent_ID, vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), windowNo));
                    sb.Append(CreateTree1(newTrees, ((System.Windows.Forms.TreeNode)vt).Nodes, baseUrl, windowNo));
                }
                else
                {
                    sb.Append(GetTreeItem1(trees, vt.Parent_ID, vt.Node_ID, System.Net.WebUtility.HtmlEncode(vt.SetName), vt.ImageKey, vt.GetAction(), vt.GetActionID(), baseUrl, windowNo, vt.OnBar));
                }
            }
            sb.Append(GetSummaryItemEnd());

            return sb.ToString();
        }

        /// <summary>
        /// summary node start html string 
        /// </summary>
        /// <param name="id">id of node</param>
        /// <param name="text">text display</param>
        /// <returns>html string</returns>
        private string GetSummaryItemStart1(TreeStructure newTrees, int parent_ID, int id, string text, string windowNo = "")
        {
            var h = "";
            //h += " { text: '" + text + "', issummary: true , nodeid:" + id + ",items:[";

            newTrees.text = text;
            newTrees.IsSummary = true;
            newTrees.NodeID = id;
            newTrees.TreeParentID = parent_ID;
            newTrees.color = "white";
            newTrees.bColor = "#0084c4";
            newTrees.ImageSource = "Areas/VA003/Images/orgstr-org.png";
            newTrees.Visibility = "inherit";
            newTrees.DeleteVisibility = "inherit";
            return h;
        }

        /// <summary>
        /// summary node end html string 
        /// </summary>
        /// <returns></returns>
        private string GetSummaryItemEnd1()
        {
            return "]},";
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
        private string GetTreeItem1(TreeStructure newTrees, int parent_ID, int id, string text, string img, string action, int aid, string baseUrl, string windowNo = "", bool onBar = false)
        {
            if (action.Trim() == "") { action = "W"; img = "W"; }
            var h = "";
            //h += " { text: '" + text + "', issummary: false, nodeid:" + id + " },";
            if (newTrees.items == null)
            {
                newTrees.items = new List<TreeStructure>();
            }
            TreeStructure nTree = new TreeStructure();
            newTrees.items.Add(nTree);
            nTree.text = text;
            nTree.IsSummary = false;
            if (lstLegalEntities.IndexOf(id) > -1)
            {
                nTree.IsLegal = true;
                nTree.bColor = "#dc8a20";
                nTree.ImageSource = "Areas/VA003/Images/orgstr-legal-entity.PNG";
            }
            else if (lstOrgUnits.IndexOf(id) > -1)
            {
                nTree.IsOrgUnit = true;
                nTree.bColor = "rgba(86, 186, 109, 1)";
                nTree.ImageSource = "Areas/VA003/Images/orgstr-store.PNG";
            }
            else
            {
                nTree.IsOrgUnit = false;
                nTree.bColor = "rgba(43, 174, 250, 0.78)";
                nTree.ImageSource = "Areas/VA003/Images/orgstr-store.PNG";
            }
            nTree.NodeID = id;
            nTree.TreeParentID = parent_ID;
            nTree.Visibility = "none";
            nTree.DeleteVisibility = "inherit";
            nTree.color = "white";

            return h;
        }

        public OrgStructureLookup AddNewTree(string name, bool isorgunit)
        {
            OrgStructureLookup data = new OrgStructureLookup();

            MTree tree = new MTree(ctx, 0, null);
            tree.SetName(name);
            tree.SetAD_Table_ID(MTable.Get_Table_ID("AD_Org"));
            tree.SetTreeType("OO");

            // VIS0060: Set where clause on Tree for Organization to specify if it is Tree for Org Unit or not.
            if (tree.Get_ColumnIndex("WhereClause") >= 0)
            {
                tree.Set_Value("WhereClause", isorgunit ? "AD_Org.IsOrgUnit='Y'" : "AD_Org.IsOrgUnit='N'");
            }

            if (tree.Save())
            {
                CreateReportHeirarchy(tree);


                string sql = @"SELECT AD_Tree.AD_Tree_ID,
                          AD_Tree.Name,PA_Hierarchy.AD_tree_org_id,PA_Hierarchy.ref_tree_org_ID
                        FROM AD_Tree JOIN 
                        PA_Hierarchy ON AD_Tree.ad_tree_id       =PA_Hierarchy.AD_Tree_Org_ID"
                       + " WHERE AD_Tree.AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND AD_Tree.AD_Table_ID=" + MTable.Get_Table_ID("AD_Org") + " AND AD_Tree.IsActive='Y' AND AD_Tree.IsAllNodes='Y' "
                            + "ORDER BY AD_Tree.AD_Tree_ID DESC";

                DataSet ds = DB.ExecuteDataset(sql);
                data.AllReportHierarchy = new List<OrgKeyVal>();
                data.AllReportHierarchy.Add(new OrgKeyVal { Key = -1, Name = "" });
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        bool ref_tree_org_id = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_tree_org_id"]) == tree.GetAD_Tree_ID() ? true : false;

                        if (Util.GetValueOfInt(ds.Tables[0].Rows[i]["ref_tree_org_ID"]) > 0)
                        {
                            data.AllReportHierarchy.Add(new OrgKeyVal { Key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Tree_ID"]), Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]), Selected = ref_tree_org_id, IsDefault = true });
                        }
                        else
                        {
                            data.AllReportHierarchy.Add(new OrgKeyVal { Key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Tree_ID"]), Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]), Selected = ref_tree_org_id, IsDefault = false });
                        }
                    }
                }

                return data;
            }
            else
            {
                return null;
            }

        }


        private void CreateReportHeirarchy(MTree newTree)
        {

            MClientInfo cInfo = new MClientInfo(ctx, ctx.GetAD_Client_ID(), null);

            MHierarchy hie = new MHierarchy(ctx, 0, null);
            hie.SetName(newTree.GetName());
            hie.SetAD_Tree_Org_ID(newTree.GetAD_Tree_ID());
            hie.SetAD_Tree_Activity_ID(cInfo.GetAD_Tree_Activity_ID());
            hie.SetAD_Tree_BPartner_ID(cInfo.GetAD_Tree_BPartner_ID());
            hie.SetAD_Tree_Campaign_ID(cInfo.GetAD_Tree_Campaign_ID());
            hie.SetAD_Tree_Product_ID(cInfo.GetAD_Tree_Product_ID());
            hie.SetAD_Tree_Project_ID(cInfo.GetAD_Tree_Project_ID());
            hie.SetAD_Tree_SalesRegion_ID(cInfo.GetAD_Tree_SalesRegion_ID());

            string sql = "SELECT AD_Tree_ID FROM AD_Tree WHERE isActive='Y' AND AD_Client_ID=" + ctx.GetAD_Client_ID() + @" AND AD_Table_ID=
                                                                       (SELECT AD_TAble_ID FROM AD_Table WHERE TableName='C_ElementValue')";
            object accountTree = DB.ExecuteScalar(sql, null, null);
            if (accountTree == null || accountTree == DBNull.Value)
            {
                //return "Account Tree Not Found";
            }

            hie.SetAD_Tree_Account_ID(Convert.ToInt32(accountTree));

            hie.Save();
        }
        /// <summary>
        ///This function is used to Get Sequence for Tree
        /// </summary>
        /// <param name="TreeID"> Id of the current tree</param>
        /// <returns>Seuence for the selected tree</returns>
        public int GetSequenceforTree(int TreeID)
        {
            string sql = "SELECT  MAX(seqNo) FROM AD_TreeNode WHERE AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND AD_Tree_ID=" + TreeID;
            return Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
        }
        /// <summary>
        /// This function is used to Update Orgnization logo
        /// </summary>
        /// <param name="OrgId"> Orgnization Id</param>
        /// <returns>Updated logo</returns>
        public int UpdateLogo(int OrgId)
        {
            string sql = "UPDATE AD_OrgInfo SET Logo=null WHERE AD_ORg_ID=" + OrgId;
            return DB.ExecuteQuery(sql, null, null);
        }
        /// <summary>
        /// This function is used to zoom Organization Type window
        /// </summary>
        /// <returns>Zoom window Id</returns>
        public int ZoomToWindow()
        {
            string sql = "select ad_window_id from ad_window where name ='Organization Type'";
            return Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
        }
        /// <summary>
        /// This function is used to Insert tree node in Reporting Hierarchy
        /// <param name="Chidrens"> Child Nodes of tree</param>
        /// <param name="IsActive"> Active status</param>
        /// <param name="ParentID"> Parent Node</param>
        /// <param name="TreeIds">Tree</param>
        /// </summary>
        /// <returns></returns>
        public int InsertTreeNode(string[] Chidrens, string[] IsActive, int ParentID, int TreeIds, int ChildCount)
        {
            int res = 0;
            if (Chidrens != null && Chidrens.Length > 0)
            {
                for (int i = 0; i < Chidrens.Length; i++)
                {
                    string sql = @"INSERT INTO AD_TreeNode (AD_Client_ID, AD_Org_ID, AD_Tree_ID, Created, CreatedBy, IsActive, Node_ID, Parent_ID, Updated, Updatedby,seqNo) VALUES(" +
                     ctx.GetAD_Client_ID() + "," +
                     ctx.GetAD_Org_ID() + "," +
                     TreeIds + "," +
                     "sysdate," +
                     ctx.GetAD_User_ID() + "," +
                     "'" + IsActive[i] + "'," +
                     Chidrens[i] + "," +
                     ParentID + "," +
                     "sysdate," +
                     ctx.GetAD_User_ID() + "," + (ChildCount + 10) + ") ";
                    ChildCount = ChildCount + 10;
                    try
                    {
                        res = DB.ExecuteQuery(sql, null, null);
                    }
                    catch (Exception)
                    {
                        //
                    }
                }
            }
            return res;
        }
        /// <summary>
        /// This function is used to Update Sequence of the tree
        /// <param name="OldID">Old id before change</param>
        /// <param name="NewId">Changed new id</param>
        /// <param name="TreeId">Id of the current tree</param>
        /// <param name="OldSibling">Old Siblings (Before Change)</param>
        /// <param name="NodId">Node Id</param>
        /// </summary>
        /// <returns></returns>
        public int UpdateSeuenceOfNode(int OldID, int NewId, int TreeId, string[] OldSibling, string[] NodId, string TableName)
        {
            int res = 0;
            try
            {
                if (OldSibling != null && OldSibling.Length > 0)
                {
                    for (int i = 0; i < OldSibling.Length; i++)
                    {
                        string sql = @"UPDATE " + TableName + " SET Parent_ID=" + OldID + ", SeqNo=" + i + ", Updated=SysDate" +
                                     "WHERE AD_Tree_ID=" + TreeId + " AND Node_ID=" + OldSibling[i];
                        res = DB.ExecuteQuery(sql, null, null);
                    }
                }
                if (NodId != null && NodId.Length > 0)
                {
                    for (int i = 0; i < NodId.Length; i++)
                    {
                        string sql = @"UPDATE " + TableName + " SET Parent_ID=" + NewId + ", SeqNo=" + i + ", Updated=SysDate" +
                                     " WHERE AD_Tree_ID=" + TreeId + " AND Node_ID=" + NodId[i];
                        res = DB.ExecuteQuery(sql, null, null);
                    }
                }
            }
            catch (Exception)
            {

            }
            return res;

        }
        /// <summary>
        /// This function is used to Update Parent of the tree
        /// <param name="TreeId">Id of the tree</param>
        /// <param name="CurrentNode">Current selected Node</param>
        /// </summary>
        /// <returns></returns>
        public int UpdateParentNode(int TreeId, int CurrentNode, int NewIdForOrg, bool IsSummery)
        {
            int res = 0;
            if (IsSummery)
            {
                NewIdForOrg = 0;
            }
            else
            {
                string sql = @"update AD_TreeNode set parent_id=0 WHERE AD_Tree_ID= " + TreeId + " AND  Node_ID=" + CurrentNode;
                res = DB.ExecuteQuery(sql, null, null);
            }
            string sql1 = @"update AD_OrgInfo set parent_org_id=" + NewIdForOrg + " WHERE AD_Org_ID=" + CurrentNode;
            res = DB.ExecuteQuery(sql1, null, null);
            return res;
        }
        /// <summary>
        /// This function is used to Update Parent orgnization information of the tree
        /// <param name="Name">Name of the Org</param>
        /// <param name="AD_Org_ID">Id of the Org</param>
        /// </summary>
        /// <returns></returns>
        public int UpdateOrgnization(string Name, int Ad_Org_Id)
        {
            string sql = @"UPDATE AD_Org SET name='" + Name + "' WHERE AD_Org_ID=" + Ad_Org_Id;
            return DB.ExecuteQuery(sql, null, null);
        }
        /// <summary>
        /// This function is used to Delete Node of the tree
        /// <param name="TreeId">Name of the tree</param>
        /// <param name="NodeId">Id of the tree</param>
        /// </summary>
        /// <returns></returns>
        public int DeleteAD_TreeNode(int TreeId, int Parent_ID)
        {
            string sql = @"DELETE FROM AD_TreeNode WHERE AD_Tree_ID=" + TreeId + " AND parent_ID=" + Parent_ID;
            DB.ExecuteQuery(sql, null, null);
            string sql1 = @"DELETE FROM AD_TreeNode WHERE AD_Tree_ID=" + TreeId + " AND Node_ID=" + Parent_ID;
            return DB.ExecuteQuery(sql1, null, null);
        }

    }

    public class OrgStructureData
    {
        public int OrgID { get; set; }
        public int Tenant { get; set; }
        public string SearchKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string OrgType { get; set; }
        public string ParentOrg { get; set; }
        public string TaxID { get; set; }
        public int C_Location_ID { get; set; }
        public string EmailAddess { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string OrgSupervisor { get; set; }
        public string Warehouse { get; set; }
        public bool IsSummary { get; set; }
        public bool IsLegalEntity { get; set; }
        public string OrgImage { get; set; }
        public string ParentIDForSummary { get; set; }
        public int AD_Tree_ID { get; set; }
        public bool IsActive { get; set; }
        public bool showOrgUnit { get; set; }
        public bool costCenter { get; set; }
        public bool profitCenter { get; set; }
    }

    public class OrgStructureLookup
    {
        public List<OrgKeyVal> AllTenent { get; set; }
        public List<OrgKeyVal> AllOrgType { get; set; }
        public List<OrgKeyVal> AllParentOrg { get; set; }
        public List<OrgKeyVal> AllWarehouse { get; set; }
        public List<OrgKeyVal> AllReportHierarchy { get; set; }
        public int NameLength { get; set; }
        public int ValueLength { get; set; }
        public int TaxIDLength { get; set; }
        public int PhoneLength { get; set; }
        public int EMailLength { get; set; }
        public int FaxLength { get; set; }
        public int TreeNameLength { get; set; }
    }

    public class OrgKeyVal
    {
        public string Name { get; set; }
        public int Key { get; set; }
        public bool Selected { get; set; }
        public bool IsDefault { get; set; }
    }

    public class TreeStructure
    {
        public string text { get; set; }
        public int NodeID { get; set; }
        public int ParentID { get; set; }
        public int TreeParentID { get; set; }
        public int OrgParentID { get; set; }
        public bool IsSummary { get; set; }
        public bool IsLegal { get; set; }
        public bool IsOrgUnit { get; set; }
        public bool expanded { get; set; }
        public string TableName { get; set; }
        public string color { get; set; }
        public string bColor { get; set; }
        public string ImageSource { get; set; }
        public int AD_Tree_ID { get; set; }
        public string Visibility { get; set; }
        public string DeleteVisibility { get; set; }
        public bool IsActive { get; set; }
        public string WhereClause { get; set; }
        public List<TreeStructure> items { get; set; }
    }

    public class ReportHierarchy
    {
        public List<TreeStructure> Tree { get; set; }
        public List<OrgKeyVal> AllReportHierarchy { get; set; }
        public string ErrorMsg { get; set; }
    }

    public class TreeHierarchy
    {
        public int NodeID { get; set; }
        public int ParentNodeID { get; set; }
        public int SeqNo { get; set; }
    }

}