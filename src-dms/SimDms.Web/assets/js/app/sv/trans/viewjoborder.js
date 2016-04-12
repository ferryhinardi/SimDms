var x = 0;
var arr = [];

$(document).ready(function () {
    var options = {
        title: "Simulasi harga SPK",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", icon: "icon-file" }
        ],
        panels: [
            {
                name: "pnlJobOrderView",
                title: "Simulasi Harga SPK",
                items: [
                    {
                        text: "Basic Model",
                        type: "controls",
                        items: [
                            { name: "RefferenceCode", text: "Basic Model", cls: "span2", type: "popup", readonly: true },
                            { name: "Description", cls: "span6", readonly: true },
                        ]
                    },
                    {
                        text: "Task Type",
                        type: "controls",
                        items: [
                            { name: "TaskType", text: "Task Type", cls: "span2", type: "popup", readonly: true },
                            { name: "Description1", cls: "span6", readonly: true },
                        ]
                    },
                ]
            },
            {
                name: "pnlJobOrder",
                items: [
                    {
                        name: 'JobType', text: 'Job Type', cls: 'span4', type: 'select',
                        items: [
                            { value: 'W', text: 'W - CLAIM' },
                            { value: 'F', text: 'F - PDI & FSC' },
                            { value: 'I', text: 'I - INTERNAL' },
                            { value: 'A', text: 'A - ASURANSI' },
                            { value: 'C', text: 'C - PELANGGAN' }
                        ]
                    },
                    {
                        name: 'ItemType', text: 'Item Type', cls: 'span4', type: 'select',
                        items: [
                            { value: 'L', text: 'Labor (Jasa)' },
                            { value: 'P', text: 'Part / Material' }
                        ]
                    },
                    { name: "OperationNo", text: "Part No / Job Type", cls: "span4", type: "popup", readonly: true },
                    { name: "Qty", text: "Qty", cls: "span4" },
                    { name: "Price", text: "Price", cls: "span4 number-int" },
                    { name: "Total", text: "Total", cls: "span4 number-int", readonly: true },
                    {
                         type: "buttons", items: [
                             { name: "btnAdd", text: "", icon: "icon-plus" },
                             { name: "btnDelete", text: "", icon: "icon-remove" },
                         ]
                    }
                ]
            },
              {
                  title: "Preview",
                  xtype: "table",
                  tblname: "tblJobOrder",
                  columns: [
                      { name: "Id", cls:"hide"},
                      { name: "JobType", text: "Job Type"},
                      { name: "ItemType", text: "Item Type" },
                      { name: "OperationNo", text: "Part No" },
                      { name: "Qty", text: "Qty" },
                      { name: "Price", text: "Price" },
                      { name: "Total", text: "Total" }
                  ]
              },
        ],
      
    }

    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        $.post("sv.api/viewjoborder/default", function (result) {
            widget.default = result;
            widget.populate(result);
        });
    });

    $("#btnRefferenceCode").on("click", function () {
        widget.lookup.init({
            name: "BasicModel",
            title: "Basic Model List",
            source: "sv.api/grid/basicmodel",
            sortings: [[0, "asc"]],
            columns: [
                { mData: "RefferenceCode", sTitle: "Refference Code", sWidth: "110px" },
                { mData: "Description", sTitle: "Description", sWidth: "110px" },
                { mData: "IsActive", sTitle: "Is Active", sWidth: "110px" },
            ]
        });
        widget.lookup.show();
    });
        
    $("#btnTaskType").on("click", function () {

        if ($('#RefferenceCode').val() != '') {
            widget.lookup.init({
                name: "TaskType",
                title: "Task Type List",
                source: "sv.api/grid/TaskType?basicmodel=" + $('#RefferenceCode').val(),
                sortings: [[0, "asc"]],
                columns: [
                    { mData: "TaskType", sTitle: "Job Type", sWidth: "110px" },
                    { mData: "Description1", sTitle: "Description", sWidth: "110px" },
                ]
            });
            widget.lookup.show();
        }
        else {
            widget.alert('Basic Model tidak boleh kosong !');
        }
    });

    $("#btnOperationNo").on("click", function () {
        if ($('#TaskType').val() != '') {
            widget.lookup.init({
                name: "PartNo",
                title: "Part List",
                source: "sv.api/grid/PartNo?basicmodel=" + $('#RefferenceCode').val() + "&jobtype=" + $('#JobType').val(),
                sortings: [[0, "asc"]],
                columns: [
                    { mData: "OperationNo", sTitle: "Operation No", sWidth: "110px" },
                    { mData: "DescriptionTask", sTitle: "Description", sWidth: "110px" },
                    { mData: "Qty", sTitle: "Qty", sWidth: "110px" },
                    { mData: "Price", sTitle: "Price", sWidth: "110px" },
                ]
            });
            widget.lookup.show();
        }
        else
        {
            alert('Task Type tidak boleh kosong !');
        }
    });

    widget.lookup.onDblClick(function (e, data, name) {
        console.log(data);
        widget.populate($.extend({}, widget.default, data));

        if (name == 'PartNo')
        {
            $('#Total').val(data.Qty * data.Price);
        }
        widget.lookup.hide();
    });

    $('#Qty,#Price').on('change', function (e) {
        $('#Total').val($('#Qty').val() * $('#Price').val());

    });

    //$('#btnTaskType').attr('disabled', 'disabled');

    $('#btnAdd').on('click', function (e) {

        if ($('#JobType').val() == '' || $('#ItemType').val() == '') {
            alert('Job Type dan Item Type wajib pilih salah satu !')
        }
        else {
            var data = $("#pnlJobOrder").serializeObject();

            arr[x] = data;

            console.log(arr[x]);
            widget.populateTable({ selector: "#tblJobOrder", data: arr, selectable: true });

            x = x + 1;
        }
    });
    
    $('#btnDelete').on('click', function (e) {
        $('.row_selected').remove();
    });

    $('#btnNew').on('click', function (e) {
        widget.clearForm();
        widget.populateTable({ selector: "#tblJobOrder", data: null });
    });

});