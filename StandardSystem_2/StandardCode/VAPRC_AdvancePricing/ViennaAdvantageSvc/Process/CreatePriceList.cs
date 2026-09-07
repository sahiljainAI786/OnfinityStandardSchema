/********************************************************
    * Project Name   : Advance Pricing (VAPRC)
    * Class Name     : CreatePriceList
    * Purpose        : Generate price list
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological    Development
    * Mohit     
******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using System.Data;
using VAdvantage.Classes;

namespace ViennaAdvantageServer.Process
{
    class CreatePriceList : SvrProcess
    {
        //Variables
        int _DiscountSchema_ID = 0, _BasePriceList_ID = 0, _M_DiscountSchemaLine_ID = 0;
        int _Precision = 0, _PriceList_ID = 0, AD_Org_ID = 0;
        decimal _ListFixed = 0, _StdFixed = 0, _LimitFixed = 0, _listAddAmt = 0, _StdAddAmt = 0, _LimitAddAmt = 0, _ListDiscount = 0, _StdDiscount = 0, _LimitDiscount = 0;
        string _ListRounding = "", _StdRounding = "", _LimitRounding = "", _ListBaseVal = "", _StdBaseVal = "", _LimitBaseVal = "", _Org_ID = "";
        string _IsListFormula = "", _IsStdFormula = "", _IsLimitFormula = "", _ListFormula = "", _StdFormula = "", _LimitFormula = "";
        StringBuilder _Sql = new StringBuilder();
        string _KeepPricesForPrevLot = "", Saved = "";
        List<int> ProductsExecuted = new List<int>();
        List<ProductAttributes> ProductsAttribute = new List<ProductAttributes>();
        ProductAttributes obj = new ProductAttributes();
        DataSet DsMainRecords = new DataSet();
        DataSet DsProductsPrice = new DataSet();
        DataSet DsOrgInfo = new DataSet();
        int _CountED011 = 0, _countFormula = 0;

        protected override string DoIt()
        {
            try
            {
                _CountED011 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='ED011_'"));
                _countFormula = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_Column WHERE ColumnName = 'IsListFormula' AND AD_Table_ID = 477"));
                // To get price list from price list version
                // To Get Precision value fromk price list

                //_Sql.Append("SELECT pl.priceprecision,pl.m_pricelist_id FROM m_pricelist pl INNER JOIN m_pricelist_version pv ON(pl.m_pricelist_id= pv.m_pricelist_id)"
                //+ "where pv.m_pricelist_version_id=" + _BasePriceList_ID);
                //DataSet ds = new DataSet();
                //ds = DB.ExecuteDataset(_Sql.ToString());
                //if (ds.Tables[0].Rows.Count > 0)
                //{
                //    _Precision = Util.GetValueOfInt(ds.Tables[0].Rows[0]["priceprecision"]);
                //    _PriceList_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["m_pricelist_id"]);
                //    ds.Dispose();
                //}

                //To Maintain All records from database into dataset
                _Sql.Clear();
                if (_countFormula > 0)
                {
                    _Sql.Append(" SELECT dsl.m_product_id,dsl.m_product_category_id,dsl.LIST_BASE,dsl.STD_BASE,dsl.LIMIT_BASE,dsl.List_Fixed,dsl.STD_FIXED,dsl.limit_fixed,"
                        + " dsl.List_AddAmt,dsl.std_addamt,dsl.limit_addamt,dsl.List_Discount,dsl.std_discount,dsl.limit_discount,dsl.list_rounding,dsl.std_rounding,"
                        + " dsl.limit_rounding,dsl.vaprc_pricesprevlot,M_DiscountSchemaline_ID,dsl.c_bpartner_id, dsl.IsListFormula,dsl.IsStdFormula,dsl.IsLimitFormula,"
                        + " dsl.ListFormula,dsl.StdFormula,dsl.LimitFormula FROM m_discountschema dsh INNER JOIN m_discountschemaline dsl ON"
                        + " (dsh.m_discountschema_id = dsl.m_discountschema_id) WHERE dsh.m_discountschema_id=" + _DiscountSchema_ID);
                }
                else
                {
                    _Sql.Append(" SELECT dsl.m_product_id,dsl.m_product_category_id,dsl.LIST_BASE,dsl.STD_BASE,dsl.LIMIT_BASE,dsl.List_Fixed,dsl.STD_FIXED,dsl.limit_fixed,"
                        + " dsl.List_AddAmt,dsl.std_addamt,dsl.limit_addamt,dsl.List_Discount,dsl.std_discount,dsl.limit_discount,dsl.list_rounding,dsl.std_rounding,"
                        + " dsl.limit_rounding,dsl.vaprc_pricesprevlot,M_DiscountSchemaline_ID,dsl.c_bpartner_id FROM m_discountschema dsh INNER JOIN m_discountschemaline dsl"
                        + " ON (dsh.m_discountschema_id= dsl.m_discountschema_id) WHERE dsh.m_discountschema_id=" + _DiscountSchema_ID);
                }
                DsMainRecords = DB.ExecuteDataset(_Sql.ToString());
                //Get Values From discountschemaline where product_id and Productcategory_id are null
                if (DsMainRecords.Tables[0].Rows.Count > 0)
                {
                    DataRow[] DRMaximumIddsl = DsMainRecords.Tables[0].Select("M_Product_Category_ID is null AND M_Product_ID is null AND C_BPartner_ID is null ", " M_DiscountSchemaline_ID DESC");
                    if (DRMaximumIddsl.Length > 0)
                    {
                        _ListBaseVal = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[2]);
                        _StdBaseVal = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[3]);
                        _LimitBaseVal = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[4]);
                        _ListFixed = Util.GetValueOfDecimal(DRMaximumIddsl[0].ItemArray[5]);
                        _StdFixed = Util.GetValueOfDecimal(DRMaximumIddsl[0].ItemArray[6]);
                        _LimitFixed = Util.GetValueOfDecimal(DRMaximumIddsl[0].ItemArray[7]);
                        _listAddAmt = Util.GetValueOfDecimal(DRMaximumIddsl[0].ItemArray[8]);
                        _StdAddAmt = Util.GetValueOfDecimal(DRMaximumIddsl[0].ItemArray[9]);
                        _LimitAddAmt = Util.GetValueOfDecimal(DRMaximumIddsl[0].ItemArray[10]);
                        _ListDiscount = Util.GetValueOfDecimal(DRMaximumIddsl[0].ItemArray[11]);
                        _StdDiscount = Util.GetValueOfDecimal(DRMaximumIddsl[0].ItemArray[12]);
                        _LimitDiscount = Util.GetValueOfDecimal(DRMaximumIddsl[0].ItemArray[13]);
                        _ListRounding = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[14]);
                        _StdRounding = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[15]);
                        _LimitRounding = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[16]);
                        _KeepPricesForPrevLot = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[17]);
                        _M_DiscountSchemaLine_ID = Util.GetValueOfInt(DRMaximumIddsl[0].ItemArray[18]);
                        if (_countFormula > 0)
                        {
                            _IsListFormula = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[20]);
                            _IsStdFormula = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[21]);
                            _IsLimitFormula = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[22]);
                            _ListFormula = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[23]);
                            _StdFormula = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[24]);
                            _LimitFormula = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[25]);
                        }
                    }
                    _Sql.Clear();
                    _Sql.Append("SELECT plv.ad_org_id,org.issummary FROM m_pricelist_version plv INNER JOIN ad_org org ON (plv.ad_org_id= org.ad_org_id)"
                          + "WHERE plv.m_pricelist_version_id=" + _BasePriceList_ID);

                    DsOrgInfo = DB.ExecuteDataset(_Sql.ToString());
                    if (DsOrgInfo.Tables[0].Rows.Count > 0)
                    {
                        AD_Org_ID = Util.GetValueOfInt(DsOrgInfo.Tables[0].Rows[0]["AD_Org_ID"]);
                        //if (AD_Org_ID == 0)
                        //{
                        //    return Msg.GetMsg(GetCtx(), "VAPRC_BasePListInSummaryLevelOrg");
                        //}
                        if (Util.GetValueOfString(DsOrgInfo.Tables[0].Rows[0]["issummary"]) == "N")
                        {
                            _Org_ID = Util.GetValueOfString(DsOrgInfo.Tables[0].Rows[0]["AD_Org_ID"]);
                        }
                        else
                        {
                            int _Ad_Tree_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Ad_Tree_ID from Ad_Tree WHERE treetype='OO' and ad_client_id=" + GetCtx().GetAD_Client_ID()));
                            if (_Ad_Tree_ID != 0)
                            {

                                _Sql.Clear();
                                _Sql.Append("SELECT Node_ID FROM AD_TreeNode WHERE AD_Tree_ID=" + _Ad_Tree_ID + " and parent_id=" + Util.GetValueOfString(DsOrgInfo.Tables[0].Rows[0]["AD_Org_ID"]));
                                IDataReader idr = null;
                                try
                                {
                                    idr = DB.ExecuteReader(_Sql.ToString());
                                    if (idr != null)
                                    {
                                        while (idr.Read())
                                        {
                                            _Org_ID = _Org_ID + idr.GetValue(0) + ",";
                                        }
                                        _Org_ID = _Org_ID.Trim(',');
                                        idr.Close();
                                    }

                                }
                                catch
                                {
                                    if (idr != null)
                                    {
                                        idr.Close();
                                        idr = null;
                                    }
                                }
                                finally
                                {
                                    if (idr != null)
                                    {
                                        idr.Close();
                                        idr = null;
                                    }
                                }
                            }
                        }
                    }

                }
                //Getting all the products from product price based on base price list
                // _Sql = " SELECT ppr.* FROM m_pricelist_version plv INNER JOIN m_productprice ppr  ON(plv.m_pricelist_version_id= ppr.m_pricelist_version_id)"
                //    + "WHERE plv.m_pricelist_version_id=" + _BasePriceList_ID + " ORDER BY m_product_id, m_attributesetinstance_id asc ";

                _Sql.Clear();
                if (_CountED011 > 0)
                {
                    _Sql.Append("SELECT ppr.AD_CLIENT_ID,ppr.AD_ORG_ID,ppr.M_PRICELIST_VERSION_ID,ppr.M_PRODUCT_ID,COALESCE(currencyConvert(ppr.PriceLimit,bpl.C_Currency_ID, pl.C_Currency_ID, dl.ConversionDate,"
                        + " dl.C_ConversionType_ID, plv.AD_Client_ID, plv.AD_Org_ID), 0) as PRICELIMIT,COALESCE(currencyConvert(ppr.PriceList,bpl.C_Currency_ID, pl.C_Currency_ID, dl.ConversionDate,"
                        + " dl.C_ConversionType_ID, plv.AD_Client_ID, plv.AD_Org_ID), 0) as PRICELIST,COALESCE(currencyConvert(ppr.PriceStd,bpl.C_Currency_ID, pl.C_Currency_ID, dl.ConversionDate,"
                        + " dl.C_ConversionType_ID, plv.AD_Client_ID, plv.AD_Org_ID), 0) as PRICESTD,ppr.M_ATTRIBUTESETINSTANCE_ID, ppr.LOT,nvl( po.c_bpartner_id,0) as Vendor, case when ppr.C_UOM_ID is null"
                        + " then p.C_UOM_ID else ppr.C_UOM_ID end as C_UOM_ID,ppr.PriceList AS base_PriceList, ppr.PriceStd AS base_PriceStd, ppr.PriceLimit AS base_PriceLimit"
                        +" FROM M_ProductPrice ppr INNER JOIN M_PriceList_Version plv ON plv.M_PriceList_Version_ID = ppr.M_PriceList_Version_ID"
                        + " INNER JOIN M_PriceList pl ON pl.M_PriceList_ID = " + _PriceList_ID + " INNER JOIN M_PriceList_Version bplv ON ppr.M_PriceList_Version_ID=bplv.M_PriceList_Version_ID"
                        + " INNER JOIN M_PriceList bpl ON bplv.M_PriceList_ID=bpl.M_PriceList_ID Inner JOIN M_product p ON p.M_product_id = ppr.M_product_id LEFT JOIN M_Product_PO po on (p.M_product_id=po.M_Product_ID "
                        + " and po.ISCURRENTVENDOR='Y') INNER JOIN M_DiscountSchemaLine dl ON dl.M_DiscountSchemaLine_ID=" + _M_DiscountSchemaLine_ID + " WHERE ppr.m_pricelist_version_id=" + _BasePriceList_ID + " ORDER BY M_product_id,  M_AttributeSetInstance_id ASC");
                }
                else
                {
                    _Sql.Append("SELECT ppr.AD_CLIENT_ID,ppr.AD_ORG_ID,ppr.M_PRICELIST_VERSION_ID,ppr.M_PRODUCT_ID,COALESCE(currencyConvert(ppr.PriceLimit,bpl.C_Currency_ID, pl.C_Currency_ID, dl.ConversionDate,"
                        + " dl.C_ConversionType_ID, plv.AD_Client_ID, plv.AD_Org_ID),0) as PRICELIMIT,COALESCE(currencyConvert(ppr.PriceList,bpl.C_Currency_ID, pl.C_Currency_ID, dl.ConversionDate,"
                        + " dl.C_ConversionType_ID, plv.AD_Client_ID, plv.AD_Org_ID), 0) as PRICELIST,COALESCE(currencyConvert(ppr.PriceStd,bpl.C_Currency_ID, pl.C_Currency_ID, dl.ConversionDate,"
                        + " dl.C_ConversionType_ID, plv.AD_Client_ID, plv.AD_Org_ID), 0) as PRICESTD,ppr.M_ATTRIBUTESETINSTANCE_ID,ppr.LOT,nvl( po.c_bpartner_id,0) as Vendor,"
                        + "ppr.PriceList AS base_PriceList, ppr.PriceStd AS base_PriceStd, ppr.PriceLimit AS base_PriceLimit " 
                        +" FROM M_ProductPrice ppr INNER JOIN M_PriceList_Version plv ON plv.M_PriceList_Version_ID = ppr.M_PriceList_Version_ID"
                        + " INNER JOIN M_PriceList pl ON pl.M_PriceList_ID = " + _PriceList_ID + " INNER JOIN M_PriceList_Version bplv ON ppr.M_PriceList_Version_ID=bplv.M_PriceList_Version_ID"
                        + " INNER JOIN M_PriceList bpl ON bplv.M_PriceList_ID=bpl.M_PriceList_ID Inner JOIN M_product p ON p.M_product_id = ppr.M_product_id LEFT JOIN M_Product_PO po on (p.M_product_id=po.M_Product_ID "
                        + " and po.ISCURRENTVENDOR='Y') INNER JOIN M_DiscountSchemaLine dl ON dl.M_DiscountSchemaLine_ID=" + _M_DiscountSchemaLine_ID + " WHERE ppr.m_pricelist_version_id=" + _BasePriceList_ID + " ORDER BY M_product_id,  M_AttributeSetInstance_id ASC");
                }


                DsProductsPrice = DB.ExecuteDataset(_Sql.ToString());
                _Sql.Clear();
                // Checking Discount schemaline for every product and productcategory
                MPriceList pl = new MPriceList(GetCtx(), _PriceList_ID, null);
                _Precision = pl.GetPricePrecision();
                if (DsProductsPrice != null)
                {
                    if (DsProductsPrice.Tables[0].Rows.Count > 0)
                    {                       
                        MPriceListVersion PriceListVersion = new MPriceListVersion(GetCtx(), 0, null);
                        PriceListVersion.SetAD_Org_ID(AD_Org_ID);
                        PriceListVersion.SetName(Util.GetValueOfString(System.DateTime.Now));
                        PriceListVersion.SetM_PriceList_ID(_PriceList_ID);
                        PriceListVersion.SetM_DiscountSchema_ID(_DiscountSchema_ID);
                        PriceListVersion.SetM_Pricelist_Version_Base_ID(_BasePriceList_ID);
                        if (PriceListVersion.Save())
                        {
                            for (int i = 0; i < DsProductsPrice.Tables[0].Rows.Count; i++)
                            {
                                //check the Conversion found or not, if not can't proceed forward
                                if ((Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PriceList"]) == 0 && Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["base_PriceList"]) != 0) ||
                                    (Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PriceStd"]) == 0 && Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["base_PriceStd"]) != 0) ||
                                    (Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PriceLimit"]) == 0 && Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["base_PriceLimit"]) != 0))
                                {
                                    //if Conversion not found then return a message
                                      return  Msg.GetMsg(GetCtx(), "VAPRC_ConversionOrPriceNotFound");

                                }
                                int C_UOM_ID = 0;
                                if (_CountED011 > 0)
                                {
                                    C_UOM_ID = Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["C_UOM_ID"]);
                                }
                                bool Status = false;
                                if (ProductsAttribute.Count > 0)
                                {
                                    for (int j = 0; j < ProductsAttribute.Count; j++)
                                    {
                                        if (_CountED011 > 0)
                                        {
                                            if (ProductsAttribute[j].ProductID == Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]) && ProductsAttribute[j].Attribute_ID == Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) && ProductsAttribute[j].UOM_ID == C_UOM_ID)
                                            {
                                                Status = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (ProductsAttribute[j].ProductID == Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]) && ProductsAttribute[j].Attribute_ID == Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]))
                                            {
                                                Status = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (Status == false)
                                {
                                    DataRow[] DRProductBased = DsMainRecords.Tables[0].Select("M_Product_ID=" + Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                    if (DRProductBased.Length > 0)
                                    {
                                        MProductPrice ProductPrice = new MProductPrice(GetCtx(), 0, null);
                                        ProductPrice.SetAD_Org_ID(AD_Org_ID);
                                        ProductPrice.SetM_Product_ID(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                        if (_countFormula > 0 && Util.GetValueOfString(DRProductBased[0].ItemArray[20]) == "Y")
                                        {
                                            ProductPrice.SetPriceList(Calculate(Util.GetValueOfString(DRProductBased[0].ItemArray[2]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                            Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                            Util.GetValueOfDecimal(DRProductBased[0].ItemArray[5]), Util.GetValueOfString(DRProductBased[0].ItemArray[23]),
                                            Util.GetValueOfDecimal(DRProductBased[0].ItemArray[8]), Util.GetValueOfString(DRProductBased[0].ItemArray[14]), _Precision));
                                        }
                                        else
                                        {
                                            ProductPrice.SetPriceList(Calculate(Util.GetValueOfString(DRProductBased[0].ItemArray[2]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                            Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                            Util.GetValueOfDecimal(DRProductBased[0].ItemArray[5]), Util.GetValueOfDecimal(DRProductBased[0].ItemArray[8]),
                                            Util.GetValueOfDecimal(DRProductBased[0].ItemArray[11]), Util.GetValueOfString(DRProductBased[0].ItemArray[14]), _Precision));
                                        }
                                        if (_countFormula > 0 && Util.GetValueOfString(DRProductBased[0].ItemArray[21]) == "Y")
                                        {
                                            ProductPrice.SetPriceStd(Calculate(Util.GetValueOfString(DRProductBased[0].ItemArray[3]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                            Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                            Util.GetValueOfDecimal(DRProductBased[0].ItemArray[6]), Util.GetValueOfString(DRProductBased[0].ItemArray[24]),
                                            Util.GetValueOfDecimal(DRProductBased[0].ItemArray[9]), Util.GetValueOfString(DRProductBased[0].ItemArray[15]), _Precision));
                                        }
                                        else
                                        {
                                            ProductPrice.SetPriceStd(Calculate(Util.GetValueOfString(DRProductBased[0].ItemArray[3]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                            Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                            Util.GetValueOfDecimal(DRProductBased[0].ItemArray[6]), Util.GetValueOfDecimal(DRProductBased[0].ItemArray[9]),
                                            Util.GetValueOfDecimal(DRProductBased[0].ItemArray[12]), Util.GetValueOfString(DRProductBased[0].ItemArray[15]), _Precision));
                                        }

                                        if (_countFormula > 0 && Util.GetValueOfString(DRProductBased[0].ItemArray[22]) == "Y")
                                        {
                                            ProductPrice.SetPriceLimit(Calculate(Util.GetValueOfString(DRProductBased[0].ItemArray[4]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                            Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                            Util.GetValueOfDecimal(DRProductBased[0].ItemArray[7]), Util.GetValueOfString(DRProductBased[0].ItemArray[25]),
                                            Util.GetValueOfDecimal(DRProductBased[0].ItemArray[10]), Util.GetValueOfString(DRProductBased[0].ItemArray[16]), _Precision));
                                        }
                                        else
                                        {
                                            ProductPrice.SetPriceLimit(Calculate(Util.GetValueOfString(DRProductBased[0].ItemArray[4]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                            Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                            Util.GetValueOfDecimal(DRProductBased[0].ItemArray[7]), Util.GetValueOfDecimal(DRProductBased[0].ItemArray[10]),
                                            Util.GetValueOfDecimal(DRProductBased[0].ItemArray[13]), Util.GetValueOfString(DRProductBased[0].ItemArray[16]), _Precision));
                                        }
                                        ProductPrice.SetM_PriceList_Version_ID(PriceListVersion.GetM_PriceList_Version_ID());
                                        if (_CountED011 > 0)
                                        {
                                            ProductPrice.SetC_UOM_ID(C_UOM_ID);
                                        }
                                        ProductPrice.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                        ProductPrice.SetLot(Util.GetValueOfString(DsProductsPrice.Tables[0].Rows[i]["LOT"]));
                                        if (ProductPrice.Save())
                                        {
                                            Saved = "Saved";
                                        }
                                        else
                                        {

                                        }
                                        if (Util.GetValueOfString(DRProductBased[0].ItemArray[17]) == "Y")
                                        {
                                            if (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) == 0)
                                            {
                                                KeepPricesForPreviousLots(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]), PriceListVersion.GetM_PriceList_Version_ID(), DsProductsPrice, C_UOM_ID);
                                            }

                                        }
                                        obj.ProductID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                        obj.Attribute_ID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                        if (_CountED011 > 0)
                                        {
                                            obj.UOM_ID = C_UOM_ID;
                                        }
                                        ProductsAttribute.Add(obj);
                                        continue;

                                    }
                                    else
                                    {

                                        if (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["Vendor"]) != 0)
                                        {
                                            DataRow[] DRVendorBased = DsMainRecords.Tables[0].Select("C_BPartner_ID=" + Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["Vendor"]));
                                            if (DRVendorBased.Length > 0)
                                            {
                                                MProductPrice ProductPrice = new MProductPrice(GetCtx(), 0, null);
                                                ProductPrice.SetAD_Org_ID(AD_Org_ID);
                                                if (_countFormula > 0 && Util.GetValueOfString(DRVendorBased[0].ItemArray[20]) == "Y")
                                                {
                                                    ProductPrice.SetPriceList(Calculate(Util.GetValueOfString(DRVendorBased[0].ItemArray[2]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                    Util.GetValueOfDecimal(DRVendorBased[0].ItemArray[5]), Util.GetValueOfString(DRVendorBased[0].ItemArray[23]),
                                                    Util.GetValueOfDecimal(DRVendorBased[0].ItemArray[8]), Util.GetValueOfString(DRVendorBased[0].ItemArray[14]), _Precision));
                                                }
                                                else
                                                {
                                                    ProductPrice.SetPriceList(Calculate(Util.GetValueOfString(DRVendorBased[0].ItemArray[2]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                    Util.GetValueOfDecimal(DRVendorBased[0].ItemArray[5]), Util.GetValueOfDecimal(DRVendorBased[0].ItemArray[8]),
                                                    Util.GetValueOfDecimal(DRVendorBased[0].ItemArray[11]), Util.GetValueOfString(DRVendorBased[0].ItemArray[14]), _Precision));
                                                }
                                                if (_countFormula > 0 && Util.GetValueOfString(DRVendorBased[0].ItemArray[21]) == "Y")
                                                {
                                                    ProductPrice.SetPriceStd(Calculate(Util.GetValueOfString(DRVendorBased[0].ItemArray[3]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                    Util.GetValueOfDecimal(DRVendorBased[0].ItemArray[6]), Util.GetValueOfString(DRVendorBased[0].ItemArray[24]),
                                                    Util.GetValueOfDecimal(DRVendorBased[0].ItemArray[9]), Util.GetValueOfString(DRVendorBased[0].ItemArray[15]), _Precision));
                                                }
                                                else
                                                {
                                                    ProductPrice.SetPriceStd(Calculate(Util.GetValueOfString(DRVendorBased[0].ItemArray[3]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                    Util.GetValueOfDecimal(DRVendorBased[0].ItemArray[6]), Util.GetValueOfDecimal(DRVendorBased[0].ItemArray[9]),
                                                    Util.GetValueOfDecimal(DRVendorBased[0].ItemArray[12]), Util.GetValueOfString(DRVendorBased[0].ItemArray[15]), _Precision));
                                                }
                                                if (_countFormula > 0 && Util.GetValueOfString(DRVendorBased[0].ItemArray[22]) == "Y")
                                                {
                                                    ProductPrice.SetPriceLimit(Calculate(Util.GetValueOfString(DRVendorBased[0].ItemArray[4]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                    Util.GetValueOfDecimal(DRVendorBased[0].ItemArray[7]), Util.GetValueOfString(DRVendorBased[0].ItemArray[25]),
                                                    Util.GetValueOfDecimal(DRVendorBased[0].ItemArray[10]), Util.GetValueOfString(DRVendorBased[0].ItemArray[16]), _Precision));
                                                }
                                                else
                                                {
                                                    ProductPrice.SetPriceLimit(Calculate(Util.GetValueOfString(DRVendorBased[0].ItemArray[4]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                    Util.GetValueOfDecimal(DRVendorBased[0].ItemArray[7]), Util.GetValueOfDecimal(DRVendorBased[0].ItemArray[10]),
                                                    Util.GetValueOfDecimal(DRVendorBased[0].ItemArray[13]), Util.GetValueOfString(DRVendorBased[0].ItemArray[16]), _Precision));
                                                }
                                                ProductPrice.SetM_PriceList_Version_ID(PriceListVersion.GetM_PriceList_Version_ID());
                                                if (_CountED011 > 0)
                                                {
                                                    ProductPrice.SetC_UOM_ID(C_UOM_ID);
                                                }
                                                ProductPrice.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                ProductPrice.SetLot(Util.GetValueOfString(DsProductsPrice.Tables[0].Rows[i]["LOT"]));
                                                if (ProductPrice.Save())
                                                {
                                                    Saved = "Saved";
                                                }
                                                else
                                                {

                                                }
                                                if (Util.GetValueOfString(DRVendorBased[0].ItemArray[17]) == "Y")
                                                {
                                                    if (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) == 0)
                                                    {
                                                        KeepPricesForPreviousLots(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                            Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]), PriceListVersion.GetM_PriceList_Version_ID(), DsProductsPrice, C_UOM_ID);
                                                    }
                                                }
                                                obj.ProductID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                                obj.Attribute_ID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                if (_CountED011 > 0)
                                                {
                                                    obj.UOM_ID = (C_UOM_ID);
                                                }
                                                ProductsAttribute.Add(obj);
                                                continue;
                                            }
                                            else
                                            {
                                                _Sql.Append("SELECT  M_Product_Category_ID FROM M_Product WHERE M_Product_ID=" + Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                                int _ProductCategory_ID = Util.GetValueOfInt(DB.ExecuteScalar(_Sql.ToString()));
                                                _Sql.Clear();
                                                DataRow[] DRProdCategoryBased = DsMainRecords.Tables[0].Select("M_Product_Category_ID=" + _ProductCategory_ID);
                                                if (DRProdCategoryBased.Length > 0)
                                                {
                                                    MProductPrice ProductPrice = new MProductPrice(GetCtx(), 0, null);
                                                    ProductPrice.SetAD_Org_ID(AD_Org_ID);
                                                    ProductPrice.SetM_Product_ID(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                                    if (_countFormula > 0 && Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[20]) == "Y")
                                                    {
                                                        ProductPrice.SetPriceList(Calculate(Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[2]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                        Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                        Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[5]), Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[23]),
                                                        Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[8]), Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[14]), _Precision));
                                                    }
                                                    else
                                                    {
                                                        ProductPrice.SetPriceList(Calculate(Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[2]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                        Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                        Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[5]), Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[8]),
                                                        Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[11]), Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[14]), _Precision));
                                                    }
                                                    if (_countFormula > 0 && Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[21]) == "Y")
                                                    {
                                                        ProductPrice.SetPriceStd(Calculate(Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[3]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                        Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                        Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[6]), Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[24]),
                                                        Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[9]), Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[15]), _Precision));
                                                    }
                                                    else
                                                    {
                                                        ProductPrice.SetPriceStd(Calculate(Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[3]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                        Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                        Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[6]), Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[9]),
                                                        Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[12]), Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[15]), _Precision));
                                                    }
                                                    if (_countFormula > 0 && Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[22]) == "Y")
                                                    {
                                                        ProductPrice.SetPriceLimit(Calculate(Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[4]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                        Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                        Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[7]), Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[25]),
                                                        Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[10]), Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[16]), _Precision));
                                                    }
                                                    else
                                                    {
                                                        ProductPrice.SetPriceLimit(Calculate(Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[4]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                        Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                        Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[7]), Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[10]),
                                                        Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[13]), Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[16]), _Precision));
                                                    }
                                                    ProductPrice.SetM_PriceList_Version_ID(PriceListVersion.GetM_PriceList_Version_ID());
                                                    if (_CountED011 > 0)
                                                    {
                                                        ProductPrice.SetC_UOM_ID(C_UOM_ID);
                                                    }
                                                    ProductPrice.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                    ProductPrice.SetLot(Util.GetValueOfString(DsProductsPrice.Tables[0].Rows[i]["LOT"]));
                                                    if (ProductPrice.Save())
                                                    {
                                                        Saved = "Saved";
                                                    }
                                                    else
                                                    {

                                                    }
                                                    if (Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[17]) == "Y")
                                                    {
                                                        if (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) == 0)
                                                        {
                                                            KeepPricesForPreviousLots(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                                Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]), PriceListVersion.GetM_PriceList_Version_ID(), DsProductsPrice, C_UOM_ID);
                                                        }
                                                    }
                                                    obj.ProductID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                                    obj.Attribute_ID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                    if (_CountED011 > 0)
                                                    {
                                                        obj.UOM_ID = (C_UOM_ID);
                                                    }
                                                    ProductsAttribute.Add(obj);
                                                    //ProductsExecuted.Add(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                                    //AttributesExecutes.Add(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                    continue;
                                                    //AttributesExecutes.Add(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                }

                                                else
                                                {
                                                    MProductPrice ProductPrice = new MProductPrice(GetCtx(), 0, null);
                                                    ProductPrice.SetAD_Org_ID(AD_Org_ID);
                                                    ProductPrice.SetM_Product_ID(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                                    if (_countFormula > 0 && _IsListFormula == "Y")
                                                    {
                                                        ProductPrice.SetPriceList(Calculate(_ListBaseVal, Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                        Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                        _ListFixed, _ListFormula, _listAddAmt, _ListRounding, _Precision));
                                                    }
                                                    else
                                                    {
                                                        ProductPrice.SetPriceList(Calculate(_ListBaseVal, Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                        Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                        _ListFixed, _listAddAmt, _ListDiscount, _ListRounding, _Precision));
                                                    }
                                                    if (_countFormula > 0 && _IsStdFormula == "Y")
                                                    {
                                                        ProductPrice.SetPriceStd(Calculate(_StdBaseVal, Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                        Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                        _StdFixed, _StdFormula, _StdAddAmt, _StdRounding, _Precision));
                                                    }
                                                    else
                                                    {
                                                        ProductPrice.SetPriceStd(Calculate(_StdBaseVal, Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                       Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                       _StdFixed, _StdAddAmt, _StdDiscount, _StdRounding, _Precision));
                                                    }
                                                    if (_countFormula > 0 && _IsLimitFormula == "Y")
                                                    {
                                                        ProductPrice.SetPriceLimit(Calculate(_LimitBaseVal, Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                        Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                        _LimitFixed, _LimitFormula, _LimitAddAmt, _LimitRounding, _Precision));
                                                    }
                                                    else
                                                    {
                                                        ProductPrice.SetPriceLimit(Calculate(_LimitBaseVal, Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                        Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                        _LimitFixed, _LimitAddAmt, _LimitDiscount, _LimitRounding, _Precision));
                                                    }
                                                    ProductPrice.SetM_PriceList_Version_ID(PriceListVersion.GetM_PriceList_Version_ID());
                                                    if (_CountED011 > 0)
                                                    {
                                                        ProductPrice.SetC_UOM_ID(C_UOM_ID);
                                                    }
                                                    ProductPrice.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                    ProductPrice.SetLot(Util.GetValueOfString(DsProductsPrice.Tables[0].Rows[i]["LOT"]));
                                                    if (ProductPrice.Save())
                                                    {
                                                        Saved = "Saved";
                                                    }
                                                    else
                                                    {

                                                    }
                                                    if (_KeepPricesForPrevLot == "Y")
                                                    {
                                                        if (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) == 0)
                                                        {
                                                            KeepPricesForPreviousLots(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                                Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]), PriceListVersion.GetM_PriceList_Version_ID(), DsProductsPrice, C_UOM_ID);
                                                        }
                                                    }
                                                    obj.ProductID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                                    obj.Attribute_ID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                    if (_CountED011 > 0)
                                                    {
                                                        obj.UOM_ID = (C_UOM_ID);
                                                    }
                                                    ProductsAttribute.Add(obj);
                                                    //ProductsExecuted.Add(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                                    //AttributesExecutes.Add(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                    continue;

                                                }
                                            }
                                        }
                                        else
                                        {
                                            _Sql.Append("SELECT  M_Product_Category_ID FROM M_Product WHERE M_Product_ID=" + Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                            int _ProductCategory_ID = Util.GetValueOfInt(DB.ExecuteScalar(_Sql.ToString()));
                                            _Sql.Clear();
                                            DataRow[] DRProdCategoryBased = DsMainRecords.Tables[0].Select("M_Product_Category_ID=" + _ProductCategory_ID);
                                            if (DRProdCategoryBased.Length > 0)
                                            {
                                                MProductPrice ProductPrice = new MProductPrice(GetCtx(), 0, null);
                                                ProductPrice.SetAD_Org_ID(AD_Org_ID);
                                                ProductPrice.SetM_Product_ID(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                                if (_countFormula > 0 && Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[20]) == "Y")
                                                {
                                                    ProductPrice.SetPriceList(Calculate(Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[2]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                    Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[5]), Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[23]),
                                                    Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[8]), Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[14]), _Precision));
                                                }
                                                else
                                                {
                                                    ProductPrice.SetPriceList(Calculate(Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[2]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                    Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[5]), Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[8]),
                                                    Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[11]), Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[14]), _Precision));
                                                }
                                                if (_countFormula > 0 && Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[21]) == "Y")
                                                {
                                                    ProductPrice.SetPriceStd(Calculate(Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[3]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                    Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[6]), Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[24]),
                                                    Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[9]), Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[15]), _Precision));
                                                }
                                                else
                                                {
                                                    ProductPrice.SetPriceStd(Calculate(Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[3]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                    Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[6]), Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[9]),
                                                    Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[12]), Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[15]), _Precision));
                                                }
                                                if (_countFormula > 0 && Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[22]) == "Y")
                                                {
                                                    ProductPrice.SetPriceLimit(Calculate(Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[4]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                    Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[7]), Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[25]),
                                                    Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[10]), Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[16]), _Precision));
                                                }
                                                else
                                                {
                                                    ProductPrice.SetPriceLimit(Calculate(Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[4]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                    Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[7]), Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[10]),
                                                    Util.GetValueOfDecimal(DRProdCategoryBased[0].ItemArray[13]), Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[16]), _Precision));
                                                }
                                                ProductPrice.SetM_PriceList_Version_ID(PriceListVersion.GetM_PriceList_Version_ID());
                                                if (_CountED011 > 0)
                                                {
                                                    ProductPrice.SetC_UOM_ID(C_UOM_ID);
                                                }
                                                ProductPrice.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                ProductPrice.SetLot(Util.GetValueOfString(DsProductsPrice.Tables[0].Rows[i]["LOT"]));
                                                if (ProductPrice.Save())
                                                {
                                                    Saved = "Saved";
                                                }
                                                else
                                                {

                                                }
                                                if (Util.GetValueOfString(DRProdCategoryBased[0].ItemArray[17]) == "Y")
                                                {
                                                    if (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) == 0)
                                                    {
                                                        KeepPricesForPreviousLots(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                            Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]), PriceListVersion.GetM_PriceList_Version_ID(), DsProductsPrice, C_UOM_ID);
                                                    }
                                                }
                                                obj.ProductID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                                obj.Attribute_ID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                if (_CountED011 > 0)
                                                {
                                                    obj.UOM_ID = C_UOM_ID;
                                                }
                                                ProductsAttribute.Add(obj);
                                                continue;
                                            }

                                            else
                                            {
                                                MProductPrice ProductPrice = new MProductPrice(GetCtx(), 0, null);
                                                ProductPrice.SetAD_Org_ID(AD_Org_ID);
                                                ProductPrice.SetM_Product_ID(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                                if (_countFormula > 0 && _IsListFormula == "Y")
                                                {
                                                    ProductPrice.SetPriceList(Calculate(_ListBaseVal, Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                    _ListFixed, _ListFormula, _listAddAmt, _ListRounding, _Precision));
                                                }
                                                else
                                                {
                                                    ProductPrice.SetPriceList(Calculate(_ListBaseVal, Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                    _ListFixed, _listAddAmt, _ListDiscount, _ListRounding, _Precision));
                                                }
                                                if (_countFormula > 0 && _IsStdFormula == "Y")
                                                {
                                                    ProductPrice.SetPriceStd(Calculate(_StdBaseVal, Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                    _StdFixed, _StdFormula, _StdAddAmt, _StdRounding, _Precision));
                                                }
                                                else
                                                {
                                                    ProductPrice.SetPriceStd(Calculate(_StdBaseVal, Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                    _StdFixed, _StdAddAmt, _StdDiscount, _StdRounding, _Precision));
                                                }
                                                if (_countFormula > 0 && _IsLimitFormula == "Y")
                                                {
                                                    ProductPrice.SetPriceLimit(Calculate(_LimitBaseVal, Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                    _LimitFixed, _LimitFormula, _LimitAddAmt, _LimitRounding, _Precision));
                                                }
                                                else
                                                {
                                                    ProductPrice.SetPriceLimit(Calculate(_LimitBaseVal, Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                    Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                    _LimitFixed, _LimitAddAmt, _LimitDiscount, _LimitRounding, _Precision));
                                                }
                                                ProductPrice.SetM_PriceList_Version_ID(PriceListVersion.GetM_PriceList_Version_ID());
                                                if (_CountED011 > 0)
                                                {
                                                    ProductPrice.SetC_UOM_ID(C_UOM_ID);
                                                }
                                                ProductPrice.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                ProductPrice.SetLot(Util.GetValueOfString(DsProductsPrice.Tables[0].Rows[i]["LOT"]));
                                                if (ProductPrice.Save())
                                                {
                                                    Saved = "Saved";
                                                }
                                                else
                                                {

                                                }
                                                if (_KeepPricesForPrevLot == "Y")
                                                {
                                                    if (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) == 0)
                                                    {
                                                        KeepPricesForPreviousLots(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                            Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]), PriceListVersion.GetM_PriceList_Version_ID(), DsProductsPrice, C_UOM_ID);
                                                    }
                                                }
                                                obj.ProductID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                                obj.Attribute_ID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                if (_CountED011 > 0)
                                                {
                                                    obj.UOM_ID = C_UOM_ID;
                                                }
                                                ProductsAttribute.Add(obj);
                                                continue;

                                            }
                                        }
                                    }
                                }
                            }
                            if (Saved.Length > 0)
                            {
                                DsProductsPrice.Dispose();
                                DsMainRecords.Dispose();
                                return Msg.GetMsg(GetCtx(), "VAPRC_PriceListCreatedSuccess");
                            }
                        }
                        else
                        {
                            DsProductsPrice.Dispose();
                            DsMainRecords.Dispose();
                            return Msg.GetMsg(GetCtx(), "VAPRC_PriceListVersionNotSaved");
                        }
                    }
                    else
                    {
                        //if no record found on ProductPrice against base Price List
                        //or on DiscountScmehaLine of Selected Discount Schema 
                        return Msg.GetMsg(GetCtx(),"VAPRC_DataNotFound");
                    }
                }
            }
            catch (Exception)
            {
                if (DsOrgInfo != null)
                {

                    DsOrgInfo.Dispose();
                }
                if (DsProductsPrice != null)
                {
                    DsProductsPrice.Dispose();
                }
                if (DsMainRecords != null)
                {
                    DsMainRecords.Dispose();
                }
            }
            finally
            {
                if (DsOrgInfo != null)
                {

                    DsOrgInfo.Dispose();
                }
                if (DsProductsPrice != null)
                {
                    DsProductsPrice.Dispose();
                }
                if (DsMainRecords != null)
                {
                    DsMainRecords.Dispose();
                }
            }
            return "";


        }

        //Previous Lot Based Entries
        public void KeepPricesForPreviousLots(int Product_id, decimal PriceList, decimal PriceStd, decimal PriceLimit, int PriceListVersion_ID, DataSet DsPPrice, int UOM_ID)
        {
            DataSet DsLotBased = new DataSet();
            try
            {
                _Sql.Append("SELECT t.* FROM (SELECT str.m_attributesetinstance_id AttributeSetInstance_ID, SUM(str.qtyonhand) AS SumTotal, asi.lot as LotNo FROM m_storage str "
                        + " INNER JOIN m_locator loc  ON (str.m_locator_id= loc.m_locator_id)  INNER JOIN m_warehouse whs ON(loc.m_warehouse_id= whs.m_warehouse_id) "
                        + " INNER JOIN m_attributesetinstance asi ON (asi.m_attributesetinstance_id=str.m_attributesetinstance_id) WHERE whs.ad_org_id  IN (" + _Org_ID + ") "
                        + " AND str.m_product_id  =" + Product_id + " AND str.m_attributesetinstance_id is not null GROUP BY str.m_attributesetinstance_id, asi.lot   order by str.m_attributesetinstance_id asc ) t  WHERE SumTotal>0 ");
                DsLotBased = DB.ExecuteDataset(_Sql.ToString());
                _Sql.Clear();
                if (DsLotBased != null)
                {
                    if (DsLotBased.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < DsLotBased.Tables[0].Rows.Count - 1; j++)
                        {
                            DataRow[] DRAttributeBased = DsPPrice.Tables[0].Select(" M_Product_ID=" + Product_id + " AND M_AttributeSetInstance_ID=" + Util.GetValueOfInt(DsLotBased.Tables[0].Rows[j]["AttributeSetInstance_ID"]) + "");
                            if (DRAttributeBased.Length > 0)
                            {

                                MProductPrice PPrice = new MProductPrice(GetCtx(), 0, null);
                                PPrice.SetAD_Org_ID(AD_Org_ID);
                                PPrice.SetM_Product_ID(Product_id);
                                PPrice.SetPriceList(Util.GetValueOfDecimal(DRAttributeBased[0].ItemArray[5]));
                                PPrice.SetPriceStd(Util.GetValueOfDecimal(DRAttributeBased[0].ItemArray[6]));
                                PPrice.SetPriceLimit(Util.GetValueOfDecimal(DRAttributeBased[0].ItemArray[4]));
                                int AttributeSetInstance_ID = Util.GetValueOfInt(DsLotBased.Tables[0].Rows[j]["AttributeSetInstance_ID"]);
                                if (AttributeSetInstance_ID != 0)
                                {
                                    PPrice.SetM_AttributeSetInstance_ID(AttributeSetInstance_ID);
                                }
                                else
                                {
                                    PPrice.SetM_AttributeSetInstance_ID(0);
                                }
                                if (_CountED011 > 0)
                                {
                                    PPrice.SetC_UOM_ID(Util.GetValueOfInt(DRAttributeBased[0].ItemArray[10]));
                                }
                                PPrice.SetLot(Util.GetValueOfString(DsLotBased.Tables[0].Rows[j]["LotNo"]));
                                PPrice.SetM_PriceList_Version_ID(PriceListVersion_ID);
                                if (PPrice.Save())
                                {
                                    Saved = "Saved";
                                }
                                else
                                {
                                }
                            }
                            else
                            {
                                MProductPrice PPrice = new MProductPrice(GetCtx(), 0, null);
                                PPrice.SetAD_Org_ID(AD_Org_ID);
                                PPrice.SetM_Product_ID(Product_id);
                                int AttributeSetInstance_ID = Util.GetValueOfInt(DsLotBased.Tables[0].Rows[j]["AttributeSetInstance_ID"]);
                                if (AttributeSetInstance_ID != 0)
                                {
                                    PPrice.SetM_AttributeSetInstance_ID(AttributeSetInstance_ID);
                                }
                                else
                                {
                                    PPrice.SetM_AttributeSetInstance_ID(0);
                                }
                                PPrice.SetPriceList(PriceList);
                                PPrice.SetPriceStd(PriceStd);
                                PPrice.SetPriceLimit(PriceLimit);
                                PPrice.SetLot(Util.GetValueOfString(DsLotBased.Tables[0].Rows[j]["LotNo"]));
                                if (_CountED011 > 0)
                                {
                                    PPrice.SetC_UOM_ID(UOM_ID);
                                }
                                PPrice.SetM_PriceList_Version_ID(PriceListVersion_ID);
                                if (PPrice.Save())
                                {
                                    Saved = "Saved";
                                }
                                else
                                {
                                }
                            }
                        }
                        DsLotBased.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                DsLotBased.Dispose();
            }

        }


        // Calculate Method
        private Decimal Calculate(String base1,
         Decimal list, Decimal std, Decimal limit, Decimal fix,
         Decimal add, Decimal discount, String round, int curPrecision)
        {
            Decimal? calc = null;
            double dd = 0.0;
            if (MDiscountSchemaLine.LIST_BASE_ListPrice.Equals(base1))
            {
                dd = Convert.ToDouble(list);//.doubleValue();
            }
            else if (MDiscountSchemaLine.LIST_BASE_StandardPrice.Equals(base1))
            {
                dd = Convert.ToDouble(std);//.doubleValue();
            }
            else if (MDiscountSchemaLine.LIST_BASE_LimitPOPrice.Equals(base1))
            {
                dd = Convert.ToDouble(limit);//.doubleValue();
            }
            else if (MDiscountSchemaLine.LIST_BASE_FixedPrice.Equals(base1))
            {
                calc = fix;
            }
            else
            {
                throw new Exception("Unknown Base=" + base1);
            }
            if (calc == null)
            {
                if (Env.Signum(add) != 0)
                {
                    dd += Convert.ToDouble(add);//.doubleValue();
                }
                if (Env.Signum(discount) != 0)
                {
                    dd *= 1 - (Convert.ToDouble(discount) / 100.0);
                }
                calc = new Decimal(dd);
            }
            //	Rounding
            if (MDiscountSchemaLine.LIST_ROUNDING_CurrencyPrecision.Equals(round))
            {
                calc = Decimal.Round(calc.Value, curPrecision, MidpointRounding.AwayFromZero);//calc.setScale(curPrecision, Decimal.ROUND_HALF_UP);
            }
            else if (MDiscountSchemaLine.LIST_ROUNDING_Dime102030.Equals(round))
            {
                calc = Decimal.Round(calc.Value, 1, MidpointRounding.AwayFromZero);//calc.setScale(1, Decimal.ROUND_HALF_UP);
            }
            else if (MDiscountSchemaLine.LIST_ROUNDING_Hundred.Equals(round))
            {
                calc = Decimal.Round(calc.Value, -2, MidpointRounding.AwayFromZero);//calc.setScale(-2, Decimal.ROUND_HALF_UP);
            }
            else if (MDiscountSchemaLine.LIST_ROUNDING_Nickel051015.Equals(round))
            {
                Decimal mm = new Decimal(20);
                calc = Decimal.Multiply(calc.Value, mm);
                calc = Decimal.Round(calc.Value, 0, MidpointRounding.AwayFromZero);//calc.setScale(0, Decimal.ROUND_HALF_UP);
                calc = Decimal.Round(Decimal.Divide(calc.Value, mm), 2, MidpointRounding.AwayFromZero);// Decimal.ROUND_HALF_UP);
            }
            else if (MDiscountSchemaLine.LIST_ROUNDING_NoRounding.Equals(round))
            {
                ;
            }
            else if (MDiscountSchemaLine.LIST_ROUNDING_Quarter255075.Equals(round))
            {
                Decimal mm = new Decimal(4);
                calc = Decimal.Multiply(calc.Value, mm);
                calc = Decimal.Round(calc.Value, 0, MidpointRounding.AwayFromZero);// calc.setScale(0, Decimal.ROUND_HALF_UP);
                calc = Decimal.Round(Decimal.Divide(calc.Value, mm), 2, MidpointRounding.AwayFromZero);// calc.divide(mm, 2, Decimal.ROUND_HALF_UP);
            }
            else if (MDiscountSchemaLine.LIST_ROUNDING_Ten10002000.Equals(round))
            {
                calc = Decimal.Round(calc.Value, -1, MidpointRounding.AwayFromZero);//calc.setScale(-1, Decimal.ROUND_HALF_UP);
            }
            else if (MDiscountSchemaLine.LIST_ROUNDING_Thousand.Equals(round))
            {
                calc = Decimal.Round(calc.Value, -3, MidpointRounding.AwayFromZero);//calc.setScale(-3, Decimal.ROUND_HALF_UP);
            }
            else if (MDiscountSchemaLine.LIST_ROUNDING_WholeNumber00.Equals(round))
            {
                calc = Decimal.Round(calc.Value, 0, MidpointRounding.AwayFromZero);//calc.setScale(0, Decimal.ROUND_HALF_UP);
            }

            return calc.Value;
        }

        // Calculate Method Formula Based
        private Decimal Calculate(String base1,
         Decimal list, Decimal std, Decimal limit, Decimal fix, string formula,
         Decimal add, String round, int curPrecision)
        {
            Decimal? calc = null;
            double dd = 0.0;
            if (MDiscountSchemaLine.LIST_BASE_ListPrice.Equals(base1))
            {
                dd = Convert.ToDouble(list);//.doubleValue();
            }
            else if (MDiscountSchemaLine.LIST_BASE_StandardPrice.Equals(base1))
            {
                dd = Convert.ToDouble(std);//.doubleValue();
            }
            else if (MDiscountSchemaLine.LIST_BASE_LimitPOPrice.Equals(base1))
            {
                dd = Convert.ToDouble(limit);//.doubleValue();
            }
            else if (MDiscountSchemaLine.LIST_BASE_FixedPrice.Equals(base1))
            {
                calc = fix;
            }
            else
            {
                throw new Exception("Unknown Base=" + base1);
            }
            if (calc == null)
            {
                if (Env.Signum(add) != 0)
                {
                    dd += Convert.ToDouble(add);//.doubleValue();
                }
                if (!String.IsNullOrEmpty(formula))
                {
                    List<String> operatorList = new List<String>();
                    List<String> operandList = new List<String>();
                    bool negStart = false;
                    if (formula.IndexOf("-") == 0)
                    {
                        negStart = true;
                    }
                    StringTokenizer st = new StringTokenizer(formula, "+-", true);
                    while (st.HasMoreTokens())
                    {
                        String token = st.NextToken();

                        if ("+-".Contains(token))
                        {
                            operatorList.Add(token);
                        }
                        else
                        {
                            operandList.Add(token);
                        }
                    }

                    if (operandList.Count > 0)
                    {
                        for (int i = 0; i < operandList.Count; i++)
                        {
                            if (negStart && i == 0)
                            {
                                dd *= 1 + (Convert.ToDouble(operandList[i]) / 100.0);
                            }
                            else if (i == 0)
                            {
                                dd *= 1 - (Convert.ToDouble(operandList[i]) / 100.0);
                            }
                            else if (negStart)
                            {
                                if (operatorList[i] == "+")
                                {
                                    dd *= 1 - (Convert.ToDouble(operandList[i]) / 100.0);
                                }
                                else
                                {
                                    dd *= 1 + (Convert.ToDouble(operandList[i]) / 100.0);
                                }
                            }
                            else
                            {
                                if (operatorList[i - 1] == "+")
                                {
                                    dd *= 1 - (Convert.ToDouble(operandList[i]) / 100.0);
                                }
                                else
                                {
                                    dd *= 1 + (Convert.ToDouble(operandList[i]) / 100.0);
                                }
                            }
                        }
                    }
                }

                calc = new Decimal(dd);
            }
            //	Rounding
            if (MDiscountSchemaLine.LIST_ROUNDING_CurrencyPrecision.Equals(round))
            {
                calc = Decimal.Round(calc.Value, curPrecision, MidpointRounding.AwayFromZero);//calc.setScale(curPrecision, Decimal.ROUND_HALF_UP);
            }
            else if (MDiscountSchemaLine.LIST_ROUNDING_Dime102030.Equals(round))
            {
                calc = Decimal.Round(calc.Value, 1, MidpointRounding.AwayFromZero);//calc.setScale(1, Decimal.ROUND_HALF_UP);
            }
            else if (MDiscountSchemaLine.LIST_ROUNDING_Hundred.Equals(round))
            {
                calc = Decimal.Round(calc.Value, -2, MidpointRounding.AwayFromZero);//calc.setScale(-2, Decimal.ROUND_HALF_UP);
            }
            else if (MDiscountSchemaLine.LIST_ROUNDING_Nickel051015.Equals(round))
            {
                Decimal mm = new Decimal(20);
                calc = Decimal.Multiply(calc.Value, mm);
                calc = Decimal.Round(calc.Value, 0, MidpointRounding.AwayFromZero);//calc.setScale(0, Decimal.ROUND_HALF_UP);
                calc = Decimal.Round(Decimal.Divide(calc.Value, mm), 2, MidpointRounding.AwayFromZero);// Decimal.ROUND_HALF_UP);
            }
            else if (MDiscountSchemaLine.LIST_ROUNDING_NoRounding.Equals(round))
            {
                ;
            }
            else if (MDiscountSchemaLine.LIST_ROUNDING_Quarter255075.Equals(round))
            {
                Decimal mm = new Decimal(4);
                calc = Decimal.Multiply(calc.Value, mm);
                calc = Decimal.Round(calc.Value, 0, MidpointRounding.AwayFromZero);// calc.setScale(0, Decimal.ROUND_HALF_UP);
                calc = Decimal.Round(Decimal.Divide(calc.Value, mm), 2, MidpointRounding.AwayFromZero);// calc.divide(mm, 2, Decimal.ROUND_HALF_UP);
            }
            else if (MDiscountSchemaLine.LIST_ROUNDING_Ten10002000.Equals(round))
            {
                calc = Decimal.Round(calc.Value, -1, MidpointRounding.AwayFromZero);//calc.setScale(-1, Decimal.ROUND_HALF_UP);
            }
            else if (MDiscountSchemaLine.LIST_ROUNDING_Thousand.Equals(round))
            {
                calc = Decimal.Round(calc.Value, -3, MidpointRounding.AwayFromZero);//calc.setScale(-3, Decimal.ROUND_HALF_UP);
            }
            else if (MDiscountSchemaLine.LIST_ROUNDING_WholeNumber00.Equals(round))
            {
                calc = Decimal.Round(calc.Value, 0, MidpointRounding.AwayFromZero);//calc.setScale(0, Decimal.ROUND_HALF_UP);
            }

            return calc.Value;
        }
        // Prepare Parameters
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
                else if (name.Equals("M_PriceList_ID"))
                {
                    _PriceList_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("M_DiscountSchema_ID"))
                {
                    _DiscountSchema_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("M_PriceList_Version_ID"))
                {
                    _BasePriceList_ID = para[i].GetParameterAsInt();
                }
            }
        }
        class ProductAttributes
        {
            int _Product_ID, _Attribute_ID, _UOM_ID;
            public int ProductID
            {
                get
                {
                    return this._Product_ID;
                }
                set
                {
                    this._Product_ID = value;
                }
            }
            public int Attribute_ID
            {
                get
                {
                    return this._Attribute_ID;
                }
                set
                {
                    this._Attribute_ID = value;
                }
            }
            public int UOM_ID
            {
                get
                {
                    return this._UOM_ID;
                }
                set
                {
                    this._UOM_ID = value;
                }
            }
        }
    }

}
