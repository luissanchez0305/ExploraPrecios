<%@ Page Language="C#" MasterPageFile="~/Views/Shared/SiteMng.Master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage<List<Explora_Precios.Web.Controllers.ViewModels.ClientViewModel>>" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
Exploraprecios.com - Manejo de productos y catalogos - Manejo de Catalogos de Clientes
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<h2><a href="/ProductManager">Manejo de productos y catalogos</a> - Manejo de Catalogos de Clientes</h2>
<table>
    <tr style="vertical-align:top">
        <td>
            <% 
                var switchContent = Model[0].isActive; 
                foreach (var client in Model)
               { %>
                <% if (client.isActive != switchContent)
                   {%><br />
                   <% switchContent = client.isActive;
                   } %>
                <div class="<%= !client.isActive ? "deactivated" : "item" %>" style="width:150px;">
                    <a href="javascript:void(0)" class="clientLink"><%= client.clientName %></a><input type="hidden" value="<%= client.clientId %>" />
                </div>
            <% } %>
        </td>
        <td>        
            <input type="hidden" id="currentClientId" />
            <img id="catalog_Loading" src="/content/images/loading_big.gif" alt="Loading" />
            <h3><label id="catalogTitle"></label></h3>
            <div id="unAttachedProducts">
                <b><a href="javascript:void(0)" id="showUnattachedItems">Productos sin catalogo</a></b>
            </div><br />
            <div id="catalog">
            </div>
        </td>
        <td>        
            <img id="product_Loading" src="/content/images/loading_big.gif" alt="Loading" /><label id="loading_productTitle"></label>
            <h3><label id="productTitle"></label></h3>
            <input type="hidden" id="currentItemId" />
            <input type="hidden" id="currentDisplay" />
            <div id="types"><a href="javascript:(void)" class="navDisplay">Existen</a>&nbsp;<a href="javascript:(void)" class="navDisplay">Nuevos</a>&nbsp;<a href="javascript:(void)" class="navDisplay">Borrados</a></div>
            <div id="product">
            </div>
        </td>
    </tr>
</table>

<script type="text/javascript">
    $(document).ready(function () {
        $("#catalog_Loading").hide();
        $("#product_Loading").hide();
        $("#loading_productTitle").hide();
        $("#unAttachedProducts").hide();
        $("#types").hide();
    });

    $('.mainClientProduct').live("click", function () {
        var $this = $(this);
        if ($this.prev().val() == 'True') {
//            $(".shownMainClientProduct").val('False');
            $this.attr('checked', false);
            $this.prev().val('False');
        }
        else {
//            $(".shownMainClientProduct").val('True');
            $this.prev().val('True');
        }
    });

    $(".clientLink").click(function () {
        var _clienId = $(this).next().val();
        $("#catalog_Loading").show();
        $.ajax({
            data: { clientId: _clienId, type: "Local", isShortVersion: "true" },
            url: '<%= Url.Action("GetCatalogClientList", "ProductManager") %>',
            dataType: "json",
            error: function (e) {
                alert("error: corriendo url " + e);
                $("#catalog_Loading").hide();
                $("#unAttachedProducts").hide();
                if ($("#product").val().length == 0)
                    $("#navProduct").hide();
            },
            success: function (jObject) {
                $("#catalog_Loading").hide();
                $("#unAttachedProducts").show();
                $("#catalogTitle").html(jObject.msg);
                $("#catalog").html(jObject.html);
                $("#productTitle").html("");
                $("#product").html("");
                $("#navProduct").hide();
                $("#types").hide();
                $("#currentClientId").val(_clienId);
            }
        });
    });

    $("#showUnattachedItems").live("click", function (jObject) {
        $("#product_Loading").show(); 
        $("#currentDisplay").val('original');
        $.get('<%= Url.Action("LoadUnattachedProducts", "ProductManager") %>', { client_Id: $("#currentClientId").val() }, function (jObject) {
            $("#product_Loading").hide();
            if (jObject.result == 'success' || jObject.result == 'empty') {
                $("#currentClientId").val(jObject.client);
                $("#types").hide();
                $("#currentItemId").val(0);
                $("#loading_productTitle").html("");
                $("#productTitle").html("<b>Productos sin catalogo asignado</b>");
                if (jObject.result == 'success') {
                    $("#navProduct").show();
                    $("#product").html(jObject.html);
                    // ------ cargar el navegador de productos -------
                    $("#nextVal").val("1");
                    $("#navPrev").addClass("disabled");
                    if (jObject.total == 1)
                        $("#navNext").addClass("disabled");
                    else
                        $(".pageTitle").html("Pagina 1 de " + jObject.total);
                }
                else {
                    $("#product").html("<b style='color:red;'>No existen productos sin catalogo</b><br/>" + jObject.html);
                    $("#navProduct").hide();
                }
            }
            else if (jObject.result == 'fail') {
                $("#loading_productTitle").hide();
                $("#loading_productTitle").html("");
                alert("Error: " + jObject.msg);
            }
        });
    });

    $(".showItems").live("click", function () {
        $this = $(this);

        var _itemID = $this.next().val();
        var _display = "original";
        var _type = "Local";
        if ($this.html() == "Local" || $this.html() == "Online") {
            _itemID = $this.next("input").val();
            _type = $this.html();
        }

        $("#product_Loading").show();
        $("#loading_productTitle").show();
        $("#loading_productTitle").html("<b>Cargando</b> " + $this.html() + "...");

        $.ajax({
            data: { loadSession: true, itemId: _itemID, display: _display },
            url: '<%= Url.Action("GetCatalogProducts", "ProductManager") %>',
            dataType: "json",
            error: function (e) {
                $("#product_Loading").hide();
                $("#loading_productTitle").hide();
                $("#loading_productTitle").html("");
                $("#navProduct").hide();
                alert("error: corriendo url " + e);
            },
            success: function (jObject) {
                if (jObject.result == "success" || jObject.result == "empty") {
                    $("#productTitle").html($this.html());
                    $("#product_Loading").hide();
                    $("#loading_productTitle").hide();
                    $("#types").show();
                    $("#loading_productTitle").html("");
                    $("#currentItemId").val(_itemID);
                    $("#currentClientId").val(jObject.client);
                    $("#currentDisplay").val(_display);
                    $(".navDisplay").removeClass("disabled");
                    $(".navDisplay").first().addClass("disabled");
                    if (jObject.result == "success") {
                        $("#navProduct").show();
                        $("#product").html(jObject.html);
                        // ------ cargar el navegador de productos -------
                        $("#nextVal").val("1");
                        $("#navPrev").addClass("disabled");
                        if (jObject.total == 1)
                            $("#navNext").addClass("disabled");
                        else
                        $(".pageTitle").html("Pagina 1 de " + jObject.total);
                    }
                    else {
                        $("#product").html("<b style='color:red;'>No existen productos para esta categoria</b><br/>" + jObject.html);
                        $("#navProduct").hide();
                    }
                }
                else if (jObject.result == "fail") {
                    $("#product_Loading").hide();
                    $("#loading_productTitle").hide();
                    $("#loading_productTitle").html("");
                    $("#navProduct").hide();
                    alert("Hubo un error - " + jObject.msg);
                }
            }
        });
    });

    $(".navDisplay").live("click", function () {
        var $this = $(this);
        if ($this.hasClass("disabled")) return;

        $("#product_Loading").show();        
        $(".navDisplay").removeClass("disabled");
        $this.addClass("disabled");
        var _display = "";
        if ($this.html() == "Nuevos") {
            _display = "new";
        }
        else if ($this.html() == "Borrados") {
            _display = "deleted";
        }
        else if ($this.html() == "Existen") {
            _display = "original";
        }

        $("#currentDisplay").val(_display);
        PageLoader(0);

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
        var _clientId = $("#currentClientId").val();
        var _display = $("#currentDisplay").val();
        var _itemID = $("#currentItemId").val();
        $.ajax({
            data: { loadSession: false, itemId: _itemID, clientId: _clientId, page: _page, display: _display },
            url: '<%= Url.Action("GetCatalogProducts", "ProductManager") %>',
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
                    $("#product").html(jObject.html);
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
                    var emptyListHtml = "<b style='color:red;'>No existen productos para esta categoria</b><br/>";
                    if (_display == "original")
                        emptyListHtml = emptyListHtml + jObject.html;
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
