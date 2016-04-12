SimDms.Widget.prototype.kgrid = function (options, callback) {
    var _this = this;
    var url = options.url;
    var params = options.params || {};
    var serverBinding = (options.serverBinding || false);
    var kds = {};
    var toolbars = (options.toolbars == undefined) ? undefined : "<div class='k-toolbars'>" + this.generateButtons(options.toolbars) + "</div>";

    if (serverBinding) {
        kds = new kendo.data.DataSource({
            serverPaging: true,
            serverFiltering: (options.isServerFiltering == undefined) ? true : options.isServerFiltering,
            serverFiltering: true,
            serverSorting: true,
            pageSize: (options.pageSize || 10),
            group: options.group,
            aggregate: options.aggregate,
            sort: options.sort,
            transport: {
                read: {
                    url: SimDms.baseUrl + url,
                    dataType: "json",
                    type: "POST",
                    data: params
                },
            },
            schema: {
                data: "data",
                total: "total",
                model: options.model || {}
            },
        });
    }
    else {
        if (options.url !== undefined) {
            kds = new kendo.data.DataSource({
                pageSize: (options.pageSize || 10),
                group: options.group,
                aggregate: options.aggregate,
                sort: options.sort,
                transport: {
                    read: {
                        url: SimDms.baseUrl + url,
                        dataType: "json",
                        type: "POST",
                        data: params
                    },
                },
            });
        }
        else if (options.data !== undefined) {
            kds = new kendo.data.DataSource({
                pageSize: (options.pageSize || 10),
                group: options.group,
                aggregate: options.aggregate,
                sort: options.sort,
                data: options.data
            });
        }
        else {
            kds = new kendo.data.DataSource({
                pageSize: (options.pageSize || 10),
                group: options.group,
                aggregate: options.aggregate,
                sort: options.sort,
                data: []
            });
        }
    }
    
    (options.columns || []).forEach(function (column) {
        if (column.type == 'date') {
            column.template = '#= (' + column.field + ' == undefined || (moment(' + column.field + ').format("YYYYMMDD") <= "19000101")) ? "-" : moment(' + column.field + ').format("DD MMM YYYY") #'
        }
        else if (column.type == 'datetime') {
            column.template = '#= (' + column.field + ' == undefined || (moment(' + column.field + ').format("YYYYMMDD") <= "19000101")) ? "-" : moment(' + column.field + ').format("DD MMM YYYY HH:mm:ss") #'
        }
        else if (column.type == 'decimal') {
            column.template = '<div class="right">#= ((' + column.field + ' == undefined) ? "" : number_format(' + column.field + ', 2)) #</div>'
        }
        else if (column.type == 'number') {
            column.template = '<div class="right">#= ((' + column.field + ' == undefined) ? "" : number_format(' + column.field + ', 0)) #</div>'
        }
        else if (column.type == 'align-right') {
            column.template = '<div class="right">#= ' + column.field + ' #</div>';
        }
        else if (column.type == 'align-center') {
            column.template = '<div class="center">#= ' + column.field + ' #</div>';
        }
    });

    $(".kgrid #" + options.name).empty();
    var oGrid = $(".kgrid #" + options.name).kendoGrid({
        dataSource: kds,
        detailTemplate: options.detailTemplate,
        change:(options.change || undefined ),         
        groupable: (options.groupable === undefined ? false : options.groupable),
        sortable: (options.sortable === undefined ? true : options.sortable),
        filterable: (options.filterable === undefined ? { extra: false, operators: { string: { contains: "Contains" }, date: { gte: "Start", lte: "End" } } } : options.filterable),
        navigatable: (options.navigatable === undefined ? true : options.navigatable),
        pageable: (options.pageable === undefined ? { pageSizes: [5, 10, 20, 50, 100, 500, 1000] } : options.pageable),
        dataBinding: (options.dataBinding || function () { }),
        dataBound: (options.dataBound || function () {
            $(".kgrid #" + options.name + " .k-grid-content tr").on("dblclick", function () {
                if (options.onDblClick !== undefined) {
                    options.onDblClick();
                }
            });

            if (options.onDblClick !== undefined) {
                //console.log("ondblclick");
            }

            if (options.onComplete !== undefined) {
                options.onComplete();
            }

        }),
        group: options.group,
        columns: options.columns,
        toolbar: (options.toolbar || toolbars),
        detailInit: options.detailInit,
        //selectable: options.selectable,
        selectable: "multiple",
        columnMenu: true,
        reorderable: true,
    });

    oGrid.refresh = function (params) {
        if (kds !== undefined) {
            kds.options.transport.read.data = params;
            kds.read();
        }
    }

    setTimeout(function () {
        if (_this.isNullOrEmpty(callback) == false && typeof callback == "function") {
            callback("configured");
        }
    }, 1000);

    return oGrid;
}

SimDms.Widget.prototype.klookup = function (options, callback) {
    var _this = this;
    var _init = true;
    if (typeof options.onSelected == "function") {
        var selector = (options.selector || ("#" + options.name));
        $(selector).on("click", showLookup);

        if (options.controller !== undefined) {
            var _url = options.controller.url;
            var _value = "#" + options.controller.value;
            var _text = "#" + options.controller.text;

            $(_value).on("change", function () {
                _this.post(_url, { id: $(this).val() }, function (result) {
                    $(_text).val(result.data);
                    if (!result.success) {
                        $(_value).val("");
                    }
                });
            });
        }
    }
    else {
        return showLookup();
    }

    function showLookup()
    {
        var kloolup = {
            kds: undefined,
            evtDblClick: [],
            dblClick: function (callback) {
                this.evtDblClick.push(callback);
            },
            afterRenderKlookup: renderKlookupDatepicker()
        };

        var trandom = "";
        var possible = "abcdefghijklmnopqrstuvwxyz";
        for (var i = 0; i < 5; i++) trandom += possible.charAt(Math.floor(Math.random() * possible.length));

        var gridName = "lkukgrid" + trandom;
        var params = {};
        if (options.params !== undefined) {
            if (options.params.name == "controls" && options.params.items !== undefined) {
                $.each(options.params.items, function (idx, val) {
                    params[val.param] = $("[name=" + val.name + "]").val();
                });
            }
            else {
                for (var key in options.params) {
                    if (key.substr(0, 3) !== "flt") {
                        params[key] = options.params[key];
                    }
                }
            }
        }
        else {
            params = $("#" + gridName + ".kgrid").find(".k-filter").serializeObject();
        }

        if (_this.isNullOrEmpty(options.dynamicParams) == false) {
            $.each(options.dynamicParams || [], function (key, val) {
                if (_this.isNullOrEmpty(val.value)==false) {
                    params[(val.name || "")] = val.value;
                }
                else {
                    params[(val.name || "")] = $("[name='" + (val.element || "") + "']").val();
                }
            });
        }
        var template = "<div class=\"panel klookup\">" +
                       "<div class=\"header\">" +
                       "<div class=\"title\">" + options.title + "</div>" +
                       "<div class=\"buttons\">" +
                       "<button class=\"button small btn btn-info\" id=\"btnClosePanel\"><i class=\"icon icon-hand-right\"></i></button>" +
                       "</div>" +
                       "</div>" +
                       "<div class=\"content kgrid\">" +
                       "<div id=\"" + gridName + "\"></div>" +
                       "</div>" +
                       "<div class=\"footer\"><div class=\"buttons\">" +
                       "<button class=\"button small btn btn-success\" id=\"btnSelectData\"><i class=\"icon icon-location-arrow\"></i> Select</button>" +
                       "<button class=\"button small btn btn-danger\" id=\"btnCancelPanel\"><i class=\"icon icon-undo\"></i> Cancel</button>" +
                       "</div></div>" +
                       "</div>";

        $(".body > .panel.lookup").empty();
        $(".body > .panel.lookup").html(template);
        $(".body > .panel").fadeIn("slow");

        function dataBound(e) {
            var grid = $(".kgrid #" + gridName).data("kendoGrid");

            if (grid !== undefined) {
                var row = undefined;
                $(".k-grid-content tr").on("click", function () { row = $(this); });
                $(".k-grid-content tr").on("dblclick", selectData);
                $(".body > .panel").find("#btnSelectData").on("click", selectData);
                $(".body > .panel").find("#btnCancelPanel,#btnClosePanel").on("click", hide);
            }

            function hide() {
                $(".body > .panel").fadeOut();
                $(".body > .panel .kgrid").find(".k-filter input").val("");
            }

            function selectData() {
                if (row !== undefined) {
                    var data = grid.dataItem(row);
                    for (var i = 0; i < kloolup.evtDblClick.length; i++) {
                        kloolup.evtDblClick[i](data);
                        hide();
                    }

                    if (typeof options.onSelected == "function") {
                        options.onSelected(data);
                        hide();
                    }

                    if (options.onSelectedRows !== undefined) {
                        var rows = $(".kgrid #" + gridName + " tbody tr.k-state-selected");
                        var data = [];
                        $.each(rows, function (idx, val) {
                            data.push(grid.dataItem(val));
                        });

                        options.onSelectedRows(data);
                        hide();
                    }
                }
            }

            $(".kgrid #" + gridName).find(".k-filter input, .k-filter select").off();
            $(".kgrid #" + gridName).find(".k-filter input, .k-filter select").on("change", function () {
                $.extend(params, $(".kgrid #" + gridName).find(".k-filter").serializeObject());
                kloolup.kds.read();
                renderKlookupDatepicker();
            });
        }

        if (options.serverBinding) {
            kloolup.kds = new kendo.data.DataSource({
                serverPaging: true,
                serverFiltering: (options.isServerFiltering == undefined) ? true : options.isServerFiltering,
                serverSorting: true,
                pageSize: (options.pageSize || 15),
                group: options.group,
                aggregate: options.aggregate,
                sort: options.sort,
                transport: {
                    read: {
                        url: SimDms.baseUrl + options.url,
                        data: params,
                        dataType: "json",
                        type: "POST",
                    },
                },
                schema: {
                    data: "data",
                    total: "total"
                },
            });
        }

        $(".kgrid #" + gridName).empty();
        var oGrid = $(".kgrid #" + gridName).kendoGrid({
            dataSource: kloolup.kds,
            toolbar: generateFilters(options.filters),
            detailTemplate: options.detailTemplate,
            groupable: (options.groupable === undefined ? false : options.groupable),
            sortable: (options.sortable === undefined ? true : options.sortable),
            filterable: (options.filterable === undefined ? { extra: false, operators: { string: { contains: "Contains" } } } : options.filterable),
            pageable: (options.pageable === undefined ? { pageSizes: [5, 10, 15, 25, 50, 100] } : options.pageable),
            dataBinding: (options.dataBinding || function () { }),
            selectable: ((options.multiSelect || false) ? "multiple" : false),
            columns: options.columns,
            detailInit: options.detailInit,
            dataBound: dataBound,
            navigatable: true,
            columnMenu: true,
            reorderable: true,
			resizable: true,
        });

        function generateFilters(filters) {
            var html = "";

            $.each(filters || [], function (idx, val) {
                var clss = (val.cls === undefined) ? "" : " class='" + val.cls + "'";
                var left = (val.left == undefined) ? "" : " style=\"padding-left:" + val.left + "px\"";
                switch (val.type) {
                    case "controls":
                        html += "<div" + clss + ">\n";
                        html += "<label>" + val.text + "</label>\n";
                        html += "<div" + left + "><div class=\"controls\">";
                        $.each(val.items || [], function () {
                            html += "<div class=\"" + (this.cls || "") + "\">\n";
                            html += generateItem(this);
                            html += "</div>\n";
                        });
                        html += "</div></div>\n";
                        html += "</div>\n";
                        break;
                    default:
                        html += "<div" + clss + ">\n";
                        html += "<label>" + val.text + "</label>\n";
                        html += "<div" + left + ">" + generateItem(val) + "</div>\n";
                        html += "</div>\n";
                        break;
                }
            });

            html = (html.length == 0) ? "" : "<div class=\"k-filter\">\n" + html + "</div>";
            return html;
        }

        function generateItem(item) {
            var html = "";
            var clss = (item.cls == undefined) ? "" : " class='" + item.cls + "'";
            var labl = (item.text == undefined) ? "" : "<label>" + item.text + "</label>\n";
            var plch = " placeHolder=\"" + (item.placeHolder || item.text || "") + "\"";
            var attr = " name=\"" + item.name + "\"" + plch;
            switch (item.type) {
                case "select":
                    var htmlOptions = "";
                    $.each(item.items || [], function (key, val) {
                        var defaultSelected = (val.selected != undefined) ? val.selected : '';
                        htmlOptions += "<option value='" + (val.value || "") + "' " + defaultSelected + ">" + (val.text || "") + "</option>";
                    });

                    html += "<select id='" + (item.name || "") + "' name='" + (item.name || "") + "'>" + htmlOptions + "</select>";

                    break;

                case "datepicker":
                    html += "<div class='datepicker-wrapper'><input type='text' class='datepicker'" + attr + " id='" + item.name + "'/></div>";

                    break;
                default:
                    html += "<input type=\"text\"" + attr + "></input>\n";
                    break;
            }
            return html;
        }

        return kloolup;
    }

    function renderKlookupDatepicker() {
        setTimeout(function () {
            $(".panel.lookup input[type='date']").addClass("datepicker");
            $(".panel.lookup .datepicker").removeClass('hasDatepicker').removeAttr('id').datepicker({
                dateFormat: "dd-M-yy",
                showOtherMonths: true,
                selectOtherMonths: true,
                changeMonth: true,
                changeYear: true,
            });
        }, 250)
    }
}

SimDms.Widget.prototype.chart = function (options, callback) {
    if (options.source == undefined) {
        var url = options.url;
        var params = options.params || {};

        this.post(url, params, function (result) {
            if (result.success) {
                generateChart(result.source);
            }
            generateChart(result);
        });
    }
    else {
        generateChart(options.source);
    }

    function generateChart(source) {
        var data = [];
        var ctgs = [];

        $.each(source, function (idx, val) {
            data.push(val.value);
            ctgs.push(val.name);
        });

        $("#" + options.name).kendoChart({
            legend: { position: "right" },
            seriesDefaults: { type: "column" },
            series: [{ data: data }],
            categoryAxis: { categories: ctgs },
            tooltip: {
                visible: true,
                format: "{0}",
                template: (options.template || "#= value #")
            }
        });
    }
}

SimDms.Widget.prototype.pieChart = function (options) {
    $(options.selector).kendoChart({
        title: options.title,
        legend: {
            position: options.legendPosition
        },
        seriesDefaults: {
            pie: {
                color: "red",
                opacity: 0.7
            },
            labels: {
                template: "#= category # - #= kendo.format('{0:P}', percentage)#",
                position: "outsideEnd",
                visible: true,
                background: "transparent"
            }
        },
        series: [
            { type: "pie", data: options.data/*[1, 2]*/ }
        ],
        tooltip: {
            visible: true,
            template: options.tooltip //"#= category # - #= kendo.format('{0:P}', percentage) #"
        },
        chartArea: (options.chartArea || {})
    });
}
