<%@ Page Language="C#" MasterPageFile="~/Views/Shared/SiteMng.Master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage<Explora_Precios.Web.Controllers.ViewModels.ProductViewModel>" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
Exploraprecios.com - Manejo de productos - Busqueda por referencia
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<h2><a href="/ProductManager">Manejo de productos</a> - Busqueda por referencia</h2>
    <div>
        Reference: 
        <input type="text" id="reference_text" />
        <input type="button" id="getProduct" value="Go"/>
    </div>  
    <%  using (Html.BeginForm("DoCreateProduct", "ProductManager", FormMethod.Post, new { id = "mngProductForm", onsubmit = "return false;" }))
        {
            //Html.AntiForgeryToken();%>
        <%--<form id="mngProductForm" action="">--%>
        
            <div class="Content">
                <img alt="Loading..." style="display:none;" class="smallImg" id="loading_image" src="/content/images/loading_big.gif" />
                <span id="notfound" style="display:none">No existe</span>
                <h2 id="success" style="display:none">Successfully saved</h2>
                <div id="productContainer">
                </div>
            </div>
        <%--</form>--%>
     <% } %>
     <script type="text/javascript">        
        $(document).ready(function(){
            $("#getProduct").click(function(){
                var ref = $("#reference_text").val();
                LoadProductAjax("/ProductManager/MngProductByReference", ref);
            });
     
            $("#btnSave").live("click",function(){
                SaveProductAjax();
                return false;
            });
        })
     </script>
</asp:Content>
