USE BD_PRUEBA;
GO


/* ===========================================================
   1. DATOS DE PRUEBA
   =========================================================== */
INSERT INTO Productos (Nombre, Precio, Stock)
VALUES ('Laptop Dell Vostro', 4800.00, 10),
       ('Mouse KlipXtreme', 150.00, 50),
       ('Teclado Microsoft', 130.00, 30);

INSERT INTO Clientes (Nombre, Email)
VALUES ('Derck Cuyan', 'dereck@mail.com'),
       ('María Gómez', 'maria@mail.com');

GO

/* ===========================================================
   2. CREAR TIPO TABLA
   =========================================================== */
CREATE TYPE dbo.DetalleVentaType AS TABLE
(
    ProductoId INT,
    Cantidad INT,
    PrecioUnitario DECIMAL(10,2)
);

GO

/* ===========================================================
   3. PROCEDIMIENTO: REGISTRAR VENTA
   =========================================================== */
CREATE PROCEDURE sp_RegistrarVenta
    @ClienteId INT,
    @DetallesVenta AS dbo.DetalleVentaType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Insertar cabecera de la venta
        DECLARE @VentaId INT;
        INSERT INTO Ventas (Fecha, ClienteId)
        VALUES (GETDATE(), @ClienteId);
        SET @VentaId = SCOPE_IDENTITY();

        -- Variables de detalle
        DECLARE @ProductoId INT, @Cantidad INT, @PrecioUnitario DECIMAL(10,2);

        DECLARE cur CURSOR FOR
        SELECT ProductoId, Cantidad, PrecioUnitario FROM @DetallesVenta;

        OPEN cur;
        FETCH NEXT FROM cur INTO @ProductoId, @Cantidad, @PrecioUnitario;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Validar stock
            IF (SELECT Stock FROM Productos WHERE Id = @ProductoId) < @Cantidad
            BEGIN
                RAISERROR('Stock insuficiente para el producto %d', 16, 1, @ProductoId);
                ROLLBACK TRANSACTION;
                RETURN;
            END

            -- Insertar detalle con Subtotal calculado
            INSERT INTO DetalleVentas (VentaId, ProductoId, Cantidad, PrecioUnitario, Subtotal)
            VALUES (@VentaId, @ProductoId, @Cantidad, @PrecioUnitario, @Cantidad * @PrecioUnitario);

            -- Actualizar stock
            UPDATE Productos
            SET Stock = Stock - @Cantidad
            WHERE Id = @ProductoId;

            FETCH NEXT FROM cur INTO @ProductoId, @Cantidad, @PrecioUnitario;
        END

        CLOSE cur;
        DEALLOCATE cur;

        COMMIT TRANSACTION;
        PRINT 'Venta registrada correctamente.';

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT 'Error al registrar la venta: ' + ERROR_MESSAGE();
    END CATCH
END
GO

/* ===========================================================
   4. PROCEDIMIENTO: OBTENER VENTAS POR CLIENTE
   =========================================================== */
CREATE PROCEDURE sp_GetVentasPorCliente
    @ClienteId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        V.Id AS VentaId,
        V.Fecha,
        C.Nombre AS Cliente,
        P.Nombre AS Producto,
        DV.Cantidad,
        DV.PrecioUnitario,
        DV.Subtotal
    FROM Ventas V
    INNER JOIN Clientes C ON V.ClienteId = C.Id
    INNER JOIN DetalleVentas DV ON V.Id = DV.VentaId
    INNER JOIN Productos P ON DV.ProductoId = P.Id
    WHERE V.ClienteId = @ClienteId
    ORDER BY V.Fecha DESC;
END
GO

GO

/* ===========================================================
   5. VENTAS DE PRUEBA
   =========================================================== */

DECLARE @Detalles1 AS dbo.DetalleVentaType;
INSERT INTO @Detalles1 (ProductoId, Cantidad, PrecioUnitario)
VALUES (1, 1, 4800.00), (2, 2, 150.00);
EXEC sp_RegistrarVenta @ClienteId = 1, @DetallesVenta = @Detalles1;

DECLARE @Detalles2 AS dbo.DetalleVentaType;
INSERT INTO @Detalles2 (ProductoId, Cantidad, PrecioUnitario)
VALUES (3, 2, 130.00);
EXEC sp_RegistrarVenta @ClienteId = 2, @DetallesVenta = @Detalles2;


DECLARE @Detalles3 AS dbo.DetalleVentaType;
INSERT INTO @Detalles3 (ProductoId, Cantidad, PrecioUnitario)
VALUES (1, 1, 4800.00), (3, 1, 130.00);
EXEC sp_RegistrarVenta @ClienteId = 1, @DetallesVenta = @Detalles3;


DECLARE @Detalles4 AS dbo.DetalleVentaType;
INSERT INTO @Detalles4 (ProductoId, Cantidad, PrecioUnitario)
VALUES (2, 3, 150.00);
EXEC sp_RegistrarVenta @ClienteId = 2, @DetallesVenta = @Detalles4;

DECLARE @Detalles5 AS dbo.DetalleVentaType;
INSERT INTO @Detalles5 (ProductoId, Cantidad, PrecioUnitario)
VALUES (1, 2, 4800.00), (3, 1, 130.00);
EXEC sp_RegistrarVenta @ClienteId = 1, @DetallesVenta = @Detalles5;


GO

/* ===========================================================
   6. CONSULTAS DE VALIDACIÓN
   =========================================================== */
   
-- Historiales 
EXEC sp_GetVentasPorCliente @ClienteId = 1;

EXEC sp_GetVentasPorCliente @ClienteId = 2;

-- Stock actual
SELECT * FROM Productos;

-- Totales por cliente
SELECT C.Nombre, SUM(D.Subtotal) AS TotalVendido
FROM Ventas V
JOIN Clientes C ON V.ClienteId = C.Id
JOIN DetalleVentas D ON V.Id = D.VentaId
GROUP BY C.Nombre;

-- Productos más vendidos
SELECT P.Nombre, SUM(D.Cantidad) AS TotalVendido
FROM DetalleVentas D
JOIN Productos P ON D.ProductoId = P.Id
GROUP BY P.Nombre
ORDER BY TotalVendido DESC;
GO
