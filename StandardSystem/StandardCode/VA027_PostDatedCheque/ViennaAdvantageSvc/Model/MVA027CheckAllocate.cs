using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace ViennaAdvantage.Model
{
    class MVA027CheckAllocate : X_VA027_CheckAllocate
    {

        public MVA027CheckAllocate(Ctx ctx, int VA027_CheckAllocate_ID, Trx trxName)
           : base(ctx, VA027_CheckAllocate_ID, trxName)
        {
        }

        public MVA027CheckAllocate(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// during saving a new record, system will check same invoice schedule reference exist or not
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns>true/false</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            string sql;
            if (Get_ColumnIndex("C_InvoicePaySchedule_ID") >= 0 && GetC_InvoicePaySchedule_ID() > 0 && Is_ValueChanged("C_InvoicePaySchedule_ID"))
            {
                //check if records exist for same invoice pay schedule
                sql = "SELECT COUNT(iii.VA027_CheckAllocate_ID) FROM  VA027_Postdatedcheck i INNER JOIN VA027_ChequeDetails ii" +
                  " ON i.VA027_PostDatedCheck_ID = ii.VA027_PostDatedCheck_ID INNER JOIN VA027_CheckAllocate iii" +
                  " ON ii.VA027_ChequeDetails_id = iii.VA027_ChequeDetails_ID WHERE i.DocStatus NOT IN('RE', 'VO') AND iii.C_InvoicePaySchedule_ID = " + GetC_InvoicePaySchedule_ID();

                if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName())) > 0)
                {
                    log.SaveError("", Msg.GetMsg(GetCtx(), "VIS_NotSaveDuplicateRecord"));
                    return false;
                }
            }
            //get Currency Precision
            sql = "SELECT StdPrecision FROM  VA027_PostDatedCheck i INNER JOIN C_Currency c ON i.C_Currency_ID = c.C_Currency_ID " +
                  "WHERE VA027_PostDatedCheck_ID = (SELECT VA027_PostDatedCheck_ID FROM VA027_ChequeDetails " +
                  "WHERE VA027_ChequeDetails_ID = " + GetVA027_ChequeDetails_ID() + ")";

            int precesion = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));

            // set amount on the basis of precision
            SetAmount(Util.GetValueOfDecimal(Decimal.Round(GetAmount(), precesion)));
            SetInvoiceAmt(Util.GetValueOfDecimal(Decimal.Round(GetInvoiceAmt(), precesion)));
            SetWriteOffAmt(Util.GetValueOfDecimal(Decimal.Round(GetWriteOffAmt(), precesion)));
            SetDiscountAmt(Util.GetValueOfDecimal(Decimal.Round(GetDiscountAmt(), precesion)));

            //Amount+ write of+ discount should be equal to Invoice amount
            if (GetAmount() + GetDiscountAmt() + GetWriteOffAmt() != GetInvoiceAmt())
            {
                log.SaveError("", Msg.GetMsg(GetCtx(), "VA027_SumofAmountsEqualsInvoiceAmt"));
                return false;
            }

            return true;
        }

        /// <summary>
        /// update Amount, converted Amount , writeoff and discount amount on Cheque details and postdatedcheck with the sum of 
        /// checkallocate and chequedeatils amount respectively
        /// </summary>
        /// <param name="newRecord"></param>
        /// <param name="success"></param>
        /// <returns>true</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            int stdprecision = 0;
            int C_Tax_ID = 0;
            Decimal surchargeAmt = Env.ZERO;
            Decimal TaxAmt = Env.ZERO;
            Decimal PayAmt = Env.ZERO;

            string sql = "UPDATE VA027_ChequeDetails i "
             + "  SET VA027_ChequeAmount ="
             + "   (SELECT COALESCE(SUM(Amount), 0) FROM VA027_checkallocate il WHERE i.VA027_ChequeDetails_ID = il.VA027_ChequeDetails_ID), VA027_IsCheckAllocated='Y'"
             + " WHERE VA027_ChequeDetails_ID =" + GetVA027_ChequeDetails_ID() /*+ "And AD_Org_ID=" + GetAD_Org_ID() + "AND AD_Client_ID=" + GetAD_Client_ID()*/;

            int count = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_TrxName()));

            if (count > 0)
            {
                sql = "UPDATE va027_postdatedcheck SET VA027_PayAmt =" +
        " (SELECT COALESCE(SUM(il.VA027_ChequeAmount),0) FROM va027_postdatedcheck i INNER JOIN VA027_ChequeDetails il" +
        " ON i.va027_postdatedcheck_ID = il.va027_postdatedcheck_ID WHERE il.va027_postdatedcheck_ID =" +
        "(SELECT va027_postdatedcheck_ID FROM VA027_ChequeDetails WHERE VA027_ChequeDetails_id = " + GetVA027_ChequeDetails_ID() + ")" +
       " AND i.AD_Org_ID = " + GetAD_Org_ID() + " AND i.AD_Client_ID = " + GetAD_Client_ID() + ")," +
        " VA027_ConvertedAmount = (SELECT COALESCE(SUM(il.VA027_ChequeAmount),0) FROM va027_postdatedcheck i INNER JOIN VA027_ChequeDetails il" +
          " ON i.va027_postdatedcheck_ID = il.va027_postdatedcheck_ID WHERE il.va027_postdatedcheck_ID =" +
         " (SELECT va027_postdatedcheck_ID FROM VA027_ChequeDetails WHERE VA027_ChequeDetails_id =" + GetVA027_ChequeDetails_ID() + ")" +
      "  AND i.AD_Org_ID = " + GetAD_Org_ID() + "  AND i.AD_Client_ID = " + GetAD_Client_ID() + ")," +
      " va027_writeoffAmt =(SELECT COALESCE(SUM(Writeoffamt),0) FROM Va027_checkAllocate i " +
      "INNER JOIN va027_chequedetails ii ON i.va027_chequedetails_id = ii.va027_chequedetails_id " +
      "INNER JOIN va027_postdatedCheck iii ON ii.va027_postdatedcheck_ID = iii.va027_postdatedcheck_ID " +
     " WHERE iii.va027_postdatedcheck_ID = (SELECT va027_postdatedcheck_ID FROM VA027_ChequeDetails WHERE VA027_ChequeDetails_id =" + GetVA027_ChequeDetails_ID() + ")" +
     " AND i.AD_Org_ID = " + GetAD_Org_ID() + "  AND i.AD_Client_ID = " + GetAD_Client_ID() + ")," +
      " VA027_discountAmt = (SELECT COALESCE(SUM(discountAmt),0) FROM Va027_checkAllocate i " +
     " INNER JOIN va027_chequedetails ii ON i.va027_chequedetails_id = ii.va027_chequedetails_id " +
     " INNER JOIN va027_postdatedCheck iii ON ii.va027_postdatedcheck_ID = iii.va027_postdatedcheck_ID " +
     " WHERE iii.va027_postdatedcheck_ID = (SELECT va027_postdatedcheck_ID FROM VA027_ChequeDetails WHERE VA027_ChequeDetails_id =" + GetVA027_ChequeDetails_ID() + ")" +
     " AND i.AD_Org_ID = " + GetAD_Org_ID() + "  AND i.AD_Client_ID = " + GetAD_Client_ID() + ") " +
      "  WHERE va027_postdatedcheck_ID =" +
        "  (SELECT va027_postdatedcheck_ID FROM VA027_ChequeDetails WHERE VA027_ChequeDetails_id =" + GetVA027_ChequeDetails_ID() +
          ") And AD_Org_ID = " + GetAD_Org_ID() + " AND AD_Client_ID = " + GetAD_Client_ID();

                DB.ExecuteQuery(sql, null, Get_TrxName());
            }

            sql = "SELECT StdPrecision, C_Tax_ID, VA027_PayAmt FROM VA027_PostDatedCheck i INNER JOIN C_Currency c ON i.C_Currency_ID = c.C_Currency_ID " +
                 "WHERE VA027_PostDatedCheck_ID = (SELECT VA027_PostDatedCheck_ID FROM VA027_ChequeDetails " +
                 "WHERE VA027_ChequeDetails_ID = " + GetVA027_ChequeDetails_ID() + ") AND i.C_Charge_ID > 0 AND i.C_Tax_ID > 0 ";

            DataSet ds = new DataSet();
            ds = DB.ExecuteDataset(sql, null, Get_Trx());
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                stdprecision = Util.GetValueOfInt(ds.Tables[0].Rows[0]["StdPrecision"]);
                C_Tax_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_Tax_ID"]);
                PayAmt = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["VA027_PayAmt"]);

                MTax tax = new MTax(GetCtx(), C_Tax_ID, null);

                if (tax.Get_ColumnIndex("Surcharge_Tax_ID") > 0 && tax.GetSurcharge_Tax_ID() > 0)
                {
                    TaxAmt = tax.CalculateSurcharge(PayAmt, true, stdprecision, out surchargeAmt);
                }
                else
                {
                    TaxAmt = tax.CalculateTax(PayAmt, true, stdprecision);
                }
                
                sql = "UPDATE VA027_PostDatedCheck SET TaxAmount= " + TaxAmt + " , SurchargeAmt= " + surchargeAmt +
                " WHERE VA027_PostDatedCheck_ID = (SELECT VA027_PostDatedCheck_ID FROM VA027_ChequeDetails WHERE VA027_ChequeDetails_ID =" + GetVA027_ChequeDetails_ID() + " )";
                
                DB.ExecuteQuery(sql, null, Get_TrxName());
            }
            return true;
        }

        /// <summary>
        ///  update Amount on Cheque details and postdatedcheck with the sum of 
        /// checkallocate and chequedeatils amount respectively
        /// </summary>
        /// <param name="success"></param>
        /// <returns>true</returns>
        protected override bool AfterDelete(bool success)
        {
            int stdprecision = 0;
            int C_Tax_ID = 0;
            Decimal surchargeAmt = Env.ZERO;
            Decimal TaxAmt = Env.ZERO;
            Decimal PayAmt = Env.ZERO;
            string sql = null;
            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VA027_CheckAllocate_ID) FROM VA027_CheckAllocate WHERE VA027_ChequeDetails_ID = " + GetVA027_ChequeDetails_ID(), null, Get_TrxName())) > 0)
            {
                sql = "UPDATE VA027_ChequeDetails i"
                + "  SET VA027_ChequeAmount ="
                + "   (SELECT COALESCE(SUM(Amount), 0) FROM VA027_checkallocate il WHERE i.VA027_ChequeDetails_ID = il.VA027_ChequeDetails_ID)"
                + " WHERE VA027_ChequeDetails_ID =" + GetVA027_ChequeDetails_ID() /*+ "And AD_Org_ID=" + GetAD_Org_ID() + "AND AD_Client_ID=" + GetAD_Client_ID()*/;
            }
            else
            {
                sql = "UPDATE VA027_ChequeDetails i "
            + "  SET VA027_ChequeAmount ="
            + "   (SELECT COALESCE(SUM(Amount), 0) FROM VA027_checkallocate il WHERE i.VA027_ChequeDetails_ID = il.VA027_ChequeDetails_ID), VA027_IsCheckAllocated='N'"
            + " WHERE VA027_ChequeDetails_ID =" + GetVA027_ChequeDetails_ID();
            }
            int count = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_TrxName()));

            if (count > 0)
            {
               sql = "UPDATE va027_postdatedcheck SET VA027_PayAmt =" +
        " (SELECT COALESCE(SUM(il.VA027_ChequeAmount),0) FROM va027_postdatedcheck i INNER JOIN VA027_ChequeDetails il" +
        " ON i.va027_postdatedcheck_ID = il.va027_postdatedcheck_ID WHERE il.va027_postdatedcheck_ID =" +
        "(SELECT va027_postdatedcheck_ID FROM VA027_ChequeDetails WHERE VA027_ChequeDetails_id = " + GetVA027_ChequeDetails_ID() + ")" +
       " AND i.AD_Org_ID = " + GetAD_Org_ID() + " AND i.AD_Client_ID = " + GetAD_Client_ID() + ")," +
        " VA027_ConvertedAmount = (SELECT COALESCE(SUM(il.VA027_ChequeAmount),0) FROM va027_postdatedcheck i INNER JOIN VA027_ChequeDetails il" +
          " ON i.VA027_postdatedcheck_ID = il.va027_postdatedcheck_ID WHERE il.va027_postdatedcheck_ID =" +
         " (SELECT VA027_postdatedcheck_ID FROM VA027_ChequeDetails WHERE VA027_ChequeDetails_id =" + GetVA027_ChequeDetails_ID() + ")" +
      "  AND i.AD_Org_ID = " + GetAD_Org_ID() + "  AND i.AD_Client_ID = " + GetAD_Client_ID() + ")," +
      " VA027_WriteOffAmt =(SELECT COALESCE(SUM(Writeoffamt),0) FROM Va027_checkAllocate i " +
      "INNER JOIN va027_chequedetails ii ON i.va027_chequedetails_id = ii.va027_chequedetails_id " +
      "INNER JOIN va027_postdatedCheck iii ON ii.va027_postdatedcheck_ID = iii.va027_postdatedcheck_ID " +
     " WHERE iii.va027_postdatedcheck_ID = (SELECT va027_postdatedcheck_ID FROM VA027_ChequeDetails WHERE VA027_ChequeDetails_id =" + GetVA027_ChequeDetails_ID() + ")" +
     " AND i.AD_Org_ID = " + GetAD_Org_ID() + "  AND i.AD_Client_ID = " + GetAD_Client_ID() + ")," +
      " VA027_discountAmt = (SELECT COALESCE(SUM(discountAmt),0) FROM Va027_checkAllocate i " +
     " INNER JOIN va027_chequedetails ii ON i.va027_chequedetails_id = ii.va027_chequedetails_id " +
     " INNER JOIN va027_postdatedCheck iii ON ii.va027_postdatedcheck_ID = iii.va027_postdatedcheck_ID " +
     " WHERE iii.va027_postdatedcheck_ID = (SELECT va027_postdatedcheck_ID FROM VA027_ChequeDetails WHERE VA027_ChequeDetails_id =" + GetVA027_ChequeDetails_ID() + ")" +
     " AND i.AD_Org_ID = " + GetAD_Org_ID() + "  AND i.AD_Client_ID = " + GetAD_Client_ID() + ") " +
      "  WHERE va027_postdatedcheck_ID =" +
        "  (SELECT va027_postdatedcheck_ID FROM VA027_ChequeDetails WHERE VA027_ChequeDetails_id =" + GetVA027_ChequeDetails_ID() +
          ") And AD_Org_ID = " + GetAD_Org_ID() + " AND AD_Client_ID = " + GetAD_Client_ID();

                DB.ExecuteQuery(sql, null, Get_TrxName());

            }
            sql = "SELECT StdPrecision, C_Tax_ID, VA027_PayAmt FROM VA027_PostDatedCheck i INNER JOIN C_Currency c ON i.C_Currency_ID = c.C_Currency_ID " +
                 "WHERE VA027_PostDatedCheck_ID = (SELECT VA027_PostDatedCheck_ID FROM VA027_ChequeDetails " +
                 "WHERE VA027_ChequeDetails_ID = " + GetVA027_ChequeDetails_ID() + ") AND i.C_Charge_ID > 0 AND i.C_Tax_ID > 0 ";

            DataSet ds = new DataSet();
            ds = DB.ExecuteDataset(sql, null, Get_Trx());
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                stdprecision = Util.GetValueOfInt(ds.Tables[0].Rows[0]["StdPrecision"]);
                C_Tax_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_Tax_ID"]);
                PayAmt = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["VA027_PayAmt"]);

                MTax tax = new MTax(GetCtx(), C_Tax_ID, null);

                if (tax.Get_ColumnIndex("Surcharge_Tax_ID") > 0 && tax.GetSurcharge_Tax_ID() > 0)
                {
                    TaxAmt = tax.CalculateSurcharge(PayAmt, true, stdprecision, out surchargeAmt);
                }
                else
                {
                    TaxAmt = tax.CalculateTax(PayAmt, true, stdprecision);
                }

                sql = "UPDATE VA027_PostDatedCheck SET TaxAmount= " + TaxAmt + " , SurchargeAmt= " + surchargeAmt +
                " WHERE VA027_PostDatedCheck_ID = (SELECT VA027_PostDatedCheck_ID FROM VA027_ChequeDetails WHERE VA027_ChequeDetails_ID =" + GetVA027_ChequeDetails_ID() + " )";

                DB.ExecuteQuery(sql, null, Get_TrxName());
            }

            return true;
        }
    }
}
