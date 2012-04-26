<%@ Page Language="C#" MasterPageFile="~/Views/Shared/SiteMng.Master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage<List<Explora_Precios.Web.Controllers.ViewModels.ClientViewModel>>" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
Exploraprecios.com - Manejo de productos y catalogos - Manejo de Catalogos de Clientes
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<h2><a href="/Manager">Manejo de productos y catalogos</a> - Manejo de Catalogos de Clientes</h2>
<% 
    var switchContent = Model[0].isActive;
    foreach (var client in Model)
   { %>
    <% if (client.isActive != switchContent)
       {%><br />
       <% switchContent = client.isActive;
       } %>
    <div class="<%= !client.isActive ? "deactivated" : "item" %>" style="width:200px;">
        <%= client.clientName %> 
        <a href="javascript:void(0)" class="clientOnlineCheckLink" style="float:right; padding-left: 10px;">Online</a>
        <a href="javascript:void(0)" class="clientLocalLink" style="float:right;">Local</a><input type="hidden" value="<%= client.clientId %>" />
    </div>
<% } %>
<table cellpadding="10">
    <tr>
        <th><span id="OnSiteTitle"></span></th>
        <th><span id="OnDBTitle"></span></th>
    </tr>
    <tr style="vertical-align:top">
        <td style="width:700px;">
            <form id="OnSite_NotDB_Form" onsubmit="return false;">  
                <img id="OnSite_NotDB_Loading_Up" src="/content/images/loading_big.gif" alt="Loading" />
                <div id="OnSite_NotDB"></div>
                <img id="OnSite_NotDB_Loading_Down" src="/content/images/loading_big.gif" alt="Loading" />
                <label id="success_OnSite_NotDB">Salvado con exito</label>
                <input type="button" id="btnOnSite_Save" value="Guardar" />
            </form>
        </td>
        <td style="width:700px;">     
            <form id="OnDB_NotSite_Form" onsubmit="return false;">   
                <img id="OnDB_NotSite_Loading_Up" src="/content/images/loading_big.gif" alt="Loading" />
                <div id="OnDB_NotSite"></div>
                <img id="OnDB_NotSite_Loading_Down" src="/content/images/loading_big.gif" alt="Loading" />
                <label id="success_OnDB_NotSite">Salvado con exito</label>
            </form>
        </td>
    </tr>
</table>
<script type="text/javascript">
    $(document).ready(function () {
        $("#OnSite_NotDB_Loading_Up").hide();
        $("#OnDB_NotSite_Loading_Up").hide();
        $("#OnSite_NotDB_Loading_Down").hide();
        $("#OnDB_NotSite_Loading_Down").hide();
        $("#success_OnSite_NotDB").hide();
        $("#success_OnDB_NotSite").hide();
        $("#btnOnSite_Save").hide();
        $("#btnOnBD_Save").hide();
    });

    $("a").click(function () {
        $("#success_OnSite_NotDB").hide();
        $("#success_OnDB_NotSite").hide();
    });

    $(".clientLocalLink").click(function () {
        var _clienId = $(this).next().val();
        $("#OnSite_NotDB_Loading_Up").show();
        $.ajax({
            data: { clientId: _clienId, type: "Local", isShortVersion: "false" },
            url: '<%= Url.Action("GetCatalogClientList", "Manager") %>',
            dataType: "json",
            error: function (e) {
                alert("error: corriendo url " + e);
                $("#OnSite_NotDB_Loading_Up").hide();
            },
            success: function (jObject) {
                SuccessCatalogDisplay(jObject, "OnSite_NotDB", "OnSite", false);
                $("#OnSiteTitle").html("<h2>Catalogo Local</h2><br/>" + jObject.msg);
                $("#OnDBTitle").html("");
                $("#OnDB_NotSite").html("");
            }
        });
    });

    $(".clientOnlineCheckLink").click(function () {
        // Llenar la data de catalogo del site que no se encuentra en nuestra BD
        var _clienId = $(this).next().next().val();
        $("#OnSite_NotDB_Loading_Up").show();
        $("#OnDB_NotSite_Loading_Up").show();
        $.ajax({
            data: { clientId: _clienId, type: "OnSite", isShortVersion: "false" },
            url: '<%= Url.Action("GetCatalogClientList", "Manager") %>',
            dataType: "json",
            error: function (e) {
                alert("error: corriendo url " + e);
                $("#OnSite_NotDB_Loading_Up").hide();
            },
            success: function (jObject) {
                SuccessCatalogDisplay(jObject, "OnSite_NotDB", "OnSite", false);
            }
        });

        // Llenar la data de catalogo de nuestra BD que no se encuentra en el site
        $.ajax({
            data: { clientId: _clienId, type: "OnDB", isShortVersion: "false" },
            url: '<%= Url.Action("GetCatalogClientList", "Manager") %>',
            dataType: "json",
            error: function (e) {
                alert("error: corriendo url " + e);
                $("#OnDB_NotSite_Loading_Up").hide();
            },
            success: function (jObject) {
                SuccessCatalogDisplay(jObject, "OnDB_NotSite", "OnDB", false);
            }
        });
    });

    $("#btnOnSite_Save").click(function () {
        SaveCatalog($("#OnSite_NotDB_Form").serialize(), "OnSite_NotDB", "OnSite");
    });
//    $("#btnOnBD_Save").click(function () {
//        SaveCatalog($("#OnDB_NotSite_Form").serialize(), "OnDB_NotSite", "OnDB");
//    });
    function SaveCatalog(obj, proc, shortproc) {
        $("#" + proc + "_Loading_Down").show();
        $.ajax({
            data: obj,
            dataType: "json",
            url: "/Manager/DoSaveClientCatalog",
            type: "POST",
            error: function () {
                alert("ajax error");
                $("#success_" + proc).hide();
                $("#" + proc + "_Loading_Down").hide();
            },
            success: function (jObject) {
                if (jObject.result == "fail") {
                    alert("fallo grabando");
                    $("#success_" + proc).hide();
                    $("#" + proc + "_Loading_Down").hide();
                }
                else if (jObject.result == "error") {
                    alert("error del model");
                    $("#success_" + proc).hide();
                    $("#" + proc + "_Loading_Down").hide();
                }
                else {
                    SuccessCatalogDisplay(jObject, proc, shortproc, true);
                    $("#success_" + proc).show();
                }
            }
        });
    }

    function SuccessCatalogDisplay(obj, control, title, clean) {
        if (clean) {
            $("#" + title + "Title").html("");
            $("#" + control).html("");
            $("#btn" + title + "_Save").hide();
        }
        else {
            var where = title == "OnDB" ? "en la BD - No estan en el site<br/>" : title == "OnSite" ? "en el Sitio - No estan en la BD" : "";
            $("#" + title + "Title").html("<h2>Estan " + where + "</h2><br/>" + obj.msg);
            $("#" + control).html(obj.html);
            $("#btn" + title + "_Save").show();
        }
        $("#" + control + "_Loading_Up").hide();
        $("#" + control + "_Loading_Down").hide();
    }
</script>
</asp:Content>
