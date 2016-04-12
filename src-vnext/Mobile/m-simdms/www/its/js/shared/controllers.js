appControllers.controller('DialogController', function ($scope, $mdDialog, displayOption) {

    $scope.displayOption = displayOption;

    $scope.cancel = function () {
        $mdDialog.cancel(); 
    };

    $scope.ok = function () {
        $mdDialog.hide();
    };
});

appControllers.controller('ChangePasswordController', function ($scope, $mdDialog, displayOption, $http, BASE_ADDRESS) {

    $scope.displayOption = displayOption;

    $scope.data = {oldpassword:'', newpassword:'', confirmpassword:''};

    $scope.cancel = function () {
        $mdDialog.cancel(); 
    };

    $scope.ok = function () {
        
        console.log($scope.data)

        var params = {
            UserName: $scope.data.oldpassword,
            Password: $scope.data.newpassword
        }
        console.log(params)

        $.ajax({
            type: "POST",
            url: BASE_ADDRESS + "changepassword",
            data: params,
            timeout: 3000,
            beforeSend: function (xhr) {
                xhr.setRequestHeader ("Authorization",   window.globalVariable.Token);
            } 
        }).done(function (data) {
            console.log(data)
        });

        //$http({ url: BASE_ADDRESS + "changepassword", data: params, method: 'POST' }).then(
        //function (data) {
        //    console.log(data)
        //    //$mdDialog.hide();
        //}
        //);



    };
});

appControllers.controller('toastController', function ($scope, displayOption) {
    $scope.displayOption = displayOption;
})

.constant('AUTH_EVENTS', {
    notAuthenticated: 'auth-not-authenticated',
    notAuthorized: 'auth-not-authorized'
})

.constant('USER_ROLES', {
    admin: 'admin_role',
    public: 'public_role'
});
     