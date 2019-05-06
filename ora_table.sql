

/*全局规则表*/
CREATE TABLE EACategory
	(
	ID             VARCHAR (50) NOT NULL, /*业务规则*/
	RNAME          VARCHAR (50),/*名称*/
	TmpTab         VARCHAR (50),/*临时表名*/
	ImprtType      CHAR (1) DEFAULT ('0'), /*导入类型 1 本地0 excel*/
	ImprtProc      VARCHAR (100),/*上传存储过程*/
	CancelProc     VARCHAR (100),/*取消上传存储过程*/
	Note           VARCHAR (100),/*备注*/
	CreateUser     VARCHAR (50),
	CreateTime     VARCHAR (20),
	LastModifyUser VARCHAR (50),
	LastModifyTime VARCHAR (20),
	ImprtDLL       VARCHAR (100),/*扩展DLL 程序;类名称*/
	GetProc        VARCHAR (100),/*本地取数存储过程*/
	CONSTRAINT PK_EACATEGORY PRIMARY KEY (ID)
	)
GO

/*全局规则列信息*/
CREATE TABLE EACatCols
	(
	ID         VARCHAR (50) NOT NULL,/*列ID*/
	RID        VARCHAR (50),/*业务ID*/
	FCode      VARCHAR (50),/*字段编号*/
	FName      VARCHAR (50),/*字段名称*/
	FType      VARCHAR (50),/*字段类型 char varchar int date float*/
	IsShow     CHAR (1),/*是否显示*/
	FLength    int,/*长度*/
	FPrec      int,/*精度*/
	IsReadonly CHAR (1),/*是否只读*/
	HelpID     VARCHAR (50),/*帮助ID  EAHelp主键*/
	BindCol    VARCHAR (50),/*帮助绑定字段*/
	IsMatch    VARCHAR (50) DEFAULT ('0'), /*暂时不用*/
	IsRequire  CHAR (1) DEFAULT ('0'),/*是否必填*/
	RCols      clob,/*返回字段*/
	SCols      clob,/*赋值字段*/
	SortOrder  int,/*列排序*/
	Width      VARCHAR (40),/*列宽度*/
	CONSTRAINT PK_EACATCOLS PRIMARY KEY (ID)
	)
GO


/*单位规则表*/
CREATE TABLE EACmpCategory
	(
	ID             VARCHAR (50) NOT NULL,/*单位规则ID*/
	CID            VARCHAR (50),/*全局规则ID*/
	DWBH           VARCHAR (50),/*单位编号？*/
	BMBH           VARCHAR (50),/*部门编号？ 估计可以不用*/
	RNAME          VARCHAR (50),/*名称？*/
	TmpTab         VARCHAR (50),/*从全局表带过来*/
	ImprtType      VARCHAR (50),/*从全局表带过来*/
	ImprtProc      VARCHAR (100),/*从全局表带过来*/
	CancelProc     VARCHAR (100),/*从全局表带过来*/
	Note           VARCHAR (200),
	CreateUser     VARCHAR (50),
	CreateTime     VARCHAR (20),
	LastModifyUser VARCHAR (50),
	LastModifyTime VARCHAR (20),
	StartLine      VARCHAR (50),	/*Excel起始行*/
	IsREF          CHAR (1),		/*是否显示关联导入*/
	RefStart       INT,				/*关联表读取起始行*/
	RefProc        VARCHAR (50),	/*关联表上传调用存储过程*/
	RefRemoveProc  VARCHAR (50),	/*关联表移除调用存储过程*/
	IsCustom       CHAR (1),		/*是否显示自定义按钮*/
	CustomProc     VARCHAR (50),/*自定义点击执行存储过程*/
	CustomName     VARCHAR (50),/*自定义按钮名称*/
	CONSTRAINT PK_EACMPCATEGORY PRIMARY KEY (ID)
	)
GO
/*单位规则列信息*/
CREATE TABLE EACmpCateCols
	(
	ID           VARCHAR (50) NOT NULL, /*列ID*/
	RID          VARCHAR (50),/*单位规ID*/
	FCode        VARCHAR (50),/*字段编号 从全局列表带过来*/
	FName        VARCHAR (50),/*字段名称 从全局列表带过来*/
	MatchName    VARCHAR (50),/*对应excel标题*/
	DeafultValue VARCHAR (50),/*默认值*/
	CalcSQL      clob,/*计算sql （xx=‘1’）where 1=1 */
	SortOrder    int,/*列排序*/
	CONSTRAINT PK_EACMPCATECOLS PRIMARY KEY (ID)
	)
GO



/*左侧分类导航*/
CREATE TABLE EAFunc
	(
	Id       VARCHAR (50) NOT NULL,
	Code     VARCHAR (50),
	Name     VARCHAR (50),
	IsDetail CHAR (1) DEFAULT ('0'),/*是否明细*/
	PId      VARCHAR (50),
	REFID    VARCHAR (50),
	REFType  CHAR (1),    /*1、全局规则ID 2、url 3、winform*/
	URLInfo varchar(1000),/*超链接URL信息？*/
	FormInfo varchar(200),/*winform form dll*/
	CONSTRAINT PK_EAFUNC PRIMARY KEY (Id)
	)
GO

/*自定义帮助表*/
CREATE TABLE EAHelp
	(
	ID          VARCHAR (50) NOT NULL,/*帮助ID*/
	CODE        VARCHAR (50),/*编号 也就是表名称*/
	NAME        VARCHAR (50),/*帮助名称？*/
	SFields     VARCHAR (1000),/*取数字段？*/
	SFilter     VARCHAR (50),/*帮助过滤条件 and xx='' */
	HelpType    CHAR (1) DEFAULT ('0'),/*帮助类型 1 树形 0普通*/
	PathField   VARCHAR (50),/*分级码字段*/
	LevelField  VARCHAR (50),/*级数字段*/
	DetailField VARCHAR (50),/*是否明细字段*/
	IDField     VARCHAR (50),/*主键字段*/
	PIDField    VARCHAR (50),/*父ID字段*/
	GradeFormat VARCHAR (50),/*分级结构4444444*/
	ShowCols    VARCHAR (1000),/*显示列 格式 LSBZDW_DWBH,单位编号,200;LSBZDW_DWMC,单位名称,200  */
	PageSize    VARCHAR (50),/*分页大小*/
	CodeField   VARCHAR (50),/*帮助编号字段*/
	NameField   VARCHAR (50),/*帮助名称字段*/
	CONSTRAINT PK_EAHELP PRIMARY KEY (ID)
	)
GO

CREATE TABLE EAOpLog
	(
	ID       VARCHAR (50) NOT NULL,
	RID      VARCHAR (50),
	UserCode VARCHAR (50),
	OpInfo   clob,
	OpTime   VARCHAR (50),
	CONSTRAINT PK_EAOPLOG PRIMARY KEY (ID)
	)
GO

/*关联表*/
CREATE TABLE EARefTable
	(
	ID         VARCHAR (36) NOT NULL,
	UserId     VARCHAR (36),/*用户ID*/
	UserName   VARCHAR (36),/*用户名称*/
	YWID       VARCHAR (36),/*业务标识ID*/
	DWBH       VARCHAR (36),/*单位编号*/
	FLAG       CHAR (1),/*状态*/
	CreateTime VARCHAR (20),/*创建时间*/
	XM01       VARCHAR (255),
	XM02       VARCHAR (255),
	XM03       VARCHAR (255),
	XM04       VARCHAR (255),
	XM05       VARCHAR (255),
	XM06       VARCHAR (255),
	XM07       VARCHAR (255),
	XM08       VARCHAR (255),
	XM09       VARCHAR (255),
	XM10       VARCHAR (255),
	XM11       VARCHAR (255),
	XM12       VARCHAR (255),
	XM13       VARCHAR (255),
	XM14       VARCHAR (255),
	XM15       VARCHAR (255),
	XM16       VARCHAR (255),
	XM17       VARCHAR (255),
	XM18       VARCHAR (255),
	XM19       VARCHAR (255),
	XM20       VARCHAR (255),
	XM21       VARCHAR (255),
	XM22       VARCHAR (255),
	XM23       VARCHAR (255),
	XM24       VARCHAR (255),
	XM25       VARCHAR (255),
	XM26       VARCHAR (255),
	XM27       VARCHAR (255),
	XM28       VARCHAR (255),
	XM29       VARCHAR (255),
	XM30       VARCHAR (255),
	XM31       VARCHAR (255),
	XM32       VARCHAR (255),
	XM33       VARCHAR (255),
	XM34       VARCHAR (255),
	XM35       VARCHAR (255),
	XM36       VARCHAR (255),
	XM37       VARCHAR (255),
	XM38       VARCHAR (255),
	XM39       VARCHAR (255),
	XM40       VARCHAR (255),
	XM41       VARCHAR (255),
	XM42       VARCHAR (255),
	XM43       VARCHAR (255),
	XM44       VARCHAR (255),
	XM45       VARCHAR (255),
	XM46       VARCHAR (255),
	XM47       VARCHAR (255),
	XM48       VARCHAR (255),
	XM49       VARCHAR (255),
	XM50       VARCHAR (255),
	CONSTRAINT PK_EAREFTABLE PRIMARY KEY (ID)
	)
GO

/*测试用临时表*/
CREATE TABLE TMP_LSWLDW
	(
	ID         VARCHAR (50) NOT NULL,
	FLAG       CHAR (1) DEFAULT ('0'),/*默认有  0 待上传 1已上传 2处理中 必须有*/
	CreateUser VARCHAR (50),/*创建用户账号 必须有*/
	CreateTime VARCHAR (20),/*创建时间 必须有*/
	COLORZT    CHAR (1),/*颜色状态 必须有 1 黄色 3绿色 4红色*/
	DWBH       VARCHAR (50),/*导入单位信息 必须有*/
	YWID       VARCHAR (50),/*业务标识ID 全局规则ID？ 必须有*/
	DWMC       VARCHAR (50),
	NOTE       VARCHAR (1000),
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




