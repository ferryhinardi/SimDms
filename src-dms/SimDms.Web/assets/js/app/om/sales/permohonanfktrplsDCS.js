"use strict"
var status = 0;

function omPermohonanFakturPolisDCSController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "GetFakturPolisiDCSHeader",
            title: "Input Faktur Polisi DCS",
            manager: spSalesManager,
            query: "GetFakturPolisiDCSHeader",
            defaultSort: "DealerCode asc",
            columns: [
                { field: "DealerCode", title: "DealerCode" },
                { field: "CustomerName", title: "CustomerName" },
                { field: "BatchNo", title: "BatchNo" },
                { field: "Status", title: "Status" },
                { field: "CreatedDate", title: "CreatedDate" }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.DealerCode = data.DealerCode;
            me.data.BatchNo = data.BatchNo;
            
            var datas = {
                "BatchNo": data.BatchNo
            }

            if (data.Status == "Un - Posted") {
                $('#btnProses').removeAttr('disabled');
            } else {
                $http.post('om.api/PermohonanFktrPls/SelectBatchByLockedBy', datas)
                       .success(function (e) {
                           if (e.success) {
                               me.data.ReqNo = e.Result[0].ReqNo;
                               me.data.ReqDate = e.Result[0].ReqDate;
                               me.data.ReffNo = e.Result[0].ReffNo;
                               me.data.ReffDate = e.Result[0].ReffDate;
                               me.data.Remark = e.Result[0].Remark;
                               me.data.IsFaktur = e.Result[0].IsFaktur;
                           } else {
                                    MsgBox(e.message, MSG_ERROR);
                           }
                       })
                        .error(function (e) {
                            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                        });
            }
            me.Apply();
        });
    }

    me.DealerCode = function () {
        me.browse();
    }

    me.Proses = function () {
        if (me.data.ReffNo == undefined) {
            MsgBox("No referensi harus diisi", MSG_ERROR);
        }
        //else {
        //    var datDetail = [];

        //    $.each(me.detail, function (key, val) {
        //        var arr = {
        //            "DocNo": val["DocNo"],
        //            "TypeJournal": val["TypeJournal"]
        //        }
        //        datDetail.push(arr);
        //    });

        //    var dat = {};
        //    dat["model"] = datDetail;
        //    var JSONData = JSON.stringify(dat);

        //    $http.post('om.api/RePostingJurnal/RePostingJurnal', JSONData).
        //    success(function (dl, status, headers, config) {
        //        if (dl.success) {
        //            MsgBox(dl.message);
        //            me.initialize();
        //        } else {
        //            MsgBox(dl.message, MSG_ERROR);
        //            console.log(dl.error_log);
        //        }
        //    }).
        //    error(function (e, status, headers, config) {
        //        MsgBox("Connecting server error", MSG_ERROR);
        //    });
        //}
    }

    me.griddetail = new webix.ui({
        container: "wxkendaraan",
        view: "wxtable", css:"alternating",
        scrollX: true,
        columns: [
            { id: "SONo", header: "No.SO", width: 200 },
            { id: "ChassisCode", header: "Kode Rangka", width: 90},
            { id: "ChassisNo", header: "No Rangka", width: 75},
            { id: "EngineCode", header: "Kode Mesin", width: 75},
            { id: "EngineNo", header: "No Mesin", width: 75},
            { id: "SalesModelCode", header: "Sales Model Code", width: 100},
            { id: "SalesModelYear", header: "Year", width: 50},
            { id: "SalesModelDescription", header: "Description", width: 175},
            { id: "ModelLine", header: "Model Line", width: 100},
            { id: "ColourDescription", header: "Warna", width: 200},
            { id: "ServiceBookNo", header: "Service Book No", width: 100},
            { id: "FakturPolisiNo", header: "No.Fakt.Polisi", width: 100},
            { id: "FakturPolisiDate", header: "Tgl FPol", width: 75},
            { id: "FpolisiModelDescription", header: "FP Model Description", width: 1250},
            { id: "SISDeliveryOrderNo", header: "SIS-No. DO", width: 75},
            { id: "SISDeliveryOrderDate", header: "SIS-Tgl DO", width: 75},
            { id: "SISSuratJalanNo", header: "SIS-No.SJ", width: 75},
            { id: "SISSuratJalanAtasNama", header: "SIS SJ Atas Nama", width: 150},
            { id: "DealerClass", header: "Class", width: 75},
            { id: "DealerName", header: "Dealer", width: 200},
            { id: "SKPKNo", header: "SKPK No", width: 75},
            { id: "SKPKName", header: "SKPK Nama 1", width: 150},
            { id: "SKPKName2", header: "SKPK Nama 2", width: 150},
            { id: "SKPKAddr1", header: "SKPK Add 1", width: 200},
            { id: "SKPKAddr2", header: "SKPK Add 2", width: 200},
            { id: "SKPKAddr3", header: "SKPK Add 3", width: 200},
            { id: "SKPKCityCode", header: "SKPK Kota", width: 75},
            { id: "SKPKPhoneNo1", header: "SKPK Telp 1", width: 75},
            { id: "SKPKPhoneNo2", header: "SKPK Telp 2", width: 75},
            { id: "SKPKHPNo", header: "SKPK HP", width: 75},
            { id: "SKPKBirthday", header: "SKPK Tgl Lahir", width: 100},
            { id: "FPolName", header: "FPol Nama 1", width: 150},
            { id: "FPolName2", header: "FPol Nama 2", width: 150},
            { id: "FPolAddr1", header: "FPol Add 1", width: 200},
            { id: "FPolAddr2", header: "FPol Add 2", width: 200},
            { id: "FPolAddr3", header: "FPol Add 3", width: 200},
            { id: "FPolPostCode", header: "FPol Kode Pos", width: 75},
            { id: "FPolPostName", header: "FPol Nama Kode Pos", width: 150},
            { id: "FPolCityCode", header: "FPol Kota", width: 75},
            { id: "FPolKecamatanCode", header:"FPol Kec", width: 75},
            { id: "FPolPhoneNo1", header: "FPol Telp 1", width: 75},
            { id: "FPolPhoneNo2", header: "FPol Telp 2", width: 75},
            { id: "FPolHPNo", header: "FPol HP", width: 75},
            { id: "FPolBirthday", header: "FPol Tgl Lahir", width: 100},
            { id: "IdentificationNo", header: "No Identitas", width: 125 },
        ]
    });

    me.initialize = function () {
        me.status = "Status";
        $('#Status').html(me.status);
        $('#Status').css(
        {
            "font-size": "32px",
            "color": "red",
            "font-weight": "bold",
            "text-align": "center"
        });

        me.isPrintAvailable = true;
        me.isApprove = true;
        me.isCancel = false;

        me.griddetail.adjust();
    }

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Permohonan Faktur Polis (Sub-Dealer)",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
        ],
        //toolbars: WxButtons,
        panels: [
            {
                name: "pnlPermohonan",
                items: [
                    { name: "DealerCode", model: "data.DealerCode", cls: "span3", text: "Sub Dealer", disable: "isStatus", type: "popup", click: "DealerCode()" },
                    { name: "BacthNo", model: "data.BacthNo", text: "BacthNo", cls: "span3", readonly: true, disable: "isStatus" },
                    { name: "ReqNo", model: "data.ReqNo", text: "No.Permohonan", cls: "span3", placeHolder: "RTS/YY/XXXXXX", disable: true },
                    {
                        type: "controls",
                        text: "Tgl. Retur",
                        cls: "span5",
                        items: [
                            { name: "ReqDate", model: "data.ReqDate", type: "ng-datepicker", cls: "span3" },
                            {
                                type: "buttons", cls: "span3 left", items: [
                                    {
                                        name: "btnProses", text: "Proses", cls: "btn-small btn-info", icon: "icon-ok", click: "Proses()"
                                        //,disable: "data.Stat == 0 || data.Stat == 2 || data.Stat == 3"
                                    }
                                ]
                            },
                            { name: "Status", text: "", cls: "span2 center", readonly: true, type: "label" },
                        ]
                    },
                    { name: "ReffNo", model: "data.ReffNo", text: "No.Reff", cls: "span3", required: true, validasi: "required", disable: "isStatus" },
                    {
                        text: "Tgl.Reff",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "ReffDate", model: "data.ReffDate", placeHolder: "Tgl. Reff", cls: "span4", type: 'ng-datepicker', disable: "data.isActive1 == false || isStatus" },
                            { name: "isActive1", model: "data.isActive1", type: 'x-switch', cls: "span4", float: 'left', disable: "isStatus" },

                        ]
                    },
                    { name: "StatusFaktur", model: "data.StatusFaktur", text: "IsFaktur", type: 'check', cls: "span2 full", float: 'left', disable: "isStatus" },
                    { name: "Remark", model: "data.Remark", text: "Keterangan", cls: "span8", disable: "isStatus" },
                ]
            },
            {
                name: "wxkendaraan",
                xtype: "wxtable",
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omPermohonanFakturPolisDCSController");
    }



});