CREATE TABLE [dbo].[Restaurants](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](250) NULL
 CONSTRAINT [PK_Restaurants] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
 
GO

CREATE TABLE [dbo].[Tables](
    [Id] [int] IDENTITY(1,1) NOT NULL,
	[Number] varchar(10) NOT NULL,
	[State] varchar(10) NOT NULL,
	TableType [nvarchar] (50) NOT NULL,
	Capacity [int] NOT NULL,
	RestaurantId [int] NOT NULL,
	LightAddress [nvarchar] (200) NULL,
    XCoordinate [float]  NULL,
	YCoordinate [float]  NULL
	)
	GO
	ALTER TABLE [Tables] ADD
 CONSTRAINT [PK_Tables] PRIMARY KEY CLUSTERED ( [Id] ASC ),
CONSTRAINT [FK_Tables_Restaurants] FOREIGN KEY([RestaurantId]) REFERENCES Restaurants(Id)

alter table tables add PartySize int
alter table tables add SeatedTime datetime
alter table tables add AverageSeatDuration int
 
GO

create table [dbo].Waits(
	[Id] int identity(1,1) not null,
	[PartySize] int not null,
	[Created] DateTime not null,
	[Name] nvarchar(50) not null,
	[Phone] nvarchar(15) not null,
	[EstimatedWait] int not null,
	[RestaurantId] int not null,
	Code int not null
)
GO
	ALTER TABLE [Waits] ADD
 CONSTRAINT [PK_Waits] PRIMARY KEY CLUSTERED ( [Id] ASC ),
CONSTRAINT [FK_Waits_Restaurants] FOREIGN KEY([RestaurantId]) REFERENCES Restaurants(Id)

alter table waits add [State] varchar(10) not null
alter table waits add [NotifiedTime] DateTime null
alter table waits add TableId int null

GO

alter table restaurants add [ApiKey] varchar(50) not null default 'no key'
alter table tables add [Shape] varchar(20) null

insert into Restaurants ([Name]) values ('Red Robin')


CREATE TABLE [dbo].[Staff](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](250) NULL,
	[Ranking] int not null default 0,
	[RestaurantId] int null,
	[ShiftStart] DateTime null,
	[State] varchar(20) null,
	[DateCreated] DateTime not null default GETDATE())
GO
	ALTER TABLE [Staff] ADD
 CONSTRAINT [Pk_Staff] PRIMARY KEY CLUSTERED ( [Id] ASC ),
CONSTRAINT [FK_Staff_Restaurant] FOREIGN KEY([RestaurantId]) REFERENCES Restaurants(Id)
 
GO
alter table Staff add [Deleted] bit

CREATE TABLE [dbo].[Section](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [RestaurantId]  int null,
	[StaffCount] int not null default 0,
	[DateCreated] DateTime not null default GETDATE())
GO
	ALTER TABLE [Section] ADD
 CONSTRAINT [Pk_Section] PRIMARY KEY CLUSTERED ( [Id] ASC ),
CONSTRAINT [FK_Section_Restaurant] FOREIGN KEY([RestaurantId]) REFERENCES Restaurants(Id)
 

 CREATE TABLE [dbo].[SectionTables](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SectionId]  int null,
    [StaffIndex] int not null default 0,
	[TableId]  int null)
GO
	ALTER TABLE [SectionTables] ADD
 CONSTRAINT [Pk_SectionTables] PRIMARY KEY CLUSTERED ( [Id] ASC ),
CONSTRAINT [FK_SectionTables_SectionId] FOREIGN KEY([SectionId]) REFERENCES Section(Id),
CONSTRAINT [FK_SectionTables_Table] FOREIGN KEY([TableId]) REFERENCES [Tables](Id)
 