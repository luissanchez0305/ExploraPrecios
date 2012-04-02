<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Explora_Precios.Web.Controllers.ViewModels.GroupManagerViewModel>" %>


	<div><h2 style="float:left; vertical-align:top; padding:0px; margin:0px;">Mis grupos</h2><img style="margin-left:5px;" src="/Content/Images/people.png" width="16px" height="16px" alt="group" /></div>
	<div id="GroupManagerList">
		<% Html.RenderPartial("PartialViews/GroupManagerList", Model.GroupsViewModel); %>
	</div>
	<input type="hidden" id="groupManagerCurrentPage" value="0" />
	<%= Html.Hidden("groupManagerTotalPage", Model.TotalPage) %>
	<div style="width:100%">
		<div class="nav back" style="position:absolute; display:none; float:left;">
			<div style="background:url(/content/images/ui-icons_2e83ff_256x240.png) no-repeat -98px -51px; width:10px; height:10px;"></div>
		</div>
		<img id="groupLoading" src="/Content/Images/ajax-loader.gif" style="display:none; position:absolute; left:48%;" width="20px" height="20px" alt="loading..." /> 
		<div class="nav forward" style="float:right; <%= Model.TotalPage == 0 ? "display:none;" : "" %>">
			<div style="background:url(/content/images/ui-icons_2e83ff_256x240.png) no-repeat -34px -51px; width:10px; height:10px;"></div>
		</div>
	</div>
