<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Explora_Precios.Web.Controllers.ViewModels.ContactViewModel>" %>
<%@ Import Namespace="Explora_Precios.Web.Controllers.Helpers" %>
<asp:Content ID="HeaderTitleContent" ContentPlaceHolderID="TitleContent" runat="server">
ExploraPrecios.com
</asp:Content>
<asp:Content ID="ProductContent" ContentPlaceHolderID="MainContent" runat="server">
<label class="validated bold cGray">Su correo electronico ha sido validado. En breve sera redirigido a nuestra seccion de productos donde podrá aprovechar de todos los beneficios de formar parte de ExploraPrecios.com</label>
<script type="text/javascript">
    var autoTimer;
    $(document).ready(function () {
        autoTimer = setInterval('window.location="/Home"', 10000);
    });
</script>
</asp:Content>
