import {UserHandler} from './user-handler.js';

const userHandler = new UserHandler({
    formId: 'user-form',
    inputId: 'user-name',
    apiUrl: 'https://localhost:7025/users',
    containerSelector: '.user-cards'
});

userHandler.init();