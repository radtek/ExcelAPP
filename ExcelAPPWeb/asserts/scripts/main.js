
var currentIndex = 0;

$("#txtQueryDateEnd").leeDate({});
$("#txtQueryDate").leeDate({});


$("#txtFileInfo").leeTextBox({});
$("#tabinfo").leeTab({
    onafterSelectTabItem: function (tabid, id, tabindex) {
        currentIndex = tabindex;
        ImportController.refreshData();

        if (currentIndex == 0) {
            $("#btnCancel").attr("disabled", "disabled");
            $("#btnUpload").removeAttr("disabled");
            $("#gridInfo").leeUI().options.disabled = false;

            $(".deleteWrap").show();
            $(".hasWrap").hide();
        } else {

            $(".deleteWrap").hide();
            $(".hasWrap").show();
            $("#btnUpload").attr("disabled", "disabled");
            $("#btnCancel").removeAttr("disabled");
            $("#gridInfo").leeUI().options.disabled = true;
        }
    }
});


var CurModel = null;
var hasLoaded = false;
var deleteRow = [];
var deleteRowRef = [];
var hasRequire = {};
var helpurl = "";

$("body").on("click", ".lee-grid-row-cell-inner .grid_remove", function (e) {

    var grid = $(this).closest(".lee-ui-grid").leeUI();
    var cell = $(this).closest(".lee-grid-row-cell");
    var row = $(this).closest(".lee-grid-row");
    var rowobj = grid.getRow(row.attr("id").split("|")[2]);
    grid.deleteRow(rowobj);
    deleteRow.push(rowobj);
    e.stopPropagation();
});
var ImportController = {

    export: function () {
        var grid = $("#gridInfo").leeUI();
        var data = grid.getData();
        var columns = grid.options.columns;
        var cols = [];
        for (var item in columns) {
            if (columns[item]["name"] == "操作")
                continue;
            cols.push({
                "0": columns[item]["name"],
                "1": columns[item]["display"],
                "2": columns[item]["width"]
            })

        }
        parent.APIBridge.Export(JSON.stringify(cols), JSON.stringify(data));
    },
    removeRows: function () {
        var self = this;
        var grid = $("#gridInfo").leeUI();
        var rows = grid.getCheckedRows();
        if (!rows.length) return;

        $.leeDialog.confirm("确认要-删除选中数据吗？", "提示", function (flag) {
            if (flag) {
                Msg.load("删除中..请稍后");
                var ids = [];
                for (var item in rows) {
                    ids.push(rows[item]["ID"]);
                }
                idp.service.deleteRow(CurModel.ID, ids.join(",")).done(function (data) {
                    Msg.loaded();
                    if (data.res) {
                        Msg.alert(data.data);
                        self.refreshData();
                    }
                });
                //for (var item in rows) {
                //    Msg.load("删除第" + item + "行总计" + rows + " 请勿关闭！");
                //    grid.deleteRow(rows[item]);
                //    deleteRow.push(rows[item]);
                //}

            }


        });

    },
    getQuery: function (key) {
        var reg = new RegExp("(^|&)" + key + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]);
        return "";
    },
    getRuleInfo: function () {
        var self = this;
        var dwbh = this.getQuery("dwbh");
        var lbID = this.getQuery("id");

        if (dwbh == "") {
            Msg.alert("没有获取到单位信息！");
            return;
        }
        if (lbID == "") {
            Msg.alert("没有获取到模板类别！");
            return;
        }
        Msg.load();
        var res = idp.service.loadConfig(dwbh, lbID).done(function (data) {
            Msg.loaded();
            CurModel = data.data;
            parent.APIBridge && parent.APIBridge.setModel(self.getQuery("id"), JSON.stringify(CurModel));
            if (!CurModel) {
                Msg.alert("当前单位下没有配置导入规则信息！");
                Msg.loaded();
                return;
            }
            if (CurModel.ImprtType == "1") {
                self.setDataWrap(true);
                self.setImportTips("当前导入为excel导入，请选择对应的excel模板进行导入！");
                self.initGrid(CurModel);
                $(window).resize();
            } else {
                self.setDataWrap(true);
                self.hideFile();
                self.setImportTips("当前导入为本地数据源导入！");
                self.initGrid(CurModel);
                $(window).resize();
            }

            if (CurModel.IsCustom == "1") {
                $("#btnCustom").show().html(CurModel.CustomName);
            }
            $(".ref-tip").show().html(CurModel.REFHELP);
            helpurl = CurModel.UPURL;
        });



    },
    initGrid: function (res) {
        $(".datawrap").addClass("hasdata");
        this.matchFields = [];
        var self = this;
        //加载列信息形成空表格
        var cols = [];
        if (res.Cols && res.Cols.length) {
            $.each(res.Cols, function (i, obj) {


                if (obj.IsMatch == "1")
                    self.matchFields.push(obj.FCode);


                var col = {
                    display: obj.FName, name: obj.FCode,
                    width: obj.Width ? obj.Width : 100,
                    align: "left",
                    required: obj.IsRequire == "1" ? true : false,
                    editor: getEditor(obj)
                };

                if (obj.IsRequire == "1") {
                    hasRequire[obj.FCode] = obj.FName;
                }
                if (obj.FCode == "FLAG") {
                    col.render = function (row) {
                        return row["FLAG"] == "1" ? "已上传" : "未上传";
                    }
                }
                if (obj.IsSum == "1") {
                    col.totalSummary =
                        {
                            render: function (suminf, column, cell) {
                                return '<div>合计:' + suminf.sum.toFixed(2)  + '</div>';
                            },
                            type: 'sum'
                        };
                }
                //if (obj.FType == "date") {
                //    col.render = function (rowdata, rowindex, value, column) {
                //        if (value) {

                //        }
                //    }
                //}
                cols.push(col);
            })

            cols.push({
                display: "操作", name: "操作",
                sort: false,
                width: 60,
                align: "center",
                render: function (g) {
                    if (g.FLAG == "0") {
                        return "<button class='lee-btn lee-btn-xs grid_remove'>删除</button>";
                    }

                }
            });

        }


        function getEditor(obj) {
            if (obj.IsReadOnly == "1") return null;
            if (obj.FCode == "FLAG") return null;


            if (obj.FType == "date") {
                return { type: "date", showType: "datetime" };
            }
            if (obj.HelpID) {
                if (obj.HelpID.length > 3) {
                    var opts = {
                        "valueField": obj.BindCol,
                        "textField": obj.BindCol,
                        type: "lookup", helpID: obj.HelpID,
                        getFilter: function () {
                            var row = this.host_grid_row;
                            var filter = obj.HelpFitler;
                            filter = filter.replace("{GS_DWBH}", CurModel.DWBH);
                            for (var item in row) {
                                filter = filter.replace("{" + item + "}", row[item]);
                            }

                            return filter;
                        },
                        service: {
                            getQueryHelpSwitch: function (helpID, value, codeField, textField, filter) {

                                var defer = $.Deferred();
                                var data = idp.service.getHelpData(
                                    helpID,
                                    " and " + obj.BindCol + " like '" + value + "%'",
                                    "",
                                    1,
                                    1000
                                ).done(function (data) {

                                    defer.resolve({ res: data.res, data: data.data.Rows });
                                });
                                return defer.promise();
                            }
                        }

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

                    opts.isMatch = obj.IsMatch;
                    opts.matchName = obj.MatchRule;

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

                            var before = JSON.parse(JSON.stringify(p.gridEditParm.record));
                            var gridManger = p.host_grid;
                            p.gridEditParm.record = $.extend(p.gridEditParm.record, vsobj);

                            gridManger.updateRow(p.gridEditParm.rowindex, vsobj);

                            gridManger.endEdit();

                            vsobj[obj.FCode] = data[0][opts.valueField];
                            if (opts.isMatch == "1")
                                DealOtherRow(before, vsobj, gridManger, self.matchFields);
                            var cell = gridManger.getCellObj(p.gridEditParm.record, p.gridEditParm.column);
                            window.setTimeout(function () { gridManger._applyEditor(cell) }, 100);


                        }
                    }

                    function DealOtherRow(record, vsobj, gridManger, matchFields) {
                        var data = $("#gridInfo").leeUI().data;

                        var fields = opts.matchName;
                        if (!fields) return;
                        var codeField = fields.split(",")[0];

                        var nameField = fields.split(",")[1];
                        for (var item in data.Rows) {
                            var row = data.Rows[item];
                            var isUpdate = false;
                            if (record["ID"] != row["ID"]) {

                                if (row[codeField] == record[codeField] && row[nameField] == record[nameField]) {
                                    isUpdate = true;
                                }
                                if (isUpdate) {
                                    gridManger.updateRow(row, vsobj);
                                }
                            }

                        }

                    }

                    //opts.service = {
                    //    getQueryHelpSwitch: function (helpID, value, codeField, textField, filter) {

                    //    }
                    //};

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
            }


            if (obj.FType == "char" || obj.FType == "varchar") {
                return { type: "text" };
            }

            if (obj.FType == "number") {
                return { type: "number" };
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
            //autoColWidth: true,
            onBeforeSelectRow: function (rowdata, rowid, rowobj) {
                //alert(rowdata.FLAG);
                //if (rowdata.FLAG == "1" && currentIndex == "1") return false;
            },
            parms: function () {
                if ($("#txtQueryDate").val().length != 10) {
                    alert("起始日期不能为空或格式不正确");
                    return '';
                }
                if ($("#txtQueryDateEnd").val().length != 10) {
                    alert("结束日期不能为空或格式不正确");
                    return '';
                }
                var res = {
                    "id": CurModel.ID,
                    "flag": currentIndex,
                    "start": $("#txtQueryDate").val(),
                    "end": $("#txtQueryDateEnd").val()
                };

                return res;
            },
            dataAction: "server",
            url: "../api/refresh.ashx",
            usePager: true,
            pageSize: 200,
            pageSizeOptions: [200, 400, 600, 5000],
            rowClsRender: function (data) {
                if (data["COLORZT"] == "1") {
                    return "yellow";
                }
                if (data["COLORZT"] == "2") {
                    return "done";

                }
                if (data["COLORZT"] == "3") {
                    return "green";
                }
                if (data["COLORZT"] == "4") {
                    return "red";
                }
            },
            heightDiff: -1,
            enabledEdit: true,
            columns: cols,
            data: { Rows: [] }
        });

    },
    chooseFile: function (id) {
        parent.APIBridge.chooseFile(this.getQuery("id"));
    },
    chooseRelFile: function (id) {
        parent.APIBridge.chooseRelFile(this.getQuery("id"));
    },
    setFilePath: function (path) {
        $("#txtFileInfo").val(path);
    },
    setFilePathNew: function (path) {
        $("#txtFileInfoNew").val(path);
    },
    setDataWrap: function (flag) {
        flag && $(".datawrap").addClass("show")
    },
    hideFile: function () {
        $(".fileWrap").hide();
    },
    setImportTips: function (msg) {
        //设置导入提示信息
        $(".export-tip").html(msg);
    },
    showREF: function () {
        this.dgRef = $.leeDialog.open({
            target: $("#dataWrapSettings"),
            targetBody: true,
            title: "关联表设置",
            name: 'refset',
            isHidden: false,
            showMax: true,
            width: 900,
            slide: false,
            height: 400,
            //buttons: [
            //    {
            //        id: "dialog_lookup_ok",
            //        text: '选择',
            //        cls: 'lee-btn-primary lee-dialog-btn-ok',
            //        onclick: function (item, dialog) {
            //            APIBridge.SetSettings(JSON.stringify(getModel()));
            //            dialog.close()

            //        }
            //    },
            //    {
            //        text: '取消',
            //        cls: 'lee-dialog-btn-cancel ',
            //        onclick: function (item, dialog) {
            //            dialog.close()

            //        }
            //    }
            //]
        });
    },
    setDescription: function () {
        //设置规则信息
    },
    refreshData: function () {
        var self = this;
        $("#gridInfo").leeUI().loadData(true);
        //Msg.load();
        //idp.service.refreshData(CurModel.ID).done(function (data) {
        //    self.setData(data.data);
        //    Msg.loaded();
        //});
    },
    refreshRefData: function () {
        var self = this;
        $("#gridRel").leeUI().loadData(true);

    },
    loadExcelData: function () {
        var self = this;

        if (CurModel.ImprtType == "0") {
            var data = parent.APIBridge.LoadLocalData(this.getQuery("id"));
            $("#btnUpload").removeAttr("disabled");
            Msg.load("加载Excel数据");
            idp.service.LoadExcelDataLocal(CurModel.ID, data).done(function (data) {

                if (data.res) {
                    Msg.sucess("操作成功！");
                    self.refreshData();
                    if (CurModel.IsREF == "1") {
                        self.showREF();
                    }
                    if (data.tips) {
                        Msg.alert(data.tips);
                    }
                }

            })
        } else {

            if ($("#txtFileInfo").val() == "") {
                Msg.alert("请选择要加载的excel!")
                return;
            }
            Msg.load("加载Excel数据");
            var data = parent.APIBridge.LoadExcelData(this.getQuery("id"));
            $("#btnUpload").removeAttr("disabled");

            idp.service.loadExcelData(CurModel.ID, data).done(function (data) {

                if (data.res) {
                    Msg.sucess("操作成功！");
                    self.refreshData();
                    if (CurModel.IsREF == "1") {
                        self.showREF();
                    }
                    if (data.tips) {
                        Msg.alert(data.tips);
                    }
                }


            })
        }


        //加载excel 并插入中间表 返回给grid
    },
    LoadExcelDataNew: function () {
        var self = this;
        if ($("#txtFileInfoNew").val() == "") {
            Msg.alert("请选择要加载的excel!")
            return;
        }
        Msg.load("正在加载Excel数据");
        var data = parent.APIBridge.LoadExcelDataNew(this.getQuery("id"));
        //$("#btnUpload").removeAttr("disabled");

        idp.service.loadExcelDataNew(CurModel.ID, data).done(function (data) {
            Msg.sucess("操作成功！");
            $(".ref-tip").show().html("");
            self.initREFGrid(data.data);

        });
    },
    initREFGrid: function (keys) {

        var cols = [];
        $.each(keys, function (key, val) {
            var col = {
                display: key + "(" + val + ")", name: key,
                width: 100,
                align: "left",
                editor: { type: "text" }
            };
            cols.push(col);
        })


        if ($("#gridRel").leeUI()) {
            $("#gridRel").leeUI().destroy();

            $("#refWrap").append('<div id="gridRel"></div>');

        }
        $("#gridRel").leeGrid({
            checkbox: true,
            showHeaderFilter: false,
            rownumbers: true,
            usePager: false,
            height: "300",
            headerRowHeight: 28,
            rowHeight: 28,
            autoColWidth: true,
            onBeforeSelectRow: function (rowdata, rowid, rowobj) {
            },
            parms: function () {
                var res = {
                    "id": CurModel.ID,
                    "flag": "",
                    "start": $("#txtQueryDate").val(),
                    "end": $("#txtQueryDateEnd").val()
                };

                return res;
            },
            dataAction: "server",
            url: "../api/ref.ashx",
            usePager: true,
            pageSize: 200,
            pageSizeOptions: [200, 400, 600],
            heightDiff: -1,
            enabledEdit: true,
            columns: cols,
            data: { Rows: [] }
        });

        $("#btnUploadRef").removeAttr("disabled");

    },
    setData: function (data) {

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
    custom: function () {
        var self = this;
        var data = $("#gridInfo").leeGrid().getCheckedRows();
        if (data.length < 1) {
            Msg.alert("请选择选中数据!")
            return;
        }
        idp.service.custom(CurModel.ID, JSON.stringify(data)).done(function (data) {
            Msg.sucess("操作成功！");
            self.refreshData();
        })
    },
    Help: function () {


        var self = this;
        idp.service.GetUserInfo().done(function (data) {
            var CurModel = data.data;
            currentuserid = CurModel.Id;
            currentusername = CurModel.Name;
            currentcode = CurModel.Code;
            // currentgsdwbh = $("#txtOrgInfo").leeUI().getValue();
            if (helpurl.length > 1) {
                var helpurln = helpurl + CurModel.Code + "&p_USERNAME=" + currentcode;
                open(helpurln);
            }
        });


    },
    beginUpload: function () {
        var self = this;
        var data = $("#gridInfo").leeGrid().getCheckedRows();

        if (data.length < 1) {
            Msg.alert("请选择选中数据!")
            return;
        }

        for (var item in data) {
            for (var key in hasRequire) {

                if (data[item][key] == "" || data[item][key] == null || $.trim(data[item][key]) == "") {

                    Msg.alert("请填写第" + (parseInt(item) + 1) + "行" + "字段【" + hasRequire[key] + "】的值");
                    return;
                }
            }
        }
        $.leeDialog.confirm("确认要上传吗？", "提示", function (flag) {
            if (flag) {
                Msg.load("正在上传");
                idp.service.beginUpload(CurModel.ID, JSON.stringify(data), JSON.stringify(deleteRow)).done(function (data) {

                    if (data.tips) {
                        Msg.alert(data.tips);
                    }
                    if (data.res) {
                        Msg.sucess("上传成功！");
                        self.refreshData();
                    }

                })
            }
        });
    },
    beginUploadRef: function () {
        var self = this;
        var data = $("#gridRel").leeGrid().getCheckedRows();
        if (data.length < 1) {
            Msg.alert("请选择选中数据!")
            return;
        }

        $.leeDialog.confirm("确认要上传关联吗", "提示", function (flag) {
            if (flag) {
                Msg.load("正在上传");
                idp.service.beginUploadRef(CurModel.ID, JSON.stringify(data), JSON.stringify(deleteRowRef)).done(function (data) {
                    if (data.res) {
                        Msg.loaded();
                        Msg.sucess("上传成功！");
                        ImportController.refreshData();
                        self.refreshRefData();
                        self.dgRef.close();
                    }
                })
            }
        });




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
    removeRef: function () {
        //removeRef

        var self = this;
        $.leeDialog.confirm("确认要移除历史?", "提示", function (flag) {
            if (flag) {
                var date = $("#txtQueryDateRef").val()
                idp.service.removeRef(CurModel.ID, date).done(function (data) {
                    if (data.res) {
                        Msg.sucess("操作成功！");
                        ImportController.refreshData();
                        self.refreshRefData();
                        self.dgRef.close();
                    }
                });
            }
        });

    },
    cancelUpload: function () {


        var self = this;
        var data = $("#gridInfo").leeGrid().getCheckedRows();
        if (data.length < 1) {
            Msg.alert("请选择选中数据!")
            return;
        }

        $.leeDialog.confirm("确认取消上传吗？", "提示", function (flag) {
            if (flag) {
                idp.service.cancelUpload(CurModel.ID, JSON.stringify(data)).done(function (data) {
                    if (data.res) {
                        Msg.sucess("取消上传成功！");
                        self.refreshData();
                    }
                })
            }
        });

    }
}


$(function () {

    $("#txtQueryDateRef").leeDate({ range: true });
    $("#dataWrapSettings input").leeTextBox({});
    ImportController.getRuleInfo();
});
