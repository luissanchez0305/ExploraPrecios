<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.GroupManagerViewModel>" %>


	<h2>Mis grupos</h2>
	<div id="GroupManagerList">
		<% Html.RenderPartial("PartialViews/GroupManagerList", Model.GroupsViewModel); %>
	</div>
	<input type="hidden" id="groupManagerCurrentPage" value="0" />
	<%= Html.Hidden("groupManagerTotalPage", Model.TotalPage) %>
	<div class="nav"><a class="back" href="javascript:void(0);" style="display:none;"><</a>&nbsp;&nbsp;&nbsp;<a class="forward" href="javascript:void(0);">></a></div>
