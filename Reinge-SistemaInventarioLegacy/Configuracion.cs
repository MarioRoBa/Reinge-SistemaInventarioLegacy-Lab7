using System;

namespace SistemaInventarioLegacy
{
    public class Configuracion
    {
        // NOTA: Cambiar estos valores según su entorno
        public static string CadenaConexion =
    Environment.GetEnvironmentVariable("INVENTARIO_CONNECTION_STRING")
    ?? "Server=localhost;Database=InventarioLegacyDB;Trusted_Connection=True;TrustServerCertificate=True;";
        
        public static string NombreEmpresa = "Distribuidora XYZ S.A.";
        public static string VersionSistema = "1.0.3";
        public static double ImpuestoVenta = 0.13; // 13% IVA
        public static int MaximoIntentos = 3;
        public static string RutaReportes = @"C:\Reportes\Inventario\";
        public static string EmailAdmin = "admin@distribuidoraxyz.com";
        public static string ServidorSMTP = "smtp.distribuidoraxyz.com";
        public static string PasswordEmail = "correo2023!";
        
        // Descuentos por tipo de cliente
        public static double DescuentoRegular = 0.0;
        public static double DescuentoMayorista = 0.10;
        public static double DescuentoVIP = 0.15;
        
        public static bool ModoDebug = true;
        
        public static void MostrarConfiguracion()
        {
            Console.WriteLine("=== Configuración del Sistema ===");
            Console.WriteLine("Empresa: " + NombreEmpresa);
            Console.WriteLine("Versión: " + VersionSistema);
            Console.WriteLine("Base de datos: " + CadenaConexion);
            Console.WriteLine("Impuesto: " + (ImpuestoVenta * 100) + "%");
            Console.WriteLine("Email admin: " + EmailAdmin);
            Console.WriteLine("Modo debug: " + (ModoDebug ? "Activado" : "Desactivado"));
            Console.WriteLine("================================");
        }
    }
}
