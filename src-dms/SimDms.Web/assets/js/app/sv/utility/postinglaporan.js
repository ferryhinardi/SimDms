"use strict"

function postinglaporan($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sv.api/PostingLaporan/GetMonthNamesCombo').
        success(function (data, status, headers, config) {
            me.comboMonth = data;
        });

    me.MonthChange = function () {
        me.data.Month = $('#Month').select2('val');
    }

    me.initialize = function () {
        me.data.Year = new Date().getFullYear().toString();
        me.data.IsReposting = false;
        me.data.Month = "";
    }

    me.Process = function () {
        var year = me.data.Year;
        if (year.trim() == "") {
            return;
        }
        var confirmation = "Apakah yakin akan dilakukan Posting ";
        confirmation += me.data.IsReposting ? "Ulang ?" : "?";
        MsgConfirm(confirmation, function (ok) {
            if (!ok) return;
            var msgReposting = me.data.IsReposting ? "REPOSTING" : "";

            //POSTING HISTORY
            var data = {
                year: me.data.Year,
                month: me.data.Month
            }
            $http.post('sv.api/PostingLaporan/PostingHistory', data).
                success(function (data, status, headers, config) {
                    if (data.message == "") {
                        MsgBox("Posting berhasil");
                        me.initialize();
                    } else {
                        MsgBox("Posting Gagal, " + data.message);
                    }
                }).error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });
        });      
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Posting Laporan",
        toolbars: [
            { name: "btnNew", text: "New", cls: "btn btn-success", icon: "icon-refresh", click: "initialize()" },
        ],
        items: [
            {
                text: "Periode",
                type: "controls",
                items: [
                    { name: "Month", type: "select2", cls: "span2", datasource: "comboMonth", change: "MonthChange()" },
                    { name: "Year", type: "text", cls: "span1 number-only", style: "text-align:right" }
                ]
                
            },
            { name: "IsReposting", text: "Reposting", cls: "span8", type: "ng-check" },
            {
                type: "buttons",
                items: [
                    { name: "btnProcess", text: "Proses", cls: "btn btn-info", click: "Process()" }
                ]
                
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("postinglaporan");
    }

    $('#Year').on("blur", function () {
        var year = $(this).val();
        if(year == "" || year.length > 4) $('#Year').val(new Date().getFullYear().toString());
    });
});