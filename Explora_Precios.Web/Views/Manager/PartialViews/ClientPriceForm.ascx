<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Explora_Precios.Web.Controllers.ViewModels.ClientViewModel>>" %>
<%@ Import Namespace="Explora_Precios.ApplicationServices" %>
<% 
	var noReference = Model[0].reference != null && Model[0].reference.Length == 0;
	var clientId = Model[0].clientId; %>
<b><label>Clients & Prices</label></b>
<table id="clientPriceTable">
<% if (noReference)
   { %>
<tr id="possibleReferences"><td colspan="6">
	<h2>Posibles # de referencia</h2>
	<div style="float:left;">
		<%foreach(var @ref in Model[0].masterReference.Split(' ')){ %>
		<%= Html.CheckBox("ref_" + @ref, false, new { value = @ref })%> <label><%= @ref %></label><br />
		<% } %>
	</div>
	<div>
		<input type="button" onclick="javascript:insertReference();" value="Seleccionar" />
		<label id="tempdiv"></label>
	</div>
</td></tr>
<% } %>
<tr><th>&nbsp;</th><th>Price</th><th>Url</th><th>Reference</th></tr>
<tbody>
<% var clientIndex = 0;
 foreach (var client in Model)
   {   %>
<tr class="<%= (client.productStatus != Explora_Precios.ApplicationServices.ClientServices.ItemType.Possible_Related && !client.showMe) ? "hideMe" : "" %> <%= (client.productStatus == Explora_Precios.ApplicationServices.ClientServices.ItemType.Possible_Related) ? "possible" : "" %>">
	<td>
		<input type="hidden" id="clientList[<%= clientIndex %>].clientId" name="clientList[<%= clientIndex %>].clientId" value="<%= client.clientId %>" />
		<%= client.clientName %>
	</td>
	<td><% if (client.showMe)
		   {%><%= Html.TextBox("clientList[" + clientIndex + "].price", client.price, new { @class = "price" })%><% }
		   else {%> <%= Html.Hidden("clientList[" + clientIndex + "].price", client.price) %><%= client.price %><% } %></td>
	<td><% if (client.showMe)
		   {%><%= Html.TextBox("clientList[" + clientIndex + "].url", client.url, new { @class = "url" })%><% }
		   else {%><%= Html.Hidden("clientList[" + clientIndex + "].url", client.url)%><a href="<%= client.url %>" target="_blank"><%= client.url.Shorten(10) %></a><% } %></td>
	<td><% if (client.showMe)
		   {%><%= Html.TextBox("clientList[" + clientIndex + "].reference", client.reference, new { @class = "clientReference reference" + (noReference ? " hideme" : "") })%><% }
		   else {%><%= Html.Hidden("clientList[" + clientIndex + "].reference", client.reference)%><%= client.reference %><%} %></td>     
	<td id="isActiveCell<%= clientIndex %>"><% if (client.showMe)
		   {%><%= Html.CheckBox("clientList[" + clientIndex.ToString() + "].isActive", client.isActive)%>
		   <%= Html.Hidden("clientList[" + clientIndex + "].isMain", !(client.productStatus == Explora_Precios.ApplicationServices.ClientServices.ItemType.OnSite_NotOnDB), new { @class = "shownMainClientProduct" })%><%}
		   else {%><%= Html.Hidden("clientList[" + clientIndex + "].isActive", client.isActive)%>
		   <%= Html.Hidden("clientList[" + clientIndex + "].isMain", client.isMain)%>
		   <input type="radio" name="mainGroup" id="clientList[<%= clientIndex %>].isMain" class="mainClientProduct" />
		   <% } %>
	</td>
	<td><%= Html.Hidden("clientList[" + clientIndex.ToString() + "].productStatus", client.productStatus)%>
	<% var priceDisplay = "";            
		if (client.productStatus == Explora_Precios.ApplicationServices.ClientServices.ItemType.OnSite_NotOnDB)
		{ priceDisplay = "<label style=\"font-weight:bold; color: Green;\">Nuevo</label><br />"; }
	   else if (client.productStatus == Explora_Precios.ApplicationServices.ClientServices.ItemType.OnDB_NotOnSite)
		{ priceDisplay = "<label style=\"font-weight:bold; color: Red;\">Borrado</label><br />"; }
	   else if (client.productStatus == Explora_Precios.ApplicationServices.ClientServices.ItemType.Local)
		{
			priceDisplay = client.oldPrice > 0 ? "<label style=\"font-weight:bold;" + ((client.oldPrice != client.price) ? " color: Blue;" : "") + "\">Original: " + client.oldPrice + "</label>" : "" +
							  "<input type=\"hidden\" id=\"clientList[" + clientIndex + "].oldPrice\" name=\"clientList[" + clientIndex + "].oldPrice\" value=\"" + client.oldPrice.ToString("F2") + "\"><br />";
		}%>        
	   <%= priceDisplay %>
	   <%--<label style="font-weight:bold; color: Red;"> 
		<%= client.pageStatus == Explora_Precios.ApplicationServices.ClientServices.PageStatus.PageChanged ? "Sitio: Cambio" :
			   client.pageStatus == Explora_Precios.ApplicationServices.ClientServices.PageStatus.PageNotSet ? "Sitio: N/D" : ""%></label>--%>
	   <% if (client.specialPrice > 0)
		  {%><br /><label style="font-weight:bold;"> Oferta: $<%= client.specialPrice.ToString("F2") %></label><%= Html.Hidden("clientList["+clientIndex+"].specialPrice", client.specialPrice) %><%} %>
	</td>
</tr>
<% clientIndex++;
   } // end foreach %>
</tbody>
<tfoot>
<tr>
	<td>
	<select id="Clients" name="Clients">
		<% foreach(var client in new SharpArch.Data.NHibernate.Repository<Explora_Precios.Core.Client>().GetAll()){ %>
		<option value="<%= client.Id %>"><%= client.name %></option>    
		<% } %>
	</select>
	</td>
	<td>
		<input type="text" id="clientPrice" class="price" />
	</td>
	<td>
		<input type="text" id="clientProductUrl" class="url"/>
	</td>
	<td>
		<input type="text" id="clientProductReference" class="reference"/>
	</td>
	<td><a href="javascript:void(0);" id="assignClientPrice">Asignar</a></td>
</tr>
</tfoot>
</table>
<script type="text/javascript">
	function insertReference() {
		var objs = $('input[id^="ref_"]:checked');
		var _reference = "";
		for (var i = 0; i < objs.length; i++) {
			var obj = objs[i];
			_reference += $(obj).val();
		}
		$.get('<%= Url.Action("GetReferenceRelation", "Manager") %>', { reference: _reference, clientId: <%= clientId %>}, function (jObject) {
			if (jObject.result == "success"){
				if(jObject.found){
					//$('input[name="clientList[0].isActive"]').remove();
					//$('#isActiveCell0').append('<input type="hidden" id="clientList[0].isMain" name="clientList[0].isMain" value="false" /><input type="radio" name="mainGroup" id="clientList[0].isMain" class="mainClientProduct" />');
					if($("#clientPriceTable tbody tr").length == 1){
						var dataArray = jObject.data.split(';');
						dataArray.forEach(function(element, index, array){
							var itemArray = element.split(',');
							var clientId = itemArray[0];
							var clientName = itemArray[1];
							var price = itemArray[2];
							var url = itemArray[3];
							var reference = itemArray[4];
							$("#clientPriceTable tbody tr:last").after('<tr>'+
								'<td><input type="hidden" id="clientList['+(parseInt(index)+1)+'].clientId" name="clientList['+(parseInt(index)+1)+'].clientId" value="'+clientId+'" />'+clientName+'</td>'+
								'<td><input type="hidden" id="clientList['+(parseInt(index)+1)+'].price" name="clientList['+(parseInt(index)+1)+'].price" value="'+price+'" />'+price+'</td>'+
								'<td><input type="hidden" id="clientList['+(parseInt(index)+1)+'].url" name="clientList['+(parseInt(index)+1)+'].url" value="'+url+'" /><a href="'+url+'" target="_blank">'+ url.substring(0, 15) + '...</a></td>'+
								'<td><input type="hidden" id="clientList['+(parseInt(index)+1)+'].reference" name="clientList['+(parseInt(index)+1)+'].reference" value="'+reference+'" />'+reference+'</td>'+
								'<td id="isActiveCell'+index+'"><input type="hidden" id="clientList['+(parseInt(index)+1)+'].isMain" name="clientList['+(parseInt(index)+1)+'].isMain" value="false" /><input type="radio" name="mainGroup" id="clientList['+(parseInt(index)+1)+'].isMain" class="mainClientProduct" /></td>'+
								'<td><input type="hidden" id="clientList['+(parseInt(index)+1)+'].productStatus" name="clientList['+(parseInt(index)+1)+'].productStatus" value="Local" />&nbsp;</td>'+
							'</tr>');						
						});
					}
				}	
				$("#productRef").val(_reference);
				$(".clientReference").removeClass("hideme");
				$(".clientReference").val(_reference);
				$("#possibleReferences").addClass("hideme");
			}
		});
	}
</script>