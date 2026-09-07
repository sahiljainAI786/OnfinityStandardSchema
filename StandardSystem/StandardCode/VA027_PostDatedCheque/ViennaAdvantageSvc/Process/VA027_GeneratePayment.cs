using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using ViennaAdvantage.Model;
//using ViennaAdvantageSvc.Model;

namespace ViennaAdvantage.Process
{
    public class VA027_GeneratePayment : SvrProcess
    {
        StringBuilder _sql = new StringBuilder();
        //int _docType = 0;

        protected override string DoIt()
        {
            try
            {
                MVA027PostDatedCheck _pdc = new MVA027PostDatedCheck(GetCtx(),GetRecord_ID(),null);
                int _id=_pdc.GetVA027_PostDatedCheck_ID();
                _sql.Append(@"SELECT C_PAYMENT_ID FROM VA027_POSTDATEDCHECK WHERE VA027_POSTDATEDCHECK_ID=" + _id + "AND AD_Client_ID = " + GetAD_Client_ID());
                int _payID=Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
                if (_payID > 0)
                {
                    return Msg.GetMsg(GetCtx(), "VA027_PaymentAlreadyGenerated");
                }
                string _docStatus = _pdc.GetDocStatus();
                if (_docStatus == "CO")
                {
                    ViennaAdvantage.Process.VA027_GenPayment _genPayment = new ViennaAdvantage.Process.VA027_GenPayment();
                    _genPayment.GenratePayment(GetCtx(), GetRecord_ID(), Get_TrxName());
                    return Msg.GetMsg(GetCtx(), "VA027_PaymentGenerated");
                }
                return Msg.GetMsg(GetCtx(), "VA027_PDCNotCompleted");
            }
            catch (Exception ex)
            {
                log.Severe(ex.ToString());
            }

            return "";
        }


        protected override void Prepare()
        {

        }
    }
}