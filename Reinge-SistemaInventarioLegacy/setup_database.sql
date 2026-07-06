-- =============================================
-- SistemaInventarioLegacy - Setup de Base de Datos
-- Ejecutar este script completo en SQL Server
-- =============================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'InventarioLegacyDB')
BEGIN
    ALTER DATABASE InventarioLegacyDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE InventarioLegacyDB;
END
GO

CREATE DATABASE InventarioLegacyDB;
GO

USE InventarioLegacyDB;
GO

-- =============================================
-- TABLAS
-- =============================================

CREATE TABLE Categorias (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(500),
    Activa BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE()
);

CREATE TABLE Productos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Codigo VARCHAR(20) NOT NULL,
    Nombre VARCHAR(200) NOT NULL,
    Descripcion VARCHAR(1000),
    PrecioCompra DECIMAL(18,2) NOT NULL,
    PrecioVenta DECIMAL(18,2) NOT NULL,
    Stock INT NOT NULL DEFAULT 0,
    StockMinimo INT NOT NULL DEFAULT 5,
    CategoriaId INT FOREIGN KEY REFERENCES Categorias(Id),
    Activo BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    UltimaModificacion DATETIME DEFAULT GETDATE()
);

CREATE TABLE Clientes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Email VARCHAR(200),
    Telefono VARCHAR(20),
    Direccion VARCHAR(500),
    TipoCliente INT DEFAULT 1, -- 1=Regular, 2=Mayorista, 3=VIP
    Activo BIT DEFAULT 1,
    FechaRegistro DATETIME DEFAULT GETDATE()
);

CREATE TABLE Pedidos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClienteId INT FOREIGN KEY REFERENCES Clientes(Id),
    FechaPedido DATETIME DEFAULT GETDATE(),
    Estado INT DEFAULT 1, -- 1=Pendiente, 2=Procesando, 3=Enviado, 4=Entregado, 5=Cancelado
    SubTotal DECIMAL(18,2),
    Impuesto DECIMAL(18,2),
    Total DECIMAL(18,2),
    Direccion VARCHAR(500),
    Notas VARCHAR(1000),
    FechaEnvio DATETIME,
    FechaEntrega DATETIME
);

CREATE TABLE DetallePedidos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PedidoId INT FOREIGN KEY REFERENCES Pedidos(Id),
    ProductoId INT FOREIGN KEY REFERENCES Productos(Id),
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL,
    Descuento DECIMAL(5,2) DEFAULT 0,
    Subtotal DECIMAL(18,2) NOT NULL
);

CREATE TABLE Proveedores (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(200) NOT NULL,
    Contacto VARCHAR(100),
    Email VARCHAR(200),
    Telefono VARCHAR(20),
    Direccion VARCHAR(500),
    Activo BIT DEFAULT 1
);

CREATE TABLE MovimientosInventario (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductoId INT FOREIGN KEY REFERENCES Productos(Id),
    TipoMovimiento INT NOT NULL, -- 1=Entrada, 2=Salida, 3=Ajuste
    Cantidad INT NOT NULL,
    Motivo VARCHAR(500),
    UsuarioId INT,
    Fecha DATETIME DEFAULT GETDATE()
);

CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario VARCHAR(50) NOT NULL,
    Contrasena VARCHAR(100) NOT NULL, -- Almacenada en texto plano (problema intencional)
    NombreCompleto VARCHAR(200),
    Rol INT DEFAULT 1, -- 1=Operador, 2=Supervisor, 3=Admin
    Activo BIT DEFAULT 1,
    UltimoAcceso DATETIME
);

-- =============================================
-- DATOS DE EJEMPLO
-- =============================================

INSERT INTO Categorias (Nombre, Descripcion) VALUES
('Electrónica', 'Productos electrónicos y tecnología'),
('Oficina', 'Suministros y equipos de oficina'),
('Limpieza', 'Productos de limpieza e higiene'),
('Alimentos', 'Productos alimenticios no perecederos'),
('Herramientas', 'Herramientas manuales y eléctricas');

INSERT INTO Productos (Codigo, Nombre, Descripcion, PrecioCompra, PrecioVenta, Stock, StockMinimo, CategoriaId) VALUES
('ELEC-001', 'Monitor LED 24"', 'Monitor LED Full HD 24 pulgadas', 150.00, 225.00, 45, 10, 1),
('ELEC-002', 'Teclado Mecánico', 'Teclado mecánico retroiluminado', 35.00, 55.00, 120, 20, 1),
('ELEC-003', 'Mouse Inalámbrico', 'Mouse óptico inalámbrico', 12.00, 22.00, 200, 30, 1),
('ELEC-004', 'Cable HDMI 2m', 'Cable HDMI 2.0 de 2 metros', 5.00, 12.00, 500, 50, 1),
('ELEC-005', 'Hub USB 4 puertos', 'Hub USB 3.0 de 4 puertos', 8.00, 18.00, 80, 15, 1),
('OFIC-001', 'Resma Papel A4', 'Papel bond A4 500 hojas', 3.50, 6.00, 300, 50, 2),
('OFIC-002', 'Carpeta Manila', 'Carpeta manila tamaño carta', 0.25, 0.50, 1000, 100, 2),
('OFIC-003', 'Lapicero Azul', 'Lapicero tinta azul punta media', 0.30, 0.75, 2000, 200, 2),
('OFIC-004', 'Engrapadora', 'Engrapadora metálica estándar', 4.00, 8.50, 60, 10, 2),
('OFIC-005', 'Cinta Adhesiva', 'Cinta adhesiva transparente 48mm', 1.00, 2.50, 400, 50, 2),
('LIMP-001', 'Desinfectante 1L', 'Desinfectante multiusos 1 litro', 2.00, 4.50, 150, 30, 3),
('LIMP-002', 'Jabón Líquido 500ml', 'Jabón líquido antibacterial', 1.50, 3.50, 200, 40, 3),
('LIMP-003', 'Escoba Industrial', 'Escoba para uso industrial', 5.00, 9.00, 40, 8, 3),
('ALIM-001', 'Café Molido 500g', 'Café molido premium 500 gramos', 6.00, 10.00, 100, 20, 4),
('ALIM-002', 'Azúcar 1kg', 'Azúcar blanca refinada 1 kilo', 1.00, 2.00, 250, 50, 4),
('HERR-001', 'Destornillador Phillips', 'Destornillador Phillips #2', 2.00, 4.50, 75, 15, 5),
('HERR-002', 'Martillo', 'Martillo de carpintero 16oz', 8.00, 15.00, 30, 5, 5),
('HERR-003', 'Cinta Métrica 5m', 'Cinta métrica retráctil 5 metros', 3.00, 7.00, 50, 10, 5);

INSERT INTO Clientes (Nombre, Apellido, Email, Telefono, Direccion, TipoCliente) VALUES
('Carlos', 'Méndez', 'cmendez@email.com', '8888-1234', 'San José, Barrio Escalante', 1),
('María', 'Rodríguez', 'mrodriguez@empresa.com', '8888-5678', 'Heredia, San Francisco', 2),
('José', 'Fernández', 'jfernandez@corp.com', '8888-9012', 'Alajuela, Centro', 3),
('Ana', 'López', 'alopez@email.com', '8888-3456', 'Cartago, Paraíso', 1),
('Luis', 'Vargas', 'lvargas@mayorista.com', '8888-7890', 'San José, Pavas', 2),
('Patricia', 'Mora', 'pmora@email.com', '8888-2345', 'Heredia, Barva', 1),
('Roberto', 'Jiménez', 'rjimenez@vip.com', '8888-6789', 'Escazú, San Rafael', 3),
('Sofía', 'Castro', 'scastro@email.com', '8888-0123', 'San José, Tibás', 1);

INSERT INTO Proveedores (Nombre, Contacto, Email, Telefono, Direccion) VALUES
('TechDistribuidora S.A.', 'Pedro Arias', 'ventas@techdist.com', '2222-1111', 'San José, La Uruca'),
('Suministros del Valle', 'Laura Quesada', 'info@sumvalley.com', '2222-2222', 'Heredia, Industrial'),
('Importadora Global', 'Miguel Chen', 'mchen@impglobal.com', '2222-3333', 'Alajuela, Zona Franca');

INSERT INTO Usuarios (NombreUsuario, Contrasena, NombreCompleto, Rol) VALUES
('admin', 'admin123', 'Administrador del Sistema', 3),
('operador1', 'password', 'Juan Pérez', 1),
('supervisor1', '12345', 'María Solano', 2);

-- Insertar algunos pedidos de ejemplo
INSERT INTO Pedidos (ClienteId, Estado, SubTotal, Impuesto, Total, Direccion) VALUES
(1, 4, 280.00, 36.40, 316.40, 'San José, Barrio Escalante'),
(2, 4, 1200.00, 156.00, 1356.00, 'Heredia, San Francisco'),
(3, 3, 450.00, 58.50, 508.50, 'Alajuela, Centro'),
(1, 1, 55.00, 7.15, 62.15, 'San José, Barrio Escalante'),
(5, 2, 3000.00, 390.00, 3390.00, 'San José, Pavas');

INSERT INTO DetallePedidos (PedidoId, ProductoId, Cantidad, PrecioUnitario, Descuento, Subtotal) VALUES
(1, 1, 1, 225.00, 0, 225.00),
(1, 2, 1, 55.00, 0, 55.00),
(2, 1, 4, 225.00, 10, 810.00),
(2, 3, 10, 22.00, 5, 209.00),
(2, 4, 20, 12.00, 15, 204.00),
(3, 1, 2, 225.00, 0, 450.00),
(4, 2, 1, 55.00, 0, 55.00),
(5, 6, 500, 6.00, 0, 3000.00);

INSERT INTO MovimientosInventario (ProductoId, TipoMovimiento, Cantidad, Motivo) VALUES
(1, 1, 50, 'Compra inicial'),
(2, 1, 150, 'Compra inicial'),
(1, 2, 5, 'Venta pedido #1'),
(3, 1, 200, 'Reposición de stock');

GO
PRINT 'Base de datos InventarioLegacyDB creada exitosamente.';
PRINT 'Usuarios de prueba: admin/admin123, operador1/password, supervisor1/12345';
GO
