var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterCustomersController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('gn.api/combo/LoadLookup?CodeId=PRDT').
    success(function (data, status, headers, config) {
        me.ProductTypes = data;
    });

    $http.post('gn.api/combo/LoadLookup?CodeId=GNDR').
    success(function (data, status, headers, config) {
        me.Genders = data;
    });

    $http.post('gn.api/combo/LoadLookup?CodeId=ESTM').
    success(function (data, status, headers, config) {
        me.Estimate = data;
    });

    function header(data) {
        $http.post('gn.api/CompanyProfile/Header?',me.data).
        success(function (data, status, headers, config) {
            if (data.data) {
                me.data = data.data;
                if (data.data.IsFPJCentralized == 1) {
                    $('#IsFPJCentralized').prop('checked', true);
                } else { $('#IsFPJCentralized').prop('checked', false); }
                if (data.data.IsLinkToSpare == 1) {
                    $('#IsLinkToSpare').prop('checked', true);
                } else { $('#IsLinkToSpare').prop('checked', false); }
                if (data.data.IsLinkToService == 1) {
                    $('#IsLinkToService').prop('checked', true);
                } else { $('#IsLinkToService').prop('checked', false); }
                if (data.data.IsLinkToSales == 1) {
                    $('#IsLinkToSales').prop('checked', true);
                } else { $('#IsLinkToSales').prop('checked', false); }
                if (data.data.IsLinkToFinance == 1) {
                    $('#IsLinkToFinance').prop('checked', true);
                } else { $('#IsLinkToFinance').prop('checked', false); }

                lookupName('CITY', me.data.CityCode, 'CityName');
                me.isSave = false;
                me.Apply();
               
            }
    });
    }

    function detailspare() {
        $http.post('gn.api/CompanyProfile/DetailSpare?', me.data).
        success(function (data, status, headers, config) {
            if (data.data) {
                me.pamodel = data.data;
                if (data.data.isPurchasePriceIncPPN == 1) {
                    $('#isPurchasePriceIncPPN').prop('checked', true);
                } else { $('#isPurchasePriceIncPPN').prop('checked', false); }
                if (data.data.isRetailPriceIncPPN == 1) {
                    $('#isRetailPriceIncPPN').prop('checked', true);
                } else { $('#isRetailPriceIncPPN').prop('checked', false); }
                if (data.data.IsLinkWRS == 1) {
                    $('#IsLinkWRS').prop('checked', true);
                } else { $('#IsLinkWRS').prop('checked', false); }
                //me.Apply();
            }
        });
    }

    function detailservice() {
        $http.post('gn.api/CompanyProfile/DetailService?', me.data).
        success(function (data, status, headers, config) {
            if (data.data) {
                me.pbmodel.FiscalPeriod = data.data.FiscalPeriod;
                me.pbmodel.FiscalMonth = data.data.FiscalMonth;
                me.pbmodel.FiscalYear = data.data.FiscalYear;
                me.pbmodel.PeriodBeg = data.data.PeriodBeg;
                me.pbmodel.PeriodEnd = data.data.PeriodEnd;
                me.pbmodel.TransDate = data.data.TransDate;
                me.pbmodel.MOUNo = data.data.MOUNo;
                me.pbmodel.MOUDate = data.data.MOUDate;
                me.pbmodel.BuildingOwnership = data.data.BuildingOwnership;
                me.pbmodel.LandOwnership = data.data.LandOwnership;
                me.pbmodel.PhoneNo = data.data.PhoneNo;
                me.pbmodel.HandPhoneNo = data.data.HandPhoneNo;
                me.pbmodel.FaxNo = data.data.FaxNo;
                me.pbmodel.EmailAddr = data.data.EmailAddr;
                me.pbmodel.ContactPersonName = data.data.ContactPersonName;
                me.pbmodel.EstimateTimeFlag = data.data.EstimateTimeFlag;
            }
        });
    }

    function detailsales() {
        $http.post('gn.api/CompanyProfile/DetailUnit?', me.data).
        success(function (data, status, headers, config) {
            if (data.data) {
                me.pcmodel.FiscalPeriod = data.data.FiscalPeriod;
                me.pcmodel.FiscalMonth = data.data.FiscalMonth;
                me.pcmodel.FiscalYear = data.data.FiscalYear;
                me.pcmodel.PeriodBeg = data.data.PeriodBeg;
                me.pcmodel.PeriodEnd = data.data.PeriodEnd;
                me.pcmodel.TransDate = data.data.TransDate;
                me.pcmodel.PhoneNo = data.data.PhoneNo;
                me.pcmodel.HandPhoneNo = data.data.HandPhoneNo;
                me.pcmodel.FaxNo = data.data.FaxNo;
                me.pcmodel.EmailAddr = data.data.EmailAddr;
                me.pcmodel.ContactPersonName = data.data.ContactPersonName;
            }
        });
    }

    function detailfinance() {
        $http.post('gn.api/CompanyProfile/DetailFinance?', me.data).
        success(function (data, status, headers, config) {
            if (data.data) {
                me.pdmodel.FiscalPeriod = data.data.FiscalPeriod;
                me.pdmodel.FiscalMonth = data.data.FiscalMonth;
                me.pdmodel.FiscalYear = data.data.FiscalYear;
                me.pdmodel.PeriodBeg = data.data.PeriodBeg;
                me.pdmodel.PeriodEnd = data.data.PeriodEnd;

                me.pdmodel.FiscalPeriodAR = data.data.FiscalPeriodAR;
                me.pdmodel.FiscalMonthAR = data.data.FiscalMonthAR;
                me.pdmodel.FiscalYearAR = data.data.FiscalYearAR;
                me.pdmodel.PeriodBegAR = data.data.PeriodBegAR;
                me.pdmodel.PeriodEndAR = data.data.PeriodEndAR;

                me.pdmodel.FiscalPeriodGL = data.data.FiscalPeriodGL;
                me.pdmodel.FiscalMonthGL = data.data.FiscalMonthGL;
                me.pdmodel.FiscalYearGL = data.data.FiscalYearGL;
                me.pdmodel.PeriodBegGL = data.data.PeriodBegGL;
                me.pdmodel.PeriodEndGL = data.data.PeriodEndGL;
            }
        });
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "MasterCompanyProfileBrowse",
            title: "Company Profile Browse",
            manager: gnManager,
            query: "AllBranch",
            defaultSort: "BranchCode asc",
            columns: [
            { field: "BranchCode", title: "Branch Code" },
            { field: "CompanyName", title: "Branch Name" },
            { field: "CompanyCode", title: "Company Code" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                me.isSave = false;
                if (data.IsFPJCentralized == 1) {
                    $('#IsFPJCentralized').prop('checked', true);
                } else { $('#IsFPJCentralized').prop('checked', false); }
                if (data.IsLinkToSpare == 1) {
                    $('#IsLinkToSpare').prop('checked', true);
                } else { $('#IsLinkToSpare').prop('checked', false); }
                if (data.IsLinkToService == 1) {
                    $('#IsLinkToService').prop('checked', true);
                } else { $('#IsLinkToService').prop('checked', false); }
                if (data.IsLinkToSales == 1) {
                    $('#IsLinkToSales').prop('checked', true);
                } else { $('#IsLinkToSales').prop('checked', false); }
                if (data.IsLinkToFinance == 1) {
                    $('#IsLinkToFinance').prop('checked', true);
                } else { $('#IsLinkToFinance').prop('checked', false); }
                //header(data);
                detailspare();
                detailservice();
                detailsales();
                detailfinance();
                $("#BranchCode").attr('disabled', 'disabled');
                
                me.Apply();
                //$("input[type=text].number").each(function (i, v) {
                //    alert($(this).val());
                //    var val0 = $(this).val();
                //    var valt = number_format(val0, 2);
                //    $(this).val(valt);
                //});
               
                $("input[type=text].number").each(function (i, v) {
                    //alert($(this).val());
                    var val0 = $(this).val();
                    var valt = number_format(val0, 2);
                    $(this).val(valt);
                });
                $("input[type=text].number").each(function (i, v) {
                   // alert($(this).val());
                    var val0 = $(this).val();
                    var valt = number_format(val0, 2);
                    $(this).val(valt);
                });
            }
           
        });
    }

    function CompName(BranchCode) {
        $http.post('gn.api/Lookup/getCompanyCode?branchCode=' + BranchCode ).
       success(function (v, status, headers, config) {
           if (v.TitleName != '') {
               me.data.CompanyCode = v.TitleName;
               header();
               detailspare();
               detailservice();
               detailsales();
           } else {
               me.init();
               me.Branch();
           }
       });
    }

    function lookupName(codeid, value, name) {
        $http.post('gn.api/Lookup/getLookupName?CodeId=' + codeid + '&LookupValue=' + value).
        success(function (v, status, headers, config) {
            if (v.TitleName != '') {
                switch (codeid) {
                    case 'CITY':
                        me.data.CityName = v.TitleName;
                        break;
                    case 'TRPJ':
                        break;
                    case 'CBPJ':
                        break;
                }
            }else{
                switch (codeid) {
                    case 'CITY':
                        $('#CityCode').val('');
                        $('#CityName').val('');
                        me.City();
                        break;
                    case 'TRPJ':
                        $('#TaxTransCode').val('');
                        me.TaxTrans();
                        break;
                    case 'CBPJ':
                        $('#TaxCabCode').val('');
                        me.TaxCab();
                        break;
                }
            }
        });
    }

    me.Branch = function () {
        me.init();
        var lookup = Wx.blookup({
            name: "BranchLookUp",
            title: "Lookup Branch",
            manager: gnManager,
            query: "GetOrganizationDtl",
            defaultSort: "BranchCode asc",
            columns: [
                { field: "BranchCode", title: "Branch Code" },
                { field: "BranchName", title: "Branch Name" },
                { field: "CompanyCode", title: "Company Code" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.BranchCode = data.BranchCode;
                me.data.CompanyCode = data.CompanyCode;
                me.data.CompanyName = data.BranchName;
                $("#BranchCode").attr('disabled', 'disabled');
                header();
                detailspare();
                detailservice();
                detailsales();
                
                me.Apply();
            }
        });
    };

    me.City = function () {
        var lookup = Wx.blookup({
            name: "CityLookup",
            title: "Lookup City",
            manager: gnManager,
            query: new breeze.EntityQuery().from("LookUpDtlAll").withParameters({ param: "CITY" }),
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "City Code" },
                { field: "LookUpValueName", title: "City Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.CityCode = data.LookUpValue;
                me.data.CityName = data.LookUpValueName;
                me.Apply();
            }
        });

    };

    me.TaxTrans = function () {
        var lookup = Wx.blookup({
            name: "TaxTransLookup",
            title: "Lookup Tax Trans",
            manager: gnManager,
            query: new breeze.EntityQuery().from("LookUpDtlAll").withParameters({ param: "TRPJ" }),
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "Tax Trans Code" },
                { field: "LookUpValueName", title: "Tax Trans Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.TaxTransCode = data.LookUpValue;
                me.Apply();
            }
        });

    };

    me.TaxCab = function () {
        var lookup = Wx.blookup({
            name: "TaxBranchLookup",
            title: "Lookup Tax Branch",
            manager: gnManager,
            query: new breeze.EntityQuery().from("LookUpDtlAll").withParameters({ param: "CBPJ" }),
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "Tax Branch Code" },
                { field: "LookUpValueName", title: "Tax Branch Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.TaxCabCode = data.LookUpValue;
                me.Apply();
            }
        });

    };

    me.Fiscal = function () {
        var lookup = Wx.blookup({
            name: "FiscalLookUp",
            title: "Lookup Fiscal",
            manager: gnManager,
            query: "periodes",
            defaultSort: "PeriodeNum asc",
            columns: [
                { field: "PeriodeNum", title: "Periode No." },
                { field: "FiscalMonth", title: "Fiscal Month" },
                { field: "PeriodeName", title: "PeriodeName" },
                { field: "FiscalYear", title: "Fiscal Year" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.pamodel.FiscalPeriod = data.PeriodeNum;
                me.pamodel.FiscalMonth = data.FiscalMonth;
                me.pamodel.FiscalYear = data.FiscalYear;
                me.pamodel.PeriodBeg = data.FromDate;
                me.pamodel.PeriodEnd = data.EndDate;
                me.pamodel.TransDate = me.now();
                me.Apply();
            }
        });
    };

    me.Fiscal1 = function () {
        var lookup = Wx.blookup({
            name: "FiscalLookUp",
            title: "Lookup Fiscal",
            manager: gnManager,
            query: "periodes",
            defaultSort: "PeriodeNum asc",
            columns: [
                { field: "PeriodeNum", title: "Periode No." },
                { field: "FiscalMonth", title: "Fiscal Month" },
                { field: "PeriodeName", title: "PeriodeName" },
                { field: "FiscalYear", title: "Fiscal Year" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.pbmodel.FiscalPeriod = data.PeriodeNum;
                me.pbmodel.FiscalMonth = data.FiscalMonth;
                me.pbmodel.FiscalYear = data.FiscalYear;
                me.pbmodel.PeriodBeg = data.FromDate;
                me.pbmodel.PeriodEnd = data.EndDate;
                me.pbmodel.TransDate = me.now();
                me.Apply();
            }
        });
    };

    me.Fiscal2 = function () {
        var lookup = Wx.blookup({
            name: "FiscalLookUp",
            title: "Lookup Fiscal",
            manager: gnManager,
            query: "periodes",
            defaultSort: "PeriodeNum asc",
            columns: [
                { field: "PeriodeNum", title: "Periode No." },
                { field: "FiscalMonth", title: "Fiscal Month" },
                { field: "PeriodeName", title: "PeriodeName" },
                { field: "FiscalYear", title: "Fiscal Year" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.pcmodel.FiscalPeriod = data.PeriodeNum;
                me.pcmodel.FiscalMonth = data.FiscalMonth;
                me.pcmodel.FiscalYear = data.FiscalYear;
                me.pcmodel.PeriodBeg = data.FromDate;
                me.pcmodel.PeriodEnd = data.EndDate;
                me.pcmodel.TransDate = me.now();
                me.Apply();
            }
        });
    };

    me.Fiscal3 = function () {
        var lookup = Wx.blookup({
            name: "FiscalLookUp",
            title: "Lookup Fiscal",
            manager: gnManager,
            query: "periodes",
            defaultSort: "PeriodeNum asc",
            columns: [
                { field: "PeriodeNum", title: "Periode No." },
                { field: "FiscalMonth", title: "Fiscal Month" },
                { field: "PeriodeName", title: "PeriodeName" },
                { field: "FiscalYear", title: "Fiscal Year" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.pdmodel.FiscalPeriod = data.PeriodeNum;
                me.pdmodel.FiscalMonth = data.FiscalMonth;
                me.pdmodel.FiscalYear = data.FiscalYear;
                me.pdmodel.PeriodBeg = data.FromDate;
                me.pdmodel.PeriodEnd = data.EndDate;
                me.Apply();
            }
        });
    };

    me.FiscalAR = function () {
        var lookup = Wx.blookup({
            name: "FiscalLookUp",
            title: "Lookup Fiscal",
            manager: gnManager,
            query: "periodes",
            defaultSort: "PeriodeNum asc",
            columns: [
                { field: "PeriodeNum", title: "Periode No." },
                { field: "FiscalMonth", title: "Fiscal Month" },
                { field: "PeriodeName", title: "PeriodeName" },
                { field: "FiscalYear", title: "Fiscal Year" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.pdmodel.FiscalPeriodAR = data.PeriodeNum;
                me.pdmodel.FiscalMonthAR = data.FiscalMonth;
                me.pdmodel.FiscalYearAR = data.FiscalYear;
                me.pdmodel.PeriodBegAR = data.FromDate;
                me.pdmodel.PeriodEndAR = data.EndDate;
                me.Apply();
            }
        });
    };

    me.FiscalGL = function () {
        var lookup = Wx.blookup({
            name: "FiscalLookUp",
            title: "Lookup Fiscal",
            manager: gnManager,
            query: "periodes",
            defaultSort: "PeriodeNum asc",
            columns: [
                { field: "PeriodeNum", title: "Periode No." },
                { field: "FiscalMonth", title: "Fiscal Month" },
                { field: "PeriodeName", title: "PeriodeName" },
                { field: "FiscalYear", title: "Fiscal Year" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.pdmodel.FiscalPeriodGL = data.PeriodeNum;
                me.pdmodel.FiscalMonthGL = data.FiscalMonth;
                me.pdmodel.FiscalYearGL = data.FiscalYear;
                me.pdmodel.PeriodBegGL = data.FromDate;
                me.pdmodel.PeriodEndGL = data.EndDate;
                me.Apply();
            }
        });
    };

    $("[name = 'CityCode']").on('blur', function () {
        if ($('#CityCode').val() || $('#CityCode').val() != '') {
            lookupName('CITY', $('#CityCode').val());
        } else {
            me.data.CityName = '';
            me.Apply();
        }
    });

    $("[name = 'TaxTransCode']").on('blur', function () {
        if ($('#TaxTransCode').val() || $('#TaxTransCode').val() != '') {
            lookupName('TRPJ', $('#TaxTransCode').val());
        }
    });

    $("[name = 'TaxCabCode']").on('blur', function () {
        if ($('#TaxCabCode').val() || $('#TaxCabCode').val() != '') {
            lookupName('CBPJ', $('#TaxCabCode').val());
        }
    });

    $("[name = 'BranchCode']").on('blur', function () {
        if ($('#BranchCode').val() || $('#BranchCode').val() != '') {
            CompName($('#BranchCode').val());
        } else {
            me.init();
            me.Branch();
        }
    });

    me.initialize = function () {
        me.pamodel = {};
        me.pamodel.BOPeriod = 0;
        me.pamodel.ABCClassAPct = number_format(0, 2);
        me.pamodel.ABCClassBPct = number_format(0, 2);
        me.pamodel.ABCClassCPct = number_format(0, 2);
        me.pamodel.TransDate = me.now();
        me.pbmodel = {};
        me.pbmodel.TransDate = me.now();
        me.pcmodel = {};
        me.pcmodel.TransDate = me.now();
        me.pdmodel = {};
        me.data.SKPDate = me.now();
        me.data.NPWPDate = me.now();
        $('#IsLinkToService').prop('checked', true);
        $('#IsLinkToSpare').prop('checked', true);
        $('#IsLinkToSales').prop('checked', true);
        $('#IsLinkToFinance').prop('checked', true);
        $('#BranchCode').removeAttr('disabled');
        me.hasChanged = false;
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('gn.api/CompanyProfile/Delete', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    me.delete2();
                    me.delete3();
                    me.delete4();
                    me.delete5();
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

    me.saveData = function (e, param) {
        //alert(me.data.isFPJCentralized);
        me.data.IsFPJCentralized = $('#IsFPJCentralized').prop('checked');
        me.data.IsLinkToSpare = $('#IsLinkToSpare').prop('checked');
        me.data.IsLinkToService = $('#IsLinkToService').prop('checked');
        me.data.IsLinkToSales = $('#IsLinkToSales').prop('checked');
        me.data.IsLinkToFinance = $('#IsLinkToFinance').prop('checked');
        
        $http.post('gn.api/CompanyProfile/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.status) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                    me.save2();
                    me.save3();
                    me.save4();
                    me.save5();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.save2 = function () {
        me.pamodel.BranchCode = me.data.BranchCode;
        me.pamodel.IsLinkWRS = $('#IsLinkWRS').prop('checked');
        me.pamodel.isRetailPriceIncPPN = $('#isRetailPriceIncPPN').prop('checked');
        me.pamodel.isPurchasePriceIncPPN = $('#isPurchasePriceIncPPN').prop('checked');
        me.pamodel.ContactPersonName = $('#ContactPersonName').val();
        $http.post('gn.api/CompanyProfile/Save2', me.pamodel).
            success(function (data, status, headers, config) {
                if (data.status) {
                    //Wx.Success("Data saved...");
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.save3 = function (e, param) {
        me.pbmodel.BranchCode = me.data.BranchCode;
        //alert(me.pbmodel.EstimateTimeFlag);
        $http.post('gn.api/CompanyProfile/Save3', me.pbmodel).
            success(function (data, status, headers, config) {
                if (data.status) {
                    //Wx.Success("Data saved...");
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.save4 = function (e, param) {
        me.pcmodel.BranchCode = me.data.BranchCode;
        $http.post('gn.api/CompanyProfile/Save4', me.pcmodel).
            success(function (data, status, headers, config) {
                if (data.status) {
                    //Wx.Success("Data saved...");
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.save5 = function (e, param) {
        me.pdmodel.BranchCode = me.data.BranchCode;
        $http.post('gn.api/CompanyProfile/Save5', me.pdmodel).
            success(function (data, status, headers, config) {
                if (data.status) {
                    //Wx.Success("Data saved...");
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }
    me.start();
}

$(document).ready(function () {
    var options = {
        title: 'Company Profile',
        xtype: 'panels',
        toolbars: WxButtons,
        panels: [
            {
                name: 'Company ',
                title: '',
                items: [
                    { name: 'IsLinkToSpare', type: 'check', text: 'Link Spare', cls: 'span2', float: 'left' },
                    { name: 'IsLinkToService', type: 'check', text: 'Link Service', cls: 'span2', float: 'left' },
                    { name: 'IsLinkToSales', type: 'check', text: 'Link Sales', cls: 'span2', float: 'left' },
                    { name: 'IsLinkToFinance', type: 'check', text: 'Link Finance', cls: 'span2', float: 'left' },
                    { name: 'IsFPJCentralized', type: 'check', text: 'Penomoran Pjk Terpusat', cls: 'span8', float: 'left' }
                ]
            },
             {
                 name: 'Company ',
                 title: '',
                 items: [
                      { name: 'BranchCode', model: 'data.BranchCode', type: 'popup', text: 'Kode Perusahaan', cls: 'span4', click: 'Branch()', required: true, validasi: "required" },
                      { name: 'CompanyCode', type: 'text', text: 'Kode Kantor Pusat', cls: 'span4', readonly: true }
                 ]
             },
              {
                  name: 'Company',
                  title: '',
                  items: [
                   { name: 'CompanyName', type: 'text', text: 'Nama', cls: 'span8', readonly: true },
                    { name: 'CompanyGovName', type: 'text', text: 'Nama Goverment', cls: 'span8', required: true, validasi: "required", maxlength: 100 },
                    { name: 'Address1', type: 'text', text: 'Alamat', cls: 'span8', maxlength: 100 },
                    { name: 'Address2', type: 'text', text: '', cls: 'span8', placeholder: 'Alamat', maxlength: 100 },
                    { name: 'Address3', type: 'text', text: '', cls: 'span8', placeholder: 'Alamat', maxlength: 100 },
                    { name: 'Address4', type: 'text', text: '', cls: 'span8', placeholder: 'Alamat', maxlength: 100 },
                    {
                        name: 'CityCategory',
                        type: 'controls',
                        text: 'Kota',
                        required: true,
                        items: [
                            { name: 'CityCode', model: 'data.CityCode', type: 'popup', text: 'Kode Kota', cls: 'span3', click: 'City()', required: true, validasi: "required" },
                            { name: 'CityName', model: 'data.CityName', type: 'text', text: 'Nama Kota', cls: 'span5', readonly: true }
                        ]
                    },
                    { name: 'ZipCode', type: 'text', text: 'Kode Pos', cls: 'span2', placeholder: 'Kode Pos', maxlength: 15 },
                    { name: 'PhoneNo', type: 'text', text: 'Telepon', cls: 'span3', placeholder: 'Telepon', maxlength: 15 },
                    { name: 'FaxNo', type: 'text', text: 'Fax', cls: 'span3', placeholder: 'No. Fax', maxlength: 15 },
                    { name: "NPWPNo", type: "text", type: "ng-maskedit", mask: "##.###.###.#-###.###", text: "No. NPWP", cls: "span4", required: true, validasi: "required" },
                    { name: 'NPWPDate', type: 'ng-datepicker', text: 'Tgl. NPWP', cls: 'span4' },
                    { name: "SKPNo", type: "text", type: "ng-maskedit", mask: "##.###.###.#-###.###", text: "SK. Pengukuhan", cls: "span4", required: true, validasi: "required" },
                    //{ name: 'SKPNo', type: 'text', text: 'SKP No', cls: 'span4', required: true, validasi: "required" },
                    { name: 'SKPDate', type: 'ng-datepicker', text: 'Tgl. Pengukuhan', cls: 'span4' },
                    { name: 'OwnershipName', type: 'text', text: 'Nama Pemilik', cls: 'span8', maxlength: 100 },
                    { name: 'ProductType', model: "data.ProductType", type: 'select2', text: 'Tipe Produk', cls: 'span3', datasource: 'ProductTypes', required: true, validasi: "required" },
                    { name: 'TaxTransCode', type: 'popup', text: 'Kd. Trans. Pajak ', cls: 'span2', click: 'TaxTrans()', required: true, validasi: "required" },
                    { name: 'TaxCabCode', type: 'popup', text: 'Kd.. Cabang Pajak', cls: 'span3', click: 'TaxCab()', required: true, validasi: "required" }
                  ]
              },
            {
                xtype: "tabs",
                name: "tabProfile",
                items: [
                    { name: "tabSparepart", text: "Sparepart", cls: "active" },
                    { name: "tabService", text: "Service" },
                    { name: "tabUnit", text: "Unit" },
                    { name: "tabFinance", text: "Finance" }
                ],

            },
            {
                name: "tabSparepart",
                cls: "tabProfile tabSparepart",
                items: [
                    { name: "FiscalPeriod", model: "pamodel.FiscalPeriod", type: "popup", text: "Fiscal Periode", cls: "span3 number-int", readonly: true, click: "Fiscal()" },
                    { name: "FiscalMonth", model: "pamodel.FiscalMonth", text: "Bulan Fiscal", cls: "span2 number-int", type: "text", readonly: true },
                    { name: "FiscalYear", model: "pamodel.FiscalYear", text: "Tahun Fiscal", cls: "span3 number-int", type: "text", readonly: true },
                    { name: "PeriodBeg", model: "pamodel.PeriodBeg", text: "Periode Transaksi", cls: "span3", type: "ng-datepicker", readonly: true },
                    { name: "PeriodEnd", model: "pamodel.PeriodEnd", text: "s/d", cls: "span3 ", type: "ng-datepicker", readonly: true },
                    { name: "TransDate", model: "pamodel.TransDate", text: "Tgl. Proses", cls: "span3 ", type: "ng-datepicker", readonly: true },
                    { type: "divider", text: "" },
                    { name: "isPurchasePriceIncPPN", model: "pamodel.isPurchasePriceIncPPN", text: "Harga Beli ATPM Termasuk PPN", cls: "span2", type: "check", float: "left" },
                    { name: "isRetailPriceIncPPN", model: "pamodel.isRetailPriceIncPPN", text: "Harga Jual ATPM Termasuk PPN", cls: "span2 ", type: "check", float: "left" },
                    { name: "IsLinkWRS", model: "pamodel.IsLinkWRS", text: "Binning to WRS", cls: "span2", type: "check", float: "left" },
                    { type: "divider" },
                    { name: "BOPeriod", model: "pamodel.BOPeriod", text: "BO. Periode", cls: "span3 number-int", type: "text", required: true, validasi: "required", maxlength: 4 },
                    { name: "ContactPersonName", model: "pamodel.ContactPersonName", text: "Contact Person", cls: "span5", type: "text", required: true, validasi: "required", maxlength: 100 },
                    { name: "PhoneNo", model: "pamodel.PhoneNo", text: "Telp", cls: "span3", type: "text", maxlength: 15 },
                    { name: "HandPhoneNo", model: "pamodel.HandphoneNo", text: "Hand Phone", cls: "span5", type: "text", maxlength: 15 },
                    { name: "FaxNo", model: "pamodel.FaxNo", text: "Fax", cls: "span3", type: "text", maxlength: 15 },
                    { name: "EmailAddr", model: "pamodel.EmailAddr", text: "Email", cls: "span5 ", type: "text", maxlength: 15 },
                    { type: "divider", text: "ABC Class %" },
                    { name: "ABCClassAPct", model: "pamodel.ABCClassAPct", text: "A Class(%)", cls: "span2 number", type: "text", required: true, validasi: "required", maxlength: 7 },
                    { name: "ABCClassBPct", model: "pamodel.ABCClassBPct", text: "B Class(%)", cls: "span2 number", type: "text", required: true, validasi: "required", maxlength: 7 },
                    { name: "ABCClassCPct", model: "pamodel.ABCClassCPct", text: "C Class(%)", cls: "span2 number", type: "text", required: true, validasi: "required", maxlength: 7 },
                    { type: "divider", text: "" }
                ]
            },
            {

                name: "tabService",
                cls: "tabProfile tabService",
                items: [
                    { name: "FiscalPeriodSr", model: "pbmodel.FiscalPeriod", type: "popup", text: "Fiscal Periode", cls: "span3 number-int", readonly: true, click: "Fiscal1()" },
                    { name: "FiscalMonthSr", model: "pbmodel.FiscalMonth", text: "Bulan Fiscal", cls: "span2 number-int", type: "text", readonly: true },
                    { name: "FiscalYearSr", model: "pbmodel.FiscalYear", text: "Tahun Fiscal", cls: "span3 number-int", type: "text", readonly: true },
                    { name: "PeriodBegSr", model: "pbmodel.PeriodBeg", text: "Periode Transaksi", cls: "span3 ", type: "ng-datepicker", readonly: true },
                    { name: "PeriodEndSr", model: "pbmodel.PeriodEnd", text: "s/d", cls: "span3 ", type: "ng-datepicker", readonly: true },
                    { name: "TransDateSr", model: "pbmodel.TransDate", text: "Tgl. Proses", cls: "span3 ", type: "ng-datepicker", readonly: true },
                    { type: "divider", text: "" },
                    { name: "MOUNo", model: "pbmodel.MOUNo", text: "No. Mou", cls: "span5", type: "text" },
                    { name: "MOUDate", model: "pbmodel.MOUDate", text: "Tgl. Mou", cls: "span3", type: "ng-datepicker" },
                    { type: "divider", text: "" },
                    { name: "BuildingOwnership", model: "pbmodel.BuildingOwnership", text: "Bangunan", cls: "span5", type: "text", maxlength: 100 },
                    { name: "LandOwnership", model: "pbmodel.LandOwnership", text: "Tanah", cls: "span3 ", type: "text", maxlength: 100 },
                    { type: "divider", text: "" },
                    { name: "PhoneNo", model: "pbmodel.PhoneNo", text: "Telp", cls: "span4", type: "text", maxlength: 20 },
                    { name: "HandPhoneNo", model: "pbmodel.HandPhoneNo", text: "Hand Phone", cls: "span4", type: "text", maxlength: 20 },
                    { name: "FaxNo", model: "pbmodel.FaxNo", text: "Fax", cls: "span4", type: "text", maxlength: 20 },
                    { name: "EmailAddr", model: "pbmodel.EmailAddr", text: "Email", cls: "span4 ", type: "text", maxlength: 100 },
                    { name: "ContactPersonName", model: "pbmodel.ContactPersonName", text: "Contact Person", cls: "span4", type: "text", maxlength: 100 },
                    { name: 'EstimateTimeFlag', model: "pbmodel.EstimateTimeFlag", type: 'select2', text: 'Perhitungan Waktu', cls: 'span4', datasource: 'Estimate' }
                ]
            },
            {
                name: "tabUnit",
                cls: "tabProfile tabUnit",
                items: [
                     { name: "FiscalPeriodUn", model: "pcmodel.FiscalPeriod", type: "popup", text: "Fiscal Periode", cls: "span3 number-int-int", readonly: true, click: "Fiscal2()" },
                    { name: "FiscalMonthUn", model: "pcmodel.FiscalMonth", text: "Bulan Fiscal", cls: "span2 number-int", type: "text", readonly: true },
                    { name: "FiscalYearUn", model: "pcmodel.FiscalYear", text: "Tahun Fiscal", cls: "span3 number-int", type: "text", readonly: true },
                    { name: "PeriodBegUn", model: "pcmodel.PeriodBeg", text: "Periode Transaksi", cls: "span3 ", type: "ng-datepicker", readonly: true },
                    { name: "PeriodEndUn", model: "pcmodel.PeriodEnd", text: "s/d", cls: "span3 ", type: "ng-datepicker", readonly: true },
                    { name: "TransDateUn", model: "pcmodel.TransDate", text: "Tgl. Proses", cls: "span3 ", type: "ng-datepicker", readonly: true },
                     { type: "divider", text: "" },
                    { name: "PhoneNo", model: "pcmodel.PhoneNo", text: "Telp", cls: "span3", type: "text", maxlength: 15 },
                    { name: "HandPhoneNo", model: "pcmodel.HandPhoneNo", text: "Hand Phone", cls: "span5", type: "text", maxlength: 15 },
                    { name: "FaxNo", model: "pcmodel.FaxNo", text: "Fax", cls: "span3 ", type: "text", maxlength: 15 },
                    { name: "EmailAddr", model: "pcmodel.EmailAddr", text: "Email", cls: "span5 ", type: "text", maxlength: 15 },
                    { name: "ContactPersonName", model: "pcmodel.ContactPersonName", text: "Contact Person", cls: "span8", type: "text", maxlength: 100 },
                ]
            },
             {
                 name: "tabFinance",
                 cls: "tabProfile tabFinance",
                 items: [
                    { name: "FiscalPeriodFn", model: "pdmodel.FiscalPeriod", type: "popup", text: "Fiscal Periode", cls: "span3 number-int", readonly: true, click: "Fiscal3()" },
                    { name: "FiscalMonthFn", model: "pdmodel.FiscalMonth", text: "Bulan Fiscal", cls: "span2 number-int", type: "text", readonly: true },
                    { name: "FiscalYearFn", model: "pdmodel.FiscalYear", text: "Tahun Fiscal", cls: "span3 number-int", type: "text", readonly: true },
                    { name: "PeriodBegFn", model: "pdmodel.PeriodBeg", text: "Tgl. Transaksi", cls: "span3 ", type: "ng-datepicker", readonly: true },
                    { name: "PeriodEndFn", model: "pdmodel.PeriodEnd", text: "s/d", cls: "span3", type: "ng-datepicker", readonly: true },
                    { type: "divider", text: "" },
                    { name: "FiscalPeriodAR", model: "pdmodel.FiscalPeriodAR", type: "popup", text: "Fiscal Periode", cls: "span3 number-int", readonly: true, click: "FiscalAR()" },
                    { name: "FiscalMonthAR", model: "pdmodel.FiscalMonthAR", text: "bulan Fiscal", cls: "span2 number-int", type: "text", readonly: true },
                    { name: "FiscalYearAR", model: "pdmodel.FiscalYearAR", text: "Tahun Fiscal", cls: "span3 number-int", type: "text", readonly: true },
                    { name: "PeriodBegAR", model: "pdmodel.PeriodBegAR", text: "Tgl. Transaksi", cls: "span3 ", type: "ng-datepicker", readonly: true },
                    { name: "PeriodEndAR", model: "pdmodel.PeriodEndAR", text: "s/d", cls: "span3", type: "ng-datepicker", readonly: true },
                    { type: "divider", text: "" },
                    { name: "FiscalPeriodGL", model: "pdmodel.FiscalPeriodGL", type: "popup", text: "Fiscal Periode ", cls: "span3 number-int", readonly: true, click: "FiscalGL()" },
                    { name: "FiscalMonthGL", model: "pdmodel.FiscalMonthGL", text: "Bulan Fiscal", cls: "span2 number-int", type: "text", readonly: true },
                    { name: "FiscalYearGL", model: "pdmodel.FiscalYearGL", text: "Tahun Fiscal", cls: "span3 number-int", type: "text", readonly: true },
                    { name: "PeriodBegGL", model: "pdmodel.PeriodBegGL", text: "Tgl. Transaksi", cls: "span3 ", type: "ng-datepicker", readonly: true },
                    { name: "PeriodEndGL", model: "pdmodel.PeriodEndGL", text: "s/d", cls: "span3", type: "ng-datepicker", readonly: true }

                 ]
             }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("gnMasterCustomersController");
    }
});