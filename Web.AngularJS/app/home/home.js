'use strict';

angular.module('myApp.home', ['ngRoute'])

    .config(['$routeProvider', ($routeProvider) => {
        $routeProvider.when('/', {
            templateUrl: 'home/view.html',
            controller: 'HomeCtrl'
        });
    }])

    .controller('HomeCtrl', ['$scope',
        function ($scope) {
            $scope.announcements = [];
        }]);