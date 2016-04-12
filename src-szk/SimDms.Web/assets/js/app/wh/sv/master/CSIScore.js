$(document).ready(function () {
    var options = {
        title: "Customer Satisfaction Index Score",
        xtype: "panels",
        toolbars: [
            { name: "btnRefresh", text: "Refresh", cls: "btn btn-info", icon: "fa fa-refresh", click: "Refresh()" },
            { name: "btnSave", text: "Save", cls: "btn btn-info", icon: "fa fa-save", click: "Save()" },
            { name: "btnDownload", text: "Generate Excel", cls: "btn btn-info", icon: "fa fa-download", additional: "disabled=disabled", click: "Upload()" }
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
                            { name: "PeriodYear", id: "PeriodYear", cls: "span2", type: "select" },
                            { name: "PeriodMonth", id: "PeriodMonth", cls: "span3", type: "select" },
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
        Wx.select({ selector: "[name=PeriodYear]", url: "wh.api/combo/years" });
        Wx.select({ selector: "[name=PeriodMonth]", url: "wh.api/combo/listofmonth" });
    }

    var hotCSIScore = hotRenderer();
    var scoreresult = "";

    $('#btnRefresh').click(function () {
        params = $('#pnlPeriod').serializeObject();
        //var data = hotCSIScore.getData().slice(0);
        //data.splice(0, 1);
        
        $.ajax({
            async: true,
            type: "POST",
            data: params,
            url: "wh.api/CSIScore/ReloadCSIScore",
            success: function (response) {
                if (response.message == "Success") {
                    ReloadCallback(response);
                    $('#btnDownload').attr('disabled', 'disabled');
                    $('#btnSave').removeAttr('disabled');
                }
                else {
                    sdms.info(response.data, response.message);

                }
            }
        });
    });

    $('#btnSave').click(function () {
        params = $('#pnlPeriod').serializeObject();
        var data = hotCSIScore.getData().slice(0);
        data.splice(0, 1);

        var error = "";
        if (params.PeriodYear == "") {
            error += "Tahun masih kosong";
        };
        if (params.PeriodMonth == "") {
            error += "Bulan masih kosong";
        };
        if (data.length === 1) {
            error += "Data masih kosong";
        };
        if (error != "") {
            sdms.info("Data belum lengkap", "Error");
            return;
        }

        listdata = [];

        for (var i = 0; i < data.length - 1; i++) {
            listdata.push(rowToJson(data[i]));
        }
        params.listScore = JSON.stringify(listdata);

        $.ajax({
            async: true,
            type: "POST",
            data: params,
            url: "wh.api/report/UploadCSIScore",
            success: function (data) {
                if (data.success) {
                    scoreresult = data.value;
                    $('#btnSave').attr('disabled', 'disabled');
                    $('#btnDownload').removeAttr('disabled');
                    sdms.info("Success", "Success");
                    return;
                }
                if (!data.success && data.errorlist !== undefined) {
                    var grayRenderer = function (instance, td, row, col, prop, value, cellProperties) {
                        Handsontable.renderers.TextRenderer.apply(this, arguments);
                        td.style.backgroundColor = '#ECECEC';
                    };

                    var wrongCodeRenderer = function (instance, td, row, col, prop, value, cellProperties) {
                        Handsontable.renderers.TextRenderer.apply(this, arguments);

                        if (col < 3) {
                            for (var i = 0; i < data.errorlist.length; i++) {
                                if (hotCSIScore.getDataAtCell(row, 1) === data.errorlist[i].ServiceCode) {
                                    if (col === 0) {
                                        td.style.background = 'red';
                                    }
                                    else if (col === 1 || col === 2) {
                                        td.style.color = 'red';
                                    }
                                    break;
                                }
                            }
                        }
                    };

                    Handsontable.renderers.registerRenderer('wrongCodeRenderer', wrongCodeRenderer);
                    settings2 = {

                        cells: function (row, col, prop) {
                            var cellProperties = {};

                            if (row < 1) {
                                this.renderer = grayRenderer;
                                cellProperties.readOnly = true;
                            }
                            else if (col < 3) {
                                if (!hotCSIScore.isEmptyRow(row)) {
                                    cellProperties.renderer = "wrongCodeRenderer";
                                }
                            }

                            return cellProperties;
                        }
                    };
                    hotCSIScore.updateSettings(settings2);

                    sdms.info("Data dealer belum benar", "Error");
                    return;
                }
                if (!data.success) {
                    sdms.info("Success", "Error");
                }
            }
        });
    });

    $('#btnDownload').click(function () {
        if (scoreresult != "") {
            location.href = 'wh.api/report/DownloadExcelFile?key=' + scoreresult + '&filename=CSI Score';
        }
        else {
            sdms.info("No data to download", "Error");
        }
        scoreresult = "";
        $('#btnDownload').attr('disabled', 'disabled');
        $('#btnSave').removeAttr('disabled');
        saveCallback();
    });
});

function saveCallback() {
    hotCSIScore.alter('remove_row', 1, window.hotCSIScore.countRows());
    hotCSIScore.setDataAtCell([], null, null, 'loadData');
}

function ReloadCallback(result) {

    if (result.data !== null) {
        var i, n = result.data.length;
        var datadump = [];

        for (i = 0; i < n; i++) {
            //datadump.push(me.jsonToRow(response.data[i]));
            var returnObj = result.data[i];
            datadump.push([1 + i, 0, i + 1])
            datadump.push([1 + i, 1, returnObj.ServiceCode])
            datadump.push([1 + i, 2, returnObj.ServiceName])
            datadump.push([1 + i, 3, returnObj.ServiceInitiation])
            datadump.push([1 + i, 4, returnObj.ServiceAdvisor])
            datadump.push([1 + i, 5, returnObj.ServiceFaciltiy])
            datadump.push([1 + i, 6, returnObj.VehiclePickup])
            datadump.push([1 + i, 7, returnObj.ServiceQuality])
            datadump.push([1 + i, 8, returnObj.Score])
        }

        hotCSIScore.alter('remove_row', 1, window.hotCSIScore.countRows());
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

    var data = [
        ["No", "Code", "Dealer Name", "SI", "SA", "SF", "VP", "SQ", "Total Score"]
    ],
      container = document.getElementById('wxgridcell'),
      settings1 = {
          data: data,
          colWidths: [50, 100, 400, 50, 50, 50, 50, 50, 100],
          columns: [
              {
                  //data: "No"
              },
              {
                  //data: "Nama",
              },
              {
                  //data: "Dealer"
              },
              {
                  //data: "SI"
                  type: "numeric",
                  format: "0,0.00",
                  className: "htCenter"
              },
              {
                  //data: "SA"
                  type: "numeric",
                  format: "0,0.00",
                  className: "htCenter"
              },
              {
                  //data: "SF"
                  type: "numeric",
                  format: "0,0.00",
                  className: "htCenter"
              },
              {
                  //data: "VP"
                  type: "numeric",
                  format: "0,0.00",
                  className: "htCenter"
              },
              {
                  //data: "SQ"
                  type: "numeric",
                  format: "0,0.00",
                  className: "htCenter"
              },
              {
                  //data: "Total Score"
                  type: "numeric",
                  format: "0,0.00",
                  className: "htCenter"
              }
          ],
          contextMenu: true,
          minSpareRows: 1,
          currentRowClassName: 'currentRow',
          currentColClassName: 'currentCol',
          autoWrapRow: true,
          manualColumnResize: true,
          persistentState: true,
          cells: function (row, col, prop) {
              var cellProperties = {};

              if (row < 1) {
                  this.renderer = grayRenderer;
                  cellProperties.readOnly = true;
              }

              return cellProperties;
          },
          afterChange: function (changes, source) {
              if (source !== 'edit' && source !== 'paste' && source !== 'autofill') return;

              if (changes !== null) {
                  var me = this;
                  var newvalue = changes[0][3];
                  var cRow = changes[0][0];
                  var cCol = changes[0][1];
              }

              if (source === 'paste' || source === 'autofill') {
                  var row = 1;
                  while (hotCSIScore.getDataAtCell(row, 3) !== null || hotCSIScore.getDataAtCell(row, 4) !== null || hotCSIScore.getDataAtCell(row, 5) !== null || hotCSIScore.getDataAtCell(row, 6) !== null || hotCSIScore.getDataAtCell(row, 7) !== null) {
                      var total = 0;
                      for (var i = 3; i < 8; i++) {
                          if (hotCSIScore.getDataAtCell(row, i) !== "") {
                              total += hotCSIScore.getDataAtCell(row, i);
                          }
                      }
                      total = total / 5;
                      hotCSIScore.setDataAtCell(row, 8, total);
                      row++;
                  }
                  return;
              }

              if (cCol > 0 && cCol < 8) {
                  var total = 0;
                  for (var i = 3; i < 8; i++) {
                      if (hotCSIScore.getDataAtCell(cRow, i) !== "") {
                          total += hotCSIScore.getDataAtCell(cRow, i);
                      }
                  }
                  total = total / 5;
                  hotCSIScore.setDataAtCell(cRow, 8, total);
              }
              this.renderer = codeRenderer;
          }
      };

    hotCSIScore = new Handsontable(container, settings1);
    hotCSIScore.render();

    //$('.ht_master').css('position', 'absolute');
    $('.wtHolder').css('margin-top', '0');
    return hotCSIScore;
}

function rowToJson(data) {
    var returnObj = new Object();
    returnObj.ServiceCode = data[1];
    returnObj.ServiceInitiation = data[3];
    returnObj.ServiceAdvisor = data[4];
    returnObj.ServiceFaciltiy = data[5];
    returnObj.VehiclePickup = data[6];
    returnObj.ServiceQuality = data[7];
    returnObj.Score = data[8];
    return returnObj;
}

function exportXls() {
    var params = $("#pnlPeriod").serializeObject();
    params.Department = "SALES";

    $.ajax({
        async: true,
        type: "POST",
        data: params,
        url: "wh.api/report/InqEmployees?type=mutation",
        success: function (data) {
            if (data.message == "") {
                location.href = 'wh.api/report/DownloadExcelFile?key=' + data.value + '&filename=Personal Mutation Employee';
            } else {
                sdms.info(data.message, "Error");
            }
        }
    });

}