using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Web.Helpers;
using System.Web.Hosting;
using VIS.Classes;
using VIS.DBase;
using System.Data;
using VAdvantage.Logging;
using ViennaAdvantage.Model;
using System.Text;
using VIS.DataContracts;
using VIS.Models;

namespace VA005.Models
{
    public class ProductManagementModel
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(MProductPrice).FullName);

        /// <summary>
        /// Get all Products Data
        /// </summary>
        /// <param name="searchText">Search Text</param>
        /// <param name="searchQuery">Search Query</param>
        /// <param name="pcat_ID">Product Category</param>
        /// <param name="pageNo">Page No</param>
        /// <param name="pageSize">No of records on Page</param>
        /// <param name="ctx">Context</param>
        /// <param name="Parent_ID">Parent Product</param>
        /// <returns>List of Products Data</returns>
        public List<ProductInfo> GeProdInfo(string searchText, int searchQuery, int pcat_ID, int pageNo, int pageSize, Ctx ctx, int Parent_ID)
        {
            List<ProductInfo> pInfo = new List<ProductInfo>();
            int count = 0;
            
            StringBuilder sql = new StringBuilder();
            string orderby = " ORDER BY M_Product.Value";
            sql.Append(@"SELECT DISTINCT M_Product.Name,M_Product.Value,M_Product.M_Product_ID,M_Product.IsActive,M_Product.M_AttributeSet_ID, M_Product.AD_Image_ID, M_Product.AD_Client_ID,M_Product.AD_Org_ID, M_Product.M_Product_Category_ID, M_Product_Category.Name as ProdCat, C_UOM.Name as UOM, M_Product.C_UOM_ID, M_Product.UPC FROM M_Product M_Product
                            INNER JOIN C_UOM C_UOM ON M_Product.C_UOM_ID = C_UOM.C_UOM_ID INNER JOIN M_Product_Category M_Product_Category ON M_Product.M_Product_Category_ID = 
                            M_Product_Category.M_Product_Category_ID LEFT OUTER JOIN M_Manufacturer M_Manufacturer ON M_Product.M_Product_ID = M_Manufacturer.M_Product_ID
                            LEFT OUTER JOIN M_ProductAttributes M_ProductAttributes ON M_Product.M_Product_ID = M_ProductAttributes.M_Product_ID WHERE M_Product.IsActive = 'Y' AND M_Product.IsSummary = 'N' AND M_Product.AD_Client_ID = " + ctx.GetAD_Client_ID());
            if (!String.IsNullOrEmpty(searchText))
            {
                sql.Append(" AND (UPPER(M_Product.Name) LIKE UPPER('%" + searchText + "%') OR UPPER(M_Product.UPC) LIKE UPPER('" + searchText + "')  OR  UPPER(M_Product.Value) LIKE UPPER('%" + searchText + "%')" +
                " OR UPPER(M_Manufacturer.UPC) LIKE UPPER('" + searchText + "') OR UPPER(M_ProductAttributes.UPC) LIKE UPPER('" + searchText + "'))");
            }
            if (searchQuery > 0)
            {
                string code = Util.GetValueOfString(DB.ExecuteScalar("SELECT CODE FROM AD_UserQuery WHERE AD_UserQuery_ID =" + searchQuery, null, null));
                if (!String.IsNullOrEmpty(code))
                {
                    sql.Append(" AND " + code);
                }
            }

            if (pcat_ID > 0)
            {
                sql.Append(" AND M_Product_Category.M_Product_Category_ID = " + pcat_ID);
            }
            
            if (Parent_ID > 0)
            {
                int AD_Table_ID = MTable.Get_Table_ID("M_Product");
                string sqlnew = "SELECT AD_Tree_ID FROM AD_Tree "
                + "WHERE AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND AD_Table_ID=" + AD_Table_ID + " AND IsActive='Y' AND IsAllNodes='Y' "
                            + "ORDER BY IsDefault DESC, AD_Tree_ID";

                int AD_Tree_ID = Util.GetValueOfInt(DB.ExecuteScalar(sqlnew, null, null));

                GetChildNodesID(Parent_ID, "AD_TreeNodePR", AD_Tree_ID, "M_Product");
                sql.Append(@" AND M_Product.M_Product_ID IN (SELECT M_Product.M_Product_ID  FROM AD_TreeNodePR AD_TreeNodePR  INNER JOIN AD_Tree AD_Tree  ON AD_Tree.AD_Tree_ID =AD_TreeNodePR.AD_Tree_ID 
                        INNER JOIN M_Product M_Product ON M_Product.M_Product_ID=AD_TreeNodePR.Node_ID  WHERE AD_TreeNodePR.Parent_ID IN (" + parentIDs.ToString() + ") AND AD_Tree.TreeType ='PR')");
            }

            // JID_0788: Implemented role based security
            string qry = MRole.GetDefault(ctx).AddAccessSQL(sql.ToString(), "M_Product", true, false);

            if (pageNo == 1)
            {
                string sql1 = qry.Replace(@"DISTINCT M_Product.Name,M_Product.Value,M_Product.M_Product_ID,M_Product.IsActive,M_Product.M_AttributeSet_ID, M_Product.AD_Image_ID, M_Product.AD_Client_ID,M_Product.AD_Org_ID, M_Product.M_Product_Category_ID, M_Product_Category.Name as ProdCat, C_UOM.Name as UOM, M_Product.C_UOM_ID, M_Product.UPC", "Count(*)");
                count = Util.GetValueOfInt(DB.ExecuteScalar(sql1, null, null));
            }
            DataSet ds = DB.ExecuteDatasetPaging(qry + orderby, pageNo, pageSize);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ProductInfo prodInfo = new ProductInfo();
                    prodInfo.Prodname = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    prodInfo.SearchKey = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]);
                    prodInfo.M_ProductID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]);
                    prodInfo.M_ProdCatID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_Category_ID"]);
                    prodInfo.C_UomID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]);
                    prodInfo.ProCatName = Util.GetValueOfString(ds.Tables[0].Rows[i]["ProdCat"]);
                    prodInfo.UOM = Util.GetValueOfString(ds.Tables[0].Rows[i]["UOM"]);
                    prodInfo.M_AttributeSetID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSet_ID"]);
                    prodInfo.AD_ClientID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Client_ID"]);
                    prodInfo.AD_OrgID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Org_ID"]);
                    //prodInfo.UserTableID = ProdTableID;
                    //prodInfo.UserWindowID = ProdWindowID;
                    prodInfo.productCount = count;
                    prodInfo.UPC = Util.GetValueOfString(ds.Tables[0].Rows[i]["UPC"]);
                    prodInfo.IsActive = ds.Tables[0].Rows[i]["IsActive"].ToString() == "Y" ? true : false;

                    if (ds.Tables[0].Rows[i]["AD_Image_ID"] != DBNull.Value && ds.Tables[0].Rows[i]["AD_Image_ID"] != null && Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Image_ID"]) > 0)
                    {
                        MImage mimg = new MImage(ctx, Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Image_ID"]), null);
                        var imgfll = mimg.GetThumbnailURL(46, 46);
                        prodInfo.ProdImage = imgfll;

                        if (prodInfo.ProdImage == "FileDoesn'tExist" || prodInfo.ProdImage == "NoRecordFound")
                        {
                            prodInfo.ProdImage = "";
                        }
                    }
                    else
                    {
                        prodInfo.ProdImage = "";
                    }
                    pInfo.Add(prodInfo);
                }
            }
            return pInfo;
        }

        StringBuilder parentIDs = new StringBuilder();
        private void GetChildNodesID(int currentnode, string tableName, int treeID, string adtableName)
        {
            if (parentIDs.Length == 0)
            {
                parentIDs.Append(currentnode);
            }
            else
            {
                parentIDs.Append(",").Append(currentnode);
            }


            //  string sql = "SELECT node_ID FROM " + tableName + " WHERE AD_Tree_ID=" + treeID + " AND Parent_ID = " + currentnode + " AND NODE_ID IN (SELECT " + adtableName + "_ID FROM " + adtableName + " WHERE ISActive='Y' AND IsSummary='Y')";


            string sql = "SELECT pr.node_ID FROM " + tableName + "   pr JOIN " + adtableName + " mp on pr.Node_ID=mp." + adtableName + "_id  WHERE pr.AD_Tree_ID=" + treeID + " AND pr.Parent_ID = " + currentnode + " AND mp.ISActive='Y' AND mp.IsSummary='Y'";

            DataSet ds = DB.ExecuteDataset(sql);
            if (ds == null || ds.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    GetChildNodesID(Convert.ToInt32(ds.Tables[0].Rows[j]["node_ID"]), tableName, treeID, adtableName);
                }
            }
        }

        /// <summary>
        /// Get Product Categories
        /// </summary>
        /// <param name="searchText">Search Text</param>
        /// <param name="pageNo">Page No</param>
        /// <param name="pageSize">No of records on a Page</param>
        /// <param name="ctx">Context</param>
        /// <returns>List of Product Category Data</returns>
        public List<ProdCatInfo> GetProdCats(string searchText, int pageNo, int pageSize, Ctx ctx)
        {
            int count = 0;
            List<ProdCatInfo> pInfo = new List<ProdCatInfo>();
            StringBuilder sql = new StringBuilder(@"SELECT COUNT(M_Product.M_Product_ID) AS ProdCount,M_Product_Category.M_Product_Category_ID,M_Product_Category.Name
                            FROM M_Product_Category M_Product_Category INNER JOIN M_Product M_Product ON M_Product_Category.M_Product_Category_ID = M_Product.M_Product_Category_ID 
                            WHERE M_Product_Category.IsActive = 'Y' AND M_Product.IsActive = 'Y' AND M_Product.IsSummary = 'N' AND M_Product_Category.AD_Client_ID = " + ctx.GetAD_Client_ID());

            //count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM AD_MODULEINFO WHERE PREFIX='BGT01_'", null, null));
            //if (count > 0)
            //{
            //    sql += " AND M_Product.BGT01_isstyle='N' AND M_Product_Category.BGT01_IsSold = 'N'";
            //}

            // Added by Bharat on 09 March 2018 to add search on Product categories
            if (!String.IsNullOrEmpty(searchText))
            {
                sql.Append(" AND UPPER(M_Product_Category.Name) LIKE UPPER('%" + searchText + "%')");
            }

            // JID_0788: Implemented role based security
            string qry = MRole.GetDefault(ctx).AddAccessSQL(sql.ToString(), "M_Product", true, false);

            sql.Clear();
            sql.Append(qry + " GROUP BY M_Product_Category.M_Product_Category_ID,M_Product_Category.Name Order BY M_Product_Category.Name ");

            if (pageNo == 1)
            {
                string sql1 = "SELECT COUNT(M_Product_Category_ID) FROM (" + sql + ") t";
                count = Util.GetValueOfInt(DB.ExecuteScalar(sql1, null, null));
            }

            DataSet ds = DB.ExecuteDatasetPaging(sql.ToString(), pageNo, pageSize);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ProdCatInfo prodInfo = new ProdCatInfo();
                    prodInfo.Catname = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    prodInfo.M_ProdCatID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_Category_ID"]);
                    prodInfo.ProdCount = Util.GetValueOfInt(ds.Tables[0].Rows[i]["ProdCount"]);
                    prodInfo.TotalRecords = count;
                    pInfo.Add(prodInfo);
                }
            }
            return pInfo;
        }

        public DataSet GetProductDetails(int M_Product_ID, Ctx ct)
        {
            int AD_Table_ID = MTable.Get_Table_ID("M_Product");
            var qry = "SELECT AD_Tree_ID FROM AD_Tree "
            + "WHERE AD_Client_ID=" + ct.GetAD_Client_ID() + " AND AD_Table_ID=" + AD_Table_ID + " AND IsActive='Y' AND IsAllNodes='Y' "
                        + "ORDER BY IsDefault DESC, AD_Tree_ID";
            int AD_Tree_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, null));

            string sql = @"SELECT DISTINCT img.ImageURL, p.Name,NVL(p.UPC, ' ') as UPC,NVL(a.Name,' ') AS ATTRIBUTE,u.name AS UOM,pc.Name AS ProductCategory,t.Name AS TaxCategory,
                (SELECT Name FROM AD_Ref_List WHERE AD_Reference_ID = 270 AND Value = p.ProductType) AS ProductType,CASE WHEN (tree.Parent_ID = 0) THEN tr.Name ELSE 
                NVL((SELECT Name FROM M_Product WHERE M_Product_ID = tree.Parent_ID),' ') END AS Tree 
                FROM M_Product p LEFT OUTER JOIN C_UOM u ON u.C_UOM_ID = p.C_UOM_ID LEFT OUTER JOIN M_AttributeSet a ON a.M_AttributeSet_ID = p.M_AttributeSet_ID 
                LEFT OUTER JOIN AD_Image img ON p.AD_Image_ID = img.AD_Image_ID LEFT OUTER JOIN M_Product_Category pc ON pc.M_Product_Category_ID = p.M_Product_Category_ID 
                LEFT OUTER JOIN C_TaxCategory t ON t.C_TaxCategory_ID = p.C_TaxCategory_ID LEFT OUTER JOIN AD_TreeNodePR tree ON (p.M_Product_ID = tree.Node_ID AND tree.AD_Tree_ID = " + AD_Tree_ID +
                " ) LEFT OUTER JOIN AD_Tree tr ON tr.AD_Tree_ID = " + AD_Tree_ID + " WHERE p.M_Product_ID = " + M_Product_ID;
            DataSet dsProd = DB.ExecuteDataset(sql, null, null);
            if (dsProd != null && dsProd.Tables[0] != null)
            {
                foreach (DataColumn column in dsProd.Tables[0].Columns)
                {
                    column.ColumnName = column.ColumnName.ToUpper();
                }
            }
            return dsProd;
        }

        public List<PriceInfo> GetProductPrices(List<int> M_Product_ID, int PriceList, Ctx ct)
        {
            List<PriceInfo> lst = new List<PriceInfo>();
            try
            {
                string qry = @"SELECT prd.M_Product_ID,prd.Name,pr.PriceList,pr.PriceStd,pr.PriceLimit,pr.C_UOM_ID,u.Name as UOM,
                            pr.Lot,pr.M_AttributeSetInstance_ID,ats.Description
                            FROM M_ProductPrice pr INNER JOIN C_UOM u ON (pr.C_UOM_ID= u.C_UOM_ID) 
                            INNER JOIN M_Product prd ON (pr.M_Product_ID= prd.M_Product_ID)
                            INNER JOIN M_AttributeSetInstance ats ON (pr.M_AttributeSetInstance_ID = ats.M_AttributeSetInstance_ID)
                            WHERE pr.IsActive = 'Y' AND pr.M_PriceList_Version_ID = " + PriceList;
                DataSet dsProd = DB.ExecuteDataset(MRole.GetDefault(ct).AddAccessSQL(qry, "pr", true, false), null, null);
                int Recid = 0;
                for (int i = 0; i < M_Product_ID.Count; i++)
                {
                    qry = "SELECT img.ImageUrl FROM M_Product prd LEFT OUTER JOIN AD_Image img ON prd.AD_Image_ID = img.AD_Image_ID WHERE prd.M_Product_ID = " + M_Product_ID[i];
                    var img = Util.GetValueOfString(DB.ExecuteScalar(qry, null, null));
                    if (dsProd != null && dsProd.Tables[0].Rows.Count > 0)
                    {
                        DataRow[] dr = dsProd.Tables[0].Select("M_Product_ID = " + M_Product_ID[i]);
                        if (dr.Length > 0)
                        {
                            for (int j = 0; j < dr.Length; j++)
                            {
                                Recid = Recid + 1;
                                PriceInfo pr = new PriceInfo();
                                pr.recid = Recid;
                                pr.product_ID = M_Product_ID[i];
                                pr.Product = Util.GetValueOfString(dr[j]["Name"]);
                                pr.PriceList = Util.GetValueOfDecimal(dr[j]["PriceList"]);
                                pr.PriceStd = Util.GetValueOfDecimal(dr[j]["PriceStd"]);
                                pr.PriceLimit = Util.GetValueOfDecimal(dr[j]["PriceLimit"]);
                                pr.C_Uom_ID = Util.GetValueOfInt(dr[j]["C_UOM_ID"]);
                                pr.UOM = Util.GetValueOfString(dr[j]["UOM"]);
                                pr.Lot = Util.GetValueOfString(dr[j]["Lot"]);
                                pr.attribute_ID = Util.GetValueOfInt(dr[j]["M_AttributeSetInstance_ID"]);
                                pr.Attribute = Util.GetValueOfString(dr[j]["Description"]);
                                pr.ImageUrl = img;
                                lst.Add(pr);
                            }
                        }
                        else
                        {
                            IDataReader idr = DB.ExecuteReader("SELECT Name, C_UOM_ID FROM M_Product WHERE M_Product_ID = " + M_Product_ID[i], null, null);
                            string prodName = "";
                            int uom = 0;
                            while (idr.Read())
                            {
                                prodName = idr.GetString(0);
                                uom = idr.GetInt32(1);
                            }
                            if (idr != null)
                            {
                                idr.Close();
                            }
                            Recid = Recid + 1;
                            PriceInfo pr = new PriceInfo();
                            pr.recid = Recid;
                            pr.product_ID = M_Product_ID[i];
                            pr.Product = prodName;
                            pr.PriceList = 0;
                            pr.PriceStd = 0;
                            pr.PriceLimit = 0;
                            pr.C_Uom_ID = uom;
                            pr.UOM = "";
                            pr.Lot = "";
                            pr.attribute_ID = 0;
                            pr.Attribute = "";
                            pr.ImageUrl = img;
                            lst.Add(pr);
                        }
                    }
                    else
                    {
                        IDataReader dr = DB.ExecuteReader("SELECT Name, C_UOM_ID FROM M_Product WHERE M_Product_ID = " + M_Product_ID[i], null, null);
                        string prodName = "";
                        int uom = 0;
                        while (dr.Read())
                        {
                            prodName = dr.GetString(0);
                            uom = dr.GetInt32(1);
                        }
                        if (dr != null)
                        {
                            dr.Close();
                        }
                        Recid = Recid + 1;
                        PriceInfo pr = new PriceInfo();
                        pr.recid = Recid;
                        pr.product_ID = M_Product_ID[i];
                        pr.Product = prodName;
                        pr.PriceList = 0;
                        pr.PriceStd = 0;
                        pr.PriceLimit = 0;
                        pr.C_Uom_ID = uom;
                        pr.UOM = "";
                        pr.Lot = "";
                        pr.attribute_ID = 0;
                        pr.Attribute = "";
                        pr.ImageUrl = img;
                        lst.Add(pr);
                    }
                }
            }
            catch
            {

            }
            return lst;
        }

        public List<PriceInfo> GetSupplierData(List<int> M_Product_ID, int Supplier, Ctx ct)
        {
            List<PriceInfo> lst = new List<PriceInfo>();
            try
            {
                string qry = "SELECT act.C_Currency_ID FROM AD_ClientInfo tnt INNER JOIN C_AcctSchema act ON tnt.C_AcctSchema1_ID = act.C_AcctSchema_ID WHERE tnt.AD_Client_ID = " + ct.GetAD_Client_ID();
                int currency = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, null));
                qry = "SELECT prd.M_Product_ID,prd.Name, po.PriceList, po.Order_Min, po.Order_Pack, po.C_UOM_ID, u.Name AS UOM, po.C_Currency_ID, po.DeliveryTime_Promised" +
                        " FROM M_Product_PO po INNER JOIN C_UOM u ON(po.C_UOM_ID= u.C_UOM_ID) INNER JOIN C_Currency c ON(po.C_Currency_ID = c.C_Currency_ID) INNER JOIN M_Product prd" +
                        " ON (po.M_Product_ID= prd.M_Product_ID) WHERE po.IsActive = 'Y' AND po.C_BPartner_ID = " + Supplier;
                DataSet dsProd = DB.ExecuteDataset(qry, null, null);
                int Recid = 0;
                for (int i = 0; i < M_Product_ID.Count; i++)
                {
                    if (dsProd != null && dsProd.Tables[0].Rows.Count > 0)
                    {
                        DataRow[] dr = dsProd.Tables[0].Select("M_Product_ID = " + M_Product_ID[i]);
                        if (dr.Length > 0)
                        {
                            for (int j = 0; j < dr.Length; j++)
                            {
                                Recid = Recid + 1;
                                PriceInfo pr = new PriceInfo();
                                pr.recid = Recid;
                                pr.product_ID = M_Product_ID[i];
                                pr.Product = Util.GetValueOfString(dr[j]["Name"]);
                                pr.PriceList = Util.GetValueOfDecimal(dr[j]["PriceList"]);
                                pr.OrderMin = Util.GetValueOfDecimal(dr[j]["Order_Min"]);
                                pr.OrderPack = Util.GetValueOfDecimal(dr[j]["Order_Pack"]);
                                pr.C_Uom_ID = Util.GetValueOfInt(dr[j]["C_UOM_ID"]);
                                pr.C_Currency_ID = Util.GetValueOfInt(dr[j]["C_Currency_ID"]);
                                pr.DeliveryTime = Util.GetValueOfInt(dr[j]["DeliveryTime_Promised"]);
                                lst.Add(pr);
                            }
                        }
                        else
                        {
                            IDataReader idr = DB.ExecuteReader("SELECT Name, C_UOM_ID FROM M_Product WHERE M_Product_ID = " + M_Product_ID[i], null, null);
                            string prodName = "";
                            int uom = 0;
                            while (idr.Read())
                            {
                                prodName = idr.GetString(0);
                                uom = idr.GetInt32(1);
                            }
                            if (idr != null)
                            {
                                idr.Close();
                            }
                            Recid = Recid + 1;
                            PriceInfo pr = new PriceInfo();
                            pr.recid = Recid;
                            pr.product_ID = M_Product_ID[i];
                            pr.Product = prodName;
                            pr.PriceList = 0;
                            pr.OrderMin = 0;
                            pr.OrderPack = 0;
                            pr.C_Uom_ID = uom;
                            pr.C_Currency_ID = currency;
                            pr.DeliveryTime = 0;
                            lst.Add(pr);
                        }
                    }
                    else
                    {
                        IDataReader dr = DB.ExecuteReader("SELECT Name, C_UOM_ID FROM M_Product WHERE M_Product_ID = " + M_Product_ID[i], null, null);
                        string prodName = "";
                        int uom = 0;
                        while (dr.Read())
                        {
                            prodName = dr.GetString(0);
                            uom = dr.GetInt32(1);
                        }
                        if (dr != null)
                        {
                            dr.Close();
                        }
                        Recid = Recid + 1;
                        PriceInfo pr = new PriceInfo();
                        pr.recid = Recid;
                        pr.product_ID = M_Product_ID[i];
                        pr.Product = prodName;
                        pr.PriceList = 0;
                        pr.OrderMin = 0;
                        pr.OrderPack = 0;
                        pr.C_Uom_ID = uom;
                        pr.C_Currency_ID = currency;
                        pr.DeliveryTime = 0;
                        lst.Add(pr);
                    }
                }
            }
            catch(Exception e)
            {

            }
            return lst;
        }

        public string SetRelatedProduct(int id, int relProd_ID, Ctx ctx)
        {
            MProduct prd = new MProduct(ctx, relProd_ID, null);
            X_M_RelatedProduct relProd = new X_M_RelatedProduct(ctx, 0, null);
            relProd.SetAD_Org_ID(ctx.GetAD_Org_ID());
            relProd.SetAD_Client_ID(ctx.GetAD_Client_ID());
            relProd.SetM_Product_ID(id);
            relProd.SetRelatedProduct_ID(relProd_ID);
            relProd.SetName(prd.GetName());
            relProd.SetRelatedProductType("A");
            if (!relProd.Save())
            {
                ValueNamePair pp = VLogger.RetrieveError();
                return pp != null ? pp.GetValue() + " " + pp.ToString() : "";
            }
            else
            {
                MProduct pd = new MProduct(ctx, id, null);
                X_M_RelatedProduct Prod = new X_M_RelatedProduct(ctx, 0, null);
                Prod.SetAD_Org_ID(ctx.GetAD_Org_ID());
                Prod.SetAD_Client_ID(ctx.GetAD_Client_ID());
                Prod.SetM_Product_ID(relProd_ID);
                Prod.SetRelatedProduct_ID(id);
                Prod.SetName(pd.GetName());
                Prod.SetRelatedProductType("A");
                if (!Prod.Save())
                {

                }
            }
            return "";
        }

        public string UpdateRelatedProduct(int id, List<RelatedInfo> relate, Ctx ctx)
        {
            IDataReader idr = null;
            DataTable dt = null;
            string error = "";
            MProduct prd = new MProduct(ctx, id, null);
            for (int i = 0; i < relate.Count; i++)
            {
                string sql = "SELECT * FROM M_RelatedProduct WHERE M_Product_ID = " + id + " AND RelatedProduct_ID = " + relate[i].M_Product_ID;
                idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        X_M_RelatedProduct relProd = new X_M_RelatedProduct(ctx, dr, null);
                        relProd.SetName(relate[i].Product);
                        relProd.SetRelatedProductType(relate[i].RelatedType);
                        if (!relProd.Save())
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            error += pp.GetName() + pp != null ? " - " + pp.GetValue() + " " + pp.ToString() : "" + "\n";
                        }
                    }
                }
            }
            return error;
        }

        public string deleteRelatedProduct(Int32 id, List<RelatedInfo> relate, Ctx ctx)
        {
            string error = "";
            string qry = "";
            int no = 0;
            MProduct pro = null;
            for (int i = 0; i < relate.Count; i++)
            {
                pro = new MProduct(ctx, relate[i].M_Product_ID, null);
                qry = "DELETE FROM M_RelatedProduct WHERE RelatedProduct_ID = " + relate[i].M_Product_ID + " AND M_Product_ID = " + id;
                no = DB.ExecuteQuery(qry, null, null);
                if (no <= 0)
                {
                    error += Msg.GetMsg(ctx, "DeleteError") + pro.GetName() + "\n";
                }
            }
            return error;
        }

        public string UpdatePrice(Int32 id, List<PriceInfo> price, Ctx ctx)
        {
            string error = "";
            if (price != null)
            {
                for (int j = 0; j < price.Count; j++)
                {
                    MProductPrice mPrice = null;
                    MProduct pcat = new MProduct(ctx, Util.GetValueOfInt(price[j].product_ID), null);

                    string sql = "SELECT * FROM M_ProductPrice WHERE M_PriceList_Version_ID=" + id + " AND M_Product_ID=" + price[j].product_ID;
                    Tuple<string, string, string> minfo = null;
                    if (Env.HasModulePrefix("VAPRC_", out minfo))
                    {
                        sql += " AND M_AttributeSetInstance_ID = " + price[j].attribute_ID;
                    }
                    Tuple<string, string, string> ainfo = null;
                    if (Env.HasModulePrefix("ED011_", out ainfo))
                    {
                        sql += " AND C_UOM_ID = " + price[j].C_Uom_ID;
                    }
                    DataSet ds = new DataSet();
                    try
                    {
                        ds = DB.ExecuteDataset(sql, null, null);
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DataRow dr = ds.Tables[0].Rows[i];
                            mPrice = new MProductPrice(ctx, dr, null);
                        }
                        ds = null;
                    }
                    catch (Exception e)
                    {
                        _log.Log(Level.SEVERE, sql, e);
                    }
                    if (mPrice == null)
                    {
                        mPrice = new MProductPrice(ctx, id, Util.GetValueOfInt(price[j].product_ID), null);
                    }
                    mPrice.SetPriceList(price[j].PriceList);
                    mPrice.SetPriceStd(price[j].PriceStd);
                    mPrice.SetPriceLimit(price[j].PriceLimit);
                    mPrice.SetLot(price[j].Lot);
                    if (Env.HasModulePrefix("ED011_", out ainfo))
                    {
                        mPrice.SetC_UOM_ID(price[j].C_Uom_ID);
                    }
                    if (Env.HasModulePrefix("VAPRC_", out minfo))
                    {
                        mPrice.SetM_AttributeSetInstance_ID(price[j].attribute_ID);
                    }
                    if (!mPrice.Save())
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        error += pcat.GetName() + (pp != null ? " - " + pp.GetValue() + " " + pp.ToString() : "") + "\n";
                    }
                }
            }
            return error;
        }

        public string UpdateProduct(List<Int32> product, List<columnInfo> columnName, Ctx ctx)
        {
            string error = "";
            for (int i = 0; i < product.Count; i++)
            {
                MProduct pcat = new MProduct(ctx, Util.GetValueOfInt(product[i]), null);
                for (int item = 0; item < columnName.Count; item++)
                {
                    if (columnName[item].KEYVALUE == "IsActive")
                    {
                        pcat.Set_Value(columnName[item].KEYVALUE, columnName[item].VALUE1VALUE == "Y" ? true : false);
                    }
                    else
                    {
                        pcat.Set_Value(columnName[item].KEYVALUE, columnName[item].VALUE1VALUE);
                    }
                }
                if (!pcat.Save())
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    error += pcat.GetName() + (pp != null ? " - " + pp.GetValue() + " " + pp.ToString() : "") + "\n";
                }
            }
            return error;
        }

        public string UpdateSupplier(int cb_ID, List<PriceInfo> columnName, Ctx ctx)
        {
            IDataReader idr = null;
            DataTable dt = null;
            string error = "";
            for (int i = 0; i < columnName.Count; i++)
            {
                MProduct pro = new MProduct(ctx, columnName[i].product_ID, null);
                string qry = "SELECT * FROM M_Product_PO WHERE M_Product_ID = " + columnName[i].product_ID + " AND C_BPartner_ID = " + cb_ID;
                idr = DB.ExecuteReader(qry, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        MProductPO po = new MProductPO(ctx, dr, null);
                        for (int item = 0; item < columnName.Count; item++)
                        {
                            po.SetOrder_Min(columnName[i].OrderMin);
                            po.SetOrder_Pack(columnName[i].OrderPack);
                            po.SetC_UOM_ID(columnName[i].C_Uom_ID);
                            po.SetC_Currency_ID(columnName[i].C_Currency_ID);
                            po.SetPriceList(columnName[i].PriceList);
                            po.SetIsCurrentVendor(true);
                            po.SetDeliveryTime_Promised(columnName[i].DeliveryTime);
                            if (!po.Save())
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                error += pro.GetName() + (pp != null ? " - " + Msg.GetMsg(ctx, pp.GetValue()) + " " + pp.ToString() : "") + "\n";
                            }
                            else
                            {
                                qry = "UPDATE M_Product_PO SET IsCurrentVendor = 'N' WHERE M_Product_ID = " + columnName[i].product_ID + " AND C_BPartner_ID != " + cb_ID;
                                DB.ExecuteQuery(qry, null, null);
                            }
                        }
                    }
                }
                else
                {
                    MProductPO po = new MProductPO(ctx, 0, null);
                    po.SetC_BPartner_ID(cb_ID);
                    po.SetM_Product_ID(columnName[i].product_ID);
                    po.SetVendorProductNo(pro.GetValue());
                    for (int item = 0; item < columnName.Count; item++)
                    {
                        po.SetOrder_Min(columnName[i].OrderMin);
                        po.SetOrder_Pack(columnName[i].OrderPack);
                        po.SetC_UOM_ID(columnName[i].C_Uom_ID);
                        po.SetC_Currency_ID(columnName[i].C_Currency_ID);
                        po.SetPriceList(columnName[i].PriceList);
                        po.SetIsCurrentVendor(true);
                        po.SetDeliveryTime_Promised(columnName[i].DeliveryTime);
                        if (!po.Save())
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            error += pro.GetName() + (pp != null ? " - " + Msg.GetMsg(ctx, pp.GetValue()) + " " + pp.ToString() : "") + "\n";


                        }
                        else
                        {
                            qry = "UPDATE M_Product_PO SET IsCurrentVendor = 'N' WHERE M_Product_ID = " + columnName[i].product_ID + " AND C_BPartner_ID != " + cb_ID;
                            DB.ExecuteQuery(qry, null, null);
                        }
                    }
                }
            }
            return error;
        }

        public ProdInfo save(Int32 id, Int32 orgid, String Name, String Value, String Producttype, Int32 attrSet, Int32 taxcat, Int32 prodcat, Int32 uom, String upc, int parent_ID, Ctx ctx)
        {
            ProdInfo PInfo = new ProdInfo();
            MProduct pcat = new MProduct(ctx, id, null);
            pcat.SetAD_Org_ID(orgid);
            pcat.SetName(Name);
            pcat.SetValue(Value);
            pcat.SetProductType(Producttype);
            pcat.SetM_AttributeSet_ID(attrSet);
            pcat.SetC_TaxCategory_ID(taxcat);
            pcat.SetM_Product_Category_ID(prodcat);
            pcat.SetC_UOM_ID(uom);
            pcat.SetUPC(upc);
            if (!pcat.Save())
            {
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null)
                {
                    PInfo.error = pp.GetValue() + " " + pp.ToString();
                }
            }
            else
            {
                PInfo.error = "";
                PInfo.M_Product_ID = pcat.Get_ID();
                string sql = "UPDATE AD_TreeNodePR SET Parent_ID = " + parent_ID + " WHERE Node_ID = " + pcat.Get_ID();
                DB.ExecuteQuery(sql, null, null);
            }
            return PInfo;
        }

        public String SaveConversion(Int32 id, Int32 convid, Decimal mul, Decimal div, Int32 uom, Int32 uomTo, String upc, Ctx ctx)
        {
            MUOMConversion uomConv = new MUOMConversion(ctx, convid, null);
            uomConv.SetM_Product_ID(id);
            uomConv.SetDivideRate(mul);
            uomConv.SetMultiplyRate(div);
            uomConv.SetC_UOM_ID(uom);
            uomConv.SetC_UOM_To_ID(uomTo);
            uomConv.SetUPC(upc);
            if (!uomConv.Save())
            {
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null)
                {
                    return pp.GetValue() + " " + pp.ToString();
                }
            }
            return "";
        }

        public string saveInventoryCount(string columnName, Ctx ctx)
        {
            string error = "";
            MVAICNTInventoryCount cnt = new MVAICNTInventoryCount(ctx, 0, null);
            cnt.SetVAICNT_TransactionType("OT");
            cnt.SetVAICNT_ScanName(columnName);
            if (!cnt.Save())
            {
                //ValueNamePair pp = VLogger.RetrieveError();
                //error = pp.ToString();
            }
            else
            {
                error = cnt.Get_ID().ToString();
            }

            return error;
        }

        public string saveInventory(List<Int32> product, int count_id, List<PriceInfo> columnName, Ctx ctx)
        {
            string error = "";
            string qry = "";
            int lineID = 0;
            string upc = "";
            MProduct pro = null;
            for (int i = 0; i < columnName.Count; i++)
            {
                if (String.IsNullOrEmpty(columnName[i].UPC))
                {
                    upc = " ";
                }
                else
                {
                    upc = Util.GetValueOfString(columnName[i].UPC);
                }
                qry = "SELECT VAICNT_InventoryCountLine_ID FROM VAICNT_InventoryCountLine WHERE M_Product_ID = " + columnName[i].product_ID + " AND VAICNT_InventoryCount_ID=" + count_id +
                    " AND NVL(C_UOM_ID,0) = " + columnName[i].C_Uom_ID + " AND NVL(M_AttributeSetInstance_ID,0) = " + columnName[i].attribute_ID + " AND nvl(UPC,' ') ='" + upc + "'";
                lineID = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, null));
                MVAICNTInventoryCountLine iline = new MVAICNTInventoryCountLine(ctx, lineID, null);
                pro = new MProduct(ctx, columnName[i].product_ID, null);
                if (lineID > 0)
                {
                    iline.SetC_UOM_ID(columnName[i].C_Uom_ID);
                    iline.SetM_AttributeSetInstance_ID(columnName[i].attribute_ID);
                    iline.SetUPC(columnName[i].UPC);
                    iline.SetVAICNT_Quantity(iline.GetVAICNT_Quantity() + columnName[i].Qty);
                }
                else
                {
                    iline.SetVAICNT_InventoryCount_ID(count_id);
                    iline.SetM_Product_ID(columnName[i].product_ID);
                    iline.SetC_UOM_ID(columnName[i].C_Uom_ID);
                    iline.SetM_AttributeSetInstance_ID(columnName[i].attribute_ID);
                    iline.SetUPC(columnName[i].UPC);
                    iline.SetVAICNT_Quantity(columnName[i].Qty);
                }
                if (!iline.Save())
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    error += pro.GetName() + (pp != null ? " - " + pp.GetValue() + " " + pp.ToString() : "") + "\n";
                }
            }
            return error;
        }

        public String updateVarient(Int32 id, String upc, Ctx ctx)
        {
            MProductAttributes patr = new MProductAttributes(ctx, id, null);
            patr.SetUPC(upc);
            if (!patr.Save())
            {
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null)
                    return pp.GetValue() + " " + pp.ToString();
            }
            return "";
        }

        public string updateInventory(List<PriceInfo> columnName, Ctx ctx)
        {
            string error = "";
            int lineID = 0;
            MProduct pro = null;
            for (int i = 0; i < columnName.Count; i++)
            {
                if (columnName[i].Qty <= 0)
                {
                    continue;
                }
                lineID = columnName[i].LineID;
                pro = new MProduct(ctx, columnName[i].product_ID, null);
                MVAICNTInventoryCountLine iline = new MVAICNTInventoryCountLine(ctx, lineID, null);
                iline.SetC_UOM_ID(columnName[i].C_Uom_ID);
                iline.SetM_AttributeSetInstance_ID(columnName[i].attribute_ID);
                iline.SetUPC(columnName[i].UPC);
                iline.SetVAICNT_Quantity(columnName[i].Qty);
                if (!iline.Save())
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    error += pro.GetName() + (pp != null ? " - " + pp.GetValue() + " " + pp.ToString() : "") + "\n";
                }
            }
            return error;
        }

        public string deleteInventory(List<PriceInfo> columnName, Ctx ctx)
        {
            string error = "";
            string qry = "";
            int lineID = 0;
            int no = 0;
            MProduct pro = null;
            for (int i = 0; i < columnName.Count; i++)
            {
                pro = new MProduct(ctx, columnName[i].product_ID, null);
                lineID = columnName[i].LineID;
                qry = "DELETE FROM VAICNT_InventoryCountLine WHERE VAICNT_InventoryCountLine_ID = " + lineID;
                no = DB.ExecuteQuery(qry, null, null);
                if (no <= 0)
                {
                    error += Msg.GetMsg(ctx, "DeleteError") + pro.GetName() + "\n";
                }
            }
            return error;
        }

        public string saveAttribute(int product, int attributeset, List<Int32> attributeid, Dictionary<string, string> attrvalueid, Ctx ctx)
        {
            string error = "";
            string qry = "";
            int lineID = 0;
            string upc = "";
            int Key = 0, Value = 0;
            MProduct pro = null;
            PAttributesModel obj = new PAttributesModel();
            Dictionary<int, List<int>> insert = null;
            List<List<Int32>> values = null;
            MAttributeSet aset = new MAttributeSet(ctx, attributeset, null);
            MAttribute[] attributes = aset.GetMAttributes(true);

            //List<AttributeValues> attrValues = attrvalueid.OrderBy(o => o.pid).ToList();
            insert = new Dictionary<int, List<int>>();
            for (int i = 0; i < attributeid.Count; i++)
            {
                if (attrvalueid.ContainsKey(Util.GetValueOfString(attributeid[i])))
                {
                    foreach (String key in attrvalueid.Keys)
                    {
                        if (key.Equals(Util.GetValueOfString(attributeid[i])))
                        {
                            string attrval = attrvalueid[key];
                            List<int> listVal = new List<int>();
                            string[] splitval = attrval.Split(',');
                            foreach (string a in splitval)
                            {
                                listVal.Add(Util.GetValueOfInt(a));
                            }
                            insert.Add(Util.GetValueOfInt(key), listVal);
                        }
                    }
                }
                else
                {
                    List<Int32> blank = new List<Int32>();
                    blank.Add(0);
                    insert.Add(Util.GetValueOfInt(attributeid[i]), blank);
                }
            }

            for (int j = 0; j < attributeid.Count; j++)
            {
                values = CreateComb(insert[attributeid[j]], values);
            }

            if (values.Count > 0)
            {
                AttributeInstance value = null;
                for (int vl = 0; vl < values.Count; vl++)
                {
                    List<KeyNamePair> lst = new List<KeyNamePair>();
                    foreach (int item in values[vl])
                    {
                        MAttributeValue atVal = new MAttributeValue(ctx, Util.GetValueOfInt(item), null);
                        KeyNamePair knp = new KeyNamePair();
                        knp.Key = item;
                        knp.Name = atVal.GetName();
                        lst.Add(knp);
                    }
                    value = obj.SaveAttribute(0, "", "", System.DateTime.Now.ToShortDateString(), "", false, 0, product, 0, "", false, lst, ctx);
                    error = value.Error;
                }
            }
            return error;
        }

        private List<List<int>> CreateComb(List<Int32> atr, List<List<Int32>> vals)
        {
            List<List<Int32>> temp = new List<List<int>>();
            for (int i = 0; i < atr.Count; i++)
            {
                if (vals != null)
                {

                    for (int j = 0; j < vals.Count; j++)
                    {
                        List<Int32> temp2 = new List<int>();
                        var row = vals[j];
                        for (int k = 0; k < row.Count; k++)
                        {
                            temp2.Add(row[k]);
                        }
                        temp2.Add(atr[i]);
                        temp.Add(temp2);
                    }
                }
                else
                {
                    List<int> l = new List<int>();
                    l.Add(atr[i]);
                    temp.Add(l);
                }
            }
            return temp;
        }

        /// <summary>
        /// USed to Create Tree. This is main function which intialize tree creation.
        /// </summary>
        public object CreateTree(int attributeSet_id, Ctx _ctx)
        {
            List<VA005_TreeStructure> final = null;
            MAttributeSet aset = new MAttributeSet(_ctx, attributeSet_id, null);
            if (aset.IsLot() || aset.IsSerNo() || aset.IsGuaranteeDate())
            {
                return null;
            }

            string sql = "SELECT mau.M_Attribute_ID, ma.Name, mau.M_AttributeSet_ID"
                    + " FROM M_AttributeUse mau"
                    + " INNER JOIN M_Attribute ma ON (mau.M_Attribute_ID=ma.M_Attribute_ID)"
                    + " WHERE mau.IsActive='Y' AND ma.IsActive='Y'"
                    + " AND mau.M_AttributeSet_ID=" + attributeSet_id + " ORDER BY mau.SeqNo, mau.M_Attribute_ID";

            DataSet ds = DB.ExecuteDataset(sql);

            if (ds == null || ds.Tables.Count < 1)
            {
                return null;
            }
            else if (ds.Tables[0].Rows.Count > 0)
            {
                string setName = aset.GetName();
                VA005_TreeStructure tree = new VA005_TreeStructure();
                tree.text = setName;
                tree.ImageSource = "Areas/VA005/Images/attSet.png";
                tree.NodeID = 0;
                tree.ParentID = 0;
                tree.expanded = true;
                tree.visibility = "none";
                tree.checkbox = "none";
                tree.ShowInfo = "none";
                tree.padding = "8px 10px 8px 10px";
                tree.margin = "0 0 0 0";
                tree.items = new List<VA005_TreeStructure>();
                LoadAttributeUse(_ctx, ds, tree, attributeSet_id);
                //hidden
                final = new List<VA005_TreeStructure>();
                final.Add(tree);
            }
            return final;
        }

        /// <summary>
        ///  Create Attribute Used in each attribute Set and then fill each attribute Use with respective Value.
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="parentTree"></param>
        /// <param name="parentID"></param>
        public void LoadAttributeUse(Ctx _ctx, DataSet ds, VA005_TreeStructure parentTree, int parentID)
        {
            DataRow[] drs = ds.Tables[0].Select();

            if (drs != null && drs.Length > 0)
            {
                for (int j = 0; j < drs.Length; j++)
                {
                    VA005_TreeStructure attributeSetTree = new VA005_TreeStructure();
                    attributeSetTree.text = Convert.ToString(drs[j]["Name"]);
                    ////attributeSetTree.NodeID = Convert.ToInt32(drs[j]["m_attributeSet_ID"]);===================
                    attributeSetTree.NodeID = Convert.ToInt32(drs[j]["m_attribute_ID"]);
                    attributeSetTree.ParentID = Convert.ToInt32(drs[j]["m_attributeset_ID"]);
                    attributeSetTree.ImageSource = "Areas/VA005/Images/att.png";
                    attributeSetTree.visibility = "inherit";
                    attributeSetTree.checkbox = "none";
                    attributeSetTree.ShowInfo = "inherit";
                    attributeSetTree.padding = "8px 10px 8px 38px";
                    attributeSetTree.margin = "0 0 0 15px";
                    attributeSetTree.Type = "2";
                    attributeSetTree.expanded = true;
                    attributeSetTree.items = new List<VA005_TreeStructure>();
                    parentTree.items.Add(attributeSetTree);
                    LoadAttributeValue(_ctx, attributeSetTree, Convert.ToInt32(drs[j]["m_attribute_ID"]));
                }
            }
        }

        /// <summary>
        /// Create Attribute Values
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="parentTree"></param>
        /// <param name="parentID"></param>
        public void LoadAttributeValue(Ctx _ctx, VA005_TreeStructure parentTree, int parentID)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(MRole.GetDefault(_ctx).AddAccessSQL("SELECT M_Attribute_ID , name,  M_AttributeValue_ID FROM M_AttributeValue WHERE IsActive='Y' AND M_Attribute_ID =" + parentID, "M_AttributeValue", true, true));
            SqlParamsIn sqlpar = new SqlParamsIn();
            sqlpar.sql = sql.ToString();
            sqlpar.pageSize = 0;

            VIS.Helpers.SqlHelper sHelper = new VIS.Helpers.SqlHelper();

            DataSet ds = sHelper.ExecuteDataSet(sqlpar);
            if (ds == null || ds.Tables.Count < 1)
            {
                return;
            }
            else if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow[] drs = ds.Tables[0].Select("m_attribute_id=" + parentID);
                if (drs != null && drs.Length > 0)
                {
                    for (int j = 0; j < drs.Length; j++)
                    {
                        VA005_TreeStructure attributeSetTree = new VA005_TreeStructure();
                        attributeSetTree.text = Convert.ToString(drs[j]["Name"]);
                        attributeSetTree.NodeID = Convert.ToInt32(drs[j]["M_AttributeValue_ID"]);
                        attributeSetTree.ParentID = Convert.ToInt32(drs[j]["M_Attribute_ID"]);
                        attributeSetTree.visibility = "none";
                        attributeSetTree.checkbox = "inherit";
                        attributeSetTree.ShowInfo = "none";
                        attributeSetTree.padding = "8px 10px 8px 38px";
                        attributeSetTree.margin = "0 0 0 20px";
                        attributeSetTree.Type = "3";
                        attributeSetTree.expanded = true;
                        parentTree.items.Add(attributeSetTree);
                    }
                }
            }
        }

        /// <summary>
        /// Save images
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="serverPath"></param>
        /// <param name="file"></param>
        /// <param name="imageID"></param>
        /// <param name="isDatabaseSave"></param>
        /// <returns></returns>
        public int SaveImage(Ctx ctx, string serverPath, HttpPostedFileBase file, int imageID, bool isDatabaseSave)
        {
            HttpPostedFileBase hpf = file as HttpPostedFileBase;

            string savedFileName = Path.Combine(serverPath, Path.GetFileName(hpf.FileName));
            hpf.SaveAs(savedFileName);
            MemoryStream ms = new MemoryStream();
            hpf.InputStream.CopyTo(ms);
            byte[] byteArray = ms.ToArray();
            FileInfo file1 = new FileInfo(savedFileName);
            if (file1.Exists)
            {
                file1.Delete(); //Delete Temporary file             
            }

            string imgByte = Convert.ToBase64String(byteArray);
            var id = CommonFunctions.SaveImage(ctx, byteArray, imageID, hpf.FileName.Substring(hpf.FileName.LastIndexOf('.')), isDatabaseSave);
            return id;
        }

        public VA005_AttributeValueImage SaveAttributeValueImage(Ctx ctx, byte[] buffer, string imageName, bool isSaveInDB, int productID, string attributeValue, string attimages_id)
        {
            string imageDataURL = null;
            //MProduct user = new MProduct(ctx, productID, null);
            //int imageID = Util.GetValueOfInt(user.GetAD_Image_ID());

            int attimagesIDS = 0;
            if (attimages_id != "")
            {
                attimagesIDS = Convert.ToInt32(attimages_id);
            }

            if (attimages_id == "")
            {
                attimages_id = "0";
            }

            string qry = @"SELECT M_ProductAttributeImage_ID FROM M_ProductAttributeImage
                                WHERE M_AttributeValue_ID=" + attributeValue + @"
                                AND m_product_id =" + productID + @"
                                AND ad_image_id =" + attimages_id + @"";
            int getID = Util.GetValueOfInt(VAdvantage.DataBase.DB.ExecuteScalar(qry));
            if (attimagesIDS > 0)
            {
                attimagesIDS = getID;
            }


            VA005_AttributeValueImage obj = new VA005_AttributeValueImage();
            MProductAttributeImage objt = new MProductAttributeImage(ctx, attimagesIDS, null);
            int imageID = objt.GetAD_Image_ID();
            MImage mimg = new MImage(ctx, imageID, null);
            mimg.ByteArray = buffer;
            mimg.ImageFormat = imageName.Substring(imageName.LastIndexOf('.'));
            mimg.SetName(imageName);
            if (isSaveInDB)
            {
                mimg.SetBinaryData(buffer);
                //mimg.SetImageURL(string.Empty);
            }
            //mimg.SetImageURL("Images/Thumb32x32");//Image Saved in File System so instead of byteArray image Url will be set            
            //mimg.SetImageURL("TempFiles/ProductImages");
            //mimg.SetImageURL("Images");
            mimg.SetImageURL(mimg.ImageFormat);

            if (!mimg.Save())
            {
                return obj;
            }
            else
            {
                objt.SetAD_Client_ID(ctx.GetAD_Client_ID());
                objt.SetAD_Org_ID(ctx.GetAD_Org_ID());
                objt.SetM_Product_ID(productID);
                objt.SetM_AttributeValue_ID(Convert.ToInt32(attributeValue));
                objt.SetAD_Image_ID(mimg.GetAD_Image_ID());
                if (!objt.Save())
                {
                    return obj;
                }

                string filePath = Path.Combine(HostingEnvironment.MapPath("~/Images/Thumb32x32"), mimg.GetAD_Image_ID() + mimg.ImageFormat);

                if (File.Exists(filePath))
                {
                    byte[] imageByteData = System.IO.File.ReadAllBytes(filePath);
                    string imageBase64Data = Convert.ToBase64String(imageByteData);
                    imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                }
            }


            obj.urls = imageDataURL;
            obj.imgID = mimg.GetAD_Image_ID();


            return obj;
            //return mimg.GetAD_Image_ID();
        }

        public string DeleteAttributeValues(Ctx ctx, List<VA005_AttributeValueDeletes> lindatatoString)
        {
            string result = "";

            for (int i = 0; i < lindatatoString.Count; i++)
            {
                string qry = @"SELECT M_ProductAttributeImage_id FROM M_ProductAttributeImage
                                WHERE M_AttributeValue_ID=" + lindatatoString[i].M_AttributeValue_ID + @"
                                AND m_product_id =" + lindatatoString[i].M_Product_ID + @"
                                AND ad_image_id =" + lindatatoString[i].AD_Imaeg_ID + @"";
                int getID = Util.GetValueOfInt(VAdvantage.DataBase.DB.ExecuteScalar(qry));

                if (getID > 0)
                {
                    MProductAttributeImage objt = new MProductAttributeImage(ctx, getID, null);
                    if (!objt.Delete(true))
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null)
                            result = pp.GetValue() + " " + pp.ToString();
                        return result;
                    }
                }
            }

            return result;
        }

        public List<VA005_GetAttributeValuesbgt> GetAttributeValues(Ctx _ctx, int product_id)
        {
            List<VA005_GetAttributeValuesbgt> obj = new List<VA005_GetAttributeValuesbgt>();

            string sql = @"SELECT attimage.M_ProductAttributeImage_id AS attimages ,mAttribute.name AS attName, attvalue.name AS attValue, 
                              mAttribute.M_Attribute_id,attvalue.M_AttributeValue_id,
                              adimg.imageurl AS imageurl, adimg.name AS imgName,
                              adimg.ad_image_id AS AD_Image_ID
                              FROM M_Product product 
                              INNER JOIN M_AttributeUse attuse ON attuse.M_AttributeSet_id=product.M_AttributeSet_id 
                              INNER JOIN M_Attribute mAttribute ON attuse.M_Attribute_id=mAttribute.M_Attribute_id 
                              LEFT JOIN M_AttributeValue attvalue ON mAttribute.M_Attribute_id=attvalue.M_Attribute_id 

LEFT JOIN M_ProductAttributeImage attimage ON (attimage.M_Product_id =product.M_Product_id 
AND attimage.M_AttributeValue_ID = attvalue.M_AttributeValue_ID)
                             
LEFT JOIN ad_image adimg ON adimg.ad_image_id    =attimage.ad_image_id
                              WHERE product.m_product_id  =" + product_id +
                             " ORDER BY mAttribute.M_Attribute_id,attvalue.M_AttributeValue_id";

            //LEFT JOIN M_ProductAttributeImage attimage ON attimage.M_Product_id =product.M_Product_id
            sql = MRole.GetDefault(_ctx).AddAccessSQL(sql, "M_Product", true, false);
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        VA005_GetAttributeValuesbgt obj1 = new VA005_GetAttributeValuesbgt();
                        obj1.attributeID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Attribute_ID"]);
                        obj1.AttributeValueID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeValue_id"]);
                        obj1.AttributeName = Util.GetValueOfString(ds.Tables[0].Rows[i]["attName"]);
                        obj1.attValueName = Util.GetValueOfString(ds.Tables[0].Rows[i]["attValue"]);
                        //obj1.attimages = Util.GetValueOfString(ds.Tables[0].Rows[i]["attimages"]);
                        obj1.attimages = Util.GetValueOfString(ds.Tables[0].Rows[i]["AD_Image_ID"]);
                        obj1.imageName = Util.GetValueOfString(ds.Tables[0].Rows[i]["imgName"]);


                        if (ds.Tables[0].Rows[i]["AD_Image_ID"] != DBNull.Value && ds.Tables[0].Rows[i]["AD_Image_ID"] != null && Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Image_ID"]) > 0)
                        {
                            MImage mimg = new MImage(_ctx, Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Image_ID"]), null);
                            var imgfll = mimg.GetThumbnailURL(32, 32);
                            obj1.imageurl = imgfll;

                            if (obj1.imageurl == "FileDoesn'tExist" || obj1.imageurl == "NoRecordFound")
                            {
                                obj1.imageurl = "";
                            }
                        }
                        else
                        {
                            obj1.imageurl = "";
                        }
                        obj.Add(obj1);
                    }
                }
            }

            return obj;
        }


        // Added by Bharat on 28 Feb 2018 to get Image Url from Product
        public string GetImageUrl(string fields, Ctx ctx)
        {
            string imgURL = "";
            int M_Product_ID = Util.GetValueOfInt(fields);
            string sql = "SELECT img.ImageUrl FROM M_Product prd LEFT OUTER JOIN AD_Image img ON prd.AD_Image_ID = img.AD_Image_ID WHERE prd.M_Product_ID = " + M_Product_ID;
            imgURL = Util.GetValueOfString(DB.ExecuteScalar(sql, null, null));
            return imgURL;
        }

        // Added by Bharat on 28 Feb 2018 to get Organizations
        public List<Dictionary<string, object>> LoadOrganization(Ctx ctx)
        {
            List<Dictionary<string, object>> reDIc = null;
            string sql = MRole.GetDefault(ctx).AddAccessSQL("SELECT AD_Org_ID, Name FROM AD_Org WHERE IsActive = 'Y' AND IsOrgUnit='N' AND (IsSummary='N' OR AD_Org_ID=0) ",
                    "AD_Org", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO) + " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                reDIc = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["AD_Org_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Org_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    reDIc.Add(obj);
                }
            }
            return reDIc;
        }

        // Added by Bharat on 28 Feb 2018 to get Product Type
        public List<Dictionary<string, object>> LoadProductType(Ctx ctx)
        {
            List<Dictionary<string, object>> reDIc = null;
            string sql = "SELECT Value, Name FROM  AD_Ref_List WHERE AD_Reference_ID = 270 AND IsActive = 'Y'";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                reDIc = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["Value"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    reDIc.Add(obj);
                }
            }
            return reDIc;
        }

        // Added by Bharat on 28 Feb 2018 to get Attribute Set
        public List<Dictionary<string, object>> LoadAttributes(Ctx ctx)
        {
            List<Dictionary<string, object>> reDIc = null;
            string sql = MRole.GetDefault(ctx).AddAccessSQL("SELECT M_AttributeSet_ID, Name FROM M_AttributeSet WHERE IsActive = 'Y'",
                    "M_AttributeSet", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO) + " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                reDIc = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["M_AttributeSet_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSet_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    reDIc.Add(obj);
                }
            }
            return reDIc;
        }

        // Added by Bharat on 28 Feb 2018 to get Tax Categories
        public List<Dictionary<string, object>> LoadTaxCategories(Ctx ctx)
        {
            List<Dictionary<string, object>> reDIc = null;
            string sql = MRole.GetDefault(ctx).AddAccessSQL("SELECT C_TaxCategory_ID, Name FROM C_TaxCategory WHERE IsActive = 'Y'",
                    "C_TaxCategory", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO) + " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                reDIc = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["C_TaxCategory_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_TaxCategory_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    reDIc.Add(obj);
                }
            }
            return reDIc;
        }

        // Added by Bharat on 28 Feb 2018 to get Product Categories
        public List<Dictionary<string, object>> LoadProductCategories(Ctx ctx)
        {
            List<Dictionary<string, object>> reDIc = null;
            string sql = MRole.GetDefault(ctx).AddAccessSQL("SELECT M_Product_Category_ID, Name FROM M_Product_Category WHERE IsActive = 'Y'",
                    "M_Product_Category", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO) + " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                reDIc = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["M_Product_Category_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_Category_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    reDIc.Add(obj);
                }
            }
            return reDIc;
        }

        // Added by Bharat on 28 Feb 2018 to get UOM
        public List<Dictionary<string, object>> LoadUOM(Ctx ctx)
        {
            List<Dictionary<string, object>> reDIc = null;
            string sql = MRole.GetDefault(ctx).AddAccessSQL("SELECT C_UOM_ID, Name FROM C_UOM WHERE IsActive = 'Y'",
                    "C_UOM", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO) + " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                reDIc = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["C_UOM_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    reDIc.Add(obj);
                }
            }
            return reDIc;
        }

        // Added by Bharat on 05 March 2018 to get Currency
        public List<Dictionary<string, object>> LoadCurrency(Ctx ctx)
        {
            List<Dictionary<string, object>> reDIc = null;
            string sql = MRole.GetDefault(ctx).AddAccessSQL("SELECT C_Currency_ID, ISO_Code || ' (' || (CurSymbol) ||')' as cur FROM C_Currency WHERE IsActive = 'Y'",
                    "C_Currency", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO) + " ORDER BY ISO_Code";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                reDIc = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["C_Currency_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Currency_ID"]);
                    obj["ISO_Code"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["cur"]);
                    reDIc.Add(obj);
                }
            }
            return reDIc;
        }

        // Added by Bharat on 05 March 2018 to get Price List
        public List<Dictionary<string, object>> LoadPriceList(Ctx ctx)
        {
            List<Dictionary<string, object>> reDIc = null;
            string sql = MRole.GetDefault(ctx).AddAccessSQL("SELECT M_PriceList_Version_ID, Name FROM M_PriceList_Version WHERE IsActive = 'Y'",
                    "M_PriceList_Version", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                reDIc = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["M_PriceList_Version_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_PriceList_Version_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    reDIc.Add(obj);
                }
            }
            return reDIc;
        }

        // Added by Bharat on 05 March 2018 to get Supplier
        public List<Dictionary<string, object>> LoadSupplier(Ctx ctx)
        {
            List<Dictionary<string, object>> reDIc = null;
            string sql = MRole.GetDefault(ctx).AddAccessSQL("SELECT C_BPartner_ID, Name FROM C_BPartner WHERE IsActive = 'Y' AND IsVendor = 'Y'",
                    "C_BPartner", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO) + " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                reDIc = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["C_BPartner_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_BPartner_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    reDIc.Add(obj);
                }
            }
            return reDIc;
        }

        // Added by Bharat on 05 March 2018 to get Cart
        public List<Dictionary<string, object>> LoadCart(Ctx ctx)
        {
            List<Dictionary<string, object>> reDIc = null;
            string sql = MRole.GetDefault(ctx).AddAccessSQL("SELECT VAICNT_InventoryCount_ID, VAICNT_ScanName FROM VAICNT_InventoryCount WHERE IsActive = 'Y' AND VAICNT_TransactionType = 'OT'",
                    "VAICNT_InventoryCount", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO) + " ORDER BY VAICNT_ScanName";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                reDIc = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["VAICNT_InventoryCount_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAICNT_InventoryCount_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["VAICNT_ScanName"]);
                    reDIc.Add(obj);
                }
            }
            return reDIc;
        }

        // Added by Bharat on 05 March 2018 to get Queries
        public List<Dictionary<string, object>> LoadQueries(Ctx ctx)
        {
            List<Dictionary<string, object>> reDIc = null;
            string sql = "SELECT AD_UserQuery_ID, Name FROM AD_UserQuery WHERE IsActive = 'Y' AND AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND AD_Tab_ID = 180";
            DataSet ds = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql, "AD_UserQuery", true, false), null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                reDIc = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["AD_UserQuery_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_UserQuery_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    reDIc.Add(obj);
                }
            }
            return reDIc;
        }

        // Added by Bharat on 05 March 2018 to get Categoty Data
        public Dictionary<string, object> LoadOnCategorySelect(Ctx ctx, int _PCat_ID)
        {
            Dictionary<string, object> obj = null;
            StringBuilder Sql = new StringBuilder();
            Tuple<string, string, string> mInfo = null;
            bool _countDTD001 = Env.HasModulePrefix("DTD001_", out mInfo);
            Sql.Append("SELECT IsActive");
            if (_countDTD001)
            {
                Sql.Append(", ProductType");
            }
            Sql.Append(", M_AttributeSet_ID, C_TaxCategory_ID FROM M_Product_Category WHERE M_Product_Category_ID = " + _PCat_ID);
            DataSet ds = DB.ExecuteDataset(Sql.ToString(), null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                obj = new Dictionary<string, object>();
                obj["_countDTD001"] = _countDTD001;
                if (_countDTD001)
                {
                    obj["ProductType"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["ProductType"]);
                }
                obj["M_AttributeSet_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_AttributeSet_ID"]);
                obj["C_TaxCategory_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_TaxCategory_ID"]);

            }
            return obj;
        }

        // Added by Bharat on 05 March 2018 to get Related Type
        public List<Dictionary<string, object>> LoadRelatedType(Ctx ctx)
        {
            List<Dictionary<string, object>> reDIc = null;
            string sql = "SELECT Value, Name FROM AD_Ref_List WHERE AD_Reference_ID = (SELECT AD_Reference_ID FROM AD_Reference WHERE Name = 'M_RelatedProduct Type')";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                reDIc = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["Value"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    reDIc.Add(obj);
                }
            }
            return reDIc;
        }

        // Added by Bharat on 05 March 2018 to get Window ID from Name
        public int GetWindow_ID(string fields, Ctx ctx)
        {
            int window_ID = 0;
            string windowName = Util.GetValueOfString(fields);
            string sql = "SELECT AD_Window_ID FROM AD_Window WHERE Name = '" + windowName + "'";
            window_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            return window_ID;
        }

        // Added by Bharat on 05 March 2018 to get Product Details
        public Dictionary<string, object> LoadProductData(int prod_ID, Ctx ctx)
        {
            Dictionary<string, object> obj = null;
            string sql = "SELECT AD_Org_ID, Name, Value, C_UOM_ID, M_AttributeSet_ID, M_Product_Category_ID, ProductType, C_TaxCategory_ID, UPC FROM M_Product WHERE M_Product_ID = " + prod_ID;
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                obj = new Dictionary<string, object>();
                obj["AD_Org_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_Org_ID"]);
                obj["Value"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Value"]);
                obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Name"]);
                obj["C_UOM_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_UOM_ID"]);
                obj["M_AttributeSet_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_AttributeSet_ID"]);
                obj["M_Product_Category_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_Product_Category_ID"]);
                obj["ProductType"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["ProductType"]);
                obj["C_TaxCategory_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_TaxCategory_ID"]);
                obj["UPC"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["UPC"]);
            }
            return obj;
        }

        // Added by Bharat on 05 March 2018 to get UOM Conversion Data
        public Dictionary<string, object> LoadUOMConversionData(int c_UomConv_ID, Ctx ctx)
        {
            // Added by shifali on 06 July 2020 to get DivideRate and MultiplyRate Data
            Dictionary<string, object> obj = null;
            string sql = @"SELECT prd.Name, prd.M_Product_ID, uc.C_UOM_To_ID, uc.UPC,uc.MultiplyRate AS DivideRate, uc.DivideRate AS MultiplyRate 
                FROM C_UOM_Conversion uc INNER JOIN M_Product prd ON uc.M_Product_ID = prd.M_Product_ID WHERE uc.C_UOM_Conversion_ID = " + c_UomConv_ID;
            DataSet ds = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql, "uc", true, false), null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                obj = new Dictionary<string, object>();
                obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Name"]);
                obj["C_UOM_To_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_UOM_To_ID"]);
                obj["M_Product_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_Product_ID"]);
                obj["UPC"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["UPC"]);
                obj["DivideRate"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["DivideRate"]);
                obj["MultiplyRate"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["MultiplyRate"]);
            }
            return obj;
        }

        // Added by Bharat on 05 March 2018 to get Product Attribute Data
        public Dictionary<string, object> LoadProdAttributeData(int product_ID, int m_attribute_ID, string upcvalue, Ctx ctx)
        {
            Dictionary<string, object> obj = null;
            string sql = "SELECT prd.Name, prd.M_Product_ID, prd.C_UOM_ID, patr.M_AttributeSetInstance_ID, ats.Description, patr.UPC FROM M_ProductAttributes patr " +
                "INNER JOIN M_Product prd ON patr.M_Product_ID = prd.M_Product_ID INNER JOIN M_AttributeSetInstance ats ON patr.M_AttributeSetInstance_ID = ats.M_AttributeSetInstance_ID " +
                "WHERE patr.M_Product_ID = " + product_ID + " AND patr.M_AttributeSetInstance_ID = " + m_attribute_ID + " AND nvl(patr.UPC,' ') = '" + upcvalue + "'";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                obj = new Dictionary<string, object>();
                obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Name"]);
                obj["C_UOM_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_UOM_ID"]);
                obj["M_Product_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_Product_ID"]);
                obj["M_AttributeSetInstance_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_AttributeSetInstance_ID"]);
                obj["Description"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Description"]);
                obj["UPC"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["UPC"]);
            }
            return obj;
        }

        // Added by Bharat on 05 March 2018 to get Product Data from Cart
        public Dictionary<string, object> LoadProductCartData(int ProdID, Ctx ctx)
        {
            Dictionary<string, object> obj = null;
            string sql = "SELECT prd.Name, prd.C_UOM_ID, prd.M_AttributeSetInstance_ID, ats.Description, prd.UPC FROM M_Product prd LEFT OUTER JOIN M_AttributeSetInstance ats " +
                    "ON prd.M_AttributeSetInstance_ID = ats.M_AttributeSetInstance_ID WHERE prd.M_Product_ID = " + ProdID;
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                obj = new Dictionary<string, object>();
                obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Name"]);
                obj["C_UOM_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_UOM_ID"]);
                obj["M_AttributeSetInstance_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_AttributeSetInstance_ID"]);
                obj["Description"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Description"]);
                obj["UPC"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["UPC"]);
            }
            return obj;
        }

        // Added by Bharat on 05 March 2018 to get UPC of Product
        public string GetProductUPC(string fields, Ctx ctx)
        {
            string upc = "";
            int prod_ID = Util.GetValueOfInt(fields);
            string sql = "SELECT UPC FROM M_ProductAttributes WHERE M_ProductAttributes_ID = " + prod_ID;
            upc = Util.GetValueOfString(DB.ExecuteScalar(sql, null, null));
            return upc;
        }

        // Added by Bharat on 05 March 2018 to get UOM Conversion Data of UOM
        public Dictionary<string, object> LoadUOMRate(int c_UomConv_ID, Ctx ctx)
        {
            Dictionary<string, object> obj = null;
            string sql = "SELECT C_UOM_To_ID, MultiplyRate AS DivideRate, DivideRate AS MultiplyRate, UPC FROM C_UOM_Conversion WHERE C_UOM_Conversion_ID=" + c_UomConv_ID;
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                obj = new Dictionary<string, object>();
                obj["C_UOM_To_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_UOM_To_ID"]);
                obj["DivideRate"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["DivideRate"]);
                obj["MultiplyRate"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["MultiplyRate"]);
                obj["UPC"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["UPC"]);
            }
            return obj;
        }

        // Added by Bharat on 05 March 2018 to get UOM Conversion Data of Product
        public List<Dictionary<string, object>> LoadUomGroup(int prod_ID, Ctx ctx)
        {
            List<Dictionary<string, object>> retDic = null;
            string sql = "SELECT (SELECT u.Name FROM C_UOM u WHERE u.C_UOM_ID = uc.C_UOM_TO_ID) AS UomTo, uc.C_UOM_TO_ID, uc.MultiplyRate AS DivideRate, uc.DivideRate AS MultiplyRate, uc.C_UOM_Conversion_ID," +
            " uc.UPC FROM C_UOM_Conversion uc INNER JOIN M_Product p ON uc.M_Product_ID = p.M_Product_ID WHERE p.M_Product_ID =" + prod_ID;
            DataSet ds = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql, "uc", true, false), null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["UomTo"]);
                    obj["C_UOM_To_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_To_ID"]);
                    obj["DivideRate"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["DivideRate"]);
                    obj["MultiplyRate"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["MultiplyRate"]);
                    obj["C_UOM_Conversion_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_Conversion_ID"]);
                    obj["UPC"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["UPC"]);
                    retDic.Add(obj);
                }
            }
            return retDic;
        }

        // Added by Bharat on 05 March 2018 to get Varients Data of Product
        public List<Dictionary<string, object>> LoadVarients(int prod_ID, Ctx ctx)
        {
            List<Dictionary<string, object>> retDic = null;
            string sql = "SELECT patr.UPC,patr.M_AttributeSetInstance_ID,ats.Description,patr.M_ProductAttributes_ID FROM M_ProductAttributes patr INNER JOIN M_AttributeSetInstance ats ON" +
                    " (patr.M_AttributeSetInstance_ID = ats.M_AttributeSetInstance_ID) WHERE patr.M_Product_ID = " + prod_ID;
            DataSet ds = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql, "patr", true, false), null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["M_AttributeSetInstance_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                    obj["Description"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Description"]);
                    obj["M_ProductAttributes_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_ProductAttributes_ID"]);
                    obj["UPC"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["UPC"]);
                    retDic.Add(obj);
                }
            }
            return retDic;
        }

        // Added by Bharat on 05 March 2018 to get Price List Data 
        public Dictionary<string, object> LoadPriceListData(int pricelist_ID, Ctx ctx)
        {
            Dictionary<string, object> obj = null;
            string sql = "SELECT pl.PricePrecision, cur.ISO_Code FROM M_PriceList_Version plv INNER JOIN M_PriceList pl ON plv.M_PriceList_ID=pl.M_PriceList_ID " +
                        " INNER JOIN C_Currency cur ON pl.C_Currency_ID = cur.C_Currency_ID WHERE plv.M_PriceList_Version_ID = " + pricelist_ID;
            DataSet ds = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql, "plv", true, false), null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                obj = new Dictionary<string, object>();
                obj["PricePrecision"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["PricePrecision"]);
                obj["ISO_Code"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["ISO_Code"]);
            }
            return obj;
        }

        public decimal GetListPrice(int PriceListVersion, int product_ID, int uom, int attribute)
        {
            string sql = "SELECT pr.PriceList FROM M_ProductPrice pr INNER JOIN M_PriceList_Version plv" +
                    " ON pr.M_PriceList_Version_ID = plv.M_PriceList_Version_ID WHERE pr.M_PriceList_Version_ID=" + PriceListVersion
                    + " AND pr.M_Product_ID = " + product_ID +
                    " AND pr.C_UOM_ID = " + uom + " AND pr.M_AttributeSetInstance_ID = " + attribute;

            decimal price = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
            return price;
        }

        // Added by Bharat on 05 March 2018 to get Varients Data of Product
        public List<Dictionary<string, object>> LoadReleatedData(int prod_ID, Ctx ctx)
        {
            List<Dictionary<string, object>> retDic = null;
            string sql = "SELECT s.Name AS Product, p.Name as RelatedProduct, s.RelatedProductType, s.RelatedProduct_ID"
                + " FROM M_RelatedProduct s INNER JOIN M_Product p ON (p.M_Product_ID = s.RelatedProduct_ID)"
                + " WHERE s.IsActive='Y' AND s.M_Product_ID = " + prod_ID;
            DataSet ds = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql, "s", true, false), null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["RelatedProduct_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["RelatedProduct_ID"]);
                    obj["Product"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Product"]);
                    obj["RelatedProductType"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["RelatedProductType"]);
                    obj["RelatedProduct"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["RelatedProduct"]);
                    retDic.Add(obj);
                }
            }
            return retDic;
        }

        // Added by Bharat on 05 March 2018 to get Cart Data of Product
        public List<Dictionary<string, object>> LoadCartData(int invCount_ID, Ctx ctx)
        {
            List<Dictionary<string, object>> retDic = null;
            string sql = "SELECT po.VAICNT_InventoryCountLine_ID,po.M_Product_ID,prd.Name, po.C_UOM_ID, u.Name AS UOM, po.UPC, po.M_AttributeSetInstance_ID, ats.Description, po.VAICNT_Quantity," +
                        " prd.M_AttributeSet_ID FROM VAICNT_InventoryCountLine po LEFT JOIN C_UOM u ON po.C_UOM_ID = u.C_UOM_ID LEFT JOIN M_Product prd" +
                        " ON po.M_Product_ID= prd.M_Product_ID LEFT JOIN M_AttributeSetInstance ats ON po.M_AttributeSetInstance_ID = ats.M_AttributeSetInstance_ID" +
                        " WHERE po.IsActive = 'Y' AND po.VAICNT_InventoryCount_ID = " + invCount_ID + " ORDER BY po.Line";
            DataSet ds = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql, "po", true, false), null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["VAICNT_InventoryCountLine_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAICNT_InventoryCountLine_ID"]);
                    obj["M_Product_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    obj["C_UOM_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]);
                    obj["UOM"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["UOM"]);
                    obj["UPC"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["UPC"]);
                    obj["M_AttributeSetInstance_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                    obj["Description"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Description"]);
                    obj["VAICNT_Quantity"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["VAICNT_Quantity"]);
                    obj["M_AttributeSet_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSet_ID"]);
                    retDic.Add(obj);
                }
            }
            return retDic;
        }

        // Added by Bharat on 06 March 2018 to get Parent Product Data
        public Dictionary<string, object> LoadParentData(Ctx ctx, int parent_ID)
        {
            Dictionary<string, object> obj = null;
            string sql = "SELECT C_TaxCategory_ID, M_Product_Category_ID FROM M_Product WHERE M_Product_ID = " + parent_ID; ;
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                obj = new Dictionary<string, object>();
                obj["M_Product_Category_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_Product_Category_ID"]);
                obj["C_TaxCategory_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_TaxCategory_ID"]);

            }
            return obj;
        }
    }


    public class VA005_GetAttributeValuesbgt
    {
        public int attributeID { get; set; }
        public int AttributeValueID { get; set; }
        public string AttributeName { get; set; }
        public string attValueName { get; set; }
        public string imageurl { get; set; }
        public string imageName { get; set; }
        public string attimages { get; set; }
    }
    public class VA005_AttributeValueDeletes
    {
        public string M_AttributeValue_ID { get; set; }
        public string M_Product_ID { get; set; }
        public string AD_Imaeg_ID { get; set; }
    }

    public class VA005_AttributeValueImage
    {
        public string urls { get; set; }
        public int imgID { get; set; }
    }

    public class ProductData
    {
        public List<ProductInfo> UserInfo { get; set; }

    }

    public class ProductInfo
    {
        public string Prodname { get; set; }
        public int M_ProductID { get; set; }
        public int AD_OrgID { get; set; }
        public int AD_ClientID { get; set; }
        public int M_ProdCatID { get; set; }
        public int M_AttributeSetID { get; set; }
        public string SearchKey { get; set; }
        public string ProCatName { get; set; }
        public int C_UomID { get; set; }
        public string UOM { get; set; }
        public string ProductType { get; set; }
        public bool IsActive { get; set; }
        public string ProdImage { get; set; }
        public int UserTableID { get; set; }
        public int UserWindowID { get; set; }
        public int productCount { get; set; }
        public string UPC { get; set; }
    }

    public class ProdCatInfo
    {
        public string Catname { get; set; }
        public int ProdCount { get; set; }
        public int M_ProdCatID { get; set; }
        public int TotalRecords { get; set; }
    }

    public class columnInfo
    {
        public string KEYVALUE { get; set; }
        public string KEYNAME { get; set; }
        public string VALUE1NAME { get; set; }
        public string VALUE1VALUE { get; set; }
    }

    public class PriceInfo
    {
        public int recid { get; set; }
        public int LineID { get; set; }
        public int product_ID { get; set; }
        public string Product { get; set; }
        public decimal PriceList { get; set; }
        public decimal PriceStd { get; set; }
        public decimal PriceLimit { get; set; }
        public int C_Uom_ID { get; set; }
        public string UOM { get; set; }
        public string Lot { get; set; }
        public string UPC { get; set; }
        public decimal Qty { get; set; }
        public int attribute_ID { get; set; }
        public string Attribute { get; set; }
        public bool updated { get; set; }
        public decimal OrderMin { get; set; }
        public decimal OrderPack { get; set; }
        public int C_Currency_ID { get; set; }
        public int DeliveryTime { get; set; }
        public string ImageUrl { get; set; }
    }

    public class AttributeValues
    {
        public int pid { get; set; }
        public int Nid { get; set; }
    }

    public class RelatedInfo
    {
        public string Product { get; set; }
        public string RelatedType { get; set; }
        public int M_Product_ID { get; set; }
    }

    public class ProdInfo
    {
        public string error { get; set; }
        public int M_Product_ID { get; set; }
    }
}