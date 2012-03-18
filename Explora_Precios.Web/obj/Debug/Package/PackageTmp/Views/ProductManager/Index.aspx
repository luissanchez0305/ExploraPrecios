<%@ Page Language="C#" MasterPageFile="~/Views/Shared/SiteMng.Master" AutoEventWireup="true" 
    Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
ExploraPrecios.com - Manejo de productos
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<br /><br />
<%--<%= Html.ActionLink("Actualizar Producto", "UpdateProduct") %>
<br />--%><br />
<table cellpadding="10">
<tr>
    <th>Modificacion de Catalogo</th>
    <th>Modificacion de productos</th>
</tr>
<tr style="vertical-align:top">
<td>
    <%= Html.ActionLink("Catalogo General", "MngCatalog") %><br />
    <%= Html.ActionLink("Catalogo de Clientes", "MngCatalogClient") %><br />
</td>
<td>
    <%= Html.ActionLink("Productos online", "MngProductsByClient") %> - Utiliza el catalogo de los clientes para obtener sus productos<br />
    <%= Html.ActionLink("Productos locales", "MngProductsLocal")%> - Utiliza nuestro catalogo para obtener los productos<br /> 
    <%= Html.ActionLink("Productos individuales", "MngProductByReference")%><br /><br />

    <%= Html.ActionLink("Productos reportados", "MngProductReported")%><br />
    <%= Html.ActionLink("Buscar repetidos", "RepeatedProducts")%><br />
    <% var showAutomaticFields = System.Configuration.ConfigurationManager.AppSettings["showAutomaticFields"]; %>
    <div id="AutomaticUpdate" <%= (!string.IsNullOrEmpty(showAutomaticFields) && Boolean.Parse(showAutomaticFields)) ? "" : "style=\"display:none;\"" %>><input type="button" id="AutomaticUpdateBtn" value="Automatic Update" /><img id="image_Loading" src="/content/images/loading_big.gif" alt="Loading" /><label id="updateTitle"></label></div>
</td>
</tr>
</table>
<script type="text/javascript">
    $(document).ready(function () {
        $("#image_Loading").hide();
    });
    $("#AutomaticUpdateBtn").click(function () {
        $("#image_Loading").show();
        $.ajax({
            url: '<%= Url.Action("LoadAutomaticUpdate", "ProductManager") %>',
            dataType: 'json',
            error: function () {
                $("#image_Loading").hide();
                $("#updateTitle").html('ERROR');
            },
            success: function (data) {
                $("#image_Loading").hide();
                if (data.result == 'success') {
                    $("#updateTitle").html('LISTO');
                }
                else if (data.result == 'fail') {
                    $("#updateTitle").html('FALLO: ' + data.msg);
                }
            }
        });
    });
</script>
</asp:Content>

