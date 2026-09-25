BEGIN;

INSERT INTO "Client" ("Name") VALUES
    ('Иванов Иван'),
    ('Петров Пётр'),
    ('Сидорова Анна');

INSERT INTO "Equipment" ("SerialEquipment", "TypeEquipment") VALUES
    ('SN-10001', 'Принтер'),
    ('SN-10002', 'Ноутбук'),
    ('SN-10003', 'Монитор');

INSERT INTO "User" ("Login", "Password", "Phone", "UserRole") VALUES
    ('admin', 'admin', '79000000001', 0),
    ('manager', 'manager', '79000000002', 1),
    ('spec1', 'spec1', '79000000003', 2),
    ('spec2', 'spec2', '79000000004', 2);

INSERT INTO "Tasks" (
    "Priority", "Status", "SpecialistReport", "ClientComment",
    "ClientId", "EquipmentId", "EquipmentProblem", "EquipmentTypeOfProblem",
    "DateOfRegistration", "DateOfStartWork", "DateOfClose", "WorkHours"
) VALUES
    ('Высокий', 0, NULL, 'Не печатает',
        (SELECT "Id" FROM "Client" WHERE "Name" = 'Иванов Иван'),
        (SELECT "Id" FROM "Equipment" WHERE "SerialEquipment" = 'SN-10001'),
        'Не захватывает бумагу', 'Механика',
        TIMESTAMPTZ '2026-09-20 09:00:00+00', NULL, NULL, NULL),
    ('Средний', 1, NULL, 'Греется',
        (SELECT "Id" FROM "Client" WHERE "Name" = 'Петров Пётр'),
        (SELECT "Id" FROM "Equipment" WHERE "SerialEquipment" = 'SN-10002'),
        'Выключается под нагрузкой', 'Питание',
        TIMESTAMPTZ '2026-09-18 08:00:00+00', TIMESTAMPTZ '2026-09-19 10:00:00+00', NULL, 3),
    ('Низкий', 3, 'Заменён кабель', 'Нет изображения',
        (SELECT "Id" FROM "Client" WHERE "Name" = 'Сидорова Анна'),
        (SELECT "Id" FROM "Equipment" WHERE "SerialEquipment" = 'SN-10003'),
        'Нет сигнала', 'Кабель',
        TIMESTAMPTZ '2026-09-10 07:00:00+00', TIMESTAMPTZ '2026-09-11 09:00:00+00', TIMESTAMPTZ '2026-09-12 15:00:00+00', 5);

INSERT INTO "RepairTaskUser" ("SpecialistsId", "TasksId") VALUES
    ((SELECT "Id" FROM "User" WHERE "Login" = 'spec1'),
     (SELECT "Id" FROM "Tasks" WHERE "EquipmentProblem" = 'Выключается под нагрузкой')),
    ((SELECT "Id" FROM "User" WHERE "Login" = 'spec1'),
     (SELECT "Id" FROM "Tasks" WHERE "EquipmentProblem" = 'Нет сигнала'));

INSERT INTO "TaskEquipment" ("TaskId", "EquipmentId", "Problem", "TypeOfProblem") VALUES
    ((SELECT "Id" FROM "Tasks" WHERE "EquipmentProblem" = 'Нет сигнала'),
     (SELECT "Id" FROM "Equipment" WHERE "SerialEquipment" = 'SN-10002'),
     'Нет изображения на втором экране', 'Кабель');

INSERT INTO "OrderSpare" ("NameSpare", "ExpectedCost", "TotalCost", "TaskId", "DateOfOrder", "DateOfGetting") VALUES
    ('Кабель HDMI', 500, 450,
        (SELECT "Id" FROM "Tasks" WHERE "EquipmentProblem" = 'Нет сигнала'),
        TIMESTAMPTZ '2026-09-11 09:30:00+00', TIMESTAMPTZ '2026-09-12 11:00:00+00');

COMMIT;