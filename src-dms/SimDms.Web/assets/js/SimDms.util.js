SimDms.Widget.prototype.defaultData = {};

SimDms.Widget.prototype.changeSwitchValue = function (params) {
    var panel = "";

    //console.log(params);

    if (this.isNullOrEmpty(params.panel) == false) {
        panel = "#" + params.panel;
    }

    if (params.value == "true" || params.value == true) {
        $(panel + " #" + params.name + "Y").click();
        $(panel + " #" + params.name + "Y").attr("value", "true");
        $(panel + " #" + params.name + "N").attr("value", "true");
    }
    else if (params.value == "false" || params.value == false) {
        $(panel + " #" + params.name + "N").click();
        $(panel + " #" + params.name + "Y").attr("value", "false");
        $(panel + " #" + params.name + "N").attr("value", "false");
    }
}

SimDms.Widget.prototype.isNullOrEmpty = function (params) {
    var status = false;
    var _this = this;

    if (_this.isArray(params) == true) {
        $.each(params, function (key, val) {
            if (_this.isNullOrEmpty(val) == true) {
                status = true;
            }
        });
    }
    else {
        if (params != null && params != undefined && params !== null && params !== undefined && $.trim(params) != "" && $.trim(params) !== "") {
            status = false;
        }
        else {
            status = true;
        }
    }

    return status;
}

SimDms.Widget.prototype.isArray = function (params) {
    try {
        if (params === undefined ) return false;
        if (params.constructor === Array) {
            return true;
        }
        else {
            return false;
        }
    }
    catch (ex) {
        return false;
    }
}

SimDms.Widget.prototype.clearFormCallback = function () { }

SimDms.Widget.prototype.initClearFormCallback = function (paramCallback) {
    this.clearFormCallback = paramCallback;
}

SimDms.Widget.prototype.toDateFormat = function (data) {
    try {
        return moment(data).format(SimDms.dateFormat);
    }
    catch (ex) {
        return "-";
    }
}

SimDms.Widget.prototype.toDateTimeFormat = function (data) {
    return moment(data).format(SimDms.dateTimeFormat);
}

SimDms.Widget.prototype.toTimeFormat = function (data) {
    return moment(data).format(SimDms.timeFormat);
}

SimDms.Widget.prototype.clearForm = function (formName) {
    if (this.isNullOrEmpty(formName)) {
        $(".main form input[type='text'], .main form textarea, .main form select").val("");
        $.each(this.defaultData, function (key, val) {
            $("#" + key + ", input[name='" + key + "']").val(val);
        });

        $.each($("input[type='radio']"), function (key, val) {
            var side = $(val).attr("id");
            side = side.toString();

            if (side.charAt(side.length - 1) == 'N') {
                $(val).click();
            }
        });
        $("input[type='radio']").attr("value", "false");
    }
    else {
        $(".main form[name='" + formName + "'] input[type='text'], .main form[name='" + (formName || "") + "'] textarea, .main form[name='" + (formName || "") + "'] select").val("");
        $.each(this.defaultData, function (key, val) {
            $("#" + key + ", input[name='" + key + "']").val(val);
        });

        $.each($("form[name='" + formName + "'] input[type='radio']"), function (key, val) {
            var side = $(val).attr("id");
            side = side.toString();

            if (side.charAt(side.length - 1) == 'N') {
                $(val).click();
            }
        });
        $("form[name='" + formName + "'] input[type='radio']").attr("value", "false");
    }

    if (this.isNullOrEmpty(this.clearFormCallback) == false) {
        this.clearFormCallback();
    }
}

SimDms.Widget.prototype.clearPanelInputs = function (params) {
    var _this = this;

    if (this.isNullOrEmpty(params) == false) {
        $.each(params, function (key, val) {
            $("#" + val + " input[type='text']").val("");
            $("#" + val + " textarea").val("");
            $("#" + val + " select").val("");

            var itemPanel = val;
            var itemRadio = $("#" + val + " input[type='radio']");
            var iterator = 0;

            $.each(itemRadio, function (key, val) {
                var itemName = $(val).attr("name");
                var parameters = {
                    panel: itemPanel,
                    name: itemName,
                    value: false
                };

                if (iterator % 2 == 0) {
                    _this.changeSwitchValue(parameters);
                }

                iterator++;
            });
        })
    }
}

SimDms.Widget.prototype.showPanels = function (params) {
    if (this.isNullOrEmpty(params) == false) {
        $.each(params, function (key, val) {
            var itemObject = $("#" + val.name);
            if (val.isVisible == true) {
                itemObject.fadeIn();
            }
            else {
                itemObject.fadeOut();
            }
        });
    }
}

SimDms.Widget.prototype.showItem = function (params) {
    if (this.isNullOrEmpty(params) == false) {
        $.each(params, function (key, val) {
            var itemObject = $("[name='" + val.name + "']");
            var itemObjectWrapper;

            if (itemObject.length <= 0) {
                itemObject = $("textarea[name='" + val.name + "']");
            }

            if (itemObject.length <= 0) {
                itemObject = $("[id='" + val.name + "']");
            }

            if (itemObject.attr("data-type") == "datepicker") {
                itemObjectWrapper = itemObject.parent("div").parent("div").parent("div");
            }
            else if (itemObject.attr("type") == "text") {
                itemObjectWrapper = itemObject.parent("div").parent("div");
            }
            else {
                itemObjectWrapper = $("[name='" + val.name + "']").parent("div").parent("div");
            }

            if (val.isVisible == true || val.isVisible == "true") {
                itemObjectWrapper.fadeIn();
            }
            else {
                itemObject.val("");
                itemObjectWrapper.fadeOut();
            }
        });
    }
}

SimDms.Widget.prototype.getValue = function (params) {
    if (this.isNullOrEmpty(params) == false) {
        switch (params.type) {
            case "select":
                return $("select[name='" + params.name + "']").val();

            case "text":
                return $("input[name='" + params.name + "']").val();

            case "input":
                return $("input[name='" + params.name + "']").val();

            case "textarea":
                return $("textarea[name='" + params.name + "']").val();

            case "switch":
                return $("input[name='" + params.name + "']:radio").val();
        }
    }
}

SimDms.Widget.prototype.getForms = function (formName) {
    if (this.isNullOrEmpty(formName) == false) {
        //return $(".main form[name='" + formName + "']").serializeObject();
        return $("form[name='" + formName + "']").serializeObject();
    }
    else {
        return $(".main form").serializeObject();
    }
}

SimDms.Widget.prototype.getObject = function (itemName, itemType, itemAttribute) {
    /*
        itemAttribute {
	        data-source: "json"
        }
    */
    var selector;

    if (this.isNullOrEmpty(itemName) == false) {
        if (this.isNullOrEmpty(itemType) == true) {
            selector = "input[name='" + itemName + "']";
        }
        else {
            switch (itemType) {
                case "select":
                    if (this.isNullOrEmpty(itemAttribute) == false) {
                        selector = itemType + "[data-source='json']";
                    }
                    else {
                        selector = itemType + "[name='" + itemName + "']";
                    }
                    break;

                case "table":
                    if (this.isNullOrEmpty(itemAttribute) == false) {
                        selector = itemType + "[data-source='json']";
                    }
                    else {
                        selector = itemType + "[name='" + itemName + "']";
                    }
                    break;

                case "image":
                    selector = "img#" + itemName + "";
                    break;
                case "textarea":
                case "button":
                    selector = itemType + "[name='" + itemName + "']";
                    break;

                default:
                    selector = "[name='" + itemName + "']";
                    break;
            }
        }
    }

    return $(selector);
}

SimDms.Widget.prototype.eventList;

SimDms.Widget.prototype.setEventList = function (paramEventList) {
    this.eventList = paramEventList;
}

SimDms.Widget.prototype.initializeEvent = function () {
    var params = this.eventList;
    var _this = this;

    if (this.isNullOrEmpty(params) == false) {
        $.each(params, function (key, val) {
            var itemObject = _this.getObject(val.name, val.type);
            switch (val.eventType) {
                case "click":
                    itemObject.on("click", function (evt) {
                        evt.preventDefault();
                        val.event(evt);
                        return false;
                    });
                    break;

                case "change":
                    itemObject.on("change", function (evt) {
                        evt.preventDefault();
                        val.event(evt);
                    });
                    break;

                case "input":
                    itemObject.on("input", function (evt) {
                        evt.preventDefault();
                        val.event(evt);
                    });
                    break;

                case "blur":
                    itemObject.on("blur", function (evt) {
                        evt.preventDefault();
                        val.event(evt);
                    });
                    break;

                case "keypress":
                    itemObject.keypress(function (evt) {
                        val.event(evt);
                    });
                    break;

                case "keyup":
                    itemObject.keypress(function (evt) {
                        val.event(evt);
                    });
                    break;

                case "keydown":
                    itemObject.keypress(function (evt) {
                        evt.preventDefault();
                        val.event(evt);
                    });
                    break;
            }
        });
    }
}

SimDms.Widget.prototype.initializeUpload = function () {
    var itemsObjects = $("button[binding-type='button-upload']");
    var _this = this;

    $.each(itemsObjects, function (key, val) {
        var itemObject = $(this);

        var dataBindingAttr = itemObject.attr("binding-data");
        var dataBindingUrl = itemObject.attr("binding-url");
        var callbackFunction = itemObject.attr("binding-callback");
        var onUploadFunction = itemObject.attr("binding-on-upload");
        var onProgressFunction = itemObject.attr("binding-on-progress");
        if (_this.isNullOrEmpty(dataBindingAttr) == false) {
            var formContent = "";
            var formName = "form" + dataBindingAttr;
            var fileElement = $("form[name='" + formName + "'] input[name='file']");
            fileElement.remove();

            formContent += "<form action='" + dataBindingUrl + "' method='post' name='" + formName + "' id='" + formName + "' class='hide' enctype='multipart/form-data'><input type='file' name='file'></form>";
            $("body").append(formContent);

            fileElement = $("form[name='" + formName + "'] input[name='file']");
            itemObject.on("click", function (events) {
                fileElement.click();
            });

            fileElement.on("change", function (key, val) {
                //$("#" + formName).ajaxSubmit({
                //    beforeSend: function () {
                //        $(".ajax-loader").fadeIn();
                //    },
                //    success: function (result) {
                //        if (_this.isNullOrEmpty(result) == false) {
                //            if (_this.isNullOrEmpty(callbackFunction) == false) {
                //                eval(callbackFunction + "(result, _this)");
                //            }
                //        }
                //        else {
                //        }
                //    },
                //    complete: function () {
                //        $(".ajax-loader").fadeOut();
                //    },
                //    error: function () {
                //        _this.showNotification("Sorry, file cannot uploaded into server.");
                //    }
                //});

                var formData = new FormData();
                var progressInterval = null;
                formData.append("file", fileElement[0].files[0]);

                $.ajax({
                    url: dataBindingUrl,
                    data: formData,
                    type: "POST",
                    cache: false,
                    contentType: false,
                    processData: false,
                    beforeSend: function() {
                        $(".ajax-loader").fadeIn();
                    },
                    xhr: function () {
                        var __this = _this;
                        myXhr = $.ajaxSettings.xhr();
                        if (myXhr.upload) {
                            myXhr.upload.addEventListener('progress', function (evt) {
                                var progress = 0;
                                if (evt.lengthComputable) {
                                    progress = Math.round(evt.loaded * 100 / evt.total);
                                    eval(onUploadFunction + "(progress);");

                                    if (progress >= 100) {
                                        if (_this.isNullOrEmpty(onProgressFunction) == false) {
                                            eval("progressInterval = setInterval(function() {" + onProgressFunction + "();}, 1000);");
                                        }
                                    }
                                }
                                else if (_this.isNullOrEmpty(onProgressFunction) == false) {
                                    eval("progressInterval = setInterval(" + onProgressFunction + "(), 1000);");
                                }
                            }, false);
                        }
                        return myXhr;
                    },
                    complete: function (result) {
                        //options.events.complete(result);
                        $(".ajax-loader").fadeOut();
                        clearInterval(progressInterval);
                        _this.hideCornerNotification();
                    },
                    success: function (result) {
                        //options.events.success(result);
                        eval(callbackFunction + "(result);");
                    },
                    error: function (a, b, c) {
                        _this.showNotification("Sorry, we cannot process your request.");
                    }
                });
            });
        }
    });
}

SimDms.Widget.prototype.cleanJsonDate = function (dateValue) {
    try {
        return eval("new " + dateValue.slice(1, -1));
    }
    catch (ex) {
        return null;
    }
}

SimDms.Widget.prototype.setInnerGrid = function (options) {
    /*
    
    var options = {
        xtype: "grid",
        title: "Element title",
	    name: "",
        source: "",
        columns: [
            { 
                mData: "CustomerCode", sTitle: "Cust Code", sWidth: "110px",
                mRender: function(id, data, full) {
	                return .......
                }
            }
        ],
        additionalParams: [
            { name: "", element: "" }
        ],
        onDblClick: function(evt, data) {
	    },
        editButton: true,
        deleteButton: true,
        editAction: function(evt, data){},
        deletetAction: function(evt, data){},
    }

    */
    var _this = this;

    if (_this.isNullOrEmpty(options) == false) {
        //$.each(options, function (key, val) {
        _this.innerGrid[(options.tblname || options.name)] = options;
        //});
    }
}

SimDms.Widget.prototype.initializeInnerGrid = function () {
    var _this = this;

    if (_this.isNullOrEmpty(_this.innerGrid) == false) {
        $.each(_this.innerGrid, function (key, val) {
            var tableObject = _this.getObject((val.tblname || val.name), "table");
            var simDms = SimDms;
            var selectable = val.selectable;
            var multiselect = val.multiselect;

            if (_this.isNullOrEmpty(val.autoLoad) == false && val.autoLoad == true) {
                val.instance = tableObject.dataTable({
                    bProcessing: true,
                    bServerSide: true,
                    sServerMethod: "POST",
                    sPaginationType: "full_numbers",
                    sAjaxSource: simDms.baseUrl + val.source,
                    aaSorting: (val.sortings || []),
                    aoColumns: val.columns,
                    fnServerData: function (sSource, aoData, fnCallback, oSettings) {
                        var params = val.additionalParams;

                        if (_this.isNullOrEmpty(params) == false) {
                            $.each(params, function (key, value) {
                                aoData.push({
                                    name: value.name,
                                    value: _this.getValue({ name: value.element, type: value.type || "text" })
                                });
                            });
                        }

                        oSettings.jqXHR = $.ajax({
                            "dataType": 'json',
                            "type": "POST",
                            "url": sSource,
                            "data": aoData,
                            "success": fnCallback
                        });
                    },
                    fnPreDrawCallback: function () {
                    },
                    fnDrawCallback: function (e) {
                        if (selectable) {
                            if (multiselect) {
                                $("#" + val.tblname + " tbody tr").click(function (e) {
                                    var self = $(this);
                                    if (!self.hasClass("row_selected")) {
                                        self.addClass("row_selected");
                                    }
                                    else {
                                        self.removeClass("row_selected");
                                    }
                                });
                            }
                            else {
                                $("#" + val.tblname + " tbody tr").click(function (e) {
                                    var self = $(this);
                                    if (!self.hasClass("row_selected")) {
                                        self.parent().children().removeClass("row_selected");
                                        self.addClass("row_selected");
                                    }
                                    else {
                                        self.removeClass("row_selected");
                                    }
                                });
                            }
                        }

                        $("#" + val.name + " tbody tr .icon").on("click", function (evt) {
                            for (var i = 0; i < _this.gridSettings.evtclick.length; i++) {
                                var tableName = val.name;
                                var iconName = $(this).context.className.split('-')[1].split(' ')[0];
                                var data = val.instance.fnGetData($(this).parent().parent()[0]);
                                _this.gridSettings.evtclick[i](iconName, data);

                                //console.log(data);

                                //                        var iconButton = $(this);
                                //                        if (iconButton.hasClass("icon-edit")) {
                                //                            console.log(iconName);
                                //                        }
                                //                        if (iconButton.hasClass("icon-delete")) {
                                //                            console.log("delete");
                                //                        }
                            }
                            this.onGridClick = function (callback) { _this.gridSettings.evtclick.push(callback); }
                            var data = val.instance.fnGetData($(this).parent().parent()[0]);
                            var iconButton = $(this);
                            if (iconButton.hasClass("icon-edit") && _this.isNullOrEmpty(val.editAction) == false) {
                                val.editAction(evt, data);
                            }

                            if (iconButton.hasClass("icon-trash") && _this.isNullOrEmpty(val.deleteAction) == false) {
                                val.deleteAction(evt, data);
                            }
                        });

                        if (_this.isNullOrEmpty(val.onDblClick) == false) {
                            $("#" + val.name + " tbody tr").on("dblclick", function (evt) {
                                var data = val.instance.fnGetData(this);
                                val.onDblClick(evt, data);
                            });
                        }
                    },
                    aLengthMenu: [[5, 10, 15, 25, 50, 100], [5, 10, 15, 25, 50, 100]],
                    iDisplayLength: 10,
                }).fnSetFilteringDelay();
            }
            else {
                val.instance = tableObject.dataTable({
                    bProcessing: true,
                    bServerSide: true,
                    sServerMethod: "POST",
                    sPaginationType: "full_numbers",
                    sAjaxSource: simDms.baseUrl + val.source,
                    aaSorting: (val.sortings || []),
                    aoColumns: val.columns,
                    fnServerData: function (sSource, aoData, fnCallback, oSettings) {
                        var params = val.additionalParams;

                        if (_this.isNullOrEmpty(params) == false) {
                            $.each(params, function (key, value) {
                                aoData.push({
                                    name: value.name,
                                    value: (_this.getValue({ name: value.element, type: value.type || "text" })) || value.value
                                });
                            });
                        }

                        oSettings.jqXHR = $.ajax({
                            "dataType": 'json',
                            "type": "POST",
                            "url": sSource,
                            "data": aoData,
                            "success": fnCallback
                        });
                    },
                    fnPreDrawCallback: function () {
                    },
                    fnDrawCallback: function (e) {
                        if (selectable) {
                            if (multiselect) {
                                $("#" + val.tblname + " tbody tr").click(function (e) {
                                    var self = $(this);
                                    if (!self.hasClass("row_selected")) {
                                        self.addClass("row_selected");
                                    }
                                    else {
                                        self.removeClass("row_selected");
                                    }
                                });
                            }
                            else {
                                $("#" + val.tblname + " tbody tr").click(function (e) {
                                    var self = $(this);
                                    if (!self.hasClass("row_selected")) {
                                        self.parent().children().removeClass("row_selected");
                                        self.addClass("row_selected");
                                    }
                                    else {
                                        self.removeClass("row_selected");
                                    }
                                });
                            }
                        }

                        $("#" + val.name + " tbody tr .icon").on("click", function (evt) {
                            for (var i = 0; i < _this.gridSettings.evtclick.length; i++) {
                                var tableName = val.name;
                                var iconName = $(this).context.className.split('-')[1].split(' ')[0];
                                var data = val.instance.fnGetData($(this).parent().parent()[0]);
                                _this.gridSettings.evtclick[i](iconName, data);

                                //console.log(data);

                                //var iconButton = $(this);
                                //if (iconButton.hasClass("icon-edit")) {
                                //    console.log(iconName);
                                //}
                                //if (iconButton.hasClass("icon-delete")) {
                                //    console.log("delete");
                                //}
                            }
                            this.onGridClick = function (callback) { _this.gridSettings.evtclick.push(callback); }
                            var data = val.instance.fnGetData($(this).parent().parent()[0]);
                            var iconButton = $(this);
                            if (iconButton.hasClass("icon-edit") && _this.isNullOrEmpty(val.editAction) == false) {
                                val.editAction(evt, data);
                            }

                            if (iconButton.hasClass("icon-trash") && _this.isNullOrEmpty(val.deleteAction) == false) {
                                val.deleteAction(evt, data);
                            }
                        });

                        if (_this.isNullOrEmpty(val.onDblClick) == false) {
                            $("#" + val.name + " tbody tr").on("dblclick", function (evt) {
                                var data = val.instance.fnGetData(this);
                                val.onDblClick(evt, data);
                            });
                        }
                    },
                    aLengthMenu: [[5, 10, 15, 25, 50, 100], [5, 10, 15, 25, 50, 100]],
                    iDisplayLength: 10,
                }).fnSetFilteringDelay();
            }
        });
    }
}

SimDms.Widget.prototype.populateInnerGrid = function (options) {
    var tableObject = this.getObject(options.name, "table");

    var simDms = SimDms;

    var innerGrid = tableObject.dataTable({
        additionalParams: {
            param1: "satu",
            param2: "dua",
            param3: "tiga"
        },
        bProcessing: true,
        bServerSide: false,
        sServerMethod: "POST",
        sAjaxSource: simDms.baseUrl + options.url,
        sPaginationType: "full_numbers",
        bAutoWidth: false,
        fnPreDrawCallback: function () {
            $(".page .ajax-loader").fadeIn();
        },
        fnDrawCallback: function () {
            $(".page .ajax-loader").fadeOut();

            //console.log("draw grid callback ...");
        },
        aLengthMenu: [[10, 15, 25, 50, 100], [10, 15, 25, 50, 100]],
        iDisplayLength: 10,
        aoColumns: options.columns,
        aaSorting: options.sortings || [[0, "asc"]]
    }).fnSetFilteringDelay();
}

SimDms.Widget.prototype.clearSelectedRecord = function (gridName) {
    var selector = "";

    if (this.isNullOrEmpty(gridName) == false) {
        selector = "#" + gridName + " tbody tr";
    }
    else {
        selector = "table tbody tr";
    }

    $(selector).removeClass("row_selected");
}

SimDms.Widget.prototype.reloadGridData = function (gridName) {
    var _this = this;
    if (_this.isNullOrEmpty(_this.innerGrid) == false) {
        var val = _this.innerGrid[gridName];
        var selectable = val.selectable;
        var multiselect = val.multiselect;

        var tableObject = _this.getObject(gridName, "table");
        tableObject.dataTable().fnDestroy();
        var actionButtonsHtml = "";

        if (val.editButton) {
            actionButtonsHtml += "<i class='icon icon-edit' />";
        }

        if (val.deleteButton) {
            actionButtonsHtml += "<i class'icon icon-delete' />";
        }

        var gridColumns = new Array();
        $.each(val.columns, function (key, value) {
            gridColumns.push(value);
        });

        if (val.editButton || val.deleteButton) {
            //val.columns.push(
            //gridColumns.push(
            //     {
            //         sTitle: "Action",
            //         "mDataProp": "",
            //         "sClass": "",
            //         "sDefaultContent": "<i class='icon icon-edit' /><i class='icon icon-delete' />",
            //     }
            //);
        }

        var simDms = SimDms;
        var tableOptions = {
            bProcessing: true,
            bServerSide: true,
            sServerMethod: "POST",
            sPaginationType: "full_numbers",
            sAjaxSource: simDms.baseUrl + val.source,
            aaSorting: (val.sortings || []),
            //aoColumns: val.columns,
            aoColumns: gridColumns,
            fnServerData: function (sSource, aoData, fnCallback, oSettings) {
                var params = val.additionalParams;
                if (_this.isNullOrEmpty(params) == false) {
                    $.each(params, function (key, value) {
                        aoData.push({
                            name: value.name,
                            value: _this.getValue({ name: value.element, type: value.type || "text" })
                        });
                    });
                }
                oSettings.jqXHR = $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": fnCallback
                });
            },
            fnPreDrawCallback: function () {
            },
            fnDrawCallback: function (e) {
                if (selectable == true) {
                    if (multiselect) {
                        $("#" + (val.tblname || val.name) + " tbody tr").click(function (e) {
                            var self = $(this);
                            if (!self.hasClass("row_selected")) {
                                self.addClass("row_selected");
                            }
                            else {
                                self.removeClass("row_selected");
                            }
                        });
                    }
                    else {
                        $("#" + (val.tblname || val.name) + " tbody tr").click(function (e) {
                            var self = $(this);
                            if (!self.hasClass("row_selected")) {
                                self.parent().children().removeClass("row_selected");
                                self.addClass("row_selected");
                            }
                            else {
                                self.removeClass("row_selected");
                            }
                        });
                    }
                }

                $("#" + val.tblname + " tbody tr .icon").on("click", function (evt) {
                    for (var i = 0; i < _this.gridSettings.evtclick.length; i++) {
                        var tableName = val.name;
                        var iconName = $(this).context.className.split('-')[1].split(' ')[0];
                        var data = val.instance.fnGetData($(this).parent().parent()[0]);
                        _this.gridSettings.evtclick[i](iconName, data);

                        //    var iconButton = $(this);
                        //    if (iconButton.hasClass("icon-edit")) {

                        //    }
                        //    if (iconButton.hasClass("icon-delete")) {
                        //    }
                    }
                    this.onGridClick = function (callback) { _this.gridSettings.evtclick.push(callback); }
                    var data = val.instance.fnGetData($(this).parent().parent()[0]);
                    var iconButton = $(this);

                    if (iconButton.hasClass("icon-edit") && _this.isNullOrEmpty(val.editAction) == false) {
                        val.editAction(evt, data);
                    }

                    if (iconButton.hasClass("icon-trash") && _this.isNullOrEmpty(val.deleteAction) == false) {
                        val.deleteAction(evt, data);
                    }
                });

                if (_this.isNullOrEmpty(val.onDblClick) == false) {
                    $("#" + val.name + " tbody tr").on("dblclick", function (evt) {
                        var data = val.instance.fnGetData(this);
                        val.onDblClick(evt, data);
                    });
                }
            },
            aLengthMenu: [[5, 10, 15, 25, 50, 100], [5, 10, 15, 25, 50, 100]],
            iDisplayLength: 10,
        };



        //tableOptions["aoColumnDefs"] = [
        //    { "aTargets": [actionButtonsIndex], 
        //        "sType": "html", 
        //        "fnRender": function(o, val) { 
        //            return $("<div/>").html(o.aData[actionButtonsIndex]).text();
        //        } 
        //    }
        //];

        val.instance = tableObject.dataTable(tableOptions).fnSetFilteringDelay();
    }
}

SimDms.Widget.prototype.destroyGrid = function (gridName) {
    $("#" + gridName).dataTable().fnDestroy();
}

SimDms.Widget.prototype.populateData = function (data, callback) {
    $(".page .main label.error").hide();
    var _this = this;

    $.each(data, function (key, value) {
        var $ctrl = $("[name=" + key + "]");
        var type = $ctrl.data("type");
        $ctrl.removeClass("error");
        var itemObject;

        if (type === undefined) {
            itemObject = _this.getObject(key);
            //$(".main #" + key).val(value);
            itemObject.val(value);
        }
        else if (type == "date") {
            itemObject = _this.getObject(key);

        }
        else {
            if (type === "switch") {
                value = (value || false);
                $("#" + key + "Y").prop('checked', value).val(value);
                $("#" + key + "N").prop('checked', !value).val(value);
            }
            if (type === "datepicker" || type === "date") {
                value = (value) ? moment(value).format(SimDms.dateFormat) : undefined;
                $(".main [name=\"" + key + "\"]").val(value);
            }
        }

        $(".page .main form").valid();
    });

    if (callback !== undefined && typeof callback === "function") {
        callback(data);
    }
}

SimDms.Widget.prototype.cascadeSettings;

SimDms.Widget.prototype.setCascade = function (options) {
    //Sample options
    /*
    options = [] 
        {
            sourceItem: "",
            targetItem: "",
            sourceItemUrl: "",
            targetItemUrl: "",
            bindingVariables: [
                { name: "", source: "" },
                { name: "", source: "" },
                { name: "", source: "" }
            ]
            optionalText : "-- Pilih Satu --"
        }
    ]
    */

    this.cascadeSettings = options;
}

SimDms.Widget.prototype.initializeCascade = function () {
    var _this = this;

    if (this.isNullOrEmpty(this.cascadeSettings) == false) {
        $.each(this.cascadeSettings, function (key, val) {
            var sourceItem = _this.getObject(val.sourceItem, "select");
            var targetItem = _this.getObject(val.targetItem, "select");

            sourceItem.on("change", function (evt) {
                var params = {};
                params["id"] = _this.getValue({ name: val.sourceItem, type: "select" });

                if (_this.isNullOrEmpty(val.bindingVariables) == false) {
                    $.each(val.bindingVariables, function (key1, val1) {
                        params[val1.name] = _this.getValue({ name: val1.source, type: (val1.sourceType || "select") });
                    });
                }

                var condition = false;
                var conditionEvent = false;
                if (_this.isNullOrEmpty(val.condition) == false) {
                    conditionEvent = true;
                    var query = "if (";
                    $.each(val.condition, function (key2, val2) {
                        query += "_this.getValue({name: '" + val2.name + "', type: '" + val2.type + "'})" + val2.condition + " " + (val.conditionType || "&&") + " ";
                    });
                    query = query.substr(0, query.length - 3) + ") ";
                    query += "{ condition = true; }";
                    eval(query);
                }

                targetItem.html("<option value=''>" + SimDms.selectOneText + "</option>");

                if ((conditionEvent == true && condition == true) || (conditionEvent == false)) {
                    _this.post(val.targetItemUrl, params, function (result) {
                        var selectItems = "";

                        if (_this.isNullOrEmpty(val.optionalText) == false) {
                            selectItems += "<option value=" + undefined + ">" + val.optionalText + "</option>";
                        }
                        else {
                            selectItems += "<option value=''>" + SimDms.selectOneText + "</option>";
                        }

                        $.each(result, function (key2, val2) {
                            selectItems += "<option value='" + val2.value + "'>" + val2.text + "</option>";
                        });

                        targetItem.html(selectItems);
                    });
                }
            });
        });
    }
}

SimDms.Widget.prototype.cascade = function (options) {
    /*
    {
        source: "",
        target: "",
        url: "",
        additionalParams: [
            {
	            name: "",
                element: "",
                type: ""
            }
        ]
	}
    */

    var _this = this;

    if (_this.isNullOrEmpty(options) == false) {
        var sourceElement = _this.getObject(options.source, "select");
        var targetElement = _this.getObject(options.target, "select");

        var params = {};

        params["id"] = _this.getValue({ name: options.source, type: "select" });

        if (_this.isNullOrEmpty(options.sourceValue) == false) {
            params["id"] = options.sourceValue;
        }

        $.each(options.additionalParams, function (key, val) {
            var paramValue;

            if (_this.isNullOrEmpty(val.value) == true) {
                paramValue = _this.getValue({ name: val.element, type: val.type });
            }
            else {
                paramValue = val.value;
            }
            params[val.name] = paramValue;
        });

        targetElement.html("<option value=''>" + SimDms.selectOneText + "</option>");

        _this.post(options.url, params, function (result) {
            var htmlContent = "";

            if (_this.isNullOrEmpty(options.optionalText) == false) {
                htmlContent += "<option value=''>" + val.optionalText + "</option>";
            }
            else {
                htmlContent += "<option value=''>" + SimDms.selectOneText + "</option>";
            }

            $.each(result, function (key, val) {
                htmlContent += "<option value='" + val.value + "'>" + val.text + "</option>";
            });


            targetElement.val(options.target, options.targetValue);
            targetElement.html(htmlContent);
            _this.clearValidation();

            if (_this.isNullOrEmpty(options.targetValue) == false) {
                targetElement.val(options.targetValue || "");
            }


            //setTimeout(function () {
            //    if (options.enabled == false) {
            //        targetElement.attr("disabled", "disabled");
            //    }
            //    else {
            //        targetElement.removeAttr("disabled");
            //    }
            //}, 500);
        });
    }
}

SimDms.Widget.prototype.selectSettings;

SimDms.Widget.prototype.initializeSelect = function () {
    var _this = this;
    if (_this.isNullOrEmpty(_this.selectSettings) == false) {
        $.each(_this.selectSettings, function (key, val) {
            var selectElement = _this.getObject(val.name, val.type || "select");
            var optionalText = "";
            var selectItems = "";

            if (_this.isNullOrEmpty(val.optionalText) == true) {
                optionalText += "<option value=''>" + SimDms.selectOneText + "</option>";
            }
            else {
                optionalText += "<option value=''>" + val.optionalText + "</option>";
            }

            var selectParams = val.params || {};

            var params = $.extend({
                id: _this.getValue({ name: val.name, type: val.type || "select" })
            }, selectParams);


            //console.log(val.additionalParams);

            //if (_this.isNullOrEmpty(val.additionalParams) == false) {
            //    $.each(val.additionalParams, function (key1, val1) {
            //        params[val1.name] = _this.getValue({ name: val1.element, type: val1.type }) || val1.value;
            //    });
            //}

            //console.log(params);

            if (_this.isNullOrEmpty(val.cascade) == false) {
                var triggerElement = _this.getObject(val.cascade.name, "select");

                triggerElement.on("change", function (evt) {
                    var condition = false;
                    var conditionStatus = false;
                    var queries = "";

                    if (_this.isNullOrEmpty(val.cascade.conditions) == false) {
                        condition = true;
                        queries += "if (";
                        $.each(val.cascade.conditions, function (key1, val1) {
                            queries += "_this.getValue({ name: '" + val1.name + "', type: '" + (val1.type || "select") + "' })" + val1.condition + " " + (val.conditionType || "&&") + " ";
                        });

                        queries = queries.substring(0, queries.length - 3);
                        queries += " ) { conditionStatus = true; }";
                    }

                    if (_this.isNullOrEmpty(queries) == false) {
                        eval(queries);
                    }

                    //var params = {
                    //    id: _this.getValue({ name: val.name, type: val.type || "select" })
                    //};


                    if ((condition == true && conditionStatus == true) || condition == false) {
                        params["id"] = _this.getValue({ name: val.cascade.name, type: val.cascade.type || "select" });

                        if (_this.isNullOrEmpty(val.cascade.additionalParams) == false) {
                            $.each(val.cascade.additionalParams, function (key2, val2) {
                                var paramValue = _this.getValue({ name: val2.source, type: val2.type || "input" }) || $("[name='" + val2.source + "']").val();
                                var paramName = val2.name;

                                params[val2.name] = paramValue;
                            });
                        }

                        _this.post(val.url, params, function (result) {
                            selectItems = optionalText + "";

                            if (_this.isNullOrEmpty(result) == false) {
                                $.each(result, function (key1, val1) {
                                    selectItems += "<option value='" + val1.value + "'>" + val1.text + "</option>";
                                });
                            }

                            selectElement.html(selectItems);
                            //selectElement.change();
                        });
                    }
                    else {
                        selectItems = optionalText + "";
                        selectElement.html(selectItems);
                    }

                });
            } else {
                _this.post(val.url, params, function (result) {
                    selectItems = optionalText + "";

                    if (_this.isNullOrEmpty(result) == false) {
                        $.each(result, function (key1, val1) {
                            selectItems += "<option value='" + val1.value + "'>" + val1.text + "</option>";
                        });
                    }

                    selectElement.html(selectItems);
                });
            }
        });
    }
}

SimDms.Widget.prototype.setSelect = function (options) {
    this.selectSettings = options;

    /*
    var options = [
        {
            name: "[element]",
            url: "[data source url]",
            optionalText: "[Optional text for select one]",
            params: {},
            defaultValue: 
            cascade: {
                name: "[trigger element]",
                type: "[optional, detault is 'select']",
                source: "[cascade source that trigger]",
                additionalParams: [ 
                    { name: "", source: "", type: "" },
                    { name: "", source: "", type: "" }
                ],
                condition: [
                    { name: "",  }
                ]
            }            
        }
    ]
    */
}

SimDms.Widget.prototype.selects = function (options) {
    /*
        options = {
	        name: "",
            source: "",
            type: "default select",
            optionalText: "",
            additionalParams: [
                {
	                name: "",
                    element: "",
                    type: "[default input]"
                }
            ]
        }
    */

    var _this = this;

    if (_this.isNullOrEmpty(options) == false) {
        var params = {};
        var itemObject = _this.getObject(options.name, options.type || "select");

        $.each(options.additionalParams, function (key, val) {
            params[val.name] = _this.getValue({ name: val.element, type: val.type }) || val.value;
        });

        var html = "";

        if (_this.isNullOrEmpty(options.optionalText) == false) {
            html += "<option value=''>" + options.optionalText + "</option>";
        }
        else {
            html += "<option value=''>" + SimDms.selectOneText + "</option>";
        }

        _this.post(options.source, params, function (result) {
            if (_this.isNullOrEmpty(result) == false) {
                $.each(result, function (key, val) {
                    html += "<option value='" + val.value + "'>" + val.text.toUpperCase() + "</option>";
                });

                itemObject.html(html);
            }
        });
    }
}

SimDms.Widget.prototype.innerGrid = {}

SimDms.Widget.prototype.gridSettings = { evtclick: [] };

SimDms.Widget.prototype.popupSettings = {};

SimDms.Widget.prototype.setPopup = function (options) {
    var _this = this;
    if (_this.isNullOrEmpty(options) == false) {
        _this.popupSettings[options.name] = options;
    }
}

SimDms.Widget.prototype.initializePopup = function () {
    var _this = this;

    $.each(_this.popupSettings, function (key, val) {
        var buttonObject = _this.getObject("btn" + val.name, "button");
        var simDms = SimDms;

        //console.log(val);
        buttonObject.on("click", function (evt) {
            _this.lookup.init({
                name: "grid" + val.name,
                title: val.title,
                source: val.source,
                columns: val.columns,
                additionalParams: val.additionalParams
            });
            _this.lookup.show();
        });
    });
}

SimDms.Widget.prototype.popupOverlaySettings = {};

SimDms.Widget.prototype.setPopups = function (options) {
    /*
    {
	    name: "",
        title: "",
        buttons: [
            { name: "", text: "", icon: ""}
        ],
        listeners: [
            { name: "", eventType: "", event: function(evt) {} }
        ],
        items: [
            { name: "", type: "", cls: "" }
        ],
        generatedHtml : ""
    }
    */

    var _this = this;
    if (_this.isNullOrEmpty(_this.popupOverlaySettings) == false) {
        $.each(options, function (key, val) {
            _this.popupOverlaySettings[val.name] = val;
        });
    }

}

SimDms.Widget.prototype.initializePopupOverlays = function () {
    var _this = this;
    var overlayObject = $(".overlay, .popup-frame");
    var popupContent = "<div class='gl-widget'>";
    popupContent += "<form>";

    overlayObject.on("click", function (evt) {
        overlayObject.fadeOut();
    });

    if (_this.isNullOrEmpty(_this.popupOverlaySettings) == false) {
        $.each(_this.popupOverlaySettings, function (key, val) {
            var htmlButtons = "";
            var prefix = "";
            var htmlElements = "";

            $.each(val.buttons, function (key, val) {
                htmlButtons += "<button type='button' class='" + (val.cls || "") + "' name='" + val.name + "' id='" + val.name + "'>" + val.text + "<i class='icon " + (val.icon || "") + "'></i></button>";
            });


            if (_this.isNullOrEmpty(val.title) == false) {
                prefix += "<div class=\"subtitle\">" + val.title + "<i class='icon icon-minus'></i></div>";
            }
            else {
                prefix += "<div class=\"divider\"></div>";
            }

            $.each(val.items, function (key, val) {
                switch (val.type) {
                    case "form":
                        htmlElements = _this.generateForm(options.items);
                        break;
                    case "panel":
                        htmlElements = _this.generatePanel(options);
                        break;
                    case "grid":
                        htmlElements = _this.generateGrid(options);
                        break;
                    case "report":
                        htmlElements = "<form>" + _this.generatePanel(options) + "<div class=\"panel frame\"><iframe></iframe></div></form>";
                        break;
                    case "panels":
                        htmlElements = "<form>" + _this.generatePanels(options.panels) + "</form>";
                        break;
                    case "grid-form":
                        htmlElements = _this.generateGrid(options) + _this.generateForm(options.items);
                        break;
                    case "grid-panels":
                        htmlElements = _this.generateGrid(options) + "<form class=\"gl-form\">" + _this.generatePanels(options.panels) + "</form>";
                        break;
                    default:
                        break;
                }
            });
        });
    }

    popupContent += "</form>";
    popupContent += "</div>";

    $(".popup-frame .inner").html(popupContent);
}

SimDms.Widget.prototype.validate = function (formName) {
    if (this.isNullOrEmpty(formName) == false) {
        //return (".main #" + formName + "]").valid();

        return $(".page .main form#" + formName).valid();
        //return $("form#" + formName).valid();
        //return this.validateSelectedForm(formName);
    }

    return $(".main form").valid();
    //return this.validateMainForm();
}

SimDms.Widget.prototype.validateMainForm = function () {
    var validation = true;
    var form = $('.page .main form');
    form = $(form[0]);
   
    var inputs = form.children('div.panel').children('div').children('div').children('input');
    var lookupInputs = form.children('div').children('div').children('div').children('div').children('div').children('div').children('input');
    var lookupInputs2 = form.children('div').children('div').children('div').children('div').children('div').children('input');
    console.log(inputs);
    console.log(lookupInputs);
    console.log(lookupInputs2);

    if (validation == true) {
        $.each(function (key, val) {
            console.log(val);
        });
    }

    return validation;
}

SimDms.Widget.prototype.validateSelectedForm = function () {

}

SimDms.Widget.prototype.clearValidation = function (formName) {
    var selector = "";

    if (this.isNullOrEmpty(formName) == false) {
        selector += "form[name='" + formName + "'] ";
    }
    else {
        selector += "form ";
    }

    $(selector + "label.error").hide();
    $(selector + ".error").removeClass("error");
}

SimDms.Widget.prototype.enabled = function (options) {
    /*
    var options = {
	    elements: [
            { name: "", type: "default is [input]", status: true or false }
        ],
        status: true or false
    }
    */

    var _this = this;

    if (_this.isNullOrEmpty(options) == false) {
        $.each(options.elements, function (key, val) {
            _this.getObject(val.name, val.type || "input").prop("disabled", val.status || options.status);
        });
    }
}

SimDms.Widget.prototype.enableElement = function (options) {
    /*
        options = [
            { name: "", type: "", status: "true || false" }
        ]
    */

    var _this = this;

    if (_this.isNullOrEmpty(options) == false) {
        $.each(options, function (key, val) {
            var itemObject = _this.getObject(val.name, val.type || "input");

            if (val.status == false) {
                itemObject.attr("disabled", true);
            }
            else {
                itemObject.removeAttr("disabled");
            }
        });
    }
}

SimDms.Widget.prototype.wrappingElement = function (options) {
    if (this.isNullOrEmpty(options) == false) {
        var mainForm = $(".gl-widget").children().children();
        mainForm.unwrap();
        $(options.selector).wrap(options.wrapper);
    }
}

SimDms.Widget.prototype.imageSettings = {};

SimDms.Widget.prototype.setImageSettings = function (options) {
    var _this = this;

    if (_this.isNullOrEmpty(options) == false) {
        _this.imageSettings[options.name] = options;
    }
}

SimDms.Widget.prototype.initializeImagesInput = function () {
    var _this = this;
    $.each(_this.imageSettings, function (key, val) {
        var itemObject = _this.getObject(val.name, "image");
        $.each(val.events, function (key2, val2) {
            itemObject.on(val2.eventType, function (evt) {
                val2.event(evt);
            });
        });
    });
}

SimDms.Widget.prototype.setItems = function (options) {
    var _this = this;

    if (_this.isNullOrEmpty(options) == false) {

        var html = "<option value=''>" + (options.optionalText || SimDms.selectOneText) + "</option>";
        $.each(options.data, function (key, val) {
            html += "<option value='" + val.value + "'>" + val.text + "</option>";
        });

        var itemObject = _this.getObject(options.name, (options.type || "input"));
        itemObject.html(html);
    }
}

SimDms.Widget.prototype.showInputElement = function (options) {
    var _this = this;
    if (_this.isNullOrEmpty(options) == false) {
        var itemObject;

        switch (options.type) {
            case "controls":
                itemObject = _this.getObject(options.name, (options.type || "input")).parent();
                break;

            case "switch":
                itemObject = _this.getObject(options.name, (options.type || "input")).parent().parent().parent();
                itemObject = $("input[name='" + (options.name || "") + "']").parent().parent().parent();

                break;
            default:
                itemObject = _this.getObject(options.name, (options.type || "input")).parent().parent();
                break;
        }

        if (options.visible == false) {
            itemObject.hide();
        }
        else {
            itemObject.show();
            itemObject.css({
                "display": "block"
            });

            setTimeout(function () {
                itemObject.show();
                itemObject.css({
                    "display": "block"
                });
            }, 500);
        }
    }
}

SimDms.Widget.prototype.export = function (options) {
    /*
        options = {
            name: "",
            role: "",  default value is "grid"
            exportType: "" -> default value is "excel"
        };
    */

    var _this = this;

    if (_this.isNullOrEmpty(options) == false) {
        var result = "";
        var fileHeader = "";
        var element = $("#" + (options.name || ""));
        options.role = options.role || "grid";
        var dataSource;
        var data;

        switch (options.exportType) {
            case "excel":
                fileHeader = "data:application/vnd.ms-excel,";
                break;

            default:
                fileHeader = "data:application/vnd.ms-excel,";
                break;
        }

        var isKendo = false;
        var isKGrid = element.hasClass("k-grid");
        var isHasGridDataRole = element.attr("data-role") == ((options.role) || "grid") ? true : false;

        if (isKGrid && isHasGridDataRole) {
            var header = element.children(".k-grid-header").children(".k-grid-header-wrap").children("table").children("thead").children("tr").children("th");
            var columns = new Array();
            switch (options.role) {
                case "grid":
                    //console.log("File Header : " + fileHeader);

                    result += fileHeader;
                    result += "<table>";

                    //Generate header
                    //result += "<thead>";
                    result += "<tr>";
                    $.each(header, function (key, val) {
                        result += "<th>" + ($(val).attr("data-title") || "") + "</th>";
                        columns.push(($(val).attr("data-title") || ""));
                    });
                    result += "</tr>";
                    //result += "</thead>";

                    dataSource = element.data("kendoGrid").dataSource;
                    data = dataSource.data();

                    var strExecOuter = "";
                    var strExecInner = "";
                    //result += "<tbody>";

                    $.each(data, function (key1, val1) {
                        result += "<tr>";
                        $.each(columns, function (key2, val2) {
                            result += "<td>";
                            result += val2;
                            result += "</td>";
                        });
                        result += "</tr>";
                    });
                    //result += "</tbody>";

                    result += "</table>";

                    if (window.navigator.msSaveBlob) {
                        window.navigator.msSaveBlob(new Blob([result]), 'export.csv');
                    } else {
                        window.open(result);
                    }
                    break;

                default:

                    break;
            }

        }
    }
}

SimDms.Widget.prototype.initializePopupContainer = function () {
    var overlay = $(".overlay");
    var popupContainer = $(".popup-frame.outer");

    overlay.on("click", function () {
        var cropAreaSelection = $("body > div.imgareaselect-outer, div.imgareaselect-border4, div.imgareaselect-border3, div.imgareaselect-border2, div.imgareaselect-border1, div.imgareaselect-selection, .imgareaselect-outer");
        popupContainer.fadeOut();
        overlay.fadeOut();
        cropAreaSelection.fadeOut();
    });
}

SimDms.Widget.prototype.showCustomPopup = function (param1, param2, param3) {
    /*
        param1 = html content
        param2 :    - isCustomSize (true / false)
                    - callback function
        param3 :    - callback function
    */

    var _this = this;
    var popupContainer = $(".popup-frame.outer");
    var popupElementContainer = $(".popup-frame.outer > .popup-frame.inner");
    var overlay = $(".overlay");

    overlay.fadeIn();
    popupContainer.fadeIn();
    popupElementContainer.html(param1);
    popupElementContainer.show();

    if (arguments.length == 2) {
        if (typeof param2 == "function") {
            param2();
        }

        if ((typeof param2 != "function" && param2.status == false) || typeof param2 == "function") {
            _this.setPopupSize(popupContainer);
        }

        //_this.setPopupSize(popupContainer);
    }

    if (arguments.length == 3) {
        if (param2.status == false) {
            _this.setPopupSize(popupContainer);
        }
        else {
            _this.setPopupSize(popupContainer, param2);
        }
        if (typeof param3 == "function") {
            param3();
        }
    }
}

SimDms.Widget.prototype.popupContainer;

SimDms.Widget.prototype.setPopupSize = function (popupContainer, paramSize) {
    var _this = this;


    if (_this.isNullOrEmpty(paramSize) == false && paramSize.status == true) {
        SimDms.popupHeight = paramSize.height;
        SimDms.popupWidth = paramSize.width;
    }

    var screenHeight = $(window).height();
    var screenWidth = $(window).width();
    var popupHeight = SimDms.popupHeight;
    var popupWidth = SimDms.popupWidth;
    var left = (screenWidth - popupWidth) / 2;
    var top = (screenHeight - popupHeight) / 2;

    popupContainer.css({
        "position": "absolute",
        "left": left + "px",
        "top": top + "px",
        "height": popupHeight + "px",
        "width": popupWidth + "px"
    });
}

SimDms.Widget.prototype.hideCustomPopup = function () {
    var popupContainer = $(".popup-frame.outer, .overlay, div.imgareaselect-outer, div.imgareaselect-border4");
    popupContainer.fadeOut();
}

var widgetInstance;

SimDms.Widget.prototype.uploadFileEvents = {};

SimDms.Widget.prototype.uploadFile = function (options) {
    /*
        options = {
	        url: "",
            formData: FormData,
            events: {
	            progress: function(progress){},
                complete: function() {},
                error: function() {}
            }
        }
    */

    var _this = this;
    widgetInstance = this;

    _this.uploadFileEvents = options.events;

    $.ajax({
        url: options.url,
        data: options.data,
        type: "POST",
        cache: false,
        contentType: false,
        processData: false,
        xhr: function () {
            var __this = _this;
            myXhr = $.ajaxSettings.xhr();
            if (myXhr.upload) {
                myXhr.upload.addEventListener('progress', __this.uploadProgress, false);
            }
            return myXhr;
        },
        complete: function (result) {
            options.events.complete(result);
        },
        success: function (result) {
            options.events.success(result);
        },
        error: function (a, b, c) {
            options.events.error(a, b, c);
        }
    });
}

SimDms.Widget.prototype.uploadProgress = function (evt) {
    var _this = this;
    var progress = 0;
    if (evt.lengthComputable) {
        progress = evt.loaded * 100 / evt.total;
    }

    widgetInstance.uploadFileEvents.progress(progress);
}

SimDms.Widget.prototype.overrideElementSettings = function () {
    var _this = this;
    var rawHash = window.location.hash.replace("#lnk/", "");
    var url = "Settings/OverriddenSettings";
    var params = {
        HashLink: rawHash
    };

    _this.post(url, params, function (result) {
        if (_this.isNullOrEmpty(result) == false && result.success == true) {
            $.each(result.data || [], function (key, val) {
                var element = $("[name='" + (val.FieldID || "") + "']");
                var rootWrapper;
                var label;

                switch (val.Type) {
                    case "text":
                        rootWrapper = element.parent("div").parent("div");
                        label = rootWrapper.children("label");
                        break;

                    case "select":
                        rootWrapper = element.parent("div").parent("div");
                        label = rootWrapper.children("label");
                        break;

                    case "popup":
                        rootWrapper = element.parent("div.popup-wrapper").parent("div").parent("div.controls-wrapper").parent("div").parent("div");
                        label = rootWrapper.children("label");
                        break;

                    case "tabs":
                        rootWrapper = $("[data-name='" + (val.FieldID || "") + "']");
                        label = rootWrapper.children("a");
                        break;

                    default:

                        break;
                }


                if (label.length > 0) {
                    label.text(val.Title || "");
                }

                var isVisible = false;
                switch (val.Visibility) {
                    case 0:
                        isVisible = false;
                        break;
                    case 1:
                        isVisible = true;
                        break;
                }

                if (rootWrapper.length > 0 && isVisible == false) {
                    rootWrapper.remove();
                }
            });
        }
    });
}

SimDms.Widget.prototype.export = function (options) {
    var _this = this;

    if (_this.isNullOrEmpty(options) == false) {
        var url = options.url;
        window.location = url;
    }
}

SimDms.Widget.prototype.showCornerNotification = function (message) {
    var wrapper = $(".notification-corner-wrapper");
    var obj = $(".notification-corner-wrapper > .notification-corner");
    obj.html("<strong>" + message + "</strong>");
    wrapper.fadeIn();
    obj.show();
}

SimDms.Widget.prototype.hideCornerNotification = function () {
    var obj = $(".notification-corner-wrapper");
    obj.fadeOut();
}