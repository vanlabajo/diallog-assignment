'use strict';

angular.module('myApp.pending-booking', ['ngRoute'])

    .config(['$routeProvider', function ($routeProvider) {
        $routeProvider.when('/pending-bookings', {
            templateUrl: 'pending-booking/view.html',
            controller: 'PendingBookingCtrl'
        });
    }])

    .controller('PendingBookingCtrl', ['$scope', '$http', 'SpinnerFactory', 'AuthFactory', 'DialogFactory',
        function ($scope, $http, spinnerFactory, authFactory, dialogFactory) {
            const url = 'http://localhost:62080/api/pendingbookings';

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
        }]);