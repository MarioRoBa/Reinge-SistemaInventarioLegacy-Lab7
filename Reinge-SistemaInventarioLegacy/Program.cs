using System;
using System.Collections.Generic;

namespace SistemaInventarioLegacy
{
    class Program
    {
        static Usuario usuarioActual = null;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = Configuracion.NombreEmpresa + " - Sistema de Inventario v" + Configuracion.VersionSistema;

            if (Configuracion.ModoDebug)
            {
                Configuracion.MostrarConfiguracion();
                Console.WriteLine();
            }

            // Login
            bool autenticado = false;
            int intentos = 0;

            while (!autenticado && intentos < Configuracion.MaximoIntentos)
            {
                Console.WriteLine("═══ INICIO DE SESIÓN ═══");
                Console.Write("  Usuario: ");
                string usuario = Console.ReadLine();
                Console.Write("  Contraseña: ");
                string contrasena = Console.ReadLine();

                usuarioActual = AccesoDatos.ValidarUsuario(usuario, contrasena);

                if (usuarioActual != null)
                {
                    autenticado = true;
                    Utilidades.RegistrarAcceso(usuarioActual.NombreUsuario, "Login exitoso");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("  ¡Bienvenido, " + usuarioActual.NombreCompleto + "!");
                    Console.ResetColor();
                    Console.WriteLine("  Rol: " + Utilidades.ObtenerNombreRol(usuarioActual.Rol));
                    System.Threading.Thread.Sleep(1500);
                }
                else
                {
                    intentos++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  Credenciales incorrectas. Intento " + intentos + " de " + Configuracion.MaximoIntentos);
                    Console.ResetColor();
                    Utilidades.RegistrarError("Login fallido para usuario: " + usuario);
                }
            }

            if (!autenticado)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Se excedió el número máximo de intentos. El programa se cerrará.");
                Console.ResetColor();
                return;
            }

            // Menú principal - God Class: todo el flujo en una sola clase
            bool salir = false;
            while (!salir)
            {
                Utilidades.MostrarEncabezado("MENÚ PRINCIPAL - " + Configuracion.NombreEmpresa);
                Console.WriteLine("║  Usuario: " + usuarioActual.NombreCompleto.PadRight(36) + "║");
                Console.WriteLine("╚══════════════════════════════════════════════════╝");
                Console.WriteLine();
                Console.WriteLine("  1. Gestión de Productos");
                Console.WriteLine("  2. Gestión de Clientes");
                Console.WriteLine("  3. Gestión de Pedidos");
                Console.WriteLine("  4. Inventario");
                Console.WriteLine("  5. Reportes");
                Console.WriteLine("  6. Proveedores");

                // Verificación de permisos hardcoded
                if (usuarioActual.Rol >= 2) // Magic number: 2 = supervisor
                {
                    Console.WriteLine("  7. Administración");
                }

                Console.WriteLine("  0. Salir");
                Console.WriteLine();

                int opcion = Utilidades.LeerEntero("  Seleccione: ");

                switch (opcion)
                {
                    case 1: MenuProductos(); break;
                    case 2: MenuClientes(); break;
                    case 3: MenuPedidos(); break;
                    case 4: MenuInventario(); break;
                    case 5: MenuReportes(); break;
                    case 6: MenuProveedores(); break;
                    case 7:
                        if (usuarioActual.Rol >= 2)
                            MenuAdministracion();
                        else
                            Utilidades.MostrarMensajeError("No tiene permisos para esta opción.");
                        break;
                    case 0:
                        salir = Utilidades.Confirmar("¿Desea salir del sistema?");
                        if (salir)
                            Utilidades.RegistrarAcceso(usuarioActual.NombreUsuario, "Logout");
                        break;
                    default:
                        Utilidades.MostrarMensajeError("Opción no válida.");
                        break;
                }
            }

            Console.WriteLine("  Gracias por usar el sistema. ¡Hasta pronto!");
        }

        // ============================================================
        // MENÚ PRODUCTOS - Lógica de negocio mezclada con presentación
        // ============================================================

        static void MenuProductos()
        {
            bool volver = false;
            while (!volver)
            {
                Utilidades.MostrarEncabezado("GESTIÓN DE PRODUCTOS");
                Console.WriteLine("  1. Listar todos los productos");
                Console.WriteLine("  2. Buscar producto");
                Console.WriteLine("  3. Ver producto por ID");
                Console.WriteLine("  4. Agregar nuevo producto");
                Console.WriteLine("  5. Editar producto");
                Console.WriteLine("  6. Eliminar producto");
                Console.WriteLine("  7. Productos por categoría");
                Console.WriteLine("  0. Volver");

                int opcion = Utilidades.LeerEntero("  Seleccione: ");

                switch (opcion)
                {
                    case 1:
                        ListarProductos();
                        break;
                    case 2:
                        BuscarProductos();
                        break;
                    case 3:
                        VerProducto();
                        break;
                    case 4:
                        AgregarProducto();
                        break;
                    case 5:
                        EditarProducto();
                        break;
                    case 6:
                        EliminarProducto();
                        break;
                    case 7:
                        ProductosPorCategoria();
                        break;
                    case 0:
                        volver = true;
                        break;
                }
            }
        }

        static void ListarProductos()
        {
            Utilidades.MostrarEncabezado("LISTA DE PRODUCTOS");
            List<Producto> productos = AccesoDatos.ObtenerProductos();

            Console.WriteLine("  {0,-5} {1,-8} {2,-25} {3,12} {4,8}",
                "ID", "Código", "Nombre", "Precio", "Stock");
            Utilidades.MostrarSeparador();

            foreach (var p in productos)
            {
                if (p.Stock <= p.StockMinimo)
                    Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine("  {0,-5} {1,-8} {2,-25} {3,12} {4,8}",
                    p.Id, p.Codigo,
                    p.Nombre.Length > 25 ? p.Nombre.Substring(0, 22) + "..." : p.Nombre,
                    Utilidades.FormatearMoneda(p.PrecioVenta),
                    p.Stock);

                Console.ResetColor();
            }

            Console.WriteLine();
            Console.WriteLine("  Total: " + productos.Count + " productos");
            Utilidades.Pausar();
        }

        static void BuscarProductos()
        {
            Utilidades.MostrarEncabezado("BUSCAR PRODUCTO");
            string termino = Utilidades.LeerTexto("  Término de búsqueda: ");

            // El término va directo a SQL sin sanitizar
            List<Producto> productos = AccesoDatos.BuscarProductos(termino);

            if (productos.Count == 0)
            {
                Utilidades.MostrarMensajeAdvertencia("No se encontraron productos.");
                Utilidades.Pausar();
                return;
            }

            Console.WriteLine("  Resultados: " + productos.Count);
            Console.WriteLine();

            foreach (var p in productos)
            {
                Console.WriteLine("  [{0}] {1} - {2} | Stock: {3} | Precio: {4}",
                    p.Id, p.Codigo, p.Nombre, p.Stock, Utilidades.FormatearMoneda(p.PrecioVenta));
            }

            Utilidades.Pausar();
        }

        static void VerProducto()
        {
            Utilidades.MostrarEncabezado("DETALLE DE PRODUCTO");
            int id = Utilidades.LeerEntero("  ID del producto: ");
            Producto p = AccesoDatos.ObtenerProductoPorId(id);

            if (p == null)
            {
                Utilidades.MostrarMensajeError("Producto no encontrado.");
                Utilidades.Pausar();
                return;
            }

            Console.WriteLine("  ID:                " + p.Id);
            Console.WriteLine("  Código:            " + p.Codigo);
            Console.WriteLine("  Nombre:            " + p.Nombre);
            Console.WriteLine("  Descripción:       " + p.Descripcion);
            Console.WriteLine("  Precio de compra:  " + Utilidades.FormatearMoneda(p.PrecioCompra));
            Console.WriteLine("  Precio de venta:   " + Utilidades.FormatearMoneda(p.PrecioVenta));
            Console.WriteLine("  Margen:            " + Utilidades.CalcularMargen(p.PrecioCompra, p.PrecioVenta).ToString("N1") + "%");
            Console.WriteLine("  Stock:             " + p.Stock);
            Console.WriteLine("  Stock mínimo:      " + p.StockMinimo);
            Console.WriteLine("  Categoría ID:      " + p.CategoriaId);
            Console.WriteLine("  Creado:            " + Utilidades.FormatearFecha(p.FechaCreacion));
            Console.WriteLine("  Última modif.:     " + Utilidades.FormatearFecha(p.UltimaModificacion));

            if (p.Stock <= p.StockMinimo)
            {
                Utilidades.MostrarMensajeAdvertencia("¡Stock bajo! Se recomienda reabastecer.");
            }

            Utilidades.Pausar();
        }

        static void AgregarProducto()
        {
            Utilidades.MostrarEncabezado("AGREGAR PRODUCTO");

            // Sin validaciones de negocio
            string codigo = Utilidades.LeerTexto("  Código: ");
            string nombre = Utilidades.LeerTexto("  Nombre: ");
            string descripcion = Utilidades.LeerTexto("  Descripción: ", false);
            decimal precioCompra = Utilidades.LeerDecimal("  Precio de compra: ");
            decimal precioVenta = Utilidades.LeerDecimal("  Precio de venta: ");
            int stock = Utilidades.LeerEntero("  Stock inicial: ");
            int stockMinimo = Utilidades.LeerEntero("  Stock mínimo: ");

            // Mostrar categorías
            Console.WriteLine("  Categorías disponibles:");
            List<Categoria> categorias = AccesoDatos.ObtenerCategorias();
            foreach (var c in categorias)
            {
                Console.WriteLine("    " + c.Id + ". " + c.Nombre);
            }
            int categoriaId = Utilidades.LeerEntero("  Categoría: ");

            // No valida que precioVenta > precioCompra
            // No valida que stock >= 0
            // No valida que categoriaId sea válido
            // No valida código duplicado

            if (Utilidades.Confirmar("¿Guardar producto?"))
            {
                int id = AccesoDatos.InsertarProducto(codigo, nombre, descripcion,
                    precioCompra, precioVenta, stock, stockMinimo, categoriaId);

                // Registrar movimiento de entrada
                AccesoDatos.RegistrarMovimiento(id, 1, stock, "Stock inicial");

                Utilidades.MostrarMensajeExito("Producto creado con ID: " + id);
                Utilidades.RegistrarAcceso(usuarioActual.NombreUsuario, "Creó producto: " + nombre);
            }

            Utilidades.Pausar();
        }

        static void EditarProducto()
        {
            Utilidades.MostrarEncabezado("EDITAR PRODUCTO");
            int id = Utilidades.LeerEntero("  ID del producto a editar: ");
            Producto p = AccesoDatos.ObtenerProductoPorId(id);

            if (p == null)
            {
                Utilidades.MostrarMensajeError("Producto no encontrado.");
                Utilidades.Pausar();
                return;
            }

            Console.WriteLine("  Producto actual: " + p.Nombre);
            Console.WriteLine("  (Deje en blanco para mantener el valor actual)");
            Console.WriteLine();

            Console.Write("  Nombre [" + p.Nombre + "]: ");
            string nombre = Console.ReadLine();
            if (string.IsNullOrEmpty(nombre)) nombre = p.Nombre;

            Console.Write("  Descripción [" + p.Descripcion + "]: ");
            string descripcion = Console.ReadLine();
            if (string.IsNullOrEmpty(descripcion)) descripcion = p.Descripcion;

            Console.Write("  Precio compra [" + p.PrecioCompra + "]: ");
            string precioCompraStr = Console.ReadLine();
            decimal precioCompra = string.IsNullOrEmpty(precioCompraStr) ? p.PrecioCompra : decimal.Parse(precioCompraStr);

            Console.Write("  Precio venta [" + p.PrecioVenta + "]: ");
            string precioVentaStr = Console.ReadLine();
            decimal precioVenta = string.IsNullOrEmpty(precioVentaStr) ? p.PrecioVenta : decimal.Parse(precioVentaStr);

            Console.Write("  Stock mínimo [" + p.StockMinimo + "]: ");
            string stockMinimoStr = Console.ReadLine();
            int stockMinimo = string.IsNullOrEmpty(stockMinimoStr) ? p.StockMinimo : int.Parse(stockMinimoStr);

            Console.Write("  Categoría ID [" + p.CategoriaId + "]: ");
            string categoriaIdStr = Console.ReadLine();
            int categoriaId = string.IsNullOrEmpty(categoriaIdStr) ? p.CategoriaId : int.Parse(categoriaIdStr);

            // decimal.Parse puede lanzar excepción - no hay try/catch
            if (Utilidades.Confirmar("¿Guardar cambios?"))
            {
                AccesoDatos.ActualizarProducto(id, nombre, descripcion, precioCompra, precioVenta, stockMinimo, categoriaId);
                Utilidades.MostrarMensajeExito("Producto actualizado correctamente.");
                Utilidades.RegistrarAcceso(usuarioActual.NombreUsuario, "Editó producto ID: " + id);
            }

            Utilidades.Pausar();
        }

        static void EliminarProducto()
        {
            Utilidades.MostrarEncabezado("ELIMINAR PRODUCTO");
            int id = Utilidades.LeerEntero("  ID del producto a eliminar: ");
            Producto p = AccesoDatos.ObtenerProductoPorId(id);

            if (p == null)
            {
                Utilidades.MostrarMensajeError("Producto no encontrado.");
                Utilidades.Pausar();
                return;
            }

            Console.WriteLine("  Producto: " + p.Nombre);
            Console.WriteLine("  Stock: " + p.Stock);

            // No verifica si hay pedidos pendientes con este producto
            if (Utilidades.Confirmar("¿Está seguro de eliminar este producto?"))
            {
                AccesoDatos.EliminarProducto(id);
                Utilidades.MostrarMensajeExito("Producto eliminado (desactivado).");
                Utilidades.RegistrarAcceso(usuarioActual.NombreUsuario, "Eliminó producto: " + p.Nombre);
            }

            Utilidades.Pausar();
        }

        static void ProductosPorCategoria()
        {
            Utilidades.MostrarEncabezado("PRODUCTOS POR CATEGORÍA");
            List<Categoria> categorias = AccesoDatos.ObtenerCategorias();

            Console.WriteLine("  Categorías:");
            foreach (var c in categorias)
            {
                Console.WriteLine("    " + c.Id + ". " + c.Nombre);
            }

            int catId = Utilidades.LeerEntero("  Seleccione categoría: ");
            List<Producto> productos = AccesoDatos.ObtenerProductosPorCategoria(catId);

            Console.WriteLine();
            Console.WriteLine("  Productos en esta categoría: " + productos.Count);
            Utilidades.MostrarSeparador();

            foreach (var p in productos)
            {
                Console.WriteLine("  [{0}] {1} | {2} | Stock: {3}",
                    p.Codigo, p.Nombre, Utilidades.FormatearMoneda(p.PrecioVenta), p.Stock);
            }

            Utilidades.Pausar();
        }

        // ============================================================
        // MENÚ CLIENTES
        // ============================================================

        static void MenuClientes()
        {
            bool volver = false;
            while (!volver)
            {
                Utilidades.MostrarEncabezado("GESTIÓN DE CLIENTES");
                Console.WriteLine("  1. Listar clientes");
                Console.WriteLine("  2. Buscar cliente");
                Console.WriteLine("  3. Ver detalle de cliente");
                Console.WriteLine("  4. Agregar cliente");
                Console.WriteLine("  5. Editar cliente");
                Console.WriteLine("  0. Volver");

                int opcion = Utilidades.LeerEntero("  Seleccione: ");

                switch (opcion)
                {
                    case 1:
                        Utilidades.MostrarEncabezado("LISTA DE CLIENTES");
                        List<Cliente> clientes = AccesoDatos.ObtenerClientes();
                        foreach (var c in clientes)
                        {
                            Console.WriteLine("  [{0}] {1} {2} | {3} | {4}",
                                c.Id, c.Nombre, c.Apellido,
                                Utilidades.ObtenerNombreTipoCliente(c.TipoCliente),
                                c.Email);
                        }
                        Console.WriteLine("  Total: " + clientes.Count);
                        Utilidades.Pausar();
                        break;
                    case 2:
                        Utilidades.MostrarEncabezado("BUSCAR CLIENTE");
                        string termino = Utilidades.LeerTexto("  Término: ");
                        List<Cliente> resultados = AccesoDatos.BuscarClientes(termino);
                        foreach (var c in resultados)
                        {
                            Console.WriteLine("  [{0}] {1} {2} | {3}",
                                c.Id, c.Nombre, c.Apellido, c.Email);
                        }
                        if (resultados.Count == 0)
                            Utilidades.MostrarMensajeAdvertencia("Sin resultados.");
                        Utilidades.Pausar();
                        break;
                    case 3:
                        Utilidades.MostrarEncabezado("DETALLE DE CLIENTE");
                        int idCliente = Utilidades.LeerEntero("  ID: ");
                        Cliente cliente = AccesoDatos.ObtenerClientePorId(idCliente);
                        if (cliente != null)
                            cliente.MostrarInfo();
                        else
                            Utilidades.MostrarMensajeError("No encontrado.");
                        Utilidades.Pausar();
                        break;
                    case 4:
                        AgregarCliente();
                        break;
                    case 5:
                        EditarCliente();
                        break;
                    case 0:
                        volver = true;
                        break;
                }
            }
        }

        static void AgregarCliente()
        {
            Utilidades.MostrarEncabezado("AGREGAR CLIENTE");

            string nombre = Utilidades.LeerTexto("  Nombre: ");
            string apellido = Utilidades.LeerTexto("  Apellido: ");
            string email = Utilidades.LeerTexto("  Email: ", false);
            string telefono = Utilidades.LeerTexto("  Teléfono: ", false);
            string direccion = Utilidades.LeerTexto("  Dirección: ", false);

            Console.WriteLine("  Tipo de cliente:");
            Console.WriteLine("    1. Regular");
            Console.WriteLine("    2. Mayorista");
            Console.WriteLine("    3. VIP");
            int tipo = Utilidades.LeerEntero("  Tipo: ");

            // Validación de email insuficiente
            if (!string.IsNullOrEmpty(email) && !Utilidades.ValidarEmail(email))
            {
                Utilidades.MostrarMensajeAdvertencia("Email parece inválido, pero se guardará de todos modos.");
            }

            if (Utilidades.Confirmar("¿Guardar cliente?"))
            {
                int id = AccesoDatos.InsertarCliente(nombre, apellido, email, telefono, direccion, tipo);
                Utilidades.MostrarMensajeExito("Cliente creado con ID: " + id);
            }

            Utilidades.Pausar();
        }

        static void EditarCliente()
        {
            Utilidades.MostrarEncabezado("EDITAR CLIENTE");
            int id = Utilidades.LeerEntero("  ID del cliente: ");
            Cliente c = AccesoDatos.ObtenerClientePorId(id);

            if (c == null)
            {
                Utilidades.MostrarMensajeError("Cliente no encontrado.");
                Utilidades.Pausar();
                return;
            }

            Console.WriteLine("  Cliente: " + c.Nombre + " " + c.Apellido);
            Console.Write("  Nombre [" + c.Nombre + "]: ");
            string nombre = Console.ReadLine();
            if (string.IsNullOrEmpty(nombre)) nombre = c.Nombre;

            Console.Write("  Apellido [" + c.Apellido + "]: ");
            string apellido = Console.ReadLine();
            if (string.IsNullOrEmpty(apellido)) apellido = c.Apellido;

            Console.Write("  Email [" + c.Email + "]: ");
            string email = Console.ReadLine();
            if (string.IsNullOrEmpty(email)) email = c.Email;

            Console.Write("  Teléfono [" + c.Telefono + "]: ");
            string telefono = Console.ReadLine();
            if (string.IsNullOrEmpty(telefono)) telefono = c.Telefono;

            Console.Write("  Dirección [" + c.Direccion + "]: ");
            string direccion = Console.ReadLine();
            if (string.IsNullOrEmpty(direccion)) direccion = c.Direccion;

            Console.Write("  Tipo (1=Regular, 2=Mayorista, 3=VIP) [" + c.TipoCliente + "]: ");
            string tipoStr = Console.ReadLine();
            int tipo = string.IsNullOrEmpty(tipoStr) ? c.TipoCliente : int.Parse(tipoStr);

            if (Utilidades.Confirmar("¿Guardar cambios?"))
            {
                AccesoDatos.ActualizarCliente(id, nombre, apellido, email, telefono, direccion, tipo);
                Utilidades.MostrarMensajeExito("Cliente actualizado.");
            }

            Utilidades.Pausar();
        }

        // ============================================================
        // MENÚ PEDIDOS - Lógica de negocio directa
        // ============================================================

        static void MenuPedidos()
        {
            bool volver = false;
            while (!volver)
            {
                Utilidades.MostrarEncabezado("GESTIÓN DE PEDIDOS");
                Console.WriteLine("  1. Nuevo pedido");
                Console.WriteLine("  2. Ver pedidos");
                Console.WriteLine("  3. Ver detalle de pedido");
                Console.WriteLine("  4. Cambiar estado de pedido");
                Console.WriteLine("  0. Volver");

                int opcion = Utilidades.LeerEntero("  Seleccione: ");

                switch (opcion)
                {
                    case 1: CrearPedido(); break;
                    case 2: VerPedidos(); break;
                    case 3: VerDetallePedido(); break;
                    case 4: CambiarEstadoPedido(); break;
                    case 0: volver = true; break;
                }
            }
        }

        // Método largo con lógica de negocio mezclada con UI
        static void CrearPedido()
        {
            Utilidades.MostrarEncabezado("NUEVO PEDIDO");

            // Seleccionar cliente
            Console.WriteLine("  Clientes disponibles:");
            List<Cliente> clientes = AccesoDatos.ObtenerClientes();
            foreach (var c in clientes)
            {
                Console.WriteLine("    {0}. {1} {2} ({3})",
                    c.Id, c.Nombre, c.Apellido, Utilidades.ObtenerNombreTipoCliente(c.TipoCliente));
            }

            int clienteId = Utilidades.LeerEntero("  ID del cliente: ");
            Cliente cliente = AccesoDatos.ObtenerClientePorId(clienteId);

            if (cliente == null)
            {
                Utilidades.MostrarMensajeError("Cliente no encontrado.");
                Utilidades.Pausar();
                return;
            }

            Console.WriteLine("  Cliente: " + cliente.Nombre + " " + cliente.Apellido);
            string direccion = Utilidades.LeerTexto("  Dirección de envío [" + cliente.Direccion + "]: ", false);
            if (string.IsNullOrEmpty(direccion)) direccion = cliente.Direccion;

            string notas = Utilidades.LeerTexto("  Notas: ", false);

            // Agregar productos
            List<DetallePedido> detalles = new List<DetallePedido>();
            bool agregarMas = true;

            while (agregarMas)
            {
                Console.WriteLine();
                Console.WriteLine("  Productos disponibles:");
                List<Producto> productos = AccesoDatos.ObtenerProductos();
                foreach (var p in productos)
                {
                    Console.WriteLine("    {0}. {1} | {2} | Stock: {3}",
                        p.Id, p.Nombre, Utilidades.FormatearMoneda(p.PrecioVenta), p.Stock);
                }

                int productoId = Utilidades.LeerEntero("  ID del producto: ");
                Producto producto = AccesoDatos.ObtenerProductoPorId(productoId);

                if (producto == null)
                {
                    Utilidades.MostrarMensajeError("Producto no encontrado.");
                    continue;
                }

                int cantidad = Utilidades.LeerEntero("  Cantidad: ");

                // No valida stock suficiente aquí (lo hace en la BD, podría quedar negativo)
                if (cantidad <= 0)
                {
                    Utilidades.MostrarMensajeError("La cantidad debe ser mayor a 0.");
                    continue;
                }

                // Calcular descuento basado en tipo de cliente
                decimal descuento = 0;
                if (cliente.TipoCliente == 2) descuento = 10; // Magic number: 10% mayorista
                if (cliente.TipoCliente == 3) descuento = 15; // Magic number: 15% VIP

                decimal precioConDescuento = producto.PrecioVenta * (1 - descuento / 100);
                decimal subtotal = precioConDescuento * cantidad;

                DetallePedido detalle = new DetallePedido();
                detalle.ProductoId = productoId;
                detalle.Cantidad = cantidad;
                detalle.PrecioUnitario = producto.PrecioVenta;
                detalle.Descuento = descuento;
                detalle.Subtotal = subtotal;
                detalle.NombreProducto = producto.Nombre;
                detalles.Add(detalle);

                Console.WriteLine("  → Agregado: " + producto.Nombre + " x" + cantidad +
                    " = " + Utilidades.FormatearMoneda(subtotal));

                agregarMas = Utilidades.Confirmar("¿Agregar otro producto?");
            }

            if (detalles.Count == 0)
            {
                Utilidades.MostrarMensajeAdvertencia("Pedido cancelado - sin productos.");
                Utilidades.Pausar();
                return;
            }

            // Resumen del pedido
            Console.WriteLine();
            Console.WriteLine("  ═══ RESUMEN DEL PEDIDO ═══");
            decimal totalSubtotal = 0;
            foreach (var d in detalles)
            {
                Console.WriteLine("  {0} x{1} → {2}",
                    d.NombreProducto, d.Cantidad, Utilidades.FormatearMoneda(d.Subtotal));
                totalSubtotal += d.Subtotal;
            }

            decimal impuesto = totalSubtotal * (decimal)Configuracion.ImpuestoVenta;
            decimal totalFinal = totalSubtotal + impuesto;

            Console.WriteLine("  ──────────────────────");
            Console.WriteLine("  Subtotal:  " + Utilidades.FormatearMoneda(totalSubtotal));
            Console.WriteLine("  Impuesto:  " + Utilidades.FormatearMoneda(impuesto) + " (13%)");
            Console.WriteLine("  TOTAL:     " + Utilidades.FormatearMoneda(totalFinal));

            if (Utilidades.Confirmar("¿Confirmar pedido?"))
            {
                int pedidoId = AccesoDatos.CrearPedido(clienteId, direccion, notas, detalles);
                Utilidades.MostrarMensajeExito("Pedido #" + pedidoId + " creado exitosamente.");
                Utilidades.RegistrarAcceso(usuarioActual.NombreUsuario, "Creó pedido #" + pedidoId);
            }
            else
            {
                Utilidades.MostrarMensajeAdvertencia("Pedido cancelado.");
            }

            Utilidades.Pausar();
        }

        static void VerPedidos()
        {
            Utilidades.MostrarEncabezado("LISTA DE PEDIDOS");
            List<Pedido> pedidos = AccesoDatos.ObtenerPedidos();

            Console.WriteLine("  {0,-6} {1,-25} {2,-12} {3,12} {4,-12}",
                "ID", "Cliente", "Fecha", "Total", "Estado");
            Utilidades.MostrarSeparador();

            foreach (var p in pedidos)
            {
                Console.WriteLine("  {0,-6} {1,-25} {2,-12} {3,12} {4,-12}",
                    p.Id, p.NombreCliente,
                    Utilidades.FormatearFechaCorta(p.FechaPedido),
                    Utilidades.FormatearMoneda(p.Total),
                    Utilidades.ObtenerNombreEstadoPedido(p.Estado));
            }

            Console.WriteLine("  Total: " + pedidos.Count + " pedidos");
            Utilidades.Pausar();
        }

        static void VerDetallePedido()
        {
            Utilidades.MostrarEncabezado("DETALLE DE PEDIDO");
            int pedidoId = Utilidades.LeerEntero("  ID del pedido: ");
            List<DetallePedido> detalles = AccesoDatos.ObtenerDetallesPedido(pedidoId);

            if (detalles.Count == 0)
            {
                Utilidades.MostrarMensajeError("Pedido no encontrado o sin detalles.");
                Utilidades.Pausar();
                return;
            }

            Console.WriteLine("  {0,-25} {1,8} {2,12} {3,8} {4,12}",
                "Producto", "Cant.", "P.Unit.", "Desc%", "Subtotal");
            Utilidades.MostrarSeparador();

            decimal total = 0;
            foreach (var d in detalles)
            {
                Console.WriteLine("  {0,-25} {1,8} {2,12} {3,7}% {4,12}",
                    d.NombreProducto.Length > 25 ? d.NombreProducto.Substring(0, 22) + "..." : d.NombreProducto,
                    d.Cantidad,
                    Utilidades.FormatearMoneda(d.PrecioUnitario),
                    d.Descuento,
                    Utilidades.FormatearMoneda(d.Subtotal));
                total += d.Subtotal;
            }

            Console.WriteLine();
            Console.WriteLine("  Subtotal: " + Utilidades.FormatearMoneda(total));
            Console.WriteLine("  Impuesto: " + Utilidades.FormatearMoneda(total * (decimal)Configuracion.ImpuestoVenta));
            Console.WriteLine("  Total:    " + Utilidades.FormatearMoneda(total + total * (decimal)Configuracion.ImpuestoVenta));

            Utilidades.Pausar();
        }

        static void CambiarEstadoPedido()
        {
            Utilidades.MostrarEncabezado("CAMBIAR ESTADO DE PEDIDO");
            int pedidoId = Utilidades.LeerEntero("  ID del pedido: ");

            Console.WriteLine("  Nuevo estado:");
            Console.WriteLine("    1. Pendiente");
            Console.WriteLine("    2. Procesando");
            Console.WriteLine("    3. Enviado");
            Console.WriteLine("    4. Entregado");
            Console.WriteLine("    5. Cancelado");

            int nuevoEstado = Utilidades.LeerEntero("  Estado: ");

            // No valida transiciones válidas (ej: no debería poder pasar de Entregado a Pendiente)
            // No valida que el pedido exista
            if (nuevoEstado >= 1 && nuevoEstado <= 5)
            {
                AccesoDatos.ActualizarEstadoPedido(pedidoId, nuevoEstado);
                Utilidades.MostrarMensajeExito("Estado actualizado a: " + Utilidades.ObtenerNombreEstadoPedido(nuevoEstado));

                // Si se cancela, debería devolver stock - pero no lo hace
                if (nuevoEstado == 5)
                {
                    Utilidades.MostrarMensajeAdvertencia("NOTA: El stock NO fue devuelto automáticamente.");
                }
            }
            else
            {
                Utilidades.MostrarMensajeError("Estado no válido.");
            }

            Utilidades.Pausar();
        }

        // ============================================================
        // MENÚ INVENTARIO
        // ============================================================

        static void MenuInventario()
        {
            bool volver = false;
            while (!volver)
            {
                Utilidades.MostrarEncabezado("GESTIÓN DE INVENTARIO");
                Console.WriteLine("  1. Registrar entrada de mercadería");
                Console.WriteLine("  2. Registrar salida / ajuste");
                Console.WriteLine("  3. Productos bajo stock mínimo");
                Console.WriteLine("  4. Ver movimientos de un producto");
                Console.WriteLine("  0. Volver");

                int opcion = Utilidades.LeerEntero("  Seleccione: ");

                switch (opcion)
                {
                    case 1:
                        RegistrarEntrada();
                        break;
                    case 2:
                        RegistrarSalida();
                        break;
                    case 3:
                        Reportes.GenerarReporteProductosBajoStock();
                        break;
                    case 4:
                        Reportes.GenerarReporteMovimientos();
                        break;
                    case 0:
                        volver = true;
                        break;
                }
            }
        }

        static void RegistrarEntrada()
        {
            Utilidades.MostrarEncabezado("ENTRADA DE MERCADERÍA");
            int productoId = Utilidades.LeerEntero("  ID del producto: ");
            Producto p = AccesoDatos.ObtenerProductoPorId(productoId);

            if (p == null)
            {
                Utilidades.MostrarMensajeError("Producto no encontrado.");
                Utilidades.Pausar();
                return;
            }

            Console.WriteLine("  Producto: " + p.Nombre + " | Stock actual: " + p.Stock);
            int cantidad = Utilidades.LeerEntero("  Cantidad a ingresar: ");
            string motivo = Utilidades.LeerTexto("  Motivo: ");

            if (cantidad > 0)
            {
                int nuevoStock = p.Stock + cantidad;
                AccesoDatos.ActualizarStock(productoId, nuevoStock);
                AccesoDatos.RegistrarMovimiento(productoId, 1, cantidad, motivo);
                Utilidades.MostrarMensajeExito("Stock actualizado. Nuevo stock: " + nuevoStock);
            }
            else
            {
                Utilidades.MostrarMensajeError("La cantidad debe ser mayor a 0.");
            }

            Utilidades.Pausar();
        }

        static void RegistrarSalida()
        {
            Utilidades.MostrarEncabezado("SALIDA / AJUSTE DE INVENTARIO");
            int productoId = Utilidades.LeerEntero("  ID del producto: ");
            Producto p = AccesoDatos.ObtenerProductoPorId(productoId);

            if (p == null)
            {
                Utilidades.MostrarMensajeError("Producto no encontrado.");
                Utilidades.Pausar();
                return;
            }

            Console.WriteLine("  Producto: " + p.Nombre + " | Stock actual: " + p.Stock);
            Console.WriteLine("  Tipo: 2=Salida, 3=Ajuste");
            int tipo = Utilidades.LeerEntero("  Tipo de movimiento: ");
            int cantidad = Utilidades.LeerEntero("  Cantidad: ");
            string motivo = Utilidades.LeerTexto("  Motivo: ");

            // No valida que quede stock suficiente (puede quedar negativo)
            int nuevoStock = p.Stock - cantidad;
            AccesoDatos.ActualizarStock(productoId, nuevoStock);
            AccesoDatos.RegistrarMovimiento(productoId, tipo, cantidad, motivo);

            if (nuevoStock < 0)
            {
                Utilidades.MostrarMensajeAdvertencia("¡ATENCIÓN! El stock quedó en negativo: " + nuevoStock);
            }

            Utilidades.MostrarMensajeExito("Movimiento registrado. Nuevo stock: " + nuevoStock);
            Utilidades.Pausar();
        }

        // ============================================================
        // MENÚ REPORTES
        // ============================================================

        static void MenuReportes()
        {
            bool volver = false;
            while (!volver)
            {
                Utilidades.MostrarEncabezado("REPORTES");
                Console.WriteLine("  1. Inventario completo");
                Console.WriteLine("  2. Productos bajo stock");
                Console.WriteLine("  3. Ventas");
                Console.WriteLine("  4. Clientes");
                Console.WriteLine("  5. Pedidos por estado");
                Console.WriteLine("  6. Exportar inventario a archivo");
                Console.WriteLine("  0. Volver");

                int opcion = Utilidades.LeerEntero("  Seleccione: ");

                switch (opcion)
                {
                    case 1: Reportes.GenerarReporteInventario(); break;
                    case 2: Reportes.GenerarReporteProductosBajoStock(); break;
                    case 3: Reportes.GenerarReporteVentas(); break;
                    case 4: Reportes.GenerarReporteClientes(); break;
                    case 5: Reportes.GenerarReportePedidos(); break;
                    case 6: Reportes.ExportarReporteInventarioTexto(); break;
                    case 0: volver = true; break;
                }
            }
        }

        // ============================================================
        // MENÚ PROVEEDORES - Mínimo funcional
        // ============================================================

        static void MenuProveedores()
        {
            Utilidades.MostrarEncabezado("PROVEEDORES");
            List<Proveedor> proveedores = AccesoDatos.ObtenerProveedores();

            Console.WriteLine("  {0,-5} {1,-25} {2,-20} {3,-25}",
                "ID", "Nombre", "Contacto", "Email");
            Utilidades.MostrarSeparador();

            foreach (var p in proveedores)
            {
                Console.WriteLine("  {0,-5} {1,-25} {2,-20} {3,-25}",
                    p.Id, p.Nombre, p.Contacto, p.Email);
            }

            Utilidades.Pausar();
        }

        // ============================================================
        // MENÚ ADMINISTRACIÓN - Solo para supervisores/admins
        // ============================================================

        static void MenuAdministracion()
        {
            Utilidades.MostrarEncabezado("ADMINISTRACIÓN");

            // Solo admin puede ver configuración
            if (usuarioActual.Rol == 3) // Magic number
            {
                Console.WriteLine("  1. Ver configuración del sistema");
                Console.WriteLine("  0. Volver");

                int opcion = Utilidades.LeerEntero("  Seleccione: ");

                if (opcion == 1)
                {
                    Configuracion.MostrarConfiguracion();
                    // Muestra contraseñas en pantalla - problema de seguridad
                    Utilidades.Pausar();
                }
            }
            else
            {
                Utilidades.MostrarMensajeAdvertencia("Su rol no tiene opciones adicionales en este momento.");
                Utilidades.Pausar();
            }
        }
    }
}
