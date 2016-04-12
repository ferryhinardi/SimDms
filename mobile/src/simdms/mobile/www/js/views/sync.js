var mobile = mobile || {};

function syncView() {
    var data = [
        { name: 'uploaded', text: 'Outstanding Data', subtext: 'Monitoring Outstanding Data' },
        //{ name: 'uploadeddetail', text: 'Outstanding Data Detail', subtext: 'Monitoring Outstanding Data Detail by Table' },
        { name: 'extracted', text: 'Received Dealer Data', subtext: 'Monitoring Extracted Data' },
        { name: 'extracteddetail', text: 'Received Dealer Data Detail', subtext: 'Received Dealer Data Detail by Table' },
    ];
    mobile.list({
        data: data,
        onClick: function (action) {
            mobile.navigate(action);
        }
    });
}
