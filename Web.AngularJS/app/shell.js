'use strict';

angular.module('myApp.shell', [])

    .controller('ShellCtrl', ['$scope', '$rootScope', '$window', '$document', '$location', 'AuthFactory', '$timeout',
        async function ($scope, $rootScope, $window, $document, $location, authFactory, $timeout) {
            $scope.isBusy = true;
            $scope.isAuthenticated = false;
            $scope.user = null;

            $scope.minheight = ($window.innerHeight - 50) + 'px'; // 50px for the header
            angular.element($window).on('resize', () => {
                $scope.minheight = ($window.innerHeight - 50) + 'px';
            });

            activate();

            async function activate() {
                await authFactory.configureClient();

                const query = $window.location.search;
                const shouldParseResult = query.includes('code=') && query.includes('state=');
                if (shouldParseResult) {
                    await authFactory.handleRedirectCallback();
                    $window.history.replaceState({}, $document[0].title, '/');  
                    $window.location.search = '';                  
                }

                await authFactory.isAuthenticated();

                toggleSpinner(false);
            }

            $scope.searchMenu = (searchTerm) => {
                angular.element('.sidebar-menu > li').not('.header').each(function () {
                    if (angular.element(this).text().search(new RegExp(searchTerm, 'i')) < 0) {
                        angular.element(this).hide();
                    } else {
                        angular.element(this).show();
                        if (angular.element(this).hasClass('treeview')) {
                            angular.element(this).children('ul').find('li').each(() => {
                                if (angular.element(this).text().search(new RegExp(searchTerm, 'i')) < 0) {
                                    angular.element(this).hide();
                                } else {
                                    angular.element(this).show();
                                }
                            });
                        }
                    }
                });
            };

            $scope.login = authFactory.login;
            $scope.logout = authFactory.logout;

            function toggleSpinner(show) { $scope.isBusy = show; }

            $rootScope.$on('$routeChangeStart', () => {
                toggleSpinner(true);
            });

            $rootScope.$on('spinner.toggle', (_event, data) => {
                toggleSpinner(data.show);
            });

            $rootScope.$on('$locationChangeStart', (event) => {
                if ($location.path() !== '/' && !$scope.isAuthenticated) {
                    event.preventDefault();
                    toggleSpinner(false);
                }
                else if ($location.path() === '') {
                    event.preventDefault();
                    toggleSpinner(false);
                }
            });

            $rootScope.$on('auth.isauthenticated', (_event, data) => {
                $timeout(() => {
                    toggleSpinner(true);
                });
                $timeout(() => {
                    $scope.isAuthenticated = data;

                    if ($scope.isAuthenticated) {
                        $scope.user = authFactory.getUser();
                        if ($scope.user !== null && $scope.user !== undefined) {
                            if ($scope.user.picture === null || $scope.user.picture === undefined || $scope.user.picture === '') {
                                $scope.user.picture = 'lib/adminlte/img/avatar5.png';
                            }
                        }

                        console.log($scope.user);
                    }

                    $location.path('/');
                    toggleSpinner(false);
                }, 1800);
            });
        }]);