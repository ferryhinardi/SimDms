var simdms = window.simdms || {};

sdms.hideAjaxLoad = function () {
    $(".page .ajax-loader").fadeOut();
}

sdms.info = function (o, p) {
    if (o.showMethod) toastr.options.showMethod = o.showMethod;
    if (o.hideMethod) toastr.options.hideMethod = o.hideMethod;
    if (o.timeout) toastr.options.timeout = o.timeout;

    toastr.options.positionClass = (o.positionClass || 'toast-top-right');

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

sdms.notify = function (o, p) {
    o.positionClass = 'toast-bottom-right';
    sdms.info(o, p);
}

sdms.numberFormat = function (number, decimals, dec_point, thousands_sep, type) {
    number = (number + '').replace(/[^0-9+\-Ee.]/g, '');
    var n = !isFinite(+number) ? 0 : +number,
      prec = !isFinite(+decimals) ? 0 : Math.abs(decimals),
      sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
      dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
      s = '',
      toFixedFix = function (n, prec) {
          var k = Math.pow(10, prec);
          return '' + Math.round(n * k) / k;
      };
    // Fix for IE parseFloat(0.55).toFixed(0) = 0;
    s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
    if (s[0].length > 3) {
        s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
    }
    if ((s[1] || '').length < prec) {
        s[1] = s[1] || '';
        s[1] += new Array(prec - s[1].length + 1).join('0');
    }
    return s.join(dec);
}

sdms.numberformat = function (number, decimals, dec_point, thousands_sep, type) {
    return sdms.numberFormat(number, decimals, dec_point, thousands_sep, type);
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

sdms.renderKgrid = function (source) {
    var kds = {}, columns = []; params = {};

    if (source.filter) {
        if (typeof source.filter == 'string') {
            params = sdms.serialize(source.filter);
        }
        else {
            params = source.filter;
        }
    }

    if (source.params) {
        var row = sdms.serialize();
        (source.params || []).forEach(function (val) {
            if (val.value) {
                params[val.param] = value;
            }
            else if (row[val.name]) {
                params[val.param] = row[val.name];
            }
        });
    }

    if (source.data) {
        if (typeof source.data == 'string') {
            kds = new kendo.data.DataSource({
                pageSize: (source.pageSize || 10),
                group: source.group,
                aggregate: source.aggregate,
                sort: source.sort,
                transport: {
                    read: {
                        url: source.data,
                        dataType: 'json',
                        type: 'POST',
                        data: params
                    },
                },
            });
            populateGrid();
        }
        else {
            kds = new kendo.data.DataSource({
                data: source.data,
                type: 'json',
                pageSize: (source.pageSize || 10),
                group: source.group,
                aggregate: source.aggregate,
                sort: source.sort
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
                    url: source.url,
                    dataType: 'json',
                    type: 'POST',
                    data: params
                },
            },
            schema: {
                data: 'data',
                total: 'total'
            },
            pageSize: (source.pageSize || 10),
            group: source.group,
            aggregate: source.aggregate,
            sort: source.sort
        });
        populateGrid();
    };

    function populateGrid() {
        (source.fields || []).forEach(function (field) {
            var column = { field: field.name, title: field.text, width: field.width, template: field.template, attributes: field.attributes };

            switch (field.type) {
                case 'int':
                    column.template = '<div class="right">#= ((' + column.field + ' == undefined) ? "" : sdms.numberFormat(' + column.field + ', 0)) #</div>';
                    column.filterable = false;
                    break;
                case 'decimal':
                    column.template = '<div class="right">#= ((' + column.field + ' == undefined) ? "" : sdms.numberFormat(' + column.field + ', 2)) #</div>';
                    column.filterable = false;
                    break;
                case 'date':
                    column.template = '#= (' + column.field + ' == undefined || (moment(' + column.field + ').format("YYYYMMDD") <= "19000101")) ? "-" : moment(' + column.field + ').format("DD MMM YYYY") #'
                    column.filterable = false;
                    break;
                case 'datetime':
                    column.template = '#= (' + column.field + ' == undefined || (moment(' + column.field + ').format("YYYYMMDD") <= "19000101")) ? "-" : moment(' + column.field + ').format("DD MMM YYYY HH:mm:ss") #'
                    column.filterable = false;
                    break;
                default:
                    break;
            }

            columns.push(column);
        });

        //sdms.showLoading();
        source.kgrid = $('[data-kgrid=' + source.name + ']').kendoGrid({
            dataSource: kds,
            columns: columns,
            detailTemplate: source.detailTemplate,
            groupable: (source.groupable === undefined ? false : source.groupable),
            sortable: (source.sortable === undefined ? true : source.sortable),
            pageable: (source.pageable === undefined ? true : source.pageable),
            selectable: source.selectable || 'multiple',  // [ 'cell', 'row', 'multiple, row', 'multiple, cell' ]
            filterable: (source.filterable === undefined ? { extra: false, operators: { string: { contains: 'Contains' } } } : source.filterable),
            columnMenu: true,
            reorderable: true,
            dataBinding: function (e) {
                if (source.dataBinding) source.dataBinding(e);
            },
            dataBound: function (e) {
                //sdms.hideLoading();
                if (source.dataBound) source.dataBound(e);
                if (source.dblclick) {
                    $('[data-kgrid=' + source.name + '] tbody tr').on('dblclick', function () {
                        var grid = $('[data-kgrid=' + source.name + ']').data("kendoGrid");
                        var data = grid.dataItem($(this));

                        if (typeof source.dblclick === 'function') source.dblclick(data);
                        if (typeof source.dblclick === 'string') {
                            if (SimDms.activeObject && SimDms.activeObject) SimDms.activeObject.options[source.dblclick](data);
                            if (sdms.options) sdms.options[source.dblclick](data);
                        }
                        sdms.lookup();
                    });
                }
            }
        });

        source.kgrid.refresh = function (params) {
            if (kds !== undefined) {
                kds.options.transport.read.data = params;
                kds.read();
            }
        }
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
            case "datepicker":
                html += generateLabel(field);
                html += "<div class='datepicker-wrapper'><input type='text' data-type='datepicker' class='datepicker' placeholder='dd-MMM-yyyy' " + fieldName + attribute + "/></div>";
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

sdms.popup = function (o) {
    var sources = [];
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
                    bindsources();
                    break;
                case "datepicker":
                    html += generateLabel(field);
                    html += "<div class='datepicker-wrapper'><input type='text' data-type='datepicker' class='datepicker' placeholder='dd-MMM-yyyy' " + fieldName + attribute + "/></div>";
                    break;
                case "kdatepicker":
                    var wrapper = " class=\"k-datepicker-wrapper" + ((field.cls == undefined) ? "\"" : (" " + field.cls + "\""));
                    html = "<div" + wrapper + "><input type=\"text\" data-type=\"kdatepicker\" placeholder=\"dd-MMM-yyyy\"" + fieldName + attribute + "/></div>";
                    break;
                case 'radio':
                    html += '<div>';
                    html += '<input type=\"radio\" value="' + field.value + '" name="' + field.name + '" id="' + field.id + '"' + attribute + (field.checked == true ? ' checked' : ' ') + '/><label for="' + field.id + '" style="margin-top:-25px; padding-left:15px; padding-bottom: 10px; font-size: .875em;font-family:"segoeui";">' + field.text + '</label>';
                    //html += generateLabel(field);
                    html += '</div>';
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
        function bindsources() {
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
        }

        // begin kendo controls
        var controls = [];
        //$('[data-type=kdatepicker]').kendoDatePicker({ format: 'dd-MMM-yyyy' });
        ////$('[data-type=datepicker]').datepicker({ format: 'dd-MMM-yyyy' });
        //$('[data-type=kmonthpicker]').kendoDatePicker({ start: 'year', depth: 'year', format: 'MMMM yyyy' });
        //$('[data-type=ktimepicker]').kendoTimePicker({ format: 'HH:mm' });
        //$('[data-type=kdatetimepicker]').kendoDateTimePicker({ format: 'dd-MMM-yyyy HH:mm' });
        //$('[data-type=decimal]').kendoNumericTextBox({ format: 'n2', step: 0.01 });

        //$(".datepicker").datepicker('destroy').datepicker({
        //    dateFormat: "dd-M-yy",
        //    showOtherMonths: true,
        //    selectOtherMonths: true,
        //    changeMonth: true,
        //    changeYear: true,
        //});

        $(".datepicker").removeClass('hasDatepicker').removeAttr('id').datepicker({
            dateFormat: "dd-M-yy",
            showOtherMonths: true,
            selectOtherMonths: true,
            changeMonth: true,
            changeYear: true,
        });
        // begin kendo controls
        $("[data-type=kdatepicker]").kendoDatePicker({ format: "dd-MMM-yyyy" });
        $("[data-type=kmonthpicker]").kendoDatePicker({ start: "year", depth: "year", format: "MMMM yyyy" });
        $("[data-type=ktimepicker]").kendoTimePicker({ format: "HH:mm" });
        $("[data-type=kdatetimepicker]").kendoDateTimePicker({ format: "dd-MMM-yyyy HH:mm" });
        $("[data-type=decimal]").kendoNumericTextBox({ format: "n2", step: 0.01 });


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

        function renderBody() {
            var _html = '';
            _html += '<div class="modal-body">';

            if (o.rows) {
                (o.rows || []).forEach(function (row) {
                    _html += renderRow(row);
                });
            }

            //_html += renderRow(o.rows);
            //_html += '<div data-kgrid=' + o.name + '></div>'
            _html += '</div>';

            return _html;
        }

        function renderFooter() {
            var _html = '<div class="modal-footer">';
            _html += '<button type="button" data-action="ok" class="btn btn-primary">';
            _html += '<i class="fa fa-check"></i> ' + o.rows[0].okbutton.name;
            _html += '</button>&nbsp;';
            _html += '<button type="button" class="btn btn-default" data-dismiss="modal">';
            _html += '<i class="fa fa-undo"></i> Cancel';
            _html += '</button>';
            _html += '</div>';
            return _html;
        }

        var html = '';
        html += '<div class="modal-dialog">';
        html += '<div class="modal-content">';
        html += renderHeader();
        html += renderBody();
        html += renderFooter();
        html += '</div>';
        html += '</div>';

        $('#modal .modal').removeClass().addClass('modal fade lookup').html(html).modal();

        if (typeof o.ownStyle != 'undefined') {
            $('.wg-modal .modal').css({ 'width': '50%', 'margin-left': '23%' });
            if (o.onclick) {
                $('#modal .modal [data-action]').on('click', function () {
                    if (typeof o.onclick === 'function') o.onclick();
                });
            }

            if (typeof o.rows[0].fields[0].action != 'undefined') {
                $('#' + o.rows[0].fields[0].name).on("change", function () {
                    o.rows[0].fields[0].action($(this));
                });
            }
        }

        //o.selectable = "row";
        //sdms.renderKgrid(o);
    }
    else {
        $('#modal .modal').modal('hide');
    }
}
