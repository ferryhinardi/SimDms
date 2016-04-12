var report = "";
var isCEO = false;
var isSuzuki = false;
var empID = "";
var pType = "";
var cmpCode = "";
var OutletCode = "";
"use strict"

function inqitsstatus($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Report = [
        { "value": '0', "text": 'By Dealer' },
        { "value": '1', "text": 'By Type' }
    ];

    me.Type = [
        { "value": '0', "text": 'Summary' },
        { "value": '1', "text": 'Detail' }
    ];

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

    me.groupModel = function () {
        var lookup = Wx.blookup({
            name: "SelectLookUpGroupModel",
            title: "Group Model",
            manager: MasterITS,
            query: new breeze.EntityQuery.from("SelectLookUpGroupModel"),
            columns: [
                { field: "GroupModel", title: "GroupModel" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.groupModel = data.GroupModel;
                me.Apply();
            }
        });
    }

    me.tipeKendaraan = function () {
        var gModel = me.data.groupModel;
        var variant = '';

        if (gModel == "<----Select All---->" || gModel == undefined) {
            gModel = '';
        }
        var lookup = Wx.blookup({
            name: "SelectLookUpTipeKendaraan",
            title: "Tipe Kendaraan",
            manager: MasterITS,
            query: new breeze.EntityQuery().from("SelectLookUpTipeKendaraan").withParameters({ tipeKendaraan: gModel, variant: variant }),
            defaultSort: "TipeKendaraan asc",
            columns: [
                { field: "TipeKendaraan", title: "Tipe Kendaraan" },
                { field: "Variant", title: "Variant" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.tipeKendaraan = data.TipeKendaraan;
                me.Apply();
            }
        });
    }

    me.variant = function () {
        var gModel = me.data.groupModel;
        var tipe = me.data.tipeKendaraan;
        var variant = '';

        if (tipe == "<----Select All---->" || tipe == undefined) {
            tipe = '';
        }
        var lookup = Wx.blookup({
            name: "SelectLookUpTipeKendaraan",
            title: "Tipe Kendaraan",
            manager: MasterITS,
            query: new breeze.EntityQuery().from("SelectLookUpTipeKendaraan").withParameters({ tipeKendaraan: gModel, variant: variant }),
            columns: [
                { field: "TipeKendaraan", title: "Tipe Kendaraan" },
                { field: "Variant", title: "Variant" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.variant = data.Variant;
                me.Apply();
            }
        });
    }

    me.grid1 = webix.ui({
        container: "InqITS",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        height: 450,
        autoHeight: false,
        columns: [
                { id: "CompanyName", header: "Dealer Name", width: 120 },
                { id: "BranchName", header: "Outlet Name", width: 120 },
                { id: "TipeKendaraan", header: "Tipe Kendaraan", width: 120 },
                { id: "Variant", header: "Variant", width: 150 },
                { id: "LastProgress", header: "Last Progress", width: 120 },
                { id: "SaldoAwal", header: "Saldo Awal", width: 120 },
                { id: "WeekOuts1", header: "Outs 1", width: 75 },
                { id: "WeekOuts2", header: "Outs 2", width: 75 },
                { id: "WeekOuts3", header: "Outs 3", width: 75 },
                { id: "WeekOuts4", header: "Outs 4", width: 75 },
                { id: "WeekOuts5", header: "Outs 5", width: 75 },
                //{ id: "WeekOuts6", header: "Outs 6", width: 75 },
                { id: "TotalWeekOuts", header: "Total Outs", width: 120 },
                { id: "Week1", header: "Week 1", width: 75 },
                { id: "Week2", header: "Week 2", width: 75 },
                { id: "Week3", header: "Week 3", width: 75 },
                { id: "Week4", header: "Week 4", width: 75 },
                { id: "Week5", header: "Week 5", width: 75 },
                //{ id: "Week6", header: "Week 6", width: 75 },
                { id: "TotalWeek", header: "Total Week", width: 120 },
                { id: "Total", header: "Total", width: 90 },
        ]
    });

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    });


    me.clkQuery = function () {
        var gModel = me.data.groupModel;
        var tKndn = me.data.tipeKendaraan;
        var vrnt = me.data.variant;
        var blnPeriodeAwl = moment(me.data.dtpFrom).format("MM");
        var blnPeriodeAkh = moment(me.data.dtpTo).format("MM");

        if (gModel == "<----Select All---->" || gModel == undefined) {
            gModel = "";
        }
        if (tKndn == "<----Select All---->" || tKndn == undefined) {
            tKndn = "";
        }
        if (vrnt == "<----Select All---->" || vrnt == undefined) {
            vrnt = "";
        }

        if (me.data.dtpFrom > me.data.dtpTo) {
            MsgBox("Periode awal tidak boleh lebih besar dari periode akhir", MSG_INFO);
            return;
        }

        if (blnPeriodeAwl != blnPeriodeAkh) {
            MsgBox("Bulan periode harus sama", MSG_INFO);
            return;
        }

        var data = {
            begindate: moment(me.data.dtpFrom).format("YYYYMMDD"),
            enddate: moment(me.data.dtpTo).format("YYYYMMDD"),
            area: $('#cmbArea').select2('data').text,
            dealer: $('[name=cmbDealer]').val(),
            outlet: $('[name=cmbOutlet]').val(),
            groupModel: gModel,
            tipeKendaraan: tKndn,
            variant: vrnt,
            summary: 0
        }
        $('.page > .ajax-loader').show();
        $http.post('its.api/InquiryIts/inqItsStatusLoadQuery', data)
          .success(function (dt, status, headers, config) {
              $('.page > .ajax-loader').hide();
              if (dt != "") {
                  //me.loadTableData(me.grid1, dt);
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
        var url = "";
        var spID = "";
        var startDate = moment(me.data.dtpFrom).format('YYYY-MM-DD');
        var endDate = moment(me.data.dtpTo).format('YYYY-MM-DD');
        var area = $('#cmbArea').select2('data').text;
        var dealer = $('[name=cmbDealer]').val();
        var outlet = $('[name=cmbOutlet]').val();
        var dName = $('#cmbDealer').select2('data').text;
        var oName = $('#cmbOutlet').select2('data').text;

        var gModel = me.data.groupModel;
        var tKndn = me.data.tipeKendaraan;
        var vrnt = me.data.variant;

        var rpt = me.data.Report;
        var typ = me.data.Type;

        var blnPeriodeAwl = moment(me.data.dtpFrom).format("MM");
        var blnPeriodeAkh = moment(me.data.dtpTo).format("MM");

        if (gModel == "<----Select All---->" || gModel == undefined) {
            gModel = "";
        }
        if (tKndn == "<----Select All---->" || tKndn == undefined) {
            tKndn = "";
        }
        if (vrnt == "<----Select All---->" || vrnt == undefined) {
            vrnt = "";
        }

        if (me.data.dtpFrom > me.data.dtpTo) {
            MsgBox("Periode awal tidak boleh lebih besar dari periode akhir", MSG_INFO);
            return;
        }

        if (blnPeriodeAwl != blnPeriodeAkh) {
            MsgBox("Bulan periode harus sama", MSG_INFO);
            return;
        }


        if (rpt == "" || rpt == undefined) {
            MsgBox("Pilih dulu jenis reportnya", MSG_INFO);
            return;
        }
        if (typ == "" || typ == undefined) {
            MsgBox("Pilih dulu jenis typenya", MSG_INFO);
            return;
        }

        if (rpt == 0) {
            if (typ == 0) {
                url = "its.api/Inquiry/inquiryItsStatusDSgenexcell?";
                spID = "uspfn_InquiryITSWithStatusByDealer";

                var params = "&StartDate=" + startDate;
                params += "&EndDate=" + endDate;
                params += "&Area=" + area;
                params += "&Dealer=" + dealer;
                params += "&Outlet=" + outlet;
                params += "&SpID=" + spID;
                params += "&DName=" + dName;
                params += "&OName=" + oName;
                params += "&GroupModel=" + gModel;
                params += "&TipeKendaraan=" + tKndn;
                params += "&Variant=" + vrnt;
                params += "&Type=" + 1;

                url = url + params;
                window.location = url;
            }
            else {
                url = "its.api/Inquiry/inquiryItsStatusDDgenexcell?";
                spID = "uspfn_InquiryITSWithStatusByDealer";

                var params = "&StartDate=" + startDate;
                params += "&EndDate=" + endDate;
                params += "&Area=" + area;
                params += "&Dealer=" + dealer;
                params += "&Outlet=" + outlet;
                params += "&SpID=" + spID;
                params += "&DName=" + dName;
                params += "&OName=" + oName;
                params += "&GroupModel=" + gModel;
                params += "&TipeKendaraan=" + tKndn;
                params += "&Variant=" + vrnt;
                params += "&Type=" + 0;

                url = url + params;
                window.location = url;

            }
        }
        else {
            if (typ == 0) {
                url = "its.api/Inquiry/inquiryItsStatusTSgenexcell?";
                spID = "uspfn_InquiryITSWithStatusByType_Rev";

                var params = "&StartDate=" + startDate;
                params += "&EndDate=" + endDate;
                params += "&Area=" + area;
                params += "&Dealer=" + dealer;
                params += "&Outlet=" + outlet;
                params += "&SpID=" + spID;
                params += "&DName=" + dName;
                params += "&OName=" + oName;
                params += "&GroupModel=" + gModel;
                params += "&TipeKendaraan=" + tKndn;
                params += "&Variant=" + vrnt;
                params += "&Type=" + 1;

                url = url + params;
                window.location = url;

            }
            else {

                url = "its.api/Inquiry/inquiryItsStatusTDgenexcell?";
                spID = "uspfn_InquiryITSWithStatusByType_Rev";

                var params = "&StartDate=" + startDate;
                params += "&EndDate=" + endDate;
                params += "&Area=" + area;
                params += "&Dealer=" + dealer;
                params += "&Outlet=" + outlet;
                params += "&SpID=" + spID;
                params += "&DName=" + dName;
                params += "&OName=" + oName;
                params += "&GroupModel=" + gModel;
                params += "&TipeKendaraan=" + tKndn;
                params += "&Variant=" + vrnt;
                params += "&Type=" + 0;

                url = url + params;
                window.location = url;
            }
        }

    }

    me.initialize = function () {
        me.data = {};
        me.comboArea = {};
        me.comboDealer = {};
        me.comboOutlet = {};
        me.data.Report = '0';
        me.data.Type = '0';
        me.isPrintAvailable = false;
        me.clearTable(me.grid1);
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
        title: "Inquiry ITS With Status",
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
                        text: "Group Model",
                        type: "controls",
                        items: [
                            { name: "groupModel", cls: "span2 full", text: "<----Select All---->", type: "popup", btnName: "btnGroupModel", click: "groupModel()", readonly: true },
                        ]
                    },
                    {
                        text: "Tipe Kendaraan",
                        type: "controls",
                        items: [
                            { name: "tipeKendaraan", cls: "span2 full", text: "<----Select All---->", type: "popup", btnName: "btnTipeKendaraan", click: "tipeKendaraan()", readonly: true },
                        ]
                    },
                    {
                        text: "Variant",
                        type: "controls",
                        items: [
                            { name: "variant", cls: "span2 full", text: "<----Select All---->", type: "popup", btnName: "btnVariant", click: "variant()", readonly: true },
                        ]
                    },
                    { name: "Report", opt_text: "", cls: "span3", type: "select2", text: "Report", datasource: "Report" },
                    { name: "Type", opt_text: "", cls: "span3", type: "select2", text: "Type", datasource: "Type" },

                    {
                        type: "buttons", cls: "span8 left", items: [
                        { name: "btnQuery", cls: "btn-small", text: "Query", icon: "icon-process", click: "clkQuery()", cls: "span2", style: "width:80px;" },
                        { name: "btnExcel", cls: "btn-small", text: "Excel", icon: "icon-process", click: "clkExcel()", cls: "span2", style: "width:80px;" }
                        ]
                    }
                ]
            },
            {
                title: "List Inquiry ITS With Status",
                items: [
                    {
                        name: "InqITS",
                        title: "List Inquiry ITS With Status",
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
        SimDms.Angular("inqitsstatus");
    }
});

