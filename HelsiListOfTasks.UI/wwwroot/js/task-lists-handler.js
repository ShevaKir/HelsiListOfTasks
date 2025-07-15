export class TaskListsHandler {
    constructor({
                    formElement,
                    inputElement,
                    apiUrl,
                    containerElement,
                    prevPageButton,
                    nextPageButton,
                    pageNumberElement,
                    pageSize = 3
                }) {
        this.form = formElement;
        this.input = inputElement;
        this.apiUrl = apiUrl;
        this.container = containerElement;
        this.prevBtn = prevPageButton;
        this.nextBtn = nextPageButton;
        this.pageNumber = pageNumberElement;
        this.userId = new URLSearchParams(window.location.search).get("userId");
        this.pageSize = pageSize;
        this.currentPage = 1;
    }

    init() {
        if (!this.form || !this.input || !this.container || !this.userId) {
            console.warn("TaskListsHandler: Missing required elements or userId");
            return;
        }

        this.prevBtn?.addEventListener("click", async () => {
            if (this.currentPage > 1) {
                this.currentPage--;
                await this.renderPage();
            }
        });

        this.nextBtn?.addEventListener("click", async () => {
            this.currentPage++;
            await this.renderPage();
        });

        this.renderPage();

        this.form.addEventListener("submit", this.handleSubmit.bind(this));
    }

    async renderPage() {
        const offset = (this.currentPage - 1) * this.pageSize;

        try {
            const response = await fetch(`${this.apiUrl}?offset=${offset}&limit=${this.pageSize}`, {
                headers: {
                    "X-User-Id": this.userId
                }
            });

            if (!response.ok) {
                alert("Error loading task lists");
                return;
            }

            const taskLists = await response.json();
            this.container.innerHTML = "";

            taskLists.forEach(t => {
                const el = this.createTaskListElement(t);
                this.container.appendChild(el);
            });

            this.pageNumber.textContent = `Page ${this.currentPage}`;
            this.prevBtn.disabled = this.currentPage === 1;
            this.nextBtn.disabled = taskLists.length < this.pageSize;

        } catch (err) {
            alert(err.message);
        }
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

            this.input.value = "";
            await this.renderPage();

        } catch (err) {
            alert(err.message);
        }
    }

    async deleteTaskList(id) {
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

            await this.renderPage();

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
            alert("Error updating task list");
            return;
        }

        await this.renderPage();
    }

    async handleShare(taskListId) {
        const userSelect = document.getElementById("user-select");
        if (!userSelect) {
            alert("User selector not found");
            return;
        }

        userSelect.hidden = false;

        const onChange = async () => {
            const targetUserId = userSelect.value;
            if (!targetUserId) return;

            try {
                const response = await fetch(`${this.apiUrl}/${taskListId}/sharing/${targetUserId}`, {
                    method: "POST",
                    headers: {
                        "X-User-Id": this.userId
                    }
                });

                if (!response.ok) {
                    alert("Error sharing task list");
                    return;
                }

                await this.renderPage();
            } catch (err) {
                alert(err.message);
            } finally {
                userSelect.hidden = true;
                userSelect.value = "";
                userSelect.removeEventListener("change", onChange);
            }
        };

        userSelect.removeEventListener("change", onChange);
        userSelect.addEventListener("change", onChange);
    }


    createTaskListElement(taskList) {
        const el = document.createElement("div");
        el.className = "task-lists";
        el.dataset.id = taskList.id;
        el.dataset.ownerId = taskList.ownerId;

        const isCollaborative = taskList.ownerId !== this.userId;
        if (isCollaborative) el.classList.add("task-lists-collaboration");

        const sharedSection = (taskList.sharedWithUserIds?.length ?? 0) > 0
            ? `<p>Sharing Users:</p><ul>${taskList.sharedWithUserIds.map(id =>
                `<li>                            
                    <div class="task-lists-sharing">
                       <p>${id}</p>
                        <div class="task-lists-delete-sharing">
                            <img src="icons/delete.svg" alt="Delete">
                        </div>
                    </div>
                </li>`).join("")}</ul>`
            : `<p>The task list is only available to you.</p>`;

        const controlsHtml = isCollaborative ? "" : `
        <div class="task-lists-share">
            <img src="icons/share.svg" alt="Share">
        </div>
        <div class="task-lists-edit">
            <img src="icons/edit.svg" alt="Edit">
        </div>
        <div class="task-lists-delete">
            <img src="icons/delete.svg" alt="Delete">
        </div>
    `;

        const collaborationNote = isCollaborative
            ? `<p><strong>Collaborative (owned by ${taskList.ownerId})</strong></p>`
            : "";

        el.innerHTML = `
        <div class="task-lists-controls">
            ${controlsHtml}
        </div>
        <h4>${taskList.title}</h4>
        <p>Created At: ${taskList.createdAt}</p>
        ${collaborationNote}
        ${sharedSection}
    `;

        if (!isCollaborative) this.bindControls(el, taskList.id);
        return el;
    }


    bindControls(el, id) {
        const deleteBtn = el.querySelector(".task-lists-delete");
        const editBtn = el.querySelector(".task-lists-edit");
        const titleEl = el.querySelector("h4");
        const controlsContainer = el.querySelector(".task-lists-controls");
        const shareBtn = el.querySelector(".task-lists-share");

        deleteBtn?.addEventListener("click", async () => await this.deleteTaskList(id));

        editBtn?.addEventListener("click", () => {
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
            controlsContainer.appendChild(saveBtn);
            controlsContainer.appendChild(cancelBtn);
            editBtn.style.display = "none";

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
                    await this.updateTaskList(id, newTitle);
                } catch (err) {
                    alert(err.message);
                }
            });
        });

        shareBtn?.addEventListener("click", async () => {
            await this.handleShare(id);
        });

        el.querySelectorAll(".task-lists-delete-sharing").forEach(button => {
            button.addEventListener("click", async () => {
                debugger
                const userDiv = button.closest(".task-lists-sharing");
                const targetUserId = userDiv?.querySelector("p")?.textContent?.trim();
                if (!targetUserId) return;

                try {
                    const response = await fetch(`${this.apiUrl}/${id}/sharing/${targetUserId}`, {
                        method: "DELETE",
                        headers: {
                            "X-User-Id": this.userId
                        }
                    });

                    if (!response.ok) {
                        alert("Error removing share");
                        return;
                    }

                    await this.renderPage();
                } catch (err) {
                    alert(err.message);
                }
            });
        });
    }
}
