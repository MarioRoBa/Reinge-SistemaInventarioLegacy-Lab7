using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SistemaInventarioLegacy
{
    public class Utilidades
    {
        // ============================================================
        // FORMATEO DE DATOS
        // ============================================================

        public static string FormatearMoneda(decimal monto)
        {
            return "$" + monto.ToString("N2");
        }

        public static string FormatearFecha(DateTime fecha)
        {
            return fecha.ToString("dd/MM/yyyy HH:mm");
        }

        public static string FormatearFechaCorta(DateTime fecha)
        {
            return fecha.ToString("dd/MM/yyyy");
        }

        public static string ObtenerNombreEstadoPedido(int estado)
        {
            if (estado == 1) return "Pendiente";
            if (estado == 2) return "Procesando";
            if (estado == 3) return "Enviado";
            if (estado == 4) return "Entregado";
            if (estado == 5) return "Cancelado";
            return "Desconocido";
        }

        public static string ObtenerNombreTipoCliente(int tipo)
        {
            if (tipo == 1) return "Regular";
            if (tipo == 2) return "Mayorista";
            if (tipo == 3) return "VIP";
            return "Desconocido";
        }

        public static string ObtenerNombreTipoMovimiento(int tipo)
        {
            if (tipo == 1) return "Entrada";
            if (tipo == 2) return "Salida";
            if (tipo == 3) return "Ajuste";
            return "Desconocido";
        }

        public static string ObtenerNombreRol(int rol)
        {
            if (rol == 1) return "Operador";
            if (rol == 2) return "Supervisor";
            if (rol == 3) return "Administrador";
            return "Desconocido";
        }

        // ============================================================
        // VALIDACIONES (incompletas y mezcladas con lógica de UI)
        // ============================================================

        public static bool ValidarEmail(string email)
        {
            // Validación muy básica e incorrecta
            if (email.Contains("@") && email.Contains("."))
                return true;
            return false;
        }

        public static bool ValidarTelefono(string telefono)
        {
            // Solo verifica longitud
            if (telefono.Length >= 8 && telefono.Length <= 15)
                return true;
            return false;
        }

        public static int LeerEntero(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje);
                string input = Console.ReadLine();
                int resultado;
                if (int.TryParse(input, out resultado))
                    return resultado;
                Console.WriteLine("  Error: Ingrese un número válido.");
            }
        }

        public static decimal LeerDecimal(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje);
                string input = Console.ReadLine();
                decimal resultado;
                if (decimal.TryParse(input, out resultado))
                    return resultado;
                Console.WriteLine("  Error: Ingrese un número válido.");
            }
        }

        public static string LeerTexto(string mensaje, bool requerido = true)
        {
            while (true)
            {
                Console.Write(mensaje);
                string input = Console.ReadLine();
                if (!requerido || !string.IsNullOrWhiteSpace(input))
                    return input;
                Console.WriteLine("  Error: Este campo es requerido.");
            }
        }

        public static bool Confirmar(string mensaje)
        {
            Console.Write(mensaje + " (S/N): ");
            string respuesta = Console.ReadLine().Trim().ToUpper();
            return respuesta == "S" || respuesta == "SI" || respuesta == "SÍ";
        }

        // ============================================================
        // UTILIDADES DE PRESENTACIÓN
        // ============================================================

        public static void MostrarEncabezado(string titulo)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════════════╗");
            Console.WriteLine("║  " + titulo.PadRight(47) + "║");
            Console.WriteLine("╠══════════════════════════════════════════════════╣");
            Console.ResetColor();
        }

        public static void MostrarSeparador()
        {
            Console.WriteLine("──────────────────────────────────────────────────");
        }

        public static void MostrarMensajeExito(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  ✓ " + mensaje);
            Console.ResetColor();
        }

        public static void MostrarMensajeError(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  ✗ " + mensaje);
            Console.ResetColor();
        }

        public static void MostrarMensajeAdvertencia(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  ⚠ " + mensaje);
            Console.ResetColor();
        }

        public static void Pausar()
        {
            Console.WriteLine();
            Console.Write("  Presione Enter para continuar...");
            Console.ReadLine();
        }

        // ============================================================
        // CÓDIGO MUERTO - Estas funciones no se usan en ningún lugar
        // ============================================================

        // Nunca se llama desde ningún lugar del programa
        public static string EncriptarTexto(string texto)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(texto);
            return Convert.ToBase64String(bytes);
        }

        // Nunca se llama desde ningún lugar del programa
        public static string DesencriptarTexto(string textoEncriptado)
        {
            byte[] bytes = Convert.FromBase64String(textoEncriptado);
            return Encoding.UTF8.GetString(bytes);
        }

        // Nunca se llama - además es Base64, NO encriptación real
        public static bool VerificarContrasena(string contrasena, string contrasenaEncriptada)
        {
            string encriptada = EncriptarTexto(contrasena);
            return encriptada == contrasenaEncriptada;
        }

        // Nunca se llama desde ningún lugar del programa
        public static void ExportarCSV(List<Producto> productos, string rutaArchivo)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Codigo,Nombre,PrecioCompra,PrecioVenta,Stock");
            foreach (var p in productos)
            {
                sb.AppendLine(p.Codigo + "," + p.Nombre + "," + p.PrecioCompra + "," + p.PrecioVenta + "," + p.Stock);
            }
            File.WriteAllText(rutaArchivo, sb.ToString());
        }

        // Nunca se llama desde ningún lugar del programa
        public static List<string> LeerCSV(string rutaArchivo)
        {
            List<string> lineas = new List<string>();
            if (File.Exists(rutaArchivo))
            {
                string[] contenido = File.ReadAllLines(rutaArchivo);
                foreach (string linea in contenido)
                {
                    lineas.Add(linea);
                }
            }
            return lineas;
        }

        // Nunca se llama desde ningún lugar del programa
        public static void EnviarCorreo(string destinatario, string asunto, string cuerpo)
        {
            // Implementación vacía / stub
            Console.WriteLine("Enviando correo a: " + destinatario);
            Console.WriteLine("Asunto: " + asunto);
            // TODO: Implementar envío real de correo
        }

        // Nunca se llama desde ningún lugar del programa
        public static string GenerarCodigoProducto(string prefijo)
        {
            Random rnd = new Random();
            int numero = rnd.Next(1, 9999);
            return prefijo + "-" + numero.ToString("D4");
        }

        // Nunca se llama - y tiene un bug: siempre genera el mismo "aleatorio"
        // porque crea un new Random() cada vez que se llama
        public static string GenerarPassword(int longitud)
        {
            Random rnd = new Random(42); // Semilla fija - siempre genera lo mismo
            string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < longitud; i++)
            {
                sb.Append(caracteres[rnd.Next(caracteres.Length)]);
            }
            return sb.ToString();
        }

        // ============================================================
        // LOGGING - Mezclado aquí cuando debería ser su propia clase
        // ============================================================

        private static string archivoLog = @"C:\Logs\inventario.log";

        public static void RegistrarLog(string mensaje)
        {
            try
            {
                string linea = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | " + mensaje;
                // No verifica si el directorio existe antes de escribir
                File.AppendAllText(archivoLog, linea + Environment.NewLine);
            }
            catch
            {
                // Silencia todos los errores - anti-patrón
            }
        }

        public static void RegistrarError(string error)
        {
            RegistrarLog("ERROR: " + error);
        }

        public static void RegistrarAcceso(string usuario, string accion)
        {
            RegistrarLog("ACCESO: " + usuario + " - " + accion);
        }

        // ============================================================
        // CÁLCULOS - Mezclados aquí con lógica de negocio
        // ============================================================

        public static decimal CalcularDescuento(decimal subtotal, int tipoCliente)
        {
            double descuento = 0;
            if (tipoCliente == 1) descuento = Configuracion.DescuentoRegular;
            else if (tipoCliente == 2) descuento = Configuracion.DescuentoMayorista;
            else if (tipoCliente == 3) descuento = Configuracion.DescuentoVIP;
            return subtotal * (decimal)descuento;
        }

        public static decimal CalcularImpuesto(decimal subtotal)
        {
            return subtotal * (decimal)Configuracion.ImpuestoVenta;
        }

        public static decimal CalcularTotal(decimal subtotal, decimal descuento, decimal impuesto)
        {
            return subtotal - descuento + impuesto;
        }

        // Margen de ganancia
        public static decimal CalcularMargen(decimal precioCompra, decimal precioVenta)
        {
            if (precioCompra == 0) return 0;
            return ((precioVenta - precioCompra) / precioCompra) * 100;
        }
    }
}
