

/*ȫ�ֹ����*/
CREATE TABLE EACategory
	(
	ID             VARCHAR (50) NOT NULL, /*ҵ�����*/
	RNAME          VARCHAR (50),/*����*/
	TmpTab         VARCHAR (50),/*��ʱ����*/
	ImprtType      CHAR (1) DEFAULT ('0'), /*�������� 1 ����0 excel*/
	ImprtProc      VARCHAR (100),/*�ϴ��洢����*/
	CancelProc     VARCHAR (100),/*ȡ���ϴ��洢����*/
	Note           VARCHAR (100),/*��ע*/
	CreateUser     VARCHAR (50),
	CreateTime     VARCHAR (20),
	LastModifyUser VARCHAR (50),
	LastModifyTime VARCHAR (20),
	ImprtDLL       VARCHAR (100),/*��չDLL ����;������*/
	GetProc        VARCHAR (100),/*����ȡ���洢����*/
	CONSTRAINT PK_EACATEGORY PRIMARY KEY (ID)
	)
GO

/*ȫ�ֹ�������Ϣ*/
CREATE TABLE EACatCols
	(
	ID         VARCHAR (50) NOT NULL,/*��ID*/
	RID        VARCHAR (50),/*ҵ��ID*/
	FCode      VARCHAR (50),/*�ֶα��*/
	FName      VARCHAR (50),/*�ֶ�����*/
	FType      VARCHAR (50),/*�ֶ����� char varchar int date float*/
	IsShow     CHAR (1),/*�Ƿ���ʾ*/
	FLength    int,/*����*/
	FPrec      int,/*����*/
	IsReadonly CHAR (1),/*�Ƿ�ֻ��*/
	HelpID     VARCHAR (50),/*����ID  EAHelp����*/
	BindCol    VARCHAR (50),/*�������ֶ�*/
	IsMatch    VARCHAR (50) DEFAULT ('0'), /*��ʱ����*/
	IsRequire  CHAR (1) DEFAULT ('0'),/*�Ƿ����*/
	RCols      clob,/*�����ֶ�*/
	SCols      clob,/*��ֵ�ֶ�*/
	SortOrder  int,/*������*/
	Width      VARCHAR (40),/*�п��*/
	CONSTRAINT PK_EACATCOLS PRIMARY KEY (ID)
	)
GO


/*��λ�����*/
CREATE TABLE EACmpCategory
	(
	ID             VARCHAR (50) NOT NULL,/*��λ����ID*/
	CID            VARCHAR (50),/*ȫ�ֹ���ID*/
	DWBH           VARCHAR (50),/*��λ��ţ�*/
	BMBH           VARCHAR (50),/*���ű�ţ� ���ƿ��Բ���*/
	RNAME          VARCHAR (50),/*���ƣ�*/
	TmpTab         VARCHAR (50),/*��ȫ�ֱ������*/
	ImprtType      VARCHAR (50),/*��ȫ�ֱ������*/
	ImprtProc      VARCHAR (100),/*��ȫ�ֱ������*/
	CancelProc     VARCHAR (100),/*��ȫ�ֱ������*/
	Note           VARCHAR (200),
	CreateUser     VARCHAR (50),
	CreateTime     VARCHAR (20),
	LastModifyUser VARCHAR (50),
	LastModifyTime VARCHAR (20),
	StartLine      VARCHAR (50),	/*Excel��ʼ��*/
	IsREF          CHAR (1),		/*�Ƿ���ʾ��������*/
	RefStart       INT,				/*�������ȡ��ʼ��*/
	RefProc        VARCHAR (50),	/*�������ϴ����ô洢����*/
	RefRemoveProc  VARCHAR (50),	/*�������Ƴ����ô洢����*/
	IsCustom       CHAR (1),		/*�Ƿ���ʾ�Զ��尴ť*/
	CustomProc     VARCHAR (50),/*�Զ�����ִ�д洢����*/
	CustomName     VARCHAR (50),/*�Զ��尴ť����*/
	CONSTRAINT PK_EACMPCATEGORY PRIMARY KEY (ID)
	)
GO
/*��λ��������Ϣ*/
CREATE TABLE EACmpCateCols
	(
	ID           VARCHAR (50) NOT NULL, /*��ID*/
	RID          VARCHAR (50),/*��λ��ID*/
	FCode        VARCHAR (50),/*�ֶα�� ��ȫ���б������*/
	FName        VARCHAR (50),/*�ֶ����� ��ȫ���б������*/
	MatchName    VARCHAR (50),/*��Ӧexcel����*/
	DeafultValue VARCHAR (50),/*Ĭ��ֵ*/
	CalcSQL      clob,/*����sql ��xx=��1����where 1=1 */
	SortOrder    int,/*������*/
	CONSTRAINT PK_EACMPCATECOLS PRIMARY KEY (ID)
	)
GO



/*�����ർ��*/
CREATE TABLE EAFunc
	(
	Id       VARCHAR (50) NOT NULL,
	Code     VARCHAR (50),
	Name     VARCHAR (50),
	IsDetail CHAR (1) DEFAULT ('0'),/*�Ƿ���ϸ*/
	PId      VARCHAR (50),
	REFID    VARCHAR (50),
	REFType  CHAR (1),    /*1��ȫ�ֹ���ID 2��url 3��winform*/
	URLInfo varchar(1000),/*������URL��Ϣ��*/
	FormInfo varchar(200),/*winform form dll*/
	CONSTRAINT PK_EAFUNC PRIMARY KEY (Id)
	)
GO

/*�Զ��������*/
CREATE TABLE EAHelp
	(
	ID          VARCHAR (50) NOT NULL,/*����ID*/
	CODE        VARCHAR (50),/*��� Ҳ���Ǳ�����*/
	NAME        VARCHAR (50),/*�������ƣ�*/
	SFields     VARCHAR (1000),/*ȡ���ֶΣ�*/
	SFilter     VARCHAR (50),/*������������ and xx='' */
	HelpType    CHAR (1) DEFAULT ('0'),/*�������� 1 ���� 0��ͨ*/
	PathField   VARCHAR (50),/*�ּ����ֶ�*/
	LevelField  VARCHAR (50),/*�����ֶ�*/
	DetailField VARCHAR (50),/*�Ƿ���ϸ�ֶ�*/
	IDField     VARCHAR (50),/*�����ֶ�*/
	PIDField    VARCHAR (50),/*��ID�ֶ�*/
	GradeFormat VARCHAR (50),/*�ּ��ṹ4444444*/
	ShowCols    VARCHAR (1000),/*��ʾ�� ��ʽ LSBZDW_DWBH,��λ���,200;LSBZDW_DWMC,��λ����,200  */
	PageSize    VARCHAR (50),/*��ҳ��С*/
	CodeField   VARCHAR (50),/*��������ֶ�*/
	NameField   VARCHAR (50),/*���������ֶ�*/
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

/*������*/
CREATE TABLE EARefTable
	(
	ID         VARCHAR (36) NOT NULL,
	UserId     VARCHAR (36),/*�û�ID*/
	UserName   VARCHAR (36),/*�û�����*/
	YWID       VARCHAR (36),/*ҵ���ʶID*/
	DWBH       VARCHAR (36),/*��λ���*/
	FLAG       CHAR (1),/*״̬*/
	CreateTime VARCHAR (20),/*����ʱ��*/
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

/*��������ʱ��*/
CREATE TABLE TMP_LSWLDW
	(
	ID         VARCHAR (50) NOT NULL,
	FLAG       CHAR (1) DEFAULT ('0'),/*Ĭ����  0 ���ϴ� 1���ϴ� 2������ ������*/
	CreateUser VARCHAR (50),/*�����û��˺� ������*/
	CreateTime VARCHAR (20),/*����ʱ�� ������*/
	COLORZT    CHAR (1),/*��ɫ״̬ ������ 1 ��ɫ 3��ɫ 4��ɫ*/
	DWBH       VARCHAR (50),/*���뵥λ��Ϣ ������*/
	YWID       VARCHAR (50),/*ҵ���ʶID ȫ�ֹ���ID�� ������*/
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




