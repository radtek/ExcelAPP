

/*ȫ�ֹ����*/
CREATE TABLE EACategory
	(
	ID             VARCHAR (50) NOT NULL, /*ҵ�����*/
	RNAME          VARCHAR (50),/*����*/
	TmpTab         VARCHAR (50),/*��ʱ����*/
	ImprtType      CHAR (1) DEFAULT ('0'), /*�������� 1 ����0 excel*/
	ImprtProc      VARCHAR (100),/*�ϴ��洢����*/
	CancelProc     VARCHAR (100),/*ȡ���ϴ��洢����*/
	LoadProc       VARCHAR (100),/*�ϴ����м����ô洢����*/
	SwitchProc     VARCHAR (100),/*�л�ִ�д洢����*/
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
	IsMatch    VARCHAR (50) DEFAULT ('0'), /*�Ƿ�ƥ��*/
	MatchName  VARCHAR (200), /*�ĸ��ֶμ�¼ƥ�����*/
	IsRequire  CHAR (1) DEFAULT ('0'),/*�Ƿ����*/
	RCols      clob,/*�����ֶ�*/
	SCols      clob,/*��ֵ�ֶ�*/
	SortOrder  int,/*������*/
	Width      VARCHAR (40),/*�п��*/
	MatchRule  VARCHAR(200),/*�����ֶ� ����������������*/
	HelpFilter varchar(500),/*��������*/
	IsSum      char(1) DEFAULT ('0'),/*�Ƿ�ϼ�*/
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




create table YZ_E_File_File 
( FileId  varchar2 (60) , FileName  varchar2 (60) , FilePath  varchar2 (60) , FileType  varchar2 (60) , AddTime DATE  DEFAULT sysdate)

create table  YZ_E_SDNOTICE_READ (READID VARCHAR2(60)  default   SYS_GUID() ,READNAME VARCHAR2(60),READTIME DATE default sysdate ,RComment VARCHAR2(4000))





/*��ά�����������*/

/*��ѯ�Զ����ֶ�*/

CREATE TABLE EACustomFields
	(
	ClassSetCode   VARCHAR (40) NOT NULL,
	ActTable       VARCHAR (50) NOT NULL,
	FieldName      VARCHAR (50) NOT NULL,
	DisplayName    VARCHAR (50),
	Note           VARCHAR (100),
	FieldType      CHAR (1) DEFAULT ('0') NOT NULL,
	InputType      CHAR (1) DEFAULT ('0') NOT NULL,
	IsDeleted      CHAR (1) DEFAULT ('0') NOT NULL,
	IsRequired     CHAR (1) DEFAULT ('0') NOT NULL,
	CharLength     INT DEFAULT ((0)) NOT NULL,
	MaxValue       DECIMAL (20, 8) DEFAULT ((0)) NOT NULL,
	MinValue       DECIMAL (20, 8) DEFAULT ((0)) NOT NULL,
	DecimalDigits  INT DEFAULT ((0)) NOT NULL,
	DateTimeFormat CHAR (1) DEFAULT ('0') NOT NULL,
	DefaultValue   VARCHAR (255),
	GetInfoFrom    VARCHAR (50),
	GetFiledType   CHAR (1),
	GetFieldType   CHAR (1),
	Formula        VARCHAR (255),
	IsUserCustom   CHAR (1) DEFAULT ('1') NOT NULL,
	IsDisplay      CHAR (1) DEFAULT ('1') NOT NULL,
	DisplayOrder   INT DEFAULT ((100)) NOT NULL,
	OrderFields    VARCHAR (1000),
	IfEdit         CHAR (1) DEFAULT ('1'),
	FormulaOrder   INT,
	ALIAS          VARCHAR (20),
	DSID           VARCHAR (30),
	GetInfoWhere   VARCHAR (300),
	GetOtherValue  VARCHAR (400),
	FieldColSpan   CHAR (1) DEFAULT ('1'),
	CONSTRAINT PK_EACUSTOMFIELDS PRIMARY KEY (ClassSetCode, ActTable, FieldName)
	)
GO

/*��ѯ����*/
CREATE TABLE JTPUBQRDEF
	(
	JTPUBQRDEF_ID      VARCHAR (40) NOT NULL,
	JTPUBQRDEF_BH      VARCHAR (40),
	JTPUBQRDEF_MC      VARCHAR (100),
	JTPUBQRDEF_TITLE   VARCHAR (100),
	JTPUBQRDEF_SUBTIL  VARCHAR (100),
	JTPUBQRDEF_TYPE    VARCHAR (10),
	JTPUBQRDEF_DBSRC   VARCHAR (50),
	JTPUBQRDEF_SQL     clob,
	JTPUBQRDEF_ORA    clob,
	JTPUBQRDEF_DWFIELD VARCHAR (400),
	JTPUBQRDEF_RQFIELD VARCHAR (40),
	JTPUBQRDEF_WHERE   VARCHAR (100),
	CONSTRAINT PK_JTPUBQRDEF PRIMARY KEY (JTPUBQRDEF_ID)
	)
GO




/*��ѯ��������*/

CREATE TABLE JTPUBQRPARAMDEF
	(
	PARAMDEF_ID     VARCHAR (40) NOT NULL,
	PARAMDEF_QRYID  VARCHAR (40),
	PARAMDEF_ORD    VARCHAR (40),
	PARAMDEF_NAME   VARCHAR (100),
	PARAMDEF_TYPE   VARCHAR (10),
	PARAMDEF_CMP    VARCHAR (20),
	PARAMDEF_ISUSR  VARCHAR (1),
	PARAMDEF_CMPSTR VARCHAR (80),
	CONSTRAINT PK_JTPUBQRPARAMDEF PRIMARY KEY (PARAMDEF_ID)
	)
GO



/*��������*/

CREATE TABLE JTPUBLINKQRY
	(
	JTPUBLINKQRY_QRYID        VARCHAR (40) NOT NULL,
	JTPUBLINKQRY_CODE         VARCHAR (200) NOT NULL,
	JTPUBLINKQRY_NAME         VARCHAR (200),
	JTPUBLINKQRY_TYPE         VARCHAR (100),
	JTPUBLINKQRY_URL_PATH     VARCHAR (1000),
	JTPUBLINKQRY_CLASS        VARCHAR (1000),
	JTPUBLINKQRY_PARAMS       VARCHAR (1000),
	JTPUBLINKQRY_REPLACEVALUE VARCHAR (1000),
	CONSTRAINT PK_JTPUBLINKQRY PRIMARY KEY (JTPUBLINKQRY_QRYID, JTPUBLINKQRY_CODE)
	)
GO


/*��ά��������*/
CREATE TABLE JTPUBQRYFAV
	(
	QRYFAV_QRYID     VARCHAR (40) NOT NULL,
	QRYFAV_HASH      VARCHAR (200) NOT NULL,
	QRYFAV_NAME      VARCHAR (200) NOT NULL,
	QRYFAV_LOCATION  VARCHAR (10),
	QRYFAV_CREATER   VARCHAR (40),
	QRYFAV_ROW       VARCHAR (2000),
	QRYFAV_COL       VARCHAR (2000),
	QRYFAV_DATA      VARCHAR (2000),
	QRYFAV_FILTER    VARCHAR (2000),
	QRYFAV_SEL       VARCHAR (2000),
	QRYFAV_DEFAULT   CHAR (1),
	QRYFAV_FILTERVAL VARCHAR (2000),
	CONSTRAINT PK_JTPUBQRYFAV PRIMARY KEY (QRYFAV_QRYID, QRYFAV_HASH, QRYFAV_NAME)
	)
GO


CREATE TABLE JTPUBQRYFAVFILTER
	(
	QRYFILTER_QRYID VARCHAR (40) NOT NULL,
	QRYFILTER_USER  VARCHAR (40) NOT NULL,
	QRYFILTER_HASH  VARCHAR (200) NOT NULL,
	QRYFILTER_NAME  VARCHAR (80) NOT NULL,
	QRYFILTER_FIELD VARCHAR (40) NOT NULL,
	QRYFILTER_VALUE VARCHAR (500) NOT NULL,
	CONSTRAINT PK_JTPUBQRYFAVFILTER PRIMARY KEY (QRYFILTER_QRYID, QRYFILTER_USER, QRYFILTER_HASH, QRYFILTER_NAME, QRYFILTER_FIELD, QRYFILTER_VALUE)
	)
GO





CREATE TABLE JTPUBQRYFAVVISIT
	(
	QRYFAVVISIT_QRYID  VARCHAR (40) NOT NULL,
	QRYFAVVISIT_HASH   VARCHAR (200) NOT NULL,
	QRYFAVVISIT_USER   VARCHAR (40) NOT NULL,
	QRYFAVVISIT_ROW    VARCHAR (1000),
	QRYFAVVISIT_COL    VARCHAR (1000),
	QRYFAVVISIT_DATA   VARCHAR (1000),
	QRYFAVVISIT_FILTER VARCHAR (1000),
	QRYFAVVISIT_SEL    VARCHAR (1000),
	CONSTRAINT PK_JTPUBQRYFAVVISIT PRIMARY KEY (QRYFAVVISIT_QRYID, QRYFAVVISIT_HASH, QRYFAVVISIT_USER)
	)
GO



CREATE TABLE EAOTGS
	(
	F_DWBH  VARCHAR (30) DEFAULT (' ') NOT NULL,
	F_ID    VARCHAR (60) NOT NULL,
	F_GSBH  VARCHAR (2) NOT NULL,
	F_OTBH  VARCHAR (2) NOT NULL,
	F_TEXT  VARCHAR (255) NOT NULL,
	F_OTBZ  VARCHAR (1) NOT NULL,
	F_JS    VARCHAR (1) NOT NULL,
	F_ALIGN VARCHAR (1) NOT NULL,
	CONSTRAINT PK_EAOTGS PRIMARY KEY (F_DWBH, F_ID, F_GSBH, F_OTBH)
	)
GO





CREATE TABLE EATIGS
	(
	F_DWBH     VARCHAR (30) DEFAULT (' ') NOT NULL,
	F_ID       VARCHAR (60) NOT NULL,
	F_GSBH     VARCHAR (2) NOT NULL,
	F_TIBH     VARCHAR (18) NOT NULL,
	F_JS       VARCHAR (1),
	F_FIELD    VARCHAR (50),
	F_TEXT     VARCHAR (255),
	F_TYPE     VARCHAR (1),
	F_ALIGN    VARCHAR (1),
	F_WIDTH    INT DEFAULT ((0)),
	F_PROP     VARCHAR (3),
	F_PREC     VARCHAR (20),
	F_DISP     VARCHAR (254),
	F_REAL     VARCHAR (254),
	F_HJBZ     VARCHAR (1) DEFAULT ('0'),
	F_PXBZ     VARCHAR (1) DEFAULT ('0'),
	F_YHBZ     VARCHAR (1) DEFAULT ('0'),
	F_FHBZ     VARCHAR (1) DEFAULT ('1'),
	F_HZBZ     VARCHAR (1) DEFAULT ('0'),
	F_COLOR    VARCHAR (700) DEFAULT (' '),
	F_FORMAT   VARCHAR (50),
	F_ISGD     VARCHAR (1),
	F_TSTEXT   VARCHAR (200),
	F_GROUP    VARCHAR (50),
	F_TSHID    VARCHAR (50),
	F_PSUMTYPE VARCHAR (10),
	CONSTRAINT PK_EATIGS PRIMARY KEY (F_DWBH, F_ID, F_GSBH, F_TIBH)
	)
GO



CREATE TABLE EATIGSDEVQRY
	(
	F_DWBH  VARCHAR (30) NOT NULL,
	F_ID    VARCHAR (60) NOT NULL,
	F_GSBH  VARCHAR (2) NOT NULL,
	F_TIBH  VARCHAR (18) NOT NULL,
	F_JS    VARCHAR (1),
	F_FIELD VARCHAR (30),
	F_TEXT  VARCHAR (30),
	F_TYPE  VARCHAR (1),
	F_ALIGN VARCHAR (1),
	F_WIDTH INT,
	F_PROP  VARCHAR (3),
	F_PREC  VARCHAR (20),
	F_DISP  VARCHAR (254),
	F_REAL  VARCHAR (254),
	F_HJBZ  VARCHAR (1),
	F_PXBZ  VARCHAR (1),
	F_YHBZ  VARCHAR (1),
	F_FHBZ  VARCHAR (1),
	F_HZBZ  VARCHAR (1),
	F_COLOR VARCHAR (700),
	CONSTRAINT PRI_EATIGSDEVQRY PRIMARY KEY (F_DWBH, F_ID, F_GSBH, F_TIBH)
	)
GO

CREATE TABLE EAZBGS
	(
	F_DWBH  VARCHAR (30) DEFAULT (' ') NOT NULL,
	F_ID    VARCHAR (60) NOT NULL,
	F_GSBH  VARCHAR (2) NOT NULL,
	F_GSMC  VARCHAR (30),
	F_CAPT  VARCHAR (255) NOT NULL,
	F_CAPH  INT DEFAULT ((0)) NOT NULL,
	F_SCAPH INT DEFAULT ((0)) NOT NULL,
	F_BTGD  INT DEFAULT ((0)) NOT NULL,
	F_ROWH  INT DEFAULT ((0)) NOT NULL,
	F_BWGD  INT DEFAULT ((0)) NOT NULL,
	F_SPACE INT DEFAULT ((0)) NOT NULL,
	F_SYBZ  CHAR (1) DEFAULT ('0'),
	CONSTRAINT PK_EAZBGS PRIMARY KEY (F_DWBH, F_ID, F_GSBH)
	)
GO



/*���Ӱ汾�� 2019-07-18*/
CREATE TABLE dbo.EAVersion
	(
	ID      VARCHAR (50),
	VERSION VARCHAR (50)
	)
GO

