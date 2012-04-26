<%@ Page Language="C#" MasterPageFile="~/Views/Shared/SiteMng.Master" AutoEventWireup="true"
	Inherits="System.Web.Mvc.ViewPage<IList<Explora_Precios.Web.Controllers.ViewModels.ClientViewModel>>" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
Exploraprecios.com - Manejo de productos y catalogos - Manejo de clientes
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<h2><a href="/Manager">Manejo de productos y catalogos</a> - Manejo de clientes</h2>
<% var choosen = Model.SingleOrDefault(c => c.choosen);
   var choosenShown = false; %>
<table cellpadding="10" width="50%">
	<tr>
		<td><%= Html.ActionLink("NUEVO", "MngClients?id=0", "Manager") %></td>
	</tr>
	<% foreach (var clientModel in Model)
	{
		if (!string.IsNullOrEmpty(clientModel.clientName))
		{ %>
	<tr>
		<td>
			<% if (clientModel.choosen)
	  { %><b><% } %>
			<%= Html.ActionLink(clientModel.clientName, "MngClients?id=" + clientModel.clientId, "Manager")%>
			<% if (clientModel.choosen)
	  { %></b><% } %>
		</td>
		<% if (choosen != null && !choosenShown)
	 { %>
		<td rowspan="<%= Model.Count %>" style="vertical-align:top;">
				<% Html.BeginForm("SaveClient", "Manager", FormMethod.Post); %>
				<table width="300px">
					<tr>
						<td>Nombre</td>
						<td>
							<%= Html.Hidden("clientId", choosen.clientId)%>
							<%= Html.TextBox("clientName", choosen.clientName)%>
						</td>
					</tr>
					<tr>
						<td>URL</td>
						<td><%= Html.TextBox("url", choosen.url)%></td>
					</tr>
					<tr>
						<td>Facebook Id</td>
						<td><%= Html.TextBox("facebookId", choosen.facebookId)%></td>
					</tr>
					<tr>
						<td>Facebook Publish</td>
						<td><%= Html.CheckBox("facebookPublish", choosen.facebookPublish)%></td>
					</tr>
					<tr>
						<td>Activo</td>
						<td><%= Html.CheckBox("isActive", choosen.isActive)%></td>
					</tr>
					<tr>
						<td>Direccion de catalogo</td>
						<td><%= Html.TextBox("catalogAddress", choosen.catalogAddress)%></td>
					</tr>
					<tr>
						<td>&nbsp;</td>
						<td><input type="submit" value="Guardar" /></td>
					</tr>
				</table>
				<% Html.EndForm(); %>
		</td>
		<%
		choosenShown = true;
	 } %>
	</tr>
	<% }
	} %>
</table>
</asp:Content>
