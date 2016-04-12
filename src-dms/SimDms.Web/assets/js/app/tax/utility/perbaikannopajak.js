"use strict";
var Stat = 0;
function PerbaikanNoPajak($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        $http.post('om.api/ReportSales/Periode').
          success(function (e) {
              me.data.DateFrom = "2014/07/01";//e.DateFrom;
              me.data.DateTo = e.DateTo;
          });
        me.grid1.adjust();
    }

    me.FPJGovNo = function () {
        var lookup = Wx.blookup({
            name: "GenerateTax4Lookup",
            title: "FJ No",
            manager: TaxManager,
            query: new breeze.EntityQuery().from("GenerateTax4Lookup").withParameters({ DateFrom: me.data.DateFrom, DateTo: me.data.DateTo }),
            defaultSort: "FPJGovNo asc",
            columns: [
                { field: "BranchCode", title:  "Branch Code"}, 
                { field: "FPJGovNo", title:  "FPJ Gov No"},
                { field: "FPJGovDate", title: "FPJ Gov Date", template: "#= moment(FPJGovDate).format('DD MMM YYYY') #" },
                { field: "DocNo", title:  "Doc No"},
                { field: "DocDate", title: "Doc Date", template: "#= moment(DocDate).format('DD MMM YYYY') #" },
                { field: "RefNo", title:  "Ref No"},
                { field: "RefDate", title: "Ref Date" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.FPJGovNo = data.FPJGovNo;
                me.data.FPJGovNoNew = data.FPJGovNo;
                me.data.FPJGovDate = data.FPJGovDate;
                me.data.DocNo = data.DocNo;
                me.data.DocDate = data.DocDate;
                me.data.ReffNo = data.RefNo;
                me.data.ReffDate = data.RefDate;
                me.Apply();
            }
        });
    };

    me.Inquiry = function () {
        var DateFrom = moment(me.data.DateFrom).format('MM DD YYYY');
        var DateTo = moment(me.data.DateTo).format('MM DD YYYY');

        $http.post('tax.api/utility/GetListTax?DateFrom=' + DateFrom + '&DateTo=' + DateTo).
          success(function (data, status, headers, config) {
              me.loadTableData(me.grid1, data);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.save = function () {
        //MsgBox('Under Contruction', MSG_ERROR);
        $http.post('tax.api/utility/save2?FPJGovNo=' + me.data.FPJGovNo + '&FPJGovNoNew=' + me.data.FPJGovNoNew + '&DocNo=' + me.data.DocNo).
          success(function (data, status, headers, config) {
              if (data.success) {
                  me.data.FPJGovNo = me.data.FPJGovNoNew;
              } else {
                  MsgBox(data.message, MSG_ERROR);
              }
          }).
            error(function (e, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                console.log(e);
            });
    }

    me.grid1 = new webix.ui({
        container: "wxtableDtl",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "FpjSeqNo", header: "Seq No", fillspace: true },
            { id: "FPJGovNo", header: "FPJ Gov No", fillspace: true },
            { id: "FPJGovDate", header: "FPJ Gov Date", fillspace: true, format: me.dateFormat },
        ],on: {
        onSelectChange: function () {
            if (me.grid1.getSelectedId() !== undefined) {
                var data = this.getItem(me.grid1.getSelectedId().id);
                me.data.FPJGovNo = data.FPJGovNo;
                me.data.FPJGovNoNew = data.FPJGovNo;
                me.data.FPJGovDate = data.FPJGovDate;
                me.data.DocNo = data.DocNo;
                me.data.DocDate = data.DocDate;
                me.data.ReffNo = data.RefNo;
                me.data.ReffDate = data.RefDate;
                me.Apply();
            }
        }
    }
    });

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Perbaikan Faktur Pajak",
        xtype: "panels",
        toolbars: [
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },

            ],
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        text: "Select Date",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "DateFrom", text: "", cls: "span2", type: "ng-datepicker" },
                            { type: "label", text: "s.d", cls: "span1 mylabel" },
                            { name: "DateTo", text: "", cls: "span2", type: "ng-datepicker" },
                            {
                                type: "buttons", cls: "span2", items: [
                                     { name: "Inquiry", text: "Inquiry", icon: "", click: "Inquiry()", cls: "button small btn btn-success" },
                                ]
                            },
                        ]
                    },
                    { type: "hr" },
                    { name: "FPJGovNo", model: "data.FPJGovNo",text: "FPJ Gov No", cls: "span6", type: "popup", click: "FPJGovNo()", readonly: true },
                    { name: "FPJGovNoNew", text: "FPJ Gov No (New)", cls: "span6", required: true },
                    { name: "FPJGovDate", text: "FPJ Gov Date", cls: "span3", type: "ng-datepicker", readonly: true },
                    {
                        text: "Doc No / Doc Date",
                        type: "controls",
                        cls: "span6",
                        items: [
                            { name: "DocNo", cls: "span3", readonly: true },
                            { name: "DocDate", cls: "span5", type: "ng-datepicker", readonly: true },
                        ]
                    },
                    {
                        text: "Reff No / Reff Date",
                        type: "controls",
                        cls: "span6",
                        items: [
                            { name: "ReffNo", cls: "span3", readonly: true },
                            { name: "ReffDate", cls: "span5", type: "ng-datepicker", readonly: true },
                        ]
                    },
                    {
                        name: "wxtableDtl",
                        type: "wxdiv",
                    }
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("PerbaikanNoPajak");
    }



});