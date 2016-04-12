function IntensifController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.initialize = function () {
        me.hasChanged = false;
        me.data.By = "MK";
        me.data.DateFrom = me.now();
        me.data.DateTo = me.now();
        me.data.NIKChk = false;
        me.data.NamaChk = false;
    }

    me.PrintPreview = function () {
        if (me.data.NIKChk == true && me.data.NIK == undefined)
        {
            MsgBox('Silakan Isi NIK', MSG_INFO);
        }

        else if (me.data.NamaChk == true && me.data.Nama == undefined) {
            MsgBox('Silakan Isi Nama', MSG_INFO);
        }
        else {
            var dateFrom = moment(me.data.DateFrom).format('YYYYMMDD');
            var dateTo = moment(me.data.DateTo).format('YYYYMMDD');
            var By = me.data.By;
            var NIK = me.data.NIKChk == false ? "%" : me.data.NIK;
            var Nama = me.data.NamaChk == false ? "%" : me.data.Nama;
            var Type = "REKAP";

            var ReportId = 'SvRpReport010' + By;
            var par = [
                dateFrom, dateTo, NIK, Nama, Type
            ]
            var rparam = 'PERIODE : ' + moment(Date.now()).format('DD-MMMM-YYYY');

            Wx.showPdfReport({
                id: ReportId,
                pparam: par.join(','),
                rparam: rparam,
                type: "devex"
            });
        }
    }

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Perincian Intensif",
        xtype: "panels",
        toolbars: [
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "PrintPreview()" },
        ],
        panels: [
           {
               title: "Perincian Intensif",
               name: "perincianintensif",
               items: [
                   {
                       name: "By", required: true, cls: "span4 full", text: "By", type: "select", items: [
                         { value: "MK", text: "Mekanik" },
                         { value: "SA", text: "SA" },
                         { value: "FM", text: "FM" },
                       ]
                   },
                   { name: "DateFrom", required: true, cls: "span4", text: "Periode Tanggal", type: "ng-datepicker", required: true },
                   { name: "DateTo", text: "s/d", cls: "span4", type: "ng-datepicker", required: true },
                   { name: "NIKChk", model: "data.NIKChk", text: "NIK", cls: "span2", type: "x-switch" },
                   { name: "NIK", cls: "span2", disable: "data.NIKChk == false" },
                   { name: "NamaChk", model: "data.NamaChk", text: "Nama", cls: "span2", type: "x-switch" },
                   { name: "Nama", cls: "span2", disable: "data.NamaChk == false" }
               ],
           }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("IntensifController");
    }

});

//$(document).ready(function () {
//    var options = {
//        title: "Perincian Intensif",
//        xtype: "panels",
//        toolbars: [
//            { name: "btnProcess", text: "Process", icon: "icon-bolt" }
//        ],
//        panels: [
//           {
//               title: "Perincian Intensif",
//               name: "perincianintensif",
//               items: [
//                   {
//                       name: "By", required: true, cls: "span4 full", text: "By", type: "select", items: [
//                         { value: "MK", text: "Mekanik" },
//                         { value: "SA", text: "SA" },
//                         { value: "FM", text: "FM" },
//                       ]
//                   },
//                   { name: "DateFrom", required: true, cls: "span4", text: "Periode Tanggal", type: "kdatepicker", required: true },
//                   { name: "DateTo", text: "s/d", cls: "span4", type: "kdatepicker", required: true },
//                   { name: "NIKChk", text: "NIK", cls: "span2", type: "switch" },
//                   { name: "NIK", cls: "span2" },
//                   { name: "NamaChk", text: "Nama", cls: "span2", type: "switch" },
//                   { name: "Nama", cls: "span2" }
//               ],
//           }]
//    }
//    var widget = new SimDms.Widget(options);
//    widget.render(function () {
//        //$(".frame").css({ top: 230 });
//        //$(".panel").css({ 'max-width': 1300 });
//        $('#NIK,#Nama').attr("ReadOnly", "ReadOnly");
//        var dateFrom = $('input[name="DateFrom"]').val();
//        var dateTo = $('input[name="DateTo"]').val();
//        var date1 = new Date();
//        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);
//        widget.populate({ DateFrom: date2, DateTo: date1 });
//    });

//    //$('#PoliceNo,#BasicModel,#RangkaNo,#MesinNo,#PelangganName').attr("ReadOnly");
//    $("#btnProcess").on("click", function () { showReport(); });
//    $('input[type="radio"]').on('change', function (e) {
//        switch (e.currentTarget.name) {
//            case "NIKChk":
//                if ($('#NIK').attr("readonly") == "readonly") {
//                    $('#NIK').removeAttr("ReadOnly");
//                }
//                else {
//                    $('#NIK').val("");
//                    $('#NIK').attr("ReadOnly", "ReadOnly");
//                }
//                break;
//            case "NamaChk":
//                if ($('#Nama').attr("readonly") == "readonly") {
//                    $('#Nama').removeAttr("ReadOnly");
//                }
//                else {
//                    $('#Nama').val("");
//                    $('#Nama').attr("ReadOnly", "ReadOnly");
//                }
//                break;
//            default:
//                alert("Default");
//                break;

//        }
//        e.preventDefault();
//    });

//    $('#btnProcess').click(function () {
//        var valid = $(".main form").valid();
//        if (valid) {
//            showReport();
//        }
//    });
//    function showReport() {
//        var dateFrom = $('[name="DateFrom"]').val();
//        var dateTo = $('[name="DateTo"]').val();
//        var status = $('#Status').val();
//        var By = $('#By').val();
//        var NIK = ($('input[name="NIKChk"]').val() == "true"? $('#NIK').val() : "%");
//        var Nama = ($('input[name="NamaChk"]').val() == "true" ? $('#Nama').val() : "%");
//        var Type = "REKAP";
//        var data = dateFrom + "," + dateTo + "," + NIK + "," + Nama + "," + Type;
//        var ReportType = 'SvRpReport011' + $('#By').val();
        
//        widget.showPdfReport({
//            id: ReportType,
//            pparam: data,
//            rparam: "admin",
//            type: "devex"
//        });
//        //widget.showReport({
//        //    id: "SvRpReport013",
//        //    type: "devex",
//        //    par: data
//        //});
//    }
//});