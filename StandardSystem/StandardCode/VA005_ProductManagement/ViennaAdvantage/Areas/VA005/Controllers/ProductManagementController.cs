using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Filters;
using VIS.Models;
using VA005.Models;
using System.Web.Hosting;
using System.Data;
using VAdvantage.DataBase;


namespace VA005.Controllers
{
    public class ProductManagementController : Controller
    {
        static List<VA005.Models.AttachedFileInfo> lstAttachmentInfo = new List<VA005.Models.AttachedFileInfo>();
        static bool isCancle = false;
        public ActionResult Index(int windowNo)
        {
            ViewBag.WindowNo = windowNo;
            return PartialView(Session["ctx"] as VAdvantage.Utility.Ctx);
        }

        public JsonResult GetProdInfo(string searchText, int SearchQuery, int proCategory, int pageNo, int pageSize, int Parent_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            ProductManagementModel model = new ProductManagementModel();
            List<ProductInfo> prods = model.GeProdInfo(searchText, SearchQuery, proCategory, pageNo, pageSize, ct, Parent_ID);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProdCats(string searchText, int pageNo, int pageSize)
        {
            Ctx ct = Session["ctx"] as Ctx;
            ProductManagementModel model = new ProductManagementModel();
            List<ProdCatInfo> prods = model.GetProdCats(searchText, pageNo, pageSize, ct);
            return Json(JsonConvert.SerializeObject(prods), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductDetails(int M_Product_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            ProductManagementModel model = new ProductManagementModel();
            DataSet dsProd = model.GetProductDetails(M_Product_ID, ct);
            return Json(JsonConvert.SerializeObject(dsProd), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSupplierData(List<int> M_Product_ID, int Supplier)
        {
            Ctx ct = Session["ctx"] as Ctx;
            ProductManagementModel model = new ProductManagementModel();
            List<PriceInfo> dsProd = model.GetSupplierData(M_Product_ID, Supplier, ct);
            return Json(JsonConvert.SerializeObject(dsProd), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductPrices(List<int> M_Product_ID, int PriceList)
        {
            Ctx ct = Session["ctx"] as Ctx;
            ProductManagementModel model = new ProductManagementModel();
            List<PriceInfo> dsProd = model.GetProductPrices(M_Product_ID, PriceList, ct);
            return Json(JsonConvert.SerializeObject(dsProd), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetRelatedProduct(int M_Product_ID, int RelatedProduct)
        {
            Ctx ct = Session["ctx"] as Ctx;
            ProductManagementModel model = new ProductManagementModel();
            string data = model.SetRelatedProduct(M_Product_ID, RelatedProduct, ct);
            return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateRelatedProduct(Int32 id, List<RelatedInfo> relatedData)
        {
            string value = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                ProductManagementModel model = new ProductManagementModel();
                value = model.UpdateRelatedProduct(id, relatedData, ctx);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult deleteRelatedProduct(Int32 id, List<RelatedInfo> relatedData)
        {
            string value = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                ProductManagementModel model = new ProductManagementModel();
                value = model.deleteRelatedProduct(id, relatedData, ctx);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdatePrice(Int32 id, List<PriceInfo> priceData)
        {
            string value = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                ProductManagementModel model = new ProductManagementModel();
                value = model.UpdatePrice(id, priceData, ctx);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateProduct(List<Int32> prodIDs, List<columnInfo> ColumnName)
        {
            string value = "";
            List<String> prodID = new List<String>();
            //if (prodIDs != null && prodIDs.Trim().Length > 0)
            //{
            //    prodID = JsonConvert.DeserializeObject<List<String>>(prodIDs);
            //}            
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                ProductManagementModel model = new ProductManagementModel();
                value = model.UpdateProduct(prodIDs, ColumnName, ctx);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateSupplier(int Supplier, List<PriceInfo> ColumnName)
        {
            string value = "";
            List<String> prodID = new List<String>();
            //if (prodIDs != null && prodIDs.Trim().Length > 0)
            //{
            //    prodID = JsonConvert.DeserializeObject<List<String>>(prodIDs);
            //}            
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                ProductManagementModel model = new ProductManagementModel();
                value = model.UpdateSupplier(Supplier, ColumnName, ctx);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveConversion(Int32 id, Int32 Conv_ID, Decimal Mul, Decimal Div, Int32 UOM, Int32 UomTo, String UOMUPC)
        {
            string value = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                ProductManagementModel model = new ProductManagementModel();
                value = model.SaveConversion(id, Conv_ID, Mul, Div, UOM, UomTo, UOMUPC, ctx);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult saveInventoryCount(string ColumnName)
        {
            string value = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                ProductManagementModel model = new ProductManagementModel();
                value = model.saveInventoryCount(ColumnName, ctx);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveInventory(List<Int32> prodIDs, int Count_ID, List<PriceInfo> ColumnName)
        {
            string value = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                ProductManagementModel model = new ProductManagementModel();
                value = model.saveInventory(prodIDs, Count_ID, ColumnName, ctx);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult updateVarient(Int32 id, String UpcCode)
        {
            string value = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                ProductManagementModel model = new ProductManagementModel();
                value = model.updateVarient(id, UpcCode, ctx);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult updateInventory(List<PriceInfo> ColumnName)
        {
            string value = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                ProductManagementModel model = new ProductManagementModel();
                value = model.updateInventory(ColumnName, ctx);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult deleteInventory(List<PriceInfo> ColumnName)
        {
            string value = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                ProductManagementModel model = new ProductManagementModel();
                value = model.deleteInventory(ColumnName, ctx);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Save(Int32 id, Int32 Org, String Name, String Value, String ProductType, Int32 attrSet, Int32 taxcat, Int32 prodCat, Int32 UOM, String UPC, int Parent_ID)
        {
            ProdInfo value = null;
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                ProductManagementModel model = new ProductManagementModel();
                value = model.save(id, Org, Name, Value, ProductType, attrSet, taxcat, prodCat, UOM, UPC, Parent_ID, ctx);
            }
            return Json(JsonConvert.SerializeObject(value), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AttributeSetListing(int AttributeSet_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            ProductManagementModel obj = new ProductManagementModel();
            var value = obj.CreateTree(AttributeSet_ID, ctx);
            return Json(JsonConvert.SerializeObject(value), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveAttribute(int id, int AttributeSet, List<Int32> ColumnName, Dictionary<string, string> ColumnValue)
        {
            string value = "";
            List<String> prodID = new List<String>();
            //if (prodIDs != null && prodIDs.Trim().Length > 0)
            //{
            //    prodID = JsonConvert.DeserializeObject<List<String>>(prodIDs);
            //}            
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                ProductManagementModel model = new ProductManagementModel();
                value = model.saveAttribute(id, AttributeSet, ColumnName, ColumnValue, ctx);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult SaveImage(HttpPostedFileBase file, bool isDatabaseSave, string ad_image_id)
        {
            if (file == null)
            {
                Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }

            ProductManagementModel obj = new ProductManagementModel();
            var value = 0;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                if (!Directory.Exists(Path.Combine(Server.MapPath("~/Images"))))
                {
                    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Images")));
                }
                value = obj.SaveImage(ctx, Server.MapPath("~/Images"), file, Convert.ToInt32(ad_image_id), isDatabaseSave);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        /********************/

        // GET: /VA005/AddProductImage/
        [HttpPost]
        public JsonResult SaveImages(HttpPostedFileBase files, string folderKey)
        {
            if (isCancle)
            {
                var jsonResult1 = Json(JsonConvert.SerializeObject("NotUploaded"), JsonRequestBehavior.AllowGet);
                jsonResult1.MaxJsonLength = int.MaxValue;
                return jsonResult1;
            }
            if (files != null)
            {
                string folderName = "ProductImages";
                HttpPostedFileBase hpf = files as HttpPostedFileBase;
                string storedfileName = Path.GetFileName(hpf.FileName);
                string fileExtension = Path.GetExtension(hpf.FileName);
                string fileName = Path.GetFileNameWithoutExtension(hpf.FileName);
                if (!Directory.Exists(HostingEnvironment.MapPath(@"~/TempFiles")))
                {
                    Directory.CreateDirectory(HostingEnvironment.MapPath(@"~/TempFiles"));
                }
                if (!Directory.Exists(Path.Combine(HostingEnvironment.MapPath(@"~/TempFiles"), folderName)))
                {
                    Directory.CreateDirectory(Path.Combine(HostingEnvironment.MapPath(@"~/TempFiles"), folderName));
                }
                string savedFileName = Path.Combine(HostingEnvironment.MapPath(@"~/TempFiles/" + folderName + ""), fileName + "_" + folderKey + fileExtension);
                hpf.SaveAs(savedFileName);
                MemoryStream ms = new MemoryStream();

                try
                {
                    hpf.InputStream.CopyTo(ms);
                    byte[] byteArray = ms.ToArray();

                    if (!Directory.Exists(Path.Combine(Server.MapPath("~/TempDownload"), folderKey)))
                    {
                        Directory.CreateDirectory(Path.Combine(Server.MapPath("~/TempDownload"), folderKey));
                    }

                    using (FileStream fs = new FileStream(Path.Combine(HostingEnvironment.MapPath(@"~/TempDownload"), folderKey, storedfileName), FileMode.Append, System.IO.FileAccess.Write))
                    {
                        fs.Write(byteArray, 0, byteArray.Length);
                        fs.Close();
                    }
                    Common.ConvertByteArrayToThumbnail(byteArray, fileName + "_" + folderKey + fileExtension);
                }
                catch
                {
                    if (ms != null)
                    {
                        ms.Close();
                    }

                }
                finally
                {
                    if (ms != null)
                    {
                        ms.Close();
                    }

                }
            }
            var jsonResult = Json(JsonConvert.SerializeObject(null), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        /********************/

        public JsonResult GetImages()
        {
            isCancle = false;
            List<ImageProperties> lst = new List<ImageProperties>();
            if (Directory.Exists(HostingEnvironment.MapPath("~/TempFiles/ProductImages/Thumb100x100")))
            {
                DirectoryInfo dir = new DirectoryInfo(HostingEnvironment.MapPath("~/TempFiles/ProductImages/Thumb100x100"));
                List<FileInfo> file = dir.GetFiles().OrderByDescending(f => f.CreationTime).ToList();
                List<ImageDetail> lstImg = new List<ImageDetail>();
                foreach (FileInfo file2 in file)
                {
                    byte[] imageByteData = System.IO.File.ReadAllBytes(file2.FullName);
                    string imageBase64Data = Convert.ToBase64String(imageByteData);
                    string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                    ImageDetail objImgDet = new ImageDetail()
                    {
                        fileExtension = file2.Extension,
                        fileName = file2.Name,
                        fileBase64Data = imageDataURL,
                        filePath = file2.FullName
                    };

                    ImageProperties objImgPro = new ImageProperties()
                    {
                        ImageDetail = objImgDet,
                        isUpload = true
                    };
                    lst.Add(objImgPro);
                }
            }
            var jsonResult = Json(JsonConvert.SerializeObject(lst), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        [HttpPost]
        public JsonResult DeleteProductImage(string[] images)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            Common.DeleteImages(images);
            var jsonResult = Json(JsonConvert.SerializeObject(0), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult SaveProductImage(string imagePath, string imageName, int productID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            byte[] imageByteData = System.IO.File.ReadAllBytes(imagePath);
            var image = Common.SaveUserImage(ctx, imageByteData, imageName, true, productID);
            var jsonResult = Json(JsonConvert.SerializeObject(image), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult Remove(string[] fKey, bool cancle)
        {
            isCancle = cancle;
            if (fKey != null)
            {
                //for (int i = 0; i < fKey.Length; i++)
                //{
                //    string filepath = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "TempFiles/ProductImages", fKey[i]);

                //    System.IO.DirectoryInfo myDirInfo = new DirectoryInfo(filepath);

                //    foreach (FileInfo file in myDirInfo.GetFiles())
                //    {
                //        file.Delete();
                //    }
                //    DirectoryInfo[] subDir = myDirInfo.GetDirectories();
                //    foreach (DirectoryInfo dir in subDir)
                //    {
                //        dir.Delete();
                //    }
                //    Directory.Delete(filepath);
                //}
            }
            return Json(JsonConvert.SerializeObject(null), JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Remove file
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
        [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
        //[AjaxValidateAntiForgeryToken] // validate antiforgery token 
        //[SessionState(SessionStateBehavior.ReadOnly)]
        [HttpGet]
        public JsonResult RemoveFile(string uid)
        {
            var match = lstAttachmentInfo.Where(a => a.FileUID == uid).FirstOrDefault();
            if (match != null)
            {
                lstAttachmentInfo.Remove(match);
            }
            return Json(JsonConvert.SerializeObject(null), JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Save unique id for uploaded files
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
        [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
        //[AjaxValidateAntiForgeryToken] // validate antiforgery token 
        //[SessionState(SessionStateBehavior.ReadOnly)]
        [HttpPost]
        public JsonResult SaveUID(string uid)
        {
            lstAttachmentInfo[lstAttachmentInfo.Count - 1].FileUID = uid;
            return Json(JsonConvert.SerializeObject(null), JsonRequestBehavior.AllowGet);

        }


        //Manish 28/9/2016

        public JsonResult SaveAttributeValueImage(string imagePath, string imageName, int productID, string attributeValue, string attimages_id)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            byte[] imageByteData = System.IO.File.ReadAllBytes(imagePath);
            ProductManagementModel obj = new ProductManagementModel();
            var image = obj.SaveAttributeValueImage(ctx, imageByteData, imageName, true, productID, attributeValue, attimages_id);
            var jsonResult = Json(JsonConvert.SerializeObject(image), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult DeleteAttributeValues(string attValueImagesID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            ProductManagementModel obj = new ProductManagementModel();
            List<VA005_AttributeValueDeletes> lindatatoString1 = JsonConvert.DeserializeObject<List<VA005_AttributeValueDeletes>>(attValueImagesID);
            var image = obj.DeleteAttributeValues(ctx, lindatatoString1);
            return Json(JsonConvert.SerializeObject(image), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAttributeValues(int product_id)
        {
            Ctx ct = Session["ctx"] as Ctx;
            ProductManagementModel model = new ProductManagementModel();
            var res = model.GetAttributeValues(ct, product_id);
            return Json(JsonConvert.SerializeObject(res), JsonRequestBehavior.AllowGet);
        }
        //end

        // Added by Bharat to get Image Url from Product on 28 Feb 2018
        public JsonResult GetImageUrl(string fields)
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.GetImageUrl(fields, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to get Attribute Set from Product on 28 Feb 2018
        public JsonResult GetAttributeSet(string fields)
        {
            ProductManagementModel model = new ProductManagementModel();
            int M_Product_ID = Util.GetValueOfInt(fields);
            MProduct prd = new MProduct(Session["ctx"] as Ctx, M_Product_ID, null);
            return Json(JsonConvert.SerializeObject(prd.GetM_AttributeSet_ID()), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Organization on 28 Feb 2018
        public JsonResult LoadOrganization()
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadOrganization(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Product Type on 28 Feb 2018
        public JsonResult LoadProductType()
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadProductType(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Attribute Set on 28 Feb 2018
        public JsonResult LoadAttributes()
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadAttributes(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Tax Categories on 28 Feb 2018
        public JsonResult LoadTaxCategories()
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadTaxCategories(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Product Categories on 28 Feb 2018
        public JsonResult LoadProductCategories()
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadProductCategories(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load UOM on 28 Feb 2018
        public JsonResult LoadUOM()
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadUOM(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Currency on 05 March 2018
        public JsonResult LoadCurrency()
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadCurrency(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Price List on 05 March 2018
        public JsonResult LoadPriceList()
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadPriceList(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Supplier on 05 March 2018
        public JsonResult LoadSupplier()
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadSupplier(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Cart on 05 March 2018
        public JsonResult LoadCart()
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadCart(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Supplier on 05 March 2018
        public JsonResult LoadQueries()
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadQueries(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Category Data on 05 March 2018
        public JsonResult LoadOnCategorySelect(int ProdCatID)
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadOnCategorySelect(Session["ctx"] as Ctx, ProdCatID)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Related Type on 05 March 2018
        public JsonResult LoadRelatedType()
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadRelatedType(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Window ID on 05 March 2018
        public JsonResult GetWindow_ID(string fields)
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.GetWindow_ID(fields, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Product Details on 05 March 2018
        public JsonResult LoadProductData(int M_Product_ID)
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadProductData(M_Product_ID, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load UOM Conversion Data on 05 March 2018
        public JsonResult LoadUOMConversionData(int C_UOM_Conversion_ID)
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadUOMConversionData(C_UOM_Conversion_ID, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Product Attribute Data on 05 March 2018
        public JsonResult LoadProdAttributeData(int M_Product_ID, int M_AttributeSetInstance_ID, string UPC)
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadProdAttributeData(M_Product_ID, M_AttributeSetInstance_ID, UPC, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Product Data to Cart on 05 March 2018
        public JsonResult LoadProductCartData(int M_Product_ID)
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadProductCartData(M_Product_ID, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load UPC of Product on 05 March 2018
        public JsonResult GetProductUPC(string fields)
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.GetProductUPC(fields, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load UOM Conversion Data on 05 March 2018
        public JsonResult LoadUOMRate(int C_UOM_Conversion_ID)
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadUOMConversionData(C_UOM_Conversion_ID, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Delete UOM Conversion Data on 05 March 2018
        public JsonResult DeleteConversion(int C_UOM_Conversion_ID)
        {
            string sql = "DELETE FROM C_UOM_Conversion WHERE C_UOM_Conversion_ID=" + C_UOM_Conversion_ID;
            int no = DB.ExecuteQuery(sql, null, null);
            return Json(JsonConvert.SerializeObject(no), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load UOM Conversion Data on 05 March 2018
        public JsonResult LoadUomGroup(int M_Product_ID)
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadUomGroup(M_Product_ID, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Varient Data on 05 March 2018
        public JsonResult LoadVarients(int M_Product_ID)
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadVarients(M_Product_ID, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Price List Data on 05 March 2018
        public JsonResult LoadPriceListData(int M_PriceList_ID)
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadPriceListData(M_PriceList_ID, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListPrice(int PriceListVersion_ID, int Product_ID, int UOM, int Attribute)
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.GetListPrice(PriceListVersion_ID, Product_ID, UOM, Attribute)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Releated Data on 05 March 2018
        public JsonResult LoadReleatedData(int M_Product_ID)
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadReleatedData(M_Product_ID, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Cart Data on 05 March 2018
        public JsonResult LoadCartData(int VAICNT_InventoryCount_ID)
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadCartData(VAICNT_InventoryCount_ID, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat to Load Parent Product Data on 06 March 2018
        public JsonResult LoadParentData(int M_Product_ID)
        {
            ProductManagementModel model = new ProductManagementModel();
            return Json(JsonConvert.SerializeObject(model.LoadParentData(Session["ctx"] as Ctx, M_Product_ID)), JsonRequestBehavior.AllowGet);
        }
    }
}