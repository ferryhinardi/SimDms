var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spItemPriceController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    me.Consol = [
        { "value": "N", "text": "No" },
        { "value": "Y", "text": "Yes" }
    ];

    me.Debet = [
        { "value": "D", "text": "Debet" },
        { "value": "K", "text": "Credit" }
    ];

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "AccountLookUp",
            title: "Lookup Account",
            manager: gnManager,
            query: "Account",
            defaultSort: "CreatedDate desc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Description" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                me.grid.data = data;
                me.callAccount(data.AccountType);
                me.loadTableData(me.grid1, me.grid.data);
                me.accdesc();
                me.isSave = false;
                me.data.oid = true;
                me.Apply();
            }
        });
    };

    me.accdesc = function (data) {
        //branch&company
        $http.post('gn.api/Chart/Company').
          success(function (data, status, headers, config) {
              if (data.success) {
                  me.data.CompanyDesc = data.data.Description;
                  me.data.BranchDesc = data.data.Description1;
              } 
          });
        //$http.post('gn.api/Chart/AccDesc?tipe=200&segacc=' + me.data.Branch).
        //   success(function (data, status, headers, config) {
        //       if (data.success) {
        //           me.data.BranchDesc = data.data.Description;
        //       } 
        //   });

        //CostCentreDesc
        $http.post('gn.api/Chart/CostCentreDesc?lookupvalue=' + me.data.CostCtrCode).
           success(function (data, status, headers, config) {
               if (data.success) {
                   me.data.CostCtrCodeDesc = data.data.LookUpValueName;
               } 
           });

        //Product type
        $http.post('gn.api/Chart/AccDesc?tipe=300&segacc=' + me.data.ProductType).
           success(function (data, status, headers, config) {
               if (data.success) {
                   me.data.ProductTypeDesc = data.data.Description;
                   //alert(data.data.Description)
               }
           });

        //Natural Account
        $http.post('gn.api/Chart/AccDesc?tipe=400&segacc=' + me.data.NaturalAccount).
           success(function (data, status, headers, config) {
               if (data.success) {
                   me.data.NaturalAccountDesc = data.data.Description;
               }
           });

        //Inter Company
        $http.post('gn.api/Chart/AccDesc?tipe=500&segacc=' + me.data.InterCompany).
           success(function (data, status, headers, config) {
               if (data.success) {
                   me.data.InterCompanyDesc = data.data.Description;
               }
           });

        //Future use
        $http.post('gn.api/Chart/AccDesc?tipe=600&segacc=' + me.data.Futureuse).
           success(function (data, status, headers, config) {
               if (data.success) {
                   me.data.FutureuseDesc = data.data.Description;
               }
           });
    }
    me.segAcc = function (p, desc) {
        //if (!p) { p=200}
        var lookup = Wx.blookup({
            name: "SegmentAcc",
            title: desc + " Lookup",
            manager: gnManager,
            query: new breeze.EntityQuery().from("SegmentAcc").withParameters({ param: p }),
            defaultSort: "SegAccNo asc",
            columns: [
            { field: "SegAccNo", title: "Account " + desc },
            { field: "Description", title: "Account " + desc + " Desc" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                switch(p){
                    case 300:
                        me.data.ProductType = data.SegAccNo;
                        me.data.ProductTypeDesc= data.Description
                        break;
                    case 400:
                        break;
                    case 500:
                        me.data.InterCompany = data.SegAccNo;
                        me.data.InterCompanyDesc = data.Description
                        break;
                    case 600:
                        me.data.Futureuse = data.SegAccNo;
                        me.data.FutureuseDesc = data.Description;
                        break;
                    default:
                        me.data.Branch = data.SegAccNo;
                        me.data.BranchDesc = data.Description;
                }
                me.isSave = false;
                me.setAccountNo();
                me.clearTable(me.grid1);
                me.Apply();
            }
        });
    }

    me.CostCenter = function () {
        var lookup = Wx.blookup({
            name: "CostCenterLookup",
            title: "Lookup Cost Center",
            manager: gnManager,
            query: new breeze.EntityQuery().from("LookUpDtlAll").withParameters({ param: "CSTR" }),
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "Account Cost Center" },
                { field: "LookUpValueName", title: "Description" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.CostCtrCode = data.LookUpValue;
                me.data.CostCtrCodeDesc = data.LookUpValueName;
                me.setAccountNo();
                me.clearTable(me.grid1);
                me.Apply();
            }
        });
    };

    me.natural = function (p, desc) {
           //if (!p) { p=200}
           var lookup = Wx.blookup({
               name: "SegmentAcc",
               title: "Natural Account Lookup",
               manager: gnManager,
               query: new breeze.EntityQuery().from("SegmentAcc").withParameters({ param: p }),
               defaultSort: "SegAccNo asc",
               columns: [
               { field: "SegAccNo", title: "Account " + desc },
               { field: "Parent", title: "Account Type" + desc },
               { field: "Description", title: "Account " + desc + " Desc" }
               ]
           });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.NaturalAccount = data.SegAccNo;
                me.data.NaturalAccountDesc = data.Description
                me.data.AccountType = data.Parent;
                me.callAccount(data.Parent);
                me.setAccountNo();
                me.clearTable(me.grid1);
                me.Apply();
            }
        });
     }

    me.callAccount = function (a) {
         $http.post('gn.api/Chart/AccountDesc?lookupvalue='+ a ).
           success(function (data, status, headers, config) {
               if (data.success) {
                   me.data.Notes = data.data.LookUpValueName;
               } else {
                   MsgBox(data.message, MSG_ERROR);
               }
           });
     }

    me.setAccountNo = function () {
         var A = ""; var B = ""; var C = "."; var D = "."; var E = "."; var F = "."; var G = ".";
         var m = ""; var n = "."; var o = "."; var p = "."; var q = "."; var r = "."; var s = ".";
         if (me.data.Company) { A = (me.data.Company).replace(/\s/g, ''); }
         if (me.data.Branch) { B = (me.data.Branch).replace(/\s/g, ''); }
         if (me.data.CostCtrCode) { C = (me.data.CostCtrCode).replace(/\s/g, ''); }
         if (me.data.ProductType) { D = D + (me.data.ProductType).replace(/\s/g, ''); }
         if (me.data.NaturalAccount) { E = E + (me.data.NaturalAccount).replace(/\s/g, ''); }
         if (me.data.InterCompany) { F = F + (me.data.InterCompany).replace(/\s/g, ''); }
         if (me.data.Futureuse) { G = G + (me.data.Futureuse).replace(/\s/g, ''); }

        if (me.data.CompanyDesc) { m = me.data.CompanyDesc; }
        if (me.data.BranchDesc) { n = (me.data.BranchDesc).replace(/\s/g, ''); }
        if (me.data.CostCtrCodeDesc) { o = o + (me.data.CostCtrCodeDesc).replace(/\s/g, ''); }
        if (me.data.ProductTypeDesc) { p = p + (me.data.ProductTypeDesc).replace(/\s/g, ''); }
        if (me.data.NaturalAccountDesc) { q = q + (me.data.NaturalAccountDesc).replace(/\s/g, ''); }
        if (me.data.InterCompanyDesc) { r = r + (me.data.InterCompanyDesc).replace(/\s/g, ''); }
        if (me.data.FutureuseDesc) { s = s + (me.data.FutureuseDesc).replace(/\s/g, ''); }
         me.data.AccountNo = A + '.' + B + '.' + C + D + E + F + G;
         me.data.Description = m + "." + n + o + p + q + r + s;
        // A = $("#Company").val();
        // B = $("[name = 'Branch']").val();
        // C = $("[name = 'CostCtrCode']").val();
        // D = $("[name = 'ProductType']").val();
        // E = $("[name = 'NaturalAccount']").val();
        // F = $("[name = 'InterCompany']").val();
        // G = $("[name = 'Futureuse']").val();
        // alert(A);
        // if (A) { (A).replace(/\s/g, ''); }
        // if (B) { $("[name = 'Branch']").val().replace(/\s/g, ''); }
        // if (C) { $("[name = 'CostCtrCode']").val().replace(/\s/g, ''); }
        // if (D) { $("[name = 'ProductType']").val().replace(/\s/g, ''); }
        // if (E) { $("[name = 'NaturalAccount']").val().replace(/\s/g, ''); }
        // if (F) { $("[name = 'InterCompany']").val().replace(/\s/g, ''); }
        // if (G) { $("[name = 'Futureuse']").val().replace(/\s/g, ''); }
  
        //if ($("[name = 'CompanyDesc']").val()!="") { $("[name = 'CompanyDesc']").val().replace(/\s/g, ''); }
        //if ($("[name = 'BranchDesc']").val() != "") { $("[name = 'BranchDesc']").val().replace(/\s/g, ''); }
        //if ($("[name = 'CostCtrCodeDesc']").val() != "") { $("[name = 'CostCtrCodeDesc']").val().replace(/\s/g, ''); }
        //if ($("[name = 'ProductTypeDesc']").val() != "") { $("[name = 'ProductTypeDesc']").val().replace(/\s/g, ''); }
        //if ($("[name = 'NaturalAccountDesc']").val() != "") { $("[name = 'NaturalAccountDesc']").val().replace(/\s/g, ''); }
        //if ($("[name = 'InterCompanyDesc']").val() != "") { $("[name = 'InterCompanyDesc']").val().replace(/\s/g, ''); }
        //if ($("[name = 'FutureuseDesc']").val() != "") { $("[name = 'FutureuseDesc']").val().replace(/\s/g, ''); }
        //me.data.AccountNo = A + '.' + B + '.' + C + D + E + F + G;
         //me.data.Description = m + "." + n + o + p + q + r + s;
    }

    me.initialize = function () {
        //$('#test').attr("onkeypress", "myfunc(event)");
        me.data = {};
        me.data.FutureuseDesc = "";
        me.clearTable(me.grid1);
        $http.post('gn.api/Chart/Company').
           success(function (data, status, headers, config) {
               if (data.success) {
                   me.data.Company = data.data.SegAccNo;
                   me.data.CompanyDesc = data.data.Description;
                   me.data.Branch = data.data.SegAccNo1;
                   me.data.BranchDesc = data.data.Description1;
                  // me.Apply();
                   me.setAccountNo();
               } else {
                   MsgBox(data.message, MSG_ERROR);
               }
           });

        //$http.post('gn.api/Chart/Branch').
        //    success(function (data, status, headers, config) {
        //        if (data.success) {
        //            me.data.Branch = data.data.SegAccNo;
        //            me.data.BranchDesc = data.data.Description;
        //            //me.Apply();
                    
        //        } else {
        //            MsgBox(data.message, MSG_ERROR);
        //        }
        //    });

        me.data.FromDate = me.now();
        me.data.EndDate = me.now();
        me.data.Balance = 'D';
        me.data.Consol = 'N';
        me.data.oid = true;
    }

    $("[name = 'Branch']").on('blur', function () {
        if ($('#Branch').val() || $('#Branch').val() != '') {
            var SegAccNo = $('#Branch').val();
            $http.post('gn.api/Organisasi/CheckCompanyAccNo?SegAccNo=' + SegAccNo + '&TipeSegAcc=200').
            success(function (v, status, headers, config) {
                if (!v.success) {
                    me.data.Branch = '';
                    me.setAccountNo();
                    me.segAcc(200, 'Branch');
                } else {
                    me.setAccountNo();
                }
            });
        } else {
            $('#Branch').val('');
            me.setAccountNo();
            me.segAcc(200, 'Branch');
        }
    });

    $("[name = 'Futureuse']").on('blur', function () {
        if ($('#Futureuse').val() || $('#Futureuse').val() != '') {
            var SegAccNo = $('#Futureuse').val();
            $http.post('gn.api/Organisasi/CheckCompanyAccNo?SegAccNo=' + SegAccNo + '&TipeSegAcc=600').
            success(function (v, status, headers, config) {
                if (!v.success) {
                    $('#Futureuse').val('');
                    me.data.Futureuse = '';
                    me.setAccountNo();
                    me.segAcc(600, 'Future Use');
                } else {
                    me.setAccountNo();
                }
            });
        } else {
            $('#Futureuse').val('');
            me.setAccountNo();
            me.segAcc(600, 'Future Use');
        }
    });

    $("[name = 'InterCompany']").on('blur', function () {
        if ($('#InterCompany').val() || $('#InterCompany').val() != '') {
            var SegAccNo = $('#InterCompany').val();
            $http.post('gn.api/Organisasi/CheckCompanyAccNo?SegAccNo=' + SegAccNo + '&TipeSegAcc=500').
            success(function (v, status, headers, config) {
                if (!v.success) {
                    $('#InterCompany').val('');
                    me.data.InterCompany = '';
                    me.setAccountNo();
                    me.segAcc(500, 'Inter Company');
                } else {
                    me.setAccountNo();
                }
            });
        } else {
            $('#InterCompany').val('');
            me.setAccountNo();
            me.segAcc(500, 'Inter Company');
        }
    });

    $("[name = 'ProductType']").on('blur', function () {
        if ($('#ProductType').val() || $('#ProductType').val() != '') {
            var SegAccNo = $('#ProductType').val();
            $http.post('gn.api/Organisasi/CheckCompanyAccNo?SegAccNo=' + SegAccNo + '&TipeSegAcc=300').
            success(function (v, status, headers, config) {
                if (!v.success) {
                    $('#ProductType').val('');
                    me.data.ProductType = '';
                    me.setAccountNo();
                    me.segAcc(300, 'ProductT ype');
                } else {
                    me.setAccountNo();
                }
            });
        } else {
            $('#ProductType').val('');
            me.setAccountNo();
            me.segAcc(300, 'Product Type');
        }
    });

    $("[name = 'NaturalAccount']").on('blur', function () {
        if ($('#NaturalAccount').val() || $('#NaturalAccount').val() != '') {
            var SegAccNo = $('#NaturalAccount').val();
            $http.post('gn.api/Organisasi/CheckCompanyAccNo?SegAccNo=' + SegAccNo + '&TipeSegAcc=400').
            success(function (v, status, headers, config) {
                if (!v.success) {
                    $('#NaturalAccount').val('');
                    me.data.NaturalAccount = '';
                    me.setAccountNo();
                    me.natural(400, 'Natural Acc.');
                } else {
                    me.setAccountNo();
                }
            });
        } else {
            $('#NaturalAccount').val('');
            me.setAccountNo();
            me.natural(400, 'Natural Acc.');
        }
    });

    $("[name = 'CostCtrCode']").on('blur', function () {
        if ($('#CostCtrCode').val() || $('#CostCtrCode').val() != '') {
            $http.post('gn.api/Lookup/getLookupName?CodeId=CSTR&LookupValue=' + $('#CostCtrCode').val()).
            success(function (v, status, headers, config) {
                if (v.TitleName != '') {
                    me.setAccountNo();
                } else {
                    $('#CostCtrCode').val('');
                    me.data.CostCtrCode = '';
                    me.setAccountNo();
                    me.CostCenter();
                }
            });
        } else {
            $('#CostCtrCode').val('');
            me.setAccountNo();
            me.CostCenter();
        }
    });

    me.initGrid = function () {
        me.grid1 = new webix.ui({
            container: "wxsalestarget",
            view: "wxtable", css:"alternating", scrollX: true,
            columns: [
                { id: "AccountNo", header: "No. Account", width: 340 },
                { id: "Description", header: "Description", width: 610 },
                { id: "AccountType", header: "Account Type", width: 100 }
            ],

            on: {
                onSelectChange: function () {
                    console.log("");
                    //if (me.grid1.getSelectedId() !== undefined) {
                    //    me.data = this.getItem(me.grid1.getSelectedId().id);
                    //    me.data.oid = me.grid1.getSelectedId();
                    //    me.Apply();
                    //}
                }
            }
        });
    }

    me.initGrid();

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.saveData = function (e, param) {
        if (!me.data.CostCtrCode || !me.data.Futureuse || !me.data.ProductType || !me.data.AccountType || !me.data.NaturalAccount || !me.data.InterCompany) {
            MsgBox("Data belum terisi lengkap", MSG_ERROR);
        }
        else {
            if (me.data.AccountCode) { me.data.SegAccNo = me.data.AccountCode }
            $http.post('gn.api/Chart/Save', me.data).
                success(function (data, status, headers, config) {
                    if (data.status) {
                        Wx.Success("Data saved...");
                       // me.cancelOrClose();
                        me.data.oid = true;
                        me.grid.data = data;
                        me.loadTableData(me.grid1, me.grid.data);
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
            if (result) {
                $http.post('gn.api/Chart/delete', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.init();
                        Wx.Info("Record has been deleted...");
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    // MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    console.log(e);
                });
            }
        });
    }

    me.start();
}
$(document).ready(function () {
    var options = {
        title: "Chart of Account GL",
        xtype: "panels",
        toolbars: [{ name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || hasChanged || isInitialize", click: "browse()" }],//WxButtons,
        panels: [
            {
                name: "pnlInfo",
                title: "",
                items: [
                    //{ name: "test", type: "spinner" },
                         { name: "Company", model:"data.Company", type: "text", text: "Company Account", cls: "span4", disable: true },
                        // { name: "FutureuseDesc", model: "data.FutureuseDesc", type: "hidden" },
                         {
                             type: "controls", text: "Branch Account", cls: "span4", required: true, items: [
                                { name: "Branch", model: "data.Branch", type: "popup", text: "Branch Account", cls: "span8", maxlength: "3", click: "segAcc(200, 'Branch')", required: true, validasi: "required" },
                                { name: "BranchDesc", type: "hidden" },
                             ]
                         },
                          {
                              type: "controls", text: "Cost Center", cls: "span4", required: true, items: [
                                 { name: "CostCtrCode", type: "popup", text: "Cost Center", cls: "span8", click: "CostCenter()", required: true, validasi: "required", maxlength : 3},

                              ]
                          },
                         {
                             type: "controls", text: "Future Use", cls: "span4", required: true, items: [
                                { name: "Futureuse", type: "popup", text: "Future Use", cls: "span8", validasi: "max(15)", click: "segAcc(600, 'Future Use')", required: true, validasi: "required", maxlength: 3 },

                             ]
                         },
                         {
                             type: "controls", text: "Product Type", cls: "span4", required: true, items: [
                                { name: "ProductType", type: "popup", text: "Product Type", cls: "span8", validasi: "max(15)", click: "segAcc(300, 'Product Type')", required: true, validasi: "required", maxlength: 5 },

                             ]
                         },
                         
                          {
                              type: "controls", text: "Account Type", cls: "span4", required: true, items: [
                                  { name: "AccountType", type: "text", text: "Tipe Account", cls: "span2", disable: true },
                                  { name: "Notes", type: "text", text: "", cls: "span6", disable: true },
                              ]
                          },
                         {
                             type: "controls", text: "Natural Account", cls: "span4", required: true, items: [
                                { name: "NaturalAccount", type: "popup", text: "Natural Account", cls: "span8", click: "natural(400, 'Natural Acc.')", required: true, validasi: "required", maxlength: 6 },

                             ]
                         },
                         
                         { name: "AccountNo", type: "text", text: "No Account", cls: "span4", validasi: "max(15)", disable: true, required: true, validasi: "required" },
                         { name: "InterCompany", type: "popup", text: "Inter Company", cls: "span4", click: "segAcc(500,'Internal')", required: true, validasi: "required", maxlength: 3 },
                        
                         { name: "Description", type: "text", text: "Keterangan Account", cls: "span8", required: true, validasi: "required", },
                         { name: 'FromDate', type: 'ng-datepicker', text: 'Tgl. Mulai', cls: 'span4' },
                         { name: 'EndDate', type: 'ng-datepicker', text: 'Tgl. Akhir', cls: 'span4' },
                         { name: 'Balance', model: "data.Balance", type: 'select2', text: 'Balance', cls: 'span4', datasource: 'Debet' },
                         { name: 'Consol', model: "data.Consol", type: 'select2', text: 'Konsolidasi', cls: 'span4', datasource: 'Consol' },
                         {
                               type: "buttons",
                               items: [
                                       { name: "btnAddModel", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "save()", show: "data.oid !== undefined" },
                                       //{ name: "btnUpdateModel", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "save()", show: "data.oid !== undefined" },
                                       //{ name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete()", show: "data.oid !== undefined" },
                                       //{ name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "cancelOrClose()", show: "data.oid !== undefined" }
                               ]
                         },
                ]
            },
            {
                name: "wxsalestarget",
                xtype: "wxtable",
            },

        ]
    };
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spItemPriceController");
    }
});