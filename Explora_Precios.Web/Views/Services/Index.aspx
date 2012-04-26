<%@ Page Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>ExploraPrecios - Planes & Servicios</title>
	<link href="../../Content/Style.css" rel="stylesheet" type="text/css" />
	<link rel="stylesheet" type="text/css" href="http://http.cdnlayer.com/wsl/Styles/lcms.css?111013" />   
	<%--<link rel="stylesheet" type="text/css" href="http://www.lightcms.com/websites/luz/templates/LightCMS/css/style.css" />--%>
	<link href="../../Content/ServicesStyle.css" rel="stylesheet" type="text/css" />
	<link rel="stylesheet" type="text/css" href="http://www.lightcms.com/websites/luz/templates/LightCMS/css/windows.css" />    
	<!--[if IE 6]><link href="http://www.lightcms.com/styles/ie6.css" rel="stylesheet" type="text/css" /><![endif]-->		<!--[if IE 7]><link href="http://www.lightcms.com/styles/ie7.css" rel="stylesheet" type="text/css" /><![endif]-->    <!--[if IE 8]><link rel="stylesheet" href="http://www.lightcms.com/websites/luz/templates/LightCMS/css/ie8.css" /><![endif]-->    <!--[if IE 7]><link rel="stylesheet" href="http://www.lightcms.com/websites/luz/templates/LightCMS/css/ie7.css" /><![endif]-->
	<script type="text/javascript" src="../../Scripts/jquery-1.6.4.min.js"></script>
	<script type="text/javascript" src="../../Scripts/Explora_Precios.js"></script>
	<script type="text/javascript" src="../../Scripts/jquery.simpletip-1.3.1.min.js"></script></head>
<body class="insideFullWidth pricing">
	<div id="pageHeader">    
		<div class="logo" style="float:left;">
			
		</div>
		<div id="pageTitle">Planes y Servicios</div>
		<div id="contact">Para mayor información contáctenos <a href="/Contact"><img src="../../Content/Images/mail-icon.png" alt="Contactenos" width="20px" height="15px"/></a>
		<br />o escríbanos a <a href="mailto:info@exploraprecios.com" title="info@exploraprecios.com">info@exploraprecios.com</a></div>
	</div>
	<div id="bodyWrapper">
		<div id="body">        			<div id="mainColumn">				<div id="pricingContainer">                					<section id="pricingColumns">				
						<div class="pricingColumnDescription" style="float:left; padding-left:5px;">
							<h2>&nbsp;</h2>
							<p class="price">&nbsp;</p>							<p class="price_6months">&nbsp;</p>  							<p class="price_12months">&nbsp;</p>                          							<ul>                            								<li><div class="info products height"><img src="../../Content/images/ico_info.png" alt="attention" width="15px" height="15px" /></div><span>Productos</span></li>								<li class="pricingColumnsStore">Store Features</li>                     
								<li><div class="info groups height"><img src="../../Content/images/ico_info.png" alt="attention" width="15px" height="15px" /></div><span>Grupos de <br />compra</span></li>   								<li class="pricingColumnsStore">Store Features</li>                     
								<li><div class="info follow height"><img src="../../Content/images/ico_info.png" alt="attention" width="15px" height="15px" /></div><span>Sigue el <br />producto</span></li>   								<li class="pricingColumnsStore">Store Features</li>                     
								<li><div class="info mainBanner height"><img src="../../Content/images/ico_info.png" alt="attention" width="15px" height="15px" /></div><span>Publicación en<br />banner principal</span></li>      								<li class="pricingColumnsStore">Store Features</li>                 								<li><div class="info highlights"><img src="../../Content/images/ico_info.png" alt="attention" width="15px" height="15px" /></div><span>Destacados en exploraprecios.com</span></li>                    								<li class="pricingColumnsStore">Store Features</li>                 
								<li><div class="info marketChannel height"><img src="../../Content/images/ico_info.png" alt="attention" width="15px" height="15px" /></div><span>Publicación en<br /><i>market channels</i></span></li>   
							</ul>						</div>                    
						<div class="pricingColumn" id="pricingColumn_basic" style="float:left; padding-left:5px;">                        
							<h2>Básico</h2>                        
							<p class="price">$35<span>/mes</span></p>
							<p class="price_6months">$190<span> /6 meses</span></p>
							<p class="price_6months">$364<span> /12 meses</span></p>
							<span class="pricingBadge">&nbsp;</span>                        							<ul>                            								<li><span>10 - 30</span></li>								<li class="pricingColumnsStore">Store Features</li>                     
								<li><img src="../../Content/Images/ico_ok.gif" width="15px" height="15px" /></li>   								<li class="pricingColumnsStore space">Store Features</li>                     
								<li><img src="../../Content/Images/ico_ok.gif" width="15px" height="15px" /></li> 								<li class="pricingColumnsStore space">Store Features</li>                 
								<li><img src="../../Content/Images/ico_no.png" width="15px" height="15px" /></li>  								<li class="pricingColumnsStore space">Store Features</li>                            
								<li><img src="../../Content/Images/ico_no.png" width="15px" height="15px" /></li>								<li class="pricingColumnsStore space">Store Features</li>                          								<li><img src="../../Content/Images/ico_no.png" width="15px" height="15px" /></li>           
							</ul>                    
						</div>                    
						<div class="pricingColumn" id="pricingColumn_plus" style="float:left; padding-left:5px;">                        
							<h2>Standard</h2>
							<p class="price">$71<span>/mes</span></p>							<p class="price_6months">$393<span> /6 meses</span></p>							<p class="price_12months">$761<span> /12 meses</span></p>							<span class="pricingBadge">Popular</span>							<ul>                            								<li><span>31 - 60</span></li>								<li class="pricingColumnsStore">Store Features</li>                     
								<li><img src="../../Content/Images/ico_ok.gif" width="15px" height="15px" /></li>   								<li class="pricingColumnsStore space">Store Features</li>                     
								<li><img src="../../Content/Images/ico_ok.gif" width="15px" height="15px" /></li> 								<li class="pricingColumnsStore space">Store Features</li>                     
								<li><img src="../../Content/Images/ico_ok.gif" width="15px" height="15px" /></li>    								<li class="pricingColumnsStore space">Store Features</li>                            								<li><img src="../../Content/Images/ico_no.png" width="15px" height="15px" /></li>                        								<li class="pricingColumnsStore space">Store Features</li>       
								<li><img src="../../Content/Images/ico_no.png" width="15px" height="15px" /></li>                        							</ul>                    						</div>                    						<div class="pricingColumn" id="pricingColumn_advanced" style="float:left; padding-left:5px;">                        							<h2>Avanzado</h2>                        							<p class="price">$134<span>/mes</span></p>							<p class="price_6months">$739<span> /6 meses</span></p>							<p class="price_12months">$1,430<span> /12 meses</span></p>                        							<ul>                            								<li><span>61 - 90</span></li>								<li class="pricingColumnsStore">Store Features</li>                     
								<li><img src="../../Content/Images/ico_ok.gif" width="15px" height="15px" /></li>   								<li class="pricingColumnsStore space">Store Features</li>                     
								<li><img src="../../Content/Images/ico_ok.gif" width="15px" height="15px" /></li> 								<li class="pricingColumnsStore space">Store Features</li>                      
								<li><img src="../../Content/Images/ico_ok.gif" width="15px" height="15px" /></li>        								<li class="pricingColumnsStore space">Store Features</li>                            
								<li><img src="../../Content/Images/ico_ok.gif" width="15px" height="15px" /></li>                     								<li class="pricingColumnsStore space">Store Features</li>                            
								<li><img src="../../Content/Images/ico_no.png" width="15px" height="15px" /></li>      							</ul>                    						</div>                    						<div class="pricingColumn" id="pricingColumn_pro" style="float:left; padding-left:5px;">                        							<h2>Premium</h2>                        							<p class="price">$184<span>/mes</span></p>							<p class="price_6months">$1,041<span> /6 meses</span></p>							<p class="price_12months">$1,965<span> /12 meses</span></p>							<ul>                            								<li><span>91+</span></li>								<li class="pricingColumnsStore">Store Features</li>                     
								<li><img src="../../Content/Images/ico_ok.gif" width="15px" height="15px" /></li>   								<li class="pricingColumnsStore space">Store Features</li>                     
								<li><img src="../../Content/Images/ico_ok.gif" width="15px" height="15px" /></li> 								<li class="pricingColumnsStore space">Store Features</li>                            
								<li><img src="../../Content/Images/ico_ok.gif" width="15px" height="15px" /></li>                         								<li class="pricingColumnsStore space">Store Features</li>                            								<li><img src="../../Content/Images/ico_ok.gif" width="15px" height="15px" /></li>                        								<li class="pricingColumnsStore space">Store Features</li>                            								<li><img src="../../Content/Images/ico_ok.gif" width="15px" height="15px" /></li>                       							</ul>                    						</div>					</section>				</div>
			</div>
		</div>
	</div>
	<div class="textElement">
		<div id="pageFooter">
			© 2011 MVC E-commerce. All Rights Reserved. Created by <a class="contactus" href="/Contact" target="_self" title="Contactáctenos">eSphera</a>
		</div>
	</div>
	<script type="text/javascript">
		$('.info.products').simpletip({
			content: '<table><tr><td><h2>Productos</h2>La publicación de productos de forma unitaria tiene un costo de $1.50 al mes.<br/><br/>Cualquier pregunta dirigirse a <a href="/Contact" class="FollowContact" style="color:Red;">Contacto</a> o síguenos en <a href="http://www.facebook.com/ExploraPrecios" target="_blank" class="FollowFacebook" style="color:Red;">Facebook</a> donde encontrarás instrucciones de como utilizarlo.</td></tr></table>',
			fixed: true,
			position: [-120, 30],
			showEffect: 'slide',
			hideEffect: 'fade',
			showTime: 300
		});
		$('.info.groups').simpletip({
			content: '<table><tr><td><h2>Grupos de compra</h2>Este servicio le permite a nuestros usuarios crear grupos de compras con sus productos. Así ud. logrará hacer ventas en grupo deshaciéndose de mucho de su inventario fácilmente.<br/><br/>Cualquier pregunta dirigirse a <a href="/Contact" class="FollowContact" style="color:Red;">Contacto</a> o síguenos en <a href="http://www.facebook.com/ExploraPrecios" target="_blank" class="FollowFacebook" style="color:Red;">Facebook</a> donde encontrarás instrucciones de como utilizarlo.</td></tr></table>',
			fixed: true,
			position: [-120, 80],
			showEffect: 'slide',
			hideEffect: 'fade',
			showTime: 300
		});
		$('.info.follow').simpletip({
			content: '<table><tr><td><h2>Sigue el producto</h2>Los usuarios al seguir productos van a poder saber de cambios en los precios de sus productos o enterarse inmediatamente cuando estos entran en oferta, acercándolos aún mas con su tienda.<br/><br/>Cualquier pregunta dirigirse a <a href="/Contact" class="FollowContact" style="color:Red;">Contacto</a> o síguenos en <a href="http://www.facebook.com/ExploraPrecios" target="_blank" class="FollowFacebook" style="color:Red;">Facebook</a> donde encontrarás instrucciones de como utilizarlo.</td></tr></table>',
			fixed: true,
			position: [-120, 160],
			showEffect: 'slide',
			hideEffect: 'fade',
			showTime: 300
		});
		$('.info.mainBanner').simpletip({
			content: '<table><tr><td><h2>Banner Principal</h2>Aprovechando el flujo usuarios en constante crecimiento contamos con un banner que ocupa casi todo el rango visual del espectador, lo cual lo hace excelente para ser utilizado para promover su mercancia o diversos servicios.<br/>El costo unitario por publicación es de $7.50.<br/><br/>Cualquier pregunta dirigirse a <a href="/Contact" class="FollowContact" style="color:Red;">Contacto</a> o síguenos en <a href="http://www.facebook.com/ExploraPrecios" target="_blank" class="FollowFacebook" style="color:Red;">Facebook</a> donde encontrarás instrucciones de como utilizarlo.</td></tr></table>',
			fixed: true,
			position: [-120, 220],
			showEffect: 'slide',
			hideEffect: 'fade',
			showTime: 300
		});
		$('.info.highlights').simpletip({
			content: '<table><tr><td><h2>Productos Destacados</h2>Los productos introducidos en estos banners podrán ser vistos en cada una de las páginas abiertas por los usuarios.<br/>El costo de publicaciones ilimitadas es de $10.00 al mes.<br/><br/>Cualquier pregunta dirigirse a <a href="/Contact" class="FollowContact" style="color:Red;">Contacto</a> o síguenos en <a href="http://www.facebook.com/ExploraPrecios" target="_blank" class="FollowFacebook" style="color:Red;">Facebook</a> donde encontrarás instrucciones de como utilizarlo.</td></tr></table>',
			fixed: true,
			position: [-120, 350],
			showEffect: 'slide',
			hideEffect: 'fade',
			showTime: 300
		});
		$('.info.marketChannel').simpletip({
			content: '<table><tr><td><h2>Facebook y Twitter</h2>Si su tienda cuenta con Facebook o Twitter, toda la interactividad que los usuarios tengan con sus productos será reflejada en la página de su tienda.<br/>El costo de publicaciones ilimitadas es de $12.50 al mes.<br/><br/>Cualquier pregunta dirigirse a <a href="/Contact" class="FollowContact" style="color:Red;">Contacto</a> o síguenos en <a href="http://www.facebook.com/ExploraPrecios" target="_blank" class="FollowFacebook" style="color:Red;">Facebook</a> donde encontrarás instrucciones de como utilizarlo.</td></tr></table>',
			fixed: true,
			position: [-120, 430],
			showEffect: 'slide',
			hideEffect: 'fade',
			showTime: 300
		});
	</script>
</body>
</html>
