<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.UserViewModel>" %>
<%@ Import Namespace="Explora_Precios.Web.Controllers.Helpers" %>
<div id="RegisterPanel" class="ltr">
	<form method="post" id="RegisterForm" name="RegisterForm" action="/Account/Register">
		<%= Html.Hidden("Redirect", ViewData["redirect"])%>
		<%= Html.Hidden("IsNew", Model.IsNew) %>
		<%= Html.ValidationSummary("Por favor corregir los elementos que estan en rojo.") %>
		<%=Html.ValidationMessage("ErrorPassword", new { @class = "validation-summary-errors" })%>
		<%=Html.Hidden("NewUser", Model.IsNew ? "true" : "false")%>
		<ul>
			<li id="foli0" class="notranslate">
				<label class="desc" id="title0" for="Field0">
				Nombre
				<span id="Span1" class="req">*</span>
				</label>
				<span>
				<%= Html.TextBox("name", Model.Name) %>
				<label for="Field0">Nombre</label>
				</span>
				<span>
				<%= Html.TextBox("lastName", Model.LastName) %>
				<label for="Field1">Apellido</label>
				</span>
			</li>
			<li id="foli2" class="complex notranslate leftHalf">
				<label class="desc" id="title2" for="Field2">
				Naciste el
				<span id="req_2" class="req">*</span>
				</label>
				<span class="full addr1">
					<%= Html.TextBox("birthdate", !Model.Birthdate.IsMinDate() ? Model.Birthdate.ToShortDateString() : "" )%>
				</span>
			</li>
			<li id="foli9" class="notranslate rightHalf">
				<label class="desc" id="title9" for="Field9">
				Sexo
				<span id="req_9" class="req">*</span>
				</label>
				<div>
				<%= Html.RadioButton("gender", "f", Model.Gender == 'f') %> F <%= Html.RadioButton("gender", "m", Model.Gender == 'm')%> M 
				</div>
			</li>
			<li id="foli8" class="phone notranslate leftHalf">
				<label class="desc" id="title8" for="Field8">
				<br />
				Email
				</label>
				<span>
				<%= Html.TextBox("email", Model.Email) %>
				<%= Html.Hidden("hdn_email", Model.Email) %>
				</span>
			</li><% if (!Model.IsNew)
					{ %>
			<li id="foli9" class="phone notranslate rightHalf">
				<label class="desc" id="Label1" for="Field8">
				Contraseña Actual (Introducir solo para cambiar la contraseña)
				</label>
				<span>
				<%= Html.Password("password")%>
				</span>
			</li>
			<% } %>
			<li id="foli10" class="phone notranslate leftHalf">
				<label class="desc" id="Label2" for="Field8">
				<% if (!Model.IsNew)
				   { %>Nueva <% } %>Contraseña
				</label>
				<span>
				<%= Html.Password("newPassword")%>
				</span>
			</li>
			<li id="foli11" class="phone notranslate rightHalf">
				<label class="desc" id="Label3" for="Field8">
				Confirmación de contraseña
				</label>
				<span>
				<%= Html.Password("confirmPassword")%>
				</span>
			</li>     
			<li id="foli12" class="navigation notranslate leftHalf"> 
				<div class="loginbox">
					<% if (Model.IsNew)
					   { %><a href="javascript:void(0)" class="login" style="padding-right: 5px;">Ingresar</a> 
					<a href="javascript:void(0)" class="forgot" style="padding-right: 5px;">Olvido la contraseña?</a><br />
					<div class="will_login facebook hand" style="float:right;"><img src="../../Content/Images/facebook_login_button.gif" onclick="authorizeFacebook(); return false;" /></div><% } %>
				</div>
			</li>
			<li id="foli13" class="button notranslate rightHalf">
				<input class="button" type="button" id="btnSubmit" value="<% if(Model.IsNew){ %>Registrar<% } else { %>Guardar<% } %>" /><img alt="Loading..." src="/content/images/loading_big.gif" class="SmallLoading" />
			</li>
			<li id="foli14" class="notranslate cGray bold">                
			</li>
		</ul>
	</form>
</div>
<script type="text/javascript">
	$(document).ready(function () {
		$('#foli14').hide();
		$('.SmallLoading').hide();
		$("#FloatingPanel").dialog("option", "title", <%if(Model.IsNew){ %>'Llene el formulario para registrarse en ExploraPrecios.com' <% } else { %>'Perfil de usuario'<% } %>);
		$('#RegisterForm').keypress(function(event){
			if(event.which == 13){
				RegisterMe();
			}
		});
	});
	$("#birthdate").datepicker({
		showOn: 'both',
		buttonImage: '/Content/images/calendar.gif',
		duration: 0,
		changeYear: true,
		yearRange: '1950:2010',
		monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Sept.', 'Octubre', 'Nov.', 'Diciembre'],
		autoSize: true
	});
	$('#btnSubmit').click(function () {
		RegisterMe();
	});
	function RegisterMe(){
		$('.SmallLoading').show();
		var goOn = true;
		if($("#NewUser").val() == "false" && $("#hdn_email").val() != $("#RegisterForm #email").val()) {
			goOn = confirm("Vas a cambiar tu direccion de correo electronico?.\nEn unos momentos vas recibir un mensaje a tu nuevo correo para validarlo.\n\nDeseas Seguir?");
		}
		if(goOn) {
			$.ajax({
				type: "POST",
				url: '<%= Url.Action("Register", "Account") %>',
				data: $('#RegisterForm').serialize(),
				dataType: "json",
				error: function (x, e) {
					$('.SmallLoading').hide();
					alert('Error');
				},
				success: function (data) {
					$('.SmallLoading').hide();
					if (data.result == "fail") {
						$("#RegisterPanel").html(data.html);
					}
					else {
						$('#foli14').show();
						$('#foli14').html(data.msg);
						if(data.result == "redirect")
							autoFloatingTimer = setInterval("ClearRedirect("+data.html+")", 15000);
						else
							autoFloatingTimer = setInterval("Clear()", 15000);
					}
				}
			});
		}
		else {
			$('.SmallLoading').hide();
		}
	
	}
</script>