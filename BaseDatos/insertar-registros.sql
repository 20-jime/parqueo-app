-- =====================================================
-- Datos de ejemplo: Parqueo de automóviles
-- Motor: PostgreSQL
-- Ejecutar DESPUÉS de diseno-bd.sql
-- =====================================================

INSERT INTO prq_automoviles (color, anio, fabricante, tipo) VALUES
('Rojo',   2020, 'Toyota',  'Sedan'),
('Negro',  2018, 'Nissan',  '4x4'),
('Blanco', 2022, 'Honda',   'Moto'),
('Azul',   2019, 'Hyundai', 'Sedan'),
('Gris',   2021, 'Suzuki',  '4x4');

INSERT INTO prq_parqueo (nombre_provincia, nombre, precio_por_hora) VALUES
('San José', 'Parqueo Central San José', 1500.00),
('Heredia',  'Parqueo Plaza Heredia',    1200.00);

INSERT INTO prq_ingreso_automoviles (id_parqueo, id_automovil, fecha_hora_entrada, fecha_hora_salida) VALUES
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