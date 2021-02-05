'use strict';

angular.module('myApp.payment-admin', ['ngRoute'])

    .config(['$routeProvider', function ($routeProvider) {
        $routeProvider.when('/payments-admin', {
            templateUrl: 'payment-admin/view.html',
            controller: 'PaymentAdminCtrl'
        });
    }])

    .controller('PaymentAdminCtrl', ['$scope', '$http', '$location', 'SpinnerFactory', 'AuthFactory',
        function ($scope, $http, $location, spinnerFactory, authFactory) {
            const url = 'http://localhost:62080/api/paymentsadmin';

            $scope.payments = [];
            $scope.searchByReference;

            $scope.fetch = async function () {
                spinnerFactory.show();
                const token = await authFactory.getToken();
                $http.get(url, {
                    headers: { Authorization: `Bearer ${token}` }
                }).then(function (response) {
                    $scope.payments = response.data;
                }, function (response) {
                    console.log(response);
                    spinnerFactory.hide();
                }).finally(function () { spinnerFactory.hide(); });
            };

            activate();

            function activate() {
                $scope.fetch();
            }

            $scope.fetchByReference = async function () {
                if ($scope.searchByReference) {
                    $scope.payments = [];
                    spinnerFactory.show();
                    const token = await authFactory.getToken();
                    $http.get(url + '/' + $scope.searchByReference, {
                        headers: { Authorization: `Bearer ${token}` }
                    }).then(function (response) {
                        $scope.payments = response.data;
                    }, function (response) {
                        console.log(response);
                        spinnerFactory.hide();
                    }).finally(function () { spinnerFactory.hide(); });
                } else {
                    $scope.fetch();
                }
            };
        }]);