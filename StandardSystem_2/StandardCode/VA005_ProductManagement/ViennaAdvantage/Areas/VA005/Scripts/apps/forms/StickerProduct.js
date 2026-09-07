; VA005 = window.VA005 || {};

; (function (VA005, $) {
    ; VA005.StickerProduct = function () { };
    ; VA005.StickerProduct.prototype.create = function (StickerHeader, PriceListVersion, Records) {
        debugger;
        var $MainFormDiv = $('<div style="width: 100%;" >');
        $MainFormDiv = $('<div id="VA005_Barcode" style="width:112px"></div>');
        $MainFormDiv.append($('</div>'));

        function GenerateBarcode() {
            debugger;
            var btypeEan13 = "ean13";
            var btypeEan8 = "ean8";
            var btypeC39 = "code39";
            var btypeC128 = "code128"
            var renderer = "css";
            var settings = {
                output: renderer,
                bgColor: "#FFFFFF",
                color: "#000000",
                barWidth: 1,
                barHeight: 30,
                moduleSize: 2,
                // posX: $("#posX").val(),
                // posY: $("#posY").val(),
                addQuietZone: 1
            };
            var id = 0;
            var product_ID = 0, uom = 0, attribute = 0, qty = 0, priceList = "";
            var prodName = "", curCode = "", upc = "";
            var error = "";
            //var DS = VIS.DB.executeDataSet("select dtd001_stickerheader, dtd001_price, dtd001_qty, value, upc from dtd001_productstickerline where dtd001_productsticker_id=" + Record_ID);
            var DivBarcode = null;
            var $report = $("#VA005_Barcode");
            //var sql = "SELECT cur.iso_code FROM M_PriceList_Version plv INNER JOIN M_PriceList pl ON plv.M_PriceList_ID= pl.M_PriceList_ID INNER JOIN C_Currency cur ON pl.C_Currency_ID = cur.C_Currency_ID" +
            //    " WHERE plv.M_PriceList_Version_ID=" + PriceListVersion;
            // curCode = VIS.Utility.Util.getValueOfString(VIS.DB.executeScalar(sql));

            var dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadPriceListData", { "M_PriceList_ID": PriceListVersion });
            if (dr != null) {
                curCode = dr["ISO_Code"];
            }

            for (item in Records) {
                product_ID = VIS.Utility.Util.getValueOfInt(Records[item].product_ID);
                prodName = VIS.Utility.Util.getValueOfString(Records[item].product);
                upc = VIS.Utility.Util.getValueOfString(Records[item].UPC);
                uom = VIS.Utility.Util.getValueOfInt(Records[item].C_Uom_ID);
                attribute = VIS.Utility.Util.getValueOfInt(Records[item].attribute_ID);
                qty = VIS.Utility.Util.getValueOfInt(Records[item].Qty);
                if (CheckSpecialCharacter(upc) == true || upc.indexOf(':') != -1 || upc.indexOf(';') != -1 || upc.indexOf('!') != -1 || upc.indexOf('~') != -1) {
                    continue;
                }
                //priceList = VIS.Utility.Util.getValueOfString(VIS.DB.executeScalar(" SELECT  pr.PriceList FROM M_ProductPrice pr INNER JOIN M_PriceList_Version plv" +
                //    " ON pr.M_PriceList_Version_ID = plv.M_PriceList_Version_ID WHERE pr.M_PriceList_Version_ID=" + PriceListVersion + " AND pr.M_Product_ID = " + product_ID +
                //    " AND pr.C_UOM_ID = " + uom + " AND pr.M_AttributeSetInstance_ID = " + attribute));

                priceList = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/GetListPrice",
                    { "PriceListVersion_ID": PriceListVersion, "Product_ID": product_ID, "UOM": uom, "Attribute": attribute });

                for (var j = 0; j < qty; j++) {
                    id = id + 1;
                    if (id == 1) {
                        if (upc.length == 13) {
                            DivBarcode = '<div style="text-align: center;width:100%;margin-left: -13px;margin-top: 0px  " >'
                        }
                        else {
                            DivBarcode = '<div style="text-align: center;width:100%;margin-left: -4px;margin-top: 0px  " >'
                        }
                    }
                    else {
                        if (upc.length == 13) {
                            DivBarcode = '<div style="text-align: center;width:100%;margin-top: 20px;margin-left: -13px; " >'
                        }
                        else {
                            DivBarcode = '<div style="text-align: center;width:100%;margin-top: 20px;margin-left: -4px; " >'
                        }
                    }
                    if (StickerHeader != null) {
                        DivBarcode += '<div id=VA005_Header' + id + ' style="font-size: small;width:100%;">' + StickerHeader + '</div>'
                    }
                    else {
                        DivBarcode += '<div id=VA005_Header' + id + ' style="font-size: small;width:100%;"> &nbsp; </div>'
                    }
                    DivBarcode += '<div id=VA005_Bars' + id + ' style="margin: 0 auto;"></div>'
                    DivBarcode += '<div id=VA005_Price' + id + ' style="font-size: small;width:100%">' + priceList + ' ' + curCode + '</div>'
                    DivBarcode += '</div>';
                    $report.append(DivBarcode);
                    DivBarcode = null;


                    if (upc.length == 12) {
                        $("#VA005_Bars" + id).html("").show().barcode(upc, btypeEan13, settings);
                    }
                    else if (upc.length == 7) {
                        $("#VA005_Bars" + id).html("").show().barcode(upc, btypeEan8, settings);
                    }
                    else if (upc.length == 4) {
                        $("#VA005_Bars" + id).html("").show().barcode(upc, btypeC39, settings);
                    }
                    else {
                        $("#VA005_Bars" + id).html("").show().barcode(upc, btypeC128, settings);
                    }
                }
            }
            var mywindow = window.open();
            mywindow.document.write('<html><head>');
            mywindow.document.write('</head><body >');

            mywindow.document.write($report.html());
            mywindow.document.write('</body></html>');
            mywindow.print();
            mywindow.close();
            $report.empty();
        };

        function CheckSpecialCharacter(value) {
            return (/[@#$%^&*()_\\"|?/`,.<>{}+=]/.test(value));
        }

        var StickerForm = new VIS.ChildDialog(); //create object of child dialog
        StickerForm.setContent($MainFormDiv);
        debugger;
        StickerForm.setTitle(VIS.Msg.getMsg("VA005_GenerateSticker"));
        //materialIssue.setHeight(400);
        StickerForm.setWidth("40%");
        //StickerForm.setHeight("100%");
        StickerForm.setEnableResize(false);
        StickerForm.setModal(true);
        StickerForm.show();
        StickerForm.onOkClick = function (e) {
            debugger;
            GenerateBarcode();
        };
    };
})(VA005, jQuery);