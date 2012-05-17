<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<String>" %>
<% var IsLoggedIn = !string.IsNullOrEmpty(Model); %>
<div><embed src='http://goanimate.com//api/animation/player?utm_source=embed' type='application/x-shockwave-flash' wmode='transparent' width='400' height='286' FlashVars='movieOwner=lsanchez&movieId=0tlVe4koBOu8&movieLid=0&movieTitle=Promo1&movieDesc=&userId=&apiserver=http%3A//goanimate.com/&appCode=go&thumbnailURL=http%3A//goanimate.com//files/thumbnails/movie/2042/4682042/11167195L.jpg&fb_app_url=http%3A//goanimate.com/&copyable=0&showButtons=1&isEmbed=1&chain_mids=&ctc=go&tlang=en_US&isPublished=1&movieOwnerId=0YdX8hrm9uos&is_private_shared=0&autostart=1' allowScriptAccess='always' allowFullScreen='true' style="margin-top: <%= IsLoggedIn ? "40" : "4" %>px; display:block; margin-left:auto; margin-right:auto;"></embed></div>
<div class="<%= IsLoggedIn ? "hideme" : "" %>" style="width:480px; height:50px; margin-left:45px; border:6px solid #CE8036; padding: 2px; border-radius:10px; ">
	<div id="video_register_title" style="font:bold 12px Arial; color:#4E351F; text-align:center; text-shadow:2px 1px 0 #FEEDB4;">Registrarse es muy fácil. Sólo introduce tu email o usa tu Facebook.</div>
	<div id="video_register_email" style="float:left; margin-top:5px;">
		<form id="video_form" name="video_form" action="<%= Url.Action("VideoRegister", "Account") %>" method="post"><%= Html.TextBox("video_email", "", new { @class = "searchbox_textbox" }) %><input class="formbutton" type="button" id="video_submit" value="Enviar" style="top:2px; position: relative;" /><img alt="Loading..." src="/content/images/loading_big.gif" class="SmallLoading" style="float:right;" /></form>
		<label id="video_form_result" class="cGray bold"></label>
	</div>
	<div id="video_register_facebook" style="float:right; margin-top:5px;">
		<img class="hand" src="../../Content/Images/facebook_login_button.gif" onclick="authorizeFacebook(); return false;" style="margin-top:4px;"  />
	</div>
</div>
<script type="text/javascript">
	$(document).ready(function () {
		$('#video_form_result').hide();
		$('.SmallLoading').hide();
		clearTextBox('video_email', 'Tu email');
		$('#video_email').focus(function () {
			if ($('#video_email').val() == 'Tu email') {
				$('#video_email').attr('style', 'color:black;');
				$('#video_email').val('');
			}
		}).blur(function () {
			clearTextBox('video_email', 'Tu email');
		});
	});
	$('#video_form').keypress(function (event) {
		if (event.which == 13) {
			//RunForgotPassword();
		}
	});
	$('#video_submit').click(function () {
		$('.SmallLoading').show();
		$.ajax({
			type: "POST",
			url: '<%= Url.Action("VideoRegister", "Account") %>',
			data: $('#video_form').serialize(),
			dataType: "json",
			error: function (x, e) {
				$('.SmallLoading').hide();
				jAlert(e);
			},
			success: function (data) {
				$('.SmallLoading').hide();
				if (data.result == "fail") {
					jAlert(data.msg);
				}
				else {
					$('#video_register_title').hide();
					$('#video_form').hide();
					$('#video_register_facebook').hide();
					$('#video_form_result').show();
					$('#video_form_result').html(data.msg);
				}
			}
		});
	});
</script>