<%@ Page Language="C#" MasterPageFile="~/Views/Shared/SiteMng.Master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
Exploraprecios.com - Manejo de productos y catalogos - Manejo General del Catalogo
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<h2><a href="/ProductManager">Manejo de productos y catalogos</a> - Manejo General del Catalogo 
<img id="Image_Loading" src="/content/images/loading_big.gif" alt="Loading" class="smallImg" /></h2>
<table cellpadding="10" width="100%">
    <tr>
        <th><span id="DepartmentTitle">Departamentos</span></th>
        <th><span id="CategoryTitle">Categorias</span></th>
        <th><span id="SubCategoryTitle">Sub-Categorias</span></th>
        <th><span id="ProductTypeTitle">Tipo de Productos</span></th>
    </tr>
    <tr style="vertical-align:top">
        <td style="width:400px;">
            <div id="DepartmentDiv"></div>
            <br />
            <div id="DivNewDepartment">
                <input type="text" id="NewDepartment" class="w120" /> <a href="javascript:void(0)" id="AddDepartment" style="float:right;">Agregar</a>
            </div>
        </td>
        <td style="width:400px;">   
            <h4><div id="CurrentDepartmentTitle"></div></h4>
            <input type="hidden" id="CurrentDepartment" />
            <div id="CategoryDiv"></div>  
            <br />
            <div id="DivNewCategory" class="item">
                <input type="text" id="NewCategory" class="w120" /> <a href="javascript:void(0)" id="AddCategory" style="float:right;">Agregar</a>
            </div>          
        </td>
        <td style="width:400px;">
            <h4><div id="CurrentCategoryTitle"></div></h4>
            <input type="hidden" id="CurrentCategory" />
            <div id="SubCategoryDiv"></div>
            <br />
            <div id="DivNewSubCategory">
                <input type="text" id="NewSubCategory" class="w120" /> <a href="javascript:void(0)" id="AddSubCategory" style="float:right;">Agregar</a>
            </div>
        </td>
        <td style="width:400px;">   
            <h4><div id="CurrentSubCategoryTitle"></div></h4>
            <input type="hidden" id="CurrentSubCategory" />
            <div id="ProductTypeDiv"></div>
            <br />
            <div id="DivNewProductType">
                <input type="text" id="NewProductType" class="w120" /> <a href="javascript:void(0)" id="AddProductType" style="float:right;">Agregar</a>
            </div>
        </td>
    </tr>
</table>
<script type="text/javascript">
    $(document).ready(function () {
        $('div[id^="DivNew"]').hide();
        $("#Image_Loading").hide();
        CallAjaxCatalogItems("", "Department", 0, false);
    });

    function CallAjaxCatalogItems(_title, _type, _id, _loadNext) {
        $("#Image_Loading").show();
        $.ajax({
            data: { type: _type, id: _id, loadNext: _loadNext },
            url: '<%= Url.Action("GetCatalogList", "ProductManager") %>',
            dataType: "json",
            error: function (e) {
                alert("error: corriendo url " + e);
                $("#Image_Loading").hide();
            },
            success: function (jObject) {
                $("#Image_Loading").hide();
                if (_loadNext) {
                    $("#Current" + _type + "Title").html(_title);
                    $("#Current" + _type).val(_id);
                    if (_type == "Department") { // buscamos categorias      
                        _type = "Category";
                        $("#CurrentCategoryTitle").html("");
                        $("#CurrentSubCategoryTitle").html("");
                        $("#CurrentCategory").val("");
                        $("#CurrentSubCategory").val("");
                        $("#SubCategoryDiv").html("");
                        $("#ProductTypeDiv").html("");
                        $("#DivNewSubCategory").hide();
                        $("#DivNewProductType").hide();
                    }
                    else if (_type == "Category") { // buscamos subcategorias    
                        _type = "SubCategory";
                        $("#CurrentSubcategoryTitle").html("");
                        $("#CurrentSubcategory").val("");
                        $("#ProductTypeDiv").html(" ");
                        $("#DivNewProductType").hide();
                    }
                    else if (_type == "SubCategory") { // buscamos tipos de productos   
                        _type = "ProductType";
                    }
                }
                $("#DivNew" + _type).show();
                if (jObject.empty == "False") {
                    $("#" + _type + "Div").html(jObject.html);
                }
                else { $("#" + _type + "Div").html("No contiene " + _title); }
            }
        });
    }

    function GetParentId(type) {
        if (type == "Category") {
            return $("#CurrentDepartment").val();
        }
        else if (type == "SubCategory") {
            return $("#CurrentCategory").val();
        }
        else if (type == "ProductType") {
            return $("#CurrentSubCategory").val();
        }
        return 0;
    }

    $('a[id^="Add"]').click(function () {
        var $this = $(this);
        var _type = $this.attr("id").substring(3);
        var _value = $this.prev().val();
        var _parentId = "0";
        if (_value.length > 0) {
            _parentId = GetParentId(_type);
            $("#Image_Loading").show();
            $.ajax({
                url: '<%= Url.Action("AddCatalogItem", "ProductManager") %>',
                data: { type: _type, parentId: _parentId, value: _value },
                dataType: "json",
                error: function (e) {
                    $("#Image_Loading").hide();
                    alert("error: corriendo url " + e);
                },
                success: function (jObject) {
                    if (jObject.result == "fail") {
                        $("#Image_Loading").hide();
                        alert("Error - " + jObject.msg);
                    }
                    else if (jObject.result == "success") {
                        $("#Image_Loading").hide();
                        $this.prev().val("");
                        CallAjaxCatalogItems("", _type, _parentId, false);
                    }
                }
            });
        }
    });

    $(".Select").live("click", function () {
        var $this = $(this);
        var title = $this.prev().prev().val();
        var type = $this.next().next().val();
        var id = $this.next().val();
        CallAjaxCatalogItems(title, type, id, true);
    });

    $(".Delete").live("click", function () {
        var $this = $(this);
        var _id = $this.next().next().val();
        var _type = $this.next().next().next().val();
        if (_type.lenght == 0) {
            _id = $this.next();
            _type = $this.next().next().val();
        }
        $("#Image_Loading").show();
        $.ajax({
            url: '<%= Url.Action("DeleteCatalogItem", "ProductManager") %>',
            data: { type: _type, id: _id },
            dataType: "json",
            error: function (e) {
                $("#Image_Loading").hide();
                alert("error: corriendo url " + e);
            },
            success: function (jObject) {
                if (jObject.result == "fail") {
                    $("#Image_Loading").hide();
                    alert("Error - " + jObject.msg);
                }
                else if (jObject.result == "success") {
                    $("#Image_Loading").hide();
                    CallAjaxCatalogItems("", _type, GetParentId(_type), false);
                }
            }
        });

    });

    $(".Save").live("click", function () {
        var $this = $(this);
        var _type = $this.prev().prev().val();
        var data = $("#" + _type + "Form").serialize();
        $("#Image_Loading").show();
        $.ajax({
            url: '<%= Url.Action("DoSaveCatalog", "ProductManager") %>',
            data: data,
            dataType: "json",
            type: "POST",
            error: function (e) {
                $("#Image_Loading").hide();
                alert("error: corriendo url " + e);
            },
            success: function (jObject) {
                if (jObject.result == "fail") {
                    $("#Image_Loading").hide();
                    alert("Error - " + jObject.msg);
                }
                else if (jObject.result == "success") {
                    $("#Image_Loading").hide();
                    CallAjaxCatalogItems("", _type, GetParentId(_type), false);
                }
            }
        });

    });
</script>
</asp:Content>
