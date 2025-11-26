document.addEventListener("DOMContentLoaded", () => {

    const tablaBody = document.querySelector("#tablaReservas tbody");
    const modal = document.getElementById("modalReserva");
    const cerrarModal = document.getElementById("cerrarModal");
    const btnNuevaReserva = document.getElementById("btnNuevaReserva");
    const formReserva = document.getElementById("formReserva");
    const labSelect = document.getElementById("labId");

    let editId = null;

    /* ==========================================================
        FUNCIÓN GENERAL PARA FETCH CON COOKIES + MANEJO DE 401
    ========================================================== */
    async function apiFetch(url, options = {}) {
        const config = {
            credentials: "include",
            headers: { "Content-Type": "application/json" },
            ...options
        };

        const res = await fetch(url, config);

        if (res.status === 401) {
            Swal.fire({
                icon: "warning",
                title: "Sesión expirada",
                text: "Por favor inicia sesión nuevamente."
            }).then(() => window.location.href = "/Auth/Login");

            return null;
        }

        return res;
    }

    /* ==========================================================
        CARGAR RESERVAS
    ========================================================== */
    async function cargarReservas() {
        const res = await apiFetch("/api/reserva");

        if (!res) return;

        const data = await res.json();
        tablaBody.innerHTML = "";

        data.forEach(r => {
            const fila = document.createElement("tr");

            fila.innerHTML = `
                <td>${r.id}</td>
                <td>${r.labName}</td>
                <td>${r.usuarioName}</td>
                <td>${r.fecha.split("T")[0]}</td>
                <td>${r.horaInicio.substring(0, 5)}</td>
                <td>${r.horaFin.substring(0, 5)}</td>
                <td>
                    <button class="btnEdit" data-id="${r.id}">Editar</button>
                    <button class="btnDelete" data-id="${r.id}">Eliminar</button>
                </td>
            `;

            tablaBody.appendChild(fila);
        });

        asignarEventosTabla();
    }

    /* ==========================================================
        CARGAR LABORATORIOS
    ========================================================== */
    async function cargarLaboratorios() {
        const res = await apiFetch("/api/laboratorio");

        if (!res) return;

        const labs = await res.json();

        labSelect.innerHTML = labs
            .map(l => `<option value="${l.id}">${l.labName}</option>`)
            .join("");
    }

    /* ==========================================================
        ABRIR MODAL NUEVA RESERVA
    ========================================================== */
    btnNuevaReserva.addEventListener("click", () => {
        editId = null;
        formReserva.reset();

        document.getElementById("modalTitulo").textContent = "Nueva Reserva";
        modal.classList.remove("hidden");
    });

    cerrarModal.addEventListener("click", () => modal.classList.add("hidden"));

    /* ==========================================================
        GUARDAR RESERVA (crear o editar)
    ========================================================== */
    formReserva.addEventListener("submit", async e => {
        e.preventDefault();

        const data = {
            labId: Number(labSelect.value),
            fecha: document.getElementById("fecha").value,
            horaInicio: document.getElementById("horaInicio").value,
            horaFin: document.getElementById("horaFin").value
        };

        const url = editId ? `/api/reserva/${editId}` : "/api/reserva";
        const method = editId ? "PUT" : "POST";

        const res = await apiFetch(url, {
            method,
            body: JSON.stringify(data)
        });

        if (!res) return;

        if (res.ok) {
            Swal.fire({
                icon: "success",
                title: "Reserva guardada correctamente"
            });

            modal.classList.add("hidden");
            cargarReservas();
        } else {
            const err = await res.text();
            Swal.fire({
                icon: "error",
                title: "Error",
                text: err
            });
        }
    });

    /* ==========================================================
        ASIGNAR EVENTOS A BOTONES DE TABLA
    ========================================================== */
    function asignarEventosTabla() {
        document.querySelectorAll(".btnEdit").forEach(btn => {
            btn.addEventListener("click", () => editarReserva(btn.dataset.id));
        });

        document.querySelectorAll(".btnDelete").forEach(btn => {
            btn.addEventListener("click", () => eliminarReserva(btn.dataset.id));
        });
    }

    /* ==========================================================
        EDITAR RESERVA
    ========================================================== */
    async function editarReserva(id) {
        editId = id;

        const res = await apiFetch(`/api/reserva/${id}`);
        if (!res) return;

        const r = await res.json();

        labSelect.value = r.labId;
        document.getElementById("fecha").value = r.fecha.split("T")[0];
        document.getElementById("horaInicio").value = r.horaInicio.substring(0, 5);
        document.getElementById("horaFin").value = r.horaFin.substring(0, 5);

        document.getElementById("modalTitulo").textContent = "Editar Reserva";
        modal.classList.remove("hidden");
    }

    /* ==========================================================
        ELIMINAR RESERVA
    ========================================================== */
    async function eliminarReserva(id) {
        const confirmar = await Swal.fire({
            icon: "warning",
            title: "¿Eliminar reserva?",
            text: "Esta acción no se puede deshacer.",
            showCancelButton: true,
            confirmButtonText: "Sí, eliminar",
            cancelButtonText: "Cancelar"
        });

        if (!confirmar.isConfirmed) return;

        const res = await apiFetch(`/api/reserva/${id}`, { method: "DELETE" });

        if (!res) return;

        if (res.ok) {
            Swal.fire({
                icon: "success",
                title: "Reserva eliminada"
            });

            cargarReservas();
        } else {
            Swal.fire({
                icon: "error",
                title: "Error al eliminar"
            });
        }
    }

    /* ==========================================================
       INICIALIZACIÓN
    ========================================================== */
    cargarLaboratorios();
    cargarReservas();
});
