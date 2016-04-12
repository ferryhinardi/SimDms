"use strict"

function roomSAController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Room SA",
        xtype: "iframe",
        url: "../assets/custom/RoomSA.cshtml"
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("roomSAController");
    }
});