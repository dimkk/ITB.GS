(function () {
    function init() {
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides({
            OnPostRender: OnPostRender,
        });
    }

    var AllMunicipalities;
    var SettlementOptions;
    function GetMunicipalityControl() {
        return $('[id^="DocumentTpMunicipality"]');
    }
    function GetSettlementControl() {
        return $('[id^="DocumentTpSettlement"]');
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
        if (context.FormContext.itemAttributes.Url.indexOf('/Lists/DocumentTP') >= 0 && context.ListSchema.Field[0].Name == 'DocumentTpStatus') {
            $('[id^="DocumentTpStatus"]').attr('disabled', 'disabled');
        }
    }

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        init();
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            SC.OnLoaded(function () {
                InitMunicipality();
            });
        }, 'sp.js');
    }, 'clienttemplates.js');
})();
