

INSERT INTO EACatCols (ID, RID, FCode, FName, FType, IsShow, FLength, IsReadonly, HelpID, IsMatch, IsRequire, HelpType, RCols, SCols, SortOrder, Width, BindCol)
VALUES ('0001', '01', 'FLAG', '标识', 'char', '1', '1', '1', NULL, '0', '0', '1', NULL, NULL, NULL, '80', NULL)
/

INSERT INTO EACatCols (ID, RID, FCode, FName, FType, IsShow, FLength, IsReadonly, HelpID, IsMatch, IsRequire, HelpType, RCols, SCols, SortOrder, Width, BindCol)
VALUES ('0002', '01', 'DWBH', '单位编号', 'varchar', '1', '1', '1', NULL, '1', '1', '1', NULL, NULL, NULL, '300', NULL)
/

INSERT INTO EACatCols (ID, RID, FCode, FName, FType, IsShow, FLength, IsReadonly, HelpID, IsMatch, IsRequire, HelpType, RCols, SCols, SortOrder, Width, BindCol)
VALUES ('0003', '01', 'DWMC', '单位名称', 'varchar', '1', '1', '1', 'LSBZDW', '1', '1', '1', 'LSBZDW_DWMC,LSBZDW_DWBH', 'DWMC,DWBH', NULL, '300', 'LSBZDW_DWMC')
/

INSERT INTO EACatCols (ID, RID, FCode, FName, FType, IsShow, FLength, IsReadonly, HelpID, IsMatch, IsRequire, HelpType, RCols, SCols, SortOrder, Width, BindCol)
VALUES ('0004', '01', 'NOTE', '备注', 'varchar', '1', '1', '0', NULL, '0', '0', '1', NULL, NULL, NULL, '300', NULL)
/

INSERT INTO EACatCols (ID, RID, FCode, FName, FType, IsShow, FLength, IsReadonly, HelpID, IsMatch, IsRequire, HelpType, RCols, SCols, SortOrder, Width, BindCol)
VALUES ('0005', '02', 'DWBH', '单位编号', 'varchar', '1', '1', '1', NULL, '1', '1', '1', NULL, NULL, NULL, '300', NULL)
/

INSERT INTO EACatCols (ID, RID, FCode, FName, FType, IsShow, FLength, IsReadonly, HelpID, IsMatch, IsRequire, HelpType, RCols, SCols, SortOrder, Width, BindCol)
VALUES ('0006', '02', 'DWMC', '单位名称', 'varchar', '1', '1', '1', 'LSBZDW', '1', '1', '1', 'LSBZDW_DWMC,LSBZDW_DWBH', 'DWMC,DWBH', NULL, '300', 'LSBZDW_DWMC')
/

INSERT INTO EACatCols (ID, RID, FCode, FName, FType, IsShow, FLength, IsReadonly, HelpID, IsMatch, IsRequire, HelpType, RCols, SCols, SortOrder, Width, BindCol)
VALUES ('0007', '02', 'NOTE', '备注', 'varchar', '1', '1', '0', NULL, '0', '0', '1', NULL, NULL, NULL, '300', NULL)
/

INSERT INTO EACatCols (ID, RID, FCode, FName, FType, IsShow, FLength, IsReadonly, HelpID, IsMatch, IsRequire, HelpType, RCols, SCols, SortOrder, Width, BindCol)
VALUES ('0008', '02', 'FLAG', '标识', 'char', '1', '1', '1', NULL, '0', '0', '1', NULL, NULL, NULL, '80', NULL)
/

INSERT INTO EACategory (ID, RNAME, TmpTab, ImprtType, ImprtProc, CancelProc, IgnoreSQL, AfterImportSQL, CheckSQL, Note, CreateUser, CreateTime, LastModifyUser, LastModifyTime, ImprtDLL, CancelDLL, GetProc, RefEND, GetSQL, IsREF, RefStart, RefProc, IsCustom, CustomProc, CustomName)
VALUES ('01', '往来单位信息', 'TMP_LSWLDW', '1', 'Proc_Import_LSWLDW', 'Proc_Cancel_LSWLDW', NULL, NULL, NULL, '暂无', 'test', '2019-04-14 11:00:00', 'test', '2019-04-14 11:00:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '1', 'Proc_Import_LSWLDW', '单位按钮导入')
/

INSERT INTO EACategory (ID, RNAME, TmpTab, ImprtType, ImprtProc, CancelProc, IgnoreSQL, AfterImportSQL, CheckSQL, Note, CreateUser, CreateTime, LastModifyUser, LastModifyTime, ImprtDLL, CancelDLL, GetProc, RefEND, GetSQL, IsREF, RefStart, RefProc, IsCustom, CustomProc, CustomName)
VALUES ('02', '往来单位信息1', 'TMP_LSWLDW1', '0', 'Proc_Import_LSWLDW', 'Proc_Cancel_LSWLDW', NULL, NULL, NULL, '暂无', 'test', '2019-04-14 11:00:00', 'test', '2019-04-14 11:00:00', NULL, NULL, 'TMP_GET_PROC', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
/


INSERT INTO EACmpCateCols (ID, RID, FCode, FName, MatchName, DeafultValue, CalcSQL, FomratSQL, RMatchName, SortOrder)
VALUES ('0001', '0101', 'FLAG', '标识', NULL, '0', NULL, NULL, NULL, NULL)
/

INSERT INTO EACmpCateCols (ID, RID, FCode, FName, MatchName, DeafultValue, CalcSQL, FomratSQL, RMatchName, SortOrder)
VALUES ('0002', '0101', 'DWBH', '单位编号', '客户编号', NULL, ' select  newid()', NULL, NULL, NULL)
/

INSERT INTO EACmpCateCols (ID, RID, FCode, FName, MatchName, DeafultValue, CalcSQL, FomratSQL, RMatchName, SortOrder)
VALUES ('0003', '0101', 'DWMC', '单位名称', '客户名称', NULL, NULL, NULL, NULL, NULL)
/

INSERT INTO EACmpCateCols (ID, RID, FCode, FName, MatchName, DeafultValue, CalcSQL, FomratSQL, RMatchName, SortOrder)
VALUES ('0004', '0101', 'NOTE', '备注', '说明', NULL, 'DWMC+DWBH', NULL, NULL, NULL)
/

INSERT INTO EACmpCateCols (ID, RID, FCode, FName, MatchName, DeafultValue, CalcSQL, FomratSQL, RMatchName, SortOrder)
VALUES ('0005', '0201', 'DWBH', '单位编号', '客户编号', NULL, ' ', NULL, NULL, NULL)
/

INSERT INTO EACmpCateCols (ID, RID, FCode, FName, MatchName, DeafultValue, CalcSQL, FomratSQL, RMatchName, SortOrder)
VALUES ('0006', '0201', 'DWMC', '单位名称', '客户名称', NULL, NULL, NULL, NULL, NULL)
/

INSERT INTO EACmpCateCols (ID, RID, FCode, FName, MatchName, DeafultValue, CalcSQL, FomratSQL, RMatchName, SortOrder)
VALUES ('0007', '0201', 'NOTE', '备注', '说明', NULL, 'DWMC+DWBH', NULL, NULL, NULL)
/

INSERT INTO EACmpCateCols (ID, RID, FCode, FName, MatchName, DeafultValue, CalcSQL, FomratSQL, RMatchName, SortOrder)
VALUES ('0008', '0201', 'FLAG', '标识', NULL, '0', NULL, NULL, NULL, NULL)
/

INSERT INTO EACmpCategory (ID, CID, DWBH, BMBH, RNAME, TmpTab, ImprtType, ImprtProc, CancelProc, IgnoreSQL, AfterImportSQL, CheckSQL, Note, CreateUser, CreateTime, LastModifyUser, LastModifyTime, StartLine, EndKeyWord, GetProc, GetSQL, IsREF, RefStart, RefEnd, RefProc)
VALUES ('0101', '01', '0001', '01', '往来单位导入', 'TMP_LSWLDW', '1', 'Proc_Import_LSWLDW', 'Proc_Cancel_LSWLDW', 'and DWMC IS NULL', NULL, NULL, '暂无', 'liwl', '2019-04-04 11:00:00', 'liwl', '2019-04-04 11:00:00', '1', NULL, NULL, NULL, NULL, NULL, NULL, NULL)
/

INSERT INTO EACmpCategory (ID, CID, DWBH, BMBH, RNAME, TmpTab, ImprtType, ImprtProc, CancelProc, IgnoreSQL, AfterImportSQL, CheckSQL, Note, CreateUser, CreateTime, LastModifyUser, LastModifyTime, StartLine, EndKeyWord, GetProc, GetSQL, IsREF, RefStart, RefEnd, RefProc)
VALUES ('0201', '02', '0001', '01', '往来单位导入', 'TMP_LSWLDW1', '0', 'Proc_Import_LSWLDW', 'Proc_Cancel_LSWLDW', 'and DWMC IS NULL', NULL, NULL, '暂无', 'liwl', '2019-04-04 11:00:00', 'liwl', '2019-04-04 11:00:00', '1', NULL, NULL, NULL, NULL, NULL, NULL, NULL)
/

INSERT INTO EAFunc (Id, Code, Name, IsDetail, PId, REFID, REFType)
VALUES ('01', 'LS', '基础数据', '0', NULL, NULL, NULL)
/

INSERT INTO EAFunc (Id, Code, Name, IsDetail, PId, REFID, REFType)
VALUES ('0101', 'LS01', '往来单位导入', '1', '01', '01', '1')
/

INSERT INTO EAFunc (Id, Code, Name, IsDetail, PId, REFID, REFType)
VALUES ('0102', 'LS02', '往来单位导入1', '1', '01', '02', '1')
/

INSERT INTO EAFunc (Id, Code, Name, IsDetail, PId, REFID, REFType)
VALUES ('02', 'WZ', '物资管理', '0', NULL, NULL, '1')
/

INSERT INTO EAFunc (Id, Code, Name, IsDetail, PId, REFID, REFType)
VALUES ('0201', 'WZ01', '物料导入', '1', '02', '01', '1')
/

INSERT INTO EAHelp (ID, CODE, NAME, SFields, SFilter, HelpType, PathField, LevelField, DetailField, IDField, PIDField, GradeFormat, ShowCols, PageSize, CodeField, NameField)
VALUES ('EACATE/RY', 'EACATE/RY', '导入类别', 'ID,RNAME,TMPTAB', NULL, '0', NULL, NULL, 'ID', NULL, NULL, NULL, 'CODE,编号,100;RNAME,名称,200', '50', 'RNAME', 'RNAME')
/

INSERT INTO EAHelp (ID, CODE, NAME, SFields, SFilter, HelpType, PathField, LevelField, DetailField, IDField, PIDField, GradeFormat, ShowCols, PageSize, CodeField, NameField)
VALUES ('LSBZDW', 'LSBZDW', '标准单位帮助', NULL, NULL, '1', 'LSBZDW_DWNM', 'LSBZDW_JS', 'LSBZDW_MX', NULL, NULL, '4444444444444444444444', 'LSBZDW_DWBH,单位编号,200;LSBZDW_DWMC,单位名称,200', '30', 'LSBZDW_DWBH', 'LSBZDW_DWMC')
/



/*插入版本号*/

/*增加版本表 2019-07-18*/
INSERT INTO eaversion (ID, VERSION)
VALUES ('sys', '1.0.0')
/