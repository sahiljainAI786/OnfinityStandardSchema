using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.Utility;
using ViennaAdvantage.Model;

namespace ViennaAdvantage.Model
{
    public class MVA027PostDatedCheck : X_VA027_PostDatedCheck, DocAction
    {
        private String _processMsg = null;
        private string sql = "";
        public const String REVERSE_INDICATOR = "^";
        public MVA027PostDatedCheck(Ctx ctx, int VA027_PostDatedCheck_ID, Trx trxName)
            : base(ctx, VA027_PostDatedCheck_ID, trxName)
        {
        }
        public MVA027PostDatedCheck(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }


        protected override bool BeforeSave(bool newRecord)
        {
            //JID_1292: Cheque Date must be greater than Account Date
            if (newRecord || Is_ValueChanged("VA027_CheckDate") || Is_ValueChanged("IsActive"))
            {
                // not to check in case of Reverse 
                if (!(GetVA027_Description() != null && GetVA027_Description().Contains("{->")))
                {
                    // can be check when Multi cheque is False
                    if (Util.GetValueOfString(Get_Value("VA027_MultiCheque")) == "False")
                    {
                        if (GetVA027_CheckDate().Value.Date <= GetDateAcct().Value.Date)
                        {
                            log.SaveError("", Msg.GetMsg(GetCtx(), "VA027_CheckDateCanbeGreaterSys"));         // Cheque Date must be greater than Account Date
                            return false;
                        }
                    }
                }
            }
            //VIS_427 Handled this code as if user change bank account and their exist any record in Cheque detail tab then delete those records in order to change the bank account
            if(Is_ValueChanged("C_BankAccount_ID") && 
                Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VA027_ChequeDetails_ID) FROM VA027_ChequeDetails WHERE VA027_PostDatedCheck_ID = " + GetVA027_PostDatedCheck_ID(), null, Get_Trx())) > 0)
            {
                log.SaveError("", Msg.GetMsg(GetCtx(), "VA027_DeleteLinesFirst"));
                return false;
            }
            //if charge is not selected then set the tax_id=0
            if (GetC_Charge_ID() == 0)
            {
                SetC_Tax_ID(0);
            }
            else
            {
                sql = "SELECT COUNT(VA027_CheckAllocate_ID) FROM VA027_CheckAllocate i INNER JOIN VA027_ChequeDetails ii " +
                    "ON i.VA027_ChequeDetails_ID = ii.VA027_ChequeDetails_ID INNER JOIN VA027_PostDatedCheck iii " +
                    "ON ii.VA027_PostDatedCheck_ID = iii.VA027_PostDatedCheck_ID " +
                    "WHERE iii.VA027_PostDatedCheck_ID = " + GetVA027_PostDatedCheck_ID();
                if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName())) > 0)
                {
                    log.SaveError("", Msg.GetMsg(GetCtx(), "VA027_DeleLineFirst"));
                    return false;
                }
            }

            // Validate -- if check detail recotd exisy then not able to chage document type Account, Bank and Multicheck checkbox
            if (Util.GetValueOfString(Get_ValueOld("VA027_MultiCheque")) == "True")
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VA027_ChequeDetails WHERE VA027_PostDatedCheck_ID = " + GetVA027_PostDatedCheck_ID(), null, Get_Trx())) > 0)
                {
                    if (Is_ValueChanged("C_DocType_ID") || Is_ValueChanged("VA027_MultiCheque") || Is_ValueChanged("C_BankAccount_ID"))
                    {
                        SetVA027_CheckNo("");
                        log.SaveError("", Msg.GetMsg(GetCtx(), "VA027_RecordExistnotChange"));
                        return false;
                    }
                }
                SetVA027_CheckNo("");
            }

            // validate when we giving check (Payable)
            MDocType docType = new MDocType(GetCtx(), GetC_DocType_ID(), Get_Trx());
            if (docType.GetDocBaseType() != "PDR")
            {
                // Validate Unique entry based on bank  and Check No
                if ((!string.IsNullOrEmpty(GetVA027_CheckNo()) && Util.GetValueOfString(Get_Value("VA027_MultiCheque")) == "False") &&
                    (newRecord || Is_ValueChanged("VA027_CheckNo") || Is_ValueChanged("IsActive") || Is_ValueChanged("C_DocType_ID") || Is_ValueChanged("C_BankAccount_ID")))
                {
                    string sql = null;
                    // check on Cheque Detail Tab
                    sql = @"SELECT COUNT(*) FROM VA027_ChequeDetails cd INNER JOIN VA027_PostDatedCheck pdc ON pdc.VA027_PostDatedCheck_ID = cd.VA027_PostDatedCheck_ID
                            INNER JOIN  C_BankAccount ba ON ba.C_BankAccount_ID = pdc.C_BankAccount_ID 
                            INNER JOIN C_Bank b ON b.C_Bank_ID = ba.C_Bank_ID 
                            INNER JOIN C_DocType dt ON dt.C_DocType_ID = pdc.C_DocType_ID
                            WHERE cd.IsActive = 'Y' AND pdc.IsActive = 'Y' AND pdc.VA027_MultiCheque = 'Y' AND pdc.DocStatus NOT IN ('RE', 'VO') AND
                            b.C_Bank_ID = (SELECT C_Bank_ID FROM c_bankaccount WHERE C_BankAccount_ID =" + GetC_BankAccount_ID() +
                            @" ) AND cd.VA027_CheckNo = '" + GetVA027_CheckNo() + @"' AND dt.DocBaseType <> 'PDR'";
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                    if (count > 0)
                    {
                        log.SaveError("", Msg.GetMsg(GetCtx(), "VA027_Bank_ChequeNoNotSame"));
                        return false;
                    }
                    else
                    {
                        // check independently on Post Dated check table
                        count = 0;
                        if (newRecord)
                        {
                            sql = @"SELECT COUNT(*) FROM  VA027_PostDatedCheck pdc 
                                INNER JOIN  C_BankAccount ba ON ba.C_BankAccount_ID = pdc.C_BankAccount_ID 
                                INNER JOIN C_Bank b ON b.C_Bank_ID = ba.C_Bank_ID  
                                INNER JOIN C_DocType dt ON dt.C_DocType_ID = pdc.C_DocType_ID
                                WHERE pdc.IsActive = 'Y' AND pdc.DocStatus NOT IN ('RE', 'VO') AND dt.DocBaseType <> 'PDR' AND
                                b.C_Bank_ID = (SELECT C_Bank_ID FROM c_bankaccount WHERE C_BankAccount_ID =" + GetC_BankAccount_ID() +
                                     @" ) AND pdc.VA027_CheckNo = '" + GetVA027_CheckNo() + @"'";
                        }
                        else
                        {
                            sql = @"SELECT COUNT(*) FROM  VA027_PostDatedCheck pdc 
                                INNER JOIN  C_BankAccount ba ON ba.C_BankAccount_ID = pdc.C_BankAccount_ID 
                                INNER JOIN C_Bank b ON b.C_Bank_ID = ba.C_Bank_ID  
                                INNER JOIN C_DocType dt ON dt.C_DocType_ID = pdc.C_DocType_ID
                                WHERE  pdc.IsActive = 'Y' AND pdc.DocStatus NOT IN ('RE', 'VO') AND dt.DocBaseType <> 'PDR' AND 
                                b.C_Bank_ID = (SELECT C_Bank_ID FROM c_bankaccount WHERE C_BankAccount_ID =" + GetC_BankAccount_ID() +
                                    @" ) AND pdc.VA027_CheckNo = '" + GetVA027_CheckNo() + @"' AND pdc.VA027_PostDatedCheck_ID <> " + GetVA027_PostDatedCheck_ID();
                        }
                        count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                        if (count > 0)
                        {
                            log.SaveError("", Msg.GetMsg(GetCtx(), "VA027_Bank_ChequeNoNotSame"));
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
                        b.C_Bank_ID = (SELECT C_Bank_ID FROM c_bankaccount WHERE C_BankAccount_ID =" + GetC_BankAccount_ID() +
                            @" ) AND pdc.CheckNo = '" + GetVA027_CheckNo() + @"' AND DocStatus NOT IN ('RE', 'VO') AND dt.DocBaseType <> 'ARR'";
                    count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                    if (count > 0)
                    {
                        log.SaveError("", Msg.GetMsg(GetCtx(), "VA027_Bank_ChequeNoNotSameOnPay"));
                        return false;
                    }
                }
            }
            // End
            return true;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (Util.GetValueOfString(Get_Value("VA027_MultiCheque")) == "True")
            {
                String sql = "UPDATE VA027_PostDatedCheck i"
                   + " SET VA027_PayAmt="
                       + "(SELECT COALESCE(SUM(VA027_ChequeAmount),0) FROM VA027_ChequeDetails il WHERE i.VA027_PostDatedCheck_ID=il.VA027_PostDatedCheck_ID) "
                   + "WHERE VA027_PostDatedCheck_ID=" + GetVA027_PostDatedCheck_ID();
                DB.ExecuteQuery(sql, null, Get_TrxName());
            }
            return true;
        }


        public virtual bool ProcessIt(String processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }

        public String PrepareIt()
        {
            log.Info(ToString());
            //VIS_427 TaskId 5285 29/02/2024 User Validation Before Prepare
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModelValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;

            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            /*VIS_427 DevOps ID 6473 14/04/2025 Get the value of date for which the period and non business day
           check will be considered*/
            DateTime? DateForPeriodCheck = GetDateAcct();
            if (Get_ColumnIndex("ReversalDoc_ID") >= 0 && Util.GetValueOfInt(Get_Value("ReversalDoc_ID")) > 0 && Get_ColumnIndex("IsReversal") >= 0 && Util.GetValueOfBool(Get_Value("IsReversal"))
                && Get_ColumnIndex("VAS_ReversedDate") >= 0 && Get_Value("VAS_ReversedDate") != null)
            {
                DateForPeriodCheck = Util.GetValueOfDateTime(Get_Value("VAS_ReversedDate"));
            }
            if (!MPeriod.IsOpen(GetCtx(), DateForPeriodCheck, dt.GetDocBaseType(), GetAD_Org_ID()))
            {
                _processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), DateForPeriodCheck, GetAD_Org_ID()))
            {
                _processMsg = VAdvantage.Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }

            if (IsVA027_MultiCheque())
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("Select Count(VA027_ChequeDetails_ID) From VA027_ChequeDetails Where VA027_PostDatedCheck_ID=" + GetVA027_PostDatedCheck_ID(), null, Get_Trx())) <= 0)
                {
                    _processMsg = "@NoLinesFound@";
                    return DocActionVariables.STATUS_INVALID;
                }
            }
            //VIS_427 TaskId 5285 29/02/2024 User Validation After Prepare
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModelValidatorVariables.DOCTIMING_AFTER_PREPARE);
            if (_processMsg != null)
            {
                return DocActionVariables.STATUS_INVALID;
            }

            return DocActionVariables.STATUS_INPROGRESS;
        }

        public virtual String CompleteIt()
        {
            //VIS_427 TaskId 5285 29/02/2024 User Validation Before Complete
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModelValidatorVariables.DOCTIMING_BEFORE_COMPLETE);
            if (_processMsg != null)
            {
                return DocActionVariables.STATUS_INPROGRESS;
            }

            //complete only if payAmt is not negative or 0
            if (GetVA027_PayAmt() > 0)
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VA027_ChequeDetails_ID) FROM VA027_ChequeDetails WHERE VA027_PostDatedCheck_ID=" + GetVA027_PostDatedCheck_ID(), null, Get_Trx())) > 0)
                {
                    string sql = "";
                    int count = 0;
                    DataSet _ds = new DataSet();
                    _ds = DB.ExecuteDataset("SELECT * FROM VA027_ChequeDetails WHERE VA027_PostDatedCheck_ID=" + GetVA027_PostDatedCheck_ID(), null, Get_Trx());
                    if (_ds != null && _ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                        {
                            sql = "SELECT COUNT(VA027_CheckAllocate_ID) FROM VA027_CheckAllocate i INNER JOIN C_InvoicePaySchedule ii" +
                                " ON i.C_InvoicePaySchedule_ID = ii.C_InvoicePaySchedule_ID" +
                                " WHERE i.VA027_ChequeDetails_ID =" + Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VA027_ChequeDetails_ID"]) +
                                " AND ii.VA009_IsPaid='Y'";
                            count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                            if (count > 0)
                            {
                                _processMsg = count + Msg.GetMsg(GetCtx(), "VA027_InvoicPayeSchedulealreadypaid");
                                return DocActionVariables.STATUS_INPROGRESS;
                            }
                            else
                            {
                                if (Util.GetValueOfInt(DB.ExecuteQuery("UPDATE VA027_ChequeDetails  SET Processed = 'Y' WHERE VA027_ChequeDetails_ID = " + Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VA027_ChequeDetails_ID"]), null, Get_Trx())) < 0)
                                {
                                    log.Severe("Processed not update on chequedetails" + Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VA027_ChequeDetails_ID"]));
                                }
                            }
                        }
                    }
                }
                //VIS_427 TaskId 5285 29/02/2024 User Validation After Complete
                _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModelValidatorVariables.DOCTIMING_AFTER_COMPLETE);
                if (_processMsg != null)
                {
                    return DocActionVariables.STATUS_INPROGRESS;
                }
                SetProcessed(true);
                SetDocAction(DOCACTION_Close);
                return DocActionVariables.STATUS_COMPLETED;
            }
            else
            {
                _processMsg = Msg.GetMsg(GetCtx(), "VA027_AmountShouldGreaterThan0");
                return DocActionVariables.STATUS_INPROGRESS;
            }
        }
        public virtual bool CloseIt()
        {
            log.Info(ToString());
            //VIS_427 TaskId 5285 29/02/2024 User Validation Before Close
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModelValidatorVariables.DOCTIMING_BEFORE_CLOSE);
            if (_processMsg != null)
            {
                return false;
            }

            //VIS_427 TaskId 5285 29/02/2024 User Validation After Close
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModelValidatorVariables.DOCTIMING_AFTER_CLOSE);
            if (_processMsg != null)
            {
                return false;
            }

            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        public virtual bool VoidIt()
        {
            log.Info(ToString());
            bool res = true;
            if (DOCSTATUS_Closed.Equals(GetDocStatus())
                || DOCSTATUS_Reversed.Equals(GetDocStatus())
                || DOCSTATUS_Voided.Equals(GetDocStatus()))
            {
                _processMsg = "Document Closed: " + GetDocStatus();
                return false;
            }
            //VIS_427 TaskId 5285 29/02/2024 User Validation Before Void
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModelValidatorVariables.DOCTIMING_BEFORE_VOID);
            if (_processMsg != null)
            {
                return false;
            }

            //Processed- Documnent canot be voided if payment reference exist
            if (DOCSTATUS_Completed.Equals(GetDocStatus()))
            {
                if (IsVA027_PaymentGenerated())
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "VA027_PaymentAvailable");
                    return false;
                }
            }


            //	Not Processed
            if (DOCSTATUS_Drafted.Equals(GetDocStatus())
                || DOCSTATUS_Invalid.Equals(GetDocStatus())
                || DOCSTATUS_InProgress.Equals(GetDocStatus())
                || DOCSTATUS_Approved.Equals(GetDocStatus())
                || DOCSTATUS_NotApproved.Equals(GetDocStatus()))
            {
                AddDescription(Msg.GetMsg(GetCtx(), "Voided") + " (" + GetVA027_PayAmt() + ")");
                SetVA027_PayAmt(Env.ZERO);
                SetVA027_DiscountAmt(Env.ZERO);
                SetVA027_WriteoffAmt(Env.ZERO);

            }
            else
            {
                res = ReverseCorrectIt();
            }
            if (res == true)
            {
                //VIS_427 TaskId 5285 29/02/2024 User Validation After Void
                _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModelValidatorVariables.DOCTIMING_AFTER_VOID);
                if (_processMsg != null)
                {
                    return false;
                }

                SetProcessed(true);
                SetDocAction(DOCACTION_None);
                return true;
            }
            else
                return false;
        }

        public bool ReverseCorrectIt()
        {
            try
            {
                //VIS_427 TaskId 5285 29/02/2024 User Validation Before ReverseCorrect
                _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModelValidatorVariables.DOCTIMING_BEFORE_REVERSECORRECT);
                if (_processMsg != null)
                {
                    return false;
                }

                MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
                /*VIS_427 DevOps ID 6473 14/04/2025 Get the value of date for which the period and non business day
                check will be considered*/
                DateTime? DateForPeriodCheck = Get_ColumnIndex("VAS_ReversedDate") >= 0 && Get_Value("VAS_ReversedDate") != null
                ? Util.GetValueOfDateTime(Get_Value("VAS_ReversedDate")) : GetDateAcct();
                if (!MPeriod.IsOpen(GetCtx(), DateForPeriodCheck, dt.GetDocBaseType(), GetAD_Org_ID()))
                {
                    _processMsg = "@PeriodClosed@";
                    return false;
                }

                // is Non Business Day?
                if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), DateForPeriodCheck, GetAD_Org_ID()))
                {
                    _processMsg = VAdvantage.Common.Common.NONBUSINESSDAY;
                    return false;
                }

                if (GetC_Payment_ID() > 0)
                {
                    MPayment pay = new MPayment(GetCtx(), GetC_Payment_ID(), Get_Trx());
                    if (pay.GetDocStatus() != "RE" && pay.GetDocStatus() != "VO")
                    {
                        _processMsg = Msg.GetMsg(GetCtx(), "VA027_PaymentNotVoid");
                        Get_Trx().Rollback();
                        return false;
                    }
                }
                MVA027PostDatedCheck reversal = new MVA027PostDatedCheck(GetCtx(), 0, Get_Trx());
                CopyValues(this, reversal);
                reversal.SetClientOrg(this);
                //VIS_045: 17-August-2023 -> DevOps Task ID:2327 -> Handle reversal amount 
                reversal.SetVA027_PayAmt(Decimal.Negate(GetVA027_PayAmt()));
                reversal.SetVA027_ConvertedAmount(Decimal.Negate(GetVA027_ConvertedAmount()));
                reversal.SetVA027_DiscountAmt(Decimal.Negate(GetVA027_DiscountAmt()));
                reversal.SetVA027_WriteoffAmt(Decimal.Negate(GetVA027_WriteoffAmt()));
                reversal.SetDocumentNo(GetDocumentNo() + REVERSE_INDICATOR);
                if (!string.IsNullOrEmpty(GetVA027_CheckNo()))
                    reversal.SetVA027_CheckNo(GetVA027_CheckNo() + REVERSE_INDICATOR);
                reversal.SetDocStatus(DOCSTATUS_Drafted);
                reversal.SetDocAction(DOCACTION_Complete);
                reversal.SetProcessed(false);
                reversal.SetProcessing(false);
                reversal.SetPosted(false);
                reversal.SetVA027_Description(GetVA027_Description());
                reversal.AddDescription("{->" + GetDocumentNo() + ")");
                //VIS_427 14/04/2025 Set date with reversal date if column exist
                if (Get_ColumnIndex("VAS_ReversedDate") >= 0 && Get_Value("VAS_ReversedDate") != null)
                {
                    reversal.Set_Value("VAS_ReversedDate", Util.GetValueOfDateTime(Get_Value("VAS_ReversedDate")));
                }
                // VIS_045, 17-Apr-2025, Set IsReversal true on Reversed Document
                if (Get_ColumnIndex("IsReversal") >= 0)
                {
                    reversal.Set_Value("IsReversal", true);
                }
                // Set Original Record ID on Reversal Document
                if (Get_ColumnIndex("ReversalDoc_ID") >= 0)
                {
                    reversal.Set_Value("ReversalDoc_ID", GetVA027_PostDatedCheck_ID());
                }
                if (reversal.Save(Get_Trx()))
                {
                    DataSet ds = new DataSet();
                    ds = DB.ExecuteDataset("Select * from VA027_CHEQUEDETAILS Where VA027_PostDatedCheck_ID=" + GetVA027_PostDatedCheck_ID(), null, Get_Trx());
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        for (Int32 i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            MVA027ChequeDetails original = new MVA027ChequeDetails(GetCtx(), ds.Tables[0].Rows[i], Get_Trx());
                            MVA027ChequeDetails cd = new MVA027ChequeDetails(GetCtx(), 0, Get_Trx());
                            if (original.GetC_Payment_ID() > 0)
                            {
                                MPayment payment = new MPayment(GetCtx(), original.GetC_Payment_ID(), Get_Trx());
                                if (payment.GetDocStatus() != "RE" && payment.GetDocStatus() != "VO")
                                {
                                    _processMsg = Msg.GetMsg(GetCtx(), "VA027_PaymentNotVoid");
                                    Get_Trx().Rollback();
                                    return false;
                                }
                            }
                            cd.SetAD_Client_ID(original.GetAD_Client_ID());
                            cd.SetAD_Org_ID(original.GetAD_Org_ID());
                            cd.SetVA027_AccountName(original.GetVA027_AccountName());
                            cd.SetLineNo(original.GetLineNo());
                            cd.SetVA027_PostDatedCheck_ID(reversal.GetVA027_PostDatedCheck_ID());
                            cd.SetVA027_AccountNo(original.GetVA027_AccountNo());
                            cd.SetVA027_CheckDate(original.GetVA027_CheckDate());
                            cd.SetVA027_CheckNo(original.GetVA027_CheckNo() + REVERSE_INDICATOR);
                            cd.SetVA027_ChequeAmount(Decimal.Negate(original.GetVA027_ChequeAmount()));
                            cd.SetVA027_MICR(original.GetVA027_MICR());
                            cd.SetVA027_PaymentStatus("3");
                            cd.SetVA027_ValidMonth(original.GetVA027_ValidMonth());
                            cd.SetProcessed(true);
                            cd.Save(Get_Trx());
                        }
                    }
                }

                if (!reversal.ProcessIt(DocActionVariables.ACTION_COMPLETE))
                {
                    _processMsg = "Reversal ERROR: " + reversal.GetProcessMsg();
                    return false;
                }
                reversal.CloseIt();
                _processMsg = reversal.GetDocumentNo();
                reversal.SetVA027_PaymentStatus("3");
                reversal.SetDocStatus(DOCSTATUS_Reversed);
                reversal.SetDocAction(DOCACTION_None);
                reversal.Save(Get_Trx());

                this.AddDescription("(" + GetDocumentNo() + "<-)");
                //VIS_427 TaskId 5285 29/02/2024 User Validation After ReverseCorrect
                _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModelValidatorVariables.DOCTIMING_AFTER_REVERSECORRECT);
                if (_processMsg != null)
                {
                    return false;
                }
                SetDocStatus(DOCSTATUS_Reversed);
                SetDocAction(DOCACTION_None);
                SetProcessed(true);

            }
            catch (Exception ex)
            {
                log.Severe(ex.ToString());
                log.Severe("Error in Reverse.");
            }

            return true;
        }

        public void AddDescription(String description)
        {
            String desc = GetVA027_Description();
            if (desc == null)
                SetVA027_Description(description);
            else
                SetVA027_Description(desc + " | " + description);
        }
        public virtual bool InvalidateIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        public virtual bool ApproveIt()
        {
            log.Info(ToString());
            return true;
        }

        public virtual bool RejectIt()
        {
            return true;
        }

        public virtual bool UnlockIt()
        {
            log.Info(ToString());
            return true;
        }

        public bool ReverseAccrualIt()
        {
            log.Info(ToString());
            return false;
        }

        public virtual bool ReActivateIt()
        {
            log.Info(ToString());
            //VIS_427 TaskId 5285 29/02/2024 User Validation at Reactivate
            if (this.ModelAction != null)
            {
                // Reactivation functionality not supported, but to be enhanced through Model Validatior with Skip Base functionality 
                bool skipBase = false;
                _processMsg = this.ModelAction.ReActivateIt(out skipBase);
                if (!String.IsNullOrEmpty(_processMsg))
                {
                    return false;
                }

                if (skipBase)
                    return true;
            }
            _processMsg = Msg.GetMsg(GetCtx(), "VA027_NotImplemented"); ;
            return false;
        }

        public String GetDocumentInfo()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            return dt.GetName() + " " + GetDocumentNo();
        }

        public String GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetDocumentNo());
            //	: Total Lines = 123.00 (#1)
            //sb.Append(":")
            //    .Append(" (#").Append(GetLines(false).Length).Append(")");
            //	 - Description
            if (GetVA027_Description() != null && GetVA027_Description().Length > 0)
                sb.Append(" - ").Append(GetVA027_Description());
            return sb.ToString();
        }

        public FileInfo CreatePDF()
        {
            return null;
        }

        public String GetProcessMsg()
        {
            return _processMsg;
        }

        public int GetDoc_User_ID()
        {
            return GetCreatedBy();
        }
        public Decimal GetApprovalAmt()
        {
            return Env.ZERO;
        }

        public bool IsComplete()
        {
            String ds = GetDocStatus();
            return DOCSTATUS_Completed.Equals(ds)
                || DOCSTATUS_Closed.Equals(ds)
                || DOCSTATUS_Reversed.Equals(ds);
        }

        public Env.QueryParams GetLineOrgsQueryInfo()
        {
            return null;
        }

        public DateTime? GetDocumentDate()
        {
            return null;
        }

        public string GetDocBaseType()
        {
            return null;
        }

        public void SetProcessMsg(string processMsg)
        {

        }


    }

}
