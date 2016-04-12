"use strict"

function omFakturPolisiController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.browse = function () {

        var lookup = Wx.blookup({
            name: "FakturPolisiBrowse",
            title: "Faktur Polisi",
            manager: spSalesManager,
            query: "FakturPolisiBrowse",
            defaultSort: "FakturPolisiNo asc",
            columns: [
                  { field: "FakturPolisiNo", title: "No. Faktur Polisi" },
                  {
                      field: "FakturPolisiDate", title: "Tgl. Faktur Polisi", sWidth: "130px",
                      template: "#= (FakturPolisiDate == undefined) ? '' : moment(FakturPolisiDate).format('DD MMM YYYY') #"
                  },
                  { field: "ChassisCode", title: "Kode Rangka" },
                  { field: "ChassisNo", title: "No. Rangka" },
                  { field: "ReqNo", title: "No.Request" },
                  { field: "Status", title: "Status" },
            ]
        });
        lookup.dblClick(function (data) {
            me.isApprove = data.Stat == "2" || data.Stat == "3";
            $('#lblStatus').html(data.Status);
            if (data.Stat == "0") {
                $('#btnApprove').removeAttr('disabled');
            }
            else {
                $('#btnApprove').attr('disabled', 'disabled');
            }

            $('#FakturPolisiNo').attr('disabled', 'disabled');

            me.lookupAfterSelect(data);
            me.isSave = false;
            me.Apply();
        });
    }

    me.chassisCode = function () {
        var lookup = Wx.blookup({
            name: "ChassisCodeBrowse",
            title: "Kode Rangka",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("ChassisBrowse").withParameters({ chassisCode: "", isChassisCode: 1 }),
            defaultSort: "ChassisCode asc",
            columns: [
               { field: 'ChassisCode', title: 'Kode Rangka' },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.ChassisCode = data.ChassisCode;
            me.isSave = false;
            me.Apply();
        });
    }

    me.chassisNo = function () {
        var lookup = Wx.blookup({
            name: "ChassisNoBrowse",
            title: "No. Rangka",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("ChassisBrowse").withParameters({ chassisCode: me.data.ChassisCode, isChassisCode: 0 }),
            defaultSort: "ChassisNo asc",
            columns: [
               { field: 'ChassisNo', title: 'No. Rangka' },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.ChassisNo = data.ChassisNo
            me.isSave = false;
            me.Apply();
        });
    }

    me.saveData = function () {
        if (me.data.FakturPolisiNo == null || me.data.ChassisCode == null || me.data.ChassisNo == null) {
            Wx.alert("Ada informasi yang belum lengkap");
        }
        else {
            $http.post('om.api/fakturpolisi/save', me.data)
            .success(function (e) {
                if (e.success) {
                    $('#btnApprove').removeAttr('disabled');
                    $('#lblStatus').html(e.status);
                    me.isApprove = false;
                    Wx.Success(e.message);
                } else {
                    MsgBox(e.message, MSG_ERROR);
                }
            })
            .error(function (e) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
            });
        }
    }

    me.delete = function () {
        if (confirm("Apakah Anda Yakin???", "Posting Data")) {
            $http.post('om.api/fakturpolisi/delete', me.data)
            .success(function (e) {
                if (e.success) {
                    me.init();
                }
                Wx.Success(e.message);
            })
            .error(function (e) {

            });
        }
    }

    me.approve = function () {
        if (confirm("Apakah Anda Yakin???", "Posting Data")) {
            $http.post('om.api/fakturpolisi/approve', me.data)
            .success(function (e) {
                if (e.success) {
                    $('#lblStatus').html(e.status);
                    $('#btnApprove').attr('disabled', 'disabled');
                    me.isApprove = true;
                }
                Wx.Success(e.message);
            })
            .error(function (e) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
            });
        }
    }



    me.initialize = function () {
        me.data.FakturPolisiDate = me.now();
        me.data.IsBlanko = true;
        me.isApprove = true;
        $('#lblStatus').html("NEW");
        $('#lblStatus').css(
         {
             "font-size": "28px",
             "color": "red",
             "font-weight": "bold",
             "text-align": "right"
         });

        $('#btnApprove , #ChassisCode, #ChassisNo').attr('disabled', 'disabled');

        $('#FakturPolisiNo').removeAttr('disabled');

    }
    me.isApprove = true;
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Faktur Polisi",
        xtype: "panels",
        toolbars: [{ name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "!isApprove", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" }
        ],
        panels: [
             {
                 name: "pnlStatus",
                 items: [
                     { name: "lblStatus", text: "", cls: "span4", readonly: true, type: "label" },
                      {
                          type: "buttons", cls: "span4", items: [
                                 { name: "btnApprove", text: "Approve", cls: "btn btn-info", icon: "icon-ok", click: "approve()", disable: true },
                          ]
                      }
                 ]
             },
            {
                name: "pnlFakturPolisi",
                items: [
                    { name: "FakturPolisiNo", text: "No. Faktur Polisi", cls: "span4", required: true, validasi: "required", maxlength: 15 },
                    { name: "FakturPolisiDate", text: "Tgl. Faktur Polisi", cls: "span4", type: "ng-datepicker" },
                    { name: "ChassisCode", text: "Kode Rangka", cls: "span4", type: "popup", click: "chassisCode()" },
                    { name: "ChassisNo", text: "No Rangka", cls: "span4", type: "popup", click: "chassisNo()" },
                    { name: "IsBlanko", text: "Blanko?", cls: "span2", type: "x-switch" },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("omFakturPolisiController");
    }
});