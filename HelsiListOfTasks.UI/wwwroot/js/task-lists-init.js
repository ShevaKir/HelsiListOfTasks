import { TaskListsHandler } from './task-lists-handler.js';
document.addEventListener("DOMContentLoaded", () => {
    const taskListsHandler = new TaskListsHandler({
        formId: "task-lists-form",
        inputId: "task-lists-title",
        apiUrl: "https://localhost:7025/task-lists",
        containerSelector: ".task-lists-collection"
    });

    taskListsHandler.init();
});