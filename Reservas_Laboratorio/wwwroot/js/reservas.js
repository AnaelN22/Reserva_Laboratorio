document.addEventListener("DOMContentLoaded", () => {
    const tablaBody = document.querySelector("#tablaReservas tbody");
    const modal = document.getElementById("modalReserva");
    const cerrarModal = document.getElementById("cerrarModal");
    const btnNuevaReserva = document.getElementById("btnNuevaReserva");
    const formReserva = document.getElementById("formReserva");
    const labSelect = document.getElementById("labId");

    let editId = null;

    // Cargar reservas
    async function cargarReservas() {
        const res = await fetch("/api/reserva", { headers: { "Authorization": "Bearer " + localStorage.getItem("token") } });
        const data = await res.json();
        tablaBody.innerHTML = "";
        data.forEach(r => {
            tablaBody.innerHTML += `
        <tr>
          <td>${r.id}</td>
          <td>${r.labName}</td>
          <td>${r.usuarioName}</td>
          <td>${r.fecha.split("T")[0]}</td>
          <td>${r.horaInicio}</td>
          <td>${r.horaFin}</td>
          <td>
            <button onclick="editarReserva(${r.id})">Editar</button>
            <button onclick="eliminarReserva(${r.id})">Eliminar</button>
          </td>
        </tr>
      `;
        });
    }

    // Cargar laboratorios
    async function cargarLaboratorios() {
        const res = await fetch("/api/laboratorio");
        const labs = await res.json();
        labSelect.innerHTML = labs.map(l => `<option value="${l.id}">${l.labName}</option>`).join("");
    }

    // Abrir modal
    btnNuevaReserva.addEventListener("click", () => {
        editId = null;
        formReserva.reset();
        modal.classList.remove("hidden");
    });

    cerrarModal.addEventListener("click", () => modal.classList.add("hidden"));

    // Guardar reserva
    formReserva.addEventListener("submit", async e => {
        e.preventDefault();
        const data = {
            labId: labSelect.value,
            fecha: document.getElementById("fecha").value,
            horaInicio: document.getElementById("horaInicio")..value,
            horaFin: document.getElementById("horaFin").value
        };

        const url = editId ? `/api/reserva/${editId}` : "/api/reserva";
        const method = editId ? "PUT" : "POST";

        const res = await fetch(url, {
            method,
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + localStorage.getItem("token")
            },
            body: JSON.stringify(data)
        });

        if (res.ok) {
            alert("Reserva guardada correctamente");
            modal.classList.add("hidden");
            cargarReservas();
        } else {
            const err = await res.text();
            alert("Error: " + err);
        }
    });

    // Funciones globales
    window.editarReserva = async (id) => {
        editId = id;
        const res = await fetch(`/api/reserva/${id}`);
        const r = await res.json();
        labSelect.value = r.labId;
        document.getElementById("fecha").value = r.fecha.split("T")[0];
        document.getElementById("horaInicio").value = r.horaInicio.substring(0, 5);
        document.getElementById("horaFin").value = r.horaFin.substring(0, 5);
        modal.classList.remove("hidden");
    };

    window.eliminarReserva = async (id) => {
        if (confirm("¿Seguro que deseas eliminar esta reserva?")) {
            const res = await fetch(`/api/reserva/${id}`, {
                method: "DELETE",
                headers: { "Authorization": "Bearer " + localStorage.getItem("token") }
            });
            if (res.ok) {
                cargarReservas();
            } else {
                alert("Error al eliminar");
            }
        }
    };

    // Inicialización
    cargarLaboratorios();
    cargarReservas();
});