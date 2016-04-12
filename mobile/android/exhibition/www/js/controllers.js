angular.module('starter.controllers', ['LocalStorageModule'])

.controller('AppCtrl', function ($scope, $ionicModal, $timeout, $http, $state, $ionicLoading, localStorageService, BASE_ADDRESS) {
  
    $http.defaults.useXDomain = true;

    var _this = this;

  // Form data for the login modal
  $scope.loginData = {};

  // Create the login modal that we will use later
  $ionicModal.fromTemplateUrl('templates/login.html', {
    scope: $scope
  }).then(function(modal) {
      $scope.modal = modal;
      $scope.modal.show();
  });
 
  // Triggered in the login modal to close it
  $scope.closeLogin = function() {
    $scope.modal.hide();
  };

  // Open the login modal
  $scope.login = function() {
    $scope.modal.show();
  };


  // Perform the login action when the user submits the login form
  $scope.doLogin = function() {

      var url = BASE_ADDRESS + "/app/gettoken";
      $scope.loginData.grant_type = "password";

      $ionicLoading.show({
          template: 'loading'
      })

      $http.post(url, $scope.loginData).
        then(function (data) {
            var result = data.data;
            $ionicLoading.hide();
            if ((typeof(result) === "object") && (result !== null)) {
                if (result.access_token !== null) {
                    $http.defaults.headers.common['Authorization'] = 'Bearer ' + result.access_token;
                    $scope.closeLogin();
                    $state.go('app.welcome')
                }
            } else {
                console.log('Error')
                localStorageService.remove('token');
            }
        });
  };

})

.controller('PlaylistsCtrl', function ($scope, $http, BASE_ADDRESS) {

    $http.post(BASE_ADDRESS + "/suzuki/simdms/listevents").success(
    function (data) {
        $scope.exhibitions = data.data;
    }
    ).error(function (a, b, c, d) {
        console.log(a, b, c, d)
    })

})

.controller('dovsfpvsitsCtrl', function ($scope, $http, BASE_ADDRESS) {

    $http.post(BASE_ADDRESS + "/suzuki/simdms/DovsFPvsITS").success(
    function (data) {
        $scope.dovsfpvsits = data.data.Table[0];
        console.log($scope.dovsfpvsits)
    }
    ).error(function (a, b, c, d) {
        console.log(a, b, c, d)
    })

})


.controller('WelcomeCtrl', function ($scope, $http, BASE_ADDRESS) {


})

.controller('TipeCtrl', function ($scope, $stateParams, $http, BASE_ADDRESS, localStorageService) {

    console.log($stateParams)

    var currentdata = localStorageService.get('currentdata');
    $scope.Tipe = $stateParams.Tipe;
    $scope.listdetail = currentdata.Detail[$stateParams.Tipe];
    console.log($scope.listdetail)

})

.controller('PlaylistCtrl', function ($scope, $stateParams, $http, BASE_ADDRESS, localStorageService) {

    $scope.param = $stateParams;

    $http.post(BASE_ADDRESS + "/suzuki/simdms/exhibitionresult?id=" + $scope.param.Id).success(
    function (data) {
        $scope.exhibitions = data.data;
        $scope.data = {};

        if ($scope.exhibitions.Table !== undefined) {
            $scope.data.Total = $scope.exhibitions.Table[0].Total;
        }

        if ($scope.exhibitions.Table1 !== undefined) {
            $scope.data.Top = $scope.exhibitions.Table1;
        }

        if ($scope.exhibitions.Table2 !== undefined) {
            $scope.data.SummaryByType = $scope.exhibitions.Table2;
        }

        if ($scope.exhibitions.Table3 !== undefined) {
            $scope.data.Detail = {}

            $.each($scope.exhibitions.Table3, function (k, v) {
                if ($scope.data.Detail[v.TipeKendaraan] === undefined) {
                    $scope.data.Detail[v.TipeKendaraan] = []
                }
                $scope.data.Detail[v.TipeKendaraan].push(v);
            })
        }

        localStorageService.set('currentdata', $scope.data);
        var myvalues = [];

        $.each($scope.data.Top, function (k, v) {
            myvalues.push(v.Total);
        });

        $('.sparkline').sparkline(myvalues, { type: 'bar', barColor: 'blue', barWidth: 5, barSpacing: 3, barHeight: 80 });

    }
    ).error(function (a, b, c, d) {
        
    })

});
