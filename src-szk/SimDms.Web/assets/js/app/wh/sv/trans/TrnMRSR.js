var obj = [];
$(document).ready(function () {
    var options = {
        title: "Input Data MRSR",
        xtype: "panels",
        toolbars: [
            { name: "btnRefresh", text: "Refresh", cls: "btn btn-info", icon: "fa fa-refresh", click: "Refresh()" },
            { name: "btnSave", text: "Save", cls: "btn btn-info", icon: "fa fa-save", click: "Save()" },
            //{ name: "btnDownload", text: "Generate Excel", cls: "btn btn-info", icon: "fa fa-download", additional: "disabled=disabled", click: "Upload()" }
        ],
        panels: [
            {
                name: "pnlPeriod",
                cls: "full",
                items: [
                    {
                        type: "controls",
                        cls: "span7",
                        text: "Period",
                        items: [
                            { name: "PeriodYear", id: "PeriodYear", cls: "span2", type: "select", optionalText: "-- SELECT ONE --",  },
                        ]
                    }
                ]
            },
            { type: "div", cls: "divider" },
            { name: 'wxgridcell', xtype: 'wxtable' },
            //{
            //    type: "div", cls: "pagination", controls: [
            //        { type: "a"}
            //    ]
            //},
        ]
    };

    var Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        var hotCSIScore;
        var scoreresult;
        $.ajax({
            async: true,
            type: "POST",
            data: "",
            url: "wh.api/SvTrans/ReloadMRSR",
            success: function (response) {
                if (response.message == "Success") {
                   
                    var i, n = response.data.length;

                    obj[0] = 'Month/ Item';
                    for (i = 0; i < n; i++) {
                        obj[i + 1] = response.data[i];
                    }
                    hotCSIScore = hotRenderer();
                    scoreresult = "";

                    ReloadCallback(response);
                    console.log(hotCSIScore.getData(3, 1));
                }
                else {
                    sdms.info(response.data, response.message);

                }
            }
        });
        Wx.select({ selector: "[name=PeriodYear]", url: "wh.api/combo/years"});
        params = $('#pnlPeriod').serializeObject();
    }

    $('#btnRefresh').click(function () {
        Wx.render(init);
    });

    $('#btnSave').click(function () {
        params = $('#pnlPeriod').serializeObject();
        var data = hotCSIScore.getData().slice(0);
        data.splice(0, 2);
        //console.log(data);

        var error = "";
        if (params.PeriodYear == "") {
            error += "Tahun masih kosong";
        };

        if (error != "") {
            sdms.info(error, "Error");
        } else {
            listdata = [];
            for (var i = 0; i < data.length; i++) {
                var listdt = new Object();
                for (var j = 0; j < data[i].length; j++) {
                    //listdt.data = data[i][j + 1];
                    if (data[i][j + 1] != null && data[i][data[i].length - 9] != null) {
                        listdt.productType = "4W";
                        listdt.year = params.PeriodYear;
                        listdt.month = data[i][0];
                        listdt.MRSRCode = j + 1;
                        listdt.data = data[i][j + 1];
                        
                        listdata.push(rowToJson(listdt));
                    }
                }
            }
            params.listScore = JSON.stringify(listdata);

            $.ajax({
                async: true,
                type: "POST",
                data: params,
                url: "wh.api/SvTrans/MRSRSave",
                success: function (data) {
                    if (data.success) {
                        scoreresult = data.value;
                        //$('#btnSave').attr('disabled', 'disabled');
                        //$('#btnDownload').removeAttr('disabled');
                        sdms.info("Success", "Success");
                        getData();
                        return;
                    }
                    if (!data.success) {
                        sdms.info(data.message, "Error");
                    }
                }
            });
        }
    });

    $("#PeriodYear").on("change", function () {
        getData();
    });

});


function ReloadCallback(result) {

    if (result.data2 !== null) {
        var i, n = result.data2.length;
        var datadump = [];
        datadump.push([1, 0, "Previous year Average"])
        for (i = 0; i < n; i++) {
            //datadump.push(me.jsonToRow(response.data[i]));
            var returnObj = result.data2[i];
            datadump.push([2 + i, 0, returnObj])
        }
        
        //hotCSIScore.alter('remove_row', 0, window.hotCSIScore.countRows());
        hotCSIScore.setDataAtCell(datadump, null, null, 'loadData');

    }
}

function onGenerate() {
    var a = 1;
}

function hotRenderer() {
   
    var grayRenderer = function (instance, td, row, col, prop, value, cellProperties) {
        Handsontable.renderers.TextRenderer.apply(this, arguments);
        td.style.backgroundColor = '#ECECEC';
    };

    var codeRenderer = function (instance, td, row, col, prop, value, cellProperties) {
        Handsontable.renderers.TextRenderer.apply(this, arguments);
        td.style.color = '';
        td.style.background = '';
    };
    
    //var data = [
    //    ["Month/Item", "Previous Year Average", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]
    //],
    var data = [obj],
      container = document.getElementById('wxgridcell'),
      settings1 = {
          data: data,
          colWidths: [80, 80, 70, 70, 70, 119, 100, 100, 100, 100],
          columns: [
              {
                  //data: "VP"
                  //type: "numeric",
                  //format: "0,0.00",
                  //className: "htCenter"
              },
              {
                  type: 'date',
                  dateFormat: 'YYYYMMDD',
                  correctFormat: true,
                  defaultDate: new Date()
              },
              {
                  //data: "Total Score"
                  //type: "numeric",
                  //format: "0,0.00",
                  //className: "htCenter"
              },
               {
                   //data: "SI"
                   //type: "numeric",
                   //format: "0,0.00",
                   //className: "htCenter"
               },
              {
                  //data: "SA"
                  //type: "numeric",
                  //format: "0,0.00",
                  //className: "htCenter"
              },
              {
                  //data: "SF"
                  //type: "numeric",
                  //format: "0,0.00",
                  //className: "htCenter"
              },
              {
                  //data: "VP"
                  //type: "numeric",
                  //format: "0,0.00",
                  //className: "htCenter"
              },
              {
                  //data: "SQ"
                  //type: "numeric",
                  //format: "0,0.00",
                  //className: "htCenter"
              },
              {
                  //data: "Total Score"
                  //type: "numeric",
                  //format: "0,0.00",
                  //className: "htCenter"
              },
              {
                  //data: "SQ"
                  //type: "numeric",
                  //format: "0,0.00",
                  //className: "htCenter"
              },
          ],
          contextMenu: true,
          currentRowClassName: 'currentRow',
          currentColClassName: 'currentCol',
          autoWrapRow: true,
          manualColumnResize: true,
          persistentState: true,
      };
    
    hotCSIScore = new Handsontable(container, settings1);
    hotCSIScore.render();
    hotCSIScore.updateSettings({
        cells: function (row, col, prop) {
            var cellProperties = {};
            if (row === 0 || row === 1) {
                cellProperties.readOnly = true;
                if (row === 0) {
                    cellProperties.renderer = function (instance, td, row, col, prop, value, cellProperties) {
                        Handsontable.renderers.TextRenderer.apply(this, arguments);
                        td.style.backgroundColor = '#a39f9f';
                        td.style.fontWeight = 'bold';
                    };
                }

                if (row === 1) {
                    cellProperties.renderer = function (instance, td, row, col, prop, value, cellProperties) {
                        Handsontable.renderers.TextRenderer.apply(this, arguments);
                        td.style.backgroundColor = '#ECECEC';
                    };
                }
            }

            return cellProperties;
        },
        cell: [
          { row: 0, col: 0, className: "htCenter htMiddle" },
          { row: 0, col: 1, className: "htCenter htMiddle" },
          { row: 0, col: 2, className: "htCenter htMiddle" },
          { row: 0, col: 3, className: "htCenter htMiddle" },
          { row: 0, col: 4, className: "htCenter htMiddle" },
          { row: 0, col: 5, className: "htCenter htMiddle" },
          { row: 0, col: 6, className: "htCenter htMiddle" },
          { row: 0, col: 7, className: "htCenter htMiddle" },
          { row: 0, col: 8, className: "htCenter htMiddle" },
          { row: 0, col: 9, className: "htCenter htMiddle" },
        ],
    });
   
    //$('.ht_master').css('position', 'absolute');
    $('.wtHolder').css('margin-top', '0');
    return hotCSIScore;
}

function rowToJson(data) {
    console.log(data.productType)
    console.log(data.year)
    console.log(new Date(Date.parse(data.month + " 1," + data.year)).getMonth() + 1)
    console.log(data.MRSRCode)
    console.log(data.data)
    var returnObj = new Object();
    returnObj.CompanyCode = "000000";
    returnObj.BranchCode = "000000";
    returnObj.ProductType = data.productType;
    returnObj.PeriodYear = data.year;
    returnObj.PeriodMonth = new Date(Date.parse(data.month + " 1," + data.year)).getMonth() + 1;
    returnObj.MRSRCode = data.MRSRCode;
    returnObj.MRSRData = data.data;
    return returnObj;
}

function getData() {
    var params = $("#pnlPeriod").serializeObject();
    $('.page > .ajax-loader').show();
    $.ajax({
        async: true,
        type: "POST",
        data: params,
        url: "wh.api/SvTrans/getData",
        success: function (data) {
            for (i = 0; i < 12; i++) {
                //var returnObj = data.data[i];
                hotCSIScore.setDataAtCell(2 + i, 1, null);
                hotCSIScore.setDataAtCell(2 + i, 2, null);
                hotCSIScore.setDataAtCell(2 + i, 3, null);
                hotCSIScore.setDataAtCell(2 + i, 4, null);
                hotCSIScore.setDataAtCell(2 + i, 5, null);
                hotCSIScore.setDataAtCell(2 + i, 6, null);
                hotCSIScore.setDataAtCell(2 + i, 7, null);
                hotCSIScore.setDataAtCell(2 + i, 8, null);
                hotCSIScore.setDataAtCell(2 + i, 9, null);
            }
            if (data.message == "Success") {
                //console.log(data.data[0]);
                var i, n = data.data.length;
                var datadump = [];
                var j = 2;
                    for (i = 0; i < n ; i++) {
                        var m = data.data[i];
                        //var returnObj = data.data[i];
                        hotCSIScore.setDataAtCell(j, 1, m.a);
                        hotCSIScore.setDataAtCell(j, 2, m.b);
                        hotCSIScore.setDataAtCell(j, 3, m.c);
                        hotCSIScore.setDataAtCell(j, 4, m.d);
                        hotCSIScore.setDataAtCell(j, 5, m.e);
                        hotCSIScore.setDataAtCell(j, 6, m.f);
                        hotCSIScore.setDataAtCell(j, 7, m.g);
                        hotCSIScore.setDataAtCell(j, 8, m.h);
                        hotCSIScore.setDataAtCell(j, 9, m.i);
                        j++;
                    }
                    $('.page > .ajax-loader').hide();
                //hotCSIScore.setDataAtCell(0, 0, 'new value');
                //hotCSIScore.setDataAtCell(datadump, null, null, 'loadData');
            } else {
                sdms.info(data.message, "Error");
            }
        }
    });
}

