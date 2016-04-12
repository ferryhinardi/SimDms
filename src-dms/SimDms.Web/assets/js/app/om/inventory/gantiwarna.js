var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function omKaroseriController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.loadDetail = function (data) {
        $http.post('om.api/GantiWarna/DetailLoad?DocNo=' + data.DocNo).
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

    me.browse = function () {
        me.init();
        var lookup = Wx.blookup({
            name: "ColorChangeLookup",
            title: "ColorChange Browse",
            manager: spSalesManager,
            query: "ColorChanges",
            defaultSort: "DocNo desc",
            columns: [
                { field: "DocNo", title: "No.Dokumen" },
                { field: "DocDate", title: "Tgl", template: "#= (DocDate == undefined) ? '' : moment(DocDate).format('DD MMM YYYY') #" },
                { field: "ReferenceNo", title: "No.Referensi" },
                { field: 'ReferenceDate', title: 'Tgl.Reff', template: "#= (ReferenceDate == undefined) ? '' : moment(ReferenceDate).format('DD MMM YYYY') #" },
                { field: 'Status', title: 'Status' },
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
                me.isSave = false;
                me.isPrintAvailable = true;
                me.isPrintEnable = true;
                me.detail.TransferInNo = data.TransferInNo;
                me.Apply();
            }
        });
    }

    me.disableHDR = function (status) {
        if (status == 'PRINTED' || status == 'CLOSED' || status > 0) {
            if (status == 'PRINTED' || status == 1) {
                $('#btnApprove').prop('disabled', false);
            } else {
                $('#btnApprove').prop('disabled', true);
            }
            $('#btnAddDetail').attr('disabled', true);
            $('#btnPrintPreview').attr('disabled', true);
            $('#btnDelete').attr('disabled', true);
            var x = 'DocNo,DocDate,ReferenceNo,ReferenceDate,Remark,Status,isC1';
            x += 'SalesModelCode,SalesModelYear,ChassisCode,ChassisNo,EngineCode,EngineNo,ColourCode,ColourName';
            var y = x.split(',', 35);
            var z = y.length;
            for (i = 0; i <= z; i++) {
                $('#' + y[i]).attr('disabled', true);
                $('#btn' + y[i]).attr('disabled', true);
            }
            $("[name='Remark']").prop('disabled', true);
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
    }

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
            case 'CLOSED' || '2':
                $('#statusLbl').text("CLOSED");
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

    me.SalesModelCode = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeBrowse",
            title: "Model",
            manager: spSalesManager,
            query: "MstModelColourBrowse",
            defaultSort: "SalesModelCode asc",
            columns: [
               { field: "SalesModelCode", title: "Sales Model Code" },
               { field: "SalesModelDesc", title: "Sales Model Desc" },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail.SalesModelCode = data.SalesModelCode;
            me.Apply();
        });
    }

    me.SalesModelYear = function () {
        var lookup = Wx.blookup({
            name: "SalesModelYearBrowse",
            title: "Model Year",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("SalesModelYear4ColourChange").withParameters({ SalesModelCode: me.detail.SalesModelCode }),
            defaultSort: "SalesModelYear asc",
            columns: [
               { field: 'SalesModelYear', title: 'Sales Model Year' },
               { field: 'SalesModelDesc', title: 'Sales Model Desc' },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail.SalesModelYear = data.SalesModelYear;
            me.detail.ChassisCode = data.ChassisCode;
            me.detail.SalesModelDesc = data.SalesModelDesc;
            me.Apply();
        });
    }

    me.Chassis = function () {
        if (me.detail.ChassisCode) {//| me.detail.ChassisCode != '') {
            var lookup = Wx.blookup({
                name: "SalesModelYearBrowse",
                title: "Model Year",
                manager: spSalesManager,
                query: new breeze.EntityQuery.from("Select4ChassisNo").withParameters({ SalesModelCode: me.detail.SalesModelCode, SalesModelYear: me.detail.SalesModelYear, ChassisCode: me.detail.ChassisCode }),
                defaultSort: "ChassisNo asc",
                columns: [
                   { field: 'ChassisNo', title: 'No.Rangka' },
                   { field: 'EngineCode', title: 'Kode Mesin' },
                   { field: 'EngineNo', title: 'No.Mesin' },
                   { field: 'WarehouseCode', title: 'Kode WH.' },
                   { field: 'WarehouseName', title: 'Nama WH' },
                   { field: 'ColourCode', title: 'Kode Warna' },
                   { field: 'ColourName', title: 'Nama Warna' },
                ]
            });
            lookup.dblClick(function (data) {
                me.detail.ChassisNo = data.ChassisNo;
                me.detail.EngineNo = data.EngineNo;
                me.detail.EngineCode = data.EngineCode;
                me.detail.ColourCodeFrom = data.ColourCode;
                me.detail.ColourNameFrom = data.ColourName;
                me.detail.WareHouseCode = data.WarehouseCode;
                me.detail.WarehouseName = data.WarehouseName;
                me.Apply();
            });
        } else {
            MsgBox("You Must Fill Sales Model Code, Sales Model Year and Chassis Code!", MSG_ERROR)
        }

    }

    me.ColourNew = function () {
        if (me.detail.SalesModelCode) {//| me.detail.ChassisCode != '') {
            var lookup = Wx.blookup({
                name: "SalesModelYearBrowse",
                title: "Model Year",
                manager: spSalesManager,
                query: new breeze.EntityQuery.from("ColourLookup").withParameters({ SalesModelCode: me.detail.SalesModelCode }),
                defaultSort: "ColourCodeNew asc",
                columns: [
                   { field: 'ColourCodeNew', title: 'Colour Code New' },
                   { field: 'ColourNew', title: 'Colour New' },
                ]
            });
            lookup.dblClick(function (data) {
                me.detail.ColourCodeTo = data.ColourCodeNew;
                me.detail.ColourNameTo = data.ColourNew;
                me.Apply();
            });
        } else {
            MsgBox("You Must Fill Sales Model Code, Sales Model Year and Chassis Code!", MSG_ERROR)
        }

    }

    me.saveData = function (e, param) {
        $http.post('om.api/gantiwarna/save', me.data)//{ model: me.data, salesmodelcode: me.detail.SalesModelCode, options: me.options })
        .success(function (data, status, headers, config) {
            if (data.success) {
                $('#DocNo').val(data.data.DocNo);
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
        me.linkDocNo();
        if (me.detail.DocNo == '' || !me.detail.DocNo) {
            SimDms.Warning("Please fill Doc No!");
        } else {
            $http.post('om.api/GantiWarna/save2', me.detail).
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
        me.data.DocNo = $('#DocNo').val();
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('om.api/GantiWarna/Delete', me.data).
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
        me.detail.DocNo = $('#DocNo').val();
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('om.api/GantiWarna/Delete2', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    var DocNo = me.detail.DocNo;
                    me.detail = {};
                    me.clearTable(me.grid1);
                    me.detail.DocNo = DocNo;
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

    me.linkDocNo = function () {
        me.detail.DocNo = $('#DocNo').val();
    }

    me.initialize = function () {
        $('#btnApprove').prop('disabled', true);
        var x = 'DocNo,DocDate,ReferenceNo,ReferenceDate,Remark,Status,isC1';
        var y = x.split(',', 25);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).attr('disabled', false);
            $('#btn' + y[i]).removeAttr('disabled');
        }
        me.disableFiil();
        me.clearTable(me.grid1);
        me.clearTable(me.gridDetailColour);
        me.detail = {};
        me.data.DocDate = me.now();
        me.data.ReferenceDate = me.now();
        $('#isC1').prop('checked', true);
        $('#statusLbl').text("NEW");
        $('#statusLbl').css(
        {
            "font-size": "34px",
            "color": "red",
            "font-weight": "bold",
            "text-align": "center"
        });
    }

    me.disableFiil = function () {
        var p = 'ChassisCode,ChassisNo,EngineCode,EngineNo,SalesModelCode,SalesModelYear,ColourCodeFrom,ColourCodeTo,WarehouseCode,RemarkDtl';
        var q = p.split(',', 25);
        var r = q.length;
        for (i = 0; i <= r; i++) {
            $('#' + q[i]).attr('disabled', true);
            $('#btn' + q[i]).attr('disabled', true);
        }
        $("[name='Remark1']").prop('disabled', true);
        $('#btnAddDetail').attr('disabled', true);
    }

    me.enableFiil = function () {
        var p = 'ChassisCode,ChassisNo,EngineCode,EngineNo,SalesModelCode,SalesModelYear,ColourCodeFrom,ColourCodeTo,WarehouseCode,RemarkDtl';
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

    me.printPreview = function () {
        if (me.data.Status == 'OPEN') {
            $http.post('om.api/gantiwarna/updateHdr', me.data).
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
        //    rparam: "semua",
        //    type: "devex"
        //});
    }

    me.approve = function () {
        $http.post('om.api/gantiwarna/updateHdr', me.data).
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

    me.initGrid = function () {
        me.grid1 = new webix.ui({
            container: "wxbpu",
            view: "wxtable", css:"alternating", scrollX: true,
            columns: [
                { id: "SalesModelCode", header: "Model", width: 150 },
                { id: "SalesModelYear", header: "Year", width: 150 },
                { id: "ChassisCode", header: "Kode Rangka", width: 150 },
                { id: "ChassisNo", header: "No. Rangka", width: 150 },
                { id: "EngineCode", header: "Kode Mesin", width: 150 },
                { id: "EngineNo", header: "No. Mesin", width: 150 },
                { id: "ColourCodeOld", header: "Wrn. Lama", width: 150 },
                { id: "ColourOld", header: "Nama Warna Lama", width: 150 },
                { id: "ColourCodeNew", header: "Wrn. Baru", width: 150 },
                { id: "ColourNew", header: "Nama Warna Baru", width: 150 },
            ],
            on: {
                onSelectChange: function () {
                    if (me.grid1.getSelectedId() !== undefined) {
                        //alert(me.grid1.getSelectedId().id.columns.ColourCode);
                        me.detail = this.getItem(me.grid1.getSelectedId().id);
                        me.detail.oid = me.grid1.getSelectedId();
                        //console.log(this.getItem(me.grid1.getSelectedId().id))
                        if (me.data.Status == 'PRINTED' || me.data.Status == 'CLOSED') {
                            //alert(me.data.Status);
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
        title: "Ganti Warna",
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
                     { name: "DocNo", text: "No. Dokumen", cls: "span4", readonly: true, placeHolder: 'CCO/XX/YYYYYY' },
                     { name: "DocDate", text: "Tgl. Dokumen", cls: "span4", type: "ng-datepicker" },
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
                     { name: "Remark", text: "Keterangan", cls: "span8", readonly: false },

                ]
            },
            {
                name: "pnlBPU",
                title: "Detail",
                items: [
                     { name: "SalesModelCode", model: "detail.SalesModelCode", cls: "span3", placeHolder: "Sales Model Code", type: "popup", btnName: "btnPartNo", disabled: "!datas.isLoad", click: "SalesModelCode()", required: true, validasi: "required", text: "Sales Model Code" },
                     {
                         text: "Year",
                         type: "controls",
                         cls: "span5",
                         items: [
                            { name: "SalesModelYear", model: "detail.SalesModelYear", cls: "span3", placeHolder: "Sales Model Year", type: "popup", btnName: "btnPartNo", disabled: "!datas.isLoad", click: "SalesModelYear()", required: true, validasi: "required", text: "Sales Model Year" },
                            { name: "SalesModelDesc", model: "detail.SalesModelDesc", text: "SalesModel Desc", cls: "span5", readonly: true, type: "text" },
                         ]
                     },
                    
                     { name: "ChassisCode", model: "detail.ChassisCode", text: "Kode Rangka", cls: "span3", readonly: true },
                      {
                          text: "Kode/ No. Mesin",
                          type: "controls",
                          cls: "span5",
                          items: [
                             { name: "EngineCode", model: "detail.EngineCode", text: "Kode Mesin", cls: "span3", readonly: true },
                              { name: "EngineNo", model: "detail.EngineNo", text: "No. Mesin", cls: "span5", readonly: true, type: "text" },
                          ]
                      },
                      { name: "ChassisNo", model: "detail.ChassisNo", text: "No. Rangka ", cls: "span3", readonly: false, type: "popup", click: "Chassis()" },
                      {
                          text: "Gudang",
                          type: "controls",
                          cls: "span5",
                          //required: true,
                          items: [
                              { name: "WareHouseCode", model: "detail.WareHouseCode", cls: "span3", text: "Gudang", type: "text", readonly: true },
                              { name: "WarehouseName", model: "detail.WarehouseName", text: "Name Gudang Asal", cls: "span5", readonly: true },
                          ]
                      },
                      
                      {
                          text: "Warna Lama",
                          type: "controls",
                          cls: "span4",
                          items: [
                             { name: "ColourCodeFrom", model: "detail.ColourCodeFrom", text: "Colour Code", cls: "span3", readonly: true },
                              { name: "ColourNameFrom", model: "detail.ColourNameFrom", text: "Colour Name", cls: "span5", readonly: true, type: "text" },
                          ]
                      },
                      {
                           text: "Warna Baru",
                           type: "controls",
                           cls: "span4",
                           items: [
                              { name: "ColourCodeTo", model: "detail.ColourCodeTo", text: "Colour Code", cls: "span3", type: "popup", click: "ColourNew()" },
                               { name: "ColourNameTo", model: "detail.ColourNameTo", text: "Colour Name", cls: "span5", readonly: true, type: "text" },
                           ]
                       },
                       { name: "RemarkDtl", model: "detail.RemarkDtl", text: "Keterangan", cls: "span8", readonly: false },
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