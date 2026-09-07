/********************************************************
 * Module Name    : VA003
 * Purpose        : Create Organization Structure
 * Class Used     : 
 * Chronological Development
 * Karan         9 July 2015
 ******************************************************/

; (function (VA003, $) {

    VA003.OrgStructure.AddNode = function (trID, orgStructure, parent_ID, nameLength, valueLength) {
        var treID = trID;
        var winNo = orgStructure.windowNo;
        var $root = $('<div class="vis-formouterwrpdiv">');
        var $topfields = $('<div style="width:100%" class="VA003-form-top-fields">');
        var ch = null;
        var self = this;
        var $btnOK = null;
        var $btnCancel = null;

        var $value = $('<input maxlength="' + valueLength + '" type="text" data-name="searchkey" placeholder=" " data-placeholder="">');
        var $name = $('<input class="vis-ev-col-mandatory" maxlength="' + nameLength + '"  type="text" data-name="name" placeholder=" " data-placeholder="">');
        // var $desc = $('<input type="text" data-name="desc">');


        function createDesign() {
            $topfields.append($('<div class="VA003-form-data input-group vis-input-wrap">').append($('<div class="vis-control-wrap">').append($value).append('<label>' + VIS.Msg.getMsg("SearchKey") + '</label>')));
            $topfields.append($('<div class="VA003-form-data input-group vis-input-wrap">').append($('<div class="vis-control-wrap">').append($name).append('<label>' + VIS.Msg.getMsg("Name") + '</label>')));
            // $topfields.append($('<div class="vis-os-form-data"></div>').append('<label>' + VIS.Msg.getMsg("Description") + '</label>').append($desc));


            $btnOK = $('<button class="VIS_Pref_btn-2"  style="margin-top:0px;margin-bottom:0px;margin-right:10px;margin-left:10px;">' + VIS.Msg.getMsg("OK") + '</button>');
            $btnCancel = $('<button class="VIS_Pref_btn-2"  style="margin-bottom:0px;margin-top:0px;">' + VIS.Msg.getMsg("Cancel") + '</button>');



            $root.append($topfields).append($btnCancel).append($btnOK);

            $btnOK.on("click", ok);
            $btnCancel.on("click", cancel);

        };

        function createBusyIndicator() {
            $bsyDiv = $("<div>");
            $bsyDiv.css("position", "absolute");
            $bsyDiv.css("bottom", "0");
            $bsyDiv.css("background", "url('" + VIS.Application.contextUrl + "Areas/VIS/Images/busy.gif') no-repeat");
            $bsyDiv.css("background-position", "center center");
            $bsyDiv.css("width", "97%");
            $bsyDiv.css("height", "98%");
            $bsyDiv.css('text-align', 'center');
            $bsyDiv.css('z-index', '1000');
            $bsyDiv[0].style.visibility = "hidden";
            $root.append($bsyDiv);
        };

        
        createBusyIndicator();

        function events() {


            $name.on("change", function () {
                if ($name.val().length > 0) {
                    //$name.css("background-color", "white");
                    $name.removeClass('vis-ev-col-mandatory')
                }
                else {
                    //$name.css("background-color", "#ffb6c1");
                    $name.addClass('vis-ev-col-mandatory');
                }
            });

            //$value.on("change", function () {
            //    if ($value.val().length > 0) {
            //        $value.css("background-color", "white");
            //    }
            //    else {
            //        $value.css("background-color", "#ffb6c1");
            //    }
            //});


            //ch.onOkClick = function (e) {



            //  ch.onCancelClick = function () {


        };

        function ok(e) {

            if ($name.val().trim().length == 0) {
                VIS.ADialog.info("EnterName");
                e.preventDefault();
                return false;
            }

            //if ($value.val().trim().length == 0) {
            //    VIS.ADialog.info("VA003_EnterValue");
            //    e.preventDefault();
            //    return false;
            //}

            $bsyDiv[0].style.visibility = "visible";
            window.setTimeout(function () {
                $.ajax({
                    url: VIS.Application.contextUrl + "OrgStructure/AddOrgNode",
                    async: false,
                    data: { treeID: treID, name: VIS.Utility.encodeText($name.val().trim()), description: "", value: VIS.Utility.encodeText($value.val().trim()), windowNo: winNo, parentID: parent_ID },
                    success: function (result) {
                        var data = JSON.parse(result);
                        if (data.ErrorMsg != null && data.ErrorMsg.length > 0) {
                            VIS.ADialog.error(data.ErrorMsg);
                            $bsyDiv[0].style.visibility = "hidden";
                            e.preventDefault();
                            return false;
                        }
                        orgStructure.addNodeToTree(data.Tree);
                        $bsyDiv[0].style.visibility = "hidden";
                        ch.close();
                    },
                    error: function () {
                        $bsyDiv[0].style.visibility = "hidden";
                    }
                })
            }, 20);
        };

        function cancel(e) {
            ch.close();
        }


        this.show = function () {
            ch = new VIS.ChildDialog();
            ch.setContent($root);
            ch.setWidth(340);
            ch.setHeight(297);
            ch.setTitle(VIS.Msg.getMsg("VA003_AddNode"));
            ch.setModal(true);
            ch.onClose = function () {
                if (self.onClose) self.onClose();
                self.dispose();
            };
            ch.show();
            ch.hideButtons();
            events();
            createDesign();
            
        };

        this.dispose = function () {
            $root.remove();
            $root = null;
            $topfields.remove();
            $topfields = null;
            ch = null;
            self = null;

        };


    };



})(VA003, jQuery);