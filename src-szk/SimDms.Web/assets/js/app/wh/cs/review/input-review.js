var selectedRow = {};
$(document).ready(function () {
    var options = {
        title: "Review GM",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                title: "Review Activity",
                items: [
                    { name: "GroupArea", text: "Area", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "CompanyCode", text: "Dealer", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "BranchCode", text: "Outlet", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: 'Plan', text: 'Activity', type: 'select', cls: 'span6' },
                    {
                        name: "pnlFilter", text: "Input Date", cls: 'flt', type: "controls", items: [
                            { name: "DateFrom", text: "Date From", cls: "span2", type: "datepicker" },
                            { name: "DateTo", text: "Date To", cls: "span2", type: "datepicker" },
                        ]
                    },
                    {
                        name: "ParStatus", text: "Status", type: "select", cls: "span3 flt2", items: [
                            { value: "0", text: "All Status" },
                            { value: "1", text: "Not Inputted" },
                            { value: "2", text: "Inputted" },
                        ],
                        fullItem: true
                    },
                    { name: "PeriodYear", text: "Year", type: "select", cls: "span3 flt2" },
                    { name: "ParMonth1", text: "Month From", type: "select", cls: "span3 flt2" },
                    { name: "ParMonth2", text: "Month To", type: "select", cls: "span3 flt2" },
                ]
            },
            { name: 'pnlData', xtype: 'k-grid', },
            {
                title: "Review Details", name: "pnlReviewDetail", cls: "hide",
                items: [
                    { name: 'InputFrom', text: 'Date From', type: 'datepicker', cls: 'span3', disabled: true },
                    { name: 'InputTo', text: 'Date To', type: 'datepicker', cls: 'span3', disabled: true },
                    { name: 'Check', text: 'Check', type: 'textarea', cls: 'span6', readonly: true },
                    { name: 'Action', text: 'Action', type: 'textarea', cls: 'span6', readonly: true },
                    { name: 'PIC', text: 'PIC', type: 'select', cls: 'span6', disabled: true },
                    { name: 'CommentbyGM', text: 'Comment by GM', type: 'textarea', cls: 'span6', readonly: true },
                    { name: 'CommentbySIS', text: 'Comment by SIS', type: 'textarea', cls: 'span6' },
                ]
            }
        ],
        toolbars: [
            { name: 'refresh', text: 'Refresh Activity', icon: 'fa fa-refresh' },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "hide" },
        ],
    }
    var widget = new SimDms.Widget(options);

    widget.setSelect([
        { name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" },
        { name: "Plan", url: "wh.api/combo/ReviewPlans" }
    ]);
    widget.render(function () {
        var initial = { StartMonths: [], EndMonths: [] };
        for (var i = 1; i <= 12; i++) {
            initial.StartMonths.push({ value: i, text: moment(i.toString(), 'M').format('MMMM') });
            initial.EndMonths.push({ value: i, text: moment(i.toString(), 'M').format('MMMM') });
        }
        var date1 = new Date();
        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);
        widget.populate({ DateFrom: date2, DateTo: date1 });

        $("[name=GroupArea]").on("change", function () {
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/DealerList", params: { LinkedModule: "mp", GroupArea: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/branchs", params: { comp: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=BranchCode]").prop("selectedIndex", 0);
            $("[name=BranchCode]").change();
        });

        widget.select({ selector: "#PeriodYear", url: "wh.api/Combo/ListOfYear10" });
        widget.select({ selector: "#PIC", url: "wh.api/Combo/Employee/" });
        widget.select({ selector: '#ParMonth1', data: initial.StartMonths });
        widget.select({ selector: '#ParMonth2', data: initial.EndMonths });

        $("#Plan").on("change", function () {
            var plan = $(this).val();
            if (plan == "") {
                $('.flt, .flt2').addClass('hide');
            }
            else if (plan == "BIRTHDAY CALL") {
                $(".flt2").show();
                $(".flt").hide();
            }
            else {
                $(".flt2").hide();
                $(".flt").show();
            }
        }).trigger("change");
        /*
        $('#btnBrowse').on('click', function () {
            var lookup = sdms.lookup({
                name: "LkuReviews",
                title: "Review List",
                url: "wh.api/LookUpGrid/CsReviews",
                sort: ({ field: "OutletAbbreviation", dir: "asc" }, { field: "DateFrom", dir: "desc" }),
                fields: [
                    { name: "BranchCode", text: "OutletCode", hide: true },
                    { name: "OutletAbbreviation", text: "Outlet", width: "200px" },
                    { name: "DateFrom", text: "Date From", width: "110px", template: "#= (DateFrom == undefined) ? '' : moment(DateFrom).format('DD MMM YYYY') #" },
                    { name: "DateTo", text: "Date To", width: "110px", template: "#= (DateTo == undefined) ? '' : moment(DateTo).format('DD MMM YYYY') #" },
                    { name: "Plan", text: "Plan", width: "150px" },
                    { name: "Do", text: "Do", width: "200px" },
                    { name: "Check", text: "Check", width: "300px" },
                    { name: "Action", text: "Action", width: "300px" },
                    { name: "PIC", text: "PIC" },
                    { name: "CommentbyGM", text: "Comment by GM", width: "300px" },
                    { name: "CommentbySIS", text: "Comment by SIS", width: "300px" }
                ],
                dblclick: function (row) {
                    row.InputFrom = row.DateFrom;
                    row.InputTo = row.DateTo;
                    row.Plan = row.Plan.toUpperCase();
                    widget.populate(row);
                    $("[name=GroupArea]").select2('val', row.GroupArea).trigger("change");
                    setTimeout(function () {
                        $("[name=CompanyCode]").select2('val', row.CompanyCode).trigger("change");
                        setTimeout(function () {
                            $("[name=BranchCode]").select2('val', row.BranchCode).trigger("change");
                            $("#Plan").trigger("change");
                            switchPlan(row.Plan);
                        }, 1000);
                    }, 1000);
                    selectedRow = row;
                    $("#btnSave").show()
                }
            });
        });
        */
        $("#refresh").click(function (e) {
            var plan = $("#Plan").val();
            //switchPlan(plan);
            $("#pnlReviewDetail").slideUp();
            refreshGrid();
        });

        $("#btnSave").click(function (e) {
            var data = {};
            data.CompanyCode = selectedRow.CompanyCode;
            data.BranchCode = selectedRow.BranchCode;
            data.EmployeeID = selectedRow.EmployeeID;
            data.DateFrom = moment(selectedRow.DateFrom).format("YYYY-MM-DD");
            data.DateTo = moment(selectedRow.DateTo).format("YYYY-MM-DD");
            data.Plan = selectedRow.Plan;
            data.Do = selectedRow.Do;
            data.Check = selectedRow.Check;
            data.Action = selectedRow.Action;
            data.PIC = selectedRow.PIC;
            data.CommentbyGM = selectedRow.CommentbyGM;
            data.CreatedBy = selectedRow.CreatedBy;
            data.CreatedDate = selectedRow.CreatedDate;
            data.LastupdateBy = selectedRow.LastupdateBy;
            data.LastupdateDate = selectedRow.LastupdateDate;
            data.isDeleted = selectedRow.isDeleted;

            data.CommentbySIS = $("#CommentbySIS").val();
            widget.post("wh.api/CsReviews/Save", data, function (data) {
                sdms.info("Save Success");
                reset();
            });
        });
    });

    function refreshGrid() {
        widget.kgrid({
            url: "wh.api/LookUpGrid/CsReviews",
            name: "pnlData",
            params: $("#pnlFilter").serializeObject(),
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "BranchCode", width: 120, title: "OutletCode", hidden: true },
                { field: "EmployeeID", width: 120, title: "EmployeeID", hidden: true },
                { field: "DealerAbbreviation", title: "Dealer", width: 200 },
                { field: "OutletAbbreviation", title: "Outlet", width: 200 },
                { field: "DateFrom", title: "Date From", width: 110, template: "#= (DateFrom == undefined) ? '' : moment(DateFrom).format('DD MMM YYYY') #" },
                { field: "DateTo", title: "Date To", width: 110, template: "#= (DateTo == undefined) ? '' : moment(DateTo).format('DD MMM YYYY') #" },
                { field: "Plan", title: "Plan", width: 150 },
                { field: "Do", title: "Do", width: 200 },
                { field: "Check", title: "Check", width: 300 },
                { field: "Action", title: "Action", width: 300 },
                { field: "PIC", title: "PIC", width: 120 },
                { field: "CommentbyGM", title: "Comment by GM", width: 300 },
                { field: "CommentbySIS", title: "Comment by SIS", width: 300 }
            ],
            detailInit: detailInit,
            onDblClick: function (e) {
                var data = $("#pnlFilter").serializeObject();
                data.EmployeeID = getSelectedCell(2);
                data.DateFrom = getSelectedCell(5);
                data.DateTo = getSelectedCell(6);
                data.InputFrom = getSelectedCell(5);
                data.InputTo = getSelectedCell(6);
                data.Check = getSelectedCell(9);
                data.Action = getSelectedCell(10);
                data.PIC = getSelectedCell(11);
                data.CommentbyGM = getSelectedCell(12);
                data.CommentbySIS = getSelectedCell(13);
                widget.populate(data);
                selectedRow = data;
                $("#pnlReviewDetail").slideDown();
                $("#btnSave").show()
            }
        });
    }

    function detailInit(e) {
        var row = e.data;
        switchPlan(row.Plan, e);
    }
    
    function switchPlan(plan, e) {
        switch (plan.toUpperCase()) {
            case "3 DAYS CALL":
                //refreshGrid(['DEALER', 'OUTLET', 'JUMLAH BPK', 'INPUT BY CRO', 'PERSENTASE']);
                refreshDetail(e, "wh.api/Chart/CsReportTDayCall", widget.serializeObject('pnlFilter'));
                break;
            case "BIRTHDAY CALL":
                //refreshGrid(['PERIODE', 'DEALER', 'JUMLAH CUSTOMER', 'INPUT OLEH CRO', 'GIFT', 'SMS', 'TELEPHONE', 'Letter', 'Souvenir']);
                refreshDetail(e, "wh.api/Chart/CustBirthdayReport", widget.serializeObject('pnlFilter'));
                break;
            case "BPKB REMINDER":
                //refreshGrid(['DEALER', 'READY DATE', 'JUMLAH CUSTOMER', 'INPUT OLEH CRO', 'TIDAK DAPAT DIHUBUNGI', 'PERSENTASE']);
                refreshDetail(e, "wh.api/Chart/CsReportBPKBReminder", widget.serializeObject('pnlFilter'));
                break;
            case "STNK EXTENSION":
                //refreshGrid(['DEALER', 'OUTLET', 'JUMLAH STNK', 'INPUT OLEH CRO', 'PERSENTASE']);
                refreshDetail(e, "wh.api/Chart/StnkExtension", widget.serializeObject('pnlFilter'));
                break;
            case "DELIVERY OUTSTANDING":
                break;
        }
    }

    function refreshDetail(e, url, param) {
        widget.post(url, param, function (data) {
            var invisibleColumn = ["DealerAbbreviation", "OutletName", "CompanyCode", "CompanyName", "BranchCode", "Area", "Dealer", "Outlet"];
            if (data.length > 0) {
                for (var i in data) {
                    var column = [];
                    for (var j in data[i]) {
                        if ($.inArray(j, invisibleColumn) !== -1)
                            column.push({ field: j, width: 150, title: j, hidden: true });
                        else
                            column.push({ field: j, width: 150, title: j });
                    }
                }
                    
                $("<div/>").appendTo(e.detailCell).css("width", $(".k-grid-content").width() - 100).kendoGrid({
                    dataSource: { data: data, pageSize: 10 },
                    pageable: true,
                    columns: column
                });
            }
            else {
                $("<div/>").appendTo(e.detailCell).kendoGrid({
                    dataSource: { data: [{ Info: "Data Tidak Ada" }] },
                    columns: [{ field: "Info", title: "Info" }]
                });
            }
        })
    }
    
    /*
    function refreshGrid(header) {
        $("#pnlData").html("");
        var table = d3.select('#pnlData').append('table').attr({ 'class': 'table-chart' })
        var thead = table.append('thead');
        thead.append('tr')
            .selectAll('th')
            .data(header)
            .enter()
            .append('th')
            .attr({
                'style': function (d, i) {
                    if (i == 0) return 'width:15%'
                    else return 'width:15%'
                },
                'class': function (d, i) {
                }
            })
            .text(function (d) { return d })
        var tbody = table.append('tbody');
        var tfoot = table.append('tfoot');
    }

    function refreshData(options) {
        var filter = widget.serializeObject('pnlFilter');
        url = options.url || "";
        persentase = options.persentase || [];
        footerSpan = options.footerSpan || 0;
        
        widget.post(url, filter, function (result) {
            var tbody = d3.select('#pnlData tbody');
            var tdata = [];
            tbody.selectAll('tr').data(result).enter().append('tr');
            tbody.selectAll('tr').data(result).exit().remove();

            if (result.length == 0) {
                $("#pnlData tbody")
                .html(function () {
                    html = "";
                    html += "<tr style='text-align:center'>";
                    html += "<td colspan='" + $("#pnlData table thead th").length + "'>TIDAK ADA DATA</td>";
                    html += "</tr>";
                    return html;
                });
                return;
            }

            tbody.selectAll('tr')
                .data(result)
                .html(function (d, i) {
                    var html = '';
                    for (var a in d) {
                        if (typeof options.field !== "undefined") {
                            if ($.inArray(a, options.field) != -1) {
                                if (a.toLowerCase() == "month")
                                    html += '<td>' + moment(d[a].toString(), 'MM').format('MMMM') + '</td>';
                                else if (typeof d[a] == "number") {
                                    html += '<td class="number">' + (isNaN(d[a]) ? 0 : widget.numberFormat(d[a])) + ((a == "Percentation") ? '%' : '') + '</td>';
                                    if (typeof tdata[a] == "undefined") {
                                        tdata[a] = 0;
                                    }
                                    tdata[a] += d[a];
                                }
                                else
                                    html += '<td>' + d[a] + '</td>';
                            }
                        }
                        else {
                            if (a.toLowerCase() == "month")
                                html += '<td>' + moment(d[a].toString(), 'MM').format('MMMM') + '</td>';
                            else if (typeof d[a] == "number") {
                                html += '<td class="number">' + (isNaN(d[a]) ? 0 : widget.numberFormat(d[a])) + '</td>';
                                if (typeof tdata[a] == "undefined") {
                                    tdata[a] = 0;
                                }
                                tdata[a] += d[a];
                            }
                            else
                                html += '<td>' + d[a] + '</td>';
                        }
                    }
                    return html;
                });

            if (persentase.length > 2) {
                if (!isNaN(tdata[persentase[1]] / tdata[persentase[0]])) {
                    tdata["Percentation"] = (tdata[persentase[1]] / tdata[persentase[0]]) * 100;
                }
            }

            if (footerSpan != 0) {
                var tfoot = d3.select('#pnlData tfoot');
                tfoot.html(function () {
                    var html = '';
                    html += '<tr>';
                    html += '<td colspan="' + footerSpan + '">TOTAL</td>';
                    for (var i in tdata) {
                        html += '<td class="number">' + (isNaN(tdata[i]) ? 0 : tdata[i]) + ((i == "Percentation") ? '%' : '') + '</td>';
                    }
                    html += '</tr>';
                    return html;
                });
            }
        });
    }
    */
    function reset() {
        $("#Plan, #GroupArea, #CompanyCode, #BranchCode, #ParStatus, #PeriodYear, #ParMonth1, #ParMonth2, #PIC").select2("val", "");
        widget.select({ selector: "[name=CompanyCode]", data: [], optionalText: "-- SELECT ALL --" });
        widget.select({ selector: "[name=BranchCode]", data: [], optionalText: "-- SELECT ALL --" });
        $('.flt, .flt2, #pnlReviewDetail, #btnSave').hide();
        $("#pnlData").empty();
        $("[name=InputFrom], [name=InputTo], [name=Check], [name=Action], [name=CommentbyGM], [name=CommentbySIS]").val("")
        var date1 = new Date();
        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);
        widget.populate({ DateFrom: date2, DateTo: date1 });
    }

    function getSelectedCell(idx) {
        return $(".kgrid .k-grid-content tr.k-state-selected td:eq(" + idx + ")").text();
    }
});