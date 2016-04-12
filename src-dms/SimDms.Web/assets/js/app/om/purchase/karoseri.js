var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function omKaroseriController($scope, $http, $injector, blockUI) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.WareHouseCode = function () {
        var lookup = Wx.blookup({
            name: "WarehouseCodeLookup",
            title: "Lookup Warehouse",
            manager: spSalesManager,
            query: "WareHouse",
            defaultSort: "seqno asc",
            columns: [
                { field: "LookUpValue", title: "Warehouse Code" },
                { field: "LookUpValueName", title: "Warehouse Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.WareHouseCode = data.LookUpValue;
                me.data.WareHouseName = data.LookUpValueName;
                me.Apply();
            }
        });
    };

    me.loadDetail = function (data) {
        $http.post('om.api/Karoseri/KaroseriDetailLoad?KaroseriSPKNo=' + data.KaroseriSPKNo).
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
            name: "KaroseriLookup",
            title: "Karoseri Browse",
            manager: spSalesManager,
            query: "KaroseriBrowses",
            defaultSort: "KaroseriSPKNo desc",
            columns: [
                { field: "KaroseriSPKNo", title: "Karoseri SPK No" },
                { field: "KaroseriSPKDate", title: "Karoseri SPK Date", template: "#= (KaroseriSPKDate == undefined) ? '' : moment(KaroseriSPKDate).format('DD MMM YYYY') #" },
                { field: "RefferenceNo", title: "Refference No" },
                { field: 'SupplierCode', title: 'Supplier Code' },
                { field: 'SupplierName', title: 'Supplier Name' },
                { field: 'SalesModelCodeOld', title: 'Sales Model Old' },
                { field: "SalesModelDesc", title: "Sales Model Desc." },
                { field: "Remark", title: "Remark" },
                { field: 'Status', title: 'Status' },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.getKaroseri(data);
            }
        });
    }

    me.getKaroseri = function (data) {
        //alert(data.Status);
        me.checkStatus(data.Status);
        if (data.Status == 'PRINT' || data.Status == 'CLOSE') {
            if (data.Status == 'PRINT') {
                $('#btnApprove').prop('disabled', false);
            } else {
                $('#btnApprove').prop('disabled', true);
            }

            //var x = localStorage.getItem('MyData');
            var x = 'KaroseriSPKNo,KaroseriSPKDate,RefferenceNo,RefferenceDate,SupplierCode,SalesModelCodeOld,SalesModelYear,SalesModelCodeNew,ChassisCode,Quantity,DPPMaterial,DPPFee,DPPOthers,PPn,Total,DurationDays,Remark,Status,isC1,isC2';
            var y = x.split(',', 25);
            var z = y.length;
            for (i = 0; i <= z; i++) {
                $('#' + y[i]).attr('disabled', true);
            }
        }

        me.lookupAfterSelect(data);

        console.log(data.WareHouseCode);
        me.data.WareHouseCode = data.WareHouseCode;
        me.data.WareHouseName = data.WareHouseName;
        me.detail.ChassisCode = data.ChassisCode;
        console.log(data.Total);
        //me.checkbox(data);
        // $('#btnAddBPU').prop('disabled', false);
        // me.switchBPU = '4';
        me.loadDetail(data);

        //me.isSave = false;

        me.isLoadData = true;
        me.isPrintAvailable = true;
        //me.isPrintEnable = true;
        me.Apply();

        //alert(data.ChassisCode);

        //$('#RefferenceFakturPajakDate').prop('readonly', true);
        // $("[name='RefferenceFakturPajakDate']").prop('disabled', true);
    }

    me.checkbox = function (data) {
        if ((data.RefferenceDate).substring(0, 4) != "1900") {
            $('#isC1').prop('checked', true);
            $('#RefferenceDate').prop('readonly', false);
            //alert((data.RefferenceFakturPajakDate).substring(0, 4));
        } else {
            $('#isC1').prop('checked', false);
            $('#RefferenceDate').prop('readonly', true);
            me.data.RefferenceDate = undefined;
        }

        if (data.Total != '0') {
            $('#isC2').prop('checked', true);
            $('#Total').prop('readonly', false);
            me.data.DPPMaterial = 0;
            //alert((data.RefferenceFakturPajakDate).substring(0, 4));
        } else {
            $('#isC2').prop('checked', false);
            $('#Total').prop('readonly', true);
            me.data.Total = undefined;
        }
    }

    me.checkStatus = function (Status) {
        switch ((Status)) {
            case 'OPEN' || '0':
                $('#statusLbl').text("OPEN");
                me.allowEdit = true;
                //me.allowEditDetail = true;
                break;
            case 'PRINT'||'1':
                $('#statusLbl').text("PRINTED");
                me.allowEdit = true;
                //me.allowEditDetail = true;
                break;
            case 'CLOSE'||'2':
                $('#statusLbl').text("CLOSED");
                me.allowEdit = false;
                //me.allowEditDetail = false;
                break;
            case 'CANCEL'||'3':
                $('#statusLbl').text("CANCELED");
                me.allowEdit = true;
                //me.allowEditDetail = false;
                break;
            //case 5:
            //    $('#statusLbl').text("POSTED");
            //    me.allowEdit = false;
            //    //me.allowEditDetail = false;
            //    break;
            case '9':
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
                $('#statusLbl').text("CLOSED");
                me.allowEdit = false;
                //me.allowEditDetail = false;
                break;
            case '3':
                $('#statusLbl').text("CANCELED");
                me.allowEdit = true;
                //me.allowEditDetail = false;
                break;
        }
    }

    me.supplier = function () {
        var lookup = Wx.blookup({
            name: "SupplierBrowse",
            title: "Pemasok",
            manager: spSalesManager,
            query: "supplier4Karoseri",
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

    me.salesModelCodeOld = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeBrowse",
            title: "Model",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("SalesModelCodeOld").withParameters({ supplierCode: me.data.SupplierCode }),
            defaultSort: "SalesModelCodeOld asc",
            columns: [
               { field: "SalesModelCodeOld", title: "Sales Model Code" },
               { field: "SalesModelDesc", title: "Sales Model Desc" },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.SalesModelCodeOld = data.SalesModelCodeOld;
            me.data.SalesModelNameOld = data.SalesModelDesc;
            me.data.SalesModelCodeNew = data.SalesModelCodeNew;
            me.data.PPn = data.PPn;
            me.data.DPPOthers = data.DPPOthers;
            me.data.DPPFee = data.DPPFee;
            me.data.DPPMaterial = data.DPPMaterial;
            me.data.Total = data.Total;
           // me.lookupAfterSelect(data);
            me.Apply();
        });
    }

    me.salesModelCodeNew = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeBrowse",
            title: "Model",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("SalesModelCodeBrowse").withParameters({ supplierCode: me.data.SupplierCode }),
            defaultSort: "SalesModelCode asc",
            columns: [
               { field: "SalesModelCode", title: "Sales Model Code" },
               { field: "SalesModelDesc", title: "Sales Model Desc" },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.SalesModelCodeNew = data.SalesModelCode;
            me.Apply();
        });
    }

    me.salesModelYear = function () {
        var lookup = Wx.blookup({
            name: "SalesModelYearBrowse",
            title: "Model Year",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("SalesModelYear").withParameters({ SalesModelCode: me.data.SalesModelCodeOld }),
            defaultSort: "SalesModelYear asc",
            columns: [
               { field: 'SalesModelYear', title: 'Sales Model Year' },
               { field: 'SalesModelDesc', title: 'Sales Model Desc' },
                //{ field: 'ChassisCode', title: 'ChassisNo' },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.SalesModelYear = data.SalesModelYear;
            me.data.ChassisCode = data.ChassisCode;
            me.detail.ChassisCode = data.ChassisCode;
            me.Apply();
        });
    }

    me.chassisno = function () {
        if (me.detail.ChassisCode){ //| me.detail.ChassisCode != '') {
            var lookup = Wx.blookup({
                name: "SalesModelYearBrowse",
                title: "Model Year",
                manager: spSalesManager,
                query: new breeze.EntityQuery.from("ChssisNoLoad").withParameters({ SalesModelCode: me.data.SalesModelCodeOld, SalesModelYear: me.data.SalesModelYear, ChassisCode: me.detail.ChassisCode }),
                defaultSort: "ChassisNo asc",
                columns: [
                   { field: 'ChassisNo', title: 'Chassis No.' }
                    //{ field: 'ChassisCode', title: 'ChassisNo' },
                ]
            });
            lookup.dblClick(function (data) {
                me.detail.ChassisNo = data.ChassisNo;
                me.detail.EngineNo = data.EngineNo;
                me.detail.EngineCode = data.EngineCode;
                me.detail.ColourCodeOld = data.ColourCodeOld;
                me.detail.ColourOld = data.ColourOld;
                $('#btnAddDetail').removeAttr('disabled');
                //me.lookupAfterSelect(data);
                me.Apply();
            });
        } else {
            MsgBox("You Must Fill Sales Model Code, Sales Model Year and Chassis Code!", MSG_ERROR)
        }
       
    }

    me.colourcode = function () {
        if (me.data.SalesModelCodeOld) {//| me.detail.ChassisCode != '') {
            var lookup = Wx.blookup({
                name: "SalesModelYearBrowse",
                title: "Model Year",
                manager: spSalesManager,
                query: new breeze.EntityQuery.from("ColourLookup").withParameters({ SalesModelCode: me.data.SalesModelCodeNew }),
                defaultSort: "ColourCodeNew asc",
                columns: [
                   { field: 'ColourCodeNew', title: 'Colour Code New' },
                   { field: 'ColourNew', title: 'Colour New' },
                ]
            });
            lookup.dblClick(function (data) {
                me.detail.ColourCodeNew = data.ColourCodeNew;
                me.detail.ColourNew = data.ColourNew;
                me.Apply();
            });
        } else {
            MsgBox("You Must Fill Sales Model Code, Sales Model Year and Chassis Code!", MSG_ERROR)
        }

    }

    me.saveData = function (e, param) {
        $http.post('om.api/Karoseri/save', me.data)//{ model: me.data, salesmodelcode: me.detail.SalesModelCode, options: me.options })
        .success(function (data, status, headers, config) {
            if (data.success) {
                me.data.KaroseriSPKNo = data.data.KaroseriSPKNo;
               
                Wx.Success("Data saved...");
                me.startEditing();
            } else {
                MsgBox(data.message, MSG_ERROR);
            }
        }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.AddEditDetail = function () {
        //alert(me.detail.TrainingCode);
        me.linkKaroseriSPKNo();
        if (me.detail.KaroseriSPKNo == '' || !me.detail.KaroseriSPKNo) {
            SimDms.Warning("Please fill Karoseri SPK No!");
        } else {
            //me.linkKaroseriSPKNo();

            $http.post('om.api/Karoseri/save2', me.detail).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success(data.message);
                        me.startEditing();
                        me.clearTable(me.grid1);
                        me.grid.model = data.data;
                        me.grid.model.ColourNew = data.colorNew;
                        me.loadTableData(me.grid1, me.grid.model);
                        me.detail.oid = true;
                        me.CloseModel();
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
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('om.api/Karoseri/Delete', me.data).
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
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('om.api/Karoseri/Delete2', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    //me.detail.ChassisNo = undefined;
                    //me.detail.EngineNo = undefined;
                    //me.detail.EngineCode = undefined;
                    //me.detail.ColourCodeOld = undefined;
                    //me.detail.ColourOld = undefined;
                    //me.detail.ColourCodeNew = undefined;
                    //me.detail.ColourNew = undefined;
                    //me.detail.Remark = undefined;
                    var ChassisCode = me.detail.ChassisCode;
                    me.detail = {};
                    me.clearTable(me.grid1);
                    me.detail.ChassisCode = ChassisCode;
                    $('#btnAddDetail').attr('disabled', true);
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
        
        $http.post('om.api/Karoseri/GetPurchaseKaroseri?KaroseriSPKNo=' + $('#KaroseriSPKNo').val()).
               success(function (data, status, headers, config) {
                   console.log(data.ChassisCode);
                   $('#ChassisCode').val(data.data.ChassisCode);
               });

        //$('#ChassisNo').val('');
        //$('#EngineCode').val('');
        //$('#EngineNo').val('');
        //$('#ColourCodeOld').val('');
        //$('#ColourOld').val('');
        //$('#ColourCodeNew').val('');
        //$('#ColourNew').val('');
        //$('#Remark').val('');
        me.grid1.clearSelection();
    }

    me.linkKaroseriSPKNo = function () {
        me.detail.KaroseriSPKNo = me.data.KaroseriSPKNo;
    }

    me.initialize = function () {
    $http.post('om.api/Karoseri/CheckValidasiHolding').
        success(function (data, status, headers, config) {
            if (data.success) {
                $('#btnApprove').prop('disabled', true);
                $('#btnAddDetail').removeAttr('disabled');
                //var x = localStorage.getItem('MyData');
                var x = 'KaroseriSPKNo,KaroseriSPKDate,RefferenceNo,RefferenceDate,SupplierCode,SalesModelCodeOld,SalesModelYear,SalesModelCodeNew,ChassisCode,Quantity,Total,DurationDays,Remark,Status,isC1,isC2';
                var y = x.split(',', 25);
                var z = y.length;
                for (i = 0; i <= z; i++) {
                    $('#' + y[i]).attr('disabled', false);
                }
                var m = 'Quantity,DPPMaterial,DPPFee,DPPOthers,PPn,Total';
                var n = m.split(',', 25);
                var o = n.length;
                for (i = 0; i <= o; i++) {
                    $('#' + n[i]).val('0');
                }
                me.disableFiil();
                me.clearTable(me.grid1);
                me.clearTable(me.gridDetailColour);
                me.colour = {};
                me.detail = {};
                me.data.KaroseriSPKDate = me.now();
                me.data.RefferenceDate = me.now();

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
            }
            else {
                Wx.alert(data.message);
                $('.toolbar,.body').css({
                    "pointerEvents": "none",
                    "opacity": "0.5",
                    "background": "#CCC"
                });
                return;
            }
        });
    }

    me.disableFiil = function () {
        var p = 'DPPMaterial,DPPFee,DPPOthers,PPn';
        var q = p.split(',', 25);
        var r = q.length;
        for (i = 0; i <= r; i++) {
            $('#' + q[i]).attr('disabled', true);
        }
    }

    me.enableFiil = function () {
        var p = 'DPPMaterial,DPPFee,DPPOthers,PPn';
        var q = p.split(',', 25);
        var r = q.length;
        for (i = 0; i <= r; i++) {
            $('#' + q[i]).removeAttr('disabled');
            $('#' + q[i]).val('0');
        }
    }

    me.Tambah = function () {
        var a = $('#DPPMaterial').val();
        var b = $('#DPPFee').val();
        var c = $('#DPPOthers').val();
        a = a.split(',').join('');
        b = b.split(',').join('');
        c = c.split(',').join('');
        var d = (parseFloat(a) * 10) / 100;
        //var c = parseFloat(a) - parseFloat(b);
        var e = parseInt(a) + parseInt(b) + parseInt(c) + parseInt(d);
        $('#PPn').val(d);
        $('#Total').val(e);
    }

    $('#Total').on('blur', function (e) {
        $http.post('om.api/Karoseri/ReCalculateDPPnPPN', me.data)
            .success(function (data) {
                me.data.DPPMaterial = data.data.DPPMaterial;
                me.data.PPn = data.data.PPn;
            });
    })
    $('#DPPMaterial').on('blur', function (e) {
        me.Tambah();
    })
    $('#DPPFee').on('blur', function (e) {
        me.Tambah();
    })
    $('#DPPOthers').on('blur', function (e) {
        me.Tambah();
    })
    $('#isC1').on('change', function (e) {
        if ($('#isC1').prop('checked') == true) {
            me.data.RefferenceDate = me.now();
            $('#RefferenceDate').prop('readonly', false);
        } else {
            me.data.RefferenceDate = undefined;
            $('#RefferenceDate').prop('readonly', true);
        }
        me.Apply();
    })
    $('#isC2').on('change', function (e) {
        me.data.DPPFee = me.data.DPPMaterial = me.data.DPPOthers = me.data.PPn = 0;
        $('#PPn').val(0);
        if ($('#isC2').prop('checked') == true) {
            me.data.Total = '0';
            $('#Total').prop('readonly', false);
            me.disableFiil();
        } else {
            me.data.Total = undefined;
            $('#Total').prop('readonly', true);
            me.enableFiil();

        }
        me.Apply();
    })

    me.printPreview = function () {
        //if (me.data.Status == 'OPEN') {
            $http.post('om.api/karoseri/printKaroseri', me.data).
             success(function (data, status, headers, config) {
                 if (data.success) {
                     var ReportId = 'OmRpPurTrn009';
                     var par = me.data.KaroseriSPKNo + ',' + me.data.KaroseriSPKNo;
                     var rparam = 'Print SPK - Karoseri'

                     Wx.showPdfReport({
                         id: ReportId,
                         pparam: par,
                         rparam: rparam,
                         type: "devex"
                     });

                     me.checkStatus(data.data.Status);
                     $('#btnApprove').removeAttr('disabled');
                    
                 } else {
                     MsgBox(data.message, MSG_ERROR);
                 }
             }).
             error(function (data, status, headers, config) {
                 //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                 MsgBox(data.message, MSG_ERROR);
             });
        //}

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
        $http.post('om.api/karoseri/updateHdr', me.data).
             success(function (data, status, headers, config) {
                 if (data.success) {
                     //alert(data.data.Status);
                     me.checkStatus(data.data.Status);
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
                { id: "ChassisNo", header: "Chassis No.", width: 200 },
                { id: "EngineNo", header: "Engine No.", width: 200 },
                { id: "ColourCodeNew", header: "Colour Code New", width: 200 },
                { id: "ColourNew", header: "Colour Name New", width: 250 },
                { id: "Remark", header: "Remark", width: 250 },
            ],
            on: {
                onSelectChange: function () {
                    if (me.grid1.getSelectedId() !== undefined) {
                        //alert(me.grid1.getSelectedId().id.columns.ColourCode);
                        me.detail = this.getItem(me.grid1.getSelectedId().id);
                        me.detail.oid = me.grid1.getSelectedId();
                        //console.log(this.getItem(me.grid1.getSelectedId().id))
                        if (me.data.Status == 'PRINT' || me.data.Status == 'CLOSE') {
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
        title: "Karoseri",
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
                     { name: "KaroseriSPKNo", text: "Karoseri SPK No.", cls: "span4", readonly: true, placeHolder: 'KRI/XX/YYYYYY' },
                     { name: "KaroseriSPKDate", text: "Karoseri SPK Date", cls: "span4", type: "ng-datepicker" },
                     { name: "RefferenceNo", text: "Reff. No. ", cls: "span4", readonly: false, type: "text" },
                     {
                        text: "Reff Date",
                        type: "controls",
                        cls: "span4",
                        items: [
                           { name: 'isC1', type: 'check', text: 'Tax Centralized', cls: 'span1', float: 'left' },
                            { name: "RefferenceDate", text: "Reff Date", cls: "span7", type: 'ng-datepicker', disabled: true },
                        ]
                     },
                     {
                         text: "Supplier",
                         type: "controls",
                         cls: "span8",
                          required: true, 
                         items: [
                             { name: "SupplierCode", cls: "span3", text: "Supplier Code", type: "text", type: "popup", click: "supplier()", required: true, validasi: "required" },
                             { name: "SupplierName", text: "Supplier Name", cls: "span5", readonly: true },
                         ]
                     },
                     { name: "SalesModelCodeOld", model: "data.SalesModelCodeOld", cls: "span4 ", placeHolder: "Sales Model Old", type: "popup", btnName: "btnPartNo", disabled: "!datas.isLoad", click: "salesModelCodeOld()", required: true, validasi: "required", text: "Sales Model Code Old" },
                     { name: "SalesModelYear", model: "data.SalesModelYear", cls: "span4", placeHolder: "Sales Model Year", type: "popup", btnName: "btnPartNo", disabled: "!datas.isLoad", click: "salesModelYear()", required: true, validasi: "required", text: "Sales Model Year" },
                     { name: "SalesModelCodeNew", model: "data.SalesModelCodeNew", cls: "span4 ", placeHolder: "Sales Model New", type: "popup", btnName: "btnPartNo", readonly: true,  click: "salesModelCodeNew()", text: "Sales Model Code New"}, //disabled: "!datas.isLoad", validasi: "required", required: true },
                     { name: "DurationDays", text: "Duration(Hari)", cls: "span4 number-int", readonly: false, required: true, validasi: "required" },
                     {
                         text: "Total",
                         type: "controls",
                         cls: "span4",
                         items: [
                            { name: 'isC2', type: 'check', text: 'Tax Centralized', cls: 'span1', float: 'left' },
                            { name: "Total", model: "data.Total", text: "Total", cls: "span5 full number-int", disable: "!data.isC2", value: "0" },
                         ]
                     },
                      {
                          text: "WareHouse",
                          type: "controls",
                          cls: "span4",
                          required: true,
                          items: [
                              { name: "WareHouseCode", model: "data.WareHouseCode", cls: "span3", text: "WareHouse", type: "popup", click: "WareHouseCode()", required: true, validasi: "required", maxlength: 15 },
                              { name: "WareHouseName", model: "data.WareHouseName", text: "WareHouse Name", cls: "span5", readonly: true },
                          ]
                      },
                     { name: "DPPMaterial", model: 'data.DPPMaterial', type: "text", text: "DPP Material", cls: "span4 number-int", disable: true},
                     { name: "DPPOthers", model: 'data.DPPOthers', text: "DPP Others", cls: "span4 number-int", disable: true},
                     { name: "DPPFee", model: 'data.DPPFee', type: "text", text: "DPP Fee", cls: "span4 number-int", disable: true },
                     { name: "PPn", model: 'data.PPn', type: "text", text: "PPn", cls: "span4 number-int", disable: true },
                     { name: "Remark", text: "Notes", cls: "span8", readonly: false },

                ]
            },
            {
                name: "pnlBPU",
                title: "Karoseri Detail",
                items: [
                     {
                         text: "Code/ Chassis No",
                         type: "controls",
                         cls: "span8 full",
                         items: [
                            { name: "ChassisCode", model: "detail.ChassisCode", text: "Chassis Code", cls: "span4", readonly: true },
                             { name: "ChassisNo", model: "detail.ChassisNo", text: "Chassis No. ", cls: "span4", readonly: false, type: "popup", click: "chassisno()" },
                         ]
                     },
                      {
                          text: "Code/ Engine No.",
                          type: "controls",
                          cls: "span8 full",
                          items: [
                             { name: "EngineCode", model: "detail.EngineCode", text: "Engine Code", cls: "span4", readonly: true },
                              { name: "EngineNo", model: "detail.EngineNo", text: "Chassis Code ", cls: "span4", readonly: true, type: "text"},
                          ]
                      },
                      {
                          text: "Colour Old",
                          type: "controls",
                          cls: "span8 full",
                          items: [
                             { name: "ColourCodeOld", model: "detail.ColourCodeOld", text: "Colour Code Old", cls: "span4", readonly: true },
                              { name: "ColourOld", model: "detail.ColourOld", text: "Colour Name Old ", cls: "span4", readonly: true, type: "text" },
                          ]
                      },
                       {
                            text: "Colour New",
                            type: "controls",
                            cls: "span8 full",
                            items: [
                               { name: "ColourCodeNew", model: "detail.ColourCodeNew", text: "Colour Code New", cls: "span4", readonly: false, type: "popup", click: "colourcode()" },
                                { name: "ColourNew", model: "detail.ColourNew", text: "Colour Name New ", cls: "span4", readonly: true },
                            ]
                       },
                       { name: "Remark", model: "detail.Remark", text: "Remark", cls: "span8", readonly: false },
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