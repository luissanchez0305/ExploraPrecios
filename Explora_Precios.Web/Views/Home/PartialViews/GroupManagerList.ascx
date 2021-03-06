﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Explora_Precios.Web.Controllers.ViewModels.GroupViewModel>>" %>
<%@ Import Namespace="Explora_Precios.ApplicationServices" %>

	<ul class="groupList">
		<li>
			<label>Producto</label>
			<label style="float:right;">Creado</label>
		</li>
	<% foreach (var group in Model) { %>
		<li>
			<input type="hidden" value="<%= group.ProductId %>" /> 
			<a href="javascript:void(0)" class="groupItem"><%= group.ProductName.Shorten(38) %></a> 
			<label style="float:right;"><%= group.CreatedDate.ToShortDateString() %></label>
		</li>
	<% } %>
	</ul>
