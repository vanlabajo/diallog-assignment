'use strict';

angular.module('myApp.booking', ['ngRoute'])

    .config(['$routeProvider', function ($routeProvider) {
        $routeProvider.when('/bookings', {
            templateUrl: 'booking/view.html',
            controller: 'BookingCtrl'
        });
        $routeProvider.when('/booking/form', {
            templateUrl: 'booking/form.html',
            controller: 'BookingFormCtrl'
        });
    }])

    .factory('BookingFactory', [
        function () {
            let factory = {};
            let dirtyObject = null;

            factory.setDirtyObject = object => {
                dirtyObject = object;
            };

            factory.getDirtyObject = () => dirtyObject;

            return factory;
        }])

    .controller('BookingCtrl', ['$scope', '$http', '$location', 'SpinnerFactory', 'AuthFactory', 'BookingFactory', 'DialogFactory',
        function ($scope, $http, $location, spinnerFactory, authFactory, bookingFactory, dialogFactory) {
            const url = 'http://localhost:62080/api/bookings';

            $scope.bookings = [];
            $scope.searchByReference;

            $scope.fetch = async function () {
                spinnerFactory.show();
                const token = await authFactory.getToken();
                $http.get(url, {
                    headers: { Authorization: `Bearer ${token}` }
                }).then(function (response) {
                    $scope.bookings = response.data;
                    angular.forEach($scope.bookings, function (booking) {
                        booking.totalPayment = 0;
                        if (booking.payments.length > 0) {
                            angular.forEach(booking.payments, function (payment) {
                                booking.totalPayment += payment.amount;
                            });
                        }
                    });
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
                    $scope.bookings = [];
                    spinnerFactory.show();
                    const token = await authFactory.getToken();
                    $http.get(url + '/' + $scope.searchByReference, {
                        headers: { Authorization: `Bearer ${token}` }
                    }).then(function (response) {
                        $scope.bookings = response.data;
                        angular.forEach($scope.bookings, function (booking) {
                            booking.totalPayment = 0;
                            if (booking.payments.length > 0) {
                                angular.forEach(booking.payments, function (payment) {
                                    booking.totalPayment += payment.amount;
                                });
                            }
                        });
                    }, function (response) {
                        console.log(response);
                        spinnerFactory.hide();
                    }).finally(function () { spinnerFactory.hide(); });
                } else {
                    $scope.fetch();
                }
            };

            $scope.update = function (booking) {
                bookingFactory.setDirtyObject(booking);
                $location.path('/booking/form');
            };

            $scope.add = function () {
                bookingFactory.setDirtyObject({
                    id: 0,
                    guestName: authFactory.getUser().email,
                    car: null,
                    startDateUtc: new Date(),
                    endDateUtc: new Date()
                });
                $location.path('/booking/form');
            };

            $scope.makePayment = async function (booking) {
                const yes = await dialogFactory.confirmationDialog('Payment', 'Are you sure you want to make payment?', 'Yes', 'No');
                if (yes) {
                    const paymentUrl = 'http://localhost:62080/api/bookingpayments';
                    const token = await authFactory.getToken();

                    spinnerFactory.show();
                    $http.post(paymentUrl, booking, {
                        headers: { Authorization: `Bearer ${token}` }
                    }).then(function (response) {
                        if (response.data.success)
                            dialogFactory.alertDialog('Success', 'Successfully paid booking.');
                        else
                            dialogFactory.alertDialog('Error', 'Error in making payment. Please try again later.');

                        $scope.fetch();
                    }, function (response) {
                        console.log(response);
                        spinnerFactory.hide();
                    }).finally(function () { spinnerFactory.hide(); $scope.isWorking = false; });
                }                
            };

            $scope.cancel = async function (booking) {
                const yes = await dialogFactory.confirmationDialog('Cancellation', 'Are you sure you want to cancel this booking?', 'Yes', 'No');
                if (yes) {
                    const cancellationUrl = 'http://localhost:62080/api/bookingcancellations';
                    const token = await authFactory.getToken();

                    spinnerFactory.show();
                    $http.post(cancellationUrl, booking, {
                        headers: { Authorization: `Bearer ${token}` }
                    }).then(function (response) {
                        if (response.data.success)
                            dialogFactory.alertDialog('Success', 'Successfully cancelled booking.');
                        else
                            dialogFactory.alertDialog('Error', 'Error in cancelling booking. Please try again later.');

                        $scope.fetch();
                    }, function (response) {
                        console.log(response);
                        spinnerFactory.hide();
                    }).finally(function () { spinnerFactory.hide(); $scope.isWorking = false; });
                }
            };
        }])

    .controller('BookingFormCtrl', ['$scope', '$http', '$location', 'SpinnerFactory', 'DialogFactory', 'AuthFactory', 'BookingFactory',
        function ($scope, $http, $location, spinnerFactory, dialogFactory, authFactory, bookingFactory) {
            const url = 'http://localhost:62080/api/bookings';

            $scope.isWorking = false;
            $scope.validationErrors = {};
            $scope.dirtyObject = bookingFactory.getDirtyObject();
            $scope.cars = [];

            activate();

            async function activate() {
                if ($scope.dirtyObject === null) {
                    $location.path('/bookings');
                    return;
                }

                if ($scope.dirtyObject.startDateUtc && $scope.dirtyObject.startDateUtc !== '') {
                    $scope.dirtyObject.startDateUtc = moment.utc($scope.dirtyObject.startDateUtc).local().toDate();
                    $scope.dirtyObject.endDateUtc = moment.utc($scope.dirtyObject.endDateUtc).local().toDate();
                }

                const carsUrl = 'http://localhost:62080/api/cars';
                const token = await authFactory.getToken();
                $http.get(carsUrl, {
                    headers: { Authorization: `Bearer ${token}` }
                }).then(function (response) {
                    $scope.cars = response.data;
                }, function (response) {
                    console.log(response);
                    spinnerFactory.hide();
                }).finally(function () { spinnerFactory.hide(); });
            }

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
                                    dialogFactory.alertDialog('Success', 'Successfully saved booking.');
                                    $scope.validationErrors = {};
                                    $location.path("/bookings");
                                }
                                else {
                                    dialogFactory.alertDialog('Error', 'Error in saving booking. Please check for validation errors and try again.');
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
                                    dialogFactory.alertDialog('Success', 'Successfully saved booking.');
                                    $scope.validationErrors = {};
                                    $location.path("/bookings");
                                }
                                else {
                                    dialogFactory.alertDialog('Error', 'Error in updating booking. Please check for validation errors and try again.');
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
                bookingFactory.setDirtyObject(null);
                $location.path("/bookings");
            };
        }]);