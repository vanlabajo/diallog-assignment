'use strict';

angular.module('myApp.auth', [])
    .factory('AuthFactory', ['auth0.config', '$window',
        function (config, $window) {
            let factory = {};
            let auth0 = null;

            factory.configureClient = async () => {
                auth0 = await createAuth0Client({
                    domain: config.domain,
                    client_id: config.clientId
                });
            };

            factory.isAuthenticated = async () => {
                if (auth0 !== null) {
                    return await auth0.isAuthenticated();
                }
                return false;
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

            factory.getUser = async () => {
                await auth0.getUser();
            };

            return factory;
        }]);