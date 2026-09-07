
; VA005 = window.VA005 || {};
//java script function closer
; (function (VA005, $) {

    //Form Class function fullnamespace
    VA005.ProductCategoryForm = function () {
        this.frame;
        this.windowNo;
        var $self = this; //scoped self pointer
        var $root = $("<div class='vis-forms-container'>");
        var mainProductCategoryUl = null;
        var btnAddCategory = null;
        var $BusyIndicator = null;
        var divLeft = null;
        var divRight = null;
        var name = "";
        var pcat_ID = 0;
        var pcatImg = 0;
        var pcatIdOld = 0;
        var pcatOld = "";
        var imageUrl = null;
        var binaryData = null;
        var count = 0;
        var catName = "";
        var currentElement = null;
        var btnSave = null;
        var btnClose = null;
        var btnDelete = null;
        var btnMultiple = null;
        var btnAttributeSet = null;
        var btnTaxCategory = null;
        var btnAssetGroup = null;
        var btnUndo = null;
        var btnZoom = null;
        var txtName = null;
        var txtValue = null;
        var txtDesc = null;
        var cmbProductType = null;
        var cmbmatPolicy = null;
        var cmbAttributeSet = null;
        var cmbTaxCategory = null;
        var cmbAssetGroup = null;
        var IsDefault = null;
        var IsSelfService = null;
        var IsConsumable = null;
        var lblConsumable = null;
        var fileUpload = null;
        //var holderDiv = null;
        var imgUsrImage = null;
        var lblfileUpload = null;
        var chkDataBaseSave = null;
        var chkMultiple = null;
        var change = false;
        var ad_image_id = 0;
        var pageNo = 0;
        var PAGESIZE = 50;
        var pcats = [];
        var searchKey = "";
        var attrSetId = 0;
        var pType = "";
        var Description = "";
        var matPol = "";
        var taxCatID = 0;
        var asetGrp = 0;
        var IsCon = "";
        //var VA005_divTaxCat = null;
        var divzoomWindow = null;
        var options = {
            Attribute: "N",
            Tax: "N",
            Asset: "N"
        }
        /*
       Load parial vied by ajax request
       */
        function load() {
            busyIndicator();
            $root.css("width", "100%");
            $root.css("height", "100%");
            $root.css("position", "relative");
            $root.load(VIS.Application.contextUrl + "VA005/ProductCategory/Index?windowno=" + $self.windowNo, function () {
                loadRoot();
            });
        };

        function loadRoot() {

            $root.find(".VA005-report-tittle").text(VIS.Msg.getElement(VIS.Env.getCtx(), "M_Product_Category_ID"));
            $root.find(".VA005-form-top-fields h4").text(VIS.Msg.getElement(VIS.Env.getCtx(), "M_Product_Category_ID"));
            $root.find("#VA005_Value_" + $self.windowNo).text(VIS.Msg.getElement(VIS.Env.getCtx(), "Value"));
            $root.find("#VA005_ProductType_" + $self.windowNo).text(VIS.Msg.getElement(VIS.Env.getCtx(), "ProductType")).append("<sup class='.VA005-sup'>*</sup>");;
            $root.find("#VA005_Attribute_" + $self.windowNo).text(VIS.Msg.getElement(VIS.Env.getCtx(), "M_AttributeSet_ID"));
            $root.find("#VA005_MatPol_" + $self.windowNo).text(VIS.Msg.getElement(VIS.Env.getCtx(), "MMPolicy"));
            $root.find("#VA005_TaxCat_" + $self.windowNo).text(VIS.Msg.getElement(VIS.Env.getCtx(), "C_TaxCategory_ID"));
            $root.find("#VA005_AstGrp_" + $self.windowNo).text(VIS.Msg.getElement(VIS.Env.getCtx(), "A_Asset_Group_ID"));
            $root.find(".VA005-effect-on").text(VIS.Msg.getMsg("VA005_multiOn"));
            $root.find(".VA005-effect-off").text(VIS.Msg.getMsg("VA005_multiOff"));
            $root.find("#btnZoomWindow_" + $self.windowNo).attr('title', VIS.Msg.getMsg("VA005_Zoom"));
            $root.find("#btnRefresh_" + $self.windowNo).attr('title', VIS.Msg.getMsg("VA005_Refresh"));
            divzoomWindow = $root.find("#VA005_zoomWindow_" + $self.windowNo);
            //$root.find("#VA005_Zoom_" + $self.windowNo).text(VIS.Msg.getMsg("VA005_Zoom"));
            //$root.find("#VA005_Refresh_" + $self.windowNo).text(VIS.Msg.getMsg("VA005_Refresh"));
            //var lookup = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), $self.windowNo, 2066, VIS.DisplayType.TableDir);
            //cmbTaxCategory = new VIS.Controls.VComboBox("C_TaxCategory_ID", false, false, true, lookup, 150, VIS.DisplayType.TableDir, 0);
            //VA005_divTaxCat.append(cmbTaxCategory.getControl()).append(cmbTaxCategory.getBtn(0));





            divLeft = $root.find("#divLeft_" + $self.windowNo);
            divRight = $root.find("#divRight_" + $self.windowNo);
            mainProductCategoryUl = $root.find("#VA005_ProductCategory_" + $self.windowNo);
            btnAddCategory = $root.find("#VA005_AddCategory_" + $self.windowNo);
            btnSave = $root.find("#btnSave_" + $self.windowNo);
            btnClose = $root.find("#btnClose_" + $self.windowNo);
            btnDelete = $root.find("#btnDelete_" + $self.windowNo);
            btnMultiple = $root.find("#btnMultiple_" + $self.windowNo);
            btnUndo = $root.find("#btnUndo_" + $self.windowNo);
            btnZoom = $root.find("#btnZoom_" + $self.windowNo);
            btnAttributeSet = $root.find("#btnAttributeSet_" + $self.windowNo);
            btnTaxCategory = $root.find("#btnTaxCategory_" + $self.windowNo);
            btnAssetGroup = $root.find("#btnAssetGroup_" + $self.windowNo);
            txtName = $root.find("#txtName_" + $self.windowNo);
            txtValue = $root.find("#txtValue_" + $self.windowNo);
            txtDesc = $root.find("#txtDesc_" + $self.windowNo);
            cmbProductType = $root.find("#cmbProductType_" + $self.windowNo);
            cmbmatPolicy = $root.find("#cmbMatPolicy_" + $self.windowNo);
            cmbAttributeSet = $root.find("#cmbAttributeSet_" + $self.windowNo);
            cmbTaxCategory = $root.find("#cmbTaxCategory_" + $self.windowNo);
            cmbAssetGroup = $root.find("#cmbAssetGroup_" + $self.windowNo);
            //IsDefault = $root.find("#IsDefault_" + $self.windowNo);
            //IsSelfService = $root.find("#IsSelfService_" + $self.windowNo);
            IsConsumable = $root.find("#IsConsumable_" + $self.windowNo);
            lblConsumable = $root.find("#lblConsumable_" + $self.windowNo);
            lblConsumable.text(VIS.Msg.getElement(VIS.Env.getCtx(), "DTD001_IsConsumable"));
            fileUpload = $root.find("#fileUpload_" + $self.windowNo);
            imgUsrImage = $root.find("#imgUsrImage_" + $self.windowNo);
            lblfileUpload = $root.find("#lblfileUpload_" + $self.windowNo);
            //chkDataBaseSave = $root.find("#chkDataBaseSave_" + $self.windowNo);
            chkMultiple = $root.find("#chkMultiple_" + $self.windowNo);
            btnMultiple.hide();
            pageNo++;            
            LoadCategory(pageNo, PAGESIZE);
            LoadAttributes();
            LoadTaxCategories();
            LoadAssetGroup();
            LoadProductTypes();
            LoadMatPolicies();
            if (cmbProductType.val() != "I") {
                IsConsumable.css({ "display": "none" });
                lblConsumable.css({ "display": "none" });
            }
            mainProductCategoryUl.find('li:eq(1) .VA005-cat-caption').addClass('VA005-highlighted');
            fillCategory(mainProductCategoryUl.find('li:eq(1)').attr('procatid'));
            btnUndo.attr('disabled', 'disabled').css("opacity", 0.6);
            btnSave.attr('disabled', 'disabled').css("opacity", 0.6);
            btnDelete.attr('disabled', 'disabled').css("opacity", 0.6);
            btnZoom.attr('title', VIS.Msg.getMsg("VA005_Zoom"));
            Events();
            //ClearData();
            $BusyIndicator[0].style.visibility = "hidden";
        };
        var LoadProductTypes = function () {
            cmbProductType.append('<option value="E">' + VIS.Msg.getMsg("VA005_Expense") + '</option><option selected="selected" value="I">' + VIS.Msg.getMsg("VA005_Item") + '</option>' +
                '<option value="R">' + VIS.Msg.getMsg("VA005_Resource") + '</option><option value="S">' + VIS.Msg.getMsg("VA005_Service") + '</option>');
        }

        var LoadMatPolicies = function () {
            cmbmatPolicy.append('<option value="F">' + VIS.Msg.getMsg("VA005_Fifo") + '</option><option selected="selected" value="L">' + VIS.Msg.getMsg("VA005_Lifo") + '</option>');
        }

        var LoadAttributes = function () {
            cmbAttributeSet.empty();
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductCategory/LoadAttribut", "", LoadAttrCallBack);
            //var qry = "SELECT M_AttributeSet_ID,Name FROM M_AttributeSet WHERE IsActive = 'Y' AND AD_Client_ID = " + VIS.context.getAD_Client_ID();
            // VIS.DB.executeReader(qry.toString(), null, LoadAttrCallBack);
        };
        function LoadAttrCallBack(dr) {
            cmbAttributeSet.append(" <option value = 0></option>");
            if (dr.length > 0) {
                for (var i = 0; i < dr.length; i++) {
                    key = VIS.Utility.Util.getValueOfInt(dr[i].M_AttributeSet_ID);
                    value = VIS.Utility.encodeText(dr[i].Name);
                    cmbAttributeSet.append(" <option value=" + key + ">" + value + "</option>");
                }
            }
        };
        var LoadTaxCategories = function () {
            cmbTaxCategory.empty();
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductCategory/LoadTaxCategory", "", TaxCatCallBack);
            //var qry = "SELECT C_TaxCategory_ID,Name FROM C_TaxCategory WHERE IsActive = 'Y' AND AD_Client_ID = " + VIS.context.getAD_Client_ID();
            //VIS.DB.executeReader(qry.toString(), null, TaxCatCallBack);
        };

        function TaxCatCallBack(dr) {
            cmbTaxCategory.append(" <option value = 0></option>");
            if (dr.length > 0) {
                for (var i = 0; i < dr.length; i++) {
                    key = VIS.Utility.Util.getValueOfInt(dr[i].C_TaxCategory_ID);
                    value = VIS.Utility.encodeText(dr[i].Name);
                    cmbTaxCategory.append(" <option value=" + key + ">" + value + "</option>");
                }
            }
        };

        var LoadAssetGroup = function () {
            cmbAssetGroup.empty();
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductCategory/LoadAssetGroup", "", AssetCallBack);
            //var qry = "SELECT A_Asset_Group_ID,Name FROM A_Asset_Group WHERE IsActive = 'Y' AND AD_Client_ID = " + VIS.context.getAD_Client_ID();
            //VIS.DB.executeReader(qry.toString(), null, AssetCallBack);
        };

        function AssetCallBack(dr) {
            cmbAssetGroup.append(" <option value = 0></option>");
            if (dr.length > 0) {
                for (var i = 0; i < dr.length; i++) {
                    key = VIS.Utility.Util.getValueOfInt(dr[i].A_Asset_Group_ID);
                    value = dr[i].Name;
                    cmbAssetGroup.append(" <option value=" + key + ">" + value + "</option>");
                }
            }
        };

        var Events = function () {
            function addEffect(btn, functionName) {
                var options = { to: mainProductCategoryUl.find('li:eq(1)'), className: "wsp-ui-effects-transfer" };
                btn.effect("transfer", options, 600, functionName);
            };

            cmbProductType.change(function () {
                if (cmbProductType.val() == "I" ) {
                    IsConsumable.css({ "display": "block" });
                    lblConsumable.css({ "display": "block" });
                }
                else {
                    IsConsumable.css({ "display": "none" });
                    lblConsumable.css({ "display": "none" });
                    IsConsumable.prop("checked", false);
                }
                btnUndo.removeAttr('disabled').css("opacity", 1);
                btnSave.removeAttr('disabled').css("opacity", 1);
            });

            if (fileUpload != null) {
                fileUpload.on("change", function () {
                    imgUsrImage.show();
                    var file = fileUpload[0].files[0],
                        reader = new FileReader();
                    reader.onload = function (event) {
                        imgUsrImage.removeAttr("src").attr("src", event.target.result);

                        // if (imgUsrImage[0].width > 300) { // holder width
                        //holderDiv.css("overflow", "auto");
                        //
                        // }
                    };
                    reader.readAsDataURL(file);
                    change = true;
                    btnUndo.removeAttr('disabled').css("opacity", 1);
                    btnSave.removeAttr('disabled').css("opacity", 1);
                    reader = null;
                    file = null;
                    return false;
                });
            }
            txtName.change(function () {
                btnUndo.removeAttr('disabled').css("opacity", 1);
                btnSave.removeAttr('disabled').css("opacity", 1);
            });
            txtDesc.change(function () {
                btnUndo.removeAttr('disabled').css("opacity", 1);
                btnSave.removeAttr('disabled').css("opacity", 1);
            });
            txtValue.change(function () {
                btnUndo.removeAttr('disabled').css("opacity", 1);
                btnSave.removeAttr('disabled').css("opacity", 1);
            });
            txtName.on("keydown", function () {
                btnUndo.removeAttr('disabled').css("opacity", 1);
                btnSave.removeAttr('disabled').css("opacity", 1);
            });
            txtDesc.on("keydown", function () {
                btnUndo.removeAttr('disabled').css("opacity", 1);
                btnSave.removeAttr('disabled').css("opacity", 1);
            });
            txtValue.on("keydown", function () {
                btnUndo.removeAttr('disabled').css("opacity", 1);
                btnSave.removeAttr('disabled').css("opacity", 1);
            });
            cmbAttributeSet.change(function () {
                btnUndo.removeAttr('disabled').css("opacity", 1);
                btnSave.removeAttr('disabled').css("opacity", 1);
            });
            cmbAssetGroup.change(function () {
                btnUndo.removeAttr('disabled').css("opacity", 1);
                btnSave.removeAttr('disabled').css("opacity", 1);
            });
            cmbmatPolicy.change(function () {
                btnUndo.removeAttr('disabled').css("opacity", 1);
                btnSave.removeAttr('disabled').css("opacity", 1);
            });
            cmbTaxCategory.change(function () {
                btnUndo.removeAttr('disabled').css("opacity", 1);
                btnSave.removeAttr('disabled').css("opacity", 1);
            });
            IsConsumable.change(function () {
                btnUndo.removeAttr('disabled').css("opacity", 1);
                btnSave.removeAttr('disabled').css("opacity", 1);
            });

            if (mainProductCategoryUl != null) {
                mainProductCategoryUl.on("click", "li", function (e) {
                    currentElement = $(this);
                    if (currentElement.is(":first-child")) {
                        addCategory();
                        addEffect(currentElement);
                    }
                    else {
                        var target = $(e.target);
                        pcat_ID = currentElement.find("input[type=text]").attr("procatID");
                        pcatImg = currentElement.find("p").attr("procatID");
                        pcatOld = currentElement.find("p[procatid='" + pcatImg + "']").text();
                        // Done by Shifali to change the multiple selection category (183 point)
                        if (target.hasClass("VA005-catboxcheck")) {
                            if (target.prop("checked")) {
                                window.setTimeout(function () {
                                    mainProductCategoryUl.find("li[procatid='" + pcatImg + "'] .VA005-cat-caption").addClass('VA005-catboxchecked VA005-highlighted');
                                }, 200);
                                pcats.push(pcatImg);
                                if (pcats.length == 1) {
                                    btnDelete.removeAttr('disabled').css("opacity", 1);
                                }
                            }
                            else {
                                window.setTimeout(function () {
                                    mainProductCategoryUl.find("li[procatid='" + pcatImg + "'] .VA005-cat-caption").removeClass('VA005-catboxchecked VA005-highlighted');
                                }, 200);
                                pcats.splice(pcats.indexOf(pcatImg), 1);
                                if (pcats.length == 0) {
                                    btnDelete.attr('disabled', 'disabled').css("opacity", 0.6);
                                }
                            }
                        }

                        else if (!chkMultiple.prop("checked")) {
                            if (pcatIdOld != pcat_ID) {
                                pcatIdOld = pcat_ID;
                                fillCategory(pcat_ID)
                            }
                            if (mainProductCategoryUl.find("li[procatid='" + pcatImg + "'] .VA005-cat-caption").hasClass('VA005-highlighted')) {
                                ShowOrHide(true, currentElement);
                            }
                            else {
                                mainProductCategoryUl.find("li .VA005-cat-caption").removeClass('VA005-highlighted VA005-catboxchecked');
                                mainProductCategoryUl.find(".VA005-catboxcheck").prop("checked", false);
                                pcats = [];
                                // mainProductCategoryUl.find("li[procatid='" + pcatImg + "'] .VA005-cat-caption").addClass('VA005-highlighted');
                                target.addClass('VA005-highlighted');
                                target.find(".VA005-catboxcheck").prop("checked", true);
                                pcats.push(pcatImg);
                                btnDelete.removeAttr('disabled').css("opacity", 1);
                            }
                            //pcats = [];
                            //if (VIS.Utility.Util.getValueOfInt(pcatImg) > 0) {
                            //    pcats.push(pcatImg);
                            //}
                        }
                        else {
                            if (target.length > 0) {
                                mainProductCategoryUl.find(".VA005-catboxcheck").prop("checked", false);
                                if (target.hasClass('VA005-highlighted')) {
                                    //if (mainProductCategoryUl.find("li[procatid='" + pcatImg + "'] .VA005-cat-caption").hasClass('VA005-highlighted')) {
                                    mainProductCategoryUl.find("li[procatid='" + pcatImg + "'] .VA005-cat-caption").removeClass('VA005-highlighted');
                                    //target.find(".VA005-catboxcheck").prop("checked", false);
                                    pcats.splice(pcats.indexOf(pcatImg), 1);
                                }
                                else {
                                    //mainProductCategoryUl.find("li[procatid='" + pcatImg + "'] .VA005-cat-caption").addClass('VA005-highlighted');
                                    target.addClass('VA005-highlighted');
                                    target.find(".VA005-catboxcheck").prop("checked", true);
                                    pcats.push(pcatImg);
                                }
                            }
                            else {
                                return;
                            }
                        }
                    }
                });

                mainProductCategoryUl.on("keydown", "input[type=text]", function (event) {

                    if (event.keyCode == 13 || event.keyCode == 9) {        //will work on press of tab key or enter key                        
                        if (currentElement != null) {
                            ShowOrHide(false, currentElement);
                            name = event.target.value;
                            pcat_ID = $(event.target).attr("procatID");
                            if (pcatOld != name) {
                                updateCategory(pcat_ID, name, currentElement);
                                fillCategory(pcat_ID);
                            }
                        }
                        currentElement = null;
                    }
                });

                mainProductCategoryUl.on("focusout", "input[type=text]", function () {

                    if (currentElement != null) {
                        ShowOrHide(false, currentElement);
                        name = event.target.value;
                        pcat_ID = $(event.target).attr("procatID");
                        if (pcatOld != name) {
                            updateCategory(pcat_ID, name, currentElement);
                            fillCategory(pcat_ID);
                        }
                    }
                    currentElement = null;
                });
                // Done change by Shifali on 16th July 2020 to LoadCategory while scrolling
                divLeft.on("scroll", function () {
                    if ($(this).scrollTop() + $(this).innerHeight() >= this.scrollHeight) {
                        pageNo++;
                        LoadCategory(pageNo, PAGESIZE);
                    }
                });
            }

            if (btnSave != null) {
                btnSave.on("click", function () {
                    if (VIS.Utility.Util.getValueOfString(txtName.val()) == "") {
                        VIS.ADialog.error("VA005_FieldMandatory");
                        return;
                    }

                    if (VIS.Utility.Util.getValueOfString(cmbProductType.val()) == "") {
                        VIS.ADialog.error("VA005_ProductType");
                        return;
                    }

                    $BusyIndicator[0].style.visibility = "visible";
                    if (change) {
                        var xhr = new XMLHttpRequest();
                        var fd = new FormData();
                        fd.append("file", fileUpload[0].files[0]);
                        fd.append("isDatabaseSave", false);
                        fd.append("ad_image_id", ad_image_id);
                        xhr.open("POST", VIS.Application.contextUrl + "ProductCategory/SaveImage", true);
                        xhr.send(fd);
                        xhr.addEventListener("load", function (event) {
                            var dd = event.target.response;
                            dd = JSON.parse(dd);
                            ad_image_id = dd.result;
                            $.ajax({
                                type: "POST",
                                url: VIS.Application.contextUrl + "ProductCategory/Update1",
                                dataType: "json",
                                data: {
                                    id: pcat_ID,
                                    Name: txtName.val(),
                                    Value: txtValue.val(),
                                    ProductType: cmbProductType.val(),
                                    MatPolicy: cmbmatPolicy.val(),
                                    Desc: txtDesc.val(),
                                    attrSet: VIS.Utility.Util.getValueOfInt(cmbAttributeSet.val()),
                                    taxcat: VIS.Utility.Util.getValueOfInt(cmbTaxCategory.val()),
                                    assetGrp: VIS.Utility.Util.getValueOfInt(cmbAssetGroup.val()),
                                    consumable: IsConsumable.prop('checked'),
                                    imageId: ad_image_id
                                },
                                success: function (data) {
                                    var returnValue = data.result;

                                    if (returnValue) {
                                        change = false;
                                        if (mainProductCategoryUl != null) {
                                            var element1 = mainProductCategoryUl.find("input[procatid='" + pcat_ID + "']");
                                            var element2 = mainProductCategoryUl.find("p[procatid='" + pcat_ID + "']");
                                            var imgCtrl = mainProductCategoryUl.find("img[procatid='" + pcat_ID + "']");
                                            element1.attr('value', txtName.val());
                                            element2.text(txtName.val());
                                            var imgUrl = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductCategory/GetimgUrl", { "ad_image_id": ad_image_id });
                                            //var sql = "SELECT ImageUrl FROM AD_Image WHERE AD_Image_ID = " + ad_image_id;
                                            //var imgUrl = VIS.Utility.Util.getValueOfString(VIS.DB.executeScalar(sql.toString()));
                                            if (imgUrl != "") {
                                                imgUrl = imgUrl.substring(imgUrl.lastIndexOf("/") + 1, imgUrl.length);
                                                var d = new Date();
                                                imgCtrl.removeAttr("src").attr("src", VIS.Application.contextUrl + "Images/Thumb140x120/" + imgUrl + "?" + d.getTime());
                                            }
                                            //ClearData();                                           
                                        }
                                        // Added by Shifali on 16th July 2020 to get the same data after undo (JID_1860)
                                        fillCategory(pcat_ID);
                                    }
                                    $BusyIndicator[0].style.visibility = "hidden";
                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    //
                                    console.log(textStatus);
                                    $BusyIndicator[0].style.visibility = "hidden";
                                    alert("RecordNotSaved");
                                    return;
                                }
                            });
                        }, true);
                    }
                    else {
                        $.ajax({
                            type: "POST",
                            url: VIS.Application.contextUrl + "ProductCategory/Update1",
                            dataType: "json",
                            contentType: "application/json; charset=utf-8",
                            data: JSON.stringify({
                                id: pcat_ID,
                                Name: txtName.val(),
                                Value: txtValue.val(),
                                ProductType: cmbProductType.val(),
                                MatPolicy: cmbmatPolicy.val(),
                                Desc: txtDesc.val(),
                                attrSet: VIS.Utility.Util.getValueOfInt(cmbAttributeSet.val()),
                                taxcat: VIS.Utility.Util.getValueOfInt(cmbTaxCategory.val()),
                                assetGrp: VIS.Utility.Util.getValueOfInt(cmbAssetGroup.val()),
                                consumable: IsConsumable.prop('checked'),
                                imageId: ad_image_id
                            }),
                            success: function (data) {
                                var returnValue = data.result;

                                if (returnValue) {
                                    if (mainProductCategoryUl != null) {
                                        var element1 = mainProductCategoryUl.find("input[procatid='" + pcat_ID + "']");
                                        var element2 = mainProductCategoryUl.find("p[procatid='" + pcat_ID + "']");
                                        element1.attr('value', txtName.val());
                                        element2.text(txtName.val());
                                        //ClearData();
                                    }
                                    fillCategory(pcat_ID);
                                }
                                $BusyIndicator[0].style.visibility = "hidden";
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                //
                                console.log(textStatus);
                                $BusyIndicator[0].style.visibility = "hidden";
                                alert("RecordNotSaved");
                                return;
                            }
                        });
                    }
                    btnUndo.attr('disabled', 'disabled').css("opacity", 0.6);
                    btnSave.attr('disabled', 'disabled').css("opacity", 0.6);
                });
            }

            if (btnDelete != null) {
                btnDelete.on("click", function () {
                    var NameList = [];
                    if (pcats.length > 0) {
                        // Added by Shifali on 16th July to correct error msg (JID_1860)
                        //if (VIS.ADialog.confirm("DeleteRecord?")) {
                        VIS.ADialog.confirm("VA005_DeleteIt", true, "", "Confirm", function (result) {
                            if (result == true) {
                                $BusyIndicator[0].style.visibility = "visible";
                                $.ajax({
                                    type: "POST",
                                    url: VIS.Application.contextUrl + "ProductCategory/DeleteCategory",
                                    dataType: "json",
                                    contentType: "application/json; charset=utf-8",
                                    data: JSON.stringify({
                                        pcats: pcats
                                    }),
                                    success: function (data) {
                                        if (data.result.length > 0) {
                                            for (var i = 0; i < data.result.length; i++) {
                                                if (data.result[i].Name != "") {
                                                    NameList.push(data.result[i].Name);
                                                }
                                                else {
                                                    var li = mainProductCategoryUl.find("li[procatid='" + data.result[i].Key + "']");
                                                    li.remove();
                                                    ClearData();
                                                }
                                            }
                                            if (NameList != "") {
                                                VIS.ADialog.error("VA005_ProductCategory", true, NameList.join(", "));
                                            }
                                        }
                                    },
                                    error: function (jqXHR, textStatus, errorThrown) {
                                        //
                                        console.log(textStatus);
                                        $BusyIndicator[0].style.visibility = "hidden";
                                        alert("RecordNotSaved");
                                        return;
                                    }
                                    //for (var item in pcats) {
                                    //    var sql = "DELETE FROM M_Product_Category WHERE M_Product_Category_ID = " + pcats[item];
                                    //    var no = VIS.DB.executeQuery(sql.toString(), null, null);
                                    //    if (no <= 0) {
                                    //        //VIS.ADialog.error("VA005_ProductCategory");
                                    //        var str = "SELECT Name FROM M_Product_Category WHERE M_Product_Category_ID = " + pcats[item];
                                    //        var name = VIS.DB.executeScalar(str, null, null);
                                    //        NameList.push(name);
                                    //        VIS.ADialog.error("VA005_ProductCategory", true, NameList.toString());
                                    //    }
                                    //    else {
                                    //        var li = mainProductCategoryUl.find("li[procatid='" + pcats[item] + "']");
                                    //        li.remove();
                                    //        ClearData();
                                    //    }
                                    //}
                                });
                                pcats = [];
                                btnDelete.attr('disabled', 'disabled').css("opacity", 0.6);
                                mainProductCategoryUl.find(".VA005-catboxcheck").prop("checked", false);
                                mainProductCategoryUl.find('li .VA005-cat-caption').removeClass('VA005-catboxchecked');
                                mainProductCategoryUl.find('li .VA005-cat-caption').removeClass('VA005-highlighted');
                                mainProductCategoryUl.find('li:eq(1) .VA005-cat-caption').addClass('VA005-highlighted');
                                fillCategory(mainProductCategoryUl.find('li:eq(1)').attr('procatid'));
                            }
                        });
                    }
                    //fillCategory(mainProductCategoryUl.find('li:eq(1)').attr('procatid'));
                    $BusyIndicator[0].style.visibility = "hidden";
                });
            }

            if (btnMultiple != null) {
                btnMultiple.on("click", function () {

                    if (btnMultiple.hasClass('VA005-btn-Multiple-on')) {
                        btnMultiple.removeClass('VA005-btn-Multiple-on');
                        btnMultiple.addClass('VA005-btn-Multiple');
                        pcats = [];
                        chkMultiple.prop("checked", false);
                        mainProductCategoryUl.find("li:eq(0)").show();
                        btnSave.show();
                        btnUndo.show();
                        //divLeft.animate({ 'width': '61%' }, 500);
                        divRight.animate({ 'width': "39%" }, 500);
                        divRight.css('padding', '15px 0 15px 15px');
                        mainProductCategoryUl.find('li .VA005-cat-caption').removeClass('VA005-highlighted');
                        $(".VA005-effect-off").fadeIn();
                        $(".VA005-effect-off").fadeOut(2000);
                        mainProductCategoryUl.find('li:eq(1) .VA005-cat-caption').addClass('VA005-highlighted');
                        fillCategory(mainProductCategoryUl.find('li:eq(1)').attr('procatid'));

                    }
                    else {
                        btnMultiple.removeClass('VA005-btn-Multiple');
                        btnMultiple.addClass('VA005-btn-Multiple-on');
                        chkMultiple.prop("checked", true);
                        ClearData();
                        pcatIdOld = 0;
                        mainProductCategoryUl.find("li:eq(0)").hide();
                        btnSave.hide();
                        btnUndo.hide();
                        //divLeft.animate({ 'width': '100%' }, 500);
                        divRight.animate({ 'width': "0%" }, 500);
                        divRight.css('padding', '0');
                        $(".VA005-effect-on").fadeIn();
                        $(".VA005-effect-on").fadeOut(2000);
                    }
                });
            }

            if (btnUndo != null) {
                btnUndo.on("click", function () {
                    // fillCategory(mainProductCategoryUl.find('li:eq(1)').attr('procatid'));
                    $BusyIndicator[0].style.visibility = "visible";
                    txtName.val(catName);
                    txtValue.val(searchKey);
                    txtDesc.val(Description);
                    cmbAttributeSet.val(attrSetId);
                    cmbProductType.val(pType);
                    cmbmatPolicy.val(matPol);
                    cmbTaxCategory.val(taxCatID);
                    cmbAssetGroup.val(asetGrp);
                    if (IsCon == "Y") {
                        IsConsumable.prop("checked", true);
                    }
                    else {
                        IsConsumable.prop("checked", false);
                    }
                    if (imageUrl != "") {
                        imageUrl = imageUrl.substring(imageUrl.lastIndexOf("/") + 1, imageUrl.length);
                        var d = new Date();
                        imgUsrImage.removeAttr("src").attr("src", VIS.Application.contextUrl + "Images/Thumb140x120/" + imageUrl + "?" + d.getTime());
                    }
                    else {
                        //imgUsrImage.removeAttr("src").attr("src", VIS.Application.contextUrl + "Areas/VA005/Images/img-defult.png");
                        imgUsrImage.removeAttr("src").attr("src", "");

                    }
                    fileUpload.val("");
                    btnUndo.attr('disabled', 'disabled').css("opacity", 0.6);
                    btnSave.attr('disabled', 'disabled').css("opacity", 0.6);
                    $BusyIndicator[0].style.visibility = "hidden";
                });
            }

            if (btnAttributeSet != null) {
                btnAttributeSet.on("click", function (e) {

                    options.Attribute = "Y";
                    options.Asset = "N";
                    options.Tax = "N";

                    //opt.style = 
                    btnAttributeSet.w2overlay(divzoomWindow.clone(true));
                    //$root.find(".w2ui-reset").css("left", e.clientX - 40);
                    e.stopPropagation();
                });
            }

            if (btnTaxCategory != null) {
                btnTaxCategory.on("click", function (e) {

                    options.Attribute = "N";
                    options.Asset = "N";
                    options.Tax = "Y";
                    btnTaxCategory.w2overlay(divzoomWindow.clone(true));
                    e.stopPropagation();
                });
            }

            if (btnAssetGroup != null) {
                btnAssetGroup.on("click", function (e) {

                    options.Attribute = "N";
                    options.Asset = "Y";
                    options.Tax = "N";
                    btnAssetGroup.w2overlay(divzoomWindow.clone(true));
                    e.stopPropagation();
                });
            }

            if (btnZoom != null) {
                btnZoom.on("click", function () {

                    var ad_window_Id = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductCategory/LoadWindow", { "windowName": 'Product Category'});

                    try {
                        //if (dr.read()) {
                        //    ad_window_Id = dr.getInt(0);
                        //}
                        if (ad_window_Id > 0) {
                            var zoomQuery = new VIS.Query();
                            zoomQuery.addRestriction("M_Product_Category_ID", VIS.Query.prototype.EQUAL, VIS.Utility.Util.getValueOfInt(pcat_ID));
                            zoomQuery.setRecordCount(1);
                            VIS.viewManager.startWindow(ad_window_Id, zoomQuery);
                        }
                    }
                    catch (e) {
                        console.log(e);
                    }
                    //var sql = "select ad_window_id from ad_window where name = 'Product Category'";// Upper( name)=Upper('user' )
                    //var ad_window_Id = 0;
                    //try {
                    //    var dr = VIS.DB.executeDataReader(sql);
                    //    if (dr.read()) {
                    //        ad_window_Id = dr.getInt(0);
                    //    }
                    //    dr.dispose();
                    //    if (ad_window_Id > 0) {
                    //        var zoomQuery = new VIS.Query();
                    //        zoomQuery.addRestriction("M_Product_Category_ID", VIS.Query.prototype.EQUAL, VIS.Utility.Util.getValueOfInt(pcat_ID));
                    //        zoomQuery.setRecordCount(1);
                    //        VIS.viewManager.startWindow(ad_window_Id, zoomQuery);
                    //    }
                    //}
                    //catch (e) {
                    //    console.log(e);
                    //}
                });
            }

            if (btnClose != null) {
                btnClose.on("click", function () {

                    $self.dispose();
                });
            }

            if (divzoomWindow != null) {
                divzoomWindow.on("click", "LI", function (e) {

                    var action = $(e.target).data("action");
                    if (action == VIS.Actions.refresh) {
                        if (options.Attribute == "Y")
                            LoadAttributes();
                        else if (options.Tax == "Y")
                            LoadTaxCategories();
                        else if (options.Asset == "Y")
                            LoadAssetGroup();
                    }
                    else if (action == VIS.Actions.zoom) {
                        if (options.Attribute == "Y")
                            zoomToWindow(VIS.Utility.Util.getValueOfInt(cmbAttributeSet.val()), "Attribute Set");
                        else if (options.Tax == "Y")
                            zoomToWindow(VIS.Utility.Util.getValueOfInt(cmbTaxCategory.val()), "Tax Category");
                        else if (options.Asset == "Y")
                            zoomToWindow(VIS.Utility.Util.getValueOfInt(cmbAssetGroup.val()), "Asset Group");
                    }
                });
            }
        };

        var zoomToWindow = function (record_id, windowName) {
            var ad_window_Id = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductCategory/LoadWindow", { "windowName": windowName });

            try {
                //if (dr.read()) {
                //    ad_window_Id = dr.getInt(0);
                //}
                if (ad_window_Id > 0) {
                    var zoomQuery = new VIS.Query();
                    if (windowName == "Attribute Set")
                        zoomQuery.addRestriction("M_AttributeSet_ID", VIS.Query.prototype.EQUAL, record_id);
                    else if (windowName == "Tax Category")
                        zoomQuery.addRestriction("C_TaxCategory_ID", VIS.Query.prototype.EQUAL, record_id);
                    else if (windowName == "Asset Group")
                        zoomQuery.addRestriction("A_Asset_Group_ID", VIS.Query.prototype.EQUAL, record_id);
                    zoomQuery.setRecordCount(1);
                    VIS.viewManager.startWindow(ad_window_Id, zoomQuery);
                }
            }
            catch (e) {
                console.log(e);
            }
            //var sql = "select ad_window_id from ad_window where name = '" + windowName + "'";// Upper( name)=Upper('user' )
            //var ad_window_Id = 0;
            //try {
            //    var dr = VIS.DB.executeDataReader(sql);
            //    if (dr.read()) {
            //        ad_window_Id = dr.getInt(0);
            //    }
            //    dr.dispose();
            //    if (ad_window_Id > 0) {
            //        var zoomQuery = new VIS.Query();
            //        if (windowName == "Attribute Set")
            //            zoomQuery.addRestriction("M_AttributeSet_ID", VIS.Query.prototype.EQUAL, record_id);
            //        else if (windowName == "Tax Category")
            //            zoomQuery.addRestriction("C_TaxCategory_ID", VIS.Query.prototype.EQUAL, record_id);
            //        else if (windowName == "Asset Group")
            //            zoomQuery.addRestriction("A_Asset_Group_ID", VIS.Query.prototype.EQUAL, record_id);
            //        zoomQuery.setRecordCount(1);
            //        VIS.viewManager.startWindow(ad_window_Id, zoomQuery);
            //    }
            //}
            //catch (e) {
            //    console.log(e);
            //

        };

        var addCategory = function () {
            count = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductCategory/GetAddCategory", "");
            //var sql = "SELECT Count(*) FROM M_Product_Category WHERE IsActive = 'Y' AND AD_Client_ID = " + VIS.Env.getCtx().getAD_Client_ID();
            //count = VIS.Utility.Util.getValueOfInt(VIS.DB.exe`cuteScalar(sql.toString())) + 1;
            catName = "Category-" + count;
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "ProductCategory/Save",
                dataType: "json",
                async: false,
                data: {
                    Name: catName
                },
                success: function (data) {
                    data = jQuery.parseJSON(data);
                    var returnValue = data.Key;
                    var returnName = data.Name;

                    if (returnValue != "") {
                        if (mainProductCategoryUl != null) {
                            mainProductCategoryUl.find('li:eq(0)').after("<li class='VA005-pro-cat' procatID=" + returnValue + "><div procatID=" + returnValue + " class='VA005-cat-img'><img style='height: 100%;width: 100%;' procatID=" + returnValue
                                + " src=''> </div> <div class='VA005-cat-caption'><input type='checkbox' class='VA005-catboxcheck'> <p procatID=" + returnValue + " > " + returnName +
                                " </p> <p style='display:none;'><input procatID=" + returnValue + " type='text' value='" + returnName + "'></p></div></li>");
                        }
                        if (!chkMultiple.prop("checked")) {
                            fillCategory(returnValue)
                            mainProductCategoryUl.find('li .VA005-cat-caption').removeClass('VA005-highlighted');
                            mainProductCategoryUl.find("li[procatid='" + returnValue + "'] .VA005-cat-caption").addClass('VA005-highlighted');
                            pcat_ID = returnValue;
                            //pcats = [];
                            //pcats.push(returnValue);
                        }
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    //
                    console.log(textStatus);
                    if (returnValue) {
                        return;
                    }
                }
            });
        };

        var fillCategory = function (cat_ID) {

            if (VIS.Utility.Util.getValueOfInt(cat_ID) > 0) {
                pcat_ID = cat_ID;
                $BusyIndicator[0].style.visibility = "visible";
                divRight.removeClass("VA005-web_dialog_overlay");
                divRight.find('input, textarea, button, select').removeAttr('disabled');
                // var sql = "";
               
                    VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductCategory/GetCategory", { "M_Product_Category_ID": cat_ID }, FillCatCallBack);
                    //sql = "SELECT pc.Name,pc.Value,pc.M_AttributeSet_ID,pc.ProductType,pc.MMPolicy,pc.Description,pc.C_TaxCategory_ID,pc.A_Asset_Group_ID,pc.DTD001_IsConsumable,pc.AD_Image_ID,img.ImageUrl,img.BinaryData FROM M_Product_Category pc" +
                    //  " LEFT JOIN AD_Image img ON pc.AD_Image_ID = img.AD_Image_ID WHERE pc.M_Product_Category_ID = " + cat_ID;
      
                // VIS.DB.executeReader(sql.toString(), null, FillCatCallBack);
            }
        };

        function FillCatCallBack(dr) {
            if (dr.length > 0) {
                for (var i = 0; i < dr.length; i++) {
                    catName = dr[i].Name;
                    searchKey = dr[i].Value;
                    attrSetId = dr[i].M_AttributeSet_ID;
                    pType = dr[i].ProductType;
                    matPol = dr[i].MMPolicy;
                    Description = dr[i].Description;
                    taxCatID = dr[i].C_TaxCategory_ID;
                    asetGrp = dr[i].A_Asset_Group_ID;
                    IsCon = dr[i].DTD001_IsConsumable;
                    ad_image_id = dr[i].AD_Image_ID;
                    imageUrl = dr[i].ImageUrl;
                    txtName.val(catName);
                    txtValue.val(searchKey);
                    txtDesc.val(Description);
                    cmbAttributeSet.val(attrSetId);
                    cmbProductType.val(pType);
                    cmbmatPolicy.val(matPol);
                    cmbTaxCategory.val(taxCatID);
                    cmbAssetGroup.val(asetGrp);
                    if (IsCon == "Y") {
                        IsConsumable.prop("checked", true);
                    }
                    else {
                        IsConsumable.prop("checked", false);
                    }
                    if (imageUrl != "") {
                        imageUrl = imageUrl.substring(imageUrl.lastIndexOf("/") + 1, imageUrl.length);
                        var d = new Date();
                        imgUsrImage.removeAttr("src").attr("src", VIS.Application.contextUrl + "Images/Thumb140x120/" + imageUrl + "?" + d.getTime());
                    }
                    else {
                        //imgUsrImage.removeAttr("src").attr("src", VIS.Application.contextUrl + "Areas/VA005/Images/img-defult.png");
                        imgUsrImage.removeAttr("src").attr("src", "");

                    }
                    btnUndo.attr('disabled', 'disabled').css("opacity", 0.6);
                    btnSave.attr('disabled', 'disabled').css("opacity", 0.6);
                }
                //dr.close();
                //pcats = [];
                //pcats.push(pcat_ID);
                $BusyIndicator[0].style.visibility = "hidden";
            }
        };

        var updateCategory = function (pcat_ID, pname, item) {
            var element1 = item.find("p").eq(0);
            var element2 = item.find("input");
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "ProductCategory/Update",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({
                    ID: pcat_ID,
                    Name: pname
                }),
                success: function (data) {
                    var returnValue = data.result;

                    if (returnValue) {
                        if (mainProductCategoryUl != null) {
                            element1.text(pname);
                        }
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    //
                    console.log(textStatus);
                    if (textStatus) {
                        return;
                    }
                }
            });
        };

        var ShowOrHide = function (isVisible, item) {
            var element1 = item.find("p").eq(0);
            var element2 = item.find("p").eq(1);
            if (isVisible) {
                if (!chkMultiple.prop("checked")) {
                    element1.css({ "display": "none" });
                    element2.css({ "display": "block" });
                    element2.find("input").focus();
                    element2.find("input").select();
                }
            }
            else {
                element1.css({ "display": "block" });
                element2.css({ "display": "none" });
            }
        };

        //var CheckDTD001 = function () {
            //var no = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductCategory/GetCheckDTD001", "");
            //var sql = "SELECT AD_Column_ID FROM AD_Column WHERE AD_Table_ID = 209 AND ColumnName = 'DTD001_IsConsumable'";
            // var no = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(sql.toString(), null, null));
            //if (no > 0) {
               // return true;
           // }
           // return false;
        //};

        var ClearData = function () {
            pcat_ID = 0;
            txtName.val("");
            txtDesc.val("");
            txtValue.val("");
            cmbAssetGroup.val(-1);
            cmbAttributeSet.val(-1);
            cmbmatPolicy.val("F");
            cmbProductType.val("I");
            cmbTaxCategory.val(-1);
            //imgUsrImage.removeAttr("src").attr("src", VIS.Application.contextUrl + "Areas/VA005/Images/Img-defult.png");
            imgUsrImage.removeAttr("src").attr("src", "");
            //divRight.addClass("VA005-web_dialog_overlay");
            //divRight.find('input, textarea, button, select').attr('disabled', 'disabled');
        }

        var LoadCategory = function (pgNo, pgSize) {
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductCategory/LoadCategory", { "PGNo": pgNo, "PGSize": pgSize}, CategoryCallBack);
            //var sql = "SELECT pc.Name,pc.M_Product_Category_ID,img.ImageUrl,img.BinaryData FROM M_Product_Category pc LEFT JOIN AD_Image img ON pc.AD_Image_ID = img.AD_Image_ID WHERE pc.IsActive='Y' AND pc.AD_Client_ID = " + VIS.Env.getCtx().getAD_Client_ID();
            //  Added by Shifali to access product acc to org
            //sql = VIS.MRole.addAccessSQL(sql, "M_Product_Category", true, true);
            //VIS.DB.executeDataReaderPaging(sql.toString(), pgNo, pgSize, null, CategoryCallBack);
        };

        function CategoryCallBack(dr) {
            if (dr.length > 0) {
                for (var i = 0; i < dr.length; i++) {
                    name = dr[i].Name;
                    pcat_ID = dr[i].M_Product_Category_ID;
                    imageUrl = dr[i].ImageUrl;
                    //binaryData = dr.getString(3);
                    if (imageUrl != "") {
                        imageUrl = imageUrl.substring(imageUrl.lastIndexOf("/") + 1, imageUrl.length);
                        var d = new Date();
                        mainProductCategoryUl.append("<li class='VA005-pro-cat' procatID=" + pcat_ID + "><div procatID=" + pcat_ID + " class='VA005-cat-img'><img procatID=" + pcat_ID + " style='height: 100%;width: 100%;' src='"
                            + VIS.Application.contextUrl + "Images/Thumb140x120/" + imageUrl + "?" + d.getTime() + "'> </div> <div class='VA005-cat-caption'> <input type='checkbox' class='VA005-catboxcheck'><p procatID=" + pcat_ID + " > " + name +
                            " </p> <p style='display:none;width:100%;'><input procatID=" + pcat_ID + " type='text' value='" + name + "'></p></div></li>");
                    }
                    else {
                        mainProductCategoryUl.append("<li class='VA005-pro-cat' procatID=" + pcat_ID + "><div procatID=" + pcat_ID + " class='VA005-cat-img'><img procatID=" + pcat_ID
                            + " style='height: 100%;width: 100%;' src=''> </div> <div class='VA005-cat-caption'><input type='checkbox' class='VA005-catboxcheck'> <p procatID=" + pcat_ID + "> " + name +
                            " </p> <p style='display:none;width:100%;'><input procatID=" + pcat_ID + " type='text' value='" + name + "'></p></div></li>");
                    }
                }
                //dr.close();
            }
        }

        this.Initialize = function () {
            //load by java script
            load();
        }

        //Privilized function
        this.getRoot = function () {
            return $root;
        };

        function busyIndicator() {
            $BusyIndicator = $('<div class="vis-busyindicatorouterwrap" style="visibility: hidden;"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
            //$BusyIndicator.css({
            //    "position": "absolute", "width": "98%", "height": "97%", 'text-align': 'center'
            //});
            $BusyIndicator[0].style.visibility = "visible";
            $root.append($BusyIndicator);
        };
        /*
        dispose all object used in this form
        */
        this.disposeComponent = function () {
            $self = null;
            if ($root)
                $root.remove();
            $root = null;

            if (btnSave)
                btnSave.off("click");
            if (btnClose)
                btnClose.off("click");
            if (btnDelete)
                btnDelete.off("click");
            if (btnMultiple)
                btnMultiple.off("click");
            if (btnAddCategory)
                btnAddCategory.off("click");
            if (mainProductCategoryUl) {
                mainProductCategoryUl.off("click");
                mainProductCategoryUl.remove();
            }
            $busyDiv = null;
            divLeft = null;
            divRight = null;
            chkMultiple = null;
            txtName = null;
            mainProductCategoryUl = null;
            btnAddCategory = null;
            $BusyIndicator = null;
            name = "";
            pcat_ID = 0;
            pcatImg = 0;
            pcatIdOld = 0;
            pcatOld = "";
            imageUrl = null;
            binaryData = null;
            count = 0;
            catName = "";
            currentElement = null;
            btnSave = null;
            btnDelete = null;
            btnClose = null;
            btnMultiple = null;
            btnUndo = null;
            btnAttributeSet = null;
            btnTaxCategory = null;
            btnAssetGroup = null;
            txtName = null;
            txtValue = null;
            txtDesc = null;
            cmbProductType = null;
            cmbmatPolicy = null;
            cmbAttributeSet = null;
            cmbTaxCategory = null;
            cmbAssetGroup = null;
            IsDefault = null;
            IsSelfService = null;
            IsConsumable = null;
            fileUpload = null;
            //holderDiv = null;
            imgUsrImage = null;
            lblfileUpload = null;
            chkDataBaseSave = null;
            change = false;
            ad_image_id = 0;
            pageNo = 0;
            PAGESIZE = 0;
            searchKey = "";
            attrSetId = 0;
            pType = "";
            Description = "";
            matPol = "";
            taxCatID = 0;
            asetGrp = 0;
            IsCon = "";
            this.getRoot = null;
            this.disposeComponent = null;
        };
    };

    //Must Implement with same parameter
    VA005.ProductCategoryForm.prototype.init = function (windowNo, frame) {
        //Assign to this Varable
        this.frame = frame;
        this.windowNo = windowNo;
        var obj = this.Initialize();
        this.frame.getContentGrid().append(this.getRoot());
        this.frame.hideHeader(true);
    };

    //Must implement dispose
    VA005.ProductCategoryForm.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };

})(VA005, jQuery);