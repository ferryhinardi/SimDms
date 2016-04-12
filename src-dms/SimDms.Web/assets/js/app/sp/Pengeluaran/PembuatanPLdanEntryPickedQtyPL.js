var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spPembuatanPLdanEntryPickedQtyPLController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/GetLookupByCodeID', { CodeID: "SLTP" }).
    success(function (data, status, headers, config) {
        me.comboSalesType = data;
    });

    me.$watch('data.TransType', function (e) {
        if (me.isDisable == true) {
            me.isTransfer = false;
            me.data.isExternal = false;
        }
        else {
            if (e == 10) {
                me.isTransfer = true;
            }
            else {
                me.isTransfer = false;
                me.data.isExternal = false;
            }
        }
    });

    me.GetCust = function () {
        var query = "";
        var column = [
            { field: "CustomerCode", title: "Customer Code" },
            { field: "CustomerName", title: "Customer Name" },
            { field: 'Address', title: 'Address' },
            { field: 'ProfitCenter', title: 'Profit Center' }
        ];
        var filter = "CustomerCode asc"
        if (parseInt(me.IsBORelease) === 0) {
            query = "CustPickingListLookupNewOrder";
        }
        if (parseInt(me.IsBORelease) === 1) {
            query = "CustPickingListLookupBackOrder";
        }
        if (me.data.isExternal === true)
        {
            query = new breeze.EntityQuery.from("OpenData").withParameters({ status: '1' });
                filter = "BranchCode asc"
                var column = [
                    { field: "CompanyName", title: "Company Name" },
                    { field: "BranchCode", title: "Branch Code" },
                    { field: "BranchName", title: "Branch Name" },
                ];
        }
        
        if (query !== "") {
            var lookup = Wx.blookup({
                name: "CustPickingListLookup",
                title: "Pencarian Pelanggan",
                manager: me.data.isExternal === true ? pengeluaranManager : spManager,
                query: query,
                defaultSort: filter,
                columns: column
            });
            lookup.dblClick(function (result) {
                if (result != null) {
                    if (me.data.isExternal == true) {
                        me.data.CustomerCode = result.BranchCode;
                        me.data.CustomerName = result.BranchCode;
                    }
                    else {
                        me.data.CustomerCode = result.CustomerCode;
                        me.data.CustomerName = result.CustomerName;
                    }
                
                    me.Apply();
                    //me.lookupAfterSelect(result);
                    me.Apply();
                    $('#pnlB, #pnlC').hide();
                }
            });
        }
    }

    me.browse = function () {
        me.isBrowse = true;
        var query = "";
        if (parseInt(me.IsBORelease) === 0) {
            query = "BrowsePickingHdrBackOrder";
        }
        else if (parseInt(me.IsBORelease) === 1) {
            query = "BrowsePickingHdrNewOrder";
        }
        if (query != "") {
            var lookup = Wx.blookup({
                name: "CustPickingListLookup",
                title: "Pencarian No. Picking List",
                manager: spManager,
                query: query,
                defaultSort: "PickingSlipDate desc",
                columns: [
                { field: "PickingSlipNo", title: "Picking Sli pNo", width:100 },
                { field: "PickingSlipDate", title: "Picking Slip Date", template: "#= (PickingSlipDate == undefined) ? '' : moment(PickingSlipDate).format('DD MMM YYYY') #", width:100 },
                { field: "CustomerCode", title: "Customer Code", width:100 },
                { field: "CustomerName", title: "Customer Name", width:300 },
                ]
            });
            lookup.dblClick(function (result) {
                if (result != null) {
                    me.startEditing();
                    $('#pnlB, #pnlC').show();
                    $('#btnDelete').hide();
                    me.SalesTypeDDL(result.SalesType);
                    me.isDisable = true;                    
                    setTimeout(function () { me.data = result; me.detail2.Remark = result.Remark;}, 200);
                    setTimeout(function () { me.FindOrderDetail(true); }, 300);
                    setTimeout(function () { me.FindPartDetails(true); }, 2000);
                    me.CheckPickStatus(result.PickingSlipNo);
                    me.startEditing();
                }
            });
        }
    }

    me.GetPickedBy = function () {
        var lookup = Wx.blookup({
            name: "EmployeePickedByLookup",
            title: "Pencarian Karyawan",
            manager: spManager,
            query: "EmployeePickedByLookup",
            defaultSort: "EmployeeID asc",
            columns: [
            { field: "EmployeeID", title: "ID Karyawan" },
            { field: "EmployeeName", title: "Nama Karyawan" }
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.PickedBy = result.EmployeeID;
                me.data.PickedByName = result.EmployeeName;
                me.Apply();
                //me.lookupAfterSelect(result);
                me.Apply();
                //me.loadDetail(result);
            }
        });
    }
         
    me.saveData = function (e, param) {
        $http.post('sp.api/TargetPenjualan/save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.FindOrderDetail = function (isBrowse) {
        me.griddetail = [];
        me.grid1.detail = {};
        me.clearTable(me.grid1);
        me.grid1.adjust();
        me.grid2.adjust();

        var url = "";
        me.isBrowse = isBrowse;
        console.log(me.IsBORelease);
        me.data.IsBORelease = me.IsBORelease == "1" ? true : false;

        if (isBrowse) {
            url = 'sp.api/EntryPickedList/GetPickingHdr';
        } else {
            url = 'sp.api/EntryPickedList/GetCustomerOrderDtl';
        }
        $http.post(url, me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    $('#pnlB').show();
                    if (me.isBrowse) {
                        $.each(data.data, function (key, value) {
                            me.griddetail.push(value.DocNo);
                            console.log(value.DocNo);
                            console.log(me.griddetail);
                        });
                    }

                    me.grid1.detail = data.data;
                    me.loadTableData(me.grid1, me.grid1.detail);
                    me.grid1.adjust();

                } else {
                    MsgBox(data.message);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });

        if (isBrowse && me.data.SalesType == 0) {
            me.grid1.hideColumn("CustomerCode");
            me.grid1.showColumn("PaymentName");
        } else {
            me.grid1.hideColumn("PaymentName");
            me.grid1.showColumn("CustomerCode");
        }

        
        if (me.IsBORelease == '0') {
            me.grid1.hideColumn("OrderNo");
            me.grid1.hideColumn("OrderDate");
            me.grid1.hideColumn("PaymentName");

            me.grid1.showColumn("CustomerCode")
            me.grid1.showColumn("ReferenceNo")
            me.grid1.showColumn("ReferenceDate")
        }
        else
        {
            me.grid1.showColumn("OrderNo");
            me.grid1.showColumn("OrderDate");
            me.grid1.showColumn("PaymentName");

            me.grid1.hideColumn("CustomerCode")
            me.grid1.hideColumn("ReferenceNo")
            me.grid1.hideColumn("ReferenceDate")
        }

    }

    me.helpInvoice = function () {
        $http.post('sp.api/entrypickedlist/HelpInsertInvoice')
       .success(function (e) {
           if (e) {
               Wx.Success('yuhu');
           } else {
               MsgBox('bwee', MSG_ERROR);
           }
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
       });
    }
    
            
    me.initialize = function()
    {
        //me.helpInvoice();
        me.clearTable(me.grid1);
        me.clearTable(me.grid2);

        $http.post('sp.api/EntryPickedlist/DefaultNonSales')
        .success(function (e) {
          me.isTrex = e.isTrex;
        });

        me.isTransfer = false;
        me.data.isExternal = false;

        me.detail = {};
        me.detail2 = {};
        me.griddetail = [];
        $('#pnlB, #pnlC').hide();
        me.DisablePL = true;
        me.DisableBill = true;
        me.DisablePrint = true;
        me.DisableSvPrt = true;
        me.DisableClrPrt = true;
        me.isDisable = false;
        me.isBrowse = false;
        me.isPrintAvailable = false;
        //me.isPrintAvailable = true;
        //me.IsBORelease = "0";
        $http.post('sp.api/EntryPickedList/Default').
          success(function (dl, status, headers, config) {
              me.data.PickingSlipDate = dl.PickingSlipDate;
          });
        //me.data.PickingSlipDate = me.now();
        $('#StatusPL').html("");
    }

    function custom_checkbox(obj, common, value) {
        if (me.isBrowse) {
            value = me.isBrowse;
        }
        var index = me.griddetail.indexOf(obj.DocNo);
        var returnVal = "";
        if (index > -1) {
            me.griddetail.splice(index, 1);
        }

        if (value) {
            me.griddetail.push(obj.DocNo);
            //returnVal = "<div class='webix_table_checkbox'> YES </div>";
            returnVal = '<input type="checkbox" checked="true" class="webix_table_checkbox">';
        }
        else {
            //if (me.griddetail.length === 0) {
            //    me.griddetail.push("-");
            //    //me.FindPartDetails(false);
            //}
            //returnVal = "<div class='webix_table_checkbox'> NO </div>";
            returnVal = '<input type="checkbox" class="webix_table_checkbox">';
        }

        //console.log(index);
        //console.log(value);
        //console.log(obj);
        //console.log(me.griddetail);

        //if (me.griddetail.length >= 1) {
        //}
        return returnVal;
    };

    me.FindPartDetails = function (isBrowse) {
        me.grid2.detail = {};
        me.clearTable(me.grid2);
        me.grid2.adjust();
        //if (me.griddetail.length === 1) {
        //    MsgBox("Silahkan Pilih Data Customer Order Detail", MSG_ERROR);
        //}
        //else {
            if (isBrowse) {
                url = 'sp.api/EntryPickedList/GetPickingDtl';
            } else {
                url = 'sp.api/EntryPickedList/GetPartOrderDetail';
            }
            
            if (me.IsBORelease === "1") {
                $('#btnGenPL').html("BO Release");
            } else {
                $('#btnGenPL').html("Generate PL");
            }
            $http.post(url, { model: me.data, DocNoList: me.griddetail }).
            success(function (data, status, headers, config) {
                if (data.success) {
                    $('#pnlC').slideDown();
                    me.grid2.detail = data.data;
                    me.loadTableData(me.grid2, me.grid2.detail);
                    me.grid2.adjust();
                    //var index = me.griddetail.indexOf("-");
                    //if (index > -1) {
                    //    me.griddetail.splice(index, 1);
                    //}
                    if (data.data.length > 0) {
                        me.DisablePL = false;
                    }
                    if (me.isBrowse || data.data.length <= 0) {
                        me.DisablePL = true;
                    }
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        //}
    }

    me.CheckPickStatus = function (PickingSlipNo) {
        $http.post('sp.api/Pengeluaran/CheckStatus', { WhereValue: PickingSlipNo, Table: "spTrnSPickingHdr", ColumnName: "PickingSlipNo" })
        .success(function (v, status, headers, config) {
            if (v.success) {
                $('#StatusPL').html('<span style="font-size:28px;color:red;font-weight:bold">' + v.statusPrint.toUpperCase() + "</span>");
                if (v.statusCode === "0") {
                    me.DisablePL = true;
                    me.DisablePrint = false;
                    me.isPrintAvailable = true;
                    me.DisableBill = true;
                } else if (v.statusCode === "1") {
                    me.DisablePL = true;
                    me.DisablePrint = false;
                    me.isPrintAvailable = true;
                    me.DisableBill = false;
                } else if (v.statusCode === "2" || v.statusCode === "3") {
                    me.DisablePL = true;
                    me.DisablePrint = true;
                    me.isPrintAvailable = false;
                    me.DisableBill = true;
                }
            } else {
                // show an error message
                MsgBox(v.message, MSG_ERROR);
            }
            me.startEditing();
        }).error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.GenPL = function () {
        if (confirm("Apakah Anda Yakin??")) {
            $http.post('sp.api/EntryPickedList/GeneratePL', { model: me.data, DocNoList: me.griddetail, isExternal: me.data.isExternal }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data Picking List Berhasil di Generate, ID=" + data.PickingSlipNo);
                        var pickingSlipNo = data.PickingSlipNo;
                        me.data.PickingSlipNo = data.PickingSlipNo;
                        me.grid1.detail = {};
                        me.grid2.detail = {};
                        me.loadTableData(me.grid1, me.grid1.detail);
                        me.loadTableData(me.grid2, me.grid2.detail);
                        me.grid1.adjust();
                        me.grid2.adjust();
                        me.isBrowse = true;
                        me.isDisable = true;
                        me.CheckPickStatus(pickingSlipNo);
                        me.griddetail = [];
                        //console.log(me.griddetail);
                        //setTimeout(function () { me.GetPL(pickingSlipNo); }, 200);
                        setTimeout(function () { me.FindOrderDetail(true); }, 500);
                        //console.log(me.griddetail);
                        setTimeout(function () { me.FindPartDetails(true); }, 1000);
                        //console.log(me.griddetail);
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
        }
    }

    me.GetPL = function (pickingSlipNo) {
            $http.post('sp.api/EntryPickedList/GetDataPickingHdr', { PickingSlipNo: pickingSlipNo }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.data = data.data;
                        //me.Apply();
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
    }
    
    me.GenBill = function () {
        if (confirm("Apakah Anda Yakin??")) {
            $http.post('sp.api/EntryPickedList/BtnGenerateBill', { model: me.data, DocNoList: me.griddetail }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.isBrowse = false;
                        me.ClearDetails();
                        me.CheckPickStatus(me.data.PickingSlipNo);
                        Wx.Success(data.message);
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
        }
    }

    me.SaveRemark = function () {
        if (confirm("Apakah Anda Yakin??")) {
            $http.post('sp.api/EntryPickedList/UpdateRemarkPickingHdr', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Remark has been saved...");
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        }
    }


    me.grid1 = new webix.ui({
        container: "wxcustorderdetail",
        view:"wxtable", 
        columns: [
            {
                id: "chkSelect",
                header: { content: "masterCheckbox", contentId: "chkSelect" },
                template: custom_checkbox, width: 40,
            },

            { id: "DocNo", header: "No. Dokumen", fillspace: true },
            { id: "DocDate", header: "Tgl. Dokumen", css: 'text-right', fillspace: true, format: me.dateFormat },
            { id: "PaymentName", header: "Cara Pembayaran", fillspace: true },
            { id: "OrderNo", header: "No. Order", fillspace: true },
            { id: "OrderDate", header: "Tgl. Order", fillspace: true, format: me.dateFormat  },
            { id: "CustomerCode", header: "Kode Customer", fillspace: true },
            { id: "ReferenceNo", header: "No. Pemohon", fillspace: true },
            { id: "ReferenceDate", header: "Tgl. Pemohon", fillspace: true, format: me.dateFormat }
        ],
        checkboxRefresh: true,
        on:{
            onSelectChange:function(){
                if (me.grid1.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.grid1.getSelectedId().id);
                    me.detail.oid = me.grid1.getSelectedId();
                    me.Apply();                    
                }
            }
        }          
    });

    me.grid2 = new webix.ui({
        container: "wxpartdetail",
        view: "wxtable", css:"alternating",
        scrollX: true,
        width: 945,
        columns: [
            { id: "DocNo", header: "No. Dokumen", width: 160 },
            { id: "PartNo", header: "No. Part", width: 160 },
            { id: "PartNoOriginal", header: "No. Part Original", width: 160 },
            { id: "ExPickingSlipNo", header: "Ex. No. Picking Slip", width: 160 },
            { id: "QtyPick", header: "Pick Qty.", width: 100, format: me.intFormat, css: 'text-right' },
            { id: "QtyPicked", header: "Picked Qty.", width: 100, format: me.intFormat, css: 'text-right' },
            { id: "QtyBill", header: "Bill Qty.", width: 100, format: me.intFormat, css: 'text-right' },
        ],
        on: {
            onSelectChange: function () {
                if (me.grid2.getSelectedId() !== undefined) {
                    if (me.isBrowse) {
                        var datagrid2 = this.getItem(me.grid2.getSelectedId().id);
                        me.detail.DocNo = datagrid2.DocNo;
                        me.detail.PartNo = datagrid2.PartNo;
                        me.detail.PartNoOriginal = datagrid2.PartNoOriginal;
                        me.detail.ExPickingSlipNo = datagrid2.ExPickingSlipNo;
                        me.detail.QtyPick = datagrid2.QtyPick;
                        me.detail.QtyPicked = datagrid2.QtyPicked;
                        me.detail.QtyBill = datagrid2.QtyBill;
                        me.DisableSvPrt = false;
                        me.DisableClrPrt = false;
                        me.Apply();
                    }
                }
            }
        }
    });

    me.SavePart = function () {
        if (confirm("Apakah Anda Yakin??")) {
            $http.post('sp.api/EntryPickedList/UpdatePickPartDetail', { model: me.detail, PickingSlipNo: me.data.PickingSlipNo }).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Part Order Details has been saved...");
                    me.ClearDetails();
                    me.FindPartDetails(true);
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        }
    }

    me.ClearDetails = function () {
        me.detail.DocNo = undefined;
        me.detail.PartNo = undefined;
        me.detail.PartNoOriginal = undefined;
        me.detail.ExPickingSlipNo = undefined;
        me.detail.QtyPick = 0;
        me.detail.QtyPicked = 0;
        me.detail.QtyBill = 0;
        me.DisableSvPrt = true;
        me.DisableClrPrt = true;
        me.loadTableData(me.grid2, me.grid2.detail);
        me.grid2.adjust();
    }


    me.printPreview = function () {
        $http.post('sp.api/EntryPickedList/UpdatePrintStatus', me.data)
        .success(function (v, status, headers, config) {
            if (v.success) {

                BootstrapDialog.show({
                    message: $(
                        '<div class="container">' +
                            '<div class="row">' +
                                '<p class="col-xs-2 control-label"><b>Ukuran Kertas</b></p>' +
                                '<input type="radio" name="sizeType" id="sizeType1" value="SpRpTrn033" style="cursor: pointer;">&nbsp 1/2 Hal &nbsp&nbsp' +
                                '<input type="radio" name="sizeType" id="sizeType2" value="SpRpTrn033Long" checked style="cursor: pointer;">&nbsp 1 Hal' +
                            '</div>' +
                        '</div>'),
                    closable: false,
                    draggable: true,
                    type: BootstrapDialog.TYPE_INFO,
                    title: 'Print',
                    buttons: [{
                        label: ' Print',
                        cssClass: 'btn-primary icon-print',
                        action: function (dialogRef) {
                            me.printDocument();
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
                // show an error message
                MsgBox(v.message, MSG_ERROR);
            }
        }).error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.printDocument = function () {
        me.CheckPickStatus(me.data.PickingSlipNo);

        var data = me.data.PickingSlipNo + "," + me.data.PickingSlipNo + "," + "profitcenter" + "," + "typeofgoods";
        var rparam = me.data.TransType;

        Wx.showPdfReport({
            id: $('input[name=sizeType]:checked').val(),
            pparam: data,
            rparam: rparam,
            textprint: true,
            type: "devex"
        });

    }
    
    $('#SalesType').on('change', function () {
        var val = $(this).val();
        me.SalesTypeDDL(val);
    });

    me.SalesTypeDDL = function (SalesCode) {
        var codeID = "";
        switch (SalesCode.toString()) {
            case "0":
                codeID = "TTPJ";
                break;
            case "1":
                codeID = "TTNP";
                break;
            case "2":
                codeID = "TTSR";
                break;
            case "3":
                codeID = "TTSL";
                break;
            default:
                break;
        }
        me.comboTransType = {};
        $http.post('sp.api/Combo/GetLookupByCodeID', { CodeID: codeID }).
        success(function (data, status, headers, config) {
            me.comboTransType = data;
        });
    }

    webix.event(window, "resize", function(){ 
        me.grid1.adjust(); 
    })

    me.stdChangedMonitoring = function (n, o) {

            if (me.isLoadData) {
                me.isPrintAvailable = true;
            }

            if (!me.isInProcess) {
                var eq = (n == o);

                // check apakah perubahan data tersebut memiliki nilai atau object kosong (empty object)
                if (!(_.isEmpty(n)) && !eq) {
                    if (!me.hasChanged && !me.isLoadData) {
                        me.isLoadData = false;
                    }
                    if (!me.isSave) {
                        me.isLoadData = false;
                    }
                } else {
                    me.hasChanged = false;
                    me.isSave = false;
                }
            }
    }

    me.$watch('data', function (nVal, oVal) {
        me.isPrintAvailable = true;
        me.isLoadData = true;
        me.isSave = true;
    }, true)

    me.start();
    me.IsBORelease = "0";
}


$(document).ready(function () {
    var options = {
        title: "Picking List and Entry Picked Qty PL",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlPrint",
                title: " ",
                items: [
                { name: "StatusPL", text: "", cls: "span4", readonly: true, type: "label" },
                ]
            },
            {
                name: "pnlA",
                title: "Picking List Detail",
                items: [
                        {
                            type: "optionbuttons",
                            name: "tabpage1",
                            model: "IsBORelease",
                            text: "",
                            items: [
                                { name: "0", text: "Generate PL Dari Order Baru" },
                                { name: "1", text: "Generate PL dari Back Order" },
                            ]
                        },
                        { name: "PickingSlipNo", text: "No. Picking List", cls: "span3", placeHolder: "PLS/XX/YYYYYY", readonly: true },
                        { name: "PickingSlipDate", text: "Tgl. Picking List", type: "ng-datepicker", cls: "span3", placeHolder: "dd-MMM-yyyy", disable:"isDisable" },
                        { name: "SalesType", text: "Asal Dokumen", cls: "span3  number", placeHolder: "0", readonly: true, type: "select2", datasource: "comboSalesType", validasi: "required", required: true, disable: "isDisable" },
                        { name: "TransType", text: "Tipe Transaksi", cls: "span3  number", placeHolder: "0", readonly: true, type: "select2", datasource: "comboTransType", disable: "isDisable", validasi: "required", required: true },
                        { name: "isExternal", text: "External", cls: "span2", type: "x-switch", disable: "isTransfer==false", show: "isTrex==true" },
                        {
                            text: "Pelanggan", type: "controls", type: "controls", required: true, items: [
                            { name: "CustomerCode", cls: "span2", placeHolder: "Customer Code", type: "popup", btnName: "btnCust", readonly: true, click: "GetCust()", disable: "isDisable", validasi: "required", required: true },
                            { name: "CustomerName", cls: "span4", placeHolder: "Customer Name", readonly:true }]
                        },
                        {
                            text: "Pick By", type: "controls", type: "controls", required: true, items: [
                            { name: "PickedBy", cls: "span2", placeHolder: "Picked By", type: "popup", btnName: "btnPicked", readonly: true, click: "GetPickedBy()", required: true, disable: "isDisable", validasi: "required", required: true },
                            { name: "PickedByName", cls: "span4", placeHolder: "Picked Name", readonly:true }]
                        },
                        {
                            type: "buttons", cls: "span4", items: [
                                { name: "btnFind", text: "Cari", click: "FindOrderDetail(false)", disable:"isDisable"},
                            ]
                        },
                ]   
            },
            {
                name: "pnlB",
                title: "Customer Order Detail",
                items: [
                        {
                            text: "Keterangan", type: "controls", type: "controls", items: [
                            { name: "Remark", cls: "span4", placeHolder: "Keterangan", disable: "!isDisable" },
                            {
                                type: "buttons", cls: "span4", items: [
                                    { name: "btnSaveRemark", text: "Simpan Keterangan", click: "SaveRemark()", disable: "!isDisable" },
                                ]
                            }]
                        },
                        {
                            name: "wxcustorderdetail",
                            type: "wxdiv"
                        },
                        {
                            type: "buttons", cls: "span4", items: [
                                { name: "btnFind", text: "Cari Part Details", click: "FindPartDetails(false)", disable:"isDisable" },
                            ]
                        },
                ]
            },
            {
                name: "pnlC",
                title: "Part Detail",
                cls: "span8 full",
                items: [
                        { name: "DocNo", text: "No. Dokumen", cls: "span2", placeHolder: "", readonly: true, model:"detail.DocNo" },
                        { name: "PartNo", text: "No. Part", cls: "span2", placeHolder: "", readonly: true, model: "detail.PartNo" },
                        { name: "PartNoOriginal", text: "No. Part Original", cls: "span2", placeHolder: "", readonly: true, model: "detail.PartNoOriginal" },
                        { name: "ExPickingSlipNo", text: "Ex. No Picking Slip", cls: "span2", placeHolder: "", readonly: true, model: "detail.ExPickingSlipNo" },
                        { name: "QtyPick", text: "Pick Qty", cls: "span2", placeHolder: "0", readonly: true, model: "detail.QtyPick" },
                        { name: "QtyPicked", text: "Picked Qty", cls: "span2", placeHolder: "0", disable: "!isDisable" , model: "detail.QtyPicked" },
                        { name: "QtyBill", text: "Bill Qty", cls: "span2", placeHolder: "0", readonly: true, model: "detail.QtyBill" },
                        {
                            type: "buttons", cls: "span4 full", items: [
                                { name: "btnSavePart", text: "Save", click: "SavePart()", disable:"DisableSvPrt" },
                                { name: "tbnClrPart", text: "Clear", click: "ClearDetails()", disable: "DisableClrPrt" }
                            ]
                        },
                        {
                            name:"wxpartdetail",
                            type: "wxdiv",
                            cls: "span8 full"
                        },
                        {
                            type: "buttons", cls: "span4", items: [
                                { name: "btnGenPL", text: "Generate PL", click: "GenPL()", disable: "DisablePL" },
                                { name: "btnGenBill", text: "Generate Bill", click: "GenBill()", disable: "DisableBill" }
                            ]
                        },
                ]
            }
        ]
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spPembuatanPLdanEntryPickedQtyPLController"); 
    }

});