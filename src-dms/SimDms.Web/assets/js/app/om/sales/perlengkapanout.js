"use strict";

function SalesPerlengkapanOut($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });


    me.uistate = function () {
        switch (me.data.Status) {
            case "": //new                
                $("input,button")
                    .prop('disabled', false);
                $("#pnlHeader input ,#pnlDtl input,#pnlDtl button,#btnApprove,textarea")
                    .prop('disabled', true);
                $("#Remark,#chkReffDate,#PerlengkapanDate,#ReferenceNo").prop('disabled', false);
                $("#Status label").html(me.data.StatusDsc);
                $("#btnDelete,#btnPrintPreview").hide();                
                break;
            case "0": //open
                $("#Status label").html(me.data.StatusDsc);                
                $('.popup-wrapper input,#PerlengkapanType,#btnSourceDoc,#PerlengkapanDate').prop('disabled', true);
                $(".btnsls,#RemarkSlsMdl,#btnSalesModelCode,#QuantitySlsMdl").prop('disabled', false);
                $("#btnDelete,#btnPrintPreview").show();                
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
                $("#btnDelete,#btnPrintPreview").show();
                $("form button, form input").prop('disabled', true);


                break;
            case "3": //canceled
                $("#Status label").html(me.data.StatusDsc);
                $("form input,form button").prop('disabled', true);
                $("#btnDelete,#btnPrintPreview").hide();
                break;
            case "9":
                break;
            default: break;
        }
    }

    me.chkReffDate_change = function () {
        $("#ReferenceDate").prop("disabled", !me.data.chkReffDate);        
    }

    me.save = function () {
        console.log('saaaave');
        if(me.data.SourceDoc=="")
        {
            me.doclkp();
            return;
        }
        $http.post("om.api/PerlengkapanOut/Save", me.data)
            .success(function (rslt) {
                if (rslt.success == true) {
                    me.data.PerlengkapanNo = rslt.PerlengkapanNo;
                    me.data.Status = rslt.Status;
                    me.data.StatusDsc = rslt.StatusDsc;
                    me.uistate();
                    me.Apply();
                    MsgBox("Data Saved");
                }
                else {                  
                        MsgBox(rslt.message, MSG_ERROR);                 
                }
            });
    }

    me.delete = function () {
        $http.post("om.api/PerlengkapanOut/Delete",me.data)
        .success(function (rslt) {
            if (rslt.success == true) {
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
        $http.post("om.api/PerlengkapanOut/Approve", {PerlengkapanNo:me.data.PerlengkapanNo})
            .success(function (rslt) {
                if (rslt.success == true) {
                    me.data.Status = rslt.Status;
                    me.data.StatusDsc = rslt.StatusDsc;
                    me.uistate();
                    MsgBox("Approved");
                }
                else {
                    MsgBox(rslt.message, MSG_ERROR);
                }
            });
    }



    me.newdetail = function () {
        me.data.detailSalesModel.PerlengkapanNo="";
        me.data.detailSalesModel.SalesModelCode	="";
        me.data.detailSalesModel.Quantity=0;
        me.data.detailSalesModel.Remark = "";
        me.griddetilsalesmodel.clearSelection();
        $(".btnplkp,#RemarkDtl,#btnPerlengkapanNoDtl").prop('disabled', true);

        me.data.detailPerlengkapan.PerlengkapanNo	="";
        me.data.detailPerlengkapan.SalesModelCode	="";
        me.data.detailPerlengkapan.PerlengkapanCode	="";
        me.data.detailPerlengkapan.QuantityStd=0;
        me.data.detailPerlengkapan.Quantity	=0;
        me.data.detailPerlengkapan.Remark = "";
        me.griddetilperlengkapan.clearAll();
        //me.Apply();
    }

    me.adddetail = function () {
        console.log('add detail');
        if (me.data.detailSalesModel.SalesModelCode == "" ||me.data.detailSalesModel.SalesModelCode ==undefined ) {  
            console.log(me.data.detailSalesModel.SalesModelCode); 
            me.slsmdlcode();
            return;
        }

        if (me.data.detailSalesModel.Quantity < 1) {
            $("#QuantitySlsMdl").addClass('error');
            return;
        }
        var data = me.data.detailSalesModel;
        data.PerlengkapanNo = me.data.PerlengkapanNo;
        $http.post("om.api/PerlengkapanOut/savedetailmodel", data)
          .success(function (rslt) {
              if (rslt.success == true) {               
                  me.newdetail();
                  me.data.Status = rslt.Status;
                  me.data.StatusDsc = rslt.StatusDsc;
                  me.uistate();
                  $http.post("om.api/Grid/SlsPerlengkapanOutDetailSalesCode", { PerlengkapanNo: me.data.PerlengkapanNo })
                    .success(function (rslt) {                        
                        me.loadTableData(me.griddetilsalesmodel,rslt);
                        me.griddetilsalesmodel.adjust();
                    });
                  $("#btnNewDetail").click();
              }
              else {                 
                      MsgBox(rslt.message, MSG_ERROR);                  
              }
          });

    }


    me.deletedetail = function () { 
        if (me.data.detailSalesModel.DOSeq != "" || me.data.detailSalesModel.SalesModelCode !=undefined) {
            $http.post("om.api/PerlengkapanOut/deleteDetilmodel", { PerlengkapanNo:me.data.PerlengkapanNo,SalesModelCode:me.data.detailSalesModel.SalesModelCode})
          .success(function (rslt) {
              if (rslt.success == true) {
                  me.newdetail();
                  me.data.Status = rslt.Status;
                  me.data.StatusDsc = rslt.StatusDsc;
                  me.uistate();
                  $http.post("om.api/Grid/SlsPerlengkapanOutDetailSalesCode", { BPKNo: me.data.BPKNo })
                    .success(function (rslt) {                        
                        me.loadTableData(me.griddetilsalesmodel,rslt);
                        me.griddetilsalesmodel.adjust();
                    });                  
              }
              else {
                  MsgBox(rslt.message, MSG_ERROR);
              }
          });
        }
    }


    me.newdetailplkp = function () {        
        me.data.detailPerlengkapan.SalesModelCode = "";
        me.data.detailPerlengkapan.PerlengkapanCode = "";
        me.data.detailPerlengkapan.PerlengkapanName = "";
        me.data.detailPerlengkapan.QuantityStd = 0;
        me.data.detailPerlengkapan.Quantity = 0;
        me.data.detailPerlengkapan.Remark = "";
        me.griddetilperlengkapan.clearSelection();
    }

    me.adddetailplkp = function () {        
        if (me.data.detailPerlengkapan.PerlengkapanCode == "" || me.data.detailPerlengkapan.PerlengkapanCode ==undefined) {            
            me.perlengkapanlkp();
            return;
        }

        if (me.data.detailPerlengkapan.Quantity == "" || me.data.detailPerlengkapan.Quantity == "0") {
            $("#QuantityPlkp").addClass('error');
            return;
        }

        $("#QuantityPlkp").removeClass('error');
        var data = me.data.detailPerlengkapan;
        data.PerlengkapanNo = me.data.PerlengkapanNo;
        data.SalesModelCode = me.data.detailSalesModel.SalesModelCode;
        $http.post("om.api/PerlengkapanOut/savedetailperlengkapan", data)
          .success(function (rslt) {
              if (rslt.success == true) {
                  me.data.Status = rslt.Status;
                  me.data.StatusDsc = rslt.StatusDsc;                  
                  me.uistate();                  
                  MsgBox("Data Saved");                  
                  $http.post("om.api/grid/SlsPerlengkapanOutDetailPerlengkapan", { PerlengkapanNo: me.data.PerlengkapanNo, SalesModelCode: me.data.detailSalesModel.SalesModelCode })
                    .success(function (rslt, status, headers, config) {
                        me.data.gridPerlengkapan = rslt;
                        me.loadTableData(me.griddetilperlengkapan, me.data.gridPerlengkapan);
                        me.griddetilperlengkapan.adjust();
                        me.griddetilperlengkapan.clearSelection();
                    });
                  me.newdetailplkp();
              }
              else {
                  MsgBox(rslt.message, MSG_ERROR);                 
              }
          });
    }



    me.deletedetailplkp = function () {
        if (me.griddetilperlengkapan.PerlengkapanCode != "") {
            $http.post("om.api/PerlengkapanOut/deletedtlperlenglapan", { PerlengkapanNo: me.data.PerlengkapanNo,SalesModelCode:me.data.detailSalesModel.SalesModelCode,PerlengkapanCode:me.data.detailPerlengkapan.PerlengkapanCode})
          .success(function (rslt) {
              if (rslt.success == true) {
                  me.newdetailplkp();
                  me.data.Status = rslt.Status;
                  me.data.StatusDsc = rslt.StatusDsc;
                  me.uistate();
                  $http.post("om.api/grid/SlsPerlengkapanOutDetailPerlengkapan", { PerlengkapanNo: me.data.PerlengkapanNo, SalesModelCode: me.data.detailSalesModel.SalesModelCode })
                    .success(function (rslt, status, headers, config) {
                        me.data.gridPerlengkapan = rslt;
                        me.loadTableData(me.griddetilperlengkapan, me.data.gridPerlengkapan);
                        me.griddetilperlengkapan.adjust();
                        me.griddetilperlengkapan.clearSelection();                        
                    });                  
              }
              else {
                  MsgBox(rslt.message, MSG_ERROR);
              }
          });
        }
    }


    me.printPreview = function () {
        $http.post("om.api/PerlengkapanOut/Print", { PerlengkapanNo: me.data.PerlengkapanNo })
        .success(function (rslt) {
            if (rslt.success == true) {
                me.data.Status = rslt.Status;
                me.data.StatusDsc = rslt.StatusDsc;
                me.uistate();
                me.Print();
            }
            else {
                MsgBox(rslt.message, MSG_ERROR);
            }
        });
    }

   

    me.Print = function () {
      
        var par = me.data.PerlengkapanNo + ',' + me.data.PerlengkapanNo;
        var ReportId = "OmRpSalesTrn008";

        Wx.showPdfReport({
            id: ReportId,
            pparam: par,
            textprint: true,
            //rparam: rparam,
            type: "devex"
        });
    }


    me.browse = function () {
        var lookup = Wx.klookup({
            name: "BrowsePerlengkapanOut",
            title: "Perlengkapan Out",
            url: "om.api/grid/SlsPerlengkapanOutBrowse",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "PerlengkapanNo", title: "No BPU", width: 130 },
                {
                    field: "PerlengkapanDate", title: "Tgl BPU", width: 130,
                    template: "#= (PerlengkapanDate == undefined) ? '' : moment(PerlengkapanDate).format('DD MMM YYYY') #"
                },

                { field: "PerlengkapanTypeDsc", title: "No.Reff.DO", width: 130 },
                { field: "ReferenceNo", title: "No.Reff.SJ", width: 150 },
                { field: "SourceDoc", title: "No.PO", width: 130 },
                { field: "SupplierName", title: "Pemasok", width: 300 },
                { field: "Remark", title: "Tipe", width: 50 },
                { field: "StatusDsc", title: "Status", width: 130 }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.PerlengkapanNo = data.PerlengkapanNo;
            me.data.PerlengkapanDate = data.PerlengkapanDate;
            me.data.ReferenceNo = data.ReferenceNo;
            me.data.SourceDoc = data.SourceDoc;
            me.data.Remark = data.Remark;
            me.data.PerlengkapanType = data.PerlengkapanType;
            me.data.PerlengkapanTypeDsc = data.PerlengkapanTypeDsc;
            me.data.Status = data.Status;
            me.data.StatusDsc = data.StatusDsc;
            me.data.CustomerCode = data.CustomerCode;
            $http.post("om.api/grid/SlsPerlengkapanOutDetailSalesCode", { PerlengkapanNo: me.data.PerlengkapanNo, CustomerCode: me.data.CustomerCode })
            .success(function (rslt, status, headers, config) {
                me.data.gridSalesModel = rslt.data;
                me.data.CustomerName = rslt.CustomerName;
                me.loadTableData(me.griddetilsalesmodel, me.data.gridSalesModel);
                me.griddetilsalesmodel.adjust();
                me.uistate();
            });
        });
    }

    me.doclkp = function () {
        if (me.data.PerlengkapanType == 1) {
            var lookup = Wx.klookup({
                name: "BrowsePerlengkapanOut",
                title: "Document",
                url: "om.api/grid/SlsPerlengkapanOutBrwSrcDoc",
                serverBinding: true,
                params: { PerlengkapanType: me.data.PerlengkapanType },
                pageSize: 10,
                columns: [
                    { field: "BPKNo", title: "Sumber Dokumen", width: 130 },
                    { field: "CustomerName", title: "Pelanggan", width: 50 },
                    { field: "StatusDsc", title: "Alamat", width: 130 }
                ]
            });
            lookup.dblClick(function (data) {
                me.data.SourceDoc = data.BPKNo;
                me.data.CustomerCode = data.CustomerCode;
                me.data.CustomerName = data.CustomerName;
                me.Apply();
            });
        }
        else if (me.data.PerlengkapanType == 2) {
            var lookup = Wx.klookup({
                name: "BrowsePerlengkapanOut",
                title: "Perlengkapan Out",
                url: "om.api/grid/SlsPerlengkapanOutBrwSrcDoc",
                serverBinding: true,
                params: { PerlengkapanType: me.data.PerlengkapanType },
                pageSize: 10,
                columns: [
                    { field: "TransferOutNo", title: "Transfer No", width: 130 },                    
                ]
            });
            lookup.dblClick(function (data) {
                me.data.SourceDoc = data.TransferOutNo;
                //me.data.CustomerCode = data.CustomerCode;
                //me.data.CustomerName = data.CustomerName;
                me.Apply();
            });
        }
        else {
            var lookup = Wx.klookup({
                name: "BrowsePerlengkapanOut",
                title: "Perlengkapan Out",
                url: "om.api/grid/SlsPerlengkapanOutBrwSrcDoc",
                serverBinding: true,
                params: { PerlengkapanType: me.data.PerlengkapanType },
                pageSize: 10,
                columns: [
                    { field: "ReturnNo", title: "Sumber Dokumen", width: 130 }                    
                ]
            });
            lookup.dblClick(function (data) {
                me.data.SourceDoc = data.ReturnNo;
                //me.data.CustomerCode = data.CustomerCode;
                //me.data.CustomerName = data.CustomerName;
                me.Apply();
            });
        }
    }

    me.slsmdlcode = function () {
        var lookup = Wx.klookup({
            name: "BrowsePerlengkapanOut",
            title: "Sales Model",
            url: "om.api/grid/SlsPerlengkapanOutLkpSlsMdlCd",
            serverBinding: true,
            params: { PerlengkapanType: me.data.PerlengkapanType,SourceDoc:me.data.SourceDoc },
            pageSize: 10,
            columns: [
                { field: "SalesModelCode", title: "Kode Sales Model", width: 130 },
                { field: "SalesModelDesc", title: "Deskripsi" }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.detailSalesModel.SalesModelCode = data.SalesModelCode;
            me.Apply();
        });
    }

    me.perlengkapanlkp = function () {
        var lookup = Wx.klookup({
            name: "BrowsePerlengkapanOut",
            title: "Sales Model",
            url: "om.api/grid/SlsPerlengkapanOutLkpSlsMdlDtl",
            params:{SalesModelCode:me.data.detailSalesModel.SalesModelCode},
            serverBinding: true,            
            pageSize: 10,
            columns: [
                { field: "PerlengkapanCode", title: "Kode", width: 130 },
                { field: "PerlengkapanName", title: "Nama Perlengkapan"}                
            ]
        });
        lookup.dblClick(function (data) {
            me.data.detailPerlengkapan.PerlengkapanCode = data.PerlengkapanCode;
            me.data.detailPerlengkapan.PerlengkapanName = data.PerlengkapanName;
            me.data.detailPerlengkapan.Remark = data.Remark;
            me.data.detailPerlengkapan.QuantityStd = data.Quantity;
            me.Apply();
        });
    }

    me.griddetilsalesmodel = new webix.ui({
        container: "wxdetilsalesmodel",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        //autoHeight: false,
        height: 260,
        width: 520,
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", fillspace: true },
            { id: "Quantity", header: "Jumlah", format: webix.i18n.intFormat, fillspace: true },
            { id: "Remark", header: "Keterangan", fillspace: true }
        ],
        on: {
            onSelectChange: function () {
                $(".btnplkp,#RemarkDtl,#btnPerlengkapanNoDtl,#QuantityPlkp").prop('disabled', false);
                if (me.griddetilsalesmodel.getSelectedId() !== undefined) {
                    var griddata = this.getItem(me.griddetilsalesmodel.getSelectedId());
                    me.data.detailSalesModel.SalesModelCode = griddata.SalesModelCode;
                    me.data.detailSalesModel.Quantity = griddata.Quantity;
                    me.data.detailSalesModel.Remark = griddata.Remark;
                    $http.post("om.api/grid/SlsPerlengkapanOutDetailPerlengkapan", { PerlengkapanNo:me.data.PerlengkapanNo,SalesModelCode: griddata.SalesModelCode })
                    .success(function (rslt, status, headers, config) {                       
                        me.data.gridPerlengkapan = rslt;
                        me.loadTableData(me.griddetilperlengkapan, me.data.gridPerlengkapan);
                        me.griddetilperlengkapan.adjust();
                        me.griddetilperlengkapan.clearSelection();                      
                    });
                    me.Apply();
                    
                }
            }   
        }
    });


    me.griddetilperlengkapan = new webix.ui({
        container: "wxdetilperlengkapan",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        //autoHeight: false,
        height: 260,
        width: 520,
        columns: [
            { id: "PerlengkapanNo", header: "Kode", width: 100 },
            { id: "PerlengkapanName", header: "Nama Perlengkapan", width: 150 },
            { id: "QuantityStd", header: "Jumlah Standar", format: webix.i18n.intFormat, width: 80 },
            { id: "Quantity", header: "Jumlah", format: webix.i18n.intFormat, width: 80 },
            { id: "Remark", header: "Keterangan", fillspace: true }
        ],
        on: {
            onSelectChange: function () {
                if (me.griddetilperlengkapan.getSelectedId() !== undefined) {
                    var griddata = this.getItem(me.griddetilperlengkapan.getSelectedId());
                    me.data.detailPerlengkapan.PerlengkapanNo = griddata.PerlengkapanNo;                    
                    me.data.detailPerlengkapan.PerlengkapanName = griddata.PerlengkapanName;
                    me.data.detailPerlengkapan.PerlengkapanCode = griddata.PerlengkapanCode;
                    me.data.detailPerlengkapan.QuantityStd = griddata.QuantityStd;
                    me.data.detailPerlengkapan.Quantity = griddata.Quantity;
                    me.data.detailPerlengkapan.Remark = griddata.Remark;
                    me.Apply();
                }
            }
        }
    });



    webix.event(window, "resize", function () {
        me.griddetilsalesmodel.adjust();
        me.griddetilperlengkapan.adjust();
    })



    me.initialize = function () {
        $('#SourceDoc').attr('disabled', 'disabled');
        $("#PerlengkapanType option[value='']").remove();
        $("#PerlengkapanType").on('change', function () {            
            me.data.SourceDoc = "";
            me.Apply();
        });

        $('#PerlengkapanDate,#ReferenceDate').attr('onkeydown', 'return false');

        $('#Status label').css(
          {
              "font-size": "32px",
              "color": "red",
              "font-weight": "bold",
              "text-align": "center"
          });

        me.data = {};
        me.data.StatusDsc = "New";
        me.data.Status = "";        
        me.data.detailSalesModel = {};
        me.data.detailPerlengkapan = {};
        me.data.PerlengkapanType = 1;
        me.data.gridSalesModel = [];
        me.data.gridPerlengkapan=[];
        me.data.PerlengkapanDate = me.data.ReferenceDate = moment().format();
        me.data.chkReffDate = false;
        me.data.SourceDoc = "";


        $http.post("om.api/grid/WarehouseList")
       .success(function (data) {
           if (data.data.length > 0) {
               me.data.WareHouseCode = data.data[0].LookUpValue;
               me.data.WareHouseName = data.data[0].LookUpValueName;
               me.isLoadData = false;
               //me.Apply();
           }
       });
        me.griddetilsalesmodel.clearAll();
        me.griddetilsalesmodel.adjust();

        me.griddetilperlengkapan.clearAll();
        me.griddetilperlengkapan.adjust();        
        me.uistate();
        $("#btnDelete,#btnPrintPreview").removeClass('ng-hide');
        me.cancelOrClose = function () {
            me.init();        
        }

        //$("[data-validate='required']").addClass('error');

    };


    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Perlengkapan Out",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlHeader",
                items: [
                    { name: "PerlengkapanNo", model: "data.PerlengkapanNo", text: "No.Perlengkapan", cls: "span3", placeHolder: "PLK/YY/XXXXXX" },
                    {
                        name: "PerlengkapanDateCtrl", type: "controls", text: "Tgl. Perlengkapan", cls: "span5",
                        items: [
                            { name: "chd", model: "data.chkReffDate", cls: "span1", type: "ng-check",style:"visibility:hidden;" },
                            { name: "PerlengkapanDate", model: "data.PerlengkapanDate", type: "ng-datepicker", cls: "span3" },
                            { name: "Status", model: "model.Status", text: "Open", cls: "span3", readonly: true, type: "label" },
                        ]
                    },
                    { name: "ReferenceNo", model: "data.ReferenceNo", text: "No.Reff", cls: "span3", disabled: true },
                    {
                        name: "DODateCtrl", type: "controls", text: "Tgl. Reff", cls: "span5",
                        items: [
                            { name: "chkReffDate", model: "data.chkReffDate", cls: "span1", type: "ng-check", disable: "me.statusBPU === 'APPROVED' || !allowEdit", change: "chkReffDate_change()" },
                            { name: "ReferenceDate", model: "data.ReferenceDate", type: "ng-datepicker", cls: "span3" },
                            {
                                type: "buttons", cls: "span3 left", items: [
                                   { name: "btnApprove", text: "Approve", cls: "btn-small btn-info", icon: "icon-ok", click: "approve()", disable: "me.statusBPU === NEW || me.statusBPU === APPROVED" }
                                ]
                            }
                        ]
                    },
                    {
                        name: "PerlengkapanType",
                        text: "Type",
                        type: "select",
                        cls: "span3",
                        model: "data.PerlengkapanType",                        
                        items: [
                            { value: '1', text: 'BPK' },
                            { value: '2', text: 'TRANSFER' },
                            { value: '3', text: 'RETURN' }
                        ]
                    },

                   {
                       name: "SourceCtrl", type: "controls", text: "Sumber Dok", required: true, cls: "span5",
                          items: [
                                    { name: "chd", model: "data.chkReffDate", cls: "span1", type: "ng-check", style: "visibility:hidden;" },
                                    { name: "SourceDoc", model: "data.SourceDoc", text: "Sumber Dok.", type: "popup", cls: "span3", click: "doclkp()", validasi: "required"},
                          ]
                   },
                   { name: "CustomerName", model: "data.CustomerName", text: "Pelanggan", cls: "span8" },
                   { name: "Remark", model: "data.Remark", text: "Keterangan", cls: "span8" }
                ],
            },
            {
                name: "pnlDtl",
                title: "Detil",
                items: [
                    { type: "label", text: "Detil Sales Model", cls: "span4", style: "font-size: 14px; color: blue;" },
                    { type: "label", text: "Detil Perlengkapan", cls: "span4", style: "font-size: 14px; color: blue;" },
                    { type: "div", cls: "divider span3" },
                    { type: "separator", cls: "span1" },
                    { type: "div", cls: "divider span4" },                    
                    {
                        name: "SalesModelCodeCtrl",
                        type: "controls",
                        text: "Sales Model Code",
                        cls: "span4",
                        items: [
                            { name: "SalesModelCode", model: "data.detailSalesModel.SalesModelCode", type: "popup", cls: "span6", click: "slsmdlcode()" }
                        ]
                    },

                    {
                        name: "ctrlperlengkapan",
                        type: "controls",
                        text: "Perlengkapan",
                        cls: "span4",
                        items: [
                            { name: "PerlengkapanNoDtl", model: "data.detailPerlengkapan.PerlengkapanCode", type: "popup", cls: "span3", click: "perlengkapanlkp()" },
                            { name: "PerlengkapanName", model: "data.detailPerlengkapan.PerlengkapanName", cls: "span5" }
                        ]
                    },
                     { name: "QuantitySlsMdl", model: "data.detailSalesModel.Quantity", text: "jumlah",  cls: "span4 number-int" },
                     { name: "QuantityStdPlkp", model: "data.detailPerlengkapan.QuantityStd", text: "Jumlah Standar",  cls: "span4 number-int" },
                     { name: "RemarkSlsMdl", model: "data.detailSalesModel.Remark", type: "textarea", text: "Keterangan", cls: "span4", style: "min-height: 118px; " },
                     { name: "QuantityPlkp", model: "data.detailPerlengkapan.Quantity", text: "Jumlah", cls: "span4 number-int" },
                     { name:"tmp1",cls:"span4",type:"hidden"},
                     { name: "RemarkDtl", model: "data.detailPerlengkapan.Remark", text: "Keterangan", cls: "span4" },
                     {
                         type: "buttons", cls: "span4 left", items: [
                                 { name: "btnNewDetailPlkp", text: "New", cls: "btnplkp btn-small btn-info", icon: "icon-ok", click: "newdetailplkp()", disable: 'me.data.Status ===""' },
                                 { name: "btnSavePlkp", text: "Save", cls: "btnplkp btn-small btn-info", icon: "icon-ok", click: "adddetailplkp()", disable: 'me.data.Status ===""' },
                                 { name: "btnDeletePlkp", text: "Delete", cls: "btnplkp btn-small btn-info", icon: "icon-ok", click: "deletedetailplkp()", disable: 'me.data.Status ===""' }
                         ]
                     },
                      {
                          type: "buttons", cls: "span4 left full", items: [
                                { name: "btnNewDetailSales", text: "New", cls: "btnsls btn-small btn-info", icon: "icon-ok", click: "newdetail()", disable: 'me.data.Status ===""' },
                                { name: "btnSaveSales", text: "Save", cls: "btnsls btn-small btn-info", icon: "icon-ok", click: "adddetail()", disable: 'me.data.Status ===""' },
                                { name: "btnDeleteSales", text: "Delete", cls: "btnsls btn-small btn-info", icon: "icon-ok", click: "deletedetail()", disable: 'me.data.Status ===""' }
                          ]
                      },
                     {
                         name: "wxdetilsalesmodel",
                         type: "wxdiv",
                         cls: "span4"
                     },
                      {
                          name: "wxdetilperlengkapan",
                          cls: "span4",
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
        SimDms.Angular("SalesPerlengkapanOut");
    }



});