import {UserFormHandler} from '/js/user-form-handler.js';

const handler = new UserFormHandler({
    formId: 'user-form',
    inputId: 'user-name',
    apiUrl: 'https://localhost:7025/users',
    containerSelector: '.user-cards'
});

handler.init();