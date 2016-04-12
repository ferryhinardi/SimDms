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

        $http.post('gn.api/inquiry/dealerinfo').
         success(function (data, status, headers, config) {
             me.data.Area = data[0].Area;
             me.data.DealerName = data[0].DealerName;
             me.data.Abbr = data[0].Abbr;
         }).error(function (e, status, headers, config) {
             MsgBox(e, MSG_ERROR);
         });

    }
 
    me.OnTabChange = function(e, id)
    {
       

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
    

    me.exportToXLS = function()
    {
        me.data.StartDate = moment(me.data.StartDate).format('YYYY-MM-DD')
        me.data.EndDate = moment(me.data.EndDate).format('YYYY-MM-DD')
        me.data.BranchName = $('#Branch').select2('data').text;

        $('.page > .ajax-loader').show();

        $.fileDownload('DoReport/Customers.xlsx', {
            httpMethod: "POST",
            //preparingMessageHtml: "We are preparing your report, please wait...",
            //failMessageHtml: "There was a problem generating your report, please try again.",
            data: me.data
        }).done(function () {
            $('.page > .ajax-loader').hide();
        });


    }

    me.execQuery = function () {
        var src = "gn.api/Customer/Inquiry";
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
                //{ field: "RowNumber", title: "No.", width: 100, align: "right" },
                { field: "CompanyCode", title: "Dealer Code", width: 120 },
                { field: "CustomerCode", title: "Customer Code", width: 120 },
                { field: "CustomerName", title: "Customer Name", width: 250 },
                { field: "CustomerStatus", title: "Customer Status", width: 150 },
                { field: "Address", title: "Address", width: 650 },
                { field: "ProvinceName", title: "Province Name", width: 350 },
                { field: "CityName", title: "City Name", width: 350 },
                { field: "ZipNo", title: "Zip No", width: 105 },
                { field: "KelurahanDesa", title: "Kelurahan / Desa", width: 210 },
                { field: "KecamatanDistrik", title: "Kecamatan / Distrik", width: 180 },
                { field: "KotaKabupaten", title: "Kota / Kabupaten", width: 150 },
                { field: "IbuKota", title: "Ibu Kota", width: 150 },
                { field: "PhoneNo", title: "Phone No", width: 150 },
                { field: "HPNo", title: "HP No", width: 180 },
                { field: "FaxNo", title: "Fax No", width: 180 },
                { field: "OfficePhoneNo", title: "Office Phone No", width: 180 },
                { field: "Email", title: "Email", width: 180 },
                { field: "Gender", title: "Gender", width: 180 },
                { field: "BirthDate", title: "Birth Date", width: 150, template: "#= (BirthDate===null) ? '' :  moment(BirthDate).format('DD MMM YYYY') #" },
                
                { field: "LastUpdateDate", title: "Last Update Date", width: 150, template: "#= (LastUpdateDate===null) ? '' :  moment(LastUpdateDate).format('DD MMM YYYY') #" }
            ]
        });
    }

    me.AllowPeriod = function()
    {
        return !me.data.AllowPeriod;
    }

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Inquiry Customer",
        xtype: "panels",
        toolbars:WxButtons,
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
                            { name: "EndDate", cls: "span2", placeHolder: "", type: "ng-datepicker", disable: "AllowPeriod()" },
                        ]
                    },
                    {
                        text: "Area",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "Area", readonly: true, text: "Area", cls: "span5 " },
                        ]
                    },
                    {
                        text: "Dealer",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "DealerName", readonly: true, text: "Dealer", cls: "span5 " },
                        ]
                    },
                    {
                        text: "Outlet",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "Branch", type: "select2", text: "Branch", cls: "span5 ", datasource: "comboBranch", opt_text: "ALL" }
                        ]
                    },
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

        SimDms.Angular("gnInquiryCustomersController");
    }


});