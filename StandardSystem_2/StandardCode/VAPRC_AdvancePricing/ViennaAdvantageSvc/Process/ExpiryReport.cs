/********************************************************
    * Project Name   : Vienna Advance Pricing(VAPRC)
    * Class Name     : ExpiryReport
    * Purpose        : Used to get Expired Product Details,
    *                  Create New Pricelist Version for Expired Products and Create Movement
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological  : Development
    * Developer      : Pratap
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
using System.Data.SqlClient;

namespace ViennaAdvantageServer.Process
{
    public class ExpiryReport : SvrProcess
    {
        #region Variables
        bool _status = false;
        private string _moveDocNo = "";
        private int _Org_ID = 0;
        private int _Client_ID = 0;
        private int _Warehouse_ID = 0;
        private int _Locator_ID = 0;
        private int _ExpiryDays = 0;
        private int _LocatorTo_ID = 0;
        private int _PriceList_Version_ID;
        private string _IsApplyDiscount = "N";
        private string _Product_ID = "";
        private string _ProductIDList = "";
        private string _ProductAttributeSetId = "";
        private double _DiscountPercent = 0.0;
        private double _DiscountAmount = 0.0;
        private DateTime? _ExpiryDate = null;
        private DateTime? _today = DateTime.Now;
        private StringBuilder _sql = new StringBuilder();
        private StringBuilder _expiryDateCheck = new StringBuilder();
        private StringBuilder sqlchkProductID = new StringBuilder();
        private StringBuilder _queryLots = new StringBuilder();
        private StringBuilder _queryLots1 = new StringBuilder();
        private StringBuilder _updateProduct = new StringBuilder();
        private StringBuilder _queryMoveCreate = new StringBuilder();
        DataSet _dsProducts = new DataSet();
        #endregion
        protected override string DoIt()
        {
            try
            {
                string _query = "Select count(*) from  VAPRC_TEMP_EXPIRYREPORT";
                int no = Util.GetValueOfInt(DB.ExecuteScalar(_query, null, null));
                if (no > 0)
                {
                    _query = "Delete from  VAPRC_TEMP_EXPIRYREPORT";
                    no = DB.ExecuteQuery(_query, null, null);
                }

                _query = "Select AD_ORG_ID from  M_WAREHOUSE where M_WAREHOUSE_ID=" + _Warehouse_ID;
                _Org_ID = Util.GetValueOfInt(DB.ExecuteScalar(_query, null, null));

                if (_Locator_ID > 0)
                {
                    _sql.Append("SELECT AD_CLIENT_ID ,  AD_ORG_ID ,  M_WAREHOUSE_ID ,  M_LOCATOR_ID ,  M_PRODUCT_ID ,  VAPRC_PRODUCTKEY ,  VAPRC_EAN ,  VAPRC_PRODUCTNAME ,  LOT ,  VAPRC_EXPIRYDATE ,  QTYONHAND ,  VAPRC_EXPIRYDAYS ,  M_LOCATORTO_ID ,  VAPRC_ISAPPLYDISCOUNT ,  VAPRC_DISCOUNTPERCENT ,  VAPRC_DISCOUNTAMOUNT ,  M_PRICELIST_VERSION_ID, M_ATTRIBUTESETINSTANCE_ID, C_UOM_ID FROM VAPRC_EXPIRYREPORT_V WHERE QTYONHAND > 0 AND AD_ORG_ID=" + _Org_ID);
                }
                else
                {
                    _sql.Append("SELECT AD_CLIENT_ID ,  AD_ORG_ID ,  M_WAREHOUSE_ID , null AS M_LOCATOR_ID ,  M_PRODUCT_ID ,  VAPRC_PRODUCTKEY ,  VAPRC_EAN ,  VAPRC_PRODUCTNAME ,  LOT ,  VAPRC_EXPIRYDATE ,  SUM(QTYONHAND) AS QTYONHAND ,  VAPRC_EXPIRYDAYS ,  M_LOCATORTO_ID ,  VAPRC_ISAPPLYDISCOUNT ,  VAPRC_DISCOUNTPERCENT ,  VAPRC_DISCOUNTAMOUNT ,  M_PRICELIST_VERSION_ID, M_ATTRIBUTESETINSTANCE_ID, C_UOM_ID FROM VAPRC_EXPIRYREPORT_V WHERE QTYONHAND > 0 AND AD_ORG_ID=" + _Org_ID);
                }
                _sql.Append(" AND M_WAREHOUSE_ID=" + _Warehouse_ID);
                _ExpiryDate = _today.Value.AddDays(_ExpiryDays);

                if (_ExpiryDays == 0)
                {
                    //_sql.Append(" AND VAPRC_EXPIRYDATE < TO_DATE('" + _today.Value.ToShortDateString() + " 23:59:59','MM-DD-YYYY HH24:MI:SS')");
                    _sql.Append(" AND VAPRC_EXPIRYDATE <" + GlobalVariable.TO_DATE(_today, true));
                }
                else if (_ExpiryDays > 0)
                {
                    //_sql.Append(" AND VAPRC_EXPIRYDATE BETWEEN TO_DATE('" + _today.Value.ToShortDateString() + " 00:00:00','MM-DD-YYYY HH24:MI:SS') AND TO_DATE('" + _ExpiryDate.Value.ToShortDateString() + " 23:59:59','MM-DD-YYYY HH24:MI:SS')");
                    _sql.Append(" AND VAPRC_EXPIRYDATE BETWEEN " + GlobalVariable.TO_DATE(_today, true) + " AND " + GlobalVariable.TO_DATE(_ExpiryDate, true));
                }
                else if (_ExpiryDays < 0)
                {
                    //_sql.Append(" AND VAPRC_EXPIRYDATE BETWEEN TO_DATE('" + _ExpiryDate.Value.ToShortDateString() + " 00:00:00','MM-DD-YYYY HH24:MI:SS') AND TO_DATE('" + _today.Value.ToShortDateString() + " 23:59:59','MM-DD-YYYY HH24:MI:SS')");
                    _sql.Append(" AND VAPRC_EXPIRYDATE BETWEEN " + GlobalVariable.TO_DATE(_ExpiryDate, true) + " AND " + GlobalVariable.TO_DATE(_today, true));
                }


                if (_Locator_ID > 0)
                {
                    _sql.Append(" AND M_LOCATOR_ID=" + _Locator_ID);
                }
                if (_Product_ID != null && _Product_ID != "")
                {
                    _sql.Append(" AND M_PRODUCT_ID IN (" + _Product_ID + ")");
                    _ProductIDList = _Product_ID;
                }
                if (_Locator_ID > 0)
                {

                }
                else
                {
                    _sql.Append(" GROUP BY ( AD_CLIENT_ID ,  AD_ORG_ID ,  M_WAREHOUSE_ID , M_PRODUCT_ID ,  VAPRC_PRODUCTKEY ,  VAPRC_EAN ,  VAPRC_PRODUCTNAME ,  LOT ,  VAPRC_EXPIRYDATE ,  VAPRC_EXPIRYDAYS ,  M_LOCATORTO_ID ,  VAPRC_ISAPPLYDISCOUNT ,  VAPRC_DISCOUNTPERCENT ,  VAPRC_DISCOUNTAMOUNT ,  M_PRICELIST_VERSION_ID, M_ATTRIBUTESETINSTANCE_ID, C_UOM_ID)");
                }

                #region Insert in Temp Table
                _dsProducts = DB.ExecuteDataset(_sql.ToString());
                _sql.Clear();
                int _dsProductsCount = _dsProducts.Tables[0].Rows.Count;
                if (_dsProductsCount > 0)
                {
                    SqlParameter[] param = new SqlParameter[3];
                    for (int i = 0; i < _dsProductsCount; i++)
                    {
                        param[0] = new SqlParameter("@param0", Util.GetValueOfDecimal(_dsProducts.Tables[0].Rows[i]["QTYONHAND"]));
                        param[1] = new SqlParameter("@param1", _DiscountPercent);
                        param[2] = new SqlParameter("@param2", _DiscountAmount);
                        _sql.Clear();
                        _expiryDateCheck.Clear();
                        _expiryDateCheck.Append(" null ");
                        if (Util.GetValueOfDateTime(_dsProducts.Tables[0].Rows[i]["VAPRC_EXPIRYDATE"]) != null)
                        {
                            _expiryDateCheck.Clear();
                            _expiryDateCheck.Append(GlobalVariable.TO_DATE(Util.GetValueOfDateTime(_dsProducts.Tables[0].Rows[i]["VAPRC_EXPIRYDATE"]), true));
                        }
                        _sql.Append(@"INSERT INTO VAPRC_TEMP_EXPIRYREPORT  (   "
                         + " AD_CLIENT_ID ,    AD_ORG_ID ,    M_WAREHOUSE_ID ,    M_LOCATOR_ID ,  "
                         + " M_PRODUCT_ID ,    VAPRC_PRODUCTKEY ,    VAPRC_EAN ,    VAPRC_PRODUCTNAME , "
                         + " LOT ,  QTYONHAND ,  VAPRC_EXPIRYDATE ,  VAPRC_EXPIRYDAYS ,    M_LOCATORTO_ID , "
                         + " VAPRC_ISAPPLYDISCOUNT ,    VAPRC_DISCOUNTPERCENT ,    VAPRC_DISCOUNTAMOUNT , "
                         + "  M_PRICELIST_VERSION_ID , VAPRC_MovementCreated , M_ATTRIBUTESETINSTANCE_ID) ");
                        _sql.Append("VALUES  ( " + Util.GetValueOfInt(_dsProducts.Tables[0].Rows[i]["AD_CLIENT_ID"]) + ", "
                         + Util.GetValueOfInt(_dsProducts.Tables[0].Rows[i]["AD_ORG_ID"]) + ", "
                         + _Warehouse_ID + ", ");
                        if (_Locator_ID > 0)
                        {

                            _sql.Append(Util.GetValueOfInt(_dsProducts.Tables[0].Rows[i]["M_LOCATOR_ID"]) + ", ");
                        }
                        else
                        {
                            _sql.Append(" null " + ", ");
                        }

                        //_sql.Append(Util.GetValueOfInt(_dsProducts.Tables[0].Rows[i]["M_PRODUCT_ID"]) + ", "
                        // + " '" + Util.GetValueOfString(_dsProducts.Tables[0].Rows[i]["VAPRC_PRODUCTKEY"]) + "' ,"
                        // + " '" + Util.GetValueOfString(_dsProducts.Tables[0].Rows[i]["VAPRC_EAN"]) + "' ,"
                        // + " '" + Util.GetValueOfString(_dsProducts.Tables[0].Rows[i]["VAPRC_PRODUCTNAME"]) + "' ,"
                        // + " '" + Util.GetValueOfString(_dsProducts.Tables[0].Rows[i]["LOT"]) + "' ,"
                        // + " @param2 , ");
                        //_sql.Append(_expiryDateCheck.ToString() + ", ");
                        //_sql.Append(_ExpiryDays + ", ");
                        //_sql.Append(_LocatorTo_ID + ", ");
                        //_sql.Append(" '" + _IsApplyDiscount + "' , ");
                        //_sql.Append(" @param  , ");
                        //_sql.Append(" @param1 , ");
                        //_sql.Append(_PriceList_Version_ID + ", ");
                        //_sql.Append(" 'N' , ");
                        //_sql.Append(Util.GetValueOfInt(_dsProducts.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                        //_sql.Append(" )");
                        
                        //Changes done by Vivek 31-12-2015
                        _sql.Append(Util.GetValueOfInt(_dsProducts.Tables[0].Rows[i]["M_PRODUCT_ID"]) + ", "
                         + " '" + Util.GetValueOfString(_dsProducts.Tables[0].Rows[i]["VAPRC_PRODUCTKEY"]) + "' ,"
                         + " '" + Util.GetValueOfString(_dsProducts.Tables[0].Rows[i]["VAPRC_EAN"]) + "' ,"
                         + " '" + Util.GetValueOfString(_dsProducts.Tables[0].Rows[i]["VAPRC_PRODUCTNAME"]) + "' ,"
                         + " '" + Util.GetValueOfString(_dsProducts.Tables[0].Rows[i]["LOT"]) + "' ,"
                         + " @param0 , ");
                        _sql.Append(_expiryDateCheck.ToString() + ", ");
                        _sql.Append(_ExpiryDays + ", ");
                        _sql.Append(_LocatorTo_ID + ", ");
                        _sql.Append(" '" + _IsApplyDiscount + "' , ");
                        _sql.Append(" @param1  , ");
                        _sql.Append(" @param2 , ");
                        _sql.Append(_PriceList_Version_ID + ", ");
                        _sql.Append(" 'N' , ");
                        _sql.Append(Util.GetValueOfInt(_dsProducts.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                        _sql.Append(" )");

                        if (_Product_ID == null || _Product_ID == "")
                        {
                            if (i != _dsProductsCount - 1)
                            {
                                _ProductIDList = _ProductIDList + Util.GetValueOfInt(_dsProducts.Tables[0].Rows[i]["M_PRODUCT_ID"]) + ",";
                            }
                            if (i == _dsProductsCount - 1)
                            {
                                _ProductIDList = _ProductIDList + Util.GetValueOfInt(_dsProducts.Tables[0].Rows[i]["M_PRODUCT_ID"]);
                            }
                        }

                        if (Util.GetValueOfInt(_dsProducts.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) != 0)
                        {
                            _ProductAttributeSetId = _ProductAttributeSetId + Util.GetValueOfInt(_dsProducts.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]) + ",";
                        }

                        _expiryDateCheck.Clear();

                        int _chk = DB.ExecuteQuery(_sql.ToString(), param, null);
                    }
                    //if (_ProductAttributeSetId != "")
                    //{
                    //    _ProductAttributeSetId = _ProductAttributeSetId.ToString().Trim(',');
                    //}
                }
                #endregion
                #region Create PriceList Version
                if (_Locator_ID > 0 && _IsApplyDiscount == "Y" && _PriceList_Version_ID > 0)
                {
                    int _CountED011 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='ED011_'"));

                    int _newPriceList_Version_ID = 0;
                    string _sqlPriceVrsn = "select * from m_pricelist_version where m_pricelist_version_id=" + _PriceList_Version_ID;
                    DataSet _dsPriceVrsn = new DataSet();
                    _dsPriceVrsn = DB.ExecuteDataset(_sqlPriceVrsn);
                    DataSet _dsProductPrice = new DataSet();
                    string _sqlProductPrice = "select * from m_productprice where m_pricelist_version_id=" + _PriceList_Version_ID;
                    _dsProductPrice = DB.ExecuteDataset(_sqlProductPrice);
                    if (_dsPriceVrsn != null)
                    {
                        if (_dsPriceVrsn.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < _dsPriceVrsn.Tables[0].Rows.Count; i++)
                            {
                                MPriceListVersion _priceListVer = new MPriceListVersion(GetCtx(), 0, null);
                                _priceListVer.SetAD_Org_ID(Util.GetValueOfInt(_dsPriceVrsn.Tables[0].Rows[i]["AD_ORG_ID"]));
                                _priceListVer.SetAD_Client_ID(Util.GetValueOfInt(_dsPriceVrsn.Tables[0].Rows[i]["AD_CLIENT_ID"]));
                                _priceListVer.SetName(Util.GetValueOfString(System.DateTime.Now));
                                _priceListVer.SetM_PriceList_ID(Util.GetValueOfInt(_dsPriceVrsn.Tables[0].Rows[i]["M_PRICELIST_ID"]));
                                _priceListVer.SetM_DiscountSchema_ID(Util.GetValueOfInt(_dsPriceVrsn.Tables[0].Rows[i]["M_DISCOUNTSCHEMA_ID"]));
                                _priceListVer.SetM_Pricelist_Version_Base_ID(Util.GetValueOfInt(_dsPriceVrsn.Tables[0].Rows[i]["M_PRICELIST_VERSION_BASE_ID"]));
                                if (_priceListVer.Save())
                                {
                                    _newPriceList_Version_ID = _priceListVer.GetM_PriceList_Version_ID();
                                    if (_dsProductPrice != null)
                                    {
                                        if (_dsProductPrice.Tables[0].Rows.Count > 0)
                                        {
                                            for (int k = 0; k < _dsProductPrice.Tables[0].Rows.Count; k++)
                                            {
                                                MProductPrice _productPrice = new MProductPrice(GetCtx(), 0, null);
                                                _productPrice.SetAD_Org_ID(Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["AD_ORG_ID"]));
                                                _productPrice.SetAD_Client_ID(Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["AD_CLIENT_ID"]));
                                                _productPrice.SetM_PriceList_Version_ID(_newPriceList_Version_ID);
                                                _productPrice.SetM_Product_ID(Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["M_PRODUCT_ID"]));
                                                _productPrice.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                _productPrice.SetLot(Util.GetValueOfString(_dsProductPrice.Tables[0].Rows[k]["LOT"]));
                                                _productPrice.SetPriceList(Util.GetValueOfDecimal(_dsProductPrice.Tables[0].Rows[k]["PRICELIST"]));
                                                _productPrice.SetPriceStd(Util.GetValueOfDecimal(_dsProductPrice.Tables[0].Rows[k]["PRICESTD"]));
                                                _productPrice.SetPriceLimit(Util.GetValueOfDecimal(_dsProductPrice.Tables[0].Rows[k]["PRICELIMIT"]));
                                                if (_CountED011 > 0)
                                                {
                                                    _productPrice.SetC_UOM_ID(Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["C_UOM_ID"]));
                                                }
                                                if (_productPrice.Save())
                                                {
                                                    sqlchkProductID.Append("select m_product_id from VAPRC_TEMP_EXPIRYREPORT where M_PRODUCT_ID=" + Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["M_PRODUCT_ID"]));
                                                    int chkProductID = Util.GetValueOfInt(DB.ExecuteScalar(sqlchkProductID.ToString(), null, null));
                                                    sqlchkProductID.Clear();
                                                    if (chkProductID > 0)
                                                    {

                                                        #region Update 11/21/2014 Pratap
                                                        //string _queryLots = @"select m_product_id , M_ATTRIBUTESETINSTANCE_ID, LOT from VAPRC_TEMP_EXPIRYREPORT where M_PRODUCT_ID=" 
                                                        //    + Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["M_PRODUCT_ID"]) 
                                                        //    + " and M_ATTRIBUTESETINSTANCE_ID !=" + Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["M_ATTRIBUTESETINSTANCE_ID"])
                                                        //    + " AND M_ATTRIBUTESETINSTANCE_ID NOT IN  (SELECT m_attributesetinstance_id   FROM m_productprice   WHERE m_pricelist_version_id="
                                                        //    +_PriceList_Version_ID+"  AND m_product_id  =" + Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["M_PRODUCT_ID"]) + "  )";

                                                        _queryLots.Append(@"select m_product_id , M_ATTRIBUTESETINSTANCE_ID, LOT from VAPRC_EXPIRYREPORT_V where AD_ORG_ID=" + _Org_ID + " AND M_WAREHOUSE_ID=" + _Warehouse_ID + " AND M_LOCATOR_ID=" + _Locator_ID + " and M_PRODUCT_ID="
                                                            + Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["M_PRODUCT_ID"]));
                                                        //+ " and M_ATTRIBUTESETINSTANCE_ID !=" + Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["M_ATTRIBUTESETINSTANCE_ID"])
                                                        //+ " AND M_ATTRIBUTESETINSTANCE_ID NOT IN  (SELECT m_attributesetinstance_id   FROM m_productprice   WHERE m_pricelist_version_id="
                                                        //+ _PriceList_Version_ID + "  AND m_product_id  =" + Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["M_PRODUCT_ID"]) + "  )";
                                                        DataSet _dsProductPriceLots = new DataSet();
                                                        _dsProductPriceLots = DB.ExecuteDataset(_queryLots.ToString());
                                                        _queryLots.Clear();
                                                        if (_dsProductPriceLots.Tables[0].Rows.Count > 1)
                                                        {
                                                            _queryLots1.Append(@"select m_product_id , M_ATTRIBUTESETINSTANCE_ID, LOT from VAPRC_TEMP_EXPIRYREPORT where M_PRODUCT_ID="
                                                                + Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["M_PRODUCT_ID"])
                                                                + " and M_ATTRIBUTESETINSTANCE_ID !=" + Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["M_ATTRIBUTESETINSTANCE_ID"])
                                                                + " AND M_ATTRIBUTESETINSTANCE_ID NOT IN  (SELECT m_attributesetinstance_id   FROM m_productprice   WHERE m_pricelist_version_id="
                                                                + _PriceList_Version_ID + "  AND m_product_id  =" + Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["M_PRODUCT_ID"]) + "  )");
                                                            DataSet _dsProductPriceLots1 = new DataSet();
                                                            _dsProductPriceLots1 = DB.ExecuteDataset(_queryLots1.ToString());
                                                            _queryLots1.Clear();
                                                            for (int m = 0; m < _dsProductPriceLots1.Tables[0].Rows.Count; m++)
                                                            {
                                                                // _ProductAttributeSetId = _ProductAttributeSetId + Util.GetValueOfInt(_dsProductPriceLots1.Tables[0].Rows[m]["M_ATTRIBUTESETINSTANCE_ID"]) + ",";
                                                                MProductPrice _productPriceLot = new MProductPrice(GetCtx(), 0, null);
                                                                _productPriceLot.SetAD_Org_ID(Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["AD_ORG_ID"]));
                                                                _productPriceLot.SetAD_Client_ID(Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["AD_CLIENT_ID"]));
                                                                _productPriceLot.SetM_PriceList_Version_ID(_newPriceList_Version_ID);
                                                                _productPriceLot.SetM_Product_ID(Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["M_PRODUCT_ID"]));
                                                                _productPriceLot.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(_dsProductPriceLots1.Tables[0].Rows[m]["M_ATTRIBUTESETINSTANCE_ID"]));
                                                                _productPriceLot.SetLot(Util.GetValueOfString(_dsProductPriceLots1.Tables[0].Rows[m]["LOT"]));
                                                                _productPriceLot.SetPriceList(Util.GetValueOfDecimal(_dsProductPrice.Tables[0].Rows[k]["PRICELIST"]));
                                                                _productPriceLot.SetPriceStd(Util.GetValueOfDecimal(_dsProductPrice.Tables[0].Rows[k]["PRICESTD"]));
                                                                _productPriceLot.SetPriceLimit(Util.GetValueOfDecimal(_dsProductPrice.Tables[0].Rows[k]["PRICELIMIT"]));
                                                                if (_CountED011 > 0)
                                                                {
                                                                    _productPriceLot.SetC_UOM_ID(Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["C_UOM_ID"]));
                                                                }
                                                                if (!_productPriceLot.Save())
                                                                {
                                                                }
                                                            }
                                                        }
                                                        else if (_dsProductPriceLots.Tables[0].Rows.Count == 1 && Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["M_ATTRIBUTESETINSTANCE_ID"]) == 0)
                                                        {
                                                            SqlParameter[] param = new SqlParameter[1];
                                                            _updateProduct.Clear();
                                                            if (_DiscountPercent > 0)
                                                            {
                                                                param[0] = new SqlParameter("@param", _DiscountPercent);
                                                                _updateProduct.Append("UPDATE m_productprice SET PRICELIMIT= ROUND((PRICELIMIT-((PRICELIMIT * @param )/100)),2),  PRICELIST=ROUND((PRICELIST-((PRICELIST* @param )/100)),2),  PRICESTD=ROUND((PRICESTD-((PRICESTD* @param )/100)),2) WHERE m_pricelist_version_id=" + _newPriceList_Version_ID + " AND m_product_id  =" + Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["M_PRODUCT_ID"]) + " AND M_ATTRIBUTESETINSTANCE_ID = 0 ");
                                                            }

                                                            else if (_DiscountAmount > 0)
                                                            {
                                                                param[0] = new SqlParameter("@param", _DiscountAmount);
                                                                _updateProduct.Append("UPDATE m_productprice SET PRICELIMIT= ROUND((PRICELIMIT- @param ),2),  PRICELIST=ROUND((PRICELIST- @param ),2),  PRICESTD=ROUND((PRICESTD- @param ),2) WHERE m_pricelist_version_id=" + _newPriceList_Version_ID + " AND m_product_id  = " + Util.GetValueOfInt(_dsProductPrice.Tables[0].Rows[k]["M_PRODUCT_ID"]) + " AND M_ATTRIBUTESETINSTANCE_ID = 0 ");
                                                            }
                                                            int _chkUpdate = DB.ExecuteQuery(_updateProduct.ToString(), param, null);
                                                            _updateProduct.Clear();
                                                        }
                                                        _dsProductPriceLots.Dispose();
                                                        #endregion
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    _dsProductPrice.Dispose();
                                }
                            }
                        }
                    }
                    _dsPriceVrsn.Dispose();
                    if (_ProductAttributeSetId != "")
                    {
                        _ProductAttributeSetId = _ProductAttributeSetId.ToString().Trim(',');
                    }
                    if (_ProductIDList != "" && _ProductIDList != null)
                    {
                        SqlParameter[] param = new SqlParameter[1];
                        string _updateProduct = "";
                        if (_DiscountPercent > 0)
                        {
                            param[0] = new SqlParameter("@param", _DiscountPercent);
                            _updateProduct = "UPDATE m_productprice SET PRICELIMIT= ROUND((PRICELIMIT-((PRICELIMIT* @param )/100)),2),  PRICELIST=ROUND((PRICELIST-((PRICELIST* @param )/100)),2),  PRICESTD=ROUND((PRICESTD-((PRICESTD* @param )/100)),2) WHERE m_pricelist_version_id=" + _newPriceList_Version_ID + " AND m_product_id  IN (" + _ProductIDList + ") AND M_ATTRIBUTESETINSTANCE_ID IN (" + _ProductAttributeSetId + ") ";
                        }

                        else if (_DiscountAmount > 0)
                        {
                            param[0] = new SqlParameter("@param", _DiscountAmount);
                            _updateProduct = "UPDATE m_productprice SET PRICELIMIT= ROUND((PRICELIMIT- @param ),2),  PRICELIST=ROUND((PRICELIST- @param ),2),  PRICESTD=ROUND((PRICESTD- @param ),2) WHERE m_pricelist_version_id=" + _newPriceList_Version_ID + " AND m_product_id  IN (" + _ProductIDList + ") AND M_ATTRIBUTESETINSTANCE_ID IN (" + _ProductAttributeSetId + ") ";
                        }
                        int _chkUpdate = DB.ExecuteQuery(_updateProduct, param, null);
                        if (_chkUpdate == 1)
                        {

                        }
                    }
                }
                #endregion
                #region Move Products
                if (_Locator_ID > 0 && _LocatorTo_ID > 0 && _dsProductsCount > 0 && _Locator_ID != _LocatorTo_ID)
                {
                    _query = "SELECT c_doctype_id FROM c_doctype WHERE docbasetype = 'MMM' AND ad_client_id  =" + _Client_ID;
                    int _docTypeID = Util.GetValueOfInt(DB.ExecuteScalar(_query, null, null));
                    MMovement _move = new MMovement(GetCtx(), 0, null);
                    _move.SetC_DocType_ID(_docTypeID);
                    _move.SetMovementDate(System.DateTime.Now);
                    _move.SetAD_Org_ID(_Org_ID);
                    int _CountDTD001 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='DTD001_'"));
                    if (_CountDTD001 > 0)
                    {
                        _query = "SELECT m_warehouse_id FROM m_locator WHERE m_locator_id=" + _LocatorTo_ID;
                        int _ToWarehouse_ID = Util.GetValueOfInt(DB.ExecuteScalar(_query, null, null));
                        _move.SetDTD001_MWarehouseSource_ID(_Warehouse_ID);
                        _move.SetM_Warehouse_ID(_ToWarehouse_ID);
                    }

                    if (_move.Save())
                    {
                        int _movementId = _move.GetM_Movement_ID();
                        if (_dsProductsCount > 0)
                        {
                            for (int i = 0; i < _dsProductsCount; i++)
                            {
                                MMovementLine _moveLine = new MMovementLine(GetCtx(), 0, null);
                                _moveLine.SetAD_Org_ID(_Org_ID);
                                _moveLine.SetM_Locator_ID(_Locator_ID);
                                _moveLine.SetM_LocatorTo_ID(_LocatorTo_ID);
                                _moveLine.SetM_Movement_ID(_movementId);
                                _moveLine.SetM_Product_ID(Util.GetValueOfInt(_dsProducts.Tables[0].Rows[i]["M_PRODUCT_ID"]));
                                _moveLine.SetMovementQty(Util.GetValueOfDecimal(_dsProducts.Tables[0].Rows[i]["QTYONHAND"]));
                                
                                // Set Qty Entered and UOM
                                _moveLine.SetQtyEntered(Util.GetValueOfDecimal(_dsProducts.Tables[0].Rows[i]["QTYONHAND"]));
                                _moveLine.SetC_UOM_ID(Util.GetValueOfInt(_dsProducts.Tables[0].Rows[i]["C_UOM_ID"]));
                                _moveLine.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(_dsProducts.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                if (_moveLine.Save())
                                {
                                    _queryMoveCreate.Append("update  VAPRC_TEMP_EXPIRYREPORT set VAPRC_MovementCreated='Y' where M_Locator_ID=" + _Locator_ID + " and M_Product_ID=" + Util.GetValueOfInt(_dsProducts.Tables[0].Rows[i]["M_PRODUCT_ID"]) + " and M_ATTRIBUTESETINSTANCE_ID=" + Util.GetValueOfInt(_dsProducts.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]));
                                    int noMoveCreate = DB.ExecuteQuery(_queryMoveCreate.ToString(), null, null);
                                    _queryMoveCreate.Clear();
                                }
                            }

                            string chk = _move.CompleteIt();
                            if (chk == "CO")
                            {
                                //ViennaAdvantage.Model.MMovement _moveCmp = new ViennaAdvantage.Model.MMovement(GetCtx(), _movementId, null);
                                _move.SetDocStatus("CO");
                                _move.SetDocAction("CL");
                                _move.SetProcessed(true);
                                if (_move.Save())
                                {
                                    _moveDocNo = _move.GetDocumentNo();
                                    _status = true;
                                }
                            }
                            else if (chk == "IP")
                            {
                                // ViennaAdvantage.Model.MMovement _moveCmp = new ViennaAdvantage.Model.MMovement(GetCtx(), _movementId, null);
                                _move.SetDocStatus("IP");
                                if (_move.Save())
                                {
                                    _moveDocNo = _move.GetDocumentNo();
                                    _status = true;
                                }
                            }
                        }
                    }
                }
                #endregion
                _dsProducts.Dispose();
            }
            catch
            {
            }
            if (_status)
            {
                return Msg.GetMsg(GetCtx(), "VAPRC_MovementCreated") + _moveDocNo;
            }
            return "";
        }

        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String _paraName = para[i].GetParameterName();
                if (_paraName == null)
                {
                    ;
                }
                else if (_paraName.Equals("VAPRC_Warehouse"))
                {
                    _Warehouse_ID = para[i].GetParameterAsInt();
                }
                else if (_paraName.Equals("M_Locator_ID"))
                {
                    _Locator_ID = para[i].GetParameterAsInt();
                }
                else if (_paraName.Equals("M_Product_ID"))
                {
                    _Product_ID = Util.GetValueOfString(para[i].GetParameter());
                }
                else if (_paraName.Equals("VAPRC_ExpiryDays"))
                {
                    _ExpiryDays = para[i].GetParameterAsInt();
                }
                else if (_paraName.Equals("M_LocatorTo_ID"))
                {
                    _LocatorTo_ID = para[i].GetParameterAsInt();
                }
                else if (_paraName.Equals("VAPRC_IsApplyDiscount"))
                {
                    _IsApplyDiscount = Util.GetValueOfString(para[i].GetParameter());
                }
                else if (_paraName.Equals("VAPRC_DiscountPercent"))
                {
                    _DiscountPercent = Util.GetValueOfDouble(para[i].GetParameter());
                }
                else if (_paraName.Equals("VAPRC_DiscountAmount"))
                {
                    _DiscountAmount = Util.GetValueOfDouble(para[i].GetParameter());
                }
                else if (_paraName.Equals("M_PriceList_Version_ID"))
                {
                    _PriceList_Version_ID = para[i].GetParameterAsInt();
                }

            }
            _Client_ID = GetCtx().GetAD_Client_ID();
        }
    }
}
