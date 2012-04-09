<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.HomeViewModel>" %>
<script type="text/javascript">
$(document).ready(function(){
	$(".department").val(<%= Model.departmentId %>);
});
</script>
	<% if (Model.categories.Count > 0) { %>
	<h3>Catalogo de <%= Model.departmentTitle %></h3>
		<%= Html.Hidden("departmentId", Model.departmentId, new { @class = "department" })%>
	<div class="suckerdiv">
		<ul id="suckertree1">
			<% foreach (var category in Model.categories)
	   { %>
			<li><a href="/Home/Products?catlev=1&id=<%= category.categoryId %>">
				<%= category.categoryTitle %></a>
				<% if (category.subCategories.Count > 0)
				   { %>
						<ul>
						 <% foreach (var subcategory in category.subCategories)
							{ %>
								<li><a href="/Home/Products?catlev=2&id=<%= subcategory.subCategoryId %>"><%= subcategory.subCategoryTitle %></a>
								<% if (subcategory.productTypes.Count > 0)
								   { %>
										<ul>
										<% foreach (var producttype in subcategory.productTypes)
										   { %>
												<li><a href="/Home/Products?catlev=3&id=<%= producttype.productTypeId %>"><%= producttype.productTypeTitle %></a></li>
										<% } %>
										</ul>
								<%} %></li>
						 <% } %>
						</ul>
				<% 
				   }  %></li>
			<% 
	   } %>
		</ul>
	</div>
	<% } %>

<div class="separator">
	<% if (Model.Filter.FilterPrices.Count() > 0 || Model.Filter.UndoPriceFilter != null || Model.Filter.UndoSaleFilter != null)
	 { %>
		<h3>Precios</h3>
		<% if (Model.Filter.UndoPriceFilter != null)
		 {%>
			<div class="suckerdiv filter">
				Filtrado: <label><%= Model.Filter.UndoPriceFilter.Name %></label> <input type="hidden" value="<%= Model.Filter.UndoPriceFilter.Url %>" /><div class="hand undofilter" style="background: url(/content/images/ui-icons_cd0a0a_256x240.png) no-repeat -98px -130px; width:11px; height:11px; float:right;"></div>
			</div>
		 <%} %>
		<% if (Model.Filter.UndoSaleFilter != null)
		 {%>
			<div class="suckerdiv filter">
				Filtrado: <label><%= Model.Filter.UndoSaleFilter.Name %></label> <input type="hidden" value="<%= Model.Filter.UndoSaleFilter.Url %>" /><div class="hand undofilter" style="background: url(/content/images/ui-icons_cd0a0a_256x240.png) no-repeat -98px -130px; width:11px; height:11px; float:right;"></div>
			</div>
		 <%} %>
		<div class="suckerdiv">
		<% foreach (var filter in Model.Filter.FilterPrices)
		{%>
			<ul class="suckertree1">
					<li><a <% if(filter.Url.Contains("o=true")) { %>style="background-color:#F21400; color:#F4EFCF;"<% } %> class="filter" href="<%= filter.Url %>"><%= filter.Name%></a></li>
			</ul>
		<% } %>
		</div>
	<% } %>
	<% if (Model.Filter.FilterBrands.Count() > 0 || Model.Filter.UndoBrandFilter != null)
	{ %>
		<h3>Marcas</h3>
		<% if (Model.Filter.UndoBrandFilter != null)
		 {%>
			<div class="suckerdiv filter">
				Filtrado: <label><%= Model.Filter.UndoBrandFilter.Name%></label> <input type="hidden" value="<%= Model.Filter.UndoBrandFilter.Url %>" /><div class="hand undofilter" style="background: url(/content/images/ui-icons_cd0a0a_256x240.png) no-repeat -98px -130px; width:11px; height:11px; float:right;"></div>
			</div>
		 <%} %>
		<div class="suckerdiv">
		<% foreach (var filter in Model.Filter.FilterBrands)
		{%>
			<ul class="suckertree1">
					<li><a class="filter" href="<%= filter.Url %>"><%= filter.Name%></a></li>
			</ul>
		<% } %>
		</div>
	<% } %>
</div>
<script type="text/javascript">
	$(".undofilter").click(function () {
		document.location = $(this).prev().val();
	});
</script>

