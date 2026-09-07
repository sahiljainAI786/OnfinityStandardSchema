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
    using System.Data;/** Generated Model for VA027_CheckAllocate
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VA027_CheckAllocate : PO
    {
        public X_VA027_CheckAllocate(Context ctx, int VA027_CheckAllocate_ID, Trx trxName) : base(ctx, VA027_CheckAllocate_ID, trxName)
        {/** if (VA027_CheckAllocate_ID == 0){SetAmount (0.0);SetC_InvoicePaySchedule_ID (0);SetC_Invoice_ID  (0);SetDiscountAmt (0.0);SetVA027_CheckAllocate_ID (0);SetVA027_ChequeDetails_ID (0);SetWriteOffAmt (0.0);} */
        }
        public X_VA027_CheckAllocate(Ctx ctx, int VA027_CheckAllocate_ID, Trx trxName) : base(ctx, VA027_CheckAllocate_ID, trxName)
        {/** if (VA027_CheckAllocate_ID == 0){SetAmount (0.0);SetC_InvoicePaySchedule_ID (0);SetC_Invoice_ID  (0);SetDiscountAmt (0.0);SetVA027_CheckAllocate_ID (0);SetVA027_ChequeDetails_ID (0);SetWriteOffAmt (0.0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VA027_CheckAllocate(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VA027_CheckAllocate(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VA027_CheckAllocate(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VA027_CheckAllocate() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27878150256352L;/** Last Updated Timestamp 7/29/2020 12:15:39 PM */
        public static long updatedMS = 1596024939563L;/** AD_Table_ID=1001656 */
        public static int Table_ID; // =1001656;
                                    /** TableName=VA027_CheckAllocate */
        public static String Table_Name = "VA027_CheckAllocate";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_VA027_CheckAllocate[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Amount.
@param Amount Amount in a defined currency */
        public void SetAmount(Decimal? Amount) { if (Amount == null) throw new ArgumentException("Amount is mandatory."); Set_Value("Amount", (Decimal?)Amount); }/** Get Amount.
@return Amount in a defined currency */
        public Decimal GetAmount() { Object bd = Get_Value("Amount"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Invoice Payment Schedule.
@param C_InvoicePaySchedule_ID Invoice Payment Schedule */
        public void SetC_InvoicePaySchedule_ID(int C_InvoicePaySchedule_ID) { if (C_InvoicePaySchedule_ID < 1) throw new ArgumentException("C_InvoicePaySchedule_ID is mandatory."); Set_Value("C_InvoicePaySchedule_ID", C_InvoicePaySchedule_ID); }/** Get Invoice Payment Schedule.
@return Invoice Payment Schedule */
        public int GetC_InvoicePaySchedule_ID() { Object ii = Get_Value("C_InvoicePaySchedule_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Invoice.
@param C_Invoice_ID  Invoice Identifier */
        public void SetC_Invoice_ID(int C_Invoice_ID) { Set_Value("C_Invoice_ID ", C_Invoice_ID); }/** Get Invoice.
@return Invoice Identifier */
        public int GetC_Invoice_ID() { Object ii = Get_Value("C_Invoice_ID "); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Discount Amount.
@param DiscountAmt Calculated amount of discount */
        public void SetDiscountAmt(Decimal? DiscountAmt) { if (DiscountAmt == null) throw new ArgumentException("DiscountAmt is mandatory."); Set_Value("DiscountAmt", (Decimal?)DiscountAmt); }/** Get Discount Amount.
@return Calculated amount of discount */
        public Decimal GetDiscountAmt() { Object bd = Get_Value("DiscountAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Invoice Amt.
@param InvoiceAmt Invoice Amt */
        public void SetInvoiceAmt(Decimal? InvoiceAmt) { Set_Value("InvoiceAmt", (Decimal?)InvoiceAmt); }/** Get Invoice Amt.
@return Invoice Amt */
        public Decimal GetInvoiceAmt() { Object bd = Get_Value("InvoiceAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set VA027_CheckAllocate_ID.
@param VA027_CheckAllocate_ID VA027_CheckAllocate_ID */
        public void SetVA027_CheckAllocate_ID(int VA027_CheckAllocate_ID) { if (VA027_CheckAllocate_ID < 1) throw new ArgumentException("VA027_CheckAllocate_ID is mandatory."); Set_ValueNoCheck("VA027_CheckAllocate_ID", VA027_CheckAllocate_ID); }/** Get VA027_CheckAllocate_ID.
@return VA027_CheckAllocate_ID */
        public int GetVA027_CheckAllocate_ID() { Object ii = Get_Value("VA027_CheckAllocate_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Cheque Detail .
@param VA027_ChequeDetails_ID The Cheque Detail is a unique identifier.  */
        public void SetVA027_ChequeDetails_ID(int VA027_ChequeDetails_ID) { if (VA027_ChequeDetails_ID < 1) throw new ArgumentException("VA027_ChequeDetails_ID is mandatory."); Set_ValueNoCheck("VA027_ChequeDetails_ID", VA027_ChequeDetails_ID); }/** Get Cheque Detail .
@return The Cheque Detail is a unique identifier.  */
        public int GetVA027_ChequeDetails_ID() { Object ii = Get_Value("VA027_ChequeDetails_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Write-off Amount.
@param WriteOffAmt Amount to write-off */
        public void SetWriteOffAmt(Decimal? WriteOffAmt) { if (WriteOffAmt == null) throw new ArgumentException("WriteOffAmt is mandatory."); Set_Value("WriteOffAmt", (Decimal?)WriteOffAmt); }/** Get Write-off Amount.
@return Amount to write-off */
        public Decimal GetWriteOffAmt() { Object bd = Get_Value("WriteOffAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
    }
}