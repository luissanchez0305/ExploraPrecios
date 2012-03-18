<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Explora_Precios.Web.Controllers.ViewModels.CategoryViewModel>>" %>

    <form id="CategoryForm" onsubmit="return false;">
        <div class="item">
            <%  var index = 0; 
                foreach(var category in Model){ %>
            <%= Html.TextBox("Category[" + index + "].categoryTitle", category.categoryTitle, new { @class = "w120" })%> 
            <a href="javascript:void(0)" class="Delete" style="float:right; padding-left:5px;">Borrar</a> <a href="javascript:void(0)" class="Select" style="float:right;">Seleccionar</a>
            <%= Html.Hidden("Category[" + index.ToString() + "].categoryId", category.categoryId)%>
            <%= Html.Hidden("CategoryLabel", "Category")%><br />
            <% index++;
                } %>
            <input type="button" id="SaveCategory" value="Guardar" class="Save" />
        </div>
    </form>
