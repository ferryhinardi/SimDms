var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

$(document).ready(function () {		
    var options = {
        title: "test",
        xtype: "iframe",
        url: "assets/js/app/sp/inquiry/inqsparepart_demo.js"
    };  

	Wx = new SimDms.Widget(options);
	Wx.default = {};
	Wx.render(init);

	function init(s) {
	    console.log("init");
	}

});
