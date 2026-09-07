using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Utility;
using ViennaAdvantage.Model;
using VAdvantage.Model;
using VA005.Models;


namespace VA005.Controllers
{
    public class AttributeController : Controller
    {
        //
        // GET: /VA005/Attribute/
        public ActionResult Index(int windowNo)
        {
            ViewBag.WindowNumber = windowNo;
            return View();
        }

        public JsonResult SaveAttributemodel(VA005_SaveAttribute value)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            Attribute obj = new Attribute(ctx);

            value.name = Server.HtmlDecode(value.name);
            value.description = Server.HtmlDecode(value.description);

            var text = obj.SaveAttribute(value);
            return Json(JsonConvert.SerializeObject(text), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectionSavemodel(VA005_SaveSelectionList value)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            Attribute obj = new Attribute(ctx);

            value.secname = Server.HtmlDecode(value.secname);
            value.searchkey = Server.HtmlDecode(value.searchkey);

            var text = obj.SelectionSave(value);         
            return Json(JsonConvert.SerializeObject(text), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowGrideData(VA005_ShowGrideData value)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            Attribute obj = new Attribute(ctx);
            var text = obj.ShowGrideData(value);
            return Json(new { result =text}, JsonRequestBehavior.AllowGet);           
        }
//=================================================
        //public JsonResult SaveAttributeuses(Attribute.VA005_SaveAttributeuses value)
        //{
        //    Ctx ctx = Session["ctx"] as Ctx;
        //    Attribute obj = new Attribute(ctx);
        //    var text = obj.SaveAttributeuses(value);
        //    return Json(JsonConvert.SerializeObject(text), JsonRequestBehavior.AllowGet);
        //}

        // Added by Shifali on 04 Aug 2020 to Save multiple attributes
        /// <summary>
        /// Save Attribute Uses
        /// </summary>
        /// <param name="attributsetid">attributesetid</param>
        /// <param name="nid">node id</param>
        /// <returns>Result</returns>
        public JsonResult SaveAttributeuses(string attributsetid,int nid)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            Attribute obj = new Attribute(ctx);
            var text = obj.SaveAttributeuses(ctx, attributsetid,nid);
            return Json(JsonConvert.SerializeObject(text), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAttributeValue(VA005_DeleteAttributeValue value)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            Attribute obj = new Attribute(ctx);
            var text = obj.DeleteAttributeValue(value);
            return Json(JsonConvert.SerializeObject(text), JsonRequestBehavior.AllowGet);
           // return Json(new { text }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Attribute Count
        /// </summary>
        /// <param name="SelectedAttributeID">SelectedAttributeID</param>
        /// <returns>JSON Data</returns>
        //public JsonResult GetAttributeCount(int SelectedAttributeID)
        //{
        //    Ctx ctx = Session["ctx"] as Ctx;
        //    Attribute obj = new Attribute(ctx);
        //    int value = obj.GetAttributeCount(SelectedAttributeID);
        //    return Json(new { value }, JsonRequestBehavior.AllowGet);
        //}
        /// <summary>
        /// Load Select
        /// </summary>
        /// <returns>JSON Data</returns>
        public JsonResult LoadSelectAttribute()
        {
            Attribute model = new Attribute(Session["Ctx"] as Ctx);
            List<ValueNamePair> value = model.LoadSelectAttribute();
            return Json(JsonConvert.SerializeObject(value), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Load Attribute
        /// </summary>
        /// <param name="ID">ID</param>
        /// <returns>JSON Data</returns>
        public JsonResult LoadAttribute(int ID)
        {
            Attribute model = new Attribute(Session["Ctx"] as Ctx);
            List<AttributeDatas> value = model.LoadAttribute(ID);
            return Json(JsonConvert.SerializeObject(value), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Field Length
        /// </summary>
        /// <param name="TableID">Table ID</param>
        /// <param name="COLUMNNAME">Column Name</param>
        /// <returns>JSON Data</returns>
        public JsonResult GetFieldLength(int TableID, string COLUMNNAME)
        {
            Attribute model = new Attribute(Session["Ctx"] as Ctx);
            List<ColumnData> value = model.GetFieldLength(TableID, COLUMNNAME);
            return Json(JsonConvert.SerializeObject(value), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get Field
        /// </summary>
        /// <param name="TableID">TableID</param>
        /// <param name="COLUMNNAME">COLUMNNAME</param>
        /// <returns>JSON Data</returns>
        public JsonResult GetField(int TableID, string COLUMNNAME)
        {
            Attribute model = new Attribute(Session["Ctx"] as Ctx);
            List<ColumnData> value = model.GetField(TableID, COLUMNNAME);
            return Json(JsonConvert.SerializeObject(value), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Update 
        /// </summary>
        /// <param name="fields">fields</param>
        /// <returns>JSON Data</returns>
        public JsonResult Update(string fields)
        {
            Attribute model = new Attribute(Session["Ctx"] as Ctx);
            var value = model.Update(fields);
            return Json(JsonConvert.SerializeObject(value), JsonRequestBehavior.AllowGet);
        }
    }
}