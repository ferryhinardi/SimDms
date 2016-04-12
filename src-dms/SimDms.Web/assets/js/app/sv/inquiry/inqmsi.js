var status = 'N';

"use strict";

function svInqMsIController($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sv.api/inqmsi/ListOfYear').
    success(function (data, status, headers, config) {
      me.comboYear = data;
    });

    $http.post('sv.api/inqmsi/ListOfMonth').
    success(function (data, status, headers, config) {
        me.comboMonth = data;
    });

    me.default = function () {
        $http.post('sv.api/inqmsi/default').
        success(function (data, status, headers, config) {
            me.data.Month = data.Month;
            me.data.Year = data.Year;
        });
    }

    me.loadData = function () {
        me.kgridMSIInfo();
        me.isPrintAvailable = true;
    }

    me.genWMSIA = function () {
        $http.post('sv.api/inqmsi/generatewmsia', me.data)
        .success(function (data, status, headers, config) {
            Wx.showFlatFile({ data: data });
        }).error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    SavePopup = function () {
        window.location = "sv.api/inqmsi/DownloadFile?year=" + me.data.Year + "&month=" + me.data.Month;
    }

    SendPopup = function () {
        $http.post('sv.api/inqmsi/ValidateHeaderFile', me.data)
        .success(function (e) {
            if (!e.success) {
                MsgConfirm(e.message, function (result) {
                    if (result) {
                        MsgConfirm("Apakah anda yakin ingin mengirim data ini ?", function (result) {
                            if (result) {
                                $http.post('sv.api/inqmsi/SendFile', me.data)
                                .success(function (data, status, headers, config) {
                                    if (data.success) {
                                        Wx.Success(data.message);
                                        me.HideForm();
                                    }
                                    else {
                                        MsgBox(data.message, MSG_ERROR);
                                    }
                                }).error(function (e, status, headers, config) {
                                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                                });
                            }
                        });
                    }
                });
            }
            else {
                MsgConfirm("Apakah anda yakin ingin mengirim data ini ?", function (result) {
                    if (result) {
                        $http.post('sv.api/inqmsi/SendFile', me.data)
                        .success(function (data, status, headers, config) {
                            if (data.success) {
                                Wx.Success(data.message);
                                me.HideForm();
                            }
                            else {
                                MsgBox(data.message, MSG_ERROR);
                            }
                        }).error(function (e, status, headers, config) {
                            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                        });
                    }
                });
            }
        }).error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.kgridMSIInfo = function () {
        var lookup = Wx.kgrid({
            url: "sv.api/inqmsi/LoadData",
            name: "KGridMSIInfo",
            params: me.data,
            scrollable: true,
            pageSize: 100,
            group: [
                   { field: "MsiGroup"},
                   ],
            columns: [
                { field: "SeqNo", title: "No.", width: 50 },
                {
                  field: "MsiGroup", title: "Units In Stock",
                  groupHeaderTemplate: "Group : #= value #",
                },
                { field: "MsiDesc", title: 'Keterangan', width: 400 },
                { field: "Unit", title: "Unit", width:75 },
                { field: "Average", title: "Average", width: 100 },
                { field: "Total", title: "Total", width: 100 },
                { field: "Jan", title: 'Jan', width: 100 },
                { field: "Feb", title: 'Feb', width: 100 },
                { field: "Mar", title: 'Mar', width: 100 },
                { field: "Apr", title: 'Apr', width: 100 },
                { field: "May", title: 'May', width: 100 },
                { field: "Jun", title: 'Jun', width: 100 },
                { field: "Jul", title: 'Jul', width: 100 },
                { field: "Aug", title: 'Aug', width: 100 },
                { field: "Sep", title: 'Sep', width: 100 },
                { field: "Oct", title: 'Oct', width: 100 },
                { field: "Nov", title: 'Nov', width: 100 },
                { field: "Dec", title: 'Dec', width: 100 }
                ],
        });
    }

    me.printPreview = function () {
    
        var par = me.data.Year + "," + '1' + "," + me.data.Month;
        var rparam ='print'
        Wx.showPdfReport({
            id: 'SvRpReport021MSI',
            pparam: par,
            rparam: rparam,
            type: "devex"
        });
    }

    me.initialize = function () {
        me.default();
        me.isPrintAvailable = false;
    }

    me.start();

    me.HideForm = function () {
        $(".body > .panel").fadeOut();
    }
}

$(document).ready(function () {
    var options = {
        title: "Inquiry Suzuki MSI",
        xtype: "panels",
        toolbars: [
            { name: 'btnLoadData', text: 'Load Data', cls: "btn btn-info", icon: 'icon-search', click: "loadData()" },
            { name: 'btnGenerate', text: 'Generate WMSIA', icon: "icon-plus", cls: "btn btn-success", click: "genWMSIA()" },
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", show: "isPrintAvailable" }
        ],
        panels: [
            {
                name:"pnlInqMSI",
                title: "Suzuki Major Service Indicator (MSI)",
                items: [
                     { name: "Year", cls: "span4", text: "Year", type: "select2", datasource: "comboYear" },
                     { name: "Month", cls: "span4", text: "Month", type: "select2", datasource: "comboMonth" },
                ]
            },
            { 
                name: "KGridMSIInfo",
                title: "Suzuki MSI Data",
                xtype: "k-grid"
            },
        ]
    }

   

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("svInqMsIController");
    }
});

