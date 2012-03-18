<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Explora_Precios.Web.Controllers.ViewModels.DepartmentViewModel>>" %>

    <form id="DepartmentForm" onsubmit="return false;">
        <div class="item">
            <%  var index = 0; 
                foreach(var department in Model){ %>
            <%= Html.TextBox("Department[" + index + "].departmentTitle", department.departmentTitle, new { @class = "w120"}) %>
            <a href="javascript:void(0)" class="Delete" style="float:right; padding-left:5px;">Borrar</a> <a href="javascript:void(0)" class="Select" style="float:right;">Seleccionar</a> 
            <%= Html.Hidden("Department[" + index.ToString() + "].departmentId", department.departmentId)%>
            <%= Html.Hidden("DepartmentLabel","Department") %><br />
            <% index++;
                } %>
            <input type="button" id="SaveDepartment" value="Guardar" class="Save" />
        </div>
    </form>
