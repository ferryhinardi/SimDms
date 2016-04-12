var mobile = new BooGr.Mobile({
    brand: 'SDMS Mobile Apps',
    pages: [
        { name: 'home', title: 'Home' },
        { name: 'sync', title: 'Dealer Sync Data', back: 'home', footer: true },

        { name: 'uploaded', title: 'Outstanding Data', back: 'sync', footer: true },
        { name: 'extracted', title: 'Extracted Data', back: 'sync', footer: true },
        { name: 'extracteddetail', title: 'Data by Table', back: 'sync', footer: true },

        { name: 'extracteddealer', title: 'Data Per Dealer', back: 'extracted', footer: true },
    ],
    shortcut: [
        { name: 'chgpwd', text: 'Change Password' },
        { name: 'signout', text: 'Sign Out' },
    ],
    onRendered: function () {
        mobile.navigate('home');
        mobile.storage.set('sdms.user', { uid: 'dev' });
    },
    onPageChanged: pageChanged,
});

function pageChanged(page, options) {
    var back = options && options.back;
    if (!back) {
        if (page == 'home') homeView();
        if (page == 'sync') syncView();

        // sync module
        if (page == 'uploaded') uploadedView();
        if (page == 'extracted') extractedView();
        if (page == 'extracteddetail') extracteddetailView();
        if (page == 'extracteddealer') extracteddealerView();
    }
    window.location.hash = page;
}
