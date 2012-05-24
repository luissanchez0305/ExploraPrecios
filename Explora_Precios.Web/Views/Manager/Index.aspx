<%@ Page Language="C#" MasterPageFile="~/Views/Shared/SiteMng.Master" AutoEventWireup="true" 
	Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
ExploraPrecios.com - Manejo de productos
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<br /><br />
<br /><br />
<table cellpadding="10">
	<tr><th>Modificacion de Clientes</th></tr>
	<tr><td><%= Html.ActionLink("Clientes", "MngClients") %></td></tr>
</table>
<table cellpadding="10">
<tr>
	<th>Modificacion de Catalogo</th>
	<th>Modificacion de Productos</th>
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
	<div><input type="button" id="UpdateCounters" value="Actualiza contadores" /><label id="updateCounterTitle"></label></div>
</td>
</tr>
</table>
<script type="text/javascript">
	$(document).ready(function () {
		$("#image_Loading").hide();
	});

	$("#UpdateCounters").click(function () {
		$("#image_Loading").show();
		$.ajax({
			url: '<%= Url.Action("UpdateCounters", "Manager") %>',
			dataType: 'json',
			error: function () {
				$("#image_Loading").hide();
				$("#updateCounterTitle").html('ERROR');
			},
			success: function (data) {
				$("#image_Loading").hide();
				if (data.result == 'success') {
					$("#updateCounterTitle").html('LISTO' + ' ' + getCurrentTime());
				}
				else if (data.result == 'fail') {
					$("#updateCounterTitle").html('FALLO: ' + data.msg + ' ' + getCurrentTime());
				}
			}
		});
	});
	$("#AutomaticUpdateBtn").click(function () {
		$("#image_Loading").show();
		$.ajax({
			url: '<%= Url.Action("LoadAutomaticUpdate", "Manager") %>',
			dataType: 'json',
			error: function () {
				$("#image_Loading").hide();
				$("#updateTitle").html('ERROR');
			},
			success: function (data) {
				$("#image_Loading").hide();
				if (data.result == 'success') {
					$("#updateTitle").html('LISTO' + ' ' + getCurrentTime());
				}
				else if (data.result == 'fail') {
					$("#updateTitle").html('FALLO: ' + data.msg + ' ' + getCurrentTime());
				}
			}
		});
	});

	function getCurrentTime() {
		var date = new Date();
		return date.getMonth() + '/' + date.getDate() + '/' + date.getYear() + ' ' + date.getHours() + ':' + date.getMinutes() + ':' + date.getSeconds();
	}
</script>
</asp:Content>

