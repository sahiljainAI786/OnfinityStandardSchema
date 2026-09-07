using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using ViennaAdvantage.Model;
//using ViennaAdvantageSvc.Model;


namespace ViennaAdvantage.Process
{
    public class VA027_GenPayment : SvrProcess
    {
        //private string _docBaseType = null;
        private string _status = null;
        private string _exeStatus = null;
        private string _msg = null;
        MVA027ChequeDetails cqd = null;
        private DateTime? _sysDate = DateTime.Now;
        string res = null;
        private string documentno = "";
        private int paymentDocumentTypeId = 0;
        private int c_BankAccount_ID = 0;
        private StringBuilder output = new StringBuilder();
        ValueNamePair pp = null;
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("C_DocType_ID"))
                {
                    paymentDocumentTypeId = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("C_BankAccount_ID"))
                {
                    c_BankAccount_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
            }
        }

        protected override string DoIt()
        {
            MVA027PostDatedCheck pdc = new MVA027PostDatedCheck(GetCtx(), GetRecord_ID(), Get_TrxName());
            if (!pdc.IsVA027_MultiCheque() && pdc.IsVA027_DiscountingPDC() && pdc.GetDocStatus() == "CO")
            {
                res = GenratePaymentHdr(GetCtx(), GetRecord_ID(), paymentDocumentTypeId, Get_TrxName());
            }
            else if (!pdc.IsVA027_MultiCheque() && Convert.ToDateTime(pdc.GetVA027_CheckDate()) <= _sysDate && pdc.GetDocStatus() == "CO")
            {
                res = GenratePaymentHdr(GetCtx(), GetRecord_ID(), paymentDocumentTypeId, Get_TrxName());
            }
            else if (pdc.GetDocStatus() == "CO" && pdc.IsVA027_MultiCheque())
            {
                res = GenratePaymentLine(GetCtx(), GetRecord_ID(), paymentDocumentTypeId, Get_TrxName());
            }
            else if (pdc.GetDocStatus() != "CO")
            {
                return "@DocumentNotCompleted@";
            }
            else
                return "PaymentNotGenerated";

            if (res.Contains("E") && !res.Contains("F") && !res.Contains("N"))
            {
                return Msg.GetMsg(GetCtx(), "VA027_PaymentAlreadyGenerated");
            }
            else if (res.Contains("F"))
            {
                if (String.IsNullOrEmpty(documentno))
                {
                    return Msg.GetMsg(GetCtx(), "VA027_PaymentNotGenerated");
                }
                else
                {
                    return Msg.GetMsg(GetCtx(), "VA027_FewPaymentGenerated") + " " + documentno;
                }
            }
         
            else if (res.Contains("N"))
            {
                return _msg;
            }
            else
            {
                return Msg.GetMsg(GetCtx(), "VA027_PaymentGenerated") + " " + documentno;
            }
        }

        /** Generate Payment for Single Cheque  */
        public string GenratePaymentHdr(Ctx ctx, int Record_Id, int paymentDocumentTypeId, Trx trxName)
        {
            StringBuilder _sql = new StringBuilder();
            MVA027PostDatedCheck _pdc = new MVA027PostDatedCheck(ctx, Record_Id, trxName);
            MPayment _payment = new MPayment(ctx, 0, trxName);
            _payment.SetAD_Client_ID(_pdc.GetAD_Client_ID());
            _payment.SetAD_Org_ID(_pdc.GetAD_Org_ID());
            if (c_BankAccount_ID == 0)
            {
                _payment.SetC_BankAccount_ID(_pdc.GetC_BankAccount_ID());
            }
            else
            {
                _payment.SetC_BankAccount_ID(c_BankAccount_ID);
            }
            _payment.SetDateTrx(_sysDate);          // Set Today date in transaction Date.
            if (_pdc.IsVA027_DiscountingPDC())
            {
                _payment.SetDateAcct(_sysDate);        // In Case of Discounting PDC set Today date in Account Date.      
            }
            else
            {
                _payment.SetDateAcct(_pdc.GetVA027_CheckDate());
            }
            if (_pdc.IsVA027_DiscountingPDC())
            {
                _payment.SetVA027_DiscountingPDC(true);
            }
            _payment.SetDescription(_pdc.GetVA027_Description());
            if (_pdc.GetC_BPartner_ID() > 0)
            {
                _payment.SetC_BPartner_ID(_pdc.GetC_BPartner_ID());
                _payment.SetC_BPartner_Location_ID(_pdc.GetC_BPartner_Location_ID());
            }
            _payment.SetIsPrepayment(_pdc.IsVA027_IsPrepayment());
            if (_pdc.GetC_Invoice_ID() > 0)
            {
                _payment.SetC_Invoice_ID(_pdc.GetC_Invoice_ID());
            }
            if (_pdc.GetC_InvoicePaySchedule_ID() > 0)
            {
                _payment.SetC_InvoicePaySchedule_ID(_pdc.GetC_InvoicePaySchedule_ID());
            }
            if (_pdc.GetC_Order_ID() > 0)
            {
                _payment.SetC_Order_ID(_pdc.GetC_Order_ID());
            }
            if (_pdc.GetVA009_OrderPaySchedule_ID() > 0)
            {
                _payment.SetVA009_OrderPaySchedule_ID(_pdc.GetVA009_OrderPaySchedule_ID());
            }
            if (_pdc.GetC_Charge_ID() > 0)
            {
                _payment.SetC_Charge_ID(_pdc.GetC_Charge_ID());
            }
            if (_pdc.GetVA027_Payee() != null)
            {
                _payment.SetDescription(_pdc.GetVA027_Payee());
            }
            _payment.SetC_Tax_ID(_pdc.GetC_Tax_ID());
            _payment.SetTaxAmount(Math.Round(_pdc.GetTaxAmount(), 2));

            // if Surcharge Amount field is available then set value from Post Dated Check.
            if (_payment.Get_ColumnIndex("SurchargeAmt") > 0)
            {
                _payment.Set_Value("SurchargeAmt", Util.GetValueOfDecimal(_pdc.Get_Value("SurchargeAmt")));
            }

            _payment.SetPayAmt(Math.Round(_pdc.GetVA027_PayAmt(), 2));
            _payment.SetC_Currency_ID(_pdc.GetC_Currency_ID());
            _payment.SetDiscountAmt(Math.Round(_pdc.GetVA027_DiscountAmt(), 2));
            _payment.SetWriteOffAmt(Math.Round(_pdc.GetVA027_WriteoffAmt(), 2));
            _payment.SetVA009_PaymentMethod_ID(_pdc.GetVA009_PaymentMethod_ID());
            _payment.SetCheckNo(_pdc.GetVA027_CheckNo());
            _payment.SetCheckDate(_pdc.GetVA027_CheckDate());
            _payment.SetValidMonths(_pdc.GetVA027_ValidMonth());
            _payment.SetMicr(_pdc.GetVA027_MICR());
            _payment.SetAccountNo(_pdc.GetVA027_AccountNo());
            _payment.SetA_Name(_pdc.GetVA027_AccountName());
            _payment.SetPDCType(_pdc.GetPDCType());
            _sql.Clear();
            _payment.SetC_DocType_ID(paymentDocumentTypeId);
            _exeStatus = _payment.GetVA009_ExecutionStatus();
            _payment.SetVA009_ExecutionStatus(_exeStatus = "I");
            if (_payment.Save(trxName))
            {
                _status = null;
                _status = _payment.CompleteIt();
            }
            else
            {
                return "F";
            }

            if (_status == "CO")
            {
                _payment.SetDocStatus("CO");
                _pdc.SetVA027_PaymentStatus("1");
                _payment.Set_Value("VA027_PostDatedCheck_ID", _pdc.GetVA027_PostDatedCheck_ID());
                _payment.Save(trxName);
            }

            _pdc.SetC_Payment_ID(_payment.GetC_Payment_ID());
            _pdc.SetVA027_PaymentGenerated(true);
            _pdc.SetVA027_GeneratePayment("Y");
            if (!_pdc.Save(trxName))
            {
                return "E";
            }
            else
            {
                documentno = _payment.GetDocumentNo();
                return "Success";
            }
        }

        /// <summary>
        ///  Generate Payment for Multi Cheque Details 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Record_Id">post dated check id</param>
        /// <param name="paymentDocumentTypeId">payment document type id</param>
        /// <param name="trxName">trx</param>
        /// <returns>message</returns>
        public string GenratePaymentLine(Ctx ctx, int Record_Id, int paymentDocumentTypeId, Trx trxName)
        {
            int stdprecision = 0;
            Decimal surchargeAmt = Env.ZERO;
            Decimal TaxAmt = Env.ZERO;
            StringBuilder _sql = new StringBuilder();
            MVA027PostDatedCheck _pdc = new MVA027PostDatedCheck(ctx, Record_Id, trxName);
            String sql = "SELECT * FROM VA027_ChequeDetails WHERE VA027_PostDatedCheck_ID= " + Record_Id + " AND NVL(C_Payment_ID,0)=0 ORDER BY Va027_CheckDate";
            DataSet _ds = new DataSet();
            MPayment _payment = null;
            _ds = DB.ExecuteDataset(sql.ToString(), null, trxName);
            int _count = _ds.Tables[0].Rows.Count;
            if (_ds != null && _ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                {
                    MVA027ChequeDetails cd = new MVA027ChequeDetails(GetCtx(), Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VA027_ChequeDetails_ID"]), Get_Trx());
                    if (cd.IsVA027_DiscountingPDC())
                    {
                        if (Util.GetValueOfInt(_ds.Tables[0].Rows[i]["C_Payment_ID"]) == 0)
                        {
                            _sql.Clear();
                            _payment = new MPayment(ctx, 0, trxName);
                            _payment.SetAD_Client_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["AD_Client_ID"]));
                            _payment.SetAD_Org_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["AD_Org_ID"]));
                            if (c_BankAccount_ID == 0)
                            {
                                _payment.SetC_BankAccount_ID(_pdc.GetC_BankAccount_ID());
                            }
                            else
                            {
                                _payment.SetC_BankAccount_ID(c_BankAccount_ID);
                            }
                            _payment.SetDateTrx(_sysDate);
                            _payment.SetDateAcct(_sysDate);                       //cd.GetVA027_CheckDate());
                            _payment.SetDescription(_pdc.GetVA027_Description());
                            _payment.SetIsPrepayment(_pdc.IsVA027_IsPrepayment());
                            if (_pdc.GetC_Charge_ID() > 0)
                            {
                                _payment.SetC_Charge_ID(_pdc.GetC_Charge_ID());
                            }
                            _payment.SetVA027_DiscountingPDC(true);
                            if (_pdc.GetVA027_Payee() != null)
                            {
                                _payment.SetDescription(_pdc.GetVA027_Payee());
                            }

                            if (_pdc.GetC_BPartner_ID() > 0)
                            {
                                _payment.SetC_BPartner_ID(_pdc.GetC_BPartner_ID());
                                if (_pdc.GetC_BPartner_Location_ID() > 0)
                                {
                                    _payment.SetC_BPartner_Location_ID(_pdc.GetC_BPartner_Location_ID());
                                }
                            }
                            //end here
                            _payment.SetC_Tax_ID(_pdc.GetC_Tax_ID());
                            _payment.SetPayAmt(Math.Round(Util.GetValueOfDecimal(_ds.Tables[0].Rows[i]["VA027_ChequeAmount"]), 2));
                            _payment.SetC_Currency_ID(_pdc.GetC_Currency_ID());
                            _payment.SetDiscountAmt(Math.Round(_pdc.GetVA027_DiscountAmt(), 2));
                            _payment.SetWriteOffAmt(Math.Round(_pdc.GetVA027_WriteoffAmt(), 2));
                            _payment.SetVA009_PaymentMethod_ID(_pdc.GetVA009_PaymentMethod_ID());
                            _payment.SetCheckNo(Util.GetValueOfString(_ds.Tables[0].Rows[i]["VA027_CheckNo"]));
                            _payment.SetCheckDate(Util.GetValueOfDateTime(_ds.Tables[0].Rows[i]["VA027_CheckDate"]));
                            _payment.SetValidMonths(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VA027_ValidMonth"]));
                            _payment.SetMicr(Util.GetValueOfString(_ds.Tables[0].Rows[i]["VA027_MICR"]));
                            _payment.SetAccountNo(Util.GetValueOfString(_ds.Tables[0].Rows[i]["VA027_AccountNo"]));
                            _payment.SetA_Name(Util.GetValueOfString(_ds.Tables[0].Rows[i]["VA027_AccountName"]));
                            _payment.SetPDCType(_pdc.GetPDCType());
                            //calculate Tax Amount 
                            if (_pdc.GetC_Tax_ID() > 0)
                            {
                                sql = "SELECT StdPrecision FROM VA027_PostDatedCheck i INNER JOIN C_Currency c ON i.C_Currency_ID = c.C_Currency_ID " +
                                "WHERE VA027_PostDatedCheck_ID = " + _pdc.GetVA027_PostDatedCheck_ID();
                                stdprecision = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                                MTax tax = new MTax(GetCtx(), _pdc.GetC_Tax_ID(), null);
                                if (tax.Get_ColumnIndex("Surcharge_Tax_ID") > 0 && tax.GetSurcharge_Tax_ID() > 0)
                                {
                                    TaxAmt = tax.CalculateSurcharge(Util.GetValueOfDecimal(_ds.Tables[0].Rows[i]["VA027_ChequeAmount"]), true, stdprecision, out surchargeAmt);
                                }
                                else
                                {
                                    TaxAmt = tax.CalculateTax(Util.GetValueOfDecimal(_ds.Tables[0].Rows[i]["VA027_ChequeAmount"]), true, stdprecision);
                                }
                                _payment.SetTaxAmount(TaxAmt);
                                _payment.Set_Value("SurchargeAmt", surchargeAmt);

                            }

                            _sql.Clear();
                            _payment.SetC_DocType_ID(paymentDocumentTypeId);
                            _exeStatus = _payment.GetVA009_ExecutionStatus();
                            _payment.SetVA009_ExecutionStatus(_exeStatus = "I");
                            if (_payment.Save(trxName))
                            {
                                //payment will get completed only if PaymentAllocate is generated.
                                if (GenratePaymentAllocate(GetCtx(), Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VA027_ChequeDetails_ID"]), _payment.GetC_Payment_ID(), _payment.Get_TrxName()))
                                {
                                    _status = null;
                                    _status = _payment.CompleteIt();
                                }
                                else
                                {
                                    //return "N";
                                    output.Append("N");
                                    continue;
                                }
                            }
                            else
                            {
                                //return "F";
                                pp = VLogger.RetrieveError();
                                log.Info("Error Saving product save for PDC : " + pp.GetValue() + "  , Error Name : " + pp.GetName());
                                output.Append("F");
                                continue;
                            }
                            if (_status == "CO")
                            {
                                _payment.SetDocStatus("CO");
                                _payment.Set_Value("VA027_PostDatedCheck_ID", _pdc.GetVA027_PostDatedCheck_ID());
                                if (_payment.Save(trxName))
                                {
                                    cqd = new MVA027ChequeDetails(ctx, Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VA027_ChequeDetails_ID"]), trxName);
                                    cqd.SetC_Payment_ID(_payment.GetC_Payment_ID());
                                    cqd.SetVA027_PaymentStatus("1");
                                    if (!cqd.Save(trxName))
                                    {
                                        trxName.Rollback();
                                        pp = VLogger.RetrieveError();
                                        log.Info("Error Saving Chequedetails : " + pp.GetValue() + "  , Error Name : " + pp.GetName());
                                        _msg = Msg.GetMsg(ctx, "ChequedetailsNotSaved") + ", " + (pp != null ? pp.GetName() : "");

                                        //return "N";
                                        output.Append("N");
                                        continue;
                                    }
                                }
                                //VIS_427 fixed code to add dot after last document number
                                if (i == _ds.Tables[0].Rows.Count - 1)
                                {
                                    documentno += _payment.GetDocumentNo();
                                }
                                else
                                {
                                    documentno += _payment.GetDocumentNo() + ",";
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Convert.ToDateTime(_ds.Tables[0].Rows[i]["VA027_CheckDate"]) <= _sysDate)
                        {
                            if (Util.GetValueOfInt(_ds.Tables[0].Rows[i]["C_Payment_ID"]) == 0)
                            {
                                _sql.Clear();
                                _payment = new MPayment(ctx, 0, trxName);
                                _payment.SetAD_Client_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["AD_Client_ID"]));
                                _payment.SetAD_Org_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["AD_Org_ID"]));
                                if (c_BankAccount_ID == 0)
                                    _payment.SetC_BankAccount_ID(_pdc.GetC_BankAccount_ID());
                                else
                                    _payment.SetC_BankAccount_ID(c_BankAccount_ID);
                                _payment.SetDateTrx(_sysDate);
                                _payment.SetDateAcct(cd.GetVA027_CheckDate());
                                _payment.SetDescription(_pdc.GetVA027_Description());
                                _payment.SetIsPrepayment(_pdc.IsVA027_IsPrepayment());
                                if (_pdc.GetC_Charge_ID() > 0)
                                {
                                    _payment.SetC_Charge_ID(_pdc.GetC_Charge_ID());
                                }
                                if (_pdc.GetVA027_Payee() != null)
                                {
                                    _payment.SetDescription(_pdc.GetVA027_Payee());
                                }

                                if (_pdc.GetC_BPartner_ID() > 0)
                                {
                                    _payment.SetC_BPartner_ID(_pdc.GetC_BPartner_ID());
                                    if (_pdc.GetC_BPartner_Location_ID() > 0)
                                    {
                                        _payment.SetC_BPartner_Location_ID(_pdc.GetC_BPartner_Location_ID());
                                    }
                                }

                                _payment.SetC_Tax_ID(_pdc.GetC_Tax_ID());
                                _payment.SetPayAmt(Math.Round(Util.GetValueOfDecimal(_ds.Tables[0].Rows[i]["VA027_ChequeAmount"]), 2));
                                _payment.SetC_Currency_ID(_pdc.GetC_Currency_ID());
                                _payment.SetDiscountAmt(Math.Round(_pdc.GetVA027_DiscountAmt(), 2));
                                _payment.SetWriteOffAmt(Math.Round(_pdc.GetVA027_WriteoffAmt(), 2));
                                _payment.SetVA009_PaymentMethod_ID(_pdc.GetVA009_PaymentMethod_ID());
                                _payment.SetCheckNo(Util.GetValueOfString(_ds.Tables[0].Rows[i]["VA027_CheckNo"]));
                                _payment.SetCheckDate(Util.GetValueOfDateTime(_ds.Tables[0].Rows[i]["VA027_CheckDate"]));
                                _payment.SetValidMonths(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VA027_ValidMonth"]));
                                _payment.SetMicr(Util.GetValueOfString(_ds.Tables[0].Rows[i]["VA027_MICR"]));
                                _payment.SetAccountNo(Util.GetValueOfString(_ds.Tables[0].Rows[i]["VA027_AccountNo"]));
                                _payment.SetA_Name(Util.GetValueOfString(_ds.Tables[0].Rows[i]["VA027_AccountName"]));
                                _payment.SetPDCType(_pdc.GetPDCType());

                                //calculate Tax Amount 
                                if (_pdc.GetC_Tax_ID() > 0)
                                {
                                    sql = "SELECT StdPrecision FROM VA027_PostDatedCheck i INNER JOIN C_Currency c ON i.C_Currency_ID = c.C_Currency_ID " +
                                    "WHERE VA027_PostDatedCheck_ID = " + _pdc.GetVA027_PostDatedCheck_ID();
                                    stdprecision = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                                    MTax tax = new MTax(GetCtx(), _pdc.GetC_Tax_ID(), null);
                                    if (tax.Get_ColumnIndex("Surcharge_Tax_ID") > 0 && tax.GetSurcharge_Tax_ID() > 0)
                                    {
                                        TaxAmt = tax.CalculateSurcharge(Util.GetValueOfDecimal(_ds.Tables[0].Rows[i]["VA027_ChequeAmount"]), true, stdprecision, out surchargeAmt);
                                    }
                                    else
                                    {
                                        TaxAmt = tax.CalculateTax(Util.GetValueOfDecimal(_ds.Tables[0].Rows[i]["VA027_ChequeAmount"]), true, stdprecision);
                                    }
                                    _payment.SetTaxAmount(TaxAmt);
                                    _payment.Set_Value("SurchargeAmt", surchargeAmt);
                                }
                                _sql.Clear();
                                _payment.SetC_DocType_ID(paymentDocumentTypeId);
                                _exeStatus = _payment.GetVA009_ExecutionStatus();
                                _payment.SetVA009_ExecutionStatus(_exeStatus = "I");
                                if (_payment.Save(trxName))
                                {
                                    //payment will get completed only if PaymentAllocate is generated.
                                    if (GenratePaymentAllocate(GetCtx(), Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VA027_ChequeDetails_ID"]), _payment.GetC_Payment_ID(), _payment.Get_TrxName()))
                                    {
                                        _status = null;
                                        _status = _payment.CompleteIt();
                                    }
                                    else
                                    {
                                        //return "N";
                                        output.Append("N");
                                        continue;
                                    }
                                }
                                else
                                {
                                    //return "F";
                                    output.Append("F");
                                    pp = VLogger.RetrieveError();
                                    log.Info("Error Saving product save for PDC : " + pp.GetValue() + "  , Error Name : " + pp.GetName());
                                    continue;
                                }
                                if (_status == "CO")
                                {
                                    _payment.SetDocStatus("CO");
                                    _payment.Set_Value("VA027_PostDatedCheck_ID", _pdc.GetVA027_PostDatedCheck_ID());
                                    if (_payment.Save(trxName))
                                    {
                                        cqd = new MVA027ChequeDetails(ctx, Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VA027_ChequeDetails_ID"]), trxName);
                                        cqd.SetC_Payment_ID(_payment.GetC_Payment_ID());
                                        cqd.SetVA027_PaymentStatus("1");
                                        if (!cqd.Save(trxName))
                                        {
                                            trxName.Rollback();
                                            pp = VLogger.RetrieveError();
                                            log.Info("Error Saving Chequedetails : " + pp.GetValue() + "  , Error Name : " + pp.GetName());
                                            _msg = Msg.GetMsg(ctx, "ChequedetailsNotSaved") + ", " + (pp != null ? pp.GetName() : "");
                                            //return "N";
                                            output.Append("N");
                                            continue;
                                        }
                                    }
                                    //VIS_427 fixed code to add dot after last document number
                                    if (i == _ds.Tables[0].Rows.Count - 1)
                                    {
                                        documentno += _payment.GetDocumentNo();
                                    }
                                    else
                                    {
                                        documentno += _payment.GetDocumentNo() + ",";
                                    }
                                }
                            }
                            else
                            {
                                //return "E";
                                output.Append("E");
                                continue;
                            }
                        }
                        else if (String.IsNullOrEmpty(documentno)) // when payment generate against any check line then don't return F
                        {
                            //return "F";
                            output.Append("F");
                            continue;
                        }
                    }

                    // when payment generated against any Line then commit that record, 
                    // bcz we have to rollback those payment whihc is not completed or partially created
                    trxName.Commit();
                }
            }
            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(VA027_ChequeDetails_ID) From VA027_ChequeDetails Where VA027_PostDatedCheck_ID=" + Record_Id
                + " AND VA027_PaymentStatus!='1'", null, trxName)) == 0)
            {
                _pdc.SetVA027_PaymentStatus("1");
                _pdc.SetVA027_PaymentGenerated(true);
                _pdc.SetVA027_GeneratePayment("Y");
                if (!_pdc.Save(trxName))
                {
                    return "E";
                }
            }

            return String.IsNullOrEmpty(output.ToString()) ? "Success" : output.ToString();
        }

        /// <summary>
        /// Generate Payment Allocate As per ChequeDetails
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="Record_Id">VA027_ChequeDetails_ID</param>
        /// <param name="paymentId">C_Payment_ID</param>
        /// <param name="trxName">trxName</param>
        /// <returns>true/false</returns>
        public bool GenratePaymentAllocate(Ctx ctx, int Record_Id, int paymentId, Trx trxName)
        {
            String sql = "SELECT * FROM VA027_Checkallocate WHERE VA027_ChequeDetails_ID=" + Record_Id;
            DataSet ds = new DataSet();
            MPaymentAllocate _paymentAllocate = null;
            ds = DB.ExecuteDataset(sql.ToString(), null, trxName);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    _paymentAllocate = new MPaymentAllocate(ctx, 0, trxName);
                    _paymentAllocate.SetAD_Client_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Client_ID"]));
                    _paymentAllocate.SetAD_Org_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Org_ID"]));
                    _paymentAllocate.SetC_Payment_ID(paymentId);
                    _paymentAllocate.SetC_Invoice_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Invoice_ID"]));
                    _paymentAllocate.SetC_InvoicePaySchedule_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_InvoicePaySchedule_ID"]));
                    _paymentAllocate.SetAmount(Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Amount"]));
                    _paymentAllocate.SetInvoiceAmt(Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["InvoiceAmt"]));
                    _paymentAllocate.SetWriteOffAmt(Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["WriteOffAmt"]));
                    _paymentAllocate.SetDiscountAmt(Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["DiscountAmt"]));

                    if (!_paymentAllocate.Save(trxName))
                    {
                        trxName.Rollback();
                        pp = VLogger.RetrieveError();
                        log.Info("Error Occured while generating Payment Allocate : " + pp.GetValue() + "  , Error Name : " + pp.GetName());
                        _msg = Msg.GetMsg(ctx, "PaymentNotAllocated") + ", " + (pp != null ? pp.GetName() : "");
                        return false;
                    }

                }
            }
            return true;
        }
    }
}
