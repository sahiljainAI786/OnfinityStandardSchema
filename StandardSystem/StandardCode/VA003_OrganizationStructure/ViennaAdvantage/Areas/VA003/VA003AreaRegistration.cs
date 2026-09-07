using System.Web.Mvc;
using System.Web.Optimization;

namespace ViennaAdvantage.Areas.VA003
{
    public class VA003AreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "VA003";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "VA003_default",
                "VA003/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );



            StyleBundle style = new StyleBundle("~/Areas/VA003/Contents/VA003Style");

            /* ==>  Here include all css files in style bundle......see example below....  */

            //style.Include("~/Areas/VA003/Contents/OrgStructure.css");


            ScriptBundle script = new ScriptBundle("~/Areas/VA003/Scripts/VA003Js");

            /*-------------------------------------------------------
                    Here include all js files in style bundle......see example below....
            //     --------------------------------------------------------*/
            //script.Include("~/Areas/VA003/Scripts/apps/orgstructure/orgstructure.js",
            //    "~/Areas/VA003/Scripts/apps/orgstructure/addnode.js",
            //    "~/Areas/VA003/Scripts/apps/orgstructure/info.js"
            //);

            script.Include("~/Areas/VA003/Scripts/apps/orgstructure/orgstructureall.min.js");
            style.Include("~/Areas/VA003/Contents/OrgStructureall.min.css");

            VAdvantage.ModuleBundles.RegisterScriptBundle(script, "VA003", 10);
            VAdvantage.ModuleBundles.RegisterStyleBundle(style, "VA003", 10);
        }
    }
}