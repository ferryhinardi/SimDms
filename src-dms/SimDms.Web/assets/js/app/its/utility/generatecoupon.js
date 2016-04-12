"use strict"

function generatecoupon($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.GridActive = "0";
        me.cbAllData = false;
        me.Couponcb = 0;
        me.Datecb = 0;
        me.pageSize = 10;
        me.pages = 0;
        me.Coupons = [];
        me.CouponDatas = [];
        me.CouponFrom = "";
        me.CouponTo = "";
        me.DateFrom = "";
        me.DateTo = "";
    };

    me.refresh = function () {
        var url = 'its.api/UtilityCoupon/Coupons';

        if (me.Couponcb == 0) {
            me.CouponFrom = "";
            me.CouponTo = "";
        }
        else {
            me.CouponFrom = $('#CouponFrom').val();
            me.CouponTo = $('#CouponTo').val();
        }

        if (me.Datecb == 0) {
            me.DateFrom = "";
            me.DateTo = "";
        }
        else {
            me.DateFrom = $('#DateFrom').val() + ' 00:00:00';
            me.DateTo = $('#DateTo').val() + ' 23:59:00';
        }

        $.ajax({
            async: false,
            type: "POST",
            url: url,
            data: {
                cbAllData: me.cbAllData,
                Outlet: $('#BranchCode').val(),
                CouponFrom: me.CouponFrom,
                CouponTo: me.CouponTo,
                DateFrom: me.DateFrom,
                DateTo: me.DateTo
            },
            success: function (v, status, headers, config) {
                $('.page > .ajax-loader').hide();
                if (v.data !== undefined) {
                    me.pages = v.data.length / me.pageSize;
                    for (var i = 1; i < me.pages + 1; i++) {
                        if (me.Coupons[i] === undefined) {
                            me.Coupons[i] = [];
                        }
                    }
                    me.refreshGrid(v.data);
                    $('#btnProcess').show();
                }
                else
                    MsgBox(v.message, MSG_ERROR);

            },
            error: function (e, status, headers, config) {
                $('.page > .ajax-loader').hide();
                MsgBox(e, MSG_ERROR);
            }
        });

    };

    me.refreshGrid = function (result) {
        Wx.kgrid({
            data: result,
            name: "Coupons",
            serverBinding: false,
            pageSize: me.pageSize,
            pageSizes: true,
            columns: [
                //{
                //    field: "CoupunNumber", title: " ", width: 50,
                //    template: "<input type='checkbox' id='#: CoupunNumber #' />",
                //    filterable: { multi: true },
                //    sortable: false
                //},
                { field: "CompanyCode", title: "CompanyCode", hidden: true },
                { field: "InquiryNumber", title: "InquiryNumber", hidden: true },
                {
                    field: "CoupunNumber", title: "No Kupon", width: 150,
                    filterable: {
                        multi: true
                    }
                },
                { field: "TestDriveDate", title: "Tanggal Test Drive", width: 200, filterable: { multi: true }, template: "#= (TestDriveDate == undefined) ? '' : moment(TestDriveDate).format('DD MMM YYYY') #" },
                { field: "NamaProspek", title: "Nama", width: 150 },
                { field: "ProspekIdentityNo", title: "No SIM A", width: 150 },
                { field: "AlamatProspek", title: "Alamat", width: 200 },
                { field: "TelpRumah", title: "Telp/HP", width: 150 },
                { field: "Email", title: "Email", width: 200 },
                { field: "EmployeeName", title: "Nama Salesman", width: 250 },
                { field: "EmployeeID", title: "ID SIS (ITS)", width: 150 },
                { field: "IdentityNo", title: "No KTP", width: 150 },
                //{ field: "CompanyName", title: "Dealer", width: 300 },
                //{ field: "BranchName", title: "Daerah", width: 400 },
                { field: "CompanyGovName", title: "Dealer", width: 300 },
                { field: "CompanyName", title: "Daerah", width: 400 },
                { field: "Remark", title: "Remarks", width: 10 },
                { field: "ProcessFlag", title: "ProcessFlag", hidden: true }
            ],
            dataBound: function (e) {
                me.GridActive = "1";
                //setTimeout(function () {
                //    if ($('#cbSelectAll').prop('checked')) {
                //        $('#Coupons input[type=checkbox]').attr('checked', true);
                //    }
                //    else {
                //        $('#Coupons input[type=checkbox]').removeAttr('checked');
                //    }

                //    var page = $('li.k-current-page').children('span.k-link.k-pager-nav').text();

                //    if (me.Coupons[page] !== undefined) {
                //        for (var i = 0; i < me.pageSize; i++) {
                //            console.log(me.Coupons[page][i])
                //            $('#' + me.Coupons[page][i]).attr('checked', true);
                //        }
                //    }
                //    $('#Coupons input[type=checkbox]').click(function () {
                //        if (this.checked) {
                //            me.Coupons[page].push($(this).attr('id'));
                //            console.log(me.Coupons[page])
                //            me.CouponDatas.push({
                //                CompanyCode: $(this).parent('td').siblings('td:eq(1)').text(),
                //                InquiryNumber: $(this).parent('td').siblings('td:eq(2)').text(),
                //                CouponData: $(this).attr('id')
                //            });
                //        }
                //        else {
                //            me.Coupons[page].pop($(this).val());
                //            me.CouponDatas.pop({
                //                CompanyCode: $(this).parent('td').siblings('td:eq(1)').text(),
                //                InquiryNumber: $(this).parent('td').siblings('td:eq(2)').text(),
                //                CouponData: $(this).attr('id')
                //            });
                //        }
                //    });

                //}, 50);
            }
        });
    }

    $('#cbSelectAll').on('click', function (e) {
        $('#Coupons').data("kendoGrid").refresh();
    });

    me.save = function () {
        $('#Coupons').data("kendoGrid").refresh();
        var grid = $('#Coupons').data("kendoGrid"); //{ All: $('#cbSelectAll').prop('checked'), CouponDatas: me.CouponDatas };

        var dataSource = grid.dataSource;
        var filters = dataSource.filter();
        var allData = dataSource.data();
        var query = new kendo.data.Query(allData);
        var currentData = query.filter(filters).data;

        var dt = [];

        $.each(currentData, function (key, val) {
            var arr = {
                //"CompanyCode": val["CompanyCode"],
                //"InquiryNumber": val["InquiryNumber"],
                "CoupunNumber": val["CoupunNumber"]
            }
            dt.push(arr);
            console.log(dt);
        });

        //var data = JSON.stringify(currentData);
        var data = JSON.stringify(dt);
        console.log(data);

        //if (data.All && data.CouponDatas.length < 1) {
        if (data == []) {
            Wx.Error("No data selected");
            return;
        }

        var url = 'its.api/UtilityCoupon/Generate';
        $http.post(url, data)
            .success(function (v, status, headers, config) {
                if (v.value !== undefined) {
                    $('.page > .ajax-loader').hide();
                    me.refreshGrid(null);
                    me.exportXls(v);
                }
                else
                    //MsgBox(v.message, MSG_ERROR);
                    MsgBox(v.message, MSG_INFO);
            }).error(function (e, status, headers, config) {
                $('.page > .ajax-loader').hide();
                MsgBox(e, MSG_ERROR);
            });
    }

    me.exportXls = function (data) {
        sdms.info("Please wait...");
        location.href = 'its.api/UtilityCoupon/DownloadExcelFiles?key=' + data.value;
    }

    $http.post('its.api/combo/Outlets?id=ALL').
    success(function (data, status, headers, config) {
        me.Outlets = data;
    });

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Generate Coupon",
        xtype: "panels",
        toolbars: [
                { name: "btnProcess", text: "Process", cls: "btn btn-success", icon: "icon-gear", click: "save()", style: "display: none" },
                { name: "btnRefresh", text: "Refresh", cls: "btn btn-refresh", icon: "icon-refresh", click: "refresh()" }
        ],
        panels: [
            {
                name: "pnlGenerate",
                items: [
                    { name: "BranchCode", text: "Outlet", type: "select2", cls: "span6", opt_text: "-- SELECT ALL --", datasource: "Outlets" },
                    {
                        name: "Coupon", text: "Coupon", type: "controls",
                        items: [
                            { name: "Couponcb", type: "ng-check", cls: "span1", model: "Couponcb" },
                            { name: "CouponFrom", type: "text", cls: "span2", disable: "Couponcb == 0" },
                            { name: "labelCoupon", type: "label", cls: "span1", text: "to" },
                            { name: "CouponTo", type: "text", cls: "span2", disable: "Couponcb == 0" }
                        ]
                    },
                    {
                        name: "TestDriveDate", text: "Test Drive Date", type: "controls",
                        items: [
                            { name: "Datecb", type: "ng-check", cls: "span1", model: "Datecb" },
                            { name: "DateFrom", type: "ng-datepicker", cls: "span2", disable: "Datecb == 0" },
                            { type: "label", text: "to", cls: "span1", style: "line-height: 33px" },
                            { name: "DateTo", type: "ng-datepicker", cls: "span2", disable: "Datecb == 0" },
                        ]
                    },
                    {
                        name: "CSelectAll", type: "controls", items: [
                            //{ name: "cbSelectAll", type: "ng-check", cls: "span1", disable: "GridActive == 0" },
                            //{ name: "lblSelectAll", type: "label", cls: "span2", text: "Select All", disable: "GridActive == 0" },
                            { name: "cbAllData", type: "ng-check", cls: "span1", disable: "GridActive == 0", model: "cbAllData" },
                            { name: "lblAllData", type: "label", cls: "span2", text: "Show All Data", disable: "GridActive == 0" }
                        ]
                    }
                ]
            },
            {
                name: "Coupons",
                xtype: "k-grid",
            },
        ]
    };

    Wx = new SimDms.Widget(options);

    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("generatecoupon");
    }

});

//failed
function epicfail() {
    if ($('#cbSelectAll').prop('checked')) {
        $('#Coupons tbody input[type=checkbox]').attr('checked', true);

        for (var i = 0; i < me.pages; i++) {
            for (var j = 0; i < me.pageSize; i++) {
                me.Coupons[i].push($(this).attr('id'));
                me.CouponDatas.push({
                    CouponData: $(this).attr('id')
                });
            }
        }
        $('#couponheader').attr('checked', true);
        //$('#couponheader').click();
    }
    else {
        for (var i = 0; i < me.pages; i++) {
            for (var j = 0; i < me.pageSize; i++) {
                me.Coupons[i].pop($(this).attr('id'));
                me.CouponDatas.pop({
                    CouponData: $(this).attr('id')
                });
            }
        }
        //$('#Coupons input[type=checkbox]').removeAttr('checked');
        //$('#Coupons tbody input[type=checkbox]').removeAttr('checked');
        $('#couponheader').removeAttr('checked');
    }

    var page = $('li.k-current-page').children('span.k-link.k-pager-nav').text();


    if (me.Coupons[page] !== undefined) {
        if (me.Coupons[page].length == me.pageSize) {
            $('#Coupons thead input[type=checkbox]').attr('checked', true);
        }
        else {
            $('#Coupons thead input[type=checkbox]').removeAttr('checked');
        }
        for (var i = 0; i < me.pageSize; i++) {
            console.log(me.Coupons[page][i])
            $('#' + me.Coupons[page][i]).attr('checked', true);
        }
    }
    $('#Coupons tbody input[type=checkbox]').click(function () {
        if (this.checked) {
            me.Coupons[page].push($(this).attr('id'));
            me.CouponDatas.push({
                CompanyCode: $(this).parent('td').siblings('td:eq(1)').text(),
                InquiryNumber: $(this).parent('td').siblings('td:eq(2)').text(),
                CouponData: $(this).attr('id')
            });
        }
        else {
            me.Coupons[page].pop($(this).val());
            me.CouponDatas.pop({
                CompanyCode: $(this).parent('td').siblings('td:eq(1)').text(),
                InquiryNumber: $(this).parent('td').siblings('td:eq(2)').text(),
                CouponData: $(this).attr('id')
            });
            $('#cbSelectAll').removeAttr('checked');
        }
    });

    $('#couponheader').click(function (e) {
        //e.preventDefault();
        $('#Coupons').data("kendoGrid").refresh();
        //$('#couponheader').attr('checked', true);
        //$('#Coupons tbody input[type=checkbox]').click();
        //if ($('#couponheader').prop('checked') && !$('#Coupons tbody input[type=checkbox]').prop('checked')) {
        //    $('#Coupons tbody input[type=checkbox]').attr('checked', true);
        //}
        //else if (!$('#couponheader').prop('checked') && $('#Coupons tbody input[type=checkbox]').prop('checked')) {
        //    $('#Coupons tbody input[type=checkbox]').removeAttr('checked');
        //}
    });

}