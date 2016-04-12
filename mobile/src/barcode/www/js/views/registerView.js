var mobile = mobile || {};

function registerView() {
    var dealers = mobile.storage.get('sdms.dealers');

    html = '';
    html += '<div class="form-group">';
    html += ' <label for="UserName">User Name</label>';
    html += ' <input type="text" class="form-control" id="UserName" data-field="UserName" placeholder="User Name"/>';
    html += '</div>';
    html += '<div class="form-group">';
    html += ' <label for="FirstName">First Name</label>';
    html += ' <input type="text" class="form-control" id="FirstName" data-field="FirstName" placeholder="First Name"/>';
    html += '</div>';
    html += '<div class="form-group">';
    html += ' <label for="LastName">Last Name</label>';
    html += ' <input type="text" class="form-control" id="LastName" data-field="LastName" placeholder="Last Name"/>';
    html += '</div>';
    html += '<div class="form-group">';
    html += ' <label for="DealerCode">Select Dealer</label>';
    html += ' <select class="form-control" id="DealerCode" data-field="DealerCode">';
    html += '  <option value="">--</option>';
    (dealers || []).forEach(function (dealer) {
        html += '  <option value="' + dealer.DealerCode + '">' + dealer.DealerName + '</option>';
    });
    html += ' </select>';
    html += '</div>';

    $('[data-name=register] .form').html(html)

    mobile.onclick('register', 'save', function () {
        var dealer = Enumerable.From(dealers)
            .Where(function (x) { return x.DealerCode == $('[data-name=register] [data-field=DealerCode]').val() })
            .FirstOrDefault();

        if (dealer) {
            var user = {
                UserName: $('[data-name=register] [data-field=UserName]').val(),
                FirstName: $('[data-name=register] [data-field=FirstName]').val(),
                LastName: $('[data-name=register] [data-field=LastName]').val(),
                Dealer: { DealerCode: dealer.DealerCode, DealerName: dealer.DealerName },
                DealerCode: dealer.DealerCode,
                DealerName: dealer.DealerName,
                ApiUrl: dealer.ApiUrl
            }

            if (!user.UserName) {
                mobile.alert('User Name tidak boleh kosong');
                mobile.focus('register', 'UserName');
                return;
            }

            if (!user.DealerCode) {
                mobile.alert('Dealer tidak boleh kosong');
                mobile.focus('register', 'DealerCode');
                return;
            }

            mobile.storage.set('sdms.user', user);
            mobile.navigate('branch');
        }
    });

    mobile.onclick('register', 'cancel', function () {
        mobile.navigate('login', { back: true });
    });
}