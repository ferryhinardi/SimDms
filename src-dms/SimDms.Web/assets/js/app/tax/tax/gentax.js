var fromDate = ''
var endDate = ''
var bFPJCentralized = false;
var bChooseProfitCenter = false;
var isLinkFin = false;
var isLinkSP = false;
var isLinkSV = false;
var isLinkSL = false;

function taxGenTaxController($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    $("#bChooseProfitCenter").on('click', function (e) {
        if ($("#bChooseProfitCenter").is(':checked')) {
            $('#ProfitCenter').attr('disabled', 'disabled');
            $('#pnlB').hide();
            $('#pnlB2').show();
            me.clearTable(me.grid1);
            me.clearTable(me.grid2);
            $('#btnGenerate').attr('disabled', 'disabled');
            bChooseProfitCenter = true;
        }
        else {
            $('#ProfitCenter').removeAttr('disabled');
            $('#pnlB').show();
            $('#pnlB2').hide();
            me.clearTable(me.grid1);
            me.clearTable(me.grid2);
            $('#btnGenerate').attr('disabled', 'disabled');
            bChooseProfitCenter = false;
        }
        console.log(bChooseProfitCenter);
    });

    $("#bFPJG").on('click', function (e) {
        if ($("#btnGenerate").is(':enabled')) {
            me.queryFPJ();
        }
    });

    $http.post('gn.api/Combo/ProfitCenters').
   success(function (data, status, headers, config) {
       me.ProfitCenters = data;
   });

    me.generateFPJ = function () {
        me.griddetail = [];
        var docNo = '|';

        if (bChooseProfitCenter == false) {
            $.each(me.grid1.detail, function (key, value) {
                if (value.chkSelect == 1) {
                    me.griddetail.push(value.BranchCode + ' ' + value.DocNo);
                    docNo += value.BranchCode + ' ' + value.DocNo + "|";
                }
            });

        }
        else
            me.griddetail.length = me.grid2.detail.length;


        if (me.griddetail.length === 0) {
            MsgBox("Tidak ada Faktur Pajak yang dipilih");
        }
        else {
            var fpjDate = '';
            var chkAll = false;
            var profitCenter = ''

            if (bChooseProfitCenter == false)
                profitCenter = $('#ProfitCenter').val();

            if (me.griddetail.length == me.grid2.detail.length) {
                chkAll = true;
                docNo = '';
            }

            if (chkAll == true && $('#ProfitCenter').val() == "000") {
                fpjDate = $('input[name="DateTo"]').val();
            }
            else {
                profitCenter = $('#ProfitCenter').val();
                //if (me.griddetail.length > 0) fpjDate = me.grid1.detail[me.griddetail.length - 1].DocDate;
                //else
                    fpjDate = $('input[name="DateTo"]').val();
            }

            MsgConfirm("Apakah anda yakin ?", function (result) {
                if (result) {
                    $(".page .ajax-loader").fadeIn();
                    $.post('tax.api/GenTax/IsValidTransDate?startDate=' + fromDate + '&fpjDate=' + fpjDate + '&docNo=' + docNo + '&profitCenter=' + profitCenter + '&isFPJCentral='
                    + bFPJCentralized + '&isLinkFin=' + isLinkFin + '&isLinkSP=' + isLinkSP + '&isLinkSV=' + isLinkSV + '&isLinkSL=' + isLinkSL + '&bCheckAll=' + chkAll
                    + '&isFPJGU=' + $("#bFPJG").is(':checked'))
                    .success(function (data, status, headers, config) {
                        if (data.success == false) {
                            if (data.bProcess == false && chkAll == false) {
                                MsgBox(data.message);
                                $(".page .ajax-loader").fadeOut();
                                if (data.bPending == true) {
                                    me.clearTable(me.grid3);
                                    me.grid3.detail = data.data;
                                    me.loadTableData(me.grid3, me.grid3.detail);
                                }
                            }
                            else {
                                if (data.bPending == true) {
                                    me.clearTable(me.grid3);
                                    me.grid3.detail = data.data;
                                    me.loadTableData(me.grid3, me.grid3.detail);
                                }
                                if (data.bProcess == true) {
                                    $.post('tax.api/GenTax/GenerateTax?startDate=' + fromDate + '&fpjDate=' + fpjDate + '&docNo=' + docNo + '&profitCenter=' +
                                                profitCenter + '&isFPJCentral=' + bFPJCentralized + '&isFPJGU=' + $("#bFPJG").is(':checked') + '&bCheckAll=' + chkAll).success(function (data, status, headers, config) {
                                                    if (data.success == true) {
                                                        MsgBox("Faktur Pajak berhasil di-generate", MSG_SUCCESS);
                                                        $('#btnGenerate, #pnlB, #pnlB2').attr('disabled', 'disabled');
                                                        me.clearTable(me.grid1);
                                                        me.clearTable(me.grid2);
                                                        me.clearTable(me.grid3);

                                                        me.grid1.detail = data.data;
                                                        me.loadTableData(me.grid1, me.grid1.detail);
                                                        me.grid2.detail = data.data;
                                                        me.loadTableData(me.grid2, me.grid2.detail);

                                                        $(".page .ajax-loader").fadeOut();
                                                    }
                                                    else {
                                                        MsgBox(data.message, MSG_ERROR);
                                                        $(".page .ajax-loader").fadeOut();
                                                    }
                                                });
                                }
                                else {
                                    $(".page .ajax-loader").fadeOut();
                                    MsgConfirm(data.message + '\r\n \r\n  Ingin Lanjut ?', function (result) {
                                        if (result) {
                                            $(".page .ajax-loader").fadeIn();
                                            $.post('tax.api/GenTax/GenerateTax?startDate=' + fromDate + '&fpjDate=' + fpjDate + '&docNo=' + docNo + '&profitCenter=' +
                                                profitCenter + '&isFPJCentral=' + bFPJCentralized + '&isFPJGU=' + $("#bFPJG").is(':checked') + '&bCheckAll=' + chkAll).success(function (data, status, headers, config) {
                                                    if (data.success == true) {
                                                        MsgBox("Faktur Pajak berhasil di-generate", MSG_SUCCESS);
                                                        $('#btnGenerate, #pnlB, #pnlB2').attr('disabled', 'disabled');
                                                        me.clearTable(me.grid1);
                                                        me.clearTable(me.grid2);
                                                        me.clearTable(me.grid3);

                                                        me.grid1.detail = data.data;
                                                        me.loadTableData(me.grid1, me.grid1.detail);
                                                        me.grid2.detail = data.data;
                                                        me.loadTableData(me.grid2, me.grid2.detail);

                                                        $(".page .ajax-loader").fadeOut();
                                                    }
                                                    else {
                                                        MsgBox(data.message, MSG_ERROR);
                                                        $(".page .ajax-loader").fadeOut();
                                                    }
                                                });
                                        }
                                    });
                                }
                            }
                        }
                        else {
                            $.post('tax.api/GenTax/GenerateTax?startDate=' + fromDate + '&fpjDate=' + fpjDate + '&docNo=' + docNo + '&profitCenter=' +
                                        profitCenter + '&isFPJCentral=' + bFPJCentralized + '&isFPJGU=' + $("#bFPJG").is(':checked') + '&bCheckAll=' + chkAll).success(function (data, status, headers, config) {
                                            if (data.success == true) {
                                                MsgBox("Faktur Pajak berhasil di-generate", MSG_SUCCESS);
                                                $('#btnGenerate, #pnlB, #pnlB2').attr('disabled', 'disabled');
                                                me.clearTable(me.grid1);
                                                me.clearTable(me.grid2);
                                                me.clearTable(me.grid3);

                                                me.grid1.detail = data.data;
                                                me.loadTableData(me.grid1, me.grid1.detail);
                                                me.grid2.detail = data.data;
                                                me.loadTableData(me.grid2, me.grid2.detail);

                                                $(".page .ajax-loader").fadeOut();
                                            }
                                            else {
                                                MsgBox(data.message, MSG_ERROR);

                                                $(".page .ajax-loader").fadeOut();
                                            }
                                        });
                        }
                    });
                }
            });
        }
    }

    me.refreshGrid = function (grid1, grid2) {
        $.post('tax.api/GenTax/PreviewFPJGenerated?startDate=' + fromDate + '&endDate=' + endDate + '&isFPJCentral=' + bFPJCentralized + '&profitCenter=' + me.ProfitCenter)
        .success(function (data, status, headers, config) {
            if (data.success == true) {
                me.grid1.detail = data.data;
                me.loadTableData(me.grid1, me.grid1.detail);
                me.grid2.detail = data.data;
                me.loadTableData(me.grid2, me.grid2.detail);
            }
        });
    }

    me.RefreshDisplay = function () {
        me.clearTable(me.grid1);
        me.clearTable(me.grid2);
        $('#btnGenerate').attr('disabled', 'disabled');
    }

    me.queryFPJ = function () {
        me.clearTable(me.grid1);
        me.clearTable(me.grid2);
        me.clearTable(me.grid3);

        var profitCenter = '%';

        if (bChooseProfitCenter == true)
        {
            if ($('#ProfitCenter').val() != "000")
                profitCenter = me.ProfitCenter;
        }
        else {
            profitCenter = me.ProfitCenter;
        }
        console.log(profitCenter);

        $.post('tax.api/GenTax/ValidateQueryFpj?hiddenFromDate=' + fromDate + '&hiddenEndDate=' + endDate + '&fpjDate=' + $('input[name="DateTo"]').val() +
            '&bFPJCentralized=' + bFPJCentralized + '&profitCenter=' + profitCenter + '&bFPJG=' + $("#bFPJG").is(':checked')).success(function (data, status, headers, config) {
                if (data.message != '') {
                    Wx.alert(data.message);
                    $('#btnGenerate').attr('disabled', 'disabled');
                }
                else {
                    me.grid1.detail = data.data;
                    me.loadTableData(me.grid1, me.grid1.detail);
                    me.grid2.detail = data.data;
                    me.loadTableData(me.grid2, me.grid2.detail);

                    if (me.grid1.detail.length > 0) {
                        $('#btnGenerate').removeAttr('disabled');
                    }
                    else {
                        $('#btnGenerate').attr('disabled', 'disabled');
                    }
                }
            });
    }

    me.initialize = function () {
        me.griddetail = [];
        me.clearTable(me.grid1);
        me.clearTable(me.grid2);
        me.clearTable(me.grid3);
        $('#pnlB2').hide();
        $('#pnlB').show();

        $.post('tax.api/GenTax/Init').success(function (data, status, headers, config) {
            $('input[name="DateTo"]').val(data.fpjDate);
            $('#btnGenerate').attr('disabled', 'disabled');
            isLinkFin = data.isLinkFin;
            isLinkSP = data.isLinkSP;
            isLinkSV = data.isLinkSV;
            isLinkSL = data.isLinkSL;
            if (data.isLinkFin == true) {
                $('#btnPeriod, #Period').attr('disabled', 'disabled');
                $('#Period').val(data.periodeBegAR);
                $('#PeriodName').val('Dari Tanggal ' + data.periodeBegAR + ' S/D ' + data.periodeEndAR);
                fromDate = data.periodeBegAR;
                endDate = data.periodeEndAR;
            }
            else {
                $('#btnPeriod, #Period').removeAttr('disabled');
            }
            if (data.isHolding == true) {
                $('#btnPeriod, #Period, #ProfitCenter, #bChooseProfitCenterY, #bChooseProfitCenterN, #btnQuery, #btnGenerate').attr('disabled', 'disabled');
                $('input[name="DateTo"]').attr('disabled', 'disabled');
            }

            me.ProfitCenter = data.ProfitCenter;

            if (me.ProfitCenter == "000") {
                $('#btnPeriod, #ProfitCenter, #bChooseProfitCenterY, #bChooseProfitCenterN').removeAttr('disabled');
            }
            else {
                $('#btnPeriod, #ProfitCenter, #bChooseProfitCenterY, #bChooseProfitCenterN').attr('disabled', 'disabled');
            }

            bFPJCentralized = data.bFPJCentralized;

            me.Apply();
        });
    }

    me.grid1 = new webix.ui({
        container: "wxtaxdetail",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "chkSelect", header: { content: "masterCheckbox", contentId: "mc1" }, template: "{common.checkbox()}", width: 40 },
            { id: "No", header: "No", width: 50 },
            { id: "BranchCode", header: "Kode Branch", fillspace: true },
            { id: "ProfitCenter", header: "Profit Center", fillspace: true },
            { id: "FPJGovNo", header: "No FPJ", width: 150 },
            { id: "FPJGovDate", header: "Tgl FPJ", width: 100 },
            { id: "DocNo", header: "No Dokumen", fillspace: true },
            { id: "DocDate", header: "Tgl Dokumen", width: 100 },
            { id: "CustName", header: "Nama Cust", fillspace: true },
            { id: "InvNo", header: "No Inv", fillspace: true }

        ],
        //checkboxRefresh: true,
        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.grid1.getSelectedId().id);
                    me.detail.oid = me.grid1.getSelectedId();
                    me.Apply();
                }
            }
        }
    });

    me.grid2 = new webix.ui({
        container: "wxtaxdetail2",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "No", header: "No", width: 50 },
            { id: "BranchCode", header: "Kode Branch", fillspace: true },
            { id: "ProfitCenter", header: "Profit Center", fillspace: true },
            { id: "FPJGovNo", header: "No FPJ", width: 150 },
            { id: "FPJGovDate", header: "Tgl FPJ", width: 100 },
            { id: "DocNo", header: "No Dokumen", fillspace: true },
            { id: "DocDate", header: "Tgl Dokumen", width: 100 },
            { id: "CustName", header: "Nama Cust", fillspace: true },
            { id: "InvNo", header: "No Inv", fillspace: true }
        ],
        checkboxRefresh: true,
        on: {
            onSelectChange: function () {
                if (me.grid3.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.grid3.getSelectedId().id);
                    me.detail.oid = me.grid3.getSelectedId();
                    me.Apply();
                }
            }
        }
    });

    me.grid3 = new webix.ui({
        container: "wxpendingdoclist",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "BranchCode", header: "Kode Branch", fillspace: true },
            { id: "ProfitCenter", header: "Profit Center", fillspace: true },
            { id: "InvoiceNo", header: "No Invoice", fillspace: true },
            { id: "InvoiceDate", header: "Tgl Invoice", fillspace: true },
            { id: "Status", header: "Status", fillspace: true },
        ],
        checkboxRefresh: true
    });

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Generate Faktur Pajak",
        xtype: "panels",
        panels: [
           {
               name: "pnlA",
               title: "",
               items: [
                   {
                       text: "Period",
                       type: "controls",
                       items: [
                           { name: "Period", cls: "span2", placeHolder: "Periode", type: "popup", btnName: "btnPeriod" },
                           { name: "PeriodName", cls: "span6", placeHolder: "Nama Periode", readonly: true },
                       ]
                   },
                   { name: "DateTo", text: "s/d Tanggal", cls: "span3", type: "datepicker", format: 'DD MMM YYYY' },
                   { name: "bFPJG", text: "FPJ Gabungan", type: "check", cls: "span2" },
                   {
                       type: "controls",
                       text: "Profit Center",
                       items: [
                        { name: "bChooseProfitCenter", text: "Profit Center", type: "check", cls: "span1" },
                        { name: "ProfitCenter", text: "Profit Center", cls: "span2", type: "select2", datasource: "ProfitCenters", model: "ProfitCenter", change: "RefreshDisplay()" },
                       ],
                   },

                   {
                       type: "buttons",
                       items: [
                             { name: "btnQuery", text: "QUERY", icon: "icon-plus", cls: "btn btn-success", click: "queryFPJ()" },
                             { name: "btnGenerate", text: "GENERATE", icon: "icon-plus", cls: "btn btn-success", click: "generateFPJ()" }
                       ]
                   },

               ]
           },
           {
               name: "pnlB",
               title: "Daftar Faktur Pajak",
               items: [
                    {
                        name: "wxtaxdetail",
                        type: "wxdiv",
                    },
               ]
           },
            {
                name: "pnlB2",
                title: "Daftar Faktur Pajak",
                items: [
                     {
                         name: "wxtaxdetail2",
                         type: "wxdiv",
                     },
                ]
            },
           {
               name: "pnlC",
               title: "List Dokumen Pending",
               items: [
                   {
                       name: "wxpendingdoclist",
                       type: "wxdiv"
                   }
               ]
           }
        ]
    }

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("taxGenTaxController");
    }
});