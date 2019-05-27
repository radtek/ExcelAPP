var dg = null;
var Msg = {
    alert: function (msg, title, callback) {
        $.leeDialog && $.leeDialog.alert(msg, title, 'warn', callback, { width: 620 });

    },
    danger: function (msg, title, callback) {
        title = title || "异常";
        $.leeDialog.alert(msg, title, 'error', callback, { width: 720, height: 350 });
    },
    sucess: function (msg, title) {
        title = title || "提示";
        $.leeDialog && $.leeDialog.alert(msg, title, 'success');
    },
    load: function (msg) {
        msg = msg || "正在加载..";
        $.leeDialog.loading(msg, true);
    },
    loaded: function () {
        $.leeDialog && $.leeDialog.hideLoading();
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
};


var idp = idp || {};


idp.service = (function (win, $, core) {

    var basicURL = "../api/";

    $.ajaxSetup({
        beforeSend: function (XHR, s) {
        },
        success: function (data) {
            console.log(data);

            if (data.res == false) {
                Msg.alert(data.msg);
            }
            Msg.loaded();
        }
    });

    var service = {};


    //请求基类
    service.requestApi = function (api, data) {
        var url = basicURL + api;
        return $.ajax({
            //contentType: 'application/json',
            url: url,
            data: data,
            dataType: "json",
            type: "post"
        });
    }






    /*通用服务*/

    // 登录
    service.login = function (usercode, pwd) {
        return this.requestApi("account.ashx", {
            usercode: usercode,
            pwd: pwd
        });
    }
    // 获取菜单
    service.getFunc = function (usercode, pwd) {
        return this.requestApi("help.ashx", {
            op: "GetFunc"
        });
    }

    service.getHelpInfo = function (id) {
        return this.requestApi("help.ashx", {
            op: "GetHelpInfo",
            id: id
        });
    }
    service.getHelpData = function (id, filter, order, page, pageSize) {
        return this.requestApi("help.ashx", {
            op: "GetHelpData",
            id: id,
            filter: filter,
            order: order,
            page: page,
            pageSize: pageSize
        });
    }


    service.getTmpData = function (id, filter, order, page, pageSize, flag, start, end) {
        return this.requestApi("refresh.ashx", {
            id: id,
            filter: filter,
            order: order,
            page: page,
            pageSize: pageSize,
            flag: flag,
            startDate: start,
            endDate: end
        });
    }

    service.getHelpData = function (id, filter, order, page, pageSize) {
        return this.requestApi("help.ashx", {
            op: "GetHelpData",
            id: id,
            filter: filter,
            order: order,
            page: page,
            pageSize: pageSize
        });
    }


    service.beginUpload = function (ruleid, data, rdata) {
        return this.requestApi("excel.ashx", {
            op: "BeginUpload",
            ruleid: ruleid,
            data: data,
            rdata: rdata
        });
    }

    service.beginUploadRef = function (ruleid, data, rdata) {
        return this.requestApi("excel.ashx", {
            op: "UploadDataRef",
            ruleid: ruleid,
            data: data,
            rdata: rdata
        });
    }

    service.cancelUpload = function (ruleid, data) {
        return this.requestApi("excel.ashx", {
            op: "CancelUpload",
            ruleid: ruleid,
            data: data
        });
    }

    service.custom = function (ruleid, data) {
        return this.requestApi("excel.ashx", {
            op: "Custom",
            ruleid: ruleid,
            data: data
        });
    }
    service.refreshData = function (ruleid) {
        return this.requestApi("excel.ashx", {
            op: "RefreshData",
            ruleid: ruleid
        });
    }




    service.removeRef = function (ruleid, date) {
        return this.requestApi("excel.ashx", {
            op: "removeRef",
            ruleid: ruleid,
            date: date
        });
    }
    service.loadExcelData = function (ruleid, data) {
        return this.requestApi("excel.ashx", {
            op: "LoadExcelData",
            ruleid: ruleid,
            tmpdata: data
        });
    }
    service.LoadExcelDataLocal = function (ruleid, data) {
        return this.requestApi("excel.ashx", {
            op: "LoadExcelDataLocal",
            ruleid: ruleid,
            tmpdata: data
        });
    }


    service.loadExcelDataNew = function (ruleid, data) {
        return this.requestApi("excel.ashx", {
            op: "loadExcelDataNew",
            ruleid: ruleid,
            tmpdata: data
        });
    }
    service.deleteRow = function (ruleid, ids) {
        return this.requestApi("excel.ashx", {
            op: "deleteRow",
            ruleid: ruleid,
            ids: ids
        });
    }
    service.GetUserInfo = function () {
        return this.requestApi("rule.ashx", {
            op: "GetUserInfo"
        });
    }
    service.GetCookie = function () {
        return this.requestApi("rule.ashx", {
            op: "GetCookie"
        });
    }
    service.SetCookie = function (dwbh) {
        return this.requestApi("rule.ashx", {
            op: "SetCookie",
            dwbh: dwbh
        });
    }
    service.loadConfig = function (dwbh, lbid) {
        return this.requestApi("rule.ashx", {
            op: "LoadConfig",
            dwbh: dwbh,
            lbid: lbid
        });
    }
    return service;
})(window, $, window.idp.core || {});

