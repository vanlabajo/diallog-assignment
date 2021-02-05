'use strict';

angular.module('myApp.utilities', [])
    .factory('SpinnerFactory', ['$rootScope',
        function ($rootScope) {
            let factory = {};

            factory.show = async () => {
                $rootScope.$broadcast('spinner.toggle', { show: true });
            };

            factory.hide = () => {
                $rootScope.$broadcast('spinner.toggle', { show: false });
            };

            return factory;
        }]);