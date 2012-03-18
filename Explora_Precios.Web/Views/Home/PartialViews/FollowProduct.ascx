<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.FollowViewModel>" %>
    
<form id="FollowForm" style="background-color:#F6F5EA;">
    <%= Html.Hidden("Product", ViewData["product"])%>
    <label style="font-weight:bold;">Quiero que ExploraPrecios.com me envie un correo</label><br />
    <label class="WhenOffer">Cuando este en oferta</label>&nbsp;<%= Html.CheckBox("Offer", Model.Offer) %><br />
    <label class="WhenPrice">Cuando baje de precio</label>&nbsp;<%= Html.CheckBox("Price", Model.Price) %><br />
    <div style="width: 100%; background-color:#F6F5EA;"><a href="javascript:ClosePanel('follow');" style="padding-left:5px; float:right;" id="FollowCancel" class="anchor">Cancelar</a><input type="button" style="float:right;" class="formbutton" id="FollowFormSubmit" value="Guardar" /><label class="FollowDone" style="float:right; padding-right:5px; font-weight:bold;" >Listo!</label></div>
</form> 
<script type="text/javascript">
    $(document).ready(function () {
        $('.FollowDone').hide();
    });
    $('#FollowFormSubmit').click(function () {
        $.ajax({
            type: "POST",
            url: '<%= Url.Action("SetFollow", "Home") %>',
            data: $('#FollowForm').serialize(),
            dataType: "json",
            error: function (x, e) {
                alert("Error");
            },
            success: function (data) {
                if (data.result == "fail") {
                    alert(data.msg);
                }
                else {
                    $('.FollowDone').show('slow');
                    autoTimer = setInterval("ClearPanel('Follow')", 5000);
                }
            }
        });
    });
</script>