using System.Web.Mvc;
using System.Web.Optimization;



//NOTE:--    Please replace ViennaAdvantage with prefix of your module..



namespace VA005 //  Please replace namespace with prefix of your module..
{
    public class VA005AreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "VA005";   //Please replace "ViennaAdvantage" with prefix of your module.......
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "VA005_default",
                "VA005/{controller}/{action}/{id}",
                new { controller = "ProductManagement", action = "Index", id = UrlParameter.Optional }
                , new[] { "VA005.Controllers" }
            );    // Please replace ViennaAdvantage with prefix of your module...

            ScriptBundle script = new ScriptBundle("~/Areas/VA005/Scripts/VA005Js");
            StyleBundle style = new StyleBundle("~/Areas/VA005/Contents/VA005Style");

            /* ==>  Here include all css files in style bundle......see example below....  */

            //style.Include("~/Areas/VA005/Contents/VA005_productCategory.css",
            //              "~/Areas/VA005/Contents/VA005_style.css",
            //              "~/Areas/VA005/Contents/VA005_ProductMgt.css");

            //style.Include("~/Areas/VA005/Contents/VA005_rtl.css");


            //script.Include("~/Areas/VA005/Scripts/apps/forms/productmgtform.js");
            //script.Include("~/Areas/VA005/Scripts/apps/forms/productcategoryform.js",
            //            "~/Areas/VA005/Scripts/apps/forms/jquery-barcode.js",
            //            "~/Areas/VA005/Scripts/apps/forms/attribute.js",
            //            "~/Areas/VA005/Scripts/apps/forms/attributeSetListing.js",
            //            "~/Areas/VA005/Scripts/apps/forms/uploadimage.js",
            //            "~/Areas/VA005/Scripts/apps/forms/jquery-touch.js",
            //            "~/Areas/VA005/Scripts/apps/forms/StickerProduct.js",
            //            "~/Areas/VA005/Scripts/apps/forms/jquery.popupoverlay.js");


            style.Include("~/Areas/VA005/Contents/VA005.all.min.css");
            script.Include("~/Areas/VA005/Scripts/VA005.all.min.js");

            /*-------------------------------------------------------
              Please replace "ViennaAdvantage" with prefix of your module..
             * 
             * 1. first parameter is script/style bundle...
             * 
             * 2. Second parameter is module prefix...
             * 
             * 3. Third parameter is order of loading... (dafault is 10 )
             * 
             --------------------------------------------------------*/

            VAdvantage.ModuleBundles.RegisterScriptBundle(script, "VA005", 10);
            VAdvantage.ModuleBundles.RegisterStyleBundle(style, "VA005", 10);
        }
    }
}