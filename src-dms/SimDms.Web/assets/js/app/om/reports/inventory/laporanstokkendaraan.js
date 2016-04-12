"use strict"

var orderBy = "0"

var isGudang = false;
var isModel = false;
var isColour = false;

function omReportStokKendaraanController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/Months').
    success(function (data, status, headers, config) {
        me.comboMonth = data;
    });

    me.default = function () {
        $http.post('om.api/reportinventory/StockVehicleDate').
        success(function (data) {
            $('#Month').select2('val', data.Month);
            me.data.Year = data.Year;
        });
    }

    me.$watch('options', function (newValue, oldValue) {
        me.init();
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
            orderBy = newValue;
        }
    });

    me.codeBrowse = function (base, e) {
        var lookup = Wx.blookup({
            name: "CodeBrowse",
            title: base,
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("CodeBrowse").withParameters({ baseOn: base }),
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
            else {
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

    me.colourBrowse = function (e) {
        var lookup = Wx.blookup({
            name: "ColourBrowse",
            title: "Warna",
            manager: spSalesManager,
            query: "ColourBrowse",
            defaultSort: "Code asc",
            columns: [
               { field: 'Code', title: 'Kode Warna' },
               { field: 'Desc', title: 'Deskripsi Warna' },
            ]
        });
        lookup.dblClick(function (data) {
                if (e == 0) {
                    me.data.ColourFrom = data.Code;
                    me.data.ColourFromDesc = data.Desc;
                }
                else {
                    me.data.ColourTo = data.Code;
                    me.data.ColourToDesc = data.Desc;
                }
            me.Apply();
        });
    }

    me.printPreview = function () {
        var ReportId = "";
        var data = $(".main form").serializeObject();
        
            ReportId = orderBy == "0" ? "OmRpInvRgs004B" :"OmRpInvRgs004";
          
            var par = [
                me.data.Year,
                (parseInt($('#Month').select2('val')) + 1),
                me.data.GudangFrom,
                me.data.GudangTo,
                me.data.ModelFrom,
                me.data.ModelTo,
                me.data.YearFrom,
                me.data.YearTo,
                me.data.ColourFrom,
                me.data.ColourTo,
                orderBy
            ]

            var rparam = 'TAHUN : ' + me.data.Year + ' BULAN : ' + $('#Month').select2('data').text;
            if (isGudang) {
                var data = {
                    validate: "Gudang",
                    CodeFrom: me.data.GudangFrom,
                    CodeTo: me.data.GudangTo
                };

                $http.post('om.api/ReportInventory/ValidatePrint', data)
                .success(function (e) {
                    if (e.success) {
                        if(!isModel || !isColour)
                        {
                            Wx.showPdfReport({
                                id: ReportId,
                                pparam: par.join(','),
                                rparam: rparam,
                                type: "devex"
                            });
                        }
                    }
                    else {
                        MsgBox(e.message, MSG_ERROR);
                        return;
                    }
                })
                .error(function (e) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });
            }
            if (isModel) {
                var data = {
                    validate: "Model",
                    CodeFrom: me.data.ModelFrom,
                    CodeTo: me.data.ModelTo
                };
                $http.post('om.api/ReportInventory/ValidatePrint', data)
                .success(function (e) {
                    if (e.success) {
                        if (!isGudang || !isColour) {
                            Wx.showPdfReport({
                                id: ReportId,
                                pparam: par.join(','),
                                rparam: rparam,
                                type: "devex"
                            });
                        }
                    }
                    else {
                        MsgBox(e.message, MSG_ERROR);
                        return;
                    }
                })
                .error(function (e) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });
            }
            if(isColour){
                var data = {
                    validate: "Colour",
                    CodeFrom: me.data.ColourFrom,
                    CodeTo: me.data.ColourTo
                }
                $http.post('om.api/ReportInventory/ValidatePrint', data)
                .success(function (e) {
                    if (e.success) {
                        if (!isModel || !isGudang) {
                            Wx.showPdfReport({
                                id: ReportId,
                                pparam: par.join(','),
                                rparam: rparam,
                                type: "devex"
                            });
                        }
                    }
                    else {
                        MsgBox(e.message, MSG_ERROR);
                        return;
                    }
                })
                .error(function (e) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });
            }

            if (!isGudang && !isModel && !isColour) {
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
        isModel = true
        $('#btnModelFrom, #btnModelTo').removeAttr('disabled');
    });

    $("#YearSwitchN").on('change', function (e) {
        $('#YearFrom, #YearTo').attr('readonly', true);
        $('#YearFrom, #YearTo').val('');
    });
    $("#YearSwitchY").on('change', function (e) {
        $('#YearFrom, #YearTo').removeAttr('readonly');
    });

    $("#ColourSwitchN").on('change', function (e) 
    {
        isColour = false;
        $('#btnColourFrom, #btnColourTo').attr('disabled', 'disabled');
        $('#ColourFrom, #ColourFromDesc, #ColourTo, #ColourToDesc').val('');
    });
    $("#ColourSwitchY").on('change', function (e) {
        isColour = true;
        $('#btnColourFrom, #btnColourTo').removeAttr('disabled');
    });

    me.initialize = function () {
        me.default();

        $('#btnModelFrom, #btnModelTo,#btnGudangFrom, #btnGudangTo, #btnColourFrom, #btnColourTo').attr('disabled', 'disabled');
    }
   
    me.options = "0";
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Laporan Stok Kendaraan",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlPerincianStok",
                items: [
                    { name: "Year", required: true, text: "Tahun", cls: "span4 full" },
                    { name: "Month", cls: "span4", text: "Bulan", type: "select2", datasource: "comboMonth" },
                    { name: "GudangSwitch", text: "Gudang", cls: "span2 full", type: "switch" },
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
                    { name: "ModelSwitch", text: "Sales Model Code", cls: "span2 full", type: "switch" },
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
                    { name: "YearSwitch", text: "Sales Model Year", cls: "span2", type: "switch" },
                    { name: "YearFrom",cls: "span3", readonly: true },
                    { name: "YearTo",text:"S/D",placeHolder:" ", cls: "span3", readonly: true },
                    { name: "ColourSwitch", text: "Warna", cls: "span2 full", type: "switch" },
                    {
                        type: "controls",
                        items: [
                            { name: "ColourFrom", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "colourBrowse(0)" },
                            { name: "ColourFromDesc", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    {
                        type: "controls",
                        items: [
                            { name: "ColourTo", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "colourBrowse(1)" },
                            { name: "ColourToDesc", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                   
                      {
                          type: "optionbuttons",
                          name: "tabpageoptions",
                          model: "options",
                          text: "Urut Berdasarkan",
                          items: [
                              { name: "0", text: "Gudang" },
                              { name: "1", text: "Model" },
                          ]
                      },
                  

                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("omReportStokKendaraanController");
    }
});