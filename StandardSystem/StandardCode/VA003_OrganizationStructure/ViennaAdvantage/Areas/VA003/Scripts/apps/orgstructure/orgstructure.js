/********************************************************
 * Module Name    : VA003
 * Purpose        : Create Organization Structure
 * Class Used     : 
 * Chronological Development
 * Karan         2 July 2015
 ******************************************************/

; VA003 = window.VA003 || {};
; (function (VA003, $) {

    function OrgStructure() {
        this.frame = null;
        this.windowNo = null;
        var $self = this;
        var ctx = VIS.Env.getCtx();
        var ad_Org_ID = 0;// it will hold org id of current org, on new record it will become 0
        var seletedOrgID = 0;// it will hold org id of current org, on new record it will not become 0
        var selecedSummayNode = 0;// it will hold summary node of current org


        var tableName = "";
        var AD_Tree_ID = 0;
        var $root = $('<div class="VA003-content-wrap">');
        var $leftTreeContainer = $('<div>');
        var $middleContainer = $('<div style="width:200px:height:100%;overflow:auto">');
        var $leftdivContainer = $('<div class="VA003-leftMiddle-wrap">');
        var $rightdivContainer = $('<div class="VA003-right-slide" data-isexpanded="Y">');
        var $buttonsDiv = null;
        var $divFormWrap = null;
        var divLeftTree = $('<div >');
        var divTreeContainer = null;
        var $divRightTree = null;
        var $divHierarchyCombo = null;

        //var $cmbTenant = null;
        var $txtSerackKey = null;
        var $txtName = null;
        //var $txtDesc = null;
        var $chkIsLegal = null;
        var $chkIsSummary = null;
        var $chkIsActive = null;
        var $cmbOrgType = null;
        // var $cmbParentaOrg = null;
        var $txtTax = null;
        var $txtPhone = null;
        var $txtEmail = null;
        var $txtFax = null;
        //var $txtOrgSuperviser = null;
        var $txtLocation = null;
        //var $cmbWarehouse = null;
        var $imgUpload = null;
        var $imageControl = null;
        var $lblimgUpload = null;
        var $btnRemoveBtn = null;
        var $cmbReportHirerchy = $('<select>');
        var $lblOrgInfo = null;
        var allLookupLoaded = false;

        var $btnNewLegalEntity = null;
        var $btnSummary = null;
        var $btnaddNewOrg = null;
        var $btnSave = null;
        var $btnUndo = null;
        var $btnAddNode = null;
        var $btnRefreshRight = null;
        var $btnAddNewTree = null;
        var $btnSlider = null;// $('<a class="VA003-anchorSlide"></a>');
        var $btnInfo = null;
        var $btncloseChart = null;
        var $ulOverlay = null;
        var $btnOpenOverlay = null;

        var nameLength = 0;
        var valueLength = 0;
        var treeLength = 0;
        var ad_window_Id = 0;


        // New elements-mohit
        var $liShowOrgUnit = null;
        var $showOrUnits = null;
        var $labelShowOrgUnit = null;
        var $chkIsCostCenter = null;
        var $lblCostCenter = null;
        var $chkIsProfitCenter = null;
        var $lblProfitCenter = null;

        //var $btnAddChildNode = null;

        var $btnGenerateReport = $('<button class="VA003-genrate-report"><i class="vis vis-cog"></i>' + VIS.Msg.getMsg("VA003_GenrateReportingHierarchy") + '</button>');
        var $btnRefresh = $('<button class="VA003-refresh"><i class="vis vis-refresh"></i></button>');
        var $btnExpandLeftTree = $('<button title="' + VIS.Msg.getMsg('VA003_ExpandTree') + '" class="VA003-expand"></button>');
        var $btnExpandRightTree = null;
        var $toolbarDiv = $('<div class="vis-awindow-header vis-menuTitle">');
        var $bsyDiv;

        var oldValues = null;
        var newValues = {};
        var isSummarySelected = false;
        var isLegalDisable = false;
        var isHeaderNode = true;
        var lstTreeParentIds = [];
        var lstRightTreeParentIds = [];
        //var canDrop = true;
        var needSave = false;
        var formHeight = 0;
        var formWidth = 0;
        var $ulRightTree = null;
        var infoRoot = null;
        var refreshTree = false;
        var whereclause = null;

        /*
          Initialize Components
      */
        this.initializeComponent = function () {
            createHeader();
            createTopButtons();
            createLeftPanel();
            createCenterPanel();
            createRightPanel();
            createBusyIndicator();
            $bsyDiv[0].style.visibility = "visible";
            loadLookups();
            getTree();
            //loadOrgData(11, true);
            //$root.append($leftTreeContainer);
            createOverlay();

            createFirstTabInfo();
            createSecondtabInfo();
            createThirdTabInfo();

            eventHanling();
        };

        function createHeader() {
            $btncloseChart = $('<a href="javascript:void(0)"  class="vis-icon-menuclose vis vis-mark"></a>');

            var pheader = $('<p>' + VIS.Msg.getMsg("VA003_OrgStructure") + ' </p>');
            $root.append($toolbarDiv.append($btncloseChart).append(pheader));
            $btncloseChart.click(function (e) {
                if ($self.frame)
                    $self.frame.dispose();

                $self = null;
                e.stopPropagation();
            });
        }


        function createLeftPanel() {
            divTreeContainer = $('<div style="padding: 0px;border-right: white;" class="VA003-left-tree-wrap">');
            $leftdivContainer.append($('<div class="VA003-left-tree" style="padding: 0px;overflow:auto">').append(divTreeContainer)).append($('<div class="VA003-div-reportgeneratediv">').append($btnGenerateReport).append($btnExpandLeftTree).append($btnRefresh));

            var treeHeight = $leftdivContainer.outerHeight() - ($buttonsDiv.outerHeight() + $btnGenerateReport.outerHeight() + 130);
            divTreeContainer.parent().height(treeHeight);

        };

        function createBusyIndicator() {
            $bsyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
            //$bsyDiv.css("position", "absolute");
            //$bsyDiv.css("bottom", "0");
            //$bsyDiv.css("background", "url('" + VIS.Application.contextUrl + "Areas/VIS/Images/busy.gif') no-repeat");
            //$bsyDiv.css("background-position", "center center");
            //$bsyDiv.css("width", "98%");
            //$bsyDiv.css("height", "98%");
            //$bsyDiv.css('text-align', 'center');
            //$bsyDiv.css('z-index', '1000');
            $bsyDiv[0].style.visibility = "hidden";
            $root.append($bsyDiv);
        };

        function createRightPanel() {

            var $divRightInnerContainer = $('<div class="VA003-rightinnerwrapper">');



            $rightdivContainer.append($divRightInnerContainer);

            var $addDiv = $('<div>');

            $ulRightTree = $('<ul class="VA003-topRight-icons">');
            $btnRefreshRight = $('<li><a  style="padding:0px" ><span style="height:31px;width:31px" class="VA003-Refresh-icon vis vis-refresh"></span></a></li>');
            $btnAddNode = $('<li><a  style="padding:0px" title="' + VIS.Msg.getMsg('VA003_AddNode') + '"><span style="height:31px;width:31px" class="VA003-AddNode-icon vis vis-tl-2"></span></a></li>');
            $btnAddNewTree = $('<li><a style="padding:0px" title="' + VIS.Msg.getMsg('VA003_AddTree') + '"><span style="height:31px;width:31px" class="VA003-AddTree-icon vis vis-plus"></span></a></li>');
            $btnExpandRightTree = $('<li ><a style="padding:0px" title="' + VIS.Msg.getMsg('VA003_ExpandTree') + '"><span style="height:31px;width:31px" class="VA003-Expand-icon"></span></a></li>');


            $ulRightTree.append($btnRefreshRight).append($btnAddNewTree).append($btnAddNode).append($btnExpandRightTree);


            //$btnAddNode = $('<button class="VA003-genrate-repor" style="float: right;">' + VIS.Msg.getMsg("VA003_AddNode") + '</button>');

            //$btnAddNewTree = $('<button class="VA003-genrate-repor" style="float: right;">' + VIS.Msg.getMsg("VA003_AddTree") + '</button>');

            $divRightInnerContainer.append($('<div class="VA003-right-top">').append('<h4>' + VIS.Msg.getMsg('VA003_ReportingHierarchy') + '</h4>'));
            //.append($btnAddNode));

            $root.append($rightdivContainer);

            var $DivRightTopWrap = $('<div class="VA003-righttopdatawrap">');

            $divHierarchyCombo = $('<div class="VA003-form-data-drp">');

            $divRightInnerContainer.append($DivRightTopWrap);
            $DivRightTopWrap.append($divHierarchyCombo.append($cmbReportHirerchy)).append($addDiv.append($ulRightTree));

            $divRightTree = $('<div style="float:left;width:100%" id="' + $self.windowNo + 'orgrighttree">');

            $divRightInnerContainer.append($divRightTree);

            ChangeHeirarcyCmboWidth();
        };

        function createOverlay() {
            $ulOverlay = $('<ul  class="vis-apanel-rb-ul">');
            $ulOverlay.append('<li data-action="zoom" title=' + VIS.Msg.getMsg('Zoom') + ' style="opacity: 1"><i data-action="zoom" class="vis vis-find"></i></li>');
            $ulOverlay.append('<li data-action="refresh"  title=' + VIS.Msg.getMsg('Refresh') + ' ><i data-action="refresh" class="vis vis-refresh"></i></li>');
        };

        function eventHanling() {

            $btnNewLegalEntity.on("click", function (e) {
                if ($btnNewLegalEntity.css('opacity') == "0.5") {
                    return;
                }
                closeSlider();
                newLegalEntity();
            });

            $btnSummary.on("click", function (e) {
                if ($btnSummary.css('opacity') == "0.5") {
                    return;
                }
                closeSlider();
                newSummary();
            });

            $btnaddNewOrg.on("click", function (e) {
                if ($btnaddNewOrg.css('opacity') == "0.5") {
                    return;
                }
                closeSlider();
                addNewOrg();
            });

            $showOrUnits.on('change', function (e) {
                refreshLeftTree();



            });

            $btnSave.on("click", save);
            $btnUndo.on("click", undo);
            $lblimgUpload.on("change", changeOrgPic);
            $btnRemoveBtn.on("click", removeIcon);
            $chkIsLegal.on("click", chklegalEntity);
            $chkIsSummary.on("click", chkSummary);
            $btnGenerateReport.on("click", generateReport);
            $cmbReportHirerchy.on("change", hierarchyChange);
            $btnAddNode.on("click", addNode);
            $btnRefreshRight.on("click", hierarchyChange);
            $btnAddNewTree.on("click", addnewTree);
            $btnSlider.on("click", slide);
            $btnRefresh.on("click", refreshLeftTree);
            $btnInfo.on("click", showInfo);
            $btnExpandLeftTree.on("click", leftTreeExpanderClick);
            $btnExpandRightTree.on("click", rightTreeExpanderClick);

            $txtName.on("change", function () {
                if ($txtName.val().length > 0) {
                    //$txtName.css("background-color", "white");
                    $txtName.removeClass('vis-ev-col-mandatory');
                }
                else {
                    //$txtName.css("background-color", "#ffb6c1");
                    $txtName.addClass('vis-ev-col-mandatory');
                }
            });

            //$txtSerackKey.on("change", function () {
            //    if ($txtSerackKey.val().length > 0) {
            //        $txtSerackKey.css("background-color", "white");
            //    }
            //    else {
            //        $txtSerackKey.css("background-color", "#ffb6c1");
            //    }
            //});

            $btnOpenOverlay.on("click", openOverLay);

            $chkIsActive.on("change", activeChange);

            $ulOverlay.on(VIS.Events.onTouchStartOrClick, "LI", overlayClicked);

        };

        function activeChange(e) {
            if (!$chkIsActive.prop("checked")) {
                var selected = divLeftTree.data("kendoTreeView").select();

                var childd = divLeftTree.data("kendoTreeView").select().find('.k-group').find('.data-id');

                if (!checkActiveChild(childd)) {
                    setStatus(true);
                    needSave = false;
                    VIS.ADialog.info("VA003_SetInActive");
                    $chkIsActive.prop("checked", true);
                }

            }
            else {
                var paerent = divLeftTree.data("kendoTreeView").parent(divLeftTree.data("kendoTreeView").select());
                if (parent) {
                    if ($(paerent.find('.data-id')[0]).data("active") == false) {
                        setStatus(true);
                        needSave = false;
                        VIS.ADialog.info("VA003_SetActive");
                        $chkIsActive.prop("checked", false);
                    }
                }
            }
        };

        function checkActiveChild(lstChild) {
            var allowInActive = true;
            if (lstChild != null && lstChild.length > 0) {
                for (var i = 0; i < lstChild.length; i++) {
                    if ($(lstChild[i]).data("active")) {
                        allowInActive = false;
                        break;
                    }
                }
            }

            return allowInActive;
        };

        function removeIcon() {

            var file = $imgUpload[0].files[0];
            var deletse = false;
            if (file) {
                deletse = true;
            }
            else if ($imageControl.attr('src')) {
                deletse = true;
            }


            if (deletse) {
                if (!VIS.ADialog.ask("DeleteIt")) {
                    return;
                }

                //var res = VIS.DB.executeQuery("UPDATE AD_OrgInfo SET Logo=null WHERE AD_ORg_ID=" + ad_Org_ID);
                var res = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "OrgStructure/UpdateLogo", { AD_ORg_ID: ad_Org_ID });
                if (res > 0) {
                    hideImage();
                }
            }

        };

        function openOverLay() {
            $btnOpenOverlay.w2overlay($ulOverlay.clone(true));
        };

        function overlayClicked(e) {
            var action = $(e.target).data("action");
            $bsyDiv[0].style.visibility = "visible";
            if (action == VIS.Actions.refresh) {

                $.ajax({
                    url: VIS.Application.contextUrl + "OrgStructure/RefreshOrgType",
                    async: true,
                    success: function (result) {
                        var AllOrgType = JSON.parse(result);
                        $cmbOrgType.empty();
                        for (var i = 0; i < AllOrgType.length; i++) {
                            $cmbOrgType.append('<option value=' + AllOrgType[i].Key + '>' + AllOrgType[i].Name + '</option>');
                        }
                        $bsyDiv[0].style.visibility = "hidden";
                    },
                    error: function (e) {
                        console.log(e);
                        $bsyDiv[0].style.visibility = "hidden";
                    }
                });


            }
            else if (action == VIS.Actions.zoom) {
                zoomToWindow(VIS.Utility.Util.getValueOfInt($cmbOrgType.val()), "Organization Type");
            }
        };


        var zoomToWindow = function (record_id, windowName) {
            //var sql = "select ad_window_id from ad_window where name = '" + windowName + "'";// Upper( name)=Upper('user' )
            try {
                //var dr = VIS.DB.executeDataReader(sql);
                //if (dr.read()) {
                //    ad_window_Id = dr.getInt(0);
                //}
                //dr.dispose();
                if (ad_window_Id == 0) {
                    ad_window_Id = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "OrgStructure/ZoomToWindow", null);
                }
                if (ad_window_Id > 0) {
                    var zoomQuery = new VIS.Query();
                    zoomQuery.addRestriction("AD_OrgType", VIS.Query.prototype.EQUAL, record_id);
                    zoomQuery.setRecordCount(1);
                    VIS.viewManager.startWindow(ad_window_Id, zoomQuery);
                }
                $bsyDiv[0].style.visibility = "hidden";
            }
            catch (e) {
                $bsyDiv[0].style.visibility = "hidden";
                console.log(e);
            }

        };

        function createTopButtons() {
            $buttonsDiv = $('<div class="VA003-top-buttons">');

            var $ulLeft = $(' <ul class="VA003-topLeft-icons">');
            $btnSummary = $('<li><a title="' + VIS.Msg.getMsg('VA003_AddNode') + '" class="VA003-org-icon"></a></li>');
            $btnNewLegalEntity = $('<li><a title="' + VIS.Msg.getMsg('VA003_AddLegalEntity') + '" class="VA003-entity-icon"></a></li>');
            $btnaddNewOrg = $(' <li><a title="' + VIS.Msg.getMsg('VA003_AddOrgUnit') + '" class="VA003-orgUnit-icon"></a></li>');

            $btnSlider = $(' <li><a title="' + VIS.Msg.getMsg('Edit') + '" class="VA003-edit-icon"></a></li>');
            $liShowOrgUnit = $('<li class="VA003-shworgchkwrp"></li>');
            $showOrUnits = $('<input type="checkbox" data-name="legal" >');

            $labelShowOrgUnit = $('<li><label >' + VIS.Msg.getMsg("VA003_ShowOrgUnits") + '</label></li>');
            $liShowOrgUnit.append($showOrUnits).append($labelShowOrgUnit);
            $btnInfo = $btnHdrSend = $('<i class="VA003-InfoIcon vis vis-info" title="' + VIS.Msg.getMsg("VA003_info").replace('&', '') + '"> </i>'); //$(' <a title="' + VIS.Msg.getMsg('VA003_info') + '" class="VA003-info-icon"></a>');

            $toolbarDiv.append($btnInfo);

            $ulLeft.append($btnSummary).append($btnNewLegalEntity).append($btnaddNewOrg).append($btnSlider).append($liShowOrgUnit);//.append($btnInfo);//.append($btnAddChildNode);

            var $ulRight = $('<ul class="VA003-topRight-icons">');
            $btnUndo = $('<li><a title="' + VIS.Msg.getMsg('VA003_Undo') + '"><span class="VA003-undo-icon vis vis-ignore"></span></a></li>');
            $btnSave = $('<li><a title="' + VIS.Msg.getMsg('VA003_Save') + '"><span class="VA003-undo-icon vis vis-save"></span></a></li>');
            $ulRight.append($btnUndo).append($btnSave);

            setStatus(true);

            $buttonsDiv.append($ulLeft).append($ulRight);

            $leftdivContainer.append($buttonsDiv);
            $root.append($leftdivContainer);

        };

        function getTree() {
            $.ajax({
                url: VIS.Application.contextUrl + "OrgStructure/GetTree",
                async: true,
                data: { windowNo: $self.windowNo, showOrgUnits: $showOrUnits.is(':checked') },
                success: function (result) {
                    divTreeContainer.append(divLeftTree);
                    lstTreeParentIds = [];
                    createLeftTree(result);
                }
            });



            //[
            //    {
            //        text: 'IdeasInc. Organization', expanded: true, 'data-tableName': 'AD_TreeNode', nodeid: 0, items: [
            //          { text: 'HQ', issummary: false, nodeid: 11 },
            //          {
            //              text: 'ABC', issummary: true, nodeid: 1000027, items: [
            //                { text: 'Store', issummary: false, nodeid: 12 },
            //                {   text: 'HQ1', issummary: true, nodeid: 1000025, items: [
            //                      { text: 'HQ Digital', issummary: false, nodeid: 1000026 },
            //                    ]
            //                },
            //              ]
            //          },
            //          { text: 'Legal', issummary: false, nodeid: 1000028 },
            //        ]
            //    }
            //]

        };

        function refreshLeftTree() {
            getTree();
        };


        function leftTreeExpanderClick() {
            if ($btnExpandLeftTree.hasClass('VA003-expand')) {
                $btnExpandLeftTree.removeClass('VA003-expand');
                $btnExpandLeftTree.addClass('VA003-collapase');
                divLeftTree.data("kendoTreeView").expand(".k-item");
                $btnExpandLeftTree.prop("title", VIS.Msg.getMsg('VA003_CollapseTree'));
            }
            else {
                $btnExpandLeftTree.removeClass('VA003-collapase');
                $btnExpandLeftTree.addClass('VA003-expand');
                divLeftTree.data("kendoTreeView").collapse(".k-item");
                $btnExpandLeftTree.prop("title", VIS.Msg.getMsg('VA003_ExpandTree'));
            }
        };

        function rightTreeExpanderClick() {
            var spa = $btnExpandRightTree.find('span');

            if (spa.hasClass('VA003-Expand-icon')) {
                spa.removeClass('VA003-Expand-icon');
                spa.addClass('VA003-collapse-icon');
                $divRightTree.data("kendoTreeView").expand(".k-item");
                spa.prop("title", VIS.Msg.getMsg('VA003_CollapseTree'));
            }
            else {
                spa.removeClass('VA003-collapse-icon');
                spa.addClass('VA003-Expand-icon');
                $divRightTree.data("kendoTreeView").collapse(".k-item");
                spa.prop("title", VIS.Msg.getMsg('VA003_ExpandTree'));
            }
        };


        function createLeftTree(result) {
            if (result == null || result == undefined) {
                return null;
            }



            var data = JSON.parse(result);

            tableName = data[0].TableName;
            AD_Tree_ID = data[0].AD_Tree_ID;


            if (divLeftTree.data("kendoTreeView") != undefined) {
                divLeftTree.data("kendoTreeView").destroy();
                divLeftTree.empty();
            }

            divLeftTree.kendoTreeView({
                dataSource: data,
                dragAndDrop: true,
                drop: onDrop,
                dragstart: onDragStart,
                select: onSelect,
                // template: "#= item.text #<input type='hidden' class='data-id' value='#= item.NodeID #' data-summary='#= item.IsSummary #' />"

                template: "<img src='" + VIS.Application.contextUrl + "#= item.ImageSource #' style='vertical-align: text-top;float: left;margin: 4px 0px 0px 10px;'><i class='#= item.ImageSource #'></i>" +
                    "<p style='min-width:122px;border-radius:4px;margin:0px;padding: 7px 10px 7px 38px; background-color:#= item.bColor #; color: #= item.color #'>#= item.text #</p>" +
                    "<input type='hidden' class='data-id' value='#= item.NodeID #' data-active='#= item.IsActive #' data-summary='#= item.IsSummary #' data-orgparentid='#= item.OrgParentID #' data-parentid='#= item.ParentID #'  " +
                    " data-legal='#= item.IsLegal #' data-isorgunit='#= item.IsOrgUnit #' data-Treeparentid='#= item.TreeParentID #' data-treeid='" + AD_Tree_ID + "' />",
            });

            divLeftTree.data("kendoTreeView").select(".k-first");


            window.setTimeout(function () {

                //divLeftTree.find('.k-treeview').css({ 'border-color': 'transparent', 'background-color': 'transparent', '-webkit-box-shadow': 'none' });
                divLeftTree.find('.k-in').css({ 'cursor': 'pointer' });
                //divLeftTree.find('.k-state-hover').css({ 'border-color': 'transparent', 'background-color': 'transparent', '-webkit-box-shadow': 'none' });
                //divLeftTree.find('.k-state-focused').css({ 'border-color': 'transparent', 'background-color': 'transparent', '-webkit-box-shadow': 'none' });
                //divLeftTree.find('.k-state-selected').css({ 'border-color': 'rgba(229, 228, 225, 1)', 'background-color': '#DADADA', '-webkit-box-shadow': 'rgba(229, 228, 225, 1)'});
                //divLeftTree.find('.k-state-selected').css({ 'border-color': 'rgba(229, 228, 225, 1)', 'background-color': '#DADADA', '-webkit-box-shadow': 'rgba(229, 228, 225, 1)', 'box-shadow': 'transparent' });
                //divLeftTree.find('.k-state-focused.k-state-selected').css({ 'box-shadow': 'none', '-webkit-box-shadow': 'none' });
            }, 100);

            $self.sizeChanged();
            setEanbleDisableControls(true);
            parentNodeSelected();
            setMandatoryColor(false);
            $bsyDiv[0].style.visibility = "hidden";
        };

        function createCenterPanel() {
            $divFormWrap = $('<div class="VA003-form-wrap vis-formouterwrpdiv">');


            var $newDiv = $('<div class="VA003-forminnerwrap">');


            var $divFormTop = $('<div class="VA003-form-top">');
            var $divTopFields = $('<div class="VA003-form-top-fields">');

            $newDiv.append($divFormTop)

            $lblOrgInfo = $('<label class="VA003-formheadingwrap">' + VIS.Msg.getMsg('VA003_OrgInfo') + '</label>');

            $divFormWrap.append($newDiv.append($divFormTop.append($('<div class="VA003-form-data"></div>').append($lblOrgInfo)).append($divTopFields)));

            //$cmbTenant = $('<select data-name="tenant">');
            //$divTopFields.append($(' <div class="VA003-form-data">').append('<label>' + VIS.Msg.getMsg("Tenant") + '</label>').append($cmbTenant));
            //valueChangeEvent($cmbTenant);

            //$divTopFields.append($('<div class="VA003-form-data"></div>').append($lblOrgInfo));

            $txtSerackKey = $('<input type="text" data-name="searchkey">');
            $divTopFields.append($('<div class="VA003-form-data input-group vis-input-wrap">').append($('<div class="vis-control-wrap">').append($txtSerackKey).append('<label>' + VIS.Msg.getMsg("SearchKey") + '</label>')));
            valueChangeEvent($txtSerackKey);

            $txtName = $('<input type="text" data-name="name">');
            $divTopFields.append($(' <div class="VA003-form-data input-group vis-input-wrap">').append($('<div class="vis-control-wrap">').append($txtName).append('<label>' + VIS.Msg.getMsg("Name") + '</label>')));
            valueChangeEvent($txtName);



            /*******************************      ORG IMAGE         ***********************/

            var $divImageWrap = $('<div class="VA003-image-wrap">');
            $imageControl = $('  <img src="' + VIS.Application.contextUrl + 'Areas/VA003/Images/orgstr-org-logo.png" alt="Organization Logo">');
            $divImageWrap.append($('<span class="VA003-image-area">').append($imageControl));

            // $imgUpload = $('<input id="VA003-file-input" type="file"accept="image/png">');
            //    $lblimgUpload = $(' <label for="VA003-file-input" class="VA003-file-label">' + VIS.Msg.getMsg('VA003_Browse') + '</label> ');
            $lblimgUpload = $('<a><span class="vis vis-pencil" style="cursor: pointer;"><input type="file" style="position: absolute; top: 0px; right: 0px; opacity: 0;cursor: pointer;"></span></a>');//.append($imgUpload);
            $btnRemoveBtn = $('<a class="vis vis-cross" style="cursor:pointer"></a>');
            $imgUpload = $lblimgUpload.find('input');



            $divImageWrap.append($('<div class="VA003-image-upload">').append($(' <div class="VA003-imageuploadinner"> </div>').append($lblimgUpload).append($btnRemoveBtn)));
            $divFormTop.append($divImageWrap);

            /******************************************************/


            var $divFullFields = $('<div class="VA003-form-fullFields">');
            $newDiv.append($divFullFields);

            //$txtDesc = $('<textarea data-name="desc" style="width:100%">');
            //$divFullFields.append($(' <div class="VA003-form-data">').append('<label>' + VIS.Msg.getMsg("Description") + '</label>').append($txtDesc));
            //valueChangeEvent($txtDesc);


            var lookups = new VIS.MLocationLookup(ctx, $self.windowNo);
            $txtLocation = new VIS.Controls.VLocation("C_Location_ID", false, false, true, VIS.DisplayType.Location, lookups);
            $($txtLocation.getControl()).data("name", "address");
            $divFullFields.append($('<div class="VA003-form-data input-group vis-input-wrap">').append($('<div class="vis-control-wrap">').append($txtLocation.getControl().attr('data-hasbtn', ' ').addClass('VA003-addressControl')).append('<label>' + VIS.Msg.getMsg("Address") + '</label>')).append($('<div class="input-group-append">').append($txtLocation.getBtn(0)).append($txtLocation.getBtn(1))));
            //valueChangeEvent($txtLocation.getControl());

            $txtLocation.fireValueChanged = locationChanged;


            //var lookup = VIS.MLookupFactory.getMLookUp(ctx, $self.windowNo, 10424, VIS.DisplayType.Search);
            //$txtOrgSuperviser = new VIS.Controls.VTextBoxButton("AD_User_ID", true, false, true, VIS.DisplayType.Search, lookup);
            //$($txtOrgSuperviser.getControl()).data("name", "orgsuperwiser");
            //$divFullFields.append($(' <div class="VA003-form-data">').append('<label>' + VIS.Msg.getMsg("VA003_Supervisor") + '</label>').append($txtOrgSuperviser.getControl().css("width", "92%")).append($txtOrgSuperviser.getBtn(0)));
            //valueChangeEvent($txtOrgSuperviser.getControl());

            $btnOpenOverlay = $('<button tabindex="-1" class="vis-controls-txtbtn-table-td2 input-group-text"><i tabindex="-1" class="fa fa-ellipsis-v"></i></button>');

            $cmbOrgType = $('<select data-name="orgtype" data-hasbtn=" ">');
            $divFullFields.append($(' <div class="VA003-form-data input-group vis-input-wrap">').append($('<div class="vis-control-wrap">').append($cmbOrgType).append('<label>' + VIS.Msg.getMsg("VA003_OrgType") + '</label>')).append($('<div class="input-group-append">').append($btnOpenOverlay)));
            valueChangeEvent($cmbOrgType);

            //$cmbParentaOrg = $('<select data-name="parentorg">');
            //$divFullFields.append($(' <div class="VA003-form-data">').append('<label>' + VIS.Msg.getMsg("VA003_ParentOrg") + '</label>').append($cmbParentaOrg));
            //valueChangeEvent($cmbParentaOrg);

            $txtTax = $('<input type="text" data-name="taxid">');
            $divFullFields.append($(' <div class="VA003-form-data input-group vis-input-wrap">').append($('<div class="vis-control-wrap">').append($txtTax).append('<label>' + VIS.Msg.getMsg("VA003_TaxID") + '</label>')));
            valueChangeEvent($txtTax);

            $txtPhone = $('<input type="text" data-name="phone">');
            $divFullFields.append($(' <div class="VA003-form-data input-group vis-input-wrap">').append($('<div class="vis-control-wrap">').append($txtPhone).append('<label>' + VIS.Msg.getMsg("Phone") + '</label>')));
            valueChangeEvent($txtPhone);

            $txtEmail = $('<input type="text"  data-name="email">');
            $divFullFields.append($(' <div class="VA003-form-data input-group vis-input-wrap">').append($('<div class="vis-control-wrap">').append($txtEmail).append('<label>' + VIS.Msg.getMsg("EMail") + '</label>')));
            valueChangeEvent($txtEmail);


            $txtFax = $('<input type="text" data-name="fax">');
            $divFullFields.append($(' <div class="VA003-form-data input-group vis-input-wrap">').append($('<div class="vis-control-wrap">').append($txtFax).append('<label>' + VIS.Msg.getMsg("Fax") + '</label>')));
            valueChangeEvent($txtFax);


            var $divCheckbox = $(' <div class="VA003-form-data">');

            $chkIsLegal = $('<input type="checkbox" data-name="legal">');
            $divCheckbox.append($(' <div style="width:50%; margin-bottom: 15px"  class="VA003-form-data">').append($chkIsLegal).append('<label>' + VIS.Msg.getMsg("VA003_LegalEntity") + '</label>'));
            valueChangeEvent($chkIsLegal);

            $chkIsSummary = $('<input type="checkbox" data-name="summary">');
            $divCheckbox.append($(' <div style="width:50%; margin-bottom: 15px" class="VA003-form-data">').append($chkIsSummary).append('<label>' + VIS.Msg.getMsg("VA003_SummaryLevel") + '</label>'));

            valueChangeEvent($chkIsSummary);

            $chkIsCostCenter = $('<input type="checkbox" data-name="summary">');
            $lblCostCenter = $(' <div style="width:50%; margin-bottom: 15px" class="VA003-form-data">').append($chkIsCostCenter).append('<label>' + VIS.Msg.translate(VIS.Env.getCtx(), "IsCostCenter") + '</label>');
            $divCheckbox.append($lblCostCenter);
            // when we change cost center value, then save button to be enabled
            valueChangeEvent($chkIsCostCenter);

            $chkIsProfitCenter = $('<input type="checkbox" data-name="summary">');
            $lblProfitCenter = $(' <div style="width:50%; margin-bottom: 15px" class="VA003-form-data">').append($chkIsProfitCenter).append('<label>' + VIS.Msg.translate(VIS.Env.getCtx(), "IsProfitCenter") + '</label>');
            $divCheckbox.append($lblProfitCenter);
            // when we change profit center value, then save button to be enabled
            valueChangeEvent($chkIsProfitCenter);

            $divFullFields.append($divCheckbox);

            var $divActiveCheckbox = $(' <div class="VA003-form-data">');
            $chkIsActive = $('<input type="checkbox" data-name="Active">');
            $divActiveCheckbox.append($(' <div style="width:50%" class="VA003-form-data">').append($chkIsActive).append('<label>' + VIS.Msg.getMsg("VA003_Active") + '</label>'));
            valueChangeEvent($chkIsActive);

            $chkIsActive.prop("checked", true);

            $divFullFields.append($divActiveCheckbox);

            //$cmbWarehouse = $('<select data-name="warehouse">');
            //$divFullFields.append($(' <div class="VA003-form-data">').append('<label>' + VIS.Msg.getMsg("Warehouse") + '</label>').append($cmbWarehouse));
            //valueChangeEvent($cmbWarehouse);

            //$btnNewLegalEntity = $('<input type="button" value="New Legal Entity">');
            //$btnSummary = $('<input type="button" value="New Summary Level">');
            //$btnaddNewOrg = $('<input type="button" value="New">');
            //$btnSave = $('<input type="button" value="Save">');
            //$btnUndo = $('<input type="button" value="Undo">');

            //$root.append($middleContainer);
            //$middleContainer.append($cmbTenant).append('</br>').append($txtSerackKey).append('</br>').append($txtName).append('</br>').append($txtDesc).append('</br>').append($chkIsLegal).append('</br>').append($chkIsSummary).append('</br>').append($cmbOrgType).append('</br>')
            //    .append($cmbParentaOrg).append('</br>').append($txtTax).append('</br>').append($txtEmail).append('</br>').append($txtPhone).append('</br>')
            //.append($txtFax).append('</br>').append($txtOrgSuperviser.getControl()).append($txtOrgSuperviser.getBtn(0)).append('</br>').append($cmbWarehouse).append('</br>').append($txtLocation.getControl().css("width", "80%")).append($txtLocation.getBtn(0)).append($txtLocation.getBtn(1))
            //.append('</br>').append($btnNewLegalEntity).append('</br>').append($btnSummary).append('</br>').append($btnaddNewOrg).append('</br>').append($btnSave).append('</br>').append($btnUndo);

            $leftdivContainer.append($divFormWrap);
        };

        function changeorgLabelText(newOrg) {
            if (newOrg) {
                $lblOrgInfo.text(VIS.Msg.getMsg('VA003_AddOrg'));
            }
            else {
                $lblOrgInfo.text(VIS.Msg.getMsg('VA003_OrgInfo'));
            }
        };

        function locationChanged(e) {
            needSave = true;
            //canDrop = false;
            setStatus(false);
        }

        function onDragStart(e) {
            //if (!canDrop) {
            //    e.preventDefault();
            //    return;
            //}

            //var isActive = $(e.sourceNode).find(".data-id").data("active");
            //if (!isActive)
            //{
            //    e.preventDefault();
            //    return false;
            //}


            if (needSave) {
                e.preventDefault();

                window.setTimeout(function () {

                    if (VIS.ADialog.ask("VA003_SaveExisting")) {
                        save();
                    }
                    else {
                        undo();
                    }
                }, 200);
                needSave = false;
                return;
            }


        };

        function onDrop(e) {

            if (!e.valid) {
                e.preventDefault();
                return;
            }

            if (e.dropPosition == "before") {
                e.preventDefault();
                return;
            }
            else if (e.dropPosition == "after") {
                e.preventDefault();
                return;
            }

            var isSourceSummary = $(e.sourceNode).find(".data-id").data("summary");

            var isorgunit = $(e.sourceNode).find(".data-id").data("isorgunit") ? "'Y'" : "'N'";

            if (whereclause != null && whereclause.length > 0 && whereclause.contains("IsOrgUnit")) {
                var orgunit = whereclause.substring(whereclause.indexOf("=") + 1);
                if (isorgunit != orgunit) {
                    e.preventDefault();
                    return;
                }
            }
            else if ($(e.sourceNode).find(".data-id").data("isorgunit")) {
                e.preventDefault();
                return;
            }

            var destree = $(e.destinationNode).find(".data-id").data("treeid");
            var sourTree = $(e.sourceNode).find(".data-id").data("treeid");

            if (sourTree != destree) {


                var isActive = $(e.sourceNode).find(".data-id").data("active");
                if (!isActive) {
                    e.preventDefault();
                    return false;
                }
                e.preventDefault();
                if ($(e.destinationNode).find(".data-id").data("summary")) {
                    addNodeToNewTree(this, e);
                    $cmbReportHirerchy.trigger("change");

                }
                return;
            }



            var isCurrentLegal = $(e.sourceNode).find(".data-id").data("legal");

            if ($(e.destinationNode).find(".data-id").data("summary") != true && isCurrentLegal == true)  // if droping legal item on any other node than summary then don't allow this...
            {
                e.preventDefault();
                return;
            }
            var isSumarry = $(e.destinationNode).find(".data-id").data("summary");


            if (isSourceSummary && !isSumarry)  // summary can be child of summary only
            {
                e.preventDefault();
                return;
            }

            var treeid = $(e.sourceNode).find(".data-id").data("treeid");

            var currentNodeID = $(e.sourceNode).find(".data-id").val();

            var oldID = $(e.sourceNode).find(".data-id").data('treeparentid');  //parent nodeid of  dragging item

            var elements = $.grep(lstTreeParentIds, function (ele, index) {
                return ele.id == currentNodeID;
            });

            if (elements != undefined && elements != null && elements.length > 0) {
                oldID = elements[0].pid;
            }

            //if (isSourceSummary) {
            //    oldID = $(e.sourceNode).find(".data-id").data('parentid');  //parent nodeid of  dragging item
            //}
            //else if (oldID == 0 && !isSourceSummary) {
            //    oldID = $(e.sourceNode).find(".data-id").val();
            //}

            var newIDForOrgInfo = $(e.destinationNode).find(".data-id").val();    //nodeid of  destination node
            if (isSumarry) {
                newIDForOrgInfo = 0;
            }

            var newID = 0;//$(e.destinationNode).find(".data-id").data('treeparentid');

            if ($(e.destinationNode).find(".data-id").data("summary") == true) {
                newID = $(e.destinationNode).find(".data-id").val();
            }
            else if (newID == 0 && isSumarry) {
                newID = $(e.destinationNode).find(".data-id").val();
            }

            if (oldID == newID && isSumarry) {
                if ($(e.destinationNode).find(".data-id").val() > 0) {
                    e.preventDefault();
                    return;
                }
            }

            //   $(e.sourceNode).find(".data-id").data('treeparentid', newID);

            // $(e.sourceNode).find(".data-id")getAttribute("data-myval") //returns "10"

            $(e.sourceNode).find(".data-id")[0].setAttribute("data-treeparentid", newID);

            if (elements != undefined && elements != null && elements.length > 0) {
                elements[0].pid = newID;
            }
            else {
                lstTreeParentIds.push({ 'id': currentNodeID, 'pid': newID });
            }



            $(e.sourceNode).find(".data-id").data("orgparentid", newIDForOrgInfo);

            if (isSumarry) {
                newIDForOrgInfo = 0;
                $(e.sourceNode).find(".data-id").data("parentid", parseInt(newID));
            }
            else {
                $(e.sourceNode).find(".data-id").data("parentid", 0);
            }

            $bsyDiv[0].style.visibility = "visible";

            var oldSiblings = $(e.sourceNode).siblings();       //siblings of dragging item

            var destinChildd = $(e.destinationNode);        //destination node 

            var destinChild = $(e.destinationNode).children('ul').children();// get children (li) of destination UL.
            var activee = $(e.sourceNode).find(".data-id").data("active");




            if (destinChild == null || destinChild == "" || destinChild.length == 0) { // if destination node has not any child yet, the we will allow defalt functaionlity of drag drop. 

                window.setTimeout(function () {     // after waitng when child has been created, we will update sequence  of source and destination
                    destinChild = destinChildd.children('ul').children();
                    updateSequence(oldSiblings, oldID, destinChild, newID, treeid, isSumarry, currentNodeID, newIDForOrgInfo, true);
                    //divLeftTree.data("kendoTreeView").trigger('select', { node: divLeftTree.data("kendoTreeView").select() });



                    divLeftTree.data("kendoTreeView").trigger('select', { node: $(e.sourceNode) });

                    setBackground(isCurrentLegal, isSourceSummary, activee, $(e.sourceNode));

                }, 200);

            }
            else {
                //if destination has children, then we will make sure that new item being droped ,will become first  item of destination. 


                // divLeftTree.data("kendoTreeView").remove(e.sourceNode);

                // $(e.destinationNode).children('ul').prepend(e.sourceNode);  //now append mannuly dragging item as first item.

                window.setTimeout(function () {     // after waitng when child has been created, we will update sequence  of source and destination
                    destinChild = $(e.destinationNode).children('ul').children();

                    if (oldSiblings.length == 1 && oldSiblings.find('.data-id').val() == newID) {
                        oldSiblings = [];
                    }
                    updateSequence(oldSiblings, oldID, destinChild, newID, treeid, isSumarry, currentNodeID, newIDForOrgInfo, true);//we will update sequence  of source and destination

                    divLeftTree.data("kendoTreeView").trigger('select', { node: $(e.sourceNode) });

                    setBackground(isCurrentLegal, isSourceSummary, activee, $(e.sourceNode));
                    //var selectedNode = divLeftTree.data("kendoTreeView").select();

                }, 200);
                //e.preventDefault(); //For this we prevent default functionality as it append item as last item.
                //return;
            }
        };

        function setBackground(isLegal, isSummary, isActive, item) {

            var selectitem = divLeftTree.data("kendoTreeView").findByUid(item.data('uid'));
            divLeftTree.data("kendoTreeView").select(selectitem);


            if (isLegal) {
                if (isActive) {
                    $(e.sourceNode).data('uid')
                    $(divLeftTree.data("kendoTreeView").select().find('p')[0]).css('background-color', "#dc8a20");
                }
                else {
                    $(divLeftTree.data("kendoTreeView").select().find('p')[0]).css('background-color', "#F4C993");
                }
            }
            else if (isSummary) {
                if (isActive) {
                    $(divLeftTree.data("kendoTreeView").select().find('p')[0]).css('background-color', "#0084c4");
                }
                else {
                    $(divLeftTree.data("kendoTreeView").select().find('p')[0]).css('background-color', "rgba(0, 132, 196, 0.7)");
                }
            }
            else {
                if (isActive) {
                    $(divLeftTree.data("kendoTreeView").select().find('p')[0]).css('background-color', "rgba(43, 174, 250, 0.78)");
                }
                else {
                    $(divLeftTree.data("kendoTreeView").select().find('p')[0]).css('background-color', "rgba(166, 222, 255, 1)");
                }
            }
        };

        function onHierarchyDrop(e) {


            if (!e.valid) {
                e.preventDefault();
                return;
            }

            if (e.dropPosition == "before" && $(e.destinationNode).find(".data-id").val() == 0) {
                e.preventDefault();
                return;
            }


            var isDraggingBetween = false;

            if (e.dropPosition == "before") {
                //e.preventDefault();
                //return;
                isDraggingBetween = true;
            }
            else if (e.dropPosition == "after") {
                //e.preventDefault();
                //return;
                isDraggingBetween = true;
            }

            var isSumarry = $(e.destinationNode).find(".data-id").data("summary");

            if (!isSumarry && !isDraggingBetween) {
                e.preventDefault();
                return;
            }

            var isSourceLegal = $(e.sourceNode).find(".data-id").data("legal");
            if (isSourceLegal && !isSumarry && !isDraggingBetween) {
                e.preventDefault();
                return;
            }

            var isSourceSummary = $(e.sourceNode).find(".data-id").data("summary");


            if (isSourceSummary && !isSumarry)  // summary can be child of summary only
            {
                e.preventDefault();
                return;
            }


            var treeid = $(e.sourceNode).find(".data-id").data("treeid");

            var currentNodeID = $(e.sourceNode).find(".data-id").val();


            var oldID = $(e.sourceNode).find(".data-id").data('treeparentid');  //parent nodeid of  dragging item

            if (oldID == 0 && isSourceSummary) {
                oldID = $(e.sourceNode).find(".data-id").val();
            }


            var elements = $.grep(lstRightTreeParentIds, function (ele, index) {
                return ele.id == currentNodeID;
            });

            if (elements != undefined && elements != null && elements.length > 0) {
                oldID = elements[0].pid;
            }

            var newID = 0;

            if (isDraggingBetween) {
                newID = $(e.destinationNode).find(".data-id").data('treeparentid');
                //if (newID == 0) {
                //    newID = $(e.destinationNode).find(".data-id").val();
                //}
            }
            else {
                newID = $(e.destinationNode).find(".data-id").val();
            }





            if (oldID == newID && !isDraggingBetween) {
                e.preventDefault();
                return;
            }


            if (elements != undefined && elements != null && elements.length > 0) {
                elements[0].pid = newID;
            }
            else {
                lstRightTreeParentIds.push({ 'id': currentNodeID, 'pid': newID });
            }


            $bsyDiv[0].style.visibility = "visible";

            var oldSiblings = $(e.sourceNode).siblings();       //siblings of dragging item

            var destinChildd = $(e.destinationNode);        //destination node 

            var destinChild = $(e.destinationNode).children('ul').children();// get children (li) of destination UL.


            //if (destinChild == null || destinChild == "" || destinChild.length == 0) { // if destination node has not any child yet, the we will allow defalt functaionlity of drag drop. 

            //    window.setTimeout(function () {     // after waitng when child has been created, we will update sequence  of source and destination
            //        destinChild = destinChildd.children('ul').children();
            //        updateSequenceforHierarchy(oldSiblings, oldID, destinChild, newID, treeid, isSumarry, currentNodeID, null, false);


            //    }, 200);

            //}
            //else {
            //if destination has children, then we will make sure that new item being droped ,will become first  item of destination. 
            //e.preventDefault(); //For this we prevent default functionality as it append item as last item.
            //$(e.destinationNode).children('ul').prepend(e.sourceNode);  //now append mannuly dragging item as first item.


            window.setTimeout(function () {     // after waitng when child has been created, we will update sequence  of source and destination
                if (isSumarry) {
                    destinChild = $(e.destinationNode).children('ul').children();
                }
                if (isDraggingBetween) {
                    destinChild = $(e.destinationNode).parent().children('li');
                }

                //if (oldSiblings.length == 1 && oldSiblings.find('.data-id').val() == newID) {
                oldSiblings = [];
                //}
                updateSequenceforHierarchy(oldSiblings, oldID, destinChild, newID, treeid, isSumarry, currentNodeID, null, false);//we will update sequence  of source and destination


            }, 200);
            //}
        };

        function addNodeToNewTree(oldTree, e) {
            var sourceItem = oldTree.dataItem(e.sourceNode).toJSON();
            var destinationNode = $(e.destinationNode);
            //var targetTree = destinationNode.closest("[data-role='treeview']").data("kendoTreeView");
            //targetTree.append(sourceItem, destinationNode);

            var node_ID = $(e.sourceNode).find(".data-id").val();
            var parentID = $(e.destinationNode).find(".data-id").val();
            var treeIDs = $(e.destinationNode).find(".data-id").data("treeid");

            var queries = [];
            var sql = '';

            var chidren = $($(e.sourceNode).find('.data-id')[0]);

            var parent_ID = 0;

            var childCount = 0;

            // var sql = "SELECT  max(seqNo) FROM AD_TreeNode WHERE AD_Client_ID=" + ctx.getAD_Client_ID() + " AND AD_Tree_ID=" + treeIDs;
            try {
                //var dr = VIS.DB.executeReader(sql, null, null);
                //if (dr.read()) {
                //    childCount = dr.getString(0);
                //}
                var sequenceNo = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "OrgStructure/LoadSequenceforTree", { TreeId: treeIDs });
                if (sequenceNo != null)
                    childCount = sequenceNo;

            }
            catch (ex) {

            }
            var chidrens = [];
            var isActive = [];
            for (var i = 0; i < chidren.length; i++) {

                //var isActive = $(chidren[i]).data("active") == true ? "Y" : "N";
                //sql = "INSERT INTO AD_TreeNode (AD_Client_ID, AD_Org_ID, AD_Tree_ID, Created, CreatedBy, IsActive, Node_ID, Parent_ID, Updated, Updatedby,seqNo) VALUES(" +
                //    ctx.getAD_Client_ID() + "," +
                //    ctx.getAD_Org_ID() + "," +
                //    treeIDs + "," +
                //    "sysdate," +
                //    ctx.getAD_User_ID() + "," +
                //    "'" + isActive + "'," +
                //    $(chidren[i]).val() + "," +
                //    parentID + "," +
                //    "sysdate," +
                //    ctx.getAD_User_ID() + "," + (parseInt(childCount) + 10) + ") ";
                //childCount = childCount + 10;
                //queries.push(sql);
                chidrens[i] = $(chidren[i]).val();
                isActive[i] = $(chidren[i]).data("active") == true ? "Y" : "N";
            }
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "OrgStructure/InsertTreeNode", { Chidrens: chidrens, IsActive: isActive, ParentID: parentID, TreeIds: treeIDs, ChildCount: VIS.Utility.Util.getValueOfInt(childCount) });
            //console.log(queries);
            //VIS.DB.executeQueries(queries);

        };

        function onSelect(e) {

            if (needSave) {
                if (VIS.ADialog.ask("VA003_SaveExisting")) {
                    var res = save();
                    if (!res) {
                        e.preventDefault();
                        return;
                    }
                }
                else {
                    undo();
                }
            }

            $lblOrgInfo.text(VIS.Msg.getMsg('VA003_OrgInfo'));
            var node = $(e.node).find(".data-id");

            var orgID = node.val();  //nodeid of  clicked item
            $bsyDiv[0].style.visibility = "visible";
            if (orgID > 0) {
                childNodeSelected();
                setEanbleDisableControls(false);
                loadOrgData(orgID, false);
                isHeaderNode = false;
                //clearControls();
                setMandatoryColor(false);
            }
            else {                                          // if top level node is selected, then it is not an org, so disable all controls
                parentNodeSelected();
                setEanbleDisableControls(true);
                isHeaderNode = true;
                clearControls();
                updateOldValue();
                setMandatoryColor(false);

                $bsyDiv[0].style.visibility = "hidden";
            }

            var orgParentID = node.data("orgparentid");
            if (orgParentID > 0) {
                $chkIsSummary.prop("disabled", true);
            }

            //if (orgParentID > 0) {
            //    $chkIsLegal.prop("disabled", true);

            //    isLegalDisable = true;
            //}
            //else {
            //    $chkIsLegal.prop("disabled", false);
            //    isLegalDisable = false;
            //}



            isSummarySelected = node.data("summary");
            if (!isSummarySelected || node.data("legal")) {                                // if non legel selected and it is not summary level , then legal cannot be inserted
                $btnNewLegalEntity.prop("disabled", true);
                $btnNewLegalEntity.css('opacity', '0.5');

            }
            if (!isSummarySelected && !node.data("legal")) {
                $btnaddNewOrg.prop("disabled", true);
                $btnaddNewOrg.css('opacity', '0.5');
            }
            if (!isSummarySelected) {
                $btnSummary.prop("disabled", true);
                $btnSummary.css('opacity', '0.5');
            }
            else {
                $btnSummary.prop("disabled", false);
                $btnSummary.css('opacity', '1');
            }

            if (isSummarySelected) {
                selecedSummayNode = node.val();
                seletedOrgID = 0;
            }
            else {
                selecedSummayNode = node.data("treeparentid");
                seletedOrgID = node.val();
            }
            if (selecedSummayNode == "undefined" || selecedSummayNode == null || selecedSummayNode == undefined) {
                selecedSummayNode = 0;
            }

            window.setTimeout(function () {

                //divLeftTree.find('.k-treeview').css({ 'border-color': 'transparent', 'background-color': 'transparent', '-webkit-box-shadow': 'none' });
                divLeftTree.find('.k-in').css({ 'cursor': 'pointer' });
                //divLeftTree.find('.k-state-hover').css({ 'border-color': 'transparent', 'background-color': 'transparent', '-webkit-box-shadow': 'none' });
                //divLeftTree.find('.k-state-focused').css({ 'border-color': 'transparent', 'background-color': 'transparent', '-webkit-box-shadow': 'none' });
                //divLeftTree.find('.k-state-selected').css({ 'border-color': 'rgba(229, 228, 225, 1)', 'background-color': '#DADADA', '-webkit-box-shadow': 'rgba(229, 228, 225, 1)', 'padding': '4px' });
                //divLeftTree.find('.k-state-selected').css({ 'border-color': 'rgba(229, 228, 225, 1)', 'background-color': '#DADADA', '-webkit-box-shadow': 'rgba(229, 228, 225, 1)', 'padding': '4px', 'box-shadow': 'transparent' });
                divLeftTree.find('.k-state-focused.k-state-selected').css({ 'box-shadow': 'none', '-webkit-box-shadow': 'none' });
            }, 100);
        };

        function parentNodeSelected() {

            $btnNewLegalEntity.prop("disabled", false);
            $btnNewLegalEntity.css('opacity', '1');

            $btnaddNewOrg.prop("disabled", false);
            $btnaddNewOrg.css('opacity', '1');

            $txtName.attr("disabled", true);
            $txtSerackKey.attr("disabled", true);
            //$txtName.css("background-color", '#F4F4F4');
            //$txtSerackKey.css("background-color", '#F4F4F4');

            $chkIsLegal.attr("disabled", true);
            $chkIsSummary.attr("disabled", true);
            $chkIsActive.attr("disabled", true);
            clearControls();
            hideImage();

        };

        function childNodeSelected() {

            $btnNewLegalEntity.prop("disabled", false);
            $btnNewLegalEntity.css('opacity', '1');

            $btnaddNewOrg.prop("disabled", false);
            $btnaddNewOrg.css('opacity', '1');

            $txtName.attr("disabled", false);
            $txtSerackKey.attr("disabled", false);
            $chkIsActive.attr("disabled", false);
            //$txtName.css("background-color", 'white');
            //$txtSerackKey.css("background-color", 'white');

            $chkIsLegal.attr("disabled", false);
            $chkIsSummary.attr("disabled", false);

        };

        function updateSequence(oldSiblings, oldID, destinChild, newID, treeid, isSumarry, currentNodeID, newIDForOrgInfo) {
            //var queries = [];
            //var sql = '';
            var OldSiblings = [];
            var NodeIDs = [];
            for (var i = 0; i < oldSiblings.length; i++) {
                //sql = "UPDATE ";
                //sql += tableName + " SET Parent_ID=" + oldID + ", SeqNo=" + i + ", Updated=SysDate" +
                //    " WHERE AD_Tree_ID=" + treeid + " AND Node_ID=" + $(oldSiblings[i]).find(".data-id").val();
                //queries.push(sql);
                OldSiblings[i] = VIS.Utility.Util.getValueOfInt($(oldSiblings[i]).find(".data-id").val());
            }
            for (var i = 0; i < destinChild.length; i++) {
                //sql = "UPDATE ";
                //sql += tableName + " SET Parent_ID=" + newID + ", SeqNo=" + i + ", Updated=SysDate" +
                //    " WHERE AD_Tree_ID=" + treeid + " AND Node_ID=" + $(destinChild[i]).find(".data-id").val();
                //queries.push(sql);
                NodeIDs[i] = $(destinChild[i]).find(".data-id").val();
            }
            var res = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "OrgStructure/UpdateSequence", { TableName: tableName, OldID: oldID, NewId: newID, TreeId: treeid, OldSibling: OldSiblings, NodId: NodeIDs });
            var parentID = newID;

            //if (isSumarry) {
            //    newIDForOrgInfo = 0;
            //}
            //else {
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "OrgStructure/UpdateParentNode", { TreeId: treeid, CurrentNode: currentNodeID, NewIdForOrg: newIDForOrgInfo, IsSummery: isSumarry });
                //sql = " update AD_TreeNode set parent_id=0 WHERE AD_Tree_ID= " + treeid + " AND  Node_ID=" + currentNodeID;
                //queries.push(sql);
            //}
            //VIS.dataContext.getJSONData(VIS.Application.contextUrl + "OrgStructure/UpdateParentOrgInfo", { NewID: newIDForOrgInfo, CurrentNode: currentNodeID });
            //sql = " update AD_OrgInfo set parent_org_id=" + newIDForOrgInfo + " WHERE AD_Org_ID=" + currentNodeID;
            //queries.push(sql);

            //console.log(queries);
            //VIS.DB.executeQueries(queries)
            $bsyDiv[0].style.visibility = "hidden";
        };

        function updateSequenceforHierarchy(oldSiblings, oldID, destinChild, newID, treeid, isSumarry, currentNodeID) {
            var oldSiblings = new Array();
            var Nodeids = new Array();
            for (var i = 0; i < oldSiblings.length; i++) {
                //sql = "UPDATE ";
                //sql += tableName + " SET Parent_ID=" + oldID + ", SeqNo=" + i + ", Updated=SysDate" +
                //    " WHERE AD_Tree_ID=" + treeid + " AND Node_ID=" + $(oldSiblings[i]).find(".data-id").val();
                //queries.push(sql);
                oldSiblings[i] = VIS.Utility.Util.getValueOfInt($(oldSiblings[i]).find(".data-id").val());
            }

            for (var i = 0; i < destinChild.length; i++) {
                //sql = "UPDATE ";
                //sql += tableName + " SET Parent_ID=" + newID + ", SeqNo=" + i + ", Updated=SysDate" +
                //    " WHERE AD_Tree_ID=" + treeid + " AND Node_ID=" + $(destinChild[i]).find(".data-id").val();
                //queries.push(sql);
                Nodeids[i] = $(destinChild[i]).find(".data-id").val();
            }
            var res = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "OrgStructure/UpdateSequence", { TableName: tableName, OldID: oldID, NewId: newID, TreeId: treeid, OldSibling: oldSiblings, NodId: Nodeids });

            var parentID = newID;

            //if (!isSumarry) {
            //    sql = " update AD_TreeNode set parent_id=0 WHERE AD_Tree_ID= " + treeid + " AND  Node_ID=" + currentNodeID;
            //    queries.push(sql);
            //}

            console.log(Nodeids);
            //VIS.DB.executeQueries(queries);
            $bsyDiv[0].style.visibility = "hidden";
        };

        function loadLookups() {
            $.ajax({
                url: VIS.Application.contextUrl + "OrgStructure/LoadLookups",
                success: function (result) {
                    var data = JSON.parse(result);
                    if (data && allLookupLoaded == false) {
                        fillCombos(data.AllOrgType, data.AllReportHierarchy, data);
                        allLookupLoaded = true;
                    }
                }
            });

        };

        function loadOrgData(orgID, loadLookups) {
            $.ajax({
                url: VIS.Application.contextUrl + "OrgStructure/GetOrgInfo",
                async: true,
                data: { orgID: orgID, loadLookups: loadLookups },
                success: function (result) {
                    var data = JSON.parse(result);


                    ad_Org_ID = data.OrgID;
                    seletedOrgID = ad_Org_ID;

                    setOrgDataIntoFields(data, true);
                    $bsyDiv[0].style.visibility = "hidden";
                },
                error: function (eror) {
                    VIS.ADialog.error("VA003_ErrorgettingOrg" + eror);
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });
        }

        function fillCombos(AllOrgType, AllReportHierarchy, data) {
            //if (AllTenent) {
            //    for (var i = 0; i < AllTenent.length; i++) {
            //        $cmbTenant.append('<option value=' + AllTenent[i].Key + '>' + AllTenent[i].Name + '</option>');
            //    }
            //}

            if (AllOrgType) {
                for (var i = 0; i < AllOrgType.length; i++) {
                    $cmbOrgType.append('<option value=' + AllOrgType[i].Key + '>' + AllOrgType[i].Name + '</option>');
                }
            }

            fillCmbHierarchy(AllReportHierarchy);

            $txtName.attr("maxlength", data.NameLength);
            $txtSerackKey.attr("maxlength", data.ValueLength);
            $txtEmail.attr("maxlength", data.EMailLength);
            $txtFax.attr("maxlength", data.FaxLength);
            $txtPhone.attr("maxlength", data.PhoneLength);
            $txtTax.attr("maxlength", data.TaxIDLength);

            nameLength = data.NameLength;
            valueLength = data.ValueLength;
            treeLength = data.TreeNameLength;
            //if (AllParentOrg) {
            //    for (var i = 0; i < AllParentOrg.length; i++) {
            //        $cmbParentaOrg.append('<option value=' + AllParentOrg[i].Key + '>' + AllParentOrg[i].Name + '</option>');
            //    }
            //}

            //if (AllWarehouse) {
            //    for (var i = 0; i < AllWarehouse.length; i++) {
            //        $cmbWarehouse.append('<option value=' + AllWarehouse[i].Key + '>' + AllWarehouse[i].Name + '</option>');
            //    }
            //}
        };

        function setOrgDataIntoFields(data, updateOldValues) {
            $chkIsSummary.attr("disabled", false);
            if (!isLegalDisable) {
                $chkIsLegal.attr("disabled", false);
            }
            if (data == null || data == undefined) {
                $txtSerackKey.val("");
                $txtName.val("");
                $txtTax.val("");
                $txtEmail.val("");
                $txtPhone.val("");
                $txtFax.val("");
                $txtLocation.setValue(null);
                $chkIsLegal.prop("checked", false);
                $cmbOrgType.val(-1);
                $chkIsSummary.prop("checked", false);
                setEanbleDisableControls(false);
                hideImage();
                setMandatoryColor(true);

            }
            else {
                // $cmbTenant.val(data.Tenant);
                $txtSerackKey.val((data.SearchKey));
                $txtName.val((data.Name));
                //  $txtDesc.val(data.Description);
                $cmbOrgType.val(data.OrgType);
                // $cmbParentaOrg.val(data.ParentOrg);
                $txtTax.val((data.TaxID));
                $txtEmail.val((data.EmailAddess));
                $txtPhone.val((data.Phone));
                $txtFax.val((data.Fax));
                // $cmbWarehouse.val(data.Warehouse);
                if (data.C_Location_ID == 0) {
                    $txtLocation.setValue(null);
                }
                else {
                    $txtLocation.setValue(data.C_Location_ID);
                }
                // $txtOrgSuperviser.setValue(data.OrgSupervisor);



                if (data.IsLegalEntity != null && data.IsLegalEntity != "") {
                    $chkIsLegal.prop("checked", data.IsLegalEntity);
                    setEanbleDisableControls(false);
                }
                else {
                    $chkIsLegal.prop("checked", false);
                    setEanbleDisableControls(false);
                }


                if (data.IsSummary != null && data.IsSummary != "") {
                    $chkIsSummary.prop("checked", data.IsSummary);
                    setEanbleDisableControls(true);
                }
                else {
                    $chkIsSummary.prop("checked", false);
                    setEanbleDisableControls(false);
                }

                if (data.ParentOrg > 0) {
                    $chkIsLegal.prop("disabled", true);
                    $chkIsSummary.prop("disabled", true);
                    isLegalDisable = true;
                }
                else {
                    $chkIsLegal.prop("disabled", false);
                    $chkIsSummary.prop("disabled", false);
                    isLegalDisable = false;
                }
                if (data.IsActive) {
                    $chkIsActive.prop("checked", data.IsActive);
                }
                else {
                    $chkIsActive.prop("checked", false);
                }

                //set cost center and profit center
                if (data.profitCenter != undefined && data.profitCenter != null && data.profitCenter != "") {
                    $chkIsProfitCenter.prop("checked", data.profitCenter);
                }
                else {
                    $chkIsProfitCenter.prop("checked", false);
                }
                if (data.costCenter != undefined && data.costCenter != null && data.costCenter != "") {
                    $chkIsCostCenter.prop("checked", data.costCenter);
                }
                else {
                    $chkIsCostCenter.prop("checked", false);
                }

                if (data.OrgImage != null && data.OrgImage != undefined && data.OrgImage.length > 0) {
                    $imageControl.show();
                    $($imageControl.parent()).css("background-color", "white");
                    $imageControl.attr('src', VIS.Application.contextUrl + data.OrgImage);
                }
                else {
                    hideImage()
                }
                if (updateOldValues) {
                    updateOldValue(data);
                }
                setMandatoryColor(false);
            }
        };

        function hideImage() {
            $($imageControl.parent()).css("background-color", "rgba(var(--v-c-secondary), .4)");
            $imageControl.attr('src', null);
            $imageControl.hide();
        };

        function updateOldValue(data) {
            if (data) {
                oldValues = {
                    "Tenant": data.Tenant, "SearchKey": data.SearchKey, "Name": data.Name, "TaxID": data.TaxID,
                    "EmailAddess": data.EmailAddess, "Phone": data.Phone, "Fax": data.Fax,
                    "C_Location_ID": data.C_Location_ID, "IsSummary": data.IsSummary, "IsLegalEntity": data.IsLegalEntity, "OrgImage": data.OrgImage, 'IsActive': data.IsActive,
                    "costCenter": data.costCenter, "profitCenter": data.profitCenter
                };
            }
            else {
                oldValues = {
                    "Tenant": "", "SearchKey": "", "Name": "", "TaxID": "",
                    "EmailAddess": "", "Phone": "", "Fax": "",
                    "C_Location_ID": "", "IsSummary": "", "IsLegalEntity": "", "OrgImage": "", 'IsActive': "", "costCenter": "", "profitCenter": ""
                };
            }
        };

        function newLegalEntity(e) {
            var ret = checkNeedSave();
            if (ret) {
                return;
            }
            setStatus(false);
            enableNameValue();
            addEffect($btnNewLegalEntity);
            clearControls();
            setMandatoryColor(true);
            changeorgLabelText(true);
            setEanbleDisableControls(false);

            $chkIsSummary.attr("disabled", true);
            $chkIsLegal.attr("disabled", true);
            $chkIsSummary.prop("checked", false);
            $chkIsLegal.prop("checked", true);

            $chkIsCostCenter.prop("checked", false);
            $chkIsProfitCenter.prop("checked", false);
            $lblCostCenter.hide();
            $lblProfitCenter.hide();

        };

        function newSummary(e) {
            var ret = checkNeedSave();
            if (ret) {
                return;
            }
            setStatus(false);
            addEffect($btnSummary);
            clearControls();
            changeorgLabelText(true);
            enableNameValue();
            setEanbleDisableControls(true);
            setMandatoryColor(true);
            $chkIsSummary.attr("disabled", true);
            $chkIsLegal.attr("disabled", true);
            $chkIsSummary.prop("checked", true);
            $chkIsLegal.prop("checked", false);

            $chkIsCostCenter.prop("checked", false);
            $chkIsProfitCenter.prop("checked", false);
            $lblCostCenter.hide();
            $lblProfitCenter.hide();
        };

        function checkNeedSave() {
            if (needSave) {
                if (VIS.ADialog.ask("VA003_SaveExisting")) {
                    save();
                }
                else {
                    undo();
                }
                needSave = false;
                return true;
            }
            return false;
        };

        function addNewOrg(e) {
            var ret = checkNeedSave();
            if (ret) {
                return;
            }
            setStatus(false);
            enableNameValue();
            addEffect($btnaddNewOrg);
            clearControls();
            setMandatoryColor(true);
            changeorgLabelText(true);
            $chkIsSummary.attr("disabled", true);
            $chkIsLegal.attr("disabled", true);
            $chkIsSummary.prop("checked", false);
            $chkIsLegal.prop("checked", false);
            setEanbleDisableControls(false);

            $chkIsCostCenter.prop("checked", false);
            $chkIsProfitCenter.prop("checked", false);
            $lblCostCenter.show();
            $lblProfitCenter.show();
        };

        function enableNameValue() {
            $txtName.attr("disabled", false);
            $txtSerackKey.attr("disabled", false);
            //$txtName.css("background-color", "white");
            //$txtSerackKey.css("background-color", "white");
        }

        function addEffect(btn) {
            var options = { to: $divFormWrap, className: "VA003-ui-effects-transfer" };
            btn.effect("transfer", options, 600, function () { });
            $imgUpload.val("");
        };

        function clearControls() {
            ad_Org_ID = 0;
            //   $cmbTenant.val(ctx.getAD_Client_ID());
            $txtSerackKey.val("");
            $txtName.val("");

            // $txtDesc.val("");
            $cmbOrgType.val(-1);
            // $cmbParentaOrg.val(-1);
            // $cmbOrgType.val("");
            $txtEmail.val("");
            $txtPhone.val("");
            $txtFax.val("");
            $txtTax.val("");
            //  $cmbWarehouse.val(-1);
            $txtLocation.setValue(null);

            $chkIsLegal.attr("checked", false);
            $chkIsSummary.attr("checked", false);
            $chkIsActive.prop("checked", true);
            //$imageControl.attr('src', VIS.Application.contextUrl + 'Areas/VIS/Images/login/logo.PNG');
            hideImage();

            // $txtOrgSuperviser.setValue(-1);
        };

        function setMandatoryColor(flag) {
            if (flag) {
                //$txtName.css("background-color", "#ffb6c1");
                $txtName.addClass('vis-ev-col-mandatory');
                //   $txtSerackKey.css("background-color", '#ffb6c1');
            }
            else {
                if (isHeaderNode) {
                    //  $txtSerackKey.css("background-color", '#F4F4F4');
                }
                else {
                    //$txtName.css("background-color", "white");
                    $txtName.removeClass('vis-ev-col-mandatory');
                    // $txtSerackKey.css("background-color", 'white');
                }
            }
        };

        function setEanbleDisableControls(disabled) {
            $cmbOrgType.attr("disabled", disabled);
            //  $cmbParentaOrg.attr("disabled", disabled);
            //  $cmbOrgType.attr("disabled", disabled);
            //   $cmbWarehouse.attr("disabled", disabled);

            $txtEmail.attr("disabled", disabled);
            $txtPhone.attr("disabled", disabled);
            $txtFax.attr("disabled", disabled);
            $txtTax.attr("disabled", disabled);
            $txtLocation.getControl().attr("disabled", disabled);
            $txtLocation.getBtn(0).attr("disabled", disabled);
            $txtLocation.getBtn(1).attr("disabled", disabled);

            //$chkIsSummary.attr("disabled", disabled);
            //$chkIsLegal.attr("disabled", disabled);
            //hide  profit center and cost center
            if ($chkIsLegal.is(':checked') || $chkIsSummary.is(':checked')) {
                $chkIsCostCenter.attr('hidden', true);
                $chkIsProfitCenter.attr('hidden', true);
                $lblCostCenter.attr('hidden', true);
                $lblProfitCenter.attr('hidden', true);
                $chkIsCostCenter.prop('checked', false);
                $chkIsProfitCenter.prop('checked', false);
            }
            else {
                $chkIsCostCenter.attr('hidden', false);
                $chkIsProfitCenter.attr('hidden', false);
                $lblCostCenter.attr('hidden', false);
                $lblProfitCenter.attr('hidden', false);
            }

            var bgColor = "white";

            if (disabled) {
                bgColor = "#F4F4F4";
            }
            //$txtEmail.css("background-color", bgColor);
            //$txtPhone.css("background-color", bgColor);
            //$txtFax.css("background-color", bgColor);
            //$txtTax.css("background-color", bgColor);
            //$cmbOrgType.css("background-color", bgColor);
            //$txtLocation.getControl().css("background", bgColor);
            //$txtLocation.getBtn(0).css("background", bgColor);
            //$txtLocation.getBtn(1).css("background", bgColor);



            //  $txtOrgSuperviser.getControl().attr("disabled", disabled);
            // $txtOrgSuperviser.getBtn(0).attr("disabled", disabled);
        };

        function valueChangeEvent(ctrl) {
            if (ctrl.is(':text')) {
                ctrl.on("keydown", function (e) {
                    needSave = true;
                    setStatus(false);
                });
            }
            else {
                ctrl.on("change", function (e) {
                    needSave = true;
                    setStatus(false);
                });
            }
        };

        function save(e) {

            if ($btnSave.css("opacity") == "0.5") {
                return;
            }

            if ($txtName.val().trim().length == 0) {
                VIS.ADialog.info("EnterName");
                return;
            }

            //if ($txtSerackKey.val().trim().length == 0) {
            //    VIS.ADialog.info("VA003_EnterValue");
            //    return;
            //}

            needSave = false;

            if (selecedSummayNode == ad_Org_ID) {
                selecedSummayNode = 0;
            }



            // var sql = "SELECT Parent_Org_ID FROM AD_OrgInfo WHERE AD_Org_ID=";




            $bsyDiv[0].style.visibility = "visible";
            window.setTimeout(function () {

                var params = {
                    OrgID: ad_Org_ID, Tenant: ctx.getAD_Client_ID(), SearchKey: VIS.Utility.encodeText($txtSerackKey.val().trim()), Name: VIS.Utility.encodeText($txtName.val().trim()), C_Location_ID: $txtLocation.getValue(),
                    TaxID: VIS.Utility.encodeText($txtTax.val().trim()), EmailAddess: VIS.Utility.encodeText($txtEmail.val().trim()), Phone: VIS.Utility.encodeText($txtPhone.val().trim()), Fax: VIS.Utility.encodeText($txtFax.val().trim()),
                    OrgType: $cmbOrgType.val(), IsActive: $chkIsActive.is(':checked'),
                    IsSummary: $chkIsSummary.is(':checked'), IsLegalEntity: $chkIsLegal.is(':checked'), ParentIDForSummary: selecedSummayNode, AD_Tree_ID: AD_Tree_ID, showOrgUnit: $showOrUnits.is(':checked'), costCenter: $chkIsCostCenter.is(':checked'), profitCenter: $chkIsProfitCenter.is(':checked')
                };



                if (ad_Org_ID == 0 && oldValues != null && !oldValues.IsSummary) {
                    params["ParentOrg"] = seletedOrgID;
                }

                $.ajax({
                    url: VIS.Application.contextUrl + "OrgStructure/InsertOrUpdateOrg",
                    async: true,
                    data: params,
                    success: function (result) {
                        if (result) {

                            if (JSON.parse(result) == "VIS_OrgExist") {
                                VIS.ADialog.warn("VA003_OrgExist");
                                $bsyDiv[0].style.visibility = "hidden";
                                return;
                            }
                            else if (JSON.parse(result).indexOf("Error") > -1) {
                                VIS.ADialog.warn(JSON.parse(result));
                                $bsyDiv[0].style.visibility = "hidden";
                                return;
                            }
                            var isActve = $chkIsActive.is(':checked');
                            if (ad_Org_ID == 0) {
                                //
                                //var div = $rightdivContainer.find('#' + $self.windowNo + 'orgrighttree');
                                ad_Org_ID = parseInt(JSON.parse(result));
                                var objNewNode = {};
                                objNewNode["color"] = "white";
                                objNewNode["NodeID"] = ad_Org_ID;
                                objNewNode["text"] = VIS.Utility.encodeText($txtName.val());
                                objNewNode["IsSummary"] = false;

                                var isSummary = $chkIsSummary.is(':checked');
                                var isleg = $chkIsLegal.is(':checked');
                                var bgColor = '';
                                var imgSource = '';


                                var selectedNode = divLeftTree.data("kendoTreeView").select();

                                if ($chkIsLegal.is(':checked')) {
                                    if (isActve) {
                                        bgColor = "#dc8a20";
                                    }
                                    else {
                                        bgColor = "#F4C993";
                                    }
                                    imgSource = "Areas/VA003/Images/orgstr-legal-entity.PNG";
                                }
                                else if ($chkIsSummary.is(':checked')) {
                                    if (isActve) {
                                        bgColor = "#0084c4";
                                    }
                                    else {
                                        bgColor = "rgba(0, 132, 196, 0.7)";
                                    }

                                    imgSource = "Areas/VA003/Images/orgstr-org.png";
                                }
                                else {
                                    if (isActve) {
                                        bgColor = "rgba(43, 174, 250, 0.78)";
                                    }
                                    else {
                                        bgColor = "rgba(166, 222, 255, 1)";
                                    }
                                    imgSource = "Areas/VA003/Images/orgstr-store.PNG";
                                }


                                if (!isSummary || isleg) {                                // if non legel selected and it is not summary level , then legal cannot be inserted
                                    $btnNewLegalEntity.prop("disabled", true);
                                    $btnNewLegalEntity.css('opacity', '0.5');

                                }
                                if (!isSummary) {
                                    $btnSummary.prop("disabled", true);
                                    $btnSummary.css('opacity', '0.5');
                                }
                                else {
                                    $btnSummary.prop("disabled", false);
                                    $btnSummary.css('opacity', '1');
                                }

                                if (isSummary) {
                                    selecedSummayNode = ad_Org_ID;
                                    seletedOrgID = 0;
                                }
                                else {
                                    selecedSummayNode = selecedSummayNode;
                                    seletedOrgID = ad_Org_ID;
                                }


                                var newChild = divLeftTree.data("kendoTreeView").append({
                                    text: VIS.Utility.encodeText($txtName.val()),

                                    'bColor': bgColor,
                                    'color': "white", 'NodeID': ad_Org_ID,
                                    'IsSummary': isSummary,
                                    'ImageSource': imgSource,
                                    'ParentID': seletedOrgID,//,
                                    'TreeParentID': selecedSummayNode,//,
                                    'OrgParentID': parseInt(seletedOrgID),
                                    'IsLegal': isleg,
                                    'IsActive': isActve
                                }, selectedNode);

                                divLeftTree.data("kendoTreeView").select(newChild);

                                divLeftTree.data("kendoTreeView").trigger('select', { node: newChild });

                                changeOrgPic();
                                $chkIsProfitCenter.prop("checked", false);
                                $chkIsCostCenter.prop("checked", false);
                                VIS.ADialog.info('Saved');
                                $bsyDiv[0].style.visibility = "hidden";
                            }
                            else if (!refreshTree) {
                                // commented bcz when we save record, system  not showing value as true, if having on database
                                //$chkIsProfitCenter.prop("checked", false);
                                //$chkIsCostCenter.prop("checked", false);
                                VIS.ADialog.info('Saved');

                                if ($chkIsLegal.is(':checked')) {
                                    if (isActve) {

                                        $(divLeftTree.data("kendoTreeView").select().find('p')[0]).css('background-color', "#dc8a20");
                                    }
                                    else {
                                        $(divLeftTree.data("kendoTreeView").select().find('p')[0]).css('background-color', "#F4C993");
                                    }
                                }
                                else if ($chkIsSummary.is(':checked')) {
                                    if (isActve) {
                                        $(divLeftTree.data("kendoTreeView").select().find('p')[0]).css('background-color', "#0084c4");
                                    }
                                    else {
                                        $(divLeftTree.data("kendoTreeView").select().find('p')[0]).css('background-color', "rgba(0, 132, 196, 0.7)");
                                    }
                                }
                                else {
                                    if (isActve) {
                                        $(divLeftTree.data("kendoTreeView").select().find('p')[0]).css('background-color', "rgba(43, 174, 250, 0.78)");
                                    }
                                    else {
                                        $(divLeftTree.data("kendoTreeView").select().find('p')[0]).css('background-color', "rgba(166, 222, 255, 1)");
                                    }
                                }
                                if (isActve) {
                                    $($(divLeftTree.data("kendoTreeView").select()).find('.data-id')[0]).data('active', true);
                                }
                                else {
                                    $($(divLeftTree.data("kendoTreeView").select()).find('.data-id')[0]).data('active', false);
                                }
                                $bsyDiv[0].style.visibility = "hidden";
                            }
                            else {
                                createLeftTree(result);
                                clearControls();
                                setEanbleDisableControls(true);
                                $bsyDiv[0].style.visibility = "hidden";
                            }
                            refreshTree = false;
                            updateOldValue(params);

                            setStatus(true);

                        }
                        changeorgLabelText(false);
                    },
                    error: function (eror) {
                        VIS.ADialog.error("VA003_ErrorInSaving" + eror);
                        changeorgLabelText(false);
                    }
                });
            }, 200);

        };


        function undo(e) {

            if ($btnUndo.css("opacity") == "0.5") {
                return;
            }
            needSave = false;
            //canDrop = true;


            setOrgDataIntoFields(oldValues, false);
            setStatus(true);
            changeorgLabelText(false);
            setMandatoryColor(false);
        };

        function setStatus(disable) {
            // canDrop = disable;
            //needSave = disable;
            $btnUndo.prop('disabled', disable);

            $btnSave.prop('disabled', disable);
            if (disable) {
                $btnUndo.css('opacity', '.5');
                $btnSave.css('opacity', '.5');
            }
            else {
                $btnUndo.css('opacity', '1');
                $btnSave.css('opacity', '1');
            }
        };

        function changeOrgPic(e) {
            if (ad_Org_ID > 0) {
                var file = $imgUpload[0].files[0];



                if (file != null && file != undefined) {


                    if (file.size > (50 * 1024)) {
                        $imgUpload.val("");
                        VIS.ADialog.info('VA003_picSize');
                        return;
                    }



                    $bsyDiv[0].style.visibility = "visible";


                    var xhr = new XMLHttpRequest();
                    var fd = new FormData();
                    fd.append("pic", file);
                    fd.append("orgID", ad_Org_ID);
                    xhr.open("POST", VIS.Application.contextUrl + "OrgStructure/UploadPic");
                    xhr.send(fd);
                    xhr.addEventListener("load", function (eve) {
                        var dd = eve.target.response;
                        var res = JSON.parse(dd);
                        var a = JSON.parse(res);
                        $imageControl.show();
                        $($imageControl.parent()).css("background-color", "white");
                        $imageControl.attr('src', VIS.Application.contextUrl + a);
                        $bsyDiv[0].style.visibility = "hidden";
                    });
                }
                $imgUpload.val("");
            }
            else {

                var file = $imgUpload[0].files[0];
                if (file) {

                    if (file.size > (50 * 1024)) {
                        $imgUpload.val("");
                        VIS.ADialog.info('VA003_picSize');
                        return;
                    }

                    var reader = new FileReader();
                    reader.onloadend = function () {
                        $imageControl.show();
                        $($imageControl.parent()).css("background-color", "white");
                        $imageControl.attr('src', reader.result);
                        //$imageControl.src = reader.result;
                    }
                    reader.readAsDataURL(file);

                }

            }
        };

        function chklegalEntity(e) {

            if (ad_Org_ID > 0) {
                if ($chkIsSummary.is(':checked')) {
                    $chkIsSummary.prop('checked', false);
                }
                refreshTree = true;
            }

            setEanbleDisableControls($chkIsLegal.is(':checked'));
        };

        function chkSummary(e) {

            if (ad_Org_ID > 0) {
                if ($chkIsLegal.is(':checked')) {
                    if (!VIS.ADialog.ask("VA003_changeToSummary")) {
                        e.preventDefault();

                        return;
                    }
                    $chkIsLegal.prop('checked', false);

                    refreshTree = true;
                }
                else if (!isSummarySelected && $chkIsSummary.is(':checked')) {
                    if (!VIS.ADialog.ask("VA003_changeToSummary")) {
                        e.preventDefault();
                        return;
                    }
                    $chkIsLegal.attr('checked', false);
                    refreshTree = true;
                }
                if ($chkIsSummary.is(':checked')) {
                    $chkIsLegal.attr('disabled', true);
                }
                else {
                    $chkIsLegal.attr('disabled', false);
                }
            }

            setEanbleDisableControls($chkIsSummary.is(':checked'));
        };

        function generateReport(e) {

            $bsyDiv[0].style.visibility = "visible";
            window.setTimeout(function () {
                $.ajax({
                    url: VIS.Application.contextUrl + "OrgStructure/GenerateHeirarchy",
                    async: true,
                    data: { windowNo: $self.windowNo },
                    success: function (result) {
                        var data = JSON.parse(result);
                        if (data != null) {

                            if (data == 'ProcessFailed') {
                                VIS.ADialog.error('ProcessFailed');
                                $bsyDiv[0].style.visibility = "hidden";
                                return;
                            }
                            else if (data == 'ProcessNoInstance') {
                                VIS.ADialog.error('ProcessNoInstance');
                                $bsyDiv[0].style.visibility = "hidden";
                                return;
                            }
                            else if (jQuery.type(data) == "string" && data.indexOf("Error") > -1) {
                                VIS.ADialog.error(data);
                                $bsyDiv[0].style.visibility = "hidden";
                                return;
                            }

                            var AllReportHierarchy = data.AllReportHierarchy;

                            fillCmbHierarchy(AllReportHierarchy);


                            var treeData = data.Tree;


                            updateHeirarchyTree(treeData);

                        }
                    },
                    error: function (e) {
                        console.log(e);
                        VIS.ADialog.error("VA003_ControllerNotFound");
                        $bsyDiv[0].style.visibility = "hidden";
                    }
                });
            }, 200);


        };

        function hierarchyChange(e) {
            lstRightTreeParentIds = [];
            var treeID = $cmbReportHirerchy.val();
            if (treeID < 0) {
                //var div = $rightdivContainer.find('#' + $self.windowNo + 'orgrighttree');
                //if (div == undefined || div == null || div.length == 0) {
                //}
                //else {
                if ($divRightTree.data("kendoTreeView") != undefined) {
                    $divRightTree.data("kendoTreeView").destroy();
                    $divRightTree.empty();
                }
                //}

                return;
            }

            if ($cmbReportHirerchy.find('option:selected').data('isdefault') == "Y") {
                $btnAddNode.css('opacity', '0.5');
            }
            else {
                $btnAddNode.css('opacity', '1');
            }

            //var doDrag = ($cmbReportHirerchy.find('option:selected').data('isdefault') == "Y") ? false : true;
            $bsyDiv[0].style.visibility = "visible";
            $.ajax({
                url: VIS.Application.contextUrl + "OrgStructure/CreateTree",
                //data: { windowNo: $self.windowNo, treeID: treeID, getOrgInfoTree: doDrag },
                data: { windowNo: $self.windowNo, treeID: treeID },
                success: function (result) {
                    treeData = JSON.parse(result);
                    updateHeirarchyTree(treeData);
                    if ($cmbReportHirerchy.find('option:selected').data('isdefault') == "N") {
                        $divRightTree.data("kendoTreeView").expand(".k-item");
                    }
                },
                error: function (e) {
                    VIS.ADialog.error("VA003_ControllerNotFound");
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });
        };

        function updateHeirarchyTree(treeData) {
            //var div = $rightdivContainer.find('#' + $self.windowNo + 'orgrighttree');
            //if (div == undefined || div == null || div.length == 0) {
            //    div = $('<div style="float:left;width:100%" id="' + $self.windowNo + 'orgrighttree">');
            //    $rightdivContainer.append(div);
            //}
            //else {
            //$('#' + $self.windowNo + 'orgrighttree').data("kendoTreeView").dataSource.data([]);
            //$('#' + $self.windowNo + 'orgrighttree').data("kendoTreeView").dataSource.data(treeData);
            //if ($('#' + $self.windowNo + 'orgrighttree').data("kendoTreeView") != undefined) {
            //    $('#' + $self.windowNo + 'orgrighttree').data("kendoTreeView").destroy();
            //    div.empty();
            //}
            //}

            //tableName = treeData[0].TableName;
            //AD_Tree_ID = treeData[0].AD_Tree_ID;

            if ($divRightTree.data("kendoTreeView") != undefined) {
                $divRightTree.data("kendoTreeView").destroy();
                $divRightTree.empty();
            }
            var doDrag = ($cmbReportHirerchy.find('option:selected').data('isdefault') == "Y") ? false : true;

            var trreeid = treeData[0].AD_Tree_ID;
            whereclause = treeData[0].WhereClause;
            if (doDrag) {
                $divRightTree.kendoTreeView({
                    dataSource: treeData,
                    dragAndDrop: doDrag,
                    drop: onHierarchyDrop,
                    select: onSelectRht,
                    template: "<div style='float: left;border-radius: 4px;display: inline-flex; align-items: center; background: #= item.bColor #;'><img src='" + VIS.Application.contextUrl + "#= item.ImageSource #' style='vertical-align: text-top;float: left;margin: 0px 0px 0px 10px; '><i class='#= item.ImageSource #' style='float: left;'></i>" +
                        "<p style='float: left;min-width:122px;border-radius:4px;margin:0px;padding: 7px 10px 7px 10px;  color: #= item.color #'>#= item.text #</p>" +
                        "<input type='hidden' class='data-id' value='#= item.NodeID #'  data-legal='#= item.IsLegal #'  data-summary='#= item.IsSummary #'  data-parentid='#= item.ParentID #'  data-Treeparentid='#= item.TreeParentID #'   data-treeid='" + trreeid + "' />" +
                        "</div><div style='float:left; display: flex; align-items: center;'><a style='display:#= item.DeleteVisibility #' title='" + VIS.Msg.getMsg("Delete") + "' class='VA003-delete-link vis vis-mark' ></a><a style='display:#= item.Visibility #' title='" + VIS.Msg.getMsg("VA003_Rename") + "'  class='VA003-rename-link vis vis-pencil' ></a></div>",
                });
            }
            else {
                $divRightTree.kendoTreeView({
                    dataSource: treeData,
                    dragAndDrop: doDrag,
                    drop: onHierarchyDrop,
                    select: onSelectRht,
                    template: "<div style='float: left;border-radius: 4px;float: left;border-radius: 4px;display: inline-flex; align-items: center;background: #= item.bColor #;'><img src='" + VIS.Application.contextUrl + "#= item.ImageSource #' style='vertical-align: text-top;float: left;margin: 0px 0px 0px 10px; '><i class='#= item.ImageSource #' style='float: left;'></i>" +
                        "<p style='float: left;min-width:122px;border-radius:4px;margin:0px;padding: 7px 10px 7px 10px;  color: #= item.color #'>#= item.text #</p>" +
                        "<input type='hidden' class='data-id' value='#= item.NodeID #'  data-legal='#= item.IsLegal #'  data-summary='#= item.IsSummary #'  data-parentid='#= item.ParentID #'  data-Treeparentid='#= item.TreeParentID #'   data-treeid='" + trreeid + "' />" +
                        "</div><div style='float:left;display:#= item.Visibility #'><a title='" + VIS.Msg.getMsg("VA003_Rename") + "'  class='VA003-rename-link vis vis-pencil' ></a></div>",
                });
            }


            if ($divRightTree) {
                var topheight = $($rightdivContainer.find('.VA003-right-top')).outerHeight();
                var comHeight = $($rightdivContainer.find('.VA003-form-data')).outerHeight();
                $divRightTree.height($rightdivContainer.outerHeight() - (35 + 45 + 40));

            }


            window.setTimeout(function () {

                //$divRightTree.find('.k-treeview').css({ 'border-color': 'transparent', 'background-color': 'transparent', '-webkit-box-shadow': 'none' });
                $divRightTree.find('.k-in').css({ 'cursor': 'pointer' });
                //$divRightTree.find('.k-state-hover').css({ 'border-color': 'transparent', 'background-color': 'transparent', '-webkit-box-shadow': 'none' });
                //$divRightTree.find('.k-state-focused').css({ 'border-color': 'transparent', 'background-color': 'transparent', '-webkit-box-shadow': 'none' });
                //$divRightTree.find('.k-state-selected').css({ 'border-color': 'rgba(229, 228, 225, 1)', 'background-color': '#DADADA', '-webkit-box-shadow': 'rgba(229, 228, 225, 1)', 'padding': '4px' });
                //$divRightTree.find('.k-state-selected').css({ 'border-color': 'rgba(229, 228, 225, 1)', 'background-color': '#DADADA', '-webkit-box-shadow': 'rgba(229, 228, 225, 1)', 'padding': '4px', 'box-shadow': 'transparent' });
                //$divRightTree.find('.k-state-focused.k-state-selected').css({ 'box-shadow': 'none', '-webkit-box-shadow': 'none' });
            }, 100);

            $rightdivContainer.off(VIS.Events.onTouchStartOrClick, ".VA003-delete-link");
            $rightdivContainer.on(VIS.Events.onTouchStartOrClick, ".VA003-delete-link", deleteNode);

            $rightdivContainer.off(VIS.Events.onTouchStartOrClick, ".VA003-rename-link");
            $rightdivContainer.on(VIS.Events.onTouchStartOrClick, ".VA003-rename-link", renameNode);

            $bsyDiv[0].style.visibility = "hidden";
        };

        function onSelectRht(e) {
            window.setTimeout(function () {
                //$divRightTree.find('.k-treeview').css({ 'border-color': 'transparent', 'background-color': 'transparent', '-webkit-box-shadow': 'none' });
                $divRightTree.find('.k-in').css({ 'cursor': 'pointer' });
                //$divRightTree.find('.k-state-hover').css({ 'border-color': 'transparent', 'background-color': 'transparent', '-webkit-box-shadow': 'none' });
                //$divRightTree.find('.k-state-focused').css({ 'border-color': 'transparent', 'background-color': 'transparent', '-webkit-box-shadow': 'none' });
                //$divRightTree.find('.k-state-selected').css({ 'border-color': 'rgba(229, 228, 225, 1)', 'background-color': '#DADADA', '-webkit-box-shadow': 'rgba(229, 228, 225, 1)', 'padding': '4px' });
                //$divRightTree.find('.k-state-selected').css({ 'border-color': 'rgba(229, 228, 225, 1)', 'background-color': '#DADADA', '-webkit-box-shadow': 'rgba(229, 228, 225, 1)', 'padding': '4px', 'box-shadow': 'transparent' });
                //$divRightTree.find('.k-state-focused.k-state-selected').css({ 'box-shadow': 'none', '-webkit-box-shadow': 'none' });
            }, 100);
        };

        function renameNode(e) {
            //e.preventDefault();


            var val = $($(this).closest(".k-item")).find(".data-id").val();

            var treeview = $divRightTree.data("kendoTreeView");
            var existingName = treeview.dataItem(treeview.select()).text;

            var $root1 = $('<div class="vis-formouterwrpdiv">');
            var $topfields1 = $('<div style="width:100%" class="VA003-form-top-fields">');
            var $name1 = $('<input  maxlength="' + nameLength + '" type="text" data-name="name" placeholder=" " data-placeholder="">');

            $name1.val(existingName);
            $name1.select();

            $topfields1.append($('<div class="VA003-form-data input-group vis-input-wrap">').append($('<div class="vis-control-wrap">').append($name1).append('<label>' + VIS.Msg.getMsg("Name") + '</label>')));
            $root1.append($topfields1);
            var ch1 = new VIS.ChildDialog();
            ch1.setContent($root1);
            ch1.setWidth('350px');
            ch1.setTitle(VIS.Msg.getMsg("VA003_RenameNode"));
            ch1.setModal(true);
            ch1.onClose = function () {
                //if (self.onClose) self.onClose();
                //self.dispose();
            };



            ch1.show();
            ch1.onOkClick = function (e) {

                if ($name1.val().trim().length == 0) {
                    VIS.ADialog.info("EnterName");
                    return;
                }
                $bsyDiv[0].style.visibility = "visible";
                // var queries = [];
                // var sql = '';

                //sql = "UPDATE AD_Org SET name='" + VIS.Utility.encodeText($name1.val()) + "' WHERE AD_Org_ID=" + val;
                // queries.push(sql);

                //console.log(queries);
                //VIS.DB.executeQueries(queries, null, function (ret) {
                //    console.log(ret);
                //});
                VIS.dataContext.getJSONData(VIS.Application.contextUrl + "OrgStructure/UpdateOrgnization", { Name: VIS.Utility.encodeText($name1.val()), AD_Org_ID: val });

                var treeview = $divRightTree.data("kendoTreeView");
                treeview.dataItem(treeview.select()).set('text', VIS.Utility.encodeText($name1.val()));
                //$($(this).closest(".k-item")).find(".data-id").set('text', "New node text");

                $bsyDiv[0].style.visibility = "hidden";
            };

            ch1.onCancelClick = function () {
                // alert("Cancel");
            };






        };

        function deleteNode(e) {
            // e.preventDefault();

            if (!VIS.ADialog.ask("DeleteIt")) {
                return;
            }


            $bsyDiv[0].style.visibility = "visible";
            var treeview = $divRightTree.data("kendoTreeView");
            treeview.remove($(this).closest(".k-item"));


            var val = $($(this).closest(".k-item")).find(".data-id").val();
            var treeID = $($(this).closest(".k-item")).find(".data-id").data("treeid");

            //var queries = [];
            //var sql = '';

            //sql = "DELETE FROM AD_TreeNode WHERE AD_Tree_ID=" + treeID + " AND parent_ID=" + val;
            //queries.push(sql);


            //sql = "DELETE FROM AD_TreeNode WHERE AD_Tree_ID=" + treeID + " AND Node_ID=" + val;
            //queries.push(sql);

            //console.log(queries);
            //VIS.DB.executeQueries(queries, null, function (ret) {
            //    console.log(ret);
            //});
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "OrgStructure/DeleteAD_TreeNode", { TreeId: treeID, Parent_ID: val });
            //VIS.dataContext.getJSONData(VIS.Application.contextUrl + "OrgStructure/DeleteTreeNode", { TreeId: treeID, Node_ID: val });

            $bsyDiv[0].style.visibility = "hidden";

        };

        function addNode(e) {
            //var div = $rightdivContainer.find('#' + $self.windowNo + 'orgrighttree');
            //if (div == undefined || div == null || div.length == 0) {
            //    return;
            //}

            //if ($divRightTree.data("kendoTreeView").select().find(".data-id").val() > 0 && !$divRightTree.data("kendoTreeView").select().find(".data-id").data("summary")) {
            //    VIS.ADialog.info("VA003_SelectSummary");
            //    return;
            //}


            if ($btnAddNode.css('opacity') == '0.5') {
                return;
            }


            var selectedNode = $divRightTree.data("kendoTreeView").select().find(".data-id").val();


            var addnodes = new VA003.OrgStructure.AddNode($cmbReportHirerchy.val(), $self, selectedNode, nameLength, valueLength);
            addnodes.show();
        };

        function slide(e) {
            if ($rightdivContainer.data("isexpanded") == "N") {
                $rightdivContainer.data("isexpanded", "Y");
                $rightdivContainer.animate(
                    { 'width': '67%' }, 800, function () {
                        $btnSlider.find('img').attr('src', VIS.Application.contextUrl + 'Areas/VA003/Images/orgstr-toggle.png');
                        ChangeHeirarcyCmboWidth();
                    });

                $btnSlider.find('a').removeClass('VA003-toggle-icon');
                $btnSlider.find('a').addClass('VA003-edit-icon');

            }
            else {
                closeSlider();
            }


        };

        function closeSlider() {
            if ($rightdivContainer.data("isexpanded") == "Y") {
                $rightdivContainer.data("isexpanded", "N");
                $rightdivContainer.animate(
                    { 'width': '35%' }, 800, function () {
                        $btnSlider.find('img').attr('src', VIS.Application.contextUrl + 'Areas/VA003/Images/arrow-left.png');
                        ChangeHeirarcyCmboWidth();
                    });
                $btnSlider.find('a').addClass('VA003-toggle-icon');
                $btnSlider.find('a').removeClass('VA003-edit-icon');
            }

        };

        function ChangeHeirarcyCmboWidth() {
            //$cmbReportHirerchy.width($divHierarchyCombo.width() -96);
            //$ulRightTree.css("margin-left", $divHierarchyCombo.width() +10);
        };

        this.addNodeToTree = function (data) {
            updateHeirarchyTree(data);
        };

        function addnewTree(e) {
            var $root1 = $('<div class="vis-formouterwrpdiv">');
            var $topfields1 = $('<div style="width:100%" class="VA003-form-top-fields">');
            var $topfields2 = $('<div style="width:100%" class="VA003-form-top-fields">');
            var $name1 = $('<input maxlength="' + treeLength + '" type="text" data-name="name" placeholder=" " data-placeholder="">');
            var $orgUnitTree = $('<input type="checkbox" data-name="isorgunit">');

            $topfields1.append($('<div class="VA003-form-data input-group vis-input-wrap">').append($('<div class="vis-control-wrap">').append($name1).append('<label>' + VIS.Msg.getMsg("Name") + '</label>')));
            $topfields2.append($('<div style="margin-bottom:15px" class="VA003-form-top-fields">').append($orgUnitTree).append('<label>' + VIS.Msg.getMsg("VA003_OrgUnitTree") + '</label>'));

            $root1.append($topfields1);
            $root1.append($topfields2);

            var ch = new VIS.ChildDialog();
            ch.setContent($root1);
            ch.setWidth('350px');
            ch.setTitle(VIS.Msg.getMsg("VA003_AddTree"));
            ch.setModal(true);
            ch.onClose = function () {
                //if (self.onClose) self.onClose();
                //self.dispose();
            };
            ch.show();
            ch.onOkClick = function (e) {

                if ($name1.val().trim().length == 0) {
                    VIS.ADialog.info("EnterName");
                    e.preventDefault();
                    return false;
                }
                $bsyDiv[0].style.visibility = "visible";
                $.ajax({
                    url: VIS.Application.contextUrl + "OrgStructure/AddNewTree",
                    async: true,
                    data: { name: VIS.Utility.encodeText($name1.val().trim()), IsOrgUnit: $orgUnitTree.is(':checked') },
                    success: function (result) {
                        var data = JSON.parse(result);
                        if (data == null || data == undefined) {
                            $bsyDiv[0].style.visibility = "hidden";
                            return null;
                        }
                        $cmbReportHirerchy.empty();
                        var AllReportHierarchy = data.AllReportHierarchy;

                        fillCmbHierarchy(AllReportHierarchy);

                    }
                });
            };

            ch.onCancelClick = function () {
                //   alert("Cancel");
            };

        };

        function fillCmbHierarchy(AllReportHierarchy) {
            $cmbReportHirerchy.empty();
            if (AllReportHierarchy) {
                for (var i = 0; i < AllReportHierarchy.length; i++) {

                    var aa = '<option ';

                    if (AllReportHierarchy[i].Selected) {
                        aa += ' selected ';
                    }


                    if (AllReportHierarchy[i].IsDefault) {
                        aa += ' data-isDefault="Y"';
                    }
                    else {
                        aa += ' data-isDefault="N" ';
                    }

                    aa += ' value=' + AllReportHierarchy[i].Key + '>' + VIS.Utility.encodeText(AllReportHierarchy[i].Name) + '</option>';
                    $cmbReportHirerchy.append(aa);
                }
            }

            $cmbReportHirerchy.trigger("change");
            $bsyDiv[0].style.visibility = "hidden";
        };


        function showInfo(e) {

            $root.find('.VA003-infoRoot-leftDivContainer').css('display', 'inherit');
            var info = new VA003.OrgStructure.info($self.removeinfo);
            info.init();
            infoRoot = info.getRoot();

            $root.prepend(infoRoot);
        };

        function createFirstTabInfo() {

            var $leftContainer = $('<div  class="VA003-infoRoot-leftDivContainer">');

            var $leftDiv = $('<div  class="VA003-infoRoot-leftDiv">');
            $($leftdivContainer.find('.VA003-left-tree')).append($leftContainer);

            var $leftdiv1 = $('<div>');
            var $leftdiv2 = $('<div>');
            var $leftdiv3 = $('<div>');
            var $leftdiv4 = $('<div>');
            var $leftdiv5 = $('<div>');
            var $leftdiv6 = $('<div>');

            $leftdiv1.append($('<div class="VA003-Info-Icons" ><img class="VA003-infoRoot-icon" src="' + VIS.Application.contextUrl + 'Areas/VA003/Images/orgstr-org1.png"></div>'));
            $leftdiv1.append($('<div><div><span class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_SummaryNodes') + '</span></div><div><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_SumaryInfo') + '</span></div></div>'));

            $leftdiv2.append($('<div class="VA003-Info-Icons"><img src="' + VIS.Application.contextUrl + 'Areas/VA003/Images/orgstr-legal-entity1.png"></div>'));
            $leftdiv2.append($('<div><div><span class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_LegalNodes') + '</span></div><div><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_LegalInfo') + '</span></div></div>'));

            $leftdiv3.append($('<div class="VA003-Info-Icons"><img class="VA003-infoRoot-icon" src="' + VIS.Application.contextUrl + 'Areas/VA003/Images/orgstr-store1.png"></div>'));
            $leftdiv3.append($('<div><div><span class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_OrgUnitsNodes') + '</span></div><div><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_NonLegalInfo') + '</span></div></div>'));


            $leftdiv4.append($('<div class="VA003-Info-Icons"><img class="VA003-infoRoot-icon" src="' + VIS.Application.contextUrl + 'Areas/VA003/Images/orgstr-edit.png"></div>'));
            $leftdiv4.append($('<div><div><span class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('Edit') + '</span></div><div><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_EditInfo') + '</span></div></div>'));

            $leftdiv5.append($('<div class="VA003-Info-Icons"><img class="VA003-infoRoot-icon" src="' + VIS.Application.contextUrl + 'Areas/VA003/Images/orgstr-folder-minus.png"></div>'));
            $leftdiv5.append($('<div><div><span class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_ExpandTree') + '</span></div><div><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_ExpandTreeInfo') + '</span></div></div>'));

            $leftdiv6.append($('<div class="VA003-Info-Icons"><img class="VA003-infoRoot-icon" src="' + VIS.Application.contextUrl + 'Areas/VA003/Images/orgstr-folder-add.png"></div>'));
            $leftdiv6.append($('<div><div><span class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_CollapseTree') + '</span></div><div><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_CollapseTreeInfo') + '</span></div></div>'));



            $leftDiv.append($leftdiv1).append($leftdiv2).append($leftdiv3).append($leftdiv4).append($leftdiv5).append($leftdiv6);

            $leftContainer.append($leftDiv);
        };

        function createSecondtabInfo() {

            var $leftContainer = $('<div  class="VA003-infoRoot-leftDivContainer">');

            var $leftDiv = $('<div  class="VA003-infoRoot-leftDiv">');
            $leftDiv.append($('<div style="display: block; padding: 0 15px;"><div style=" width: 100%; float: left; "><span class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_OrgInfo') + '</span></div><div><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_OrgnizationInfo') + '</span></div></div>'));

            var $leftdiv4 = $('<div style="float:left;width:100%">');

            $leftdiv4.append($('<div class="VA003-Info-Icons"><img class="VA003-infoRoot-icon" src="' + VIS.Application.contextUrl + 'Areas/VA003/Images/orgstr-toggle.png"></div>'));
            $leftdiv4.append($('<div><div><span class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_EditUndo') + '</span></div><div><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_EditUndoInfo') + '</span></div></div>'));
            $leftDiv.append($leftdiv4);
            $divFormWrap.append($leftContainer);
            $leftContainer.append($leftDiv);
        };

        function createThirdTabInfo() {
            var $leftContainer = $('<div style="margin-top:65px" class="VA003-infoRoot-leftDivContainer">');

            var $leftDiv = $('<div  class="VA003-infoRoot-leftDiv">');
            $leftDiv.append($('<div style="display: block; padding: 0 15px;"><div style=" width: 100%; float: left; "><span class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_ReportingHierarchy') + '</span></div><div><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_HieraryInfo') + '</span></div></div>'));
            //$leftDiv.append($('<div style="overflow-y;margin-bottom:5px"><div style="overflow:auto"><span data-uid="1005338"  class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_LegalNodes') + '</span></div><div style="overflow:auto"><span class="VA003-infoRoot-infos">this sis asdf asfa sfdgasi fgaispudft asfa</span></div></div>'));
            var $leftdiv1 = $('<div>');
            var $leftdiv2 = $('<div>');
            var $leftdiv5 = $('<div>');
            var $leftdiv6 = $('<div>');


            $leftdiv5.append($('<div class="VA003-Info-Icons"><img class="VA003-infoRoot-icon" src="' + VIS.Application.contextUrl + 'Areas/VA003/Images/orgstr-folder-minus.png"></div>'));
            $leftdiv5.append($('<div><div><span class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_ExpandTree') + '</span></div><div><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_ExpandTreeInfo') + '</span></div></div>'));

            $leftdiv6.append($('<div class="VA003-Info-Icons"><img class="VA003-infoRoot-icon" src="' + VIS.Application.contextUrl + 'Areas/VA003/Images/orgstr-folder-add.png"></div>'));
            $leftdiv6.append($('<div><div><span class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_CollapseTree') + '</span></div><div><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_CollapseTreeInfo') + '</span></div></div>'));

            $leftdiv1.append($('<div class="VA003-Info-Icons"><img class="VA003-infoRoot-icon" src="' + VIS.Application.contextUrl + 'Areas/VA003/Images/orgstr-addtree.png"></div>'));
            $leftdiv1.append($('<div><div><span class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_AddTree') + '</span></div><div><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_AddTreeInfo') + '</span></div></div>'));

            $leftdiv2.append($('<div class="VA003-Info-Icons"><img class="VA003-infoRoot-icon" src="' + VIS.Application.contextUrl + 'Areas/VA003/Images/orgstr-tree-node.png"></div>'));
            $leftdiv2.append($('<div><div><span class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_AddNode') + '</span></div><div><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_AddNodeinfo') + '</span></div></div>'));

            $leftDiv.append($leftdiv1).append($leftdiv2).append($leftdiv5).append($leftdiv6);
            $rightdivContainer.append($leftContainer);
            $leftContainer.append($leftDiv);
        };

        this.removeinfo = function () {
            $root.find('.VA003-infoRoot-leftDivContainer').css('display', 'none');
            infoRoot.remove();
        };

        this.formSizeChanged = function (height, width) {
            if (height) {

                $leftdivContainer.height(height);
                //$rightdivContainer.height(height);
            }

            var treeHeight = $leftdivContainer.outerHeight() - ($buttonsDiv.outerHeight() + $btnGenerateReport.outerHeight() + 25 + 42);
            divTreeContainer.parent().height(treeHeight);
            // + $btnGenerateReport.outerHeight() + 35
            var formHeight = $leftdivContainer.height() - ($buttonsDiv.height() + 35 + 40);
            $divFormWrap.height(formHeight);

            var righttrr = $rightdivContainer.find('#' + $self.windowNo + 'orgrighttree');
            if (righttrr) {
                var topheight = $($rightdivContainer.find('.VA003-right-top')).height();
                var comHeight = $($rightdivContainer.find('.VA003-form-data')).height();
                righttrr.height($rightdivContainer.height() - (35 + 45 + 40));

            }
        };

        /*************************************/

        this.getRoot = function () {
            return $root;
        };

        /*************************************/

        this.disposeComponent = function () {

            $rightdivContainer.off(VIS.Events.onTouchStartOrClick, ".VA003-delete-link");
            $rightdivContainer.off(VIS.Events.onTouchStartOrClick, ".VA003-rename-link");


            $btnNewLegalEntity.off("click");
            $btnSummary.off("click");
            $btnaddNewOrg.off("click");
            $btnSave.off("click");
            $btnUndo.off("click");
            $imgUpload.off("change");
            $chkIsLegal.off("click");
            $chkIsSummary.off("click");
            $btnGenerateReport.off("click");
            $cmbReportHirerchy.off("change");
            $btnAddNode.off("click");
            $btnAddNewTree.off("click");

            if (divLeftTree.data("kendoTreeView") != undefined) {
                divLeftTree.data("kendoTreeView").destroy();
                divLeftTree.empty();
            }

            if ($divRightTree.data("kendoTreeView") != undefined) {
                $divRightTree.data("kendoTreeView").destroy();
                $divRightTree.empty();
            }


            $self.windowNo = null;
            ctx = null;
            ad_Org_ID = 0;
            seletedOrgID = 0;
            selecedSummayNode = 0;
            tableName = "";
            AD_Tree_ID = 0;

            // $cmbTenant.remove();
            $txtSerackKey.remove();
            $txtName.remove();
            //$txtDesc.remove();
            $chkIsLegal.remove();
            $chkIsSummary.remove();
            $cmbOrgType.remove();
            // $cmbParentaOrg.remove();
            $txtTax.remove();
            $txtPhone.remove();
            $txtEmail.remove();
            $txtFax.remove();
            //$txtOrgSuperviser.remove();
            // $txtLocation.remove();
            //$cmbWarehouse.remove();

            // $cmbTenant = null;
            $txtSerackKey = null;
            $txtName = null;
            //$txtDesc = null;
            $chkIsLegal = null;
            $chkIsSummary = null;
            $cmbOrgType = null;
            //  $cmbParentaOrg = null;
            $txtTax = null;
            $txtPhone = null;
            $txtEmail = null;
            $txtFax = null;
            $txtLocation = null;
            $cmbWarehouse = null;


            $imgUpload.remove();
            $imageControl.remove();
            $lblimgUpload.remove();
            $cmbReportHirerchy.remove();
            $lblOrgInfo.remove();


            $imgUpload = null;
            $imageControl = null;
            $lblimgUpload = null;
            $cmbReportHirerchy = null;
            $lblOrgInfo = null;

            $btnNewLegalEntity.remove();
            $btnSummary.remove();
            $btnaddNewOrg.remove();
            $btnSave.remove();
            $btnUndo.remove();
            $btnAddNode.remove();
            $btnAddNewTree.remove();
            $buttonsDiv.remove();

            $btnNewLegalEntity = null;
            $btnSummary = null;
            $btnaddNewOrg = null;
            $btnSave = null;
            $btnUndo = null;
            $btnAddNode = null;
            $btnAddNewTree = null;
            $buttonsDiv = null;


            nameLength = null;
            valueLength = null;
            treeLength = null;
            // $btnAddChildNode.remove();
            $btnAddChildNode = null;
            $btnGenerateReport.remove();
            $btnGenerateReport = null;
            $bsyDiv.remove();
            $bsyDiv = null;

            oldValues = null;
            newValues = null;
            ad_window_Id = null;
            $leftTreeContainer.remove();
            $middleContainer.remove();
            $leftdivContainer.remove();
            $rightdivContainer.remove();
            $divFormWrap.remove();
            divLeftTree.remove();
            divTreeContainer.remove();
            $divRightTree.remove();

            $leftTreeContainer = null;
            $middleContainer = null;
            $leftdivContainer = null;
            $rightdivContainer = null;
            $buttonsDiv = null;
            $divFormWrap = null;
            divLeftTree = null;
            divTreeContainer = null;
            $divRightTree = null;

            $root.remove();
            $root = null;

            $self.frame = null;

            $self = null;

        };

        /*************************************/

        this.lockUI = function (abc) {

        };

        this.unlockUI = function (abc) {

        };

        this.sizeChanged = function (height, width) {
            formHeight = height;
            formWidth = width;
            this.formSizeChanged(height, width);
        }

    };

    OrgStructure.prototype.refresh = function () {
        this.formSizeChanged();
    };

    OrgStructure.prototype.sizeChanged = function (height, width) {
        this.sizeChanged(height, width);
    };


    OrgStructure.prototype.init = function (windowNo, frame) {
        this.frame = frame;
        this.windowNo = windowNo;
        frame.setTitle("VA003_OrgStructure");
        frame.hideHeader(true);
        this.frame.getContentGrid().append(this.getRoot());
        this.initializeComponent();

    };

    OrgStructure.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function

        this.frame = null;
    };

    VA003.OrgStructure = OrgStructure;

})(VA003, jQuery);