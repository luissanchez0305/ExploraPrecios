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
    <!--[if IE 6]><link href="http://www.lightcms.com/styles/ie6.css" rel="stylesheet" type="text/css" /><![endif]-->        <!--[if IE 7]><link href="http://www.lightcms.com/styles/ie7.css" rel="stylesheet" type="text/css" /><![endif]-->    <!--[if IE 8]><link rel="stylesheet" href="http://www.lightcms.com/websites/luz/templates/LightCMS/css/ie8.css" /><![endif]-->    <!--[if IE 7]><link rel="stylesheet" href="http://www.lightcms.com/websites/luz/templates/LightCMS/css/ie7.css" /><![endif]-->
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
    <div id="body">        <div id="mainColumn"><div id="pricingContainer">                <section id="pricingColumns">                
                    <div class="pricingColumnDescription" style="float:left; padding-left:5px;">                    
                        <h2>&nbsp;</h2>
                        <p class="price">&nbsp;</p><p class="price_6months">&nbsp;</p>  <p class="price_12months">&nbsp;</p>                          <ul>                            <li><span>Productos</span></li>                            <li class="pricingColumnsStore">Store Features</li>                            <li><span>Ofertas en exploraprecios.com*</span></li>                            <li class="pricingColumnsStore">Store Features</li>                            <li><span>Ofertas en<br /><i>market channels*</i></span></li>
                        </ul>                    </div>                    <div class="pricingColumn" id="pricingColumn_basic" style="float:left; padding-left:5px;">                        <h2>Básico</h2>                        <p class="price">$16<span>/mes</span></p><p class="price_6months">$89<span> /6 meses</span></p><p class="price_6months">$170<span> /12 meses</span></p>
                        <span class="pricingBadge">&nbsp;</span>                        <ul>                            <li><span>1 - 30</span></li>                            <li class="pricingColumnsStore">Store Features</li>                            <li><br /><div class="not_offered_normal_basic"><img src="../../Content/Images/600px-no-symbol-svg.png" width="15px" height="15px" style="margin-top:8px" /></div></li>                            <li class="pricingColumnsStore">Store Features</li>                            <li><br /><div class="not_offered_market_channel_basic"><img src="../../Content/Images/600px-no-symbol-svg.png" width="15px" height="15px" /></div></li>
                        </ul>                    </div>                    <div class="pricingColumn" id="pricingColumn_plus" style="float:left; padding-left:5px;">                        <h2>Standard</h2>
                        <p class="price">$28<span>/mes</span></p><p class="price_6months">$155<span> /6 meses</span></p><p class="price_12months">$297<span> /12 meses</span></p>                        <span class="pricingBadge">Popular</span>                        <ul>                            <li><span>31 - 100</span></li>                            <li class="pricingColumnsStore">Store Features</li>                            <li><br /><div class="offered_normal_standard"><img src="../../Content/Images/ok-yes-hook-icone-9585-128.png" width="20px" height="20px" /></div></li>                            <li class="pricingColumnsStore">Store Features</li>                            <li><br /><div class="not_offered_market_channel_standard"><img src="../../Content/Images/600px-no-symbol-svg.png" width="15px" height="15px" /></div><br /></li>                        </ul>                    </div>                    <div class="pricingColumn" id="pricingColumn_advanced" style="float:left; padding-left:5px;">                        <h2>Avanzado</h2>                        <p class="price">$48<span>/mes</span></p><p class="price_6months">$265<span> /6 meses</span></p><p class="price_12months">$508<span> /12 meses</span></p>                        <ul>                            <li><span>101 - 300</span></li>                            <li class="pricingColumnsStore">&nbsp;</li>                            <li><br /><div class="offered_normal_advanced"><img src="../../Content/Images/ok-yes-hook-icone-9585-128.png" width="20px" height="20px" /></div></li>                            <li class="pricingColumnsStore">Store Features</li>                            <li><br /><div class="offered_market_channel_advanced"><img src="../../Content/Images/ok-yes-hook-icone-9585-128.png" width="20px" height="20px" /></div></li>                        </ul>                    </div>                    <div class="pricingColumn" id="pricingColumn_pro" style="float:left; padding-left:5px;">                        <h2>Premium</h2>                        <p class="price">$64<span>/mes</span><p class="price_6months">$354<span> /6 meses</span><p class="price_12months">$676<span> /12 meses</span></p>                        <ul>                            <li><span>301+</span></li>                            
                            <li class="pricingColumnsStore">Store Features</li>                            <li><br /><div class="offered_normal_premium"><img src="../../Content/Images/ok-yes-hook-icone-9585-128.png" width="20px" height="20px" /></div></li>                            <li class="pricingColumnsStore">Store Features</li>                            <li><br /><div class="offered_market_channel_premium"><img src="../../Content/Images/ok-yes-hook-icone-9585-128.png" width="20px" height="20px" /></div></li>                        </ul>                    </div>                    </section>                </div>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;*Para saber los precios de las ofertas deslice el <i>mouse</i> por encima de la opción
            </div>
        </div>
    </div>
    <div class="textElement">
	    <div id="pageFooter">
		    © 2011 MVC E-commerce. All Rights Reserved. Created by <a class="contactus" href="/Contact" target="_self" title="Contactáctenos">eSphera</a>
	    </div>
    </div>
    <script type="text/javascript">
        $(".not_offered_normal_basic").simpletip({
            content: '<div style="position:left; width:190px;">Este plan no incluye ofertas. Sin embargo, en caso de querer desplegarlas, el costo es de $3.00 cada una.</div>',
            fixed: true,
            position: ["-280", "-200"],
            showEffect: 'slide',
            hideEffect: 'fade'
        });
        $(".not_offered_market_channel_basic").simpletip({
            content: '<div style="position:left; width:190px;">Este plan no incluye ofertas. Sin embargo, en caso de querer desplegarlas, el costo es de $4.00 cada una.</div>',
            fixed: true,
            position: ["-280", "-200"],
            showEffect: 'slide',
            hideEffect: 'fade'
        });
        $(".offered_normal_standard").simpletip({
            content: '<div style="position:left; width:250px;">Plan de 12 meses: 5 ofertas al mes - $2.70 x oferta</div>',
            fixed: true,
            position: ["-450", "-200"],
            showEffect: 'slide',
            hideEffect: 'fade'
        });
        $(".not_offered_market_channel_standard").simpletip({
            content: '<div style="position:left; width:190px;">Este plan no incluye ofertas. Sin embargo, en caso de querer desplegarlas, el costo es de $4.00 cada una.</div>',
            fixed: true,
            position: ["-450", "-200"],
            showEffect: 'slide',
            hideEffect: 'fade'
        });
        $(".offered_normal_advanced").simpletip({
            content: '<div style="position:left; width:260px;">Plan de 6 meses: 5 ofertas al mes - 2.80 x oferta<br/>Plan de 12 meses: 10 ofertas al mes - $2.70 x oferta</div>',
            fixed: true,
            position: ["-625", "-200"],
            showEffect: 'slide',
            hideEffect: 'fade'
        });
        $(".offered_market_channel_advanced").simpletip({
            content: '<div style="position:left; width:250px;">Plan de 12 meses: 5 ofertas al mes - $3.60 x oferta</div>',
            fixed: true,
            position: ["-625", "-200"],
            showEffect: 'slide',
            hideEffect: 'fade'
        });
        $(".offered_normal_premium").simpletip({
            content: '<div style="position:left; width:260px;">Plan de 6 meses: 10 ofertas al mes - $2.80 x oferta<br/>Plan de 12 meses: 15 ofertas al mes - $2.70 x oferta</div>',
            fixed: true,
            position: ["-800", "-200"],
            showEffect: 'slide',
            hideEffect: 'fade'
        });
        $(".offered_market_channel_premium").simpletip({
            content: '<div style="position:left; width:260px;">Plan de 6 meses: 5 ofertas al mes - $3.70 x oferta<br/>Plan de 12 meses: 10 ofertas al mes - $3.60 x oferta</div>',
            fixed: true,
            position: ["-800", "-200"],
            showEffect: 'slide',
            hideEffect: 'fade'
        });
    </script>
</body>
</html>
