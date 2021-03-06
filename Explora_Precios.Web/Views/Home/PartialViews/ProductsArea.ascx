﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.HomeViewModel>" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="System.ComponentModel"%>
<%@ Import Namespace="MvcContrib.UI.Grid" %>
<%@ Import Namespace="MvcContrib.UI.Grid.ActionSyntax" %>
<%@ Import Namespace="MvcContrib.UI.Pager" %>
<%@ Import Namespace="Explora_Precios.Core" %>
<%@ Import Namespace="Explora_Precios.Core.Helper" %>
<%@ Import Namespace="Explora_Precios.Web" %>
<%@ Import Namespace="Explora_Precios.Web.Controllers" %>
<!-- BEGIN Productos de flotantes -->
<div class="nav_floating_area roundedBorder">
	<div id="grid_floating" class="grid" style="width:100%; padding-top:15px; padding-bottom:10px;"></div>
	<div class="nav_floating nav_floating_bottom hand"><div style="margin-top:2px">Cerrar <img src="../../Content/Images/hide.png" alt="Cerrar" class="nav_floating_hide" /> </div></div>
</div>
<!-- END Productos de flotantes -->

<!-- LISTA DE PRODUCTOS -->
<div id="gridTopic" class="topic">
	<% if (Model.isSearch)
	   { %>
	Resultado de la busqueda: <b><i>
		<%= ViewData["search_text"] %></i></b>
	<%}
	   else
	   { %>
	<% Html.RenderPartial("PartialViews/BreadCrumsMenu", Model.catalog); %>
	<%}//end if %>
	<div class="display">
		<input type="hidden" value="gridview" id="CurrentProductDisplay" />
		<div id="gridTopic_grid" class="gridview" onclick="changeDisplay(this)">
		</div>
		<div id="gridTopic_list" class="listview" onclick="changeDisplay(this)">
		</div>
	</div>
</div>
<div id="grid" class="grid">
	<% Html.RenderPartial("PartialViews/ProductsList", Model.productsListViewModel); %>
	<input type="hidden" id="NextPage" value="1" />
	<% if(!Model.isSearch ) { %>
		<%= Html.Hidden("CatLevel", Model.currentCatalogLevel) %>
		<%= Html.Hidden("CatId", Model.currentCatalogId) %>
	<% } %>
</div>
<script type="text/javascript">
	var lastPaged = 0;
	var calculatingPage = false;
	var showingGoTop = false;
	var goTopShowedAt = 0;

	<% if(Model.productsListViewModel.products.HasNextPage) { %>
	$(document).ready(function () {
		//alert("window " + $(window).height() + " left " + $('div.left').height());
		$(window).scroll(function () {
			var diff = $('#grid').height() - $(window).height();
			//var h = $('#grid').height();
			var y = $(window).scrollTop();
			if (y >= diff && lastPaged < y && !calculatingPage && $('#NextPage').val() != '-1') {
				calculatingPage = true;
				lastPaged = y;
				var page = $('#NextPage').val();
				var url = '';
				var params;
				<% if(Model.isFilter) { %>
					url = '/Home/ScrollFilter';
					var b = getParameterByName('b');
					var p = getParameterByName('p');
					var s = getParameterByName('s');
					params = { catLev: $('#CatLevel').val(), id: $('#CatId').val(), page: page, o: getParameterByName('o'), b: b.length > 0 ? b : undefined, p: p.length > 0 ? p : undefined, s: s.length > 0 ? s : undefined };
				<% } else if (Model.isSearch) { %>
					url = '/Home/ScrollSearch';
					params = { s: getParameterByName('s'), page: page };
				<% } else { %>
					url = '/Home/ScrollProducts';
					params = { catLev: $('#CatLevel').val(), id: $('#CatId').val(), page: page };
				<% } %>
				$('#pager_' + page).show();
				$.get(url, params, function (data) {					
					$('#pager_' + page).hide();
					calculatingPage = false;
					var currentProducts = $('#grid').html();
					$('#grid').append(data.html);
					switchDisplay($('#CurrentProductDisplay').val());
					if(data.hasNext) {
						var nextPage = parseInt(page) + 1;
						$('#NextPage').val(nextPage);
						$('#grid').append('<div class="pager" id="pager_' + nextPage + '" style="display:none;"><img alt="Loading..." src="/content/images/loading_big.gif" class="SmallLoading" style="margin-left:325px;" /></div>');
					}
					else					
						$('#NextPage').val('-1');
				});
			}
			else if(lastPaged - getWindowHeight() > diff) {
					lastPaged = diff - getWindowHeight();
			}
			// Shows the goTop
			if(y > getWindowHeight() && !showingGoTop) {
				$('.menu-root').show();
				goTopShowedAt = y;
				showingGoTop = true;
				$(".bottomDisclaimer").simpletip({
					content: '<img src="../../Content/Images/information3-sc49.png" width="14px" height="14px" style="position:left;"/> Los precios mostrados son obtenidos de las páginas web de cada establecimiento comercial, por lo que sugerimos revisar la página del proveedor para verificar la vigencia del precio ofrecido. <br/><br/>Las ofertas y promociones presentadas en esta página pueden haber variado, sugerimos revisar la página web del comercio provedor del producto.',
					fixed: true,
					position: ["-30", "-120"],
					showEffect: 'slide',
					hideEffect: 'fade'
				});
			}
			// Hides the goTop
			if(y < goTopShowedAt && showingGoTop) {			
				goTopShowedAt = 0;
				$('.menu-root').hide();
				showingGoTop = false;
			}
		});
		$('#grid').append('<div class="pager" id="pager_1" style="display:none;"><img alt="Loading..." src="/content/images/loading_big.gif" class="SmallLoading" style="margin-left:325px;" /></div>');
	});
	<%} else { %>
	
	<% } %>
</script>
