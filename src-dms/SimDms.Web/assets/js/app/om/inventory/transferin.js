var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function omTransferInController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.loadDetail = function (data) {
        $http.post('om.api/TransferIn/TransferInDetailLoad?TransferInNo=' + data.TransferInNo).
               success(function (data, status, headers, config) {
                   if (data != '') {
                       //alert('112');
                       me.grid.detail = data;
                       me.loadTableData(me.grid1, me.grid.detail);
                       me.isSave = false;
                       me.isPrintAvailable = true;
                       me.isPrintEnable = true;
                   } else {
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
            name: "TransferInLookup",
            title: "Transfer In",
            manager: spSalesManager,
            query: "TransferInBrowse",
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
                me.status = data.Status;
                //me.disableHDR();
                //if (data.Status == 'OPEN') {
                //    $('#btnAddDetail').attr('disabled', false);
                //    $('#btnPrintPreview').removeAttr('disabled');
                //    $('#btnDelete').removeAttr('disabled');
                //    me.enableFiil();
                //}
                me.lookupAfterSelect(data);
                me.checkbox(data);
                me.loadDetail(data);
                me.detail.TransferInNo = data.TransferInNo;
                //me.isPrintAvailable = true
                me.checkStatus(data.Status);
                me.Apply();
            }
        });
    }

    //me.disableHDR = function (status) {
    //    if (status == 'PRINTED' || status == 'TRANSFERED' || status > 0) {
    //        if (status == 'PRINTED' || status == 1) {
    //            $('#btnApprove').prop('disabled', false);
    //        } else {
    //            $('#btnApprove').prop('disabled', true);
    //        }
    //        //$('#btnAddDetail').attr('disabled', true);
    //        //$('#btnPrintPreview').attr('disabled', true);
    //        //$('#btnDelete').attr('disabled', true);
    //        var x = 'TransferInNo,TransferInDate,TransferOutNo,ReferenceNo,ReferenceDate,BranchCodeFrom,WareHouseCodeFrom,BranchCodeTo,WareHouseCodeTo,ReturnDate,Status,isC1,isC2';
    //        x += 'TransferOutNo,SalesModelCode,SalesModelYear,ChassisCode,ChassisNo,EngineCode,EngineNo,ColourCode,ColourName,StatusTransferIn';
    //        var y = x.split(',', 35);
    //        var z = y.length;
    //        for (i = 0; i <= z; i++) {
    //            $('#' + y[i]).attr('disabled', true);
    //            $('#btn' + y[i]).attr('disabled', true);
    //        }
    //        $("[name='Remark']").prop('disabled', true);
    //        $("[name='isC2']").prop('disabled', true);
    //        $("[name='isAll']").prop('disabled', true);
    //    }
    //}

    me.disableHDR = function () {
        var x = 'TransferInNo,TransferInDate,TransferOutNo,ReferenceNo,ReferenceDate,BranchCodeFrom,WareHouseCodeFrom,BranchCodeTo,WareHouseCodeTo,ReturnDate,Status,';
        x += 'TransferOutNo,SalesModelCode,SalesModelYear,ChassisCode,ChassisNo,EngineCode,EngineNo,ColourCode,ColourName,StatusTransferIn';
        var y = x.split(',', 35);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).attr('disabled', true);
            $('#btn' + y[i]).attr('disabled', true);
        }
        $("[name='isC1']").prop('disabled', true);
        $("[name='isC2']").prop('disabled', true);
        $("[name='isAll']").prop('disabled', true);
        $("[name='Remark']").prop('disabled', true);
    }

    me.disableButtonDetail = function () {
        $("[name='btnUpdateDetail']").prop('disabled', true);
        $("[name='btnDeleteDetail']").prop('disabled', true);
        $("[name='btnCancelDetail']").prop('disabled', true);
    }

    me.enableBtDetail = function () {
        $("[name='btnUpdateDetail']").removeAttr('disabled');
        $("[name='btnDeleteDetail']").removeAttr('disabled');
        $("[name='btnCancelDetail']").removeAttr('disabled');
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
        console.log("Status: " + Status)
        switch ((Status)) {
            case 'OPEN':
                $('#statusLbl').text("OPEN");
                me.allowEdit = true;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                me.enableFiil()
                //me.allowEditDetail = true;
                break;
            case 'PRINTED':
                $('#statusLbl').text("PRINTED");
                me.allowEdit = true;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                $('#btnApprove').prop('disabled', false);
                me.enableFiil();
                //me.allowEditDetail = true;
                break;
            case 'APPROVED':
                $('#statusLbl').text("APPROVED");
                me.allowEdit = false;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                $('#btnSave').hide();
                $('#btnDelete').hide();
                $('#btnApprove').prop('disabled', true);
                me.disableHDR();
                me.disableButtonDetail();
                //me.allowEditDetail = false;
                break;
            case 'DELETED':
                $('#statusLbl').text("DELETED");
                me.allowEdit = true;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                $('#btnSave').hide();
                $('#btnDelete').hide();
                $('#btnApprove').prop('disabled', true);
                me.disableHDR();
                me.disableButtonDetail();
                //me.allowEditDetail = false;
                break;
            case 'TRANSFERED':
                $('#statusLbl').text("TRANSFERED");
                me.allowEdit = false;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                $('#btnSave').hide();
                $('#btnDelete').hide();
                $('#btnApprove').prop('disabled', true);
                me.disableHDR();
                me.disableButtonDetail();
                //me.isApprove = true;
                //me.allowEditDetail = false;
                break;
            case 'FINISHED':
                $('#statusLbl').text("FINISHED");
                me.allowEdit = false;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                $('#btnSave').hide();
                $('#btnDelete').hide();
                $('#btnApprove').prop('disabled', true);
                me.disableHDR();
                me.disableButtonDetail();
                break;
            case '0':
                $('#statusLbl').text("OPEN");
                me.allowEdit = true;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                me.enableFiil();
                $('#btnApprove').prop('disabled', true);
                //me.allowEditDetail = true;
                break;
            case '1':
                $('#statusLbl').text("PRINTED");
                me.allowEdit = true;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                me.enableFiil();
                $('#btnApprove').prop('disabled', false);
                //me.allowEditDetail = true;
                break;
            case '2':
                $('#statusLbl').text("APPROVED");
                me.allowEdit = false;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                $('#btnSave').hide();
                $('#btnDelete').hide();
                $('#btnApprove').prop('disabled', true);
                me.disableHDR()
                me.disableButtonDetail()
                //me.allowEditDetail = false;
                break;
            case '3':
                $('#statusLbl').text("DELETED");
                me.allowEdit = true;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                $('#btnSave').hide();
                $('#btnDelete').hide();
                $('#btnApprove').prop('disabled', true);
                me.disableHDR()
                me.disableButtonDetail()
                //me.allowEditDetail = false;
                break;
            case '5':
                $('#statusLbl').text("TRANSFERED");
                me.allowEdit = false;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                $('#btnDelete').hide();
                $('#btnApprove').prop('disabled', true);
                me.disableHDR()
                me.disableButtonDetail()
                //me.allowEditDetail = false;
                break;
            case '9':
                $('#statusLbl').text("FINISHED");
                me.allowEdit = false;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                $('#btnSave').hide();
                $('#btnDelete').hide();
                $('#btnApprove').prop('disabled', true);
                me.disableHDR()
                me.disableButtonDetail()
                break;
        }
    }

    me.salesModelCode = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeBrowse",
            title: "Model",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("Select4ModelByTOut").withParameters({ TransferOutNo: me.data.TransferOutNo }),
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
            query: new breeze.EntityQuery.from("Select4ModelYearByTOut").withParameters({ SalesModelCode: me.detail.SalesModelCode, TransferOutNo: me.data.TransferOutNo }),
            defaultSort: "SalesModelYear asc",
            columns: [
               { field: 'SalesModelYear', title: 'Sales Model Year' },
               { field: 'SalesModelDesc', title: 'Sales Model Desc' },
                //{ field: 'ChassisCode', title: 'ChassisNo' },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail.SalesModelYear = data.SalesModelYear;
            me.detail.SalesModelDesc = data.SalesModelDesc;
            me.detail.ChassisNo = data.ChassisNo;
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
                query: new breeze.EntityQuery.from("Select4ChassisByTOut").withParameters({ SalesModelCode: me.detail.SalesModelCode, SalesModelYear: me.detail.SalesModelYear, ChassisCode: me.detail.ChassisCode, TransferOutNo: me.data.TransferOutNo }),
                defaultSort: "ChassisNo asc",
                columns: [
                   { field: 'ChassisNo', title: 'No. Rangka' },
                   { field: 'EngineNo', title: 'No. Mesin' },
                   { field: 'ColourCode', title: 'Kode Warna' },
                   { field: 'ColourName', title: 'Nama Warna' },
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

    me.TransferOut = function () {
        var lookup = Wx.blookup({
            name: "TransferOutBrowse",
            title: "Transfer Out",
            manager: spSalesManager,
            query: "Select4Out",
            defaultSort: "TransferOutNo asc",
            columns: [
               { field: 'TransferOutNo', title: 'No. Transfer Out' },
               { field: 'TransferOutDate', title: 'Tgl. Transfer Out', template: "#= (TransferOutDate == undefined) ? '' : moment(TransferOutDate).format('DD MMM YYYY') #" },
               { field: 'BranchFrom', title: 'Cabang Asal' },
               { field: 'BranchTo', title: 'Cabang Tujuan' },
               { field: 'WareHouseFrom', title: 'Gudang Asal' },
               { field: 'WareHouseTo', title: 'Gudang Tujuan' },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.TransferOutNo = data.TransferOutNo;
            me.data.BranchCodeFrom = data.BranchCodeFrom;
            me.data.BranchFrom = data.BranchFrom;
            me.data.BranchCodeTo = data.BranchCodeTo;
            me.data.BranchTo = data.BranchTo;
            me.data.WareHouseCodeFrom = data.WareHouseCodeFrom;
            me.data.WareHouseFrom = data.WareHouseFrom;
            me.data.WareHouseCodeTo = data.WareHouseCodeTo;
            me.data.WareHouseTo = data.WareHouseTo;
            //$('#btnAddDetail').removeAttr('disabled');
            //me.lookupAfterSelect(data);
            me.Apply();
        });
    }

    me.saveData = function () {
        var param = $(".main .gl-widget").serializeObject();
        var isAll = (param.isAll === undefined) ? false : true
        $http.post('om.api/TransferIn/save', { model: me.data, isAll: isAll })//{ model: me.data, salesmodelcode: me.detail.SalesModelCode, options: me.options })
        .success(function (data, status, headers, config) {
            if (data.success) {
                $('#TransferInNo').val(data.transferInNo);
                Wx.Success("Data saved...");
                data.TransferInNo = data.transferInNo;
                me.loadDetail(data);
                me.status = 'OPEN';
                me.startEditing();
                me.enableFiil();
                me.checkStatus('OPEN');
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
        if (me.detail.TransferInNo == '' || !me.detail.TransferInNo) {
            SimDms.Warning("Please fill Transfer In No!");
        }
        else if (me.detail.SalesModelCode == undefined || me.detail.SalesModelYear == undefined || me.detail.ChassisNo == undefined
                || me.detail.SalesModelCode == '' || me.detail.SalesModelYear == '' || me.detail.ChassisNo == '') {
            SimDms.Warning("Data Detail masih ada yang kurang");
        }
        else {
            $http.post('om.api/TransferIn/save2', me.detail).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Update Detail Berhasil");
                        me.startEditing();
                        me.clearTable(me.grid1);
                        me.grid.model = data.data;
                        me.loadTableData(me.grid1, me.grid.model);
                        me.detail.oid = true;
                        me.isSave = false;
                        me.isPrintAvailable = true;
                        me.isPrintEnable = true;
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
        }

    };

    me.delete = function () {
        me.data.TransferInNo = $('#TransferInNo').val();
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('om.api/TransferIn/Delete', me.data).
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

    me.delete2 = function () {
        me.detail.TransferInNo = $('#TransferInNo').val();
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('om.api/TransferIn/Delete2', { trOutNo: me.data.TransferOutNo, branchCodeFrom: me.data.BranchCodeFrom, model: me.detail }).
            success(function (data, status, headers, config) {
                if (data.success) {
                    var TransferInNo = me.detail.TransferInNo;
                    me.detail = {};
                    me.clearTable(me.grid1);
                    me.detail.TransferInNo = TransferInNo;
                    me.loadDetail(me.detail);
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
        me.data.TransferInNo = "";

        $('#isC1').prop('checked', true);
        $('#isC2').prop('checked', true);
        $('#statusLbl').text("NEW");
        $('#statusLbl').css(
        {
            "font-size": "34px",
            "color": "red",
            "font-weight": "bold",
            "text-align": "center"
        });
        me.isPrintEnable = true
        me.isPrintAvailable = false;
        me.status = 'NEW';
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
        $('#btnAddDetail').removeAttr('disabled');
    }

    me.enableFiil = function () {
        var p = 'SalesModelCode,SalesModelYear,ChassisCode,ChassisNo,EngineCode,EngineNo,ColourCode,Remark,StatusTransferOut';
        var q = p.split(',', 25);
        var r = q.length;
        for (i = 0; i <= r; i++) {
            $('#' + q[i]).removeAttr('disabled');
            $('#btn' + q[i]).removeAttr('disabled');
        }
        $("[name='Remark1']").prop('disabled', false);
        //$('#btnAddDetail').removeAttr('disabled');
        me.enableBtDetail()
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
        if (me.data.Status !== 'CANCELED') {
            BootstrapDialog.show({
                message: $(
                    '<div class="container">' +
                    '<div class="row">' +
                    '<p class="col-xs-2 control-label"><b>Ukuran Kertas</b></p>' +
                    '<input type="radio" name="sizeType" id="sizeType1" value="half" checked style="cursor: pointer;">&nbsp Print satu halaman &nbsp&nbsp' +
                    '<input type="radio" name="sizeType" id="sizeType2" value="full" style="cursor: pointer;">&nbsp Print setengah halaman</div></div>'),
                closable: false,
                draggable: true,
                type: BootstrapDialog.TYPE_INFO,
                title: 'Print',
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
        }
    }

    me.print = function () {
        var sizeType = $('input[name=sizeType]:checked').val() === 'half';
        var model = angular.copy(me.data);
        var transferInNo = model.TransferInNo == undefined || model.TransferInNo == '' ? $('#TransferInNo').val() : model.TransferInNo;
        model.TransferInNo = transferInNo;
        $http.post('om.api/TransferIn/Print', model).
        success(function (data, status, headers, config) {
            if (data.success) {
                Wx.Success("Print Berhasil");
                me.disableHDR(data.data.Status);
                me.checkStatus(data.data.Status);
                me.disableFiil();

                var prm = [
                    transferInNo
                ];

                if (!sizeType) {
                    Wx.showPdfReport({
                        id: "OmRpInventTrn002",
                        pparam: prm.join(','),
                        rparam: "Print Transfer In",
                        type: "devex"
                    });
                }
                else {
                    Wx.showPdfReport({
                        id: "OmRpInventTrn002A",
                        pparam: prm.join(','),
                        rparam: "Print Transfer In",
                        type: "devex"
                    });
                }
            } else {
                MsgBox(data.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox(data.message, MSG_ERROR);
        });
    }

    me.approve = function () {
        MsgConfirm("Approve Transfer In ?", function (result) {
            if (result) {
                var model = angular.copy(me.data);
                var transferInNo = model.TransferInNo == undefined || model.TransferInNo == '' ? $('#TransferInNo').val() : model.TransferInNo;
                model.TransferInNo = transferInNo;
                $http.post('om.api/TransferIn/Approve', model).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Approve Berhasil");
                        me.checkStatus(data.data.Status);
                        me.disableHDR(data.data.Status);
                        $('#btnApprove').prop('disabled', true);
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox(data.message, MSG_ERROR);
                });
            }
            else {
                return;
            }
        });
    }

    me.initGrid = function () {
        me.grid1 = new webix.ui({
            container: "wxbpu",
            view: "wxtable", css: "alternating", scrollX: true,
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
                        //if (me.data.Status == 'PRINTED' || me.data.Status == 'CLOSED') {
                        //    $('#btnUpdateDetail').attr('disabled', true);
                        //    $('#btnDeleteDetail').attr('disabled', true);
                        //}
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

    me.$watch('isLoadData', function (newData, oldData) {
        console.log(me.status);
        console.log(me.isLoadData);
        if (me.status != "NEW" && me.status != undefined) {
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


    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Transfer In",
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
                     { name: "TransferInNo", model: "data.TransferInNo", text: "No. Transfer", cls: "span4", readonly: true, placeHolder: 'VTI/XX/YYYYYY' },
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
                     { name: "TransferOutNo", cls: "span4 full", text: "No. Transfer Out", type: "text", type: "popup", click: "TransferOut()", required: true, validasi: "required" },

                     {
                         type: "controls",
                         cls: "span3 full",
                         items: [
                               { name: 'isAll', type: 'check', cls: 'span1' },
                               { type: "label", text: 'Pilih Semua', cls: "span7 mylabel" },
                         ]
                     },
                     {
                         text: "Cabang Asal",
                         type: "controls",
                         cls: "span8",
                         required: true,
                         items: [
                             { name: "BranchCodeFrom", cls: "span3", text: "Cabang Asal", type: "text", readonly: true },
                             { name: "BranchFrom", text: "Nama Cabang Asal", cls: "span5", readonly: true },
                         ]
                     },
                     {
                         text: "Gudang Asal",
                         type: "controls",
                         cls: "span8",
                         required: true,
                         items: [
                             { name: "WareHouseCodeFrom", cls: "span3", text: "Gudang Asal", type: "text", readonly: true },
                             { name: "WareHouseFrom", text: "Name Gudang Asal", cls: "span5", readonly: true },
                         ]
                     },
                     {
                         text: "Cabang Tujuan",
                         type: "controls",
                         cls: "span8",
                         required: true,
                         items: [
                             { name: "BranchCodeTo", cls: "span3", text: "Cabang Tujuan", type: "text", readonly: true },
                             { name: "BranchTo", text: "Nama Cabang Tujuan", cls: "span5", readonly: true },
                         ]
                     },
                     {
                         text: "Gudang Tujuan",
                         type: "controls",
                         cls: "span8",
                         required: true,
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
                     { name: "SalesModelCode", model: "detail.SalesModelCode", cls: "span4", placeHolder: "Sales Model Code", type: "popup", btnName: "btnPartNo", disabled: "!datas.isLoad", click: "salesModelCode()", required: true, validasi: "required", text: "Sales Model Code" },
                     {
                         text: "Code/ Chassis No",
                         type: "controls",
                         cls: "span4",
                         items: [
                            { name: "SalesModelYear", model: "detail.SalesModelYear", cls: "span3", placeHolder: "Sales Model Year", type: "popup", disabled: "!datas.isLoad", click: "salesModelYear()", required: true, validasi: "required", text: "Sales Model Year" },
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
                                 { name: "btnUpdateDetail", text: "Save", icon: "icon-save", cls: "btn btn-success", click: "AddEditDetail()" },
                                 { name: "btnDeleteDetail", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete2()", show: "detail.oid !== undefined" },
                                 { name: "btnCancelDetail", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel()", show: "detail.oid !== undefined" }
                         ]
                     },
                ]
            },
             {
                 name: "wxbpu",
                 xtype: "wxtable",
             }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("omTransferInController");
    }

});