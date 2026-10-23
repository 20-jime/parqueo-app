-- =====================================================
-- Diseño de base de datos: Parqueo de automóviles
-- Motor: SQL Server
-- Todas las tablas llevan el prefijo PRQ
-- =====================================================

-- Tabla de automóviles
CREATE TABLE PRQ_Automoviles (
    IdAutomovil INT IDENTITY(1,1) NOT NULL,
    Color       NVARCHAR(30)  NOT NULL,
    Anio        INT           NOT NULL,
    Fabricante  NVARCHAR(50)  NOT NULL,
    Tipo        NVARCHAR(10)  NOT NULL,
    CONSTRAINT PK_PRQ_Automoviles PRIMARY KEY (IdAutomovil),
    CONSTRAINT CK_PRQ_Automoviles_Anio CHECK (Anio BETWEEN 1900 AND 2100),
    CONSTRAINT CK_PRQ_Automoviles_Tipo CHECK (Tipo IN ('Sedan', '4x4', 'Moto'))
);

-- Tabla de parqueos
CREATE TABLE PRQ_Parqueo (
    IdParqueo       INT IDENTITY(1,1) NOT NULL,
    NombreProvincia NVARCHAR(50)  NOT NULL,
    Nombre          NVARCHAR(100) NOT NULL,
    PrecioPorHora   DECIMAL(10,2) NOT NULL,
    CONSTRAINT PK_PRQ_Parqueo PRIMARY KEY (IdParqueo),
    CONSTRAINT CK_PRQ_Parqueo_Precio CHECK (PrecioPorHora >= 0)
);

-- Tabla de ingreso de automóviles al parqueo
CREATE TABLE PRQ_IngresoAutomoviles (
    Consecutivo      INT IDENTITY(1,1) NOT NULL,
    IdParqueo        INT          NOT NULL,
    IdAutomovil      INT          NOT NULL,
    FechaHoraEntrada DATETIME2    NOT NULL,
    FechaHoraSalida  DATETIME2    NULL,  -- NULL = el automóvil aún no ha salido
    CONSTRAINT PK_PRQ_IngresoAutomoviles PRIMARY KEY (Consecutivo),
    CONSTRAINT FK_PRQ_Ingreso_Parqueo
        FOREIGN KEY (IdParqueo) REFERENCES PRQ_Parqueo (IdParqueo),
    CONSTRAINT FK_PRQ_Ingreso_Automovil
        FOREIGN KEY (IdAutomovil) REFERENCES PRQ_Automoviles (IdAutomovil),
    CONSTRAINT CK_PRQ_Ingreso_Fechas
        CHECK (FechaHoraSalida IS NULL OR FechaHoraSalida >= FechaHoraEntrada)
);