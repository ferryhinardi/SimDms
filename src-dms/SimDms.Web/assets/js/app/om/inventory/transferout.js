var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function omKaroseriController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.loadDetail = function (data) {
        $http.post('om.api/TransferOut/TransferOutDetailLoad?TransferOutNo=' + data.TransferOutNo).
               success(function (data, status, headers, config) {
                   me.grid.detail = data;
                   me.loadTableData(me.grid1, me.grid.detail);
               }).
               error(function (e, status, headers, config) {
                   //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   console.log(e);
               });
    }

    me.browse = function () {
        me.init();
        var lookup = Wx.blookup({
            name: "TransferOutLookup",
            title: "TransferOut Browse",
            manager: spSalesManager,
            query: "TranferoutLookup",
            defaultSort: "TransferOutNo desc",
            columns: [
                { field: "TransferOutNo", title: "No. Transfer" },
                { field: "TransferOutDate", title: "Tanggal", template: "#= (TransferOutDate == undefined) ? '' : moment(TransferOutDate).format('DD MMM YYYY') #" },
                { field: "ReferenceNo", title: "Referensi" },
                { field: 'BranchFrom', title: 'Cabang Asal' },
                { field: 'BrancTo', title: 'Cabang Tujuan' },
                { field: 'WareHouseFrom', title: 'Gudang Asal' },
                { field: "WareHouseTo", title: "Gudang Tujuan" },
                { field: 'ReturnDate', title: 'Tgl. Rtr', template: "#= (ReturnDate == undefined) ? '' : moment(ReturnDate).format('DD MMM YYYY') #" },
                { field: "Remark", title: "Keterangan" },
                { field: 'Status', title: 'Status' },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.status = data.Status
                me.checkStatus(data.Status);
                //me.disableHDR(data.Status);
                //if (data.Status == 'OPEN') {
                //    $('#btnAddDetail').attr('disabled', false);
                //    $('#btnPrintPreview').removeAttr('disabled');
                //    $('#btnDelete').removeAttr('disabled');
                //    me.enableFiil();
                //}
                //if (data.Status == 'TRANSFERED' || data.Status == 'DELETED') {
                //    $('#btnDelete').attr('disabled', true);
                //    if (data.Status == 'DELETED')
                //        me.isPrintEnable = false;
                //}
                //me.lookupAfterSelect(data);
                me.data = data;
                me.checkbox(data);
                me.loadDetail(data);
                //me.isSave = false;
                //me.isPrintEnable = false;
                me.isLoadData = true;
                me.isPrintAvailable = true;
                me.detail.TransferOutNo = data.TransferOutNo;
                //createCookie('TransferOutNo', data.TransferOutNo, 1);
                me.Apply();
               }
        });
    }

    //me.disableHDR = function (status) {
    //    if (status == 'PRINTED' || status == 'APPROVED' || status > 0) {
    //        if (status == 'PRINTED' || status == 1) {
    //            $('#btnApprove').prop('disabled', false);
    //        } else {
    //            $('#btnApprove').prop('disabled', true);
    //        }
    //        $('#btnAddDetail').attr('disabled', true);
    //        //$('#btnPrintPreview').attr('disabled', true);
    //        $('#btnDelete').attr('disabled', true);
    //        var x = 'TransferOutNo,TransferOutDate,ReferenceNo,ReferenceDate,BranchCodeFrom,WareHouseCodeFrom,BranchCodeTo,WareHouseCodeTo,ReturnDate,Remark,Status,isC1,isC2';
    //        var y = x.split(',', 25);
    //        var z = y.length;
    //        for (i = 0; i <= z; i++) {
    //            $('#' + y[i]).attr('disabled', true);
    //            $('#btn' + y[i]).attr('disabled', true);
    //        }
    //    }
    //}

    me.disableHDR = function (status) {
            var x = 'TransferOutNo,TransferOutDate,ReferenceNo,ReferenceDate,BranchCodeFrom,WareHouseCodeFrom,BranchCodeTo,WareHouseCodeTo,ReturnDate,Remark,Status,isC1,isC2';
            var y = x.split(',', 25);
            var z = y.length;
            for (i = 0; i <= z; i++) {
                $('#' + y[i]).attr('disabled', true);
                $('#btn' + y[i]).attr('disabled', true);
            }
            //$('#btnApprove').attr('disabled', true)
    }

    me.disableBtDetail = function () {
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
        } else {
            $('#isC1').prop('checked', false);
            $('#ReferenceDate').prop('readonly', true);
            me.data.ReferenceDate = undefined;
        }

        //if (data.Total != '0') {
        //    $('#isC2').prop('checked', true);
        //    $('#Total').prop('readonly', false);
        //    //alert((data.RefferenceFakturPajakDate).substring(0, 4));
        //} else {
        //    $('#isC2').prop('checked', false);
        //    $('#Total').prop('readonly', true);
        //    me.data.Total = undefined;
        //}
    }

    me.checkStatus = function (Status) {
        Status = Status.split(' ').join('');
        switch (Status) {
            case 'OPEN':
                $('#statusLbl').text("OPEN");
                me.allowEdit = true;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                me.enableFiil();
                break;
            case 'PRINTED':
                $('#statusLbl').text("PRINTED");
                me.allowEdit = true;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                $('#btnApprove').prop('disabled', false);
                me.enableFiil();
                break;
            case 'APPROVED':
                $('#statusLbl').text("APPROVED");
                me.allowEdit = false;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                $('#btnSave').hide();
                $('#btnDelete').hide();
                $('#btnApprove').prop('disabled', true);
                me.disableHDR()
                me.disableBtDetail();
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
                me.disableBtDetail();
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
                me.disableBtDetail();
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
                me.disableBtDetail();
                break;
            case '0':
                $('#statusLbl').text("OPEN");
                me.allowEdit = true;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                me.enableFiil();
                $('#btnApprove').prop('disabled', true);
                break;
            case '1':
                $('#statusLbl').text("PRINTED");
                me.allowEdit = true;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                me.Apply();
                $('#btnApprove').prop('disabled', false);
                me.enableFiil();
                break;
            case '2':
                $('#statusLbl').text("APPROVED");
                me.allowEdit = false;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                $('#btnSave').hide();
                $('#btnDelete').hide();
                $('#btnApprove').prop('disabled', true);
                me.disableHDR();
                me.disableBtDetail();
                break;
            case '3':
                $('#statusLbl').text("DELETED");
                me.allowEdit = true;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                $('#btnSave').hide();
                $('#btnDelete').hide();
                $('#btnApprove').prop('disabled', true);
                me.disableHDR();
                me.disableBtDetail();
                break;
            case '5':
                $('#statusLbl').text("TRANSFERED");
                me.allowEdit = false;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                $('#btnSave').hide();
                $('#btnDelete').hide();
                $('#btnApprove').prop('disabled', true);
                me.disableHDR();
                me.disableBtDetail();
                break;
            case '9':
                $('#statusLbl').text("FINISHED");
                me.allowEdit = false;
                me.isPrintAvailable = true;
                me.isLoadData = true;
                $('#btnSave').hide();
                $('#btnDelete').hide();
                $('#btnApprove').prop('disabled', true);
                me.disableHDR();
                me.disableBtDetail();
                break;
        }
    }

    me.BranchTo = function () {
        var lookup = Wx.blookup({
            name: "BranchToBrowse",
            title: "Model",
            manager: spSalesManager,
            query: "BranchToLookup",
            defaultSort: "BranchCode asc",
            columns: [
               { field: "BranchCode", title: "Kode Cabang" },
               { field: "BranchName", title: "Nama Cabang" },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.BranchCodeTo = data.BranchCode;
            me.data.BranchTo = data.BranchName;
            me.WHNew(data.BranchCode);
            me.Apply();
        });
    }

    me.salesModelCode = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeBrowse",
            title: "Model",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("Select4Model").withParameters({ WarehouseCode: me.data.WareHouseCodeFrom }),
            defaultSort: "SalesModelCode asc",
            columns: [
               { field: "SalesModelCode", title: "Sales Model Code" },
               { field: "SalesModelDesc", title: "Sales Model Desc" },
               { field: 'EngineCode', title: 'Kode Mesin' },
               { field: 'Qty', title: 'Quantity' },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail = {};
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
            query: new breeze.EntityQuery.from("Select4ModelYear").withParameters({ SalesModelCode: me.detail.SalesModelCode, WarehouseCode: me.data.WareHouseCodeFrom }), //Select4ModelYear
            defaultSort: "SalesModelYear asc",
            columns: [
               { field: 'SalesModelYear', title: 'Year' },
                { field: 'SalesModelCode', title: 'S. Model Code' },
               { field: 'SalesModelDesc', title: 'S. Model Desc' },
               { field: 'ChassisCode', title: 'Kode Mesin' },
               { field: 'Qty', title: 'Quantity' },
                //{ field: 'ChassisCode', title: 'ChassisNo' },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail.SalesModelYear = data.SalesModelYear;
            me.detail.SalesModelDesc = data.SalesModelDesc;
            me.detail.ChassisCode = data.ChassisCode;
            me.Apply();
        });
    }

    me.WHNew = function (data) {
        $http.post('om.api/TransferOut/Select4WH?branchCode=' + data)
            .success(function (data, status, headers, config) {
                //alert(data.data.LookUpValue);
                me.data.WareHouseCodeTo = data.data.LookUpValue;
                me.data.WareHouseTo = data.data.LookUpValueName;
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.WHNewLookup = function (x)
    {
        if (x == "1") {
            var branch = me.data.BranchCodeFrom;
        }else {
            var branch = me.data.BranchCodeTo;
        }

        var lookup = Wx.blookup({
                name: "SelectWHBrowse",
                title: "Gudang",
                manager: spSalesManager,
                query:  new breeze.EntityQuery.from("Select4WH").withParameters({ BranchCode: branch }),
                defaultSort: "LookUpValue asc",
                columns: [
                    { field: 'LookUpValue', title: 'Kode Gudang' },
                    { field: 'LookUpValueName', title: 'Nama Gudang' },
                ]
            });
            lookup.dblClick(function (data) {
                //alert(data.LookUpValue);
                if (x == "1") {
                    me.data.WareHouseCodeFrom = data.LookUpValue;
                    me.data.WareHouseFrom = data.LookUpValueName;
                } else {
                    me.data.WareHouseCodeTo = data.LookUpValue;
                    me.data.WareHouseTo = data.LookUpValueName;
                }
                me.Apply();
            });
    }

    me.Chassis = function () {
        if (me.detail.SalesModelCode && me.detail.SalesModelYear) {//| me.detail.ChassisCode != '') {
            var lookup = Wx.blookup({
                name: "SalesModelYearBrowse",
                title: "Model Year",
                manager: spSalesManager,
                query: new breeze.EntityQuery.from("Select4Chassis").withParameters({ SalesModelCode: me.detail.SalesModelCode, SalesModelYear: me.detail.SalesModelYear, ChassisCode: me.detail.ChassisCode, WareHouseCode: me.data.WareHouseCodeFrom }),
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
                me.detail.ColourCode= data.ColourCode;
                me.detail.ColourName = data.ColourName;
                me.Apply();
            });
        } else {
            MsgBox("Sales Model Code/ Sales Model Year harus di isi!", MSG_ERROR)
        }

    }

    me.saveData = function (e, param) {
        if (me.data.BranchCodeFrom == me.data.BranchCodeTo && me.data.WareHouseCodeFrom == me.data.WareHouseCodeTo) {
            MsgBox("Kode Cabang/Gudang asal dan tujuan tidak boleh sama!", MSG_ERROR);
    }else{
        $http.post('om.api/TransferOut/save', me.data)//{ model: me.data, salesmodelcode: me.detail.SalesModelCode, options: me.options })
           .success(function (data, status, headers, config) {
               if (data.success) {
                   //me.data.TransferOutNo = data.data.TransferOutNo;
                   $('#TransferOutNo').val(data.data.TransferOutNo);
                   me.data = data.data;
                   me.data.Status = "OPEN";
                   me.status = 'OPEN'
                   Wx.Success("Data saved...");
                   me.startEditing();
                   me.enableFiil();
                   me.checkStatus(data.data.Status);
                   me.Apply();
               } else {
                   MsgBox(data.message, MSG_ERROR);
               }
           }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        }
    };

    me.AddEditDetail = function () {
        console.log(me.detail.SalesModelCode);
        me.linkTransferOutNo();
        if (me.detail.TransferOutNo == '' || !me.detail.TransferOutNo) {
            SimDms.Warning("Please fill Transfer Out No!");
        }
        else if (me.detail.SalesModelCode == undefined || me.detail.SalesModelYear == undefined || me.detail.ChassisNo == undefined
            || me.detail.SalesModelCode == '' || me.detail.SalesModelYear == '' || me.detail.ChassisNo == '')
        {
            SimDms.Warning("Data Detail masih ada yang kurang");
        }
        else {
            //me.linkKaroseriSPKNo();

            $http.post('om.api/TransferOut/save2', me.detail).
                success(function (data, status, headers, config) {
                    if (data.status) {
                        Wx.Success("Save Berhasil");
                        me.startEditing();
                        me.clearTable(me.grid1);
                        me.grid.model = data.data;
                        me.loadTableData(me.grid1, me.grid.model);
                        me.detail.oid = true;
                        me.isSave = false;
                        me.isPrintAvailable = true;
                        me.isPrintEnable = true;
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

    me.delete = function () {
        me.data.TransferOutNo = $('#TransferOutNo').val();
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('om.api/TransferOut/Delete', me.data).
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
        me.detail.TransferOutNo = me.data.TransferOutNo;
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('om.api/TransferOut/Delete2', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    var TransferOutNo = me.detail.TransferOutNo;
                    me.detail = {};
                    me.clearTable(me.grid1);
                    me.detail.TransferOutNo = TransferOutNo;
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

    me.linkTransferOutNo = function () {
        me.detail.TransferOutNo = $('#TransferOutNo').val();
    }

    me.default = function () {
        $http.post('om.api/TransferOut/TransferOutDefault', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    //alert(data[0].BranchCodeFrom);
                    me.data.BranchCodeFrom = data.data.BranchCodeFrom;
                    me.data.BranchFrom = data.data.BranchNameFrom;
                    me.data.WareHouseCodeFrom = data.data.WareHouseCodeFrom;
                    me.data.WareHouseFrom = data.data.WareHouseNameFrom;
    
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.initialize = function () {
        me.default();
        //$('#btnApprove').prop('disabled', true);
        $('#btnApprove').attr('disabled', true)
        $('#btnAddDetail').attr('disabled', true);
        var x = 'TransferOutNo,TransferOutDate,ReferenceNo,ReferenceDate,BranchCodeFrom,WareHouseCodeFrom,BranchCodeTo,WareHouseCodeTo,ReturnDate,Remark,Status,isC1,isC2';
        var y = x.split(',', 25);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).attr('disabled', false);
            $('#btn' + y[i]).attr('disabled', false);
        }
        me.disableFiil();
        me.clearTable(me.grid1);
        me.detail = {};
        me.data.TransferOutDate = me.now();
        me.data.ReferenceDate = me.now();
        me.data.ReturnDate = me.now();
        me.data.TransferOutNo = '';

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
        me.status = 'NEW'
    }

    me.disableFiil = function () {
        var p = 'SalesModelCode,SalesModelYear,ChassisCode,ChassisNo,EngineCode,EngineNo,ColourCode,Remark,StatusTransferIn';
        var q = p.split(',', 25);
        var r = q.length;
        for (i = 0; i <= r; i++) {
            $('#' + q[i]).attr('disabled', true);
            $('#btn' + q[i]).attr('disabled', true);
        }
        $("[name='Remark']").prop('disabled', true);
        $('#btnAddDetail').removeAttr('disabled');
    }

    me.enableFiil = function () {
        var p = 'SalesModelCode,SalesModelYear,ChassisCode,ChassisNo,EngineCode,EngineNo,ColourCode,Remark,StatusTransferIn';
        var q = p.split(',', 25);
        var r = q.length;
        for (i = 0; i <= r; i++) {
            $('#' + q[i]).removeAttr('disabled');
            $('#btn' + q[i]).removeAttr('disabled');
        }
        $("[name='Remark']").prop('disabled', false);
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
        if (me.data.Status != 'CANCELED') {
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
        var transferOutNo = model.TransferOutNo == undefined || model.TransferOutNo == '' ? $('#TransferOutNo').val() : model.TransferOutNo;
        model.TransferOutNo = transferOutNo;
        $http.post('om.api/TransferOut/Print', model).
        success(function (data, status, headers, config) {
            if (data.success) {
                console.log(data.data.Status);
                Wx.Success("Print Berhasil");
                me.disableHDR(data.data.Status);
                me.checkStatus(data.data.Status);
                me.disableFiil();

                var prm = [
                    transferOutNo
                ];

                if (!sizeType) {
                    Wx.showPdfReport({
                        id: "OmRpInventTrn001",
                        pparam: prm.join(','),
                        rparam: "Print Transfer Out",
                        type: "devex"
                    });
                }
                else {
                    Wx.showPdfReport({
                        id: "OmRpInventTrn001A",
                        pparam: prm.join(','),
                        rparam: "Print Transfer Out",
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
        MsgConfirm("Approve Transfer Out ?", function (result) {
            if (result) {
                var model = angular.copy(me.data);
                var transferOutNo = model.TransferOutNo == undefined || model.TransferOutNo == '' ? $('#TransferOutNo').val() : model.TransferOutNo;
                model.TransferOutNo = transferOutNo;
                $http.post('om.api/TransferOut/updateHdr', model).
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
        title: "Transfer Out",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnloptions",
                items: [

                      {
                          type: "buttons", cls: "span4", items: [
                                 { name: "btnApprove", text: "Approve", cls: "btn btn-info", icon: "icon-ok", click: "approve()"}, //disable: true },
                          ]
                      },
                      { type: "label", name: "statusLbl", text: "NEW", cls: "span3" }
                ]
            },
            {
                name: "pnlPO",
                items: [
                     { name: "TransferOutNo", model: "data.TransferOutNo", text: "No. Transfer", cls: "span4", readonly: true, placeHolder: 'VTO/XX/YYYYYY' },
                     { name: "TransferOutDate", text: "Tgl. Transfer", cls: "span4", type: "ng-datepicker" },
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
                             { name: "WareHouseCodeFrom", cls: "span3", text: "Gudang Asal", type: "text", type: "popup", click: "WHNewLookup(1)", required: true, validasi: "required" },
                             { name: "WareHouseFrom", text: "Name Gudang Asal", cls: "span5", readonly: true },
                         ]
                     },
                     {
                         text: "Cabang Tujuan",
                         type: "controls",
                         cls: "span8",
                         required: true,
                         items: [
                             { name: "BranchCodeTo", cls: "span3", text: "Cabang Tujuan", type: "text", type: "popup", click: "BranchTo()", required: true, validasi: "required" },
                             { name: "BranchTo", text: "Nama Cabang Tujuan", cls: "span5", readonly: true },
                         ]
                     },
                     {
                         text: "Gudang Tujuan",
                         type: "controls",
                         cls: "span8",
                         required: true,
                         items: [
                             { name: "WareHouseCodeTo", cls: "span3", text: "Gudang Tujuan", type: "text", type: "popup", click: "WHNewLookup(2)", required: true, validasi: "required" },
                             { name: "WareHouseTo", text: "Nama Gudang Tujuan", cls: "span5", readonly: true },
                         ]
                     },
                     {
                         text: "Tgl. Return.",
                         type: "controls",
                         cls: "span3",
                         items: [
                            { name: 'isC2', type: 'check', text: '', cls: 'span1', float: 'left' },
                             { name: "ReturnDate", text: "Tgl. Return", cls: "span7", type: 'ng-datepicker', disable: true }
                         ]
                     },
                     
                     { name: "Remark", text: "Keterangan", cls: "span5", readonly: false },

                ]
            },
            {
                name: "pnlBPU",
                title: "Detil Transfer Out",
                items: [
                    { name: "TransferOutSeq", model: "detail.TransferOutSeq", cls: "", type: "hidden", text: "" },
                     { name: "SalesModelCode", model: "detail.SalesModelCode", cls: "span4", placeHolder: "Sales Model Code", type: "popup", disable: true, click: "salesModelCode()", required: true, validasi: "required", text: "Sales Model Code" },
                     {
                         text: "Sales Model Year",
                         type: "controls",
                         cls: "span4",
                         items: [
                            { name: "SalesModelYear", model: "detail.SalesModelYear", cls: "span3", placeHolder: "Sales Model Year", type: "popup", disable: true, click: "salesModelYear()", required: true, validasi: "required", text: "Sales Model Year" },
                            { name: "SalesModelDesc", model: "detail.SalesModelDesc", text: "SalesModel Desc", cls: "span5", readonly: true, type: "text" },
                         ]
                     },
                     {
                         text: "Kode/ No. Rangka",
                         type: "controls",
                         cls: "span4",
                         items: [
                            { name: "ChassisCode", model: "detail.ChassisCode", text: "Kode Rangka", cls: "span4", readonly: true },
                             { name: "ChassisNo", model: "detail.ChassisNo", text: "No. Rangka ", cls: "span4", type: "popup", click: "Chassis()", disable: true },
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
                       { name: "Remark", model: "detail.Remark", text: "Remark", cls: "span8", disable: true, readonly: false },
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
        SimDms.Angular("omKaroseriController");
    }

});