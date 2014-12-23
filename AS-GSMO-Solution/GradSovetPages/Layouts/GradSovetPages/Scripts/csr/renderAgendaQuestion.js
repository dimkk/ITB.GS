/// <reference path="renderCore.js" />
/// <reference path="../SP.debug.js" />
/// <reference path="../SP.Core.debug.js" />
/// <reference path="../SP.runtime.debug.js" />

(function () {

    var author, editor, created, modified;
    var exceptList = [];
    var renderCore;

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

        var resultHtml = '';
        resultHtml += renderItemHeader(context);
        resultHtml += '<div class="form-horizontal" role="form">';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Описание', 2, 10, "AgendaQuestionDescription");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Инвестор', 2, 10, "AgendaQuestionInvestor");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Кадастровый номер', 2, 10, "CadastreNumber");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Категория вопроса', 2, 4, "QuestionCategoryLink");
		resultHtml += renderFieldBlock(context, 'Тип решения', 2, 4, "AgendaQuestionDecisionType");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Докладчик', 2, 4, "AgendaQuestionReporterFullNameLink");
        resultHtml += renderFieldBlock(context, 'Содокладчики', 2, 4, "AgendaQuestionSoreporterFullNameLink");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Муниципальный район/Городской округ', 2, 4, "IssueMunicipalityGs");
        resultHtml += renderFieldBlock(context, 'Поселение', 2, 4, "IssueSettlementGs");
        resultHtml += '</div>';
		
        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Решение', 2, 10, "AgendaQuestionProtocolDecision");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Адрес', 2, 10, "AgendaQuestionAddress");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlockByDisplayName(context, 'Тип объекта', 2, 4, "Тип объекта");
        resultHtml += renderFieldBlock(context, 'Наименование объекта', 2, 4, "AgendaQuestionSiteName");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Тема', 2, 10, "AgendaQuestionTheme");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Номер', 2, 4, "AgendaQuestionNumber");
        resultHtml += renderFieldBlock(context, 'Дата поступления', 2, 4, "AgendaQuestionIncomingDate");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
		resultHtml += renderFieldBlock(context, 'Рассмотрен', 2, 4, "AgendaQuestionIsConsidered");
        resultHtml += renderFieldBlock(context, 'Основание', 2, 4, "AgendaQuestionReason");
        resultHtml += '</div>';
		
        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Заявитель на комиссию', 2, 4, "AgendaQuestionDeclarant");
		resultHtml += renderFieldBlock(context, 'Тип проекта', 2, 4, "AgendaQuestionProjectType");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Заседание', 2, 4, "MeetingLink");
        resultHtml += renderFieldBlock(context, 'Внешние источники', 2, 4, "AgendaQuestionExtResources");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 'Связанный вопрос', 2, 10, "AgendaLinkedQuestionLink");
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
        return $('[id^="IssueMunicipalityGs"]');
    }
    function GetSettlementControl() {
        return $('[id^="IssueSettlementGs"]');
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

    function OnPostRender(context) {
        if (context.ControlMode !== SPClientTemplates.ClientControlMode.DisplayForm) {
            SC.OnLoaded(function () {
                InitMunicipality();
            });
			
			var hasContext = document.referrer &&
	            (~document.referrer.indexOf('MeetingList/DispForm') ||
	            ~document.referrer.indexOf('MeetingList/EditForm'));
	        if (!hasContext) return;
	        
	        var parentId = null;
	        var params = document.referrer.split('?')[1].split('&');
	        for (var i = 0; i < params.length; i++) {
	            var param = params[i].split('=');
	            if (param[0] !== 'ID') continue;
	
	            parentId = param[1];
	            break;
	        }
	        
	        var selects = $("select[id^='MeetingLink']");
	        if (selects[0]) {
	        	$(selects[0]).val(parentId);
	        }
        }

        var prefix = context.FormUniqueId + context.FormContext.listAttributes.Id;
        $get(prefix + 'Author').innerHTML   = author;
        $get(prefix + 'Created').innerHTML  = created;
        $get(prefix + 'Editor').innerHTML   = editor;
        $get(prefix + 'Modified').innerHTML = modified;
    }

    function createLabelMarkup(value, span) {
        return '<label class="col-lg-' + span + '">' + value + '</label>';
    }

    function renderFieldBlock(context, label, labelSpan, inputSpan, fieldName) {
        var resultHtml = '';
        resultHtml += createLabelMarkup(label, labelSpan);
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

    function renderFieldBlockByDisplayName(context, label, labelSpan, inputSpan, displayName) {
        var fieldName = renderCore.getInternalFieldName(displayName);
        if (!fieldName)
            return "";

        return renderFieldBlock(context, label, labelSpan, inputSpan, fieldName);
    }

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        init();
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            RegisterModuleInit(SPClientTemplates.Utility.ReplaceUrlTokens("~site/_layouts/15/gradsovetpages/Scripts/csr/renderAgendaQuestion.js"), init);
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
