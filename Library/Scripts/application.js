﻿$(document).ready(function () {
    if ($("#PublisherId").val() == '')
        $("#newPublisher").show();
    else
        $("#newPublisher").hide();
    $("#PublisherId").change(function () {
        if ($("#PublisherId").val() == '')
            $("#newPublisher").show();
        else
            $("#newPublisher").hide();
    });
    $('input[type=file]').change(function () {
        document.getElementById("test").innerHTML = this.value;
    });
})