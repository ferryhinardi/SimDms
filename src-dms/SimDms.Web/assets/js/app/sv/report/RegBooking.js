$(document).ready(function () {
    var options = {
        title: "Register Booking / Estimasi",
        xtype: "panels",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" }
        ],
        panels: [
           {
               title: "Contact Information",
               name: "contactInformation",
               items: [
                   { name: "DateFrom", required: true, cls: "span4", text: "Periode Tanggal", type: "kdatepicker", required: true },
                   { name: "DateTo", text: "s/d", cls: "span4", type: "kdatepicker", required: true },
                   { name: "Status", required: true, cls: "span4", text: "Status", type: "select", items: [{ value: "BOK", text: "Booking" }, { value: "EST", text: "Estimasi" }] },
                   {
                       name: "Sort", required: true, cls: "span4", text: "Sort", type: "select", items: [
                         { value: "PoliceRegNo", text: "No Polisi" },
                         { value: "BasicModel", text: "Basic Model" },
                         { value: "Chassis", text: "No Rangka" },
                         { value: "Engine", text: "No Mesin" },
                         { value: "CustomerFullName", text: "Nama Pelanggan" },
                         { value: "DocumentDate", text: "Tanggal" }]
                   },
                   { name: "Police", text: "No Polisi", cls: "span2", type: "switch"},
                   { name: "PoliceNo", cls: "span2" },
                   { name: "Basic", text: "Basic Model", cls: "span2", type: "switch"},
                   { name: "BasicModel", cls: "span2" },
                   { name: "Rangka", text: "No Rangka", cls: "span2", type: "switch"},
                   { name: "RangkaNo", cls: "span2" },
                   { name: "Mesin", text: "Mesin", cls: "span2", type: "switch"},
                   { name: "MesinNo", cls: "span2" },
                   { name: "Pelanggan", text: "Nama Pelanggan", cls: "span2", type: "switch"},
                   { name: "PelangganName", cls: "span2" },
               ],
           }]
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        //$(".frame").css({ top: 230 });
        //$(".panel").css({ 'max-width': 1300 });
        $('#PoliceNo,#BasicModel,#RangkaNo,#MesinNo,#PelangganName').attr("ReadOnly", "ReadOnly");
        var dateFrom = $('input[name="DateFrom"]').val();
        var dateTo = $('input[name="DateTo"]').val();

        var dt = new Date()
        var date1 = new Date(dt.getFullYear(), dt.getMonth()+1, 0);
        var date2 = new Date(dt.getFullYear(), dt.getMonth(), 1);
        widget.populate({ DateFrom: date2, DateTo: date1 });
    });

    //$('#PoliceNo,#BasicModel,#RangkaNo,#MesinNo,#PelangganName').attr("ReadOnly");
    $("#btnProcess").on("click", function () { showReport(); });
    $('input[type="radio"]').on('change', function (e) {
        console.log($('#PoliceNo').attr("ReadOnly"));
        switch (e.currentTarget.name) {
            case "Police":
                if ($('#PoliceNo').attr("readonly") == "readonly") {
                    $('#PoliceNo').removeAttr("ReadOnly");
                }
                else {
                    $('#PoliceNo').val("");
                    $('#PoliceNo').attr("ReadOnly","ReadOnly");
                }
                break;
            case "Basic":
                if ($('#BasicModel').attr("readonly") == "readonly") {
                    $('#BasicModel').removeAttr("ReadOnly");
                }
                else {
                    $('#BasicModel').val("");
                    $('#BasicModel').attr("ReadOnly", "ReadOnly");
                }
                break;
            case "Rangka":
                if ($('#RangkaNo').attr("readonly") == "readonly") {
                    $('#RangkaNo').removeAttr("ReadOnly");
                }
                else {
                    $('#RangkaNo').val("");
                    $('#RangkaNo').attr("ReadOnly", "ReadOnly");
                }
                break;
            case "Mesin":
                if ($('#MesinNo').attr("readonly") == "readonly") {
                    $('#MesinNo').removeAttr("ReadOnly");
                }
                else {
                    $('#MesinNo').val("");
                    $('#MesinNo').attr("ReadOnly", "ReadOnly");
                }
                break;
            case "Pelanggan":
                if ($('#PelangganName').attr("readonly") == "readonly") {
                    $('#PelangganName').removeAttr("ReadOnly");
                }
                else {
                    $('#PelangganName').val("");
                    $('#PelangganName').attr("ReadOnly", "ReadOnly");
                }
                break;
            default:
                alert("Default");
                break;

        }
        e.preventDefault();
    });

    $('#btnProcess').click(function () {
        var valid = $(".main form").valid();
        if (valid) {
            showReport();
        }
    });
    function showReport() {
        var dateFrom = $('[name="DateFrom"]').val();
        var dateTo = $('[name="DateTo"]').val();
        var status = $('#Status').val();
        var sort = $('#Sort').val();
        var policeNo = $('#PoliceNo').val();
        var rangkaNo = $('#RangkaNo').val();
        var pelangganName = $('#PelangganName').val();
        var basicModel = $('#BasicModel').val();
        var mesinNo = $('#MesinNo').val();
        var data = status+","+policeNo+","+basicModel+","+rangkaNo+","+mesinNo+","+pelangganName+","+dateFrom+","+dateTo+","+sort;
        widget.showPdfReport({
            id: "SvRpReport013",
            pparam: data,
            rparam: "admin",
            type: "devex"
        });
        //widget.showReport({
        //    id: "SvRpReport013",
        //    type: "devex",
        //    par: data
        //});
    }
});