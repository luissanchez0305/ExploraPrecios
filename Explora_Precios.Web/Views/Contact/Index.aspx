<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Explora_Precios.Web.Controllers.ViewModels.ContactViewModel>" %>
<%@ Import Namespace="Explora_Precios.Web.Controllers.Helpers" %>
<asp:Content ID="HeaderTitleContent" ContentPlaceHolderID="TitleContent" runat="server">
ExploraPrecios.com
</asp:Content>
<asp:Content ID="LeftMenuContent" ContentPlaceHolderID="LeftContent" runat="server">
<script charset="utf-8" src="http://widgets.twimg.com/j/2/widget.js"></script>
<script>
	new TWTR.Widget({
		version: 2,
		type: 'profile',
		rpp: 6,
		interval: 30000,
		width: 220,
		height: 500,
		theme: {
			shell: {
				background: '#A7C2CF',
				color: '#292710'
			},
			tweets: {
				background: '#ffffff',
				color: '#574e44',
				links: '#a88962'
			}
		},
		features: {
			scrollbar: false,
			loop: true,
			live: true,
			behavior: 'all'
		}
	}).render().setUser('exploraprecios').start();
</script>
<div style="width: 200px; padding-top:10px;">
<script type="text/javascript"><!--
	google_ad_client = "ca-pub-8106411801578478";
	/* 200x200 Small Left */
	google_ad_slot = "5459383721";
	google_ad_width = 200;
	google_ad_height = 200;
//-->
</script>
</div>
<script type="text/javascript"
src="http://pagead2.googlesyndication.com/pagead/show_ads.js">
</script>
</asp:Content>
<asp:Content ID="ProductContent" ContentPlaceHolderID="MainContent" runat="server">
<%= Html.ValidationSummary("Corrija los siguientes errores:")%>
<form id="contactform" method="post" action="/Contact/SendContact">
 <div class="contact">
    <% if (Model.success)
       { %>
        <label class="contactsuccess" for="cfsuccess">El mensaje ha sido enviado satisfactoriamente. En menos de 24 horas recibirá respuesta por parte de uno de nuestros representantes.</label> <br /><br />
    <% } %>
    <label class="contactlabel">Por favor proporcione la siguiente informacion y nos pondremos en comunicacion con usted tan pronto nos sea posible</label><br /><br /><br /><br /><br />
	<div>
	 <label for="cfname">Nombre: <em>(requerido)</em></label>
	 <%= Html.TextBox("name", Model.name, new {size = "30"}) %>
	</div>
	<div>
	 <label for="cfemail">Email: <em>(requerido)</em></label>
	 <%= Html.TextBox("email", Model.email, new {size = "30"}) %>
	</div>

	<!-- Add any more form fields that you want to add here -->

	<div>
	 <label for="cfmessage">Su Mensaje: <em>(requerido)</em></label>
	 <%= Html.TextArea("message", Model.message, new {cols = "30", rows = "8"}) %>
	</div>
    <%= Html.Captcha() %>
 </div>

 <div><input type="submit" value="Enviar" name="cfsubmit" id="cfsubmit" class="formbutton" /></div>
</form>
    <div id="FloatingPanel"></div>
    <div id="SuccessFBRegister"></div>
    <input type="hidden" id="current" />
    <input type="hidden" id="text" />
<script type="text/javascript">
    var autoFloatingTimer;
    $("#FloatingPanel").dialog({
        autoOpen: false,
        bgiframe: false,
        modal: true,
        draggable: false,
        height: 400,
        width: 620,
        resizable: false
    });
    $("#FloatingPanel").hide();
    $("#FloatingPanel").dialog("close");
    $("#FloatingPanel").bind("clickoutside", function (event) {
        $(this).hide();
        $(this).dialog("close");
    });
    $("#SuccessFBRegister").dialog({
        autoOpen: false,
        bgiframe: false,
        modal: true,
        draggable: false,
        height: 100,
        width: 100,
        resizable: false
    });
    $("#SuccessFBRegister").hide();
    $("#SuccessFBRegister").dialog("close");

    $(".gridItem").live('click', function () {
        var $this = $(this);
        var val = $(this).prev().val();
        LoadProductDetail($this, val, "0");        
    });

    $('.loginbox a').live('click', function () {
        var goAjax = true;
        var $this = $(this);
        var url = '';
        var _redirect = ''
        $('.LoginLoading').show();

        //        alert($this.attr('class'));
        if ($this.attr('class') == 'login') {
            $(".ui-dialog-titlebar").hide();
            $("#FloatingPanel").bind("clickoutside", function (event) {
                $(this).hide();
                $(this).dialog("close");
            });
            url = '<%= Url.Action("Login", "Account") %>';
        }
        else if ($this.attr('class') == 'register' || $this.attr('class') == 'profile') {
            $(".ui-dialog-titlebar").show();
            $("#FloatingPanel").unbind("clickoutside");
            $("#FloatingPanel").dialog("option", "title", 'Llene el formulario para registrarse en ExploraPrecios.com');
            url = '<%= Url.Action("Register", "Account") %>';
        }
        else if ($this.attr('class') == 'logout') {
            goAjax = false;
            window.location = '<%= Url.Action("Logout", "Account") %>';
        }
        else if ($this.attr('class') == 'forgot') {
            $(".ui-dialog-titlebar").hide();
            $("#FloatingPanel").bind("clickoutside", function (event) {
                $(this).hide();
                $(this).dialog("close");
            });
            url = '<%= Url.Action("Forgot", "Account") %>';
        }
        _redirect = $('#Redirect').val();

        if (goAjax) {
            $.blockUI({ message: '<h4>Un momento por favor...</h4>' });
            $.ajax({
                type: "GET",
                url: url,
                data: { redirect: _redirect },
                dataType: "json",
                error: function (x, e) {
                    $('.LoginLoading').hide();
                    $.unblockUI();
                },
                success: function (data) {
                    $('.LoginLoading').hide();
                    if (data.result == "fail") {
                        alert(data.msg);
                        $.unblockUI();
                    }
                    else {
                        $("#FloatingPanel").show();
                        $("#FloatingPanel").dialog("open");
                        $("#FloatingPanel").html(data.html);
                        $.unblockUI();
                    }
                }
            });
        }
    });
    function Clear() {
        clearInterval(autoFloatingTimer);
        $("#FloatingPanel").hide();
        $("#FloatingPanel").dialog("close");    
    }
</script>

</asp:Content>
