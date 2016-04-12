var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function SupplierController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('gn.api/Combo/TOP?IsShowedAll=true').
    success(function (data, status, headers, config) {
        me.comboTTPJ = data;
        //me.init(); 
    });

    me.comboISPKP = [{ "value": "1", "text": "ACTIVE" }, { "value": "0", "text": "NOT ACTIVE" }];

    function lookupName(codeid, value) {
        $http.post('gn.api/Lookup/getLookupName?CodeId=' + codeid + '&LookupValue=' + value).
        success(function (v, status, headers, config) {
            if (v.TitleName != '') {
                switch (codeid) {
                    case 'CITY':
                        me.data.CityName = v.TitleName;
                        break;
                    case 'AREA':
                        me.data.AreaName = v.TitleName;
                        break;
                    case 'PROV':
                        me.data.ProvinceName = v.TitleName;
                        break;
                    case 'PFCN':
                        me.detail2.ProfitCenterName = v.TitleName;
                        break;
                    case 'SPGR':
                        me.detail2.SupplierGradeName = v.TitleName;
                        break;
                    case 'BANK':
                        me.detail3.BankName = v.TitleName;
                        break;
                }
            } else {
                switch (codeid) {
                    case 'CITY':
                        $('#CityCode').val('');
                        $('#CityName').val('');
                        me.CityLkp();
                        break;
                    case 'AREA':
                        $('#AreaCode').val('');
                        $('#AreaName').val('');
                        me.AreaLkp();
                        break;
                    case 'PROV':
                        $('#ProvinceCode').val('');
                        $('#ProvinceName').val('');
                        me.ProvinceLkp();
                        break;
                    case 'PFCN':
                        $('#ProfitCenterCode').val('');
                        $('#ProfitCenterName').val('');
                        me.btnProfitCenter();
                        break;
                    case 'SPGR':
                        $('#SupplierGrade').val('');
                        $('#SupplierGradeName').val('');
                        me.btnSupGrade();
                        break;
                    case 'BANK':
                        $('#BankCode').val('');
                        $('#BankName').val('');
                        me.BankLkp();
                        break;
                }
            }
        });
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "SupplierCode",
            title: "Supplier Browse",
            manager: gnManager,
            query: "SuppliersBrowse",
            defaultSort: "SupplierCode asc",
            columns: [
            { field: "SupplierCode", title: "Supplier Code" },
            { field: "SupplierName", title: "Supplier Name" },
            ]
        });
        lookup.dblClick(function (data) {
            me.loadDetail(data);
        });
    }

    me.loadDetail = function (param) {
        $http.post('gn.api/Supplier/PopulateSupplierDetails', { "SupplierCode": param.SupplierCode }).
        success(function (result, status, headers, config) {
            if (result.success) {
                me.data = result.data;
                me.loadProfitCenterList(param.SupplierCode);
                me.loadBankList(param.SupplierCode);
                me.detail2.SupplierCode = param.SupplierCode;
                me.detail3.SupplierCode = param.SupplierCode;
                setTimeout(function () { me.startEditing(); }, 500);
                me.allowInputDetail = true;
            } else {
                MsgBox(result.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.PopulateProfitCenterDtl = function (SupplierCode, ProfitCenter) {
        $http.post('gn.api/Supplier/PopulateSupplierPCDetails', { "SupplierCode": SupplierCode, "ProfitCenterCode": ProfitCenter }).
        success(function (result, status, headers, config) {
            if (result.success) {
                me.detail2 = result.data;
                me.detail2.SupplierCode = me.data.SupplierCode;
                me.detail2.ProfitCenterCodeOld = result.data.ProfitCenterCode;
            } else {
                me.detail2.ProfitCenterCodeOld = undefined;
                MsgBox(result.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            me.detail2.ProfitCenterCodeOld = undefined;
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.loadProfitCenterList = function (SupplierCode) {
        $http.post('gn.api/Supplier/PopulateSupplierPCList', { "SupplierCode": SupplierCode }).
        success(function (result, status, headers, config) {
            if (result.success) {
                me.grid1.detail = result.data;
                me.loadTableData(me.grid1, me.grid1.detail);
            } else {
                MsgBox(result.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.loadBankList = function (SupplierCode) {
        $http.post('gn.api/Supplier/PopulateSupplierBankList', { "SupplierCode": SupplierCode }).
        success(function (result, status, headers, config) {
            if (result.success) {
                me.grid2.detail = result.data;
                me.loadTableData(me.grid2, me.grid2.detail);
                me.detail3.BankCodeOld = undefined;
            } else {
                me.detail3.BankCodeOld = undefined;
                MsgBox(result.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            me.detail3.BankCodeOld = undefined;
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    $('div > p').click(function () {
        var name = $(this).data("name");
        if (name == "tabBANK") {
            me.loadBankList(me.data.SupplierCode);
        } else if (name == "tabPC") {
            me.loadProfitCenterList(me.data.SupplierCode);
        }
    });

    me.grid1 = new webix.ui({
        container: "wxprofitcenter",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "ProfitCenterCode", header: "Kode", fillspace: true },
            { id: "ProfitCenterName", header: "Profit Center", fillspace: true },
            { id: "SupplierClass", header: "Supplier Class", fillspace: true },
            { id: "ContactPerson", header: "Contact Person", fillspace: true }
        ],
        checkboxRefresh: true,
        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    var data = this.getItem(me.grid1.getSelectedId().id);
                    me.PopulateProfitCenterDtl(data.SupplierCode, data.ProfitCenterCode);
                    me.Apply();
                }
            }
        }
    });

    me.grid2 = new webix.ui({
        container: "wxbank",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "BankCode", header: "BankCode", fillspace: true },
            { id: "BankName", header: "BankName", fillspace: true },
            { id: "AccountName", header: "Account Name", fillspace: true },
            { id: "AccountBank", header: "Account Bank", fillspace: true }
        ],
        checkboxRefresh: true,
        on: {
            onSelectChange: function () {
                if (me.grid2.getSelectedId() !== undefined) {
                    var data = this.getItem(me.grid2.getSelectedId().id);
                    me.detail3 = data;
                    me.detail3.BankCodeOld = data.BankCode;
                    me.Apply();
                }
            }
        }
    });

    me.setSupplierCode = function () {
        $http.post('gn.api/Supplier/checkSupplierCode').
         success(function (data, status, headers, config) {
             if (data == "True") {
                 $('#SupplierCode').attr("disabled", "disabled");
                 $('#StandardCode').attr("disabled", "disabled");
                 me.data.SupplierCode = "*******";
                 me.data.StandardCode = "*******";
             }
             else {
                 me.data.SupplierCode = "";
                 me.data.StandardCode = "";
             }
         });
    }

    me.initialize = function () {
        me.setSupplierCode();
        me.data.NPWPDate = me.now();
        //me.IsShowPanelB = false;
        //me.data.POSDate = me.now();
        //me.data.isBO = true;
        //me.data.Status = -1;
        //me.isPrintAvailable = true;
        //me.DataSelected = false;
        //me.data.isPKP = true;
        me.data.Status = "1";
        me.detail2 = {};
        me.detail3 = {};
        //me.datas = {};
        me.data.isLoad = false;
        me.clearTable(me.grid1);
        me.detail2.DiscPct = 0;
        me.detail2.isBlackList = false;
        me.detail2.ProfitCenterCodeOld = undefined;
        me.allowInputDetail = false;
        me.Apply();

        $http.post('gn.api/Supplier/OnPageLoad').
        success(function (data, status, headers, config) {
            me.control = {};
            me.control.IsDisableSC = true;
            me.control.IsNpWp = true;
            me.control.IsDisableNPWP = true;
            if (data.success) {
                me.control.IsDisableSC = data.IsDisableSC;
                me.data.isPKP = data.IsPkp === "true" || data.IsPkp === true;
                me.control.IsDisableIsPkp = data.IsDisableIsPkp;
                me.isAuto = data.IsDisableSC === "true" || data.IsDisableSC === true;;
            } else {
                MsgBox(data.message, MSG_ERROR);
                me.control.IsDisableSC = true;
            }
        });
        $('#btnProfitCenterCode,#btnTaxCode,#btnSupplierGrade,#btnSupplierClass').removeAttr("style");
        $('#btnBankCode').removeAttr("style");
    }

    $("#SupplierCode").on("blur", function () {
        var val = $(this).val();
        me.data.StandardCode = val;
        me.Apply();
    });

    me.delete = function () {
        MsgConfirm("Are you sure to delete a current record?", function (e) {
            if (e == true) {
                $http.post('gn.api/Supplier/DeleteSupplier', me.data).
                success(function (result, status, headers, config) {
                    if (result.success) {
                        Wx.Success(result.message);
                        //me.init();
                        //me.initialize();
                    } else {
                        MsgBox(result.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.saveData = function () {
        $http.post('gn.api/Supplier/SaveSupplier', me.data).
        success(function (result, status, headers, config) {
            if (result.success) {
                me.loadDetail(result);
                Wx.Success(result.message);
                me.startEditing();
                me.allowInputDetail = true;
            } else {
                MsgBox(result.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
}

    me.AddPC = function () {
        var Field = "ProfitCenterCode,TaxCode,SupplierGrade,SupplierClass,TOPCode";
        var Names = "Profit Center Code,Kode Pajak,Supplier Grade,Supplier Class,Pembayaran";
        var ret = me.CheckMandatory(Field, Names);
        if (ret != "") {
            MsgBox(ret + " Harus diisi terlebih dahulu !", MSG_INFO);
        } else {
            var valid = $(".main form").valid();
            if (valid) {
                me.detail2.SupplierCode = me.data.SupplierCode;
                var params = {
                    model: me.data,
                    supPCModel: me.detail2
                };
                $http.post('gn.api/Supplier/SaveProfitCenter', params).
                success(function (result, status, headers, config) {
                    if (result.success) {
                        Wx.Success(result.message);
                        me.detail2 = {};
                        me.loadProfitCenterList(me.data.SupplierCode);
                    } else {
                        MsgBox(result.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        }
        
    }

    me.DeletePC = function () {
        MsgConfirm("Apakah anda yakin?", function (e) {
            if (e == true) {
                me.detail2.SupplierCode = me.data.SupplierCode;
                $http.post('gn.api/Supplier/DeleteProfitCenter', me.detail2).
                success(function (result, status, headers, config) {
                    if (result.success) {
                        Wx.Success(result.message);
                        me.detail2 = {};
                        me.loadProfitCenterList(me.data.SupplierCode);
                        me.detail2.ProfitCenterCodeOld = undefined;
                    } else {
                        MsgBox(result.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
            else {
                me.ClearPC();
            }
        });
    }

    me.ClearPC = function () {
        me.detail2 = {};
        me.detail2.ProfitCenterCodeOld = undefined;
        me.Apply();
        me.loadTableData(me.grid1, me.grid1.detail);
        me.grid1.clearSelection();
    }

    me.AddBank = function () {
        var Field = "BankCode";
        var Names = "Kode Bank";
        var ret = me.CheckMandatory(Field, Names);
        if (ret != "") {
            MsgBox(ret + " Harus diisi terlebih dahulu !", MSG_INFO);
        } else {
            var valid = $(".main form").valid();
            if (valid) {
                me.detail3.SupplierCode = me.data.SupplierCode;
                $http.post('gn.api/Supplier/SaveBank', me.detail3).
                success(function (result, status, headers, config) {
                    if (result.success) {
                        Wx.Success("Data Supplier Bank saved...");
                        me.detail3 = {};
                        me.loadBankList(me.data.SupplierCode);
                        me.detail3.BankCodeOld = undefined;
                    } else {
                        me.detail3.BankCodeOld = undefined;
                        MsgBox(result.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    me.detail3.BankCodeOld = undefined;
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        }
        
    }

    me.DeleteBank = function () {
        MsgConfirm("Are you sure to delete a current record?", function (e) {
            if (e == true) {
                me.detail3.SupplierCode = me.data.SupplierCode;
                $http.post('gn.api/Supplier/DeleteBank', me.detail3).
                success(function (result, status, headers, config) {
                    if (result.success) {
                        Wx.Success(result.message);
                        me.detail3 = {};
                        me.loadBankList(me.data.SupplierCode);
                        me.detail3.BankCodeOld = undefined;
                    } else {
                        MsgBox(result.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
            else {
                me.ClearBank();
            }
        });
    }

    me.ClearBank = function () {
        me.detail3 = {};
        me.detail3.BankCodeOld = undefined;
        me.Apply();
        me.loadTableData(me.grid2, me.grid2.detail);
        me.grid2.clearSelection();
    }

    me.btnProfitCenter = function () {
        var lookup = Wx.blookup({
            name: "ProfitCenters",
            title: "Profit Center Lookup",
            manager: gnManager,
            query: "ProfitCenters",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "Profit Center Code" },
                { field: "LookUpValueName", title: "Profit Center Name" },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail2.ProfitCenterCode = data.LookUpValue;
            me.detail2.ProfitCenterName = data.LookUpValueName;
            me.Apply();
        });
    }

    me.btnSupGrade = function () {
        var lookup = Wx.blookup({
            name: "SupplierGrade",
            title: "Supplier Grade Lookup",
            manager: gnManager,
            query: "SupplierGrade",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "Supplier Grade Code" },
                { field: "LookUpValueName", title: "Supplier Grade" },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail2.SupplierGrade = data.LookUpValue;
            me.detail2.SupplierGradeName = data.LookUpValueName;
            me.Apply();
        });
    }

    me.btnTaxCode = function () {
        var lookup = Wx.blookup({
            name: "Taxes",
            title: "Tax Lookup",
            manager: gnManager,
            query: "Taxes",
            defaultSort: "TaxCode asc",
            columns: [
                { field: "TaxCode", title: "Tax Code" },
                { field: "TaxPct", title: "Jumlah Pajak" },
                { field: "Description", title: "Description" },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail2.TaxCode = data.TaxCode;
            me.detail2.TaxCodeName = data.Description;
            me.Apply();
        });
    }

    me.btnSupClass = function () {
        var lookup = Wx.blookup({
            name: "SupplierClass",
            title: "Supplier Class Lookup",
            manager: gnManager,
            query: new breeze.EntityQuery.from("SupplierClass4Lookup").withParameters({ ProfitCenterCode: (me.detail2.ProfitCenterCode==undefined) ? "" : me.detail2.ProfitCenterCode }),
            defaultSort: "SupplierClass asc",
            columns: [
                { field: "SupplierClass", title: "Supplier Class" },
                { field: "SupplierClassName", title: "Supplier Class Name" },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail2.SupplierClass = data.SupplierClass;
            me.detail2.SupplierClassName = data.SupplierClassName;
            me.Apply();
        });
    }

    //lookup
    me.ProvinceLkp = function () {
        var lookup = Wx.blookup({
            name: "ProvinceLkp",
            title: "Province Lookup",
            manager: gnManager,
            query: "KodeProvinsi",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "Province Code" },
                { field: "LookUpValueName", title: "Province Name" },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.ProvinceCode = data.LookUpValue;
            me.data.ProvinceName = data.LookUpValueName;

            //reset value
            me.data.CityCode = "";
            me.data.CityName = "";
            me.data.AreaCode = "";
            me.data.AreaName = "";
            me.Apply();
        });
    }

    me.CityLkp = function () {
        if (me.data.ProvinceCode != undefined) {
            var lookup = Wx.blookup({
                name: "CityLkp",
                title: "City Lookup",
                manager: gnManager,
                query: new breeze.EntityQuery().from("KodeCity").withParameters({ ProvinceCode: me.data.ProvinceCode }), //query: "KodeCity?ProvinceCode=" + me.data.ProvinceCode,
                defaultSort: "LookUpValue asc",
                columns: [
                { field: "LookUpValue", title: "City Code" },
                { field: "LookUpValueName", title: "City Name" },
                ]
            });
            lookup.dblClick(function (data) {
                me.data.CityCode = data.LookUpValue;
                me.data.CityName = data.LookUpValueName;
                me.Apply();
            });
        } else {
            SimDms.Warning("Province undefined, please fill province!");
        }
    }

    me.AreaLkp = function () {
        if (!me.data.CityCode) {
            SimDms.Warning("City undefined, please fill City!");
        } else {
            var lookup = Wx.blookup({
                name: "AreaLkp",
                title: "Area Lookup",
                manager: gnManager,
                query: "KodeArea?CityCode=" + me.data.CityCode,
                defaultSort: "LookUpValue asc",
                columns: [
                { field: "LookUpValue", title: "Area Code" },
                { field: "LookUpValueName", title: "Area Name" },
                ]
            });
            lookup.dblClick(function (data) {
                me.data.AreaCode = data.LookUpValue;
                me.data.AreaName = data.LookUpValueName;
                me.Apply();
            });
        }
    }

    me.BankLkp = function () {
        var lookup = Wx.blookup({
            name: "Banks",
            title: "Bank Lookup",
            manager: gnManager,
            query: "Banks",
            defaultSort: "LookUpValue asc",
            columns: [
            { field: "LookUpValue", title: "Bank Code" },
            { field: "LookUpValueName", title: "Bank Name" },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail3.BankCode = data.LookUpValue;
            me.detail3.BankName = data.LookUpValueName;
            me.Apply();
        });
    }

    //me.initGrid();

    webix.event(window, "resize", function () {
        //me.grid1.adjust();
    })

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Supplier",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
                    {
                        name: "pnlA",
                        title: "",
                        items: [
                                {
                                    text: "Supplier", type: "controls", type: "controls", required: true, items: [
                                      { name: "SupplierCode", model: "data.SupplierCode", type: "text", text: "Supplier Code", cls: "span2", validasi: "required", disable: "isAuto || isLoadData", maxlength: 15 },
                                      { name: "SupplierName", model: "data.SupplierName", cls: "span4", placeHolder: "Supplier Name", validasi: "required,max(100)" }]
                                },
                                { name: "StandardCode", model: "data.StandardCode", text: "Kode Standard", cls: "span3", validasi: "required", disable: "isAuto || isLoadData", maxlength: 15 },
                                { name: "SupplierGovName", text: "Nama Instansi", cls: "span5", validasi: "required", maxlength: 100 },
                                { name: "NPWPNo", type: "text", type: "ng-maskedit", placeHolder: "__.___.___._-___.___", mask: "##.###.###.#-###.###", text: "NPWP No", cls: "span3", validasi: "required" },
                                { name: "NPWPDate", text: "Tgl. NPWP", cls: "span3", type: "ng-datepicker" },
                                { name: "isPKP", model: "data.isPKP", text: "NPWP?", type: "ng-check", cls: "span2", disable: "control.IsDisableNPWP" },
                                {
                                    text: "Address", type: "controls", type: "controls", required: true, items: [
                                      { name: "Address1", cls: "span4", placeHolder: "Address1", validasi: "required", maxlength: 100 },
                                      { name: "Address2", cls: "span4", placeHolder: "Address2", validasi: "required", maxlength: 100 },
                                      { name: "Address3", cls: "span4", placeHolder: "Address3", maxlength: 100 },
                                      { name: "Address4", cls: "span4", placeHolder: "Address4", maxlength: 100 }]
                                },
                                {
                                    text: "Propinsi", type: "controls", type: "controls", required: true, items: [
                                      { name: "ProvinceCode", cls: "span2", placeHolder: "Code", type: "popup", btnName: "bnProvince", click: "ProvinceLkp()", validasi: "required", readonly: true },
                                      { name: "ProvinceName", cls: "span4", placeHolder: "Propinsi", readonly: true }]
                                },
                                {
                                    text: "Kota", type: "controls", type: "controls", required: true, items: [
                                      { name: "CityCode", cls: "span2", placeHolder: "Code", type: "popup", btnName: "btnCity", click: "CityLkp()", validasi: "required,max(15)", readonly: true },
                                      { name: "CityName", cls: "span4", placeHolder: "Kota", readonly: true }]
                                },
                                {
                                    text: "Area", type: "controls", type: "controls", required: true, items: [
                                      { name: "AreaCode", cls: "span2", placeHolder: "Code", type: "popup", btnName: "btnArea", click: "AreaLkp()", validasi: "required,max(15)", readonly: true },
                                      { name: "AreaName", cls: "span4", placeHolder: "Area", readonly: true }]
                                },
                                { name: "Telp", text: "Telphone", cls: "span3", maxlength: 15, type: "numeric" },
                                { name: "HPNo", text: "Handphone", cls: "span3", maxlength: 15, type: "numeric" },
                                { name: "FaxNo", text: "Fax No", cls: "span3", maxlength: 15, type: "numeric" },
                                { name: "ZipNo", text: "Kode Pos", cls: "span3", maxlength: 15, type: "numeric" },
                                { name: "Status", text: "Status", type: "select2", cls: "span3", datasource: 'comboISPKP' }
                        ]
                    },
                    {
                        xtype: "tabs",
                        name: "tabSP",
                        items: [
                            { name: "tabPC", text: "Profit Center" },
                            { name: "tabBANK", text: "Bank" },
                        ]
                    },
                    {
                        title: "Profit Center",
                        cls: "tabSP tabPC",
                        items: [
                            {
                                text: "Kode Profit Center", type: "controls", type: "controls", required: true, items: [
                                  { name: "ProfitCenterCode", model: "detail2.ProfitCenterCode", cls: "span2", placeHolder: "Profit Center Code", type: "popup", btnName: "btnPartNo", click: "btnProfitCenter()", style: "background-color: rgb(255, 218, 204)", readonly: true  },
                                  { name: "ProfitCenterName", model: "detail2.ProfitCenterName", cls: "span4", placeHolder: "Profit Center Name", readonly: true }]
                            },
                            {
                                text: "Diskon", type: "controls", type: "controls", items: [
                                      { name: "DiscPct", model: "detail2.DiscPct", cls: "span3 number", type: "decimal", min: 0, max: 100,  placeholder: "Diskon" },
                                      { name: "TOPCode", model: "detail2.TOPCode", text: "Transaction Type", type: "select2", cls: "span3", datasource: 'comboTTPJ', style: "background-color: rgb(255, 218, 204)"}
                                ]
                            },
                            {
                                text: "Supplier Grade", type: "controls", type: "controls", required: true, items: [
                                  { name: "SupplierGrade", model: "detail2.SupplierGrade", cls: "span2", placeHolder: "Supplier Grade", type: "popup", btnName: "btnSupGrade", click: "btnSupGrade()", readonly: true, style: "background-color: rgb(255, 218, 204)" },
                                  { name: "SupplierGradeName", model: "detail2.SupplierGradeName", cls: "span4", placeHolder: "Supplier Grade Name", readonly: true }]
                            },
                            { name: "ContactPerson", model: "detail2.ContactPerson", text: "Contact Person", cls: "span4", maxlength: 100 },
                            { name: "isBlackList", text: "Blacklist?", model: "detail2.isBlackList", type: "ng-check", cls: "span2" },
                            {
                                text: "Kode Pajak", type: "controls", type: "controls", required: true, items: [
                                  { name: "TaxCode", model: "detail2.TaxCode", cls: "span2", placeHolder: "Tax Code", type: "popup", btnName: "btnTaxCode", click: "btnTaxCode()", readonly: true, style: "background-color: rgb(255, 218, 204)" },
                                  { name: "TaxCodeName", model: "detail2.TaxCodeName", cls: "span4", placeHolder: "Tax Code Name", readonly: true }]
                            },
                            {
                                text: "Class Supplier", type: "controls", type: "controls", required: true, items: [
                                  { name: "SupplierClass", model: "detail2.SupplierClass", cls: "span2", placeHolder: "Supplier Class", type: "popup", btnName: "btnPartNo", click: "btnSupClass()", readonly: true, style: "background-color: rgb(255, 218, 204)" },
                                  { name: "SupplierClassName", model: "detail2.SupplierClassName", cls: "span4", placeHolder: "Supplier Class Name", readonly: true }]
                            },

                            {
                                type: "buttons", cls: "span4", items: [
                                    { name: "btnAddPC", text: "Add", icon: "icon-plus", click: "AddPC()", cls: "btn btn-primary", disable: "allowInputDetail == false" },
                                    { name: "btnDltPC", text: "Delete", icon: "icon-remove", click: "DeletePC()", cls: "btn btn-danger", disable: "detail2.ProfitCenterCodeOld === undefined" }
                                ]
                            },
                            {
                                name: "wxprofitcenter",
                                type: "wxdiv",
                            },
                        ]
                    },
                    {
                        title: "Bank",
                        cls: "tabSP tabBANK",
                        items: [
                            {
                                text: "Bank", type: "controls", type: "controls", required: true, items: [
                                  { name: "BankCode", model: "detail3.BankCode", cls: "span2", placeHolder: "Bank Code", type: "popup", btnName: "btnBank", click: "BankLkp()", style: "background-color: rgb(255, 218, 204)", readonly: true },
                                  { name: "BankName", model: "detail3.BankName", cls: "span4", placeHolder: "Bank Name", readonly: true }]
                            },
                            { name: "AccountName", model: "detail3.AccountName", text: "Nama Account", cls: "span6 full", maxlength: 100 },
                            { name: "AccountBank", model: "detail3.AccountBank", text: "Bank Account", cls: "span6 full", maxlength: 100 },
                            {
                                type: "buttons", cls: "span4", items: [
                                    { name: "btnAddB", text: "Add", icon: "icon-plus", click: "AddBank()", cls: "btn btn-primary", disable: "allowInputDetail == false" },
                                    { name: "btnDltB", text: "Delete", icon: "icon-remove", click: "DeleteBank()", cls: "btn btn-danger", disable: "detail3.BankCodeOld === undefined" }
                                ]
                            },
                            {
                                name: "wxbank",
                                type: "wxdiv",
                            },
                        ]
                    },
        ]

    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("SupplierController");
    }
});