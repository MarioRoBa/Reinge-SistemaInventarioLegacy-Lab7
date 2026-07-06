using System;
using System.Collections.Generic;

namespace SistemaInventarioLegacy
{
    public class Producto
    {
        public int Id;
        public string Codigo;
        public string Nombre;
        public string Descripcion;
        public decimal PrecioCompra;
        public decimal PrecioVenta;
        public int Stock;
        public int StockMinimo;
        public int CategoriaId;
        public bool Activo;
        public DateTime FechaCreacion;
        public DateTime UltimaModificacion;
    }

    public class Cliente
    {
        public int Id;
        public string Nombre;
        public string Apellido;
        public string Email;
        public string Telefono;
        public string Direccion;
        public int TipoCliente; // 1=Regular, 2=Mayorista, 3=VIP
        public bool Activo;
        public DateTime FechaRegistro;

        // Método que no pertenece aquí (debería estar en una capa de presentación)
        public void MostrarInfo()
        {
            Console.WriteLine("  ID: " + Id);
            Console.WriteLine("  Nombre: " + Nombre + " " + Apellido);
            Console.WriteLine("  Email: " + Email);
            Console.WriteLine("  Teléfono: " + Telefono);
            Console.WriteLine("  Dirección: " + Direccion);
            string tipo = "";
            if (TipoCliente == 1) tipo = "Regular";
            if (TipoCliente == 2) tipo = "Mayorista";
            if (TipoCliente == 3) tipo = "VIP";
            Console.WriteLine("  Tipo: " + tipo);
        }
    }

    public class Pedido
    {
        public int Id;
        public int ClienteId;
        public DateTime FechaPedido;
        public int Estado; // 1=Pendiente, 2=Procesando, 3=Enviado, 4=Entregado, 5=Cancelado
        public decimal SubTotal;
        public decimal Impuesto;
        public decimal Total;
        public string Direccion;
        public string Notas;
        public DateTime? FechaEnvio;
        public DateTime? FechaEntrega;
        
        // Datos del cliente denormalizados (acoplamiento innecesario)
        public string NombreCliente;
        public string EmailCliente;
    }

    public class DetallePedido
    {
        public int Id;
        public int PedidoId;
        public int ProductoId;
        public int Cantidad;
        public decimal PrecioUnitario;
        public decimal Descuento;
        public decimal Subtotal;
        
        // Datos del producto denormalizados
        public string NombreProducto;
    }

    public class Categoria
    {
        public int Id;
        public string Nombre;
        public string Descripcion;
        public bool Activa;
        public DateTime FechaCreacion;
    }

    public class Proveedor
    {
        public int Id;
        public string Nombre;
        public string Contacto;
        public string Email;
        public string Telefono;
        public string Direccion;
        public bool Activo;
    }

    public class MovimientoInventario
    {
        public int Id;
        public int ProductoId;
        public int TipoMovimiento; // 1=Entrada, 2=Salida, 3=Ajuste
        public int Cantidad;
        public string Motivo;
        public int UsuarioId;
        public DateTime Fecha;
        
        public string NombreProducto;
    }

    public class Usuario
    {
        public int Id;
        public string NombreUsuario;
        public string Contrasena; // Texto plano - problema de seguridad
        public string NombreCompleto;
        public int Rol; // 1=Operador, 2=Supervisor, 3=Admin
        public bool Activo;
        public DateTime? UltimoAcceso;
    }

    // Clase que mezcla datos con lógica de formato - anti-patrón
    public class ResumenVentas
    {
        public int TotalPedidos;
        public decimal MontoTotal;
        public decimal PromedioVenta;
        public int PedidosPendientes;
        public int PedidosCompletados;
        public int PedidosCancelados;

        public void ImprimirResumen()
        {
            Console.WriteLine("╔══════════════════════════════════╗");
            Console.WriteLine("║     RESUMEN DE VENTAS            ║");
            Console.WriteLine("╠══════════════════════════════════╣");
            Console.WriteLine("║ Total pedidos:     " + TotalPedidos.ToString().PadLeft(10) + "   ║");
            Console.WriteLine("║ Monto total:    $" + MontoTotal.ToString("N2").PadLeft(12) + "   ║");
            Console.WriteLine("║ Promedio venta: $" + PromedioVenta.ToString("N2").PadLeft(12) + "   ║");
            Console.WriteLine("║ Pendientes:        " + PedidosPendientes.ToString().PadLeft(10) + "   ║");
            Console.WriteLine("║ Completados:       " + PedidosCompletados.ToString().PadLeft(10) + "   ║");
            Console.WriteLine("║ Cancelados:        " + PedidosCancelados.ToString().PadLeft(10) + "   ║");
            Console.WriteLine("╚══════════════════════════════════╝");
        }
    }
}
