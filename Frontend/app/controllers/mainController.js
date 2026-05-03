'use strict';

/**
 * MainController
 *
 * Demonstrates async/await usage in AngularJS 1.x.
 * After each awaited operation we call safeApply() to push changes into
 * Angular's digest cycle — the standard pattern when mixing native Promises
 * (or async/await) with AngularJS scope updates.
 */
angular.module('dormitoryApp')
    .controller('MainController', ['$scope', 'DormitoryService',
    function ($scope, DormitoryService) {
        const vm = this;

        vm.dormitories         = [];
        vm.filteredDormitories = [];
        vm.cities              = [];
        vm.suggestions         = [];
        vm.searchCity          = '';
        vm.showSuggestions     = false;
        vm.isLoading           = false;
        vm.error               = null;

        // ── Helpers ──────────────────────────────────────────────────────────

        /**
         * Safely triggers a digest cycle whether or not one is already running.
         * Required when scope is updated after an awaited (non-Angular) Promise.
         */
        function safeApply(fn) {
            const phase = $scope.$root.$$phase;
            if (phase === '$apply' || phase === '$digest') {
                fn();
            } else {
                $scope.$apply(fn);
            }
        }

        /**
         * Maps a fetch/network error to a user-friendly Hebrew message.
         * @param {Error} err
         * @returns {string}
         */
        function resolveErrorMessage(err) {
            if (!err.status || err.name === 'TypeError') {
                return 'לא ניתן להתחבר לשרת. אנא בדוק את החיבור ונסה שוב.';
            }
            if (err.status === 500) {
                return 'שגיאת שרת פנימית. אנא נסה שוב מאוחר יותר.';
            }
            if (err.status === 503) {
                return 'השרת אינו זמין כרגע. אנא נסה שוב בעוד מספר דקות.';
            }
            if (err.status === 400) {
                return `שגיאת ולידציה: ${err.message}`;
            }
            return `שגיאה בלתי צפויה (${err.status || err.message}).`;
        }

        // ── Public API (bound to view via controllerAs) ───────────────────────

        /**
         * Loads dormitory summaries from the API.
         * Uses async/await — digest is triggered manually via safeApply().
         */
        vm.loadDormitories = async function () {
            safeApply(function () {
                vm.isLoading = true;
                vm.error     = null;
            });

            try {
                const data = await DormitoryService.getDormitories();
                safeApply(function () {
                    vm.dormitories         = data;
                    vm.filteredDormitories = data;
                    vm.cities              = [...new Set(data.map(d => d.city))].sort();
                    vm.isLoading           = false;
                });
            } catch (err) {
                safeApply(function () {
                    vm.isLoading = false;
                    vm.error     = resolveErrorMessage(err);
                });
            }
        };

        /**
         * Called on every keystroke in the search input.
         * Updates the autocomplete suggestion list and filters the table — client-side only.
         */
        vm.onSearchChange = function () {
            const query = (vm.searchCity || '').toLowerCase().trim();

            if (!query) {
                vm.filteredDormitories = vm.dormitories;
                vm.suggestions         = [];
                return;
            }

            vm.suggestions = vm.cities.filter(c =>
                c.toLowerCase().includes(query)
            );

            vm.filteredDormitories = vm.dormitories.filter(d =>
                d.city.toLowerCase().includes(query)
            );
        };

        /**
         * Called when the user clicks a suggestion.
         * @param {string} city
         */
        vm.selectCity = function (city) {
            vm.searchCity          = city;
            vm.showSuggestions     = false;
            vm.suggestions         = [];
            vm.filteredDormitories = vm.dormitories.filter(d =>
                d.city.toLowerCase() === city.toLowerCase()
            );
        };

        /** Clears the search box and restores the full list. */
        vm.clearSearch = function () {
            vm.searchCity          = '';
            vm.suggestions         = [];
            vm.showSuggestions     = false;
            vm.filteredDormitories = vm.dormitories;
        };

        // Close suggestion dropdown when clicking outside the autocomplete widget
        document.addEventListener('click', function (e) {
            if (!e.target.closest('.autocomplete-wrapper')) {
                safeApply(function () {
                    vm.showSuggestions = false;
                });
            }
        });

        // Initial data load
        vm.loadDormitories();
    }]);
