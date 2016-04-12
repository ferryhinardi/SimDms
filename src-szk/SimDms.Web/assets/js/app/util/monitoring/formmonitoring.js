var ptype = "4W";
$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: "Monitoring Penggunaan Aplikasi Dealer",
        xtype: "panels",
        toolbars: [
            { action: 'refresh', text: 'Refresh', icon: 'fa fa-refresh' },
            //{ action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
            //{ action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
            { action: 'exportexcel', text: 'Export to Excel', icon: 'fa fa-file-excel-o', cls: '', name: 'exportToExcel' },
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        name: 'ProductType', text: 'Product Type', type: 'select', cls: 'span5',
                        items: [
                            { text: "2 Wheelers", value: "2W" },
                            { text: "4 Wheelers", value: "4W" },
                        ]
                    },
                    { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span5' },
                    {
                        name: 'Aplikasi', text: 'Aplikasi', type: 'select', cls: 'span5',
                        items: [
                            { text: "SNIS", value: "SNIS" },
                            { text: "Market Share", value: "MS" },
                            { text: "Man Power Management", value: "MM" },
                            { text: "CS Aplication", value: "CS" },
                            { text: "Questionnaire", value: "QA" },
                            { text: "Questionnaire CBU", value: "QA2" },
                            { text: "Media Promotion Bank", value: "MPB" }
                        ]
                    },
                    { type: "span" },
                    { name: 'ExportDetails', text: 'Export Details', cls: "span4", type: "check", lblstyle: 'style="visibility:hidden;"' },
                ]
            },
            {
                name: "pnlResult",
                xtype: "k-grid",
            },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case 'refresh':
                    refresh();
                    break;
                case 'collapse':
                    widget.exitFullWindow();
                    widget.showToolbars(['refresh', 'expand']);
                    break;
                case 'expand':
                    widget.requestFullWindow();
                    widget.showToolbars(['refresh', 'collapse']);
                    break;
                default:
                    break;
            }
        },
    });

    widget.render(function () {
        //widget.post('wh.api/svtrans/GetCurrentDealer', function (r) {
        //    ptype = r.ProductType;
        //});

        //var initial = {
        //    Aplikasi: "1",
        //    ProductType: ptype
        //};

        // widget.populate(initial);

        widget.setSelect([
            {
                name: "Dealer",
                url: "wh.api/combo/ListDealers?",
                optionalText: "-- ALL -- ",
                cascade: {
                    name: "ProductType"
                }

            }
        ]);
    });

    $('#pnlFilter #ProductType').on('change', function () {
        var m = $('#pnlFilter #Aplikasi').val();
        refresh(m);
        //alert("masa "+m);
    });
    $('#pnlFilter #Dealer').on('change', function () {
        var n = $('#pnlFilter #Aplikasi').val();
        refresh(n);
    });
    $('#pnlFilter #Aplikasi').on('change', function () {
        console.log($('#pnlFilter [name="Aplikasi"]').val());
        console.log($('#pnlFilter #Aplikasi').val());
        refresh($('#pnlFilter #Aplikasi').val());
    });
    //refresh();

    function refresh(x) {
        var filter = widget.serializeObject('pnlFilter');
        console.log(filter.Aplikasi);
        console.log('awal ' + x);
        x = x == undefined ? filter.Aplikasi : x;
        console.log('hasil ' + x);
        if (x == "SNIS") {
            console.log(x);
            widget.kgrid({
                url: "wh.api/inquiry/MonitoringDealer",
                name: "pnlResult",
                params: filter,
                sort: [
                    { field: "DealerCode", dir: "asc" },
                ],
                columns: [
                    { title: 'Dealer Code', field: 'DealerCode', width: 120 },
                    { title: 'Dealer Name', field: 'DealerName', width: 270 },
                    { title: 'Outlet Code', field: 'OutletCode', width: 150 },
                    { title: 'Outlet Name', field: 'OutletName', width: 400 },
                    { title: 'User Name SNIS', field: 'UserName', width: 200 },
                    { title: 'Last Login Date SNIS', field: 'LastLogin', width: 200, type: 'date' },
                    { title: 'Last Update Date SNIS', field: 'LastUpdated', width: 200, type: 'date' },
                ],
                detailInit: function (e) {
                    widget.post("wh.api/inquiry/MonitoringDealerDetails", { Dealer: e.data.DealerCode, Outlet: e.data.OutletCode, Aplikasi: $('[name=Aplikasi] option:selected').val() }, function (data) {
                        if (data.length > 0) {
                            $("<div/>").appendTo(e.detailCell).kendoGrid({
                                dataSource: { data: data },
                                columns: [
                                    { field: "TableName", title: "Table Name", width: 160 },
                                    { field: "LastLogin", title: "Last Login Date", width: 160, template: "#= moment(LastLogin).format('DD MMM YYYY') #" },
                                    { field: "LastUpdated", title: "Last Update Date", width: 160, template: "#= moment(LastLogin).format('DD MMM YYYY') #" },
                                ]
                            });
                        }
                        else {
                            $("<div/>").appendTo(e.detailCell).kendoGrid({
                                dataSource: { data: [{ Info: "Tidak ada table" }] },
                                columns: [{ field: "Info", title: "Info" }]
                            });
                        }
                    })
                },
            });
        }
        if (x == "MS") {
            widget.kgrid({
                url: "wh.api/inquiry/MonitoringDealer",
                name: "pnlResult",
                params: filter,
                sort: [
                    { field: "DealerCode", dir: "asc" },
                ],
                columns: [
                    { title: 'Dealer Code', field: 'DealerCode', width: 120 },
                    { title: 'Dealer Name', field: 'DealerName', width: 270 },
                    { title: 'Outlet Code', field: 'OutletCode', width: 150 },
                    { title: 'Outlet Name', field: 'OutletName', width: 400 },
                    { title: 'User Name Market Share', field: 'UserName', width: 200 },
                    { title: 'Last Login Date Market Share', field: 'LastLogin', width: 200, type: 'date' },
                    { title: 'Last Update Date Market Share', field: 'LastUpdated', width: 200, type: 'date' },
                ],
                detailInit: function (e) {
                    widget.post("wh.api/inquiry/MonitoringDealerDetails", { Dealer: e.data.DealerCode, Outlet: e.data.OutletCode, Aplikasi: $('[name=Aplikasi] option:selected').val() }, function (data) {
                        if (data.length > 0) {
                            $("<div/>").appendTo(e.detailCell).kendoGrid({
                                dataSource: { data: data },
                                columns: [
                                    { field: "TableName", title: "Table Name", width: 160 },
                                    { field: "LastLogin", title: "Last Login Date", width: 160, template: "#= moment(LastLogin).format('DD MMM YYYY') #" },
                                    { field: "LastUpdated", title: "Last Update Date", width: 160, template: "#= moment(LastLogin).format('DD MMM YYYY') #" },
                                ]
                            });
                        }
                        else {
                            $("<div/>").appendTo(e.detailCell).kendoGrid({
                                dataSource: { data: [{ Info: "Tidak ada table" }] },
                                columns: [{ field: "Info", title: "Info" }]
                            });
                        }
                    })
                },
            });
        }
        if (x == "MM") {
            widget.kgrid({
                url: "wh.api/inquiry/MonitoringDealer",
                name: "pnlResult",
                params: filter,
                sort: [
                    { field: "DealerCode", dir: "asc" },
                ],
                columns: [
                    { title: 'Dealer Code', field: 'DealerCode', width: 120 },
                    { title: 'Dealer Name', field: 'DealerName', width: 270 },
                    { title: 'Outlet Code', field: 'OutletCode', width: 150 },
                    { title: 'Outlet Name', field: 'OutletName', width: 400 },
                    { title: 'Last Login Date Man Power Management', field: 'LastLogin', width: 200, type: 'date' },
                    { title: 'Last Update Date Man Power Management', field: 'LastUpdated', width: 200, type: 'date' },
                ],
                detailInit: function (e) {
                    widget.post("wh.api/inquiry/MonitoringDealerDetails", { Dealer: e.data.DealerCode, Outlet: e.data.OutletCode, Aplikasi: $('[name=Aplikasi] option:selected').val() }, function (data) {
                        if (data.length > 0) {
                            $("<div/>").appendTo(e.detailCell).kendoGrid({
                                dataSource: { data: data },
                                columns: [
                                    { field: "TableName", title: "Table Name", width: 160 },
                                    { field: "LastLogin", title: "Last Login Date", width: 160, template: "#= moment(LastLogin).format('DD MMM YYYY') #" },
                                    { field: "LastUpdated", title: "Last Update Date", width: 160, template: "#= moment(LastLogin).format('DD MMM YYYY') #" },
                                ]
                            });
                        }
                        else {
                            $("<div/>").appendTo(e.detailCell).kendoGrid({
                                dataSource: { data: [{ Info: "Tidak ada table" }] },
                                columns: [{ field: "Info", title: "Info" }]
                            });
                        }
                    })
                },
            });
        }
        if (x == "CS") {
            widget.kgrid({
                url: "wh.api/inquiry/MonitoringDealer",
                name: "pnlResult",
                params: filter,
                sort: [
                    { field: "DealerCode", dir: "asc" },
                ],
                columns: [
                    { title: 'Dealer Code', field: 'DealerCode', width: 120 },
                    { title: 'Dealer Name', field: 'DealerName', width: 270 },
                    { title: 'Outlet Code', field: 'OutletCode', width: 150 },
                    { title: 'Outlet Name', field: 'OutletName', width: 400 },
                    { title: 'Last Login Date CS Aplication', field: 'LastLogin', width: 200, type: 'date' },
                    { title: 'Last Update Date CS Aplication', field: 'LastUpdated', width: 200, type: 'date' },
                ],
                detailInit: function (e) {
                    widget.post("wh.api/inquiry/MonitoringDealerDetails", { Dealer: e.data.DealerCode, Outlet: e.data.OutletCode, Aplikasi: $('[name=Aplikasi] option:selected').val() }, function (data) {
                        if (data.length > 0) {
                            $("<div/>").appendTo(e.detailCell).kendoGrid({
                                dataSource: { data: data },
                                columns: [
                                    { field: "TableName", title: "Table Name", width: 160 },
                                    { field: "LastLogin", title: "Last Login Date", width: 160, template: "#= moment(LastLogin).format('DD MMM YYYY') #" },
                                    { field: "LastUpdated", title: "Last Update Date", width: 160, template: "#= moment(LastLogin).format('DD MMM YYYY') #" },
                                ]
                            });
                        }
                        else {
                            $("<div/>").appendTo(e.detailCell).kendoGrid({
                                dataSource: { data: [{ Info: "Tidak ada table" }] },
                                columns: [{ field: "Info", title: "Info" }]
                            });
                        }
                    })
                },
            });
        }

        if (x == "QA") {
            console.log(x);
            widget.kgrid({
                url: "wh.api/inquiry/MonitoringDealer",
                name: "pnlResult",
                params: filter,
                sort: [
                    { field: "DealerCode", dir: "asc" },
                ],
                columns: [
                    { title: 'Dealer Code', field: 'DealerCode', width: 120 },
                    { title: 'Dealer Name', field: 'DealerName', width: 270 },
                    { title: 'Outlet Code', field: 'OutletCode', width: 150 },
                    { title: 'Outlet Name', field: 'OutletName', width: 400 },
                    { title: 'User Name QA', field: 'UserName', width: 200 },
                    { title: 'Last Login Date QA', field: 'LastLogin', width: 200, type: 'date' },
                    { title: 'Last Update Date QA', field: 'LastUpdated', width: 200, type: 'date' },
                ],
                detailInit: function (e) {
                    widget.post("wh.api/inquiry/MonitoringDealerDetails", { Dealer: e.data.DealerCode, Outlet: e.data.OutletCode, Aplikasi: $('[name=Aplikasi] option:selected').val() }, function (data) {
                        if (data.length > 0) {
                            $("<div/>").appendTo(e.detailCell).kendoGrid({
                                dataSource: { data: data },
                                columns: [
                                    { field: "TableName", title: "Table Name", width: 160 },
                                    { field: "LastLogin", title: "Last Login Date", width: 160, template: "#= moment(LastLogin).format('DD MMM YYYY') #" },
                                    { field: "LastUpdated", title: "Last Update Date", width: 160, template: "#= moment(LastLogin).format('DD MMM YYYY') #" },
                                ]
                            });
                        }
                        else {
                            $("<div/>").appendTo(e.detailCell).kendoGrid({
                                dataSource: { data: [{ Info: "Tidak ada table" }] },
                                columns: [{ field: "Info", title: "Info" }]
                            });
                        }
                    })
                },
            });
        }

        if (x == "QA2") {
            console.log(x);
            widget.kgrid({
                url: "wh.api/inquiry/MonitoringDealer",
                name: "pnlResult",
                params: filter,
                sort: [
                    { field: "DealerCode", dir: "asc" },
                ],
                columns: [
                    { title: 'Dealer Code', field: 'DealerCode', width: 120 },
                    { title: 'Dealer Name', field: 'DealerName', width: 270 },
                    { title: 'Outlet Code', field: 'OutletCode', width: 150 },
                    { title: 'Outlet Name', field: 'OutletName', width: 400 },
                    { title: 'User Name QA2', field: 'UserName', width: 200 },
                    { title: 'Last Login Date QA2', field: 'LastLogin', width: 200, type: 'date' },
                    { title: 'Last Update Date QA2', field: 'LastUpdated', width: 200, type: 'date' },
                ],
                detailInit: function (e) {
                    widget.post("wh.api/inquiry/MonitoringDealerDetails", { Dealer: e.data.DealerCode, Outlet: e.data.OutletCode, Aplikasi: $('[name=Aplikasi] option:selected').val() }, function (data) {
                        if (data.length > 0) {
                            $("<div/>").appendTo(e.detailCell).kendoGrid({
                                dataSource: { data: data },
                                columns: [
                                    { field: "TableName", title: "Table Name", width: 160 },
                                    { field: "LastLogin", title: "Last Login Date", width: 160, template: "#= moment(LastLogin).format('DD MMM YYYY') #" },
                                    { field: "LastUpdated", title: "Last Update Date", width: 160, template: "#= moment(LastLogin).format('DD MMM YYYY') #" },
                                ]
                            });
                        }
                        else {
                            $("<div/>").appendTo(e.detailCell).kendoGrid({
                                dataSource: { data: [{ Info: "Tidak ada table" }] },
                                columns: [{ field: "Info", title: "Info" }]
                            });
                        }
                    })
                },
            });
        }

        if (x == "MPB") {
            console.log(x);
            widget.kgrid({
                url: "wh.api/inquiry/MonitoringDealer",
                name: "pnlResult",
                params: filter,
                sort: [
                    { field: "DealerCode", dir: "asc" },
                ],
                columns: [
                    { title: 'Dealer Code', field: 'DealerCode', width: 120 },
                    { title: 'Dealer Name', field: 'DealerName', width: 270 },
                    { title: 'Outlet Code', field: 'OutletCode', width: 150 },
                    { title: 'Outlet Name', field: 'OutletName', width: 400 },
                    { title: 'User Name MPB', field: 'UserName', width: 200 },
                    { title: 'Last Login Date MPB', field: 'LastLogin', width: 200, type: 'date' },
                    { title: 'Last Update Date MPB', field: 'LastUpdated', width: 200, type: 'date' },
                ],
                detailInit: function (e) {
                    widget.post("wh.api/inquiry/MonitoringDealerDetails", { Dealer: e.data.DealerCode, Outlet: e.data.OutletCode, Aplikasi: $('[name=Aplikasi] option:selected').val() }, function (data) {
                        if (data.length > 0) {
                            $("<div/>").appendTo(e.detailCell).kendoGrid({
                                dataSource: { data: data },
                                columns: [
                                    { field: "TableName", title: "Table Name", width: 160 },
                                    { field: "LastLogin", title: "Last Login Date", width: 160, template: "#= moment(LastLogin).format('DD MMM YYYY') #" },
                                    { field: "LastUpdated", title: "Last Update Date", width: 160, template: "#= moment(LastLogin).format('DD MMM YYYY') #" },
                                ]
                            });
                        }
                        else {
                            $("<div/>").appendTo(e.detailCell).kendoGrid({
                                dataSource: { data: [{ Info: "Tidak ada table" }] },
                                columns: [{ field: "Info", title: "Info" }]
                            });
                        }
                    })
                },
            });
        }
    }

    $("#refresh").on("click", function (e) {
        refresh($('#pnlFilter #Aplikasi').val());
    });

    $("#exportToExcel").on("click", function (e) {
        var caption = 'Form Monitoring Penggunaan Aplikasi ';
        var dealer = $('#pnlFilter #Dealer').val();
        var iswhat = $('#pnlFilter #Aplikasi').val();
        var exportdetail = $('#pnlFilter #ExportDetails').is(':checked') ? 1 : 0;

        console.log(dealer);
        console.log(iswhat);
        if (iswhat == 'SNIS') {
            caption += 'SNIS';
        } else if (iswhat == 'MS') {
            caption += 'Market Share';
        } else if (iswhat == 'MM') {
            caption += 'Manpower Management';
        } else {
            caption += 'CS Aplication';
        }
        console.log(caption);
        var url = "wh.api/InquiryProd/MonitoringDealer?";
        var params = "&ProductType=" + $('#pnlFilter #ProductType').val();
        params += "&Dealer=" + dealer;
        params += "&IsWhat=" + iswhat;
        params += "&Caption=" + caption;
        params += "&ExportDetails=" + exportdetail;
        url = url + params;
        window.location = url;
    });

    //$("#exportToExcel").on("click", function (e) {

    //    var params = widget.serializeObject('pnlFilter');

    //    e.preventDefault();
    //    $('.page > .ajax-loader').show();

    //    $.fileDownload('doreport/DealerInfo.xlsx', {
    //        httpMethod: "POST",
    //        //preparingMessageHtml: "We are preparing your report, please wait...",
    //        //failMessageHtml: "There was a problem generating your report, please try again.",
    //        data: params
    //    }).done(function () {
    //        $('.page > .ajax-loader').hide();
    //    });

    //});

});
