

INSERT INTO EACatCols (ID, RID, FCode, FName, FType, IsShow, FLength, IsReadonly, HelpID, IsMatch, IsRequire, HelpType, RCols, SCols, SortOrder, Width, BindCol)
VALUES ('0001', '01', 'FLAG', '��ʶ', 'char', '1', '1', '1', NULL, '0', '0', '1', NULL, NULL, NULL, '80', NULL)
/

INSERT INTO EACatCols (ID, RID, FCode, FName, FType, IsShow, FLength, IsReadonly, HelpID, IsMatch, IsRequire, HelpType, RCols, SCols, SortOrder, Width, BindCol)
VALUES ('0002', '01', 'DWBH', '��λ���', 'varchar', '1', '1', '1', NULL, '1', '1', '1', NULL, NULL, NULL, '300', NULL)
/

INSERT INTO EACatCols (ID, RID, FCode, FName, FType, IsShow, FLength, IsReadonly, HelpID, IsMatch, IsRequire, HelpType, RCols, SCols, SortOrder, Width, BindCol)
VALUES ('0003', '01', 'DWMC', '��λ����', 'varchar', '1', '1', '1', 'LSBZDW', '1', '1', '1', 'LSBZDW_DWMC,LSBZDW_DWBH', 'DWMC,DWBH', NULL, '300', 'LSBZDW_DWMC')
/

INSERT INTO EACatCols (ID, RID, FCode, FName, FType, IsShow, FLength, IsReadonly, HelpID, IsMatch, IsRequire, HelpType, RCols, SCols, SortOrder, Width, BindCol)
VALUES ('0004', '01', 'NOTE', '��ע', 'varchar', '1', '1', '0', NULL, '0', '0', '1', NULL, NULL, NULL, '300', NULL)
/

INSERT INTO EACatCols (ID, RID, FCode, FName, FType, IsShow, FLength, IsReadonly, HelpID, IsMatch, IsRequire, HelpType, RCols, SCols, SortOrder, Width, BindCol)
VALUES ('0005', '02', 'DWBH', '��λ���', 'varchar', '1', '1', '1', NULL, '1', '1', '1', NULL, NULL, NULL, '300', NULL)
/

INSERT INTO EACatCols (ID, RID, FCode, FName, FType, IsShow, FLength, IsReadonly, HelpID, IsMatch, IsRequire, HelpType, RCols, SCols, SortOrder, Width, BindCol)
VALUES ('0006', '02', 'DWMC', '��λ����', 'varchar', '1', '1', '1', 'LSBZDW', '1', '1', '1', 'LSBZDW_DWMC,LSBZDW_DWBH', 'DWMC,DWBH', NULL, '300', 'LSBZDW_DWMC')
/

INSERT INTO EACatCols (ID, RID, FCode, FName, FType, IsShow, FLength, IsReadonly, HelpID, IsMatch, IsRequire, HelpType, RCols, SCols, SortOrder, Width, BindCol)
VALUES ('0007', '02', 'NOTE', '��ע', 'varchar', '1', '1', '0', NULL, '0', '0', '1', NULL, NULL, NULL, '300', NULL)
/

INSERT INTO EACatCols (ID, RID, FCode, FName, FType, IsShow, FLength, IsReadonly, HelpID, IsMatch, IsRequire, HelpType, RCols, SCols, SortOrder, Width, BindCol)
VALUES ('0008', '02', 'FLAG', '��ʶ', 'char', '1', '1', '1', NULL, '0', '0', '1', NULL, NULL, NULL, '80', NULL)
/

INSERT INTO EACategory (ID, RNAME, TmpTab, ImprtType, ImprtProc, CancelProc, IgnoreSQL, AfterImportSQL, CheckSQL, Note, CreateUser, CreateTime, LastModifyUser, LastModifyTime, ImprtDLL, CancelDLL, GetProc, RefEND, GetSQL, IsREF, RefStart, RefProc, IsCustom, CustomProc, CustomName)
VALUES ('01', '������λ��Ϣ', 'TMP_LSWLDW', '1', 'Proc_Import_LSWLDW', 'Proc_Cancel_LSWLDW', NULL, NULL, NULL, '����', 'test', '2019-04-14 11:00:00', 'test', '2019-04-14 11:00:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '1', 'Proc_Import_LSWLDW', '��λ��ť����')
/

INSERT INTO EACategory (ID, RNAME, TmpTab, ImprtType, ImprtProc, CancelProc, IgnoreSQL, AfterImportSQL, CheckSQL, Note, CreateUser, CreateTime, LastModifyUser, LastModifyTime, ImprtDLL, CancelDLL, GetProc, RefEND, GetSQL, IsREF, RefStart, RefProc, IsCustom, CustomProc, CustomName)
VALUES ('02', '������λ��Ϣ1', 'TMP_LSWLDW1', '0', 'Proc_Import_LSWLDW', 'Proc_Cancel_LSWLDW', NULL, NULL, NULL, '����', 'test', '2019-04-14 11:00:00', 'test', '2019-04-14 11:00:00', NULL, NULL, 'TMP_GET_PROC', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
/


INSERT INTO EACmpCateCols (ID, RID, FCode, FName, MatchName, DeafultValue, CalcSQL, FomratSQL, RMatchName, SortOrder)
VALUES ('0001', '0101', 'FLAG', '��ʶ', NULL, '0', NULL, NULL, NULL, NULL)
/

INSERT INTO EACmpCateCols (ID, RID, FCode, FName, MatchName, DeafultValue, CalcSQL, FomratSQL, RMatchName, SortOrder)
VALUES ('0002', '0101', 'DWBH', '��λ���', '�ͻ����', NULL, ' select  newid()', NULL, NULL, NULL)
/

INSERT INTO EACmpCateCols (ID, RID, FCode, FName, MatchName, DeafultValue, CalcSQL, FomratSQL, RMatchName, SortOrder)
VALUES ('0003', '0101', 'DWMC', '��λ����', '�ͻ�����', NULL, NULL, NULL, NULL, NULL)
/

INSERT INTO EACmpCateCols (ID, RID, FCode, FName, MatchName, DeafultValue, CalcSQL, FomratSQL, RMatchName, SortOrder)
VALUES ('0004', '0101', 'NOTE', '��ע', '˵��', NULL, 'DWMC+DWBH', NULL, NULL, NULL)
/

INSERT INTO EACmpCateCols (ID, RID, FCode, FName, MatchName, DeafultValue, CalcSQL, FomratSQL, RMatchName, SortOrder)
VALUES ('0005', '0201', 'DWBH', '��λ���', '�ͻ����', NULL, ' ', NULL, NULL, NULL)
/

INSERT INTO EACmpCateCols (ID, RID, FCode, FName, MatchName, DeafultValue, CalcSQL, FomratSQL, RMatchName, SortOrder)
VALUES ('0006', '0201', 'DWMC', '��λ����', '�ͻ�����', NULL, NULL, NULL, NULL, NULL)
/

INSERT INTO EACmpCateCols (ID, RID, FCode, FName, MatchName, DeafultValue, CalcSQL, FomratSQL, RMatchName, SortOrder)
VALUES ('0007', '0201', 'NOTE', '��ע', '˵��', NULL, 'DWMC+DWBH', NULL, NULL, NULL)
/

INSERT INTO EACmpCateCols (ID, RID, FCode, FName, MatchName, DeafultValue, CalcSQL, FomratSQL, RMatchName, SortOrder)
VALUES ('0008', '0201', 'FLAG', '��ʶ', NULL, '0', NULL, NULL, NULL, NULL)
/

INSERT INTO EACmpCategory (ID, CID, DWBH, BMBH, RNAME, TmpTab, ImprtType, ImprtProc, CancelProc, IgnoreSQL, AfterImportSQL, CheckSQL, Note, CreateUser, CreateTime, LastModifyUser, LastModifyTime, StartLine, EndKeyWord, GetProc, GetSQL, IsREF, RefStart, RefEnd, RefProc)
VALUES ('0101', '01', '0001', '01', '������λ����', 'TMP_LSWLDW', '1', 'Proc_Import_LSWLDW', 'Proc_Cancel_LSWLDW', 'and DWMC IS NULL', NULL, NULL, '����', 'liwl', '2019-04-04 11:00:00', 'liwl', '2019-04-04 11:00:00', '1', NULL, NULL, NULL, NULL, NULL, NULL, NULL)
/

INSERT INTO EACmpCategory (ID, CID, DWBH, BMBH, RNAME, TmpTab, ImprtType, ImprtProc, CancelProc, IgnoreSQL, AfterImportSQL, CheckSQL, Note, CreateUser, CreateTime, LastModifyUser, LastModifyTime, StartLine, EndKeyWord, GetProc, GetSQL, IsREF, RefStart, RefEnd, RefProc)
VALUES ('0201', '02', '0001', '01', '������λ����', 'TMP_LSWLDW1', '0', 'Proc_Import_LSWLDW', 'Proc_Cancel_LSWLDW', 'and DWMC IS NULL', NULL, NULL, '����', 'liwl', '2019-04-04 11:00:00', 'liwl', '2019-04-04 11:00:00', '1', NULL, NULL, NULL, NULL, NULL, NULL, NULL)
/

INSERT INTO EAFunc (Id, Code, Name, IsDetail, PId, REFID, REFType)
VALUES ('01', 'LS', '��������', '0', NULL, NULL, NULL)
/

INSERT INTO EAFunc (Id, Code, Name, IsDetail, PId, REFID, REFType)
VALUES ('0101', 'LS01', '������λ����', '1', '01', '01', '1')
/

INSERT INTO EAFunc (Id, Code, Name, IsDetail, PId, REFID, REFType)
VALUES ('0102', 'LS02', '������λ����1', '1', '01', '02', '1')
/

INSERT INTO EAFunc (Id, Code, Name, IsDetail, PId, REFID, REFType)
VALUES ('02', 'WZ', '���ʹ���', '0', NULL, NULL, '1')
/

INSERT INTO EAFunc (Id, Code, Name, IsDetail, PId, REFID, REFType)
VALUES ('0201', 'WZ01', '���ϵ���', '1', '02', '01', '1')
/

INSERT INTO EAHelp (ID, CODE, NAME, SFields, SFilter, HelpType, PathField, LevelField, DetailField, IDField, PIDField, GradeFormat, ShowCols, PageSize, CodeField, NameField)
VALUES ('EACATE/RY', 'EACATE/RY', '�������', 'ID,RNAME,TMPTAB', NULL, '0', NULL, NULL, 'ID', NULL, NULL, NULL, 'CODE,���,100;RNAME,����,200', '50', 'RNAME', 'RNAME')
/

INSERT INTO EAHelp (ID, CODE, NAME, SFields, SFilter, HelpType, PathField, LevelField, DetailField, IDField, PIDField, GradeFormat, ShowCols, PageSize, CodeField, NameField)
VALUES ('LSBZDW', 'LSBZDW', '��׼��λ����', NULL, NULL, '1', 'LSBZDW_DWNM', 'LSBZDW_JS', 'LSBZDW_MX', NULL, NULL, '4444444444444444444444', 'LSBZDW_DWBH,��λ���,200;LSBZDW_DWMC,��λ����,200', '30', 'LSBZDW_DWBH', 'LSBZDW_DWMC')
/



/*����汾��*/

/*���Ӱ汾�� 2019-07-18*/
INSERT INTO eaversion (ID, VERSION)
VALUES ('sys', '1.0.0')
/