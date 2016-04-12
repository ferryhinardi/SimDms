var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function SpMovingController($scope, $http, $injector) {

    // Untuk mempermudah penulisan kode program, define variable me sebagai $scope (Alias)
    var me = $scope;

    // Inheritance / implementasi metode pewarisan pada object/class angularjs controller
    $injector.invoke(BaseController, this, {$scope: me});

    // initialisasi / reset
    me.initialize = function()
    {

    }

    // show popup dialog to select existing record
    me.browse = function()
    {		
        var lookup = Wx.blookup({
            name: "movingcodeBrowse",
            title: "Moving Code Browse",
            manager: spManager,
            query: "movingcode",
            defaultSort: "MovingCode asc",
            columns: [
                { field: 'MovingCode',      title: 'Moving Code' },
                { field: 'MovingCodeName',  title: 'Moving Code  Name'},
                { field: 'Param1',  title: 'Param1', type: "number" },
                { field: 'Sign1',   title: 'Sign1' },
                { field: 'Param2',  title: 'Param2', type: "number" },
                { field: 'Sign2',   title: 'Sign2' },
            ],
        });

        // fungsi untuk menghandle double click event pada k-grid / select button
        lookup.dblClick(function (data) {
            if (data != null) {                
                me.lookupAfterSelect(data);                
                me.Apply();
                me.isSave = false;      
            }
        });
    }
	
    // save data, posting data from user cache to the server
	me.saveData = function(e, param)
	{	
		// Using $http provider from angular to posting data to the server	
		$http.post('sp.api/movingcode/save', me.data).
		success(function(data, status, headers, config) {

            if (data.success){
                Wx.Success("Data saved...");
                me.startEditing();              
            } else {
                // show an error message
                MsgBox(data.message, MSG_ERROR);
            }
		}).
		error(function(data, status, headers, config) {
			//MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            console.log(data); 
		});
	}

    me.delete = function()
    {
        // show confirmation dialog to the user
        // check respond, if true, notify to the server to remove current record
        MsgConfirm("Are you sure to delete current record?",function(result)
        {
            if (result)
            {
                // call web api by $http provider (async mode)
                $http.post('sp.api/movingcode/delete', me.data)
                    .success(function(v, status, headers, config){
                        if(v.success)
                        {
                            Wx.Info("Record has been deleted...");
                            me.init();
                        } else {
                            // show an error message
                            MsgBox(v.message, MSG_ERROR);
                        }
                    }).error(function(e, status, headers, config) {
                       // MsgBox(e, MSG_ERROR);
                        console.log(e); 
                    });
            }           
        })        
    }

    // define object untuk inisialisasi data selection
    me.dataOption = [{ value: '<  ', text: '<'  },
                    { value: '<= ', text: '<=' },
                    { value: '=  ' , text: '='  },
                    { value: '>  ' , text: '>'  },
                    { value: '>=  ', text: '>=' }];                                       
                                    

    me.start();    
}

$(document).ready(function () {

    var options = {
        serviceName: "breeze/sparepart",
        title: "Moving Code",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlMC",
                title: "Moving Code",
                items: [

                        { name: "MovingCode", text: "Moving Code", cls: "span4", disable: "IsEditing()", validasi:"required" },
                        { name: "MovingCodeName", text: "MovingCode Name", cls: "span4", validasi:"required" },
                        {
                            text: "Parameter",
                            type: "controls",
                            items: [
                                { name: "Param1", cls: "span1", placeHolder: "0", type: "int", min:-1, max:999},
                                { name: "Sign1", type: "select2", cls: "span2", datasource: "dataOption"},                                
                                { cls: "span1 label-valign", text: "&nbsp;  X  &nbsp;", type:"label" },
								{ name: "Sign2", type: "select2", cls: "span2", datasource: "dataOption"},
                                { name: "Param2", cls: "span1", placeHolder: "0", type: "int", min: -1, max:999},                                
                            ]
                        }                     
                ]
            },         
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("SpMovingController");
    }	

});