<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<string>>" %>

<ul id="TickerBanner">
<% foreach (var item in Model)
   {  %>
    <%= item %>
    <%} %>
</ul>