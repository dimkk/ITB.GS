/// <reference path="renderCore.js" />
/// <reference path="../SP.debug.js" />
/// <reference path="../SP.Core.debug.js" />
/// <reference path="../SP.runtime.debug.js" />
/// <reference path="../clienttemplates.debug.js" />

(function () {

    function init() {
        var hasContext = document.referrer && (~document.referrer.indexOf('Lists/ReportLandList/DispForm') || 
            ~document.referrer.indexOf('Lists/ReportLandList/EditForm'));
        if (hasContext) {
            // регистрируем шаблон только в случае открытия новой формы из контекста формы отчета
            SPClientTemplates.TemplateManager.RegisterTemplateOverrides({
                Templates: {
                    Fields: {
                        'ReportAttachmentReportLand': { 'NewForm': renderAssignmentReportLink },
                        'ReportAttachmentIsAttachLand': {
                            'NewForm': renderAttachmentIsForReport,
                            'EditForm': renderAttachmentIsForReport,
                            'DisplayForm': renderAttachmentIsForReport
                        }
                    },
                    OnPostRender: OnPostRender
                },
                ListTemplateType: 10260
            });
        }
        else {
            SPClientTemplates.TemplateManager.RegisterTemplateOverrides({
                Templates: {
                    Fields: {
                        'ReportAttachmentIsAttachLand': {
                            'NewForm': renderAttachmentIsForReport,
                            'EditForm': renderAttachmentIsForReport,
                            'DisplayForm': renderAttachmentIsForReport
                        }
                    },
                    OnPostRender: OnPostRenderAttach
                },
                ListTemplateType: 10260
            });
        }
    }

    function renderAttachmentIsForReport(ctx) {
        var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);
        formCtx.registerGetValueCallback(formCtx.fieldName, getAttachmentIsForReport);

        return (String).format("<div id='{0}'></div>", formCtx.fieldName);
    }

    function renderAssignmentReportLink(ctx) {
        var formCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);
        formCtx.registerGetValueCallback(formCtx.fieldName, getAssignmentReportLinkId);
        
        return (String).format("<div id='{0}'></div>", formCtx.fieldName);
    }

    function getAttachmentIsForReport() {
        return true;
    }

    function getAssignmentReportLinkId() {
        var params = document.referrer.split('?')[1].split('&');
        for (var i = 0; i < params.length; i++) {
            var param = params[i].split('=');
            if (param[0] !== 'ID') continue;

            return param[1];
        }
        return null;
    }

    function OnPostRender(ctx) {
        var parentId = getAssignmentReportLinkId();
        if (parentId) {
            $('#ReportAttachmentReportLand').html(
                (String).format("Идентификатор отчета {0}", parentId));
        }

        $('#ReportAttachmentReportLand').closest('tr').css('display', 'none');
        OnPostRenderAttach(ctx);
    }

    function OnPostRenderAttach(ctx) {
        $('#ReportAttachmentIsAttachLand').closest('tr').css('display', 'none');
    }
    

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        init();
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            RegisterModuleInit(SPClientTemplates.Utility.ReplaceUrlTokens("~site/_layouts/15/GS.Land.Web/Scripts/csr/renderReportAttachLand.js"), init);
        }, 'sp.js');
    }, 'clienttemplates.js');
})();
