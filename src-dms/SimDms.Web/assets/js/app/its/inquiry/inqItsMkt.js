var report = "";
var isCEO = false;
var isSuzuki = false;
var empID = "";
var pType = "";
var cmpCode = "";
var OutletCode = "";
"use strict"

function inqits($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.getUserProperties = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'its.api/InquiryIts/itsUserProperties',
            success: function (dt) {
                isCEO = dt.data.isCOO;
                empID = dt.data.EmployeeID;
                pType = dt.data.ProductType;
                cmpCode = dt.data.cmpCode;
                OutletCode = dt.data.OutletCode;
            }
        });
    }

    me.setComboArea = function () {
        if (isSuzuki) {
            $.ajax({
                async: false,
                type: "POST",
                data: { id: "", jns: "A" },
                url: 'its.api/Combo/getInqItsCombo',
                success: function (dt) {
                    if (dt != "") {
                        me.comboArea = dt;
                    } else {
                        MsgBox("User belum link ke data karyawan!", MSG_INFO);
                        Wx.enable({ value: false, items: ["cmbArea", "cmbDealer", "cmbOutlet"] })
                        $('#btnQuery').attr('disabled', 'disabled');
                        $('#btnExcel').attr('disabled', 'disabled');
                    }
                }
            });
        } else {
            $.ajax({
                async: false,
                type: "POST",
                data: { id: cmpCode, jns: 'A' },
                url: 'its.api/Combo/getInqItsCombo',
                success: function (dt) {
                    if (dt != "") {
                        me.comboArea = dt;
                        setTimeout(function () {
                            $('[name=cmbArea]').select2().select2('val', $('[name=cmbArea] option:eq(1)').val());
                        }, 2000)
                        Wx.enable({ value: false, items: ["cmbArea"] })
                        me.setComboDealer(cmpCode);
                    } else {
                        MsgBox("User belum link ke data karyawan!", MSG_INFO);
                        Wx.enable({ value: false, items: ["cmbArea", "cmbDealer", "cmbOutlet"] })
                        $('#btnQuery').attr('disabled', 'disabled');
                        $('#btnExcel').attr('disabled', 'disabled');
                    }
                }
            });

        }
    }

    me.setComboDealer = function (dcode) {
        var data = {}
        if (isSuzuki) {
            data = {
                id: "",
                jns: "D"
            }
            $http.post('its.api/Combo/getInqItsCombo', data).
              success(function (dt, status, headers, config) {
                  if (dt != "") {
                      me.comboDealer = dt
                      me.setComboOutlet();
                  } else {
                      Wx.enable({ value: false, items: ["cmbArea", "cmbDealer", "cmbOutlet"] })
                      $('#btnQuery').attr('disabled', 'disabled');
                      $('#btnExcel').attr('disabled', 'disabled');
                  }
              });
        } else {
            data = {
                id: dcode,
                jns: "D"
            }
            $http.post('its.api/Combo/getInqItsCombo', data).
              success(function (dt, status, headers, config) {
                  if (dt != "") {
                      me.comboDealer = dt
                      setTimeout(function () {
                          $('[name=cmbDealer]').select2().select2('val', $('[name=cmbDealer] option:eq(1)').val());
                      }, 2000)
                      Wx.enable({ value: false, items: ["cmbDealer"] })
                      me.setComboOutlet(OutletCode);
                  } else {
                      Wx.enable({ value: false, items: ["cmbArea", "cmbDealer", "cmbOutlet"] })
                      $('#btnQuery').attr('disabled', 'disabled');
                      $('#btnExcel').attr('disabled', 'disabled');
                  }
              });
        }
    }

    me.setComboOutlet = function (otlCode) {
        var data = {}
        if (isSuzuki) {
            data = {
                id: "",
                jns: "O"
            }
            $http.post('its.api/Combo/getInqItsCombo', data).
              success(function (dt, status, headers, config) {
                  me.comboOutlet = dt
              });
        } else {
            if (isCEO) {
                data = {
                    id: "",
                    p1: cmpCode
                }
                $http.post('its.api/Combo/getInqItsComboOutlet', data).
                  success(function (dt, status, headers, config) {
                      if (dt != "") {
                          me.comboOutlet = dt
                      } else {
                          Wx.enable({ value: false, items: ["cmbArea", "cmbDealer", "cmbOutlet"] })
                          $('#btnQuery').attr('disabled', 'disabled');
                          $('#btnExcel').attr('disabled', 'disabled');
                      }
                  });
            } else {
                data = {
                    id: OutletCode,
                    p1: cmpCode
                }
                $http.post('its.api/Combo/getInqItsComboOutlet', data).
                  success(function (dt, status, headers, config) {
                      if (dt != "") {
                          me.comboOutlet = dt
                          setTimeout(function () {
                              $('[name=cmbOutlet]').select2().select2('val', $('[name=cmbOutlet] option:eq(1)').val());
                          }, 2000)
                          Wx.enable({ value: false, items: ["cmbOutlet"] })
                      } else {
                          Wx.enable({ value: false, items: ["cmbArea", "cmbDealer", "cmbOutlet"] })
                          $('#btnQuery').attr('disabled', 'disabled');
                          $('#btnExcel').attr('disabled', 'disabled');
                      }
                  });
            }
        }
    }

    me.grid1 = webix.ui({
        container: "InqITS",
        view: "wxtable", css:"alternating",
        scrollX: true,
        columns: [
                { id: "DealerAbbreviation", header: "Dealer Name", width: 120 },
                { id: "TipeKendaraan", header: "Tipe Kendaraan", width: 120 },
                { id: "Variant", header: "Variant", width: 120 },
                { id: "OutsINQ", header: "Outstanding Inquiry", width: 150 },
                { id: "NewINQ", header: "New INQ", width: 100 },
                { id: "OutsSPK", header: "Outs SPK", width: 90 },
                { id: "NewSPK", header: "New SPK", width: 80 },
                { id: "CancelSPK", header: "Cancel SPK", width: 80 },
                { id: "FakturPolisi", header: "Faktur Polisi", width: 90 },
                { id: "Balance", header: "Balance", width: 90 },
                { id: "ATTestDrive", header: "AT Test Drive", width: 90 },
                { id: "MTTestDrive", header: "MT Test Drive", width: 90 },
        ]
    });

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    });


    me.clkQuery = function () {
        var data = {
            begindate: moment(me.data.dtpFrom).format("YYYYMMDD"),
            enddate: moment(me.data.dtpTo).format("YYYYMMDD"),
            area: $('#cmbArea').select2('data').text,
            dealer: $('[name=cmbDealer]').val(),
            outlet: $('[name=cmbOutlet]').val()
        }
        $('.page > .ajax-loader').show();
        $http.post('its.api/InquiryIts/inqItsMktLoadQuery', data)
          .success(function (dt, status, headers, config) {
              $('.page > .ajax-loader').hide();
              if (dt != "") {
                  me.loadTableData(me.grid1, dt.dtbl);
                  me.isPrintAvailable = true;
              } else {
                  me.clearTable(me.grid1);
                  me.isPrintAvailable = true;
              }

              console.log(dt);
          })
          .error(function (e, status, headers, config) {
              $('.page > .ajax-loader').hide();
              MsgBox(e, MSG_ERROR);
          });
    }

    me.clkExcel = function () {        

        var spID = "uspfn_OmInquiryMKT_genExcel";
        var startDate = moment(me.data.dtpFrom).format('YYYY-MM-DD');
        var endDate = moment(me.data.dtpTo).format('YYYY-MM-DD');
        var area = $('#cmbArea').select2('data').text;
        var dealer = $('[name=cmbDealer]').val();
        var outlet = $('[name=cmbOutlet]').val();
        var dName = $('#cmbDealer').select2('data').text;
        var oName = $('#cmbOutlet').select2('data').text;

        var url = "its.api/Inquiry/inquiryItsMktgenexcell?";

        var params = "&StartDate=" + startDate;
        params += "&EndDate=" + endDate;
        params += "&BranchCode=" + me.data.OutletCode;
        params += "&Area=" + area;
        params += "&Dealer=" + dealer;
        params += "&Outlet=" + outlet;
        params += "&SpID=" + spID;
        params += "&DName=" + dName;
        params += "&OName=" + oName;
        url = url + params;
        window.location = url;
    }

    me.initialize = function () {
        me.data = {};
        me.comboArea = {};
        me.comboDealer = {};
        me.comboOutlet = {};
        me.isPrintAvailable = false;
        me.clearTable(me.grid1);
        //me.data.dtpFrom = me.data.dtpTo = new Date();
        var date = new Date(), y = date.getFullYear(), m = date.getMonth();
        me.data.dtpFrom = new Date(y, m, 1);
        me.data.dtpTo = new Date(y, m + 1, 0);
        me.getUserProperties();
        me.setComboArea();
    }
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Inquiry ITS (Management)",
        xtype: "panels",
        toolbars: [
                    //{ name: "btnPrint", cls: "btn btn-primary", text: "Print", icon: "icon-print", click: "printPreview()", show: "isPrintAvailable" }
        ],
        panels: [
            {
                items: [
                    {
                        text: "Periode",
                        cls: "span5",
                        type: "controls", items: [
                            { name: "dtpFrom", text: "Date From", cls: "span4", type: "ng-datepicker" },
                            { name: "dtpTo", text: "Date To", cls: "span4", type: "ng-datepicker" },
                        ]
                    },
                    {
                        text: "Area",
                        type: "controls",
                        items: [
                            { name: "cmbArea", cls: "span6", type: "select2", click: "AreaCode()", datasource: "comboArea", disable: "false" },
                        ]
                    },
                    {
                        text: "Dealer",
                        type: "controls",
                        items: [
                            { name: "cmbDealer", cls: "span6", type: "select2", click: "DealerCode()", datasource: "comboDealer", disable: "false" },
                        ]
                    },
                    {
                        text: "Branch/Outlet",
                        type: "controls",
                        items: [
                            { name: "cmbOutlet", cls: "span6", type: "select2", opt_text: "-- SELECT ALL --", click: "OutletCode()", datasource: "comboOutlet", disable: "false" },
                        ]
                    },
                    {
                        type: "buttons", cls: "span8 left", items: [
                        { name: "btnQuery", cls: "btn-small", text: "Query", icon: "icon-process", click: "clkQuery()", cls: "span2", style: "width:80px;" },
                        { name: "btnExcel", cls: "btn-small", text: "Excel", icon: "icon-process", click: "clkExcel()", cls: "span2", style: "width:80px;" }
                        ]
                    }
                ]
            },
            {
                title: "List Inquiry ITS (Management)",
                items: [
                    {
                        name: "InqITS",
                        title: "List Inquiry ITS (Management)",
                        type: "wxdiv"
                    },
                    {
                        name: "InqITS_Pager",
                        type: "wxdiv"
                    }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("inqits");
    }
});

