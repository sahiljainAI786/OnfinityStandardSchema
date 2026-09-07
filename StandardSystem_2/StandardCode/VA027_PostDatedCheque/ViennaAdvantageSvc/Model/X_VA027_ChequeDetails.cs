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
    using System.Data;/** Generated Model for VA027_ChequeDetails
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VA027_ChequeDetails : PO
    {
        public X_VA027_ChequeDetails(Context ctx, int VA027_ChequeDetails_ID, Trx trxName) : base(ctx, VA027_ChequeDetails_ID, trxName)
        {/** if (VA027_ChequeDetails_ID == 0){SetLineNo (0.0);// @SQL=SELECT NVL(MAX(LineNo),0)+10 AS DefaultValue FROM VA027_ChequeDetails  WHERE VA027_PostDatedCheck_ID=@VA027_PostDatedCheck_ID@
SetVA027_CheckDate (DateTime.Now);SetVA027_ChequeAmount (0.0);SetVA027_ChequeDetails_ID (0);SetVA027_PostDatedCheck_ID (0);} */
        }
        public X_VA027_ChequeDetails(Ctx ctx, int VA027_ChequeDetails_ID, Trx trxName) : base(ctx, VA027_ChequeDetails_ID, trxName)
        {/** if (VA027_ChequeDetails_ID == 0){SetLineNo (0.0);// @SQL=SELECT NVL(MAX(LineNo),0)+10 AS DefaultValue FROM VA027_ChequeDetails  WHERE VA027_PostDatedCheck_ID=@VA027_PostDatedCheck_ID@
SetVA027_CheckDate (DateTime.Now);SetVA027_ChequeAmount (0.0);SetVA027_ChequeDetails_ID (0);SetVA027_PostDatedCheck_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VA027_ChequeDetails(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VA027_ChequeDetails(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VA027_ChequeDetails(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VA027_ChequeDetails() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27880546635004L;/** Last Updated Timestamp 8/26/2020 5:55:18 AM */
        public static long updatedMS = 1598421318215L;/** AD_Table_ID=1001272 */
        public static int Table_ID; // =1001272;
        /** TableName=VA027_ChequeDetails */
        public static String Table_Name = "VA027_ChequeDetails";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_VA027_ChequeDetails[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Payment.
@param C_Payment_ID Payment identifier */
        public void SetC_Payment_ID(int C_Payment_ID)
        {
            if (C_Payment_ID <= 0) Set_Value("C_Payment_ID", null);
            else
                Set_Value("C_Payment_ID", C_Payment_ID);
        }/** Get Payment.
@return Payment identifier */
        public int GetC_Payment_ID() { Object ii = Get_Value("C_Payment_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Line No.
@param LineNo Line No */
        public void SetLineNo(Decimal? LineNo) { if (LineNo == null) throw new ArgumentException("LineNo is mandatory."); Set_Value("LineNo", (Decimal?)LineNo); }/** Get Line No.
@return Line No */
        public Decimal GetLineNo() { Object bd = Get_Value("LineNo"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Processed.
@param Processed The document has been processed */
        public void SetProcessed(Boolean Processed) { Set_Value("Processed", Processed); }/** Get Processed.
@return The document has been processed */
        public Boolean IsProcessed() { Object oo = Get_Value("Processed"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Cancel Check.
@param VA009_CancelCheck Cancel Check */
        public void SetVA009_CancelCheck(String VA009_CancelCheck) { if (VA009_CancelCheck != null && VA009_CancelCheck.Length > 10) { log.Warning("Length > 10 - truncated"); VA009_CancelCheck = VA009_CancelCheck.Substring(0, 10); } Set_Value("VA009_CancelCheck", VA009_CancelCheck); }/** Get Cancel Check.
@return Cancel Check */
        public String GetVA009_CancelCheck() { return (String)Get_Value("VA009_CancelCheck"); }/** Set Cancelled.
@param VA009_IsCancelled Cancelled */
        public void SetVA009_IsCancelled(Boolean VA009_IsCancelled) { Set_Value("VA009_IsCancelled", VA009_IsCancelled); }/** Get Cancelled.
@return Cancelled */
        public Boolean IsVA009_IsCancelled() { Object oo = Get_Value("VA009_IsCancelled"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Print Check.
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
        public String GetVA009_RePrintCheck() { return (String)Get_Value("VA009_RePrintCheck"); }/** Set Account Name.
@param VA027_AccountName The Name of the Credit Card or Account holder */
        public void SetVA027_AccountName(String VA027_AccountName) { if (VA027_AccountName != null && VA027_AccountName.Length > 60) { log.Warning("Length > 60 - truncated"); VA027_AccountName = VA027_AccountName.Substring(0, 60); } Set_Value("VA027_AccountName", VA027_AccountName); }/** Get Account Name.
@return The Name of the Credit Card or Account holder */
        public String GetVA027_AccountName() { return (String)Get_Value("VA027_AccountName"); }/** Set Account Number.
@param VA027_AccountNo The Account Number indicates the Number assigned to this bank account. */
        public void SetVA027_AccountNo(String VA027_AccountNo) { if (VA027_AccountNo != null && VA027_AccountNo.Length > 20) { log.Warning("Length > 20 - truncated"); VA027_AccountNo = VA027_AccountNo.Substring(0, 20); } Set_Value("VA027_AccountNo", VA027_AccountNo); }/** Get Account Number.
@return The Account Number indicates the Number assigned to this bank account. */
        public String GetVA027_AccountNo() { return (String)Get_Value("VA027_AccountNo"); }/** Set Check Date.
@param VA027_CheckDate This field indicates the check printed date */
        public void SetVA027_CheckDate(DateTime? VA027_CheckDate) { if (VA027_CheckDate == null) throw new ArgumentException("VA027_CheckDate is mandatory."); Set_Value("VA027_CheckDate", (DateTime?)VA027_CheckDate); }/** Get Check Date.
@return This field indicates the check printed date */
        public DateTime? GetVA027_CheckDate() { return (DateTime?)Get_Value("VA027_CheckDate"); }/** Set Check No.
@param VA027_CheckNo The Check Number indicates the number on the check */
        public void SetVA027_CheckNo(String VA027_CheckNo) { if (VA027_CheckNo != null && VA027_CheckNo.Length > 50) { log.Warning("Length > 50 - truncated"); VA027_CheckNo = VA027_CheckNo.Substring(0, 50); } Set_Value("VA027_CheckNo", VA027_CheckNo); }/** Get Check No.
@return The Check Number indicates the number on the check */
        public String GetVA027_CheckNo() { return (String)Get_Value("VA027_CheckNo"); }/** Set Cheque Amount.
@param VA027_ChequeAmount  It indicates the amount, which is written on the check.  */
        public void SetVA027_ChequeAmount(Decimal? VA027_ChequeAmount) { if (VA027_ChequeAmount == null) throw new ArgumentException("VA027_ChequeAmount is mandatory."); Set_Value("VA027_ChequeAmount", (Decimal?)VA027_ChequeAmount); }/** Get Cheque Amount.
@return  It indicates the amount, which is written on the check.  */
        public Decimal GetVA027_ChequeAmount() { Object bd = Get_Value("VA027_ChequeAmount"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set VA027_ChequeDetails_ID.
@param VA027_ChequeDetails_ID VA027_ChequeDetails_ID */
        public void SetVA027_ChequeDetails_ID(int VA027_ChequeDetails_ID) { if (VA027_ChequeDetails_ID < 1) throw new ArgumentException("VA027_ChequeDetails_ID is mandatory."); Set_ValueNoCheck("VA027_ChequeDetails_ID", VA027_ChequeDetails_ID); }/** Get VA027_ChequeDetails_ID.
@return VA027_ChequeDetails_ID */
        public int GetVA027_ChequeDetails_ID() { Object ii = Get_Value("VA027_ChequeDetails_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Discounting PDC.
@param VA027_DiscountingPDC Discounting PDC */
        public void SetVA027_DiscountingPDC(Boolean VA027_DiscountingPDC) { Set_Value("VA027_DiscountingPDC", VA027_DiscountingPDC); }/** Get Discounting PDC.
@return Discounting PDC */
        public Boolean IsVA027_DiscountingPDC() { Object oo = Get_Value("VA027_DiscountingPDC"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set IsCheckAllocated.
@param VA027_IsCheckAllocated It indicates that data is available on allocate tab */
        public void SetVA027_IsCheckAllocated(Boolean VA027_IsCheckAllocated) { Set_Value("VA027_IsCheckAllocated", VA027_IsCheckAllocated); }/** Get IsCheckAllocated.
@return It indicates that data is available on allocate tab */
        public Boolean IsVA027_IsCheckAllocated() { Object oo = Get_Value("VA027_IsCheckAllocated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set MICR.
@param VA027_MICR The Micr number is the combination of the bank routing number, account number and check number */
        public void SetVA027_MICR(String VA027_MICR) { if (VA027_MICR != null && VA027_MICR.Length > 100) { log.Warning("Length > 100 - truncated"); VA027_MICR = VA027_MICR.Substring(0, 100); } Set_Value("VA027_MICR", VA027_MICR); }/** Get MICR.
@return The Micr number is the combination of the bank routing number, account number and check number */
        public String GetVA027_MICR() { return (String)Get_Value("VA027_MICR"); }
        /** VA027_PaymentStatus AD_Reference_ID=1000491 */
        public static int VA027_PAYMENTSTATUS_AD_Reference_ID = 1000491;/** PDC Received = 0 */
        public static String VA027_PAYMENTSTATUS_PDCReceived = "0";/** Realization In-Progress = 1 */
        public static String VA027_PAYMENTSTATUS_RealizationIn_Progress = "1";/** PDC Realized = 2 */
        public static String VA027_PAYMENTSTATUS_PDCRealized = "2";/** Bounced = 3 */
        public static String VA027_PAYMENTSTATUS_Bounced = "3";/** Replaced = 4 */
        public static String VA027_PAYMENTSTATUS_Replaced = "4";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsVA027_PaymentStatusValid(String test) { return test == null || test.Equals("0") || test.Equals("1") || test.Equals("2") || test.Equals("3") || test.Equals("4"); }/** Set Payment Status.
@param VA027_PaymentStatus It indicates the current status of the PDC payment., whether it is released/bounced/rejected */
        public void SetVA027_PaymentStatus(String VA027_PaymentStatus)
        {
            if (!IsVA027_PaymentStatusValid(VA027_PaymentStatus))
                throw new ArgumentException("VA027_PaymentStatus Invalid value - " + VA027_PaymentStatus + " - Reference_ID=1000491 - 0 - 1 - 2 - 3 - 4"); if (VA027_PaymentStatus != null && VA027_PaymentStatus.Length > 1) { log.Warning("Length > 1 - truncated"); VA027_PaymentStatus = VA027_PaymentStatus.Substring(0, 1); }
            Set_Value("VA027_PaymentStatus", VA027_PaymentStatus);
        }/** Get Payment Status.
@return It indicates the current status of the PDC payment., whether it is released/bounced/rejected */
        public String GetVA027_PaymentStatus() { return (String)Get_Value("VA027_PaymentStatus"); }/** Set Post Dated Cheque.
@param VA027_PostDatedCheck_ID Post Dated Cheque is a cheque written by the drawer (payer) for a date in the future. */
        public void SetVA027_PostDatedCheck_ID(int VA027_PostDatedCheck_ID) { if (VA027_PostDatedCheck_ID < 1) throw new ArgumentException("VA027_PostDatedCheck_ID is mandatory."); Set_ValueNoCheck("VA027_PostDatedCheck_ID", VA027_PostDatedCheck_ID); }/** Get Post Dated Cheque.
@return Post Dated Cheque is a cheque written by the drawer (payer) for a date in the future. */
        public int GetVA027_PostDatedCheck_ID() { Object ii = Get_Value("VA027_PostDatedCheck_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Valid Month.
@param VA027_ValidMonth This indicates from check validity of check from check date. */
        public void SetVA027_ValidMonth(int VA027_ValidMonth) { Set_Value("VA027_ValidMonth", VA027_ValidMonth); }/** Get Valid Month.
@return This indicates from check validity of check from check date. */
        public int GetVA027_ValidMonth() { Object ii = Get_Value("VA027_ValidMonth"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }
}