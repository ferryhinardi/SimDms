var opt, hotgrid, dataCount, iHeight = 0;
var widget = new SimDms.Widget({
    title: 'Campaign Claim',
    xtype: 'panels',
    //autorender: true, 
    toolbars: [
        { action: 'doExport', text: 'Export', icon: 'fa fa-file-excel-o', name: "export" },
        { action: 'clear', text: 'Clear', icon: 'fa fa-refresh', name: "clear" },
    ],
    panels: [
        {
            name: "pnlA",
            required: true,
            items: [
                
                 {
                     name: "ProductType", text: "Product Type", type: "select", cls: "span3 full",
                     items: [
                         { value: "2W", text: "2W" },
                         { value: "4W", text: "4W" }
                     ]
                 },
                 {
                    text: "Kode Complain",
                    type: "controls",
                    items: [
                        { name: "ComplainCode", text: "Kode Complain", type: "popup", cls: "span2", action: "LkpKodeComplain", readonly: true },
                        { name: "ComplainName", text: "Nama Complain", cls: "span6", readonly: true, maxlength: 100 },
                    ]
                },
                {
                    text: "Kode Defect",
                    type: "controls",
                    items: [
                        { name: "DefectCode", text: "Kode Defect", type: "popup", cls: "span2", action: "LkpKodeDefect", readonly: true },
                        { name: "DefectName", text: "Nama Defect", cls: "span6", readonly: true, maxlength: 100 },
                    ]
                },
                {
                    text: "Kode / No Rangka",
                    type: "controls",
                    items: [
                        { name: "ChassisCode", text: "Rangka", type: "popup", cls: "span2", action: "LkpRangka", readonly: true },
                        { name: "ChassisStartNo", cls: "span2 number-int", text: "No Rangka Awal", readonly: true },
                        { name: "ChassisEndNo", cls: "span2 number-int", text: "No Rangka Akhir", readonly: true },
                    ]
                },
                {   
                    text: "Jenis Pekerjaan",
                    type: "controls",
                    items: [
                        { name: "OperationNo", text: "Jenis Pekerjaan", cls: "span2", type: "popup", readOnly: true, action: "OperationNo", readonly: true },
                        { name: "OperationName", text: "Nama Pekerjaan", cls: "span6", readonly: true, maxlength: 100 },
                    ]
                },
                { name: "Description", text: "Keterangan", placeholder: "Keterangan", cls: "span8", maxlength: 150 },
            ]
        },
        {
            name: "wxgrid",
            xtype: 'wxtable'
        }
    ],


    onInit: function (wgt) {
        wgt.call('initial', wgt);
    },

    initial: function (wgt) {
        widget = widget || wgt;
        //$("[name=export], [name=clear]").hide()
        //$("#wxgrid").hide();
        widget.call('clear');
    },

    LkpKodeComplain: function () {
        sdms.lookup({
            title: 'Lookup Complain Code',
            url: 'wh.api/Campaign/ComplainCampaign?pType=' + $('[name=ProductType]').val(),
            sort: [{ field: 'ComplainCode', dir: 'desc' }],
            fields: [
                    { name: 'ComplainCode', text: 'Kode Complain', width: 150 },
                    { name: 'Description', text: 'Keterangan', width: 220 },
                    { name: 'DescriptionEng', text: 'Keterangan (English)', width: 220 },
            ],
            dblclick: 'loadComplain',
            onclick: 'loadComplain'
        });
    },

    loadComplain: function (row) {
        widget.populate({ ComplainCode: row.ComplainCode, ComplainName: row.Description })
    },

    LkpKodeDefect: function () {
        sdms.lookup({
            title: 'Lookup Defect Data',
            url: 'wh.api/Campaign/DefectCampaign?pType=' + $('[name=ProductType]').val() + '&cCode=' + $('[name=ComplainCode]').val(),
            sort: [{ field: 'DecfectCode', dir: 'desc' }],
            fields: [
                    { name: 'DefectCode', text: 'Kode Defect', width: 150 },
                    { name: 'Description', text: 'Keterangan', width: 220 },
                    { name: 'DescriptionEng', text: 'Keterangan (English)', width: 220 },
            ],
            dblclick: 'loadDefect',
            onclick: 'loadDefect'
         });
    },

    loadDefect: function (row) {
        widget.populate({ DefectCode: row.DefectCode, DefectName: row.Description })
    },

    LkpRangka: function () {
        sdms.lookup({
            title: 'Lookup Chassis Code',
            url: 'wh.api/Campaign/ChassisCampaign?pType=' + $('[name=ProductType]').val() + '&cCode=' + $('[name=ComplainCode]').val() + '&dCode=' + $('[name=DefectCode]').val(),
            sort: [{ field: 'ChassisCode', dir: 'desc' }],
            fields: [
                    { name: 'ChassisCode', text: 'Kode Rangka', width: 150 },
                    { name: 'ChassisStartNo', text: 'No Rangka Dari', width: 220 },
                    { name: 'ChassisEndNo', text: 'No Rangka Sampai', width: 220 },
            ],
            dblclick: 'loadChassis',
            onclick: 'loadChassis'
        });
    },

    loadChassis: function (row) {
        widget.populate({ ChassisCode: row.ChassisCode, ChassisStartNo: row.ChassisStartNo, ChassisEndNo: row.ChassisEndNo })
    },

    OperationNo: function () {
            sdms.lookup({
                title: 'Lookup Operation Data',
                url: 'wh.api/Campaign/OperationCampaign?pType=' + $('[name=ProductType]').val() + '&cCode=' + $('[name=ComplainCode]').val() + '&dCode=' + $('[name=DefectCode]').val() + '&chassisCode=' + $('[name=ChassisCode]').val() + '&chassisStartNo=' + $('[name=ChassisStartNo]').val() + '&chassisEndNo=' + $('[name=ChassisEndNo]').val(),
                sort: [{ field: 'OperationNo', dir: 'desc' }],
                fields: [
                    { name: 'OperationNo', text: 'No Pekerjaan' },
                    { name: 'OperationName', text: 'Nama Pekerjaan'},
                    { name: 'Description', text: 'Nama Campaign' },
                ],
                dblclick: 'loadOperation',
                onclick: 'loadOperation'
            })
    },

    loadOperation: function (row) {
            widget.populate({ OperationNo: row.OperationNo, OperationName: row.OperationName, Description: row.Description })
    },

    doExport: function () {
            var record = widget.serializeObject();

            if (!record.ProductType) { sdms.info({ type: 'error', text: 'Product Type tidak boleh kosong' }); return; }

            record.ChassisStartNo = +record.ChassisStartNo.replace(/[^\d\.-]/g, '');
            record.ChassisEndNo = +record.ChassisEndNo.replace(/[^\d\.-]/g, '');

            var params = widget.serializeObject('pnlA');
            widget.post('wh.api/Campaign/doExport', params, function (result) {
                if (result.message == "") {
                    location.href = 'wh.api/Campaign/DownloadExcelFile?key=' + result.value + '&fileName=CampaignClaim';
                }
            })
    },


    clear: function () {
        widget.clear()
        //$('name=ProductType').val('')
    }
});


