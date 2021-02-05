'use strict';

angular.module('myApp.car', ['ngRoute'])

    .config(['$routeProvider', function ($routeProvider) {
        $routeProvider.when('/cars', {
            templateUrl: 'car/view.html',
            controller: 'CarCtrl'
        });
        $routeProvider.when('/car/form', {
            templateUrl: 'car/form.html',
            controller: 'CarFormCtrl'
        });
    }])

    .factory('CarFactory', [
        function () {
            let factory = {};
            let dirtyObject = null;

            factory.setDirtyObject = object => {
                dirtyObject = object;
            };

            factory.getDirtyObject = () => dirtyObject;

            return factory;
        }])

    .controller('CarCtrl', ['$scope', '$http', '$location', 'SpinnerFactory', 'AuthFactory', 'CarFactory',
        function ($scope, $http, $location, spinnerFactory, authFactory, carFactory) {
            const url = 'http://localhost:62080/api/cars';

            $scope.cars = [];
            $scope.searchByType;

            $scope.fetch = async function () {
                spinnerFactory.show();
                const token = await authFactory.getToken();
                $http.get(url, {
                    headers: { Authorization: `Bearer ${token}` }
                }).then(function (response) {
                    $scope.cars = response.data;
                }, function (response) {
                    console.log(response);
                    spinnerFactory.hide();
                }).finally(function () { spinnerFactory.hide(); });
            };

            activate();

            function activate() {
                $scope.fetch();
            }

            $scope.fetchByType = async function () {
                if ($scope.searchByType) {
                    $scope.cars = [];
                    spinnerFactory.show();
                    const token = await authFactory.getToken();
                    $http.get(url + '/' + $scope.searchByType, {
                        headers: { Authorization: `Bearer ${token}` }
                    }).then(function (response) {
                        $scope.cars = response.data;
                    }, function (response) {
                        console.log(response);
                        spinnerFactory.hide();
                    }).finally(function () { spinnerFactory.hide(); });
                } else {
                    $scope.fetch();
                }
            };

            $scope.update = function (car) {
                carFactory.setDirtyObject(car);
                $location.path('/car/form');
            };

            $scope.add = function () {
                carFactory.setDirtyObject({
                    id: 0,
                    type: 'Sedan',
                    size: 0,
                    gasConsumption: '',
                    dailyRentalCost: 0,
                    numberOfUnits: 0
                });
                $location.path('/car/form');
            };
        }])

    .controller('CarFormCtrl', ['$scope', '$http', '$location', 'SpinnerFactory', 'DialogFactory', 'AuthFactory', 'CarFactory',
        function ($scope, $http, $location, spinnerFactory, dialogFactory, authFactory, carFactory) {
            const url = 'http://localhost:62080/api/cars';

            $scope.isWorking = false;
            $scope.validationErrors = {};
            $scope.dirtyObject = carFactory.getDirtyObject();

            activate();

            function activate() {
                if ($scope.dirtyObject === null) {
                    $location.path('/cars');
                    return;
                }
                spinnerFactory.hide();
            }

            $scope.types = ['Sedan', 'SUV', 'Pickup'];

            $scope.submit = function () {
                dialogFactory.confirmationDialog('Submit', 'Are you sure you want to submit?', 'Yes', 'No')
                    .then(async function () {
                        $scope.isWorking = true;
                        spinnerFactory.show();
                        const token = await authFactory.getToken();
                        const model = angular.copy($scope.dirtyObject);
                        if ($scope.dirtyObject.id === 0) {
                            $http.post(url, model, {
                                headers: { Authorization: `Bearer ${token}` }
                            }).then(function (response) {
                                if (response.data.success) {
                                    dialogFactory.alertDialog('Success', 'Successfully added car.');
                                    $scope.validationErrors = {};
                                    $location.path("/cars");
                                }
                                else {
                                    dialogFactory.alertDialog('Error', 'Error in adding car. Please check for validation errors and try again.');
                                    $scope.validationErrors = response.data.validationErrors;                                    
                                }
                            }, function (response) {
                                console.log(response);
                                spinnerFactory.hide();
                            }).finally(function () { spinnerFactory.hide(); $scope.isWorking = false; });
                        } else {
                            $http.put(url + '/' + model.id, model, {
                                headers: { Authorization: `Bearer ${token}` }
                            }).then(function (response) {
                                if (response.data.success) {
                                    dialogFactory.alertDialog('Success', 'Successfully updated car.');
                                    $scope.validationErrors = {};
                                    $location.path("/cars");
                                }
                                else {
                                    dialogFactory.alertDialog('Error', 'Error in updating car. Please check for validation errors and try again.');
                                    $scope.validationErrors = response.data.validationErrors;
                                }
                            }, function (response) {
                                console.log(response);
                                spinnerFactory.hide();
                            }).finally(function () { spinnerFactory.hide(); $scope.isWorking = false; });
                        }
                    });
            };

            $scope.cancel = function () {
                carFactory.setDirtyObject(null);
                $location.path("/cars");
            };
        }]);