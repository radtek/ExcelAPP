CREATE TABLE EACatCols
	(
	ID         VARCHAR (50) NOT NULL,
	RID        VARCHAR (50),
	FCode      VARCHAR (50),
	FName      VARCHAR (50),
	FType      VARCHAR (50),
	IsShow     CHAR (1),
	FLength    VARCHAR (20),
	IsReadonly CHAR (1),
	HelpID     VARCHAR (50),
	IsMatch    VARCHAR (50) DEFAULT ('0'),
	IsRequire  CHAR (1) DEFAULT ('0'),
	HelpType   CHAR (1) DEFAULT ('1'),
	RCols      VARCHAR (max),
	SCols      VARCHAR (max),
	SortOrder  VARCHAR (40),
	Width      VARCHAR (40),
	BindCol    VARCHAR (50),
	CONSTRAINT PK_EACATCOLS PRIMARY KEY (ID)
	)
GO


CREATE TABLE EACategory
	(
	ID             VARCHAR (50) NOT NULL,
	RNAME          VARCHAR (50),
	TmpTab         VARCHAR (50),
	ImprtType      CHAR (1) DEFAULT ('0'),
	ImprtProc      VARCHAR (1000),
	CancelProc     VARCHAR (1000),
	IgnoreSQL      VARCHAR (max),
	AfterImportSQL VARCHAR (max),
	CheckSQL       VARCHAR (max),
	Note           VARCHAR (100),
	CreateUser     VARCHAR (50),
	CreateTime     VARCHAR (20),
	LastModifyUser VARCHAR (50),
	LastModifyTime VARCHAR (20),
	ImprtDLL       VARCHAR (100),
	CancelDLL      VARCHAR (100),
	GetProc        VARCHAR (100),
	RefEND         INT,
	GetSQL         VARCHAR (1000),
	IsREF          CHAR (1),
	RefStart       INT,
	RefProc        VARCHAR (100),
	IsCustom       CHAR (1),
	CustomProc     VARCHAR (50),
	CustomName     VARCHAR (50),
	CONSTRAINT PK_EACATEGORY PRIMARY KEY (ID)
	)
GO


CREATE TABLE EACmpCateCols
	(
	ID           VARCHAR (50) NOT NULL,
	RID          VARCHAR (50),
	FCode        VARCHAR (50),
	FName        VARCHAR (50),
	MatchName    VARCHAR (50),
	DeafultValue VARCHAR (50),
	CalcSQL      VARCHAR (max),
	FomratSQL    VARCHAR (max),
	RMatchName   VARCHAR (50),
	SortOrder    VARCHAR (40),
	CONSTRAINT PK_EACMPCATECOLS PRIMARY KEY (ID)
	)
GO




CREATE TABLE EACmpCategory
	(
	ID             VARCHAR (50) NOT NULL,
	CID            VARCHAR (50),
	DWBH           VARCHAR (50),
	BMBH           VARCHAR (50),
	RNAME          VARCHAR (50),
	TmpTab         VARCHAR (50),
	ImprtType      VARCHAR (50),
	ImprtProc      VARCHAR (100),
	CancelProc     VARCHAR (100),
	IgnoreSQL      VARCHAR (max),
	AfterImportSQL VARCHAR (max),
	CheckSQL       VARCHAR (max),
	Note           VARCHAR (200),
	CreateUser     VARCHAR (50),
	CreateTime     VARCHAR (20),
	LastModifyUser VARCHAR (50),
	LastModifyTime VARCHAR (20),
	StartLine      VARCHAR (50),
	EndKeyWord     VARCHAR (50),
	GetProc        VARCHAR (max),
	GetSQL         VARCHAR (max),
	IsREF          CHAR (1),
	RefStart       INT,
	RefEnd         INT,
	RefProc        VARCHAR (50),
	CONSTRAINT PK_EACMPCATEGORY PRIMARY KEY (ID)
	)
GO



CREATE TABLE EAFunc
	(
	Id       VARCHAR (50) NOT NULL,
	Code     VARCHAR (50),
	Name     VARCHAR (50),
	IsDetail CHAR (1) DEFAULT ('0'),
	PId      VARCHAR (50),
	REFID    VARCHAR (50),
	REFType  CHAR (1),
	CONSTRAINT PK_EAFUNC PRIMARY KEY (Id)
	)
GO

CREATE TABLE EAHelp
	(
	ID          VARCHAR (50) NOT NULL,
	CODE        VARCHAR (50),
	NAME        VARCHAR (50),
	SFields     VARCHAR (1000),
	SFilter     VARCHAR (50),
	HelpType    CHAR (1) DEFAULT ('1'),
	PathField   VARCHAR (50),
	LevelField  VARCHAR (50),
	DetailField VARCHAR (50),
	IDField     VARCHAR (50),
	PIDField    VARCHAR (50),
	GradeFormat VARCHAR (50),
	ShowCols    VARCHAR (1000),
	PageSize    VARCHAR (50),
	CodeField   VARCHAR (50),
	NameField   VARCHAR (50),
	CONSTRAINT PK_EAHELP PRIMARY KEY (ID)
	)
GO


CREATE TABLE EAOpLog
	(
	ID       VARCHAR (50) NOT NULL,
	RID      VARCHAR (50),
	UserCode VARCHAR (50),
	OpInfo   VARCHAR (max),
	OpTime   VARCHAR (50),
	CONSTRAINT PK_EAOPLOG PRIMARY KEY (ID)
	)
GO


CREATE TABLE EARefTable
	(
	ID         VARCHAR (36) NOT NULL,
	UserId     VARCHAR (36),
	UserName   VARCHAR (36),
	YWID       VARCHAR (36),
	DWBH       VARCHAR (36),
	FLAG       CHAR (1),
	CreateTime VARCHAR (20),
	XM01       VARCHAR (255),
	XM02       VARCHAR (255),
	XM03       VARCHAR (255),
	XM04       VARCHAR (255),
	XM05       VARCHAR (255),
	XM07       VARCHAR (255),
	XM06       VARCHAR (255),
	XM18       VARCHAR (255),
	XM17       VARCHAR (255),
	XM16       VARCHAR (255),
	XM15       VARCHAR (255),
	XM12       VARCHAR (255),
	XM11       VARCHAR (255),
	XM10       VARCHAR (255),
	XM09       VARCHAR (255),
	XM14       VARCHAR (255),
	XM13       VARCHAR (255),
	XM08       VARCHAR (255),
	XM19       VARCHAR (255),
	XM20       VARCHAR (255),
	CONSTRAINT PK_EAREFTABLE PRIMARY KEY (ID)
	)
GO


CREATE TABLE TMP_LSWLDW
	(
	ID         VARCHAR (50) NOT NULL,
	FLAG       CHAR (1) DEFAULT ('0'),
	DWBH       VARCHAR (50),
	DWMC       VARCHAR (50),
	NOTE       VARCHAR (1000),
	CreateUser VARCHAR (50),
	CreateTime VARCHAR (20),
	COLORZT    CHAR (1),
	CONSTRAINT PK_TMP_LSWLDW PRIMARY KEY (ID)
	)
GO



CREATE TABLE TMP_LSWLDW1
	(
	ID         VARCHAR (50) NOT NULL,
	FLAG       CHAR (1) DEFAULT ('0'),
	DWBH       VARCHAR (50),
	DWMC       VARCHAR (50),
	NOTE       VARCHAR (1000),
	CreateUser VARCHAR (50),
	CreateTime VARCHAR (20),
	COLORZT    CHAR (1),
	CONSTRAINT PK_TMP_LSWLDW1 PRIMARY KEY (ID)
	)
GO




