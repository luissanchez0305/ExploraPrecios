<%@ Page Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Explora_Precios.ApplicationServices" %>
<%
	var email = new EmailServices("info@exploraprecios.com", "Error", "Error YA!");
	try
	{
		email.Send();
	}
	catch { } %>
	<html>
		<head>
			<title>ExploraPrecios.com - Página de error</title>
			<link href="/content/images/compass_icon.png" rel="shortcut icon" /> 
		</head>
		<body style="font-size:14px; font-family:Arial;">
			<div style="background: url(/content/images/logo_modificado.png) no-repeat left top ; width:275px;height:146px; float:right;"></div>
			<img style="float:left;" src="/Content/Images/warning_icon.png" alt="warning" width="315px" height="273px" />
			<div style="position: absolute; top: 40%; text-align:center; width:100%; height:100%; vertical-align:middle;">
				<h1>Algo sucedió</h1>
				<div><label>Podría <a href="/Contact">contactarnos</a></label> o <label>volver a la <a href="/">página</a></label></div>
				<div>En todo caso nuestro equipo técnico ha sido avisado y estamos trabajando en darte el mejor servicio.</div><br />
				<h3>Disculpe los inconvenientes</h3>
			</div>
		</body>
	</html>