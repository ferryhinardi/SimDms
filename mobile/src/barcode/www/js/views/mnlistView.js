var mobile = mobile || {};

function mnlistView() {
    mobile.writeTitle();

    var data = [
        { name: 'sprinput', text: 'Input Part', subtext: 'Input part using barcode' },
        { name: 'sprlist', text: 'List of Part', subtext: 'List of Inputed parts' },
        { name: 'sprexit', text: 'Exit', subtext: 'Exit Application' }
    ];
    mobile.list({ selector: '[data-name=mnlist] .form', type: 'list', data: data })

    mobile.onclick('mnlist', 'sprinput', function (e) {
        e.preventDefault();
        mobile.navigate('sprinput');
    });

    mobile.onclick('mnlist', 'sprlist', function (e) {
        e.preventDefault();
        mobile.navigate('sprlist');
    });

    mobile.onclick('mnlist', 'sprexit', function () {
        exit();
    });
}
