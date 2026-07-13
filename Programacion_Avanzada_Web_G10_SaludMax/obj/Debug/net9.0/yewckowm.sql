IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [ServiciosMedicos] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [Descripcion] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_ServiciosMedicos] PRIMARY KEY ([Id])
);

CREATE TABLE [Usuarios] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [Correo] nvarchar(max) NOT NULL,
    [Contrasena] nvarchar(max) NOT NULL,
    [Rol] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id])
);

CREATE TABLE [Citas] (
    [Id] int NOT NULL IDENTITY,
    [Fecha] datetime2 NOT NULL,
    [Horario] nvarchar(max) NOT NULL,
    [Estado] int NOT NULL,
    [UsuarioId] int NOT NULL,
    [ServicioMedicoId] int NOT NULL,
    CONSTRAINT [PK_Citas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Citas_ServiciosMedicos_ServicioMedicoId] FOREIGN KEY ([ServicioMedicoId]) REFERENCES [ServiciosMedicos] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Citas_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_Citas_ServicioMedicoId] ON [Citas] ([ServicioMedicoId]);

CREATE INDEX [IX_Citas_UsuarioId] ON [Citas] ([UsuarioId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260712153908_InitialCreate', N'9.0.17');

DECLARE @var sysname;
SELECT @var = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'Rol');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT [' + @var + '];');
ALTER TABLE [Usuarios] DROP COLUMN [Rol];

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Citas]') AND [c].[name] = N'Horario');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Citas] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Citas] DROP COLUMN [Horario];

ALTER TABLE [Usuarios] ADD [RolId] int NOT NULL DEFAULT 0;

ALTER TABLE [Citas] ADD [HorarioId] int NOT NULL DEFAULT 0;

CREATE TABLE [Horarios] (
    [Id] int NOT NULL IDENTITY,
    [Hora] time NOT NULL,
    CONSTRAINT [PK_Horarios] PRIMARY KEY ([Id])
);

CREATE TABLE [Roles] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);

CREATE INDEX [IX_Usuarios_RolId] ON [Usuarios] ([RolId]);

CREATE INDEX [IX_Citas_HorarioId] ON [Citas] ([HorarioId]);

ALTER TABLE [Citas] ADD CONSTRAINT [FK_Citas_Horarios_HorarioId] FOREIGN KEY ([HorarioId]) REFERENCES [Horarios] ([Id]) ON DELETE CASCADE;

ALTER TABLE [Usuarios] ADD CONSTRAINT [FK_Usuarios_Roles_RolId] FOREIGN KEY ([RolId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260713005759_AgregarRolYHorario', N'9.0.17');

COMMIT;
GO

