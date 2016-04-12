var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spEntryReturnSupplySlipController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });
    
    me.browse = function () {
        var lookup = Wx.blookup({
            name: "SupplySlipBrowse",
            title: "Pencarian No. Supply Slip Retur",
            manager: spManager,
            query: "GetReturnSupplySlipBrowse",
            defaultSort: "ReturnNo desc",
            columns: [
            { field: "ReturnNo", title: "No. Return" },
            { field: "ReturnDate", title: "Tgl. Return" }
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                //me.lookupAfterSelect(result);   
                //me.isSave = false;
                $('#pnlD').show();
                //load details info
                $http.post('sp.api/EntryReturSupplySlip/GetDetailsBrowse', result).
                success(function (data, status, headers, config) {
                    //console.log(data);
                    me.data = data;
                    $('#ReturnDate,#LmpDate').removeAttr('readonly');
                    me.ReturnSSDetails(me.data);
                    me.CheckReturnSS(result.ReturnNo);
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }
    
    me.ReturnSSDetails = function (result) {
        //load details data ReturnSS
        $http.post('sp.api/EntryReturSupplySlip/GetDetailsReturnSS', result).
        success(function (data, status, headers, config) {
            me.grid.detail = data;
            me.loadTableData(me.grid1, me.grid.detail);
            if (data.length > 0) {
                //me.isPrintAvailable = true;
                $('#btnPrintPreview').show();
            }
            else {
                //me.isPrintAvailable = false;
                $('#btnPrintPreview').hide();
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.CheckDetailsData = function (data) {
        if (data.length > 1) {
            //me.isPrintAvailable = true;
            $('#btnPrintPreview').show();
        }
        else {
            //me.isPrintAvailable = false;
            $('#btnPrintPreview').hide();
        }
    }

    me.ClearDtl = function () {
        me.detail.PartNo = undefined;
        me.detail.PartNoOriginal = undefined;
        me.detail.DocNo = undefined;
        me.detail.QtyLmp = undefined;
        me.detail.QtyBill = undefined;
        me.detail.CompanyCode = undefined;
        me.detail.BranchCode = undefined;
        me.detail.WarehouseCode = undefined;
        me.detail.ReturnNo = undefined;
        me.detail.LmpNo = undefined;
        me.detail.LmpDate = undefined;
        me.loadTableData(me.grid1, me.grid.detail);
        $('#btnSaveDtl,#btnClrDtl,#btnDelDtl,#QtyBill').attr('disabled', 'disabled');
    }

    me.GetPartNoLmp = function () {
        me.ClearDtl();
        var lookup = Wx.blookup({
            name: "PartDetailsLkp",
            title: "Pencarian No. Parts",
            manager: spManager,
            query: "GetNoPartReturnSupplySlip?DocNo="+me.data.LmpNo,
            defaultSort: "LmpNo asc",
            columns: [
            { field: "PartNo", title: "Part No." },
            { field: "PartNoOriginal", title: "Part No. Original" },
            { field: "DocNo", title: "Doc No" },
            { field: "QtyLmp", title: "Qty. Lampiran" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.BindDataDetais(result);
                me.Apply();
            }
        });
    }

    me.LmpNoLookup = function () {
        var lookup = Wx.blookup({
            name: "Lampiran Lookup",
            title: "Pencarian No. Lampiran",
            manager: spManager,
            query: "ReturLampiranLookup",
            defaultSort: "LmpNo desc",
            columns: [
            { field: "LmpNo", title: "No. Lampiran" },
            { field: "LmpDate", title: "Tgl. Lampiran" },
            { field: "DocNo", title: "Doc No" },
            { field: "UsageDocNo", title: "Usage Doc No" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.Apply();
                $http.post('sp.api/EntryReturSupplySlip/GetDetailsLookup', result).
                success(function (data, status, headers, config) {
                    me.data = data;
                    //me.data.ReturnDate = me.data.ReferenceDate = me.now();
                    me.getDateServer();
                    me.data.ReturnNo = 'SSR/XX/YYYYY';
                    $('#ReturnDate,#LmpDate').removeAttr('readonly');
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.save = function (e, param) {
        $http.post('sp.api/EntryReturSupplySlip/save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.data.ReturnNo = data.returnNo;
                    $('#pnlD').show();
                    me.ReturnSSDetails(me.data);
                    me.CheckReturnSS(me.data.ReturnNo);
           

                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }


    me.UpdateGridDetail = function(data)
    {
        me.grid.detail = data;     
        me.loadTableData(me.grid1, me.grid.detail); 
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/EntryReturSupplySlip/DeleteRtrSS', me.data).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        Wx.Info("Record has been deleted...");
                        //me.init();      
                        me.CheckReturnSS(me.data.ReturnNo);
                    } else {
                        MsgBox(dl.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.getDateServer = function () {
        $http.post('sp.api/EntryReturSupplySlip/getCurrentDate').
        success(function (dl, status, headers, config) {
            if (dl.success) {
                //console.log(me.data.ReferenceDate)
                if (me.data.LmpDate == undefined)
                {
                    me.data.LmpDate = dl.cDate;
                }
                me.data.ReturnDate = me.data.ReferenceDate = dl.cDate;
            }
        }).
        error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.LinkDetail = function()
    {
        me.detail.Month = me.data.Month;
        me.detail.Year = me.data.Year;
    }

    me.initialize = function()
    {
        me.isPrintAvailable = false;
        $('#btnPosting').attr('disabled', 'disabled');
        me.data.ReturnNo = 'SSR/XX/YYYYY';
        me.getDateServer();
        //me.data.ReturnDate = me.data.LmpDate = me.data.ReferenceDate = me.now();
        $('#EntryRSS').html("");
        me.clearTable(me.grid1);
        $('#pnlD').hide();
        me.detail = {};
    }


    me.grid1 = new webix.ui({
        container: "wxreturdetail",
        view:"wxtable", 
        columns:[
            { id: "PartNo", header: "No. Part", fillspace: true },
            { id: "PartNoOriginal", header: "No. Part Original", fillspace: true },
            { id: "DocNo", header: "No. Dokumen", fillspace: true },
            { id: "QtyLmp", header: "Qty. Lampiran", fillspace: true },
            { id: "QtyBill", header: "Qty. Return", fillspace: true }
        ],
        on:{
            onSelectChange:function(){
                if (me.grid1.getSelectedId() !== undefined) {
                    var details = this.getItem(me.grid1.getSelectedId().id);
                    me.BindDataDetais(details);
                    me.Apply();                    
                }
            }
        }          
    });

    me.BindDataDetais = function (details) {
        console.log("BindDataDetais = " + me.data.statusCode);
        if ((parseInt(me.data.statusCode) < 2 && me.data.statusCode !== undefined) && !(parseInt(me.data.statusCode) > 2 && parseInt(me.data.statusCode) < 2)) {
            me.detail.PartNo = details.PartNo;
            me.detail.PartNoOriginal = details.PartNoOriginal;
            me.detail.DocNo = details.DocNo;
            me.detail.QtyLmp = details.QtyLmp;
            me.detail.QtyBill = details.QtyBill;
            me.detail.CompanyCode = details.CompanyCode;
            me.detail.BranchCode = details.BranchCode;
            me.detail.WarehouseCode = details.WarehouseCode;
            me.detail.ReturnNo = details.ReturnNo;
            me.detail.LmpNo = details.LmpNo;
            me.detail.LmpDate = details.LmpDate;
            me.detail.ReturnNo = me.data.ReturnNo;
            $('#QtyBill').removeAttr('readonly');
            $('#QtyBill,#btnSaveDtl,#btnClrDtl,#btnDelDtl').removeAttr('disabled');
        }
    }

    me.DeleteDtl = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('sp.api/EntryReturSupplySlip/DeleteDetailsSS', me.detail).
                    success(function (dl, status, headers, config) {
                        if (dl.success) {
                            Wx.Info("Record has been deleted...");
                            me.UpdateStatus(me.data);
                            me.ReturnSSDetails(me.data);
                        }
                        me.ClearDtl();
                        me.loadTableData(me.grid1, me.grid.detail);
            }).
            error(function (e, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        });
    }

    me.UpdateStatus = function (data) {
        $http.post('sp.api/EntryReturSupplySlip/UpdateStatus', me.data).
        success(function (dl, status, headers, config) {
            if (dl.success) {
                me.CheckReturnSS(me.data.ReturnNo);
            }
            me.ClearDtl();
            me.loadTableData(me.grid1, me.grid.detail);
        }).
        error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.SaveDtl = function () {
        if (me.detail.QtyBill <= 0) {
            MsgBox("Jumlah part yang akan di retur harus lebih besar dari 0", MSG_ERROR);
        }
        else {
            $http.post('sp.api/EntryReturSupplySlip/SaveDetailsSS', me.detail).
            success(function (dl, status, headers, config) {
                if (dl.success) {
                    me.UpdateStatus(me.data);
                    Wx.Info("Record has been Added...");
                    me.ReturnSSDetails(me.data);
                }
                else {
                    MsgBox(dl.message, MSG_ERROR);
                }
                me.ClearDtl();
                me.loadTableData(me.grid1, me.grid.detail);
            }).
            error(function (e, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        }
    }

    me.CheckReturnSS = function (ReturnNo) {
        $http.post('sp.api/Pengeluaran/CheckStatus', { WhereValue: ReturnNo, Table: "spTrnSRturSSHdr", ColumnName: "ReturnNo" })
        .success(function (v, status, headers, config) {
            if (v.success) {
                //console.log(v);
                $('#EntryRSS').html('<span style="font-size:28px;color:red;font-weight:bold">' + v.statusPrint.toUpperCase() + "</span>");
                if (v.statusCode === "0") {
                    //$('#btnPrint').removeAttr('disabled');
                    me.isPrintAvailable = true;
                    $('#btnPosting').attr('disabled', 'disabled');
                    $('#ReturnDate,#LmpDate').removeAttr('readonly');
                    me.data.statusCode = v.statusCode;
                } else if (v.statusCode === "1") {
                    me.isPrintAvailable = true;
                    $('#btnPosting').removeAttr('disabled');
                    $('#ReturnDate,#LmpDate').attr('readonly', 'readonly');
                    me.data.statusCode = v.statusCode;
                } else if (v.statusCode === "2" || v.statusCode === "3") {
                    me.isPrintAvailable = false;
                    $('#btnPosting').attr('disabled', 'disabled');
                    $('#ReturnDate,#LmpDate').attr('readonly', 'readonly');
                    me.data.statusCode = v.statusCode;
                }

                setTimeout(function () {
                    me.hasChanged = false;
                    me.startEditing();
                    me.isSave = false;
                    $scope.$apply();
                }, 1000);
                me.ClearDtl();
            } else {
                // show an error message
                MsgBox(v.message, MSG_ERROR);
            }
        }).error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.printPreview = function () {
        $http.post('sp.api/EntryReturSupplySlip/Print', me.data)
        .success(function (v, status, headers, config) {
            if (v.success) {

                BootstrapDialog.show({
                    message: $(
                        '<div class="container">' +
                            '<div class="row">' +
                                '<p class="col-xs-2 control-label"><b>Ukuran Kertas</b></p>' +
                                '<input type="radio" name="sizeType" id="sizeType1" value="SpRpTrn013A" style="cursor: pointer;">&nbsp 1/2 Hal &nbsp&nbsp' +
                                '<input type="radio" name="sizeType" id="sizeType2" value="SpRpTrn013" checked style="cursor: pointer;">&nbsp 1 Hal' +
                            '</div>' +
                        '</div>'),
                    closable: false,
                    draggable: true,
                    type: BootstrapDialog.TYPE_INFO,
                    title: 'Print',
                    buttons: [{
                        label: ' Print',
                        cssClass: 'btn-primary icon-print',
                        action: function (dialogRef) {
                            me.printDocument();
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


                me.CheckReturnSS(me.data.ReturnNo);
            } else {
                // show an error message
                MsgBox(v.message, MSG_ERROR);
            }
        }).error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.printDocument = function () {

        var data = me.data.ReturnNo + "," + me.data.ReturnNo + "," + "300" + "," + me.UserInfo.TypeOfGoods;
        var rparam = "admin";
        console.log(data);
        Wx.showPdfReport({
            id: $('input[name=sizeType]:checked').val(),
            pparam: data,
            rparam: rparam,
            textprint: true,
            type: "devex"
        });

    }

    me.PostingRtrSS = function () {
        $http.post('sp.api/EntryReturSupplySlip/PostRtrSS', me.data)
        .success(function (result, status, headers, config) {
            if (result.success) {
                me.data.statusCode = "2";
                me.CheckReturnSS(me.data.ReturnNo);
                $('#ReturnDate, #LmpDate').attr('readonly', 'readonly');
            } else {
                // show an error message
                MsgBox(result.message, MSG_ERROR);
            }
        }).error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    webix.event(window, "resize", function(){ 
        me.grid1.adjust(); 
    })

    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Entry Return Supply Slip",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlPrint",
                title: " ",
                items: [
                { name: "EntryRSS", text: "", cls: "span4", readonly: true, type: "label" },
                {
                    type: "buttons", cls: "span4", items: [
                     //{ name: "btnPrint", text: "Print", disable: true, click: "PrintRtrSS()" }    
                      { name: "btnPosting", text: "Posting", disable: true, click: "PostingRtrSS()" },
                    ]
                },
                ]
            },
            {
                name: "pnlA",
                title: "",
                items: [
                        { name: "ReturnNo", text: "No. SSR", cls: "span3", placeHolder: "No. SSR", readonly: true },
                        { name: "ReturnDate", text: "Tgl. SSR", cls: "span3", placeHolder: "Tgl. SSR", type: "ng-datepicker" },
                        {
                            text: "No. Lampiran",
                            type: "controls",
                            cls:"span3",
                            items: [
                                { name: "LmpNo", placeHolder: "No. Lampiran", type: "popup", btnName: "btnFPJ", readonly: true, click: "LmpNoLookup()" },
                            ]
                        },
                        { name: "LmpDate", text: "Tgl. Lampiran", cls: "span3", placeHolder: "Tgl. Lampiran", type: "ng-datepicker" },
                        { name: "ReferenceNo", text: "No. Referensi", cls: "span3", placeHolder: "No. Referensi" },
                        { name: "ReferenceDate", text: "Tgl. Referensi", cls: "span3", placeHolder: "Tgl. Referensi", type: "ng-datepicker" },
                    ]   
            },
            {
                name: "pnlB",              
                title: "Informasi Kendaraan",
                items: [
                    { name: "PoliceRegNo", text: "No. Polisi", cls: "span3", placeHolder: "No. Polisi", readonly: true },
                    { name: "VIN", text: "VIN", cls: "span3", placeHolder: "VIN", readonly: true },
                    { name: "EngineNo", text: "No. Mesin", cls: "span3", placeHolder: "No. Mesin", readonly: true },
                    { name: "ServiceBookNo", text: "No. Buku Srv", cls: "span3", placeHolder: "No. Buku Srv", readonly: true },
                    { name: "BasicModel", text: "Model Kend", cls: "span3", placeHolder: "Model Kend", readonly: true },
                    { name: "JobOrderNo", text: "No. SPK", cls: "span3", placeHolder: "No. SPK", readonly: true },
                    { name: "ColorCode", text: "Warna", cls: "span3", placeHolder: "Warna", readonly: true },
                    { name: "JobOrderDate", text: "Tgl. SPK", cls: "span3", placeHolder: "Tgl. SPK", readonly: true, type: "ng-datepicker" },
                ]
            },    
            {
                name: "pnlC",
                title: "Informasi Pelanggan",
                items: [
                    { name: "CustomerCode", text: "Pelanggan", cls: "span3", placeHolder: "Pelanggan", readonly: true },
                    { name: "CustomerName", text: "Nama", cls: "span3", placeHolder: "Nama", readonly: true },
                    { name: "Address1", text: "Alamat", cls: "span3", placeHolder: "Alamat", readonly: true },
                    { name: "Address2", text: "", cls: "span3", placeHolder: "Alamat", readonly: true },
                    { name: "CityCode", text: "Kota", cls: "span3", placeHolder: "City Code", readonly: true },
                    { name: "City", text: "", cls: "span3", placeHolder: "City", readonly: true },
                    { name: "HpNo", text: "Handphone", cls: "span3", placeHolder: "Handphone", readonly: true },
                    { name: "FaxNo", text: "Fax", cls: "span3", placeHolder: "Fax", readonly: true },
                ]
            },
            {
                name: "pnlD",
                title: "Details",
                items: [
                    {
                        text: "No. Part",
                        type: "controls",
                        cls: "span3",
                        items: [
                            { name: "PartNo", model: "detail.PartNo", placeHolder: "Part No.", type: "popup", btnName: "btnFPJ", readonly: true, click: "GetPartNoLmp()" },
                        ]
                    },
                    { name: "PartNoOriginal", model: "detail.PartNoOriginal", text: "No. Part Original", cls: "span3", placeHolder: "Part No Original", readonly: true },
                    { name: "DocNo", model: "detail.DocNo", text: "No. Dokumen", cls: "span2", placeHolder: "No. Dokumen", readonly: true },
                    { name: "QtyLmp", model: "detail.QtyLmp", text: "Qty. Lampiran", cls: "span2", placeHolder: "Qty Lampiran", readonly: true },
                    { name: "QtyBill", model: "detail.QtyBill", text: "Qty. Retur", cls: "span2", placeHolder: "Qty Retur", readonly: true },
                    { name: "CompanyCode", model: "detail.CompanyCode", text: "", cls: "span2", placeHolder: "", readonly: true, type: "hidden" },
                    { name: "BranchCode", model: "detail.BranchCode", text: "", cls: "span2", placeHolder: "", readonly: true, type: "hidden" },
                    { name: "WarehouseCode", model: "detail.WarehouseCode", text: "", cls: "span2", placeHolder: "", readonly: true, type: "hidden" },
                    { name: "ReturnNo", model: "detail.ReturnNo", text: "", cls: "span2", placeHolder: "", readonly: true, type: "hidden" },
                    { name: "LmpNo", model: "detail.LmpNo", text: "", cls: "span2", placeHolder: "", readonly: true, type: "hidden" },
                    { name: "LmpDate", model: "detail.LmpDate", text: "", cls: "span2", placeHolder: "", readonly: true, type: "hidden" },
                    {
                        type: "buttons", cls: "span5 full", items: [
                            { name: "btnSaveDtl", text: "Save", icon: "icon-plus", click: "SaveDtl()", cls: "btn btn-primary", disable: "detail.PartNo === undefined" },
                            { name: "btnDelDtl", text: "Delete", icon: "icon-remove", click: "DeleteDtl()", cls: "btn btn-danger", disable: "detail.PartNo === undefined" },
                            { name: "btnClrDtl", text: "Clear", icon: "icon-remove", click: "ClearDtl()", cls: "btn btn-danger", disable: "detail.PartNo === undefined" },
                        ]
                    },
                    {
                        name: "wxreturdetail",
                        type: "wxdiv",
                    },
                ]
            },
            
        ]
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spEntryReturnSupplySlipController"); 
    }

});