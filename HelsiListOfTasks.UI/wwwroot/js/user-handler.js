export class UserHandler {
    constructor({formId, inputId, apiUrl, containerSelector}) {
        this.form = document.getElementById(formId);
        this.input = document.getElementById(inputId);
        this.apiUrl = apiUrl;
        this.container = document.querySelector(containerSelector);
    }

    init() {
        if (!this.form || !this.input || !this.container) {
            console.warn("UserFormHandler: No form or container found");
            return;
        }

        this.container.querySelectorAll(".user-card").forEach(card => {
            const id = card.getAttribute("data-id");
            const deleteBtn = card.querySelector(".user-card-delete");
            if (id && deleteBtn) {
                deleteBtn.addEventListener("click", async (e) => {
                    e.stopPropagation();
                    await this.deleteUser(id, card);
                });
            }
            if (id) {
                card.addEventListener("click", () => {
                    window.location.href = `/task-lists?userId=${id}`;
                });
            }
        });

        this.form.addEventListener("submit", this.handleSubmit.bind(this));
    }

    async handleSubmit(event) {
        event.preventDefault();

        const name = this.input.value.trim();
        if (!name) return;

        try {
            const response = await fetch(this.apiUrl, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({name})
            });

            if (!response.ok) {
                alert("Error creating user");
            }

            const user = await response.json();
            this.addUserCard(user);
            this.input.value = "";
        } catch (err) {
            alert(err.message);
        }
    }

    async deleteUser(id, cardElement) {
        if (!confirm("Are you sure you want to delete the user?")) return;

        try {
            const response = await fetch(`${this.apiUrl}/${id}`, {
                method: "DELETE"
            });

            if (!response.ok) alert("Error removing user");

            cardElement.remove();
        } catch (err) {
            alert(err.message);
        }
    }

    addUserCard(user) {
        const card = document.createElement("div");
        card.className = "user-card";
        card.setAttribute("data-id", user.id);
        card.innerHTML = `
            <div class="user-card-delete">
                <img src="icons/delete.svg" alt="Delete">
            </div>
            <h4>${user.name}</h4>
            <p>ID: ${user.id}</p>
        `;

        const deleteBtn = card.querySelector(".user-card-delete");
        deleteBtn.addEventListener("click", async () => {
            await this.deleteUser(user.id, card);
        });

        card.addEventListener("click", () => {
            window.location.href = `/task-lists?userId=${user.id}`;
        });

        this.container.appendChild(card);
    }
}
