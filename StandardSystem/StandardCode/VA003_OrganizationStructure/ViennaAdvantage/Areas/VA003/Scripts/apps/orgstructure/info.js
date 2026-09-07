/********************************************************
 * Module Name    : VA003
 * Purpose        : Show info regarding buttons used in Organization Structure
 * Class Used     : 
 * Chronological Development
 * Karan         23 July 2015
 ******************************************************/

; (function (VA003, $) {

    VA003.OrgStructure.info = function (removeRoot) {
        var $root = $('<div class="VA003-infoRoot">')

        this.init = function () {
            createClose();
            //createFirstTabInfo();
            //createSecondtabInfo();
            //createThirdTabInfo();
        };

        function createClose() {
            //var str = '<ul class="VA003-topRight-icons VA003-infoRoot-CloseInfo"><li><a ><span style="height:16px;width:auto" class="glyphicon glyphicon-remove VA003-AddNode-infoCloseicon"></span></a></li></ul>';
            var str = '<ul class="VA003-topRight-icons VA003-infoRoot-CloseInfo"><li><a style="float: initial;padding:0px"><span style="height:29px;width:29px" class="VA003-AddNode-infoCloseicon"></span></a></li></ul>';
            $root.append($(str));

            $($root.find('.VA003-infoRoot-CloseInfo')).on("click", function ()
            {
                removeRoot();
            });
        }


        function createFirstTabInfo() {

            var $leftContainer = $('<div  class="VA003-infoRoot-leftDivContainer">');

            var $leftDiv = $('<div  class="VA003-infoRoot-leftDiv">');
            $root.append($leftContainer);

            var $leftdiv1 = $('<div style="overflow:auto">');
            var $leftdiv2 = $('<div style="overflow:auto">');
            var $leftdiv3 = $('<div style="overflow:auto">');
            var $leftdiv4 = $('<div style="overflow:auto">');
            var $leftdiv5 = $('<div style="overflow:auto">');

            $leftdiv1.append($('<div style="overflow: auto;display: inline;float: left;margin: 5px;position:relative;line-height:45px;text-align:center;"><img data-uid="1005338" style="background-color:#9c9c9c" class="VA003-infoRoot-icon" src="' + VIS.Application.contextUrl + 'Areas/VA003/Images/orgstr-org.png"></div>'));
            $leftdiv1.append($('<div style="overflow-y;margin-bottom:5px"><div style="overflow:auto"><span class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_SummaryNodes') + '</span></div><div style="overflow:auto"><span class="VA003-infoRoot-infos">'+VIS.Msg.getMsg('VA003_SumaryInfo')+'</span></div></div>'));

            $leftdiv2.append($('<div style="overflow: auto;display: inline;float: left;margin: 5px;position:relative;line-height:45px;text-align:center;"><img data-uid="1005338" style="background-color:#dc8a20"  class="VA003-infoRoot-icon" src="' + VIS.Application.contextUrl + 'Areas/VA003/Images/orgstr-legal-entity.png"></div>'));
            $leftdiv2.append($('<div style="overflow-y;margin-bottom:5px"><div style="overflow:auto"><span data-uid="1005338"  class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_LegalNodes') + '</span></div><div style="overflow:auto"><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_LegalInfo') + '</span></div></div>'));

            $leftdiv3.append($('<div style="overflow: auto;display: inline;float: left;margin: 5px;position:relative;line-height:45px;text-align:center;"><img data-uid="1005338" style="background-color:rgba(43, 174, 250, 0.78)"  class="VA003-infoRoot-icon" src="' + VIS.Application.contextUrl + 'Areas/VA003/Images/orgstr-store.png"></div>'));
            $leftdiv3.append($('<div style="overflow-y;margin-bottom:5px"><div style="overflow:auto"><span data-uid="1005338"  class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_OrgUnitsNodes') + '</span></div><div style="overflow:auto"><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_NonLegalInfo') + '</span></div></div>'));

            $leftdiv4.append($('<div style="overflow: auto;display: inline;float: left;margin: 5px;position:relative;line-height:45px;text-align:center;"><img data-uid="1005338" style="background-color:rgba(43, 174, 250, 0.78)"  class="VA003-infoRoot-icon" src="' + VIS.Application.contextUrl + 'Areas/VA003/Images/orgstr-edit.png"></div>'));
            $leftdiv4.append($('<div style="overflow-y;margin-bottom:5px"><div style="overflow:auto"><span data-uid="1005338"  class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('Edit') + '</span></div><div style="overflow:auto"><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_EditInfo') + '</span></div></div>'));

            $leftdiv5.append($('<div style="overflow: auto;display: inline;float: left;margin: 5px;position:relative;line-height:45px;text-align:center;"><img data-uid="1005338" style="background-color:rgba(43, 174, 250, 0.78)"  class="VA003-infoRoot-icon" src="' + VIS.Application.contextUrl + 'Areas/VA003/Images/folder-minus.png"></div>'));
            $leftdiv5.append($('<div style="overflow-y;margin-bottom:5px"><div style="overflow:auto"><span data-uid="1005338"  class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_ExpandTree') + '</span></div><div style="overflow:auto"><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_ExpandTreelInfo') + '</span></div></div>'));



            $leftDiv.append($leftdiv1).append($leftdiv2).append($leftdiv3).append($leftdiv4).append($leftdiv5);

            $leftContainer.append($leftDiv);
        };


        function createSecondtabInfo() {

            var $leftContainer = $('<div  class="VA003-infoRoot-leftDivContainer">');

            var $leftDiv = $('<div  class="VA003-infoRoot-leftDiv">');
            $leftDiv.append($('<div style="margin: 10px;margin-top: 0px;"><div style="overflow:auto"><span class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_OrgInfo') + '</span></div><div style="overflow:auto"><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_OrgnizationInfo') + '</span></div></div>'));
            
            $root.append($leftContainer);
            $leftContainer.append($leftDiv);
        };


        function createThirdTabInfo() {
            var $leftContainer = $('<div  class="VA003-infoRoot-leftDivContainer">');

            var $leftDiv = $('<div  class="VA003-infoRoot-leftDiv">');
            $leftDiv.append($('<div style="margin: 10px;margin-top: 0px;"><div style="overflow:auto"><span class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_ReportingHierarchy') + '</span></div><div style="overflow:auto"><span class="VA003-infoRoot-infos">' + VIS.Msg.getMsg('VA003_HieraryInfo') + '</span></div></div>'));
            //$leftDiv.append($('<div style="overflow-y;margin-bottom:5px"><div style="overflow:auto"><span data-uid="1005338"  class="VA003-infoRoot-Header" >' + VIS.Msg.getMsg('VA003_LegalNodes') + '</span></div><div style="overflow:auto"><span class="VA003-infoRoot-infos">this sis asdf asfa sfdgasi fgaispudft asfa</span></div></div>'));
            $root.append($leftContainer);
            $leftContainer.append($leftDiv);
        };



        this.getRoot = function () {
            return $root;
        };
    };

})(VA003, jQuery);