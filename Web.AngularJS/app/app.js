'use strict';

// Declare app level module which depends on views, and core components
angular.module('myApp', [
    'ngAnimate',
    'ngRoute',
    'ngSanitize',
    'ui.bootstrap',
    'angularMoment',

    'myApp.config',
    'myApp.utilities',
    'myApp.auth',
    'myApp.shell',
    'myApp.home',
    'myApp.car',
    'myApp.booking',
    'myApp.pending-booking-admin',
    'myApp.pending-booking',
    'myApp.payment-admin',
    'myApp.payment'
])

    .config(['$locationProvider', '$routeProvider',
        function ($locationProvider, $routeProvider) {
            $locationProvider.hashPrefix('!');

            $routeProvider.otherwise({ redirectTo: '/' });
        }]);