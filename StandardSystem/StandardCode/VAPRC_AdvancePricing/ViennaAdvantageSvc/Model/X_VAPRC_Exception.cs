namespace ViennaAdvantageSvc.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for VAPRC_Exception
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAPRC_Exception : PO{public X_VAPRC_Exception (Context ctx, int VAPRC_Exception_ID, Trx trxName) : base (ctx, VAPRC_Exception_ID, trxName){/** if (VAPRC_Exception_ID == 0){SetM_DiscountSchemaLine_ID (0);SetVAPRC_Exception_ID (0);} */
}public X_VAPRC_Exception (Ctx ctx, int VAPRC_Exception_ID, Trx trxName) : base (ctx, VAPRC_Exception_ID, trxName){/** if (VAPRC_Exception_ID == 0){SetM_DiscountSchemaLine_ID (0);SetVAPRC_Exception_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPRC_Exception (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPRC_Exception (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPRC_Exception (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAPRC_Exception(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27797351432293L;/** Last Updated Timestamp 1/6/2018 1:38:35 PM */
public static long updatedMS = 1515226115504L;/** AD_Table_ID=1001472 */
public static int Table_ID; // =1001472;
/** TableName=VAPRC_Exception */
public static String Table_Name="VAPRC_Exception";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(6);/** AccessLevel
@return 6 - System - Client 
*/
protected override int Get_AccessLevel(){return Convert.ToInt32(accessLevel.ToString());}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Info
@return info
*/
public override String ToString(){StringBuilder sb = new StringBuilder ("X_VAPRC_Exception[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Business Partner.
@param C_BPartner_ID Identifies a Customer/Prospect */
public void SetC_BPartner_ID (int C_BPartner_ID){if (C_BPartner_ID <= 0) Set_Value ("C_BPartner_ID", null);else
Set_Value ("C_BPartner_ID", C_BPartner_ID);}/** Get Business Partner.
@return Identifies a Customer/Prospect */
public int GetC_BPartner_ID() {Object ii = Get_Value("C_BPartner_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Brand.
@param M_Brand_ID Brand */
public void SetM_Brand_ID (int M_Brand_ID){if (M_Brand_ID <= 0) Set_Value ("M_Brand_ID", null);else
Set_Value ("M_Brand_ID", M_Brand_ID);}/** Get Brand.
@return Brand */
public int GetM_Brand_ID() {Object ii = Get_Value("M_Brand_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Discount Pricelist.
@param M_DiscountSchemaLine_ID Line of the pricelist trade discount schema */
public void SetM_DiscountSchemaLine_ID (int M_DiscountSchemaLine_ID){if (M_DiscountSchemaLine_ID < 1) throw new ArgumentException ("M_DiscountSchemaLine_ID is mandatory.");Set_ValueNoCheck ("M_DiscountSchemaLine_ID", M_DiscountSchemaLine_ID);}/** Get Discount Pricelist.
@return Line of the pricelist trade discount schema */
public int GetM_DiscountSchemaLine_ID() {Object ii = Get_Value("M_DiscountSchemaLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Product Category.
@param M_Product_Category_ID Category of a Product */
public void SetM_Product_Category_ID (int M_Product_Category_ID){if (M_Product_Category_ID <= 0) Set_Value ("M_Product_Category_ID", null);else
Set_Value ("M_Product_Category_ID", M_Product_Category_ID);}/** Get Product Category.
@return Category of a Product */
public int GetM_Product_Category_ID() {Object ii = Get_Value("M_Product_Category_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID){if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);else
Set_Value ("M_Product_ID", M_Product_ID);}/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() {Object ii = Get_Value("M_Product_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set VAPRC_Exception_ID.
@param VAPRC_Exception_ID VAPRC_Exception_ID */
public void SetVAPRC_Exception_ID (int VAPRC_Exception_ID){if (VAPRC_Exception_ID < 1) throw new ArgumentException ("VAPRC_Exception_ID is mandatory.");Set_ValueNoCheck ("VAPRC_Exception_ID", VAPRC_Exception_ID);}/** Get VAPRC_Exception_ID.
@return VAPRC_Exception_ID */
public int GetVAPRC_Exception_ID() {Object ii = Get_Value("VAPRC_Exception_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** VAPRC_SumLevelProd AD_Reference_ID=1000599 */
public static int VAPRC_SUMLEVELPROD_AD_Reference_ID=1000599;/** Set Summary Level.
@param VAPRC_SumLevelProd Summary Level Products */
public void SetVAPRC_SumLevelProd (int VAPRC_SumLevelProd){Set_Value ("VAPRC_SumLevelProd", VAPRC_SumLevelProd);}/** Get Summary Level.
@return Summary Level Products */
public int GetVAPRC_SumLevelProd() {Object ii = Get_Value("VAPRC_SumLevelProd");if (ii == null) return 0;return Convert.ToInt32(ii);}}
}