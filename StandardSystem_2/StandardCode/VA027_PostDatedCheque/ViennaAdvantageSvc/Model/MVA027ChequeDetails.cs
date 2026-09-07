using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.Utility;
using ViennaAdvantage.Model;

namespace ViennaAdvantage.Model
{
    public class MVA027ChequeDetails : X_VA027_ChequeDetails
    {
        public MVA027ChequeDetails(Ctx ctx, int VA027_ChequeDetails_ID, Trx trxName)
            : base(ctx, VA027_ChequeDetails_ID, trxName)
        {
        }

        public MVA027ChequeDetails(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        protected override bool BeforeSave(bool newRecord)
        {
            //JID_1292: Cheque Date must be greater than Account Date
            MVA027PostDatedCheck pdc = new MVA027PostDatedCheck(GetCtx(), GetVA027_PostDatedCheck_ID(), Get_Trx());
            if (newRecord || Is_ValueChanged("VA027_CheckDate") || Is_ValueChanged("IsActive"))
            {
                if (!(pdc.GetVA027_Description() != null && pdc.GetVA027_Description().Contains("{->")))
                {
                    if (GetVA027_CheckDate().Value.Date <= pdc.GetDateAcct().Value.Date)
                    {
                        log.SaveError("", Msg.GetMsg(GetCtx(), "VA027_CheckDateCanbeGreaterSys"));
                        return false;
                    }
                }
            }

            // validate when we giving check (Payable)
            MDocType docType = new MDocType(GetCtx(), pdc.GetC_DocType_ID(), Get_Trx());
            if (docType.GetDocBaseType() != "PDR")
            {
                // Validate Unique entry based on bank Account and Check No
                if ((!string.IsNullOrEmpty(GetVA027_CheckNo())) && (newRecord || Is_ValueChanged("VA027_CheckNo")))
                {
                    string sql = null;
                    // check on cheque detail tab
                    if (newRecord)
                    {
                        sql = @"SELECT COUNT(*) FROM VA027_ChequeDetails cd INNER JOIN VA027_PostDatedCheck pdc ON pdc.VA027_PostDatedCheck_ID = cd.VA027_PostDatedCheck_ID 
                            INNER JOIN  C_BankAccount ba ON ba.C_BankAccount_ID = pdc.C_BankAccount_ID 
                            INNER JOIN C_Bank b ON b.C_Bank_ID = ba.C_Bank_ID  
                            INNER JOIN C_DocType dt ON dt.C_DocType_ID = pdc.C_DocType_ID 
                            WHERE cd.IsActive = 'Y' AND pdc.IsActive = 'Y' AND pdc.DocStatus NOT IN ('RE', 'VO') AND dt.DocBaseType <> 'PDR' AND 
                            b.C_Bank_ID = (SELECT C_Bank_ID FROM c_bankaccount WHERE C_BankAccount_ID =" + pdc.GetC_BankAccount_ID() +
                                @" ) AND cd.VA027_CheckNo = '" + GetVA027_CheckNo() + @"'";
                    }
                    else
                    {
                        sql = @"SELECT COUNT(*) FROM VA027_ChequeDetails cd INNER JOIN VA027_PostDatedCheck pdc ON pdc.VA027_PostDatedCheck_ID = cd.VA027_PostDatedCheck_ID 
                            INNER JOIN  C_BankAccount ba ON ba.C_BankAccount_ID = pdc.C_BankAccount_ID 
                            INNER JOIN C_Bank b ON b.C_Bank_ID = ba.C_Bank_ID  
                            INNER JOIN C_DocType dt ON dt.C_DocType_ID = pdc.C_DocType_ID 
                            WHERE cd.IsActive = 'Y' AND pdc.IsActive = 'Y' AND pdc.DocStatus NOT IN ('RE', 'VO') AND dt.DocBaseType <> 'PDR' AND 
                            b.C_Bank_ID = (SELECT C_Bank_ID FROM c_bankaccount WHERE C_BankAccount_ID =" + pdc.GetC_BankAccount_ID() +
                                @" ) AND cd.VA027_CheckNo = '" + GetVA027_CheckNo() + @"' AND cd.VA027_ChequeDetails_ID <> " + GetVA027_ChequeDetails_ID();
                    }
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                    if (count > 0)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "VA027_Bank_ChequeNoNotSame"));
                        return false;
                    }
                    else
                    {
                        // check independently on Post Dated check table
                        count = 0;
                        sql = @"SELECT COUNT(*) FROM  VA027_PostDatedCheck pdc  
                            INNER JOIN  C_BankAccount ba ON ba.C_BankAccount_ID = pdc.C_BankAccount_ID 
                            INNER JOIN C_Bank b ON b.C_Bank_ID = ba.C_Bank_ID  
                            INNER JOIN C_DocType dt ON dt.C_DocType_ID = pdc.C_DocType_ID 
                            WHERE  pdc.IsActive = 'Y' AND pdc.VA027_MultiCheque = 'N' AND dt.DocBaseType <> 'PDR' AND 
                            b.C_Bank_ID = (SELECT C_Bank_ID FROM c_bankaccount WHERE C_BankAccount_ID =" + pdc.GetC_BankAccount_ID() +
                                @" ) AND pdc.VA027_CheckNo = '" + GetVA027_CheckNo() + @"' AND pdc.DocStatus NOT IN ('RE', 'VO')";
                        count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                        if (count > 0)
                        {
                            log.SaveError("Error", Msg.GetMsg(GetCtx(), "VA027_Bank_ChequeNoNotSame"));
                            return false;
                        }
                    }

                    // check Payment Table
                    count = 0;
                    sql = @"SELECT COUNT(*) FROM  C_Payment pdc
                        INNER JOIN  C_BankAccount ba ON ba.C_BankAccount_ID = pdc.C_BankAccount_ID 
                        INNER JOIN C_Bank b ON b.C_Bank_ID = ba.C_Bank_ID  
                        INNER JOIN C_DocType dt ON dt.C_DocType_ID = pdc.C_DocType_ID
                        WHERE  pdc.IsActive = 'Y' AND
                        b.C_Bank_ID = (SELECT C_Bank_ID FROM c_bankaccount WHERE C_BankAccount_ID =" + pdc.GetC_BankAccount_ID() +
                            @" ) AND pdc.CheckNo = '" + GetVA027_CheckNo() + @"' AND DocStatus NOT IN ('RE', 'VO') AND dt.DocBaseType <> 'ARR'";
                    count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                    if (count > 0)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "VA027_Bank_ChequeNoNotSameOnPay"));
                        return false;
                    }
                }
            }
            //VIS_427 17/02/2025 Set the account number to be the same as the post-dated check account number field. 
            if (string.IsNullOrEmpty(Util.GetValueOfString(GetVA027_AccountNo())))
            {
                SetVA027_AccountNo(pdc.GetVA027_AccountNo());
            }
            //VIS_427 17/02/2025 Set the account name to be the same as the post-dated check account name field. 
            if (string.IsNullOrEmpty(Util.GetValueOfString(GetVA027_AccountName())))
            {
                SetVA027_AccountName(pdc.GetVA027_AccountName());
            }
            return true;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            //if (newRecord || (GetVA027_ChequeAmount() != Util.GetValueOfDecimal(Get_ValueOld("VA027_ChequeAmount"))))
            //{
            //    MVA027PostDatedCheck _PSD = new MVA027PostDatedCheck(GetCtx(), GetVA027_PostDatedCheck_ID(), null);
            //    _PSD.SetVA027_PayAmt(_PSD.GetVA027_PayAmt() + GetVA027_ChequeAmount());
            //    _PSD.Save();
            //}
            int stdprecision = 0;
            int C_Tax_ID = 0;
            Decimal surchargeAmt = Env.ZERO;
            Decimal TaxAmt = Env.ZERO;
            Decimal PayAmt = Env.ZERO;

            string sql = "UPDATE VA027_PostDatedCheck i"
                + " SET VA027_PayAmt="
                    + "(SELECT COALESCE(SUM(VA027_ChequeAmount),0) FROM VA027_ChequeDetails il WHERE i.VA027_PostDatedCheck_ID=il.VA027_PostDatedCheck_ID), "
                    + " VA027_ConvertedAmount = (SELECT COALESCE(SUM(VA027_ChequeAmount),0) FROM VA027_ChequeDetails il WHERE i.VA027_PostDatedCheck_ID=il.VA027_PostDatedCheck_ID) "
                + "WHERE VA027_PostDatedCheck_ID=" + GetVA027_PostDatedCheck_ID();

            DB.ExecuteQuery(sql, null, Get_TrxName());

            sql = "SELECT StdPrecision, C_Tax_ID, VA027_PayAmt FROM VA027_PostDatedCheck i " +
               "INNER JOIN C_Currency c ON i.C_Currency_ID = c.C_Currency_ID WHERE VA027_PostDatedCheck_ID =" + GetVA027_PostDatedCheck_ID() +
               " AND i.C_Charge_ID > 0 AND i.C_Tax_ID > 0";

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
                   " WHERE VA027_PostDatedCheck_ID = " + GetVA027_PostDatedCheck_ID();

                DB.ExecuteQuery(sql, null, Get_TrxName());
            }
            return true;
        }

        protected override bool BeforeDelete()
        {
            return true;
        }

        protected override bool AfterDelete(bool success)
        {
            //MVA027PostDatedCheck _PSD = new MVA027PostDatedCheck(GetCtx(), GetVA027_PostDatedCheck_ID(), null);
            //_PSD.SetVA027_PayAmt(_PSD.GetVA027_PayAmt() - GetVA027_ChequeAmount());
            //_PSD.Save();
            int stdprecision = 0;
            int C_Tax_ID = 0;
            Decimal surchargeAmt = Env.ZERO;
            Decimal TaxAmt = Env.ZERO;
            Decimal PayAmt = Env.ZERO;

            string sql = "UPDATE VA027_PostDatedCheck i"
               + " SET VA027_PayAmt="
                   + "(SELECT COALESCE(SUM(VA027_ChequeAmount),0) FROM VA027_ChequeDetails il WHERE i.VA027_PostDatedCheck_ID=il.VA027_PostDatedCheck_ID), "
                   + " VA027_ConvertedAmount = (SELECT COALESCE(SUM(VA027_ChequeAmount),0) FROM VA027_ChequeDetails il WHERE i.VA027_PostDatedCheck_ID=il.VA027_PostDatedCheck_ID) "
               + "WHERE VA027_PostDatedCheck_ID=" + GetVA027_PostDatedCheck_ID();
            DB.ExecuteQuery(sql, null, Get_TrxName());

            sql = "SELECT StdPrecision, C_Tax_ID, VA027_PayAmt FROM VA027_PostDatedCheck i " +
               "INNER JOIN C_Currency c ON i.C_Currency_ID = c.C_Currency_ID WHERE VA027_PostDatedCheck_ID =" + GetVA027_PostDatedCheck_ID() +
               " AND i.C_Charge_ID > 0 AND i.C_Tax_ID > 0";

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
                   " WHERE VA027_PostDatedCheck_ID = " + GetVA027_PostDatedCheck_ID();

                DB.ExecuteQuery(sql, null, Get_TrxName());
            }
            return true;
        }
    }
}
