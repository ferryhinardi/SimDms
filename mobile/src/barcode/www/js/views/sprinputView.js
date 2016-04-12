var mobile = mobile || {};

function sprinputView() {
    mobile.writeTitle();

    var box = mobile.storage.get('sdms.box') || { caseno: '', info: '', parts: [] };
    var tbrSave = $('[data-name=sprinput] [data-action=save]');
    tbrSave.addClass('hide');

    var html = '';
    html += '<div class="form-group">';
    html += ' <label for="CaseNo">Case Number</label>';
    html += ' <div class="input-group">';
    html += '  <input type="text" class="form-control" id="CaseNo" data-field="CaseNo" placeholder="Case Number"/>';
    html += '  <span class="input-group-addon" data-action="caseno"><i class="fa fa-camera"></i></span>';
    html += ' </div>';
    html += '</div>';
    html += '<div class="form-group">';
    html += ' <label for="Info">Information</label>';
    html += ' <input type="text" class="form-control" id="Info" data-field="Info" placeholder="Information"/>';
    html += '</div>';
    html += '<div class="form-group" style="margin: 0">';
    html += ' <label>List of Part</label>';
    html += '</div>';
    html += '<div class="table">';
    html += ' <div class="row theader">';
    html += '  <div class="col-xs-10"><i class="fa fa-circle-o"></i><i class="fa fa-circle-o"></i>Kode Part</div>';
    html += '  <div class="col-xs-2 center">Qty</div>';
    html += ' </div>';
    html += ' <div class="tbody-list"></div>';
    html += '</div>';
    html += '<div class="form-group">';
    html += ' <div class="input-group">';
    html += '  <input type="text" class="form-control" data-field="PartNo" placeholder="Part Number"/>';
    html += '  <input type="number" class="form-control hide" data-field="Qty" placeholder="Part Qty"/>';
    html += '  <span class="input-group-addon" data-action="add"><i class="fa fa-plus"></i></span>';
    html += ' </div>';
    html += '</div>';
    $('[data-name=sprinput] .form').html(html);

    mobile.onclick('sprinput', 'add', function () {
        var caseno = $('[data-name=sprinput] [data-field=CaseNo]');
        var partno = $('[data-name=sprinput] [data-field=PartNo]');

        if (!caseno.val()) {
            mobile.alert('Case Number harus diisi terlebih dahulu');
            caseno.focus();
            return;
        }
        if (!partno.val()) {
            mobile.alert('Part Number harus diisi terlebih dahulu');
            partno.focus();
            return;
        }

        addPart(partno.val());
    });

    mobile.onclick('sprinput', 'caseno', function () {
        if (mobile.cordova) {
            cordova.plugins.barcodeScanner.scan(
                function (result) { $('[data-name=sprinput] [data-field=CaseNo]').val(result.text); },
                function (error) { alert("Scanning failed: " + error); });
        }
    });

    mobile.onclick('sprinput', 'scan', function () {
        var caseno = $('[data-name=sprinput] [data-field=CaseNo]');
        if (!caseno.val()) {
            mobile.alert('Case Number harus diisi terlebih dahulu');
            caseno.focus();
            return;
        }

        if (mobile.cordova) {
            cordova.plugins.barcodeScanner.scan(
                function (result) { addPart(result.text); },
                function (error) { alert("Scanning failed: " + error); }
            );
        }
    });

    mobile.onclick('sprinput', 'save', function () {
        var boxes = mobile.storage.get('sdms.boxes') || [];

        for (var i = 0; i < boxes.length; i++) {
            if (boxes[i].caseno == box.caseno) {
                delete boxes[i];
            }
        }

        box.caseno = $('[data-name=sprinput] [data-field=CaseNo]').val();
        if (box.caseno && box.parts.length > 0) {
            box.info = $('[data-name=sprinput] [data-field=Info]').val();
            box.savedate = new Date();
            boxes.push(box);
            mobile.storage.set('sdms.boxes', boxes);
            mobile.navigate('sprlist');
        }

        mobile.storage.remove('sdms.box');
    });

    mobile.onclick('sprinput', 'cancel', function () {
        mobile.storage.remove('sdms.box');
        mobile.navigate('mnlist');
    });

    function addPart(partno) {
        box.caseno = $('[data-name=sprinput] [data-field=CaseNo]').val().toUpperCase();
        box.info = $('[data-name=sprinput] [data-field=Info]').val().toUpperCase();

        if ($('[data-name=sprinput] .input-group [data-field=Qty]').hasClass('hide')) {
            var found = false;
            (box.parts).forEach(function (part) {
                if (part.PartNo == partno.toUpperCase()) {
                    found = true;
                    part.Qty = part.Qty + 1;
                }
            });

            if (!found) {
                box.parts.push({ PartNo: partno.toUpperCase(), Qty: 1 });
            }
        }
        else {
            if (!$('[data-name=sprinput] .input-group [data-field=Qty]').val()) return;

            (box.parts).forEach(function (part) {
                if (part.PartNo == partno.toUpperCase()) {
                    part.Qty = parseInt($('[data-name=sprinput] .input-group [data-field=Qty]').val());
                }
            });
        }

        $('[data-name=sprinput] .input-group [data-field=PartNo]').val('');
        $('[data-name=sprinput] .input-group [data-field=Qty]').val('0').addClass('hide');
        $('[data-name=sprinput] .input-group [data-action=add] i').removeClass('fa-save').addClass('fa-plus');
        populateParts();
    }

    function populateParts() {
        var html = '';
        $('[data-name=sprinput] [data-field=CaseNo]').val(box.caseno || '');
        $('[data-name=sprinput] [data-field=Info]').val(box.info || '');

        (box.parts).forEach(function (part) {
            html += '<div class="row tbody">';
            html += '<div class="col-xs-10"><label><i class="fa fa-edit" data-action="edit"></i><i class="fa fa-trash-o" data-action="remove"></i> <span data-list="PartNo">' + part.PartNo + '</span></label></div>';
            html += '<div class="col-xs-2 right"><label><span data-list="Qty">' + part.Qty + '</span></label></div>';
            html += '</div>';
        });
        $('[data-name=sprinput] .form .table .tbody-list').html(html);
        $('[data-name=sprinput] [data-field=PartNo]').val('');

        mobile.onclick('sprinput', 'edit', function () {
            var row = $(this).parent().parent().parent();
            var oPart = {
                PartNo: row.find('[data-list=PartNo]').text(),
                PartQty: row.find('[data-list=Qty]').text()
            }

            $('[data-name=sprinput] .input-group [data-field=PartNo]').val(oPart.PartNo);
            $('[data-name=sprinput] .input-group [data-field=Qty]').val(oPart.PartQty).removeClass('hide');
            $('[data-name=sprinput] .input-group [data-action=add] i').removeClass('fa-plus').addClass('fa-save');
        });

        mobile.onclick('sprinput', 'remove', function () {
            var row = $(this).parent().parent().parent();
            var oPart = {
                PartNo: row.find('[data-list=PartNo]').text(),
                PartQty: row.find('[data-list=Qty]').text()
            }

            mobile.confirm({
                message: oPart.PartNo + ' akan dihapus?',
                onOk: function () {
                    var parts = [];
                    (box.parts).forEach(function (part) {
                        if (part.PartNo.toUpperCase() != oPart.PartNo.toUpperCase()) {
                            parts.push(part);
                        }
                    });
                    box.parts = parts;
                    populateParts();
                }
            });
        });

        if (box.parts.length > 0) {
            if (tbrSave.hasClass('hide')) {
                tbrSave.removeClass('hide');
            }
        }
        else {
            if (!tbrSave.hasClass('hide')) {
                tbrSave.addClass('hide');
            }
        }
    }

    populateParts();
}
