"use strict"

function omUtilSendFile($scope, $http, $injector) {
    var me = $scope;
    var currentDate = moment().format();
    var contents = "";

    $injector.invoke(BaseController, this, { $scope: me });

    //console.log(me.data.isCBU)

    me.LookUpRequest = function (val) {
        var params = {
            dateFrom: moment(me.data.DateFrom).format('YYYY/MM/DD'),
            dateTo: moment(me.data.DateTo).format('YYYY/MM/DD'),
            isCBU: me.data.IsCBU,
            isFPOL: (me.data.SendType == "Faktur" ? true : false)
        };
        var lookup = Wx.klookup({
            name: "lookupRequest",
            title: "No. Req. Faktur Polisi",
            url: "om.api/SendFile/Select4Send",
            params: params,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "ReqNo", title: "No.Request", width: 200 },
                {
                    field: "ReqDate", title: "Tgl. Request", width: 130,
                    template: "#= (ReqDate == undefined) ? '' : moment(ReqDate).format('DD MMM YYYY') #"
                }
            ]
        });
        lookup.dblClick(function (data) {
            if (val == "from") {
                me.data.RequestFrom = data.ReqNo;
                me.data.RequestTo = data.ReqNo;
            }
            else {
                me.data.RequestTo = data.ReqNo;
            }
            me.Apply();
        });
    };

    me.SetStock = function () {
        me.IsStock = true;
        me.IsFaktur = false;
        me.data.SendType = "Stock";

        $("#lblCBU label:first").html("");
        $("#lblFilter").html("Stock");
    };

    me.SetFaktur = function () {
        me.IsStock = true;
        me.IsFaktur = true;
        me.data.SendType = "Faktur";

        $("#IsCBU").show();
        $("#lblCBU label:first").html("is CBU");
        $("#lblFilter").html("Faktur Polisi");
        $("label[for=noreqcontrol]").html("No. Req.");
    }

    me.SetRevfaktur = function () {
        me.IsStock = true;
        me.IsFaktur = true;
        me.data.SendType = "RevFaktur";

        $("#IsCBU").hide();
        $("#lblCBU label:first").html("");
        $("#lblFilter").html("Revisi Faktur Polisi");
        $("label[for=noreqcontrol]").html("No. Revisi");
    }

    me.SendData = function () {
        if (me.IsStock) {
            $http.post("om.api/SendFile/DcsValidated")
            .success(function (result) {
                if (result) {
                    me.DisplayData();
                    return;
                }
                else {
                    me.DownloadData();
                    return;
                }
            })
            .error(function (e) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
            });
        }
    };

    me.DownloadData = function () {
        me.data.IsFPOL = me.data.SendType == "Faktur" ? true : false;
        me.data.IsStock = me.data.SendType == "Stock" ? true : false;
        me.data.IsRFPOL = me.data.SendType == "Revfaktur" ? true : false;
        $('.page > .ajax-loader').show();
        $http.post("om.api/SendFile/DataForDownload_Validated", me.data)
        .success(function (result) {
            if (result.success != undefined) {
                if (!result.success) {
                    MsgBox(result.message, MSG_WARNING);
                    $('.page > .ajax-loader').hide();
                    return;
                }
            }

            var dat = me.data;
            var isCBU = (dat.IsCBU != undefined) ? dat.IsCBU : false;
            var params = "DateFrom=" + moment(dat.DateFrom).format("YYYY/MM/DD") + "&DateTo=" + moment(dat.DateTo).format("YYYY/MM/DD") +
                "&RequestFrom=" + dat.RequestFrom + "&RequestTo=" + dat.RequestTo +
                "&IsFPOL=" + dat.IsFPOL + "&IsStock=" + dat.IsStock + "&IsRFPOL=" + dat.IsRFPOL
                "&IsRequest=" + dat.IsRequest + "&IsCBU=" + isCBU;

            $('.page > .ajax-loader').show();
            var url = SimDms.baseUrl + 'om.api/SendFile/DataForDownload?' + params;
            window.location = url;
            $('.page > .ajax-loader').hide();
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
            $('.page > .ajax-loader').hide();
            console.log(result.error_log);
        });
    };

    me.DisplayData = function () {
        me.data.IsFPOL = me.data.SendType == "Faktur" ? true : false;
        me.data.IsStock = me.data.SendType == "Stock" ? true : false;
        me.data.IsRFPOL = me.data.SendType == "RevFaktur" ? true : false;
        
        $('.page > .ajax-loader').show();
        $http.post("om.api/SendFile/DataForDisplay", me.data)
        .success(function (result) {
            if (result) {
                if (result.data != undefined || result.data != null) {
                    contents = result.data;
                    BootstrapDialog.show({
                        //BootstrapDialog:'SIZE_LARGE',
                        title: '<b><i class="icon icon-gear"></i> ' + result.code + '</b>',
                        message: result.data,
                        buttons: [{
                            id: 'btnSaveFile',
                            label: 'Save File',
                            cssClass: 'button',
                            icon: 'icon icon-save',
                            action: function (dialogItself) {
                                var dat = me.data;
                                console.log(dat.IsRequest)
                                var isCBU = (dat.IsCBU != undefined) ? dat.IsCBU : false;
                                var params = "DateFrom=" + moment(dat.DateFrom).format("YYYY/MM/DD") + "&DateTo=" + moment(dat.DateTo).format("YYYY/MM/DD") +
                                    "&RequestFrom=" + dat.RequestFrom + "&RequestTo=" + dat.RequestTo +
                                    "&IsFPOL=" + dat.IsFPOL + "&IsStock=" + dat.IsStock + "&IsRFPOL=" + dat.IsRFPOL +
                                    "&IsRequest=" + dat.IsRequest + "&IsCBU=" + isCBU;

                                var url = SimDms.baseUrl + 'om.api/SendFile/DataForDownload?' + params;
                                window.location = url;
                            }
                        },
                        {
                            id: 'btnSendFile',
                            label: 'Send File',
                            cssClass: 'button',
                            icon: 'icon icon-location-arrow',
                            action: function (dialogItself) {
                                BootstrapDialog.confirm('Ingin Mengirim File?', function (result) {
                                    if (result) {
                                        var param = { Contents: contents };
                                        $.post("om.api/SendFile/SendToDcs", param, function (result) {
                                            if (result.success) {
                                                MsgBox(result.message);
                                            }
                                            else {
                                                MsgBox(result.message, MSG_ERROR);
                                            }
                                        }).
                                        error(function (data, status, headers, config) {
                                            MsgBox("Connection to the server failed...., status " + status, MSG_ERROR);
                                        });
                                        console.log(contents);
                                    }
                                    //else {
                                    //    BootstrapDialog.alert('Testing');
                                    //}
                                });
                                dialogItself.close();
                            }
                        },
                        {
                            label: 'Close',
                            cssClass: 'button',
                            icon: 'icon icon-undo',
                            action: function (dialogItself) {
                                dialogItself.close();
                            }
                        }]
                    });
                } else {
                    MsgBox(result.message, MSG_WARNING);
                }
                $('.page > .ajax-loader').hide();
                return;
            }
            else {
                MsgBox(result.message, MSG_WARNING);
                $('.page > .ajax-loader').hide();
                return;
            }
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
            $('.page > .ajax-loader').hide();
            console.log(result.error_log);
        });
    };

    me.initialize = function () {
        me.IsStock = false;
        me.IsFaktur = false;

        me.data.DateFrom = currentDate;
        me.data.DateTo = currentDate;
        me.data.IsRequest = false;
        me.data.RequestFrom = "";
        me.data.RequestTo = "";
        me.data.IsCBU = false;
    }
    me.start();
};

$(document).ready(function () {
    var options = {
        title: "Send File",
        xtype: "panels",
        panels: [
            {
                name: "pnlStatus",
                cls: "span8",
                items: [
                    { type: "label", text: "Pilihan Send Data", style: "font-size: 14px; color : blue;" },
                    { type: "div", cls: "divider span6" },
                    {
                        type: "controls",
                        name: "optionsSending",
                        cls: "span4",
                        items: [
                            {
                                type: "buttons",
                                //name: "optionsSending",
                                model: "data.SendType",
                                cls: "span6",
                                items: [
                                    { name: "Faktur", model: "data.IsFPOL", text: "Faktur", click: "SetFaktur()", style: "margin-right: 10px" },
                                    { name: "Stock", model: "data.IsStock", text: "Stock", click: "SetStock()", style: "margin-right: 10px" },
                                    { name: "Revfaktur", model: "data.IsRFPOL", text: "Revisi Faktur", click: "SetRevfaktur()" }
                                ]
                            },
                            { name: "IsCBU", model: "data.IsCBU", type: "ng-check", cls: "span1", show: "IsFaktur" },
                            { name: "lblCBU", type: "label", text: "", cls: "span1", style: "line-height: 33px;" }
                        ]
                    },
                    { name: "lblFilter", type: "label", text: "", style: "font-size: 14px; color : blue;" },
                    { type: "div", cls: "divider span6" },
                    {
                        type: "controls",
                        cls: "span6",
                        text: "Dari tanggal",
                        show: "IsStock",
                        items: [
                            { name: "a", type: "ng-check", cls: "span1", style: "visibility: hidden;" },
                            { name: "DateFrom", model: "data.DateFrom", cls: "span3", type: 'ng-datepicker' },
                            { type: "label", text: "sampai tanggal", cls: "span1", style: "line-height: 33px;" },
                            { name: "DateTo", model: "data.DateTo", cls: "span3", type: 'ng-datepicker' },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span6",
                        name: "noreqcontrol",
                        text: "No. Req.",
                        show: "IsFaktur",
                        items: [
                            { name: "IsRequest", model: "data.IsRequest", type: "ng-check", cls: "span1" },
                            { name: "RequestFrom", model: "data.RequestFrom", cls: "span3", type: 'popup', click: "LookUpRequest('from')", readonly: true, disable: "!data.IsRequest" },
                            { type: "label", text: "sampai dengan", cls: "span1", style: "line-height: 33px;" },
                            { name: "RequestTo", model: "data.RequestTo", cls: "span3", type: 'popup', click: "LookUpRequest('to')", readonly: true, disable: "!data.IsRequest || data.RequestFrom ===''" },
                        ]
                    },
                    {
                        type: "buttons", cls: "span6", items: [
                              {
                                  name: "btnSend", text: "Send", cls: "btn btn-success span6", icon: "icon icon-location-arrow", click: "SendData()"
                              }
                        ]
                    }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("omUtilSendFile");
    }
});
