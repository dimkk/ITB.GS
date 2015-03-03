(function () {
    function init() {
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides({
            OnPostRender: OnPostRender,
        });
    }

    function OnPostRender(ctx) {
		if (!ctx.listUrlDir.endsWith("/Lists/tasks"))
			return;
		
		var rows = ctx.ListData.Row;
		for (var i = 0; i < rows.length; i++)
		{
			if (rows[i]["result"] === "срок истек")
			{
				var rowElementId = GenerateIIDForListItem(ctx, rows[i]);
				var tr = document.getElementById(rowElementId);
				tr.style.backgroundColor = "#daa";
			}
		}
    }

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        init();
    }, 'clienttemplates.js');
})();
