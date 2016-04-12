var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function sppengeluaranfakturpenjualan($scope, $http, $injector) {

    // Untuk mempermudah penulisan kode program, define variable me sebagai $scope (Alias)
    var me = $scope;

    // Inheritance / implementasi metode pewarisan pada object/class angularjs controller
    $injector.invoke(BaseController, this, { $scope: me });

    // initialisasi / reset
    me.initialize = function () {
        me.data.FPJSignature = me.data.FPJDate = me.data.PickingSlipDate = me.now();
        me.data.FPJNo = "FPJ/XX/YYYYYY";
        $('#FPJStatus').html("");
        $('#btnPrint').attr("disabled", "disabled");
        me.isPrintAvailable = false;
        $('#CustomerNameTagih,#Address1Tagih,#Address2Tagih,#Address3Tagih,#Address4Tagih').attr('readonly', 'readonly');
        me.clearTable(me.grid1);
        me.isLoadData = false;
        me.CurrentPLNo = "";
        me.CurrentFPJNo = "";
        $('#btnGen').attr('disabled', 'disabled');
    }
    me.start();


    me.grid1 = new webix.ui({
        container: "wxtableDtl",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "PartNo", header: "Part No", fillspace: true },
            { id: "PartNoOriginal", header: "Part No Original", fillspace: true },
            { id: "DocNo", header: "Doc No", fillspace: true },
            //{ id: "DocDate", header: "Doc Date", fillspace: true},
            { id: "QtyBill", header: "Qty. Bill", fillspace: true, css: { 'text-align': 'right' } },
        ]
    });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "FPJLookup",
            title: "Faktur Pajak Lookup",
            manager: spManager,
            query: "FPJLookup",
            //defaultSort: "FPJDate desc, FPJNo desc",
            columns: [
            { field: "FPJNo", title: "FPJ No", width: 60 },
            { field: "FPJDate", title: "FPJ Date", template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #", width: 60 },
            { field: "PickingSlipNo", title: "Picking Slip No", width: 60 },
            { field: "PickingSlipDate", title: "Picking Slip Date", template: "#= (PickingSlipDate == undefined) ? '' : moment(PickingSlipDate).format('DD MMM YYYY') #", width: 60 },
            { field: "InvoiceNo", title: "Invoice No", width: 60 },
            { field: "InvoiceDate", title: "Invoice Date", template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #", width: 60 },
            { field: "CustomerName", title: "Customer Name", width: 250 }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                me.isSave = true;
                me.Apply();
                console.log(data);
                var FPJNo = data.FPJNo;
                me.isLoadData = true;
                me.GetSpTrnSFPJDtl(data.FPJNo);
                CheckFPJStatus(FPJNo);
                $('#btnGen').removeAttr('disabled');
            }
        });
    }

    me.GetSpTrnSFPJDtl = function (FPJNo) {
        $http.post('sp.api/Pengeluaran/GetSpTrnSFPJDtl', { "FPJNo": FPJNo }).
        success(function (data, status, headers, config) {
            me.grid.detail = data;
            me.loadTableData(me.grid1, me.grid.detail);
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    // show popup dialog to select existing record
    me.pickinglistno = function () {
        var lookup = Wx.blookup({
            name: "PickingList",
            title: "Picking List Browse",
            manager: spManager,
            query: "PickingList",
            defaultSort: "PickingSlipNo desc ",
            columns: [
            { field: "PickingSlipNo", title: "Picking Slip No" },
            { field: "PickingSlipDate", title: "Picking Slip Date", template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #" },
            { field: "InvoiceNo", title: "Invoice No" },
            { field: "InvoiceDate", title: "Invoice Date", template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #" },
            //{ field: "ProductType", title: "Product Type" },
            //{ field: "CustomerCode", title: "CustomerCode" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                //CheckFPJStatus("");
                var pickingSlipDateA = data.PickingSlipDate.split('T');
                var date1 = new Date(pickingSlipDateA[0]);
                var date2 = new Date(date1.getFullYear(), date1.getMonth(), date1.getDay());
                data.PickingSlipDate = pickingSlipDateA[0];
                data.TOPCode = "D00";
                data.TOPDays = "00";
                data.CustomerCodeTagih = data.CustomerCode;
                data.CustomerNameTagih = data.CustomerName;
                data.Address1Tagih = data.Address1;
                data.Address2Tagih = data.Address2;
                data.Address3Tagih = data.Address3;
                data.Address4Tagih = data.Address4;
                //get picking data deta
                //me.lookupAfterSelect(data);
                me.data = data;
                me.isLoadData = false;
                me.data.FPJDate = me.data.FPJSignature = me.now();
                me.GetTrnSInvoiceDtl(data.InvoiceNo);
                $('#CustomerNameTagih,#Address1Tagih,#Address2Tagih,#Address3Tagih,#Address4Tagih').attr('readonly', 'readonly');
                $('#FPJStatus').html('');
                me.Apply();
                //setTimeout(function () { me.isSave = true; me.hasChanged = true; me.isInitialize = false; console.log("12312d12d");},1000);
            }
        });
    }

    $('div > p').click(function () {
        var name = $(this).data("name");
        if (name == "tabDP") {
            console.log(me.isLoadData);
            if (me.isLoadData) {
                me.GetSpTrnSFPJDtl(me.data.FPJNo);
            }
            else {
                me.GetTrnSInvoiceDtl(me.data.InvoiceNo);
            }
        }
    });

    me.GetTrnSInvoiceDtl = function (InvoiceNo) {
        $http.post('sp.api/Pengeluaran/GetTrnSInvoiceDtl', { "InvoiceNo": InvoiceNo }).
               success(function (data, status, headers, config) {
                   me.grid.detail = data;
                   me.loadTableData(me.grid1, me.grid.detail);
               }).
               error(function (data, status, headers, config) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               });
    }

    me.delete = function () {
        // show confirmation dialog to the user
        // check respond, if true, notify to the server to remove current record
        //MsgConfirm("Are you sure to delete current record?", function (result) {
        //    if (result) {
        //        // call web api by $http provider (async mode)
        //        $http.post('/sp.api/movingcode/delete', me.data)
        //            .success(function (v, status, headers, config) {
        //                if (v.success) {
        //                    Wx.Info("Record has been deleted...");
        //                    me.init();
        //                } else {
        //                    // show an error message
        //                    MsgBox(v.message, MSG_ERROR);
        //                }
        //            }).error(function (e, status, headers, config) {
        //                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        //            });
        //    }
        //})
    }



    // define object untuk inisialisasi data selection
    me.dataOption = [{ value: '<', text: '<' },
                    { value: '<=', text: '<=' },
                    { value: '=', text: '=' },
                    { value: '>', text: '>' },
                    { value: '>=', text: '>=' }];


    // fungsi untuk menset nilai data.sign1 berdasarkan event change dari selection control
    me.updateOption = function (i) {
        switch (i) {
            case 1: me.data.Sign1 = me.combo.sign1 != null ? me.combo.sign1.value : null; break;
            case 2: me.data.Sign2 = me.combo.sign2 != null ? me.combo.sign2.value : null; break;
        }
    }

    me.save = function (e, param) {
        $http.post('sp.api/Pengeluaran/save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.data.FPJNo = data.FPJNo;
                    setTimeout(function () { me.startEditing() }, 3000);
                    CheckFPJStatus(data.FPJNo);
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.allowEdit = function () {
        me.isLoadData = false;
        me.isEditable = false;
        me.hasChanged = true;
        $("#btnCancel").html("<i class='icon icon-undo'></i>Cancel");
        $('#CustomerNameTagih,#Address1Tagih,#Address2Tagih,#Address3Tagih,#Address4Tagih').removeAttr('readonly', 'readonly');
    }

    function CheckFPJStatus(FPJNO) {
        $http.post('sp.api/Pengeluaran/CheckStatus', { WhereValue: FPJNO, Table: "SpTrnSFPJHdr", ColumnName: "FPJNo" })
                    .success(function (v, status, headers, config) {
                        if (v.success) {
                            //console.log(v);
                            $('#FPJStatus').html('<span style="font-size:28px;color:red;font-weight:bold">' + v.statusPrint.toUpperCase() + "</span>");
                            if (parseInt(v.statusCode) >= 0) {
                                $('#btnPrint').removeAttr('disabled');
                                me.isPrintAvailable = true;
                                $('#CustomerNameTagih,#Address1Tagih,#Address2Tagih,#Address3Tagih,#Address4Tagih').removeAttr('readonly', 'readonly');
                                me.isEditable = true;
                                //me.Apply();
                            }
                        } else {
                            // show an error message
                            MsgBox(v.message, MSG_ERROR);
                        }
                        me.startEditing();
                        $('#btnDelete').hide();
                    }).error(function (e, status, headers, config) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    });
    }

    //me.init = function () {

    //    me.isInitialize = true;
    //    me.data = {};
    //    me.grid = {};

    //    if (Wx !== undefined) {
    //        Wx.reset();
    //    }

    //    $("#btnCancel").html("<i class='icon icon-undo'></i>Cancel");

    //    setTimeout(function () {
    //        me.initialize();
    //        $scope.$apply();
    //        setTimeout(function () {
    //            me.hasChanged = false;
    //            me.isLoadData = false;
    //            me.isEditable = false;
    //            me.isSave = false;
    //            me.isInitialize = false;
    //            $scope.$apply();
    //        }, 1250);
    //    }, 50);

    //}

    me.genPinvs = function () {
        var fileName = "PINVS";
        var url = "sp.api/Pengeluaran/GenTextPINVS?";
        var params = "&fpj=" + me.data.FPJNo;
        params += "&flName=" + fileName;
        window.location = url + params;
        console.log(url);
    }

    me.popupPrintChoose = function () {
        BootstrapDialog.show({
            message: $(
                '<div class="container">' +
                '<div class="row">' +
                '<input type="radio" name="printtype" id="fullpage" value="1" checked>&nbsp 1 Halaman ' +
                '<input type="radio" name="printtype" id="halfpage" value="5">&nbsp 1/2 Halaman</div></div>'),
            closable: false,
            draggable: true,
            type: BootstrapDialog.TYPE_INFO,
            title: 'Print Pembuatan Faktur Penjualan',
            buttons: [{
                label: ' Print',
                cssClass: 'btn-primary icon-print',
                action: function (dialogRef) {
                    me.print();
                    dialogRef.close();
                }
            }, {
                label: ' Cancel',
                cssClass: 'btn-warning icon-remove',
                action: function (dialogRef) {
                    dialogRef.close();
                }
            }]
        });
    };




    me.printPreview = function () {        
        me.popupPrintChoose();
    };

    me.print = function () {
        var FPJNO = $('#FPJNo').val();
        $http.post('sp.api/Pengeluaran/UpdateStatusFakturPajak', { NoFakturPajak: FPJNO })
                .success(function (v, status, headers, config) {
                    if (v.success) {

                        var data = $('[name="FPJNo"]').val() + "," + $('[name="FPJNo"]').val() + "," + "profitcenter" + "," + "0" + "," + "typeofgoods";
                        
                        var rparam = ($("input[name='printtype']:checked").val() == "1" ? "SpRpTrn011Long" : "SpRpTrn011Short");
                        

                        Wx.showPdfReport({
                            id: rparam,
                            pparam: data,
                            rparam: rparam,
                            textprint: true,
                            type: "devex"
                        });

                        CheckFPJStatus(FPJNO);

                    } else {
                        // show an error message
                        MsgBox(v.message, MSG_ERROR);
                    }
                }).error(function (e, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
    }

    $('#FPJNo').on('blur', function () {
        if ($('#FPJNo').val() == "" || me.data.FPJNo == "" || me.CurrentFPJNo == $('#FPJNo').val()) return
        $(".ajax-loader").show();
        var data = { FPJNo: $('#FPJNo').val() }

        $http.post('sp.api/pengeluaran/GetDataByNoFPJ', data)
         .success(function (dt, status, headers, config) {
             if (dt.success) {
                 me.lookupAfterSelect(dt.data);
                 me.isSave = true;
                 var FPJNo = dt.data.FPJNo;
                 me.isLoadData = true;
                 me.GetSpTrnSFPJDtl(dt.data.FPJNo);
                 CheckFPJStatus(FPJNo);
                 $('#btnGen').removeAttr('disabled');
                 me.CurrentFPJNo = $('#FPJNo').val();
                 $(".ajax-loader").hide();
             }
             else {
                 $(".ajax-loader").hide();
                 MsgBox(dt.message, MSG_ERROR)
             }
         })
         .error(function (e) {
             $(".ajax-loader").hide();
             MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
         });
    });

    $('#PickingSlipNo').on('blur', function () {
        if ($('#PickingSlipNo').val() == "" || me.data.PickingSlipNo == "" || me.CurrentPLNo == $('#PickingSlipNo').val()) return
           
            $(".ajax-loader").show();
            var data = { PickingListNo: $('#PickingSlipNo').val() }

            $http.post('sp.api/pengeluaran/GetDataByPickingListNo', data)
             .success(function (dt, status, headers, config) {
                 if (dt.success) {
                     var pickingSlipDateA = dt.data.PickingSlipDate.split('T');
                     var date1 = new Date(pickingSlipDateA[0]);
                     var date2 = new Date(date1.getFullYear(), date1.getMonth(), date1.getDay());
                     dt.data.PickingSlipDate = pickingSlipDateA[0];
                     dt.data.TOPCode = "D00";
                     dt.data.TOPDays = "00";
                     dt.data.CustomerCodeTagih = dt.data.CustomerCode;
                     dt.data.CustomerNameTagih = dt.data.CustomerName;
                     dt.data.Address1Tagih = dt.data.Address1;
                     dt.data.Address2Tagih = dt.data.Address2;
                     dt.data.Address3Tagih = dt.data.Address3;
                     dt.data.Address4Tagih = dt.data.Address4;
                     //get picking data
                     me.lookupAfterSelect(dt.data);
                     me.data = dt.data;
                     me.CurrentPLNo = $('#PickingSlipNo').val();

                     setTimeout(function () {
                         me.isLoadData = false;
                         me.data.FPJDate = me.data.FPJSignature = me.now();
                         me.GetTrnSInvoiceDtl(me.data.InvoiceNo);
                         $('#CustomerNameTagih,#Address1Tagih,#Address2Tagih,#Address3Tagih,#Address4Tagih').attr('readonly', 'readonly');
                         $('#FPJStatus').html('');
                         $('#PickingSlipNo').attr('disabled', 'disabled');
                     }, 500)

                     $(".ajax-loader").hide();
                 }
                 else {
                     $(".ajax-loader").hide();
                     MsgBox(dt.message, MSG_ERROR)
                     $('#PickingSlipNo').removeAttr('disabled')
                 }
             })
             .error(function (e) {
                 $(".ajax-loader").hide();
                 MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
             });
    });


}

$(document).ready(function () {

    var options =
        {
            serviceName: "breeze/sparepart",
            title: "Pembuatan Faktur Penjualan",
            xtype: "panels",
            toolbars: WxButtons,
            panels: [
                {
                    name: "pnlFP",
                    title: " ",
                    items: [
                    { name: "FPJStatus", text: "", cls: "span4", readonly: true, type: "label" },
                    //{
                    //    type: "buttons", cls: "span4", items: [
                    //      { name: "btnPrint", text: "Print", disable:true },
                    //    ]
                    //},
                    {
                        type: "buttons", cls: "span4", items: [
                          { name: "btnGen", text: "GeneratePINVS", disable: true, click: "genPinvs()" },
                        ]
                    },
                    ]
                },
                {
                    name: "pnlFP",
                    title: "Pembuatan Faktur Penjualan",
                    items: [
                            { name: "FPJNo", text: "No. Faktur Penjualan", cls: "span3", disable: "IsEditing()", placeHolder: "FPJ/XX/YYYYY" },
                            { name: "FPJDate", text: "Tgl Faktur Penjualan", cls: "span3", readonly: true, type: "ng-datepicker" },
                            { name: "FPJGovNo", text: "No Faktur Pajak", cls: "span3", readonly: true },
                            { name: "FPJSignature", text: "Tgl. Masa Pajak", cls: "span3", type: "ng-datepicker", readonly: true },
                            { name: "InvoiceNo", text: "", type: "hidden", cls: "span3", readonly: true },
                            { name: "InvoiceDate", text: "", cls: "span3", type: "hidden", readonly: true },
                            { name: "PickingSlipNo", text: "Picking List No.", cls: "span3", validasi: "required", disable: "IsEditing()", type: "popup", btnName: "btnPartNo", click: "pickinglistno()" },
                            { name: "PickingSlipDate", text: "Tgl. Picking List", cls: "span3", type: "ng-datepicker", readonly: true },
                            { name: "TOPCode", text: "Kode TOP", cls: "span3", readonly: true },
                            { name: "TOPDays", text: "TOP Hari", cls: "span3", readonly: true },
                            {
                                text: "Pelanggan", type: "controls", type: "controls", items: [
                                { name: "CustomerCode", cls: "span2", placeHolder: "Category Code", readonly: true },
                                { name: "CustomerName", cls: "span4", placeHolder: "Category Name", readonly: true }]
                            }
                    ]
                },
                {
                    name: "pnlDPP",
                    title: "",
                    items: [
                        { name: "TotDPPAmt", text: "Total DPP", cls: "span3", readonly: true },
                        { name: "TotPPNAmt", text: "Total PPN", cls: "span3", readonly: true },
                        { name: "TotFinalSalesAmt", text: "Total Penjualan", cls: "span3", readonly: true },
                        { name: "TransType", text: "TransType", cls: "span3", readonly: true },
                        { name: "TotSalesAmt", text: "TotSalesAmt", cls: "span3", readonly: true },
                        { name: "TotSalesQty", text: "TotSalesQty", cls: "span3", readonly: true },
                        { name: "TransType", text: "TransType", cls: "span3", readonly: true },
                        { name: "CustomerCodeBill", text: "CustomerCodeBill", cls: "span3", readonly: true },
                        { name: "CustomerCodeShip", text: "CustomerCodeShip", cls: "span3", readonly: true }
                    ],
                },
                {
                    xtype: "tabs",
                    name: "tabFP",
                    items: [
                        { name: "tabDP", text: "Details Pemesanan" },
                        { name: "tabPP", text: "Pengiriman dan Penagihan" },
                    ]
                },
                {
                    title: "Alamat Pengiriman",
                    cls: "tabFP tabPP",
                    items: [
                        {
                            text: "Pelanggan", type: "controls", type: "controls", items: [
                            { name: "CustomerCode", cls: "span2", placeHolder: "Customer Code", readonly: true },
                            { name: "CustomerName", cls: "span4", placeHolder: "Customer Name", readonly: true }]
                        },
                        { name: "Address1", text: "Alamat", cls: "span6", readonly: true },
                        { name: "Address2", text: "", cls: "span6", readonly: true },
                        { name: "Address3", text: "", cls: "span6", readonly: true },
                        { name: "Address4", text: "", cls: "span6", readonly: true },
                    ]
                },
                {
                    title: "Alamat Penagihan",
                    cls: "tabFP tabPP",
                    items: [
                        {
                            text: "Pelanggan", type: "controls", type: "controls", items: [
                            { name: "CustomerCodeTagih", cls: "span2", placeHolder: "Customer Code", disable: "IsEditing()", readonly: true },
                            { name: "CustomerNameTagih", cls: "span4", placeHolder: "Customer Name" }]
                        },
                        { name: "Address1Tagih", text: "Alamat", cls: "span6", disable: "me.IsDisable" },
                        { name: "Address2Tagih", text: "", cls: "span6" },
                        { name: "Address3Tagih", text: "", cls: "span6" },
                        { name: "Address4Tagih", text: "", cls: "span6" }
                    ]
                },
                {
                    name: "pnlC",
                    title: "Details Pemesanan",
                    cls: "tabFP tabDP",
                    items: [
                        {
                            name: "wxtableDtl",
                            type: "wxdiv",
                        }
                    ]
                }
            ]
        };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("sppengeluaranfakturpenjualan");
    }

});