using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Web.Helpers;
using System.Web.Hosting;
using VAdvantage.DataBase;
using VIS.Classes;
using System.Data;

namespace VA005.Models
{
    public class ProductCategortModel
    {
        public KeyNamePair AddCategory(string name, VAdvantage.Utility.Ctx ctx)
        {
            KeyNamePair category = null;
            VAdvantage.Model.MProductCategory pcat = new VAdvantage.Model.MProductCategory(ctx, 0, null);
            pcat.SetAD_Client_ID(ctx.GetAD_Client_ID());
            pcat.SetAD_Org_ID(ctx.GetAD_Org_ID());
            pcat.SetName(name);
            if (pcat.Save())
            {
                category = new KeyNamePair(pcat.Get_ID(), name);
                return category;
            }
            return category;
        }

        public bool UpdateCategory(Int32 id, string name, VAdvantage.Utility.Ctx ctx)
        {
            VAdvantage.Model.MProductCategory pcat = new VAdvantage.Model.MProductCategory(ctx, id, null);
            pcat.SetName(name);
            if (pcat.Save())
            {
                return true;
            }
            return false;
        }

        public bool UpdateCategory(Int32 id, String Name, String Value, String Producttype, String matPolicy, String Desc, Int32 attrSet, Int32 taxcat, Int32 assetGrp, Boolean Consumable, Int32 image_ID, VAdvantage.Utility.Ctx ctx)
        {
            VAdvantage.Model.MProductCategory pcat = new VAdvantage.Model.MProductCategory(ctx, id, null);
            pcat.SetName(Name);
            pcat.SetValue(Value);
            pcat.SetProductType(Producttype);
            pcat.SetMMPolicy(matPolicy);
            pcat.SetDescription(Desc);
            pcat.SetM_AttributeSet_ID(attrSet);
            pcat.SetC_TaxCategory_ID(taxcat);
            pcat.SetA_Asset_Group_ID(assetGrp);
            pcat.SetAD_Image_ID(image_ID);
            Tuple<String, String, String> mInfo = null;
            if (Env.HasModulePrefix("DTD001_", out mInfo))
            {
                pcat.SetDTD001_IsConsumable(Consumable);
            }
            if (pcat.Save())
            {
                return true;
            }
            return false;
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
        /// <summary>
        /// Method to delete product category 
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="pcats">ProductCategoryID</param>
        /// <returns>Result</returns>
        public List<KeyNamePair> DeleteCategory(Ctx ctx, string[] pcats)
        {

            //string[] param = pcats.Split(',');

            List<KeyNamePair> NameList = new List<KeyNamePair>();
            for (int i = 0; i < pcats.Length; i++)
            {
                string sql = "DELETE FROM M_Product_Category WHERE M_Product_Category_ID = " + pcats[i];
                int result = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
                KeyNamePair obj = new KeyNamePair();
                obj.Key = Util.GetValueOfInt(pcats[i]);
                obj.Name = string.Empty;
                if (result < 0)
                {
                    string str = "SELECT Name FROM M_Product_Category WHERE M_Product_Category_ID = " + pcats[i];
                    obj.Name = Util.GetValueOfString(DB.ExecuteScalar(str, null, null));
                }
                NameList.Add(obj);
            }
            return NameList;
        }
        /// <summary>
        /// Method For Attribute DropDown
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <returns>Fill The Drop Down</returns>
        public List<Attribut> LoadAttribut(Ctx ctx)
        {
            List<Attribut> PPData = new List<Attribut>();
            string sql = "SELECT M_AttributeSet_ID,Name FROM M_AttributeSet WHERE IsActive = 'Y' AND AD_Client_ID = " + ctx.GetAD_Client_ID();
            DataSet ds = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql, "M_AttributeSet", true, false), null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Attribut PData = new Attribut();
                    PData.M_AttributeSet_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSet_ID"]);
                    PData.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    PPData.Add(PData);
                }
            }
            return PPData;
        }
        /// <summary>
        /// Method For Tax Category Drop Down
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <returns>Data into Drop Down</returns>
        public List<TaxCategory> LoadTaxCategory(Ctx ctx)
        {
            List<TaxCategory> PPData = new List<TaxCategory>();
            string sql = "SELECT C_TaxCategory_ID,Name FROM C_TaxCategory WHERE IsActive = 'Y' AND AD_Client_ID = " + ctx.GetAD_Client_ID();
            DataSet ds = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql, "C_TaxCategory", true, false), null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    TaxCategory PData = new TaxCategory();
                    PData.C_TaxCategory_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_TaxCategory_ID"]);
                    PData.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    PPData.Add(PData);
                }
            }
            return PPData;
        }
        /// <summary>
        /// Method For Asset Drop Dowm
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <returns>Data into Drop Down</returns>
        public List<AssetGroup> LoadAssetGroup(Ctx ctx)
        {
            List<AssetGroup> PPData = new List<AssetGroup>();
            string sql = "SELECT A_Asset_Group_ID,Name FROM A_Asset_Group WHERE IsActive = 'Y' AND AD_Client_ID = " + ctx.GetAD_Client_ID();
            DataSet ds = DB.ExecuteDataset(MRole.GetDefault(ctx).AddAccessSQL(sql, "A_Asset_Group", true, false), null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AssetGroup PData = new AssetGroup();
                    PData.A_Asset_Group_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["A_Asset_Group_ID"]);
                    PData.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    PPData.Add(PData);
                }
            }
            return PPData;
        }
        /// <summary>
        /// Method For Category
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="M_Product_Category_ID">Product_ID</param>
        /// <returns>Data into Drop Down</returns>
        public List<Category> GetCategory(Ctx ctx, int M_Product_Category_ID)
        {
            List<Category> PPData = new List<Category>();
            string sql = @"SELECT pc.Name,pc.Value,pc.M_AttributeSet_ID,pc.ProductType,pc.MMPolicy,pc.Description,pc.C_TaxCategory_ID,pc.A_Asset_Group_ID,pc.DTD001_IsConsumable,pc.AD_Image_ID,img.ImageUrl,img.BinaryData FROM M_Product_Category pc" +
             " LEFT JOIN AD_Image img ON pc.AD_Image_ID = img.AD_Image_ID WHERE pc.M_Product_Category_ID = " + M_Product_Category_ID;
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Category PData = new Category();
                    PData.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    PData.Value = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]);
                    PData.M_AttributeSet_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSet_ID"]);
                    PData.ProductType = Util.GetValueOfString(ds.Tables[0].Rows[i]["ProductType"]);
                    PData.MMPolicy = Util.GetValueOfString(ds.Tables[0].Rows[i]["MMPolicy"]);
                    PData.Description = Util.GetValueOfString(ds.Tables[0].Rows[i]["Description"]);
                    PData.C_TaxCategory_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_TaxCategory_ID"]);
                    PData.A_Asset_Group_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["A_Asset_Group_ID"]);
                    PData.DTD001_IsConsumable = Util.GetValueOfString(ds.Tables[0].Rows[i]["DTD001_IsConsumable"]);
                    PData.AD_Image_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Image_ID"]);
                    PData.ImageUrl = Util.GetValueOfString(ds.Tables[0].Rows[i]["ImageUrl"]);
                    PData.BinaryData = Util.GetValueOfString(ds.Tables[0].Rows[i]["BinaryData"]);
                    PPData.Add(PData);
                }
            }
            return PPData;
        }
        /// <summary>
        /// Methode For Load Category Drop down
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <returns>Data into Drop Down</returns>
        public List<LoadCategory> LoadCategory(Ctx ctx, int PGNo, int PGSize)
        {
            List<LoadCategory> PPData = new List<LoadCategory>();
            string sql = @"SELECT pc.Name,pc.M_Product_Category_ID,img.ImageUrl,img.BinaryData FROM M_Product_Category pc LEFT JOIN AD_Image img ON pc.AD_Image_ID = img.AD_Image_ID WHERE pc.IsActive = 'Y' AND pc.AD_Client_ID = " + ctx.GetAD_Client_ID();
            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "pc", true, false);
            DataSet ds = VIS.DBase.DB.ExecuteDatasetPaging(sql, PGNo, PGSize);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    LoadCategory PData = new LoadCategory();
                    PData.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    PData.M_Product_Category_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_Category_ID"]);
                    PData.ImageUrl = Util.GetValueOfString(ds.Tables[0].Rows[i]["ImageUrl"]);

                    PPData.Add(PData);
                }
            }
            return PPData;
        }
        /// <summary>
        /// Method For ZoomWindow
        /// </summary>
        /// <param name="windowName">Window Name</param>
        /// <returns>Load Window</returns>
        public int LoadWindow(string windowName)
        {
            string sql = "SELECT AD_Window_ID FROM AD_Window WHERE Name = '" + windowName + "'";
            int rule = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            return rule;
        }
      
        /// <summary>
        /// Get imgUrl
        /// </summary>
        /// <param name="ad_image_id">Image ID</param>
        /// <returns>Url Data</returns>
        public string GetimgUrl(int ad_image_id)
        {
            string sql = "SELECT ImageUrl FROM AD_Image WHERE AD_Image_ID = " + ad_image_id;
            string rule = Util.GetValueOfString(DB.ExecuteScalar(sql, null, null));
            return rule;
        }
        /// <summary>
        /// Get Category Count
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <returns>Category Count</returns>
        public string GetAddCategory(Ctx ctx)
        {
            string sql = "SELECT COUNT(M_Product_Category_ID) FROM M_Product_Category WHERE IsActive = 'Y' AND AD_Client_ID = " + ctx.GetAD_Client_ID();
            string rule = Util.GetValueOfString(DB.ExecuteScalar(MRole.GetDefault(ctx).AddAccessSQL(sql, "M_Product_Category", true, false), null, null));
            return rule;
        }
    }

    /// <summary>
    /// Declare Properties For Attribute DropDown
    /// </summary>
    public class Attribut
    {
        public int M_AttributeSet_ID { get; set; }

        public string Name { get; set; }
    }
    /// <summary>
    /// Declare Properties For Tax Category DropDown
    /// </summary>
    public class TaxCategory
    {
        public int C_TaxCategory_ID { get; set; }

        public string Name { get; set; }
    }
    /// <summary>
    /// Declare Properties For Tax AssetGroup DropDown
    /// </summary>
    public class AssetGroup
    {
        public int A_Asset_Group_ID { get; set; }

        public string Name { get; set; }
    }
    /// <summary>
    /// Declare Properties For Category
    /// </summary>
    public class Category
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int M_AttributeSet_ID { get; set; }
        public string ProductType { get; set; }
        public string MMPolicy { get; set; }
        public string Description { get; set; }
        public int C_TaxCategory_ID { get; set; }
        public int A_Asset_Group_ID { get; set; }
        public string DTD001_IsConsumable { get; set; }
        public int AD_Image_ID { get; set; }
        public string ImageUrl { get; set; }
        public string BinaryData { get; set; }
    }
    /// <summary>
    /// Declare Properties For Load Category
    /// </summary>
    public class LoadCategory
    {
        public string Name { get; set; }
        public int M_Product_Category_ID { get; set; }
        public string ImageUrl { get; set; }
    }

}