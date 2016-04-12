var mobile = mobile || {};

function branchView() {
    var user = mobile.storage.get('sdms.user');
    var content = $('[data-name=branch] .form');

    if (!user.Outlet || content.children().length == 0) {
        var html = '';
        html += '<div class="form-group">';
        html += ' <label>User Name</label>';
        html += ' <input type="text" class="form-control" data-field="UserName" readonly="readonly"/>';
        html += '</div>';
        html += '<div class="form-group">';
        html += ' <label>First Name</label>';
        html += ' <input type="text" class="form-control" data-field="FirstName" readonly="readonly"/>';
        html += '</div>';
        html += '<div class="form-group">';
        html += ' <label>Last Name</label>';
        html += ' <input type="text" class="form-control" data-field="LastName" readonly="readonly"/>';
        html += '</div>';
        html += '<div class="form-group">';
        html += ' <label>Dealer</label>';
        html += ' <input type="text" class="form-control" data-field="DealerName" readonly="readonly"/>';
        html += '</div>';
        html += '<div class="form-group">';
        html += ' <label>Select Branch</label>';
        html += ' <select class="form-control" data-field="OutletCode">';
        html += ' </select>';
        html += '</div>';
        content.html(html);

        for (var key in user) {
            $('[data-name=branch] [data-field=' + key + ']').val(user[key]);
        }

        mobile.showProcess();
        mobile.postData({
            url: 'OutletList',
            data: { DealerCode: user.DealerCode },
            success: function (result) {
                mobile.hideProcess();

                var html = '<option value="">--</option>';
                (result || []).forEach(function (outlet) {
                    html += '<option value="' + outlet.OutletCode + '">' + outlet.OutletName + '</option>';
                });
                $('[data-name=branch] [data-field=OutletCode]').html(html);
            },
            error: function () {
                mobile.hideProcess();
            }
        })

        mobile.onclick('branch', 'save', function () {
            var Outlet = {
                OutletCode: $('[data-name=branch] [data-field=OutletCode]').val(),
                OutletName: $('[data-name=branch] [data-field=OutletCode] option:selected').text()
            }
            if (!Outlet.OutletCode) {
                mobile.alert('Anda belum memilih outlet');
            }
            else {
                user.Outlet = Outlet;
                mobile.storage.set('sdms.user', user);
                mobile.navigate('mnlist');
            }
        });
    }
}