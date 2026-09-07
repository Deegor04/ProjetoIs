CREATE TABLE [dbo].[subscription] (
    [resource-name]           NVARCHAR (50)  NOT NULL,
    [creation-datetime]       DATETIME       NOT NULL,
    [container-resource-name] NVARCHAR (50)  NOT NULL,
    [res-type]                NVARCHAR (50)  NOT NULL,
    [evt]                     INT            NOT NULL,
    [endpoint]                NVARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([resource-name] ASC),
    FOREIGN KEY ([container-resource-name]) REFERENCES [dbo].[container] ([resource-name]),
    CHECK ([res-type]='subscription'),
    CHECK ([evt]=(2) OR [evt]=(1))
);