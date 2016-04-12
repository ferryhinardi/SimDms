"use strict";
var pType = "";
function SalesTransBPk($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.uistate = function () {
        switch (me.data.Status) {
            case "": //new
                $("input,button")
                    .prop('disabled', false);

                $("#pnlHeader input ,#pnlKet input,#pnlKet button,#btnApprove, #ShipTo")
                    .prop('disabled', true);
                $("#Remark,#BPKDate").prop('disabled', false);

                $("#Status label").html(me.data.StatusDsc);
                $("#btnDelete,#btnPrintPreview").hide();
                $("input[name='isAll']").prop('disabled', false);
                me.allowEdit = true
                break;
            case "0": //open
                $("#Status label").html(me.data.StatusDsc);
                //$("#Remark").prop('disabled', true);
                //$("#pnlHeader button").prop('disabled', true);
                $("#pnlKet button,#detRemark").prop('disabled', false)
                $("#btnDelete,#btnPrintPreview").show();
                $("input[name='StatusPDI']").prop('disabled', false);
                $("#btnAddDetail,#btnRemDetail,#btnNewDetail").show();
                me.allowEdit = true
                break;
            case "1": //printed
                $("#Status label").html(me.data.StatusDsc);
                $("#Remark").prop('disabled', true);
                $("#pnlHeader button").prop('disabled', true);
                $("#pnlKet button,#detRemark,#btnApprove").prop('disabled', false)
                $("#btnDelete,#btnPrintPreview").show();
                $("input[name='StatusPDI']").prop('disabled', false);
                me.allowEdit = true
                break;
            case "2": //approved
                $("#Status label").html(me.data.StatusDsc);
                $('#btnApprove').prop('disabled', true);
                $("#btnPrintPreview").show();
                $("#btnSave,#btnAddDetail,#btnRemDetail,#btnNewDetail,#btnDelete").hide();
                $("form button, form input").prop('disabled', true);
                me.allowEdit = false
                break;
            case "3": //canceled
                $("#Status label").html(me.data.StatusDsc);
                $("form input,form button").prop('disabled', true);
                $("#btnDelete,#btnPrintPreview").hide();
                $("#btnSave,#btnAddDetail,#btnRemDetail,#btnNewDetail").hide();
                me.allowEdit = false
                break;
            case "9":
                break;
            default: break;
        }
    }


    me.save = function () {
        $http.post("om.api/SalesBPK/Save", me.data)
            .success(function (rslt) {
                if (rslt.success == true) {
                    me.data.BPKNo = rslt.BPKNo;
                    me.data.Status = rslt.Status;
                    me.data.StatusDsc = rslt.StatusDsc;
                    me.uistate();
                    me.Apply();
                    MsgBox("Data Saved");
                    if (me.data.isAll) {
                        $http.post("om.api/Grid/SlsBPKDtl", { BPKNo: me.data.BPKNo })
                        .success(function (rslt) {
                            me.data.grid = rslt;
                            me.loadTableData(me.griddetilbpk, me.data.grid);
                            me.griddetilbpk.adjust();
                        });
                        $("#btnNewDetail").click();
                        me.Apply();
                    }
                }
                else {
                    if (rslt.browse != undefined) {
                        me.Apply();
                        if (rslt.browse == "so") {
                            $("#btnSONo").click();
                        }
                        else {
                            $("#btnWareHouseCode").click();
                        }
                    }
                    else {
                        MsgBox(rslt.message, MSG_ERROR);
                    }
                }
            });
    }

    me.delete = function () {
        $http.post("om.api/SalesBPK/Delete", { BPKNo: me.data.BPKNo })
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
        $http.post("om.api/SalesBPK/Approve", { BPKNo: me.data.BPKNo, BPKDate: me.data.BPKDate, DONo: me.data.DONo, SONo: me.data.SONo })
            .success(function (rslt) {
                if (rslt.success == true) {
                    me.data.Status = rslt.Status;
                    me.data.StatusDsc = rslt.StatusDsc;
                    me.uistate();
                    Wx.Success(rslt.message);
                }
                else {
                    MsgBox(rslt.message, MSG_ERROR);
                }
            });
    }



    me.newdetail = function () {
        me.data.detail.DOSeq = "";
        me.data.detail.SalesModelCode = "";
        me.data.detail.SalesModelYear = "";
        me.data.detail.SalesModelDesc = "";
        me.data.detail.ChassisCode = "";
        me.data.detail.ChassisNo = "";
        me.data.detail.EngineCode = "";
        me.data.detail.EngineNo = "";
        me.data.detail.ColourCode = "";
        me.data.detail.RefferenceDesc1 = "";
        me.data.detail.Remark = "";
        me.griddetilbpk.clearSelection();
        me.Apply();

    }

    me.adddetail = function () {
        me.Apply();
        if (me.data.detail.SalesModelCode == "") {
            // me.Apply();
            $("#btnSalesModelCode").click();
            return;
        }

        if (me.data.detail.SalesModelYear == "") {
            //me.Apply();
            $("#btnSalesModelYear").click();
            return;
        }

        if (me.data.detail.ChassisNo == "") {

            //$("#btnChassisNo").click();
            //return;
        }



        var data = me.data.detail;
        data.BPKNo = me.data.BPKNo;
        console.log(me.data.detail);
        $http.post("om.api/SalesBPk/SaveDetail", data)
          .success(function (rslt) {
              if (rslt.success == true) {
                  me.data.detail.BPKSeq = rslt.BPKSeq;
                  me.uistate();
                  me.Apply();
                  MsgBox("Data Saved");
                  $http.post("om.api/Grid/SlsBPKDtl", { BPKNo: me.data.BPKNo })
                    .success(function (rslt) {
                        me.data.grid = rslt;
                        me.loadTableData(me.griddetilbpk, me.data.grid);
                        me.griddetilbpk.adjust();
                    });
                  $("#btnNewDetail").click();
              }
              else {
                  if (rslt.browse != undefined) {
                      me.Apply();
                      if (rslt.browse == "so") {
                          $("#btnSONo").click();
                      }
                      else {
                          $("#btnWareHouseCode").click();
                      }
                  }
                  else {
                      MsgBox(rslt.message, MSG_ERROR);
                  }
              }
          });
        
    }

    me.deletedetail = function () {
        if (me.data.detail.BPKSeq != "") {
            $http.post("om.api/SalesBPK/DeleteDetail", { BPKNo: me.data.BPKNo, BPKSeq: me.data.detail.BPKSeq })
          .success(function (rslt) {
              if (rslt.success == true) {
                  $http.post("om.api/Grid/SlsBPKDtl", { BPKNo: me.data.BPKNo })
                    .success(function (rslt) {
                        me.data.grid = rslt;
                        me.loadTableData(me.griddetilbpk, me.data.grid);
                        me.griddetilbpk.adjust();
                    });
                  Wx.Success("Detail berhasil di delete!");
                  me.newdetail();
                  me.uistate();
                  me.Apply();
              }
              else {
                  MsgBox(rslt.message, MSG_ERROR);
              }
          });
        }
    }



    me.printPreview = function () {
        $http.post("om.api/SalesBPK/PrintBPK", { BPKNo: me.data.BPKNo,Status:me.data.Status })
        .success(function (rslt) {
            if (rslt.success == true) {
                me.data.Status = rslt.Status;
                me.data.StatusDsc = rslt.StatusDsc;
                me.data.Flag = rslt.Flag;
                me.uistate();
                me.printDialog();
                pType = rslt.pType;

            }
            else {
                MsgBox(rslt.message, MSG_ERROR);
            }
        });
    }

    me.printDialogOld = function () {
        BootstrapDialog.show({
            message: $(
'<div class="container">' +
 ' <div class="dlg1">'+
  '  <div class="row">'+
  '    <input type="radio" checked="" value="0" id="PrintType1" name="PrintType">&nbsp; Pre-Printed'+
   ' </div>  '+
    '<div class="row">'+
     ' <input type="radio" value="1" id="PrintType2" name="PrintType">&nbsp; Formating'+
    '</div>'+
    '<div class="row">'+
      '<input type="radio" value="2" id="PrintType3" name="PrintType">&nbsp; Surat Jalan Sheet'+
    '</div>'+    
  '</div>'+
  '<div class="dlg2 hide">' +
     '<div class="row">'+
     '<input type="radio" checked="" value="3" id="PrintType4" name="PrintType2">&nbsp; Pre-Printed'+
    '</div>  '+
    '<div class="row">'+
      '<input type="radio" value="4" id="PrintType5" name="PrintType2">&nbsp; Formating'+
    '</div>    '+
  '</div>' +
  '<div class="dlg3 hide">' +
     '<div class="row">' +
     '<input type="radio" checked="" value="0" id="PrintType6" name="pagetype">&nbsp;  Print Satu Halaman' +
    '</div>  ' +
    '<div class="row">' +
      '<input type="radio" value="1" id="PrintType7" name="pagetype">&nbsp; Print Setengah Halaman' +
    '</div>    ' +
  '</div>' +
'</div>'
                ),
            closable: false,
            draggable: true,
            type: BootstrapDialog.TYPE_INFO,
            title: 'Print',
            buttons: [{
                label: ' Print',
                cssClass: 'btn-primary icon-print',
                action: function (dialogRef) {

                    console.log(dialogRef);
                    me.data.Print = {};
                    if (!$("div.dlg1").hasClass('hide')) {
                        me.data.Print.prtype = $("input[name='PrintType']:checked").val();
                        if (me.data.Print.prtype == "2") {
                            $("div.dlg1").addClass('hide');
                            $("div.dlg3").removeClass('hide');
                            //console.log("Print Surat Jalan");
                        }
                        else {
                            $("div.dlg1").addClass('hide');
                            $("div.dlg3").removeClass('hide');
                        }
                    }
                    else if (!$("div.dlg2").hasClass('hide')) {
                        $("div.dlg2").addClass('hide');
                        $("div.dlg3").removeClass('hide');
                        me.data.Print.prtype = $("input[name='PrintType2']:checked").val();
                    }
                    else {
                        me.data.Print.pgtype = $("input[name='pagetype']:checked").val();

                    }
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
    }

    me.printDialog = function () {
        BootstrapDialog.show({
            message: $(
'<div class="container">' +
 ' <div class="dlg1">' +
  '  <div class="row">' +
  '    <input type="radio" checked="" value="0" id="PrintType1" name="PrintType">&nbsp; Pre-Printed' +
   ' </div>  ' +
    '<div class="row">' +
     ' <input type="radio" value="1" id="PrintType2" name="PrintType">&nbsp; Formating' +
    '</div>' +
    '<div class="row">' +
      '<input type="radio" value="2" id="PrintType3" name="PrintType">&nbsp; Surat Jalan Sheet' +
    '</div>' +
  '</div>' +
  '<div class="dlg3 hide">' +
     '<div class="row">' +
     '<input type="radio" checked="" value="0" id="PrintType6" name="pagetype">&nbsp;  Print Satu Halaman' +
    '</div>  ' +
    '<div class="row">' +
      '<input type="radio" value="1" id="PrintType7" name="pagetype">&nbsp; Print Setengah Halaman' +
    '</div>    ' +
  '</div>' +
'</div>'
                ),
            closable: false,
            draggable: true,
            type: BootstrapDialog.TYPE_INFO,
            title: 'Print',
            buttons: [{
                label: ' Print',
                cssClass: 'btn-primary icon-print',
                action: function (dialogRef) {
                    me.data.Print = {};
                    me.data.Print.prtype = $("input[name='PrintType']:checked").val();
                    if (me.data.Print.prtype == 2) {
                        if (!$("div.dlg1").hasClass('hide')) {
                            $("div.dlg1").addClass('hide');
                            $("div.dlg3").removeClass('hide');
                        }
                        else {
                            me.Print();
                            dialogRef.close();
                        }
                    }
                    else {
                        me.Print();
                        dialogRef.close();
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
    }

    me.Print = function () {
        var sizeType = $('input[name=pagetype]:checked').val();
        var ReportId = '';
        var par = [me.data.BPKNo, me.data.BPKNo, "profitcenter"];

        if (me.data.Print.prtype == "0") {
            ReportId = 'OmRpSalesTrn003A';
        }
        else if (me.data.Print.prtype == "1") {
            if (me.data.Flag == "") {
                if (pType == "4W") {
                    console.log("4W");
                    ReportId = 'OmRpSalesTrn003D';
                }
                else {
                    console.log("2W");
                    ReportId = 'OmRpSalesTrn003DR2';
                }
            }
            else
            {
                ReportId = 'OmRpSalesTrn003D' + me.data.Flag;
            }
        }
        else if (me.data.Print.prtype == "2")
        //else if (me.data.Print.prtype == "3" && me.data.Print.prtype == "4") 
        {
            par = [me.data.BPKNo, me.data.BPKNo, "profitcenter",""];
            if (sizeType == 0) {
                ReportId = 'OmRpSalesTrn003B';
            }
            else {
                ReportId = 'OmRpSalesTrn003C';
            }
        }

        var rparam = ['','','producttype']
        console.log(ReportId);
        Wx.showPdfReport({
            id: ReportId,
            pparam: par,
            rparam: (me.data.Print.prtype == "1") ? rparam : '',
            textprint:true,
            type: "devex"
        });
    }


    me.browse = function () {
        var lookup = Wx.klookup({
            name: "BrowseBPKLkp",
            title: "BPK",
            url: "om.api/grid/SlsBPKBrowse",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "BPKNo", title: "No.BPK", width: 110 },
                { field: "TypeSales", title: "Tipe", width: 130 },
                {
                    field: "BPKDate", title: "Tgl BPK", width: 110,
                    template: "#= (BPKDate == undefined) ? '' : moment(BPKDate).format('DD MMM YYYY') #"
                },
                { field: "SONo", title: "No SO", width: 130 },
                { field: "SKPKNo", title: "No. SKPK", width: 150 },
                { field: "RefferenceNo", title: "No. Reff", width: 130 },
                { field: "CustomerName", title: "Pelanggan", width: 200 },
                { field: "Address", title: "Alamat", width: 400 },
                { field: "ShipToDsc", title: "Kirim Ke", width: 300 },
                { field: "WareHouseCode", title: "Gudang", width: 100 },
                { field: "ExpeditionDsc", title: "Ekspedisi", width: 100 },
                { field: "StatusDsc", title: "Status", width: 110 }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.BPKNo = data.BPKNo;
            me.data.BPKDate = data.BPKDate;
            me.data.SONo = data.SONo;
            me.data.DONo = data.DONo;
            me.data.CustomerCode = data.CustomerCode;
            me.data.CustomerName = data.CustomerName;
            me.data.Address = data.Address;
            me.data.ShipTo = data.ShipTo;
            me.data.ShipToDsc = data.ShipToDsc;
            me.data.WareHouseCode = data.WareHouseCode;
            me.data.WrhDsc = data.WrhDsc;
            me.data.Expedition = data.Expedition;
            me.data.ExpeditionDsc = data.ExpeditionDsc;
            me.data.Status = data.Status;
            me.data.StatusDsc = data.StatusDsc;
            me.data.Remark = data.Remark;

            $http.post("om.api/grid/SlsBpkDtl", { BPKNo: me.data.BPKNo })
            .success(function (rslt, status, headers, config) {
                me.data.grid = rslt;
                me.loadTableData(me.griddetilbpk, me.data.grid);
                me.griddetilbpk.adjust();                
                me.uistate();

            });

            
        });
    }
    

    me.dolkp = function () {
        var lookup = Wx.klookup({
            name: "lookupSO",
            title: "DO",
            url: "om.api/Grid/SlsBpkLkpDO",
            serverBinding: true,
            pageSize: 10,
            columns: [
                 { field: "DONo", title: "No.DO", width: 150 },
                 { field: "SalesTypeDsc", title: "Tipe", width: 150 },
                 { field: "DODate", title: "Tgl. DO", width: 150, template: "#= DODate==null ? '' : moment(DODate).format('DD MMM YYYY') #" },
                 { field: "SKPKNo", title: "No.SKPK", width: 150 },
                 { field: "RefferenceNo", title: "No.Reff", width: 150 },
                 { field: "CustomerCode", title: "Kode", width: 150 },
                 { field: "CustomerName", title: "Pelanggan", width: 250 },
                 { field: "Address", title: "Alamat", width: 500 },
                 { field: "ShipTo", title: "Kode Kirim", width: 150 },
                 { field: "ShipName", title: "Kirim Ke", width: 500 }
            ]
        });

        lookup.dblClick(function (data) {
            me.data.DONo = data.DONo;
            me.data.SONo = data.SONo;
            me.data.CustomerName = data.CustomerName;
            me.data.CustomerCode = data.CustomerCode;
            me.data.WareHouseCode = data.WareHouseCode;
            me.data.WareHouseName = data.WareHouseName;
            me.data.ShipTo = data.ShipTo;
            me.data.ShipToDsc = data.ShipName;
            me.data.Expedition = data.Expedition;
            me.data.ExpeditionDsc = data.ExpeditionName;
            me.data.Remark = data.Remark;
            //$('#ShipTo').attr('disabled', 'disabled');
            //$('#WareHouseCode').attr('disabled', 'disabled');
            //$('#Expedition').attr('disabled', 'disabled');
            me.Apply();
        });
    }

    $("[name = 'isAll']").on('change', function () {
        me.data.isAll = $('#isAll').prop('checked');
        me.Apply();
        //console.log(me.data.isAll);
    });

    //me.Shiptolkp = function () {
    //    var lookup = Wx.klookup({
    //        name: "lookupShipto",
    //        title: "Shipto",
    //        url: "om.api/Grid/slsdolkpshipto",
    //        serverBinding: true,
    //        pageSize: 10,
    //        columns: [
    //             { field: "CustomerCode", title: "Customer Code", width: 150 },
    //             { field: "CustomerName", title: "Customer Name" }]
    //    });

    //    lookup.dblClick(function (data) {
    //        me.data.ShipTo = data.CustomerCode;
    //        me.data.ShipToDsc = data.CustomerName;
    //        me.Apply();
    //    });
    //}

    me.Shiptolkp = function () {
        var lookup = Wx.klookup({
            name: "lookupShipto",
            title: "Shipto",
            url: "om.api/Grid/SlsDoLkpShiptoV2?cols=" + 5,
            serverBinding: true,
            pageSize: 10,
            columns: [
                 { field: "CustomerCode", title: "Customer Code", width: 150 },
                 { field: "CustomerName", title: "Customer Name" }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.ShipTo = data.CustomerCode;
            me.data.ShipToDsc = data.CustomerName;
            me.Apply();
        });
    }

    me.WrhLkp = function () {
        var lookup = Wx.klookup({
            name: "lkpWareHouseCode",
            title: "WareHouse",
            url: "om.api/grid/WarehouseList",
            serverBinding: true,
            columns: [
                { field: "LookUpValue", title: "Kode Gudang", width: 100 },
                { field: "LookUpValueName", title: "Nama Gudang", width: 200 },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.WareHouseCode = data.LookUpValue;
            me.data.WareHouseName = data.LookUpValueName;
            me.Apply();
        });
    }

    me.expdtionlkp = function () {
        var lookup = Wx.klookup({
            name: "lkpExpeditionCode",
            title: "Expedition",
            url: "om.api/grid/slsdolkpexpedition",
            serverBinding: true,
            columns: [
                { field: "SupplierCode", title: "Kode", width: 150 },
                { field: "SupplierName", title: "Deskripsi" }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.Expedition = data.SupplierCode;
            me.data.ExpeditionDsc = data.SupplierName;
            me.Apply();
        });
    }

    me.slsmdlcode = function () {
        var lookup = Wx.klookup({
            name: "salesmodelcodegrd",
            title: "Sales Model",
            url: "om.api/grid/SlsBPKLkpSlsMdlCd",
            params: { DONo: me.data.DONo },
            serverBinding: true,
            columns: [
                { field: "SalesModelCode", title: "Kode", width: 150 },
                { field: "SalesModelDesc", title: "Deskripsi" }
            ]
        });

        lookup.dblClick(function (data) {
            me.data.detail.SalesModelCode = data.SalesModelCode;
            me.Apply();
        });
    }


    me.slsmdlyear = function () {
        var lookup = Wx.klookup({
            name: "salesmodelcodegrd",
            title: "Sales Model",
            url: "om.api/grid/SlsBPKLkpSlsMdlYear",
            params: { DONo: me.data.DONo, SalesModelCode: me.data.detail.SalesModelCode },
            serverBinding: true,
            columns: [
                { field: "SalesModelYear", title: "Kode", width: 150 },
                { field: "SalesModelDesc", title: "Deskripsi" }
            ]
        });

        lookup.dblClick(function (data) {
            me.data.detail.SalesModelYear = data.SalesModelYear;
            me.data.detail.SalesModelDesc = data.SalesModelDesc;
            me.data.detail.ChassisCode = data.ChassisCode;
            me.Apply();
        });
    }

    me.chassisnolkp = function () {
        var lookup = Wx.klookup({
            name: "salesmodelcodegrd",
            title: "Sales Model",
            url: "om.api/grid/SlsBPKLkpChasisNo",
            params: {
                DONo: me.data.DONo,
                BPKNo: me.data.BPKNo,
                SalesModelCode: me.data.detail.SalesModelCode,
                SalesModelYear: me.data.detail.SalesModelYear,
                ChassisCode: me.data.detail.ChassisCode,
                WareHouseCode: me.data.WareHouseCode
            },
            serverBinding: true,
            columns: [
                { field: "ChassisNo", title: "Chassis No", width: 150 },
                { field: "EngineNo", title: "Engine No" }
            ]
        });

        lookup.dblClick(function (data) {
            //console.log(data);
            me.data.detail.ChassisNo = data.ChassisNo;
            me.data.detail.EngineCode = data.EngineCode;
            me.data.detail.EngineNo = data.EngineNo;
            me.data.detail.ColourCode = data.ColourCode;
            me.data.detail.RefferenceDesc1 = data.RefferenceDesc1;
            me.Apply();
        });
    }

    $("#StatusPDIN").on('change', function (e) {
        me.data.detail.StatusPDI == false;
        me.Apply();
    });
    $("#StatusPDIY").on('change', function (e) {
        me.data.detail.StatusPDI == true;
        me.Apply();
    });

    me.griddetilbpk = new webix.ui({
        container: "wxdetildo",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 150 },
            { id: "SalesModelYear", header: "Sales Model Year" },
            { id: "SalesModelDesc", header: "Sales Model Desc", width: 150 },
            { id: "ChassisCode", header: "Kode Rangka", width: 150 },
            { id: "ChassisNo", header: "NO. Rangka" },
            { id: "EngineCode", header: "Kode Mesin" },
            { id: "EngineNo", header: "No Mesin" },
            { id: "ColourCode", header: "Kode Warna" },
            { id: "RefferenceDesc1", header: "Nama Warna", width: 200 },
            { id: "StatusPDI", header: "PDI", width: 200 },
            { id: "Remark", header: "Keterangan", fillspace: true }
        ],
        on: {
            onSelectChange: function () {
                if (me.griddetilbpk.getSelectedId() !== undefined) {
                    var griddata = this.getItem(me.griddetilbpk.getSelectedId());
                    me.data.detail.SalesModelCode = griddata.SalesModelCode;
                    me.data.detail.SalesModelYear = griddata.SalesModelYear;
                    me.data.detail.SalesModelDesc = griddata.SalesModelDesc;
                    me.data.detail.ChassisCode = griddata.ChassisCode;
                    me.data.detail.ChassisNo = griddata.ChassisNo;
                    me.data.detail.EngineCode = griddata.EngineCode;
                    me.data.detail.EngineNo = griddata.EngineNo;
                    me.data.detail.ColourCode = griddata.ColourCode;
                    me.data.detail.RefferenceDesc1 = griddata.RefferenceDesc1;
                    me.data.detail.StatusPDI = (griddata.StatusPDI == "0") ? false : true;
                    me.data.detail.detRemark = griddata.Remark;
                    me.data.detail.BPKSeq = griddata.BPKSeq;
                    me.Apply();
                    console.log(me.data.detail);
                }
            }
        }
    });

    webix.event(window, "resize", function () {
        me.griddetilbpk.adjust();
    })



    me.initialize = function () {
        $('#BPKDate').attr('onkeydown', 'return false');

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
        me.data.detail = {};
        me.data.detail.SalesModelCode = "";
        me.data.detail.SalesModelYear = "";
        me.data.detail.ChassisNo = "";
        me.data.grid = [];
        me.data.BPKDate = moment().format();
        me.data.isAll = false;


        $http.post("om.api/grid/WarehouseList")
       .success(function (data) {
           if (data.data.length > 0) {
               me.data.WareHouseCode = data.data[0].LookUpValue;
               me.data.WareHouseName = data.data[0].LookUpValueName;
               me.isLoadData = false;

               //me.Apply();
           }
       });

        me.griddetilbpk.adjust();
        me.griddetilbpk.clearAll();
        me.uistate();
        $("#btnDelete,#btnPrintPreview").removeClass('ng-hide');
    };


    me.start();
}



$(document).ready(function () {
    var options = {
        title: "BPK",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlHeader",
                items: [
                    { name: "BPKNo", text: "No.BPK", model: "data.BPKNo", cls: "span3", placeHolder: "BPK/YY/XXXXXX", disabled: true },
                    {
                        name: "DODateCtrl", type: "controls", text: "Tgl. BPK", cls: "span5",
                        items: [
                            { name: "BPKDate", model: "data.BPKDate", type: "ng-datepicker", cls: "span3" },
                            {
                                type: "buttons", cls: "span2 left", items: [
                                   { name: "btnApprove", text: "Approve", cls: "btn-small btn-info", icon: "icon-ok", click: "approve()", disable: "me.statusBPU === NEW || me.statusBPU === APPROVED" }
                                ]
                            },
                            { name: "Status", text: "Open", cls: "span2", readonly: true, type: "label" },
                        ]
                    },
                    { name: "DONo", model: "data.DONo", text: "No.DO ", type: "popup", cls: "span3", click: "dolkp()" },
                    { name: "isAll", model: "data.isAll", text: "Pilih Semua", type: "check", cls: "span2" },
                    { name: "SONo", model: "data.SONo", text: "No. SO", cls: "span3" },
                    { name: "CustomerName", model: "data.CustomerName", text: "Pelanggan", cls: "span8" },
                    {
                        name: "kirimke",
                        type: "controls",
                        text: "Kirim Ke",
                        items: [
                            { name: "ShipTo", model: "data.ShipTo", type: "popup", cls: "span3", click: "Shiptolkp()" },
                            { name: "ShipToDsc", model: "data.ShipToDsc", cls: "span5" },
                        ]
                    },
                    {
                        name: "gudng",
                        type: "controls",
                        text: "Gudang",
                        items: [
                            { name: "WareHouseCode", model: "data.WareHouseCode", type: "popup", cls: "span3", click: "WrhLkp()", disable: "true" },
                            { name: "WareHouseName", model: "data.WareHouseName", cls: "span5" }
                        ]
                    },
                    {
                        name: "expedisi",
                        type: "controls",
                        text: "Expedisi",
                        items: [
                            { name: "Expedition", model: "data.Expedition", type: "popup", cls: "span3", click: "expdtionlkp()" },
                            { name: "ExpeditionDsc", model: "data.ExpeditionDsc", cls: "span5" }
                        ]
                    },
                    { name: "Remark", model: "data.Remark", text: "Keterangan", cls: "span8" }
                ],
            },
            {
                name: "pnlKet",
                title: "Detil BPK",
                items: [
                     { name: "slsmdlcode",model:"data.detail.SalesModelCode", text: "Sales Model Code", type: "popup", cls: "span3", click: "slsmdlcode()" },
                     {
                         name: "expedisi",
                         type: "controls",
                         text: "Sales Model Year",
                         items: [
                             { name: "SalesModelYear", model: "data.detail.SalesModelYear", type: "popup", cls: "span2", click: "slsmdlyear()" },
                             { name: "SalesModelDesc", model: "data.detail.SalesModelDesc", cls: "span3" }
                         ]
                     },
                     {
                         name: "expedisi",
                         type: "controls",
                         text: "Kode/No Rangka",
                         items: [

                             { name: "ChassisCode", model: "data.detail.ChassisCode", cls: "span3" },
                             { name: "ChassisNo", model: "data.detail.ChassisNo", type: "popup", cls: "span2", click: "chassisnolkp()" }
                         ]
                     },
                      {
                          name: "expedisi",
                          type: "controls",
                          text: "Kode/No Mesin",
                          items: [

                              { name: "EngineCode", model: "data.detail.EngineCode", cls: "span3" },
                              { name: "EngineNo", model: "data.detail.EngineNo", cls: "span2" }
                          ]
                      },
                      //{ name: "RefferenceDesc1", model: "data.detail.RefferenceDesc1", text: "warna", cls: "span4" },
                      {
                          name: "ctrlColour",
                          type: "controls",
                          text: "Warna",
                          items: [
                                { name: "ColourCode", model: "data.detail.ColourCode", cls: "span3" },
                                { name: "RefferenceDesc1", model: "data.detail.RefferenceDesc1", cls: "span5" },
                          ]
                      },
                      { name: "StatusPDI", model: "data.detail.StatusPDI", text: "is PDI", type: "x-switch", cls: "span2" },
                      { name: "detRemark", model: "data.detail.detRemark", text: "Keterangan", cls: "span6" },
                      {
                          type: "buttons", cls: "span4 left", items: [
                              { name: "btnAddDetail", text: "Save", cls: "btn-small btn-info", icon: "icon-ok", click: "adddetail()", disable: 'me.data.Status ===""' },
                              { name: "btnRemDetail", text: "Delete", cls: "btn-small btn-info", icon: "icon-ok", click: "deletedetail()", disable: 'me.data.Status === ""' },
                              { name: "btnNewDetail", text: "New", cls: "btn-small btn-info", icon: "icon-ok", click: "newdetail()", disable: 'me.data.DOSeq ===""' }
                          ]
                      },
                      {
                          name: "wxdetildo",
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
        SimDms.Angular("SalesTransBPk");
    }





});