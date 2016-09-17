CREATE TABLE [dbo].[Job] (
    [JobID]   INT          IDENTITY (1, 1) NOT NULL,
    [JobName] VARCHAR (50) NULL,
    [LastRun] DATETIME     NULL,
    [NextRun] DATETIME     NULL,
    PRIMARY KEY CLUSTERED ([JobID] ASC)
);



