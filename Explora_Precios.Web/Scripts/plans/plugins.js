$(document).ready(function () {

	//Makes the navigation sticky to top of page when page scrolls down
	var msie6 = $.browser == 'msie' && $.browser.version < 7;

	if (!msie6) {
		var top = $('.navigation').offset().top - parseFloat($('.navigation').css('margin-top').replace(/auto/, 0));
		$(window).scroll(function (event) {
			var y = $(this).scrollTop();
			if (y >= top) {
				$('.nav_image').addClass('fixed');
				$('.nav_image').show();
				$('.navigation').addClass('fixed');
				$('.slidey').addClass('nudge');
			} else {
				$('.nav_image').removeClass('fixed');
				$('.nav_image').hide();
				$('.navigation').removeClass('fixed');
				$('.slidey').removeClass('nudge');
			}
		});
	}

	// Makes buttons look nice
	$("#divButton, #linkButton").button();
	$("#divButton").click(function () {
		$("#contactForm").submit();
	});

	$(".radio").buttonset();

	// Smooth Scrolling Buttons
	$("#radio1").click(function () {
		$.scrollTo("#home", 1000);
	});
	$("#radio2").click(function () {
		$.scrollTo("#servicios", 800, {
			offset: -111
		});
	});
	$("#radio3").click(function () {
		$.scrollTo(".efectividad", 1000, {
			offset: -111
		});
	});
	$("#radio4").click(function () {
		$.scrollTo(".planes", 1000, {
			offset: -111
		});
	});

});