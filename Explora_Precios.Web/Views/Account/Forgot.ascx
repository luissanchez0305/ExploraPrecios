<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div id="ForgotPanel" class="ltr">
	<form id="ForgotForm" name="ForgotForm" action="/Account/Forgot" method="post"> 
		<%= Html.Hidden("Redirect", ViewData["redirect"])%>
		<header id="header" class="info">
		<h2>Olvido su contraseña?</h2>
		<div>Introduzca su correo electrónico y recibirá un mensaje con su nueva contraseña, la cual podrá cambiar cuando desee.</div>
		</header>
		<ul>
			<li id="foli1" class="notranslate">
				<%=Html.ValidationSummary()%>
				<label class="desc" id="title0" for="Field0">
				Correo electronico
				<span id="Span1" class="req">*</span>
				</label>
				<span>
				<%= Html.TextBox("email") %>
				</span>
			</li>            
			<li id="foli2" class="navigation notranslate leftHalf">            
				<div class="loginbox">
					<a href="javascript:void(0)" class="register" style="padding-right: 5px;">Registrese</a>
					<a href="javascript:void(0)" class="login" style="padding-right: 5px;">Ingresar</a>
				</div>
			</li>
			<li id="foli3" class="button notranslate rightHalf">
				<input class="button" type="button" id="btnForgotPassword" value="Enviar" /><img alt="Loading..." src="/content/images/loading_big.gif" class="SmallLoading" />
			</li>
			<li id="foli14" class="notranslate cGray bold">                
			</li>
		</ul>
	</form>
</div>
<script type="text/javascript">
	$(document).ready(function () {
		$('.SmallLoading').hide();
		$('#ForgotForm').keypress(function (event) {
			if (event.which == 13) {
				RunForgotPassword();
			}
		});
	});
	$('#btnForgotPassword').click(function () {
		RunForgotPassword();
	});
	function RunForgotPassword() {
		$('.SmallLoading').show();
		$.ajax({
			type: "POST",
			url: '<%= Url.Action("Forgot", "Account") %>',
			data: $('#ForgotForm').serialize(),
			dataType: "json",
			error: function (x, e) {
				$('.SmallLoading').hide();
				alert('Error');
			},
			success: function (data) {
				$('.SmallLoading').hide();
				if (data.result == "fail") {
					$("#ForgotPanel").html(data.html);
				}
				else {
					$('#foli14').show();
					$('#foli14').html(data.msg);
					if (data.result == "redirect")
						autoFloatingTimer = setInterval("ClearRedirect(" + data.html + ")", 15000);
					else
						autoFloatingTimer = setInterval("Clear()", 15000);
				}
			}
		});		
	}
</script>