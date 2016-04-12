var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spInquiryDataPemasokController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });
 
    me.browse = function () {
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
                me.data = data;

                $http.post('sp.api/MasterItem/GetSupplierKontrak?SPCODE=' + data.SupplierCode + '&CITYCODE=' + data.CityCode ).
                success(function (data, status, headers, config) {
                    if ( data.success)
                    {
                        me.data.ContractNo = data.kontrak;
                        me.data.Kota = data.kota;  
                        me.Apply();                      
                    }
                });
                me.Apply();
            }
        });
    }

    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Inquiry Supplier",
        xtype: "panel",
        items: [
               
                { name: "SupplierCode", text: "Kode Supplier", cls: "span4 ", type: "popup", btnName: "btnX",  click:"browse()"},
                { name: "SupplierName", text: "Nama Supplier", cls: "span8", readonly: true },
                { name: "Address1", text: "Alamat", cls: "span8", readonly: true },
                { name: "Address2", text: "", cls: "span8", readonly: true },
                { name: "Address3", text: "", cls: "span8", readonly: true },
                { name: "Address4", text: "", cls: "span8", readonly: true },
                {
                    text: "Telepon / Fax",
                    type: "controls",
                    items: [
                        { name: "PhoneNo", cls: "span4",  readonly: true },
                        { name: "FaxNo",cls: "span4",  readonly: true }
                    ]
                },                
                { name: "ContractNo", text: "Kontrak Kerja", cls: "span4 full", readonly: true },
                { name: "Kota", text: "Kota", cls: "span4 full ", readonly: true },
            ]   
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spInquiryDataPemasokController"); 
    }

});