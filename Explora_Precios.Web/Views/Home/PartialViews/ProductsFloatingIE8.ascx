<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.ProductsListViewModel>" %>

<!-- BEGIN Productos de flotantes -->
    <div id="nav_floating_titleIE8" style="font-size: 15px; font-weight: bold; font-family:Arial;">
        <label><%= Request.Url.AbsoluteUri.Contains("new") ? "Productos Nuevos" : "Productos en oferta" %></label>
    </div>
    <div id="grid_floatingIE8" class="grid" style="padding-top:15px; padding-bottom:10px;">
    <% Html.RenderPartial("PartialViews/ProductsList", Model); %>
    </div>
<!-- END Productos de flotantes -->