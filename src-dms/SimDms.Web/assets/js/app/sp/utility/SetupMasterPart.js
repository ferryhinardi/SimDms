"use strict"

var ucb = {};
function SetupMasterPart($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    });

    $http.post('its.api/Combo/Branch')
    .success(function (dt, status, headers, config) {
           me.comboBranch = dt;           
    });

    ucb=function(data){
        me.loadTableData(me.grid1, data);
    }


    me.supplierBrowse = function () {
        var lookup = Wx.blookup({
            name: "SupplierBrowse",
            title: "Pemasok",
            manager: spSalesManager,
            query: "SupplierCodeLookup",
            defaultSort: "SupplierCode asc",
            columns: [
                { field: "SupplierCode", title: "Kode Pemasok" },
                { field: "SupplierName", title: "Nama Pemasok" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SupplierCode = data.SupplierCode;
                me.data.SupplierName = data.SupplierName;
                me.Apply();
            }
        });
    };

    me.Process = function () {
        if (me.cbBranch == undefined)
        {
            MsgBox("Silahkan pilih cabang", MSG_ERROR);
            return;
        }

        if (me.cbtype == undefined) {
            MsgBox("Tipe tidak boleh kosong", MSG_ERROR);
            return;
        }

        if (me.data.SupplierCode == undefined && me.cbtype==1)
        {
            MsgBox("Supplier Tidak boleh kosong", MSG_ERROR);
            return;
        }

        var formData = new FormData();
        var progressInterval = null;
        var fileElement = $("form[name='formtxtFile'] input[name='file']");
        formData.append("file", fileElement[0].files[0]);
        formData.append("uploadType", me.cbtype);
        formData.append("uBranchCode", me.cbBranch);
        formData.append("POSDate", me.POSDate);
        formData.append("SupplierCode", me.data.SupplierCode);
        formData.append("Keterangan", me.Keterangan);


        $.ajax({
            url: "sp.api/uploadfile/UploadFileMasterPart",
            data: formData,
            type: "POST",
            cache: false,
            contentType: false,
            processData: false,            
            complete: function (result) {
            },
            success: function (result) {
                
                if (result.success == true)
                {
                    MsgBox(result.message, MSG_INFO);
                }
                else{
                    MsgBox(result.message, MSG_ERROR);
                }
                
            },
            error: function (a, b, c) {                
            }
        });

    }

    me.comboType = [{ text: "Direct To Item Master", value: 0 }, { text: "Create PO", value: 1 }];

    me.grid1 = new webix.ui({
        container: "wxuploadfile",
        view: "wxtable", css: "alternating",
        autowidth: true,        
        autoHeight: false,
        height: 1024,
        scrollY: true,
        columns: [
            { id: "Seqno", header: "SeqNo", width: 50 },
            { id: "PartNo", header: "No. Part", width: 150 },
            { id: "MovingCode", header: "MC", width: 50 },
            { id: "ABCClass", header: "ABC", width: 50 },
            { id: "LocationCode", header: "LOC", width: 50 },
            { id: "TypeOfGoods", header: "Type Of", width: 60 },
            { id: "BornDate", header: "Tgl. Lahir", width: 120 },
            { id: "Qty", header: "QTY", width: 50},
            { id: "CostPrice", header: "Harga Pokok", width: 120 },
            { id: "TotalCost", header: "Total Harga Pokok", width: 120 },
            { id: "Purchase", header: "Harga Beli", width: 120 },
            { id: "Retail", header: "Harga Jual", width: 120 },
            { id: "SupplierCode", header: "Kode Supplier", width: 120 },
            { id: "PartName", header: "Nama Part", width: 300 }
        ],
    });

    me.Browse = function () {

    }

    
    me.Query = function () {
        
    }

    me.initialize = function () {
        $("#pnlPO").hide();

        $http.post('sp.api/MasterItem/default')
          .success(function (v) {
              me.POSDate = v.BornDate;
          });

    }


    $("#cmbType").on('change', function () {
        //console.log($("#cmbType").val())
        
        if ($("#cmbType").val() == 1)
        {
            $("#pnlPO").show();
        }
        else
        {
            $("#pnlPO").hide();
        }
    });


    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Setup Master Part",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", cls: "btn btn-success", icon: "icon-refresh", click: "initialize()" }
        ],
        panels: [
             {
                 items: [
                    {
                        text: "Kode Cabang",
                        cls: "span4",
                        type: "controls", items: [
                            { name: "cmbBranch", type: "select2", model:"cbBranch", opt_text: "[SELECT ONE]", datasource: "comboBranch", required: true, validasi: "required" }
                        ]
                    },
                    {
                        text: "Tipe",
                        cls: "span4",
                        type: "controls", items: [
                            { name: "cmbType", type: "select2", model:"cbtype", opt_text: "[SELECT ONE]", datasource: "comboType", required: true, validasi: "required" }
                        ]
                    },
                             
                    {
                        name: "totalQtyC",
                        text: "Total Qty",
                        cls: "span5",
                        show: "!isComboShow",
                        type: "controls", items: [
                            { name: "totalQty", cls: "span6", placeholder: "Total Qty", readonly: true },

                        ]
                    },
                    {
                        type: "buttons",
                        cls: "span3",
                        items: [
                            { name: "btnQuery", cls: "btn btn-info", icon: "icon-search", text: "Query", click: "Query()", style: "width:120px;" },
                        ]
                    },                    
                    {
                        text: "Total Cost",
                        cls: "span5",
                        type: "controls", items: [
                            { name: "totalCost", cls: "span6", placeholder: "" , readonly: true},
                        ]
                    },
                    {
                        cls: "span3",                        
                        type: "controls", items: [
                            {
                                type: "buttons",
                                cls: "span1",
                                text: "Klik untuk",
                                items: [
                                    { name: "btnProcess", cls: "btn btn-info", icon: "icon-gear", text: " Process", click: "Process()", style: "width:120px;" }                                
                                ]
                            },                         
                        ]
                    },  
                    { name: "txtFile", text: "File (*.xlsx)", cls: "span4 full", readonly: true, type: "upload", url: "sp.api/uploadfile/UploadFileMasterPart", icon: "icon-upload", callback: "uploadCallback", onUpload: "onUpload" },
                    
                 ]
             },
              {
                  name: "pnlPO",
                  items: [
                      
                      { name: "POSDate", model:"POSDate", text: "Tgl. POS", cls: "span4 full", type: "ng-datepicker" },                                            
                      {
                          text: "Pemasok",
                          type: "controls",
                          required: true,
                          items: [
                              { name: "SupplierCode", cls: "span2", placeHolder: "Kode Pemasok", readonly: true, type: "popup", click: "supplierBrowse()" },                              
                              { name: "SupplierName", cls: "span6", placeHolder: "Nama Pemasok", readonly: true }
                          ]
                      },
                      { name: "Keterangan", text: "Keterangan", model: "Keterangan", cls: "span12", placeHolder: "Keterangan" },
                      
                  ]
              },
              {

                  items: [
                       {
                           name: "wxuploadfile",
                           type: "wxdiv",
                       }
                  ]
              }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);


    function init(s) {
        SimDms.Angular("SetupMasterPart");
        $('#btnProcess').attr('disabled', 'disabled');
    }
});


function uploadCallback(result, obj) {
  
        $("[name=txtFileShowed]").val(result.FileName);
        $("#totalQty").val(result.TotalQty);
        $("#totalCost").val(result.TotalCostPrice);
        ucb(result);
        $('#btnProcess').prop('disabled', false);
    
}


function onUpload(uploadProgress) {
    //Wx.Success("Uploading file : " + uploadProgress + " %");
    //console.log("this is onUpload");
}
