var rb = 1;
var whereParam = "";

$(document).ready(function () {
    var options = {
        title: "History Kendaraan",
        xtype: "panels",
        toolbars: [
          { name: "btnProcess", text: "Print", icon: "icon-print" }
        ],
        panels: [
            {
                title: "History Kendaraan",
                name: "perincianintensif",
                items: [
                    { name: "Year", required: true, cls: "span4 full", text: "Periode tahun" }
                ],
            },
            {
                title: "Filter & Sort",
                name: "Filter",
                items: [
                    { name: "lblFilter", type: "label", text: "Filter", cls: "span4 lbl" },
                    { name: "lblSort", type: "label", text: "Sort", cls: "span4 lbl" },
                    { name: "PoliceRegNo", cls: "span4", text: "No. Polisi" },
                    { name: "rbPoliceRegNo", cls: "span2", type: "switch", text: "No. Polisi" },
                    { name: "BasicModel", cls: "span4", text: "Basic Model" },
                    { name: "rbBasicModel", cls: "span2", type: "switch", text: "Basic Model" },
                    { name: "ChassisNo", cls: "span4", text: "No. Rangka" },
                    { name: "rbChassisNo", cls: "span2", type: "switch", text: "No. Rangka" },
                    { name: "EngineNo", cls: "span4", text: "No. Mesin" },
                    { name: "rbEngineNo", cls: "span2", type: "switch", text: "No. Mesin" },
                    { name: "CustomerName", cls: "span4", text: "Nama Pelanggan" },
                    { name: "rbCustomerName", cls: "span2", type: "switch", text: "Nama Pelanggan" }
                ]
            },
        ]
    }

    var widget = new SimDms.Widget(options);
    widget.render(function () {
        var date1 = new Date();
        var year = date1.getFullYear();
        var data = {
            Year: year,
            rbPoliceRegNo: true
        };
        widget.populate(data);
    });

    //$('#PoliceNo,#BasicModel,#RangkaNo,#MesinNo,#PelangganName').attr("ReadOnly");
    $("#btnProcess").on("click", function () {
        var polreg = "";
        var basmod = "";
        var chasno = "";
        var engno = "";
        var cusname = "";

        if ($("#PoliceRegNo").val() != "") {
            polreg = "#Temp1.PoliceRegNo like ''%" + $("#PoliceRegNo").val() + "%'' and ";
        }
        if ($("#BasicModel").val() != "") {
            basmod = "#Temp1.BasicModel like ''%" + $("#BasicModel").val() + "%'' and ";
        }
        if ($("#ChassisNo").val() != "") {
            chasno = "#Temp1.ChassisNo like ''%" + $("#ChassisNo").val() + "%'' and ";
        }
        if ($("#EngineNo").val() != "") {
            engno = "#Temp1.EngineNo like ''%" + $("#EngineNo").val() + "%'' and ";
        }
        if ($("#CustomerName").val() != "") {
            cusname = "#Temp1.CustomerName like ''%" + $("#CustomerName").val() + "%''";
        }

        whereParam = "where " + polreg + basmod + chasno + engno + cusname;
        console.log(whereParam);

        showReport();
    });

    function showReport() {
        var year = $('#Year').val();
        var par = "producttype" + "," + year + "," + rb + "," + "";
        var ReportType = 'SvRpReport012';

        widget.showPdfReport({
            id: ReportType,
            pparam: par,
            rparam: "Print SvRpReport012",
            type: "devex"
        });
    }

    $('input[type="radio"]').on('change', function (e) {
        var polreg = false;
        var basmod = false;
        var chasno = false;
        var engno = false;
        var cusname = false;
        switch (e.currentTarget.name) {
            case "rbPoliceRegNo":
                polreg = true;
                rb = 1;
                break;
            case "rbBasicModel":
                basmod = true;
                rb = 2;
                break;
            case "rbChassisNo":
                chasno = true;
                rb = 3;
                break;
            case "rbEngineNo":
                engno = true;
                rb = 4;
                break;
            case "rbCustomerName":
                cusname = true;
                rb = 5;
                break;
            default:
                break;
        }
        var data = {
            rbPoliceRegNo: polreg,
            rbBasicModel: basmod,
            rbChassisNo: chasno,
            rbEngineNo: engno,
            rbCustomerName: cusname
        };
        widget.populate(data);
    });

    $('.lbl').css(
       {
           "font-size": "28px",
           "font-weight": "bold",
           "margin-bottom": "25px",
       });
});