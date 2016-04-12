"use strict";

var baseOnModul = "0";
var baseOnStock = "SI";
var baseOnOrder = "0";

var isGudang = false;
var isModel = false;

function RptStockInventoryV2($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.$watch('optionByModul', function (newValue, oldValue) {
        me.init();
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
            baseOnModul = newValue;
        }
    });

    me.$watch('optionByStock', function (newValue, oldValue) {
        me.init();
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
            baseOnStock = newValue;
        }
    });

    me.$watch('optionByOrder', function (newValue, oldValue) {
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


    $http.post('gn.api/combo/Organizations').
   success(function (data, status, headers, config) {
       me.Company = data;
   });
    $http.post('gn.api/combo/Branchs').
    success(function (data, status, headers, config) {
        me.Branch = data;
    });

    me.GudangFrom = function () {
        var lookup = Wx.blookup({
            name: "CityLookup",
            title: "Lookup Sales",
            manager: gnManager,
            query: new breeze.EntityQuery().from("LookUpDtlAll").withParameters({ param: "GPAR" }),
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "Kode Sales" },
                { field: "LookUpValueName", title: "Deskripsi" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.SalesCodeFrom = data.LookUpValue;
                me.data.SalesNameFrom = data.LookUpValueName;
                me.Apply();
            }
        });

    }

    me.printPreview = function () {
        var ReportId = "";
        var title = "SEMUA MODEL";
        var data = $(".main form").serializeObject();
        var tipeSP = "";

        if (baseOnStock == "SI") {
            if (baseOnOrder == "1") {
                if ($('#chkColor').prop('checked') == true) {
                    ReportId = "OmRpInvRgs003E";
                }
                else ReportId = "OmRpInvRgs003";
            }
            else {
                if ($('#chkColor').prop('checked') == true) ReportId = "OmRpInvRgs003D";
                else ReportId = "OmRpInvRgs003A";
            }

            tipeSP = "1";
        }
        else {
            if (baseOnOrder == "1") {
                if ($('#chkColor').prop('checked') == true) ReportId = "OmRpInvRgs003F";
                else ReportId = "OmRpInvRgs003B";
            }
            else {
                if ($('#chkColor').prop('checked') == true) {
                    ReportId = "OmRpInvRgs003G";
                }
                else ReportId = "OmRpInvRgs003C";
            }

            tipeSP = "2";
        }

        var par = "";

        if ($('#chkCompany').prop('checked') == true) {

            var par = [
                'companycode',
                me.data.BranchCode,
                moment(me.data.ByDate).format('YYYYMMDD'),
                me.data.GudangFrom,
                me.data.GudangTo,
                me.data.ModelFrom,
                me.data.ModelTo,
                baseOnModul,
                baseOnOrder
            ];
        }
        else {
            var par = [
                moment(me.data.ByDate).format('YYYYMMDD'),
                me.data.GudangFrom,
                me.data.GudangTo,
                me.data.ModelFrom,
                me.data.ModelTo,
                baseOnModul,
                baseOnOrder
            ];
        }

        console.log(par);
        var rparam = 'PERTANGGAL : ' + moment(me.data.ByDate).format('DD-MMMM-YYYY') + ',' + "MODEL : " + title

        if (isGudang || isModel) {

            var data = {
                isGudang: isGudang,
                CodeFrom: isGudang ? me.data.GudangFrom : me.data.ModelFrom,
                CodeTo: isGudang ? me.data.GudangTo : me.data.ModelTo
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
        else {
            Wx.showPdfReport({
                id: ReportId,
                pparam: par.join(','),
                rparam: rparam,
                type: "devex"
            });
        }


    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        me.data.Dates = me.now();

        $('#BranchCode').attr('disabled', true);
        $('#CompanyCode').attr('disabled', true);
        $('#GudangFrom').attr('disabled', true);
        $('#GudangFromDesc').attr('disabled', true);
        $('#GudangTo').attr('disabled', true);
        $('#GudangToDesc').attr('disabled', true);
        $('#ModelFrom').attr('disabled', true);
        $('#ModelFromDesc').attr('disabled', true);
        $('#ModelTo').attr('disabled', true);
        $('#ModelToDesc').attr('disabled', true);


        me.isPrintAvailable = true;
    }

    $('#chkCompany').on('change', function (e) {
        if ($('#chkCompany').prop('checked') == true) {
            $('#BranchCode').removeAttr('disabled');
            $('#CompanyCode').removeAttr('disabled');
            $http.get('breeze/sales/CurrentUserInfo').
              success(function (dl, status, headers, config) {
                  me.data.CompanyCode = dl.CompanyCode;
                  me.data.BranchCode = dl.BranchCode;
              });
        } else {
            $('#BranchCode').attr('disabled', true);
            $('#CompanyCode').attr('disabled', true);
            me.data.CompanyCode = undefined;
            me.data.BranchCode = undefined;
        }
        me.Apply();
    })

    $('#chkGudang').on('change', function (e) {
        if ($('#chkGudang').prop('checked') == true) {
            isGudang = true;
            $('#GudangFrom').removeAttr('disabled');
            $('#GudangFromDesc').removeAttr('disabled');
            $('#GudangTo').removeAttr('disabled');
            $('#GudangToDesc').removeAttr('disabled');
        } else {
            isGudang = false;
            $('#GudangFrom').attr('disabled', true);
            $('#GudangFromDesc').attr('disabled', true);
            $('#GudangTo').attr('disabled', true);
            $('#GudangToDesc').attr('disabled', true);
            me.data.GudangFrom = undefined;
            me.data.GudangFromDesc = undefined;
            me.data.GudangTo = undefined;
            me.data.GudangToDesc = undefined;
        }
        me.Apply();
    })

    $('#chkModel').on('change', function (e) {
        if ($('#chkModel').prop('checked') == true) {
            isModel = true;
            $('#ModelFrom').removeAttr('disabled');
            $('#ModelFromDesc').removeAttr('disabled');
            $('#ModelTo').removeAttr('disabled');
            $('#ModelTo').removeAttr('disabled');
        } else {
            isModel = false;
            $('#ModelFrom').attr('disabled', true);
            $('#ModelFromDesc').attr('disabled', true);
            $('#ModelTo').attr('disabled', true);
            $('#ModelToDesc').attr('disabled', true);
            me.data.ModelFrom = undefined;
            me.data.ModelFromDesc = undefined;
            me.data.ModelTo = undefined;
            me.data.ModelToDesc = undefined;
        }
        me.Apply();
    })


    me.optionByModul = "0";
    me.optionByStock = "SI";
    me.optionByOrder = "Gudang";
    me.start();

    me.start();
    me.options = '0';

}

$(document).ready(function () {
    var options = {
        title: "Laporan Stock Inventory V.2",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: 'chkCompany', type: 'check', text: 'Cabang', cls: 'span1 full', float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "CompanyCode", opt_text: "", cls: "span7", type: "select2", text: "Company", datasource: "Company" },
                                { name: "BranchCode", opt_text: "", cls: "span7", type: "select2", text: "Branch", datasource: "Branch" },
                            ]
                        },

                        {
                            text: "Berdasarkan Tanggal",
                            type: "controls",
                            items: [
                                {
                                    type: "optionbuttons",
                                    name: "tabpageoptions",
                                    model: "optionByModul",
                                    items: [
                                        { name: "0", text: "BPU" },
                                        { name: "1", text: "DO Supplier" },
                                        { name: "2", text: "SJ Supplier" },
                                    ]
                                },
                            ]
                        },
                        { name: "Dates", text: "Tanggal", model: "data.Dates", placeHolder: "Tanggal", cls: "span4", type: 'ng-datepicker' },
                        { name: "chkGudang", model: "data.chkGudang", text: "Gudang", cls: "span4 full ", type: "ng-check" },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "GudangFrom", model: "data.GudangFrom", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "codeBrowse('Gudang',0)", disable: "!data.chkGudang" },
                                { name: "GudangFromDesc", model: "data.GudangFromDesc", cls: "span5", placeHolder: " ", readonly: true, disable: "!data.chkGudang" }
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "GudangTo", model: "data.GudangTo", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "codeBrowse('Gudang',1)", disable: "!data.chkGudang" },
                                { name: "GudangToDesc", model: "data.GudangToDesc", cls: "span5", placeHolder: " ", readonly: true, disable: "!data.chkGudang" }
                            ]
                        },
                        { name: "chkModel", model: "data.chkModel", text: "Model", cls: "span4 full ", type: "ng-check" },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "ModelFrom", model: "data.ModelFrom", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "codeBrowse('Model',0)", disable: "!data.chkModel" },
                                { name: "ModelFromDesc", model: "data.ModelFromDesc", cls: "span5", placeHolder: " ", readonly: true, disable: "!data.chkModel" }
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "ModelTo", model: "data.ModelTo", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "codeBrowse('Model',1)", disable: "!data.chkModel" },
                                { name: "ModelToDesc", model: "data.ModelToDesc", cls: "span5", placeHolder: " ", readonly: true, disable: "!data.chkModel" }
                            ]
                        },
                        {
                            text: "Berdasarkan",
                            type: "controls",
                            items: [
                                {
                                    type: "optionbuttons",
                                    name: "tabpageoptions",
                                    model: "optionByStock",
                                    items: [
                                        { name: "SI", text: "Stock Inventory" },
                                        { name: "SA", text: "Stock Alokasi" }
                                    ]
                                },
                            ]
                        },
                        {
                            text: "Urut Berdasarkan",
                            type: "controls",
                            items: [
                                {
                                    type: "optionbuttons",
                                    name: "tabpageoptions",
                                    model: "optionByOrder",
                                    items: [
                                        { name: "Gudang", text: "Gudang" },
                                        { name: "Model", text: "Model" }
                                    ]
                                },
                            ]
                        },

                        { name: "chkColor", model: "data.chkColor", text: "Deskripsi Warna", cls: "span4 full ", type: "ng-check" },

                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        $(".switchlabel").attr("style", "padding:9px 0px 0px 5px")
        SimDms.Angular("RptStockInventoryV2");

    }
});