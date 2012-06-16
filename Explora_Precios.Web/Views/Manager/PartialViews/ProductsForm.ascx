<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.ProductViewModel>" %>

<form id="ProductForm">
<div class="pageTitle" style="text-align:center;"></div>
<table width="500px">
	<tr style="vertical-align:top">
		<td>
			<%= Html.Hidden("catalogLevel", Model.catalogLevel) %>
			<%= Html.Hidden("catalogId", Model.catalogId) %>
			<input type="hidden" id="catalog.productId" name="productId"
			 value="<%= Model.productId %>" /><br />
			 <table>
				<tr>
					<td><b># de Referencia</b></td>
					<td><input type="text" id="productRef" name="productRef"
					value="<%= Model.productRef %>"/></td>
				</tr>        
				<tr>
					<td><b>Nombre</b></td>
					<td><input type="text" id="productName" name="productName" value="<%= Model.productName %>" /></td>
				</tr>        
				<tr>
					<td><b>Marca</b></td>
					<td><input type="text" id="productBrand" name="productBrand" value="<%= Model.productBrand %>" /></td>
				</tr>
				<tr>
					<td><b>URL de Imagen</b></td>
					<td><input type="text" id="productImageUrl" name="productImageUrl" value="<%= Model.productImageUrl %>" /> <a href="javascript:void(0)" class="showImage">Mostrar +</a></td>
				</tr>
				<tr>
					<td colspan="2"><img src="<%= Model.productImageUrl %>" alt="image" height="50%" width="50%" class="productImage" /></td>
<%--                </tr>
				<tr style="vertical-align:top">
					<td><b>Descripcion:</b></td>
					<td><textarea cols="30" rows="6" id="productDescription" name="productDescription"><%= Model.productDescription %></textarea></td>
				</tr>--%>
			 </table>
		</td>
		<td rowspan="2">
			<b>Caracteristicas:</b><br />
			Nueva caracteristica:<br />
			<table>
				<thead> 
					<tr>
						<td>Nombre</td>
						<td>Descripcion</td>
						<td>&nbsp;</td>
					</tr>
				</thead>
				<tbody> 
					<tr>
						<td><input type="text" id="qualityName" style="width:80px;" /> </td>
						<td><input type="text" id="qualityValue" /> </td>
						<td><a id="addQuality" href="javascript:void(0);">Agregar</a></td>                    
					</tr>
				</tbody>
			</table><br />
	
				<% if (Model.qualities.Count > 0)
				   { %>
			<a href="javascript:void(0);" id="showQualities">Mostrar +</a>
				   <% }
				   else { %> 
						<b>Sin caracteristicas</b> <% } %>
			<table id="tableQualities">
				<tbody> 
				<% var index2 = 0; 
				   foreach (var quality in Model.qualities)
				   {%>
						<tr>
							<td>
							<input type="hidden" value="<%= quality.Id %>" name="qualities[<%= index2 %>].Id" id="qualities[<%= index2 %>].Id" />
								<input type="text" value="<%= quality.name %>" name="qualities[<%= index2 %>].name"
									id="qualities[<%= index2 %>].name" style="width:80px;" />
							</td>
							<td>
								<input type="text" id="qualities[<%= index2 %>].value" name="qualities[<%= index2 %>].value"
									value="<%=  quality.value %>" /></td>
							<td> 
								<%= Html.CheckBox("qualities[" + index2 + "].active", quality.active) %>
							</td>
						</tr>
					<% index2++;
						} %>
					</tbody>
				</table>
		</td>
	</tr>
	<tr>
		<td>
			<% Html.RenderPartial("PartialViews/ClientPriceForm", Model.clientList); %>
			<div style="padding-left:50px"><% Html.RenderPartial("PartialViews/ShowCatalogCascade", Model.catalogProduct); %></div>        
		</td>
	</tr>
	<tr> 
		<td colspan="2">
			<input type="button" id="btnProductSave" value="Guardar" /> <%if (Model.hasChanged) { %><span style="color:Red;">Por favor guardar</span><%} %>
		</td>
	</tr>
</table>
<div id="navProduct">
	<table width="100%">
		<tr>
			<td><a id="navPrev" class="nav" style="float:left;" href="javascript:void(0)">Atras</a><input type="hidden" id="prevVal" /></td>
			<td><img id="product_Page_Loading" src="/content/images/loading_big.gif" alt="Loading" class="smallImg" /><div class="pageTitle"></div><a href="javascript:void(0)" id="pageJumpButton">To Page</a><input type="text" id="pageJump" style="width:20px;" /></td>
			<td><a id="navNext" class="nav" style="float:right;" href="javascript:void(0)">Siguiente</a><input type="hidden" id="nextVal" /></td>
		</tr>
	</table>
</div>
</form>
<script type="text/javascript">
	$(document).ready(function () {
		$("#tableQualities").hide();
		$("#product_Page_Loading").hide();
		$(".productImage").hide();
		$(".hideMe").hide();
	});

	$(".showImage").click(function () {
		if ($(this).html().indexOf("Mostrar") > -1) {
			$(".productImage").show();
			$(this).html("Ocultar -");
		}
		else {
			$(".productImage").hide();
			$(this).html("Mostrar +");
		}
	});

	$("#showQualities").click(function () {
		if ($(this).html().indexOf("Mostrar") > -1) {
			$("#tableQualities").show();
			$(this).html("Ocultar -");
		}
		else {
			$("#tableQualities").hide();
			$(this).html("Mostrar +");            
		}
	});

	$("#btnProductSave").click(function () {
		$("#product_Page_Loading").show();
		$.ajax({
			data: $("#ProductForm").serialize(),
			url: '/Manager/DoCreateProduct?page=' + ($("#nextVal").val() - 1) + '&client=' + $("#currentClientId").val(), //<%= Url.Action("DoCreateProduct","Manager") %>',
			dataType: "json",
			type: "POST",
			error: function(e){
				$("#product_Page_Loading").hide();     
				alert("error: corriendo url " + e);
			},
			success: function(jObject){
				if (jObject.result == "success") {
					$("#product_Page_Loading").hide();
					//$("#product").html(jObject.html);
				}
				else if (jObject.result == "fail") {
					$("#product_Page_Loading").hide();    
					alert("Hubo error en el proceso: " + jObject.msg);
				}            
			}
		});
	});
</script>
