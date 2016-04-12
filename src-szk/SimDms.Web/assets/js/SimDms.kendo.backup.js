SimDms.Widget.prototype.kgrid = function (options, callback) {
    var url = options.url;
    var params = options.params || {};
    var serverBinding = (options.serverBinding || false);

    if (serverBinding) {
        var kds = new kendo.data.DataSource({
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

        $(".kgrid #" + options.name).empty();
        var oGrid = $(".kgrid #" + options.name).kendoGrid({
            dataSource: kds,
            detailTemplate: options.detailTemplate,
            groupable: (options.groupable === undefined ? false : options.groupable),
            sortable: (options.sortable === undefined ? true : options.sortable),
            filterable: (options.filterable === undefined ? { extra: false, operators: { string: { contains: "Contains" } } } : options.filterable),
            navigatable: (options.navigatable === undefined ? true : options.navigatable),
            pageable: (options.pageable === undefined ? { pageSizes: [5, 10, 20, 50, 100] } : options.pageable),
            dataBinding: (options.dataBinding || function () { }),
            dataBound: (options.dataBound || function () { }),
            columns: options.columns,
            detailInit: options.detailInit,
            selectable: options.selectable,
            columnMenu: true,
            reorderable: true,
        });

        oGrid.refresh = function (params) {
            if (kds !== undefined) {
                kds.options.transport.read.data = params;
                kds.read();
            }
        }

        return oGrid;
    }
    else {
        this.post(url, params, function (data) {
            $(".kgrid #" + options.name).empty();
            var oGrid = $(".kgrid #" + options.name).kendoGrid({
                dataSource: {
                    data: data,
                    pageSize: (options.pageSize || 10),
                    group: options.group,
                    aggregate: options.aggregate,
                    sort: options.sort,
                },
                detailTemplate: options.detailTemplate,
                groupable: (options.groupable === undefined ? false : options.groupable),
                sortable: (options.sortable === undefined ? true : options.sortable),
                filterable: (options.filterable === undefined ? { extra: false, operators: { string: { contains: "Contains" } } } : options.filterable),
                navigatable: (options.navigatable === undefined ? true : options.navigatable),
                pageable: (options.pageable === undefined ? { pageSizes: [5, 10, 20, 50, 100] } : options.pageable),
                dataBound: (options.dataBound || function () { }),
                columns: options.columns,
                detailInit: options.detailInit,
                selectable: options.selectable,
                columnMenu: true,
                reorderable: true,
            });
        });
    }
}

SimDms.Widget.prototype.chart = function (options, callback) {
    var url = options.url;
    var params = options.params || {}
    var legend = options.legend || { position: "bottom" };
    this.post(url, params, function (result) {
        $(".chart-wrapper #" + options.name).empty();
        $(".chart-wrapper #" + options.name).kendoChart({
            title: { text: (options.title || result.title) || "" },
            legend: legend,
            seriesDefaults: {
                labels: {
                    visible: true,
                    background: "transparent",
                    template: (options.template || result.template) || "#= category #: #= value#",
                },
                type: (options.type || result.type) || "pie",
            },
            series: result.series
        });

        if (callback !== undefined) {
            callback("configured");
        }
    })
}

SimDms.Widget.prototype.klookup = function (options, callback) {
    var kloolup = {
        evtDblClick: [],
        dblClick: function (callback) {
            this.evtDblClick.push(callback);
        }
    };

    var template = "<div class=\"panel klookup\">" +
                   "<div class=\"header\">" +
                   "<div class=\"title\">" + options.title + "</div>" +
                   "<div class=\"buttons\">" +
                   "<button class=\"button small\" id=\"btnClosePanel\"><i class=\"icon icon-hand-right\"></i></button>" +
                   "</div>" +
                   "</div>" +
                   "<div class=\"content kgrid\">" +
                   "<div id=\"" + options.name + "\"></div>" +
                   "</div>" +
                   "<div class=\"footer\"><div class=\"buttons\">" +
                   "<button class=\"button small\" id=\"btnSelectData\"><i class=\"icon icon-location-arrow\"></i> Select</button>" +
                   "<button class=\"button small\" id=\"btnCancelPanel\"><i class=\"icon icon-undo\"></i> Cancel</button>" +
                   "</div></div>" +
                   "</div>";

    $(".body > .panel").empty();
    $(".body > .panel").html(template);
    $(".body > .panel").fadeIn("slow");

    function dataBound(e) {
        var grid = $(".kgrid #" + options.name).data("kendoGrid");

        if (grid !== undefined) {
            var row = undefined;
            $(".k-grid-content tr").on("click", function () { row = $(this); });
            $(".k-grid-content tr").on("dblclick", selectData);
            $(".body > .panel").find("#btnSelectData").on("click", selectData);
            $(".body > .panel").find("#btnCancelPanel,#btnClosePanel").on("click", hide);
        }

        function hide() {
            $(".body > .panel").fadeOut();
        }

        function selectData() {
            if (row !== undefined) {
                var data = grid.dataItem(row);
                for (var i = 0; i < kloolup.evtDblClick.length; i++) {
                    kloolup.evtDblClick[i](data);
                    hide();
                }
            }
        }
    }

    this.kgrid($.extend(options,
        {
            selectable: false,
            navigatable: true,
            dataBound: dataBound,
            pageable: { pageSize: 15, pageSizes: [5, 10, 15, 20, 50, 100] }
        }));

    return kloolup;
}