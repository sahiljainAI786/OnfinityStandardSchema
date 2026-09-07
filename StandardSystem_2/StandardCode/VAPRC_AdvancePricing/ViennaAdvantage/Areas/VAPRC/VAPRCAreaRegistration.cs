using System.Web.Mvc;
using System.Web.Optimization;



//NOTE:--    Please replace ViennaAdvantage with Module Prefix Code(MPC)..



namespace VAPRC //  Please replace namespace with  Module Prefix Code(MPC)..
{
    public class ViennaAdvantageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "VAPRC";   //Please replace "ViennaAdvantage" with  Module Prefix Code(MPC)..
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "VAPRC_default",
                "VAPRC/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
                , new[] { "VAPRC.Controllers" }
            );    // Please replace ViennaAdvantage with Module Prefix Code(MPC)..


            StyleBundle style = new StyleBundle("~/Areas/VAPRC/Contents/VAPRCStyle");

            /* ==>  Here include all css files in style bundle......see example below....  */

            style.Include("~/Areas/VAPRC/Contents/VAPRC.css");
            //              "~/Areas/ViennaAdvantage/Contents/example2.css");

            ScriptBundle script = new ScriptBundle("~/Areas/VAPRC/Scripts/VAPRCJs");

            //script.Include("~/Areas/VAPRC/Scripts/model/callouts.js");
            //               "~/Areas/ViennaAdvantage/Scripts/example2.js");






            VAdvantage.ModuleBundles.RegisterScriptBundle(script, "VAPRC", 10);
            VAdvantage.ModuleBundles.RegisterStyleBundle(style, "VAPRC", 10);
        }
    }
}