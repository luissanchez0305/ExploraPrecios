<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.ProductViewModel>" %>

<div class="contentLeft">
    <h3>Detalles de producto</h3>
</div>
<div class="contentRight">
    <h3>Informacion de clientes y catalogo</h3>
</div>  
<div class="contentLeft">
    <%= Html.Hidden("isNewProduct", Model.isNewProduct) %>
    <%= Html.Hidden("catalogLevel", Model.catalogLevel) %>
    <%= Html.Hidden("catalogId", Model.catalogId) %>
    <input type="hidden" id="catalog.productId" name="productId"
     value="<%= Model.productId %>" />
    <br />
    # de Referencia:
    <input type="text" id="productRef" name="productRef"
        value="<%= Model.productRef %>" /><% if (Model.isNewProduct){ %><label class="newProduct">New</label><%}%>
    <br />
    Nombre: 
    <input type="text" id="productName" name="productName"
        value="<%= Model.productName %>" />
    <br />
    Marca: <input type="text" id="productBrand" name="productBrand"
        value="<%= Model.productBrand %>" /><br />
        
    URL de Imagen: <input type="text" id="productImageUrl" name="productImageUrl" value="<%= Model.productImageUrl %>" />
    <img src="<%= Model.productImageUrl %>" alt="image" height="50%" width="50%" /><br />    
    Descripcion:    
    <textarea cols="30" rows="6" id="productDescription" 
    name="productDescription" value="<%= Model.productDescription %>"></textarea><br />
    Nueva caracteristica:<br />
    <input type="text" id="qualityName" style="width:80px;" /> 
    <input type="text" id="qualityValue" /> 
    <a id="addQuality">Agregar</a><br /><br />
    Caracteristicas: <br />
    <table id="tableQualities">
        <tbody> 
    <% if (Model.qualities.Count > 0)
       {  
        var index2 = 0;
        foreach (var quality in Model.qualities)
        {
           %>
    <tr>
        <td>
<%--            <% if (quality.name.Length == 0)
               { %> 
            <select id="ddl_Quality" name="ddl_Quality" style="width: 200px;" class="ddlqualitychange">
                <option value="no" selected="selected">Seleccione uno...</option>
                <% foreach (var name in Model.qualityNames)
                   { %>
                    <option value="<%= name %>"><%= name%></option>
                <% } %>
                <option value="new">Ingrese nuevo</option>
            </select>
               <%
        } %>--%>
        <input type="hidden" value="<%= quality.Id %>" name="qualities[<%= index2 %>].Id" id=qualities[<%= index2 %>].Id"" />
            <input type="text" value="<%= quality.name %>" name="qualities[<%= index2 %>].name"
                id="qualities[<%= index2 %>].name"   />
        </td>
        <td>
            <input type="text" id="qualities[<%= index2 %>].value" name="qualities[<%= index2 %>].value"
                value="<%=  quality.value %>" /></td>
        <td> 
            <%= Html.CheckBox("qualities[" + index2 + "].active", quality.active) %>
        </td>
    </tr>
    <% index2++;
        } %>
       <%}
       else { %> <tr id="noQualities"><td><b>Sin caracteristicas</b></td></tr> <% } %>
            </tbody>
        </table>
</div>
<div class="contentRight">
<% Html.RenderPartial("ClientPriceForm", Model.clientList); %>
<% 
    Html.RenderPartial("ShowCatalogCascade", Model.catalogProduct); %>
</div>
            <input type="submit" id="btnSave" value="Save" />
