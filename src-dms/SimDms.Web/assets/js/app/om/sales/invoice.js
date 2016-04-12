"use strict";

function SalesInvoice($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    $('#SONo').on('blur', function () {
        if (me.data.SONo == null || me.data.SONo == '') return;

    });

    me.uistate = function () {
        switch (me.data.Status) {
            case "": //new
                me.data.StatusDsc = "New";
                $("input")
                    .prop('readonly', false);

                $("button")
                    .prop('disabled', false);


                $("#pnlHeader input ")
                    .prop('readonly', true);

                $("#pnlKet input,#pnlKet button,#pnlSlsMdl input,#pnlSlsMdl button,#btnApprove")
                    .prop('disabled', true);

                $("#Remark").prop('readonly', false);
                $("#Status label").html(me.data.StatusDsc);
                $("#btnDelete,#btnPrintPreview").hide();
                $("#InvoiceDate, #DueDate").prop('disabled', true);
                $('#SONo').prop('readonly', false);

                break;
            case "0": //open
                $("#Status label").html(me.data.StatusDsc);
                //$("#Remark").prop('disabled', true);
                //$("#pnlHeader button").prop('disabled', true);
                $("#pnlKet button,#pnlKet input").prop('disabled', false);
                $("#btnDelete,#btnPrintPreview").show();
                $("#pnlHeader input,#pnlHeader button").prop('disabled', true);

                //$("input[name='StatusPDI']").prop('disabled', false);
                break;
            case "1": //printed
                $("#Status label").html(me.data.StatusDsc);
                $("#Remark").prop('disabled', true);
                $("#pnlHeader button").prop('disabled', true);
                $("#pnlKet button,#detRemark,#btnApprove").prop('disabled', false)
                $("#btnDelete,#btnPrintPreview").show();
                break;
            case "2": //approved
                $("#Status label").html(me.data.StatusDsc);
                $('#btnApprove').prop('disabled', true);
                $("#btnDelete,#btnSave").hide();
                $("form button, form input").prop('disabled', true);
                $("#btnPrintPreview").show();
                break;
            case "3": //canceled
                $("#Status label").html(me.data.StatusDsc);
                $("form input,form button").prop('disabled', true);
                $("#btnDelete,#btnPrintPreview").hide();
                break;
            case "5":
                $("#Status label").html(me.data.StatusDsc);
                $('#btnApprove').prop('disabled', true);
                $("#btnDelete,#btnPrintPreview").show();
                $("form button, form input").prop('disabled', true);
                break;
            case "9":
                $("#Status label").html(me.data.StatusDsc); $("#Status label").html(me.data.StatusDsc);
                $('#btnApprove').prop('disabled', true);
                $("#btnDelete,#btnPrintPreview").show();
                $("form button, form input").prop('disabled', true);
                break;
            default: break;
        }
    }

    me.browse = function () {
        var lookup = Wx.klookup({
            name: "lookupSO",
            title: "Invoice",
            url: "om.api/Grid/SlsInvLkpBrowse",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "InvoiceNo", title: "No.Invoice", width: 150 },
                { field: "SalesType", title: "Tipe", width: 150 },
                { field: "InvoiceDate", title: "Tgl. Invoice", width: 150, template: "#= InvoiceDate==null ? '' : moment(InvoiceDate).format('DD MMM YYYY') #" },
                { field: "SONo", title: "No.SO", width: 150 },
                { field: "SKPKNo", title: "No.SKPK", width: 150 },
                { field: "RefferenceNo", title: "No.Reff", width: 150 },
                { field: "CustomerCode", title: "Pelanggan", width: 150 },
                { field: "CustomerName", title: "Nama Pelanggan", width: 150 },
                { field: "Address", title: "Alamat", width: 400 },
                { field: "BillTo", title: "Tagih Ke", width: 150 },
                { field: "BillName", title: "Nama Pelanggan", width: 150 },
                { field: "StatusDsc", title: "Status Code", width: 150 }
            ]
        });


        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.Address = data.Address;
                me.data.BillName = data.BillName;
                me.data.BillTo = data.BillTo;
                me.data.BranchCode = data.BranchCode;
                me.data.CustomerCode = data.CustomerCode;
                me.data.CustomerName = data.CustomerName;
                me.data.InvoiceDate = data.InvoiceDate;
                me.data.InvoiceNo = data.InvoiceNo;
                me.data.RefferenceNo = data.RefferenceNo;
                me.data.SKPKNo = data.SKPKNo;
                me.data.SONo = data.SONo;
                me.data.SalesType = data.SalesType;
                me.data.SalesTypeDsc = data.SalesTypeDsc;
                me.data.Status = data.Status;
                me.data.StatusDsc = data.StatusDsc;
                me.data.Remark = data.Remark;
                me.Apply();

                var params = {
                    InvoiceNo: me.data.InvoiceNo
                }

                $http.post('om.api/grid/SlsInvDtlBPk', params)
               .success(function (result) {
                   me.data.gridbpkdata = result;
                   me.loadTableData(me.gridbpk, me.data.gridbpkdata);
                   me.gridbpk.adjust();
                   me.uistate();

               })
               .error(function (result) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
               });
            }
        });
    }

    me.slsmdlcode = function () {
        console.log(me.data.bpk.BPKNo);
        var lookup = Wx.klookup({
            name: "salesmodelcodelkp",
            title: "Sales Model",
            url: "om.api/grid/SlsInvLkpSlsMdlCd",
            params: { BPKNo: me.data.bpk.BPKNo },
            serverBinding: true,
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code", width: 150 },
                { field: "SalesModelDesc", title: "Keterangan" }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.detil.SalesModelCode = data.SalesModelCode;
            me.data.detil.SalesModelDesc = data.SalesModelDesc;
            me.Apply();
        });
    }

    me.slsmdlyear = function () {
        var lookup = Wx.klookup({
            name: "salesmodelyearlkp",
            params: { BPKNo: me.data.bpk.BPKNo, SalesModelCode: me.data.detil.SalesModelCode },
            title: "Sales Model Year",
            url: "om.api/grid/SlsInvLkpSlsMdlYear",
            serverBinding: true,
            columns: [
                { field: "SalesModelYear", title: "Sales Model Year", width: 150 },
                { field: "SalesModelDesc", title: "Keterangan" }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.detil.SalesModelYear = data.SalesModelYear;
            me.data.detil.SalesModelDesc = data.SalesModelDesc;
            me.validatesalesmodel();
        });
    }

    $("[name = 'isAll']").on('change', function () {
        me.data.bpk.isAll = $('#isAll').prop('checked');
        me.Apply();
        //console.log(me.data.bpk.isAll);
    });

    me.bpklkp = function () {
        var lookup = Wx.klookup({
            name: "salesmodelbpk",
            title: "BPK",
            url: "om.api/grid/SlsInvLkpBPK",
            params: { InvoiceDate: me.data.InvoiceDate, SONo: me.data.SONo },
            serverBinding: true,
            columns: [
                { field: "BPKNo", title: "No. ", width: 150 },
                {
                    field: "BPKDate", title: "Tanggal BPK",
                    template: "#= (BPKDate == undefined) ? '' : moment(BPKDate).format('DD MMM YYYY') #"
                }
            ]
        });

        lookup.dblClick(function (data) {
            me.data.bpk.BPKNo = data.BPKNo;

            $http.post("om.api/Invoice/validatebpk", { InvoiceNo: me.data.InvoiceNo, BPKNo: data.BPKNo })
           .success(function (rslt) {
               if (!rslt.success) {
                   me.data.bpk.BPKNo = '';
                   MsgBox(rslt.message, MSG_ERROR);
                   me.uistate();
               }
           });
            me.Apply();
        });
    }

    //me.lkpbillto = function () {
    //    var lookup = Wx.klookup({
    //        name: "lkpbillto",
    //        title: "Bill To",
    //        url: "om.api/grid/SlsInvLkpBillTo",
    //        serverBinding: true,
    //        columns: [
    //            { field: "CustomerCode", title: "Kode", width: 150 },
    //            { field: "CustomerName", title: "Nama" }
    //        ]
    //    });

    //    lookup.dblClick(function (data) {
    //        me.data.BillTo = data.CustomerCode;
    //        me.data.BillName = data.CustomerName;
    //        me.Apply();
    //    });
    //}

    me.lkpbillto = function () {
        var lookup = Wx.klookup({
            name: "lkpbillto",
            title: "Bill To",
            url: "om.api/Grid/SlsDoLkpShiptoV2?cols=" + 5,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "CustomerCode", title: "Kode", width: 150 },
                { field: "CustomerName", title: "Nama" }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.BillTo = data.CustomerCode;
            me.data.BillName = data.CustomerName;
            me.Apply();
        });
    }

    me.lkpso = function () {
        var lookup = Wx.klookup({
            name: "lkpso",
            title: "SO",
            url: "om.api/grid/SlsInvLkpSO",
            serverBinding: true,
            columns: [
                { field: "SONo", title: "No. SO", width: 150 },
                { field: "SalesTypeDsc", title: "Tipe", width: 150 },
                { field: "SKPKNo", title: "No. SKPK", width: 150 },
                { field: "RefferenceNo", title: "No. Reff", width: 150 },
                { field: "CustomerCode", title: "Kode Pelanggan", width: 150 },
                { field: "CustomerName", title: "Nama Pelanggan", width: 150 },
                { field: "Address", title: "Alamat", width: 400 },
                { field: "BillTo", title: "Tagih ke", width: 150 },
                { field: "BillName", title: "Nama", width: 150 },
                { field: "TOPDays", title: "TOPDays", width: 150 }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.SONo = data.SONo;
            me.data.CustomerCode = data.CustomerCode;
            me.data.CustomerName = data.CustomerName;
            me.data.BillTo = data.BillTo;
            me.data.BillName = data.BillName;
            me.data.DueDate = moment(me.data.InvoiceDate).add(data.TOPDays, 'days');
            me.Apply();
        });
    }

    me.gridbpk = new webix.ui({
        container: "wxdetilbpk",
        view: "wxtable", css: "alternating",
        columns: [
            { id: "BPKNo", header: "No. BPK", width: 300 },
            { id: "Remark", header: "Keterangan", fillspace: true }
        ],
        on: {
            onSelectChange: function () {
                if (me.gridbpk.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridbpk.getSelectedId());
                    me.data.bpk.BPKNo = data.BPKNo;
                    me.data.bpk.Remark = data.Remark;
                    $("#pnlSlsMdl input").prop('readonly', true);
                    $("#pnlSlsMdl button,#pnlSlsMdl input").prop('disabled', false);
                    $('#RemarkDetil').prop('readonly', false);
                    $http.post('om.api/grid/SlsInvDtlSlsModel', { InvoiceNo: me.data.InvoiceNo, BPKNo: me.data.BPKNo })
                        .success(function (result) {
                            me.data.griddetildata = result;
                            me.loadTableData(me.griddetil, me.data.griddetildata);
                            me.griddetil.adjust();
                            me.uistate();
                            $('.gl-widget').animate({ scrollTop: 2000 }, 0);
                        })
                       .error(function (result) {
                           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                       });
                    /*Load Detail Part*/
                    $http.post('om.api/grid/getPartAccessories', { InvoiceNo: me.data.InvoiceNo })
                        .success(function (result) {
                            if (result.success) {
                                me.data.gridpartdata = result.data;
                                me.loadTableData(me.gridpart, me.data.gridpartdata);
                                me.gridpart.adjust();
                            }
                        })
                       .error(function (result) {
                           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                       });
                }
            }
        }
    });

    me.gridpart = new webix.ui({
        container: "wxdetilpart",
        view: "wxtable", css: "alternating",
        columns: [
            { id: "PartNo", header: "No Part", fillspace: true },
            { id: "PartName", header: "Nama Part", fillspace: true },
            { id: "SupplySlipNo", header: "No Supply Slip", fillspace: true },
            { id: "Quantity", header: "Jumlah", format: webix.i18n.numberFormat, fillspace: true },
            { id: "DPP", header: "DPP", format: webix.i18n.numberFormat, fillspace: true },
            { id: "PPn", header: "PPn", format: webix.i18n.numberFormat, fillspace: true },
            { id: "Total", header: "Total", format: webix.i18n.numberFormat, fillspace: true },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridpart.getSelectedId() !== undefined) {
                    //me.Apply();
                }
            }
        }
    });

    me.griddetil = new webix.ui({
        container: "wxdetil",
        view: "wxtable", css: "alternating",
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 150 },
            { id: "SalesModelYear", header: "Sales Model Year", width: 120 },
            { id: "Quantity", header: "Jumlah", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscDPP", header: "DPP Setelah Diskon", width: 100, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscPPn", header: "PPn Setelah Diskon", width: 100, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscPPnBM", header: "PPnBM Setelah Diskon", width: 100, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "OthersDPP", header: "DPP Lain-Lain", width: 100, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "OthersPPn", header: "PPn Lain-Lain", width: 100, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "Remark", header: "Keterangan", fillspace: true }
        ],
        on: {
            onSelectChange: function () {
                if (me.griddetil.getSelectedId() !== undefined) {
                    var data = this.getItem(me.griddetil.getSelectedId());
                    me.data.detil.AfterDiscDPP = data.AfterDiscDPP;
                    me.data.detil.AfterDiscPPn = data.AfterDiscPPn;
                    me.data.detil.AfterDiscPPnBM = data.AfterDiscPPnBM;
                    me.data.detil.AfterDiscTotal = data.AfterDiscTotal;
                    me.data.detil.BPKNo = data.BPKNo;
                    me.data.detil.BeforeDiscDPP = data.BeforeDiscDPP;
                    me.data.detil.DepositAmt = data.DepositAmt;
                    me.data.detil.DiscExcludePPn = data.DiscExcludePPn;
                    me.data.detil.DiscIncludePPn = data.DiscIncludePPn;
                    me.data.detil.InvoiceNo = data.InvoiceNo;
                    me.data.detil.OthersAmt = data.OthersAmt;
                    me.data.detil.OthersDPP = data.OthersDPP;
                    me.data.detil.OthersPPn = data.OthersPPn;
                    me.data.detil.PPnBMPaid = data.PPnBMPaid;
                    me.data.detil.Quantity = data.Quantity;
                    me.data.detil.QuantityReturn = data.QuantityReturn;
                    me.data.detil.Remark = data.Remark;
                    me.data.detil.SalesModelCode = data.SalesModelCode;
                    me.data.detil.SalesModelYear = data.SalesModelYear;
                    me.data.detil.ShipAmt = data.ShipAmt;
                    me.data.detil.id = data.id;

                    $http.post("om.api/Invoice/vldtSlsMdlYr", me.data.detil)
                    .success(function (rslt) {
                        if (rslt != null) {
                            me.data.detil.SalesModelDesc = rslt.SalesModelDesc;
                            me.uistate();
                        }
                    });
                }
            }
        }
    });


    me.validatesalesmodel = function () {
        var data = me.data.detil;
        data.SONo = me.data.SONo;
        $http.post("om.api/Invoice/modeldetil", data)
                  .success(function (rslt) {
                      if (rslt.success) {
                          if (rslt.isinvmodel) {
                              var data = rslt.data;
                              me.data.detil.AfterDiscDPP = data.AfterDiscDPP;//
                              me.data.detil.AfterDiscPPn = data.AfterDiscPPn;//
                              me.data.detil.AfterDiscPPnBM = data.AfterDiscPPnBM;
                              me.data.detil.AfterDiscTotal = data.AfterDiscTotal;
                              me.data.detil.BPKNo = data.BPKNo;
                              me.data.detil.BeforeDiscDPP = data.BeforeDiscDPP;//
                              me.data.detil.DepositAmt = data.DepositAmt;
                              me.data.detil.DiscExcludePPn = data.DiscExcludePPn;
                              me.data.detil.DiscIncludePPn = data.DiscIncludePPn;
                              me.data.detil.InvoiceNo = data.InvoiceNo;
                              me.data.detil.OthersAmt = data.OthersAmt;
                              me.data.detil.OthersDPP = data.OthersDPP;
                              me.data.detil.OthersPPn = data.OthersPPn;
                              me.data.detil.PPnBMPaid = data.PPnBMPaid;
                              me.data.detil.Quantity = data.Quantity;//
                              me.data.detil.QuantityReturn = data.QuantityReturn;
                              me.data.detil.Remark = data.Remark;
                              me.data.detil.SalesModelCode = data.SalesModelCode;//
                              me.data.detil.SalesModelYear = data.SalesModelYear;//
                              me.data.detil.ShipAmt = data.ShipAmt;
                          }
                          else {
                              var data = rslt.somdl;
                              me.data.detil.AfterDiscDPP = data.AfterDiscDPP;
                              me.data.detil.AfterDiscPPn = data.AfterDiscPPn;
                              me.data.detil.AfterDiscPPnBM = data.AfterDiscPPnBM;
                              me.data.detil.AfterDiscTotal = data.AfterDiscTotal;
                              me.data.detil.BeforeDiscDPP = data.BeforeDiscDPP;
                              me.data.detil.DepositAmt = data.DepositAmt;
                              me.data.detil.DiscExcludePPn = data.DiscExcludePPn;
                              me.data.detil.DiscIncludePPn = data.DiscIncludePPn;

                              me.data.detil.InvoiceNo = data.InvoiceNo;
                              me.data.detil.OthersAmt = data.OthersAmt;
                              me.data.detil.OthersDPP = data.OthersDPP;
                              me.data.detil.OthersPPn = data.OthersPPn;
                              me.data.detil.PPnBMPaid = data.PPnBMPaid;
                              me.data.detil.Remark = data.Remark;
                              me.data.detil.SalesModelCode = data.SalesModelCode;
                              me.data.detil.SalesModelYear = data.SalesModelYear;
                              me.data.detil.ShipAmt = data.ShipAmt;
                              me.data.detil.id = data.id;

                              data = rslt.bpkmdl;

                              me.data.detil.Quantity = data.QuantityBPK;
                              me.data.detil.QuantityReturn = data.QuantityReturn;
                              me.data.detil.BPKNo = data.BPKNo;
                          }

                          setTimeout(function () {
                              $("#pnlSlsMdl input").blur();
                          }, 300);

                      }
                      else {

                          console.log(rslt);
                          //me.data.detil.SalesModelDesc = rslt.SalesModelDesc;
                          //me.uistate();
                      }
                  });
    }

    me.saveData = function () {
        $http.post("om.api/Invoice/Save", me.data)
           .success(function (rslt) {
               if (rslt.success) {
                   me.data.InvoiceNo = rslt.InvoiceNo;
                   me.data.InvoiceDate = rslt.InvoiceDate;
                   me.data.Status = rslt.Status;
                   me.data.StatusDsc = rslt.StatusDsc;
                   me.uistate();
               }
               else {
                   MsgBox(rslt.message, MSG_ERROR);
               }
           });
    }

    me.delete = function () {
        $http.post("om.api/Invoice/Delete", me.data)
          .success(function (rslt) {
              if (rslt.success) {
                  me.data.InvoiceNo = rslt.InvoiceNo;
                  me.data.Status = rslt.Status;
                  me.data.StatusDsc = rslt.StatusDsc;
                  me.uistate();
              }
              else {
                  MsgBox(rslt.message, MSG_ERROR);
              }
          });
    }

    me.approve = function () {
        $http.post("om.api/Invoice/cekDataInvoiceVin", { invNo: $('#InvoiceNo').val() })
           .success(function (rslt) {
               if (rslt.success) {
                   $http.post("om.api/Invoice/Approve", me.data)
                    .success(function (rslt) {
                        if (rslt.success) {
                            me.data.Status = rslt.Status;
                            me.data.StatusDsc = rslt.StatusDsc;
                            me.uistate();
                            Wx.Success("Approved Invoice Berhasil");
                        }
                        else {
                            MsgBox(rslt.message, MSG_INFO);
                        }
                    });
               }
               else {
                   MsgBox(rslt.message, MSG_ERROR);
               }
           });
    }

    me.newbpk = function () {
        me.data.bpk.BPKNo = '';
        me.data.bpk.Remark = '';
        me.gridbpk.clearSelection();
        $("#pnlSlsMdl input,#pnlSlsMdl button").prop('disabled', true);
        me.newSlsMdl();
        me.griddetil.clearAll();
    }

    me.addbpk = function () {
        if (me.data.bpk.BPKNo == undefined || me.data.bpk.BPKNo == '') {
            $('#BPKNo').addClass('error');
            return;
        }

        me.data.bpk.InvoiceNo = me.data.InvoiceNo;


        $http.post("om.api/Invoice/addDtlBpk", me.data.bpk)
           .success(function (rslt) {
               if (rslt.success) {
                   $http.post('om.api/grid/SlsInvDtlBPk', { InvoiceNo: me.data.InvoiceNo })
                                 .success(function (result) {
                                     me.data.gridbpkdata = result;
                                     me.data.Status = rslt.Status;
                                     me.data.StatusDsc = rslt.StatusDsc;
                                     me.loadTableData(me.gridbpk, me.data.gridbpkdata);
                                     me.gridbpk.adjust();
                                     me.uistate();
                                     me.newSlsMdl();
                                     Wx.Success("Detail BPK berhasil disimpan!");
                                 })
               }
               else {
                   MsgBox(rslt.message, MSG_INFO);
               }
           });
    }
    me.delbpk = function () {
        var param = me.data.bpk;
        param.InvoiceNo = me.data.InvoiceNo;
        $http.post("om.api/Invoice/delDtlBpk", param)
           .success(function (rslt) {
               if (rslt.success) {
                   me.newSlsMdl();
                   $http.post('om.api/grid/SlsInvDtlBPk', { InvoiceNo: me.data.InvoiceNo })
                                 .success(function (result) {
                                     me.data.gridbpkdata = result;
                                     me.data.Status = rslt.Status;
                                     me.data.StatusDsc = rslt.StatusDsc;
                                     me.loadTableData(me.gridbpk, me.data.gridbpkdata);
                                     me.gridbpk.adjust();
                                     me.uistate();
                                     Wx.Success("Delete BPK berhasil!");
                                 });
               }
               else {
                   MsgBox(rslt.message, MSG_ERROR);
               }
           });
    }

    me.newSlsMdl = function () {
        me.data.detil.AfterDiscDPP = '';
        me.data.detil.AfterDiscPPn = '';
        me.data.detil.AfterDiscPPnBM = '';
        me.data.detil.AfterDiscTotal = '';
        me.data.detil.BPKNo = me.data.bpk.BPKNo;
        me.data.detil.BeforeDiscDPP = '';
        me.data.detil.DepositAmt = '';
        me.data.detil.DiscExcludePPn = '';
        me.data.detil.DiscIncludePPn = '';
        me.data.detil.InvoiceNo = '';
        me.data.detil.OthersAmt = '';
        me.data.detil.OthersDPP = '';
        me.data.detil.OthersPPn = '';
        me.data.detil.PPnBMPaid = '';
        me.data.detil.Quantity = '';
        me.data.detil.QuantityReturn = '';
        me.data.detil.Remark = '';
        me.data.detil.SalesModelCode = '';
        me.data.detil.SalesModelYear = '';
        me.data.detil.ShipAmt = '';
        me.data.detil.id = '';
        me.data.detil.SalesModelDesc = '';
        me.griddetil.clearSelection();
    }

    me.addSlsMdl = function () {
        var valid = true;

        if (me.data.detil.SalesModelCode == '') {
            $("#SalesModelCode").addClass('error');
            valid = false;
        }
        if (me.data.detil.SalesModelYear == '') {
            $("#SalesModelYear").addClass('error');
            valid = false;
        }

        if (!valid)
            return;

        var param = me.data.detil;
        param.InvoiceNo = me.data.InvoiceNo;
        $http.post("om.api/Invoice/addDtlSlsMdl", param)
           .success(function (rslt) {
               if (rslt.success) {
                   me.data.Status = rslt.Status;
                   me.data.StatusDsc = rslt.StatusDsc;
                   me.uistate();
                   $http.post('om.api/grid/SlsInvDtlSlsModel', { InvoiceNo: me.data.InvoiceNo, BPKNo: me.data.BPKNo })
                       .success(function (result) {
                           me.data.griddetildata = result;
                           me.loadTableData(me.griddetil, me.data.griddetildata);
                           me.griddetil.adjust();
                           $('.gl-widget').animate({ scrollTop: 2000 }, 0);
                       })
                      .error(function (result) {
                          MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                      });
               }
               else {
                   MsgBox(rslt.message, MSG_INFO);
               }
           });
    }
    me.delSlsMdl = function () {
        $http.post("om.api/Invoice/delDtlSlsMdl", me.data.detil)
           .success(function (rslt) {
               if (rslt.success) {
                   me.data.Status = rslt.Status;
                   me.data.StatusDsc = rslt.StatusDsc;
                   $http.post('om.api/grid/SlsInvDtlSlsModel', { InvoiceNo: me.data.InvoiceNo, BPKNo: me.data.BPKNo })
                       .success(function (result) {
                           me.data.griddetildata = result;
                           me.loadTableData(me.griddetil, me.data.griddetildata);
                           me.griddetil.adjust();
                           me.uistate();
                           $('.gl-widget').animate({ scrollTop: 2000 }, 0);
                           me.newSlsMdl();
                       })
                      .error(function (result) {
                          MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                      });
               }
               else {
                   MsgBox(rslt.message, MSG_ERROR);
               }
           });
    }

    me.print = function () {
        var hidePart = !$("#cbSparepartAccs").prop('checked');
        var ReportId = '';
        var par = '';

        if (me.data.Print.Flag == 'UMC') {
            ReportId = 'OmRpSalesTrn014';
            par = me.data.InvoiceNo + ',' + me.data.InvoiceNo;
        }
        else {
            if (me.data.Print.prtype == 'i') {
                //console.log("Params: " + me.data.Print.params)
                if ($("input[name='preprinted']:checked").val() == 0) {
                    //console.log(me.data.Print.Flag);
                    if (me.data.Print.Flag == '') {
                        ReportId = "OmRpSalesTrn004";
                    }
                    else {
                        ReportId = "OmRpSalesTrn004" + me.data.Print.Flag;
                    }
                }
                else {
                    ReportId = "OmRpSalesTrn004A";
                }
                par = ([me.data.InvoiceNo, me.data.InvoiceNo, '100', '', hidePart]).join(',');

            }
            else {
                par = me.data.InvoiceNo + ',' + me.data.InvoiceNo;
                if ($("input[name='paper']:checked").val() == 1) {
                    ReportId = "OmRpSalesTrn009";
                }
                else {
                    ReportId = "OmRpSalesTrn009A";
                }
            }

        }

        console.log(ReportId);
        Wx.showPdfReport({
            id: ReportId,
            pparam: par,
            textprint: true,
            //rparam: rparam,
            type: "devex"
        });

    }


    me.printPreview = function () {
        $http.post("om.api/Invoice/cekDataInvoiceVin", { invNo: $('#InvoiceNo').val() })
        .success(function (rslt) {
            if (rslt.success) {
                $http.post("om.api/Invoice/Print", { InvoiceNo: me.data.InvoiceNo, Status: me.data.Status })
                .success(function (rslt) {
                    me.data.Print.Flag = "";
                    if (rslt.success == true) {
                        me.data.Print.Flag = rslt.flag;
                        me.data.Status = rslt.Status;
                        me.data.StatusDsc = rslt.StatusDsc;
                        me.data.Flag = rslt.Flag;
                        me.uistate();
                        me.printDialog();

                    }
                    else {
                        MsgBox(rslt.message, MSG_ERROR);
                    }
                });
            } else {
                MsgBox(rslt.message, MSG_INFO);
            }
        });
    }

    me.printDialog = function () {
        var dl = BootstrapDialog.show({
            message: $(
'<div class="container">' +
'<div class="dlg1">' +
    '<div class="row">   ' +
     '<input type="checkbox" name="cbSparepartAccs" id="cbSparepartAccs"> Tampilkan Sparepart / Accesories' +
    '</div>  ' +
    '<div class="row">   ' +
     '<input type="radio" name="PrintType" id="PrintType1" value="i" checked="">&nbsp; Print Invoice             ' +
    '</div>  ' +
    '<div class="row" id="dlgpreprinted" style="padding-left:20px;">' +
      '<span>PrePrinted:&nbsp;</span>' +
      '<input type="radio" name="preprinted" id="preprinted1" value="1" checked="">&nbsp;Ya&nbsp;&nbsp;' +
      '<input type="radio" name="preprinted" id="preprinted2" value="0">&nbsp;Tidak      ' +
      '</div>  ' +
      '<div class="row"> ' +
       '<input type="radio" name="PrintType" id="PrintType2" value="n">&nbsp; Print Nota Debet' +
      '</div>  ' +
      '<div class="row" id="dlgpaper" style="padding-left:20px;display:none;">' +
        '<span>Kertas:&nbsp;</span>' +
       '<input type="radio" name="paper" id="paper1" value="5" checked="">&nbsp;1/2 Hal.' +
      '<input type="radio" name="paper" id="paper2" value="1">&nbsp;1 Hal.' +
     '</div>  ' +
    '</div>' +
'</div>'
                ),
            closable: false,
            draggable: true,
            type: BootstrapDialog.TYPE_INFO,
            title: 'Print',
            onshow: function (dialog) {

                setTimeout(function () {
                    $("input[name='PrintType']").on('click', function () {
                        if ($(this).val() == 'i') {
                            $("#dlgpreprinted").show();
                            $("#dlgpaper").hide();
                        }
                        else {
                            $("#dlgpreprinted").hide();
                            $("#dlgpaper").show();
                        }
                    });
                }, 300);
            },
            buttons: [{
                label: ' Print',
                cssClass: 'btn-primary icon-print',
                action: function (dialogRef) {
                    me.data.Print.prtype = $("input[name='PrintType']:checked").val();
                    if (me.data.Print.prtype == "i") {
                        me.data.Print.params == $("input[name='preprinted']:checked").val();
                    }
                    else {
                        me.data.Print.params == $("input[name='paper']:checked").val();
                    }
                    me.print();
                    dialogRef.close();
                }
            },
            {
                label: ' Cancel',
                cssClass: 'btn-warning icon-remove',
                action: function (dialogRef) {
                    dialogRef.close();
                }
            }]

        });

    }

    webix.event(window, "resize", function () {
        me.gridbpk.adjust();
        me.gridpart.adjust();
        me.griddetil.adjust();
    });

    me.initialize = function () {
        $('#InvoiceDate,#DueDate').attr('onkeydown', 'return false');
        $('#Status label').css(
        {
            "font-size": "32px",
            "color": "red",
            "font-weight": "bold"
        });

        me.data = {};
        me.data.Status = "";
        me.data.StatusDsc = "";

        me.data.gridbpkdata =
            me.data.gridpartdata =
                me.data.griddetildata = [];

        me.data.bpk = me.data.detil = {};


        me.data.InvoiceDate = me.data.DueDate = moment().format();
        me.data.detil.SalesModelCode = '';
        me.data.detil.SalesModelYear = '';
        me.data.bpk.isAll = false;

        me.data.Print = {};
        me.data.Print.params = "";

        me.gridbpk.clearAll();
        me.gridpart.clearAll();
        me.griddetil.clearAll();

        me.gridbpk.adjust();
        me.gridpart.adjust();
        me.griddetil.adjust();
        me.uistate();
        $("#btnDelete,#btnPrintPreview").removeClass('ng-hide');

    };

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Invoice",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlHeader",
                items: [
                    { name: "InvoiceNo", model: "data.InvoiceNo", text: "No.Invoice", cls: "span3", placeHolder: "INV/YY/XXXXXX" },
                    {
                        name: "InvoiceDateCtrl", type: "controls", text: "Tgl. Invoice", cls: "span5",
                        items: [
                            { name: "InvoiceDate", model: "data.InvoiceDate", type: "ng-datepicker", cls: "span3" },
                            {
                                type: "buttons", cls: "span2 left", items: [
                                   { name: "btnApprove", text: "Approve", cls: "btn-small btn-info", icon: "icon-ok", click: "approve()", disable: "me.statusBPU === NEW || me.statusBPU === APPROVED" }
                                ]
                            },
                            { name: "Status", model: "data.StatusDsc", text: "Open", cls: "span2", readonly: true, type: "label" }
                        ]
                    },
                    { name: "SONo", model: "data.SONo", text: "No. SO", type: "popup", cls: "span3", click: "lkpso()", validasi: "required" },
                    { name: "CustomerName", model: "data.CustomerName", text: "Pelanggan", cls: "span5" },
                    {
                        name: "DODateCtrl", type: "controls", required: true, text: "Tagih ke", cls: "span5",
                        items: [
                            { name: "BillTo", model: "data.BillTo", text: "BillTo", type: "popup", cls: "span3", click: "lkpbillto()", validasi: "required" },
                            { name: "BillToDsc", model: "data.BillName", text: "", cls: "span5" },
                        ]
                    },
                    { name: "DueDate", model: "data.DueDate", text: "Jatuh Tempo", type: "ng-datepicker", cls: "span3" },
                    { name: "Remark", model: "data.Remark", text: "Keterangan", cls: "span8" }
                ],
            },
            {
                name: "pnlKet",
                title: "Detil BPK",
                items: [
                     { name: "BPKNo", model: "data.bpk.BPKNo", text: "No. BPK", validasi: "required", type: "popup", cls: "span3", click: "bpklkp()" },
                     { name: "isAll", model: "data.bpk.isAll", text: "Pilih Semua", type: "check", cls: "span3" },
                     { name: "RemarkBpk", model: "data.bpk.Remark", text: "Keterangan", cls: "span8" },
                     {
                         type: "buttons", cls: "span4 left", items: [
                              { name: "btnNewBpk", text: "New", cls: "btn-small btn-info", icon: "icon-file", click: "newbpk()", disable: 'me.data.DOSeq ===""' },
                              { name: "btnAddBpk", text: "Save", cls: "btn-small btn-success", icon: "icon-save", click: "addbpk()", disable: 'me.data.Status ===""' },
                              { name: "btnDelBpk", text: "Delete", cls: "btn-small btn-danger", icon: "icon-remove", click: "delbpk()", disable: 'me.data.Status === ""' }
                         ]
                     },
                     {
                         name: "wxdetilbpk",
                         type: "wxdiv"
                     },
                     { name: "expdscd", text: "Detil Part", type: "label", cls: "span8" },
                     {
                         name: "wxdetilpart",
                         type: "wxdiv"
                     }
                ]
            },
             {
                 name: "pnlSlsMdl",
                 title: "Detil Sales Model",
                 items: [
                     { name: "SalesModelCode", model: "data.detil.SalesModelCode", validasi: "required", text: "Sales Model Code", type: "popup", cls: "span4", click: "slsmdlcode()" },
                     {
                         name: "DODateCtrl", type: "controls", text: "Sales Model Year", required: true, cls: "span4",
                         items: [
                             { name: "SalesModelYear", model: "data.detil.SalesModelYear", validasi: "required", type: "popup", cls: "span3", click: "slsmdlyear()" },
                             { name: "SalesModelDesc", model: "data.detil.SalesModelDesc", cls: "span5" },
                         ]
                     },
                      { name: "Quantity", model: "data.detil.Quantity", text: "Quantity", cls: "span4 number-int" },
                      { name: "AfterDiscDPP", model: "data.detil.AfterDiscDPP", text: "DPP Setelah Diskon", cls: "number-int span4", placeHolder: "0" },
                      { name: "BeforeDiscDPP", model: "data.detil.BeforeDiscDPP", text: "DPP Belum Diskon", cls: "span4 number-int" },
                      { name: "AfterDiscPPn", model: "data.detil.AfterDiscPPn", text: "PPn Setelah Diskon", cls: "span4 number-int" },
                      { name: "DiscIncludePPn", model: "data.detil.DiscIncludePPn", text: "Diskon", cls: "number-int span4" },
                      { name: "AfterDiscPPn", model: "data.detil.AfterDiscPPnBM", text: "PPnBM Setelah Diskon", cls: "span4 number-int" },
                      { name: "OthersDPP", model: "data.detil.OthersDPP", text: "DPP Lain-lain", cls: "span4 number-int" },
                      { name: "OthersPPn", model: "data.detil.OthersPPn", text: "PPn Lain-lain", cls: "span4 number-int" },
                      { name: "RemarkDetil", model: "data.detil.Remark", text: "Keterangan", cls: "span8" },
                      {
                          type: "buttons", cls: "span4 left", items: [
                              { name: "btnNewSlsMdl", text: "New", cls: "btn-small btn-info", icon: "icon-file", click: "newSlsMdl()", disable: 'me.data.DOSeq ===""' },
                              { name: "btnAddSlsMdl", text: "Save", cls: "btn-small btn-success", icon: "icon-save", click: "addSlsMdl()", disable: 'me.data.Status ===""' },
                              { name: "btnDelSlsMdl", text: "Delete", cls: "btn-small btn-danger", icon: "icon-remove", click: "delSlsMdl()", disable: 'me.data.Status === ""' }
                          ]
                      },
                      {
                          name: "wxdetil",
                          type: "wxdiv"
                      }
                 ]
             }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);


    function init(s) {
        SimDms.Angular("SalesInvoice");
    }



});