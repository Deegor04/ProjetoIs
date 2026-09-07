CREATE TABLE [dbo].[application] (
    [resource-name]     NVARCHAR (50) NOT NULL,
    [res-type]          NVARCHAR (50) NOT NULL,
    [creation-datetime] DATETIME      NOT NULL,
    PRIMARY KEY CLUSTERED ([resource-name] ASC),
    CHECK ([res-type]='application')
);