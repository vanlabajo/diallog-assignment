(function () {
    'use strict';

    var bootstrapModule = angular.module('myApp.utilities');

    bootstrapModule.factory('DialogFactory', ['$uibModal', '$templateCache', modalDialog]);

    function modalDialog($uibModal, $templateCache) {
        var service = {
            deleteDialog: deleteDialog,
            confirmationDialog: confirmationDialog,
            alertDialog: alertDialog
        };

        $templateCache.put('modalDialog.tpl.html',
            '<div class="modal-header">' +
            '   <button type="button" class="close" data-dismiss="modal" aria-label="Close" ng-click="cancel()"><span aria-hidden="true">&times;</span></button>' +
            '   <h4 class="modal-title">{{title}}</h4>' +
            '</div>' +
            '<div class="modal-body">' +
            '   <p ng-bind-html="message"></p>' +
            '</div>' +
            '<div class="modal-footer">' +
            '   <button type="button" class="btn btn-default" data-dismiss="modal" ng-click="cancel()">{{cancelText}}</button>' +
            '   <button type="button" class="btn btn-primary" ng-click="ok()">{{okText}}</button>' +
            '</div>');

        $templateCache.put('alertDialog.tpl.html',
            '<div class="modal-header">' +
            '   <button type="button" class="close" data-dismiss="modal" aria-label="Close" ng-click="ok()"><span aria-hidden="true">&times;</span></button>' +
            '   <h4 class="modal-title">{{title}}</h4>' +
            '</div>' +
            '<div class="modal-body">' +
            '   <p ng-bind-html="message"></p>' +
            '</div>' +
            '<div class="modal-footer">' +
            '   <button type="button" class="btn btn-primary" ng-click="ok()">{{okText}}</button>' +
            '</div>');

        return service;

        function deleteDialog(itemName) {
            var title = 'Confirm Delete';
            itemName = itemName || 'item';
            var msg = 'Delete ' + itemName + '?';

            return confirmationDialog(title, msg);
        }

        function confirmationDialog(title, msg, okText, cancelText) {

            var modalOptions = {
                templateUrl: 'modalDialog.tpl.html',
                controller: ModalInstance,
                keyboard: true,
                resolve: {
                    options: function () {
                        return {
                            title: title,
                            message: msg,
                            okText: okText,
                            cancelText: cancelText
                        };
                    }
                }
            };

            return $uibModal.open(modalOptions).result; 
        }

        function alertDialog(title, msg) {
            var modalOptions = {
                templateUrl: 'alertDialog.tpl.html',
                controller: ModalInstance,
                keyboard: true,
                resolve: {
                    options: function () {
                        return {
                            title: title,
                            message: msg,
                            okText: 'OK',
                            cancelText: 'Cancel'
                        };
                    }
                }
            };

            return $uibModal.open(modalOptions).result;
        }
    }

    var ModalInstance = ['$scope', '$uibModalInstance', 'options',
        function ($scope, $uibModalInstance, options) {
            $scope.title = options.title || 'Title';
            $scope.message = options.message || '';
            $scope.okText = options.okText || 'OK';
            $scope.cancelText = options.cancelText || 'Cancel';
            $scope.ok = function () { $uibModalInstance.close('ok'); };
            $scope.cancel = function () { $uibModalInstance.dismiss('cancel'); };
        }];
})();