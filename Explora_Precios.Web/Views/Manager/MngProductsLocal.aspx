<%@ Page Language="C#" MasterPageFile="~/Views/Shared/SiteMng.Master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage<Explora_Precios.Web.Controllers.ViewModels.CatalogViewModel>" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
Exploraprecios.com - Manejo de productos y catalogos - Manejo de Catalogos de Clientes
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<h2><a href="/Manager">Manejo de productos y catalogos</a> - Manejo de Catalogos de Clientes</h2>
<table>
    <tr style="vertical-align:top;">
        <td><ul class="menulist" id="listMenuRoot">
            <% 
                var subMenusCount = 0; 
                foreach (var department in Model.departments)
               {
                   var categories = department.categories.Where(cat => cat.departmentId == department.departmentId); %>
                <li>
                    <a href="#" class="catalogLink">
                    <%= department.departmentTitle %>
                    </a>  
                    <input type="hidden" class="level" value="0" />
                    <input type="hidden" class="catalog" value="<%= department.departmentId %>" />
                    <input type="hidden" class="title" value="<%= department.departmentTitle %>" />
                    <% if (categories.Count() > 0)
                       {
                           subMenusCount++;%>
                    <ul>
                        <% foreach (var category in categories)
                           {
                               var subCategories = category.subCategories.Where(subCat => subCat.categoryId == category.categoryId);
                               %>
                        <li>
                            <a href="#" class="catalogLink">
                                <%= category.categoryTitle %>
                            </a>
                            <input type="hidden" class="level" value="1" />
                            <input type="hidden" class="catalog" value="<%= category.categoryId %>" />
                            <input type="hidden" class="title" value="<%= department.departmentTitle %> - <%= category.categoryTitle %>" />
                            <% if (subCategories.Count() > 0)
                               {
                                   subMenusCount++;%>
                                <ul>
                                <% foreach (var subCategory in subCategories)
                                   {
                                       var productTypes = subCategory.productTypes.Where(prodType => prodType.subCategoryId == subCategory.subCategoryId); %>
                                    <li>
                                        <a href="#" class="catalogLink">
                                            <%= subCategory.subCategoryTitle %>
                                        </a>
                                        <input type="hidden" class="level" value="2" />
                                        <input type="hidden" class="catalog" value="<%= subCategory.subCategoryId %>" />
                                        <input type="hidden" class="title" value="<%= department.departmentTitle %> - <%= category.categoryTitle %> - <%= subCategory.subCategoryTitle %>" />
                                        <% if (productTypes.Count() > 0)
                                           {
                                               subMenusCount++;%>
                                        <ul>
                                        <% foreach (var productType in productTypes)
                                            { %>
                                            <li>
                                                <a href="#" class="catalogLink"><%= productType.productTypeTitle %></a>
                                                <input type="hidden" class="level" value="3" />
                                                <input type="hidden" class="catalog" value="<%= productType.productTypeId %>" />
                                                <input type="hidden" class="title" value="<%= department.departmentTitle %> - <%= category.categoryTitle %> - <%= subCategory.subCategoryTitle %> - <%= productType.productTypeTitle %>" />
                                            </li>
                                        <% } %>
                                        </ul>
                                               <%} %>
                                    </li>
                                                   
                                <%} %>
                                </ul>
                               <%} %>
                        </li>
                        <%} %>
                    </ul>
                    <%} %>
                </li>
                   <%
               } %>
            </ul>
        </td>
        <td>
            <img id="product_Loading" src="/content/images/loading_big.gif" alt="Loading" /><label id="loading_productTitle"></label>
            <div id="productDisplay"></div>
        </td>
    </tr>
</table>
<script type="text/javascript">
    var listMenu = new FSMenu('listMenu', true, 'display', 'block', 'none');
    listMenu.showDelay = 2;
    listMenu.animations[listMenu.animations.length] = FSMenu.animFade;
    listMenu.animations[listMenu.animations.length] = FSMenu.animSwipeDown;
    var arrow = null;
    if (document.createElement && document.documentElement) {
        arrow = document.createElement('span');
        arrow.appendChild(document.createTextNode('>'));
        arrow.className = 'subind';
    }
    addReadyEvent(new Function('listMenu.activateMenu("listMenuRoot", arrow)'));
    $(document).ready(function () {
        $("#product_Loading").hide();
        $("#loading_productTitle").hide();
    });
    $('.catalogLink').click(function () {
        var $this = $(this);
        var _level = $this.next().val();
        var _catalog = $this.next().next().val();
        var title = $this.next().next().next().val();
        $("#loading_productTitle").html('<b>Cargando</b> ' + title);
        $("#loading_productTitle").show();
        $("#product_Loading").show();
        $.get('<%= Url.Action("GetProducts", "Manager") %>', { level: _level, catalog: _catalog },
        function (data) {
            $("#product_Loading").hide();
            $("#loading_productTitle").html('<h3>'+title+'</h3>');
            if (data.result == "success") {
                $("#productDisplay").html(data.html);
                $("#nextVal").val("1");
                $("#navPrev").addClass("disabled");
                if (data.total == 1)
                    $("#navNext").addClass("disabled");
                else
                    $(".pageTitle").html("Pagina 1 de " + data.total);
            }
            else {
                $("#productDisplay").html("<b style='color:red;'>No existen productos para esta categoria</b><br/>");
                $("#navProduct").hide();
            }
        });
    });

    /* Navegacion de productos */
    $(".nav").live("click", function () {
        var $this = $(this);
        if ($this.hasClass("disabled")) return;
        $("#product_Page_Loading").show();

        var _page = $this.next().val();
        PageLoader(_page);
    });

    $("#pageJumpButton").live("click", function () {
        $("#product_Page_Loading").show();
        var $this = $(this);
        var _page = $this.next().val();

        PageLoader(_page - 1);
    });

    function PageLoader(_page) {
        $.ajax({
            data: { page: _page },
            url: '<%= Url.Action("GetProductPage", "Manager") %>',
            dataType: "json",
            error: function (e) {
                $("#product_Page_Loading").hide();
                alert("Error: corriendo url " + e);
            },
            success: function (jObject) {
                $("#product_Loading").hide();
                $("#product_Page_Loading").hide();
                if (jObject.result == "success") {
                    $("#navProduct").show();
                    $("#productDisplay").html(jObject.html);
                    $("#prevVal").val(parseInt(_page) - 1);
                    $("#nextVal").val(parseInt(_page) + 1);
                    $(".pageTitle").html("Pagina " + (parseInt(_page) + 1) + " de " + jObject.total);
                    if (_page >= 1)
                        $("#navPrev").removeClass("disabled");
                    else
                        $("#navPrev").addClass("disabled");
                    if (_page == jObject.total - 1)
                        $("#navNext").addClass("disabled");
                    else
                        $("#navNext").removeClass("disabled");
                }
                else if (jObject.result == "empty") {
                    var emptyListHtml = "<b style='color:red;'>No existen productos para esta categoria</b>";
                    $("#product").html(emptyListHtml);
                    $("#navProduct").hide();

                }
                else if (jObject.result == "fail") {
                    alert("Hubo un error - " + jObject.msg);
                }
            }
        });
    }
</script>
</asp:Content>
