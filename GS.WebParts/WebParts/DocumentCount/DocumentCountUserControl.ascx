<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentCountUserControl.ascx.cs" Inherits="GS.WebParts.DocumentCountUserControl, GS.WebParts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=76fad1f12ae5d8a7" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<style type="text/css">
.table th {
	color: #fff;
    background-color: #428bca;
	text-align: center;
}
.table td {
	text-align: center;
}
</style>

<div id="DocumentCountWebPartID">
    <div>
        <div class="form-group">
            <label class="col-lg-1">Дата с:</label>
            <div class="col-lg-2">
                <SharePoint:DateTimeControl ID="dtFrom" DateOnly="True" runat="server" />
            </div>
            <label class="col-lg-1">Дата до:</label>
            <div class="col-lg-2">
                <SharePoint:DateTimeControl ID="dtTo" DateOnly="True" runat="server" />
            </div>
			<div class="col-lg-1">
				<button type="button" class="btn btn-default" onclick="ShowClick()">Показать</button>
			</div>
        </div>
    </div>
	<br/><br/><br/>
    <div id="PanelContent">
        <table class="table table-bordered table-striped table-hover">
            <thead>
                <tr>
                    <th>Наименование услуги</th>
                    <th>Всего</th>
                    <th>В работе всего</th>
                    <th>Количество выдано</th>
                    <th>Нарушен срок</th>
                    <th>Количество под жилые объекты</th>
                    <th>Количество под нежилые объекты</th>
                </tr>
            </thead>
            <tbody data-bind="foreach: Documents">
				<!-- ko if: !IsSubElement -->
				<tr>
					<td colspan="7"></td>
				</tr>
				<!-- /ko -->
                <tr>
                    <td data-bind="text: Name, style: { textAlign: IsSubElement ? 'right' : 'left', fontWeight: IsSubElement ? '' : 'bold' }"></td>
                    <td>
                        <a data-bind="text: AllCount, attr: { href: AllLink, target: '_blank' }"></a>
                    </td>
					<td>
                        <a data-bind="text: WorkCount, attr: { href: WorkLink, target: '_blank' }"></a>
					</td>
					<td>
                        <a data-bind="text: IssuedCount, attr: { href: IssuedLink, target: '_blank' }"></a>
					</td>
					<td>
                        <a data-bind="text: ExpiredCount, attr: { href: ExpiredLink, target: '_blank' }"></a>
					</td>
                    <td>
                        <a data-bind="text: InhabitedCount, attr: { href: InhabitedLink, target: '_blank' }"></a>
                    </td>
                    <td>
                        <a data-bind="text: UnInhabitedCount, attr: { href: UninhabitedLink, target: '_blank' }"></a>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</div>

<SharePoint:ScriptLink ID="sl1" runat="server" Name="GradSovetPages/Scripts/knockout-2.3.0.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
<SharePoint:ScriptLink ID="sl2" runat="server" Name="GradSovetPages/Scripts/camljs.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
<SharePoint:ScriptLink ID="sl3" runat="server" Name="GradSovetPages/Scripts/spin.min.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
<SharePoint:ScriptLink ID="sl4" runat="server" Name="GradSovetPages/Scripts/gsUtils.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>
<SharePoint:ScriptLink ID="sl5" runat="server" Name="GS.WebParts/DocumentCount/DocumentCountModel.js" Localizable="False" OnDemand="False"></SharePoint:ScriptLink>

<script type="text/javascript">
	function parseDate(date) {
		if (!date) return;
		var parts = date.split(".");
		return (new Date(+parts[2], parts[1] - 1, +parts[0])).format('yyyy-MM-dd');
	}
	function ShowClick() {
		Model.loadData({ DateFrom: parseDate($('[id$="dtFromDate"]').val()), DateTo: parseDate($('[id$="dtToDate"]').val()) });
	}
	var Model;
    $(document).ready(function () {
        SP.SOD.executeFunc("sp.js", "SP.ClientContext", function () {
            SP.SOD.executeOrDelayUntilScriptLoaded(function () {
				Model = new DocumentCount.Model();
                ko.applyBindings(Model.loadData(), document.getElementById("DocumentCountWebPartID"));
            }, "sp.js");
        });
    });
</script>
