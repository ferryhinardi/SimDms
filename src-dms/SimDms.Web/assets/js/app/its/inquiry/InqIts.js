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
                data: { id:"", jns: "A" },
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
        }else{
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
                        //setTimeout(function () {
                        //    dlrCode = me.getDealerCode($("[name=cmbArea]").val())
                        //}, 2000)
                        Wx.enable({ value: false, items: ["cmbArea"] })
                        //console.log(dlrCode);
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
        scrollY: true,
        height: 450,
        autoHeight: false,
        columns: [
                { id: "DealerAbbreviation", header: "Dealer Name", width: 120 },
                { id: "OutletAbbreviation", header: "Outlet Name", width: 280 },
                { id: "TipeKendaraan", header: "Tipe Kendaraan", width: 120 },
                { id: "Variant", header: "Variant", width: 120 },
                { id: "NewINQ", header: "New INQ", width: 100, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "HPNewINQ", header: "HP from New Inq", width: 80, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "PrcntHPNewINQ", header: "%", width: 50, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "SpkfrNI", header: "SpkfrNI", width: 80, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "PrcntNewINQ", header: "SpkfrNI", width: 80, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "OutsINQ", header: "Outstanding Inquiry", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "HPOutsINQ", header: "HP from Outstanding INQ", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "PrcntHPOutsINQ", header: "%", width: 50, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "OutSPKfrNI", header: "OutSPKfrNI", width: 90, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "PrcntOutsINQ", header: "%", width: 50, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "TotalINQ", header: "TotalINQ", width: 80, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "TotalHP", header: "TotalHP", width: 80, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "PrcntTotalHP", header: "%", width: 50, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "TotalSPK", header: "TotalSPK", width: 80, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "PrcntTotalSPK", header: "%", width: 50, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "Lost", header: "Lost", width: 80, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "Cancel", header: "Cancel", width: 80, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "FakturPolisi", header: "FakturPolisi", width: 90, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "SOH", header: "SOH", width: 80, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
        ]
    });

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    });

    me.printPreview = function () {
        MsgBox("Fungsi print belum jalan ya!", MSG_INFO)
        //var firstPeriod = moment(me.data.dtpFrom).format("YYYY-MM-DD");
        //var endPeriod = moment(me.data.dtpTo).format("YYYY-MM-DD");
        //var period = moment(me.data.dtpFrom).format("DD-MM-YYYY") + " S/D " + moment(me.data.dtpFrom).format("DD-MM-YYYY");
        //var bm = $("[name=cmbBM]").val();
        //var sh = $("[name=cmbSH]").val();
        //var sm = $("[name=cmbSM]").val();
        //var par = firstPeriod + "," + endPeriod + "," + bm + "," + sh + "," + sm + "," + iTab;
        //var rparam = period + "," + bm + "," + sh + "," + "" + sm;

        //Wx.showPdfReport({
        //    id: report,
        //    pparam: par,
        //    rparam: rparam,
        //    type: "devex"
        //});
    }

    //me.getInqItsArea = function (grpCode) {
    //    $.ajax({
    //        async: false,
    //        type: "POST",
    //        data: { grpCode: grpCode },
    //        url: 'its.api/InquiryIts/inqGetArea',
    //        success: function (dt) {
    //            me.data.Area = dt.Area;
    //        }
    //    });
    //}

    me.clkQuery = function () {
        var data = {
            begindate: moment(me.data.dtpFrom).format("YYYYMMDD"),
            enddate: moment(me.data.dtpTo).format("YYYYMMDD"),
            area: $('#cmbArea').select2('data').text,
            dealer: $('[name=cmbDealer]').val(),
            outlet: $('[name=cmbOutlet]').val()
        }
        $('.page > .ajax-loader').show();
        $http.post('its.api/InquiryIts/inqItsLoadQuery', data)
          .success(function (dt, status, headers, config) {
              console.log(dt);
              $('.page > .ajax-loader').hide();
              if (dt != "") {
                  me.loadTableData(me.grid1, dt.dtbl);
                  me.isPrintAvailable = true;
              } else {
                  me.clearTable(me.grid1);
                  me.isPrintAvailable = true;
              }
          })
          .error(function (e, status, headers, config) {
              $('.page > .ajax-loader').hide();
              MsgBox(e, MSG_ERROR);
          });
    }

    me.clkExcel = function () {
        //me.data.dtpFrom = moment(me.data.dtpFrom).format('YYYY-MM-DD')
        //me.data.dtpTo = moment(me.data.dtpTo).format('YYYY-MM-DD')
        //me.data.Area = $('#cmbArea').select2('data').text
        //me.data.Dealer = $('[name=cmbDealer]').val()
        //me.data.DealerName = $('#cmbDealer').select2('data').text
        //me.data.Outlet = $('[name=cmbOutlet]').val()
        //me.data.OutletName = $('#cmbOutlet').select2('data').text
     
        //$('.page > .ajax-loader').show();

        //$.fileDownload('DoReport/InquiryITS.xlsx', {
        //    httpMethod: "POST",
        //    data: me.data
        //}).done(function () {
        //    $('.page > .ajax-loader').hide();
        //});

        //start update by fhi 24032014 : update format excell

        var url = "its.api/Inquiry/inquiryIts?";
        var spID = "uspfn_InquiryITS";
        var startDate = moment(me.data.dtpFrom).format('YYYY-MM-DD');
        var endDate = moment(me.data.dtpTo).format('YYYY-MM-DD');
        var area = $('#cmbArea').select2('data').text;
        var dealer = $('[name=cmbDealer]').val();
        var outlet = $('[name=cmbOutlet]').val();
        var dName = $('#cmbDealer').select2('data').text;
        var oName = $('#cmbOutlet').select2('data').text;

        if (me.data.dtpFrom > me.data.dtpTo) {
            MsgBox("Periode awal tidak boleh lebih besar dari periode akhir", MSG_INFO);
            return;
        }

        if (outlet == "<----Select All---->" || outlet == undefined) {
            outlet = "";
        }


        var params = "&StartDate=" + startDate;
        params += "&EndDate=" + endDate;
        params += "&Area=" + area;
        params += "&Dealer=" + dealer;
        params += "&Outlet=" + outlet;
        params += "&SpID=" + spID;
        params += "&DName=" + dName;
        params += "&OName=" + oName;

        url = url + params;
        window.location = url;

        //end

    }

    //me.testOpenLookup = function () {
    //    var lookup = Wx.blookup({
    //        name: "testLookup",
    //        title: "Test Open Lookup",
    //        manager: testHrEmployee, //Managernya ada di asset --> js -->dbcontext
    //        query: "TestEmplLookup", //Querry ke web --> Controller --> Api
    //        defaultSort: "EmployeeID asc",
    //        columns: [
    //            { field: "EmployeeID", title: "ID Karyawan" },
    //            { field: "EmployeeName", title: "Nama Karyawan" },
    //        ],
    //    });
    //    lookup.dblClick(function (data) {
    //        if (data != null) {
    //            me.data.testLookup = data.EmployeeID;
    //            me.data.fillLookup = data.EmployeeName;
    //            me.Apply();
    //        }
    //    });

    //}


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
        title: "Inquiry - Inquiry ITS",
        xtype: "panels",
        toolbars: [
                    //{ name: "btnPrint", cls: "btn btn-primary", text: "Print", icon: "icon-print", click: "printPreview()", show: "isPrintAvailable" }
        ],
        panels: [
            {
                items: [
                    {
                        text: "Date (From - To)",
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
                    //{
                    //    text: "Test BLookup",
                    //    type: "controls",
                    //    items: [
                    //        { name: "testLookup", cls: "span2", type: "popup", btnNane: "btnTestLookup", click: "testOpenLookup()" },
                    //        { name: "fillLookup", cls: "span6", readonly: true }
                    //    ]
                    //},
                    {
                        type: "buttons", cls: "span8 left", items: [
                        { name: "btnQuery", cls: "btn-small", text: "Query", icon: "icon-process", click: "clkQuery()", cls: "span2", style: "width:80px;" },
                        { name: "btnExcel", cls: "btn-small", text: "Excel", icon: "icon-process", click: "clkExcel()", cls: "span2", style: "width:80px;" }
                        ]
                    }
                ]
            },
            {
                title: "List Inquiry ITS",
                items: [
                    {
                        name: "InqITS",
                        title: "List Inquiry ITS",
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

