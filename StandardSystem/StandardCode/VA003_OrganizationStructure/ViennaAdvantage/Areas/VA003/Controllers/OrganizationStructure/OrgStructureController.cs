/********************************************************
 * Module Name    : VA003
 * Purpose        : Create Organization Structure
 * Class Used     : 
 * Chronological Development
 * Karan         2 July 2015
 ******************************************************/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Filters;
using VIS.Models;

namespace VIS.Controllers
{

    public class OrgStructureController : Controller
    {
        //
        // GET: /VIS/OrgStructure/
        [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
        [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
        //[AjaxValidateAntiForgeryToken] // validate antiforgery token 
        public ActionResult Index()
        {
            return View();
        }

        [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
        [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
        public ActionResult GetTree(int windowNo, bool showOrgUnits)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            return Json(JsonConvert.SerializeObject(orgStrct.GetTree(windowNo, @Url.Content("~/"), "", showOrgUnits)), JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
        [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
        public ActionResult GetInitSettings()
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            return Content(orgStrct.GetInitSettings());
        }

        [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
        [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
        public ActionResult GetOrgInfo(int orgID, bool loadLookups)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            return Json(JsonConvert.SerializeObject(orgStrct.GetOrgInfo(orgID, loadLookups)), JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
        [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
        public ActionResult InsertOrUpdateOrg(OrgStructureData data)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            data.Name = Server.HtmlDecode(data.Name).Replace("'", "''");

            if (!String.IsNullOrEmpty(data.SearchKey))
            {
                data.SearchKey = Server.HtmlDecode(data.SearchKey).Replace("'", "''");
            }

            if (!String.IsNullOrEmpty(data.TaxID))
            {
                data.TaxID = Server.HtmlDecode(data.TaxID).Replace("'", "''");
            }

            if (!String.IsNullOrEmpty(data.EmailAddess))
            {
                data.EmailAddess = Server.HtmlDecode(data.EmailAddess).Replace("'", "''");
            }

            if (!String.IsNullOrEmpty(data.Phone))
            {
                data.Phone = Server.HtmlDecode(data.Phone).Replace("'", "''");
            }

            if (!String.IsNullOrEmpty(data.Fax))
            {
                data.Fax = Server.HtmlDecode(data.Fax).Replace("'", "''");
            }

            return Json(JsonConvert.SerializeObject(orgStrct.InsertOrUpdateOrg(data)), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadPic(HttpPostedFileBase pic, int orgID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            MemoryStream ms = new MemoryStream();
            pic.InputStream.CopyTo(ms);
            return Json(JsonConvert.SerializeObject(orgStrct.UploadPic(ms.ToArray(), orgID)), JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
        [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
        public ActionResult LoadLookups()
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            return Json(JsonConvert.SerializeObject(orgStrct.LoadLookups()), JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
        [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
        public ActionResult GenerateHeirarchy(int windowNo)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            return Json(JsonConvert.SerializeObject(orgStrct.GenerateHierarchy(@Url.Content("~/"), windowNo)), JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
        [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
        public ActionResult CreateTree(int windowNo, int treeID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            return Json(JsonConvert.SerializeObject(orgStrct.CreateTree1(treeID, @Url.Content("~/"), windowNo)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Create Tree Structure for selected Tree
        /// </summary>
        /// <param name="windowNo">Window no</param>
        /// <param name="treeID">Tree ID</param>
        /// <param name="LegalEntityIds">LegalEntity IDs</param>
        /// <returns>TreeData</returns>
        /// <writer>VIS_0045</writer>
        [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
        [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
        public ActionResult FRPTCreateTree(int windowNo, int treeID, string LegalEntityIds)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            return Json(JsonConvert.SerializeObject(orgStrct.CreateTree1(treeID, @Url.Content("~/"), windowNo, LegalEntityIds)), JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
        [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
        public ActionResult AddOrgNode(int treeID, string name, string description, string value, int windowNo, string parentID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            name = Server.HtmlDecode(name).Replace("'", "''");
            if (!string.IsNullOrEmpty(value))
            {
                value = Server.HtmlDecode(value).Replace("'", "''");
            }


            return Json(JsonConvert.SerializeObject(orgStrct.AddOrgNode(treeID, name, description, value, windowNo, @Url.Content("~/"), parentID)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Add New Tree
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="IsOrgUnit">Tree for Org Unit</param>
        /// <returns></returns>
        [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
        [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
        public ActionResult AddNewTree(string name, bool IsOrgUnit)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            name = Server.HtmlDecode(name);

            return Json(JsonConvert.SerializeObject(orgStrct.AddNewTree(name, IsOrgUnit)), JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
        [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
        public ActionResult RefreshOrgType()
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            return Json(JsonConvert.SerializeObject(orgStrct.RefreshOrgType()), JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
        [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
        /// <summary>
        /// This Method is used to Get Sequence for Tree
        /// </summary>
        /// <param name="TreeID"> Id of the current tree</param>
        /// <returns>Seuence for the selected tree</returns>
        public ActionResult LoadSequenceforTree(int TreeID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            return Json(JsonConvert.SerializeObject(orgStrct.GetSequenceforTree(TreeID)), JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
        [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
        /// <summary>
        ///This Method is used to Get Sequence for Tree
        /// </summary>
        /// <param name="AD_ORg_ID"> orgnization Id</param>
        /// <returns>Updated logo</returns>
        public ActionResult UpdateLogo(int AD_ORg_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            return Json(JsonConvert.SerializeObject(orgStrct.UpdateLogo(AD_ORg_ID)), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This Method is used to zoom specific window
        /// </summary>
        /// <returns>Zoom window Id</returns>
        public ActionResult ZoomToWindow()
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            return Json(JsonConvert.SerializeObject(orgStrct.ZoomToWindow()), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This Method is used to Insert new node into the tree
        /// <param name="Chidrens">Ids of childs</param>
        /// <param name="IsActive">IsActive</param>
        /// <param name="ParentID">Id of the Parent Node</param>
        /// <param name="TreeIds">Id of the trees</param>
        /// <param name="ChildCount">count of childs</param>
        /// </summary>
        /// <returns></returns>
        public ActionResult InsertTreeNode(string[] Chidrens, string[] IsActive, int ParentID, int TreeIds, int ChildCount)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            return Json(JsonConvert.SerializeObject(orgStrct.InsertTreeNode(Chidrens, IsActive, ParentID, TreeIds, ChildCount)), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This Method is used to Update Sequence of the tree
        /// <param name="OldID">Old id before change</param>
        /// <param name="NewId">Changed new id</param>
        /// <param name="TreeId">Id of the current tree</param>
        /// <param name="OldSibling">Old Siblings (Before Change)</param>
        /// <param name="NodId">Node Id</param>
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateSequence(int OldID, int NewId, int TreeId, string[] OldSibling, string[] NodId, string TableName)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            return Json(JsonConvert.SerializeObject(orgStrct.UpdateSeuenceOfNode(OldID, NewId, TreeId, OldSibling, NodId, TableName)), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///This Method is used to Update Parent Node of the tree 
        /// <param name="TreeId">Id of the tree</param>
        /// <param name="CurrentNode">Current selected Node</param>
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateParentNode(int TreeId, int CurrentNode, int NewIdForOrg, bool IsSummery)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            return Json(JsonConvert.SerializeObject(orgStrct.UpdateParentNode(TreeId, CurrentNode, NewIdForOrg, IsSummery)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///This Method is used to Update Parent orgnization information of the tree
        /// <param name="Name">Name of the Org</param>
        /// <param name="AD_Org_ID">Id of the Org</param>
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateOrgnization(string Name, int AD_Org_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            return Json(JsonConvert.SerializeObject(orgStrct.UpdateOrgnization(Name, AD_Org_ID)), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This Method is used to Delete Node of the tree
        /// <param name="TreeId">Name of the tree</param>
        /// <param name="NodeId">Id of the tree</param>
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteAD_TreeNode(int TreeId, int Parent_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            OrgStructure orgStrct = new OrgStructure(ctx);
            return Json(JsonConvert.SerializeObject(orgStrct.DeleteAD_TreeNode(TreeId, Parent_ID)), JsonRequestBehavior.AllowGet);
        }

    }
}