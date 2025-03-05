// Función que calcula el cupo según los valores ingresados
function calcularCupo() {
    const activoCorriente = parseFloat(document.getElementById("ActivoCorriente").value) || 0;
    const activoFijo = parseFloat(document.getElementById("ActivoFijo").value) || 0;
    const capital = parseFloat(document.getElementById("Capital").value) || 0;
    const reserva = parseFloat(document.getElementById("Reserva").value) || 0;
    const perdida = parseFloat(document.getElementById("Perdida").value) || 0;
    const ventas = parseFloat(document.getElementById("Ventas").value) || 0;
    const utilidad = parseFloat(document.getElementById("Utilidad").value) || 0;

    console.log({ activoCorriente, activoFijo, capital, reserva, perdida, ventas, utilidad });

    const patrimonioNeto = capital + reserva - perdida;
    const baseCupo = (activoCorriente * 0.1) + (activoFijo * 0.05) + (ventas * 0.03) + (utilidad * 0.01) + (patrimonioNeto * 0.2);
    const cupoFinal = baseCupo * 0.10;

    console.log("Cupo Final calculado:", cupoFinal);

    document.getElementById("cupoAsignado").value = cupoFinal.toLocaleString('es-ES', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    document.getElementById("cupoAsignadoHidden").value = cupoFinal.toLocaleString('es-ES', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

// Esperar a que el DOM se cargue y agregar el evento "input" a todos los campos numéricos
document.addEventListener('DOMContentLoaded', () => {
    const inputs = document.querySelectorAll("input[type='number']");
    inputs.forEach(input => {
        input.addEventListener("input", calcularCupo);
    });
});
