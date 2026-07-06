USE InventarioLegacyDB;
GO

SET XACT_ABORT ON;
BEGIN TRAN;

DECLARE @CreoCategorias BIT = 0;
DECLARE @CreoCategoriaId BIT = 0;
DECLARE @CreoFK BIT = 0;

IF OBJECT_ID('dbo.__MigracionesLegacy','U') IS NOT NULL
BEGIN
    SELECT
        @CreoCategorias = CreoCategorias,
        @CreoCategoriaId = CreoCategoriaId,
        @CreoFK = CreoFK
    FROM dbo.__MigracionesLegacy
    WHERE Nombre = 'migracion_001_normalizar_categorias';
END;

IF OBJECT_ID('dbo.vw_Productos','V') IS NOT NULL
    DROP VIEW dbo.vw_Productos;

IF @CreoFK = 1
   AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Productos_Categorias')
BEGIN
    ALTER TABLE dbo.Productos DROP CONSTRAINT FK_Productos_Categorias;
END;

IF @CreoCategoriaId = 1
   AND COL_LENGTH('dbo.Productos','CategoriaId') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Productos DROP COLUMN CategoriaId;
END;

IF @CreoCategorias = 1
   AND OBJECT_ID('dbo.Categorias','U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Categorias;
END;

IF OBJECT_ID('dbo.__MigracionesLegacy','U') IS NOT NULL
BEGIN
    DELETE FROM dbo.__MigracionesLegacy
    WHERE Nombre = 'migracion_001_normalizar_categorias';
END;

COMMIT TRAN;
GO