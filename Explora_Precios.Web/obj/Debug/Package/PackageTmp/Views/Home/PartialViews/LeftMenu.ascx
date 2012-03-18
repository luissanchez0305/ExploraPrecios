<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.HomeViewModel>" %>
<script type="text/javascript">
$(document).ready(function(){
    $(".department").val(<%= Model.departmentId %>);
});
</script>
    <h3>Catalogo de <%= Model.departmentTitle %></h3>
        <%= Html.Hidden("departmentId", Model.departmentId, new { @class = "department" })%>
    <div class="suckerdiv">
        <ul id="suckertree1">
            <% foreach (var category in Model.categories)
       { %>
            <li><a href="/Home/Products?catlev=1&id=<%= category.categoryId %>">
                <%= category.categoryTitle %></a>
                <% if (category.subCategories.Count > 0)
                   { %>
                        <ul>
                         <% foreach (var subcategory in category.subCategories)
                            { %>
                                <li><a href="/Home/Products?catlev=2&id=<%= subcategory.subCategoryId %>"><%= subcategory.subCategoryTitle %></a>
                                <% if (subcategory.productTypes.Count > 0)
                                   { %>
                                        <ul>
                                        <% foreach (var producttype in subcategory.productTypes)
                                           { %>
                                                <li><a href="/Home/Products?catlev=3&id=<%= producttype.productTypeId %>"><%= producttype.productTypeTitle %></a></li>
                                        <% } %>
                                        </ul>
                                <%} %></li>
                         <% } %>
                        </ul>
                <% 
                   }  %></li>
            <% 
       } %>
        </ul>
    </div>
<%--    <h3>
        <a href="#">Section 2</a></h3>
    <div>
        <p>
            Sed non urna. Donec et ante. Phasellus eu ligula. Vestibulum sit amet purus. Vivamus
            hendrerit, dolor at aliquet laoreet, mauris turpis porttitor velit, faucibus interdum
            tellus libero ac justo. Vivamus non quam. In suscipit faucibus urna.
        </p>
    </div>--%>
    <%  // obtenemos el menor nivel del catalogo
        var catalogLevel = Model.catalog.categoryId == 0 ? 0 : Model.catalog.subCategoryId == 0 ? 1 : Model.catalog.productTypeId == 0 ? 2 : 3;
        // obtenemos el id del menor nivel del catalogo
        var catalogLevelId = Model.catalog.categoryId == 0 ? Model.catalog.departmentId : Model.catalog.subCategoryId == 0 ? Model.catalog.categoryId : Model.catalog.productTypeId == 0 ? Model.catalog.subCategoryId : Model.catalog.productTypeId;
        var filterItems = Explora_Precios.Web.Controllers.Helpers.CatalogHelper.FilterLoad(Model.allProducts, Model.isSearch, Model.isSearch ? ViewData["search_text"].ToString() + "," + Model.departmentId.ToString() : catalogLevel.ToString() + "," + catalogLevelId.ToString());%>
<div <%= (filterItems.Count > 1 || !string.IsNullOrEmpty(Model.filterBackUrl) ? "class=\"separator\"" : "") %>>
    <%if(!string.IsNullOrEmpty(Model.filterBackUrl)){ %>
    <h4>
        <a href="<%= Model.filterBackUrl %>">Deshacer filtro</a><%--<img src="../../content/images/close.png" alt="close"/>--%></h4>
    <%} %>
    <% foreach (var filter in filterItems)
       {%>
        <h3><%= filter.value %></h3>
        <div class="suckerdiv">
        <ul class="suckertree1">
            <% foreach (var filterItem in filter.items.Where(filterItem => filterItem.name.Length > 0))
               { %>
                <li><a <% if(filterItem.url.Contains("f=o")) { %>style="background-color:#F21400; color:#F4EFCF;"<% } %> class="filter" href="<%= filterItem.url %>"><%= filterItem.name%></a></li>
            <%} %>
        </ul>
        </div>
    <% } %>
</div>
<%--<ul class="f12">
    <% foreach (var category in Model.categories)
   { %>
    <li><a href="?catlev=1&id=<%= category.categoryId %>">
        <%= category.categoryTitle %></a></li>
    <% } %>
</ul>--%>

