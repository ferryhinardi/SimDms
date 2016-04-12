
function oUtlClosingMonthController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.default = function () {
        $http.post("om.api/closingmonth/default")
        .success(function (e) {
            me.data = e;
            me.data.PeriodBeg = e.PeriodBeg;
            me.data.PeriodEnd = e.PeriodEnd;
        })
    }

    me.closing = function () {
        $http.post('om.api/closingmonth/validateclosing')
        .success(function (e) {
            if (e.success) {
                $http.post('om.api/closingmonth/closingmonth')
                .success(function (e) {
                    if (e.success) {
                        Wx.Success(e.message);
                    } else {
                        MsgBox(e.message, MSG_ERROR);
                    }
                })
                .error(function (e) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            } else {
                if (e.print) {
                    Wx.confirm(e.message, function (e) {
                        if(e =='Yes')
                        {
                            var ReportId = 'OmRpDocPending';
                            var par = [
                                me.PeriodBeg
                            ]
                            var rparam = 'Tanggal : ' + moment(me.data.PeriodBeg).format('DD-MM-YYY') + " s/d " + moment(me.data.PeriodEnd).format('DD-MM-YYYY');

                            Wx.showPdfReport({
                                id: ReportId,
                                pparam: par,
                                rparam: rparam,
                                type: "devex"
                            });
                        }
                    })
                }
                else {
                    MsgBox(e.message, MSG_ERROR);
                }
            }
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.cancelOrClose = function () {

    }

    me.initialize = function () {
        me.default();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Tutup Bulan",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "Clear", cls: "btn btn-primary", icon: "", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlClosingMonth",
                items: [
                   { name: "Year", text: "Tahun", cls: "span2", readonly: true },
                   {
                       type: "buttons", cls: "span4", items: [
                              { name: "btnClosing", text: "TUTUP BULAN", cls: "btn btn-info",  click: "closing()" }
                       ]
                   },
                   {
                       text: "Periode",
                       type: "controls",
                       items: [
                           { name: "Period", cls: "span2", placeHolder: " ", readonly: true },
                           { name: "PeriodDesc", cls: "span4", placeHolder: " ", readonly: true },
                       ]
                   },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("oUtlClosingMonthController");
    }
});