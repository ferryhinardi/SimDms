$(document).ready(function () {
    var options = {
        title: "Monitoring Productivity",
        xtype: "panels",
        toolbars: [
            { name: "btnExportExcel", text: "Generate Excel", icon: "fa fa-file-excel-o", cls: "small" },
        ],
        panels: [
            {
                name: "panelFilter",
                items: [
                    { name: "CompanyCode", type: "select", cls: "span5 full", text: "Dealer" },
                    { name: "BranchCode", type: "select", cls: "span5 full", text: "Outlet", opt_text: "-- SELECT ALL --" },
                    { name: "PeriodDate", type: "datepicker", cls: "span3 full", text: "Period" },
                ]
            }
        ]
    };

    var widget = new SimDms.Widget(options);
    widget.render(renderCallback);


    function renderCallback() {
        initComponent();
        widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/Organizations", params: { comp: $("[name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
    }

    function initComponent() {
        var btnExportExcel = $("#btnExportExcel");
        var comboCompanyCode = $("[name='CompanyCode']");

        btnExportExcel.off();
        btnExportExcel.on("click", function () {
            refreshData();
        });

        comboCompanyCode.off();
        comboCompanyCode.on("change", function () {
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/Branches", params: { id: $("[name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
        });
    }

    function refreshData() {
        var url = "wh.api/inquiry/MonitoringProductivity";
        var params = widget.getForms();

        widget.post(url, params, function (result) {
            if (widget.isNullOrEmpty(result) == false) {
                var html = "";
                var periodDate = params.PeriodDate;
                var dealer = "-";
                var outlet = "-";

                if (!widget.isNullOrEmpty(params.CompanyCode)) {
                    dealer = $("[name='CompanyCode'] option:selected").text();
                }

                if (!widget.isNullOrEmpty(params.BranchCode)) {
                    outlet = $("[name='BranchCode'] option:selected").text();
                }

                if (widget.isNullOrEmpty(periodDate)) {
                    periodDate = moment(new Date()).format("DD MMM YYYY");
                }
                else {
                    periodDate = moment(periodDate).format("DD MMM YYYY");
                }

                html += "<div style='font-weightL bolder; text-align: center; font-weight: bolder; font-size: 1.2em;'>Monitoring Productivity</div>";
                html += "<div style='text-align: center; font-weight: bolder; font-size: 1.2em;'>Periode " + periodDate + "</div>";
                html += "<div>&nbsp;</div>";

                html += "<div>";
                html += "<div style='display: inline: width: 180px;'>Dealer  &nbsp;&nbsp;&nbsp;: " + dealer + "</div>";
                html += "<div style='display: inline: width: 180px;'>Outlet  &nbsp;&nbsp;&nbsp;: " + outlet + "</div>";
                html += "<div style='display: inline: width: 180px;'>Periode : " + periodDate + "</div>";
                html += "</div>";

                html += "<div>&nbsp;</div>";


                html += "<table style='border: 1px solid black;'>";

                html += "<thead>";
                html += "<tr style='background-color: #2BA6CB; font-weight: bolder;'>";
                html += "<td style='border: 1px solid black; width: 100px; text-align: center;'></td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"01\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"02\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"03\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"04\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"05\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"06\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"07\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"08\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"09\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"10\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"11\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"12\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"13\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"14\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"15\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"16\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"17\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"18\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"19\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"20\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"21\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"22\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"23\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"24\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"25\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"26\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"27\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"28\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"29\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"30\"</td>";
                html += "<td style='border: 1px solid black; width: 40px; text-align: center;'>=\"31\"</td>";
                html += "<td style='border: 1px solid black; width: 100px; text-align: center;'></td>";
                html += "</tr>";
                html += "</thead>";

                html += "<tbody>";
                
                $.each(result || [], function (key, val) {
                    html += "<tr style='background-color: #F6F6F6;'>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["INQ"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["01"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["02"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["03"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["04"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["05"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["06"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["07"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["08"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["09"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["10"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["11"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["12"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["13"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["14"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["15"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["16"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["17"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["18"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["19"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["20"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["21"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["22"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["23"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["24"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["25"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["26"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["27"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["28"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["29"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["30"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["31"] + "</td>";
                    html += "<td style='border: 1px solid black; text-align: center;'>" + val["INQ"] + "</td>";
                    html += "</tr>";
                });

                html += "</tbody>";

                html += "</table>";

                window.open('data:application/vnd.ms-excel,' + encodeURIComponent(html));
            }
            else {
                widget.showNotification("Maaf, permintaan anda tidak dapat diproses.");
            }
        });
    }
});