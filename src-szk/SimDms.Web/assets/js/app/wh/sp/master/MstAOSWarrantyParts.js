$(document).ready(function () {
    var options = {
        title: "Master AOS Warranty Parts",
        xtype: "panels",
        toolbars: [
            { action: "Refresh", text: "New", icon: "fa fa-refresh" },
            { action: 'Query', text: 'Query', icon: 'fa fa-bolt' },
            { action: 'Save', text: 'Save', icon: 'fa fa-save' },
            { action: 'Publish', text: 'Publish', icon: 'fa fa-push'}
        ],
        panels: [
            {
                name: "pnlMain",
                type: "panels",
                items: [
                    {
                        text: "Part No",
                        type: "controls",
                        items: [
                            { name: "PartNo", cls: "span2", placeHolder: "Part No", readonly: true, type: "popup" },
                            { name: "PartName", cls: "span4", placeHolder: "Part Name", readonly: true },
                            {
                                type: "buttons",
                                items: [
                                    { name: "btnSearch", text: " Search", icon: "fa fa-search" },
                                ]
                            },
                        ]
                    },
                ]
            },
            {
                name: 'wxgrid',
                xtype: 'wxtable',
                ctrlstyle: ' style="padding-left:40px;"'
            },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case 'Refresh':
                    window.masterAOSTable.destroy();
                    $('#PartNo, #PartName').val("");
                    gridRender(null);
                    break;
                case 'Query':
                    window.masterAOSTable.destroy();
                    getWarrantyParts(null);
                    break;
                case 'Save':
                    save();
                    break;
                case 'Publish':
                    publish();
                    break;
                default:
                    break;
            }
        },
    }

    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        $('#PartNo, #PartName').val("");
        gridRender(null);
    });

    $('#btnPartNo').on('click', function (e) {
        widget.lookup.init({
            name: "LookupPartNo",
            title: "Lookup PartNo",
            source: "wh.api/lookupgrid/WarrantyParts",
            columns: [
                { mData: 'PartNo', sTitle: 'Part No', sWidth: 220 },
                { mData: 'PartName', sTitle: 'Part Name', sWidth: 220 },
            ],
            additionalParams: [
                { name: "PartNo", element: "PartNo", type: "select" }
        ]
        });
        widget.lookup.show();
    });

    widget.lookup.onDblClick(function (e, data, name) {
        if (1 == 1) {
            widget.populate(data);
        }
        widget.lookup.hide();
    });

    $('#btnSearch').on('click', function (e) {
        window.masterAOSTable.destroy();
        getWarrantyParts($("#PartNo").val());
    });

    function save() {
        var con = confirm("Save data?");
        if (con == true) {
            sdms.showAjaxLoad();
            var data = window.masterAOSTable.getData().slice(0);

            if (data.length === 1) return;
            var listData = [];

            for (var i = 0; i < data.length; i++) {
                var dx = rowToJson(data[i]);
                listData.push(dx);
            }
            var postData = {
                Data: JSON.stringify(listData)
            }

            $.ajax({
                type: "POST",
                url: 'wh.api/mstaoswarrantyparts/save',
                dataType: 'json',
                data: postData,
                success: function (e) {
                    if (e.success) {
                        //window.masterAOSTable.alter('remove_row', 0, window.masterAOSTable.countRows());
                        //gridRender()
                        window.masterAOSTable.setDataAtCell(jsonToRow(e.data), null, null, 'loadData');
                        widget.showNotification('Data Saved.');
                        sdms.hideAjaxLoad();
                    } else {
                        sdms.hideAjaxLoad();
                    }
                }
            });
        }
        else
        {
            return;
        }
    }

    function getWarrantyParts(partNo) {
        widget.post('wh.api/mstaoswarrantyparts/getWarrantyParts', { partNo: partNo }, function (e) {
            gridRender();
            if (e.length > 0) {
                window.masterAOSTable.setDataAtCell(jsonToRow(e), null, null, 'loadData');
            }
            else
            {
                widget.showNotification("tidak ada data yang ditampilkan");
            }
        })
    }

    function gridRender() {
        var lastRow = 0;
        var data = null;
        var container = document.getElementById('wxgrid'),
            settings = {
                data: data,
                colHeaders: ["No", "Part No", "Warranty Parts", "Part Name", "Status", ""],
                colWidths: [40, 150, 150, 250, 200],
                search: true,
                allowInsertRow: true,
                contextMenu: true,
                autoWrapRow: true,
                persistentState: true,
                minSpareRows: 1,
                columns: [
                    { readOnly: true, className: "htMiddle" },
                    { readOnly: false, className: "htMiddle" },
                    { type: 'checkbox', className: "htMiddle" },
                    { readOnly: true, className: "htMiddle" },
                    { readOnly: true, className: "htMiddle" },
                ],
                cells: function (row, col, prop) {
                    //console.log(row);
                    //console.log(col);
                    //console.log(prop);

                },
                afterChange: function (changes, source) {
                    var table = this;
                    if (source == 'edit') {
                        sdms.showAjaxLoad();
                        var rowIndex = changes[0][0];
                        var columnIndex = changes[0][1];
                        var oldValue = changes[0][2];
                        var newValue = changes[0][3];

                        if (columnIndex == 1) {
                            if (oldValue != newValue) {
                                $.post('wh.api/MstAOSWarrantyParts/getPartName', { partNo: newValue }, function (e) {
                                    table.setDataAtCell(rowIndex, 0, (rowIndex + 1));
                                    table.setDataAtCell(rowIndex, 3, e.partName);
                                    table.setDataAtCell(rowIndex, 4, e.status);
                                    sdms.hideAjaxLoad();
                                });
                            }
                        }

                        if (columnIndex == 2) {
                            sdms.hideAjaxLoad();
                        }

                        sdms.hideAjaxLoad();
                    }
                    else if (source == 'paste')
                    {
                        if (!changes) {
                            return;
                        }
                        $.each(changes, function (index, element) {
                            var change = element;
                            var rowIndex = change[0];
                            var columnIndex = change[1];

                            var oldValue = change[2];
                            var newValue = change[3];

                            if (columnIndex == 2)
                            {
                                if (newValue.toString() != "TRUE")
                                {
                                    newValue = newValue == 1 ? true : newValue == 0 ? false : false;
                                    table.setDataAtCell(rowIndex, 2, newValue);
                                }
                                else{
                                    table.setDataAtCell(rowIndex, 2, true);
                                }
                                sdms.hideAjaxLoad();
                            }

                            if (columnIndex == 1) {
                                if (oldValue != newValue) {
                                    $.post('wh.api/MstAOSWarrantyParts/getPartName', { partNo: newValue }, function (e) {
                                        table.setDataAtCell(rowIndex, 0, (rowIndex + 1));
                                        table.setDataAtCell(rowIndex, 3, e.partName);
                                        table.setDataAtCell(rowIndex, 4, e.status);

                                        sdms.hideAjaxLoad();
                                    });
                                }
                            }
                            sdms.hideAjaxLoad();
                        });
                    }
                },
            };
        window.masterAOSTable = new Handsontable(container, settings);
        masterAOSTable.render();
        $('#wxgrid .ht_master .wtHolder, .wtHider').css({ "padding-left": "0", "overflow": "hidden", "height": "" });
        $('.ht_clone_top').remove();
    }

    function publish() {
        var pub = confirm("publish?");
        if (pub == true) {
            sdms.showAjaxLoad();
            widget.post('wh.api/mstaoswarrantyparts/publish', function (e) {
                if (e.success) {
                    sdms.hideAjaxLoad();
                    widget.showNotification('Data published.');
                }
            })
        }
        else { return;}
    }
});

var rowToJson = function (data) {
    var returnObj = new Object();
    returnObj.No = data[0];
    returnObj.PartNo = data[1];
    returnObj.isWarrantyParts = data[2];
    returnObj.PartName = data[3];
    returnObj.Status = data[4];
    return returnObj;
}

var jsonToRow = function (returnObj) {
    var data = []
    for (var i = 0; i < returnObj.length; i++) {
        data.push([i , 0, returnObj[i].No]);
        data.push([i , 1, returnObj[i].PartNo]);
        data.push([i , 2, returnObj[i].isWarrantyParts]);
        data.push([i , 3, returnObj[i].PartName]);
        data.push([i , 4, returnObj[i].Status]);
    }
    
    return data;
    //return function () {
    //    var page = parseInt(window.location.hash.replace('#', ''), 10) || 1,
    //      limit = 6,
    //      row = (page - 1) * limit,
    //      count = page * limit,
    //      part = [];

    //    for (; row < count; row++) {
    //        part.push(data[row]);
    //    }
    //    return part;
    //}
};



