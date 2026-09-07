
/*
 ************************************************************************** 
* Purpose : upload Images in folder selected by user
 ************************************************************************
*/
; VA005 = window.VA005 || {};
(function (VA005, $) {


    VA005.uploadImage = function (folderID, windowID, tableID, recordID, productUl, pmObj) {
        var isCancle = false;
        var productMgtObj = pmObj;
        var ulProduct = productUl;
        var log = VIS.Logging.VLogger.getVLogger("UploadDocument");
        var self = this;
        var $uploadLayout = null;
        var $divUploadDiv = null;
        var $uploadControl = null;
        var $divDocumentInformation = null;
        var $tableDocumnetInformation = null;
        var $row1 = null;
        var $row1col1 = null;
        var $row1col2 = null;
        var $row1col3 = null;
        var $row1col4 = null;
        var $lbldocCategory = null;
        var $docCategory = null;
        var $lbldocVersion = null;
        var $docVersion = null;
        var $row2 = null;
        var $row2col1 = null;
        var $row2col2 = null;
        var $row2col3 = null;
        var $row2col4 = null;
        var $lblkeepVersion = null;
        var $keepVersion = null;
        var $lbldocFolder = null;
        var $docfolder = null;
        var $divbtnLayout = null;
        var $btnOK = null;
        var $btnCancel = null;
        var docCategory = "";
        var keepVersionOption = "";
        var folderOption = "";
        var $btnSubmit = null;
        var $form = null;
        var count = 0;
        var multipleFile = [];
        var folderInfo = {};
        var uploadDocumentDialog = null;
        var upload = null;
        var containerdiv = null;
        var lookup = null;
        var folderWhere = "";
        var btnRemove = null;
        var files = null;
        var $dialog = null;
        var totalfiles = null;
        var uploadedCount = 0;
        var uploadCount = 0;
        //*****Single Upload*****
        var $divSingleUploadInfo = null;
        var $divdocumentName = null;
        var $divMetaDataKeyword = null;
        var $divFileType = null;
        var $divMetaDataDescription = null;
        var $lblDocumentName = null;
        var $txtDocumentName = null;
        var $lblKeyword = null;
        var $txtKeyword = null;
        var $lblFileType = null;
        var $txtFileType = null;
        var $lblDescription = null;
        var $txtDescription = null;
        //************
        //********Common Doc Information**********
        var $divCommonDocInfo = null;
        var $divDocCategory = null;
        var $divDocVersion = null;
        var $divDocKeepVersion = null;
        var $divDocFolder = null;
        var folderKey = [];
        this.FileArray = [];
        var totalFiles = 0;
        load();
        //******************
        //********************
        //Initialize function which is called at the form load
        //*********************
        this.initialize = function () {
            customDesign();
            openUploadDocumentForm();
            kendoUpload();
            customKendoUpload();
            $docVersion.val("1.0");
            events();
        };

        function load() {
            customDesign();
            openUploadDocumentForm();
            kendoUpload();
            customKendoUpload();
            $docVersion.val("1.0");
            events();

        };
        //********************
        //This function called when size of window changed
        //*********************
        $(window).on("resize", function () {

        });

        //********************
        //Change the size of control resize of window
        //*********************
        function controlWidthOnResize() {
            if ($(window).width() > $(window).height()) {
                $dialog.css("width", $(window).width() * (68 / 100));
            }
            else {
                $dialog.css("width", $(window).width() * (60 / 100));
            }
            $docCategory.css("width", $(window).width() * (25 / 100));
            $docVersion.css("width", $(window).width() * (25 / 100));
            $keepVersion.css("width", $(window).width() * (25 / 100));
            $docfolder.getControl().css("width", $(window).width() * (23 / 100));
            docVersion.css("margin-left", $(window).width() * (5 / 100));
            $docfolder.getControl().css("width", $(window).width() * (5 / 100));
        };

        //********************
        //This function customs the design of form
        //*********************  
        function customDesign() {

            $uploadLayout = $("<div class='va005-imageupload-popupwrap'></div>");

            //$form = $('<form method="post" action=' + VIS.Application.contextUrl + 'VADMS/UploadDocument/SaveMultiple></form>');

            $divUploadDiv = $('<div class="demo-section k-header"></div>');
            $uploadControl = $('<input name="files" id="files" type="file" capture="camera" accept="image/*"/>');   //Upload Control
            $divUploadDiv.append($uploadControl);           

            //$form.append($divUploadDiv);

            $uploadLayout.append($divUploadDiv);

            //********** Single Upload Information******************
            $divSingleUploadInfo = $('<div"></div>');
            $divdocumentName = $('<div></div>');
            $divMetaDataKeyword = $('<div style="float:left"></div>');
            $divFileType = $('<div style="float:left;margin-left:60px;"></div>');
            $divMetaDataDescription = $('<div style="margin-top:80px;"></div>');

            $lblDocumentName = $('<p>' + VIS.Msg.getMsg("VADMS_DocName") + '<p>');
            $txtDocumentName = $('<input type="text" style="width:100%;"></input>');
            $lblKeyword = $('<p>' + VIS.Msg.getMsg("VADMS_MetaDataName") + '<p>');
            $txtKeyword = $('<input type="text" style="width:350px;"></input>');
            $lblFileType = $('<p>' + VIS.Msg.getMsg("VADMS_FileType") + '<p>');
            $txtFileType = $('<input type="text" style="width:350px;"></input>');
            $lblDescription = $('<p>' + VIS.Msg.getMsg("VADMS_Description") + '<p>');
            $txtDescription = $('<textarea type="text" style="width:100%;"></textarea>');

            $divdocumentName.append($lblDocumentName);
            $divdocumentName.append($txtDocumentName);
            $divMetaDataKeyword.append($lblKeyword);
            $divMetaDataKeyword.append($txtKeyword);
            $divFileType.append($lblFileType);
            $divFileType.append($txtFileType);
            $divMetaDataDescription.append($lblDescription);
            $divMetaDataDescription.append($txtDescription);

            $divSingleUploadInfo.append($divdocumentName);
            $divSingleUploadInfo.append($divMetaDataKeyword);
            $divSingleUploadInfo.append($divFileType);
            $divSingleUploadInfo.append($divMetaDataDescription);
            $divSingleUploadInfo.css("display", "none");
            //****************************


            //******Common Information for multiple and Single Upload******
            $divCommonDocInfo = $('<div></div>');
            $divDocCategory = $('<div style="margin-top:10px;"></div>');
            $divDocVersion = $('<div style="float:right;margin-left:428px;margin-top:-50px;"></div>');
            $divDocKeepVersion = $('<div style="margin-top:10px;"></div>');
            $divDocFolder = $('<div style="float:right;margin-top:-50px;"></div>');

            $lbldocCategory = $('<p style="display:none">' + VIS.Msg.getMsg("VADMS_DocType") + '<p>');
            // getDocumentCategory();   //Get Document Category List from database
            $docCategory = $('<select style="width:320px;display:none"></select>');  //Documnet Category
            $docCategory.append(docCategory);

            $lbldocVersion = $('<p style="display:none">' + VIS.Msg.getMsg("VADMS_Version") + '</p>');
            $docVersion = $('<input type="text" style="width:320px;display:none" maxlength="10" pattern="[0-9.]*"></input>'); //Version

            $lblkeepVersion = $('<p style="display:none">' + VIS.Msg.getMsg("VADMS_IskeepVersion") + '</p>');
            $keepVersion = $('<select style="width:320px;display:none"></select>');//Keep Version Dropdown
            //getKeepVersionList();  //Get Keep Version List From Database 
            //$keepVersion.append(keepVersionOption)

            $lbldocFolder = $('<p style="display:none">' + VIS.Msg.getMsg("VADMS_SaveIn") + '</p>');
            //*************Lookup for folder************
            //folderWhere = VADMS.getFolderWhere(false);
            containerdiv = $('<div style="display:none"></div>');
            //lookup = VIS.MLookupFactory.get(VIS.context, 0, 0, VIS.DisplayType.Search, "VADMS_Folder_ID", 0, false, folderWhere);
            //$docfolder = new VIS.Controls.VTextBoxButton("VADMS_Folder_ID", false, false, true, VIS.DisplayType.Search, lookup);



            //containerdiv.append($docfolder.getControl());
            //containerdiv.append($docfolder.getBtn(0));
            //containerdiv.append($docfolder.getBtn(1));
            //if (VADMS.Common.windowWidth <= VADMS.Common.IPadWidth) {
            //    $docfolder.getControl().css("width", "220px");
            //}
            //else {
            //    $docfolder.getControl().css("width", "250px");
            //}

            //if (folderID > 0) {
            //    $docfolder.setValue(folderID);
            //}
            //*******LookUp Folder Ends**************


            $divDocCategory.append($lbldocCategory);
            $divDocCategory.append($docCategory);

            $divDocVersion.append($lbldocVersion);
            $divDocVersion.append($docVersion);

            $divDocKeepVersion.append($lblkeepVersion);
            $divDocKeepVersion.append($keepVersion);

            $divDocFolder.append($lbldocFolder);
            $divDocFolder.append(containerdiv);

            $divCommonDocInfo.append($divDocCategory);
            $divCommonDocInfo.append($divDocVersion);
            $divCommonDocInfo.append($divDocKeepVersion);
            $divCommonDocInfo.append($divDocFolder);
            //*****************Common Information Ends********************
            $divDocumentInformation = $('<div class="VA005-doc_information"></div>');
            $divbtnLayout = $('<div class="vis-pull-right"></div>');
            $btnOK = $('<input type="submit" class="VA005-upload-buttons VA005-poupOkCancelBtn" value=' + VIS.Msg.getMsg("OK") + '></input>');
            $btnCancel = $('<input type="submit" class="VA005-poupOkCancelBtn" value=' + VIS.Msg.getMsg("Cancel") + '></input>');
            $divbtnLayout.append($btnCancel);
            $divbtnLayout.append($btnOK);
            $divDocumentInformation.append($divSingleUploadInfo);
            $divDocumentInformation.append($divCommonDocInfo);
            $divDocumentInformation.append($divbtnLayout);


            $uploadLayout.append($divDocumentInformation);
            //*******************To Change Template of Kendo Upload Control (Starts)*****************************

            var scrptText = '<script id="fileTemplate" type="text/x-kendo-template">' +
                ' <span class="k-progress"></span>' +
                '  <div class="file-wrapper" style="margin-top:5px;">' +
                ' <div class="VA005-attach-file-top"><a class="file-close-ico"></a></div>' +
                ' <div class="VA005-attach-file-content">' +
                '  <div class="VA005-file-icon #=addExtensionClass(files[0].extension)#"></div>' +

                ' <div class="VA005-attach-file-text" style="margin-bottom:10px"> ' +
                '  <h4 class="file-heading file-name-heading">#=CheckName(files[0].name)#</h4>' +

                '  <h4 class="file-heading file-size-heading">#=size# bytes</h4>' +
                '  <h4 class="file-heading file-size-heading" style="display: none;">#=getUID(files[0].uid)#</h4>' +
                '</div>' +
                '</div>' +
                '  <button type="button" class="k-upload-action"></button>' +
                ' </div>' +
                '  </script>';

            var scrptFunction = '<script>function addExtensionClass(extension) {' +
                'switch (extension) {' +
                '  case ".jpg":' +
                ' case ".img":' +
                ' case ".png":' +
                '  case ".gif":' +
                '     return "VA005-img-file";' +
                '  case ".doc":' +
                ' case ".docx":' +
                '     return "vadms-doc-file";' +
                ' case ".xls":' +
                ' case ".xlsx":' +
                '     return "vadms-xls-file";' +
                ' case ".pdf":' +
                '     return "vadms-pdf-file";' +
                ' case ".txt":' +
                '     return "vadms-text-file";' +
                'case ".zip":' +
                'case ".rar":' +
                '   return "vadms-zip-file";' +
                'default:' +
                '   return "vadms-default-file";' +
                '}' +
                '}</script>';




            var scrptFunctionNameSize = '<script> function CheckName(name) {' +
                'var lblFName = $("<p>");if (name.length > 17) {var shortName = name.toString().substr(0, 17);lblFName.append(shortName);' +
                ' var aFName = $("<a class=\'VIS_Pref_tooltip\'>").append(\'...\');var span = $("<span style=\'top:40px\'>");' +
                'span.append($("<img class=\'VIS_Pref_callout\'>").attr(\'src\', VIS.Application.contextUrl + \'Areas/VIS/Images/ccc.png\').append(\'ToolTip Text\'));' +
                'span.append($("<label class=\'VIS_Pref_Label_Font\'>").append(name));aFName.append(span);lblFName.append(aFName)' +
                '}' +
                'else {lblFName.append(name);}return lblFName.html();}</script>';

            var scrptFunctionUid = '<script>function getUID(uid) {' +
                'return uid;' +
                '}</script>';
            var $kendoTemplateScript = $(scrptText);
            $uploadLayout.append($kendoTemplateScript);
            $uploadLayout.append($(scrptFunction));
            $uploadLayout.append($(scrptFunctionNameSize));
            $uploadLayout.append($(scrptFunctionUid));

            //******************Kendo Upload Template Changed End**************
            $btnOK.prop("disabled", true);
        };

        //********************
        //This function defines the evenets of kendo Upload Control
        //*********************       
        function kendoUpload() {
            var url = VIS.Application.contextUrl + "VA005/ProductManagement/SaveImages";
            $("#files").kendoUpload({
                async: {
                    saveUrl: url,
                    autoUpload: true,
                    localization: {
                        select: "customSelect"
                    }


                },
                select: onSelect,
                upload: function (e) {

                    //if (e.files[0].size > 25000) {
                    //    totalfiles = totalfiles - 1;
                    //    e.preventDefault();
                    //    if (uploadCount > 0) {
                    //        $btnOK.prop("disabled", false);
                    //    }
                    //    alert(VIS.Msg.getMsg("FileSizeExceed"));

                    //    return;
                    //}
                    var validFileExtensions = [".jpg", ".jpeg", ".gif", ".png", ".img", ".bmp", ".tiff", ".bpg", ".thumb", ".psd"];
                    var extension = e.files[0].extension;
                    var ext = extension.toLowerCase();
                    //var ext=extension.split('.').pop();
                    if ($.inArray(ext, validFileExtensions) == -1) {
                        totalfiles = totalfiles - 1;
                        e.preventDefault();
                        if (uploadCount > 0) {
                            $btnOK.prop("disabled", false);
                        }
                        //alert(e.files[0].name + ' is not a image.');
                        //alert(e.files[0].name + " " + VIS.Msg.getMsg("VA005_NotAImage") + "");
                        return;
                    }

                    for (var j = 0; j < multipleFile.length; j++) {

                        if (e.files[0].name == multipleFile[j].name) {
                            totalfiles = totalfiles - 1;
                            e.preventDefault();
                            if (uploadCount > 0) {
                                $btnOK.prop("disabled", false);
                            }
                            return;
                        }
                    }
                    var key = (new Date()).getTime();
                    e.data = { folderKey: key };
                    customKendoUpload();
                    uploadCount++;
                    for (var i = 0; i < e.files.length; i++) {

                        multipleFile[count] = e.files[i];
                        $(multipleFile[count]).attr("folderKey", key);
                        folderKey[count] = key;
                        count++;
                    }
                    dynamicDesignForKendo(count);




                },
                success: onSuccess,
                cancel: onCancel,
                error: function (e) {
                    customKendoUpload();

                },
                progress: function (e) {

                    customKendoUpload();
                },
                template: kendo.template($('#fileTemplate').html())


            });
            upload = $("#files").data("kendoUpload");
            function addExtensionClass(extension) {

                switch (extension) {
                    case '.jpg':
                    case '.img':
                    case '.png':
                    case '.gif':
                        return "img-file";
                    case '.doc':
                    case '.docx':
                        return "doc-file";
                    case '.xls':
                    case '.xlsx':
                        return "xls-file";
                    case '.pdf':
                        return "pdf-file";
                    case '.zip':
                    case '.rar':
                        return "zip-file";
                    default:
                        return "default-file";
                }
            };
            function onSuccess(e) {

                uploadedCount += 1;
                customKendoUpload();
                btnRemove = $(".file-close-ico");
                divEvents();
                var file = e.files;
                if (count > 0) {
                    if (uploadedCount == totalfiles) {
                        $btnOK.prop("disabled", false);
                    }
                }



            };

            function dynamicDesignForKendo(count) {
                if (count > 3) {
                    $dialog.css("height", "380px");
                    // $dialog.css("height", $(window).height() - 394);
                    //if ($(window).width() > $(window).height()) {
                    //    $dialog.css("height", $(window).height()*(55/100));
                    //}
                    //else {
                    //    $dialog.css("height", $(window).height() * (51 / 100));
                    //}
                }
                if (count == 0) {
                    $dialog.css("height", "247px");
                    //$dialog.css("height", $(window).height() - 584);
                    //if ($(window).width() > $(window).height()) {
                    //    $dialog.css("height", $(window).height() * (32 / 100));
                    //}
                    //else {
                    //    $dialog.css("height", $(window).height() * (29 / 100));
                    //}
                    $(".k-upload-files").css("overflow", "hidden");
                }

                else if (count > 6) {
                    $(".k-upload-files").css("overflow", "scroll");
                    $dialog.css("height", "450px");
                }
                else if (count > 0 && count <= 3) {
                    $dialog.css("height", "280px");
                    $(".k-upload-files").css("overflow", "hidden");
                }


            };
            function onCancel(e) {
                totalfiles = totalfiles - 1;
                //dynamicDesignForKendo(totalfiles);
                for (var i = 0; i < multipleFile.length; i++) {
                    if (multipleFile[i].name == e.files[0].name) {
                        folderKey = jQuery.grep(folderKey, function (value) {
                            return value !== multipleFile[i].folderKey;
                        });
                        multipleFile.splice(i, 1);
                        count--;
                    }
                }
                dynamicDesignForKendo(count);
                //folderKey.splice(folderKey.length - 1, 1);
                if (uploadedCount == totalfiles) {
                    $btnOK.prop("disabled", false);
                }
            };
            function divEvents() {
                btnRemove.on("click", function () {

                    var c = null;


                    var uid = $($($($(this).parent()).parent()).children()[4]).text();
                    var divFInfo = $($($(this).parent()).parent()).parent();
                    divFInfo.css("display", "none");
                    var html = divFInfo.html();
                    if (count == 1) {
                        $($($($(this).parent()).parent()).parent()).parent().remove();
                    }
                    divFInfo.remove();
                    divFInfo.empty();
                    divFInfo = null;
                    if (html != null) {
                        if (count > 0) {
                            count--;
                        }
                    }
                    $.ajax({

                        async: false,
                        type: "GET",
                        url: VIS.Application.contextUrl + "VA005/ProductManagement/RemoveFile?uid=" + uid,
                        dataType: "Json",
                        success: function (data) {
                        },
                        error: function (e) {
                            console.log(e);
                        },

                    });
                    if (count == 0) {
                        $dialog.css("height", "252px");
                        $(".k-upload-files").css("height", "0px");
                        $btnOK.prop("disabled", true);
                        $(".k-upload-files").css("overflow", "hidden");
                        $($($("#files").parent()).parent()).children()[2].remove();
                    }
                    if (count > 3) {
                        $dialog.css("height", "380px");
                    }
                    else if (count > 0 && count <= 3) {
                        $dialog.css("height", "280px");
                        $(".k-upload-files").css("overflow", "hidden");
                    }
                    if (count > 6) {
                        $(".k-upload-files").css("overflow", "scroll");
                        $dialog.css("height", "453px");
                    }
                });
            };
            function onComplete(e) {

                $.ajax({
                    async: false,
                    type: "POST",
                    dataType: "json",
                    url: VIS.Application.contextUrl + "VADMS/UploadDocument/SaveMultiple",
                    data:
                    {
                        files: JSON.stringify(multipleFile)
                    },
                    success: function (data) {

                    },
                    error: function (e) {
                        console.log(e);
                    }
                });
            };
            function onUpload(e) {
                customKendoUpload();
                var files = e.files;

            };
            function onSelect(e) {

                var isBreak;
                files = e.files;
                $btnOK.prop("disabled", true);
                var recordUploaded = uploadedCount;
                uploadedCount = 0;
                totalfiles = files.length;
                customKendoUpload();
                //if (e.files.length > 10) {
                //    VIS.ADialog.info("VADMS_FileCountExceed");
                //    if (recordUploaded > 0) {
                //        $btnOK.prop("disabled", false);
                //    }
                //    e.preventDefault();
                //}

            };

        };

        //********************
        //Change the style of Kendo Upload
        //*********************
        function customKendoUpload() {

            $uploadLayout.find(".k-file").css("display", "-webkit-inline-box");
            // $(".k-file").css("border", "1px solid #eeeeee !Important");
            $uploadLayout.find(".k-file").css("width", "194px");

            $uploadLayout.find(".k-file").css("margin", "9px 11px 0px 0px");
            $uploadLayout.find(".k-file").css("float", "left");
            $uploadLayout.find(".k-button").css("color", "rgba(var(--v-c-on-primary), 1)");
            $uploadLayout.find(".k-button").css("background-color", "rgba(var(--v-c-primary), 1)");
            $uploadLayout.find(".k-button").css("background-image", "none");
            $uploadLayout.find(".k-button").css("border", "none");
            $uploadLayout.find(".k-button").css("margin-left", "-3px");
            //$(".demo-section k-header").css("height", "initial !Important");
            // $(".k-upload-files").css("overflow","scroll");
            // $(".k-upload-files").css("height", "200px");
            $uploadLayout.find(".k-header").css("background-color", "white");
            $uploadLayout.find(".k-upload-files").css("max-height", "200px");
            $uploadLayout.find(".k-upload-empty").css("border-width", "1px");
            $uploadLayout.find(".k-upload-files").css("overflow", "hidden");
            $uploadLayout.find(".k-upload-files").css("background", "white");
            $uploadLayout.find(".k-upload-files").css("margin-left", "10px");
            $uploadLayout.find(".k-upload-files.k-reset").css({ "margin-left": "0px", "overflow": "hidden", "max-height": "300px", "background": "white", "float": "left", "width": "100%", "box-sizing": "border-box", "padding": "0 10px" });
            $uploadLayout.find(".k-widget.k-upload.k-header").css({ "float": "left", "width": "100%" });
            $uploadLayout.find(".k-progress").css("background-color", "#ddffd0");
        }

        //********************
        //This function clears the list of files which has been canceled
        //*********************
        function clearFileList() {

            $.ajax({

                async: true,
                type: "POST",
                url: VIS.Application.contextUrl + "VA005/ProductManagement/Remove",
                dataType: "Json",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ fKey: folderKey, cancle: isCancle }),
                success: function (data) {

                },
                error: function (e) {
                    console.log(e);
                }
            });
            folderKey = [];
        };

        //********************
        //This function clears the list of files which has been canceled
        //*********************
        function removeFiles() {

            $.ajax({
                async: true,
                type: "POST",
                url: VIS.Application.contextUrl + "VADMS/UploadDocument/RemoveFile",
                dataType: "Json",
                success: function (data) {

                },
                error: function (e) {
                    console.log(e);
                }
            });
        };

        //********************
        //This function open the Upload Document Dialog
        //*********************
        function openUploadDocumentForm() {
            uploadDocumentDialog = new VIS.ChildDialog();
            uploadDocumentDialog.setContent($uploadLayout);
            uploadDocumentDialog.setHeight(230);
            uploadDocumentDialog.setWidth(705);
            uploadDocumentDialog.setTitle(VIS.Msg.getMsg("VA005_UploadImage"));
            uploadDocumentDialog.setEnableResize(false);
            uploadDocumentDialog.setModal(true);
            $dialog = uploadDocumentDialog.show();
            uploadDocumentDialog.hidebuttons();    //buttons are hidden because custom buttons are used in this dialog


        };

        //********************
        //Get Document Category
        //*********************
        //function getDocumentCategory() {
        //    var url = VIS.Application.contextUrl + "VADMS/UploadDocument/GetDocumentCategory";
        //    $.ajax({
        //        url: url,
        //        data:
        //            {
        //            },
        //        async: false,
        //        type: "GET",
        //        dataType: "json",
        //        success: function (data) {

        //            var category = JSON.parse(data);
        //            docCategory += "<option value='-1'  selected='selected'></option>";
        //            for (var i = 0; i < category.length; i++) {
        //                docCategory += "<option value='" + category[i]["Key"] + "'>" + category[i]["Value"] + "</option>";
        //            }
        //        },
        //        error: function (e) {
        //            console.log(e);
        //        }
        //    });
        //};

        //********************
        //Get Keep Version List
        //*********************
        //function getKeepVersionList() {

        //    var url = VIS.Application.contextUrl + "VADMS/UploadDocument/GetKeepVersionList";
        //    $.ajax({
        //        url: url,
        //        data:
        //            {
        //            },
        //        async: false,
        //        type: "GET",
        //        dataType: "json",
        //        success: function (data) {
        //            var keepVersion = JSON.parse(data);
        //            for (var i = 0; i < keepVersion.length; i++) {
        //                if (keepVersion[i]["Key"] == 2) {
        //                    keepVersionOption += "<option value='" + keepVersion[i]["Key"] + "' selected='selected'>" + keepVersion[i]["Value"] + "</option>";
        //                }
        //                else {
        //                    keepVersionOption += "<option value='" + keepVersion[i]["Key"] + "'>" + keepVersion[i]["Value"] + "</option>";
        //                }
        //            }
        //        },
        //        error: function (e) {
        //            console.log(e);
        //        }
        //    });
        //};

        //********************
        //Get folders information from database in which document is to be uploaded
        //*********************
        //function getFolders() {

        //    var url = VIS.Application.contextUrl + "VADMS/UploadDocument/GetFolders";
        //    $.ajax({
        //        url: url,
        //        data:
        //            {
        //            },
        //        async: false,
        //        type: "GET",
        //        dataType: "json",
        //        success: function (data) {
        //            var folder = JSON.parse(data);
        //            for (var i = 0; i < folder.length; i++) {
        //                folderOption += "<option value='" + folder[i]["Key"] + "'>" + folder[i]["Value"] + "</option>";
        //            }
        //        },
        //        error: function (e) {
        //            console.log(e);
        //        }
        //    });
        //};

        //********************
        //Get folders where sqlQuery which is to be append in FolderLookup
        //*********************
        //function getFolderWhere() {
        //    $.ajax(
        //        {
        //            url: VIS.Application.contextUrl + "VADMS/Common/FolderWhere",
        //            type: "GET",
        //            dataType: "json",
        //            async: false,
        //            success: function (data) {

        //                folderWhere = JSON.parse(data);
        //            },
        //            error: function (e) {
        //                console.log(e);
        //            }
        //        });
        //};

        //********************
        //Get Images from ProductImages folder
        //*********************

        function GetImages() {
            $.ajax({
                async: false,
                type: "GET",
                dataType: "json",
                url: VIS.Application.contextUrl + "VA005/ProductManagement/GetImages",
                success: function (data) {
                    var result = JSON.parse(data);
                    ulProduct.children().remove();
                    for (var i = 0; i < result.length; i++) {
                        ulProduct.append("<li><img class='VA005-imgProduct' src=" + result[i].ImageDetail.fileBase64Data + " ></li>")
                    }
                },
                error: function (e) {
                    console.log(e);
                }
            });
        };
        //********************
        //Events of form
        //*********************
        function events() {
            $btnOK.on('click', function () {
                productMgtObj.GetImages();
                uploadDocumentDialog.close();
            });
            $docVersion.on("keypress", function (e) {
                e.stopPropagation();
                var code = (e.keyCode ? e.keyCode : e.which);
                var regex = /^[0-9. ]*$/;
                if ((!regex.test(String.fromCharCode(e.keyCode)) && code != 13)) {
                    e.preventDefault();
                    return;
                }

            });
            $btnCancel.on('click', function () {
                isCancle = true;
                uploadDocumentDialog.close();
                clearFileList();
                dis();
            });
            openUploadDocumentForm.onClose = function () {
                dis();
            };
        };

        //********************
        //Dispose the variables
        //*********************
        this.disposeComponent = function () {
            $uploadLayout = null;
            $divUploadDiv = null;
            $uploadControl = null;
            $divDocumentInformation = null;
            $tableDocumnetInformation = null;
            $row1 = null;
            $row1col1 = null;
            $row1col2 = null;
            $row1col3 = null;
            $row1col4 = null;
            $lbldocCategory = null;
            $docCategory = null;
            $lbldocVersion = null;
            $docVersion = null;
            $row2 = null;
            $row2col1 = null;
            $row2col2 = null;
            $row2col3 = null;
            $row2col4 = null;
            $lblkeepVersion = null;
            $keepVersion = null;
            $lbldocFolder = null;
            $docfolder = null;
            $divbtnLayout = null;
            $btnOK = null;
            $btnCancel = null;
            docCategory = null;
            keepVersionOption = null;
            folderOption = null;
            $btnSubmit = null;
            $form = null;
            uploadDocumentDialog = null;
            clearFileList();
            containerdiv = null;
            lookup = null;
            $divSingleUploadInfo = null;
            $divdocumentName = null;
            $divMetaDataKeyword = null;
            $divFileType = null;
            $divMetaDataDescription = null;
            $lblDocumentName = null;
            $txtDocumentName = null;
            $lblKeyword = null;
            $txtKeyword = null;
            $lblFileType = null;
            $txtFileType = null;
            $lblDescription = null;
            $txtDescription = null;
            $divCommonDocInfo = null;
            $divDocCategory = null;
            $divDocVersion = null;
            $divDocKeepVersion = null;
            $divDocFolder = null;
            count = 0;
            // detach events and prepare for safe removal
            upload.destroy();
        };
        function dis() {
            $uploadLayout = null;
            $divUploadDiv = null;
            $uploadControl = null;
            $divDocumentInformation = null;
            $tableDocumnetInformation = null;
            $row1 = null;
            $row1col1 = null;
            $row1col2 = null;
            $row1col3 = null;
            $row1col4 = null;
            $lbldocCategory = null;
            $docCategory = null;
            $lbldocVersion = null;
            $docVersion = null;
            $row2 = null;
            $row2col1 = null;
            $row2col2 = null;
            $row2col3 = null;
            $row2col4 = null;
            $lblkeepVersion = null;
            $keepVersion = null;
            $lbldocFolder = null;
            $docfolder = null;
            $divbtnLayout = null;
            $btnOK = null;
            $btnCancel = null;
            docCategory = null;
            keepVersionOption = null;
            folderOption = null;
            $btnSubmit = null;
            $form = null;
            uploadDocumentDialog = null;
            //  clearFileList();
            containerdiv = null;
            lookup = null;
            $divSingleUploadInfo = null;
            $divdocumentName = null;
            $divMetaDataKeyword = null;
            $divFileType = null;
            $divMetaDataDescription = null;
            $lblDocumentName = null;
            $txtDocumentName = null;
            $lblKeyword = null;
            $txtKeyword = null;
            $lblFileType = null;
            $txtFileType = null;
            $lblDescription = null;
            $txtDescription = null;
            $divCommonDocInfo = null;
            $divDocCategory = null;
            $divDocVersion = null;
            $divDocKeepVersion = null;
            $divDocFolder = null;
            count = 0;
            // detach events and prepare for safe removal
            upload.destroy();
        };
    };
    VA005.uploadImage.prototype.init = function (windowNo, frame) {

        //Assign to this Varable
        this.frame = frame;
        this.windowNo = windowNo;
        // this.initialize();

    };
    //Must implement dispose
    VA005.uploadImage.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };
})(VA005, jQuery);