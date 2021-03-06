﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.ProductViewModel>" %>

<%@ Import Namespace="Explora_Precios.Web.Controllers.Helpers" %>
<% var moneyFormat = "#,###.00";

   var highlight = Request.QueryString["_highlight"];
   var id_val = Explora_Precios.ApplicationServices.Helper.IdEncrypter.EncryptStringAES(Model.productId.ToString(), System.Configuration.ConfigurationSettings.AppSettings["PublicKey"]);
   var productImageSize = Model.productImage.FitImage(250, 200);%>

	<div id="FloatingPanel" class="details">
	<input type="hidden" id="displaying" value="<%= id_val %>" />
	<div id="BreadCrums">
		Producto en <b><%= Explora_Precios.Web.Controllers.Helpers.CatalogHelper.CatalogToString(true, Model.catalogProduct, true)%></b>
		
		<div class="attention disclaimerlink"><img src="../../Content/Images/information3-sc49.png" alt="Attention" /></div>
		<div class="rating" style="display:none;"><div class="text">Califica este producto </div><div id="star"></div><div class="thanks">Gracias!</div></div>
		<div class="follow"><label class="text bold hand">Sigue este producto </label>&nbsp;<div class="imagecontainer"><img class="image" alt="Follow Arrow" src="/Content/Images/red_arrow.jpg" /></div><div class="followPanel"></div></div>
		<div class="group">
		<% if (Model.group.Grouped != Explora_Precios.Web.Controllers.ViewModels.GroupDisplay.InGroup)
		{ %>
		<label class="text bold hand"><%= Model.group.Grouped == Explora_Precios.Web.Controllers.ViewModels.GroupDisplay.IncludeMe ? "Unirse al" : "Crea un"%> grupo</label>&nbsp;
		<% }
		else { %>
		<label class="text bold hand <%= Model.group.IsFacebooked ? "" : "noaction" %>">Grupo - <%= Model.group.GroupSize %> pers.</label>&nbsp;
		<% } %><div class="imagecontainer"><img class="image" alt="Group Product" src="/Content/Images/people.png" height="18px" width="18px" /></div><div class="groupPanel"></div>
		</div>
		<%--<div class="share">
			<!-- AddThis Button BEGIN -->
			<a class="addthis_button" href="http://www.addthis.com/bookmark.php?v=250&amp;pubid=ra-4f215c4a5401ec00"><img src="http://s7.addthis.com/static/btn/sm-share-en.gif" width="83" height="16" alt="Bookmark and Share" style="border:0"/></a>
			<!-- AddThis Button END -->    
		</div>--%>
	</div>
	<div class="productLeft">
		<img width="<%= productImageSize[0] %>" height="<%= productImageSize[1] %>" class="productImage" alt="<%= Model.productName %>" src="/ShowImage/?image=<%= Explora_Precios.ApplicationServices.CommonUtilities.CacheImage(Model.productImage) %>" />
		<!-- TODO LISTA DE IMAGENES -->
		<div class="qualities">
			<label class="title">Caracteristicas</label><div class="<%= Model.isLiked ? "liked" : "like" %> hand" style="float:right;" src="../../../Content/Images/facebook-like.jpg" width="79px" height="20px"></div>
				<% var index = 0; 
					foreach (var quality in Model.qualities)
				   {
					   %>
				<div class="qualityItem <%= index %> <%= index%2==1 ? "even" : "" %>">
					<label class="name"><%= quality.name %></label>
					<label class="value"><%= quality.value %></label>
				</div>
				<% index++;
				   } %>
		</div>
	</div>
	<div class="productRight">
		<div class="productTitle"><%--<h2><%= Model.productName %><br />(<%= Model.productRef %>)</h2>--%>
		<label>Desde 
			<label>$<%= Model.lowestPrice.ToString(moneyFormat) %></label> hasta 
			<label>$<%= Model.highestPrice.ToString(moneyFormat) %></label>
		</label>
		</div>
		<div class="productStores">
		<% if (highlight == "0")
		   { %>
		Se encuentra en <b><%= Model.clientList.Count%></b> tienda(s):<br/>
		<%} %>
		<% var first = true;
			foreach (var store in Model.clientList)
		   {
				if(highlight == "0" || highlight == store.clientId.ToString()){
			   var isReported = store.reported > store.modified;
				%>
			<div class="<%= (first ? "first" : "store") %>">
				<div class="storeName"><%= store.clientName %></div>
				<div><label class="<%= (store.specialPrice > 0) ? "discountOriginal" : "price" %>">$<%= store.price.ToString(moneyFormat) %></label><% if (store.specialPrice > 0) {%><label class="discountOffer"> - $<%= store.specialPrice.ToString(moneyFormat) %></label>&nbsp;<img src="../../Content/Images/etiqueta_oferta.gif" alt="Oferta" width="22px" height="25px" style="float:right;" /><% } %></div>
				<% if (store.url.Length > 0) { %>
					<% if (store.specialPrice == 0){%><br /><%}%><div class="openProduct">
						<div class="attention25_20" title="<%= isReported ? "Este producto ya ha sido reportado como desactualizado" : "Reporta este producto como desactualizado" %>">
							<div class="clickReport hand <%= isReported ? "reported" : "reportme" %>" style="float:left;"></div>
							<input type="hidden" class="current" value="<%= Explora_Precios.ApplicationServices.Helper.IdEncrypter.EncryptStringAES(store.clientProductId.ToString(), System.Configuration.ConfigurationSettings.AppSettings["PublicKey"]) %>" />
							<a href="/Home/AccessLink?cpId=<%= store.clientProductId %>&url=<%= store.url %>" target="_blank">Ir a la pagina (<%= store.domain %>)</a>
							<div class="ProductReport" style="display:none; width:330px; padding-top: 5px;"><img id="reportLoadingImg" src="../../Content/Images/ajax-loader.gif" alt="Loading..." /></div>
						</div>
					</div> 
				<%}%>
			</div><br />
		<% first = false;
		   } 
		   } %>
		</div>
	</div>
</div>
<div></div>
<script type="text/javascript">
	var autoTimer;
	$(document).ready(function () {
		//$(".followPanel").hide();
		$("#reportLoadingImg").hide();
		$(".thanks").hide();
		$(".like").click(function(){
			var $this = $(this);
			$.ajax({
				type: "GET",
				url: '<%= Url.Action("SetLike", "Home") %>',
				data: { _id: $("#displaying").val() },
				dataType: "json",
				error: function (x, e) {
					jAlert('Error', 'Alerta!');
				},
				success: function (data) {
					if (data.result == "fail") {
						jAlert(data.msg, 'Alerta!');
					}
					else {                   
						$this.removeClass('like');
						$this.addClass('liked');
					}
				}
			});
		});
	});
	$("#star").raty({
		click: function (score, evt) {
			$.ajax({
				type: "GET",
				url: '<%= Url.Action("SetRate", "Home") %>',
				data: { _product: $("#displaying").val(), _value: score },
				dataType: "json",
				error: function (x, e) {
					alert("error");
				},
				success: function (data) {
					if (data.result == "fail") {
						alert(data.msg);
					}
					else {                        
						$(".thanks").show('slow');
						autoTimer = setInterval("ClearThanks("+data.avg+")", 5000);
					}
				}
			});
		},
		hintList: ['Malo', 'Mas o menos', 'Regular', 'Bueno', 'Muy bueno'],
		start: <%= Model.rating %>
	});

	$(".follow .text").click(function(){
		ClickEvent('follow');
	});

	$(".follow .image").click(function(){
		ClickEvent('follow');
	});

	$(".group .text").click(function(){
		if(!$(this).hasClass('noaction'))
			ClickEvent('group');
	});

	$(".group .image").click(function(){    
		if(!$(".group .text").hasClass('noaction'))
			ClickEvent('group');
	});

	function ClickEvent(obj)
	{
		if(!$("." + obj + " .text").hasClass('showing')){
			$("." + obj + " .text").addClass('showing');
			$("." + obj + " .imagecontainer").addClass('showing');

			$.ajax({
				type: "GET",
				url: '/Home/Get' + obj.charAt(0).toUpperCase() + obj.substring(1),
				data: { redirect: $('#displaying').val() },
				dataType: "json",
				error: function (x, e) {
					alert('Error');
				},
				success: function (data) {
					if (data.result == "fail") {
						alert(data.msg);
					}
					else {
						$('.' + obj + 'Panel').html(data.html);
						$('.' + obj + 'Panel').show(); 
					}
				}
			});   
		}
		else
		{
			ClosePanel(obj);
		}
	}

	function ClearThanks(avg) {
		clearInterval(autoTimer);
		$(".thanks").hide('slow');
		$("#star").raty('start', avg);
	}

	function ClearPanel(obj) {
		clearInterval(autoTimer);
		$('.' + obj + 'Done').hide();
		ClosePanel(obj.charAt(0).toLowerCase() + obj.substring(1));
	}
	 
	function ClosePanel(obj) {
		$("." + obj + " .text").removeClass('showing');
		$("." + obj + " .imagecontainer").removeClass('showing');
		$('.' + obj + 'Panel').hide(); 
	}

	$(".follow .imagecontainer").simpletip({
		content: '<table><tr><td>Al seguir un producto podrás saber el momento exacto cuando el mismo este en oferta <br />o cuando este baje de precio. Cuando estas condiciones se cumplan recibirás un correo electrónico.<br/><br/>Cualquier pregunta dirigirse a <a href="/Contact" class="FollowContact" style="color:Red;">Contacto</a> o síguenos en <a href="http://www.facebook.com/ExploraPrecios" target="_blank" class="FollowFacebook" style="color:Red;">Facebook</a> donde encontrarás instrucciones de como utilizarlo.</td></tr></table>',
		fixed: true,
		position: [-150, 20],
		showEffect: 'slide',
		hideEffect: 'fade',
		showTime: 300
	});
	
	$(".group .imagecontainer").simpletip({
		content: '<table><tr><td>Al <%= Model.group.Grouped == Explora_Precios.Web.Controllers.ViewModels.GroupDisplay.IncludeMe ? "ingresar a este" : "crear un" %> grupo <%= Model.group.Grouped == Explora_Precios.Web.Controllers.ViewModels.GroupDisplay.IncludeMe ? "" : "con este producto" %> estarás mostrándole tú interés de participar en un mega descuento a las tiendas que venden este producto.<br /><br /> Mientras mas personas entren a este grupo mejor, así que envíaselo a todos tus amigos y ahorra mucho dinero.<br/><br/>Cualquier pregunta dirigirse a <a href="/Contact" class="FollowContact" style="color:Red;">Contacto</a> o síguenos en <a href="http://www.facebook.com/ExploraPrecios" target="_blank" class="FollowFacebook" style="color:Red;">Facebook</a> donde encontrarás instrucciones de como utilizarlo.</td></tr></table>',
		fixed: true,
		position: [-200, 20],
		showEffect: 'slide',
		hideEffect: 'fade',
		showTime: 300
	});

	$(".exploraoferta").simpletip({
		content: '<table><tr><td><img src="../../Content/Images/eo_tag.jpg" width="50px" height="50px" /></td><td>Este símbolo indica que ExploraPrecios.com disminuye un 10% la oferta ofrecida por la tienda. <br/>Solo haga click aquí y guarde el código generado.</td></tr></table>',
		fixed: true,
		position: [-50, 20],
		showEffect: 'slide',
		hideEffect: 'fade',
		showTime: 300
	});

	$(".disclaimerlink").simpletip({
		content: disclaimer,
		fixed: true,
		position: [-300, 20],
		showEffect: 'slide',
		hideEffect: 'fade',
		showTime: 300
	});

	$(".clickReport").click(function () {
		var $this = $(this);
		if ($this.hasClass('reportme')) {
			var productReportDiv = $this.next().next().next();
			var goOn = !productReportDiv.hasClass('showing');
			$(".ProductReport").hide('slow');
			$(".ProductReport").removeClass('showing');

			if (goOn) {
				$("#current").val($this.next().val());
				productReportDiv.show('slow');
				$("#reportLoadingImg").show();
				if (!productReportDiv.hasClass('showing')) {
					$.ajax({
						type: "GET",
						url: '<%= Url.Action("GenerateCaptcha", "Home") %>',
						dataType: "json",
						error: function (x, e) {
							productReportDiv.slideUp();
							productReportDiv.hide();
							productReportDiv.html('');
							alert("error");
						},
						success: function (data) {
							if (data.result == "fail") {
								productReportDiv.hide();
								productReportDiv.html('');
								alert(data.msg);
							}
							else {
								$("#text").val(data.text);
								productReportDiv.addClass('showing');
								productReportDiv.html(data.html);
							}
						}
					});
				}
				else {
					productReportDiv.removeClass('showing');
					productReportDiv.hide('slow');
				} 
			}
		}
	});
</script>
