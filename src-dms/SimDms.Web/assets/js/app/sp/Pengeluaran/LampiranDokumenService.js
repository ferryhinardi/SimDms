"use strict";
var supHalf = 'SpRpTrn039A';
var supFull = 'SpRpTrn039';
var pickHalf = 'SpRpTrn033';
var pickFull = 'SpRpTrn033Long';
var lamHalf = 'SpRpTrn028A';
var lamFull = 'SpRpTrn028';

function spLampiranDokumenServiceController($scope, $http, $injector) {
    var statusSS = '';

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/LampiranDokumenService/Default')
    .success(function (result) {
        me.currentDate = result.currentDate;
    })
    .error(function (data, status, headers, config) {
        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
    });

    me.browse = function () {
        me.JobOrderLookup();
    }

    me.printModal = function () {
        var optLmp = '<input disabled="disabled" type="radio" name="reportType" id="reportType3" value="LS">&nbsp Lampiran</div>';
        if( statusSS == 3){
            optLmp = '<input type="radio" name="reportType" id="reportType3" value="LS" style="cursor: pointer;">&nbsp Lampiran</div>';
        }

        BootstrapDialog.show({
            message: $(
                '<div class="container">'+
                '<div class="row">'+
                '<p class="col-xs-2 control-label"><b>Jenis Laporan</b></p>' +
                '<input type="radio" name="reportType" id="reportType1" value="SS" checked style="cursor: pointer;">&nbsp Supply Slip &nbsp&nbsp' +
                '<input type="radio" name="reportType" id="reportType2" value="PL" style="cursor: pointer;">&nbsp Picking List &nbsp&nbsp' +
                optLmp
                 +

                '<div class="row">' +
                '<p class="col-xs-2 control-label"><b>Ukuran Kertas</b></p>' +
                '<input type="radio" name="sizeType" id="sizeType1" value="half" checked style="cursor: pointer;">&nbsp 1/2 Hal &nbsp&nbsp' +
                '<input type="radio" name="sizeType" id="sizeType2" value="full" style="cursor: pointer;">&nbsp 1 Hal</div></div>'),
            closable: false,
            draggable: true,
            type: BootstrapDialog.TYPE_INFO,
            title: 'Print',
            buttons: [{
                label: ' Print',
                cssClass: 'btn-primary icon-print',
                action: function (dialogRef) {
                    me.printPreview();
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
    }

    me.printPreview = function () {
        var reportType = $('input[name=reportType]:checked').val();
        var sizeType = $('input[name=sizeType]:checked').val() === 'half';
        var pparam = "";
        var rparam = "";
        
        console.log(reportType);
        console.log(sizeType);
        
        var ReportId = reportType === 'SS' && sizeType ? supHalf :
                       reportType === 'SS' && !sizeType ? supFull :
                       reportType === 'PL' && sizeType ? pickHalf :
                       reportType === 'PL' && !sizeType ? pickFull :
                       reportType === 'LS' && sizeType ? lamFull : lamHalf;

        $http.post('sp.api/LampiranDokumenService/prePrint', { "JobOrderNo": me.data.JobOrderNo, "statSS": statusSS })
       .success(function (e) {
           for (var i = 0; i < e.length; i++) {
               //var par = statusSS === 3 ? e[i].SalesType + ',' + e[i].LmpNo + ',' + e[i].LmpNo + ',' + '300' + ',' + e[i].TypeOfGoods + ',' + '1' :
               //    reportType === 'SS' ? e[i].DocNo + ',' + e[i].DocNo + ',' + '300' + ',' + e[i].SalesType + ',' + e[i].TypeOfGoods :
               //    e[i].PickingSlipNo + ',' + e[i].PickingSlipNo + ',' + '300' + ',' + e[i].TypeOfGoods;

               console.log(ReportId);

               $http.post('sp.api/LampiranDokumenService/reportParam', { transType: e[i].TransType })
                 .success(function (e) {
                     rparam = e;
                   })
                   .error(function (e) {
                       MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                   });

                   switch (ReportId) {
                       case "SpRpTrn039":
                       case "SpRpTrn039A":
                           pparam = e[i].DocNo + ',' + e[i].DocNo + ',' + '300' + ',' + e[i].SalesType + ',' + e[i].TypeOfGoods;
                           break;
                       case "SpRpTrn033":
                       case "SpRpTrn033Long":
                           pparam = e[i].PickingSlipNo + ',' + e[i].PickingSlipNo + ',' + '300' + ',' + e[i].TypeOfGoods;
                           break;
                       case "SpRpTrn028":
                       case "SpRpTrn028A":
                           pparam = +e[i].SalesType + ',' + e[i].LmpNo + ',' + e[i].LmpNo + ',' + '300' + ',' + '%' + ',' + '1';
                           break;
                       default:
                           return;
                           break;

                           console.log(ReportId);
                   }

                   //Wx.showPdfReportNewTab({
                   //    id: ReportId,
                   //    pparam: pparam,
                   //    rparam: rparam,
                   //    textprint:true,
                   //    type: "devex"
                   //});

                   Wx.showPdfReport({
                       id: ReportId,
                       pparam: pparam,
                       rparam: rparam,
                       textprint: true,
                       type: "devex"
                   });
           }
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
       });
    }

    me.loadPickingList = function (JobOrderNo) {
        if (JobOrderNo != "" && JobOrderNo != undefined) {
            $http.post('sp.api/LampiranDokumenService/GetSelectPickingSlip', { "JobOrderNo": JobOrderNo }).
               success(function (data, status, headers, config) {
                   me.PickList = [];
                   $.each(data, function (key, value) {
                       me.PickList.push(value.PickingSlipNo);
                   });
                   me.grid2.detail = data;
                   me.loadTableData(me.grid2, me.grid2.detail);
                   me.grid2.adjust();
               }).
               error(function (data, status, headers, config) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               });
        }
        else {
            me.grid2.detail = {};
            me.clearTable(me.grid2);
        }
    }

    me.loadSupplySlip = function (JobOrderNo, callback) {
        if (JobOrderNo != "" && JobOrderNo != undefined) {
            $http.post('sp.api/LampiranDokumenService/GetPreviewSupplySlip', { "JobOrderNo": JobOrderNo }).
               success(function (data, status, headers, config) {
                   me.grid1.detail = data;
                   me.loadTableData(me.grid1, me.grid1.detail);
                   me.grid1.adjust();
                   
                   if(callback != undefined){
                       callback();
                   }
               }).
               error(function (data, status, headers, config) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               });
        }
        else {
            me.grid1.detail = {};
            me.clearTable(me.grid1);
        }
    }

    me.loadDetail = function (data,process) {
        $http.post('sp.api/LampiranDokumenService/CheckPickingData', { "model":data,"Process" : process}).
           success(function (data, status, headers, config) {
               me.data.TransType = data.TransType;
               me.data.OrderDate = data.OrderDate;
               me.control = data.Controls;

               $('#PickStatus').html('<span style="font-size:28px;color:red;font-weight:bold">' + data.Controls.PickStatus + "</span>");
               statusSS = data.Controls.Status;
           }).
           error(function (data, status, headers, config) {
               MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
           });
    }

    $('div > p').click(function () {
        var name = $(this).data("name");
        //if (!me.IsLmpProcess) {
            if (name === 'tabSS') {
                me.loadSupplySlip(me.data.JobOrderNo);
            }
            else if (name === 'tabPL') {
                if (!me.IsLmpProcess)
                    me.loadPickingList(me.data.JobOrderNo);
                else
                    me.GetSelectPickingSlipAfterLmp();
            }
            else if (name === 'tabLS') {
                me.GetGeneratedLampiranData(me.PickList);
            }
        //}
    });

    $('#JobOrderNo').on('blur', function () {
        if (me.data.JobOrderNo == null || me.data.JobOrderNo == '') return;
        $http.post('sp.api/LampiranDokumenService/JobOrderNo', me.data).success(function (data) {
            if (data.success) {
                me.initialize();
                me.lookupAfterSelect(data.data);
                me.isSave = false;
                me.loadDetail(data.data, "lookup");
                me.loadPickingList(data.data.JobOrderNo);
                me.loadSupplySlip(data.data.JobOrderNo);
                console.log(me.control.Status);
                if (me.control.Status > 1) {
                    me.isPrintAvailable = true;
                    me.isLoadData = true;
                }
                //me.Apply();
            }
            else {
                me.data.JobOrderNo = '';
                me.JobOrderLookup();
            }
        });
    });

    $('#PickedBy').on('blur', function () {
        if (me.PickedBy == null || me.PickedBy == '') {
            me.PickedByName = '';
            return;
        }
        $http.post('sp.api/LampiranDokumenService/EmployeePickedUp', { "PickedBy": me.PickedBy }).success(function (data) {
            if (data.success) {
                me.PickedBy = data.data.EmployeeID;
                me.PickedByName = data.data.EmployeeName;
            }
            else {
                me.PickedBy = me.PickedByName = '';
                me.GetEmpPickedBy();
            }
        });
    });

    $('#JobOrderNo').on('keypress', function (e) {
        if (e.keyCode == 13) {
            if (me.data.JobOrderNo == null || me.data.JobOrderNo == '') return;

            $("#JobOrderDate").focus();
        }
    });

    me.JobOrderLookup = function (callback) {
        var lookup = Wx.blookup({
            name: "SPKNoLookUp",
            title: "No. SPK Lookup",
            manager: spManager,
            query: "SPKNoLookUp",
            defaultSort: "JobOrderNo desc",
            columns: [
            { field: "JobOrderNo", title: "Job Order No" },
            { field: "JobOrderDate", title: "Job Order Date", template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #" },
            { field: "CustomerCode", title: "Customer Code" },
            { field: "CustomerCodeBill", title: "Customer Code Bill" },
            { field: "CustomerName", title: "Customer Name" }
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.PopulateDetail(result)
            }
        });
    }

    me.PopulateDetail = function(result){
        me.initialize();
        me.lookupAfterSelect(result);
        me.isSave = false;
        me.Apply();
        me.loadDetail(result,"lookup");
        me.loadPickingList(result.JobOrderNo);
        me.loadSupplySlip(result.JobOrderNo, me.showPrintButton);
    }

    me.showPrintButton = function() {
        if (statusSS == 2 || statusSS == 3)
        {
            me.isLoadData = true;
            me.isPrintAvailable = true;
        }
    }

    me.initialize = function () {
        me.detail = {};
        me.control = {};
        me.IsDisablePick = false;
        me.IsDisableLmp = false;
        me.control.IsDisableBtnLmp = true;
        me.control.IsDisableBtnPicking = true;
        me.PickedBy = "";
        me.PickedByName = "";
        me.IsLmpProcess = false;
        me.PickList = [];
        me.clearTable(me.grid1);
        me.clearTable(me.grid2);
        $('#PickStatus').html('<span style="font-size:28px;color:red;font-weight:bold"></span>');
        $("p[data-name='tabSS']").addClass('active');
        me.data = {};
        me.data.JobOrderDate = me.data.OrderDate = me.currentDate; //me.now();
        //me.Apply();
        me.grid1.adjust();
        me.grid2.adjust();
        me.grid3.adjust();
    }

    me.grid1 = new webix.ui({
        container: "wxtableSSDtl",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "NoUrut", header: "No", fillspace: true },
            { id: "TipePart", header: "Tipe Part", fillspace: true },
            { id: "PartNo", header: "Part No", fillspace: true },
            { id: "PartName", header: "Nama Part", fillspace: true },
            { id: "DiscPct", header: "Disc", fillspace: true, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "QtyOrder", header: "Order", fillspace: true, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "RetailPrice", header: "Harga Jual", fillspace: true, format: webix.i18n.intFormat, css: { "text-align": "right" } },
            { id: "NetSalesAmt", header: "Harga Bersih", fillspace: true, format: webix.i18n.intFormat, css: { "text-align": "right" } },
        ]
    });

    me.grid2 = new webix.ui({
        container: "wxtablePLDtl",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "No", header: "Part No", fillspace: true },
            { id: "DocNo", header: "No. Supply Slip", fillspace: true },
            { id: "PickingSlipNo", header: "Pick Slip No", fillspace: true },
            { id: "PartNo", header: "Part No", fillspace: true },
            { id: "PartNoOri", header: "Part No Original", fillspace: true },
            { id: "QtySupply", header: "Pick Qty.", fillspace: true, format: webix.i18n.numberFormat, css:{"text-align":"right"}  },
            { id: "QtyPicked", header: "Picked Qty.", fillspace: true, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "QtyBill", header: "Bill Qty.", fillspace: true, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
        ],
        on: {
            onSelectChange: function () {
                if (me.grid2.getSelectedId() !== undefined) {
                    var details = this.getItem(me.grid2.getSelectedId().id);
                    me.detail = details;
                    me.Apply();
                }
            }
        }
    });

    me.grid3 = new webix.ui({
        container: "wxtableLmpDtl",
        view: "wxtable", css:"alternating",
        columns: [
            //{ id: "NoUrut", header: "Part No", fillspace: true },
            { id: "LmpNo", header: "No. Lampiran", fillspace: true },
            { id: "PartNo", header: "No. Part", fillspace: true },
            { id: "PartNoOriginal", header: "No. Part Original", fillspace: true },
            { id: "DocNo", header: "DocNo", fillspace: true },
            { id: "DocDate", header: "DocDate", fillspace: true },
            { id: "ReferenceNo", header: "No. Order", fillspace: true },
            { id: "QtyBill", header: "Qty. Lampiran", fillspace: true, format: webix.i18n.numberFormat, css: { "text-align": "right" } }
        ],
        on: {
            onSelectChange: function () {
                if (me.grid3.getSelectedId() !== undefined) {
                    //var details = this.getItem(me.grid2.getSelectedId().id);
                    //me.detail = details;
                    //me.Apply();
                }
            }
        }
    });

    me.PickProcess = function () {
        if (confirm("Nomor Supply Slip dan Picking akan berlangsung bersamaan.\nLanjutkan proses ?")) {
            $http.post('sp.api/LampiranDokumenService/GenerateSupplySlipPickingEnhance', { "model": me.data, "Process": "pick" }).
            success(function (data, status, headers, config) {
                if (data.success) {
                    me.control = data.LmpDocService.Controls;
                    
                    $('#PickStatus').html('<span style="font-size:28px;color:red;font-weight:bold">' + data.LmpDocService.Controls.PickStatus + "</span>");
                    statusSS = data.LmpDocService.Controls.Status;
                    if (statusSS === 2 || statusSS === 3)
                    {
                        Wx.Success("Picking Slip has been saved...");
                        me.showPrintButton();
                        me.loadDetail(me.data, "pick");
                        me.loadPickingList(me.data.JobOrderNo);
                        $(".panel.tabsDetails").hide();
                        $(".panel.tabsDetails.tabPL").show();
                        $("p[data-name='tabSS']").removeClass('active');
                        $("p[data-name='tabLS']").removeClass('active');
                        $("p[data-name='tabPL']").addClass('active');
                    }
                }
                else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        }
    }

    me.LmpProcess = function () {
        //var date = new Date();
        if (me.PickedBy === "") {
            MsgBox("Informasi Picked by kurang lengkap.");
        }
        else {
            if (confirm("Nomor Lampiran dan Generate Bill akan berlangsung bersamaan.\nLanjutkan proses ?")) {
                $http.post('sp.api/LampiranDokumenService/ProcessLampiran', { "JobOrderNo": me.data.JobOrderNo, "CustomerCode": me.data.CustomerCode, "PickedBy": me.PickedBy, "PickList": me.PickList, "OrderDate": me.currentDate }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Lampiran has been saved...");
                        me.control = data.Controls;
                        $('#PickStatus').html('<span style="font-size:28px;color:red;font-weight:bold">' + data.Controls.PickStatus + "</span>");

                        statusSS = data.Controls.Status;
                        //me.Apply();
                        setTimeout(function () { me.GetGeneratedLampiranData(me.PickList); }, 200);
                        me.IsLmpProcess = true;
                        me.GetSelectPickingSlipAfterLmp(me.data.JobOrderNo);
                        
                        me.isPrintAvailable = true;
                        me.isLoadData = true;
                    }
                    else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        }
    }

    me.SaveDtl = function () {
        var qtyPicked = $scope.detail.QtyPicked == '' ? 0 : parseFloat($scope.detail.QtyPicked);
        var qtySupply = parseFloat($scope.detail.QtySupply)

        if (qtyPicked > qtySupply) {
            MsgBox("Quantity picked tidak boleh lebih besar dari quantity pick.", MSG_WARNING);
            return;
        }

        if (qtyPicked > qtySupply) {
            MsgBox("Quantity picked tidak boleh lebih besar dari quantity pick.", MSG_WARNING);
            return;
        }

        if (qtyPicked < 0 ) {
            MsgBox("Nilai tidak boleh < 0 !", MSG_WARNING);
            return;
        }

        if (confirm("Apakah anda yakin ?")) {
            me.detail.QtyPicked = qtyPicked;

            $http.post('sp.api/LampiranDokumenService/SavePickingList', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    me.loadPickingList(me.data.JobOrderNo);
                    me.ClearDtl();
                }
                else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        }
    }

    me.GetEmpPickedBy = function () {
        var lookup = Wx.blookup({
            name: "EmployeePickedByLookup",
            title: "Pencarian Karyawan",
            manager: spManager,
            query: "EmployeePickedByLookup",
            defaultSort: "EmployeeID asc",
            columns: [
            { field: "EmployeeID", title: "ID Karyawan" },
            { field: "EmployeeName", title: "Nama Karyawan" }
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.PickedBy = result.EmployeeID;
                me.PickedByName = result.EmployeeName;
                me.Apply();
            }
        });
    }

    me.GetGeneratedLampiranData = function (PickList) {
        if (PickList != "" && PickList != undefined) {
            $http.post('sp.api/LampiranDokumenService/GetGeneratedLampiran', { "PickList": PickList }).
                success(function (data, status, headers, config) {
                    me.grid3.detail = data;
                    me.loadTableData(me.grid3, me.grid3.detail);
                    me.grid3.adjust();

                }).error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
        }
        else {
            me.grid3.detail = {};
            me.clearTable(me.grid3);
        }
    }

    me.GetSelectPickingSlipAfterLmp = function () {
        $http.post('sp.api/LampiranDokumenService/GetSelectPickingSlipAfterLmp', { "JobOrderNo": me.data.JobOrderNo }).
            success(function (data) {
                me.grid2.detail = data;
                me.loadTableData(me.grid2, me.grid2.detail);
            });
    }
    

    me.ClearDtl = function () {
        me.detail = {};
        me.loadTableData(me.grid2, me.grid2.detail);
    }

    $http.post('sp.api/Combo/GetLookupByCodeID', { CodeID: "TTSR" }).
    success(function (data, status, headers, config) {
        me.comboTransType = data;
    });



    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Lampiran Dokumen Service",
        xtype: "panels",
        toolbars: [
                //{ name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", show: "isPrintAvailable && isLoadData", click: "printModal()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlStatus",
                title: " ",
                items: [
                { name: "PickStatus", text: "", cls: "span4", readonly: true, type: "label" },
                {
                    type: "buttons", cls: "span4", items: [
                      { name: "btnPicking", text: "Proses Picking", click: "PickProcess()", disable: "control.IsDisableBtnPicking" },
                      { name: "btnLmp", text: "Proses Lampiran", click: "LmpProcess()", disable: "control.IsDisableBtnLmp" },
                    ]
                },
                ]
            },
            {
                name: "pnlA",
                title: "Pelanggan",
                items: [
                        //{ name: "chkCetak", model: "data.IsCetak", cls: "span2", text: "SPK yg sudah dibuat lampiran", type: "check" },
                        {
                            text: "No. SPK", type: "controls",  items: [
                            { name: "JobOrderNo", cls: "span2", placeHolder: "No. SPK", type: "popup", btnName: "btnJobOrder", click: "JobOrderLookup()", validasi: "required" },
                            { name: "JobOrderDate", model: "data.JobOrderDate", cls: "span2", text: "Tgl. SPK", type: "ng-datepicker", disable: "control.IsDisableJobOrderDate", format: "dd-MMM-yyyy" },
                            ]
                        }
                ]
            },
            {
                xtype: "tabs",
                name: "tabsDetails",
                items: [
                    { name: "tabSS", text: "Supply Slip" },
                    { name: "tabPL", text: "Picking List" },
                    { name: "tabLS", text: "Lampiran Service" },
                ]
            },
            {
                title: "Supply Slip",
                cls: "tabsDetails tabSS",
                items: [
                    { name: "TransType", text: "Tipe Transaksi", cls: "span3", placeHolder: "0", validasi: "required", type: "select2", datasource: "comboTransType", disable: "control.IsDisableTransType" },
                    {
                        text: "Pelanggan", type: "controls",  items: [
                              { name: "CustomerCode", cls: "span2", placeHolder: "Customer Code", readonly: true },
                              { name: "CustomerName", text: "Customer Name", cls: "span2", placeHolder: "Customer Name", readonly: true },
                        ]
                    },
                    { name: "JobOrderNo", text: "No. Order", cls: "span3", readonly: true },
                    { name: "OrderDate", text: "Tgl. Order", cls: "span3", type: "ng-datepicker", disable: "control.IsDisableOrderDate" },
                    {
                        name: "wxtableSSDtl",
                        type: "wxdiv",
                    }
                ]
            },
            {
                title: "Picking List",
                cls: "tabsDetails tabPL",
                items: [{
                    text: "Karyawan", type: "controls",items: [
                    { name: "PickedBy", model: "PickedBy", cls: "span2", placeHolder: "Picked By.", type: "popup", btnName: "btnPickedBy", click: "GetEmpPickedBy()", disable: "control.IsDisableBtnPickedBy" },
                    { name: "PickedByName", model: "PickedByName", cls: "span4", placeHolder: "Picked By Name", readonly: true }]
                },
                {
                    text: "Part Details", type: "controls",  items: [
                        { name: "DocNo", model: "detail.DocNo", cls: "span1", placeHolder: "Supply No", readonly: true },
                        { name: "PickingSlipNo", model: "detail.PickingSlipNo", cls: "span1", placeHolder: "Pick Slip No", readonly: true },
                        { name: "PartNo", model: "detail.PartNo", cls: "span1", placeHolder: "Part No", readonly: true },
                        { name: "PartNoOri", model: "detail.PartNoOri", cls: "span1", placeHolder: "Part No Original", readonly: true },
                        { name: "QtySupply", model: "detail.QtySupply", cls: "span1", placeHolder: "Pick Qty", readonly: true },
                        { name: "QtyPicked", model: "detail.QtyPicked", cls: "span1 number", placeHolder: "Picked Qty", disable: "control.IsDisableBtnPickedBy" },
                        { name: "QtyBill", model: "detail.QtyBill", cls: "span1", placeHolder: "Bill Qty", readonly: true },
                    ]
                },
                {
                    type: "buttons", cls: "span5 full", items: [
                               { name: "btnSaveDtl", text: "Save", icon: "icon-plus", click: "SaveDtl()", cls: "btn btn-primary", disable: "detail.PickingSlipNo === undefined" },
                               { name: "btnClrDtl", text: "Clear", icon: "icon-remove", click: "ClearDtl()", cls: "btn btn-danger", disable: "detail.PickingSlipNo === undefined" },
                    ]
                },
                {
                    name: "wxtablePLDtl",
                    type: "wxdiv",
                }
                ]
            },
            {
                title: "Lampiran Service",
                cls: "tabsDetails tabLS",
                items: [
                   {
                       text: "Pelanggan", type: "controls",  items: [
                              { name: "CustomerCode", cls: "span2", placeHolder: "Customer Code", readonly: true },
                              { name: "CustomerName", text: "Customer Name", cls: "span2", placeHolder: "Customer Name", readonly: true },
                       ]
                   },
                   { name: "TransType", cls: "span4", text: "Tipe Transaksi", placeHolder: "Tipe Transaksi", readonly: true },
                   {
                       name: "wxtableLmpDtl",
                       type: "wxdiv",
                   }
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spLampiranDokumenServiceController");
    }

});