<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.GroupViewModel>" %>
    
<form id="GroupForm" style="background-color:#F6F5EA;">
    <%= Html.Hidden("Product", ViewData["product"])%>
    <label class="WhenOffer">Enviar al muro de Facebook</label><div class="Answer">?</div>&nbsp;<%= Html.CheckBox("FacebookWall", Model.DoPublish) %><br />
    <div style="width: 100%; background-color:#F6F5EA;"><a href="javascript:ClosePanel('group');" style="padding-left:5px; float:right;" id="FollowCancel" class="anchor">Cancelar</a>
		<input type="button" style="float:right;" class="formbutton" id="GroupFormSubmit" 
			value="<%= Model.group.Grouped == Explora_Precios.Web.Controllers.ViewModels.GroupDisplay.Create ? "Crear" : Model.group.Grouped == Explora_Precios.Web.Controllers.ViewModels.GroupDisplay.InGroup ? "Enviar" : "Unirse" %>" />
		<label class="GroupDone" style="float:right; padding-right:5px; font-weight:bold;" >Listo!</label>
	</div>
</form> 
<script type="text/javascript">
    $(document).ready(function () {
        $('.GroupDone').hide();
    });
    $('#GroupFormSubmit').click(function () {
        $.ajax({
            type: "POST",
            url: '<%= Url.Action("SetGroup", "Home") %>',
            data: $('#GroupForm').serialize(),
            dataType: "json",
            error: function (x, e) {
                alert("Error");
            },
            success: function (data) {
                if (data.result == "fail") {
                    alert(data.msg);
                }
                else {
                    $('.GroupDone').show('slow');
                    autoTimer = setInterval("ClearPanel('Group')", 5000);
                }
            }
        });
    });
</script>