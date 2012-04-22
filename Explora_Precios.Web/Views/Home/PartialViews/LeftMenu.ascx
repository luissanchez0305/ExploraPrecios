<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.HomeViewModel>" %>
<script type="text/javascript">
var offerUrl = GetOfferUrl();
$(document).ready(function(){
	$(".department").val(<%= Model.departmentId %>);
	$(".filter.offer").attr('href', offerUrl);
	$("#sliderBar").slider({
		min: <%= Model.Filter.CurrentMinPrice %>,
		max: <%= Model.Filter.CurrentMaxPrice %>,
		step: 1,
		values: [<%= Model.Filter.CurrentMinPrice %>, <%= Model.Filter.CurrentMaxPrice %>],
		slide: function(event, ui) {
			for (var i = 0; i < ui.values.length; ++i) {
				$("input.sliderValue[dataindex=" + i + "]").val(ui.values[i]);
				$("label.sliderValue[dataindex=" + i + "]").html('$' + ui.values[i]);
				var button = $('.filterButton');
				if(!button.hasClass('enable')){
					button.addClass('enable');
					button.addClass('hand');
					button.click(function() {
						var p = getParameterByName('p');
						var b = getParameterByName('b');
						var min = $("input.sliderValue[dataindex=0]").val();
						var max = $("input.sliderValue[dataindex=1]").val();
						var url = '/Home/Filter?o=false&p=' + min + "," + max + '&cl=<%=Model.catalog.currentCatalogLevel %>&ci=<%=Model.catalog.currentCatalogId%>';
						if(b.length > 0)
							url += '&b=' + b;
						document.location = url;
					});
				}
			}
		}
	});
});
$("input.sliderValue").change(function() {
	var $this = $(this);
	$("#sliderBar").slider("values", $this.data("index"), $this.val());
	alert($this.data("index"));
});
function GetOfferUrl() {
	var response = '/Home/Filter?o=true&cl=<%=Model.catalog.currentCatalogLevel %>&ci=<%=Model.catalog.currentCatalogId%>';
	var p = getParameterByName('p');
	var b = getParameterByName('b');
	if(b.length > 0)
		response += '&b=' + b;
	if (p.length > 0)
		response += '&p=' + p;
	return response;
}
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

	<% if (Model.Filter.HasFilterPrices || Model.Filter.UndoPriceFilter != null || Model.Filter.UndoSaleFilter != null)
	 { %>
	<div class="separator">
		<h3>Precios</h3>
		<%= Html.Hidden("sliderValueMin", Model.Filter.CurrentMinPrice, new { @class = "sliderValue", dataindex = "0" }) %>
		<%= Html.Hidden("sliderValueMax", Model.Filter.CurrentMaxPrice, new { @class = "sliderValue", dataindex = "1" })%>

		<div class="suckerdiv">
			<ul class="suckertree1">
					<li><a class="filter offer" href="">Ofertas</a></li>
			</ul>
		</div>

		<div id="sliderBar" style="width:200px; margin: 15px;"></div>
		<div id="sliderValues" style="width:214px; margin: 10px; margin-bottom: 20px;">
			<label id="LabelMinPrice" class="sliderValue" style="float:left; font-weight:bold;" dataindex="0">$<%= Model.Filter.CurrentMinPrice %></label>&nbsp;-&nbsp;<label id="LabelMaxPrice" style="font-weight:bold;" class="sliderValue" dataindex="1">$<%= Model.Filter.CurrentMaxPrice %></label>
			<div class="filterButton"></div>
		</div>
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
	<% } %>
	</div>
	<% if (Model.Filter.FilterBrands.Count() > 0 || Model.Filter.UndoBrandFilter != null)
	{ %>
	<div class="separator">
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
	</div>
	<% } %>
<script type="text/javascript">
	$(".undofilter").click(function () {
		document.location = $(this).prev().val();
	});
</script>

