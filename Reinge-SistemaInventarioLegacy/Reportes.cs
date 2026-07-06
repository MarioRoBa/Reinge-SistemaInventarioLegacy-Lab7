using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SistemaInventarioLegacy
{
    public class Reportes
    {
        // ============================================================
        // REPORTE DE INVENTARIO
        // ============================================================

        public static void GenerarReporteInventario()
        {
            Utilidades.MostrarEncabezado("REPORTE DE INVENTARIO");

            List<Producto> productos = AccesoDatos.ObtenerProductos();

            Console.WriteLine("  {0,-8} {1,-25} {2,12} {3,12} {4,8} {5,8}",
                "Código", "Nombre", "P.Compra", "P.Venta", "Stock", "Mín.");
            Utilidades.MostrarSeparador();

            decimal valorTotal = 0;
            int totalItems = 0;
            int productosBajoStock = 0;

            foreach (var p in productos)
            {
                // Magic number: color rojo si stock < 5 (debería usar StockMinimo)
                if (p.Stock < 5)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    productosBajoStock++;
                }
                else if (p.Stock <= p.StockMinimo)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    productosBajoStock++;
                }

                Console.WriteLine("  {0,-8} {1,-25} {2,12} {3,12} {4,8} {5,8}",
                    p.Codigo,
                    p.Nombre.Length > 25 ? p.Nombre.Substring(0, 22) + "..." : p.Nombre,
                    Utilidades.FormatearMoneda(p.PrecioCompra),
                    Utilidades.FormatearMoneda(p.PrecioVenta),
                    p.Stock,
                    p.StockMinimo);

                Console.ResetColor();
                valorTotal += p.PrecioVenta * p.Stock;
                totalItems += p.Stock;
            }

            Utilidades.MostrarSeparador();
            Console.WriteLine("  Total productos: " + productos.Count);
            Console.WriteLine("  Total unidades en inventario: " + totalItems);
            Console.WriteLine("  Valor total del inventario: " + Utilidades.FormatearMoneda(valorTotal));
            Console.WriteLine("  Productos bajo stock mínimo: " + productosBajoStock);

            // Magic numbers: márgenes hardcoded
            decimal margenPromedio = 0;
            if (productos.Count > 0)
            {
                decimal sumaMargen = 0;
                foreach (var p in productos)
                {
                    sumaMargen += Utilidades.CalcularMargen(p.PrecioCompra, p.PrecioVenta);
                }
                margenPromedio = sumaMargen / productos.Count;
            }
            Console.WriteLine("  Margen de ganancia promedio: " + margenPromedio.ToString("N1") + "%");

            // Alerta arbitraria con magic number
            if (margenPromedio < 30) // ¿Por qué 30? No hay documentación
            {
                Utilidades.MostrarMensajeAdvertencia("El margen promedio está por debajo del 30%");
            }

            Utilidades.Pausar();
        }

        // ============================================================
        // REPORTE DE PRODUCTOS BAJO STOCK
        // ============================================================

        public static void GenerarReporteProductosBajoStock()
        {
            Utilidades.MostrarEncabezado("PRODUCTOS BAJO STOCK MÍNIMO");

            List<Producto> productos = AccesoDatos.ObtenerProductosBajoStock();

            if (productos.Count == 0)
            {
                Utilidades.MostrarMensajeExito("No hay productos bajo stock mínimo.");
                Utilidades.Pausar();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  ⚠ Se encontraron " + productos.Count + " productos que requieren reposición:");
            Console.ResetColor();
            Console.WriteLine();

            Console.WriteLine("  {0,-8} {1,-25} {2,10} {3,10} {4,12}",
                "Código", "Nombre", "Stock", "Mínimo", "Faltante");
            Utilidades.MostrarSeparador();

            foreach (var p in productos)
            {
                int faltante = p.StockMinimo - p.Stock;
                // Magic number: sugiere ordenar el doble del mínimo
                int sugerido = p.StockMinimo * 2; // ¿Por qué * 2?

                Console.WriteLine("  {0,-8} {1,-25} {2,10} {3,10} {4,12}",
                    p.Codigo,
                    p.Nombre.Length > 25 ? p.Nombre.Substring(0, 22) + "..." : p.Nombre,
                    p.Stock,
                    p.StockMinimo,
                    faltante);
                Console.WriteLine("           → Sugerido ordenar: " + sugerido + " unidades");
            }

            Utilidades.MostrarSeparador();

            // Estimar costo de reposición
            decimal costoReposicion = 0;
            foreach (var p in productos)
            {
                int faltante = p.StockMinimo - p.Stock;
                if (faltante > 0)
                    costoReposicion += p.PrecioCompra * faltante;
            }
            Console.WriteLine("  Costo estimado de reposición mínima: " + Utilidades.FormatearMoneda(costoReposicion));

            Utilidades.Pausar();
        }

        // ============================================================
        // REPORTE DE VENTAS
        // ============================================================

        public static void GenerarReporteVentas()
        {
            Utilidades.MostrarEncabezado("REPORTE DE VENTAS");

            ResumenVentas resumen = AccesoDatos.ObtenerResumenVentas();
            resumen.ImprimirResumen();

            Console.WriteLine();
            Console.WriteLine("  Ventas por mes (año actual):");
            Utilidades.MostrarSeparador();

            int anioActual = DateTime.Now.Year;
            decimal totalAnual = 0;

            // Hardcoded: nombres de meses en lugar de usar CultureInfo
            string[] meses = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                              "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };

            for (int i = 1; i <= 12; i++)
            {
                decimal ventasMes = AccesoDatos.ObtenerVentasPorMes(anioActual, i);
                totalAnual += ventasMes;

                // Magic numbers: umbrales de color sin documentar
                if (ventasMes > 10000) // ¿Por qué 10000?
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (ventasMes > 5000) // ¿Por qué 5000?
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else if (ventasMes > 0)
                    Console.ForegroundColor = ConsoleColor.White;
                else
                    Console.ForegroundColor = ConsoleColor.DarkGray;

                // Barra visual proporcional - magic number: escala de 50 caracteres
                int barras = (int)(ventasMes / 500); // ¿Por qué 500?
                if (barras > 50) barras = 50; // ¿Por qué 50?
                string barra = new string('█', barras);

                Console.WriteLine("  {0,-12} {1,14} {2}", meses[i - 1], Utilidades.FormatearMoneda(ventasMes), barra);
                Console.ResetColor();
            }

            Utilidades.MostrarSeparador();
            Console.WriteLine("  Total anual: " + Utilidades.FormatearMoneda(totalAnual));

            // Magic number: meta de ventas anuales
            decimal metaAnual = 100000; // ¿De dónde sale este número?
            decimal porcentajeMeta = totalAnual / metaAnual * 100;
            Console.WriteLine("  Meta anual: " + Utilidades.FormatearMoneda(metaAnual));
            Console.WriteLine("  Progreso: " + porcentajeMeta.ToString("N1") + "%");

            if (porcentajeMeta >= 100)
                Utilidades.MostrarMensajeExito("¡Meta alcanzada!");
            else if (porcentajeMeta >= 75) // Magic number
                Utilidades.MostrarMensajeAdvertencia("Falta " + (100 - porcentajeMeta).ToString("N1") + "% para alcanzar la meta");
            else
                Utilidades.MostrarMensajeError("Ventas por debajo del 75% de la meta");

            Utilidades.Pausar();
        }

        // ============================================================
        // REPORTE DE CLIENTES
        // ============================================================

        public static void GenerarReporteClientes()
        {
            Utilidades.MostrarEncabezado("REPORTE DE CLIENTES");

            List<Cliente> clientes = AccesoDatos.ObtenerClientes();

            int regulares = 0, mayoristas = 0, vip = 0;

            Console.WriteLine("  {0,-5} {1,-30} {2,-15} {3,-25}",
                "ID", "Nombre", "Tipo", "Email");
            Utilidades.MostrarSeparador();

            foreach (var c in clientes)
            {
                string tipo = Utilidades.ObtenerNombreTipoCliente(c.TipoCliente);

                // Conteo duplicado - podría hacerse con LINQ o SQL
                if (c.TipoCliente == 1) regulares++;
                if (c.TipoCliente == 2) mayoristas++;
                if (c.TipoCliente == 3) vip++;

                Console.WriteLine("  {0,-5} {1,-30} {2,-15} {3,-25}",
                    c.Id,
                    (c.Nombre + " " + c.Apellido).Length > 30
                        ? (c.Nombre + " " + c.Apellido).Substring(0, 27) + "..."
                        : c.Nombre + " " + c.Apellido,
                    tipo,
                    c.Email);
            }

            Utilidades.MostrarSeparador();
            Console.WriteLine("  Total clientes: " + clientes.Count);
            Console.WriteLine("  Regulares: " + regulares);
            Console.WriteLine("  Mayoristas: " + mayoristas);
            Console.WriteLine("  VIP: " + vip);

            Utilidades.Pausar();
        }

        // ============================================================
        // REPORTE DE PEDIDOS POR ESTADO
        // ============================================================

        public static void GenerarReportePedidos()
        {
            Utilidades.MostrarEncabezado("REPORTE DE PEDIDOS");

            Console.WriteLine("  Filtrar por estado:");
            Console.WriteLine("  1. Pendientes");
            Console.WriteLine("  2. En proceso");
            Console.WriteLine("  3. Enviados");
            Console.WriteLine("  4. Entregados");
            Console.WriteLine("  5. Cancelados");
            Console.WriteLine("  0. Todos");

            int filtro = Utilidades.LeerEntero("  Seleccione: ");

            List<Pedido> pedidos;
            if (filtro == 0)
                pedidos = AccesoDatos.ObtenerPedidos();
            else
                pedidos = AccesoDatos.ObtenerPedidosPorEstado(filtro);

            Console.WriteLine();
            Console.WriteLine("  {0,-6} {1,-25} {2,-12} {3,12} {4,-12}",
                "ID", "Cliente", "Fecha", "Total", "Estado");
            Utilidades.MostrarSeparador();

            decimal totalFiltrado = 0;

            foreach (var p in pedidos)
            {
                string estado = Utilidades.ObtenerNombreEstadoPedido(p.Estado);

                // Colores hardcoded por estado - magic numbers
                if (p.Estado == 1) Console.ForegroundColor = ConsoleColor.Yellow;
                else if (p.Estado == 2) Console.ForegroundColor = ConsoleColor.Cyan;
                else if (p.Estado == 3) Console.ForegroundColor = ConsoleColor.Blue;
                else if (p.Estado == 4) Console.ForegroundColor = ConsoleColor.Green;
                else if (p.Estado == 5) Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("  {0,-6} {1,-25} {2,-12} {3,12} {4,-12}",
                    p.Id,
                    p.NombreCliente.Length > 25 ? p.NombreCliente.Substring(0, 22) + "..." : p.NombreCliente,
                    Utilidades.FormatearFechaCorta(p.FechaPedido),
                    Utilidades.FormatearMoneda(p.Total),
                    estado);

                Console.ResetColor();
                totalFiltrado += p.Total;
            }

            Utilidades.MostrarSeparador();
            Console.WriteLine("  Pedidos mostrados: " + pedidos.Count);
            Console.WriteLine("  Monto total: " + Utilidades.FormatearMoneda(totalFiltrado));

            Utilidades.Pausar();
        }

        // ============================================================
        // EXPORTAR REPORTE A ARCHIVO - Responsabilidad mezclada
        // ============================================================

        public static void ExportarReporteInventarioTexto()
        {
            List<Producto> productos = AccesoDatos.ObtenerProductos();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine("  REPORTE DE INVENTARIO");
            sb.AppendLine("  Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            sb.AppendLine("  Empresa: " + Configuracion.NombreEmpresa);
            sb.AppendLine("========================================");
            sb.AppendLine();

            sb.AppendLine(string.Format("{0,-8} {1,-25} {2,12} {3,12} {4,8}",
                "Código", "Nombre", "P.Compra", "P.Venta", "Stock"));
            sb.AppendLine("────────────────────────────────────────────────────────────────────");

            decimal valorTotal = 0;
            foreach (var p in productos)
            {
                sb.AppendLine(string.Format("{0,-8} {1,-25} {2,12} {3,12} {4,8}",
                    p.Codigo, p.Nombre, 
                    Utilidades.FormatearMoneda(p.PrecioCompra),
                    Utilidades.FormatearMoneda(p.PrecioVenta),
                    p.Stock));
                valorTotal += p.PrecioVenta * p.Stock;
            }

            sb.AppendLine();
            sb.AppendLine("Total productos: " + productos.Count);
            sb.AppendLine("Valor total del inventario: " + Utilidades.FormatearMoneda(valorTotal));

            // Ruta hardcoded que probablemente no existe
            string ruta = Configuracion.RutaReportes + "inventario_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            try
            {
                // No crea el directorio si no existe
                File.WriteAllText(ruta, sb.ToString());
                Utilidades.MostrarMensajeExito("Reporte exportado a: " + ruta);
            }
            catch (Exception ex)
            {
                Utilidades.MostrarMensajeError("Error al exportar: " + ex.Message);
            }

            Utilidades.Pausar();
        }

        // ============================================================
        // REPORTE DE MOVIMIENTOS DE UN PRODUCTO
        // ============================================================

        public static void GenerarReporteMovimientos()
        {
            Utilidades.MostrarEncabezado("MOVIMIENTOS DE INVENTARIO");

            int productoId = Utilidades.LeerEntero("  ID del producto: ");
            Producto producto = AccesoDatos.ObtenerProductoPorId(productoId);

            if (producto == null)
            {
                Utilidades.MostrarMensajeError("Producto no encontrado.");
                Utilidades.Pausar();
                return;
            }

            Console.WriteLine("  Producto: " + producto.Nombre + " (" + producto.Codigo + ")");
            Console.WriteLine("  Stock actual: " + producto.Stock);
            Console.WriteLine();

            List<MovimientoInventario> movimientos = AccesoDatos.ObtenerMovimientos(productoId);

            Console.WriteLine("  {0,-6} {1,-12} {2,-10} {3,10} {4,-30}",
                "ID", "Tipo", "Fecha", "Cantidad", "Motivo");
            Utilidades.MostrarSeparador();

            foreach (var m in movimientos)
            {
                string tipo = Utilidades.ObtenerNombreTipoMovimiento(m.TipoMovimiento);

                if (m.TipoMovimiento == 1) Console.ForegroundColor = ConsoleColor.Green;
                else if (m.TipoMovimiento == 2) Console.ForegroundColor = ConsoleColor.Red;
                else Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine("  {0,-6} {1,-12} {2,-10} {3,10} {4,-30}",
                    m.Id, tipo,
                    Utilidades.FormatearFechaCorta(m.Fecha),
                    (m.TipoMovimiento == 1 ? "+" : "-") + m.Cantidad,
                    m.Motivo.Length > 30 ? m.Motivo.Substring(0, 27) + "..." : m.Motivo);

                Console.ResetColor();
            }

            Utilidades.MostrarSeparador();
            Console.WriteLine("  Total movimientos: " + movimientos.Count);

            Utilidades.Pausar();
        }
    }
}
