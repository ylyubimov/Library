$(document).ready(function () {
    $("#PublisherId").change(function(){
        if ($("#PublisherId").val() == '')
            $("#newPublisher").show();
        else
            $("#newPublisher").hide();
    })
})