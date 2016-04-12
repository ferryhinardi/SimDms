appControllers.controller('myDashboardCtrl', function ($rootScope,$scope,$state,$stateParams,BASE_COLOR,$timeout,$ionicModal,BASE_ADDRESS,$http,$cookies,$mdDialog) {


    $scope.isAnimated = $stateParams.isAnimated;

    $scope.data = {
        UserName: '',
        Password: ''
    }

    $scope.profile = {};

    $scope.COLOR = BASE_COLOR;
    $scope.dataSummary = {
        STOCK: 0,
        INV: 0,
        INQ: 0,
        SPK:0
    }

    $timeout(function () {
        $scope.$apply();
    },100)

	// doSomeThing is for do something when user click on a button
    $scope.doSomeThing = function () {
    	// You can put any function here.
    } // End doSomeThing.

    // goToSetting is for navigate to Dashboard Setting page
    $scope.goToSetting = function () {
        $state.go("app.setting");
    };// End goToSetting.

    $scope.stock = 'STOCK';
    $scope.invoice = 'INVOICE';

    $scope.imgAvatar = BASE_ADDRESS + 'avatar/unknown.png';

    $scope.login = function ($event) {
            
	        $.ajax({
	            type: "POST",
	            url: BASE_ADDRESS + "gettoken",
	            data: $scope.data,
	            timeout: 3000,
	        }).done(function (data) {
                var ret = JSON.parse(data)
                
	            if (ret.access_token !== null && ret.access_token !== undefined) {

	                 window.globalVariable.Token = 'bearer ' + ret.access_token;
                     $scope.modal.hide();

                     $http.get(BASE_ADDRESS + "userinfo").success(
                     function (data) {
                         $scope.profile = data.data;
                         $scope.summary = data.data.Summary[0];
                         $scope.imgAvatar = BASE_ADDRESS + 'avatar/' + data.data.Image;
                         window.globalVariable.internalUser = data.data.InternalUser;
                         console.log(window.globalVariable.internalUser)
                         $rootScope.$broadcast('changeUser');
                         if (data.data.InternalUser) {
                                $scope.dataSummary = {
                                    STOCK: NumDigit($scope.summary.CurrentDO),
                                    INV: NumDigit($scope.summary.CurrentFP),
                                    INQ: NumDigit($scope.summary.CurrentInq),
                                    SPK:NumDigit($scope.summary.CurrentSpk)
                                }
                              $scope.stock = 'DO';
                              $scope.invoice = 'F. POL';
                         } else {
                             $scope.dataSummary = {
                                    STOCK: NumDigit($scope.summary.CurrentStock),
                                    INV: NumDigit($scope.summary.CurrentInv),
                                    INQ: NumDigit($scope.summary.CurrentInq),
                                    SPK:NumDigit($scope.summary.CurrentSpk)
                                }
                         }
                     }
                     ).error(function (a, b, c, d) {
                         console.log(a, b, c, d)
                     });

	            } else {
                    alert(ret.error_description)
	            }

	        })

    }
    
    // Create the login modal that we will use later
    $ionicModal.fromTemplateUrl('templates/authentication/html/login.html', {
        scope: $scope
    }).then(function (modal) {
        $scope.modal = modal;
        if (globalVariable.Token == '') {
            $scope.modal.show();
        } else {
            $http.get(BASE_ADDRESS + "userinfo").success(
                     function (data) {
                         $scope.profile = data.data;
                         $scope.summary = data.data.Summary[0];
                         $scope.imgAvatar = BASE_ADDRESS + 'avatar/' + data.data.Image;

                         if (data.data.InternalUser) {
                                $scope.dataSummary = {
                                    STOCK: NumDigit($scope.summary.CurrentDO),
                                    INV: NumDigit($scope.summary.CurrentFP),
                                    INQ: NumDigit($scope.summary.CurrentInq),
                                    SPK:NumDigit($scope.summary.CurrentSpk)
                                }
                              $scope.stock = 'DO';
                                $scope.invoice = 'F. POL';
                         } else {
                             $scope.dataSummary = {
                                    STOCK: NumDigit($scope.summary.CurrentStock),
                                    INV: NumDigit($scope.summary.CurrentInv),
                                    INQ: NumDigit($scope.summary.CurrentInq),
                                    SPK:NumDigit($scope.summary.CurrentSpk)
                                }
                         }
                     }
                     ).error(function (a, b, c, d) {
                         console.log(a, b, c, d)
                     });
        }
        
    });
 
 
});// End of controller expense dashboard.



appControllers.controller('myDashboardSettingCtrl', function ($scope, $state, $location, $ionicHistory,$ionicViewSwitcher,$timeout, $mdUtil, $mdSidenav, $log, $ionicPlatform, $mdDialog, $mdBottomSheet, $mdMenu, $mdSelect) {

    $scope.navigateTo = function (stateName,objectData) {
	
		if (stateName=="exit"){
			
		} else
        if ($ionicHistory.currentStateName() != stateName) {
            $ionicHistory.nextViewOptions({
                disableAnimate: false,
                disableBack: true
            });

            //Next view animate will display in back direction
            $ionicViewSwitcher.nextDirection('back');

            $state.go(stateName, {
                isAnimated: objectData,
            });
        }
    }; // End of navigateTo.
	
	
	$scope.logout = function($event){
		$mdDialog.show({
            controller: 'DialogController',
            templateUrl: 'confirm-dialog.html',
            targetEvent: $event,
            locals: {
                displayOption: {
                    title: "Logout ?",
                    content: "Do you really want to logout ?",
                    ok: "Yes",
                    cancel: "Cancel"
                }
            }
		}).then(function () {
		    globalVariable.Token = '';
            $state.go('app.dashboard', {
                isAnimated: true 
            }, { reload: true });
        }, function () {
            $state.go('app.dashboard', {
                isAnimated: true,
            } );
        });		
	}
	
	
	$scope.changePassword = function($event){
		$mdDialog.show({
            controller: 'ChangePasswordController',
            templateUrl: 'change-password-dialog.html',
            targetEvent: $event,
            locals: {
                displayOption: {
                    title: "Change Password...",
                    content: "",
                    ok: "OK",
                    cancel: "Cancel"
                }
            }
        }).then(function () {
            $state.go('app.login', {
                isAnimated: true,
            });
        }, function () {
            $state.go('app.dashboard', {
                isAnimated: true,
            });
        });		
	}


});  
