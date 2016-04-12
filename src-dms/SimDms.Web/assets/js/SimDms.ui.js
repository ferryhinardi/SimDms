var simdms = window.simdms || {};

sdms.hideAjaxLoad = function () {
    $(".page .ajax-loader").fadeOut();
}

sdms.jsonp = function (o) {
    $.ajax({
        type: 'POST',
        url: o.url,
        dataType: 'jsonp',
        crossDomain: true,
        data: o.params,
        success: function (data, status, xhr) {
            if (o.success) o.success(data, status, xhr);
        },
        error: function (res, status, err) {
            if (o.error) o.error(res, status, err);
        },
        complete: function () {
            if (o.complete) o.complete();
        }
    });
}

sdms.post = function (o, callback) {
    $.post(sdms.baseUrl + o.url, o.params, function (result) {
        if (callback) callback(result);
    })
}

sdms.populate = function (o) {
    if (o) {
        for (var key in o) {
            var field = $('[data-field="' + key + '"]');
            switch (field.data('type')) {
                default:
                    field.val(o[key]);
            }
        }
    }
}

sdms.refresh = function (o) {
    if (typeof o == 'string') {
        refresh(o);
    }
    else {
        (o || []).forEach(refresh);
    }

    function refresh(name) {
        var source = sdms.source[name];
        switch (source.type) {
            case 'kgrid':
                sdms.renderKgrid(source);
                break;
            default:
                break;
        }
    }
}

sdms.renderKgrid = function (kgrid) {
    var kds = {}, columns = []; params = undefined;
    $('.page > .ajax-loader').show();

    if (kgrid.filter && typeof kgrid.filter == 'string') {
        params = sdms.serialize(kgrid.filter);
    }

    if (kgrid.data) {
        if (typeof kgrid.data == 'string') {
            sdms.post({ url: kgrid.data, params: params }, function (r) {
                kds = new kendo.data.DataSource({
                    data: r,
                    pageSize: (kgrid.pageSize || 10),
                    group: kgrid.group,
                    aggregate: kgrid.aggregate,
                    sort: kgrid.sort
                });
                populateGrid();
            });
        }
        else {
            kds = new kendo.data.DataSource({
                data: kgrid.data,
                pageSize: (kgrid.pageSize || 10),
                group: kgrid.group,
                aggregate: kgrid.aggregate,
                sort: kgrid.sort
            });
            populateGrid();
        }
    }
    else {
        kds = new kendo.data.DataSource({
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            transport: {
                read: {
                    url: kgrid.url,
                    dataType: 'json',
                    type: 'POST',
                    //data: params
                },
            },
            schema: {
                data: 'data',
                total: 'total'
            },
            pageSize: (kgrid.pageSize || 10),
            group: kgrid.group,
            aggregate: kgrid.aggregate,
            sort: kgrid.sort
        });
        populateGrid();
    };

    function populateGrid() {
        (kgrid.fields || []).forEach(function (field) {
            var column = { field: field.name, title: field.text, width: field.width };

            switch (field.type) {
                case 'int':
                    column.template = '<div class="right">#= ((' + column.field + ' == undefined) ? "" : gelang.numberFormat(' + column.field + ', 0)) #</div>';
                    break;
                case 'decimal':
                    column.template = '<div class="right">#= ((' + column.field + ' == undefined) ? "" : gelang.numberFormat(' + column.field + ', 2)) #</div>';
                    break;
                case 'date':
                    column.template = '#= (' + column.field + ' == undefined || (moment(' + column.field + ').format("YYYYMMDD") <= "19000101")) ? "-" : moment(' + column.field + ').format("DD MMM YYYY") #'
                    break;
                case 'datetime':
                    column.template = '#= (' + column.field + ' == undefined || (moment(' + column.field + ').format("YYYYMMDD") <= "19000101")) ? "-" : moment(' + column.field + ').format("DD MMM YYYY HH:mm:ss") #'
                    break;
                default:
            }

            columns.push(column);
        });

        $('[data-kgrid]').empty().removeClass();
        $('[data-kgrid=' + kgrid.name + ']').kendoGrid({
            dataSource: kds,
            columns: columns,
            detailTemplate: kgrid.detailTemplate,
            groupable: (kgrid.groupable === undefined ? false : kgrid.groupable),
            sortable: (kgrid.sortable === undefined ? true : kgrid.sortable),
            pageable: (kgrid.pageable === undefined ? true : kgrid.pageable),
            selectable: "multiple",
            columnMenu: true,
            reorderable: true,
            dataBinding: function (e) {
                if (kgrid.dataBinding) kgrid.dataBinding(e);
            },
            dataBound: function (e) {
                $('.page > .ajax-loader').hide();
                if (kgrid.dataBound) kgrid.dataBound(e);
            }
        });
    }
}

sdms.serialize = function (o) {
    var object = {};
    var selector = 'body > .page .main';
    if (o && typeof o == 'string') {
        selector += ' #' + o;
    }

    var fields = $(selector).find('[data-field]');
    for (var i = 0; i < fields.length; i++) {
        var field = fields[i];
        object[$(field).data('field')] = $(field).val();
    }
    return object;
}

sdms.ui = function (o) {
    var title_selector = '.page .title h3';
    var toolbar_selector = '.page .toolbar';
    var main_selector = '.page .body .main';
    var sources = [];

    if (o.title) $(title_selector).html(o.title);
    if (o.toolbars) {
        var html = '';
        (o.toolbars || []).forEach(function (val) {
            var action = (val.action == undefined) ? '' : ' data-action="' + val.action + '"';
            html += '<button ' + action + ' class="button small ' + (val.class || '') + '" name="' + val.name + '" id="' + val.name + '"><i class="icon fa ' + (val.icon || '') + '"></i>' + val.text + '</button>';
        });
        $(toolbar_selector).html(html);
    }
    if (o.rows) {
        var html = '<div class="gl-widget">';
        (o.rows || []).forEach(function (row) {
            html += renderRow(row);
        });
        html += '</div>';
        $(main_selector).html(html);
    }

    function renderRow(row) {
        var id = (row.name) ? (' id="' + row.name + '"') : '';
        var html = '<div class="panel"' + id + '>';
        if (row.text) {
            html += '<div class="subtitle">' + row.text + '</div>';
        }
        switch (row.type) {
            case 'kgrid':
                html = '<div class="panel kgrid">';
                html += (row.text) ? '<div class="divider"></div>' : '';
                html += '<div><div data-kgrid="' + row.name + '"></div><div></div>';
                html += '</div>';
                sources.push(row);
                break;
            default:
                (row.fields || []).forEach(function (field) {
                    html += '<div class="' + (field.class || '') + (field.cols ? (' span' + field.cols) : '') + '">' + renderField(field) + '</div>';
                });
                break;
        }
        html += '</div>';
        return html;
    }

    function renderField(field) {
        var html = '';

        var fieldName = ' name="' + field.name + '" id="' + field.name + '" data-field="' + field.name + '"';
        var attribute = (field.readonly ? ' readonly="readonly"' : '') + ' placeholder="' + (field.placeholder || field.text) + '"'
            + ((typeof field.min == 'number') ? (' data-min="' + field.min + '"') : '')
            + ((typeof field.max == 'number') ? (' data-max="' + field.max + '"') : '');

        switch (field.type) {
            case 'popup':
                html += generateLabel(field);
                html += '<div>';
                html += ' <div class="popup-wrapper">';
                html += '  <input type="text"' + fieldName + attribute + '/>';
                html += '  <button type="button" data-action="' + field.name + '" class="button"><i class="fa ' + field.icon + '"></i></button>';
                html += ' </div>';
                html += '</div>';
                break;
            case 'int':
                html += generateLabel(field);
                html += '<div>';
                html += ' <div class="k-number-wrapper">';
                html += '  <input type="text" data-type="int"' + fieldName + attribute + '/>';
                html += ' </div>';
                html += '</div>';
                break;
            case 'select':
                html += generateLabel(field);
                html += '<div>';
                html += '<select ' + fieldName + '><option>--</option></select>';
                html += '</div>';
                if (field.source) sources.push(field);

                break;
            default:
                html += generateLabel(field);
                html += '<div><input type="text"' + fieldName + attribute + '/></div>';
                break;
        }
        return html;
    }

    function generateLabel(field) {
        return '<label for="' + field.name + '">' + field.text + '</label>';
    }

    function bindingSource(source, callback) {
        switch (source.type) {
            case 'kgrid':
                //renderKgrid(source);
                sdms.renderKgrid(source);
                break;
            case 'select':
                break
            default:
                break;
        }
        if (callback) callback();
    }

    sdms.source = {};
    (sources).forEach(function (field) {
        sdms.source[field.name] = field;
        switch (field.type) {
            case 'kgrid':
                sdms.renderKgrid(field);
                break;
            case 'select':
                var source = field.source;
                if (source.url) {
                    if (source.cascade) {
                        $('select#' + field.name).html('<option>' + (source.text || '-- SELECT ONE --') + '</option>');

                        var cascade = field.source.cascade;
                        $('select#' + cascade.source).on('change', function () {
                            source.params = {};
                            source.params[cascade.name] = $('select#' + cascade.source).val();

                            sdms.post(source, function (rows) {
                                var html = (rows.length > 1) ? ('<option>' + (source.text || '-- SELECT ONE --') + '</option>') : '';
                                (rows || []).forEach(function (row) {
                                    html += '<option value="' + row.value + '">' + row.text + '</option>';
                                });
                                $('select#' + field.name).html(html);
                            })
                        });
                    }
                    else {
                        sdms.post(source, function (rows) {
                            var html = '<option>' + (source.text || '-- SELECT ONE --') + '</option>';
                            (rows || []).forEach(function (row) {
                                html += '<option value="' + row.value + '">' + row.text + '</option>';
                            });
                            $('select#' + field.name).html(html);
                        })
                    }
                }
                else if (source.data) {
                    var html = '<option>' + (source.text || '-- SELECT ONE --') + '</option>';
                    (source.data || []).forEach(function (row) {
                        if (typeof row == 'string') {
                            html += '<option value="' + row + '">' + row + '</option>';
                        }
                        else {
                            html += '<option value="' + row.value + '">' + row.text + '</option>';
                        }
                    });
                    $('select#' + field.name).html(html);
                }
                break;
            default:

        }
    });

    // begin kendo controls
    var controls = undefined;
    $('[data-type=kdatepicker]').kendoDatePicker({ format: 'dd-MMM-yyyy' });
    $('[data-type=kmonthpicker]').kendoDatePicker({ start: 'year', depth: 'year', format: 'MMMM yyyy' });
    $('[data-type=ktimepicker]').kendoTimePicker({ format: 'HH:mm' });
    $('[data-type=kdatetimepicker]').kendoDateTimePicker({ format: 'dd-MMM-yyyy HH:mm' });
    $('[data-type=decimal]').kendoNumericTextBox({ format: 'n2', step: 0.01 });
    controls = $('[data-type=int]') || [];
    for (var i = 0; i < controls.length; i++) {
        var control = $(controls[i]);
        control.kendoNumericTextBox({ format: 'n0', step: 1, min: control.data('min'), max: control.data('max') });
    }

    $('[data-type=intt]').kendoNumericTextBox({ format: 'n0', step: 1 });
    $('[data-type=krichtext]').kendoEditor({ encoded: true });

    $('[data-action]').on('click', function () {
        if (o.onClick) o.onClick($(this).data('action'));
    });

    if (o.onRendered) o.onRendered();
    if (o.init) o.init();
    if (o.onChange) {
        $('select[data-field]').on('change', function (e) {
            var _this = $(this)
            o.onChange(_this.data('field'), _this.val());
        });
    }
}

sdms.report = function (o) {
    switch (o.type) {
        case 'xlsx':
            reportXls(o);
            break;
        case 'preview':
            reportXls(o);
            break;
        default:
            break;
    }

    function reportXls(o) {
        $('.page > .ajax-loader').show();
        sdms.post(o, function (r) {
            //console.log(r);

            if (r.rows) {
                if (r.rows < 60000 && r.fileUrl) {
                    $('.page > .ajax-loader').hide();
                    window.location.href = r.fileUrl;

                    sdms.info('data sebanyak ' + r.rows + ' record sedang digenerate');
                }
                else {
                    $('.page > .ajax-loader').hide();

                    sdms.info({ type: 'error', text: 'tidak bisa mengenerate data sebanyak ' + r.rows + ' record' });
                }
            }
        });
    }
}

sdms.info = function (o, p) {
    if (o.showMethod) toastr.options.showMethod = o.showMethod;
    if (o.hideMethod) toastr.options.hideMethod = o.hideMethod;
    if (o.timeout) toastr.options.timeout = o.timeout;

    if (o) {
        if (typeof o == 'string') {
            toastr.info(o, p);
        }
        else {
            switch (o.type) {
                case 'warning':
                    toastr.warning(o.text, o.title);
                    break;
                case 'success':
                    toastr.success(o.text, o.title);
                    break;
                case 'error':
                    toastr.error(o.text, o.title);
                    break;
                default:
                    toastr.info(o.text, o.title);
                    break;
            }
        }
    }
}

sdms.save = function (o) {
    if (o.params) {
        var params = {};
        for (var key in o.params) {
            params['p_' + key] = o.params[key]
        }
        o.params = params;
    }
    if (o.url) {
        sdms.post(o, function (r) {
            if (o.finish) o.finish(r);
            if (o.success && r.success) o.success(r);
            if (o.error && !r.success) o.error(r);
        })
    }
}

sdms.showAjaxLoad = function () {
    $(".page .ajax-loader").fadeIn();
}

sdms.lookup = function (o) {
    if (o) {
        function renderHeader() {
            var _html = '';
            if (o.title) {
                _html += '<div class="modal-header">';
                _html += '<h4 class="modal-title">' + o.title + '</h4>';
                _html += '</div>';
            }
            return _html;
        }

        function renderBody() {
            var _html = '';
            _html += '<div class="modal-body">';
            _html += '<div data-kgrid=' + o.name + '></div>'
            _html += '</div>';
            return _html;
        }

        function renderFooter() {
            var _html = '<div class="modal-footer">';
            _html += '<button type="button" data-action="ok" class="btn btn-primary">';
            _html += '<i class="fa fa-check"></i> OK';
            _html += '</button>&nbsp;';
            _html += '<button type="button" class="btn btn-default" data-dismiss="modal">';
            _html += '<i class="fa fa-undo"></i> Cancel';
            _html += '</button>';
            _html += '</div>';
            return _html;
        }

        var html = '';
        html += '<div class="modal-dialog kgrid">';
        html += '<div class="modal-content">';
        html += renderHeader();
        html += renderBody();
        html += renderFooter();
        html += '</div>';
        html += '</div>';

        $('#modal .modal').removeClass().addClass('modal fade lookup').html(html).modal();
        if (o.onclick) {
            $('#modal .modal [data-action]').on('click', function () {
                var selector = '#modal .modal [data-kgrid]';
                var row = $(selector).data('kendoGrid').dataItem($(selector).find('.k-state-selected'));
                if (row) {
                    sdms.lookup();
                    if (typeof o.onclick === 'function') o.onclick(row);
                    if (typeof o.onclick === 'string') {
                        if (SimDms.activeObject && SimDms.activeObject.options[o.onclick]) {
                            SimDms.activeObject.options[o.onclick](row);
                        }

                        if (sdms.options && sdms.options[o.onclick]) {
                            sdms.options[o.onclick](row);
                        }
                    }
                }
            });
        }

        o.selectable = "row";
        sdms.renderKgrid(o);
    }
    else {
        $('#modal .modal').modal('hide');
    }
}