<%@ Page Language="C#" MasterPageFile="~/Views/Shared/SiteMng.Master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage<List<Explora_Precios.Web.Controllers.ViewModels.RepeatedProductsViewModel>>" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
Exploraprecios.com - Manejo de productos - Productos repetidos
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<h2><a href="javascript:history.go(-1);">Ir a menu de manejo de productos</a></h2>
    <div class="Content">
        <div id="productsContainer">
        <% foreach (var product in Model)
           { %>
           <a href="<%= product.masterProduct.productImageUrl %>"><img src="<%= product.masterProduct.productImageUrl %>" width="50" height="50" alt="<%= product.masterProduct.productName%>" /></a>
           <h3><%= product.masterProduct.productName %> - <%= product.masterProduct.productRef %> (<%= product.masterProduct.productId %>)</h3> <a href="/SearchRepeated/<%= product.masterProduct.productId %>">Unir</a>       
            <table border="2">
                <% foreach (var productRepeated in product.repeatedProducts)
                   { %>
                <tr>
                    <td><img src="<%= productRepeated.productRepeated.productImageUrl %>" width="40" height="40" alt="<%= productRepeated.productRepeated.productName %>" /> (<%= productRepeated.productRepeated.productId %>)</td>
                    <td><%= productRepeated.productRepeated.productName %><table border="1" width="100%"><% foreach (var clientProduct in productRepeated.productRepeated.clientList.Where(c => c.price > 0))
                                                                               { %><tr><td><a href="<%= clientProduct.url %>" ><%= clientProduct.clientName %></a></td></tr><% } %></table></td>
                    <td><%= productRepeated.productRepeated.productRef%></td>
                    <td><%= Html.CheckBox("todoname", productRepeated.isChecked) %></td>
                </tr>
                <% } %>
            </table><br />
        <% } %>
        </div>
    </div>
     <script type="text/javascript">
        $(document).ready(function(){
            $("#getProduct").click(function(){
                var url = $("#url_text").val();
                LoadProductAjax("/Manager/CreateProduct", url, false);
            });    
            
            $("#btnSave").live("click",function(){
                SaveProductAjax();
                return false;
            });      
            
            $("#CreateNew").click(function(){
                LoadProductAjax("/Manager/CreateNewProduct",'', false);
            });  
        })
     </script>
</asp:Content>
