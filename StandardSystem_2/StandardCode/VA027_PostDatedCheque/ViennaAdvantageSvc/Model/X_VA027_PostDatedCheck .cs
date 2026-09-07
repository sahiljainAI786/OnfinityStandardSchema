namespace ViennaAdvantage.Model
{
    /** Generated Model - DO NOT CHANGE */
    using System;
    using System.Text;
    using VAdvantage.DataBase;
    using VAdvantage.Common;
    using VAdvantage.Classes;
    using VAdvantage.Process;
    using VAdvantage.Model;
    using VAdvantage.Utility;
    using System.Data;/** Generated Model for VA027_PostDatedCheck
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VA027_PostDatedCheck : PO
    {
        public X_VA027_PostDatedCheck(Context ctx, int VA027_PostDatedCheck_ID, Trx trxName)
            : base(ctx, VA027_PostDatedCheck_ID, trxName)
        {/** if (VA027_PostDatedCheck_ID == 0){SetC_BPartner_ID (0);SetC_BankAccount_ID (0);SetC_Currency_ID (0);SetC_DocType_ID (0);SetDocStatus (null);// DR
SetVA009_PaymentMethod_ID (0);SetVA027_CheckDate (DateTime.Now);SetVA027_CheckNo (null);SetVA027_PayAmt (0.0);SetVA027_PostDatedCheck_ID (0);SetVA027_TrxDate (DateTime.Now);// SYSDATE
} */
        }
        public X_VA027_PostDatedCheck(Ctx ctx, int VA027_PostDatedCheck_ID, Trx trxName)
            : base(ctx, VA027_PostDatedCheck_ID, trxName)
        {/** if (VA027_PostDatedCheck_ID == 0){SetC_BPartner_ID (0);SetC_BankAccount_ID (0);SetC_Currency_ID (0);SetC_DocType_ID (0);SetDocStatus (null);// DR
SetVA009_PaymentMethod_ID (0);SetVA027_CheckDate (DateTime.Now);SetVA027_CheckNo (null);SetVA027_PayAmt (0.0);SetVA027_PostDatedCheck_ID (0);SetVA027_TrxDate (DateTime.Now);// SYSDATE
} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VA027_PostDatedCheck(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VA027_PostDatedCheck(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VA027_PostDatedCheck(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VA027_PostDatedCheck() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27753990197672L;/** Last Updated Timestamp 8/22/2016 4:51:20 PM */
        public static long updatedMS = 1471864880883L;/** AD_Table_ID=1001258 */
        public static int Table_ID; // =1001258;
        /** TableName=VA027_PostDatedCheck */
        public static String Table_Name = "VA027_PostDatedCheck";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
*/
        protected override int Get_AccessLevel() { return Convert.ToInt32(accessLevel.ToString()); }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Context ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Ctx ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Info
@return info
*/
        public override String ToString() { StringBuilder sb = new StringBuilder("X_VA027_PostDatedCheck[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Business Partner.
@param C_BPartner_ID Identifies a Customer/Prospect */
        public void SetC_BPartner_ID(int C_BPartner_ID) { if (C_BPartner_ID < 1) throw new ArgumentException("C_BPartner_ID is mandatory."); Set_Value("C_BPartner_ID", C_BPartner_ID); }/** Get Business Partner.
@return Identifies a Customer/Prospect */
        public int GetC_BPartner_ID() { Object ii = Get_Value("C_BPartner_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Location.
@param C_BPartner_Location_ID Identifies the address for this Account/Prospect. */
        public void SetC_BPartner_Location_ID(int C_BPartner_Location_ID)
        {
            if (C_BPartner_Location_ID <= 0) Set_Value("C_BPartner_Location_ID", null);
            else
                Set_Value("C_BPartner_Location_ID", C_BPartner_Location_ID);
        }/** Get Location.
@return Identifies the address for this Account/Prospect. */
        public int GetC_BPartner_Location_ID() { Object ii = Get_Value("C_BPartner_Location_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Bank Account.
@param C_BankAccount_ID Account at the Bank */
        public void SetC_BankAccount_ID(int C_BankAccount_ID) { if (C_BankAccount_ID < 1) throw new ArgumentException("C_BankAccount_ID is mandatory."); Set_Value("C_BankAccount_ID", C_BankAccount_ID); }/** Get Bank Account.
@return Account at the Bank */
        public int GetC_BankAccount_ID() { Object ii = Get_Value("C_BankAccount_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Charge.
@param C_Charge_ID Additional document charges */
        public void SetC_Charge_ID(int C_Charge_ID)
        {
            if (C_Charge_ID <= 0) Set_Value("C_Charge_ID", null);
            else
                Set_Value("C_Charge_ID", C_Charge_ID);
        }/** Get Charge.
@return Additional document charges */
        public int GetC_Charge_ID() { Object ii = Get_Value("C_Charge_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Currency.
@param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID) { if (C_Currency_ID < 1) throw new ArgumentException("C_Currency_ID is mandatory."); Set_Value("C_Currency_ID", C_Currency_ID); }/** Get Currency.
@return The Currency for this record */
        public int GetC_Currency_ID() { Object ii = Get_Value("C_Currency_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Document Type.
@param C_DocType_ID Document type or rules */
        public void SetC_DocType_ID(int C_DocType_ID) { if (C_DocType_ID < 0) throw new ArgumentException("C_DocType_ID is mandatory."); Set_Value("C_DocType_ID", C_DocType_ID); }/** Get Document Type.
@return Document type or rules */
        public int GetC_DocType_ID() { Object ii = Get_Value("C_DocType_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Invoice Payment Schedule.
@param C_InvoicePaySchedule_ID Invoice Payment Schedule */
        public void SetC_InvoicePaySchedule_ID(int C_InvoicePaySchedule_ID)
        {
            if (C_InvoicePaySchedule_ID <= 0) Set_Value("C_InvoicePaySchedule_ID", null);
            else
                Set_Value("C_InvoicePaySchedule_ID", C_InvoicePaySchedule_ID);
        }/** Get Invoice Payment Schedule.
@return Invoice Payment Schedule */
        public int GetC_InvoicePaySchedule_ID() { Object ii = Get_Value("C_InvoicePaySchedule_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Invoice.
@param C_Invoice_ID Invoice Identifier */
        public void SetC_Invoice_ID(int C_Invoice_ID)
        {
            if (C_Invoice_ID <= 0) Set_Value("C_Invoice_ID", null);
            else
                Set_Value("C_Invoice_ID", C_Invoice_ID);
        }/** Get Invoice.
@return Invoice Identifier */
        public int GetC_Invoice_ID() { Object ii = Get_Value("C_Invoice_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order.
@param C_Order_ID Sales Order */
        public void SetC_Order_ID(int C_Order_ID)
        {
            if (C_Order_ID <= 0) Set_Value("C_Order_ID", null);
            else
                Set_Value("C_Order_ID", C_Order_ID);
        }/** Get Order.
@return Sales Order */
        public int GetC_Order_ID() { Object ii = Get_Value("C_Order_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Payment.
@param C_Payment_ID Payment identifier */
        public void SetC_Payment_ID(int C_Payment_ID)
        {
            if (C_Payment_ID <= 0) Set_Value("C_Payment_ID", null);
            else
                Set_Value("C_Payment_ID", C_Payment_ID);
        }/** Get Payment.
@return Payment identifier */
        public int GetC_Payment_ID() { Object ii = Get_Value("C_Payment_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Tax.
@param C_Tax_ID Tax identifier */
        public void SetC_Tax_ID(int C_Tax_ID)
        {
            if (C_Tax_ID <= 0) Set_Value("C_Tax_ID", null);
            else
                Set_Value("C_Tax_ID", C_Tax_ID);
        }/** Get Tax.
@return Tax identifier */
        public int GetC_Tax_ID() { Object ii = Get_Value("C_Tax_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Conversion Rate.
@param CurrencyRate Currency Conversion Rate */
        public void SetCurrencyRate(Decimal? CurrencyRate) { Set_Value("CurrencyRate", (Decimal?)CurrencyRate); }/** Get Conversion Rate.
@return Currency Conversion Rate */
        public Decimal GetCurrencyRate() { Object bd = Get_Value("CurrencyRate"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Account Date.
@param DateAcct General Ledger Date */
        public void SetDateAcct(DateTime? DateAcct) { Set_Value("DateAcct", (DateTime?)DateAcct); }/** Get Account Date.
@return General Ledger Date */
        public DateTime? GetDateAcct() { return (DateTime?)Get_Value("DateAcct"); }
        /** DocAction AD_Reference_ID=135 */
        public static int DOCACTION_AD_Reference_ID = 135;/** <None> = -- */
        public static String DOCACTION_None = "--";/** Approve = AP */
        public static String DOCACTION_Approve = "AP";/** Close = CL */
        public static String DOCACTION_Close = "CL";/** Complete = CO */
        public static String DOCACTION_Complete = "CO";/** Invalidate = IN */
        public static String DOCACTION_Invalidate = "IN";/** Post = PO */
        public static String DOCACTION_Post = "PO";/** Prepare = PR */
        public static String DOCACTION_Prepare = "PR";/** Reverse - Accrual = RA */
        public static String DOCACTION_Reverse_Accrual = "RA";/** Reverse - Correct = RC */
        public static String DOCACTION_Reverse_Correct = "RC";/** Re-activate = RE */
        public static String DOCACTION_Re_Activate = "RE";/** Reject = RJ */
        public static String DOCACTION_Reject = "RJ";/** Void = VO */
        public static String DOCACTION_Void = "VO";/** Wait Complete = WC */
        public static String DOCACTION_WaitComplete = "WC";/** Unlock = XL */
        public static String DOCACTION_Unlock = "XL";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsDocActionValid(String test) { return test == null || test.Equals("--") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("IN") || test.Equals("PO") || test.Equals("PR") || test.Equals("RA") || test.Equals("RC") || test.Equals("RE") || test.Equals("RJ") || test.Equals("VO") || test.Equals("WC") || test.Equals("XL"); }/** Set Document Action.
@param DocAction The targeted status of the document */
        public void SetDocAction(String DocAction)
        {
            if (!IsDocActionValid(DocAction))
                throw new ArgumentException("DocAction Invalid value - " + DocAction + " - Reference_ID=135 - -- - AP - CL - CO - IN - PO - PR - RA - RC - RE - RJ - VO - WC - XL"); if (DocAction != null && DocAction.Length > 2) { log.Warning("Length > 2 - truncated"); DocAction = DocAction.Substring(0, 2); } Set_Value("DocAction", DocAction);
        }/** Get Document Action.
@return The targeted status of the document */
        public String GetDocAction() { return (String)Get_Value("DocAction"); }
        /** DocStatus AD_Reference_ID=131 */
        public static int DOCSTATUS_AD_Reference_ID = 131;/** Unknown = ?? */
        public static String DOCSTATUS_Unknown = "??";/** Approved = AP */
        public static String DOCSTATUS_Approved = "AP";/** Closed = CL */
        public static String DOCSTATUS_Closed = "CL";/** Completed = CO */
        public static String DOCSTATUS_Completed = "CO";/** Drafted = DR */
        public static String DOCSTATUS_Drafted = "DR";/** Invalid = IN */
        public static String DOCSTATUS_Invalid = "IN";/** In Progress = IP */
        public static String DOCSTATUS_InProgress = "IP";/** Not Approved = NA */
        public static String DOCSTATUS_NotApproved = "NA";/** Reversed = RE */
        public static String DOCSTATUS_Reversed = "RE";/** Voided = VO */
        public static String DOCSTATUS_Voided = "VO";/** Waiting Confirmation = WC */
        public static String DOCSTATUS_WaitingConfirmation = "WC";/** Waiting Payment = WP */
        public static String DOCSTATUS_WaitingPayment = "WP";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsDocStatusValid(String test) { return test.Equals("??") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("DR") || test.Equals("IN") || test.Equals("IP") || test.Equals("NA") || test.Equals("RE") || test.Equals("VO") || test.Equals("WC") || test.Equals("WP"); }/** Set Document Status.
@param DocStatus The current status of the document */
        public void SetDocStatus(String DocStatus)
        {
            if (DocStatus == null) throw new ArgumentException("DocStatus is mandatory"); if (!IsDocStatusValid(DocStatus))
                throw new ArgumentException("DocStatus Invalid value - " + DocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP"); if (DocStatus.Length > 2) { log.Warning("Length > 2 - truncated"); DocStatus = DocStatus.Substring(0, 2); } Set_Value("DocStatus", DocStatus);
        }/** Get Document Status.
@return The current status of the document */
        public String GetDocStatus() { return (String)Get_Value("DocStatus"); }/** Set DocumentNo.
@param DocumentNo Document sequence number of the document */
        public void SetDocumentNo(String DocumentNo) { if (DocumentNo != null && DocumentNo.Length > 50) { log.Warning("Length > 50 - truncated"); DocumentNo = DocumentNo.Substring(0, 50); } Set_Value("DocumentNo", DocumentNo); }/** Get DocumentNo.
@return Document sequence number of the document */
        public String GetDocumentNo() { return (String)Get_Value("DocumentNo"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Approved.
@param IsApproved Indicates if this document requires approval */
        public void SetIsApproved(Boolean IsApproved) { Set_Value("IsApproved", IsApproved); }/** Get Approved.
@return Indicates if this document requires approval */
        public Boolean IsApproved() { Object oo = Get_Value("IsApproved"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
        /** PDCType AD_Reference_ID=1000499 */
        public static int PDCTYPE_AD_Reference_ID = 1000499;/** Direct PDC = D */
        public static String PDCTYPE_DirectPDC = "D";/** Normal PDC = N */
        public static String PDCTYPE_NormalPDC = "N";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsPDCTypeValid(String test) { return test == null || test.Equals("D") || test.Equals("N"); }/** Set PDC Type.
@param PDCType This field identifies the type of PDC */
        public void SetPDCType(String PDCType)
        {
            if (!IsPDCTypeValid(PDCType))
                throw new ArgumentException("PDCType Invalid value - " + PDCType + " - Reference_ID=1000499 - D - N"); if (PDCType != null && PDCType.Length > 1) { log.Warning("Length > 1 - truncated"); PDCType = PDCType.Substring(0, 1); } Set_Value("PDCType", PDCType);
        }/** Get PDC Type.
@return This field identifies the type of PDC */
        public String GetPDCType() { return (String)Get_Value("PDCType"); }/** Set Posted.
@param Posted Posting status */
        public void SetPosted(Boolean Posted) { Set_Value("Posted", Posted); }/** Get Posted.
@return Posting status */
        public Boolean IsPosted() { Object oo = Get_Value("Posted"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Processed.
@param Processed The document has been processed */
        public void SetProcessed(Boolean Processed) { Set_Value("Processed", Processed); }/** Get Processed.
@return The document has been processed */
        public Boolean IsProcessed() { Object oo = Get_Value("Processed"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Process Now.
@param Processing Process Now */
        public void SetProcessing(Boolean Processing) { Set_Value("Processing", Processing); }/** Get Process Now.
@return Process Now */
        public Boolean IsProcessing() { Object oo = Get_Value("Processing"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set TaxAmount.
@param TaxAmount TaxAmount */
        public void SetTaxAmount(Decimal? TaxAmount) { Set_Value("TaxAmount", (Decimal?)TaxAmount); }/** Get TaxAmount.
@return TaxAmount */
        public Decimal GetTaxAmount() { Object bd = Get_Value("TaxAmount"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Order Payment Schedule.
@param VA009_OrderPaySchedule_ID Order Payment Schedule */
        public void SetVA009_OrderPaySchedule_ID(int VA009_OrderPaySchedule_ID)
        {
            if (VA009_OrderPaySchedule_ID <= 0) Set_Value("VA009_OrderPaySchedule_ID", null);
            else
                Set_Value("VA009_OrderPaySchedule_ID", VA009_OrderPaySchedule_ID);
        }/** Get Order Payment Schedule.
@return Order Payment Schedule */
        public int GetVA009_OrderPaySchedule_ID() { Object ii = Get_Value("VA009_OrderPaySchedule_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Payment Method.
@param VA009_PaymentMethod_ID Payment Method */
        public void SetVA009_PaymentMethod_ID(int VA009_PaymentMethod_ID) { if (VA009_PaymentMethod_ID < 1) throw new ArgumentException("VA009_PaymentMethod_ID is mandatory."); Set_Value("VA009_PaymentMethod_ID", VA009_PaymentMethod_ID); }/** Get Payment Method.
@return Payment Method */
        public int GetVA009_PaymentMethod_ID() { Object ii = Get_Value("VA009_PaymentMethod_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Account Name.
@param VA027_AccountName Account Name */
        public void SetVA027_AccountName(String VA027_AccountName) { if (VA027_AccountName != null && VA027_AccountName.Length > 60) { log.Warning("Length > 60 - truncated"); VA027_AccountName = VA027_AccountName.Substring(0, 60); } Set_Value("VA027_AccountName", VA027_AccountName); }/** Get Account Name.
@return Account Name */
        public String GetVA027_AccountName() { return (String)Get_Value("VA027_AccountName"); }/** Set Account Number.
@param VA027_AccountNo Account Number */
        public void SetVA027_AccountNo(String VA027_AccountNo) { if (VA027_AccountNo != null && VA027_AccountNo.Length > 20) { log.Warning("Length > 20 - truncated"); VA027_AccountNo = VA027_AccountNo.Substring(0, 20); } Set_Value("VA027_AccountNo", VA027_AccountNo); }/** Get Account Number.
@return Account Number */
        public String GetVA027_AccountNo() { return (String)Get_Value("VA027_AccountNo"); }/** Set Cheque Date.
@param VA027_CheckDate Cheque Date */
        public void SetVA027_CheckDate(DateTime? VA027_CheckDate) { if (VA027_CheckDate == null) throw new ArgumentException("VA027_CheckDate is mandatory."); Set_Value("VA027_CheckDate", (DateTime?)VA027_CheckDate); }/** Get Cheque Date.
@return Cheque Date */
        public DateTime? GetVA027_CheckDate() { return (DateTime?)Get_Value("VA027_CheckDate"); }/** Set Cheque No.
@param VA027_CheckNo Cheque No */
        public void SetVA027_CheckNo(String VA027_CheckNo) { if (VA027_CheckNo == null) throw new ArgumentException("VA027_CheckNo is mandatory."); if (VA027_CheckNo.Length > 50) { log.Warning("Length > 50 - truncated"); VA027_CheckNo = VA027_CheckNo.Substring(0, 50); } Set_Value("VA027_CheckNo", VA027_CheckNo); }/** Get Cheque No.
@return Cheque No */
        public String GetVA027_CheckNo() { return (String)Get_Value("VA027_CheckNo"); }/** Set Converted Amount.
@param VA027_ConvertedAmount Converted Amount */
        public void SetVA027_ConvertedAmount(Decimal? VA027_ConvertedAmount) { Set_Value("VA027_ConvertedAmount", (Decimal?)VA027_ConvertedAmount); }/** Get Converted Amount.
@return Converted Amount */
        public Decimal GetVA027_ConvertedAmount() { Object bd = Get_Value("VA027_ConvertedAmount"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Description.
@param VA027_Description Description */
        public void SetVA027_Description(String VA027_Description) { if (VA027_Description != null && VA027_Description.Length > 100) { log.Warning("Length > 100 - truncated"); VA027_Description = VA027_Description.Substring(0, 100); } Set_Value("VA027_Description", VA027_Description); }/** Get Description.
@return Description */
        public String GetVA027_Description() { return (String)Get_Value("VA027_Description"); }/** Set Discount Amount.
@param VA027_DiscountAmt Discount Amount */
        public void SetVA027_DiscountAmt(Decimal? VA027_DiscountAmt) { Set_Value("VA027_DiscountAmt", (Decimal?)VA027_DiscountAmt); }/** Get Discount Amount.
@return Discount Amount */
        public Decimal GetVA027_DiscountAmt() { Object bd = Get_Value("VA027_DiscountAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Discounting PDC.
@param VA027_DiscountingPDC Discounting PDC */
        public void SetVA027_DiscountingPDC(Boolean VA027_DiscountingPDC) { Set_Value("VA027_DiscountingPDC", VA027_DiscountingPDC); }/** Get Discounting PDC.
@return Discounting PDC */
        public Boolean IsVA027_DiscountingPDC() { Object oo = Get_Value("VA027_DiscountingPDC"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Generate Payment.
@param VA027_GeneratePayment Generate Payment */
        public void SetVA027_GeneratePayment(String VA027_GeneratePayment) { if (VA027_GeneratePayment != null && VA027_GeneratePayment.Length > 1) { log.Warning("Length > 1 - truncated"); VA027_GeneratePayment = VA027_GeneratePayment.Substring(0, 1); } Set_Value("VA027_GeneratePayment", VA027_GeneratePayment); }/** Get Generate Payment.
@return Generate Payment */
        public String GetVA027_GeneratePayment() { return (String)Get_Value("VA027_GeneratePayment"); }/** Set Prepayment.
@param VA027_IsPrepayment Prepayment */
        public void SetVA027_IsPrepayment(Boolean VA027_IsPrepayment) { Set_Value("VA027_IsPrepayment", VA027_IsPrepayment); }/** Get Prepayment.
@return Prepayment */
        public Boolean IsVA027_IsPrepayment() { Object oo = Get_Value("VA027_IsPrepayment"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set MICR.
@param VA027_MICR MICR */
        public void SetVA027_MICR(String VA027_MICR) { if (VA027_MICR != null && VA027_MICR.Length > 100) { log.Warning("Length > 100 - truncated"); VA027_MICR = VA027_MICR.Substring(0, 100); } Set_Value("VA027_MICR", VA027_MICR); }/** Get MICR.
@return MICR */
        public String GetVA027_MICR() { return (String)Get_Value("VA027_MICR"); }/** Set Payment Amount.
@param VA027_PayAmt Payment Amount */
        public void SetVA027_PayAmt(Decimal? VA027_PayAmt) { if (VA027_PayAmt == null) throw new ArgumentException("VA027_PayAmt is mandatory."); Set_Value("VA027_PayAmt", (Decimal?)VA027_PayAmt); }/** Get Payment Amount.
@return Payment Amount */
        public Decimal GetVA027_PayAmt() { Object bd = Get_Value("VA027_PayAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Payment Generated.
@param VA027_PaymentGenerated Payment Generated */
        public void SetVA027_PaymentGenerated(Boolean VA027_PaymentGenerated) { Set_Value("VA027_PaymentGenerated", VA027_PaymentGenerated); }/** Get Payment Generated.
@return Payment Generated */
        public Boolean IsVA027_PaymentGenerated() { Object oo = Get_Value("VA027_PaymentGenerated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
        /** VA027_PaymentStatus AD_Reference_ID=1000491 */
        public static int VA027_PAYMENTSTATUS_AD_Reference_ID = 1000491;/** PDC Received = 0 */
        public static String VA027_PAYMENTSTATUS_PDCReceived = "0";/** Realization In-Progress = 1 */
        public static String VA027_PAYMENTSTATUS_RealizationIn_Progress = "1";/** PDC Realized = 2 */
        public static String VA027_PAYMENTSTATUS_PDCRealized = "2";/** Bounced = 3 */
        public static String VA027_PAYMENTSTATUS_Bounced = "3";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsVA027_PaymentStatusValid(String test) { return test == null || test.Equals("0") || test.Equals("1") || test.Equals("2") || test.Equals("3"); }/** Set Payment Status.
@param VA027_PaymentStatus Payment Status */
        public void SetVA027_PaymentStatus(String VA027_PaymentStatus)
        {
            if (!IsVA027_PaymentStatusValid(VA027_PaymentStatus))
                throw new ArgumentException("VA027_PaymentStatus Invalid value - " + VA027_PaymentStatus + " - Reference_ID=1000491 - 0 - 1 - 2 - 3"); if (VA027_PaymentStatus != null && VA027_PaymentStatus.Length > 1) { log.Warning("Length > 1 - truncated"); VA027_PaymentStatus = VA027_PaymentStatus.Substring(0, 1); } Set_Value("VA027_PaymentStatus", VA027_PaymentStatus);
        }/** Get Payment Status.
@return Payment Status */
        public String GetVA027_PaymentStatus() { return (String)Get_Value("VA027_PaymentStatus"); }/** Set VA027_PostDatedCheck_ID.
@param VA027_PostDatedCheck_ID VA027_PostDatedCheck_ID */
        public void SetVA027_PostDatedCheck_ID(int VA027_PostDatedCheck_ID) { if (VA027_PostDatedCheck_ID < 1) throw new ArgumentException("VA027_PostDatedCheck_ID is mandatory."); Set_ValueNoCheck("VA027_PostDatedCheck_ID", VA027_PostDatedCheck_ID); }/** Get VA027_PostDatedCheck_ID.
@return VA027_PostDatedCheck_ID */
        public int GetVA027_PostDatedCheck_ID() { Object ii = Get_Value("VA027_PostDatedCheck_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** VA027_Ref_Payment_ID AD_Reference_ID=1000492 */
        public static int VA027_REF_PAYMENT_ID_AD_Reference_ID = 1000492;/** Set Reference PDC.
@param VA027_Ref_Payment_ID Reference PDC */
        public void SetVA027_Ref_Payment_ID(int VA027_Ref_Payment_ID)
        {
            if (VA027_Ref_Payment_ID <= 0) Set_Value("VA027_Ref_Payment_ID", null);
            else
                Set_Value("VA027_Ref_Payment_ID", VA027_Ref_Payment_ID);
        }/** Get Reference PDC.
@return Reference PDC */
        public int GetVA027_Ref_Payment_ID() { Object ii = Get_Value("VA027_Ref_Payment_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Transaction Date.
@param VA027_TrxDate Transaction Date */
        public void SetVA027_TrxDate(DateTime? VA027_TrxDate) { if (VA027_TrxDate == null) throw new ArgumentException("VA027_TrxDate is mandatory."); Set_Value("VA027_TrxDate", (DateTime?)VA027_TrxDate); }/** Get Transaction Date.
@return Transaction Date */
        public DateTime? GetVA027_TrxDate() { return (DateTime?)Get_Value("VA027_TrxDate"); }/** Set Valid Month.
@param VA027_ValidMonth Valid Month */
        public void SetVA027_ValidMonth(int VA027_ValidMonth) { Set_Value("VA027_ValidMonth", VA027_ValidMonth); }/** Get Valid Month.
@return Valid Month */
        public int GetVA027_ValidMonth() { Object ii = Get_Value("VA027_ValidMonth"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Write Off Amount.
@param VA027_WriteoffAmt Write Off Amount */
        public void SetVA027_WriteoffAmt(Decimal? VA027_WriteoffAmt) { Set_Value("VA027_WriteoffAmt", (Decimal?)VA027_WriteoffAmt); }/** Get Write Off Amount.
@return Write Off Amount */
        public Decimal GetVA027_WriteoffAmt() { Object bd = Get_Value("VA027_WriteoffAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
        /** Set Multi Cheque.
@param VA027_MultiCheque Multi Cheque */
        public void SetVA027_MultiCheque(Boolean VA027_MultiCheque) { Set_Value("VA027_MultiCheque", VA027_MultiCheque); }/** Get Multi Cheque.
@return Multi Cheque */
        public Boolean IsVA027_MultiCheque() { Object oo = Get_Value("VA027_MultiCheque"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
        /** Set Payee.
@param VA027_Payee Payee */
        public void SetVA027_Payee(String VA027_Payee) { if (VA027_Payee != null && VA027_Payee.Length > 20) { log.Warning("Length > 20 - truncated"); VA027_Payee = VA027_Payee.Substring(0, 20); } Set_Value("VA027_Payee", VA027_Payee); }/** Get Payee.
@return Payee */
        public String GetVA027_Payee() { return (String)Get_Value("VA027_Payee"); }

        //Column Added by Arpit Rai on 16-Sept-2016 For GOM Module
        /** Set Cancel Check.
@param GOM01_CancelCheck Cancel Check */
        public void SetGOM01_CancelCheck(String GOM01_CancelCheck) { if (GOM01_CancelCheck != null && GOM01_CancelCheck.Length > 10) { log.Warning("Length > 10 - truncated"); GOM01_CancelCheck = GOM01_CancelCheck.Substring(0, 10); } Set_Value("GOM01_CancelCheck", GOM01_CancelCheck); }/** Get Cancel Check.
@return Cancel Check */
        public String GetGOM01_CancelCheck() { return (String)Get_Value("GOM01_CancelCheck"); }/** Set Print Check.
@param GOM01_PrintCheck Print Check */
        public void SetGOM01_PrintCheck(String GOM01_PrintCheck) { if (GOM01_PrintCheck != null && GOM01_PrintCheck.Length > 10) { log.Warning("Length > 10 - truncated"); GOM01_PrintCheck = GOM01_PrintCheck.Substring(0, 10); } Set_Value("GOM01_PrintCheck", GOM01_PrintCheck); }/** Get Print Check.
@return Print Check */
        public String GetGOM01_PrintCheck() { return (String)Get_Value("GOM01_PrintCheck"); }/** Set Check Printed.
@param GOM01_Printed Check Printed */
        public void SetGOM01_Printed(Boolean GOM01_Printed) { Set_Value("GOM01_Printed", GOM01_Printed); }/** Get Check Printed.
@return Check Printed */
        public Boolean IsGOM01_Printed() { Object oo = Get_Value("GOM01_Printed"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Re-Print Check.
@param GOM01_RePrintCheck Re-Print Check */
        public void SetGOM01_RePrintCheck(String GOM01_RePrintCheck) { if (GOM01_RePrintCheck != null && GOM01_RePrintCheck.Length > 10) { log.Warning("Length > 10 - truncated"); GOM01_RePrintCheck = GOM01_RePrintCheck.Substring(0, 10); } Set_Value("GOM01_RePrintCheck", GOM01_RePrintCheck); }/** Get Re-Print Check.
@return Re-Print Check */
        public String GetGOM01_RePrintCheck() { return (String)Get_Value("GOM01_RePrintCheck"); }
        //End Here Arpit Rai


        //Columns Added By Anuj on 06-May-2017 For GOM01 Module     
        /** Set Cancel Check.
@param VA009_CancelCheck Cancel Check */
        public void SetVA009_CancelCheck(String VA009_CancelCheck) { if (VA009_CancelCheck != null && VA009_CancelCheck.Length > 10) { log.Warning("Length > 10 - truncated"); VA009_CancelCheck = VA009_CancelCheck.Substring(0, 10); } Set_Value("VA009_CancelCheck", VA009_CancelCheck); }/** Get Cancel Check.
@return Cancel Check */
        public String GetVA009_CancelCheck() { return (String)Get_Value("VA009_CancelCheck"); }/** Set Print Check.
@param VA009_PrintCheck Print Check */
        public void SetVA009_PrintCheck(String VA009_PrintCheck) { if (VA009_PrintCheck != null && VA009_PrintCheck.Length > 10) { log.Warning("Length > 10 - truncated"); VA009_PrintCheck = VA009_PrintCheck.Substring(0, 10); } Set_Value("VA009_PrintCheck", VA009_PrintCheck); }/** Get Print Check.
@return Print Check */
        public String GetVA009_PrintCheck() { return (String)Get_Value("VA009_PrintCheck"); }/** Set Check Printed.
@param VA009_Printed Check Printed */
        public void SetVA009_Printed(Boolean VA009_Printed) { Set_Value("VA009_Printed", VA009_Printed); }/** Get Check Printed.
@return Check Printed */
        public Boolean IsVA009_Printed() { Object oo = Get_Value("VA009_Printed"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Re-Print Check.
@param VA009_RePrintCheck Re-Print Check */
        public void SetVA009_RePrintCheck(String VA009_RePrintCheck) { if (VA009_RePrintCheck != null && VA009_RePrintCheck.Length > 10) { log.Warning("Length > 10 - truncated"); VA009_RePrintCheck = VA009_RePrintCheck.Substring(0, 10); } Set_Value("VA009_RePrintCheck", VA009_RePrintCheck); }/** Get Re-Print Check.
@return Re-Print Check */
        public String GetVA009_RePrintCheck() { return (String)Get_Value("VA009_RePrintCheck"); }
        //End Here Anuj

    }
}