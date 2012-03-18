<%@ Page Language="C#" MasterPageFile="~/Views/Shared/SiteMng.Master" AutoEventWireup="true" 
    Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
ExploraPrecios.com - Manejo de productos
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<br /><br /><%= Html.ActionLink("Por Url - Crear o Actualizar Producto", "CreateProduct")%><br />
<%--<%= Html.ActionLink("Actualizar Producto", "UpdateProduct") %>
<br />--%><br />
<%= Html.ActionLink("Por Cliente", "MngProductByClient") %><br />
<%= Html.ActionLink("Por Catalogo", "MngProductByCatalog") %><br />
<%= Html.ActionLink("Por Referencia", "MngProductByReference")%><br />
<%= Html.ActionLink("Buscar Repetidos", "SearchRepeated")%><br />
<%= Html.ActionLink("Posibles productos a ser borrados", "PosibleDelete")%><br />
</asp:Content>

