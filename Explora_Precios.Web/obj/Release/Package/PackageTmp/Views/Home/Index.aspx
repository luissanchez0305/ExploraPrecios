<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Explora_Precios.Web.Controllers.ViewModels.HomeViewModel>" %>

<asp:Content ID="HeaderTitleContent" ContentPlaceHolderID="TitleContent" runat="server">
ExploraPrecios.com
</asp:Content>
<asp:Content ID="LeftMenuContent" ContentPlaceHolderID="LeftContent" runat="server">
<% if (Boolean.Parse(System.Configuration.ConfigurationManager.AppSettings["GoProductManager"])) {  %>
<script type="text/javascript">
    //document.location.href = "/ProductManager";
</script>
<%} %>
<% Html.RenderPartial("LeftMenu", Model); %>
</asp:Content>
<asp:Content ID="ProductContent" ContentPlaceHolderID="MainContent" runat="server">
    <% var moneyFormat = "#,###.00"; %>

    <script type="text/javascript">
        $(document).ready(function(){
             LoadBanner('banners');
             $("#ProductDetails").dialog({
                autoOpen: false,
                bgiframe: false,
                modal: true,    
                draggable: false,
                height: 400,
                width: 600,
                resizable: false    
             });
             $("#ProductDetails").hide();
             $("#ProductDetails").dialog("close");
             $("#ProductDetailsClose").live("click", function(){
                 $("#ProductDetails").hide();
                 $("#ProductDetails").dialog("close"); 
                 $.unblockUI();
             });
             $(".gridItem").click(function(){
                var val = $(this).prev().val();
                $.blockUI({ message: '<h4>Un momento por favor...</h4>' });
                $.ajax({                        
                    type: "GET",
                    url: '<%= Url.Action("ProductDisplay", "Home") %>',
                    data: { _valId: val },
                    dataType: "json",
                    error: function(x,e) {
                        //alert(e);
                        $.unblockUI();
                    },
                    success: function(data){
                        if(data.result == "fail") {
                            alert(data.msg);
                            $.unblockUI();
                        }
                        else {   
                            $("#ProductDetails").show();
                            $("#ProductDetails").dialog("open");//, "title", data.msg);
                            $("#ProductDetails").data("title.dialog", data.msg);
                            $("#ProductDetails").html(data.html);
                            $.unblockUI();
                        }
                    }
                });
             });
             $("#item_<%= Model.departmentTitle %>").addClass("active");
        });
        function enlarge(ctr)
        {
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
    </script>
    <div id="ProductDetails"></div>
    <div id="DisplayArea">
        <% Html.RenderPartial("ProductList", Model); %>        
    </div>
</asp:Content>
