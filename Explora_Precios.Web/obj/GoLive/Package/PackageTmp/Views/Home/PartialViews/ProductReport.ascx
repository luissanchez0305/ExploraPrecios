<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Byte[]>" %>
<div style="float:right; font-size: 10px;">
    Por favor introduzca los <br />caracteres en la imagen<br />
    y presione el boton "Reportar"<br />
    <label id="novalid"></label>
</div>
<div>
    <img class="captchaImage" alt="Captcha Image" src="/ShowImage/?image=<%= new Explora_Precios.ApplicationServices.CommonUtilities().CacheImage(Model) %>" />
    <br /><input type="text" style="width:45px; height:15px;" id="captchaText" />&nbsp;<img id="refreshCaptchaImage" class="hand" style="width:16px; height:16px;" alt="Captcha Image" src="../../Content/Images/view-refresh.png" />&nbsp;<input type="button" id="captchaButton" value="Reportar" class="formbutton" />
</div>
<script type="text/javascript">
    $("#refreshCaptchaImage").click(function () {
        $.get('<%= Url.Action("GenerateImageCaptcha", "Home") %>', function (data) {
            $(".captchaImage").attr('src', '/ShowImage/?image=' + data.key);
            $("#text").val(data.text);
        });
    });

    $("#captchaButton").click(function () {
        var $this = $(this);
        var productReportDiv = $this.parent().parent();
        $.ajax({
            type: "GET",
            url: '<%= Url.Action("ValidateCaptcha", "Home") %>',
            data: { _textGened: $("#text").val(), _text: $("#captchaText").val(), _current: $("#current").val() },
            dataType: "json",
            error: function (x, e) {
                alert("error");
            },
            success: function (data) {
                if (data.result == "fail") {
                    alert(data.msg);
                }
                else {
                    if (data.result == "novalid") {
                        productReportDiv.html(data.html);
                        $("#novalid").html('Caracteres incorrectos');
                        $("#text").val(data.text);
                    }
                    else if (data.result == "valid") {
                        $("#FloatingPanel").data("title.dialog", data.msg);
                        $("#FloatingPanel").html(data.html);
                    }
                }
            }
        });
    });
</script>