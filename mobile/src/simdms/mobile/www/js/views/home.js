var mobile = mobile || {};

function homeView() {
    var data = [
        { name: 'sync', text: 'Dealer Sync Data', subtext: '' },
    ];
    mobile.list({
        data: data,
        onClick: function (action) {
            mobile.navigate(action);
        }
    });
}


