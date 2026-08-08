
// JavaScript source code
$("#filteraccount").on("click", function () {

    if ($("#fromdate").val() == "" || $("#todate").val() == "") {
        alert("Please select date filter");
    } else {
        $('#overlay').show();
        $.ajax({
            type: 'GET',
            cache: false,
            url: '/CustomerReport/FilterAccountToAccountReport',
            dataType: 'json',
            data: {
                branch_code: $("#branchfilter").val(),
                status: $("#statusfilter").val(),
                fromdate: $("#fromdate").val(),
                todate: $("#todate").val(),
                inc: $("#inc").val()
            },
            contentType: 'application/json',
            success: function (result) {
                console.log(result);
                $("#example").dataTable().fnClearTable();
                $.each(result, function (status, data) {
                    $.each(data, function (innerstatus, innerdata) {
                        var data = [];
                        data.push(innerdata.alsocustomername);
                        data.push(innerdata.TranToAccount);
                        data.push(innerdata.TranReqAmount);
                        data.push(innerdata.CustomerName);
                        data.push(innerdata.TranStatus);
                        data.push(innerdata.FT);
                        data.push(innerdata.TranDate);
                        $("#example").dataTable().fnAddData(data);
                    });
                });
                $('#overlay').hide();
            }
        });
    }


});

setInterval(function () { $("#overflow").hide(); }, 10000);

var value = parseInt(document.getElementById('number').value, 10);
value = isNaN(value) ? 0 : value;
value++;
document.getElementById('number').value = value;

var i = 0;
document.getElementById('nextButton').onclick((e) => {
    console.log('click');
});