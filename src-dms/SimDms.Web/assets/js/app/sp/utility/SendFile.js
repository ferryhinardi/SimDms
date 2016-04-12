var type = "";

function spSendFileController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/Years').
    success(function (data, status, headers, config) {
        me.comboYear = data;
    });

    $http.post('sp.api/Combo/Months').
    success(function (data, status, headers, config) {
        me.comboMonth = data;
    });

    $('#FirstPeriod').on('change', function (e) {
        var firstPeriod = $('#FirstPeriod').val();
        var endPeriod = $('#EndPeriod').val();

        if (firstPeriod > endPeriod) { $('#EndPeriod').val(firstPeriod) }
    });

    $('#EndPeriod').on('change', function (e) {
        var firstPeriod = $('#FirstPeriod').val();
        var endPeriod = $('#EndPeriod').val();

        if (firstPeriod < endPeriod) { $('#FirstPeriod').val(endPeriod) }
    });

    me.default = function () {
        $http.post('sp.api/sendfile/default').
        success(function (data, status, headers, config) {
            $('#Month').select2('val', data.Month);
            $('#Year').select2('val', data.Year);
            me.data.Month = data.Month;
            me.data.Year = data.Year;
        });
    }

    me.browseSupplier = function () {
        var lookup = Wx.blookup({
            name: "Supplier",
            title: "Pencarian penerima",
            manager: SpUtilityManager,
            query: "Supplier",
            columns: [
                { field: 'SupplierCode', title: 'Kode Pemasok' },
                { field: 'StandardCode', title: 'Kode Standar' },
                { field: 'SupplierName', title: 'Nama Pemasok' },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.SupplierCode = data.SupplierCode;
            me.data.StandardCode = data.StandardCode;
            me.data.SupplierName = data.SupplierName;
            me.Apply();
        });
    }

    me.sendData = function () {
        $http.post('sp.api/sendFile/SendData', me.data)
               .success(function (e) {
                   type = e.type;
                   if (e.success) {
                       Wx.showFlatFile({ data: e.contents });
                   } else {
                       window.location = 'sp.api/sendFile/GetData?type=';
                   }
               })
               .error(function (e) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               });
    }

    SavePopup = function () {
        window.location = 'sp.api/sendFile/GetData?type=' + type;
    }

    SendPopup = function () {
        $http.post('sp.api/sendFile/ValidateHeaderFile', { type: type, contents: $('#pnlViewData').val() })
        .success(function (e) {
            if (!e.success) {
                MsgConfirm(e.message, function (result) {
                    if (result) {
                        MsgConfirm("Apakah anda yakin ingin mengirim data ini ?", function (result) {
                            if (result) {
                                $http.post('sp.api/sendfile/sendfile', { type: type, contents: $('#pnlViewData').val() })
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
                        $http.post('sp.api/sendfile/sendfile', { type: type, contents: $('#pnlViewData').val() })
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

    me.initialize = function()
    {
        me.default();
        me.data.FirstPeriod = me.data.EndPeriod = me.now();
        me.data.Daily = me.data.Stock = me.data.Sales = me.data.PlanRealization = me.data.BackOrder = me.data.LeadTime = false
    }

    me.start();

    me.HideForm = function () {
        $(".body > .panel").fadeOut();
    }

}


$(document).ready(function () {
    var options = {
        title: "Send File",
        xtype: "panels",

        panels: [
            {
                name: "pnlA",
                title: "",
                items: [
                        { name: "Month", cls: "span4", text: "Periode", type: "select2", datasource: "comboMonth" },
                        { name: "Year", required: true, cls: "span4", text: "-", type: "select2", datasource: "comboYear" },
                        { name: "Daily", text: "Daily Report Activity", type: "ng-check", cls: "span4" },
                        { name: "PlanRealization", text: "Plan And Realization", type: "ng-check", cls: "span4" },
                        { name: "Stock", text: "Stock", type: "ng-check", cls: "span4" },
                        { name: "BackOrder", text: "Back Order", type: "ng-check", cls: "span4" },
                        { name: "Sales", text: "Sales", type: "ng-check", cls: "span4" },
                        { type: "label", text: "Option Untuk Lead Time", style: "font-size: 14px; color : blue;" },
                        { type: "div", cls: "divider" },
                        { name: "FirstPeriod", text: "Periode",cls: "span3", type: "ng-datepicker" },
                        { name: "EndPeriod", text: "S/D", cls: "span3", type: "ng-datepicker"},
                        { name: "LeadTime", text: "Lead Time Monitoring", type: "ng-check", cls: "span4" },
                        { type: "div", cls: "divider" },
                        {
                            text: "Penerima",
                            type: "controls",
                            items: [
                                    { name: "SupplierCode", click: "browseSupplier()", cls: "span2", placeHolder: "Kode Penerima", type: "popup" },
                                    { name: "SupplierName", cls: "span6", placeHolder: "Nama Penerima", readonly: true }
                            ]
                        },
                        {
                            type: "buttons", cls: "span2", items: [
                                { name: "SendData", text: "   Send Data", icon: "icon-gear", click: "sendData()", cls: "button small btn btn-warning" },
                            ]
                        },
                ]
            },
        ]
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spSendFileController");
    }

});