var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function MasterPerlengkapanController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.browse = function () {
        var lookup = Wx.blookup({
            name: "PerlengkapanBrowse",
            title: "Perlengkapan",
            manager: spSalesManager,
            query: "PerlengkapanBrowse",
            defaultSort: "PerlengkapanCode asc",
            columns: [
                { field: "PerlengkapanCode", title: "Kode Perlengkapan" },
                { field: "PerlengkapanName", title: "Nama Perlengkapan" },
                { field: "Remark", title: "Keterangan" },
                { field: "Status2", title: "Status Aktif" },
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.isSave = false;
                me.data.Status = result.Status == 0 ? false : true;
                me.Apply();
                $('#PerlengkapanCode').attr('disabled', 'disabled');

            }

        });
    }

    me.initialize = function () {
        var date = new Date;
        me.hasChanged = false;
        me.data.Status = true;
        $('#PerlengkapanCode').removeAttr('disabled');
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/MstPerlengkapan/Delete', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.init();
                        Wx.Success("Data deleted...");
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.saveData = function (e, param) {
        $http.post('om.api/MstPerlengkapan/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                    $('#PerlengkapanCode').attr('disabled', 'disabled');

                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (e, status, headers, config) {
                //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                console.log(e);
            });
    }

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Perlengkapan",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "Perlangkapan",
                title: "Perlengkapan",
                items: [
                        { name: "PerlengkapanCode", cls: "span2 full", text: "Kode Perlengkapan", maxlength: 15, required: true, validasi: "required" },
                        { name: "PerlengkapanName", cls: "span4 full", text: "Nama Perlengkapan", maxlength: 100, required: true, validasi: "required" },
                        { name: "Remark", cls: "span4 full", text: "Keterangan", maxlength: 100 },
                        { name: "Status", text: "Status Active", type: "x-switch", cls: "span2" }

                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("MasterPerlengkapanController");
    }

});