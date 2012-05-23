<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.MainMenuModel>" %>
	<% 	if (Model.DepList != null)
		{ %>
		<ul class="main-menu roundedBorder">
			<% foreach (var dep in Model.DepList)
			{ %>
				<li class="item">
					<a id="item_<%= dep.name %>" href="/Home/Products/?catlev=0&id=<%= Html.Encode(dep.Id) %>"><%= dep.name%></a>
					<% if ((Model.DepId == null || !(Model.DepId != null && Model.DepId == dep.Id)) && dep.categories.Count > 1) { %>
					<ul>
						<%foreach (var cat in dep.categories) { %>
						<li class="child">
							<a href="/Home/Products/?catlev=1&id=<%= Html.Encode(cat.Id) %>"><%= cat.name%></a>
							<% if (cat.subCategories.Count > 0) { %>
							<ul>
								<% foreach (var subCat in cat.subCategories) { %>
								<li class="child">
									<a href="/Home/Products/?catlev=2&id=<%= Html.Encode(subCat.Id) %>"><%= subCat.name%></a>
									<%if (subCat.productTypes.Count > 0) { %>
									<ul>
										<%foreach (var prodType in subCat.productTypes) { %>
											<li class="child">
												<a href="/Home/Products/?catlev=3&id=<%= Html.Encode(prodType.Id) %>"><%= prodType.name%></a>
											</li>
										<% } %>
									</ul>
									<% } %>
								</li>
								<% } %>
							</ul>
							<% } %>
						</li>
						<% } %>
					</ul>
					<% } %>
				</li>
		<% } %>
				
		<% if (Model.DisplayNewProducts)
			{ %>
				<li class="tabCounter"><a id="item_OfferNew" href="javascript:void(0);">Nuevos</a></li>
			<% } %>
		</ul>
		<div class="bar"></div>
	<% } %>

<script type="text/javascript">
	$("ul.main-menu").superfish();
</script>