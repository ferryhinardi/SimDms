"use strict";


function DailyPosting($scope, $http, $injector, $timeout, blockUI) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    
    me.save = function () {

        var params = {
            PostingDate: moment(me.data.PostingDate).format('YYYY-MM-DD')
        }

        blockUI.start("Proccessing...");

        $http.post('gn.api/Posting/DailyPostingMultiCompany', params).
          success(function (response, status, headers, config) {
                blockUI.stop();
                if (response.success=="1") {
                    MsgBox(response.info, MSG_INFO);
                } else {
                    MsgBox(response.info, MSG_WARNING);
                }
          }).
          error(function (response, status, headers, config) {
              blockUI.stop();
              MsgBox(response, MSG_WARNING);
          });

    }

    me.initialize = function () {
        me.data = {};
        me.data.PostingDate = moment().format('DD MMM YYYY');
        console.log(me.data)
    };

    me.start();

}



$(document).ready(function () {
    var options = {
        title: "Daily Posting",
        xtype: "panels",

        toolbars: [
            { name: "btnPosting", text: "Posting", cls: "btn btn-warning", icon: "icon-bolt", click: "save()" },
        ],

        panels: [
        {
            name: "pnlA",
            items: [{
                        text: "Posting Date",
                        type: "controls",
                        cls: "span8",
                        items: [
                            {
                                name: "PostingDate",
                                cls: "span2",
                                type:'ng-datepicker',
                                text: "Posting Date"
                            }
                        ]
                    }
            ]// end of panel  
        }, {
            name: 'wxgridcell',
            xtype: 'wxtable'
        }

        ] // end of panel
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("DailyPosting");
    }

    setTimeout(function () {

        var strTemplate =
'<div class="alert alert-block alert-info">' +
'<div  style="margin:10px 10px 0 10px;padding:10px 10px 0 10px;"><h1 class="alert-heading"><i class="fa fa-warning"></i><font color="red"> PERHATIAN !!!</font></h1></div>' +
'    <div style="margin:10px 10px;padding:10px 10px;font-weight:bold;line-height:150%;font-size:20px;">' +
        'SETIAP HARI WAJIB MELAKUKAN DAILY POSTING . . .</BR>' +
        'DAILY POSTING DILAKUKAN SETELAH BERAKHIRNYA SELURUH TRANSAKSI HARIAN (SALES, SPAREPART & SERVICE).</BR></BR>' +
        'TERIMA KASIH.</BR>' +
'    </div>'+
'</div>'

        $('#wxgridcell').html(strTemplate);


        var datepicker = $("#PostingDate").data("kendoDatePicker");

        datepicker.setOptions({
            min: new Date(moment().add(-30,'days')),
            max: new Date()
    });


    },1000)

});