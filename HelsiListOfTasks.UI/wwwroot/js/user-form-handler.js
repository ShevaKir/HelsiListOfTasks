export class UserFormHandler {
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

    addUserCard(user) {
        const card = document.createElement("div");
        card.className = "user-card";
        card.innerHTML = `
            <h4>${user.name}</h4>
            <p>ID: ${user.id}</p>
        `;
        this.container.appendChild(card);
    }
}
