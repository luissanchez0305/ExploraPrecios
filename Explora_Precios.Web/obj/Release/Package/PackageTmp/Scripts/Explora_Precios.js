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

$(document).ready(function (){
    $(".logo").click(function(){    
        window.location.href = "/Home";
    });
    $(".searchbox_btn").click(function(){
        var search_txt = $(".searchbox_textbox").val();        
        var department_Id = $("#departmentId").val();
        window.location.href = "/Home/Search?s=" + search_txt + "&d=" + department_Id;
    
    });
    $("#search_text").bind("keydown", function(event){  
        if(event.which == 13){
            $(this).next().click();
        }
    });
    $(".CBupdate").change(function(){
        if($(this).val() == "on"){
            $(this).next().val("True");
        }
        else {
            $(this).next().val("False");
        }
    });
    $(".ddlqualitychange").change(function(){
        $(this).next().val($(this).val());
        if($(this).val() == "new") {
           $(this).next().val(""); 
        }
    });
    $("#addQuality").live("click", function()
    {
        var qName = $("#qualityName").val();
        var qVal = $("#qualityValue").val();
        if(qName.length > 1 && qVal.length > 1)
        {
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
                        .attr("type","hidden")
                        .attr("value","0")
                        .attr("name","qualities["+rowcount+"].Id")
                        .attr("id","qualities["+rowcount+"].Id")
                    )
                    .append($("<input>")
                        .attr("type","text")
                        .attr("value",qName)
                        .attr("name","qualities["+rowcount+"].name")
                        .attr("id","qualities["+rowcount+"].name")
                    )
                )
                .append($("<td>")
                    .append($("<input>")
                        .attr("type","text")
                        .attr("value",qVal)
                        .attr("name","qualities["+rowcount+"].value")
                        .attr("id","qualities["+rowcount+"].value")
                    )
                )
                .append($("<td>")
                    .append($("<input>")
                        .attr("type","checkbox")
                        .attr("class","CBUpdate")
                        .attr("checked","checked")
                    )
                    .append($("<input>")
                        .attr("type","hidden")
                        .attr("name","qualities["+rowcount+"].active")
                        .attr("id","qualities["+rowcount+"].active")
                        .attr("value","True")
                    )
                )
            );
        }
    });
    
    $("#assignClientPrice").live("click",function(){
        var clientId = $("#Clients").val();
        var clientName = $("#Clients option:selected").text()
        var price = $("#clientPrice").val();
        var url = $("#clientProductUrl").val();
        if(price.length > 0)
        {
            var rowcount = $("#clientPriceTable tr").length;
            $("#clientPriceTable").find("tbody")
                .append($("<tr>")
                    .append($("<td>")
                        .append($("<input>")
                            .attr("type","hidden")
                            .attr("value",clientId)
                            .attr("name","clientList["+rowcount+"].clientId")
                            .attr("id","clientList["+rowcount+"].clientId")                                
                        )
                        .append(clientName)
                    )
                    .append($("<td>")
                        .append($("<input>")
                            .attr("type","text")
                            .attr("value",price)
                            .attr("name","clientList["+rowcount+"].price")
                            .attr("id","clientList["+rowcount+"].price")                                      
                        )
                    )
                    .append($("<td>")
                        .append($("<input>")
                            .attr("type","text")
                            .attr("value",url)
                            .attr("name","clientList["+rowcount+"].url")
                            .attr("id","clientList["+rowcount+"].url")                                      
                        )
                    )
                    .append($("<td>")
                        .append($("<input>")
                            .attr("checked","checked")
                            .attr("value","True")
                            .attr("type","checkbox")
                            .attr("name","clientList["+rowcount+"].isActive")
                            .attr("id","clientList["+rowcount+"].isActive")                                      
                        )
                        .append($("<input>")
                            .attr("value","True")
                            .attr("type","hidden")
                            .attr("name","clientList["+rowcount+"].isActive")                                   
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
    function SaveProductAjax()
    {    
        $("#loading_image").css("display","inline");
        $.ajax({
            data: $("#mngProductForm").serialize(),
            dataType: "json",
            url: "/ProductManager/DoCreateProduct",
            type: "POST",
            error: function() {
                alert("ajax error");
                $("#success").css("display","none");
                $("#loading_image").css("display","none");   
            },            
            success: function(jObject) {
                if(jObject.result == "fail") {
                    alert("error");
                    $("#success").css("display","none");
                    $("#loading_image").css("display","none");   
                }
                else {
                    $("#productContainer").html(jObject.html);
                    $("#success").css("display","inline");
                    $("#loading_image").css("display","none");   
                }
            }
        });
    }