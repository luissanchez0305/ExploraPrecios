<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.ClientViewModel>" %>
<%@ Import Namespace="Explora_Precios.ApplicationServices" %>
<%@ Import Namespace="Explora_Precios.Web.Controllers.Helpers" %>
<%  var moneyFormat = "#,###.00";
    var side = ViewData["LeftRight"].ToString();
    var position = int.Parse(ViewData["Position"].ToString()); 
    var priceStoreLeft = side == "Left" ? (position * 650) +  185 : (position * 650) + 510;
    var nameLeft = side == "Left" ? (position * 650) + 180 : (position * 650) + 505;
	var imageSize = Model.image.FitImage(110, 80); %>
    <%= Html.Hidden("hdn_productId", Model.productId) %>
<div class="offer<%= side %>">
    <img class="offerLogo" src="../../Content/Images/logo_oferta.gif" alt="Oferta" />
    &nbsp;<img class="offerImage" src="/ShowImage/?image=<%= Explora_Precios.ApplicationServices.CommonUtilities.CacheImage(Model.image) %>" alt="<%= Model.brandName + " - " + Model.productName  %>" width="<%= imageSize[0] %>" height="<%= imageSize[1] %>"/>
    <label class="offer<%= side %>Name" style="top:10px; left: <%= nameLeft.ToString() %>px;"><%= Model.productName.Shorten(18) %></label>
    <label class="offer<%= side %>Prices" style="top:30px; left: <%= priceStoreLeft.ToString() %>px;"><label class="offerOriginalPrice"><%= "$" + Model.price.ToString(moneyFormat) %></label> - <label class="offerSpecialPrice"><%= "$" + Model.specialPrice.ToString(moneyFormat) %></label></label>
    <label class="offer<%= side %>Store" style="top:50px; left: <%= priceStoreLeft.ToString() %>px;"><%= Model.clientName %></label>
</div>
