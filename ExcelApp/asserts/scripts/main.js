var dg = null;
var currentIndex = 0;
var Msg = {
    alert: function (msg, title) {
        title = title || "提示";
        $.leeDialog.alert(msg, title, 'warn');
    },
    sucess: function (msg, title) {
        title = title || "提示";
        $.leeDialog.alert(msg, title, 'success');
    },
    load: function (msg) {
        msg = msg || "正在加载..";
        $.leeDialog.loading(msg, true);
    },
    loaded: function () {
        $.leeDialog.hideLoading();
    },
    progress: function () {
        dg = $.leeDialog.open({
            target: $("#progresswrap"),
            targetBody: true,
            title: "导入",
            name: 'lookupwindow',
            isHidden: false,
            showMax: true,
            width: 500,
            slide: false,
            height: 100
        });
    },
    hideProgress() {
        dg & dg.close();
    }
}


$("#txtOrgInfo").leeLookup({
    helpID: "LSBZDW",
    valueField: "LSBZDW_DWBH",
    textField: "LSBZDW_DWMC",
    url: "lookup.html?id=",
    getFilter: function () { }
});
$("#txtLbInfo").leeLookup({
    helpID: "EACATEGORY",
    valueField: "ID",
    textField: "RNAME",
    url: "lookup.html?id=",
    getFilter: function () { }
});
$("#txtFileInfo").leeTextBox({});
$("#tabinfo").leeTab({
    onafterSelectTabItem: function (tabid, id, tabindex) {
        currentIndex = tabindex;
        ImportController.refreshData();

        if (currentIndex == 0) {
            $("#btnCancel").attr("disabled", "disabled");
            $("#btnUpload").removeAttr("disabled");
        } else {
            $("#btnUpload").attr("disabled", "disabled");
            $("#btnCancel").removeAttr("disabled");
        }
    }
});
//{ "ID":"0101", "CID":"01", "RNAME":"往来单位导入", "DWBH":"0001", "BMBH":"01", "TmpTab":"TMP_LSWLDW", "ImprtType":"1", "ImprtProc":"Proc_Import_LSWLDW", "CancelProc":"Proc_Cancel_LSWLDW", "IgnoreSQL":"and DWMC IS NULL", "AfterImportSQL":null, "CheckSQL":null, "Note":"暂无", "StartLine":"1", "EndKeyWord":null, "CreateUser":"liwl", "CreateTime":"2019-04-04 11:00:00", "LastModifyUser":"liwl", "LastModifyTime":"2019-04-04 11:00:00", "Cols":[{ "ID": "0001", "RID": "0101", "FCode": "FLAG", "FName": "标识", "MatchName": null, "DeafultValue": "0", "CalcSQL": null, "FomratSQL": null, "RMatchName": null }, { "ID": "0002", "RID": "0101", "FCode": "DWBH", "FName": "单位编号", "MatchName": "客户编号", "DeafultValue": null, "CalcSQL": " select  newid()", "FomratSQL": null, "RMatchName": null }, { "ID": "0003", "RID": "0101", "FCode": "DWMC", "FName": "单位名称", "MatchName": "客户名称", "DeafultValue": null, "CalcSQL": null, "FomratSQL": null, "RMatchName": null }, { "ID": "0004", "RID": "0101", "FCode": "NOTE", "FName": "备注", "MatchName": "说明", "DeafultValue": null, "CalcSQL": "DWMC+DWBH", "FomratSQL": null, "RMatchName": null }], "Tmp":{ "ID":"01", "RNAME":"往来单位信息", "TmpTab":"TMP_LSWLDW", "ImprtType":"1", "ImprtProc":"Proc_Import_LSWLDW", "CancelProc":"Proc_Cancel_LSWLDW", "IgnoreSQL":null, "AfterImportSQL":null, "CheckSQL":null, "Note":"暂无", "CreateUser":"test", "CreateTime":"2019-04-14 11:00:00", "LastModifyUser":"test", "LastModifyTime":"2019-04-14 11:00:00", "Cols":[{ "ID": "0001", "RID": "01", "FCode": "FLAG", "FName": "标识", "FType": "char", "IsShow": "1", "FLength": "1", "IsReadonly": "1", "HelpID": null, "IsMatch": "0", "IsRequire": "0", "HelpType": "1", "RCols": null, "SSql": null, "RName": null }, { "ID": "0002", "RID": "01", "FCode": "DWBH", "FName": "单位编号", "FType": "varchar", "IsShow": "1", "FLength": "1", "IsReadonly": "1", "HelpID": null, "IsMatch": "1", "IsRequire": "1", "HelpType": "1", "RCols": null, "SSql": null, "RName": null }, { "ID": "0003", "RID": "01", "FCode": "DWMC", "FName": "单位名称", "FType": "varchar", "IsShow": "1", "FLength": "1", "IsReadonly": "1", "HelpID": null, "IsMatch": "1", "IsRequire": "1", "HelpType": "1", "RCols": null, "SSql": null, "RName": null }, { "ID": "0004", "RID": "01", "FCode": "NOTE", "FName": "备注", "FType": "varchar", "IsShow": "1", "FLength": "1", "IsReadonly": "0", "HelpID": null, "IsMatch": "0", "IsRequire": "0", "HelpType": "1", "RCols": null, "SSql": null, "RName": null }] } }


//$("#btnGetRule").LeeToolTip({ title: "Excel导入 临时表TMP_LSBZDW" });
//$("#btnGetRule").LeeToolTip({ title: "Excel导入3 临时表TMP_LSBZDW" });

//var rowData =
//    { DWBH: "0001", DWMC: "北京新东方集团", P: "北京市", C: "东城区", D: "市辖区", DQ: "华北", DM: "139393939393", FR: "张丽", DH: "13878786763", "EMAIL": "1234@qq.com", ZB: "1000w" };

//var data = [];
////data.push({ PDWBH:"",DWBH: "0", DWMC: "北京新东方集团", P: "北京市", C: "东城区", D: "市辖区", DQ: "华北", DM: "139393939393", FR: "张丽", DH: "13878786763", "EMAIL": "1234@qq.com", ZB: "1000w" });

//for (var i = 1; i < 100; i++) {

//    data.push({ FLAG: "1", PDWBH: "", DWBH: i, __hideChild: false, DWMC: "北京新东方集团", P: "北京市", C: "东城区", D: "市辖区", DQ: "华北", DM: "139393939393", FR: "张丽", DH: "13878786763", "EMAIL": "1234@qq.com", ZB: "1000w" });
//}




var CurModel = null;
var hasLoaded = false;
var ImportController = {

    getRuleInfo: function () {
        var dwbh = $("#txtOrgInfo").leeUI().getValue();
        var lbID = $("#txtLbInfo").leeUI().getValue();

        if (dwbh == "") {
            Msg.alert("请选择单位信息！");
            return;
        }
        if (lbID == "") {
            Msg.alert("请选择模板类别！");
            return;
        }
        Msg.load();
        var res = APIBridge.LoadConfig(dwbh, lbID);
        CurModel = JSON.parse(res);
        console.log(res);

        if (!CurModel) {
            Msg.alert("当前单位下没有配置导入规则信息！");
            Msg.loaded();
            return;
        }
        if (CurModel.ImprtType == "1") {
            this.setDataWrap(true);
            this.setImportTips("当前导入为excel导入，请选择对应的excel模板进行导入！");
            this.initGrid(CurModel);
            $(window).resize();
        }
        Msg.loaded();
    },
    initGrid: function (res) {
        $(".datawrap").addClass("hasdata");

        //加载列信息形成空表格
        var cols = [];
        if (res.Cols && res.Cols.length) {
            $.each(res.Cols, function (i, obj) {

                var col = {
                    display: obj.FName, name: obj.FCode,
                    width: obj.Width ? obj.Width : 100,
                    align: "left",
                    editor: getEditor(obj)
                };
                if (obj.FCode == "FLAG") {
                    col.render = function (row) {
                        return row["FLAG"] == "1" ? "已上传" : "未上传";
                    }
                }
                cols.push(col);
            })
        }


        function getEditor(obj) {
            if (obj.IsReadonly == "1") return null;
            if (obj.FCode == "FLAG") return null;
            if (obj.HelpID) {
                var opts = {

                    "valueField": obj.BindCol,
                    "textField": obj.BindCol,
                    type: "lookup", helpID: obj.HelpID, getFilter: function () { }
                };

                var arr = [];
                var rcol = obj.RCols.split(",");
                var scol = obj.SCols.split(",");
                for (var i in rcol) {
                    arr.push({
                        FField: scol[i],
                        HField: rcol[i]
                    });
                }

                opts.mapFields = arr;

                opts.url = "lookup.html?id=";

                opts.onConfirmSelect = function (g, p, data, srcID) {
                    function getMapObj(mapFields, data) {
                        var vsobj = {};
                        for (var i = 0; i < mapFields.length; i++) {
                            var HField = mapFields[i]["HField"];
                            var FField = mapFields[i]["FField"];
                            vsobj[FField] = data[HField] ? data[HField] : "";
                        }
                        return vsobj;
                    }

                    if (p.gridEditParm) {
                        // 多选模式 返回多条// 多选模式返回逗号合并// 如果是单选模式g
                        var mapFields = p.mapFields;
                        var vsobj = getMapObj(p.mapFields, data[0]);
                        var gridManger = p.host_grid;
                        p.gridEditParm.record = $.extend(p.gridEditParm.record, vsobj);

                        gridManger.updateRow(p.gridEditParm.rowindex, vsobj);

                        gridManger.endEdit();
                        var cell = gridManger.getCellObj(p.gridEditParm.record, p.gridEditParm.column);
                        window.setTimeout(function () { gridManger._applyEditor(cell) }, 100);
                    }
                }

                opts.service = {
                    getQueryHelpSwitch: function (helpID, value, codeField, textField, filter) {

                    }
                };

                opts.onClearValue = function (g, p, data, srcID) {
                    if (p.gridEditParm) {
                        var vsobj = {};
                        var mapFields = p.mapFields;
                        for (var i = 0; i < mapFields.length; i++) {
                            var FField = mapFields[i]["FField"];
                            vsobj[FField] = "";//这里得处理一下数字or日期
                        }
                        var gridManger = p.host_grid;
                        p.gridEditParm.record = $.extend(p.gridEditParm.record, vsobj);
                        gridManger.updateRow(p.gridEditParm.rowindex, vsobj);
                        gridManger.endEdit();
                    }
                }

                return opts;
            }


            if (obj.FType == "char" || obj.FType == "varchar") {
                return { type: "text" };
            }

            if (obj.FType == "number") {
                return { type: "number" };
            } if (obj.FType == "date") {
                return { type: "date" };
            }

        }


        if ($("#gridInfo").leeUI()) {
            $("#gridInfo").leeUI().destroy();

            $(".datawrap .data").append('<div id="gridInfo" style="border-top:0;"></div>');

        }
        $("#gridInfo").leeGrid({
            checkbox: true,
            showHeaderFilter: true,
            rownumbers: true,
            usePager: false,
            height: "100%",
            headerRowHeight: 28,
            rowHeight: 28,
            onBeforeSelectRow: function (rowdata, rowid, rowobj) {
                //alert(rowdata.FLAG);
                //if (rowdata.FLAG == "1" && currentIndex == "1") return false;
            },
            rowClsRender: function (data) {
                if (data["FLAG"] == "1") {
                    return "done";
                }
            },
            heightDiff: -18,
            enabledEdit: true,
            columns: cols,
            data: { Rows: [] }
        });

    },
    chooseFile: function () {
        APIBridge.chooseFile();
    },
    setFilePath: function (path) {
        $("#txtFileInfo").val(path);
    },
    setDataWrap: function (flag) {
        flag && $(".datawrap").addClass("show")
    },
    setImportTips: function (msg) {
        //设置导入提示信息
        $(".export-tip").html(msg);
    },
    setDescription: function () {
        //设置规则信息
    },
    refreshData: function () {

        Msg.load();
        APIBridge.refreshData();
    },
    loadExcelData: function () {

        if ($("#txtFileInfo").val() == "") {
            Msg.alert("请选择要加载的excel!")
            return;
        }
        Msg.load();
        APIBridge.LoadExcelData();
        $("#btnUpload").removeAttr("disabled");
        //加载excel 并插入中间表 返回给grid
    },
    setData: function (data) {
        data = JSON.parse(data);
        data = data.filter(function (value, index) {
            return value.FLAG == currentIndex;
        });
        this.setGridData(data);
        Msg.loaded();
    },
    setGridData: function (data) {
        $("#gridInfo").leeGrid().loadData({ Rows: data })
    },
    showError: function (res) {
        Msg.alert(res);
        Msg.loaded();
    },
    beginUpload: function () {
        var data = $("#gridInfo").leeGrid().getCheckedRows();
        if (data.length < 1) {
            Msg.alert("请选择选中数据!")
            return;
        }
        APIBridge.BeginUpload(JSON.stringify(data));
        Msg.progress();
    },
    progress: function (data) {
        $(".lee-progress-bg").css("width", data + "%");
        if (data == "100") {
            Msg.sucess("操作成功！");
            Msg.hideProgress();
            this.refreshData();
        }
    },
    msg: function (data) {
        Msg.alert(data);
        console.log(data);
    },
    setMessage: function (data) {
        $(".toolstrip").html(data);
    },
    cancelUpload: function () {

        var data = $("#gridInfo").leeGrid().getCheckedRows();
        if (data.length < 1) {
            Msg.alert("请选择选中数据!")
            return;
        }
        APIBridge.CancelUpload(JSON.stringify(data));
        Msg.progress();
    }
}