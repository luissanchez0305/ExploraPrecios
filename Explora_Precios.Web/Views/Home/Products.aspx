<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Explora_Precios.Web.Controllers.ViewModels.HomeViewModel>" %>

<asp:Content ID="HeaderTitleContent" ContentPlaceHolderID="TitleContent" runat="server">
ExploraPrecios.com
</asp:Content>
<asp:Content ID="LeftMenuContent" ContentPlaceHolderID="LeftContent" runat="server">
<% Html.RenderPartial("PartialViews/LeftMenu", Model); %>
</asp:Content>
<asp:Content ID="ProductContent" ContentPlaceHolderID="MainContent" runat="server">
    <% var moneyFormat = "#,###.00"; %>

    <script type="text/javascript">
        var autoFloatingTimer;
        $(document).ready(function () {
            LoadBanner('banners');
            $("#item_<%= Model.departmentTitle %>").addClass("active");

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

            $(".gridItem, .gridItem_v").live('click', function () {
                LoadProductDetail($(this).prev().val(), '0');
            });
        });

        $('.loginbox a').live('click', function () {
        	var goAjax = true;
        	var $this = $(this);
        	var url = '';
        	var _redirect = ''
        	$('.LoginLoading').show();

        	//            alert($this.attr('class'));
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

        function enlarge(ctr) {
            var gridItem = $('#' + ctr.id);
            var className =gridItem.attr('class');
            
            
            if(gridItem.hasClass('gridItem'))
            {
                //grid
                gridItem.toggleClass('gridItem_on');
            }
            else
            {
                //list
                gridItem.toggleClass('gridItem_v_on');
            }
        }
        function displaySwitch(type)
        {
            var grid = $(".topic .grid");
            var list = $(".topic .list");
            if(type == 'grid')
            {
                if(grid.hasClass('grid_on'))
                    grid.removeClass('grid_on');
                else
                    grid.addClass('grid_on');
            }
            else
            {
                if(list.hasClass('list_on'))
                    list.removeClass('list_on');
                else
                    list.addClass('list_on');   
            }
        }
        function changeDisplay(ctr, id)
        {
            var source = $('#' + ctr.id);
            if(source.hasClass('listview'))
            {
                $('#' + id + ' .gridItem').removeClass('gridItem').addClass('gridItem_v');
            }
            else
            {
                $('#' + id + ' .gridItem_v').removeClass('gridItem_v').addClass('gridItem');
            }
        }
        function LoadBanner(ctr)
        {
            $('#' + ctr + ' .banner:gt(0) ').css("display","none");
            $('#' + ctr + ' ul li a').mouseover(function(){
                var index = $('#' + ctr + ' ul li a').index(this);
                $('#' + ctr + ' .banner').css("display","none");
                $('#' + ctr + ' .banner:eq(' + index+ ') ').fadeIn('slow');
            });
        }
        function Clear() {
            clearInterval(autoFloatingTimer);
            $("#FloatingPanel").hide();
            $("#FloatingPanel").dialog("close");
        }
        function ClearRedirect(html) {
            clearInterval(autoFloatingTimer);
            $("#FloatingPanel").html(html);
        }
    </script>
    <div id="FloatingPanel"></div>
    <div id="SuccessFBRegister"></div>
    <input type="hidden" id="current" />
    <input type="hidden" id="text" />
    <div id="DisplayArea">
        <% Html.RenderPartial("PartialViews/ProductsArea", Model); %>        
    </div>
</asp:Content>
