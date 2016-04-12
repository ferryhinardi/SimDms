var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spInquiryDataPelangganController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });
 
    me.browse = function () {
        var lookup = Wx.blookup({
            name: "CustomersBrowse",
            title: "Customers Browse",
            manager: gnManager,
            query: "CustomersAll",
            defaultSort: "CustomerCode asc",
            columns: [
                { field: "CustomerCode", title: "Customer Code" },
                { field: "CustomerName", title: "Customer Name" },
                { field: "Address1", title: "Address" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);

                $http.post('sp.api/MasterItem/GetCustomerKontrak?CUSCODE=' + data.CustomerCode + '&CITYCODE=' + data.CityCode ).
                success(function (data, status, headers, config) {
                    if ( data.success)
                    {
                        me.data.ContractNo = data.kontrak;
                        me.data.Kota = data.kota;  
                        me.data.ClassCustomer = data.kelas;
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
        title: "Inquiry Customer",
        xtype: "panel",
        items: [
               
                { name: "CustomerCode", text: "Kode Pelanggan", cls: "span4 ", type: "popup", btnName: "btnX",  click:"browse()"},
                { name: "CustomerName", text: "Nama Pelanggan", cls: "span8", readonly: true },
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
                { name: "Kota", text: "Kota", cls: "span4 full", readonly: true },
                { name: "ClassCustomer", text: "Class Pelanggan", cls: "span4 full", readonly: true },
            ]   
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spInquiryDataPelangganController"); 
    }

});