"use strict"

function roomQueueController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.show = function () {
        var win = window.open('../assets/custom/RoomQueue.cshtml', '_blank');
        win.focus();
    }

    me.initialize = function () {
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Room Queue",
        xtype: "iframe",
        url: "../assets/custom/RoomQueue.cshtml",
        toolbars :[
             { name: "btnShow", text: "Show", icon: "fa fa-bullseye" , click:"show()"}
        ],
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("roomQueueController");
    }
});