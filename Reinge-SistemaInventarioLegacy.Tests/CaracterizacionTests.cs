using SistemaInventarioLegacy;
using Xunit;

namespace Reinge_SistemaInventarioLegacy.Tests
{
    public class CaracterizacionTests
    {
        [Fact]
        public void CalcularDescuento_ClienteMayorista_DevuelveComportamientoActual()
        {
            decimal resultado = Utilidades.CalcularDescuento(1000m, 2);

            Assert.Equal(100m, resultado);
        }

        [Fact]
        public void CalcularDescuento_ClienteVIP_DevuelveComportamientoActual()
        {
            decimal resultado = Utilidades.CalcularDescuento(1000m, 3);

            Assert.Equal(150m, resultado);
        }

        [Fact]
        public void CalcularDescuento_TipoClienteDesconocido_DevuelveCero()
        {
            decimal resultado = Utilidades.CalcularDescuento(1000m, 99);

            Assert.Equal(0m, resultado);
        }

        [Fact]
        public void CalcularImpuesto_SubtotalMil_DevuelveComportamientoActual()
        {
            decimal resultado = Utilidades.CalcularImpuesto(1000m);

            Assert.Equal(130m, resultado);
        }

        [Fact]
        public void CalcularTotal_SubtotalDescuentoImpuesto_DevuelveComportamientoActual()
        {
            decimal resultado = Utilidades.CalcularTotal(1000m, 100m, 130m);

            Assert.Equal(1030m, resultado);
        }

        [Fact]
        public void CalcularMargen_CompraCincuentaVentaSetentaYCinco_DevuelveCincuentaPorCiento()
        {
            decimal resultado = Utilidades.CalcularMargen(50m, 75m);

            Assert.Equal(50m, resultado);
        }

        [Fact]
        public void ValidarEmail_EmailConArrobaYPunto_DevuelveTrue()
        {
            bool resultado = Utilidades.ValidarEmail("cliente@correo.com");

            Assert.True(resultado);
        }

        [Fact]
        public void ObtenerNombreEstadoPedido_EstadoDesconocido_DevuelveDesconocido()
        {
            string resultado = Utilidades.ObtenerNombreEstadoPedido(99);

            Assert.Equal("Desconocido", resultado);
        }
    }
}