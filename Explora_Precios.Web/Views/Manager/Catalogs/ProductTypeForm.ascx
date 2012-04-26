<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Explora_Precios.Web.Controllers.ViewModels.ProductTypeViewModel>>" %>

    <form id="ProductTypeForm" onsubmit="return false;">
        <div class="item">
            <%  var index = 0; 
                foreach(var productType in Model){ %>
            <%= Html.TextBox("ProductType[" + index + "].productTypeTitle", productType.productTypeTitle, new { @class = "w120" })%> 
            <a href="javascript:void(0)" class="Delete">Borrar</a> <a href="#" style="display:none;"></a>
            <%= Html.Hidden("ProductType[" + index.ToString() + "].productTypeId", productType.productTypeId)%>
            <%= Html.Hidden("ProductTypeLabel", "ProductType")%><br />
            <% index++;
                } %>
            <input type="button" id="SaveProductType" value="Guardar" class="Save" />
        </div>
    </form>
