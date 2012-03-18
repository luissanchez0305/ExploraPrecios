<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.CatalogViewModel>" %>
Productos en <b><%= Explora_Precios.Web.Controllers.Helpers.CatalogHelper.CatalogToString(Request.Url.PathAndQuery.Contains("Filter") ? true : false, Model, true)%></b>
