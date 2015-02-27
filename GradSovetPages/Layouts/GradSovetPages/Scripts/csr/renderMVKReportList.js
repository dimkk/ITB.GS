(function () {
    function init() {
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides({
            OnPostRender: OnPostRender,
        });
    }

    function OnPostRender(ctx) {
		if (!ctx.listUrlDir)
			return;
 		if (!ctx.listUrlDir.endsWith("/Lists/ReportMVKList"))
			return;
		
		var now = new Date().getTime();
		var millisecondsPerDay = 1000 * 60 * 60 * 24;
		
		var rows = ctx.ListData.Row;
		for (var i = 0; i < rows.length; i++) {
			var dateString = rows[i]["_x041f__x043e__x0440__x0443__x04"];
			if (dateString) {
				var date = Date.parseLocale(dateString, "dd.MM.yyyy");
				date.setTime(date.getTime() - millisecondsPerDay * 3);
				if (now >= date.getTime()) {
					var rowElementId = GenerateIIDForListItem(ctx, rows[i]);
					var tr = document.getElementById(rowElementId);
					tr.style.backgroundColor = "#daa";
				}
			}
		}
    }

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        init();
    }, 'clienttemplates.js');
})();
