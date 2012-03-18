<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.ProductsListViewModel>" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="System.ComponentModel"%>
<%@ Import Namespace="MvcContrib.UI.Grid" %>
<%@ Import Namespace="MvcContrib.UI.Grid.ActionSyntax" %>
<%@ Import Namespace="MvcContrib.UI.Pager" %>
<%@ Import Namespace="Explora_Precios.Core" %>
<%@ Import Namespace="Explora_Precios.Core.Helper" %>
<%@ Import Namespace="Explora_Precios.Web" %>
<%@ Import Namespace="Explora_Precios.Web.Controllers" %>
<%@ Import Namespace="Explora_Precios.Web.Controllers.Helpers" %>
<% var moneyFormat = "#,###.00"; %>

<% var isFloating = Request.Url.AbsolutePath.Contains("GetFloatingProducts");
   var isIE8 = Request.Url.AbsoluteUri.Contains("ie8"); %>

<% if (Model.products.Count > 0)
           { %>
   <% foreach (var product in Model.productsList)
               {
           var specialOffer = product.clientList.SingleOrDefault(client => client.specialPrice > 0) != null ?
               product.clientList.Where(client => client.specialPrice > 0).OrderBy(orderPrice => orderPrice.specialPrice).First().specialPrice.ToString(moneyFormat) : "0";
           %>
    <%= Html.Hidden("productId_" + product.productId, product.productId) %>
    <div id="<%= product.productRef.Replace(" ","_") %>" class="gridItem <%= isIE8 ? "nogo" : "" %>" onmouseover="enlarge(this);" onmouseout="enlarge(this);" >
        <div class="frame">
            <img width="125" height="100" alt="<%= product.productName %>" src="/ShowImage/?image=<%= new Explora_Precios.ApplicationServices.CommonUtilities().CacheImage(product.productImage) %>" />
            <% if (specialOffer != "0")
               {%>
            <img src="../../Content/Images/etiqueta_oferta.gif" alt="Oferta" width="30px" height="35px" class="offerImage" /><%} %>
        </div>
        <div class="content">
            <div class="brand">
                <%= product.productBrand%></div>
            <% if (ViewData["isSearch"] != null && bool.Parse(ViewData["isSearch"].ToString()))
               {
                   var catalog_anchors = Explora_Precios.Web.Controllers.Helpers.CatalogHelper.CatalogToString(true, product.catalogProduct, true); %>
                   <div class="catalog">
                               <%= catalog_anchors%></div>
            <%}//end if  %>
            <div class="price">
                $<%= specialOffer != "0" ? specialOffer : product.clientList.OrderBy(x => x.price).First().price.ToString(moneyFormat)%></div>
            <div class="stores">
                <%= product.clientList.Count%> Tienda(s)</div>
            <div class="name">
                <a href="#">
                    <%= product.productName.Shorten(40) %></a></div>
            <div class="desc">
                <!--<%= product.productDescription%>-->
                <b>Caracteristicas</b>
                <table class="qualitiesTable">
                    <% var qualityIndex = 0;
                     foreach (var quality in product.qualities)
                     {
                         if (qualityIndex < 4)
                         {
                             %>
                    <tr>
                        <td>
                            <%= quality.name.Trim()%>
                        </td>
                        <td>
                            <%= quality.value.Trim()%>
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
    <div class="pager <%= isFloating ? "ajaxArea" : "" %>">
        <%= Html.Pager(ViewData.Model.products.PageSize, ViewData.Model.products.PageNumber,
                        ViewData.Model.products.TotalItemCount, Request.Url.Query.Replace("?","").Split('&'))%>
                        <%} %>
    </div>   
    <% } 
           else
           {%>
    No encontramos ningun producto con esa caracteristica<br />
    Por favor, intente nuevamente con otra opcion
    <%} // end if %>