"use strict"

var pDok = "0";
var baseOn = "Gudang";
var isDetail = false;

function omReportInventoryPerincianStokController($scope, $http, $injector) {

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

    me.codeBrowse = function (e) {
        var lookup = Wx.blookup({
            name: "CodeBrowse",
            title: baseOn,
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("CodeBrowse").withParameters({baseOn : baseOn}),
            defaultSort: "Code asc",
            columns: [
               { field: 'Code', title: baseOn == "Gudang" ? 'Kode Gudang' : 'Model' },
               { field: 'Desc', title: baseOn == "Gudang" ? 'Nama Gudang' : 'Keterangan' },
            ]
        });
        lookup.dblClick(function (data) {
            if (e == 0) {
                me.data.CodeFrom = data.Code;
                me.data.CodeFromDesc = data.Desc;
            }
            else {
                me.data.CodeTo = data.Code;
                me.data.CodeToDesc = data.Desc;
            }
            me.Apply();
        }); 
    }

    me.printPreview = function () {
        var ReportId = "";
        var data = $(".main form").serializeObject();

        ReportId = (baseOn == "Gudang" && data.colourSwitch == "true") ? "OmRpInvRgs001A" : (baseOn == "Gudang" && data.colourSwitch == "false") ? "OmRpInvRgs001" : (baseOn == "Model" && data.colourSwitch == "true") ? "OmRpInvRgs002A" : "OmRpInvRgs002";
        var par = [
            moment(me.data.ByDate).format('YYYYMMDD'),
            me.data.CodeFrom,
            me.data.CodeTo,
            pDok
        ]
        var rparam = 'PERTANGGAL : ' + moment(me.data.ByDate).format('DD-MMMM-YYYY')

        if (isDetail) {
            $http.post('om.api/ReportInventory/ValidatePrintPerincianStok', me.data)
           .success(function (e) {
               if (e.success) {
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

    $("#detailSwitchN").on('change', function (e) {
        isDetail = false;
        $('#btnCodeFrom, #btnCodeTo').attr('disabled', 'disabled');
        $('#CodeFrom, #CodeFromDesc, #CodeTo, #CodeToDesc').val('');
    });
    $("#detailSwitchY").on('change', function (e) {
        isDetail = true;
        $('#btnCodeFrom, #btnCodeTo').removeAttr('disabled');
    });

    me.initialize = function () {
        me.data.ByDate = me.now();

        $('#btnCodeFrom, #btnCodeTo').attr('disabled', 'disabled');
    }

    me.optionsByDate = "0";
    me.optionsBaseOn = "Gudang";
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Perincian Stok",
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
                    {
                        type: "optionbuttons",
                        name: "tabpageoptions",
                        model: "optionsBaseOn",
                        text: "Berdasarkan",
                        items: [
                            { name: "Gudang", text: "Gudang" },
                            { name: "Model", text: "Model" },
                        ]
                    },
                    { name: "detailSwitch", cls: "span2 full", type: "switch" },
                    {
                        text: "kode Awal",
                        type: "controls",
                        items: [
                            { name: "CodeFrom", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click:"codeBrowse(0)" },
                            { name: "CodeFromDesc", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    {
                        text: "kode Akhir",
                        type: "controls",
                        items: [
                            { name: "CodeTo", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "codeBrowse(1)" },
                            { name: "CodeToDesc", cls: "span6", placeHolder: " ", readonly: true }
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
        SimDms.Angular("omReportInventoryPerincianStokController");
    }
});