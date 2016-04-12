var status = 'N';
var salesType = '0';
var IsPORDD = false;
var par = '';
var rparam = '';
var profitcenter = "300";
var cusDiscPct = "";
var qtyPartAcc = 0;
var dataCMB = '';
var dataTemp = '';
"use strict";

function spEntryPengeluaranStockController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/OrderNo').
    success(function (data, status, headers, config) {
        dataTemp = data;
        me.comboOrderNo = dataTemp;
        me.init();
    });

    $http.post('sp.api/Combo/TransactionType?CodeId=TTPJ').
    success(function (data, status, headers, config) {
        me.comboTTPJ = data;
        me.init();
    });

    $http.post('sp.api/Combo/TransactionType?CodeId=TTNP').
    success(function (data, status, headers, config) {
        me.comboTTNP = data;
        //me.init();
    });

    $http.post('sp.api/Combo/TransactionType?CodeId=TTSR').
    success(function (data, status, headers, config) {
        me.comboTTSR = data;
        //me.init();
    });

    $http.post('sp.api/Combo/TransactionType?CodeId=TTSL').
    success(function (data, status, headers, config) {
        me.comboTTSL = data;
        ///me.init();
    });
    
    me.$watch('selling', function (newValue, oldValue) {
        if (!me.isInProcess) {

            var eq = (newValue == oldValue);

            // check apakah perubahan data tersebut memiliki nilai atau object kosong (empty object)
            if (!(_.isEmpty(newValue)) && !eq) {
                if (!me.hasChanged && !me.isLoadData) {
                    me.hasChanged = true;
                    me.isLoadData = false;
                }
                if (!me.isSave) {
                    me.isSave = true;
                    me.hasChanged = true;
                    me.isLoadData = false;
                }
            } else {
                me.hasChanged = false;
                me.isSave = false;
            }
        }

    }, true);

    me.$watch('sellingPORDD', function (newValue, oldValue) {
        if (!me.isInProcess) {

            var eq = (newValue == oldValue);

            // check apakah perubahan data tersebut memiliki nilai atau object kosong (empty object)
            if (!(_.isEmpty(newValue)) && !eq) {
                if (!me.hasChanged && !me.isLoadData) {
                    me.hasChanged = true;
                    me.isLoadData = false;
                }
                if (!me.isSave) {
                    me.isSave = true;
                    me.hasChanged = true;
                    me.isLoadData = false;
                }
            } else {
                me.hasChanged = false;
                me.isSave = false;
            }
        }

    }, true);

    me.$watch('nonselling', function (newValue, oldValue) {
        salesType = '1';
        if (!me.isInProcess) {

            var eq = (newValue == oldValue);

            // check apakah perubahan data tersebut memiliki nilai atau object kosong (empty object)
            if (!(_.isEmpty(newValue)) && !eq) {
                if (!me.hasChanged && !me.isLoadData) {
                    me.hasChanged = true;
                    me.isLoadData = false;
                }
                if (!me.isSave) {
                    me.isSave = true;
                    me.hasChanged = true;
                    me.isLoadData = false;
                }
            } else {
                me.hasChanged = false;
                me.isSave = false;
            }
        }

    }, true);

    me.$watch('service', function (newValue, oldValue) {
        if (!me.isInProcess) {

            var eq = (newValue == oldValue);

            // check apakah perubahan data tersebut memiliki nilai atau object kosong (empty object)
            if (!(_.isEmpty(newValue)) && !eq) {
                if (!me.hasChanged && !me.isLoadData) {
                    me.hasChanged = true;
                    me.isLoadData = false;
                }
                if (!me.isSave) {
                    me.isSave = true;
                    me.hasChanged = true;
                    me.isLoadData = false;
                }
            } else {
                me.hasChanged = false;
                me.isSave = false;
            }
        }

    }, true);

    me.$watch('unitorder', function (newValue, oldValue) {
        if (!me.isInProcess) {

            var eq = (newValue == oldValue);

            // check apakah perubahan data tersebut memiliki nilai atau object kosong (empty object)
            if (!(_.isEmpty(newValue)) && !eq) {
                if (!me.hasChanged && !me.isLoadData) {
                    me.hasChanged = true;
                    me.isLoadData = false;
                }
                if (!me.isSave) {
                    me.isSave = true;
                    me.hasChanged = true;
                    me.isLoadData = false;
                }
            } else {
                me.hasChanged = false;
                me.isSave = false;
            }
        }

    }, true);

    me.$watch('jenisTransaksi', function (newValue, oldValue) {
        IsPORDD = false;
        me.init();
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
        }
    });

    me.$watch('nonselling.TransType', function (e) {
        if (e == 10)
        {
            me.isTransfer = true;
        }
        else
        {
            me.isTransfer = false;
            if (me.nonselling != undefined) {
                me.nonselling.isExternal = false;
            }
        }
    });

    //me.$watch('nonselling.isExternal', function (e) {
    //    console.log(e);
    //});

    $scope.$on('Selling', function () {
        salesType = '0';
        $('#DocNo').attr('placeholder', 'SOC/XX/YYYYYY');
    });

    $scope.$on('SellingPORDD', function () {
        salesType = '0';
        IsPORDD = true;
        $('#DocNo').attr('placeholder', 'SOC/XX/YYYYYY');
    });

    $scope.$on('NonSelling', function () {
        salesType = '1';
        $('#DocNo').attr('placeholder', 'BPS/XX/YYYYYY');
        me.gridOrderDetailNonSelling.hideColumn("RefferenceNo");

        $http.post('sp.api/EntryPengeluaranStock/DefaultNonSales')
       .success(function (e) {
           me.isTrex = e.isTrex;
       })
      
    });

    $scope.$on('Service', function () {
        salesType = '2';
        $('#DocNo').attr('placeholder', 'SSS/XX/YYYYYY');
    });

    $scope.$on('UnitOrder', function () {
        salesType = '3';
        $('#DocNo').attr('placeholder', 'SSU/XX/YYYYYY');
    });

    me.printPreview = function () {
        $http.post('sp.api/entrypengeluaranstock/print', { docNo: me.data.DocNo })
        .success(function (e) {
            if (e.success) {

                switch (me.jenisTransaksi) {
                    case "Selling":
                        par = me.data.DocNo + "," + me.data.DocNo + "," + "typeofgoods";
                        rparam = "PRINT";
						
						Wx.showPdfReport({
                            id: "SpRpTrn031",
                            pparam: par,
                            rparam: rparam,
                            textprint:true,
                            type: "devex"
                        });
						
                        break;
                    case "SellingPORDD":
                        par = me.data.DocNo + "," + me.data.DocNo + "," + "typeofgoods";
                        rparam = "PRINT";
						Wx.showPdfReport({
                            id: "SpRpTrn031",
                            pparam: par,
                            rparam: rparam,
                            textprint: true,
                            type: "devex"
                        });
                        break;
                    case "NonSelling":
                        par = me.data.DocNo + "," + me.data.DocNo + "," + profitcenter + "," + salesType + "," + "typeofgoods";
                        rparam = "SPAREPART";
                        console.log(par);
						Wx.showPdfReport({
                            id: "SpRpTrn009",
                            pparam: par,
                            rparam: rparam,
                            textprint:true,
                            type: "devex"
                        });
                        break;
                    case "Service":
                        par = me.data.DocNo + "," + me.data.DocNo + "," + profitcenter + "," + salesType + "," + "typeofgoods";
                        rparam = "SSS";
						
						Wx.showPdfReport({
                            id: "SpRpTrn039A",
                            pparam: par,
                            rparam: rparam,
                            textprint: true,
                            type: "devex"
                        });
                        break;
                    case "UnitOrder":
                        par = me.data.DocNo + "," + me.data.DocNo + "," + profitcenter + "," + salesType + "," + "typeofgoods";
                        rparam = "SSU";
						
						Wx.showPdfReport({
						    id: "SpRpTrn039U",
                            pparam: par,
                            rparam: rparam,
                            textprint: true,
                            type: "devex"
                        });
                        break;
                }

                $('#btnStockAllocation').removeAttr('disabled', 'disabled');
                $('#btnCancelSO').removeAttr('disabled', 'disabled');
                me.CheckPengeluaranStock(me.data.DocNo);
            }
            else {
                Wx.alert(e.message);
            }
        })
        .error(function (e) {
            MsgBox(e, MSG_ERROR);
        });

    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "DocumentNoLookup",
            title: "Documents No. Browse",
            manager: pengeluaranManager,
            query: new breeze.EntityQuery.from("DocumentNoBrowseNew").withParameters({ SalesType: salesType, IsPORDD: IsPORDD }),
            defaultSort: "DocNo desc",
            columns: [
                { field: "DocNo", title: "Document No." },
                { field: "DocDate", title: "Document Date", template: "#= (DocDate == undefined) ? '' : moment(DocDate).format('DD MMM YYYY') #" },
                { field: "CustomerCode", title: "Customer Code" },
                { field: "CustomerName", title: "Customer Name" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.CheckPengeluaranStock(result.DocNo);
                me.isInProcess = true;
                me.loadDetail(result);
                me.Apply();
                //me.unlockCustPartNo();
                $('#DocDate').attr('disabled', 'disabled');
                $('#OrderNo3').attr('disabled', 'disabled');
            }
        });
    }

    me.loadDetail = function (data) {
        me.data.DocNo = data.DocNo;
        me.data.DocDate = data.DocDate;
        $.post('sp.api/entrypengeluaranstock/gettransdetail', { docNo: data.DocNo }, function (result) {
            var detail = result.Detail;
            var emp = result.Employee;

            switch (me.jenisTransaksi) {
                case "Selling":
                    me.selling.TransType = detail.TransType;
                    me.selling.isSubstitution = detail.isSubstitution;
                    me.selling.isBO = detail.isBO;
                    me.selling.OrderNo = detail.OrderNo;
                    me.selling.OrderDate = detail.OrderDate;
                    me.selling.EmployeeID = emp.EmployeeID;
                    me.selling.EmployeeName = emp.EmployeeName;
                    me.selling.TotSalesAmt = detail.TotSalesAmt === null ? 0 : number_format(detail.TotSalesAmt);
                    me.selling.TotDiscAmt = detail.TotDiscAmt === null ? 0 : number_format(detail.TotDiscAmt);
                    me.selling.TotDPPAmt = detail.TotDPPAmt === null ? 0 : number_format(detail.TotDPPAmt);
                    me.selling.TotPPNAmt = detail.TotPPNAmt === null ? 0 : number_format(detail.TotPPNAmt);
                    me.selling.TotFinalSalesAmt = detail.TotFinalSalesAmt === null ? 0 : number_format(detail.TotFinalSalesAmt);

                    me.CustDtl(detail.CustomerCode, detail.CustomerCodeBill);
                    break;
                case "SellingPORDD":
                    me.sellingPORDD.TransType = detail.TransType;
                    me.sellingPORDD.isSubstitution = detail.isSubstitution;
                    me.sellingPORDD.isBO = detail.isBO;
                    me.sellingPORDD.OrderNo = detail.OrderNo;
                    me.sellingPORDD.OrderDate = detail.OrderDate;
                    me.sellingPORDD.EmployeeID = emp.EmployeeID;
                    me.sellingPORDD.EmployeeName = emp.EmployeeName;
                    //me.sellingPORDD.TotSalesAmt = detail.TotSalesAmt == null ? 0 : parseFloat(detail.TotSalesAmt);
                    //me.sellingPORDD.TotDiscAmt = detail.TotDiscAmt == null ? 0 : parseFloat(detail.TotDiscAmt);
                    //me.sellingPORDD.TotDPPAmt = detail.TotDPPAmt == null ? 0 : parseFloat(detail.TotDPPAmt);
                    //me.sellingPORDD.TotPPNAmt = detail.TotPPNAmt == null ? 0 : parseFloat(detail.TotPPNAmt);
                    //me.sellingPORDD.TotFinalSalesAmt = detail.TotFinalSalesAmt == null ? 0 : parseFloat(detail.TotFinalSalesAmt);

                    me.CustDtl(detail.CustomerCode, detail.CustomerCodeBill);
                    break;
                case "NonSelling":
                    dataCMB = detail.OrderNo;
                    dataTemp.push({ "value": detail.OrderNo, "text": detail.OrderNo });
                    me.Apply();
                    me.nonselling.TransType = detail.TransType;
                    me.nonselling.isSubstitution = detail.isSubstitution;
                    me.nonselling.isBO = detail.isBO;
                    me.nonselling.OrderNo = detail.OrderNo;
                    me.nonselling.OrderDate = detail.OrderDate;
                    me.nonselling.isExternal = detail.isLocked;
                    
                    me.CustDtl(detail.CustomerCode, detail.CustomerCodeBill);
                    break;
                case "Service":
                    me.service.TransType = detail.TransType;
                    me.service.isSubstitution = detail.isSubstitution;
                    me.service.isBO = detail.isBO;
                    me.service.OrderNo = detail.OrderNo;
                    me.service.OrderDate = detail.OrderDate;
                    me.service.TotSalesAmt = number_format(detail.TotSalesAmt);
                    me.service.TotDiscAmt = number_format(detail.TotDiscAmt);
                    me.service.TotDPPAmt = number_format(detail.TotDPPAmt);
                    me.service.TotPPNAmt = number_format(detail.TotPPNAmt);
                    me.service.TotFinalSalesAmt = number_format(detail.TotFinalSalesAmt);
                    me.service.UsageDocNo = detail.UsageDocNo;
                    me.service.UsageDocDate = detail.UsageDocDate;

                    me.CustDtl(detail.CustomerCode, detail.CustomerCodeBill);
                    break;
                case "UnitOrder":
                    me.unitorder.TransType = detail.TransType;
                    me.unitorder.isSubstitution = detail.isSubstitution;
                    me.unitorder.UsageDocNo = detail.UsageDocNo;
                    me.unitorder.isBO = detail.isBO;
                    me.unitorder.OrderNo = detail.OrderNo;
                    me.unitorder.OrderDate = detail.OrderDate;
                    me.unitorder.TotSalesAmt = number_format(detail.TotSalesAmt);
                    me.unitorder.TotDiscAmt = number_format(detail.TotDiscAmt);
                    me.unitorder.TotDPPAmt = number_format(detail.TotDPPAmt);
                    me.unitorder.TotPPNAmt = number_format(detail.TotPPNAmt);
                    me.unitorder.TotFinalSalesAmt = number_format(detail.TotFinalSalesAmt);

                    me.CustDtl(detail.CustomerCode, detail.CustomerCodeBill);
                    break;
            }

            me.grid.detail = result.Table;
            if (me.jenisTransaksi == 'NonSelling') {
                me.loadTableData(me.gridOrderDetailNonSelling, me.grid.detail);
            }
            else {
                me.loadTableData(me.gridOrderDetail, me.grid.detail);
            }

            if (detail.Status == 1) {
                $('#btnStockAllocation').removeAttr('disabled', 'disabled');
                $('#btnCancelSO').removeAttr('disabled', 'disabled');
            }
            else {
                $('#btnStockAllocation').attr('disabled', 'disabled');
                $('#btnCancelSO').attr('disabled', 'disabled');
            }
            me.readyToModified();
            me.Apply();
            me.startEditing();
        });
    };

    me.CustomerCode = function () {
        var column = [
            { field: "CustomerCode", title: "Customer Code" },
            { field: "CustomerName", title: "Customer Name" },
            { field: 'Address', title: 'Address' },
            { field: 'ProfitCenter', title: 'Profit Center' }
        ];

        var filter = "CustomerCode asc"

        var lookupstring = "CustomerCodeLookup";
        if (me.jenisTransaksi == 'NonSelling' && me.nonselling.TransType == '10') {
            lookupstring = "CustomerCodeTransStockLookup";

            if (me.nonselling.isExternal == true) {
                lookupstring = "OpenData";
                filter = "BranchCode asc"
                var column = [
                    { field: "CompanyName", title: "Company Name" },
                    { field: "BranchCode", title: "Branch Code" },
                    { field: "BranchName", title: "Branch Name" },
                ];
            }
        }

        var lookup = Wx.blookup({
            name: "CustomerLookup",
            title: "Master Customer Lookup",
            manager: pengeluaranManager,
            query: new breeze.EntityQuery.from(lookupstring).withParameters({ status: '1' }),
            defaultSort: filter,
            //filters: [
            //{
            //    text: "Status",
            //    type: "controls",
            //    items: [
            //                {
            //                    name: "fltStatus",
            //                    type: "select",
            //                    cls: "span4",
            //                    readonly: true,
            //                    items: [
            //                        { value: '0', text: 'Show Active Status' },
            //                        { value: '1', text: 'Show All Status' }
            //                    ]
            //                },
            //    ]
            //}
            //],
            columns: column
        });
        lookup.dblClick(function (data) {
            if (data !== null) {
                if (me.nonselling.isExternal == true) {
                    me.CustDtl(data.BranchCode, data.BranchCode);
                }
                else {
                    me.CustDtl(data.CustomerCode, data.CustomerCode);
                }
                me.data.DocDate = me.now();
                me.selling.OrderDate = me.nonselling.OrderDate = me.service.OrderDate = me.unitorder.OrderDate = me.now();
                me.Apply();
            }
        });
    };

    me.CustomerShipping = function () {
        var lookupstring = "CustomerCodeLookup";
        if (me.jenisTransaksi == 'NonSelling' && me.nonselling.TransType == '10') {
            lookupstring = "CustomerCodeTransStockLookup";
        }

        var lookup = Wx.blookup({
            name: "CustomerShippingLookup",
            title: "Master Customer Lookup",
            manager: pengeluaranManager,
            query: new breeze.EntityQuery.from(lookupstring).withParameters({ status: '1' }),
            defaultSort: "CustomerCode asc",
            columns: [
                { field: "CustomerCode", title: "Customer Code" },
                { field: "CustomerName", title: "Customer Name" },
                { field: 'Address', title: 'Address' },
                { field: 'ProfitCenter', title: 'Profit Center' },
            ]
        });
        lookup.dblClick(function (data) {
            if (data !== null) {
                //.log(data);
                me.CustShippingDtl(data.CustomerCode, data.CustomerCode);
                me.Apply();
            }
        });
    };

    me.CustDtl = function (custCode, custCodeBill) {
        $.post('sp.api/entrypengeluaranstock/getcustomerdetail', { customerCode: custCode, customerCodeBill: custCodeBill, salesType: salesType }, function (result) {

            var dtl = result.customerdtl;
            var dtlbill = result.customerbilldtl;
            cusDiscPct = result.discPct;

            if (dtl == null && dtlbill == null)
            {
                return MsgBox("Detail Customer/Bill Customer tidak ditemukan!, silahkan cek customer Profitcenter nya.", MSG_ERROR);
            }

            me.data.CustomerCodeDtl = dtl.CustomerCode;
            me.data.CustomerNameDtl = dtl.CustomerName;
            me.data.Address1 = dtl.Address1;
            me.data.Address2 = dtl.Address2;
            me.data.Address3 = dtl.Address3;
            me.data.Address4 = dtl.Address4;

            me.data.CustomerCodeBillDtl = dtlbill.CustomerCode;
            me.data.CustomerNameBillDtl = dtlbill.CustomerName;
            me.data.AddressBill1 = dtlbill.Address1;
            me.data.AddressBill2 = dtlbill.Address2;
            me.data.AddressBill3 = dtlbill.Address3;
            me.data.AddressBill4 = dtlbill.Address4;

            switch (me.jenisTransaksi) {
                case "Selling":
                    me.selling.CustomerCode = dtl.CustomerCode;
                    me.selling.CustomerName = dtl.CustomerName;
                    me.selling.TOPCode = result.topcode;
                    me.selling.TOPDays = result.topdays;
                    me.selling.PaymentCode = result.paymentdesc;
                    me.selling.DiscPct = result.discPct;
                    break;
                case "SellingPORDD":
                    me.sellingPORDD.CustomerCode = dtl.CustomerCode;
                    me.sellingPORDD.CustomerName = dtl.CustomerName;
                    me.sellingPORDD.TOPCode = result.topcode;
                    me.sellingPORDD.TOPDays = result.topdays;
                    me.sellingPORDD.PaymentCode = result.paymentdesc;
                    me.sellingPORDD.DiscPct = result.discPct;
                    break;
                case 'NonSelling':
                    me.nonselling.CustomerCode = dtl.CustomerCode;
                    me.nonselling.CustomerName = dtl.CustomerName;
                    break;
                case 'Service':
                    me.service.CustomerCode = dtl.CustomerCode;
                    me.service.CustomerName = dtl.CustomerName;
                    me.service.TOPCode = result.topcode;
                    me.service.TOPDays = result.topdays;
                    me.service.PaymentCode = result.paymentdesc;
                    me.service.DiscPct = result.discPct;
                    break;
                case 'UnitOrder':
                    me.unitorder.CustomerCode = dtl.CustomerCode;
                    me.unitorder.CustomerName = dtl.CustomerName;
                    me.unitorder.TOPCode = result.topcode;
                    me.unitorder.TOPDays = result.topdays;
                    me.unitorder.PaymentCode = result.paymentdesc;
                    me.unitorder.DiscPct = result.discPct;
                    break;
                default:
                    break;
            }
            me.Apply();
        });
    };

    me.CustShippingDtl = function (custCode, custCodeBill) {
        $.post('sp.api/entrypengeluaranstock/getcustomerdetail', { customerCode: custCode, customerCodeBill: custCodeBill, salesType: salesType }, function (result) {

            var dtl = result.customerdtl;
            var dtlbill = result.customerbilldtl;

            me.data.CustomerCodeDtl = dtl.CustomerCode;
            me.data.CustomerNameDtl = dtl.CustomerName;
            me.data.Address1 = dtl.Address1;
            me.data.Address2 = dtl.Address2;
            me.data.Address3 = dtl.Address3;
            me.data.Address4 = dtl.Address4;
            me.Apply();
        });
     };

    me.PaymentCode = function () {
        var lookup = Wx.blookup({
            name: "PaymentLookup",
            title: "Payment Lookup",
            manager: pengeluaranManager,
            query: "PaymentLookup",
            defaultSort: "PaymentCode asc",
            columns: [
                { field: 'PaymentCode', title: 'Payment Code' },
                { field: 'PaymentDesc', title: 'Desc' }
            ]
        });
        lookup.dblClick(function (data) {
            switch (me.jenisTransaksi) {
                case 'Selling':
                    me.selling.PaymentCode = data.PaymentDesc;
                    break;
                case 'SellingPORDD':
                    me.sellingPORDD.PaymentCode = data.PaymentDesc;
                    break;
                default:
                    break;
            }

            me.Apply();
        });
    };

    me.JobOrder = function () {
        var lookup = Wx.blookup({
            name: "JobOrderLookup",
            title: "SPK No. Lookup",
            manager: pengeluaranManager,
            query: "JobOrderLookup",
            defaultSort: "JobOrderNo asc",
            columns: [
                { field: 'JobOrderNo', title: 'SPK No.' },
                {
                    field: "JobOrderDate", title: "SPK Date", sWidth: "130px",
                    template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #"
                },
                { field: 'CustomerCode', title: 'Customer' },
            ]
        });
        lookup.dblClick(function (data) {
            me.service.UsageDocNo = data.JobOrderNo;
            me.service.UsageDocDate = data.JobOrderDate;
            me.CustDtl(data.CustomerCode, data.CustomerCodeBill);
            me.isSave = false;
            me.Apply();
        });
    };

    me.SORNo = function () {
        var lookup = Wx.blookup({
            name: "SONoLookup",
            title: "SOR NO Lookup",
            manager: pengeluaranManager,
            query: "SONoLookup",
            defaultSort: "SONo asc",
            columns: [
                { field: 'SONo', title: 'SOR No.' },
                {
                    field: "SODate", title: "SO Date", sWidth: "130px",
                    template: "#= (SODate == undefined) ? '' : moment(SODate).format('DD MMM YYYY') #"
                },
                { field: 'CustomerCode', title: 'Customer' },
                { field: 'BillTo', title: 'Bill To' },
                { field: 'ShipTo', title: 'Ship To' },
            ]
        });
        lookup.dblClick(function (data) {
            me.unitorder.UsageDocNo = data.SONo;
            me.unitorder.UsageDocDate = data.SODate;
            me.CustDtl(data.CustomerCode, data.CustomerCodeBill);
            me.isSave = false;
            me.Apply();
        });
    };

    me.OrderNo = function () {
        if (me.sellingPORDD.CustomerCode == '') {
            Wx.alert("Customer Code tidak boleh kosong");
        }
        else {
            var lookup = Wx.blookup({
                name: "OrderNoLookup",
                title: "Order Lookup",
                manager: pengeluaranManager,
                query: new breeze.EntityQuery.from("OrderNoLookup").withParameters({ CustomerCode: me.sellingPORDD.CustomerCode }),
                defaultSort: "OrderNo asc",
                columns: [
                    { field: "OrderNo", title: "Order No." },
                    {
                        field: "OrderDate", title: "Order Date", sWidth: "130px",
                        template: "#= (OrderDate == undefined) ? '' : moment(OrderDate).format('DD MMM YYYY') #"
                    },
                ]
            });
            lookup.dblClick(function (data) {

                $http.post('sp.api/entrypengeluaranstock/GetTransPORDDDetail', { customerCode: me.sellingPORDD.CustomerCode, orderNo: data.OrderNo }).
                success(function (e) {
                    me.sellingPORDD.TransType = e.transType;
                    me.sellingPORDD.TotSalesAmt = e.totalSales == null ? 0 : parseFloat(e.totalSales);
                    me.sellingPORDD.TotDiscAmt = e.totalDisc == null ? 0 : parseFloat(e.totalDisc);
                    me.sellingPORDD.TotDPPAmt = e.totalDPP == null ? 0 : parseFloat(e.totalDPP);
                    me.sellingPORDD.TotPPNAmt = e.totalPPN == null ? 0 : parseFloat(e.totalPPN);
                    me.sellingPORDD.TotFinalSalesAmt = e.total == null ? 0 : parseFloat(e.total);
                }).error(function (e) {
                    MsgBox(e.message, MSG_ERROR);
                });

                me.sellingPORDD.OrderNo = data.OrderNo;
                me.sellingPORDD.OrderDate = data.OrderDate;
                me.isSave = false;
                me.Apply();
            });
        }
    }

    me.ValidateHppValue = function (callback) {
        var params = $.extend(me.data, me.dtlPart);

        $http.post('sp.api/entrypengeluaranstock/ValidateHppValue', params).
        success(function (result) {
            if (result.success) {
                if (result.message !== "") {
                    MsgConfirm(result.message, function (action) {
                        if (action == false) {
                            return;
                        }
                        else {
                            if (callback != undefined) {
                                callback();
                            }
                        }
                    });
                }
                else {
                    if (callback != undefined) {
                        callback();
                    }
                }
            }
            else {
                MsgBox(result.message, MSG_ERROR);
                return;
            }
        }).error(function (result, status, headers, config) {
            console.log(result);
            return;
        });
    }

    me.AddPart = function () {
        var maindata = $.extend(me.data, me.dtlPart);

        $http.post('sp.api/entrypengeluaranstock/ConfirmBO', { modelDtl: maindata })
        .success(function (e) {
            if (!e.success) {
                MsgConfirm("Customer ini mempunyai BO Part ini sejumlah " + e.QtyBO + " Apakah akan dilanjutkan?", function (result) {
                    if (!result) return;
                    else {
                        $http.post('sp.api/entrypengeluaranstock/validatepart', maindata)
                        .success(function (e) {
                            if (!e.success && e.status != "" && e.status == "BO") {
                                MsgConfirm(e.message, function (result) {
                                    if (result) {
                                        $http.post('sp.api/entrypengeluaranstock/savepart', maindata).
                                         success(function (e) {
                                             if (e.success) {
                                                 Wx.Success("Data saved...");
                                                 me.loadDetail(e.data);
                                                 me.data.DocDate = e.docDate;
                                                 me.dtlPart = {};
                                                 me.Apply();
                                             }
                                             else {
                                                 MsgBox(e.message, MSG_ERROR);
                                             }
                                         }).error(function (e, status, headers, config) {
                                             console.log(e);
                                         });
                                    }
                                });
                            }
                            else if (!e.success) {
                                MsgBox(e.message, MSG_ERROR);
                            }
                            else if (e.success) {
                                $http.post('sp.api/entrypengeluaranstock/savepartbranch', maindata).
                                        success(function (e) {
                                            if (e.success) {
                                                Wx.Success("Data saved...");
                                                me.loadDetail(e.data);
                                                me.data.DocDate = e.docDate;
                                                me.dtlPart = {};
                                                //me.Apply(); 
                                            }
                                            else {
                                                MsgBox(e.message, MSG_ERROR);
                                            }
                                        }).error(function (e, status, headers, config) {
                                            console.log(e);
                                        });
                            }
                        });
                    }
                });
            }
            else {
                $http.post('sp.api/entrypengeluaranstock/validatepart', maindata)
                .success(function (e) {
                    if (!e.success && e.status != "" && e.status == "BO") {
                        MsgConfirm(e.message, function (result) {
                            if (result) {
                                $http.post('sp.api/entrypengeluaranstock/savepart', maindata).
                                 success(function (e) {
                                     if (e.success) {
                                         Wx.Success("Data saved...");
                                         me.loadDetail(e.data);
                                         me.data.DocDate = e.docDate;
                                         me.dtlPart = {};
                                         me.Apply();
                                     }
                                     else {
                                         MsgBox(e.message, MSG_ERROR);
                                     }
                                 }).error(function (e, status, headers, config) {
                                     console.log(e);
                                 });
                            }
                        });
                    }
                    else if (!e.success) {
                        MsgBox(e.message, MSG_ERROR);
                    }
                    else if (e.success) {
                        $http.post('sp.api/entrypengeluaranstock/savepartbranch', maindata).
                                success(function (e) {
                                    if (e.success) {
                                        Wx.Success("Data saved...");
                                        me.loadDetail(e.data);
                                        me.data.DocDate = e.docDate;
                                        me.dtlPart = {};
                                        //me.Apply(); 
                                    }
                                    else {
                                        MsgBox(e.message, MSG_ERROR);
                                    }
                                }).error(function (e, status, headers, config) {
                                    console.log(e);
                                });
                    }
                });
            }
        });
    }

    me.DeletePart = function () {
        var maindata = $.extend(me.data, me.dtlPart);
        var src = "sp.api/entrypengeluaranstock/DeletePart";

        MsgConfirm("Are you sure to delete a current record?", function (result) {
            if (result) {
                $.ajax({
                    async: false,
                    type: "POST",
                    data: maindata,
                    url: src
                }).done(function (data) {
                    if (data.success) {
                        Wx.Success("Model " + me.dtlPart.PartNo + " has been remove from current part");
                        me.loadDetail(data.Data);
                        //me.clearTable(me.gridOrderDetail);
                        //me.grid.detail = data.Table;
                        //if (me.jenisTransaksi == 'NonSelling') {
                        //    me.loadTableData(me.gridOrderDetailNonSelling, me.grid.detail);
                        //}
                        //else {
                        //    me.loadTableData(me.gridOrderDetail, me.grid.detail);
                        //}
                        me.dtlPart = {};
                    }
                    else
                    {
                        MsgBox(data.message, MSG_ERROR);
                    }
                });
            }
        });
    };

    me.PartNo = function () {
        if (me.jenisTransaksi == "UnitOrder")
        {
            if (me.unitorder.UsageDocNo != undefined) {
                var lookup = Wx.blookup({
                    name: "PartNoLookup",
                    title: "Item Lookup",
                    manager: pengeluaranManager,
                    query: new breeze.EntityQuery().from("PartNoLookupUnit").withParameters({ SONo: me.unitorder.UsageDocNo, DocNo: me.data.DocNo }),
                    defaultSort: "PartNo asc",
                    columns: [
                        { field: "PartNo", title: "Part No", width: 180 },
                        {
                            field: "AvailQty", title: "Avail Qty.", width: 100,
                            template: '<div style="text-align:right;">#= kendo.toString(AvailQty, "n2") #</div>'
                        },
                        {
                            field: "OnOrder", title: "On Order Qty.", width: 105,
                            template: '<div style="text-align:right;">#= kendo.toString(OnOrder, "n2") #</div>'
                        },
                        {
                            field: "RetailPrice", title: "Retail Price", width: 105,
                            template: '<div style="text-align:right;">#= kendo.toString(RetailPrice, "n0") #</div>'
                        },
                        {
                            field: "RetailPriceInclTax", title: "Retail Price Incl Tax", width: 150,
                            template: '<div style="text-align:right;">#= kendo.toString(RetailPriceInclTax, "n0") #</div>'
                        },
                        { field: "MovingCode", title: "Mv. CD", width: 80, template: '<div style="text-align:right;">#= kendo.toString(MovingCode, "n0") #</div>' },
                        { field: "ABCClass", title: "ABC Class", width: 100 },
                        { field: "PartName", title: "Part Name" }
                    ]
                });
                lookup.dblClick(function (data) {
                    me.dtlPart.old = undefined;
                    me.dtlPart.PartNo = data.PartNo;
                    me.dtlPart.PartName = data.PartName;
                    me.dtlPart.RetailPrice = data.RetailPrice;
                    me.dtlPart.DiscPct = cusDiscPct;
                    me.getQtyOrder();
                    me.isSave = false;
                    me.Apply();
                });
            }else{
                MsgBox("Silahkan Pilih No SO Terlebih Dahulu!", MSG_INFO)
            }
        } else {
            var lookup = Wx.blookup({
                name: "PartNoLookup",
                title: "Item Lookup",
                manager: pengeluaranManager,
                query: new breeze.EntityQuery().from("PartNoLookupReq").withParameters({ ReqNo: me.nonselling.OrderNo, DocNo: me.data.DocNo, Customer: me.nonselling.CustomerCode }),//"PartNoLookupReq", 
                defaultSort: "PartNo asc",
                columns: [
                    { field: "PartNo", title: "Part No", width: 180 },
                    {
                        field: "AvailQty", title: "Avail Qty.", width: 100,
                        template: '<div style="text-align:right;">#= kendo.toString(AvailQty, "n2") #</div>'
                    },
                    {
                        field: "OnOrder", title: "On Order Qty.", width: 105,
                        template: '<div style="text-align:right;">#= kendo.toString(OnOrder, "n2") #</div>'
                    },
                    {
                        field: "RetailPrice", title: "Retail Price", width: 105,
                        template: '<div style="text-align:right;">#= kendo.toString(RetailPrice, "n0") #</div>'
                    },
                    {
                        field: "RetailPriceInclTax", title: "Retail Price Incl Tax", width: 150,
                        template: '<div style="text-align:right;">#= kendo.toString(RetailPriceInclTax, "n0") #</div>'
                    },
                    { field: "MovingCode", title: "Mv. CD", width: 80, template: '<div style="text-align:right;">#= kendo.toString(MovingCode, "n0") #</div>' },
                    { field: "ABCClass", title: "ABC Class", width: 100 },
                    { field: "PartName", title: "Part Name" }
                ]
            });
            lookup.dblClick(function (data) {
                me.dtlPart.old = undefined;
                me.dtlPart.PartNo = data.PartNo;
                me.dtlPart.PartName = data.PartName;
                me.dtlPart.RetailPrice = data.RetailPrice;
                me.dtlPart.DiscPct = cusDiscPct;
                me.dtlPart.QtyOrder = data.OnOrder;
                me.isSave = false;
                me.Apply();
            });
        }
    }

    me.getQtyOrder = function () {
        $http.post('sp.api/entrypengeluaranstock/getQtyAccUnitOrder', { SONo: me.unitorder.UsageDocNo, PartNo: me.dtlPart.PartNo })
       .success(function (e) {
           if (e.success) {
               me.dtlPart.QtyOrder = e.qty;
               qtyPartAcc = e.qty;
           }
       });
    }

    me.GetDiscPct = function (customerCode) {
        $http.post('sp.api/entrypengeluaranstock/getdiscpct', customerCode)
       .success(function (e) {
               me.dtlPart.DiscPct = e;
       })
    }

    me.SalesPart = function () {
        var lookup = Wx.blookup({
            name: "SalesPartLookup",
            title: "Sales Part Lookup",
            manager: pengeluaranManager,
            query: "SalesPartLookup",
            defaultSort: "EmployeeID asc",
            columns: [
             { field: 'EmployeeID', title: 'Part Sales Code' },
             { field: 'EmployeeName', title: 'Part Sales Name' }
            ]
        });
        lookup.dblClick(function (data) {
            switch (me.jenisTransaksi) {
                case "Selling":
                    me.selling.EmployeeID = data.EmployeeID;
                    me.selling.EmployeeName = data.EmployeeName;
                    break;
                case "SellingPORDD":
                    me.sellingPORDD.EmployeeID = data.EmployeeID;
                    me.sellingPORDD.EmployeeName = data.EmployeeName;
                    break;
                default:
                    break;
            }
            me.Apply();
        });
    };

    me.saveData = function (e, param) {
        console.log(me.grid.detail);
        var param = {
            SalesType: salesType, IsPORDD: IsPORDD, CustomerCodeBill: me.data.CustomerCodeBillDtl, CustomerCodeShip: me.data.CustomerCodeDtl
        }
        console.log('CustomerCodeBillDtl : ' + me.data.CustomerCodeBillDtl);
        console.log('CustomerCodeDtl : ' + me.data.CustomerCodeDtl);
        switch (me.jenisTransaksi) {
            case "Selling":
                if (me.selling.TransType == '' || me.selling.TransType == undefined) {
                    Wx.alert("Tipe Transaksi harus pilih salah satu!");
                }
                else {
                    me.selling.isExternal = false;
                    var maindata = $.extend(me.selling, me.data, param);

                    $http.post('sp.api/entrypengeluaranstock/save', maindata).
                    success(function (e) {
                        if (e.success) {
                            Wx.Success("Data saved...");
                            me.loadDetail(e.data);
                            //me.unlockCustPartNo();
                        }
                        else {
                            MsgBox(e.message, MSG_ERROR);
                        }
                    }).
                    error(function (e) {
                        MsgBox(e, MSG_ERROR);
                    });
                }

                break;
            case "SellingPORDD":
                me.sellingPORDD.isExternal = false;
                var maindata = $.extend(me.data, me.sellingPORDD, param);
                
                $http.post('sp.api/entrypengeluaranstock/save', maindata).
                success(function (e) {
                    if (e.success) {
                        Wx.Success("Data saved...");
                        me.loadDetail(e.data);
                    }
                    else {
                        MsgBox(e.message, MSG_ERROR);
                    }
                }).
                error(function (e) {
                    MsgBox(e, MSG_ERROR);
                });

                break;
            case "NonSelling":
                if ($('#OrderNo3').val() != "") {
                    if (me.nonselling.TransType == '' || me.nonselling.TransType == undefined || me.nonselling.OrderNo == undefined || me.nonselling.OrderNo == '') {
                        if (me.nonselling.TransType == '' || me.nonselling.TransType == undefined)
                            Wx.alert("Tipe Transaksi harus pilih salah satu!");
                        else Wx.alert("Order No harus pilih salah satu!");
                    }
                    else {
                        var maindata = $.extend(me.data, me.nonselling, param);
                        console.log('maindata : ' + maindata);
                        $http.post('sp.api/entrypengeluaranstock/save', maindata)
                            .success(function (e) {
                                if (e.success) {
                                    Wx.Success("Data saved...");
                                    console.log(e.data);
                                    me.loadDetail(e.data);
                                    $('#OrderNo3').attr('disabled', 'disabled');
                                }
                                else if (!e.success) {
                                    MsgBox(e.message, MSG_ERROR);
                                }
                            })
                            .error(function (e) {
                                MsgBox(e, MSG_ERROR);
                            });
                    }
                }
                break;
            case "Service":
                if (me.service.TransType == '' || me.service.TransType == undefined) {
                    Wx.alert("Tipe Transaksi harus pilih salah satu!");
                }
                else {
                    me.service.isExternal = false;
                    var maindata = $.extend(me.data, me.service, param);

                    $http.post('sp.api/entrypengeluaranstock/save', maindata)
                        .success(function (e) {
                            if (e.success) {
                                Wx.Success("Data saved...");
                                console.log(e.data);
                                me.loadDetail(e.data);
                            }
                            else if (!e.success) {
                                MsgBox(e.message, MSG_ERROR);
                            }
                        })
                        .error(function (e) {
                            MsgBox(e, MSG_ERROR);
                        });
                }
                break;
            case "UnitOrder":
                if (me.unitorder.TransType == '' || me.unitorder.TransType == undefined) {
                    Wx.alert("Tipe Transaksi harus pilih salah satu!");
                }
                else {
                    me.unitorder.isExternal = false;
                    var maindata = $.extend(me.data, me.unitorder, param);

                    $http.post('sp.api/entrypengeluaranstock/save', maindata)
                       .success(function (e) {
                           if (e.success) {
                               Wx.Success("Data saved...");
                               console.log(e.data);
                               me.loadDetail(e.data);
                           }
                           else if (!e.success) {
                               MsgBox(e.message, MSG_ERROR);
                           }
                       })
                       .error(function (e) {
                           MsgBox(e, MSG_ERROR);
                       });
                }
                break;
            default:
                break;
        }
    }

    me.AddEditModel = function () {

        if (me.detail.CategoryCode === undefined || me.detail.CategoryCode == null) {
            MsgBox("CategoryCode is required!!!", MSG_ERROR);
            return;
        }

        me.LinkDetail();

        $http.post('sp.api/TargetPenjualan/save2', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");

                    me.UpdateGridDetail(data.data);

                    me.data.QtyTarget = data.total.QtyTarget;
                    me.data.AmountTarget = data.total.AmountTarget;
                    me.data.TotalJaringan = data.total.TotalJaringan;

                    me.startEditing();
                    me.detail = {};

                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/entrypengeluaranstock/delete', me.data).
                success(function (e) {
                    if (e.success) {
                        Wx.Info("Record has been deleted...");
                        me.init();
                    } else {
                        MsgBox(e.message, MSG_ERROR);
                    }
                }).
                error(function (eg) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.showCustomer = function () {

        var custCode = "";
        switch (me.jenisTransaksi) {
            case "Selling":
                custCode = me.selling.CustomerCode;
                break
            case "SellingPORDD":
                custCode = me.sellingPORDD.CustomerCode;
                break
            case "NonSelling":
                custCode = me.nonselling.CustomerCode;
                break
            case "Service":
                custCode = me.service.CustomerCode;
                break
            case "UnitOrder":
                custCode = me.unitorder.CustomerCode;
                break
        }

        Wx.loadForm();
        Wx.showForm({ url: "gn/master/customer", params: custCode });
    }

    //untuk alokasi stok
    me.StockAllocation = function () {
        var param = {
            SalesType: salesType, IsPORDD: IsPORDD, CustomerCodeBill: me.data.CustomerCodeBillDtl, CustomerCodeShip: me.data.CustomerCodeDtl
        }
        var maindata
        switch (me.jenisTransaksi) {
            case "Selling":
                maindata = $.extend(me.selling, me.data, param);
                break;
            case "SellingPORDD":
                maindata = $.extend(me.data, me.sellingPORDD, param);
                break;
            case "NonSelling":
                maindata = $.extend(me.data, me.nonselling, param);
                break;
            case "Service":
                maindata = $.extend(me.data, me.service, param);
                break;
            case "UnitOrder":
                maindata = $.extend(me.data, me.unitorder, param);
                break;
        }

        $http.post('sp.api/entrypengeluaranstock/AllocationStockBranch', maindata)
        .success(function (e) {
            if (e.success) {
                Wx.Success("Proses alokasi stok berhasil");
                me.CheckPengeluaranStock(me.data.DocNo);
                $('#btnStockAllocation, #btnCancelSO, #btnCustomer').attr('disabled', 'disabled');
                $('#OrderNo3').attr('disabled', 'disabled');
                me.isPrintAvailable = false;
            }
            else {
                MsgBox(e.message, MSG_ERROR);
            }
        }).error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.CancelSO = function () {
        var param = {
            SalesType: salesType, IsPORDD: IsPORDD, CustomerCodeBill: me.data.CustomerCodeBillDtl, CustomerCodeShip: me.data.CustomerCodeDtl
        }
        var maindata
        switch (me.jenisTransaksi) {
            case "Selling":
                maindata = $.extend(me.selling, me.data, param);
                break;
            case "SellingPORDD":
                maindata = $.extend(me.data, me.sellingPORDD, param);
                break;
            case "NonSelling":
                maindata = $.extend(me.data, me.nonselling, param);
                break;
            case "Service":
                maindata = $.extend(me.data, me.service, param);
                break;
            case "UnitOrder":
                maindata = $.extend(me.data, me.unitorder, param);
                break;
        }

        $http.post('sp.api/entrypengeluaranstock/CancelSO', maindata)
        .success(function (e) {
            MsgBox(e.message);
        }).error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.InsertInvoice = function () {
        $http.post('sp.api/EntryPickedList/HelpInsertInvoice')
        .success(function (e) {
            if (e.success) {
                Wx.Success("Proses insert berhasil");
               
            }
            else {
                MsgBox(e.message, MSG_ERROR);
            }
        }).error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.initialize = function () {
        me.selling = {};
        me.sellingPORDD = {};
        me.service = {};
        me.unitorder = {};
        me.dtlPart = {};
        me.isTransfer = false;
        me.nonselling = {};

        me.nonselling.TransType = 10;

        me.clearTable(me.gridOrderDetail);
        me.clearTable(me.gridOrderDetailNonSelling);

        $('#btnStockAllocation').attr('disabled', 'disabled');
        $('#btnCancelSO').attr('disabled', 'disabled');
        $('#DocDate').removeAttr('disabled');
        $('#OrderNo3').removeAttr('disabled');
        $("[ng-model = 'jenisTransaksi']").attr('disabled', 'disabled');
        $('#Status').html("");
        $('#Status').css(
            {
                "font-size": "32px",
                "color": "red",
                "font-weight": "bold",
                "text-align": "center"
            });
        
        me.isPrintAvailable = true;
        $http.post('om.api/EntryPengeluaranStock/Default').
          success(function (dl, status, headers, config) {
              me.data.DocDate = dl.DocDate;
          });
        $('#DocDate').attr('disabled', 'disabled');
        
        me.selling.OrderDate = me.nonselling.OrderDate = me.service.OrderDate = me.unitorder.OrderDate = me.now();
        me.selling.isSubstitution = true;
        me.selling.isBO = true;
        me.sellingPORDD.isSubstitution = true;
        me.sellingPORDD.isBO = true;
        me.nonselling.isSubstitution = false;
        me.nonselling.isBO = false;
        me.nonselling.isExternal = false;
        me.service.UsageDocDate = me.now();
        me.unitorder.SORDate = me.now();
        //$('.total').val(0.00);
        
        me.isInProcess = false;
        //me.lockCustPartNo();
        $('#Status').html("");
        $("p[data-name='0']").click();
        
    }

    me.cancelOrClose = function () {
       dataTemp.pop();
       me.init();
    };

    me.lockCustPartNo = function () {
        $('#btnCustomerCode3').attr('disabled', 'disabled');
        $('#btnPartNo').attr('disabled', 'disabled');
    }

    me.unlockCustPartNo = function () {
        $('#btnCustomerCode3').removeAttr('disabled')
        $('#btnPartNo').removeAttr('disabled')
    }

    me.gridOrderDetail = new webix.ui({
        container: "wxOrderDetail",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "No", header: "No", width: 45 },
            { id: "PartNo", header: "Part No", width: 130 },
            { id: "PartName", header: "Part Name", fillspace: true },
            { id: "DiscPct", header: "Disc %", width: 70, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "QtyOrder", header: "Order Qty.", width: 100, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "QtySupply", header: "Supply Qty.", width: 100, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "QtyBO", header: "BO Qty.", width: 80, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "RetailPrice", header: "Retail Price", width: 100, format: webix.i18n.intFormat, css: { "text-align": "right" } },
            { id: "NetSalesAmt", header: "Net Sales Amt", width: 120, format: webix.i18n.intFormat, css: { "text-align": "right" } }
        ],
        on: {
            onSelectChange: function () {
                if (me.gridOrderDetail.getSelectedId() !== undefined) {
                    me.dtlPart = this.getItem(me.gridOrderDetail.getSelectedId().id);
                    me.dtlPart.old = me.gridOrderDetail.getSelectedId();
                    me.dtlPart.old = me.dtlPart.PartNo;
                    me.dtlPart.old = me.dtlPart.PartName;
                    me.dtlPart.old = me.dtlPart.DiscPct;
                    me.dtlPart.old = me.dtlPart.QtyOrder;
                    me.dtlPart.old = me.dtlPart.RetailPrice;
                    me.Apply();
                }
            }
        }
    });

    $("[name = 'PartNo']").on('blur', function () {
        if (me.jenisTransaksi == "UnitOrder") {
            if ($('#PartNo').val() != "")
            {
                $http.post('sp.api/entrypengeluaranstock/getQtyAccUnitOrder', { SONo: me.unitorder.UsageDocNo, PartNo: $('#PartNo').val() })
                .success(function (e) {
                    if (!e.success) {
                        return MsgBox("Part ini tidak ada dalam accesories Unit")
                    } else {
                        qtyPartAcc = e.qty;
                    }
                });
                $http.post('sp.api/EntryPengeluaranStock/GetPartName', { PartNo: $('#PartNo').val(), SONo: me.unitorder.UsageDocNo })
                .success(function (data) {
                    if (data.success) {
                        me.dtlPart.PartName = data.data.iName;
                        me.dtlPart.RetailPrice = data.data.iPrice;
                        me.dtlPart.DiscPct = cusDiscPct;
                        me.dtlPart.QtyOrder = qtyPartAcc;
                        me.isSave = false;
                        console.log("Name: " + me.dtlPart.PartName)
                        console.log("Harga: " + me.dtlPart.RetailPrice)
                    }
                    else {
                        if (!data.exist) {
                            me.dtlPart.PartNo = '';
                            me.dtlPart.PartName = '';
                            me.dtlPart.QtyOrder = 0;
                            me.PartNo();
                        }
                    }
                });
            }
        } else {
            $http.post('sp.api/EntryPengeluaranStock/GetPartName?PartNo=' + $('#PartNo').val())
                .success(function (data) {
                    if (data.success) {
                        me.dtlPart.PartName = data.data.iName;
                        me.dtlPart.RetailPrice = data.data.iPrice;
                        me.dtlPart.DiscPct = cusDiscPct;
                        me.dtlPart.QtyOrder = 0;
                        me.isSave = false;
                        console.log("Name: " + me.dtlPart.PartName)
                        console.log("Harga: " + me.dtlPart.RetailPrice)
                    }
                    else {
                        me.dtlPart.PartNo = '';
                        me.dtlPart.PartName = '';
                        me.PartNo();
                    }
                });
        }
    });

    $("[name = 'QtyOrder']").on('blur', function () {
        if (me.jenisTransaksi == "UnitOrder") {
            me.dtlPart.QtyOrder = qtyPartAcc;
            me.Apply()
            //console.log(qtyPartAcc);
        }
    });

    $("[name = 'OrderNo3']").on('change', function () {
        if ($('#OrderNo3').val() != "") {
            $http.post('sp.api/entrypengeluaranstock/GetTranPREQ', { REQNo: $('#OrderNo3').val() })
            .success(function (e) {
                if (!e.success) {
                    return MsgBox("Part ini tidak ada dalam accesories Unit")
                } else {
                    me.nonselling.OrderDate3 = e.Header.REQDate;
                    me.nonselling.CustomerCode = e.Header.BranchCode;
                    me.nonselling.CustomerName = e.Customer;

                    me.grid.detail = e.Detail;
                    //if (me.jenisTransaksi == 'NonSelling') {
                    //    console.log('masuk sini');
                    //    me.loadTableData(me.gridOrderDetailNonSelling, me.grid.detail);
                    //}
                }
            });
           
        }
    });

    me.backOrder = function (e) {
        return { "background-color": "#FFAAAA" ,"text-align": "right" };
    }

    me.gridOrderDetailNonSelling = new webix.ui({
        container: "wxOrderDetailNonSelling",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "No", header: "No", width: 45 },
            { id: "ReferenceNo", header: "Doc No", fillspace: true },
            { id: "PartNo", header: "Part No", fillspace: true },
            { id: "PartName", header: "Part Name", fillspace: true },
            { id: "QtyOrder", header: "Order Qty.", width: 100, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            //{ id: "AvailQty", header: "Avail Qty.", width: 100, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            //{ id: "BOQty", header: "BO Qty.", width: 100, format: webix.i18n.numberFormat,  cssFormat: me.backOrder },
            // id: "CostPrice", header: "Cost Price", width: 120, format: webix.i18n.intFormat, css: { "text-align": "right" } }
        ],
        on: {
            onSelectChange: function () {
                if (me.gridOrderDetailNonSelling.getSelectedId() !== undefined) {
                    me.dtlPart = this.getItem(me.gridOrderDetailNonSelling.getSelectedId().id);
                    me.dtlPart.old = me.gridOrderDetailNonSelling.getSelectedId();
                    me.dtlPart.old = me.dtlPart.PartNo;
                    me.dtlPart.old = me.dtlPart.PartName;
                    me.dtlPart.old = me.dtlPart.QtyOrder;
                    //me.dtlPart.old = me.dtlPart.RetailPrice;
                    me.Apply();
                }
            }
        }
    });

    me.OnTabChange = function (e, id) {
        me.gridOrderDetail.adjust();
        me.gridOrderDetailNonSelling.adjust();
    };

    me.CheckPengeluaranStock = function (DocNo) {
        $http.post('sp.api/Pengeluaran/CheckStatus', { WhereValue: DocNo, Table: "SpTrnSORDHdr", ColumnName: "DocNo" })
        .success(function (v, status, headers, config) {
            if (v.success) {
                $('#Status').html('<span style="font-size:28px;color:red;font-weight:bold">' + v.statusPrint.toUpperCase() + "</span>");
            } else {
                // show an error message
                MsgBox(v.message, MSG_ERROR);
            }
            me.startEditing();
        }).error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.start();
    me.jenisTransaksi = "NonSelling";
}

$(document).ready(function () {
    var options = {
        title: "Entry Pengeluaran Stock (Branch)",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pntStatus",
                title: " ",
                items: [
                { name: "Status", text: "", cls: "span4", readonly: true, type: "label" },
                ]
            },
            {
                name: "pnlA",
                title: "",
                items: [
                        { name: "DocNo", text: "Document No.", cls: "span4", placeHolder: "BPS/XX/YYYYYY", readonly: true },
                        { name: "DocDate", text: "Document Date", cls: "span4", type: "ng-datepicker" },
                        {
                            type: "buttons", items: [
                              { name: "btnCustomer", text: "Customer", icon: "icon-user", cls: "btn btn-info", click: "showCustomer()" },
                              { name: "btnStockAllocation", text: "Stock Allocation", disable: true, icon: "icon-plus", cls: "btn btn-success", click: "StockAllocation()" },
                              { name: "btnCancelSO", text: "Cancel SO", disable: true, icon: "icon-remove", cls: "btn btn-danger", click: "CancelSO()" },
                              //{ name: "btnInsert", text: "Insert Invoice", icon: "icon-remove", cls: "btn btn-danger", click: "InsertInvoice()" },
                            ]
                        },
                ]
            },
            {
                name: "pnlB",
                title: "",
                items: [
                      {
                          type: "optionbuttons",
                          name: "tabpage1",
                          model: "jenisTransaksi",
                          text: "Document Source",
                          items: [
                              { name: "Selling", text: "Sales" },
                              { name: "SellingPORDD", text: "Sales (PORDD)" },
                              { name: "NonSelling", text: "Non Sales (BPS)" },
                              { name: "Service", text: "Service" },
                              { name: "UnitOrder", text: "Unit Order" },
                          ]
                      },
                    ]
            },
            {
                name: "Selling",
                title: "Sales",
                cls: "tabpage1 Selling animate-show",
                show: "jenisTransaksi == 'Selling'",
                items: [
                    { name: "sellingTransType", model: "selling.TransType", text: "Transaction Type", type: "select2", cls: "span4", datasource: 'comboTTPJ', validasi :"" },
                    { name: "Subtitusi1", model: "selling.isSubstitution", text: "Substitution?", cls: "span2", type: "x-switch" },
                    { name: "BO1", model: "selling.isBO", text: "BO?", cls: "span2", type: "x-switch" },
                    {
                        text: "Customer",
                        type: "controls",
                        required: true,
                        items: [
                            { name: "CustomerCode1", model: "selling.CustomerCode", cls: "span2", placeHolder: "Customer Code", readonly: true, type: "popup", click: "CustomerCode()", required: true, validasi: "required" },
                            { name: "CustomerName1", model: "selling.CustomerName", cls: "span6", placeHolder: "Customer Name", readonly: true }
                        ]
                    },
                    { name: "OrderNo1", model: "selling.OrderNo", text: "Order No.", cls: "span4", required: true, validasi: "required" },
                    { name: "TOPCode1", model: "selling.TOPCode", text: "TOP Code", cls: "span4", readonly: true },
                    { name: "OrderDate1", model: "selling.OrderDate", text: "Order Date", cls: "span4", type: 'ng-datepicker' },
                    { name: "TOPDays1", model: "selling.TOPDays", text: "TOP Days", cls: "span4", readonly: true, value: "0" },
                    { name: "Payment1", model: "selling.PaymentCode", text: "Payment", cls: "span4 full", type: "popup", readonly: true, click: "PaymentCode()", required: true, validasi: "required" },
                    {
                        text: "SalesPart",
                        type: "controls",
                        items: [
                            { name: "EmployeeID1", model: "selling.EmployeeID", cls: "span2", placeHolder: "Sales Part Code", readonly: true, type: "popup", click: "SalesPart()", required: true, validasi: "required" },
                            { name: "EmployeeName1", model: "selling.EmployeeName", cls: "span6", placeHolder: "Sales Part Name", readonly: true }
                        ]
                    },
                    { name: "TotSalesAmt1", model: "selling.TotSalesAmt", text: "Sales Amt", cls: "span4 total number-int", readonly: true, value: 0 },
                    { name: "TotDiscAmt1", model: "selling.TotDiscAmt", text: "Disc Amt", cls: "span4 total number-int", readonly: true, value: 0 },
                    { name: "TotalDPPAmt1", model: "selling.TotDPPAmt", text: "DPP Amt", cls: "span4 total number-int", readonly: true, value: 0 },
                    { name: "TotalPPNAmt1", model: "selling.TotPPNAmt", text: "PPN Amt", cls: "span4 total number-int", readonly: true, value: 0 },
                    { name: "TotFinalSalesAmt1", model: "selling.TotFinalSalesAmt", text: "Total Amt", cls: "span4 total number-int", readonly: true, value: 0 }
                ]
            },
            {
                name: "SellingPORDD",
                title: "Sales (PORDD)",
                cls: "tabpage1 SellingPORDD animate-show",
                show: "jenisTransaksi == 'SellingPORDD'",
                items: [
                    { name: "sellingPORDDTransType", model: "sellingPORDD.TransType", text: "Transaction Type", type: "select2", cls: "span4", datasource: 'comboTTPJ', disable: true },
                    { name: "Subtitusi2", model: "sellingPORDD.isSubstitution", text: "Subtitusi?", cls: "span2", type: "x-switch", disable: true },
                    { name: "BO2", model: "sellingPORDD.isBO", text: "BO?", cls: "span2", type: "x-switch", disable: true },
                    {
                        text: "Customer",
                        type: "controls",
                        items: [
                            { name: "CustomerCode2", model: "sellingPORDD.CustomerCode", cls: "span2", placeHolder: "Customer Code", readonly: true, type: "popup", click: "CustomerCode()" },
                            { name: "CustomerName2", model: "sellingPORDD.CustomerName", cls: "span6", placeHolder: "Customer Name", readonly: true }
                        ]
                    },
                    { name: "OrderNo2", model: "sellingPORDD.OrderNo", text: "Order No.", cls: "span4", type: "popup", click: "OrderNo()" },
                    { name: "TOPCode2", model: "sellingPORDD.TOPCode", text: "TOP Code", cls: "span4" },
                    { name: "OrderDate2", model: "sellingPORDD.OrderDate", text: "Order Date", cls: "span4", readonly: true, type: 'ng-datepicker' },
                    { name: "TOPDays2", model: "sellingPORDD.TOPDays", text: "TOP Days", cls: "span4", readonly: true, value: "0" },
                    { name: "Payment2", model: "sellingPORDD.PaymentCode", text: "Payment", cls: "span4 full", type: "popup", readonly: true, click: "PaymentCode()" },
                    {
                        text: "Part Sales",
                        type: "controls",
                        items: [
                            { name: "EmployeeID2", model: "sellingPORDD.EmployeeID", cls: "span2", placeHolder: "Sales Part Code", readonly: true, type: "popup", click: "SalesPart()" },
                            { name: "EmployeeName2", model: "sellingPORDD.EmployeeName", cls: "span6", placeHolder: "Sales Part Name", readonly: true }
                        ]
                    },
                    { name: "TotSalesAmt2", model: "sellingPORDD.TotSalesAmt", text: "Sales Amt", cls: "span4 total number-int", readonly: true, value: 0 },
                    { name: "TotDiscAmt", model: "sellingPORDD.TotDiscAmt", text: "Disc Amt", cls: "span4 total number-int", readonly: true, value: 0 },
                    { name: "TotalDPPAmt2", model: "sellingPORDD.TotDPPAmt", text: "DPP Amt", cls: "span4 total number-int", readonly: true, value: 0 },
                    { name: "TotalPPNAmt2", model: "sellingPORDD.TotPPNAmt", text: "PPN Amt", cls: "span4 total number-int", readonly: true, value: 0 },
                    { name: "TotFinalSalesAmt2", model: "sellingPORDD.TotFinalSalesAmt", text: "Total Amt", cls: "span4 total number-int", readonly: true, value: 0 },
                ]
            },
            {
                name: "NonSelling",
                title: "Non Sales (BPS)",
                cls: "tabpage1 NonSelling animate-show",
                show: "jenisTransaksi == 'NonSelling'",
                items: [
                    { name: "nonSellingTransType", model: "nonselling.TransType", text: "Transaction Type", type: "select2", cls: "span4 full", datasource: "comboTTNP", disable: true },
                    { name: "External", model: "nonselling.isExternal", text: "External", cls: "span2", type: "x-switch", disable: "isTransfer==false", show: "isTrex==true" },
                    { name: "Subtitusi3", model: "nonselling.isSubstitution", text: "Subtitusi?", cls: "span2", type: "x-switch", disable: true },
                    { name: "BO3", model: "nonselling.isBO", text: "BO?", cls: "span2", type: "x-switch", disable: true },
                    { type: "controls", cls: "span8", items: [] },
                    { type: "controls", cls: "span8", items: [] },
                    { name: "OrderNo3", model: "nonselling.OrderNo", text: "Order No.", cls: "span4", type: "select2", required: true, validasi: "required", datasource: "comboOrderNo" },
                    { name: "OrderDate3", model: "nonselling.OrderDate", text: "Order Date", cls: "span4", type: 'ng-datepicker' },
                    {
                        text: "Customer",
                        type: "controls",
                        style: "margin-top:14px",
                        items: [
                            { name: "CustomerCode3", model: "nonselling.CustomerCode", cls: "span2", placeHolder: "Customer Code", readonly: true, type: "popup", click: "CustomerCode()" },
                            { name: "CustomerName3", model: "nonselling.CustomerName", cls: "span6", placeHolder: "Customer Name", readonly: true }
                        ]
                    },
                                    ]
            },
            {
                name: "nonsellingpart",
                show: "jenisTransaksi == 'NonSelling'",
                cls: "tabpage1 NonSelling animate-show",
                items: [
                           {
                               text: "Part No",
                               type: "controls",
                               items: [
                                       { name: "PartNo", cls: "span2", placeHolder: "Part No", type: "popup", readonly: true, click: "PartNo()", model: 'dtlPart.PartNo' },
                                       { name: "PartName", cls: "span6", placeHolder: "Part Name", readonly: true, model: 'dtlPart.PartName' }
                               ]
                           },
                           { name: "QtyOrder", text: "Qty Order", cls: "span2 number", model: 'dtlPart.QtyOrder' },
                           { name: "RetailPrice", cls: "hide", model: 'dtlPart.RetailPrice' },
                           {
                               type: "buttons",
                               items: [
                                       { name: "btnAddPart", text: "Add Part", icon: "icon-plus", cls: "btn btn-info", click: "AddPart()", show: "dtlPart.old === undefined", disable: "dtlPart.PartNo === undefined" },
                                       { name: "btnUpdatePart", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "UpdatePart()", show: "dtlPart.old !== undefined" },
                                       { name: "btnDeletePart", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeletePart()", show: "dtlPart.old !== undefined" },
                               ]
                           },
                           {
                               name: "wxOrderDetailNonSelling",
                               type: "wxdiv"
                           }
                ]
            },
            {
                name: "Service",
                title: "Service",
                cls: "tabpage1 Service animate-show",
                show: "jenisTransaksi == 'Service'",
                items: [
                    { name: "ServiceTransType", model: "service.TransType", text: "Transaction Type", type: "select2", cls: "span4 full", datasource: "comboTTSR" },
                    { name: "SPKNo4", model: "service.UsageDocNo", text: "SPK No.", cls: "span4", type: "popup", click: "JobOrder()", readonly: true },
                    { name: "SPKDate4", model: "service.UsageDocDate", text: "SPK Date", cls: "span4", type: "ng-datepicker" },
                    {
                        text: "Customer",
                        type: "controls",
                        items: [
                            { name: "CustomerCode4", model: "service.CustomerCode", cls: "span2", placeHolder: "Customer Code", readonly: true },
                            { name: "CustomerName4", model: "service.CustomerName", cls: "span6", placeHolder: "Customer Name", readonly: true },
                        ]
                    },
                    { name: "OrderNo4", model: "service.OrderNo", text: "Order No.", cls: "span4" },
                    { name: "TOPCode4", model: "service.TOPCode", text: "TOP Code", cls: "span4", readonly: true },
                    { name: "OrderDate4", model: "service.OrderDate", text: "Order Date", cls: "span4", type: 'ng-datepicker' },
                    { name: "TOPDays4", model: "service.TOPDays", text: "TOP Days", cls: "span4", readonly: true, value: "0" },
                    { name: "Payment4", model: "service.PaymentCode", text: "Payment", cls: "span4 full", readonly: true },
                    { name: "TotSalesAmt4", model: "service.TotSalesAmt", text: "Sales Amt", cls: "span4 total number-int", readonly: true, value: 0 },
                    { name: "TotDiscAmt4", model: "service.TotDiscAmt", text: "Discount Amt", cls: "span4 total number-int", readonly: true, value: 0 },
                    { name: "TotalDPPAmt4", model: "service.TotDPPAmt", text: "DPP Amt", cls: "span4 total number-int", readonly: true, value: 0 },
                    { name: "TotalPPNAmt4", model: "service.TotPPNAmt", text: "PPN Amt", cls: "span4 total number-int", readonly: true, value: 0 },
                    { name: "TotFinalSalesAmt4", model: "service.TotFinalSalesAmt", text: "Total Amt", cls: "span4 total number-int", readonly: true, value: 0 },
                ]
            },
            {
                name: "UnitOrder",
                title: "Unit Order",
                cls: "tabpage1 UnitOrder animate-show",
                show: "jenisTransaksi == 'UnitOrder'",
                items: [
                    { name: "UnitOrderTransType", model: "unitorder.TransType", text: "Transaction Type", type: "select2", cls: "span4 full", datasource: "comboTTSL" },
                    { name: "SORNo5", model: "unitorder.UsageDocNo", text: "SOR No.", cls: "span4", type: 'popup', click: "SORNo()", readonly: true },
                    { name: "SORDate5", model: "unitorder.UsageDocDate", text: "SOR Date", cls: "span4", type: "ng-datepicker", readonly: true },
                    {
                        text: "Customer",
                        type: "controls",
                        items: [
                            { name: "CustomerCode5", model: "unitorder.CustomerCode", cls: "span2", placeHolder: "Customer Code", readonly: true },
                            { name: "CustomerName5", model: "unitorder.CustomerName", cls: "span6", placeHolder: "Customer Name", readonly: true },
                        ]
                    },
                    { name: "OrderNo5", model: "unitorder.OrderNo", text: "Order No.", cls: "span4" },
                    { name: "TOPCode5", model: "unitorder.TOPCode", text: "TOP Code", cls: "span4", readonly: true },
                    { name: "OrderDate5", model: "unitorder.OrderDate", text: "Order Date", cls: "span4", type: 'ng-datepicker' },
                    { name: "TOPDays5", model: "unitorder.TOPDays", text: "TOP Days", cls: "span4", readonly: true, value: "0" },
                    { name: "Payment5", model: "unitorder.PaymentCode", text: "Payment", cls: "span4 full", readonly: true },
                    { name: "TotSalesAmt5", model: "unitorder.TotSalesAmt", text: "Total Jual", cls: "span4 total number-int", readonly: true, value: 0 },
                    { name: "TotDiscAmt5", model: "unitorder.TotDiscAmt", text: "Total Diskon", cls: "span4 total number-int", readonly: true, value: 0 },
                    { name: "TotalDPPAmt5", model: "unitorder.TotDPPAmt", text: "Total DPP", cls: "span4 total number-int", readonly: true, value: 0 },
                    { name: "TotalPPNAmt5", model: "unitorder.TotPPNAmt", text: "Total PPN", cls: "span4 total number-int", readonly: true, value: 0 },
                    { name: "TotFinalSalesAmt5", model: "unitorder.TotFinalSalesAmt", text: "Total", cls: "span4 total number-int", readonly: true, value: 0 },
                ]
            },
            {
                xtype: "tabs",
                show: "jenisTransaksi != 'NonSelling'",
                name: "tabpageDetail1",
                items: [
                    { name: "0", text: "Billing and Shipping" },
                    { name: "1", text: "Order Detail" }
                ]
            },
            {
                name: "0",
                title: "Billing and Shipping",
                show: "jenisTransaksi != 'NonSelling'",
                cls: "tabpageDetail1 0",
                items: [
                   {
                       text: "Shipping Address",
                       type: "controls",
                       items: [
                           //{ name: "CustomerCodeDtl", cls: "span2", placeHolder: "Customer Code", readonly: true },
                           { name: "CustomerCodeDtl", cls: "span2", placeHolder: "Customer Code", type: "popup", click: "CustomerShipping()" },
                           { name: "CustomerNameDtl", cls: "span6", placeHolder: "Customer Name", readonly: true },
                       ]
                   },
                   { name: "Address1", readonly: true },
                   { name: "Address2", readonly: true },
                   { name: "Address3", readonly: true },
                   { name: "Address4", readonly: true },
                   {
                       text: "Billing Address",
                       type: "controls",
                       items: [
                           { name: "CustomerCodeBillDtl", cls: "span2", placeHolder: "Customer Code", readonly: true },
                           { name: "CustomerNameBillDtl", cls: "span6", placeHolder: "Customer Name", readonly: true },
                       ]
                   },
                   { name: "AddressBill1", readonly: true },
                   { name: "AddressBill2", readonly: true },
                   { name: "AddressBill3", readonly: true },
                   { name: "AddressBill4", readonly: true },
                ]
            },
            {
                name: "1",
                title: "Order Detail",
                show: "jenisTransaksi != 'NonSelling'",
                cls: "tabpageDetail1 1",
                items: [
                            {
                                text: "Part No",
                                type: "controls",
                                items: [
                                        { name: "PartNo", cls: "span2", placeHolder: "Part No", type: "popup", click: "PartNo()", model: 'dtlPart.PartNo' },//readonly: true, 
                                        { name: "PartName", cls: "span6", placeHolder: "Part Name", readonly: true, model: 'dtlPart.PartName' }
                                ]
                            },
                            { name: "DiscPct", text: "Disc %", value: "0.00", cls: "span4 number", model: 'dtlPart.DiscPct' },
                            { name: "QtyOrder", text: "Qty Order", value: "0.00", cls: "span4 number", model: 'dtlPart.QtyOrder' },
                            { name: "RetailPrice", cls: "hide", model: 'dtlPart.RetailPrice' },
                            {
                                type: "buttons",
                                items: [
                                        { name: "btnAddPart", text: "Add Part", icon: "icon-plus", cls: "btn btn-info", click: "ValidateHppValue(AddPart)", show: "dtlPart.old === undefined", disable: "dtlPart.PartNo === undefined" },
                                        { name: "btnUpdatePart", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddPart()", show: "dtlPart.old !== undefined" },
                                        { name: "btnDeletePart", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeletePart()", show: "dtlPart.old !== undefined" },
                                ]
                            },
                            {
                                name: "wxOrderDetail",
                                type: "wxdiv"
                            }
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    $("p[data-name='0']").addClass('active');

    function init(s) {
        SimDms.Angular("spEntryPengeluaranStockController");
    }
});