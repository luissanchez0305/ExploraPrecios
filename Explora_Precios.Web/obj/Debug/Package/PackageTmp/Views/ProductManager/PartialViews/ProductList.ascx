<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Explora_Precios.Web.Controllers.ViewModels.ClientViewModel>>" %>
<label>Clients & Prices</label>
<table id="clientPriceTable">
<tbody>
<% var clientIndex = 0;
 foreach (var client in Model.Where(x => x.price > 0))
   {   %>
<tr>
    <td>
        <input type="hidden" id="clientList[<%= clientIndex %>].clientId" name="clientList[<%= clientIndex %>].clientId" value="<%= client.clientId %>" />
        <%= client.clientName %>
    </td>
    <td>
        <input type="text" id="clientList[<%= clientIndex %>].price" name="clientList[<%= clientIndex %>].price" value="<%= client.price %>" />
    </td>
    <td>
        <input type="text" id="clientList[<%= clientIndex %>].url" name="clientList[<%= clientIndex %>].url" value="<%= client.url %>" />
    </td>
    <td>
        <%= Html.CheckBox("clientList[" + clientIndex.ToString() + "].isActive", client.isActive) %>
    </td>
</tr>
<% clientIndex++;
   } // end foreach %>
</tbody>
</table>

<table>
<tr>
    <td>
    <select id="Clients" name="Clients">
        <% foreach(var client in Model){ %>
        <option value="<%= client.clientId %>"><%= client.clientName %></option>    
        <% } %>
    </select>
    </td>
    <td>
        <input type="text" id="clientPrice" />
    </td>
    <td>
        <input type="text" id="clientProductUrl" />
    </td>
    <td><a href="#" id="assignClientPrice">Asignar</a></td>
</tr>
</table>