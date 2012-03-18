<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.ProductViewModel>" %>
<% var moneyFormat = "#,###.00"; %>
<div id="productDetails" class="details">
    <div id="BreadCrums">
        Producto en <b><%= Explora_Precios.Web.Controllers.Helpers.CatalogHelper.CatalogToAnchors(true, Model.catalogProduct)%></b>
    </div>
    <div class="productLeft">
        <img width="250" height="200" class="productImage" alt="<%= Model.productName %>" src="/ShowImage/?image=<%= new Explora_Precios.ApplicationServices.CommonUtilities().CacheImage(Model.productImage) %>" />
        <!-- TODO LISTA DE IMAGENES -->
        <div class="qualities">
            <label class="title">Caracteristicas</label>
            <table class="qualitiesTable">
                <% foreach (var quality in Model.qualities)
                   { %>
                <tr>
                    <td>
                        <%= quality.name %>
                    </td>
                    <td>
                        <%= quality.value %>
                    </td>
                </tr>
                <% } %>
            </table>
        </div>
        <a id="ProductDetailsClose" href="#">Close</a>
    </div>
    <div class="productRight">
        <div class="productTitle"><%--<h2><%= Model.productName %><br />(<%= Model.productRef %>)</h2>--%>
        <label>Desde 
            <label>$<%= Model.clientList.First().price.ToString(moneyFormat)%></label> hasta 
            <label>$<%= Model.clientList.Last().price.ToString(moneyFormat) %></label>
        </label>
        </div>
        <div class="productStores">
        Se encuentra en <b><%= Model.clientList.Count %></b> tienda(s):<br/>
        <% var first = true;
            foreach (var store in Model.clientList)
           { %>
            <div class="<%= (first ? "first" : "store") %>">
                <div class="storeName"><%= store.clientName %></div>
                <div class="price">$<%= store.price.ToString(moneyFormat) %></div>
                <% if (store.url.Length > 0) { %><div class="openProduct"><a href="<%= store.url %>" target="_blank">Ir a la pagina (<%= store.domain %>)</a></div> <%}%>
            </div>
        <% first = false;} %>
        </div>
    </div>
</div>
