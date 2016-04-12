SimDms.Widget.prototype.kgrid = function (options, callback) {
    var _this = this;
    var url = options.url;
    var params = options.params || {};
    var serverBinding = (options.serverBinding || false);
    var kds = {};
    var toolbars = (options.toolbars == undefined) ? undefined : "<div class='k-toolbars'>" + this.generateButtons(options.toolbars) + "</div>";

    $(".page .ajax-loader").fadeIn();

    if (serverBinding) {
        kds = new kendo.data.DataSource({
            serverPaging: true,
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
                total: "total"
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
        groupable: (options.groupable === undefined ? false : options.groupable),
        sortable: (options.sortable === undefined ? true : options.sortable),
        filterable: (options.filterable === undefined ? { extra: false, operators: { string: { contains: "Contains" } } } : options.filterable),
        navigatable: (options.navigatable === undefined ? true : options.navigatable),
        //pageable: (options.pageable === undefined ? { pageSizes: [5, 10, 20, 50, 100, 500, 1000] } : options.pageable),
        pageable: (options.pageable === undefined ? true : options.pageable),
        change: (options.onChange || function () { }),
        dataBinding: (options.dataBinding || function () { $(".page .ajax-loader").fadeOut(); }),
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

            $(".page .ajax-loader").fadeOut();
        }),
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
            setTimeout(function () {
                kds.read();
            }, 500);
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

    function showLookup() {
        var kloolup = {
            kds: undefined,
            evtDblClick: [],
            dblClick: function (callback) {
                this.evtDblClick.push(callback);
            }
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
                if (_this.isNullOrEmpty(val.value) == false) {
                    params[(val.name || "")] = val.value;
                }
                else {
                    params[(val.name || "")] = $("[name='" + (val.element || "") + "']").val();
                }
            });
        }

        console.log(params);

        var template = "<div class=\"panel klookup\">" +
                       "<div class=\"header\">" +
                       "<div class=\"title\">" + options.title + "</div>" +
                       "<div class=\"buttons\">" +
                       "<button class=\"button small\" id=\"btnClosePanel\"><i class=\"icon icon-hand-right\"></i></button>" +
                       "</div>" +
                       "</div>" +
                       "<div class=\"content kgrid\">" +
                       "<div id=\"" + gridName + "\"></div>" +
                       "</div>" +
                       "<div class=\"footer\"><div class=\"buttons\">" +
                       "<button class=\"button small\" id=\"btnSelectData\"><i class=\"icon icon-location-arrow\"></i> Select</button>" +
                       "<button class=\"button small\" id=\"btnCancelPanel\"><i class=\"icon icon-undo\"></i> Cancel</button>" +
                       "</div></div>" +
                       "</div>";

        $(".body > .panel.lookup").empty();
        $(".body > .panel.lookup").html(template);
        $(".body > .panel").fadeIn("slow");

        function dataBound(e) {
            console.log('data bound called');

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
            });
        }

        if (options.serverBinding) {
            kloolup.kds = new kendo.data.DataSource({
                serverPaging: true,
                serverFiltering: true,
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
        console.log('before grid');
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
        });

        console.log('after grid');

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
                        htmlOptions += "<option value='" + (val.value || "") + "'>" + (val.text || "") + "</option>";
                    });

                    html += "<select id='" + (item.name || "") + "' name='" + (item.name || "") + "'>" + htmlOptions + "</select>";

                    break;

                default:
                    html += "<input type=\"text\"" + attr + "></input>\n";
                    break;
            }
            return html;
        }

        return kloolup;
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

SimDms.Widget.prototype.lineChart = function (options) {
    console.log(options.series);
    $(options.selector).kendoChart({
        title: {
            text: options.title,
            font: (options.font || '30px sans-serif'),
        },
        legend: {
            position: 'bottom'
        },
        seriesDefaults: {
            type: 'line',
        },
        series: options.series,
        valueAxis: {
            labels: {
                format: (options.axisformat || "{0}%"),
                template: "#= FormatLongNumber(value) #"
            },
            line: {
                visible: false
            },
            axisCrossingValue: (options.axiscrossing || -10)
        },
        categoryAxis: {
            categories: options.axis,
            majorGridLines: {
                visible: false
            }
        },
        tooltip: {
            visible: true,
            format: '{0}%',
            template: '#= series.name #: #= FormatToCurrency(value) #'
        }
    });
}

SimDms.Widget.prototype.lineChartMultiAxis = function (options) {
    $(options.selector).kendoChart({
        title: {
            text: options.title,
            font: (options.font || '30px sans-serif'),
        },
        legend: {
            position: 'bottom'
        },
        seriesDefaults: {
            type: 'line',
        },
        series: options.series,
        valueAxes: options.valueAxes,
        panes: options.panes,
        categoryAxis: {
            categories: options.axis,
            axisCrossingValues: [0, 12],
            justified: true
        },
        tooltip: {
            visible: true,
            format: '{0}%',
            template: '#= series.name #: #= FormatToCurrency(value) #'
        }
    });
}

SimDms.Widget.prototype.tableChart = function (options) {
    var self = this;
    var html = '';
    html += '<table>';
    html += '<thead>';
    html += '<tr>';
    html += '<th class="info">' + (options.name || 'Chart') + '</th>';
    for (var i = 0; i < (options.cols || []).length; i++) {
        html += '<th style="' + (options.hstyle || 'font-size:12px') + '">' + options.cols[i] + '</th>';
    }
    html += '</tr>';
    html += '</thead>';
    html += '<tbody>';
    (options.series || []).forEach(function (raws) {
        html += '<tr>';
        html += '<th style="' + (options.lstyle || 'font-size:12px;text-align:left') + '">' + raws.name + '</th>';
        for (var i = 0; i < (options.cols || []).length; i++) {
            if (options.format) {
                html += '<td style="' + (options.bstyle || 'font-size:10px;text-align:right') + '">' + ((raws.data[i] == undefined) ? '' : self.numberFormat(raws.data[i])) + '</td>';
            }
            else {
                html += '<td style="' + (options.bstyle || 'font-size:10px;text-align:right') + '">' + ((raws.data[i] == undefined) ? '' : raws.data[i]) + '</td>';
            }
        }
        html += '</tr>';
    });
    html += '</tbody>';
    html += '</table>';

    $(options.selector).html(html);
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
        }
    });
}

SimDms.Widget.prototype.radialGauge = function (options) {
    $(options.selector).kendoRadialGauge({

        pointer: {
            value: options.pointervalue
        },

        scale: {
            minorUnit: 5,
            startAngle: -30,
            endAngle: 210,
            max: options.max || 180,
            labels: {
                position: options.labelposition || "inside"
            },
            ranges: [
                {
                    from: 0,
                    to: 20,
                    color: "#c20000"
                },
                {
                    from: 20,
                    to: 40,
                    color: "#ff7a00"
                },
                {
                    from: 40,
                    to: 100,
                    color: "#ffc700"
                },
                {
                    from: 100,
                    to: 120,
                    color: "#BEFF00"
                }, {
                    from: 120,
                    to: 150,
                    color: "#5EFF00"
                }, {
                    from: 150,
                    to: 180,
                    color: "#05C200"
                }, {
                    from: 180,
                    to: options.max,
                    color: "#00C23D"
                }
            ]
        }
    });
}

SimDms.Widget.prototype.panelBar = function (options) {
    var html = '<ul id=\"' + options.name + '\" class=\"' + (options.cls || "") + '\">';
    var items = options.items;

    $.each(items || [], function (idx, item) {
        html += "<li class=\"k-state-active\">" + item.text;
        html += '<ul id=\"' + item.name + '\">'
        if (item.subitems.length) {
            if (item.colWidth === undefined) {
                item.colWidth = ["10%", "50%", "40%"];
            }
            $.each(item.subitems || [], function (idx, subitem) {
                html += "<li style=\"display: flex;\">";
                html += "<span class=\"k-link\" style=\"width:" + item.colWidth[0] + "\">" + (idx + 1) + ". </span> ";
                html += "<span class=\"k-link\" style=\"width:" + item.colWidth[1] + "\">" + (subitem.text || "") + "</span>";
                html += "<span class=\"k-link\" style=\"width:" + item.colWidth[2] + "\">" + (FormatToCurrency(subitem.value, '', false) || 0) + "</span>";
                html += "</li>";
            });
        }
        html += "</ul>"
        html += "</li>";
    });

    html += '</ul>';

    $(options.selector).html(html);

    var panelBar = $("#" + options.name).kendoPanelBar().data("kendoPanelBar");
}

SimDms.Widget.prototype.panelBarItem = function (options) {
    var html = '';
    var items = options.items;

    if (options.colWidth === undefined) {
        options.colWidth = ["10%", "50%", "40%"];
    }
    $('#' + options.name).html(html);
    $.each(items || [], function (idx, item) {
        html += "<li style=\"display: flex;\">";
        html += "<span class=\"k-link\" style=\"width:" + options.colWidth[0] + "\">" + (idx + 1) + ". </span> ";
        html += "<span class=\"k-link\" style=\"width:" + options.colWidth[1] + "\">" + (item.text || "") + "</span>";
        html += "<span class=\"k-link\" style=\"width:" + options.colWidth[2] + "\">" + (FormatToCurrency(item.value, '', false) || 0) + "</span>";
        html += "</li>";
        $('#' + options.name).append(html);
        html = '';
    });

    var panelBar = $("#" + options.panelbar).kendoPanelBar().data("kendoPanelBar");
}

function FormatToCurrency(num, currency, flagCent) {
    num = num.toString().replace(/\$|\,/g, '');
    if (isNaN(num))
        num = "0";
    sign = (num == (num = Math.abs(num)));
    num = Math.floor(num * 100 + 0.50000000001);
    cents = num % 100;
    num = Math.floor(num / 100).toString();
    if (cents < 10)
        cents = "0" + cents;
    for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3) ; i++)
        num = num.substring(0, num.length - (4 * i + 3)) + ',' + num.substring(num.length - (4 * i + 3));
    return (flagCent) ? (((sign) ? '' : '-') + (currency || "") + num + '.' + cents) : (((sign) ? '' : '-') + (currency || "") + num);
}

function FormatLongNumber(value) {
    if (value == 0) {
        return 0;
    }
    else {
        // hundreds
        if (value <= 999) {
            return value;
        }
            // thousands
        else if (value >= 1000 && value <= 999999) {
            return (value / 1000) + 'K';
        }
            // millions
        else if (value >= 1000000 && value <= 999999999) {
            return (value / 1000000) + 'M';
        }
            // billions
        else if (value >= 1000000000 && value <= 999999999999) {
            return (value / 1000000000) + 'B';
        }
        else
            return value;
    }
}
