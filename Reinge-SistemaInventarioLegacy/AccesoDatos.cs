using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SistemaInventarioLegacy
{
    public class AccesoDatos
    {
        // ============================================================
        // PRODUCTOS
        // ============================================================

        public static List<Producto> ObtenerProductos()
        {
            List<Producto> productos = new List<Producto>();
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Productos WHERE Activo = 1 ORDER BY Nombre", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Producto p = new Producto();
                p.Id = (int)reader["Id"];
                p.Codigo = reader["Codigo"].ToString();
                p.Nombre = reader["Nombre"].ToString();
                p.Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : "";
                p.PrecioCompra = (decimal)reader["PrecioCompra"];
                p.PrecioVenta = (decimal)reader["PrecioVenta"];
                p.Stock = (int)reader["Stock"];
                p.StockMinimo = (int)reader["StockMinimo"];
                p.CategoriaId = reader["CategoriaId"] != DBNull.Value ? (int)reader["CategoriaId"] : 0;
                p.Activo = (bool)reader["Activo"];
                p.FechaCreacion = (DateTime)reader["FechaCreacion"];
                p.UltimaModificacion = (DateTime)reader["UltimaModificacion"];
                productos.Add(p);
            }
            reader.Close();
            conn.Close();
            return productos;
        }

        // SQL INJECTION: concatenación directa de parámetros del usuario
        public static List<Producto> BuscarProductos(string termino)
        {
            List<Producto> productos = new List<Producto>();
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            string sql = "SELECT * FROM Productos WHERE Activo = 1 AND (Nombre LIKE '%" + termino + "%' OR Codigo LIKE '%" + termino + "%')";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Producto p = new Producto();
                p.Id = (int)reader["Id"];
                p.Codigo = reader["Codigo"].ToString();
                p.Nombre = reader["Nombre"].ToString();
                p.Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : "";
                p.PrecioCompra = (decimal)reader["PrecioCompra"];
                p.PrecioVenta = (decimal)reader["PrecioVenta"];
                p.Stock = (int)reader["Stock"];
                p.StockMinimo = (int)reader["StockMinimo"];
                p.CategoriaId = reader["CategoriaId"] != DBNull.Value ? (int)reader["CategoriaId"] : 0;
                p.Activo = (bool)reader["Activo"];
                p.FechaCreacion = (DateTime)reader["FechaCreacion"];
                p.UltimaModificacion = (DateTime)reader["UltimaModificacion"];
                productos.Add(p);
            }
            reader.Close();
            conn.Close();
            return productos;
        }

        public static Producto ObtenerProductoPorId(int id)
        {
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            // SQL Injection: aunque id es int aquí, el patrón es peligroso
            SqlCommand cmd = new SqlCommand("SELECT * FROM Productos WHERE Id = " + id, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            Producto p = null;
            if (reader.Read())
            {
                p = new Producto();
                p.Id = (int)reader["Id"];
                p.Codigo = reader["Codigo"].ToString();
                p.Nombre = reader["Nombre"].ToString();
                p.Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : "";
                p.PrecioCompra = (decimal)reader["PrecioCompra"];
                p.PrecioVenta = (decimal)reader["PrecioVenta"];
                p.Stock = (int)reader["Stock"];
                p.StockMinimo = (int)reader["StockMinimo"];
                p.CategoriaId = reader["CategoriaId"] != DBNull.Value ? (int)reader["CategoriaId"] : 0;
                p.Activo = (bool)reader["Activo"];
                p.FechaCreacion = (DateTime)reader["FechaCreacion"];
                p.UltimaModificacion = (DateTime)reader["UltimaModificacion"];
            }
            reader.Close();
            conn.Close();
            return p;
        }

        // Código duplicado: mismo mapeo de reader a Producto repetido muchas veces
        public static List<Producto> ObtenerProductosPorCategoria(int categoriaId)
        {
            List<Producto> productos = new List<Producto>();
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Productos WHERE Activo = 1 AND CategoriaId = " + categoriaId, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Producto p = new Producto();
                p.Id = (int)reader["Id"];
                p.Codigo = reader["Codigo"].ToString();
                p.Nombre = reader["Nombre"].ToString();
                p.Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : "";
                p.PrecioCompra = (decimal)reader["PrecioCompra"];
                p.PrecioVenta = (decimal)reader["PrecioVenta"];
                p.Stock = (int)reader["Stock"];
                p.StockMinimo = (int)reader["StockMinimo"];
                p.CategoriaId = reader["CategoriaId"] != DBNull.Value ? (int)reader["CategoriaId"] : 0;
                p.Activo = (bool)reader["Activo"];
                p.FechaCreacion = (DateTime)reader["FechaCreacion"];
                p.UltimaModificacion = (DateTime)reader["UltimaModificacion"];
                productos.Add(p);
            }
            reader.Close();
            conn.Close();
            return productos;
        }

        // SQL Injection en INSERT
        public static int InsertarProducto(string codigo, string nombre, string descripcion,
            decimal precioCompra, decimal precioVenta, int stock, int stockMinimo, int categoriaId)
        {
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            string sql = "INSERT INTO Productos (Codigo, Nombre, Descripcion, PrecioCompra, PrecioVenta, Stock, StockMinimo, CategoriaId) " +
                         "VALUES ('" + codigo + "', '" + nombre + "', '" + descripcion + "', " +
                         precioCompra + ", " + precioVenta + ", " + stock + ", " + stockMinimo + ", " + categoriaId + "); SELECT SCOPE_IDENTITY();";
            SqlCommand cmd = new SqlCommand(sql, conn);
            int id = Convert.ToInt32(cmd.ExecuteScalar());
            conn.Close();
            return id;
        }

        // SQL Injection en UPDATE
        public static void ActualizarProducto(int id, string nombre, string descripcion,
            decimal precioCompra, decimal precioVenta, int stockMinimo, int categoriaId)
        {
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            string sql = "UPDATE Productos SET Nombre = '" + nombre + "', Descripcion = '" + descripcion + "', " +
                         "PrecioCompra = " + precioCompra + ", PrecioVenta = " + precioVenta + ", " +
                         "StockMinimo = " + stockMinimo + ", CategoriaId = " + categoriaId + ", " +
                         "UltimaModificacion = GETDATE() WHERE Id = " + id;
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public static void ActualizarStock(int productoId, int nuevoStock)
        {
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            string sql = "UPDATE Productos SET Stock = " + nuevoStock + ", UltimaModificacion = GETDATE() WHERE Id = " + productoId;
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public static void EliminarProducto(int id)
        {
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            // Eliminación lógica
            string sql = "UPDATE Productos SET Activo = 0 WHERE Id = " + id;
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public static List<Producto> ObtenerProductosBajoStock()
        {
            List<Producto> productos = new List<Producto>();
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Productos WHERE Activo = 1 AND Stock <= StockMinimo", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Producto p = new Producto();
                p.Id = (int)reader["Id"];
                p.Codigo = reader["Codigo"].ToString();
                p.Nombre = reader["Nombre"].ToString();
                p.PrecioCompra = (decimal)reader["PrecioCompra"];
                p.PrecioVenta = (decimal)reader["PrecioVenta"];
                p.Stock = (int)reader["Stock"];
                p.StockMinimo = (int)reader["StockMinimo"];
                p.Activo = (bool)reader["Activo"];
                productos.Add(p);
            }
            reader.Close();
            conn.Close();
            return productos;
        }

        // ============================================================
        // CLIENTES
        // ============================================================

        public static List<Cliente> ObtenerClientes()
        {
            List<Cliente> clientes = new List<Cliente>();
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Clientes WHERE Activo = 1 ORDER BY Apellido, Nombre", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Cliente c = new Cliente();
                c.Id = (int)reader["Id"];
                c.Nombre = reader["Nombre"].ToString();
                c.Apellido = reader["Apellido"].ToString();
                c.Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "";
                c.Telefono = reader["Telefono"] != DBNull.Value ? reader["Telefono"].ToString() : "";
                c.Direccion = reader["Direccion"] != DBNull.Value ? reader["Direccion"].ToString() : "";
                c.TipoCliente = (int)reader["TipoCliente"];
                c.Activo = (bool)reader["Activo"];
                c.FechaRegistro = (DateTime)reader["FechaRegistro"];
                clientes.Add(c);
            }
            reader.Close();
            conn.Close();
            return clientes;
        }

        // SQL Injection en búsqueda de clientes
        public static List<Cliente> BuscarClientes(string termino)
        {
            List<Cliente> clientes = new List<Cliente>();
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            string sql = "SELECT * FROM Clientes WHERE Activo = 1 AND (Nombre LIKE '%" + termino + "%' OR Apellido LIKE '%" + termino + "%' OR Email LIKE '%" + termino + "%')";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Cliente c = new Cliente();
                c.Id = (int)reader["Id"];
                c.Nombre = reader["Nombre"].ToString();
                c.Apellido = reader["Apellido"].ToString();
                c.Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "";
                c.Telefono = reader["Telefono"] != DBNull.Value ? reader["Telefono"].ToString() : "";
                c.Direccion = reader["Direccion"] != DBNull.Value ? reader["Direccion"].ToString() : "";
                c.TipoCliente = (int)reader["TipoCliente"];
                c.Activo = (bool)reader["Activo"];
                c.FechaRegistro = (DateTime)reader["FechaRegistro"];
                clientes.Add(c);
            }
            reader.Close();
            conn.Close();
            return clientes;
        }

        public static Cliente ObtenerClientePorId(int id)
        {
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Clientes WHERE Id = " + id, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            Cliente c = null;
            if (reader.Read())
            {
                c = new Cliente();
                c.Id = (int)reader["Id"];
                c.Nombre = reader["Nombre"].ToString();
                c.Apellido = reader["Apellido"].ToString();
                c.Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "";
                c.Telefono = reader["Telefono"] != DBNull.Value ? reader["Telefono"].ToString() : "";
                c.Direccion = reader["Direccion"] != DBNull.Value ? reader["Direccion"].ToString() : "";
                c.TipoCliente = (int)reader["TipoCliente"];
                c.Activo = (bool)reader["Activo"];
                c.FechaRegistro = (DateTime)reader["FechaRegistro"];
            }
            reader.Close();
            conn.Close();
            return c;
        }

        public static int InsertarCliente(string nombre, string apellido, string email,
            string telefono, string direccion, int tipoCliente)
        {
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            string sql = "INSERT INTO Clientes (Nombre, Apellido, Email, Telefono, Direccion, TipoCliente) " +
                         "VALUES ('" + nombre + "', '" + apellido + "', '" + email + "', '" + telefono + "', '" + direccion + "', " + tipoCliente + "); SELECT SCOPE_IDENTITY();";
            SqlCommand cmd = new SqlCommand(sql, conn);
            int id = Convert.ToInt32(cmd.ExecuteScalar());
            conn.Close();
            return id;
        }

        public static void ActualizarCliente(int id, string nombre, string apellido, string email,
            string telefono, string direccion, int tipoCliente)
        {
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            string sql = "UPDATE Clientes SET Nombre = '" + nombre + "', Apellido = '" + apellido + "', " +
                         "Email = '" + email + "', Telefono = '" + telefono + "', Direccion = '" + direccion + "', " +
                         "TipoCliente = " + tipoCliente + " WHERE Id = " + id;
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        // ============================================================
        // PEDIDOS
        // ============================================================

        public static List<Pedido> ObtenerPedidos()
        {
            List<Pedido> pedidos = new List<Pedido>();
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            string sql = "SELECT p.*, c.Nombre + ' ' + c.Apellido AS NombreCliente, c.Email AS EmailCliente " +
                         "FROM Pedidos p INNER JOIN Clientes c ON p.ClienteId = c.Id ORDER BY p.FechaPedido DESC";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Pedido p = new Pedido();
                p.Id = (int)reader["Id"];
                p.ClienteId = (int)reader["ClienteId"];
                p.FechaPedido = (DateTime)reader["FechaPedido"];
                p.Estado = (int)reader["Estado"];
                p.SubTotal = reader["SubTotal"] != DBNull.Value ? (decimal)reader["SubTotal"] : 0;
                p.Impuesto = reader["Impuesto"] != DBNull.Value ? (decimal)reader["Impuesto"] : 0;
                p.Total = reader["Total"] != DBNull.Value ? (decimal)reader["Total"] : 0;
                p.Direccion = reader["Direccion"] != DBNull.Value ? reader["Direccion"].ToString() : "";
                p.Notas = reader["Notas"] != DBNull.Value ? reader["Notas"].ToString() : "";
                p.NombreCliente = reader["NombreCliente"].ToString();
                p.EmailCliente = reader["EmailCliente"] != DBNull.Value ? reader["EmailCliente"].ToString() : "";
                pedidos.Add(p);
            }
            reader.Close();
            conn.Close();
            return pedidos;
        }

        public static List<Pedido> ObtenerPedidosPorEstado(int estado)
        {
            List<Pedido> pedidos = new List<Pedido>();
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            string sql = "SELECT p.*, c.Nombre + ' ' + c.Apellido AS NombreCliente, c.Email AS EmailCliente " +
                         "FROM Pedidos p INNER JOIN Clientes c ON p.ClienteId = c.Id WHERE p.Estado = " + estado + " ORDER BY p.FechaPedido DESC";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Pedido p = new Pedido();
                p.Id = (int)reader["Id"];
                p.ClienteId = (int)reader["ClienteId"];
                p.FechaPedido = (DateTime)reader["FechaPedido"];
                p.Estado = (int)reader["Estado"];
                p.SubTotal = reader["SubTotal"] != DBNull.Value ? (decimal)reader["SubTotal"] : 0;
                p.Impuesto = reader["Impuesto"] != DBNull.Value ? (decimal)reader["Impuesto"] : 0;
                p.Total = reader["Total"] != DBNull.Value ? (decimal)reader["Total"] : 0;
                p.Direccion = reader["Direccion"] != DBNull.Value ? reader["Direccion"].ToString() : "";
                p.NombreCliente = reader["NombreCliente"].ToString();
                p.EmailCliente = reader["EmailCliente"] != DBNull.Value ? reader["EmailCliente"].ToString() : "";
                pedidos.Add(p);
            }
            reader.Close();
            conn.Close();
            return pedidos;
        }

        // Sin transacción: si falla a mitad, queda inconsistente
        public static int CrearPedido(int clienteId, string direccion, string notas,
            List<DetallePedido> detalles)
        {
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();

            // Calcular totales
            decimal subtotal = 0;
            foreach (var d in detalles)
            {
                subtotal += d.Subtotal;
            }
            decimal impuesto = subtotal * (decimal)Configuracion.ImpuestoVenta;
            decimal total = subtotal + impuesto;

            // Insertar pedido
            string sql = "INSERT INTO Pedidos (ClienteId, Estado, SubTotal, Impuesto, Total, Direccion, Notas) " +
                         "VALUES (" + clienteId + ", 1, " + subtotal + ", " + impuesto + ", " + total + ", '" + direccion + "', '" + notas + "'); SELECT SCOPE_IDENTITY();";
            SqlCommand cmd = new SqlCommand(sql, conn);
            int pedidoId = Convert.ToInt32(cmd.ExecuteScalar());

            // Insertar detalles - sin validar stock suficiente
            foreach (var d in detalles)
            {
                string sqlDetalle = "INSERT INTO DetallePedidos (PedidoId, ProductoId, Cantidad, PrecioUnitario, Descuento, Subtotal) " +
                                    "VALUES (" + pedidoId + ", " + d.ProductoId + ", " + d.Cantidad + ", " + d.PrecioUnitario + ", " + d.Descuento + ", " + d.Subtotal + ")";
                SqlCommand cmdDetalle = new SqlCommand(sqlDetalle, conn);
                cmdDetalle.ExecuteNonQuery();

                // Reducir stock - no valida si hay suficiente
                string sqlStock = "UPDATE Productos SET Stock = Stock - " + d.Cantidad + " WHERE Id = " + d.ProductoId;
                SqlCommand cmdStock = new SqlCommand(sqlStock, conn);
                cmdStock.ExecuteNonQuery();

                // Registrar movimiento
                string sqlMov = "INSERT INTO MovimientosInventario (ProductoId, TipoMovimiento, Cantidad, Motivo) " +
                                "VALUES (" + d.ProductoId + ", 2, " + d.Cantidad + ", 'Venta - Pedido #" + pedidoId + "')";
                SqlCommand cmdMov = new SqlCommand(sqlMov, conn);
                cmdMov.ExecuteNonQuery();
            }

            conn.Close();
            return pedidoId;
        }

        public static void ActualizarEstadoPedido(int pedidoId, int nuevoEstado)
        {
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            string sql = "UPDATE Pedidos SET Estado = " + nuevoEstado;
            // Agregar fecha según estado
            if (nuevoEstado == 3) // Enviado
                sql += ", FechaEnvio = GETDATE()";
            if (nuevoEstado == 4) // Entregado
                sql += ", FechaEntrega = GETDATE()";
            sql += " WHERE Id = " + pedidoId;
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public static List<DetallePedido> ObtenerDetallesPedido(int pedidoId)
        {
            List<DetallePedido> detalles = new List<DetallePedido>();
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            string sql = "SELECT dp.*, pr.Nombre AS NombreProducto FROM DetallePedidos dp " +
                         "INNER JOIN Productos pr ON dp.ProductoId = pr.Id WHERE dp.PedidoId = " + pedidoId;
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DetallePedido d = new DetallePedido();
                d.Id = (int)reader["Id"];
                d.PedidoId = (int)reader["PedidoId"];
                d.ProductoId = (int)reader["ProductoId"];
                d.Cantidad = (int)reader["Cantidad"];
                d.PrecioUnitario = (decimal)reader["PrecioUnitario"];
                d.Descuento = (decimal)reader["Descuento"];
                d.Subtotal = (decimal)reader["Subtotal"];
                d.NombreProducto = reader["NombreProducto"].ToString();
                detalles.Add(d);
            }
            reader.Close();
            conn.Close();
            return detalles;
        }

        // ============================================================
        // CATEGORIAS
        // ============================================================

        public static List<Categoria> ObtenerCategorias()
        {
            List<Categoria> categorias = new List<Categoria>();
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Categorias WHERE Activa = 1 ORDER BY Nombre", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Categoria c = new Categoria();
                c.Id = (int)reader["Id"];
                c.Nombre = reader["Nombre"].ToString();
                c.Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : "";
                c.Activa = (bool)reader["Activa"];
                categorias.Add(c);
            }
            reader.Close();
            conn.Close();
            return categorias;
        }

        // ============================================================
        // PROVEEDORES
        // ============================================================

        public static List<Proveedor> ObtenerProveedores()
        {
            List<Proveedor> proveedores = new List<Proveedor>();
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Proveedores WHERE Activo = 1 ORDER BY Nombre", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Proveedor p = new Proveedor();
                p.Id = (int)reader["Id"];
                p.Nombre = reader["Nombre"].ToString();
                p.Contacto = reader["Contacto"] != DBNull.Value ? reader["Contacto"].ToString() : "";
                p.Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "";
                p.Telefono = reader["Telefono"] != DBNull.Value ? reader["Telefono"].ToString() : "";
                p.Direccion = reader["Direccion"] != DBNull.Value ? reader["Direccion"].ToString() : "";
                p.Activo = (bool)reader["Activo"];
                proveedores.Add(p);
            }
            reader.Close();
            conn.Close();
            return proveedores;
        }

        // ============================================================
        // MOVIMIENTOS DE INVENTARIO
        // ============================================================

        public static void RegistrarMovimiento(int productoId, int tipo, int cantidad, string motivo)
        {
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            string sql = "INSERT INTO MovimientosInventario (ProductoId, TipoMovimiento, Cantidad, Motivo) " +
                         "VALUES (" + productoId + ", " + tipo + ", " + cantidad + ", '" + motivo + "')";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public static List<MovimientoInventario> ObtenerMovimientos(int productoId)
        {
            List<MovimientoInventario> movimientos = new List<MovimientoInventario>();
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            string sql = "SELECT mi.*, p.Nombre AS NombreProducto FROM MovimientosInventario mi " +
                         "INNER JOIN Productos p ON mi.ProductoId = p.Id WHERE mi.ProductoId = " + productoId + " ORDER BY mi.Fecha DESC";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                MovimientoInventario m = new MovimientoInventario();
                m.Id = (int)reader["Id"];
                m.ProductoId = (int)reader["ProductoId"];
                m.TipoMovimiento = (int)reader["TipoMovimiento"];
                m.Cantidad = (int)reader["Cantidad"];
                m.Motivo = reader["Motivo"] != DBNull.Value ? reader["Motivo"].ToString() : "";
                m.Fecha = (DateTime)reader["Fecha"];
                m.NombreProducto = reader["NombreProducto"].ToString();
                movimientos.Add(m);
            }
            reader.Close();
            conn.Close();
            return movimientos;
        }

        // ============================================================
        // USUARIOS Y AUTENTICACIÓN
        // ============================================================

        // Contraseñas almacenadas en texto plano - problema de seguridad grave
        public static Usuario ValidarUsuario(string nombreUsuario, string contrasena)
        {
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            // SQL Injection en autenticación - el peor lugar posible
            string sql = "SELECT * FROM Usuarios WHERE NombreUsuario = '" + nombreUsuario + "' AND Contrasena = '" + contrasena + "' AND Activo = 1";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            Usuario u = null;
            if (reader.Read())
            {
                u = new Usuario();
                u.Id = (int)reader["Id"];
                u.NombreUsuario = reader["NombreUsuario"].ToString();
                u.NombreCompleto = reader["NombreCompleto"] != DBNull.Value ? reader["NombreCompleto"].ToString() : "";
                u.Rol = (int)reader["Rol"];
                u.Activo = (bool)reader["Activo"];
            }
            reader.Close();

            // Actualizar último acceso
            if (u != null)
            {
                SqlCommand cmdUpdate = new SqlCommand("UPDATE Usuarios SET UltimoAcceso = GETDATE() WHERE Id = " + u.Id, conn);
                cmdUpdate.ExecuteNonQuery();
            }

            conn.Close();
            return u;
        }

        // ============================================================
        // REPORTES (queries directos, deberían estar en stored procedures)
        // ============================================================

        public static ResumenVentas ObtenerResumenVentas()
        {
            ResumenVentas resumen = new ResumenVentas();
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();

            SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM Pedidos", conn);
            resumen.TotalPedidos = (int)cmd1.ExecuteScalar();

            SqlCommand cmd2 = new SqlCommand("SELECT ISNULL(SUM(Total), 0) FROM Pedidos WHERE Estado != 5", conn);
            resumen.MontoTotal = (decimal)cmd2.ExecuteScalar();

            if (resumen.TotalPedidos > 0)
                resumen.PromedioVenta = resumen.MontoTotal / resumen.TotalPedidos;

            SqlCommand cmd3 = new SqlCommand("SELECT COUNT(*) FROM Pedidos WHERE Estado = 1", conn);
            resumen.PedidosPendientes = (int)cmd3.ExecuteScalar();

            SqlCommand cmd4 = new SqlCommand("SELECT COUNT(*) FROM Pedidos WHERE Estado = 4", conn);
            resumen.PedidosCompletados = (int)cmd4.ExecuteScalar();

            SqlCommand cmd5 = new SqlCommand("SELECT COUNT(*) FROM Pedidos WHERE Estado = 5", conn);
            resumen.PedidosCancelados = (int)cmd5.ExecuteScalar();

            conn.Close();
            return resumen;
        }

        public static decimal ObtenerVentasPorMes(int anio, int mes)
        {
            SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
            conn.Open();
            string sql = "SELECT ISNULL(SUM(Total), 0) FROM Pedidos WHERE YEAR(FechaPedido) = " + anio +
                         " AND MONTH(FechaPedido) = " + mes + " AND Estado != 5";
            SqlCommand cmd = new SqlCommand(sql, conn);
            decimal total = (decimal)cmd.ExecuteScalar();
            conn.Close();
            return total;
        }
    }
}
