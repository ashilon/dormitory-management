'use strict';

/**
 * DormitoryService
 * Uses the native Fetch API with async/await for clean, readable async code.
 * Translates HTTP error statuses into typed Error objects so the controller
 * can produce user-friendly Hebrew error messages.
 */
angular.module('dormitoryApp')
    .service('DormitoryService', [function () {
        const API_BASE = 'http://localhost:5000/api';

        /**
         * Fetches all education places with student statistics.
         * @returns {Promise<Array<EducationPlaceSummaryDto>>}
         */
        this.getDormitories = async function () {
            const response = await fetch(`${API_BASE}/education-places`);
            if (!response.ok) {
                const err = new Error(`HTTP ${response.status}`);
                err.status = response.status;
                throw err;
            }
            return response.json();
        };

        /**
         * Upserts a student record.
         * Pass { id: null } or omit id to insert; pass a valid id to update.
         * @param {Object} student
         * @returns {Promise<Student>}
         */
        this.upsertStudent = async function (student) {
            const response = await fetch(`${API_BASE}/students`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(student)
            });
            if (!response.ok) {
                const body = await response.json().catch(() => ({}));
                const err  = new Error(body.message || `HTTP ${response.status}`);
                err.status = response.status;
                throw err;
            }
            return response.json();
        };
    }]);
