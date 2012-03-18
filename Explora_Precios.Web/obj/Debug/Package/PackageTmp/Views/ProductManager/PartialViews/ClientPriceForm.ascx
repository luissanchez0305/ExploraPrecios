<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Explora_Precios.Web.Controllers.ViewModels.ClientViewModel>>" %>
<%@ Import Namespace="Explora_Precios.Web.Controllers.Helpers" %>
<b><label>Clients & Prices</label></b>
<table id="clientPriceTable">
<thead><tr><th>&nbsp;</th><th>Price</th><th>Url</th><th>Reference</th></tr></thead>
<tbody>
<% var clientIndex = 0;
 foreach (var client in Model)
   {   %>
<tr class="<%= (client.productStatus != Explora_Precios.ApplicationServices.ClientServices.ItemType.Possible_Related && !client.showMe) ? "hideMe" : "" %> <%= (client.productStatus == Explora_Precios.ApplicationServices.ClientServices.ItemType.Possible_Related) ? "possible" : "" %>">
    <td>
        <input type="hidden" id="clientList[<%= clientIndex %>].clientId" name="clientList[<%= clientIndex %>].clientId" value="<%= client.clientId %>" />
        <%= client.clientName %>
    </td>
    <td><% if (client.showMe)
           {%><%= Html.TextBox("clientList[" + clientIndex + "].price", client.price, new { @class = "price" })%><% }
           else {%> <%= Html.Hidden("clientList[" + clientIndex + "].price", client.price) %><%= client.price %><% } %></td>
    <td><% if (client.showMe)
           {%><%= Html.TextBox("clientList[" + clientIndex + "].url", client.url, new { @class = "url" })%><% }
           else {%><%= Html.Hidden("clientList[" + clientIndex + "].url", client.url)%><a href="<%= client.url %>" target="_blank"><%= client.url.Shorten(10) %></a><% } %></td>
    <td><% if (client.showMe)
           {%><%= Html.TextBox("clientList[" + clientIndex + "].reference", client.reference, new { @class = "reference" })%><% }
           else {%><%= Html.Hidden("clientList[" + clientIndex + "].reference", client.reference)%><%= client.reference %><%} %></td>     
    <td><% if (client.showMe)
           {%><%= Html.CheckBox("clientList[" + clientIndex.ToString() + "].isActive", client.isActive)%>
           <%= Html.Hidden("clientList[" + clientIndex + "].isMain", !(client.productStatus == Explora_Precios.ApplicationServices.ClientServices.ItemType.OnSite_NotOnDB), new { @class = "shownMainClientProduct" })%><%}
           else {%><%= Html.Hidden("clientList[" + clientIndex + "].isActive", client.isActive)%>
           <%= Html.Hidden("clientList[" + clientIndex + "].isMain", client.isMain)%>
           <input type="radio" name="mainGroup" id="clientList[<%= clientIndex %>].isMain" class="mainClientProduct" />
           <% } %>
    </td>
    <td><%= Html.Hidden("clientList[" + clientIndex.ToString() + "].productStatus", client.productStatus)%>
    <% var priceDisplay = "";            
        if (client.productStatus == Explora_Precios.ApplicationServices.ClientServices.ItemType.OnSite_NotOnDB)
        { priceDisplay = "<label style=\"font-weight:bold; color: Green;\">Nuevo</label><br />"; }
       else if (client.productStatus == Explora_Precios.ApplicationServices.ClientServices.ItemType.OnDB_NotOnSite)
        { priceDisplay = "<label style=\"font-weight:bold; color: Red;\">Borrado</label><br />"; }
       else if (client.productStatus == Explora_Precios.ApplicationServices.ClientServices.ItemType.Local)
        {
            priceDisplay = client.oldPrice > 0 ? "<label style=\"font-weight:bold;" + ((client.oldPrice != client.price) ? " color: Blue;" : "") + "\">Original: " + client.oldPrice + "</label>" : "" +
                              "<input type=\"hidden\" id=\"clientList[" + clientIndex + "].oldPrice\" name=\"clientList[" + clientIndex + "].oldPrice\" value=\"" + client.oldPrice.ToString("F2") + "\"><br />";
        }%>        
       <%= priceDisplay %>
       <%--<label style="font-weight:bold; color: Red;"> 
        <%= client.pageStatus == Explora_Precios.ApplicationServices.ClientServices.PageStatus.PageChanged ? "Sitio: Cambio" :
               client.pageStatus == Explora_Precios.ApplicationServices.ClientServices.PageStatus.PageNotSet ? "Sitio: N/D" : ""%></label>--%>
       <% if (client.specialPrice > 0)
          {%><br /><label style="font-weight:bold;"> Oferta: $<%= client.specialPrice.ToString("F2") %></label><%= Html.Hidden("clientList["+clientIndex+"].specialPrice", client.specialPrice) %><%} %>
    </td>
</tr>
<% clientIndex++;
   } // end foreach %>
</tbody>
<tfoot>
<tr>
    <td>
    <select id="Clients" name="Clients">
        <% foreach(var client in new SharpArch.Data.NHibernate.Repository<Explora_Precios.Core.Client>().GetAll()){ %>
        <option value="<%= client.Id %>"><%= client.name %></option>    
        <% } %>
    </select>
    </td>
    <td>
        <input type="text" id="clientPrice" class="price" />
    </td>
    <td>
        <input type="text" id="clientProductUrl" class="url"/>
    </td>
    <td>
        <input type="text" id="clientProductReference" class="reference"/>
    </td>
    <td><a href="javascript:void(0);" id="assignClientPrice">Asignar</a></td>
</tr>
</tfoot>
</table>