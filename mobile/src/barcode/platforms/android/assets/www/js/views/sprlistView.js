function sprlistView() {
    mobile.writeTitle();
    var user = mobile.storage.get('sdms.user');
    var boxes = [];
    var boxesSelected = [];

    reloadData();

    mobile.onclick('sprlist', 'process', function () {
        var selectedTag = $('[data-name=sprlist] .form .form-list .active');
        if (selectedTag.length > 0) {
            mobile.confirm({
                message: 'Process selected data',
                onOk: function () {
                    processSelected(selectedTag);
                },
                onCancel: function () {
                    console.log('Cancel');
                }
            });
        }
        else {
            mobile.alert('Anda tidak memilih data untuk di proses');
        }
    });

    mobile.onclick('sprlist', 'remove', function () {
        var selectedTag = $('[data-name=sprlist] .form .form-list .active');
        if (selectedTag.length > 0) {
            mobile.confirm({
                message: 'Remove selected data',
                onOk: function () {
                    removeSelected(selectedTag);
                },
                onCancel: function () {
                    console.log('Cancel');
                }
            });
        }
        else {
            mobile.alert('Anda tidak memilih data untuk di proses');
        }
    });

    mobile.onclick('sprlist', 'add', function () {
        mobile.navigate('sprinput');
    });

    mobile.onclick('sprlist', 'edit', function () {
        var caseno = $('[data-name=sprlist] .form .form-list .active').data('field');
        var box = Enumerable.From(boxes).Where(function (x) { return x.caseno == caseno }).FirstOrDefault();

        if (box) {
            mobile.storage.set('sdms.box', box);
            mobile.navigate('sprinput');
        }
        else {
            mobile.alert('Anda tidak memilih data untuk di proses');
        }
    });

    mobile.onclick('sprlist', 'clear', function () {
        clearAll();
    });

    mobile.onclick('sprlist', 'checkall', function () {
        checkAll();
    });

    function reloadData() {
        var rows = [];

        boxes = mobile.storage.get('sdms.boxes') || [];
        (boxes || []).forEach(function (box) {
            rows.push({
                name: box.caseno,
                text: box.caseno,
                subtext: 'Input Date : ' + moment(box.savedate).format('DD-MMM-YYYY  HH:mm:ss')
            });
        });
        mobile.list({ selector: '[data-name=sprlist] .form', type: 'list', action: 'box', data: rows });
        mobile.onclick('sprlist', 'box', function () {
            var box = $(this);
            if (box.hasClass('active')) {
                box.removeClass('active');
            }
            else {
                box.addClass('active');
            }
            refreshToolbar();
        });
        refreshToolbar();
    }

    function processSelected(selectedTag) {
        boxesSelected = [];
        for (var i = 0; i < selectedTag.length; i++) {
            var caseno = $(selectedTag[i]).data('field');
            var box = Enumerable.From(boxes).Where(function (x) { return x.caseno == caseno }).FirstOrDefault();

            boxesSelected.push(box);
        }

        async.eachSeries((boxesSelected || []), processBox, function (err) {
            if (err) throw err;

            (boxesSelected).forEach(function (box) {
                for (var i = 0; i < boxes.length; i++) {
                    if (boxes[i] && box.caseno == boxes[i].caseno) {
                        delete boxes[i];
                    }
                }
            });
            mobile.storage.set('sdms.boxes', boxes);
            reloadData();
        });

        function processBox(box, callback) {
            for (var i = 0; i < boxes.length; i++) {
                if (boxes[i] && box.caseno == boxes[i].caseno) {
                    box.username = user.UserName;
                    box.savedate = moment(box.savedate).format('YYYY-MM-DD HH:mm:ss');
                    box.outlet = user.Outlet.OutletCode;

                    mobile.showProcess();
                    mobile.postData({
                        url: 'SaveBox',
                        key: 'sdms.dealers',
                        data: { box: JSON.stringify(box) },
                        success: function (result) {
                            mobile.hideProcess();
                            if (callback) callback();
                        },
                        error: function () {
                            mobile.hideProcess();
                        }
                    })
                }
            }
        }
    }

    function removeSelected(selectedTag) {
        boxesSelected = [];
        for (var i = 0; i < selectedTag.length; i++) {
            var caseno = $(selectedTag[i]).data('field');
            var box = Enumerable.From(boxes).Where(function (x) { return x.caseno == caseno }).FirstOrDefault();

            boxesSelected.push(box);
        }

        (boxesSelected).forEach(function (box) {
            for (var i = 0; i < boxes.length; i++) {
                if (boxes[i] && box.caseno == boxes[i].caseno) {
                    delete boxes[i];
                    console.log('box.caseno', box.caseno, i, boxes[i]);
                }
            }
        });
        mobile.storage.set('sdms.boxes', boxes);
        reloadData();
    }

    function clearAll() {
        $('[data-name=sprlist] .form .list-group-item').removeClass('active');
        refreshToolbar();
    }

    function checkAll() {
        $('[data-name=sprlist] .form .list-group-item').addClass('active');
        refreshToolbar();
    }

    function refreshToolbar() {
        var selected = $('[data-name=sprlist] .form .form-list .active').length;
        switch (selected) {
            case 0:
                $('[data-name=sprlist] [data-action=edit]').addClass('hide');
                $('[data-name=sprlist] [data-action=remove]').addClass('hide');
                $('[data-name=sprlist] [data-action=process]').addClass('hide');
                $('[data-name=sprlist] [data-action=clear]').addClass('hide');
                $('[data-name=sprlist] [data-action=checkall]').removeClass('hide');
                break;
            case 1:
                $('[data-name=sprlist] [data-action=edit]').removeClass('hide');
                $('[data-name=sprlist] [data-action=remove]').removeClass('hide');
                $('[data-name=sprlist] [data-action=process]').removeClass('hide');
                $('[data-name=sprlist] [data-action=clear]').removeClass('hide');
                $('[data-name=sprlist] [data-action=checkall]').removeClass('hide');
                break;
            default:
                $('[data-name=sprlist] [data-action=edit]').addClass('hide');
                $('[data-name=sprlist] [data-action=remove]').removeClass('hide');
                $('[data-name=sprlist] [data-action=process]').removeClass('hide');
                $('[data-name=sprlist] [data-action=clear]').removeClass('hide');

                if (selected == boxes.length) {
                    $('[data-name=sprlist] [data-action=checkall]').addClass('hide');
                }
                else {
                    $('[data-name=sprlist] [data-action=checkall]').removeClass('hide');
                }
                break;
        }
    }
}

