/* Menu JSMenu */
function LoadJSMenu() {
	//<![CDATA[

	// For each menu you create, you must create a matching "FSMenu" JavaScript object to represent
	// it and manage its behaviour. You don't have to edit this script at all if you don't want to;
	// these comments are just here for completeness. Also, feel free to paste this script into the
	// external .JS file to make including it in your pages easier!

	// Here's a menu object to control the above list of menu data:
	var listMenu = new FSMenu('listMenu', true, 'display', 'block', 'none');

	// The parameters of the FSMenu object are:
	//  1) Its own name in quotes.
	//  2) Whether this is a nested list menu or not (in this case, true means yes).
	//  3) The CSS property name to change when menus are shown and hidden.
	//  4) The visible value of that CSS property.
	//  5) The hidden value of that CSS property.
	//
	// Next, here's some optional settings for delays and highlighting:
	//  * showDelay is the time (in milliseconds) to display a new child menu.
	//    Remember that 1000 milliseconds = 1 second.
	//  * switchDelay is the time to switch from one child menu to another child menu.
	//    Set this higher and point at 2 neighbouring items to see what it does.
	//  * hideDelay is the time it takes for a menu to hide after mouseout.
	//    Set this to a negative number to disable hiding entirely.
	//  * cssLitClass is the CSS classname applied to parent items of active menus.
	//  * showOnClick will, suprisingly, set the menus to show on click. Pick one of 4 values:
	//     0 = All levels show on mouseover.
	//     1 = Menu activates on click of first level, then shows on mouseover.
	//     2 = All levels activate on click, then shows on mouseover.
	//     3 = All levels show on click only (no mouseover at all).
	//  * hideOnClick hides all visible menus when one is clicked (defaults to true).
	//  * animInSpeed and animOutSpeed set the animation speed. Set to a number
	//    between 0 and 1 where higher = faster. Setting both to 1 disables animation.

	listMenu.showDelay = 2;
	//listMenu.switchDelay = 125;
	//listMenu.hideDelay = 500;
	//listMenu.cssLitClass = 'highlighted';
	//listMenu.showOnClick = 0;
	//listMenu.hideOnClick = true;
	//listMenu.animInSpeed = 0.2;
	//listMenu.animOutSpeed = 0.2;

	// Now the fun part... animation! This script supports animation plugins you
	// can add to each menu object you create. I have provided 3 to get you started.
	// To enable animation, add one or more functions to the menuObject.animations
	// array; available animations are:
	//  * FSMenu.animSwipeDown is a "swipe" animation that sweeps the menu down.
	//  * FSMenu.animFade is an alpha fading animation using tranparency.
	//  * FSMenu.animClipDown is a "blind" animation similar to 'Swipe'.
	// They are listed inside the "fsmenu.js" file for you to modify and extend :).

	// I'm applying two at once to listMenu. Delete this to disable!
	listMenu.animations[listMenu.animations.length] = FSMenu.animFade;
	listMenu.animations[listMenu.animations.length] = FSMenu.animSwipeDown;
	//listMenu.animations[listMenu.animations.length] = FSMenu.animClipDown;


	// Finally, on page load you have to activate the menu by calling its 'activateMenu()' method.
	// I've provided an "addEvent" method that lets you easily run page events across browsers.
	// You pass the activateMenu() function two parameters:
	//  (1) The ID of the outermost <ul> list tag containing your menu data.
	//  (2) A node containing your submenu popout arrow indicator.
	// If none of that made sense, just cut and paste this next bit for each menu you create.

	var arrow = null;
	if (document.createElement && document.documentElement) {
		arrow = document.createElement('span');
		arrow.appendChild(document.createTextNode('>'));
		// Feel free to replace the above two lines with these for a small arrow image...
		//arrow = document.createElement('img');
		//arrow.src = 'arrow.gif';
		//arrow.style.borderWidth = '0';
		arrow.className = 'subind';
	}
	addReadyEvent(new Function('listMenu.activateMenu("listMenuRoot", arrow)'));

	// Helps with swapping background images on mouseover in IE. Not needed otherwise.
	//if (document.execCommand) document.execCommand("BackgroundImageCache", false, true);

	// You may wish to leave your menu as a visible list initially, then apply its style
	// dynamically on activation for better accessibility. Screenreaders and older browsers will
	// then see all your menu data, but there will be a 'flicker' of the raw list before the
	// page has completely loaded. If you want to do this, remove the CLASS="..." attribute from
	// the above outermost UL tag, and uncomment this line:
	//addReadyEvent(new Function('getRef("listMenuRoot").className="menulist"'));


	// TO CREATE MULTIPLE MENUS:
	// 1) Duplicate the <ul> menu data and this <script> element.
	// 2) In the <ul> change id="listMenuRoot" to id="otherMenuRoot".
	// 3) In the <script> change each instance of "listMenu" to "otherMenu"
	// 4) In the addReadyEvent line above ensure "otherMenuRoot" is activated.
	// Repeat as necessary with a unique name for each menu you want.
	// You can also give each a unique CLASS and apply multiple stylesheets
	// for different menu appearances/layouts, consult a CSS reference on this.
	//]]>
}

function runDropDownAjax(source_singular, source_plural, target_singular, target_plural){

	var val = $("#" + source_plural + " > option:selected").attr("value");
		
	$.ajax({
		type: "GET",
		contentType: "application/json; charset=utf-8",
		url: "Find"+target_singular+"By"+source_singular+"Id/" + val,
		data: "{}",
		dataType: "json",
		success: function(data){
			if(data.length > 1){
				var options = '';
				for (p in data){
					var data_pair = data[p];
					options += "<option value='" + data_pair.Id + "'>" + data_pair.Name + "</option>";
				}
				$("#" + target_plural).removeAttr('disabled').html(options);
			}
			else {
				$("#"+target_plural).attr('disabled', true).html('');
				$("#"+target_singular+"Div").append('<div>No contiene '+target_plural+'</div>');
			}
		}
	});
} 
/* Ready general */
$(document).ready(function () {

	$(".logo").click(function () {
		window.location.href = "/Home";
	});

	$("#pageContentManagement a").live("mouseover", function () { $(this).addClass("hoverMe"); })
	.live("mouseout", function () { $(this).removeClass("hoverMe"); });

	$(".searchbox_btn").click(function () {
		var search_txt = $(".searchbox_textbox").val();
		var department_Id = $("#departmentId").hasClass('department') ? $("#departmentId").val() : 1;
		window.location.href = "/Home/Search?s=" + search_txt;
	});

	$("#search_text").bind("keydown", function (event) {
		if (event.which == 13) {
			$(this).next().click();
		}
	});

	$(".CBupdate").change(function () {
		if ($(this).val() == "on") {
			$(this).next().val("True");
		}
		else {
			$(this).next().val("False");
		}
	});

	$(".ddlqualitychange").change(function () {
		$(this).next().val($(this).val());
		if ($(this).val() == "new") {
			$(this).next().val("");
		}
	});

	$("#addQuality").live("click", function () {
		var qName = $("#qualityName").val();
		var qVal = $("#qualityValue").val();
		$("#qualityName").val("");
		$("#qualityValue").val("");
		if (qName.length > 1 && qVal.length > 1) {
			$("#noQualities").remove();
			var rowcount = $("#tableQualities tr").length;
			/*$.ajax({                
			type: "GET",
			contentType: "application/json; charset=utf-8",
			url: "FindQualityName?v=" + val,
			data: "{}",
			dataType: "json",
			success: function(data){
			data.qId
			}
			});*/

			// Nuevo row de caracteristica
			$("#tableQualities").find("tbody")
			.append($("<tr>")
				.append($("<td>")
					.append($("<input>")
						.attr("type", "hidden")
						.attr("value", "0")
						.attr("name", "qualities[" + rowcount + "].Id")
						.attr("id", "qualities[" + rowcount + "].Id")
					)
					.append($("<input>")
						.attr("type", "text")
						.attr("value", qName)
						.attr("name", "qualities[" + rowcount + "].name")
						.attr("id", "qualities[" + rowcount + "].name")
						.attr("style", "width:80px")
					)
				)
				.append($("<td>")
					.append($("<input>")
						.attr("type", "text")
						.attr("value", qVal)
						.attr("name", "qualities[" + rowcount + "].value")
						.attr("id", "qualities[" + rowcount + "].value")
					)
				)
				.append($("<td>")
					.append($("<input>")
						.attr("type", "checkbox")
						.attr("class", "CBUpdate")
						.attr("checked", "checked")
					)
					.append($("<input>")
						.attr("type", "hidden")
						.attr("name", "qualities[" + rowcount + "].active")
						.attr("id", "qualities[" + rowcount + "].active")
						.attr("value", "True")
					)
				)
			);
		}
	});

	$("#assignClientPrice").live("click", function () {
		var clientId = $("#Clients").val();
		var clientName = $("#Clients option:selected").text()
		var price = $("#clientPrice").val();
		var url = $("#clientProductUrl").val();
		var reference = $("#clientProductReference").val();
		$("#clientPrice").val("");
		$("#clientProductUrl").val("");
		$("#clientProductReference").val("");
		if (price.length > 0) {
			var rowcount = $("#clientPriceTable tr").length - 2;
			$("#clientPriceTable").find("tbody")
				.append($("<tr>")
					.append($("<td>")
						.append($("<input>")
							.attr("type", "hidden")
							.attr("value", clientId)
							.attr("name", "clientList[" + rowcount + "].clientId")
							.attr("id", "clientList[" + rowcount + "].clientId")
						)
						.append(clientName)
					)
					.append($("<td>")
						.append($("<input>")
							.attr("type", "text")
							.attr("value", price)
							.attr("name", "clientList[" + rowcount + "].price")
							.attr("id", "clientList[" + rowcount + "].price")
							.attr("class", "price")
						)
					)
					.append($("<td>")
						.append($("<input>")
							.attr("type", "text")
							.attr("value", url)
							.attr("name", "clientList[" + rowcount + "].url")
							.attr("id", "clientList[" + rowcount + "].url")
							.attr("class", "url")
						)
					)
					.append($("<td>")
						.append($("<input>")
							.attr("type", "text")
							.attr("value", reference)
							.attr("name", "clientList[" + rowcount + "].reference")
							.attr("id", "clientList[" + rowcount + "].reference")
							.attr("class", "reference")
						)
					)
					.append($("<td>")
						.append($("<input>")
							.attr("checked", "checked")
							.attr("value", "True")
							.attr("type", "checkbox")
							.attr("name", "clientList[" + rowcount + "].isActive")
							.attr("id", "clientList[" + rowcount + "].isActive")
						)
						.append($("<input>")
							.attr("value", "True")
							.attr("type", "hidden")
							.attr("name", "clientList[" + rowcount + "].isActive")
						)
					)
				)
		}
	});
});    

	function LoadProductAjax(url, ref)
	{    
		var dataArray = {};
		if(ref.length > 0)
			dataArray = {param:ref};
		$("#loading_image").css("display","inline");
		$.ajax({
			data: (dataArray),
			url: url,
			dataType: "json",
			error: function(e) {
				alert("error: corriendo url " + e);  
			},
			success: function(jObject){
				if(jObject.result == "fail")
				{
					alert("error: " + jObject.msg);
					$("#loading_image").css("display","none");
				}
				else
				{
					if(jObject.found)                      
					{
						$("#productContainer").html(jObject.html);  
						$("#notfound").css("display","none");
						//$("#success").css("display","inline");
						$("#loading_image").css("display","none");
					}            
					else                     
					{    
						$("#notfound").css("display","inline");  
						$("#success").css("display","none"); 
						$("#loading_image").css("display","none");   
					}
				}
			}
		});
	}
	function SaveProductAjax() {
		$("#loading_image").css("display", "inline");
		$.ajax({
			data: $("#mngProductForm").serialize(),
			dataType: "json",
			url: "/ProductManager/DoCreateProduct",
			type: "POST",
			error: function () {
				alert("ajax error");
				$("#success").css("display", "none");
				$("#loading_image").css("display", "none");
			},
			success: function (jObject) {
				if (jObject.result == "fail") {
					alert("error");
					$("#success").css("display", "none");
					$("#loading_image").css("display", "none");
				}
				else {
					$("#productContainer").html(jObject.html);
					$("#success").css("display", "inline");
					$("#loading_image").css("display", "none");
				}
			}
		});
	}

	function getParameterByName(name) {
		name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
		var regexS = "[\\?&]" + name + "=([^&#]*)";
		var regex = new RegExp(regexS);
		var results = regex.exec(window.location.search);
		if (results == null)
			return "";
		else
			return decodeURIComponent(results[1].replace(/\+/g, " "));
	}