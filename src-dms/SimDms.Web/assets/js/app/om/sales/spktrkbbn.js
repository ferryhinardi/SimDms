var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function omKaroseriController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

       me.CType = [
       { "value": '0', "text": 'Leasing' },
       { "value": '1', "text": 'Cabang' },
       { "value": '2', "text": 'Pelanggan' }
       ];


    $('#isC1').on('change', function (e) { //onChange
        if ($('#isC1').prop('checked') == true) {
            me.data.RefferenceDate = me.now();
            $('#RefferenceDate').prop('readonly', false);
        } else {
            me.data.RefferenceDate = undefined;
            $('#RefferenceDate').prop('readonly', true);
        }
        me.Apply();
    })

    $('#isC2').on('change', function (e) { //onChange
        if ($('#isC2').prop('checked') == true) {
            me.detail.STNKInDate = me.now();
            $('#STNKInDate').prop('readonly', false);
        } else {
            me.detail.STNKInDate = undefined;
            $('#STNKInDate').prop('readonly', true);
        }
        me.Apply();
    })

    $('#isC3').on('change', function (e) { //onChange
        if ($('#isC3').prop('checked') == true) {
            me.detail.STNKOutDate = me.now();
            $('#STNKOutDate').prop('readonly', false);
        } else {
            me.detail.STNKOutDate = undefined;
            $('#STNKOutDate').prop('readonly', true);
        }
        me.Apply();
    })

    $('#isC4').on('change', function (e) { //onChange
        if ($('#isC4').prop('checked') == true) {
            me.detail.KIRInDate = me.now();
            $('#KIRInDate').prop('readonly', false);
        } else {
            me.detail.KIRInDate = undefined;
            $('#KIRInDate').prop('readonly', true);
        }
        me.Apply();
    })

    $('#isC5').on('change', function (e) { //onChange
        if ($('#isC5').prop('checked') == true) {
            me.detail.KIROutDate = me.now();
            $('#KIROutDate').prop('readonly', false);
        } else {
            me.detail.KIROutDate = undefined;
            $('#KIROutDate').prop('readonly', true);
        }
        me.Apply();
    })

    $('#isC6').on('change', function (e) { //onChange
        if ($('#isC6').prop('checked') == true) {
            me.detail.BPKBInDate = me.now();
            $('#BPKBInDate').prop('readonly', false);
        } else {
            me.detail.BPKBInDate = undefined;
            $('#BPKBInDate').prop('readonly', true);
        }
        me.Apply();
    }) 

    $('#isC7').on('change', function (e) { //onChange
        if ($('#isC7').prop('checked') == true) {
            me.detail.PoliceRegistrationDate = me.now();
            $('#PoliceRegistrationDate').prop('readonly', false);
        } else {
            me.detail.PoliceRegistrationDate = undefined;
            $('#PoliceRegistrationDate').prop('readonly', true);
        }
        me.Apply();
    }) 

    $('#isC8').on('change', function (e) { //onChange
        if ($('#isC8').prop('checked') == true) {
            me.subdetail.BPKBOutDate = me.now();
            $('#BPKBOutDate').prop('readonly', false);
        } else {
            me.subdetail.BPKBOutDate = undefined;
            $('#BPKBOutDate').prop('readonly', true);
        }
        me.Apply();
    })

    me.loadDetail = function (data) {
        $http.post('om.api/STNKBBN/DetailLoad?SPKNo=' + data.SPKNo).
               success(function (data, status, headers, config) {
                   if (data != '') {
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

    me.loadSubDetail = function (data) {
        $http.post('om.api/STNKBBN/SubDetailLoad?SPKNo=' + data.SPKNo + '&ChassisCode=' + data.ChassisCode + '&ChassisNo=' + data.ChassisNo).
               success(function (data, status, headers, config) {
                   //if (data != '') {
                       me.grid.detail = data;
                       me.loadTableData(me.grid2, me.grid.detail);
                       //me.isSave = false;
                       //me.isPrintAvailable = true;
                       //me.isPrintEnable = true;
                  // } else {
                      // me.isPrintAvailable = false;
                      // me.isPrintEnable = false;
                   //}
               }).
               error(function (e, status, headers, config) {
                   console.log(e);
               });
    }

    me.browse = function () {
        me.init();
        var lookup = Wx.blookup({
            name: "SPKBBNLookup",
            title: "SPKBBN Browse",
            manager: spSalesManager,
            query: "SPKBBN",
            defaultSort: "SPKNo desc",
            columns: [
                { field: "SPKNo", title: "No.SPK" },
                { field: "SPKDate", title: "Tgl.SPK", template: "#= (SPKDate == undefined) ? '' : moment(SPKDate).format('DD MMM YYYY') #" },
                { field: "SupplierCode", title: "Kode Pemasok" },
                { field: 'Status', title: 'Status' },
                { field: 'ChassisCode', title: 'Kode Rangka' },
                { field: 'ChassisNo', title: 'No.Rangka' },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.checkStatus(data.Status);
                me.disableHDR(data.Status);
                if (data.Status == 'OPEN') {
                    $('#btnAddDetail').attr('disabled', false);
                    $('#btnPrintPreview').removeAttr('disabled');
                    $('#btnDelete').removeAttr('disabled');
                    me.enableFiil();
                }
                me.lookupAfterSelect(data);
                me.checkbox(data);
                me.loadDetail(data);
                me.Apply();
            }
        });
    }

    me.supplierBrowse = function () {
        var lookup = Wx.blookup({
            name: "SupplierBrowse",
            title: "Pemasok",
            manager: spSalesManager,
            query: "SupplierCodeLookup",
            defaultSort: "SupplierCode asc",
            columns: [
                { field: "SupplierCode", title: "Kode Pemasok" },
                { field: "SupplierName", title: "Nama Pemasok" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SupplierCode = data.SupplierCode;
                me.data.SupplierName = data.SupplierName;
                me.Apply();
            }
        });
    };

    me.Chassis = function () {
            var lookup = Wx.blookup({
                name: "ChassisCode Lookup",
                title: "ChassisCode Year",
                manager: spSalesManager,
                query: "ChassisCodeSPKBBN",
                defaultSort: "ChassisCode asc",
                columns: [
                   { field: 'ChassisCode', title: 'Kode Rangka' },
                ]
            });
            lookup.dblClick(function (data) {
                me.detail.ChassisCode = data.ChassisCode;
                me.Apply();
            });
    }

    me.ChassisNo = function () {
        var lookup = Wx.blookup({
            name: "ChassisCode Lookup",
            title: "ChassisCode Year",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("ChassisNoSPKBBN").withParameters({ ChassisCode: me.detail.ChassisCode }),
            defaultSort: "ChassisNo asc",
            columns: [
               { field: 'ChassisNo', title: 'Kode Rangka' },
               { field: 'ReqInNo', title: 'No.Permohonan' },
               { field: 'FakturPolisiNo', title: 'Faktur Polisi' },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail.ChassisNo = data.ChassisNo;
            me.detail.ReqInNo = data.ReqInNo;
            me.detail.FakturPolisiNo = data.FakturPolisiNo;
            me.detail.Address = data.Address;
            me.detail.CustomerCode = data.CustomerCode;
            me.detail.CustomerName = data.CustomerName;
            me.detail.Leasing = data.Leasing;
            me.Apply();
        });
    }

    me.CusType = function () {
        var type = me.subdetail.BPKBOutType
        if (type == undefined || type == null || type == '') {
            MsgBox('BPKB diserahkan harus dipilih salah satu', MSG_ERROR);
        }
        else {
            var name = $('#BPKBOutType').select2('data').text;
            var lookup = Wx.blookup({
                name: 'Lookup',
                title: name,
                manager: spSalesManager,
                query: new breeze.EntityQuery.from("SelectBPKB").withParameters({ type: type }),
                defaultSort: "Code asc",
                columns: [
                   { field: 'Code', title: 'Kode' },
                   { field: 'Name', title: 'Nama' },
                ]
            });
            lookup.dblClick(function (data) {
                me.subdetail.BPKBOutBy = data.Code;
                me.subdetail.BPKBOutByName = data.Name;
                me.Apply();
            });
        }
    }

    me.saveData = function (e, param) {
        if (me.data.RefferenceNo != "" && me.data.RefferenceNo != undefined)
            if (me.data.RefferenceDate == undefined)
                {
                MsgBox("Nomor Reff dan Tanggal Reff Harus Diisi", MSG_ERROR);
                return;
            }



        $http.post('om.api/STNKBBN/save', me.data)//{ model: me.data, salesmodelcode: me.detail.SalesModelCode, options: me.options })
        .success(function (data, status, headers, config) {
            if (data.success) {
                $('#SPKNo').val(data.data.SPKNo);
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
        me.linkSPKNo();
        if (me.detail.SPKNo == '' || !me.detail.SPKNo) {
            SimDms.Warning("Please fill Doc No!");
        } else {
            $http.post('om.api/STNKBBN/save2', me.detail).
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

    me.AddEditSubDetail = function () {
        me.linkSPKNo();
        if (me.subdetail.SPKNo == '' || !me.subdetail.SPKNo) {
            SimDms.Warning("Please fill Doc No!");
        } else {
            $http.post('om.api/STNKBBN/save3', me.detail).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Update Detail Berhasil");
                        me.startEditing();
                        me.clearTable(me.grid2);
                        me.grid.model = data.data;
                        me.loadTableData(me.grid2, me.grid.model);
                        me.subdetail.oid = true;
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
        me.data.SPKNo = $('#SPKNo').val();
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('om.api/STNKBBN/Delete', me.data).
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
        me.detail.SPKNo = $('#SPKNo').val();
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('om.api/STNKBBN/Delete2', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    var SPKNo = me.detail.SPKNo;
                    me.detail = {};
                    me.clearTable(me.grid1);
                    me.detail.SPKNo = SPKNo;
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

    me.delete3 = function () {
        me.detail.SPKNo = $('#SPKNo').val();
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('om.api/STNKBBN/Delete3', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    var SPKNo = me.detail.SPKNo;
                    me.subdetail = {};
                    me.clearTable(me.grid2);
                    me.subdetail.SPKNo = SPKNo;
                    me.loadSubDetail(me.subdetail);
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

    me.printPreview = function () {
        if (me.data.Status == 'OPEN') {
            $http.post('om.api/STNKBBN/updateHdr', me.data).
             success(function (data, status, headers, config) {
                 if (data.success) {
                     Wx.Success("Print Berhasil");
                     me.checkStatus(data.data.Status);
                     me.disableFiil();
                     me.disableHDR(data.data.Status);
                 } else {
                     MsgBox(data.message, MSG_ERROR);
                 }
             }).
             error(function (data, status, headers, config) {
                 MsgBox(data.message, MSG_ERROR);
             });
        }

        //var prm = [
        //           // me.data.CompanyCode,
        //            me.data.RefferenceType,
        //            me.data.RefferenceCode,
        //            me.data.RefferenceCodeTo,
        //            me.data.Status
        //];
        //Wx.showPdfReport({
        //    id: "OmRpMst001",
        //    pparam: prm.join(','),
        //    textprint: true,
        //    rparam: "semua",
        //    type: "devex"
        //});
    }

    me.approve = function () {
        $http.post('om.api/STNKBBN/updateHdr', me.data).
             success(function (data, status, headers, config) {
                 if (data.success) {
                     //alert(data.data.Status);
                     me.checkStatus(data.data.Status);
                     me.disableHDR(data.data.Status);
                 } else {
                     MsgBox(data.message, MSG_ERROR);
                 }
             }).
             error(function (data, status, headers, config) {
                 MsgBox(data.message, MSG_ERROR);
             });
    }

    me.CloseModel = function () {
        me.detail = {};
        me.grid1.clearSelection();
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
            var x = 'SPKNo,SPKDate,RefferenceNo,RefferenceDate,SupplierCode,Remark,Status,isC1';
            var y = x.split(',', 35);
            var z = y.length;
            for (i = 0; i <= z; i++) {
                $('#' + y[i]).attr('disabled', true);
                $('#btn' + y[i]).attr('disabled', true);
            }
        }
    }

    me.enableFiil = function () {
        var p = 'ChassisCode,ChassisNo,ReqInNo,FakturPolisiNo,PoliceRegistrationNo,PoliceRegistrationDate,STNKInDate,STNKInBy,STNKOutDate,STNKOutBy,BPKBInDate,BPKBInBy,BPKBOutDate,BPKBOutBy,KIRInDate,KIRInBy,KIROutDate,KIROutBy,RemarkDtl,BPKBNo,isC2,isC3,isC4,isC5,isC6';
        var q = p.split(',', 30);
        var r = q.length;
        for (i = 0; i <= r; i++) {
            $('#' + q[i]).removeAttr('disabled');
            $('#btn' + q[i]).removeAttr('disabled');
        }
        $("[name='Remark1']").prop('disabled', false);
        $('#btnAddDetail').removeAttr('disabled');
    }

    me.disableFiil = function () {
        var p = 'ChassisCode,ChassisNo,ReqInNo,FakturPolisiNo,PoliceRegistrationNo,PoliceRegistrationDate,STNKInDate,STNKInBy,STNKOutDate,STNKOutBy,BPKBInDate,BPKBInBy,BPKBOutDate,BPKBOutBy,KIRInDate,KIRInBy,KIROutDate,KIROutBy,RemarkDtl,BPKBNo,isC2,isC3,isC4,isC5,isC6';
        var q = p.split(',', 25);
        var r = q.length;
        for (i = 0; i <= r; i++) {
            $('#' + q[i]).attr('disabled', true);
            $('#btn' + q[i]).attr('disabled', true);
        }
        $("[name='RemarkDtl']").prop('disabled', true);
        $('#btnAddDetail').attr('disabled', true);
    }

    me.initialize = function () {
        $('#btnApprove').prop('disabled', true);
        var x = 'SPKNo,SPKDate,RefferenceNo,RefferenceDate,SupplierCode,Remark,Status,isC1';
        var y = x.split(',', 25);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).attr('disabled', false);
            $('#btn' + y[i]).removeAttr('disabled');
        }
        me.disableFiil();
        me.clearTable(me.grid1);
        me.clearTable(me.grid2);
        me.detail = {};
        me.subdetail = {};
        me.data.SPKDate = me.now();
        $('#isC1').prop('checked', false);
        $('#statusLbl').text("NEW");
        $('#statusLbl').css(
        {
            "font-size": "34px",
            "color": "red",
            "font-weight": "bold",
            "text-align": "center"
        });
    }

    me.initGrid = function () {
        me.grid1 = new webix.ui({
            container: "wxbpu",
            view: "wxtable", css:"alternating", scrollX: true,
            columns: [
                { id: "ChassisCode", header: "Kode Rangka", width: 200 },
                { id: "ChassisNo", header: "No. Rangka", width: 200 },
                { id: "ReqInNo", header: "Request Faktur", width: 200 },
                { id: "FakturPolisiNo", header: "Faktur Polisi", width: 200 },
                { id: "Remark", header: "Keterangan", width: 300 },
            ],
            on: {
                onSelectChange: function () {
                    if (me.grid1.getSelectedId() !== undefined) {
                        me.detail = this.getItem(me.grid1.getSelectedId().id);
                        me.detail.oid = me.grid1.getSelectedId();
                        console.log(this.getItem(me.grid1.getSelectedId().id));
                        me.checkboxDTL(me.detail);
                        $('#ChassisCode').attr('disabled', true);
                        $('#ChassisNo').attr('disabled', true);
                        if (me.data.Status == 'PRINTED' || me.data.Status == 'APPROVED') {
                            $('#btnUpdateDetail').attr('disabled', true);
                            $('#btnDeleteDetail').attr('disabled', true);
                        }
                        me.loadSubDetail(me.detail);
                        me.Apply();
                    }
                }
            }
        });
    }

    me.grid2 = new webix.ui({
        container: "wxSubDetail",
        view: "wxtable", css:"alternating", scrollX: true,
        columns: [
            { id: "tipe", header: "Diserahkan Ke", width: 200 },
            { id: "Nama", header: "Nama", width: 200 },
            { id: "BPKBOutDateDesc", header: "Tgl.Serah", width: 200 },
        ],
        on: {
            onSelectChange: function () {
                if (me.grid2.getSelectedId() !== undefined) {
                    me.subdetail = this.getItem(me.grid2.getSelectedId().id);
                    me.subdetail.oid = me.grid2.getSelectedId();
                    //console.log(this.getItem(me.grid2.getSelectedId().id));
                    me.checkboxSDTL(me.subdetail);
                    if (me.data.Status == 'PRINTED' || me.data.Status == 'APPROVED') {
                        $('#btnUpdateSubDetail').attr('disabled', true);
                        $('#btnDeleteSubDetail').attr('disabled', true);
                    }
                    me.Apply();
                }
            }
        }
    });
    me.initGrid();

    me.checkStatus = function (Status) {
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
            case 'APPROVED' || '2':
                $('#statusLbl').text("APPROVED");
                me.allowEdit = false;
                //me.allowEditDetail = false;
                break;
            case 'DELETED' || '3':
                $('#statusLbl').text("DELETED");
                me.allowEdit = true;
                //me.allowEditDetail = false;
                break;
            case 'TRANSFERED' || '5':
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

    me.checkbox = function (data) { //onload Detail or header
        if ((data.RefferenceDate).substring(0, 4) != "1900") {
            $('#isC1').prop('checked', true);
            $('#RefferenceDate').prop('readonly', false);
            //alert((data.RefferenceFakturPajakDate).substring(0, 4));
        } else {
            $('#isC1').prop('checked', false);
            $('#RefferenceDate').prop('readonly', true);
            me.data.RefferenceDate = undefined;
        }
    }

    me.checkboxDTL = function (data) { //onload Detail or header
        if ((data.STNKInDate).substring(0, 4) != "1900") {
            $('#isC2').prop('checked', true);
            $('#STNKInDate').prop('readonly', false);
            //alert((data.RefferenceFakturPajakDate).substring(0, 4));
        } else {
            $('#isC2').prop('checked', false);
            $('#STNKInDate').prop('readonly', true);
            me.detail.STNKInDate = undefined;
        }

        if ((data.STNKOutDate).substring(0, 4) != "1900") {
            $('#isC3').prop('checked', true);
            $('#STNKOutDate').prop('readonly', false);
            //alert((data.RefferenceFakturPajakDate).substring(0, 4));
        } else {
            $('#isC3').prop('checked', false);
            $('#STNKOutDate').prop('readonly', true);
            me.detail.STNKOutDate = undefined;
        }

        if ((data.KIRInDate || '1900').substring(0, 4) != "1900") {
            $('#isC4').prop('checked', true);
            $('#KIRInDate').prop('readonly', false);
            //alert((data.KIRInDate).substring(0, 4));
        } else {
            $('#isC4').prop('checked', false);
            $('#KIRInDate').prop('readonly', true);
            me.detail.KIRInDate = undefined;
        }

        if ((data.KIROutDate || '1900').substring(0, 4) != "1900") {
            $('#isC5').prop('checked', true);
            $('#KIROutDate').prop('readonly', false);
            //alert((data.RefferenceFakturPajakDate).substring(0, 4));
        } else {
            $('#isC5').prop('checked', false);
            $('#KIROutDate').prop('readonly', true);
            me.detail.KIROutDate = undefined;
        }

        if ((data.BPKBInDate || '1900').substring(0, 4) != "1900") {
            $('#isC6').prop('checked', true);
            $('#BPKBInDate').prop('readonly', false);
            //alert((data.RefferenceFakturPajakDate).substring(0, 4));
        } else {
            $('#isC6').prop('checked', false);
            $('#BPKBInDate').prop('readonly', true);
            me.detail.BPKBInDate = undefined;
        }

        if ((data.PoliceRegistrationDate).substring(0, 4) != "1900") {
            $('#isC7').prop('checked', true);
            $('#PoliceRegistrationDate').prop('readonly', false);
            //alert((data.RefferenceFakturPajakDate).substring(0, 4));
        } else {
            $('#isC7').prop('checked', false);
            $('#PoliceRegistrationDate').prop('readonly', true);
            me.detail.PoliceRegistrationDate = undefined;
        }
    }

    me.checkboxSDTL = function (data) { //onload Detail or header
        if ((data.BPKBOutDate).substring(0, 4) != "1900") {
            $('#isC8').prop('checked', true);
            $('#BPKBOutDate').prop('readonly', false);
            //alert((data.RefferenceFakturPajakDate).substring(0, 4));
        } else {
            $('#isC8').prop('checked', false);
            $('#BPKBOutDate').prop('readonly', true);
            me.subdetail.BPKBOutDate = undefined;
        }
    }

    me.linkSPKNo = function () {
        me.detail.SPKNo = $('#SPKNo').val();
    }

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "SPK & Tracking BBN",
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
                     { name: "SPKNo", text: "No. SPK", cls: "span4", readonly: true, placeHolder: 'SPF/XX/YYYYYY' },
                     { name: "SPKDate", text: "Tgl. SPK", cls: "span4", type: "ng-datepicker" },
                     { name: "RefferenceNo", text: "No. Ref.", cls: "span4", readonly: false, type: "text" },
                     {
                         text: "Tgl. Ref.",
                         type: "controls",
                         cls: "span4",
                         items: [
                            { name: 'isC1', type: 'check', text: 'Tax Centralized', cls: 'span1', float: 'left' },
                             { name: "RefferenceDate", text: "Reff Date", cls: "span7", type: 'ng-datepicker', disabled: true },
                         ]
                     },
                     {
                         text: "Pemasok",
                         type: "controls",
                         cls: "span8",
                         required: true,
                         items: [
                             { name: "SupplierCode", cls: "span3", text: "Supplier Code", type: "text", type: "popup", click: "supplierBrowse()", required: true, validasi: "required" },
                             { name: "SupplierName", text: "Supplier Name", cls: "span5", readonly: true },
                         ]
                     },
                     { name: "Remark", text: "Keterangan", cls: "span8", readonly: false },

                ]
            },
            {
                name: "Detail",
                title: "Detail",
                items: [
                      { name: "ChassisCode", model: "detail.ChassisCode", text: "Kode Rangka", cls: "span3", readonly: false, type: "popup", click: "Chassis()" },
                      { name: "ChassisNo", model: "detail.ChassisNo", text: "No.Rangka ", cls: "span2", readonly: false, type: "popup", click: "ChassisNo()" },
                      { name: "ReqInNo", model: "detail.ReqInNo", cls: "span3", text: "Request Faktur", type: "text", readonly: true },
                      {
                          text: "Pelanggan",
                          type: "controls",
                          cls: "span5",
                          //required: true,
                          items: [
                              { name: "CustomerCode", model: "detail.CustomerCode", cls: "span2", text: "Kode Pelanggan", type: "text", readonly: true },
                              { name: "CustomerName", model: "detail.CustomerName", text: "Nama Pelanggan", cls: "span6", readonly: true },
                          ]
                      },
                      { name: "Leasing", model: "detail.Leasing", cls: "span3", text: "Leasing", type: "text", readonly: true },
                      { name: "Address", model: "detail.Address", cls: "span8", text: "Alamat", type: "text", readonly: true },
                      { name: "FakturPolisiNo", model: "detail.FakturPolisiNo", cls: "span3", text: "Faktur Polisi", type: "text", readonly: true },
                      { name: "PoliceRegistrationNo", model: "detail.PoliceRegistrationNo", cls: "span2", text: "No.Polisi", type: "text" },
                      {
                          text: "Tgl.No.Polisi",
                          type: "controls",
                          cls: "span3",
                          items: [
                             { name: 'isC7', type: 'check', text: 'Tax Centralized', cls: 'span1', float: 'left' },
                             { name: "PoliceRegistrationDate", model: "detail.PoliceRegistrationDate", text: "Tgl.No.Polisi", cls: "span7", type: 'ng-datepicker', disabled: true },
                          ]
                      },
                      
                      { name: "STNKInBy", model: "detail.STNKInBy", cls: "span4", text: "STNK Diterima Oleh" },
                      {
                          text: "STNK Diterima",
                          type: "controls",
                          cls: "span4",
                          items: [
                             { name: 'isC2', type: 'check', text: 'Tax Centralized', cls: 'span1', float: 'left' },
                             { name: "STNKInDate", model: "detail.STNKInDate", text: "Reff Date", cls: "span7", type: 'ng-datepicker', disabled: true },
                          ]
                      },
                      { name: "STNKOutBy", model: "detail.STNKOutBy", cls: "span4", text: "STNK Diserahkan Ke", type: "text" },
                      {
                          text: "STNK Diserahkan",
                          type: "controls",
                          cls: "span4",
                          items: [
                             { name: 'isC3', type: 'check', text: 'Tax Centralized', cls: 'span1', float: 'left' },
                             { name: "STNKOutDate", model: "detail.STNKOutDate", text: "Reff Date", cls: "span7", type: 'ng-datepicker', disabled: true },
                          ]
                      },
                      { name: "KIRInBy", model: "detail.KIRInBy", cls: "span4", text: "KIR Diterima Oleh", type: "text" },
                       {
                           text: "KIR Diterima",
                           type: "controls",
                           cls: "span4",
                           items: [
                              { name: 'isC4', type: 'check', text: 'Tax Centralized', cls: 'span1', float: 'left' },
                              { name: "KIRInDate", model: "detail.KIRInDate", text: "Reff Date", cls: "span7", type: 'ng-datepicker', disabled: true },
                           ]
                       },
                       { name: "KIROutBy", model: "detail.KIROutBy", cls: "span4", text: "KIR Diserahkan Ke", type: "text" },
                       {
                            text: "KIR Diserahkan",
                            type: "controls",
                            cls: "span4",
                            items: [
                               { name: 'isC5', type: 'check', text: 'Tax Centralized', cls: 'span1', float: 'left' },
                               { name: "KIROutDate", model: "detail.KIROutDate", text: "Reff Date", cls: "span7", type: 'ng-datepicker', disabled: true },
                            ]
                       },
                       { name: "BPKBNo", model: "detail.BPKBNo", cls: "span4 full", text: "No.BPKB", type: "text" },
                       { name: "BPKBInBy", model: "detail.BPKBInBy", cls: "span4", text: "BPKB Diterima Oleh", type: "text" },
                       {
                             text: "BPKB Diterima",
                             type: "controls",
                             cls: "span4",
                             items: [
                                { name: 'isC6', type: 'check', text: 'Tax Centralized', cls: 'span1', float: 'left' },
                                { name: "BPKBInDate", model: "detail.BPKBInDate", text: "Reff Date", cls: "span7", type: 'ng-datepicker', disabled: true },
                             ]
                       },
                          
                       { name: "RemarkDtl", model: "detail.Remark", text: "Keterangan", cls: "span8", readonly: false },
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
              {
                  name: "pnlSubDetail",
                  title: "Sub Detail",
                  items: [
                        
                        {
                            text: "BPKB Diserahkan",
                            type: "controls",
                            cls: "span4",
                            items: [
                               { name: 'isC8', type: 'check', text: 'Tax Centralized', cls: 'span1', float: 'left' },
                               { name: "BPKBOutDate", model: "subdetail.BPKBOutDate", text: "Tgl.No.Polisi", cls: "span7", type: 'ng-datepicker', disabled: true },
                            ]
                        },
                        { name: "BPKBOutType", model: "subdetail.BPKBOutType", cls: "span4", text: "BPKB Diserahkan Ke", type: "select2", datasource: "CType" },
                        {
                            text: "",
                            type: "controls",
                            cls: "span8",
                            items: [
                                { name: "BPKBOutBy", model: "subdetail.BPKBOutBy", cls: "span3", text: "", type: "text", readonly: false, type: "popup", click: "CusType()"},
                                { name: "BPKBOutByName", model: "subdetail.BPKBOutByName", text: "", cls: "span5", readonly: true },
                            ]
                        },
                        
                         {
                             type: "buttons",
                             items: [
                                     { name: "btnAddSubDetail", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddEditSubDetail()", show: "subdetail.oid === undefined", disable: true },
                                     { name: "btnUpdateSubDetail", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddEditSubDetail()", show: "subdetail.oid !== undefined" },
                                     { name: "btnDeleteSubDetail", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete3()", show: "subdetail.oid !== undefined" },
                                     { name: "btnCancelSubDetail", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "SubCloseModel()", show: "subdetail.oid !== undefined" }
                             ]
                         },
                  ]
              },
             {
                 name: "wxSubDetail",
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