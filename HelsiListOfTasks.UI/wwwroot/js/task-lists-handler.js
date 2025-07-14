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

        this.container.querySelectorAll(".task-lists").forEach(taskListEl => {
            const id = taskListEl.getAttribute("data-id");
            this.bindControls(taskListEl, id);
        });

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
                alert("Error creating task list");
                return;
            }

            const taskList = await response.json();
            this.addTaskLists(taskList);
            this.input.value = "";
        } catch (err) {
            alert(err.message);
        }
    }

    async deleteTaskLists(id, taskListElement) {
        if (!confirm("Are you sure you want to delete the task list?")) return;

        try {
            const response = await fetch(`${this.apiUrl}/${id}`, {
                method: "DELETE",
                headers: {
                    "X-User-Id": this.userId
                },
            });

            if (!response.ok) {
                alert("Error removing task list");
                return;
            }

            taskListElement.remove();
        } catch (err) {
            alert(err.message);
        }
    }

    async updateTaskList(id, newTitle) {
        const response = await fetch(`${this.apiUrl}/${id}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                "X-User-Id": this.userId
            },
            body: JSON.stringify({title: newTitle})
        });

        if (!response.ok) {
            throw new Error("Error updating task list");
        }

        return await response.json();
    }

    bindControls(taskListEl, id) {
        const deleteBtn = taskListEl.querySelector(".task-lists-delete");
        if (deleteBtn) {
            deleteBtn.addEventListener("click", async (e) => {
                e.stopPropagation();
                await this.deleteTaskLists(id, taskListEl);
            });
        }

        const titleEl = taskListEl.querySelector("h4");
        const controlsContainer = taskListEl.querySelector(".task-lists-controls");
        const editBtn = taskListEl.querySelector(".task-lists-edit");

        if (!editBtn || !titleEl || !controlsContainer) return;

        editBtn.addEventListener("click", () => {
            const currentTitle = titleEl.textContent;

            const input = document.createElement("input");
            input.type = "text";
            input.value = currentTitle;
            input.className = "task-title-input";

            const saveBtn = document.createElement("button");
            saveBtn.textContent = "Save";
            saveBtn.className = "task-title-save";

            const cancelBtn = document.createElement("button");
            cancelBtn.textContent = "Cancel";
            cancelBtn.className = "task-title-cancel";

            titleEl.replaceWith(input);
            saveBtn.style.marginLeft = "8px";
            cancelBtn.style.marginLeft = "4px";

            editBtn.style.display = "none";
            controlsContainer.appendChild(saveBtn);
            controlsContainer.appendChild(cancelBtn);

            cancelBtn.addEventListener("click", () => {
                input.replaceWith(titleEl);
                saveBtn.remove();
                cancelBtn.remove();
                editBtn.style.display = "";
            });

            saveBtn.addEventListener("click", async () => {
                const newTitle = input.value.trim();
                if (!newTitle) return;

                try {
                    const updatedTaskList = await this.updateTaskList(id, newTitle);
                    const newTaskListEl = this.createTaskListElement(updatedTaskList);
                    taskListEl.replaceWith(newTaskListEl);
                } catch (err) {
                    alert(err.message);
                }
            });
        });
    }

    addTaskLists(taskList) {
        const taskListEl = this.createTaskListElement(taskList);
        this.container.appendChild(taskListEl);
    }

    createTaskListElement(taskList) {
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
                <div class="task-lists-edit" id="task-list-edit">
                    <img src="icons/edit.svg" alt="Edit">
                </div>
            </div>
            <h4>${taskList.title}</h4>
            <p>Created At: ${taskList.createdAt}</p>
            ${sharedSection}
        `;

        this.bindControls(taskListElement, taskList.id);
        return taskListElement;
    }
}
