using System.Web.Mvc;
using System.Web.Optimization;



//NOTE:--    Please replace VA027 with prefix of your module..



namespace VA027 //  Please replace namespace with prefix of your module..
{
    public class VA027AreaRegistration : AreaRegistration
    {
        public override string AreaName 
        {
            get
            {
                return "VA027";   //Please replace "VA027" with prefix of your module.......
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "VA027_default",
                "VA027/{controller}/{action}/{id}",
                new { controller = "StyleManagement", action = "Index", id = UrlParameter.Optional }
                , new[] { "VA027.Controllers" }
            );    // Please replace VA027 with prefix of your module...


            StyleBundle style = new StyleBundle("~/Areas/VA027/Contents/VA027Style");
            ScriptBundle script = new ScriptBundle("~/Areas/VA027/Scripts/VA027Js");

            //style.Include("~/Areas/VA027/Contents/VA027_View.css");


            //script.Include("~/Areas/VA027/Scripts/model/Callouts.js");


            style.Include("~/Areas/VA027/Contents/VA027.all.min.css");
            script.Include("~/Areas/VA027/Scripts/VA027.all.min.js");
            /*-------------------------------------------------------
              Please replace "VA027" with prefix of your module..
             * 
             * 1. first parameter is script/style bundle...
             * 
             * 2. Second parameter is module prefix...
             * 
             * 3. Third parameter is order of loading... (dafault is 10 )
             * 
             --------------------------------------------------------*/

            VAdvantage.ModuleBundles.RegisterScriptBundle(script, "VA027", 20);
            VAdvantage.ModuleBundles.RegisterStyleBundle(style, "VA027", 20);
        }
    }
}