/// <reference path="../camljs.js" />
/// <reference path="../spin.js" />

(function () {

    var spinOptions = {
        lines: 13, // The number of lines to draw
        length: 20, // The length of each line
        width: 10, // The line thickness
        radius: 30, // The radius of the inner circle
        corners: 1, // Corner roundness (0..1)
        rotate: 0, // The rotation offset
        direction: 1, // 1: clockwise, -1: counterclockwise
        color: '#000', // #rgb or #rrggbb or array of colors
        speed: 1.7, // Rounds per second
        trail: 60, // Afterglow percentage
        shadow: false, // Whether to render a shadow
        hwaccel: false, // Whether to use hardware acceleration
        className: 'spinner', // The CSS class to assign to the spinner
        zIndex: 2e9, // The z-index (defaults to 2000000000)
        top: 'auto', // Top position relative to parent in px
        left: 'auto' // Left position relative to parent in px
    };

    var webCurrentUser;

    var modelAllBaseParticipants = [];

    function formatDate(date) {
        if (date === undefined || date.getDate === undefined) return;

        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        var hours = date.getHours();
        var minutes = date.getMinutes();

        if (day < 10) day = "0" + day;
        if (month < 10) month = "0" + month;
        if (minutes < 10) minutes = "0" + minutes;

        return day + "." + month + "." + year + " " + hours + ":" + minutes;
    }

    function parseDate(dateStr) {
        if (dateStr === undefined) return;

        var components = dateStr.split("/");
        return (new Date(+components[2], components[1] - 1, +components[0]));
    }

    function checkIfNumberValid(value) {
        return value != "" && !isNaN(value);
    }

    function saveProjectTypeIfNew(cuurentArray, qProjectType, list) {
        if (qProjectType == null || qProjectType == "") return null;
        var res = $.grep(cuurentArray, function (item) { return item.name == qProjectType; });
        if (res.length == 0) {
            var listItem = list.addItem(new SP.ListItemCreationInformation());
            listItem.set_item("Title", qProjectType);
            listItem.update();
            $.appWebContext.load(listItem);
            return qProjectType;
        }
        return null;
    }

    function getChoicesFromMetadata(fields, fieldInternalName) {
        var result = [{ name: 'Выберите значение...' }];
        for (var i = 0; i < fields.length; i++) {
            if (fields[i].get_internalName() == fieldInternalName) {
                var choices = fields[i].get_choices();
                for (var j = 0; j < choices.length; j++) {
                    result.push({ name: choices[j] });
                }
                return result;
            }
        }
        return result;
    }

    function setCurrentMeetingsNumber(number) {
        var queryUrl = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getByTitle('ConfigurationList')/items(1)";
        var newData = { __metadata: { 'type': 'SP.Data.ConfigurationListListItem' }, CurrentMeetingNumber: number };
        var requestBody = JSON.stringify(newData);
        $.ajax({
            url: encodeURI(queryUrl),
            data: requestBody,
            headers: {
                "accept": "application/json;odata=verbose",
                "X-RequestDigest": $("#__REQUESTDIGEST").val(),
                "X-HTTP-Method": "MERGE",
                "content-length": requestBody.length,
                "content-type": "application/json;odata=verbose",
                "If-Match": "*"
            },
            type: "POST",
            async: false,
            success: function (data) {
            }
        });
    }

    function getCurrentMeetingsNumber() {
        var result = 0;
        var queryUrl = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getByTitle('ConfigurationList')/items(1)";
        $.ajax({
            url: encodeURI(queryUrl),
            headers: {
                Accept: "application/json;odata=verbose"
            },
            method: "GET",
            async: false,
            success: function (data) {
                if (data.d) {
                    result = data.d.CurrentMeetingNumber;
                }
            }
        });
        return result;
    }

    // helper function for reading binary files
    function arrayBufferToBase64(buffer) {
        var binary = '';
        var bytes = new window.Uint8Array(buffer);
        var len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return binary;
    }

    function saveReportsAttachments(assignments) {
        var list = $.appWebContext.get_web().get_lists().getByTitle("Вложения отчета по поручению");
        var created = [];
        for (var assignment in assignments) {
            var currAssign = assignments[assignment];
            for (var report in currAssign.assignmentReports()) {
                var currentReport = currAssign.assignmentReports()[report];
                for (var attach in currentReport.reportAttachments()) {
                    var current = currentReport.reportAttachments()[attach];
                    // Build a request up to send with the CSOM.
                    if (current._destroy) {
                        // Handle deleted objects
                        // Deleted items that are marked "new" have never been saved to SharePoint to start with
                        if (current.Id() != "") {
                            var listItem = list.getItemById(current.Id());
                            listItem.deleteObject();
                        }
                    } else if (current.Id() == "") {
                        var listItem = list.addItem(new SP.ListItemCreationInformation());
                        listItem.set_item("AttachmentDescription", current.Descr());
                        listItem.set_item("AssignmentReportLink", currentReport.Id());
                        listItem.set_item("AttachmentDocumentTypeLink", current.DType());
                        listItem.set_item("AttachmentIsForReport", current.IsForReport());
                        listItem.update();

                        // Save a reference to both the SP.ListItem object and the KO Object so we can update
                        // the latter with the former's ID once the object has been created.
                        created.push({ spItem: listItem, koItem: current });
                        $.appWebContext.load(listItem);
                    } else {
                        // The item is neither new nor deleted, handle it as an update.
                        var listItem = list.getItemById(current.Id());
                        listItem.set_item("AttachmentDescription", current.Descr());
                        listItem.set_item("AttachmentDocumentTypeLink", current.DType());
                        listItem.update();
                    }
                }
            }
        }
        return created;

    }

    function getCurrentUser() {
        $.ajax({
            url: encodeURI(_spPageContextInfo.webAbsoluteUrl + "/_api/web/getuserbyid(" + _spPageContextInfo.userId + ")"),
            contentType: "application/json;odata=verbose",
            headers: { "accept": "application/json;odata=verbose" },
            success: function (data) {
                if (data.d) {
                    webCurrentUser = data.d;
                }
            },
            error: function () {
            }
        });
    }

    function uploadAttachments(createdAttachments, listTitle) {
        for (var item in createdAttachments) {
            createdAttachments[item].koItem.Id(createdAttachments[item].spItem.get_id());
            createdAttachments[item].koItem.New = false;

            var reader = new window.FileReader();
            var file = createdAttachments[item].koItem.File();
            if (file == undefined) return;

            reader.onerror = function (result) { alert("File read failed"); };
            reader.onload = (function (currentItem, name) {
                var itemId = currentItem.spItem.get_id();
                var itemName = name;

                return function (result) {
                    var contents = arrayBufferToBase64(result.target.result);
                    var uploadUrl = "/_api/web/lists/GetByTitle('" + listTitle + "')/items(" + itemId + ")/AttachmentFiles/add(FileName='" + itemName + "')";
                    var createItem = new SP.RequestExecutor(_spPageContextInfo.webAbsoluteUrl);

                    createItem.executeAsync({
                        url: _spPageContextInfo.webAbsoluteUrl + uploadUrl,
                        method: "POST",
                        headers: { "Accept": "application/json; odata=verbose" },
                        binaryStringRequestBody: true,
                        body: contents,
                        state: "Update",
                        success: (function (el) {
                            return function (data) {
                                var jsonObject = JSON.parse(data.body);
                                el.koItem.FileName(jsonObject.d.FileName);
                                el.koItem.FileUrl(_spPageContextInfo.siteAbsoluteUrl + jsonObject.d.ServerRelativeUrl);
                            };
                        })(currentItem)
                    });
                };
            })(createdAttachments[item], file.name);
            reader.readAsArrayBuffer(file);
        }
    }

    function saveAssignmentsJournal(assignments) {
        var assignmentJournalList = $.appWebContext.get_web().get_lists().getByTitle("Записи журналов поручений");
        for (var assignment in assignments) {
            for (var journal in assignments[assignment].assignmentJournal()) {
                if (assignments[assignment].assignmentJournal()[journal].New) {
                    var item = assignmentJournalList.addItem(new SP.ListItemCreationInformation());
                    item.set_item("AssignmentLink", assignments[assignment].Id());
                    item.set_item("AssignmentJournalEntryDescr", assignments[assignment].assignmentJournal()[journal].AssignmentJournalEntryDescr());
                    item.update();
                    assignments[assignment].assignmentJournal()[journal].New = false;
                }
            }
        }
    }

    function saveAssignmentsReports(assignments) {
        var assignmentReportsList = $.appWebContext.get_web().get_lists().getByTitle("Отчеты по поручениям");
        var createdReports = [];
        for (var assignment in assignments) {
            for (var report in assignments[assignment].assignmentReports()) {
                var currentReport = assignments[assignment].assignmentReports()[report];
                // Build a request up to send with the CSOM.
                if (currentReport._destroy) {
                    // Handle deleted objects
                    // Deleted items that are marked "new" have never been saved to SharePoint to start with
                    if (currentReport.Id() != "") {
                        var listItem = assignmentReportsList.getItemById(currentReport.Id());
                        listItem.deleteObject();
                    }
                } else if (currentReport.Id() == "") {
                    // Handle new objects to be created.
                    var listItem = assignmentReportsList.addItem(new SP.ListItemCreationInformation());
                    saveSPEntity(modelMetaData.assignmentReport.fields, currentReport, listItem);
                    listItem.set_item("AssignmentLink", assignments[assignment].Id());
                    listItem.update();

                    // Save a reference to both the SP.ListItem object and the KO Object so we can update
                    // the latter with the former's ID once the object has been created.
                    createdReports.push({
                        spItem: listItem,
                        koItem: currentReport
                    });
                    $.appWebContext.load(listItem);

                } else {
                    // The item is neither new nor deleted, handle it as an update.
                    var listItem = assignmentReportsList.getItemById(currentReport.Id());
                    saveSPEntity(modelMetaData.assignmentReport.fields, currentReport, listItem);
                    listItem.update();
                }
            }
        }
        return createdReports;
    }

    function logAssignmentChanges(prop, assignment) {
        //todo: refactoring
        //DisplayName="Тип поручения" Name="AssignmentTypeLink"
        var result = null;
        if (prop != "AssignmentLink" && prop != "AssignmentDependencyType") {
            var oldField = assignment[prop]() == null ? "" : assignment[prop]();
            var newField = assignment["edit" + prop]() == null ? "" : assignment["edit" + prop]();
            var description = "";
            switch (prop) {
                case "AssignmentNumber":
                    if (oldField.toString() != newField.toString()) {
                        description = "Номер - " + oldField.toString() + " -> " + newField.toString();
                    }
                    break;
                case "AssignmentText":
                    if (oldField.toString() != newField.toString()) {
                        description = "Текст поручения - " + oldField.toString() + " -> " + newField.toString();
                    }
                    break;
                case "AssignmentNote":
                    if (oldField.toString() != newField.toString()) {
                        description = "Примечание - " + oldField.toString() + " -> " + newField.toString();
                    }
                    break;
                case "AssignmentPredecessorLinkValue":
                    if (oldField.toString() != newField.toString()) {
                        description = "Зависимое поручение - " + oldField.toString() + " -> " + newField.toString();
                    }
                    break;
                case "AssignmentCreationDate":
                    if (oldField.toString() != newField.toString()) {
                        description = "Дата создания - " + oldField.toString() + " -> " + newField.toString();
                    }
                    break;
                case "AssignmentPlanDate":
                    if (oldField.toString() != newField.toString()) {
                        description = "Планируемая дата исполнения - " + oldField.toString() + " -> " + newField.toString();
                    }
                    break;
                case "AssignmentFactDate":
                    if (oldField.toString() != newField.toString()) {
                        description = "Фактическая дата исполнения - " + oldField.toString() + " -> " + newField.toString();
                    }
                    break;
                case "AssignmentDaysForResolve":
                    if (oldField.toString() != newField.toString()) {
                        description = "Количество дней на решение - " + oldField.toString() + " -> " + newField.toString();
                    }
                    break;
                case "AssignmentStatus":
                    if (oldField.toString() != newField.toString()) {
                        description = "Статус - " + oldField.toString() + " -> " + newField.toString();
                    }
                    break;
                case "AssignmentInspectState":
                    if (oldField.toString() != newField.toString()) {
                        description = "Состояние контроля - " + oldField.toString() + " -> " + newField.toString();
                    }
                    break;
                case "AssignmentDayType":
                    if (oldField.toString() != newField.toString()) {
                        description = "Тип дней - " + oldField.toString() + " -> " + newField.toString();
                    }
                    break;
                case "AssignmentExecutorFullNameLink":
                    var oldValue = assignment["AssignmentExecutorFullNameLinkValue"]() == null ? "" : assignment["AssignmentExecutorFullNameLinkValue"]().toString();
                    var newValue = assignment["editAssignmentExecutorFullNameLinkValue"]() == null ? "" : assignment["editAssignmentExecutorFullNameLinkValue"]().toString();
                    if (oldValue != newValue) {
                        description = "Исполнитель - " + oldValue + " -> " + newValue;
                    }
                    break;
                case "IssueLink":
                    var oldValue = assignment["IssueLinkValue"]() == null ? "" : assignment["IssueLinkValue"]().toString();
                    var newValue = assignment["editIssueLinkValue"]() == null ? "" : assignment["editIssueLinkValue"]().toString();
                    if (oldValue != newValue) {
                        description = "Вопрос повестки - " + oldValue + " -> " + newValue;
                    }
                    break;
                case "AssignmentSoexecutors":
                    if (oldField.toString() != newField.toString()) {
                        description = "Соисполнители";
                    }
                    break;
            }
            if (description != "") {
                var newLog = getNewEntity(modelMetaData.assignmentJournal.fields);
                var currentUserName = webCurrentUser.LoginName.split('|');
                newLog.Author = currentUserName.length > 1 ? currentUserName[1] : currentUserName[0];
                var nowDate = new Date();
                newLog.CreatedDate = nowDate.format("dd/MM/yy");
                result = new AssignmentJournal(newLog);
                result.AssignmentJournalEntryDescr(description);
            }
        }
        return result;
    }

    function getAvailableQuestionTypes() {
        var result = [];
        var fileQuery = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getByTitle('Категории вопросов')/items";
        $.ajax({
            url: encodeURI(fileQuery),
            headers: {
                Accept: "application/json;odata=verbose"
            },
            method: "GET",
            async: false,
            success: function (data) {
                if (data) {
                    for (i = 0; i < data.d.results.length; i++) {
                        result.push(new QuestionType(data.d.results[i]));
                    }
                }
            }
        });
        return result;
    }

    function getMeasureList() {
        var result = [];
        var fileQuery = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getByTitle('Градостроительные показатели')/items";
        $.ajax({
            url: encodeURI(fileQuery),
            headers: {
                Accept: "application/json;odata=verbose"
            },
            method: "GET",
            async: false,
            success: function (data) {
                if (data) {
                    for (i = 0; i < data.d.results.length; i++) {
                        result.push(new Measure(data.d.results[i]));
                    }
                }
            }
        });
        return result;
    }

    function getAvailableAssignmentTypes() {
        var result = [];
        var fileQuery = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getByTitle('Типы поручений')/items";
        $.ajax({
            url: encodeURI(fileQuery),
            headers: {
                Accept: "application/json;odata=verbose"
            },
            method: "GET",
            async: false,
            success: function (data) {
                if (data) {
                    for (i = 0; i < data.d.results.length; i++) {
                        result.push(new AssignmentTypes(data.d.results[i]));
                    }
                }
            }
        });
        return result;
    }

    function openParticipantSelectDialog(title, container, isMulti) {
        var options = {
            title: title,
            url: isMulti ? _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/GS.Land.Web/Pages/ParticipantsSelection.aspx?Multi=1" : _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/GS.Land.Web/Pages/ParticipantsSelection.aspx",
            width: 800,
            height: 400,
            dialogReturnValueCallback: function (dialogResult, returnValue) {
                closeSelectParticipantModal(dialogResult, returnValue, container);
            }
        };
        SP.UI.ModalDialog.showModalDialog(options);
        setCssForModalDialog();
    }

    function openQuestionAttachmentsDialog(title, id) {
        var options = {
            title: title,
            url: _spPageContextInfo.webAbsoluteUrl + (String).format("/_layouts/15/GS.Land.Web/Pages/IssueAttachmentLand.aspx?ID={0}", id),
            width: 800,
            height: 400,
            dialogReturnValueCallback: function (dialogResult, returnValue) {
                // ToDo: 
            }
        };
        SP.UI.ModalDialog.showModalDialog(options);
        setCssForModalDialog();
    }

    function openQuestionObjectsDialog(title, data) {
        if (data.issueObjects().length == 0)
            return;

        var url = _spPageContextInfo.webAbsoluteUrl;
        if (data.issueObjects().length > 1)
            url = url + (String).format("/Lists/IssueObjectLandList/AllItems.aspx?FilterField1=IssueObjectIssueIdLand&FilterValue1={0}", data.Id());
        else
            url = url + (String).format("/Lists/Object/DispForm.aspx?ID={0}", data.issueObjects()[0].IssueObjectIssueIdLand().get_lookupId());
        var options = {
            title: title,
            url: url,
            width: 1024,
            height: 768,
            autoSize: false,
            allowMaximize: true,
            dialogReturnValueCallback: function (dialogResult, returnValue) {
                // ToDo: 
            }
        };
        SP.UI.ModalDialog.showModalDialog(options);
        setCssForModalDialog();
    }

    function openOrderRgAddDialog(title) {
        var options = {
            title: title,
            url: _spPageContextInfo.webAbsoluteUrl + "/Lists/OrderLandList/Unused.aspx",
            width: 1024,
            height: 768,
            dialogReturnValueCallback: function (dialogResult, returnValue) {
                if (dialogResult == SP.UI.DialogResult.OK) {
                    var selectedOrdersId = JSON.parse(returnValue);
                    if (selectedOrdersId.length <= 0)
                        return;
                    console.log(selectedOrdersId);
                    var queryUrl = _spPageContextInfo.webServerRelativeUrl + _spPageContextInfo.serverRequestPath + "/AddOrdersRg";
                    var newData = { meetingRgId: $.listItemId };
                    newData.orderRgIdList = [];
                    selectedOrdersId.map(function (el) { newData.orderRgIdList.push(el.id); });
                    $.ajax({
                        url: encodeURI(queryUrl),
                        data: JSON.stringify(newData),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        type: "POST",
                        success: function (data) {
                            console.log(data);
                            SP.UI.ModalDialog.RefreshPage(dialogResult);
                        },
                        error: function (msg) {
                            console.log("RequestError! " + msg);
                        }
                    });
                }
            }
        };
        SP.UI.ModalDialog.showModalDialog(options);
        setCssForModalDialog();
    }

    function openIssueRgCreateDialog(title) {
        var options = {
            title: title,
            url: _spPageContextInfo.webAbsoluteUrl + "/Lists/IssueLandList/NewForm2.aspx",
            width: 1024,
            height: 768,
            dialogReturnValueCallback: function (dialogResult, returnValue) {
                SP.UI.ModalDialog.RefreshPage(dialogResult);
            }
        };
        SP.UI.ModalDialog.showModalDialog(options);
        setCssForModalDialog();
    }

    function getNewParticipantLookupValue(id) {
        var result = new SP.FieldLookupValue();
        result.set_lookupId(id);
        return result;
    }

    function setCssForModalDialog() {
        //$(".ms-dlgContent").addClass("ui-dialog ui-widget ui-widget-content ui-corner-all ui-front ui-dialog-buttons ui-draggable");
        //$(".ms-dlgTitle").addClass("ui-dialog-titlebar ui-helper-clearfix");
        //$("#dialogTitleSpan").addClass("modalDialogTitle");
    }

    function closeSelectParticipantModal(result, target, inputId) {
        if (result == SP.UI.DialogResult.OK) {
            $(inputId).val(target).change();
        }
    };

    function loadMetadataInfo(callback) {
        for (var item in modelMetaData) {
            modelMetaData[item].contentTypes = getListByUrl(modelMetaData[item].listName).get_contentTypes();
            $.appWebContext.load(modelMetaData[item].contentTypes);
        }
        $.appWebContext.executeQueryAsync(function () {
            for (var itm in modelMetaData) {
                for (var i = 0; i < modelMetaData[itm].contentTypes.get_count() ; i++) {
                    if (!modelMetaData[itm].contentTypes.getItemAtIndex(i).get_sealed()) {
                        modelMetaData[itm].contentType = modelMetaData[itm].contentTypes.getItemAtIndex(i);
                        break;
                    }
                }
                modelMetaData[itm].fields = modelMetaData[itm].contentType.get_fields();
                $.appWebContext.load(modelMetaData[itm].fields);
            }
            $.appWebContext.executeQueryAsync(function () {
                for (var fldItm in modelMetaData) {
                    var fieldsArr = [];
                    for (var i = 0; i < modelMetaData[fldItm].fields.get_count() ; i++) {
                        // don't load SPD created fields
                        var fldItmIntName = modelMetaData[fldItm].fields.getItemAtIndex(i).get_internalName();
                        if ((fldItmIntName.toLowerCase().indexOf('__x') > 0) || (fldItmIntName == 'IssueDeclarant') || (fldItmIntName == 'ContentType')) continue;

                        fieldsArr.push(modelMetaData[fldItm].fields.getItemAtIndex(i));
                    }
                    modelMetaData[fldItm].fields = fieldsArr;
                }
                callback([]);
            }, function (sender, args) {
                alert('Request failed. ' + args.get_message() +
                    '\n' + args.get_stackTrace());
            });
        }, function (sender, args) {
            alert('Request failed. ' + args.get_message() +
                '\n' + args.get_stackTrace());
        });
    }

    //load item attachment info
    function loadListItemAttachmentInfo(listId, itemId) {
        var result = { FileUrl: "", FileName: "" };
        var fileQuery = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getById('" + listId + "')/items(" + itemId + ")/AttachmentFiles";
        $.ajax({
            url: encodeURI(fileQuery),
            headers: {
                Accept: "application/json;odata=verbose"
            },
            method: "GET",
            async: false,
            success: function (data) {
                if (data) {
                    if (data.d.results.length > 0) {
                        // each attach can hold only one file, so we take only first file
                        var url = data.d.results[0].ServerRelativeUrl;
                        var index = url.indexOf(_spPageContextInfo.webServerRelativeUrl);
                        url = index == 0 && _spPageContextInfo.webServerRelativeUrl != '/' ? url.substring(_spPageContextInfo.webServerRelativeUrl.length) : url;
                        result.FileUrl = _spPageContextInfo.webAbsoluteUrl + url;
                        result.FileName = data.d.results[0].FileName;
                    }
                }
            }
        });
        return result;
    }

    //load attachments for issue questions
    function loadAssignmentReportAttachments(assignId) {
        var attachments = [];
        var select = "$select=ID,AssignmentReportLink/Id,AttachmentDescription,AttachmentDocumentTypeLink/Id,AttachmentIsForReport";
        var expand = "&$expand=AttachmentFiles,AssignmentReportLink,AttachmentDocumentTypeLink";
        var fileQuery = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getByTitle('Вложения отчета по поручению')/items?" + select + expand + "&$filter=AssignmentReportLink/Id eq " + assignId;

        $.ajax({
            url: encodeURI(fileQuery),
            headers: { Accept: "application/json;odata=verbose" },
            method: "GET",
            async: false,
            success: function (data) {
                if (data) {
                    // each attach can hold only one file, so we take only first file
                    for (i = 0; i < data.d.results.length; i++) {
                        var fileInfo = loadListItemAttachmentInfo("Вложения отчета по поручению", data.d.results[i].ID);
                        attachments.push(new meetingAttach({
                            AttachmentDescription: data.d.results[i].AttachmentDescription,
                            MeetingLink: data.d.results[i].AssignmentReportLink.Id,
                            Id: data.d.results[i].ID,
                            FileUrl: fileInfo.FileUrl,
                            FileName: fileInfo.FileName,
                            FilePath: "",
                            AttachmentDocumentTypeLinkValue: data.d.results[i].AttachmentDocumentTypeLink.Id ? data.d.results[i].AttachmentDocumentTypeLink.Id : "",
                            AttachmentIsForReport: data.d.results[i].AttachmentIsForReport,
                            New: false
                        }));
                    }
                }
            }
        });
        return attachments;
    }

    function getParticipiantsEntities(lookups) {
        var result = [];
        if (lookups != null) {
            var select = "$select=ParticipantFullName,ParticipantPositionLink/Title,ParticipantOrgLink/OrganizationName,ParticipantRole,ID";
            var expand = "&$expand=ParticipantPositionLink,ParticipantOrgLink";
            var filter = "";
            var lookupsIds = [];
            $.each(lookups, function () {
                lookupsIds.push(this.get_lookupId());
            });
            lookupsIds = $.unique(lookupsIds);
            if (lookupsIds.length > 0) filter = "&$filter=Id eq " + lookupsIds[0];
            else return [];
            for (i = 1; i < lookupsIds.length; i++) {
                filter += "or Id eq " + lookupsIds[i];
            }
            var fileQuery = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getByTitle('Справочник участников')/items?" + select + expand + filter;
            $.ajax({
                url: encodeURI(fileQuery),
                headers: { Accept: "application/json;odata=verbose" },
                method: "GET",
                async: false,
                success: function (data) {
                    if (data.d) {
                        for (i = 0; i < data.d.results.length; i++) {
                            result.push(new participantEntity(data.d.results[i]));
                        }
                    }
                }
            });
        }
        return result;
    }

    function QuestionType(data) {
        this.Id = data.Id;
        this.Name = data.QuestionCategoryName;
    }

    function Measure(data) {
        this.Id = data.Id;
        this.Value = data.Value;
        this.Unit = data.Unit;
    }

    //типы поручений
    function AssignmentTypes(data) {
        this.Id = data.Id;
        this.AssignmentTypeName = data.AssignmentTypeName;
        this.AssignmentTypeDescr = data.AssignmentTypeDescr;
    }

    // assignment report entity
    function AssignmentReport(data) {
        for (var prop in data) {
            if (prop == 'New') {
                this[prop] = data[prop];
            }
            else if (prop == 'Id') {
                this[prop] = ko.observable(data[prop]);
            }
            else {
                if (prop.toLowerCase().endsWith("link")) {
                    this[prop + "Value"] = ko.observable(data[prop] ? data[prop].get_lookupValue() : null);
                    this[prop + "Id"] = ko.observable(data[prop] ? data[prop].get_lookupId() : null);
                    this["edit" + prop + "Value"] = ko.observable(data[prop] ? data[prop].get_lookupValue() : null);
                    this["edit" + prop + "Id"] = ko.observable(data[prop] ? data[prop].get_lookupId() : null);
                }
                // regular property
                this[prop] = ko.observable(data[prop]);
                // property fo editing
                this["edit" + prop] = ko.observable(data[prop]);
            }
        }

        var attachments = this.Id() > 0 ? loadAssignmentReportAttachments(this.Id()) : null;
        this.reportAttachments = attachments != null ? ko.observableArray(attachments) : ko.observableArray([]);
        this.editReportAttachments = attachments != null ? ko.observableArray(attachments) : ko.observableArray([]);

        this.editReportAttachmentsFiltered = function () {
            return ko.utils.arrayFilter(this.editReportAttachments(), function (attachment) {
                return attachment.IsForReport();
            });
        };

        this.editRequestAttachmentsFiltered = function () {
            return ko.utils.arrayFilter(this.editReportAttachments(), function (attachment) {
                return !attachment.IsForReport();
            });
        };

        this.removeReportAttach = function (attachment) {
            this.editReportAttachments.destroy(attachment);
        }.bind(this);

        this.addRequestAttach = function () {
            this.doAddReportAttach(false);
        };

        this.addReportAttach = function () {
            this.doAddReportAttach(true);
        };

        this.doAddReportAttach = function (isReportAttach) {
            this.editReportAttachments.push(new meetingAttach({
                Id: "",
                AttachmentDescription: "",
                MeetingLink: this.Id(),
                FileUrl: "",
                FileName: "",
                FilePath: "",
                AttachmentDocumentTypeLinkValue: "",
                AttachmentIsForReport: isReportAttach,
                New: true
            }));
        }.bind(this);

        //persist edits to real values on accept
        this.accept = function () {
            for (var prop in data) {
                if ((prop != 'New') && (prop != 'Id')) {
                    if (prop.toLowerCase().endsWith("link")) {
                        this[prop + "Value"](this["edit" + prop + "Value"]());
                        this[prop + "Id"](this["edit" + prop + "Id"]());
                    }
                    this[prop](this["edit" + prop]());
                }
            }
            this.reportAttachments(this.editReportAttachments());
        }.bind(this);

        //reset to originals on cancel
        this.cancel = function () {
            for (var prop in data) {
                if ((prop != 'New') && (prop != 'Id')) {
                    if (prop.toLowerCase().endsWith("link")) {
                        this["edit" + prop + "Value"](this[prop + "Value"]());
                        this["edit" + prop + "Id"](this[prop + "Id"]());
                    }
                    this["edit" + prop](this[prop]());
                }
            }
            this.editReportAttachments(this.reportAttachments());
        }.bind(this);
    }

    // assignment journal entity
    function AssignmentJournal(data) {
        for (var prop in data) {
            if (prop == 'New') {
                this[prop] = data[prop];
            }
            else {
                this[prop] = ko.observable(data[prop]);
            }
        }
    }

    // поручение
    function Assignment(data) {

        var selfAssign = this;
        for (var prop in data) {
            if (prop == 'New') {
                selfAssign[prop] = data[prop];
            }
            else if (prop == 'Id') {
                selfAssign[prop] = ko.observable(data[prop]);
            }
            else {
                if (prop.toLowerCase().endsWith("link")) {
                    selfAssign[prop + "Value"] = ko.observable(data[prop] ? data[prop].get_lookupValue() : null);
                    selfAssign[prop + "Id"] = ko.observable(data[prop] ? data[prop].get_lookupId() : null);
                    selfAssign["edit" + prop + "Value"] = ko.observable(data[prop] ? data[prop].get_lookupValue() : null);
                    selfAssign["edit" + prop + "Id"] = ko.observable(data[prop] ? data[prop].get_lookupId() : null);
                }
                // regular property
                selfAssign[prop] = ko.observable(data[prop]);
                // property fo editing
                selfAssign["edit" + prop] = ko.observable(data[prop]);
            }
        }

        selfAssign.selectedRequest = ko.observable(undefined);
        selfAssign.assignmentReports = ko.observableArray([]);
        selfAssign.assignmentJournal = ko.observableArray([]);
        selfAssign.editAssignmentReports = ko.observableArray([]);

        selfAssign.changedExecutor = ko.observable("");
        selfAssign.changedExecutorIsNull = ko.computed(function () {
            return !(
                    selfAssign.editAssignmentExecutorFullNameLinkValue() ||
                    selfAssign.editAssignmentExecutorPositionLinkValue() ||
                    selfAssign.editAssignmentExecutorOrganizationLinkValue());
        }, selfAssign);
        selfAssign.changedSoexecutors = ko.observable("");

        selfAssign.assignmentSoexecutors = ko.observableArray([]);
        selfAssign.changedAssignmentSoexecutors = ko.observableArray([]);

        //load executor
        selfAssign.loadExecutor = function () {
            if (selfAssign.editAssignmentExecutorFullNameLink() != null) {
                var particinants = $.grep(modelAllBaseParticipants, function (e) { return e.Id().toString() == selfAssign.editAssignmentExecutorFullNameLinkId().toString(); });
                if (particinants.length > 0) {
                    selfAssign.AssignmentExecutorPositionLinkValue(particinants[0].ParticipantPosition());
                    selfAssign.editAssignmentExecutorPositionLinkValue(particinants[0].ParticipantPosition());
                    selfAssign.AssignmentExecutorOrganizationLinkValue(particinants[0].ParticipantOrg());
                    selfAssign.editAssignmentExecutorOrganizationLinkValue(particinants[0].ParticipantOrg());
                }
            }
        }.bind(selfAssign);

        //load soexecutors
        selfAssign.loadSoexecutors = function () {
            if (selfAssign.AssignmentSoexecutors() == null) return;
            var result = [];
            $.each(selfAssign.AssignmentSoexecutors(), function () {
                var lookupId = this.get_lookupId();
                var reporters = $.grep(modelAllBaseParticipants, function (e) { return e.Id().toString() == lookupId.toString(); });
                if (reporters.length > 0) {
                    result.push(reporters[0]);
                }
            });
            selfAssign.assignmentSoexecutors(result);
            selfAssign.changedAssignmentSoexecutors(result);
        }.bind(selfAssign);

        //выбор исполнителя
        selfAssign.selectExecutor = function () {
            var options = {
                title: "Выбор исполнителя",
                url: _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/GS.Land.Web/Pages/ExecutorSelection.aspx",
                width: 800,
                height: 400,
                dialogReturnValueCallback: function (dialogResult, returnValue) {
                    closeSelectParticipantModal(dialogResult, returnValue, "#hiddenExecutor");
                }
            };
            SP.UI.ModalDialog.showModalDialog(options);
            setCssForModalDialog();
        }.bind(selfAssign);

        //выбор соисполнителей
        selfAssign.selectSoexecutors = function () {
            var options = {
                title: "Выбор соисполнителей",
                url: _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/GS.Land.Web/Pages/ParticipantsSelection.aspx?Multi=1",
                width: 800,
                height: 400,
                dialogReturnValueCallback: function (dialogResult, returnValue) {
                    closeSelectParticipantModal(dialogResult, returnValue, "#hiddenSoexecutors");
                }
            };
            SP.UI.ModalDialog.showModalDialog(options);
            setCssForModalDialog();
        }.bind(selfAssign);

        //change Soexecutors
        selfAssign.onSoexecutorsChange = ko.computed(function () {
            if (selfAssign.changedSoexecutors() != "") {
                var reporterObject = $.parseJSON(selfAssign.changedSoexecutors());
                for (i = 0; i < reporterObject.length; i++) {
                    var item = new participantEntity(reporterObject[i]);
                    if (!ko.utils.arrayFirst(selfAssign.changedAssignmentSoexecutors(), function (arrItem) { return item.Id() == arrItem.Id(); })) {
                        selfAssign.changedAssignmentSoexecutors.push(item);
                        if (selfAssign.editAssignmentSoexecutors() == null) selfAssign.editAssignmentSoexecutors([]);
                        var reporterLookup = new SP.FieldLookupValue();
                        reporterLookup.set_lookupId(reporterObject[i].Id);
                        selfAssign.editAssignmentSoexecutors().push(reporterLookup);
                    }
                }
                selfAssign.changedSoexecutors("");
            }
        }, selfAssign);

        // change Executor
        selfAssign.onExecutorChange = ko.computed(function () {
            if (selfAssign.changedExecutor() != "") {
                var executorObject = $.parseJSON(selfAssign.changedExecutor());
                var executorLookup = new SP.FieldLookupValue();
                executorLookup.set_lookupId(executorObject.Id);
                selfAssign.editAssignmentExecutorFullNameLinkValue(executorObject.FullName);
                selfAssign.editAssignmentExecutorFullNameLinkId(executorObject.Id);
                selfAssign.editAssignmentExecutorPositionLinkValue(executorObject.Position);
                selfAssign.editAssignmentExecutorPositionLinkId(executorObject.Id);
                selfAssign.editAssignmentExecutorOrganizationLinkValue(executorObject.Org);
                selfAssign.editAssignmentExecutorOrganizationLinkId(executorObject.Id);
                switch (executorObject.ExecutorType) {
                    case "FIO":
                        selfAssign.editAssignmentExecutorFullNameLink(executorLookup);
                        selfAssign.editAssignmentExecutorPositionLink(null);
                        selfAssign.editAssignmentExecutorOrganizationLink(null);
                        break;
                    case "Position":
                        selfAssign.editAssignmentExecutorFullNameLink(null);
                        selfAssign.editAssignmentExecutorPositionLink(executorLookup);
                        selfAssign.editAssignmentExecutorOrganizationLink(null);
                        break;
                    case "Organization":
                        selfAssign.editAssignmentExecutorFullNameLink(null);
                        selfAssign.editAssignmentExecutorPositionLink(null);
                        selfAssign.editAssignmentExecutorOrganizationLink(executorLookup);
                }
                selfAssign.changedExecutor("");
            }
        }, selfAssign);

        //persist edits to real values on accept
        selfAssign.accept = function () {
            for (var prop in data) {
                if ((prop != 'New') && (prop != 'Id')) {
                    var logEntity = logAssignmentChanges(prop, selfAssign);
                    if (logEntity != null) {
                        selfAssign.assignmentJournal.push(logEntity);
                    }
                    if (prop.toLowerCase().endsWith("link")) {
                        selfAssign[prop + "Value"](selfAssign["edit" + prop + "Value"]());
                        selfAssign[prop + "Id"](selfAssign["edit" + prop + "Id"]());
                    }
                    selfAssign[prop](selfAssign["edit" + prop]());
                }
            }
            selfAssign.assignmentSoexecutors(selfAssign.changedAssignmentSoexecutors());
            selfAssign.assignmentReports(selfAssign.editAssignmentReports());
        }.bind(selfAssign);

        //reset to originals on cancel
        selfAssign.cancel = function () {
            for (var prop in data) {
                if ((prop != 'New') && (prop != 'Id')) {
                    if (prop.toLowerCase().endsWith("link")) {
                        selfAssign["edit" + prop + "Value"](selfAssign[prop + "Value"]());
                        selfAssign["edit" + prop + "Id"](selfAssign[prop + "Id"]());
                    }
                    selfAssign["edit" + prop](selfAssign[prop]());
                }
            }
            selfAssign.changedAssignmentSoexecutors(selfAssign.assignmentSoexecutors());
            selfAssign.editAssignmentReports(selfAssign.assignmentReports());
        }.bind(selfAssign);

        selfAssign.assignmentTypeComputed = ko.computed({
            read: function () { return selfAssign.editAssignmentTypeLink() ? selfAssign.editAssignmentTypeLink().get_lookupId() : ""; },
            write: function (id) {
                selfAssign.editAssignmentTypeLink(getNewParticipantLookupValue(id));
                selfAssign.editAssignmentTypeLinkId(id);
            }
        }, selfAssign);

        selfAssign.removeAssignSoexecutor = function (ent) {
            selfAssign.changedAssignmentSoexecutors.remove(ent);
            this.editAssignmentSoexecutors($.grep(this.editAssignmentSoexecutors(), function (n, i) {
                return n.get_lookupId().toString() != ent.Id().toString();
            }));
        }.bind(selfAssign);

        ///////////////////////////////////////////////////////////////////////
        // Requests
        // remove Request
        selfAssign.removeRequest = function (request) {
            selfAssign.editAssignmentReports.destroy(request);
        }.bind(selfAssign);

        // edit Request
        selfAssign.editRequest = function (request) {
            selfAssign.selectedRequest(request);
        }.bind(selfAssign);

        // accept request
        selfAssign.acceptRequest = function () {
            var selected = selfAssign.selectedRequest();
            selected.accept();

            if (selected.New) {
                var e = newFromKOEntity(modelMetaData.assignmentReport.fields, selected);
                e.Id = "";
                e.New = false;
                var newEntity = new AssignmentReport(e);
                newEntity.reportAttachments(selected.reportAttachments());
                newEntity.editReportAttachments(selected.reportAttachments());
                selfAssign.editAssignmentReports.push(newEntity);
            }
            selfAssign.selectedRequest(undefined);
        };

        // cancel changes in request
        selfAssign.cancelRequest = function () {
            selfAssign.selectedRequest().cancel();
            selfAssign.selectedRequest(undefined);
        };
        // add request
        selfAssign.addRequest = function () {
            var e = getNewEntity(modelMetaData.assignmentReport.fields);
            var nowDate = new Date();
            e['AssignmentReportRequestDate'] = nowDate.format("dd/MM/yyyy");
            selfAssign.selectedRequest(new AssignmentReport(e));
            selectRequestsTab('tab-request-request');
        };
        // add report
        selfAssign.addReport = function () {
            var e = getNewEntity(modelMetaData.assignmentReport.fields);
            var nowDate = new Date();
            e['AssignmentReportFactAnswerDate'] = nowDate.format("dd/MM/yyyy");
            selfAssign.selectedRequest(new AssignmentReport(e));
            selectRequestsTab('tab-request-report');
        };
    }

    // вопрос повестки
    function issue(data) {
        for (var prop in data) {
            if (prop == 'New') {
                this[prop] = data[prop];
            } else if (prop == 'Id') {
                this[prop] = ko.observable(data[prop]);
            } else {
                // regular property
                this[prop] = ko.observable(data[prop]);
                // property fo editing
                this["edit" + prop] = ko.observable(data[prop]);
                if (prop == "AgendaLinkedQuestionLink") {
                    this[prop + "Value"] = ko.observable(data[prop] ? data[prop].get_lookupValue() : null);
                    this[prop + "Id"] = ko.observable(data[prop] ? data[prop].get_lookupId() : null);
                    this["edit" + prop + "Value"] = ko.observable(data[prop] ? data[prop].get_lookupValue() : null);
                    this["edit" + prop + "Id"] = ko.observable(data[prop] ? data[prop].get_lookupId() : null);
                }
            }
        }
        // вычислимое поле для идентификации вопроса в таблице
        this.calcContent = ko.computed(function () {
            var str = "";
            //str += this.IssueSiteName() ? "Объект: " + this.IssueSiteName() + ";" : "";
            //str += this.IssueReason() ? "Основание: " + this.IssueReason() + ";" : "";
            //str += this.IssueDeclarant() ? "Заявитель на комиссию: " + this.IssueDeclarant() + ";" : "";
            //str += this.IssueIncomingDate() ? "Дата поступления: " + this.IssueIncomingDate() + ";" : "";
            //str += "Тип проекта: ???" + ";";
            str += this.IssueAddressLand() ? "Адрес объекта: " + this.IssueAddressLand() + ";" : "";
            //str += this.IssueInvestorLand() ? "Инвестор: " + this.IssueInvestorLand() + ";" : "";

            return str || this.IssueDescriptionLand();
        }, this);

        // вычислимое поле для списка докладчиков
        this.calcReporters = ko.computed(function () {
            var reporters = [];
            if (this.IssueReporterLand && this.IssueReporterLand()) {
                reporters.push(this.IssueReporterLand().get_lookupValue());
            }

                if (this.IssueCoReportersLand() != null) {
                    $.each(this.IssueCoReportersLand(), function () {
                        reporters.push(this.get_lookupValue());
                    });
                }

            return reporters.join("; ");
        }, this);

        // режим отображения диалога с вопросом повестки
        this.mode = ko.observable("full");
        // ссылка на карточку вопроса
        this.httpLink = ko.observable(
            (String).format("{0}//{1}/sites/gca/Lists/IssueLandList/DispForm2.aspx?ID={2}&ContentTypeId={3}",
                window.location.protocol,
                window.location.host,
                data.Id,
                modelMetaData.issue.ctId
            )
        );

        this.gotoEditQuestion = function () {
            window.open(_spPageContextInfo.webAbsoluteUrl + '/Lists/IssueLandList/DispForm2.aspx?ID=' + data.Id, '_blank');
        }

        this.changedReporter = ko.observable("");
        this.changedSoreporters = ko.observable("");

        this.issueObjects = ko.observableArray([]);

        this.issueMeasures = ko.observableArray([]);
        //this.issueMeasures(getMeasureList());
        // ToDo
        this.issueMeasures(null);

        this.issueAttachments = ko.observableArray([]);
        this.editIssueAttachments = ko.observableArray([]);

        this.issueReporterFIO = ko.observable();
        this.editIssueReporterFIO = ko.observable();
        this.issueReporterPosition = ko.observable();
        this.editIssueReporterPosition = ko.observable();
        this.issueReporterOrganizationName = ko.observable();
        this.editIssueReporterOrganizationName = ko.observable();
        this.changedReporterIsNull = ko.computed(function () {
            return !(
                    this.editIssueReporterFIO() ||
                    this.editIssueReporterPosition() ||
                    this.editIssueReporterOrganizationName()
            )
        }, this);
        this.issueSoreporters = ko.observableArray([]);
        this.editIssueSoreporters = ko.observableArray([]);

        //load reporter
/*         this.loadReporter = function () {
            if (this.IssueReporterFullNameLink() == null) return;
            var lookupId = this.IssueReporterFullNameLink().get_lookupId();
            var reporters = $.grep(modelAllBaseParticipants, function (e) { return e.Id().toString() == lookupId.toString(); });
            if (reporters.length > 0) {
                this.issueReporterFIO(reporters[0].ParticipantFullName());
                this.editIssueReporterFIO(reporters[0].ParticipantFullName());
                this.issueReporterPosition(reporters[0].ParticipantPosition());
                this.editIssueReporterPosition(reporters[0].ParticipantPosition());
                this.issueReporterOrganizationName(reporters[0].ParticipantOrg());
                this.editIssueReporterOrganizationName(reporters[0].ParticipantOrg());
            }
        }.bind(this); */

        //load soreporters
/*         this.loadSoreporters = function () {
            if (this.IssueSoreporterFullNameLink() == null) return;
            var result = [];
            $.each(this.IssueSoreporterFullNameLink(), function () {
                var lookupId = this.get_lookupId();
                var reporters = $.grep(modelAllBaseParticipants, function (e) { return e.Id().toString() == lookupId.toString(); });
                if (reporters.length > 0) {
                    result.push(reporters[0]);
                }
            });
            this.issueSoreporters(result);
            this.editIssueSoreporters(result);
        }.bind(this); */

        //выбор докладчика
        this.selectReporter = function () { openParticipantSelectDialog("Выбор докладчика", "#hiddenReporter", false); }.bind(this);

        // диалог отображения вложений
        this.showAttachments = function () {
            openQuestionAttachmentsDialog(
                (String).format("Вложения вопроса повестки №{0}", data.IssueNumberLand), data.Id);
        };

        // диалог отображения объектов
        this.showObjects = function () {
            openQuestionObjectsDialog(
                (String).format("Объекты вопроса повестки №{0}", data.IssueNumberTextLand), data.Id);
        };

        //select linked question
        //this.selectLinkedQuestion = function() {
        //    var options = {
        //        title: "Выбор связанного вопроса",
        //        url: _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/GS.Land.Web/Pages/SearchQuestion.aspx",
        //        width: 800,
        //        height: 400,
        //        dialogReturnValueCallback: function (dialogResult, returnValue) {
        //            closeSelectParticipantModal(dialogResult, returnValue, "#hiddenLinkedQuestion");
        //        }
        //    };
        //    SP.UI.ModalDialog.showModalDialog(options);
        //    setCssForModalDialog();
        //}.bind(this);

        //выбор содокладчиков
        //this.selectSoreporters = function () { openParticipantSelectDialog("Выбор содокладчиков", "#hiddenSoreporters", true); }.bind(this);

        //удалить содокладчика
        //this.removeSoreporter = function(soreporter) {
        //    this.editIssueSoreporters.remove(soreporter);
        //    this.editIssueSoreporterFullNameLink($.grep(this.editIssueSoreporterFullNameLink(), function (n, i) {
        //        return n.get_lookupId().toString() != soreporter.Id().toString();
        //    }));
        //}.bind(this);

        //change Soreporters
        //this.onSoreportersChange = ko.computed(function () {
        //    if (this.changedSoreporters() != "") {
        //        var reporterObject = $.parseJSON(this.changedSoreporters());
        //        for (i = 0; i < reporterObject.length; i++) {
        //            var item = new participantEntity(reporterObject[i]);
        //            if (!ko.utils.arrayFirst(this.editIssueSoreporters(), function (arrItem) { return item.Id() == arrItem.Id(); })) {
        //                this.editIssueSoreporters.push(item);
        //                if (this.editIssueSoreporterFullNameLink() == null) this.editIssueSoreporterFullNameLink([]);
        //                var reporterLookup = new SP.FieldLookupValue();
        //                reporterLookup.set_lookupId(reporterObject[i].Id);
        //                this.editIssueSoreporterFullNameLink().push(reporterLookup);
        //            }
        //        }
        //        this.changedSoreporters("");
        //    }
        //}, this);

        // change reporter
        //this.onReporterChange = ko.computed(function() {
        //    if (this.changedReporter() != "") {
        //        var reporterObject = $.parseJSON(this.changedReporter());
        //        var reporterLookup = new SP.FieldLookupValue();
        //        reporterLookup.set_lookupId(reporterObject[0].Id);
        //        this.editIssueReporterFullNameLink(reporterLookup);
        //        this.editIssueReporterFIO(reporterObject[0].ParticipantFullName);
        //        this.editIssueReporterPosition(reporterObject[0].ParticipantPosition);
        //        this.editIssueReporterOrganizationName(reporterObject[0].ParticipantOrg);
        //        this.changedReporter("");
        //    }
        //}, this);

        //this.editQuestionCategoryId = ko.computed({            
        //    read: function () { return this.editQuestionCategoryLink() ? this.editQuestionCategoryLink().get_lookupId() : null; },
        //    write: function (value) {
        //        if (value) {
        //            var lookup = new SP.FieldLookupValue();
        //            lookup.set_lookupId(value);
        //            this.editQuestionCategoryLink(lookup);
        //        } else {
        //            this.editQuestionCategoryLink(null);
        //        }
        //    }
        //}, this);

        //change linked question
        //this.onLinkedQuestionChange = ko.computed({
        //    read: function () { return this.editAgendaLinkedQuestionLinkValue() ? this.editAgendaLinkedQuestionLinkValue() : ""; },
        //    write: function (value) {
        //        if (value != "" && value != this.editAgendaLinkedQuestionLinkValue()) {
        //            var jsObject = $.parseJSON(value);
        //            var lookup = new SP.FieldLookupValue();
        //            lookup.set_lookupId(jsObject.Id);
        //            this.editAgendaLinkedQuestionLink(lookup);
        //            this.editAgendaLinkedQuestionLinkValue(jsObject.IssueNumber);
        //            this.editAgendaLinkedQuestionLinkId(jsObject.Id);
        //        }
        //    }
        //}, this);

        // additional properties
        //this.ListTheme = ko.computed(function () {
        //    if (this.IssueTheme()) {
        //        return this.IssueTheme().length > 70 ? this.IssueNumber() + ". " + this.IssueTheme().substring(0, 67) + "..." : this.IssueNumber() + ". " + this.IssueTheme();
        //    }
        //    else {
        //        return null;
        //    }
        //}, this);

        //persist edits to real values on accept
        //this.accept = function () {
        //    for (var prop in data) {
        //        if ((prop != 'New') && (prop != 'Id')) {
        //            this[prop](this["edit" + prop]());
        //            if (prop == "AgendaLinkedQuestionLink") {
        //                this[prop + "Value"](this["edit" + prop + "Value"]());
        //                this[prop + "Id"](this["edit" + prop + "Id"]());
        //            }
        //        }
        //    }
        //    this.issueReporterFIO(this.editIssueReporterFIO());
        //    this.issueReporterPosition(this.editIssueReporterPosition());
        //    this.issueReporterOrganizationName(this.editIssueReporterOrganizationName());
        //    this.issueSoreporters(this.editIssueSoreporters());
        //    this.issueAttachments(this.editIssueAttachments());
        //}.bind(this);

        //reset to originals on cancel
        //this.cancel = function () {
        //    for (var prop in data) {
        //        if ((prop != 'New') && (prop != 'Id')) {
        //            this["edit" + prop](this[prop]());
        //            if (prop == "AgendaLinkedQuestionLink") {
        //                this["edit" + prop + "Value"](this[prop + "Value"]());
        //                this["edit" + prop + "Id"](this[prop + "Id"]());
        //            }
        //        }
        //    }
        //    this.editIssueReporterFIO(this.issueReporterFIO());
        //    this.editIssueReporterPosition(this.issueReporterPosition());
        //    this.editIssueReporterOrganizationName(this.issueReporterOrganizationName());
        //    this.editIssueSoreporters(this.issueSoreporters());
        //    this.editIssueAttachments(this.issueAttachments());
        //}.bind(this);

        //this.addQuestionAttachment = function () {
        //    this.editIssueAttachments.push(new meetingAttach({
        //        Id: "",
        //        AttachmentDescription: "",
        //        MeetingLink: this.Id(),
        //        FileUrl: "",
        //        FileName: "",
        //        FilePath: "",
        //        AttachmentDocumentTypeLinkValue: "",
        //        New: true
        //    }));
        //}.bind(this);

        //this.removeQuestionAttachment = function (attachment) {
        //    this.editIssueAttachments.destroy(attachment);
        //}.bind(this);
        //    this.selectIssueProjectType = function (dataset, dataum) {
        //    this.editIssueProjectType(dataum.label);
        //}.bind(this);

        this.editQuestionComment = function () { ShowCommentWindow(this); }
    }

    // Объект вопроса повестки
    function issueObject(data) {
        for (var prop in data) {
            if (prop == 'New') {
                this[prop] = data[prop];
            } else if (prop == 'Id') {
                this[prop] = ko.observable(data[prop]);
            } else {
                // regular property
                this[prop] = ko.observable(data[prop]);
                // property fo editing
                this["edit" + prop] = ko.observable(data[prop]);
                if (prop == "AgendaLinkedQuestionLink") {
                    this[prop + "Value"] = ko.observable(data[prop] ? data[prop].get_lookupValue() : null);
                    this[prop + "Id"] = ko.observable(data[prop] ? data[prop].get_lookupId() : null);
                    this["edit" + prop + "Value"] = ko.observable(data[prop] ? data[prop].get_lookupValue() : null);
                    this["edit" + prop + "Id"] = ko.observable(data[prop] ? data[prop].get_lookupId() : null);
                }
            }
        }
    }

    // meeting attach
    // each attach can hold only one file
    function meetingAttach(data) {
        this.Id = ko.observable(data.Id);
        this.Descr = ko.observable(data.AttachmentDescription);
        this.MeetingLink = ko.observable(data.MeetingLink);
        // remote server url
        this.FileUrl = ko.observable(data.FileUrl);
        // remote file name
        this.FileName = ko.observable(data.FileName);
        // local file path (when adding new file)
        this.FilePath = ko.observable(data.FilePath);
        this.DType = ko.observable(data.AttachmentDocumentTypeLinkValue);
        this.IsForReport = ko.observable(data.AttachmentIsForReport);
        this.New = data.New;
        this.File = ko.observable();

        this.selectedFile = function (domInput, element, event) {
            if (domInput.files == undefined) {
                this.File = domInput.files;
            } else {
                this.File(domInput.files[0]);
            }
        }.bind(this);
    }

    // participant entity
    function participantEntity(data) {
        this.Id = ko.observable(data.Id);
        this.ParticipantFullName = ko.observable(data.ParticipantFullName);
        this.ParticipantPosition = data.ParticipantPositionLink != undefined ? ko.observable(data.ParticipantPositionLink.Title) : ko.observable(data.ParticipantPosition);
        this.ParticipantOrg = data.ParticipantOrgLink != undefined ? ko.observable(data.ParticipantOrgLink.OrganizationName) : ko.observable(data.ParticipantOrg);
        this.ParticipantRole = ko.observable(data.ParticipantRole);
    }

    // meeting
    function meeting(data) {
        for (var prop in data) {
            if (prop == 'New') {
                this[prop] = data[prop];
            }
            else {
                this[prop] = ko.observable(data[prop]);
            }
        }

        this.changeTitle = ko.computed(function () {
            var number = this.MeetingNumberLand();
            var nowDate = new Date();
            var meetingYear = this.CreatedDate == undefined ? nowDate.getFullYear() : +this.CreatedDate().split('/')[2];
            this.Title(number + "-" + meetingYear);
        }, this);

        //выбор председателя
        //this.ChairManFullNameLinkValue = ko.observable(this.ChairManFullNameLink() ? this.ChairManFullNameLink().get_lookupValue() : null);
        //this.ChairManFullNameLinkId = ko.computed({
        //    read: function () { return this.ChairManFullNameLink() ? this.ChairManFullNameLink().get_lookupId() : null; },
        //    write: function (json) {
        //        var parsedJson = $.parseJSON(json);
        //        this.ChairManFullNameLink(getNewParticipantLookupValue(parsedJson[0].Id));
        //        this.ChairManFullNameLinkValue(parsedJson[0].ParticipantFullName);
        //    }
        //}, this);
        //this.selectChairMan = function() { openParticipantSelectDialog("Выбор председателя", "#hidden-meeting-chair-man", false); }.bind(this);

        //выбор ответственного секретаря
        //this.ProtocolResponsibleSecretaryLinkValue = ko.observable(this.ProtocolResponsibleSecretaryLink() ? this.ProtocolResponsibleSecretaryLink().get_lookupValue() : null);
        //this.ProtocolResponsibleSecretaryLinkId = ko.computed({
        //    read: function () { return this.ProtocolResponsibleSecretaryLink() ? this.ProtocolResponsibleSecretaryLink().get_lookupId() : null; },
        //    write: function (json) {
        //        var parsedJson = $.parseJSON(json);
        //        this.ProtocolResponsibleSecretaryLink(getNewParticipantLookupValue(parsedJson[0].Id));
        //        this.ProtocolResponsibleSecretaryLinkValue(parsedJson[0].ParticipantFullName);
        //    }
        //}, this);
        //this.selectResponsibleSecretary = function () { openParticipantSelectDialog("Выбор ответственного секретаря", "#hidden-responsible-secretary", false); }.bind(this);

        //выбор секретаря
        //this.SecretaryFullNameLinkValue = ko.observable(this.SecretaryFullNameLink() ? this.SecretaryFullNameLink().get_lookupValue() : null);
        //this.SecretaryFullNameLinkId = ko.computed({
        //    read: function () { return this.SecretaryFullNameLink() ? this.SecretaryFullNameLink().get_lookupId() : null; },
        //    write: function (json) {
        //        var parsedJson = $.parseJSON(json);
        //        this.SecretaryFullNameLink(getNewParticipantLookupValue(parsedJson[0].Id));
        //        this.SecretaryFullNameLinkValue(parsedJson[0].ParticipantFullName);
        //    }
        //}, this);
        //this.selectSecretary = function () { openParticipantSelectDialog("Выбор секретаря", "#hidden-secretary", false); }.bind(this);

        //additional participants selection
        //this.additionalParticipants = ko.observableArray(getParticipiantsEntities(data.ProtocolExtParticipants));
        //this.selectAdditionalParticipants = function () { openParticipantSelectDialog("Выбор остальных участников", "#hidden-additioanl-participants", true); }.bind(this);
        //this.AdditionalParticipantsInput = ko.computed({
        //    read: function () {
        //        return "";
        //    },
        //    write: function (json) {
        //        var parsedJson = $.parseJSON(json);
        //        for (i = 0; i < parsedJson.length; i++) {
        //            var item = new participantEntity(parsedJson[i]);
        //            if (!ko.utils.arrayFirst(this.additionalParticipants(), function (arrItem) { return item.Id() == arrItem.Id(); })) {
        //                this.additionalParticipants.push(item);
        //                if (this.ProtocolExtParticipants() == null) this.ProtocolExtParticipants([]);
        //                var lookup = new SP.FieldLookupValue();
        //                lookup.set_lookupId(item.Id());
        //                this.ProtocolExtParticipants().push(lookup);
        //            }
        //        }
        //    }
        //}, this);

        //this.removeAdditionalPartcipant = function (participant) {
        //    this.additionalParticipants.remove(participant);
        //    this.ProtocolExtParticipants($.grep(this.ProtocolExtParticipants(), function (n, i) {
        //        return n.get_lookupId().toString() != participant.Id().toString();
        //    }));
        //}.bind(this);
    }

    // load entity data
    function loadSPEntity(fields, SPEntity) {
        var entity = {};

        for (var i = 0; i < fields.length; i++) {
            var fieldName = fields[i].get_internalName();
            try {
                if (fields[i].get_typeAsString() == 'DateTime') {
                    entity[fieldName] = formatDate(SPEntity.get_item(fieldName));
                } else {
                    entity[fieldName] = SPEntity.get_item(fieldName);
                }
            } catch (e) {
                entity[fieldName] = "Не удалось получить " + fieldName;
                if (console) {
                    console.log("Не удалось обнаружить поле " + fieldName + ": " + e.message);
                }
            }
        }
        entity.Id = SPEntity.get_item("ID");

        // by some reasons this filed doesn't loaded by CSOM for some profiles
        try {
            entity.Author = SPEntity.get_item("Author").get_lookupValue();
        } catch (e) {
            if (console) {
                console.log("Не удалось обнаружить поле Author: " + e.message);
            }
        }

        entity.CreatedDate = formatDate(SPEntity.get_item("Created"));
        entity.New = false;
        return entity;
    }

    // save entity data
    function saveSPEntity(fields, koItem, SPEntity) {
        for (var i = 0; i < fields.length; i++) {
            var fieldName = fields[i].get_internalName();
            if (fieldName.toLowerCase().indexOf("date") >= 0) {
                if (koItem[fieldName]()) {
                    SPEntity.set_item(fieldName, parseDate(koItem[fieldName]()));
                }
                else {
                    SPEntity.set_item(fieldName, null);
                }
            }
            else {
                SPEntity.set_item(fieldName, koItem[fieldName]());
            }
        }
    }

    // new model entity
    function getNewEntity(fields) {
        var newEntity = {};
        for (var i = 0; i < fields.length; i++) {
            var fieldName = fields[i].get_internalName();

            newEntity[fieldName] = fields[i].get_defaultValue();
        }
        newEntity.Id = "";
        newEntity.New = true;

        return newEntity;
    }

    // new entity from ko object
    function newFromKOEntity(fields, koEntity) {
        var newEntity = {};
        for (var i = 0; i < fields.length; i++) {
            var fieldName = fields[i].get_internalName();

            newEntity[fieldName] = koEntity[fieldName]();
        }
        return newEntity;
    }

    function meetingViewModel() {
        var self = this;
        self.initFileReader = function (elem) {
        };
        self.allProjectsTypes = ko.observableArray([]);
        self.allQuestions = ko.observableArray([]);
        self.currentMeetingsNumber = ko.observable();
        self.meeting = ko.observable({});
        self.issues = ko.observableArray([]);
        self.issueObjects = ko.observableArray([]);
        /*self.issueTypes = ko.observableArray(getAvailableQuestionTypes());
        self.assignmentTypes = ko.observableArray(getAvailableAssignmentTypes());*/
        self.issueTypes = ko.observableArray([]);
        self.assignmentTypes = ko.observableArray([]);
        self.assignments = ko.observableArray([]);
        self.filteredAssignments = function (questionValue) {
            return ko.utils.arrayFilter(self.assignments(), function (assignment) {
                return (assignment.editIssueLinkValue().toString() === questionValue.toString());
            });
        };
        self.availableMeetingStatuses = ko.observableArray([]);
        self.assignmentStatuses = ko.observableArray([]);
        self.assignmentReportResolutions = ko.observableArray([]);
        self.assignmentInspectStates = ko.observableArray([]);
        self.assignmentDayTypes = ko.observableArray([]);
        self.availableAttachDocTypes = ko.observableArray([]);
        self.attachments = ko.observableArray([]);
        self.scanAttach = ko.observable(new meetingAttach({
            AttachmentDescription: "",
            MeetingLink: "",
            Id: "",
            FileUrl: "",
            FileName: "",
            FilePath: "",
            AttachmentDocumentTypeLinkValue: "",
            New: true
        }));
        self.editEnabled = ko.observable();
        self.isReporter = ko.observable();

        var meetingList = getListByUrl(modelMetaData.meeting.listName);
        var issueList = getListByUrl(modelMetaData.issue.listName);
        //var questionObjectList = getListByUrl(modelMetaData.issueObject.listName);
        var attachList = getListByUrl(modelMetaData.meetingAttachment.listName);

        var assignmentList; // = $.appWebContext.get_web().get_lists().getByTitle("Поручения");
        var attachDocTypeList = $.appWebContext.get_web().get_lists().getByTitle("Типы документов вложений");
        var questionsProjectTypesList = $.appWebContext.get_web().get_lists().getByTitle("Типы проектов вопросов повестки");

        // spinner
        var spinner = new Spinner(spinOptions);


        loadMetadataInfo(function () {
            getCurrentUser();
            self.getPermissions();
            self.loadData();
        });

        self.getPermissions = function () {
            self.editEnabled(false);
        }

        self.loadData = function () {
            self.availableMeetingStatuses(getChoicesFromMetadata(modelMetaData.meeting.fields, "MeetingStatusLand"));

            //load attach document types book
            var dtquery = new SP.CamlQuery();
            var attachDocTypeListInst = attachDocTypeList.getItems(dtquery);
            var questionsProjectTypes = questionsProjectTypesList.getItems(dtquery);
            $.appWebContext.load(attachDocTypeListInst);
            $.appWebContext.load(questionsProjectTypes);
            $.appWebContext.executeQueryAsync(function () {
                var enumerator = attachDocTypeListInst.getEnumerator();
                while (enumerator.moveNext()) {
                    self.availableAttachDocTypes().push({
                        id: enumerator.get_current().get_item("ID"),
                        name: enumerator.get_current().get_item("Title")
                    });
                }
                enumerator = questionsProjectTypes.getEnumerator();
                var ptArray = [];
                while (enumerator.moveNext()) {
                    ptArray.push({ label: enumerator.get_current().get_item("Title"), value: enumerator.get_current().get_item("Title") });
                }
                self.allProjectsTypes(ptArray);
            });

            self.currentMeetingsNumber(getCurrentMeetingsNumber());
            if ($.pageMode == "new" || $.listItemId == "undefined") {
                self.meeting(new meeting(getNewEntity(modelMetaData.meeting.fields)));
                self.meeting().MeetingNumber(self.currentMeetingsNumber() + 1);
            }
            else {
                // loading meeting properties
                var query = new SP.CamlQuery();
                query.set_viewXml("<View><Query><Where><Eq><FieldRef Name='ID'/><Value Type='Text'>" + $.listItemId + "</Value></Eq></Where></Query></View>");
                var meetingInstance = meetingList.getItems(query);
                $.appWebContext.load(meetingInstance);

                // loading issue questions
                var aqCamlQuery = new CamlBuilder().Where().LookupField("IssueMeetingLand").Id().EqualTo($.listItemId).OrderBy("IssueNumberLand");
                var spaqQuery = new SP.CamlQuery();
                spaqQuery.set_viewXml("<View><Query>" + aqCamlQuery.ToString() + "</Query></View>");
                var issueListInst = issueList.getItems(spaqQuery);
                $.appWebContext.load(issueListInst);

                // loading attachments
                var atCamlQuery = new CamlBuilder().Where().LookupField("MeetingAttachmentMeetingLand").Id().EqualTo($.listItemId);
                var spatQuery = new SP.CamlQuery();
                spatQuery.set_viewXml("<View><Query>" + atCamlQuery.ToString() + "</Query></View>");
                var attachListInst = attachList.getItems(spatQuery);
                $.appWebContext.load(attachListInst);

                var spinTarget = document.getElementById("IssueTableDiv");
                if (spinTarget) {
                    $(spinTarget.children).each(function (i, e) {
                        $(e).hide();
                    });
                    $(spinTarget).css("min-height", "150px");
                    spinner.spin(spinTarget);
                }

                $.appWebContext.executeQueryAsync(function () {
                    try {
                        self.meeting(new meeting(loadSPEntity(modelMetaData.meeting.fields, meetingInstance.get_data()[0])));

                        //load questions to model
                        var questions = [];
                        var allMeetingBaseParticipants = [];
                        var enumerator = issueListInst.getEnumerator();
                        while (enumerator.moveNext()) {
                            var newQuest = new issue(loadSPEntity(modelMetaData.issue.fields, enumerator.get_current()));
                            if (newQuest.IssueReporterLand && newQuest.IssueReporterLand() != null)
                                allMeetingBaseParticipants.push(newQuest.IssueReporterLand());
                            if (newQuest.IssueCoReportersLand() != null)
                                $.each(newQuest.IssueCoReportersLand(),
                                    function () { allMeetingBaseParticipants.push(this); });
                            questions.push(newQuest);
                        }
                        self.issues(questions);

                        //load attachments file info
                        var attachs = [];
                        var scancopyAttach, attach;
                        enumerator = attachListInst.getEnumerator();

                        //get file container items
                        while (enumerator.moveNext()) {
                            var docTypeLink = enumerator.get_current().get_item("MeetingAttachmentDocTypeLand");
                            var fileInfo = loadListItemAttachmentInfo(attachList.get_id().toString(), enumerator.get_current().get_item("ID"));

                            attach = new meetingAttach({
                                AttachmentDescription: enumerator.get_current().get_item("MeetingAttachmentDescriptionLand"),
                                MeetingLink: enumerator.get_current().get_item("MeetingAttachmentMeetingLand"),
                                Id: enumerator.get_current().get_item("ID"),
                                FileUrl: fileInfo.FileUrl,
                                FileName: fileInfo.FileName,
                                FilePath: "",
                                AttachmentDocumentTypeLinkValue: docTypeLink ? docTypeLink.get_lookupId() : "",
                                New: false
                            });

                            if (enumerator.get_current().get_item("MeetingAttachmentProtocolCopyLan")) {
                                scancopyAttach = attach;
                            }
                            else {
                                attachs.push(attach);
                            }
                        }
                        self.attachments(attachs);
                        if (scancopyAttach) self.scanAttach(scancopyAttach);

                        var camlQuery = new CamlBuilder().Where().LookupField("_x0412__x043e__x043f__x0440__x04").Id().In($.map(self.issues(), function (el) { return el.Id(); }));
                        //var camlQuery = new CamlBuilder().Where().LookupField("_x0412__x043e__x043f__x0440__x04").Id().EqualTo(883);
                        var spQuery = new SP.CamlQuery();
                        spQuery.set_viewXml("<View><Query>" + camlQuery.ToString() + "</Query></View>");
                        //spQuery.set_viewXml("<View><Query><Where><Eq><FieldRef Name='_x0412__x043e__x043f__x0440__x04'/><Value Type='Text'>2553</Value></Eq></Where></Query></View>");
                        //var questionObjects = questionObjectList.getItems(spQuery);
                        //$.appWebContext.load(questionObjects);

                        // loading assignments
                        //var camlQuery = new CamlBuilder().Where().LookupField("IssueLink").Id().In($.map(self.issues(), function (el) { return el.Id(); }));
                        //var spQuery = new SP.CamlQuery();
                        //spQuery.set_viewXml("<View><Query>" + camlQuery.ToString() + "</Query></View>");
                        //var assignmentListInst = assignmentList.getItems(spQuery);
                        //$.appWebContext.load(assignmentListInst);

                        $.appWebContext.executeQueryAsync(function () {
                            var objects = [];
                            var objectsData = [];
                            //var enumerator = questionObjects.getEnumerator();
                            //while (enumerator.moveNext()) {
                            //    var od = loadSPEntity(modelMetaData.issueObject.fields, enumerator.get_current());
                            //    var newObject = new issueObject(od);
                            //    objectsData.push(od);
                            //    objects.push(newObject);
                            //}
                            //self.issueObjects(objects);

                            //for (var i = 0; i < questions.length; i++) {
                            //    var q = questions[i];
                            //    var fo;
                            //    for (var j = 0; j < objects.length; j++) {
                            //        if (objects[j]._x0412__x043e__x043f__x0440__x04().get_lookupId() == q.Id())
                            //            fo = objects[j];
                            //    }
                            //    if (fo)
                            //        q.issueObjects.push(fo);
                            //}

                            var items = [];
                            self.assignments(items);

                            //load participants for assignments and questions
                            modelAllBaseParticipants = getParticipiantsEntities(allMeetingBaseParticipants);
                            $.each(self.issues(), function () {
                                //this.loadReporter();
                                //this.loadSoreporters();
                            });
                            $.each(self.assignments(), function () {
                                this.loadExecutor();
                                this.loadSoexecutors();
                            });
                        });
                    }
                    catch (e) {
                        alert(e.message);
                    }
                    finally {
                        spinner.stop();
                        if (spinTarget) {
                            $(spinTarget.children).each(function (i, e) {
                                $(e).show();
                            });
                            $(spinTarget).css("min-height", "0px");
                        }
                    }

                }, function (sender, args) {
                    spinner.stop();
                    if (spinTarget) {
                        $(spinTarget.children).each(function (i, e) {
                            $(e).show();
                        });
                        $(spinTarget).css("min-height", "0px");
                    }
                    alert('Request failed. ' + args.get_message() +
                        '\n' + args.get_stackTrace());
                });
            }
        };

        // actions
        self.selectedIssue = ko.observable();
        self.selectedAssignment = ko.observable();

        ///////////////////////////////////////////////////////////////////////
        // Assignment

        // edit assignment
        self.editAssignment = function (assignmentQuestionToEdit) {
            assignmentQuestionToEdit.selectedRequest(undefined);
            self.selectedAssignment(assignmentQuestionToEdit);
        };

        // add assignment
        self.addAssignment = function () {
            var e = getNewEntity(modelMetaData.assignment.fields);
            var newKoEntity = new Assignment(e);
            self.selectedAssignment(newKoEntity);
            self.selectedAssignment().editAssignmentNumber(self.assignments().length + 1);
            self.selectedAssignment().Id("");
            self.assignments.push(self.selectedAssignment());
        };

        // remove assignment
        self.removeAssignment = function (assignment) {
            self.assignments.destroy(assignment);
        };

        // accept assignment
        self.acceptAssignment = function () {
            var selected = self.selectedAssignment();
            if (checkIfNumberValid(selected.editAssignmentNumber())) {
                selected.accept();
                if (selected.New) selected.New = false;
                self.selectedAssignment("");
            } else {
                alert("Номер поручения должен быть числом > 0!");
            }
        };

        // cancel changes in assignment
        self.cancelAssignment = function () {
            if (self.selectedAssignment().New) {
                self.assignments.remove(self.selectedAssignment());
            } else {
                self.selectedAssignment().cancel();
            }
            self.selectedAssignment("");
        };

        ///////////////////////////////////////////////////////////////////////
        // Agenda Question

        // edit issue question
        self.editIssue = function (issueToEdit) {
            issueToEdit.mode("full");
            self.selectedIssue(issueToEdit);
        };

        // edit issue question
        self.editIssueOnlyFiles = function (issueToEdit) {
            issueToEdit.mode("onlyFiles");
            self.selectedIssue(issueToEdit);
        };

        // add issue question
        self.addIssue = function () {
            var e = getNewEntity(modelMetaData.issue.fields);
            e.MeetingLink = self.meeting().MeetingNumber();
            self.selectedIssue(new issue(e));
            self.selectedIssue().editIssueNumber(self.issues().length + 1);
            self.selectedIssue().mode("full");
        };

        self.addOrderRg = function () {
            openOrderRgAddDialog("Добавить заявки");
        }

        self.createIssueRg = function () {
            openIssueRgCreateDialog("Создать вопрос");
        }

        // remove issue question
        self.removeIssue = function (issue) {
            var currentNumber = issue.IssueNumberLand();
            var questions = self.issues();

            var number = currentNumber;
            for (var i = questions.indexOf(issue) + 1; i < questions.length; i++) {
                var listItem = issueList.getItemById(questions[i].Id());
                listItem.set_item('IssueNumberLand', number);
                listItem.update();
                number++;
            }
            var listItem = issueList.getItemById(issue.Id());
            listItem.deleteObject();

            $.appWebContext.executeQueryAsync(function () {
                number = currentNumber;
                for (var i = questions.indexOf(issue) + 1; i < questions.length; i++) {
                    questions[i].IssueNumberLand(number);
                    number++;
                }
                self.issues.remove(issue);
            },
			function (sender, args) {
			    alert('Request failed. ' + args.get_message() +
                    '\n' + args.get_stackTrace());
			});
        };

        self.canMoveUpIssue = function (issue) {
            return self.issues().indexOf(issue) > 0;
        };

        self.canMoveDownIssue = function (issue) {
            return self.issues().indexOf(issue) < self.issues().length - 1;
        };

        self.moveIssue = function (current, isUp) {
            console.log((isUp ? "Up" : "Down") + " question " + current.Id());
            var direction = isUp ? -1 : 1;
            var prev = self.issues()[self.issues().indexOf(current) + direction];
            var currentNumber = current.IssueNumberLand();
            var prevNumber = prev.IssueNumberLand();

            var currentListItem = issueList.getItemById(current.Id());
            var prevListItem = issueList.getItemById(prev.Id());
            $.appWebContext.load(prevListItem);
            $.appWebContext.load(currentListItem);

            $.appWebContext.executeQueryAsync(function () {
                console.log("Questions updated success");
                prevListItem.set_item('IssueNumberLand', currentNumber);
                currentListItem.set_item('IssueNumberLand', prevNumber);
                prevListItem.update();
                currentListItem.update();

                $.appWebContext.executeQueryAsync(function () {
                    console.log("Move question " + current.Id() + " success");
                    prev.IssueNumberLand(currentNumber);
                    current.IssueNumberLand(prevNumber);
                    self.issues(self.issues().sort(function (a, b) {
                        return a.IssueNumberLand() > b.IssueNumberLand() ? 1 : -1;
                    }))
                },
                function (sender, args) {
                    alert('Request failed. ' + args.get_message() +
                        '\n' + args.get_stackTrace());
                });
            },
				function (sender, args) {
				    alert('Request failed. ' + args.get_message() +
						'\n' + args.get_stackTrace());
				});
        }

        self.moveUpIssue = function (current) {
            self.moveIssue(current, true);
        };

        self.moveDownIssue = function (current) {
            self.moveIssue(current, false);
        };

        // accept issue question
        self.acceptIssue = function () {
            var selected = self.selectedIssue();
            if (checkIfNumberValid(selected.editIssueNumber())) {
                selected.accept();

                if (selected.New) {
                    var e = newFromKOEntity(modelMetaData.issue.fields, selected);
                    // для вновь добавляемого вопроса нет данных (полного имени) для докладчиков, 
                    // они хранятся отдельно в свойствах вопроса, но не относятся к хранимой части модели
                    // поэтому их нужно перенести
                    e.issueReporterFIO = selected.issueReporterFIO();
                    e.issueSoreporters = selected.issueSoreporters();

                    // new object to be created.
                    var listItem = issueList.addItem(new SP.ListItemCreationInformation());
                    saveSPEntity(modelMetaData.issue.fields, selected, listItem);
                    listItem.update();
                    $.appWebContext.load(listItem);
                    $.appWebContext.executeQueryAsync(function () {
                        e.Id = listItem.get_id();
                        e.New = false;
                        e.MeetingLink = self.meeting().MeetingNumber();
                        self.issues.push(new issue(e));
                        self.selectedIssue("");
                    });
                } else self.selectedIssue("");
            } else {
                alert("Номер вопроса должен быть числом > 0!");
            }
        };

        // cancel changes in issue question
        self.cancelIssue = function () {
            self.selectedIssue().cancel();
            self.selectedIssue("");
        };

        ///////////////////////////////////////////////////////////////////////
        // Meeting Attach

        // add meeting attach
        self.addAttach = function () {
            self.attachments.push(new meetingAttach({
                Id: "",
                AttachmentDescription: "",
                MeetingLink: self.meeting().MeetingNumber(),
                FileUrl: "",
                FileName: "",
                FilePath: "",
                AttachmentDocumentTypeLinkValue: "",
                New: true
            }));
        };

        // remove meeting attach
        self.removeAttach = function (attach) {
            self.attachments.destroy(attach);
        };

        // delete scan attach
        self.deleteScanAttach = function () {
            // Handle deleted objects
            var listItem = attachList.getItemById(self.scanAttach().Id());
            listItem.deleteObject();

            $.appWebContext.executeQueryAsync(function () {
                self.scanAttach(new meetingAttach({
                    AttachmentDescription: "",
                    MeetingLink: self.meeting().Id(),
                    Id: "",
                    FileUrl: "",
                    FileName: "",
                    FilePath: "",
                    AttachmentDocumentTypeLinkValue: "",
                    New: true
                }));
            });
        };

        self.showPlaceOnMap = function () {
            if (self.meeting().MeetingPlace() != "") {
                window.open("http://maps.yandex.ru/?text=" + self.meeting().MeetingPlace(), "_blank");
            }
        };

        self.exportToWord = function () {
            __doPostBack('Export.WordByTemplate', JSON.stringify({ ListName: 'MeetingLandList', ListItemId: $.listItemId }));
        };

        self.closeForm = function () {
            window.location.href = $.backUrl;
        };

        // save the entire meeting
        self.save = function () {
            if (isNaN(self.meeting().MeetingNumber())) {
                alert("Ошибка при сохранении! Номер заседания должен быть числом!");
                return;
            }

            var createdMeeting;
            // meeting's properties
            if (self.meeting().New) {
                // handle new meeting
                if (self.currentMeetingsNumber() > self.meeting().MeetingNumber()) {
                    alert("Ошибка при сохранении! Заданный номер заседания меньше последнего существующего!");
                    return;
                }

                setCurrentMeetingsNumber(self.meeting().MeetingNumber());
                var listItem = meetingList.addItem(new SP.ListItemCreationInformation());
                saveSPEntity(modelMetaData.meeting.fields, self.meeting(), listItem);
                listItem.update();
                createdMeeting = listItem;

                $.appWebContext.load(listItem);
            }
            else {
                // handle updating
                var meetingItem = meetingList.getItemById(self.meeting().Id());
                saveSPEntity(modelMetaData.meeting.fields, self.meeting(), meetingItem);
                meetingItem.update();
            }

            // in order to get meeting Id we need to save it
            $.appWebContext.executeQueryAsync(
            function () {
                if (createdMeeting) {
                    self.meeting().Id(createdMeeting.get_id());
                    self.meeting().New = false;
                }

                // issue questions
                var createdIssues = [];
                for (var issue in self.issues()) {
                    // Build a request up to send with the CSOM.
                    if (self.issues()[issue]._destroy) {
                        // Handle deleted objects
                        // Deleted items that are marked "new" have never been saved to SharePoint to start with
                        if (self.issues()[issue].Id() != "") {
                            var listItem = issueList.getItemById(self.issues()[issue].Id());
                            listItem.deleteObject();
                        }
                    } else {
                        // The item is neither new nor deleted, handle it as an update.
                        var listItem = issueList.getItemById(self.issues()[issue].Id());
                        saveSPEntity(modelMetaData.issue.fields, self.issues()[issue], listItem);
                        // link to parent
                        listItem.set_item("MeetingLink", self.meeting().Id());
                        listItem.update();
                        var newProjectType = saveProjectTypeIfNew(self.allProjectsTypes(), self.issues()[issue].IssueProjectType(), questionsProjectTypesList);
                        if (newProjectType != null) self.allProjectsTypes.push({ label: newProjectType, value: newProjectType });
                    }
                }

                // assignments
                var createdAssignments = [];
                for (var assignment in self.assignments()) {
                    // Build a request up to send with the CSOM.
                    if (self.assignments()[assignment]._destroy) {
                        // Handle deleted objects
                        // Deleted items that are marked "new" have never been saved to SharePoint to start with
                        if (self.assignments()[assignment].Id() != "") {
                            var listItem = assignmentList.getItemById(self.assignments()[assignment].Id());
                            listItem.deleteObject();
                        }
                    } else {
                        var listItem = self.assignments()[assignment].Id() == "" ? assignmentList.addItem(new SP.ListItemCreationInformation()) : assignmentList.getItemById(self.assignments()[assignment].Id());
                        saveSPEntity(modelMetaData.assignment.fields, self.assignments()[assignment], listItem);
                        // link to parent
                        var linkedQuestions = $.grep(self.issues(), function (e) { return e.IssueNumber() == self.assignments()[assignment].IssueLinkValue(); });
                        if (linkedQuestions.length > 0) {
                            if (linkedQuestions.length > 1) console.log("Duplicate issue question numbers in this meeting");
                            listItem.set_item("IssueLink", linkedQuestions[0].Id());
                        }
                        listItem.update();
                        if (self.assignments()[assignment].Id() == "") {
                            // Handle new objects to be created.
                            createdAssignments.push({ spItem: listItem, koItem: self.assignments()[assignment] });
                            $.appWebContext.load(listItem);
                        }
                    }
                }

                // meeting attachs
                var createdAttachments = [];
                var createdScanAttach = [];
                for (var attach in self.attachments()) {
                    if (self.attachments()[attach]._destroy) {
                        // Handle deleted objects
                        if (self.attachments()[attach].Id() != "") {
                            var listItem = attachList.getItemById(self.attachments()[attach].Id());
                            listItem.deleteObject();
                        }
                    } else {
                        var listItem = (self.attachments()[attach].Id() == "") && (self.attachments()[attach].File() != undefined) ? attachList.addItem(new SP.ListItemCreationInformation()) : attachList.getItemById(self.attachments()[attach].Id());
                        listItem.set_item("AttachmentDescription", self.attachments()[attach].Descr());
                        listItem.set_item("MeetingLink", self.meeting().Id());
                        listItem.set_item("AttachmentDocumentTypeLink", self.attachments()[attach].DType());
                        listItem.set_item("AttachmentProtocolScanCopy", false);
                        listItem.update();

                        if (self.attachments()[attach].Id() == "" && self.attachments()[attach].File() != undefined) {
                            // Save a reference to both the SP.ListItem object and the KO Object so we can update
                            // the latter with the former's ID once the object has been created.
                            createdAttachments.push({ spItem: listItem, koItem: self.attachments()[attach] });
                            $.appWebContext.load(listItem);
                        }
                    }
                }

                // scan attach
                if (self.scanAttach()._destroy) {
                    // todo
                } else {
                    if (self.scanAttach().File() != undefined) {
                        var listItem = self.scanAttach().Id() == "" ? attachList.addItem(new SP.ListItemCreationInformation()) : attachList.getItemById(self.scanAttach().Id());
                        listItem.set_item("AttachmentDescription", self.scanAttach().Descr());
                        listItem.set_item("MeetingLink", self.meeting().Id());
                        listItem.set_item("AttachmentProtocolScanCopy", true);
                        listItem.update();
                        if (self.scanAttach().Id() == "") {
                            // Save a reference to both the SP.ListItem object and the KO Object so we can update
                            // the latter with the former's ID once the object has been created.
                            createdScanAttach.push({ spItem: listItem, koItem: self.scanAttach() });
                            $.appWebContext.load(listItem);
                        }
                    }
                }

                // Now we have built our request, send it to the server for processing.
                $.appWebContext.executeQueryAsync(function () {
                    // delete from model destroyed questions
                    for (var i = self.issues().length - 1; i >= 0; i--) if (self.issues()[i]._destroy) self.issues.splice(i, 1);

                    //upload attachments
                    uploadAttachments(createdScanAttach, "Вложения заседаний");
                    uploadAttachments(createdAttachments, "Вложения заседаний");

                    // save was successful. Now we need to itterate through our newly
                    // created items and ensure that Knockout knows that the ID has changed.
                    for (var item in createdAssignments) {
                        createdAssignments[item].koItem.Id(createdAssignments[item].spItem.get_id());
                        createdAssignments[item].koItem.New = false;
                    }

                    // delete from model destroyed assignments
                    for (var i = self.assignments().length - 1; i >= 0; i--) if (self.assignments()[i]._destroy) self.assignments.splice(i, 1);
                    // delete from model destroyed attachments
                    for (var i = self.attachments().length - 1; i >= 0; i--) if (self.attachments()[i]._destroy) self.attachments.splice(i, 1);

                    saveAssignmentsJournal(self.assignments());
                    var newCreatedReports = saveAssignmentsReports(self.assignments());
                    $.appWebContext.executeQueryAsync(function () {
                        for (var item in newCreatedReports) {
                            newCreatedReports[item].koItem.Id(newCreatedReports[item].spItem.get_id());
                            newCreatedReports[item].koItem.New = false;
                        }

                        var newReportsAttachments = saveReportsAttachments(self.assignments());
                        $.appWebContext.executeQueryAsync(function () {
                            uploadAttachments(newReportsAttachments, "Вложения отчета по поручению");
                            $.appWebContext.executeQueryAsync(function () { alert("Успешно сохранено"); }, function () { alert("Ошибка при сохранении вложений Отчетов!"); });
                        }, function () { alert("Ошибка при сохранении вложений Отчетов!"); });
                    }, function () { alert("Не могу сохранить данные отчётов!"); });
                },
                    function (sender, args) {
                        alert("Ошибка обновления данных заседания");
                        console.log(args.get_message());
                    });
            },
                    function (sender, args) {
                        alert("Error saving new meeting");
                        console.log(args.get_message());
                    });
        };
    }

    // custom binding for buttons with icons
    ko.bindingHandlers.iconButton = {
        init: function (element, valueAccessor) {
            var options = valueAccessor() || {};
            $(element).button(options);
        }
    };

    ko.bindingHandlers.datePicker = {
        init: function (element, valueAccessor) {
            var options = valueAccessor() || {};
            $(element).datepicker(options);
        }
    };

    ko.bindingHandlers.accordion = {
        init: function (element, valueAccessor) {
            var options = valueAccessor() || {};
            $(element).accordion(options);
        }
    };

    // custom binding for jquery dialogs
    ko.bindingHandlers.jqDialog = {
        init: function (element, valueAccessor) {
            var options = ko.utils.unwrapObservable(valueAccessor()) || {};

            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $(element).dialog("destroy");
            });

            $(element).dialog(options);
        }
    };

    //custom binding handler for dialog open/close
    ko.bindingHandlers.openDialog = {
        update: function (element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            if (value) {
                $(element).dialog("open");
            } else {
                $(element).dialog("close");
            }
        }
    };

    //custom binding dialog buttons init
    ko.bindingHandlers.jqDialogButton = {
        init: function (element, valueAccessor) {
            var options = ko.utils.unwrapObservable(valueAccessor()) || {};

            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $(element).button("destroy");
            });

            $(element).button(options);
        }
    };

    //custom binding dialog for bootstrap
    ko.bindingHandlers.showModal = {
        init: function (element, valueAccessor) {
        },
        update: function (element, valueAccessor) {
            var value = valueAccessor();
            if (ko.utils.unwrapObservable(value)) {
                $(element).modal('show');
            }
            else {
                $(element).modal('hide');
            }
        }
    };

    ko.bindingHandlers.jqAuto = {
        init: function (element, params) {
            $(element).autocomplete(params());
        },
        update: function (element, params) {
            $(element).autocomplete("option", "source", params().source);
        }
    };

    var allLists;
    function loadLists(callback) {
        allLists = $.appWebContext.get_web().get_lists();
        $.appWebContext.load(allLists, 'Include(Id,EntityTypeName)');
        $.appWebContext.executeQueryAsync(function () {
            for (var i = 0; i < allLists.get_count() ; i++) {
                var item = allLists.get_item(i);
                item.name = item.get_entityTypeName();
            };
            callback();
        }, function (sender, args) {
            alert('Request failed. ' + args.get_message() +
                '\n' + args.get_stackTrace());
        });
    }
    function getListByUrl(name) {
        for (var i = 0; i < allLists.get_count() ; i++) {
            var item = allLists.get_item(i);
            if (item.name.indexOf(name) == 0)
                return item;
        }
        console.log("Не найден список " + name);
        return null;
    }

    function sharepointReady() {
        $.appWebContext = SP.ClientContext.get_current();

        loadLists(function () {
            SP.SOD.executeOrDelayUntilScriptLoaded(function () {
                ko.applyBindings(new meetingViewModel());
            }, "sp.js");
        });
    }

    $(document).ready(function () {

        $.hostweburl = decodeURIComponent($.getUrlVar("SPHostUrl"));
        $.pageMode = decodeURIComponent($.getUrlVar("mode"));
        $.backUrl = decodeURIComponent($.getUrlVar("Source"));
        $.listItemId = decodeURIComponent($.getUrlVar("ID"));

        // локализация datepicker
        moment.lang('ru');

        SP.SOD.executeFunc("sp.js", "SP.ClientContext", sharepointReady);
    });

})();
