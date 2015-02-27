/// <reference path="renderCore.js" />
/// <reference path="../SP.debug.js" />
/// <reference path="../SP.Core.debug.js" />
/// <reference path="../SP.runtime.debug.js" />

(function () {

    var author, editor, created, modified;
    var exceptList = [];
    var renderCore;

	var mainBlock, omsuBlock;
	
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
            ListTemplateType: 100,
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
        resultHtml += renderFieldBlock(context, 2, 4, "Dtp_x2116_PP");
        resultHtml += renderFieldBlock(context, 2, 4, "DtpDateOfRegistration");
        resultHtml += '</div>';
		
        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 10, "DtpAddress");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 4, "DtpApplicant");
        resultHtml += renderFieldBlock(context, 2, 4, "DtpKadastryNumber");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 4, "DtpArea");
        resultHtml += renderFieldBlock(context, 2, 4, "DtpDistrict");
        resultHtml += '</div>';
		
        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 10, "DtpNameOfQuestion");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 4, "DtpOmsuDate");
        resultHtml += renderFieldBlock(context, 2, 4, "DtpOmsuResult");
        resultHtml += '</div>';

        resultHtml += '<div class="form-group">';
        resultHtml += renderFieldBlock(context, 2, 10, "DtpOmsuComment");
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
        return $('[id^="DtpArea"]');
    }
    function GetSettlementControl() {
        return $('[id^="DtpDistrict"]');
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
			editable('DtpArea', mainBlock);
			editable('DtpDistrict', mainBlock);
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
		window["WPQ2FormCtx"].PostBackRequired = true;	//Включаем постбэк для возможности прикрепления вложений
        if (context.ControlMode !== SPClientTemplates.ClientControlMode.DisplayForm) {
            SC.OnLoaded(function () {
                InitMunicipality();
                var groups = SC.Context.get_web().get_currentUser().get_groups();
				SC.Context.load(groups);
				SC.Execute(function() {
					console.log('Groups loaded');
					var groupItems = groups.get_data();
					var write = findGroup(groupItems, 'ДТП - Полный доступ');
					var omsu = findGroup(groupItems, 'ДТП - ОМСУ');
					mainBlock = (omsu.length == 0) | (write.length > 0);
					omsuBlock = omsu.length > 0;
					editable('Dtp_x2116_PP', mainBlock);
					editable('DtpDateOfRegistration', mainBlock);
					editable('DtpAddress', mainBlock);
					editable('DtpApplicant', mainBlock);
					editable('DtpKadastryNumber', mainBlock);
					editable('DtpArea', mainBlock);
					editable('DtpDistrict', mainBlock);
					editable('DtpNameOfQuestion', mainBlock);
					editable('DtpOmsuDate', mainBlock);
					editable('DtpOmsuResult', omsuBlock);
					editable('DtpOmsuComment', omsuBlock);
					//if (!mainBlock) {
					//	$('[id^="attachmentsOnClient"]').remove();
					//	$('[id^="idAttachmentsTable"] td:odd').remove();
					//}
				}, function (sender, args) {
					alert('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
				});
            });
        }

        var prefix = context.FormUniqueId + context.FormContext.listAttributes.Id;
        $get(prefix + 'Author').innerHTML = author;
        $get(prefix + 'Created').innerHTML = created;
        $get(prefix + 'Editor').innerHTML = editor;
        $get(prefix + 'Modified').innerHTML = modified;
    }

	function findGroup(groups, name) {
		return $.grep(groups, function(e) { return e.get_title() == name });
	}
	
	function editable(name, editable) {
		if (!editable)
			$('[id^="' + name + '"]').attr('disabled', 'disabled');
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
            RegisterModuleInit(SPClientTemplates.Utility.ReplaceUrlTokens("~site/_layouts/15/SAMRT.Web/Scripts/csr/renderReestrDTP.js"), init);
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
	//Скрываем стандартную кнопку добавления вложений
	$('[id="Ribbon.ListForm.Edit.Actions"]').hide();
});
