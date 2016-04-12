"use strict"

var batchNo = '';

function omPurchaseOrderController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $scope.$on('0', function () {
        $('#btnRefferenceNo').attr('disabled', true);
    });

    $scope.$on('1', function () {
        $('#btnRefferenceNo').removeAttr('disabled');
    });

    me.$watch('options', function (newValue, oldValue) {
        me.init();
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
        }
    });

    me.browse = function () {
        me.options = '0';
        //me.initialize();
        var lookup = Wx.blookup({
            name: "POLookup",
            title: "PO",
            manager: spSalesManager,
            query: "POBrowse",
            defaultSort: "PONo desc",
            columns: [
                { field: "PONo", title: "No. PO" },
                { field: "SupplierCode", title: "Kode Pemasok" },
                { field: "SupplierName", title: "Nama Pemasok" },
                { field: 'RefferenceNo', title: 'No. Ref' },
                { field: 'Status', title: 'Status PO' },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                //$('#btnSupplierCode').attr('disabled', true);
                //$('#RefferenceNo').attr('readonly', true);
                //$('#RefferenceDate').attr('disabled', true);
                $('#PODate').attr('disabled', true);

                //$('#BillTo').attr('readonly', true);
                //$('#ShipTo').attr('readonly', true);
                //$('#Remark').attr('readonly', true);

                $('#POStatus').html(data.Status);
                me.lookupAfterSelect(data);
                //if (data.Stat == "1") { $('#btnApprove').removeAttr('disabled'); }
                //else { $('#btnApprove').attr('disabled', 'disabled'); }
                me.isApprove = data.Stat == "2";

                switch (data.Stat) {
                    case "1":
                        $('#btnApprove').removeAttr('disabled');
                        $('#Remark').prop('readonly', false);
                        break;
                    case "2":
                        me.isCancel = true;
                        $('#btnSupplierCode, #RefferenceDate, #PODate').attr('disabled', true);
                        $('#BillTo, #ShipTo, #Remark, #RefferenceNo').attr('readonly', true);
                        break;
                    case "3":
                        me.isCancel = true;
                        $('#btnSupplierCode, #RefferenceDate, #PODate').attr('disabled', true);
                        $('#BillTo, #ShipTo, #Remark, #RefferenceNo').attr('readonly', true);
                        break;
                    default:
                        $('#btnApprove').attr('disabled', 'disabled');
                }

                me.loadDetail(data);
            }
        });
    };

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

    me.refferenceNoBrowse = function () {
        var lookup = Wx.blookup({
            name: "RefferenceNoBrowse",
            title: "Reff",
            manager: spSalesManager,
            query: "ReffNoBrowse",
            defaultSort: "RefferenceNo asc",
            columns: [
                { field: "BatchNo", title: "No. Batch" },
                { field: "RefferenceNo", title: "No. REff" },
                { field: 'RefferenceDate', title: 'Tgl. Reff', template: "#= (RefferenceDate == undefined) ? '' : moment(RefferenceDate).format('DD MMM YYYY') #" },
                { field: 'DealerCode', title: 'Pemasok' },
            ]
        });
        lookup.dblClick(function (data) {
            //if (data != null) {
            //    me.lookupAfterSelect(data);
            //}
            batchNo = data.BatchNo;
            me.data.RefferenceNo = data.RefferenceNo;
            me.data.RefferenceDate = data.RefferenceDate;
            me.data.SupplierCode = data.SupplierCode;
            me.Apply();
            me.data.SupplierName = data.SupplierName;
            me.Apply();
        });
    };

    me.salesModelCode = function () {
        me.detail = {};
        me.detail.switchTotal = true;
        var lookup = Wx.blookup({
            name: "SalesModelCodeBrowse",
            title: "Model",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("SalesModelCodeBrowse").withParameters({supplierCode : me.data.SupplierCode}),
            defaultSort: "SalesModelCode asc",
            columns: [
               { field: "SalesModelCode", title: "Sales Model Code" },
               { field: "SalesModelDesc", title: "Sales Model Desc" },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail.SalesModelCode = data.SalesModelCode;
            me.detail.SalesModelDesc = data.SalesModelDesc;
            me.Apply();
        });
    }

    me.salesModelYear = function () {
        var lookup = Wx.blookup({
            name: "SalesModelYearBrowse",
            title: "Model Year",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("SalesModelYearBrowse").withParameters({supplierCode : me.data.SupplierCode, salesModelCode: me.detail.SalesModelCode}),
            defaultSort: "SalesModelYear asc",
            columns: [
               { field: 'SalesModelYear', title: 'Sales Model Year' },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail.SalesModelYear = data.SalesModelYear;
            me.detail.PPnBMPaid = data.PPnBMPaid;
            me.detail.BeforeDiscTotal = data.Total;
            me.Apply();
        });
    }

    me.colourCodeBrowse = function () {
        var lookup = Wx.blookup({
            name: "ColorCodeBrowse",
            title: "Warna",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("ColourCodeBrowse").withParameters({ salesModelCode: me.detail.SalesModelCode }),
            defaultSort: "ColourCode asc",
            columns: [
                   { field: 'ColourCode', title: 'Kode Warna' },
                   { field: 'ColourDesc', title: 'Deskripsi Warna' },
            ]
        });
        lookup.dblClick(function (data) {
            me.colour.ColourCode = data.ColourCode;
            me.colour.ColourDesc = data.ColourDesc;
            me.colour.Remark = data.Remark
            $('#btnAddColour').removeAttr('disabled');
            $('#btnDeleteColour').removeAttr('disabled');
            me.Apply();
        });
    };

    me.loadDetail = function (data) {
        $http.post('om.api/po/getpo', data)
               .success(function (e) {
                   $('#pnlDetailPO').show();
                   $('#pnlAfterDisc').show();
                   $('#pnlBeforeDisc').show();
                   if (e.success) {
                       if (e.grid.length == "0") {
                           me.isPrintAvailable = false;
                           //$('#btnApprove').attr('disabled', true);
                       }
                       else {
                           me.loadTableData(me.gridDetailPO, e.grid);
                           me.isPrintAvailable = true;
                           //$('#btnApprove').removeAttr('disabled');
                       }
                       me.data = e.record[0];

                       
                   }
                   else {
                       MsgBox(e.message, MSG_ERROR);
                   }
               })
               .error(function (e) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
               });
       

    }

    me.gridDetailPO = new webix.ui({
        container: "wxDetailPO",
        view: "wxtable", css:"alternating",
        scrollX: true,
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 140, },
            { id: "SalesModelYear", header: "Sales Model Year", width: 140, },
            { id: "QuantityPO", header: "Jumlah PO", width: 100, css: { "text-align": "right" } },
            { id: "AfterDiscTotal", header: "Harga Setelah Diskon", width: 140, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscDPP", header: "DPP Setelah Diskon", width: 140, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscPPn", header: "PPn Setelah Diskon", width: 140, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscPPnBM", header: "PPnBMSetelah Diskon", width: 140, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "OthersDPP", header: "DPP Lain-lain", width: 140, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "OthersPPn", header: "PPn Lain-lain", width: 140, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "Remark", header: "Keterangan", width: 140 },
        //    { id: "PPnBmPaid", header: "PPnBMPaid", width: 100, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
        //    { id: "BeforeDiscTotal", header: "BeforeDiscTotal", width: 100, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
        //    { id: "DiscIncludePPn", header: "DiscIncludePPn", width: 100, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridDetailPO.getSelectedId() !== undefined) {
                    
                    var data = this.getItem(me.gridDetailPO.getSelectedId().id);

                    var datas = {
                        "PONo": me.data.PONo,
                        "SupplierCode": me.data.SupplierCode,
                        "SalesModelCode": data.SalesModelCode,
                        "SalesModelYear": data.SalesModelYear
                    }
                    $http.post('om.api/po/getdetailpo', datas)
                    .success(function (e) {
                        if (e.success) {
                            $('#pnlDetailColor').show();
                            me.colour.ColourCode = '';
                            me.colour.ColourDesc = '';
                            me.colour.Quantity = '0';
                            me.colour.Remark = '';
                            $('#btnAddColour').attr('disabled', 'disabled');
                            $('#btnDeleteColour').attr('disabled', 'disabled');
                            me.detail = e.detail[0];
                            me.loadTableData(me.gridDetailColour, e.colour);
                            me.detail.switchTotal = true;
                            me.delayEditing();
                        } else {
                            MsgBox(e.message, MSG_ERROR);
                        }
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
                }
            }
        }
    });

    me.gridDetailColour = new webix.ui({
        container: "wxDetailColour",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "ColourCode", header: "Warna", fillspace: true },
            { id: "Quantity", header: "Jumlah", width: 100, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "Remark", header: "Keterangan", fillspace: true },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridDetailColour.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridDetailColour.getSelectedId());
                    me.colour = data;
                    $('#btnAddColour').removeAttr('disabled');
                    $('#btnDeleteColour').removeAttr('disabled');
                    me.Apply();
                }
            }
        }
    });

    $('#RefferenceNo').on('change', function (e) {
        var refNo = me.data.RefferenceNo;
        if (refNo !== '')
        {
            $('#RefferenceDate').removeAttr('disabled');
        }
        else {
            $('#RefferenceDate').attr('disabled', true);
        }
    })

    $('#AfterDiscTotal, #AfterDiscDPP').on('change', function (e) {
        var isCheck = e.currentTarget.id === "AfterDiscTotal"
        var data = { val: isCheck, supplierCode: me.data.SupplierCode, poModel: me.detail };
        $http.post('om.api/po/calculateprice', data)
       .success(function (e) {
           me.detail.AfterDiscTotal = e.totalPrice;
           me.detail.AfterDiscDPP = e.dpp;
           me.detail.AfterDiscPPn = e.ppn;
           me.detail.AfterDiscPPnBM = e.ppnBm;
           me.detail.DiscIncludePPn = e.disc;
           me.delayEditing();
       });
    });

    $('#AfterDiscPPn, #AfterDiscPPnBM').on('change', function (e) {
        var afterDPP = parseFloat(me.detail.AfterDiscDPP);
        var afterPPn = parseFloat(me.detail.AfterDiscPPn);
        var afterPPnBM = parseFloat(me.detail.AfterDiscPPnBM);


        var totalPrice = afterDPP+ afterPPn + afterPPnBM;
        var disc = parseFloat(me.detail.BeforeDiscTotal) - totalPrice;

        me.detail.AfterDiscTotal = totalPrice;
        me.detail.DiscIncludePPn = disc;
        me.Apply();
        me.delayEditing();
    });

    me.save = function () {
        if (me.data.SupplierCode == null) { MsgBox("Supplier Code harus diisi!!", MSG_ERROR); }
        else {
            $http.post('om.api/po/validatesave', { model: me.data, salesmodelcode: me.detail.SalesModelCode, options: me.options, batchNo: batchNo })
           .success(function (e) {
                if (e.success) {
                    if (e.outstanding) {
                        if (confirm(e.message)) {
                            var par = me.data.RefferenceNo + "," + batchNo + "," + "" + "," + 1;

                            Wx.showPdfReport({
                                id: "OmRpPurTrn011",
                                pparam: par,
                                rparam: "",
                                type: "devex"
                            });

                            //return;
                       }
                       else {
                           //return;
                       }
                   }

                    $('#POStatus').html("OPEN");
                    Wx.Success(e.message2);
                    me.data.PONo = e.PONo;
                    me.loadDetail(me.data);
                    me.isInitialize = false;
                    me.hasChanged = false;
                    me.isLoadData = true;
                    me.gridDetailPO.adjust();
                    me.gridDetailColour.adjust();
                    $("#btnCancel").html("<i class='icon icon-hand-right'></i>Close");
                    $('#pnlDetailPO, #pnlAfterDisc, #pnlBeforeDisc').show();
               } else {
                    if (e.outstanding) {
                        if (confirm(e.message)) {
                            var par = me.data.RefferenceNo + "," + batchNo + "," + "" + "," + 1;

                            Wx.showPdfReport({
                                id: "OmRpPurTrn011",
                                pparam: par,
                                rparam: "",
                                type: "devex"
                            });
                            MsgBox(e.message2, MSG_ERROR);

                            return;
                        }
                        else {
                            MsgBox(e.message2, MSG_ERROR);
                            return;
                        }
                    }
               }
           })
           .error(function (e) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
           });
        }
    };

    me.delete = function () {
        if (confirm("Apakah Anda Yakin???", "Posting Data")) {
            $http.post('om.api/po/cancelpo', me.data)
            .success(function (e) {
                if (e.success) {
                    me.resetStat(e.status);
                    me.isApprove = true;
                    me.isCancel = true;
                }
                Wx.Success(e.message);
            })
            .error(function (e) {

            });
        }
    }

    me.approve = function () {
        if (confirm("Apakah Anda Yakin???", "Posting Data")) {
            $http.post('om.api/po/approvepo', me.data)
            .success(function (e) {
                if (e.success) {
                    me.resetStat(e.status);
                    me.isApprove = true;
                    me.isCancel = true;
                    me.isPrintAvailable = true;
                }
                Wx.Success(e.message);
            })
            .error(function (e) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
            });
        } 
    }
   
    me.SaveModel = function () {
        if (me.detail.AfterDiscDPP === undefined || me.detail.AfterDiscDPP <= 0) {
            Wx.alert("Harga Total/DPP tidak boleh kurang atau sama dengan nol.");
        }
        else {
            $http.post('om.api/po/savedetail', { model: me.data, poModel: me.detail })
           .success(function (e) {
               if (e.success) {
                   me.resetStat("OPEN");
                   if (e.outstanding) {
                       if (confirm(e.message)) {

                           var opt = me.options == "0";
                           var par = [
                               me.data.RefferenceNo,
                               opt ? "" : batchNo,
                               opt ? me.detail.SalesModelCode : "",
                               opt ? 0 : 1
                           ]

                           console.log(par);
                           Wx.showPdfReport({
                               id: "OmRpPurTrn011",
                               pparam: par,
                               rparam: "",
                               type: "devex"
                           });
                       }
                       else {
                       }
                   }
                   Wx.Success("Data Saved");
                   me.loadDetail(me.data);
                   me.detail = {};
               } else {
                   MsgBox(e.message, MSG_ERROR);
               }
           })
           .error(function (e) {
               MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
           });
        }
    };

    me.DeleteModel = function () {
          $http.post('om.api/po/deletedetail', { model: me.data, poModel: me.detail, colourModel: me.colour })
         .success(function (e) {
             if (e.success) {
                 Wx.Success("Data detail telah berhasil dihapus.");
                 me.loadDetail({ PONo: me.data.PONo });
                 me.resetStat("OPEN");
                 me.detail = {};
             } else {
                 MsgBox(e.message, MSG_ERROR);
             }
         })
         .error(function (e) {
             MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
         });
    }
    
    me.SaveColour = function () {
        if (me.detail.AfterDiscDPP === undefined || me.colour.Quantity <= 0) {
            Wx.alert("Jumlah tidak boleh kurang atau sama dengan nol.");
        }
        else {
            $http.post('om.api/po/savecolour', { model: me.data, poModel: me.detail, colourModel: me.colour })
           .success(function (e) {
               if (e.success) {
                   Wx.Success(e.message);
                   var data = {PONo : me.data.PONo, salesModelCode : me.detail.SalesModelCode, salesModelYear : me.detail.SalesModelYear}
                   me.loadDetailColour(data);
                   me.resetStat("OPEN");
                   me.colour = {};
                   me.colour.Quantity = 0;
               } else {
                   MsgBox(e.message, MSG_ERROR);
               }
           })
           .error(function (e) {
               MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
           });
        }
    };

    me.DeleteColour = function () {
        $http.post('om.api/po/deletecolour', { model: me.data, poModel: me.detail, colourModel: me.colour })
      .success(function (e) {
          if (e.success) {
              Wx.Success("Data telah berhasil dihapus.");
              var data = { PONo: me.data.PONo, salesModelCode: me.detail.SalesModelCode, salesModelYear: me.detail.SalesModelYear }
              me.loadDetailColour(data);
              me.resetStat("OPEN");
              me.colour = {};
              me.colour.Quantity = 0;
          } else {
              MsgBox(e.message, MSG_ERROR);
          }
      })
      .error(function (e) {
          MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
      });
    }

    me.loadDetailColour = function (data) {
        $http.post('om.api/po/getdetailcolour', data)
        .success(function (e) {
            $http.post('om.api/po/getpo', data)
            .success(function (e) {
                 if (e.success) {
                     me.data = e.record[0];
                     me.loadTableData(me.gridDetailPO, e.grid);
                 }
                 else {
                     MsgBox(e.message, MSG_ERROR);
                 }
             })
             .error(function (e) {
                 MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
             });

            me.loadTableData(me.gridDetailColour, e);
      })
      .error(function (e) {
          MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
      });
    }

    me.resetStat = function (stat) {
        $('#POStatus').html(stat);
        $('#btnApprove').attr('disabled', 'disabled');
    }

    me.printPreview = function () {
        $http.post('om.api/po/preprint', me.data)
       .success(function (e) {
           if (e.success) {
               $('#POStatus').html(e.Status);
               if (e.stat == "1") { $('#btnApprove').removeAttr('disabled'); }           
               BootstrapDialog.show({
                   message: $(
                       '<div class="container">' +
                       '<div class="row">' +

                       '<input type="radio" name="sizeType" id="sizeType1" value="full" checked>&nbsp Print Satu Halaman</div>' +

                       '<div class="row">' +

                       '<input type="radio" name="sizeType" id="sizeType2" value="half">&nbsp Print Setengah Halaman</div>'),
                   closable: false,
                   draggable: true,
                   type: BootstrapDialog.TYPE_INFO,
                   title: 'Print',
                   buttons: [{
                       label: ' Print',
                       cssClass: 'btn-primary icon-print',
                       action: function (dialogRef) {
                           me.Print();
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
           } else {
               MsgBox(e.message, MSG_ERROR);
           }
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
       });
    }

    me.Print = function () {
        var sizeType = $('input[name=sizeType]:checked').val() === 'full';

        var ReportId = sizeType ? 'OmRpPurTrn001' : 'OmRpPurTrn001A';
        var par = [
            me.data.PONo,
            me.data.PONo,
            '100',
            ''
        ]
        var rparam = 'Print Purchase Order'

        Wx.showPdfReport({
            id: ReportId,
            pparam: par,
            rparam: rparam,
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.clearTable(me.gridDetailPO);
        me.clearTable(me.gridDetailColour);
        me.detail = {};
        me.colour = {};
        me.data.PODate = me.now();
        me.data.RefferenceDate = me.now();
        me.hasChanged = false;
        me.isPrintAvailable = true;
        me.isApprove = true;
        me.isCancel = false;
        $('#btnAddColour').removeAttr('disabled');
        $('#btnDeleteColour').removeAttr('disabled');
        $('#POStatus').html("NEW");
        $('#POStatus').css(
        {
            "font-size": "32px",
            "color": "red",
            "font-weight": "bold",
            "text-align": "center"
        });

        $('#pnlDetailColor').hide();
        $('#pnlDetailPO').hide();
        $('#pnlAfterDisc').hide();
        $('#pnlBeforeDisc').hide();

        $('#btnSupplierCode').removeAttr('disabled');
        $('#RefferenceNo').removeAttr('readonly');
        $('#RefferenceDate').removeAttr('disabled');
        $('#PODate').removeAttr('disabled');
        $('#BillTo').removeAttr('readonly');
        $('#ShipTo').removeAttr('readonly');
        $('#Remark').removeAttr('readonly');
        $('#btnApprove').attr('disabled', true);
        me.detail.switchTotal = true;

        me.colour.Quantity = 0;
    }

    webix.event(window, "resize", function () {
        me.gridDetailPO.adjust();
        me.gridDetailColour.adjust();
    });

    me.start();
    me.options = '0';
    me.isApprove = true;
    me.isCancel = false;
    $('#btnRefferenceNo').attr('disabled', true);

    me.cancelOrClose = function () {
        //me.hasChanged = false;
        //me.isLoadData = false;
        //me.isEditable = false;
        //me.isSave = false;
        //me.isInitialize = false;
        me.initialize();
        setTimeout(function () {
            me.hasChanged = false;
            me.isLoadData = false;
            me.isEditable = false;
            me.isSave = false;
            me.isInitialize = false;
            $scope.$apply();
        }, 50);
    }
   
    me.$watch('colour.Quantity', function (newVal, oldVal) {
        if (newVal == '') {
            me.colour.Quantity = 0;
        }
    });
}

$(document).ready(function () {
    var options = {
        title: "Purchase Order",
        xtype: "panels",
        toolbars:[
                    { name: "btnBrowse", text: "Browse", cls:"btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize" , click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls:"btn btn-danger",  icon: "icon-remove", show: "!isApprove", click: "delete()" },
                    { name: "btnSave",   text: "Save",   cls:"btn btn-success", icon: "icon-save",   show: "hasChanged && !isInitialize", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls:"btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls:"btn btn-primary", icon: "icon-print", show: "isPrintAvailable && isLoadData", click: "printPreview()" , disable: "!isPrintEnable" },
                 ],
        panels: [
                 {
                     name: "pnloptions",
                     items: [
                           {
                               type: "optionbuttons",
                               name: "tabpageoptions",
                               model: "options",
                               items: [
                                   { name: "0", text: "Manual" },
                                   { name: "1", text: "Upload" },
                               ]
                           },
                     ]
                 },
                 {
                     name: "pnlStatus",
                     items: [
                          { name: "POStatus", text: "", cls: "span4 left", readonly: true, type: "label" },
                          {
                              type: "buttons", cls: "span4", items: [
                                     { name: "btnApprove", text: "Approve", cls: "btn btn-info", icon: "icon-ok", click: "approve()", disable: true },
                                     //{ name: "btnCancel", text: "Cancel", cls: "btn btn-info", icon: "icon-remove", click: "cancel()" },
                              ]
                          }
                     ]
                 },
                 {
                     name: "pnlPO",
                     items: [
                         { name: "PONo", text: "No. PO", model:"data.PONo", cls: "span4", readonly: true, placeHolder: 'PUR/XX/YYYYYY'},
                         { name: "PODate", text: "Tgl. PO", model: "data.PODate", cls: "span4", type: "ng-datepicker" },
                         { name: "RefferenceNo", model: "data.RefferenceNo", text: "No. Reff", cls: "span4", type: "popup", click: "refferenceNoBrowse()" },
                         { name: "RefferenceDate", text: "Tgl. Reff", model: "data.RefferenceDate", cls: "span4", type: 'ng-datepicker', disabled: true },
                         {
                             text: "Pemasok",
                             type: "controls",
                             required: true,
                             items: [
                                 { name: "SupplierCode", model: "data.SupplierCode", cls: "span2", placeHolder: "Kode Pemasok", readonly: true, type: "popup", click: "supplierBrowse()" },
                                 { name: "SupplierName", model: "data.SupplierName", cls: "span6", placeHolder: "Nama Pemasok", readonly: true }
                             ]
                         },
                         { name: "BillTo", text: "Bayar Ke", cls: "span12", readonly: false },
                         { name: "ShipTo", text: "Kirim Ke", cls: "span12", readonly: false },
                         { name: "Remark", text: "Keterangan", cls: "span12", readonly: false },
                     ]
                 },
                 {
                     name: "pnlDetailPO",
                     title: "Detail PO",
                     items: [
                         {
                             text: "Sales Model Code",
                             type: "controls",
                             required: true,
                             items: [
                                 { name: "SalesModelCode", model: "detail.SalesModelCode", cls: "span2", placeHolder: "Sales Model Code", readonly: true, type: "popup" ,click:'salesModelCode()'},
                                 { name: "SalesModelDesc", model: "detail.SalesModelDesc", cls: "span6", placeHolder: "Sales Model Desc", readonly: true }
                             ]
                         },
                       { name: "SalesModelYear", model: "detail.SalesModelYear", text: "Sales Model Year", cls: "span4", type: "popup", required: true, readonly: true, click: 'salesModelYear()' },
                       { name: "PPnBMPaid", model: "detail.PPnBMPaid", text: "PPnBM tlh Dibayar", cls: "span4 number-int", readonly: true, value: 0 },
                     ]
                 },
                 {
                     name: 'pnlAfterDisc',
                     title: 'Harga Setelah Diskon',
                     cls: 'span4',
                     items: [
                        {
                            text: "Harga Total",
                            type: "controls",
                            cls: "span4",
                            items: [
                                { name: "AfterDiscTotal", model: "detail.AfterDiscTotal", placeHolder: "Harga Total", cls: "span6 number-int", disable: "!detail.switchTotal", value: 0 },
                                { name: "switchTotal", model:"detail.switchTotal", cls: "span2", type: "x-switch"},
                            ]
                        },
                        { name: "AfterDiscDPP", model: "detail.AfterDiscDPP", text: "DPP", cls: "span4 number-int", disable: "detail.switchTotal", value: 0 },
                        { name: "AfterDiscPPn", model: "detail.AfterDiscPPn", text: "PPn", cls: "span4 number-int", disable: "detail.switchTotal", value: 0 },
                        { name: "AfterDiscPPnBM", model: "detail.AfterDiscPPnBM", text: "PPnBM", cls: "span4 number-int", disable: "detail.switchTotal", value: 0 },
                     ]
                 },
                 {
                     name: 'pnlBeforeDisc',
                     title: 'Harga Sebelum Diskon',
                     cls: 'span4',
                     items: [
                         { name: "BeforeDiscTotal", model: "detail.BeforeDiscTotal", text: "Harga Total", cls: "span4 number-int", readonly: true, value: 0 },
                         { name: "TotalDisc", model: "detail.DiscIncludePPn", text: "Diskon", cls: "span4 number-int", readonly: true, value: 0 },
                         { name: "OthersDPP", model: "detail.OthersDPP", text: "DPP Lain-Lain", cls: "span4 number-int", readonly: false, value: 0 },
                         { name: "OthersPPn", model: "detail.OthersPPn", text: "PPn Lain-Lain", cls: "span4 number-int", readonly: false, value: 0 },
                         { name: "Remark", model: "detail.Remark", text: "Keterangan", cls: "span12", readonly: false },
                         {
                             type: "buttons",
                             items: [
                                     { name: "btnAddModel", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "SaveModel()", disable: "detail.SalesModelCode === undefinedisCancel" , show: "!isCancel"},
                                     { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeleteModel()", disable: "detail.SalesModelCode === undefined" , show:"!isCancel" },
                             ]
                         },
                         {
                             name: "wxDetailPO",
                             type: "wxdiv"
                         },
                     ]
                 },
                 {
                     name: 'pnlDetailColor',
                     title: 'Detail Warna',
                     items: [
                         {
                             text: "Warna",
                             type: "controls",
                             items: [
                                 { name: "ColourCode", model: "colour.ColourCode", cls: "span2", placeHolder: "Kode Warna", readonly: true, type: "popup", click: 'colourCodeBrowse()' },
                                 { name: "ColourDesc", model: "colour.ColourDesc", cls: "span6", placeHolder: "Nama Warna", readonly: true }
                             ]
                         },
                         { name: "Quantity", model: "colour.Quantity", text: "Jumlah", cls: "span4 full number-int", readonly: false },
                         { name: "Remark", model: "colour.Remark", text: "Keterangan", cls: "span12", readonly: false },
                         {
                             type: "buttons",
                             items: [
                                     //{ name: "btnAddColour", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "SaveColour()", disable: "colour.ColourCode === undefined", show: "!isCancel" },
                                     //{ name: "btnDeleteColour", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeleteColour()", disable: "colour.ColourCode === undefined", show: "!isCancel" },
                                     { name: "btnAddColour", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "SaveColour()", disable: "true", show: "!isCancel" },
                                     { name: "btnDeleteColour", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeleteColour()", disable: "true", show: "!isCancel" },
                             ]
                         },
                         {
                             name: "wxDetailColour",
                             type: "wxdiv"
                         },
                     ]
                 },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("omPurchaseOrderController");
    }
});


