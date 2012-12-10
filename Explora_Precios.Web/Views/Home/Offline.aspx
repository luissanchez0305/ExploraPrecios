<%@ Page Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Explora_Precios.ApplicationServices" %>
<%
	//if (Request.QueryString["aspxerrorpath"].Length > 0)
	//{
	//    var email = new EmailServices("info@exploraprecios.com", "Error en " + System.Configuration.ConfigurationManager.AppSettings["Enviroment"], "Error en " + Request.QueryString["aspxerrorpath"]);
	//    try
	//    {
	//        email.Send();
	//    }
	//    catch { }
	//} %>
	<html>
		<head>
			<title>ExploraPrecios.com - En remodelación</title>
			<link href="/content/images/compass_icon.png" rel="shortcut icon" /> 
		</head>
		<body style="font-size:14px; font-family:Arial;">
			<div style="background: url(/content/images/logo_modificado.png) no-repeat left top ; width:275px;height:146px; float:right;"></div>
			<img style="float:left;" src="/Content/Images/warning_icon.png" alt="warning" width="315px" height="273px" />
			<div style="position: absolute; top: 40%; text-align:center; width:100%; height:100%; vertical-align:middle;">
				<h1>Temporalmente en remodelación</h1>
				<div><label>En este momento nos encontramos realizando enormes cambios para brindarles un mejor servicio para cubrir todas sus necesidades en beneficio de su bolsillo</label></div>
				<div><label>Si gusta podría <a href="/Contact2">contactarnos</a></label></div><br />
				<h3>Disculpe los inconvenientes</h3>
			</div>
		</body>
	</html>