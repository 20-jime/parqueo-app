-- =====================================================
-- Datos de ejemplo: Parqueo de automóviles
-- Motor: SQL Server
-- Ejecutar DESPUÉS de diseno-bd.sql
-- =====================================================

-- 5 automóviles (IdAutomovil 1 a 5)
INSERT INTO PRQ_Automoviles (Color, Anio, Fabricante, Tipo) VALUES
('Rojo',   2020, 'Toyota',  'Sedan'),
('Negro',  2018, 'Nissan',  '4x4'),
('Blanco', 2022, 'Honda',   'Moto'),
('Azul',   2019, 'Hyundai', 'Sedan'),
('Gris',   2021, 'Suzuki',  '4x4');

-- 2 parqueos (IdParqueo 1 y 2)
INSERT INTO PRQ_Parqueo (NombreProvincia, Nombre, PrecioPorHora) VALUES
('San José', 'Parqueo Central San José', 1500.00),
('Heredia',  'Parqueo Plaza Heredia',    1200.00);

-- 15 ingresos (las salidas en NULL son automóviles que aún no han salido)
INSERT INTO PRQ_IngresoAutomoviles (IdParqueo, IdAutomovil, FechaHoraEntrada, FechaHoraSalida) VALUES
(1, 1, '2026-10-01 08:00:00', '2026-10-01 10:30:00'),
(1, 2, '2026-10-01 09:15:00', '2026-10-01 17:45:00'),
(2, 3, '2026-10-02 07:30:00', '2026-10-02 09:00:00'),
(2, 4, '2026-10-02 12:00:00', '2026-10-02 14:20:00'),
(1, 5, '2026-10-03 10:00:00', '2026-10-03 13:10:00'),
(1, 1, '2026-10-05 08:30:00', '2026-10-05 12:00:00'),
(2, 2, '2026-10-06 15:00:00', '2026-10-06 18:30:00'),
(1, 3, '2026-10-07 09:00:00', '2026-10-07 11:15:00'),
(2, 5, '2026-10-08 06:45:00', '2026-10-08 16:00:00'),
(1, 4, '2026-10-09 13:00:00', '2026-10-09 15:40:00'),
(2, 1, '2026-10-12 08:00:00', '2026-10-12 09:30:00'),
(1, 2, '2026-10-14 11:00:00', '2026-10-14 14:00:00'),
(2, 3, '2026-10-20 10:00:00', NULL),
(1, 4, '2026-10-21 14:30:00', NULL),
(2, 5, '2026-10-23 07:50:00', NULL); 