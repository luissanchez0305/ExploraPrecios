<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.CatalogViewModel>" %>

<!-- Departmentos -->
<div id="DepartmentDiv">
<p>
    <label>Departamentos: </label>
    <select id="Departments" name="Departments" style="width: 200px;" class="ddlchange">
        <% foreach (var item in Model.departments)
           {
               if (item.departmentId == Model.departmentId)
               { 
            %>
                <option value="<%= item.departmentId %>" selected="selected"><%= item.departmentTitle%></option><%}%>
            <%else
            {%>
                <option value="<%= item.departmentId %>"><%= item.departmentTitle%></option>
            <% } %>
        <% } %>
    </select>
    <input type="hidden" id="levelId_department" value="0" />
</p>
</div>
<!-- Categorias -->
<div id="CategoryDiv"></div>
<p>
    <label>Categorias: </label>
    <select id="Categories" <% if (Model.departmentId == 0) { %> disabled <% } %> class="ddlchange">
        <% if (Model.departmentId != 0)
           { %>
            <% foreach (var item in Model.categories)
               {
                   if (Model.categoryId == item.categoryId)
                   {%>
                        <option value="<%= item.categoryId %>" selected="selected"><%= item.categoryTitle%></option>
                    <%}
                   else
                   {%>
                        <option value="<%= item.categoryId %>"><%= item.categoryTitle%></option>
                    <% } %>
            <% } %>
        <% } %>
    </select>
    <input type="hidden" id="levelId_category" value="1" />
</p>
<!-- SubCategorias -->
<div id="SubCategoryDiv"></div>
<p>
    <label>Subcategorias: </label>
    <select id="SubCategories" <% if (Model.categoryId == 0) { %> disabled <% } %> class="ddlchange">
        <% if (Model.categoryId != 0)
           { %>
            <% foreach (var item in Model.subCategories)
               {
                   if (Model.subCategoryId == item.subCategoryId)
                   {%>
                        <option value="<%= item.subCategoryId %>" selected="selected"><%= item.subCategoryTitle%></option>
                    <%}
                   else
                   {%>
                        <option value="<%= item.subCategoryId %>"><%= item.subCategoryTitle%></option>
                    <% } %>
            <% } %>
        <% } %>
        </select>
    <input type="hidden" id="levelId_subCategory" value="2" />
</p>
<!-- Tipo de productos -->
<div id="ProductTypeDiv"></div>
<p>
    <label>Tipo de productos:
    </label>
    <select id="ProductTypes" <% if (Model.subCategoryId == 0) { %> disabled <% } %> class="ddlchange">
        <% if (Model.subCategoryId != 0)
           { %>
            <% foreach (var item in Model.productTypes)
               {
                   if (Model.productTypeId == item.productTypeId)
                   {%>
                        <option value="<%= item.productTypeId %>" selected="selected"><%= item.productTypeTitle%></option>
                    <%}
                   else
                   {%>
                        <option value="<%= item.productTypeId %>"><%= item.productTypeTitle%></option>
                    <% } %>
            <% } %>
        <% } %>
        </select>        
    <input type="hidden" id="levelId_productType" value="3" />
</p>    

<script type="text/javascript">
    $(document).ready(function () {
        $(".ddlchange").change(function () {

            // This IF statement is only for the product management
            if ($("#catalogLevel")) {
                $("#catalogLevel").val($(this).next().val());
                $("#catalogId").val($(this).val());
            }
            // This IF statement is only for the catalog management
            if ($(this).next().val() == 0 && $(this).parent().parent().parent().prev().attr("id").indexOf("CatalogList") > -1) {
                $(this).parent().parent().parent().prev().val($(this).val()); // Modify the catalog
                $(this).parent().parent().parent().prev().prev().val($(this).next().val()); // Modify the level
            }
            else if ($(this).next().val() != 0 && $(this).parent().parent().prev().attr("id").indexOf("CatalogList") > -1) {
                $(this).parent().parent().prev().val($(this).val()); // Modify the catalog
                $(this).parent().parent().prev().prev().val($(this).next().val()); // Modify the level                
            }
        });
        $("select#Departments").change(function () {
            var source_singular = "Department";
            var source_plural = "Departments";
            var target_singular = "Category";
            var target_plural = "Categories";
            $("#Categories").attr('disabled', true).html('');
            $("#SubCategories").attr('disabled', true).html('');
            $("#ProductTypes").attr('disabled', true).html('');
            $('#CategoryDiv > div').remove(); // Remove any existing Category
            $('#SubCategoryDiv > div').remove(); // Remove any existing SubCategory
            $('#ProductTypeDiv > div').remove(); // Remove any existing ProductType
            $('#catId').val('0');
            $('#subCatId').val('0');
            $('#prodTypeId').val('0');

            runDropDownAjax(source_singular, source_plural, target_singular, target_plural);
        });

        $("select#Categories").change(function () {
            var source_singular = "Category";
            var source_plural = "Categories";
            var target_singular = "SubCategory";
            var target_plural = "SubCategories";
            $("#SubCategories").attr('disabled', true).html('');
            $("#ProductTypes").attr('disabled', true).html('');
            $('#SubCategoryDiv > div').remove(); // Remove any existing SubCategory
            $('#ProductTypeDiv > div').remove(); // Remove any existing ProductType
            $('#subCatId').val('0');
            $('#prodTypeId').val('0');

            runDropDownAjax(source_singular, source_plural, target_singular, target_plural);
        });

        $("select#SubCategories").change(function () {
            var source_singular = "SubCategory";
            var source_plural = "SubCategories";
            var target_singular = "ProductType";
            var target_plural = "ProductTypes";
            $("#ProductTypes").attr('disabled', true).html('');
            $('#ProductTypeDiv > div').remove(); // Remove any existing ProductType
            $('#prodTypeId').val('0');

            runDropDownAjax(source_singular, source_plural, target_singular, target_plural);
        });
    });
</script>


