var droplinemenu={
hasClick:{},
buildmenu:function(menuid){
	jQuery(document).ready(function ($) {   
		var process = "." + menuid.replace(" ",".") + ">li>a";
		var img = $(process).css('background-image');	
		$(process).css('background-image','none');
		
		$(process).parent().has('ul').find('a:first').css('background-image',img);
		
		$($(process).parent()[0]).addClass('selected');
		
		$('#lblMenu').text($($(process).parent()[0]).text().trim());
		
		$(process).unbind('click').click(function (e) {
			
			if ($(this).parent().find('ul').val() == undefined)
			{
				$(process).parent().removeClass('selected');
				droplinemenu.hasClick = 0;
			}			
			else if (($(this).parent().find('ul').attr('style') == undefined) || (droplinemenu.hasClick == 1))
			{
				$(this).parent().find('ul').css('visibility', 'visible');
				droplinemenu.hasClick = 1;
			}			
			if (droplinemenu.hasClick != 1)
			{
				//$(process).parent().removeClass('selected');
				if (droplinemenu.hasClick != 2)
				{
					$(this).parent().addClass('selected');
				}
				$(this).parent().find('ul').css('visibility', 'visible');
				
				droplinemenu.hasClick = 1;
			}
			
        });
		
		$(process).parent().find('ul > li').unbind('click').click(function(e){
			
			$(process).parent().removeClass('selected');
			if (droplinemenu.hasClick == 1)
			{				
				$(this).parent().parent().addClass('selected');
				$(this).parent().css('visibility', 'hidden');
				droplinemenu.hasClick = 2;
				$('#lblMenu').text($($(this)).text().trim());
			}			
		});
		
		$(process).unbind('mouseover').mouseover(function(e){
			
			if ((e.type == "mouseover") && (droplinemenu.hasClick == 1))
			{
					
				$(process).parent().find('ul').css("visibility","hidden");
				$(this).parent().find('ul').css("visibility","visible");
			}
		});
	}) //end document.ready
}
}