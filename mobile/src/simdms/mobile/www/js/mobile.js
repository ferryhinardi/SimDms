var BooGr = {
    baseApi: 'http://dms.suzuki.co.id/simdms/wh.api/mobileszk/',
    //baseApi: 'http://tbsdmsap01/simdms/wh.api/mobileszk/',
    //baseApi: 'http://localhost:7701/wh.api/mobileszk/',
}

BooGr.Mobile = function (options) {
    var self = this;
    this.options = options;
    $(document).ready(function () { self.render() });
}

BooGr.Mobile.prototype.render = function () {
    var self = this;
    var storage = self.storage;
    var options = self.options;
    var html = '';

    async.eachSeries((options.pages || []), renderPage, function (err) {
        if (err) throw (err);

        rendefShortcut();

        html += '<div class="popup"></div>';          // render popup
        html += '<div class="pageblock"></div>';      // render pageblock
        html += '<div class="ajaxloader"><i class="fa fa-cog fa-spin"/></i></div>';     // render ajaxloader

        $('body').html(html);
        $('body [data-action=back]').on('click', function () {
            var back = $(this).data('back');
            self.navigate(back);
        });
        $('body [data-action=search]').on('click', function () {
            self.search();
        });
        $('body [data-action=search-clear]').on('click', function () {
            self.search({ mode: 'clear' });
        });
        $('body [data-action=shortcut]').on('click', function () {
            self.shortcut();
        });
        $('body').on('click', function (e) {
            if ($(e.target).data('action') !== 'shortcut') {
                self.clearfix();
            }
        });

        $('.body').not('.hasfooter').attr('style', 'min-height:' + (window.innerHeight - 50) + 'px');
        $('.body.hasfooter').attr('style', 'min-height:' + (window.innerHeight - 100) + 'px');

        if (window.cordova) {
            document.addEventListener('deviceready', function () {
                self.cordova = window.cordova;
                if (options.onRendered) options.onRendered();
            }, false);
        }
        else {
            if (options.onRendered) options.onRendered();
        }
    });

    function renderPage(page, callback) {
        html += '<div class="page ' + (page.type || '') + '" data-name="' + page.name + '" id="' + page.name + '">';
        renderHeader(page);
        renderBody(page);
        renderFooter(page);
        html += '</div>';
        if (callback) callback();
    }
    function renderHeader(page) {
        html += '<div class="header">';
        if (page.back) {
            html += '<div class="toolbar">';
            html += ' <i class="fa fa-chevron-left" data-action="back" data-back="' + page.back + '"></i>';
            html += '</div>';
        }
        if (page.toolbars) {
            html += '<div class="toolbar right">';
            (page.toolbars || []).forEach(function (toolbar) {
                html += ' <i class="fa ' + toolbar.icon + '" data-action="' + toolbar.name + '"></i>';
            });
            html += ' <i class="fa fa-ellipsis-v" data-action="shortcut"></i>';
            html += '</div>';
        }
        else {
            if (page.shortcut != false) {
                html += '<div class="toolbar right">';
                html += ' <i class="fa fa-ellipsis-v" data-action="shortcut"></i>';
                html += '</div>';
            }
        }
        html += ' <div class="brand">' + options.brand + '</div>';
        html += ' <div class="title">' + page.title + '</div>';
        html += ' <div class="search">';
        html += '  <div class="form-group has-success has-feedback">';
        html += '   <input type="text" class="form-control">';
        html += '   <span class="glyphicon glyphicon-remove form-control-feedback" data-action="search-clear"></span>';
        html += '  </div>';
        html += ' </div>';
        html += '</div>';
    }
    function renderBody(page) {
        html += '<div class="body animated' + ((page.footer) ? ' hasfooter' : '') + '">';
        if (page.body) {
            (page.body.divs || []).forEach(function (div) {
                html += '<div class="' + div + '"></div>';
            });
        }
        html += '</div>';
    }
    function renderFooter(page) {
        if (page.footer) {
            html += '<div class="footer"></div>';
        }
    }
    function rendefShortcut() {
        if (options.shortcut) {
            html += '<div class="shortcut">';
            (options.shortcut).forEach(function (item) {
                html += '<span data-action="' + item.name + '">' + item.text + '</span>';
            });
            html += '</div>';
        }
    }
}

BooGr.Mobile.prototype.navigate = function (name, options) {
    if ((name || '').length < 1) return;

    var self = this;
    var page1 = $('.page.active');
    var page2 = $('.page[data-name=' + name + ']');

    if (page1.data.name !== name) {
        page1.removeClass('active');
        page2.addClass('active');

        if (options && options.back) {
            if (self.options.onPageChanged) {
                self.options.onPageChanged(name, { back: true });
            }
        }
        else {
            if (options && options.uid) { self.storage.save(options) }

            if (self.options.onPageChanged) {
                self.options.onPageChanged(name);
            }
        }
    }
}

BooGr.Mobile.prototype.list = function (options, callback) {
    var self = this;
    var html = '';
    if (options.title || options.text) {
        html += '<h3 class="title">' + (options.title || options.text) + '</h3>';
    }
    html += '<div class="list-group list">';
    (options.data || []).forEach(function (row) {
        html += '<div class="list-group-item ' + (options.status || '') + '" data-action="' + (options.action || row.name) + '" data-field="' + row.name + '">';
        if (row.text) {
            if (row.subtext) {
                html += '<div class="list-group-item-heading text">' + row.text + '</div>';
                html += '<p>' + row.subtext + '</p>';
            }
            else {
                if (row.badge) html += '<span class="badge">' + row.badge + '</span>';
                html += '<div class="text">' + row.text + '</div>';
            }

            if (row.info) {
                html += '<div class="status ' + row.info.status + '">' + row.info.text + '</div>';
            }

            if (row.infotext) {
                html += '<div class="status ' + row.infostatus + '">' + row.infotext + '</div>';
            }
        }
        html += '</div>';
    });
    html += '</div>';

    var selector = options.selector;
    if (!selector) selector = '[data-name=' + $('.page.active').data('name') + '] .body';
    $(selector).html(html);
    $(selector + ' .list-group-item').off('click');
    $(selector + ' .list-group-item').on('click', function () {
        var action = $(this).data('action');
        if (action && options.onClick) options.onClick(action, $(this).find('.text').text());
        if (options.checkbox || options.check) {
            if ($(this).hasClass('active')) {
                $(this).removeClass('active');
            }
            else {
                $(this).addClass('active');
            }
        }
        if (options.radiolist || options.radio) {
            $(this).parent().find('.list-group-item.active').removeClass('active');
            $(this).addClass('active');
        }
    });

    if (options.search) {
        var pageName = $('.page.active').data('name');
        self.listjs = new List(pageName, { valueNames: options.search.fields });
    }
    if (callback) callback();
}

BooGr.Mobile.prototype.form = function (page) {
    var self = this;
    var pgname = (page.pgname) || $('.page.active').data('name');
    var firstLoaded = ($('[data-name=' + pgname + '] .body').children().length == 0);
    if (firstLoaded) {
        var sources = [];
        if (page.body) {
            var html = '';

            async.eachSeries((page.body.panels || []), renderPanel, function (err) {
                if (err) throw (err);
                $('[data-name=' + pgname + '] .body').html(html);
            })

            function renderPanel(panel, callback) {
                html += '<div class="panel ' + (panel.name || '') + '">';

                if (panel.title || panel.text) {
                    html += '<h3 class="title">' + (panel.title || panel.text) + '</h3>';
                }
                async.eachSeries((panel.fields || []), renderField, function (err) {
                    if (err) throw (err);

                    html += '</div>';
                    if (callback) callback();
                });
            }

            function renderField(field, callback) {
                switch (field.type || 'text') {
                    case 'text':
                        var readonly = (field.readonly == true) ? ' readonly="readonly"' : ''
                        html += '<div class="form-group">';
                        html += '<label>' + field.text + '</label>';
                        if (field.icon) {
                            html += '<div class="input-group">';
                            html += ' <input ' + readonly + ' type="text" class="form-control" data-field="' + field.name + '" placeholder="' + field.text + '" />';
                            html += ' <span class="input-group-addon" data-action="' + (field.action || '') + '"><i class="fa ' + field.icon + '"></i></span>';
                            html += '</div>';
                        }
                        else {
                            html += '<input ' + readonly + ' type="text" class="form-control" data-field="' + field.name + '" placeholder="' + field.text + '">';
                        }
                        html += '</div>';
                        break;
                    case 'textarea':
                        var readonly = (field.readonly == true) ? ' readonly="readonly"' : ''
                        html += '<div class="form-group">';
                        html += '<label>' + field.text + '</label>';
                        html += '<textarea rows=5' + readonly + ' class="form-control" data-field="' + field.name + '" placeholder="' + field.text + '"></textarea>';
                        html += '</div>';
                        break;
                    case 'select':
                        html += '<div class="form-group">';
                        html += ' <label>' + field.text + '</label>';
                        html += ' <select class="form-control" data-type="select" data-field="' + field.name + '">';
                        html += '  <option> -- </option>';
                        html += ' </select>';
                        html += '</div>';
                        break;
                    case 'button':
                        html += '<button class="btn btn-primary btn-block" data-action="' + (field.action || field.name) + '">';
                        html += ' <i class="fa ' + field.icon + '"></i>&nbsp;' + field.text;
                        html += '</button>';
                        break;
                    case 'logo':
                        html += '<div class="logo"><img src="' + field.src + '" alt="" /></div>';
                        break;
                    case 'list':
                        html += '<div class="form-group">';
                        html += ' <div id="' + field.name + '"></div>';
                        html += '</div>';

                        sources.push(field);
                        break;
                    case 'picture4':
                        html += '<div class="picture-group">'
                        html += '<div class="row pict">'
                        html += ' <div class="col-xs-6">'
                        html += '  <div class="thumbnail"><img src="img/camera.jpg" alt="" /></div>';
                        html += ' </div>';
                        html += ' <div class="col-xs-6">'
                        html += '  <div class="thumbnail"><img src="img/camera.jpg" alt="" /></div>';
                        html += ' </div>';
                        html += '</div>';
                        html += '<div class="row pict">'
                        html += ' <div class="col-xs-6">'
                        html += '  <div class="thumbnail"><img src="img/camera.jpg" alt="" /></div>';
                        html += ' </div>';
                        html += ' <div class="col-xs-6">'
                        html += '  <div class="thumbnail"><img src="img/camera.jpg" alt="" /></div>';
                        html += ' </div>';
                        html += '</div>';
                        html += '</div>';
                        break;
                    case 'picture2':
                        html += '<div class="picture-group">'
                        html += '<div class="row pict">'
                        html += ' <div class="col-xs-6">'
                        html += '  <div class="thumbnail"><img src="img/camera.jpg" alt="" /></div>';
                        html += ' </div>';
                        html += ' <div class="col-xs-6">'
                        html += '  <div class="thumbnail"><img src="img/camera.jpg" alt="" /></div>';
                        html += ' </div>';
                        html += '</div>';
                        html += '</div>';
                        break;
                    default:
                        break;
                }

                if (callback) callback();
            }
        }

        if (page.footer) {
            var html = '';

            async.eachSeries((page.footer.bars || []), renderBar, function (err) {
                if (err) throw (err);

                $('[data-name=' + pgname + '] .footer').html(html);
            })

            function renderBar(bar, callback) {
                var inactive = (bar.inactive) ? ' inactive' : '';
                html += '<div class="footer-bar' + inactive + '" data-action="' + (bar.action || bar.name) + '">';
                html += ' <i class="fa ' + bar.icon + '"></i>';
                html += ' <span>' + bar.text + '</span>';
                html += '</div>';

                if (callback) callback();
            }
        }


        if (page.onRendered) page.onRendered();
        if (page.onReady) page.onReady();
        if (page.onClick) {
            $('#' + pgname + ' .footer-bar').on('click', function () {
                page.onClick($(this).data('action'));
            });
        }
        if (page.onCameraClick) {
            $('#' + pgname + ' .picture-group .thumbnail').off('click');
            $('#' + pgname + ' .picture-group .thumbnail').on('click', function () {
                page.onCameraClick(this);
            });
        }

        (sources).forEach(function (source) {
            var selector = '[data-name=' + pgname + '] .body';
            if (source.type == 'list') {
                source.selector = selector + ' #' + source.name;
                self.list(source);
            }
        });
    }
    else {
        if (page.onReady) page.onReady();
    }
}

BooGr.Mobile.prototype.search = function (option) {
    var self = this;
    var page = $('.page.active');
    var name = page.data('name');

    if (option && option.mode == 'clear') {
        page.find('.header .brand,.header .title').show();
        page.find('.header .search').hide();
        page.find('.header .search input').val('').focus();
        if (self.listjs) self.listjs.search();
    }
    else {
        page.find('.header .brand,.header .title').hide();
        page.find('.header .search').show();
        page.find('.header .search input').focus();
    }
}

BooGr.Mobile.prototype.shortcut = function () {
    var self = this;
    var shortcut = $('.shortcut');

    if (shortcut.hasClass('active')) {
        shortcut.removeClass('active');
    }
    else {
        shortcut.addClass('active');
    }
}

BooGr.Mobile.prototype.clearfix = function () {
    var self = this;
    var shortcut = $('.shortcut');

    if (shortcut.hasClass('active')) {
        shortcut.removeClass('active');
    }
}

BooGr.Mobile.prototype.populate = function (data) {
    for (var key in data) {
        $('.page.active').find('[data-field=' + key + ']').val(data[key]);
    }
}

BooGr.Mobile.prototype.bind = function (options) {
    var self = this;
    var selector = (options.selector || ('.page.active [data-field=' + options.name + ']'));
    var type = (options.type || $(selector).data('type'));

    if (type == 'select') {
        options.selector = selector;
        self.bindSelect(options);
    }

    if (type == 'list') {
        if (options.showProcess) self.showProcess();
        self.post({
            url: options.url,
            params: options.params,
            success: function (data) {
                options.data = data;
                options.onClick = options.onClick;
                self.list(options);
            },
            complete: function () {
                if (options.showProcess) self.hideProcess();
                if (options.onComplete) options.onComplete();
            }
        })
    }
}

BooGr.Mobile.prototype.bindSelect = function (options) {
    var self = this;

    if (options.url) {
        self.post({
            url: options.url,
            success: function (data) {
                var combobox = d3.select(options.selector)
                if (combobox.selectAll('option').length > data.length) {
                    combobox.selectAll('option')
                        .data(data)
                        .attr({ value: function (row) { return row.value } })
                        .text(function (row) { return row.text })
                        .exit()
                        .remove();
                }
                else {
                    data.unshift({ value: '', text: '--' });
                    combobox.selectAll('option')
                        .data(data)
                        .attr({ value: function (row) { return row.value } })
                        .text(function (row) { return row.text })
                        .enter()
                        .append('option')
                        .attr({ value: function (row) { return row.value } })
                        .text(function (row) { return row.text })
                }

                if (options.onChange) {
                    $(options.selector).on('change', function () {
                        var value = $(options.selector).val();
                        options.onChange(value);
                    });
                }
            }
        })
    }
};

BooGr.Mobile.prototype.serializeData = function () {
    var data = {};
    var controls = $('.page.active').find('[data-field]');
    for (var i = 0; i < controls.length; i++) {
        var control = $(controls[i]);
        data[control.data('field')] = control.val();
    }
    return data;
}

// replace localStorage
BooGr.Mobile.prototype.storage = {
    set: function (key, value) {
        if (Object.prototype.toString.call(value) == '[object Array]') {
            var realValue = [];
            (value || []).forEach(function (val) {
                if (val) realValue.push(val);
            });
            localStorage.setItem(key, JSON.stringify(realValue));
            value = realValue;
        }
        else {
            localStorage.setItem(key, JSON.stringify(value));
        }

        return value;
    },
    save: function (value) {
        localStorage.setItem('sdms.user', JSON.stringify(value));
    },
    get: function (key) {
        var value = JSON.parse(localStorage.getItem(key || 'sdms.user'));
        if (Object.prototype.toString.call(value) == '[object Array]') {
            var realValue = [];
            (value || []).forEach(function (val) {
                if (val) realValue.push(val);
            });
            return realValue;
        }
        else {
            return value;
        }
    },
    remove: function (key) {
        localStorage.removeItem(key);
    },
    update: function (key, value) {
        var data = this.get(key);
        for (var name in value) {
            data[name] = value[name];
        }
        this.set(key, data);
    },
    clear: function () {
        localStorage.clear();
    }
}

BooGr.Mobile.prototype.post = function (options) {
    var self = this;
    $.ajax({
        type: 'POST',
        url: BooGr.baseApi + options.url,
        crossDomain: true,
        data: options.params,
        dataType: 'jsonp',
        success: function (data, status, xhr) {
            if (options.key) self.storage.set(options.key, data);
            if (options.success) options.success(data, status, xhr);
        },
        error: function (res, status, err) {
            if (options.error) options.error(res, status, err);

            self.alert('Sorry, your request cannot be processed right now!');
        },
        complete: function () {
            if (options.complete != undefined && typeof options.complete == 'function') {
                options.complete();
            }
        }
    });
}

BooGr.Mobile.prototype.showProcess = function () {
    $('.pageblock,.ajaxloader').addClass('active');
}

BooGr.Mobile.prototype.hideProcess = function () {
    $('.pageblock,.ajaxloader').removeClass('active');
}

// plugin wrapper
BooGr.Mobile.prototype.geolocation = function (callback) {
    var self = this;
    self.showProcess();
    if (window.cordova) {
        navigator.geolocation.getCurrentPosition(onSuccess, onError);

        function onSuccess(position) {
            self.hideProcess();
            if (callback) callback(null, { latitude: position.coords.latitude, longitude: position.coords.longitude });
        }

        function onError(err) {
            self.hideProcess();
            if (callback) callback(err);
        }
    }
    else {
        self.hideProcess();
        if (callback) callback(null, { latitude: 280, longitude: 190 });
    }
}

