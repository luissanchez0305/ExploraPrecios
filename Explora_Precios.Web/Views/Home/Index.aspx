<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Explora_Precios.Web.Controllers.ViewModels.IntroViewModel>" %>
<%@ Import Namespace="Explora_Precios.Web.Controllers.Helpers" %>
<asp:Content ID="HeaderTitleContent" ContentPlaceHolderID="TitleContent" runat="server">
ExploraPrecios.com
</asp:Content>
<asp:Content ID="LeftMenuContent" ContentPlaceHolderID="LeftContent" runat="server">
<script charset="utf-8" src="http://widgets.twimg.com/j/2/widget.js"></script>
<script>
	new TWTR.Widget({
		version: 2,
		type: 'profile',
		rpp: 6,
		interval: 30000,
		width: 220,
		height: 500,
		theme: {
			shell: {
				background: '#A7C2CF',
				color: '#292710'
			},
			tweets: {
				background: '#ffffff',
				color: '#574e44',
				links: '#a88962'
			}
		},
		features: {
			scrollbar: false,
			loop: true,
			live: true,
			behavior: 'all'
		}
	}).render().setUser('exploraprecios').start();
</script>
<div style="width: 200px; padding-top:10px;">
<script type="text/javascript"><!--
	google_ad_client = "ca-pub-8106411801578478";
	/* 200x200 Small Left */
	google_ad_slot = "5459383721";
	google_ad_width = 200;
	google_ad_height = 200;
//-->
</script>
</div>
<script type="text/javascript"
src="http://pagead2.googlesyndication.com/pagead/show_ads.js">
</script>
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

			//LoadSlider('MegaBanner', true);
			$('.pix_diapo').diapo({ time: 30000 });
			LoadSlider('MediumBanner', false);
			LoadSlider('OffersBanner', false);
			LoadSlider('NewProductsBanner', false);
			$('.start-stop').hide();
			$('.thumbNav').hide();
			$("ul#TickerBanner").webTicker();

		});

		$('.loginbox a, .register').live('click', function () {
			var goAjax = true;
			var $this = $(this);
			var url = '';
			var _redirect = ''
			$('.LoginLoading').show();

			//            alert($this.attr('class'));
			if ($this.attr('class') == 'login') {
				$(".ui-dialog-titlebar").hide();
				$("#FloatingPanel").bind("clickoutside", function (event) {
					$(this).hide();
					$(this).dialog("close");
				});
				url = '<%= Url.Action("Login", "Account") %>';
			}
			else if ($this.attr('class') == 'register' || $this.attr('class') == 'profile') {
				$(".ui-dialog-titlebar").show();
				$("#FloatingPanel").unbind("clickoutside");
				$("#FloatingPanel").dialog("option", "title", 'Llene el formulario para registrarse en ExploraPrecios.com');
				url = '<%= Url.Action("Register", "Account") %>';
			}
			else if ($this.attr('class') == 'logout') {
				goAjax = false;
				window.location = '<%= Url.Action("Logout", "Account") %>';
			}
			else if ($this.attr('class') == 'forgot') {
				$(".ui-dialog-titlebar").hide();
				$("#FloatingPanel").bind("clickoutside", function (event) {
					$(this).hide();
					$(this).dialog("close");
				});
				url = '<%= Url.Action("Forgot", "Account") %>';
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
						$('.LoginLoading').hide();
						$.unblockUI();
					},
					success: function (data) {
						$('.LoginLoading').hide();
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
			var gridItem = $('#' + ctr.id);
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
					<img src="/Content/images/banner/Crowd1.jpg">
					<div class="elemHover fadeIn" style="left:10px; top:25px; bottom:auto; padding-top:10px; width:auto;">
						<div style="font-size:30px; font-weight:bold; left:20px; top:0px; position:relative; color:white; color:#F8EA9B; text-shadow: 0 0 5px #D57B5F;">
							Ya todos lo saben!!
						</div>
					</div>
					<div class="elemHover fromTop fadeIn" style="left:90px; top:80px; bottom:auto; padding-top:10px; width:430px; ">
						<div style="font-size:20px; font-weight:bold; left:200px; top:0px; position:relative; color:#F3F1F1; text-shadow: 0 0 5px #0D0D00;">
							Ha llegado la solución que esperabas para ahorrar comprando lo que de verdad buscas
						</div>
					</div>
					<div class="elemHover caption fromLeft" style="bottom:40px; width:auto; -webkit-border-top-right-radius: 6px; -webkit-border-bottom-right-radius: 6px; -moz-border-radius-topright: 6px; -moz-border-radius-bottomright: 6px; border-top-right-radius: 6px; border-bottom-right-radius: 6px;">
						<label style="color:Red">
							ExploraPrecios.com
						</label>
						&nbsp;te premite explorar los productos de la tienda de tu preferencia desde tu computadora. Sin levantar el telefono, sin salir de casa u oficina y con las <b>ofertas</b> del momento.
					</div>
				</div>
				<div>
					<img src="/Content/images/banner/Luminesque_clinic_erina_beauty.jpg">
					<div class="elemHover fadeIn" style="left:10px; top:15px; bottom:auto; padding-top:10px; width:auto;">
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
					<div class="elemHover fadeIn" style="left:10px; top:15px; bottom:auto; padding-top:10px; width:auto;">
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
					<div class="elemHover fadeIn" style="left:10px; top:15px; bottom:auto; padding-top:10px; width:auto;">
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
		#OffersBanner, #NewProductsBanner {
		  width: 700px;
		  height: 90px;
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
		}
	</style>
	<div id="ProductDisplayPanel">
		<% if (Model.HighlightProducts.Count() > 0)
		   {  %>
		<div class="highlighted">
			<label class="sectiontitle" style="color:#C80F02">Productos destacados</label>
			<ul id="MediumBanner">
				<%  var index = 0;
					var isLiOpen = false;
					foreach (var product in Model.HighlightProducts)
					{
						if (!isLiOpen)
						{
							isLiOpen = true;%>
					<li>
				<% } %>
					<input type="hidden" value="<%= product.ClientId %>" />
					<input type="hidden" value="<%= product.ProductId %>" />
					<div class="item <%= index % 5 == 0 ? "" : "border" %> <%= (index + 1) % 5 == 0 ? "" : "space" %>">
						<% if (product.Image != null)
						   { %>
						<img src="/ShowImage/?image=<%= new Explora_Precios.ApplicationServices.CommonUtilities().CacheImage(product.Image) %>" alt="image" />
						<% } %>
						<div class="title"><%= product.Name.Shorten(25)%></div>
						<div class="itemprice">$<%= string.Format("{0:0.00}", product.Price)%></div>
						<div class="client"><%= product.Client%></div>
					</div> 
					<% if ((isLiOpen && (index + 1) % 5 == 0) || Model.HighlightProducts.Count() - 1 == index)
					   {%>
					</li>
					<% isLiOpen = false;
					   } %>     
				<%  
					   index++;
					}
				%>
			</ul>
		</div>       
		<%} %> 
		<% if (Model.OfferProducts.Count() > 0)
		   {  %>
		<div class="section">
			<label class="sectiontitle">Productos en ofertas</label>
			<ul id="OffersBanner">
				<%  var index = 0;
					var isLiOpen = false;
					foreach (var product in Model.OfferProducts)
					{
						if (!isLiOpen)
						{
							isLiOpen = true;%>
					<li>
				<% } %>
					<input type="hidden" value="<%= product.ClientId %>" />
					<input type="hidden" value="<%= product.ProductId %>" />
					<div class="item">
						<div class="title"><%= product.Name.Shorten(15)%></div>
						<div class="itemprice">$<%= string.Format("{0:0.00}", product.Price)%></div>
						<div class="client"><%= product.Client%></div>
						<% if (product.Image != null)
						   { %>
						<img src="/ShowImage/?image=<%= new Explora_Precios.ApplicationServices.CommonUtilities().CacheImage(product.Image) %>" alt="image" />                        
					   <% } %>     
					</div> 
					<% if ((isLiOpen && (index + 1) % 5 == 0) || Model.OfferProducts.Count() - 1 == index)
					   {%>
					</li>
					<% isLiOpen = false;
					   } %>     
				<%  
					   index++;
					} %>  
			</ul>
		</div>
		<%} %>
		<% if (Model.NewProducts.Count() > 0)
		   { %>
		<div class="section">
			<label class="sectiontitle">Productos nuevos</label>
			<ul id="NewProductsBanner">
				<%  var index = 0;
					var isLiOpen = false;
					foreach (var product in Model.NewProducts)
					{
						if (!isLiOpen)
						{
							isLiOpen = true;%>
					<li>
				<% } %>
					<input type="hidden" value="<%= product.ClientId %>" />
					<input type="hidden" value="<%= product.ProductId %>" />
					<div class="item">
						<div class="title"><%= product.Name.Shorten(15)%></div>
						<div class="itemprice">$<%= string.Format("{0:0.00}", product.Price)%></div>
						<div class="client"><%= product.Client%></div>
						<% if (product.Image != null)
                            { %>
						<img src="/ShowImage/?image=<%= new Explora_Precios.ApplicationServices.CommonUtilities().CacheImage(product.Image) %>" alt="image" />
                        <% } %>
					</div> 
					<% if ((isLiOpen && (index + 1) % 5 == 0) || Model.NewProducts.Count() - 1 == index)
					   {%>
					</li>
					<% isLiOpen = false;
					   } %>     
				<%  
					   index++;
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
