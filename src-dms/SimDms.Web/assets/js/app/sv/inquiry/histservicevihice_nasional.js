var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function HistoryServiceNasional($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {

        me.data.PoliceRegNo = "";
        me.data.NoVin = "";
        me.DataSource = "nasional";
        me.Apply();

    }

    me.Query = function () {
        if ((me.data.PoliceRegNo == undefined || me.data.PoliceRegNo == "") && (me.data.NoVin == undefined || me.data.NoVin == "")) {
            console.log("Gagal query");
            MsgBox('Minimal salah satu data harus diisi', MSG_INFO);

        }
        else {
            console.log(me.DataSource);
            var s = me.DataSource;
            refreshGrid(s);
        }
    }


    function refreshGrid(s) {
        
        console.log(s);
        var prms = {
            PoliceRegNo: me.data.PoliceRegNo,
            NoVin: me.data.NoVin
        }
        var urllink;
        console.log(s);
        if (s == "nasional") {
            urllink = "sv.api/InqVehicleHistory/GetVehicleHistoryNasional";
        }
        else if (s == "lokal") {
            urllink = "sv.api/InqVehicleHistory/GetVehicleHistoryLocal";
        }
        console.log(s);
        console.log(urllink);
        Wx.kgrid({
            url: urllink,
            name: "wxinfokendaraan",
            params: prms,
            pageSize: 10,
            multiselect: true,
            scrollable: true,
            group: [
                   { field: "tanggal", dir: "desc"},
            ],
            columns: [
                { field: "joborderdate", title: "Tanggal ", width: '125px', template: "#= (joborderdate == undefined) ? '' : moment(joborderdate).format('DD MMM YYYY') #" },
                { field: "policeregno", title: "No. Polisi", width: '125px' },
                { field: "vin", title: "No. VIN", width: '185px' },                
                { field: "Odometer", title: "Odometer", width: '125px' },
                { field: "JobsDesc", title: "Nama Pekerjaan", width: '300px' },
                { field: "PartName", title: "Nama Part (Jika ada pergantian)", width: '300px' },
                { field: "qty", title: "Jumlah", width: '125px' },
            ],
        });
    }


    me.start();
}



$(document).ready(function () {
    var options = {
        title: "History Vehicle National",
        xtype: "panels",
        panels: [
            {
                name: "pnlRefService",
                title: "Informasi Kerdaraan",
                items: [
                    { name: "PoliceRegNo", required: true, cls: "span4 full", text: "No Polisi" },
                    { name: "NoVin", required: true, cls: "span4 full", text: "No. Vin" },
                    {
                        type: "optionbuttons",
                        text: "Data Source",
                        model: "DataSource",
                        cls: "span4",
                        items: [
                            { name: "nasional", text: "Nasional" },
                            { name: "lokal", text: "Lokal" }
                        ]
                    },
                    {
                        type: "buttons",
                        items: [
                             { name: "Query", text: "Query", icon: "icon-Search", cls: "span4", click: "Query()" },
                        ]
                    },

                ]
            },
            {
                name: "wxinfokendaraan",
                xtype: "k-grid",
                cls: "expand"
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("HistoryServiceNasional");
    }




});