'use strict';

angular.module('myApp.home', ['ngRoute'])

    .config(['$routeProvider', ($routeProvider) => {
        $routeProvider.when('/', {
            templateUrl: 'home/view.html',
            controller: 'HomeCtrl'
        });
    }])

    .controller('HomeCtrl', ['$scope', 'SpinnerFactory',
        function ($scope, spinnerFactory) {
            $scope.announcements = [];

            activate();

            function activate() {
                spinnerFactory.hide();
            }

        }]);