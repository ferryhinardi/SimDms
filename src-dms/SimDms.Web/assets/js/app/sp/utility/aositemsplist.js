"use strict";
function spAosItemSpList($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });
    var codeId = 'TPGO';
    $http.post('sp.api/Combo/LoadLookup?CodeID=' + codeId).
    success(function (data, status, headers, config) {
        me.cboTipePart = data;
    });

    me.generate = function () {
        if (me.data.tipePart == null || me.data.tipePart == undefined || me.data.tipePart == '' ) {
            MsgBox('Silahkan pilih salah satu tipe part dahulu..', MSG_INFO);
            return;
        }

        console.log(me.data.tipePart);
        //me.exportToXLS(me.data.tipePart);
        me.exportToExcel(me.data.tipePart);
    }

    me.exportToXLS = function (typeOfGoods) {
        me.data.typeOfGoods = typeOfGoods;

        $('.page > .ajax-loader').show();
        var today = new Date();
        var date = moment(today.toJSON()).format('YYYYMMD_Hms');
        console.log(date);
        $.fileDownload('DoReport/AosItemSpList.xlsx', {
            httpMethod: "POST",
            //preparingMessageHtml: "We are preparing your report, please wait...",
            //failMessageHtml: "There was a problem generating your report, please try again.",
            data: me.data
        }).done(function () {
            $('.page > .ajax-loader').hide();
        });
    }

    me.exportToExcel = function (typeOfGoods) {
        Wx.XlsxReport({
            url: 'sp.api/utility/ExportAosItemSpList',
            type: 'xlsx',
            params: {
                typeOfGoods: typeOfGoods
            }
        });
    }
    
    me.initialize = function () {
        me.data = {};
        me.Apply();
    }

    me.start();

}
$(document).ready(function () {
    var options = {
        title: "Generate AOS Item Sparepart List",
        xtype: "panels",
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: "tipePart", model: "data.tipePart", text: "Tipe Part", opt_text: "[SELECT ONE]", cls: "span4 full", type: "select2", datasource: "cboTipePart" },
                        {
                            type: "buttons", cls: "full", items: [
                                  {
                                      name: "btnGenerate", text: " Generate", cls: "btn btn-primary span8 full", icon: "icon icon-bolt", click: "generate()"
                                  }
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
        SimDms.Angular("spAosItemSpList");
    }



});