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

COMMIT;
GO

