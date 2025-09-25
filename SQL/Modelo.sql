USE BD_PRUEBA;
GO

/* ===========================================================
   1. CREACIÓN DE TABLAS
   =========================================================== */

CREATE TABLE Productos (
    Id INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL
);

CREATE TABLE Clientes (
    Id INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL
);

CREATE TABLE Ventas (
    Id INT IDENTITY PRIMARY KEY,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    ClienteId INT NOT NULL FOREIGN KEY REFERENCES Clientes(Id)
);

CREATE TABLE DetalleVentas (
    Id INT IDENTITY PRIMARY KEY,
    VentaId INT NOT NULL FOREIGN KEY REFERENCES Ventas(Id),
    ProductoId INT NOT NULL FOREIGN KEY REFERENCES Productos(Id),
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL
);
GO