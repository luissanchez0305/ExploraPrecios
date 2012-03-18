<%@ Page Language="C#" MasterPageFile="~/Views/Shared/SiteMng.Master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage<Explora_Precios.Web.Controllers.ViewModels.ProductViewModel>" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
Exploraprecios.com - Manejo de productos - Agregar o actualizar un producto
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<h2>Manejo de productos - Agregar o actualizar un producto</h2>
    <div>
        Search by reference: 
        <input type="text" id="search_text" value="<%= ViewData["search_text"] %>" />
        <input type="button" value="Search"/>
    </div>  
    <%  using (Html.BeginForm("DoUpdatePrices", "HomeManager", FormMethod.Post))
        {  Html.AntiForgeryToken();%>
            <div class="Content">
                <div class="contentLeft">
                    <h3>Detalles de producto</h3>
                </div>
                <div class="contentRight">
                    <h3>Informacion de clientes y catalogo</h3>
                </div>  
                <% Html.RenderPartial("ProductForm", Model); %>
            </div>
            <input type="submit" id="btnSearch" name="button" value="Save" />
     <% } %>
     <script type="text/javascript">
        $(document).ready(function(){
            $("#addQuality").click(function()
            {
                var qName = $("#qualityName").val();
                var qVal = $("#qualityValue").val();
                if(qName.length > 1 && qVal.length > 1)
                {
                    $("#noQualities").remove();
                    var rowcount = $("#tableQualities tr").length;
                    /*$.ajax({                
                        type: "GET",
                        contentType: "application/json; charset=utf-8",
                        url: "FindQualityName?v=" + val,
                        data: "{}",
                        dataType: "json",
                        success: function(data){
                            data.qId
                        }
                    });*/
                    
                    // Nuevo row de caracteristica
                    $("#tableQualities").find("tbody")
                    .append($("<tr>")
                        .append($("<td>")
                            .append($("<input>")
                                .attr("type","hidden")
                                .attr("value","0")
                                .attr("name","qualities["+rowcount+"].Id")
                                .attr("id","qualities["+rowcount+"].Id")
                            )
                        )
                        .append($("<td>")
                            .append($("<input>")
                                .attr("type","text")
                                .attr("value",qName)
                                .attr("name","qualities["+rowcount+"].name")
                                .attr("id","qualities["+rowcount+"].name")
                            )
                        )
                        .append($("<td>")
                            .append($("<input>")
                                .attr("type","text")
                                .attr("value",qVal)
                                .attr("name","qualities["+rowcount+"].value")
                                .attr("id","qualities["+rowcount+"].value")
                            )
                        )
                        .append($("<td>")
                            .append($("<input>")
                                .attr("type","checkbox")
                                .attr("class","CBUpdate")
                                .attr("checked","checked")
                            )
                            .append($("<input>")
                                .attr("type","hidden")
                                .attr("name","qualities["+rowcount+"].active")
                                .attr("id","qualities["+rowcount+"].active")
                                .attr("value","True")
                            )
                        )
                    );
                }
            });
            
            $("#assignClientPrice").click(function(){
                var clientId = $("#Clients").val();
                var clientName = $("#Clients option:selected").text()
                var price = $("#clientPrice").val();
                if(price.length > 0)
                {
                    var rowcount = $("#clientPriceTable tr").length;
                    $("#clientPriceTable").find("tbody")
                        .append($("<tr>")
                            .append($("<td>")
                                .append($("<input>")
                                    .attr("type","hidden")
                                    .attr("value",clientId)
                                    .attr("name","client["+rowcount+"].Id")
                                    .attr("id","client["+rowcount+"].Id")                                
                                )
                                .append(clientName)
                            )
                            .append($("<td>")
                                .append($("<input>")
                                    .attr("type","text")
                                    .attr("value",price)
                                    .attr("name","client["+rowcount+"].price")
                                    .attr("id","client["+rowcount+"].price")                                      
                                )
                            )
                        )
                }                
            });
            
        })
     </script>
</asp:Content>
