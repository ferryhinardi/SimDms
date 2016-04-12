$(document).ready(function () {

    window.currentpassword = "";

    var options = {
        title: "Management Publish SQL",
        xtype: "grid-form",
        urlList: "util/grid/tasks",
        toolbars: [
            { name: "btnCreate", text: "Create", icon: "fa fa-file" },
            { name: "btnView", text: "View", icon: "fa fa-file", cls: "hide" },
            { name: "btnSave", text: "Save", icon: "fa fa-save", cls: "hide" },
            { name: "btnCancel", text: "Cancel", icon: "fa fa-undo", cls: "hide" },
            { name: "btnTest", text: "Test Query", cls: "hide" },
        ],
        items: [
            { name: "TaskNo", maxlength: 30, text: "Task No", required: true, cls: "span3" },
            { name: "TaskName", type: "text", maxlength: 50, text: "Task Name", required: true, cls: "span3" },
            { name: "DealerCode", maxlength: 250, text: "Dealer  Code", cls: "span2", required: true, type: "text", float: "left" },
            { name: "SQL", type: "textarea", text: "", cls: "span8 full", float: "left" },
            { name: "mytable", type: "div", text: "", cls: "span8 full", float: "left" },
            { name: "myGrid", type: "div", text: "", cls: "span8 full", float: "left" },
        ],
        columns: [
            { mData: "TaskNo", sTitle: "Task No", sWidth: "110px" },
            { mData: "TaskName", sTitle: "Task Name"  },
            { mData: "DealerCode", sTitle: "Dealer Code"  },
            { mData: "FileName", sTitle: "File Name", sWidth: "200px" },
            { mData: "Status", sTitle: "Status", sWidth: "100px" },
            { mData: "UserId", sTitle: "Created By", sWidth: "180px" },
            { mData: "CreatedDate", sTitle: "Created Date", sWidth: "180px" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {

        //widget.setSelect([
        //    { name: "DealerCode", url: "util/user/dealerlist" },
        //    { name: "RoleId", url: "util/role/list" }
        //]);

        //$.post("util/user/default", function (result) {
        //    widget.default = result;
        //    widget.populate(result);
        //});

        //var data = ["6002401", "6002401001", "6006400001A", "6006400001B", "6006400001C", "6006406", "6006408", "6006410", "6007402", "6014401", "6015401", "6019401", "6021406", "6023401", "6026401", "6031401", "6039401", "6039401002", "6039401003", "6039401004", "6040401", "6045401", "6051401", "6052401", "6055201", "6058401", "6060401", "6062401", "6063401", "6071201", "6071401", "6074201", "6074401", "6078401", "6079401", "6080401", "6081401", "6083401", "6083401002", "6083401100", "6088401", "6089401", "6092401", "6092401100", "6093401", "6111201", "6113204", "6114201", "6115202", "6115204", "6115204001ACH", "6115204001BDG", "6115204001BLI", "6115204001HLD", "6115204001JKT", "6115204001KDR", "6115204001LPG", "6115204001MDN", "6115204001PLB", "6115204001SBY", "6115204001SLO", "6115204001SMR", "6115206", "6115207", "6115208", "6115209", "6118201", "6118201012", "6120201", "6130201", "6145201", "6145202", "6145203", "6145205", "6156401", "6156401000", "6158401", "6159401", "6159401000", "6162401", "6340401", "6346401", "6354401", "6419401", "6432401", "6435201", "6435202", "6435203", "6437201", "6468401", "6469401", "6477401", "6482401", "6489401", "6491401", "6500401", "6506201", "6515201", "6525201", "6545201", "6548201", "6552201", "6554201", "6558201", "6583401", "6585201", "6623401", "6630401", "6641401", "6641401DEMO"];
        
        ////create AutoComplete UI component
        //$("#DealerCode").kendoAutoComplete({
        //    dataSource: data,
        //    filter: "startswith"
        //});

    });



    $("#btnTest").on("click", function () {            
        var _sql = window.sqlEditor.getValue();
        var params = { to: $("#DealerCode").val(), type: 'sqlRaw', command: _sql };
        if (window.currentpassword == "") {
            window.currentpassword = prompt("Please enter a password to continue");
        }

        if (window.currentpassword == "script4dealer") {
            socketClient.emit('pm', params);
        } else {
            alert("Access Denied")
            window.currentpassword =""
        }
        
    });

    socketClient.on('result', function (from, result, err, info) {

        console.log('result')
        console.log(result)

        if (result != null)
        {
            var cols = [];

            _.map(result.meta, function (o) {
                cols.push(o.name);
            })

            console.log(cols)
            console.log(result.rows)

            $("#mytable").html('');
             
            window.hot = $("#mytable").handsontable({
                data: result.rows,
                colHeaders: cols,
                minSpareRows: 1,
                rowHeaders: true,
                colHeaders: true,
                contextMenu: true
            });
          
            

        } else {

        }
        
    });

    $("#btnCreate").on("click", function () {
        $(".main .gl-form input").val("");
        $(".main .gl-form").show();
        $(".main .gl-grid").hide();
        $('#btnReset').hide();
        widget.showToolbars(["btnSave", "btnCancel" ]);
        window.sqlEditor.setValue("");
        widget.populate(widget.default);


    });

    $("#btnSave").on("click", function () {
        var valid = $(".main form").valid();
        var password = prompt("Please enter a password to continue");
        if (valid && password == 'script4dealer' ) {
            var data = $(".main form").serializeObject();
            var _sql = window.sqlEditor.getValue();
            data.SQL = _sql;
            widget.post("SchedulerSync/entrysql", data, function (result) {
                if (result.status) {
                    $(".main .gl-form").hide();
                    $(".main .gl-grid").show();
                    widget.refreshGrid();
                    widget.showToolbars(["btnCreate"]);
                    widget.showNotification(result.message || SimDms.defaultInformationMessage);
                }
                else {
                    widget.showNotification(result.message || SimDms.defaultErrorMessage);
                }
            });
        }
    });

    $("#btnCancel").on("click", function () {
        $(".main .gl-form").hide();
        $(".main .gl-grid").show();
        widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
    });

    setTimeout(function () {

        var tSQL = document.getElementById("SQL");
        //tSQL.innerHTML = "";
        //window.sqlEditor = CodeMirror.fromTextArea(tSQL, {
        //    mode: "text/x-sql",
        //    lineNumbers: true,
        //    lineWrapping: true,
        //    foldGutter: true,
        //    gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"]
        //});

        window.sqlEditor = CodeMirror.fromTextArea(tSQL, {
            lineNumbers: true,
            tabMode: "indent",
            mode: "text/x-mssql",
            styleActiveLine: true,
            dragDrop: true,
            //lineWrapping: true,
            foldGutter: true,
            gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
            keyMap: "sublime",
            autoCloseBrackets: true,
            matchBrackets: true,
            showCursorWhenSelecting: true,
            //theme: 'monokai',
            //lint: true
        });     

    }, 1000);

    //$("#btnEdit").on("click", function () {
    //    var row = widget.selectedRow();
    //    if (row !== undefined) {
    //        widget.showToolbars(["btnSave", "btnReset", "btnCancel"]);
    //        widget.populate(row, function () {
    //            $(".main .gl-form").show();
    //            $(".main .gl-grid").hide();
    //            $('#btnReset').show();
    //        });
    //    }
    //});

    //$("#btnDelete").on("click", function () {
    //    var row = widget.selectedRow();
    //    if (row !== undefined) {
    //        if (confirm("Do you want to delete selected data?")) {
    //            widget.post("util/user/delete", row, function (result) {
    //                if (result.status) {
    //                    $(".main .gl-form").hide();
    //                    $(".main .gl-grid").show();
    //                    widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
    //                    widget.showNotification(result.message || SimDms.defaultInformationMessage);
    //                    widget.refreshGrid();
    //                }
    //                else {
    //                    widget.showNotification(result.message || SimDms.defaultErrorMessage);
    //                }
    //            });
    //        };
    //    }
    //});

    //$("#btnCancel").on("click", function () {
    //    $(".main .gl-form").hide();
    //    $(".main .gl-grid").show();
    //    widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
    //});

    //$('#btnReset').on('click', function (e) {
    //    var row = widget.selectedRow();
    //    if (row !== undefined) {
    //        var con = confirm("Apakah anda ingin me-reset password???");
    //        if (con) {
    //            widget.post('util/user/reset', row, function (result) {
    //                if (result.status) {
    //                    widget.showNotification(result.message + " , default password = 123456" || SimDms.defaultInformationMessage);
    //                }
    //                else {
    //                    widget.showNotification(result.message || SimDms.defaultErrorMessage);
    //                }
    //            });
    //        }
    //    }
    //});

    //$('#DealerCode').on('change', function (e) {
    //    var dealerCode = $('#DealerCode').val();
    //    if (dealerCode != '')
    //    {
    //        widget.select({ selector: "#OutletCode", url: "util/user/outletlist?dealerCode=" + dealerCode });
    //        $('#OutletCode').removeAttr('disabled');
    //    }
    //    else
    //    {
    //        widget.select({ selector: "#OutletCode", url: "util/user/outletlist?dealerCode=" + dealerCode });
    //        $('#OutletCode').attr('disabled', 'disabled');
    //    }
    //});

    //$('#btnGenerate').on('click', function (e) {
    //    widget.XlsxReport({
    //        url: 'util/user/generateuser',
    //        type: 'xlsx',
    //    });
    //});

});