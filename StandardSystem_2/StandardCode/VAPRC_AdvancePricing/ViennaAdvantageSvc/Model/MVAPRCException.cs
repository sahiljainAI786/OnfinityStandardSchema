using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using ViennaAdvantageSvc.Model;

namespace ViennaAdvantage.Model
{
    class MVAPRCException : X_VAPRC_Exception
    {
         public MVAPRCException(Ctx ctx, int VA009_Batch_ID, Trx trxName)
            : base(ctx, VA009_Batch_ID, trxName)
        {
        }
         public MVAPRCException(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

         protected override bool BeforeSave(bool newRecord)
         {
             // Issue id SI_0418_2 in google sheet standard issues On Exception tab, one field selection  should be mandatory.
             if (
             GetM_Brand_ID() == 0 &&
             GetC_BPartner_ID() == 0 &&
             GetM_Product_Category_ID() == 0 &&
             GetM_Product_ID() == 0 &&
             GetVAPRC_SumLevelProd() == 0)
             {
                 log.SaveError("",Msg.GetMsg(GetCtx(),"VAPRC_ExceptionSaveErr"));
                 return false;

             }

             return base.BeforeSave(newRecord);
         }
    }
}
