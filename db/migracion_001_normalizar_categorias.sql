USE InventarioLegacyDB;
GO

SET XACT_ABORT ON;
BEGIN TRAN;

IF OBJECT_ID('dbo.__MigracionesLegacy','U') IS NULL
BEGIN
    CREATE TABLE dbo.__MigracionesLegacy (
        Nombre VARCHAR(150) PRIMARY KEY,
        FechaEjecucion DATETIME NOT NULL DEFAULT GETDATE(),
        CreoCategorias BIT NOT NULL DEFAULT 0,
        CreoCategoriaId BIT NOT NULL DEFAULT 0,
        CreoFK BIT NOT NULL DEFAULT 0
    );
END;

DECLARE @CreoCategorias BIT = 0;
DECLARE @CreoCategoriaId BIT = 0;
DECLARE @CreoFK BIT = 0;

IF OBJECT_ID('dbo.Categorias','U') IS NULL
BEGIN
    CREATE TABLE dbo.Categorias (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre VARCHAR(100) NOT NULL UNIQUE,
        Descripcion VARCHAR(500) NULL,
        Activa BIT DEFAULT 1,
        FechaCreacion DATETIME DEFAULT GETDATE()
    );

    SET @CreoCategorias = 1;
END;

IF COL_LENGTH('dbo.Productos','CategoriaId') IS NULL
BEGIN
    ALTER TABLE dbo.Productos ADD CategoriaId INT NULL;
    SET @CreoCategoriaId = 1;
END;

IF COL_LENGTH('dbo.Productos','Categoria') IS NOT NULL
BEGIN
    EXEC('
        INSERT INTO dbo.Categorias (Nombre)
        SELECT DISTINCT LTRIM(RTRIM(Categoria))
        FROM dbo.Productos
        WHERE Categoria IS NOT NULL
          AND LTRIM(RTRIM(Categoria)) <> ''''
          AND NOT EXISTS (
              SELECT 1
              FROM dbo.Categorias c
              WHERE c.Nombre = LTRIM(RTRIM(Productos.Categoria))
          );

        UPDATE p
        SET p.CategoriaId = c.Id
        FROM dbo.Productos p
        JOIN dbo.Categorias c
          ON c.Nombre = LTRIM(RTRIM(p.Categoria))
        WHERE p.CategoriaId IS NULL;
    ');
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_key_columns fkc
    JOIN sys.columns pc
      ON pc.object_id = fkc.parent_object_id
     AND pc.column_id = fkc.parent_column_id
    WHERE fkc.parent_object_id = OBJECT_ID('dbo.Productos')
      AND fkc.referenced_object_id = OBJECT_ID('dbo.Categorias')
      AND pc.name = 'CategoriaId'
)
BEGIN
    ALTER TABLE dbo.Productos
    ADD CONSTRAINT FK_Productos_Categorias
    FOREIGN KEY (CategoriaId) REFERENCES dbo.Categorias(Id);

    SET @CreoFK = 1;
END;

IF COL_LENGTH('dbo.Productos','Categoria') IS NOT NULL
BEGIN
    EXEC('
        CREATE OR ALTER VIEW dbo.vw_Productos AS
        SELECT
            p.Id,
            p.Codigo,
            p.Nombre,
            p.Descripcion,
            COALESCE(c.Nombre, p.Categoria) AS Categoria,
            p.PrecioCompra,
            p.PrecioVenta,
            p.Stock,
            p.StockMinimo,
            p.CategoriaId,
            p.Activo,
            p.FechaCreacion,
            p.UltimaModificacion
        FROM dbo.Productos p
        LEFT JOIN dbo.Categorias c ON c.Id = p.CategoriaId;
    ');
END
ELSE
BEGIN
    EXEC('
        CREATE OR ALTER VIEW dbo.vw_Productos AS
        SELECT
            p.Id,
            p.Codigo,
            p.Nombre,
            p.Descripcion,
            c.Nombre AS Categoria,
            p.PrecioCompra,
            p.PrecioVenta,
            p.Stock,
            p.StockMinimo,
            p.CategoriaId,
            p.Activo,
            p.FechaCreacion,
            p.UltimaModificacion
        FROM dbo.Productos p
        LEFT JOIN dbo.Categorias c ON c.Id = p.CategoriaId;
    ');
END;

MERGE dbo.__MigracionesLegacy AS target
USING (
    SELECT
        'migracion_001_normalizar_categorias' AS Nombre,
        @CreoCategorias AS CreoCategorias,
        @CreoCategoriaId AS CreoCategoriaId,
        @CreoFK AS CreoFK
) AS source
ON target.Nombre = source.Nombre
WHEN MATCHED THEN
    UPDATE SET
        FechaEjecucion = GETDATE(),
        CreoCategorias = source.CreoCategorias,
        CreoCategoriaId = source.CreoCategoriaId,
        CreoFK = source.CreoFK
WHEN NOT MATCHED THEN
    INSERT (Nombre, CreoCategorias, CreoCategoriaId, CreoFK)
    VALUES (source.Nombre, source.CreoCategorias, source.CreoCategoriaId, source.CreoFK);

COMMIT TRAN;
GO