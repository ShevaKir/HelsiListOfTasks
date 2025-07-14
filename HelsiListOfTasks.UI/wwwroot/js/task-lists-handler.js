export class TaskListsHandler {
    constructor({formId, inputId, apiUrl, containerSelector}) {
        this.form = document.getElementById(formId);
        this.input = document.getElementById(inputId);
        this.apiUrl = apiUrl;
        this.container = document.querySelector(containerSelector);
        this.userId = new URLSearchParams(window.location.search).get("userId");
    }

    init() {
        if (!this.form || !this.input || !this.container) {
            console.warn("TaskListsHandler: No form or container found");
            return;
        }

        if (!this.userId) {
            console.warn("TaskListsHandler: No userId in query string");
            return;
        }

        this.form.addEventListener("submit", this.handleSubmit.bind(this));
    }

    async handleSubmit(event) {
        event.preventDefault();

        const title = this.input.value.trim();
        if (!title) return;

        try {
            const response = await fetch(this.apiUrl, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "X-User-Id": this.userId
                },
                body: JSON.stringify({title})
            });

            if (!response.ok) {
                alert("Error creating user");
            }

            const taskLists = await response.json();
            this.addTaskLists(taskLists);
            this.input.value = "";
        } catch (err) {
            alert(err.message);
        }
    }

    addTaskLists(taskList) {
        const taskListElement = document.createElement("div");
        taskListElement.className = "task-lists";
        taskListElement.setAttribute("data-id", taskList.id);

        const sharedUsers = taskList.sharedWithUserIds ?? [];

        const sharedSection = sharedUsers.length > 0
            ? `<p>Sharing Users:</p>
           <ul>
               ${sharedUsers.map(userId => `<li>${userId}</li>`).join("")}
           </ul>`
            : `<p>The task list is only available to you.</p>`;

        taskListElement.innerHTML = `
            <div class="task-lists-controls">
                <div class="task-lists-delete" id="task-list-delete">
                    <img src="icons/delete.svg" alt="Delete">
                </div>
            </div>
            <h4>${taskList.title}</h4>
            <p>Created At: ${taskList.createdAt}</p>
            ${sharedSection}
        `;
        
        this.container.appendChild(taskListElement);
    }

}