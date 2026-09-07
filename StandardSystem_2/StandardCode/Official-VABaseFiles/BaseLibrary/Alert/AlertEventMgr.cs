/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : AlertEventMgr
 * Purpose        : 
 * Chronological    Development
 * ruby            17 Sep 2025
  ******************************************************/

using System.Collections.Generic;
using VAdvantage.Model;


namespace VAdvantage.Alert
{
    public interface AlertEventMgr
    {

        /// <summary>
        ///Process alert event Value 
        /// </summary>
        /// <param name="document">document</param>
        /// <param name="AD_Table_ID">table</param>
        /// <returns> true if WF started</returns>
        bool Process(PO document, POInfo pinfo,string eventType);
    }
}
