<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div class="LoginContainer">
    <div id="layer02_holder">
      <div id="center"></div>
    </div>

    <div id="layer03_holder">
      <div id="center">
      <table width="100%" border="0" cellspacing="0" cellpadding="0">
      <tr>
        <td>
        <form method="post" action="/Account/Login" id="LoginForm">
                <label class="title">Por favor introduzca su correo y contraseña</label><br />
                <label>Email  
                <%= Html.TextBox("email", "", new { @class = "textuser" })%>
                </label><br />
                <label>Contraseña
                <%= Html.Password("password", "", new { @class = "textpass", style = "margin-top:5px;" })%>
                </label><br /><br />
                <label><%= Html.CheckBox("remember") %>Recordarme</label>
                <label style="float:right;"><input type="button" class="button" id="btnChangePassword" value="Entrar" /><img alt="Loading..." src="/content/images/loading_big.gif" class="LoginLoading" style="left:-45px;" /></label><br />
                <%=Html.ValidationMessage("ErrorMessage", new { @class = "validation-summary-errors", style = "position:relative; top:10px;" })%>
                <%= Html.Hidden("Redirect", ViewData["redirect"])%>
            <div id="layer04_holder" class="loginbox">
                <div id="center">
                    <a href="javascript:void(0)" class="register" style="padding-right: 5px;">Registrese</a>
                    <a href="javascript:void(0)" class="forgot" style="padding-right: 5px;">Olvido la contraseña?</a>
                </div>
				<div class="will_login facebook hand" style="float:right;"><img src="../../Content/Images/facebook_login_button.gif" onclick="authorizeFacebook(); return false;" /></div>
            </div>
        </form>
        </td>
      </tr>
      </table>
      </div>
    </div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $('.LoginLoading').hide();
    });
    $('#btnChangePassword').click(function () {
        $('.LoginLoading').show();
        $.ajax({
            type: "POST",
            url: '<%= Url.Action("Login", "Account") %>',
            data: $('#LoginForm').serialize(),
            dataType: "json",
            error: function (x, e) {
                $('.LoginLoading').hide();
                alert('Error');
            },
            success: function (data) {
                $('.LoginLoading').hide();
                if (data.result == "fail") {
                    $(".LoginContainer").html(data.html);
                }
                else if (data.result == "redirect") {
                    $(".ui-dialog-titlebar").show();
                    $("#FloatingPanel").html(data.html);
                }
                else {
                    window.location = '<%= Url.Action("Index", "Home") %>';
                }
            }
        });
    });
</script>