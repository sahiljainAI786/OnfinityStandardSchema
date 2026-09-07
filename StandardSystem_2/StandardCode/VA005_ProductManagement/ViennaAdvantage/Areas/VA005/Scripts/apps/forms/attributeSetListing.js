; VA005 = window.VA005 || {};

; (function (VA005, $) {
    function AttributeSetListing() {

        this.windowNo;
        this.frame;
        var $self = this;
        var $root = $("<div class='vis-forms-container' style='height:100%;background-color:white;' >");
        var $bsyDiv = null;
        var $tree = null;
        var treedesign = null;
        var $treeView = null;
        var $divid = null;
        var $openAttributeset = null;
        var $deleteAttribute = null;
        var $openattdialog = null;
        var $addatttextname = null;
        var $$addatttextdes = null;
        var $cmbattsectmandtype = null;
        var $addatttextexpdate = null;
        var $mandatoryexpdate = null;
        var $addatttextlot = null;
        var $addatttextserial = null;
        var $addatttextlotserialtext = null;
        var $cmbSerialDropdown = null;
        var $addattaddbtn = null;
        var $saveattributesetdata = null;
        var $divnametxt = null;
        var $divstarttxt = null;
        var $divcurrenttxt = null;
        var $incrementtext = null;
        var $prefixtext = null;
        var $serfixtext = null;
        var $savelotserialdata = null;
        var $divhideandshow = null;
        var $divLeftTree = null;
        var $getidfromattributeset = null;
        var selectedAttributeID = 0;
        var nodeID = 0;
        var attributeID = 0;
        var $attributeobject = null;
        var $attributeRoot = null;
        var $attributeRoot2 = null;
        var $attobject = null;
        var $icon = null;
        var $iconullist = null;
        var $listid = null;
        var lotvaluee = null;
        var serialvaluee = null;
        var columnLength = null;
        var okbtnattributeset = null;
        var cancelbtnattributeset = null;
        var lotidjson = null;
        var lotidget = null;
        var clumnlengthLotLength = null;
        var columnstartnoLength = null;
        var columncurrenttextLength = null;
        var columnincremnetnoLength = null;
        var columnprefixLength = null;
        var columnsufixLength = null;
        var btnonattributesethideshow = null;
        //var $hideshowbtndiv = null;
        var lotserialdialogobject = null;
        var lotserialdialog = null;
        var hideshowrightmaindiv = null;
        var attributedivdesign = null;
        var islotcheck = false;
        var isserialcheck = false;
        var $opendialog = null;
        var $editbtnoflotserialwindow = null;
        var $divdesignforattribyteset = null;
        var $addnewattribute = null;
        var $attributedivleftdragdrop = null;
        //Changes
        var $attributedivleftdragdrop_header = null;
        /////
        var $attributevaluedivleftdragdrop = null;
        var $findidfromdivboxes = null;
        var $middivforattributevalue = null;
        //var $attributeiddivbox = null;
        var $leftboxdivwidth = null;
        var $hideshowrightmaindiv = null;
        var $delattributeboxes = null;
        //var flag = true;
        //var flag = 1;
        var flag = null;
        this.flag = 1;
        flag = 1;
        var $editattributeboxes = null;
        var $hideshoweditbutton = null;
        var $leftboxbackdivforspliter = null;
        var $splitter = null;
        var $deletemsz = null;

        var $dragevents = false;

        var attributeidgetonokclick = null;
        var findnamecount = null;
        var findnamecount1 = null;
        // var $deletebtnhideshow = null;
        var selectedIds = [];
        var $editbtnatthideshow = null;
        // var $middivforattributevaluehideshow = null;
        var $gettypeoftree = null;
        var $leftboxattvaluebelow = null;
        var $AttributeValuegetid = null;
        var $editattvaluecompare = null;
        var gettargettext = null;
        var textattribute = null;
        //var istanceattribute = null;
        var selecttreeid = null;
        var attrlabelvalue = null;
        var targetdropvaluewithoutreload = 0;
        this.Expandable = false;
        this.getidfromattboxexforaddnode = null;
        this.getidfromdivboxesselect = null;
        this.isSaveOnItemFocus = false;
        var draggabledivid = 0;
        var atttextonactive = null;
        var $dataid = 0;
        var getidafterremovecheck = 0;
        var selectflag = true;


        //================================================
        this.initialize = function () {



            //DragDropDocument();
            // DragDropDocument();

            createBusyIndicator();




            //**** div design for tree...
            treeViewForAttributeSetListing();

            //**** load tree data..          
            loadTreeData();



            //*** attributeset design...
            openAddAtrributeDialog();

            //*** Get mandatory type data..
            getMendatoryTypeData();
            filedlength();

            //*** get attribute design..
            getAttributeRoot();

            //**** Event Handelling...
            eventHandelling();

            attributeAppendDiv();
            //attributevalueAppendDiv();

            //dragdropnewold();

            //this.refresh(10, 10);
            window.setTimeout(function () {

                $self.formSizeChanged();



            }, 200);

        };

        this.ShowDialog = function () {

        };

        this.getRoot = function () {
            return $root;
        };

        //*** First Div for tree view attributeset listing...
        function treeViewForAttributeSetListing() {
            $tree = $("<div style='height:100%' class='vis-formouterwrpdiv' >");
            treedesign = "<div id='" + $self.windowNo + "leftboxdivwidth' class='VA005-right-wrap-leftboxdiv'>"
                + "<div class='VA005-top-wrap-treedesign'>"
                + "<h4>" + VIS.Msg.getMsg("VA005_Attribute") + "</h4>"
                + "<div class='VA005-top-right-treedesign'>"
                // + "<div id='" + $self.windowNo + "editbtnatthideshow' style='z-index:2;display:none;opacity:0.6;height: 50px;position: absolute; width: 2%;'></div>"
                //+ "<div id='" + $self.windowNo + "hideshoweditbutton' style='height: 29px;width: 26px;position: absolute;'></div>"
                //+ "<span><img style='cursor:pointer;margin-top:11px' src='" + VIS.Application.contextUrl + "Areas/VA005/Images/edt.png' id='" + $self.windowNo + "editattributeboxes'></img></span>"
                + "<span id='" + $self.windowNo + "editattributeboxes' class='vis vis-edit' title='" + VIS.Msg.getMsg("VA005_EditAttribute") + "'   ></span>"
                // + "<span style='cursor:pointer;margin-top:3px;font-size:20px'  id='" + $self.windowNo + "editattributeboxes' class='glyphicon glyphicon-edit' >        <div id='1hideshoweditbutton' style='background-color:yellow;height: 27px;width: 26px;position: absolute;margin-top: -20px;'></div>                     </span>"
                + "<span id='" + $self.windowNo + "delattributeboxes' class='vis vis-delete' title='" + VIS.Msg.getMsg("VA005_DeleteAttribute") + "' ></span>"
                //+ "<span><img style='cursor:pointer;display:none' src='" + VIS.Application.contextUrl + "Areas/VA005/Images/udo.jpg' id='" + $self.windowNo + "undoattributeboxes'></img></span>"

                + "</div>"
                + "</div>"
                + "<div class='VA005-att-tree-vieww'>"
                + "</div>"

                ///*** top div of left side for attribute....
                + "<div id='" + $self.windowNo + "leftboxbackdivforspliter' class='VA005-att-lefttopwrap'>"
                //+ "<div style='float:left;width:100%;border-bottom:1px solid #ccc;overflow-y:auto;overflow-x:none;height:101%' class='VA005_maindivleft' id='" + $self.windowNo + "attributedivleftdragdrop'>"
                + "<div class='VA005-att-lefttopinnerwrap VA005_maindivleft' id='" + $self.windowNo + "attributedivleftdragdrop'>"
                + "</div>"
                // + "<div style='cursor:n-resize;background-color:pink;float:left;width:100%;height:10px' class='VA005_splitter' id='" + $self.windowNo + "VA005_splitter'>"
                //  + "</div>"
                + "</div>"

                ///*** bottom div of left side for attribute....
                + "<div id='" + $self.windowNo + "leftboxattvaluebelow'  style='height:45%;'>"
                + "<div id='" + $self.windowNo + "AttributeValuegetid' class='VA005-att-tree-vieww''>"
                + "<h5>" + VIS.Msg.getMsg("VA005_AttributeValue") + "</h5>"
                + "</div>"
                + "<div style='float:left'>"
                + "</div>"
                + "<div style='margin-left:0px;overflow:auto;height:76%;float:left' id='" + $self.windowNo + "attributevaluedivleftdragdrop'>"
                + "</div>"
                + "</div>"
                + "</div>"


                + "<div id='" + $self.windowNo + "middivforattributevalue' class='VA005-right-wrap-leftboxdiv' style='overflow:auto;display:none;width: 30%;'>"
                //+ "<div id='" + $self.windowNo + "middivforattributevaluehideshow' style='display:none;height: 71%;  position: absolute; width: 29%; opacity: 0.6; z-index: 3;'></div>"

                + "</div>"


                //*** right div for attributeset listing....
                + "<div id='" + $self.windowNo + "btndiv' class='VA005-right-wrap-div-setlisting-top'>"
                //+ "<div id='" + $self.windowNo + "hideshowrightdiv' style='width:100%;height:auto;' >"
                + "<div class='VA005-top-wrap-div-h VA005-top-wrap-treedesign'>"
                + "<h4>" + VIS.Msg.getMsg("VA005_AttributeSetListing") + "</h4>"
                + "<div style='position:relative' class='VA005-top-right-treedesign VA005-top-right-div-openattset'>"
                //+ "<span style='margin-right:10px'><img src='" + VIS.Application.contextUrl + "Areas/VA005/Images/add.png' id='" + $self.windowNo + "openattributeset'></img></span>"
                + "<span id='" + $self.windowNo + "openattributeset' class='vis vis-plus' title='" + VIS.Msg.getMsg("VA005_OpenAttributeSet") + "'   ></span>"

                //+ "<div id='" + $self.windowNo + "deletebtnhideshow' style='opacity:0.6;height: 37px;width: 27px;position: absolute;float: right;top: 0;right: 0;z-index: 2;'></div>"

                + "<span id='" + $self.windowNo + "deleteattribute' class='VA005-disabled vis vis-delete' title='" + VIS.Msg.getMsg("VA005_DeleteAttributeSet") + "' ></span>"
                //+ "<span><img src='" + VIS.Application.contextUrl + "Areas/VA005/Images/del.png' id='" + $self.windowNo + "openattribute'></img></span>"
                + "</div>"
                + "</div>"
                //+ "<div class='VA005-att-tree-div-showtree' style='height:95%'>"
                //+ "<div id='" + $self.windowNo + "showtree' style='height:95%;width:auto'></div>"
                + "<div style='z-index:999999' class='VA005-att-tree-div-showtree'  >"
                //+ "<div style='z-index:999999' id='" + $self.windowNo + "showtree' style='min-width:420px;-webkit-overflow-scrolling:touch;' ></div>"
                + "<div  id='" + $self.windowNo + "showtree' style='min-width:350px;-webkit-overflow-scrolling:touch;' ></div>"
                + "</div>"
                //-webkit-overflow-scrolling: touch;
                + "</div>"
                + "<span id='" + $self.windowNo + "deletemsz' style='display:none;color:red'>" + VIS.Msg.getMsg("VA005_DeletSuccessfully") + "</span>"
                + "</div>"
            $tree.append(treedesign);

            $deletemsz = $tree.find("#" + $self.windowNo + "deletemsz");

            $AttributeValuegetid = $tree.find("#" + $self.windowNo + "AttributeValuegetid");

            $leftboxattvaluebelow = $tree.find("#" + $self.windowNo + "leftboxattvaluebelow");

            //  $deletebtnhideshow = $tree.find("#" + $self.windowNo + "deletebtnhideshow");

            //$editbtnatthideshow = $tree.find("#" + $self.windowNo + "editbtnatthideshow");

            // $middivforattributevaluehideshow = $tree.find("#" + $self.windowNo + "middivforattributevaluehideshow");

            $splitter = $tree.find("#" + $self.windowNo + "VA005_splitter");
            $leftboxbackdivforspliter = $tree.find("#" + $self.windowNo + "leftboxbackdivforspliter");
            $hideshoweditbutton = $tree.find("#" + $self.windowNo + "hideshoweditbutton");

            //*** left full div...
            $leftboxdivwidth = $tree.find("#" + $self.windowNo + "leftboxdivwidth");

            //*** edit image on left top div....
            $editattributeboxes = $tree.find("#" + $self.windowNo + "editattributeboxes");

            //*** Undo image on left top div....
            $delattributeboxes = $tree.find("#" + $self.windowNo + "delattributeboxes");

            $middivforattributevalue = $tree.find("#" + $self.windowNo + "middivforattributevalue");
            //***plus btn for add new attributeset value...
            $addnewattribute = $tree.find("#" + $self.windowNo + "addnewattribute");

            //*** append attribute divboxes leftside...
            $attributedivleftdragdrop = $tree.find("#" + $self.windowNo + "attributedivleftdragdrop");

            $attributedivleftdragdrop_header = $tree.find("#" + $self.windowNo + "attributedivleftdragdrop_header");

            //*** append attribute value boxes left side...
            $attributevaluedivleftdragdrop = $tree.find("#" + $self.windowNo + "attributevaluedivleftdragdrop");

            //*** tree div id..
            $divid = $tree.find("#" + $self.windowNo + "showtree");

            $hideshowrightmaindiv = $tree.find("#" + $self.windowNo + "hideshowrightdiv");

            //*** For New Plus Button on the top of tree..
            $openAttributeset = $tree.find("#" + $self.windowNo + "openattributeset");

            //*** It will be work for Delete function, top of the page, delete icon
            $deleteAttribute = $tree.find("#" + $self.windowNo + "deleteattribute");

            $root.append($tree);

            //** get main right div id in treeview
            $treeView = $tree.find("#" + $self.windowNo + "btndiv");


            //$attributedivleftdragdrop.splitter({ type: 'h' });

        };

        var getidoflablefordrop = null;
        var attributeidbydivboxes = [];
        var attributesetidbydivboxes = null;
        var $senddivatt = null;
        //*** attribute box show in left box..
        ///** Today
        function attributeAppendDiv() {

            $attributedivleftdragdrop.empty();
            var str = "<div class='VA005-attributedivstatic  VA005-divboxtarget-design-hover' title='" + VIS.Msg.getMsg("VA005_AddAttribute") + "' >"
                // var str = "<div style='cursor:pointer' class='VA005-attributedivstatic  VA005-divboxtarget-design-hover' VA005_plusimg='N'>"
                + "<span><i class='vis vis-plus' id='" + $self.windowNo + "addnewattribute'></i></span>"
                + "</div><div class='VA005-innerattrb'></div>";
            $attributedivleftdragdrop.append($(str));

            $attributedivleftdragdrop.find('.VA005-attributedivstatic').on("click", staticdivappendinleftboxes);

            $senddivatt = $attributedivleftdragdrop.find('.VA005-attributedivstatic');

            //var sql = "SELECT NAME, M_Attribute_ID FROM M_Attribute WHERE ISACTIVE='Y'";
            //var sql = "SELECT NAME, M_Attribute_ID FROM M_Attribute WHERE isactive='Y' ORDER BY M_Attribute_ID DESC";

            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/AttributeListing/GetAttribute", "", AttributeAppendCatCallBack);


            //var sql = "SELECT NAME, M_Attribute_ID,isactive FROM M_Attribute  ORDER BY M_Attribute_ID DESC";
            //sql = VIS.MRole.addAccessSQL(sql, "M_Attribute", true, true);
            //var ds = VIS.DB.executeDataSet(sql, null, null);


            // if (ds != null) {
            //    var str = '';
            //    for (var i = 0; i < ds.tables[0].rows.length; i++) {
            //        //str += "<div style='cursor:pointer' data-id='" + VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells["m_attribute_id"]) + "' class='VA005-attributediv  VA005-divboxtarget-design-hover'>"
            //        //                 //+ "<label  style='font-weight:normal'>" + ds.tables[0].rows[i].cells["name"] + "</label>"
            //        //                 + "<div style='float: left; width: 100%;'><input attid=" + VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells["m_attribute_id"]) + " class='VA005-checkboxonleftdiv' type='checkbox' style='float: left;'></div>"
            //        //                 //+ "<label id='" + $self.windowNo + "lableidofdivboxes' style='word-break:break-word;line-height:20px;cursor:pointer;font-weight:normal'>" + ds.tables[0].rows[i].cells["name"] + "</label>"
            //        //                  + "<label id='" + $self.windowNo + "lableidofdivboxes' style='word-break:break-word;line-height: 44px;cursor:pointer;font-weight:normal;float: left;text-align: center;width: 100%;'>" + ds.tables[0].rows[i].cells["name"] + "</label>"
            //        //         + "</div>";

            //        str += getTemaplate(VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells["m_attribute_id"]), VIS.Utility.encodeText(ds.tables[0].rows[i].cells["name"]), ds.tables[0].rows[i].cells["isactive"]);

            //        //if (ds.tables[0].rows[i].cells["isactive"] == "N") {
            //        //     
            //        //    return
            //        //}
            //    }



            // $attributedivleftdragdrop.find('.VA005-innerattrb').append($(str));
            //$attributedivleftdragdrop.append($(str));

            ////getidoflablefordrop = $attributedivleftdragdrop.find("#" + $self.windowNo + "lableidofdivboxes");

            ////$attributedivleftdragdrop.find('.VA005-attributediv').on("click", attributeDivClicked);
            //$attributedivleftdragdrop.find('.VA005-attributediv').on(VIS.Events.onTouchStartOrClick, attributeDivClicked);

            ////*** check box for multiple selction delete....
            ////$attributedivleftdragdrop.find('.VA005-checkboxonleftdiv').on("click", function (evt) {
            ////    checkboxclickformultyselect(evt, $(this));
            ////});

            //// Done by shifali on 30th july to drag multiple attributes
            //$attributedivleftdragdrop.find('.VA005-attributediv').draggable({
            //    cursorAt: { left: -10, top: -10 },
            //    helper: function () {
            //        // Getting attributes which needs to be dragged
            //        var selected = $($attributedivleftdragdrop.find('.VA005-attributediv div').find("input:checked")).parent().parent();
            //        if (selected.length === 0) {
            //            selected = $(this);
            //        }
            //        var container = $('<div/>').attr('id', 'draggingContainer');
            //        container.append(selected.clone());
            //        return container;
            //    },
            //    start: function (event, ui) {
            //        $dragevents = true;
            //    },
            //    drag: function (event, ui) {
            //    },

            //    stop: function () {
            //        $dragevents = false;
            //    }
            //});


            //$attributedivleftdragdrop.find('.VA005-attributediv').draggable({
            //    //zIndex: 2,
            //    revert: "invalid",
            //    helper: "clone",
            //    containment: $root,
            //    start: function (event, ui) {
            //        debugger;
            //        //$($($(this).find('.k-state-hover').parents('li')[0]).find('.k-state-hover')).css('z-index', '99999');
            //        //$($divLeftTree.find('.va005-parentss').parent()).css('z-index', '99999');

            //            //attrlabelvalue = $(this).find('label');
            //            //draggabledivid = $(this).attr("data-id");


            //        for (var j = 0; j < selectedIds.length; j++) {
            //            attrlabelvalue = $($($attributedivleftdragdrop.find("[data-id='" + selectedIds[j] + "']"))[0]).find('label');
            //            draggabledivid = $($($attributedivleftdragdrop.find("[data-id='" + selectedIds[j] + "']"))[0]).attr("data-id");
            //            $dragevents = true;
            //        }
            //    },
            //    drag: function (event, ui) {
            //        if ($(this).data('isactive') == 'N') {
            //            $dragevents = false;
            //            return false;
            //        }
            //    },

            //    stop: function () {
            //        $dragevents = false;
            //    }
            //    //hoverClass: function () {
            //    //         
            //    //        if ($(event.target).children().eq(1).find("a").data("dtype") != 1) {
            //    //            return;
            //    //        }
            //    //        $(event.target).css("background-color", "red");
            //    //    },                  


            //});




            //       $divLeftTree.data("kendoTreeView").wrapper.off("mouseenter mouseleave").find(".k-item > div:only-child .k-in") .on({
            //           mouseenter: function () {  $(this).addClass("k-state-hover"); },
            //    mouseleave: function () { $(this).removeClass("k-state-hover"); }
            //});




            //$divLeftTree.kendoDropTarget({
            //    drop: function (e)
            //    {
            //        if ($(event.target).children().eq(1).find("a").data("dtype") != 1) {
            //                                return;
            //                            }
            //                            $(event.target).css("background-color", "red");
            //    }
            //});


            //$divLeftTree.find(".va005_mouseover").mouseover(function () {
            //    if ($(event.target).children().eq(1).find("a").data("dtype") != 1) {
            //                    return;
            //                }
            //                $(event.target).css("background-color", "red");
            //})


            //}
        };


        function AttributeAppendCatCallBack(dr) {
            var str = '';
            if (dr.length > 0) {
                for (var i = 0; i < dr.length; i++) {
                    str += getTemaplate(VIS.Utility.Util.getValueOfInt(dr[i].m_attribute_id), VIS.Utility.encodeText(dr[i].name), dr[i].isactive);
                }
            }

            $attributedivleftdragdrop.append($(str));

            //getidoflablefordrop = $attributedivleftdragdrop.find("#" + $self.windowNo + "lableidofdivboxes");

            //$attributedivleftdragdrop.find('.VA005-attributediv').on("click", attributeDivClicked);
            $attributedivleftdragdrop.find('.VA005-attributediv').on(VIS.Events.onTouchStartOrClick, attributeDivClicked);

            //*** check box for multiple selction delete....
            //$attributedivleftdragdrop.find('.VA005-checkboxonleftdiv').on("click", function (evt) {
            //    checkboxclickformultyselect(evt, $(this));
            //});

            // Done by shifali on 30th july to drag multiple attributes
            $attributedivleftdragdrop.find('.VA005-attributediv').draggable({
                cursorAt: { left: -10, top: -10 },
                helper: function () {
                    // Getting attributes which needs to be dragged
                    var selected = $($attributedivleftdragdrop.find('.VA005-attributediv div').find("input:checked")).parent().parent();
                    if (selected.length === 0) {
                        selected = $(this);
                    }
                    var container = $('<div/>').attr('id', 'draggingContainer');
                    container.append(selected.clone());
                    return container;
                },
                start: function (event, ui) {
                    $dragevents = true;
                },
                drag: function (event, ui) {
                },

                stop: function () {
                    $dragevents = false;
                }

            });

        };

        function DropItem() {
            $divid.find(".k-in").droppable({
                tolerance: 'pointer',
                drop: function (event, ui) {
                    debugger;
                    $bsyDiv[0].style.visibility = "visible";
                    //window.setTimeout(function () {
                    // attributeidbydivboxes = ($(ui.helper)).data('id');
                    // attributesetidbydivboxes = ($(($($(this).find('.k-state-hover').parents('li')[0])).find('a')[0])).data('NID');
                    //alert(attributesetidbydivboxes);

                    // Done by Shifali on 30th july to drop multiple selected attributes
                    for (var i = 0; i < selectedIds.length; i++) {
                        attributeidbydivboxes.push($($($attributedivleftdragdrop.find("[data-id='" + selectedIds[i] + "']"))[0]).attr("data-id"));
                    }
                    attributesetidbydivboxes = $(this).children().eq(0).attr('NID');
                    if (attributeidbydivboxes.length > 0) {
                        saveAttribute(attributeidbydivboxes, attributesetidbydivboxes);
                        // loadTreeData();
                        //}, 200);
                        attributeidbydivboxes = [];
                    }
                    else {
                        $bsyDiv[0].style.visibility = "hidden";
                    }
                }
            });
        };


        function saveAttribute(attributeidbydivboxes, attributesetidbydivboxes) {

            $bsyDiv[0].style.visibility = "visible";
            //    var valuesendtoctrl = {
            //        attributsetid: attributeidbydivboxes,
            //        lablename: attributesetidbydivboxes
            //};
            var attributsetid = attributeidbydivboxes.toString();
            $.ajax({
                url: VIS.Application.contextUrl + "Attribute/SaveAttributeuses",
                type: 'Post',
                async: false,
                datatype: "Json",
                data: { attributsetid: attributsetid, nid: attributesetidbydivboxes },
                success: function (data) {
                    //selectedAttributeID = JSON.parse(data);
                    var result = JSON.parse(data);
                    loadTreeData();


                    //var objNewNode = {};
                    //objNewNode["text"] = attrlabelvalue
                    //objNewNode["nodeid"] = result;                    
                    //ImageSource = "Areas/VA005/Images/attSet.png";
                    //var selectedNode = $($($divLeftTree.find('.k-state-hover').parents('li')[0])).find('.k-top').find('.k-state-hover');



                    //var selectedNode = $($($divLeftTree.find('.k-state-hover').parents('li')[0])).find('.k-state-hover');

                    //var newChild = $divLeftTree.data("kendoTreeView").append({
                    //    ImageSource: "Areas/VA005/Images/att.png",
                    //    text: attrlabelvalue.text(),
                    //    'NodeID': result,
                    //    'UID': attributeidbydivboxes + "_2_" + $self.windowNo,
                    //    'Type': '2'

                    //}, selectedNode);

                    //selectedNode.dataSource.read();



                    //newChild.find('img').css('margin', '6px 20px 0px 30px');
                    //  newChild.find('p').css({ 'margin': '7px 47px 0px 30px' });


                    // newChild.find(".va005-editattributecls").on("click", editItem);


                    //callattvalue(draggabledivid);

                    // loadTreeData();
                    //$divLeftTree.data("kendoTreeView").select(newChild);
                    $bsyDiv[0].style.visibility = "hidden";



                },
                error: function (data) {
                    //alert(data)
                },

            });
            // attributeidgetonokclick = selectedAttributeID;
        };

        //var attvaluefetch = null;
        //var attvaluefetchid = null;
        //function callattvalue(draggabledivid)
        //{
        //     
        //    var sql = "SELECT m_attribute_ID, name,M_AttributeValue_ID FROM M_AttributeValue WHERE IsActive='Y' AND m_attribute_id=" + draggabledivid;
        //    //         SELECT m_attribute_ID, name,M_AttributeValue_ID FROM M_AttributeValue WHERE IsActive='Y' and m_attribute_id='1000742';
        //    sql = VIS.MRole.addAccessSQL(sql, "M_AttributeValue", true, true);
        //    var ds = VIS.DB.executeDataSet(sql, null, null);

        //    for (var i = 0; i < ds.tables[0].rows.length; i++) {

        //        attvaluefetch += VIS.Utility.encodeText(ds.tables[0].rows[i].cells["name"]);
        //        attvaluefetchid += VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[0].cells["m_attributevalue_id"]);
        //        //str += "<div data-id='" + VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells["m_attributevalue_id"]) + "' class='VA005-attributediv  VA005-divboxtarget-design-hover'>"
        //        //                //+ "<label class='VA005-textoverfllowhidden1' title='" + VIS.Utility.encodeText(ds.tables[0].rows[i].cells["name"]) + "'>" + VIS.Utility.encodeText(ds.tables[0].rows[i].cells["name"]) + "</label>"
        //        //                + "<label class='VA005-textoverfllowhidden2' title='" + VIS.Utility.encodeText(ds.tables[0].rows[i].cells["name"]) + "'>" + VIS.Utility.encodeText(ds.tables[0].rows[i].cells["name"]) + "</label>"
        //        //        + "</div>";
        //    }


        //};



        function staticdivappendinleftboxes() {

            debugger;
            $editattributeboxes.removeClass("VA005-disabled");
            if ($attributeobject.$getvalueonchange == true) {
                //var msg = "" + VIS.Msg.getMsg("VA005_SaveAttributeIt") + "";
                // var r = VIS.ADialog.ask(msg);
                // Added by Shifali to print message acc to culture JID_1864
                VIS.ADialog.confirm("VA005_SaveAttributeIt", true, "", "Confirm", function (result) {
                    if (result == true) {
                        $attributeobject.onchangesaveattribute();
                        $attributeobject.$getvalueonchange = false;
                    }
                    else {
                        $attributeobject.$getvalueonchange = false;
                    }
                });
            }

            $bsyDiv[0].style.visibility = "visible";

            $delattributeboxes.removeClass("VA005-disabled");

            selectedAttributeID = 0;
            selectedIds = [];
            saveAttributeonaddplusbtn();
            $leftboxdivwidth.animate({ width: "40%" });
            $openattdialog.css("display", "none");
            $treeView.animate({ width: "30%" });
            $treeView.css("display", "inherit");
            ////*** Show attribute dialog
            //objectcall();
            $attributeobject.cleartext();
            $middivforattributevalue.animate({ width: "30%" });
            $middivforattributevalue.css({ "display": "inherit", 'width': '30%' });

            $attributeobject.attributecount(findnamecount);

            $attributedivleftdragdrop.find('.VA005-attributediv').draggable('destroy');

            var str = getTemaplate(selectedAttributeID, (VIS.Msg.getMsg("VA005_Attribute") + findnamecount), 'Y');
            //str = "<div  data-id='" + selectedAttributeID + "' style='word-break:break-word;line-height:20px;cursor:pointer' id='" + $self.windowNo + "divappendinleftsideboxes_header' class='VA005-attributediv  VA005-divboxtarget-design-hover'>"
            //    + "<div style='float: left; width: 100%;'><input attid='" + selectedAttributeID + "' class='VA005-checkboxonleftdiv' type='checkbox' style='float: left;'></div>"
            //         + "<label style='word-break:break-word;line-height:43px;font-weight:normal;cursor:pointer'>" + VIS.Msg.getMsg("VA005_Attribute") + findnamecount + "</label>"
            //      + "</div>";            
            $attributedivleftdragdrop.find('.VA005-innerattrb').prepend($(str));

            //$editattributeboxes.css("display", "inherit");

            //*** Show attribute dialog
            objectcall();

            $attributedivleftdragdrop.attr("VA005_plusimg", 'Y');


            $attributedivleftdragdrop.find('.VA005-divboxtarget-design').removeClass('VA005-divboxtarget-design');
            //===
            $($attributedivleftdragdrop.children()).find('div .VA005-checkboxonleftdiv').removeAttr("checked");


            $($attributedivleftdragdrop.find('.VA005-innerattrb').children('div')[0]).addClass('VA005-divboxtarget-design');
            $($attributedivleftdragdrop.find('.VA005-innerattrb').children('div')[0]).find('.VA005-checkboxonleftdiv').prop("checked", true);
            //$($attributedivleftdragdrop.find('.VA005-innerattrb').children('div')[0]).addClass('VA005-textcolorchange-onedit');

            $($($attributedivleftdragdrop.children()).find('label')).removeClass('VA005-textcolorchange-onedit');

            //$($attributedivleftdragdrop.children()).removeClass("VA005-textcolorchange-onedit");
            textattribute = $($attributedivleftdragdrop.find('.VA005-innerattrb').children('div')[0]).find('label');
            //$($attributedivleftdragdrop.children().find('label')).removeClass("VA005-textcolorchange-onedit");
            //$(target.find('label')).addClass("VA005-textcolorchange-onedit");

            selectedIds.push(selectedAttributeID);
            //$attributedivleftdragdrop.find('.VA005-checkboxonleftdiv').on("click", function (evt) {
            //    checkboxclickformultyselect(evt, $(this));
            //});


            $attributedivleftdragdrop.find('.VA005-attributediv').off(VIS.Events.onTouchStartOrClick, attributeDivClicked);
            $attributedivleftdragdrop.find('.VA005-attributediv').on(VIS.Events.onTouchStartOrClick, attributeDivClicked);
            $attributedivleftdragdrop.find('.VA005-attributediv').draggable({

                start: function (event, ui) {

                    $dragevents = true;

                    attrlabelvalue = $(this).find('label');
                },
                drag: function (event, ui) {
                    if ($(this).data('isactive') == 'N') {
                        $dragevents = false;
                        return false;
                    }
                },


                stop: function () {
                    $dragevents = false;
                },
                revert: "invalid",
                helper: "clone",
                containment: $root,
            });

            //$divid.find(".k-in").droppable({

            //    drop: function (event, ui) {

            //        $bsyDiv[0].style.visibility = "visible";
            //        attributeidbydivboxes = ($(ui.draggable)).data('id');
            //        // attributesetidbydivboxes = ($(($($(this).find('.k-state-hover').parents('li')[0])).find('a')[0])).data('nodeid');
            //        attributesetidbydivboxes = $(this).children().eq(0).attr('NID');
            //        //$divLeftTree
            //        saveAttribute(attributeidbydivboxes, attributesetidbydivboxes);
            //        // loadTreeData();
            //        //$bsyDiv[0].style.visibility = "hidden";

            //        // $attributeobject.gridekendobtns();
            //    }
            //});
            //$attributedivleftdragdrop.find('.VA005-attributediv').draggable({
            //    revert: "invalid",
            //    helper: "clone",
            //    containment: $root,
            //});

            //$divid.droppable({
            //    drop: function (event, ui) {
            //        $bsyDiv[0].style.visibility = "visible";
            //        attributeidbydivboxes = ($(ui.draggable)).data('id');VA005-textoverfllowhidden
            //        attributesetidbydivboxes = ($(($($(this).find('.k-state-hover').parents('li')[0])).find('a')[0])).data('nodeid');

            //        saveAttribute(attributeidbydivboxes, attributesetidbydivboxes);
            //        loadTreeData();
            //        $bsyDiv[0].style.visibility = "hidden";

            //        // $attributeobject.gridekendobtns();
            //    }
            //});

            $bsyDiv[0].style.visibility = "hidden";
            //dragdropnewold();

        };


        ////////////////////========================
        function getTemaplate(selectedAttributeID, findnamecount, isActive) {
            str = "<div data-isActive='" + isActive + "' data-id='" + selectedAttributeID + "' title='" + findnamecount + "' id='" + $self.windowNo + "divappendinleftsideboxes_header' class='VA005-attributediv  VA005-divboxtarget-design-hover'>"
                + "<div class='VA005-attrdiv style='float: left; width: 100%;'><input attid='" + selectedAttributeID + "' class='VA005-checkboxonleftdiv' type='checkbox' style='float: left;'></div>";
            //+ "<label style='word-break:break-word;line-height:20px;margin-top:10px;font-weight:normal;cursor:pointer'>" + findnamecount + "</label>"
            //+ "<label class='VA005-textoverfllowhidden'>" + findnamecount + "</label>"

            if (isActive == 'Y') {
                str += "<label class='VA005-textoverfllowhidden'>" + findnamecount + "</label>";
            }
            else {
                str += "<label class='VA005-textoverfllowhidden-lightcolor'>" + findnamecount + "</label>";
            }
            str += "</div>";
            //-----------------------------
            //$dataid = str.find("#" + $self.windowNo + "data-id");



            return str;
        };

        //*** Update text of left div
        this.updateSelectedAttribute = function (value) {

            $($attributedivleftdragdrop.find('.VA005-divboxtarget-design').find('label')).text(value);
            //atttextonactive = $($attributedivleftdragdrop.find('.VA005-divboxtarget-design').find('label')).text(value);
            //atttextonactive.css("color","black");
            //$($attributedivleftdragdrop.find('label')).text(value);

            $($attributedivleftdragdrop.find('.VA005-divboxtarget-design').find('label')).css({ "word-break": "break-word", "line-height": "20px" });


            //            word-break:break-word;line-height:43px;font-weight:normal

            //$($attributedivleftdragdrop.find('.VA005-divboxtarget-design').find('label')).text(value);
            //loadTreeData();
            //$($attributedivleftdragdrop.find('.VA005-divboxtarget-design').css({ "word-break": "break-word", "line-height": "80px" }));
            //word-break:break-word;line-height:80px

        };

        //*** Left Div Text Color change, click on mid-div active checkbox click 
        this.updatetextcolor = function (value, isActive) {
            debugger
            if (isActive) {
                $($attributedivleftdragdrop.find('.VA005-divboxtarget-design').find('label')).text(value);
                atttextonactive = $($attributedivleftdragdrop.find('.VA005-divboxtarget-design').find('label')).text(value);
                atttextonactive.css("color", "black");
                $($($attributedivleftdragdrop.find('.VA005-divboxtarget-design').find('label')).parent()).data('isactive', 'Y');
            }
            else {
                $($attributedivleftdragdrop.find('.VA005-divboxtarget-design').find('label')).text(value);
                atttextonactive = $($attributedivleftdragdrop.find('.VA005-divboxtarget-design').find('label')).text(value);
                atttextonactive.css("color", "#AFA5A5");
                $($($attributedivleftdragdrop.find('.VA005-divboxtarget-design').find('label')).parent()).data('isactive', 'N');
            }
        }


        //*** delete multiple items....

        var getmultipleid = 0;

        function checkboxclickformultyselect(evt, ctr) {

            if (ctr.is(':checked')) {
                getmultipleid = ctr.attr("attid");
                selectedIds.push(getmultipleid);
            }
            else {
                //Remove From Array
                getmultipleid = ctr.attr("attid");

                selectedIds.splice(selectedIds.indexOf(getmultipleid), 1);
            }

            evt.stopPropagation();

            //$($attributedivleftdragdrop.children()).removeClass('VA005-divboxtarget-design');
            //$($attributedivleftdragdrop.find('.VA005-innerattrb').children('div')[0]).addClass('VA005-divboxtarget-design');
        };


        function saveAttributeonaddplusbtn() {
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/AttributeListing/GetAttributeCount", "", AttributeCountAddCallBack);


            //var sql = "SELECT count(name) as Name FROM M_Attribute WHERE isactive='Y'";
            //sql = VIS.MRole.addAccessSQL(sql, "M_Attribute", true, true);
            //var ds = VIS.DB.executeDataSet(sql, null, null);
            //findnamecount = ds.tables[0].rows[0].cells["name"] + 1;

            //var valuesendtoctrl = {
            //    name: VIS.Msg.getMsg("VA005_Attribute") + " " + findnamecount,
            //    isactivefield: true,
            //    mandatory: false,
            //    istanceattribute: true
            //    // name: findnamecount
            //    //description: VIS.Utility.encodeText($textdesc.val().toString().trim()),
            //    //attributetype: $cmbselect.val(),

            //    //searchkey: VIS.Utility.encodeText($textsearchdiv.val().toString().trim()),
            //    //secname: VIS.Utility.encodeText($textnamediv.val().toString().trim()),
            //    //ID: selectedAttributeID
            //};

            //$.ajax({
            //    url: VIS.Application.contextUrl + "Attribute/SaveAttributemodel",
            //    type: 'Post',
            //    async: false,
            //    datatype: "Json",
            //    data: valuesendtoctrl,
            //    success: function (data) {
            //        selectedAttributeID = JSON.parse(data);
            //        $self.getidfromdivboxesselect = selectedAttributeID + "_2_" + $self.windowNo;

            //    },
            //   error: function (data) {
            //        //alert(data)
            //    },
            //});
            // attributeAppendDiv();
            //attributeidgetonokclick = selectedAttributeID;
        };

        function AttributeCountAddCallBack(dr) {

            if (dr != null) {
                findnamecount = dr['value'] + 1;
            }

            var valuesendtoctrl = {
                name: VIS.Msg.getMsg("VA005_Attribute") + " " + findnamecount,
                isactivefield: true,
                mandatory: false,
                istanceattribute: true
                // name: findnamecount
                //description: VIS.Utility.encodeText($textdesc.val().toString().trim()),
                //attributetype: $cmbselect.val(),

                //searchkey: VIS.Utility.encodeText($textsearchdiv.val().toString().trim()),
                //secname: VIS.Utility.encodeText($textnamediv.val().toString().trim()),
                //ID: selectedAttributeID
            };
            $.ajax({
                url: VIS.Application.contextUrl + "Attribute/SaveAttributemodel",
                type: 'Post',
                async: false,
                datatype: "Json",
                data: valuesendtoctrl,
                success: function (data) {
                    selectedAttributeID = JSON.parse(data);
                    $self.getidfromdivboxesselect = selectedAttributeID + "_2_" + $self.windowNo;

                },
                error: function (data) {
                    //alert(data)
                },
            });
        }


        this.attributevallueonattributeclick = function (selectedAttributeID) {
            debugger;
            if ($attributeobject.$cmbovaluecheck == 'L') {



                $attributevaluedivleftdragdrop.empty();
                //var sql = "SELECT M_ATTRIBUTEVALUE_ID, name, M_ATTRIBUTE_ID FROM m_attributevalue where M_ATTRIBUTE_ID=" + selectedAttributeID;
                // Added "ORDER BY CLAUSE" by Shifali on 16th July 2020

                VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/AttributeListing/GetAttributeValue", { "SelectAttributeID": selectedAttributeID }, AttributeValueOnClickCallBack);

                //var sql = "SELECT M_ATTRIBUTEVALUE_ID, name, M_ATTRIBUTE_ID FROM m_attributevalue WHERE M_ATTRIBUTE_ID=" + selectedAttributeID + " ORDER BY M_ATTRIBUTEVALUE_ID ";

                // sql = VIS.MRole.addAccessSQL(sql, "M_AttributeValue", true, true);
                // var ds = VIS.DB.executeDataSet(sql, null, null);


                //var ds = VIS.DB.executeDataSet(sql, null, null);

                //if (ds != null) {
                //    var str = '';
                //    for (var i = 0; i < ds.tables[0].rows.length; i++) {
                //     str += "<div data-id='" + VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells["m_attributevalue_id"]) + "' class='VA005-attributediv  VA005-divboxtarget-design-hover'>"
                //           //+ "<label class='VA005-textoverfllowhidden1' title='" + VIS.Utility.encodeText(ds.tables[0].rows[i].cells["name"]) + "'>" + VIS.Utility.encodeText(ds.tables[0].rows[i].cells["name"]) + "</label>"
                //            + "<label class='VA005-textoverfllowhidden2' title='" + VIS.Utility.encodeText(ds.tables[0].rows[i].cells["name"]) + "'>" + VIS.Utility.encodeText(ds.tables[0].rows[i].cells["name"]) + "</label>"
                //            + "</div>";
                //    }
                //    $attributevaluedivleftdragdrop.append($(str));

                //    // $attributevaluedivleftdragdrop.find('.VA005-attributediv').on("click", attributeValueDivClicked);
                //    //$attributevaluedivleftdragdrop.find('.VA005-attributediv').draggable({
                //    //    revert: "invalid",
                //    //    helper: "clone",
                //    //    containment: $root,
                //    //});

                //    //$divid.droppable({
                //    //    drop: function (event, ui) {
                //    //         
                //    //        attributeidbydivboxes = ($(ui.draggable)).data('id');
                //    //        attributesetidbydivboxes = ($(($($(this).find('.k-state-hover').parents('li')[0])).find('a')[0])).data('nodeid');

                //    //        saveAttribute(attributeidbydivboxes, attributesetidbydivboxes);
                //    //        loadTreeData();
                //    //    }
                //    //});
                // }
            }
        }

        function AttributeValueOnClickCallBack(dr) {
            var str = '';
            if (dr.length > 0) {
                for (var i = 0; i < dr.length; i++) {
                    str += "<div data-id='" + VIS.Utility.Util.getValueOfInt(dr[i].m_attributevalue_id) + "' class='VA005-attributediv  VA005-divboxtarget-design-hover'>"
                        //+ "<label class='VA005-textoverfllowhidden1' title='" + VIS.Utility.encodeText(ds.tables[0].rows[i].cells["name"]) + "'>" + VIS.Utility.encodeText(ds.tables[0].rows[i].cells["name"]) + "</label>"
                        + "<label class='VA005-textoverfllowhidden2' title='" + VIS.Utility.encodeText(dr[i].name) + "'>" + VIS.Utility.encodeText(dr[i].name) + "</label>"
                        + "</div>";
                }
            }
            $attributevaluedivleftdragdrop.append($(str));
        };



        //this.attributevallueonattributeclick = function (selectedAttributeID) {

        //    $attributevaluedivleftdragdrop.empty();
        //    var sql = "SELECT M_ATTRIBUTEVALUE_ID, name, M_ATTRIBUTE_ID FROM m_attributevalue where M_ATTRIBUTE_ID=" + selectedAttributeID;
        //    VIS.DB.executeDataSet(sql, null, attrClickCallBack);
        //}
        //function attrClickCallBack(ds)
        //{
        //    if (ds != null)
        //    {

        //        var str = '';
        //        for (var i = 0; i < ds.tables[0].rows.length; i++) {
        //            str += "<div data-id='" + VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells["m_attributevalue_id"]) + "' class='VA005-attributediv  VA005-divboxtarget-design-hover'>"
        //                            //+ "<label class='VA005-textoverfllowhidden1' title='" + VIS.Utility.encodeText(ds.tables[0].rows[i].cells["name"]) + "'>" + VIS.Utility.encodeText(ds.tables[0].rows[i].cells["name"]) + "</label>"
        //                            + "<label class='VA005-textoverfllowhidden2' title='" + VIS.Utility.encodeText(ds.tables[0].rows[i].cells["name"]) + "'>" + VIS.Utility.encodeText(ds.tables[0].rows[i].cells["name"]) + "</label>"
        //                    + "</div>";
        //        }
        //        $attributevaluedivleftdragdrop.append($(str));

        //        // $attributevaluedivleftdragdrop.find('.VA005-attributediv').on("click", attributeValueDivClicked);
        //        //$attributevaluedivleftdragdrop.find('.VA005-attributediv').draggable({
        //        //    revert: "invalid",
        //        //    helper: "clone",
        //        //    containment: $root,
        //        //});

        //        //$divid.droppable({
        //        //    drop: function (event, ui) {
        //        //         
        //        //        attributeidbydivboxes = ($(ui.draggable)).data('id');
        //        //        attributesetidbydivboxes = ($(($($(this).find('.k-state-hover').parents('li')[0])).find('a')[0])).data('nodeid');

        //        //        saveAttribute(attributeidbydivboxes, attributesetidbydivboxes);
        //        //        loadTreeData();
        //        //    }
        //        //});
        //    }
        //};





        //var $setcss = null;
        //*** add or remove class of divboxes on click...

        function attributeDivClicked(e) {
            debugger;
            //var target = $(e.target);
            e.stopPropagation();
            $self.isSaveOnItemFocus = false;
            if ($attributeobject.$getvalueonchange == true) {
                //Done by Shifali on 16th July 2020 (JID_1861)
                // var msg = "" + VIS.Msg.getMsg("VA005_SaveAttributeIt") + "";
                //var r = VIS.ADialog.ask(msg);
                VIS.ADialog.confirm("VA005_SaveAttributeIt", true, "", "Confirm", function (result) {
                    if (result == true) {
                        $self.isSaveOnItemFocus = true;
                        $attributeobject.onchangesaveattribute();
                        $attributeobject.cmboonclick();
                        $self.attributevallueonattributeclick(selectedAttributeID);
                        // gettargettext.val($attributeobject.$textnametextvalue);

                        //$($attributedivleftdragdrop.find('label')).text($attributeobject.$textnametextvalue);
                        //var text = $attributeobject.$textnametextvalue;

                        if (!textattribute.is('label')) {
                            if (textattribute.hasClass('VA005-attributediv')) {
                                textattribute = textattribute.find('label');
                            }
                            else {
                                textattribute = textattribute.siblings()[0];
                            }
                        }

                        textattribute.text($attributeobject.$textnametextvalue);

                        textattribute.addClass("VA005-textoverfllowhidden");

                        loadTreeData();

                        //if ($attributeobject.$activechkbox)
                        if ($attributeobject.$activechkbox) {
                            textattribute.css("color", "black");
                            //textattribute.css("color", "red")
                        }
                        else {
                            textattribute.css("color", "#AFA5A5");
                            //textattribute.css("color", "black");
                        }
                    }
                    else {
                        $attributeobject.$getvalueonchange = false;
                    }
                });

            }


            $delattributeboxes.removeClass("VA005-disabled");

            var target = $(e.target);
            textattribute = target;

            if (target.hasClass('VA005-checkboxonleftdiv')) {
                //===========
                target.prop("disabled", true);
                window.setTimeout(function () {

                    $editattributeboxes.removeClass("VA005-disabled");
                    selectedAttributeID = VIS.Utility.Util.getValueOfInt(target.attr("attid"));

                    getidfromattboxexforaddnode = selectedAttributeID;

                    if (target.prop("checked")) {
                        selectedIds.push(selectedAttributeID);
                        target.parent().parent().addClass('VA005-divboxtarget-design');
                        if (selectedIds.length == 1) {
                            objectcall();
                            $attributeobject.cmboonclick();
                            $self.attributevallueonattributeclick(selectedAttributeID);
                            $editattributeboxes.removeClass("VA005-disabled");
                            $delattributeboxes.removeClass("VA005-disabled");
                        }
                        else {
                            $attributevaluedivleftdragdrop.empty();
                        }
                    }
                    else {
                        if (selectedIds.length == 1) {
                            $editattributeboxes.removeClass("VA005-disabled");
                            $delattributeboxes.removeClass("VA005-disabled");
                        }

                        selectedIds.splice(selectedIds.indexOf(selectedAttributeID), 1);
                        target.parent().parent().removeClass('VA005-divboxtarget-design');
                        getidafterremovecheck = selectedIds;
                        //=======
                        if (getidafterremovecheck.length == 1) {
                            //selectedAttributeID = getidafterremovecheck;
                            selectedAttributeID = selectedIds[0];
                            objectcall();
                            $attributeobject.cmboonclick();
                            $self.attributevallueonattributeclick(getidafterremovecheck);
                            //selectedAttributeID = getidafterremovecheck;
                            //$attributeobject.cmboonclick();
                            //objectcall();
                        }
                    }

                    if (selectedIds.length == 0 || selectedIds.length > 1) {

                        $attributevaluedivleftdragdrop.empty();
                        //$editbtnatthideshow.css("display", "inherit");
                        //$middivforattributevaluehideshow.css("display", "inherit");
                        // $attributeobject.cleartext();
                        $editattributeboxes.addClass("VA005-disabled");

                        if ($middivforattributevalue.is(':visible')) {
                            $leftboxdivwidth.animate({ width: "50%" });
                            $middivforattributevalue.animate({ width: "30%" });
                            $middivforattributevalue.css({ "display": "none", 'width': '0%' });
                            $treeView.animate({ width: "50%" });
                            $treeView.css("display", "inherit");
                            $openattdialog.animate({ width: "50%" });
                            $attributeobject.cleartext();
                        }
                    }


                    $($attributedivleftdragdrop.children().find('label')).removeClass("VA005-textcolorchange-onedit");


                    target.prop("disabled", false);
                    // $(target.find('label')).addClass("VA005-textcolorchange-onedit");
                }, 200);

            }
            else if (target.is('label')) {
                selectedIds = [];
                target = target.parent();
                selectedAttributeID = target.data('id');

                getidfromattboxexforaddnode = selectedAttributeID;

                selectedIds.push(selectedAttributeID);

                $($attributedivleftdragdrop.children()).removeClass('VA005-divboxtarget-design');
                //$($attributevaluedivleftdragdrop.children()).removeClass('VA005-divboxtarget-design');
                $attributedivleftdragdrop.find('.VA005-divboxtarget-design').removeClass('VA005-divboxtarget-design');

                target.addClass('VA005-divboxtarget-design');
                //$setcss = target.addClass('VA005-divboxtarget-design');
                objectcall();
                $attributeobject.cmboonclick();

                // $undoattributeboxes.css("display", "none");
                // $editattributeboxes.css("display", "inherit");


                $self.attributevallueonattributeclick(selectedAttributeID);

                $($attributedivleftdragdrop.children()).find('div .VA005-checkboxonleftdiv').removeAttr("checked");
                target.find('div .VA005-checkboxonleftdiv').prop("checked", "true");


                //$editbtnatthideshow.css("display", "none");
                //$middivforattributevaluehideshow.css("display", "none");

                $editattributeboxes.removeClass("VA005-disabled");

                $($attributedivleftdragdrop.children().find('label')).removeClass("VA005-textcolorchange-onedit");
                //$(target.find('label')).addClass("VA005-textcolorchange-onedit");
                //$attributedivleftdragdrop.find('.VA005-checkboxonleftdiv').prop("checked", "false");


            }
            else if (target.hasClass("VA005-attributediv")) {
                selectedIds = [];
                selectedAttributeID = target.data('id');

                getidfromattboxexforaddnode = selectedAttributeID;

                selectedIds.push(selectedAttributeID);

                $($attributedivleftdragdrop.children()).removeClass('VA005-divboxtarget-design');
                //$($attributevaluedivleftdragdrop.children()).removeClass('VA005-divboxtarget-design');
                $attributedivleftdragdrop.find('.VA005-divboxtarget-design').removeClass('VA005-divboxtarget-design');

                target.addClass('VA005-divboxtarget-design');
                //$setcss = target.addClass('VA005-divboxtarget-design');

                objectcall();
                $attributeobject.cmboonclick();
                $self.attributevallueonattributeclick(selectedAttributeID);
                // $undoattributeboxes.css("display", "none");
                // $editattributeboxes.css("display", "inherit");





                $($attributedivleftdragdrop.children()).find('div .VA005-checkboxonleftdiv').removeAttr("checked");
                target.find('div .VA005-checkboxonleftdiv').prop("checked", "true");

                //$editbtnatthideshow.css("display", "none");
                // $middivforattributevaluehideshow.css("display", "none");
                $editattributeboxes.removeClass("VA005-disabled");

                $($attributedivleftdragdrop.children().find('label')).removeClass("VA005-textcolorchange-onedit");
                //$(target.find('label')).addClass("VA005-textcolorchange-onedit");


            }
            else if (target.parent().hasClass("VA005-attributediv")) {
                selectedIds = [];
                target = target.parent();
                selectedAttributeID = target.data('id');

                getidfromattboxexforaddnode = selectedAttributeID;

                selectedIds.push(selectedAttributeID);

                $($attributedivleftdragdrop.children()).removeClass('VA005-divboxtarget-design');
                //$($attributevaluedivleftdragdrop.children()).removeClass('VA005-divboxtarget-design');
                $attributedivleftdragdrop.find('.VA005-divboxtarget-design').removeClass('VA005-divboxtarget-design');

                target.addClass('VA005-divboxtarget-design');
                //$setcss = target.addClass('VA005-divboxtarget-design');
                objectcall();
                $attributeobject.cmboonclick();
                $self.attributevallueonattributeclick(selectedAttributeID);
                // $undoattributeboxes.css("display", "none");
                // $editattributeboxes.css("display", "inherit");




                $($attributedivleftdragdrop.children()).find('div .VA005-checkboxonleftdiv').removeAttr("checked");
                target.find('div .VA005-checkboxonleftdiv').prop("checked", "true");
                // $editbtnatthideshow.css("display", "none");
                //$middivforattributevaluehideshow.css("display", "none");
                $editattributeboxes.removeClass("VA005-disabled");

                $($attributedivleftdragdrop.children().find('label')).removeClass("VA005-textcolorchange-onedit");
                // $(target.find('label')).addClass("VA005-textcolorchange-onedit");


            }
            $attributeobject.$getvalueonchange = false;

        };










        ///**8 Left div work...
        function attributeValueDivClicked(e) {

            var target = $(e.target);
            if (target.is('label')) {
                target = target.parent();
            }
            //var id = target.data('id');
            selectedAttributeID = target.data('id');

            $($attributevaluedivleftdragdrop.children()).removeClass('VA005-divboxtarget-design');
            $($attributevaluedivleftdragdrop.children()).removeClass('VA005-divboxtarget-design');


            target.addClass('VA005-divboxtarget-design');


            //$undoattributeboxes.css("display", "none");
            //$editattributeboxes.css("display", "inherit");
        };

        //function DragDropDocument()
        //{
        //     
        //    //$attributedivleftdragdrop.find(".VA005-innerattrb").draggable({
        //    //selectedAttributeID.draggable({
        //    //$root.find(".VA005_maindivleft .VA005-attributediv").draggable({
        //    selectedAttributeID.draggable({
        //        revert: "invalid", // when not dropped, the item will revert back to its initial position
        //        containment: "document",
        //        helper: "clone",
        //        cursor: 'move',
        //        start: function (event, ui) {
        //            ui.helper.animate({
        //                width: 80,
        //                height: 50
        //            });
        //        }
        //    });
        //    //$root.find(".vis-group-users-container .vis-group-user-wrap").droppable({
        //    //    drop: function (event, ui) {
        //    //        $(this).append($(ui.draggable).attr("path"));
        //    //        SaveProductImage($(ui.draggable).attr("path"), $(ui.draggable).attr("filename"), parseInt($(this).attr("data-uid")));
        //    //         
        //    //    }
        //    //});            
        //};


        //*** attributevalue box show in left box..
        //function attributevalueAppendDiv()
        //{
        //     
        //    $attributevaluedivleftdragdrop.empty();
        //    var sql = "SELECT NAME, M_AttributeValue_ID FROM M_AttributeValue WHERE ISACTIVE='Y'";  
        //    sql = VIS.MRole.addAccessSQL(sql, "M_AttributeValue", true, true);
        //    var ds = VIS.DB.executeDataSet(sql, null, null);
        //    if (ds != null)
        //    {
        //        for (var i = 0; i < ds.tables[0].rows.length; i++) {
        //            //var str = "<div data-id='" + VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells["m_attributevalue_id"]) + "' style='height:65px;width:95px;margin:0px 10px 20px 0px;float:left;border:3px solid #ccc;padding:20px'>"
        //            var str = "<div data-id='" + VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[i].cells["m_attributevalue_id"]) + "' class='VA005-attributediv  VA005-divboxtarget-design-hover'>"
        //                            + "<label>" + ds.tables[0].rows[i].cells["name"] + "</label>"
        //                    + "</div>";
        //            $attributevaluedivleftdragdrop.append($(str));
        //        }
        //        $attributevaluedivleftdragdrop.find('.VA005-attributediv').on("click", attributeDivClicked);
        //    }
        //}


        ///****designe Open first attributeset dialog...
        function openAddAtrributeDialog() {
            $openattdialog = $("<div  class='VA005-left-wrap-div-openattributre' style='display:none;'>");
            $opendialog = "<div class='VA005-att-tree-vieww VA005-top-wrap-treedesign'><h4>" + VIS.Msg.getMsg("VA005_AddAttributeSet") + "</h4></div>"

                + "<div style='float: left; width: 100%;'>"
                + "<table class='VA005-att-righttable mt-2'>"
                //+ "<tr>"
                //+ "<td colspan='2'>" + VIS.Msg.getMsg("Name") + " </td>"
                //+ "</tr>"
                + "<tr>"
                + "<td><div class='input-group vis-input-wrap'><div class='vis-control-wrap'><input class='vis-ev-col-mandatory' id='" + $self.windowNo + "txtname' type='text' placeholder=' ' data-placeholder=''><label>" + VIS.Msg.getMsg("Name") + "</label></div></div></td>"
                + "</tr>"
                //+ "<tr>"
                //+ "<td>" + VIS.Msg.getMsg("Description") + "</td>"
                //+ "</tr>"
                + "<tr>"
                + "<td><div class='input-group vis-input-wrap'><div class='vis-control-wrap'><input id='" + $self.windowNo + "txtdes'  type='text' placeholder=' ' data-placeholder='' ><label>" + VIS.Msg.getMsg("Description") + "</label></div></div></td>"
                + "</tr>"
                //+ "<tr>"
                //+ "<td>" + VIS.Msg.getMsg("VA005_MandatoryType") + "</td>"
                //+ "</tr>"
                + "<tr>"
                + "<td><div class='input-group vis-input-wrap'><div class='vis-control-wrap'><select id='" + $self.windowNo + "mandatorysec'></select><label>" + VIS.Msg.getMsg("VA005_MandatoryType") + "</label></div></div></td>"
                + "</tr>"
                //+ "<tr>"
                //+ "<td  style='width:110px'></td>"
                //+ "</tr>"
                //+ "<div style='margin-top: 5px;'></div>"
                + "<tr>"
                //+ "<td><input style='margin:0 0 10px 0' type='checkbox'  id='" + $self.windowNo + "expirydate' />" + VIS.Msg.getMsg("VA005_ExpiryDate") + "</td>" + "<td><input style='margin:0' type='checkbox' disabled id='" + $self.windowNo + "mandatoryexpdate' />" + VIS.Msg.getMsg("VA005_MandatoryExpiryDate") + "</td>"

                + "<td><div style='width: 50%; float: left' class='input-group vis-input-wrap'><div class='vis-control-wrap'><label class='vis-ec-col-lblchkbox'><input type='checkbox'  id='" + $self.windowNo + "expirydate' />" + VIS.Msg.getMsg("VA005_ExpiryDate") + "</label></div></div>"
                + "<div style='width: 50%; float: left' class='input-group vis-input-wrap'><div class='vis-control-wrap'><label class='vis-ec-col-lblchkbox'><input type='checkbox' disabled id='" + $self.windowNo + "mandatoryexpdate' />" + VIS.Msg.getMsg("VA005_MandatoryExpiryDate") + "</label></div></div></td>"
                + "</tr>"
                //+ "<tr>"
                //+ "<td style='width:110px'></td>"
                //+ "</tr>"
                //+ "<div style='margin-top: 20px;'></div>"
                + "<tr>"
                //+ "<td><input style='margin:5px 0 10px 0' id='" + $self.windowNo + "lot'  type='radio' name='ab' checked='checked' /><span style='margin-left: 8px;'>" + VIS.Msg.getMsg("VA005_Lot") + "</span></td>" + "<td><input style='margin:5px 0 10px 0' id='" + $self.windowNo + "serial' type='radio' name='ab' /><span style='margin-left: 8px;'>" + VIS.Msg.getMsg("VA005_Serial") + "</span></td>"
                + "<td><div style='width: 50%; float: left' class='input-group vis-input-wrap'><div class='vis-control-wrap'><label class='vis-ec-col-lblchkbox'><input id='" + $self.windowNo + "lot'  type='checkbox'>" + VIS.Msg.getMsg("VA005_Lot") + "</label></div></div>"
                + "<div style='width: 50%; float: left' class='input-group vis-input-wrap'><div class='vis-control-wrap'><label class='vis-ec-col-lblchkbox'><input id='" + $self.windowNo + "serial' type='checkbox' >" + VIS.Msg.getMsg("VA005_Serial") + "</label></div></td>"
                + "</tr>"
                //+ "<tr>"
                //+ "<td><label id='" + $self.windowNo + "lotandserialname'>" + VIS.Msg.getMsg("VA005_Lot&Serial") + "</label></td>"
                //+ "</tr>"
                //+ "<div style='margin-top: 15px;'></div>"
                + "<tr>"
                + "<td><div class='input-group vis-input-wrap'><div class='vis-control-wrap'><select class='VA005-selectattsetcmb' id='" + $self.windowNo + "lotandserialdatadropdown' data-hasbtn=' '></select><label id='" + $self.windowNo + "lotandserialname'>" + VIS.Msg.getMsg("VA005_Lot&Serial") + "</label></div> <div class='input-group-append'><span title='" + VIS.Msg.getMsg("VA005_Edit") + "'  id='" + $self.windowNo + "editbtn' class='input-group-text vis vis-edit'></span>   <span title='" + VIS.Msg.getMsg("VA005_AddLotSerial") + "'    class='input-group-text vis vis-plus'  id='" + $self.windowNo + "addbtn'></span>        </div></div>   </td>"
                + "</tr>"
                //+ "<tr>"
                //+ "<td style='width:70px'></td>"
                //+ "</tr>"
                + "<tr>"
                //+ "<td colspan='2'><input id='" + $self.windowNo + "okbtnattributeset' class='ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' style='margin:14px 0px 0px 62.5%' type='button'  value='OK'/><input id='" + $self.windowNo + "cancelbtnattributeset' class='ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' style='margin:12px 0px 0px 8px' type='button'  value='Cancel'/></td>"
                + "<td><div class='d-flex justify-content-end'><input id='" + $self.windowNo + "okbtnattributeset' class='mr-3 ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' type='button'  value='" + VIS.Msg.getMsg("OK") + "'/><input id='" + $self.windowNo + "cancelbtnattributeset' class='ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' type='button'  value='" + VIS.Msg.getMsg("Cancel") + "'/></div></td>"
                + "</tr>"
                + "</table>"
                + "</div>"

            $openattdialog.append($opendialog);

            //*** Append attributeset design in first design...
            $tree.append($openattdialog);

            $addatttextname = $openattdialog.find("#" + $self.windowNo + "txtname");
            $$addatttextdes = $openattdialog.find("#" + $self.windowNo + "txtdes");
            $cmbattsectmandtype = $openattdialog.find("#" + $self.windowNo + "mandatorysec");

            //*** Expiry date check box on attributeset dialog
            $addatttextexpdate = $openattdialog.find("#" + $self.windowNo + "expirydate");

            //*** Mandatory Expiry date check box on attributeset dialog
            $mandatoryexpdate = $openattdialog.find("#" + $self.windowNo + "mandatoryexpdate");

            //*** lot redio btn..
            $addatttextlot = $openattdialog.find("#" + $self.windowNo + "lot");
            $addatttextserial = $openattdialog.find("#" + $self.windowNo + "serial");

            //*** lot and serial lable ..
            $addatttextlotserialtext = $openattdialog.find("#" + $self.windowNo + "lotandserialname");

            //*** Comb box for lot and serial data..
            $cmbSerialDropdown = $openattdialog.find("#" + $self.windowNo + "lotandserialdatadropdown");

            //*** Add New Button on attributeset dialog form for lot and serial data...
            $addattaddbtn = $openattdialog.find("#" + $self.windowNo + "addbtn");

            ////*** attributeset dialog, on second div..
            //$saveattributesetdata = $openattdialog.find("#" + $self.windowNo + "saveattributesetdata");

            //*** hideshow div of btn near lotserial dropdata...
            // $hideshowbtndiv = $openattdialog.find("#" + $self.windowNo + "hideshowbtndiv");

            //*** second div id for toggel..
            $divhideandshow = $openattdialog.find("#" + $self.windowNo + "divhideandshow");

            $editbtnoflotserialwindow = $openattdialog.find("#" + $self.windowNo + "editbtn");

            //*** attributeset ok button...
            okbtnattributeset = $openattdialog.find("#" + $self.windowNo + "okbtnattributeset");

            //*** attributeset cancel button...
            cancelbtnattributeset = $openattdialog.find("#" + $self.windowNo + "cancelbtnattributeset");

            //*** attributeset dialog event handelling..
            dialogAttributeSetventHandle();
        };

        //*** lot serial dialog disign.....
        function lotserialDesign() {

            lotserialdialog = $('<div>');
            lotserialdialogobject = "<div id='" + $self.windowNo + "divhideandshow' style='height:auto;width:100%;'>"
                + "<table style='width:100%'>"
                //+ "<tr>"
                //+ "<td>" + VIS.Msg.getMsg("Name") + "</td>"
                //+ "</tr>"
                + "<tr>"
                + "<td><div class='input-group vis-input-wrap'><div class='vis-control-wrap'><input class='vis-ev-col-mandatory' id='" + $self.windowNo + "divnametxt' type='text' ><label>" + VIS.Msg.getMsg("Name") + "</label></div></div></td>"
                + "</tr>"
                //+ "<tr>"
                //+ "<td>" + VIS.Msg.getMsg("VA005_StartNo") + "</td>"
                //+ "</tr>"
                + "<tr>"
                + "<td><div class='input-group vis-input-wrap'><div class='vis-control-wrap'><input id='" + $self.windowNo + "divstarttext' type='number' value='100' /><label>" + VIS.Msg.getMsg("VA005_StartNo") + "</label></div></div></td>"
                + "</tr>"
                //+ "<tr>"
                //+ "<td>" + VIS.Msg.getMsg("VA005_CurrentNext") + "</td>"
                //+ "</tr>"
                + "<tr>"
                + "<td><div class='input-group vis-input-wrap'><div class='vis-control-wrap'><input id='" + $self.windowNo + "divcurrenttext' type='number'  value='100' /><label>" + VIS.Msg.getMsg("VA005_CurrentNext") + "</label></div></div></td>"
                + "</tr>"
                //+ "<tr>"
                //+ "<td>" + VIS.Msg.getMsg("VA005_Increment") + "</td>"
                //+ "</tr>"
                + "<tr>"
                + "<td><div class='input-group vis-input-wrap'><div class='vis-control-wrap'><input id='" + $self.windowNo + "incrementtext' type='number'  value='1' /><label>" + VIS.Msg.getMsg("VA005_Increment") + "</label></div></div></td>"
                + "</tr>"
                //+ "<tr>"
                //+ "<td>" + VIS.Msg.getMsg("VA005_Prefix") + "</td>"
                //+ "</tr>"
                + "<tr>"
                + "<td><div class='input-group vis-input-wrap'><div class='vis-control-wrap'><input id='" + $self.windowNo + "prefixtxt' type='text' /><label>" + VIS.Msg.getMsg("VA005_Prefix") + "</label></div></div></td>"
                + "</tr>"
                //+ "<tr>"
                //+ "<td>" + VIS.Msg.getMsg("VA005_Suffix") + "</td>"
                //+ "</tr>"
                + "<tr>"
                + "<td><div class='input-group vis-input-wrap'><div class='vis-control-wrap'><input id='" + $self.windowNo + "serfixtxt' type='text' /><label>" + VIS.Msg.getMsg("VA005_Suffix") + "</label></div></div></td>"
                + "</tr>"
                + "</table>"
                + "</div>"
            lotserialdialog.append(lotserialdialogobject);

            $divnametxt = lotserialdialog.find("#" + $self.windowNo + "divnametxt");
            $divstarttxt = lotserialdialog.find("#" + $self.windowNo + "divstarttext");
            $divcurrenttxt = lotserialdialog.find("#" + $self.windowNo + "divcurrenttext");
            $incrementtext = lotserialdialog.find("#" + $self.windowNo + "incrementtext");
            $prefixtext = lotserialdialog.find("#" + $self.windowNo + "prefixtxt");
            $serfixtext = lotserialdialog.find("#" + $self.windowNo + "serfixtxt");


        }




        //*** lot and serial dialog..
        function dialodforlotserial() {
            //Dialog Design..
            lotserialDesign();
            eventslotserialdialog();
            var createTab = new VIS.ChildDialog();
            createTab.setHeight(450);
            createTab.setWidth(450);
            createTab.setEnableResize(false);

            if ($addatttextlot.prop("checked")) {
                createTab.setTitle(VIS.Msg.getMsg('VA005_AddLot'));
            }
            else {
                createTab.setTitle(VIS.Msg.getMsg('VA005_AddSerial'));
            }
            createTab.setModal(true);
            createTab.setContent(lotserialdialog);
            createTab.show();
            createTab.onClose = function () {
                clearDivtext();
            };
            createTab.onOkClick = function (e) {

                if ($divnametxt.val().trim().length > 0) {
                    if ($addatttextlot.prop("checked")) {
                        SaveLotData();
                        lotDataDropdown();
                    }
                    if ($addatttextserial.prop("checked")) {
                        saveSerialData();
                        serialDatadDopdown();
                    }
                }
                else {
                    VIS.ADialog.info("VA005_NameFieldRequired");
                    return false;
                }
                clearDivtext();
            };
            createTab.onCancelClick = function () {
                clearDivtext();
            };
        }


        //*** get attribute design from another js file..
        function getAttributeRoot() {

            /// create object of attribute form.. and send treeview for design..
            $attributeobject = new VA005.AForms.attribute($treeView, attributeAppendDiv, $middivforattributevalue, $leftboxdivwidth, $delattributeboxes, $editattributeboxes, flag, $attributevaluedivleftdragdrop, $senddivatt, $self, $bsyDiv);
            $attributeRoot = $attributeobject.getRoot();
            $tree.append($attributeRoot);



            $attributeRoot2 = $attributeobject.getRoot2();
            $middivforattributevalue.append($attributeRoot2);
            $attributeRoot.css("display", "none");
        };

        //*** Create object for attribute form..
        function objectcall() {

            $attributeobject.editAttribute(selectedAttributeID, loadTreeData);
            $attributeRoot.css("display", "inherit");
        };


        function deleteattributefromdata() {
            var NameList = [];

            $.ajax({
                url: VIS.Application.contextUrl + "AttributeListing/DeleteAttributeFromData",
                type: 'Post',
                async: false,
                datatype: "Json",
                contentType: 'application/json',
                //cache: false,
                data: JSON.stringify({ values: selectedIds }),
                success: function (data) {
                    $attributevaluedivleftdragdrop.empty();
                    // $divLeftTree.data("kendoTreeView").remove(selectedNode.closest(".k-item"));
                    //grdRows.splice(grdRows.indexOf(row), 1);]
                    // Added by shifali on 27th July 2020 to display name of attributes which are not deleted
                    if (data.value.length > 0) {
                        for (var i = 0; i < data.value.length; i++) {
                            if (data.value[i].Name != "") {
                                NameList.push(data.value[i].Name);
                            }
                        }
                    }
                    if (NameList != "") {
                        VIS.ADialog.error("VA005_AttributeCategory", true, NameList.join(", "));
                    }
                    //if (data.value != "") {
                    //    alert(data.value);
                    //}
                },
                error: function (data) {
                    if (data.value != null) {
                        //alert(data.value);
                        VIS.ADialog.error(data.value);
                    }
                },
            });
        }


        var eventHandelling = function () {

            if (selectedIds.length == 0) {
                $delattributeboxes.addClass("VA005-disabled");
            }

            $editattributeboxes.addClass("VA005-disabled");

            $delattributeboxes.on("click", function () {

                $bsyDiv[0].style.visibility = "visible";

                //if (sqlattribute != 0 && sqlattribute != null) {

                //if (r == true) {
                //if (selectedIds.length > 0) {
                if (selectedIds.length > 0) {
                    //var msg = "" + VIS.Msg.getMsg("VA005_DeleteIt") + "";

                    //var r = VIS.ADialog.ask("VA005_DeleteIt");
                    VIS.ADialog.confirm("VA005_DeleteIt", true, "", "Confirm", function (result) {
                        if (result == true) {
                            var strIds = "";
                            for (var i = 0; i < selectedIds.length; i++) {
                                if (i == 0) {
                                    strIds += selectedIds[i];
                                }
                                else {
                                    strIds += "," + selectedIds[i];
                                }
                            }
                            //var sqlattribute = "delete from m_attribute where isactive='Y' and m_attribute_id IN  (" + strIds + ")";
                            //VIS.DB.executeQuery(sqlattribute, null, null);
                            deleteattributefromdata();
                            loadTreeData();
                            //// $divLeftTree.data("kendoTreeView").remove(selectedNode.closest(".k-item"));
                            attributeAppendDiv();
                            $bsyDiv[0].style.visibility = "hidden";
                            $delattributeboxes.addClass("VA005-disabled");

                            $attributeobject.cleartext();
                            selectedIds = [];
                            $editattributeboxes.addClass("VA005-disabled");
                        }
                        else {
                            $delattributeboxes.removeClass("VA005-disabled");
                        }
                    });
                }
                else {
                    //VIS.ADialog.info("VA005_SelectAttributeBoxesFirst");
                }

                $bsyDiv[0].style.visibility = "hidden";
            })



            //$splitter.resizable({
            //    maxheight: 20,
            //    minwidth: 150
            //});$leftboxbackdivforspliter

            if ($addatttextlot.is(":checked")) {
                //createTab.setTitle(VIS.Msg.getMsg('VA005_AddLot'));
                $addatttextlotserialtext.text(VIS.Msg.getMsg('VA005_LotValue'));
                lotDataDropdown();
                // $hideshowbtndiv.hide();
            }
            else {
                //createTab.setTitle(VIS.Msg.getMsg('VA005_AddSerial'));
            }




            $leftboxbackdivforspliter.resizable({
                //maxheight: 20,
                //minwidth: 150
            });

            $leftboxbackdivforspliter.on("resize", function (evt, ui) {


                var currentheight = ui.size.height;
                //$attributedivleftdragdrop.css("height", currentheight);

                //$attributedivleftdragdrop.css("overflow-x", "none");
                //$attributedivleftdragdrop.css("overflow-y", "auto");

                //$attributedivleftdragdrop.css("width", "100%");
                $leftboxbackdivforspliter.css("width", "100%");



                var height = null;
                height = $leftboxdivwidth.height() - ($AttributeValuegetid.height() + 10);
                //$attributedivleftdragdrop.css("max-height", height);
                $leftboxbackdivforspliter.css("max-height", height);

                //$leftboxattvaluebelow.height($leftboxdivwidth.height() - ($leftboxbackdivforspliter.height() + 10));


                //$attributedivleftdragdrop.css("max-height", currentheight);               
                //$leftboxbackdivforspliter.css("max-height", currentheight);
                //$attributedivleftdragdrop.css("max-height", "350px");
                //$leftboxbackdivforspliter.css("max-height", "350px");

            });



            //$attributedivleftdragdrop.resizable({
            //    maxheight: 20,
            //    minwidth: 150
            //});

            //$attributedivleftdragdrop.on("resize", function (evt, ui)
            //{
            //     
            //    var currentheight = ui.size.height;

            //    var height = "120px";

            //    $(this).height(currentheight);

            //    // set the content panel width
            //    $attributedivleftdragdrop.height($attributedivleftdragdrop - currentheight - height);

            //});


            //$attributedivleftdragdrop.on("resize", function (evt){
            //     


            //});







            //$undoattributeboxes.on("click", function () {
            //    //$root.css("display", "none");
            //    $treeView.css("display", "inherit");
            //    $middivforattributevalue.animate({ width: "0%" });
            //    $middivforattributevalue.css("display", "none");
            //    $treeView.animate({ width: "50%" });

            //    $leftboxdivwidth.animate({ width: "50%" });
            //    //$root.animate({ width: "50%" });
            //    //attributeAppendDiv();
            //    $undoattributeboxes.css("display", "none");
            //    $editattributeboxes.css("display", "inherit");
            //});


            $editattributeboxes.on("click", function () {
                debugger;

                $editattvaluecompare = selectedAttributeID;

                //if (selectedAttributeID != null && selectedAttributeID != 0) {

                if ($attributeobject.$getvalueonchange == true) {
                    return;
                }

                if (selectedIds.length == 1) {
                    objectcall();
                    if (!$middivforattributevalue.is(':visible')) {
                        $leftboxdivwidth.animate({ width: "40%" });
                        $middivforattributevalue.animate({ width: "30%" });
                        $middivforattributevalue.css({ "display": "inherit", 'width': '30%' });
                        $treeView.animate({ width: "30%" });
                        $treeView.css("display", "inherit");
                        $openattdialog.animate({ width: "30%" });
                    }

                    //objectcall();
                    //$($attributedivleftdragdrop.find('.VA005-divboxtarget-design').find('label')).css("color", "darkgrey")

                    // $($attributedivleftdragdrop.find('.VA005-divboxtarget-design').find('label')).addClass("VA005-textcolorchange-onedit");
                    //                    $($attributedivleftdragdrop.children().find('label')).removeClass("VA005-textcolorchange-onedit");


                    $attributeobject.cmboonclick()
                    //VA005 - textcolorchange - onedit

                    //$openattdialog.css("display", "none");
                    //  $leftboxdivwidth.animate({ width: "50%" });
                    //$attributeRoot2.css("display", "inherit");
                    //$middivforattributevalue.animate({ width: "50%" });                       
                    //$treeView.animate({ width: "30%" });

                }
                //else {
                //    VIS.ADialog.info("VA005_SelectAttributeBoxesFirst");               
                //}

            });



            //*** Delete btn top of the attributeset listing tree
            $deleteAttribute.on("click", function () {

                // 
                //var selectedNode = $divLeftTree.data("kendoTreeView").select();
                //nodeID = $(selectedNode.find('.data-id ')).data('nodeid');



                //if (nodeID != null) {

                //    $bsyDiv[0].style.visibility = "visible";
                //    var msg = "" + VIS.Msg.getMsg("VA005_DeleteIt") + "";

                //    var r = VIS.ADialog.ask(msg);

                //    if (r == true) {
                deletefromtreedata();

                //        //$deletemsz.css("display", "inherit");

                //        // $deletemsz.fadeOut(5000);

                //    }
                //    else {
                //        $bsyDiv[0].style.visibility = "hidden";
                //        return;
                //    }
                //}
                //else {
                //    //==================///////////alert();
                //    VIS.ADialog.info("VA005_SelectField");
                //}

                //$bsyDiv[0].style.visibility = "hidden";
                // deletefromtreedata();
            });

            //*** Plus Btn top of the attributeset listing tree ..
            $openAttributeset.on("click", function () {

                nodeID = 0;
                $addatttextlot.attr("checked", false);
                $addatttextserial.attr("checked", false);
                $treeView.css("display", "none");
                //$hideshowbtndiv.show();
                //$hideshowbtndiv.fadeTo("fast", 0.33);
                $openattdialog.css("display", "inherit");
                $addatttextname.trigger("change");
                $addatttextname.val("");
                //$addatttextname.focus();

                $middivforattributevalue.animate({ width: "0%" });
                $middivforattributevalue.css({ "display": "none", 'width': '0%' });
                $leftboxdivwidth.animate({ width: "50%" });
                $openattdialog.animate({ width: "50%" });

                $attributevaluedivleftdragdrop.empty();

            });



            if (nodeID == null) {
                //*** Expiry date check box
                $addatttextexpdate.prop("checked", false);
            }

            if ($addatttextexpdate.attr("checked")) {
                $addatttextexpdate.trigger("click");
            }

            if ($addatttextlot.prop("checked")) {
                $addatttextlot.trigger("click");
            }
            if ($addatttextserial.prop("checked")) {
                $addatttextserial.trigger("click");
            }

            //Btn with lot and serialcombo
            $addattaddbtn.prop('disabled', true);
            $addatttextlot.off("off");
            $addatttextlot.on("click", function () {

                if ($addatttextlot.is(":checked")) {
                    $addatttextserial.prop("checked", false);
                    $editbtnoflotserialwindow.removeClass("VA005-disabled");
                    $addattaddbtn.removeClass("VA005-disabled");
                    lotDataDropdown();
                }
                else {
                    $editbtnoflotserialwindow.addClass("VA005-disabled");
                    $addattaddbtn.addClass("VA005-disabled");
                    $cmbSerialDropdown.empty();

                }
                // $hideshowbtndiv.hide();
                $addatttextlotserialtext.text(VIS.Msg.getMsg('VA005_LotValue'));
                //lotDataDropdown();
                //btnEnable();
            });

            $addatttextserial.on("click", function () {
                // $hideshowbtndiv.hide();
                $editbtnoflotserialwindow.removeClass("VA005-disabled");
                $addattaddbtn.removeClass("VA005-disabled");


                if ($addatttextserial.is(":checked")) {
                    $addatttextlot.prop("checked", false);
                    $editbtnoflotserialwindow.removeClass("VA005-disabled");
                    $addattaddbtn.removeClass("VA005-disabled");
                    serialDatadDopdown();
                }
                else {
                    $editbtnoflotserialwindow.addClass("VA005-disabled");
                    $addattaddbtn.addClass("VA005-disabled");
                    $cmbSerialDropdown.empty();
                }
                $addatttextlotserialtext.text(VIS.Msg.getMsg('VA005_SerialValue'));
                // $addatttextlot.prop("checked", false);
                //serialDatadDopdown();
                btnEnable();
            });

            //Btn with lot and serialcombo
            $addattaddbtn.on("click", function (evt) {
                if ($addatttextlot.is(":checked") || $addatttextserial.is(":checked")) {
                    evt.stopPropagation();
                    $divhideandshow.toggle();
                    dialodforlotserial();
                }
            });

            //*** Btn for second div of attributeset dialog
            //$saveattributesetdata.on("click", function ()
            //{
            //    if ($addatttextlot.prop("checked")) {
            //        if ($divnametxt.val().trim().length > 0) {
            //            SaveLotData();
            //            lotDataDropdown();
            //        }
            //    }
            //    if ($addatttextserial.prop("checked")) {
            //        ;
            //        if ($divnametxt.val().trim().length > 0) {
            //            saveSerialData();
            //            serialDatadDopdown();
            //        }
            //    }
            //    clearDivtext();
            //});

            //*** Expiry date check box
            $addatttextexpdate.on("click", function () {
                if ($addatttextexpdate.prop("checked")) {

                    $mandatoryexpdate.attr('disabled', false);
                }
                else {
                    // Mandatory checkbox..
                    $mandatoryexpdate.attr('disabled', true);
                    $mandatoryexpdate.attr('checked', false);
                }
            });

            //*** first attributeset dailog text btn
            $addatttextname.on("change", function () {
                if ($addatttextname.val().trim().length <= 0) {
                    //$addatttextname.css("background-color", "pink");
                    $addatttextname.addClass('vis-ev-col-mandatory');
                }
                else {
                    //$addatttextname.css("background-color", "white");
                    $addatttextname.removeClass('vis-ev-col-mandatory');
                }
            });



            okbtnattributeset.on("click", function () {

                if ($addatttextname.val().trim().length > 0) {
                    SaveAttributeSetValue();
                    cleartext();
                    $treeView.css("display", "inherit");
                    $openattdialog.css("display", "none");
                    $treeView.animate({ width: "50%" });
                    //flag = false;
                }
                else {
                    VIS.ADialog.info("VA005_NameFieldRequired");
                    //alert(VIS.Msg.getMsg("VA005_NameFieldRequired"));
                    //flag = false;
                    return false;
                }
                $addatttextname.trigger("change");

                loadTreeData();
                $addatttextname.trigger("change");
                $treeView.css("display", "inherit");
                $openattdialog.css("display", "none");
            });

            cancelbtnattributeset.on("click", function () {

                cleartext();
                //$addatttextlot.prop("checked", true);
                $addatttextname.trigger("change");
                $treeView.css("display", "inherit");
                $openattdialog.css("display", "none");
                $treeView.animate({ width: "50%" });
                $(".VA005-attributedivstatic").parent().attr("VA005_plusimg", 'N');
                attributeAppendDiv();
                selectedIds = [];
                $editattributeboxes.addClass("VA005-disabled");
                $delattributeboxes.addClass("VA005-disabled");
            });


            $editbtnoflotserialwindow.on("click", function () {


                if (lotidget != "null" || serialidget != "null") {
                    if ($addatttextlot.is(":checked")) {

                        var windowid = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/AttributeListing/GetWindow_ID", { "Control": 'Lot Control' });

                        //var windowid = VIS.Utility.Util.getValueOfInt(dr[i].ad_window_id);
                        //var sql1 = "select ad_window_id from ad_window where name='Lot Control'";
                        //var ds = VIS.DB.executeDataSet(sql1, null, null);

                        //var windowid = VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[0].cells["ad_window_id"]);


                        var zoomQuery = new VIS.Query();
                        zoomQuery.addRestriction("M_LotCtl_ID", VIS.Query.prototype.EQUAL, lotidget);
                        VIS.viewManager.startWindow(windowid, zoomQuery);
                    }

                    if ($addatttextserial.is(":checked")) {

                        var windowid = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/AttributeListing/GetWindow_ID", { "Control": 'Serial No Control' });

                        // var windowid = VIS.Utility.Util.getValueOfInt(dr["ad_window_id"]);
                        //var sql1 = "select ad_window_id from ad_window where name='Serial No Control'";
                        //var ds = VIS.DB.executeDataSet(sql1, null, null);

                        //var windowid = VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[0].cells["ad_window_id"]);

                        var zoomQuery = new VIS.Query();
                        zoomQuery.addRestriction("M_SerNoCtl_ID", VIS.Query.prototype.EQUAL, serialidget);
                        VIS.viewManager.startWindow(windowid, zoomQuery);
                    }
                }
                else {
                    VIS.ADialog.info("VA005_Selectlotorserialcheck");
                    //alert(VIS.Msg.getMsg("VA005_Selectlotorserialcheck"));
                }
            });


            $cmbSerialDropdown.on("change", function () {

                if ($addatttextlot.is(":checked")) {
                    lotidget = VIS.Utility.Util.getValueOfInt($cmbSerialDropdown.val());
                }
                else if ($addatttextserial.is(":checked")) {
                    serialidget = VIS.Utility.Util.getValueOfInt($cmbSerialDropdown.val());
                }
            });

            $addnewattribute.off("click");
            $addnewattribute.on("click", function () {

                selectedAttributeID = 0;
                //$treeView.css("display", "none");
                $treeView.animate({ width: "20%" });
                $middivforattributevalue.css({ "display": "inherit", 'width': '30%' });
                //*** Show attribute dialog
                objectcall();
                $attributeobject.cleartext();
                $openattdialog.css("display", "inherit");
                var str = '';
                str = "<div id='" + $self.windowNo + "divappendinleftsideboxes' class='VA005-attributediv  VA005-divboxtarget-design-hover'>"
                    + "<label>" + VIS.Msg.getMsg("VA005_AddAttribute") + "</label>"
                    + "</div>";
                $attributedivleftdragdrop.prepend($(str));
                //  $undoattributeboxes.css("display", "none");
                //$editattributeboxes.css("display", "inherit");
            });


        };

        // All attributeset dialog events..
        function dialogAttributeSetventHandle() {

            if (nodeID > 0) {
                editAttributeSet(nodeID);
            }
            else {
                cleartext();
            }

            if ($addatttextname.val().trim().length <= 0) {
                //$addatttextname.css("background-color", "pink");
                $addatttextname.addClass('vis-ev-col-mandatory');
            }
            else {
                //$addatttextname.css("background-color", "white");
                $addatttextname.removeClass('vis-ev-col-mandatory');
            }
        };

        //Btn enable with lot and serial combo box...
        function btnEnable() {
            $addattaddbtn.prop('disabled', false);
        };

        //*** Get mandatory type data..
        function getMendatoryTypeData() {
            $cmbattsectmandtype.empty();
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/AttributeListing/GetMandatoryType", "", MandatoryTypeCallBack);

            //var sql = "SELECT  AD_Ref_list.Name, AD_Ref_list.value" +
            //    " FROM AD_Ref_list" +
            //    " JOIN AD_Reference" +
            //    " ON AD_Ref_List.AD_Reference_ID=AD_Reference.AD_Reference_ID" +
            //    " WHERE AD_Reference.Name='M_AttributeSet MandatoryType' AND ad_ref_list.isactive='Y'";
            //var ds = VIS.DB.executeReader(sql.toString(), null);
            //if (ds != null) {
            //    var key, value = null;
            //    while (ds.read()) {
            //        value = ds.getString(0);
            //        key = ds.getString(1);
            //        $cmbattsectmandtype.append($("<Option value=" + key + ">" + value + "</option>"));
            //    }
            //    ds.close();
            //}
        };

        function MandatoryTypeCallBack(dr) {
            if (dr != null) {
                var Key, Name = null;
                for (var i = 0; i < dr.length; i++) {
                    Key = dr[i].Key;
                    Name = dr[i].Name;
                    $cmbattsectmandtype.append($("<Option value=" + Key + ">" + Name + "</option>"));
                }
            }
        };

        //*** Lot data dropdown data..
        function lotDataDropdown() {

            $cmbSerialDropdown.empty();

            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/AttributeListing/GetLotData", "", LotDataCallBack);

            // var sql = "SELECT M_LotCtl_id,Name FROM M_LotCtl WHERE isActive='Y'";

            //sql = VIS.MRole.addAccessSQL(sql, "M_LotCtl", true, true);

            //var ds = VIS.DB.executeReader(sql.toString(), null);
            //if (ds != null) {
            //    var key, value = null;
            //    $cmbSerialDropdown.append($("<Option value=''> </option>"));
            //    while (ds.read()) {
            //        value = ds.getString(1);
            //        key = VIS.Utility.Util.getValueOfInt(ds.getString(0));
            //        $cmbSerialDropdown.append($("<Option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>"));
            //    }
            //    ds.close();
            //}
        };
        function LotDataCallBack(dr) {
            if (dr != null) {
                var Key, Name = null;
                $cmbSerialDropdown.append($("<Option value=''> </option>"));
                for (var i = 0; i < dr.length; i++) {
                    Key = dr[i].Key;
                    Name = dr[i].Name;
                    $cmbSerialDropdown.append($("<Option value=" + Key + ">" + VIS.Utility.encodeText(Name) + "</option>"));
                }
            }
        };


        //*** Serial data dropdown
        function serialDatadDopdown() {

            $cmbSerialDropdown.empty();
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/AttributeListing/GetSerialData", "", SerialDataCallBack);


            //var sql = "SELECT M_SerNoCtl_id,Name FROM M_SerNoCtl WHERE isActive='Y'";
            //sql = VIS.MRole.addAccessSQL(sql, "M_SerNoCtl", true, true);
            //var ds = VIS.DB.executeReader(sql.toString(), null);
            //if (ds != null) {
            //    var key, value = null;
            //    $cmbSerialDropdown.append($("<Option value=''> </option>"));
            //    while (ds.read()) {
            //        value = ds.getString(1);
            //        key = VIS.Utility.Util.getValueOfInt(ds.getString(0));
            //        $cmbSerialDropdown.append($("<Option value=" + key + ">" + VIS.Utility.encodeText(value) + "</option>"));
            //    }
            //    ds.close();
            //}
        };
        function SerialDataCallBack(dr) {
            if (dr != null) {
                var Key, Name = null;
                $cmbSerialDropdown.append($("<Option value=''> </option>"));
                for (var i = 0; i < dr.length; i++) {
                    Key = dr[i].Key;
                    Name = dr[i].Name;
                    $cmbSerialDropdown.append($("<Option value=" + Key + ">" + VIS.Utility.encodeText(Name) + "</option>"));
                }
            }
        };



        //*** Save attributeset value..
        function SaveAttributeSetValue() {

            if ($addatttextlot.is(":checked")) {
                lotvaluee = $cmbSerialDropdown.val();
                islotcheck = "true";
            }
            else {
                //lotvaluee = "null";
                islotcheck = "false";
            }
            if ($addatttextserial.is(":checked")) {
                serialvaluee = $cmbSerialDropdown.val();
                isserialcheck = "true";
            }
            else {
                //serialvaluee = "null";
                isserialcheck = "false";
            }

            var valuesendtoctrl = {
                name: VIS.Utility.encodeText($addatttextname.val()),
                description: VIS.Utility.encodeText($$addatttextdes.val()),
                mandatorytype: $cmbattsectmandtype.val(),
                IsGuaranteeDate: $addatttextexpdate.prop("checked"),
                IsGuaranteeDateMandatory: $mandatoryexpdate.prop("checked"),
                lotvalue: lotvaluee,
                serialvalue: serialvaluee,
                islotcheck: islotcheck,
                isserialcheck: isserialcheck,
                ID: nodeID
            };

            $.ajax({
                url: VIS.Application.contextUrl + "AttributeListing/SaveAttributeSetValue",
                type: 'Post',
                async: false,
                datatype: "Json",
                cache: false,

                data: valuesendtoctrl,

                success: function (data) {

                    $getidfromattributeset = JSON.parse(data);
                },
                error: function (data) {

                },
            });
        };

        //***  Edit function for attributeset dialog...
        function editAttributeSet(nodeID) {

            cleartext();
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/AttributeListing/GetAttributeSetData", { "NodeID": nodeID }, AttributeSetDataCallBack);

            //var sql = "Select NAME,DESCRIPTION,MANDATORYTYPE,ISGUARANTEEDATE,ISGUARANTEEDATEMANDATORY,M_LOTCTL_ID,M_SERNOCTL_ID,IsLot,IsSerNo FROM M_AttributeSet WHERE M_Attributeset_ID=" + nodeID;
            // sql = VIS.MRole.addAccessSQL(sql, "M_AttributeSet", true, true);            
            //var ds = VIS.DB.executeDataSet(sql, null, null);
            //if (ds != null) {
            //    if (ds.tables[0].rows.length > 0) {
            //        for (var i = 0; i < ds.tables[0].rows.length; i++) {
            //            $addatttextname.val(ds.tables[0].rows[0].cells["name"].toString().trim());
            //            //$$addatttextdes.val(ds.tables[0].rows[0].cells["description"].toString().trim());
            //            $$addatttextdes.val(ds.tables[0].rows[0].cells["description"]);
            //            $cmbattsectmandtype.val(ds.tables[0].rows[0].cells["mandatorytype"]);
            //            $addatttextexpdate.val(ds.tables[0].rows[0].cells["isguaranteedate"]);

            //            if (ds.tables[0].rows[0].cells["isguaranteedate"] == "Y") {
            //                $addatttextexpdate.prop("checked", true);
            //                $mandatoryexpdate.prop('disabled', false);

            //            }
            //            else {
            //                $addatttextexpdate.prop("checked", false);
            //            }

            //            $mandatoryexpdate.val(ds.tables[0].rows[0].cells["isguaranteedatemandatory"]);

            //            if (ds.tables[0].rows[0].cells["isguaranteedatemandatory"] == "Y") {
            //                $mandatoryexpdate.attr('disabled', false);
            //                $mandatoryexpdate.prop('checked', true);
            //            }
            //            else {
            //                $mandatoryexpdate.prop('checked', false);
            //            }

            //            $addatttextlot.val(ds.tables[0].rows[0].cells["m_lotctl_id"]);

            //            if (ds.tables[0].rows[0].cells["islot"] == 'Y') {
            //                $addatttextlot.prop("checked", true);
            //                //$addatttextlot.trigger("click");
            //                lotDataDropdown();
            //                $cmbSerialDropdown.val(ds.tables[0].rows[0].cells["m_lotctl_id"]);
            //                lotidget = ds.tables[0].rows[0].cells["m_lotctl_id"];
            //            }
            //            else {
            //                $addatttextlot.prop("checked", false);
            //            }

            //            $addatttextserial.val(ds.tables[0].rows[0].cells["m_sernoctl_id"]);

            //            if (ds.tables[0].rows[0].cells["isserno"] == 'Y') {
            //                $addatttextserial.prop("checked", true);
            //                //$addatttextserial.trigger("click");
            //                serialDatadDopdown();
            //                $cmbSerialDropdown.val(ds.tables[0].rows[0].cells["m_sernoctl_id"]);
            //                serialidget = ds.tables[0].rows[0].cells["m_sernoctl_id"];
            //            }
            //            else {
            //                $addatttextserial.prop("checked", false);
            //            }
            //        }
            //    }
            //}

            //$addattaddbtn.prop('disabled', true);
            //if ($addatttextname.val().trim().length <= 0) {
            //    //$addatttextname.css("background-color", "pink");
            //    $addatttextname.addClass('vis-ev-col-mandatory');
            //}
            //else {
            //    //$addatttextname.css("background-color", "white");
            //    $addatttextname.removeClass('vis-ev-col-mandatory');
            //}

        };
        function AttributeSetDataCallBack(dr) {
            if (dr != null) {
                if (dr.length > 0) {
                    for (var i = 0; i < dr.length; i++) {
                        $addatttextname.val(dr[i].name);
                        $$addatttextdes.val(dr[i].description);
                        $cmbattsectmandtype.val(dr[i].mandatorytype);
                        $addatttextexpdate.val(dr[i].isguaranteedate);

                        if (dr[i].isguaranteedate == "Y") {
                            $addatttextexpdate.prop("checked", true);
                            $mandatoryexpdate.prop('disabled', false);

                        }
                        else {
                            $addatttextexpdate.prop("checked", false);
                        }

                        $mandatoryexpdate.val(dr[i].isguaranteedatemandatory);

                        if (dr[i].isguaranteedatemandatory == "Y") {
                            $mandatoryexpdate.attr('disabled', false);
                            $mandatoryexpdate.prop('checked', true);
                        }
                        else {
                            $mandatoryexpdate.prop('checked', false);
                        }

                        $addatttextlot.val(dr[i].m_lotctl_id);

                        if (dr[i].islot == 'Y') {
                            $addatttextlot.prop("checked", true);
                            //$addatttextlot.trigger("click");
                            lotDataDropdown();
                            $cmbSerialDropdown.val(dr[i].lotctl_id);
                            lotidget = dr[i].m_lotctl_id;
                        }
                        else {
                            $addatttextlot.prop("checked", false);
                        }

                        $addatttextserial.val(dr[i].m_sernoctl_id);

                        if (dr[i].isserno == 'Y') {
                            $addatttextserial.prop("checked", true);
                            //$addatttextserial.trigger("click");
                            serialDatadDopdown();
                            $cmbSerialDropdown.val(dr[i]._sernoctl_id);
                            serialidget = dr[i].m_sernoctl_id;
                        }
                        else {
                            $addatttextserial.prop("checked", false);
                        }
                    }
                }
            }

            $addattaddbtn.prop('disabled', true);
            if ($addatttextname.val().trim().length <= 0) {
                //$addatttextname.css("background-color", "pink");
                $addatttextname.addClass('vis-ev-col-mandatory');
            }
            else {
                //$addatttextname.css("background-color", "white");
                $addatttextname.removeClass('vis-ev-col-mandatory');
            }
        };


        function createBusyIndicator() {
            $bsyDiv = $("<div>");
            $bsyDiv.css("position", "absolute");
            $bsyDiv.css("bottom", "0");
            $bsyDiv.css("background", "url('" + VIS.Application.contextUrl + "Areas/VIS/Images/busy.gif') no-repeat");
            $bsyDiv.css("background-position", "center center");
            $bsyDiv.css("width", "98%");
            $bsyDiv.css("height", "98%");
            $bsyDiv.css('text-align', 'center');
            $bsyDiv.css('z-index', '1000');
            //$bsyDiv.css('background-color', '#0084c4');
            //$bsyDiv.css('opacity', '0.2');
            $bsyDiv[0].style.visibility = "visible";
            $root.append($bsyDiv);
        };

        this.removeatttvaluewithgrid = function (attvalidondel, attidonattvaldel, _count) {

            $bsyDiv[0].style.visibility = "visible";

            var attvalidondelgrid = attvalidondel + "_3_" + $self.windowNo;

            //var attvaluenode = $($($divLeftTree.find('.k-group').find("div[id='" + attvalidondel + "_3_" + $self.windowNo + "']")).parents()[2]);
            var attvaluenode = $($divLeftTree.find('.k-group').find("div[id='" + attvalidondel + "_3_" + $self.windowNo + "']"));
            // var attvaluenode = $divLeftTree.find('.k-group').find('.va005-parentss').parents().find("div[id='" + attvalidondel + "_3_" + $self.windowNo + "']");

            //var sql = "SELECT COUNT(*) FROM M_AttributeValue WHERE M_Attribute_ID=" + attidonattvaldel;
            //SQL = VIS.MRole.addAccessSQL(sql, "M_AttributeValue", true, true);
            //var _count = VIS.DB.executeScalar(sql);

            //if (_count <= 0) {
            //    var minus = $($($($($divLeftTree.find('.k-group').find("div[id='" + attvalidondel + "_3_" + $self.windowNo + "']")).parents()[2])).parents()[1]).find('.k-minus');
            //    minus.remove();
            //}

            for (var i = 0; i < attvaluenode.length; i++) {

                if (_count <= 0) {
                    //var minus = $($($($($divLeftTree.find('.k-group').find("div[id='" + attvalidondel + "_3_" + $self.windowNo + "']")).parents()[2])).parents()[1]).find('.k-minus');
                    var minus = $($divLeftTree.find("div[id='" + attvalidondel + "_3_" + $self.windowNo + "']").parent().parent().parent().parent().parent()).find('.k-minus');
                    minus.remove();
                }
                ($(attvaluenode[i]).parent().parent().parent()).remove();
                $(attvaluenode[i]).remove();
            }

            $bsyDiv[0].style.visibility = "hidden";
            //var treeview = $divLeftTree.data("kendoTreeView");            
            //var attvaluenode=$divLeftTree.find('.k-group').find("div[id='" + data + "_3_" + $self.windowNo + "']");           

        };

        this.updateatttvalueintree = function (attributevalueidgride, updatedname) {
            debugger;
            var createattributevalueidgride = attributevalueidgride + "_3_" + $self.windowNo;
            var selectedNode = $divLeftTree.find("div[id='" + createattributevalueidgride + "']");
            for (var i = 0; i < selectedNode.length; i++) {
                //$(selectedNode[i]).text(updatedname);
                selectedNode.find('p').text(updatedname);
            }
        };


        this.addatttvaluewithgrid = function (attidfromattform, name, $getidfromattributeset) {
            debugger;
            //+ "_3_" + $self.windowNo
            var getidattvalonsave = $getidfromattributeset + "_3_" + $self.windowNo;
            attidfromattform = attidfromattform + "_2_" + $self.windowNo;
            //var selectedNode = $divLeftTree.find('.k-group').find('.k-state-selected').find("div[id='" + attidfromattform + "_2_" + $self.windowNo + "']");
            //var selectedNode = $divLeftTree.find('.k-group').find("div[id='" + attidfromattform + "']");
            //var selectedNode = $divLeftTree.find("div[id='" + attidfromattform + "']");

            //var arr = [];
            var selectedNode = $divLeftTree.find("div[id='" + attidfromattform + "']");
            //arr.push(selectedNode);

            for (var i = 0; i < selectedNode.length; i++) {
                var newChild = $divLeftTree.data("kendoTreeView").append({
                    text: name,
                    'NodeID': $getidfromattributeset,
                    'UID': getidattvalonsave
                }, $(selectedNode[i]));

                //newChild.find('p').css({ 'margin': '7px 47px 0px 67px' });
                newChild.find('p').css({ 'margin': '7px 12px 0px 67px' });
                newChild.find('img').css("display", 'none');
                $(newChild.find('.va005-editiconattvaluehide')).css('display', 'none');
                newChild.find('.k-in').css('height', '33px');
                $(newChild.find('.va005-editiconattinfoclosehide')).css("display", "none");
            }


            //var newChild = $divLeftTree.data("kendoTreeView").append({
            //    text: name,
            //    'NodeID': $getidfromattributeset,
            //    'UID': getidattvalonsave
            //}, selectedNode);



            //newChild.find('p').css({ 'margin': '7px 47px 0px 67px' });
            //newChild.find('img').css("display", 'none');
            //$(newChild.find('.va005-editiconattvaluehide')).css('display', 'none');
            //$(newChild.find('.va005-editiconattinfoclosehide')).css("display", "none");
        };




        //Main Load tree function ...
        var loadTreeData = function () {
            $bsyDiv[0].style.visibility = "visible";
            $.ajax({
                url: VIS.Application.contextUrl + "AttributeListing/TreeViewForAttributeSetListing",
                type: 'Get',
                async: false,
                data: {
                    Expend: $self.windowNo
                },
                success: function (data) {
                    var res = JSON.parse(data);

                    if ($divLeftTree && $divLeftTree.data("kendoTreeView") != undefined) {
                        $divLeftTree.data("kendoTreeView").destroy();
                        $divLeftTree.empty();
                    }

                    $divLeftTree = $divid.kendoTreeView({
                        dragAndDrop: false,
                        dataSource: res,
                        //dragstart:function(e)
                        //{
                        //    //e.preventDefault();
                        //    //return false;

                        //},
                        //drop:function(){alert('adas')},

                        dataBound: function (e) {

                        },



                        select: function (event) {

                            debugger;

                            selectflag = false;
                            //window.setTimeout(function () {
                            //    $divLeftTree.find(".k-state-selected").css("background-color", "blue");
                            //}, 60);

                            var selectedNode = $(event.node);
                            //nodeID = $(selectedNode.find('.data-id ')).data('nodeid');
                            //alert('hello');
                            var type = $(selectedNode.find('.data-id ')).data('dtype');

                            if (type == 1) {
                                //$deletebtnhideshow.css("display", "none");
                                $gettypeoftree = type;
                                $deleteAttribute.removeClass("VA005-disabled");
                            }
                            else if (type == 2) {

                                // $deletebtnhideshow.css("display", "inherit");
                                $deleteAttribute.addClass("VA005-disabled");
                            }
                            else if (type == 3) {
                                // $deletebtnhideshow.css("display", "inherit");
                                $deleteAttribute.addClass("VA005-disabled");
                            }


                            //var $item = $(event.node);
                            //console.log( $item );
                            //alert( "selected" );
                        },
                        //    var uidss='#'+ id+'_2'+window;

                        //$(uidds)

                        //*** First div for tree contain first image and paragraph on tree..
                        template: "<div  id='#= item.UID #'  nid='#= item.NID #'  class='va005-parentss'><div class='#= item.classforgetnod #'>" +
                            //"<div style='overflow:auto' class='va005-parentss'><div class='#= item.classforgetnod #' style='margin:#= item.margin #;float:left'>" +
                            "<div  class='va005_mouseover' style='float:left'>" +
                            "<i class='#= item.ImageSource #' style='display:#= item.visibility #''></i>" +
                            //"<img src='" + VIS.Application.contextUrl + "#= item.ImageSource #' style='display:#= item.visibility #''>" +
                            "<p>#= item.text #</p>" +
                            "</div>" +
                            "</div>" +

                            //*** second div for tree contain attribute edit..
                            "<div class='va005-editiconattvaluehide'  >" +
                            "<div style='display:#= item.visibility #'>" +
                            //"<a style='cursor:pointer;display:#= item.visibility #'><img src='" + VIS.Application.contextUrl + "#= item.Image2 #' data-nodeid='#= item.NodeID #' data-dType= '#= item.Type #' class='va005_editSet' style='vertical-align: text-top;float: left;margin: 8px 0px 0px 0px;display:#= item.visibility #''></a>" +
                            "<a  style='display:#= item.visibility #' data-nodeid='#= item.NodeID #' data-dType= '#= item.Type #' class='data-id va005_editSet vis vis-edit va005-editattributecls' ></a>" +
                            "</div>" +
                            "</div>" +

                            //*** Thired div for tree show only related attributeset value..
                            "<div style='margin: 0;float:left' class='va005-editiconattinfoclosehide'>" +
                            "<div style='width:58px;float:right;display:#= item.ShowInfo #'>" +
                            //"<a style='cursor:pointer;display:#= item.ShowInfo #'><img src='" + VIS.Application.contextUrl + "#= item.Image3 #'  data-nodeid='#= item.NodeID #' class='va005_editattribute'  style='vertical-align: text-top;float: left;margin: 6px 0px 10px 12px;display:#= item.ShowInfo #''></a>" +
                            "<a style='cursor:pointer;display:#= item.ShowInfo #'><span  data-nodeid='#= item.NodeID #' class='va005_editattribute fa fa-italic'  style='float: left;margin: 9px 0px 10px 9px;display:#= item.ShowInfo #''></span></a>" +
                            "<a style='cursor:pointer;display:#= item.ShowInfo #'><span  data-parentid='#= item.ParentID #'  data-nodeid='#= item.NodeID #' class='va005_remove fa fa-times'  style='float: left;margin: 9px 0px 10px 9px;display:#= item.ShowInfo #''></span></a>" +
                            "</div>" +
                            "</div></div>"
                    });

                    $divLeftTree.off(VIS.Events.onTouchStartOrClick, ".va005_editSet");
                    $divLeftTree.on(VIS.Events.onTouchStartOrClick, ".va005_editSet", editItem);

                    $divLeftTree.off(VIS.Events.onTouchStartOrClick, ".va005_editattribute");
                    $divLeftTree.on(VIS.Events.onTouchStartOrClick, ".va005_editattribute", showDialogforLinkAttSetAndAtt);
                    $divLeftTree.on(VIS.Events.onTouchStartOrClick, ".va005_remove", removeattfromattset);

                    $bsyDiv[0].style.visibility = "hidden";
                    DropItem();
                    //$divLeftTree.find(".k-state-hover").css("background-color", "Gainsboro ");
                },
                error: function (data) {
                    alert(data);
                    $bsyDiv[0].style.visibility = "hidden";
                },

            });


            dragdropnewold();

        };

        function dragdropnewold() {
            debugger;
            //$bsyDiv[0].style.visibility = "visible";
            //$divLeftTree.data("kendoTreeView").wrapper.find(".va005-parentss").eq(1).attr('id');
            //$divLeftTree.data("kendoTreeView").wrapper.find(".va005-parentss").eq(1).parent().on({
            $divLeftTree.data("kendoTreeView").wrapper.find(".va005-parentss").parent().on({
                mouseenter: function () {
                    //window.setTimeout(function () {
                    //    $divLeftTree.find(".k-state-hover").css("background-color", "Gainsboro");                        
                    //}, 60);

                    if ($dragevents == true) {
                        //$(this).addClass("k-state-hover");VA005-classforgetnod 
                        var element = $(this).find(".classforgetnod");
                        if (element.length > 0) {
                            $(this).find(".va005-parentss").addClass("VA005-classforgetnod");
                        }
                        else {
                            $(this).css({ 'cursor': 'not-allowed' });
                        }
                    }
                },
                mouseleave: function () {

                    //window.setTimeout(function () {
                    //    $divLeftTree.find(".ui-droppable").css("background-color", "transparent");
                    //}, 60);


                    if ($dragevents == true) {
                        var element = $(this).find(".classforgetnod");
                        if (element.length > 0) {
                            $(this).find(".va005-parentss").removeClass("VA005-classforgetnod");

                        }
                        else {
                            $(this).css({ 'cursor': 'auto' });

                        }
                    }
                }
            });
            //$divLeftTree.data("kendoTreeView").wrapper.find(".va005-parentss").on({
            //    mouseenter: function () {
            //        if ($dragevents == true) {
            //            //$(this).addClass("k-state-hover");VA005-classforgetnod 
            //            var element = $(this).find(".classforgetnod");
            //            if (element.length > 0) {
            //                $(this).find(".va005-parentss").addClass("VA005-classforgetnod");
            //            }
            //            else {
            //                $(this).css({ 'cursor': 'not-allowed' });

            //            }
            //        }
            //    },
            //    mouseleave: function () {
            //        if ($dragevents == true) {
            //            var element = $(this).find(".classforgetnod");
            //            if (element.length > 0) {
            //                $(this).find(".va005-parentss").removeClass("VA005-classforgetnod");
            //            }
            //            else {
            //                $(this).css({ 'cursor': 'auto' });
            //            }
            //        }
            //    }
            //});

            //$bsyDiv[0].style.visibility = "hidden";
        }



        function removeattfromattset(e) {

            var attributeID = $(this).data("nodeid");
            e.preventDefault();
            var treeview = $divLeftTree.data("kendoTreeView");
            treeview.remove($(this).closest(".k-item"));

            var ParentID = $(this).data("parentid");

            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/AttributeListing/DeleteAttributeUse", { "AttributeID": attributeID, "ParentId": ParentID });

            //var sql = "DELETE FROM M_Attributeuse WHERE M_Attribute_ID=" + attributeID + " and M_Attributeset_id=" + ParentID;
            //var ds = VIS.DB.executeDataSet(sql, null, null);


        }

        var $idgetforattribute = false;

        function editItem(e) {

            var target = $(this).data("dtype");
            if (target == "1") {

                $bsyDiv[0].style.visibility = "visible";
                nodeID = $(this).data("nodeid");
                $treeView.css("display", "none");

                $leftboxdivwidth.animate({ width: "50%" });
                $middivforattributevalue.animate({ width: "0%" });
                $middivforattributevalue.css({ "display": "none", 'width': '0%' });

                $openattdialog.animate({ width: "50%" });
                $openattdialog.css("display", "inherit");
                editAttributeSet(nodeID);
                $attributeobject.gridkendo();

                // $editbtnatthideshow.css("display", "none");
                // $middivforattributevaluehideshow.css("display", "none");
                $($attributedivleftdragdrop.children()).find('div .VA005-checkboxonleftdiv').removeAttr("checked");

                $attributedivleftdragdrop.find('.VA005-divboxtarget-design').removeClass('VA005-divboxtarget-design');
                // $attributedivleftdragdrop.find('.VA005-divboxtarget-design').removeClass('VA005-divboxtarget-design');
                selectedIds = [];
                $delattributeboxes.addClass("VA005-disabled");
                $bsyDiv[0].style.visibility = "hidden";
            }
            else if (target == "2") {
                $treeView.animate({ width: "30%" });
                //$divid.css({ 'min-width': '420px', 'overflow': 'auto' });
                $bsyDiv[0].style.visibility = "visible";

                $editattributeboxes.addClass("VA005-disabled");
                selectedAttributeID = $(this).data("nodeid");

                getidfromattboxexforaddnode = selectedAttributeID;

                textattribute = $($attributedivleftdragdrop.find("[data-id='" + selectedAttributeID + "']")).find("label");
                $self.$idgetforattribute = true;

                //$treeView.css("display", "none");
                //*** Show attribute dialog
                objectcall();
                if (!$middivforattributevalue.is(':visible')) {
                    $leftboxdivwidth.animate({ width: "40%" });
                    $openattdialog.css("display", "none");
                    $treeView.animate({ width: "30%" });
                    $middivforattributevalue.animate({ width: "30%" });
                    $middivforattributevalue.css({ "display": "inherit", 'width': '30%' });
                }


                $attributedivleftdragdrop.find('.VA005-divboxtarget-design').removeClass('VA005-divboxtarget-design');

                // $editbtnatthideshow.css("display", "none");
                // $middivforattributevaluehideshow.css("display", "none");
                $($attributedivleftdragdrop.children()).find('div .VA005-checkboxonleftdiv').removeAttr("checked");
                $delattributeboxes.addClass("VA005-disabled");
                selectedIds = [];


                // window.setTimeout(function () {
                $divid.css("overflow", "auto")

                //}, 200);

                // window.setTimeout(function () {
                //            $divid.css("overflow", "auto")
                //            $('#' + $self.windowNo + 'showtree').css("min-width", "420px");
                //}, 1000);

                $attributeobject.cmboonclick();

                $self.attributevallueonattributeclick(selectedAttributeID);

                $bsyDiv[0].style.visibility = "hidden";




                //$treeView.css("display", "none");           
                //$openattdialog.animate({ width: "30%" });
                //  //if (flag == true)
            }


        };


        function deletefromtreedata() {

            $bsyDiv[0].style.visibility = "visible";

            var selectedNode = $divLeftTree.data("kendoTreeView").select();
            nodeID = $(selectedNode.find('.data-id ')).data('nodeid');

            var type = $(selectedNode.find('.data-id ')).data('dtype');

            if (type == "1") {


                //var msg = "" + VIS.Msg.getMsg("VA005_DeleteIt") + "";
                //var r = VIS.ADialog.ask(msg);
                VIS.ADialog.confirm("VA005_DeleteIt", true, "", "Confirm", function (result) {
                    if (result == true) {

                        var valuesendtoctrl = {
                            attributesetdelID: nodeID
                        };

                        $.ajax({
                            url: VIS.Application.contextUrl + "AttributeListing/DeleteAttributeSetValue",
                            type: 'Post',
                            async: false,
                            datatype: "Json",
                            cache: false,
                            data: valuesendtoctrl,
                            success: function (data) {
                                data = JSON.parse(data);
                                var dataget = data;
                                if (dataget == "") {
                                    $divLeftTree.data("kendoTreeView").remove(selectedNode.closest(".k-item"));
                                }
                                else {
                                    VIS.ADialog.error(dataget);
                                }
                                //grdRows.splice(grdRows.indexOf(row), 1);

                            },
                            error: function (data) {
                                if (data != null) {
                                    VIS.ADialog.error(data);
                                }
                            },
                        });
                    }
                });


                //var sql = "delete from m_attributeset where isactive='Y' and m_attributeset_id=" + nodeID;
                //var ds = VIS.DB.executeDataSet(sql, null, null);
                //$divLeftTree.data("kendoTreeView").remove(selectedNode.closest(".k-item"));
                //$attributeobject.gridkendo();              
            }
            //else if (type == "2") {
            //    var sqlattribute = "delete from m_attribute where isactive='Y' and m_attribute_id=" + nodeID;
            //    var ds = VIS.DB.executeDataSet(sqlattribute, null, null);
            //    //loadTreeData();
            //    $divLeftTree.data("kendoTreeView").remove(selectedNode.closest(".k-item"));
            //    attributeAppendDiv();
            //    //$attributeobject.gridkendo();
            //}
            //else if (type == '3') {
            //    var sqlattvalue = "delete from m_attributevalue where isactive='Y' and m_attributevalue_id=" + nodeID;
            //    var ds = VIS.DB.executeDataSet(sqlattvalue, null, null);
            //    // loadTreeData();
            //    $divLeftTree.data("kendoTreeView").remove(selectedNode.closest(".k-item"));
            //    $attributeobject.gridkendo();
            //}
            //else {
            //    VIS.ADialog.info("VA005_SelectOneFieldMendatory");
            //    //alert(VIS.Msg.getMsg("VA005_SelectOneFieldMendatory"));
            //}
            $bsyDiv[0].style.visibility = "hidden";

        }

        //*** Dailog design for "I" name icon open related dialog
        function iconDialog() {
            $icon = $('<div>');
            //$iconullist = "<div style='padding:20px;height:auto;width:100%'>"
            //               //+   "<ul style='list-style-type: none;' id='" + $self.windowNo + "iconUi'>"
            //            + "<p style='font-size: 19px;'>Attribute Link With attributeSet :</p>"
            //            + "<ul style='list-style-type: none;font-size: 17px;line-height: 1.7em;' id='" + $self.windowNo + "iconUi'>"
            //            + "</ul>"
            //            + "</div>"

            $iconullist = "<div style='height:auto;width:100%'>"
                //+   "<ul style='list-style-type: none;' id='" + $self.windowNo + "iconUi'>"
                //+ "<p style='font-size: 16px;color: #535353;margin-bottom: 3px;'>Attribute Link With attributeSet :</p>"
                + "<ul style='line-height: 1.7em;padding-left: 2rem;;margin:0' id='" + $self.windowNo + "iconUi'>"
                + "</ul>"
                + "</div>"
            $icon.append($iconullist);

            //*** find "UI" ID
            $listid = $icon.find("#" + $self.windowNo + "iconUi");
        };

        //*** Get related data on icon click...
        function linkAttSetAndAttribute(attributeID) {
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/AttributeListing/LoadAttributeUse", { "AttributeID": attributeID }, AttributeUseCallBack);

            //var sql = "select mas.Name from m_attributeuse masu JOIN m_attributeset mas on masu.m_attributeset_id=mas.m_attributeset_id where masu.m_attribute_id=" + attributeID;
            //var ds = VIS.DB.executeDataSet(sql, null, null);
            //var ds = VIS.DB.executeReader(sql.toString(), null);
            //var $str = null;
            //if (ds != null) {
            //    while (ds.read()) {
            //        $listid.append($("<li>" + VIS.Utility.Util.getValueOfString(ds.getString(0)) + "</li>"));
            //    }
            //    ds.close();
            //}
        };
        function AttributeUseCallBack(dr) {
            if (dr != null) {
                for (var i = 0; i < dr.value.length; i++) {
                    $listid.append($("<li>" + VIS.Utility.Util.getValueOfString(dr.value[i]) + "</li>"));
                }
            }
        };

        //*** Show Link AttSet And Att()
        function showDialogforLinkAttSetAndAtt() {

            attributeID = $(this).data("nodeid");
            iconDialog();
            linkAttSetAndAttribute(attributeID);
            var createTab = new VIS.ChildDialog();
            createTab.setHeight(300);
            createTab.setWidth(350);
            createTab.setEnableResize(false);
            createTab.setTitle(VIS.Msg.getMsg('VA005_LinkAttributeSetAndAttribute'));
            createTab.setModal(true);
            createTab.setContent($icon);
            createTab.show();
            createTab.onClose = function () {
            };
            createTab.onOkClick = function (e) {
            };
            createTab.onCancelClick = function () {
            };
        };

        //*** Save Lot data...      
        function SaveLotData() {
            var valuesendtoctrl = {
                Name: VIS.Utility.encodeText($divnametxt.val()),
                StartNo: VIS.Utility.encodeText($divstarttxt.val()),
                CurrentNext: VIS.Utility.encodeText($divcurrenttxt.val()),
                Increment: VIS.Utility.encodeText($incrementtext.val()),
                prefix: VIS.Utility.encodeText($prefixtext.val()),
                Suffix: VIS.Utility.encodeText($serfixtext.val()),
                IsLot: true,
            };

            $.ajax({
                url: VIS.Application.contextUrl + "AttributeListing/SaveLotData",
                type: 'Post',
                async: false,
                datatype: "Json",
                cache: false,
                data: valuesendtoctrl,

                success: function (data) {
                    lotidjson = JSON.parse(data);
                },
                error: function (data) {

                },
            });
            lotidget = lotidjson;
        };

        //Clear div text of attributeset dialog
        function clearDivtext() {
            $divnametxt.val("");
            $divstarttxt.val("");
            $divcurrenttxt.val("");
            $incrementtext.val("");
            $prefixtext.val("");
            $serfixtext.val("");
            $divnametxt.trigger("change");
        }

        var serialidjson = null;
        var serialidget = null;
        //*** Save serial data...
        function saveSerialData() {

            var valuesendtoctrl = {
                name: $divnametxt.val(),
                startno: $divstarttxt.val(),
                currentnext: $divcurrenttxt.val(),
                incrementno: $incrementtext.val(),
                prefix: $prefixtext.val(),
                sufix: $serfixtext.val(),
                IsLot: true,
            };

            $.ajax({
                url: VIS.Application.contextUrl + "AttributeListing/saveSerialData",
                type: 'Post',
                async: false,
                datatype: "Json",
                cache: false,
                data: valuesendtoctrl,

                success: function (data) {
                    serialidjson = JSON.parse(data);
                },
                error: function (data) {

                },
            });

            serialidget = serialidjson;
        };

        //*** Attributeset dialog clear top div valule
        function cleartext() {

            $addatttextname.val("");
            $$addatttextdes.val("");
            $addatttextexpdate.prop("checked", false)
            $mandatoryexpdate.prop("checked", false);
            $mandatoryexpdate.prop("disabled", true);
            $addatttextlot.prop("checked", false);
            $addatttextserial.prop("checked", false);
            //$cmbSerialDropdown.val("");
            $cmbSerialDropdown.empty();
        };

        //*** Get field length....      

        function filedlength() {

            //var tableattributesetID = "select ad_table_id from ad_table where tablename='M_AttributeSet'";
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/AttributeListing/GetFieldLength", { "TableID": 560, "COLUMNNAME": "'Name','Description'" }, FieldCallBack);

            //var sqlname = "SELECT fieldlength FROM ad_column WHERE ad_table_id=(" + tableattributesetID + ") AND COLUMNNAME='Name'  AND isActive ='Y'";
            // var ds = VIS.DB.executeReader(sqlname.toString(), null);
            //columnLength = VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[0].cells.fieldlength);
            //$addatttextname.attr("maxlength", columnLength);

            //var sqldes = "SELECT fieldlength FROM ad_column WHERE ad_table_id=(" + tableattributesetID + ") AND COLUMNNAME ='Description'  AND isActive ='Y'";
            //var ds1 = VIS.DB.executeReader(sqldes.toString(), null);
            //columndesLength = VIS.Utility.Util.getValueOfInt(ds1.tables[0].rows[0].cells.fieldlength);
            //$$addatttextdes.attr("maxlength", columndesLength);

        };
        function FieldCallBack(dr) {
            if (dr != null) {
                for (var i = 0; i < dr.length; i++) {
                    if (dr[i].COLUMNNAME == 'Name') {
                        $addatttextname.attr("maxlength", dr[i].fieldlength);
                    }
                    else if (dr[i].COLUMNNAME == 'Description') {
                        $addatttextdes.attr("maxlength", dr[i].fieldlength);
                    }
                }
            }
        }


        //*** event handelling for lot serial dialog...
        function eventslotserialdialog() {

            // ***second Div text box on attributeset dialog
            $divnametxt.on("change", function () {

                if ($divnametxt.val().trim().length <= 0) {
                    $divnametxt.css("background-color", "pink");
                }
                else {
                    $divnametxt.css("background-color", "white");
                }
            });

            fieldlengthlotserialdialog();
        }

        //*** field length for lot serial dialog
        function fieldlengthlotserialdialog() {

            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "AttributeListing/GetFieldLength", { "TableID": 556, "COLUMNNAME": "'Name','StartNo','CurrentNext','IncrementNo','Prefix','Suffix'" }, LengthCallBack);


            //var lottableID = "select ad_table_id from ad_table where tablename='M_LotCtl'";
            //var sqldivname = "SELECT fieldlength FROM ad_column WHERE ad_table_id=(" + lottableID + ") AND COLUMNNAME='Name' AND isActive  ='Y'";
            //var ds2 = VIS.DB.executeReader(sqldivname.toString(), null);
            //clumnlengthLotLength = VIS.Utility.Util.getValueOfInt(ds2.tables[0].rows[0].cells.fieldlength);
            //$divnametxt.attr("maxlength", clumnlengthLotLength);

            //var sqldivstartno = "SELECT fieldlength FROM ad_column WHERE ad_table_id=(" + lottableID + ") AND COLUMNNAME='StartNo' AND isActive  ='Y'";
            //var ds3 = VIS.DB.executeReader(sqldivstartno.toString(), null);
            //columnstartnoLength = VIS.Utility.Util.getValueOfInt(ds3.tables[0].rows[0].cells.fieldlength);
            //$divstarttxt.attr("maxlength", columnstartnoLength);

            //var sqldivcurrenttext = "SELECT fieldlength FROM ad_column WHERE ad_table_id=(" + lottableID + ") AND COLUMNNAME='CurrentNext' AND isActive  ='Y'";
            //var ds4 = VIS.DB.executeReader(sqldivcurrenttext.toString(), null);
            //columncurrenttextLength = VIS.Utility.Util.getValueOfInt(ds4.tables[0].rows[0].cells.fieldlength);
            //$divcurrenttxt.attr("maxlength", columncurrenttextLength);

            //var sqldivincrement = "SELECT fieldlength FROM ad_column WHERE ad_table_id=(" + lottableID + ") AND COLUMNNAME='IncrementNo' AND isActive  ='Y'";
            //var ds5 = VIS.DB.executeReader(sqldivincrement.toString(), null);
            //columnincremnetnoLength = VIS.Utility.Util.getValueOfInt(ds5.tables[0].rows[0].cells.fieldlength);
            //$incrementtext.attr("maxlength", columnincremnetnoLength);

            //var sqldivprefix = "SELECT fieldlength FROM ad_column WHERE ad_table_id=(" + lottableID + ") AND COLUMNNAME='Prefix' AND isActive  ='Y'";
            //var ds6 = VIS.DB.executeReader(sqldivprefix.toString(), null);
            //columnprefixLength = VIS.Utility.Util.getValueOfInt(ds6.tables[0].rows[0].cells.fieldlength);
            //$prefixtext.attr("maxlength", columnprefixLength);

            //var sqldivSuffix = "SELECT fieldlength FROM ad_column WHERE ad_table_id=(" + lottableID + ") AND COLUMNNAME='Suffix' AND isActive  ='Y'";
            //var ds7 = VIS.DB.executeReader(sqldivSuffix.toString(), null);
            //columnsufixLength = VIS.Utility.Util.getValueOfInt(ds7.tables[0].rows[0].cells.fieldlength);
            //$prefixtext.attr("maxlength", columnsufixLength);
        }
        function LengthCallBack(dr) {
            if (dr != null) {
                for (var i = 0; i < dr.length; i++) {
                    if (dr[i].COLUMNNAME == 'Name') {
                        $divnametxt.attr("maxlength", dr[i].fieldlength);
                    }
                    else if (dr[i].COLUMNNAME == 'StartNo') {
                        $divstarttxt.attr("maxlength", dr[i].fieldlength);
                    }
                    else if (dr[i].COLUMNNAME == 'CurrentNext') {
                        $divstarttxt.attr("maxlength", dr[i].fieldlength);
                    }
                    else if (dr[i].COLUMNNAME == 'IncrementNo') {
                        $divstarttxt.attr("maxlength", dr[i].fieldlength);
                    }
                    else if (dr[i].COLUMNNAME == 'Prefix') {
                        $divstarttxt.attr("maxlength", dr[i].fieldlength);
                    }
                    else if (dr[i].COLUMNNAME == 'Suffix') {
                        $divstarttxt.attr("maxlength", dr[i].fieldlength);
                    }
                }
            }
        }

        this.disposeComponent = function () {
            $attributeobject.disposeComponent();
            remove();
            $tree = null;
            $divid = null;
            $openAttributeset = null;
            $openattdialog = null;
            $addatttextname = null;
            $$addatttextdes = null;
            $cmbattsectmandtype = null;
            $addatttextexpdate = null;
            $mandatoryexpdate = null;
            $addatttextlot = null;
            $addatttextserial = null;
            $addatttextlotserialtext = null;
            $cmbSerialDropdown = null;
            $addattaddbtn = null;
            $saveattributesetdata = null;
            $divnametxt = null;
            $divstarttxt = null;
            $divcurrenttxt = null;
            $incrementtext = null;
            $prefixtext = null;
            $serfixtext = null;
            $savelotserialdata = null;
            $divhideandshow = null;

        };

        function remove() {
            $openAttributeset.off("click");
            $addatttextlot.off("click");
            $addatttextserial.off("click");
            $addattaddbtn.off("click");
            //$saveattributesetdata.off("click");
            $addatttextexpdate.off("click");
            $addatttextname.off("click");
            //$divnametxt.off("click");

            $tree.remove();
            $divid.remove();
            $openAttributeset.remove();
            $openattdialog.remove();
            $addatttextname.remove();
            $$addatttextdes.remove();
            $cmbattsectmandtype.remove();
            $addatttextexpdate.remove();
            $mandatoryexpdate.remove();
            $addatttextlot.remove();
            $addatttextserial.remove();
            $addatttextlotserialtext.remove();
            $cmbSerialDropdown.remove();
            $addattaddbtn.remove();
        };

        this.formSizeChanged = function (height, width) {

            $divid.height($treeView.height() - ($treeView.find('.VA005-top-wrap-div-h').height() + 10));

            //$leftboxbackdivforspliter.height($leftboxdivwidth.height() - ($leftboxattvaluebelow.height() + 10));

            // $attributedivleftdragdrop.height($leftboxdivwidth.height() - ($leftboxattvaluebelow.height() + 10));

            // $leftboxattvaluebelow.height($leftboxdivwidth.height() - ($leftboxbackdivforspliter.height() + 10));
            //$leftboxdivwidth (leftdiv)
            //$leftboxbackdivforspliter(lefttopdiv)
            //$leftboxattvaluebelow(leftbelowdiv)

        };

        this.sizeChanged = function (height, width) {
            //formHeight = height;
            //formWidth = width;

            this.formSizeChanged(height, width);
        };

    };

    AttributeSetListing.prototype.init = function (windowNo, frame) {

        this.windowNo = windowNo;
        this.frame = frame;
        var obj = this.initialize();
        this.frame.getContentGrid().append(this.getRoot());
        this.formSizeChanged();
    };

    AttributeSetListing.prototype.sizeChanged = function (height, width) {

        this.formSizeChanged(height, width);
    };

    AttributeSetListing.prototype.refresh = function (height, width) {

        this.formSizeChanged();
    };

    //Must implement dispose
    AttributeSetListing.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };



    VA005.AForms = VA005.AForms || {};
    VA005.AForms.AttributeSetListing = AttributeSetListing;

})(VA005, jQuery);




