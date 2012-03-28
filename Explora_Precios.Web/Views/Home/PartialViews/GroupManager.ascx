<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Explora_Precios.Web.Controllers.ViewModels.GroupViewModel>>" %>
    
	<h2>Estos son sus grupos</h2>
	<ul class="groupList">
	<% foreach (var group in Model) { %>
		<li><input type="hidden" value="<%= group.Product %>" /><a href="javascript:void(0)" class="groupItem"><%= group.ProductName %></a></li>
	<% } %>
	</ul>