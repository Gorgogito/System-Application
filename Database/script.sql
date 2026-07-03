USE [master]
GO

/****** Object:  Database [BDAplication]    Script Date: 30/05/2026 15:37:35 ******/
CREATE DATABASE [BDAplication]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'BDAplication', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQL2022\MSSQL\DATA\BDAplication.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'BDAplication_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQL2022\MSSQL\DATA\BDAplication_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BDAplication].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [BDAplication] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [BDAplication] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [BDAplication] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [BDAplication] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [BDAplication] SET ARITHABORT OFF 
GO

ALTER DATABASE [BDAplication] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [BDAplication] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [BDAplication] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [BDAplication] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [BDAplication] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [BDAplication] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [BDAplication] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [BDAplication] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [BDAplication] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [BDAplication] SET  ENABLE_BROKER 
GO

ALTER DATABASE [BDAplication] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [BDAplication] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [BDAplication] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [BDAplication] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [BDAplication] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [BDAplication] SET READ_COMMITTED_SNAPSHOT ON 
GO

ALTER DATABASE [BDAplication] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [BDAplication] SET RECOVERY FULL 
GO

ALTER DATABASE [BDAplication] SET  MULTI_USER 
GO

ALTER DATABASE [BDAplication] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [BDAplication] SET DB_CHAINING OFF 
GO

ALTER DATABASE [BDAplication] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [BDAplication] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [BDAplication] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [BDAplication] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO

ALTER DATABASE [BDAplication] SET QUERY_STORE = ON
GO

ALTER DATABASE [BDAplication] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO

ALTER DATABASE [BDAplication] SET  READ_WRITE 
GO


USE [BDAplication]
GO
/****** Object:  Table [dbo].[account]    Script Date: 30/05/2026 15:36:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[account](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](12) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](300) NOT NULL,
	[Balance] [decimal](18, 2) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_account] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Backlogs]    Script Date: 30/05/2026 15:36:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Backlogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[Priority] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[UserCreated] [nvarchar](50) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Backlogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[movement]    Script Date: 30/05/2026 15:36:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[movement](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](12) NOT NULL,
	[AccountId] [int] NOT NULL,
	[Concept] [nvarchar](300) NOT NULL,
	[TypeConceptId] [int] NULL,
	[Date] [datetime2](7) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Type] [nvarchar](1) NOT NULL,
	[Balance] [decimal](18, 2) NOT NULL,
	[IsTransfer] [bit] NOT NULL,
	[TransferSourceDestiny] [nvarchar](1) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_movement] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Planners]    Script Date: 30/05/2026 15:36:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Planners](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BacklogId] [int] NOT NULL,
	[Day] [int] NOT NULL,
	[Month] [int] NOT NULL,
	[Year] [int] NOT NULL,
	[Notes] [nvarchar](500) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[UserCreated] [nvarchar](50) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Planners] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 30/05/2026 15:36:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](200) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubTask]    Script Date: 30/05/2026 15:36:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubTask](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BoardTaskId] [int] NOT NULL,
	[Title] [nvarchar](300) NOT NULL,
	[IsCompleted] [bit] NOT NULL,
	[UserCreated] [nvarchar](50) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_SubTask] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaskBoard]    Script Date: 30/05/2026 15:36:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskBoard](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](1000) NOT NULL,
	[Priority] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[UserCreated] [nvarchar](50) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateUpdated] [datetime2](7) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_TaskBoard] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[transfer]    Script Date: 30/05/2026 15:36:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[transfer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SourceAccountId] [int] NOT NULL,
	[SourceMovementId] [int] NOT NULL,
	[DestinyAccountId] [int] NOT NULL,
	[DestinyMovementId] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_transfer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[typeconcept]    Script Date: 30/05/2026 15:36:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[typeconcept](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](12) NOT NULL,
	[Description] [nvarchar](100) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_typeconcept] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 30/05/2026 15:36:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[PasswordHash] [nvarchar](max) NOT NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](150) NOT NULL,
	[RoleId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[movement]  WITH CHECK ADD  CONSTRAINT [FK_movement_account_AccountId] FOREIGN KEY([AccountId])
REFERENCES [dbo].[account] ([Id])
GO
ALTER TABLE [dbo].[movement] CHECK CONSTRAINT [FK_movement_account_AccountId]
GO
ALTER TABLE [dbo].[movement]  WITH CHECK ADD  CONSTRAINT [FK_movement_typeconcept_TypeConceptId] FOREIGN KEY([TypeConceptId])
REFERENCES [dbo].[typeconcept] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[movement] CHECK CONSTRAINT [FK_movement_typeconcept_TypeConceptId]
GO
ALTER TABLE [dbo].[Planners]  WITH CHECK ADD  CONSTRAINT [FK_Planners_Backlogs_BacklogId] FOREIGN KEY([BacklogId])
REFERENCES [dbo].[Backlogs] ([Id])
GO
ALTER TABLE [dbo].[Planners] CHECK CONSTRAINT [FK_Planners_Backlogs_BacklogId]
GO
ALTER TABLE [dbo].[SubTask]  WITH CHECK ADD  CONSTRAINT [FK_SubTask_TaskBoard_BoardTaskId] FOREIGN KEY([BoardTaskId])
REFERENCES [dbo].[TaskBoard] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SubTask] CHECK CONSTRAINT [FK_SubTask_TaskBoard_BoardTaskId]
GO
ALTER TABLE [dbo].[transfer]  WITH CHECK ADD  CONSTRAINT [FK_transfer_account_DestinyAccountId] FOREIGN KEY([DestinyAccountId])
REFERENCES [dbo].[account] ([Id])
GO
ALTER TABLE [dbo].[transfer] CHECK CONSTRAINT [FK_transfer_account_DestinyAccountId]
GO
ALTER TABLE [dbo].[transfer]  WITH CHECK ADD  CONSTRAINT [FK_transfer_account_SourceAccountId] FOREIGN KEY([SourceAccountId])
REFERENCES [dbo].[account] ([Id])
GO
ALTER TABLE [dbo].[transfer] CHECK CONSTRAINT [FK_transfer_account_SourceAccountId]
GO
ALTER TABLE [dbo].[transfer]  WITH CHECK ADD  CONSTRAINT [FK_transfer_movement_DestinyMovementId] FOREIGN KEY([DestinyMovementId])
REFERENCES [dbo].[movement] ([Id])
GO
ALTER TABLE [dbo].[transfer] CHECK CONSTRAINT [FK_transfer_movement_DestinyMovementId]
GO
ALTER TABLE [dbo].[transfer]  WITH CHECK ADD  CONSTRAINT [FK_transfer_movement_SourceMovementId] FOREIGN KEY([SourceMovementId])
REFERENCES [dbo].[movement] ([Id])
GO
ALTER TABLE [dbo].[transfer] CHECK CONSTRAINT [FK_transfer_movement_SourceMovementId]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Roles_RoleId]
GO
