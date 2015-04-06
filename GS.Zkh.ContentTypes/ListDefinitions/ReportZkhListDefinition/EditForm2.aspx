<%@ Page Language="C#" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint,Version=15.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="wssuc" TagName="ToolBar" Src="~/_controltemplates/15/ToolBar.ascx" %>

<asp:Content ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ListFormPageTitle runat="server" />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <span class="die">
        <SharePoint:ListProperty Property="LinkTitle" runat="server" ID="ID_LinkTitle" />
    </span>
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="container" id="body">
        <ul id="tabs" class="nav nav-tabs">
            <li class="active">
                <a href="#tab-common" data-toggle="tab">Карточка отчета</a>
            </li>
            <li>
                <a href="#tab-report" data-toggle="tab">Вложения</a>
            </li>
        </ul>
        <div id="tabContent" class="tab-content">
            <div id="tab-common" class="tab-pane fade active in">
                <WebPartPages:WebPartZone runat="server" FrameType="None" ID="Main" Title="loc:Main"/>
            </div>
            <div id="tab-report" class="tab-pane fade">
                <WebPartPages:WebPartZone ID="wzReports" runat="server" Title="Reports Zone"/>
            </div>
        </div>
    </div>
    <div class="container" id="buttons">
        <wssuc:ToolBar CssClass="ms-formtoolbar" id="toolBarTbl" RightButtonSeparator="&amp;#160;" runat="server">
            <template_buttons>
			<SharePoint:CreatedModifiedInfo ID="CreatedModifiedInfo" runat="server"/>
		</template_buttons>
            <template_rightbuttons>
			<SharePoint:SaveButton ID="SaveButton" runat="server"/>
			<SharePoint:GoBackButton ID="GoBackButton" runat="server"/>
		</template_rightbuttons>
        </wssuc:ToolBar>
    </div>
</asp:Content>