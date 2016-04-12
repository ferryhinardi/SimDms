"use strict";

function ListPriceBrances($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    me.IsAllowToSave = false;
    me.IsBrowseData = false;

    me.tableDefault = [
      ["Sales Model", "", "", "Group Price", "Include PPN", "", "", "Exclude PPN", "", "", "PPN","", "PPNBM Paid", " Effective Date DD-MM-YYYY", "Is Active"],
      ["Code", "Year", "Description", "", "Retail Price", "Disc Price", "Net Sales", "Retail Price", "Disc Price", "Net Sales", "Before Disc", "After Disc", "","",""],
    ]

    me.rowToJson = function (data) {
        var returnObj = new Object();
        returnObj.SalesModelCode = data[0];
        returnObj.SalesModelYear = data[1];
        returnObj.SalesModelDesc = data[2];
        returnObj.GroupPrice = data[3];
        returnObj.RetailPriceIncludePPN = data[4];
        returnObj.DiscPriceIncludePPN = data[5];
        returnObj.NetPriceIncludePPN = data[6];
        returnObj.RetailPriceExcludePPN = data[7];
        returnObj.DiscPriceExcludePPN = data[8];
        returnObj.NetPriceExcludePPN = data[9];
        returnObj.PPNBeforeDisc = data[10];
        returnObj.PPNAfterDisc = data[11];
        returnObj.PPNBMPaid = data[12];
        returnObj.EffectiveDate = moment(data[13],'DD-MM-YYYY').format('YYYY-MM-DD');
        returnObj.isStatus = data[14];
        return returnObj;
    }

    me.jsonToRow = function (returnObj) {
        var data = [
        returnObj.SalesModelCode,
        returnObj.SalesModelYear,
        returnObj.SalesModelDesc,
        returnObj.GroupPrice,
        returnObj.RetailPriceIncludePPN ,
        returnObj.DiscPriceIncludePPN,
        returnObj.NetSalesIncludePPN,
        returnObj.RetailPriceExcludePPN,
        returnObj.DiscPriceExcludePPN,
        returnObj.NetSalesExcludePPN,
        returnObj.PPNBeforeDisc,
        returnObj.PPNAfterDisc,
        returnObj.PPNBMPaid,
        moment(returnObj.EffectiveDate).format('DD-MM-YYYY'),
        returnObj.isStatus ]
        return data;
    }
    
    me.save = function () {

        var data = window.hotPriceList.getData().slice(0);
        data.splice(0, 2);

        if (data.length === 1) return;
        var listData = [];

        for (var i = 0; i < data.length-1; i++)
        {
            listData.push(me.rowToJson(data[i]));
        }

        var postData = {
            BranchCode: me.data.BranchCode,
            SupplierCode: me.data.SupplierCode,
            Data: JSON.stringify(listData)
        }

        $.ajax({
            type: "POST",
            url: 'om.api/PriceList/Save',
            dataType: 'json',
            data: postData
            , success: function (response) {
                Wx.Success("Data saved...");
                me.IsBrowseData = false;
                window.hotPriceList.alter('remove_row', 2, window.hotPriceList.countRows());
                me.IsAllowToSave = false;
                me.hasChanged = false;
                me.IsBrowseData = false;
            }
        });


    }
    

    me.browseSupplier = function () {
        var lookup = Wx.blookup({
            name: "suppliercodeBrowse",
            title: "Supplier Code Browse",
            manager: spManager,
            query: "Suppliers",
            defaultSort: "SupplierCode asc",
            columns: [
             { field: 'SupplierCode', title: 'Supplier Code' },
             { field: 'SupplierName', title: 'Supplier Name' }
            ]
        });

        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SupplierCode = data.SupplierCode;
                me.data.SupplierName = data.SupplierName;
                me.isSave = false;
                me.Apply();
            }
        });
    }


    me.browseBranch = function () {
        var lookup = Wx.blookup({
            name: "branchcodeBrowse",
            title: "Branch Browse",
            manager: spManager,
            query: "Branches",
            defaultSort: "BranchCode asc",
            columns: [
             { field: 'BranchCode', title: 'Branch Code' },
             { field: 'CompanyName', title: 'Branch Name' }
            ]
        });

        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BranchCode = data.BranchCode;
                me.data.CompanyName = data.CompanyName;
                me.isSave = false;
                me.Apply();
            }
        });
    }


    me.browse = function () {


        var params = me.data;

        if (params.SalesModelCode === undefined) {
            params.SalesModelCode = '';
        }

        if (params.SalesModelYear === undefined) {
            params.SalesModelYear = '';
        }

        if (params.IsActive === undefined) {
            params.IsActive = false;
        }
        console.log(params);

        $.ajax({
            type: "POST",
            url: 'om.api/PriceList/LoadTable',
            dataType: 'json',
            data: params
            , success: function (response) {
                
                if (response.data !== null)
                {
                    var i, n = response.data.length;
                    var datadump = [];

                    for(i=0;i<n;i++)
                    {
                        //datadump.push(me.jsonToRow(response.data[i]));
                        var returnObj = response.data[i];
                        datadump.push([2+i,0,returnObj.SalesModelCode])
                        datadump.push([2+i, 1, returnObj.SalesModelYear])
                        datadump.push([2+i,2,returnObj.SalesModelDesc])
                        datadump.push([2+i,3,returnObj.GroupPrice])
                        datadump.push([2+i,4,returnObj.RetailPriceIncludePPN])
                        datadump.push([2+i,5,returnObj.DiscPriceIncludePPN])
                        datadump.push([2+i,6,returnObj.NetSalesIncludePPN])
                        datadump.push([2+i,7,returnObj.RetailPriceExcludePPN])
                        datadump.push([2+i,8,returnObj.DiscPriceExcludePPN])
                        datadump.push([2+i,9,returnObj.NetSalesExcludePPN])
                        datadump.push([2+i,10,returnObj.PPNBeforeDisc])
                        datadump.push([2+i,11,returnObj.PPNAfterDisc])
                        datadump.push([2+i,12,returnObj.PPNBMPaid])
                        datadump.push([2+i,13,moment(returnObj.EffectiveDate).format('DD-MM-YYYY')])
                        datadump.push([2+i,14,returnObj.isStatus])
                    }

                    window.hotPriceList.alter('remove_row', 2, window.hotPriceList.countRows());
                    window.hotPriceList.setDataAtCell(datadump, null, null, 'loadData');
                    me.IsBrowseData = true;
                }
            }
        });

    }
    
    me.initialize = function () {
        me.data = {};
        me.data.SupplierCode = '00100100';
        me.data.SupplierName = 'PT. SUZUKI INDOMOBIL SALES';
        me.data.IsAllStatus = true;
        me.IsAllowToSave = false;
        me.IsActive = true;
        me.IsBrowseData = false;
        if (window.hotPriceList !== undefined)
        {
            window.hotPriceList.alter('remove_row', 2, window.hotPriceList.countRows());
        }
    };

    me.IsEditData = function()
    {
        return me.data.BranchCode > 0;
    }

    me.start();

    $('#InjectData').bind('change', function()
    {
        if (!me.IsBrowseData) {
            if (!me.IsAllowToSave)
            {
                me.IsAllowToSave = true;
                me.data.BranchCode = '';
                me.data.CompanyName = '';
                $("#btnCancel").html("<i class='icon icon-hand-right'></i>Close");
            }
        } else {
            if (!me.IsAllowToSave)
            {
                me.IsAllowToSave = true;
                $("#btnCancel").html("<i class='icon icon-hand-right'></i>Close");
            }
        }

    })

    me.runPivot = function()
    {

    }
}



$(document).ready(function () {





    var options = {
        title: "TEST PIVOT GRID",
        xtype: "panels",
        toolbars: [ 
                    { name: "btnTest", text: "Pivot", cls: "btn btn-info", icon: "icon-search" },
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "(hasChanged && !IsAllowToSave)", click: "browse()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && IsAllowToSave", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Close", cls: "btn btn-warning", icon: "icon-remove", show: "hasChanged && IsAllowToSave", click: "cancelOrClose()" },
        ],
        panels: [
        {
            name: "pnlA",
            items: [
                    {
                    text: "Branch",
                    type: "controls",
                    cls: "span8",
                    items: [
                        //{
                        //    name: "IsBranch",
                        //    type: "ng-check",
                        //    cls: "span1",
                        //},
                        {
                            name: "BranchCode",
                            cls: "span2",
                            type: "popup",
                            text: "Branch Code",
                            btnName: "btnBranch",
                            readonly: true, click: "browseBranch()"
                        },
                        {
                            name: "CompanyName",
                            cls: "span4",
                            text: "Branch Name",
                            readonly: true
                        }
                    ]
                    },
                    {
                        text: "Supplier",
                        type: "controls",
                        cls: "span8",
                        items: [
                            //{
                            //    type: "ng-check",
                            //    cls: "span1",
                            //    style: "visibility: hidden;"
                            //},
                            {
                                name: "SupplierCode",
                                cls: "span2",
                                type: "popup",
                                text: "Supplier Code",
                                btnName: "btnBranch",
                                required: true,
                                readonly: true, click: "browseSupplier()", validasi: "required"
                            },
                            {
                                name: "SupplierName",
                                cls: "span4",
                                text: "Supplier Name",
                                required: true,
                                readonly: true, validasi: "required"
                            }
                        ]
                    },
                    {
                        text: "Sales Model Year",
                        type: "controls",
                        cls: "span8",
                        items: [
                            //{
                            //    name: "IsYear",
                            //    type: "ng-check",
                            //    cls: "span1 hide",
                            //},
                            {
                                name: "SalesModelCode",
                                cls: "span2",
                                text: "Sales Model Code"
                            },
                            {
                                name: "SalesModelYear",
                                cls: "span1",
                                text: "Year"
                            },
                            {
                                name: "InjectData",
                                cls: "span1",
                                style: "visibility: hidden;",
                                text: "Year"
                            },         
                        ]
                    },
                    { name: "IsActive", text: "Is Active ?", type: "x-switch", cls: "span1", disable: 'data.IsAllStatus==true' },
                    {
                        name: "IsAllStatus",
                        type: "ng-check",
                        text: 'All',
                        cls: "span1",
                    },
            ]// end of panel  
        //}, {
        //    name: 'wxgridcell',
        //    xtype: 'wxtable'
        }, {
            name: 'wxpivotgrid',
            xtype: 'wxtable',
            style: 'margin-top: 35px;'
        }

       ] // end of panel
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("ListPriceBrances");
    }


    //$.ajax({
    //    //url: 'php/cars.php', // commented out because our website is hosted on static GitHub Pages
    //    url: 'om.api/combo/GetSalesModel',
    //    dataType: 'json',
    //    data: {
    //        query: query
    //    },
    //    success: function (response) {
    //        console.log("response", response);
    //        //process(JSON.parse(response.data)); // JSON.parse takes string as a argument
    //        process(response.data);

    //    }
    //});


    $('#btnTest').on('click', function () {
        $.ajax({
            type: "POST",
            url: 'om.api/PriceList/PivotDemo',
            dataType: 'json',
            success: function (response) {
                if (response.data !== null) {
                    window.pivotdata = response.data;
                    console.log(window.pivotdata)
                    $("#wxpivotgrid").pivotUI(window.pivotdata, {

                    });
                }
            }
        });
    })
    

});