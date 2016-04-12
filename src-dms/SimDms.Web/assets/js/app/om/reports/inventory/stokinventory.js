"use strict"

var pDok = "0";
var baseOn = "SI";
var baseOnOrder = "0";

var isGudang = false;
var isModel = false;

function omReportStockInventoryController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.$watch('optionsByDate', function (newValue, oldValue) {
        me.init();
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
            pDok = newValue;
        }
    });

    me.$watch('optionsBaseOn', function (newValue, oldValue) {
        me.init();
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
            baseOn = newValue;
        }
    });

    me.$watch('optionsBaseOnOrder', function (newValue, oldValue) {
        me.init();
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
            baseOnOrder = newValue == "Gudang" ? "0" : "1";
        }
    });

    me.codeBrowse = function (base, e) {
        var lookup = Wx.blookup({
            name: "CodeBrowse",
            title: base,
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("CodeBrowse").withParameters({ baseOn: base}),
            defaultSort: "Code asc",
            columns: [
               { field: 'Code', title: base == "Gudang" ? 'Kode Gudang' : 'Model' },
               { field: 'Desc', title: base == "Gudang" ? 'Nama Gudang' : 'Keterangan' },
            ]
        });
        lookup.dblClick(function (data) {
            if (base == "Gudang") {
                if (e == 0) {
                    me.data.GudangFrom = data.Code;
                    me.data.GudangFromDesc = data.Desc;
                }
                else {
                    me.data.GudangTo = data.Code;
                    me.data.GudangToDesc = data.Desc;
                }
            }
            else
            {
                if (e == 0) {
                    me.data.ModelFrom = data.Code;
                    me.data.ModelFromDesc = data.Desc;
                }
                else {
                    me.data.ModelTo = data.Code;
                    me.data.ModelToDesc = data.Desc;
                }
            }
            me.Apply();
        });
    }

    me.printPreview = function () {
        var ReportId = "";
        var title = "SEMUA MODEL";
        var data = $(".main form").serializeObject();

        if (baseOn == "SI") {
            console.log("si", baseOnOrder);
            if (baseOnOrder == "1") {
                if (data.colourSwitch == "true") {
                    ReportId = "OmRpInvRgs003E";
                }
                else ReportId = "OmRpInvRgs003";
            }
            else {
                if (data.colourSwitch == "true") ReportId = "OmRpInvRgs003D";
                else ReportId = "OmRpInvRgs003A";
            }
        }
        else {
            if (baseOnOrder == "1") {
                if (data.colourSwitch == "true") ReportId = "OmRpInvRgs003F";
                else ReportId = "OmRpInvRgs003B"; 
            }
            else {
                if (data.colourSwitch == "true") {
                    ReportId = "OmRpInvRgs003G";
                }
                else ReportId = "OmRpInvRgs003C";
            }
        }

        console.log(ReportId);

        var par = [
            moment(me.data.ByDate).format('YYYYMMDD'),
            me.data.GudangFrom,
            me.data.GudangTo,
            me.data.ModelFrom,
            me.data.ModelTo,
            pDok,
            baseOnOrder
        ]
        var rparam = 'PERTANGGAL : ' + moment(me.data.ByDate).format('DD-MMMM-YYYY')+','+ "MODEL : " + title

        if (isGudang || isModel) {

            var data = {
                isGudang : isGudang,
                CodeFrom: isGudang ? me.data.GudangFrom : me.data.ModelFrom,
                CodeTo : isGudang ? me.data.GudangTo : me.data.ModelTo
            };

            $http.post('om.api/ReportInventory/ValidatePrintStockInventory', data)
            .success(function (e) {
                if (e.success) {
                    if (isModel) {
                        title = "[" + me.data.ModelTo + "] " + me.data.ModelFromDesc + " S/D " + "[" + me.data.ModelFrom + "] " + me.data.ModelToDesc;
                        rparam = 'PERTANGGAL : ' + moment(me.data.ByDate).format('DD-MMMM-YYYY') + ',' + "MODEL : " + title
                    }
                    Wx.showPdfReport({
                        id: ReportId,
                        pparam: par.join(','),
                        rparam: rparam,
                        type: "devex"
                    });
                } else {
                    MsgBox(e.message, MSG_ERROR);
                    return;
                }
            })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
        }
        else
        {
            Wx.showPdfReport({
                id: ReportId,
                pparam: par.join(','),
                rparam: rparam,
                type: "devex"
            });
        }
    }

    $("#GudangSwitchN").on('change', function (e) {
        isGudang = false;
        $('#btnGudangFrom, #btnGudangTo').attr('disabled', 'disabled');
        $('#GudangFrom, #GudangFromDesc, #GudangTo, #GudangToDesc').val('');
    });
    $("#GudangSwitchY").on('change', function (e) {
        isGudang = true;
        $('#btnGudangFrom, #btnGudangTo').removeAttr('disabled');
    });

    $("#ModelSwitchN").on('change', function (e) {
        isModel = false;
        $('#btnModelFrom, #btnModelTo').attr('disabled', 'disabled');
        $('#ModelFrom, #ModelFromDesc, #ModelTo, #ModelToDesc').val('');
    });
    $("#ModelSwitchY").on('change', function (e) {
        isModel = true;
        $('#btnModelFrom, #btnModelTo').removeAttr('disabled');
    });

    me.initialize = function () {
        me.data.ByDate = me.now();

        $('#btnModelFrom, #btnModelTo,#btnGudangFrom, #btnGudangTo').attr('disabled', 'disabled');
    }
    me.optionsByDate = "0";
    me.optionsBaseOn = "SI";
    me.optionsBaseOnOrder = "Gudang";
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Stok Inventory",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
           {
               name: "pnlPerincianStok",
               items: [
                   {
                       type: "optionbuttons",
                       name: "tabpageoptions",
                       model: "optionsByDate",
                       text: "Berdasarkan Tgl.",
                       items: [
                           { name: "0", text: "BPU" },
                           { name: "1", text: "DO Supplier" },
                           { name: "2", text: "SJ SUpplier" },
                       ]
                   },
                   { name: "ByDate", text: "Per Tanggal", cls: "span4", type: "ng-datepicker" },
                   { name: "GudangSwitch",text:"Gudang", cls: "span2 full", type: "switch" },
                   {
                       text: "kode Gudang Awal",
                       type: "controls",
                       items: [
                           { name: "GudangFrom", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "codeBrowse('Gudang',0)" },
                           { name: "GudangFromDesc", cls: "span6", placeHolder: " ", readonly: true }
                       ]
                   },
                   {
                       text: "kode Gudang Akhir",
                       type: "controls",
                       items: [
                           { name: "GudangTo", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "codeBrowse('Gudang',1)" },
                           { name: "GudangToDesc", cls: "span6", placeHolder: " ", readonly: true }
                       ]
                   },
                   { name: "ModelSwitch",text:"Model", cls: "span2 full", type: "switch" },
                   {
                       text: "kode Model Awal",
                       type: "controls",
                       items: [
                           { name: "ModelFrom", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "codeBrowse('Model',0)" },
                           { name: "ModelFromDesc", cls: "span6", placeHolder: " ", readonly: true }
                       ]
                   },
                   {
                       text: "kode Model Akhir",
                       type: "controls",
                       items: [
                           { name: "ModelTo", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "codeBrowse('Model',1)" },
                           { name: "ModelToDesc", cls: "span6", placeHolder: " ", readonly: true }
                       ]
                   },
                    {
                        type: "optionbuttons",
                        name: "tabpageoptions",
                        model: "optionsBaseOn",
                        text: "Berdasarkan",
                        items: [
                            { name: "SI", text: "Stok Inventory" },
                            { name: "SA", text: "Stok Alokasi" },
                        ]
                    },
                     {
                         type: "optionbuttons",
                         name: "tabpageoptions",
                         model: "optionsBaseOnOrder",
                         text: "Urut Berdasarkan",
                         items: [
                             { name: "Gudang", text: "Gudang" },
                             { name: "Model", text: "Model" },
                         ]
                     },
                   { name: "colourSwitch", text: "", cls: "span2", type: "switch" },
                   { name: "lblColour", text: "Tampilkan Deskripsi Warna", cls: "span4", readonly: true, type: "label" },

               ]
           }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("omReportStockInventoryController");
    }
});