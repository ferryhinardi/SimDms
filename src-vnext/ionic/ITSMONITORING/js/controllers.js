angular.module('starter.controllers', ['LocalStorageModule'])

.controller('AppCtrl', function ($scope, $ionicModal, $timeout, $http, $state, $mdToast, $ionicLoading, localStorageService, BASE_ADDRESS) {

    $http.defaults.useXDomain = true;

    var _this = this;

    // Form data for the login modal
    $scope.loginData = {
       // UserName: 'admin',
       // Password: 'M45t3r'
    };

    // Create the login modal that we will use later
    $ionicModal.fromTemplateUrl('templates/login.html', {
        scope: $scope
    }).then(function (modal) {
        $scope.modal = modal;
        $scope.modal.show();
    });

    // Triggered in the login modal to close it
    $scope.closeLogin = function () {
        $scope.modal.hide();
    };

    // Open the login modal
    $scope.login = function () {
        $scope.modal.show();
    };


    // Perform the login action when the user submits the login form
    $scope.doLogin = function () {

        var url = BASE_ADDRESS + "/app/gettoken";
        $scope.loginData.grant_type = "password";

        $ionicLoading.show({
            template: 'loading'
        })

        $http.post(url, $scope.loginData).
          then(function (data) {
              var result = data.data;
              $ionicLoading.hide();
              if ((typeof (result) === "object") && (result !== null)) {
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

.controller('ChangePasswordCtrl', function ($scope, $http, BASE_ADDRESS, $mdToast) {
    $scope.ForgotPassword = {};
    $scope.doChangePassword = function() {
        var newPassowrd = $scope.ForgotPassword.Password;
        var confirmPassowrd = $scope.ForgotPassword.ConfirmPassword;
        if (newPassowrd != confirmPassowrd) {
            $mdToast.show({
                controller: 'toastController',
                templateUrl: 'toast.html',
                hideDelay: 10000,
                position: 'bottom',
                locals: {
                    displayOption: {
                        title: 'Error, Confirm Password and New Password not match!'
                    }
                }
            });
        }
        else {
            var data = {
                UserName : $scope.ForgotPassword.UserName,
                Password : $scope.ForgotPassword.Password    
            }
            $http.post(BASE_ADDRESS + "/suzuki/simdms/changepassword", data)
            .success(function(result) {
                console.log(result);
            })
            .error(function (a, b, c, d) {
                console.log(a, b, c, d)
            })
        }
    };
})

.controller('dovsfpvsitsCtrl', function ($scope, $http, BASE_ADDRESS, $timeout, ionicMaterialMotion, ionicMaterialInk) {
    $scope.listComparation = [];

    $http.post(BASE_ADDRESS + "/suzuki/simdms/DovsFPvsITS").success(
    function (ret) {
        var data = ret.data.Table[0];
        $scope.listComparation = [];

        var dataDO = {
            name: 'DO',
            color: 'mdc-bg-purple-A700',
            currName: moment(data.Current).format('MMM').toUpperCase() + ' ' + data.CurrDate,
            prevName: moment(data.Previous).format('MMM').toUpperCase() + ' ' + data.CurrDate,
            prevNameTotal: moment(data.Previous).format('MMM').toUpperCase() + ' SUM',
            currToDate: NumDigit(data.CurrentDO),
            prevToDate: NumDigit(data.LastDateDO),
            prevTotal: NumDigit(data.PreviousDO),
            CmpToDate: NumDigit((data.CurrentDO / data.LastDateDO) * 100) + '%',
            CmpToTotal: NumDigit((data.CurrentDO / data.PreviousDO) * 100) + '%'
        }
        
        $scope.listComparation.push(dataDO);

        var dataFP = {
            name: 'FP',
            color: 'mdc-bg-deep-purple-900',
            currName: moment(data.Current).format('MMM').toUpperCase() + ' ' + data.CurrDate,
            prevName: moment(data.Previous).format('MMM').toUpperCase() + ' ' + data.CurrDate,
            prevNameTotal: moment(data.Previous).format('MMM').toUpperCase() + ' SUM',
            currToDate: NumDigit(data.CurrentFP),
            prevToDate: NumDigit(data.LastDateFP),
            prevTotal: NumDigit(data.PreviousFP),
            CmpToDate: NumDigit((data.CurrentFP / data.LastDateFP) * 100) + '%',
            CmpToTotal: NumDigit((data.CurrentFP / data.PreviousFP) * 100) + '%'
        }

        $scope.listComparation.push(dataFP);

        var dataInq = {
            name: 'INQ',
            color: 'mdc-bg-light-green-900',
            currName: moment(data.Current).format('MMM').toUpperCase() + ' ' + data.CurrDate,
            prevName: moment(data.Previous).format('MMM').toUpperCase() + ' ' + data.CurrDate,
            prevNameTotal: moment(data.Previous).format('MMM').toUpperCase() + ' SUM',
            currToDate: NumDigit(data.CurrentInq),
            prevToDate: NumDigit(data.LastDateInq),
            prevTotal: NumDigit(data.PreviousInq),
            CmpToDate: NumDigit((data.CurrentInq / data.LastDateInq) * 100) + '%',
            CmpToTotal: NumDigit((data.CurrentInq / data.PreviousInq) * 100) + '%'
        }

        $scope.listComparation.push(dataInq);

        var dataSpk = {
            name: 'SPK',
            color: 'mdc-bg-indigo-900',
            currName: moment(data.Current).format('MMM').toUpperCase() + ' ' + data.CurrDate,
            prevName: moment(data.Previous).format('MMM').toUpperCase() + ' ' + data.CurrDate,
            prevNameTotal: moment(data.Previous).format('MMM').toUpperCase() + ' SUM',
            currToDate: NumDigit(data.CurrentSpk),
            prevToDate: NumDigit(data.LastDateSpk),
            prevTotal: NumDigit(data.PreviousSpk),
            CmpToDate: NumDigit((data.CurrentSpk / data.LastDateSpk) * 100) + '%',
            CmpToTotal: NumDigit((data.CurrentSpk / data.PreviousSpk) * 100) + '%'
        }

        $scope.listComparation.push(dataSpk);

        console.log($scope.listComparation)

        $timeout(function () {
            ionicMaterialMotion.fadeSlideIn({
                selector: '.animate-fade-slide-in .item'
            });
        }, 200);

        // Activate ink for controller
        ionicMaterialInk.displayEffect();

    }
    ).error(function (a, b, c, d) {
        console.log(a, b, c, d)
    })

})

.controller('dovsfpvsits2Ctrl', function ($scope, $http, BASE_ADDRESS, $timeout, ionicMaterialMotion, ionicMaterialInk) {
    $scope.listComparation = [];

    $http.post(BASE_ADDRESS + "/suzuki/simdms/DovsFPvsITS2").success(
    function (ret) {
        var data = ret.data.Table[0];
        $scope.listComparation = [];

        var dataDO = {
            name: 'DO',
            color: 'mdc-bg-purple-A700',
            currName: moment(data.Current).format('MMM').toUpperCase() + ' ' + data.CurrDate,
            prevName: moment(data.Previous).format('MMM').toUpperCase() + ' ' + data.PrevDate,
            prevNameTotal: moment(data.Previous).format('MMM').toUpperCase() + ' SUM',
            currToDate: NumDigit(data.CurrentDO),
            prevToDate: NumDigit(data.LastDateDO),
            prevTotal: NumDigit(data.PreviousDO),
            CmpToDate: NumDigit((data.CurrentDO / data.LastDateDO) * 100) + '%',
            CmpToTotal: NumDigit((data.CurrentDO / data.PreviousDO) * 100) + '%'
        }
        
        $scope.listComparation.push(dataDO);

        var dataFP = {
            name: 'FP',
            color: 'mdc-bg-deep-purple-900',
            currName: moment(data.Current).format('MMM').toUpperCase() + ' ' + data.CurrDate,
            prevName: moment(data.Previous).format('MMM').toUpperCase() + ' ' + data.PrevDate,
            prevNameTotal: moment(data.Previous).format('MMM').toUpperCase() + ' SUM',
            currToDate: NumDigit(data.CurrentFP),
            prevToDate: NumDigit(data.LastDateFP),
            prevTotal: NumDigit(data.PreviousFP),
            CmpToDate: NumDigit((data.CurrentFP / data.LastDateFP) * 100) + '%',
            CmpToTotal: NumDigit((data.CurrentFP / data.PreviousFP) * 100) + '%'
        }

        $scope.listComparation.push(dataFP);

        var dataInq = {
            name: 'INQ',
            color: 'mdc-bg-light-green-900',
            currName: moment(data.Current).format('MMM').toUpperCase() + ' ' + data.CurrDate,
            prevName: moment(data.Previous).format('MMM').toUpperCase() + ' ' + data.PrevDate,
            prevNameTotal: moment(data.Previous).format('MMM').toUpperCase() + ' SUM',
            currToDate: NumDigit(data.CurrentInq),
            prevToDate: NumDigit(data.LastDateInq),
            prevTotal: NumDigit(data.PreviousInq),
            CmpToDate: NumDigit((data.CurrentInq / data.LastDateInq) * 100) + '%',
            CmpToTotal: NumDigit((data.CurrentInq / data.PreviousInq) * 100) + '%'
        }

        $scope.listComparation.push(dataInq);

        var dataSpk = {
            name: 'SPK',
            color: 'mdc-bg-indigo-900',
            currName: moment(data.Current).format('MMM').toUpperCase() + ' ' + data.CurrDate,
            prevName: moment(data.Previous).format('MMM').toUpperCase() + ' ' + data.PrevDate,
            prevNameTotal: moment(data.Previous).format('MMM').toUpperCase() + ' SUM',
            currToDate: NumDigit(data.CurrentSpk),
            prevToDate: NumDigit(data.LastDateSpk),
            prevTotal: NumDigit(data.PreviousSpk),
            CmpToDate: NumDigit((data.CurrentSpk / data.LastDateSpk) * 100) + '%',
            CmpToTotal: NumDigit((data.CurrentSpk / data.PreviousSpk) * 100) + '%'
        }

        $scope.listComparation.push(dataSpk);

        console.log($scope.listComparation)

        $timeout(function () {
            ionicMaterialMotion.fadeSlideIn({
                selector: '.animate-fade-slide-in .item'
            });
        }, 200);

        // Activate ink for controller
        ionicMaterialInk.displayEffect();

    }
    ).error(function (a, b, c, d) {
        console.log(a, b, c, d)
    })

})

.controller('stcvsinvvsitsCtrl', function ($scope, $filter, $http, BASE_ADDRESS, $timeout, ionicMaterialMotion, ionicMaterialInk) {
    $scope.data = [];
    $scope.filterData = [];
    $scope.listComparation = [];
    $scope.ListBranchs = [];
    $scope.ListFilterBranchs = [];
    $scope.lastUpdateTime = "";
    $scope.currentCompany = "";
    $scope.flag = 0;
    
    $scope.options = {
        data: [],
        selectedOption: [],
        selectedOption2: [],
        DisplayType: "T"
    };
	
	$scope.loadDealer = function(cb) {
	
		$http.post(BASE_ADDRESS + "/suzuki/simdms/ListOutletDealer")
        .success(function (ret) {
			console.log(ret);
            $scope.options.data = ret.data.Table;
            $scope.ListBranchs = ret.data.Table1;
        })
        .error(function (a, b, c, d) {
            console.log(a, b, c, d)
        })

		$http.post(BASE_ADDRESS + "/suzuki/simdms/CurrentCompany")
        .success(function (ret) {
            var temp;
            $scope.currentCompany = ret;
			console.log($scope.currentCompany)
            if ($scope.currentCompany == "")
                $scope.disabledCompany = false;
            else {
                $scope.disabledCompany = true;
                temp = $filter('filter')($scope.options.data, { DealerCode: ret });
            }
        })
        .error(function (a, b, c, d) {
            console.log(a, b, c, d)
        })
		
		if (cb) cb();
	}
    
	$scope.loadDealer();

    $scope.
        $watch('options.selectedOption', function(company) {
            var CompanyCode = "ALL";
            if (company.length > 0)
                CompanyCode = company;
            $scope.selectedCompany = CompanyCode;
            $scope.ListFilterBranchs = $filter('filter')($scope.ListBranchs, { DealerCode: CompanyCode });
            $scope.selectedOutlet = "ALL";
            GetDataExecutiveSummary(null);
        });
    
    $scope.
        $watchGroup(['options.selectedOption2', 'options.DisplayType'], 
        function(newValues, oldValues, scope) {
            var OutletCode = (newValues[0] == null || newValues[0] == undefined || typeof newValues[0] != "string") ? "ALL" : newValues[0]; // OutletCode
            var DisplayType = (newValues[1] == null || newValues[1] == undefined) ? $scope.options.DisplayType : newValues[1];

            $scope.selectedOutlet = OutletCode;
            $scope.filterData = $filter('filter')($scope.data, { BranchCode: OutletCode, DisplayType: DisplayType });
            if ($scope.filterData.length == 0) 
                $scope.filterData = reset();
            else 
                $scope.filterData = $scope.filterData[0];
            GetExecutiveSummary();
        });

    $scope.doRefresh = function() {
		$scope.loadDealer(function(){
		
			if ($scope.currentCompany == "")
                $scope.disabledCompany = false;
            else {
                $scope.disabledCompany = true;
                temp = $filter('filter')($scope.options.data, { DealerCode: $scope.currentCompany });
            }
			
			GetDataExecutiveSummary(function() {
				$scope.$broadcast('scroll.refreshComplete');
			})			
		});

    };

    function GetDataExecutiveSummary(Callback) {
        if ($scope.selectedCompany == "ALL") $scope.filterData = reset();
        var BranchCode = (typeof $scope.selectedOutlet != "string") ? "ALL" : $scope.selectedOutlet;
        console.log($scope.selectedCompany, BranchCode, $scope.options.DisplayType);
        $http.post(BASE_ADDRESS + "/suzuki/simdms/DealerExecutiveSummary", {id: $scope.selectedCompany})
            .success(function (ret) {
				console.log(ret)
                $scope.data = ret.data.Table;
                $scope.filterData = $filter('filter')($scope.data, { BranchCode: BranchCode, DisplayType: $scope.options.DisplayType });
                if ($scope.filterData.length == 0) 
                    $scope.filterData = reset();
                else 
                    $scope.filterData = $scope.filterData[0];
                GetExecutiveSummary();
            })
            .error(function (a, b, c, d) {
                console.log(a, b, c, d)
            })
            .then(function() {
                GetExecutiveSummary();
                if (typeof Callback === "function") {
                    Callback();
                }
            })
    }

    function reset() {
        var data = {
            CurrentDate: new Date(),
            PrevMonthOfCurrDate: new Date(new Date().getFullYear(), new Date().getMonth()-1, new Date().getDate()),
            Prev2MonthOfCurrDate: new Date(new Date().getFullYear(), new Date().getMonth()-2, new Date().getDate()),
            PeriodOfCurrMonth: "1-" + new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate(),
            PeriodOfPrevMonth: "1-" + new Date(new Date().getFullYear(), new Date().getMonth(), 0).getDate(),
            CurrentStock: 0,
            LastDateStock: 0,
            PreviousStock: 0,
            Previous2Stock: 0,
            CurrentInv: 0,
            LastDateInv: 0,
            PreviousInv: 0,
            Previous2Inv: 0,
            CurrentInq: 0,
            LastDateInq: 0,
            PreviousInq: 0,
            Previous2Inq: 0,
            CurrentSpk: 0,
            LastDateSpk: 0,
            PreviousSpk: 0,
            Previous2Spk: 0,
            LastUpdateDate: ""
        };
        return data;
    }

    function GetExecutiveSummary() {
        $scope.listComparation = [];
        var data = $scope.filterData;
        // console.log(data);
        $scope.lastUpdateTime = (data.LastUpdateDate == "") ? "No Connection" : moment(data.LastUpdateDate, "YYYY-MM-DD hh:mm:ss").fromNow();
        var dataStock = {
            name: 'STOCK',
            color: 'mdc-bg-purple-A700',
            currName: moment(data.CurrentDate).format('MMM').toUpperCase() + ' ' + data.PeriodOfCurrMonth,
            prevName: moment(data.PrevMonthOfCurrDate).format('MMM').toUpperCase() + ' ' + data.PeriodOfPrevMonth,
            prevNameTotal: moment(data.PrevMonthOfCurrDate).format('MMM').toUpperCase() + ' SUM',
            prevName2Total: moment(data.Prev2MonthOfCurrDate).format('MMM').toUpperCase() + ' SUM',
            currToDate: NumDigit(data.CurrentStock),
            prevToDate: NumDigit(data.LastDateStock),
            prevTotal: NumDigit(data.PreviousStock),
            prev2Total: NumDigit(data.Previous2Stock),
            CmpToDate: (data.CurrentStock == 0 || data.LastDateStock == 0) ? "-" : NumDigit((data.CurrentStock / data.LastDateStock) * 100) + '%',
            CmpToTotal: (data.CurrentStock == 0 || data.PreviousStock == 0) ? "-" : NumDigit((data.CurrentStock / data.PreviousStock) * 100) + '%',
            CmpTo2Total: (data.CurrentStock == 0 || data.Previous2Stock == 0) ? "-" : NumDigit((data.CurrentStock / data.Previous2Stock) * 100) + '%'
        }
        $scope.listComparation.push(dataStock);
        var dataINV = {
            name: 'INV',
            color: 'mdc-bg-deep-purple-900',
            currName: moment(data.CurrentDate).format('MMM').toUpperCase() + ' ' + data.PeriodOfCurrMonth,
            prevName: moment(data.PrevMonthOfCurrDate).format('MMM').toUpperCase() + ' ' + data.PeriodOfPrevMonth,
            prevNameTotal: moment(data.PrevMonthOfCurrDate).format('MMM').toUpperCase() + ' SUM',
            prevName2Total: moment(data.Prev2MonthOfCurrDate).format('MMM').toUpperCase() + ' SUM',
            currToDate: NumDigit(data.CurrentInv),
            prevToDate: NumDigit(data.LastDateInv),
            prevTotal: NumDigit(data.PreviousInv),
            prev2Total: NumDigit(data.Previous2Inv),
            CmpToDate: (data.CurrentInv == 0 || data.LastDateInv == 0) ? "-" : NumDigit((data.CurrentInv / data.LastDateInv) * 100) + '%',
            CmpToTotal: (data.CurrentInv == 0 || data.PreviousInv == 0) ? "-" : NumDigit((data.CurrentInv / data.PreviousInv) * 100) + '%',
            CmpTo2Total: (data.CurrentInv == 0 || data.Previous2Inv == 0) ? "-" : NumDigit((data.CurrentInv / data.Previous2Inv) * 100) + '%'
        }
        $scope.listComparation.push(dataINV);
        var dataInq = {
            name: 'INQ',
            color: 'mdc-bg-light-green-900',
            currName: moment(data.CurrentDate).format('MMM').toUpperCase() + ' ' + data.PeriodOfCurrMonth,
            prevName: moment(data.PrevMonthOfCurrDate).format('MMM').toUpperCase() + ' ' + data.PeriodOfPrevMonth,
            prevNameTotal: moment(data.PrevMonthOfCurrDate).format('MMM').toUpperCase() + ' SUM',
            prevName2Total: moment(data.Prev2MonthOfCurrDate).format('MMM').toUpperCase() + ' SUM',
            currToDate: NumDigit(data.CurrentInq),
            prevToDate: NumDigit(data.LastDateInq),
            prevTotal: NumDigit(data.PreviousInq),
            prev2Total: NumDigit(data.Previous2Inq),
            CmpToDate: (data.CurrentInq == 0 || data.LastDateInq == 0) ? "-" : NumDigit((data.CurrentInq / data.LastDateInq) * 100) + '%',
            CmpToTotal: (data.CurrentInq == 0 || data.PreviousInq == 0) ? "-" : NumDigit((data.CurrentInq / data.PreviousInq) * 100) + '%',
            CmpTo2Total: (data.CurrentInq == 0 || data.Previous2Inq == 0) ? "-" : NumDigit((data.CurrentInq / data.Previous2Inq) * 100) + '%'
        }
        $scope.listComparation.push(dataInq);
        var dataSpk = {
            name: 'SPK',
            color: 'mdc-bg-indigo-900',
            currName: moment(data.CurrentDate).format('MMM').toUpperCase() + ' ' + data.PeriodOfCurrMonth,
            prevName: moment(data.PrevMonthOfCurrDate).format('MMM').toUpperCase() + ' ' + data.PeriodOfPrevMonth,
            prevNameTotal: moment(data.PrevMonthOfCurrDate).format('MMM').toUpperCase() + ' SUM',
            prevName2Total: moment(data.Prev2MonthOfCurrDate).format('MMM').toUpperCase() + ' SUM',
            currToDate: NumDigit(data.CurrentSpk),
            prevToDate: NumDigit(data.LastDateSpk),
            prevTotal: NumDigit(data.PreviousSpk),
            prev2Total: NumDigit(data.Previous2Spk),
            CmpToDate: (data.CurrentSpk == 0 || data.LastDateSpk == 0) ? "-" : NumDigit((data.CurrentSpk / data.LastDateSpk) * 100) + '%',
            CmpToTotal: (data.CurrentSpk == 0 || data.PreviousSpk == 0) ? "-" : NumDigit((data.CurrentSpk / data.PreviousSpk) * 100) + '%',
            CmpTo2Total: (data.CurrentSpk == 0 || data.Previous2Spk == 0) ? "-" : NumDigit((data.CurrentSpk / data.Previous2Spk) * 100) + '%'
        }
        $scope.listComparation.push(dataSpk);
        // console.log($scope.listComparation);
    
        $timeout(function () {
            ionicMaterialMotion.fadeSlideIn({
                selector: '.animate-fade-slide-in .item'
            });
        }, 200);

        // Activate ink for controller
        ionicMaterialInk.displayEffect();
    }
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

})

.controller('toastController', function ($scope, displayOption) {

    //this variable for display wording of toast.
    //object schema:
    // displayOption: {
    //    title: "Data Saved !"
    //}

    $scope.displayOption = displayOption;
});
