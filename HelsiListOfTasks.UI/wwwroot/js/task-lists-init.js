import { TaskListsHandler } from './task-lists-handler.js';

document.addEventListener("DOMContentLoaded", () => {
    const taskListsHandler = new TaskListsHandler({
        formElement: document.getElementById("task-lists-form"),
        inputElement: document.getElementById("task-lists-title"),
        apiUrl: "https://localhost:7025/task-lists",
        containerElement: document.querySelector(".task-lists-collection"),
        prevPageButton: document.getElementById("prev-page"),
        nextPageButton: document.getElementById("next-page"),
        pageNumberElement: document.getElementById("page-number")
    });

    taskListsHandler.init();
});
