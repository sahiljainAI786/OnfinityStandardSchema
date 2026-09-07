using System;
using System.Collections.Generic;
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
using VA005.Models;

namespace VA005.Controllers
{
    public class VA005_TreeController : Controller
    {
        //
        // GET: /VA005/Tree/
        public ActionResult GetTreeAsString( bool editable)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            int AD_Table_ID=MTable.Get_Table_ID("M_Product");
            var sql = "SELECT AD_Tree_ID FROM AD_Tree "
            + "WHERE AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND AD_Table_ID=" + AD_Table_ID + " AND IsActive='Y' AND IsAllNodes='Y' "
                        + "ORDER BY IsDefault DESC, AD_Tree_ID";

            int AD_Tree_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql,null,null));

            List<SetTree> html = new List<SetTree>();
            if (Session["ctx"] != null)
            {
                var m = new VA005_ProductHelper(Session["ctx"] as Ctx);
                var tree = m.GetMenuTree(AD_Tree_ID, editable, true, 0);
                List<SetTree> setttreeobj = new List<SetTree>();
                SetTree trees = new SetTree();
                trees.AD_Tree_ID = AD_Tree_ID;
                setttreeobj.Add(trees);
                html = m.GetMenuTreeUI(trees, tree.GetRootNode(), tree.GetNodeTableName());
                //  html = m.GetMenuTreeUI(tree.GetRootNode(), @Url.Content("~/"), windowNo.ToString(), tree.GetNodeTableName());
                //m.dispose();
            }
            return Json(JsonConvert.SerializeObject(html), JsonRequestBehavior.AllowGet);
        }
    }    
}