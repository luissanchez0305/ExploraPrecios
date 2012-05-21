<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Explora_Precios.Web.Controllers.ViewModels.IntroViewModel>" %>
<%@ Import Namespace="Explora_Precios.Web.Controllers.Helpers" %>
<%@ Import Namespace="Explora_Precios.ApplicationServices" %>
<asp:Content ID="HeaderTitleContent" ContentPlaceHolderID="TitleContent" runat="server">
ExploraPrecios.com
</asp:Content>
<asp:Content ID="LeftMenuContent" ContentPlaceHolderID="LeftContent" runat="server">
<% Html.RenderPartial("PartialViews/LeftFiller"); %>
</asp:Content>
<asp:Content ID="ProductContent" ContentPlaceHolderID="MainContent" runat="server">
	<% var moneyFormat = "#,###.00"; %>

	<script type="text/javascript">
		var autoFloatingTimer;
		$(document).ready(function () {
			LoadBanner('banners');

			$("#FloatingPanel").dialog({
				autoOpen: false,
				bgiframe: false,
				modal: true,
				draggable: false,
				height: 400,
				width: 620,
				resizable: false
			});
			$("#FloatingPanel").hide();
			$("#FloatingPanel").dialog("close");
			$("#FloatingPanel").bind("clickoutside", function (event) {
				$(this).hide();
				$(this).dialog("close");
			});

			$(".gridItem, .gridItem_v").live('click', function () {
				var $this = $(this);
				var val = $(this).prev().val();
				LoadProductDetail(val, "0");
			});

			$('.pix_diapo').diapo({ time: 30000, loader: 'none' });
			LoadSlider('MediumBanner', false);
			LoadSlider('OffersBanner', false);
			LoadSlider('NewProductsBanner', false);
			LoadSlider('GroupedBanner', false);
			$('.start-stop').hide();
			$('.thumbNav').hide();
			$("ul#TickerBanner").webTicker();
			$("ul#TickerBanner").show();
		});

		// Paginacion en sliders
		$('div.anythingSlider-metallic .forward, div.anythingSlider-metallic .back').live('click', function () {
			var $this = $(this);
			var maxPage = $this.parent().prev().prev().val();
			var curPage = $this.parent().prev().prev().prev().val();
			if ($this.hasClass('forward')) {
				if (curPage == maxPage) {
					curPage = 1;
				}
				else {
					curPage = parseInt(curPage) + 1;
				}
			}
			else if ($this.hasClass('back')) {
				if (curPage == 1) {
					curPage = maxPage;
				}
				else {
					curPage = curPage - 1;
				}
			}
			$this.parent().prev().prev().prev().val(curPage);
			var curBanner = $this.parent().parent().attr('id');
			$.get('<%= Url.Action("PageBanner", "Home") %>', { toPage: curPage, banner: curBanner },
			function (data) {
				var values = data.guids.split(';');
				for (i = 1; i <= data.count; i++) {
					var value = values[i - 1].split(',');
					var guid = value[0];
					var width = value[1];
					var height = value[2];
					var item = $('#' + curBanner + '_' + curPage + '_' + i);
					item.attr('src', '/ShowImage/?image=' + guid);
					item.attr('width', width + "px");
					item.attr('height', height + "px");
				}
			});
		});

		$('.loginbox a, .register').live('click', function () {

			var goAjax = false;
			var $this = $(this);
			var url = '';
			var _redirect = ''
			$('.SmallLoading').show();

			if ($this.attr('class') == 'login') {
				$(".ui-dialog-titlebar").hide();
				$("#FloatingPanel").bind("clickoutside", function (event) {
					$(this).hide();
					$(this).dialog("close");
				});
				url = '<%= Url.Action("Login", "Account") %>';
				goAjax = true;
			}
			else if ($this.attr('class') == 'register' || $this.attr('class') == 'profile') {
				$(".ui-dialog-titlebar").show();
				$("#FloatingPanel").unbind("clickoutside");
				$("#FloatingPanel").dialog("option", "title", 'Llene el formulario para registrarse en ExploraPrecios.com');
				url = '<%= Url.Action("Register", "Account") %>';
				goAjax = true;
			}
			else if ($this.attr('class') == 'logout') {
				goAjax = false;
				window.location = '<%= Url.Action("Logout", "Account") %>';
				goAjax = true;
			}
			else if ($this.attr('class') == 'forgot') {
				$(".ui-dialog-titlebar").hide();
				$("#FloatingPanel").bind("clickoutside", function (event) {
					$(this).hide();
					$(this).dialog("close");
				});
				url = '<%= Url.Action("Forgot", "Account") %>';
				goAjax = true;
			}
			_redirect = $('#Redirect').val();

			if (goAjax) {
				$.blockUI({ message: '<h4>Un momento por favor...</h4>' });
				$.ajax({
					type: "GET",
					url: url,
					data: { redirect: _redirect },
					dataType: "json",
					error: function (x, e) {
						$('.SmallLoading').hide();
						$.unblockUI();
					},
					success: function (data) {
						$('.SmallLoading').hide();
						if (data.result == "fail") {
							alert(data.msg);
							$.unblockUI();
						}
						else {
							$("#FloatingPanel").show();
							$("#FloatingPanel").dialog("open");
							$("#FloatingPanel").html(data.html);
							$.unblockUI();
						}
					}
				});
			}
		});

		function LoadSlider(id, isAuto) {
			$('#' + id)
			.anythingSlider({
				delay: 20000,
				autoPlay: isAuto,
				toggleControls: false,
				theme: 'metallic',
				navigationFormatter: function (i, panel) {
					return '' + i;
				}
			}).find('.panel:not(.cloned) label') // ignore the cloned panels
				.attr('rel', 'group')           // add all slider images to a colorbox group
				.colorbox({
					width: '90%',
					height: '90%',
					rel: 'group'
			});
		}

		function enlarge(ctr) {
			var gridItem = $(crt);
			var className =gridItem.attr('class');
			
			
			if(gridItem.hasClass('gridItem'))
			{
				//grid
				gridItem.toggleClass('gridItem_on');
			}
			else
			{
				//list
				gridItem.toggleClass('gridItem_v_on');
			}
		}
		function displaySwitch(type)
		{
			var grid = $(".topic .grid");
			var list = $(".topic .list");
			if(type == 'grid')
			{
				if(grid.hasClass('grid_on'))
					grid.removeClass('grid_on');
				else
					grid.addClass('grid_on');
			}
			else
			{
				if(list.hasClass('list_on'))
					list.removeClass('list_on');
				else
					list.addClass('list_on');   
			}
		}
		function changeDisplay(ctr, id)
		{
			var source = $('#' + ctr.id);
			if(source.hasClass('listview'))
			{
				$('#' + id + ' .gridItem').removeClass('gridItem').addClass('gridItem_v');
			}
			else
			{
				$('#' + id + ' .gridItem_v').removeClass('gridItem_v').addClass('gridItem');
			}
		}
		function LoadBanner(ctr)
		{
			$('#' + ctr + ' .banner:gt(0) ').css("display","none");
			$('#' + ctr + ' ul li a').mouseover(function(){
				var index = $('#' + ctr + ' ul li a').index(this);
				$('#' + ctr + ' .banner').css("display","none");
				$('#' + ctr + ' .banner:eq(' + index+ ') ').fadeIn('slow');
			});
		}
		function Clear() {
			clearInterval(autoFloatingTimer);
			$("#FloatingPanel").hide();
			$("#FloatingPanel").dialog("close");
		}
		function ClearRedirect(html) {
			clearInterval(autoFloatingTimer);
			$("#FloatingPanel").html(html);
		}
	</script>

	<div id="FloatingPanel"></div>
	<div id="SuccessFBRegister"></div>
	
	<section>     
		<div style="overflow:hidden; width:745px; margin: 10px 0px 10px 0px;"> 
			<div id="MegaBanner" class="pix_diapo">
				<div>
					<img src="/Content/images/banner/feedback-2.jpg">
					<div class="elemHover fadeIn" style="left:10px; top:25px; bottom:auto; padding-top:10px; width:270px;">
						<div style="font-size:30px; font-weight:bold; left:20px; top:0px; position:relative; color:white; color:#F8EA9B; text-shadow: 0 0 5px #D57B5F;">
							Crea tu grupo ya!!
						</div>
					</div>
					<div class="elemHover fromTop fadeIn" style="left:90px; top:80px; bottom:auto; padding-top:10px; width:430px; ">
						<div style="font-size:20px; font-weight:bold; left:200px; top:0px; position:relative; color:#F3F1F1; text-shadow: 0 0 5px #0D0D00;">
							Al compartir lo que quieres comprar con tus amigos puedes ganar ofertas y promociones
						</div>
					</div>
					<div class="elemHover caption fromLeft" style="bottom:40px; width:auto; -webkit-border-top-right-radius: 6px; -webkit-border-bottom-right-radius: 6px; -moz-border-radius-topright: 6px; -moz-border-radius-bottomright: 6px; border-top-right-radius: 6px; border-bottom-right-radius: 6px;">
						<label style="color:Red">
							ExploraPrecios.com
						</label>
						&nbsp;te premite compartir eso que quieres comprar y crear <b>Grupos de Compras</b>. Con estos <i>grupos</i> le dices a las tiendas que tu y otras personas quieren comprarlo también. Así creas la posibilidad que te ofrezcan grandes <b>rebajas y promociones.</b></div>
				</div>
				<div>
					<img src="/Content/images/banner/Luminesque_clinic_erina_beauty.jpg">
					<div class="elemHover fadeIn" style="left:10px; top:15px; bottom:auto; padding-top:10px; width:320px;">
					   <div style="font-size:30px; font-weight:bold; left:20px; top:0px; position:relative; color:white; color:#A1430A; text-shadow: 0 0 5px #FFFFFF;">
							Ya te puedes relajar!!
						</div>
					</div>
					<div class="elemHover fadeIn" style="left:100px; top:70px; bottom:auto; padding-top:10px; width:400px; ">
						<div style="font-size:20px; font-weight:bold; left:200px; top:0px; position:relative; color:#E7E7E7; text-shadow: 0 0 5px #201D1F;">
							Recibes todo tipo de beneficios al inscribirte con nosotros.
						</div>
					</div>
					<div class="elemHover caption fromLeft" style="bottom:40px; width:auto; -webkit-border-top-right-radius: 6px; -webkit-border-bottom-right-radius: 6px; -moz-border-radius-topright: 6px; -moz-border-radius-bottomright: 6px; border-top-right-radius: 6px; border-bottom-right-radius: 6px;">
						Al inscribirte <b>gratis</b> en <label style="color:Red">
							ExploraPrecios.com
						</label>
						&nbsp;estarás participando en sorteos con diferentes premios y lograrás enterarte de las últimas ofertas o cambios de precios en los productos que tú de verdad quieres.<br /><a href="javascript:void(0)" style="color:#CEF46E;" class="register">Regístrate</a> y pruébalo <b>gratis</b>
					</div>
				</div>
				<div>
					<img src="/Content/images/banner/media.771.jpg">
					<div class="elemHover fadeIn" style="left:10px; top:15px; bottom:auto; padding-top:10px; width:auto;">
						<div style="font-size:30px; font-weight:bold; left:20px; top:0px; position:relative; color:white; color:#DB0550; text-shadow: 0 0 5px #FFFFFF;">
							Conéctate, navega y ahorra!!
						</div>
					</div>
					<div class="elemHover fadeIn fromRight" style="left:80px; top:65px; bottom:auto; padding-top:10px; width:450px; ">
						<div style="font-size:20px; font-weight:bold; left:200px; top:0px; position:relative; color:#597D77; text-shadow: 0 0 10px #DBE9E9;">
							Conéctate y consigue todos los productos electrónicos que siempre quisiste al mejor precio
						</div>
					</div>
					<div class="elemHover caption fromLeft" style="bottom:40px; width:auto; -webkit-border-top-right-radius: 6px; -webkit-border-bottom-right-radius: 6px; -moz-border-radius-topright: 6px; -moz-border-radius-bottomright: 6px; border-top-right-radius: 6px; border-bottom-right-radius: 6px;">
						Lo que quieras en <a style="color:#CEF46E;" href="/Home/Products/?catlev=0&id=1">electrónicos</a> lo puedes encontrar en  
						<label style="color:Red">
							ExploraPrecios.com
						</label>
						&nbsp;al mejor precio en diferentes tiendas de tu preferencia. Navega y entérate de todas las oportunidades de ahorrar que tienes al escoger el precios mas bajo de lo que buscas.
					</div>
				</div>
				<div>
					<img src="/Content/images/banner/media.746.jpg">
					<div class="elemHover fadeIn" style="left:10px; top:15px; bottom:auto; padding-top:10px; width:auto;">
						<div style="font-size:30px; font-weight:bold; left:20px; top:0px; position:relative; color:white; color:#DB0550; text-shadow: 0 0 5px #FFFFFF;">
							Deja de correr de un lado a otro!!
						</div>
					</div>
					<div class="elemHover fadeIn" style="left:100px; top:70px; bottom:auto; padding-top:10px; width:400px; ">
						<div style="font-size:20px; font-weight:bold; left:200px; top:0px; position:relative; color:#E7E7E7; text-shadow: 0 0 5px #201D1F;">
							Ya tienes todo lo que necesitas para tu oficina o casa en un solo lugar
						</div>
					</div>
					<div class="elemHover caption fromLeft" style="bottom:40px; width:auto; -webkit-border-top-right-radius: 6px; -webkit-border-bottom-right-radius: 6px; -moz-border-radius-topright: 6px; -moz-border-radius-bottomright: 6px; border-top-right-radius: 6px; border-bottom-right-radius: 6px;">
						Si necesitas amueblar tu <a style="color:#CEF46E;" href="/Home/Products/?catlev=0&id=4">oficina</a> ya tienes donde buscar todo lo que necesitas en 
						<label style="color:Red">
							ExploraPrecios.com
						</label>.
						&nbsp;Donde puedes navegar a traves de todas las tiendas con sus productos en oferta para que equipes tu oficina al mejor precio posible y con productos nuevos directos de la tienda.
					</div>
				</div>
				<div>
					<img src="/Content/images/banner/Choplifter-HD-achievements-740x300.jpg">
					<div class="elemHover fadeIn" style="left:10px; top:15px; bottom:auto; padding-top:10px; width:420px;">
						<div style="font-size:30px; font-weight:bold; left:20px; top:0px; position:relative; color:white; color:#DB0550; text-shadow: 0 0 5px #FFFFFF;">
							Para que juegues sin parar!!
						</div>
					</div>
					<div class="elemHover fadeIn fromLeft" style="left:100px; top:70px; bottom:auto; padding-top:10px; width:400px; ">
						<div style="font-size:20px; font-weight:bold; left:200px; top:0px; position:relative; color:#E7E7E7; text-shadow: 0 0 5px #201D1F;">
							Te mostramos donde puedes comprar tus juegos preferidos al mejor precio
						</div>
					</div>
					<div class="elemHover caption fromLeft" style="bottom:40px; width:auto; -webkit-border-top-right-radius: 6px; -webkit-border-bottom-right-radius: 6px; -moz-border-radius-topright: 6px; -moz-border-radius-bottomright: 6px; border-top-right-radius: 6px; border-bottom-right-radius: 6px;">
						Aquí en
						<label style="color:Red">
							ExploraPrecios.com
						</label>&nbsp;tenemos los juegos que siempre quisiste en la sección de <a style="color:#CEF46E;" href="/Home/Products/?catlev=0&id=5">Juegos y juguetes</a> para que puedas cambiar de juegos cuando quieras al mejor precio.
					</div>
				</div>
				<div>
					<img src="/Content/images/banner/10.jpg">
					<div class="elemHover fadeIn" style="left:10px; top:15px; bottom:auto; padding-top:10px; width:auto;">
						<div style="font-size:30px; font-weight:bold; left:20px; top:0px; position:relative; color:white; color:#F1FFCF; text-shadow: 0 0 5px #FFFFFF;">
							Lo mejor para tu hogar!!
						</div>
					</div>
					<div class="elemHover fadeIn fromRight" style="top:70px; bottom:auto; padding-top:10px; width:400px; ">
						<div style="font-size:20px; font-weight:bold; left:300px; top:0px; position:relative; color:#E7E7E7; text-shadow: 0 0 5px #201D1F;">
							Lo que quieras para tu hogar lo tienes aquí al mejor precio
						</div>
					</div>
					<div class="elemHover caption fromLeft" style="bottom:30px; width:auto; -webkit-border-top-right-radius: 6px; -webkit-border-bottom-right-radius: 6px; -moz-border-radius-topright: 6px; -moz-border-radius-bottomright: 6px; border-top-right-radius: 6px; border-bottom-right-radius: 6px;">
						Tener un hogar con las mejores cosas y ahorres en el proceso es posible en <label style="color:Red">
							ExploraPrecios.com,
						</label>&nbsp;ya que en su sección de <a style="color:#CEF46E;" href="/Home/Products/?catlev=0&id=2">Hogar</a> tiene muchos productos nuevos para escoger en la tienda de tu preferencia al mejor precio.
					</div>
				</div>
				<div>
					<img src="/Content/images/banner/ibiza-club-tickets-5.jpg">
					<div class="elemHover fadeIn" style="left:10px; top:15px; bottom:auto; padding-top:10px; width:420px;">
						<div style="font-size:30px; font-weight:bold; left:20px; top:0px; position:relative; color:white; color:#F1FFCF; text-shadow: 0 0 5px #FFFFFF;">
							Para que ahorres de verdad!!
						</div>
					</div>
					<div class="elemHover caption fromRight" style="left:300px; top:70px; bottom:auto; padding-top:10px; width:230px; ">
						<div style="font-size:20px; font-weight:bold; top:0px; position:relative; color:#E7E7E7; text-shadow: 0 0 5px #201D1F;">
							<div><div style="float:left;">Síguenos en </div>&nbsp;&nbsp;<a title="Twitter" class="marketchannel" style="position:relative; left:20px; background-position:-288px -32px" rel="nofollow" target="_blank" href="http://twitter.com/exploraprecios"></a>&nbsp;&nbsp;<a title="Facebook" class="marketchannel" style="position:relative; left:55px; background-position:-96px 0px" rel="nofollow" target="_blank" href="http://www.facebook.com/exploraprecios"></a></div>
						</div>
					</div>
					<div class="elemHover caption fromLeft" style="bottom:30px; width:auto; -webkit-border-top-right-radius: 6px; -webkit-border-bottom-right-radius: 6px; -moz-border-radius-topright: 6px; -moz-border-radius-bottomright: 6px; border-top-right-radius: 6px; border-bottom-right-radius: 6px;">
						<label style="color:Red">
							ExploraPrecios.com
						</label>&nbsp;busca que ahorres en todo lo posible y a través de diferentes redes sociales divulga información para ayudarte a ahorrar en todos los aspectos de tu vida.
					</div>
				</div>
			</div>
		</div>
	</section>
	<div class="TickerBannerPanel">
		<% Html.RenderPartial("PartialViews/ProductTicker", Model.TickerList); %>
	</div>
	<!-- Define slider dimensions here -->
	<style type="text/css"> 
		#GroupedBanner, #OffersBanner, #NewProductsBanner {
		  width: 700px;
		  height: 120px;
		  list-style: none;
		 }
		#MediumBanner {
		  width: 700px;
		  list-style: none;
		  height:170px; 
		}
		section, highlighted {
			display: block;
			overflow: hidden;
			position: relative;
			width: inherit;
		}
	</style>
	<div id="ProductDisplayPanel">
		<% if (Model.HighlightProducts.Count() > 0)
		   {  %>
		<div id="Highlighted" class="highlighted">
			<input type="hidden" id="highlighted_page" value="1" />
			<input type="hidden" id="highlighted_max" value="<%= (int)(Model.HighlightProducts.Count() / 5) + 1 %>" />
			<label class="sectiontitle" style="color:#C80F02">Productos destacados</label>
			<ul id="MediumBanner">
				<%  var index = 0;
					var page = 0;
					var indexItem = 1;
					var isLiOpen = false;
					foreach (var product in Model.HighlightProducts)
					{
						var imageSize = product.Image.FitImage(120, 80);
						page = index % 5 == 0 ? page + 1 : page + 0;
						if (!isLiOpen)
						{
							isLiOpen = true;%>
					<li>
				<% } %>
					<input type="hidden" value="<%= product.ClientId %>" />
					<input type="hidden" value="<%= product.ProductId %>" />
					<div class="item <%= index % 5 == 0 ? "" : "border" %> <%= (index + 1) % 5 == 0 ? "" : "space" %> hand">		
						<div style="height:93px">
							<img id="Highlighted_<%= page %>_<%= indexItem %>"
								<% if (product.Image != null)
							   { %>src="/ShowImage/?image=<%= Explora_Precios.ApplicationServices.CommonUtilities.CacheImage(product.Image) %>" <% } %>
							   alt="image" width="<%= imageSize[0] %>" height="<%= imageSize[1] %>"  />
						</div>
						<div class="title"><%= product.Name.Shorten(25)%></div>
						<div class="itemprice">$<%= string.Format("{0:0.00}", product.Price)%></div>
						<div class="client"><%= product.Client%></div>			
					</div> 
					<% if ((isLiOpen && (index + 1) % 5 == 0) || Model.HighlightProducts.Count() - 1 == index)
					   {%>
					</li>
					<%  isLiOpen = false;
					   } %>     
				<%  
						index++;
						indexItem = index % 5 == 0 ? 1 : indexItem + 1;
					}
				%>
			</ul>
		</div>       
		<%} %> 
		<div id="Grouped" class="section">
			<label class="sectiontitle" style="color:#C80F02">Productos en grupos</label><img class="new" src="/Content/Images/nuevo-icon.gif" alt="nuevo" />
		<% if (Model.GroupedProducts.Count() > 0)
		   {  %>
			<input type="hidden" id="grouped_page" value="1" />
			<input type="hidden" id="grouped_max" value="<%= (int)(Model.GroupedProducts.Count() / 5) + 1 %>" />
			<ul id="GroupedBanner">
				<%  var index = 0;
					var page = 0;
					var indexItem = 1;
					var isLiOpen = false;
					foreach (var group in Model.GroupedProducts)
					{
						var imageSize = group.Image.FitImage(120, 80);
						var created = DateTime.Now.Subtract(group.CreatedDate);
						page = index % 5 == 0 ? page + 1 : page + 0;
						if (!isLiOpen)
						{
							isLiOpen = true;%>
					<li>
				<% } %>
					<input type="hidden" value="0" />
					<input type="hidden" value="<%= group.ProductId %>" />
					<div class="item <%= index % 5 == 0 ? "" : "border" %> <%= (index + 1) % 5 == 0 ? "" : "space" %> hand">
						<div class="title"><%= group.ProductName.Shorten(15)%></div>
						<div style="height:80px">
							<img class="image" id="Grouped_<%= page %>_<%= indexItem %>"
								<% if (group.Image != null)
							   { %>src="/ShowImage/?image=<%= Explora_Precios.ApplicationServices.CommonUtilities.CacheImage(group.Image) %>" <% } %>
							   alt="image" width="<%= imageSize[0] %>" height="<%= imageSize[1] %>" />
						</div>
						<div class="groupsize">Con <%= group.GroupSize %> persona<%= group.GroupSize > 1 ? "s" : "" %></div>
						<div class="client"><%= created.GroupCreated() %></div>
					</div> 
					<% if ((isLiOpen && (index + 1) % 5 == 0) || Model.GroupedProducts.Count() - 1 == index)
					   {%>
					</li>
					<%  isLiOpen = false;
					   } %>     
				<%  
						index++;
						indexItem = index % 5 == 0 ? 1 : indexItem + 1;
					}
				%>
			</ul> 
		<%} %> 		
		<% if (Model.GroupedProducts.Count() < 5)
			{%>
			<div style="margin-top:<%= Model.GroupedProducts.Count() > 0 ? 40 : 200 %>px; width:<%= (5 - Model.GroupedProducts.Count()) * 138 %>px; position:relative; top:-160px; left: <%= (Model.GroupedProducts.Count() * 138) + 32 %>px; text-align:center;">
				<h2>Que esperas?</h2><h2 style="color:#C80F02;"><img src="../../Content/Images/people.png" width="30px" height="30px" /> Has tu grupo de compra ahora!!</h2>
			</div>
			<% } %>  		
		</div>    		
		<% if (Model.OfferProducts.Count() > 5)
		   {  %>
		<div id="Offers" class="section">
			<input type="hidden" id="offer_page" value="1" />
			<input type="hidden" id="offer_max" value="<%= (int)(Model.OfferProducts.Count() / 5) + 1 %>" />
			<label class="sectiontitle">Productos en ofertas</label>
			<ul id="OffersBanner">
				<%  var index = 0;
					var page = 0;
					var indexItem = 1;
					var isLiOpen = false;
					foreach (var product in Model.OfferProducts)
					{
						var imageSize = product.Image.FitImage(120, 80);
						page = index % 5 == 0 ? page + 1 : page + 0;
						if (!isLiOpen)
						{
							isLiOpen = true;%>
					<li>
				<% } %>
					<input type="hidden" value="<%= product.ClientId %>" />
					<input type="hidden" value="<%= product.ProductId %>" />
					<div class="item hand <%= index % 5 == 0 ? "" : "border" %> <%= (index + 1) % 5 == 0 ? "" : "space" %>">						
						<div class="title"><%= product.Name.Shorten(15)%></div>
						<img class="image" id="Offers_<%= page %>_<%= indexItem %>"                        
						<% if (product.Image != null)
						   { %>src="/ShowImage/?image=<%= Explora_Precios.ApplicationServices.CommonUtilities.CacheImage(product.Image) %>" <% } %> 
						   alt="image" width="<%= imageSize[0] %>" height="<%= imageSize[1] %>" />  
						<div class="itemprice">$<%= string.Format("{0:0.00}", product.Price)%></div>
						<div class="client"><%= product.Client%></div>            
						   
					</div> 
					<% if ((isLiOpen && (index + 1) % 5 == 0) || Model.OfferProducts.Count() - 1 == index)
					   {%>
					</li>
					<%  isLiOpen = false;
					   } %>     
				<%  
						index++;
						indexItem = index % 5 == 0 ? 1 : indexItem + 1;
					} %>  
			</ul>
		</div>
		<%} %>
		<% if (Model.NewProducts.Count() > 0)
		   { %>
		<div id="News" class="section">
			<input type="hidden" id="new_page" value="1" />
			<input type="hidden" id="new_max" value="<%= (int)(Model.NewProducts.Count() / 5) + 1 %>" />
			<label class="sectiontitle">Productos nuevos</label>
			<ul id="NewProductsBanner">
				<%  var index = 0;
					var page = 0;
					var indexItem = 1;
					var isLiOpen = false;
					foreach (var product in Model.NewProducts)
					{
						var imageSize = product.Image.FitImage(120, 80);
						page = index % 5 == 0 ? page + 1 : page + 0;
						if (!isLiOpen)
						{
							isLiOpen = true;%>
					<li>
				<% } %>
					<input type="hidden" value="<%= product.ClientId %>" />
					<input type="hidden" value="<%= product.ProductId %>" />
					<div class="item hand <%= index % 5 == 0 ? "" : "border" %> <%= (index + 1) % 5 == 0 ? "" : "space" %>">
						<div class="title"><%= product.Name.Shorten(15)%></div>
						<img class="image" id="News_<%= page %>_<%= indexItem %>"
						<% if (product.Image != null)
							{ %> src="/ShowImage/?image=<%= Explora_Precios.ApplicationServices.CommonUtilities.CacheImage(product.Image) %>" <% } %> 
						 alt="image" width="<%= imageSize[0] %>" height="<%= imageSize[1] %>" />
						<div class="itemprice">$<%= string.Format("{0:0.00}", product.Price)%></div>
						<div class="client"><%= product.Client%></div>
					</div> 
					<% if ((isLiOpen && (index + 1) % 5 == 0) || Model.NewProducts.Count() - 1 == index)
					   {%>
					</li>
					<%  isLiOpen = false;
					   } %>     
				<%  
						index++;
						indexItem = index % 5 == 0 ? 1 : indexItem + 1;
					} %>     
			</ul>
		</div>
		<% } %>
	</div>
	<script type="text/javascript">
		$('div.section [id$=Banner] div.item, div.highlighted [id$=Banner] div.item').live('click', function () {
				LoadProductDetail($(this).prev().val(), $(this).prev().prev().val());
		});
	</script>

</asp:Content>
