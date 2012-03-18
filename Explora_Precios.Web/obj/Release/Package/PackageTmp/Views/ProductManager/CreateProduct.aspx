<%@ Page Language="C#" MasterPageFile="~/Views/Shared/SiteMng.Master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage<Explora_Precios.Web.Controllers.ViewModels.ProductViewModel>" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
Exploraprecios.com - Manejo de productos - Agregar o actualizar un producto
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<h2><a href="/ProductManager">Manejo de productos</a> - Agregar o actualizar un producto</h2>
    <div>
        Get product from URL: 
        <input type="text" id="url_text" />
        <input type="button" id="getProduct" value="Go"/> Or
        <a href="#" id="CreateNew">Create New Product</a>
    </div>
    <%  using (Html.BeginForm("DoCreateProduct", "ProductManager", FormMethod.Post, new { id = "mngProductForm", onsubmit="return false;" }))
        {  //Html.AntiForgeryToken();%>
        <%--<form id="mngProductForm" action="">--%>
            <div class="Content">
                <img alt="Loading..." style="display:none;" width="25" height="25" id="loading_image" src="/content/images/loading_big.gif" />
                <h2 id="success" style="display:none">Successfully saved</h2>
                <div id="productContainer">
                <%--<% Html.RenderPartial("ProductForm", Model); %>--%>
                </div>
            </div>
        <%--</form>--%>
     <% } %>
     <script type="text/javascript">
        $(document).ready(function(){
            $("#getProduct").click(function(){
                var url = $("#url_text").val();
                LoadProductAjax("/ProductManager/CreateProduct", url, false);
            });    
            
            $("#btnSave").live("click",function(){
                SaveProductAjax();
                return false;
            });      
            
            $("#CreateNew").click(function(){
                LoadProductAjax("/ProductManager/CreateNewProduct",'', false);
            });  
        })
     </script>
</asp:Content>
