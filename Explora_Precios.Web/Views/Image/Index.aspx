<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Explora_Precios.Web.Controllers.ViewModels.ProductViewModel>" %>

<img width="250" height="200" class="productImage" alt="<%= Model.productName %>" src="/ShowImage/?image=<%= Explora_Precios.ApplicationServices.CommonUtilities.CacheImage(Model.productImage) %>" />
