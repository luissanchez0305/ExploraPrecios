<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<script charset="utf-8" src="http://widgets.twimg.com/j/2/widget.js"></script>
<script>
	new TWTR.Widget({
		version: 2,
		type: 'profile',
		rpp: 6,
		interval: 30000,
		width: 220,
		height: 500,
		theme: {
			shell: {
				background: '#A7C2CF',
				color: '#292710'
			},
			tweets: {
				background: '#ffffff',
				color: '#574e44',
				links: '#a88962'
			}
		},
		features: {
			scrollbar: false,
			loop: true,
			live: true,
			behavior: 'all'
		}
	}).render().setUser('exploraprecios').start();
</script>
<div style="width: 200px; padding-top:10px;">
<script type="text/javascript"><!--
	google_ad_client = "ca-pub-8106411801578478";
	/* 200x200 Small Left */
	google_ad_slot = "5459383721";
	google_ad_width = 200;
	google_ad_height = 200;
//-->
</script>
</div>
<script type="text/javascript"
src="http://pagead2.googlesyndication.com/pagead/show_ads.js">
</script>
