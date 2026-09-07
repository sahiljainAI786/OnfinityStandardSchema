/********************************************************
    * Project Name   : Vienna Advance Pricing (VAPRC)
    * Class Name     : VAPRC_CreatePriceList
    * Purpose        : Generate price list from button VAPRC
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological    Development
    * Mohit     
******************************************************/
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Logging;
using ViennaAdvantage.Model;

namespace ViennaAdvantageSvc.Process
{
    public class VAPRC_CreatePriceList
    {
        #region Variables
        int _DiscountSchema_ID = 0, _BasePriceList_ID = 0, _M_DiscountSchemaLine_ID = 0, _Record_ID = 0;
        int _Precision = 0, _PriceList_ID = 0, AD_Org_ID = 0, AD_Client_ID = 0;
        decimal _ListFixed = 0, _StdFixed = 0, _LimitFixed = 0, _listAddAmt = 0, _StdAddAmt = 0, _LimitAddAmt = 0, _ListDiscount = 0, _StdDiscount = 0, _LimitDiscount = 0;
        string _ListRounding = "", _StdRounding = "", _LimitRounding = "", _ListBaseVal = "", _StdBaseVal = "", _LimitBaseVal = "", _Org_ID = "";
        string _IsListFormula = "", _IsStdFormula = "", _IsLimitFormula = "", _ListFormula = "", _StdFormula = "", _LimitFormula = "";
        string _KeepPricesForPrevLot = "";
        //, Saved = "";
        List<int> ProductsExecuted = new List<int>();
        List<int> AttributesExecutes = new List<int>();
        List<ProductAttributes> ProductsAttribute = new List<ProductAttributes>();

        StringBuilder _Sql = new StringBuilder();
        DataSet DsMainRecords = new DataSet();
        DataSet DsProductsPrice = new DataSet();
        DataSet DsOrgInfo = new DataSet();
        int _CountED011 = 0, _countFormula = 0;
        public volatile bool completed = false;

        MDiscountSchemaLine _discountSchemaLine = null;
        decimal[] PriceArray = new decimal[3];
        //decimal convertedStdPrice = 0, convertedLimitprice = 0, convertedlistPrice = 0;

        public string _msg = "";
        List<string> _ListResult = new List<string>();

        //bool FlagProcessed = false;
        //bool FlagNotSaved = false;
        decimal PriceListAmt = 0, PriceStdAmt = 0, PriceLimitAmt = 0;
        StringBuilder _InsertSql = new StringBuilder();
        private static VLogger _log = VLogger.GetVLogger("VAPRC_CreatePriceList");

        //-----------variables for summary level work for price list----------------
        Dictionary<int, List<int>> ProdDict = new Dictionary<int, List<int>>();

        Dictionary<int, Dictionary<int, List<List<int>>>> ProdExcepDict = new Dictionary<int, Dictionary<int, List<List<int>>>>();
        Dictionary<int, List<List<int>>> ProdExcepDictSumLevel = new Dictionary<int, List<List<int>>>();



        StringBuilder parentIDs = new StringBuilder();
        StringBuilder sbSql = new StringBuilder();
        List<int> _DiscSchLine_IDs = new List<int>();
        DataSet _dsException = null;
        // bool InnerCheck, InnerExceptionCheck = false;
        //-----------variables for summary level work for price list Ends here----------------
        bool Skip = false;
        DataSet _dsVersionRecords = null;
        // New Functionality 
        List<PLDiscSchema> PlDiscScemaList = new List<PLDiscSchema>();
        List<int> OnlySumLevDiscSch = new List<int>();
        DataSet _DsSumLevel = new DataSet();
        bool IsExceptionFound = false;
        private Ctx ctx;
        private Trx trx;

        public VAPRC_CreatePriceList(Ctx ctx, Trx trx)
        {
            // TODO: Complete member initialization
            this.ctx = ctx;
            this.trx = trx;
        }

        #endregion

        public String CreatePricelist(int _Record_ID, string SkipDel, Ctx ctx, Trx trx)
        {
            ctx = this.ctx;
            trx = this.trx;
            try
            {
                String SkipDelCheck = SkipDel;
                GetVersionProducts(_Record_ID); //To get All Products information from Product price
                DB.ExecuteQuery("UPDATE M_PriceList_Version SET Processed='Y'  WHERE M_Pricelist_Version_ID= " + _Record_ID);

                _CountED011 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='ED011_'"));
                _countFormula = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_Column WHERE ColumnName = 'IsListFormula' AND AD_Table_ID = 477"));

                // To get price list from price list version
                #region Get ALL DISCOUNT SCHEMA from Price List Version
                MPriceListVersion plVersion = new MPriceListVersion(ctx, _Record_ID, trx); // Get Price List Version
                if (plVersion != null)
                {
                    AD_Client_ID = plVersion.GetAD_Client_ID();
                    //For the First Discounting schema
                    if (plVersion.GetM_DiscountSchema_ID() > 0 && plVersion.GetM_Pricelist_Version_Base_ID() > 0)
                    {
                        PLDiscSchema plds = new PLDiscSchema();
                        plds.DiscSchema_ID = Util.GetValueOfInt(plVersion.GetM_DiscountSchema_ID());
                        plds.PriceListVersion_ID = Util.GetValueOfInt(plVersion.GetM_Pricelist_Version_Base_ID());
                        PlDiscScemaList.Add(plds);
                    }
                    //For the Second Discounting Schema
                    if (Util.GetValueOfInt(plVersion.Get_Value("VAPRC_DiscSchema1")) > 0
                        && Util.GetValueOfInt(plVersion.Get_Value("VAPRC_BasePriceList1")) > 0)
                    {
                        PLDiscSchema plds = new PLDiscSchema();
                        plds.DiscSchema_ID = Util.GetValueOfInt(plVersion.Get_Value("VAPRC_DiscSchema1"));
                        plds.PriceListVersion_ID = Util.GetValueOfInt(plVersion.Get_Value("VAPRC_BasePriceList"));
                        PlDiscScemaList.Add(plds);
                    }
                    //For the Third Discounting Schema
                    if (Util.GetValueOfInt(plVersion.Get_Value("VAPRC_DiscSchema2")) > 0
                        && Util.GetValueOfInt(plVersion.Get_Value("VAPRC_BasePriceList2")) > 0)
                    {
                        PLDiscSchema plds = new PLDiscSchema();
                        plds.DiscSchema_ID = Util.GetValueOfInt(plVersion.Get_Value("VAPRC_DiscSchema2"));
                        plds.PriceListVersion_ID = Util.GetValueOfInt(plVersion.Get_Value("VAPRC_BasePriceList2"));
                        PlDiscScemaList.Add(plds);
                    }
                    //For the Fourth Discounting Schema
                    if (Util.GetValueOfInt(plVersion.Get_Value("VAPRC_DiscSchema3")) > 0
                        && Util.GetValueOfInt(plVersion.Get_Value("VAPRC_BasePriceList3")) > 0)
                    {
                        PLDiscSchema plds = new PLDiscSchema();
                        plds.DiscSchema_ID = Util.GetValueOfInt(plVersion.Get_Value("VAPRC_DiscSchema3"));
                        plds.PriceListVersion_ID = Util.GetValueOfInt(plVersion.Get_Value("VAPRC_BasePriceList3"));
                        PlDiscScemaList.Add(plds);
                    }
                    //For the Fifth Discounting Schema
                    if (Util.GetValueOfInt(plVersion.Get_Value("VAPRC_DiscSchema4")) > 0
                        && Util.GetValueOfInt(plVersion.Get_Value("VAPRC_BasePriceList4")) > 0)
                    {
                        PLDiscSchema plds = new PLDiscSchema();
                        plds.DiscSchema_ID = Util.GetValueOfInt(plVersion.Get_Value("VAPRC_DiscSchema4"));
                        plds.PriceListVersion_ID = Util.GetValueOfInt(plVersion.Get_Value("VAPRC_BasePriceList4"));
                        PlDiscScemaList.Add(plds);
                    }

                #endregion
                    #region Commented

                    //                _Sql.Append(@"SELECT AD_Client_ID, M_DISCOUNTSCHEMA_ID, M_PriceList_Version_Base_ID, VAPRC_BASEPRICELIST1, VAPRC_DISCSCHEMA1,  VAPRC_BASEPRICELIST2, VAPRC_DISCSCHEMA2,
                    //                        VAPRC_BASEPRICELIST3, VAPRC_DISCSCHEMA3, VAPRC_BASEPRICELIST4, VAPRC_DISCSCHEMA4 FROM M_PRICELIST_VERSION WHERE M_PRICELIST_VERSION_ID=" + _Record_ID);
                    //                DataSet DsHeaderValues = new DataSet(); // Get Price List Version 
                    //                DsHeaderValues = DB.ExecuteDataset(_Sql.ToString());
                    //                if (DsHeaderValues != null && DsHeaderValues.Tables[0].Rows.Count > 0)
                    //                {
                    //                    if (Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["M_DiscountSchema_ID"]) > 0 && Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["M_PriceList_Version_Base_ID"]) > 0)
                    //                    {
                    //                        PLDiscSchema plds = new PLDiscSchema();
                    //                        plds.DiscSchema_ID = Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["M_DiscountSchema_ID"]);
                    //                        plds.PriceListVersion_ID = Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["M_PriceList_Version_Base_ID"]);
                    //                        PlDiscScemaList.Add(plds);

                    //                    }
                    //                    if (Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["VAPRC_DISCSCHEMA1"]) > 0 && Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["VAPRC_BASEPRICELIST1"]) > 0)
                    //                    {
                    //                        PLDiscSchema plds = new PLDiscSchema();
                    //                        plds.DiscSchema_ID = Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["VAPRC_DISCSCHEMA1"]);
                    //                        plds.PriceListVersion_ID = Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["VAPRC_BASEPRICELIST1"]);
                    //                        PlDiscScemaList.Add(plds);

                    //                    }
                    //                    if (Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["VAPRC_DISCSCHEMA2"]) > 0 && Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["VAPRC_BASEPRICELIST2"]) > 0)
                    //                    {
                    //                        PLDiscSchema plds = new PLDiscSchema();
                    //                        plds.DiscSchema_ID = Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["VAPRC_DISCSCHEMA2"]);
                    //                        plds.PriceListVersion_ID = Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["VAPRC_BASEPRICELIST2"]);
                    //                        PlDiscScemaList.Add(plds);

                    //                    }
                    //                    if (Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["VAPRC_DISCSCHEMA3"]) > 0 && Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["VAPRC_BASEPRICELIST3"]) > 0)
                    //                    {
                    //                        PLDiscSchema plds = new PLDiscSchema();
                    //                        plds.DiscSchema_ID = Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["VAPRC_DISCSCHEMA3"]);
                    //                        plds.PriceListVersion_ID = Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["VAPRC_BASEPRICELIST3"]);
                    //                        PlDiscScemaList.Add(plds);

                    //                    }
                    //                    if (Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["VAPRC_DISCSCHEMA4"]) > 0 && Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["VAPRC_BASEPRICELIST4"]) > 0)
                    //                    {
                    //                        PLDiscSchema plds = new PLDiscSchema();
                    //                        plds.DiscSchema_ID = Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["VAPRC_DISCSCHEMA4"]);
                    //                        plds.PriceListVersion_ID = Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["VAPRC_BASEPRICELIST4"]);
                    //                        PlDiscScemaList.Add(plds);

                    //                    }
                    // AD_Client_ID = Util.GetValueOfInt(DsHeaderValues.Tables[0].Rows[0]["AD_Client_ID"]);

                    //DsHeaderValues.Dispose();

                    #endregion
                    if (PlDiscScemaList.Count > 0)
                    {
                        for (int step = 0; step < PlDiscScemaList.Count; step++)
                        {
                            _ListFixed = 0;
                            _StdFixed = 0;
                            _LimitFixed = 0;
                            _listAddAmt = 0;
                            _StdAddAmt = 0;
                            _LimitAddAmt = 0;
                            _ListDiscount = 0;
                            _StdDiscount = 0;
                            _LimitDiscount = 0;
                            _ListRounding = "";
                            _StdRounding = "";
                            _LimitRounding = "";
                            _ListBaseVal = "";
                            _StdBaseVal = "";
                            _LimitBaseVal = "";
                            _M_DiscountSchemaLine_ID = 0;
                            _IsListFormula = "";
                            _IsStdFormula = "";
                            _IsLimitFormula = "";
                            _ListFormula = "";
                            _StdFormula = "";
                            _LimitFormula = "";
                            _DiscountSchema_ID = PlDiscScemaList[step].DiscSchema_ID;
                            _BasePriceList_ID = PlDiscScemaList[step].PriceListVersion_ID;
                            _Sql.Clear();
                            // To Get Precision value from price list
                            _Sql.Append("SELECT pl.priceprecision,pl.m_pricelist_id FROM m_pricelist pl INNER JOIN m_pricelist_version pv ON(pl.m_pricelist_id= pv.m_pricelist_id)"
                            + "where pv.m_pricelist_version_id=" + _BasePriceList_ID);
                            DataSet ds = new DataSet();
                            ds = DB.ExecuteDataset(_Sql.ToString());
                            _Sql.Clear();
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                _Precision = Util.GetValueOfInt(ds.Tables[0].Rows[0]["priceprecision"]);
                                _PriceList_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["m_pricelist_id"]);
                                ds.Dispose();
                            }

                            //To Maintain All records from database into dataset

                            if (_countFormula > 0)
                            {
                                _Sql.Append(" SELECT dsl.m_product_id  ,  dsl.m_product_category_id,  dsl.LIST_BASE  ,  dsl.STD_BASE ,  dsl.LIMIT_BASE ,  dsl.List_Fixed,  dsl.STD_FIXED,"
                                    + "  dsl.limit_fixed, dsl.List_AddAmt,  dsl.std_addamt,  dsl.limit_addamt, dsl.List_Discount, dsl.std_discount,  dsl.limit_discount,  dsl.list_rounding,"
                                    + " dsl.std_rounding, dsl.limit_rounding,  dsl.vaprc_pricesprevlot ,M_DiscountSchemaline_ID,dsl.c_bpartner_id,dsl.M_Brand_ID,"
                                    + " dsl.IsListFormula,dsl.IsStdFormula,dsl.IsLimitFormula,dsl.ListFormula,dsl.StdFormula,dsl.LimitFormula,dsl.VAPRC_SumLevelProd FROM m_discountschema dsh"
                                    + " INNER JOIN m_discountschemaline dsl ON (dsh.m_discountschema_id = dsl.m_discountschema_id) WHERE dsh.m_discountschema_id=" + _DiscountSchema_ID);
                            }
                            else
                            {
                                _Sql.Append(" SELECT dsl.m_product_id,dsl.m_product_category_id,dsl.LIST_BASE,dsl.STD_BASE,dsl.LIMIT_BASE,dsl.List_Fixed,dsl.STD_FIXED,dsl.limit_fixed,"
                                    + " dsl.List_AddAmt, dsl.std_addamt, dsl.limit_addamt, dsl.List_Discount, dsl.std_discount,  dsl.limit_discount,  dsl.list_rounding,dsl.std_rounding,"
                                    + " dsl.limit_rounding, dsl.vaprc_pricesprevlot ,M_DiscountSchemaline_ID,dsl.c_bpartner_id,dsl.M_Brand_ID,dsl.VAPRC_SumLevelProd"
                                    + " FROM m_discountschema dsh INNER JOIN m_discountschemaline dsl ON (dsh.m_discountschema_id = dsl.m_discountschema_id) WHERE dsh.m_discountschema_id=" + _DiscountSchema_ID);
                            }
                            DsMainRecords = DB.ExecuteDataset(_Sql.ToString());
                            _Sql.Clear();
                            //Get Values From discountschemaline where product_id,vendor_id,brand_ID,Collection_ID and Productcategory_id are null
                            if (DsMainRecords != null)
                            {
                                if (DsMainRecords.Tables[0].Rows.Count > 0)
                                {
                                    DataRow[] DRMaximumIddsl = DsMainRecords.Tables[0].Select("M_Product_Category_ID is null AND M_Product_ID is null AND C_BPartner_ID is null AND M_Brand_ID is null AND VAPRC_SumLevelProd is null ", " M_DiscountSchemaline_ID DESC");
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
                                            _IsListFormula = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[21]);
                                            _IsStdFormula = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[22]);
                                            _IsLimitFormula = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[23]);
                                            _ListFormula = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[24]);
                                            _StdFormula = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[25]);
                                            _LimitFormula = Util.GetValueOfString(DRMaximumIddsl[0].ItemArray[26]);
                                        }
                                    }
                                    _Sql.Append("SELECT plv.ad_org_id,org.issummary FROM m_pricelist_version plv INNER JOIN ad_org org ON (plv.ad_org_id= org.ad_org_id)"
                                          + "WHERE plv.m_pricelist_version_id=" + _BasePriceList_ID);

                                    DsOrgInfo = DB.ExecuteDataset(_Sql.ToString());
                                    _Sql.Clear();
                                    if (DsOrgInfo.Tables[0].Rows.Count > 0)
                                    {
                                        AD_Org_ID = Util.GetValueOfInt(DsOrgInfo.Tables[0].Rows[0]["AD_Org_ID"]);

                                        if (Util.GetValueOfString(DsOrgInfo.Tables[0].Rows[0]["issummary"]) == "N")
                                        {
                                            _Org_ID = Util.GetValueOfString(DsOrgInfo.Tables[0].Rows[0]["AD_Org_ID"]);
                                        }
                                        else
                                        {
                                            int _Ad_Tree_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Ad_Tree_ID from Ad_Tree WHERE treetype='OO' and ad_client_id=" + Env.GetCtx().GetAD_Client_ID()));
                                            if (_Ad_Tree_ID != 0)
                                            {
                                                _Sql.Append("SELECT Node_ID FROM AD_TreeNode WHERE AD_Tree_ID=" + _Ad_Tree_ID + " and parent_id=" + Util.GetValueOfString(DsOrgInfo.Tables[0].Rows[0]["AD_Org_ID"]));
                                                IDataReader idr = null;
                                                try
                                                {
                                                    idr = DB.ExecuteReader(_Sql.ToString());
                                                    _Sql.Clear();
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

                                        DsOrgInfo.Dispose();
                                    }

                                }
                            }


                            if (_CountED011 > 0)
                            {
                                _Sql.Append(" SELECT t.AD_CLIENT_ID, t.AD_ORG_ID, t.basecurrency, t.C_Currency_ID, t.M_PRICELIST_VERSION_ID, t.M_PRODUCT_ID, t.M_ATTRIBUTESETINSTANCE_ID, t.LOT, t.Vendor,t.C_UOM_ID, t.M_Product_Category_ID ,t.DisSchema_ID,t.M_Brand_ID,"
                                + " CASE WHEN NVL(t.DisSchema_ID,0)>0 THEN VAPRC_GetPriceListException(t.M_PRODUCT_ID,NVL( t.Vendor,0),t.M_Product_Category_ID,t.M_Brand_ID,t.DisSchema_ID)"
                                + " ELSE 0 END AS EXCEPTION, CASE WHEN dl.ConversionDate IS NOT NULL AND dl.C_ConversionType_ID IS NOT NULL THEN COALESCE(currencyConvert(t.PriceLimit,t.basecurrency, t.C_Currency_ID, dl.ConversionDate,dl.C_ConversionType_ID, t.Client_ID, t.Org_ID), t.PriceLimit)"
                                + " ELSE t.PriceLimit END AS PRICELIMIT, CASE WHEN dl.ConversionDate IS NOT NULL AND dl.C_ConversionType_ID IS NOT NULL THEN COALESCE(currencyConvert(t.PriceList,t.basecurrency, t.C_Currency_ID, dl.ConversionDate,dl.C_ConversionType_ID, t.Client_ID, t.Org_ID), t.PriceList)"
                                + " ELSE t.PriceList END AS PRICELIST, CASE WHEN dl.ConversionDate IS NOT NULL AND dl.C_ConversionType_ID IS NOT NULL THEN COALESCE(currencyConvert(t.PriceStd,t.basecurrency, t.C_Currency_ID, dl.ConversionDate,dl.C_ConversionType_ID, t.Client_ID, t.Org_ID), t.PriceStd)"
                                + " ELSE t.PriceStd END AS PRICESTD FROM (SELECT ppr.AD_CLIENT_ID, ppr.AD_ORG_ID, plv.AD_Client_ID AS Client_ID, plv.AD_Org_ID AS Org_ID, bpl.C_Currency_ID AS basecurrency, pl.C_Currency_ID, ppr.M_PRICELIST_VERSION_ID, ppr.M_PRODUCT_ID,"
                                + " ppr.M_ATTRIBUTESETINSTANCE_ID, ppr.LOT, ppr.PriceLimit, ppr.PriceList, ppr.PriceStd, NVL( po.c_bpartner_id,0) AS Vendor, CASE WHEN ppr.C_UOM_ID IS NULL THEN p.C_UOM_ID ELSE ppr.C_UOM_ID END AS C_UOM_ID, p.M_Product_Category_ID,"
                                + " VAPRC_GetPriceListProduct(ppr.M_PRODUCT_ID,NVL( po.c_bpartner_id,0),p.M_Product_Category_ID,p.M_Brand_ID," + _DiscountSchema_ID + ") AS DisSchema_ID,"
                                + " p.M_Brand_ID FROM M_ProductPrice ppr INNER JOIN M_PriceList_Version plv ON plv.M_PriceList_Version_ID = " + _Record_ID
                                + " INNER JOIN M_PriceList pl ON pl.M_PriceList_ID=plv.M_PriceList_ID INNER JOIN M_PriceList_Version bplv ON ppr.M_PriceList_Version_ID=bplv.M_PriceList_Version_ID"
                                + " INNER JOIN M_PriceList bpl ON bplv.M_PriceList_ID=bpl.M_PriceList_ID INNER JOIN M_product p ON p.M_product_id = ppr.M_product_id LEFT JOIN M_Product_PO po ON (p.M_product_id = po.M_Product_ID"
                                + " AND po.ISCURRENTVENDOR='Y') LEFT JOIN M_DiscountSchemaLine dl ON dl.M_DiscountSchemaLine_ID = VAPRC_GetPriceListProduct(ppr.M_PRODUCT_ID,NVL( po.c_bpartner_id,0),p.M_Product_Category_ID,p.M_Brand_ID,"
                                + _DiscountSchema_ID + ") JOIN (SELECT row_num, AD_CLIENT_ID, AD_ORG_ID, basecurrency, M_PRICELIST_VERSION_ID, M_PRODUCT_ID, M_ATTRIBUTESETINSTANCE_ID FROM"
                                + " (SELECT prd.*, rownum AS row_num FROM (SELECT ppr.AD_CLIENT_ID, ppr.AD_ORG_ID, bpl.C_Currency_ID AS basecurrency, ppr.M_PRICELIST_VERSION_ID, ppr.M_PRODUCT_ID,"
                                + " ppr.M_ATTRIBUTESETINSTANCE_ID FROM M_ProductPrice ppr INNER JOIN M_PriceList_Version bplv ON ppr.M_PriceList_Version_ID=bplv.M_PriceList_Version_ID"
                                + " INNER JOIN M_PriceList bpl ON bplv.M_PriceList_ID=bpl.M_PriceList_ID WHERE ppr.m_pricelist_version_id = " + _BasePriceList_ID + " ORDER BY ppr.M_product_id, ppr.M_AttributeSetInstance_id ASC) prd)) pp"
                                + " ON pp.AD_CLIENT_ID = ppr.AD_CLIENT_ID AND pp.AD_ORG_ID = ppr.AD_ORG_ID AND pp.basecurrency = bpl.C_Currency_ID AND pp.M_PRICELIST_VERSION_ID =ppr.M_PRICELIST_VERSION_ID AND pp.M_PRODUCT_ID = ppr.M_PRODUCT_ID"
                                + " AND pp.M_ATTRIBUTESETINSTANCE_ID = ppr.M_ATTRIBUTESETINSTANCE_ID WHERE ppr.m_pricelist_version_id = " + _BasePriceList_ID + "ORDER BY row_num ) t LEFT JOIN M_DiscountSchemaLine dl ON dl.M_DiscountSchemaLine_ID = t.DisSchema_ID");
                            }
                            else
                            {
                                _Sql.Append(" SELECT t.AD_CLIENT_ID, t.AD_ORG_ID, t.basecurrency, t.C_Currency_ID, t.M_PRICELIST_VERSION_ID, t.M_PRODUCT_ID, t.M_ATTRIBUTESETINSTANCE_ID, t.LOT, t.Vendor,t.C_UOM_ID, t.M_Product_Category_ID ,t.DisSchema_ID,t.M_Brand_ID,"
                                + " CASE WHEN NVL(t.DisSchema_ID,0)>0 THEN VAPRC_GetPriceListException(t.M_PRODUCT_ID,NVL( t.Vendor,0),t.M_Product_Category_ID,t.M_Brand_ID,t.DisSchema_ID)"
                                + " ELSE 0 END AS EXCEPTION, CASE WHEN dl.ConversionDate IS NOT NULL AND dl.C_ConversionType_ID IS NOT NULL THEN COALESCE(currencyConvert(t.PriceLimit,t.basecurrency, t.C_Currency_ID, dl.ConversionDate,dl.C_ConversionType_ID, t.Client_ID, t.Org_ID), t.PriceLimit)"
                                + " ELSE t.PriceLimit END AS PRICELIMIT, CASE WHEN dl.ConversionDate IS NOT NULL AND dl.C_ConversionType_ID IS NOT NULL THEN COALESCE(currencyConvert(t.PriceList,t.basecurrency, t.C_Currency_ID, dl.ConversionDate,dl.C_ConversionType_ID, t.Client_ID, t.Org_ID), t.PriceList)"
                                + " ELSE t.PriceList END AS PRICELIST, CASE WHEN dl.ConversionDate IS NOT NULL AND dl.C_ConversionType_ID IS NOT NULL THEN COALESCE(currencyConvert(t.PriceStd,t.basecurrency, t.C_Currency_ID, dl.ConversionDate,dl.C_ConversionType_ID, t.Client_ID, t.Org_ID), t.PriceStd)"
                                + " ELSE t.PriceStd END AS PRICESTD FROM (SELECT ppr.AD_CLIENT_ID, ppr.AD_ORG_ID, plv.AD_Client_ID AS Client_ID, plv.AD_Org_ID AS Org_ID, bpl.C_Currency_ID AS basecurrency, pl.C_Currency_ID, ppr.M_PRICELIST_VERSION_ID, ppr.M_PRODUCT_ID,"
                                + " ppr.M_ATTRIBUTESETINSTANCE_ID, ppr.LOT, ppr.PriceLimit, ppr.PriceList, ppr.PriceStd, NVL( po.c_bpartner_id,0) AS Vendor, p.M_Product_Category_ID,"
                                + " VAPRC_GetPriceListProduct(ppr.M_PRODUCT_ID,NVL( po.c_bpartner_id,0),p.M_Product_Category_ID,p.M_Brand_ID" + _DiscountSchema_ID + ") AS DisSchema_ID,"
                                + " p.M_Brand_ID FROM M_ProductPrice ppr INNER JOIN M_PriceList_Version plv ON plv.M_PriceList_Version_ID = " + _Record_ID
                                + " INNER JOIN M_PriceList pl ON pl.M_PriceList_ID=plv.M_PriceList_ID INNER JOIN M_PriceList_Version bplv ON ppr.M_PriceList_Version_ID=bplv.M_PriceList_Version_ID"
                                + " INNER JOIN M_PriceList bpl ON bplv.M_PriceList_ID=bpl.M_PriceList_ID INNER JOIN M_product p ON p.M_product_id = ppr.M_product_id LEFT JOIN M_Product_PO po ON (p.M_product_id = po.M_Product_ID"
                                + " AND po.ISCURRENTVENDOR='Y') LEFT JOIN M_DiscountSchemaLine dl ON dl.M_DiscountSchemaLine_ID = VAPRC_GetPriceListProduct(ppr.M_PRODUCT_ID,NVL( po.c_bpartner_id,0),p.M_Product_Category_ID,p.M_Brand_ID,"
                                + _DiscountSchema_ID + ") JOIN (SELECT row_num, AD_CLIENT_ID, AD_ORG_ID, basecurrency, M_PRICELIST_VERSION_ID, M_PRODUCT_ID, M_ATTRIBUTESETINSTANCE_ID FROM"
                                + " (SELECT prd.*, rownum AS row_num FROM (SELECT ppr.AD_CLIENT_ID, ppr.AD_ORG_ID, bpl.C_Currency_ID AS basecurrency, ppr.M_PRICELIST_VERSION_ID, ppr.M_PRODUCT_ID,"
                                + " ppr.M_ATTRIBUTESETINSTANCE_ID FROM M_ProductPrice ppr INNER JOIN M_PriceList_Version bplv ON ppr.M_PriceList_Version_ID=bplv.M_PriceList_Version_ID"
                                + " INNER JOIN M_PriceList bpl ON bplv.M_PriceList_ID=bpl.M_PriceList_ID WHERE ppr.m_pricelist_version_id = " + _BasePriceList_ID + " ORDER BY ppr.M_product_id, ppr.M_AttributeSetInstance_id ASC) prd)) pp"
                                + " ON pp.AD_CLIENT_ID = ppr.AD_CLIENT_ID AND pp.AD_ORG_ID = ppr.AD_ORG_ID AND pp.basecurrency = bpl.C_Currency_ID AND pp.M_PRICELIST_VERSION_ID =ppr.M_PRICELIST_VERSION_ID AND pp.M_PRODUCT_ID = ppr.M_PRODUCT_ID"
                                + " AND pp.M_ATTRIBUTESETINSTANCE_ID = ppr.M_ATTRIBUTESETINSTANCE_ID WHERE ppr.m_pricelist_version_id = " + _BasePriceList_ID + "ORDER BY row_num ) t LEFT JOIN M_DiscountSchemaLine dl ON dl.M_DiscountSchemaLine_ID = t.DisSchema_ID");
                            }

                            DsProductsPrice = DB.ExecuteDataset(_Sql.ToString());
                            _Sql.Clear();

                            // Checking Discount schemaline for every product and productcategory

                            if (DsProductsPrice != null)
                            {
                                if (DsProductsPrice.Tables[0].Rows.Count > 0)
                                {
                                    try
                                    {
                                        GetExceptionData();
                                        CreateTreeArraySchema(ctx);
                                        CreateTreeArrayException(ctx);
                                        SummLevelDiscSchema();
                                        CreateTreeArrayExceptionSumLevel(ctx);
                                    }
                                    catch (Exception e)
                                    {
                                        DB.ExecuteQuery("UPDATE M_PriceList_Version SET Processed='N'  WHERE M_Pricelist_Version_ID= " + _Record_ID);
                                        return e.Message;
                                    }

                                    for (int i = 0; i < DsProductsPrice.Tables[0].Rows.Count; i++)
                                    {
                                        Skip = false;
                                        //To check whthr to skip product insertion or not
                                        if (SkipDelCheck == "S")
                                        {
                                            if (_dsVersionRecords != null && _dsVersionRecords.Tables[0].Rows.Count > 0)
                                            {
                                                Skip = CheckSkipStatus(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"])
                                                      , Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["C_UOM_ID"]));
                                            }

                                        }
                                        else if (SkipDelCheck == "U")
                                        {
                                            if (_dsVersionRecords != null && _dsVersionRecords.Tables[0].Rows.Count > 0)
                                            {
                                                Skip = CheckSkipStatus(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"])
                                                      , Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["C_UOM_ID"]));
                                                if (Skip && step == 0)
                                                {
                                                    Skip = false;
                                                }
                                            }
                                        }

                                        if (!Skip)
                                        {
                                            // InnerCheck = false;
                                            //InnerExceptionCheck = false;
                                            try
                                            {
                                                // if (!FlagProcessed)
                                                // {
                                                //    FlagProcessed = false;
                                                // }

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
                                                    int DiscSchema = 0;
                                                    DiscSchema = CheckProductDiscSchema(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]),
                                                        Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["DisSchema_ID"]),
                                                        Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["EXCEPTION"]), _Record_ID);
                                                    if (DiscSchema > 0 && Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["DisSchema_ID"]) > 0)
                                                    {

                                                        DataRow[] DRProductBased = DsMainRecords.Tables[0].Select("  M_DiscountSchemaline_ID=" + Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["DisSchema_ID"]));
                                                        if (DRProductBased.Length > 0)
                                                        {
                                                            if (_countFormula > 0)
                                                            {
                                                                if (SkipDelCheck == "U" && step == 0)
                                                                {
                                                                    DeleteExisting(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]), _Record_ID, C_UOM_ID);
                                                                }

                                                                if (SaveProdPrice(AD_Org_ID,
                                                                        Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]),
                                                                        _Record_ID,
                                                                        C_UOM_ID,
                                                                        Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]),
                                                                        Util.GetValueOfString(DsProductsPrice.Tables[0].Rows[i]["LOT"]),
                                                                        Util.GetValueOfString(DRProductBased[0].ItemArray[2]), //_ListBaseVal
                                                                        Util.GetValueOfString(DRProductBased[0].ItemArray[3]), //_StdBaseVal
                                                                        Util.GetValueOfString(DRProductBased[0].ItemArray[4]), //_LimitBaseVal
                                                                        Util.GetValueOfDecimal(DRProductBased[0].ItemArray[5]), //ListFixed
                                                                        Util.GetValueOfDecimal(DRProductBased[0].ItemArray[6]), // _StdFixed
                                                                        Util.GetValueOfDecimal(DRProductBased[0].ItemArray[7]), // _LimitFixed
                                                                        Util.GetValueOfDecimal(DRProductBased[0].ItemArray[8]), // _listAddAmt
                                                                        Util.GetValueOfDecimal(DRProductBased[0].ItemArray[9]), //_StdAddAmt 
                                                                        Util.GetValueOfDecimal(DRProductBased[0].ItemArray[10]), //_LimitAddAmt
                                                                        Util.GetValueOfDecimal(DRProductBased[0].ItemArray[11]), //_ListDiscount
                                                                        Util.GetValueOfDecimal(DRProductBased[0].ItemArray[12]), // _StdDiscount,
                                                                        Util.GetValueOfDecimal(DRProductBased[0].ItemArray[13]), //_LimitDiscount
                                                                        Util.GetValueOfString(DRProductBased[0].ItemArray[14]),  //_ListRounding
                                                                        Util.GetValueOfString(DRProductBased[0].ItemArray[15]),  // _StdRounding
                                                                        Util.GetValueOfString(DRProductBased[0].ItemArray[16]),  // _LimitRounding
                                                                        _Precision,
                                                                        Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                                        Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]),
                                                                        Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                                        Util.GetValueOfString(DRProductBased[0].ItemArray[21]),    //IsListFormula
                                                                        Util.GetValueOfString(DRProductBased[0].ItemArray[22]),    //_IsStdFormula
                                                                        Util.GetValueOfString(DRProductBased[0].ItemArray[23]),    //_IsLimitFormula,
                                                                        Util.GetValueOfString(DRProductBased[0].ItemArray[24]),    //_LstFormula
                                                                        Util.GetValueOfString(DRProductBased[0].ItemArray[25]),    //_StdFormula
                                                                        Util.GetValueOfString(DRProductBased[0].ItemArray[26]),    //_LimitFormula
                                                                        ctx,
                                                                        Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Brand_ID"]),
                                                                        Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_Category_ID"]), IsExceptionFound))
                                                                {
                                                                    if (Util.GetValueOfString(DRProductBased[0].ItemArray[17]) == "Y")
                                                                    {
                                                                        //if (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) == 0)
                                                                        if (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) > 0)
                                                                        {
                                                                            KeepPricesForPreviousLots(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                                                Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]), _Record_ID, DsProductsPrice, C_UOM_ID, ctx);
                                                                        }
                                                                    }
                                                                    ProductAttributes obj = new ProductAttributes();
                                                                    obj.ProductID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                                                    obj.Attribute_ID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                                    if (_CountED011 > 0)
                                                                    {
                                                                        obj.UOM_ID = C_UOM_ID;
                                                                    }
                                                                    ProductsAttribute.Add(obj);
                                                                    // FlagProcessed = true;
                                                                    continue;
                                                                }
                                                                else
                                                                {
                                                                    IsExceptionFound = false;
                                                                    // FlagNotSaved = true;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (SkipDelCheck == "U" && step == 0)
                                                                {
                                                                    DeleteExisting(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]), _Record_ID, C_UOM_ID);
                                                                }
                                                                if (SaveProdPrice(AD_Org_ID,
                                                                    Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]),
                                                                    _Record_ID,
                                                                    C_UOM_ID,
                                                                    Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]),
                                                                   Util.GetValueOfString(DsProductsPrice.Tables[0].Rows[i]["LOT"]),
                                                                   Util.GetValueOfString(DRProductBased[0].ItemArray[2]),
                                                                   Util.GetValueOfString(DRProductBased[0].ItemArray[3]),
                                                                   Util.GetValueOfString(DRProductBased[0].ItemArray[4]),
                                                                   Util.GetValueOfDecimal(DRProductBased[0].ItemArray[5]),
                                                                   Util.GetValueOfDecimal(DRProductBased[0].ItemArray[6]),
                                                                   Util.GetValueOfDecimal(DRProductBased[0].ItemArray[7]),
                                                                   Util.GetValueOfDecimal(DRProductBased[0].ItemArray[8]),
                                                                         Util.GetValueOfDecimal(DRProductBased[0].ItemArray[9]),
                                                                         Util.GetValueOfDecimal(DRProductBased[0].ItemArray[10]),
                                                                         Util.GetValueOfDecimal(DRProductBased[0].ItemArray[11]),
                                                                         Util.GetValueOfDecimal(DRProductBased[0].ItemArray[12]),
                                                                         Util.GetValueOfDecimal(DRProductBased[0].ItemArray[13]),
                                                                         Util.GetValueOfString(DRProductBased[0].ItemArray[14]),
                                                                         Util.GetValueOfString(DRProductBased[0].ItemArray[15]),
                                                                         Util.GetValueOfString(DRProductBased[0].ItemArray[16]),
                                                                         _Precision,
                                                                         Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                                         Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]),
                                                                         Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]),
                                                                         _IsListFormula,
                                                                         _IsStdFormula,
                                                                         _IsLimitFormula,
                                                                         _ListFormula,
                                                                         _StdFormula,
                                                                         _LimitFormula,
                                                                         ctx,
                                                                    Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Brand_ID"]),
                                                                    Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_Category_ID"]), IsExceptionFound))
                                                                {
                                                                    if (_KeepPricesForPrevLot == "Y")
                                                                    {
                                                                        //if (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) == 0)
                                                                        if (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) > 0)
                                                                        {
                                                                            KeepPricesForPreviousLots(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]),
                                                                                Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]), Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]), _Record_ID, DsProductsPrice, C_UOM_ID, ctx);
                                                                        }
                                                                    }
                                                                    ProductAttributes obj = new ProductAttributes();
                                                                    obj.ProductID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                                                    obj.Attribute_ID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                                    if (_CountED011 > 0)
                                                                    {
                                                                        obj.UOM_ID = (C_UOM_ID);
                                                                    }
                                                                    ProductsAttribute.Add(obj);
                                                                    //FlagProcessed = true;
                                                                    continue;
                                                                }
                                                                else
                                                                {
                                                                    IsExceptionFound = false;
                                                                    // FlagNotSaved = true;
                                                                }
                                                            }
                                                        }
                                                        // Deleted Non Discounted code
                                                    }
                                                    else if (DiscSchema > 0 && Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["DisSchema_ID"]) == 0)
                                                    {

                                                        _discountSchemaLine = new MDiscountSchemaLine(ctx, DiscSchema, null);

                                                        PriceArray[0] = Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]);
                                                        PriceArray[1] = Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]);
                                                        PriceArray[2] = Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]);

                                                        PriceArray = CurrencyConvert(ctx, PriceArray, _discountSchemaLine, Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["basecurrency"]),
                                                            Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["C_Currency_ID"]), AD_Client_ID, AD_Org_ID);


                                                        DataRow[] DRProductBased = DsMainRecords.Tables[0].Select("  M_DiscountSchemaline_ID=" + DiscSchema);
                                                        if (DRProductBased.Length > 0)
                                                        {
                                                            if (_countFormula > 0)
                                                            {
                                                                if (SkipDelCheck == "U" && step == 0)
                                                                {
                                                                    DeleteExisting(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]), _Record_ID, C_UOM_ID);
                                                                }
                                                                if (SaveProdPrice(AD_Org_ID,
                                                                    Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]),
                                                                    _Record_ID,
                                                                    C_UOM_ID,
                                                                    Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]),
                                                                    Util.GetValueOfString(DsProductsPrice.Tables[0].Rows[i]["LOT"]),
                                                                    Util.GetValueOfString(DRProductBased[0].ItemArray[2]),
                                                                    Util.GetValueOfString(DRProductBased[0].ItemArray[3]),
                                                                    Util.GetValueOfString(DRProductBased[0].ItemArray[4]),
                                                                    Util.GetValueOfDecimal(DRProductBased[0].ItemArray[5]),
                                                                    Util.GetValueOfDecimal(DRProductBased[0].ItemArray[6]),
                                                                    Util.GetValueOfDecimal(DRProductBased[0].ItemArray[7]),
                                                                    Util.GetValueOfDecimal(DRProductBased[0].ItemArray[8]),
                                                                    Util.GetValueOfDecimal(DRProductBased[0].ItemArray[9]),
                                                                    Util.GetValueOfDecimal(DRProductBased[0].ItemArray[10]),
                                                                    Util.GetValueOfDecimal(DRProductBased[0].ItemArray[11]),
                                                                    Util.GetValueOfDecimal(DRProductBased[0].ItemArray[12]),
                                                                    Util.GetValueOfDecimal(DRProductBased[0].ItemArray[13]),
                                                                    Util.GetValueOfString(DRProductBased[0].ItemArray[14]),
                                                                    Util.GetValueOfString(DRProductBased[0].ItemArray[15]),
                                                                    Util.GetValueOfString(DRProductBased[0].ItemArray[16]),
                                                                    _Precision,
                                                                    PriceArray[0],
                                                                    PriceArray[1],
                                                                    PriceArray[2],
                                                                        Util.GetValueOfString(DRProductBased[0].ItemArray[21]),
                                                                        Util.GetValueOfString(DRProductBased[0].ItemArray[22]),
                                                                        Util.GetValueOfString(DRProductBased[0].ItemArray[23]),
                                                                        Util.GetValueOfString(DRProductBased[0].ItemArray[24]),
                                                                        Util.GetValueOfString(DRProductBased[0].ItemArray[25]),
                                                                        Util.GetValueOfString(DRProductBased[0].ItemArray[26]),
                                                                        ctx,
                                                                        Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Brand_ID"]),
                                                                        Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_Category_ID"]), IsExceptionFound)
                                                                    )
                                                                {
                                                                    if (Util.GetValueOfString(DRProductBased[0].ItemArray[17]) == "Y")
                                                                    {
                                                                        if (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) > 0)
                                                                        //if (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) == 0)
                                                                        {
                                                                            KeepPricesForPreviousLots(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]), PriceArray[0],
                                                                                PriceArray[1], PriceArray[2], _Record_ID, DsProductsPrice, C_UOM_ID, ctx);
                                                                        }
                                                                    }
                                                                    ProductAttributes obj = new ProductAttributes();
                                                                    obj.ProductID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                                                    obj.Attribute_ID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                                    if (_CountED011 > 0)
                                                                    {
                                                                        obj.UOM_ID = C_UOM_ID;
                                                                    }
                                                                    ProductsAttribute.Add(obj);
                                                                    //FlagProcessed = true;
                                                                    continue;
                                                                }
                                                                else
                                                                {
                                                                    _msg = Msg.GetMsg(ctx, "VAPRC_LineNotSaved");
                                                                    // FlagNotSaved = true;
                                                                    return _msg;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (SkipDelCheck == "U" && step == 0)
                                                                {
                                                                    DeleteExisting(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]), _Record_ID, C_UOM_ID);
                                                                }
                                                                if (SaveProdPrice(AD_Org_ID,
                                                                    Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]),
                                                                    _Record_ID,
                                                                    C_UOM_ID,
                                                                    Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]),
                                                                   Util.GetValueOfString(DsProductsPrice.Tables[0].Rows[i]["LOT"]),
                                                                   Util.GetValueOfString(DRProductBased[0].ItemArray[2]),
                                                                   Util.GetValueOfString(DRProductBased[0].ItemArray[3]),
                                                                   Util.GetValueOfString(DRProductBased[0].ItemArray[4]),
                                                                   Util.GetValueOfDecimal(DRProductBased[0].ItemArray[5]),
                                                                   Util.GetValueOfDecimal(DRProductBased[0].ItemArray[6]),
                                                                   Util.GetValueOfDecimal(DRProductBased[0].ItemArray[7]),
                                                                   Util.GetValueOfDecimal(DRProductBased[0].ItemArray[8]),
                                                                         Util.GetValueOfDecimal(DRProductBased[0].ItemArray[9]),
                                                                         Util.GetValueOfDecimal(DRProductBased[0].ItemArray[10]),
                                                                         Util.GetValueOfDecimal(DRProductBased[0].ItemArray[11]),
                                                                         Util.GetValueOfDecimal(DRProductBased[0].ItemArray[12]),
                                                                         Util.GetValueOfDecimal(DRProductBased[0].ItemArray[13]),
                                                                         Util.GetValueOfString(DRProductBased[0].ItemArray[14]),
                                                                         Util.GetValueOfString(DRProductBased[0].ItemArray[15]),
                                                                         Util.GetValueOfString(DRProductBased[0].ItemArray[16]),
                                                                         _Precision,
                                                                         PriceArray[0],
                                                                         PriceArray[1],
                                                                         PriceArray[2],
                                                                         _IsListFormula,
                                                                         _IsStdFormula,
                                                                         _IsLimitFormula,
                                                                         _ListFormula,
                                                                         _StdFormula,
                                                                         _LimitFormula,
                                                                         ctx,
                                                                    Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Brand_ID"]),
                                                                    Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_Category_ID"]), IsExceptionFound))
                                                                {
                                                                    if (_KeepPricesForPrevLot == "Y")
                                                                    {
                                                                        if (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) > 0)
                                                                        //if (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) == 0)
                                                                        {
                                                                            KeepPricesForPreviousLots(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]), PriceArray[0],
                                                                                PriceArray[1], PriceArray[2], _Record_ID, DsProductsPrice, C_UOM_ID, ctx);
                                                                        }
                                                                    }
                                                                    ProductAttributes obj = new ProductAttributes();
                                                                    obj.ProductID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                                                    obj.Attribute_ID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                                    if (_CountED011 > 0)
                                                                    {
                                                                        obj.UOM_ID = (C_UOM_ID);
                                                                    }
                                                                    ProductsAttribute.Add(obj);
                                                                    //FlagProcessed = true;
                                                                    continue;


                                                                }
                                                                else
                                                                {
                                                                    _msg = Msg.GetMsg(ctx, "VAPRC_LineNotSaved");
                                                                    // FlagNotSaved = true;
                                                                    return _msg;
                                                                }
                                                            }
                                                        }
                                                        //}
                                                        // Deleted Non Discount Code
                                                    }
                                                    else
                                                    {

                                                        if (_M_DiscountSchemaLine_ID > 0)
                                                        {
                                                            _discountSchemaLine = new MDiscountSchemaLine(ctx, _M_DiscountSchemaLine_ID, null);
                                                            PriceArray[0] = Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIST"]);
                                                            PriceArray[1] = Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICESTD"]);
                                                            PriceArray[2] = Util.GetValueOfDecimal(DsProductsPrice.Tables[0].Rows[i]["PRICELIMIT"]);

                                                            PriceArray = CurrencyConvert(ctx, PriceArray, _discountSchemaLine, Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["basecurrency"]),
                                                                Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["C_Currency_ID"]), AD_Client_ID, AD_Org_ID);
                                                            if (SkipDelCheck == "U" && step == 0)
                                                            {
                                                                DeleteExisting(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]), _Record_ID, C_UOM_ID);
                                                            }
                                                            if (SaveProdPrice(AD_Org_ID,
                                                                Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]),
                                                                _Record_ID,
                                                                C_UOM_ID,
                                                                Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]),
                                                                      Util.GetValueOfString(DsProductsPrice.Tables[0].Rows[i]["LOT"]),
                                                                      _ListBaseVal,
                                                                      _StdBaseVal,
                                                                      _LimitBaseVal,
                                                                      _ListFixed,
                                                                      _StdFixed,
                                                                      _LimitFixed,
                                                                      _listAddAmt,
                                                                      _StdAddAmt,
                                                                      _LimitAddAmt,
                                                                      _ListDiscount,
                                                                      _StdDiscount,
                                                                      _LimitDiscount,
                                                                      _ListRounding,
                                                                      _StdRounding,
                                                                      _LimitRounding,
                                                                      _Precision,
                                                                      PriceArray[0],
                                                                      PriceArray[1],
                                                                      PriceArray[2],
                                                                      _IsListFormula,
                                                                      _IsStdFormula,
                                                                      _IsLimitFormula,
                                                                      _ListFormula,
                                                                      _StdFormula,
                                                                      _LimitFormula,
                                                                      ctx,
                                                                Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Brand_ID"]),
                                                                Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_Category_ID"]), IsExceptionFound))
                                                            {
                                                                if (_KeepPricesForPrevLot == "Y")
                                                                {
                                                                    //if (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) == 0)
                                                                    if (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) > 0)
                                                                    {
                                                                        KeepPricesForPreviousLots(Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]), PriceArray[0],
                                                                           PriceArray[1], PriceArray[2], _Record_ID, DsProductsPrice, C_UOM_ID, ctx);
                                                                    }
                                                                }
                                                                ProductAttributes obj = new ProductAttributes();
                                                                obj.ProductID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_Product_ID"]));
                                                                obj.Attribute_ID = (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                                if (_CountED011 > 0)
                                                                {
                                                                    obj.UOM_ID = (C_UOM_ID);
                                                                }
                                                                ProductsAttribute.Add(obj);
                                                                //FlagProcessed = true;
                                                                continue;

                                                            }
                                                            else
                                                            {
                                                                _msg = Msg.GetMsg(ctx, "VAPRC_LineNotSaved");
                                                                // FlagNotSaved = true;
                                                                return _msg;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                _msg = e.Message.ToString();
                                                completed = false;
                                                //Update status of version tab
                                                DB.ExecuteQuery("UPDATE M_PriceList_Version SET Processed='N'  WHERE M_Pricelist_Version_ID= " + _Record_ID);
                                                if (Util.GetValueOfInt(DsProductsPrice.Tables[0].Rows[i]["DisSchema_ID"]) > 0)
                                                {
                                                    _msg = Msg.GetMsg(ctx, "VAPRC_LineNotSaved");
                                                }
                                                else
                                                {
                                                    _msg = Msg.GetMsg(ctx, "VAPRC_LineNotSaved");
                                                }
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
                                                if (_dsVersionRecords != null)
                                                {
                                                    _dsVersionRecords.Dispose();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                _msg = Msg.GetMsg(Env.GetCtx(), "VAPRC_PriceListCreatedSuccess");
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, "", ex);
                _msg = Msg.GetMsg(ctx, "VAPRC_LineNotSaved");
                return _msg;
            }
            return _msg;
        }
        private void GetExceptionData()
        {
            try
            {
                if (_dsException != null && _dsException.Tables[0].Rows.Count > 0)
                {
                    _dsException.Dispose();
                }
                _Sql.Clear();
                _Sql.Append("SELECT exp.AD_Client_ID,exp.AD_Org_ID,exp.C_Bpartner_ID,exp.M_Brand_ID,exp.M_Product_ID,exp.M_Product_Category_ID,exp.VAPRC_SumLevelProd  FROM VAPRC_Exception exp INNER JOIN M_DiscountSchemaLine dsl  ON (exp.M_DiscountSchemaLine_ID=dsl.M_DiscountSchemaLine_ID)"
                                                + " WHERE exp.IsActive ='Y' AND dsl.IsActive ='Y' AND dsl.M_DiscountSchema_ID =" + _DiscountSchema_ID);
                _dsException = DB.ExecuteDataset(_Sql.ToString(), null, null);
            }
            catch (Exception e)
            {
                _log.Info("Error occured in Method GetExceptionData");
                _msg = e.Message;
            }
        }
        private DataSet GetVersionProducts(int M_PriceListVersion_ID)
        {
            try
            {
                _dsVersionRecords = new DataSet();
                _dsVersionRecords = DB.ExecuteDataset("SELECT M_Product_ID, M_AttributeSetInstance_ID, C_Uom_ID FROM M_ProductPrice WHERE M_PriceList_Version_ID=" + M_PriceListVersion_ID, null, null);
            }
            catch (Exception e)
            {
                _log.Info("Error occured while getting Records From Product Price");
                _msg = e.Message;
            }
            return _dsVersionRecords;
        }
        // Method to get all the products from summary level from discount schema line
        private void CreateTreeArraySchema(Ctx ctx)
        {
            try
            {
                ProdDict.Clear();
                _DiscSchLine_IDs.Clear();
                int AD_Table_ID = MTable.Get_Table_ID("M_Product");
                string sqlnew = "SELECT AD_Tree_ID FROM AD_Tree "
                + "WHERE AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND AD_Table_ID=" + AD_Table_ID + " AND IsActive='Y' AND IsAllNodes='Y' "
                            + "ORDER BY IsDefault DESC, AD_Tree_ID";

                int AD_Tree_ID = Util.GetValueOfInt(DB.ExecuteScalar(sqlnew, null, null));

                DataSet _dsTree = DB.ExecuteDataset("SELECT DISTINCT VAPRC_SumLevelProd,M_DiscountSchemaLine_ID FROM M_DiscountSchemaLine WHERE M_DiscountSchema_ID=" + _DiscountSchema_ID + ""
                                                    + " AND VAPRC_SumLevelProd  IS NOT NULL AND IsActive ='Y'");
                DataSet DsTreeFinal = new DataSet();
                if (_dsTree != null)
                {
                    if (_dsTree.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < _dsTree.Tables[0].Rows.Count; i++)
                        {
                            List<int> ProductTreeList1 = new List<int>();
                            if (!ProdDict.ContainsKey(Util.GetValueOfInt(_dsTree.Tables[0].Rows[i]["VAPRC_SumLevelProd"])))
                            {

                                GetChildNodesID(Util.GetValueOfInt(_dsTree.Tables[0].Rows[i]["VAPRC_SumLevelProd"]), "AD_TreeNodePR", AD_Tree_ID, "M_Product");
                                sbSql.Clear();
                                sbSql.Append("  (SELECT M_Product.M_Product_ID  FROM AD_TreeNodePR AD_TreeNodePR  INNER JOIN AD_Tree AD_Tree  ON AD_Tree.AD_Tree_ID =AD_TreeNodePR.AD_Tree_ID "
                                + " INNER JOIN M_Product M_Product ON M_Product.M_Product_ID=AD_TreeNodePR.Node_ID  WHERE AD_TreeNodePR.Parent_ID IN (" + parentIDs.ToString() + ") AND AD_Tree.TreeType ='PR')");
                                DsTreeFinal = DB.ExecuteDataset(sbSql.ToString());
                                parentIDs.Clear();
                                sbSql.Clear();
                                if (DsTreeFinal != null)
                                {
                                    if (DsTreeFinal.Tables[0].Rows.Count > 0)
                                    {
                                        for (int j = 0; j < DsTreeFinal.Tables[0].Rows.Count; j++)
                                        {
                                            ProductTreeList1.Add(Util.GetValueOfInt(DsTreeFinal.Tables[0].Rows[j]["M_Product_ID"]));
                                        }
                                        ProdDict.Add(Util.GetValueOfInt(_dsTree.Tables[0].Rows[i]["M_DiscountSchemaLine_ID"]), ProductTreeList1);
                                        _DiscSchLine_IDs.Add(Util.GetValueOfInt(_dsTree.Tables[0].Rows[i]["M_DiscountSchemaLine_ID"]));
                                    }
                                }
                            }

                        }
                        _dsTree.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                _log.Info("Error occured in Method CreateTreeArraySchema");
                _msg = e.Message;
            }
        }
        // Method to get child nodes of the currently passed product
        private void GetChildNodesID(int currentnode, string tableName, int treeID, string adtableName)
        {
            try
            {
                if (parentIDs.Length == 0)
                {
                    parentIDs.Append(currentnode);
                }
                else
                {
                    parentIDs.Append(",").Append(currentnode);
                }


                string sql = "SELECT pr.node_ID FROM " + tableName + "   pr JOIN " + adtableName + " mp on pr.Node_ID=mp." + adtableName + "_id  WHERE pr.AD_Tree_ID=" + treeID + " AND pr.Parent_ID = " + currentnode + " AND mp.ISActive='Y' AND mp.IsSummary='Y'";

                DataSet ds = DB.ExecuteDataset(sql);
                if (ds == null || ds.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        GetChildNodesID(Convert.ToInt32(ds.Tables[0].Rows[j]["node_ID"]), tableName, treeID, adtableName);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Info("Error occured in Method GetChildNodesID");
                _msg = e.Message;
            }

        }
        // Method to get all the products from summary level in exception tab
        private void CreateTreeArrayException(Ctx ctx)
        {
            try
            {
                ProdExcepDict.Clear();
                Dictionary<int, List<List<int>>> ProdExcpExpID = new Dictionary<int, List<List<int>>>();
                int AD_Table_ID = MTable.Get_Table_ID("M_Product");
                string sqlnew = "SELECT AD_Tree_ID FROM AD_Tree "
                + "WHERE AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND AD_Table_ID=" + AD_Table_ID + " AND IsActive='Y' AND IsAllNodes='Y' "
                            + "ORDER BY IsDefault DESC, AD_Tree_ID";

                int AD_Tree_ID = Util.GetValueOfInt(DB.ExecuteScalar(sqlnew, null, null));

                DataSet _dsDisSchLnId = DB.ExecuteDataset("SELECT DISTINCT  exp.M_DiscountSchemaLine_ID FROM VARPC_Exception exp INNER JOIN M_DiscountSchemaLine dsl"
                                                  + "  ON (exp.M_DiscountSchemaLine_id= dsl.M_DiscountSchemaLine_id)  WHERE exp.VAPRC_SumLevelProd IS NOT NULL AND exp.IsActive ='Y'"
                                                   + "    AND dsl.IsActive  ='Y' AND dsl.M_DiscountSchema_ID=" + _DiscountSchema_ID);
                DataSet _dsTree = null;
                DataSet DsTreeFinal = null;
                if (_dsDisSchLnId != null)
                {
                    if (_dsDisSchLnId.Tables[0].Rows.Count > 0)
                    {
                        for (int dsline = 0; dsline < _dsDisSchLnId.Tables[0].Rows.Count; dsline++)
                        {
                            ProdExcpExpID = new Dictionary<int, List<List<int>>>();
                            _dsTree = DB.ExecuteDataset("SELECT DISTINCT exp.VAPRC_SumLevelProd, exp.M_DiscountSchemaLine_ID,VAPRC_Exception_ID FROM VAPRC_Exception exp INNER JOIN M_DiscountSchemaLine dsl"
                                                 + "  ON (exp.M_DiscountSchemaLine_id= dsl.M_DiscountSchemaLine_id)  WHERE exp.VAPRC_SumLevelProd   IS NOT NULL AND exp.IsActive ='Y'"
                                                  + "    AND dsl.IsActive  ='Y' AND dsl.M_DiscountSchema_ID=" + _DiscountSchema_ID + " AND dsl.M_DiscountSchemaLine_ID=" + Util.GetValueOfInt(_dsDisSchLnId.Tables[0].Rows[dsline]["M_DiscountSchemaLine_ID"]) + " ORDER BY VAPRC_Exception_ID");

                            if (_dsTree != null)
                            {
                                if (_dsTree.Tables[0].Rows.Count > 0)
                                {
                                    //if (!ProdExcpExpID.ContainsKey(Util.GetValueOfInt(_dsTree.Tables[0].Rows[dsline]["VAPRC_Exception_ID"])))
                                    {

                                        for (int i = 0; i < _dsTree.Tables[0].Rows.Count; i++)
                                        {
                                            List<List<int>> ProductArrayList = new List<List<int>>();
                                            List<int> ProductTreeList1 = new List<int>();
                                            GetChildNodesID(Util.GetValueOfInt(_dsTree.Tables[0].Rows[i]["VAPRC_SumLevelProd"]), "AD_TreeNodePR", AD_Tree_ID, "M_Product");
                                            sbSql.Clear();
                                            sbSql.Append("  (SELECT M_Product.M_Product_ID  FROM AD_TreeNodePR AD_TreeNodePR  INNER JOIN AD_Tree AD_Tree  ON AD_Tree.AD_Tree_ID =AD_TreeNodePR.AD_Tree_ID "
                                            + " INNER JOIN M_Product M_Product ON M_Product.M_Product_ID=AD_TreeNodePR.Node_ID  WHERE AD_TreeNodePR.Parent_ID IN (" + parentIDs.ToString() + ") AND AD_Tree.TreeType ='PR')");
                                            DsTreeFinal = DB.ExecuteDataset(sbSql.ToString());
                                            parentIDs.Clear();
                                            sbSql.Clear();
                                            if (DsTreeFinal != null)
                                            {
                                                if (DsTreeFinal.Tables[0].Rows.Count > 0)
                                                {

                                                    for (int j = 0; j < DsTreeFinal.Tables[0].Rows.Count; j++)
                                                    {
                                                        ProductTreeList1.Add(Util.GetValueOfInt(DsTreeFinal.Tables[0].Rows[j]["M_Product_ID"]));
                                                    }
                                                    ProductArrayList.Add(ProductTreeList1);
                                                    ProdExcpExpID.Add(Util.GetValueOfInt(_dsTree.Tables[0].Rows[i]["VAPRC_Exception_ID"]), ProductArrayList);
                                                    DsTreeFinal.Dispose();
                                                    DsTreeFinal = null;
                                                }
                                            }
                                        }


                                    }
                                }
                                _dsTree.Dispose();
                                //_dsTree = null;
                            }
                            ProdExcepDict.Add(Util.GetValueOfInt(_dsDisSchLnId.Tables[0].Rows[dsline]["M_DiscountSchemaLine_ID"]), ProdExcpExpID);
                        }
                        _dsDisSchLnId.Dispose();
                        //_dsDisSchLnId = null;
                    }

                }
            }
            catch (Exception e)
            {
                _log.Info("Error occured in Method CreateTReeArrayException");
                _msg = e.Message;
            }
        }
        // New Methods to handle summary level along with other parameters
        private void SummLevelDiscSchema()
        {
            try
            {
                OnlySumLevDiscSch.Clear();
                _DsSumLevel.Dispose();
                _DsSumLevel = DB.ExecuteDataset(@"SELECT M_DiscountSchemaLine_ID
                                            FROM M_DiscountSchemaLine
                                            WHERE M_Product_ID         IS NULL
                                            AND M_Product_Category_ID  IS NULL
                                            AND C_Bpartner_ID          IS NULL
                                            AND M_Brand_ID             IS NULL
                                            AND VAPRC_SumLevelProd     IS NOT NULL
                                            AND IsActive                ='Y'
                                            AND M_DiscountSchema_ID     =" + _DiscountSchema_ID, null, null);
                if (_DsSumLevel != null && _DsSumLevel.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DsSumLevel.Tables[0].Rows.Count; i++)
                    {
                        OnlySumLevDiscSch.Add(Util.GetValueOfInt(_DsSumLevel.Tables[0].Rows[i]["M_DiscountSchemaLine_ID"]));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Info("Error occured in Method SummLevelDiscSchema");
                _msg = e.Message;
            }
        }
        // Method to get all the products from summary level in exception tab where only seummary level is bind
        private void CreateTreeArrayExceptionSumLevel(Ctx ctx)
        {
            try
            {
                ProdExcepDictSumLevel.Clear();
                int AD_Table_ID = MTable.Get_Table_ID("M_Product");
                string sqlnew = "SELECT AD_Tree_ID FROM AD_Tree "
                + "WHERE AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND AD_Table_ID=" + AD_Table_ID + " AND IsActive='Y' AND IsAllNodes='Y' "
                            + "ORDER BY IsDefault DESC, AD_Tree_ID";

                int AD_Tree_ID = Util.GetValueOfInt(DB.ExecuteScalar(sqlnew, null, null));

                DataSet _dsDisSchLnId = DB.ExecuteDataset("SELECT DISTINCT  exp.M_DiscountSchemaLine_ID FROM VAPRC_Exception exp INNER JOIN M_DiscountSchemaLine dsl"
                                                  + "  ON (exp.M_DiscountSchemaLine_id= dsl.M_DiscountSchemaLine_id)  WHERE exp.VAPRC_SumLevelProd   IS NOT NULL AND exp.IsActive ='Y'"
                                                   + "    AND dsl.IsActive  ='Y' AND dsl.M_DiscountSchema_ID=" + _DiscountSchema_ID);
                DataSet _dsTree = null;
                DataSet DsTreeFinal = null;
                if (_dsDisSchLnId != null)
                {
                    if (_dsDisSchLnId.Tables[0].Rows.Count > 0)
                    {
                        for (int dsline = 0; dsline < _dsDisSchLnId.Tables[0].Rows.Count; dsline++)
                        {
                            _dsTree = DB.ExecuteDataset(@"SELECT DISTINCT exp.VAPRC_SumLevelProd, exp.M_DiscountSchemaLine_ID FROM VAPRC_Exception exp INNER JOIN M_DiscountSchemaLine dsl
                                              ON (exp.M_DiscountSchemaLine_id= dsl.M_DiscountSchemaLine_id)  WHERE exp.VAPRC_SumLevelProd   IS NOT NULL
                                              AND exp.M_Product_ID IS NULL AND exp.M_Product_Category_ID IS NULL AND exp.C_Bpartner_ID  IS NULL
                                              AND exp.M_Brand_ID IS NULL
                                              AND exp.IsActive ='Y' AND dsl.IsActive  ='Y' AND dsl.M_DiscountSchema_ID=" + _DiscountSchema_ID + @" AND 
                                              dsl.M_DiscountSchemaLine_ID=" + Util.GetValueOfInt(_dsDisSchLnId.Tables[0].Rows[dsline]["M_DiscountSchemaLine_ID"]));
                            if (_dsTree != null)
                            {
                                if (_dsTree.Tables[0].Rows.Count > 0)
                                {
                                    if (!ProdExcepDictSumLevel.ContainsKey(Util.GetValueOfInt(_dsDisSchLnId.Tables[0].Rows[dsline]["M_DiscountSchemaLine_ID"])))
                                    {
                                        List<List<int>> ProductArrayList = new List<List<int>>();
                                        for (int i = 0; i < _dsTree.Tables[0].Rows.Count; i++)
                                        {

                                            List<int> ProductTreeList1 = new List<int>();
                                            GetChildNodesID(Util.GetValueOfInt(_dsTree.Tables[0].Rows[i]["VAPRC_SumLevelProd"]), "AD_TreeNodePR", AD_Tree_ID, "M_Product");
                                            sbSql.Clear();
                                            sbSql.Append("  (SELECT M_Product.M_Product_ID  FROM AD_TreeNodePR AD_TreeNodePR  INNER JOIN AD_Tree AD_Tree  ON AD_Tree.AD_Tree_ID =AD_TreeNodePR.AD_Tree_ID "
                                            + " INNER JOIN M_Product M_Product ON M_Product.M_Product_ID=AD_TreeNodePR.Node_ID  WHERE AD_TreeNodePR.Parent_ID IN (" + parentIDs.ToString() + ") AND AD_Tree.TreeType ='PR')");
                                            DsTreeFinal = DB.ExecuteDataset(sbSql.ToString());
                                            parentIDs.Clear();
                                            sbSql.Clear();
                                            if (DsTreeFinal != null)
                                            {
                                                if (DsTreeFinal.Tables[0].Rows.Count > 0)
                                                {

                                                    for (int j = 0; j < DsTreeFinal.Tables[0].Rows.Count; j++)
                                                    {
                                                        ProductTreeList1.Add(Util.GetValueOfInt(DsTreeFinal.Tables[0].Rows[j]["M_Product_ID"]));
                                                    }
                                                    ProductArrayList.Add(ProductTreeList1);

                                                    DsTreeFinal.Dispose();
                                                    DsTreeFinal = null;
                                                }
                                            }
                                        }
                                        ProdExcepDictSumLevel.Add(Util.GetValueOfInt(_dsDisSchLnId.Tables[0].Rows[dsline]["M_DiscountSchemaLine_ID"]), ProductArrayList);
                                    }
                                }
                                _dsTree.Dispose();
                                //_dsTree = null;
                            }
                        }
                        _dsDisSchLnId.Dispose();
                        //_dsDisSchLnId = null;
                    }

                }
            }
            catch (Exception e)
            {
                _log.Info("Error occured in Method CreateTreeArrayExceptionSumLevel");
                _msg = e.Message;
            }
        }
        //Check if records needed to be skipped or inserted
        private bool CheckSkipStatus(int M_Product_ID, int M_AttributeSetInstance_ID, int C_Uom_ID)
        {
            bool status = false;

            try
            {
                DataRow[] DRProdRow = _dsVersionRecords.Tables[0].Select(" M_Product_ID=" + M_Product_ID + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + " AND C_Uom_ID= " + C_Uom_ID);
                if (DRProdRow.Length > 0)
                {
                    status = true;
                }
            }
            catch (Exception e)
            {
                _log.Info("Error occured in Method CheckSkipStatus");
                _msg = e.Message;
            }
            return status;
        }
        //check product for discount schema
        private int CheckProductDiscSchema(int M_Product_ID, int DiscSchema_ID, int Exception_ID, int Record_ID)
        {
            int DiscountSchema_ID = 0;
            //int Exp_ID = 0;
            if (DiscSchema_ID > 0)
            {
                try
                {
                    if (CheckSumLevelOnDiscSchId(DiscSchema_ID))
                    {
                        if (_DiscSchLine_IDs.Count > 0 && ProdDict.Count > 0)
                        {
                            DiscountSchema_ID = CheckProdStatus(M_Product_ID, DiscSchema_ID, Exception_ID);


                        }
                    }
                    else
                    {
                        if (Exception_ID > 0)
                        {
                            if (CheckSumLevelOnExceptionId(Exception_ID))
                            {
                                if (!CheckException(DiscSchema_ID, M_Product_ID, Exception_ID))
                                {
                                    DiscountSchema_ID = DiscSchema_ID;
                                }
                                else
                                {
                                    DiscountSchema_ID = 0;
                                }
                            }
                            //else if(
                            if (CheckExceptionWithoutSummaryLevel(DiscSchema_ID, M_Product_ID, Exception_ID))
                            {
                                DiscountSchema_ID = DiscSchema_ID;
                                return DiscountSchema_ID;
                            }
                            if (!CheckExceptionSumLevel(DiscSchema_ID, M_Product_ID))
                            {
                                DiscountSchema_ID = 0;
                                return DiscountSchema_ID;
                            }
                            else
                            {
                                DiscountSchema_ID = DiscSchema_ID;
                            }
                        }
                        else
                        {
                            if (CheckExceptionSumLevel(DiscSchema_ID, M_Product_ID))
                            {
                                DiscountSchema_ID = 0;
                            }
                            else
                            {
                                DiscountSchema_ID = DiscSchema_ID;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    DB.ExecuteQuery("UPDATE M_PriceList_Version SET Processed='N'  WHERE M_Pricelist_Version_ID= " + Record_ID);
                    throw e;
                }
            }
            else
            {
                DiscountSchema_ID = CheckSumLevDiscSch(M_Product_ID, Exception_ID);
            }
            return DiscountSchema_ID;
        }
        //Delete Exsting records from product Price
        private void DeleteExisting(int _Product_ID, int _AttributeSetInstance_ID, int PLVersion_ID, int C_Uom_ID)
        {
            try
            {

                string Sql = @" SELECT Count(*) FROM M_Productprice WHERE IsActive ='Y' AND M_Product_ID =" + _Product_ID + " AND M_ATTRIBUTESETINSTANCE_ID=" + _AttributeSetInstance_ID + " AND "
                + " M_PRICELIST_VERSION_ID  =" + PLVersion_ID + " ";
                if (_CountED011 > 0)
                {
                    Sql += " AND C_Uom_ID=" + C_Uom_ID + "";
                }
                if (Util.GetValueOfInt(DB.ExecuteScalar(Sql, null, null)) > 0)
                {
                    Sql = "";
                    Sql = @" DELETE FROM M_Productprice WHERE IsActive ='Y' AND M_Product_ID =" + _Product_ID + " AND M_ATTRIBUTESETINSTANCE_ID=" + _AttributeSetInstance_ID + " AND "
                + " M_PRICELIST_VERSION_ID  =" + PLVersion_ID + " ";
                    if (_CountED011 > 0)
                    {
                        Sql += " AND C_Uom_ID=" + C_Uom_ID + "";
                    }
                    int a = DB.ExecuteQuery(Sql, null, null);
                    Sql = null;
                }
            }
            catch (Exception e)
            {
                _log.Info("Error occured while Deleting product Price");
                _msg = e.Message;
            }

        }
        // Check only discount schemna lines where summary level is bind
        private int CheckSumLevDiscSch(int M_Product_ID, int Exception_ID)
        {
            int DiscountSchema_ID = 0;
            int Exp_ID = 0;
            try
            {
                if (OnlySumLevDiscSch.Count > 0)
                {
                    if (_DiscSchLine_IDs.Count > 0)
                    {
                        for (int i = 0; i < OnlySumLevDiscSch.Count; i++)
                        {

                            List<int> List = ProdDict[OnlySumLevDiscSch[i]];
                            if (List.Contains(M_Product_ID))
                            {
                                //Need inhancements in DB for different functions
                                Exp_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT VAPRC_GetPriceListException(M_Product_ID,
                                    NVL( C_Bpartner_ID,0), M_Product_Category_ID, M_Brand_ID 
                                     ," + OnlySumLevDiscSch[i] + ") as exception FROM M_Product WHERE m_product_id=" + M_Product_ID, null, null));
                                if (Exp_ID > 0)
                                {
                                    if (CheckSumLevelOnExceptionId(Exp_ID))
                                    {
                                        if (!CheckException(OnlySumLevDiscSch[i], M_Product_ID, Exception_ID))
                                        {
                                            DiscountSchema_ID = OnlySumLevDiscSch[i];
                                        }
                                        else
                                        {
                                            DiscountSchema_ID = 0;
                                        }
                                    }
                                    else
                                    {
                                        DiscountSchema_ID = 0;
                                    }
                                }
                                else
                                {
                                    if (!CheckExceptionSumLevel(OnlySumLevDiscSch[i], M_Product_ID))
                                    {
                                        DiscountSchema_ID = OnlySumLevDiscSch[i];
                                    }
                                    else
                                    {
                                        DiscountSchema_ID = 0;
                                    }
                                }

                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                _msg = e.Message;
                _log.Info("Error occured in Method getting discount schemna lines where summary level is there");
            }

            return DiscountSchema_ID;
        }
        private bool CheckSumLevelOnDiscSchId(int DiscountSchemaLine_ID)
        {
            bool result = false;
            try
            {
                MDiscountSchemaLine dLine = new MDiscountSchemaLine(ctx, DiscountSchemaLine_ID, trx);
                if (Util.GetValueOfInt(dLine.Get_Value("VAPRC_SumLevelProd")) > 0)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                _msg = e.Message;
                _log.Info("Error occured while getting Discount Schema Data");
            }
            return result;
        }
        private bool CheckSumLevelOnExceptionId(int Exception_ID)
        {
            bool result = false;
            try
            {
                X_VAPRC_Exception exc = new X_VAPRC_Exception(ctx, Exception_ID, trx);
                if (exc.GetVAPRC_SumLevelProd() > 0)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                _msg = e.Message;
                _log.Info("Error occured while getting exception data");
            }
            return result;
        }

        public bool SaveProdPrice(int AD_Org_ID, int Product_ID, int Record_ID, int C_Uom_ID, int AttributeSetInstance_ID, string Lot, string _ListBaseVal, string _StdBaseVal, string _LimitBaseVal,
           decimal _ListFixed, decimal _StdFixed, decimal _LimitFixed, decimal _listAddAmt, decimal _StdAddAmt, decimal _LimitAddAmt, decimal _ListDiscount, decimal _StdDiscount,
           decimal _LimitDiscount, string _ListRounding, string _StdRounding, string _LimitRounding, int Precision, decimal PriceList, decimal PriceStd, decimal PriceLimit,
           string _IsListFormula, string _IsStdFormula, string _IsLimitFormula, string _ListFormula, string _StdFormula, string _LimitFormula, Ctx ctx,
           int M_Brand_ID, int M_Product_Category_ID, bool exceptionFound)
        {
            try
            {
                PriceListAmt = 0;
                PriceStdAmt = 0;
                PriceLimitAmt = 0;
                if (_IsListFormula == "Y")
                {
                    PriceListAmt = Calculate(_ListBaseVal, PriceList, PriceStd, PriceLimit, _ListFixed, _ListFormula, _listAddAmt, _ListRounding, _Precision, exceptionFound);
                }
                else
                {
                    PriceListAmt = Calculate(_ListBaseVal, PriceList, PriceStd, PriceLimit, _ListFixed, _listAddAmt, _ListDiscount, _ListRounding, _Precision, exceptionFound);
                }
                if (_IsStdFormula == "Y")
                {
                    PriceStdAmt = Calculate(_StdBaseVal, PriceList, PriceStd, PriceLimit, _StdFixed, _StdFormula, _StdAddAmt, _StdRounding, _Precision, exceptionFound);
                }
                else
                {
                    PriceStdAmt = Calculate(_StdBaseVal, PriceList, PriceStd, PriceLimit, _StdFixed, _StdAddAmt, _StdDiscount, _StdRounding, _Precision, exceptionFound);
                }
                if (_IsListFormula == "Y")
                {
                    PriceLimitAmt = Calculate(_LimitBaseVal, PriceList, PriceStd, PriceLimit, _LimitFixed, _LimitFormula, _LimitAddAmt, _LimitRounding, _Precision, exceptionFound);
                }
                else
                {
                    PriceLimitAmt = Calculate(_LimitBaseVal, PriceList, PriceStd, PriceLimit, _LimitFixed, _LimitAddAmt, _LimitDiscount, _LimitRounding, _Precision, exceptionFound);
                }


                #region Creating a new price for the Product for the current Version

                MProductPrice pp = new MProductPrice(ctx, 0, trx);
                pp.SetAD_Client_ID(AD_Client_ID);
                pp.SetAD_Org_ID(AD_Org_ID);
                pp.Set_Value("CREATED", GlobalVariable.TO_DATE(System.DateTime.Now, true));
                pp.Set_Value("UPDATED", GlobalVariable.TO_DATE(System.DateTime.Now, true));
                pp.Set_Value("CREATEDBY", ctx.GetAD_User_ID());
                pp.Set_Value("UPDATEDBY", ctx.GetAD_User_ID());
                pp.SetIsActive(true);
                pp.SetM_PriceList_Version_ID(Record_ID);
                pp.SetM_Product_ID(Product_ID);
                pp.SetPriceLimit(PriceLimitAmt);
                pp.SetPriceList(PriceListAmt);
                pp.SetPriceStd(PriceStdAmt);
                pp.SetM_AttributeSetInstance_ID(AttributeSetInstance_ID);
                if (_CountED011 > 0)
                    pp.SetC_UOM_ID(C_Uom_ID);
                pp.SetLot(Util.GetValueOfString(Lot));

                if (pp.Save(trx))
                {
                    exceptionFound = false;
                    return true;
                }
                #endregion
            }
            catch (Exception e)
            {
                _log.Info("Error occured during Inserting price list in Product Price");
                _msg = e.Message;
            }
            return false;
        }
        // Calculate Method Formula Based
        // Calculate Method
        public Decimal Calculate(String base1,
         Decimal list, Decimal std, Decimal limit, Decimal fix,
         Decimal add, Decimal discount, String round, int curPrecision, bool exceptionFound)
        {
            Decimal? calc = null;
            double dd = 0.0;
            try
            {
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
                    if (!exceptionFound)
                    {
                        if (Env.Signum(add) != 0)
                        {
                            dd += Convert.ToDouble(add);//.doubleValue();
                        }
                        if (Env.Signum(discount) != 0)
                        {
                            dd *= 1 - (Convert.ToDouble(discount) / 100.0);
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
                    calc = Decimal.Round(calc.Value, 2, MidpointRounding.AwayFromZero);//calc.setScale(-2, Decimal.ROUND_HALF_UP);
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
                    calc = Decimal.Round(calc.Value, 1, MidpointRounding.AwayFromZero);//calc.setScale(-1, Decimal.ROUND_HALF_UP);
                }
                else if (MDiscountSchemaLine.LIST_ROUNDING_Thousand.Equals(round))
                {
                    calc = Decimal.Round(calc.Value, 3, MidpointRounding.AwayFromZero);//calc.setScale(-3, Decimal.ROUND_HALF_UP);
                }
                else if (MDiscountSchemaLine.LIST_ROUNDING_WholeNumber00.Equals(round))
                {
                    calc = Decimal.Round(calc.Value, 0, MidpointRounding.AwayFromZero);//calc.setScale(0, Decimal.ROUND_HALF_UP);
                }
            }
            catch (Exception e)
            {
                IsExceptionFound = false;
                _log.Info("Error occured during Caculating Price");
                _msg = e.Message;
            }
            IsExceptionFound = false;
            return calc.Value;
        }

        // Calculate Method Formula Based
        private Decimal Calculate(String base1,
         Decimal list, Decimal std, Decimal limit, Decimal fix, string formula,
         Decimal add, String round, int curPrecision, bool exceptionFound)
        {
            Decimal? calc = null;
            double dd = 0.0;
            try
            {
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
                    if (!exceptionFound)
                    {
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
                    calc = Decimal.Round(calc.Value, 2, MidpointRounding.AwayFromZero);//calc.setScale(-2, Decimal.ROUND_HALF_UP);
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
                    calc = Decimal.Round(calc.Value, 1, MidpointRounding.AwayFromZero);//calc.setScale(-1, Decimal.ROUND_HALF_UP);
                }
                else if (MDiscountSchemaLine.LIST_ROUNDING_Thousand.Equals(round))
                {
                    calc = Decimal.Round(calc.Value, 3, MidpointRounding.AwayFromZero);//calc.setScale(-3, Decimal.ROUND_HALF_UP);
                }
                else if (MDiscountSchemaLine.LIST_ROUNDING_WholeNumber00.Equals(round))
                {
                    calc = Decimal.Round(calc.Value, 0, MidpointRounding.AwayFromZero);//calc.setScale(0, Decimal.ROUND_HALF_UP);
                }
            }
            catch (Exception e)
            {
                IsExceptionFound = false;
                _log.Info("Error occured during Caculating Price");
                _msg = e.Message;
            }
            IsExceptionFound = false;
            return calc.Value;
        }

        //Calculate currency conversion for the Product
        private decimal[] CurrencyConvert(Ctx ctx, decimal[] price, MDiscountSchemaLine discoutSchemaLine, int CurrencyFrom, int CurrencyTo, int Ad_Client_ID, int Ad_Org_ID)
        {
            decimal[] convertedPrice = new decimal[3];
            int i = 0;
            try
            {

                if (CurrencyFrom == CurrencyTo)
                {
                    return price;
                }
                while (i < 3)
                {
                    convertedPrice[i] = MConversionRate.Convert(ctx, price[i], CurrencyFrom, CurrencyTo,
                                               discoutSchemaLine.GetConversionDate(), discoutSchemaLine.GetC_ConversionType_ID(), Ad_Client_ID, Ad_Org_ID);
                    i++;
                }
            }
            catch (Exception e)
            {
                _log.Info("Error occured during Conversion at CreatePriceList");
                _msg = e.Message;
            }
            return convertedPrice;
        }

        //If attribute is there for the previos Lot then Create New price List
        public void KeepPricesForPreviousLots(int Product_id, decimal PriceList, decimal PriceStd, decimal PriceLimit, int PriceListVersion_ID, DataSet DsPPrice, int UOM_ID, Ctx ctx)
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
                if (DsLotBased.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < DsLotBased.Tables[0].Rows.Count - 1; j++)
                    {
                        DataRow[] DRAttributeBased = DsPPrice.Tables[0].Select(" M_Product_ID=" + Product_id + " AND M_AttributeSetInstance_ID=" + Util.GetValueOfInt(DsLotBased.Tables[0].Rows[j]["AttributeSetInstance_ID"]) + "");
                        if (DRAttributeBased.Length > 0)
                        {
                            #region Insert Product Price IN product Price Window based on LOT
                            MProductPrice pp = new MProductPrice(ctx, 0, trx);
                            pp.SetAD_Client_ID(AD_Client_ID);
                            pp.SetAD_Org_ID(AD_Org_ID);
                            pp.Set_Value("CREATED", GlobalVariable.TO_DATE(System.DateTime.Now, true));
                            pp.Set_Value("UPDATED", GlobalVariable.TO_DATE(System.DateTime.Now, true));
                            pp.Set_Value("CREATEDBY", ctx.GetAD_User_ID());
                            pp.Set_Value("UPDATEDBY", ctx.GetAD_User_ID());
                            pp.SetIsActive(true);
                            pp.SetM_PriceList_Version_ID(PriceListVersion_ID);
                            pp.SetM_Product_ID(Product_id);
                            pp.SetPriceLimit(Util.GetValueOfDecimal(DRAttributeBased[0].ItemArray[4]));
                            pp.SetPriceList(Util.GetValueOfDecimal(DRAttributeBased[0].ItemArray[5]));
                            pp.SetPriceStd(Util.GetValueOfDecimal(DRAttributeBased[0].ItemArray[6]));
                            pp.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(DsLotBased.Tables[0].Rows[j]["AttributeSetInstance_ID"]));
                            if (_CountED011 > 0)
                                pp.SetC_UOM_ID(Util.GetValueOfInt(DRAttributeBased[0].ItemArray[10]));
                            pp.SetLot(Util.GetValueOfString(DsLotBased.Tables[0].Rows[j]["LotNo"]));

                            if (pp.Save(trx))
                            {

                            }
                            #endregion
                        }
                        else
                        {
                            MProductPrice pp = new MProductPrice(ctx, 0, trx);
                            pp.SetAD_Client_ID(AD_Client_ID);
                            pp.SetAD_Org_ID(AD_Org_ID);
                            pp.Set_Value("CREATED", GlobalVariable.TO_DATE(System.DateTime.Now, true));
                            pp.Set_Value("UPDATED", GlobalVariable.TO_DATE(System.DateTime.Now, true));
                            pp.Set_Value("CREATEDBY", ctx.GetAD_User_ID());
                            pp.Set_Value("UPDATEDBY", ctx.GetAD_User_ID());
                            pp.SetIsActive(true);
                            pp.SetM_PriceList_Version_ID(PriceListVersion_ID);
                            pp.SetM_Product_ID(Product_id);
                            pp.SetPriceLimit(PriceLimit);
                            pp.SetPriceList(PriceList);
                            pp.SetPriceStd(PriceStd);
                            pp.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(DsLotBased.Tables[0].Rows[j]["AttributeSetInstance_ID"]));
                            if (_CountED011 > 0)
                                pp.SetC_UOM_ID(Util.GetValueOfInt(DRAttributeBased[0].ItemArray[10]));
                            pp.SetLot(Util.GetValueOfString(DsLotBased.Tables[0].Rows[j]["LotNo"]));
                            if (pp.Save(trx))
                            {
                            }
                        }
                    }
                }
                DsLotBased.Dispose();
            }
            catch (Exception e)
            {
                _log.Info("Error occured during Creating Price List From Previous Lot");
                DsLotBased.Dispose();
                _msg = e.Message;
            }
        }
        // Method to check summary level discountschema exists or not
        private int CheckProdStatus(int _Product_ID, int DiscSchema_ID, int Exception_ID)
        {
            int _DiscSchemaLine_ID = 0;
            try
            {
                if (DiscSchema_ID == 0)
                {
                    if (_DiscSchLine_IDs.Count > 0)
                    {
                        for (int i = 0; i < _DiscSchLine_IDs.Count; i++)
                        {
                            List<int> List = ProdDict[_DiscSchLine_IDs[i]];
                            if (List.Contains(_Product_ID))
                            {
                                if (!CheckException(_DiscSchLine_IDs[i], _Product_ID, Exception_ID))
                                {
                                    _DiscSchemaLine_ID = _DiscSchLine_IDs[i];
                                }
                                else
                                {
                                    _DiscSchemaLine_ID = 0;
                                }
                            }
                        }
                    }
                }
                else
                {
                    List<int> List = ProdDict[DiscSchema_ID];
                    if (List.Contains(_Product_ID))
                    {
                        if (Exception_ID > 0)
                        {
                            if (CheckSumLevelOnExceptionId(Exception_ID))
                            {
                                if (!CheckException(DiscSchema_ID, _Product_ID, Exception_ID))
                                {
                                    _DiscSchemaLine_ID = DiscSchema_ID;
                                }
                                else
                                {
                                    _DiscSchemaLine_ID = 0;
                                }
                            }
                            else
                            {
                                _DiscSchemaLine_ID = 0;
                            }
                        }
                        else
                        {
                            if (CheckExceptionSumLevel(DiscSchema_ID, _Product_ID))
                            {
                                _DiscSchemaLine_ID = 0;
                            }
                            else
                            {
                                _DiscSchemaLine_ID = DiscSchema_ID;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _msg = e.Message;
            }
            return _DiscSchemaLine_ID;
        }
        // Method to check exception for summary level in discount schema
        private bool CheckException(int _DiscountSchemaLine_ID, int _Product_ID, int _Exception_ID)
        {
            bool check = false;
            try
            {
                if (ProdExcepDict.Count > 0)
                {
                    if (ProdExcepDict.ContainsKey(_DiscountSchemaLine_ID))
                    {
                        Dictionary<int, List<List<int>>> ProdDicExceptionBased = ProdExcepDict[_DiscountSchemaLine_ID];
                        if (ProdDicExceptionBased.ContainsKey(_Exception_ID))
                        {
                            List<List<int>> Array = ProdDicExceptionBased[_Exception_ID];
                            for (int i = 0; i < Array.Count; i++)
                            {
                                if (Array[i].Contains(_Product_ID))
                                {
                                    check = true;
                                    break;
                                }
                                else
                                {
                                    check = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        check = false;
                    }

                }
                else
                {
                    check = false;
                }
            }
            catch (Exception e)
            {
                _msg = e.Message;
            }
            return check;
        }
        // Method to check exception for summary level in discount schema where only summary level is bound on exception line
        private bool CheckExceptionSumLevel(int _DiscountSchemaLine_ID, int _Product_ID)
        {
            bool check = false;
            try
            {
                if (ProdExcepDictSumLevel.Count > 0)
                {
                    if (ProdExcepDictSumLevel.ContainsKey(_DiscountSchemaLine_ID))
                    {
                        List<List<int>> Array = ProdExcepDictSumLevel[_DiscountSchemaLine_ID];
                        for (int i = 0; i < Array.Count; i++)
                        {
                            if (Array[i].Contains(_Product_ID))
                            {
                                IsExceptionFound = true;
                                check = true;
                                break;
                            }
                            else
                            {
                                check = false;
                            }
                        }
                    }
                    else
                    {
                        check = false;
                    }

                }
                else
                {
                    check = false;
                }
            }
            catch (Exception e)
            {
                _msg = e.Message;
            }
            return check;
        }
        //Method to check if product exists in exception without Summary level
        private bool CheckExceptionWithoutSummaryLevel(int DiscSchema_ID, int M_Product_ID, int Exception_ID)
        {
            try
            {
                X_VAPRC_Exception exc = new X_VAPRC_Exception(ctx, Exception_ID, trx);
                if (Util.GetValueOfInt(exc.Get_Value("VAPRC_SumLevelProd")) == 0)
                {
                    //1. Check against Product Category 
                    MProduct prod = new MProduct(ctx, M_Product_ID, trx);
                    int prodCat_ID = prod.GetM_Product_Category_ID();
                    int brandID = Util.GetValueOfInt(prod.Get_Value("M_Brand_ID"));
                    int vendor_ID = 0;
                    if (exc.GetC_BPartner_ID() > 0)
                    {
                        _Sql.Append("SELECT C_Bpartner_ID FROM M_Product_PO Where IsCurrentVendor='Y' and IsActive='Y' AND M_Product_ID=" + M_Product_ID);
                        vendor_ID = Util.GetValueOfInt(DB.ExecuteScalar(_Sql.ToString()));
                    }
                    #region Exception 1-:Only Product Category 4C1
                    if (exc.GetM_Product_Category_ID() > 0
                        && exc.GetM_Brand_ID() == 0 && exc.GetM_Product_ID() == 0 && exc.GetC_BPartner_ID() == 0 &&
                        prodCat_ID == exc.GetM_Product_Category_ID()
                        )
                    {

                        IsExceptionFound = true;
                        return true;
                    }
                    #endregion
                    #region Exception 2-:Only Product 4C1
                    else if (exc.GetM_Product_Category_ID() == 0 && exc.GetM_Product_ID() > 0 &&
                             exc.GetM_Brand_ID() == 0 && exc.GetC_BPartner_ID() == 0 &&
                        M_Product_ID == exc.GetM_Product_ID())
                    {

                        IsExceptionFound = true;
                        return true;
                    }
                    #endregion
                    #region Exception 3-:Only Brand 4C1
                    else if (exc.GetM_Brand_ID() > 0 &&
                        exc.GetM_Product_ID() == 0 && exc.GetC_BPartner_ID() == 0 && exc.GetM_Product_Category_ID() == 0
                        && exc.GetM_Brand_ID() == brandID)
                    {
                        IsExceptionFound = true;
                        return true;
                    }
                    #endregion
                    #region Exception 4-:Only Vendor 4C1
                    else if (exc.GetC_BPartner_ID() > 0 &&
                      exc.GetM_Product_ID() == 0 && exc.GetM_Brand_ID() == 0 && exc.GetM_Product_Category_ID() == 0 &&
                           vendor_ID == exc.GetC_BPartner_ID())
                    {                 
                            IsExceptionFound = true;
                            return true;                       
                    }
                    #endregion
                    #region exception 5-: Product + Product Category 4C2
                    else if (exc.GetM_Product_ID() > 0 && exc.GetM_Product_Category_ID()>0 &&
                      exc.GetC_BPartner_ID() == 0 && exc.GetM_Brand_ID() == 0  &&
                          exc.GetM_Product_ID() == M_Product_ID && exc.GetM_Product_Category_ID()==prodCat_ID)
                    {
                        IsExceptionFound = true;
                        return true;
                    }
                    #endregion
                    #region exception 6-: Product + Brand 4C2
                    else if (exc.GetM_Product_ID() > 0 && exc.GetM_Product_Category_ID() == 0 &&
                      exc.GetC_BPartner_ID() == 0 && exc.GetM_Brand_ID() > 0 &&
                          exc.GetM_Product_ID() == M_Product_ID && exc.GetM_Brand_ID() == brandID)
                    {
                        IsExceptionFound = true;
                        return true;
                    }
                    #endregion
                    #region exception 7-: Product + Vendor 4C2
                    else if (exc.GetM_Product_ID() > 0 && exc.GetM_Product_Category_ID() == 0 &&
                      exc.GetC_BPartner_ID() > 0 && exc.GetM_Brand_ID() == 0 &&
                          exc.GetM_Product_ID() == M_Product_ID && exc.GetC_BPartner_ID() == vendor_ID)
                    {
                        IsExceptionFound = true;
                        return true;
                    }
                    #endregion
                    #region exception 8-: Product Category + Brand 4C2
                    else if (exc.GetM_Product_ID() == 0 && exc.GetM_Product_Category_ID() > 0 &&
                      exc.GetC_BPartner_ID() == 0 && exc.GetM_Brand_ID() > 0 &&
                          exc.GetM_Product_Category_ID() == prodCat_ID && exc.GetM_Brand_ID() == brandID)
                    {
                        IsExceptionFound = true;
                        return true;
                    }
                    #endregion
                    #region exception 9-: Product Category + Vendor 4C2
                    else if (exc.GetM_Product_ID() == 0 && exc.GetM_Product_Category_ID() > 0 &&
                      exc.GetC_BPartner_ID() >0 && exc.GetM_Brand_ID() == 0 &&
                          exc.GetM_Product_Category_ID() == prodCat_ID && exc.GetC_BPartner_ID() == vendor_ID)
                    {
                        IsExceptionFound = true;
                        return true;
                    }
                    #endregion
                    #region exception 10-: Brand + Vendor 4C2
                    else if (exc.GetM_Product_ID() == 0 && exc.GetM_Product_Category_ID() == 0 &&
                      exc.GetC_BPartner_ID() > 0 && exc.GetM_Brand_ID() > 0 &&
                          exc.GetM_Brand_ID() == brandID && exc.GetC_BPartner_ID() == vendor_ID)
                    {
                        IsExceptionFound = true;
                        return true;
                    }
                    #endregion
                    #region exception 11-: Product + Product Category + Brand 4C3
                    else if (exc.GetM_Product_ID() > 0 && exc.GetM_Product_Category_ID() > 0 &&
                      exc.GetC_BPartner_ID() == 0 && exc.GetM_Brand_ID() > 0 &&
                          exc.GetM_Brand_ID() == brandID && exc.GetM_Product_Category_ID() == prodCat_ID && exc.GetM_Product_ID() == M_Product_ID)
                    {
                        IsExceptionFound = true;
                        return true;
                    }
                    #endregion
                    #region exception 12-: Product + Product Category + Vendor 4C3
                    else if (exc.GetM_Product_ID() > 0 && exc.GetM_Product_Category_ID() > 0 &&
                      exc.GetC_BPartner_ID() > 0 && exc.GetM_Brand_ID() == 0 &&
                          exc.GetC_BPartner_ID() == vendor_ID && exc.GetM_Product_Category_ID() == prodCat_ID && exc.GetM_Product_ID() == M_Product_ID)
                    {
                        IsExceptionFound = true;
                        return true;
                    }
                    #endregion
                    #region exception 13-: Product + Brand + Vendor 4C3
                    else if (exc.GetM_Product_ID() > 0 && exc.GetM_Product_Category_ID() == 0 &&
                      exc.GetC_BPartner_ID() > 0 && exc.GetM_Brand_ID() > 0 &&
                          exc.GetC_BPartner_ID() == vendor_ID && exc.GetM_Brand_ID() == brandID && exc.GetM_Product_ID() == M_Product_ID)
                    {
                        IsExceptionFound = true;
                        return true;
                    }
                    #endregion
                    #region exception 14-: Product Category + Brand + Vendor 4C3
                    else if (exc.GetM_Product_ID() == 0 && exc.GetM_Product_Category_ID() > 0 &&
                      exc.GetC_BPartner_ID() > 0 && exc.GetM_Brand_ID() > 0 &&
                          exc.GetC_BPartner_ID() == vendor_ID && exc.GetM_Brand_ID() == brandID && exc.GetM_Product_Category_ID() == prodCat_ID)
                    {
                        IsExceptionFound = true;
                        return true;
                    }
                    #endregion
                    #region exception 15-: Product + Product Category + Brand + Vendor 4C4
                    else if (exc.GetM_Product_ID() > 0 && exc.GetM_Product_Category_ID() > 0 &&
                      exc.GetC_BPartner_ID() > 0 && exc.GetM_Brand_ID() > 0 &&
                          exc.GetC_BPartner_ID() == vendor_ID && exc.GetM_Brand_ID() == brandID &&
                        exc.GetM_Product_Category_ID() == prodCat_ID && exc.GetM_Product_ID()==M_Product_ID)
                    {
                        IsExceptionFound = true;
                        return true;
                    }
                    #endregion
                    //Note *****Formula Used for combination is nCr = n!/(r!(n-r)!)
                }
            }
            catch (Exception ex)
            {
                _msg = ex.Message;
            }
            return false;
        }
    }
    //List Class For different discount schema
    class PLDiscSchema
    {
        public int PriceListVersion_ID { get; set; }
        public int DiscSchema_ID { get; set; }
    }
    //Property class for Attribute wise Products
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
