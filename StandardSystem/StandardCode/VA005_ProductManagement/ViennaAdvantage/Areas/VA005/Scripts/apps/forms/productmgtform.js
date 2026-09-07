
; VA005 = window.VA005 || {};
//java script function closer
; (function (VA005, $) {

    //Form Class function fullnamespace
    VA005.ProductMgtForm = function () {
        this.frame;
        this.windowNo;
        var $self = this; //scoped self pointer
        var $root = $('<div class="vis-group-assign-content vis-forms-container VA005-outermainwrapper" style="height:100%;">');
        var $zoomDiv = null;
        var $table;
        var $td0leftbar, $btnlbToggle, $searchQry, $searchCat, $searchBy, $ulLefttoolbar, $divlbMain, $td2_tr1; //left bar
        var $lb;
        var $tr;
        var $leftPanel = $('<div style="display: inline-block;float:left;height:100%;width:250px;background:rgb(87, 87, 87);">');
        var $catPanel = null;
        var $bsyDiv = null;
        var middleMain = null;
        var middleMaindiv = null;
        var $middlePanel = null;
        var $rightPanel = null;
        var ImagePanel = null;
        var $searchProduct = null;
        var $queryCat = null;
        //Manjot
        var $queryBy = null;
        var $leftui = null;
        var $categDiv = null;
        //end
        var format = VIS.DisplayType.GetNumberFormat(VIS.DisplayType.Amount);
        var dotFormatter = VIS.Env.isDecimalPoint();
        // added by Bharat on 09 March 2018
        var $txtsearchCat = null;
        var $searchCatBtn = null;
        var $searchProdBtn = null;
        var $btnCreateProd = null;
        var $divProduct = null;
        var $divProductInner = null;
        var $divUom = null;
        var $divUomGroup = null;
        var divUom = null;
        var divConversion = null;
        var $divVarient = null;
        var $divCartdata = null;
        var $divProdDetail = null;
        var $divProductDet = null;
        var $ProductDetails = null;
        var $divRelated = null;
        var divZoomProdName = null;
        var btnEditDetail = null;
        var prodName = null;
        var prodUPC = null;
        var prodAttributeSet = null;
        var prodCategory = null
        var prodTaxCat = null;
        var prodType = null;
        var prodUOM = null;
        var prodParent = null;
        var divCart = null;
        var divNewCart = null;
        var divCartList = null;
        var $divLeftTree = null;
        var divtree = null;
        var divVarient = null;
        var divAttr = null;
        var ch = null;
        var pl = null;
        var ml = null;
        var pr = null;
        var isOk = true;
        var $maindiv = $('<div class="vis-forms-container vis-formouterwrpdiv" style="display:none;"></div>'); //layout
        var $div = $('<div style="overflow-y:auto;padding-top: 5px;"></div>');
        var $divPrice = $('<div style="width:100%;height:100%;padding-top: 3px;"></div>');
        var $divPriceMain = $('<div style="width:100%;height:100%;display:none;"></div>'); //layout
        var $divSupp = $('<div style="width:100%;height:100%;"></div>');
        var $divSupplier = $('<div style="width:100%;height:100%;display:none;"></div>');
        var $divCart = $('<div style="width:100%;height:100%;"></div>');
        var $divCartMain = $('<div style="width:100%;height:100%;"></div>');
        var $divPrint = $('<div style="display:none;"></div>'); //layout
        var $divPrintPop = $('<div style="overflow-y:auto;"></div>');
        var $divRelatedGrid = $('<div style="width:100%;height:100%;"></div>');
        var priceGrid = null;
        var supplierGrid = null;
        var prodmodtmp, prodtheModTmp;
        var $divHeadProd = null;
        var $divHeadVarient = null;
        var catSpan = null;
        var currSpan = null;
        var btnCopy = null;
        var btnImage = null;
        var btnPriceList = null;
        var btnSupplier = null;
        var btnEditMultiple = null;
        var btnAddCart = null;
        var btnUomCart = null;
        var btnVarientCart = null;
        var btnShowAll = null;
        var btnSelectAll = null;
        var btnDetails = null;
        var btnVarient = null;
        var btnUom = null;
        var btnRelated = null;
        var btnCart = null;
        var btnAdduom = null;
        var btnAddVarient = null;
        var btnGenerate = null;
        var btnSaveAttr = null;
        var btnCancelGen = null;
        var Multiplier = null;
        var Division = null;

        var btnCancelVarient = null;
        var btnSaveUom = null;
        var btnCancelUom = null;
        var btnUpload = null;
        var btnImgDelete = null;
        var btnImgCancel = null;
        var btnRefreshForm = null;
        var btnNewCart = null;
        var btnEditCart = null;
        var btnRefreshCart = null;
        var btnSaveScan = null;
        var btnCancelScan = null;
        var btnPrint = null;
        var btnErase = null;
        var $attrControl = null;
        var txtEditUPC = null;
        var txtHeader = null;
        var cmbVersion = null;
        var pop = null;
        var patr_ID = 0;
        var orgid = 0;
        var txtScan = null;
        var txtName = null;
        var txtValue = null;
        var txtUpc = null;
        var $txtMul = null;
        var $txtDiv = null;
        var uomUPC = false;
        var cmbOrg = null;
        var cmbProductType = null;
        var cmbAttributeSet = null;
        var cmbTaxCategory = null;
        var cmbUOM = null;
        var cmbUomTo = null;
        var cmbCat = null;
        var cmbPriceList = null;
        var cmbSupplier = null;
        var cmbUomPrice = null;
        var cmbCart = null;
        var cart = 0;
        var btnAttributeSet = null;
        var btnProdCat = null;
        var btnTaxCategory = null;
        var btnUomZoom = null;
        var printData = [];
        var prods = [];
        var uoms = [];
        var cons = [];
        var attributes = [];
        var upcs = [];
        var ProdID = 0;
        var uomID = 0;
        var c_UomConv_ID = 0;
        var m_attribute_ID = 0;
        var upcvalue = "";
        var pcat_ID = 0;
        var M_Product_ID = 0;
        //manjot
        var parent_ID = 0, parentCat = 0, parentTax = 0;
        //end
        var productCount = 0;
        var noPages = 0;
        var noOfCats = 0;
        //actions       
        var uomCmb = null;
        var uomArray = [];
        var curArray = [];
        var multiValues = [];
        var relatedType = [];
        var AD_Column_ID = 0;
        var dGrid = null;
        var sGrid = null;
        var dRelatedGrid = null;
        var cartGrid = null;
        var fileUpload = null;
        var divUsrImage = null;
        var imgUsrImage = null;
        var name = "";

        var count = 0;
        var catName = "";
        var ad_image_id = 0;
        var pgno = 1;
        var pageno = 1;
        var PAGESIZE = 50;

        var searchKey = "";
        var attrSetId = 0;
        var pType = "";
        var Description = "";
        var matPol = "";
        var taxCatID = 0;
        var asetGrp = 0;
        var IsCon = "";
        var ProductImgul = null;
        var selectedimg = [];
        var $slctimg = null;
        var parnt = [];
        var partChild = [];
        var attr = [];
        var attrArray = {};
        var options = {
            Attribute: "N",
            Tax: "N",
            Category: "N",
            UOM: "N"
        }
        var isClick = false;
        var precision = 2;
        var curName = null;
        var prodContainerWidth = null;
        //Change By Mohit 
        var _countDTD001 = false;
        //----end-----------

        //Manjot
        var $leftTreeKeno;
        var $leftTreeDiv = $('<div>');
        var leftHeight = 0;
        //End
        // Manish 10/4/2016..
        var productAttValues = null;
        var currentheight = null;
        var $btnAttrValueControls = null;
        var $btnDelAttValues = null;
        var $btnAttValuesDivClose = null;
        var attrBottomValuesFlag = false;
        var $lblSelectedNodeText = null;
        var $getProdectIDAfterEdit = null;
        var $prodIDAfterEdit = null;

        var textDiv = null;
        var attrValWraperDiv = null;
        var midtopdivvv = null;
        var uomChange = false;
        //end 

        createBusyIndicator();
        this.Initialize = function () {
            //load by java script            
            createPanels();
        };

        this.initData = function () {

            window.setTimeout(function () {
                //Mohit
                //LoadModuleInfo();
                //--end---
                LoadQueries();
                LoadCurrency();
                LoadUomTo();
                AddProductPanel();
                $self.GetImages();
                DragDropDocument();
                DragDropProduct();
                loadCategories($txtsearchCat.val(), pageno, PAGESIZE);
                //manjot
                $leftTreeDiv.css({ "display": "none" });
                //end
                createProductList();
                events();
            }, 200);
        };

        function createPanels() {
            $td0leftbar = $("<td class='VA005-Left-Bar vis-leftsidebarouterwrap'>");
            $lb = $("<div class='VA005-apanel-lb'>");

            $btnlbToggle = $("<div class='vis-apanel-lb-toggle' ><i class='fa fa-bars' aria-hidden='true'></i></div>");
            $searchQry = $('<div class="vis-input-wrap"><div class="VA005-Serach-Query vis-control-wrap"><Select id = ' + "queryCat_" + $self.windowNo +
                ' ></Select><label>' + VIS.Msg.getMsg("VA005_SearchQuery") + '</label></div></div>');
            //Updated By Manjot For SearchBy Category And Tree
            $searchBy = $('<div class="vis-input-wrap"><div class="VA005-Serach-Query vis-control-wrap"><Select id = "searchBy_' + $self.windowNo +
                '" ><option selected="selected" value="C">' + VIS.Msg.getMsg("VA005_SearchByCat") +
                '</option><option value="T">' + VIS.Msg.getMsg("VA005_SearchByTree") + '</option></Select><label>' + VIS.Msg.getMsg("VA005_SearchBy") + '</label></div></div>');

            // Added by Bharat on 09 March 2018 to add search box for Categories
            $searchCat = $('<div class="VA005-left-input-panel input-group vis-input-wrap"><div class="vis-control-wrap"><input id = ' + "VA005_searchCat_" + $self.windowNo + ' class="VA005-left-panelInput" type="text" value="" placeholder="' + VIS.Msg.getMsg("Search") +
                '" data-hasbtn=" "></div><div class="input-group-append"><button class="VA005-leftpanelcatsearchbtn input-group-text vis-group-search-icon"><i class="vis vis-search"></i></button></div></div>');
            //updated by Manjot Given ID To UI
            $ulLefttoolbar = $("<ul id= leftUi_" + $self.windowNo + ">");
            //End
            $divlbMain = $('<div class="vis-apanel-lb-main VA005-div-left-main">');
            $divlbMain.append($searchQry);
            $divlbMain.append($searchBy);
            //End
            $divlbMain.append('<div class="VA005-left-cat-panel"> <h4>' + VIS.Msg.getMsg("VA005_Categories") + '</h4></div>');
            $divlbMain.append($searchCat);
            $divlbMain.append($ulLefttoolbar);
            //manjot
            $divlbMain.append($leftTreeDiv);
            //end
            $lb.append($btnlbToggle);
            $lb.append($divlbMain);
            $td0leftbar.append($lb);


            $divHeadProd = $('<div class="VA005-middle-head"><div class="VA005-middle-search">' +
                '<button class="vis-group-add-btn VA005-leftpanelcatsearchbtn input-group-text" style="margin-bottom: 15px;"><i class="vis vis-plus"></i></button>' +
                '<div class="input-group vis-input-wrap"><div class="vis-control-wrap"><input class="vis-group-SearchText vis-group-addLeft" value="" placeholder="' + VIS.Msg.getMsg("Search") +
                '" data-placeholder="" data-hasbtn=" " type="text"></div><div class="input-group-append"><button class="vis-group-search-icon VA005-leftpanelcatsearchbtn input-group-text"><i class="vis vis-search"></i></button></div></div></div>' +
                '<div class="VA005-middle-icons"><ul><li id="btnRefreshForm_' + $self.windowNo + '"><span class="vis vis-refresh VA005-icons-font" title="' + VIS.Msg.getMsg("VA005_Refresh") + '"></span></li>' +
                '<li id="btnCopy_' + $self.windowNo + '"><span class="vis vis-copy VA005-icons-font" title="' + VIS.Msg.getMsg("Copy") + '"></span></li>' +
                //'<span class="vis vis-edit VA005-icons-font" title="' + VIS.Msg.getMsg("VA005_CopyProduct") + '"></span></li>' +
                '<li id="btnImage_' + $self.windowNo + '"><span class="vis vis-image VA005-icons-font" title="' + VIS.Msg.getMsg("VA005_UploadImage") + '"></span></li>' +
                '<li id="btnSupplier_' + $self.windowNo + '"><span class="vis vis-user VA005-icons-font" title="' + VIS.Msg.getMsg("VA005_UpdateSupplier") + '"></span></li>' +
                '<li id="btnPriceList_' + $self.windowNo + '"><span class="fa fa-usd VA005-icons-font" title="' + VIS.Msg.getMsg("VA005_UpdatePrice") + '"></span></li>' +
                '<li id="btnAddCart_' + $self.windowNo + '"><span class="fa fa-shopping-cart VA005-icons-font" title="' + VIS.Msg.getMsg("VA005_AddCart") + '"></span></li>' +
                '<li id="btnEditMuLtiple_' + $self.windowNo + '"><span class="vis vis-edit VA005-icons-font" title="' + VIS.Msg.getMsg("VA005_UpdateProduct") + '"></span></li></ul></div></div>' +
                '<div class="VA005-middle-cat-bar"><span style="font-weight:bold;">' + VIS.Msg.getMsg("VA005_Categories") + ' : </span><span id="btnShowAll_'
                + $self.windowNo + '" class="vis-group-pointer" style="color:rgba(var(--v-c-primary), 1);">' + VIS.Msg.getMsg("All") + '</span> <span id="catInfo_' + $self.windowNo + '" style="display:none;"></span></div>' +
                '<div class="VA005-middle-cart-bar"><input type="checkbox" id="VA005_chkAll_' + $self.windowNo + '"></input><span>' + VIS.Msg.getMsg("SelectAll") + '</span>' +
                '<div class="VA005-selectedCartLblWrap"><span style="font-weight:bold;">' + VIS.Msg.getMsg("VA005_SelectedCart") +
                ' : </span><span id="VA005_cartInfo_' + $self.windowNo + '">' + VIS.Msg.getMsg("None") + '</span></div></div>');
            $divHeadVarient = $('<div class="VA005-right-head VA005-tab-control"><ul class="VA005-tabs" style="overflow: auto;white-space: nowrap;">' +
                '<li id="VA005_btnDetails_' + $self.windowNo + '" class="VA005-selectedTab">' + VIS.Msg.getMsg("Details") + '</li>' +
                '<li id="VA005_btnVarient_' + $self.windowNo + '">' + VIS.Msg.getMsg("VA005_Varients") + '</li>' +
                '<li id="VA005_btnUom_' + $self.windowNo + '">' + VIS.Msg.getElement(VIS.Env.getCtx(), "C_UOM_ID") + '</li>' +
                '<li id="VA005_btnRelated_' + $self.windowNo + '">' + VIS.Msg.getMsg("Related") + '</li>' +
                '<li id="VA005_btnCart_' + $self.windowNo + '">' + VIS.Msg.getMsg("VA005_Cart") + '</li></ul></div>');
            //$middlePanel = $('<div class="VA005-middle-left-main">').append($divHeadProd);

            //Manish 26/9/2016...            
            $btnAttrValueControls = $('<div class="VA005-attListing-header"><p id="VA005_SelectedNodeText_' + $self.windowNo + '" class="VA005-attListing-title">bfehfh</p><div style="float:right"><span id="VA005_btnDeleteAttVal_' + $self.windowNo + '"  class="VA005-RemoveLinkeImage VA005-AttValOpacity" style="font-size:18px;margin-right:6px" title="' + VIS.Msg.getMsg("VA005_RemoveImage") + '"></span><span id="VA005_btnDeleteCross_' + $self.windowNo + '"  style="font-size: 18px; cursor: pointer" class="vis vis-mark " title="' + VIS.Msg.getMsg("VA005_Close") + '"></span></div></div>');
            productAttValues = $('<div class="VA005-AttributesValMainDiv"></div>');
            $middlePanel = $('<div class="VA005-middle-left-main" style="height:98%" >').append($divHeadProd);
            //End 26/9/2016...



            $rightPanel = $('<div class="VA005-middle-right-main">');
            ImagePanel = $('<div class="VA005-middle-right-main">');
            var imagepanelTop = ('<div class="VA005-Image-panel-top"><div class="VA005-Image-panel-div"><i id="VA005_btnerase_' + $self.windowNo +
                '" class="fa fa-eraser" title="' + VIS.Msg.getMsg("Clear") + '"></i><span id="VA005_ImgDelete_' + $self.windowNo +
                '" class="vis vis-delete" title="' + VIS.Msg.getMsg("DeleteActivity") + '"></span><span id="VA005_btnUpload_' + $self.windowNo +
                '" class="vis vis-upload" title="' + VIS.Msg.getMsg("VA005_UploadImage") + '"></span><span id="VA005_btnImgCancel_' + $self.windowNo +
                '" class="fa fa-times-circle-o" style="font-size: 22px !important;" title="' + VIS.Msg.getMsg("Cancel") + '"></span></div></div>');
            ImagePanel.append(imagepanelTop);
            var divImage = $('<div class="VA005-div-images"><ul class="VA005-productimgul"></ul></div>');
            ImagePanel.append(divImage);

            ProductImgul = divImage.find(".VA005-productimgul");
            ImagePanel.hide();
            $td2_tr1 = $("<td>");
            middleMain = $('<div style="position: relative;width: 100%;height: 100%;background-color:white;">');
            middleMaindiv = $('<div class="VA005-middle-div-main vis-formouterwrpdiv">').append($middlePanel).append($rightPanel).append(ImagePanel);
            middleMain.append(middleMaindiv);
            $td2_tr1.append(middleMain);
            $tr = $("<tr>").append($td0leftbar).append($td2_tr1); //row 1
            $table = $("<table style='width:100%;height:100%;'>"); //main root
            $table.append($tr);
            $root.append($table);
            //$middlePanel.append('<div class="VA005-middle-content">');
            $rightPanel.append($divHeadVarient);
            $queryCat = $root.find("#queryCat_" + $self.windowNo);
            //Manjot
            $queryBy = $root.find("#searchBy_" + $self.windowNo);
            $leftui = $root.find("#leftUi_" + $self.windowNo);
            $categDiv = $root.find(".VA005-left-cat-panel");
            //End

            // Added by Bharat on 09 March 2018 to add search for categories
            $txtsearchCat = $divlbMain.find('#VA005_searchCat_' + $self.windowNo);
            $searchCatBtn = $divlbMain.find('.vis-group-search-icon');

            btnCopy = $root.find("#btnCopy_" + $self.windowNo);
            btnImage = $root.find("#btnImage_" + $self.windowNo);
            btnPriceList = $root.find("#btnPriceList_" + $self.windowNo);
            btnSupplier = $root.find("#btnSupplier_" + $self.windowNo);
            btnEditMultiple = $root.find("#btnEditMuLtiple_" + $self.windowNo);
            btnAddCart = $root.find("#btnAddCart_" + $self.windowNo);
            btnShowAll = $root.find("#btnShowAll_" + $self.windowNo);
            btnSelectAll = $root.find("#VA005_chkAll_" + $self.windowNo);
            btnDetails = $root.find("#VA005_btnDetails_" + $self.windowNo);
            btnVarient = $root.find("#VA005_btnVarient_" + $self.windowNo);
            btnUom = $root.find("#VA005_btnUom_" + $self.windowNo);
            btnRelated = $root.find("#VA005_btnRelated_" + $self.windowNo);
            btnCart = $root.find("#VA005_btnCart_" + $self.windowNo);
            btnImgDelete = $root.find("#VA005_ImgDelete_" + $self.windowNo);
            btnErase = $root.find("#VA005_btnerase_" + $self.windowNo);
            btnUpload = $root.find("#VA005_btnUpload_" + $self.windowNo);
            btnImgCancel = $root.find("#VA005_btnImgCancel_" + $self.windowNo);
            btnRefreshForm = $root.find("#btnRefreshForm_" + $self.windowNo);
            catSpan = $root.find("#catInfo_" + $self.windowNo);
            //middleMaindiv.hide();

            middlePanel();
            rightPanel();
            $zoomDiv = $('<ul class="vis-apanel-rb-ul"><li data-action="zoom" style="opacity: 1" title="' + VIS.Msg.getMsg("VA005_Zoom") + '"><i class="vis vis-find" data-action="zoom"></i>' +
                '</li><li data-action="refresh" title="' + VIS.Msg.getMsg("VA005_Refresh") + '"><i class="vis vis-refresh" data-action="refresh"></i></li></ul');
            //$root.append($zoomDiv);
            $ImageZoom = $('<div id="popupPhotoLandscape" class="VA005-large"></div>');
            $root.append($ImageZoom);
            $ImageZoom.hide();
            $lb.height($td0leftbar.height());
            $divlbMain.height($lb.height() - 43);
            leftHeight = $divlbMain.height() - ($root.find('.VA005-Serach-Query').height() * 2) - 30;
        };

        function loadCategories(query, pgNo, pgSize) {
            $.ajax({
                url: VIS.Application.contextUrl + "VA005/ProductManagement/GetProdCats",
                type: "GET",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                //async: false,
                data: ({ searchText: query, pageNo: pgNo, pageSize: pgSize }),
                success: function (result) {
                    var data = JSON.parse(result);
                    if (data.length > 0) {
                        if (pgNo == 1) {
                            noOfCats = Math.ceil(data[0].TotalRecords / pgSize);
                        }

                        for (item in data) {
                            $ulLefttoolbar.append('<li procatid = ' + data[item].M_ProdCatID + '><span procatid = '
                                + data[item].M_ProdCatID + '>' + VIS.Utility.encodeText(data[item].Catname) + ' (' + data[item].ProdCount + ')</span></li>');
                        }
                    }
                    if (pgNo > 1) {
                        $bsyDiv[0].style.visibility = 'hidden';
                    }
                },
                error: function () {

                    VIS.ADialog.error("VA005_ErrorLoadingProdCats");
                    $bsyDiv[0].style.visibility = 'hidden';
                }
            });
        };

        function middlePanel() {
            $searchProduct = $divHeadProd.find('.vis-group-SearchText');
            $searchProdBtn = $divHeadProd.find('.vis-group-search-icon');
            $btnCreateProd = $divHeadProd.find('.vis-group-add-btn');


            /*****  end of content-headDown  *****/


            //midtopdivvv = $('<div style="border-bottom-color: rgb(26, 160, 237); border-bottom: 2px solid rgb(26, 160, 237);">');
            midtopdivvv = $('<div style="width:100%">');
            $divProduct = $('<div class="vis-group-users-container VA005-productouterwrp" style="margin:0;">');
            $divProductInner = $('<div class="vis-group-DataContainer">');
            $divProduct.height($($root.parent()).height() - 95);
            midtopdivvv.append($divProduct);
            $divProduct.append($divProductInner);
            textDiv = $('<div style="height:100%">');
            textDiv.append(midtopdivvv).append($('<div class="VA005-attrValuesWraperDiv" >').append(productAttValues));
            $middlePanel.append(textDiv);

            attrValWraperDiv = textDiv.find(".VA005-attrValuesWraperDiv");

            midtopdivvv.resizable({
                handles: 's,se'
            });
            midtopdivvv.find(".ui-resizable-s").css("display", "none");
            midtopdivvv.find(".ui-resizable-se").css("display", "none");
            $root.find(".ui-resizable-s").css("height", "10px");


            //$divProduct = $('<div class="vis-group-users-container" style="margin:0;">');
            //$divProduct.height($($root.parent()).height() - 95);
            //$middlePanel.append($divProduct);
            //createProductList();
        };

        function rightPanel() {
            $divUom = '<div id="VA005_divUom_' + $self.windowNo + '" class="VA005-right-head-btn"><div class="VA005-Add-Btn" id="VA005_btnAddUom_' + $self.windowNo +
                '"><span class="vis vis-plus" ></span><span style="margin-left:10px;">' + VIS.Msg.getMsg("VA005_AddUom") + '</span></div>' +
                '<div id="VA005_divConversion_' + $self.windowNo + '" class="VA005-conv-form"><div class="VA005-conv-data input-group vis-input-wrap"><div class="vis-control-wrap"><select id="cmbUomTo_' + $self.windowNo + '"></select></div></div>' +
                '<div class="VA005-conv-data"><div class="VA005-conversion-data input-group vis-input-wrap"><div class="vis-control-wrap VA005-Multiplier-data">' +
                //'<label>' + VIS.Msg.getElement(VIS.Env.getCtx(), "MultiplyRate") + '</label>' +
                '</div></div>' +
                '<div class="VA005-conversion-data input-group vis-input-wrap"><div class="vis-control-wrap VA005-Division-data">' +
                //'<label>' + VIS.Msg.getElement(VIS.Env.getCtx(), "DivideRate") + '</label>' +
                '</div></div></div>' +
                '<div class="VA005-conv-data"><div class="VA005-conversion-data input-group vis-input-wrap"><div class="vis-control-wrap"><input id="VA005_uomUPC_' + $self.windowNo + '" placeholder=" " data-placeholder=""><label>' + VIS.Msg.getElement(VIS.Env.getCtx(), "UPC") + '</label></div></div>' +
                '<div class="VA005-conversion-icons"><span id="VA005_btnSaveUom_' + $self.windowNo +
                '" class="VA005-icons vis vis-save VA005-icons-font" tabindex="0" title="' + VIS.Msg.getMsg("Save") + '"></span><span id="VA005_btnCancelUom_' + $self.windowNo +
                '" class="VA005-icons fa fa-times-circle-o VA005-icons-font" tabindex="0" title="' + VIS.Msg.getMsg("Cancel") + '" style="font-size: 1.4rem;"></span></div></div>';
            // Added new controls by Shifali on 03 July 2020 to change the amount acc. to culture.
            $txtMul = new VIS.Controls.VAmountTextBox("MulAmount", false, false, true, 50, 100, VIS.DisplayType.Amount, VIS.Msg.getMsg("Amount"));
            $txtDiv = new VIS.Controls.VAmountTextBox("DivAmount", false, false, true, 50, 100, VIS.DisplayType.Amount, VIS.Msg.getMsg("Amount"));
            $txtDiv.addVetoableChangeListener(this);
            $txtMul.addVetoableChangeListener(this);
            $rightPanel.append($divUom);
            $divUomGroup = $('<div class="VA005-uom-list">');
            Multiplier = $rightPanel.find(".VA005-Multiplier-data");
            Multiplier.append($txtMul.getControl().attr('placeholder', ' ').attr('data-placeholder', '')).append('<label>' + VIS.Msg.getElement(VIS.Env.getCtx(), "MultiplyRate") + '</label>');
            Division = $rightPanel.find(".VA005-Division-data");
            Division.append($txtDiv.getControl().attr('placeholder', ' ').attr('data-placeholder', '')).append('<label>' + VIS.Msg.getElement(VIS.Env.getCtx(), "DivideRate") + '</label>');
            $rightPanel.append($divUomGroup);
            divUom = $rightPanel.find("#VA005_divUom_" + $self.windowNo);
            divUom.hide();
            $divUomGroup.hide();
            btnAdduom = $rightPanel.find("#VA005_btnAddUom_" + $self.windowNo);
            divConversion = $rightPanel.find("#VA005_divConversion_" + $self.windowNo);
            divConversion.hide();
            cmbUomTo = $rightPanel.find("#cmbUomTo_" + $self.windowNo);
            //txtMul = $rightPanel.find("#VA005_txtMul_" + $self.windowNo);
            //txtDiv = $rightPanel.find("#VA005_txtDiv_" + $self.windowNo);
            uomUPC = $rightPanel.find("#VA005_uomUPC_" + $self.windowNo);
            btnSaveUom = $rightPanel.find("#VA005_btnSaveUom_" + $self.windowNo);
            btnCancelUom = $rightPanel.find("#VA005_btnCancelUom_" + $self.windowNo);
            $divUomGroup.on("click", uomContainerClick);

            $rightPanel.append('<div id="VA005_divVarient_' + $self.windowNo + '" class="VA005-right-head-btn"><div class="VA005-Add-Btn" id="VA005_btnAddVarient_' + $self.windowNo +
                '"><span class="vis vis-plus" ></span><span style="margin-left:10px;">' + VIS.Msg.getMsg("VA005_AddVarient") + '</span></div>' +
                '<div class="VA005-Gen-Btn" id="VA005_btnGenerate_' + $self.windowNo +
                '"><span class="glyphicon glyphicon-list-alt" ></span><span class="VA005-GenVariantLabl">' + VIS.Msg.getMsg("VA005_GenVariant") + '</span></div>' +
                '<div class="VA005-conv-form"><div class="VA005-conv-data" id="VA005_divAttr_' + $self.windowNo + '"></div></div>');
            $divVarient = $('<div class="VA005-uom-list">');
            $divLeftTree = $('<div class="VA005-tree-list">');
            divtree = $('<div class="VA005-tree-data">');
            $divLeftTree.append('<div style="float:right;margin-top: 5px;"><span id="VA005_SaveAttribute_' + $self.windowNo + '" class="VA005-icons vis vis-save VA005-icons-font" title="' + VIS.Msg.getMsg("Save") + '">' +
                '</span><span id="VA005_btnCancelGenerate_' + $self.windowNo + '" class="VA005-icons vis vis-mark VA005-icons-font" title="' + VIS.Msg.getMsg("Cancel") + '"></span></div>').append(divtree);
            $rightPanel.append($divVarient).append($divLeftTree);

            divVarient = $rightPanel.find("#VA005_divVarient_" + $self.windowNo);
            divVarient.hide();
            $divVarient.hide();
            $divLeftTree.hide();
            divAttr = $rightPanel.find("#VA005_divAttr_" + $self.windowNo);
            divAttr.hide();
            btnAddVarient = $rightPanel.find("#VA005_btnAddVarient_" + $self.windowNo);
            btnGenerate = $rightPanel.find("#VA005_btnGenerate_" + $self.windowNo);
            btnSaveAttr = $rightPanel.find("#VA005_SaveAttribute_" + $self.windowNo);
            btnCancelGen = $rightPanel.find("#VA005_btnCancelGenerate_" + $self.windowNo);
            //btnGenerate.hide();
            var lookupCur = new VIS.MPAttributeLookup(VIS.context, $self.windowNo);
            $attrControl = new VIS.Controls.VPAttribute("M_AttributeSetInstance_ID", false, false, true, VIS.DisplayType.PAttribute, lookupCur, $self.windowNo, false, false, false, true);
            //$attrControl.getControl().css("width", "55%");
            //$attrControl.getControl().css("float", "left");
            //$attrControl.getControl().css("height", "29px");
            //$attrControl.getBtn(0).css("height", "29px");
            //$attrControl.getBtn(0).css("float", "left");
            divAttr.css('display', 'flex');
            var $DivAttrInputWrp = $('<div class="input-group vis-input-wrap">');
            var $DivAttrCtrlWrp = $('<div class="vis-control-wrap">');
            var $DivAttrBtnWrp = $('<div class="input-group-append">');
            divAttr.append($DivAttrInputWrp);
            $DivAttrInputWrp.append($DivAttrCtrlWrp);
            $DivAttrInputWrp.append($DivAttrBtnWrp);
            $DivAttrCtrlWrp.append($attrControl.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' '));
            //$DivAttrBtnWrp.append($attrControl.getBtn(0));
            $DivAttrBtnWrp.append('<button class="input-group-text VA005_AttrBtn"><i class="vis vis-pattribute"></i></button>');
            divAttr.append('<span id="VA005_btnCancelVarient_' + $self.windowNo + '" class="VA005-icons vis vis-mark VA005-icons-color" style="float:left;" title="' + VIS.Msg.getMsg("Cancel") + '"></span>');
            //$attrControl.getBtn(0).on(VIS.Events.onClick, AttributeCtrl);
            $rightPanel.find(".VA005_AttrBtn").on(VIS.Events.onClick, AttributeCtrl);
            btnCancelVarient = $rightPanel.find("#VA005_btnCancelVarient_" + $self.windowNo);
            $divVarient.on("click", VarientContainerClick);

            $rightPanel.append('<div id="VA005_divCart_' + $self.windowNo + '" class="VA005-right-head">' +
                '<div id="VA005_divCartList_' + $self.windowNo + '" class="VA005-conv-data"><div class="VA005-conversion-data input-group vis-input-wrap"><div class="vis-control-wrap"><select id="VA005_cmbCart_' + $self.windowNo + '"></select><label>' + VIS.Msg.getMsg("VA005_Cart") +
                '</label></div></div>' +
                '<div><span class="VA005-icons vis vis-plus VA005-icons-font" title="' + VIS.Msg.getMsg("VA005_AddNewCart") + '"></span>' +
                '<span class="VA005-icons vis vis-edit VA005-icons-font" title="' + VIS.Msg.getMsg("Edit") + '"></span><span class="VA005-icons vis vis-refresh VA005-icons-font" title="'
                + VIS.Msg.getMsg("VA005_Refresh") + '"></span><span class="VA005-icons vis vis-print VA005-icons-font" title="' + VIS.Msg.getMsg("Print") + '"></span></div></div>' +
                //<input class="vis-group-add-btn vis-group-pointer vis-group-addLeft vis-group-ass-btns" type="button"></div></div>' +
                '<div id="VA005_divNewCart_' + $self.windowNo + '" class="VA005-conv-data"><div class="VA005-conversion-data"><input id="VA005_scanName_' + $self.windowNo + '"></div>' +
                '<div><span id="VA005_SaveScanName_' + $self.windowNo + '" class="VA005-icons vis vis-save VA005-icons-font" tabindex="0" title="' + VIS.Msg.getMsg("Save") + '">' +
                '</span><span id="VA005_btnCancelScan_' + $self.windowNo + '" class="VA005-icons vis vis-mark VA005-icons-font" tabindex="0" title="' + VIS.Msg.getMsg("Cancel") + '"></span><span class="VA005-cart-update" style="display:none;"></span></div></div></div>');
            $divCartdata = $('<div class="VA005-uom-list">');
            $divCartdata.append($divCart);
            $rightPanel.append($divCartdata);
            divCart = $rightPanel.find("#VA005_divCart_" + $self.windowNo);
            divCartList = $rightPanel.find("#VA005_divCartList_" + $self.windowNo);
            divNewCart = $rightPanel.find("#VA005_divNewCart_" + $self.windowNo);
            divCart.hide();
            divNewCart.hide();
            $divCartdata.hide();
            btnNewCart = divCart.find(".vis-plus");
            btnEditCart = divCart.find(".vis-edit");
            btnRefreshCart = divCart.find(".vis-refresh");
            btnPrint = divCart.find(".vis-print");
            cmbCart = divCart.find("#VA005_cmbCart_" + $self.windowNo);
            txtScan = $rightPanel.find("#VA005_scanName_" + $self.windowNo);
            btnSaveScan = $rightPanel.find("#VA005_SaveScanName_" + $self.windowNo);
            btnCancelScan = $rightPanel.find("#VA005_btnCancelScan_" + $self.windowNo);
            $rightPanel.find('.VA005-cart-update').text(VIS.Msg.getMsg("Updated"));
            $divProdDetail = $('<div class="VA005-uom-list">');
            $divProductDet = $('<div class="VA005-right-head" >'
                + '<div class="VA005-form-top-fields VA005-prodmgtrightfieldtop">'
                + '<div id="VA005_ProdDetZoomName_' + $self.windowNo + '" style= "display:none"><h4 id="VA005_prodName_' + $self.windowNo
                + '" style="flex: 1"></h4><span id="VA005_ZoomProduct" title=' + VIS.Msg.getMsg("VA005_ZoomToProduct")
                + ' class="VA005-icons VA005-icons-font vis vis-edit" style="margin-top:5px"></span></div>'
                + '<div style="float:left; width:100%" class="VA005-data-wrap" id="VA005_UPC_' + $self.windowNo + '">' // UPC Numbers to be shown in this DIV
                + '<p>' + VIS.Msg.getElement(VIS.Env.getCtx(), "UPC") + '</p>'
                + '</div>'
                + '<div style="float:left;" class="VA005-data-wrap" id="VA005_AttributeSet_' + $self.windowNo + '">'
                + '<p>' + VIS.Msg.getElement(VIS.Env.getCtx(), "M_AttributeSet_ID") + '</p>'
                + '</div></div><!-- end of form-top-fields -->'
                + '<div class="VA005-image-wrap VA005-prodmgtimgwrap"></div></div>');
            $rightPanel.append($divProdDetail);
            $divProdDetail.append($divProductDet);
            divUsrImage = $divProductDet.find(".VA005-image-wrap");
            imgUsrImage = $('<img style="max-height: 100%; max-width: 100%;" src=' + VIS.Application.contextUrl + 'Areas/VA005/Images/img-defult.png>');
            divUsrImage.append($('<i style="max-height: 100%; max-width: 100%;" class="vis vis-image" ></i>'));
            $ProductDetails = $('<div class="VA005-form-fullFields">'
                + '<div id="VA005_ProductType_' + $self.windowNo + '" class="VA005-data-wrap"><p>' + VIS.Msg.getElement(VIS.Env.getCtx(), "ProductType") + '</p></div>'
                + '<div id="VA005_ProdCategory_' + $self.windowNo + '" class="VA005-data-wrap"><p>' + VIS.Msg.getElement(VIS.Env.getCtx(), "M_Product_Category_ID") + '</p></div>'
                + '<div id="VA005_prodTaxCat_' + $self.windowNo + '" class="VA005-data-wrap"><p>' + VIS.Msg.getElement(VIS.Env.getCtx(), "C_TaxCategory_ID") + '</p></div>'
                + '<div id="VA005_prodUOM_' + $self.windowNo + '" class="VA005-data-wrap"><p>' + VIS.Msg.getElement(VIS.Env.getCtx(), "C_UOM_ID") + '</p></div>'
                + '<div id="VA005_ProdParent_' + $self.windowNo + '" class="VA005-data-wrap"><p>' + VIS.Msg.getElement(VIS.Env.getCtx(), "IsSummary") + '</p></div>'
                + '</div><!-- end of form-fullFields -->');

            $divProdDetail.append($ProductDetails);
            divZoomProdName = $("#VA005_ProdDetZoomName_" + $self.windowNo);
            prodName = $divProductDet.find("#VA005_prodName_" + $self.windowNo);
            prodUPC = $divProductDet.find("#VA005_UPC_" + $self.windowNo);
            prodAttributeSet = $divProductDet.find("#VA005_AttributeSet_" + $self.windowNo);
            prodType = $ProductDetails.find("#VA005_ProductType_" + $self.windowNo);
            prodCategory = $ProductDetails.find("#VA005_ProdCategory_" + $self.windowNo);
            prodTaxCat = $ProductDetails.find("#VA005_prodTaxCat_" + $self.windowNo);
            prodUOM = $ProductDetails.find("#VA005_prodUOM_" + $self.windowNo);
            prodParent = $ProductDetails.find("#VA005_ProdParent_" + $self.windowNo);
            btnEditDetail = $divProductDet.find(".vis-edit");

            $divRelated = $('<div class="VA005-uom-list">');
            $rightPanel.append($divRelated);
            $divRelated.hide();
            $rightPanel.append("<div class='VA005-popup'>" +
                "<input type='text' maxlength=40 id='VA005_upcTxt_" + $self.windowNo + "'></input><span id='VA005_upcOK_" + $self.windowNo +
                "' class='vis vis-markx' title='" + VIS.Msg.getMsg("Save") + "'></span><span id='VA005_btnCancelUpc_" + $self.windowNo +
                "' class='vis vis-cross' title='" + VIS.Msg.getMsg("Cancel") + "'></span></div>");
            pop = $rightPanel.find('.VA005-popup');
            txtEditUPC = $rightPanel.find("#VA005_upcTxt_" + $self.windowNo);

            $rightPanel.find("#VA005_upcOK_" + $self.windowNo).on("click", function (e) {
                if (txtEditUPC.val() != "") {
                    editVariantUPC();
                }
                else {
                    return;
                }
            });

            var specialKeys = new Array();
            specialKeys.push(8); //Backspace
            specialKeys.push(9); //Tab
            specialKeys.push(46); //Delete
            specialKeys.push(36); //Home
            specialKeys.push(35); //End
            specialKeys.push(37); //Left
            specialKeys.push(39); //Right
            specialKeys.push(13); //Enter

            txtEditUPC.on("keydown", function (e) {

                if ((e.keyCode >= 48 && e.keyCode <= 57) || (e.keyCode >= 65 && e.keyCode <= 90) || (e.keyCode >= 97 && e.keyCode <= 105) || (specialKeys.indexOf(e.keyCode) != -1 && e.charCode != e.keyCode)) {
                    if (e.keyCode == 13) {
                        if (txtEditUPC.val() != "") {
                            editVariantUPC();
                        }
                        else {
                            return false;
                        }
                    }
                }
                else {
                    return false;
                }
            });

            $rightPanel.find("#VA005_btnCancelUpc_" + $self.windowNo).on("click", function (e) {
                pop.popup('hide');
                txtEditUPC.val("");
                patr_ID = 0;
            });
        };

        function AddProductPanel() {
            $maindiv.append($div);
            $div.append('<div class="VA005-product-data"><div class="input-group vis-input-wrap"><div class="vis-control-wrap"><input id="txtValue_' + $self.windowNo + '" type="text" placeholder=" " data-placeholder=""><label>' + VIS.Msg.getElement(VIS.Env.getCtx(), "Value") + '</label></div></div></div>' +
                '<div class="VA005-product-data"><div class="input-group vis-input-wrap"><div class="vis-control-wrap"><select id="VA005_cmbOrg_' + $self.windowNo + '"></select><label>' + VIS.Msg.getElement(VIS.Env.getCtx(), "AD_Org_ID") + '</label></div></div></div>' +

                '<div class="VA005-product-data"><div class="input-group vis-input-wrap"><div class="vis-control-wrap"><input class="vis-ev-col-mandatory" id="txtName_' + $self.windowNo + '" type="text" placeholder=" " data-placeholder="" ><label>' + VIS.Msg.getElement(VIS.Env.getCtx(), "Name") + '</label></div></div></div>' +

                '<div class="VA005-product-data"><div class="input-group vis-input-wrap"><div class="vis-control-wrap"><select class="vis-ev-col-mandatory" id="cmbCat_' + $self.windowNo + '" placeholder=" " data-placeholder="" data-hasbtn=" "></select><label>' + VIS.Msg.getElement(VIS.Env.getCtx(), "M_Product_Category_ID") +
                '</label></div><div class="input-group-append"><button  id="btnProdCat_' + $self.windowNo + '" class="input-group-text VA005-leftpanelcatsearchbtn"><i class="fa fa-ellipsis-v"></i></button></div></div></div>' +

                '<div class="VA005-product-data"><div class="input-group vis-input-wrap"><div class="vis-control-wrap"><select id="cmbType_' + $self.windowNo + '" placeholder=" " data-placeholder="" data-hasbtn=" " ></select><label>' + VIS.Msg.getElement(VIS.Env.getCtx(), "ProductType") + '</label></div></div></div>' +

                '<div class="VA005-product-data"><div class="input-group vis-input-wrap"><div class="vis-control-wrap"><select class="vis-ev-col-mandatory" id="cmbTax_' + $self.windowNo + '" placeholder=" " data-placeholder="" data-hasbtn=" "></select><label>' + VIS.Msg.getElement(VIS.Env.getCtx(), "C_TaxCategory_ID") +
                '</label></div><div class="input-group-append"><button id="btnTaxCategory_' + $self.windowNo + '" class="input-group-text VA005-leftpanelcatsearchbtn"><i class="fa fa-ellipsis-v"></i></button></div></div></div>' +

                '<div class="VA005-product-data"><div class="input-group vis-input-wrap"><div class="vis-control-wrap"><select id="cmbAttribute_' + $self.windowNo + '" placeholder=" " data-placeholder="" data-hasbtn=" "></select><label>' + VIS.Msg.getElement(VIS.Env.getCtx(), "M_AttributeSet_ID") +
                '</label></div><div class="input-group-append"><button id="btnAttributeSet_' + $self.windowNo + '" class="input-group-text VA005-leftpanelcatsearchbtn"><i class="fa fa-ellipsis-v"></i></button></div></div></div>' +

                '<div class="VA005-product-data"><div class="input-group vis-input-wrap"><div class="vis-control-wrap"><select class="vis-ev-col-mandatory" id="cmbUom_' + $self.windowNo + '" placeholder=" " data-placeholder="" data-hasbtn=" " ></select><label>' + VIS.Msg.getElement(VIS.Env.getCtx(), "C_UOM_ID") +
                '</label></div><div class="input-group-append"><button id="btnUom_' + $self.windowNo + '" class="input-group-text VA005-leftpanelcatsearchbtn"><i class="fa fa-ellipsis-v"></i></button></div></div></div>' +

                '<div class="VA005-product-data"><div class="input-group vis-input-wrap"><div class="vis-control-wrap"><input id="txtUpc_' + $self.windowNo + '" type="text" placeholder=" " data-placeholder=""><label>' + VIS.Msg.getElement(VIS.Env.getCtx(), "UPC") + '</label></div></div></div>');

            cmbOrg = $maindiv.find("#VA005_cmbOrg_" + $self.windowNo);
            txtValue = $maindiv.find("#txtValue_" + $self.windowNo);
            txtName = $maindiv.find("#txtName_" + $self.windowNo);
            cmbCat = $maindiv.find("#cmbCat_" + $self.windowNo);
            cmbTaxCategory = $maindiv.find("#cmbTax_" + $self.windowNo);
            cmbProductType = $maindiv.find("#cmbType_" + $self.windowNo);
            cmbUOM = $maindiv.find("#cmbUom_" + $self.windowNo);
            cmbAttributeSet = $maindiv.find("#cmbAttribute_" + $self.windowNo);
            txtUpc = $maindiv.find("#txtUpc_" + $self.windowNo);
            btnAttributeSet = $maindiv.find("#btnAttributeSet_" + $self.windowNo);
            btnProdCat = $maindiv.find("#btnProdCat_" + $self.windowNo);
            btnTaxCategory = $maindiv.find("#btnTaxCategory_" + $self.windowNo);
            btnUomZoom = $maindiv.find("#btnUom_" + $self.windowNo);
            LoadOrganization();
            LoadProductTypes();
            LoadAttributes();
            LoadTaxCategories();
            LoadProductCategories();
            LoadUOM(cmbUOM);
            //Mohit
            cmbCat.on("change", function () {
                LoadOnCategorySelect(cmbCat.val());
            });
        };

        function PriceUpdatePanel() {
            $divPrice.append($divPriceMain);
            $divPriceMain.append('<div style=" float: left; width: 100%; "><div class="VA005-form-data VA005-popup-head"><div class="input-group vis-input-wrap"><div class="vis-control-wrap"><select id="cmbPricelist_'
                + $self.windowNo + '"></select><label id="lblPriceList_"' + $self.windowNo + '">' + VIS.Msg.getMsg("PriceListVersion") + '</label></div></div></div><div style = "float:right; margin-top:10px">' +
                //<div class="VA005-form-data VA005-popup-head" style="margin-left:20px;"><label>' + VIS.Msg.getElement(VIS.Env.getCtx(), "C_Currency_ID") +'</label>
                '<label id="CurrInfo_' + $self.windowNo + '" style="float:right;"></label></div></div><div id="VA005_priceGrid_' + $self.windowNo + '" class="VA005-popup-data">');
            cmbPriceList = $divPriceMain.find("#cmbPricelist_" + $self.windowNo);
            priceGrid = $divPriceMain.find("#VA005_priceGrid_" + $self.windowNo);
            currSpan = $divPriceMain.find("#CurrInfo_" + $self.windowNo);
            dGrid = null;
            dGrid = priceGrid.w2grid({
                name: 'gridprice_' + $self.windowNo,
                recordHeight: 25,
                columns: [
                    { field: "product_ID", caption: "", sortable: false, size: '80px', display: false },
                    {
                        field: "ImageUrl", caption: '<div class="imgPrice" style="text-align: center;" ><span>' + VIS.Msg.translate(VIS.Env.getCtx(), "AD_Image_ID") + '</span></div>', sortable: false, size: '80px', hidden: false,
                        render: function () {
                            return '<div style="text-align: center;"></div>';
                        }
                    },
                    { field: "Product", caption: '<div ><span>' + VIS.Msg.translate(VIS.Env.getCtx(), "Product") + '</span></div>', sortable: false, size: '200px', hidden: false },
                    {
                        field: "PriceList", caption: '<div ><span>' + VIS.Msg.getElement(VIS.Env.getCtx(), "PriceList") + '</span></div>', sortable: false, size: '100px', min: 80, hidden: false, style: 'text-align: right', editable: { type: 'number' },
                        // Added by shifali on 29th July 2020 to get PriceList value acc. to culture
                        render: function (record, index, col_index) {
                            var val = record["PriceList"];
                            val = checkcommaordot(event, val);
                            return parseFloat(val).toLocaleString();
                        }
                    },
                    {
                        field: "PriceStd", caption: '<div ><span>' + VIS.Msg.getElement(VIS.Env.getCtx(), "PriceStd") + '</span></div>', sortable: false, size: '100px', min: 80, hidden: false, style: 'text-align: right', editable: { type: 'number' },
                        // Added by shifali on 29th July 2020 to get PriceStd value acc. to culture
                        render: function (record, index, col_index) {
                            var val = record["PriceStd"];
                            val = checkcommaordot(event, val);
                            return parseFloat(val).toLocaleString();
                        }
                    },
                    {
                        field: "PriceLimit", caption: '<div ><span>' + VIS.Msg.getElement(VIS.Env.getCtx(), "PriceLimit") + '</span></div>', sortable: false, size: '100px', min: 80, hidden: false, style: 'text-align: right', editable: { type: 'number' },
                        // Added by shifali on 29th July 2020 to get PriceLimit value acc. to culture
                        render: function (record, index, col_index) {
                            var val = record["PriceLimit"];
                            val = checkcommaordot(event, val);
                            return parseFloat(val).toLocaleString();
                        }
                    },
                    { field: "UOM", caption: "", sortable: false, size: '80px', display: false },
                    {
                        field: "C_Uom_ID", caption: '<div ><span>' + VIS.Msg.getElement(VIS.Env.getCtx(), "C_UOM_ID") + '</span></div>', sortable: false, size: '80px', min: 80, hidden: false, editable: { type: 'select', items: uomArray, showAll: true },
                        render: function (record, index, col_index) {
                            var html = '';
                            for (var p in uomArray) {
                                if (uomArray[p].id == this.getCellValue(index, col_index)) html = uomArray[p].text;
                            }
                            return html;
                        }
                    },
                    { field: "Lot", caption: '<div ><span>' + VIS.Msg.getElement(VIS.Env.getCtx(), "Lot") + '</span></div>', sortable: false, size: '80px', min: 80, hidden: false, editable: { type: 'text' } },
                    { field: "attribute_ID", caption: "", sortable: false, size: '80px', display: false },
                    {
                        field: "Attribute", caption: '<div ><span>' + VIS.Msg.translate(VIS.Env.getCtx(), "Attribute") + '</span></div>', sortable: false, size: '150px', hidden: false,
                        render: function () {
                            return '<div><input type=text readonly="readonly" style= "width:85%; border:none" ><i class="fa fa-list-alt VA005-gridicon" title="Attribute Set Instance" style="opacity: 1;"></i></div>';
                        }
                    },
                    { field: "updated", caption: "", sortable: false, size: '80px', display: false }
                    //{
                    //    field: "Delete", caption: '<div style="text-align: center;" ><span>' + VIS.Msg.translate(VIS.Env.getCtx(), "Delete") + '</span></div>', sortable: false, size: '40px', min: 80, hidden: false,
                    //    render: function () { return '<div style="text-align: center;"><img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/delete-ico-hover.png" alt="Delete record" title="Delete record" style="opacity: 1;"></div>'; }
                    //}
                ],
                // Added by shifali on 29th July 2020 to get value acc. to culture while editing grid column
                onEditField: function (event) {
                    id = event.recid;
                    if (event.column == 3 || event.column == 4 || event.column == 5) {
                        dGrid.records[event.index][dGrid.columns[event.column].field] = checkcommaordot(event, dGrid.records[event.index][dGrid.columns[event.column].field]);
                        var _value = format.GetFormatAmount(dGrid.records[event.index][dGrid.columns[event.column].field], "init", dotFormatter);
                        dGrid.records[event.index][dGrid.columns[event.column].field] = format.GetConvertedString(_value, dotFormatter);
                        $("#grid_gridprice_" + $self.windowNo + "_rec_" + id).keydown(function (event) {
                            if (!dotFormatter && (event.keyCode == 190 || event.keyCode == 110)) {// , separator
                                return false;
                            }
                            else if (dotFormatter && event.keyCode == 188) { // . separator
                                return false;
                            }
                            if (event.target.value.contains(".") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                                if (event.target.value.indexOf('.') > -1) {
                                    event.target.value = event.target.value.replace('.', '');
                                }
                            }
                            else if (event.target.value.contains(",") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                                if (event.target.value.indexOf(',') > -1) {
                                    event.target.value = event.target.value.replace(',', '');
                                }
                            }
                            if (event.keyCode != 8 && event.keyCode != 9 && (event.keyCode < 37 || event.keyCode > 40) &&
                                (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)
                                && event.keyCode != 109 && event.keyCode != 189 && event.keyCode != 110
                                && event.keyCode != 144 && event.keyCode != 188 && event.keyCode != 190) {
                                return false;
                            }
                        });
                    }
                },

                onChange: function (event) {

                    dGrid.records[event.index]["updated"] = true;
                    if (event.column == 3) {
                        // Added by Shifali on 29th July 2020 to get value acc. to culture
                        var _val = format.GetConvertedNumber(event.value_new, dotFormatter);
                        dGrid.records[event.index]["PriceList"] = _val.toFixed(precision);

                    }
                    else if (event.column == 4) {
                        var _val = format.GetConvertedNumber(event.value_new, dotFormatter);
                        dGrid.records[event.index]["PriceStd"] = _val.toFixed(precision);
                    }
                    else if (event.column == 5) {
                        var _val = format.GetConvertedNumber(event.value_new, dotFormatter);
                        dGrid.records[event.index]["PriceLimit"] = _val.toFixed(precision);
                    }
                    else if (event.column == 7) {
                        dGrid.records[event.index]["C_Uom_ID"] = event.value_new;
                    }
                    else if (event.column == 8) {
                        dGrid.records[event.index]["Lot"] = event.value_new;
                    }
                },

                onClick: function (event) {
                    //if (event.column == 10 && dGrid.records.length > 0) {
                    //    dGrid.remove(event.recid);
                    //    for (var k = 0; k < dGrid.records.length; k++) {
                    //        $("#grid_gridprice_" + $self.windowNo + "_rec_" + dGrid.records[k].recid).find("input[type=text]").val(dGrid.records[k]["Attribute"]);
                    //    }
                    //}
                    //else
                    if (event.column == 1 && dGrid.records.length > 0) {
                        //$(".VA005-background").css({ "opacity": "0.7" }).fadeIn("slow");

                        // Done by Bharat on 28 Feb 2018 to move queries to server side
                        var img = VIS.dataContext.getJSONRecord("VA005/ProductManagement/GetImageUrl", dGrid.records[event.recid - 1]["product_ID"].toString());

                        //sqlaa = "SELECT img.ImageUrl FROM M_Product prd LEFT OUTER JOIN AD_Image img ON prd.AD_Image_ID = img.AD_Image_ID WHERE prd.M_Product_ID = " + dGrid.records[event.recid - 1]["product_ID"];
                        //var img = VIS.Utility.Util.getValueOfString(VIS.DB.executeScalar(sqlaa));

                        var src = "";
                        if (img != null) {
                            if (img != "") {
                                img = img.substring(img.lastIndexOf("/") + 1, img.length);
                                var d = new Date();
                                src = '<img style="max-height: 100%; max-width: 100%;" src="' + VIS.Application.contextUrl + "Images/Thumb140x120/" + img + "?" + d.getTime() + '">';
                            }
                            else {
                                src = '<i class= "vis vis-image" ></i>';
                            }
                        }
                        else {
                            src = '<i class= "vis vis-image" ></i>';
                        }

                        $ImageZoom.empty();
                        $ImageZoom.append(src);
                        $ImageZoom.fadeIn("slow")

                        $ImageZoom.popup({
                            tooltipanchor: $divPrice,
                            autoopen: true,
                            type: 'tooltip',
                            background: true,
                            backgroundactive: false,
                            horizontal: 'center'
                            //offsetleft: -43,
                            //offsettop: -10
                        });
                    }
                    else if (event.column == 10 && dGrid.records.length > 0) {
                        // Done by Bharat on 28 Feb 2018 to move queries to server side
                        var mattsetid = VIS.dataContext.getJSONRecord("VA005/ProductManagement/GetAttributeSet", VIS.Utility.Util.getValueOfInt(dGrid.records[event.recid - 1]["product_ID"]));

                        //var qry = "SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + VIS.Utility.Util.getValueOfInt(dGrid.records[event.recid - 1]["product_ID"]);
                        //var mattsetid = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));

                        if (mattsetid != 0) {
                            var productWindow = AD_Column_ID == 8418;		//	HARDCODED
                            var M_Locator_ID = VIS.context.getContextAsInt($self.windowNo, "M_Locator_ID");
                            var C_BPartner_ID = VIS.context.getContextAsInt($self.windowNo, "C_BPartner_ID");
                            var obj = new VIS.PAttributesForm(VIS.Utility.Util.getValueOfInt(dGrid.records[event.recid - 1]["attribute_ID"]), VIS.Utility.Util.getValueOfInt(dGrid.records[event.recid - 1]["product_ID"]), M_Locator_ID, C_BPartner_ID, productWindow, AD_Column_ID, $self.windowNo);
                            if (obj.hasAttribute) {
                                obj.showDialog();
                            }
                            obj.onClose = function (mAttributeSetInstanceId, name, mLocatorId) {
                                if (dGrid.records[event.recid - 1]["attribute_ID"] != mAttributeSetInstanceId) {
                                    dGrid.records[event.recid - 1]["attribute_ID"] = mAttributeSetInstanceId;
                                    dGrid.records[event.recid - 1]["Attribute"] = name;
                                    $("#grid_gridprice_" + $self.windowNo + "_rec_" + event.recid).find("input[type=text]").val(name);
                                    dGrid.records[event.recid - 1]["updated"] = true;
                                }
                            };
                        }
                        else {
                            return;
                        }
                    }
                }
            });
            dGrid.hideColumn('UOM');
            dGrid.hideColumn('attribute_ID');
            dGrid.hideColumn('product_ID');
            dGrid.hideColumn('updated');
            //LoadPriceList();
        };

        // function to check comma or dot from given value and return new value
        function checkcommaordot(event, val) {
            var foundComma = false;
            event.value_new = VIS.Utility.Util.getValueOfString(val);
            if (event.value_new.contains(".")) {
                foundComma = true;
                var indices = [];
                for (var i = 0; i < event.value_new.length; i++) {
                    if (event.value_new[i] === ".")
                        indices.push(i);
                }
                if (indices.length > 1) {
                    event.value_new = removeAllButLast(event.value_new, '.');
                }
            }
            if (event.value_new.contains(",")) {
                if (foundComma) {
                    event.value_new = removeAllButLast(event.value_new, ',');
                }
                else {
                    var indices = [];
                    for (var i = 0; i < event.value_new.length; i++) {
                        if (event.value_new[i] === ",")
                            indices.push(i);
                    }
                    if (indices.length > 1) {
                        event.value_new = removeAllButLast(event.value_new, ',');
                    }
                    else {
                        event.value_new = event.value_new.replace(",", ".");
                    }
                }
            }
            if (event.value_new == "") {
                event.value_new = "0";
            }
            return event.value_new;
        };

        // Remove all seperator but only bring last seperator
        function removeAllButLast(amt, seprator) {
            var parts = amt.split(seprator);
            amt = parts.slice(0, -1).join('') + '.' + parts.slice(-1);
            if (amt.indexOf('.') == (amt.length - 1)) {
                amt = amt.replace(".", "");
            }
            return amt;
        };

        function SupplierPanel() {
            $divSupp.append($divSupplier);
            $divSupplier.append('<div class="VA005-form-data VA005-popup-head"><div class="input-group vis-input-wrap"><div class="vis-control-wrap"><select id="cmbSupplier_'
                + $self.windowNo + '"></select><label id="lblPriceList_"' + $self.windowNo + '">' + VIS.Msg.getMsg("VA005_Supplier") + '</label></div></div></div><div id="VA005_supplierGrid_' + $self.windowNo + '" class="VA005-popup-data">');
            cmbSupplier = $divSupplier.find("#cmbSupplier_" + $self.windowNo);
            supplierGrid = $divSupplier.find("#VA005_supplierGrid_" + $self.windowNo);
            sGrid = null;
            sGrid = supplierGrid.w2grid({
                name: 'gridsupplier_' + $self.windowNo,
                recordHeight: 25,
                show: {
                    //toolbar: true,  // indicates if toolbar is v isible
                    //columnHeaders: true,   // indicates if columns is visible
                    //lineNumbers: true,  // indicates if line numbers column is visible
                    //selectColumn: true,  // indicates if select column is visible
                    //toolbarReload: false,   // indicates if toolbar reload button is visible
                    //toolbarColumns: true,   // indicates if toolbar columns button is visible
                    //toolbarSearch: false,   // indicates if toolbar search controls are visible
                    //toolbarAdd: false,   // indicates if toolbar add new button is visible
                    //toolbarDelete: false,   // indicates if toolbar delete button is visible
                    //toolbarSave: false,   // indicates if toolbar save button is visible
                    //selectionBorder: false,	 // display border arround selection (for selectType = 'cell')
                    //recordTitles: false	 // indicates if to define titles for records
                },
                columns: [
                    { field: "product_ID", caption: "", sortable: false, size: '80px', display: false },
                    { field: "Product", caption: '<div ><span>' + VIS.Msg.translate(VIS.Env.getCtx(), "Product") + '</span></div>', sortable: false, size: '200px', hidden: false },
                    {
                        field: "OrderMin", caption: '<div ><span>' + VIS.Msg.getElement(VIS.Env.getCtx(), "Order_Min") + '</span></div>', sortable: false, size: '120px', min: 80, hidden: false, style: 'text-align: right', editable: { type: 'number' },
                        // Added by shifali on 29th July 2020 to get ordermin value acc. to culture
                        render: function (record, index, col_index) {
                            var val = record["OrderMin"];
                            val = checkcommaordot(event, val);
                            return parseFloat(val).toLocaleString();
                        }
                    },
                    {
                        field: "OrderPack", caption: '<div ><span>' + VIS.Msg.getElement(VIS.Env.getCtx(), "Order_Pack") + '</span></div>', sortable: false, size: '100px', min: 80, hidden: false, style: 'text-align: right', editable: { type: 'number' },
                        // Added by shifali on 29th July 2020 to get orderpack value acc. to culture
                        render: function (record, index, col_index) {
                            var val = record["OrderPack"];
                            val = checkcommaordot(event, val);
                            return parseFloat(val).toLocaleString();
                        }
                    },
                    {
                        field: "C_Uom_ID", caption: '<div ><span>' + VIS.Msg.getElement(VIS.Env.getCtx(), "C_UOM_ID") + '</span></div>', sortable: false, size: '80px', min: 80, hidden: false, editable: { type: 'select', items: uomArray, showAll: true },
                        render: function (record, index, col_index) {
                            var html = '';
                            for (var p in uomArray) {
                                if (uomArray[p].id == this.getCellValue(index, col_index)) html = uomArray[p].text;
                            }
                            return html;
                        }
                    },
                    {
                        field: "C_Currency_ID", caption: '<div ><span>' + VIS.Msg.getElement(VIS.Env.getCtx(), "C_Currency_ID") + '</span></div>', sortable: false, size: '80px', min: 80, hidden: false, editable: { type: 'select', items: curArray, showAll: true },
                        render: function (record, index, col_index) {
                            var html = '';
                            for (var p in curArray) {
                                if (curArray[p].id == this.getCellValue(index, col_index)) html = curArray[p].text;
                            }
                            return html;
                        }
                    },
                    {
                        field: "PriceList", caption: '<div ><span>' + VIS.Msg.getElement(VIS.Env.getCtx(), "PriceList") + '</span></div>', sortable: false, size: '80px', min: 80, hidden: false, style: 'text-align: right', editable: { type: 'number' },
                        // Added by Shifali to change the pricelist acc. to culture
                        //render: function (record, index, col_index) {
                        //    var val = VIS.Utility.Util.getValueOfDecimal(record["PriceList"].toFixed(precision));
                        //    return (val).toLocaleString();
                        //}
                        render: function (record, index, col_index) {
                            var val = record["PriceList"];
                            val = checkcommaordot(event, val);
                            return parseFloat(val).toLocaleString();
                        }
                    },
                    { field: "DeliveryTime", caption: '<div ><span>' + VIS.Msg.getElement(VIS.Env.getCtx(), "DeliveryTime_Promised") + '</span></div>', sortable: false, size: '100px', min: 80, hidden: false, editable: { type: 'int' } },
                    { field: "updated", caption: "", sortable: false, size: '80px', display: false }
                ],
                // Added by shifali on 29th July 2020 to get value acc. to culture while editing grid column
                onEditField: function (event) {
                    id = event.recid;
                    if (event.column == 2 || event.column == 3 || event.column == 6) {
                        sGrid.records[event.index][sGrid.columns[event.column].field] = checkcommaordot(event, sGrid.records[event.index][sGrid.columns[event.column].field]);
                        var _value = format.GetFormatAmount(sGrid.records[event.index][sGrid.columns[event.column].field], "init", dotFormatter);
                        sGrid.records[event.index][sGrid.columns[event.column].field] = format.GetConvertedString(_value, dotFormatter);
                        $("#grid_gridsupplier_" + $self.windowNo + "_rec_" + id).keydown(function (event) {
                            if (!dotFormatter && (event.keyCode == 190 || event.keyCode == 110)) {// , separator
                                return false;
                            }
                            else if (dotFormatter && event.keyCode == 188) { // . separator
                                return false;
                            }
                            if (event.target.value.contains(".") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                                if (event.target.value.indexOf('.') > -1) {
                                    event.target.value = event.target.value.replace('.', '');
                                }
                            }
                            if (event.target.value.contains(",") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                                if (event.target.value.indexOf(',') > -1) {
                                    event.target.value = event.target.value.replace('', '');
                                }
                            }
                            if (event.keyCode != 8 && event.keyCode != 9 && (event.keyCode < 37 || event.keyCode > 40) &&
                                (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)
                                && event.keyCode != 109 && event.keyCode != 189 && event.keyCode != 110
                                && event.keyCode != 144 && event.keyCode != 188 && event.keyCode != 190) {
                                return false;
                            }
                        });
                    }
                },

                onChange: function (event) {

                    sGrid.records[event.index]["updated"] = true;
                    if (event.column == 2) {
                        var _val = format.GetConvertedNumber(event.value_new, dotFormatter);
                        sGrid.records[event.index]["OrderMin"] = _val.toFixed(precision);
                    }
                    else if (event.column == 3) {
                        var _val = format.GetConvertedNumber(event.value_new, dotFormatter);
                        sGrid.records[event.index]["OrderPack"] = _val.toFixed(precision);
                    }
                    else if (event.column == 4) {
                        sGrid.records[event.index]["C_Uom_ID"] = event.value_new;
                    }
                    else if (event.column == 5) {
                        sGrid.records[event.index]["C_Currency_ID"] = event.value_new;
                    }
                    else if (event.column == 6) {
                        var _val = format.GetConvertedNumber(event.value_new, dotFormatter);
                        sGrid.records[event.index]["PriceList"] = _val.toFixed(precision);
                    }
                    else if (event.column == 7) {
                        sGrid.records[event.index]["DeliveryTime"] = event.value_new;
                    }
                },
                onClick: function (event) {
                }
            });
            sGrid.hideColumn('product_ID');
            sGrid.hideColumn('updated');
            sGrid.hideColumn('DeliveryTime');
            //LoadSupplier();
        };

        function PrintPanel() {
            $divPrint.append($divPrintPop);
            $divPrintPop.append('<div class="VA005-form-data input-group vis-input-wrap" style="margin-top: 5px;"><div class="vis-control-wrap"><input class="vis-ev-col-mandatory" id="txtHeader_'
                + $self.windowNo + '" type="text" maxlength="40"><label>' + VIS.Msg.getMsg("VA005_StickerHeader") + '</label></div></div><div class="VA005-form-data input-group vis-input-wrap"><div class="vis-control-wrap"><select class="vis-ev-col-mandatory" id="cmbVarsion_' + $self.windowNo + '"></select><label>' + VIS.Msg.getElement(VIS.Env.getCtx(), "M_PriceList_Version_ID") +
                '</label></div></div>');
            txtHeader = $divPrint.find("#txtHeader_" + $self.windowNo);
            cmbVersion = $divPrint.find("#cmbVarsion_" + $self.windowNo);
        };

        // Create Related Panel Grid at Right
        function gridRelatedPanel() {
            $divRelated.append($divRelatedGrid);
            dRelatedGrid = null;
            dRelatedGrid = $divRelatedGrid.w2grid({
                name: 'VA005_gridRelated_' + $self.windowNo,
                recordHeight: 25,
                show: {
                    toolbar: true,  // indicates if toolbar is v isible
                    //columnHeaders: true,   // indicates if columns is visible
                    //lineNumbers: true,  // indicates if line numbers column is visible
                    selectColumn: true,  // indicates if select column is visible
                    toolbarReload: false,   // indicates if toolbar reload button is visible
                    toolbarColumns: true,   // indicates if toolbar columns button is visible
                    toolbarSearch: false,   // indicates if toolbar search controls are visible
                    toolbarAdd: false,   // indicates if toolbar add new button is visible
                    toolbarDelete: true,   // indicates if toolbar delete button is visible
                    toolbarSave: true,   // indicates if toolbar save button is visible
                    //selectionBorder: false,	 // display border arround selection (for selectType = 'cell')
                    //recordTitles: false	 // indicates if to define titles for records
                },

                columns: [
                    { field: "Product", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "Name"), sortable: false, size: '36%', editable: { type: 'text' } },
                    { field: "RelatedProduct", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "RelatedProduct_ID"), sortable: false, size: '36%' },
                    //{ field: "RelatedType", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "RelatedProductType"), sortable: false, size: '32%' },
                    {
                        field: "RelatedType", caption: '<div ><span>' + VIS.Msg.getElement(VIS.Env.getCtx(), "RelatedProductType") + '</span></div>', sortable: false, size: '32%', hidden: false, editable: { type: 'select', items: relatedType, showAll: true },
                        render: function (record, index, col_index) {
                            var html = '';
                            for (var p in relatedType) {
                                if (relatedType[p].id == this.getCellValue(index, col_index)) html = relatedType[p].text;
                            }
                            return html;
                        }
                    },
                    //{ field: "QtyOnHand", caption: '<div style="text-align: center;" ><span>' + VIS.Msg.translate(VIS.Env.getCtx(), "QtyOnHand") + '</span></div>', sortable: false, size: '16%', hidden: false, render: 'number:1' },
                    //{ field: "UOM", caption: VIS.Msg.translate(VIS.Env.getCtx(), "C_UOM_ID"), sortable: false, size: '16%' },
                    //{ field: "Reserved", caption: '<div style="text-align: center;" ><span>' + VIS.Msg.getMsg("VA005_Reserved") + '</span></div>', sortable: false, size: '16%', hidden: false, render: 'number:1' },
                    //{ field: "ATP", caption: '<div style="text-align: center;" ><span>' + VIS.Msg.getMsg("VA005_ATP") + '</span></div>', sortable: false, size: '16%', hidden: false, render: 'number:1' },
                    { field: "M_Product_ID", caption: "M_Product_ID", sortable: false, size: '80px', display: false },
                    { field: "updated", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "Updated"), sortable: false, size: '80px', display: false }
                ],
                records: [],
                onChange: function (event) {
                    dRelatedGrid.records[event.index]["updated"] = true;
                    if (event.column == 0) {
                        dRelatedGrid.records[event.index]["Product"] = event.value_new;
                    }
                    else if (event.column == 2) {
                        dRelatedGrid.records[event.index]["RelatedType"] = event.value_new;
                    }
                },
                onClick: function (event) {
                },
                onDelete: function (event) {
                    event.preventDefault();
                    deleteRelatedProduct();
                },
                onSubmit: function (event) {
                    event.preventDefault();
                    updateRelatedProduct();
                },
            });
            dRelatedGrid.hideColumn('M_Product_ID');
            dRelatedGrid.hideColumn('updated');
        };

        function CartPanel() {
            $divCart.append($divCartMain);
            cartGrid = null;
            cartGrid = $divCartMain.w2grid({
                name: 'gridcart_' + $self.windowNo,
                recordHeight: 25,
                show: {
                    toolbar: true,  // indicates if toolbar is v isible
                    //columnHeaders: true,   // indicates if columns is visible
                    //lineNumbers: true,  // indicates if line numbers column is visible
                    selectColumn: true,  // indicates if select column is visible
                    toolbarReload: false,   // indicates if toolbar reload button is visible
                    toolbarColumns: true,   // indicates if toolbar columns button is visible
                    toolbarSearch: false,   // indicates if toolbar search controls are visible
                    toolbarAdd: false,   // indicates if toolbar add new button is visible
                    toolbarDelete: true,   // indicates if toolbar delete button is visible
                    toolbarSave: true,   // indicates if toolbar save button is visible
                    //selectionBorder: false,	 // display border arround selection (for selectType = 'cell')
                    //recordTitles: false	 // indicates if to define titles for records
                },
                records: [],
                columns: [
                    { field: "product_ID", caption: "product_ID", sortable: false, size: '80px', display: false },
                    { field: "Product", caption: '<div ><span>' + VIS.Msg.translate(VIS.Env.getCtx(), "Product") + '</span></div>', sortable: false, size: '35%', hidden: false },
                    {
                        field: "Qty", caption: '<div ><span>' + VIS.Msg.getElement(VIS.Env.getCtx(), "Quantity") + '</span></div>', sortable: false, size: '15%', hidden: false, style: 'text-align: right', editable: { type: 'float' },
                        //Added by Shifali to change the Qty acc. to culture.
                        render: function (record, index, col_index) {
                            var val = VIS.Utility.Util.getValueOfDecimal(record["Qty"].toFixed(precision));
                            return (val).toLocaleString();
                        }
                    },
                    {
                        field: "C_Uom_ID", caption: '<div ><span>' + VIS.Msg.getElement(VIS.Env.getCtx(), "C_UOM_ID") + '</span></div>', sortable: false, size: '15%', hidden: false, editable: { type: 'select', items: uomArray, showAll: true },
                        render: function (record, index, col_index) {
                            var html = '';
                            for (var p in uomArray) {
                                if (uomArray[p].id == this.getCellValue(index, col_index)) html = uomArray[p].text;
                            }
                            return html;
                        }
                    },
                    { field: "attribute_ID", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "M_AttributeSetInstance_ID"), sortable: false, size: '80px', display: false },
                    {
                        field: "Attribute", caption: '<div ><span>' + VIS.Msg.translate(VIS.Env.getCtx(), "Attribute") + '</span></div>', sortable: false, size: '35%', hidden: false,
                        render: function () {
                            return '<div><input type=text readonly="readonly" style= "width:85%; border:none" ><i class="fa fa-list-alt VA005-gridicon" title="Attribute Set Instance" style="opacity: 1;"></i></div>';
                        }
                    },
                    { field: "UPC", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "UPC"), sortable: false, size: '80px', editable: { type: 'text' } },
                    { field: "LineID", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "VAICNT_InventoryCountLine_ID"), sortable: false, size: '80px', display: false },
                    { field: "updated", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "Updated"), sortable: false, size: '80px', display: false }
                ],

                onChange: function (event) {

                    cartGrid.records[event.index]["updated"] = true;
                    if (event.column == 2) {
                        cartGrid.records[event.index]["Qty"] = event.value_new;
                    }
                    else if (event.column == 3) {
                        cartGrid.records[event.index]["C_Uom_ID"] = event.value_new;
                    }
                },

                onClick: function (event) {
                    if (event.column == 5 && cartGrid.records.length > 0) {
                        // Done by Bharat on 28 Feb 2018 to move queries to server side
                        var mattsetid = VIS.dataContext.getJSONRecord("VA005/ProductManagement/GetAttributeSet", VIS.Utility.Util.getValueOfInt(cartGrid.records[event.recid - 1]["product_ID"]));

                        //var qry = "SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + VIS.Utility.Util.getValueOfInt(cartGrid.records[event.recid - 1]["product_ID"]);
                        //var mattsetid = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));

                        if (mattsetid != 0) {
                            var productWindow = AD_Column_ID == 8418;		//	HARDCODED
                            var M_Locator_ID = VIS.context.getContextAsInt($self.windowNo, "M_Locator_ID");
                            var C_BPartner_ID = VIS.context.getContextAsInt($self.windowNo, "C_BPartner_ID");
                            var obj = new VIS.PAttributesForm(VIS.Utility.Util.getValueOfInt(cartGrid.records[event.recid - 1]["attribute_ID"]), VIS.Utility.Util.getValueOfInt(cartGrid.records[event.recid - 1]["product_ID"]), M_Locator_ID, C_BPartner_ID, productWindow, AD_Column_ID, $self.windowNo);
                            if (obj.hasAttribute) {
                                obj.showDialog();
                            }
                            obj.onClose = function (mAttributeSetInstanceId, name, mLocatorId) {
                                if (cartGrid.records[event.recid - 1]["attribute_ID"] != mAttributeSetInstanceId) {
                                    cartGrid.records[event.recid - 1]["attribute_ID"] = mAttributeSetInstanceId;
                                    cartGrid.records[event.recid - 1]["Attribute"] = name;
                                    $("#grid_gridcart_" + $self.windowNo + "_rec_" + event.recid).find("input[type=text]").val(name);
                                    cartGrid.records[event.recid - 1]["updated"] = true;
                                }
                            };
                        }
                        else {
                            return;
                        }
                    }
                },
                onDelete: function (event) {
                    event.preventDefault();
                    deleteInventory();
                },
                onSubmit: function (event) {
                    event.preventDefault();
                    updateInventory();
                },
                onColumnOnOff: function (event) {
                    event.onComplete = function () {
                        BindCartGrid();
                    }
                },
                onSelect: function (event) {
                    if (event.all) {
                        event.onComplete = function () {
                            for (var k = 0; k < cartGrid.records.length; k++) {
                                $("#grid_gridcart_" + $self.windowNo + "_rec_" + cartGrid.records[k].recid).find("input[type=text]").val(cartGrid.records[k].Attribute);

                                // Done by Bharat on 28 Feb 2018 to move queries to server side
                                var mattsetid = VIS.dataContext.getJSONRecord("VA005/ProductManagement/GetAttributeSet", cartGrid.records[k].product_ID);
                                //var qry = "SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + cartGrid.records[k].product_ID;
                                //if (VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry)) <= 0) {
                                if (mattsetid <= 0) {
                                    $("#grid_gridcart_" + $self.windowNo + "_rec_" + cartGrid.records[k].recid).find("input:not([type='checkbox'])").hide();
                                    $("#grid_gridcart_" + $self.windowNo + "_rec_" + cartGrid.records[k].recid).find("img").hide();
                                }
                            }
                        }
                    }
                },
            });
            cartGrid.hideColumn('product_ID');
            cartGrid.hideColumn('attribute_ID');
            cartGrid.hideColumn('UPC');
            cartGrid.hideColumn('LineID');
            cartGrid.hideColumn('updated');
            LoadCart();
        };

        var LoadOrganization = function () {
            orgid = VIS.context.getContextAsInt("#AD_Org_ID");
            cmbOrg.empty();
            // Done by Bharat on 28 Feb 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadOrganization", "", LoadOrgCallBack);

            //var qry = "SELECT AD_Org_ID,Name FROM AD_Org WHERE IsActive = 'Y' AND (IsSummary='N' OR AD_Org_ID=0)";
            //var sql = VIS.MRole.getDefault().addAccessSQL(qry, "AD_Org", VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO) // fully qualidfied - RO
            //VIS.DB.executeReader(sql.toString(), null, LoadOrgCallBack);
        };

        function LoadOrgCallBack(data) {
            //while (dr.read()) {
            //    key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
            //    value = dr.getString(1);
            //    cmbOrg.append(" <option value=" + key + ">" + value + "</option>");
            //}
            //dr.close();
            // Done by Bharat on 28 Feb 2018 to move queries to server side
            var key, value;
            if (data.length > 0) {
                for (var i in data) {
                    key = VIS.Utility.Util.getValueOfInt(data[i]["AD_Org_ID"]);
                    value = VIS.Utility.Util.getValueOfString(data[i]["Name"]);
                    cmbOrg.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
                }
            }
            cmbOrg.val(orgid);
        };

        var LoadProductTypes = function () {
            cmbProductType.empty();
            // Done by Bharat on 28 Feb 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadProductType", "", LoadProductTypeCallBack);

            //var qry = "SELECT Value,Name FROM  AD_Ref_List WHERE AD_Reference_ID = 270 AND IsActive = 'Y'";
            ////cmbProductType.append('<option value="E">' + VIS.Msg.getMsg("VA005_Expense") + '</option><option selected="selected" value="I">' + VIS.Msg.getMsg("VA005_Item") + '</option>' +
            ////            '<option value="R">' + VIS.Msg.getMsg("VA005_Resource") + '</option><option value="S">' + VIS.Msg.getMsg("VA005_Service") + '</option>');
            //VIS.DB.executeReader(qry, null, LoadProductTypeCallBack);
        }

        function LoadProductTypeCallBack(data) {
            var key, value;
            if (data.length > 0) {
                for (var i in data) {
                    key = VIS.Utility.Util.getValueOfString(data[i]["Value"]);
                    value = VIS.Utility.Util.getValueOfString(data[i]["Name"]);
                    cmbProductType.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
                }
            }
            //while (dr.read()) {
            //    key = dr.getString(0);
            //    value = dr.getString(1);
            //    cmbProductType.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
            //}
            //dr.close();
        };

        var LoadAttributes = function () {
            cmbAttributeSet.empty();
            // Done by Bharat on 28 Feb 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadAttributes", "", LoadAttributesCallBack);

            //var qry = "SELECT M_AttributeSet_ID,Name FROM M_AttributeSet WHERE IsActive = 'Y'";
            //var sql = VIS.MRole.getDefault().addAccessSQL(qry, "M_AttributeSet", VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO) // fully qualidfied - RO
            //VIS.DB.executeReader(sql.toString(), null, LoadAttributesCallBack);
        };

        function LoadAttributesCallBack(data) {
            cmbAttributeSet.append(" <option value = 0></option>");
            var key, value;
            if (data.length > 0) {
                for (var i in data) {
                    key = VIS.Utility.Util.getValueOfInt(data[i]["M_AttributeSet_ID"]);
                    value = VIS.Utility.Util.getValueOfString(data[i]["Name"]);
                    cmbAttributeSet.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
                }
            }
            //while (dr.read()) {
            //    key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
            //    value = dr.getString(1);
            //    cmbAttributeSet.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
            //}
            //dr.close();
        };

        var LoadTaxCategories = function () {
            cmbTaxCategory.empty();
            // Done by Bharat on 28 Feb 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadTaxCategories", "", LoadTaxCallBack);
            //var qry = "SELECT C_TaxCategory_ID,Name FROM C_TaxCategory WHERE IsActive = 'Y'";
            //var sql = VIS.MRole.getDefault().addAccessSQL(qry, "C_TaxCategory", VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO) // fully qualidfied - RO
            //VIS.DB.executeReader(sql.toString(), null, LoadTaxCallBack);
        };

        function LoadTaxCallBack(data) {
            cmbTaxCategory.append(" <option value = 0></option>");
            var key, value;
            if (data.length > 0) {
                for (var i in data) {
                    key = VIS.Utility.Util.getValueOfInt(data[i]["C_TaxCategory_ID"]);
                    value = VIS.Utility.Util.getValueOfString(data[i]["Name"]);
                    cmbTaxCategory.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
                }
            }
            //while (dr.read()) {
            //    key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
            //    value = dr.getString(1);
            //    cmbTaxCategory.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
            //}
            //dr.close();
        };

        var LoadProductCategories = function () {
            cmbCat.empty();
            // Done by Bharat on 28 Feb 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadProductCategories", "", LoadCatCallBack);

            //var qry = "SELECT M_Product_Category_ID,Name FROM M_Product_Category WHERE IsActive = 'Y'";            
            //var sql = VIS.MRole.getDefault().addAccessSQL(qry, "M_Product_Category", VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO) // fully qualidfied - RO
            //VIS.DB.executeReader(sql.toString(), null, LoadCatCallBack);
        };

        function LoadCatCallBack(data) {
            cmbCat.append(" <option value = 0></option>");
            var key, value;
            if (data.length > 0) {
                for (var i in data) {
                    key = VIS.Utility.Util.getValueOfInt(data[i]["M_Product_Category_ID"]);
                    value = VIS.Utility.Util.getValueOfString(data[i]["Name"]);
                    cmbCat.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
                }
            }
            //while (dr.read()) {
            //    key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
            //    value = dr.getString(1);
            //    cmbCat.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
            //}
            //dr.close();
        };

        var LoadUOM = function (cmb) {
            uomCmb = cmb;
            uomCmb.empty();
            // Done by Bharat on 28 Feb 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadUOM", "", LoadUOMCallBack);

            //var qry = "SELECT C_UOM_ID,Name FROM C_UOM WHERE IsActive = 'Y'";
            //var sql = VIS.MRole.getDefault().addAccessSQL(qry, "C_UOM", VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO) // fully qualidfied - RO
            //VIS.DB.executeReader(sql.toString(), null, LoadUOMCallBack);
        };

        function LoadUOMCallBack(data) {
            uomCmb.append(" <option value = 0></option>");
            var key, value;
            if (data.length > 0) {
                for (var i in data) {
                    key = VIS.Utility.Util.getValueOfInt(data[i]["C_UOM_ID"]);
                    value = VIS.Utility.Util.getValueOfString(data[i]["Name"]);
                    uomCmb.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
                }
            }

            //while (dr.read()) {
            //    key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
            //    value = dr.getString(1);
            //    uomCmb.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
            //}
            //dr.close();
        };

        var LoadCartUOM = function () {
            uomArray = [];
            uomArray.push({ id: 0, text: VIS.Msg.getMsg("Select") });
            // Done by Bharat on 28 Feb 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadUOM", "", LoadCartUOMCallBack);

            //var qry = "SELECT C_UOM_ID,Name FROM C_UOM WHERE IsActive = 'Y'";
            //var sql = VIS.MRole.getDefault().addAccessSQL(qry, "C_UOM", VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO) // fully qualidfied - RO
            //VIS.DB.executeReader(sql.toString(), null, LoadCartUOMCallBack);
        };

        function LoadCartUOMCallBack(data) {
            var key, value;
            if (data.length > 0) {
                for (var i in data) {
                    key = VIS.Utility.Util.getValueOfInt(data[i]["C_UOM_ID"]);
                    value = VIS.Utility.Util.getValueOfString(data[i]["Name"]);
                    uomArray.push({ id: key, text: value });
                }
            }

            //while (dr.read()) {
            //    key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
            //    value = dr.getString(1);
            //    uomArray.push({ id: key, text: value });
            //}
            //dr.close();
            CartPanel();
            PriceUpdatePanel();
            SupplierPanel();
            PrintPanel();
            LoadRelatedType();
        };

        var LoadCurrency = function () {
            curArray = [];
            curArray.push({ id: 0, text: VIS.Msg.getMsg("Select") });
            // Done by Bharat on 05 March 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadCurrency", "", LoadCurrencyCallBack);

            //var qry = "SELECT C_Currency_ID, ISO_Code || ' (' || (CurSymbol) ||')' as cur FROM C_Currency WHERE IsActive = 'Y' ORDER BY ISO_Code";
            //var sql = VIS.MRole.getDefault().addAccessSQL(qry, "C_Currency", VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO) // fully qualidfied - RO
            //VIS.DB.executeReader(sql.toString(), null, LoadCurrencyCallBack);

        };

        function LoadCurrencyCallBack(data) {
            var key, value;
            if (data.length > 0) {
                for (var i in data) {
                    key = VIS.Utility.Util.getValueOfInt(data[i]["C_Currency_ID"]);
                    value = VIS.Utility.Util.getValueOfString(data[i]["ISO_Code"]);
                    curArray.push({ id: key, text: value });
                }
            }

            //while (dr.read()) {
            //    key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
            //    value = dr.getString(1);
            //    curArray.push({ id: key, text: VIS.Utility.encodeText(value) });
            //}
            //dr.close();
            LoadCartUOM();
        };

        var LoadUomTo = function () {
            cmbUomTo.empty();
            // Done by Bharat on 28 Feb 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadUOM", "", LoadUomToCallBack);

            //var qry = "SELECT C_UOM_ID,Name FROM C_UOM WHERE IsActive = 'Y'";
            //var sql = VIS.MRole.getDefault().addAccessSQL(qry, "C_UOM", VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO); // fully qualidfied - RO
            //VIS.DB.executeReader(sql.toString(), null, LoadUomToCallBack);
        };

        function LoadUomToCallBack(data) {
            cmbUomTo.append(' <option value = -1>' + VIS.Msg.getMsg("VA005_UomTo") + '</option>');
            var key, value;
            if (data.length > 0) {
                for (var i in data) {
                    key = VIS.Utility.Util.getValueOfInt(data[i]["C_UOM_ID"]);
                    value = VIS.Utility.Util.getValueOfString(data[i]["Name"]);
                    cmbUomTo.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
                }
            }

            //while (dr.read()) {
            //    key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
            //    value = dr.getString(1);
            //    cmbUomTo.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
            //}
            //dr.close();
        };

        var LoadPriceList = function () {
            cmbPriceList.empty();
            // Done by Bharat on 05 March 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadPriceList", "", LoadPriceListCallBack);

            //var qry = "SELECT M_PriceList_Version_ID,Name FROM M_PriceList_Version WHERE IsActive = 'Y'";
            //var sql = VIS.MRole.getDefault().addAccessSQL(qry, "M_PriceList_Version", VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO) // fully qualidfied - RO
            //VIS.DB.executeReader(sql.toString(), null, LoadPriceListCallBack);
        };

        function LoadPriceListCallBack(data) {
            cmbPriceList.append(" <option value = 0></option>");
            var key, value;
            if (data.length > 0) {
                for (var i in data) {
                    key = VIS.Utility.Util.getValueOfInt(data[i]["M_PriceList_Version_ID"]);
                    value = VIS.Utility.Util.getValueOfString(data[i]["Name"]);
                    cmbPriceList.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
                }
            }

            //while (dr.read()) {
            //    key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
            //    value = dr.getString(1);
            //    cmbPriceList.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
            //}
            //dr.close();
            cmbPriceList.prop("selectedIndex", 1);
            BindPriceGrid();
        };

        var LoadSupplier = function () {
            cmbSupplier.empty();
            // Done by Bharat on 05 March 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadSupplier", "", LoadSupplierCallBack);

            //var qry = "SELECT C_BPartner_ID,Name FROM C_BPartner WHERE IsActive = 'Y' AND IsVendor = 'Y' ORDER BY Name";
            //var sql = VIS.MRole.getDefault().addAccessSQL(qry, "C_BPartner", VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO) // fully qualidfied - RO
            //VIS.DB.executeReader(sql.toString(), null, LoadSupplierCallBack);
        };

        function LoadSupplierCallBack(data) {
            //cmbSupplier.append(" <option value = 0>" + VIS.Msg.getMsg("VA005_SelSupplier") + "</option>");
            var key, value;
            if (data.length > 0) {
                for (var i in data) {
                    key = VIS.Utility.Util.getValueOfInt(data[i]["C_BPartner_ID"]);
                    value = VIS.Utility.Util.getValueOfString(data[i]["Name"]);
                    cmbSupplier.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
                }
            }

            //while (dr.read()) {
            //    key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
            //    value = dr.getString(1);
            //    cmbSupplier.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
            //}
            //dr.close();
            cmbSupplier.prop("selectedIndex", 0);
            BindSupplierGrid();
        };

        var LoadCart = function () {
            cmbCart.empty();
            // Done by Bharat on 05 March 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadCart", "", LoadcartCallBack);

            //var qry = "";
            //qry = "SELECT VAICNT_InventoryCount_ID,VAICNT_ScanName FROM VAICNT_InventoryCount WHERE IsActive = 'Y' AND VAICNT_TransactionType = 'OT' ORDER BY VAICNT_ScanName";            
            //var sql = VIS.MRole.getDefault().addAccessSQL(qry, "VAICNT_InventoryCount", VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO) // fully qualidfied - RO
            //VIS.DB.executeReader(sql.toString(), null, LoadcartCallBack);
        }

        function LoadcartCallBack(data) {
            cmbCart.append(" <option value = 0></option>");
            var key, value;
            if (data.length > 0) {
                for (var i in data) {
                    key = data[i]["VAICNT_InventoryCount_ID"];
                    value = data[i]["Name"];
                    cmbCart.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
                }
            }

            //while (dr.read()) {
            //    key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
            //    value = dr.getString(1);
            //    cmbCart.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
            //}
            //dr.close();
            cmbCart.val(cart);
            if (cart > 0) {
                $divHeadProd.find("#VA005_cartInfo_" + $self.windowNo).text(cmbCart.find('option:selected').text());
            }
            else {
                $divHeadProd.find("#VA005_cartInfo_" + $self.windowNo).text(VIS.Msg.getMsg("None"));
            }
        };

        var LoadQueries = function () {
            $queryCat.empty();
            // Done by Bharat on 05 March 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadQueries", "", LoadQueriesCallBack);

            //var qry = "SELECT AD_UserQuery_ID,Name FROM AD_UserQuery WHERE IsActive = 'Y' AND AD_Client_ID = " + VIS.context.getAD_Client_ID() + " AND AD_Tab_ID = 180";
            //VIS.DB.executeReader(qry.toString(), null, LoadQueriesCallBack);
        };

        function LoadQueriesCallBack(data) {
            $queryCat.append(" <option value = 0></option>");
            var key, value;
            if (data != null) {
                for (var i in data) {
                    key = data[i]["AD_UserQuery_ID"];
                    value = data[i]["Name"];
                    $queryCat.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
                }
            }

            //while (dr.read()) {
            //    key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
            //    value = dr.getString(1);
            //    $queryCat.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
            //}
            //dr.close();
        };

        //Mohit
        var LoadModuleInfo = function () {
            _countDTD001 = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(*) FROM AD_MODULEINFO WHERE PREFIX='DTD001_' "));
            //_countBGT01 = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar("SELECT COUNT(*) FROM AD_MODULEINFO WHERE PREFIX='BGT01_' "));
        };
        //Mohit
        var LoadOnCategorySelect = function (_PCat_ID) {
            // Done by Bharat on 05 March 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadOnCategorySelect", { "ProdCatID": _PCat_ID }, CategorySelectCallBack);

            //var Sql = "SELECT IsActive "
            //if (_countDTD001 > 0) {
            //    Sql += " , producttype ";
            //}
            //Sql += " , m_attributeset_id, c_taxcategory_id  FROM M_Product_Category WHERE m_product_category_id=" + VIS.Utility.Util.getValueOfInt(_PCat_ID);
            //var result = VIS.DB.executeDataSet(Sql);
            //if (result != null) {
            //    if (result.tables[0].rows.length > 0) {
            //        if (_countDTD001 > 0) {

            //            cmbProductType.val(VIS.Utility.Util.getValueOfString(result.tables[0].rows[0].cells.producttype));

            //        }

            //        if (VIS.Utility.Util.getValueOfInt(result.tables[0].rows[0].cells.c_taxcategory_id) > 0) {
            //            cmbTaxCategory.val(VIS.Utility.Util.getValueOfInt(result.tables[0].rows[0].cells.c_taxcategory_id)).prop('selected', true);
            //        }
            //        else {
            //            cmbTaxCategory.val(-1);
            //        }
            //        if (VIS.Utility.Util.getValueOfInt(result.tables[0].rows[0].cells.m_attributeset_id) > 0) {
            //            cmbAttributeSet.val(VIS.Utility.Util.getValueOfInt(result.tables[0].rows[0].cells.m_attributeset_id)).prop('selected', true);
            //        }
            //        else {
            //            cmbAttributeSet.val(-1);
            //        }
            //    }
            //}
        };

        function CategorySelectCallBack(data) {
            if (data != null) {
                _countDTD001 = data["_countDTD001"];
                if (_countDTD001) {
                    cmbProductType.val(data["ProductType"]);
                }

                if (data["C_TaxCategory_ID"] > 0) {
                    cmbTaxCategory.val(data["C_TaxCategory_ID"]).prop('selected', true);
                }
                else {
                    cmbTaxCategory.val(-1);
                }

                if (data["M_AttributeSet_ID"] > 0) {
                    cmbAttributeSet.val(data["M_AttributeSet_ID"]).prop('selected', true);
                }
                else {
                    cmbAttributeSet.val(-1);
                }
            }
        }

        function LoadRelatedType() {
            relatedType = [];
            // Done by Bharat on 05 March 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadRelatedType", "", callbackReplenishType);

            //var qry = "SELECT Value, Name FROM AD_Ref_List WHERE AD_Reference_ID = (SELECT AD_Reference_ID FROM AD_Reference WHERE Name = 'M_RelatedProduct Type' )";
            //VIS.DB.executeReader(qry, null, callbackReplenishType);
        };

        function callbackReplenishType(data) {
            relatedType.push({ id: '-1', text: "" });
            var key, value;
            if (data != null) {
                for (var i in data) {
                    key = data[i]["Value"];
                    value = data[i]["Name"];
                    relatedType.push({ id: key, text: VIS.Utility.encodeText(value) });
                }
            }

            //while (dr.read()) {
            //    key = (dr.getString(0));
            //    value = dr.getString(1);
            //    relatedType.push({ id: key, text: value });
            //}
            //dr.close();
            gridRelatedPanel();
        };

        function LoadVersions() {
            cmbVersion.empty();
            // Done by Bharat on 05 March 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadPriceList", "", LoadVersionCallBack);

            //var qry = "SELECT M_PriceList_Version_ID,Name FROM M_PriceList_Version WHERE IsActive = 'Y'";
            //var sql = VIS.MRole.getDefault().addAccessSQL(qry, "M_PriceList_Version", VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO) // fully qualidfied - RO
            //VIS.DB.executeReader(sql.toString(), null, LoadVersionCallBack);
        }

        function LoadVersionCallBack(data) {
            cmbVersion.append(" <option value = 0></option>");
            var key, value;
            if (data.length > 0) {
                for (var i in data) {
                    key = VIS.Utility.Util.getValueOfInt(data[i]["M_PriceList_Version_ID"]);
                    value = VIS.Utility.Util.getValueOfString(data[i]["Name"]);
                    cmbVersion.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
                }
            }

            //while (dr.read()) {
            //    key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
            //    value = dr.getString(1);
            //    cmbVersion.append(" <option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>");
            //}
            //dr.close();
        };

        /*
           Create busyIndicator
        */
        function createBusyIndicator() {
            $bsyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
            //$bsyDiv.css({
            //    "position": "absolute", "width": "98%", "height": "97%", 'text-align': 'center', 'z-index': '999'
            //});
            $bsyDiv[0].style.visibility = "visible";
            $root.append($bsyDiv);
        };

        function createProductList() {
            //prodTemplate();
            $divProduct.find('.vis-group-user-wrap').remove();
            loadProd("", 0, 0, pgno, PAGESIZE, 0);
            $divProduct.on("click", prodContainerClick);
        };

        function prodTemplate() {
            var script = ' <script type="text/x-handlebars-template">' +
                '{{#each this}}' +
                '<div class="vis-group-user-wrap style="margin-left:10px;" data-UID="{{M_ProductID}}">' +
                '<input type="checkbox" data-UID="{{M_ProductID}}" class="vis-group-addLeft VA005-checkbox">' +
                '<div class="vis-group-user-profile vis-group-pro-width">' +
                '<div style=" height:46px;width:46px" class="vis-group-user-img">' +
                '{{#if ProdImage}}' +
                '<img src="' + VIS.Application.contextUrl + '{{ProdImage}}" alt="prod-img">' +
                '{{else}}' +
                '<img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/home/defaultUser46X46.PNG" alt="prod-img">' +
                '{{/if}}' +
                '</div>' +
                '<div class="vis-group-user-text">' +
                '<p style="font-weight: bold">{{Prodname}}</p>' +
                '<span>{{ProCatName}}</span>' +
                '<span style="display:list-item;">{{SearchKey}}</span>' +
                '</div>' +
                '</div>' +
                '<div class="vis-group-user-right d-flex">' +
                '<span class="VA005_Barcode"></span>' +
                '<ul>' +
                '<li style="margin-right:10px;"><span class="fa fa-shopping-cart VA005-icons-color" data-UID="{{M_ProductID}}"></span></li>' +          // if selected user can be updated                                  
                '<li style="margin-right:10px;"><span class="vis vis-edit VA005-icons-color" data-UID="{{M_ProductID}}-{{ProdTableID}}-{{ProdWindowID}}"></span></li>' +
                '</ul>' +
                '<span class="VA005-Uom-span" data-UID="{{C_UomID}}">{{UOM}}</span>' +
                '{{#GenerateBarcode UPC ".VA005_Barcode" }}' +
                '</div>' +
                '</div>' +
                '{{/each}}​' +
                '</script>';
            prodmodtmp = $(script).html();
            prodtheModTmp = Handlebars.compile(prodmodtmp);
        };

        function loadProd(query, pcat_id, searchQuery, pgNo, pgSize, parent_ID) {
            clearRightPanel();
            var id = (pgNo - 1) * pgSize;
            if (searchQuery == null) {
                searchQuery = 0;
            }
            $.ajax({
                url: VIS.Application.contextUrl + "VA005/ProductManagement/GetProdInfo",
                type: "GET",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                //async: false,
                data: ({ searchText: query, proCategory: pcat_id, SearchQuery: searchQuery, pageNo: pgNo, pageSize: pgSize, Parent_ID: parent_ID }),
                success: function (result) {

                    $divProduct.css("width", "100%");
                    var data = JSON.parse(result);
                    if (data.length > 0) {
                        if (pgNo == 1) {
                            productCount = data[0].productCount;
                            noPages = Math.ceil(productCount / PAGESIZE);
                        }
                        if (btnSelectAll.prop("checked")) {
                            for (var i in data) {
                                id = id + 1;
                                var Prodname = VIS.Utility.encodeText(data[i].Prodname);
                                var ProCatName = VIS.Utility.encodeText(data[i].ProCatName);
                                var M_ProductID = data[i].M_ProductID;
                                var SearchKey = VIS.Utility.encodeText(data[i].SearchKey);
                                var C_UomID = data[i].C_UomID;
                                var UOM = VIS.Utility.encodeText(data[i].UOM);
                                var upc = VIS.Utility.encodeText(data[i].UPC);
                                var d = new Date();
                                var ProdImage = data[i].ProdImage;
                                var imgCtrl = "";
                                if (ProdImage == "") {
                                    //ProdImage = VIS.Application.contextUrl + "Areas/VIS/Images/home/defaultUser46X46.PNG";
                                    imgCtrl = '<i class="fa fa-user" alt="prod-img"></i>';
                                }
                                else {
                                    ProdImage = VIS.Application.contextUrl + ProdImage;
                                    imgCtrl = '<img src="' + ProdImage + "?" + d.getTime() + '" alt="prod-img">';
                                }
                                //$divProduct.append(prodtheModTmp(data));
                                prods.push(M_ProductID);

                                var divProduct = $('<div class="vis-group-user-wrap vis-group-selected-op vis-group-selected-opbackground" data-UID="' + M_ProductID + '">' +
                                    '<input type="checkbox" data-UID="' + M_ProductID + '"style="margin-left:0px;" class="vis-group-addLeft VA005-checkbox" checked="true">' +
                                    '<div draggable="true" class="VA005-productinfowrp vis-group-user-profile" data-UID="' + M_ProductID + '">' +
                                    '<div class="vis-group-user-img vis-chatimgwrap">' + imgCtrl + '</div>' +
                                    //'<div class="vis-group-user-text" style="width:calc(100% - 70px);"><p style="font-weight: bold">' + Prodname +
                                    '<div class="vis-group-user-text"><p class="VA005-NameOfSelectedNode" style="font-weight: bold">' + Prodname +
                                    '</p><span>' + ProCatName + '</span><span style="display:list-item;">' + SearchKey + '</span></div></div>' +
                                    '<div class="vis-group-user-right d-flex"><span id="VA005_Barcode_' + id + '" style="float:left;"></span><div class="d-flex flex-column"><div class="vis-group-user-right d-flex">' +
                                    '<span class="VA005-uom-icons vis vis-image VA005-icons-color" data-UID="' + M_ProductID + '" title="' + VIS.Msg.getMsg("VA005_ShowAllImages") + '"></span>' +
                                    '<span class="VA005-uom-icons vis vis-edit VA005-icons-color" data-UID="' + M_ProductID + '" title="' + VIS.Msg.getMsg("Edit") + '"></span>' +
                                    '<span class="VA005-uom-icons fa fa-shopping-cart VA005-icons-color" data-UID="' + M_ProductID + '" title="' + VIS.Msg.getMsg("VA005_AddCart") + '"></span></div>' +
                                    '<span class="VA005-Uom-span" data-UID="' + C_UomID + '">' + UOM + '</span></div></div></div>');
                                $divProductInner.append(divProduct);
                                GenerateBarcode(upc, $divProduct.find("#VA005_Barcode_" + id));
                                divProduct.find('.vis-group-user-profile').width(divProduct.width() - divProduct.find('.vis-group-user-right:eq(0)').width() - 20);
                            }
                        }
                        else {
                            for (var i in data) {
                                id = id + 1;
                                var Prodname = VIS.Utility.encodeText(data[i].Prodname);
                                var ProCatName = VIS.Utility.encodeText(data[i].ProCatName);
                                var M_ProductID = data[i].M_ProductID;
                                var SearchKey = VIS.Utility.encodeText(data[i].SearchKey);
                                var C_UomID = data[i].C_UomID;
                                var UOM = VIS.Utility.encodeText(data[i].UOM);
                                var upc = VIS.Utility.encodeText(data[i].UPC);
                                var d = new Date();
                                var ProdImage = data[i].ProdImage;
                                var imgCtrl = "";
                                if (ProdImage == "") {
                                    //ProdImage = VIS.Application.contextUrl + "Areas/VIS/Images/home/defaultUser46X46.PNG";
                                    imgCtrl = '<i class="fa fa-user" alt="prod-img"></i>';
                                }
                                else {
                                    ProdImage = VIS.Application.contextUrl + ProdImage;
                                    imgCtrl = '<img src="' + ProdImage + "?" + d.getTime() + '" alt="prod-img">';
                                }
                                //$divProduct.append(prodtheModTmp(data));
                                var divProduct = $('<div class="vis-group-user-wrap" data-UID="' + M_ProductID + '">' +
                                    '<input type="checkbox" data-UID="' + M_ProductID + '" style="margin-left:0px;" class="vis-group-addLeft VA005-checkbox">' +
                                    '<div draggable="true" class="VA005-productinfowrp vis-group-user-profile" data-UID="' + M_ProductID + '">' +
                                    '<div class="vis-group-user-img vis-chatimgwrap">' + imgCtrl + '</div>' +
                                    //'<div class="vis-group-user-text" style="width:calc(100% - 70px);"><p style="font-weight: bold">' + Prodname +
                                    '<div class="vis-group-user-text"><p class="VA005-NameOfSelectedNode" style="font-weight: bold">' + Prodname +
                                    '</p><span>' + ProCatName + '</span><span style="display:list-item;">' + SearchKey + '</span></div></div>' +
                                    '<div class="vis-group-user-right d-flex"><span id="VA005_Barcode_' + id + '" style="float:left;"></span><div class="d-flex flex-column"><div class="vis-group-user-right d-flex">' +
                                    '<span class="VA005-uom-icons vis vis-image VA005-icons-color" data-UID="' + M_ProductID + '" title="' + VIS.Msg.getMsg("VA005_ShowAllImages") + '"></span>' +
                                    '<span class="VA005-uom-icons vis vis-edit VA005-icons-color" data-UID="' + M_ProductID + '" title="' + VIS.Msg.getMsg("Edit") + '"></span>' +
                                    '<span class="VA005-uom-icons fa fa-shopping-cart VA005-icons-color" data-UID="' + M_ProductID + '" title="' + VIS.Msg.getMsg("VA005_AddCart") + '"></span></div>' +
                                    '<span class="VA005-Uom-span" data-UID="' + C_UomID + '">' + UOM + '</span></div></div></div>');
                                $divProductInner.append(divProduct);
                                GenerateBarcode(upc, $divProduct.find("#VA005_Barcode_" + id));
                                divProduct.find('.vis-group-user-profile').width(divProduct.width() - divProduct.find('.vis-group-user-right:eq(0)').width() - 20);
                            }
                        }

                    }
                    //JID_1757 In product management window if product is not there then its display the message
                    else {
                        VIS.ADialog.info("FindZeroRecords");
                    }
                    prodContainerWidth = $($('.vis-group-user-wrap')[0]).width()
                    DragDropProduct();

                    window.setTimeout(function () {
                        midtopdivvv.css("float", "left");
                        $bsyDiv[0].style.visibility = 'hidden';
                    }, 500);


                    //                    $bsyDiv[0].style.visibility = 'hidden';
                },
                error: function () {

                    VIS.ADialog.error("VA005_ErrorLoadingProducts");
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });
            DragDropDocument();
        };

        function saveProduct() {

            $bsyDiv[0].style.visibility = "visible";
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "VA005/ProductManagement/Save",
                dataType: "json",
                //async: false,
                data: {
                    id: M_Product_ID,
                    Org: cmbOrg.val(),
                    Name: VIS.Utility.Util.getValueOfString(txtName.val()),
                    Value: VIS.Utility.Util.getValueOfString(txtValue.val()),
                    ProductType: cmbProductType.val(),
                    attrSet: VIS.Utility.Util.getValueOfInt(cmbAttributeSet.val()),
                    taxcat: VIS.Utility.Util.getValueOfInt(cmbTaxCategory.val()),
                    prodCat: VIS.Utility.Util.getValueOfInt(cmbCat.val()),
                    UOM: VIS.Utility.Util.getValueOfInt(cmbUOM.val()),
                    UPC: VIS.Utility.Util.getValueOfString(txtUpc.val()),
                    Parent_ID: parent_ID
                },
                success: function (data) {
                    var returnValue = JSON.parse(data);

                    if (returnValue.error == "") {
                        if (M_Product_ID == 0) {
                            M_Product_ID = returnValue.M_Product_ID;
                            //var sql = "UPDATE AD_TreeNodePR SET Parent_ID = " + parent_ID + " WHERE Node_ID = " + returnValue.M_Product_ID;
                            //VIS.DB.executeQuery(sql);
                        }
                        ClearProdData();
                        //ch.close();
                        prods = [];
                        uoms = [];
                        pgno = 1;
                        $ulLefttoolbar.find('li').remove();
                        loadCategories($txtsearchCat.val(), 1, PAGESIZE);
                        $divProduct.find('.vis-group-user-wrap').remove();
                        loadProd($searchProduct.val(), pcat_ID, $queryCat.val(), pgno, PAGESIZE, parent_ID);
                        ch.close();
                        $bsyDiv[0].style.visibility = "hidden";
                        VIS.ADialog.info("RecSaved");
                        //CallBack(returnValue);
                    }
                    else {
                        $bsyDiv[0].style.visibility = "hidden";
                        VIS.ADialog.error(returnValue.error);
                        //CallBack(returnValue);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {

                    console.log(textStatus);
                    $bsyDiv[0].style.visibility = "hidden";
                    alert(errorThrown);
                    //CallBack(errorThrown);
                }
            });
        };

        function updatePrice() {
            $bsyDiv[0].style.visibility = "visible";
            //var qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID = " + 251 + " AND ColumnName = 'M_AttributeSetInstance_ID'";
            //var count = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
            //var hasAttribute = count > 0 ? true : false;
            //qry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID = " + 251 + " AND ColumnName = 'C_UOM_ID'";
            //count = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
            //var hasUom = count > 0 ? true : false;

            var savedPrice = [];
            for (item in dGrid.records) {
                if (dGrid.records[item].updated == true) {
                    savedPrice.push(dGrid.records[item]);
                }
            }
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "VA005/ProductManagement/UpdatePrice",
                dataType: "json",
                //async: false,
                contentType: "application/json; charset=utf-8",
                //data: JSON.stringify({ id: cmbPriceList.val(), priceData: savedPrice, HasAttribute: hasAttribute, HasUom: hasUom }),
                data: JSON.stringify({ id: cmbPriceList.val(), priceData: savedPrice }),
                success: function (data) {
                    var returnValue = data.result;

                    $bsyDiv[0].style.visibility = "hidden";
                    if (returnValue == "") {
                        //prods = [];
                        //uoms = [];
                        //pgno = 1;
                        //$divProduct.find('.vis-group-user-wrap').remove();
                        //loadProd($searchProduct.val(), pcat_ID, $queryCat.val(), pgno, PAGESIZE);
                        //Callback(returnValue);
                        pl.close();
                        VIS.ADialog.info("Updated");
                    }
                    else {
                        VIS.ADialog.error("", "", returnValue, "");
                        //Callback(returnValue);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {

                    console.log(textStatus);
                    $bsyDiv[0].style.visibility = "hidden";
                    alert(errorThrown);
                    return;
                }
            });
        };

        function updateSupplier() {

            $bsyDiv[0].style.visibility = "visible";
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "VA005/ProductManagement/UpdateSupplier",
                dataType: "json",
                //async: false,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ Supplier: cmbSupplier.val(), ColumnName: sGrid.records }),
                success: function (data) {
                    var returnValue = data.result;

                    $bsyDiv[0].style.visibility = "hidden";
                    if (returnValue == "") {
                        //prods = [];
                        //uoms = [];
                        //pgno = 1;
                        //$divProduct.find('.vis-group-user-wrap').remove();
                        //loadProd($searchProduct.val(), pcat_ID, $queryCat.val(), pgno, PAGESIZE);
                        //CallBack(returnValue);
                        ml.close();
                        VIS.ADialog.info("Updated");
                    }
                    else {
                        VIS.ADialog.error("", "", returnValue, "");
                        //CallBack(returnValue);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    // 
                    console.log(textStatus);
                    $bsyDiv[0].style.visibility = "hidden";
                    alert(errorThrown);
                    return;
                }
            });
        };

        function saveInventoryCount() {

            $bsyDiv[0].style.visibility = "visible";
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "VA005/ProductManagement/saveInventoryCount",
                dataType: "json",
                //async: false,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ ColumnName: txtScan.val() }),
                success: function (data) {

                    if (data.result != "") {
                        divNewCart.hide();
                        divCartList.show();
                        txtScan.val("");
                        cart = data.result;
                        LoadCart();
                        BindCartGrid();
                    }
                    else {
                        //VIS.ADialog.error(data.result);
                        $bsyDiv[0].style.visibility = "hidden";
                        VIS.ADialog.error(RecordNotSaved);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    // 
                    console.log(textStatus);
                    $bsyDiv[0].style.visibility = "hidden";
                    alert(errorThrown);
                    return;
                }
            });
        };

        function saveInventory() {

            if (multiValues.length > 0) {
                //$bsyDiv[0].style.visibility = "visible";
                $.ajax({
                    type: "POST",
                    url: VIS.Application.contextUrl + "VA005/ProductManagement/saveInventory",
                    dataType: "json",
                    //async: false,
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify({ prodIDs: prods, count_id: cmbCart.val(), ColumnName: multiValues }),
                    success: function (data) {

                        if (data.result == "") {
                            //prods = [];
                            //uoms = [];
                            //cons = [];
                            //attributes = [];
                            //$divProduct.find(".vis-group-user-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
                            //$divProduct.find(".VA005-checkbox").prop("checked", false);
                            //clearRightPanel();
                            BindCartGrid();
                        }
                        else {
                            isClick = false;
                            VIS.ADialog.error(data.result);
                            $bsyDiv[0].style.visibility = "hidden";
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        isClick = false;
                        console.log(textStatus);
                        $bsyDiv[0].style.visibility = "hidden";
                        alert("RecordNotSaved");
                        return;
                    }
                });
            }
            else {
                isClick = false;
                $bsyDiv[0].style.visibility = "hidden";
            }
        };

        function updateInventory() {

            var savedCart = [];
            for (item in cartGrid.records) {
                if (cartGrid.records[item].updated == true) {
                    savedCart.push(cartGrid.records[item]);
                }
            }
            if (savedCart.length > 0) {
                $bsyDiv[0].style.visibility = "visible";
                $.ajax({
                    type: "POST",
                    url: VIS.Application.contextUrl + "VA005/ProductManagement/updateInventory",
                    dataType: "json",
                    async: false,
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify({ count_id: cmbCart.val(), ColumnName: savedCart }),
                    success: function (data) {

                        $bsyDiv[0].style.visibility = "hidden";
                        if (data.result == "") {
                            BindCartGrid();
                            $(".VA005-cart-update").fadeIn();
                            $(".VA005-cart-update").fadeOut(2000);
                        }
                        else {
                            VIS.ADialog.error(data.result);
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        // 
                        console.log(textStatus);
                        $bsyDiv[0].style.visibility = "hidden";
                        alert("RecordNotSaved");
                        return;
                    }
                });
            }
        };

        function deleteInventory() {

            $bsyDiv[0].style.visibility = "visible";
            var deleteCart = [];
            var selection = cartGrid.getSelection();
            for (item in selection) {
                deleteCart.push(cartGrid.get(selection[item]));
            }
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "VA005/ProductManagement/deleteInventory",
                dataType: "json",
                async: false,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ ColumnName: deleteCart }),
                success: function (data) {

                    $bsyDiv[0].style.visibility = "hidden";
                    if (data.result == "") {
                        BindCartGrid();
                        cartGrid.selectNone();
                    }
                    else {
                        VIS.ADialog.error(data.result);
                        BindCartGrid();
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    // 
                    console.log(textStatus);
                    $bsyDiv[0].style.visibility = "hidden";
                    alert("DeleteError");
                    return;
                }
            });
        };

        function updateRelatedProduct() {
            var relatedProducts = [];
            for (item in dRelatedGrid.records) {
                if (dRelatedGrid.records[item].updated == true) {
                    relatedProducts.push(dRelatedGrid.records[item]);
                }
            }
            if (relatedProducts.length > 0) {
                $bsyDiv[0].style.visibility = "visible";
                $.ajax({
                    type: "POST",
                    url: VIS.Application.contextUrl + "VA005/ProductManagement/UpdateRelatedProduct",
                    dataType: "json",
                    //async: false,
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify({ id: prods[0], relatedData: relatedProducts }),
                    success: function (data) {

                        $bsyDiv[0].style.visibility = "hidden";
                        if (data.result == "") {
                            bindRelatedGrid();
                        }
                        else {
                            VIS.ADialog.error(data.result);
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        // 
                        console.log(textStatus);
                        $bsyDiv[0].style.visibility = "hidden";
                        alert("RecordNotSaved");
                        return;
                    }
                });
            }
        };

        function deleteRelatedProduct() {
            $bsyDiv[0].style.visibility = "visible";
            var deleteRelated = [];
            var selection = dRelatedGrid.getSelection();
            for (item in selection) {
                deleteRelated.push(dRelatedGrid.get(selection[item]));
            }
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "VA005/ProductManagement/deleteRelatedProduct",
                dataType: "json",
                async: false,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ id: prods[0], relatedData: deleteRelated }),
                success: function (data) {

                    $bsyDiv[0].style.visibility = "hidden";
                    if (data.result == "") {
                        bindRelatedGrid();
                        dRelatedGrid.selectNone();
                    }
                    else {
                        VIS.ADialog.error(data.result);
                        bindRelatedGrid();
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    // 
                    console.log(textStatus);
                    $bsyDiv[0].style.visibility = "hidden";
                    alert("DeleteError");
                    return;
                }
            });
        }

        var zoomToWindow = function (record_id, windowName) {

            // Done by Bharat on 05 March 2018 to move queries to server side
            var ad_window_Id = VIS.dataContext.getJSONRecord("VA005/ProductManagement/GetWindow_ID", windowName);

            //var sql = "select ad_window_id from ad_window where name = '" + windowName + "'";// Upper( name)=Upper('user' )
            //var ad_window_Id = 0;
            try {
                //var dr = VIS.DB.executeDataReader(sql);
                //if (dr.read()) {
                //    ad_window_Id = dr.getInt(0);
                //}
                //dr.dispose();
                if (ad_window_Id > 0) {
                    var zoomQuery = new VIS.Query();
                    if (windowName == "Product")
                        zoomQuery.addRestriction("M_Product_ID", VIS.Query.prototype.EQUAL, record_id);
                    else if (windowName == "Attribute Set")
                        zoomQuery.addRestriction("M_AttributeSet_ID", VIS.Query.prototype.EQUAL, record_id);
                    else if (windowName == "Product Category")
                        zoomQuery.addRestriction("M_Product_Category_ID", VIS.Query.prototype.EQUAL, record_id);
                    else if (windowName == "Tax Category")
                        zoomQuery.addRestriction("C_TaxCategory_ID", VIS.Query.prototype.EQUAL, record_id);
                    else if (windowName == "Unit of Measure")
                        zoomQuery.addRestriction("C_Uom_ID", VIS.Query.prototype.EQUAL, record_id);
                    else if (windowName == "VAICNT_InventoryCount")
                        zoomQuery.addRestriction("VAICNT_InventoryCount_ID", VIS.Query.prototype.EQUAL, record_id);
                    else if (windowName == "Vendor Master")
                        zoomQuery.addRestriction("C_BPartner_ID", VIS.Query.prototype.EQUAL, record_id);
                    zoomQuery.setRecordCount(1);
                    VIS.viewManager.startWindow(ad_window_Id, zoomQuery);
                    if (ch != null) {
                        ch.close();
                    }
                }
            }
            catch (e) {
                console.log(e);
            }
        };

        var fillProductDetails = function () {
            // Done by Bharat on 05 March 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadProductData", { "M_Product_ID": prods[0] }, callBackFillProduct);

            //var sql = "SELECT * FROM M_Product WHERE M_Product_ID = " + prods[0];
            //try {
            //    var dr = VIS.DB.executeDataReader(sql);
            //    if (dr.read()) {
            //        cmbOrg.val(dr.getInt("AD_Org_ID"));
            //        txtName.val(dr.getString("Name"));
            //        if (M_Product_ID == 0) {
            //            txtValue.val("");
            //        }
            //        else {
            //            txtValue.val(dr.getString("Value"));
            //        }
            //        cmbUOM.val(dr.getInt("C_UOM_ID"));
            //        cmbAttributeSet.val(dr.getInt("M_AttributeSet_ID"));
            //        cmbCat.val(dr.getInt("M_Product_Category_ID"));
            //        cmbProductType.val(dr.getString("ProductType"));
            //        cmbTaxCategory.val(dr.getInt("C_TaxCategory_ID"));
            //        txtUpc.val(dr.getString("UPC"));
            //    }
            //    dr.dispose();
            //}
            //catch (e) {
            //    console.log(e);
            //}
        };

        function callBackFillProduct(dr) {
            if (dr != null) {
                cmbOrg.val(dr["AD_Org_ID"]);
                txtName.val(dr["Name"]);
                if (M_Product_ID == 0) {
                    txtValue.val("");
                }
                else {
                    txtValue.val(dr["Value"]);
                }
                cmbUOM.val(dr["C_UOM_ID"]);
                cmbAttributeSet.val(dr["M_AttributeSet_ID"]);
                cmbCat.val(dr["M_Product_Category_ID"]);
                cmbProductType.val(dr["ProductType"]);
                cmbTaxCategory.val(dr["C_TaxCategory_ID"]);
                txtUpc.val(dr["UPC"]);
            }
        }

        var ClearProdData = function () {
            cmbOrg.val(orgid);
            txtName.val("");
            txtValue.val("");
            cmbUOM.val(-1);
            cmbAttributeSet.val(-1);
            cmbCat.val(parentCat);
            cmbProductType.val("I");
            cmbTaxCategory.val(parentTax);
            txtUpc.val("");
            M_Product_ID = 0;
        }

        function ClearPrintData() {
            txtHeader.val("");
            cmbVersion.val(0);
        };

        function events() {
            productAttValues.on("click", function (e) {
                var currTarger = $(e.target);
                if (currTarger.hasClass("VA005-AttValofbindingImg")) {
                    productAttValues.find(".VA005-AttValueImgaeBorderSelect").removeClass("VA005-AttValueImgaeBorderSelect");
                }
                else if (currTarger.hasClass("VA005-Image-AttValImg")) {
                    var currentParent = currTarger.parent();
                    if (currentParent.hasClass("VA005-AttValueImgaeBorderSelect")) {
                        currentParent.removeClass("VA005-AttValueImgaeBorderSelect");
                    }
                    else {
                        currentParent.addClass("VA005-AttValueImgaeBorderSelect");
                    }
                }
                else if (currTarger.hasClass("VA005-Image-AttValPar")) {
                    var currentParent = currTarger.parent();
                    if (currentParent.hasClass("VA005-AttValueImgaeBorderSelect")) {
                        currentParent.removeClass("VA005-AttValueImgaeBorderSelect");
                    }
                    else {
                        currentParent.addClass("VA005-AttValueImgaeBorderSelect");
                    }
                }
                else if (currTarger.hasClass("VA005-AttributesValuesImgTop")) {
                    if (currTarger.hasClass("VA005-AttValueImgaeBorderSelect")) {
                        currTarger.removeClass("VA005-AttValueImgaeBorderSelect");
                    }
                    else {
                        currTarger.addClass("VA005-AttValueImgaeBorderSelect");
                    }
                }

                if (productAttValues.find(".VA005-AttValueImgaeBorderSelect").length > 0) {
                    $btnDelAttValues.removeClass("VA005-AttValOpacity");
                }
                else {
                    $btnDelAttValues.addClass("VA005-AttValOpacity");
                }

            })

            midtopdivvv.on("resize", function (evt, ui) {
                currentheight = ui.size.height;
                $root.find(".vis-group-users-container").height(currentheight);
                productAttValues.parent().height($middlePanel.height() - ($root.find(".vis-group-users-container").height() + 108));
                productAttValues.height($middlePanel.height() - ($root.find(".vis-group-users-container").height() + 128));

                var dynamicHeight = $middlePanel.height() - 90;
                midtopdivvv.css("max-height", dynamicHeight);
                $root.find(".vis-group-users-container").css("max-height", dynamicHeight);
                midtopdivvv.css("width", "100%");
                $root.find(".vis-group-users-container").css("width", "100%");
                $divProduct.height($divProduct.height() - 20);
                window.setTimeout(function () {
                    productAttValues.height(productAttValues.parent().height() - 15);
                }, 200);
            });



            productAttValues.mouseover(function (e) {
                if ($root.find(".vis-group-users-container .vis-group-user-wrap").hasClass("ui-droppable")) {
                    $root.find(".vis-group-users-container .vis-group-user-wrap").droppable("destroy");
                }
            });

            $root.find(".vis-group-users-container").mouseover(function (e) {
                $root.find(".vis-group-users-container .vis-group-user-wrap").droppable({
                    drop: function (event, ui) {
                        SaveProductImage($(ui.draggable).attr("path"), $(ui.draggable).attr("filename"), parseInt($(this).attr("data-uid")), $(this));
                    }
                });
            });

            $ImageZoom.click(function () {
                $ImageZoom.fadeOut("slow");
            });

            if ($ulLefttoolbar) {
                $ulLefttoolbar.on("click", "LI", function (e) {

                    pcat_ID = VIS.Utility.Util.getValueOfInt($(e.target).attr("procatid"));
                    var cname = $(e.target).text();
                    cname = " -> " + cname.substr(0, cname.indexOf("("));
                    if (pcat_ID > 0) {
                        pgno = 1;
                        prods = [];
                        uoms = [];
                        $queryCat.val(0);
                        $searchProduct.val("")
                        btnSelectAll.prop("checked", false);
                        clearRightPanel();
                        catSpan.text(cname).show();
                        //Manish 10/4/2016
                        if (attrBottomValuesFlag == true) {
                            $btnAttValuesDivClose.trigger("click");
                        }
                        //end
                        $divProduct.find('.vis-group-user-wrap').remove();
                        loadProd("", pcat_ID, 0, pgno, PAGESIZE, 0);
                    }
                });
            }

            if ($zoomDiv != null) {
                $zoomDiv.on("click", "LI", function (e) {

                    var action = $(e.target).data("action");
                    if (action == VIS.Actions.refresh) {
                        if (options.Attribute == "Y")
                            LoadAttributes();
                        else if (options.Category == "Y")
                            LoadProductCategories(cmbCat);
                        else if (options.Tax == "Y")
                            LoadTaxCategories();
                        else if (options.UOM == "Y")
                            LoadUOM(cmbUOM);
                    }
                    else if (action == VIS.Actions.zoom) {
                        if (options.Attribute == "Y")
                            zoomToWindow(VIS.Utility.Util.getValueOfInt(cmbAttributeSet.val()), "Attribute Set");
                        if (options.Category == "Y")
                            zoomToWindow(VIS.Utility.Util.getValueOfInt(cmbCat.val()), "Product Category");
                        else if (options.Tax == "Y")
                            zoomToWindow(VIS.Utility.Util.getValueOfInt(cmbTaxCategory.val()), "Tax Category");
                        else if (options.UOM == "Y")
                            zoomToWindow(VIS.Utility.Util.getValueOfInt(cmbUOM.val()), "Unit of Measure");
                    }
                });
            }

            //Manjot
            $queryBy.on("change", function () {
                if ($queryBy.val() != "") {
                    if ($queryBy.val() == "C") {
                        $root.find(".VA005-middle-cat-bar").show();
                        $categDiv.css({ "display": "block" });
                        $searchCat.show();
                        $txtsearchCat.val("");
                        $ulLefttoolbar.find('li').remove();
                        loadCategories($txtsearchCat.val(), 1, PAGESIZE);
                        $leftui.css({ "display": "block" });
                        $leftTreeDiv.css({ "display": "none" });
                        pcat_ID = 0;
                        pgno = 1;
                        prods = [];
                        uoms = [];
                        parent_ID = 0;
                        parentAttribute = 0;
                        parentCat = 0;
                        parentQuality = 0;
                        parentTax = 0;
                        $queryCat.val(0);
                        $searchProduct.val("")
                        btnSelectAll.prop("checked", false);
                        clearRightPanel();
                        // Manish 10/4/2016
                        if (attrBottomValuesFlag == true) {
                            $btnAttValuesDivClose.trigger("click");
                        }
                        // end

                        $divProduct.find('.vis-group-user-wrap').remove();
                        loadProd("", pcat_ID, 0, pgno, PAGESIZE, 0);
                    }
                    else if ($queryBy.val() == "T") {
                        catSpan.text("");
                        $root.find(".VA005-middle-cat-bar").hide();
                        $categDiv.css({ "display": "none" });
                        $searchCat.hide();
                        $leftui.css({ "display": "none" });
                        $bsyDiv[0].style.visibility = "visible";
                        GetTreeForLeft();
                        pcat_ID = 0;
                        pgno = 1;
                        prods = [];
                        uoms = [];
                        parentAttribute = 0;
                        parentCat = 0;
                        parentQuality = 0;
                        parentTax = 0;
                        $queryCat.val(0);
                        $searchProduct.val("")
                        btnSelectAll.prop("checked", false);
                        clearRightPanel();
                        // Manish 10/4/2016
                        if (attrBottomValuesFlag == true) {
                            $btnAttValuesDivClose.trigger("click");
                        }
                        // end

                        $divProduct.find('.vis-group-user-wrap').remove();
                        loadProd("", pcat_ID, 0, pgno, PAGESIZE, parent_ID);
                    }
                }
            });
            //End

            /************/

            btnRefreshForm.on("click", function (e) {
                $bsyDiv[0].style.visibility = "visible";
                window.setTimeout(function () {
                    if (attrBottomValuesFlag == true) {
                        $btnAttValuesDivClose.trigger("click");
                    }
                    LoadQueries();
                    LoadUomTo();
                    LoadOrganization();
                    LoadProductTypes();
                    LoadAttributes();
                    LoadTaxCategories();
                    LoadProductCategories();
                    LoadUOM(cmbUOM);
                    DragDropDocument();
                    DragDropProduct();
                    clearRightPanel();
                    $rightPanel.show();
                    ImagePanel.hide();
                    pgno = 1;
                    prods = [];
                    $ulLefttoolbar.find('li').remove();
                    loadCategories($txtsearchCat.val(), 1, PAGESIZE);
                    $divProduct.find('.vis-group-user-wrap').remove();
                    loadProd($searchProduct.val(), pcat_ID, 0, pgno, PAGESIZE, parent_ID);
                }, 1000);
            });

            btnUpload.on("click", function (e) {
                if (btnUpload != null) {
                    VA005.uploadImage(0, 0, 0, 0, ProductImgul, $self);
                }
            });

            btnImgDelete.on("click", function (e) {
                if (btnImgDelete != null) {
                    DeleteImages();
                }
            });

            btnErase.on("click", function (e) {
                if (btnErase != null) {
                    selectedimg = [];
                    ProductImgul.find('li').removeClass('VA005-highlighted');
                }
            });
            /************/

            $searchCatBtn.on("click", function (e) {

                $bsyDiv[0].style.visibility = "visible";
                $ulLefttoolbar.find('li').remove();
                loadCategories($txtsearchCat.val(), 1, PAGESIZE);
                window.setTimeout(function () {
                    $bsyDiv[0].style.visibility = "hidden";
                }, 1000);
            });

            $txtsearchCat.on("keydown", function (e) {

                if (e.keyCode == 9 || e.keyCode == 13) {
                    $bsyDiv[0].style.visibility = "visible";
                    $ulLefttoolbar.find('li').remove();
                    loadCategories($txtsearchCat.val(), 1, PAGESIZE);
                    window.setTimeout(function () {
                        $bsyDiv[0].style.visibility = "hidden";
                    }, 1000);
                }
            });

            $searchProdBtn.on("click", function (e) {

                $bsyDiv[0].style.visibility = "visible";
                pgno = 1;
                prods = [];
                uoms = [];
                clearRightPanel();
                btnSelectAll.prop("checked", false);
                // Manish 10/4/2016
                if (attrBottomValuesFlag == true) {
                    $btnAttValuesDivClose.trigger("click");
                }
                // end
                $divProduct.find('.vis-group-user-wrap').remove();
                loadProd($searchProduct.val(), pcat_ID, $queryCat.val(), pgno, PAGESIZE, 0);
            });

            $searchProduct.on("keydown", function (e) {

                if (e.keyCode == 9 || e.keyCode == 13) {
                    $bsyDiv[0].style.visibility = "visible";
                    pgno = 1;
                    prods = [];
                    uoms = [];
                    clearRightPanel();
                    btnSelectAll.prop("checked", false);
                    // Manish 10/4/2016
                    if (attrBottomValuesFlag == true) {
                        $btnAttValuesDivClose.trigger("click");
                    }
                    // end
                    $divProduct.find('.vis-group-user-wrap').remove();
                    loadProd($searchProduct.val(), pcat_ID, $queryCat.val(), pgno, PAGESIZE, 0);
                }
            });

            $divProduct.on("scroll", prodScroll);

            if ($queryCat) {
                $queryCat.change(function () {

                    pgno = 1;
                    prods = [];
                    uoms = [];
                    pcat_ID = 0;
                    btnSelectAll.prop("checked", false);
                    $searchProduct.val("");
                    catSpan.hide();
                    clearRightPanel();
                    // Manish 10/4/2016
                    if (attrBottomValuesFlag == true) {
                        $btnAttValuesDivClose.trigger("click");
                    }
                    // end
                    $divProduct.find('.vis-group-user-wrap').remove();
                    loadProd("", 0, $queryCat.val(), pgno, PAGESIZE, parent_ID);
                });
            }

            $divlbMain.on("scroll", prodCatScroll);

            $btnlbToggle.on("click", function (e) {

                e.stopPropagation();
                var w = $td0leftbar.width();
                var wr = $td2_tr1.width();
                if (w > 50) {
                    //$ulLefttoolbar.find('span').hide();
                    //$ulLefttoolbar.find('input').hide();
                    $divlbMain.hide();
                }

                $td0leftbar.animate({
                    "width": w > 50 ? 40 : 250
                }, 300, 'swing', function () {

                    if (w < 50) {
                        $divlbMain.show();
                    }

                    $divProduct.find('.vis-group-user-profile').width($divProduct.width() - $divProduct.find('.vis-group-user-right:eq(0)').width() - 20);
                    cartGrid.resize();
                    dRelatedGrid.resize();
                });
            });

            $btnCreateProd.on("click", function (e) {

                $bsyDiv[0].style.visibility = "visible";
                //if ($queryBy.val() == "T") {
                //    if ($leftTreeDiv.find("span").hasClass("k-state-selected")) {

                //    }
                //    else {
                //        VIS.ADialog.error("VA005_SelectParent");
                //        $bsyDiv[0].style.visibility = "hidden";
                //        return false;
                //    }
                //}
                //else {
                //    VIS.ADialog.error("VA005_SelectParent");
                //    $bsyDiv[0].style.visibility = "hidden";
                //    return false;
                //}
                cmbCat.val(parentCat);
                cmbTaxCategory.val(parentTax);
                ch = new VIS.ChildDialog();
                ch.setContent($maindiv);
                $maindiv.show();
                //ch.setHeight(470);
                ch.setWidth(600);
                ch.setTitle(VIS.Msg.getMsg("VA005_AddProduct"));
                ch.setModal(true);
                //Ok Button Click
                //  ch.onOkClick =

                //Disposing Everything on Close
                ch.onClose = function () {
                    ClearProdData();
                };
                ch.show();
                prodevents();

                $bsyDiv[0].style.visibility = "hidden";
            });

            btnCopy.on("click", function (e) {

                if (prods.length == 0) {
                    VIS.ADialog.error("VA005_SelectProduct");
                    return false;
                }
                if (prods.length > 1) {
                    return false;
                }
                $bsyDiv[0].style.visibility = "visible";
                M_Product_ID = 0;
                fillProductDetails();
                ch = new VIS.ChildDialog();
                ch.setContent($maindiv);
                $maindiv.show();
                //ch.setHeight(470);
                ch.setWidth(600);
                ch.setTitle(VIS.Msg.getMsg("VA005_AddProduct"));
                ch.setModal(true);
                //Ok Button Click
                //  ch.onOkClick =

                //Disposing Everything on Close
                ch.onClose = function () {
                    ClearProdData();
                };
                ch.show();
                prodevents();

                $bsyDiv[0].style.visibility = "hidden";
            });

            btnEditDetail.on("click", function (e) {
                $bsyDiv[0].style.visibility = "visible";
                M_Product_ID = prods[0];

                fillProductDetails();
                ch = new VIS.ChildDialog();
                ch.setContent($maindiv);
                $maindiv.show();
                //ch.setHeight(470);
                ch.setWidth(600);
                ch.setTitle(VIS.Msg.getMsg("VA005_AddProduct"));
                ch.setModal(true);
                //Ok Button Click
                //  ch.onOkClick =

                //Disposing Everything on Close
                ch.onClose = function () {
                    ClearProdData();
                };
                ch.show();
                prodevents();

                $bsyDiv[0].style.visibility = "hidden";
            });

            btnImage.on("click", function (e) {

                $rightPanel.hide();
                ImagePanel.show();
                $self.GetImages();
            });

            btnImgCancel.on("click", function (e) {

                $rightPanel.show();
                ImagePanel.hide();
            });

            ProductImgul.on("click", "li", function () {

                if ($(this).hasClass('VA005-highlighted')) {
                    $(this).removeClass('VA005-highlighted');
                    selectedimg.splice(selectedimg.indexOf($(this).attr('filename')), 1);
                }
                else {
                    $(this).addClass('VA005-highlighted');
                    selectedimg.push($(this).attr('filename'));
                }
            });

            btnPriceList.on("click", function (e) {

                if (prods.length == 0) {
                    VIS.ADialog.error("VA005_SelectProduct");
                    return false;
                }
                $bsyDiv[0].style.visibility = "visible";
                pl = new VIS.ChildDialog();
                pl.setContent($divPrice);
                $divPriceMain.show();
                pl.setHeight(500);
                pl.setWidth(853);
                pl.setTitle(VIS.Msg.getMsg("VA005_UpdatePrice"));
                pl.setModal(true);
                //Ok Button Click
                //  ch.onOkClick =

                //Disposing Everything on Close
                pl.onClose = function () {
                    cmbPriceList.val(0);
                    dGrid.clear();
                    $ImageZoom.fadeOut("slow");
                };
                pl.show();
                LoadPriceList();
                priceEvents();
            });

            btnSupplier.on("click", function (e) {

                if (prods.length == 0) {
                    VIS.ADialog.error("VA005_SelectProduct");
                    return false;
                }
                $bsyDiv[0].style.visibility = "visible";
                ml = new VIS.ChildDialog();
                ml.setContent($divSupp);
                $divSupplier.show();
                ml.setHeight(500);
                ml.setWidth(703);
                ml.setTitle(VIS.Msg.getMsg("VA005_UpdateSupplier"));
                ml.setModal(true);
                //Ok Button Click
                //  ch.onOkClick =

                //Disposing Everything on Close
                ml.onClose = function () {
                    cmbSupplier.val(0);
                    sGrid.clear();
                };
                ml.show();
                LoadSupplier();
                supplierEvents();
            });

            btnEditMultiple.on("click", function (e) {

                if (prods.length == 0) {
                    VIS.ADialog.error("VA005_SelectProduct");
                    return false;
                }
                var update = new VA005.updateProductPanel($self.windowNo, 140, 208, prods);
                update.onClose = function () {
                    if (update.okBtnPressed) {
                        prods = [];
                        uoms = [];
                        $divProduct.find('.vis-group-user-wrap').remove();
                        loadProd($searchProduct.val(), pcat_ID, $queryCat.val(), 1, PAGESIZE, 0);
                    }
                    update.dispose();
                };
                update.show();
            });

            btnShowAll.on("click", function (e) {
                clearRightPanel();
                pgno = 1;
                $queryCat.val(0);
                $searchProduct.val("");
                pcat_ID = 0;
                btnSelectAll.prop("checked", false);
                catSpan.hide();
                prods = [];
                // Manish 10/4/2016
                if (attrBottomValuesFlag == true) {
                    $btnAttValuesDivClose.trigger("click");
                }
                // end
                $divProduct.find('.vis-group-user-wrap').remove();
                loadProd("", 0, 0, pgno, PAGESIZE, 0);
            });

            btnSelectAll.on("click", function (e) {

                window.setTimeout(function () {
                    if (btnSelectAll.is(":checked")) {
                        prods = [];
                        for (var item = 0; item < $divProduct.find('.vis-group-user-wrap').length; item++) {
                            prods.push($($divProduct.find('.vis-group-user-wrap')[item]).data("uid"));
                        }
                        $divProduct.find('.vis-group-user-wrap').addClass('vis-group-selected-op vis-group-selected-opbackground');
                        $divProduct.find('.VA005-checkbox').prop("checked", true);
                        clearRightPanel();
                        //loadProd("", 0, 0, pgno, PAGESIZE);
                    }
                    else {
                        prods = [];
                        $divProduct.find('.vis-group-user-wrap').removeClass('vis-group-selected-op vis-group-selected-opbackground');
                        $divProduct.find('.VA005-checkbox').prop("checked", false);
                    }
                    // Manish 10/4/2016
                    if (attrBottomValuesFlag == true) {
                        $btnAttValuesDivClose.trigger("click");
                    }
                    // end
                }, 200);
            });

            btnAddCart.on("click", function (e) {
                if (!isClick) {
                    if (cmbCart.val() == 0) {
                        VIS.ADialog.error("VA005_selectCart");
                        return;
                    }

                    if (prods.length != 0 || attributes.length != 0 || cons.length != 0) {
                        $bsyDiv[0].style.visibility = "visible";
                        isClick = true;
                        btnAddCart.find('span').css('color', 'lime');
                        window.setTimeout(function () {
                            btnAddCart.find('span').css('color', '#616364');
                        }, 1000);
                        AddToCart("A");
                        //$bsyDiv[0].style.visibility = "hidden";
                    }
                }
            });

            btnDetails.on("click", function (e) {

                if (prods.length == 0) {
                    VIS.ADialog.error("VA005_SelectProduct");
                    return false;
                }
                else if (prods.length > 1) {
                    return false;
                }
                $divHeadVarient.find("li").removeClass("VA005-selectedTab");
                btnDetails.addClass("VA005-selectedTab");
                clearVarient();
                divVarient.hide();
                cancelConversion();
                divUom.hide();
                $divUomGroup.hide();
                cons = [];
                c_UomConv_ID = 0;
                attributes = [];
                attrArray = {};
                m_attribute_ID = 0;
                $divVarient.find(".VA005-checkbox").prop("checked", false);
                $divVarient.find(".VA005-uom-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
                $divUomGroup.find(".VA005-checkbox").prop("checked", false);
                $divUomGroup.find(".VA005-uom-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
                $divVarient.hide();
                $divLeftTree.hide();
                divCart.hide();
                divCartList.show();
                divNewCart.hide();
                $divCartdata.hide();
                $divRelated.hide();
                $divProdDetail.show();
                //$divProductDet.show();
                //$ProductDetails.show();
                LoadProductDetails();
            });

            btnVarient.on("click", function (e) {

                if (prods.length == 0) {
                    VIS.ADialog.error("VA005_SelectProduct");
                    return false;
                }
                else if (prods.length > 1) {
                    return false;
                }
                if (btnVarient.hasClass("VA005-selectedTab")) {

                }
                else {
                    $divHeadVarient.find("li").removeClass("VA005-selectedTab");
                    btnVarient.addClass("VA005-selectedTab");
                    cancelConversion();
                    divUom.hide();
                    $divUomGroup.hide();
                    cons = [];
                    c_UomConv_ID = 0;
                    $divUomGroup.find(".VA005-checkbox").prop("checked", false);
                    $divUomGroup.find(".VA005-uom-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
                    divCart.hide();
                    divCartList.show();
                    divNewCart.hide();
                    $divCartdata.hide();
                    $divProdDetail.hide();
                    //$divProductDet.hide();
                    //$ProductDetails.hide();
                    $divRelated.hide();
                    divVarient.show();
                    $divVarient.show();
                    LoadVarients();
                }
            });

            btnUom.on("click", function (e) {

                if (prods.length == 0) {
                    VIS.ADialog.error("VA005_SelectProduct");
                    return false;
                }
                else if (prods.length > 1) {
                    return false;
                }
                if (btnUom.hasClass("VA005-selectedTab")) {

                }
                else {
                    $divHeadVarient.find("li").removeClass("VA005-selectedTab");
                    btnUom.addClass("VA005-selectedTab");
                    clearVarient();
                    divVarient.hide();
                    attributes = [];
                    attrArray = {};
                    m_attribute_ID = 0;
                    $divVarient.find(".VA005-checkbox").prop("checked", false);
                    $divVarient.find(".VA005-uom-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
                    $divVarient.hide();
                    $divLeftTree.hide();
                    divCart.hide();
                    divCartList.show();
                    divNewCart.hide();
                    $divCartdata.hide();
                    $divProdDetail.hide();
                    //$divProductDet.hide();
                    //$ProductDetails.hide();
                    $divRelated.hide();
                    divUom.show();
                    $divUomGroup.show();
                    LoadUomGroup();
                }
            });

            btnRelated.on("click", function (e) {

                if (prods.length == 0) {
                    VIS.ADialog.error("VA005_SelectProduct");
                    return false;
                }
                else if (prods.length > 1) {
                    return false;
                }
                if (btnRelated.hasClass("VA005-selectedTab")) {

                }
                else {
                    $divHeadVarient.find("li").removeClass("VA005-selectedTab");
                    btnRelated.addClass("VA005-selectedTab");
                    cancelConversion();
                    divUom.hide();
                    $divUomGroup.hide();
                    clearVarient();
                    divVarient.hide();
                    $divVarient.hide();
                    attrArray = {};
                    $divLeftTree.hide();
                    $divProdDetail.hide();
                    //$divProductDet.hide();
                    //$ProductDetails.hide();
                    divCart.hide();
                    $divCartdata.hide();
                    $divRelated.show();
                    $divRelated.css("height", "92%");
                    bindRelatedGrid();
                }
            });

            btnCart.on("click", function (e) {

                //if (!window.VAICNT) {
                //    VIS.ADialog.error("VA005_InventoryCount");
                //    return;
                //}                
                if (btnCart.hasClass("VA005-selectedTab")) {

                }
                else {
                    $divHeadVarient.find("li").removeClass("VA005-selectedTab");
                    btnCart.addClass("VA005-selectedTab");
                    cancelConversion();
                    divUom.hide();
                    $divUomGroup.hide();
                    clearVarient();
                    divVarient.hide();
                    $divVarient.hide();
                    attrArray = {};
                    $divLeftTree.hide();
                    $divProdDetail.hide();
                    //$divProductDet.hide();
                    //$ProductDetails.hide();
                    $divRelated.hide();
                    divCart.show();
                    $divCartdata.show();
                    $divCartdata.css("height", "75%");
                    cartGrid.resize();
                }
            });

            btnAdduom.on("click", function (e) {
                divConversion.show();
                btnAdduom.hide();
                c_UomConv_ID = 0;
                $divUomGroup.css("height", "55%");
            });

            btnAddVarient.on("click", function (e) {
                if (attrSetId > 0) {
                    divAttr.show();
                    btnAddVarient.hide();
                    btnGenerate.hide();
                    $divVarient.css("height", "75%");
                }
            });

            btnSaveAttr.on("click", function (e) {

                for (var i = 0; i < attr.length; i++) {
                    var p = attr[i];
                    if (attrArray[p]) {
                        attrArray[p] = attrArray[p].join(',');
                    }
                }

                if (attrArray) {
                    $bsyDiv[0].style.visibility = "visible";
                    $.ajax({
                        type: "POST",
                        url: VIS.Application.contextUrl + "VA005/ProductManagement/SaveAttribute",
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify({ id: prods[0], AttributeSet: attrSetId, ColumnName: attr, ColumnValue: attrArray }),
                        success: function (data) {
                            var returnValue = data.result;

                            if (returnValue == "" || returnValue == null) {
                                attrArray = {};
                                $divLeftTree.hide();
                                LoadVarients();
                                $divVarient.show();
                            }
                            else {
                                attrArray = {};
                                divtree.find(".VA005-checkbox").prop("checked", false);
                                VIS.ADialog.error(returnValue);
                            }
                            $bsyDiv[0].style.visibility = "hidden";
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            // 
                            attrArray = {};
                            divtree.find(".VA005-checkbox").prop("checked", false);
                            console.log(textStatus);
                            $bsyDiv[0].style.visibility = "hidden";
                            alert("RecordNotSaved");
                            return;
                        }
                    });
                }
            });

            btnCancelGen.on("click", function (e) {
                attrArray = {};
                divVarient.show();
                $divVarient.show();
                $divLeftTree.hide();
            });

            btnGenerate.on("click", function (e) {
                $divVarient.hide();
                $divLeftTree.show();
                GenerateTree();
            });

            btnCancelVarient.on("click", function (e) {
                divAttr.hide();
                $divLeftTree.hide();
                btnAddVarient.show();
                btnGenerate.show();
                $divVarient.show();
                $divVarient.css("height", "85%");
            });

            btnSaveUom.on("click", function (e) {
                if (cmbUomTo.val() == -1) {
                    VIS.ADialog.error("VA005_SelectUom");
                    return;
                }
                saveConversion();
            });

            btnCancelUom.on("click", function (e) {
                cancelConversion();
            });

            btnNewCart.on("click", function (e) {
                divCartList.hide();
                divNewCart.show();
            });

            btnEditCart.on("click", function (e) {
                zoomToWindow(VIS.Utility.Util.getValueOfInt(cmbCart.val()), "VAICNT_InventoryCount");
            });

            btnRefreshCart.on("click", function (e) {
                LoadCart();
            });

            btnSaveScan.on("click", function (e) {
                if (VIS.Utility.Util.getValueOfString(txtScan.val()) == "") {
                    VIS.ADialog.error("VA005_EnterScanName");
                    return;
                }
                else {
                    saveInventoryCount();
                }
            });

            btnSaveScan.on("keydown", function (e) {
                if (e.keyCode == 13) {
                    if (VIS.Utility.Util.getValueOfString(txtScan.val()) == "") {
                        VIS.ADialog.error("VA005_EnterScanName");
                        return;
                    }
                    else {
                        saveInventoryCount();
                    }
                }
            });

            btnCancelScan.on("click", function (e) {
                divNewCart.hide();
                divCartList.show();
                txtScan.val("");
            });

            btnCancelScan.on("keydown", function (e) {
                if (e.keyCode == 13) {
                    divNewCart.hide();
                    divCartList.show();
                    txtScan.val("");
                }
            });

            btnPrint.on("click", function (e) {
                var selection = cartGrid.getSelection();
                if (selection.length > 0) {
                    $bsyDiv[0].style.visibility = "visible";
                    printData = [];
                    for (item in selection) {
                        printData.push(cartGrid.get(selection[item]));
                    }
                    pr = new VIS.ChildDialog();
                    pr.setContent($divPrint);
                    $divPrint.show();
                    //pr.setHeight(300);
                    pr.setWidth(300);
                    pr.setTitle(VIS.Msg.getMsg("VA005_PrintStickers"));
                    pr.setModal(true);
                    pr.onClose = function () {
                        ClearPrintData();
                    };
                    pr.show();
                    LoadVersions();
                    printevents();
                    $bsyDiv[0].style.visibility = "hidden";
                }
                else {
                    VIS.ADialog.info("NoRecordsSelected");
                }
            });

            cmbCart.on("change", function () {
                cart = cmbCart.val();
                if (cmbCart.val() == 0) {
                    $divHeadProd.find("#VA005_cartInfo_" + $self.windowNo).text(VIS.Msg.getMsg("None"));
                }
                else {
                    $divHeadProd.find("#VA005_cartInfo_" + $self.windowNo).text(cmbCart.find('option:selected').text());
                }
                BindCartGrid();
            });

            //txtMul.on("keydown", function (event) {
            //     
            //    if (event.keyCode == 189 || event.keyCode == 109 || event.keyCode == 173) { // dash (-)
            //        if (event.keyCode == 189 && this.value.length == 0) {
            //            return true;
            //        }
            //        this.value = Number(this.value * -1);
            //        setTimeout(function () {
            //            $ctrl.trigger("change");
            //        }, 100);
            //        return false;
            //    }
            //    if (event.shiftKey) {
            //        return false;
            //    }
            //    if ((event.keyCode >= 48 && event.keyCode <= 57 && event.shiftKey == false) || (event.keyCode >= 96 && event.keyCode <= 105 && event.shiftKey == false)) {

            //    }
            //    else if (event.keyCode != 9 && event.keyCode != 8 && event.keyCode != 46) {
            //        return false;
            //    }
            //});

            cmbUomTo.on("change", function (event) {
                if (cmbUomTo.val() > 0) {
                    btnSaveUom.removeClass('VA005-disabled');
                }
                else {
                    btnSaveUom.removeClass('VA005-disabled');
                }
            });

            // Added by Shifali on 3rd July 2020 to change the date and amount acc. to culture
            this.vetoablechange = function (evt) {
                if (evt.propertyName == "MulAmount") {
                    $txtMul.setValue(evt.newValue);
                    var rate1 = evt.newValue;
                    var rate2 = VIS.Env.ZERO;
                    var one = 1.0;

                    if (VIS.Utility.Util.getValueOfDouble(rate1) != 0.0)	//	no divide by zero
                    {
                        rate2 = (one / rate1).toFixed(12);
                    }
                    uomChange = true;
                    $txtDiv.setValue(rate2);
                }
                else if (evt.propertyName == "DivAmount") {
                    $txtDiv.setValue(evt.newValue);
                    if (uomChange) {
                        uomChange = false;
                        return;
                    }
                    var rate1 = evt.newValue;
                    var rate2 = VIS.Env.ZERO;
                    var one = 1.0;

                    if (VIS.Utility.Util.getValueOfDouble(rate1) != 0.0)	//	no divide by zero
                    {
                        //rate2 = Decimal.Round(Decimal.Divide(one, rate1), 12);// MidpointRounding.AwayFromZero);  //By Sarab
                        rate2 = (one / rate1).toFixed(12);
                    }
                    $txtMul.setValue(rate2);
                }
            };

            //txtMul.on("keyup", function (event) {
            //    var rate1 = this.value;
            //    var rate2 = VIS.Env.ZERO;
            //    var one = 1.0;

            //    if (VIS.Utility.Util.getValueOfDouble(rate1) != 0.0)	//	no divide by zero
            //    {
            //        //rate2 = Decimal.Round(Decimal.Divide(one, rate1), 12);// MidpointRounding.AwayFromZero);  //By Sarab
            //        rate2 = (one / rate1).toFixed(12);
            //    }
            //    uomChange = true;
            //    txtDiv.val(rate2);
            //});

            //txtMul.on("change", function (event) {
            //    var rate1 = this.value;
            //    var rate2 = VIS.Env.ZERO;
            //    var one = 1.0;

            //    if (VIS.Utility.Util.getValueOfDouble(rate1) != 0.0)	//	no divide by zero
            //    {
            //        rate2 = (one / rate1).toFixed(12);
            //    }
            //    uomChange = true;
            //    txtDiv.val(rate2);
            //});

            //txtDiv.on("keyup", function (event) {
            //    if (uomChange) {
            //        uomChange = false;
            //        return;
            //    }
            //    var rate1 = this.value;
            //    var rate2 = VIS.Env.ZERO;
            //    var one = 1.0;

            //    if (VIS.Utility.Util.getValueOfDouble(rate1) != 0.0)	//	no divide by zero
            //    {
            //        rate2 = (one / rate1).toFixed(12);
            //    }
            //    txtMul.val(rate2);
            //});

            //txtDiv.on("change", function (event) {
            //    if (uomChange) {
            //        uomChange = false;
            //        return;
            //    }
            //    var rate1 = this.value;
            //    var rate2 = VIS.Env.ZERO;
            //    var one = 1.0;

            //    if (VIS.Utility.Util.getValueOfDouble(rate1) != 0.0)	//	no divide by zero
            //    {
            //        //rate2 = Decimal.Round(Decimal.Divide(one, rate1), 12);// MidpointRounding.AwayFromZero);  //By Sarab
            //        rate2 = (one / rate1).toFixed(12);
            //    }
            //    txtMul.val(rate2);
            //});
        };

        function AddToCart(source) {

            var ctrl = source;
            var Recid = 0;
            if (ctrl == "U") {
                // Done by Bharat on 05 March 2018 to move queries to server side
                var dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadUOMConversionData", { "C_UOM_Conversion_ID": c_UomConv_ID });
                if (dr != null) {
                    multiValues.push(
                        {
                            recid: Recid,
                            product_ID: dr["M_Product_ID"],
                            Product: dr["Name"],
                            C_Uom_ID: dr["C_UOM_To_ID"],
                            attribute_ID: 0,
                            Attribute: "",
                            UPC: dr["UPC"],
                            Qty: 1
                        });
                }
                //sqlaa = "SELECT prd.Name,prd.M_Product_ID,uc.C_UOM_To_ID,uc.UPC FROM C_UOM_Conversion uc INNER JOIN M_Product prd ON uc.M_Product_ID = prd.M_Product_ID WHERE uc.C_UOM_Conversion_ID = " + c_UomConv_ID;
                //ds = VIS.DB.executeDataSet(sqlaa.toString(), null, null);
                //if (ds != null && ds.tables[0].rows.length > 0) {
                //    for (var i = 0; i < ds.tables[0].rows.length; i++) {
                //        Recid = Recid + 1;
                //        multiValues.push(
                //        {
                //            recid: Recid,
                //            product_ID: ds.tables[0].rows[i].cells.m_product_id,
                //            Product: ds.tables[0].rows[i].cells.name,
                //            C_Uom_ID: VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells.c_uom_to_id),
                //            attribute_ID: 0,
                //            Attribute: "",
                //            UPC: VIS.Utility.Util.getValueOfString(ds.tables[0].rows[i].cells.upc),
                //            Qty: 1
                //        });
                //    }
                //}
            }
            else if (ctrl == "V") {
                if (upcvalue == "") {
                    upcvalue = " ";
                }
                // Done by Bharat on 05 March 2018 to move queries to server side
                var dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadProdAttributeData", { "M_Product_ID": prods[0], "M_AttributeSetInstance_ID": m_attribute_ID, "UPC": upcvalue });
                if (dr != null) {
                    multiValues.push(
                        {
                            recid: Recid,
                            product_ID: dr["M_Product_ID"],
                            Product: dr["Name"],
                            C_Uom_ID: dr["C_UOM_ID"],
                            attribute_ID: dr["M_AttributeSetInstance_ID"],
                            Attribute: dr["Description"],
                            UPC: dr["UPC"],
                            Qty: 1
                        });
                }

                //sqlaa = "SELECT prd.Name,prd.M_Product_ID,prd.C_UOM_ID,patr.M_AttributeSetInstance_ID,ats.Description,patr.UPC FROM M_ProductAttributes patr INNER JOIN M_Product prd ON patr.M_Product_ID = prd.M_Product_ID " +
                //        "INNER JOIN M_AttributeSetInstance ats ON patr.M_AttributeSetInstance_ID = ats.M_AttributeSetInstance_ID WHERE patr.M_Product_ID = " + prods[0] + " AND patr.M_AttributeSetInstance_ID = " + m_attribute_ID + " AND nvl(patr.UPC,' ') = '" + upcvalue + "'";
                //ds = VIS.DB.executeDataSet(sqlaa.toString(), null, null);
                //if (ds != null && ds.tables[0].rows.length > 0) {
                //    for (var i = 0; i < ds.tables[0].rows.length; i++) {
                //        Recid = Recid + 1;
                //        multiValues.push(
                //        {
                //            recid: Recid,
                //            product_ID: ds.tables[0].rows[i].cells.m_product_id,
                //            Product: ds.tables[0].rows[i].cells.name,
                //            C_Uom_ID: VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells.c_uom_id),
                //            attribute_ID: VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells.m_attributesetinstance_id),
                //            Attribute: ds.tables[0].rows[i].cells.description,
                //            UPC: VIS.Utility.Util.getValueOfString(ds.tables[0].rows[i].cells.upc),
                //            Qty: 1
                //        });
                //    }
                //}
            }
            else if (ctrl == "P") {
                // Done by Bharat on 05 March 2018 to move queries to server side
                var dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadProductCartData", { "M_Product_ID": ProdID });
                if (dr != null) {
                    multiValues.push(
                        {
                            recid: Recid,
                            product_ID: ProdID,
                            Product: dr["Name"],
                            C_Uom_ID: dr["C_UOM_ID"],
                            attribute_ID: dr["M_AttributeSetInstance_ID"],
                            Attribute: dr["Description"],
                            UPC: dr["UPC"],
                            Qty: 1
                        });
                }

                //sqlaa = "SELECT prd.Name,prd.C_UOM_ID,prd.M_AttributeSetInstance_ID,ats.Description,prd.UPC FROM M_Product prd LEFT OUTER JOIN M_AttributeSetInstance ats " +
                //    "ON prd.M_AttributeSetInstance_ID = ats.M_AttributeSetInstance_ID WHERE prd.M_Product_ID = " + ProdID;
                //ds = VIS.DB.executeDataSet(sqlaa.toString(), null, null);
                //if (ds != null && ds.tables[0].rows.length > 0) {
                //    for (var i = 0; i < ds.tables[0].rows.length; i++) {
                //        Recid = Recid + 1;
                //        multiValues.push(
                //        {
                //            recid: Recid,
                //            product_ID: ProdID,
                //            Product: ds.tables[0].rows[i].cells.name,
                //            C_Uom_ID: VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells.c_uom_id),
                //            attribute_ID: VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells.m_attributesetinstance_id),
                //            Attribute: ds.tables[0].rows[i].cells.description,
                //            UPC: VIS.Utility.Util.getValueOfString(ds.tables[0].rows[i].cells.upc),
                //            Qty: 1
                //        });
                //    }
                //}
            }
            else if (ctrl == "A") {
                if (cons.length > 0) {
                    for (var item in cons) {
                        // Done by Bharat on 05 March 2018 to move queries to server side
                        var dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadUOMConversionData", { "C_UOM_Conversion_ID": cons[item] });
                        if (dr != null) {
                            multiValues.push(
                                {
                                    recid: Recid,
                                    product_ID: dr["M_Product_ID"],
                                    Product: dr["Name"],
                                    C_Uom_ID: dr["C_UOM_To_ID"],
                                    attribute_ID: 0,
                                    Attribute: "",
                                    UPC: dr["UPC"],
                                    Qty: 1
                                });
                        }

                        //sqlaa = "SELECT prd.Name,prd.M_Product_ID,uc.C_UOM_To_ID,uc.UPC FROM C_UOM_Conversion uc INNER JOIN M_Product prd ON uc.M_Product_ID = prd.M_Product_ID WHERE uc.C_UOM_Conversion_ID = " + cons[item];
                        //ds = VIS.DB.executeDataSet(sqlaa.toString(), null, null);
                        //if (ds != null && ds.tables[0].rows.length > 0) {
                        //    for (var i = 0; i < ds.tables[0].rows.length; i++) {
                        //        Recid = Recid + 1;
                        //        multiValues.push(
                        //        {
                        //            recid: Recid,
                        //            product_ID: ds.tables[0].rows[i].cells.m_product_id,
                        //            Product: ds.tables[0].rows[i].cells.name,
                        //            C_Uom_ID: VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells.c_uom_to_id),
                        //            attribute_ID: 0,
                        //            Attribute: "",
                        //            UPC: VIS.Utility.Util.getValueOfString(ds.tables[0].rows[i].cells.upc),
                        //            Qty: 1
                        //        });
                        //    }
                        //}
                    }
                }
                else if (attributes.length > 0) {
                    for (var item in attributes) {
                        if (upcs[item] == "") {
                            upcs[item] = " ";
                        }
                        // Done by Bharat on 05 March 2018 to move queries to server side
                        var dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadProdAttributeData", { "M_Product_ID": prods[0], "M_AttributeSetInstance_ID": attributes[item], "UPC": upcs[item] });
                        if (dr != null) {
                            multiValues.push(
                                {
                                    recid: Recid,
                                    product_ID: dr["M_Product_ID"],
                                    Product: dr["Name"],
                                    C_Uom_ID: dr["C_UOM_ID"],
                                    attribute_ID: dr["M_AttributeSetInstance_ID"],
                                    Attribute: dr["Description"],
                                    UPC: dr["UPC"],
                                    Qty: 1
                                });
                        }

                        //sqlaa = "SELECT prd.Name,prd.M_Product_ID,prd.C_UOM_ID,patr.M_AttributeSetInstance_ID,ats.Description,patr.UPC FROM M_ProductAttributes patr INNER JOIN M_Product prd ON patr.M_Product_ID = prd.M_Product_ID " +
                        //    "INNER JOIN M_AttributeSetInstance ats ON patr.M_AttributeSetInstance_ID = ats.M_AttributeSetInstance_ID WHERE patr.M_Product_ID = " + prods[0] + " AND patr.M_AttributeSetInstance_ID = " + attributes[item] + " AND NVL(patr.UPC,' ') = '" + upcs[item] + "'";
                        //ds = VIS.DB.executeDataSet(sqlaa.toString(), null, null);
                        //if (ds != null && ds.tables[0].rows.length > 0) {
                        //    for (var i = 0; i < ds.tables[0].rows.length; i++) {
                        //        Recid = Recid + 1;
                        //        multiValues.push(
                        //        {
                        //            recid: Recid,
                        //            product_ID: ds.tables[0].rows[i].cells.m_product_id,
                        //            Product: ds.tables[0].rows[i].cells.name,
                        //            C_Uom_ID: VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells.c_uom_id),
                        //            attribute_ID: VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells.m_attributesetinstance_id),
                        //            Attribute: ds.tables[0].rows[i].cells.description,
                        //            UPC: VIS.Utility.Util.getValueOfString(ds.tables[0].rows[i].cells.upc),
                        //            Qty: 1
                        //        });
                        //    }
                        //}
                    }
                }
                else if (prods.length > 0) {
                    for (var item in prods) {
                        // Done by Bharat on 05 March 2018 to move queries to server side
                        var dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadProductCartData", { "M_Product_ID": prods[item] });
                        if (dr != null) {
                            multiValues.push(
                                {
                                    recid: Recid,
                                    product_ID: prods[item],
                                    Product: dr["Name"],
                                    C_Uom_ID: dr["C_UOM_ID"],
                                    attribute_ID: dr["M_AttributeSetInstance_ID"],
                                    Attribute: dr["Description"],
                                    UPC: dr["UPC"],
                                    Qty: 1
                                });
                        }

                        //sqlaa = "SELECT prd.Name,prd.C_UOM_ID,prd.M_AttributeSetInstance_ID,ats.Description,prd.UPC FROM M_Product prd LEFT OUTER JOIN M_AttributeSetInstance ats " +
                        //    "ON prd.M_AttributeSetInstance_ID = ats.M_AttributeSetInstance_ID WHERE prd.M_Product_ID = " + prods[item];
                        //ds = VIS.DB.executeDataSet(sqlaa.toString(), null, null);
                        //if (ds != null && ds.tables[0].rows.length > 0) {
                        //    for (var i = 0; i < ds.tables[0].rows.length; i++) {
                        //        Recid = Recid + 1;
                        //        multiValues.push(
                        //        {
                        //            recid: Recid,
                        //            product_ID: prods[item],
                        //            Product: ds.tables[0].rows[i].cells.name,
                        //            C_Uom_ID: VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells.c_uom_id),
                        //            attribute_ID: VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells.m_attributesetinstance_id),
                        //            Attribute: ds.tables[0].rows[i].cells.description,
                        //            UPC: VIS.Utility.Util.getValueOfString(ds.tables[0].rows[i].cells.upc),
                        //            Qty: 1
                        //        });
                        //    }
                        //}
                    }
                }
            }
            saveInventory();
        };

        function saveConversion() {

            $bsyDiv[0].style.visibility = "visible";
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "VA005/ProductManagement/SaveConversion",
                dataType: "json",
                data: {
                    id: prods[0],
                    Conv_ID: c_UomConv_ID,
                    Mul: VIS.Utility.Util.getValueOfDouble($txtMul.getValue()),
                    Div: VIS.Utility.Util.getValueOfDouble($txtDiv.getValue()),
                    UOM: uoms[0],
                    UomTo: VIS.Utility.Util.getValueOfInt(cmbUomTo.val()),
                    UOMUPC: VIS.Utility.Util.getValueOfString(uomUPC.val())
                },
                success: function (data) {
                    var returnValue = data.result;

                    if (returnValue == "") {
                        cancelConversion();
                        LoadUomGroup();
                        $divUomGroup.show();
                    }
                    else {
                        //Getting the message from UOM Window
                        VIS.ADialog.error("", "", returnValue, "");
                    }
                    $bsyDiv[0].style.visibility = "hidden";
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    // 
                    console.log(textStatus);
                    $bsyDiv[0].style.visibility = "hidden";
                    alert("RecordNotSaved");
                    return;
                }
            });
        };

        function cancelConversion() {
            cmbUomTo.val(-1);
            $txtMul.setValue(0.0);
            $txtDiv.setValue(0.0);
            uomUPC.val("");
            c_UomConv_ID = 0;
            divConversion.hide();
            btnAdduom.show();
            $divUomGroup.css("height", "85%");
        };

        function clearVarient() {
            $attrControl.getControl().val("");
            divAttr.hide();
            btnAddVarient.show();
        };

        function clearProductDet() {
            divZoomProdName.css("display", "none");
            prodName.text("");
            prodUPC.find("span").remove();
            prodAttributeSet.find("span").remove();
            prodType.find("span").remove();
            prodCategory.find("span").remove();
            prodTaxCat.find("span").remove();
            prodUOM.find("span").remove();
            prodParent.find("span").remove();
            divUsrImage.empty();
            divUsrImage.append($('<i style="max-height: 100%; max-width: 100%;" class="vis vis-image" ></i>'));
        };

        function clearRightPanel() {
            btnUom.removeClass("VA005-selectedTab");
            btnVarient.removeClass("VA005-selectedTab");
            btnRelated.removeClass("VA005-selectedTab");
            cons = [];
            attributes = [];
            attrArray = {};
            c_UomConv_ID = 0;
            m_attribute_ID = 0;
            $divVarient.find(".VA005-checkbox").prop("checked", false);
            $divVarient.find(".VA005-uom-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
            $divUomGroup.find(".VA005-checkbox").prop("checked", false);
            $divUomGroup.find(".VA005-uom-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
            cancelConversion();
            clearVarient();
            clearProductDet();
            divUom.hide();
            $divUomGroup.hide();
            divVarient.hide();
            $divVarient.hide();
            $divLeftTree.hide();
            $divRelated.hide();
            //$rightPanel.css("opacity", 0.6);
            //$rightPanel.css("background-color", "#f1f1f1");
        };

        function getAttributeValues(e, product_id) {
            $bsyDiv[0].style.visibility = "visible";
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "VA005/ProductManagement/GetAttributeValues",
                dataType: "json",
                async: false,
                data: {
                    product_id: product_id,
                },
                success: function (data) {
                    //$bsyDiv[0].style.visibility = "visible";
                    var returnValue = JSON.parse(data);
                    productAttValues.empty();
                    if (returnValue != null) {
                        if (returnValue.length > 0) {
                            var attributeIDs = 0;
                            var nameDiff = "";
                            var getDIvProduct = null;
                            var mainCreateDiv = null;
                            for (var i = 0; i < returnValue.length; i++) {
                                var src = returnValue[i]["imageurl"];
                                if (i == 0) {
                                    nameDiff = $('<div class="VA005-AttributesValuesBtm VA005-AttValues-Btm"><div><span class="glyphicon glyphicon-minus VA005-vis-plus"></span><p style="color:rgba(var(--v-c-primary), 1);font-weight:bold;">' + returnValue[i]["AttributeName"] + '</p></div></div>');
                                    productAttValues.append(nameDiff.append($('<div class="VA005-getDiv_' + i + ' VA005-AttValofbindingImg" style="width:100%;">')));
                                    getDIvProduct = productAttValues.find(".VA005-getDiv_" + i + "");
                                    nameDiff.find(".VA005-vis-plus").on("click", function () {
                                        attValueExpandcollapse($(this), $($(this).parents()[1]).find(".VA005-AttValofbindingImg"));
                                    });
                                }
                                else if (attributeIDs != returnValue[i]["attributeID"]) {
                                    nameDiff = $('<div class="VA005-AttributesValuesDown VA005-AttValues-Btm"><div><span class="glyphicon glyphicon-minus VA005-vis-plus" style="float: left; margin: 2px 5px 0 0; font-size: 12px;"></span><p style="color:rgba(var(--v-c-primary), 1);font-weight:bold;">' + returnValue[i]["AttributeName"] + '</p></div></div>');
                                    productAttValues.append(nameDiff.append($('<div class="VA005-getDiv_' + i + ' VA005-AttValofbindingImg" style="width:100%;">')));
                                    getDIvProduct = productAttValues.find(".VA005-getDiv_" + i + "");
                                    nameDiff.find(".VA005-vis-plus").on("click", function () {
                                        attValueExpandcollapse($(this), $($(this).parents()[1]).find(".VA005-AttValofbindingImg"));
                                    });
                                }
                                if (src == "") {
                                    src = VIS.Application.contextUrl + "Areas/VA005/Images/img-defult.png";
                                    mainCreateDiv = $('<div class="vis-group-user-img VA005-AttributesValuesImgTop"><img style="height: 32px;width: 32px;" class="VA005-Image-AttValImg" src="' + src + '" data-uid="' + product_id + '" data-attimages_id="' + returnValue[i]["attimages"] + '"  data-attvalID="' + returnValue[i]["AttributeValueID"] + '"  alt="prod-img"><p class="VA005-Image-AttValPar">' + returnValue[i]["attValueName"] + '</p></div>');
                                }
                                else {
                                    var dttime = new Date();
                                    var timedt = dttime.getTime();
                                    src = VIS.Application.contextUrl + src + "?" + timedt;
                                    mainCreateDiv = $('<div class="vis-group-user-img VA005-AttributesValuesImgTop"><img class="VA005-Image-AttValImg" src="' + src + '" data-uid="' + product_id + '" data-attimages_id="' + returnValue[i]["attimages"] + '"  data-attvalID="' + returnValue[i]["AttributeValueID"] + '"  alt="prod-img"><p class="VA005-Image-AttValPar">' + returnValue[i]["attValueName"] + '</p></div>');
                                }

                                getDIvProduct.append(mainCreateDiv);
                                attributeIDs = returnValue[i]["attributeID"];
                            }
                        }
                    }
                    if (returnValue.length == 0 || returnValue == null) {
                        var recNotFound = $('<span style="float: left;width: 100%;text-align: center;"><p style="padding-top: 5%;">' + VIS.Msg.getMsg("TMRecordNotFound") + '</p></span>');
                        productAttValues.append(recNotFound);
                    }

                    DropOnAttributeValues(e);
                    productAttValues.parent().prepend($btnAttrValueControls);

                    $lblSelectedNodeText = $root.find("#VA005_SelectedNodeText_" + $self.windowNo);
                    $lblSelectedNodeText.text($divProduct.find(".vis-group-selected-op").find(".VA005-NameOfSelectedNode").text());

                    var selectedTextt = $lblSelectedNodeText.text();
                    selectedTextt += " " + VIS.Msg.getMsg("VA005_Attributelisting");
                    $lblSelectedNodeText.text(selectedTextt);

                    $btnDelAttValues = $btnAttrValueControls.find("#VA005_btnDeleteAttVal_" + $self.windowNo);
                    $btnAttValuesDivClose = $btnAttrValueControls.find("#VA005_btnDeleteCross_" + $self.windowNo);

                    $btnDelAttValues.on("click", function () {
                        if (productAttValues.find(".VA005-AttValueImgaeBorderSelect").length > 0) {
                            VIS.ADialog.confirm("VA005_DoyouWanttoDelete", true, "", "Confirm", function (result) {
                                if (!result) {
                                    return;
                                }
                                else {
                                    deleteAttributeValues();
                                }
                            });
                        }
                    })
                    $btnAttValuesDivClose.on("click", function () {
                        $divProduct.height($($root.parent()).height() - 95);
                        productAttValues.parent().css("display", "none");
                        $divProduct.css("border-bottom", "none");

                        midtopdivvv.find(".ui-resizable-s").css("display", "none");
                        midtopdivvv.find(".ui-resizable-se").css("display", "none");
                        midtopdivvv.css("border-bottom", "none");
                        $divProduct.height($divProduct.height() + 20);

                        attrBottomValuesFlag = false;
                    })


                    $bsyDiv[0].style.visibility = "visible";
                    var calHeight = $middlePanel.height() - ($divProduct.height() + 105);
                    productAttValues.parent().height(calHeight);

                    midtopdivvv.height($($root.parent()).height() - 400);
                    $divProduct.height($($root.parent()).height() - 400);
                    productAttValues.parent().height($middlePanel.height() - ($root.find(".vis-group-users-container").height() + 130));
                    productAttValues.height($middlePanel.height() - ($root.find(".vis-group-users-container").height() + 130));
                    productAttValues.parent().css("display", "inherit");
                    midtopdivvv.find(".ui-resizable-s").css("display", "inherit");
                    midtopdivvv.find(".ui-resizable-se").css("display", "inherit");
                    $divProduct.height($divProduct.height() - 20);
                    midtopdivvv.css("border-bottom", "2px solid rgba(var(--v-c-primary), 1)");
                    window.setTimeout(function () {
                        $bsyDiv[0].style.visibility = "hidden";
                    }, 300);
                },
                error: function () {
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });
            //productAttValues.find(".VA005-attrValuesWraperDiv").append($btnAttrValueControls);            
        };


        function deleteAttributeValues() {
            $bsyDiv[0].style.visibility = "visible";
            var seletdValues = [];
            var lenSelected = productAttValues.find(".VA005-AttValueImgaeBorderSelect");
            for (var i = 0; i < lenSelected.length; i++) {
                var getImag = $(lenSelected.find("img")[i]);
                if (getImag.attr("data-attimages_id") == "") {
                    continue;
                }
                seletdValues.push({
                    'M_AttributeValue_ID': getImag.attr("data-attvalid"),
                    'M_Product_ID': getImag.attr("data-uid"),
                    'AD_Imaeg_ID': getImag.attr("data-attimages_id"),
                });
            }

            $.ajax({
                type: "post",
                dataType: "json",
                url: VIS.Application.contextUrl + "VA005/ProductManagement/DeleteAttributeValues",
                data: { attValueImagesID: JSON.stringify(seletdValues) },
                success: function (data) {
                    var result = JSON.parse(data);
                    if (result == "") {
                        var defaultImgs = VIS.Application.contextUrl + "Areas/VA005/Images/img-defult.png";
                        var imgss = productAttValues.find(".VA005-AttValueImgaeBorderSelect").find("img");
                        imgss.attr("src", defaultImgs);
                        imgss.css({ "height": "32px", "width": "32px" });
                        productAttValues.find(".VA005-AttValueImgaeBorderSelect").removeClass("VA005-AttValueImgaeBorderSelect");
                        $btnDelAttValues.addClass("VA005-AttValOpacity");
                    }
                    $bsyDiv[0].style.visibility = "hidden";
                },
                error: function (e) {
                    console.log(e);
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });
        };



        function DropOnAttributeValues(e) {
            productAttValues.find(".VA005-AttributesValuesImgTop").droppable({
                //productAttValues.find("img").droppable({
                drop: function (event, ui) {

                    var currentImgPaths = "";
                    if ($(ui.draggable).attr("path").contains("Thumb100x100")) {
                        currentImgPaths = $(ui.draggable).attr("path").replace("Thumb100x100\\", "");
                    }
                    else {
                        currentImgPaths = $(ui.draggable).attr("path");
                    }
                    SaveAttributeValueImage(currentImgPaths, $(ui.draggable).attr("filename"), parseInt($(this).find("img").attr("data-uid")), $(this), $(this).find("img").attr("data-attvalID"), $(this).find("img").attr("data-attimages_id"));
                },
                tolerance: 'pointer'
            });
        };

        function setBackgroundColorAttValues(ctrl) {
            if (ctrl.hasClass("VA005-AttValueImgaeBorderSelect")) {
                ctrl.removeClass("VA005-AttValueImgaeBorderSelect");
            }
            else {
                ctrl.addClass("VA005-AttValueImgaeBorderSelect");
            }
        };

        function attValueExpandcollapse(current, ctrl) {
            if ($(ctrl).height() == "0") {
                $(ctrl).css("height", "auto");
                current.removeClass("vis vis-plus");
                current.addClass("glyphicon glyphicon-minus");
            }
            else {
                $(ctrl).css("height", "0px");
                current.removeClass("glyphicon glyphicon-minus");
                current.addClass("vis vis-plus");
            }
        };


        function prodContainerClick(e) {

            var chk;
            var target = $(e.target);

            if (target.hasClass("vis-image")) {
                $bsyDiv[0].style.visibility = "visible";
                attrBottomValuesFlag = true;
                btnImage.trigger("click");
                btnSelectAll.prop("checked", false);
                prods = [];
                uoms = [];
                $divProduct.find(".VA005-checkbox").prop("checked", false);
                $divProduct.find(".vis-group-user-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
                $(target.parents('.vis-group-user-wrap')).addClass('vis-group-selected-op vis-group-selected-opbackground');
                $(target.parents('.vis-group-user-wrap')).find(".VA005-checkbox").prop("checked", true);
                ProdID = $(target.parents('.vis-group-user-wrap')).data("uid");
                uomID = $(target.parents('.vis-group-user-wrap')).find(".VA005-Uom-span").data("uid");
                prods.push(ProdID);
                uoms.push(uomID);
                //return;
            }

            else if (target.hasClass('vis-edit')) {
                ProdID = target.data("uid");
                zoomToWindow(ProdID, "Product");
                $getProdectIDAfterEdit = ProdID;
                ProdID = 0;
                return;
            }
            else if (target.hasClass('fa-shopping-cart')) {
                if (!isClick) {
                    if (cmbCart.val() == 0) {
                        VIS.ADialog.error("VA005_selectCart");
                        return;
                    }
                    ProdID = target.data("uid");
                    $bsyDiv[0].style.visibility = "visible";
                    isClick = true;
                    target.css('color', 'lime');
                    window.setTimeout(function () {
                        target.css('color', '#616364');
                    }, 1000);
                    AddToCart("P");
                }
                return;
            }
            else if (target.hasClass('VA005-checkbox')) {
                ProdID = target.data("uid");
                uomID = $(target.parents('.vis-group-user-wrap')).find(".VA005-Uom-span").data("uid");
                if (target.prop("checked")) {
                    window.setTimeout(function () {
                        $(target.parents('.vis-group-user-wrap')).addClass('vis-group-selected-op vis-group-selected-opbackground');
                    }, 200);
                    prods.push(ProdID);
                    uoms.push(uomID);
                    //LoadUomGroup();
                    //LoadVarients();
                }
                else {
                    window.setTimeout(function () {
                        $(target.parents('.vis-group-user-wrap')).removeClass('vis-group-selected-op vis-group-selected-opbackground');
                    }, 200);
                    prods.splice(prods.indexOf(ProdID), 1);
                    uoms.splice(uoms.indexOf(uomID), 1);
                    //LoadUomGroup();
                    //LoadVarients();
                }
                ProdID = 0;
            }
            else if (target.hasClass('vis-group-user-wrap')) {
                btnSelectAll.prop("checked", false);
                prods = [];
                uoms = [];
                $divProduct.find(".VA005-checkbox").prop("checked", false);
                if (target.hasClass('vis-group-selected-op vis-group-selected-opbackground')) {
                    $divProduct.find(".vis-group-user-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
                    ProdID = 0;
                    uomID = target.find(".VA005-Uom-span").data("uid");
                    uoms.splice(uoms.indexOf(uomID), 1);
                    //LoadUomGroup();
                    //LoadVarients();
                }
                else {
                    $divProduct.find(".vis-group-user-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
                    target.addClass('vis-group-selected-op vis-group-selected-opbackground');
                    target.find(".VA005-checkbox").prop("checked", true);
                    ProdID = target.data("uid");
                    uomID = target.find(".VA005-Uom-span").data("uid");
                    prods.push(ProdID);
                    uoms.push(uomID);
                    //LoadUomGroup();
                    //LoadVarients();
                }
            }
            else {
                if ($(target.parents('.vis-group-user-wrap')).length > 0) {     // length will be 0 if user's search result single or more record and there is some blank space... on click blank space nothing should be happened.
                    btnSelectAll.prop("checked", false);
                    prods = [];
                    uoms = [];
                    $divProduct.find(".VA005-checkbox").prop("checked", false);
                    if ($(target.parents('.vis-group-user-wrap')).hasClass('vis-group-selected-op vis-group-selected-opbackground')) {
                        $divProduct.find(".vis-group-user-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
                        ProdID = 0;
                        uomID = $(target.parents('.vis-group-user-wrap')).find(".VA005-Uom-span").data("uid");
                        uoms.splice(uoms.indexOf(uomID), 1);
                        //LoadUomGroup();
                        //LoadVarients();
                    }
                    else {
                        $divProduct.find(".vis-group-user-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
                        $(target.parents('.vis-group-user-wrap')).addClass('vis-group-selected-op vis-group-selected-opbackground');
                        $(target.parents('.vis-group-user-wrap')).find(".VA005-checkbox").prop("checked", true);
                        ProdID = $(target.parents('.vis-group-user-wrap')).data("uid");
                        uomID = $(target.parents('.vis-group-user-wrap')).find(".VA005-Uom-span").data("uid");
                        prods.push(ProdID);
                        uoms.push(uomID);
                    }
                }
                else {
                    return;
                }
            }
            if (prods.length == 0 || prods.length > 1) {
                clearRightPanel();
                if (prods.length > 1 && attrBottomValuesFlag == true) {
                    $btnAttValuesDivClose.trigger("click");
                }
            }
            else if (!target.hasClass("vis-image")) {
                //$rightPanel.css("opacity", 1);
                //$rightPanel.css("background-color", "white");
                cancelConversion();
                if (!$divHeadVarient.find("li").hasClass("VA005-selectedTab")) {
                    btnDetails.addClass("VA005-selectedTab")
                    $divProdDetail.show();
                    //$divProductDet.show();
                    //$ProductDetails.show();
                    LoadProductDetails();
                }
                else if (btnDetails.hasClass("VA005-selectedTab")) {
                    LoadProductDetails();
                }
                else if (btnUom.hasClass("VA005-selectedTab")) {
                    LoadUomGroup();
                }
                else if (btnVarient.hasClass("VA005-selectedTab")) {
                    LoadVarients();
                    GenerateTree();
                }
                else if (btnRelated.hasClass("VA005-selectedTab")) {
                    bindRelatedGrid();
                }
            }
            window.setTimeout(function () {
                if (attrBottomValuesFlag == true) {
                    $bsyDiv[0].style.visibility = "visible";
                    if (!target.hasClass('vis-edit')) {
                        var pIDS = 0;
                        if ($($divProduct.find(".vis-group-selected-opbackground")[0]).attr("data-uid") == 0 || $($divProduct.find(".vis-group-selected-opbackground")[0]).attr("data-uid") == "") {
                            pIDS = $getProdectIDAfterEdit;
                        }
                        else {
                            pIDS = $($divProduct.find(".vis-group-selected-opbackground")[0]).attr("data-uid");
                        }
                        getAttributeValues(e, pIDS);
                    }
                }
                window.setTimeout(function () {
                    $bsyDiv[0].style.visibility = "hidden";
                }, 500);
            }, 100);
        };

        function AttributeCtrl() {
            // Done by Bharat on 05 March 2018 to move queries to server side
            var mattsetid = VIS.dataContext.getJSONRecord("VA005/ProductManagement/GetAttributeSet", prods[0]);

            //var qry = "SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + prods[0];
            //var mattsetid = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
            if (mattsetid != 0) {
                var productWindow = false;		//	HARDCODED
                var M_Locator_ID = VIS.context.getContextAsInt($self.windowNo, "M_Locator_ID");
                var C_BPartner_ID = VIS.context.getContextAsInt($self.windowNo, "C_BPartner_ID");
                var obj = new VIS.PAttributesForm(0, prods[0], M_Locator_ID, C_BPartner_ID, productWindow, AD_Column_ID, $self.windowNo);
                if (obj.hasAttribute) {
                    obj.showDialog();
                }
                obj.onClose = function (mAttributeSetInstanceId, name, mLocatorId) {
                    //setValueInControl(mAttributeSetInstanceId, name);
                    LoadVarients();
                };
            }
            else {
                return;
            }
        };

        function uomContainerClick(e) {

            var chk;
            var target = $(e.target);
            if (target.hasClass('vis-edit')) {
                c_UomConv_ID = target.attr("conversionid");
                btnAdduom.hide();
                divConversion.show();
                fillConversion(c_UomConv_ID);
            }
            else if (target.hasClass('vis-delete')) {
                var convid = target.attr("conversionid");
                deleteConversion(convid);
            }
            else if (target.hasClass('fa-shopping-cart')) {
                if (!isClick) {
                    if (cmbCart.val() == 0) {
                        VIS.ADialog.error("VA005_selectCart");
                        return;
                    }
                    c_UomConv_ID = target.attr("conversionid");
                    $bsyDiv[0].style.visibility = "visible";
                    isClick = true;
                    target.css('color', 'lime');
                    window.setTimeout(function () {
                        target.css('color', '#616364');
                    }, 1000);
                    AddToCart("U");
                }
            }
            else if (target.hasClass('VA005-checkbox')) {

                c_UomConv_ID = $(target.parents('.VA005-uom-wrap')).attr("conversionid");
                if (target.prop("checked")) {
                    cons.push(c_UomConv_ID);
                    window.setTimeout(function () {
                        $(target.parents('.VA005-uom-wrap')).addClass('vis-group-selected-op vis-group-selected-opbackground');
                    }, 200);
                }
                else {
                    cons.splice(cons.indexOf(c_UomConv_ID), 1);
                    window.setTimeout(function () {
                        $(target.parents('.VA005-uom-wrap')).removeClass('vis-group-selected-op vis-group-selected-opbackground');
                    }, 200);
                }

            }
            else if (target.hasClass('VA005-uom-wrap')) {
                cons = [];
                $divUomGroup.find(".VA005-checkbox").prop("checked", false);
                if (target.hasClass('vis-group-selected-op vis-group-selected-opbackground')) {
                    $divUomGroup.find(".VA005-uom-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
                    c_UomConv_ID = target.attr("conversionid");
                    cons.splice(cons.indexOf(c_UomConv_ID), 1);
                }
                else {
                    $divUomGroup.find(".VA005-uom-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
                    target.addClass('vis-group-selected-op vis-group-selected-opbackground');
                    target.find(".VA005-checkbox").prop("checked", true);
                    c_UomConv_ID = target.attr("conversionid");
                    cons.push(c_UomConv_ID);
                }
            }
            else {
                if ($(target.parents('.VA005-uom-wrap')).length > 0) {     // length will be 0 if user's search result single or more record and there is some blank space... on click blank space nothing should be happened.
                    cons = [];
                    $divUomGroup.find(".VA005-checkbox").prop("checked", false);
                    if ($(target.parents('.VA005-uom-wrap')).hasClass('vis-group-selected-op vis-group-selected-opbackground')) {
                        $divUomGroup.find(".VA005-uom-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
                        c_UomConv_ID = $(target.parents('.VA005-uom-wrap')).attr("conversionid");
                        cons.splice(cons.indexOf(c_UomConv_ID), 1);
                    }
                    else {
                        $divUomGroup.find(".VA005-uom-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
                        $(target.parents('.VA005-uom-wrap')).addClass('vis-group-selected-op vis-group-selected-opbackground');
                        $(target.parents('.VA005-uom-wrap')).find(".VA005-checkbox").prop("checked", true);
                        c_UomConv_ID = $(target.parents('.VA005-uom-wrap')).attr("conversionid");
                        cons.push(c_UomConv_ID);
                    }
                }
                else {
                    return;
                }
            }
        };

        function VarientContainerClick(e) {

            var target = $(e.target);
            if (target.hasClass('VA005-checkbox')) {
                m_attribute_ID = $(target.parents('.VA005-uom-wrap')).attr("attr_id");
                upcvalue = $(target.parents('.VA005-uom-wrap')).attr("upc");
                if (target.prop("checked")) {
                    attributes.push(m_attribute_ID);
                    upcs.push(upcvalue);
                    window.setTimeout(function () {
                        $(target.parents('.VA005-uom-wrap')).addClass('vis-group-selected-op vis-group-selected-opbackground');
                    }, 200);
                }
                else {
                    attributes.splice(attributes.indexOf(m_attribute_ID), 1);
                    upcs.splice(upcs.indexOf(upcvalue), 1);
                    window.setTimeout(function () {
                        $(target.parents('.VA005-uom-wrap')).removeClass('vis-group-selected-op vis-group-selected-opbackground');
                    }, 200);
                }
            }
            else if (target.hasClass('fa-shopping-cart')) {
                if (!isClick) {
                    if (cmbCart.val() == 0) {
                        VIS.ADialog.error("VA005_selectCart");
                        return;
                    }
                    m_attribute_ID = target.attr("attr_id");
                    upcvalue = target.attr("upc");
                    $bsyDiv[0].style.visibility = "visible";
                    isClick = true;
                    target.css('color', 'lime');
                    window.setTimeout(function () {
                        target.css('color', '#616364');
                    }, 1000);
                    AddToCart("V");
                }
            }
            else if (target.hasClass('VA005-Edit-UPC')) {
                pop.popup({
                    tooltipanchor: target,
                    autoopen: true,
                    type: 'tooltip',
                    background: true,
                    backgroundactive: true,
                    //horizontal:'left'
                    offsetleft: -43,
                    offsettop: -10
                });
                patr_ID = target.attr("patrid");
                // Done by Bharat on 05 March 2018 to move queries to server side
                var upcno = VIS.dataContext.getJSONRecord("VA005/ProductManagement/GetProductUPC", patr_ID);

                //var qry = "SELECT UPC FROM M_ProductAttributes WHERE M_ProductAttributes_ID = " + patr_ID;
                //var upcno = VIS.DB.executeScalar(qry);
                txtEditUPC.val(upcno);
            }
            else if (target.hasClass('VA005-uom-wrap')) {
                attributes = [];
                upcs = [];
                $divVarient.find(".VA005-checkbox").prop("checked", false);
                if (target.hasClass('vis-group-selected-op vis-group-selected-opbackground')) {
                    $divVarient.find(".VA005-uom-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
                    m_attribute_ID = target.attr("attr_id");
                    upcvalue = target.attr("upc");
                    attributes.splice(attributes.indexOf(m_attribute_ID), 1);
                    upcs.splice(upcs.indexOf(upcvalue), 1);
                }
                else {
                    $divVarient.find(".VA005-uom-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
                    target.addClass('vis-group-selected-op vis-group-selected-opbackground');
                    target.find(".VA005-checkbox").prop("checked", true);
                    m_attribute_ID = target.attr("attr_id");
                    upcvalue = target.attr("upc");
                    attributes.push(m_attribute_ID);
                    upcs.push(upcvalue);
                }
            }
            else {
                if ($(target.parents('.VA005-uom-wrap')).length > 0) {     // length will be 0 if user's search result single or more record and there is some blank space... on click blank space nothing should be happened.
                    attributes = [];
                    upcs = [];
                    $divVarient.find(".VA005-checkbox").prop("checked", false);
                    if ($(target.parents('.VA005-uom-wrap')).hasClass('vis-group-selected-op vis-group-selected-opbackground')) {
                        $divVarient.find(".VA005-uom-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
                        m_attribute_ID = $(target.parents('.VA005-uom-wrap')).attr("attr_id");
                        upcvalue = $(target.parents('.VA005-uom-wrap')).attr("upc");
                        attributes.splice(attributes.indexOf(m_attribute_ID), 1);
                        upcs.splice(upcs.indexOf(upcvalue), 1);
                    }
                    else {
                        $divVarient.find(".VA005-uom-wrap").removeClass('vis-group-selected-op vis-group-selected-opbackground');
                        $(target.parents('.VA005-uom-wrap')).addClass('vis-group-selected-op vis-group-selected-opbackground');
                        $(target.parents('.VA005-uom-wrap')).find(".VA005-checkbox").prop("checked", true);
                        m_attribute_ID = $(target.parents('.VA005-uom-wrap')).attr("attr_id");
                        upcvalue = $(target.parents('.VA005-uom-wrap')).attr("upc");
                        attributes.push(m_attribute_ID);
                        upcs.push(upcvalue);
                    }
                }
                else {
                    return;
                }
            }
        };

        function fillConversion(ucid) {
            // Done by Bharat on 05 March 2018 to move queries to server side
            var dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadUOMRate", { "C_UOM_Conversion_ID": ucid });
            if (dr != null) {
                cmbUomTo.val(dr["C_UOM_To_ID"]);
                // Done by shifali on 06 July 2020 to set Values on DivideRate and MultiplyRate
                $txtDiv.setValue(dr["DivideRate"]);
                $txtMul.setValue(dr["MultiplyRate"]);
                uomUPC.val(dr["UPC"]);
            }

            //var qry = "SELECT C_UOM_TO_ID, MultiplyRate AS DivideRate, DivideRate AS MultiplyRate, UPC FROM C_UOM_Conversion WHERE C_UOM_Conversion_ID=" + ucid;
            //var dr = VIS.DB.executeReader(qry.toString(), null);
            //while (dr.read()) {
            //    cmbUomTo.val(dr.getInt(0));
            //    txtDiv.val(dr.getDecimal(1));
            //    txtMul.val(dr.getDecimal(2));
            //    uomUPC.val(dr.getString(3));
            //}
            //dr.close();
        };

        function deleteConversion(ucid) {
            // Done by Shifali to get the msg acc to culture
            //if (VIS.ADialog.ask("DeleteRecord?")) {
            VIS.ADialog.confirm("VA005_DeleteIt", true, "", "Confirm", function (result) {
                if (result == true) {
                    // Done by Bharat on 05 March 2018 to move queries to server side
                    var no = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/DeleteConversion", { "C_UOM_Conversion_ID": ucid });

                    //var qry = "DELETE FROM C_UOM_Conversion WHERE C_UOM_Conversion_ID=" + ucid;
                    //var no = VIS.Utility.Util.getValueOfInt(VIS.DB.executeQuery(qry.toString()));
                    if (no > 0) {
                        LoadUomGroup();
                    }
                    else {
                        VIS.ADialog.error("VA005_ConversionNotDeleted");
                    }
                }
            });
        };

        function LoadUomGroup() {

            $bsyDiv[0].style.visibility = "visible";
            cons = [];
            c_UomConv_ID = 0;
            $divUomGroup.find(".VA005-uom-wrap").remove();
            // Done by Bharat on 05 March 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadUomGroup", { "M_Product_ID": prods[0] }, LoadUomGroupCallBack);

            //var qry = "SELECT (SELECT u.Name FROM C_UOM u WHERE u.C_UOM_ID = uc.C_UOM_TO_ID) AS UomTo, uc.C_UOM_TO_ID, uc.MultiplyRate AS DivideRate, uc.DivideRate AS MultiplyRate, uc.C_UOM_Conversion_ID," +
            //" uc.UPC FROM C_UOM_Conversion uc INNER JOIN M_Product p ON uc.M_Product_ID = p.M_Product_ID WHERE p.M_Product_ID =" + prods[0];
            //var sql = VIS.MRole.getDefault().addAccessSQL(qry.toString(), "uc", VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO); // fully qualidfied - RO
            //VIS.DB.executeReader(sql.toString(), null, LoadUomGroupCallBack);
        };

        function LoadUomGroupCallBack(dr) {
            var id = 0;
            if (dr != null) {
                for (var i in dr) {
                    id = id + 1;
                    var uname = dr[i]["Name"];
                    var uomToID = dr[i]["C_UOM_To_ID"];
                    var dRate = dr[i]["DivideRate"];
                    var mRate = dr[i]["MultiplyRate"];
                    var conv_Id = dr[i]["C_UOM_Conversion_ID"];
                    var upc = dr[i]["UPC"];
                    var div = '<div class="VA005-uom-wrap" conversionid = ' + conv_Id + '><div class="VA005-item-top"><div class="VA005-item-name"><input class="VA005-checkbox" type="checkbox"><label>' + uname +
                        '</label></div><div class="VA005-item-icons">' + '<span id = "VA005_BarCode_' + id + '" ></span><span conversionid = ' + conv_Id +
                        ' class="VA005-uom-icons vis vis-delete VA005-icons-color" title="' + VIS.Msg.getMsg("DeleteActivity") + '"></span><span conversionid = ' + conv_Id +
                        ' class="VA005-uom-icons vis vis-edit VA005-icons-color" title="' + VIS.Msg.getMsg("Edit") + '"></span>' +
                        '<span class="VA005-uom-icons fa fa-shopping-cart VA005-icons-color" conversionid = ' + conv_Id + ' title="' + VIS.Msg.getMsg("VA005_AddCart") + '"></span></div></div><div><span class="VA005-multiplier">'
                        + VIS.Msg.getElement(VIS.Env.getCtx(), "MultiplyRate") + ' : ' + mRate.toLocaleString() + '</span><span class="VA005-divide">'
                        + VIS.Msg.getElement(VIS.Env.getCtx(), "DivideRate") + ' : ' + dRate.toLocaleString() + '</span></div></div>';
                    $divUomGroup.append(div);
                    GenerateBarcode(upc, $divUomGroup.find("#VA005_BarCode_" + id));
                }
            }
            //while (dr.read()) {
            //    id = id + 1;
            //    var uname = dr.getString(0);
            //    var uomToID = dr.getInt(1);
            //    var dRate = dr.getDecimal(2);
            //    var mRate = dr.getDecimal(3);
            //    var conv_Id = dr.getInt(4);
            //    var upc = dr.getString(5);
            //    var div = '<div class="VA005-uom-wrap" conversionid = ' + conv_Id + '><div class="VA005-item-top"><div class="VA005-item-name"><input class="VA005-checkbox" type="checkbox"><label>' + uname +
            //        '</label></div><div class="VA005-item-icons">' + '<span id = "VA005_BarCode_' + id + '" style="float:left;"></span><span conversionid = ' + conv_Id +
            //        ' class="VA005-uom-icons vis vis-delete VA005-icons-color" title="' + VIS.Msg.getMsg("DeleteActivity") + '"></span><span conversionid = ' + conv_Id +
            //        ' class="VA005-uom-icons vis vis-edit VA005-icons-color" title="' + VIS.Msg.getMsg("Edit") + '"></span>' +
            //        '<span class="VA005-uom-icons fa fa-shopping-cart VA005-icons-color" conversionid = ' + conv_Id + ' title="' + VIS.Msg.getMsg("VA005_AddCart") + '"></span></div></div><div><span class="VA005-multiplier">'
            //        + VIS.Msg.getElement(VIS.Env.getCtx(), "MultiplyRate") + ' : ' + mRate + '</span><span class="VA005-divide">'
            //        + VIS.Msg.getElement(VIS.Env.getCtx(), "DivideRate") + ' : ' + dRate + '</span></div></div>';
            //    $divUomGroup.append(div);
            //    GenerateBarcode(upc, $divUomGroup.find("#VA005_BarCode_" + id));
            //}
            //dr.close();
            window.setTimeout(function () {
                $bsyDiv[0].style.visibility = "hidden";
            }, 60);
        };

        function LoadVarients() {

            $bsyDiv[0].style.visibility = "visible";
            $divVarient.empty();
            // Done by Bharat on 28 Feb 2018 to move queries to server side
            VIS.Env.getCtx().setContext($self.windowNo, "M_Product_ID", VIS.Utility.Util.getValueOfInt(prods[0]));
            attrSetId = VIS.dataContext.getJSONRecord("VA005/ProductManagement/GetAttributeSet", prods[0]);

            //var qry = "SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + prods[0];
            //attrSetId = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));

            if (attrSetId > 0) {
                // Done by Bharat on 05 March 2018 to move queries to server side
                VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadVarients", { "M_Product_ID": prods[0] }, LoadVarientsCallback);

                //var qry = "SELECT patr.UPC,patr.M_AttributeSetInstance_ID,ats.Description,patr.M_ProductAttributes_ID FROM M_ProductAttributes patr INNER JOIN M_AttributeSetInstance ats ON" +
                //    " (patr.M_AttributeSetInstance_ID = ats.M_AttributeSetInstance_ID) WHERE patr.M_Product_ID = " + prods[0];
                //var sql = VIS.MRole.getDefault().addAccessSQL(qry.toString(), "patr", VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO); // fully qualidfied - RO
                //VIS.DB.executeReader(sql.toString(), null, LoadVarientsCallback);
            }
            else {
                var div = '<div style="float: left;width: 100%;"><span style="font-style:italic;font-weight:bold;">' + VIS.Msg.getMsg("PAttributeNoAttributeSet") + '</span></div>';
                $divVarient.append(div);
                $bsyDiv[0].style.visibility = "hidden";
            }
        };

        function LoadVarientsCallback(dr) {
            var id = 0;
            if (dr != null) {
                for (var i in dr) {
                    id = id + 1;
                    var upc = dr[i]["UPC"];
                    var attr_id = dr[i]["M_AttributeSetInstance_ID"];
                    var attr = dr[i]["Description"];
                    var patr_id = dr[i]["M_ProductAttributes_ID"];
                    var div = '<div class="VA005-uom-wrap" attr_id = ' + attr_id + ' upc = ' + upc + '><div class="VA005-item-top"><div class="VA005-item-name"><input class="VA005-checkbox" type="checkbox" ><label>' + attr +
                        '</label></div><div class="VA005-item-icons"><span id = "VA005_BarCode_' + id + '" style="float:left;"></span>' +
                        //'<span attr_id = '+ attr_id + ' class="VA005-uom-icons vis vis-edit VA005-icons-color"></span> 
                        '<span attr_id = ' + attr_id + ' upc = "' + upc + '" class="VA005-uom-icons fa fa-shopping-cart VA005-icons-color" title="' + VIS.Msg.getMsg("VA005_AddCart") + '"></span></div></div>' +
                        '<div style="float: left;width: 100%;"><span patrid=' + patr_id + ' class="VA005-Edit-UPC">' + VIS.Msg.getMsg("VA005_EditUPC") + '</span></div>';
                    $divVarient.append(div);
                    GenerateBarcode(upc, $divVarient.find("#VA005_BarCode_" + id));
                }
            }

            //while (dr.read()) {
            //    id = id + 1;
            //    var upc = dr.getString(0);
            //    var attr_id = dr.getInt(1);
            //    var attr = dr.getString(2);
            //    var patr_id = dr.getInt(3);
            //    var div = '<div class="VA005-uom-wrap" attr_id = ' + attr_id + ' upc = ' + upc + '><div class="VA005-item-top"><div class="VA005-item-name"><input class="VA005-checkbox" type="checkbox" ><label>' + attr +
            //        '</label></div><div class="VA005-item-icons"><span id = "VA005_BarCode_' + id + '" style="float:left;"></span>' +
            //        //'<span attr_id = '+ attr_id + ' class="VA005-uom-icons vis vis-edit VA005-icons-color"></span> 
            //        '<span attr_id = ' + attr_id + ' upc = "' + upc + '" class="VA005-uom-icons fa fa-shopping-cart VA005-icons-color" title="' + VIS.Msg.getMsg("VA005_AddCart") + '"></span></div></div>' +
            //        '<div style="float: left;width: 100%;"><span patrid=' + patr_id + ' class="VA005-Edit-UPC">' + VIS.Msg.getMsg("VA005_EditUPC") + '</span></div>';
            //    $divVarient.append(div);
            //    GenerateBarcode(upc, $divVarient.find("#VA005_BarCode_" + id));
            //}
            //dr.close();
            window.setTimeout(function () {
                $bsyDiv[0].style.visibility = "hidden";
            }, 60);
        };

        function LoadProductDetails() {
            $bsyDiv[0].style.visibility = "visible";
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/GetProductDetails", { "M_Product_ID": prods[0] }, callbackProdDetails);
        };

        function callbackProdDetails(data) {
            $bsyDiv[0].style.visibility = "visible";

            if (data.Table[0] != null) {
                divZoomProdName.css("display", "flex");
                prodName.text(data.Table[0].NAME);
                prodUPC.find("span").remove();
                prodUPC.append('<span>' + data.Table[0].UPC + '</span>');
                prodAttributeSet.find("span").remove();
                prodAttributeSet.append('<span>' + data.Table[0].ATTRIBUTE + '</span>');
                prodType.find("span").remove();
                prodType.append('<span>' + data.Table[0].PRODUCTTYPE + '</span>');
                prodCategory.find("span").remove();
                prodCategory.append('<span>' + data.Table[0].PRODUCTCATEGORY + '</span>');
                prodTaxCat.find("span").remove();
                prodTaxCat.append('<span>' + data.Table[0].TAXCATEGORY + '</span>');
                prodUOM.find("span").remove();
                prodUOM.append('<span>' + data.Table[0].UOM + '</span>');
                prodParent.find("span").remove();
                prodParent.append('<span>' + data.Table[0].TREE + '</span>');
                var imageUrl = data.Table[0].IMAGEURL;
                if (imageUrl != null) {
                    if (imageUrl != "") {
                        imageUrl = imageUrl.substring(imageUrl.lastIndexOf("/") + 1, imageUrl.length);
                        var d = new Date();
                        imgUsrImage.removeAttr("src");
                        imgUsrImage.attr("src", VIS.Application.contextUrl + "Images/Thumb140x120/" + imageUrl + "?" + d.getTime());
                        divUsrImage.empty();
                        divUsrImage.append(imgUsrImage);
                    }
                    else {
                        divUsrImage.empty();
                        divUsrImage.append($('<i style="max-height: 100%; max-width: 100%;" class="vis vis-image" ></i>'));
                    }
                }
                else {
                    divUsrImage.empty();
                    divUsrImage.append($('<i style="max-height: 100%; max-width: 100%;" class="vis vis-image" ></i>'));
                }
            }
            window.setTimeout(function () {
                $bsyDiv[0].style.visibility = "hidden";
            }, 60);
        };

        function editVariantUPC() {

            $bsyDiv[0].style.visibility = "visible";
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "VA005/ProductManagement/updateVarient",
                dataType: "json",
                data: {
                    id: patr_ID,
                    UpcCode: txtEditUPC.val()
                },
                success: function (data) {
                    var returnValue = data.result;

                    if (returnValue == "") {
                        pop.popup('hide');
                        txtEditUPC.val("");
                        patr_ID = 0;
                        LoadVarients();
                    }
                    else {
                        VIS.ADialog.error(returnValue);
                    }
                    $bsyDiv[0].style.visibility = "hidden";
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    // 
                    console.log(textStatus);
                    $bsyDiv[0].style.visibility = "hidden";
                    alert("RecordNotSaved");
                    return;
                }
            });
        };

        function editItem(e) {

            var nodeID = 0;
            var parentID = 0;
            var target = $(e.target);
            if (target.data("dtype") == "3") {
                nodeID = target.data("nodeid");
                parentID = target.data("parentid");
                if (target.hasClass('VA005-checkbox')) {
                    if (target.prop("checked")) {
                        if (!attrArray[parentID]) {
                            attrArray[parentID] = [];
                        }
                        attrArray[parentID].push(nodeID);
                    }
                    else {
                        attrArray[parentID].splice(attrArray[parentID].indexOf(nodeID), 1);
                    }
                }
                else {
                    if ($(target.parents('.VA005-attribute')).find('.VA005-checkbox').prop("checked")) {
                        $(target.parents('.VA005-attribute')).find(".VA005-checkbox").prop("checked", false);
                        attrArray[parentID].splice(attrArray[parentID].indexOf(nodeID), 1);
                    }
                    else {
                        $(target.parents('.VA005-attribute')).find(".VA005-checkbox").prop("checked", true);
                        if (!attrArray[parentID]) {
                            attrArray[parentID] = [];
                        }
                        attrArray[parentID].push(nodeID);
                    }
                }
            }
        };

        function GenerateTree() {

            attrArray = {};
            // Done by Bharat on 28 Feb 2018 to move queries to server side
            attrSetId = VIS.dataContext.getJSONRecord("VA005/ProductManagement/GetAttributeSet", prods[0]);

            //var qry = "SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + prods[0];
            //attrSetId = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
            if (attrSetId > 0) {
                $bsyDiv[0].style.visibility = "visible";
                $.ajax({
                    url: VIS.Application.contextUrl + "VA005/ProductManagement/AttributeSetListing",
                    type: 'Get',
                    data: {
                        AttributeSet_ID: attrSetId
                    },
                    //async: false,
                    success: function (data) {

                        var res = JSON.parse(data);
                        attr = [];
                        if (divtree && divtree.data("kendoTreeView") != undefined) {
                            divtree.data("kendoTreeView").destroy();
                            divtree.empty();
                        }
                        if (res != null) {
                            btnGenerate.show();
                            for (var item in res[0].items) {
                                attr.push(res[0].items[item].NodeID);
                            }
                        }
                        else {
                            divtree.empty();
                            var div = '<div style="float: left;width: 100%;"><span style="font-style:italic;font-weight:bold;white-space:normal;">' + VIS.Msg.getMsg("VA005_LotSerialAttribute") + '</span></div>';
                            divtree.append(div);
                            btnGenerate.hide();
                            $bsyDiv[0].style.visibility = "hidden";
                            return;
                            //$divVarient.show();
                        }
                        divtree.kendoTreeView({
                            dragAndDrop: false,
                            dataSource: res,

                            //*** First div for tree contain first image and paragraph on tree..
                            template: "<div style='margin:#= item.margin #;float:left' data-parentid='#= item.ParentID #' data-nodeid='#= item.NodeID #' data-dType= '#= item.Type #' class='VA005-attribute'>" +
                                "<div style='width:auto;float:left;display: flex;align-items: center;' data-parentid='#= item.ParentID #' data-nodeid='#= item.NodeID #' data-dType= '#= item.Type #'>" +
                                "<input type='checkbox' class='VA005-checkbox' style='margin: 0;" +
                                "display:#= item.checkbox #' data-parentid='#= item.ParentID #' data-nodeid='#= item.NodeID #' data-dType= '#= item.Type #'>" +
                                "<img src='" + VIS.Application.contextUrl + "#= item.ImageSource #' style='" +
                                "display:#= item.visibility #' data-parentid='#= item.ParentID #' data-nodeid='#= item.NodeID #' data-dType= '#= item.Type #'>" +
                                "<p style='border-radius:4px;padding: 5px 10px' data-parentid='#= item.ParentID #' data-nodeid='#= item.NodeID #' data-dType= '#= item.Type #'>#= item.text #</p>" +
                                "</div>" +
                                "</div>"
                        });
                        window.setTimeout(function () {
                            $bsyDiv[0].style.visibility = "hidden";
                        }, 60);
                        divtree.off("click", editItem);
                        divtree.on("click", editItem);
                    },
                    error: function (data) {
                        alert(data);
                        $bsyDiv[0].style.visibility = "hidden";
                    },
                });
            }
            else {
                $divLeftTree.hide();
                btnGenerate.hide();
                $divVarient.show();
            }
            // $bsyDiv[0].style.visibility = "hidden";
        };

        function GenerateBarcode(upc, divbarcode) {
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
                addQuietZone: 1
            };

            if (upc.length == 12) {
                divbarcode.barcode(upc, btypeEan13, settings);
            }
            else if (upc.length == 7) {
                divbarcode.barcode(upc, btypeEan8, settings);
            }
            else if (upc.length == 4) {
                divbarcode.barcode(upc, btypeC39, settings);
            }
            else {
                divbarcode.barcode(upc, btypeC128, settings);
            }
        };

        function prodScroll() {
            if ($(this).scrollTop() > 0) {
                if ($(this).scrollTop() + $(this).innerHeight() + 5 >= this.scrollHeight) {
                    if (pgno < noPages) {
                        $bsyDiv[0].style.visibility = "visible";
                        pgno++;
                        loadProd($searchProduct.val(), pcat_ID, $queryCat.val(), pgno, PAGESIZE, 0);
                    }
                }
            }
        };

        function prodCatScroll() {
            if ($(this).scrollTop() > 0) {
                if ($(this).scrollTop() + $(this).innerHeight() >= this.scrollHeight) {
                    if (pageno < noOfCats) {
                        $bsyDiv[0].style.visibility = "visible";
                        pageno++;
                        loadCategories($txtsearchCat.val(), pageno, PAGESIZE);
                    }
                }
            }
        };

        function prodevents() {
            ch.onOkClick = function (e) {
                if (VIS.Utility.Util.getValueOfString(cmbProductType.val()) == "") {
                    VIS.ADialog.error("VA005_ProductType");
                    if (e != undefined) {
                        e.preventDefault();
                    }
                    return false;
                }
                if (VIS.Utility.Util.getValueOfString(txtName.val()) == "" || VIS.Utility.Util.getValueOfInt(cmbTaxCategory.val()) == 0 || VIS.Utility.Util.getValueOfInt(cmbCat.val()) == 0 || VIS.Utility.Util.getValueOfInt(cmbUOM.val()) == 0) {
                    VIS.ADialog.error("FillMandatory");
                    return false;
                }
                //isOk = true;
                saveProduct();
                return false;
            };
            ch.onCancelClick = function () {
                ClearProdData();
            };

            btnAttributeSet.on("click", function (e) {

                options.Attribute = "Y";
                options.Category = "N";
                options.Tax = "N";
                options.UOM = "N";

                //opt.style = 
                btnAttributeSet.w2overlay($zoomDiv.clone(true));
                //$root.find(".w2ui-reset").css("left", e.clientX - 40);
                e.stopPropagation();
            });

            btnProdCat.on("click", function (e) {

                options.Attribute = "N";
                options.Category = "Y";
                options.Tax = "N";
                options.UOM = "N";

                //opt.style = 
                btnProdCat.w2overlay($zoomDiv.clone(true));
                //$root.find(".w2ui-reset").css("left", e.clientX - 40);
                e.stopPropagation();
            });

            btnTaxCategory.on("click", function (e) {

                options.Attribute = "N";
                options.Category = "N";
                options.Tax = "Y";
                options.UOM = "N";

                //opt.style = 
                btnTaxCategory.w2overlay($zoomDiv.clone(true));
                //$root.find(".w2ui-reset").css("left", e.clientX - 40);
                e.stopPropagation();
            });

            btnUomZoom.on("click", function (e) {

                options.Attribute = "N";
                options.Category = "N";
                options.Tax = "N";
                options.UOM = "Y";

                //opt.style = 
                btnUomZoom.w2overlay($zoomDiv.clone(true));
                //$root.find(".w2ui-reset").css("left", e.clientX - 40);
                e.stopPropagation();
            });
        };

        function priceEvents() {
            cmbPriceList.on("change", function () {
                $bsyDiv[0].style.visibility = "visible";
                BindPriceGrid();
            });

            pl.onOkClick = function (e) {
                if (cmbPriceList.val() == 0) {
                    VIS.ADialog.error("VA005_SelectPrice");
                    if (e != undefined) {
                        e.preventDefault();
                    }
                    return false;
                }
                //isOk = true;
                updatePrice();
                return false;
            };
            pl.onCancelClick = function () {
                pl.close();
            };
        };

        function BindPriceGrid() {
            multiValues = [];
            dGrid.clear();
            precision = 2;
            //$bsyDiv[0].style.visibility = "visible";            
            window.setTimeout(function () {
                if (cmbPriceList.val() > 0) {
                    var Recid = 0;
                    // Done by Bharat on 05 March 2018 to move queries to server side
                    var dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadPriceListData", { "M_PriceList_ID": cmbPriceList.val() });
                    if (dr != null) {
                        precision = dr["PricePrecision"];;
                        curName = dr["ISO_Code"];
                    }
                    //var sql = "SELECT pl.PricePrecision,cur.ISO_Code FROM M_PriceList_Version plv INNER JOIN M_PriceList pl ON plv.M_PriceList_ID=pl.M_PriceList_ID " +
                    //    " INNER JOIN C_Currency cur ON pl.C_Currency_ID = cur.C_Currency_ID WHERE plv.M_PriceList_Version_ID = " + cmbPriceList.val();
                    //var dr = VIS.DB.executeReader(sql);
                    //while (dr.read()) {
                    //    precision = dr.getInt(0);
                    //    curName = dr.getString(1);
                    //}
                    //dr.close();
                    currSpan.text(curName);
                    VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/GetProductPrices", { "M_Product_ID": prods, "PriceList": cmbPriceList.val() }, callBackPriceGrid);
                    //for (var item in prods) {
                    //    var sqlaa = "SELECT prd.Name,pr.PriceList,pr.PriceStd,pr.PriceLimit,pr.C_UOM_ID,u.Name as UOM,pr.Lot,pr.M_AttributeSetInstance_ID,ats.Description" +
                    //        " FROM M_ProductPrice pr INNER JOIN C_UOM u ON (pr.C_UOM_ID= u.C_UOM_ID) INNER JOIN M_Product prd ON (pr.M_Product_ID= prd.M_Product_ID)" +
                    //        " INNER JOIN M_AttributeSetInstance ats ON (pr.M_AttributeSetInstance_ID = ats.M_AttributeSetInstance_ID) " +
                    //        " WHERE pr.IsActive = 'Y' AND pr.M_PriceList_Version_ID = " + cmbPriceList.val() + " AND pr.M_Product_ID = " + prods[item];
                    //    var ds = VIS.DB.executeDataSet(sqlaa.toString(), null, null);
                    //    sqlaa = "SELECT img.ImageUrl FROM M_Product prd LEFT OUTER JOIN AD_Image img ON prd.AD_Image_ID = img.AD_Image_ID WHERE prd.M_Product_ID = " + prods[item];
                    //    var img = VIS.Utility.Util.getValueOfString(VIS.DB.executeScalar(sqlaa));
                    //    if (ds != null && ds.tables[0].rows.length > 0) {
                    //        for (var i = 0; i < ds.tables[0].rows.length; i++) {
                    //            Recid = Recid + 1;
                    //            multiValues.push(
                    //            {
                    //                recid: Recid,
                    //                product_ID: prods[item],
                    //                Product: ds.tables[0].rows[i].cells.name,
                    //                PriceList: VIS.Utility.Util.getValueOfDecimal(ds.tables[0].rows[i].cells.pricelist),
                    //                PriceStd: VIS.Utility.Util.getValueOfDecimal(ds.tables[0].rows[i].cells.pricestd),
                    //                PriceLimit: VIS.Utility.Util.getValueOfDecimal(ds.tables[0].rows[i].cells.pricelimit),
                    //                C_Uom_ID: VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells.c_uom_id),
                    //                UOM: ds.tables[0].rows[i].cells.uom,
                    //                Lot: ds.tables[0].rows[i].cells.lot,
                    //                attribute_ID: VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells.m_attributesetinstance_id),
                    //                Attribute: ds.tables[0].rows[i].cells.description,
                    //                ImageUrl: img,
                    //                updated: false
                    //            });
                    //        }
                    //    }
                    //    else {
                    //        var dr = VIS.DB.executeReader("SELECT Name, C_UOM_ID FROM M_Product WHERE M_Product_ID = " + prods[item], null);
                    //        while (dr.read()) {
                    //            var prodName = dr.getString(0);
                    //            var uom = dr.getInt(1);
                    //        }
                    //        Recid = Recid + 1;
                    //        multiValues.push(
                    //        {
                    //            recid: Recid,
                    //            product_ID: prods[item],
                    //            Product: prodName,
                    //            PriceList: 0.0,
                    //            PriceStd: 0.0,
                    //            PriceLimit: 0.0,
                    //            C_Uom_ID: uom,
                    //            UOM: "",
                    //            Lot: "",
                    //            attribute_ID: 0,
                    //            Attribute: "",
                    //            ImageUrl: img,
                    //            updated: false
                    //        });
                    //    }
                    //}
                }
                else {
                    $bsyDiv[0].style.visibility = "hidden";
                    multiValues = [];
                }
            }, 200);
        };

        function callBackPriceGrid(data) {
            if (data.length > 0) {
                multiValues = data;
                //for (var j = 0; j < dGrid.columns.length; j++) {
                //    if (dGrid.columns[j].field == "PriceList" || dGrid.columns[j].field == "PriceStd" || dGrid.columns[j].field == "PriceLimit") {
                //        dGrid.columns[j].render = function (record, index, colIndex) {
                //            var val = VIS.Utility.Util.getValueOfDecimal(record[dGrid.columns[colIndex].field]);
                //            return (val).toLocaleString();
                //        };;
                //    }
                //}
                w2utils.encodeTags(multiValues);
                dGrid.add(multiValues);
                for (var k = 0; k < dGrid.records.length; k++) {
                    var src = "";
                    $("#grid_gridprice_" + $self.windowNo + "_rec_" + dGrid.records[k].recid).find("input[type=text]").val(multiValues[k].Attribute);
                    var imag = multiValues[k].ImageUrl;
                    if (imag != null) {
                        if (imag != "") {
                            imag = imag.substring(imag.lastIndexOf("/") + 1, imag.length);
                            var d = new Date();
                            src = '<img alt="" title="" style="opacity:1;" src="' + VIS.Application.contextUrl + "Images/Thumb16x16/" + imag + "?" + d.getTime() + '">';
                        }
                        else {
                            src = '<i class= "vis vis-image" ></i>';
                        }
                    }
                    else {
                        src = '<i class= "vis vis-image" ></i>';
                    }
                    var rec = k + 1;
                    $($("#grid_gridprice_" + $self.windowNo + "_rec_" + rec).find("div")[0]).empty();
                    $($("#grid_gridprice_" + $self.windowNo + "_rec_" + rec).find("div")[0]).append(src);
                }
            }
            $bsyDiv[0].style.visibility = "hidden";
            multiValues = [];
        };

        function printevents() {
            pr.onOkClick = function (e) {
                if (VIS.Utility.Util.getValueOfString(txtHeader.val()) == "" || VIS.Utility.Util.getValueOfInt(cmbVersion.val()) == 0) {
                    VIS.ADialog.error("FillMandatory");
                    return false;
                }
                var str = VA005.StickerProduct.prototype.create(txtHeader.val(), cmbVersion.val(), printData);
                pr.close();
                return false;
            };
            pr.onCancelClick = function () {
                ClearPrintData();
            };
        };

        // Bind Related Grid 
        function bindRelatedGrid() {

            $bsyDiv[0].style.visibility = "visible";
            multiValues = [];
            var Recid = 0;
            dRelatedGrid.clear();
            Recid = Recid + 1;
            var sqlVar = "";
            //sqlVar = "SELECT DISTINCT p.Name as Product, p.M_Product_ID, u.Name AS UOM , (bomQtyOnHand(p.M_Product_ID,w.M_Warehouse_ID,0)) AS QtyOnHand,"
            //    + " bomQtyAvailable(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyAvailable, (bomQtyReserved(p.M_Product_ID,w.M_Warehouse_ID,0))  AS QtyReserved"
            //    + " FROM M_RelatedProduct s INNER JOIN M_Product p ON (p.M_Product_ID = s.RelatedProduct_ID) INNER JOIN C_UOM u ON (p.C_UOM_ID = u.C_UOM_ID) LEFT OUTER JOIN M_Storage st "
            //    + " ON (st.M_Product_ID = p.M_Product_ID) LEFT OUTER JOIN M_Locator l ON (st.M_Locator_ID = l.M_Locator_ID) LEFT OUTER JOIN M_Warehouse w ON (w.M_Warehouse_ID = l.M_Warehouse_ID)"
            //    + " WHERE s.IsActive='Y' AND s.M_Product_ID = " + prods[0];

            // Done by Bharat on 05 March 2018 to move queries to server side
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadReleatedData", { "M_Product_ID": prods[0] }, callbackRelatedGrid);
            //sqlVar = "SELECT s.Name AS Product,p.Name as RelatedProduct, s.RelatedProductType,s.RelatedProduct_ID"
            //    + " FROM M_RelatedProduct s INNER JOIN M_Product p ON (p.M_Product_ID = s.RelatedProduct_ID)"
            //    + " WHERE s.IsActive='Y' AND s.M_Product_ID = " + prods[0];
            //VIS.DB.executeReader(sqlVar.toString(), null, callbackRelatedGrid);
        };

        function callbackRelatedGrid(dr) {
            var Recid = 0;
            if (dr != null) {
                for (var i in dr) {
                    Recid = Recid + 1;
                    multiValues.push(
                        {
                            recid: Recid,
                            Product: dr[i]["Product"],
                            RelatedProduct: dr[i]["RelatedProduct"],
                            RelatedType: dr[i]["RelatedProductType"],
                            M_Product_ID: dr[i]["RelatedProduct_ID"],
                            updated: false
                        });
                }
            }
            //if (ds != null && ds.tables[0].rows.length > 0) {
            //    for (var i = 0; i < ds.tables[0].rows.length; i++) {
            //        Recid = Recid + 1;
            //        multiValues.push(
            //        {
            //            recid: Recid,
            //            Product: ds.tables[0].rows[i].cells.product,
            //            RelatedProduct: ds.tables[0].rows[i].cells.relatedproduct,
            //            RelatedType: ds.tables[0].rows[i].cells.relatedproducttype,
            //            M_Product_ID: ds.tables[0].rows[i].cells.relatedproduct_id,
            //            updated: false
            //        });
            //    }
            //}
            w2utils.encodeTags(multiValues);
            dRelatedGrid.add(multiValues);
            multiValues = [];
            window.setTimeout(function () {
                $bsyDiv[0].style.visibility = "hidden";
            }, 60);
        };

        function BindCartGrid() {

            $bsyDiv[0].style.visibility = "visible";
            multiValues = [];
            cartGrid.clear();
            if (cmbCart.val() > 0) {
                var sqlaa = "";
                var ds = null;

                // Done by Bharat on 05 March 2018 to move queries to server side
                VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadCartData", { "VAICNT_InventoryCount_ID": cmbCart.val() }, BindCartCallBack);

                //var sqlaa = "SELECT po.VAICNT_InventoryCountLine_ID,po.M_Product_ID,prd.Name, po.C_UOM_ID, u.Name AS UOM, po.UPC, po.M_AttributeSetInstance_ID, ats.Description, po.VAICNT_Quantity," +
                //        " prd.M_AttributeSet_ID FROM VAICNT_InventoryCountLine po LEFT JOIN C_UOM u ON po.C_UOM_ID = u.C_UOM_ID LEFT JOIN M_Product prd" +
                //        " ON po.M_Product_ID= prd.M_Product_ID LEFT JOIN M_AttributeSetInstance ats ON po.M_AttributeSetInstance_ID = ats.M_AttributeSetInstance_ID" +
                //        " WHERE po.IsActive = 'Y' AND po.VAICNT_InventoryCount_ID = " + cmbCart.val();
                //VIS.DB.executeDataSet(sqlaa.toString(), null, BindCartCallBack);
            }
            else {
                isClick = false;
                $bsyDiv[0].style.visibility = "hidden";
            }
        };

        function BindCartCallBack(dr) {
            var Recid = 0;
            if (dr != null) {
                for (var i in dr) {
                    Recid = Recid + 1;
                    multiValues.push(
                        {
                            recid: Recid,
                            LineID: dr[i]["VAICNT_InventoryCountLine_ID"],
                            product_ID: dr[i]["M_Product_ID"],
                            Product: dr[i]["Name"],
                            C_Uom_ID: dr[i]["C_UOM_ID"],
                            attribute_ID: dr[i]["M_AttributeSetInstance_ID"],
                            Attribute: dr[i]["Description"],
                            UPC: dr[i]["UPC"],
                            Qty: dr[i]["VAICNT_Quantity"],
                            updated: false
                        });
                }
            }

            //if (ds != null && ds.tables[0].rows.length > 0) {
            //    for (var i = 0; i < ds.tables[0].rows.length; i++) {
            //        Recid = Recid + 1;
            //        multiValues.push(
            //        {
            //            recid: Recid,
            //            LineID: ds.tables[0].rows[i].cells.vaicnt_inventorycountline_id,
            //            product_ID: ds.tables[0].rows[i].cells.m_product_id,
            //            Product: ds.tables[0].rows[i].cells.name,
            //            C_Uom_ID: VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells.c_uom_id),
            //            attribute_ID: VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells.m_attributesetinstance_id),
            //            Attribute: ds.tables[0].rows[i].cells.description,
            //            UPC: ds.tables[0].rows[i].cells.upc,
            //            Qty: VIS.Utility.Util.getValueOfDecimal(ds.tables[0].rows[i].cells.vaicnt_quantity),
            //            updated: false
            //        });
            //    }
            //}

            w2utils.encodeTags(multiValues);
            cartGrid.add(multiValues);
            for (var k = 0; k < cartGrid.records.length; k++) {
                $("#grid_gridcart_" + $self.windowNo + "_rec_" + cartGrid.records[k].recid).find("input[type=text]").val(multiValues[k].Attribute);
                // Done by Bharat on 28 Feb 2018 to move queries to server side
                var mattsetid = VIS.dataContext.getJSONRecord("VA005/ProductManagement/GetAttributeSet", multiValues[k].product_ID);

                //var qry = "SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + multiValues[k].product_ID;
                if (mattsetid <= 0) {
                    $("#grid_gridcart_" + $self.windowNo + "_rec_" + cartGrid.records[k].recid).find("input:not([type='checkbox'])").hide();
                    $("#grid_gridcart_" + $self.windowNo + "_rec_" + cartGrid.records[k].recid).find("i").hide();
                }
            }
            cartGrid.selectNone();
            multiValues = [];
            isClick = false;
            $bsyDiv[0].style.visibility = "hidden";
        };

        function supplierEvents() {
            cmbSupplier.on("change", function () {
                $bsyDiv[0].style.visibility = "visible";
                BindSupplierGrid();
            });

            ml.onOkClick = function (e) {
                if (cmbSupplier.val() == 0) {
                    VIS.ADialog.error("VA005_SelectSupplier");
                    if (e != undefined) {
                        e.preventDefault();
                    }
                    return false;
                }
                //isOk = true;
                updateSupplier();
                return false;
            };
            ml.onCancelClick = function () {
                ml.close();
            };
        };

        function BindSupplierGrid() {
            multiValues = [];
            sGrid.clear();
            precision = 2;

            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/GetSupplierData",
                { "M_Product_ID": prods, "Supplier": cmbSupplier.val() }, callBackSupplierGrid);
        };

        function callBackSupplierGrid(data) {
            if (data.length > 0) {
                multiValues = data;
                //for (var j = 0; j < sGrid.columns.length; j++) {
                //    if (sGrid.columns[j].field == "OrderMin" || sGrid.columns[j].field == "OrderPack") {
                //        sGrid.columns[j].render = function (record, index, colIndex) {
                //            var val = VIS.Utility.Util.getValueOfDecimal(record[sGrid.columns[colIndex].field].toFixed(precision));
                //            return (val).toLocaleString();
                //        };
                //    }
                //}
                w2utils.encodeTags(multiValues);
                sGrid.add(multiValues);
            }
            $bsyDiv[0].style.visibility = "hidden";
            multiValues = [];
        };

        this.GetImages = function () {
            $.ajax({
                //async: false,
                type: "GET",
                dataType: "json",
                url: VIS.Application.contextUrl + "VA005/ProductManagement/GetImages",
                success: function (data) {
                    var result = JSON.parse(data);
                    selectedimg = [];
                    ProductImgul.children().remove();
                    for (var i = 0; i < result.length; i++) {
                        ProductImgul.append("<li filename='" + result[i].ImageDetail.fileName + "' path='" + result[i].ImageDetail.filePath + "'><img src=" + result[i].ImageDetail.fileBase64Data + " ></li>")
                    }
                    DragDropDocument();
                },

                error: function (e) {
                    console.log(e);
                }
            });
        };

        var DragDropDocument = function () {
            $root.find(".VA005-productimgul li").draggable({
                revert: "invalid", // when not dropped, the item will revert back to its initial position
                containment: "document",
                helper: "clone",
                cursor: 'move',
                cursorAt: { left: -10, down: -5 },
                start: function (event, ui) {
                    ui.helper.animate({
                        width: 80,
                        height: 50
                    });
                }
            });
            $root.find(".vis-group-users-container .vis-group-user-wrap").droppable({
                drop: function (event, ui) {
                    //$(this).append($(ui.draggable).attr("path"));
                    SaveProductImage($(ui.draggable).attr("path"), $(ui.draggable).attr("filename"), parseInt($(this).attr("data-uid")), $(this));

                },
                tolerance: 'pointer'
            });
        };

        var DragDropProduct = function () {
            $divProduct.find(".vis-group-user-profile").draggable({
                start: function (event, ui) {
                    ui.helper.css("width", prodContainerWidth);
                    ui.helper.addClass('VA005-dragging');
                },
                stop: function (event, ui) {
                    ui.helper.removeClass('VA005-dragging');
                },
                cursor: "move",
                cursorAt: { left: 5 },
                revert: "invalid",
                helper: "clone",
                //containment: "document"
            });

            $divRelated.droppable({
                hoverClass: "VA005-dropping",
                drop: function (event, ui) {
                    if (($(ui.draggable)).data('uid') > 0) {
                        SetRelated(($(ui.draggable)).data('uid'));
                    }
                }
            });
        };

        function DeleteImages() {
            if (selectedimg.length > 0) {
                $.ajax({
                    async: false,
                    type: "POST",
                    dataType: "json",
                    contentType: 'application/json',
                    url: VIS.Application.contextUrl + "VA005/ProductManagement/DeleteProductImage",
                    data: JSON.stringify({ images: selectedimg }),
                    success: function (data) {
                        var result = JSON.parse(data);
                        //for (var i = 0; i < result.length; i++)
                        //if (result != null) {
                        //    ctrl.attr('src', result);                        
                        //}
                        $self.GetImages();
                    },
                    error: function (e) {
                        console.log(e);
                    }
                });
            }
        };

        function SetRelated(prdID) {
            $bsyDiv[0].style.visibility = "visible";
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/SetRelatedProduct", { "M_Product_ID": prods[0], "RelatedProduct": prdID }, callbackProdRelated);
        }

        function callbackProdRelated(data) {
            if (data == "") {
                bindRelatedGrid();
            }
            else {
                VIS.ADialog.error(data);
                $bsyDiv[0].style.visibility = "hidden";
            }
        }

        function SaveProductImage(imageUrl, imgName, proID, control) {
            var ctrl = control.find(".vis-group-user-img");
            $.ajax({
                //async: false,
                type: "GET",
                dataType: "json",
                url: VIS.Application.contextUrl + "VA005/ProductManagement/SaveProductImage",
                data: { imagePath: imageUrl, imageName: imgName, productID: proID },
                success: function (data) {
                    var result = JSON.parse(data);
                    //for (var i = 0; i < result.length; i++)
                    if (result != null) {
                        var src = '<img alt="" title="" style="opacity:1;" src="' + result + '">';
                        ctrl.empty();
                        ctrl.append(src);
                    }
                },
                error: function (e) {
                    console.log(e);
                }
            });
        };

        function SaveAttributeValueImage(imageUrl, imgName, proID, control, attributeValue, attimages_id) {
            $bsyDiv[0].style.visibility = "visible";
            var ctrl = control.find("img");
            $.ajax({
                //async: false,
                type: "GET",
                dataType: "json",
                url: VIS.Application.contextUrl + "VA005/ProductManagement/SaveAttributeValueImage",
                data: { imagePath: imageUrl, imageName: imgName, productID: proID, attributeValue: attributeValue, attimages_id: attimages_id },
                success: function (data) {
                    var result = JSON.parse(data);
                    //for (var i = 0; i < result.length; i++)
                    if (result != null) {
                        ctrl.attr('src', result.urls);
                        ctrl.attr('data-attimages_id', result.imgID);
                        ctrl.css({ "height": "auto", "width": "auto" });
                    }
                    $bsyDiv[0].style.visibility = "hidden";
                },
                error: function (e) {
                    console.log(e);
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });
        };


        function GetTreeForLeft() {
            $.ajax({
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                url: VIS.Application.contextUrl + "VA005/VA005_Tree/GetTreeAsString",
                data: JSON.stringify({ editable: true }),
                success: function (data) {
                    if (data != null) {
                        var result = JSON.parse(data);
                        if ($leftTreeDiv && $leftTreeDiv.data("kendoTreeView") != undefined) {
                            $leftTreeDiv.data("kendoTreeView").destroy();
                            $leftTreeDiv.empty();
                        }
                        $leftTreeKeno = $leftTreeDiv.kendoTreeView({
                            dragAndDrop: false,
                            dataSource: result,
                            template: "<div class='va005-parentss' id=#= item.NodeID #><div style='float:left'>" +
                                "<div  class='va005_mouseover'  style='float:left'>" +
                                "<p id=#= item.NodeID #>#= item.text #</p>" +
                                "</div>" +
                                "</div></div>",
                        });
                        $leftTreeDiv.off("click", 'p', editTreeItem);
                        $leftTreeDiv.on("click", 'p', editTreeItem);
                    }
                    $bsyDiv[0].style.visibility = "hidden";
                    $leftTreeDiv.css({
                        "display": "block", "margin-top": "10px", "cursor": "pointer"
                        , "height": "70%", "width": "100%"
                    });
                    $root.find('.k-treeview .k-icon').css({ "background-color": "rgba(var(--v-c-on-secondary), .2)", "border-radius": "4px" });
                    $root.find('.k-top k-bot').css({ "background-color": "rgba(var(--v-c-on-secondary), .2)" });
                    $root.find('.k-widget k-treeview').css({ "height": "70%", "position": "absolute" });
                    $root.find('.VA005-apanel-lb').css({ "overflow": "inherit", "position": "relative" });
                    $leftTreeDiv.height(leftHeight);
                },
                error: function (e) {
                    $bsyDiv[0].style.visibility = "hidden";
                    console.log(e);
                }
            });
        }

        function editTreeItem(e) {
            pgno = 1;
            prods = [];
            uoms = [];
            $queryCat.val(0);
            $searchProduct.val("")
            btnSelectAll.prop("checked", false);
            var nodeID = 0;
            var target = $(e.target);
            parent_ID = target.attr('id');
            clearRightPanel();
            loadParentData();
            // Manish 10/4/2016
            if (attrBottomValuesFlag == true) {
                $btnAttValuesDivClose.trigger("click");
            }
            // end
            $divProduct.find('.vis-group-user-wrap').remove();
            loadProd("", 0, $queryCat.val(), pgno, PAGESIZE, parent_ID);

        };

        function loadParentData() {
            var dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/ProductManagement/LoadParentData", { "M_Product_ID": parent_ID });
            if (dr != null) {
                parentTax = dr["C_TaxCategory_ID"];
                parentCat = dr["M_Product_Category_ID"];
            }

            //var sql = "SELECT C_TaxCategory_ID,M_Product_Category_ID FROM M_Product WHERE M_Product_ID = " + parent_ID;
            //try {
            //    var dr = VIS.DB.executeDataReader(sql);
            //    if (dr.read()) {
            //        parentTax = dr.getInt("C_TaxCategory_ID");
            //        parentCat = dr.getInt("M_Product_Category_ID");
            //        //parentAttribute = dr.getInt("C_GenAttributeSet_ID");
            //        //parentQuality = dr.getInt("VA010_QualityPlan_ID");
            //    }
            //}
            //catch (e) {
            //    console.log(e);
            //}
        };
        /*********/

        //Privilized function
        this.getRoot = function () {
            return $root;
        };

        this.setSize = function (height, width) {
            $table.height(height);
            $td0leftbar.height(height);
            $lb.height(height);
            $divlbMain.height($lb.height() - 43);
            leftHeight = $divlbMain.height() - ($root.find('.VA005-Serach-Query').height() * 2) - 30;
            $leftTreeDiv.height(leftHeight);
            //            $divProduct.height($($root.parent()).height() - 95);
            productAttValues.parent().height($middlePanel.height() - ($root.find(".vis-group-users-container").height() + 130));
            productAttValues.height($middlePanel.height() - ($root.find(".vis-group-users-container").height() + 130));

        };

        this.refreshUI = function () {
            /*Refresh Grid on Focus*/
            if (dRelatedGrid) {
                dRelatedGrid.resize();
            }

            if (cartGrid) {
                cartGrid.resize();
            }

            if ($lb)
                $lb.height($td0leftbar.height());

            if ($divlbMain) {
                $divlbMain.height($lb.height() - 43);
                leftHeight = $divlbMain.height() - ($root.find('.VA005-Serach-Query').height() * 2) - 30;
            }

            if ($divProductInner) {
                var divProduct = $divProductInner.find('.vis-group-user-wrap:eq(0)');
                $divProductInner.find('.vis-group-user-profile').width(divProduct.width() - divProduct.find('.vis-group-user-right:eq(0)').width() - 20);
            }
        };

        /*
        dispose all object used in this form
        */
        this.disposeComponent = function () {
            $self = null;
            if ($root)
                $root.remove();
            $root = null;
            $zoomDiv = null;
            $table = null;
            $td0leftbar = $btnlbToggle = $searchQry = $searchCat = $ulLefttoolbar = $divlbMain = $td2_tr1 = null;
            $lb = null;
            $tr = null;
            $leftPanel = null;
            $catPanel = null;
            $bsyDiv = null;
            middleMain = null;
            $middlePanel = null;
            $rightPanel = null;
            ImagePanel = null;
            $searchProduct = null;
            $queryCat = null;
            $searchProdBtn = null;
            $btnCreateProd = null;
            $divProduct = null;
            $divProductInner = null;
            $divUom = null;
            $divUomGroup = null;
            divUom = null;
            divConversion = null;
            $divVarient = null;
            $divLeftTree = null;
            divtree = null;
            divVarient = null;
            divAttr = null;
            $divCartdata = null;
            $divProductDet = null;
            $ProductDetails = null;
            divZoomProdName = null;
            prodName = null;
            prodUPC = null;
            prodAttributeSet = null;
            prodCategory = null
            prodTaxCat = null;
            prodType = null;
            prodUOM = null;
            divCart = null;
            divNewCart = null;
            divCartList = null;
            ch = null;
            pl = null;
            ml = null;
            isOk = null;
            $maindiv = null;
            $div = null;
            $divPriceMain = null;
            $divSupplier = null;
            $divCart = null;
            $divCartMain = null;
            priceGrid = null;
            supplierGrid = null;
            prodmodtmp = prodtheModTmp = null;
            $divHeadProd = null;
            $divHeadVarient = null;
            catSpan = null;
            btnCopy = null;
            btnImage = null;
            btnPriceList = null;
            btnSupplier = null;
            btnEditMultiple = null;
            btnAddCart = null;
            btnProdCart = null;
            btnUomCart = null;
            btnVarientCart = null;
            btnShowAll = null;
            btnSelectAll = null;
            btnDetails = null;
            btnVarient = null;
            btnUom = null;
            btnCart = null;
            btnAdduom = null;
            btnAddVarient = null;
            btnGenerate = null;
            btnSaveAttr = null;
            btnCancelGen = null;
            btnCancelVarient = null;
            btnSaveUom = null;
            btnCancelUom = null;
            btnImgDelete = null;
            btnUpload = null;
            btnErase = null;
            btnImgCancel = null;
            btnNewCart = null;
            btnRefreshCart = null;
            btnEditCart = null;
            btnSaveScan = null;
            btnCancelScan = null;
            $attrControl = null;
            txtScan = null;
            txtName = null;
            txtValue = null;
            txtUpc = null;
            $txtMul = null;
            $txtDiv = null;
            uomUPC = false;
            cmbOrg = null;
            cmbProductType = null;
            cmbAttributeSet = null;
            cmbTaxCategory = null;
            cmbUOM = null;
            cmbUomTo = null;
            cmbCat = null;
            cmbPriceList = null;
            cmbSupplier = null;
            cmbUomPrice = null;
            cmbCart = null;
            btnAttributeSet = null;
            btnProdCat = null;
            btnTaxCategory = null;
            btnUomZoom = null;
            prods = [];
            uoms = [];
            cons = [];
            attributes = [];
            upcs = [];
            ProdID = null;
            uomID = null;
            c_UomConv_ID = null;
            m_attribute_ID = null;
            upcvalue = null;
            pcat_ID = null;
            productCount = null;
            noPages = null;
            noOfCats = null;
            ProductImgul = null;
            selectedimg = [];
            $slctimg = null;
            //actions        
            uomArray = null;
            curArray = null;
            multiValues = [];
            AD_Column_ID = 0;
            dGrid = null;
            sGrid = null;
            cartGrid = null;
            this.getRoot = null;
            this.disposeComponent = null;
            attrSetId = null;
            parnt = [];
            partChild = [];
            txtEditUPC = null;
            pop = null;
            patr_ID = 0;
            orgid = null;
            options = null;
        };
    };

    VA005.ProductMgtForm.prototype.refresh = function () {
        this.refreshUI();
    };

    //Must Implement with same parameter
    VA005.ProductMgtForm.prototype.init = function (windowNo, frame) {
        //Assign to this Varable
        this.frame = frame;
        this.windowNo = windowNo;
        frame.setTitle("VA005_ProductMgt");
        this.frame.getContentGrid().append(this.getRoot());
        window.setTimeout(function (t) {
            t.Initialize();
            t.initData();
        }, 10, this);
    };

    //Must implement dispose
    VA005.ProductMgtForm.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };

    VA005.ProductMgtForm.prototype.sizeChanged = function (height, width) {
        this.setSize(height, width);
    };

    VA005.updateProductPanel = function (windowNo, AD_Window_ID, AD_Table_ID, prods) {
        var $divProdMultiple = $('<div style="height:100%">');
        var $divMultiple = $('<div style="height:100%">');
        var $busy = null;
        this.windowNo = windowNo;
        var $self = this;
        var ch = null;
        var isOk = true;
        var btnSave;
        var drpColumns;
        var tblGrid, tblBody;
        var _mTab = null;
        var findFields = null;
        var control1, control2;
        var dsAdvanceData = null;
        this.okBtnPressed = false;
        $divMultiple.append($divProdMultiple);
        createBusy();
        setBusy(true);

        $divProdMultiple.load(VIS.Application.contextUrl + 'VA005/ProductManagement/Index/?windowNo=' + $self.windowNo, function (evt) {
            initUI();
            initFind();
            bindEvents();
        });

        function initUI() {
            //right side list   
            drpColumns = $divProdMultiple.find("#VA005_drpColumn_" + $self.windowNo);
            divValue1 = $divProdMultiple.find("#VA005_divValue1_" + $self.windowNo);
            //actions            
            btnSave = $divProdMultiple.find("#VA005_btnSave_" + $self.windowNo);
            //grid 
            tblGrid = $divProdMultiple.find("#VA005_tblQry_" + $self.windowNo);
            tblBody = tblGrid.find("tbody");
        };

        function initFind() {
            VIS.AEnv.getGridWindow($self.windowNo, AD_Window_ID, function (json) {
                if (json.error != null) {
                    VIS.ADialog.error(json.error);    //log error
                    return;
                }
                var jsonData = $.parseJSON(json.result); // widow json
                VIS.context.setContextOfWindow($.parseJSON(json.wCtx), $self.windowNo);// set window context
                var GridWindow = new VIS.GridWindow(jsonData);
                if (GridWindow == null) {
                    return;
                }
                if (GridWindow.getTabs().length > 0) {
                    for (item in GridWindow.getTabs()) {
                        if (GridWindow.getTabs()[item].gridTable.AD_Table_ID == AD_Table_ID) {
                            _mTab = GridWindow.getTabs()[item];
                            break;
                        }
                    }
                }
                if (_mTab == null) {
                    return;
                }
                findFields = _mTab.getFields().sort(SortByName);
                var html = '<option value="-1"> </option>';
                for (var c = 0; c < findFields.length; c++) {
                    // get field
                    var field = findFields[c];
                    if (field.getIsEncrypted())
                        continue;
                    // get field's column name
                    var columnName = field.getColumnName();
                    if (field.getDisplayType() == VIS.DisplayType.Button) {
                        if (field.getAD_Reference_Value_ID() == 0)
                            continue;
                        if (columnName.endsWith("_ID"))
                            field.setDisplayType(VIS.DisplayType.Table);
                        else
                            field.setDisplayType(VIS.DisplayType.List);
                        //field.loadLookUp();
                    }
                    // get text to be displayed
                    var header = field.getHeader();
                    if (header == null || header.length == 0) {
                        // get text according to the language selected
                        header = VIS.Msg.getElement(VIS.context, columnName);
                        if (header == null || header.Length == 0)
                            continue;
                    }
                    // if given field is any key, then add "(ID)" to it
                    if (field.getIsKey())
                        header += (" (ID)");

                    // add a new row in datatable and set values
                    //dr = dt.NewRow();
                    //dr[0] = header; // Name
                    //dr[1] = columnName; // DB_ColName
                    //dt.Rows.Add(dr);
                    html += '<option value="' + columnName + '">' + header + '</option>';
                }
                drpColumns.html(html);
                setBusy(false);
            });
        };

        function bindEvents() {
            drpColumns.on("change", function () {


                // set control at value1 position according to the column selected
                var columnName = drpColumns.val();
                setControlNullValue(true);
                if (columnName && columnName != "-1") {
                    // get field
                    var field = getTargetMField(columnName);
                    // set control at value1 position
                    setControl(true, field);
                    // enable the save row button
                    setEnableButton(btnSave, true);//silverlight comment
                }
                // enable control at value1 position
                setValueEnabled(true);
            });

            tblGrid.on("click", function (e) {
                if (e.target.nodeName === "I") {
                    var index = $(e.target).data("index");
                    dsAdvanceData.splice(index, 1);//  .Tables[0].Rows.RemoveAt(index);
                    bindGrid(dsAdvanceData);
                }
            });
            btnSave.on("click", saveRowTemp);
        };

        function saveAdvanced(callBack) {
            // save all query lines temporarily            

            setBusy(true);
            saveRowTemp();	//	unsaved 
            if (dsAdvanceData) {
                $.ajax({
                    type: "POST",
                    url: VIS.Application.contextUrl + "VA005/ProductManagement/UpdateProduct",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify({ prodIDs: prods, ColumnName: dsAdvanceData }),
                    async: false,
                    success: function (data) {
                        var returnValue = data.result;

                        $self.okBtnPressed = true;
                        if (returnValue != "") {
                            //Done by shifali to remove the extra brackets
                            VIS.ADialog.error("", "", returnValue, "");
                            callBack(returnValue);
                        }
                        setBusy(false);
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        // 
                        console.log(textStatus);
                        setBusy(false);
                        alert("RecordNotSaved");
                        $self.okBtnPressed = false;
                        return;
                    }
                });
            }
        };

        function saveRowTemp() {
            // set column name

            var cVal = drpColumns.val();

            if (!cVal || cVal == "-1")
                return false;
            var colName = drpColumns.find("option:selected").text();
            var colValue = "";
            if (colName == null || colName.trim().length == 0) {
                return false;
            }
            else {
                // set column value
                colValue = cVal.toString();
            }
            // add row in dataset
            addRow(colName, colValue, getControlText(true), getControlValue(true));
            //reset column & operator comboBox
            drpColumns[0].selectedIndex = 0;
            //setControlNullValue();
            setControlNullValue(true);
            return true;
        };

        function addRow(colName, colValue, value1Name, value1Value) {

            if (dsAdvanceData == null)
                dsAdvanceData = [];

            var obj = {};
            obj["KEYNAME"] = colName;
            //dsAdvanceData.Tables[0].Columns.Add(dc);
            obj["KEYVALUE"] = colValue;

            obj["VALUE1NAME"] = value1Name;

            if (value1Name == "")
                obj["VALUE1VALUE"] = "";
            else {
                if (value1Value == null)
                    obj["VALUE1VALUE"] = "NULL";
                else
                    obj["VALUE1VALUE"] = VIS.Utility.Util.getValueOfString(value1Value);
            }

            dsAdvanceData.push(obj);
            bindGrid(dsAdvanceData);//for the time beeing commented today 3Dec.2010
        };

        function SortByName(a, b) {

            var aName = a.vo.Header.toLowerCase();
            var bName = b.vo.Header.toLowerCase();
            if (aName == "") {
                aName = a.vo.ColumnName.toLowerCase();
            }
            if (bName == "") {
                bName = b.vo.ColumnName.toLowerCase();
            }
            return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
        };

        function bindGrid(list) {
            tblBody.empty();
            var html = "";
            var htm = "", obj = null;

            if (list) {
                for (var i = 0, j = list.length; i < j; i++) {
                    htm = '<tr class="vis-advancedSearchTableRow">';
                    obj = list[i];
                    htm += '<td>' + obj["KEYNAME"] + '</td><td style="display:none">' + obj["KEYVALUE"] + '</td>' +
                        '<td>' + obj["VALUE1NAME"] + '</td><td style="display:none">' + obj["VALUE1VALUE"] + '</td>' +
                        '<td><i style="cursor:pointer" data-index = "' + i + '" class="vis vis-delete"></i></td>';
                    htm += '</tr>';
                    html += htm;
                }
            }
            tblBody.html(html);
        };

        function getTargetMField(columnName) {
            // if no column name, then return null
            if (columnName == null || columnName.length == 0)
                return null;
            // else find field for the given column
            for (var c = 0; c < findFields.length; c++) {
                var field = findFields[c];
                if (columnName.equals(field.getColumnName()))
                    return field;
            }
            return null;
        };

        function getControlValue(isValue1) {
            var crtlObj = null;
            // get control
            if (isValue1) {
                // crtlObj = (IControl)tblpnlA2.GetControlFromPosition(2, 1);
                crtlObj = control1;
            }
            // if control exists
            if (crtlObj != null) {
                // if control is any checkbox
                if (crtlObj.getDisplayType() == VIS.DisplayType.YesNo) {
                    if (crtlObj.getValue().toString().toLowerCase() == "true") {
                        return "Y";
                    }
                    else {
                        return "N";
                    }
                }
                // return control's value
                return crtlObj.getValue();
            }
            return "";
        };

        /* <param name="isValue1">true if get control's text at value1 position else false</param>
         */
        function getControlText(isValue1) {
            var crtlObj = null;
            // get control
            if (isValue1) {
                // crtlObj = (IControl)tblpnlA2.GetControlFromPosition(2, 1);
                crtlObj = control1;
            }
            // if control exists
            if (crtlObj != null) {
                // get control's text
                return crtlObj.getDisplay();
            }
            return "";
        };

        function setControlNullValue(isValue2) {
            var crtlObj = null;
            if (isValue2) {
                crtlObj = control1;
            }

            // if control exists
            if (crtlObj != null) {
                crtlObj.setValue(null);
            }
        };

        function setValueEnabled(isEnabled) {
            // get control
            var ctrl = divValue1.children()[1];
            var btn = null;
            if (divValue1.children().length > 2)
                btn = divValue1.children()[2];

            if (btn)
                $(btn).prop("disabled", !isEnabled).prop("readonly", !isEnabled);
            else if (ctrl != null) {
                $(ctrl).prop("disabled", !isEnabled).prop("readonly", !isEnabled);
            }
        };

        function setEnableButton(btn, isEnable) {
            btn.prop("disabled", !isEnable);
        };

        function setControl(isValue1, field) {
            // set column and row position
            /*****Get control form specified column and row from Grid***********/
            if (isValue1)
                control1 = null;
            control2 = null;
            var ctrl = null;
            var ctrl2 = null;
            if (isValue1) {
                ctrl = divValue1.children()[1];
                if (divValue1.children().length > 2)
                    ctrl2 = divValue1.children()[2];
            }

            //Remove any elements in the list
            if (ctrl != null) {
                $(ctrl).remove();
                if (ctrl2 != null)
                    $(ctrl2).remove();
                ctrl = null;
            }
            /**********************************/
            var crt = null;
            // if any filed is given
            if (field != null) {
                // if field id any key, then show number textbox 
                if (field.getIsKey()) {
                    crt = new VIS.Controls.VNumTextBox(field.getColumnName(), false, false, true, field.getDisplayLength(), field.getFieldLength(),
                        field.getColumnName());
                }
                else {
                    crt = VIS.VControlFactory.getControl(null, field, true, true, false);
                }
            }
            else {
                // if no field is given show an empty disabled textbox
                crt = new VIS.Controls.VTextBox("columnName", false, true, false, 20, 20, "format",
                    "GetObscureType", false);// VAdvantage.Controls.VTextBox.TextType.Text, DisplayType.String);
            }
            if (crt != null) {
                //crt.SetIsMandatory(false);
                crt.setReadOnly(false);

                if (VIS.DisplayType.Text == field.getDisplayType() || VIS.DisplayType.TextLong == field.getDisplayType()) {
                    crt.getControl().attr("rows", "1");
                    crt.getControl().css("width", "100%");
                }
                else if (VIS.DisplayType.YesNo == field.getDisplayType()) {
                    crt.getControl().css("clear", "both");
                }
                else if (VIS.DisplayType.IsDate(field.getDisplayType())) {
                    crt.getControl().css("line-height", "1");
                }

                var btn = null;
                if (crt.getBtnCount() > 0 && !(crt instanceof VIS.Controls.VComboBox))
                    btn = crt.getBtn(0);

                if (isValue1) {

                    divValue1.append(crt.getControl());
                    control1 = crt;
                    if (btn) {
                        divValue1.append(btn);
                        //crt.getControl().css("width", "65%");
                        crt.getControl().css("width", "calc(100% - 30px)");
                        btn.css("max-width", "35px");
                    }
                }
            }
        };

        function createBusy() {
            $busy = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
            //$busy.css({
            //    "position": "absolute", "width": "98%", "height": "97%", 'text-align': 'center'
            //});
            $divMultiple.append($busy);
        };

        function setBusy(busy) {
            isBusy = busy;
            $busy.css("visibility", isBusy ? "visible" : "hidden");
        };

        function updateEvents() {
            ch.onOkClick = function (e) {
                isOk = true;
                saveAdvanced(saveCallback);
                return isOk;
                //loadProd($searchProduct.val(), pcat_ID, $queryCat.val(), pgno, PAGESIZE);
            };
            ch.onCancelClick = function () {
                $self.okBtnPressed = false;
                ch.close();
            };
        };

        function saveCallback(data) {
            if (data == "") {
                ch.close();
            }
            else {
                isOk = false;
            }
        }

        function unBindEvents() {
            drpColumns.off("change");
            tblGrid.off("click");
            btnSave.off("click");
        };

        this.show = function () {
            setBusy(true);
            ch = new VIS.ChildDialog();
            ch.setHeight(550);
            ch.setWidth(600);
            ch.setTitle(VIS.Msg.getMsg("VA005_UpdateProduct"));
            ch.setModal(true);
            //Disposing Everything on Close
            ch.onClose = function () {
                if ($self.onClose)
                    $self.onClose();
            };

            ch.show();
            ch.setContent($divMultiple);
            updateEvents();
        };

        this.disposeComponent = function () {
            unBindEvents();
            btnSave = null;
            drpColumns = null;
            if ($divMultiple)
                $divMultiple.remove();
            $divMultiple = null;
            dsAdvanceData = null;
            control1 = control2 = null;
            $busy = null;
            this.windowNo = null;
            $self = null;
            ch = null;
            tblGrid = tblBody = null;
            _mTab = null;
            findFields = null;
            this.okBtnPressed = null;
        };
    };

    VA005.updateProductPanel.prototype.dispose = function () {
        this.disposeComponent();
    };

})(VA005, jQuery);