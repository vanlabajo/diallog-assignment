'use strict';

angular.module('myApp.auth', [])
    .factory('AuthFactory', ['auth0.config', '$window', '$rootScope',
        function (config, $window, $rootScope) {
            let factory = {};
            let auth0 = null;
            let user = null;

            factory.configureClient = async () => {
                auth0 = await createAuth0Client({
                    domain: config.domain,
                    client_id: config.clientId,
                    audience: config.audience
                });
            };

            factory.isAuthenticated = async () => {
                let result = false;
                if (auth0 !== null) {
                    const isAuthenticated = await auth0.isAuthenticated();
                    result = isAuthenticated;
                    $rootScope.$broadcast('auth.isauthenticated', isAuthenticated)

                    if (isAuthenticated) {
                        user = await auth0.getUser();
                        if (user !== null) {
                            const roles = user['https://diallog.com/roles'];
                            if (roles !== undefined) {
                                user.admin = roles.includes('admin');
                            }
                        }
                    }
                }
                return result;
            };

            factory.login = async () => {
                await auth0.loginWithRedirect({
                    redirect_uri: $window.location.origin
                });
            };

            factory.logout = () => {
                auth0.logout({
                    returnTo: $window.location.origin
                });
            };

            factory.handleRedirectCallback = async () => {
                await auth0.handleRedirectCallback();
            };

            factory.getUser = () => user;

            factory.getToken = async () => {
                const token = await auth0.getTokenSilently();
                return token;
            };

            return factory;
        }]);