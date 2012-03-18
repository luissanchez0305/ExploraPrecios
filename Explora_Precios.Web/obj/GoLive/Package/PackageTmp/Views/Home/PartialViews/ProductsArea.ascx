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
<!-- BEGIN Productos de flotantes -->
<div class="nav_floating_area roundedBorder">
    <div id="nav_floating_buttons">
        <input type="button" value="Ofertas" id="btn_onsale_floating" class="formbutton" />&nbsp;
        <input type="button" value="Nuevos" id="btn_new_floating" class="formbutton" />&nbsp;
        <img alt="Loading..." id="img_Floating_Loading" src="../../Content/Images/loading_big.gif" class="smallImg" style="display:none; vertical-align:bottom;" />
    </div>
    <div id="grid_floating" class="grid" style="padding-top:15px; padding-bottom:10px;"></div>
    <div class="nav_floating nav_floating_bottom hand">Cerrar <img src="../../Content/Images/hide.png" alt="Cerrar" class="nav_floating_hide" /> </div>
</div>
<!-- END Productos de flotantes -->

<!-- LISTA DE PRODUCTOS -->
<% var IsGridEmbedded = Request.Url.PathAndQuery.Length > 1;%>
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
        <div id="gridTopic_grid" class="gridview" onmouseover="displaySwitch('grid')" onmouseout="displaySwitch('grid')"
            onclick="changeDisplay(this, 'grid')">
        </div>
        <div id="gridTopic_list" class="listview" onmouseover="displaySwitch('list')" onmouseout="displaySwitch('list')"
            onclick="changeDisplay(this, 'grid')">
        </div>
    </div>
</div>
<% if(IsGridEmbedded){ %>
<div id="grid" class="grid">
    <% Html.RenderPartial("PartialViews/ProductsList", Model.productsListViewModel); %>
</div>
<%}
   else {%> <% Html.RenderPartial("PartialViews/ProductsList_Initial", Model.productsListViewModel); %><% } %>
