/** 
  *    Sample Class for Callout
       -  must call base class (CalloutEngine)debugger
       -- must inheirt Base class
  */
; VA027 = window.VA027 || {};

; (function (VA027, $) {
    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;

    function VA027_CalloutPostPayment() {
        VIS.CalloutEngine.call(this, "VA027.VA027_CalloutPostPayment");//must call
    };
    VIS.Utility.inheritPrototype(VA027_CalloutPostPayment, VIS.CalloutEngine);
    //-------Order---------
    VA027_CalloutPostPayment.prototype.Order = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        var C_Order_ID = value;
        if (this.isCalloutActive()		//	assuming it is resetting value
            || C_Order_ID == null
            || C_Order_ID == 0) {
            return "";
        }
        this.setCalloutActive(true);
        mTab.setValue("C_Invoice_ID", null);
        mTab.setValue("C_Charge_ID", null);
        mTab.setValue("VA027_IsPrepayment", true);// Boolean.TRUE);
        //
        mTab.setValue("VA027_DiscountAmt", VIS.Env.ZERO);
        mTab.setValue("VA027_WriteoffAmt", VIS.Env.ZERO);
        //var ts = mTab.getValue("VA027_TrxDate");
        //if (ts == null) {

        //    ts = new Date();
        //}

        //var sql = "";
        //var _CountVA009 = Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'"));
        //if (_CountVA009 > 0) {
        //    sql = "SELECT VA009_PaymentMethod_ID,C_BPartner_ID,C_Currency_ID, GrandTotal "
        //    + "FROM C_Order WHERE C_Order_ID=@param1";  // #1
        //}
        //else {
        //    sql = "SELECT C_BPartner_ID,C_Currency_ID, GrandTotal "
        //    + "FROM C_Order WHERE C_Order_ID=@param1";  // #1
        //}

        var paramStr = "";
        var dr = null;
        //var param = [];
        try {
            //param[0] = new VIS.DB.SqlParam("@param1", C_Order_ID);
            //dr = VIS.DB.executeReader(sql, param, null);

            //if (dr.read()) {
            //    var C_Currency_ID = Util.getValueOfInt(dr.get("c_currency_id"));
            //    mTab.setValue("C_Currency_ID", C_Currency_ID);
            //    var grandTotal = Util.getValueOfDecimal(dr.get("grandtotal"));
            //    if (grandTotal == null) {
            //        grandTotal = VIS.Env.ZERO;
            //    }
            //    if (_CountVA009 > 0) {
            //        var PaymentMethod = Util.getValueOfInt(dr.get("va009_paymentmethod_id"));
            //        mTab.setValue("VA009_PaymentMethod_ID", PaymentMethod);
            //    }
            //    mTab.setValue("VA027_PayAmt", grandTotal);
            //}
            //dr.close();

            paramStr = "C_Order," + value.toString();
            dr = VIS.dataContext.getJSONRecord("PDC/GetBPData", paramStr);
            if (dr != null) {
                mTab.setValue("C_Bpartner_ID", dr["C_BPartner_ID"]);
                mTab.setValue("C_Bpartner_Location_ID", dr["C_BPartner_Location_ID"]);
            }
        }
        catch (err) {
            if (dr != null) {
                dr.close();
                dr = null;
            }
            this.log.log(Level.SEVERE, sql, err);
            this.setCalloutActive(false);
            return err.message;
        }

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
    };

    //---BPartner---------
    VA027_CalloutPostPayment.prototype.BPartner = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        var _bpartner = Util.getValueOfInt(mTab.getValue("C_BPartner_ID"));
        //var _client = Util.getValueOfInt(mTab.getValue("AD_Client_ID"));
        if (_bpartner != 0) {
            try {
                var paramString = _bpartner.toString();
                var VA009_PAYMENTMETHOD_ID = VIS.dataContext.getJSONRecord("VA027/PDC/GetPaymentMethodFromBP", paramString);
                mTab.setValue("VA009_PAYMENTMETHOD_ID", VA009_PAYMENTMETHOD_ID);
            }
            catch (err) {
                this.log.severe(err.toString());
            };

        };


        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldvalue = null;
        return "";
    };

    //---Order Schedule---------
    VA027_CalloutPostPayment.prototype.OrderPaySchedule = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            //modified by arpit**********
            if ((Util.getValueOfInt(mTab.getValue("VA009_ORDERPAYSCHEDULE_ID"))) == null || (Util.getValueOfInt(mTab.getValue("VA009_ORDERPAYSCHEDULE_ID"))) == 0) {
                mTab.setValue("VA027_PayAmt", 0);
                mTab.setValue("VA027_ConvertedAmount", 0);
                this.setCalloutActive(false);
                return "";
            }
            return "";
        }
        this.setCalloutActive(true);
        var _orderSchedule = Util.getValueOfInt(mTab.getValue("VA009_ORDERPAYSCHEDULE_ID"));

        if (_orderSchedule != 0) {
            try {
                var paramString = _orderSchedule.toString();
                var GetVA009_OrderPayScheduleDetail = VIS.dataContext.getJSONRecord("VA027/PDC/GetVA009_OrderPayScheduleDetail", paramString);
                if (GetVA009_OrderPayScheduleDetail != null) {
                    mTab.setValue("VA027_PayAmt", Util.getValueOfDecimal(GetVA009_OrderPayScheduleDetail["VA027_PayAmt"]));
                    mTab.setValue("VA009_PAYMENTMETHOD_ID", Util.getValueOfInt(GetVA009_OrderPayScheduleDetail["va009_paymentmethod_id"]));
                    mTab.setValue("VA027_DISCOUNTAMT", Util.getValueOfDecimal(GetVA009_OrderPayScheduleDetail["VA027_DISCOUNTAMT"]));
                    mTab.setValue("VA027_TRXDATE", Util.getValueOfDate(GetVA009_OrderPayScheduleDetail["VA027_TRXDATE"]));
                    mTab.setValue("DateAcct", Util.getValueOfDate(GetVA009_OrderPayScheduleDetail["DateAcct"]));
                }
            }
            catch (err) {
                this.log.severe(err.toString());
            };

        };
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldvalue = null;
        return "";
    };

    //---Invoice Schedule---------
    VA027_CalloutPostPayment.prototype.InvoicePaySchedule = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            //modified by arpit**********
            if ((Util.getValueOfInt(mTab.getValue("C_INVOICEPAYSCHEDULE_ID"))) == null || (Util.getValueOfInt(mTab.getValue("C_INVOICEPAYSCHEDULE_ID"))) == 0) {
                mTab.setValue("VA027_PayAmt", 0);
                mTab.setValue("VA027_ConvertedAmount", 0);
                this.setCalloutActive(false);
                return "";
            }
            return "";
        }
        this.setCalloutActive(true);
        var _invSchedule = Util.getValueOfInt(mTab.getValue("C_INVOICEPAYSCHEDULE_ID"));
        //modified by arpit**********
        if (_invSchedule == null || _invSchedule == 0) {
            mTab.setValue("VA027_PayAmt", 0);
            this.setCalloutActive(false);
            ctx = windowNo = mTab = mField = value = oldvalue = null;
            return "";
        }

        var _bP = Util.getValueOfInt(mTab.getValue("C_BPARTNER_ID"));
        try {
            if (_invSchedule != 0 && _bP != 0) {
                var paramString = _invSchedule.toString();
                var GetInvoiceScheduleDetail = VIS.dataContext.getJSONRecord("VA027/PDC/GetInvoiceScheduleDetail", paramString);
                if (GetInvoiceScheduleDetail != null) {
                    mTab.setValue("VA027_PayAmt", Util.getValueOfDecimal(GetInvoiceScheduleDetail["VA027_PayAmt"]));
                    mTab.setValue("VA009_PAYMENTMETHOD_ID", Util.getValueOfInt(GetInvoiceScheduleDetail["va009_paymentmethod_id"]));
                    mTab.setValue("VA027_DISCOUNTAMT", Util.getValueOfDecimal(GetInvoiceScheduleDetail["VA027_DISCOUNTAMT"]));
                    mTab.setValue("VA027_TRXDATE", Util.getValueOfDate(GetInvoiceScheduleDetail["VA027_TRXDATE"]));
                    mTab.setValue("DateAcct", Util.getValueOfDate(GetInvoiceScheduleDetail["DateAcct"]));
                }
            }
        }
        catch (err) {
            this.log.severe(err.toString());
        };
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldvalue = null;
        return "";

    };

    //---Set Amount Field At PDC-Allocate -------
    VA027_CalloutPostPayment.prototype.InvoicePayScheduleAmount = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            if ((Util.getValueOfInt(mTab.getValue("C_INVOICEPAYSCHEDULE_ID"))) == 0) {
                mTab.setValue("Amount", 0);
                return "";
            }
            return "";
        }
        this.setCalloutActive(true);
        var dr = null;
        var dueAmt = 0;

        var colName = mField.getColumnName();
        var IsReturnTrx = "";
        var _invSchedule = Util.getValueOfInt(mTab.getValue("C_INVOICEPAYSCHEDULE_ID"));
        var writeoffAmt = Util.getValueOfDecimal(mTab.getValue("WriteOffAmt") == null ? VIS.Env.ZERO : mTab.getValue("WriteOffAmt"));
        var discountAmt = Util.getValueOfDecimal(mTab.getValue("DiscountAmt") == null ? VIS.Env.ZERO : mTab.getValue("DiscountAmt"));
        var amount = Util.getValueOfDecimal(mTab.getValue("Amount") == null ? VIS.Env.ZERO : mTab.getValue("Amount"));

        try {
            if (_invSchedule != 0) {
                dr = VIS.dataContext.getJSONRecord("PDC/GetInvoicePayscheduleData", _invSchedule.toString());
                if (dr != null) {
                    dueAmt = Util.getValueOfDecimal(dr["DueAmt"]);
                    mTab.setValue("InvoiceAmt", dueAmt);
                    mTab.setValue("Amount", dueAmt);
                    //Amount should be nagative in case of Return Invoice
                    IsReturnTrx = Util.getValueOfString(dr["IsReturnTrx"]);
                    if ("Y" == IsReturnTrx) {
                        if (dueAmt > 0) {
                            dueAmt = (dueAmt) * (-1);
                            mTab.setValue("InvoiceAmt", dueAmt);
                            mTab.setValue("Amount", dueAmt);
                        }
                        if (writeoffAmt > 0) {
                            writeoffAmt = (writeoffAmt) * (-1);
                            mTab.setValue("WriteOffAmt", writeoffAmt);
                        }
                        if (discountAmt > 0) {
                            discountAmt = (discountAmt) * (-1);
                            mTab.setValue("DiscountAmt", discountAmt);
                        }
                    }
                }
            }
            if (colName.equals("WriteOffAmt") || colName.equals("DiscountAmt") || colName.equals("Amount")) {
                amount = dueAmt - discountAmt - writeoffAmt;
                mTab.setValue("Amount", amount);
            }
        }
        catch (err) {
            this.log.severe(err.toString());
            this.setCalloutActive(false);
            return err.message;
        };
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldvalue = null;
        return "";
    };

    // PDC-Header -- Update VA027_PayAmt on chnage of Writeoff and Discount Amount
    VA027_CalloutPostPayment.prototype.WriteOffAmt = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);

        var _OrdPaySchedule = Util.getValueOfInt(mTab.getValue("VA009_OrderPaySchedule_ID"));
        var _invSchedule = Util.getValueOfInt(mTab.getValue("C_INVOICEPAYSCHEDULE_ID"));
        var _writeOffAmt = Util.getValueOfInt(mTab.getValue("VA027_WriteoffAmt"));
        var _discountAmt = Util.getValueOfDecimal(mTab.getValue("VA027_DiscountAmt"));
        var _amt = 0;

        try {
            if (_invSchedule != 0) {
                dr = VIS.dataContext.getJSONRecord("PDC/GetInvoicePayscheduleData", _invSchedule.toString());
                if (dr != null) {
                    _amt = Util.getValueOfDecimal(dr["DueAmt"]);
                }
            }
            if (_OrdPaySchedule != 0) {
                dr = VIS.dataContext.getJSONRecord("PDC/GetOrderPayScheduleData", _OrdPaySchedule.toString());
                if (dr != null) {
                    _amt = Util.getValueOfDecimal(dr["DueAmt"]);
                }
            }
            mTab.setValue("VA027_PayAmt", (_amt - _discountAmt - _writeOffAmt));
        }
        catch (err) {
            this.log.severe(err.toString());
            this.setCalloutActive(false);
            return err.message;
        };
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldvalue = null;
        return "";

    };

    //-------DocType--------
    VA027_CalloutPostPayment.prototype.DocType = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        var _inv = Util.getValueOfInt(mTab.getValue("C_Invoice_ID"));
        var _order = Util.getValueOfInt(mTab.getValue("C_Order_ID"));
        var _docBase = "", _docBaseType = "";
        var dr = null;

        var docType_ID = Util.getValueOfInt(mTab.getValue("C_DocType_ID"));
        if (docType_ID > 0) {
            dr = VIS.dataContext.getJSONRecord("PDC/GetDocBaseType", "C_DocType," + docType_ID.toString());
            if (dr != null) {
                _docBase = dr["DocBaseType"];

                // Set Value of Sales Transaction in Context, as Sales Transaction is used while openning the Info windows.
                // If document type is PDC Payment, On BP info should have Vendor checkbox true else Customer Checkbox should be true.
                if (_docBase == "PDP") {
                    mTab.setValue("IsSOTrx", "N");
                }
                else if (_docBase == "PDR") {
                    mTab.setValue("IsSOTrx", "Y");
                }
            }
        }

        if (_inv != 0) {
            //var _sql = "SELECT DOC.DOCBASETYPE FROM C_INVOICE INV INNER JOIN C_DOCTYPE DOC ON "
            //             + "INV.C_DOCTYPE_ID=DOC.C_DOCTYPE_ID WHERE INV.ISACTIVE='Y' AND INV.C_INVOICE_ID=" + _inv;// + " AND INV.AD_Client_ID = " + _client;//+ VIS.context.ctx["#AD_Client_ID"];
            //var _docBaseType = VIS.DB.executeScalar(_sql);
            //var _ID = Util.getValueOfInt(mTab.getValue("C_DocType_ID"));
            //if (_ID > 0) {
            //    var _qry = "SELECT DOC.DOCBASETYPE FROM C_DOCTYPE DOC WHERE DOC.ISACTIVE   ='Y' "
            //                  + " AND DOC.C_DocType_ID=" + _ID;// + " AND DOC.AD_Client_ID = " + _client;//+ VIS.context.ctx["#AD_Client_ID"];
            //}

            //var _docBase = VIS.DB.executeScalar(_qry);

            // Removed client side queries
            dr = VIS.dataContext.getJSONRecord("PDC/GetDocBaseType", "C_Invoice," + _inv.toString());
            if (dr != null) {
                _docBaseType = dr["DocBaseType"];
                if (_docBaseType == "ARI" || _docBaseType == "ARC") {
                    if (_docBase == "PDP") {
                        this.setCalloutActive(false);
                        return "VA027_PaymentDocTypeInvoiceInconsistent";
                    }
                }
                if (_docBaseType == "API" || _docBaseType == "APC") {
                    if (_docBase == "PDR") {
                        this.setCalloutActive(false);
                        return "VA027_PaymentDocTypeInvoiceInconsistent";
                    }
                }
            }
        }

        if (_order != 0) {
            //var _sql = "SELECT DOC.DOCBASETYPE FROM C_ORDER ORD INNER JOIN C_DOCTYPE DOC ON ORD.C_DOCTYPE_ID=DOC.C_DOCTYPE_ID WHERE ORD.ISACTIVE='Y' AND ORD.C_ORDER_ID=" + _order;// + " AND ORD.AD_Client_ID = " + _client;//+ VIS.context.ctx["#AD_Client_ID"];
            //var _docBaseType = VIS.DB.executeScalar(_sql);
            //var _ID = Util.getValueOfInt(mTab.getValue("C_DocType_ID"));
            //if (_ID > 0) {
            //    var _qry = "SELECT DOC.DOCBASETYPE FROM C_DOCTYPE DOC WHERE DOC.ISACTIVE   ='Y' "
            //                  + " AND DOC.C_DocType_ID=" + _ID;
            //}
            //var _docBase = VIS.DB.executeScalar(_qry);
            //var _dt = "SELECT ISSOTRX,ISRETURNTRX,GRANDTOTAL FROM C_ORDER WHERE C_ORDER_ID=" + _order;// + " AND AD_Client_ID = " + _client;//+ VIS.context.ctx["#AD_Client_ID"];
            //var ds = VIS.DB.executeDataSet(_dt);
            //var _trx = Util.getValueOfBoolean(ds.getTables()[0].getRows()[0].getCell("issotrx"));
            //var _return = Util.getValueOfBoolean(ds.getTables()[0].getRows()[0].getCell("isreturntrx"));
            //var _total = Util.getValueOfDecimal(ds.getTables()[0].getRows()[0].getCell("grandtotal"));

            // Removed client side queries
            dr = VIS.dataContext.getJSONRecord("PDC/GetDocBaseType", "C_Order," + _order.toString());
            if (dr != null) {
                var _trx = dr["IsSOTrx"] == "Y";
                var _return = dr["IsReturnTrx"] == "Y";
                var _total = dr["GrandTotal"];
                _docBaseType = dr["DocBaseType"];

                if (docType_ID > 0) {
                    if (_docBaseType == "SOO" && _trx && !_return) //SO
                    {
                        if (_docBase == "PDP") {
                            this.setCalloutActive(false);
                            return "VA027_PaymentDocTypeInvoiceInconsistent";
                        }
                    }
                    else if (_docBaseType == "SOO" && _trx && _return) // Customer RMA
                    {

                        if (_docBase == "PDP") {
                            this.setCalloutActive(false);
                            return "VA027_PaymentDocTypeInvoiceInconsistent";
                        }
                        else {
                            mTab.setValue("VA027_PayAmt", -Math.abs(_total));
                        }
                    }
                    else if (_docBaseType == "POO" && !_trx && !_return) //PO
                    {
                        if (_docBase == "PDR") {
                            this.setCalloutActive(false);
                            return "VA027_PaymentDocTypeInvoiceInconsistent";
                        }
                    }

                    else if (_docBaseType == "POO" && !_trx && _return) //Vendor RMA
                    {
                        if (_docBase == "PDP") {
                            mTab.setValue("VA027_PayAmt", -Math.abs(_total));
                        }
                        else {
                            this.setCalloutActive(false);
                            return "VA027_PaymentDocTypeInvoiceInconsistent";
                        }
                    }
                }
            }
        }

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldvalue = null;
        return "";
    };

    //----------Trx Date---------
    VA027_CalloutPostPayment.prototype.TrxDate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        var _trxDate = Util.getValueOfDate(mTab.getValue("VA027_TrxDate"));
        var _orderSchedule = Util.getValueOfInt(mTab.getValue("VA009_ORDERPAYSCHEDULE_ID"));
        var _invSchedule = Util.getValueOfInt(mTab.getValue("C_INVOICEPAYSCHEDULE_ID"));
        var _writeOffAmt = Util.getValueOfInt(mTab.getValue("VA027_WriteoffAmt"));
        if (_orderSchedule != 0 || _invSchedule != 0) {
            try {
                var paramString = _orderSchedule.toString() + ", " + _invSchedule.toString();
                var GetDiscountDateSchedule = VIS.dataContext.getJSONRecord("VA027/PDC/GetDiscountDateSchedule", paramString);
                if (GetDiscountDateSchedule != null) {
                    var _payAmt = 0;
                    var _dueAmt = Util.getValueOfDecimal(GetDiscountDateSchedule["DUEAMT"]);
                    var _discount2 = Util.getValueOfDecimal(GetDiscountDateSchedule["DISCOUNT2"]);
                    var _discountAmt = Util.getValueOfDecimal(GetDiscountDateSchedule["VA027_DISCOUNTAMT"]);
                    var _discountDate = Util.getValueOfDate(GetDiscountDateSchedule["DISCOUNTDATE"]);
                    var _discountDays = Util.getValueOfDate(GetDiscountDateSchedule["DISCOUNTDAYS2"]);
                    if (_trxDate <= _discountDate) {
                        _payAmt = _dueAmt - _discountAmt - _writeOffAmt;
                        mTab.setValue("VA027_PayAmt", _payAmt);
                        mTab.setValue("VA027_DiscountAmt", _discountAmt);
                    }
                    else if (_trxDate <= _discountDays) {
                        _payAmt = _dueAmt - _discount2 - _writeOffAmt;
                        mTab.setValue("VA027_PayAmt", _payAmt);
                        mTab.setValue("VA027_DiscountAmt", _discount2);
                    }
                    else if (_trxDate > _discountDate || _trxDate > _discountDays) {
                        mTab.setValue("VA027_PayAmt", _dueAmt);
                        mTab.setValue("VA027_DiscountAmt", VIS.Env.ZERO);
                        mTab.setValue("VA027_WriteoffAmt", VIS.Env.ZERO);
                    }
                }
            }
            catch (err) {
                this.log.severe(err.toString());
            }
            this.setCalloutActive(false);
            this.SetConvAmt(ctx, windowNo, mTab, mField, value, oldValue);
            ctx = windowNo = mTab = mField = value = oldvalue = null;
            return "";
        }

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldvalue = null;
        return "";

    };

    //-----Tax Rate----------
    VA027_CalloutPostPayment.prototype.PaymentTaxAmount = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        try {
            // get precision from currency
            var currency = VIS.dataContext.getJSONRecord("MCurrency/GetCurrency", mTab.getValue("C_Currency_ID").toString());
            var StdPrecision = currency["StdPrecision"];

            if (mField.getColumnName() == "C_Tax_ID") {
                if (Util.getValueOfDecimal(mTab.getValue("VA027_PayAmt")) > 0) {
                    var dr = null;
                    // if Surcharge Tax is selected on Tax Rate, calculate surcharge tax amount accordingly
                    if (mTab.getField("SurchargeAmt") != null) {
                        dr = VIS.dataContext.getJSONRecord("MTax/CalculateSurcharge", value.toString() + "," + mTab.getValue("VA027_PayAmt").toString() + "," + StdPrecision.toString());
                        mTab.setValue("TaxAmount", dr["TaxAmt"]);
                        mTab.setValue("SurchargeAmt", dr["SurchargeAmt"]);
                        this.setCalloutActive(false);
                        return "";
                    }
                    else {
                        var Rate = VIS.dataContext.getJSONRecord("MTax/GetTaxRate", value.toString());
                        if (Rate > 0) {
                            var TaxAmt = Util.getValueOfDecimal(Util.getValueOfDecimal(mTab.getValue("VA027_PayAmt")) - (Util.getValueOfDecimal(mTab.getValue("VA027_PayAmt")) / ((Rate / 100) + 1)));
                            mTab.setValue("TaxAmount", Util.getValueOfDecimal(TaxAmt.toFixed(2)));
                        }
                        else {
                            mTab.setValue("TaxAmount", 0);
                            this.setCalloutActive(false);
                            return "";
                        }
                    }
                }
                else {
                    this.setCalloutActive(false);
                    return "";
                }
            }
            else {
                if (Util.getValueOfInt(mTab.getValue("C_Tax_ID")) > 0) {
                    var dr = null;
                    // if Surcharge Tax is selected on Tax Rate, calculate surcharge tax amount accordingly
                    if (mTab.getField("SurchargeAmt") != null) {
                        dr = VIS.dataContext.getJSONRecord("MTax/CalculateSurcharge", mTab.getValue("C_Tax_ID").toString() + "," + mTab.getValue("VA027_PayAmt").toString() + "," + StdPrecision.toString());
                        mTab.setValue("TaxAmount", dr["TaxAmt"]);
                        mTab.setValue("SurchargeAmt", dr["SurchargeAmt"]);
                        this.setCalloutActive(false);
                        return "";
                    }
                    else {
                        var Rate = VIS.dataContext.getJSONRecord("MTax/GetTaxRate", mTab.getValue("C_Tax_ID").toString());
                        if (Rate > 0) {
                            var TaxAmt = Util.getValueOfDecimal(Util.getValueOfDecimal(mTab.getValue("VA027_PayAmt")) - (Util.getValueOfDecimal(mTab.getValue("VA027_PayAmt")) / ((Rate / 100) + 1)));
                            mTab.setValue("TaxAmount", Util.getValueOfDecimal(TaxAmt.toFixed(2)));
                        }
                        else {
                            mTab.setValue("TaxAmount", 0);
                            this.setCalloutActive(false);
                            return "";
                        }
                    }
                }
                else {
                    this.setCalloutActive(false);
                    return "";
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString()); // SD
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };


    //--------Business Partner----------
    VA027_CalloutPostPayment.prototype.BusinessPartner = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        try {
            var dr = null;
            var paramStr = "";
            var _bPartner = 0;
            var colName = mField.getColumnName();
            if (mTab.getValue("C_Bpartner_ID") == null) {
                //if (colName == "C_Invoice_ID") {
                //    _bPartner = Util.getValueOfInt(VIS.DB.executeScalar("Select C_Bpartner_ID From C_Invoice Where C_Invoice_ID=" + value));
                //    paramStr = "C_Invoice," + value.toString();
                //}
                //else if (colName == "C_Order_ID") {
                //    _bPartner = Util.getValueOfInt(VIS.DB.executeScalar("Select C_Bpartner_ID From C_Invoice Where C_Invoice_ID=" + value));
                //    paramStr = "C_Order," + value.toString();
                //}

                paramStr = "C_Invoice," + value.toString();
                dr = VIS.dataContext.getJSONRecord("PDC/GetBPData", paramStr);
                if (dr != null) {
                    mTab.setValue("C_Bpartner_ID", dr["C_BPartner_ID"]);
                    mTab.setValue("C_Bpartner_Location_ID", dr["C_BPartner_Location_ID"]);
                }
            }

            if (colName.equals("C_BPartner_ID")) {
                paramStr = "C_BPartner," + value.toString();
                dr = VIS.dataContext.getJSONRecord("PDC/GetBPData", paramStr);
                if (dr != null) {
                    mTab.setValue("C_Bpartner_Location_ID", dr["C_BPartner_Location_ID"]);
                }
            }
        }
        catch (err) {
            this.log.severe(err.toString());
        };
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldvalue = null;
        return "";

    };


    VA027_CalloutPostPayment.prototype.PayAmtNull = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        try {
            if (mTab.getValue("PDCType") == "D") {
                mTab.setValue("VA027_PayAmt", 0);
                mTab.setValue("C_Bpartner_ID", null);
                mTab.setValue("C_Bpartner_Location_ID", null);
                mTab.setValue("C_Invoice_ID", null);
                mTab.setValue("C_Order_ID", null);
            }
        }
        catch (err) {
            this.log.severe(err.toString());
        };
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldvalue = null;
        return "";

    };

    //-----To Set Converted Amount On Post Dated Cheque Window::Arpit Rai Dated Sunday,28-Aug-2016
    VA027_CalloutPostPayment.prototype.SetConvAmt = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        //	Changed Column
        var colName = mField.getColumnName();

        var C_Invoice_ID = mTab.getValue("C_Invoice_ID");
        var C_Order_ID = mTab.getValue("C_Order_ID");
        var C_ConversionType_ID = 0;
        var openAmt = VIS.Env.ZERO;
        var paramstring = "";

        if (C_Invoice_ID == null && C_Order_ID == null) {
            this.setCalloutActive(false);
            ctx = windowNo = mTab = mField = value = oldvalue = null;
            return "";
        }
        else {
            var CurrencyFrom = mTab.getValue("C_Currency_ID");
            var ds = null;
            //var sql = null;            
            var CurrencyTo = null;

            try {
                if (Util.getValueOfInt(C_Invoice_ID) != 0) {
                    //sql = "Select C_Currency_ID,C_ConversionType_ID from C_Invoice Where C_Invoice_ID = " + C_Invoice_ID;
                    //ds = VIS.DB.executeDataSet(sql.toString());
                    //if (ds != null && ds.getTables().length > 0) {
                    //    CurrencyTo = Util.getValueOfInt(ds.getTables()[0].getRows()[0].getCell("c_currency_id"));
                    //    ConversionType_ID = Util.getValueOfInt(ds.getTables()[0].getRows()[0].getCell("c_conversionType_id"));
                    //}
                    //if (ConversionType_ID == "" || ConversionType_ID == 0) {
                    //    ConversionType_ID = Util.getValueOfInt(VIS.DB.executeScalar("Select c_conversionType_id from C_conversiontype where Isdefault='Y'"));
                    //}
                    //sql = "";
                    //if (CurrencyTo == CurrencyFrom) {
                    //    mTab.setValue("VA027_ConvertedAmount", mTab.getValue("VA027_PayAmt"));
                    //}
                    //else {
                    //    sql = "SELECT currencyconvert(" + mTab.getValue("VA027_PayAmt") + "," + CurrencyTo + "," + CurrencyFrom + "," + VIS.DB.to_date(mTab.getValue("DateAcct"), true) + "," + ConversionType_ID + "," + mTab.getValue("AD_Client_ID") + "," + mTab.getValue("AD_Org_ID") + ") AS value from dual ";
                    //    mTab.setValue("VA027_ConvertedAmount", Util.getValueOfDecimal(VIS.DB.executeScalar(sql.toString())));
                    //    mTab.setValue("VA027_PayAmt", Util.getValueOfDecimal(VIS.DB.executeScalar(sql.toString())));
                    //}
                    //sql = "";
                    //if (CurrencyTo == CurrencyFrom) {
                    //    mTab.setValue("CurrencyRate", 1);
                    //}
                    //else {
                    //    sql = "SELECT MultiplyRate FROM C_Conversion_Rate WHERE C_Currency_ID=" + CurrencyTo + " AND C_Currency_To_ID=" + CurrencyFrom + " AND C_ConversionType_ID=" + ConversionType_ID + " AND " + VIS.DB.to_date(mTab.getValue("DateAcct"), true) + " BETWEEN ValidFrom AND ValidTo AND AD_Client_ID IN (0," + mTab.getValue("AD_Client_ID") + ")" + " AND AD_Org_ID IN (0," + mTab.getValue("AD_Org_ID") + ") ORDER BY AD_Client_ID DESC, AD_Org_ID DESC, ValidFrom DESC";
                    //    mTab.setValue("CurrencyRate", Util.getValueOfDecimal(VIS.DB.executeScalar(sql.toString())));
                    //}

                    var C_InvoicePaySchedule_ID = 0;
                    if (ctx.getContextAsInt(windowNo, "C_Invoice_ID") == C_Invoice_ID
                        && mTab.getValue("C_InvoicePaySchedule_ID") != null) {
                        C_InvoicePaySchedule_ID = mTab.getValue("C_InvoicePaySchedule_ID");
                    }

                    var ts = mTab.getValue("VA027_TrxDate");
                    if (ts == null) {
                        ts = new Date();
                    }
                    openAmt = mTab.getValue("VA027_PayAmt");

                    //	Get Invoice Currency and Currency Type 
                    //VIS_427 changed the format of date parameter
                    paramString = C_Invoice_ID.toString() + "," + C_InvoicePaySchedule_ID.toString() + "," + Globalize.format(Util.getValueOfDate(ts), "yyyy-MM-dd").toString();
                    var dr = VIS.dataContext.getJSONRecord("MPayment/GetInvoiceData", paramString);
                    if (dr != null) {
                        CurrencyTo = Util.getValueOfInt(dr["C_Currency_ID"]);
                        C_ConversionType_ID = Util.getValueOfInt(dr["C_ConversionType_ID"]);
                    }
                }
                //   if (C_Order_ID != "") {
                else {
                    //sql = "Select C_Currency_ID,C_ConversionType_ID from C_Order Where C_Order_ID = " + C_Order_ID;
                    //ds = VIS.DB.executeDataSet(sql.toString());
                    //if (ds != null && ds.getTables().length > 0) {
                    //    CurrencyTo = Util.getValueOfInt(ds.getTables()[0].getRows()[0].getCell("c_currency_id"));
                    //    ConversionType_ID = Util.getValueOfInt(ds.getTables()[0].getRows()[0].getCell("c_conversionType_id"));
                    //}
                    //if (ConversionType_ID == "" || ConversionType_ID == 0) {
                    //    ConversionType_ID = Util.getValueOfInt(VIS.DB.executeScalar("Select c_conversionType_id from C_conversiontype where Isdefault='Y'"));
                    //}
                    //sql = "";
                    //if (CurrencyTo == CurrencyFrom) {
                    //    mTab.setValue("VA027_ConvertedAmount", mTab.getValue("VA027_PayAmt"));
                    //}
                    //else {
                    //    sql = "select currencyconvert(" + mTab.getValue("VA027_PayAmt") + "," + CurrencyTo + "," + CurrencyFrom + "," + VIS.DB.to_date(mTab.getValue("DateAcct"), true) + "," + ConversionType_ID + "," + mTab.getValue("AD_Client_ID") + "," + mTab.getValue("AD_Org_ID") + ") as value from dual ";
                    //    mTab.setValue("VA027_ConvertedAmount", Util.getValueOfDecimal(VIS.DB.executeScalar(sql.toString())));
                    //}
                    //sql = "";
                    //if (CurrencyTo == CurrencyFrom) {
                    //    mTab.setValue("CurrencyRate", 1);
                    //}
                    //else {
                    //    sql = "SELECT MultiplyRate FROM C_Conversion_Rate WHERE C_Currency_ID=" + CurrencyTo + " AND C_Currency_To_ID=" + CurrencyFrom + " AND C_ConversionType_ID=" + ConversionType_ID + " AND " + VIS.DB.to_date(mTab.getValue("DateAcct"), true) + " BETWEEN ValidFrom AND ValidTo AND AD_Client_ID IN (0," + mTab.getValue("AD_Client_ID") + ")" + " AND AD_Org_ID IN (0," + mTab.getValue("AD_Org_ID") + ") ORDER BY AD_Client_ID DESC, AD_Org_ID DESC, ValidFrom DESC";
                    //    mTab.setValue("CurrencyRate", Util.getValueOfDecimal(VIS.DB.executeScalar(sql.toString())));
                    //}

                    var VA009_OrderPaySchedule_ID = ctx.getContextAsInt(windowNo, "VA009_OrderPaySchedule_ID");
                    openAmt = mTab.getValue("VA027_PayAmt");

                    //	Get Order Currency and Currency Type                   
                    paramstring = C_Order_ID.toString() + "," + mTab.getValue("VA027_TrxDate").toString() + "," + VA009_OrderPaySchedule_ID.toString();
                    var dr = VIS.dataContext.getJSONRecord("PDC/GetOrderData", paramstring);
                    if (dr != null) {
                        CurrencyTo = dr["C_Currency_ID"];
                        C_ConversionType_ID = dr["C_ConversionType_ID"];
                    }
                }

                var C_Currency_ID = Util.getValueOfInt(CurrencyFrom);
                paramString = C_Currency_ID.toString();
                var currency = VIS.dataContext.getJSONRecord("MCurrency/GetCurrency", paramString);
                var precision = currency["StdPrecision"];
                var ConvDate = mTab.getValue("VA027_TrxDate");

                var AD_Client_ID = ctx.getContextAsInt(windowNo, "AD_Client_ID");
                var AD_Org_ID = ctx.getContextAsInt(windowNo, "AD_Org_ID");
                var currencyRate = VIS.Env.ONE;
                
                if (CurrencyTo == CurrencyFrom) {
                    mTab.setValue("CurrencyRate", currencyRate);
                    //here set openAmt in converted amount field because the converted amount field remains unchanged whether user change writeoff or discountAmt
                    mTab.setValue("VA027_ConvertedAmount", mTab.getValue("VA027_PayAmt"));
                }
                else {
                    if ((C_Currency_ID > 0 && CurrencyTo > 0 && C_Currency_ID != CurrencyTo) || colName == "C_Currency_ID") {
                        this.log.fine("Currency To=" + CurrencyTo
                            + ", Currency From=" + C_Currency_ID
                            + ", Date=" + ConvDate + ", Type=" + C_ConversionType_ID);


                        paramstring = CurrencyTo + "," + C_Currency_ID + "," + ConvDate + "," + C_ConversionType_ID + "," + AD_Client_ID + "," + AD_Org_ID;
                        currencyRate = VIS.dataContext.getJSONRecord("MConversionRate/GetRate", paramstring);

                        if (currencyRate == null || currencyRate.toString() == 0) {
                            if (CurrencyTo == 0) {
                                return "";		//	no error message when no Invoice/Order is selected
                            }
                            this.setCalloutActive(false);
                            mTab.setValue("C_Currency_ID", CurrencyTo);
                            return "NoCurrencyConversion";
                        }
                        //
                        mTab.setValue("CurrencyRate", currencyRate);
                        openAmt = Util.getValueOfDecimal((openAmt * currencyRate).toFixed(precision));
                        mTab.setValue("VA027_ConvertedAmount", openAmt);
                    }
                }
            }

            catch (err) {
                this.log.severe(err.toString());
            }
            C_Invoice_ID = C_Order_ID = null;
        };
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldvalue = null;
        return "";

    };
    //End HERE To Set Converted Amount

    //PDC-Header-- Set Currency on basis of BankAccount
    VA027_CalloutPostPayment.prototype.BankAccount = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        var c_bankaccount_ID = value;
        try {
            var bankAccountDetails = VIS.dataContext.getJSONRecord("PDC/GetBankAcctCurrency", c_bankaccount_ID.toString());
            if (bankAccountDetails != null) {
                //VIS_427 Set the bank account details
                mTab.setValue("C_Currency_ID", bankAccountDetails["C_Currency_ID"]);
                mTab.setValue("VA027_AccountNo", bankAccountDetails["AccountNo"]);
                mTab.setValue("VA027_AccountName", bankAccountDetails["Name"]);
            }
        }
        catch (err) {
            this.log.severe(err.toString());
            this.setCalloutActive(false);
            return err.message;
        };
        this.setCalloutActive(false);
        ctx = mTab = mField = value = oldValue = null;
        return "";
    };

    VA027.Model = VA027.Model || {};
    VA027.Model.VA027_CalloutPostPayment = VA027_CalloutPostPayment;
})(VA027, jQuery);
