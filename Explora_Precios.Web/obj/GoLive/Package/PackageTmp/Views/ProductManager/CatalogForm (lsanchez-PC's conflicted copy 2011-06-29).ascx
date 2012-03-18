<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Explora_Precios.Web.Controllers.ViewModels.CatalogItemViewModel>>" %>

<% 
    var index = 0;
    foreach (var catalogItem in Model)
    { %>
    <div class="item">
        <img alt="Loading" id="loading_<%= catalogItem.type %>_<%= index %>" src="/content/images/loading_big.gif" width="20px" height="20px" />
        <%= Html.Hidden("CatalogList[" + index.ToString() + "].clientId", catalogItem.clientId)%>
        <%= Html.CheckBox("CatalogList[" + index.ToString() + "].selected", catalogItem.selected)%> 
        <a target="_blank" href="<%= "http://"+catalogItem.clientAddress+catalogItem.url %>"><%= catalogItem.name%> - <%= catalogItem.url%></a>
        <% if (catalogItem.type == "OnDB_NotOnSite")
           { %><a class="catalog_delete" style="float:right; padding-left:5px;" href="javascript:void(0)">Borrar</a>
           <% } %>
        <%= Html.Hidden("CatalogList[" + index.ToString() + "].dbId", catalogItem.dbId) %>
        <a href="javascript:void(0)" class="modifyCat_<%= catalogItem.type %>_<%= index %>" style="float:right;">Modificar catalogo</a>
        <%= Html.Hidden("hdn_index_" + index.ToString() + "_" + catalogItem.type, index)%>
        <%= Html.Hidden("hdn_type_" + index.ToString() + "_" + catalogItem.type, catalogItem.type)%>
        <%= Html.Hidden("CatalogList[" +index.ToString() +"].levelId", catalogItem.levelId) %>
        <%= Html.Hidden("CatalogList[" + index.ToString() + "].catalogId", catalogItem.catalogId)%>
        <div class="catalog" id="catalog_<%= catalogItem.type %>_<%= index %>">
        </div>
        <a href="javascript:void(0)" id="catalog_cancel_<%= catalogItem.type %>_<%= index %>" class="link">Cancelar</a><br />
    </div>
<%index++;
    } %>
<input type="submit" id="btnSave" value="Save" />
<script type="text/javascript">
    $(document).ready(function () {
        $('a[id^="catalog_cancel_"]').hide();
        $('img[id^="loading_"]').hide();
    });

    // cancela y esconde el despliegue del cascade de catalogo
    $('a[id^="catalog_cancel_"]').click(function () {
        $('div[id^="catalog_"]').html("");
        $(this).hide();
    });

    // abre el cascade de catalogo para un item en particular
    $('a[class^="modifyCat_"]').click(function () {
        var currentIndex = $(this).next().val();
        var currentType = $(this).next().next().val();
        var _levelId = $(this).next().next().next().val();
        var _catalogId = $(this).next().next().next().next().val();
        $("#catalog_" + currentType + "_" + currentIndex).next().val(_levelId);
        $("#catalog_" + currentType + "_" + currentIndex).next().next().val(_catalogId);
        $("#loading_" + currentType + "_" + currentIndex).show();
        $.ajax({
            data: { levelId: _levelId, catalogId: _catalogId },
            url: '<%= Url.Action("GetCatalogCascade", "ProductManager") %>',
            dataType: "json",
            error: function (e) {
                $("#loading_" + currentType + "_" + currentIndex).hide();
                alert("error: corriendo url " + e);
            },
            success: function (jObject) {
                $('div[id^="catalog_"]').html("");
                $('a[id^="catalog_cancel_"]').hide();
                $("#catalog_" + currentType + "_" + currentIndex).html(jObject.html);
                $("#catalog_cancel_" + currentType + "_" + currentIndex).show();
                $("#loading_" + currentType + "_" + currentIndex).hide();
            }
        });
    });

    // para borrar un item de nuestra bd
    $(".catalog_delete").click(function () {
        var _itemId = $(this).next().val();
        $.ajax({
            data: { itemId: _itemId },
            url: '<%= Url.Action("DeleteCatalogItem", "ProductManager") %>',
            dataType: "json",
            error: function (e) {
                $("#loading_" + currentType + "_" + currentIndex).hide();
                alert("error: corriendo url " + e);
            },
            success: function (jObject) {
                $(this).parent().remove();
            }
        });
    });
</script>
