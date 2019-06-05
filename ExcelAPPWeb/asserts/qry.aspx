<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="qry.aspx.cs" Inherits="ExcelAPPWeb.asserts.qry" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <link href="libs/leeui/css/common.css" rel="stylesheet" />
    <link href="libs/leeui/css/bulid.min.css" rel="stylesheet" />
    <link href="libs/leeui/css/icon.css" rel="stylesheet" />
    <script src="libs/jquery/jquery-1.10.2.min.js"></script>
    <script src="libs/laydate/laydate.js"></script>
    <link rel="stylesheet" href="css/sys.css" />
    <link rel="stylesheet" href="css/index.css" />

    <script src="libs/leeui/js/leeui.js"></script>

    <style>
        #filter_wrap {
            position: absolute;
            top: 20%;
            width: 500px;
            left: 50%;
            margin-left: -250px;
            border: 1px solid #DDD;
            padding: 10px;
            box-shadow: 1px 2px 5px #DDD;
        }

        #filter_title {
            padding-left: 10px;
            font-size: 16px;
            border-left: 3px solid #1890ff;
            margin-bottom: 10px;
        }

        #filter_line {
            border-bottom: 1px solid #DDD;
            margin-bottom: 10px;
        }

        .table-item {
            width: 100%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <input type="hidden" id="txt_query" runat="server" />
        <div id="filter_wrap">

            <div class="lee-table-form-border  table-query" style="margin: 0;">

                <div id="filter_title">往来单位查询</div>

                <div id="filter_line"></div>
                
                <div id="filter_bottom"></div>
                <div class="table-item" style="width: 100%; line-height: 34px;">
                    <div class="table-label">
                    </div>
                    <div class="table-editor">
                        <button class="lee-btn lee-btn-primary" id="btnQuery" type="button">查询</button>
                    </div>


                </div>
            </div>
        </div>
    </form>
    <script>

        var model = JSON.parse($("#txt_query").val());
        $(function () {
            for (var i = 0; i < model.filter.length; i++) {
                var html = [];
                var row = model.filter[i];
                if (row.IsDisplay == "0")
                    html.push(' <div class="table-item" style="display:none;">');
                else
                    html.push(' <div class="table-item">');
                html.push(' <div class="table-label">');
                html.push(row.DisplayName);
                html.push('    </div>');
                html.push('  <div class="table-editor">');
                html.push('        <input id="txt' + row.FieldName + '" />');
                html.push('    </div>');
                html.push('  </div> ');
                $("#filter_bottom").before(html.join(""));

                var $ele = $("#txt" + row.FieldName);
                switch (row.InputType) {
                    case "0":
                        $ele.leeTextBox();
                        break;
                    case "2":
                        $ele.leeDate();
                        break;
                    case "1":
                        $ele.leeTextBox({ number: true });
                        break;
                    case "5"://日期区间
                        $ele.leeDate({ range: true, showType: "" });
                        break;
                    case "6"://月度区间
                        $ele.leeDate({ range: true, showType: "month" });
                        break;
                    case "7"://年度区间
                        $ele.leeDate({ range: true, showType: "year" });
                        break;
                    case "4":
                        var info = row.GetInfoFrom;
                        var arr = info.split("~");
                        //"LSBZDW~LSBZDW~标准单位帮助~LSBZDW_DWBH~LSBZDW_DWMC";
                        $ele.leeLookup({
                            helpID: arr[0],
                            valueField: arr[3],
                            textField: arr[4],
                            url: "lookup.html?id=",
                            getFilter: function () { }
                        });
                        break;
                    default:
                        break;
                }
            }
            $("#btnQuery").click(btnSearch)
        });

        function btnSearch() {
            var para = [];
            var value = [];
            var sqlwhere = "";
            for (var i = 0; i < model.filter.length; i++) {
                var row = model.filter[i];
                var $ele = $("#txt" + row.FieldName);
                para.push(row.FieldName);
                var qvalue = $ele.leeUI().getValue();
                value.push($ele.leeUI().getValue());
                if (row.IsRequired == "1" && !qvalue) {
                    $.leeDialog && $.leeDialog.alert("请填写" + row.DisplayName + "的值", "", 'success');
                    return;
                }
                if (qvalue != "") {

                    if (row.InputType == "5" || row.InputType == "6" || row.InputType == "7") {
                        var arr = qvalue.split(" - ");
                        sqlwhere += " and (" + row.FieldName + ">='" + arr[0] + "' and " + row.FieldName + "<='" + arr[0] + "')";

                    } else {
                        sqlwhere += " and " + row.FieldName + row.CMP + "'" + $ele.leeUI().getValue() + "' ";
                    }


                }


            }
            var rmodel = {};
            rmodel.id = model.JTPUBQRDEF_ID;
            rmodel.title = model.JTPUBQRDEF_TITLE;
            rmodel.sql = model.JTPUBQRDEF_SQL;
            rmodel.subtitle = model.JTPUBQRDEF_SUBTIL;
            model.parr = para.join("^");
            model.varr = value.join("^")
            if (model.JTPUBQRDEF_TYPE == "PROC") {
                APIBridge.showProc(JSON.stringify(rmodel));
            } else {
                rmodel.sql += sqlwhere;

                APIBridge.showDev(JSON.stringify(rmodel));
            }
        }

    </script>
</body>
</html>
