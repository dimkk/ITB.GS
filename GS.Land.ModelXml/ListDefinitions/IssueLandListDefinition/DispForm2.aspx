﻿<%@ Page Language="C#" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint,Version=15.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="GS" Namespace="GSWeb.WebParts.AgendaQuestionTitle" Assembly="GSWeb, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9a3cb80ac0d0c704" %>
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
    <div class="container" id="header">
        <WebPartPages:WebPartZone runat="server" FrameType="None" ID="wzHeader" Title="Header Zone"/>
    </div>
    <div class="container" id="body">
        <ul id="tabs" class="nav nav-tabs">
            <li class="active">
                <a href="#tab-common" data-toggle="tab">Карточка вопроса</a>
            </li>
            <li>
                <a href="#tab-assignment" data-toggle="tab">Поручения</a>
            </li>
            <li>
                <a href="#tab-attach" data-toggle="tab">Материалы к заседанию</a>
            </li>
        </ul>
        <div id="tabContent" class="tab-content">
            <div id="tab-common" class="tab-pane fade active in">
                <WebPartPages:WebPartZone ID="Main" runat="server" FrameType="None" Title="loc:Main" />
            </div>
            <div id="tab-assignment" class="tab-pane fade">
                <WebPartPages:WebPartZone ID="wzAssignments" runat="server" Title="Assignments Zone"/>
            </div>
            <div id="tab-attach" class="tab-pane fade">
                <WebPartPages:WebPartZone ID="wzAttaches" runat="server" Title="Attaches Zone"/>
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
