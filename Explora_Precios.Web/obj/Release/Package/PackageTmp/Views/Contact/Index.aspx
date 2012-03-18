<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Explora_Precios.Web.Controllers.ViewModels.ContactViewModel>" %>
<%@ Import Namespace="Explora_Precios.Web.Controllers.Helpers" %>
<asp:Content ID="HeaderTitleContent" ContentPlaceHolderID="TitleContent" runat="server">
ExploraPrecios.com
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
</asp:Content>
