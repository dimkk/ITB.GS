(function () {
    function init() {
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides({
            OnPostRender: OnPostRender,
        });
    }

    function OnPostRender(ctx) {
		if (!ctx.listUrlDir)
			return;
		if (!ctx.listUrlDir.endsWith("/Lists/AssignmentMVKList"))
			return;
		
		var rows = ctx.ListData.Row;
		for (var i = 0; i < rows.length; i++)
		{
			var status = rows[i]["AssignmentStatusMVK"];
			var color = null;
			
			if (status === "Срок истек")
				color = "#daa";
			else if (status === "Исполнено")
				color = "#ada";
			
			if (color) {
				var rowElementId = GenerateIIDForListItem(ctx, rows[i]);
				var tr = document.getElementById(rowElementId);
				tr.style.backgroundColor = color;
			}
		}
    }

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        init();
    }, 'clienttemplates.js');
})();
