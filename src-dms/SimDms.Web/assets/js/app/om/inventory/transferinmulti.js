var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function omKaroseriController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.loadDetail = function (data) {
        $http.post('om.api/TransferInMulti/DetailLoad?TransferInNo=' + data.TransferInNo).
               success(function (data, status, headers, config) {
                   if (data != '') {
                       //alert('112');

                       me.grid.detail = data;
                       me.loadTableData(me.grid1, me.grid.detail);
                       me.isSave = false;
                       me.isPrintAvailable = true;
                       me.isPrintEnable = true;
                   }
                   else {
                       me.isPrintAvailable = false;
                       me.isPrintEnable = false;
                   }
               }).
               error(function (e, status, headers, config) {
                   console.log(e);
               });
    }

    me.browse = function () {
        me.init();
        var lookup = Wx.blookup({
            name: "TransferInMultiLookup",
            title: "Transfer In Multi Branch",
            manager: spSalesManager,
            query: "TransferInMultiBrowse",
            defaultSort: "TransferInNo desc",
            columns: [
                { field: "TransferInNo", title: "No. TransferIn" },
                { field: "TransferInDate", title: "Tgl. TransferIn", template: "#= (TransferInDate == undefined) ? '' : moment(TransferInDate).format('DD MMM YYYY') #" },
                { field: "TransferOutNo", title: "No. TransferOut" },
                { field: 'TransferOutDate', title: 'Tgl. TransferOut', template: "#= (TransferOutDate == undefined) ? '' : moment(TransferOutDate).format('DD MMM YYYY') #" },
                { field: 'Status', title: 'Status' },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.checkStatus(data.Status);
                me.checkbox(data);
                me.disableHDR(data.Status);
                if (data.Status == 'OPEN' || data.Status == 'PRINTED') {
                    $('#btnAddDetail').attr('disabled', false);
                    $('#btnPrintPreview').removeAttr('disabled');
                    $('#btnDelete').removeAttr('disabled');
                    me.enableFiil();
                }
                me.lookupAfterSelect(data);
                me.loadDetail(data);
                me.detail.TransferInNo = data.TransferInNo;
                me.Apply();
            }
        });
    }

    me.disableHDR = function (status) {
        if (status == 'PRINTED' || status == 'APPROVED' || status > 0) {
            if (status == 'PRINTED' || status == 1) {
                $('#btnApprove').prop('disabled', false);
            } else {
                $('#btnApprove').prop('disabled', true);
            }
            $('#btnAddDetail').attr('disabled', true);
            $('#btnPrintPreview').attr('disabled', true);
            $('#btnDelete').attr('disabled', true);
            var x = 'TransferInNo,TransferInDate,TransferOutNo,ReferenceNo,ReferenceDate,BranchCodeFrom,WareHouseCodeFrom,BranchCodeTo,WareHouseCodeTo,ReturnDate,Status,isC1,isC2';
            x += 'TransferOutNo,SalesModelCode,SalesModelYear,ChassisCode,ChassisNo,EngineCode,EngineNo,ColourCode,ColourName,StatusTransferIn';
            var y = x.split(',', 50);
            var z = y.length;
            for (i = 0; i <= z; i++) {
                $('#' + y[i]).attr('disabled', true);
                $('#btn' + y[i]).attr('disabled', true);
            }
            $('#Remark').attr('disabled', true);
            $('#isC2').attr('disabled', true);
        }
    }

    me.checkbox = function (data) {
        if ((data.ReferenceDate).substring(0, 4) != "1900") {
            $('#isC1').prop('checked', true);
            $('#ReferenceDate').prop('readonly', false);
            //alert((data.RefferenceFakturPajakDate).substring(0, 4));
        } else {
            $('#isC1').prop('checked', false);
            $('#ReferenceDate').prop('readonly', true);
            me.data.ReferenceDate = undefined;
        }

        if ((data.ReturnDate).substring(0, 4) != "1900") {
            $('#isC2').prop('checked', true);
            $('#ReturnDate').prop('readonly', false);
            //alert((data.RefferenceFakturPajakDate).substring(0, 4));
        } else {
            $('#isC2').prop('checked', false);
            $('#ReturnDate').prop('readonly', true);
            me.data.ReturnDate = undefined;
        }
    }

    me.checkStatus = function (Status) {
        console.log(Status);
        switch ((Status)) {
            case 'OPEN' || '0':
                $('#statusLbl').text("OPEN");
                me.allowEdit = true;
                //me.allowEditDetail = true;
                break;
            case 'PRINTED' || '1':
                $('#statusLbl').text("PRINTED");
                me.allowEdit = true;
                //me.allowEditDetail = true;
                break;
            case 'CLOSED' || '2':
                $('#statusLbl').text("CLOSED");
                me.allowEdit = false;
                $('#btnDelete').attr('disabled', true);
                //me.allowEditDetail = false;
                break;
            case 'DELETED' || '3':
                $('#statusLbl').text("DELETED");
                me.allowEdit = true;
                //me.allowEditDetail = false;
                break;
            case 'TRANSFERED'||'5':
                $('#statusLbl').text("TRANSFERED");
                    me.allowEdit = false;
                    //me.allowEditDetail = false;
                    break;
            case 'FINISHED' || '9':
                $('#statusLbl').text("FINISHED");
                me.allowEdit = false;
                break;
            case '0':
                $('#statusLbl').text("OPEN");
                me.allowEdit = true;
                //me.allowEditDetail = true;
                break;
            case '1':
                $('#statusLbl').text("PRINTED");
                me.allowEdit = true;
                //me.allowEditDetail = true;
                break;
            case '2':
                $('#statusLbl').text("APPROVED");
                me.allowEdit = false;
                //me.allowEditDetail = false;
                break;
            case '3':
                $('#statusLbl').text("DELETED");
                me.allowEdit = true;
                //me.allowEditDetail = false;
                break;
            case '5':
                $('#statusLbl').text("TRANSFERED");
                me.allowEdit = false;
                //me.allowEditDetail = false;
                break;
            case '9':
                $('#statusLbl').text("FINISHED");
                me.allowEdit = false;
                break;
        }
    }

    me.salesModelCode = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeBrowse",
            title: "Model",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("Select4ModelTrfOut").withParameters({ CompanyCodeFrom: me.data.CompanyCodeFrom, TransferOutNo: me.data.TransferOutNo }),
            defaultSort: "SalesModelCode asc",
            columns: [
             { field: "SalesModelCode", title: "Sales Model Code" },
             { field: "SalesModelDesc", title: "Sales Model Desc" },
             { field: 'EngineCode', title: 'Kode Mesin' },
             //{ field: 'Qty', title: 'Quantity' },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail.SalesModelCode = data.SalesModelCode;
            me.detail.EngineCode = data.EngineCode;
            me.Apply();
        });
    }

    me.salesModelYear = function () {
        var lookup = Wx.blookup({
            name: "SalesModelYearBrowse",
            title: "Model Year",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("Select4ModelYearTrfOut").withParameters({ CompanyCodeFrom: me.data.CompanyCodeFrom, TransferOutNo: me.data.TransferOutNo, SalesModelCode: me.detail.SalesModelCode }),
            defaultSort: "ModelYear asc",
            columns: [
               { field: 'ModelYear', title: 'Sales Model Year' },
               { field: 'SalesModelDesc', title: 'Sales Model Desc' },
               { field: 'ChassisCode', title: 'Chassis Code' },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail.SalesModelYear = data.ModelYear;
            me.detail.SalesModelDesc = data.SalesModelDesc;
//            me.detail.ChassisNo = data.ChassisNo;
            me.detail.ChassisCode = data.ChassisCode;
            me.Apply();
        });
    }

    me.Chassis = function () {
        if (me.detail.SalesModelCode && me.detail.SalesModelYear) {//| me.detail.ChassisCode != '') {
            var lookup = Wx.blookup({
                name: "SalesModelYearBrowse",
                title: "Model Year",
                manager: spSalesManager,
                query: new breeze.EntityQuery.from("Select4ChassisTrfOut").withParameters({ CompanyCodeFrom: me.data.CompanyCodeFrom, TransferOutNo: me.data.TransferOutNo, SalesModelCode: me.detail.SalesModelCode, SalesModelYear: me.detail.SalesModelYear, ChassisCode: me.detail.ChassisCode }),
                defaultSort: "ChassisNo asc",
                columns: [
                   { field: 'ChassisNo', title: 'No. Rangka' },
                   { field: 'EngineNo', title: 'No. Mesin' },
                   { field: 'ColourCode', title: 'Kode Warna' },
                   { field: 'ColourName', title: 'Ket. Warna' },
                ]
            });
            lookup.dblClick(function (data) {
                me.detail.ChassisNo = data.ChassisNo;
                me.detail.EngineNo = data.EngineNo;
                me.detail.ColourCode = data.ColourCode;
                me.detail.ColourName = data.ColourName;
                me.Apply();
            });
        } else {
            MsgBox("Sales Model Code/ Sales Model Year harus di isi!", MSG_ERROR)
        }

    }

    me.Company = function () {
        var lookup = Wx.blookup({
            name: "CompanyAccountBrowse",
            title: "Company Account",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("CompanyAccountBrowse").withParameters({ compTo: "", isCekStatus: true }),
            defaultSort: "CompanyCodeTo asc",
            columns: [
               { field: 'CompanyCodeTo ', title: 'Company Code To' },
               { field: 'BranchCodeTo', title: 'Branch Code To' },
               { field: 'WarehouseCodeTo', title: 'WareHouse Code To' },
               { field: 'WarehouseCodeToDesc', title: 'WareHouse Code To Desc' },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.CompanyCodeFrom = data.CompanyCodeTo;
            me.data.BranchCodeFrom = data.BranchCodeTo;
            me.data.BranchFrom = data.BranchCodeToDesc;
            me.data.WareHouseCodeFrom = data.WarehouseCodeTo;
            me.data.WareHouseFrom = data.WarehouseCodeToDesc;
            me.Apply();
        });
    }

    me.TransferOut = function () {
            var lookup = Wx.blookup({
                name: "TransferOutBrowse",
                title: "Transfer Out",
                manager: spSalesManager,
                query: "Select4OutTrfIn",
                query: new breeze.EntityQuery.from("Select4OutTrfIn").withParameters({ CompanyCodeFrom: me.data.CompanyCodeFrom, BranchCodeFrom: me.data.BranchCodeFrom }),
                defaultSort: "TransferOutNo asc",
                columns: [
                    { field: 'TransferOutNo', title: 'TransferOutNo' },
                    { field: 'TransferOutDate', title: 'TransferOutDate', template: "#= (TransferOutDate == undefined) ? '' : moment(TransferOutDate).format('DD MMM YYYY') #" },
                    { field: 'CompanyFrom', title: 'Company From' },
                    { field: 'BranchFrom', title: 'Branch From' },
                    { field: 'WarehouseFrom', title: 'WareHouse From' },
                    { field: 'CompanyTo', title: 'Company To' },
                    { field: 'BranchTo', title: 'Branch To' },
                    { field: 'BranchToDesc', title: 'Branch To Desc' },
                    { field: 'WarehouseTo', title: 'WareHouse To' },
                    { field: 'WarehouseToDesc', title: 'WareHouse To Desc' }
                ]
            });
            lookup.dblClick(function (data) {
                me.data.TransferOutNo = data.TransferOutNo;
                me.data.CompanyCodeTo = data.CompanyTo;
                me.data.BranchCodeTo = data.BranchTo;
                me.data.BranchTo = data.BranchToDesc;
                me.data.WareHouseCodeTo = data.WarehouseTo;
                me.data.WareHouseTo = data.WarehouseToDesc;
                //$('#btnAddDetail').removeAttr('disabled');
                //me.lookupAfterSelect(data);
                me.Apply();
            });
    }

    me.saveData = function (e, param) {
        $http.post('om.api/TransferInMulti/save', me.data)//{ model: me.data, salesmodelcode: me.detail.SalesModelCode, options: me.options })
        .success(function (data, status, headers, config) {
            if (data.success) {
                $('#TransferInNo').val(data.data.TransferInNo);
                Wx.Success("Data saved...");
                me.startEditing();
                me.enableFiil();
                me.checkStatus(data.data.Status);
            } else {
                MsgBox(data.message, MSG_ERROR);
            }
        }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.AddEditDetail = function () {
        me.linkTransferInNo();
        console.log(me.detail);
        if (me.detail.TransferInNo == '' || !me.detail.TransferInNo) {
            SimDms.Warning("Please fill Transfer In No!");
        } else {
            if (!me.detail.SalesModelCode || !me.detail.SalesModelYear || !me.detail.ChassisNo) {
                SimDms.Warning("Ada Informasi Yang Belum Lengkap!");
            } else {
                $http.post('om.api/TransferInMulti/save2', { model: me.detail, ColourName: me.detail.ColourName, CompanyCodeFrom: me.data.CompanyCodeFrom, BranchCodeFrom: me.data.BranchCodeFrom }).
                    success(function (data, status, headers, config) {
                        if (data.status) {
                            Wx.Success("Update Detail Berhasil");
                            me.startEditing();
                            me.clearTable(me.grid1);
                            me.grid.model = data.data;
                            me.loadTableData(me.grid1, me.grid.model);
                            me.loadDetail(me.detail);
                            me.detail.oid = true;
                            me.isSave = false;
                            me.isPrintAvailable = true;
                            me.isPrintEnable = true;
                            me.CloseModel();
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
        me.data.TransferInNo = $('#TransferInNo').val();
        MsgConfirm("Are you sure to delete current record?", function (e) {
            if (e) {
                $http.post('om.api/TransferInMulti/Delete', { model: me.data, CompanyCodeFrom: me.data.CompanyCodeFrom, BranchCodeFrom: me.data.BranchCodeFrom }).
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
            }
            else {
                return;
            }
        });
    }

    me.delete2 = function () {
        me.detail.TransferInNo = $('#TransferInNo').val();
        MsgConfirm("Are you sure to delete current record?", function (e) {
            if (e) {
                $http.post('om.api/TransferInMulti/Delete2', { model: me.detail, CompanyCodeFrom: me.data.CompanyCodeFrom, BranchCodeFrom: me.data.BranchCodeFrom, TransferOutNo: me.data.TransferOutNo }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        var TransferInNo = me.detail.TransferInNo;
                        me.detail = {};
                        me.clearTable(me.grid1);
                        me.detail.TransferInNo = TransferInNo;
                        me.loadDetail(me.detail);
                        Wx.Success("Data deleted...");
                        me.CloseModel();
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
            else {
                return;
            }
        });
    }

    me.CloseModel = function () {
        me.detail = {};
        me.grid1.clearSelection();
    }

    me.linkTransferInNo = function () {
        me.detail.TransferInNo = $('#TransferInNo').val();
    }

    me.initialize = function () {
        $('#btnApprove').prop('disabled', true);
        $('#btnAddDetail').attr('disabled', true);
        var x = 'TransferInNo,TransferInDate,TransferOutNo,ReferenceNo,ReferenceDate,BranchCodeFrom,WareHouseCodeFrom,BranchCodeTo,WareHouseCodeTo,ReturnDate,Remark,Status';
        var y = x.split(',', 25);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).removeAttr('disabled');
            $('#btn' + y[i]).removeAttr('disabled');
        }
        me.disableFiil();
        me.clearTable(me.grid1);
        me.clearTable(me.gridDetailColour);
        me.detail = {};
        me.data.TransferInDate = me.now();
        me.data.ReferenceDate = me.now();
        me.data.ReturnDate = me.now();
        $('#isC1').prop('checked', true);
        $('#isC2').prop('checked', true);
        $('#Tujuan').css(
       {
           "font-size": "20px",
           "color": "rgb(0, 91, 153)"
       });
        $('#Asal').css(
        {
            "font-size": "20px",
            "color": "rgb(0, 91, 153)"
        });
        $('#statusLbl').text("NEW");
        $('#statusLbl').css(
        {
            "font-size": "34px",
            "color": "red",
            "font-weight": "bold",
            "text-align": "center"
        });
        me.isSave = false;
    }

    me.disableFiil = function () {
        var p = 'SalesModelCode,SalesModelYear,ChassisCode,ChassisNo,EngineCode,EngineNo,ColourCode,StatusTransferOut';
        var q = p.split(',', 25);
        var r = q.length;
        for (i = 0; i <= r; i++) {
            $('#' + q[i]).attr('disabled', true);
            $('#btn' + q[i]).attr('disabled', true);
        }
        $("[name='Remark1']").prop('disabled', true);
       // $('#btnAddDetail').removeAttr('disabled');
    }

    me.enableFiil = function () {
        var p = 'SalesModelCode,SalesModelYear,ChassisCode,ChassisNo,EngineCode,EngineNo,ColourCode,StatusTransferOut';
        var q = p.split(',', 25);
        var r = q.length;
        for (i = 0; i <= r; i++) {
            $('#' + q[i]).removeAttr('disabled');
            $('#btn' + q[i]).removeAttr('disabled');
        }
        $("[name='Remark1']").prop('disabled', false);
        $('#btnAddDetail').removeAttr('disabled');
    }

    $('#isC1').on('change', function (e) {
        if ($('#isC1').prop('checked') == true) {
            me.data.ReferenceDate = me.now();
            $('#ReferenceDate').prop('readonly', false);
        } else {
            me.data.ReferenceDate = undefined;
            $('#ReferenceDate').prop('readonly', true);
        }
        me.Apply();
    })

    $('#isC2').on('change', function (e) {
        if ($('#isC2').prop('checked') == true) {
            me.data.ReturnDate = me.now();
            $('#ReturnDate').prop('readonly', false);
        } else {
            me.data.ReturnDate = undefined;
            $('#ReturnDate').prop('readonly', true);
        }
        me.Apply();
    })

    me.printPreview = function () {
        if (me.data.Status == 'OPEN') {
            $http.post('om.api/TransferInMulti/updateHdr', me.data).
             success(function (data, status, headers, config) {
                 if (data.success) {
                     Wx.Success("Print Berhasil");
                     me.checkStatus(data.data.Status);
                     //me.disableFiil();
                     me.disableHDR(data.data.Status);
                     $('#btnAddDetail').removeAttr('disabled');
                 } else {
                     MsgBox(data.message, MSG_ERROR);
                 }
             }).
             error(function (data, status, headers, config) {
                 MsgBox(data.message, MSG_ERROR);
             });
        }

        var prm = [
            me.data.TransferInNo
        ];
        Wx.showPdfReport({
            id: "OmRpInventTrn002B",
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }

    me.approve = function () {
        $http.post('om.api/TransferInMulti/Approve', { model: me.data, CompanyCodeFrom: me.data.CompanyCodeFrom }).
             success(function (data, status, headers, config) {
                 if (data.success) {
                     me.checkStatus(data.data.Status);
                     me.disableHDR(data.data.Status);
                     me.disableFiil();
                 } else {
                     MsgBox(data.message, MSG_ERROR);
                 }
             }).
             error(function (data, status, headers, config) {
                 MsgBox(data.message, MSG_ERROR);
             });
    }

    me.initGrid = function () {
        me.grid1 = new webix.ui({
            container: "wxbpu",
            view: "wxtable", css:"alternating", scrollX: true,
            columns: [
                { id: "SalesModelCode", header: "Sales Model Code", width: 150 },
                { id: "SalesModelYear", header: "Sales Model Year", width: 150 },
                { id: "SalesModelDesc", header: "Sales Model Desc", width: 200 },
                { id: "ChassisCode", header: "Kode Rangka", width: 150 },
                { id: "ChassisNo", header: "No. Rangka", width: 150 },
                { id: "EngineCode", header: "Kode Mesin", width: 150 },
                { id: "EngineNo", header: "No. Mesin", width: 150 },
                { id: "ColourCode", header: "Kode Warna", width: 150 },
            ],
            on: {
                onSelectChange: function () {
                    if (me.grid1.getSelectedId() !== undefined) {
                        me.detail = this.getItem(me.grid1.getSelectedId().id);
                        me.detail.oid = me.grid1.getSelectedId();
                        console.log(me.data.Status);
                        if (me.data.Status == 'PRINTED' || me.data.Status == 'APPROVED' || me.data.Status == 'CLOSED') {
                            $('#btnUpdateDetail').attr('disabled', true);
                            $('#btnDeleteDetail').attr('disabled', true);
                        }
                        me.Apply();
                    }
                }
            }
        });
    }

    me.initGrid();

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Transfer In Multi Branch",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnloptions",
                items: [

                      {
                          type: "buttons", cls: "span4", items: [
                                 { name: "btnApprove", text: "Approve", cls: "btn btn-info", icon: "icon-ok", click: "approve()", disable: true },
                          ]
                      },
                      { type: "label", name: "statusLbl", text: "NEW", cls: "span3" }
                ]
            },
            {
                name: "pnlPO",
                items: [
                     { name: "TransferInNo", text: "No. Transfer", cls: "span4", readonly: true, placeHolder: 'VTI/XX/YYYYYY' },
                     { name: "TransferInDate", text: "Tgl. Transfer", cls: "span4", type: "ng-datepicker" },
                     { name: "ReferenceNo", text: "No. Ref.", cls: "span4", readonly: false, type: "text" },
                     {
                         text: "Tgl. Ref.",
                         type: "controls",
                         cls: "span4",
                         items: [
                            { name: 'isC1', type: 'check', text: 'Tax Centralized', cls: 'span1', float: 'left' },
                             { name: "ReferenceDate", text: "Reff Date", cls: "span7", type: 'ng-datepicker', disabled: true },
                         ]
                     },
                     { type: "label", text: "Asal", name: "Asal" },
                     { type: "divider" },
                     { name: "CompanyCodeFrom", cls: "span4 full", text: "Company", type: "text", type: "popup", click: "Company()", required: true, validasi: "required" },
                     {
                         text: "Cabang",
                         type: "controls",
                         cls: "span8",
                         required: true,
                         items: [
                             { name: "BranchCodeFrom", cls: "span3", text: "Cabang Asal", type: "text", readonly: true },
                             { name: "BranchFrom", text: "Nama Cabang Asal", cls: "span5", readonly: true },
                         ]
                     },
                     {
                         text: "Gudang",
                         type: "controls",
                         cls: "span8",
                         required: true,
                         items: [
                             { name: "WareHouseCodeFrom", cls: "span3", text: "Gudang Asal", type: "text", readonly: true },
                             { name: "WareHouseFrom", text: "Name Gudang Asal", cls: "span5", readonly: true },
                         ]
                     },
                     { name: "TransferOutNo", cls: "span4 full", text: "No. Transfer Out", type: "text", type: "popup", click: "TransferOut()", required: true, validasi: "required" },
                     { type: "label", text: "Tujuan", name: "Tujuan" },
                     { type: "divider" },
                     { name: "CompanyCodeTo", cls: "span4 full", text: "Company", type: "text", readonly: true },
                     {
                         text: "Cabang",
                         type: "controls",
                         cls: "span8",
                         items: [
                             { name: "BranchCodeTo", cls: "span3", text: "Cabang Tujuan", type: "text", readonly: true },
                             { name: "BranchTo", text: "Nama Cabang Tujuan", cls: "span5", readonly: true },
                         ]
                     },
                     {
                         text: "Gudang",
                         type: "controls",
                         cls: "span8",
                         items: [
                             { name: "WareHouseCodeTo", cls: "span3", text: "Gudang Tujuan", type: "text", readonly: true },
                             { name: "WareHouseTo", text: "Nama Gudang Tujuan", cls: "span5", readonly: true },
                         ]
                     },
                     {
                         text: "Tgl. Return",
                         type: "controls",
                         cls: "span3",
                         items: [
                            { name: 'isC2', type: 'check', text: '', cls: 'span1', float: 'left' },
                             { name: "ReturnDate", text: "Tgl. Return", cls: "span7", type: 'ng-datepicker', disabled: true },
                         ]
                     },
                     { name: "Remark", text: "Keterangan", cls: "span5", readonly: false },

                ]
            },
            {
                name: "pnlBPU",
                title: "Detil Transfer In",
                items: [
                     { name: "SalesModelCode", model: "detail.SalesModelCode", cls: "span4", placeHolder: "Sales Model Code", type: "popup", btnName: "btnPartNo", disabled: true, click: "salesModelCode()", text: "Sales Model Code" },
                     {
                         text: "Code/ Chassis No",
                         type: "controls",
                         cls: "span4",
                         items: [
                            { name: "SalesModelYear", model: "detail.SalesModelYear", cls: "span3", placeHolder: "Sales Model Year", type: "popup", disabled: true, click: "salesModelYear()", text: "Sales Model Year" },
                             { name: "SalesModelDesc", model: "detail.SalesModelDesc", text: "SalesModel Desc", cls: "span5", readonly: true, type: "text" },
                         ]
                     },
                     {
                         text: "Kode/ No. Rangka",
                         type: "controls",
                         cls: "span4",
                         items: [
                            { name: "ChassisCode", model: "detail.ChassisCode", text: "Kode Rangka", cls: "span4", readonly: true },
                             { name: "ChassisNo", model: "detail.ChassisNo", text: "No. Rangka ", cls: "span4", readonly: false, type: "popup", click: "Chassis()" },
                         ]
                     },
                      {
                          text: "Kode/ No. Mesin",
                          type: "controls",
                          cls: "span4",
                          items: [
                             { name: "EngineCode", model: "detail.EngineCode", text: "Kode Mesin", cls: "span4", readonly: true },
                              { name: "EngineNo", model: "detail.EngineNo", text: "No. Mesin", cls: "span4", readonly: true, type: "text" },
                          ]
                      },
                      {
                          text: "Warna",
                          type: "controls",
                          cls: "span8 full",
                          items: [
                             { name: "ColourCode", model: "detail.ColourCode", text: "Colour Code", cls: "span4", readonly: true },
                              { name: "ColourName", model: "detail.ColourName", text: "Colour Name", cls: "span4", readonly: true, type: "text" },
                          ]
                      },
                       { name: "Remark1", model: "detail.Remark", text: "Keterangan", cls: "span8", readonly: false },
                     {
                         type: "buttons",
                         items: [
                                 { name: "btnAddDetail", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddEditDetail()", show: "detail.oid === undefined", disable: true },
                                 { name: "btnUpdateDetail", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddEditDetail()", show: "detail.oid !== undefined" },
                                 { name: "btnDeleteDetail", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete2()", show: "detail.oid !== undefined" },
                                 { name: "btnCancelDetail", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel()", show: "detail.oid !== undefined" }
                         ]
                     },
                ]
            },
             {
                 name: "wxbpu",
                 xtype: "wxtable",
             },




        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("omKaroseriController");
    }

});