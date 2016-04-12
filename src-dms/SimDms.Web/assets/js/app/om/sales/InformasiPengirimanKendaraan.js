"use strict";

function DeliveryInformation($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.rowToJson = function (data) {        
        var returnObj = new Object();
        returnObj.BPKNo = data[0];
        returnObj.BPKDate = data[1];
        //returnObj.BPKDate = moment(data[1]).format('MM-DD-YYYY');
        returnObj.SONo = data[2];
        returnObj.CustomerCode = data[3];
        returnObj.DeliveryDate = data[7];
        console.log(data);
        //returnObj.
        return returnObj;
    }

    me.Browse = function () {
        if (me.data.isBPKDate == false) {
            me.data.FromBPKDate = '01/01/1900';
            me.data.ToBPKDate = '01/01/1900';
        }
        var params = me.data;

        if (params.FromBPKNo === undefined) {
            params.FromBPKNo = '';
        }

        if (params.ToBPKNo === undefined) {
            params.ToBPKNo = '';
        }

        if (params.FromBPKDate === undefined || params.FromBPKDate === '') {
            params.FromBPKDate = '01/01/1900';
        }
        else {
            params.FromBPKDate = moment(params.FromBPKDate).format('YYYY-MM-DD')
        }

        if (params.ToBPKDate === undefined || params.ToBPKDate === '') {
            params.ToBPKDate = '01/01/1900';
        } else {
            params.ToBPKDate = moment(params.ToBPKDate).format('YYYY-MM-DD')
        }

        if (params.CustomerCode === undefined) {
            params.CustomerCode = '';
        }

        console.log(params);

        $.ajax({
            type: "POST",
            url: 'om.api/Grid/LoadTableBPK',
            dataType: 'json',
            data: params
            , success: function (response) {
                if (response.success) {

                    var i, n = response.data.length;
                    var datadump = [];
                    //var date='';

                    for (i = 0; i < n; i++) {
                        
                        var returnObj = response.data[i];
                        //if (returnObj.DeliveryDate != null) {
                        //    date = moment(returnObj.DeliveryDate).format('DD-MM-YYYY');
                        //}
                        
                        
                        datadump.push([1 + i, 0, returnObj.BPKNo])
                        datadump.push([1 + i, 1, returnObj.BPKDate])
                        //datadump.push([1 + i, 1, moment(returnObj.BPKDate).format('MM-DD-YYYY')])
                        datadump.push([1 + i, 2, returnObj.SONo])
                        datadump.push([1 + i, 3, returnObj.CustomerCode])

                        datadump.push([1 + i, 4, returnObj.ChassisNo])
                        datadump.push([1 + i, 5, returnObj.EngineNo])
                        datadump.push([1 + i, 6, returnObj.CustomerName])


                        datadump.push([1 + i, 7, returnObj.DeliveryDate])
                        //datadump.push([1 + i, 4, returnObj.DeliveryDate == '' ? returnObj.DeliveryDate : moment(returnObj.DeliveryDate).format('DD-MM-YYYY')])
                        //datadump.push([1 + i, 4, date])

                 
                    }
                    console.log(datadump);
                    window.hotPriceList.alter('remove_row', 1, window.hotPriceList.countRows());
                    window.hotPriceList.setDataAtCell(datadump, null, null, 'loadData');
                    $('#btnSave').removeAttr('disabled');
                } else {
                    MsgBox("Tidak Ada Data Di Tampilkan", MSG_INFO);
                    me.initialize();
                    window.hotPriceList.alter('remove_row', 1, window.hotPriceList.countRows());
                }
            }
        });

    }
    me.Save = function () {

        var data = window.hotPriceList.getData().slice(0);
        console.log(data);
        data.splice(0, 1);

        if (data.length === 1) return;
        var listData = [];

        for (var i = 0; i < data.length - 1; i++) {
            var b = angular.copy(data[i]);

            if (b[7].length == 10) {
                b[7] = moment(b[7],"MM/DD/YYYY").format('YYYY-MM-DD')
            }
            var dx = me.rowToJson(b);
            listData.push(dx);
        }

        var postData = {
            Data: JSON.stringify(listData)
        }
        console.log(postData);
        
        $.ajax({
            type: "POST",
            url: 'om.api/Grid/Save',
            dataType: 'json',
            data: postData
            , success: function (response) {
                //console.log(response.success);
                if (response.success) {
                    Wx.Success("Data saved...");
                    window.hotPriceList.alter('remove_row', 1, window.hotPriceList.countRows());
                    $('#btnSave').attr('disabled', 'disabled');
                } else {
                    console.log(response.a);
                    console.log(response.b);
                    MsgBox(response.msg, MSG_INFO);
                    window.hotPriceList.alter('remove_row', 1, window.hotPriceList.countRows());
                }
            }
        });


    }

    me.FromBPKNo = function () {
        var lookup = Wx.blookup({
            name: "BPKNo",
            title: "BPK No",
            manager: spSalesManager,
            query: "BPKNoLookUp",
            defaultSort: "BPKNo asc",
            columns: [
                 { field: "BPKNo", title: "No.BPK", width: 150 },
                 {
                     field: "BPKDate", title: "Tgl.BPK", width: 150,
                     template: "#= (BPKDate == undefined) ? '' : moment(BPKDate).format('DD MMM YYYY') #"
                 },
            ]
        });

        lookup.dblClick(function (data) {
            me.data.FromBPKNo = data.BPKNo;
            me.Apply();
        });
    }

    me.ToBPKNo = function () {
        var lookup = Wx.blookup({
            name: "BPKNo",
            title: "BPK No",
            manager: spSalesManager,
            query: "BPKNoLookUp",
            defaultSort: "BPKNo asc",
            columns: [
                 { field: "BPKNo", title: "No.BPK", width: 150 },
                 {
                     field: "BPKDate", title: "Tgl.BPK", width: 150,
                     template: "#= (BPKDate == undefined) ? '' : moment(BPKDate).format('DD MMM YYYY') #"
                 },
            ]
        });

        lookup.dblClick(function (data) {
            me.data.ToBPKNo = data.BPKNo;
            me.Apply();
        });
    }

    $("[name='CustomerCode']").on('blur', function () {
        if (me.data.CustomerCode != null) {
            $http.post('om.api/Grid/CustomerCode', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.CustomerName = data.data;
                   }
                   else {
                       me.data.CustomerCode = "";
                       me.data.CustomerName = "";
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    //$("[name='CustomerName']").on('blur', function () {
    //    if (me.data.CustomerName != null) {
    //        $http.post('om.api/Grid/CustomerName', me.data).
    //           success(function (data, status, headers, config) {
    //               if (data.success) {
    //                   me.data.CustomerCode = data.data;
    //               }
    //               else {
    //                   me.data.CustomerCode = "";
    //                   me.data.CustomerName = "";
    //                   me.Apply();
    //               }
    //           }).
    //           error(function (data, status, headers, config) {
    //               alert('error');
    //           });
    //    }
    //});


    $("[name = 'isBPKNo']").on('change', function () {
        me.data.isBPKNo = $('#isBPKNo').prop('checked');
        me.data.FromBPKNo = "";
        me.data.ToBPKNo = "";
        me.Apply();
    });

    $("[name = 'isBPKDate']").on('change', function () {
        me.data.isBPKDate = $('#isBPKDate').prop('checked');
        me.data.FromBPKDate = new Date;
        me.data.ToBPKDate = new Date;
        me.Apply();
    });

    $("[name = 'isCustomer']").on('change', function () {
        me.data.isCustomer = $('#isCustomer').prop('checked');
        me.data.CustomerCode = "";
        me.data.CustomerName = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data = {};
        //me.data.FromBPKDate = new Date;
        //me.data.ToBPKDate = new Date;

        $('#isBPKNo').prop('checked', false);
        me.data.isBPKNo = false;

        $('#isBPKDate').prop('checked', false);
        me.data.isBPKDate = false;

        $('#isCustomer').prop('checked', false);
        me.data.isCustomer = false;

        $('#FromBPKDate').attr('disabled', 'disabled');
        $('#ToBPKDate').attr('disabled', 'disabled');
        $('#btnSave').attr('disabled', 'disabled');
    };

    me.start();
}

$(document).ready(function () {

    var options = {
        title: "Informasi Pengiriman Kendaraan",
        xtype: "panels",
        toolbars: [
            { name: "btnBrowse", text: "Search", cls: "btn btn-info", icon: "icon-search", click: "Browse()" },
            { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", click: "Save()" },
            //{ name: "btnCancel", text: "Close", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlPeriod",
                cls: "full",
                items: [
                    {
                        type: "controls",
                        text: "No BPK dari",
                        cls: "span6",
                        items: [
                            { name: 'isBPKNo', type: 'check', cls: "span1", float: 'left' },
                            { name: "FromBPKNo", id: "FromBPKNo", cls: "span3", btnName: "btnFromBPKNo", type: "popup", click: "FromBPKNo()", disable: "data.isBPKNo == false" },
                            { type: "label", text: "S/D", cls: "span1"},
                            { name: "ToBPKNo", id: "ToBPKNo", cls: "span3", btnName: "btnToBPKNo", type: "popup", click: "ToBPKNo()", disable: "data.isBPKNo == false" },
                        ]
                    },
                    {
                        type: "controls",
                        text: "Tanggal BPK dari",
                        cls: "span6",
                        items: [
                            { name: 'isBPKDate', type: 'check', cls: "span1", float: 'left' },
                            { name: "FromBPKDate", id: "FromBPKDate", cls: "span3", type: "ng-datepicker", disable: "data.isBPKDate == false" },
                            { type: "label", text: "S/D", cls: "span1" },
                            { name: "ToBPKDate", id: "ToBPKDate", cls: "span3", type: "ng-datepicker", disable: "data.isBPKDate == false" },
                        ]
                    },
                    {
                        type: "controls",
                        text: "Pelanggan",
                        cls: "span6",
                        items: [
                            { name: 'isCustomer', type: 'check', cls: "span1", float: 'left' },
                            { name: "CustomerCode", id: "CustomerCode", cls: "span2", disable: "data.isCustomer == false" },
                            { type: "label", text: "-", cls: "span1" },
                            { name: "CustomerName", id: "CustomerName", cls: "span4", disable: "data.isCustomer == false" },
                        ]
                    },
                ]
            },
            { name: 'wxgridcell', xtype: 'wxtable' },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("DeliveryInformation");
    }

    setTimeout(function () {
        var datenow = new Date;
        var grayRenderer = function (instance, td, row, col, prop, value, cellProperties) {
            Handsontable.renderers.TextRenderer.apply(this, arguments);
            td.style.backgroundColor = '#00CCFF';
        };
        var clkNew = '';
        var clkOld = '';
        var data = [
              ["No.BPK", "Tgl.BPK", "No.SO", "Pelanggan","ChassisNo","EngineNo","Customer Name", "Tgl Delevery"]
        ],
      container = document.getElementById('wxgridcell'),
      settings1 = {
          data: data,
          colWidths: [130, 100, 130, 130,100,100,200,250],
          columns: [
            {
                //data: "BPKNo",
                readOnly: true
            },
            {
                //data: "BPKDate"
                readOnly: true,
                type: 'date',
                dateFormat: 'MM/DD/YYYY',
            },
            {
                //data: "SONo"
                readOnly: true
            },
            {
                //data: "Customer"
                readOnly: true
            },

            {
                //data: "Chassisno",
                readOnly: true,
            },
            {
                //data: "Engine No",
                readOnly: true,
            },
            {
                //data: "Customer Name",
                readOnly: true,
            },

             {
                 //data: "DeliveryDate",
                 type: 'date',
                 dateFormat: 'MM/DD/YYYY',
                 //correctFormat: true,
                 //defaultDate: null
             },
          ],
          contextMenu: true,
          minSpareRows: 1,
          currentRowClassName: 'currentRow',
          currentColClassName: 'currentCol',
          autoWrapRow: true,
          manualColumnResize: true,
          persistentState: true,
          rowHeights: [24, 24, 24],
          cells: function (row, col, prop) {
              var cellProperties = {};

              if (row == 0) {
                  this.renderer = grayRenderer;
                  cellProperties.readOnly = true;
              }
              return cellProperties;
          },
    };

        window.hotPriceList = new Handsontable(container, settings1);
        hotPriceList.render();

        $('.htContainer').css('position', 'absolute');
        $('.htContainer').css('height', '500px');
        $('htContainer').css('width', '100%');

    }, 1000);

});