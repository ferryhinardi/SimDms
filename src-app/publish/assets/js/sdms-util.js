Sdms.Util = function () {
    var self = this;

    if (Sdms['util'] !== undefined) delete Sdms['util'];
    Sdms['util'] = this;

    this.populate = function (options, callback) {
        if (options.source == undefined && options.data == undefined) {
            if (callback) callback('source not found');
        }

        if (options.source !== undefined) {
            this.post({ url: options.source, params: options.params }, function (err, doc) {
                if (err) throw err;

                populateData(doc);
            })
        }
        else {
            populateData(options.data)
        }

        function populateData(data) {
            $.each(data, function (key, value) {
                var ctrl = $('[name=' + key + ']');
                var type = ctrl.data('type');

                switch (type) {
                    case "date":
                        value = (value) ? moment(value).format("DD-MMM-YYYY") : undefined;
                        ctrl.val(value);
                        break;
                    default:
                        ctrl.val(value);
                        break;
                }
            });

            if (callback) callback(null, 'populated');
        }
    }
    this.post = function (options, callback) {
        $.post(Sdms.baseUrl + options.url, options.params, function (result) {
            if (callback) callback(null, result);
        })
    }

    this.reload = function (options) {
        switch (options.target || 'page') {
            case 'page':
                $('#main .page').addClass('hide');
                $('#main .page#' + options.name).removeClass('hide');
                if (options.title !== undefined) {
                    $('#main .page#' + options.name + ' .title .title-page').text(options.title);
                }

                switch (options.type || 'list') {
                    case 'list':
                        if (options.url == undefined) {
                            renderPageList(options.data);
                        }
                        else {
                            self.post({
                                url: options.url,
                                params: options.params
                            }, function (err, docs) {
                                if (err) throw console.log(err);

                                renderPageList(docs);
                            })
                        }
                        break;
                    case 'form':
                        renderPageForm(options.form);
                        break;
                }
                break
        }

        function renderPageList(docs) {
            var html = '';
            (docs || []).forEach(function (doc) {
                html += '<div class="row" data-name="' + doc.name + '">';
                html += ' <div class="col-xs-2 col-sm-1 col-md-1 col-lg-1"><i class="icon ' + resolveIconTag(doc.icon_a || doc.icon) + '"></i></div>';
                html += ' <div class="col-xs-8 col-sm-8">';
                html += '  <div class="list-title">' + doc.text + '<i class="fa ' + resolveIconTag(doc.icon_b) + '"></i></div>';
                html += '  <div class="list-detail">' + (doc.info || doc.text) + '</div>';
                html += ' </div>';
                html += '</div>';
            });
            $('#main .page#' + options.name + ' .content .list').html(html);

            // add back functionality
            if (options.back !== undefined) {
                $('#main .page#' + options.name + ' .header .back').off();
                $('#main .page#' + options.name + ' .header .back').on('click', function () {
                    window.location.hash = options.back;
                });
            }

            // event onRowClick 
            if (options.onRowClick) {
                $('#main .page#' + options.name + ' .content .list > .row').on('click', function () {
                    options.onRowClick($(this).data('name'));
                });
            }

            // callback when finish
            if (options.onReloaded) options.onReloaded();
        }

        function renderPageForm(form) {
            var html = '';
            var items = form.items || [];
            var sources = [];
            var evtclick = {};

            (items).forEach(function (item) {
                if (item.onClick !== undefined) { evtclick[(item.btnname || item.name)] = item }

                switch (item.type) {
                    case 'table':
                        html += '<div class="row">';
                        html += '<h3>' + item.text + '</h3>';
                        html += '<div class="table-wrapper">';
                        html += '<table class="table" id="' + item.name + '">';
                        html += '<thead>';
                        if (item.columns.length > 0) {
                            (item.columns).forEach(function (column) {
                                var style = '';
                                if (column.style !== undefined) {
                                    style = ' style="' + column.style + '"';
                                }
                                html += '<th' + style + '>' + column.text + '</th>';
                            });
                        }
                        html += '</thead>';
                        html += '<tbody>';
                        (item.data || []).forEach(function (row) {
                            html += '<tr>';
                            for (var key in row) {
                                html += '<td>' + row[key] + '</td>';
                            }
                            html += '</tr>';
                        });
                        html += '</tbody>';
                        html += '</table>';
                        html += '</div>';
                        html += '</div>';

                        if (item.url !== undefined) sources.push(item);
                        break;
                    case 'text':
                        html += '<div class="row">';
                        html += '<input type="text" placeHolder="' + item.text + '" class="form-control" name="' + item.name + '"/>'
                        html += '</div>';
                        break;
                    case 'select':
                        html += '<div class="row">';
                        html += '<select class="form-control">';
                        var options = item.data || [];
                        if (options.length > 0) {
                            if (options.length == 1) {
                                (options || []).forEach(function (row) {
                                    html += '<option value="' + row.name + '">' + row.text + '</option>';
                                });
                            }
                            else {
                                html += '<option>-- ' + (item.info || 'SELECT ONE') + ' --</option>';
                                (options || []).forEach(function (row) {
                                    html += '<option value="' + row.name + '">' + row.text + '</option>';
                                });
                            }
                        }
                        html += '</select>';
                        html += '</div>';
                        sources.push(item);
                        break;
                    case 'date':
                        html += '<div class="row">';
                        //html += '<input type="text" data-type="date" placeHolder="' + item.text + '" class="form-control" name="' + item.name + '"/>'
                        html += '<input type="date" data-type="date" placeHolder="' + item.text + '" class="form-control" name="' + item.name + '"/>'
                        html += '</div>';
                        sources.push(item);
                        break;
                    case 'text-button':
                        html += '<div class="row">';
                        html += '<h5>' + item.text + '</h5>';
                        html += '<div class="input-group">';
                        html += '<input type="text" class="form-control" name="' + item.name + '"/>'
                        html += '<span class="input-group-btn">';
                        html += ' <button id="' + item.btnname + '" class="btn btn-primary" type="button"><i class="fa fa-plus-circle"></i>Add</button>';
                        html += '</span>'
                        html += '</div>';
                        html += '</div>';
                        break;
                    case 'button':
                        html += '<div class="row">';
                        html += '<button type="button" class="btn btn-primary btn-block" id="' + item.name + '">';
                        if (item.icon !== undefined) {
                            html += '<i class="fa ' + item.icon + '"></i>';
                        }
                        html += item.text;
                        html += '</button>';
                        html += '</div>';
                        break;
                    case 'periode':
                        html += '<div class="row">';
                        html += '<h5>Periode</h5>';
                        html += '<div class="col-6"><input type="text" placeholder="Month" class="form-control" value="' + moment().format("MMMM - YYYY") + '" readonly="readonly"/></div>';
                        html += '</div>';
                        break;
                    case 'panel':
                        html += '<div class="row">';
                        html += '<div id="' + item.name + '" class="panel"></div>';
                        html += '</div>';
                        break;
                    default:
                        html += '<div class="row">';
                        html += item.text;
                        html += '</div>';
                        break;
                }
            });

            html = '<div class="form">' + html + '</div>';

            async.each(sources, loadSource, function (err, doc) {
                if (err) throw err;

                // add back functionality
                if (options.back !== undefined) {
                    $('#main .page#' + options.name + ' .header .back').off();
                    $('#main .page#' + options.name + ' .header .back').on('click', function () {
                        window.location.hash = options.back;
                    });
                }

                // event onClick
                $('#main .page#' + options.name + ' .content').html(html);
                for (var key in evtclick) {
                    $('#main .page#' + options.name + ' .content #' + key).on('click', function () {
                        evtclick[key].onClick();
                    });
                }

                // callback when finish
                if (options.onReloaded) options.onReloaded();
            });

            function loadSource(source, callback) {
                switch (source.type) {
                    case "table":
                        $.ajax({
                            type: 'POST',
                            url: Sdms.baseApi + source.url,
                            crossDomain: true,
                            dataType: 'jsonp',
                            success: function (data, status, xhr) {
                                var selector = '#main .page#' + options.name + ' .content #' + source.name;
                                var html = '';
                                var dclass = {};

                                (source.columns || []).forEach(function (column) {
                                    if (column.dclass == undefined) {
                                        dclass[column.name] = '';
                                    }
                                    else {
                                        dclass[column.name] = ' class="' + column.dclass + '"';
                                    }
                                });

                                (data || []).forEach(function (row) {
                                    html += '<tr>';
                                    for (var key in row) {
                                        html += '<td' + dclass[key] + '>' + row[key] + '</td>';
                                    }
                                    html += '</tr>';
                                });
                                $(selector + ' tbody').html(html);
                            },
                            error: function (res, status, err) {
                                alert('POST failed.');
                            }
                        });
                        break;
                    default:
                        break;
                }
                callback(null, 'done');
            }
        }
    }

    this.save = function (options, callback) {
        this.post({ url: options.source, params: options.data }, function (err, docs) {
            if (err) throw err;

            if (callback) callback(null, docs);
        });
    }
    this.serialize = function (options) {
        var selector = '#' + ((typeof options == 'string') ? options : options.name);

        return $(selector).serializeObject();
    }

    function resolveIconTag(icon) {
        if (icon !== undefined) {
            return ((icon.substr(0, 5) == 'glyph') ? 'glyphicon ' : 'fa ') + icon;
        }
        else {
            return '';
        }
    }
}

