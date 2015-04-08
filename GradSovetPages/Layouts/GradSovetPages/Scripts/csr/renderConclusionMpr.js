(function () {
    function init() {
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides({
            OnPostRender: OnPostRender,
			ListTemplateType: 101
        });
    }

    function OnPostRender(ctx) {
		if (ctx.ListSchema.Field[0].Name != 'ConclusionApplicationMpr')
			return;
		
		var parentId = renderCore.getParentListItemId(['/Lists/ReestrDTP/']);
		var c = $('[id^="ConclusionApplicationMpr"]');
		if (parentId && c.val() == '0') {
			c.val(parentId).parent().parent().parent().hide();
		}
    }

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        init();
    }, 'clienttemplates.js');
})();
