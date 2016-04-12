var opt, hotgrid, dataCount, iHeight = 0;
var widget = new SimDms.Widget({
    title: 'Master Campaign',
    xtype: 'panels',
    //autorender: true, 
    toolbars: [
        { action: 'doExport', text: 'Export', icon: 'fa fa-arrow-circle-o-up', name: "export" },
        { action: 'cleargrid', text: 'Clear', icon: 'fa fa-refresh', name: "clear" },
        { action: 'add', text: 'New', icon: 'fa fa-file', name: "add" },
        { action: 'save', text: 'Save', icon: 'fa fa-save', name: "save" },
        { action: 'browse', text: 'Browse', icon: 'fa fa-search', name: "browse" },
        { action: 'print', text: 'Print', icon: 'fa fa-print', name: "print" },
        { action: 'copypaste', text: 'Copy Paste', icon: 'fa-file-excel-o', name: "copypaste" },
        { action: 'formentry', text: 'Form Entry', name: "formentry" }
    ],
    panels: [
        {
            name: "pnlFormEntry",
            required: true,
            items: [
                
                 {
                     name: "ProductType", text: "Product Type", type: "select", cls: "span3 full",
                     items: [
                         { value: "2W", text: "Roda Dua (2W)" },
                         { value: "4W", text: "Roda Empat (4W)" }
                     ]
                 },

 
                {
                    text: "Kode Complain",
                    type: "controls",
                    required: true,
                    items: [
                        { name: "ComplainCode", text: "Kode Complain", type: "popup", cls: "span2", action: "LkpKodeComplain", readonly: true },
                        { name: "ComplainName", text: "Nama Complain", cls: "span6", readonly: true, maxlength: 100 },
                    ]
                },
                {
                    text: "Kode Defect",
                    type: "controls",
                    required: true,
                    items: [
                        { name: "DefectCode", text: "Kode Defect", type: "popup", cls: "span2", action: "LkpKodeDefect", readonly: true },
                        { name: "DefectName", text: "Nama Defect", cls: "span6", readonly: true, maxlength: 100 },
                    ]
                },
                {
                    text: "Kode / No Rangka",
                    type: "controls",
                    required: true,
                    items: [
                        {
                            name: "ChassisCode",
                            cls: "span4",
                            text: "Rangka",
                            required: true, validasi: "required", maxlength: 15
                        },
                        {
                            name: "ChassisStartNo",
                            cls: "span2 number-int",
                            text: "No Rangka Awal",
                            required: true, validasi: "required", maxlength: 10
                        },
                        {
                            name: "ChassisEndNo",
                            cls: "span2 number-int",
                            text: "No Rangka Akhir",
                            required: true, validasi: "required", maxlength: 10
                        },
                    ]
                },
                {   
                    text: "Jenis Pekerjaan",
                    type: "controls",
                    required: true,
                    items: [
                        { name: "OperationNo", text: "Jenis Pekerjaan", cls: "span2", type: "popup", readOnly: true, action: "OperationNo", readonly: true },
                        { name: "OperationName", text: "Nama Pekerjaan", cls: "span6", readonly: true, maxlength: 100 },
                    ]
                },
                { name: "Description", text: "Keterangan", placeholder: "Keterangan", cls: "span8", required: true, maxlength: 100 },
                { name: "CloseDate", text: "Tgl. Akhir", cls: "span3", type: "datepicker" },
                { name: "IsActive", text: "Status", cls: "span3", type: "switch", float: "left" },
            ]
        },
        {
            name: "pnlSend",
            items:[
                { name: "SendTo", text: "Kirim Ke", type: "select", cls: "span3 full",
                    items: [
                            { value: "2W", text: "Roda Dua (2W)" },
                            { value: "4W", text: "Roda Empat (4W)" }
                    ]
                },
                {
                    type: "buttons", cls: "span4 left", items: [
                         { name: "btnSend", text: "Send", cls: "btn-small", action: "doSend" },
                         { name: "btnCancel", text: "Cancel", cls: "btn-small", action: "doCancel" },
                     ]
                },
            ]
        },
        {
            name: "wxgrid",
            xtype: 'wxtable'
        },
        
    ],


    onInit: function (wgt) {
        wgt.call('initial', wgt);
    },

    initial: function (wgt) {
        widget = widget || wgt;
        $("[name=print], [name=formentry], [name=save], [name=clear]").hide()
        $("[name=add]").show()
        $("#IsActiveY").prop('checked', true)
        renderGrid();
        $("#pnlSend").hide();
        $("#wxgrid").hide();
        widget.call('add');
    },

    LkpKodeComplain: function () {
        sdms.lookup({
            title: 'Lookup Data',
            url: 'wh.api/Campaign/ComplainCodeList',
            sort: [{ field: 'RefferenceCode', dir: 'desc' }],
            fields: [
                    { name: 'RefferenceCode', text: 'Kode Referensi', width: 150 },
                    { name: 'Description', text: 'Keterangan', width: 220 },
                    { name: 'DescriptionEng', text: 'Keterangan (English)', width: 220 },
            ],
            dblclick: 'loadComplain',
            onclick: 'loadComplain'
        });
    },

    loadComplain: function (row) {
        widget.populate({ ComplainCode: row.RefferenceCode, ComplainName: row.Description })
    },

    LkpKodeDefect: function () {
        sdms.lookup({
            title: 'Lookup Data',
            url: 'wh.api/Campaign/DefectCodeList',
            sort: [{ field: 'RefferenceCode', dir: 'desc' }],
            fields: [
                    { name: 'RefferenceCode', text: 'Kode Referensi', width: 150 },
                    { name: 'Description', text: 'Keterangan', width: 220 },
                    { name: 'DescriptionEng', text: 'Keterangan (English)', width: 220 },
            ],
            dblclick: 'loadDefect',
            onclick: 'loadDefect'
         });
    },

    loadDefect: function (row) {
        widget.populate({ DefectCode: row.RefferenceCode, DefectName: row.Description })
    },

    browse: function () {
            sdms.lookup({
                title: 'Lookup Data',
                url: 'wh.api/Campaign/Browse',
                sort: [{ field: 'CompanyCode', dir: 'desc' }],
                fields: [
                    { name: 'ComplainCode', text: 'Kode Complain' },
                    //{ name: 'ComplainName', text: 'Nama Complain' },
                    { name: 'DefectCode', text: 'Kode Defect' },
                    //{ name: 'DefectName', text: 'Nama Defect' },
                    { name: 'ChassisCode', text: 'Kode Rangka' },
                    { name: 'ChassisStartNo', text: 'No Chassis Awal' },
                    { name: 'ChassisEndNo', text: 'No Chassis Akhir' },
                    { name: 'OperationNo', text: 'No Pekerjaan' },
                    //{ name: 'OperationName', text: 'Name Pekerjaan', hide: true },
                    { name: 'CloseDate', text: 'Tanggal Akhir', type: 'date' },
                    { name: 'Description', text: 'Keterangan', width: 400 },
                    { name: 'IsActive', text: 'Aktif' },
                ],
                dblclick: 'loadData',
                onclick: 'loadData'
            })
    },
    loadData: function (row) {
            widget.clear();
            widget.populate(row);
            $("[name=add]").show()

    },

    OperationNo: function () {
            sdms.lookup({
                title: 'Lookup Data',
                url: 'wh.api/Campaign/OperationList',
                sort: [{ field: 'OperationNo', dir: 'desc' }],
                fields: [
                    { name: 'OperationNo', text: 'No Pekerjaan' },
                    { name: 'Description', text: 'Keterangan' },
                    { name: 'IsActive', text: 'Is Active'}
                ],
                dblclick: 'loadOperation',
                onclick: 'loadOperation'
            })
    },

    loadOperation: function (row) {
            widget.populate({ OperationNo: row.OperationNo, OperationName: row.Description })
    },

    add: function () {
            widget.clear()
            $("[name=save]").show()
            $("[name=add]").hide()
            $("[name=print]").hide()
            $("[name=formentry]").hide()
            $("#IsActiveY").prop('checked', true)
            opt = 1;
    },

    save: function () {
        if (opt == 1) {
            var record = widget.serializeObject();

            if (!record.ProductType) { sdms.info({ type: 'error', text: 'Product Type tidak boleh kosong' }); return; }
            if (!record.ComplainCode) { sdms.info({ type: 'error', text: 'Complain Code tidak boleh kosong' }); return; }
            if (!record.DefectCode) { sdms.info({ type: 'error', text: 'Defect Code tidak boleh kosong' }); return; }
            if (!record.ChassisCode) { sdms.info({ type: 'error', text: 'Kode Rangka tidak boleh kosong' }); return; }
            if (!record.ChassisStartNo) { sdms.info({ type: 'error', text: 'No Rangka Awal tidak boleh kosong' }); return; }
            if (!record.ChassisEndNo) { sdms.info({ type: 'error', text: 'No Rangka Akhir tidak boleh kosong' }); return; }
            if (!record.OperationNo) { sdms.info({ type: 'error', text: 'No Pekerjaan tidak boleh kosong' }); return; }
            if (!record.Description) { sdms.info({ type: 'error', text: 'Description tidak boleh kosong' }); return; }

            $("[name=save]").attr('disabled', 'disabled');
            record.ChassisStartNo = +record.ChassisStartNo.replace(/[^\d\.-]/g, '');
            record.ChassisEndNo = +record.ChassisEndNo.replace(/[^\d\.-]/g, '');

            //console.log(JSON.stringify(record));
            sdms.showAjaxLoad();
            $.ajax({
                async: false,
                type: "POST",
                data: { Model: "[" + JSON.stringify(record) + "]" },
                url: 'wh.api/Campaign/campaignsave',
                success: function (data) {
                    if (data.success) {
                        sdms.notify({ type: 'success', text: 'Data Saved' });
                        $("[name='save']").removeAttr('disabled');
                    } else {
                        sdms.notify({ type: 'error', text: data.message });
                        $("[name='save']").removeAttr('disabled');
                    }
                    sdms.hideAjaxLoad();
                }
            });
        } else {
            var rmax = (hotgrid.getData().length)-2;
            if (!checkNull())
            {
                sdms.notify({ type: 'error', text: 'Masih terdapat kolom yang kosong!' })
            } else {
                var xdata = hotgrid.getData(1, 0, rmax, 9);
                console.log(xdata);

                var listData = [];

                for (var i = 0; i < rmax; i++) {
                    listData.push(rowToJson(xdata[i]));
                }

                //console.log(JSON.stringify(listData));
                $("[name=save]").attr('disabled', 'disabled');
                sdms.showAjaxLoad();
                $.ajax({
                    async: false,
                    type: "POST",
                    data: { Data: JSON.stringify(listData) },
                    url: 'wh.api/Campaign/bulkSave',
                    success: function (data) {
                        if (data.success)
                        {
                            sdms.notify({ type: 'success', text: 'Data Saved' });
                            $("[name='save']").removeAttr('disabled');
                        } else {
                            sdms.notify({ type: 'error', text: data.message });
                            $("[name='save']").removeAttr('disabled');
                        }
                        sdms.hideAjaxLoad();
                    }
                });
            }
        }
    },

    copypaste: function () {
            $("#pnlFormEntry").hide()
            $("#wxgrid").show()
            $("[name=formentry]").show()
            $("[name=copypaste]").hide()
            $("[name=browse]").hide()
            $("[name=export]").removeAttr('disabled')
            $("[name=clear]").show()
            $("#pnlSend").hide();
            opt = 2;
    },

    formentry: function () {
            $("#pnlFormEntry").show()
            $("#wxgrid").hide()
            $("[name=formentry]").hide()
            $("[name=copypaste]").show()
            $("[name=browse]").show();
            $("[name=export]").removeAttr('disabled')
            $("[name=clear]").hide()
            $("#pnlSend").hide();
            opt = 1;
    },

    doExport: function () {
        $("[name=export]").hide()
        $("#pnlSend").show();
        $("#pnlFormEntry").hide()
        $("#wxgrid").hide()
    },

    doSend: function () {
        if ($("[name=SendTo]").val() == "")
        {
            return sdms.notify({ type: 'error', text: "Tujuan dikirim masih kosong!" })
        }
        sdms.showAjaxLoad();
        $.ajax({
            async: false,
            type: "POST",
            //data: { },
            url: 'wh.api/Campaign/doSend?pType=' + $("[name=SendTo]").val(),
            success: function (data) {
                if (data.success) {
                    sdms.notify({ type: 'success', text: data.message });
                    $("[name='save']").removeAttr('disabled');
                } else {
                    sdms.notify({ type: 'error', text: data.message });
                    $("[name='save']").removeAttr('disabled');
                }
                sdms.hideAjaxLoad();
                $("[name=export]").removeAttr('disabled');
            }
        });
        $("[name=export]").show()
    },

    doCancel: function () {
        if (opt == 1) {
            widget.call('formentry')
        } else {
            widget.call('copypaste')
        }
        $("[name=export]").show();
    },

    cleargrid: function () {
        hotgrid.destroy();
        renderGrid();
    }
});

    function renderGrid() {
        var hdrRender = function (instance, td, row, col, prop, value, cellProperties) {
            Handsontable.renderers.TextRenderer.apply(this, arguments);
            td.style.backgroundColor = '#C5D9F1';
            td.style.fontSize = "12px";
            td.style.textAlign = 'center';
        };

        var ftrRender = function (instance, td, row, col, prop, value, cellProperties) {
            Handsontable.renderers.TextRenderer.apply(this, arguments);
            td.style.backgroundColor = '#EAF1DD';
            td.style.fontSize = "12px";
        };
        var dataRender = function (instance, td, row, col, prop, value, cellProperties) {
            Handsontable.renderers.TextRenderer.apply(this, arguments);
            td.style.backgroundColor = '#FFFFFF';
            td.style.fontSize = "12px";
        };
        var data = [
                        ["PRODUCT TYPE", "COMPLAIN CODE", "DEFECT CODE", "CHASSIS CODE", "CHASSIS START NO", "CHASSIS END NO", "OPERATION NO", "CLOSE DATE", "DESCRIPTION", "ACTIVE"],
                        ["", "", "", "", "", "", "", "", "", ""]
        ],
        container = document.getElementById('wxgrid'),
        settings = {
            data: data,
            contextMenu: true,
            minSpareRows: 1,
            autoWrapRow: true,
            manualColumnResize: true,
            rowHeights: [24, 24, 24],

            colWidths: [70, 70, 70, 110, 80, 80, 70, 90, 340, 50],
            columns: [
                    {}, {}, {}, {}, {}, {}, {}, {}, {}, {},
                    { className: "htContextMenu" }
            ],
            cells: function (row, col, prop) {
                var cellProperties = {};
                if (row < 1) {
                    this.renderer = hdrRender;
                    cellProperties.readOnly = true;
                }
                return cellProperties;
            },
        };

        hotgrid = new Handsontable(container, settings);
        hotgrid.render();
        //$(".wtHolder").css("top", "50px");
        //$(".wtHider").css("overflow", "scroll")
    }

    function checkNull()
    {
        var imax = (hotgrid.getData().length) - 2;
        console.log(imax);
        for (var i = 1; i < imax + 1; i++) {
            for (j = 0; j < 10; j++) {
                if (hotgrid.getDataAtCell(i, j) == "") {
                    return false;
                }
            }
        }
        return true;
    }

    function rowToJson(data)
    {
        var returnObj = new Object();
        returnObj.ProductType = data[0];
        returnObj.ComplainCode = data[1];
        returnObj.DefectCode = data[2];
        returnObj.ChassisCode = data[3];
        returnObj.ChassisStartNo = data[4];
        returnObj.ChassisEndNo = data[5];
        returnObj.OperationNo = data[6];
        returnObj.ClosedDate = moment(data[7], 'DD-MM-YYYY').format('YYYY-MM-DD');
        returnObj.Description = data[8];
        returnObj.IsActive = data[9];
        return returnObj;
    }


