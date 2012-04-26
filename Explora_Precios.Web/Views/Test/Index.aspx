<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" 
	Inherits="System.Web.Mvc.ViewPage" %>


<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<!--<script src="https://connect.facebook.net/en_US/all.js#appId=285146028212857&xfbml=1&channelUrl=//www.exploraprecios.com/channel.html&cookie=1&status=1"></script> 
<fb:registration 
	fields="name,birthday,gender,location,email" 
	redirect-uri="https://www.exploraprecios.com"
	width="530">
</fb:registration>-->

<%= ViewData["result"] %>
<form id="form1" method="post" action="/Test/Index">
	<%= Html.TextBox("postValue") %>
	<input type="submit" value="go" />
</form>
</asp:Content>
