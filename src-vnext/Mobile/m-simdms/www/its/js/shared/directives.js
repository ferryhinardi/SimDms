appControllers.directive('numbersOnly', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attr, ngModelCtrl) {
            function fromUser(text) {
                if (text) {
                    var transformedInput = text.replace(/[^0-9]/g, '');

                    if (transformedInput !== text) {
                        ngModelCtrl.$setViewValue(transformedInput);
                        ngModelCtrl.$render();
                    }
                    return transformedInput;
                }
                return undefined;
            }

            ngModelCtrl.$parsers.push(fromUser);
        }
    };
});

appControllers.factory('permissions', function ($rootScope) {
  var permissionList;
  return {
    setPermissions: function(permissions) {
      permissionList = permissions;
      $rootScope.$broadcast('permissionsChanged');
    },
    hasPermission: function (permission) {
      permission = permission.trim();
      return _.some(permissionList, function(item) {
        if(_.isString(item.Name)) {
          return item.Name.trim() === permission
        }
      });
    }
  };
});

appControllers.directive('hasPermission', function(permissions) {  
  return {
    link: function(scope, element, attrs) {
      if(!_.isString(attrs.hasPermission)) {
        throw 'hasPermission value must be a string'
      }
      var value = attrs.hasPermission.trim();
      var notPermissionFlag = value[0] === '!';
      if(notPermissionFlag) {
        value = value.slice(1).trim();
      }

      function toggleVisibilityBasedOnPermission() {
          var hasPermission = permissions.hasPermission(value);
          if (hasPermission && !notPermissionFlag || !hasPermission && notPermissionFlag) {
          element.show();
        }
        else {
          element.hide();
        }
      }

      toggleVisibilityBasedOnPermission();
      scope.$on('permissionsChanged', toggleVisibilityBasedOnPermission);
    }
  };
});
 