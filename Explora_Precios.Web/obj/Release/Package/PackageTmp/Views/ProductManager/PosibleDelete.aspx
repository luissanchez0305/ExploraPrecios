<%@ Page Language="C#" MasterPageFile="~/Views/Shared/SiteMng.Master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage<List<Explora_Precios.Web.Controllers.ViewModels.ProductClientViewModel>>" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
Exploraprecios.com - Manejo de productos - Productos posibles a ser borrados
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<h2><a href="javascript:history.go(-1);">Ir a menu de manejo de productos</a></h2>
    <div class="Content">
        <div id="productsContainer">
        <table>
            <tr><th>Nombre de producto</th><th>Cliente</th></tr>
            <% foreach (var productClient in Model)
                { %>
                <tr><td><a href="<%= productClient.client.url %>"> <%= productClient.product.productName %></a></td>
                <td><%= productClient.client.clientName %></td>
                <td><a href="/ProductManager/PosibleDelete?productId=<%= productClient.product.productId %>&clientId=<%= productClient.client.clientId %>">Borrar</a></td></tr>
            <% } %>
        </table>
        </div>
    </div>
</asp:Content>
