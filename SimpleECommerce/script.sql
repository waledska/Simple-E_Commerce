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
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241103174532_createAuthTablesInSql')
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241103174532_createAuthTablesInSql')
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [TokenForRessetingPass] nvarchar(max) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241103174532_createAuthTablesInSql')
BEGIN
    CREATE TABLE [emailOtps] (
        [Id] int NOT NULL IDENTITY,
        [email] nvarchar(max) NOT NULL,
        [otp] nvarchar(max) NOT NULL,
        [validTo] datetime2 NOT NULL,
        CONSTRAINT [PK_emailOtps] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241103174532_createAuthTablesInSql')
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241103174532_createAuthTablesInSql')
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241103174532_createAuthTablesInSql')
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241103174532_createAuthTablesInSql')
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241103174532_createAuthTablesInSql')
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241103174532_createAuthTablesInSql')
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241103174532_createAuthTablesInSql')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241103174532_createAuthTablesInSql')
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241103174532_createAuthTablesInSql')
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241103174532_createAuthTablesInSql')
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241103174532_createAuthTablesInSql')
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241103174532_createAuthTablesInSql')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241103174532_createAuthTablesInSql')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241103174532_createAuthTablesInSql', N'6.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241109005707_test')
BEGIN
    CREATE TABLE [Address] (
        [id] int NOT NULL IDENTITY,
        [country] nvarchar(100) NOT NULL,
        [fullName] nvarchar(255) NOT NULL,
        [mobileNumber] nvarchar(20) NOT NULL,
        [streetName] nvarchar(255) NOT NULL,
        [buildingName] nvarchar(255) NOT NULL,
        [city] nvarchar(255) NULL,
        [userId] nvarchar(max) NOT NULL,
        [isDefault] bit NOT NULL,
        CONSTRAINT [PK_Address] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241109005707_test')
BEGIN
    CREATE TABLE [Cart] (
        [id] int NOT NULL IDENTITY,
        [userId] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Cart] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241109005707_test')
BEGIN
    CREATE TABLE [Color] (
        [id] int NOT NULL IDENTITY,
        [value] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Color] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241109005707_test')
BEGIN
    CREATE TABLE [Product] (
        [id] int NOT NULL IDENTITY,
        [price] decimal(10,2) NOT NULL,
        [name] nvarchar(255) NOT NULL,
        [description] nvarchar(max) NULL,
        CONSTRAINT [PK_Product] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241109005707_test')
BEGIN
    CREATE TABLE [Size] (
        [id] int NOT NULL IDENTITY,
        [value] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Size] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241109005707_test')
BEGIN
    CREATE TABLE [Orders] (
        [id] int NOT NULL IDENTITY,
        [userId] nvarchar(max) NULL,
        [orderStatus] nvarchar(50) NOT NULL,
        [totalAmount] decimal(10,2) NOT NULL,
        [dateOfOrder] datetime NOT NULL,
        [addressId] int NOT NULL,
        CONSTRAINT [PK_Orders] PRIMARY KEY ([id]),
        CONSTRAINT [FK__Orders__addressI__49C3F6B7] FOREIGN KEY ([addressId]) REFERENCES [Address] ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241109005707_test')
BEGIN
    CREATE TABLE [ProductVariation] (
        [id] int NOT NULL IDENTITY,
        [productId] int NOT NULL,
        [colorId] int NOT NULL,
        [sizeId] int NOT NULL,
        [SKU] nvarchar(100) NULL,
        [quantityInStock] int NOT NULL,
        CONSTRAINT [PK_ProductVariation] PRIMARY KEY ([id]),
        CONSTRAINT [FK__ProductVa__color__3E52440B] FOREIGN KEY ([colorId]) REFERENCES [Color] ([id]),
        CONSTRAINT [FK__ProductVa__produ__3D5E1FD2] FOREIGN KEY ([productId]) REFERENCES [Product] ([id]),
        CONSTRAINT [FK__ProductVa__sizeI__3F466844] FOREIGN KEY ([sizeId]) REFERENCES [Size] ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241109005707_test')
BEGIN
    CREATE TABLE [CartRow] (
        [id] int NOT NULL IDENTITY,
        [cartId] int NOT NULL,
        [productVariationId] int NOT NULL,
        [quantity] int NOT NULL,
        CONSTRAINT [PK_CartRow] PRIMARY KEY ([id]),
        CONSTRAINT [FK__CartRow__cartId__45F365D3] FOREIGN KEY ([cartId]) REFERENCES [Cart] ([id]),
        CONSTRAINT [FK__CartRow__product__46E78A0C] FOREIGN KEY ([productVariationId]) REFERENCES [ProductVariation] ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241109005707_test')
BEGIN
    CREATE TABLE [OrderRow] (
        [id] int NOT NULL IDENTITY,
        [orderId] int NOT NULL,
        [productVariationId] int NOT NULL,
        [quantity] int NOT NULL,
        [priceForProduct] decimal(10,2) NOT NULL,
        CONSTRAINT [PK_OrderRow] PRIMARY KEY ([id]),
        CONSTRAINT [FK__OrderRow__orderI__4CA06362] FOREIGN KEY ([orderId]) REFERENCES [Orders] ([id]),
        CONSTRAINT [FK__OrderRow__produc__4D94879B] FOREIGN KEY ([productVariationId]) REFERENCES [ProductVariation] ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241109005707_test')
BEGIN
    CREATE INDEX [IX_CartRow_cartId] ON [CartRow] ([cartId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241109005707_test')
BEGIN
    CREATE INDEX [IX_CartRow_productVariationId] ON [CartRow] ([productVariationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241109005707_test')
BEGIN
    CREATE INDEX [IX_OrderRow_orderId] ON [OrderRow] ([orderId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241109005707_test')
BEGIN
    CREATE INDEX [IX_OrderRow_productVariationId] ON [OrderRow] ([productVariationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241109005707_test')
BEGIN
    CREATE INDEX [IX_Orders_addressId] ON [Orders] ([addressId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241109005707_test')
BEGIN
    CREATE INDEX [IX_ProductVariation_colorId] ON [ProductVariation] ([colorId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241109005707_test')
BEGIN
    CREATE INDEX [IX_ProductVariation_productId] ON [ProductVariation] ([productId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241109005707_test')
BEGIN
    CREATE INDEX [IX_ProductVariation_sizeId] ON [ProductVariation] ([sizeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241109005707_test')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241109005707_test', N'6.0.0');
END;
GO

COMMIT;
GO

