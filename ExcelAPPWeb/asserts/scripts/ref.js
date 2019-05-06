
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
        } else {
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

        });



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
            autoColWidth: true,
            onBeforeSelectRow: function (rowdata, rowid, rowobj) {
                //alert(rowdata.FLAG);
                //if (rowdata.FLAG == "1" && currentIndex == "1") return false;
            },
            parms: function () {

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
            pageSize: 100,

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
    chooseRelFile: function (id) {
        parent.APIBridge.chooseRelFile(this.getQuery("id"));
    },
    setFilePathNew: function (path) {
        $("#txtFileInfoNew").val(path);
    },
    setImportTips: function (msg) {
        //设置导入提示信息
        $(".export-tip").html(msg);
    },
    showREF: function () {
        $.leeDialog.open({
            target: $("#dataWrapSettings"),
            targetBody: true,
            title: "数据链接设置",
            name: 'test',
            isHidden: false,
            showMax: true,
            width: 900,
            slide: false,
            height: 350,
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
    loadExcelData: function () {
        var self = this;
        if ($("#txtFileInfo").val() == "") {
            Msg.alert("请选择要加载的excel!")
            return;
        }
        Msg.load("正在加载Excel数据");
        var data = parent.APIBridge.LoadExcelData(this.getQuery("id"));
        $("#btnUpload").removeAttr("disabled");

        idp.service.loadExcelData(CurModel.ID, data).done(function (data) {
            Msg.sucess("操作成功！");
            self.refreshData();
        })

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

        });
    },
    initREFGrid: function (keys) {

        var cols = [];
        if (keys.length) {
            $.each(keys, function (i, obj) {

                var col = {
                    display: obj, name: obj,
                    width: 100,
                    align: "left",
                    editor: { type: "text" }
                };

                cols.push(col);
            })

        }
        if ($("#gridInfoRef").leeUI()) {
            $("#gridInfoRef").leeUI().destroy();

            $(".datawrap .data").append('<div id="gridInfoRef"></div>');

        }
        $("#gridInfo").leeGrid({
            checkbox: true,
            showHeaderFilter: true,
            rownumbers: true,
            usePager: false,
            height: "100%",
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
            pageSize: 100,
            heightDiff: -1,
            enabledEdit: true,
            columns: cols,
            data: { Rows: [] }
        });

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
    beginUpload: function () {
        var self = this;
        var data = $("#gridInfo").leeGrid().getCheckedRows();
        if (data.length < 1) {
            Msg.alert("请选择选中数据!")
            return;
        }
        idp.service.beginUpload(CurModel.ID, JSON.stringify(data), JSON.stringify(deleteRow)).done(function (data) {
            if (data.res) {
                Msg.sucess("上传成功！");
                self.refreshData();
            }
        })


    },
    beginUploadRef: function () {
        var self = this;
        var data = $("#gridInfoRef").leeGrid().getCheckedRows();
        if (data.length < 1) {
            Msg.alert("请选择选中数据!")
            return;
        }
        idp.service.beginUploadRef(CurModel.ID, JSON.stringify(data), JSON.stringify(deleteRowRef)).done(function (data) {
            if (data.res) {
                Msg.sucess("上传成功！");
                self.refreshData();
            }
        })


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


        var self = this;
        var data = $("#gridInfo").leeGrid().getCheckedRows();
        if (data.length < 1) {
            Msg.alert("请选择选中数据!")
            return;
        }
        idp.service.cancelUpload(CurModel.ID, JSON.stringify(data)).done(function (data) {
            if (data.res) {
                Msg.sucess("取消上传成功！");
                self.refreshData();
            }
        })
    }
}


$(function () {
    $("#dataWrapSettings input").leeTextBox({});
    ImportController.getRuleInfo();
});
