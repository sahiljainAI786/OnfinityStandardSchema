; VA005 = window.VA005 || {};

; (function (VA005, $) {
    function attribute(treeView, attributeAppendDiv, $middivforattributevalue, $leftboxdivwidth, $delattributeboxes, $editattributeboxes, flag, $attributevaluedivleftdragdrop, $senddivatt, objAttrSet, $bsyDiv) {
        this.windowNo;
        this.frame;
        var $senddivatt = $senddivatt;
        //var $setcss = $setcss;
        var $treeView = treeView;
        var $leftboxdivwidth = $leftboxdivwidth;
        var $attributevaluedivleftdragdrop = $attributevaluedivleftdragdrop;
        var $delattributeboxes = $delattributeboxes;
        var $editattributeboxes = $editattributeboxes;
        var flag = flag;
        var dataSource = null;
        var $middivforattributevalue = $middivforattributevalue;
        var $root = $("<div class='VA005-left-wrap-div-openattributre vis-forms-container' >");
        var $root2 = $("<div>");
        var $self = this;
        var columnLength = null;
        var columnlengthattributevalue = null;
        var columndescLength = null;
        var columnvalueLength = null;
        var refreshFunction = null;
        var selectedAttributeID = 0;
        var griddat = null;
        var idsendongrideshow = {};
        var kendoGrd = null;
        var attrbuteSetObj = objAttrSet;
        var attributeidgetonokclick = null;
        var mandatory = false;
        var isactivefield = true;
        var $activechkbox = false;

        var $getvalueonchange = false;
        var $textnametextvalue = null;
        this.$cmbovaluecheck = null;

     //   this.$getcheckboxclickcolor = false;


        //var $topdivforname = $('<div class="VA005-att-tree-vieww"><h4 style="float:left;color:#1aa0ed">' + VIS.Msg.getMsg("VA005_AddAttribute") + '</h4></div>');
        var $topdivforname = $('<div class="VA005-top-wrap-treedesign VA005-att-tree-vieww"><h4>' + VIS.Msg.getMsg("VA005_EditAttribute") + '</h4></div>');
        var $maindiv = $('<div style="height:100%">');
        var $textname = $('<input class="VA005-text-color-att-text" type="text" maxlength="20" placeholder=" " data-placeholder=""  >');
        var $textdesc = $('<input class="VA005-text-color-att-text"  type="text" placeholder=" " data-placeholder="" >');
        var $cmbselect = $('<select>');
        var $chkboxmandatoryfield = $('<input type="checkbox">');
        var $chkboxisactivefield = $('<input type="checkbox" checked>');




        //var $btnaddselect = $('<input type="button"  style="margin-top: 0px;float:right" value="Add New" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" >');
        //var $btnaddselect = $("<span style='float:right' ><img src='" + VIS.Application.contextUrl + "Areas/VA005/Images/add.png' id='" + $self.windowNo + "openattributeset'></img></span>");
        // var $btnaddselect = $("<span id='" + $self.windowNo + "openattributeset' class='glyphicon glyphicon-plus' style='float:right;font-size:21px;'></span>");

        //var $maindiv2 = $('<div style="display:none;">');

        // var $maindiv2 = $('<div style="margin-top:96px">');
        var $textsearchdiv = $('<input  class="VA005-text-color-att-text" type="text" placeholder=" " data-placeholder="" >');
        var $textnamediv = $('<input   class="VA005-text-color-att-text" class="vis-ev-col-mandatory" type="text" placeholder=" " data-placeholder="" >');
        var $btndiv = $('<input type="button" value="Save">');

        var $btnok = $('<input style="position:inherit;padding: 6px 14px !important; margin-left: 10px;" type="button" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" value="' + VIS.Msg.getMsg("Ok") + '" >')
        //var $btnok = $('<input type="button" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" value="OK" >')
        var $btncancel = $('<input style="position:inherit;padding: 6px 14px !important; margin-left: 10px;" type="button"  class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" value="' + VIS.Msg.getMsg("Cancel") + ' ">')

        var $btnokmiddiv = $('<input style="padding: 6px 14px !important; margin-left: 10px;" type="button" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" value="OK" >')
        var $btncancelmiddiv = $('<input style="padding: 6px 14px !important; margin-left: 10px;" type="button"  class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" value="Cancel">')
        var $attributenameh4 = $('<h4>' + VIS.Msg.getMsg("VA005_AddAttributeValue") + '</h4>');
        var $griddiv = $('<div  style="position:inherit;float:left;height:50px;width:100%;margin-top:10px;">');
        //var $griddiv = $('<div  style="z-index:0;float:left;height:50px;width:100%;margin-top:10px;">');


        var $opacitydiv = $('<div style="display:none;background-color:white;opacity:0.7;height: 32px;width:28px;float: right;position: absolute;right: 0;">');
        var newObject = null;
        // var $bsyDiv = null;

        //*** Design for attribute dailog..
        attributeDialogDeign();

        //*** Attribute Type combo box..
        selectionList();

        filedlength();

        eventCall();

        //this.textnamedivtextfill = function() {
        //    $textnamediv.text = "Add New..";
        //}

        var textvalue = null;
        var value = null;
        var valattribute = null;
        this.attributecount = function (value) {
             
            //var sql = "select count(name) as Name from m_attribute where isactive='Y'";
            //var ds = VIS.DB.executeDataSet(sql, null, null);
            //value = ds.tables[0].rows[0].cells["name"] + 1;
            //ds.tables[0].rows[0].cells["Name"]
            $textname.val(value);
           
            
            //textvalue = "Add Attribute " + value;
        }


        this.gridkendo = function () {
            gridekendobtns();
        }


        function deletefromgrid(selectedItem)
        {
            var attvalidfordel = selectedItem;
           
            $bsyDiv[0].style.visibility = "visible";
            var valuesendtoctrl = {
                //attributeID: e.data.models[e.data.models.length - 1].attributevalueid
                attributeID: selectedItem
            };

            $.ajax({
                url: VIS.Application.contextUrl + "Attribute/DeleteAttributeValue",
                type: 'Post',
                async: false,
                datatype: "Json",
                cache: false,
                data: valuesendtoctrl,
                success: function (data) {
                    data = JSON.parse(data);
                    var dataget = data;
                    if (dataget == "") {
                        //grdRows.splice(grdRows.indexOf(row), 1);

                        kendoGrd.data('kendoGrid').dataSource.remove(kendoGrd.data('kendoGrid').select().closest("tr"));
                        var elements = -1;

                        $.grep(grdRows, function (ele, index) {
                            //$bsyDiv[0].style.visibility = "visible";
                            if (ele.attributevalueid == selectedItem)
                            {
                                //$bsyDiv[0].style.visibility = "visible";
                                elements = index;
                            }
                        });


                        grdRows.splice(elements, 1);

                         
                    
                            //var sql = "SELECT COUNT(*) FROM M_AttributeValue WHERE M_Attribute_ID=" + selectedAttributeID;
                           // SQL = VIS.MRole.addAccessSQL(sql, "M_AttributeValue", true, true);
                        var _count = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "Attribute/GetAttributeCount", { "SelectedAttributeID": selectedAttributeID});
                          
                        objAttrSet.removeatttvaluewithgrid(attvalidfordel, selectedAttributeID, _count);

                        

                        kendoGrd.data('kendoGrid').dataSource.read();
                        attrbuteSetObj.attributevallueonattributeclick(selectedAttributeID);
                      
                        // gridekendobtns();

                      
                        //refreshFunction();


                        //$bsyDiv[0].style.visibility = "hidden";
                    }
                    else {
                        VIS.ADialog.error(dataget);
                        $bsyDiv[0].style.visibility = "hidden";
                    }
                },
                error: function (data) {
                    alert(data);
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });
             $bsyDiv[0].style.visibility = "hidden";
        }
        var grdRows = [];
        //=========================================================================================================================
        //var griddat = null;
        //var idsendongrideshow = {};
        //var kendoGrd = null;
        var getgridselectedid = 0;
        function gridekendobtns() {
            
            $bsyDiv[0].style.visibility = "visible";

            idsendongrideshow = {
                attributevalueid: selectedAttributeID
            };
            //////////////////
            var ismobile = /ipad|iphone|ipod/i.test(navigator.userAgent.toLowerCase());
            var height=250;
            if(ismobile)
            {
                //height=195;
                height = 210;
            }

            $.ajax({
                url: VIS.Application.contextUrl + "Attribute/ShowGrideData",
                dataType: "json",
                type: "POST",
                async: true,
                data: idsendongrideshow,
                success: function (data) {
                    $bsyDiv[0].style.visibility = "visible";
                    var row = {};
                    var dData = data.result;
                    grdRows = [];
                    for (var i = 0; i < dData.length; i++) {
                        row = {};
                        row["name"] = dData[i].name;
                        row["searchkey"] = dData[i].searchkey;
                        row["attributevalueid"] = dData[i].attributevalueid;
                        grdRows.push(row);
                        getgridselectedid = dData[i].attributevalueid;                        
                    }

                    var flagg = false;

                    var dataSource = new kendo.data.DataSource({
                        batch: true,
                        pageSize: 10,
                        schema: {
                            model: {
                                id: "attributevalueid",
                                fields: {
                                    attributevalueid: { editable: false, nullable: false },
                                    name: { validation: { required: true } },
                                    //searchkey: { validation: { required: false } }
                                    searchkey: {}
                                }

                            }
                        },
                        transport: {
                            read: function (options) {
                                $bsyDiv[0].style.visibility = "visible";
                                options.success(grdRows);
                                //attrbuteSetObj.attributevallueonattributeclick(selectedAttributeID);
                                $bsyDiv[0].style.visibility = "hidden";
                                //alert('ho ha ha');
                                //flagg = true;
                            },
                            update: function (options) {
                                var paramString = options.data.models[0]["name"].toString() + ", " + options.data.models[0]["searchkey"].toString() + ", " + options.data.models[0]["attributevalueid"].toString();
                                 VIS.dataContext.getJSONRecord("Attribute/Update", paramString);
                                //var sql = "update m_attributevalue set name='" + options.data.models[0]["name"] + "',value='" + options.data.models[0]["searchkey"] + "' where m_attributevalue_id=" + options.data.models[0]["attributevalueid"];
                                //VIS.DB.executeDataSet(sql, null);
                                var elements = $.grep(grdRows, function (ele, index) {
                                    if (ele.attributevalueid == options.data.models[0]["attributevalueid"]) {
                                        ele.name = options.data.models[0]["name"];
                                        if (options.data.models[0]["searchkey"] != null) {
                                            ele.searchkey = options.data.models[0]["searchkey"];
                                        }
                                        else {
                                            ele.searchkey = options.data.models[0]["name"];
                                        }
                                    }
                                    return '';
                                    // gridekendobtns();                              
                                });
                                kendoGrd.data('kendoGrid').dataSource.read();
                               // gridekendobtns();
                                attrbuteSetObj.attributevallueonattributeclick(selectedAttributeID);
                                //refreshFunction();

                                attrbuteSetObj.updateatttvalueintree((options.data.models[0]["attributevalueid"]), (options.data.models[0]["name"]));


                            },
                            //destroy: function (e) {
                            //   
                            //    e.preventDefault();
                            //    var valuesendtoctrl = {
                            //        attributeID: e.data.models[e.data.models.length - 1].attributevalueid
                            //    };

                            //    $.ajax({
                            //        url: VIS.Application.contextUrl + "Attribute/DeleteAttributeValue",
                            //        type: 'Post',
                            //        async: false,
                            //        datatype: "Json",
                            //        cache: false,
                            //        data: valuesendtoctrl,
                            //        success: function (data) {
                            //            var dataget = data;
                            //            if (dataget == "true") {
                            //                grdRows.splice(grdRows.indexOf(row), 1);
                            //                $griddiv.data('kendoGrid').refresh();
                            //                attrbuteSetObj.attributevallueonattributeclick(selectedAttributeID);
                            //                refreshFunction();
                            //            }
                            //            else {
                            //                return true;
                            //            }
                            //        },
                            //        error: function (data) {
                            //            if (data != null) {
                            //                alert(data);
                            //            }
                            //        }
                            //    });

                            //    //var sql = "delete from m_attributevalue  where m_attributevalue_id=" + options.data.models[0]["attributevalueid"];
                            //    //VIS.DB.executeDataSet(sql, null);

                            //    //$griddiv.data('kendoGrid').refresh();
                            //    //attrbuteSetObj.attributevallueonattributeclick(selectedAttributeID);
                            //    //refreshFunction();
                            //},
                            create: function (options) {
                                 
                                $bsyDiv[0].style.visibility = "visible";
                                var valuesendtoctrl = {
                                    searchkey: VIS.Utility.encodeText(options.data.models[0].searchkey),
                                    secname: VIS.Utility.encodeText(options.data.models[0].name),
                                    attributeID: selectedAttributeID
                                };

                                $.ajax({
                                    url: VIS.Application.contextUrl + "Attribute/SelectionSavemodel",
                                    type: 'Post',
                                    async: false,
                                    datatype: "Json",
                                    cache: false,
                                    data: valuesendtoctrl,
                                    success: function (data) {
                                       
                                        $getidfromattributeset = JSON.parse(data);

                                        var valuesendtoctrl1 = {
                                            searchkey: options.data.models[0].searchkey,
                                            name: options.data.models[0].name,
                                            attributevalueid: $getidfromattributeset
                                        };

                                        grdRows.push(valuesendtoctrl1);


                                         
                                        attrbuteSetObj.addatttvaluewithgrid(selectedAttributeID, (options.data.models[0].name), $getidfromattributeset);
                                        gridekendobtns();
                                        $self.cmboonclick();
                                        attrbuteSetObj.attributevallueonattributeclick(selectedAttributeID);


                                        //kendoGrd.data('kendoGrid').dataSource.read();
                                    },
                                    error: function (data) {
                                        $bsyDiv[0].style.visibility = "hidden";
                                    }
                                });
                                kendoGrd.data('kendoGrid').dataSource.read();
                                //kendoGrd.data('kendoGrid').dataSource.read();
                                //gridekendobtns();
                                //refreshFunction();


                                //gridekendobtns();
                                attrbuteSetObj.attributevallueonattributeclick(selectedAttributeID);
                                
                                



                                $bsyDiv[0].style.visibility = "hidden";
                            },
                            parameterMap: function (options, operation) {
                                $bsyDiv[0].style.visibility = "visible";
                                if (operation !== "read" && options.models) {
                                    return { models: kendo.stringify(options.models) };
                                    $bsyDiv[0].style.visibility = "hidden";
                                }
                            }
                        }
                    });

                    // var dSource=  dataSource.read()


                    // while (!flagg)
                    // {
                    //     ;
                    //  }


                    if (kendoGrd) {
                       
                        kendoGrd.data("kendoGrid").destroy();
                        //kendoGrd.remove();
                        kendoGrd = null;

                        $griddiv.empty();

                        //$bsyDiv[0].style.visibility = "hidden";
                        //return;
                        // kendoGrd.dataSource.read();
                    }
                    //else {
                    kendoGrd = $griddiv.kendoGrid({
                        dataSource: dataSource,
                        change: onChange,
                        pageable: true,
                        resizable: true,
                        scrollable: true,
                        //selectable: "multiple cell",
                        selectable: " ",                       
                        edit: function (e) {
                            e.container.find("input[name=name]").attr("maxlength", 28);
                        },                      


                        dataBound: function (e) {
                            //$griddiv.find('.k-grid-edit').css({ 'min-width': '10px', 'width': '27px', 'height': '27px' });                           
                            //$griddiv.find('.k-grid-delete').css({ 'min-width': '10px', 'width': '27px', 'height': '27px' });
                            //$griddiv.find('.k-grid-cancel').css({ 'min-width': '10px', 'width': '27px', 'height': '27px' });
                            //$griddiv.find('.k-grid-update').css({ 'min-width': '10px', 'width': '27px', 'height': '27px' });

                            $griddiv.find('.k-grid-edit').addClass("VA005-gridallbtn");
                            $griddiv.find('.k-grid-delete').addClass("VA005-gridallbtn");
                            $griddiv.find('.k-grid-cancel').addClass("VA005-gridallbtn");
                            $griddiv.find('.k-grid-update').addClass("VA005-gridallbtn");

                            $griddiv.find('.k-grid-header-wrap').css('position', 'inherit');
                            $griddiv.find('.k-grid-pager').css('position', 'inherit');

                            if (ismobile) {
                                //$griddiv.find('.k-pager-numbers').css({ 'position': 'inherit', 'float': 'left' });
                                //$griddiv.find('.k-pager-nav').css({ 'position': 'inherit', 'float': 'left' });
                                //$($griddiv.find('.k-pager-numbers').siblings()[2]).css('margin-left', '1px');
                                

                                $griddiv.find('.k-pager-numbers').css({ 'position': 'inherit', 'float': 'left', 'margin-top': '2px' });
                                $griddiv.find('.k-pager-nav').css({ 'position': 'inherit', 'float': 'left', 'margin-top': '2px' });
                                $($griddiv.find('.k-pager-numbers').siblings()[2]).css({'margin-left': '1px', 'margin-top': '2px'});


                                // $griddiv.find('.k-grid-content').css({ 'overflow': 'scroll','height':'100px' }); 

                                ////k-grid k-widget
                                //$griddiv.find('.k-widget').css({ 'width': '80%' });

                               // $griddiv.find('.k-grid-content').css({ 'height': '100px','z-index': 0 });

                                //css({ 'z-index': '999' });                               
                               
                                $griddiv.find('.k-grid-content').css({ 'height': '100px'});
                                $griddiv.find('.k-pager-nav').css({ 'width': '1px' });
                                $griddiv.find('.k-pager-info').css({ 'font-size': '11px', 'padding': '0px' });
                                $griddiv.find('.k-pager-nav').css({ 'width': '1px' });
                                $griddiv.find('.k-label').css({ 'font-size': '10px', 'padding': '0px', 'margin-top': '4px' });
                                



                                //k-grid-content
                                // $griddiv.find('.k-pager-first').css({ 'position': 'inherit', 'float': 'left' });
                                //$griddiv.find('.k-pager-nav').css({ 'margin-left': '2px'});

                            }
                            else {
                                $griddiv.find('.k-pager-numbers').css({ 'position': 'inherit'});
                                $griddiv.find('.k-pager-wrap').css('position', 'inherit');
                            }
                            //$griddiv.find('.k-pager-nav').css('float', 'inherit');
                            
                            
                            $griddiv.find('.k-widget').css('position', 'inherit'); 
                            $griddiv.find('.k-pager-nav').css('position', 'inherit');
                            $griddiv.find('.k-grid-content').css('position', 'inherit');
                           

                            //$griddiv.find('.k-grid-delete').addClass('glyphicon glyphicon-trash');

                            $griddiv.find('.k-grid-add').on('click', function () {
                                window.setTimeout(function () {
                                    //$griddiv.find('.k-grid-cancel').css({ 'min-width': '10px', 'width': '27px', 'height': '27px' });
                                    //$griddiv.find('.k-grid-update').css({ 'min-width': '10px', 'width': '27px', 'height': '27px' });

                                    $griddiv.find('.k-grid-cancel').addClass("VA005-gridallbtn");
                                    $griddiv.find('.k-grid-update').addClass("VA005-gridallbtn");
                                }, 1);
                            });
                            $griddiv.find('div').find('.deltemp').on("click", function (e) {
                               
                                //alert("hello");
                                // kendoGrd.data('kendoGrid').dataSource.destroy();
                                //var entityGrid = $griddiv.data("kendoGrid");       
                                
                                selectedCellID = $($($(this.parentElement).siblings()).get(2)).html();
                               // var msg = "" + VIS.Msg.getMsg("VA005_DeleteIt") + "";

                               // var r = VIS.ADialog.ask(msg);
                                VIS.ADialog.confirm("VA005_DeleteIt", true, "", "Confirm", function (result) {
                                    if (result == true) {
                                        //$bsyDiv[0].style.visibility = "visible";
                                        deletefromgrid(selectedCellID);

                                    }
                                    else {

                                        return;
                                    }
                                });

                                //deletefromgrid(selectedCellID);
                                
                            });
                            //================
                            //$griddiv.find('.k-grid-toolbar').find('.k-button-icontext').on("click", function () {
                                


                            //});
                            //================

                            //$($($griddiv.find('.deltemp')).parent()).css('min-width', '10%');
                        },
                        height: height,
                        toolbar: ["create"],
                        columns: [
                            { field: "name", title: VIS.Msg.getMsg("Name"),format: "{0:c}" },
                            { field: "searchkey", title: VIS.Msg.getMsg("Value") },
                            //{ field: "attributevalueid", hidden: true, title: VIS.Msg.getMsg("attributevalueid"), width: "60px" },
                            { field: "attributevalueid", hidden: true, title: VIS.Msg.getMsg("attributevalueid") },
                          //  { field: " ", title: " ", width: "60px" },
                           // { command: ["edit", "destroy"], title: "&nbsp;", width: "120px" }],
                            {
                                command: [{
                                    name: "edit", text: { edit: " ", cancel: " ", update: " " },
                                    //iconClass: "fa fa-trash",
                                    click: function (e) {
                                        //    //alert("clicked");
                                        //    var btns=$griddiv.find('.')
                                        $griddiv.find('.k-grid-update').css({ 'min-width': '10px', 'width': '27px', 'height': '27px' });
                                        $griddiv.find('.k-grid-cancel').css({ 'min-width': '10px', 'width': '27px', 'height': '27px' });

                                        $griddiv.find('.k-grid-cancel').on('click', function () {
                                            window.setTimeout(function () {
                                                $griddiv.find('.k-grid-edit').css({ 'min-width': '10px', 'width': '27px', 'height': '27px' });
                                                $griddiv.find('.k-grid-delete').css({ 'min-width': '10px', 'width': '27px', 'height': '27px' });

                                                $griddiv.find('div').find('.deltemp').on("click", function (e) {

                                                    //alert("hello");
                                                    // kendoGrd.data('kendoGrid').dataSource.destroy();
                                                    //var entityGrid = $griddiv.data("kendoGrid");       

                                                    selectedCellID = $($($(this.parentElement).siblings()).get(2)).html();
                                                    var msg = "" + VIS.Msg.getMsg("VA005_DeleteIt") + "";

                                                    var r = VIS.ADialog.ask(msg);

                                                    if (r == true) {
                                                        //$bsyDiv[0].style.visibility = "visible";
                                                        deletefromgrid(selectedCellID);

                                                    }
                                                    else {

                                                        return;
                                                    }

                                                    //deletefromgrid(selectedCellID);

                                                });





                                            }, 1);

                                            //======================


                                            //======================



                                        });

                                    },
                                },
                                {
                                    //name: "Delete", text: " ", template: "<div  class='deltemp' style='float:right'><a  style='min-width: 10px; width: 27px; height: 27px;' class='k-button k-button-icontext'><span  class='k-icon k-delete'></span></a></div>"//,
                                    name: "Delete", text: " ", template: "<div  class='deltemp' style='float:right'><a  class='VA005-gridallbtn k-button k-button-icontext'><span  class='k-icon k-delete'></span></a></div>"//,
                                    ////  name: "Delete", text: " ", template: kendo.template(grideidget()),
                                    ////iconClass: "fa fa-trash",
                                    //click: function (e) {
                                    //   
                                    //    alert("clicked");
                                    //}
                                }], title: "&nbsp;", width: 85
                            }],
                        editable: "inline"
                    });
                    //  }
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });


            //kendoGrd.find('.k-button').css({ 'min-width': '10px' });

            $bsyDiv[0].style.visibility = "hidden";
        };


        //grideidget();
        //function grideidget()
        //{
        //    var grideidget1 = null;
        //    grideidget1= $(" <script type='text/x-kendo-template' id='templateid'>" +
        //                 "<div data-id=#= attributevalueid #  class='deltemp' style='float:right'><a  data-id=#= attributevalueid #   style='min-width: 10px; width: 27px; height: 27px;' class='k-button k-button-icontext'><span  data-id=#= attributevalueid #   class='k-icon k-delete'></span></a></div>" +
        //               "</script>");
        //    return grideidget1.html();
        //}

        var selectedCellID = 0;
        function onChange(e) {
           
            //var selected = $.map(this.select(), function (item) {
            //    alert('adasdsa');
            //});

            selectedCellID = $(kendoGrd.data('kendoGrid').select().closest("tr").find('td').get(2)).html();
        };



        function gridekendoattributesvalule() {

            gridekendobtns();
        }


        this.cmboonclick = function () {
            debugger;
            if ($cmbselect.val() == "L") {
                // $btnaddselect.prop("disabled", false);
                gridekendobtns();
                $griddiv.show();
                $opacitydiv.hide();
            }
            else {
                //$btnaddselect.prop("disabled", true);
                $griddiv.hide();
                $opacitydiv.show();
                $attributevaluedivleftdragdrop.empty();
            }
            $self.$cmbovaluecheck = $cmbselect.val();
        }

        //=========================================================================================================================


        //this.flagcondition = function () 
        //{
        //   
        //    if (flag == 1) {
        //        flag = 1;
        //    }
        //    else
        //    {
        //        flag = 2;
        //    }
        //}


        //function createBusyIndicator() {
        //    $bsyDiv = $("<div>");
        //    $bsyDiv.css("position", "absolute");
        //    $bsyDiv.css("bottom", "0");
        //    $bsyDiv.css("background", "url('" + VIS.Application.contextUrl + "Areas/VIS/Images/busy.gif') no-repeat");
        //    $bsyDiv.css("background-position", "center center");
        //    $bsyDiv.css("width", "98%");
        //    $bsyDiv.css("height", "98%");
        //    $bsyDiv.css('text-align', 'center');
        //    $bsyDiv.css('z-index', '1000');
        //    $bsyDiv[0].style.visibility = "visible";
        //    $root.append($bsyDiv);
        //};


        //*** Get field length....
        function filedlength() {

            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "Attribute/GetFieldLength", { "TableID": 562, "COLUMNNAME": "'Name','Description'" }, LengthCallBack);



            //var tbID = "select ad_table_id from ad_table where tablename='M_Attribute'";
            //var sqlname = "SELECT fieldlength, columnname FROM ad_column WHERE ad_table_id=(" + tbID + ") AND COLUMNNAME='Name' AND isActive ='Y'";
            //var ds = VIS.DB.executeDataSet(sqlname.toString(), null);
            //columnLength = VIS.Utility.Util.getValueOfInt(ds.tables[0].rows[0].cells.fieldlength);
           // $textname.attr("maxlength", columnLength);

            //var sqldes = "SELECT fieldlength FROM ad_column WHERE ad_table_id=(" + tbID + ") AND COLUMNNAME='Description' AND isActive ='Y'";
            //var ds1 = VIS.DB.executeDataSet(sqldes.toString(), null);
            //columndescLength = VIS.Utility.Util.getValueOfInt(ds1.tables[0].rows[0].cells.fieldlength);
            //$textdesc.attr("maxlength", columndescLength);

            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "Attribute/GetField", { "TableID": 558, "COLUMNNAME": "'Name','Value'" }, FieldCallBack);


           // var tbattributevalueid = "select ad_table_id from ad_table where tablename='M_AttributeValue'";
            //var sql1 = "SELECT fieldlength FROM ad_column WHERE ad_table_id=(" + tbattributevalueid + ") AND  COLUMNNAME  ='Name' AND isActive ='Y'";
            //var ds1 = VIS.DB.executeDataSet(sql1.toString(), null);
            //columnlengthattributevalue = VIS.Utility.Util.getValueOfInt(ds1.tables[0].rows[0].cells.fieldlength);
            //$textnamediv.attr("maxlength", columnlengthattributevalue);

            //var sql1 = "SELECT fieldlength FROM ad_column WHERE ad_table_id=(" + tbattributevalueid + ") AND  COLUMNNAME  ='Value' AND isActive ='Y'";
            //var ds1 = VIS.DB.executeDataSet(sql1.toString(), null);
            //columnvalueLength = VIS.Utility.Util.getValueOfInt(ds1.tables[0].rows[0].cells.fieldlength);
            //$textnamediv.attr("maxlength", columnvalueLength);

            //sql = VIS.MRole.addAccessSQL(sql, "ad_column", true, true);
        };
        function LengthCallBack(dr) {
            if (dr != null) {
                for (var i = 0; i < dr.length; i++) {
                    if (dr[i].COLUMNNAME == 'Name') {
                        $textname.attr("maxlength", dr[i].fieldlength);
                    }
                    else if (dr[i].COLUMNNAME == 'Description') {
                        $textdesc.attr("maxlength", dr[i].fieldlength);
                    } 
                }
            }
        }
        function FieldCallBack(dr) {
            if (dr != null) {
                for (var i = 0; i < dr.length; i++) {
                    if (dr[i].COLUMNNAME == 'Name') {
                        $textnamediv.attr("maxlength", dr[i].fieldlength);
                    }
                    else if (dr[i].COLUMNNAME == 'Value') {
                        $textnamediv.attr("maxlength", dr[i].fieldlength);
                    }
                }
            }
        }
        //*** event call..
        function eventCall()
        {
            //$self.$cmbovaluecheck = $cmbselect.val();
           
            $chkboxmandatoryfield.on("click", function () {
                 
                $self.$getvalueonchange = true;
            });

            $chkboxisactivefield.on("click", function () {
                debugger;
                $self.$getvalueonchange = true;
                if ($chkboxisactivefield.is(":checked")) {
                    $self.$activechkbox = true;
                }
                else {
                    $self.$activechkbox = false;
                }                
            });





            $cmbselect.on("change", function ()
            {
                 
               
                if ($cmbselect.val() == "L")
                {                 
                    gridekendobtns();
                    $griddiv.show();
                    $opacitydiv.hide();
                    // $btnaddselect.prop("disabled", false);
                }
                else {
                    // $btnaddselect.prop("disabled", true);
                    // $maindiv2.hide();
                    $griddiv.hide();
                    $opacitydiv.show();
                    $attributevaluedivleftdragdrop.empty();
                }
                $self.$getvalueonchange = true;
               
                $self.$activechkbox = true;

                if ($chkboxisactivefield.is(":checked")) {
                    $self.$activechkbox = true;
                }
                else {
                    $self.$activechkbox = false;
                }


                $self.$cmbovaluecheck = $cmbselect.val();
                
            });

            $btndiv.on("click", function () {

                if ($textnamediv.val().trim().length > 0) {
                    saveAttributeType();
                }
            });

            $textname.on("change", function () {
                 
                $self.$getvalueonchange = true;
                //$self.$activechkbox = false;
                if ($chkboxisactivefield.is(":checked")) {
                    $self.$activechkbox = true;
                }
                else {
                    $self.$activechkbox = false;
                }

                if ($textname.val().trim().length <= 0) {
                    $textname.css("background-color", "pink");
                }
                else {
                    $textname.css("background-color", "white");
                }
                //keypress
            });
            

            $textdesc.on("change", function () {
                 
                $self.$getvalueonchange = true;

                if ($chkboxisactivefield.is(":checked")) {
                    $self.$activechkbox = true;
                }
                else {
                    $self.$activechkbox = false;
                }

                //$self.$activechkbox = false;
             
            });


            $textname.on("keydown", function () {
                 
                $self.$getvalueonchange = true;
                if ($chkboxisactivefield.is(":checked")) {
                    $self.$activechkbox = true;
                }
                else {
                    $self.$activechkbox = false;
                }
               // $self.$activechkbox = false;
            });
            $textdesc.on("keydown", function () {
                 
                $self.$getvalueonchange = true;
                if ($chkboxisactivefield.is(":checked")) {
                    $self.$activechkbox = true;
                }
                else {
                    $self.$activechkbox = false;
                }
               // $self.$activechkbox = false;

            });



            $textnamediv.on("change", function () {
                if ($textnamediv.val().trim().length <= 0) {
                    $textnamediv.css("background-color", "pink");
                }
                else {
                    $textnamediv.css("background-color", "white");
                }
            });


            $btnokmiddiv.on("click", function () {

                if ($textnamediv.val().trim().length > 0 && $textname.val().trim().length > 0) {
                    saveAttribute();
                    $textsearchdiv.val("");
                    $textnamediv.val("");
                    attributeAppendDiv();
                    //saveAttributeType();
                }
                else {
                    VIS.ADialog.info("VA005_NameMandatory");
                    //alert(VIS.Msg.getMsg("VA005_NameMandatory"));
                    return false;
                }
                //$root.css("display", "none");
                $treeView.css("display", "inherit");

                flag = 1;

            });

            $btncancelmiddiv.on("click", function () {

                //$root.css("display", "none");
                // $treeView.css("display", "inherit");
                $textsearchdiv.val("");
                $textnamediv.val("");
                $middivforattributevalue.animate({ width: "0%" });
                $middivforattributevalue.css("display", "none");
                $root.animate({ width: "50%" });
                $leftboxdivwidth.animate({ width: "50%" });
                attributeAppendDiv();
                $treeView.animate({ width: "50%" });
                flag = 1;
            });



            $btnok.on("click", function () {
               
                 
                if ($self.$getvalueonchange == true)
                {                   

                    if ($textname.val().trim().length > 0) {
                        window.setTimeout(function () {
                            objAttrSet.updateSelectedAttribute($textname.val());

                            if ($chkboxisactivefield.is(":checked"))
                            {
                                debugger;
                               // $self.$getcheckboxclickcolor = true;
                                objAttrSet.updatetextcolor($textname.val(),true);
                            }
                            else
                            {
                                //$self.$getcheckboxclickcolor = false;
                                objAttrSet.updatetextcolor($textname.val(),false);
                            }
                        
                            saveAttribute();

                            if (objAttrSet.$idgetforattribute)
                            {
                                 
                                attributeAppendDiv();
                            }


                            $self.$getvalueonchange = false;
                            $self.$activechkbox = false;
                            refreshFunction();
                            $(".VA005-attributedivstatic").parent().attr("VA005_plusimg", 'N');
                        }, 200);
                    }
                    else {
                        VIS.ADialog.info("VA005_FieldMandatory");
                        return false;
                    }
                    if ($cmbselect.val() != "L") {
                        $attributevaluedivleftdragdrop.empty();
                    }
                    else
                    {
                        attrbuteSetObj.attributevallueonattributeclick(selectedAttributeID);
                    }
                    if (refreshFunction) {
                        refreshFunction();
                    }

                    $root.css("display", "none");
                    $treeView.css("display", "inherit");                   
                }                                   
            });

            $btncancel.on("click", function () {
             
                $bsyDiv[0].style.visibility = "visible";
                //$maindiv2.hide();
                $root.css("display", "none");
                $treeView.css("display", "inherit");

                $middivforattributevalue.animate({ width: "0%" });
                $middivforattributevalue.css("display", "none");
                $treeView.animate({ width: "50%" });

                $leftboxdivwidth.animate({ width: "50%" });
                $root.animate({ width: "50%" });

                // $undoattributeboxes.css("display", "none");
                // $editattributeboxes.css("display", "inherit");
                //flag = 1;
                //attributeAppendDiv();
                $(".VA005-attributedivstatic").parent().attr("VA005_plusimg", 'N');

                $self.$getvalueonchange = false;

               // $attributevaluedivleftdragdrop.empty();

                $bsyDiv[0].style.visibility = "hidden";
            });

            //$btnaddselect.on("click", function () {

            //    //$maindiv2.toggle();
            //    //$middivforattributevalue.animate({ width: "30%" });
            //    //$middivforattributevalue.css("display", "inherit");

            //    //$treeView.animate({ width: "30%" });
            //    // $leftboxdivwidth.animate({ width: "40%" });
            //    //  $root.animate({ width: "30%" });

            //    gridekendoattributesvalule();

            //});

        };

        this.cleartext = function () {
            $textname.val("");
            $textdesc.val("");
            $attributevaluedivleftdragdrop.empty();
            //$griddiv.hide();
         

            //$textnamediv.trigger("change");
            if ($textname.val().trim().length <= 0) {
                $textname.css("background-color", "pink");
            }
            else {
                $textname.css("background-color", "white");
            }

            $griddiv.hide();
            $cmbselect.attr('disabled', true);

            $chkboxmandatoryfield.attr('disabled', true);
            $chkboxisactivefield.attr('disabled', true);

//            $griddiv.attr("disabled", "disabled");
            
            //$textname.focus();
        }


        //*** Design for attribute dailog..
        function attributeDialogDeign() {
            $maindiv.append($('<div>').append($topdivforname));
            $maindiv.append($('<div class="input-group vis-input-wrap">').append($('<div class="vis-control-wrap">').append($textname).append('<label>' + VIS.Msg.getMsg("Name") + '</label>')));
            $maindiv.append($('<div class="input-group vis-input-wrap">').append($('<div class="vis-control-wrap">').append($textdesc).append('<label>' + VIS.Msg.getMsg("Description") + '</label>')));
            //$maindiv.append($('<div>').append('<label>' + VIS.Msg.getMsg("VA005_AttributeType") + '</label>')).append($('<div style="float:left;width:100%">').append($cmbselect).append($btnaddselect));
            $maindiv.append($('<div class="input-group vis-input-wrap">').append($('<div class="vis-control-wrap">').append($cmbselect).append('<label>' + VIS.Msg.getMsg("VA005_AttributeType") + '</label>')));


            //$maindiv.append($('<div>').append('<label style="margin-bottom:0">' + VIS.Msg.getMsg("VA005_AttributeType") + '</label>'));

           

            $maindiv.append($('<div class="va005-new-att-chkboxWrap">').append($chkboxmandatoryfield).append('<label>' + VIS.Msg.getMsg("VA005_Mandatory") + '</label>').append($chkboxisactivefield).append('<label style="margin-bottom:0">' + VIS.Msg.getMsg("VA005_Active") + '</label>'));

            $maindiv.append($griddiv);


            $maindiv.append($('<div style="float: right;">').append($btnok).append($btncancel));
            ////$maindiv2.append($('<div>').append('<label>' + VIS.Msg.getMsg("VA005_MakeSelectionList") + '</label>'));
            //$maindiv2.append($('<div>').append($attributenameh4));
            //$maindiv2.append($('<div>').append('<label>' + VIS.Msg.getMsg('VA005_SearchKey') + '</label>')).append($('<div>').append($textsearchdiv));
            //$maindiv2.append($('<div>').append('<label>' + VIS.Msg.getMsg('Name') + '</label>')).append($('<div>').append($textnamediv));

            //$maindiv2.append($('<div style="float:right">').append($btnokmiddiv).append($btncancelmiddiv));
            //.append($('<div>').append($btndiv))

            //$root.append($maindiv).append($maindiv2);

            $root.append($maindiv);


            //$root2.append($maindiv).append($maindiv2);
            $root2.append($maindiv);
            $textname.focus();
        };





        //** edit attribute value, ID (nodID)  com from attribute listing form & loadtree data is a function
        this.editAttribute = function (id, loadTreeData) {
             
            refreshFunction = loadTreeData;
            selectedAttributeID = id;
            //var sql = "SELECT a.name,  a.description,a.isactive, a.ismandatory,  a.ATTRIBUTEVALUETYPE,  m.name,  m.value  FROM m_attribute a LEFT OUTER  JOIN m_attributevalue m  ON a.m_attribute_id  =m.m_attribute_id WHERE a.m_attribute_id =" + id + "   AND a.isactive  ='Y'";
           // var sql = "SELECT a.name,  a.description,a.isactive, a.ismandatory,  a.ATTRIBUTEVALUETYPE,  m.name,  m.value  FROM m_attribute a LEFT OUTER  JOIN m_attributevalue m  ON a.m_attribute_id  =m.m_attribute_id WHERE a.m_attribute_id =" + id + "";

            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/Attribute/LoadAttribute", { "Id": id }, AttributeCallBack);
            // var sql = "SELECT name,description, ATTRIBUTEVALUETYPE,isactive, ismandatory FROM m_attribute WHERE m_attribute_id=" + id + " AND isactive ='Y'";

            //var ds = VIS.DB.executeDataSet(sql, null, null);
            //if (ds != null) {
               
            //    if (ds.tables[0].rows.length > 0) {
            //        for (var i = 0; i < ds.tables[0].rows.length ; i++) {
            //            $textname.val(  ds.tables[0].rows[0].cells["name"].toString().trim());
            //            if (ds.tables[0].rows[0].cells["description"] != null) {
            //                $textdesc.val(ds.tables[0].rows[0].cells["description"].toString().trim());
            //            }
            //            if (ds.tables[0].rows[0].cells["attributevaluetype"] != null) {
            //                $cmbselect.val(ds.tables[0].rows[0].cells["attributevaluetype"]);
            //            }
                        
            //            if (ds.tables[0].rows[0].cells["isactive"] == 'Y') {
            //                $chkboxisactivefield.val(ds.tables[0].rows[0].cells["isactive"]);
            //                $chkboxisactivefield.prop("checked", true);
            //            }
            //            else
            //            {
            //                $chkboxisactivefield.prop("checked", false);
            //            }

            //            if (ds.tables[0].rows[0].cells["ismandatory"] == 'Y') {
            //                $chkboxmandatoryfield.val(ds.tables[0].rows[0].cells["ismandatory"]);
            //                $chkboxmandatoryfield.prop("checked", true);
            //            }
            //            else
            //            {
            //                $chkboxmandatoryfield.prop("checked", false);
            //            }

            //            // if ($maindiv2.is('visible'))
            //            //{
            //            //    $textsearchdiv.val(ds.tables[0].rows[0].cells["value"]);
            //            //    $textnamediv.val(ds.tables[0].rows[0].cells["name1"]);
            //            //}
            //        }
            //    }
            //    gridekendoattributesvalule();
            //}
            if ($textname.val().trim().length <= 0) {
                $textname.css("background-color", "pink");
            }
            else {
                $textname.css("background-color", "white");
            }
            //$self.$cmbovaluecheck = $cmbselect.val();
            $cmbselect.attr('disabled', false);
            $chkboxmandatoryfield.attr('disabled', false);
            $chkboxisactivefield.attr('disabled', false);
            $griddiv.show();
        };
        function AttributeCallBack(dr) {
            
                if (dr != null && dr.length > 0) {
                    for (var i = 0; i < dr.length; i++) {
                        $textname.val(dr[i].name.toString().trim());
                        if (dr[i].description != null) {
                            $textdesc.val(dr[i].description.toString().trim());
                        }
                        if (dr[i].attributevaluetype != null) {
                            $cmbselect.val(dr[i].attributevaluetype);
                        }

                        if (dr[i].isactive == 'Y') {
                            $chkboxisactivefield.val(dr[i].isactive);
                            $chkboxisactivefield.prop("checked", true);
                        }
                        else {
                            $chkboxisactivefield.prop("checked", false);
                        }

                        if (dr[i].ismandatory == 'Y') {
                            $chkboxmandatoryfield.val(dr[i].ismandatory);
                            $chkboxmandatoryfield.prop("checked", true);
                        }
                        else {
                            $chkboxmandatoryfield.prop("checked", false);
                        }  
                    }
                }
                gridekendoattributesvalule();
                    
        };


        //this.show = function () {
        //               
        //    //eventCall();
        //    $root.css("display", "block");

        //    //var createTab = new VIS.ChildDialog();
        //    //createTab.setHeight(330);
        //    //createTab.setWidth(450);
        //    //createTab.setEnableResize(false);
        //    //createTab.setTitle(VIS.Msg.getMsg('VA005_AddNewAttributeValue'));
        //    //createTab.setModal(true);
        //    //createTab.setContent($root);
        //    //createTab.show();

        //    //createTab.onOkClick = function (e) 
        //{
        //    //    if ($textname.val().trim().length > 0) {
        //    //        saveAttribute();
        //    //    }
        //    //    else
        //    //    {
        //    //        alert("Field Mandatory");
        //    //        return false;
        //    //    }
        //    //    if ($textnamediv.val().trim().length > 0) {
        //    //        saveAttributeType();
        //    //    }
        //    //    else {
        //    //        alert("Field Mandatory");
        //    //        return false;
        //    //    }
        //    //    if (refreshFunction) {
        //    //        refreshFunction();
        //    //    }
        //    //};
        //    //createTab.onCancelClick = function () {
        //    //    $maindiv2.hide();
        //    //    disposeComponent();
        //    //};
        //};

        this.saveAttributedata = function () {
            saveAttribute();
        }

        this.onchangesaveattribute = function () {
            saveAttribute();
        }

        function saveAttribute()
        {
                      

            $bsyDiv[0].style.visibility = "visible";
            if ($chkboxmandatoryfield.is(":checked")) {
                mandatory = true;
            }
            else
            {
                mandatory = false;
            }
            if ($chkboxisactivefield.is(":checked")) {
                isactivefield = true;
            }
            else {
                isactivefield = false;
            }

            var valuesendtoctrl = {
                name: VIS.Utility.encodeText($textname.val().toString().trim()),
                description: VIS.Utility.encodeText($textdesc.val().toString().trim()),

                //name: $textname.val().toString().trim(),
                //description: $textdesc.val().toString().trim(),
                attributetype: $cmbselect.val(),
                mandatory: mandatory,
                isactivefield: isactivefield,
                istanceattribute:true,
                ID: selectedAttributeID
            };

            $.ajax({
                url: VIS.Application.contextUrl + "Attribute/SaveAttributemodel",
                type: 'Post',
                datatype: "Json",
                data: valuesendtoctrl,
                success: function (data) {
                    if (!attrbuteSetObj.isSaveOnItemFocus) {
                        selectedAttributeID = JSON.parse(data);
                    }

                    //if ($chkboxisactivefield.is(":checked")) {
                    //    //var val = $textname.val(value);
                    //    //val.css("color", "red");

                    //    var val = objAttrSet.updateSelectedAttribute($textname.val());

                    //    val.css("color", "red");
                    //}

                    // $bsyDiv[0].style.visibility = "hidden";
                    //$self.$textnametextvalue = $textname.val();
                },
                error: function (data) {
                    //alert(data)
                    console.log(data);
                    $bsyDiv[0].style.visibility = "hidden";
                },
            });
            $self.$textnametextvalue = $textname.val();
            $bsyDiv[0].style.visibility = "hidden";
            // attributeidgetonokclick = selectedAttributeID;
        };

        //***save attribute value..
        function saveAttributeType() {

            var valuesendtoctrl = {
                searchkey: VIS.Utility.encodeText($textsearchdiv.val().toString().trim()),
                secname: VIS.Utility.encodeText($textnamediv.val().toString().trim()),
                attributeID: selectedAttributeID,
            };

            $.ajax({
                url: VIS.Application.contextUrl + "Attribute/SelectionSavemodel",
                type: 'Post',
                async: false,
                datatype: "Json",
                cache: false,
                data: valuesendtoctrl,

                success: function (data) {

                },
                error: function (data) {

                },
            });
        };

        //*** Attribute Type combo box..
        function selectionList() {
            $cmbselect.empty();
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "VA005/Attribute/LoadSelectAttribute", "", SelectCallBack);

            //var sql = "SELECT a.name,a.value FROM ad_ref_list a INNER JOIN ad_reference b ON a.ad_reference_id=b.ad_reference_id where b.name='M_Attribute Value Type' and a.isactive='Y'";
           // var ds = VIS.DB.executeReader(sql.toString(), null);
            //if (ds != null) {
            //    var key, value = null;
            //    while (ds.read()) {
            //        value = ds.getString(0);
            //        key = ds.getString(1);
            //        $cmbselect.append($("<Option value=" + key + ">" + value + "</option>"));
            //    }
            //    ds.close();
            //}
        };
        function SelectCallBack(dr) {
            if (dr != null) {
                var Key, Name = null;
                for (var i = 0; i < dr.length; i++) {
                    Key = dr[i].Key;
                    Name = dr[i].Name;
                    $cmbselect.append($("<Option value=" + Key + ">" + Name + "</option>"));
                }
            }
        };

        this.initialize = function () {

        };
        this.getRoot = function () {
            return $root;
            //return $root2;
        }

        this.getRoot2 = function () {
            return $root2;
        }


        this.disposeComponent = function () {
            remove();
            selectedAttributeID = null;
            $maindiv = null;
            $textname = null;
            $textdesc = null;
            $cmbselect = null;
            //$btnaddselect = null;
            // $maindiv2 = null;
            $textsearchdiv = null;
            $textnamediv = null;
            $btndiv = null;
            newObject = null;
        };

        function remove() {
            $cmbselect.off("change");
            $btndiv.off("click");
            $textname.off("change");
            $textnamediv.off("change");
            //$btnaddselect.off("click");

            $maindiv.remove();
            $textname.remove();
            $textdesc.remove();
            $cmbselect.remove();
            //$btnaddselect.remove();
            //  $maindiv2.remove();
            $textsearchdiv.remove();
            $textnamediv.remove();
            $btndiv.remove();
            //newObject.remove();
        };

    };

    //attribute.prototype.init = function (windowno, frame) {
    //    this.windowno = windowno;
    //    this.frame = frame;
    //    var obj = this.initialize()
    //    this.frame.getcontentgrid().append(this.getroot());
    //}

    VA005.AForms = VA005.AForms || {};
    VA005.AForms.attribute = attribute;

})(VA005, jQuery);

