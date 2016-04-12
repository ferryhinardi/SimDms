var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";
var status = "";
var id = "";
var CodeID = "";
function spUploadFromDCSController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/Years').
    success(function (data, status, headers, config) {
        me.comboYear = data;
    });

    $http.post('sp.api/UploadFromDcs/WsStatus').
    success(function (data, status, headers, config) {
        $('#StatusWs').html(data.status);
    });

    $http.post('sp.api/Combo/DataIDCategory').
    success(function (data, status, headers, config) {
        me.comboDataID = data;
        console.log(data);
    });

    me.dataIDChange = function () {
        me.clearTable(me.grid1);
        me.data.Contents = "";
    }

    me.initialize = function () {
        me.data.AllStatus = false;
        me.clearTable(me.grid1);
        me.detail = {};

        $http.post("sp.api/UploadFromDCS/Default").
        success(function (result) {
            me.data.PeriodFrom = result.DateFrom;
            me.data.PeriodTo = result.DateTo;
        });

        if ($('#StatusWs').val() == "Online") {
            $("#StatusWs").css({
                "font-size": "32px",
                "color": "blue"
            });
        }
        else {
            $("#StatusWs").css({
                "font-size": "32px",
                "color": "red"
            });
        }
    }

    me.PopupSaveFile = function () {
        console.log("asdasd");
    }


    me.grid1 = new webix.ui({
        container: "wxUploadForm",
        view: "wxtable", css:"alternating",
        scrollY: true,
        scrollX: true,
        autoHeight: false,
        height: 302,
        defaultSort: "CreatedDate DESC",
        columns: [
            { id: "CreatedDate", header: "Create Date", fillspace: true, format: me.dateTimeFormat },
            { id: "DataID", header: "Data ID", fillspace: true },
            { id: "Status", header: "Status", fillspace: true },
            { id: "Header", header: "Info", fillspace: true }
        ],

        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    me.data.Contents = this.getItem(me.grid1.getSelectedId().id).Contents;
                    status = this.getItem(me.grid1.getSelectedId().id).Status;
                    id = this.getItem(me.grid1.getSelectedId().id).ID;
                    CodeID = this.getItem(me.grid1.getSelectedId().id).DataID;
                    me.Apply();
                }
            }
        }
    });

    $('#btnSaveFile').click(function () {
        alert("asdasd");
    });

    me.Retrieve = function () {

        $http.post('sp.api/UploadFromDcs/RetrieveDataFromDcs', me.data).
        success(function (data, status, headers, config) {
            console.log(data);
            if (data.success) {
                me.grid.detail = data.data;
                me.loadTableData(me.grid1, me.grid.detail);
            } else {
                MsgBox(data.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });

        
    }

    me.Upload = function () {

        $http.post('sp.api/UploadFromDcs/UploadData', { "Id": id }).
        success(function (data, status, headers, config) {
            if (data.success) {
                Wx.Success("Data saved...");
                //me.loadDetail(me.data);
            } else {
                MsgBox(data.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });

        console.log(CodeID, me.data.DataId);
    }

    me.Back = function () {

    }

    me.CheckData = function () {
        $http.post('sp.api/UploadFromDcs/MappingData', { "Id": id }).
        success(function (dl, status, headers, config) {
            me.ShowCheckData(dl.data, dl.detail);
        }).
        error(function (data, status, headers, config) {
            MsgBox("Terjadi Kesalahan, Hubungi SDMS Support ", MSG_ERROR);
        });
    }

    me.ShowCheckData = function (header, detail) {
        var data = [];
        var countDataDtl = detail.length;
        if (countDataDtl > 0) {
            for (var i = 0; i < (countDataDtl) ; i++) {
                if (i == 0) {
                    var arr = { id: 0, text: "Header" };
                    data.push(arr);
                    arr = { id: 1, text: "Detail" };
                    data.push(arr);
                }
                else {
                    var arr = { id: (i + 1), text: "Detail - " + (i + 1) };
                    data.push(arr);
                }
            }
        }
        $("#selectData").select2("val", "");

        $("#selectData").select2({
            data: data,
            initSelection: function (e, callback) {
                callback({ id: 0, text: "Header" });
            }
        }).on("change", function (e) {
            var val = (e.val || 0);
            if (val == 0) {
                me.CreateGrid(header, val);
            }
            else {
                if (val == 1) {
                    me.CreateGrid(detail[0], val);
                }
                else {
                    me.CreateGrid(detail[val - 1], val);
                }
            }
        }).trigger("change");

    }

    me.CreateGrid = function (data, val) {
        var columns = [];
        var arrColumns = data[0];
        if (val == "0") {
            columns = [
             { id: "FieldDesc", header: "Field Name", fillspace: true },
             { id: "FieldValue", header: "Field Value", fillspace: true }
            ]
        }
        else {
            $.each(arrColumns, function (key, val) {
                arrCol = { id: key, header: key, width: 120 }
                columns.push(arrCol);
            });
        }

        $("#gridContent").html("");
        var grid2 = new webix.ui({
            container: "gridContent",
            view: "datatable",
            columns: columns,
            autoheight: false,
            autowidth: false,
            width: 550,
            height: 430,
            scrollX: true,
            scrollY: true,
            data: data,
            select: "row"
        });
        grid2.refreshColumns();
    };


    function GenerateDataDtl(data) {
        var columnsIX = [];
        var arrColumns = data;
        var arrCol = {};

        $.each(arrColumns, function (i, val) {
            arrCol = { id: i, header: i, width: 120 }
            columnsIX.push(arrCol);
        });

    }

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Upload From DCS",
        xtype: "panels",
        panels: [
            {
                name: "pnlA",
                title: "",
                items: [
                        { name: "StatusWs", text: "", cls: "span1 right", readonly: true, type: "label" },
                        {
                            text: "Periode",
                            type: "controls",
                            cls: "span6",
                            items: [
                                { name: "PeriodFrom", text: "", cls: "span3", type: "ng-datepicker" },
                                { type: "label", text: "s.d", cls: "span1 mylabel" },
                                { name: "PeriodTo", text: "", cls: "span3", type: "ng-datepicker" },
                            ]
                        },
                        { name: "DataId", cls: "span4", type: "select2", text: "Data ID", datasource: "comboDataID",change: "dataIDChange()" },
                        { name: "AllStatus", text: "All Status", type: "x-switch", cls: "span4" },

                        {
                            type: "buttons", cls: "span2 full", items: [
                                { name: "btnRetrieve", text: "Retrieve", icon: "icon-gear", click: "Retrieve()", cls: "button small btn btn-warning" },
                            ]
                        },

                        { type: "label", text: "Preview DCS Data", style: "font-size: 14px; color : blue;" },
                        { type: "div", cls: "divider" },
                        {
                            name: "ctlTextData",
                            type: "controls",
                            text: "",
                            cls: "span8",
                            style: "margin-top: 15px;",
                            items: [
                                {
                                    name: "wxUploadForm",
                                    cls: "span4",
                                    type: "wxdiv",
                                },
                                {
                                    name: "Contents", type: "textarea", cls: "span4", text: "", style: "height: 250px; max-height: 250px;"
                                },
                                {
                                    type: "buttons", cls: "span1", items: [
                                        {
                                            name: "btnUpload", text: " Upload", cls: "btn btn-success span2", icon: "icon icon-upload", click: "Upload()"
                                        },
                                    ]
                                },
                                {
                                    type: "modalbuttons", cls: "span1", items: [
                                        {
                                            name: "btnCheck", text: " Check Data", cls: "btn btn-warning span2", icon: "icon icon-ok", click: "CheckData()",
                                            target: "AnalisaData",
                                            modalTitle: '<i class="icon icon-ok"></i> Analisa Data',
                                            modalContent: '<label for="selectData">Select One</label><input type="hidden" id="selectData"/><br/>' +
                                                    '<div id="gridContent"></div>',
                                            modalFooter:
                                                    //'<button type="button" class="btn btn-success">Save Data</button>' +
                                                    '<button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>'
                                        }
                                    ]
                                }

                            ]
                        },
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("spUploadFromDCSController");
    }

});

