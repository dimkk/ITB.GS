/// <reference path="renderCore.js" />
/// <reference path="../SP.debug.js" />
/// <reference path="../SP.Core.debug.js" />
/// <reference path="../SP.runtime.debug.js" />

(function () {

    var author, editor, created, modified;
    var exceptList = [];
    var renderCore;

    function getMeetingControl() {
        return renderCore.getControlByFieldName('MeetingLink');
    }

    function getNumberControl() {
        return renderCore.getControlByFieldName('AgendaQuestionNumber');
    }

    function init() {
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides({
            Templates: {
                Item: renderFields
            },
            OnPostRender: OnPostRender,
            ListTemplateType: 10000,
        });
    }

    function renderItemHeader(context) {
        var resultHtml = '';
        resultHtml += '<div class="container" style="margin-top: 25px;">';

        return resultHtml;
    }

    function renderItemFooter(context) {
        var resultHtml = '';
        resultHtml += '</div>';

        return resultHtml;
    }

    function renderFields(context) {
        renderCore = window.renderCore && window.renderCore.init(context);
        if (!renderCore) {
            console.error('Не удалось инициализировать renderCore');
            return;
        }

        var builderButtonSpan = context.ControlMode === SPClientTemplates.ClientControlMode.DisplayForm ? 2 : 1;

        var resultHtml = '';
        resultHtml += renderItemHeader(context);
        resultHtml += '<div class="form-horizontal" role="form">';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 10, "IssueAddressAk");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 10, "AgendaQuestionDescription");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 4, "IssueDeclarantAk");
        resultHtml += renderFieldBlock(context, builderButtonSpan, 4, "IssueCustomerAk");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, builderButtonSpan, 4, "IssueInvestorAk");
        resultHtml += renderFieldBlock(context, 2, 4, "IssueProjectOrgAk");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 4, "CadastreNumber");
        resultHtml += renderFieldBlock(context, 2, 4, "QuestionCategoryLink");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 4, "IssueReporterAk");
        resultHtml += renderFieldBlock(context, 2, 4, "IssueDecisionTypeAk");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 4, "IssueMunicipalityAk");
        resultHtml += renderFieldBlock(context, 2, 4, "IssueSettlementAk");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 10, "AgendaQuestionProtocolDecision");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 4, "AgendaQuestionNumber");
        resultHtml += renderFieldBlock(context, 2, 4, "MeetingLink");
        resultHtml += '</div>';

        if (context.ControlMode === SPClientTemplates.ClientControlMode.DisplayForm) {
            resultHtml += '<div class="form-group">';
            resultHtml += renderFieldBlock(context, 2, 10, "AgendaQuestionIssue");
            resultHtml += '</div>';
        }

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 4, "IssueStatusAk");
        resultHtml += '</div>';

        author = context.RenderFieldByName(context, "Author");
        exceptList.push("Author");
        created = context.RenderFieldByName(context, "Created");
        exceptList.push("Created");
        editor = context.RenderFieldByName(context, "Editor");
        exceptList.push("Editor");
        modified = context.RenderFieldByName(context, "Modified");
        exceptList.push("Modified");

        resultHtml += '</div>'; //form-horizontal
        resultHtml += renderItemFooter(context);

        return resultHtml;
    }

    var AllMunicipalities;
    var SettlementOptions;
    function GetMunicipalityControl() {
        return $('[id^="IssueMunicipalityAk"]');
    }
    function GetSettlementControl() {
        return $('[id^="IssueSettlementAk"]');
    }
    function InitMunicipality() {
        var municipalityControl = GetMunicipalityControl();
        var settlementControl = GetSettlementControl();

        AllMunicipalities = SC.GetItems('Municipality', new SP.CamlQuery());
        SC.Execute(function () {
            AllMunicipalities = AllMunicipalities.get_data();
            console.log(AllMunicipalities.length + ' Municipalities loaded');

            var municipalitiesId = $.map(AllMunicipalities, function (e) {
                return !e.get_item('MunicipalityParentMunicipality') ? e.get_id().toString() : null;
            });

            municipalityControl.find('option').filter(function () {
                return this.value != '0' && $.inArray(this.value, municipalitiesId) == -1;
            }).remove();

            settlementControl.find('option').filter(function () {
                return this.value != '0' && $.inArray(this.value, municipalitiesId) == 1;
            }).remove();

            SettlementOptions = settlementControl.html();
            FillSettlement(municipalityControl.val());
        }, function (sender, args) {
            alert('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
        });

        municipalityControl.change(function () {
            FillSettlement(this.value);
        });
    }
    function FillSettlement(parentId) {
        var filteredSettlementsId = $.map(AllMunicipalities, function (e) {
            var parent = e.get_item('MunicipalityParentMunicipality');
            return parent && parent.get_lookupId() == parentId ? e.get_id().toString() : null;
        });

        var settlementControl = GetSettlementControl();
        settlementControl.html(SettlementOptions);
        settlementControl.find('option').filter(function () {
            return this.value != '0' && $.inArray(this.value, filteredSettlementsId) == -1;
        }).remove();
        if (settlementControl.children().length <= 1)
            settlementControl.attr('disabled', 'disabled');
        else
            settlementControl.removeAttr('disabled');
    }

    function FillNumber(meetingId) {
        var numberControl = getNumberControl();
        numberControl.val('');
        if (!meetingId || meetingId == 0)
            return;

        var query = new SP.CamlQuery();
        query.set_viewXml(String.format("<View><Query><Where><Eq><FieldRef Name='{0}' LookupId='TRUE'/><Value Type='Lookup'>{1}</Value></Eq></Where><OrderBy><FieldRef Name='{2}' Ascending='FALSE'/></OrderBy></Query><RowLimit>1</RowLimit></View>", 'MeetingLink', meetingId, 'AgendaQuestionNumber'));
        var issueMaxNumber = SC.GetItems('AgendaQuestionList', query, 'Include(AgendaQuestionNumber)');
        SC.Execute(function () {
            var issueNumber = issueMaxNumber.get_count() > 0 ? issueMaxNumber.getItemAtIndex(0).get_item('AgendaQuestionNumber') + 1 : 1;
            numberControl.val(issueNumber);
        }, function (sender, args) {
            alert('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
        });
    }

    function OnPostRender(context) {
        var numberControl = getNumberControl();
        var meetingControl = getMeetingControl();

        var meetingId = renderCore.getParentListItemId(['/Lists/MeetingList/EditForm', '/Pages/MeetingAk.aspx']);
        if (context.ControlMode === SPClientTemplates.ClientControlMode.NewForm && meetingId) {
            meetingControl.val(meetingId);
            meetingControl.attr('disabled', 'disabled');
        }

        if (context.ControlMode !== SPClientTemplates.ClientControlMode.DisplayForm) {
            $('[id^="IssueStatusAk"]').attr('disabled', 'disabled');
            SC.OnLoaded(function () {
                var builderListId = SC.GetList('builder').get_id().toString();
                var url = String.format('{0}/{1}/listform.aspx?PageType=8&ListId={2}', _spPageContextInfo.webAbsoluteUrl, _spPageContextInfo.layoutsUrl, builderListId);
                var element = String.format('<div class="col-lg-1"><button type="button" class="btn btn-default" style="margin: 0 0 5px 0" title="Добавить нового застройщика" onclick="window.open(&#039{0}&#039)">+</button></div>', url);
                $('[id^="IssueCustomerAk"]').parent().parent().before(element);
                $('[id^="IssueInvestorAk"]').parent().parent().before(element);

                InitMunicipality();

                if (!numberControl.val())
                    FillNumber(meetingControl.val());
            });
        }

        meetingControl.change(function () {
            FillNumber(this.value);
        });
        numberControl.attr('disabled', 'disabled');

        var prefix = context.FormUniqueId + context.FormContext.listAttributes.Id;
        $get(prefix + 'Author').innerHTML = author;
        $get(prefix + 'Created').innerHTML = created;
        $get(prefix + 'Editor').innerHTML = editor;
        $get(prefix + 'Modified').innerHTML = modified;
    }

    function createLabelMarkup(value, span) {
        return '<label class="col-lg-' + span + '">' + value + '</label>';
    }

    function renderFieldBlock(context, labelSpan, inputSpan, fieldName) {
        var resultHtml = '';
        resultHtml += createLabelMarkup(renderCore.getFieldTitle(fieldName), labelSpan);
        resultHtml += '<div class="col-lg-' + inputSpan + '">';
        resultHtml += renderField(context, fieldName);
        resultHtml += '</div>';
        exceptList.push(fieldName);

        return resultHtml;
    }

    function renderField(context, fieldname) {
        var html = context.RenderFieldByName(context, fieldname);
        var controlMode = context.FieldControlModes[fieldname];

        if (controlMode == SPClientTemplates.ClientControlMode.DisplayForm) {
            return html;
        }

        var container = document.createElement("div");
        container.innerHTML = html;
        $.each(container.getElementsByTagName("textarea"), function () {
            this.className = 'form-control';
        });
        $.each(container.querySelectorAll('input:not([type="checkbox"])'), function () {
            this.className = 'form-control';
        });
        $.each(container.getElementsByTagName("select"), function () {
            this.className = 'form-control';
        });

        return container.innerHTML;
    }

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        init();
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            RegisterModuleInit(SPClientTemplates.Utility.ReplaceUrlTokens("~site/_layouts/15/SAMRT.Web/Scripts/csr/renderAgendaQuestion.js"), init);
        }, 'sp.js');
    }, 'clienttemplates.js');
})();

//Настройка интерфейса
$(function () {
    //Устанавливаем нужный класс для стандартных кнопок
    $('#buttons input[type="button"]').attr('class', 'form-control');
    //Убираем лишний отступ у кнопок MultipleValueLookup
    $('[type="button"][id$="Button"]').css("margin", 0);
    //Увеличиваем ширину MultipleValueLookup
    $('table[id$="MultiLookup_topTable"]').css("width", "100%").find('select').parent().css("width", "50%").children().css("width", "100%");
});
