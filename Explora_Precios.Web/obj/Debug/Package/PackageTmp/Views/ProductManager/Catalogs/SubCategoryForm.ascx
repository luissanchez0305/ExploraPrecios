<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Explora_Precios.Web.Controllers.ViewModels.SubCategoryViewModel>>" %>

    <form id="SubCategoryForm" onsubmit="return false;">
        <div class="item">
            <%  var index = 0; 
                foreach(var subCategory in Model){ %>
            <%= Html.TextBox("SubCategory[" + index + "].subCategoryTitle", subCategory.subCategoryTitle, new { @class = "w120" })%> 
            <a href="javascript:void(0)" class="Delete" style="float:right; padding-left:5px;">Borrar</a> <a href="javascript:void(0)" class="Select" style="float:right;">Seleccionar</a>
            <%= Html.Hidden("SubCategory[" + index.ToString() + "].subCategoryId", subCategory.subCategoryId)%>
            <%= Html.Hidden("SubCategoryLabel", "SubCategory")%><br />
            <% index++;
                } %>
            <input type="button" id="SaveSubCategory" value="Guardar" class="Save" />
        </div>
    </form>