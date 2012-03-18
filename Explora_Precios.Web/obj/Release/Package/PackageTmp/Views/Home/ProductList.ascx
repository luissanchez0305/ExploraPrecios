<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.HomeViewModel>" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="System.ComponentModel"%>
<%@ Import Namespace="MvcContrib.UI.Grid" %>
<%@ Import Namespace="MvcContrib.UI.Grid.ActionSyntax" %>
<%@ Import Namespace="MvcContrib.UI.Pager" %>
<%@ Import Namespace="Explora_Precios.Core" %>
<%@ Import Namespace="Explora_Precios.Core.Helper" %>
<%@ Import Namespace="Explora_Precios.Web" %>
<%@ Import Namespace="Explora_Precios.Web.Controllers" %>
<% var moneyFormat = "#,###.00"; %>
<!--<div id="banners" class="banners">
            <div class="banner">
                <img src="../../Content/images/Banner.gif" />
            </div>
            <div class="banner">
                <img src="../../Content/images/Banner_2.png" />
            </div>
            <div class="banner">
                <img src="../../Content/images/Banner_1.png" />
            </div>
            <ul>
                <li><a href="#0">ASP.NET MVC Design Competition</a></li>
                <li><a href="#1">SQL Server 2008 Express</a></li>
                <li><a href="#2">Visual Studio 2008</a></li>
            </ul>
        </div>-->
<div id="gridTopic" class="topic">
    <% if (Model.isSearch)
       { %>
    Resultado de la busqueda: <b><i>
        <%= ViewData["search_text"] %></i></b>
    <%}
       else
       { %>
    <% Html.RenderPartial("BreadCrumsMenu", Model.catalog); %>
    <%}//end if %>
    <div class="display">
        <div id="gridTopic_grid" class="gridview" onmouseover="displaySwitch('grid')" onmouseout="displaySwitch('grid')"
            onclick="changeDisplay(this, 'grid')">
        </div>
        <div id="gridTopic_list" class="listview" onmouseover="displaySwitch('list')" onmouseout="displaySwitch('list')"
            onclick="changeDisplay(this, 'grid')">
        </div>
    </div>
</div>
<!-- LISTA DE PRODUCTOS -->
<div id="grid" class="grid">
    <% if (Model.products.Count > 0)
           { %>
    <% foreach (var product in Model.productsViewModel)
               {%>
    <%= Html.Hidden("productId_" + product.productId, product.productId) %>
    <div id="<%= product.productRef %>" class="gridItem" onmouseover="enlarge(this);" onmouseout="enlarge(this);">
        <div class="frame">
            <img width="125" height="100" alt="<%= product.productName %>" src="/ShowImage/?image=<%= new Explora_Precios.ApplicationServices.CommonUtilities().CacheImage(product.productImage) %>" />
        </div>
        <div class="content">
            <div class="brand">
                <%= product.productBrand%></div>
            <% if (Model.isSearch)
               {
                   var catalog_anchors = Explora_Precios.Web.Controllers.Helpers.CatalogHelper.CatalogToAnchors(true, product.catalogProduct); %>
                   <div class="catalog">
                               <%= catalog_anchors%></div>
            <%}//end if  %>
            <div class="price">
                $<%= product.clientList.OrderBy(x => x.price).First().price.ToString(moneyFormat)%></div>
            <div class="stores">
                <%= product.clientList.Count%>
                Tienda(s)</div>
            <div class="name">
                <a href="#">
                    <%= product.productName%></a></div>
            <div class="desc">
                <!--<%= product.productDescription%>-->
                <b>Caracteristicas</b>
                <table class="qualitiesTable">
                    <% var qualityIndex = 0;
                     foreach (var quality in product.qualities)
                     {
                         if (qualityIndex < 4)
                         {%>
                    <tr>
                        <td>
                            <%= quality.name%>
                        </td>
                        <td>
                            <%= quality.value%>
                        </td>
                    </tr>
                    <% }
                         else
                         {%>
                         <tr align="right"><td colspan="2"><a href="#">mas caracteristicas...</a></td></tr>
                    <% 
                        break;
                         }// end if %>
                    
                    <% qualityIndex++;
                     } // end foreach %>
                </table>
            </div>
            
        </div>
    </div>
    <% } // end foreach %>
    <% if (ViewData.Model.products.TotalItemCount > ViewData.Model.products.PageSize)
       { %>
    <div class="pager">
        <%= Html.Pager(ViewData.Model.products.PageSize, ViewData.Model.products.PageNumber,
                        ViewData.Model.products.TotalItemCount)%>
                        <%} %>
    </div>   
    <% } 
           else
           {%>
    No encontramos ningun producto con esa caracteristica<br />
    Por favor, intente nuevamente con otra opcion
    <%} // end if %>
    <!--<div id="p_1" class="gridItem" onmouseover="enlarge(this);" onmouseout="enlarge(this);">
                <div class="frame">
                    <img src="../../Content/images/Xbox360.jpg" />
                </div>
                <div class="content">
                    <div class="price">$199.99</div>
                    <div class="stores">&nbsp;<%--<a href="#"><img src="../../Content/images/AddToCart.png" /></a>--%></div>
                    <div class="name"><a href="">Xbox 360 Arcade</a></div>
                    <div class="desc">The Xbox 360 Arcade console is everything you need to hit the ground running. Plug in the console and connect the wireless controller and you're playing.</div>
                </div>
            </div>
            <div id="p_2" class="gridItem" onmouseover="enlarge(this);" onmouseout="enlarge(this);">
                <div class="frame">
                    <img src="../../Content/images/Xbox360WirelessController.jpg" />
                </div>
                <div class="content">
                    <div class="price">$199.99</div>
                    <div class="stores">&nbsp;<%--<a href="#"><img src="../../Content/images/AddToCart.png" /></a>--%></div>
                    <div class="name"><a href="">Xbox 360 Wireless controller</a></div>
                    <div class="desc">High-performance wireless gaming now comes in black! Using optimized technology, the black Xbox 360 Wireless Controller lets you enjoy a 30-foot range and up to 40 hours of life on the two included AA batteries</div>
                </div>
            </div>
            <div id="p_3" class="gridItem" onmouseover="enlarge(this);" onmouseout="enlarge(this);">
                <div class="frame">
                    <img src="../../Content/images/Xbox360WheelController.jpg" />
                </div>
                <div class="content">
                    <div class="price">$199.99</div>
                    <div class="stores">&nbsp;<%--<a href="#"><img src="../../Content/images/AddToCart.png" /></a>--%></div>
                    <div class="name"><a href="">Xbox 360 Wireless Racing Wheel</a></div>
                    <div class="desc">Racing has never felt so real! Hold on tight as you hug corner after corner, skid through the sand, or trade paint with rival cars fighting for position—the wireless wheel* simulates all the resistance and force, immersing you in a relentless and unparalleled racing experience.</div>
                </div>
            </div>
            <div id="p_4" class="gridItem" onmouseover="enlarge(this);" onmouseout="enlarge(this);">
                <div class="frame">
                    <img src="../../Content/images/Xbox360Headset.jpg" />
                </div>
                <div class="content">
                    <div class="price">$199.99</div>
                    <div class="stores">&nbsp;<%--<a href="#"><img src="../../Content/images/AddToCart.png" /></a>--%></div>
                    <div class="name"><a href="">Xbox 360 Headset</a></div>
                    <div class="desc">The Xbox 360 Headset heightens the experience of the Xbox LIVE® online gaming community, allowing you to strategize with teammates, trash-talk opponents, or just chat with friends while playing your favorite games</div>
                </div>
            </div>
            <div  id="p_5" class="gridItem" onmouseover="enlarge(this);" onmouseout="enlarge(this);">
                <div class="frame">
                    <img src="../../Content/images/Xbox360MemoryUnit.jpg" />
                </div>
                <div class="content">
                    <div class="price">$199.99</div>
                    <div class="stores">&nbsp;<%--<a href="#"><img src="../../Content/images/AddToCart.png" /></a>--%></div>
                    <div class="name"><a href="">Xbox 360 Memory Unit (512MB)</a></div>
                    <div class="desc">Take your games everywhere you go with eight times the space of the original Xbox 360™ Memory Unit. With 512 MB of memory and a keychain carrying case</div>
                </div>
            </div>
            <div id="p_6" class="gridItem" onmouseover="enlarge(this);" onmouseout="enlarge(this);">
                <div class="frame">
                    <img src="../../Content/images/xbox360MessengerKit.jpg" />
                </div>
                <div class="content">
                    <div class="price">$199.99</div>
                    <div class="stores">&nbsp;<%--<a href="#"><img src="../../Content/images/AddToCart.png" /></a>--%></div>
                    <div class="name"><a href="">Xbox 360 Messenger Kit</a></div>
                    <div class="desc">Chatting with friends and family on Xbox LIVE® and Windows-based PCs is easy using the Xbox 360 Messenger Kit. </div>
                </div>
            </div>
            <div id="p_7" class="gridItem" onmouseover="enlarge(this);" onmouseout="enlarge(this);">
                <div class="frame">
                    <img src="../../Content/images/xbox360Halo3EditionConsole.jpg" />
                </div>
                <div class="content">
                    <div class="price">$199.99</div>
                    <div class="stores">
                        &nbsp;<%--<a href="#"><img src="../../Content/images/AddToCart.png" /></a>--%></div>
                    <div class="name"><a href="">Xbox 360 Halo 3 Special Edition Console</a></div>
                    <div class="desc">The Xbox 360™ Halo® 3 Special Edition Console features an exclusive "Spartan green and gold" finish and comes bundled with a matching Xbox 360 Wireless Controller, 20GB Hard Drive, Headset, Play & Charge Kit, and exclusive Halo 3 Gamer Pics</div>
                </div>
            </div>
            <div id="p_8" class="gridItem" onmouseover="enlarge(this);" onmouseout="enlarge(this);">
                <div class="frame">
                    <img src="../../Content/images/xbox360HardDrive.jpg" />
                </div>
                <div class="content">
                    <div class="price">$199.99</div>
                    <div class="stores">&nbsp;<%--<a href="#"><img src="../../Content/images/AddToCart.png" /></a>--%></div>
                    <div class="name"><a href="">Xbox 360™ Hard Drive (120 GB)</a></div>
                    <div class="desc">The Xbox 360 120GB Hard Drive is the best option for media enthusiasts who game on Xbox 360™. It is the largest storage option for Xbox 360. Expand your Xbox 360 experience with downloadable content</div>
                </div>
            </div>-->
</div>
