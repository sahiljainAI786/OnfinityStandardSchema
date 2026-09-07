using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace VA027Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            ViennaBase.WebApiConfig.Register(config);
        }
    }
}
