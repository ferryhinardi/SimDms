"use strict"

function omUtilUploadFromDCS($scope, $http, $injector) {
    var me = $scope;
    var routeURL = "om.api/UploadFromDCS/";

    $injector.invoke(BaseController, this, { $scope: me });

    me.dataIDChange = function () {
        me.clearTable(me.gridDCSData);
        me.data.Contents = "";
    }

    me.RetrieveData = function () {
        //$http.post(routeURL + "RetriveData", me.data)
        //.success(function (result) {
        //    me.loadTableData(me.gridDCSData, result.data);
        //})
        //.error(function (e) {
        //    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        //});
        me.retrieve();
    };

    me.retrieve = function () {
        $http.post(routeURL + "RetriveData", me.data)
        .success(function (result) {
            me.loadTableData(me.gridDCSData, result.data);
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };

    me.UploadData = function (confirmed) {
        var params = {
            Id: me.data.Id,
            contents: me.data.Contents,
        }

        if (me.data.DataID == 'SFPDA' && confirmed == false) {
            $.ajax({
                async: false,
                type: "POST",
                data: params,
                url: routeURL + "CheckSFPDA",
                success: function (data) {
                    if (data.message == "") {
                        if (data.isExist) {
                            MsgConfirm("Entry ini sudah ada di database. Lanjutkan Upload?", function (ok) {
                                if (ok) me.UploadData(true);
                                else return;
                            });
                            return;
                        }
                        else me.UploadData(true);
                    } else {
                        MsgBox(data.message);
                    }
                }
            });
        } else {
            $http.post(routeURL + "UploadData", params)
            .success(function (result) {
                if (result.success) {
                    Wx.Success("Data saved...");
                    //add by fhi 15.04.2015 refresh grid setelah proses upload berhasil
                    me.retrieve();
                    //end
                }
                else {
                    MsgBox(result.message, MSG_ERROR);
                }
            }).error(function (e) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
            });
        }        
    };

    me.CheckData = function () {
        var params = {
            DataID: me.data.DataID,
            Contents: me.data.Contents
        }

        $http.post(routeURL + "CheckData", params)
        .success(function (result) {
            if (result.success) {
                me.ShowCheckData(result.header, result.detail);
            }
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };
    
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
                me.CreateGrid(header,val);
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
    };

    me.CreateGrid = function (data,val) {
        var columns = [];
        var arrColumns = data[0];
        if (val == "0") {
            columns = [
             { id: "FieldDesc", header: "Field Name", fillspace: true },
             { id: "FieldValue", header: "Field Value", fillspace: true }
            ]
        } else {
            $.each(arrColumns, function (key, val) {
                var arrCol = { id: key, header: key, width: 120 }
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
            height: 350,
            scrollX: true,
            scrollY: true,
            data: data,
            select: "row"
        });
        grid2.refreshColumns();
    };

    me.initialize = function () {
        $http.post("om.api/UploadFromDCS/Default").
        success(function (result) {
            me.data.DateFrom = result.DateFrom;
            me.data.DateTo = result.DateTo;
            me.dsDataID = result.dsDataID
            if (result.IsOnline) {
                $("#DcsStatus").html("Online");
                $("#DcsStatus").css({
                    "font-size": "32px",
                    "color": "blue"
                });
            }
            else {
                $("#DcsStatus").html("Offline");
                $("#DcsStatus").css({
                    "font-size": "32px",
                    "color": "red"
                });
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });

        me.data.AllStatus = false;

        $("label[for=ctlTextData]").remove();
        $("label[for=textData]").remove();
        
        me.clearTable(me.gridDCSData);
    };

    webix.event(window, "resize", function () {
        me.gridDCSData.adjust();
    });

    me.gridDCSData = new webix.ui({
        container: "wxDCSData",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 302,
        columns: [
            { id: "CreatedDate", header: "Create Date", width: 120, format: me.dateTimeFormat, sort: "server" },
            { id: "DataID", header: "Data ID", width: 80 },
            { id: "Status", header: "Status", width: 80 },
            { id: "Header", header: "Info", width: 450 }
        ],
        on: {
            onSelectChange: function () {
                if (me.gridDCSData.getSelectedId() !== undefined) {
                    var rec = this.getItem(me.gridDCSData.getSelectedId().id);
                    me.data.Contents = rec.Contents;
                    me.data.Id = rec.ID;
                    me.Apply();
                }
            }
        }
    });

    //me.Test = function () {
    //    $.ajax({
    //        async: false,
    //        type: "POST",
    //        url: 'om.api/UploadFromDCS/Test',
    //        data: {
    //            DataID: me.data.DataID,
    //            DateFrom: me.data.DateFrom,
    //            DateTo: me.data.DateTo,
    //            AllStatus: true
    //        },
    //        success: function (result) {
    //            console.log(result);
    //            me.loadTableData(me.gridDCSData, result.data);
    //        }
    //    });

    //}


    me.start();
};

$(document).ready(function () {
    var options = {
        title: "Upload From DCS",
        xtype: "panels",
        panels: [
            {
                name: "pnlInquiry",
                cls: "full",
                items: [

                    { type: "label", text: "Inquiry DCS Data", cls: "span6", style: "font-size: 14px; color: blue; margin-top: 20px;" },
                    { name: "DcsStatus", type: "label", text: "", cls: "span1 right" },
                    { type: "div", cls: "divider" },
                    {
                        type: "controls",
                        cls: "span8",
                        text: "Periode",
                        items: [
                            { name: "DateFrom", model: "data.DateFrom", cls: "span1", type: 'ng-datepicker' },
                            { type: "label", text: "s/d", cls: "span1", style: "line-height: 33px;" },
                            { name: "DateTo", model: "data.DateTo", cls: "span1", type: 'ng-datepicker' },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span8",
                        text: "No. Req.",
                        items: [
                            { name: "DataID", model: "data.DataID", text: "Data ID", cls: "span3", type: "select2", datasource: "dsDataID", change: "dataIDChange()" },
                            { name: "AllStatus", model: "data.AllStatus", type: "ng-check", cls: "span1" },
                            { type: "label", text: "All Status", cls: "span1", style: "line-height: 33px;" },
                        ]
                    },
                    {
                        type: "buttons", cls: "full", items: [
                              {
                                  name: "btnRetrieve", text: " Retrieve", cls: "btn btn-primary span8 full", icon: "icon icon-search", click: "RetrieveData()"
                              },
                              //{
                              //    name: "btnTest", text: "Test", cls: "btn btn-primary span8 full", icon: "icon icon-error", click: "Test()"
                              //}
                        ]
                    },
                    { type: "label", text: "Preview DCS Data", style: "font-size: 14px; color : blue;" },
                    { type: "div", cls: "divider" },
                    {
                        name: "ctlTextData",
                        type: "controls",
                        text: "Preview DCS Data",
                        cls: "span8",
                        style: "margin-top: 15px;",
                        items: [
                            {
                                name: "wxDCSData",
                                cls: "span4",
                                type: "wxdiv"
                            },
                            {
                                name: "Contents", model: "data.Contents", type: "textarea", cls: "span4", text: "", style: "height: 250px; max-height: 250px;"
                            },
                            {
                                type: "buttons", cls: "span1", items: [
                                    {
                                        name: "btnUpload", text: " Upload", cls: "btn btn-success span2", icon: "icon icon-upload", click: "UploadData(false)"
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
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("omUtilUploadFromDCS");
    }
});