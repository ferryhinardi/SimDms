var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function omHPPController($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.loadDetail = function (data, id) {
        if (id == 1) {
            $http.post('gn.api/HPP/BPUDetailLoad?HPPNo=' + data.HPPNo).
            success(function (data, status, headers, config) {
                me.grid.detail = data.data;
                me.loadTableData(me.grid1, me.grid.detail);
                if (data.bUpload) {
                    //me.options = '1';
                    //me.HideDetailButtons();
                }
            }).
            error(function (e, status, headers, config) {
                //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                console.log(e);
            });
        } else if (id == 2) {
            $http.post('gn.api/HPP/SalesModelLoad?HPPNo=' + data.HPPNo + '&BPUNo=' + data.BPUNo).
               success(function (data, status, headers, config) {
                   me.grid.detail = data;
                   me.loadTableData(me.grid2, me.grid.detail);
               }).
               error(function (e, status, headers, config) {
                   //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   console.log(e);
               });
        } else if (id == 3) {
            $http.post('gn.api/HPP/SalesModelOthersLoad' , data).
               success(function (data, status, headers, config) {
                   me.grid.detail = data;
                   me.loadTableData(me.grid3, me.grid.detail);
               }).
               error(function (e, status, headers, config) {
                   //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   console.log(e);
               });
        } else if (id==4){
            $http.post('gn.api/HPP/SubSalesModelLoad' , data).
               success(function (data, status, headers, config) {
                   me.grid.detail = data;
                   me.loadTableData(me.grid4, me.grid.detail);
               }).
               error(function (e, status, headers, config) {
                   //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   console.log(e);
               });
        }
    }

    me.browse = function () {
        //me.init();
        var lookup = Wx.blookup({
            name: "HPPLookup",
            title: "HPP",
            manager: spSalesManager,
            query: "HPPLookup",
            defaultSort: "HPPNo DESC",
            columns: [
                { field: "HPPNo", title: "HPP No.", width: 125 },
                { field: "PONo", title: "PO No.", width: 125 },
                { field: "SupplierName", title: "Supplier Name", width: 375 },
                { field: 'RefferenceInvoiceNo', title: 'Reff. Invoice', width: 125 },
                { field: 'RefferenceFakturPajakNo', title: 'Reff. FakturPajak', width: 150 },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
    
                if (data.Status == '1'|| data.Status == '2') {
                    //$('#btnApprove').prop('disabled', false);
                    //var x = localStorage.getItem('MyData');
                    var x = 'HPPDate,RefferenceInvoiceNo,isC1,RefferenceInvoiceDate,RefferenceFakturPajakNo,isC2,RefferenceFakturPajakDate,isC3,DueDate,PONo,SupplierName,RefferenceNo,BillTo,Remark';
                    var y = x.split(',', 15);
                    var z = y.length;
                    for (i = 0; i <= z; i++) {
                        $('#' + y[i]).attr('disabled', true);
                    }
                }

                //me.lookupAfterSelect(data);
                me.checkbox(data);
                $('#btnAddBPU').prop('disabled', false);
                me.switchBPU = '4';
                me.data = data;
                me.Apply();

                me.loadDetail(data, 1);
                $('#RefferenceFakturPajakDate').prop('readonly', true);
                $("[name='RefferenceFakturPajakDate']").prop('disabled', true);
                me.checkStatus(data.Status);
            }
        });
    }

    me.checkbox = function (data) {
        if ((data.RefferenceFakturPajakDate).substring(0, 4) != "1900") {
            $('#isC2').prop('checked', true);
            $('#RefferenceFakturPajakDate').prop('readonly', false);
            //alert((data.RefferenceFakturPajakDate).substring(0, 4));
        } else {
            $('#isC2').prop('checked', false);
            $('#RefferenceFakturPajakDate').prop('readonly', true);
            me.data.RefferenceFakturPajakDate = undefined;
        }

        if (data.DueDate != '' && (data.DueDate).substring(0, 4) != "1900") {
            $('#isC3').prop('checked', true);
            $('#DueDate').prop('readonly', false);
            //alert((data.RefferenceFakturPajakDate).substring(0, 4));
        } else {
            $('#isC3').prop('checked', false);
            $('#DueDate').prop('readonly', true);
            me.data.DueDate = undefined;
        }
    }

    me.checkStatus = function (Status) {
        me.data.Status = Status;
        switch ((Status)) {
            case '9':
                me.data.StatusDesc = "FINISHED";
                $('#statusLbl').text("FINISHED");
                $('#btnApprove').prop('disabled', 'disabled');
                $("#btnAddBPU,#btnUpdateBPU,#btnDeleteBPU,#btnCancelBPU").hide();
                $("#btnAddPC, #btnDltPC, #btnClrPC, #btnSave").hide();
                break;
            case '0':
                me.data.StatusDesc = "OPEN";
                $('#statusLbl').text("OPEN");
                $("#btnPrintPreview, #btnDelete").show();
                $("#btnAddPC, #btnDltPC, #btnClrPC, #btnSave, #btnDelete, #btnClose").show();

                //if (me.detail.BPUNo !== "") {
                //    $("#btnUpdateBPU").show();
                //}
                //else {
                //    $("#btnUpdateBPU").hide();
                //}
                $("#btnAddBPU,#btnUpdateBPU,#btnDeleteBPU,#btnCancelBPU").show();

                me.isLoadData = true;
                me.isPrintAvailable = true;
                me.isEQAvailable = false;
                me.isInitialize = false;
                me.Apply();
                $('#btnDelete, #btnPrintPreview').show();
                break;
            case '1':
                me.data.StatusDesc = "PRINTED";
                $('#statusLbl').text("PRINTED");
                $('#btnApprove').removeAttr('disabled');
                me.isLoadData = true;
                me.isPrintAvailable = true;
                me.isInitialize = false;
                me.Apply();
                $("#btnAddBPU").show();
                console.log('Status CHEK');
                console.log(me.detail.oid);
                $("#btnPrintPreview").show();
                break;
            case '2':
                me.data.StatusDesc = "APPROVED";
                $('#statusLbl').text("APPROVED");
                $('#btnApprove').prop('disabled', 'disabled');
                $("#btnAddBPU,#btnUpdateBPU,#btnDeleteBPU,#btnCancelBPU").hide();
                $("#btnAddPC, #btnDltPC, #btnClrPC, #btnSave").hide();
                me.isLoadData = true;
                me.isPrintAvailable = true;
                me.isInitialize = false;
                me.Apply();
                $("#btnPrintPreview").show();
                break;
            case '3':
                me.data.StatusDesc = "CANCELED";
                $('#statusLbl').text("CANCELED");
                $('#btnApprove').prop('disabled', 'disabled');
                $("#btnAddBPU,#btnUpdateBPU,#btnDeleteBPU,#btnCancelBPU").hide();
                me.isLoadData = true;
                me.isInitialize = false;
                me.Apply()
                $("#btnAddPC, #btnDltPC, #btnClrPC, #btnSave, #btnDelete, #btnPrintPreview, #btnClose").hide();
                break;
            case '5':
                me.data.StatusDesc = "POSTED";
                $('#statusLbl').text("POSTED");
                $('#btnApprove').prop('disabled', 'disabled');
                $("#btnAddBPU,#btnUpdateBPU,#btnDeleteBPU,#btnCancelBPU").hide();
                $("#btnAddPC, #btnDltPC, #btnClrPC, #btnSave").hide();
                me.isLoadData = true;
                me.isPrintAvailable = true;
                me.isInitialize = false;
                me.Apply();
                $("#btnPrintPreview").show();
                break;
            case 'new':
                me.data.StatusDesc = "NEW";
                $('#statusLbl').text("NEW");
                $("#btnAddPC, #btnDltPC, #btnClrPC, #btnSave, #btnClose").show();

                //if (me.detail.BPUNo !== undefined && me.detail.BPUNo !== "") {
                //    $("#btnUpdateBPU").show();
                //}
                //else {
                //    $("#btnUpdateBPU").hide();
                //}
                $("#btnAddBPU,#btnUpdateBPU,#btnDeleteBPU,#btnCancelBPU").show();

                $("#btnDelete,#btnPrintPreview").hide();
                break;
        }
    }

    me.PO = function () {
        var lookup = Wx.blookup({
            name: "PONoLookup",
            title: "PO No. Looukup",
            manager: spSalesManager,
            query: "POLookup",
            defaultSort: "PONo desc",
            columns: [
                { field: "PONo", title: "PO No." },
                { field: "SupplierCode", title: "Supplier Code" },
                { field: "SupplierName", title: "Supplier Name" },
                { field: "RefferenceNo", title: "Reff. No." },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                //me.lookupAfterSelect(data);
                me.data.PONo = data.PONo;
                me.data.RefferenceNo = data.RefferenceNo;
                me.data.SupplierCode = data.SupplierCode;
                me.data.SupplierName = data.SupplierName; 
                me.data.BillTo = data.BillTo;
                me.Apply();
            }
        });
    }

    me.Reff = function () {
        var lookup = Wx.blookup({
            name: "PONoLookup",
            title: "PO No. Looukup",
            manager: spSalesManager,
            query: "POLookup",
            defaultSort: "PONo desc",
            columns: [
                { field: "RefferenceNo", title: "Reff. No." },
                { field: "SupplierCode", title: "Supplier Code" },
                { field: "SupplierName", title: "Supplier Name" },
                { field: "PONo", title: "PO No." },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
            }
        });
    }

    me.ReffInv = function () {
        var lookup = Wx.blookup({
            name: "SupplierBrowse",
            title: "Pemasok",
            manager: spSalesManager,
            query: "ReffInvLookup",
            defaultSort: "BatchNo asc",
            columns: [
                { field: "BatchNo", title: "Batch No." },
                { field: "InvoiceNo", title: "Invoice No." },
                { field: "InvoiceDate", title: "Invoice Date", template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #" },
                { field: "FakturPajakNo", title: "Faktur Pajak No." },
                { field: "FakturPajakDate", title: "Faktur Pajak Date", template: "#= (FakturPajakDate == undefined) ? '' : moment(FakturPajakDate).format('DD MMM YYYY') #" },
                { field: "PONo", title: "PO No." },
                { field: "RefferenceNo", title: "Reff No" },
                { field: "SupplierCode", title: "Supplier Code" },
                { field: "SupplierName", title: "Supplier Name" },
                { field: "BillTo", title: "Bill To" },
                { field: "DueDate", title: "Due Date", template: "#= (DueDate == undefined) ? '' : moment(DueDate).format('DD MMM YYYY') #" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                //me.lookupAfterSelect(data);
                data.LockingDate = data.FakturPajakDate;
                me.data = data;
                me.data.RefferenceInvoiceDate = data.InvoiceDate;
                me.data.RefferenceInvoiceNo = data.InvoiceNo;
                me.data.RefferenceFakturPajakDate = data.FakturPajakDate ;
                me.data.RefferenceFakturPajakNo = data.FakturPajakNo;
                me.data.HPPDate = me.now();
                me.Apply();
                me.checkbox(me.data);
                me.checkStatus("new");
                console.log(me.options);

            }
        });
    }

    me.BPU = function () {
        if (me.data.PONo == '' || !me.data.PONo) {
            SimDms.Warning("Please fill PO No in Header!");
        } else {
            var lookup = Wx.blookup({
                name: "BPULookup",
                title: "BPU Lookup",
                manager: spSalesManager,
                query: new breeze.EntityQuery().from("BPUDetailLookup").withParameters({ PONo: me.data.PONo }),//"BPUDetailLookup",
                defaultSort: "BPUNo desc",
                columns: [
                    { field: "BPUNo", title: "BPU No." },
                    { field: "PONo", title: "PO No" },
                    { field: "RefferenceDONo", title: "Reff. DO No." },
                    { field: "RefferenceSJNo", title: "Reff. SJ No." },
                    
                ]
            });
            lookup.dblClick(function (data) {
                if (data != null) {
                    me.detail.PONo = data.PONo;
                    me.detail.RefferenceDONo = data.RefferenceDONo;
                    me.detail.RefferenceSJNo = data.RefferenceSJNo;
                    me.detail.BPUNo = data.BPUNo;
                    me.Apply();
                }
            });
        }
    }

    me.ReffDO = function () {
        if (me.data.PONo == '' || !me.data.PONo) {
            SimDms.Warning("Please fill PO No in Header!");
        } else {
            var lookup = Wx.blookup({
                name: "ReffDOLookup",
                title: "Reff DO Lookup",
                manager: spSalesManager,
                query: new breeze.EntityQuery().from("BPUDetailLookup").withParameters({ PONo: me.data.PONo }),//"BPUDetailLookup",
                defaultSort: "BPUNo desc",
                columns: [
                    { field: "RefferenceDONo", title: "Reff. DO No." },
                    { field: "PONo", title: "Reff. PO No" },
                    { field: "RefferenceSJNo", title: "Reff. SJ No." },
                    { field: "BPUNo", title: "BPU No." },
                ]
            });
            lookup.dblClick(function (data) {
                if (data != null) {
                    me.detail.PONo = data.PONo;
                    me.detail.RefferenceDONo = data.RefferenceDONo;
                    me.detail.RefferenceSJNo = data.RefferenceSJNo;
                    me.detail.BPUNo = data.BPUNo;
                    me.Apply();
                }
            });
        }
    }

    me.ReffSJ = function () {
        if (me.data.PONo == '' || !me.data.PONo) {
            SimDms.Warning("Please fill PO No in Header!");
        } else {
            var lookup = Wx.blookup({
                name: "ReffSJLookup",
                title: "Reff SJ Lookup",
                manager: spSalesManager,
                query: new breeze.EntityQuery().from("BPUDetailLookup").withParameters({ PONo: me.data.PONo }),//"BPUDetailLookup",
                defaultSort: "BPUNo desc",
                columns: [
                    { field: "RefferenceSJNo", title: "Reff SJ No." },
                    { field: "PONo", title: "PO No" },
                    { field: "RefferenceDONo", title: "Reff DO No." },
                    { field: "BPUNo", title: "BPU No." },
                    
                    
                ]
            });
            lookup.dblClick(function (data) {
                if (data != null) {

                    me.detail.PONo = data.PONo;
                    me.detail.RefferenceDONo = data.RefferenceDONo;
                    me.detail.RefferenceSJNo = data.RefferenceSJNo;
                    me.detail.BPUNo = data.BPUNo;
                    me.Apply();
                }
            });
        }
    }

    me.salesModelCode = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeLookup",
            title: "Model",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("SalesModelCode4HPP").withParameters({ PONo: me.data.PONo, BPUNo : me.detail.BPUNo, salesModelCode : '' }),
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code" },
                //{ field: "SalesModelDesc", title: "Sales Model Des" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail2.SalesModelCode = data.SalesModelCode;
                me.detail2.EngineNo = data.EngineNo;
                me.Apply();
                //me.loadDetail(data);
                $('#SalesModelCode').attr('disabled', 'disabled');
            }
        });
    }

    me.salesModelYear = function () {
        var lookup = Wx.blookup({
            name: "SalesModelYearLookup",
            title: "Model year",
            manager: spSalesManager,
            //query: "SalesModelYearLookup",
            query: new breeze.EntityQuery().from("SalesModelCode4HPP").withParameters({ PONo: me.data.PONo, BPUNo : me.detail.BPUNo, SalesModelCode :  me.detail2.SalesModelCode, }), 
            defaultSort: "SalesModelYear asc",
            columns: [
                { field: "SalesModelYear", title: "Sales Model Year" }
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail2.PONo = me.data.PONo;
                me.detail2.SalesModelYear = data.SalesModelYear;
                me.detail2.SupplierCode = me.data.SupplierCode;
                
                me.Apply();
                $http.post('om.api/HPP/ValidateSalesModelYear?PONo=' + me.data.PONo + '&BPUNo=' + me.data.BPUNo + '&HPPNo=' + me.data.HPPNo + '&salesModelCode=' + me.detail2.SalesModelCode + '&salesModelYear=' + me.detail2.SalesModelYear).
                    success(function (data) {
                        if (!data.success) {
                            MsgConfirm(data.message, function (result) {
                                if (result) {
                                    $http.post('om.api/HPP/PriceListBeli?PONo=' + me.data.PONo + '&salesModelCode=' + me.detail2.SalesModelCode + '&salesModelYear=' + data.data).
                                        success(function (data, status, headers, config) {
                                            if (data.success) {
                                                me.detail2.PPnBMPaid = data.data.PPnBMPaid;
                                                me.detail2.BeforeDiscDPP = data.data.BeforeDiscDPP;
                                                me.detail2.AfterDiscDPP = data.data.AfterDiscDPP;
                                                me.detail2.AfterDiscPPn = data.data.AfterDiscPPn;
                                                me.detail2.AfterDiscPPnBM = data.data.AfterDiscPPnBM;
                                                me.detail2.AfterDiscTotal = data.data.AfterDiscTotal;
                                                me.detail2.Remark = data.data.Remark;
                                                me.detail2.Quantity = data.data.QuantityBPU - data.data.QuantityBPU;
                                                me.detail2.DiscExcludePPn = data.data.DiscExcludePPn;
                                                me.ReformatNumber();

                                            }
                                        }).
                                        error(function (data, status, headers, config) {
                                            alert('error');
                                        });
                                }
                                else $('#SalesModelYear').val('');
                            });
                        }
                        else {
                            $http.post('om.api/HPP/PriceListBeli?PONo=' + me.data.PONo + '&salesModelCode=' + me.detail2.SalesModelCode + '&salesModelYear=' + me.detail2.SalesModelYear + '&BPUNo=' + me.detail.BPUNo).
                                        success(function (data, status, headers, config) {
                                            if (data.success) {
                                                me.detail2.PPnBMPaid = data.data.PPnBMPaid;
                                                me.detail2.BeforeDiscDPP = data.data.BeforeDiscDPP;
                                                me.detail2.AfterDiscDPP = data.data.AfterDiscDPP;
                                                me.detail2.AfterDiscPPn = data.data.AfterDiscPPn;
                                                me.detail2.AfterDiscPPnBM = data.data.AfterDiscPPnBM;
                                                me.detail2.AfterDiscTotal = data.data.AfterDiscTotal;
                                                me.detail2.Remark = data.data.Remark;
                                                me.detail2.Quantity = data.data.QuantityBPU;
                                                me.detail2.DiscExcludePPn = data.data.DiscExcludePPn;
                                                me.ReformatNumber();

                                            }
                                        }).
                                        error(function (data, status, headers, config) {
                                            alert('error');
                                        });
                        }
                    });
            }
        });
    }

    me.ChassisCode = function () {
        var lookup = Wx.blookup({
            name: "ChassisCodeLookup",
            title: "Chassis Code",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("ChassisCodeDtlHPP").withParameters({ PONo: me.data.PONo, BPUNo: me.detail.BPUNo, SalesModelCode: me.detail2.SalesModelCode, SalesModelYear: me.detail2.SalesModelYear }),
            defaultSort: "ChassisCode asc",
            columns: [
                { field: "ChassisCode", title: "Kode Rangka" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail4.ChassisCode = data.ChassisCode;
                me.Apply();
            }
        });
    }

    me.ChassisNo = function () {
        var lookup = Wx.blookup({
            name: "ChassisCodeLookup",
            title: "Chassis No",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("ChassisNoDtlHPP").withParameters({ PONo: me.data.PONo, BPUNo: me.detail.BPUNo, SalesModelCode: me.detail2.SalesModelCode, SalesModelYear: me.detail2.SalesModelYear, ChassisCode: me.detail4.ChassisCode }),
            defaultSort: "ChassisCode asc",
            columns: [
                { field: "ChassisNo", title: "No Rangka" },
                { field: "EngineCode", title: "Kode Mesin" },
                { field: "EngineNo", title: "No Mesin" },
                { field: "ColourCode", title: "Kode Warna" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail4.ChassisNo = data.ChassisNo;
                me.detail4.EngineNo = data.EngineNo;
                me.Apply();
            }
        });
    }

    me.$watch('switchPO', function (a, b) {
        //alert(b + a);
        if (a == 2) {
            $('#PONo').removeAttr('disabled');
            $('#btnPONo').removeAttr('disabled');
            $('#RefferenceNo').attr('disabled', true);
            $('#btnRefferenceNo').attr('disabled', true);
            $('label[btn-radio="\'2\'"]').text('On');
            $('label[btn-radio="\'3\'"]').text('Off');

        } else {
            $('#PONo').attr('disabled', true);
            $('#btnPONo').attr('disabled', true);
            $('#RefferenceNo').removeAttr('disabled');
            $('#btnRefferenceNo').removeAttr('disabled');
            $('label[btn-radio="\'2\'"]').text('Off');
            $('label[btn-radio="\'3\'"]').text('On');
        }
    }, true);

    me.$watch('options', function () {
        me.init();
    });

    me.$watch('options', function (a, b) {
        //alert(b + a);
        if (a == 0) {
            $('#btnRefferenceInvoiceNo').attr('disabled', true);
            me.data.isNew == false;
        } else {
            $('#btnRefferenceInvoiceNo').removeAttr('disabled');
            me.data.isNew == true;
        }

        console.log(me.options);

    }, true);

    me.$watch('switchBPU', function (a, b) {
        
        switch (a) {
            case '4':
                $('label[btn-radio="\'6\'"]').text('Off');
                $('label[btn-radio="\'5\'"]').text('Off');
                $('label[btn-radio="\'4\'"]').text('On');
                //alert(a);
                $('#RefferenceDONo').attr('disabled', true);
                $('#btnRefferenceDONo').attr('disabled', true);
                $('#RefferenceSJNo').attr('disabled', true);
                $('#btnRefferenceSJNo').attr('disabled', true);
                $('#BPUNo').removeAttr('disabled');
                $('#btnBPUNo').removeAttr('disabled');
                break;
            case '5':
                $('label[btn-radio="\'6\'"]').text('Off');
                $('label[btn-radio="\'5\'"]').text('On');
                $('label[btn-radio="\'4\'"]').text('Off');

                $('#RefferenceDONo').removeAttr('disabled');
                $('#btnRefferenceDONo').removeAttr('disabled');
                $('#RefferenceSJNo').attr('disabled', true);
                $('#btnRefferenceSJNo').attr('disabled', true);
                $('#BPUNo').attr('disabled', true);
                $('#btnBPUNo').attr('disabled', true);
                break;
            case '6':
                $('label[btn-radio="\'6\'"]').text('On');
                $('label[btn-radio="\'5\'"]').text('Off');
                $('label[btn-radio="\'4\'"]').text('Off');

                $('#RefferenceDONo').attr('disabled', true);
                $('#btnRefferenceDONo').attr('disabled', true);
                $('#RefferenceSJNo').removeAttr('disabled');
                $('#btnRefferenceSJNo').removeAttr('disabled');
                $('#BPUNo').attr('disabled', true);
                $('#btnBPUNo').attr('disabled', true);
                break;
        }
       
    }, true);

    me.stdChangedMonitoring = function (n, o) {
        if (me.isLoadData) {
            me.isPrintAvailable = true;
        }

        if (!me.isInProcess) {
            var eq = (n == o);

            // check apakah perubahan data tersebut memiliki nilai atau object kosong (empty object)
            if (!(_.isEmpty(n)) && !eq) {
                if (!me.hasChanged && !me.isLoadData) {
                    me.hasChanged = true;
                    me.isLoadData = false;
                }
                if (!me.isSave) {
                    //me.isLoadData = true;
                    me.hasChanged = true;
                    me.isSave = true;
                }
            } else {
                me.hasChanged = false;
                me.isSave = false;
            }
        }
    }

    me.$watch('isLoadData', function (newData, oldData) {
        if (me.data.StatusDesc != "NEW" && me.data.StatusDesc != undefined) {
            if (!me.isLoadData) {
                me.isLoadData = true;
            }
            else {
                me.isLoadData = true;
            }
        }
        else {
            me.isLoadData = false;
        }
    });

    me.saveData = function (e, param) {
        console.log(me.switchPO);
        if (me.switchPO == '2')
        {
            if(me.data.PONo==null)
            {
                MsgBox("PO Harus diisi", MSG_ERROR);
                return;
            }
        }

        if (me.options == '0') {
            $http.post('om.api/hpp/save', me.data)//{ model: me.data, salesmodelcode: me.detail.SalesModelCode, options: me.options })
            .success(function (data, status, headers, config) {
                if (data.success) {
                    //$('#HPPNo').val(data.data.HPPNo);
                    me.data.HPPNo = data.data.HPPNo;
                    me.data.RefferenceInvoiceDate = data.data.RefferenceInvoiceDate;
                    me.data.RefferenceFakturPajakDate = data.data.RefferenceFakturPajakDate;
                    me.data.DueDate = data.data.DueDate;
                    Wx.Success("Data saved...");
                    me.checkStatus("0");
                    me.startEditing();
                    $('#btnAddBPU').removeAttr('disabled');
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
                error(function (data, status, headers, config) {
                    MsgBox("Connection to the server failed...", MSG_INFO);
                });
        } else {
            $http.post('om.api/hpp/saveUpl', { model: me.data, BPUNo: me.detail.BPUNo, BatchNo: me.data.BatchNo }) //{ model: me.data, salesmodelcode: me.detail.SalesModelCode, options: me.options })
            .success(function (data, status, headers, config) {
                if (data.success) {
                    //$('#HPPNo').val(data.data.HPPNo);
                    me.data.HPPNo = data.data.HPPNo;
                    Wx.Success("Data saved...");
                    me.checkStatus("0");
                    me.startEditing();
                    me.data.isNew == false;
                    me.loadDetail(data.data, 1);
                    //$http.post('om.api/hpp/BPUDetailLoad', { HPPNo: data.data.HPPNo }).
                    //success(function (dt, status, headers, config) {
                    //        //me.loadTableData(me.grid1, dt.data);
                    //        me.loadDetail
                    //        if (dt.bUpload) {
                    //            me.options = "1"
                    //            me.HideDetailButtons();
                    //        }
                    //});

                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox("Connection to the server failed...", MSG_INFO);
            });
        }
    };

    me.AddEditBPU = function () {
        
        //alert(me.detail.TrainingCode);
        if (me.detail.BPUNo == '' || !me.detail.BPUNo) {
            SimDms.Warning("Please fill BPU No!");
        } else {
            me.linkHPPNo();
            me.detail.HPPNo = me.data.HPPNo;
            console.log(me.detail);
            $http.post('om.api/hpp/save2', me.detail).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success(data.message);
                        me.startEditing();
                        me.clearTable(me.grid1);    
                        me.grid.model = data.data;
                        me.loadDetail(data.data, 1);
                        //me.loadTableData(me.grid1, me.grid.model);
                        me.detail = {};
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
        }

    };

    me.AddEditDSM = function () {
        var Field = "SalesModelCode,SalesModelYear";
        var Names = "Sales Model Code,Sales Model Year";
        var ret = me.CheckMandatory(Field, Names);
        if (ret != "") {
            MsgBox(ret + " Harus diisi terlebih dahulu !", MSG_INFO);
        } else {
            if (me.detail.BPUNo == '' || !me.detail.BPUNo) {
                SimDms.Warning("Please fill BPU No!");
            } else {
                me.linkHPPNo();
                $http.post('om.api/hpp/save3', me.detail2).
                    success(function (data, status, headers, config) {
                        if (data.success) {
                            Wx.Success(data.message);
                            me.startEditing();
                            me.clearTable(me.grid2);
                            me.grid.model = data.data;
                            me.loadTableData(me.grid2, me.grid.model);
                            me.AddEditSDSM('x');
                            me.detail2 = {};
                        } else {
                            MsgBox(data.message, MSG_ERROR);
                        }
                    }).
                    error(function (data, status, headers, config) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    });
            }
        }

    };

    me.AddEditDSMO = function () {
        //alert(me.detail.TrainingCode);
        me.detail3.HPPNo = me.data.HPPNo;
        me.detail3.BPUNo = me.detail.BPUNo;
        me.detail3.SalesModelCode = me.detail2.SalesModelCode;
        me.detail3.SalesModelYear = me.detail2.SalesModelYear;

        if (me.detail.BPUNo == '' || !me.detail.BPUNo) {
            SimDms.Warning("Please fill BPU No!");
        } else {
            me.linkHPPNo();

            $http.post('om.api/hpp/save5', me.detail3).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success(data.message);
                        me.loadDetail(me.detail3, 3);
                        me.detail3 = {};
                        me.startEditing();
                        //me.clearTable(me.grid3);
                        //me.grid.model = data.data;
                        //me.loadTableData(me.grid3, me.grid.model);
                        
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
        }

    };

    me.AddEditSDSM = function (x) {
        if (!x) {
            me.detail4.HPPNo = me.data.HPPNo;
            me.detail4.BPUNo = me.detail.BPUNo;
            me.detail4.SalesModelCode = me.detail2.SalesModelCode;
            me.detail4.SalesModelYear = me.detail2.SalesModelYear;
            if (me.detail.BPUNo == '' || !me.detail.BPUNo) {
                SimDms.Warning("Please fill BPU No!");
            } else {
                me.linkHPPNo();
                me.detail4.SalesModelCode = me.detail2.SalesModelCode;
                me.detail4.BPUNo = me.detail2.BPUNo;
                $http.post('om.api/hpp/save4', me.detail4).
                    success(function (data, status, headers, config) {
                        if (data.success) {
                            Wx.Success(data.message);
                            me.startEditing();
                            me.clearTable(me.grid4);
                            me.grid.model = data.data;
                            //me.loadTableData(me.grid4, me.grid.model);
                            me.detail4 = {};
                            me.loadDetail(data.data, 4);
                        } else {
                            MsgBox(data.message, MSG_ERROR);
                        }
                    }).
                    error(function (data, status, headers, config) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    });
            }
        } else {
            me.detail4.HPPNo = me.data.HPPNo;
            me.detail4.BPUNo = me.detail.BPUNo;
            me.detail4.SalesModelCode = me.detail2.SalesModelCode;
            me.detail4.SalesModelYear = me.detail2.SalesModelYear;
            me.detail4.EngineNo = me.detail2.EngineNo;
            if (me.detail.BPUNo == '' || !me.detail.BPUNo) {
                SimDms.Warning("Please fill BPU No!");
            } else {
                me.linkHPPNo();
                me.detail4.SalesModelCode = me.detail2.SalesModelCode;
                me.detail4.BPUNo = me.detail2.BPUNo;
                
               if ($("#isC4").prop('checked'))
               $http.post('om.api/hpp/SaveHppSubFromAllBpuDetail', me.detail2).
                   success(function (data, status, headers, config) {
                       if (data.success) {
                           Wx.Success(data.message);
                           me.startEditing();
                           me.clearTable(me.grid4);
                           me.grid.model = [];// data.data;
                           me.loadTableData(me.grid4, me.grid.model);
                           me.detail4 = {};
                       } else {
                           MsgBox(data.message, MSG_ERROR);
                       }
                   }).
                   error(function (data, status, headers, config) {
                       MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   });
            }
        }

    };

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('om.api/hpp/Delete', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    me.init();
                    Wx.Success("Data deleted...");
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        });
    }

    me.delete2 = function ()
    {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('om.api/hpp/Delete2', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data deleted...");
                    me.loadDetail(me.data, 1);
                    me.detail = {};
                    $("[data-id='tabSub'], #pnlSalesModel, #pnlOthers, #pnlSubSalesModel").hide();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        });
    }

    me.delete3 = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/hpp/Delete3', me.detail2).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.clearTable(me.grid2);
                        me.grid.model = data.data;
                        me.loadTableData(me.grid2, me.grid.model);
                        Wx.Success("Data deleted...");
                        me.loadDetail(me.data, 2);
                        me.delete5('x');
                        me.detail2 = {};

                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.delete4 = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('om.api/hpp/Delete5', me.detail3).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data deleted...");
                    me.loadDetail(me.detail3, 3);
                    me.detail3 = {};
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        });
    }

    me.delete5 = function (x) {
        if (!x) {
            MsgConfirm("Are you sure to delete current record?", function (result) {
                $http.post('om.api/hpp/Delete4', me.detail4).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        //me.init();
                        Wx.Success("Data deleted...");
                        me.detail4 = {};                    
                        me.loadDetail(me.detail2, 4);
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            });
        } else {
            $http.post('om.api/hpp/DeleteAllbyHppDetail', me.detail2).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        //me.init();
                        Wx.Success("Data deleted...");
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
        }
    }

    me.CloseModel = function () {
        me.detail = {};
        me.grid1.clearSelection();

        me.clearSubDetailBPU();
    }
    me.CloseModel0 = function () {
        me.detail2 = {};
        me.grid2.clearSelection();
    }
    me.CloseModel1 = function () {
        me.detail3 = {};
        me.grid3.clearSelection();
    }
    me.CloseModel2 = function () {
        me.detail4 = {};
        me.grid4.clearSelection();
    }
    me.linkHPPNo = function (){
        me.detail.HPPNo = me.data.HPPNo;
        me.detail.PONo = me.data.PONo;
        me.detail2.HPPNo = me.data.HPPNo;
        me.detail2.BPUNo = me.detail.BPUNo;
    }

    me.HideDetailButtons = function () {
        $('#btnAddBPU').show();
        $('#btnUpdateBPU, #btnDeleteBPU, #btnCancelBPU').hide();
        $('#btnAddPC, #btnDltPC, #btnClrPC').hide();
    }

    me.initialize = function () {
        me.clearTable(me.grid1);
        // me.clearTable(me.grid2);
        me.justread = false;
        me.data.isNew == false;
        me.clearTable(me.gridDetailColour);
        me.colour = {};
        me.detail = {};
        me.detail2 = {};
        me.detail3 = {};
        me.detail4 = {};
        
        $('#RefferenceFakturPajakDate').attr('readonly', true);
      
        //me.data.LockingDate = moment().format("MMM yyyy");
     
        //var x = 'HPPDate,RefferenceInvoiceNo,isC1,RefferenceInvoiceDate,RefferenceFakturPajakNo,isC2,RefferenceFakturPajakDate,isC3,DueDate,PONo,SupplierName,RefferenceNo,BillTo,Remark';
        //localStorage.setItem('MyData', x);
        $("#label[ng-model='options']").attr('disabled', true);
        $("#label[ng-model='options']").attr('disabled', true);
        //x.length();
        //alert(z);
        //$('#pnlBPU').hide();
        //$('#wxbpu').hide();
        $("[data-id='tabSub']").hide();
        $('#pnlSalesModel').hide();
        $('#pnlOthers').hide();
        $('#pnlSubSalesModel').hide();

        $('#isC3').prop('checked', true);
        $('#isC1').prop('checked', true);
        //$('#statusLbl').text("NEW");
        $('#statusLbl').css(
        {
            "font-size": "34px",
            "color": "red",
            "font-weight": "bold",
            "text-align": "center"
        });

        $('label[btn-radio="\'6\'"]').text('Off');
        $('label[btn-radio="\'5\'"]').text('Off');
        $('label[btn-radio="\'4\'"]').text('On');
        $('#RefferenceDONo').attr('disabled', true);
        $('#btnRefferenceDONo').attr('disabled', true);
        $('#RefferenceSJNo').attr('disabled', true);
        $('#btnRefferenceSJNo').attr('disabled', true);
        $('#BPUNo').removeAttr('disabled');
        $('#btnBPUNo').removeAttr('disabled');
        $http.get('breeze/sales/ProfitCenter').
        success(function (dl, status, headers, config) {
            me.data.ProfitCenterCode = dl.ProfitCenter;
            me.data.HPPDate = me.now();
            me.data.DueDate = me.now();
            me.data.RefferenceFakturPajakDate = me.now();
            me.data.RefferenceInvoiceDate = me.now();
            me.data.LockingDate = me.now();
        });
        me.detail2.isC4 = true;
        me.detail2.isC5 = true;
        me.detail2.AllQty = true;
        $('#isC5').prop('checked', true);
        $('#isC4').prop('checked', true);
        $('#btnAddBPU').attr('disabled', true);
        $('#btnSalesModelCode,#btnSalesModelYear').removeAttr("style");
        me.checkStatus('new')
    }

    $("[name = 'isC4']").on('click', function () {
        if ($('#isC4').prop('checked') == false) {
            $('#Quantity').removeAttr('disabled');
            me.detail2.AllQty = false;
        } else {
            $('#Quantity').attr('disabled', true);
            me.detail2.AllQty = true;
        }
    });

    $("[name = 'isC5']").on('click', function () {
        if ($('#isC5').prop('checked') == true) {
            $('#AfterDiscTotal').removeAttr('disabled');

            $('#AfterDiscDPP').attr('disabled', true);
            $('#AfterDiscPPn').attr('disabled', true);
            $('#AfterDiscPPnBM').attr('disabled', true);
        } else {
            $('#AfterDiscTotal').attr('disabled', true);

            $('#AfterDiscDPP').removeAttr('disabled');
            $('#AfterDiscPPn').removeAttr('disabled');
            $('#AfterDiscPPnBM').removeAttr('disabled');
        }
    });

    $('div > p').click(function () {
        var name = $(this).data("name");
        if (name == "tabSalesModel") {
            me.loadDetail(me.detail, 2);
        } else if (name == "tabOthers") {
            me.loadDetail(me.detail2, 3);
        } else if (name == "tabSubSalesModel") {
            me.loadDetail(me.detail2, 4);
        }
    });

    $('#AfterDiscDPP').on('blur', function (e) {
        var a = $('#BeforeDiscDPP').val();
        var b = $('#AfterDiscDPP').val();
        a = a.split(',').join('');
        b = b.split(',').join('');
        //var c = parseFloat(a) - parseFloat(b);
        var c = parseInt(a) - parseInt(b);
        $('#DiscExcludePPn').val(c);
    })

    $('#PONo').on('blur', function (e) {
        if ($('#PONo').val() || $('#PONo').val() != '') {
            $http.post('om.api/HPP/POView?PONo=' + $('#PONo').val()).
            success(function (v, status, headers, config) {
                if (v.TitleName != '') {
                    //me.lookupAfterSelect(v.data);
                    me.data.BillTo = v.data.BillTo;
                    me.data.SupplierCode = v.data.SupplierCode;
                    me.data.SupplierName = v.data.SupplierName;
                    me.data.RefferenceNo = v.data.RefferenceNo;
                } else {
                    $('#PONo').val('');
                    $('#SupplierCode').val('');
                    $('#SupplierName').val('');
                    $('#RefferenceNo').val('');
                    me.PO();
                }
            });
        } else {
            $('#PONo').val('');
            $('#SupplierCode').val('');
            $('#SupplierName').val('');
            $('#RefferenceNo').val('');
            me.PO();
        }
    })

    $('#isC1').on('change', function (e) {
        if ($('#isC1').prop('checked') == true) {
            me.data.RefferenceInvoiceDate = me.now();
            $('#RefferenceInvoiceDate').prop('readonly', false);
        } else {
            me.data.RefferenceInvoiceDate = undefined;
            $('#RefferenceInvoiceDate').prop('readonly', true);
        }
        me.Apply();
    })

    $('#isC2').on('change', function (e) {
        if ($('#isC2').prop('checked') == true) {
            me.data.RefferenceFakturPajakDate = me.now();
            me.data.LockingDate = me.data.RefferenceFakturPajakDate;
            $('#RefferenceFakturPajakDate').prop('readonly', false);
        } else {
            //me.data.RefferenceFakturPajakDate = undefined;
            $('#RefferenceFakturPajakDate').prop('readonly', true);
        }
        me.Apply();
    })

    $('#RefferenceFakturPajakDate').on('blur', function (e) {
        me.data.LockingDate = me.data.RefferenceFakturPajakDate;
        me.Apply()
    })

    $('#isC3').on('change', function (e) {
        if ($('#isC3').prop('checked') == true) {
            me.data.DueDate = me.now();
            $('#DueDate').prop('readonly', false);
        } else {
            me.data.DueDate = undefined;
            $('#DueDate').prop('readonly', true);
        }
        me.Apply();
    })

    me.printPreview = function () {
        $http.post('om.api/HPP/PrintValidate', { HPPNo: me.data.HPPNo })
           .success(function (e) {
               if (e.success) {
                   BootstrapDialog.show({
                       message: $(
                           '<div class="container">' +
                           '<div class="row" style="margin: 0 auto; padding-bottom: 20px;">' +

                           '<input type="radio" name="sizeType" id="sizeType1" value="full" checked style="cursor: pointer;"><label for="sizeType1" style="margin-left:20px; margin-top: -25px;">&nbsp Print Satu Halaman</label></div>' +

                           '<div class="row" style="margin:0 auto;">' +

                           '<input type="radio" name="sizeType" id="sizeType2" value="half" style="cursor: pointer;"><label for="sizeType2" style="margin-left:20px; margin-top: -25px;">&nbsp Print Setengah Halaman</label></div>'),
                       closable: false,
                       draggable: true,
                       type: BootstrapDialog.TYPE_INFO,
                       title: 'Print',
                       buttons: [{
                           label: ' Print',
                           cssClass: 'btn-primary icon-print',
                           action: function (dialogRef) {
                               console.log(me.data.Status);
                               if (me.data.Status == 0 || me.data.StatusDesc == "OPEN") {
                                   $http.post('om.api/hpp/updateHdr', me.data).
                                    success(function (data, status, headers, config) {
                                        if (data.success) {
                                            me.checkStatus(data.data.Status);
                                            $('#btnApprove').removeAttr('disabled');
                                            me.print();
                                            dialogRef.close();
                                        } else {
                                            dialogRef.close();
                                            MsgBox(data.message, MSG_ERROR);
                                        }
                                    }).
                                    error(function (data, status, headers, config) {
                                        //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                                        MsgBox(data.message, MSG_ERROR);
                                    });
                               }
                           }
                       }, {
                           label: ' Cancel',
                           cssClass: 'btn-warning icon-remove',
                           action: function (dialogRef) {
                               dialogRef.close();
                           }
                       }]
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

    me.print = function () {
        var sizeType = $('input[name=sizeType]:checked').val() === 'full';
        var ReportId = sizeType ? 'OmRpPurTrn003' : 'OmRpPurTrn003A';

        console.log(ReportId);
        var par = [
            me.data.HPPNo,
            me.data.HPPNo,
            me.data.ProfitCenterCode,
            ""
        ]
        var rparam = 'Print HPP'

        Wx.showPdfReport({
            id: ReportId,
            pparam: par.join(','),
            rparam: rparam,
            type: "devex"
        });
    }

    me.approve= function(){
        $http.post('om.api/hpp/Approve', me.data).
             success(function (data, status, headers, config) {
                 if (data.success) {
                     Wx.Success(data.message);
                     me.checkStatus(data.status);
                     $('#btnApprove').attr('disabled', true);
                 } else {
                     MsgBox(data.message, MSG_ERROR);
                 }
             }).
             error(function (data, status, headers, config) {
                 //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                 MsgBox(data.message, MSG_ERROR);
             });
    }

    me.initGrid = function () {
        me.grid1 = new webix.ui({
            container: "wxbpu",
            view: "wxtable", css:"alternating", scrollX: true,
            columns: [
                { id: "BPUNo", header: "BPU No.", fillspace: true },
                { id: "RefferenceDONo", header: "DO No.", fillspace: true },
                { id: "RefferenceSJNo", header: "SJ No.", fillspace: true }
            ],
            on: {
                onSelectChange: function () {
                    if (me.grid1.getSelectedId() !== undefined) {
                        //alert(me.grid1.getSelectedId().id.columns.ColourCode);
                        me.detail = this.getItem(me.grid1.getSelectedId().id);
                        me.detail.oid = me.grid1.getSelectedId();
                        //console.log(this.getItem(me.grid1.getSelectedId().id))
                        me.Apply();
                        if ($("[data-id='tabSub']").is(":hidden")) {
                            $("[data-id='tabSub']").show();
                            $('#pnlSalesModel').show();
                        }


                        me.detail.HPPNo = me.data.HPPNo;
                        me.data.BPUNo = me.detail.BPUNo;
                        me.data.SalesModelCode = me.detail.SalesModelCode;
                        me.data.SalesModelYear = me.detail.SalesModelYear;

                        me.loadDetail(me.detail, 2);
                        me.clearSubDetailBPU();
                    }
                }
            }
        });
    }

    me.clearSubDetailBPU = function () {
        me.detail2 = {};
        me.detail3 = {};
        me.detail4 = {};

        me.clearTable(me.grid2);
        me.clearTable(me.grid3);
        me.clearTable(me.grid4);
    }

    me.grid2 = new webix.ui({
        container: "wxsalesmodel",
        view: "wxtable", css:"alternating", scrollX: true,
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 200 },
            { id: "SalesModelYear", header: "Sales Model Year", width: 200 },
            { id: "Quantity", header: "Jumlah", width: 75, css: "rightcell", format: me.intFormat },
            { id: "AfterDiscTotal", header: "Harga Setelah Diskon", width: 250, css: "rightcell", format: me.decimalFormat },
            { id: "AfterDiscDPP", header: "DPP Setelah Diskon", width: 250, css: "rightcell", format: me.decimalFormat },
            { id: "AfterDiscPPn", header: "PPN Setelah Diskon", width: 250, css: "rightcell", format: me.decimalFormat },
            { id: "AfterDiscPPnBM", header: "PPnBM Setelah Diskon", width: 250, css: "rightcell", format: me.decimalFormat },
            { id: "OthersDPP", header: "DPP Lain-lain", width: 250, css: "rightcell", format: me.decimalFormat },
            { id: "OthersPPn", header: "PPn Lain-lain", width: 250, css: "rightcell", format: me.decimalFormat },
            { id: "DiscExcludePPn", header: "Diskon Tanpa PPn", width: 200, css: "rightcell", format: me.decimalFormat }
        ], checkboxRefresh: true,
        on: {
            onSelectChange: function () {
                if (me.grid2.getSelectedId() !== undefined) {
                    //alert(me.grid1.getSelectedId().id.columns.ColourCode);
                    me.detail2 = this.getItem(me.grid2.getSelectedId().id);
                    me.detail2.oid = me.grid2.getSelectedId();
                    //console.log(this.getItem(me.grid1.getSelectedId().id))
                    me.Apply();
                }
            }
        }
    });

    me.grid3 = new webix.ui({
        container: "wxothers",
        view: "wxtable", css:"alternating", scrollX: true,
        columns: [
            { id: "OthersCode", header: "Code", fillspace: true },
            { id: "OthersDPP", header: "Others DPP", fillspace: true, css: "rightcell", format: me.decimalFormat },
            { id: "OthersPPN", header: "Others PPn", fillspace: true, css: "rightcell", format: me.decimalFormat }
        ], checkboxRefresh: true,
        on: {
            onSelectChange: function () {
                if (me.grid3.getSelectedId() !== undefined) {
                    //alert(me.grid1.getSelectedId().id.columns.ColourCode);
                    me.detail3 = this.getItem(me.grid3.getSelectedId().id);
                    me.detail3.oid = me.grid3.getSelectedId();
                    //console.log(this.getItem(me.grid1.getSelectedId().id))
                    me.Apply();
                }
            }
        }
    });

    me.grid4 = new webix.ui({
        container: "wxsubsalesmodel",
        view: "wxtable", css:"alternating", scrollX: true,
        columns: [
            { id: "ChassisCode", header: "Kode Rangka", fillspace: true },
            { id: "ChassisNo", header: "No Rangka", fillspace: true },
            { id: "EngineCode", header: "Kode Mesin", fillspace: true },
            { id: "EngineNo", header: "No Mesin", fillspace: true },
            { id: "ColourCode", header: "Kode Warna", fillspace: true }
        ], checkboxRefresh: true,
        on: {
            onSelectChange: function () {
                if (me.grid4.getSelectedId() !== undefined) {
                    //alert(me.grid1.getSelectedId().id.columns.ColourCode);
                    me.detail4 = this.getItem(me.grid4.getSelectedId().id);
                    me.detail4.oid = me.grid4.getSelectedId();
                    //console.log(this.getItem(me.grid1.getSelectedId().id))
                    //alert(me.detail4.HPPSeq);
                    me.Apply();
                }
            }
        }
    });
    me.initGrid();

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })
    me.start();
    me.options = '0';
    me.switchPO = '2';
    
}


$(document).ready(function () {
    var options = {
        title: "HPP",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnloptions",
                items: [
                      {
                          type: "optionbuttons",
                          name: "tabpageoptions",
                          model: "options",
                          cls: "span3",
                          items: [
                              { name: "0", text: "Manual" },
                              { name: "1", text: "Upload" },
                          ]
                      },
                      {
                          type: "buttons", cls: "span2", items: [
                                 { name: "btnApprove", text: "Approve", cls: "btn btn-info", icon: "icon-ok", click: "approve()", disable: true },
                          ]
                      },
                      { type: "label", name: "statusLbl", text: "", cls: "span3" }
                ]
            },
            {// 
                name: "pnlPO",
                items: [
                    { name: "HPPNo", text: "HPP No.", cls: "span4", readonly: true, placeHolder: 'HPP/XX/YYYYYY' },
                    { name: "HPPDate", text: "HPP Date", cls: "span4", type: "ng-datepicker" },
                    { name: "RefferenceInvoiceNo", text: "Reff Inv. No. ", cls: "span4", readonly: false, type: "popup", click: "ReffInv()", required: true, validasi: "required" },
                    {
                        text: "Reff Inv. Date",
                        type: "controls",
                        cls: "span4",
                        items: [
                           { name: 'isC1', type: 'check', text: 'Tax Centralized', cls: 'span1', float: 'left' },
                            { name: "RefferenceInvoiceDate", text: "Reff Inv. Date", cls: "span7", type: 'ng-datepicker', disabled: true },
                        ]
                    },
                    { name: "RefferenceFakturPajakNo", text: "Reff F. P. No. ", cls: "span4", readonly: false, required: true, validasi: "required"  },
                    {
                        text: "Reff F. P. Date",
                        type: "controls",
                        cls: "span4",
                        items: [
                           { name: 'isC2', type: 'check', text: 'Tax Centralized', cls: 'span1', float: 'left' },
                           { name: "RefferenceFakturPajakDate", text: "Reff F. P. Date", cls: "span7", type: 'ng-datepicker', readonly: true },
                        ]
                    },
                    { name: "LockingDate", text: "Masa Pajak", cls: "span4", type: 'ng-datepicker', disabled: true, format: 'MMM yyyy', required: true },
                    {
                       text: "Due Date",
                       type: "controls",
                       cls: "span4",
                       items: [
                          { name: 'isC3', type: 'check', text: 'Tax Centralized', cls: 'span1', float: 'left' },
                          { name: "DueDate", text: "Due Date", cls: "span7", type: 'ng-datepicker', disabled: true },

                       ]
                    },
                    {
                        type: "optionbuttons",
                        name: "switchPO",
                        model: "switchPO",
                        cls: "span1",
                        items: [
                            { name: "2", text: "On" },
                            { name: "3", text: "On" },

                        ]
                    },
                    { name: "PONo", cls: "span3", text: "PO No.", type: "popup", click: "PO()", disable: false, required: true },
                    { name: "SupplierCode", model: "data.SupplierCode", cls: "span3", text: "", show: "true" },
                    { name: "SupplierName", text: "Supplier", cls: "span4", readonly: true },
                    { name: "RefferenceNo", cls: "span3", text: "Reff No.", type: "popup", click: "Reff()", disable: true },
                    { name: "BillTo", text: "Bill To", cls: "span4", readonly: true },
                    { name: "Remark", text: "Notes", cls: "span8", readonly: false },

                ]
            },
            {
                name: "pnlBPU",
                title : "BPU Detail",
                items: [
                     {
                         type: "optionbuttons",
                         name: "switchBPU",
                         model: "switchBPU",
                         cls: "span1",
                         items: [
                             { name: "4", text: "On" },
                             { name: "5", text: "Off" },
                             { name: "6", text: "Off" },
                         ]
                     },
                    { name: "BPUNo", model: "detail.BPUNo", text: "BPU No. ", cls: "span4", readonly: false, type: "popup", click: "BPU()" },
                    { name: "RefferenceDONo", model: "detail.RefferenceDONo", text: "Reff Do. No. ", cls: "span4", readonly: false, type: "popup", click: "ReffDO()" },
                    { name: "RefferenceSJNo", model: "detail.RefferenceSJNo", text: "Reff SJ. No. ", cls: "span4", readonly: false, type: "popup", click: "ReffSJ()" },
                    { name: "Remark", model: "detail.Remark", text: "Notes", cls: "span8", readonly: false, type: "textarea" },
                     {
                         type: "buttons",
                         items: [
                                 { name: "btnAddBPU", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddEditBPU()", show: "detail.oid === undefined", disable: true },
                                 { name: "btnUpdateBPU", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddEditBPU()", show: "detail.oid !== undefined" },
                                 { name: "btnDeleteBPU", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete2()", show: "detail.oid !== undefined" },
                                 { name: "btnCancelBPU", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel()", show: "detail.oid !== undefined" }
                         ]
                     },
                ]
            },
             {
                 name: "wxbpu",
                 xtype: "wxtable",
             },
             {
                 xtype: "tabs",
                 name: "tabSub",
                 id: "tabSub",
                 items: [
                     { name: "tabSalesModel", text: "Detail Sales Model" },
                     { name: "tabOthers", text: "Others" },
                     { name: "tabSubSalesModel", text: "Detail Sub Sales Model" },
                 ]
             },
             {
                 name : 'pnlSalesModel',
                 //title: "Detail Sales Model",
                 cls: "tabSub tabSalesModel",
                 items: [
                     
                           { name: "SalesModelCode", model: "detail2.SalesModelCode", cls: "span4 ", placeHolder: "Sales Model Code", type: "popup", btnName: "btnPartNo", disabled: "!datas.isLoad", click: "salesModelCode()", required: true, text: "Sales Model Code", style: "background-color: rgb(255, 218, 204)" },
                           {
                               text: "Harga Total",
                               type: "controls",
                               cls: "span4",
                               items: [
                                  { name: 'isC5', model: "detail2.isC5", type: 'check', text: 'Tax Centralized', cls: 'span2', float: 'left' },
                                  { name: "AfterDiscTotal", model: "detail2.AfterDiscTotal", text: "After Discount Total", cls: "span6 full number-int", style: "background-color: rgb(255, 218, 204)" },
                               ]
                           },
                           { name: "SalesModelYear", model: "detail2.SalesModelYear", cls: "span4", placeHolder: "Sales Model Year", type: "popup", btnName: "btnPartNo", disabled: "!datas.isLoad", click: "salesModelYear()", required: true, text: "Sales Model Year", style: "background-color: rgb(255, 218, 204)" },
                            { name: "AfterDiscDPP", model: 'detail2.AfterDiscDPP', type: "text", text: "DPP", cls: "span4 number-int", disable: true },
                          {
                              text: "Semua",
                              type: "controls",
                              cls: "span4",
                              items: [
                                 { name: 'isC4', model: "detail2.AllQty", type: 'check', text: 'Semua', cls: 'span2', float: 'left' }, 
                                 { name: "Quantity", model: "detail2.Quantity", text: "Quantity", cls: "span6 full number-int", disable: true },
                              ]
                          },
                           { name: "AfterDiscPPn", model: 'detail2.AfterDiscPPn', type: "text", text: "PPn", cls: "span4 number-int", disable: true },
                           { name: "PPnBMPaid", model: 'detail2.PPnBMPaid', text: "PPnBM telah dibayar", cls: "span4 number-int", readonly: true, disable: true },
                           { name: "AfterDiscPPnBM", model: 'detail2.AfterDiscPPnBM', type: "text", text: "PPnBM", cls: "span4 number-int", disable: true },
                           { name: "BeforeDiscDPP", model: 'detail2.BeforeDiscDPP', type: "text", text: "DPP sebelum Diskon", cls: "span4 number-int", disable: true },
                           { name: "OthersDPP", model: 'detail2.OthersDPP', type: "text", text: "DPP Lain-lain", cls: "span4 number-int" },
                           { name: "DiscExcludePPn", model: 'detail2.DiscExcludePPn', type: "text", text: "Diskon", cls: "span4 number-int",  disable: true },
                           { name: "OthersPPn", model: 'detail2.OthersPPn', type: "text", text: "PPN Lain-lain", cls: "span4 number-int" },
                           { name: "Remark", model: 'detail2.Remark', type: "text", text: "Keterangan", cls: "span8" },
                     {
                         type: "buttons", cls: "span4", items: [
                             { name: "btnAddPC", text: "Add", icon: "icon-plus", click: "AddEditDSM()", cls: "btn btn-primary", disable: "detail.oid === undefined || (detail2.SalesModelCode === undefined || detail2.SalesModelYear === undefined)" },
                             { name: "btnDltPC", text: "Delete", icon: "icon-remove", click: "delete3()", cls: "btn btn-danger", disable: "detail.oid === undefined || (detail2.SalesModelCode === undefined || detail2.SalesModelYear === undefined)" },
                             { name: "btnClrPC", text: "Clear", icon: "icon-remove", click: "CloseModel0()", cls: "btn btn-danger", disable: "detail.oid === undefined" },
                         ]
                     },

                     {
                         name: "wxsalesmodel",
                         type: "wxdiv",
                     },
                 ]
             },
             {
                 name: 'pnlOthers',
                 //title: "Detail Sales Model",
                 cls: "tabSub tabOthers",
                 items: [
                           { name: "OthersCode", model: 'detail3.OthersCode', type: "text", text: "Code", cls: "span8" },
                           { name: "OthersDPP", model: 'detail3.OthersDPP', type: "text", text: "DPP", cls: "span4 number-int"},
                           { name: "OthersPPN", model: 'detail3.OthersPPN', type: "text", text: "PPN", cls: "span4 number-int" },
                     {
                         type: "buttons", cls: "span4", items: [
                             {
                                 name: "btnAddPC2", text: "Add", icon: "icon-plus", click: "AddEditDSMO()", cls: "btn btn-primary",
                                 disable: "detail.oid === undefined || (detail2.SalesModelCode === undefined || detail2.SalesModelYear === undefined) || (detail3.OthersCode === undefined)"
                             },
                             {
                                 name: "btnDltPC2", text: "Delete", icon: "icon-remove", click: "delete4()", cls: "btn btn-danger",
                                 disable: "detail.oid === undefined || (detail2.SalesModelCode === undefined || detail2.SalesModelYear === undefined) || (detail3.OthersCode === undefined)"
                             },
                             { name: "btnClrPC2", text: "Clear", icon: "icon-remove", click: "CloseModel1()", cls: "btn btn-danger", disable: "detail.oid === undefined" },
                             //{ name: "btnAddPC", text: "Add", icon: "icon-plus", click: "AddEditDSMO()", cls: "btn btn-primary", disable: "detail2.SalesModelCode === undefined" },
                             //{ name: "btnDltPC", text: "Delete", icon: "icon-remove", click: "Delete4()", cls: "btn btn-danger", disable: "detail2.SalesModelCode === undefined" },
                             //{ name: "btnClrPC", text: "Clear", icon: "icon-remove", click: "CloseModel1()", cls: "btn btn-danger", disable: "detail2.SalesModelCode === undefined" },
                         ]
                     },

                     {
                         name: "wxothers",
                         type: "wxdiv",
                     },
                 ]
             },
             {
                 name: 'pnlSubSalesModel',
                 //title: "Detail Sales Model",
                 cls: "tabSub tabSubSalesModel",
                 items: [
                            { name: "ChassisCode", model: "detail4.ChassisCode", cls: "span4 full", type: "popup", btnName: "btnPartNo", disabled: "!datas.isLoad", click: "ChassisCode()", required: true, validasi: "required", text: "Chassis Code" },
                            { name: "ChassisNo", model: "detail4.ChassisNo", cls: "span4 full", type: "popup", btnName: "btnPartNo", disabled: "!datas.isLoad", click: "ChassisNo()", required: true, validasi: "required", text: "Chassis No." },

                     {
                         type: "buttons", cls: "span4", items: [
                             {
                                 name: "btnAddPC3", text: "Add", icon: "icon-plus", click: "AddEditSDSM()", cls: "btn btn-primary",
                                 disable: "(detail2.SalesModelCode === undefined || detail2.SalesModelYear === undefined) || (detail4.ChassisCode === undefined || detail4.ChassisNo === undefined)"
                             },
                             {
                                 name: "btnDltPC3", text: "Delete", icon: "icon-remove", click: "delete5()", cls: "btn btn-danger",
                                 disable: "(detail2.SalesModelCode === undefined || detail2.SalesModelYear === undefined) || (detail4.ChassisCode === undefined || detail4.ChassisNo === undefined)"
                             },
                             { name: "btnClrPC3", text: "Clear", icon: "icon-remove", click: "CloseModel2()", cls: "btn btn-danger", disable: "detail2.SalesModelCode === undefined" },
                         ]
                     },
                     {
                         name: "wxsubsalesmodel",
                         type: "wxdiv",
                     },
                 ]
             },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("omHPPController");
    }

});
