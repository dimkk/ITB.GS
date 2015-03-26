<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IssueAttachmentZkh.aspx.cs" Inherits="GS.Zkh.Web.IssueAttachmentZkh" DynamicMasterPageFile="~masterurl/default.master" %>
<%@ Assembly Name="Microsoft.Office.DocumentManagement, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="MVK" Namespace="GS.MVK.Web" Assembly="GS.MVK.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=e6d7b98455b283b9"%>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <style type="text/css">
        .ms-menutoolbar { display: none }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <WebPartPages:webpartzone runat="server" id="MainWPZone" title="Зона веб частей" webpart="true">
        <ZoneTemplate>
			<MVK:DocumentSet runat="server" DocumentLibraryUrl="IssueAttachmentZkhList" 
                QuestionLinkFieldName="IssueAttachmentIssueZkh" Title="Набор документов" 
                FrameType="None" SuppressWebPartChrome="False" 
                Description="Веб-часть для работы с наборами документов" IsIncluded="True" ZoneID="FullPage" 
                PartOrder="2" FrameState="Normal" AllowRemove="True" AllowZoneChange="True" AllowMinimize="True" 
                AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True" 
                CatalogIconImageUrl="/_layouts/15/images/SAMRT/WebPartIcon_DocumentSet.gif" 
                TitleIconImageUrl="/_layouts/15/images/SAMRT/WebPartIcon_DocumentSet.gif" 
                DetailLink="" HelpLink="" HelpMode="Modeless" Dir="Default" 
                PartImageSmall="/_layouts/15/images/SAMRT/WebPartIcon_DocumentSet.gif" 
                MissingAssembly="Cannot import DocumentSet Title Web Part." 
                ImportErrorMessage="Cannot import DocumentSet Title Web Part." 
                PartImageLarge="/_layouts/15/images/SAMRT/WebPartIcon_DocumentSet.gif" 
                IsIncludedFilter="" ExportControlledProperties="True" 
                ConnectionID="00000000-0000-0000-0000-000000000000" ChromeType="None"
                ExportMode="All" __MarkupType="vsattributemarkup" WebPart="true" Height="" 
                Width="" ID="g_6a9aec59_e2c0_4dda_9f20_7db5f74f81ac"></MVK:DocumentSet>
        </ZoneTemplate>
    </WebPartPages:webpartzone>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Вложения вопроса повестки ЖКХ.Земля
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
Вложения вопроса повестки ЖКХ.Земля
</asp:Content>
