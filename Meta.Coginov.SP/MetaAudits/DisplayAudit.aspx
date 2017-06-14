<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DisplayAudit.aspx.cs" Inherits="Meta.Coginov.SP.Pages.DisplayAudit" MasterPageFile="~masterurl/default.master" EnableViewState="false" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <style>
        @import url("../../Style Library/datatables.css");
        @import url("../../Style Library/dashboard.css");
    </style>

    <div id="auditDataContainer" runat="server"></div>
    <div class="flow" id="auditContainer">
        <div id="fileCount" class="flow-3"><span class="title">Nomdre de fichiers</span><span class="body">99,999</span></div>
        <div id="fileSize" class="flow-3"><span class="title">Espace disque</span><span class="body">9.99G</span></div>
        <div id="sites" class="flow-3"><span class="title">Sites</span><span class="body">1</span></div>
        <div id="filesByExt"><span class="title">Fichiers par extension</span><div class="body graph"></div></div>
        <div id="fileAgeing"><span class="title">Âge des fichiers</span><div class="body graph"></div></div>
        <div id="duplicates">
            <span class="title">Doublons</span>
            <div class="body graph"></div>
            <table id="tableDuplicate" class="display dataTable">
                <thead>
                    <tr>
                        <th>Nom du fichier</th>
                        <th>Répertoire</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div id="semanticCloud">
            <span class="title">Nuage sémantique</span>
            <div class="body graph"></div>
        </div>
    </div>

    <script src="../../Style Library/all.min.js"></script>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Audit dashboard
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Audit dashboard for <span runat="server" id="auditid"></span>
</asp:Content>
