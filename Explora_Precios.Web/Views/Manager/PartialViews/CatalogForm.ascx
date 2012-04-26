<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Explora_Precios.Web.Controllers.ViewModels.CatalogItemViewModel>>" %>

<% 
    var shortVersion = Request.QueryString["isShortVersion"] != null ? Boolean.Parse(Request.QueryString["isShortVersion"]) : false;
    var index = 0;
    foreach (var catalogItem in Model)
    {  %>
    <div class="item">
         <% if (!shortVersion)
               { %>
            <img alt="Loading" id="loading_<%= catalogItem.type %>_<%= index %>" src="/content/images/loading_big.gif" class="smallImg" />
            <%= Html.Hidden("CatalogList[" + index.ToString() + "].clientId", catalogItem.clientId)%>
            <%= Html.CheckBox("CatalogList[" + index.ToString() + "].selected", catalogItem.selected)%> 
        <% } %>

        <% if(!string.IsNullOrEmpty(catalogItem.name)) { %>
            <% if (shortVersion)
               {%>
                <a href="javascript:void(0)" class="showItems">
            <% } %>
            <%= catalogItem.name %>
            <% if (shortVersion)
               {%>
                </a>
            <% } %>

            <% if (!shortVersion)
                {%>
                <%= Html.Hidden("CatalogList[" + index.ToString() + "].name", catalogItem.name) %> - 
            <% } %>
        <% } %>

        <% if (!shortVersion)
           { %>
            <a target="_blank" href="<%= "http://"+catalogItem.clientAddress+catalogItem.url %>"> <%= catalogItem.url%></a>
            <%= Html.Hidden("CatalogList[" + index.ToString() + "].url", catalogItem.url)%>
            <%= Html.Hidden("CatalogList[" + index.ToString() + "].clientAddress", catalogItem.clientAddress)%>
            <% if (catalogItem.type == "OnDB_NotOnSite" || catalogItem.type == "Local")
               { %>
               <a class="catalog_delete_<%= index %>" style="float:right; padding-left:5px;" href="javascript:void(0)">Borrar</a> (<%= catalogItem.products == null ? "0" : catalogItem.products.Count.ToString() %>)
               <% } %>
        <% } %>

        <%= Html.Hidden("CatalogList[" + index.ToString() + "].dbId", catalogItem.dbId)%>

        <% if (!shortVersion)
           { %>       
                <% if (catalogItem.type == "OnSite_NotOnDB" || catalogItem.type == "Local")
                { %>
                    <a href="javascript:void(0)" class="modifyCat_<%= catalogItem.type %>_<%= index %>" style="float:right;">Catalogo</a>
                <%} %>
                <%= Html.Hidden("hdn_index_" + index.ToString() + "_" + catalogItem.type, index)%>
                <%= Html.Hidden("CatalogList[" +index.ToString() +"].type", catalogItem.type)%>
                <%= Html.Hidden("CatalogList[" +index.ToString() +"].levelId", catalogItem.levelId) %>
                <%= Html.Hidden("CatalogList[" + index.ToString() + "].catalogId", catalogItem.catalogId)%>
                <div class="catalog" id="catalog_<%= catalogItem.type %>_<%= index %>">
                </div>
                <a href="javascript:void(0)" id="catalog_cancel_<%= catalogItem.type %>_<%= index %>" class="link">Cerrar</a><br />
        <% } %>
    </div>
<%index++;
    } %>
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
            url: '<%= Url.Action("GetCatalogCascade", "Manager") %>',
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
    var countRun = 0;
    $('a[class^="catalog_delete_"]').click(function () {
        countRun += 1;
        var $this = $(this);
        var _itemId = $this.next().val();
        $.ajax({
            data: { itemId: _itemId },
            url: '<%= Url.Action("DeleteClientCatalogItem", "Manager") %>',
            error: function (e) {
                $("#loading_" + currentType + "_" + currentIndex).hide();
                alert("error: corriendo url " + e);
            },
            success: function (jObject) {
                $this.parent().hide();
//                if (jObject.result == "success")
//                    $this.parent().hide();
//                else if (jObject.result == "fail")
//                    alert("Hubo un error - " + jObject.msg);
            }
        });
    });
</script>
