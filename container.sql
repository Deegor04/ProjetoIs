CREATE TABLE [dbo].[container] (
    [resource-name]             NVARCHAR (50) NOT NULL,
    [creation-datetime]         DATETIME      NOT NULL,
    [res-type]                  NVARCHAR (50) NOT NULL,
    [application-resource-name] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([resource-name] ASC),
    FOREIGN KEY ([application-resource-name]) REFERENCES [dbo].[application] ([resource-name]),
    CHECK ([res-type]='container')
);