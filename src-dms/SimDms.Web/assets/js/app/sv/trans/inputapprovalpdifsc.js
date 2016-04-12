"use strict"

function inputapprovalpdifsc($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $("p[data-name='0']").click(function () {
        me.tabClick('0');
    });
    $("p[data-name='1']").click(function () {
        me.tabClick('1');
    });

    $("label[ng-model='transType'] ").click(function () {
        var test1 = $(this).text();
        me.opts(test1);
    });

    me.grid0 = new webix.ui({
        container: "wxApproval",
        view: "wxtable", css:"alternating",
        checkboxRefresh: true,
        columns: [
            { id: "IsSelected", header: { content: "masterCheckbox", contentId: "chkSelect" }, template: "{common.checkbox()}", width: 40 },
            { id: "No", header: "No", width: 50, sort: "int" },
            { id: "JobOrderNo", header: "No SPK", width: 140, sort: "string" },
            { id: "JobOrderDate", header: "Tgl SPK", width: 140, format: me.dateFormat, sort: "string" },
            { id: "ServiceBookNo", header: "No Buku Service", width: 140, sort: "string" },
            { id: "ChassisNo", header: "No Chassis", width: 140, sort: "string" },
            { id: "BasicModel", header: "Basic Model", width: 140, sort: "string" },
            { id: "JobType", header: "Tipe Pekerjaan", width: 160, sort: "string" },
            { id: "TotalApprove", header: "Total", width: 100, sort: "string" }
        ]
    });

    me.grid1 = new webix.ui({
        container: "wxUnapproval",
        view: "wxtable", css:"alternating",
        checkboxRefresh: true,
        columns: [
            { id: "IsSelected", header: { content: "masterCheckbox", contentId: "chkSelect" }, template: "{common.checkbox()}", width: 40 },
            { id: "No", header: "No", width: 50, sort: "int" },
            { id: "JobOrderNo", header: "No SPK", width: 140, sort: "string" },
            { id: "JobOrderDate", header: "Tgl SPK", width: 140, sort: "" },
            { id: "ServiceBookNo", header: "No Buku Service", width: 140,  sort:"string" },
            { id: "ChassisNo", header: "No Chassis", width: 140, sort: "string" },
            { id: "BasicModel", header: "Basic Model", width: 140, sort: "string" },
            { id: "JobType", header: "Tipe Pekerjaan", width: 160, sort: "string" },
            { id: "TotalApprove", header: "Total", width: 100, sort: "string" }
        ]
    });

    me.grid0.attachEvent('onCheck', function (id, e, node) {
        me.eventOnCheck('grid0', id);
    });

    me.grid1.attachEvent('onCheck', function (id, e, node) {
        me.eventOnCheck('grid1', id);
    });

    me.eventOnCheck = function (gridName, id) {
        var ctl = gridName == 'grid1' ? me.grid1 : me.grid0;
        if (id == undefined) {
            var count = ctl.count();
            me.total = 0;
            for (var i = 0; i < count; i++) {
                var idx = ctl.getIdByIndex(i);
                var item = ctl.getItem(idx);
                if (item.IsSelected == 1) me.total += item.TotalApprove;
            }
        } else {
            var row = ctl.getItem(id);
            if (row.IsSelected == 0) {
                me.total -= row.TotalApprove;
            }
            else if (row.IsSelected == 1) {
                me.total += row.TotalApprove;
            }
        }
        $('#Total').html(number_format(me.total));
    }

    me.tabClick = function (index) {
        switch (index) {
            case '0':
                me.grid0.adjust();
                me.clearTable(me.grid0);
                me.clearTable(me.grid1);               
                $('#btnProcess').html('<i class="icon icon-gear"></i>Proses Approve');
                me.currentTab = 0;
                break;
            case '1':
                me.grid1.adjust();
                me.clearTable(me.grid0);
                me.clearTable(me.grid1);
                $('#btnProcess').html('<i class="icon icon-gear"></i>Proses Unapprove');
                me.currentTab = 1;
                break;
            default: break;
        }
    }

    me.opts = function (opt) {
        switch (opt) {
            case 'PDI / PDC':
                me.transType = "optionA";
                me.Refresh();
                break;
            case 'FSC / KSG':
                me.transType = "optionB";
                me.Refresh();
                break;
            default: break;                
        }
        console.log(opt);
    }


    me.Refresh = function () {
        var data = {
            isPDI: me.transType == "optionA"
        }
        console.log(me.transType);
        var url = me.currentTab == 0 ?
            'sv.api/InputApproval/GetSPKforApprovalPDIFSC' :
            'sv.api/InputApproval/GetSPKForUnApprovalPdiFsc';

        var grid = me.currentTab == 0 ? me.grid0 : me.grid1;

        $http.post(url, data).
            success(function (data, status, headers, config) {
                me.loadTableData(grid, data.result);
                if (me.currentTab == 0) me.detail0 = data.result;
                else me.detail1 = data.result;
            }).error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    }

    me.Process = function () {
        var grid = me.currentTab == 0 ? me.grid0 : me.grid1;
        var detail = me.currentTab == 0 ? me.detail0 : me.detail1;
        var url = 'sv.api/InputApproval/' + (me.currentTab == 0 ? 'ApproveSPKPdiFsc' : 'UnApproveSPKPdiFsc');
        var msg = 'Proses ' + (me.currentTab == 0 ? 'Approve' : 'Unapprove') + ' berhasil dilakukan';

        if (grid.count() <= 0) {
            MsgBox("Tidak ada data yang dipilih");
            return;
        }
        var data = [];
        $.each(detail, function (key, val) {
            if (val["IsSelected"] == 1) {
                data.push(detail[key]);
            }
        });
        if (data.length == 0) {
            MsgBox("Tidak ada data yang dipilih");
            return;
        }
        MsgConfirm("Apakah anda yakin ?", function (ok) {
            if (!ok) return;

            var docs = [];
            $.each(data, function (key, val) {
                var arr = {
                    "BranchCode":val["BranchCode"],
                    "ServiceNo":val["ServiceNo"]
                }
                docs.push(arr);
            });

            var JSONData = JSON.stringify(docs);


            console.log(JSONData);

            $http.post(url, JSONData).
                success(function (data, status, headers, config) {
                    if (data.message == "") {
                        MsgBox(msg);
                        me.Refresh();
                        me.total = 0;
                        $('#Total').html("");
                        //$('#btnProcess').attr('disabled', 'disabled');
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });
        });
    }
    
    me.initialize = function () {
        me.detail0 = {};
        me.detail1 = {};
        me.total = 0;
        me.currentTab = 0;
        me.transType = "optionA";
        $('#Total').css(
            {
                "font-size": "28px",
                "color": "blue",
                "font-weight": "bold",
                "text-align": "right"
            });
        $('#Total').html("");
        $("p[data-name='0']").click();
        me.clearTable(me.grid0);
        me.clearTable(me.grid1);
        me.Refresh();
        $('#btnProcess').removeAttr('disabled');
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Input Approval",
        xtype: "panels",
        panels: [
             {
                 title: "Proses Approval PDI FSC",
                 items: [
                     {
                         type: "optionbuttons",
                         text: "Choose Transaction :",
                         model: "transType",
                         name: "transType",
                         cls: "span4",
                         items: [
                             //{ name: "optionA", text: "PDI / PDC", click: "Refresh()" },
                             //{ name: "optionB", text: "FSC / KSG", click: "Refresh()" }
                            { name: "optionA", text: "PDI / PDC" },
                            { name: "optionB", text: "FSC / KSG" }
                         ]
                     },                      
                     { name: "lblTotal", text: "Total Approve: ", type: "label", cls: "span2", readonly: true },
                     { name: "Total", text: "", type: "label", cls: "span2", readonly: true },
                     {
                         type: "buttons",
                         items: [
                             { name: "btnProcess", text: "Proses Approve", cls: "btn btn-info", icon: "icon-gear", click: "Process()" }
                         ]
                     }
                 ]
             },
             {
                 xtype: "tabs",
                 name: "tabpageDetail",
                 items: [
                     { name: "0", text: "List Approval" },
                     { name: "1", text: "List Unapproval" }
                 ]
             },
             {
                 name: "0",
                 cls: "tabpageDetail 0",
                 items: [
                     { name: "wxApproval", type: "wxdiv" }
                 ]
             },
             {
                 name: "1",
                 cls: "tabpageDetail 1",
                 items: [
                     { name: "wxUnapproval", type: "wxdiv" }
                 ]
             },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("inputapprovalpdifsc");
    }

});