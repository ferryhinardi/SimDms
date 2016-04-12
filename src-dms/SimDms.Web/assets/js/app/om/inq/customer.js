var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnInquiryCustomersController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('gn.api/Combo/Branchs').
     success(function (data, status, headers, config) {
         me.comboBranch = data;
     });



    me.initialize = function () {
        me.grid = {};
        var ym = me.now("YYYY-MM") + "-01";
        me.data.StartDate = moment(ym);
        me.data.EndDate = moment(ym).add("months", 1).add("days", -1);
        me.data.AllowPeriod = false;
        me.data.Branch = '';
        me.hasChanged = false;
        me.isEQAvailable = true;
        me.isEXLSAvailable = true;

        $http.post('om.api/ReportSales/Periode').
          success(function (e) {
              me.data.DateFrom = e.DateFrom;
              me.data.DateTo = e.DateTo;
          });
        $http.post('om.api/ReportSales/CheckArea').
          success(function (e) {
              me.data.Area = e.Area;
              me.data.Dealer = e.Dealer;
          });

    }

    me.OnTabChange = function (e, id) {


    }


    me.obj_to_query = function (obj) {
        var parts = [];
        for (var key in obj) {
            if (obj.hasOwnProperty(key)) {
                parts.push(encodeURIComponent(key) + '=' + encodeURIComponent(obj[key]));
            }
        }
        return "?" + parts.join('&');
    }


    me.exportToXLS = function () {
        me.data.StartDate = moment(me.data.StartDate).format('YYYY-MM-DD')
        me.data.EndDate = moment(me.data.EndDate).format('YYYY-MM-DD')
        me.data.BranchName = $('#Branch').select2('data').text;

        $('.page > .ajax-loader').show();

        $.fileDownload('DoReport/Customers.xlsx', {
            httpMethod: "POST",
            data: me.data
        }).done(function () {
            $('.page > .ajax-loader').hide();
        });


    }

    me.execQuery = function () {
        var src = "om.api/ReportSales/InquiryCustomer";
        me.data.StartDate = moment(me.data.StartDate).format('YYYY-MM-DD')
        me.data.EndDate = moment(me.data.EndDate).format('YYYY-MM-DD')
        me.data.BranchName = $('#Branch').select2('data').text;

        console.log(me.data)

        $('.page > .ajax-loader').show();

        $http.post(src, me.data)
            .success(function (v, status, headers, config) {
                $('.page > .ajax-loader').hide();
                if (v.data !== undefined)
                    me.refreshCustomers(v.data);
                else
                    MsgBox(v.message, MSG_ERROR);

            }).error(function (e, status, headers, config) {
                $('.page > .ajax-loader').hide();
                MsgBox(e, MSG_ERROR);
            });
    }

    me.refreshCustomers = function (result) {
        Wx.kgrid({
            data: result,
            name: "InqCustomer",
            serverBinding: false,
            columns: [
                { field: "DealerAbbreviation", title: "Dealer Name", width: 150 },
            { field: "CustomerCode", title: "Customer Code", width: 150 },
            { field: "CustomerName", title: "Customer Name", width: 250 },
            { field: "CustomerGovName", title: "Customer GovName", width: 250 },
            { field: "CustomerStatus", title: "Customer Status", width: 150 },
            { field: "Address", title: "Address", width: 650 },
            { field: "ProvinceName", title: "Province Name", width: 350 },
            { field: "CityName", title: "City Name", width: 350 },
            { field: "ZipNo", title: "ZipNo", width: 110 },
            { field: "KelurahanDesa", title: "Kelurahan Desa", width: 200 },
            { field: "KecamatanDistrik", title: "Kecamatan/Distrik", width: 200 },
            { field: "KotaKabupaten", title: "Kota/Kabupaten", width: 150 },
            { field: "IbuKota", title: "Ibu Kota", width: 150 },
            { field: "PhoneNo", title: "Phone No", width: 150 },
            { field: "HPNo", title: "HP No", width: 180 },
            { field: "FaxNo", title: "Fax No", width: 180 },
            { field: "OfficePhoneNo", title: "Office Phone No", width: 180 },
            { field: "Email", title: "Email", width: 180 },
            { field: "Gender", title: "Gender", width: 180 },
            { field: "BirthDate", title: "BirthDate", width: 150, template: "#= (BirthDate===null) ? '' :  moment(BirthDate).format('DD MMM YYYY') #" },
            { field: "isPKP", title: "isPKP", width: 150 },
            { field: "NPWPNo", title: "NPWP No", width: 250 },
            { field: "NPWPDate", title: "NPWP Date", width: 150, template: "#= (NPWPDate===null) ? '' :  moment(NPWPDate).format('DD MMM YYYY') #" },
            { field: "SKPNo", title: "SKP No", width: 150 },
            { field: "SKPDate", title: "SKP Date", width: 150, template: "#= (SKPDate===null) ? '' :  moment(SKPDate).format('DD MMM YYYY') #" },
            { field: "CustomerType", title: "Customer Type", width: 150 },
            { field: "CustomerTypeDesc", title: "Customer Type Desc", width: 250 },
            { field: "CategoryCode", title: "Category Code", width: 150 },
            { field: "CategoryDesc", title: "Category Desc", width: 150 },
            { field: "Status", title: "Status", width: 150 },
            ]
        });
    }

    me.AllowPeriod = function () {
        return !me.data.AllowPeriod;
    }

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Inquiry Customer",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "Data Customer",
                title: "Filter",
                items: [

                    {
                        text: "Periode",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "AllowPeriod", type: "x-switch", text: "", cls: "span1", float: "left" },
                            { name: "StartDate", cls: "span2", placeHolder: "", type: "ng-datepicker", disable: "AllowPeriod()" },
                            { type: "label", text: "s.d", cls: "span1 mylabel" },
                            { name: "EndDate", cls: "span2", placeHolder: "", type: "ng-datepicker", disable: "AllowPeriod()" },
                        ]
                    },
                    { name: "Area", cls: "span4 full", text: "Area", type: "popup", click: "Area()" },
                    { name: "Dealer", cls: "span4 full", text: "Dealer", type: "popup", click: "Dealer()" },
                ]
            },
            {
                xtype: "tabs",
                name: "tabCustomer",
                items: [
                    { name: "tabList", text: "Customer List" },
                ],

            },
            {
                name: "InqCustomer",
                xtype: "k-grid",
            },
            //{
            //    name: "tabProfitCenter",
            //    cls: "tabCustomer tabList",
            //    items: [

            //    ]
            //},
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("gnInquiryCustomersController");
    }


});