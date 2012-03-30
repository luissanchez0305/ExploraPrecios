<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.GroupViewModel>" %>
    
<form id="GroupForm" style="background-color:#F6F5EA;">
    <%= Html.Hidden("ProductId", ViewData["product"])%>
    <% if (Model.IsFacebooked)
       { %>
    <label class="WhenOffer">Envialo a Facebook</label><div class="questionMark" style="position:relative; float:right;"></div>&nbsp;<%= Html.CheckBox("DoPublish", Model.DoPublish)%><br />
    <% } %>
    <div style="width: 100%; background-color:#F6F5EA;">
		<input type="button" style="float:left;" class="formbutton" id="GroupFormSubmit" 
			value="<%= Model.Grouped == Explora_Precios.Web.Controllers.ViewModels.GroupDisplay.Create ? "Crear" : Model.Grouped == Explora_Precios.Web.Controllers.ViewModels.GroupDisplay.InGroup ? "Enviar" : "Unirse" %>" />
        <a href="javascript:ClosePanel('group');" style="padding-left:5px; float:left;" id="FollowCancel" class="anchor">Cancelar</a>
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
                    $('.group .text').text('Grupo - ' + data.groupSize + ' pers.');
                    <% if (!Model.IsFacebooked){ %>
                    $('.group .text').addClass('noaction');
                    <% } %>
                }
            }
        });
    });

    $(".questionMark").simpletip({
        content: 'Mientras mas personas entren a este grupo, mejor será el descuento que recibirás.<br/><br/>Qué esperas?<br/><b>Dile a todos tus amigos!!</b>',
        fixed: true,
        position: [-120, 20],
        showEffect: 'slide',
        hideEffect: 'fade',
        showTime: 300
    });
</script>