const fecha = document.querySelector("#Fecha");
const horario = document.querySelector("#HorarioId");

if (fecha && horario) {
    fecha.addEventListener("change", async () => {
        const valorActual = horario.value;
        const citaId = horario.dataset.citaId || 0;

        const respuesta = await fetch(`/Citas/HorariosDisponibles?fecha=${fecha.value}&citaId=${citaId}`);
        const datos = await respuesta.json();

        horario.innerHTML =
            '<option value="0">Selecciona un horario</option>' +
            datos.map(h => `<option value="${h.id}">${h.hora}</option>`).join("");

        if ([...horario.options].some(option => option.value === valorActual)) {
            horario.value = valorActual;
        }
    });

    fecha.dispatchEvent(new Event("change"));
}