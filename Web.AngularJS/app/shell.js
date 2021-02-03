'use strict';

angular.module('myApp.shell', [])

    .controller('ShellCtrl', ['$scope', '$window', '$document', 'AuthFactory',
        async function ($scope, $window, $document, authFactory) {
            $scope.minheight = ($window.innerHeight - 50) + 'px'; // 50px for the header
            angular.element($window).on('resize', () => {
                $scope.minheight = ($window.innerHeight - 50) + 'px';
            });

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

            await authFactory.configureClient();

            $scope.logout = authFactory.logout;

            $scope.isAuthenticated = await authFactory.isAuthenticated();
            $scope.$apply();

            if (!$scope.isAuthenticated) {
                authFactory.login();

                //const user = JSON.stringify(
                //    await authFactory.getUser()
                //);
                //console.log(user);
            }

            const query = $window.location.search;
            const shouldParseResult = query.includes("code=") && query.includes("state=");
            if (shouldParseResult) {
                await authFactory.handleRedirectCallback();
                $window.history.replaceState({}, $document.title, "/");
            }
        }]);