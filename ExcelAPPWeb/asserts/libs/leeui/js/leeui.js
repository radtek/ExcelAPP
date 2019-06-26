/*
 *  leeUI 1.0.0 
 * Author skiphyuk  [ skiphyuk@163.com ] 
 * 
 */
(function ($) {
    Function.prototype.leeExtend = function (parent, overrides) {
        if (typeof parent != 'function') return this; //如果父类不是function 直接返回
        this.base = parent.prototype; //base 记录父类的原型链
        this.base.constructor = parent; //
        var f = function () { };
        f.prototype = parent.prototype;
        this.prototype = new f(); //当前原型指向new func
        this.prototype.constructor = this; //
        if (overrides) $.extend(this.prototype, overrides);
        //扩展集成方法	
    }

    Function.prototype.leeDefer = function (o, defer, args) {
        var fn = this;
        return setTimeout(function () {
            fn.apply(o, args || []);
        }, defer);
    }
    //leeUI核心对象
    window.leeUI = $.leeUI = {
        version: "1.0.0",
        managerCount: 0,
        //组件缓存
        managers: {},
        managerIdPrev: "leeUI",
        //管理器id已经存在时自动创建新的
        autoNewId: true,
        //错误提示
        error: {
            managerIsExist: '管理器id已经存在'
        },
        pluginPrev: 'lee',
        attrPrev: 'data',
        getId: function (prev) { //随机生成一个组件ID
            prev = prev || this.managerIdPrev;
            var id = prev + (1000 + this.managerCount);
            this.managerCount++;
            return id;

        },
        add: function (manager) {
            if (arguments.length == 2) {
                var m = arguments[1];
                m.id = m.id || m.options.id || arguments[0].id;
                this.addManager(m);
                return;
            }
            if (!manager.id) manager.id = this.getId(manager.__idPrev());
            //if (this.managers[manager.id]) manager.id = this.getId(manager.__idPrev());
            //if (this.managers[manager.id])
            //{
            //    throw new Error(this.error.managerIsExist);
            //}
            this.managers[manager.id] = manager; //添加一个组件信息
        },
        remove: function (arg) {
            if (typeof arg == "string" || typeof arg == "number") {
                delete leeUI.managers[arg];
            } else if (typeof arg == "object") {
                if (arg instanceof leeUI.core.Component) {
                    delete leeUI.managers[arg.id];
                } else {
                    if (!$(arg).attr(this.idAttrName)) return false;
                    delete leeUI.managers[$(arg).attr(this.idAttrName)];
                }
            }

        },
        //获取leeui对象
        //1,传入ligerui ID
        //2,传入Dom Object
        get: function (arg, idAttrName) {
            idAttrName = idAttrName || "uiid";
            if (typeof arg == "string" || typeof arg == "number") {
                return leeUI.managers[arg];
            } else if (typeof arg == "object") {
                var domObj = arg.length ? arg[0] : arg;
                var id = domObj[idAttrName] || $(domObj).attr(idAttrName);
                if (!id) return null;
                return leeUI.managers[id];
            }
            return null;
        },
        find: function (type) //根据类型查找某一个对象
        {
            var arr = [];
            for (var id in this.managers) {
                var manager = this.managers[id];
                if (type instanceof Function) {
                    if (manager instanceof type) {
                        arr.push(manager);
                    }
                } else if (type instanceof Array) {
                    if ($.inArray(manager.__getType(), type) != -1) {
                        arr.push(manager);
                    }
                } else {
                    if (manager.__getType() == type) {
                        arr.push(manager);
                    }
                }
            }
            return arr;
        },
        //$.fn.liger{Plugin} 和 $.fn.ligerGet{Plugin}Manager
        //会调用这个方法,并传入作用域(this)
        //parm [plugin]  插件名
        //parm [args] 参数(数组)
        //parm [ext] 扩展参数,定义命名空间或者id属性名
        run: function (plugin, args, ext) {
            if (!plugin) return;
            ext = $.extend({
                defaultsNamespace: 'leeUIDefaults',
                methodsNamespace: 'leeUIMethods',
                controlNamespace: 'controls',
                idAttrName: 'uiid',
                isStatic: false,
                hasElement: true, //是否拥有element主体(比如drag、resizable等辅助性插件就不拥有)
                propertyToElemnt: null //链接到element的属性名
            }, ext || {});
            plugin = plugin.replace(/^leeUIGet/, '');
            plugin = plugin.replace(/^leeUI/, '');
            if (this == null || this == window || ext.isStatic) {
                if (!leeUI.plugins[plugin]) {
                    leeUI.plugins[plugin] = {
                        fn: $[leeUI.pluginPrev + plugin],
                        isStatic: true
                    };
                }
                return new $.leeUI[ext.controlNamespace][plugin]($.extend({}, $[ext.defaultsNamespace][plugin] || {}, $[ext.defaultsNamespace][plugin + 'String'] || {}, args.length > 0 ? args[0] : {}));
            }
            if (!leeUI.plugins[plugin]) {
                leeUI.plugins[plugin] = {
                    fn: $.fn[leeUI.pluginPrev + plugin],
                    isStatic: false
                };
            }
            if (/Manager$/.test(plugin)) return leeUI.get(this, ext.idAttrName);
            this.each(function () {
                if (this[ext.idAttrName] || $(this).attr(ext.idAttrName)) {
                    var manager = leeUI.get(this[ext.idAttrName] || $(this).attr(ext.idAttrName));
                    if (manager && args.length > 0) manager.set(args[0]);
                    //已经执行过  获取控件管理器的内容
                    return;
                }
                if (args.length >= 1 && typeof args[0] == 'string') return;
                //只要第一个参数不是string类型,都执行组件的实例化工作
                var options = args.length > 0 ? args[0] : null;
                var p = $.extend({}, $[ext.defaultsNamespace][plugin], $[ext.defaultsNamespace][plugin + 'String'], options);
                if (ext.propertyToElemnt) p[ext.propertyToElemnt] = this;
                if (ext.hasElement) {
                    new $.leeUI[ext.controlNamespace][plugin](this, p);
                } else {
                    new $.leeUI[ext.controlNamespace][plugin](p);
                }
            });

            if (this.length == 0) return null;
            if (args.length == 0) return leeUI.get(this, ext.idAttrName);
            if (typeof args[0] == 'object') return leeUI.get(this, ext.idAttrName);
            if (typeof args[0] == 'string') {
                var manager = leeUI.get(this, ext.idAttrName);
                if (manager == null) return;
                if (args[0] == "option") {
                    if (args.length == 2)
                        return manager.get(args[1]); //manager get
                    else if (args.length >= 3)
                        return manager.set(args[1], args[2]); //manager set
                } else {
                    var method = args[0];
                    if (!manager[method]) return; //不存在这个方法
                    var parms = Array.apply(null, args);
                    parms.shift();
                    return manager[method].apply(manager, parms); //manager method
                }
            }
            return null;
        },
        //扩展
        //1,默认参数     
        //2,本地化扩展 
        defaults: {},
        //3,方法接口扩展
        methods: {},
        //命名空间
        //核心控件,封装了一些常用方法
        core: {},
        //命名空间
        //组件的集合
        controls: {},
        //plugin 插件的集合
        plugins: {}
    };
    //扩展对象
    $.leeUIDefaults = {};

    //扩展对象
    $.leeUIMethods = {};

    //关联起来
    leeUI.defaults = $.leeUIDefaults;
    leeUI.methods = $.leeUIMethods;

    //获取ligerui对象
    //parm [plugin]  插件名,可为空
    $.fn.leeUI = function (plugin) {
        if (plugin) {
            return leeUI.run.call(this, plugin, arguments);
        } else {
            return leeUI.get(this);
        }
    };

    //组件基类
    //1,完成定义参数处理方法和参数属性初始化的工作
    //2,完成定义事件处理方法和事件属性初始化的工作
    leeUI.core.Component = function (options) {
        //事件容器
        this.events = this.events || {};
        //配置参数
        this.options = options || {};
        //子组件集合索引
        this.children = {};
    };
    //扩展core对象的原型方法 
    $.extend(leeUI.core.Component.prototype, {
        __getType: function () {
            return 'leeUI.core.Component';
        },
        __idPrev: function () {
            return 'leeUI';
        },

        //设置属性
        // arg 属性名    value 属性值 
        // arg 属性/值   value 是否只设置事件
        set: function (arg, value, value2) {
            if (!arg) return;
            if (typeof arg == 'object') {
                var tmp;
                if (this.options != arg) {
                    $.extend(this.options, arg);
                    tmp = arg;
                } else {
                    tmp = $.extend({}, arg);
                }
                if (value == undefined || value == true) {
                    for (var p in tmp) {
                        if (p.indexOf('on') == 0) //如果是事件开头 则绑定事件
                            this.set(p, tmp[p]);
                    }
                }
                if (value == undefined || value == false) {
                    for (var p in tmp) {
                        if (p.indexOf('on') != 0) //如果不是on开头则绑定属性
                            this.set(p, tmp[p], value2);
                    }
                }
                return;
            }
            var name = arg;
            //事件参数
            if (name.indexOf('on') == 0) {
                //绑定事件
                if (typeof value == 'function')
                    this.bind(name.substr(2), value);
                return;
            }
            if (!this.options) this.options = {};
            if (this.trigger('propertychange', [arg, value]) == false) return;
            this.options[name] = value;
            var pn = '_set' + name.substr(0, 1).toUpperCase() + name.substr(1);
            if (this[pn]) {
                this[pn].call(this, value, value2);
            }
            this.trigger('propertychanged', [arg, value]);
        },

        //获取属性
        get: function (name) {
            var pn = '_get' + name.substr(0, 1).toUpperCase() + name.substr(1);
            if (this[pn]) {
                return this[pn].call(this, name);
            }
            return this.options[name];
        },

        hasBind: function (arg) {
            var name = arg.toLowerCase();
            var event = this.events[name];
            if (event && event.length) return true;
            return false;
        },

        //触发事件
        //data (可选) Array(可选)传递给事件处理函数的附加参数
        trigger: function (arg, data) {
            if (!arg) return;
            var name = arg.toLowerCase();
            var event = this.events[name];
            if (!event) return;
            data = data || [];
            if ((data instanceof Array) == false) {
                data = [data];
            }
            for (var i = 0; i < event.length; i++) {
                var ev = event[i];
                if (ev.handler.apply(ev.context, data) == false)
                    return false;
            }
        },

        //绑定事件
        bind: function (arg, handler, context) {
            if (typeof arg == 'object') {
                for (var p in arg) {
                    this.bind(p, arg[p]);
                }
                return;
            }
            if (typeof handler != 'function') return false;
            var name = arg.toLowerCase();
            var event = this.events[name] || [];
            context = context || this;
            event.push({
                handler: handler,
                context: context
            });
            this.events[name] = event;
        },

        //取消绑定
        unbind: function (arg, handler) {
            if (!arg) {
                this.events = {};
                return;
            }
            var name = arg.toLowerCase();
            var event = this.events[name];
            if (!event || !event.length) return;
            if (!handler) {
                delete this.events[name];
            } else {
                for (var i = 0, l = event.length; i < l; i++) {
                    if (event[i].handler == handler) {
                        event.splice(i, 1);
                        break;
                    }
                }
            }
        },
        destroy: function () {
            leeUI.remove(this);
        }
    });

    //界面组件基类, 
    //1,完成界面初始化:设置组件id并存入组件管理器池,初始化参数
    //2,渲染的工作,细节交给子类实现
    //parm [element] 组件对应的dom element对象
    //parm [options] 组件的参数
    leeUI.core.UIComponent = function (element, options) {
        leeUI.core.UIComponent.base.constructor.call(this, options); //基类执行
        var extendMethods = this._extendMethods(); //
        if (extendMethods) $.extend(this, extendMethods);
        this.element = element;
        this._init();
        this._preRender();
        this.trigger('render');
        this._render();
        this.trigger('rendered');
        this._rendered();
    };

    //UIComponent 继承
    leeUI.core.UIComponent.leeExtend(leeUI.core.Component, {
        __getType: function () {
            return 'leeUI.core.UIComponent';
        },
        //扩展方法
        _extendMethods: function () {

        },
        _init: function () {
            this.type = this.__getType();
            if (!this.element) {
                this.id = this.options.id || leeUI.getId(this.__idPrev());
            } else {
                if (this._isExternal) {
                    this.id = leeUI.getId(this.__idPrev());
                }
                else {
                    this.id = this.options.id || this.element.id || leeUI.getId(this.__idPrev());
                }

            }
            //存入管理器池
            leeUI.add(this);

            if (!this.element) return;

            //读取attr方法,并加载到参数,比如['url']
            var attributes = this.attr();
            if (attributes && attributes instanceof Array) {
                for (var i = 0; i < attributes.length; i++) {
                    var name = attributes[i];
                    if ($(this.element).attr(name)) {
                        this.options[name] = $(this.element).attr(name); //设置属性
                    }
                }
            }
            //读取ligerui这个属性，并加载到参数，比如 ligerui = "width:120,heigth:100"
            var p = this.options;
            if ($(this.element).attr("leeUI")) {
                try {
                    var attroptions = $(this.element).attr("leeUI");
                    if (attroptions.indexOf('{') != 0) attroptions = "{" + attroptions + "}";
                    eval("attroptions = " + attroptions + ";");
                    if (attroptions) $.extend(p, attroptions);
                } catch (e) { }
            }

            //v1.3.2增加 从data-XX 加载属性
            function loadDataOp(control, jelement) {
                var op = {};
                if (!control || control.indexOf('.') != -1) return op;
                var defaultOp = leeUI.defaults[control];
                if (!defaultOp) return op;
                for (var name in defaultOp) {
                    if (jelement.attr(leeUI.attrPrev + "-" + name)) {
                        var value = jelement.attr(leeUI.attrPrev + "-" + name);
                        if (typeof (defaultOp[name]) == "boolean") {
                            op[name] = value == "true" || value == "1";
                        } else {
                            op[name] = value;
                        }
                    }
                }
                return op;
            }

            $.extend(p, loadDataOp(this.__getType(), $(this.element)));

        },
        //预渲染,可以用于继承扩展
        _preRender: function () {

        },
        _render: function () {

        },
        _rendered: function () {
            if (this.element) {
                if (this._isExternal) return;
                $(this.element).attr("uiid", this.id);
            }
        },
        _setCls: function (value) {
            if (this.element && value) {
                $(this.element).addClass(value);
            }
        },
        //返回要转换成ligerui参数的属性,比如['url']
        attr: function () {
            return [];
        },
        destroy: function () {
            if (this.element) {
                $(this.element).remove();
            }
            this.options = null;
            leeUI.remove(this);
        }
    });

    //表单控件基类
    leeUI.controls.Input = function (element, options) {
        //执行父类构造函数
        leeUI.controls.Input.base.constructor.call(this, element, options);
    };

    leeUI.controls.Input.leeExtend(leeUI.core.UIComponent, {
        __getType: function () {
            return 'leeUI.controls.Input';
        },
        attr: function () {
            return ['nullText'];
        },
        setValue: function (value) {
            return this.set('value', value);
        },
        getValue: function () {
            return this.get('value');
        },
        //设置只读
        _setReadonly: function (readonly) {
            var wrapper = this.wrapper || this.text;
            if (!wrapper || !wrapper.hasClass("l-text")) return;
            var inputText = this.inputText;
            if (readonly) {
                if (inputText) inputText.attr("readonly", "readonly");
                wrapper.addClass("l-text-readonly");
            } else {
                if (inputText) inputText.removeAttr("readonly");
                wrapper.removeClass("l-text-readonly");
            }
        },
        setReadonly: function (readonly) {
            return this.set('readonly', readonly);
        },
        setEnabled: function () {
            return this.set('disabled', false);
        },
        setDisabled: function () {
            return this.set('disabled', true);
        },
        updateStyle: function () {

        },
        resize: function (width, height) {
            //alert("resize");
            this.set({
                width: width - 2,
                height: height + 2
            });
        }
    });

    leeUI.draggable = {
        dragging: false
    };

    leeUI.resizable = {
        reszing: false
    };

    //获取 默认的编辑构造器
    leeUI.getEditor = function (e) {
        var type = e.type,
            control = e.control,
            master = e.master;
        if (!type) return null;
        var inputTag = 0;
        if (control) control = control.substr(0, 1).toUpperCase() + control.substr(1); //控件类型 首字母大写
        var defaultOp = {
            create: function (container, editParm, controlOptions) {
                //field in form , column in grid
                var field = editParm.field || editParm.column,
                    options = controlOptions || {};
                var isInGrid = editParm.column ? true : false;
                var p = $.extend({}, e.options);
                var inputType = "text";
                if ($.inArray(type, ["password", "file", "checkbox", "radio"]) != -1) inputType = type;
                if (e.password) inputType = "password";
                var inputBody = $("<input id='txt_grid_" + editParm.column.id + "' type='" + inputType + "'/>");
                if (e.body) {
                    inputBody = e.body.clone();
                }
                inputBody.appendTo(container);
                if (editParm.field) {
                    var txtInputName = field.name;
                    var prefixID = $.isFunction(options.prefixID) ? options.prefixID(master) : (options.prefixID || "");
                    p.id = field.id || (prefixID + field.name);
                    if ($.inArray(type, ["select", "combobox", "autocomplete", "popup"]) != -1) {
                        txtInputName = field.textField || field.comboboxName;
                        if (field.comboboxName && !field.id)
                            p.id = (options.prefixID || "") + field.comboboxName;
                    }
                    if ($.inArray(type, ["select", "combobox", "autocomplete", "popup", "radiolist", "checkboxlist", "listbox"]) != -1) {
                        p.valueFieldID = prefixID + field.name;
                    }
                    if (!e.body) {
                        var inputName = prefixID + txtInputName;
                        var inputId = new Date().getTime() + "_" + ++inputTag + "_" + field.name;
                        inputBody.attr($.extend({
                            id: inputId,
                            name: inputName
                        }, field.attr));
                        if (field.cssClass) {
                            inputBody.addClass(field.cssClass);
                        }
                        if (field.validate && !master.options.unSetValidateAttr) {
                            inputBody.attr('validate', leeUI.toJSON(field.validate));
                        }
                    }
                    $.extend(p, field.options);
                }
                if (field.dictionary) //字典字段，比如:男|女
                {
                    field.editor = field.editor || {};
                    if (!field.editor.data) {
                        var dicEditorData = [],
                            dicItems = field.dictionary.split('|');
                        $(dicItems).each(function (i, dicItem) {
                            var dics = dicItem.split(',');
                            var dicItemId = dics[0],
                                dicItemText = dics.length >= 2 ? dics[1] : dics[0];
                            dicEditorData.push({
                                id: dicItemId,
                                value: dicItemId,
                                text: dicItemText
                            });
                        });
                        field.editor.data = dicEditorData;
                    }
                }
                if (field.editor) {
                    if (field.editor.options) {
                        $.extend(p, field.editor.options);
                        delete field.editor.options;
                    }
                    if (field.editor.valueColumnName) {
                        p.valueField = field.editor.valueColumnName;
                        delete field.editor.valueColumnName;
                    }
                    if (field.editor.displayColumnName) {
                        p.textField = field.editor.displayColumnName;
                        delete field.editor.displayColumnName;
                    }
                    //可扩展参数,支持动态加载
                    var ext = field.editor.p || field.editor.ext;
                    if (ext) {
                        ext = typeof (ext) == 'function' ? ext(editParm) : ext;
                        $.extend(p, ext);
                        delete field.editor.p;
                        delete field.editor.ext;
                    }
                    $.extend(p, field.editor);
                }

                if (isInGrid) {
                    p.host_grid = this;
                    p.host_grid_row = editParm.record;
                    p.host_grid_column = editParm.column;
                    p.gridEditParm = editParm;
                } else {
                    p.host_form = this;

                    if (field.readonly || p.host_form.get('readonly')) {
                        p.readonly = true;
                    }
                }
                //返回的是ligerui对象
                var lobj = inputBody['lee' + control](p);
                if (isInGrid) {
                    setTimeout(function () {
                        inputBody.focus();
                        inputBody.select();
                        //inputBody.click();
                        if (control == "DropDown") {
                            inputBody.leeUI()._toggleSelectBox(false);
                        }
                    }, 100);
                }
                return lobj;
            },
            getValue: function (editor, editParm) {
                var field = editParm.field || editParm.column;
                if (editor.getValue) {
                    var value = editor.getValue();
                    var edtirType = editParm.column ? editParm.column.editor.type : editParm.field.type;
                    //isArrayValue属性可将提交字段数据改成[id1,id2,id3]的形式
                    if (field && field.editor && field.editor.isArrayValue && value) {
                        value = value.split(';');
                    }
                    //isRef属性可将提交字段数据改成[id,value]的形式 值 名称
                    if (field && field.editor && field.editor.isRef && editor.getText) {
                        value = [value, editor.getText()];
                    }
                    //isRefMul属性可将提交字段数据改成[[id1,value1],[id2,value2]]的形式
                    if (field && field.editor && field.editor.isRefMul && editor.getText) {
                        var vs = value.split(';');
                        var ts = editor.getText().split(';');
                        value = [];
                        for (var i = 0; i < vs.length; i++) {
                            value.push([vs[i], ts[i]]);
                        }
                    }
                    if (edtirType == "int" || edtirType == "digits") {
                        value = value ? parseInt(value, 10) : 0;
                    } else if (edtirType == "float" || edtirType == "number") {
                        value = value ? parseFloat(value) : 0;
                    }
                    return value;
                }
            },
            setValue: function (editor, value, editParm) {
                var field = editParm.field || editParm.column;
                if (editor.setValue) {

                    if (editor.type == "Lookup") {
                        editor.setValue(value, value);
                        return;
                    }
                    //设置了isArrayValue属性- 如果获取到的数据是[id1,id2,id3]的形式，需要合并为一个完整字符串
                    if (field && field.editor && field.editor.isArrayValue && value) {
                        value = value.join(';');
                    }
                    //设置了isRef属性-如果获取到的数据是[id,text]的形式，需要获取[0]
                    if (field && field.editor && field.editor.isRef && $.isArray(value)) {
                        value = value[0];
                    }
                    //设置了isRefMul属性- 获取到[[id1,value1],[id2,value2]]的形式，需要合并为一个完整字符串
                    if (field && field.editor && field.editor.isRefMul && $.isArray(value)) {
                        var vs = [];
                        for (var i = 0; i < value.length; i++) {
                            vs.push(value[i].length > 1 ? value[i][1] : value[i][0]);
                        }
                        value = vs.join(';');
                    }
                    editor.setValue(value);
                }
            },
            //从控件获取到文本信息
            getText: function (editor, editParm) {
                var field = editParm.field || editParm.column;
                if (editor.getText) {
                    var text = editor.getText();
                    return text;
                }
            },
            //设置文本信息到控件去
            setText: function (editor, text, editParm) {
                if (text && editor.setText) {
                    editor.setText(text);
                }
                //如果没有把数据保存到 textField 字段，那么需要获取值字段
                else {
                    var field = editParm.field || editParm.column;
                    text = editor.setValue() || editParm.value || "";
                    //如果获取到的数据是[id,text]的形式，需要获取[0]
                    if (field && field.editor && field.editor.isRef && $.isArray(text) && text.length > 1) {
                        text = text[1];
                    }
                    //在grid的编辑里面 获取到[[id1,value1],[id2,value2]]的形式，需要合并为一个完整字符串
                    if (field && field.editor && field.editor.isRefMul && $.isArray(text) && text.length > 1) {
                        var vs = [];
                        for (var i = 0; i < text.length; i++) {
                            vs.push(text[1]);
                        }
                        text = vs.join(';');
                    }
                    if (editor.setText) {
                        editor.setText(text);
                    }
                }
            },
            getSelected: function (editor, editParm) {
                if (editor.getSelected) {
                    return editor.getSelected();
                }
            },
            resize: function (editor, width, height, editParm) {
                if (editParm.field) width = width - 2;
                if (editor.resize) editor.resize(width, height);
            },
            setEnabled: function (editor, isEnabled) {
                if (isEnabled) {
                    if (editor.setEnabled) editor.setEnabled();
                } else {
                    if (editor.setDisabled) editor.setDisabled();
                }
            },
            destroy: function (editor, editParm) {
                if (editor.destroy) editor.destroy();
            }
        };

        return $.extend({}, defaultOp, leeUI.editorCreatorDefaults || {}, e);
    }

    leeUI.win = {

        mid: 0,

        getMaskID: function () {
            this.mid += 10;
            return this.mid;
        },
        //顶端显示
        top: false,
        maskid: function (zindex) {

            var ele = $("#lee-mask-" + zindex);
            if (ele.length > 0) return;
            var style = "";
            if (zindex) {
                style = "z-index:" + String(9000 + Number(zindex)) + ";";
            }
            var windowMask = $("<div id='lee-mask-" + (zindex ? zindex : "") + "' class='lee-window-mask' style='display: block;" + style + "'></div>").appendTo('body');
        },
        unmaskid: function (id) {

            if (id) {
                $("#lee-mask-" + id).remove();
            }
        },
        //遮罩
        mask: function (win) {

            function setHeight() {
                if (!leeUI.win.windowMask) return;
                var h = $(window).height() + $(window).scrollTop();
                leeUI.win.windowMask.height(h);
            }
            if (!this.windowMask) {
                this.windowMask = $("<div  class='lee-window-mask' style='display: block;'></div>").appendTo('body');
                $(window).bind('resize.ligeruiwin', setHeight);
                $(window).bind('scroll', setHeight);
            }
            this.windowMask.show();
            setHeight();
            this.masking = true;
        },

        //取消遮罩
        unmask: function (win) {
            var jwins = $("body > .lee-dialog:visible,body > .lee-window:visible");
            for (var i = 0, l = jwins.length; i < l; i++) {
                var winid = jwins.eq(i).attr("uiid");
                if (win && win.id == winid) continue;
                //获取ligerui对象
                var winmanager = leeUI.get(winid);
                if (!winmanager) continue;
                //是否模态窗口
                var modal = winmanager.get('modal');
                //如果存在其他模态窗口，那么不会取消遮罩
                if (modal) return;
            }
            if (this.windowMask)
                this.windowMask.hide();
            this.masking = false;
        },

        //显示任务栏
        createTaskbar: function () {
            if (!this.taskbar) {
                this.taskbar = $('<div class="l-taskbar"><div class="l-taskbar-tasks"></div><div class="l-clear"></div></div>').appendTo('body');
                if (this.top) this.taskbar.addClass("l-taskbar-top");
                this.taskbar.tasks = $(".l-taskbar-tasks:first", this.taskbar);
                this.tasks = {};
            }
            this.taskbar.show();
            this.taskbar.animate({
                bottom: 0
            });
            return this.taskbar;
        },

        //关闭任务栏
        removeTaskbar: function () {
            var self = this;
            self.taskbar.animate({
                bottom: -32
            }, function () {
                self.taskbar.remove();
                self.taskbar = null;
            });
        },
        activeTask: function (win) {
            for (var winid in this.tasks) {
                var t = this.tasks[winid];
                if (winid == win.id) {
                    t.addClass("l-taskbar-task-active");
                } else {
                    t.removeClass("l-taskbar-task-active");
                }
            }
        },

        //获取任务
        getTask: function (win) {
            var self = this;
            if (!self.taskbar) return;
            if (self.tasks[win.id]) return self.tasks[win.id];
            return null;
        },

        //增加任务
        addTask: function (win) {
            var self = this;
            if (!self.taskbar) self.createTaskbar();
            if (self.tasks[win.id]) return self.tasks[win.id];
            var title = win.get('title');
            var task = self.tasks[win.id] = $('<div class="l-taskbar-task"><div class="l-taskbar-task-icon"></div><div class="l-taskbar-task-content">' + title + '</div></div>');
            self.taskbar.tasks.append(task);
            self.activeTask(win);
            task.bind('click', function () {
                self.activeTask(win);
                if (win.actived)
                    win.min();
                else
                    win.active();
            }).hover(function () {
                $(this).addClass("l-taskbar-task-over");
            }, function () {
                $(this).removeClass("l-taskbar-task-over");
            });
            return task;
        },

        hasTask: function () {
            for (var p in this.tasks) {
                if (this.tasks[p])
                    return true;
            }
            return false;
        },

        //移除任务
        removeTask: function (win) {
            var self = this;
            if (!self.taskbar) return;
            if (self.tasks[win.id]) {
                self.tasks[win.id].unbind();
                self.tasks[win.id].remove();
                delete self.tasks[win.id];
            }
            if (!self.hasTask()) {
                self.removeTaskbar();
            }
        },
        //前端显示
        setFront: function (win) {
            if (win.options.coverMode) return;
            var wins = leeUI.find(leeUI.core.Win);
            for (var i in wins) {
                var w = wins[i];
                if (w == win) {
                    $(w.element).css("z-index", "9200");
                    this.activeTask(w);
                } else {
                    $(w.element).css("z-index", "9100");
                }
            }
        }
    };

    //窗口基类 window、dialog
    leeUI.core.Win = function (element, options) {
        leeUI.core.Win.base.constructor.call(this, element, options);
    };

    leeUI.core.Win.leeExtend(leeUI.core.UIComponent, {
        __getType: function () {
            return 'leeUI.controls.Win';
        },
        mask: function (id) {
            // 遮罩

            if (this.options.modal) {
                if (id) {
                    leeUI.win.maskid(id);
                } else {
                    leeUI.win.mask(this);
                }
            }
        },
        unmask: function (id) {
            if (this.options.modal) {
                if (id) {
                    leeUI.win.unmaskid(id);
                } else {
                    leeUI.win.unmask(this);
                }
            }
        },
        min: function () { },
        max: function () { },
        active: function () { }
    });

    //这里指定编辑器类型
    leeUI.editors = {
        "text": {
            control: 'TextBox'
        },
        "dropdown": {
            control: 'DropDown'
        },
        "lookup": {
            control: 'Lookup'
        },
        "popup": {
            control: 'Popup'
        },
        "spinner": {
            control: 'Spinner'
        },
        "number": {
            control: 'TextBox'
        },
        "date": {
            control: 'Date'
        }
    }
    //grid 扩展编辑器的时候回自动给editor的options加上gridEditParm


})(jQuery);

$.browser = {};
$.browser.mozilla = /firefox/.test(navigator.userAgent.toLowerCase());
$.browser.webkit = /webkit/.test(navigator.userAgent.toLowerCase());
$.browser.opera = /opera/.test(navigator.userAgent.toLowerCase());
$.browser.msie = /msie/.test(navigator.userAgent.toLowerCase());



+
    function ($) {
        'use strict';

        // CSS TRANSITION SUPPORT (Shoutout: http://www.modernizr.com/)
        // ============================================================

        function transitionEnd() {
            var el = document.createElement('bootstrap')

            var transEndEventNames = {
                WebkitTransition: 'webkitTransitionEnd',
                MozTransition: 'transitionend',
                OTransition: 'oTransitionEnd otransitionend',
                transition: 'transitionend'
            }

            for (var name in transEndEventNames) {
                if (el.style[name] !== undefined) {
                    return {
                        end: transEndEventNames[name]
                    }
                }
            }

            return false // explicit for ie8 (  ._.)
        }

        // http://blog.alexmaccaw.com/css-transitions
        $.fn.emulateTransitionEnd = function (duration) {
            var called = false
            var $el = this
            $(this).one('bsTransitionEnd', function () {
                called = true
            })
            var callback = function () {
                if (!called) $($el).trigger($.support.transition.end)
            }
            setTimeout(callback, duration)
            return this
        }

        $(function () {
            $.support.transition = transitionEnd()

            if (!$.support.transition) return

            $.event.special.bsTransitionEnd = {
                bindType: $.support.transition.end,
                delegateType: $.support.transition.end,
                handle: function (e) {
                    if ($(e.target).is(this)) return e.handleObj.handler.apply(this, arguments)
                }
            }
        })

    }(jQuery);
(function ($) {

    $.fn.leeDrag = function (options) {
        return $.leeUI.run.call(this, "leeUIDrag", arguments, {
            idAttrName: 'dragid',
            hasElement: false,
            propertyToElemnt: 'target'
        });
        //drag 和resize比较特殊 会把 dragid resizeid挂到 dom下的一个属性上 目前是挂在到 
        //this.options.target指向的是自身的element 这里可以缓存属性
    };

    $.fn.leeGetDragManager = function () {
        return $.leeUI.run.call(this, "leeUIGetDragManager", arguments, {
            idAttrName: 'dragid',
            hasElement: false,
            propertyToElemnt: 'target'
        });
    };

    $.leeUIDefaults.Drag = {
        onStartDrag: false,
        onDrag: false,
        onStopDrag: false,
        handler: null,
        //鼠标按下再弹起，如果中间的间隔小于[dragDelay]毫秒，那么认为是点击，不会进行拖拽操作
        clickDelay: 100,
        //代理 拖动时的主体,可以是'clone'或者是函数,放回jQuery 对象
        proxy: true,
        revert: false,
        animate: true,
        onRevert: null,
        onEndRevert: null,
        //接收区域 jQuery对象或者jQuery选择字符
        receive: null,
        //进入区域
        onDragEnter: null,
        //在区域移动
        onDragOver: null,
        //离开区域
        onDragLeave: null,
        //在区域释放
        onDrop: null,
        disabled: false,
        proxyX: null, //代理相对鼠标指针的位置,如果不设置则对应target的left
        proxyY: null
    };

    $.leeUI.controls.Drag = function (options) {
        $.leeUI.controls.Drag.base.constructor.call(this, null, options);
    };

    $.leeUI.controls.Drag.leeExtend($.leeUI.core.UIComponent, {
        __getType: function () {
            return 'Drag';
        },
        __idPrev: function () {
            return 'Drag';
        },
        _render: function () {
            var g = this,
                p = this.options;
            this.set(p);
            g.cursor = "move";
            g.handler.css('cursor', g.cursor);
            g.mouseDowned = false;
            g.handler.bind('mousedown.drag', function (e) {
                if (p.disabled) return;

                if (e.button == 2) return;
                g.mouseDowned = true;
                $(document).bind("selectstart.drag", function () { return false; });
                setTimeout(function () {
                    //如果过了N毫秒,鼠标还没有弹起来，才认为是启动drag
                    if (g.mouseDowned) {
                        g._start.call(g, e);
                    }
                }, p.clickDelay || 100);
            }).bind('mousemove.drag', function () {
                if (p.disabled) return;
                g.handler.css('cursor', g.cursor);
            }).bind('mouseup.drag', function () {

                $(document).unbind("selectstart.drag");
            });

            $(document).bind('mouseup', function () {
                g.mouseDowned = false;
            });
        },
        _rendered: function () {
            this.options.target.dragid = this.id;
        },
        _start: function (e) {
            var g = this,
                p = this.options;
            if (g.reverting) return;
            if (p.disabled) return;
            g.current = {
                target: g.target,
                left: g.target.offset().left,
                top: g.target.offset().top,
                startX: e.pageX || e.screenX,
                startY: e.pageY || e.clientY
            };
            if (g.trigger('startDrag', [g.current, e]) == false) return false;
            g.cursor = "move";
            g._createProxy(p.proxy, e);
            //代理没有创建成功
            if (p.proxy && !g.proxy) return false;
            (g.proxy || g.handler).css('cursor', g.cursor);
            $(document).bind('mousemove.drag', function () {
                g._drag.apply(g, arguments);
            });
            $.leeUI.draggable.dragging = true;
            $(document).bind('mouseup.drag', function () {
                $.leeUI.draggable.dragging = false;

                g._stop.apply(g, arguments);
            });
        },
        _drag: function (e) {
            var g = this,
                p = this.options;
            if (!g.current) return;
            var pageX = e.pageX || e.screenX;
            var pageY = e.pageY || e.screenY;
            g.current.diffX = pageX - g.current.startX;
            g.current.diffY = pageY - g.current.startY;
            (g.proxy || g.handler).css('cursor', g.cursor);
            if (g.receive) {
                g.receive.each(function (i, obj) {
                    var receive = $(obj);
                    var xy = receive.offset();
                    if (pageX > xy.left && pageX < xy.left + receive.width() &&
                        pageY > xy.top && pageY < xy.top + receive.height()) {
                        if (!g.receiveEntered[i]) {
                            g.receiveEntered[i] = true;
                            g.trigger('dragEnter', [obj, g.proxy || g.target, e]);
                        } else {
                            g.trigger('dragOver', [obj, g.proxy || g.target, e]);
                        }
                    } else if (g.receiveEntered[i]) {
                        g.receiveEntered[i] = false;
                        g.trigger('dragLeave', [obj, g.proxy || g.target, e]);
                    }
                });
            }
            if (g.hasBind('drag')) {
                if (g.trigger('drag', [g.current, e]) != false) {

                } else {
                    if (g.proxy) {
                        //g._removeProxy();
                    } else {

                        //g._stop();
                    }
                }
                g._applyDrag();
            } else {
                g._applyDrag();
            }
        },
        _stop: function (e) {
            var g = this,
                p = this.options;
            $(document).unbind('mousemove.drag');
            $(document).unbind('mouseup.drag');
            $(document).unbind("selectstart.drag");
            if (g.receive) {
                g.receive.each(function (i, obj) {
                    if (g.receiveEntered[i]) {
                        g.trigger('drop', [obj, g.proxy || g.target, e]);
                    }
                });
            }
            if (g.proxy) {
                if (p.revert) {
                    if (g.hasBind('revert')) {
                        if (g.trigger('revert', [g.current, e]) != false)
                            g._revert(e);
                        else
                            g._removeProxy();
                    } else {
                        g._revert(e);
                    }
                } else {
                    g._applyDrag(g.target);
                    g._removeProxy();
                }
            }
            g.cursor = 'move';
            g.trigger('stopDrag', [g.current, e]);
            g.current = null;
            g.handler.css('cursor', g.cursor);

        },
        _revert: function (e) {

            var g = this;
            g.reverting = true;
            g.proxy.animate({
                left: g.current.left,
                top: g.current.top
            }, function () {
                g.reverting = false;
                g._removeProxy();
                g.trigger('endRevert', [g.current, e]);
                g.current = null;
            });
        },
        _applyDrag: function (applyResultBody) {
            var g = this,
                p = this.options;
            applyResultBody = applyResultBody || g.proxy || g.target;
            var cur = {},
                changed = false;
            var noproxy = applyResultBody == g.target;
            if (g.current.diffX) {
                if (noproxy || p.proxyX == null)
                    cur.left = g.current.left + g.current.diffX;
                else
                    cur.left = g.current.startX + p.proxyX + g.current.diffX;
                changed = true;
            }
            if (g.current.diffY) {
                if (noproxy || p.proxyY == null)
                    cur.top = g.current.top + g.current.diffY;
                else
                    cur.top = g.current.startY + p.proxyY + g.current.diffY;
                changed = true;
            }
            if (applyResultBody == g.target && g.proxy && p.animate) {
                g.reverting = true;
                applyResultBody.animate(cur, function () {
                    g.reverting = false;
                });
            } else {
                //这里处理是否能拉伸出去
                //				if(cur.top <= 0) {
                //					cur.top = 0;
                //				}
                //				if(cur.left <= 0) {
                //					cur.left = 0;
                //				}
                //				if(cur.top >= $(document).height() - $(applyResultBody).height()-3) {
                //					cur.top = $(document).height() - $(applyResultBody).height()-3;
                //				}
                //				if(cur.left >= $(document).width() - $(applyResultBody).width()-3) {
                //					cur.left = $(document).width() - $(applyResultBody).width()-3
                //				}
                applyResultBody.css(cur);
            }
        },
        _setReceive: function (receive) {
            this.receiveEntered = {};
            if (!receive) return;
            if (typeof receive == 'string')
                this.receive = $(receive);
            else
                this.receive = receive;
        },
        _setHandler: function (handler) {
            var g = this,
                p = this.options;
            if (!handler)
                g.handler = $(p.target);
            else
                g.handler = (typeof handler == 'string' ? $(handler, p.target) : handler);
        },
        _setTarget: function (target) {
            this.target = $(target);
        },
        _setCursor: function (cursor) {
            this.cursor = cursor;
            (this.proxy || this.handler).css('cursor', cursor);
        },
        _createProxy: function (proxy, e) {
            if (!proxy) return;
            var g = this,
                p = this.options;
            if (typeof proxy == 'function') {
                g.proxy = proxy.call(this.options.target, g, e);
            } else if (proxy == 'clone') {
                g.proxy = g.target.clone().css('position', 'absolute');
                g.proxy.appendTo('body');
            } else {
                g.proxy = $("<div class='lee-draggable'></div>");
                g.proxy.width(g.target.width()).height(g.target.height())
                g.proxy.attr("dragid", g.id).appendTo('body');
            }
            g.proxy.css({
                left: p.proxyX == null ? g.current.left : g.current.startX + p.proxyX,
                top: p.proxyY == null ? g.current.top : g.current.startY + p.proxyY
            }).show();
        },
        _removeProxy: function () {
            var g = this;
            if (g.proxy) {
                g.proxy.remove();
                g.proxy = null;
            }
        }

    });

})(jQuery);
/**
 
*/
(function ($) {
    $.fn.leeResizable = function (options) {
        return $.leeUI.run.call(this, "leeUIResizable", arguments,
            {
                idAttrName: 'resizableid',
                hasElement: false,
                propertyToElemnt: 'target'
            });
    };

    $.fn.leeGetResizableManager = function () {
        return $.leeUI.run.call(this, "leeUIGetResizableManager", arguments,
            {
                idAttrName: 'resizableid',
                hasElement: false,
                propertyToElemnt: 'target'
            });
    };


    $.leeUIDefaults.Resizable = {
        handles: 'n, e, s, w, ne, se, sw, nw',
        maxWidth: 2000,
        maxHeight: 2000,
        minWidth: 20,
        minHeight: 20,
        scope: 3,
        animate: false,
        onStartResize: function (e) { },
        onResize: function (e) { },
        onStopResize: function (e) { },
        onEndResize: null
    };

    $.leeUI.controls.Resizable = function (options) {
        $.leeUI.controls.Resizable.base.constructor.call(this, null, options);
    };

    $.leeUI.controls.Resizable.leeExtend($.leeUI.core.UIComponent, {
        __getType: function () {
            return 'Resizable';
        },
        __idPrev: function () {
            return 'Resizable';
        },
        _render: function () {
            var g = this, p = this.options;
            g.target = $(p.target);
            g.set(p);

            g.target.mousemove(function (e) {
                if (p.disabled) return;
                g.dir = g._getDir(e);
                if (g.dir)
                    g.target.css('cursor', g.dir + '-resize');
                else if (g.target.css('cursor').indexOf('-resize') > 0)
                    g.target.css('cursor', 'default');
                if (p.target.dragid) {
                    var drag = $.leeUI.get(p.target.dragid);
                    if (drag && g.dir) {
                        drag.set('disabled', true);
                    } else if (drag) {
                        drag.set('disabled', false);
                    }
                }
            }).mousedown(function (e) {
                if (p.disabled) return;
                if (g.dir) {
                    g._start(e);
                }
            });
        },
        _rendered: function () {
            this.options.target.resizableid = this.id;
        },
        _getDir: function (e) {
            var g = this, p = this.options;
            var dir = '';
            var xy = g.target.offset();
            var width = g.target.width();
            var height = g.target.height();
            var scope = p.scope;
            var pageX = e.pageX || e.screenX;
            var pageY = e.pageY || e.screenY;
            if (pageY >= xy.top && pageY < xy.top + scope) {
                dir += 'n';
            }
            else if (pageY <= xy.top + height && pageY > xy.top + height - scope) {
                dir += 's';
            }
            if (pageX >= xy.left && pageX < xy.left + scope) {
                dir += 'w';
            }
            else if (pageX <= xy.left + width && pageX > xy.left + width - scope) {
                dir += 'e';
            }
            if (p.handles == "all" || dir == "") return dir;
            if ($.inArray(dir, g.handles) != -1) return dir;
            return '';
        },
        _setHandles: function (handles) {
            if (!handles) return;
            this.handles = handles.replace(/(\s*)/g, '').split(',');
        },
        _createProxy: function () {
            var g = this;
            g.proxy = $('<div class="lee-resizable"></div>');
            g.proxy.width(g.target.width()).height(g.target.height())
            g.proxy.attr("resizableid", g.id).appendTo('body');
        },
        _removeProxy: function () {
            var g = this;
            if (g.proxy) {
                g.proxy.remove();
                g.proxy = null;
            }
        },
        _start: function (e) {
            var g = this, p = this.options;
            g._createProxy();
            g.proxy.css({
                left: g.target.offset().left,
                top: g.target.offset().top,
                position: 'absolute'
            });
            g.current = {
                dir: g.dir,
                left: g.target.offset().left,
                top: g.target.offset().top,
                startX: e.pageX || e.screenX,
                startY: e.pageY || e.clientY,
                width: g.target.width(),
                height: g.target.height()
            };
            $(document).bind("selectstart.resizable", function () { return false; });
            $(document).bind('mouseup.resizable', function () {
                g._stop.apply(g, arguments);
            });
            $(document).bind('mousemove.resizable', function () {
                g._drag.apply(g, arguments);
            });
            g.proxy.show();
            g.trigger('startResize', [g.current, e]);
        },
        changeBy: {
            t: ['n', 'ne', 'nw'],
            l: ['w', 'sw', 'nw'],
            w: ['w', 'sw', 'nw', 'e', 'ne', 'se'],
            h: ['n', 'ne', 'nw', 's', 'se', 'sw']
        },
        _drag: function (e) {
            var g = this, p = this.options;
            if (!g.current) return;
            if (!g.proxy) return;
            g.proxy.css('cursor', g.current.dir == '' ? 'default' : g.current.dir + '-resize');
            var pageX = e.pageX || e.screenX;
            var pageY = e.pageY || e.screenY;
            g.current.diffX = pageX - g.current.startX;
            g.current.diffY = pageY - g.current.startY;
            g._applyResize(g.proxy);
            g.trigger('resize', [g.current, e]);
        },
        _stop: function (e) {
            var g = this, p = this.options;
            if (g.hasBind('stopResize')) {
                if (g.trigger('stopResize', [g.current, e]) != false)
                    g._applyResize();
            }
            else {
                g._applyResize();
            }
            g._removeProxy();
            g.trigger('endResize', [g.current, e]);
            $(document).unbind("selectstart.resizable");
            $(document).unbind('mousemove.resizable');
            $(document).unbind('mouseup.resizable');
        },
        _applyResize: function (applyResultBody) {
            var g = this, p = this.options;
            var cur = {
                left: g.current.left,
                top: g.current.top,
                width: g.current.width,
                height: g.current.height
            };
            var applyToTarget = false;
            if (!applyResultBody) {
                applyResultBody = g.target;
                applyToTarget = true;
                if (!isNaN(parseInt(g.target.css('top'))))
                    cur.top = parseInt(g.target.css('top'));
                else
                    cur.top = 0;
                if (!isNaN(parseInt(g.target.css('left'))))
                    cur.left = parseInt(g.target.css('left'));
                else
                    cur.left = 0;
            }
            if ($.inArray(g.current.dir, g.changeBy.l) > -1) {
                cur.left += g.current.diffX;
                g.current.diffLeft = g.current.diffX;

            }
            else if (applyToTarget) {
                delete cur.left;
            }
            if ($.inArray(g.current.dir, g.changeBy.t) > -1) {
                cur.top += g.current.diffY;
                g.current.diffTop = g.current.diffY;
            }
            else if (applyToTarget) {
                delete cur.top;
            }
            if ($.inArray(g.current.dir, g.changeBy.w) > -1) {
                cur.width += (g.current.dir.indexOf('w') == -1 ? 1 : -1) * g.current.diffX;
                g.current.newWidth = cur.width;
            }
            else if (applyToTarget) {
                delete cur.width;
            }
            if ($.inArray(g.current.dir, g.changeBy.h) > -1) {
                cur.height += (g.current.dir.indexOf('n') == -1 ? 1 : -1) * g.current.diffY;
                g.current.newHeight = cur.height;
            }
            else if (applyToTarget) {
                delete cur.height;
            }
            if (applyToTarget && p.animate)
                applyResultBody.animate(cur);
            else
                applyResultBody.css(cur);
        }
    });



})(jQuery);
(function ($) {
    //气泡,可以在制定位置显示
    $.fn.LeeToolTip = function (p) {
        return $.leeUI.run.call(this, "leeUIToolTip", arguments, {
            idAttrName: "tooltipid"
        });
    };
    $.leeUIDefaults = $.leeUIDefaults || {};
    //气泡
    $.leeUIDefaults.ToolTip = {
        animation: true,
        placement: 'right',
        selector: false,
        template: '<div class="lee-tooltip"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>',
        trigger: 'hover focus',
        title: '',
        css: '',
        delay: 0,
        html: false,
        container: true,
        onEnter: null,
        viewport: {
            selector: 'body',
            padding: 0
        }
    };
    $.leeUI.controls.ToolTip = function (element, options) {
        $.leeUI.controls.ToolTip.base.constructor.call(this, element, options);
    };

    $.leeUI.controls.ToolTip.leeExtend($.leeUI.core.UIComponent, {
        __getType: function () {
            return 'ToolTip';
        },
        __idPrev: function () {
            return 'ToolTip';
        },
        _extendMethods: function () {
            return {};
        },
        _isExternal: function () {
            return true;
        },
        _render: function () {
            var g = this,
                p = this.options;
            g.enabled = true;
            g.type = "tooltip";
            g.$element = $(this.element);
            //g.$element.attr("tooltipid", "ToolTip" + g.$element.attr("id"));
            g.$viewport = this.options.viewport && $($.isFunction(this.options.viewport) ? this.options.viewport.call(this, this.$element) : (this.options.viewport.selector || this.options.viewport));
            g.inState = { click: false, hover: false, focus: false };

            var triggers = p.trigger.split(' ')

            for (var i = triggers.length; i--;) {
                var trigger = triggers[i]

                if (trigger == 'click') {
                    g.$element.on('click.' + g.type, p.selector, $.proxy(g.toggle, this));
                } else if (trigger != 'manual') {
                    var eventIn = trigger == 'hover' ? 'mouseenter' : 'focusin'
                    var eventOut = trigger == 'hover' ? 'mouseleave' : 'focusout'

                    g.$element.on(eventIn + '.' + g.type, p.selector, $.proxy(this.enter, this))
                    g.$element.on(eventOut + '.' + g.type, p.selector, $.proxy(this.leave, this))
                }
            }
            p.selector ?
                (p = $.extend({}, p, { trigger: 'manual', selector: '' })) :
                g.fixTitle();
            g.set(p);
            g.$element.data("tooltip", g);
        },
        _setContent: function (content) {
            var $tip = this.tip();
            var title = this.getTitle();

            $tip.find('.tooltip-inner')[this.options.html ? 'html' : 'text'](title);
            $tip.removeClass('fade in top bottom left right');
        },
        setNewTitle: function (title) {
            var $tip = this.tip();
            $tip.find('.tooltip-inner')[this.options.html ? 'html' : 'text'](title);
        },
        getDelegateOptions: function () {
            var options = {};
            var defaults = $.leeUIDefaults.ToolTip;

            this._options && $.each(this._options, function (key, value) {
                if (defaults[key] != value) options[key] = value;
            })

            return options;
        },
        toggle: function (e) {
            var g = this,
                p = this.options;
            if (e) {
                //g = $(e.currentTarget).data('bs.' + this.type)
                if (!g) {
                    //g = new this.constructor(e.currentTarget, this.getDelegateOptions())
                    //$(e.currentTarget).data('bs.' + this.type, self)
                }
            }
            if (e) {
                g.inState.click = !g.inState.click;
                if (g.isInStateTrue()) g.enter(self);
                else g.leave(self);
            } else {
                g.tip().hasClass('in') ? g.leave(self) : g.enter(self);
            }
        },
        tip: function () {
            if (!this.$tip) {
                this.$tip = $(this.options.template)
                if (this.$tip.length != 1) {
                    throw new Error(this.type + ' `template` option must consist of exactly 1 top-level element!');
                }
            }
            console.log(this.$tip.hasClass("zoom-big-fast-enter-active"));
            return this.$tip;
        },
        getUID: function (prefix) {
            do prefix += ~~(Math.random() * 1000000);
            while (document.getElementById(prefix))
            return prefix;
        },
        enter: function (obj) {
            var self = this; //obj instanceof this.constructor ?
            //obj : $(obj.currentTarget).data('bs.' + this.type);

            /*if(!self) {
				self = new this.constructor(obj.currentTarget, this.getDelegateOptions())
				$(obj.currentTarget).data('bs.' + this.type, self);
			}*/

            if (obj instanceof $.Event) {
                self.inState[obj.type == 'focusin' ? 'focus' : 'hover'] = true;
            }

            if (self.tip().hasClass('in') || self.hoverState == 'in') {
                self.hoverState = 'in';
                return;
            }

            clearTimeout(self.timeout);

            self.hoverState = 'in';

            if (!self.options.delay || !self.options.delay.show) return self.show();

            self.timeout = setTimeout(function () {
                if (self.hoverState == 'in') self.show();
            }, self.options.delay.show);
        },
        leave: function (obj) {
            var self = this; //obj instanceof this.constructor ?
            //obj : $(obj.currentTarget).data('bs.' + this.type)

            /*if(!self) {
				self = new this.constructor(obj.currentTarget, this.getDelegateOptions());
				$(obj.currentTarget).data('bs.' + this.type, self);
			}*/

            if (obj instanceof $.Event) {
                self.inState[obj.type == 'focusout' ? 'focus' : 'hover'] = false;
            }

            if (self.isInStateTrue()) return;

            clearTimeout(self.timeout);

            self.hoverState = 'out';

            if (!self.options.delay || !self.options.delay.hide) return self.hide();

            self.timeout = setTimeout(function () {
                if (self.hoverState == 'out') self.hide();
            }, self.options.delay.hide)
        },
        hide: function (callback) {
            var g = this;
            var $tip = $(this.$tip);
            var e = $.Event('hide.bs.' + this.type)

            function complete() {
                if (g.hoverState != 'in') $tip.detach();
                if (g.$element) { // TODO: Check whether guarding this code with this `if` is really necessary.
                    g.$element.trigger('hidden.bs.' + g.type);
                }

                $tip.removeClass('zoom-big-leave zoom-big-leave-active in');
                $tip.removeClass('zoom-big-fast-enter zoom-big-fast-enter-active');
                $tip.removeClass('in');
                callback && callback();
            }

            this.$element.trigger(e);

            if (e.isDefaultPrevented()) return;

            $tip.addClass('zoom-big-leave zoom-big-leave-active');

            $.support.transition && $tip.hasClass('fade') ?
                $tip
                    .one('bsTransitionEnd', complete)
                    .emulateTransitionEnd(150) :
                complete();

            this.hoverState = null;

            return this;
        },
        destroy: function () {
            this.hide();
            this.disable();
            $(this.$tip).remove();
            this.$element.data("tooltip", null);
            leeUI.remove(this);
        },
        show: function () {
            var g = this,
                p = this.options;
            var e = $.Event('show.bs.' + g.type);
            if (g.hasContent() && g.enabled) {
                g.$element.trigger(e);
                var inDom = $.contains(this.$element[0].ownerDocument.documentElement, this.$element[0]);
                if (e.isDefaultPrevented() || !inDom) return;
                var $tip = g.tip(); //获取tip实例
                var tipId = g.getUID(g.type);
                g._setContent();
                $tip.attr('id', tipId);
                if (p.animation) $tip.addClass('fade'); //淡入css
                var placement = typeof this.options.placement == 'function' ?
                    this.options.placement.call(this, $tip[0], this.$element[0]) :
                    this.options.placement;
                var autoToken = /\s?auto?\s?/i;
                var autoPlace = autoToken.test(placement)
                if (autoPlace) placement = placement.replace(autoToken, '') || 'top';
                $tip
                    .detach()
                    .css({ top: 0, left: 0, display: 'block' })
                    .addClass(placement)
                    .data('bs.' + g.type, this);
                p.container ? $tip.appendTo("body") : $tip.insertAfter(g.$element)
                g.$element.trigger('inserted.bs.' + g.type);
                //获取位置信息
                var pos = this.getPosition();
                console.log(pos);
                var actualWidth = $tip[0].offsetWidth;
                var actualHeight = $tip[0].offsetHeight;
                if (autoPlace) {
                    var orgPlacement = placement;
                    var viewportDim = this.getPosition(this.$viewport);

                    placement = placement == 'bottom' && pos.bottom + actualHeight > viewportDim.bottom ? 'top' :
                        placement == 'top' && pos.top - actualHeight < viewportDim.top ? 'bottom' :
                            placement == 'right' && pos.right + actualWidth > viewportDim.width ? 'left' :
                                placement == 'left' && pos.left - actualWidth < viewportDim.left ? 'right' :
                                    placement;

                    $tip
                        .removeClass(orgPlacement)
                        .addClass(placement);
                }
                var calculatedOffset = this.getCalculatedOffset(placement, pos, actualWidth, actualHeight);
                this.applyPlacement(calculatedOffset, placement);
                var complete = function () {
                    var prevHoverState = g.hoverState
                    g.$element.trigger('shown.bs.' + g.type)
                    g.hoverState = null

                    if (prevHoverState == 'out') g.leave(that);
                }

                $.support.transition && g.$tip.hasClass('fade') ?
                    $tip
                        .one('bsTransitionEnd', complete)
                        .emulateTransitionEnd(150) :
                    complete();
            }
            return this;
        },
        applyPlacement: function (offset, placement) {
            var $tip = this.tip();
            var width = $tip[0].offsetWidth;
            var height = $tip[0].offsetHeight;
            // manually read margins because getBoundingClientRect includes difference
            var marginTop = parseInt($tip.css('margin-top'), 10);
            var marginLeft = parseInt($tip.css('margin-left'), 10);
            // we must check for NaN for ie 8/9
            if (isNaN(marginTop)) marginTop = 0;
            if (isNaN(marginLeft)) marginLeft = 0;

            offset.top += marginTop;
            offset.left += marginLeft;

            // $.fn.offset doesn't round pixel values
            // so we use setOffset directly with our own function B-0
            $.offset.setOffset($tip[0], $.extend({
                using: function (props) {
                    $tip.css({
                        top: Math.round(props.top),
                        left: Math.round(props.left)
                    })
                }
            }, offset), 0);

            $tip.addClass('in');

            //return;
            // check to see if placing tip in new offset caused the tip to resize itself
            var actualWidth = $tip[0].offsetWidth;
            var actualHeight = $tip[0].offsetHeight;

            if (placement == 'top' && actualHeight != height) {
                offset.top = offset.top + height - actualHeight;
            }

            var delta = this.getViewportAdjustedDelta(placement, offset, actualWidth, actualHeight);

            if (delta.left) offset.left += delta.left;
            else offset.top += delta.top;

            var isVertical = /top|bottom/.test(placement);
            var arrowDelta = isVertical ? delta.left * 2 - width + actualWidth : delta.top * 2 - height + actualHeight;
            var arrowOffsetPosition = isVertical ? 'offsetWidth' : 'offsetHeight';
            console.log(offset);
            $tip.offset(offset);
            //$tip.css({ left: offset.left, top: offset.top });
            console.log($tip.offset());
            this.replaceArrow(arrowDelta, $tip[0][arrowOffsetPosition], isVertical);
            $tip.addClass("zoom-big-fast-enter zoom-big-fast-enter-active");
        },
        replaceArrow: function (delta, dimension, isVertical) {
            this.arrow()
                .css(isVertical ? 'left' : 'top', 50 * (1 - delta / dimension) + '%')
                .css(isVertical ? 'top' : 'left', '');
        },
        getCalculatedOffset: function (placement, pos, actualWidth, actualHeight) {
            return placement == 'bottom' ? { top: pos.top + pos.height, left: pos.left + pos.width / 2 - actualWidth / 2 } :
                placement == 'top' ? { top: pos.top - actualHeight, left: pos.left + pos.width / 2 - actualWidth / 2 } :
                    placement == 'left' ? { top: pos.top + pos.height / 2 - actualHeight / 2, left: pos.left - actualWidth } :
                        /* placement == 'right' */
                        { top: pos.top + pos.height / 2 - actualHeight / 2, left: pos.left + pos.width };
        },
        getViewportAdjustedDelta: function (placement, pos, actualWidth, actualHeight) {
            var delta = { top: 0, left: 0 };
            if (!this.$viewport) return delta;

            var viewportPadding = this.options.viewport && this.options.viewport.padding || 0;
            var viewportDimensions = this.getPosition(this.$viewport);

            if (/right|left/.test(placement)) {
                var topEdgeOffset = pos.top - viewportPadding - viewportDimensions.scroll;
                var bottomEdgeOffset = pos.top + viewportPadding - viewportDimensions.scroll + actualHeight;
                if (topEdgeOffset < viewportDimensions.top) { // top overflow
                    delta.top = viewportDimensions.top - topEdgeOffset;
                } else if (bottomEdgeOffset > viewportDimensions.top + viewportDimensions.height) { // bottom overflow
                    delta.top = viewportDimensions.top + viewportDimensions.height - bottomEdgeOffset;
                }
            } else {
                var leftEdgeOffset = pos.left - viewportPadding;
                var rightEdgeOffset = pos.left + viewportPadding + actualWidth;
                if (leftEdgeOffset < viewportDimensions.left) { // left overflow
                    delta.left = viewportDimensions.left - leftEdgeOffset;
                } else if (rightEdgeOffset > viewportDimensions.right) { // right overflow
                    delta.left = viewportDimensions.left + viewportDimensions.width - rightEdgeOffset;
                }
            }

            return delta;
        },
        getPosition: function ($element) {
            $element = $element || this.$element

            var el = $element[0]
            var isBody = el.tagName == 'BODY'

            var elRect = el.getBoundingClientRect();
            if (elRect.width == null) {
                // width and height are missing in IE8, so compute them manually; see https://github.com/twbs/bootstrap/issues/14093
                elRect = $.extend({}, elRect, { width: elRect.right - elRect.left, height: elRect.bottom - elRect.top });
            }
            var isSvg = window.SVGElement && el instanceof window.SVGElement;
            // Avoid using $.offset() on SVGs since it gives incorrect results in jQuery 3.
            // See https://github.com/twbs/bootstrap/issues/20280
            var elOffset = isBody ? { top: 0, left: 0 } : (isSvg ? null : $element.offset());
            var scroll = { scroll: isBody ? document.documentElement.scrollTop || document.body.scrollTop : $element.scrollTop() };
            var outerDims = isBody ? { width: $(window).width(), height: $(window).height() } : null;

            return $.extend({}, elRect, scroll, outerDims, elOffset);
        },
        hasContent: function () {
            return this.getTitle();
        },
        arrow: function () {
            return (this.$arrow = this.$arrow || this.tip().find('.tooltip-arrow'));
        },
        enable: function () {
            this.enabled = true;
            return this;
        },
        disable: function () {
            this.enabled = false;
            return this;
        },
        fixTitle: function () {
            var $e = this.$element
            if ($e.attr('title') || typeof $e.attr('data-original-title') != 'string') {
                $e.attr('data-original-title', $e.attr('title') || '').attr('title', '')
            }
        },
        isInStateTrue: function () {
            for (var key in this.inState) {
                if (this.inState[key]) return true;
            }
            return false;
        },
        getTitle: function () {
            var title;
            var $e = this.$element;
            var o = this.options;

            title = $e.attr('data-original-title') ||
                (typeof o.title == 'function' ? o.title.call($e[0]) : o.title);

            return title;
        }

    });

})(jQuery);
(function ($) {
    $.fn.LeePopOver = function (p) {
        return $.leeUI.run.call(this, "PopOver", arguments, {
            idAttrName: "popoverid"
        });
    };

    $.leeUIDefaults.PopOver = $.extend({}, $.leeUIDefaults.ToolTip, {
        placement: 'right',
        trigger: 'click',
        content: '',
        contentLoad: null,//动态加载提示内容
        template: '<div class="popover" role="tooltip"><div class="arrow"></div><h3 class="popover-title"></h3><div class="popover-content"></div></div>'
    });

    $.leeUI.controls.PopOver = function (element, options) {
        $.leeUI.controls.PopOver.base.constructor.call(this, element, options);
    };

    $.leeUI.controls.PopOver.leeExtend($.leeUI.controls.ToolTip, {
        __getType: function () {
            return 'PopOver';
        },
        __idPrev: function () {
            return 'PopOver';
        },
        _setContent: function () {
            var $tip = this.tip();
            var title = this.getTitle();

            var content = this.getContent();
            $tip.find('.popover-title')[this.options.html ? 'html' : 'text'](title);
            $tip.find('.popover-content').children().detach().end()[ // we use append for html objects to maintain js events
                this.options.html ? (typeof content == 'string' ? 'html' : 'append') : 'text'
            ](content);
            $tip.removeClass('fade top bottom left right in')

            // IE8 doesn't accept hiding via the `:empty` pseudo selector, we have to do
            // this manually by checking the contents.
            if (!$tip.find('.popover-title').html()) $tip.find('.popover-title').hide();

        },
        hasContent: function () {
            return this.getTitle() || this.getContent();
        },
        getContent: function () {
            var $e = this.$element;
            var o = this.options;
            //alert(1);v

            var res = $e.attr('content') ||
                (typeof o.content == 'function' ?
                    o.content.call($e[0]) :
                    o.content);

            return res;
        },
        arrow: function () {
            return (this.$arrow = this.$arrow || this.tip().find('.arrow'));
        }
    });
})(jQuery);
/*
 * 下拉菜单插件 contextmenu
 */
(function ($) {
    $.fn.leeMenu = function (options) {
        return $.leeUI.run.call(null, "leeUIMenu", arguments);
    };

    $.leeUIDefaults.Menu = {
        width: 120,
        top: 0,
        left: 0,
        cls: null,
        items: null,
        shadow: true,
        renderTo: "body"
    };
    $.leeUI.controls.Menu = function (options) {
        $.leeUI.controls.Menu.base.constructor.call(this, null, options);
    };
    $.leeUI.controls.Menu.leeExtend($.leeUI.core.UIComponent, {
        __getType: function () {
            return 'Menu';
        },
        __idPrev: function () {
            return 'Menu';
        },
        _extendMethods: function () {
            return {};
        },
        _render: function () {
            var g = this,
                p = this.options;
            g.menuItemCount = 0;
            //全部菜单
            g.menus = {};
            //创建顶级菜单
            g.menu = g.createMenu();
            //记录element
            g.element = g.menu[0];
            //设置位置
            g.menu.css({ top: p.top, left: p.left, width: p.width });
            p.cls && g.menu.addClass(p.cls); //自定义样式

            p.items && $(p.items).each(function (i, item) {
                //循环添加数据
                g.addItem(item);
            });

            $(document).bind('click.menu', function () {
                //隐藏所有的菜单
                for (var menuid in g.menus) {
                    var menu = g.menus[menuid];
                    if (!menu) return;
                    menu.hide();
                }
            });
            g.set(p);
        },
        show: function (options, menu) {
            var g = this,
                p = this.options;
            if (menu == undefined) menu = g.menu;
            if (options && options.left != undefined) {
                menu.css({ left: options.left });
            }
            if (options && options.top != undefined) {
                menu.css({ top: options.top });
            }
            menu.show();

        },
        hide: function (menu) {
            var g = this,
                p = this.options;
            if (menu == undefined) menu = g.menu;
            g.hideAllSubMenu(menu);
            menu.hide();

        },
        toggle: function () {
            var g = this,
                p = this.options;
            g.menu.toggle();

        },
        removeItem: function (itemid) {
            //移除按钮 
            var g = this,
                p = this.options;
            $("> .lee-menu-item[menuitemid=" + itemid + "]", g.menu.items).remove();
        },
        setEnabled: function (itemid) {
            var g = this,
                p = this.options;
            $("> .lee-menu-item[menuitemid=" + itemid + "]", g.menu.items).removeClass("lee-menu-item-disable");
        },
        setMenuText: function (itemid, text) {
            var g = this,
                p = this.options;
            $("> .lee-menu-item[menuitemid=" + itemid + "] >.lee-menu-item-text:first", g.menu.items).html(text);
        },
        setDisabled: function (itemid) {
            var g = this,
                p = this.options;
            $("> .lee-menu-item[menuitemid=" + itemid + "]", g.menu.items).addClass("lee-menu-item-disable");
        },
        isEnable: function (itemid) {
            var g = this,
                p = this.options;
            return !$("> .lee-menu-item[menuitemid=" + itemid + "]", g.menu.items).hasClass("lee-menu-item-disable");
        },
        getItemCount: function () {
            var g = this,
                p = this.options;
            return $("> .lee-menu-item", g.menu.items).length;
        },
        addItem: function (item, menu) {
            var g = this,
                p = this.options;
            if (!item) return;
            if (menu == undefined) menu = g.menu; //顶级菜单

            //添加分隔线
            if (item.line) {
                menu.items.append('<div class="lee-menu-item-line"></div>');
                return;
            }
            //下拉选项
            var ditem = $('<div class="lee-menu-item"><div class="lee-menu-item-text"></div> </div>');
            var itemcount = $("> .lee-menu-item", menu.items).length;
            menu.items.append(ditem);
            ditem.attr("leemenutemid", ++g.menuItemCount); //全局数量ID

            item.id && ditem.attr("menuitemid", item.id);

            item.text && $(">.lee-menu-item-text:first", ditem).html(item.text); //按钮文本

            if (item.icon) {
                item.position = "left" || item.position;
                ditem.append('<i class=" ' + item.position + ' lee-icon lee-' + item.icon + '"></div>');
                menu.addClass("hasicon");
            }
            if (item.img) {
                item.position = "left" || item.position;
                ditem.append('<img class=" ' + item.position + ' lee-menu-item-img " src="' + item.img + '"></img>');
                menu.addClass("hasicon");
            }
            //item.img && ditem.prepend('<div class="l-menu-item-icon"><img style="width:16px;height:16px;margin:2px;" src="' + item.img + '" /></div>');
            if (item.disable || item.disabled)
                ditem.addClass("lee-menu-item-disable"); //只读类
            if (item.children) //如果有子菜单
            {
                ditem.append('<i class="right lee-icon lee-angle-right"></i>'); //右侧图标箭头
                var newmenu = g.createMenu(ditem.attr("leemenutemid")); //创建子菜单
                g.menus[ditem.attr("leemenutemid")] = newmenu; //缓存子菜单
                newmenu.width(p.width); //设置宽度
                newmenu.hover(null, function () {
                    if (!newmenu.showedSubMenu)
                        g.hide(newmenu);
                });
                $(item.children).each(function () {
                    g.addItem(this, newmenu); //添加子菜单
                });
            }
            item.click && ditem.click(function () {
                //点击事件
                if ($(this).hasClass("lee-menu-item-disable")) return;
                item.click(item, itemcount);
            });
            item.dblclick && ditem.dblclick(function () {
                //双击事件
                if ($(this).hasClass("lee-menu-item-disable")) return;
                item.dblclick(item, itemcount);
            });

            var menuover = $("> .lee-menu-over:first", menu);
            ditem.hover(function () {

                if ($(this).hasClass("lee-menu-item-disable")) return;
                var itemtop = $(this).offset().top;
                var top = itemtop - menu.offset().top;
                menuover.css({ top: top });
                g.hideAllSubMenu(menu); //隐藏所有的菜单
                if (item.children) {

                    var leemenutemid = $(this).attr("leemenutemid");
                    if (!leemenutemid) return;
                    if (g.menus[leemenutemid]) {
                        console.log($(this).offset().left);
                        console.log($(this).width());
                        console.log($(this));
                        //显示下级
                        g.show({ top: itemtop - 1, left: $(this).offset().left + $(this).parent().width() }, g.menus[leemenutemid]); //显示下级
                        menu.showedSubMenu = true;
                    }
                }
            }, function () {
                if ($(this).hasClass("lee-menu-item-disable")) return;
                var leemenutemid = $(this).attr("leemenutemid");
                if (item.children) {
                    var leemenutemid = $(this).attr("leemenutemid");

                    if (!leemenutemid) return;
                };
            });
        },
        hideAllSubMenu: function (menu) {
            //隐藏所有的子菜单
            var g = this,
                p = this.options;
            if (menu == undefined) menu = g.menu;
            $("> .lee-menu-item", menu.items).each(function () {
                if ($("> .right", this).length > 0) {
                    var leemenutemid = $(this).attr("leemenutemid");
                    if (!leemenutemid) return;
                    g.menus[leemenutemid] && g.hide(g.menus[leemenutemid]);
                }
            });
            menu.showedSubMenu = false;
        },
        createMenu: function (parentMenuItemID) {
            //父节点ID
            var g = this,
                p = this.options;
            var menu = $('<div class="lee-menu" style="display:none"> <div class="lee-menu-inner"></div></div>');
            //主布局
            parentMenuItemID && menu.attr("leeparentmenuitemid", parentMenuItemID); //这里设置父属性
            menu.items = $("> .lee-menu-inner:first", menu); //按钮区域
            menu.appendTo(p.renderTo);
            menu.hover(null, function () {
                if (!menu.showedSubMenu)
                    $("> .lee-menu-over:first", menu).css({ top: -24 });
            });
            if (parentMenuItemID)
                g.menus[parentMenuItemID] = menu; //缓存当前数据
            else
                g.menus[0] = menu; //如果没有 那么则认为是顶级
            return menu;
        }
    });

})(jQuery);





/* ========================================================================
* Bootstrap: dropdown.js v3.3.5
* http://getbootstrap.com/javascript/#dropdowns
* ========================================================================
* Copyright 2011-2015 Twitter, Inc.
* Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
* ======================================================================== */


+function ($) {
    'use strict';

    // DROPDOWN CLASS DEFINITION
    // =========================

    var backdrop = '.dropdown-backdrop'
    var toggle = '[data-toggle="dropdown"]'
    var Dropdown = function (element) {
        $(element).on('click.bs.dropdown', this.toggle)
    }

    Dropdown.VERSION = '3.3.5'

    function getParent($this) {
        var selector = $this.attr('data-target')

        if (!selector) {
            selector = $this.attr('href')
            selector = selector && /#[A-Za-z]/.test(selector) && selector.replace(/.*(?=#[^\s]*$)/, '') // strip for ie7
        }

        var $parent = selector && $(selector)

        return $parent && $parent.length ? $parent : $this.parent()
    }

    function clearMenus(e) {
        if (e && e.which === 3) return
        $(backdrop).remove()
        $(toggle).each(function () {
            var $this = $(this)
            var $parent = getParent($this)
            var relatedTarget = { relatedTarget: this }

            if (!$parent.hasClass('open')) return

            if (e && e.type == 'click' && /input|textarea/i.test(e.target.tagName) && $.contains($parent[0], e.target)) return

            $parent.trigger(e = $.Event('hide.bs.dropdown', relatedTarget))

            if (e.isDefaultPrevented()) return

            $this.attr('aria-expanded', 'false')
            $parent.removeClass('open').trigger('hidden.bs.dropdown', relatedTarget)
        })
    }

    Dropdown.prototype.toggle = function (e) {
        var $this = $(this)

        if ($this.is('.disabled, :disabled')) return

        var $parent = getParent($this)
        var isActive = $parent.hasClass('open')

        clearMenus()

        if (!isActive) {
            if ('ontouchstart' in document.documentElement && !$parent.closest('.navbar-nav').length) {
                // if mobile we use a backdrop because click events don't delegate
                $(document.createElement('div'))
                    .addClass('dropdown-backdrop')
                    .insertAfter($(this))
                    .on('click', clearMenus)
            }

            var relatedTarget = { relatedTarget: this }
            $parent.trigger(e = $.Event('show.bs.dropdown', relatedTarget))

            if (e.isDefaultPrevented()) return

            $this
                .trigger('focus')
                .attr('aria-expanded', 'true')

            $parent
                .toggleClass('open')
                .trigger('shown.bs.dropdown', relatedTarget)
        }

        return false
    }

    Dropdown.prototype.keydown = function (e) {
        if (!/(38|40|27|32)/.test(e.which) || /input|textarea/i.test(e.target.tagName)) return

        var $this = $(this)

        e.preventDefault()
        e.stopPropagation()

        if ($this.is('.disabled, :disabled')) return

        var $parent = getParent($this)
        var isActive = $parent.hasClass('open')

        if (!isActive && e.which != 27 || isActive && e.which == 27) {
            if (e.which == 27) $parent.find(toggle).trigger('focus')
            return $this.trigger('click')
        }

        var desc = ' li:not(.disabled):visible a'
        var $items = $parent.find('.dropdown-menu' + desc)

        if (!$items.length) return

        var index = $items.index(e.target)

        if (e.which == 38 && index > 0) index--         // up
        if (e.which == 40 && index < $items.length - 1) index++         // down
        if (! ~index) index = 0

        $items.eq(index).trigger('focus')
    }


    // DROPDOWN PLUGIN DEFINITION
    // ==========================

    function Plugin(option) {
        return this.each(function () {
            var $this = $(this)
            var data = $this.data('bs.dropdown')

            if (!data) $this.data('bs.dropdown', (data = new Dropdown(this)))
            if (typeof option == 'string') data[option].call($this)
        })
    }

    var old = $.fn.dropdown

    $.fn.dropdown = Plugin
    $.fn.dropdown.Constructor = Dropdown


    // DROPDOWN NO CONFLICT
    // ====================

    $.fn.dropdown.noConflict = function () {
        $.fn.dropdown = old
        return this
    }


    // APPLY TO STANDARD DROPDOWN ELEMENTS
    // ===================================

    $(document)
        .unbind('click.bs.dropdown.data-api')
        .unbind('click.bs.dropdown.data-api', '.dropdown form')
        .unbind('click.bs.dropdown.data-api')
        .unbind('keydown.bs.dropdown.data-api')
        .unbind('keydown.bs.dropdown.data-api', '.dropdown-menu')
    $(document)
        .on('click.bs.dropdown.data-api', clearMenus)
        .on('click.bs.dropdown.data-api', '.dropdown form', function (e) { e.stopPropagation() })
        .on('click.bs.dropdown.data-api', toggle, Dropdown.prototype.toggle)
        .on('keydown.bs.dropdown.data-api', toggle, Dropdown.prototype.keydown)
        .on('keydown.bs.dropdown.data-api', '.dropdown-menu', Dropdown.prototype.keydown)

}(jQuery);
/**
 * 
 */
(function ($) {

    $.fn.leeToolBar = function (options) {
        return $.leeUI.run.call(this, "leeUIToolBar", arguments);
    };

    $.leeUIDefaults.ToolBar = {
        flex: false
    };

    $.leeUI.controls.ToolBar = function (element, options) {
        $.leeUI.controls.ToolBar.base.constructor.call(this, element, options);
    };
    $.leeUI.controls.ToolBar.leeExtend($.leeUI.core.UIComponent, {
        __getType: function () {
            return 'ToolBar';
        },
        __idPrev: function () {
            return 'ToolBar';
        },
        _extendMethods: function () {
            return [];
        },
        _render: function () {
            var g = this,
                p = this.options;
            g.toolbarItemCount = 0;
            g.toolBar = $(this.element);
            g.toolBar.addClass("lee-toolbar"); //外围包裹css
            g.set(p);
            g.disableItems = {};
            if (p.flex)
                g.bindFlex();
        },
        _setItems: function (items) {
            var g = this;
            g.toolBar.html("");
            $(items).each(function (i, item) {
                g.addItem(item);
            });
        },
        bindFlex: function () {
            $(window).resize($.proxy(this.setFlex, this));
            this.setFlex();
        },
        setFlex: function () {
            var g = this,
                p = this.options;
            var parentWidth = g.toolBar.width();
            var initWidth = 80;
            var hideBtns = [];
            var $ele = $(">div", g.toolBar);
            $(p.items).each(function (i, item) {
                var ele = $($ele[i]);
                if (ele.data("hidden")) return true;
                initWidth += ele.outerWidth();
                initWidth += parseInt(ele.css('marginLeft'));
                initWidth += parseInt(ele.css('marginRight'));
                if (initWidth < parentWidth) {
                    $($ele[i]).show();
                } else {
                    $($ele[i]).hide();
                    hideBtns.push(item);
                }
            });
            g.removeItem("_showmore");
            if (hideBtns.length > 0) {
                g.addItem({ id: "_showmore", text: "更多", type: "dropdown", childs: hideBtns });
            }



        },
        removeItem: function (itemid) {
            var g = this,
                p = this.options;
            $("> .lee-toolbar-item[toolbarid=" + itemid + "]", g.toolBar).remove();
        },
        setEnabled: function (itemid) {
            var g = this,
                p = this.options;
            $("> .lee-toolbar-item[toolbarid=" + itemid + "]", g.toolBar).removeClass("lee-toolbar-item-disable");
            $("a[toolbarid=" + itemid + "]", g.toolBar).removeAttr("disabled");
            g.disableItems[itemid] = false;
        },
        _setDisabled: function (itemid) {
            var g = this,
                p = this.options;

            $("> .lee-toolbar-item[toolbarid=" + itemid + "]", g.toolBar).addClass("lee-toolbar-item-disable");

            $("a[toolbarid=" + itemid + "]", g.toolBar).attr("disabled", "disabled");
            g.disableItems[itemid] = true;
        },
        toggleBtns: function (items, visible) {
            var g = this,
                p = this.options;
            var showfunc = visible ? "show" : "hide";

            $.each(items, function (i, itemid) {
                var ele = $("[toolbarid=" + itemid + "]", g.toolBar);
                ele[showfunc]();
                if (visible) {
                    ele.data("hidden", null);
                } else {
                    ele.data("hidden", true);
                }
            })
            if (items.length > 0 && p.flex)
                this.setFlex();

        },
        setDisabled: function (itemid) {
            var g = this,
                p = this.options;
            $("> .lee-toolbar-item[toolbarid=" + itemid + "]", g.toolBar).addClass("lee-toolbar-item-disable").removeClass("lee-panel-btn-over");

            $("a[toolbarid=" + itemid + "]", g.toolBar).attr("disabled", "disabled");
            g.disableItems[itemid] = true;
        },
        isEnable: function (itemid) {
            var g = this,
                p = this.options;
            return !$("> .lee-toolbar-item[toolbarid=" + itemid + "]", g.toolBar).hasClass("lee-toolbar-item-disable");
        },
        bulidDropDownMenu: function (item, $wrap) {
            var self = this;
            $.each(item.childs, function (i, obj) {

                if (obj.line) {
                    $wrap.append('<li class="divider"></li>');
                    return;
                }
                if (obj.header) {
                    $wrap.append('<li class="dropdown-header">' + obj.text + '</li>');
                    return;
                }
                var $item = $("<li></li>");
                obj.disable && $item.addClass("disabled");
                var attr = "";
                if (self.disableItems[obj.id])
                    attr = " disabled ";
                var $menuitem = $('<a href="#" toolbarid="' + obj.id + '" ' + attr + '><span>' + obj.text + '</span></a>');
                if (obj.childs) {
                    $item.addClass("dropdown-submenu")
                    var $subwrap = $('<ul class="dropdown-menu"></ul>');
                    $item.append($menuitem).append($subwrap);
                    self.bulidDropDownMenu(obj, $subwrap);
                } else {
                    $item.append($menuitem);
                }
                if (obj.icon) {
                    $menuitem.prepend("<i class='icon-img icon-" + obj.icon + "'></i>");
                } else if (obj.iconfont) {
                    $menuitem.prepend("<i class='lee-ion lee-ion-" + obj.iconfont + "'></i>");
                }
                $menuitem.click(function (e) {
                    //alert(2);
                    if ($(this).attr("disabled") == "disabled")
                        return;
                    if ($(this).parent().hasClass("disabled"))
                        return;
                    var res = obj.click(obj, self);
                    self.trigger('buttonClick', [res, item, e]);
                });
                $wrap.append($item);

            });
        },
        setAlign: function ($ele, align) {

            if (align == "1")
                $ele.css("float", "right");
        },
        addLine: function (item) {
            var $line = $('<div class="lee-bar-separator"></div>');
            this.setAlign($line, item.align);
            this.toolBar.append($line);
        },
        addText: function (item) {

            var $text = $('<div class="lee-toolbar-item lee-toolbar-text"><span><p>' + (item.text || "") + '</p></span></div>');
            this.setAlign($text, item.align);
            this.toolBar.append($text);
        },
        addSearchBox: function (item) {
            var g = this;
            var $search = $('<div class="lee-search-wrap lee-toolbar-item"><input class="lee-search-words" type="text" placeholder="请输入查询关键字"><i class="lee-ion-close close"></i><button class="search lee-ion-search" type="button" ></button></div>');
            this.setAlign($search, item.align);
            this.toolBar.append($search);

            var $input = $("input", $search);
            var $close = $(".close", $search);
            var $button = $("button", $search);
            $button.click(function (e) {
                var res = item.click(item, g, $input.val());// 查询按钮调用
                g.trigger('buttonClick', [res, item, e]);
            });

            $input.keyup(function (event) {
                if (event.keyCode == 13) {
                    $button.click();
                }
                showClose();
            });


            $close.click(function () {
                $input.val("");
                showClose();
                $input.focus();
                $button.click();
            });

            function showClose() {
                if ($input.val() == "") {
                    $close.hide();
                } else {
                    $close.show();
                }
            }
            return this;
        },
        addLink: function (item) {
            var $link = $('<div class="lee-search-wrap lee-toolbar-item" ><a href="javascript:void(0)" style="padding:5px;display:inline-block;" >' + item.text + '</a></div>');

            this.setAlign($link, item.align);
            this.toolBar.append($link);
            $("a", $link).click(function () {
                //alert(1);
                item.click(item);
            });
            return this;
        },
        addDropDown: function (item) {
            var attr = "";

            var $dropdown = $('<li class="dropitem dropdown lee-toolbar-item" toolbarid="' + item.id + '"> <a href="#" data-toggle="dropdown" class="lee-panel-btn"  >' + item.text + '<i style="margin-left: 5px;" class="lee-ion-android-arrow-dropdown"></i></a> <ul class="dropdown-menu"></ul></li>');
            this.setAlign($dropdown);
            if (item.icon) {
                $dropdown.find("a").prepend("<i class='icon-img icon-" + item.icon + "' style='margin-right:5px;'></i>");
            } else if (item.iconfont) {
                $dropdown.find("a").prepend("<i class='lee-ion lee-ion-" + item.iconfont + "' style='margin-right:5px;'></i>");
            }
            if (item.childs) {
                this.bulidDropDownMenu(item, $dropdown.find("ul"));
            }
            this.toolBar.append($dropdown);
        },
        addItem: function (item) {
            var g = this,
                p = this.options;
            if (item.line || item.type == "line") {
                this.addLine(item);
                return;
            }
            if (item.type == "text") {

                this.addText(item);
                return;
            }
            if (item.type == "searchbox") {
                this.addSearchBox(item);
                return;
            }


            if (item.type == "link") {
                this.addLink(item);
                return this;
            }
            if (item.type == "dropdown") {
                this.addDropDown(item);
                return;
            }
            //普通的按钮 lee-btn 三种模式
            //lee-dropdown
            //lee-date
            var ditem = $('<div class="lee-toolbar-item lee-panel-btn"><span></span></div>');
            if (item.type == "btn") {


                ditem = $('<a class="lee-btn  lee-toolbar-item"><span></span></a>');

                if (item.style) {
                    switch (item.style) {
                        case "1":
                            ditem.addClass("lee-btn-default");
                            break;
                        case "2":
                            ditem.addClass("lee-btn-primary");
                            break;
                        case "3":
                            ditem.addClass("lee-btn-sucess");
                            break;
                        case "4":
                            ditem.addClass("lee-btn-info");
                            break;
                        case "5":
                            ditem.addClass("lee-btn-danger");
                            break;
                        default:
                            break;
                    }
                } else {
                    ditem.addClass("lee-btn-primary");
                }
            }
            g.toolBar.append(ditem);
            g.setAlign(ditem, item.align);
            if (!item.id) item.id = 'item-' + (++g.toolbarItemCount);
            ditem.attr("toolbarid", item.id);
            if (item.img) {
                ditem.append("<img src='" + item.img + "' />");
                ditem.addClass("l-toolbar-item-hasicon");
            } else if (item.icon) {
                ditem.prepend("<i class='icon-img icon-" + item.icon + "'></i>");
                ditem.addClass("l-toolbar-item-hasicon");
            } else if (item.iconfont) {
                ditem.prepend("<i class='lee-ion lee-ion-" + item.iconfont + "'></i>");
                ditem.addClass("l-toolbar-item-hasicon");
            } else if (item.color) {

                ditem.append("<div class='lee-toolbar-item-color' style='background:" + item.color + "'></div>");
                ditem.addClass("l-toolbar-item-hasicon");
            }

            if (item.menu) {
                ditem.append("<i class='right lee-icon lee-angle-down'></i>");
                ditem.addClass("l-toolbar-item-hasicon");
            }
            item.text ? $("span:first", ditem).html(item.text) : $("span:first", ditem).remove();
            item.disable && ditem.addClass("lee-toolbar-item-disable");

            item.click && ditem.click(function (e) {
                if ($(this).hasClass("lee-toolbar-item-disable"))
                    return;
                var res = item.click(item, g);
                g.trigger('buttonClick', [res, item, e]);
                e
            });
            if (item.menu) {
                if (item.menu.id) return;
                //item.menu.renderTo=ditem;
                item.menu = $.fn.leeMenu(item.menu);
                ditem.hover(function () {
                    if ($(this).hasClass("lee-toolbar-item-disable")) return;
                    g.actionMenu && g.actionMenu.hide();
                    var left = $(this).offset().left;
                    var top = $(this).offset().top + $(this).height();
                    item.menu.show({
                        top: top + 10,
                        left: left
                    });
                    g.actionMenu = item.menu;
                    $(this).addClass("lee-panel-btn-over");
                }, function () {
                    if ($(this).hasClass("lee-toolbar-item-disable")) return;
                    $(this).removeClass("lee-panel-btn-over");
                });
            } else {
                ditem.hover(function () {
                    if ($(this).hasClass("lee-toolbar-item-disable")) return;
                    $(this).addClass("lee-panel-btn-over");
                }, function () {
                    if ($(this).hasClass("lee-toolbar-item-disable")) return;
                    $(this).removeClass("lee-panel-btn-over");
                });

                ditem.mousedown(function () {
                    if (!item.disable)
                        $(this).addClass("lee-panel-btn-selected");
                });

                ditem.mouseup(function () {
                    if (!item.disable)
                        $(this).removeClass("lee-panel-btn-selected");
                });
            }
        }
    });
})(jQuery);
(function ($) {

    $.fn.leeTab = function (options) {
        return $.leeUI.run.call(this, "leeUITab", arguments);
    };

    $.fn.leeGetTabManager = function () {
        return $.leeUI.run.call(this, "leeUIGetTabManager", arguments);
    };

    $.leeUIDefaults.Tab = {
        height: null,
        heightDiff: 1, // 高度补差 
        changeHeightOnResize: false,
        contextmenu: true,
        dblClickToClose: false, //是否双击时关闭
        dragToMove: false, //是否允许拖动时改变tab项的位置
        showSwitch: true, //显示切换窗口按钮

        showSwitchInTab: false, //切换窗口按钮显示在最后一项
        data: null, //传递数据容器
        onBeforeOverrideTabItem: null,
        onAfterOverrideTabItem: null,
        onBeforeRemoveTabItem: null,
        onAfterRemoveTabItem: null,
        onBeforeAddTabItem: null,
        onAfterAddTabItem: null,
        onBeforeSelectTabItem: null,
        onAfterSelectTabItem: null,
        onCloseOther: null,
        onCloseAll: null,
        onClose: null,
        onReload: null,
        onSwitchRender: null //当切换窗口层构件时的事件
    };
    $.leeUIDefaults.TabString = {
        closeMessage: "关闭当前页",
        closeOtherMessage: "关闭其他",
        closeAllMessage: "关闭所有",
        reloadMessage: "刷新"
    };

    $.leeUI.controls.Tab = function (element, options) {
        $.leeUI.controls.Tab.base.constructor.call(this, element, options);
    };
    $.leeUI.controls.Tab.leeExtend($.leeUI.core.UIComponent, {
        __getType: function () {
            return 'Tab';
        },
        __idPrev: function () {
            return 'Tab';
        },
        _extendMethods: function () {
            return {};
        },
        _render: function () {
            var g = this,
                p = this.options;
            if (p.height) g.makeFullHeight = true;
            g.tab = $(this.element);
            g.tab.addClass("lee-tab");
            g._initContextMenu();
            g.tab.content = $('<div class="lee-tab-content"></div>');
            $("> div", g.tab).appendTo(g.tab.content);
            g.tab.content.appendTo(g.tab);
            g.tab.links = $('<div class="lee-tab-links"><ul style="left: 0px; "></ul><div class="lee-tab-switch"><i class="lee-ion-ios-arrow-down"></i></div></div>');
            g.tab.links.prependTo(g.tab);
            g.tab.links.ul = $("ul", g.tab.links);
            var lselecteds = $("> div[lselected=true]", g.tab.content);
            var haslselected = lselecteds.length > 0;
            g.selectedTabId = lselecteds.attr("tabid");
            $("> div", g.tab.content).each(function (i, box) {
                var li = $('<li class=""><a></a></li>');
                var contentitem = $(this);
                if (contentitem.attr("title")) {
                    $("> a", li).html(contentitem.attr("title"));
                    contentitem.attr("title", "");
                }

                contentitem.attr("tabindex", i);
                var tabid = contentitem.attr("tabid");
                if (tabid == undefined) {
                    tabid = g.getNewTabid();
                    contentitem.attr("tabid", tabid);
                    if (contentitem.attr("lselected")) {
                        g.selectedTabId = tabid;
                    }
                }
                li.attr("tabid", tabid);
                if (!haslselected && i == 0) g.selectedTabId = tabid;
                var showClose = contentitem.attr("showClose");
                if (showClose) {
                    li.addClass("lee-tab-hasclose");
                    li.append("<i class='lee-ion-android-close lee-tab-links-item-close'></i>");
                }
                $("> ul", g.tab.links).append(li);
                if (!contentitem.hasClass("lee-tab-content-item")) contentitem.addClass("lee-tab-content-item");
                //如果有iframe的话
                if (contentitem.find("iframe").length > 0) {
                    var iframe = $("iframe:first", contentitem);
                    if (iframe[0].readyState != "complete") {
                        if (contentitem.find(".lee-tab-loading:first").length == 0)
                            contentitem.prepend("<div class='lee-tab-loading' style='display:block;'></div>");
                        var iframeloading = $(".l-tab-loading:first", contentitem);
                        iframe.bind('load.tab', function () {
                            iframeloading.hide();
                        });
                    }
                }
            });
            //init 
            g.selectTabItem(g.selectedTabId);
            //set content height
            if (p.height) {
                if (typeof (p.height) == 'string' && p.height.indexOf('%') > 0) {
                    g.onResize();
                    if (p.changeHeightOnResize) {
                        $(window).resize(function () {
                            g.onResize.call(g); //跟随窗口变化
                        });
                    }
                } else {
                    g.setHeight(p.height);
                }
            }
            if (g.makeFullHeight)
                g.setContentHeight();
            //add even 
            $("li", g.tab.links).each(function () {
                g._addTabItemEvent($(this));
            });
            g.tab.bind('dblclick.tab', function (e) {
                if (!p.dblClickToClose) return;
                g.dblclicking = true;
                var obj = (e.target || e.srcElement);
                var tagName = obj.tagName.toLowerCase();
                if (tagName == "a") {
                    var tabid = $(obj).parent().attr("tabid");
                    var allowClose = $(obj).parent().find("div.l-tab-links-item-close").length ? true : false;
                    if (allowClose) {
                        g.removeTabItem(tabid);
                    }
                }
                g.dblclicking = false;
            });

            g.set(p);
            g.setTabButton();
            g._initToolbar();
            g._initIcon();
            //set tab links width
            setTimeout(setLinksWidth, 100);
            $(window).resize(function () {
                setLinksWidth.call(g);
            });

            function setLinksWidth() {
                var w = g.tab.width() - parseInt(g.tab.links.css("marginLeft"), 10) - parseInt(g.tab.links.css("marginRight"), 10);
                g.tab.links.width(w);
                g.setTabButton();

            }
            g.bind('sysWidthChange', function () {
                setLinksWidth.call(g);
            });
        },
        _initToolbar: function () {
            var g = this,
                p = this.options;

            if (p.toolbar) {
                g.toolbars = [];
                g.toolbarWrap = $("<div class='lee-tab-toolbar'></div>");
                g.tab.links.append(g.toolbarWrap);
                for (var i = 0; i < p.toolbar.length; i++) {
                    var toolbar = $("<div></div>");
                    toolbar.leeToolBar(p.toolbar[i]);
                    g.toolbarWrap.append(toolbar);
                    g.toolbars.push(toolbar);
                }
            }
        },
        _setToolbarVisible: function () {
            var g = this,
                p = this.options;
            if (!g.toolbars) return;
            for (var i = 0; i < g.toolbars.length; i++) {

                if (i == g.selectIndex)
                    g.toolbars[i].show();
                else
                    g.toolbars[i].hide();
            }

        },
        _initIcon: function () {
            var g = this,
                p = this.options;
            if (p.icons) {
                for (var item in p.icons) {
                    var $menuitem = $("li:eq(" + item + ")", g.tab.links.ul);
                    if (p.icons[item].icon) {
                        $menuitem.prepend("<i class='icon-img icon-" + obj.icon + "'></i>");
                    } else if (p.icons[item].iconfont) {
                        $menuitem.prepend("<i class='lee-ion lee-ion-" + obj.iconfont + "'></i>");
                    }
                }

            }


        },
        _initContextMenu: function () {
            var g = this,
                p = this.options;
            if (p.contextmenu && $.fn.leeMenu) {
                g.tab.menu = $.fn.leeMenu({
                    items: [{
                        text: p.closeMessage,
                        id: 'close',
                        click: function () {
                            g._menuItemClick.apply(g, arguments);
                        }
                    },
                    {
                        text: p.closeOtherMessage,
                        id: 'closeother',
                        click: function () {
                            g._menuItemClick.apply(g, arguments);
                        }
                    },
                    {
                        text: p.closeAllMessage,
                        id: 'closeall',
                        click: function () {
                            g._menuItemClick.apply(g, arguments);
                        }
                    },
                    {
                        text: p.reloadMessage,
                        id: 'reload',
                        click: function () {
                            g._menuItemClick.apply(g, arguments);
                        }
                    }
                    ]
                });
            }
        },
        _setShowSwitch: function (value) {
            var g = this,
                p = this.options;
            if (value) {
                if (!$(".lee-tab-switch", g.tab.links).length) {
                    $("<div class='lee-tab-switch'><i class='lee-ion-chevron-down'></i></div>").appendTo(g.tab.links);
                }
                $(g.tab).addClass("lee-tab-switchable");
                $(".lee-tab-switch", g.tab).click(function () {
                    g.toggleSwitch(this);
                });
            } else {
                $(g.tab).removeClass("lee-tab-switchable");
                $("body > .lee-tab-windowsswitch").remove();
            }
        },
        _setShowSwitchInTab: function (value) {

            var g = this,
                p = this.options;
            if (p.showSwitch && value) {
                $(g.tab).removeClass("lee-tab-switchable");
                $(".lee-tab-switch", g.tab).remove();
                var tabitem = $("<li class='lee-tab-itemswitch'><a><i class='lee-ion-ios-arrow-down'></i></a></li>");
                tabitem.appendTo(g.tab.links.ul);
                tabitem.click(function () {
                    g.toggleSwitch(this);
                });
            } else {
                $(".lee-tab-itemswitch", g.tab.ul).remove();
            }
        },
        toggleSwitch: function (btn) {
            var g = this,
                p = this.options;
            if ($("body > .lee-tab-windowsswitch").length) {
                $("body > .lee-tab-windowsswitch").remove();
                return;
            }
            if (btn == null) return;
            var windowsswitch = $("<div class='lee-tab-windowsswitch'></div>").appendTo('body');
            var tabItems = g.tab.links.ul.find('>li');
            var selectedTabItemID = g.getSelectedTabItemID();
            tabItems.each(function (i, item) {
                var jlink = $("<a href='javascript:void(0)'></a>");
                jlink.text($(item).find("a").text());
                var tabid = $(item).attr("tabid");
                if (tabid == null) return;

                if (tabid == selectedTabItemID) {
                    jlink.addClass("selected");
                }

                jlink.attr("tabid", tabid);
                windowsswitch.append(jlink);
            });
            windowsswitch.css({
                top: $(btn).offset().top + $(btn).outerHeight() + 1,
                left: $(btn).offset().left - windowsswitch.outerWidth() + $(btn).outerWidth()
            });
            windowsswitch.find("a").bind("click", function (e) {
                var tabid = $(this).attr("tabid");
                if (tabid == undefined) return;
                g.selectTabItem(tabid);
                g.moveToTabItem(tabid);
                $("body > .lee-tab-windowsswitch").remove();
            });
            g.trigger('switchRender', [windowsswitch]);
        },
        _applyDrag: function (tabItemDom) {
            var g = this,
                p = this.options;
            g.droptip = g.droptip || $("<div class='lee-tab-drag-droptip' style='display:none'><div class='lee-drop-move-up'></div><div class='lee-ion-ios-arrow-down'></div></div>").appendTo('body');
            var drag = $(tabItemDom).leeDrag({
                revert: true,
                animate: false,
                proxy: function () {
                    var name = $(this).find("a").html();
                    g.dragproxy = $("<div class='lee-tab-drag-proxy' style='display:none'><div class='lee-drop-icon lee-drop-no'></div></div>").appendTo('body');
                    g.dragproxy.append(name);
                    return g.dragproxy;
                },
                onRendered: function () {
                    this.set('cursor', 'pointer');
                },
                onStartDrag: function (current, e) {
                    if (!$(tabItemDom).hasClass("lee-selected")) return false;
                    if (e.button == 2) return false;
                    var obj = e.srcElement || e.target;
                    if ($(obj).hasClass("lee-tab-links-item-close")) return false;
                },
                onDrag: function (current, e) {
                    if (g.dropIn == null)
                        g.dropIn = -1;
                    var tabItems = g.tab.links.ul.find('>li');
                    var targetIndex = tabItems.index(current.target);
                    tabItems.each(function (i, item) {
                        if (targetIndex == i) {
                            return;
                        }
                        var isAfter = i > targetIndex;
                        if (g.dropIn != -1 && g.dropIn != i) return;
                        var offset = $(this).offset();
                        var range = {
                            top: offset.top,
                            bottom: offset.top + $(this).height(),
                            left: offset.left - 10,
                            right: offset.left + 10
                        };
                        if (isAfter) {
                            range.left += $(this).width();
                            range.right += $(this).width();
                        }
                        var pageX = e.pageX || e.screenX;
                        var pageY = e.pageY || e.screenY;
                        if (pageX > range.left && pageX < range.right && pageY > range.top && pageY < range.bottom) {
                            g.droptip.css({
                                left: range.left + 5,
                                top: range.top - 9
                            }).show();
                            g.dropIn = i;
                            g.dragproxy.find(".lee-drop-icon").removeClass("lee-drop-no").addClass("lee-drop-yes");
                        } else {
                            g.dropIn = -1;
                            g.droptip.hide();
                            g.dragproxy.find(".lee-drop-icon").removeClass("lee-drop-yes").addClass("lee-drop-no");
                        }
                    });
                },
                onStopDrag: function (current, e) {
                    if (g.dropIn > -1) {
                        var to = g.tab.links.ul.find('>li:eq(' + g.dropIn + ')').attr("tabid");
                        var from = $(current.target).attr("tabid");
                        setTimeout(function () {
                            g.moveTabItem(from, to);
                        }, 0);
                        g.dropIn = -1;
                        g.dragproxy.remove();
                    }
                    g.droptip.hide();
                    this.set('cursor', 'default');
                }
            });
            return drag;
        },
        _setDragToMove: function (value) {
            if (!$.fn.leeDrag) return; //需要ligerDrag的支持
            var g = this,
                p = this.options;
            if (value) {
                if (g.drags) return;
                g.drags = g.drags || [];
                g.tab.links.ul.find('>li').each(function () {
                    g.drags.push(g._applyDrag(this));
                });
            }
        },
        moveTabItem: function (fromTabItemID, toTabItemID) {
            var g = this;
            var from = g.tab.links.ul.find(">li[tabid=" + fromTabItemID + "]");
            var to = g.tab.links.ul.find(">li[tabid=" + toTabItemID + "]");
            var index1 = g.tab.links.ul.find(">li").index(from);
            var index2 = g.tab.links.ul.find(">li").index(to);
            if (index1 < index2) {
                to.after(from);
            } else {
                to.before(from);
            }
        },
        //设置tab按钮(左和右),显示返回true,隐藏返回false
        setTabButton: function () {
            var g = this,
                p = this.options;
            var sumwidth = 0;
            $("li", g.tab.links.ul).each(function () {
                sumwidth += $(this).outerWidth() + 2;
            });
            var mainwidth = g.tab.width();
            if (sumwidth > mainwidth) {
                if (!$(".lee-tab-links-left", g.tab).length) {
                    g.tab.links.append('<div class="lee-tab-links-left"><a class="lee-ion-ios-arrow-left"></a></div><div class="lee-tab-links-right"><a class="lee-ion-ios-arrow-right"></a></div>');
                    g.setTabButtonEven();
                }
                //console.log("我是18");
                //g.tab.links.ul.animate({ left: 18 });
                console.log(g.tab.links.ul.css("left"));
                if (g.tab.links.ul.position().left <= 30) {
                    g.tab.links.ul.css("left", 18);
                }
                return true;
            } else {
                //console.log("我是0");
                //g.tab.links.ul.animate({ left: 0 });
                console.log(g.tab.links.ul.css("left"));
                if (g.tab.links.ul.position().left <= 30) {
                    g.tab.links.ul.css("left", 0);
                }
                $(".lee-tab-links-left,.lee-tab-links-right", g.tab.links).remove();
                return false;
            }
        },
        //设置左右按钮的事件 标签超出最大宽度时，可左右拖动
        setTabButtonEven: function () {
            var g = this,
                p = this.options;
            $(".lee-tab-links-left", g.tab.links).hover(function () {
                $(this).addClass("lee-tab-links-left-over");
            }, function () {
                $(this).removeClass("lee-tab-links-left-over");
            }).unbind("click").click(function () {
                g.moveToPrevTabItem();
            });
            $(".lee-tab-links-right", g.tab.links).hover(function () {
                $(this).addClass("lee-tab-links-right-over");
            }, function () {
                $(this).removeClass("lee-tab-links-right-over");
            }).unbind("click").click(function () {
                g.moveToNextTabItem();
            });
        },
        //切换到上一个tab
        moveToPrevTabItem: function (tabid) {
            var g = this,
                p = this.options;
            var tabItems = $("> li", g.tab.links.ul),
                nextBtn = $(".lee-tab-links-right", g.tab),
                prevBtn = $(".lee-tab-links-left", g.tab);
            if (!nextBtn.length || !prevBtn.length) return false;
            var nextBtnOffset = nextBtn.offset(),
                prevBtnOffset = prevBtn.offset();
            //计算应该移动到的标签项,并计算从第一项到这个标签项的上一项的宽度总和
            var moveToTabItem = null,
                currentWidth = 0;
            var prevBtnLeft = prevBtnOffset.left + prevBtn.outerWidth();
            for (var i = 0, l = tabItems.length; i < l; i++) {
                var tabitem = $(tabItems[i]);
                var offset = tabitem.offset();
                var start = offset.left,
                    end = offset.left + tabitem.outerWidth();
                if (tabid != null) {
                    if (start < prevBtnLeft && tabitem.attr("tabid") == tabid) {
                        moveToTabItem = tabitem;
                        break;
                    }
                } else if (start < prevBtnLeft && end >= prevBtnLeft) {
                    moveToTabItem = tabitem;
                    break;
                }
                currentWidth += tabitem.outerWidth() + parseInt(tabitem.css("marginLeft")) +
                    parseInt(tabitem.css("marginRight"));
            }
            if (moveToTabItem == null) return false;
            //计算出正确的移动位置
            var left = currentWidth - prevBtn.outerWidth();
            g.tab.links.ul.animate({
                left: -1 * left
            });
            return true;
        },
        //切换到下一个tab
        moveToNextTabItem: function (tabid) {
            var g = this,
                p = this.options;
            var tabItems = $("> li", g.tab.links.ul),
                nextBtn = $(".lee-tab-links-right", g.tab),
                prevBtn = $(".lee-tab-links-left", g.tab);
            if (!nextBtn.length || !prevBtn.length) return false;
            var nextBtnOffset = nextBtn.offset(),
                prevBtnOffset = prevBtn.offset();
            //计算应该移动到的标签项,并计算从第一项到这个标签项的宽度总和
            var moveToTabItem = null,
                currentWidth = 0;
            for (var i = 0, l = tabItems.length; i < l; i++) {
                var tabitem = $(tabItems[i]);
                currentWidth += tabitem.outerWidth() +
                    parseInt(tabitem.css("marginLeft")) +
                    parseInt(tabitem.css("marginRight"));
                var offset = tabitem.offset();
                var start = offset.left,
                    end = offset.left + tabitem.outerWidth();
                if (tabid != null) {
                    if (end > nextBtnOffset.left && tabitem.attr("tabid") == tabid) {
                        moveToTabItem = tabitem;
                        break;
                    }
                } else if (start <= nextBtnOffset.left && end > nextBtnOffset.left) {
                    moveToTabItem = tabitem;
                    break;
                }
            }
            if (moveToTabItem == null) return false;
            //计算出正确的移动位置
            var left = currentWidth - (nextBtnOffset.left - prevBtnOffset.left) +
                parseInt(moveToTabItem.css("marginLeft")) + parseInt(moveToTabItem.css("marginRight"));
            g.tab.links.ul.animate({
                left: -1 * left
            });
            return true;
        },
        //切换到指定的项目项
        moveToTabItem: function (tabid) {
            var g = this,
                p = this.options;
            if (!g.moveToPrevTabItem(tabid)) {
                g.moveToNextTabItem(tabid);
            }
        },
        getTabItemCount: function () {
            var g = this,
                p = this.options;
            return $("li", g.tab.links.ul).length;
        },
        getSelectedTabItemID: function () {
            var g = this,
                p = this.options;
            return $("li.lee-selected", g.tab.links.ul).attr("tabid");
        },
        removeSelectedTabItem: function () {
            var g = this,
                p = this.options;
            g.removeTabItem(g.getSelectedTabItemID());
        },
        //覆盖选择的tabitem
        overrideSelectedTabItem: function (options) {
            var g = this,
                p = this.options;
            g.overrideTabItem(g.getSelectedTabItemID(), options);
        },
        //覆盖
        overrideTabItem: function (targettabid, options) {
            var g = this,
                p = this.options;
            if (g.trigger('beforeOverrideTabItem', [targettabid]) == false)
                return false;
            var tabid = options.tabid;
            if (tabid == undefined) tabid = g.getNewTabid();
            var url = options.url;
            var content = options.content;
            var target = options.target;
            var text = options.text;
            var showClose = options.showClose;
            var height = options.height;
            //如果已经存在
            if (g.isTabItemExist(tabid)) {
                return;
            }
            var tabitem = $("li[tabid=" + targettabid + "]", g.tab.links.ul);
            var contentitem = $(".lee-tab-content-item[tabid=" + targettabid + "]", g.tab.content);
            if (!tabitem || !contentitem) return;
            tabitem.attr("tabid", tabid);
            contentitem.attr("tabid", tabid);
            if ($("iframe", contentitem).length == 0 && url) {
                contentitem.html("<iframe frameborder='0'></iframe>");
            } else if (content) {
                contentitem.html(content);
            }
            $("iframe", contentitem).attr("name", tabid);
            if (showClose == undefined) showClose = true;
            if (showClose == false) $(".lee-tab-links-item-close", tabitem).remove();
            else {
                if ($(".lee-tab-links-item-close", tabitem).length == 0)
                    tabitem.append("<i class='lee-ion-android-close lee-tab-links-item-close'></i>");
            }
            if (text == undefined) text = tabid;
            if (height) contentitem.height(height);
            $("a", tabitem).text(text);
            $("iframe", contentitem).attr("src", url);

            g.trigger('afterOverrideTabItem', [targettabid]);
        },
        //设置页签项标题
        setHeader: function (tabid, header) {
            $("li[tabid=" + tabid + "] a", this.tab.links.ul).text(header);
        },
        //选中tab项
        selectTabItem: function (tabid) {
            var g = this,
                p = this.options;
            var $ele = $("> .lee-tab-content-item[tabid=" + tabid + "]", g.tab.content);
            var id = $ele[0].id;
            var tabindex = $ele.attr("tabindex");
            if (g.trigger('beforeSelectTabItem', [tabid, id, tabindex]) == false)
                return false;
            g.selectedTabId = tabid;
            g.selectIndex = Number(tabindex);

            $ele.show().siblings().hide();

            $("li[tabid=" + tabid + "]", g.tab.links.ul).addClass("lee-selected").siblings().removeClass("lee-selected");

            g._setToolbarVisible();
            g.trigger('afterSelectTabItem', [tabid, id, tabindex]);
        },
        //移动到最后一个tab
        moveToLastTabItem: function () {
            var g = this,
                p = this.options;
            var sumwidth = 0;
            $("li", g.tab.links.ul).each(function () {
                sumwidth += $(this).width() + 2;
            });
            var mainwidth = g.tab.width();
            if (sumwidth > mainwidth) {
                var btnWitdth = $(".lee-tab-links-right", g.tab.links).width();
                g.tab.links.ul.animate({
                    left: -1 * (sumwidth - mainwidth + btnWitdth + 2)
                });
            }
        },
        getTabItemTitle: function (tabid) {
            var g = this,
                p = this.options;
            return $("li[tabid=" + tabid + "] a", g.tab.links.ul).text();
        },
        setTabItemTitle: function (tabid, title) {
            var g = this,
                p = this.options;
            $("li[tabid=" + tabid + "] a", g.tab.links.ul).text(title);
        },
        getTabItemSrc: function (tabid) {
            var g = this,
                p = this.options;
            return $(".lee-tab-content-item[tabid=" + tabid + "] iframe", g.tab.content).attr("src");
        },
        setTabItemSrc: function (tabid, url) {
            var g = this,
                p = this.options;
            var contentitem = $(".lee-tab-content-item[tabid=" + tabid + "]", g.tab.content);
            var iframeloading = $(".lee-tab-loading:first", contentitem);
            var iframe = $(".lee-tab-content-item[tabid=" + tabid + "] iframe", g.tab.content);
            iframeloading.show();
            iframe.attr("src", url).unbind('load.tab').bind('load.tab', function () {
                iframeloading.hide();
            });
        },

        //判断tab是否存在
        isTabItemExist: function (tabid) {
            var g = this,
                p = this.options;
            return $("li[tabid=" + tabid + "] a", g.tab.links.ul).length > 0;
        },
        //增加一个tab
        addTabItem: function (options) {
            var g = this,
                p = this.options;
            if (g.trigger('beforeAddTabItem', [options]) == false)
                return false;
            var tabid = options.tabid;
            if (tabid == undefined) tabid = g.getNewTabid();
            var url = options.url,
                content = options.content,
                text = options.text,
                showClose = options.showClose,
                height = options.height;
            //如果已经存在
            if (g.isTabItemExist(tabid)) {
                g.selectTabItem(tabid);
                return;
            }
            var tabitem = $("<li class='lee-tab-hasclose'><a></a><i class='lee-ion-android-close lee-tab-links-item-close'></i></li>");
            var contentitem = $("<div class='lee-tab-content-item'><div class='lee-tab-loading' style='display:block;'></div><iframe frameborder='0'></iframe></div>");
            var iframeloading = $("div:first", contentitem);
            var iframe = $("iframe:first", contentitem);
            if (g.makeFullHeight) {
                var newheight = g.tab.height() - g.tab.links.height();
                contentitem.height(newheight);
            }
            tabitem.attr("tabid", tabid);
            contentitem.attr("tabid", tabid);
            if (url) {
                iframe[0].tab = g; //增加iframe对tab对象的引用  
                if (options.data) {
                    iframe[0].openerData = options.data;
                }
                iframe.attr("name", tabid)
                    .attr("id", tabid)
                    .attr("src", url)
                    .bind('load.tab', function () {
                        iframeloading.hide();
                        if (options.callback)
                            options.callback();
                    });
            } else {
                iframe.remove();
                iframeloading.remove();
            }
            if (content) {
                contentitem.html(content);
                if (options.callback)
                    options.callback();
            } else if (options.target) {
                contentitem.append(options.target);
                if (options.callback)
                    options.callback();
            }
            if (showClose == undefined) showClose = true;
            if (showClose == false) $(".lee-tab-links-item-close", tabitem).remove();
            if (text == undefined) text = tabid;
            if (height) contentitem.height(height);
            $("a", tabitem).text(text);

            if ($(".lee-tab-itemswitch", g.tab.links.ul).length) {

                tabitem.insertBefore($(".lee-tab-itemswitch", g.tab.links.ul));
            } else {

                g.tab.links.ul.append(tabitem);
                //console.log(1);
            }

            g.tab.content.append(contentitem);

            g.selectTabItem(tabid);

            if (g.setTabButton()) {
                g.moveToTabItem(tabid);
            }
            //增加事件
            g._addTabItemEvent(tabitem);
            if (p.dragToMove && $.fn.leeDrag) {
                g.drags = g.drags || [];
                tabitem.each(function () {
                    g.drags.push(g._applyDrag(this));
                });
            }
            g.toggleSwitch();
            g.trigger('afterAddTabItem', [options]);
        },
        _addTabItemEvent: function (tabitem) {
            var g = this,
                p = this.options;
            tabitem.click(function () {
                var tabid = $(this).attr("tabid");
                g.selectTabItem(tabid);
            });
            $(tabitem).hover(
                function () {
                    tabitem.addClass("lee-tab-item-hover");
                },
                function () {
                    tabitem.removeClass("lee-tab-item-hover");
                }
            );
            //右键事件支持
            g.tab.menu && g._addTabItemContextMenuEven(tabitem);
            $(".lee-tab-links-item-close", tabitem).hover(function () {
                $(this).addClass("lee-tab-links-item-close-over");
            }, function () {
                $(this).removeClass("lee-tab-links-item-close-over");
            }).click(function () {
                var tabid = $(this).parent().attr("tabid");
                g.removeTabItem(tabid);
            });

        },
        //移除tab项
        removeTabItem: function (tabid) {
            var g = this,
                p = this.options;
            if (g.trigger('beforeRemoveTabItem', [tabid]) == false)
                return false;
            var currentIsSelected = $("li[tabid=" + tabid + "]", g.tab.links.ul).hasClass("lee-selected");
            if (currentIsSelected) {
                $(".lee-tab-content-item[tabid=" + tabid + "]", g.tab.content).prev().show();
                $("li[tabid=" + tabid + "]", g.tab.links.ul).prev().addClass("lee-selected").siblings().removeClass("lee-selected");
                //选中前一个并没有触发事件
            }
            var contentItem = $(".lee-tab-content-item[tabid=" + tabid + "]", g.tab.content);
            var jframe = $('iframe', contentItem);
            if (jframe.length) {
                var frame = jframe[0];
                frame.src = "about:blank";
                try {
                    frame.contentWindow.document.write('');
                } catch (e) { }
                $.browser.msie && CollectGarbage();
                jframe.remove();
            }
            contentItem.remove();
            $("li[tabid=" + tabid + "]", g.tab.links.ul).remove();
            g.setTabButton();
            g.trigger('afterRemoveTabItem', [tabid]);
        },

        hideTabItem: function (tabid) {
            var g = this,
                p = this.options;
            var currentIsSelected = $("li[tabid=" + tabid + "]", g.tab.links.ul).hasClass("lee-selected");
            if (currentIsSelected) {
                $(".lee-tab-content-item[tabid=" + tabid + "]", g.tab.content).prev().show();
                $("li[tabid=" + tabid + "]", g.tab.links.ul).prev().addClass("lee-selected").siblings().removeClass("lee-selected");
            }
            $("li[tabid=" + tabid + "]", g.tab.links.ul).hide();
            $(".lee-tab-content-item[tabid=" + tabid + "]", g.tab.content).hide();

        },
        showTabItem: function (tabid) {
            var g = this,
                p = this.options;
            $("li[tabid=" + tabid + "]", g.tab.links.ul).show();
        },

        addHeight: function (heightDiff) {
            var g = this,
                p = this.options;
            var newHeight = g.tab.height() + heightDiff;
            g.setHeight(newHeight);
        },
        setHeight: function (height) {
            var g = this,
                p = this.options;
            g.tab.height(height);
            g.setContentHeight();
        },
        setContentHeight: function () {
            var g = this,
                p = this.options;
            var newheight = g.tab.height() - g.tab.links.height();
            g.tab.content.height(newheight);
            $("> .lee-tab-content-item", g.tab.content).height(newheight);
        },
        getNewTabid: function () {
            var g = this,
                p = this.options;
            g.getnewidcount = g.getnewidcount || 0;
            return 'tabitem' + (++g.getnewidcount);
        },
        //notabid 过滤掉tabid的
        //noclose 过滤掉没有关闭按钮的
        getTabidList: function (notabid, noclose) {
            var g = this,
                p = this.options;
            var tabidlist = [];
            $("> li", g.tab.links.ul).each(function () {
                if ($(this).attr("tabid") &&
                    $(this).attr("tabid") != notabid &&
                    (!noclose || $(".lee-tab-links-item-close", this).length > 0)) {
                    tabidlist.push($(this).attr("tabid"));
                }
            });
            return tabidlist;
        },
        removeOther: function (tabid, compel) {
            var g = this,
                p = this.options;
            var tabidlist = g.getTabidList(tabid, true);
            $(tabidlist).each(function () {
                g.removeTabItem(this);
            });
        },
        reload: function (tabid) {
            var g = this,
                p = this.options;
            var contentitem = $(".lee-tab-content-item[tabid=" + tabid + "]");
            var iframeloading = $(".lee-tab-loading:first", contentitem);
            var iframe = $("iframe:first", contentitem);
            var url = $(iframe).attr("src");
            iframeloading.show();
            iframe.attr("src", url).unbind('load.tab').bind('load.tab', function () {
                iframeloading.hide();
            });
        },
        removeAll: function (compel) {
            var g = this,
                p = this.options;
            var tabidlist = g.getTabidList(null, true);
            $(tabidlist).each(function () {
                g.removeTabItem(this);
            });
        },
        onResize: function () {
            var g = this,
                p = this.options;
            if (!p.height || typeof (p.height) != 'string' || p.height.indexOf('%') == -1) return false;
            //set tab height
            if (g.tab.parent()[0].tagName.toLowerCase() == "body") {
                var windowHeight = $(window).height();
                windowHeight -= parseInt(g.tab.parent().css('paddingTop'));
                windowHeight -= parseInt(g.tab.parent().css('paddingBottom'));
                g.height = p.heightDiff + windowHeight * parseFloat(g.height) * 0.01;
            } else {
                g.height = p.heightDiff + (g.tab.parent().height() * parseFloat(p.height) * 0.01);
            }
            g.tab.height(g.height);
            g.setContentHeight();
        },
        _menuItemClick: function (item) {
            var g = this,
                p = this.options;
            if (!item.id || !g.actionTabid) return;
            switch (item.id) {
                case "close":
                    if (g.trigger('close') == false) return;
                    g.removeTabItem(g.actionTabid);
                    g.actionTabid = null;
                    break;
                case "closeother":
                    if (g.trigger('closeother') == false) return;
                    g.removeOther(g.actionTabid);
                    break;
                case "closeall":
                    if (g.trigger('closeall') == false) return;
                    g.removeAll();
                    g.actionTabid = null;
                    break;
                case "reload":
                    if (g.trigger('reload', [{
                        tabid: g.actionTabid
                    }]) == false) return;
                    g.selectTabItem(g.actionTabid);
                    g.reload(g.actionTabid);
                    break;
            }
        },
        _addTabItemContextMenuEven: function (tabitem) {
            var g = this,
                p = this.options;
            tabitem.bind("contextmenu", function (e) {
                if (!g.tab.menu) return;
                g.actionTabid = tabitem.attr("tabid");
                g.tab.menu.show({
                    top: e.pageY,
                    left: e.pageX
                });
                if ($(".lee-tab-links-item-close", this).length == 0) {
                    g.tab.menu.setDisabled('close');
                } else {
                    g.tab.menu.setEnabled('close');
                }
                return false;
            });
        }
    });

})(jQuery);
(function ($) {

    $.fn.leeWizard = function (options) {
        return $.leeUI.run.call(this, "leeUIWizard", arguments);
    };

    $.fn.leeGetWizardManager = function () {
        return $.leeUI.run.call(this, "leeUIGetleeUIWizardManager", arguments);
    };

    $.leeUIDefaults.Wizard = {
        toolbar: false,
        onNext: null,
        onPrev: null,
        onConfirm: null,
        fixed: true,
        skip: false,//允许导航跳转
        onBeforeSelectTabItem: null,
        onAfterSelectTabItem: null
    };
    $.leeUIDefaults.WizardString = {

    };

    $.leeUI.controls.Wizard = function (element, options) {
        $.leeUI.controls.Wizard.base.constructor.call(this, element, options);
    };
    $.leeUI.controls.Wizard.leeExtend($.leeUI.core.UIComponent, {
        __getType: function () {
            return 'Wizard';
        },
        __idPrev: function () {
            return 'Wizard';
        },
        _extendMethods: function () {
            return {};
        },
        _render: function () {
            var g = this,
                p = this.options;
            g.tab = $(this.element);
            g.tab.addClass("lee-wizard");
            //内容区
            g.tab.content = $('<div class="lee-wizard-content"></div>');
            $("> div", g.tab).appendTo(g.tab.content); //把元素内容追加到内容区
            g.tab.content.appendTo(g.tab);

            g.tab.links = $('<div class="lee-wizard-step"><ul class="steps"></ul></div>');
            g.tab.links.prependTo(g.tab);
            g.activeTabs = {};
            g.tab.links.ul = $("ul", g.tab.links);
            g.allTabLength = 0;
            $("> div", g.tab.content).each(function (i, box) {
                var li = $('<li class=""><span class="step">' + (i + 1) + '</span><span class="title"></span><span class="chevron"></span></li>');
                var contentitem = $(this);
                if (contentitem.attr("title")) {
                    $("> .title", li).html(contentitem.attr("title"));
                    contentitem.attr("title", "");
                }
                contentitem.attr("tabindex", i);
                li.attr("tabindex", i);
                var tabid = contentitem.attr("tabid");
                if (tabid == undefined) {
                    tabid = g.getNewTabid();
                    contentitem.attr("tabid", tabid);
                }
                li.attr("tabid", tabid);

                if (p.skip) {
                    var clickFunc = (function (i) {
                        return function () {
                            if (g.activeTabs[i])
                                g.select(i);
                        }
                    })(i);
                    li.click(clickFunc);
                }

                if (i == 0) g.selectedTabId = tabid;
                $("> ul", g.tab.links).append(li);
                contentitem.addClass("lee-wizard-content-item");
                g.allTabLength++;
            });
            //init 
            g.setToolbar();
            g.select(0);

            //add even 
            // $("li", g.tab.links).each(function () {
            //     g._addTabItemEvent($(this));lee-wizard-content-item
            // });
            g.set(p);

        },
        setStatus: function () {
            var g = this,
                p = this.options;
            if (g.currentIndex == 0) {
                g.btnPrev.attr("disabled", "disabled");
            } else {
                g.btnPrev.removeAttr("disabled");
            }
            if (g.allTabLength == g.currentIndex + 1) {
                g.btnNext.attr("disabled", "disabled");;
                g.btnDone.show();
            } else {
                g.btnNext.removeAttr("disabled");
                g.btnDone.hide();
            }
        },
        setToolbar: function (params) {
            var g = this,
                p = this.options;
            if (!p.toolbar) return;

            g.tab.addClass("hastoolbar")
            //底部工具栏
            g.tab.toolbar = $('<div class="lee-wizard-toolbar"></div>');
            g.tab.append(g.tab.toolbar);
            g.btnGroup = $('<div class="lee-btn-group"></div>')
            g.btnPrev = $("<button class='lee-btn'><i class='icon-Previous' style='margin-right: 3px;float: left;margin-top: 2px;'></i>上一步</button>");
            g.btnNext = $("<button class='lee-btn'>下一步<i class='icon-Next' style='margin-left: 3px;float: right;margin-top: 2px;'></i></i></button>");
            g.btnDone = $("<button class='lee-btn lee-btn-primary'><i class='lee-ion-checkmark' style='margin-right: 3px;'></i>完成</button>");
            g.btnGroup.append(g.btnPrev).append(g.btnNext);
            g.tab.toolbar.append(g.btnGroup).append(g.btnDone);


            g.btnPrev.click(function () {

                if (g.trigger('prev', [g.currentIndex]) == false)
                    return false;
                g.select(g.currentIndex - 1);
            });
            g.btnNext.click(function () {
                if (g.trigger('next', [g.currentIndex]) == false)
                    return false;
                g.select(g.currentIndex + 1);
            });

            g.btnDone.click(function () {
                g.trigger('confirm', [g.currentIndex]);
            });

        },
        select: function (tabid) {
            var g = this,
                p = this.options;
            var $ele;
            if (typeof (tabid) == "number") {

                $ele = $("> .lee-wizard-content-item[tabindex=" + tabid + "]", g.tab.content);
                tabid = $($ele).attr("tabid");
            } else {
                $ele = $("> .lee-wizard-content-item[tabid=" + tabid + "]", g.tab.content);
            }
            var id = $ele[0].id;
            var tabindex = $ele.attr("tabindex");
            if (g.trigger('beforeSelectTabItem', [tabid, id, tabindex]) == false)
                return false;
            g.selectedTabId = tabid;
            g.currentIndex = Number(tabindex);
            $ele.show().siblings().hide();

            g.activeTabs[tabindex] = true;
            $("li[tabid=" + tabid + "]", g.tab.links.ul).addClass("active").siblings().removeClass("active");
            for (var i = 0; i < Number(tabindex); i++) {
                $("li:eq(" + i + ")", g.tab.links.ul).addClass("complete");
                $("li:eq(" + i + ")>.step", g.tab.links.ul).addClass("lee-ion-checkmark");

            }

            for (var i = Number(tabindex); i < g.allTabLength; i++) {
                $("li:eq(" + i + ")", g.tab.links.ul).removeClass("complete");
                $("li:eq(" + i + ")>.step", g.tab.links.ul).removeClass("lee-ion-checkmark");
            }
            g.setStatus();
            g.trigger('afterSelectTabItem', [tabid, id, tabindex]);
        },
        getNewTabid: function () {
            var g = this,
                p = this.options;
            g.getnewidcount = g.getnewidcount || 0;
            return 'tabitem' + (++g.getnewidcount);
        }
    });

})(jQuery);
(function ($, window) {

    var id = 0;
    var tmpl = "";
    var defaults = {
        pos: "top-center", //
        status: "info",
        autoclose: "true",
        icon: null,
        title: "",
        message: null,
        timeout: "2000",
        icon: null,
        onClose: function () { }
    };
    var messages = {};
    var containers = {};
    var notify = function (options) {
        this.id = id++;
        this.options = $.extend({}, defaults, options);
        this.element = $("<div class='lee-alert'> <div></div></div>");
        this.content(this.getContent());
        if (this.options.status) {
            this.element.addClass(this.options.status);
            if (this.options.icon)
                this.element.addClass("hasicon");
            if (this.options.message)
                this.element.addClass("hasdesc");
            this.currentstatus = this.options.status;
        }
        if (!containers[this.options.pos]) {
            containers[this.options.pos] = $('<div class="lee-message lee-message-' + this.options.pos + '"></div>').appendTo("body");
        }
    };
    notify.prototype.getContent = function () {
        var html = [];
        var p = this.options;
        if (p.icon)
            html.push('<i class = "lee-icon lee-icon-' + p.icon + ' alert-icon"></i>');
        if (p.title) html.push('<span class = "alert-message" > ' + p.title + '</span>');
        if (p.message) html.push('<span class = "alert-description" >' + p.message + '</span>');
        if (p.showclose) html.push('<a class = "close" >< i class = "fa fa-remove" ></i></a>');

        return html.join("");
    }
    notify.prototype.show = function () {
        if (this.element.is(":visible")) return;
        var $this = this;
        containers[this.options.pos].show().prepend(this.element);
        var marginbottom = parseInt(this.element.css("margin-bottom"), 10);
        this.element.css({
            "opacity": 0,
            "margin-top": -1 * this.element.outerHeight(),
            "margin-bottom": 0
        }).

            animate({
                "opacity": 1,
                "margin-top": 0,
                "margin-bottom": marginbottom
            }, function () {

                if ($this.options.timeout) {

                    var closefn = function () {
                        $this.close();
                    };

                    $this.timeout = setTimeout(closefn, $this.options.timeout);

                    $this.element.hover(
                        function () {
                            clearTimeout($this.timeout);
                        },
                        function () {
                            $this.timeout = setTimeout(closefn, $this.options.timeout);
                        }
                    );
                }

            });

        return this;
    };
    notify.prototype.close = function (instantly) {
        var $this = this,
            finalize = function () {
                $this.element.remove();

                if (!containers[$this.options.pos].children().length) {
                    containers[$this.options.pos].hide();
                }

                $this.options.onClose.apply($this, []); //调用关闭事件
                //$this.element.trigger('close.uk.notify', [$this]);

                delete messages[$this.uuid];
            };

        if (this.timeout) clearTimeout(this.timeout);

        if (instantly) {
            finalize();
        } else {
            this.element.animate({
                "opacity": 0,
                "margin-top": -1 * this.element.outerHeight(),
                "margin-bottom": 0
            }, function () {
                finalize();
            });
        }
    };
    notify.prototype.content = function (html) {
        var container = this.element.find(">div");

        if (!html) {
            return container.html();
        }

        container.html(html);

        return this;
    };
    notify.prototype.status = function (status) {

        if (!status) {
            return this.currentstatus;
        }

        this.element.removeClass('ui-message-message-' + this.currentstatus).addClass('uk-notify-message-' + status);

        this.currentstatus = status;

        return this;
    }

    leeUI.Notify = function (options) {
        return (new notify(options)).show();
    };
    leeUI.Success = function (title, message, icon) {
        if (window.top != window && window.top.$ && window.top.$.leeUI) {
            return window.top.leeUI.Success(title, message, icon);
        }
        return (new notify({
            status: "success",
            icon: "success",
            title: title,
            message: message
        })).show();
    };
    leeUI.Tip = function (title, message, icon) {
        if (window.top != window && window.top.$ && window.top.$.leeUI) {
            return window.top.leeUI.Tip(title, message, icon);
        }
        return (new notify({
            status: "tip",
            title: title,
            icon: icon,
            message: message
        })).show();
    };
    leeUI.Tips = function (title, message, icon) {
        if (window.top != window && window.top.$ && window.top.$.leeUI) {
            return window.top.leeUI.Tips(title, message, icon);
        }
        return (new notify({
            status: "tipblack",
            title: title,
            icon: icon,
            message: message
        })).show();
    };
    leeUI.Warning = function (title, message, icon) {
        if (window.top != window && window.top.$ && window.top.$.leeUI) {
            return window.top.leeUI.Warning(title, message, icon);
        }
        icon = icon || "warn";
        return (new notify({
            status: "tip",
            title: title,
            icon: icon,
            message: message
        })).show();
    };
    leeUI.Error = function (title, message, icon) {
        if (window.top != window && window.top.$ && window.top.$.leeUI) {
            return window.top.leeUI.Error(title, message, icon);
        }
        icon = icon || "error";
        return (new notify({
            status: "error",
            title: title,
            icon: icon,
            message: message
        })).show();
    };

}(jQuery, window));
(function ($) {

    $.leeDialog = function () {
        return $.leeUI.run.call(null, "leeUIDialog", arguments, {
            isStatic: true
        });
    };

    $.leeUIDefaults.Dialog = {
        cls: null, //给dialog附加css class
        contentCls: null,
        id: null, //给dialog附加id
        buttons: null, //按钮集合 
        width: 280, //宽度
        height: null, //高度，默认自适应 
        content: '', //内容
        target: null, //目标对象，指定它将以appendTo()的方式载入
        targetBody: false, //载入目标是否在body上
        url: null, //目标页url，默认以iframe的方式载入
        urlParms: null, //传参
        coverMode: true,
        load: false, //是否以load()的方式加载目标页的内容 
        type: 'none', //类型 warn、success、error、question
        left: null, //位置left
        top: null, //位置top
        modal: true, //是否模态对话框
        data: null, //传递数据容器
        name: null, //创建iframe时 作为iframe的name和id 
        opener: null,
        isDrag: true, //是否拖动
        isResize: true, // 是否调整大小
        overflow: null, //弹窗内容是否允许有滚动条
        allowClose: true, //允许关闭
        timeParmName: null, //是否给URL后面加上值为new Date().getTime()的参数，如果需要指定一个参数名即可
        closeWhenEnter: null, //回车时是否关闭dialog
        isHidden: false, //关闭对话框时是否只是隐藏，还是销毁对话框
        show: true, //初始化时是否马上显示
        title: '提示', //头部 
        showMax: true, //是否显示最大化按钮 
        showToggle: false, //是否显示收缩窗口按钮
        showMin: false, //是否显示最小化按钮
        slide: true, //是否以动画的形式显示 
        fixedType: null, //在固定的位置显示, 可以设置的值有n, e, s, w, ne, se, sw, nw
        showType: null, //显示类型,可以设置为slide(固定显示时有效) 
        layoutMode: 1, //1 九宫布局, 2 上中下布局
        onLoaded: null,
        onExtend: null,
        onExtended: null,
        onCollapse: null,
        onCollapseed: null,
        onContentHeightChange: null,
        onClose: null,
        onClosed: null,
        onStopResize: null,
        minIsHide: false //最小化仅隐藏
    };
    $.leeUIDefaults.DialogString = {
        titleMessage: '提示',
        ok: '确定',
        yes: '是',
        no: '否',
        cancel: '取消',
        loadingMessage: "正在加载中....",
        waittingMessage: '正在等待中,请稍候...'
    };

    $.leeUI.controls.Dialog = function (options) {
        $.leeUI.controls.Dialog.base.constructor.call(this, null, options);
    };
    $.leeUI.controls.Dialog.leeExtend($.leeUI.core.Win, {
        __getType: function () {
            return 'Dialog';
        },
        __idPrev: function () {
            return 'Dialog';
        },
        _extendMethods: function () {
            return {};
        },
        _render: function () {
            var g = this,
                p = this.options;
            var tmpId = "";

            if (p.modal && p.coverMode) {
                g.maskid = leeUI.win.getMaskID();
            }
            //alert(p.maskid);
            g.set(p, true);
            var dialog = $('<div class="lee-dialog"><table class="lee-dialog-table" cellpadding="0" cellspacing="0" border="0"><tbody><tr> <td class="lee-dialog-tc"><div class="lee-dialog-tc-inner"><div class="lee-dialog-icon"></div><div class="lee-dialog-title"></div><div class="lee-dialog-winbtns"><div class="lee-icon lee-dialog-winbtn lee-dialog-close"></div></div></div></td></tr><tr><td class="lee-dialog-cc"><div class="lee-dialog-body"><div class="lee-dialog-image lee-icon"></div> <div class="lee-dialog-content"></div><div class="lee-dialog-buttons"><div class="lee-dialog-buttons-inner"></div></td></tr><tr><td class="lee-dialog-bc"></td></tr></tbody></table></div>');
            $('body').append(dialog); //创建布局
            g.dialog = dialog;

            g.element = dialog[0];

            g.dialog.body = $(".lee-dialog-body:first", g.dialog);
            g.dialog.header = $(".lee-dialog-tc-inner:first", g.dialog);
            g.dialog.winbtns = $(".lee-dialog-winbtns:first", g.dialog.header);
            g.dialog.buttons = $(".lee-dialog-buttons:first", g.dialog);
            g.dialog.content = $(".lee-dialog-content:first", g.dialog);
            g.set(p, false);

            if (p.allowClose == false) $(".lee-dialog-close", g.dialog).remove();
            if (p.target || p.url || p.type == "none") {

                p.type = null;
                g.dialog.addClass("lee-dialog-win");
                if (p.target) {
                    p.target.data("display", p.target.css("display"));
                }

            }
            if (p.cls) g.dialog.addClass(p.cls);
            if (p.id) g.dialog.attr("id", p.id);

            //设置锁定屏幕、拖动支持 和设置图片 如果每个弹窗是单独modal需要特殊处理

            g.mask(g.maskid);
            if (p.isDrag)
                g._applyDrag();
            if (p.isResize)
                g._applyResize();
            if (p.type)
                g._setImage();
            else {
                $(".lee-dialog-image", g.dialog).remove();
                g.dialog.content.addClass("lee-dialog-content-noimage");
                if (p.overflow) {
                    g.dialog.content.css("overflow", p.overflow)
                }
            }
            if (p.contentCls)
                g.dialog.content.addClass(p.contentCls);
            if (!p.show) {
                g.unmask(g.maskid);
                g.dialog.hide();
            }
            //设置主体内容
            if (p.target) {
                g.dialog.content.prepend(p.target);
                $(p.target).show();
            } else if (p.url) {
                var url = $.isFunction(p.url) ? p.url.call(g) : p.url;
                var urlParms = $.isFunction(p.urlParms) ? p.urlParms.call(g) : p.urlParms;
                if (p.timeParmName) {
                    urlParms = urlParms || {};
                    urlParms[p.timeParmName] = new Date().getTime();
                }
                if (urlParms) {
                    for (var name in urlParms) {
                        url += url.indexOf('?') == -1 ? "?" : "&";
                        url += name + "=" + urlParms[name];
                    }
                }
                if (p.load) {
                    g.dialog.body.load(url, function () {
                        g._saveStatus();
                        g.trigger('loaded');
                    });
                } else {
                    g.jiframe = $("<iframe frameborder='0'></iframe>");
                    var framename = p.name ? p.name : "ligerwindow" + new Date().getTime();
                    g.jiframe.attr("name", framename);
                    g.jiframe.attr("id", framename);
                    g.dialog.content.prepend(g.jiframe);
                    g.dialog.content.addClass("lee-dialog-content-nopadding lee-dialog-content-frame");

                    setTimeout(function () {
                        if (g.dialog.body.find(".lee-dialog-loading:first").length == 0)
                            g.dialog.body.append("<div class='lee-dialog-loading' style='display:block;'></div>");
                        var iframeloading = $(".lee-dialog-loading:first", g.dialog.body);
                        g.jiframe[0].dialog = g; //增加窗口对dialog对象的引用
                        /*
						可以在子窗口这样使用：
						var dialog = frameElement.dialog;
						var dialogData = dialog.get('data');//获取data参数
						dialog.set('title','新标题'); //设置标题
						dialog.close();//关闭dialog 
						*/
                        g.jiframe.attr("src", url).bind('load.dialog', function () {
                            iframeloading.hide();
                            g.trigger('loaded');
                        });
                        g.frame = window.frames[g.jiframe.attr("name")];
                    }, 0);
                    // 为了解决ie下对含有iframe的div窗口销毁不正确，进而导致第二次打开时焦点不在当前图层的问题
                    // 加入以下代码 
                    tmpId = 'jquery_ligerui_' + new Date().getTime();
                    g.tmpInput = $("<input></input>");
                    g.tmpInput.attr("id", tmpId);
                    g.dialog.content.prepend(g.tmpInput);
                }
            }
            if (p.opener) g.dialog.opener = p.opener;
            //设置按钮
            if (p.buttons) {
                $(p.buttons).each(function (i, item) {

                    var btn =
                        $('<a  href = "javascript:void(0);" class="lee-btn" id="' + item.id + '" style="width: auto;"><span>' + item.text + '</span></a>');
                    //$('<div class="lee-dialog-btn"><div class="lee-dialog-btn-l"></div><div class="lee-dialog-btn-r"></div><div class="lee-dialog-btn-inner"></div></div>');
                    //$(".lee-dialog-btn-inner", btn).html(item.text);
                    $(".lee-dialog-buttons-inner", g.dialog.buttons).append(btn);
                    item.width && btn.width(item.width);
                    item.onclick && btn.click(function () {
                        item.onclick(item, g, i);
                    });
                    if (item.cls) {
                        btn.addClass(item.cls);
                    } else {
                        btn.addClass("lee-btn-primary")
                    }

                });
            } else {
                g.dialog.buttons.remove();
            }
            $(".lee-dialog-buttons-inner", g.dialog.buttons).append("<div class='lee-clear'></div>");

            $(".lee-dialog-title", g.dialog)
                .bind("selectstart", function () {
                    return false;
                });
            g.dialog.click(function () {
                $.leeUI.win.setFront(g);
            });
            //设置事件
            $(".lee-dialog-tc .lee-dialog-close", g.dialog).click(function () {
                if (p.isHidden)
                    g.hide();
                else
                    g.close();
            });
            if (!p.fixedType) {
                if (p.width == 'auto') {
                    setTimeout(function () {
                        resetPos()
                    }, 100);
                } else {
                    resetPos();
                }
            }

            function resetPos() {
                //位置初始化
                var left = 0;
                var top = 0;
                var width = p.width || g.dialog.width();
                if (p.slide == true) p.slide = 'fast';
                if (p.left != null) left = p.left;
                else p.left = left = 0.5 * ($(window).width() - width);
                if (p.top != null) top = p.top;
                else p.top = top = 0.5 * ($(window).height() - g.dialog.height()) + $(window).scrollTop() - 10;
                if (left < 0) p.left = left = 0;
                if (top < 0) p.top = top = 0;
                g.dialog.css({
                    left: left,
                    top: top
                });
            }
            g.show();
            $('body').bind('keydown.dialog', function (e) {
                var key = e.which;
                if (key == 13) {
                    g.enter();
                } else if (key == 27) {
                    g.esc();
                }
            });

            g._updateBtnsWidth();
            g._saveStatus();
            g._onReisze();
            if (tmpId != "") {
                $("#" + tmpId).focus();
                $("#" + tmpId).remove();
            }
        },
        _borderX: 6,
        _borderY: 38,
        doMax: function (slide) {
            var g = this,
                p = this.options;
            $("body").addClass("lee-body-max-dialog")
            var width = $(window).width(),
                height = $(window).height(),
                left = 2,
                top = 2;

            if ($.leeUI.win.taskbar) {
                height -= $.leeUI.win.taskbar.outerHeight();
                if ($.leeUI.win.top) top += $.leeUI.win.taskbar.outerHeight();
            }
            //有工具栏的话
            if (slide) {
                g.dialog.body.animate({
                    width: width - g._borderX
                }, p.slide);
                g.dialog.animate({
                    left: left,
                    top: top
                }, p.slide);
                g.dialog.content.animate({
                    height: height - g._borderY - g.dialog.buttons.outerHeight()
                }, p.slide, function () {
                    g._onReisze();
                });
            } else {

                g.set({
                    width: width,
                    height: height - 2,
                    left: left,
                    top: top
                });
                g._onReisze();
            }
            g.maximum = true;
        },
        //最大化
        max: function () {
            var g = this,
                p = this.options;
            if (g.winmax) {
                g.winmax.addClass("lee-dialog-recover");
                g.doMax(p.slide);
                if (g.wintoggle) {
                    if (g.wintoggle.hasClass("lee-dialog-extend"))
                        g.wintoggle.addClass("lee-dialog-toggle-disabled lee-dialog-extend-disabled");
                    else
                        g.wintoggle.addClass("lee-dialog-toggle-disabled lee-dialog-collapse-disabled");
                }
                if (g.resizable) g.resizable.set({
                    disabled: true
                });
                if (g.draggable) g.draggable.set({
                    disabled: true
                });

                $(window).bind('resize.dialogmax', function () {
                    g.doMax(false);
                });
            }
        },

        //恢复
        recover: function () {
            var g = this,
                p = this.options;
            $("body").removeClass("lee-body-max-dialog")
            if (g.winmax) {
                g.winmax.removeClass("lee-dialog-recover");
                if (p.slide) {
                    g.dialog.body.animate({
                        width: g._width - g._borderX
                    }, p.slide);
                    g.dialog.animate({
                        left: g._left,
                        top: g._top
                    }, p.slide);
                    g.dialog.content.animate({
                        height: g._height - g._borderY - g.dialog.buttons.outerHeight()
                    }, p.slide, function () {
                        g._onReisze();
                    });
                } else {
                    g.set({
                        width: g._width,
                        height: g._height,
                        left: g._left,
                        top: g._top
                    });
                    g._onReisze();
                }
                if (g.wintoggle) {
                    g.wintoggle.removeClass("lee-dialog-toggle-disabled lee-dialog-extend-disabled lee-dialog-collapse-disabled");
                }

                $(window).unbind('resize.dialogmax');
            }
            if (this.resizable) this.resizable.set({
                disabled: false
            });
            if (g.draggable) g.draggable.set({
                disabled: false
            });
            g.maximum = false;
        },

        //最小化
        min: function () {
            var g = this,
                p = this.options;
            if (p.minIsHide) {
                g.dialog.hide();
            } else {
                var task = $.leeUI.win.getTask(this);
                if (p.slide) {
                    g.dialog.body.animate({
                        width: 1
                    }, p.slide);
                    task.y = task.offset().top + task.height();
                    task.x = task.offset().left + task.width() / 2;
                    g.dialog.animate({
                        left: task.x,
                        top: task.y
                    }, p.slide, function () {
                        g.dialog.hide();
                    });
                } else {
                    g.dialog.hide();
                }
            }
            g.unmask(g.maskid);
            g.minimize = true;
            g.actived = false;
        },

        active: function () {
            var g = this,
                p = this.options;
            if (g.minimize) {
                var width = g._width,
                    height = g._height,
                    left = g._left,
                    top = g._top;
                if (g.maximum) {
                    width = $(window).width();
                    height = $(window).height();
                    left = top = 0;
                    if ($.leeUI.win.taskbar) {
                        height -= $.leeUI.win.taskbar.outerHeight();
                        if ($.leeUI.win.top) top += $.leeUI.win.taskbar.outerHeight();
                    }
                }
                if (p.slide) {
                    g.dialog.body.animate({
                        width: width - g._borderX
                    }, p.slide);
                    g.dialog.animate({
                        left: left,
                        top: top
                    }, p.slide);
                } else {
                    g.set({
                        width: width,
                        height: height,
                        left: left,
                        top: top
                    });
                }
            }
            g.actived = true;
            g.minimize = false;
            $.leeUI.win.setFront(g);
            g.show();
        },

        //展开 收缩
        toggle: function () {

            var g = this,
                p = this.options;
            if (!g.wintoggle) return;
            if (g.wintoggle.hasClass("lee-dialog-extend"))
                g.extend();
            else
                g.collapse();
        },

        //收缩
        collapse: function (slide) {
            var g = this,
                p = this.options;
            if (!g.wintoggle) return;
            if (p.slide && slide != false)
                g.dialog.content.animate({
                    height: 1
                }, p.slide);
            else
                g.dialog.content.height(1);
            if (this.resizable) this.resizable.set({
                disabled: true
            });

            g.wintoggle.addClass("lee-dialog-extend");
        },

        //展开
        extend: function () {
            var g = this,
                p = this.options;
            if (!g.wintoggle) return;
            var contentHeight = g._height - g._borderY - g.dialog.buttons.outerHeight();
            if (p.slide)
                g.dialog.content.animate({
                    height: contentHeight
                }, p.slide);
            else
                g.dialog.content.height(contentHeight);
            if (this.resizable) this.resizable.set({
                disabled: false
            });

            g.wintoggle.removeClass("lee-dialog-extend");
        },
        _updateBtnsWidth: function () {
            var g = this;
            var btnscount = $(">div", g.dialog.winbtns).length;
            g.dialog.winbtns.width(22 * btnscount);
        },
        _setLeft: function (value) {
            if (!this.dialog) return;
            if (value != null)
                this.dialog.css({
                    left: value
                });
        },
        _setTop: function (value) {
            if (!this.dialog) return;
            if (value != null)
                this.dialog.css({
                    top: value
                });
        },
        _setWidth: function (value) {
            if (!this.dialog) return;
            if (value >= this._borderX) {
                this.dialog.body.width(value - this._borderX);
            }
        },
        _setHeight: function (value) {
            var g = this,
                p = this.options;
            if (!this.dialog) return;
            if (value == "auto") {
                g.dialog.content.height('auto');
            } else if (value >= this._borderY) {
                var height = value - this._borderY - g.dialog.buttons.outerHeight();
                if (g.trigger('ContentHeightChange', [height]) == false) return;
                if (p.load) {
                    g.dialog.body.height(height);
                } else {
                    g.dialog.content.height(height);
                }
                g.trigger('ContentHeightChanged', [height]);
            }
        },
        _setShowMax: function (value) {
            var g = this,
                p = this.options;
            if (value) {
                if (!g.winmax) {
                    g.winmax = $('<div class="lee-dialog-winbtn lee-icon lee-dialog-max"></div>').appendTo(g.dialog.winbtns)
                        .hover(function () {
                            if ($(this).hasClass("lee-dialog-recover"))
                                $(this).addClass("lee-dialog-recover-over");
                            else
                                $(this).addClass("lee-dialog-max-over");
                        }, function () {
                            $(this).removeClass("lee-dialog-max-over lee-dialog-recover-over");
                        }).click(function () {
                            if ($(this).hasClass("lee-dialog-recover"))
                                g.recover();
                            else
                                g.max();
                        });
                }
            } else if (g.winmax) {
                g.winmax.remove();
                g.winmax = null;
            }
            g._updateBtnsWidth();
        },
        _setShowMin: function (value) {
            var g = this,
                p = this.options;
            if (value) {
                if (!g.winmin) {
                    g.winmin = $('<div class="lee-icon lee-dialog-winbtn lee-dialog-min"></div>').appendTo(g.dialog.winbtns)
                        .hover(function () {
                            $(this).addClass("lee-dialog-min-over");
                        }, function () {
                            $(this).removeClass("lee-dialog-min-over");
                        }).click(function () {
                            g.min();
                        });
                    if (!p.minIsHide) {
                        $.leeUI.win.addTask(g);
                    }
                }
            } else if (g.winmin) {
                g.winmin.remove();
                g.winmin = null;
            }
            g._updateBtnsWidth();
        },
        _setShowToggle: function (value) {
            var g = this,
                p = this.options;
            if (value) {
                if (!g.wintoggle) {
                    g.wintoggle = $('<div class="lee-icon lee-dialog-winbtn lee-dialog-collapse"></div>').appendTo(g.dialog.winbtns)
                        .hover(function () {
                            if ($(this).hasClass("lee-dialog-toggle-disabled")) return;
                            if ($(this).hasClass("lee-dialog-extend"))
                                $(this).addClass("lee-dialog-extend-over");
                            else
                                $(this).addClass("lee-dialog-collapse-over");
                        }, function () {
                            $(this).removeClass("lee-dialog-extend-over lee-dialog-collapse-over");
                        }).click(function () {
                            if ($(this).hasClass("lee-dialog-toggle-disabled")) return;
                            if (g.wintoggle.hasClass("lee-dialog-extend")) {
                                if (g.trigger('extend') == false) return;
                                g.wintoggle.removeClass("lee-dialog-extend");
                                g.extend();
                                g.trigger('extended');
                            } else {
                                if (g.trigger('collapse') == false) return;
                                g.wintoggle.addClass("lee-dialog-extend");
                                g.collapse();
                                g.trigger('collapseed')
                            }
                        });
                }
            } else if (g.wintoggle) {
                g.wintoggle.remove();
                g.wintoggle = null;
            }
        },
        //按下回车
        enter: function () {
            var g = this,
                p = this.options;
            var isClose;
            if (p.closeWhenEnter != undefined) {
                isClose = p.closeWhenEnter;
            } else if (p.type == "warn" || p.type == "error" || p.type == "success" || p.type == "question") {
                isClose = true;
            }
            if (isClose) {
                g.close();
            }
        },
        esc: function () {

        },
        _removeDialog: function () {
            var g = this,
                p = this.options;
            if (p.showType && p.fixedType) {
                g.dialog.animate({
                    bottom: -1 * p.height
                }, function () {
                    remove();
                });
            } else {
                remove();
            }

            function remove() {
                var jframe = $('iframe', g.dialog);
                if (jframe.length) {
                    var frame = jframe[0];
                    frame.src = "about:blank";
                    if (frame.contentWindow && frame.contentWindow.document) {
                        try {
                            frame.contentWindow.document.write('');
                        } catch (e) { }
                    }
                    $.browser.msie && CollectGarbage();
                    jframe.remove();
                }

                if (p.targetBody) {
                    p.target.appendTo("body").css("display", p.target.data("display"));
                }
                g.dialog.remove();
            }
        },
        close: function () {
            var g = this,
                p = this.options;
            if (g.trigger('Close') == false) return;
            g.doClose();
            if (g.trigger('Closed') == false) return;
        },
        doClose: function () {
            var g = this;
            $.leeUI.win.removeTask(this);
            $.leeUI.remove(this);
            g.unmask(g.maskid);
            // 是否取消当前id的mask
            g._removeDialog();
            $('body').unbind('keydown.dialog');
            $("body").removeClass("lee-body-max-dialog")
        },
        _getVisible: function () {
            return this.dialog.is(":visible");
        },
        _setUrl: function (url) {
            var g = this,
                p = this.options;
            p.url = url;
            if (p.load) {
                g.dialog.body.html("").load(p.url, function () {
                    g.trigger('loaded');
                });
            } else if (g.jiframe) {
                g.jiframe.attr("src", p.url);
            }
        },
        _setContent: function (content) {
            this.dialog.content.html(content);
        },
        _setTitle: function (value) {
            var g = this;
            var p = this.options;
            if (value) {
                $(".lee-dialog-title", g.dialog).html(value);
            }
        },
        _hideDialog: function () {
            var g = this,
                p = this.options;
            if (p.showType && p.fixedType) {
                g.dialog.animate({
                    bottom: -1 * p.height
                }, function () {
                    g.dialog.hide();
                });
            } else {
                g.dialog.hide();
            }
        },
        hidden: function () {
            var g = this;
            $.leeUI.win.removeTask(g);
            g.dialog.hide();
            g.unmask(g.maskid);
        },
        show: function () {
            var g = this,
                p = this.options;
            g.mask(g.maskid);
            if (p.fixedType) {
                if (p.showType) {
                    g.dialog.css({
                        bottom: -1 * p.height
                    }).addClass("lee-dialog-fixed");
                    g.dialog.show().animate({
                        bottom: 0
                    });
                } else {
                    g.dialog.show().css({
                        bottom: 0
                    }).addClass("lee-dialog-fixed");
                }
            } else {
                g.dialog.show();
            }
            //前端显示 
            if (p.coverMode) {
                g.dialog.css("z-index", g.maskid + 9005);
            } else {
                $.leeUI.win.setFront.leeDefer($.leeUI.win, 100, [g]);
            }
        },
        setUrl: function (url) {
            this._setUrl(url);
        },
        _saveStatus: function () {
            var g = this;
            g._width = g.dialog.body.width();
            g._height = g.dialog.body.height();
            var top = 0;
            var left = 0;
            if (!isNaN(parseInt(g.dialog.css('top'))))
                top = parseInt(g.dialog.css('top'));
            if (!isNaN(parseInt(g.dialog.css('left'))))
                left = parseInt(g.dialog.css('left'));
            g._top = top;
            g._left = left;
        },
        _applyDrag: function () {
            var g = this,
                p = this.options;
            if ($.fn.leeDrag) {
                g.draggable = g.dialog.leeDrag({
                    handler: '.lee-dialog-title',
                    animate: false,
                    onStartDrag: function () {

                        $.leeUI.win.setFront(g);
                        var mask = $("<div class='lee-dragging-mask' style='display:block'></div>").height(g.dialog.height());
                        g.dialog.append(mask);
                        g.dialog.content.addClass('lee-dialog-content-dragging');
                    },
                    onDrag: function (current, e) {

                        var pageY = e.pageY || e.screenY;
                        if (pageY < 0) return false;
                    },
                    onStopDrag: function () {

                        g.dialog.find("div.lee-dragging-mask:first").remove();
                        g.dialog.content.removeClass('lee-dialog-content-dragging');
                        if (p.target) {
                            var triggers1 = $.leeUI.find($.leeUI.controls.DateEditor);
                            var triggers2 = $.leeUI.find($.leeUI.controls.ComboBox);
                            //更新所有下拉选择框的位置
                            $($.merge(triggers1, triggers2)).each(function () {
                                if (this.updateSelectBoxPosition)
                                    this.updateSelectBoxPosition();
                            });
                        }
                        g._saveStatus();
                    }
                });
            }
        },
        _onReisze: function () {
            var g = this,
                p = this.options;
            if (p.target || p.url) {
                var manager = $(p.target).leeUI();
                if (!manager) manager = $(p.target).find(":first").leeUI();
                if (!manager) return;
                var contentHeight = g.dialog.content.height();
                var contentWidth = g.dialog.content.width();
                manager.trigger('resize', [{
                    width: contentWidth,
                    height: contentHeight
                }]);
            }
        },
        _applyResize: function () {
            var g = this,
                p = this.options;
            if ($.fn.leeResizable) {
                g.resizable = g.dialog.leeResizable({
                    onStopResize: function (current, e) {
                        var top = 0;
                        var left = 0;
                        if (!isNaN(parseInt(g.dialog.css('top'))))
                            top = parseInt(g.dialog.css('top'));
                        if (!isNaN(parseInt(g.dialog.css('left'))))
                            left = parseInt(g.dialog.css('left'));
                        if (current.diffLeft) {
                            g.set({
                                left: left + current.diffLeft
                            });
                        }
                        if (current.diffTop) {
                            g.set({
                                top: top + current.diffTop
                            });
                        }
                        if (current.newWidth) {
                            g.set({
                                width: current.newWidth
                            });
                            g.dialog.body.css({
                                width: current.newWidth - g._borderX
                            });
                        }
                        if (current.newHeight) {
                            g.set({
                                height: current.newHeight
                            });
                        }
                        g._onReisze();
                        g._saveStatus();
                        g.trigger('stopResize');
                        return false;
                    },
                    animate: false
                });
            }
        },
        _setImage: function () {
            var g = this,
                p = this.options;
            if (p.type) {
                var alertCss = {
                    paddingLeft: 60,
                    paddingRight: 15,
                    paddingBottom: 30
                };
                if (p.type == 'success' || p.type == 'donne' || p.type == 'ok') {
                    $(".lee-dialog-image", g.dialog).addClass("lee-dialog-image-donne").show();
                    g.dialog.content.css(alertCss);
                } else if (p.type == 'error') {
                    $(".lee-dialog-image", g.dialog).addClass("lee-dialog-image-error").show();
                    g.dialog.content.css(alertCss);
                } else if (p.type == 'warn') {
                    $(".lee-dialog-image", g.dialog).addClass("lee-dialog-image-warn").show();
                    g.dialog.content.css(alertCss);
                } else if (p.type == 'question') {
                    $(".lee-dialog-image", g.dialog).addClass("lee-dialog-image-question").show();
                    g.dialog.content.css(alertCss);
                }
            }
        }
    });

    $.leeUI.controls.Dialog.prototype.hide = $.leeUI.controls.Dialog.prototype.hidden;

    $.leeDialog.open = function (p) {
        return $.leeDialog(p);
    };
    $.leeDialog.close = function () {
        var dialogs = $.leeUI.find($.leeUI.controls.Dialog.prototype.__getType());
        for (var i in dialogs) {
            var d = dialogs[i];
            d.destroy.leeDefer(d, 5);
        }
        $.leeUI.win.unmask();
    };
    $.leeDialog.show = function (p) {
        var dialogs = $.leeUI$.leeUI.find($.leeUI.controls.Dialog.prototype.__getType());
        if (dialogs.length) {
            for (var i in dialogs) {
                dialogs[i].show();
                return;
            }
        }
        return $.leeDialog(p);
    };
    $.leeDialog.hide = function () {
        var dialogs = $.leeUI.find($.leeUI.controls.Dialog.prototype.__getType());
        for (var i in dialogs) {
            var d = dialogs[i];
            d.hide();
        }
    };
    $.leeDialog.tip = function (options) {
        options = $.extend({
            showType: 'slide',
            width: 240,
            modal: false,
            height: 100
        }, options || {});

        $.extend(options, {
            fixedType: 'se',
            type: 'none',
            isDrag: false,
            isResize: false,
            showMax: false,
            showToggle: false,
            showMin: false
        });
        return $.leeDialog.open(options);
    };
    $.leeDialog.alert = function (content, title, type, callback, options) {
        content = content || "";
        if (typeof (title) == "function") {
            callback = title;
            type = null;
        } else if (typeof (type) == "function") {
            callback = type;
        }
        var btnclick = function (item, Dialog, index) {
            Dialog.close();
            if (callback)
                callback(item, Dialog, index);
        };
        p = {
            content: content,
            buttons: [{
                text: $.leeUIDefaults.DialogString.ok,
                onclick: btnclick
            }]
        };
        if (typeof (title) == "string" && title != "") p.title = title;
        if (typeof (type) == "string" && type != "") p.type = type;
        $.extend(p, {
            showMax: false,
            showToggle: false,
            showMin: false
        }, options || {});
        return $.leeDialog(p);
    };

    $.leeDialog.confirm = function (content, title, callback) {
        if (typeof (title) == "function") {
            callback = title;
            type = null;
        }
        var btnclick = function (item, Dialog) {
            Dialog.close();
            if (callback) {
                callback(item.type == 'ok');
            }
        };
        p = {
            type: 'question',
            content: content,
            buttons: [{
                text: $.leeUIDefaults.DialogString.yes,
                onclick: btnclick,
                type: 'ok',
                cls: 'lee-btn-primary lee-dialog-btn-ok'
            }, {
                text: $.leeUIDefaults.DialogString.no,
                onclick: btnclick,
                type: 'no',
                cls: 'lee-dialog-btn-no'
            }]
        };
        if (typeof (title) == "string" && title != "") p.title = title;
        $.extend(p, {
            showMax: false,
            showToggle: false,
            showMin: false
        });
        return $.leeDialog(p);
    };
    $.leeDialog.warning = function (content, title, callback, options) {
        if (typeof (title) == "function") {
            callback = title;
            type = null;
        }
        var btnclick = function (item, Dialog) {
            Dialog.close();
            if (callback) {
                callback(item.type);
            }
        };
        p = {
            type: 'question',
            content: content,
            buttons: [{
                text: $.leeUIDefaults.DialogString.yes,
                onclick: btnclick,
                type: 'yes'
            }, {
                text: $.leeUIDefaults.DialogString.no,
                onclick: btnclick,
                type: 'no'
            }, {
                text: $.leeUIDefaults.DialogString.cancel,
                onclick: btnclick,
                type: 'cancel'
            }]
        };
        if (typeof (title) == "string" && title != "") p.title = title;
        $.extend(p, {
            showMax: false,
            showToggle: false,
            showMin: false
        }, options || {});
        return $.leeDialog(p);
    };

    $.leeDialog.loading = function (title, modal) {
        if (window.top != window && window.top.$ && window.top.$.leeDialog) {
            window.top.$.leeDialog.loading(title, modal);
            return;
        }
        var html = [];
        title = title || $.leeUIDefaults.Dialog.loadingMessage;

        if ($("#lee-global-loading").length > 0) {
            if (modal) {
                $("#lee-global-mask").show();
            }
            $("#lee-global-loading .message").html(title);
            $("#lee-global-loading").show();
        } else {
            html.push("<div id='lee-global-mask' class='lee-modal-mask'></div>");
            html.push('<div  id="lee-global-loading" class="lee-loading"><div class="lee-grid-loading-inner"><div class="loader"></div><div class="message">' + title + '</div></div></div>');
            var $loading = $(html.join(""));
            $("body").append($loading);
            $loading.show();
        }
    }
    $.leeDialog.hideLoading = function () {
        if (window.top != window && window.top.$ && window.top.$.leeDialog) {
            window.top.$.leeDialog.hideLoading();
            return;
        }
        $("#lee-global-mask").hide();
        $("#lee-global-loading").hide();
    }
    $.leeDialog.waitting = function (title) {
        title = title || $.leeUIDefaults.Dialog.waittingMessage;
        return $.leeDialog.open({
            cls: 'lee-dialog-waittingdialog',
            type: 'none',
            content: '<div style="padding:4px">' + title + '</div>',
            allowClose: false
        });
    };
    $.leeDialog.closeWaitting = function () {
        var dialogs = $.leeUI.find($.leeUI.controls.Dialog);
        for (var i in dialogs) {
            var d = dialogs[i];
            if (d.dialog.hasClass("lee-dialog-waittingdialog"))
                d.close();
        }
    };
    $.leeDialog.success = function (content, title, onBtnClick, options) {
        if (window.top != window && window.top.$ && window.top.$.leeDialog) {
            return window.top.$.leeDialog.alert(content, title, 'success', onBtnClick, options);

        }
        return $.leeDialog.alert(content, title, 'success', onBtnClick, options);
    };
    $.leeDialog.error = function (content, title, onBtnClick, options) {
        if (window.top != window && window.top.$ && window.top.$.leeDialog) {
            return window.top.$.leeDialog.alert(content, title, 'error', onBtnClick, options);
        }
        return $.leeDialog.alert(content, title, 'error', onBtnClick, options);
    };
    $.leeDialog.warn = function (content, title, onBtnClick, options) {
        if (window.top != window && window.top.$ && window.top.$.leeDialog) {
            return window.top.$.leeDialog.alert(content, title, 'warn', onBtnClick, options);
        }
        return $.leeDialog.alert(content, title, 'warn', onBtnClick, options);
    };
    $.leeDialog.question = function (content, title, options) {
        if (window.top != window && window.top.$ && window.top.$.leeDialog) {
            return window.top.$.leeDialog.alert(content, title, 'question', onBtnClick, options);
        }
        return $.leeDialog.alert(content, title, 'question', null, options);
    };

    $.leeDialog.prompt = function (title, value, multi, callback) {
        var target = $('<input type="text" class="lee-dialog-inputtext"/>');
        if (typeof (multi) == "function") {
            callback = multi;
        }
        if (typeof (value) == "function") {
            callback = value;
        } else if (typeof (value) == "boolean") {
            multi = value;
        }
        if (typeof (multi) == "boolean" && multi) {
            target = $('<textarea class="lee-dialog-textarea"></textarea>');
        }
        if (typeof (value) == "string" || typeof (value) == "int") {
            target.val(value);
        }
        var btnclick = function (item, Dialog, index) {
            Dialog.close();
            if (callback) {
                callback(item.type == 'yes', target.val());
            }
        }
        p = {
            title: title,
            target: target,
            width: 320,
            buttons: [{
                text: $.leeUIDefaults.DialogString.ok,
                onclick: btnclick,
                type: 'yes'
            }, {
                text: $.leeUIDefaults.DialogString.cancel,
                onclick: btnclick,
                type: 'cancel'
            }]
        };
        return $.leeDialog(p);
    };

})(jQuery);
//bug问题记录
//1.关闭弹窗  最大化时候的取消滚动样式要取消
(function ($) {
    $.fn.leeTree = function (options) {
        return $.leeUI.run.call(this, "leeUITree", arguments);
    };

    $.fn.leeGetTreeManager = function () {
        return $.leeUI.run.call(this, "leeUIGetTreeManager", arguments);
    };

    $.leeUIDefaults.Tree = {
        url: null,
        urlParms: null, //url带参数
        data: null,
        width: "auto",
        checkbox: false,
        autoCheckboxEven: true, //复选框级联？
        enabledCompleteCheckbox: true, //是否启用半选择
        parentIcon: 'folder', //'folder',icon-tfs-tcm-static-suite 
        childIcon: 'leaf', //'leaf',
        textFieldName: 'text',
        attribute: ['id', 'url'],
        treeLine: true, //是否显示line
        nodeWidth: 90,
        statusName: '__status',
        isLeaf: null, //是否子节点的判断函数
        single: false, //是否单选
        needCancel: true, //已选的是否需要取消操作
        idFieldName: 'id',
        parentIDFieldName: "pid",
        topParentIDValue: 0,
        slide: false, //是否以动画的形式显示
        iconFieldName: 'icon',
        nodeDraggable: false, //是否允许拖拽
        nodeDraggingRender: null,
        btnClickToToggleOnly: true, //是否点击展开/收缩 按钮时才有效
        ajaxType: 'post',
        ajaxContentType: null,
        render: null, //自定义函数
        selectable: null, //可选择判断函数
        /*
		是否展开 
		    1,可以是true/false 
		    2,也可以是数字(层次)N 代表第1层到第N层都是展开的，其他收缩
		    3,或者是判断函数 函数参数e(data,level) 返回true/false

		优先级没有节点数据的isexpand属性高,并没有delay属性高
		*/
        isExpand: null,
        /*
		是否延迟加载 
		    1,可以是true/false 
		    2,也可以是数字(层次)N 代表第N层延迟加载 
		    3,或者是字符串(Url) 加载数据的远程地址
		    4,如果是数组,代表这些层都延迟加载,如[1,2]代表第1、2层延迟加载
		    5,再是函数(运行时动态获取延迟加载参数) 函数参数e(data,level),返回true/false或者{url:...,parms:...}

		优先级没有节点数据的delay属性高
		*/
        delay: null,
        idField: null, //id字段
        parentIDField: null, //parent id字段，可用于线性数据转换为tree数据
        iconClsFieldName: "iconclass",
        onBeforeAppend: function () { }, //加载数据前事件，可以通过return false取消操作
        onAppend: function () { }, //加载数据时事件，对数据进行预处理以后
        onAfterAppend: function () { }, //加载数据完事件
        onBeforeExpand: function () { },
        onContextmenu: function () { },
        onExpand: function () { },
        onBeforeCollapse: function () { },
        onCollapse: function () { },
        onBeforeSelect: function () { },
        onSelect: function () { },
        onBeforeCancelSelect: function () { },
        onCancelselect: function () { },
        onCheck: function () { },
        onSuccess: function () { },
        onError: function () { },
        onClick: function () { },
        onDblclick: function () { }

    };

    $.leeUI.controls.Tree = function (element, options) {
        $.leeUI.controls.Tree.base.constructor.call(this, element, options);
    };
    $.leeUI.controls.Tree.leeExtend($.leeUI.core.UIComponent, {
        _init: function () {
            $.leeUI.controls.Tree.base._init.call(this); //初始父类构造函数
            var g = this,
                p = this.options;
            if (p.single) p.autoCheckboxEven = false; //单选则取消级联
        },
        _render: function () {
            var g = this,
                p = this.options;
            g.set(p, true);
            g.tree = $(g.element);
            g.tree.addClass('lee-tree');
            g.toggleNodeCallbacks = [];
            g.sysAttribute = ['isexpand', 'ischecked', 'href', 'style', 'delay'];
            g.loading = $("<div class='lee-tree-loading'></div>");
            g.tree.after(g.loading);
            g.data = [];
            g.maxOutlineLevel = 1;
            g.treedataindex = 0;
            g._applyTree();
            g._setTreeEven();
            g.set(p, false);
        },
        _setDisabled: function (flag) {
        },
        _applyTree: function () { //初始化树
            var g = this,
                p = this.options;
            g.data = []; //g._getDataByTreeHTML(g.tree);
            var gridhtmlarr = g._getTreeHTMLByData(g.data, 1, [], true);
            gridhtmlarr[gridhtmlarr.length - 1] = gridhtmlarr[0] = "";
            g.tree.html(gridhtmlarr.join('')); //设置格式
            //g._upadteTreeWidth();
            //绑定hover事件
            $(".lee-body", g.tree).hover(function () {
                $(this).addClass("lee-over");
            }, function () {
                $(this).removeClass("lee-over");
            });
        },
        //增加节点集合
        //parm [newdata] 数据集合 Array
        //parm [parentNode] dom节点(li)、节点数据 或者节点 dataindex null 为根节点
        //parm [nearNode] 附加到节点的上方/下方(非必填)
        //parm [isAfter] 附加到节点的下方(非必填)
        append: function (parentNode, newdata, nearNode, isAfter) {
            var g = this,
                p = this.options;
            parentNode = g.getNodeDom(parentNode);
            if (g.trigger('beforeAppend', [parentNode, newdata]) == false) return false; //触发追加时间
            if (!newdata || !newdata.length) return false;
            if (p.idFieldName && p.parentIDFieldName)
                newdata = g.arrayToTree(newdata, p.idFieldName, p.parentIDFieldName); //格式化数据
            g._addTreeDataIndexToData(newdata);
            g._setTreeDataStatus(newdata, 'add');
            if (nearNode != null) {
                nearNode = g.getNodeDom(nearNode);
            }
            g.trigger('append', [parentNode, newdata])
            g._appendData(parentNode, newdata);
            if (parentNode == null) //增加到根节点
            {
                var gridhtmlarr = g._getTreeHTMLByData(newdata, 1, [], true);
                gridhtmlarr[gridhtmlarr.length - 1] = gridhtmlarr[0] = "";
                if (nearNode != null) {
                    $(nearNode)[isAfter ? 'after' : 'before'](gridhtmlarr.join(''));
                    g._updateStyle(parentNode ? $("ul:first", parentNode) : g.tree);
                } else {
                    //remove last node class
                    if ($("> li:last", g.tree).length > 0)
                        g._setTreeItem($("> li:last", g.tree)[0], { isLast: false });
                    g.tree.append(gridhtmlarr.join(''));
                }
                $(".lee-body", g.tree).hover(function () {
                    $(this).addClass("lee-over");
                }, function () {
                    $(this).removeClass("lee-over");
                });
                g._upadteTreeWidth();
                g.trigger('afterAppend', [parentNode, newdata])
                return;
            }
            var treeitem = $(parentNode);
            var outlineLevel = parseInt(treeitem.attr("outlinelevel"));

            var hasChildren = $("> ul", treeitem).length > 0;
            if (!hasChildren) {
                treeitem.append("<ul class='lee-children'></ul>");
                //设置为父节点
                g.upgrade(parentNode);
            }
            var isLast = [];
            for (var i = 1; i <= outlineLevel - 1; i++) {
                var currentParentTreeItem = $(g.getParentTreeItem(parentNode, i));
                isLast.push(currentParentTreeItem.hasClass("lee-last"));
            }
            isLast.push(treeitem.hasClass("lee-last"));
            var gridhtmlarr = g._getTreeHTMLByData(newdata, outlineLevel + 1, isLast, true);
            gridhtmlarr[gridhtmlarr.length - 1] = gridhtmlarr[0] = "";
            if (nearNode != null) {
                $(nearNode)[isAfter ? 'after' : 'before'](gridhtmlarr.join(''));
                g._updateStyle(parentNode ? $("ul:first", parentNode) : g.tree);
            } else {
                //remove last node class  
                if ($("> .lee-children > li:last", treeitem).length > 0)
                    g._setTreeItem($("> .lee-children > li:last", treeitem)[0], { isLast: false });
                $(">.lee-children", parentNode).append(gridhtmlarr.join(''));
            }
            g._upadteTreeWidth();
            $(">.lee-children .lee-body", parentNode).hover(function () {
                $(this).addClass("lee-over");
            }, function () {
                $(this).removeClass("lee-over");
            });
            g.trigger('afterAppend', [parentNode, newdata]);
        },
        loadData: function (node, url, param, e) {
            var g = this,
                p = this.options;
            e = $.extend({
                showLoading: function () {
                    g.loading.show();
                },
                success: function () { },
                error: function () { },
                hideLoading: function () {
                    g.loading.hide();
                }
            }, e || {});
            var ajaxtype = p.ajaxType;
            //解决树无法设置parms的问题
            param = $.extend(($.isFunction(p.parms) ? p.parms() : p.parms), param);
            if (p.ajaxContentType == "application/json" && typeof (param) != "string") {
                param = liger.toJSON(param);
            }
            var urlParms = $.isFunction(p.urlParms) ? p.urlParms.call(g) : p.urlParms;
            if (urlParms) {
                for (name in urlParms) {
                    url += url.indexOf('?') == -1 ? "?" : "&";
                    url += name + "=" + urlParms[name];
                }
            }
            var ajaxOp = {
                type: ajaxtype,
                url: url,
                data: param,
                dataType: 'json',
                beforeSend: function () {
                    e.showLoading();
                },
                success: function (data) {
                    if (!data) return;
                    if (p.idField && p.parentIDField) {
                        data = g.arrayToTree(data, p.idField, p.parentIDField);
                    }
                    e.hideLoading();
                    g.append(node, data); //加载数据 节点
                    g.trigger('success', [data]);
                    e.success(data);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    try {
                        e.hideLoading();
                        g.trigger('error', [XMLHttpRequest, textStatus, errorThrown]);
                        e.error(XMLHttpRequest, textStatus, errorThrown);
                    } catch (e) {

                    }
                }
            };
            if (p.ajaxContentType) {
                ajaxOp.contentType = p.ajaxContentType;
            }
            $.ajax(ajaxOp);
        },
        _setParms: function () {
            var g = this,
                p = this.options;
            if ($.isFunction(p.parms)) p.parms = p.parms();
        },
        _setTreeLine: function (value) {
            //虚线节点
            if (value) this.tree.removeClass("lee-tree-noline");
            else this.tree.addClass("lee-tree-noline");
        },
        _setUrl: function (url) {
            var g = this,
                p = this.options;
            if (url) {
                g.clear();
                g.loadData(null, url);
            }
        },
        _setData: function (data) {
            if (data) {
                this.clear();
                this.append(null, data); //追加数据
            }
        },
        //设置树的点击事件
        _setTreeEven: function () {
            var g = this,
                p = this.options;
            //单击
            g.tree.click(function (e) {
                //绑定树单击事件
                var obj = (e.target || e.srcElement);
                var treeitem = null;
                if (obj.tagName.toLowerCase() == "a" || obj.tagName.toLowerCase() == "span" || $(obj).hasClass("lee-box"))
                    treeitem = $(obj).parent().parent();
                else if ($(obj).hasClass("lee-body"))
                    treeitem = $(obj).parent();
                else
                    treeitem = $(obj);
                if (!treeitem) return;
                var treedataindex = parseInt(treeitem.attr("treedataindex"));
                var treenodedata = g._getDataNodeByTreeDataIndex(g.data, treedataindex);
                var treeitembtn = $("div.lee-body:first", treeitem).find("div.lee-expandable-open:first,div.lee-expandable-close:first");
                var clickOnTreeItemBtn = $(obj).hasClass("lee-expandable-open") || $(obj).hasClass("lee-expandable-close");
                //是否点击的展开收起的加号
                if (!$(obj).hasClass("lee-checkbox") && !clickOnTreeItemBtn) {
                    if (!treeitem.hasClass("lee-unselectable")) {
                        if ($(">div:first", treeitem).hasClass("lee-selected") && p.needCancel) {
                            if (g.trigger('beforeCancelSelect', [{ data: treenodedata, target: treeitem[0] }]) == false)
                                return false;

                            $(">div:first", treeitem).removeClass("lee-selected");
                            g.trigger('cancelSelect', [{ data: treenodedata, target: treeitem[0] }]);
                        } else {
                            if (g.trigger('beforeSelect', [{ data: treenodedata, target: treeitem[0] }]) == false)
                                return false;
                            $(".lee-body", g.tree).removeClass("lee-selected");
                            $(">div:first", treeitem).addClass("lee-selected");
                            g.trigger('select', [{ data: treenodedata, target: treeitem[0] }])
                        }
                    }
                }
                //chekcbox even
                if ($(obj).hasClass("lee-checkbox")) {
                    if (p.autoCheckboxEven) {
                        //状态：未选中
                        if ($(obj).hasClass("lee-checkbox-unchecked")) {
                            $(obj).removeClass("lee-checkbox-unchecked").addClass("lee-checkbox-checked");
                            $(".lee-children .lee-checkbox", treeitem)
                                .removeClass("lee-checkbox-incomplete lee-checkbox-unchecked")
                                .addClass("lee-checkbox-checked");
                            g.trigger('check', [{ data: treenodedata, target: treeitem[0] }, true]);
                        }
                        //状态：选中
                        else if ($(obj).hasClass("lee-checkbox-checked")) {
                            $(obj).removeClass("lee-checkbox-checked").addClass("lee-checkbox-unchecked");
                            $(".lee-children .lee-checkbox", treeitem)
                                .removeClass("lee-checkbox-incomplete lee-checkbox-checked")
                                .addClass("lee-checkbox-unchecked");
                            g.trigger('check', [{ data: treenodedata, target: treeitem[0] }, false]);
                        }
                        //状态：未完全选中
                        else if ($(obj).hasClass("lee-checkbox-incomplete")) {
                            $(obj).removeClass("lee-checkbox-incomplete").addClass("lee-checkbox-checked");
                            $(".lee-children .lee-checkbox", treeitem)
                                .removeClass("lee-checkbox-incomplete lee-checkbox-unchecked")
                                .addClass("lee-checkbox-checked");
                            g.trigger('check', [{ data: treenodedata, target: treeitem[0] }, true]);
                        }
                        g._setParentCheckboxStatus(treeitem);
                    } else {
                        //状态：未选中
                        if ($(obj).hasClass("lee-checkbox-unchecked")) {
                            $(obj).removeClass("lee-checkbox-unchecked").addClass("lee-checkbox-checked");
                            //是否单选
                            if (p.single) {
                                $(".lee-checkbox", g.tree).not(obj).removeClass("lee-checkbox-checked").addClass("lee-checkbox-unchecked");
                            }
                            g.trigger('check', [{ data: treenodedata, target: treeitem[0] }, true]);
                        }
                        //状态：选中
                        else if ($(obj).hasClass("lee-checkbox-checked")) {
                            $(obj).removeClass("lee-checkbox-checked").addClass("lee-checkbox-unchecked");
                            g.trigger('check', [{ data: treenodedata, target: treeitem[0] }, false]);
                        }
                    }
                }
                //状态：已经张开
                else if (treeitembtn.hasClass("lee-expandable-open") && (!p.btnClickToToggleOnly || clickOnTreeItemBtn)) {
                    if (g.trigger('beforeCollapse', [{ data: treenodedata, target: treeitem[0] }]) == false)
                        return false;
                    treeitembtn.removeClass("lee-expandable-open").addClass("lee-expandable-close");
                    if (p.slide)
                        $("> .lee-children", treeitem).slideToggle('fast');
                    else
                        $("> .lee-children", treeitem).hide();
                    $("> div ." + g._getParentNodeClassName(true), treeitem)
                        .removeClass(g._getParentNodeClassName(true))
                        .addClass(g._getParentNodeClassName());
                    g.trigger('collapse', [{ data: treenodedata, target: treeitem[0] }]);
                }
                //状态：没有张开
                else if (treeitembtn.hasClass("lee-expandable-close") && (!p.btnClickToToggleOnly || clickOnTreeItemBtn)) {
                    if (g.trigger('beforeExpand', [{ data: treenodedata, target: treeitem[0] }]) == false)
                        return false;

                    $(g.toggleNodeCallbacks).each(function () {
                        if (this.data == treenodedata) {
                            this.callback(treeitem[0], treenodedata);
                        }
                    });
                    treeitembtn.removeClass("lee-expandable-close").addClass("lee-expandable-open");
                    var callback = function () {
                        g.trigger('expand', [{ data: treenodedata, target: treeitem[0] }]);
                    };
                    if (p.slide) {
                        $("> .lee-children", treeitem).slideToggle('fast', callback);
                    } else {
                        $("> .lee-children", treeitem).show();
                        callback();
                    }
                    $("> div ." + g._getParentNodeClassName(), treeitem)
                        .removeClass(g._getParentNodeClassName())
                        .addClass(g._getParentNodeClassName(true));
                }
                g.trigger('click', [{ data: treenodedata, target: treeitem[0] }]);
            });


            g.tree.dblclick(function (e) {
                var obj = (e.target || e.srcElement);
                var treeitem = null;
                if (obj.tagName.toLowerCase() == "a" || obj.tagName.toLowerCase() == "span" || $(obj).hasClass("lee-box"))
                    treeitem = $(obj).parent().parent();
                else if ($(obj).hasClass("lee-body"))
                    treeitem = $(obj).parent();
                else
                    treeitem = $(obj);
                if (!treeitem) return;
                var treedataindex = parseInt(treeitem.attr("treedataindex"));
                var treenodedata = g._getDataNodeByTreeDataIndex(g.data, treedataindex);
                g.trigger('dblclick', [{ data: treenodedata, target: treeitem[0] }]);
            });

        },
        _getTreeHTMLByData: function (data, outlineLevel, isLast, isExpand) {
            var g = this,
                p = this.options;
            if (g.maxOutlineLevel < outlineLevel)
                g.maxOutlineLevel = outlineLevel;
            isLast = isLast || [];
            outlineLevel = outlineLevel || 1;
            var treehtmlarr = [];
            if (!isExpand) //如果默认不展开
                treehtmlarr.push('<ul class="lee-children" style="display:none">');
            else
                treehtmlarr.push("<ul class='lee-children'>");
            for (var i = 0; i < data.length; i++) {
                var o = data[i];
                var isFirst = i == 0;
                var isLastCurrent = i == data.length - 1;
                var delay = g._getDelay(o, outlineLevel);
                var isExpandCurrent = delay ? false : g._isExpand(o, outlineLevel);
                treehtmlarr.push('<li ');
                if (o.treedataindex != undefined)
                    treehtmlarr.push('treedataindex="' + o.treedataindex + '" ');

                if (o[p.idFieldName])
                    treehtmlarr.push('data-id="' + o[p.idFieldName] + '" ');
                if (isExpandCurrent)
                    treehtmlarr.push('isexpand=' + o.isexpand + ' ');
                treehtmlarr.push('outlinelevel=' + outlineLevel + ' ');
                //属性支持

                treehtmlarr.push('class="');
                isFirst && treehtmlarr.push('lee-first '); //首节点
                isLastCurrent && treehtmlarr.push('lee-last '); // 尾部
                isFirst && isLastCurrent && treehtmlarr.push('lee-onlychild '); //只有一个节点
                treehtmlarr.push('"');
                treehtmlarr.push('>');
                treehtmlarr.push('<div class="lee-body');
                //				if (p.selectable && p.selectable(o) == false)
                //              {
                //                  treehtmlarr.push(' lee-unselectable');
                //              }
                treehtmlarr.push('">');
                for (var k = 0; k <= outlineLevel - 2; k++) {
                    //if(isLast[k]) treehtmlarr.push('<div class="lee-box"></div>');
                    //else 
                    treehtmlarr.push('<div class="lee-box lee-line"></div>'); //占位tab 从第二级开始
                }
                if (g.hasChildren(o)) {
                    //是否展开的判断
                    if (isExpandCurrent) treehtmlarr.push('<div class="lee-box lee-expandable-open"></div>');
                    else treehtmlarr.push('<div class="lee-box lee-expandable-close"></div>');
                    //是否有复选框
                    if (p.checkbox) {
                        if (o.ischecked)
                            treehtmlarr.push('<div class="lee-box lee-checkbox lee-checkbox-checked"></div>');
                        else
                            treehtmlarr.push('<div class="lee-box lee-checkbox lee-checkbox-unchecked"></div>');
                    }
                    if (p.parentIcon) {
                        //node icon
                        treehtmlarr.push('<div class="lee-box lee-tree-icon '); //添加图标内容
                        treehtmlarr.push(g._getParentNodeClassName(isExpandCurrent ? true : false) + " ");

                        //添加没有图标样式
                        if (p.iconFieldName && o[p.iconFieldName]) {
                            treehtmlarr.push('lee-tree-icon-none');
                        } else if (p.iconClsFieldName && o[p.iconClsFieldName]) {

                            treehtmlarr.push('lee-tree-icon-none');
                        }

                        treehtmlarr.push('">');

                        if (p.iconFieldName && o[p.iconFieldName])
                            treehtmlarr.push('<img src="' + o[p.iconFieldName] + '" />');
                        else if (p.iconClsFieldName && o[p.iconClsFieldName])
                            treehtmlarr.push('<i class="icon-img  ' + o[p.iconClsFieldName] + '"></i>');
                        treehtmlarr.push('</div>');
                    }

                } else {
                    //如果是叶子节点
                    //添加同级最后一个节点css标记子节点
                    if (isLastCurrent) treehtmlarr.push('<div class="lee-box lee-node-last"></div>');
                    else treehtmlarr.push('<div class="lee-box lee-node"></div>');
                    //如果有checkbox
                    if (p.checkbox) {
                        if (o.ischecked)
                            treehtmlarr.push('<div class="lee-box lee-checkbox lee-checkbox-checked"></div>');
                        else
                            treehtmlarr.push('<div class="lee-box lee-checkbox lee-checkbox-unchecked"></div>');
                    }
                    if (p.childIcon) {
                        //node icon 
                        treehtmlarr.push('<div class="lee-box lee-tree-icon ');
                        treehtmlarr.push(g._getChildNodeClassName() + " ");
                        if ((p.iconFieldName && o[p.iconFieldName]) || p.iconClsFieldName && o[p.iconClsFieldName])
                            treehtmlarr.push('lee-tree-icon-none');

                        treehtmlarr.push('">');
                        if (p.iconFieldName && o[p.iconFieldName])
                            treehtmlarr.push('<img src="' + o[p.iconFieldName] + '" />');
                        else if (p.iconClsFieldName && o[p.iconClsFieldName])
                            treehtmlarr.push('<i class="icon-img  ' + o[p.iconClsFieldName] + '"></i>');
                        treehtmlarr.push('</div>');
                    }
                }

                //添加树形文字 如果有自定义渲染 那么则处理html
                if (p.render) {
                    treehtmlarr.push('<span>' + p.render(o, o[p.textFieldName]) + '</span>');
                } else {
                    treehtmlarr.push('<span>' + o[p.textFieldName] + '</span>');
                }
                treehtmlarr.push('</div>');

                if (g.hasChildren(o)) {
                    var isLastNew = [];
                    for (var k = 0; k < isLast.length; k++) {
                        isLastNew.push(isLast[k]);
                    }
                    isLastNew.push(isLastCurrent);
                    if (delay) {
                        if (delay == true) {
                            g.toggleNodeCallbacks.push({
                                data: o,
                                callback: function (dom, o) {
                                    var content = g._getTreeHTMLByData(o.children, outlineLevel + 1, isLastNew, isExpandCurrent).join('');
                                    $(dom).append(content);
                                    $(">.lee-children .lee-body", dom).hover(function () {
                                        $(this).addClass("lee-over");
                                    }, function () {
                                        $(this).removeClass("lee-over");
                                    });
                                    g._removeToggleNodeCallback(o);
                                }
                            });
                        } else if (delay.url) {
                            (function (o, url, parms) {
                                g.toggleNodeCallbacks.push({
                                    data: o,
                                    callback: function (dom, o) {
                                        g.loadData(dom, url, parms, {
                                            showLoading: function () {
                                                $("div.lee-expandable-close:first", dom).addClass("lee-box-loading");
                                            },
                                            hideLoading: function () {
                                                $("div.lee-box-loading:first", dom).removeClass("lee-box-loading");
                                            }
                                        });
                                        g._removeToggleNodeCallback(o);
                                    }
                                });
                            })(o, delay.url, delay.parms);
                        }
                    } else {
                        // 这里处理延时加载 to do
                        treehtmlarr.push(g._getTreeHTMLByData(o.children, outlineLevel + 1, isLastNew, isExpandCurrent).join(''));
                    }
                    //添加子集树Html
                }
                treehtmlarr.push('</li>');
            }
            treehtmlarr.push("</ul>");
            return treehtmlarr;
        },
        hasChildren: function (treenodedata) {
            if (this.options.isLeaf) return !this.options.isLeaf(treenodedata);
            //如果有自定义判断函数 则走自定义判断是否孩子节点函数 不然则判断children
            return treenodedata.children ? true : false;
        },
        //获取父节点 数据
        getParent: function (treenode, level) {
            var g = this;
            treenode = g.getNodeDom(treenode);
            var parentTreeNode = g.getParentTreeItem(treenode, level);
            if (!parentTreeNode) return null;
            var parentIndex = $(parentTreeNode).attr("treedataindex");
            return g._getDataNodeByTreeDataIndex(g.data, parentIndex);
        },
        //获取父节点
        getParentTreeItem: function (treenode, level) {
            var g = this;
            treenode = g.getNodeDom(treenode);
            var treeitem = $(treenode);
            if (treeitem.parent().hasClass("lee-tree"))
                return null;
            if (level == undefined) {
                if (treeitem.parent().parent("li").length == 0)
                    return null;
                return treeitem.parent().parent("li")[0];
            }
            var currentLevel = parseInt(treeitem.attr("outlinelevel"));
            var currenttreeitem = treeitem;
            for (var i = currentLevel - 1; i >= level; i--) {
                currenttreeitem = currenttreeitem.parent().parent("li");
            }
            return currenttreeitem[0];
        },
        getChecked: function () {
            var g = this, p = this.options;
            if (!this.options.checkbox) return null;
            var nodes = [];
            $(".lee-checkbox-checked", g.tree).parent().parent("li").each(function () {
                var treedataindex = parseInt($(this).attr("treedataindex"));
                nodes.push({ target: this, data: g._getDataNodeByTreeDataIndex(g.data, treedataindex) });
            });
            return nodes;
        },
        getCheckedData: function () {
            var g = this, p = this.options;
            if (!this.options.checkbox) return null;
            var nodes = [];
            $(".lee-checkbox-checked", g.tree).parent().parent("li").each(function () {
                var treedataindex = parseInt($(this).attr("treedataindex"));
                nodes.push(g._getDataNodeByTreeDataIndex(g.data, treedataindex));
            });
            return nodes;
        },



        //add by superzoc 12/24/2012 
        refreshTree: function () {
            var g = this, p = this.options;
            $.each(this.getChecked(), function (k, v) {
                g._setParentCheckboxStatus($(v.target));
            });
        },
        getSelected: function () {
            var g = this, p = this.options;
            var node = {};
            node.target = $(".lee-selected", g.tree).parent("li")[0];
            if (node.target) {
                var treedataindex = parseInt($(node.target).attr("treedataindex"));
                node.data = g._getDataNodeByTreeDataIndex(g.data, treedataindex);
                return node;
            }
            return null;
        },
        //升级为父节点级别
        upgrade: function (treeNode) {
            var g = this, p = this.options;
            $(".lee-note", treeNode).each(function () {
                $(this).removeClass("lee-note").addClass("lee-expandable-open");
            });
            $(".lee-note-last", treeNode).each(function () {
                $(this).removeClass("lee-note-last").addClass("lee-expandable-open");
            });
            $("." + g._getChildNodeClassName(), treeNode).each(function () {
                $(this)
                    .removeClass(g._getChildNodeClassName())
                    .addClass(g._getParentNodeClassName(true));
            });
        },
        //降级为叶节点级别
        demotion: function (treeNode) {
            var g = this, p = this.options;
            if (!treeNode && treeNode[0].tagName.toLowerCase() != 'li') return;
            var islast = $(treeNode).hasClass("lee-last");
            $(".lee-expandable-open", treeNode).each(function () {
                $(this).removeClass("lee-expandable-open")
                    .addClass(islast ? "lee-note-last" : "lee-note");
            });
            $(".lee-expandable-close", treeNode).each(function () {
                $(this).removeClass("lee-expandable-close")
                    .addClass(islast ? "lee-note-last" : "lee-note");
            });
            $("." + g._getParentNodeClassName(true), treeNode).each(function () {
                $(this)
                    .removeClass(g._getParentNodeClassName(true))
                    .addClass(g._getChildNodeClassName());
            });
        },
        collapseAll: function () {
            var g = this, p = this.options;
            $(".lee-expandable-open", g.tree).click();
        },
        expandAll: function () {
            var g = this, p = this.options;
            $(".lee-expandable-close", g.tree).click();
        },
        hide: function (treeNode) {
            var g = this, p = this.options;
            treeNode = g.getNodeDom(treeNode);
            if (treeNode) $(treeNode).hide();
        },
        show: function (treeNode) {
            var g = this, p = this.options;
            treeNode = g.getNodeDom(treeNode);
            if (treeNode) $(treeNode).show();
        },
        //parm [treeNode] dom节点(li)、节点数据 或者节点 dataindex
        remove: function (treeNode) {
            var g = this, p = this.options;
            treeNode = g.getNodeDom(treeNode);
            var treedataindex = parseInt($(treeNode).attr("treedataindex"));
            var treenodedata = g._getDataNodeByTreeDataIndex(g.data, treedataindex);
            if (treenodedata) g._setTreeDataStatus([treenodedata], 'delete');
            var parentNode = g.getParentTreeItem(treeNode);
            //复选框处理
            if (p.checkbox) {
                g._setParentCheckboxStatus($(treeNode));
            }
            $(treeNode).remove();
            g._updateStyle(parentNode ? $("ul:first", parentNode) : g.tree);
        },
        //parm [domnode] dom节点(li)、节点数据 或者节点 dataindex
        update: function (domnode, newnodedata) {
            var g = this, p = this.options;
            domnode = g.getNodeDom(domnode);
            var treedataindex = parseInt($(domnode).attr("treedataindex"));
            nodedata = g._getDataNodeByTreeDataIndex(g.data, treedataindex);
            for (var attr in newnodedata) {
                nodedata[attr] = newnodedata[attr];
                if (attr == p.textFieldName) {
                    $("> .lee-body > span", domnode).text(newnodedata[attr]);
                }
            }
        },
        _getChildNodeClassName: function () {
            var g = this,
                p = this.options;
            nodeclassname = 'icon-img icon-document  ';
            // return 'lee-tree-icon-' + p.childIcon; //获取子节点的样式 图标
            return nodeclassname;
        },
        _getParentNodeClassName: function (isOpen) {
            var g = this,
                p = this.options;
            var nodeclassname = 'lee-tree-icon-' + p.parentIcon; //获取父节点的样式 图标

            //var nodeclassname = 'icon-img icon-folder';

            if (isOpen) {
                //nodeclassname += '-open';
                //nodeclassname = 'icon-img icon-folder-open';
            }

            return nodeclassname;
        },
        //判断节点是否展开状态,返回true/false
        _isExpand: function (o, level) {
            var g = this,
                p = this.options;
            var isExpand = o.isExpand != null ? o.isExpand : (o.isexpand != null ? o.isexpand : p.isExpand);
            if (isExpand == null) return true;
            if (typeof (isExpand) == "function") isExpand = p.isExpand({ data: o, level: level });
            if (typeof (isExpand) == "boolean") return isExpand;
            if (typeof (isExpand) == "string") return isExpand == "true";
            if (typeof (isExpand) == "number") return isExpand > level;
            return true;
        },
        //设置数据状态
        _setTreeDataStatus: function (data, status) {
            var g = this,
                p = this.options;
            $(data).each(function () {
                this[p.statusName] = status;
                if (this.children) {
                    g._setTreeDataStatus(this.children, status);
                }
            });
        },
        //根据数据索引获取数据
        _getDataNodeByTreeDataIndex: function (data, treedataindex) {
            var g = this,
                p = this.options;
            for (var i = 0; i < data.length; i++) {
                if (data[i].treedataindex == treedataindex)
                    return data[i];
                if (data[i].children) {
                    var targetData = g._getDataNodeByTreeDataIndex(data[i].children, treedataindex);
                    if (targetData) return targetData;
                }
            }
            return null;
        },
        //设置data 索引
        _addTreeDataIndexToData: function (data) {
            var g = this,
                p = this.options;
            $(data).each(function () {
                if (this.treedataindex != undefined) return;
                this.treedataindex = g.treedataindex++;
                if (this.children) {
                    g._addTreeDataIndexToData(this.children);
                }
            });
        },
        _appendData: function (treeNode, data) {
            var g = this,
                p = this.options;
            var treedataindex = parseInt($(treeNode).attr("treedataindex"));
            var treenodedata = g._getDataNodeByTreeDataIndex(g.data, treedataindex);
            if (g.treedataindex == undefined) g.treedataindex = 0;
            if (treenodedata && treenodedata.children == undefined) treenodedata.children = [];
            $(data).each(function (i, item) {
                if (treenodedata)
                    treenodedata.children[treenodedata.children.length] = item;
                else
                    g.data[g.data.length] = item;
                g._addToNodes(item);
            });
        },
        _addToNodes: function (data) {
            var g = this,
                p = this.options;
            g.nodes = g.nodes || [];
            g.nodes.push(data);
            if (!data.children) return;
            $(data.children).each(function (i, item) {
                g._addToNodes(item);
            });
        },
        //递归设置父节点的状态
        _setParentCheckboxStatus: function (treeitem) {

            var g = this,
                p = this.options;
            //当前同级别或低级别的节点是否都选中了
            var isCheckedComplete = $(".lee-checkbox-unchecked", treeitem.parent()).length == 0;
            //当前同级别或低级别的节点是否都没有选中
            var isCheckedNull = $(".lee-checkbox-checked", treeitem.parent()).length == 0;

            if (isCheckedNull) {
                treeitem.parent().prev().find("> .lee-checkbox")
                    .removeClass("lee-checkbox-checked lee-checkbox-incomplete")
                    .addClass("lee-checkbox-unchecked");
            } else {
                if (isCheckedComplete || !p.enabledCompleteCheckbox) {
                    treeitem.parent().prev().find(".lee-checkbox")
                        .removeClass("lee-checkbox-unchecked lee-checkbox-incomplete")
                        .addClass("lee-checkbox-checked");
                } else {
                    treeitem.parent().prev().find("> .lee-checkbox")
                        .removeClass("lee-checkbox-unchecked lee-checkbox-checked")
                        .addClass("lee-checkbox-incomplete");
                }
            }
            if (treeitem.parent().parent("li").length > 0)
                g._setParentCheckboxStatus(treeitem.parent().parent("li"));
        },
        _setTreeItem: function (treeNode, options) {
            var g = this,
                p = this.options;
            if (!options) return;
            treeNode = g.getNodeDom(treeNode);
            var treeItem = $(treeNode);
            var outlineLevel = parseInt(treeItem.attr("outlinelevel"));
            if (options.isLast != undefined) {
                if (options.isLast == true) {
                    treeItem.removeClass("lee-last").addClass("lee-last");
                    $("> div .lee-node", treeItem).removeClass("lee-node").addClass("lee-node-last");
                    $(".lee-children li", treeItem)
                        .find(".lee-box:eq(" + (outlineLevel - 1) + ")")
                        .removeClass("lee-line");
                } else if (options.isLast == false) {
                    treeItem.removeClass("lee-last");
                    $("> div .lee-node-last", treeItem).removeClass("lee-node-last").addClass("lee-node");

                    $(".lee-children li", treeItem)
                        .find(".lee-box:eq(" + (outlineLevel - 1) + ")")
                        .removeClass("lee-line")
                        .addClass("lee-line");
                }
            }
        },
        _updateStyle: function (ul) {
            var g = this,
                p = this.options;
            var itmes = $(" > li", ul);
            var treeitemlength = itmes.length;
            if (!treeitemlength) return;
            //遍历设置子节点的样式
            itmes.each(function (i, item) {
                if (i == 0 && !$(this).hasClass("lee-first"))
                    $(this).addClass("lee-first");
                if (i == treeitemlength - 1 && !$(this).hasClass("lee-last"))
                    $(this).addClass("lee-last");
                if (i == 0 && i == treeitemlength - 1)
                    $(this).addClass("lee-onlychild");
                $("> div .lee-note,> div .lee-note-last", this)
                    .removeClass("lee-note lee-note-last")
                    .addClass(i == treeitemlength - 1 ? "lee-note-last" : "lee-note");
                g._setTreeItem(this, { isLast: i == treeitemlength - 1 });
            });
        },
        _upadteTreeWidth: function () {
            var g = this,
                p = this.options;
            if (p.width == "auto") {
                g.tree.width("auto");
                return;
            }
            var treeWidth = g.maxOutlineLevel * 22;
            if (p.checkbox) treeWidth += 22;
            if (p.parentIcon || p.childIcon) treeWidth += 22;
            treeWidth += p.nodeWidth;
            g.tree.width(treeWidth);

        },
        //获取节点的延迟加载状态,返回true/false (本地模式) 或者是object({url :'...',parms:null})(远程模式)
        _getDelay: function (o, level) {
            var g = this,
                p = this.options;
            var delay = o.delay != null ? o.delay : p.delay;
            if (delay == null) return false;
            if (typeof (delay) == "function") delay = delay({ data: o, level: level });
            if (typeof (delay) == "boolean") return delay;
            if (typeof (delay) == "string") return { url: delay };
            if (typeof (delay) == "number") delay = [delay];
            if ($.isArray(delay)) return $.inArray(level, delay) != -1;
            if (typeof (delay) == "object" && delay.url) return delay;
            return false;
        },
        _removeToggleNodeCallback: function (nodeData) {
            var g = this,
                p = this.options;
            for (var i = 0; i <= g.toggleNodeCallbacks.length; i++) {
                if (g.toggleNodeCallbacks[i] && g.toggleNodeCallbacks[i].data == nodeData) {
                    g.toggleNodeCallbacks.splice(i, 1); //删除节点事件
                    break;
                }
            }
        },
        getTextByID: function (id) {
            var g = this,
                p = this.options;
            var data = g.getDataByID(id);
            if (!data) return null;
            return data[p.textFieldName];
        },
        getDataByID: function (id) {
            var g = this,
                p = this.options;
            var data = null;

            if (g.data && g.data.length) {
                return find(g.data);
            }

            function find(items) {
                for (var i = 0; i < items.length; i++) {
                    var dataItem = items[i];
                    if (dataItem[p.idFieldName] == id) return dataItem;
                    if (dataItem.children && dataItem.children.length) {
                        var o = find(dataItem.children);
                        if (o) return o;
                    }
                }
                return null;
            }

            $("li", g.tree).each(function () {
                if (data) return;
                var treeitem = $(this);
                var treedataindex = parseInt(treeitem.attr("treedataindex"));
                var treenodedata = g._getDataNodeByTreeDataIndex(g.data, treedataindex);
                if (treenodedata[p.idFieldName].toString() == id.toString()) {
                    data = treenodedata;
                }
            });
            return data;
        },
        arrayToTree: function (data, id, pid) {
            //将ID、ParentID这种数据格式转换为树格式
            var g = this,
                p = this.options;
            var childrenName = "children";
            if (!data || !data.length) return [];
            var targetData = []; //存储数据的容器(返回) 
            var records = {};
            var itemLength = data.length; //数据集合的个数
            for (var i = 0; i < itemLength; i++) {
                var o = data[i];
                var key = getKey(o[id]);
                records[key] = o;
            }
            for (var i = 0; i < itemLength; i++) {
                var currentData = data[i];
                var key = getKey(currentData[pid]);
                var parentData = records[key];
                if (!parentData) {
                    targetData.push(currentData);
                    continue;
                }
                parentData[childrenName] = parentData[childrenName] || [];
                parentData[childrenName].push(currentData);
            }
            return targetData;

            function getKey(key) {
                if (typeof (key) == "string") key = key.replace(/[.]/g, '').toLowerCase();
                return key;
            }
        },
        getNodeDom: function (nodeParm) {
            //根据节点参数获取树节点
            var g = this,
                p = this.options;
            if (nodeParm == null) return nodeParm;
            if (typeof (nodeParm) == "string" || typeof (nodeParm) == "number") {
                return $("li[treedataindex=" + nodeParm + "]", g.tree).get(0);
            } else if (typeof (nodeParm) == "object" && 'treedataindex' in nodeParm) //nodedata
            {
                return g.getNodeDom(nodeParm['treedataindex']);
            } else if (nodeParm.target && nodeParm.data) {
                return nodeParm.target;
            }
            return nodeParm;
        },
        setData: function (data) {
            this.set('data', data);
        },
        getData: function () {
            return this.data;
        },
        //parm [nodeParm] dom节点(li)、节点数据 或者节点 dataindex
        cancelSelect: function (nodeParm, isTriggerEvent) {
            var g = this,
                p = this.options;
            var domNode = g.getNodeDom(nodeParm);
            var treeitem = $(domNode);
            var treedataindex = parseInt(treeitem.attr("treedataindex"));
            var treenodedata = g._getDataNodeByTreeDataIndex(g.data, treedataindex);
            var treeitembody = $(">div:first", treeitem);
            if (p.checkbox)
                $(".lee-checkbox", treeitembody).removeClass("lee-checkbox-checked").addClass("lee-checkbox-unchecked");
            else
                treeitembody.removeClass("lee-selected");
            if (isTriggerEvent != false) {
                g.trigger('cancelSelect', [{ data: treenodedata, target: treeitem[0] }]);
            }
        },
        //选择节点(参数：条件函数、Dom节点或ID值)
        selectNode: function (selectNodeParm, isTriggerEvent) {
            var g = this,
                p = this.options;
            var clause = null;
            if (typeof (selectNodeParm) == "function") {
                clause = selectNodeParm;
            } else if (typeof (selectNodeParm) == "object") {
                var treeitem = $(selectNodeParm);
                var treedataindex = parseInt(treeitem.attr("treedataindex"));
                var treenodedata = g._getDataNodeByTreeDataIndex(g.data, treedataindex);
                var treeitembody = $(">div:first", treeitem);
                if (!treeitembody.length) {
                    treeitembody = $("li[treedataindex=" + treedataindex + "] >div:first", g.tree);
                }
                if (p.checkbox) {
                    $(".lee-checkbox", treeitembody).removeClass("lee-checkbox-unchecked").addClass("lee-checkbox-checked");
                } else {
                    $("div.lee-selected", g.tree).removeClass("lee-selected");
                    treeitembody.addClass("lee-selected");
                }
                if (isTriggerEvent != false) {
                    g.trigger('select', [{ data: treenodedata, target: treeitembody.parent().get(0) }]);
                }
                return;
            } else {
                clause = function (data) {
                    if (!data[p.idFieldName] && data[p.idFieldName] != 0) return false;
                    return strTrim(data[p.idFieldName].toString()) == strTrim(selectNodeParm.toString());
                };
            }
            $("li", g.tree).each(function () {
                var treeitem = $(this);
                var treedataindex = parseInt(treeitem.attr("treedataindex"));
                var treenodedata = g._getDataNodeByTreeDataIndex(g.data, treedataindex);
                if (clause(treenodedata, treedataindex)) {
                    g.selectNode(this, isTriggerEvent);
                } else {
                    //修复多选checkbox为true时调用该方法会取消已经选中节点的问题
                    if (!g.options.checkbox) {
                        g.cancelSelect(this, isTriggerEvent);
                    }
                }
            });
        },
        clear: function (node) {
            //清空树
            var g = this,
                p = this.options;
            if (!node) {
                g.toggleNodeCallbacks = [];
                g.data = null;
                g.data = [];
                g.nodes = null;
                g.tree.html("");
            } else {

                var nodeDom = g.getNodeDom(node);
                var nodeData = g._getDataNodeByTreeDataIndex(g.data, $(nodeDom).attr("treedataindex"));
                $(nodeDom).find("ul.lee-children").remove();
                if (nodeData) nodeData.children = [];
            }
        },
        reload: function (callback) {
            var g = this,
                p = this.options;
            g.clear();
            g.loadData(null, p.url, null, {
                success: callback
            });
        },
        //刷新节点
        reloadNode: function (node, data, callback) {
            var g = this,
                p = this.options;
            g.clear(node);
            if (typeof (data) == "string") {
                g.loadData(node, data, null, {
                    success: callback
                });
            } else {
                if (!data) return;
                if (p.idField && p.parentIDField) {
                    data = g.arrayToTree(data, p.idField, p.parentIDField);
                }
                g.append(node, data);
            }
        }

    });

    function strTrim(str) {
        if (!str) return str;
        return str.replace(/(^\s*)|(\s*$)/g, '');
    };
})(jQuery);
/**
 * jQuery ligerUI 1.3.3
 * 
 * http://ligerui.com
 *  
 * Author daomi 2015 [ gd_star@163.com ] 
 * 
 */
(function ($) {
    $.fn.leeLayout = function (options) {
        return $.leeUI.run.call(this, "leeUILayout", arguments);
    };

    $.fn.leeGetLayoutManager = function () {
        return $.leeUI.run.call(this, "leeUIGetLayoutManager", arguments);
    };

    $.leeUIDefaults.Layout = {
        topHeight: 50,
        bottomHeight: 40,
        leftWidth: 110,
        centerWidth: 300,
        rightWidth: 170,
        centerBottomHeight: 100,
        allowCenterBottomResize: true,
        inWindow: true, //是否以窗口的高度为准 height设置为百分比时可用
        heightDiff: 0, //高度补差
        height: '100%', //高度
        onHeightChanged: null,
        isLeftCollapse: false, //初始化时 左边是否隐藏
        isRightCollapse: false, //初始化时 右边是否隐藏
        allowLeftCollapse: true, //是否允许 左边可以隐藏
        allowRightCollapse: true, //是否允许 右边可以隐藏
        allowLeftResize: true, //是否允许 左边可以调整大小
        allowRightResize: true, //是否允许 右边可以调整大小
        allowTopResize: true, //是否允许 头部可以调整大小
        allowBottomResize: true, //是否允许 底部可以调整大小
        space: 3, //间隔 
        onEndResize: null, //调整大小结束事件
        minLeftWidth: 80, //调整左侧宽度时的最小允许宽度
        minRightWidth: 80, //调整右侧宽度时的最小允许宽度  
        onLeftToggle: null, //左边收缩/展开事件
        onRightToggle: null //右边收缩/展开事件
    };

    $.leeUI.controls.Layout = function (element, options) {
        $.leeUI.controls.Layout.base.constructor.call(this, element, options);
    };
    $.leeUI.controls.Layout.leeExtend($.leeUI.core.UIComponent, {
        __getType: function () {
            return 'Layout';
        },
        __idPrev: function () {
            return 'Layout';
        },
        _extendMethods: function () {
            return {};
        },
        _init: function () {
            $.leeUI.controls.Layout.base._init.call(this);

            var g = this,
                p = this.options;
            if (p.InWindow != null && p.inWindow == null) p.inWindow = p.InWindow; //旧版本命名错误纠正
        },
        _render: function () {
            var g = this,
                p = this.options;
            g.layout = $(this.element);
            g.layout.addClass("lee-layout"); //添加布局
            g.width = g.layout.width(); //计算宽度
            //top
            if ($("> div[position=top]", g.layout).length > 0) {
                g.top = $("> div[position=top]", g.layout).wrap('<div class="lee-layout-top" style="top:0px;"></div>').parent();
                g.top.content = $("> div[position=top]", g.top);
                if (!g.top.content.hasClass("lee-layout-content"))
                    g.top.content.addClass("lee-layout-content");
                g.topHeight = p.topHeight;
                if (g.topHeight) {
                    g.top.height(g.topHeight);
                }
            }
            //bottom
            if ($("> div[position=bottom]", g.layout).length > 0) {
                g.bottom = $("> div[position=bottom]", g.layout).wrap('<div class="lee-layout-bottom"></div>').parent();
                g.bottom.content = $("> div[position=bottom]", g.bottom);
                if (!g.bottom.content.hasClass("lee-layout-content"))
                    g.bottom.content.addClass("lee-layout-content");

                g.bottomHeight = p.bottomHeight;
                if (g.bottomHeight) {
                    g.bottom.height(g.bottomHeight);
                }
                //set title
                var bottomtitle = g.bottom.content.attr("title");
                if (bottomtitle) {
                    g.bottom.header = $('<div class="lee-layout-header"></div>');
                    g.bottom.prepend(g.bottom.header);
                    g.bottom.header.html(bottomtitle);
                    g.bottom.content.attr("title", "");
                }
            }
            //left
            if ($("> div[position=left]", g.layout).length > 0) {
                g.left = $("> div[position=left]", g.layout).wrap('<div class="lee-layout-left" style="left:0px;"></div>').parent();

                g.left.content = $("> div[position=left]", g.left);
                //if (!g.left.content.hasClass("lee-layout-content"))
                g.left.content.addClass("lee-layout-content");

                //set title
                var lefttitle = g.left.content.attr("title");
                if (lefttitle) {
                    g.left.content.attr("title", "");
                    g.left.header = $('<div class="lee-layout-header"><div class="lee-icon lee-layout-header-toggle"></div><div class="lee-layout-header-inner"></div></div>');
                    g.left.prepend(g.left.header);
                    g.left.header.toggle = $(".lee-layout-header-toggle", g.left.header);
                    if (!p.allowLeftCollapse) $(".lee-layout-header-toggle", g.left.header).remove();
                    $(".lee-layout-header-inner", g.left.header).html(lefttitle);
                } else {
                    g.left.content.css("top", "0");
                }
                //set title 
                if (g.left.content.attr("hidetitle")) {
                    g.left.content.attr("title", "");
                    g.left.header.remove();
                }
                //set width
                g.leftWidth = p.leftWidth;
                if (g.leftWidth)
                    g.left.width(g.leftWidth);
            }
            //center
            if ($("> div[position=center]", g.layout).length > 0) {
                g.center = $("> div[position=center]", g.layout).wrap('<div class="lee-layout-center" ></div>').parent();
                g.center.content = $("> div[position=center]", g.center);
                g.center.content.addClass("lee-layout-content");
                //set title
                var centertitle = g.center.content.attr("title");
                if (centertitle) {
                    g.center.content.attr("title", "");
                    g.center.header = $('<div class="lee-layout-header"></div>');
                    g.center.prepend(g.center.header);
                    g.center.header.html(centertitle);
                }
                if (g.center.content.attr("hidetitle")) {
                    g.center.content.attr("title", "");
                    g.center.header.remove();
                }
                //set width
                g.centerWidth = p.centerWidth;
                if (g.centerWidth)
                    g.center.width(g.centerWidth);

                //centerBottom
                if ($("> div[position=centerbottom]", g.layout).length > 0) {
                    g.centerBottom = $("> div[position=centerbottom]", g.layout).wrap('<div class="lee-layout-centerbottom" ></div>').parent();
                    g.centerBottom.content = $("> div[position=centerbottom]", g.centerBottom);
                    g.centerBottom.content.addClass("lee-layout-content");
                    //set title
                    var centertitle = g.centerBottom.content.attr("title");
                    if (centertitle) {
                        g.centerBottom.content.attr("title", "");
                        g.centerBottom.header = $('<div class="lee-layout-header"></div>');
                        g.centerBottom.prepend(g.centerBottom.header);
                        g.centerBottom.header.html(centertitle);
                    }
                    if (g.centerBottom.content.attr("hidetitle")) {
                        g.centerBottom.content.attr("title", "");
                        if (g.centerBottom.header) {
                            g.centerBottom.header.remove();
                        }
                    }
                    if (g.centerWidth)
                        g.centerBottom.width(g.centerWidth);
                }
            }
            //right
            if ($("> div[position=right]", g.layout).length > 0) {
                g.right = $("> div[position=right]", g.layout).wrap('<div class="lee-layout-right"></div>').parent();

                g.right.header = $('<div class="lee-layout-header"><div class="lee-icon lee-layout-header-toggle"></div><div class="lee-layout-header-inner"></div></div>');
                g.right.prepend(g.right.header);
                g.right.header.toggle = $(".lee-layout-header-toggle", g.right.header);
                if (!p.allowRightCollapse) $(".lee-layout-header-toggle", g.right.header).remove();
                g.right.content = $("> div[position=right]", g.right);
                if (!g.right.content.hasClass("lee-layout-content"))
                    g.right.content.addClass("lee-layout-content");

                //set title
                var righttitle = g.right.content.attr("title");
                if (righttitle) {
                    g.right.content.attr("title", "");
                    $(".lee-layout-header-inner", g.right.header).html(righttitle);
                }
                if (g.right.content.attr("hidetitle")) {
                    g.right.content.attr("title", "");
                    g.right.header.remove();
                }
                //set width
                g.rightWidth = p.rightWidth;
                if (g.rightWidth)
                    g.right.width(g.rightWidth);
            }
            //lock
            g.layout.lock = $("<div class='lee-layout-lock'></div>");
            g.layout.append(g.layout.lock);
            //DropHandle
            g._addDropHandle();

            //Collapse
            g.isLeftCollapse = p.isLeftCollapse;
            g.isRightCollapse = p.isRightCollapse;
            g.leftCollapse = $('<div class="lee-layout-collapse-left" style="display: none; "><div class="lee-icon lee-layout-collapse-left-toggle"></div></div>');
            g.rightCollapse = $('<div class="lee-layout-collapse-right" style="display: none; "><div class="lee-icon lee-layout-collapse-right-toggle"></div></div>');
            g.layout.append(g.leftCollapse).append(g.rightCollapse);
            g.leftCollapse.toggle = $("> .lee-layout-collapse-left-toggle", g.leftCollapse);
            g.rightCollapse.toggle = $("> .lee-layout-collapse-right-toggle", g.rightCollapse);
            g._setCollapse();
            //init
            g._bulid();
            $(window).resize(function () {
                g._onResize();
            });
            g.set(p);
            g.mask.height(g.layout.height());
        },
        setLeftCollapse: function (isCollapse) {
            var g = this,
                p = this.options;
            if (!g.left) return false;
            g.isLeftCollapse = isCollapse;
            if (g.isLeftCollapse) {
                g.leftCollapse.show();
                g.leftDropHandle && g.leftDropHandle.hide();
                g.left.hide();
            } else {
                g.leftCollapse.hide();
                g.leftDropHandle && g.leftDropHandle.show();
                g.left.show();
            }
            g._onResize();

            g.trigger('leftToggle', [isCollapse]);
        },
        setRightCollapse: function (isCollapse) {
            var g = this,
                p = this.options;
            if (!g.right) return false;
            g.isRightCollapse = isCollapse;
            g._onResize();
            if (g.isRightCollapse) {
                g.rightCollapse.show();
                g.rightDropHandle && g.rightDropHandle.hide();
                g.right.hide();
            } else {
                g.rightCollapse.hide();
                g.rightDropHandle && g.rightDropHandle.show();
                g.right.show();
            }
            g._onResize();

            g.trigger('rightToggle', [isCollapse]);
        },
        _bulid: function () {
            var g = this,
                p = this.options;
            $("> .lee-layout-left .lee-layout-header,> .lee-layout-right .lee-layout-header", g.layout).hover(function () {
                $(this).addClass("lee-layout-header-over");
            }, function () {
                $(this).removeClass("lee-layout-header-over");

            });
            $(".lee-layout-header-toggle", g.layout).hover(function () {
                $(this).addClass("lee-layout-header-toggle-over");
            }, function () {
                $(this).removeClass("lee-layout-header-toggle-over");

            });
            $(".lee-layout-header-toggle", g.left).click(function () {
                g.setLeftCollapse(true);
            });
            $(".lee-layout-header-toggle", g.right).click(function () {
                g.setRightCollapse(true);
            });
            //set top
            g.middleTop = 0;
            if (g.top) {
                g.middleTop += g.top.height();
                g.middleTop += parseInt(g.top.css('borderTopWidth'));
                g.middleTop += parseInt(g.top.css('borderBottomWidth'));
                g.middleTop += p.space;
            }
            if (g.left) {
                g.left.css({ top: g.middleTop });
                g.leftCollapse.css({ top: g.middleTop });
            }
            if (g.center) g.center.css({ top: g.middleTop });
            if (g.right) {
                g.right.css({ top: g.middleTop });
                g.rightCollapse.css({ top: g.middleTop });
            }
            //set left
            if (g.left) g.left.css({ left: 0 });
            g._onResize();
            //g._onResize();
        },
        _setCollapse: function () {
            var g = this,
                p = this.options;
            g.leftCollapse.hover(function () {
                $(this).addClass("lee-layout-collapse-left-over");
            }, function () {
                $(this).removeClass("lee-layout-collapse-left-over");
            });
            g.leftCollapse.toggle.hover(function () {
                $(this).addClass("lee-layout-collapse-left-toggle-over");
            }, function () {
                $(this).removeClass("lee-layout-collapse-left-toggle-over");
            });
            g.rightCollapse.hover(function () {
                $(this).addClass("lee-layout-collapse-right-over");
            }, function () {
                $(this).removeClass("lee-layout-collapse-right-over");
            });
            g.rightCollapse.toggle.hover(function () {
                $(this).addClass("lee-layout-collapse-right-toggle-over");
            }, function () {
                $(this).removeClass("lee-layout-collapse-right-toggle-over");
            });
            g.leftCollapse.toggle.click(function () {
                g.setLeftCollapse(false);
            });
            g.rightCollapse.toggle.click(function () {
                g.setRightCollapse(false);
            });
            if (g.left && g.isLeftCollapse) {
                g.leftCollapse.show();
                g.leftDropHandle && g.leftDropHandle.hide();
                g.left.hide();
            }
            if (g.right && g.isRightCollapse) {
                g.rightCollapse.show();
                g.rightDropHandle && g.rightDropHandle.hide();
                g.right.hide();
            }
        },
        _addDropHandle: function () {
            var g = this,
                p = this.options;
            if (g.left && p.allowLeftResize) {
                g.leftDropHandle = $("<div class='lee-layout-drophandle-left'></div>");
                g.layout.append(g.leftDropHandle);
                g.leftDropHandle && g.leftDropHandle.show();
                g.leftDropHandle.mousedown(function (e) {
                    g._start('leftresize', e);
                });
            }
            if (g.right && p.allowRightResize) {
                g.rightDropHandle = $("<div class='lee-layout-drophandle-right'></div>");
                g.layout.append(g.rightDropHandle);
                g.rightDropHandle && g.rightDropHandle.show();
                g.rightDropHandle.mousedown(function (e) {
                    g._start('rightresize', e);
                });
            }
            if (g.top && p.allowTopResize) {
                g.topDropHandle = $("<div class='lee-layout-drophandle-top'></div>");
                g.layout.append(g.topDropHandle);
                g.topDropHandle.show();
                g.topDropHandle.mousedown(function (e) {
                    g._start('topresize', e);
                });
            }
            if (g.bottom && p.allowBottomResize) {
                g.bottomDropHandle = $("<div class='lee-layout-drophandle-bottom'></div>");
                g.layout.append(g.bottomDropHandle);
                g.bottomDropHandle.show();
                g.bottomDropHandle.mousedown(function (e) {
                    g._start('bottomresize', e);
                });
            }
            if (g.centerBottom && p.allowCenterBottomResize) {
                g.centerBottomDropHandle = $("<div class='lee-layout-drophandle-centerbottom'></div>");
                g.layout.append(g.centerBottomDropHandle);
                g.centerBottomDropHandle.show();
                g.centerBottomDropHandle.mousedown(function (e) {
                    g._start('centerbottomresize', e);
                });
            }
            g.draggingxline = $("<div class='lee-layout-dragging-xline'></div>");
            g.draggingyline = $("<div class='lee-layout-dragging-yline'></div>");
            g.mask = $("<div class='lee-dragging-mask'></div>");
            g.layout.append(g.draggingxline).append(g.draggingyline).append(g.mask);
        },
        _setDropHandlePosition: function () {
            var g = this,
                p = this.options;
            if (g.leftDropHandle) {
                g.leftDropHandle.css({ left: g.left.width() + parseInt(g.left.css('left')), height: g.middleHeight, top: g.middleTop });
            }
            if (g.rightDropHandle) {
                g.rightDropHandle.css({ left: parseInt(g.right.css('left')) - p.space, height: g.middleHeight, top: g.middleTop });
            }
            if (g.topDropHandle) {
                g.topDropHandle.css({ top: g.top.height() + parseInt(g.top.css('top')), width: g.top.width() });
            }
            if (g.bottomDropHandle) {
                g.bottomDropHandle.css({ top: parseInt(g.bottom.css('top')) - p.space, width: g.bottom.width() });
            }
            if (g.centerBottomDropHandle) {
                g.centerBottomDropHandle.css({
                    top: parseInt(g.centerBottom.css('top')) - p.space,
                    left: parseInt(g.center.css('left')),
                    width: g.center.width()
                });
            }
        },
        _onResize: function () {
            var g = this,
                p = this.options;
            var oldheight = g.layout.height();
            //set layout height 
            var h = 0;
            var windowHeight = $(window).height();
            var parentHeight = null;
            if (typeof (p.height) == "string" && p.height.indexOf('%') > 0) {
                var layoutparent = g.layout.parent();
                if (p.inWindow || layoutparent[0].tagName.toLowerCase() == "body") {
                    parentHeight = windowHeight;
                    parentHeight -= parseInt($('body').css('paddingTop'));
                    parentHeight -= parseInt($('body').css('paddingBottom'));
                } else {
                    parentHeight = layoutparent.height();
                }
                h = parentHeight * parseFloat(p.height) * 0.01;
                if (p.inWindow || layoutparent[0].tagName.toLowerCase() == "body")
                    h -= (g.layout.offset().top - parseInt($('body').css('paddingTop')));
            } else {
                h = parseInt(p.height);
            }
            h += p.heightDiff;
            g.layout.height(h);
            g.layoutHeight = g.layout.height();
            g.middleWidth = g.layout.width();
            g.middleHeight = g.layout.height();
            if (g.top) {
                g.middleHeight -= g.top.height();
                g.middleHeight -= parseInt(g.top.css('borderTopWidth'));
                g.middleHeight -= parseInt(g.top.css('borderBottomWidth'));
                g.middleHeight -= p.space;
            }
            if (g.bottom) {
                g.middleHeight -= g.bottom.height();
                g.middleHeight -= parseInt(g.bottom.css('borderTopWidth'));
                g.middleHeight -= parseInt(g.bottom.css('borderBottomWidth'));
                g.middleHeight -= p.space;
            }
            //specific
            g.middleHeight -= 2;

            if (g.hasBind('heightChanged') && g.layoutHeight != oldheight) {
                g.trigger('heightChanged', [{ layoutHeight: g.layoutHeight, diff: g.layoutHeight - oldheight, middleHeight: g.middleHeight }]);
            }

            if (g.center) {
                g.centerWidth = g.middleWidth;
                if (g.left) {
                    if (g.isLeftCollapse) { //收起的时候
                        g.centerWidth -= g.leftCollapse.width(); //折叠面板的宽度
                        g.centerWidth -= parseInt(g.leftCollapse.css('borderLeftWidth')); //边框
                        g.centerWidth -= parseInt(g.leftCollapse.css('borderRightWidth')); //边框
                        g.centerWidth -= parseInt(g.leftCollapse.css('left')); //左侧便宜
                        g.centerWidth -= p.space; //间距
                    } else {
                        g.centerWidth -= g.leftWidth; //
                        g.centerWidth -= parseInt(g.left.css('borderLeftWidth'));
                        g.centerWidth -= parseInt(g.left.css('borderRightWidth'));
                        g.centerWidth -= parseInt(g.left.css('left'));
                        g.centerWidth -= p.space;//box-sizing
                    }
                }
                if (g.right) {
                    if (g.isRightCollapse) {
                        g.centerWidth -= g.rightCollapse.width();
                        g.centerWidth -= parseInt(g.rightCollapse.css('borderLeftWidth'));
                        g.centerWidth -= parseInt(g.rightCollapse.css('borderRightWidth'));
                        g.centerWidth -= parseInt(g.rightCollapse.css('right'));
                        g.centerWidth -= p.space;
                    } else {
                        g.centerWidth -= g.rightWidth;
                        g.centerWidth -= parseInt(g.right.css('borderLeftWidth'));
                        g.centerWidth -= parseInt(g.right.css('borderRightWidth'));
                        g.centerWidth -= p.space;//box-sizing
                    }
                }
                g.centerWidth -= 2;//减去左右边框

                g.centerLeft = 0;
                if (g.left) {
                    if (g.isLeftCollapse) {
                        g.centerLeft += g.leftCollapse.width();
                        g.centerLeft += parseInt(g.leftCollapse.css('borderLeftWidth'));
                        g.centerLeft += parseInt(g.leftCollapse.css('borderRightWidth'));
                        g.centerLeft += parseInt(g.leftCollapse.css('left'));
                        g.centerLeft += p.space;
                    } else {
                        g.centerLeft += g.left.width();
                        g.centerLeft += parseInt(g.left.css('borderLeftWidth'));
                        g.centerLeft += parseInt(g.left.css('borderRightWidth'));
                        g.centerLeft += p.space;
                    }
                }
                g.center.css({ left: g.centerLeft });
                g.centerWidth >= 0 && g.center.width(g.centerWidth);
                g.middleHeight >= 0 && g.center.height(g.middleHeight);
                var contentHeight = g.middleHeight;
                if (g.center.header) contentHeight -= g.center.header.height();
                contentHeight >= 0 && g.center.content.height(contentHeight);

                g._updateCenterBottom(true);
            }
            if (g.left) {
                g.leftCollapse.height(g.middleHeight);
                g.left.height(g.middleHeight);
            }
            if (g.right) {
                g.rightCollapse.height(g.middleHeight);
                g.right.height(g.middleHeight);
                //set left
                g.rightLeft = 0;

                if (g.left) {
                    if (g.isLeftCollapse) {
                        g.rightLeft += g.leftCollapse.width();
                        g.rightLeft += parseInt(g.leftCollapse.css('borderLeftWidth'));
                        g.rightLeft += parseInt(g.leftCollapse.css('borderRightWidth'));
                        g.rightLeft += p.space;
                    } else {
                        g.rightLeft += g.left.width();
                        g.rightLeft += parseInt(g.left.css('borderLeftWidth'));
                        g.rightLeft += parseInt(g.left.css('borderRightWidth'));
                        g.rightLeft += parseInt(g.left.css('left'));
                        g.rightLeft += p.space;
                    }
                }
                if (g.center) {
                    g.rightLeft += g.center.width();
                    g.rightLeft += parseInt(g.center.css('borderLeftWidth'));
                    g.rightLeft += parseInt(g.center.css('borderRightWidth'));
                    g.rightLeft += p.space;
                }
                g.right.css({ left: g.rightLeft });
            }
            if (g.bottom) {
                g.bottomTop = g.layoutHeight - g.bottom.height() - 2;
                g.bottom.css({ top: g.bottomTop });
            }
            g._setDropHandlePosition();

        },
        //加了centerBottom以后，需要对centerBottom进行刷新处理一下
        _updateCenterBottom: function (isHeightResize) {
            var g = this,
                p = this.options;
            if (g.centerBottom) {
                if (isHeightResize) {
                    var centerBottomHeight = g.centerBottomHeight || p.centerBottomHeight;
                    g.centerBottom.css({ left: g.centerLeft });
                    g.centerWidth >= 0 && g.centerBottom.width(g.centerWidth);
                    var centerHeight = g.center.height(),
                        centerTop = parseInt(g.center.css("top"));
                    g.centerBottom.height(centerBottomHeight)
                    g.centerBottom.css({ top: centerTop + centerHeight - centerBottomHeight });
                    g.center.height(centerHeight - centerBottomHeight - 6);
                    //
                    var contentHeight = centerHeight - centerBottomHeight - 6;
                    if (g.center.header) contentHeight -= g.center.header.height();

                    contentHeight >= 0 && g.center.content.height(contentHeight);
                }
                var centerLeft = parseInt(g.center.css("left"));
                g.centerBottom.width(g.center.width()).css({ left: centerLeft });
            }
        },
        _start: function (dragtype, e) {
            var g = this,
                p = this.options;
            g.dragtype = dragtype;
            if (dragtype == 'leftresize' || dragtype == 'rightresize') {
                g.xresize = { startX: e.pageX };
                g.draggingyline.css({ left: e.pageX - g.layout.offset().left, height: g.middleHeight, top: g.middleTop }).show();
                $('body').css('cursor', 'col-resize');
                g.mask.height(g.layout.height()).removeClass("lee-layout-ymask").addClass("lee-layout-xmask").show();
            } else if (dragtype == 'topresize' || dragtype == 'bottomresize') {
                g.yresize = { startY: e.pageY };
                g.draggingxline.css({ top: e.pageY - g.layout.offset().top, width: g.layout.width() }).show();
                $('body').css('cursor', 'row-resize');
                g.mask.height(g.layout.height()).removeClass("lee-layout-xmask").addClass("lee-layout-ymask").show();
            } else if (dragtype == 'centerbottomresize') {
                g.yresize = { startY: e.pageY };
                g.draggingxline.css({ top: e.pageY - g.layout.offset().top, width: g.layout.width() }).show();
                $('body').css('cursor', 'row-resize');
                g.mask.height(g.layout.height()).removeClass("lee-layout-xmask").addClass("lee-layout-ymask").show();
            } else {
                return;
            }
            g.layout.lock.width(g.layout.width());
            g.layout.lock.height(g.layout.height());
            g.layout.lock.show();
            //if($.browser.msie || $.browser.safari) 
            $('body').bind('selectstart.layout', function () { return false; }); // 不能选择

            $(document).bind('mouseup.layout', function () {
                g._stop.apply(g, arguments);
            });
            $(document).bind('mousemove.layout', function () {
                g._drag.apply(g, arguments);
            });
        },
        _drag: function (e) {
            var g = this,
                p = this.options;
            if (g.xresize) {
                g.xresize.diff = e.pageX - g.xresize.startX;
                g.draggingyline.css({ left: e.pageX - g.layout.offset().left });
                $('body').css('cursor', 'col-resize');
            } else if (g.yresize) {
                g.yresize.diff = e.pageY - g.yresize.startY;
                g.draggingxline.css({ top: e.pageY - g.layout.offset().top });
                $('body').css('cursor', 'row-resize');
            }
        },
        _stop: function (e) {
            var g = this,
                p = this.options;
            var diff;
            if (g.xresize && g.xresize.diff != undefined) {
                diff = g.xresize.diff;
                if (g.dragtype == 'leftresize') {
                    if (p.minLeftWidth) {
                        if (g.leftWidth + g.xresize.diff < p.minLeftWidth)
                            return;
                    }
                    g.leftWidth += g.xresize.diff;
                    g.left.width(g.leftWidth);
                    if (g.center)
                        g.center.width(g.center.width() - g.xresize.diff).css({ left: parseInt(g.center.css('left')) + g.xresize.diff });
                    else if (g.right)
                        g.right.width(g.left.width() - g.xresize.diff).css({ left: parseInt(g.right.css('left')) + g.xresize.diff });
                } else if (g.dragtype == 'rightresize') {
                    if (p.minRightWidth) {
                        if (g.rightWidth - g.xresize.diff < p.minRightWidth)
                            return;
                    }
                    g.rightWidth -= g.xresize.diff;
                    g.right.width(g.rightWidth).css({ left: parseInt(g.right.css('left')) + g.xresize.diff });
                    if (g.center)
                        g.center.width(g.center.width() + g.xresize.diff);
                    else if (g.left)
                        g.left.width(g.left.width() + g.xresize.diff);
                }
                g._updateCenterBottom();
            } else if (g.yresize && g.yresize.diff != undefined) {
                diff = g.yresize.diff;
                if (g.dragtype == 'topresize') {
                    g.top.height(g.top.height() + g.yresize.diff);
                    g.middleTop += g.yresize.diff;
                    g.middleHeight -= g.yresize.diff;
                    if (g.left) {
                        g.left.css({ top: g.middleTop }).height(g.middleHeight);
                        g.leftCollapse.css({ top: g.middleTop }).height(g.middleHeight);
                    }
                    if (g.center) g.center.css({ top: g.middleTop }).height(g.middleHeight);
                    if (g.right) {
                        g.right.css({ top: g.middleTop }).height(g.middleHeight);
                        g.rightCollapse.css({ top: g.middleTop }).height(g.middleHeight);
                    }
                    g._updateCenterBottom(true);
                } else if (g.dragtype == 'bottomresize') {
                    g.bottom.height(g.bottom.height() - g.yresize.diff);
                    g.middleHeight += g.yresize.diff;
                    g.bottomTop += g.yresize.diff;
                    g.bottom.css({ top: g.bottomTop });
                    if (g.left) {
                        g.left.height(g.middleHeight);
                        g.leftCollapse.height(g.middleHeight);
                    }
                    if (g.center) g.center.height(g.middleHeight);
                    if (g.right) {
                        g.right.height(g.middleHeight);
                        g.rightCollapse.height(g.middleHeight);
                    }
                    g._updateCenterBottom(true);
                } else if (g.dragtype == 'centerbottomresize') {
                    g.centerBottomHeight = g.centerBottomHeight || p.centerBottomHeight;
                    g.centerBottomHeight -= g.yresize.diff;
                    var centerBottomTop = parseInt(g.centerBottom.css("top"));
                    g.centerBottom.css("top", centerBottomTop + g.yresize.diff);
                    g.centerBottom.height(g.centerBottom.height() - g.yresize.diff);
                    g.center.height(g.center.height() + g.yresize.diff);
                }
            }
            g.trigger('endResize', [{
                direction: g.dragtype ? g.dragtype.replace(/resize/, '') : '',
                diff: diff
            }, e]);
            g._setDropHandlePosition();
            g.draggingxline.hide();
            g.draggingyline.hide();
            g.mask.hide();
            g.xresize = g.yresize = g.dragtype = false;
            g.layout.lock.hide();
            //if($.browser.msie || $.browser.safari)
            $('body').unbind('selectstart.layout');
            $(document).unbind('mousemove.layout');
            $(document).unbind('mouseup.layout');
            $('body').css('cursor', '');

        }
    });

})(jQuery);

(function ($) {
    $.fn.leeTextBox = function () {
        return $.leeUI.run.call(this, "leeUITextBox", arguments);
    };

    $.fn.leeGetTextBoxManager = function () {
        return $.leeUI.run.call(this, "leeUIGetTextBoxManager", arguments);
    };

    $.leeUIDefaults.TextBox = {
        onChangeValue: null,
        onMouseOver: null,
        onMouseOut: null,
        onBlur: null,
        onFocus: null,
        width: "auto",
        disabled: false,
        initSelect: false,
        value: null, //初始化值 
        precision: 2, //保留小数位(仅currency时有效)
        nullText: null, //不能为空时的提示
        digits: false, //是否限定为数字输入框
        number: false, //是否限定为浮点数格式输入框
        currency: false, //是否显示为货币形式
        readonly: false //是否只读
    };

    $.leeUI.controls.TextBox = function (element, options) {
        $.leeUI.controls.TextBox.base.constructor.call(this, element, options);
    };

    $.leeUI.controls.TextBox.leeExtend($.leeUI.controls.Input, {
        __getType: function () {
            return 'TextBox'
        },
        __idPrev: function () {
            return 'TextBox';
        },
        _init: function () {
            $.leeUI.controls.TextBox.base._init.call(this);
            var g = this,
                p = this.options;
            if (!p.width) {
                p.width = $(g.element).width();
            }
            if ($(this.element).attr("readonly")) {
                p.readonly = true;
            } else if (p.readonly) {
                $(this.element).attr("readonly", true);
            }
        },
        _render: function () {
            var g = this,
                p = this.options;
            g.inputText = $(this.element);
            //外层
            g.wrapper = g.inputText.wrap('<div class="lee-text"></div>').parent();

            if (!g.inputText.hasClass("lee-text-field"))
                g.inputText.addClass("lee-text-field");
            this._setEvent();
            if (p.digits || p.number || p.currency) {
                g.inputText.addClass("lee-text-field-number");
            }
            if (p.placeholder) {
                g.inputText.attr("placeholder", p.placeholder);
            }
            g.set(p);
            g.formatValue();
        },
        destroy: function () {
            var g = this;
            if (g.wrapper) {
                g.wrapper.remove();
            }
            g.options = null;
            leeUI.remove(this);
        },
        _getValue: function () {
            var g = this,
                p = this.options;

            if (g.inputText.hasClass("lee-text-field-null")) {
                return "";
            }
            if (p.digits || p.number || p.currency) {
                return g.parseNumber();
            }
            return g.inputText.val();
        },
        _setNullText: function () {
            this.checkNotNull();
        },
        formatValue: function () {
            var g = this,
                p = this.options;
            var v = g.inputText.val() || "";
            if (v == "") return "";
            if (p.currency) {
                g.inputText.val(currencyFormatter(v, p.precision));
            } else if (p.number && p.precision && v) {
                var value = parseFloat(g.inputText.val()).toFixed(p.precision);
                g.inputText.val(value);
            }
        },
        checkNotNull: function () {
            var g = this,
                p = this.options;

            if (p.nullText && p.nullText != "null" && !p.disabled && !p.readonly) {
                if (!g.inputText.val()) {
                    g.inputText.addClass("lee-text-field-null").val(p.nullText);
                    return;
                }
            } else {
                g.inputText.removeClass("lee-text-field-null");
            }
        },
        _setEvent: function () {
            var g = this,
                p = this.options;

            function validate() {
                var value = g.inputText.val();
                if (!value || value == "-") return true;

                var r = (p.digits ? /^-?\d+$/ : /^(-?\d+)(\.)?(\d+)?$/).test(value);
                return r;
            }

            function keyCheck() {
                if (!validate()) {
                    g.inputText.val(g.parseNumber());
                }
            }
            if (p.digits || p.number || p.currency) {
                g.inputText.keyup(keyCheck).bind("paste", keyCheck);
            }
            g.inputText.bind('blur.textBox', function () {
                g.trigger('blur');
                g.checkNotNull();
                g.formatValue();
                g.wrapper.removeClass("lee-text-focus");
            }).bind('focus.textBox', function () {
                if (p.readonly) return;
                g.trigger('focus');
                if (p.nullText) {
                    if ($(this).hasClass("lee-text-field-null")) {
                        $(this).removeClass("lee-text-field-null").val("");
                    }
                }
                g.wrapper.addClass("lee-text-focus");

                if (p.digits || p.number || p.currency) {
                    $(this).val(g.parseNumber());
                    if (p.initSelect) {
                        setTimeout(function () {
                            g.inputText.select();
                        }, 150);
                    }
                }
            }).change(function () {
                g.trigger('change', [this.value]);
                g.trigger('changeValue', [this.value]);
            });
            g.wrapper.hover(function () {
                g.trigger('mouseOver');
                g.wrapper.addClass("lee-text-over");
            }, function () {
                g.trigger('mouseOut');
                g.wrapper.removeClass("lee-text-over");
            });
        },

        //将value转换为有效的数值
        //1,去除无效字符 2,小数点保留
        parseNumber: function (value) {
            var g = this,
                p = this.options;
            var isInt = p.digits ? true : false;
            value = value || g.inputText.val();
            if (value == null || value == "") return "";
            if (!(p.digits || p.number || p.currency)) return value;
            if (typeof (value) != "string") value = (value || "").toString();
            var sign = /^\-/.test(value);
            if (isInt) {
                if (value == "0") return value;
                value = value.replace(/\D+|^[0]+/g, '');
            } else {
                value = value.replace(/[^0-9.]/g, '');
                if (/^[0]+[1-9]+/.test(value)) {
                    value = value.replace(/^[0]+/, '');
                }
            }
            if (!isInt && p.precision) {
                value = parseFloat(value).toFixed(p.precision);
                if (value == "NaN") return "0";
            }
            if (sign) value = "-" + value;
            return value;
        },

        _setDisabled: function (value) {
            var g = this,
                p = this.options;
            if (value) {
                this.inputText.attr("readonly", "readonly");
                this.wrapper.addClass("lee-text-disabled");
            } else if (!p.readonly) {
                this.inputText.removeAttr("readonly");
                this.wrapper.removeClass('lee-text-disabled');
            }
        },
        _setRequired: function (value) {
            if (value) {
                this.wrapper.addClass('lee-text-required');
            } else {
                this.wrapper.removeClass('lee-text-required');
            }
        },
        _setWidth: function (value) {
            if (value > 20) {
                this.wrapper.css({ width: value });
                //this.inputText.css({ width: value - 4 });
            }
        },
        _setHeight: function (value) {
            if (value > 10) {
                //this.wrapper.height(value);
                //this.inputText.height(value - 2);
            }
        },
        _setValue: function (value) {

            var g = this,
                p = this.options;
            if (value != null) {


                this.inputText.val(value);
                if (p.digits || p.number || p.currency) {
                    this.inputText.val(g.parseNumber());
                }
            }

            this.checkNotNull();
        },
        _setLabel: function (value) {
            var g = this,
                p = this.options;
            if (!g.labelwrapper) {
                g.labelwrapper = g.wrapper.wrap('<div class="lee-labeltext"></div>').parent();
                var lable = $('<div class="lee-text-label" style="float:left;">' + value + ':&nbsp</div>');
                g.labelwrapper.prepend(lable);
                g.wrapper.css('float', 'left');
                if (!p.labelWidth) {
                    p.labelWidth = lable.width();
                } else {
                    g._setLabelWidth(p.labelWidth);
                }
                lable.height(g.wrapper.height());
                if (p.labelAlign) {
                    g._setLabelAlign(p.labelAlign);
                }
                g.labelwrapper.append('<br style="clear:both;" />');
                g.labelwrapper.width(p.labelWidth + p.width + 2);
            } else {
                g.labelwrapper.find(".lee-text-label").html(value + ':&nbsp');
            }
        },
        _setLabelWidth: function (value) {
            var g = this,
                p = this.options;
            if (!g.labelwrapper) return;
            g.labelwrapper.find(".lee-text-label").width(value);
        },
        _setLabelAlign: function (value) {
            var g = this,
                p = this.options;
            if (!g.labelwrapper) return;
            g.labelwrapper.find(".lee-text-label").css('text-align', value);
        },
        updateStyle: function () {
            var g = this,
                p = this.options;
            if (g.inputText.attr('readonly')) {
                g.wrapper.addClass("lee-text-readonly");
                p.disabled = true;
            } else {
                g.wrapper.removeClass("lee-text-readonly");
                p.disabled = false;
            }
            if (g.inputText.attr('disabled')) {
                g.wrapper.addClass("lee-text-disabled");
                p.disabled = true;
            } else {
                g.wrapper.removeClass("lee-text-disabled");
                p.disabled = false;
            }
            if (g.inputText.hasClass("lee-text-field-null") && g.inputText.val() != p.nullText) {
                g.inputText.removeClass("lee-text-field-null");
            }
            g.formatValue();
        },
        setValue: function (value) {
            this._setValue(value);
            //this.trigger('changeValue', [value]);
        }
    });

    function currencyFormatter(num, precision) {
        var cents, sign;
        if (!num) num = 0;
        num = num.toString().replace(/\$|\,/g, '').replace(/[a-zA-Z]+/g, '');
        if (num.indexOf('.') > -1) num = num.replace(/[0]+$/g, '');
        if (isNaN(num))
            num = 0;
        sign = (num == (num = Math.abs(num)));

        if (precision == null || precision == "0") {
            num = num.toString();
            cents = num.indexOf('.') != -1 ? num.substr(num.indexOf('.') + 1) : '';
            if (cents) {
                num = Math.floor(num * 1);
                num = num.toString();
            }
        } else {
            precision = parseInt(precision);
            var r = Math.pow(10, precision);
            num = Math.floor(num * r + 0.50000000001);
            cents = num % 100;
            num = Math.floor(num / r).toString();
            while (cents.toString().length < precision) {
                cents = "0" + cents;
            }
        }
        for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
            num = num.substring(0, num.length - (4 * i + 3)) + ',' +
                num.substring(num.length - (4 * i + 3));
        var numStr = "" + (((sign) ? '' : '-') + '' + num);
        if (cents) numStr += ('.' + cents);
        return numStr;
    }

})(jQuery);
/*
 * LeeUI-Button
 */

(function ($) {
    $.fn.leeButton = function (options) {
        return $.leeUI.run.call(this, "leeUIButton", arguments);
    };

    $.leeUIDefaults.Button = {
        width: "auto",
        text: 'Button',
        disabled: false,
        click: null,
        icon: "ss",
        position: "right"
    };

    $.leeUI.controls.Button = function (element, options) {
        $.leeUI.controls.Button.base.constructor.call(this, element, options);
    };
    $.leeUI.controls.Button.leeExtend($.leeUI.controls.Input, {
        __getType: function () {
            return 'Button';
        },
        __idPrev: function () {
            return 'Button';
        },
        _extendMethods: function () {
            return [];
        },
        _render: function () {
            var g = this,
                p = this.options;
            g.button = $(g.element);
            g.button.addClass("lee-btn lee-btn-primary");
            g.button.append('<span></span>');
            g.button.hover(function () {
                if (p.disabled) return;
                g.button.addClass("lee-btn-over");
            }, function () {
                if (p.disabled) return;
                g.button.removeClass("lee-btn-over");
            });
            p.click && g.button.click(function () {
                if (!p.disabled)
                    p.click();
            });
            g.set(p);
        },
        _setIcon: function (url) {
            var g = this,
                p = this.options;

            if (!url) {
                g.button.removeClass("lee-btn-hasicon");
                g.button.find('i').remove();
            } else {
                g.button.addClass("lee-btn-hasicon");
                if (p.position) {
                    g.button.addClass(p.position);
                    if (p.position == "left" || p.position == "top") {
                        g.button.prepend('<i class="lee-icon lee-search"></i>');
                    } else {
                        g.button.append('<i class="lee-icon lee-search"></i>');
                    }
                }

            }
        },
        _setEnabled: function (value) {
            if (value)
                this.button.removeClass("lee-btn-disabled");
        },
        _setDisabled: function (value) {
            if (value) {
                this.button.addClass("lee-btn-disabled");
                this.button.attr("disabled", "disabled");
                this.options.disabled = true;
            } else {
                this.button.removeClass("lee-btn-disabled");
                this.options.disabled = false;
                this.button.removeAttr("disabled");
            }
        },
        _setWidth: function (value) {
            this.button.width(value);
        },
        _setText: function (value) {
            $("span", this.button).html(value);
        },
        setValue: function (value) {
            this.set('text', value);
        },
        getValue: function () {
            return this.options.text;
        },
        setEnabled: function () {
            this.set('disabled', false);
        },
        setDisabled: function () {
            this.set('disabled', true);
        }
    });

})(jQuery);


/* ========================================================================
 * Bootstrap: button.js v3.3.7
 * http://getbootstrap.com/javascript/#buttons
 * ========================================================================
 * Copyright 2011-2016 Twitter, Inc.
 * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)
 * ======================================================================== */


+function ($) {
    'use strict';

    // BUTTON PUBLIC CLASS DEFINITION
    // ==============================

    var Button = function (element, options) {
        this.$element = $(element)
        this.options = $.extend({}, Button.DEFAULTS, options)
        this.isLoading = false
    }

    Button.VERSION = '3.3.7'

    Button.DEFAULTS = {
        loadingText: 'loading...'
    }

    Button.prototype.setState = function (state) {
        var d = 'disabled'
        var $el = this.$element
        var val = $el.is('input') ? 'val' : 'html'
        var data = $el.data()

        state += 'Text'

        if (data.resetText == null) $el.data('resetText', $el[val]())

        // push to event loop to allow forms to submit
        setTimeout($.proxy(function () {
            $el[val](data[state] == null ? this.options[state] : data[state])

            if (state == 'loadingText') {
                this.isLoading = true
                $el.addClass(d).attr(d, d).prop(d, true)
            } else if (this.isLoading) {
                this.isLoading = false
                $el.removeClass(d).removeAttr(d).prop(d, false)
            }
        }, this), 0)
    }

    Button.prototype.toggle = function () {
        var changed = true
        var $parent = this.$element.closest('[data-toggle="buttons"]')

        if ($parent.length) {
            var $input = this.$element.find('input')
            if ($input.prop('type') == 'radio') {
                if ($input.prop('checked')) changed = false
                $parent.find('.active').removeClass('active')
                this.$element.addClass('active')
            } else if ($input.prop('type') == 'checkbox') {
                if (($input.prop('checked')) !== this.$element.hasClass('active')) changed = false
                this.$element.toggleClass('active')
            }
            $input.prop('checked', this.$element.hasClass('active'))
            if (changed) $input.trigger('change')
        } else {
            this.$element.attr('aria-pressed', !this.$element.hasClass('active'))
            this.$element.toggleClass('active')
        }
    }


    // BUTTON PLUGIN DEFINITION
    // ========================

    function Plugin(option) {
        return this.each(function () {
            var $this = $(this)
            var data = $this.data('bs.button')
            var options = typeof option == 'object' && option

            if (!data) $this.data('bs.button', (data = new Button(this, options)))

            if (option == 'toggle') data.toggle()
            else if (option) data.setState(option)
        })
    }

    var old = $.fn.button

    $.fn.button = Plugin
    $.fn.button.Constructor = Button


    // BUTTON NO CONFLICT
    // ==================

    $.fn.button.noConflict = function () {
        $.fn.button = old
        return this
    }


    // BUTTON DATA-API
    // ===============

    $(document)
        .on('click.bs.button.data-api', '[data-toggle^="button"]', function (e) {

            var $btn = $(e.target).closest('.lee-btn');

            Plugin.call($btn, 'toggle')
            if (!($(e.target).is('input[type="radio"], input[type="checkbox"]'))) {
                // Prevent double click on radios, and the double selections (so cancellation) on checkboxes

                e.preventDefault()
                // The target component still receive the focus
                if ($btn.is('input,button')) $btn.trigger('focus')
                else $btn.find('input:visible,button:visible').first().trigger('focus')
            }
        })
        .on('focus.bs.button.data-api blur.bs.button.data-api', '[data-toggle^="button"]', function (e) {
            $(e.target).closest('.btn').toggleClass('focus', /^focus(in)?$/.test(e.type))
        })

}(jQuery);

/*
 * CheckBox Plugin
 */

(function ($) {
    $.fn.leeCheckBox = function (opitons) {
        return $.leeUI.run.call(this, "leeUICheckBox", arguments);
    }
    $.fn.leeGetCheckBoxManager = function () {
        return $.leeUI.run.call(this, "leeUIGetCheckBoxManager", arguments);
    };
    //初始化checkbox的默认配置
    $.leeUIDefaults.CheckBox = {
        disabled: false,
        readonly: false, //只读
        hasLabel: true,
        labelText: null,
        data: [],
        isMul: false
    };
    //扩展UIControl
    $.leeUI.controls.CheckBox = function (element, options) {
        //执行父构造函数
        $.leeUI.controls.CheckBox.base.constructor.call(this, element, options);
    };
    $.leeUI.controls.CheckBox.leeExtend($.leeUI.controls.Input, {
        __getType: function () {
            return "CheckBox";
        },
        __idPrev: function () {
            return "CheckBox";
        },
        _extendMethods: function () {

        },
        _render: function () {
            //渲染html
            var g = this;
            p = this.options;
            g.input = $(g.element);
            g.link = $("<span class='lee-checkbox'></span>");
            g.wrapper = g.input.addClass("lee-hidden").wrap("<div class='lee-checkbox-wrapper'></div>").parent();

            g.wrapper.prepend(g.link);


            if (p.labelText) {

                g.labelwrap = g.wrapper.wrap("<div class='lee-checkbox-label'></div>").parent();
                g.labelspan = $("<span class='lee-checkbox-label-span' style='font-size:12px;'>" + p.labelText + "</span>");
                g.labelwrap.append(g.labelspan);
                g.labelspan.bind("click", function () {
                    g.link.trigger("click");
                });
            }
            g.link.click(function () {
                if (g.input.attr('disabled') || g.input.attr('readonly')) { return false; }
                if (p.disabled || p.readonly) return false;
                if (g.trigger('beforeClick', [g.element]) == false) return false;
                if ($(this).hasClass("lee-checkbox-checked")) {
                    g._setValue(false);
                } else {
                    g._setValue(true);
                }

                g.input.trigger("change");
            });

            g.input.change(function () {
                if (this.checked) {
                    g.link.addClass('lee-checkbox-checked');
                }
                else {
                    g.link.removeClass('lee-checkbox-checked');
                }
                return true;
            });
            g.wrapper.hover(function () {
                if (!p.disabled)
                    $(this).addClass("lee-over");
            }, function () {
                $(this).removeClass("lee-over");
            });
            this.set(p);
            this.updateStyle();
        },
        _setCss: function () {
            this.wrapper.css(value);
        },
        _setValue: function (value) {
            var g = this,
                p = this.options;
            if (p.notbit) {
                value = (value == "1" ? true : false);
            }

            if (!value) {

                g.input[0].checked = false;
                g.link.removeClass('lee-checkbox-checked');
            } else {

                g.input[0].checked = true;

                g.link.addClass('lee-checkbox-checked');
            }
        },
        _setDisabled: function (value) {
            if (value) {
                this.input.attr('disabled', true);
                this.wrapper.addClass("lee-disabled");
            } else {
                this.input.attr('disabled', false);
                this.wrapper.removeClass("lee-disabled");
            }
        },
        _getValue: function () {
            if (this.options.notbit) {
                //alert(1);
                return (this.element.checked ? "1" : "0");
            }
            return this.element.checked;
        },
        updateStyle: function () {
            if (this.input.attr('disabled')) {
                this.wrapper.addClass("lee-disabled");
                this.options.disabled = true;
            }
            if (this.input[0].checked) {
                this.link.addClass('lee-checkbox-checked');
            } else {
                this.link.removeClass('lee-checkbox-checked');
            }
        }

    });
})(jQuery);

(function ($) {
    $.fn.leeCheckList = function () {
        return $.leeUI.run.call(this, "leeUICheckList", arguments);
    };



    $.leeUIDefaults.CheckList = {
        rowSize: 4,            //每行显示元素数   
        url: null,              //数据源URL(需返回JSON)
        data: null,             //json数据
        valueField: 'id',       //保存值字段名
        textField: 'text',      //显示值字段名
        root: null,
        onChangeValue: null,
        onSuccess: null,
        onError: null,
        splitchar: ',',
        absolute: false  //选择框是否在附加到body,并绝对定位
    };

    $.leeUI.controls.CheckList = function (element, options) {
        $.leeUI.controls.CheckList.base.constructor.call(this, element, options);
    };
    $.leeUI.controls.CheckList.leeExtend($.leeUI.controls.Input, {
        __getType: function () {
            return 'CheckList';
        },
        __idPrev: function () {
            return 'CheckList';
        },
        _extendMethods: function () {
            return {};
        },
        _render: function () {
            var g = this, p = this.options;
            g.data = p.data;
            g.valueField = null; //隐藏域(保存值) 

            if ($(this.element).is(":hidden") || $(this.element).is(":text")) {
                g.valueField = $(this.element);
                if ($(this.element).is(":text")) {
                    g.valueField.hide();
                }
            } else {
                g.valueField = $('<input type="hidden"/>');
                g.valueField[0].id = g.valueField[0].name = g.id + "_val";
            }
            if (g.valueField[0].name == null) g.valueField[0].name = g.valueField[0].id;

            g.valueField.attr("data-leeid", g.id);
            if ($(this.element).is(":hidden") || $(this.element).is(":text")) {
                g.checkList = $('<div></div>').insertBefore(this.element);
            } else {
                g.checkList = $(this.element);
            }


            g.checkList.html('<div class="lee-radiolist-inner"><table cellpadding="0" cellspacing="0" border="0" class="lee-radiolist-table"></table></div>').addClass("lee-radiolist").append(g.valueField);

            g.checkList.table = $("table:first", g.checkList);

            p.value = g.valueField.val() || p.value;

            g.set(p);

            g._addClickEven();

        },
        destroy: function () {
            if (this.checkList) this.checkList.remove();
            this.options = null;
            $.leeUI.remove(this);
        },
        clear: function () {
            this._changeValue("");
            this.trigger('clear');
        },
        _setDisabled: function (value) {
            //禁用样式
            if (value) {
                this.checkList.addClass('lee-checklist-disabled');
                $("input:checkbox", this.checkList).attr("disabled", true);
            } else {
                this.checkList.removeClass('lee-checklist-disabled');
                $("input:checkbox", this.checkList).removeAttr("disabled");
            }
        },
        _setValue: function (value) {
            var g = this, p = this.options;
            g.valueField.val(value);
            p.value = value;
            this._dataInit();
        },
        setValue: function (value) {
            this._setValue(value);
        },
        clearContent: function () {
            var g = this, p = this.options;
            $("table", g.checkList).html("");
        },
        _setData: function (data) {
            this.setData(data);
        },
        setData: function (data) {
            var g = this, p = this.options;
            if (!data || !data.length) return;
            g.data = data;
            g.refresh();
            g.updateStyle();
        },
        refresh: function () {
            var g = this, p = this.options, data = this.data;
            this.clearContent();
            if (!data) return;
            var out = [], rowSize = p.rowSize, appendRowStart = false, name = p.name || g.id;
            for (var i = 0; i < data.length; i++) {
                var val = data[i][p.valueField], txt = data[i][p.textField], id = g.id + "-" + i;
                var newRow = i % rowSize == 0;
                //0,5,10
                if (newRow) {
                    if (appendRowStart) out.push('</tr>');
                    out.push("<tr>");
                    appendRowStart = true;
                }
                out.push("<td><input type='checkbox' name='" + name + "' value='" + val + "' id='" + id + "'/><label for='" + id + "'>" + txt + "</label></td>");
            }
            if (appendRowStart) out.push('</tr>');
            g.checkList.table.append(out.join(''));
            $("input[type='checkbox']", g.checkList).leeCheckBox();
        },
        _getValue: function () {
            var g = this, p = this.options, name = p.name || g.id;

            var arr = $('input:checkbox[name="' + name + '"]:checked');
            var result = [];
            $.each(arr, function (i, ele) {
                var val = $(ele).val();
                result.push(val);
            });
            return result.join(p.splitchar);
        },
        getValue: function () {
            var val = this._getValue();
            if (!val) val = "";
            //获取值
            return val;
        },
        updateStyle: function () {
            var g = this, p = this.options;
            g._dataInit();
            //$(":checkbox", g.element).change(function () {
            //    var value = g.getValue();
            //    g.trigger('select', [{
            //        value: value
            //    }]);
            //});
        },
        _dataInit: function () {
            var g = this, p = this.options, name = p.name || g.id;
            var value = g.valueField.val();
            if (value == "") {
                $("input:checkbox[name='" + name + "']", g.checkList).each(function () {
                    //this.checked = false;

                    $(this).leeUI().setValue(false);
                });
                return;
            }
            var valuearr = value.split(p.splitchar);

            $("input:checkbox[name='" + name + "']", g.checkList).each(function () {
                var flag = false;
                if ($.inArray(this.value, valuearr) != -1) {
                    flag = true;
                }
                $(this).leeUI().setValue(flag);
            });

            //g._changeValue(value);

        },
        _setRequired: function (flag) {
        },
        _changeValue: function (newValue) {
            var g = this, p = this.options, name = p.name || g.id;
            $("input:checkbox[name='" + name + "']", g.checkList).each(function () {
                this.checked = this.value == newValue;
            });
            g.valueField.val(newValue);
            g.selectedValue = newValue;
        },
        _addClickEven: function () {
            var g = this, p = this.options;
            //选项点击
            g.checkList.click(function (e) {
                var value = g.getValue();
                if (value) g.valueField.val(value);
            });
        }
    });


})(jQuery);
(function ($) {

    $.fn.leeDropDown = function (options) {
        return $.leeUI.run.call(this, "leeUIDropDown", arguments);
    };

    $.fn.leeUIGetDropDownManager = function () {
        return $.leeUI.run.call(this, "leeUIGetDropDownManager", arguments);
    };
    //todo 多选样式调整
    //自动完成优化
    $.leeUIDefaults.DropDown = {
        resize: true, //是否调整大小
        isMultiSelect: false, //是否多选
        isShowCheckBox: false, //是否选择复选框
        isbit: false,
        columns: null, //表格状态
        width: null,
        selectBoxWidth: null, //宽度
        selectBoxHeight: 120, //高度
        selectBoxPosYDiff: -1, //下拉框位置y坐标调整
        onBeforeSelect: false, //选择前事件
        onAfterShowData: null,
        onSelected: null, //选择值事件 
        initValue: null,
        value: null,
        initText: null,
        valueField: 'id',
        textField: 'text',
        dataParmName: null,
        valueFieldID: null,
        ajaxComplete: null,
        ajaxBeforeSend: null,
        ajaxContentType: null,
        slide: false, //是否以动画的形式显示
        split: ";",
        data: null,
        dataGetter: null, //下拉框数据集获取函数
        tree: null, //下拉框以树的形式显示，tree的参数跟LigerTree的参数一致 
        treeLeafOnly: true, //是否只选择叶子
        condition: null, //列表条件搜索 参数同 ligerForm
        grid: null, //表格 参数同 ligerGrid
        onStartResize: null,
        onEndResize: null,
        hideOnLoseFocus: false, //鼠标mouseout 隐藏选项
        hideGridOnLoseFocus: false,
        url: null, //数据源URL(需返回JSON)
        urlParms: null, //url带参数
        selectBoxRender: null, //自定义selectbox的内容
        selectBoxRenderUpdate: null, //自定义selectbox(发生值改变)
        detailEnabled: true, //detailUrl是否有效
        detailUrl: null, //确定选项的时候，使用这个detailUrl加载到详细的数据
        detailPostIdField: null, //提交数据id字段名
        detailDataParmName: null, //返回数据data字段名
        detailParms: null, //附加参数
        detailDataGetter: null,
        delayLoad: false, //是否延时加载
        triggerToLoad: false, //是否在点击下拉按钮时加载 加载数据
        emptyText: null, //空行
        addRowButton: '新增', //新增按钮
        addRowButtonClick: null, //新增事件
        triggerIcon: null, //
        onSuccess: null,
        onBeforeSetData: null,
        onError: null,
        onBeforeOpen: null, //打开下拉框前事件，可以通过return false来阻止继续操作，利用这个参数可以用来调用其他函数，比如打开一个新窗口来选择值
        onButtonClick: null, //右侧图标按钮事件，可以通过return false来阻止继续操作，利用这个参数可以用来调用其他函数，比如打开一个新窗口来选择值
        onTextBoxKeyDown: null,
        onTextBoxKeyEnter: null,
        render: null, //文本框显示html函数
        absolute: true, //选择框是否在附加到body,并绝对定位
        cancelable: true, //可取消选择
        css: null, //附加css
        parms: null, //ajax提交表单 
        renderItem: null, //选项自定义函数
        autocomplete: false, //自动完成 
        autocompleteAllowEmpty: true, //是否允许空值搜索
        isTextBoxMode: false, //是否文本框的形式
        highLight: false, //自动完成是否匹配字符高亮显示
        readonly: false, //是否只读
        ajaxType: 'post',
        alwayShowInTop: false, //下拉框是否一直显示在上方
        alwayShowInDown: false, //下拉框是否一直显示在上方
        valueFieldCssClass: null,
        isRowReadOnly: null, //选项是否只读的判定函数
        rowClsRender: null, //选项行 class name 自定义函数
        keySupport: true, //按键支持： 上、下、回车 支
        initIsTriggerEvent: false, //初始化时是否触发选择事件
        conditionSearchClick: null, //下拉框表格搜索按钮自定义函数
        onChangeValue: null,
        delayLoadGrid: true, //是否在按下显示下拉框的时候才 加载 grid
        setTextBySource: true //设置文本框值时是否从数据源中加载
    };

    $.leeUIDefaults.DropDownString = {
        Search: "搜索"
    };

    $.leeUI.controls.DropDown = function (element, options) {
        $.leeUI.controls.DropDown.base.constructor.call(this, element, options);
    };
    $.leeUI.controls.DropDown.leeExtend($.leeUI.controls.Input, {
        __getType: function () {
            return 'DropDown';
        },
        _extendMethods: function () {
            return null;
        },
        _init: function () {
            $.leeUI.controls.DropDown.base._init.call(this);
            var p = this.options;
            if (p.columns) {
                p.isShowCheckBox = true;
            }
            if (p.isMultiSelect) { //多选
                p.isShowCheckBox = true;
            }
            if (p.triggerToLoad) { //延迟加载
                p.delayLoad = true;
            }
        },
        _render: function () {
            var g = this,
                p = this.options;
            g.data = p.data;
            g.inputText = null;
            g.select = null;
            g.textFieldID = "";
            g.valueFieldID = "";
            g.valueField = null; //隐藏域(保存值) 
            if ($(this.element).attr("type") == "hidden") {
                g.valueField = $(this.element);
                g.textFieldID = p.textFieldID || (this.element.id + "$text");
                g.inputText = $('<input type="text" />');
                g.inputText.attr("id", g.textFieldID).insertAfter($(this.element));
                //隐藏控件这里要添加验证attribute			 
            } else if (this.element.tagName.toLowerCase() == "input") {


                if ($(this.element).attr("readonly"))
                    p.disabled = true;
                g.inputText = $(this.element);
                g.textFieldID = this.element.id; //控件ID
                this.element.readOnly = true;
            } else if (this.element.tagName.toLowerCase() == "select") {
                $(this.element).hide();
                g.select = $(this.element);
                p.isMultiSelect = false;
                p.isShowCheckBox = false;
                p.cancelable = false;
                g.textFieldID = p.textFieldID || (this.element.id + "$text");
                g.inputText = $('<input type="text" />');
                g.inputText.attr("id", g.textFieldID).insertAfter($(this.element));
                if (!p.value && this.element.value) {
                    p.value = this.element.value;
                }

            }
            if (g.inputText[0].name == undefined) g.inputText[0].name = g.textFieldID;

            g.inputText.attr("data-dropdownid", g.id);
            if (g.valueField == null) {
                if (p.valueFieldID) {
                    g.valueField = $("#" + p.valueFieldID + ":input,[name=" + p.valueFieldID + "]:input").filter("input:hidden");
                    if (g.valueField.length == 0) g.valueField = $('<input type="hidden"/>');
                    g.valueField[0].id = g.valueField[0].name = p.valueFieldID;
                } else {
                    g.valueField = $('<input type="hidden"/>');
                    g.valueField[0].id = g.valueField[0].name = g.textFieldID + "_val";
                }
            }
            if (g.valueField[0].name == undefined) g.valueField[0].name = g.valueField[0].id;
            //update by superzoc 增加初始值
            if (p.initValue != null) g.valueField[0].value = p.initValue;
            g.valueField.attr("data-dropdownid", g.id);

            //开关
            g.link = $('<div class="lee-right"><i class="lee-icon lee-angle-down dropdown"></i></div>');
            if (p.triggerIcon) g.link.find("div:first").addClass(p.triggerIcon);
            //下拉框
            g.selectBox = $('<div class="lee-box-select" style="display:none"><div class="lee-box-select-inner"><table cellpadding="0" cellspacing="0" border="0" class="lee-box-select-table"></table></div></div>');
            g.selectBox.table = $("table:first", g.selectBox); //下拉框的table容器
            g.selectBoxInner = g.selectBox.find(".lee-box-select-inner:first");
            //外层
            g.wrapper = g.inputText.wrap('<div class="lee-text lee-text-drop-down"></div>').parent();

            g.wrapper.append(g.link);
            //添加个包裹，
            g.textwrapper = g.wrapper.wrap('<div class="lee-text-wrapper"></div>').parent();
            // 绝对定位
            if (p.absolute)
                g.selectBox.appendTo('body').addClass("lee-box-select-absolute");
            else
                g.textwrapper.append(g.selectBox);

            g.textwrapper.append(g.valueField);
            g.inputText.addClass("lee-text-field");

            //复选框样式
            if (p.isShowCheckBox && !g.select) {
                $("table", g.selectBox).addClass("lee-table-checkbox");
            } else {
                p.isShowCheckBox = false;
                $("table", g.selectBox).addClass("lee-table-nocheckbox");
            }
            //开关 事件
            g.link.hover(function () {
                if (p.disabled || p.readonly) return;
                $(this).addClass("lee-right-hover");
                //this.className = "lee-right-hover";
            }, function () {
                if (p.disabled || p.readonly) return;
                $(this).removeClass("lee-right-hover");
                //this.className = "lee-right";
            }).mousedown(function () {
                if (p.disabled || p.readonly) return;
                $(this).addClass("lee-right-pressed");
            }).mouseup(function () {
                if (p.disabled || p.readonly) return;
                $(this).addClass("lee-right-hover");
            }).click(function () {
                if (p.disabled || p.readonly) return;
                if (g.trigger('buttonClick') == false) return false;
                if (g.trigger('beforeOpen') == false) return false;
                openSelectBox();
            });

            g.inputText.click(function () {
                if (p.disabled || p.readonly) return;
                if (g.trigger('beforeOpen') == false) return false;
                openSelectBox();
            }).blur(function () {
                if (p.disabled) return;
                g.wrapper.removeClass("lee-text-focus");
            }).focus(function () {
                if (p.disabled || p.readonly) return;
                g.wrapper.addClass("lee-text-focus");
            }).change(function () {

                g.trigger('change', [this.value]);
                //alert(2);
                //值改变事件
                //alert(2);
                //g.trigger('changeValue', [this.value]);
            });

            g.wrapper.hover(function () {
                if (p.disabled || p.readonly) return;
                g.wrapper.addClass("lee-text-over");
            }, function () {
                if (p.disabled || p.readonly) return;
                g.wrapper.removeClass("lee-text-over");
            });

            function openSelectBox() {
                if (!p.autocomplete) {
                    if (p.triggerToLoad && !g.triggerLoaded) {
                        g.triggerLoaded = true;
                        g._setUrl(p.url, function () {
                            g._toggleSelectBox(g.selectBox.is(":visible"));
                        });
                    } else {
                        g._toggleSelectBox(g.selectBox.is(":visible"));
                    }
                } else {
                    g._toggleSelectBox(g.selectBox.is(":visible"));
                    g.updateSelectBoxPosition();
                }
            }
            g.resizing = false;
            //			g.selectBox.hover(null, function(e) {
            //				if(p.hideOnLoseFocus && g.selectBox.is(":visible") && !g.boxToggling && !g.resizing) {
            //					g._toggleSelectBox(true);
            //				}
            //			});  这里没有意义啊 鼠标意识焦点 隐藏框？
            //下拉框内容初始化
            g.bulidContent();

            g.set(p, null, "init");

            //下拉框宽度、高度初始化   
            if (p.selectBoxWidth) {
                g.selectBox.width(p.selectBoxWidth);
            } else {
                g.selectBox.css('width', g.wrapper.css('width'));
            }
            g.updateSelectBoxPosition();
            $(document).bind("click.combobox", function (e) {
                //修改点击空白处隐藏下拉框功能
                if ($((e.target || e.srcElement)).closest(".lee-box-select, .lee-text-drop-down").length == 0) {
                    if (g.selectBox.is(":visible")) {
                        g._toggleSelectBox(true);
                    }
                }
            });
        },
        destroy: function () {
            //销毁
            if (this.wrapper) this.wrapper.remove();
            if (this.selectBox) this.selectBox.remove();
            this.options = null;
            $.leeUI.remove(this);
        },
        clear: function () {
            if (this.getValue() == "") {
                return;//如果是空的话不触发值值改变事件
            }
            this._changeValue("", "", true);
            $("a.lee-checkbox-checked", this.selectBox).removeClass("lee-checkbox-checked");
            $("td.lee-selected", this.selectBox).removeClass("lee-selected");
            $(":checkbox", this.selectBox).each(function () {
                this.checked = false;
            });
            //tree 也要取消选中
            this.trigger('clear');
        },
        _setSelectBoxHeight: function (height) {
            if (!height) return;
            var g = this,
                p = this.options;
            //todo tree or grid
            g.selectBoxInner.height(p.selectBoxHeight);

        },
        _setCss: function (css) {
            if (css) {
                this.wrapper.addClass(css);
            }
        },
        //取消选择 
        _setCancelable: function (value) {
            var g = this,
                p = this.options;
            if (!value && g.unselect) {
                g.unselect.remove();
                g.unselect = null;
            }
            if (!value && !g.unselect) return;
            g.unselect = $('<div class="lee-clear"><i class="lee-icon lee-icon-close lee-clear-achor"></i></div>').hide();
            g.wrapper.hover(function () {
                if (!p.disabled)
                    g.unselect.show();
            }, function () {
                g.unselect.hide();
            })
            if (!p.disabled && !p.readonly && p.cancelable) {
                g.wrapper.append(g.unselect);
            }
            g.unselect.click(function () {
                g.clear();
            });
        },
        _setDisabled: function (value) {
            //禁用样式
            if (value) {
                this.options.disabled = true;
                this.wrapper.addClass('lee-text-disabled');
                this.inputText.attr("readonly", "readonly");
            } else {
                this.options.disabled = false;
                this.wrapper.removeClass('lee-text-disabled');
                //this.inputText.removeAttr("readonly");
            }
        },
        _setRequired: function (value) {
            if (value) {
                this.wrapper.addClass('lee-text-required');
            } else {
                this.wrapper.removeClass('lee-text-required');
            }
        },
        _setReadonly: function (readonly) {
            if (readonly) {
                this.wrapper.addClass("lee-text-readonly");
            } else {
                this.wrapper.removeClass("lee-text-readonly");
            }
        },
        _setWidth: function (value) {
            var g = this,
                p = this.options;
            if (value > 20) {
                g.wrapper.css({
                    width: value
                });
                //g.inputText.css({ width: value - 20 });
                if (!p.selectBoxWidth) {
                    g.selectBox.css({
                        width: value
                    });
                }
            }
        },
        _setHeight: function (value) {
            var g = this;
            if (value > 10) {
                //g.wrapper.height(value);
                //g.inputText.height(value - 2);
            }
        },
        _setResize: function (resize) {
            var g = this,
                p = this.options;
            if (p.columns) {
                return;
            }
            //调整大小支持
            if (resize && $.fn.leeResizable) {
                var handles = p.selectBoxHeight ? 'e' : 'se,s,e';
                g.selectBox.leeResizable({
                    handles: handles,
                    onStartResize: function () {
                        g.resizing = true;
                        g.trigger('startResize');
                    },
                    onEndResize: function () {
                        g.resizing = false;
                        if (g.trigger('endResize') == false)
                            return false;
                    },
                    onStopResize: function (current, e) {
                        if (g.grid) {
                            if (current.newWidth) {
                                g.selectBox.width(current.newWidth);
                            }
                            if (current.newHeight) {
                                g.set({
                                    selectBoxHeight: current.newHeight
                                });
                            }
                            g.grid.refreshSize();
                            g.trigger('endResize');
                            return false;
                        }
                        return true;
                    }
                });
                g.selectBox.append("<div class='lee-btn-nw-drop'></div>");
            }
        },
        //查找Text,适用多选和单选
        findTextByValue: function (value) {
            var g = this,
                p = this.options;
            if (value == null) return "";
            var texts = "";
            var contain = function (checkvalue) {
                var targetdata = value.toString().split(p.split);
                for (var i = 0; i < targetdata.length; i++) {
                    if (targetdata[i] == checkvalue && targetdata[i] != "") return true;
                }
                return false;
            };
            //当combobox下拉一个grid时, 不能直接取data. 必须取grid的data. 
            //原写法$(g.data) 仅适用于无grid时的典型情形
            var d;
            if (g.options.grid && g.options.grid.data)
                d = g.options.grid.data.Rows;
            else
                d = g.data;
            $(d).each(function (i, item) {
                var val = item[p.valueField];
                var txt = item[p.textField];
                if (contain(val)) {
                    texts += txt + p.split;
                }
            });
            if (texts.length > 0) texts = texts.substr(0, texts.length - 1);
            return texts;
        },
        //查找Value,适用多选和单选
        findValueByText: function (text) {
            var g = this,
                p = this.options;
            if (!text && text == "") return "";
            var contain = function (checkvalue) {
                var targetdata = text.toString().split(p.split);
                for (var i = 0; i < targetdata.length; i++) {
                    if (targetdata[i] == checkvalue) return true;
                }
                return false;
            };
            var values = "";
            $(g.data).each(function (i, item) {
                var val = item[p.valueField];
                var txt = item[p.textField];
                if (contain(txt)) {
                    values += val + p.split;
                }
            });
            if (values.length > 0) values = values.substr(0, values.length - 1);
            return values;
        },
        insertItem: function (data, index) {
            var g = this,
                p = this.options;
            g.data = g.data || [];
            g.data.splice(index, 0, data);
            g.setData(g.data);
        },
        addItem: function (data) {
            //外部接口 添加数据
            var g = this,
                p = this.options;
            g.insertItem(data, (g.data || []).length);
        },
        _setIsTextBoxMode: function (value) {
            //是否文本模式
            var g = this,
                p = this.options;
            if (value) {
                g.inputText.removeAttr("readonly");
            }
        },
        _setValue: function (value, text) {


            var g = this,
                p = this.options;

            if (p.isbit) {
                if (value) value = "1";
                else value = "0";
            }
            var isInit = false,
                isTriggerEvent = true;
            if (text == "init") {
                text = null;
                isInit = true;
                isTriggerEvent = p.initIsTriggerEvent ? true : false;
            }
            if (p.isTextBoxMode) {
                text = value;
            } else {
                text = text || g.findTextByValue(value);
            }
            if (p.tree) {
                //刷新树的选中状态
                setTimeout(function () {
                    if (p.setTextBySource) {
                        //刷新树的选中状态并更新文本框
                        g.selectValueByTree(value);
                    } else {
                        g.treeSelectInit(value);
                    }
                }, 100);
            } else if (!p.isMultiSelect) {
                g._changeValue(value, text, false);
                $("tr[value='" + value + "'] td", g.selectBox).addClass("lee-selected");
                $("tr[value!='" + value + "'] td", g.selectBox).removeClass("lee-selected");
            } else {
                g._changeValue(value, text, isTriggerEvent);
                if (value != null) {

                    var targetdata = value.toString().split(p.split);
                    $("table.lee-table-checkbox :checkbox", g.selectBox).each(function () {
                        this.checked = false;

                    });
                    for (var i = 0; i < targetdata.length; i++) {
                        $("table.lee-table-checkbox tr[value=" + targetdata[i] + "] :checkbox", g.selectBox).each(function () {
                            this.checked = true;
                        });
                    }
                }
            }
            if (p.selectBoxRenderUpdate) {
                p.selectBoxRenderUpdate.call(g, {
                    selectBox: g.selectBox,
                    value: value,
                    text: text
                });
            }
        },
        selectValue: function (value) {
            this._setValue(value);

        },
        bulidContent: function () {
            var g = this,
                p = this.options;
            this.clearContent();
            if (g.select) {
                g.setSelect();
            } else if (p.tree) {
                g.setTree(p.tree);
            }
        },
        reload: function () {
            var g = this,
                p = this.options;
            if (p.url) {
                g.set('url', p.url);
            } else if (g.grid) {
                g.grid.reload();
            }
        },
        _setUrl: function (url, callback) {
            if (!url) return;
            var g = this,
                p = this.options;
            if (p.readonly) //只读状态不加载数据
            {
                return;
            }
            if (p.delayLoad && !g.isAccessDelay && !g.triggerLoaded) {
                g.isAccessDelay = true; //已经有一次延时加载了
                return;
            }
            url = $.isFunction(url) ? url.call(g) : url;
            var urlParms = $.isFunction(p.urlParms) ? p.urlParms.call(g) : p.urlParms;
            if (urlParms) {
                for (name in urlParms) {
                    url += url.indexOf('?') == -1 ? "?" : "&";
                    url += name + "=" + urlParms[name];
                }
            }
            var parms = $.isFunction(p.parms) ? p.parms.call(g) : p.parms;
            if (p.ajaxContentType == "application/json" && typeof (parms) != "string") {
                parms = liger.toJSON(parms);
            }
            var ajaxOp = {
                type: p.ajaxType,
                url: url,
                data: parms,
                cache: false,
                dataType: 'json',
                beforeSend: p.ajaxBeforeSend,
                complete: p.ajaxComplete,
                success: function (result) {
                    var data = $.isFunction(p.dataGetter) ? data = p.dataGetter.call(g, result) : result;
                    data = p.dataParmName && data ? data[p.dataParmName] : data;
                    if (g.trigger('beforeSetData', [data]) == false) {
                        return;
                    }
                    g.setData(data);
                    g.trigger('success', [data]);
                    if ($.isFunction(callback)) callback(data);
                },
                error: function (XMLHttpRequest, textStatus) {
                    g.trigger('error', [XMLHttpRequest, textStatus]);
                }
            };
            if (p.ajaxContentType) {
                ajaxOp.contentType = p.ajaxContentType;
            }
            $.ajax(ajaxOp);
        },
        setUrl: function (url, callback) {
            return this._setUrl(url, callback);
        },
        setParm: function (name, value) {
            if (!name) return;
            var g = this;
            var parms = g.get('parms');
            if (!parms) parms = {};
            parms[name] = value;
            g.set('parms', parms);
        },
        clearContent: function () {
            var g = this,
                p = this.options;
            if (!g) return;
            $("table", g.selectBox).html("");
            if (!g) return;
            //清除下拉框内容的时候重设高度
            g._setSelectBoxHeight(p.selectBoxHeight);
            //modify end
            //g.inputText.val("");
            //g.valueField.val(""); 
        },
        setSelect: function () {
            var g = this,
                p = this.options;
            this.clearContent();
            g.data = [];
            $('option', g.select).each(function (i) {
                var val = $(this).val();
                var txt = $(this).html();
                g.data.push({
                    text: txt,
                    id: val
                });
                var tr = $("<tr><td index='" + i + "' value='" + val + "' text='" + txt + "'>" + txt + "</td>");
                $("table.lee-table-nocheckbox", g.selectBox).append(tr);

                $("td", tr).hover(function () {
                    $(this).addClass("lee-over").siblings("td").removeClass("lee-over");
                }, function () {
                    $(this).removeClass("lee-over");
                });
            });
            $('td:eq(' + g.select[0].selectedIndex + ')', g.selectBox).each(function () {
                if ($(this).hasClass("lee-selected")) {
                    g.selectBox.hide();
                    return;
                }
                $(".lee-selected", g.selectBox).removeClass("lee-selected");
                $(this).addClass("lee-selected");
                if (g.select[0].selectedIndex != $(this).attr('index') && g.select[0].onchange) {
                    g.select[0].selectedIndex = $(this).attr('index');
                    g.select[0].onchange();
                }
                var newIndex = parseInt($(this).attr('index'));
                g.select[0].selectedIndex = newIndex;
                g.select.trigger("change");
                g.selectBox.hide();
                var value = $(this).attr("value");
                var text = $(this).html();
                if (p.render) {
                    g.inputText.val(p.render(value, text));
                } else {
                    g.inputText.val(text);
                }
            });
            g._addClickEven();
        },
        _setData: function (data) {
            this.setData(data);
        },
        getRowIndex: function (value) {
            var g = this,
                p = this.options;
            if (!value) return -1;
            if (!g.data || !g.data.length) return -1;
            for (var i = 0; i < g.data.length; i++) {
                if (g.data[i] == null) continue;
                var val = g.data[i][p.valueField];
                if (val == value) return i;
            }
            return -1;
        },
        //获取行数据
        getRow: function (value) {
            var g = this,
                p = this.options;
            if (!value) return null;
            if (!g.data || !g.data.length) return null;
            for (var i = 0; i < g.data.length; i++) {
                if (g.data[i] == null) continue;
                var val = g.data[i][p.valueField];
                if (val == value) return g.data[i];
            }
            return null;
        },
        setData: function (data, autocomplete) {
            var g = this,
                p = this.options;
            if (g.select) return;
            if (p.selectBoxRender) {
                p.selectBoxRender.call(g, {
                    selectBox: g.selectBox,
                    data: data
                });
                return;
            }
            if (!data || !data.length) data = [];
            if (g.data != data) g.data = data;
            g.data = $.isFunction(g.data) ? g.data() : g.data;
            this.clearContent();
            if (p.columns) {
                g.selectBox.table.headrow = $("<tr class='lee-table-headerow'><td width='18px'></td></tr>");
                g.selectBox.table.append(g.selectBox.table.headrow);
                g.selectBox.table.addClass("lee-box-select-grid");
                for (var j = 0; j < p.columns.length; j++) {
                    var headrow = $("<td columnindex='" + j + "' columnname='" + p.columns[j].name + "'>" + p.columns[j].header + "</td>");
                    if (p.columns[j].width) {
                        headrow.width(p.columns[j].width);
                    }
                    g.selectBox.table.headrow.append(headrow);

                }
            }
            var out = [];
            if (p.emptyText) {
                g.emptyRow = {};
                g.emptyRow[p.textField] = p.emptyText;
                g.emptyRow[p.valueField] = p.emptyValue != undefined ? p.emptyValue : "";
                data.splice(0, 0, g.emptyRow);
            }
            for (var i = 0; i < data.length; i++) {
                var val = data[i][p.valueField];
                var txt = data[i][p.textField];
                var isRowReadOnly = $.isFunction(p.isRowReadOnly) ? p.isRowReadOnly(data[i]) : false;
                if (!p.columns) {
                    out.push("<tr value='" + val + "' text='" + txt + "'");

                    var cls = [];
                    if (isRowReadOnly) cls.push(" rowreadonly ");
                    if ($.isFunction(p.rowClsRender)) cls.push(p.rowClsRender(data[i]));
                    if (cls.length) {
                        out.push(" class='");
                        out.push(cls.join(''));
                        out.push("'");
                    }
                    out.push(">");
                    if (p.isShowCheckBox) {
                        out.push("<td style='width:18px;'  index='" + i + "' value='" + val + "' text='" + txt + "' ><input type='checkbox' /></td>");
                    }
                    var itemHtml = txt;
                    if (p.renderItem) {
                        itemHtml = p.renderItem.call(g, {
                            data: data[i],
                            value: val,
                            text: txt,
                            key: g.inputText.val()
                        });
                    } else if (p.autocomplete && p.highLight) {
                        itemHtml = g._highLight(txt, g.inputText.val());
                    } else {
                        itemHtml = "<span>" + itemHtml + "</span>";
                    }
                    out.push("<td index='" + i + "' value='" + val + "' text='" + txt + "' align='left'>" + itemHtml + "</td></tr>");
                } else {
                    out.push("<tr value='" + val + "'");
                    if (isRowReadOnly) out.push(" class='rowreadonly'");
                    out.push(">");
                    out.push("<td style='width:18px;'  index='" + i + "' value='" + val + "' text='" + txt + "' ><input type='checkbox' /></td>");
                    for (var j = 0; j < p.columns.length; j++) {
                        var columnname = p.columns[j].name;
                        out.push("<td>" + data[i][columnname] + "</td>");
                    }
                    out.push('</tr>');
                }
            }
            if (!p.columns) {
                if (p.isShowCheckBox) {
                    $("table.lee-table-checkbox", g.selectBox).append(out.join(''));
                } else {
                    $("table.lee-table-nocheckbox", g.selectBox).append(out.join(''));
                }
            } else {
                g.selectBox.table.append(out.join(''));
            }
            if (p.addRowButton && p.addRowButtonClick && !g.addRowButton) {
                g.addRowButton = $('<div class="lee-box-select-add"><a href="javascript:void(0)" class="link"><div class="icon"></div></a></div>');
                g.addRowButton.find(".link").append(p.addRowButton).click(p.addRowButtonClick);
                g.selectBoxInner.after(g.addRowButton);
            }
            g.set('selectBoxHeight', p.selectBoxHeight);
            //自定义复选框支持
            if (p.isShowCheckBox && $.fn.leeCheckBox) {
                $("table input:checkbox", g.selectBox).leeCheckBox();
            }

            $(".lee-table-checkbox input:checkbox", g.selectBox).change(function () {
                if (this.checked && g.hasBind('beforeSelect')) {
                    var parentTD = null;
                    if ($(this).parent().get(0).tagName.toLowerCase() == "div") {
                        parentTD = $(this).parent().parent();
                    } else {
                        parentTD = $(this).parent();
                    }
                    if (parentTD != null && g.trigger('beforeSelect', [parentTD.attr("value"), parentTD.attr("text")]) == false) {
                        g.selectBox.slideToggle("fast");
                        return false;
                    }
                }
                if (!p.isMultiSelect) {
                    if (this.checked) {
                        $("input:checked", g.selectBox).not(this).each(function () {
                            this.checked = false;
                            $(".lee-checkbox-checked", $(this).parent()).removeClass("lee-checkbox-checked");
                        });
                        g.selectBox.slideToggle("fast");
                    }
                }
                g._checkboxUpdateValue();
            });
            $("table.lee-table-nocheckbox td", g.selectBox).hover(function () {
                if (!$(this).parent().hasClass("rowreadonly")) {
                    $(this).addClass("lee-over");
                }
            }, function () {
                $(this).removeClass("lee-over");
            });
            g._addClickEven();
            //选择项初始化
            if (!autocomplete) {
                g.updateStyle();
            }

            g.trigger('afterShowData', [data]);
        },
        //树
        setTree: function (tree) {
            var g = this,
                p = this.options;
            this.clearContent();
            g.selectBox.table.remove();
            if (tree.checkbox != false) {
                tree.onCheck = function () {
                    var nodes = g.treeManager.getChecked();
                    var value = [];
                    var text = [];
                    $(nodes).each(function (i, node) {
                        if (p.treeLeafOnly && node.data.children) return;
                        value.push(node.data[p.valueField]);
                        text.push(node.data[p.textField]);
                    });
                    g._changeValue(value.join(p.split), text.join(p.split), true);
                };
            } else {
                tree.onSelect = function (node) {
                    if (g.trigger('BeforeSelect', [node]) == false) return;
                    if (p.treeLeafOnly && node.data.children) return;
                    var value = node.data[p.valueField];
                    var text = node.data[p.textField];
                    g._changeValue(value, text, true);
                    g._toggleSelectBox(true);
                };
                tree.onCancelSelect = function (node) {
                    g._changeValue("", "", true);
                };
            }
            tree.onAfterAppend = function (domnode, nodedata) {
                if (!g.treeManager) return;
                var value = null;
                if (p.initValue) value = p.initValue;
                else if (g.valueField.val() != "") value = g.valueField.val();
                g.selectValueByTree(value);
            };
            if (g.tree) {
                g.tree.remove();
            }
            g.tree = $("<ul></ul>");
            $("div:first", g.selectBox).append(g.tree);
            //新增下拉框中获取树对象的接口
            g.innerTree = g.tree.leeTree(tree);
            g.treeManager = g.tree.leeGetTreeManager();
        },
        //新增下拉框中获取树对象的接口
        getTree: function () {
            return this.innerTree;
        },
        selectValueByTree: function (value) {
            var g = this,
                p = this.options;
            if (value != null) {
                var text = g.treeSelectInit(value);

                g._changeValue(value, text, p.initIsTriggerEvent);
            }
        },
        //Tree选择状态初始化
        treeSelectInit: function (value) {
            var g = this,
                p = this.options;
            if (value != null) {
                var text = "";
                var valuelist = value.toString().split(p.split);
                $(valuelist).each(function (i, item) {
                    g.treeManager.selectNode(item.toString(), false);
                    text += g.treeManager.getTextByID(item);
                    if (i < valuelist.length - 1) text += p.split;
                });
                return text;
            }
        },
        //表格
        _getValue: function () {

            var g = this,
                p = this.options;
            if (p.isbit) {
                return $(this.valueField).val() == "1" ? true : false
            }
            if (p.isTextBoxMode) {
                return g.inputText.val();
            }
            return $(this.valueField).val();
        },
        getValue: function () {
            //获取值
            return this._getValue();
        },
        getSelected: function () {
            return this.selected;
        },
        //向下滚动
        upFocus: function () {
            var g = this,
                p = this.options;
            if (g.grid) {
                if (!g.grid.rows || !g.grid.rows.length) return;
                var selected = g.grid.getSelected();
                if (selected) {
                    var index = $.inArray(selected, g.grid.rows);
                    if (index - 1 < g.grid.rows.length) {
                        g.grid.unselect(selected);
                        g.grid.select(g.grid.rows[index - 1]);
                    }
                } else {
                    g.grid.select(g.grid.rows[0]);
                }

            } else {
                var currentIndex = g.selectBox.table.find("td.lee-over").attr("index");
                if (currentIndex == undefined || currentIndex == "0") {
                    return;
                } else {
                    currentIndex = parseInt(currentIndex) - 1;
                }
                g.selectBox.table.find("td.lee-over").removeClass("lee-over");
                g.selectBox.table.find("td[index=" + currentIndex + "]").addClass("lee-over");

                g._scrollAdjust(currentIndex);
            }
        },
        //向下滚动
        downFocus: function () {
            var g = this,
                p = this.options;
            if (g.grid) {
                if (!g.grid.rows || !g.grid.rows.length) return;
                var selected = g.grid.getSelected();
                if (selected) {
                    var index = $.inArray(selected, g.grid.rows);
                    if (index + 1 < g.grid.rows.length) {
                        g.grid.unselect(selected);
                        g.grid.select(g.grid.rows[index + 1]);
                    }
                } else {
                    g.grid.select(g.grid.rows[0]);
                }

            } else {
                var currentIndex = g.selectBox.table.find("td.lee-over").attr("index");
                if (currentIndex == g.data.length - 1) return;
                if (currentIndex == undefined) {
                    currentIndex = 0;
                } else {
                    currentIndex = parseInt(currentIndex) + 1;
                }
                g.selectBox.table.find("td.lee-over").removeClass("lee-over");
                g.selectBox.table.find("td[index=" + currentIndex + "]").addClass("lee-over");

                g._scrollAdjust(currentIndex);
            }
        },

        _scrollAdjust: function (currentIndex) {
            var g = this,
                p = this.options;
            var boxHeight = $(".lee-box-select-inner", g.selectBox).height();
            var fullHeight = $(".lee-box-select-inner table", g.selectBox).height();
            if (fullHeight <= boxHeight) return;
            var pageSplit = parseInt(fullHeight / boxHeight) + ((fullHeight % boxHeight) ? 1 : 0); //分割成几屏
            var itemHeight = fullHeight / g.data.length; //单位高度
            //计算出位于第几屏
            var pageCurrent = parseInt((currentIndex + 1) * itemHeight / boxHeight) + (((currentIndex + 1) * itemHeight % boxHeight) ? 1 : 0);
            $(".lee-box-select-inner", g.selectBox).scrollTop((pageCurrent - 1) * boxHeight);
        },

        getText: function () {
            return this.inputText.val();
        },
        setText: function (value) {
            var g = this,
                p = this.options;
            if (p.isTextBoxMode) return;
            g.inputText.val(value);
        },
        updateStyle: function () {
            var g = this,
                p = this.options;
            p.initValue = g._getValue();
            g._dataInit();
        },
        _dataInit: function () {
            var g = this,
                p = this.options;
            var value = null;
            if (p.initValue != null && p.initText != null) {

                g._changeValue(p.initValue, p.initText);
            }
            //根据值来初始化
            if (p.initValue != null) {
                value = p.initValue;
                if (p.tree) {
                    if (value)
                        g.selectValueByTree(value);
                } else if (g.data) {
                    var text = g.findTextByValue(value);
                    g._changeValue(value, text);
                }
            } else if (g.valueField.val() != "") {
                value = g.valueField.val();
                if (p.tree) {
                    if (value)
                        g.selectValueByTree(value);
                } else if (g.data) {
                    var text = g.findTextByValue(value);
                    g._changeValue(value, text);
                }
            }
            if (!p.isShowCheckBox) {
                $("table tr", g.selectBox).find("td:first").each(function () {
                    if (value != null && value == $(this).attr("value")) {
                        $(this).addClass("lee-selected");
                    } else {
                        $(this).removeClass("lee-selected");
                    }
                });
            } else {
                $(":checkbox", g.selectBox).each(function () {
                    var parentTD = null;
                    var checkbox = $(this);
                    if (checkbox.parent().get(0).tagName.toLowerCase() == "div") {
                        parentTD = checkbox.parent().parent().parent();
                    } else {
                        parentTD = checkbox.parent();
                    }
                    if (parentTD == null) return;
                    $(".lee-checkbox", parentTD).removeClass("lee-checkbox-checked");
                    checkbox[0].checked = false;
                    var valuearr = (value || "").toString().split(p.split);
                    $(valuearr).each(function (i, item) {
                        if (value != null && item == parentTD.attr("value")) {
                            $(".lee-checkbox", parentTD).addClass("lee-checkbox-checked");
                            checkbox[0].checked = true;
                        }
                    });
                });
            }
        },
        //设置值到 文本框和隐藏域
        //isSelectEvent：是否选择事件
        _changeValue: function (newValue, newText, isSelectEvent) {

            var g = this,
                p = this.options;
            g.valueField.val(newValue);
            if (p && p.render) {
                g.inputText.val(p.render(newValue, newText));
            } else {
                g.inputText.val(newText);
            }
            if (g.select) {
                $("option", g.select).each(function () {
                    $(this).attr("selected", $(this).attr("value") == newValue);
                });
            }
            g.selectedValue = newValue;
            g.selectedText = newText;



            //if (isSelectEvent && newText) {
            if (isSelectEvent) {
                //g.inputText.focus();
                g.inputText.trigger("change");
            }

            var rowData = null;
            if (newValue && typeof (newValue) == "string" && newValue.indexOf(p.split) > -1) {
                rowData = [];
                var values = newValue.split(p.split);
                $(values).each(function (i, v) {
                    rowData.push(g.getRow(v));
                });
            } else if (newValue) {
                rowData = g.getRow(newValue);
            }
            //触发选中事件

            var srcCtrl = g.textFieldID;
            if (p.gridEditParm) srcCtrl = p.gridEditParm.column.columnname;
            if (isSelectEvent) {
                g.trigger('selected', [newValue, newText, rowData, p.gridEditParm, srcCtrl]);
                //g.trigger('ChangeValue', [newValue, newText, rowData]);
            }
            //g.inputText.focus();
        },
        //更新选中的值(复选框)
        _checkboxUpdateValue: function () {
            var g = this,
                p = this.options;
            var valueStr = "";
            var textStr = "";
            $("input:checked", g.selectBox).each(function () {
                var parentTD = null;
                if ($(this).parent().get(0).tagName.toLowerCase() == "div") {
                    parentTD = $(this).parent().parent().parent();
                } else {
                    parentTD = $(this).parent();
                }
                if (!parentTD) return;
                valueStr += parentTD.attr("value") + p.split;
                textStr += parentTD.attr("text") + p.split;
            });
            if (valueStr.length > 0) valueStr = valueStr.substr(0, valueStr.length - 1);
            if (textStr.length > 0) textStr = textStr.substr(0, textStr.length - 1);
            g._changeValue(valueStr, textStr);
        },
        loadDetail: function (value, callback) {
            var g = this,
                p = this.options;
            var parms = $.isFunction(p.detailParms) ? p.detailParms.call(g) : p.detailParms;
            parms[p.detailPostIdField || "id"] = value;
            if (p.ajaxContentType == "application/json") {
                parms = liger.toJSON(parms);
            }
            var ajaxOp = {
                type: p.ajaxType,
                url: p.detailUrl,
                data: parms,
                cache: true,
                dataType: 'json',
                beforeSend: p.ajaxBeforeSend,
                complete: p.ajaxComplete,
                success: function (result) {
                    var data = $.isFunction(p.detailDataGetter) ? p.detailDataGetter(result) : result;
                    data = p.detailDataParmName ? data[p.detailDataParmName] : data;
                    callback && callback(data);
                }
            };

            if (p.ajaxContentType) {
                ajaxOp.contentType = p.ajaxContentType;
            }
            $.ajax(ajaxOp);

        },
        enabledLoadDetail: function () {
            var g = this,
                p = this.options;
            return p.detailUrl && p.detailEnabled ? true : false;
        },
        _addClickEven: function () {
            var g = this,
                p = this.options;
            //选项点击
            $(".lee-table-nocheckbox td", g.selectBox).click(function () {
                var jcell = $(this);
                var value = jcell.attr("value");
                var index = parseInt($(this).attr('index'));
                var data = g.data[index];
                var text = jcell.attr("text");
                var isRowReadonly = jcell.parent().hasClass("rowreadonly");
                if (isRowReadonly) return;
                if (g.enabledLoadDetail()) {
                    g.loadDetail(value, function (rd) {
                        g.data[index] = data = rd;
                        onItemClick();
                    });
                } else {
                    onItemClick();
                }

                function onItemClick() {

                    if (g.hasBind('beforeSelect') && g.trigger('beforeSelect', [value, text, data]) == false) {
                        g._toggleSelectBox(true);
                        return false;
                    }
                    g.selected = data;
                    if ($(this).hasClass("lee-selected")) { //如果已经选中了
                        g._toggleSelectBox(true);
                        return;
                    }
                    $(".lee-selected", g.selectBox).removeClass("lee-selected"); //移除选中样式
                    jcell.addClass("lee-selected"); //添加选中样式
                    if (g.select) {
                        if (g.select[0].selectedIndex != index) {
                            g.select[0].selectedIndex = index; //设置索引
                            g.select.trigger("change"); //触发change事件
                        }
                    }
                    g._toggleSelectBox(true);
                    g.lastInputText = text;

                    g._changeValue(value, text, true);
                }
            });
        },
        updateSelectBoxPosition: function () {
            var g = this,
                p = this.options;
            g._setSelectBoxWidth();
            if (p && p.absolute) {
                var contentHeight = $(document).height();
                if (p.alwayShowInTop || Number(g.wrapper.offset().top + 1 + g.wrapper.outerHeight() + g.selectBox.height()) > contentHeight &&
                    contentHeight > Number(g.selectBox.height() + 1)) {
                    //若下拉框大小超过当前document下边框,且当前document上留白大于下拉内容高度,下拉内容向上展现
                    g.selectBox.css({
                        left: g.wrapper.offset().left,
                        top: g.wrapper.offset().top - 1 - g.selectBox.height() + (p.selectBoxPosYDiff || 0)
                    });
                } else {
                    g.selectBox.css({
                        left: g.wrapper.offset().left,
                        top: g.wrapper.offset().top + 1 + g.wrapper.outerHeight() + (p.selectBoxPosYDiff || 0)
                    });
                }
                if (p.alwayShowInDown) {
                    g.selectBox.css({
                        left: g.wrapper.offset().left,
                        top: g.wrapper.offset().top + 1 + g.wrapper.outerHeight()
                    });
                }
            } else {
                var topheight = g.wrapper.offset().top - $(window).scrollTop();
                var selfheight = g.selectBox.height() + textHeight + 4;
                if (topheight + selfheight > $(window).height() && topheight > selfheight) {
                    g.selectBox.css("marginTop", -1 * (g.selectBox.height() + textHeight + 5) + (p.selectBoxPosYDiff || 0));
                }
            }
        },
        _setSelectBoxWidth: function () {
            var g = this,
                p = this.options;
            if (p.selectBoxWidth) {
                g.selectBox.width(p.selectBoxWidth);
            } else {
                g.selectBox.css('width', g.wrapper.css('width'));
            }
        },
        _toggleSelectBox: function (isHide) {
            var g = this,
                p = this.options;

            if (!g || !p) return;
            //避免同一界面弹出多个菜单的问题
            var managers = $.leeUI.find($.leeUI.controls.DropDown);
            for (var i = 0, l = managers.length; i < l; i++) {
                var o = managers[i];
                if (o.id != g.id) {
                    if (o.selectBox.is(":visible") != null && o.selectBox.is(":visible")) {
                        o._toggleSelectBox(true);
                    }
                }
            }
            managers = $.leeUI.find($.leeUI.controls.DateEditor);
            for (var i = 0, l = managers.length; i < l; i++) {
                var o = managers[i];
                if (o.id != g.id) {
                    if (o.dateeditor.is(":visible") != null && o.dateeditor.is(":visible")) {
                        o.dateeditor.hide(); //日期控件隐藏
                    }
                }
            }

            //图标翻转类
            if (isHide) {
                g.wrapper.removeClass("lee-text-focus");
                g.link.removeClass("reverse");
            } else {
                g.link.addClass("reverse");
                g.wrapper.addClass("lee-text-focus");
            }
            var textHeight = g.wrapper.height();
            g.boxToggling = true;
            if (isHide) {
                if (p.slide) {
                    g.selectBox.slideToggle('fast', function () {
                        g.boxToggling = false;
                    });
                } else {
                    g.selectBox.hide();
                    g.boxToggling = false;
                }
            } else {
                g.updateSelectBoxPosition();
                if (p.slide) {
                    g.selectBox.slideToggle('fast', function () {
                        g.boxToggling = false;
                        if (!p.isShowCheckBox && $('td.lee-selected', g.selectBox).length > 0) {
                            var offSet = ($('td.lee-selected', g.selectBox).offset().top - g.selectBox.offset().top);
                            $(".lee-box-select-inner", g.selectBox).animate({
                                scrollTop: offSet
                            });
                        }
                    });
                } else {
                    g._selectBoxShow();
                    g.boxToggling = false;
                    if (!g.tree && !g.grid && !p.isShowCheckBox && $('td.lee-selected', g.selectBox).length > 0) {
                        var offSet = ($('td.lee-selected', g.selectBox).offset().top - g.selectBox.offset().top);
                        $(".lee-box-select-inner", g.selectBox).animate({
                            scrollTop: offSet
                        });
                    }
                }
            }
            g.isShowed = g.selectBox.is(":visible");
            g.trigger('toggle', [isHide]);
            g.trigger(isHide ? 'hide' : 'show');
        },
        _selectBoxShow: function () {
            var g = this,
                p = this.options;

            if (p.readonly) return;
            if (!p.grid && !p.tree) {
                if (g.selectBox.table.find("tr").length || (p.selectBoxRender && g.selectBoxInner.html())) {
                    g.selectBox.show();
                } else {
                    g.selectBox.hide();
                }
                return;
            }
            g.selectBox.show();

            return;
        },
        _highLight: function (str, key) {
            if (!str) return str;
            var index = str.indexOf(key);
            if (index == -1) return str;
            return str.substring(0, index) + "<span class='lee-highLight'>" + key + "</span>" + str.substring(key.length + index);
        },
        _setAutocomplete: function (value) {
            var g = this, p = this.options;
            if (!value) return;
            if (p.readonly) return;
            g.inputText.removeAttr("readonly");
            g.lastInputText = g.inputText.val();
            g.inputText.keyup(function (event) {
                if (event.keyCode == 38 || event.keyCode == 40 || event.keyCode == 13) //up 、down、enter
                {
                    return;
                }
                if (this._acto)
                    clearTimeout(this._acto);
                this._acto = setTimeout(function () {
                    if (g.lastInputText == g.inputText.val()) return;




                    var currentKey = g.inputText.val();
                    if (currentKey) {
                        currentKey = currentKey.replace(/(^\s*)|(\s*$)/g, "");
                    }
                    else {
                        p.initValue = "";
                        g.valueField.val("");
                    }

                    g.lastInputText = g.inputText.val();

                    if ($.isFunction(value)) {
                        value.call(g, {
                            key: currentKey,
                            show: function () {
                                g._selectBoxShow();
                            }
                        });
                        return;
                    }
                    if (!p.autocompleteAllowEmpty && !currentKey) {
                        g.clear();
                        g.selectBox.hide();
                        return;
                    }
                    if (p.url) {
                        g.setParm('key', currentKey);
                        g.setUrl(p.url, function () {
                            g._selectBoxShow();
                        });
                    } else if (p.grid) {
                        g.grid.setParm('key', currentKey);
                        g.grid.reload();
                    } else {
                        var filterarr = [];
                        $.each(p.data, function (i, obj) {
                            if (obj[p.textField].indexOf(currentKey) != -1) {
                                filterarr.push(obj);
                            }
                        });
                        g.setData(filterarr, true);

                        g._toggleSelectBox(false);
                    }

                    this._acto = null;
                }, 100);
            });
        }
    });

    //键盘事件支持
    (function () {
        $(document).unbind('keydown.leeDropdown');
        $(document).bind('keydown.leeDropdown', function (event) {
            function down() {
                if (!combobox.selectBox.is(":visible")) {
                    //combobox.selectBox.show();
                    combobox._toggleSelectBox(false);
                }
                combobox.downFocus();
            }

            function toSelect() {
                if (!curGridSelected) {
                    combobox._changeValue(value, curTd.attr("text"), true);

                    combobox.selectValue(value);
                    //combobox.selectBox.hide();
                    //combobox._toggleSelectBox(true);
                    //combobox.setSelect();
                    combobox.trigger('textBoxKeyEnter', [{
                        element: curTd.get(0)
                    }]);
                } else {
                    combobox._changeValue(curGridSelected[combobox_op.valueField], curGridSelected[combobox_op.textField], true);

                    //combobox.selectBox.hide();
                    combobox.trigger('textBoxKeyEnter', [{
                        rowdata: curGridSelected
                    }]);
                }
                combobox._toggleSelectBox(true);
            }
            var curInput = $("input:focus");
            if (curInput.length && curInput.attr("data-dropdownid")) {
                var combobox = leeUI.get(curInput.attr("data-dropdownid"));
                if (!combobox) return;
                var combobox_op = combobox.options;
                if (!combobox.get("keySupport")) return;
                if (event.keyCode == 38) //up 
                {
                    combobox.upFocus();
                } else if (event.keyCode == 40) //down
                {
                    if (combobox.hasBind('textBoxKeyDown')) {
                        combobox.trigger('textBoxKeyDown', [{
                            callback: function () {
                                down();
                            }
                        }]);
                    } else {
                        down();
                    }
                } else if (event.keyCode == 13) //enter
                {
                    if (!combobox.selectBox.is(":visible")) return;
                    var curGridSelected = null;
                    if (combobox.grid) {
                        curGridSelected = combobox.grid.getSelected();

                    }
                    var curTd = combobox.selectBox.table.find("td.lee-over");
                    if (curGridSelected || curTd.length) {
                        var value = curTd.attr("value");
                        if (curGridSelected && curGridSelected.ID) value = curGridSelected.ID;

                        if (combobox.enabledLoadDetail()) {
                            combobox.loadDetail(value, function (data) {
                                if (!curGridSelected) {
                                    var index = combobox.getRowIndex(value);
                                    if (index == -1) return;
                                    combobox.data = combobox.data || [];
                                    combobox.data[index] = combobox.selected = data;
                                }
                                toSelect();
                            });
                        } else {
                            toSelect();
                        }

                    }

                }
            }
        });

    })();

})(jQuery);
(function ($) {
    $.fn.leeDate = function () {
        return $.leeUI.run.call(this, "leeUIDate", arguments);
    };
    $.leeUIDefaults.Date = {
        format: "yyyy-MM-dd",
        vFormat: "yyyy-MM-dd",//值掩码
        showTime: false,
        startDate: "",
        range: false,
        showType: "date",//year month time datetime
        cancelable: true,
        theme: "#108ee9",
        max: "",
        min: ""
    };
    $.leeUI.controls.Date = function (element, options) {
        $.leeUI.controls.Date.base.constructor.call(this, element, options);
    };

    $.leeUI.controls.Date.leeExtend($.leeUI.controls.Input, {
        __getType: function () {
            return 'Date';
        },
        __idPrev: function () {
            return 'Date';
        },
        _render: function () {
            var g = this,
                p = this.options;
            if (p.showTime) {
                p.format = "yyyy-MM-dd HH:mm:ss";
            }
            g.inputText = $(this.element);
            g.textFieldID = this.element.id;


            g.valueField = $('<input type="hidden"/>');
            g.valueField[0].id = g.valueField[0].name = g.textFieldID + "_val";

            g.link = $('<div class="lee-right"><div class="lee-icon lee-ion-android-calendar popup"></div></div>');
            g.wrapper = g.inputText.wrap('<div class="lee-text lee-text-date"></div>').parent();
            g.wrapper.append(g.link);
            g.wrapper.append(g.valueField);

            g.valueField.attr("data-uiid", g.id);
            g.valueField.data("beforeText", "").data("beforeValue", "");
            g.inputText.addClass("lee-text-field");

            //开关 事件 图标绑定
            g.link.hover(function () {
                if (p.disabled) return;
                $(this).addClass("lee-right-hover");
            }, function () {
                if (p.disabled) return;
                $(this).removeClass("lee-right-hover");
            }).mousedown(function () {
                if (p.disabled) return;
                $(this).addClass("lee-right-pressed");

            }).mouseup(function () {
                if (p.disabled) return;
                $(this).removeClass("lee-right-pressed");
            }).click(function () {
                if (p.disabled) return;
                if (g.trigger('beforeOpen', g) == false) return false;
                //g.showDate();
                g.inputText.focus();
            });

            //文本框增加事件绑定
            g.inputText.click(function () {
                if (p.disabled) return;
                g.showDate();

            }).blur(function () {
                if (p.disabled) return;
                g.wrapper.removeClass("lee-text-focus");
            }).focus(function () {
                if (p.disabled) return;
                g.wrapper.addClass("lee-text-focus");
            }).change(function () {

                g.trigger('change');
            });

            var opts = {
                elem: "#" + g.textFieldID,
                range: p.range,
                format: p.format,
                //theme: p.theme,
                type: p.showType,
                done: function (value, date, enddate) { //监听日期被切换
                    g.inputText.trigger('change');
                }
            };
            if (p.min) opts.min = p.min;
            if (p.max) opts.max = p.max;
            laydate.render(opts);
            g.set(p);
        },
        showDate: function () {
            var g = this,
                p = this.options;

            //WdatePicker({
            //    el: g.textFieldID,
            //    dateFmt: p.format
            //});
        },

        dateFormat: function (value, format) {
            var date = this.stringToDate(value);
            format = format || "yyyy-MM-dd";
            if (date == null || date == "NaN") return null;
            if (date.getSeconds() == 0) date.setSeconds((new Date()).getSeconds());
            var o = {
                "M+": date.getMonth() + 1,
                "d+": date.getDate(),
                "h+": date.getHours(),
                "H+": date.getHours(),
                "m+": date.getMinutes(),
                "s+": date.getSeconds(),
                "q+": Math.floor((date.getMonth() + 3) / 3),
                "S": date.getMilliseconds()
            };
            if (/(y+)/.test(format)) {
                format = format.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length));
            }
            for (var k in o) {
                if (new RegExp("(" + k + ")").test(format)) {
                    format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
                }
            }
            return format;
        },
        stringToDate: function (str) {
            str = str.replace("T", " ");
            var vsre = null;
            if (str != null) {
                str = str.replace(/\-/g, " ").replace(/\:/g, " ");
                var datearr = str.split(' ');
                if (datearr.length > 2) {
                    datearr[1] = Number(datearr[1]) - 1;
                    var vslen = 6 - datearr.length;
                    for (var i = 0; i < vslen; i++)
                        datearr.push('00');
                }
                vsre = new Date(datearr[0], datearr[1], datearr[2], datearr[3], datearr[4], datearr[5]);
            }
            return vsre;
        },
        _toggle: function () {

        },
        _setWidth: function (value) {
            if (value > 20) {
                this.wrapper.css({ width: value });
                //this.inputText.css({ width: value - 4 });
            }
        },
        //取消选择 
        _setCancelable: function (value) {
            var g = this,
                p = this.options;
            if (!value && g.unselect) {
                g.unselect.remove();
                g.unselect = null;
            }
            if (!value && !g.unselect) return;
            g.unselect = $('<div class="lee-clear"><i class="lee-icon lee-icon-close lee-clear-achor"></i></div>').hide();
            g.wrapper.hover(function () {
                if (!p.disabled)
                    g.unselect.show();
            }, function () {
                g.unselect.hide();
            })
            if (!p.disabled && !p.readonly && p.cancelable) {
                g.wrapper.append(g.unselect);
            }
            g.unselect.click(function () {
                g._setValue("");
                g.inputText.trigger('change');

            });
        },
        _setValue: function (value) {
            if (value)
                value = this.dateFormat(value, this.options.format)
            this.inputText.val(value);
            this.valueField.val(value);//值掩码 需要格式化
        },
        _getValue: function () {
            return this.inputText.val();
        },
        _setDisabled: function (flag) {

            //禁用样式
            if (flag) {
                this.options.disabled = true;
                this.wrapper.addClass('lee-text-disabled');
                this.inputText.attr("readonly", "readonly").attr("disabled", "disabled");
            } else {
                this.options.disabled = false;
                this.wrapper.removeClass('lee-text-disabled');
                this.inputText.removeAttr("readonly").removeAttr("disabled");
            }

        },
        _setRequired: function (value) {
            if (value) {
                this.wrapper.addClass('lee-text-required');
            } else {
                this.wrapper.removeClass('lee-text-required');
            }
        },
    });
})(jQuery);
(function ($) {

    $.fn.leeLookup = function (options) {
        return $.leeUI.run.call(this, "leeUILookup", arguments);
    };
    $.leeUIDefaults.Lookup = {
        helpID: "", //帮助ID
        modelID: "",
        valueField: 'Code', //值字段
        textField: "Name",//显示字段
        codeField: "",//检索字段
        returnFields: [],//返回字段
        mapFields: [],//赋值字段
        dgHeight: "380",
        title: "请选择",
        dgWidth: "550",
        type: "", //类型 1.页面内 2.新页面 
        nameSwitch: true,
        dockType: "", //1.下拉 2.侧边栏
        isTree: "", //是否树形
        childOnly: false,// 只选明细
        isMul: false, //是否多选
        checkbox: false,
        isMulGrid: false,// 多选你分为两种模式、1.多条，2.子表回弹
        isAuto: true, //启用检索
        url: "",//帮助预览地址
        cancelable: true,
        render: null, //显示函数   
        split: ',', //多选分隔符
        condition: null, // 条件字段,比如 {fields:[{ name : 'Title' ,op : 'like', vt : 'string',type:'text' }]}
        onBeforeOpen: null,
        textmode: false
    };
    $.leeUI.controls.Lookup = function (element, options) {
        $.leeUI.controls.Popup.base.constructor.call(this, element, options);
    };
    $.leeUI.controls.Lookup.leeExtend($.leeUI.controls.Input, {
        __getType: function () {
            return 'Lookup';
        },
        _init: function () {
            $.leeUI.controls.Lookup.base._init.call(this);
        },
        _render: function () {
            var g = this,
                p = this.options;
            g.inputText = null;
            //文本框初始化
            if (this.element.tagName.toLowerCase() == "input") {
                //this.element.readOnly = true;
                g.inputText = $(this.element); //记录初始控件
                g.textFieldID = this.element.id;

                if ($(this.element).attr("readonly"))
                    p.disabled = true;
            }
            //g.valueField = null;
            g.defer = null;
            g.valueField = $('<input type="hidden"/>');
            g.valueField[0].id = g.valueField[0].name = g.textFieldID + "_val";


            g.link = $('<div class="lee-right"><div class="lee-icon lee-ion-android-more-horizontal popup"></div></div>');
            //外层
            g.wrapper = g.inputText.wrap('<div class="lee-text lee-text-popup"></div>').parent();

            g.wrapper.append(g.link);
            g.wrapper.append(g.valueField);
            //修复popup控件没有data-ligerid的问题
            g.valueField.attr("data-uiid", g.id);

            g.valueField.data("beforeText", "").data("beforeValue", "");
            g.inputText.addClass("lee-text-field");

            if (!p.nameSwitch) {
                g.inputText.attr("readonly", "readonly");
            }
            //开关 事件 图标绑定
            g.link.hover(function () {
                if (p.disabled) return;
                $(this).addClass("lee-right-hover");
            }, function () {
                if (p.disabled) return;
                $(this).removeClass("lee-right-hover");
            }).mousedown(function () {
                if (p.disabled) return;
                $(this).addClass("lee-right-pressed");
            }).mouseup(function () {
                if (p.disabled) return;
                $(this).removeClass("lee-right-pressed");
            }).click(function () {
                if (p.disabled) return;
                if (g.trigger('beforeOpen', g) == false) return false;
                g.openLookup();
            });
            g.inputText.data("otext", "");
            g.inputText.data("ovalue", "");
            //文本框增加事件绑定
            g.inputText.click(function () {
                if (p.disabled) return;
            }).blur(function () {
                if (p.disabled) return;
                g.wrapper.removeClass("lee-text-focus");
            }).focus(function () {
                if (p.disabled) return;
                g.wrapper.addClass("lee-text-focus");
            });
            g.inputText.bind("keydown", function (e) {

                if (e.keyCode == "13") {
                    g.inputText.trigger("change");
                }

            });
            g.inputText.on("change", function (e) {
                if (!p.nameSwitch) return;
                var otext = g.valueField.data("beforeText");//原来的名称
                var ovalue = g.valueField.data("beforeValue");//原来的value


                g.onTextSearch.call(g, e, otext, ovalue);

            });

            g.wrapper.hover(function () {
                if (p.disabled) return;
                g.wrapper.addClass("lee-text-over");
            }, function () {
                if (p.disabled) return;
                g.wrapper.removeClass("lee-text-over");
            });

            g.set(p);
            //g.setTextByVal(g.getValue());
            //alert(g.getValue());
        },
        setAchorLoading: function () {
            var g = this, p = this.options;
            g.link.addClass("loading");
            g.link.find(".popup").addClass("lee-ion-load-d");
        },
        setAchorNormal: function () {
            var g = this, p = this.options;
            g.link.removeClass("loading");
            g.link.find(".popup").removeClass("lee-ion-load-d");
        },
        onTextSearch: function (e, otext, ovalue) {
            var g = this, p = this.options;

            if (g.inputText.val() == "") {
                var obj = {};
                g.setValue("", "");
                obj[p.textField] = obj[p.valueField] = "";
                g.confirmSelect([obj]);
                return;
            }
            if (g.inputText.val() == otext) return;
            g.defer = $.Deferred();//创建一个异步promise
            g.getQueryResult(g.inputText.val());
            //var res = { Code: "0101", Name: "哈哈哈" };//查询结果
            //g.setValue(res[p.valueField], res[p.textField]);
            //g.trigger("valuechange", res); // 触发值改变事件
            ////如果没有匹配 那么则把text改成原来的 并给予提示
        },
        getDefer: function () {
            var g = this;
            if (g.defer) {
                return g.defer.promise();
            }
            return null;
        },
        getQueryResult: function (value) {
            //getModelDataByDataID
            var g = this;
            if (g.query) return;
            g.query = true;
            var g = this, p = this.options;
            if (p.service) {
                g.setAchorLoading();
                var filter = "";
                if (p.getFilter) {
                    filter = p.getFilter();
                }
                p.service.getQueryHelpSwitch(p.helpID, value, p.codeField, p.textField, filter, false).done(function (data) {

                    if (data.res) {
                        var arr = data.data;

                        if (arr.length > 1 || arr.length == 0) {
                            if (p.textmode) {
                                var obj = {};
                                obj[p.textField] = obj[p.valueField] = value;
                                g.inputText.val(value);
                                g.valueField.val(value);
                                g.confirmSelect([obj]);
                            }

                            if (p.textmode && arr.length == 0) {
                                g.query = false;
                            } else {
                                g.setKeyword(value);
                                g.openLookup();
                            }
                        }
                        else {
                            g.confirmSelect(arr);// 触发选中事件 
                            g.query = false;
                        }
                    }
                    else {
                        g.query = false;
                    }
                    if (g.defer) {
                        g.defer.resolve(data);
                        g.defer = null;
                    }
                }).fail(function (data) {
                    console.log("失败");

                }).always(function () {
                    g.setAchorNormal();
                });
            }
        },
        destroy: function () {
            if (this.wrapper) this.wrapper.remove();
            //this.options = null;
            $.leeUI.remove(this);
        },
        clear: function () {
            var g = this,
                p = this.options;
            g.inputText.val("");
            g.valueField.val("");
            g.curData = null;
            g.trigger("valuechange", {}); // 清空触发值改变事件

            var srcCtrl = g.textFieldID;
            if (p.gridEditParm) srcCtrl = p.gridEditParm.column.columnname;

            g.trigger('clearValue', [g, p, {}, srcCtrl]);
        },
        getLookUpContext: function () {

            // 如果parent有dialog对象则用parentdialog

            var dgContext = this.getContext().document.getElementById('lookupwindow').contentWindow;
            return dgContext.lookupHelper;
        },
        getContext: function () {
            if (window.parent && window.parent.$ && window.parent.$.leeDialog) {
                return window.parent;
            }
            return window;
        },
        setKeyword: function (value) {
            this.keyword = value;
        },
        getKeyword: function (value) {
            return this.keyword;
        },
        dealcolse: function () {
            var g = this, p = this.options;
            g.setKeyword("");
            if (g.inputText.data("otext") !== g.inputText.val()) {
                //alert(1);
                if (!p.textmode)
                    g.inputText.val(g.inputText.data("otext"));
            }
            g.query = false;
        },
        openLookup: function () {
            var g = this, p = this.options;

            // 如果parent页面有dialog对象 则采用parent
            if (g.isopen) return; //如果已经弹出则不允许弹出了
            g.isopen = true;
            var $openDg = this.getContext().$.leeDialog.open({
                title: p.title,
                name: 'lookupwindow',
                isHidden: false,
                showMax: true,
                width: p.dgWidth,
                slide: false,
                height: p.dgHeight,
                onclose: function () {
                    g.dealcolse();
                    g.isopen = false;
                },
                url: p.url + p.helpID + "&keyword=" + g.inputText.val(),
                onLoaded: function () {
                    //alert(1);
                    var lookupHelper = g.getLookUpContext();



                    var filter = p.getFilter();
                    if (filter) {
                        lookupHelper.setFilter(filter);
                    }
                    lookupHelper.init();

                    //context.setOptions(self.opts);
                    lookupHelper.setOptions({
                        textField: p.textField,
                        codeField: p.codeField,
                        isMul: p.isMul || p.isMulGrid,
                        isChildOnly: p.isChildOnly,
                        async: p.async,
                        keyword: g.keyword,
                        filter: p.getFilter(),
                        params: p.params
                    });
                    //lookupHelper.showView();
                },
                buttons: [
                    {
                        id: "dialog_lookup_ok",
                        text: '选择',
                        cls: 'lee-btn-primary lee-dialog-btn-ok',
                        onclick: function (item, dialog) {
                            // leeUI.Error("请选中要操作的数据！");


                            var res = g.getLookUpContext().getReturnValue();
                            if (!res) {
                                if (res === false) {

                                } else {
                                    g.getContext().leeUI.Error("请选中要操作的数据！");
                                }
                            }
                            else {

                                g.confirmSelect(res);

                                //单选 多选 触发值改变事件
                                //alert(value);
                                $openDg.close();
                                g.dealcolse();
                                g.isopen = false;
                                g.setKeyword("");
                            }
                        }
                    },
                    {
                        text: '取消',
                        cls: 'lee-dialog-btn-cancel ',
                        onclick: function (item, dialog) {
                            g.dealcolse();
                            g.isopen = false;
                            $openDg.close();

                        }
                    }
                ]
            });

        },
        confirmSelect: function (data) {
            var g = this, p = this.options;
            //触发选中值事件

            var srcCtrl = g.textFieldID;
            if (p.gridEditParm) srcCtrl = p.gridEditParm.column.columnname;
            if (p.isMul && !p.gridEditParm) {
                //批量赋值
                var namearr = [];
                var textarr = [];
                for (var item in data) {
                    textarr.push(data[item][p.textField]);
                    namearr.push(data[item][p.valueField]);
                }
                g.setValue(namearr.join(";"), textarr.join(";"));
            }
            else {
                //单行赋值
                var value = data[0][p.valueField];
                var text = data[0][p.textField];
                g.setValue(value, text);
            }
            g.curData = data;
            g.trigger('change');
            //
            g.trigger('confirmSelect', [g, p, data, srcCtrl]);

        },
        getCurData: function () {
            return this.curData;
        },
        //取消选择 
        _setCancelable: function (value) {
            var g = this,
                p = this.options;
            if (!value && g.unselect) {
                g.unselect.remove();
                g.unselect = null;
            }
            if (!value && !g.unselect) return;
            g.unselect = $('<div class="lee-clear"><i class="lee-icon lee-icon-close lee-clear-achor"></i></div>').hide();
            g.wrapper.hover(function () {
                if (!p.disabled)
                    g.unselect.show();
            }, function () {
                g.unselect.hide();
            })
            if (!p.disabled && !p.readonly && p.cancelable) {
                g.wrapper.append(g.unselect);
            }
            g.unselect.click(function () {
                g.clear();
            });
        },
        _setDisabled: function (value) {
            if (value) {
                this.options.disabled = true;
                this.wrapper.addClass('lee-text-disabled');
                this.inputText.attr("readonly", "readonly");
            } else {
                this.options.disabled = false;
                this.wrapper.removeClass('lee-text-disabled');
                if (this.options.nameSwitch)
                    this.inputText.removeAttr("readonly");
            }
        },
        _setRequired: function (value) {
            if (value) {
                this.wrapper.addClass('lee-text-required');
            } else {
                this.wrapper.removeClass('lee-text-required');
            }
        },
        _setWidth: function (value) {
            var g = this;
            if (value > 20) {
                g.wrapper.css({
                    width: value
                });
                //g.inputText.css({ width: value - 20 });
            }
        },
        _setHeight: function (value) {
            var g = this;
            if (value > 10) {
                g.wrapper.height(value);
                //g.inputText.height(value - 2);
            }
        },
        _getText: function () {
            return $(this.inputText).val();
        },
        _getValue: function () {
            return $(this.valueField).val();
        },
        getValue: function () {
            return this._getValue();
        },
        getText: function () {
            return this._getText();
        },
        //设置值到  隐藏域
        setValue: function (value, text) {
            //if (value == '') return;
            var g = this,
                p = this.options;
            if (arguments.length >= 2) {
                g.setValue(value);
                g.setText(text);
                g.inputText.data("otext", text);
                g.inputText.data("ovalue", value);
                return;
            }
            g.valueField.val(value);
        },
        //设置值到 文本框 
        setText: function (text) {
            var g = this,
                p = this.options;
            if (p.render) {
                g.inputText.val(p.render(text));
            } else {
                g.inputText.val(text);
            }
        }
    });

})(jQuery);
(function ($) {

    $.fn.leePopup = function (options) {
        return $.leeUI.run.call(this, "leeUIPopup", arguments);
    };
    $.leeUIDefaults.Popup = {
        valueFieldID: null, //生成的value input:hidden 字段名
        onButtonClick: null, //利用这个参数来调用其他函数，比如打开一个新窗口来选择值 
        nullText: null, //不能为空时的提示
        disabled: false, //是否无效
        cancelable: true,
        width: null,
        heigth: null,
        onValuechange: null,
        render: null, //显示函数   
        split: ';',
        data: [],
        condition: null, // 条件字段,比如 {fields:[{ name : 'Title' ,op : 'like', vt : 'string',type:'text' }]}
        valueField: 'id', //值字段
        textField: 'text', //显示字段
        parms: null
    };

    $.leeUI.controls.Popup = function (element, options) {
        $.leeUI.controls.Popup.base.constructor.call(this, element, options);
    };
    $.leeUI.controls.Popup.leeExtend($.leeUI.controls.Input, {
        __getType: function () {
            return 'Popup';
        },
        _init: function () {
            $.leeUI.controls.Popup.base._init.call(this);
        },
        _render: function () {
            var g = this,
                p = this.options;
            g.inputText = null;
            //文本框初始化
            if (this.element.tagName.toLowerCase() == "input") {
                this.element.readOnly = true;
                g.inputText = $(this.element);
                g.textFieldID = this.element.id;
            }
            if (g.inputText[0].name == undefined) g.inputText[0].name = g.textFieldID;
            //隐藏域初始化
            g.valueField = null;
            if (p.valueFieldID) {
                g.valueField = $("#" + p.valueFieldID + ":input");
                if (g.valueField.length == 0) g.valueField = $('<input type="hidden"/>');
                if (g.valueField[0].name == undefined) g.valueField[0].id = g.valueField[0].name = p.valueFieldID;
            } else {
                g.valueField = $('<input type="hidden"/>');
                g.valueField[0].id = g.valueField[0].name = g.textFieldID + "_val";
            }
            if (g.valueField[0].name == undefined) g.valueField[0].name = g.valueField[0].id;
            //开关

            g.link = $('<div class="lee-right"><div class="lee-icon lee-icon-search popup"></div></div>');
            //外层
            g.wrapper = g.inputText.wrap('<div class="lee-text lee-text-popup"></div>').parent();

            g.wrapper.append(g.link);
            g.wrapper.append(g.valueField);
            //修复popup控件没有data-ligerid的问题
            g.valueField.attr("data-ligerid", g.id);
            g.inputText.addClass("lee-text-field");
            //开关 事件
            g.link.hover(function () {
                if (p.disabled) return;
                $(this).addClass("lee-right-hover");
                //this.className = "lee-right-hover";
            }, function () {
                if (p.disabled) return;
                $(this).removeClass("lee-right-hover");
                //this.className = "lee-right-hover";
            }).mousedown(function () {
                if (p.disabled) return;
                $(this).addClass("lee-right-pressed");
                //this.className = "lee-right-pressed";
            }).mouseup(function () {
                if (p.disabled) return;
                $(this).removeClass("lee-right-pressed");
                //this.className = "lee-right-pressed"";
            }).click(function () {
                if (p.disabled) return;
                if (g.trigger('buttonClick', g) == false) return false;
            });

            g.inputText.click(function () {
                if (p.disabled) return;
            }).blur(function () {
                if (p.disabled) return;
                g.wrapper.removeClass("lee-text-focus");
            }).focus(function () {
                if (p.disabled) return;
                g.wrapper.addClass("lee-text-focus");
            });
            g.wrapper.hover(function () {
                if (p.disabled) return;
                g.wrapper.addClass("lee-text-over");
            }, function () {
                if (p.disabled) return;
                g.wrapper.removeClass("lee-text-over");
            });

            g.set(p);
            //g.setTextByVal(g.getValue());
        },
        destroy: function () {
            if (this.wrapper) this.wrapper.remove();
            this.options = null;
            $.leeUI.remove(this);
        },
        clear: function () {
            var g = this,
                p = this.options;
            if (g.getValue() == "") {
                return; //如果是空的话不触发值值改变事件
            }
            g.inputText.val("");
            g.valueField.val("");
            g.trigger("valuechange", {});
        },
        //取消选择 
        _setCancelable: function (value) {
            var g = this,
                p = this.options;
            if (!value && g.unselect) {
                g.unselect.remove();
                g.unselect = null;
            }
            if (!value && !g.unselect) return;
            g.unselect = $('<div class="lee-clear"><i class="lee-icon lee-icon-close lee-clear-achor"></i></div>').hide();
            g.wrapper.hover(function () {
                g.unselect.show();
            }, function () {
                g.unselect.hide();
            })
            if (!p.disabled && !p.readonly && p.cancelable) {
                g.wrapper.append(g.unselect);
            }
            g.unselect.click(function () {
                g.clear();
            });
        },
        _setDisabled: function (value) {
            if (value) {
                this.wrapper.addClass('lee-text-disabled');
            } else {
                this.wrapper.removeClass('lee-text-disabled');
            }
        },
        _setWidth: function (value) {
            var g = this;
            if (value > 20) {
                g.wrapper.css({
                    width: value
                });
                //g.inputText.css({ width: value - 20 });
            }
        },
        _setHeight: function (value) {
            var g = this;
            if (value > 10) {
                g.wrapper.height(value);
                //g.inputText.height(value - 2);
            }
        },
        getData: function () {
            var g = this,
                p = this.options;
            var data = [];
            var v = $(g.valueField).val(),
                t = $(g.inputText).val();
            var values = v ? v.split(p.split) : null,
                texts = t ? t.split(p.split) : null;
            $(values).each(function (i) {
                var o = {};
                o[p.textField] = texts[i];
                o[p.valueField] = values[i];
                data.push(o);
            });
            return data;
        },
        _getText: function () {
            return $(this.inputText).val();
        },
        _getValue: function () {
            return $(this.valueField).val();
        },
        getValue: function () {
            return this._getValue();
        },
        getText: function () {
            return this._getText();
        },
        //设置值到  隐藏域
        setValue: function (value, text) {
            //if (value == '') return;
            var g = this,
                p = this.options;
            if (arguments.length >= 2) {
                g.setValue(value);
                g.setText(text);
                return;
            }
            g.valueField.val(value);
            //g.setTextByVal(value);
        },
        //设置值到 文本框 
        setText: function (text) {
            var g = this,
                p = this.options;
            if (p.render) {
                g.inputText.val(p.render(text));
            } else {
                g.inputText.val(text);
            }
        },
        addValue: function (value, text) {
            var g = this,
                p = this.options;
            if (!value) return;
            var v = g.getValue(),
                t = g.getText();
            if (!v) {
                g.setValue(value);
                g.setText(text);
            } else {
                var arrV = [],
                    arrT = [],
                    old = v.split(p.split),
                    value = value.split(p.split),
                    text = text.split(p.split);
                for (var i = 0, l = value.length; i < l; i++) {
                    if ($.inArray(value[i], old) == -1) {
                        arrV.push(value[i]);
                        arrT.push(text[i]);
                    }
                }
                if (arrV.length) {
                    g.setValue(v + p.split + arrV.join(p.split));
                    g.setText(t + p.split + arrT.join(p.split));
                }
            }
        },
        removeValue: function (value, text) {
            var g = this,
                p = this.options;
            if (!value) return;
            var v = g.getValue(),
                t = g.getText();
            if (!v) return;
            var oldV = v.split(p.split),
                oldT = t.split(p.split),
                value = value.split(p.split);
            for (var i = 0, index = -1, l = value.length; i < l; i++) {
                if ((index = $.inArray(value[i], oldV)) != -1) {
                    oldV.splice(index, 1);
                    oldT.splice(index, 1);
                }
            }
            g.setValue(oldV.join(p.split));
            g.setText(oldT.join(p.split));
        }
    });

    //*帮助分两种模式 一种是简单帮助 一种是 iframe帮助*
    $.leeUI.PopUpHelp = function (options) {
        var url = options.url;
        var textField = options.textField;
        var valueField = options.valueField;
        var filedList = []; //字段列表

        var title = options.title || "请选择";
        return function () {
            var g = this;
            var grid = $("<div></div>");
            var gridm;
            $.leeDialog.open({
                title: "选择数据源",
                width: "600",
                height: '400',
                target: grid,
                isResize: true,
                onContentHeightChange: function (height) {

                    //return false;
                },
                onStopResize: function () {

                },
                buttons: [{
                    text: '选择',
                    cls: 'lee-btn-primary lee-dialog-btn-ok',
                    onclick: function (item, dialog) {
                        //toSelect();

                        var selected = gridm.getSelected();
                        if (selected) {
                            g.setText(selected.name);
                            g.setValue(selected.name);
                            g.trigger("valuechange", selected);
                            dialog.close();
                        } else {
                            leeUI.Error("请选中要操作的数据！");
                        }
                        //alert(selected.name);

                    }
                },
                {
                    text: '取消',
                    cls: 'lee-dialog-btn-cancel ',
                    onclick: function (item, dialog) {
                        dialog.close();
                    }
                }
                ]
            });
            gridm = grid.leeGrid({
                columns: [{
                    display: '编号',
                    name: 'code',
                    align: 'left',
                    width: 100,
                    minWidth: 60
                },
                {
                    display: '名称',
                    name: 'name',
                    align: 'left',
                    width: 100,
                    minWidth: 60
                }
                ],
                alternatingRow: false,
                method: "get",
                url: 'data/datamodel.json',
                dataAction: 'server', //服务器排序
                usePager: true, //服务器分页
                pageSize: 20,
                inWindow: false,
                height: "100%",
                rownumbers: true,
                rowHeight: 30
            });
        }
    }

    $.leeUI.PopUp = {};
    //$injector 需要实现的接口有 

    $.leeUI.PopUp.LookupInjector = function (options) {
        this.options = options;
        this.init();
    }

    $.leeUI.PopUp.LookupInjector.prototype = {
        init: function () {
            var g = this,
                p = this.options;
            var self = this;
            this.wrap = $("<div style='position:absolute;top:1px;left:1px;bottom:1px;right:1px;'></div>");
            this.search = $("<div class='lee-text'></div>");
            this.grid = $("<div class='helpgrid' ></div>");


            //this.wrap.append(this.search).append(this.grid);

            if (p.filter) {
                this.$searchwrap = $('<div class="lee-search-wrap" style="float:none;margin:5px;"><input style="width:100%;" class="lee-search-words" type="text"   placeholder="请输入查询关键字"><button class="search lee-ion-search" type="button" style="position: absolute;right: 0;"></button></div>');
                this.wrap.append(this.$searchwrap);
                this.$input = $(this.$searchwrap.find("input"));
                this.$btn = $(this.$searchwrap.find("button"));

                this.$btn.click(function () {
                    self.gridm.loadData(true);
                });
                this.$input.keydown(function (event) {
                    if (event.keyCode == 13) {
                        self.gridm.loadData(true);
                    }

                });
            }

            this.wrap.append(this.grid);

        },
        getParams: function () {
            var res = [];
            res.push({ name: "keyword", value: this.$input.val() });
            return res;
        },
        initUI: function () {
            var g = this,
                p = this.options;
            if (!this.gridm) {
                this.gridm = this.grid.leeGrid({
                    columns: p.column,
                    alternatingRow: false,
                    method: p.method,
                    url: p.url,
                    dataAction: 'server', //服务器排序
                    usePager: true, //服务器分页
                    pageSize: 50,
                    inWindow: false,
                    height: "100%",
                    parms: $.proxy(this.getParams, this),
                    rownumbers: true,
                    rowHeight: 30,
                    checkbox: p.checkbox || false
                });
            } else {
                this.gridm.loadData(true);
            }

        },
        clearSearch: function () {

        },
        getOptions: function () {
            return this.options;
        },
        clear: function () {

        },
        getRenderDom: function () {
            return this.wrap;
        },

        onResize: function () { },
        onConfirm: function (popup, dialog, $injector) {
            var p = $injector.options;
            var selected = $injector.gridm.getSelectedRows();
            if (selected.length > 0) {
                if (p.checkbox) {

                } else {
                    var beforvalue = popup.getValue();
                    popup.setText(selected[0][p.textfield || "text"]);
                    popup.setValue(selected[0][p.valuefield || "id"]);
                    if (beforvalue !== selected[0][p.valuefield || "id"]) {
                        popup.trigger("valuechange", selected[0]);
                    }


                }
                return true;
            }
            leeUI.Warning("没有选中记录");
            return false;
        }
    };

    $.leeUI.PopUp.Lookup = function ($injector) {
        var options = $injector.getOptions();
        var title = options.title || "请选择";

        return function () {
            var g = this;
            $.leeDialog.open({
                title: title,
                width: options.width || 600,
                height: options.height || 400,
                target: $injector.getRenderDom(),
                isResize: true,
                overflow: "hidden",
                onContentHeightChange: function (height) {
                    $injector.onResize.call(this, g);
                },
                onStopResize: function () {
                    $injector.onResize.call(this, g);
                },
                buttons: [{
                    text: '确定',
                    cls: 'lee-btn-primary lee-dialog-btn-ok',
                    onclick: function (item, dialog) {

                        if ($injector.onConfirm.call(this, g, dialog, $injector)) {
                            dialog.close();
                        }
                    }
                },
                {
                    text: '取消',
                    cls: 'lee-dialog-btn-cancel ',
                    onclick: function (item, dialog) {
                        dialog.close();
                    }
                }
                ]
            });
            $injector.initUI();

        }
    }

    //  
    //  $.leeUI.getPopupFn = function (p,master)
    //  {
    //      p = $.extend({
    //          title: '选择数据',     //窗口标题
    //          width: 700,            //窗口宽度     
    //          height: 320,           //列表高度
    //          top: null,
    //          left: null,
    //          split: ';',
    //          valueField: null,    //接收表格的value字段名
    //          textField: null,     //接收表格的text字段名
    //          grid: null,          //表格的参数 同ligerGrid
    //          condition: null,     //搜索表单的参数 同ligerForm
    //          onSelect: function (p) { },   //选取函数 
    //          searchClick : p.searchClick,
    //          selectInit: function (rowdata) { return false }  //选择初始化
    //      }, p);
    //      if (!p.grid) return;
    //      var win, grid, condition, lastSelected = p.lastSelected || [];
    //      return function ()
    //      {
    //          show();
    //          return false;
    //      };
    //      function show()
    //      {
    //          function getGridHeight(height)
    //          {
    //              height = height || p.height;
    //              height -= conditionPanel.height();
    //              return height;
    //          }
    //          if (win)
    //          {
    //              grid._showData();
    //              win.show();
    //              grid.refreshSize();
    //              lastSelected = grid.selected.concat();
    //              return;
    //          }
    //          var panle = $("<div></div>");
    //          var conditionPanel = $("<div></div>");
    //          var gridPanel = $("<div></div>");
    //          panle.append(conditionPanel).append(gridPanel);
    //          
    //          if (p.condition)
    //          { 
    //              var conditionParm = $.extend({
    //                  labelWidth: 60,
    //                  space: 20
    //              }, p.condition);
    //              setTimeout(function ()
    //              {
    //                  condition = conditionPanel.ligerForm(conditionParm);
    //              }, 50);
    //          } else
    //          {
    //              conditionPanel.remove();
    //          }
    //          var gridParm = $.extend({
    //              columnWidth: 120,
    //              alternatingRow: false,
    //              frozen: true,
    //              rownumbers: true
    //          }, p.grid, {
    //              width: "100%",
    //              height: getGridHeight(),
    //              isChecked: p.selectInit,
    //              isSelected: p.selectInit,
    //              inWindow: false
    //          });
    //          //grid
    //          grid = gridPanel.ligerGrid(gridParm);
    //          //搜索按钮
    //          if (p.condition)
    //          {
    //             
    //              setTimeout(function ()
    //              {
    //                  var containerBtn1 = $('<li style="margin-right:9px"><div></div></li>');
    //                  $("ul:first", conditionPanel).append(containerBtn1).after('<div class="l-clear"></div>');
    //                  $("div", containerBtn1).ligerButton({
    //                      text: '搜索',
    //                      click: function ()
    //                      { 
    //                          var rules = condition.toConditions();
    //                          if (p.searchClick)
    //                          {
    //                              p.searchClick({
    //                                  grid: grid,
    //                                  rules: rules
    //                              });
    //                          } else
    //                          {
    //                              if (grid.get('url'))
    //                              {
    //                                  grid.setParm(grid.conditionParmName || 'condition', $.ligerui.toJSON(rules));
    //                                  grid.reload();
    //                              } else
    //                              {
    //                                  grid.loadData($.ligerFilter.getFilterFunction(rules));
    //                              }
    //                          }
    //                      }
    //                  });
    //              }, 100);
    //          }
    //          //dialog
    //          win = $.ligerDialog.open({
    //              title: p.title,
    //              width: p.width,
    //              height: 'auto',
    //              top: p.top,
    //              left: p.left,
    //              target: panle,
    //              isResize: true,
    //              cls: 'l-selectorwin',
    //              onContentHeightChange: function (height)
    //              {
    //                  grid.set('height', getGridHeight(height));
    //                  return false;
    //              },
    //              onStopResize: function ()
    //              {
    //                  grid.refreshSize();
    //              },
    //              buttons: [
    //               { text: '选择', onclick: function (item, dialog) { toSelect(); dialog.hide(); } },
    //               { text: '取消', onclick: function (item, dialog) { dialog.hide(); } }
    //              ]
    //          });
    //
    //          if (master)
    //          {
    //              master.includeControls = master.includeControls || [];
    //              master.includeControls.push(win);
    //          }
    //          grid.refreshSize();
    //      }
    //      function exist(value, data)
    //      {
    //          for (var i = 0; data && data[i]; i++)
    //          {
    //              var item = data[i];
    //              if (item[p.valueField] == value) return true;
    //          }
    //          return false;
    //      }
    //      function toSelect()
    //      {
    //          var selected = grid.selected || [];
    //          var value = [], text = [], data = [];
    //          $(selected).each(function (i, rowdata)
    //          {
    //              p.valueField && value.push(rowdata[p.valueField]);
    //              p.textField && text.push(rowdata[p.textField]);
    //              var o = $.extend(true, {}, this);
    //              grid.formatRecord(o, true);
    //              data.push(o);
    //          });
    //          var unSelected = [];
    //          $(lastSelected).each(function (i, item)
    //          {
    //              if (!exist(item[p.valueField], selected) && exist(item[p.valueField], grid.rows))
    //              {
    //                  unSelected.push(item);
    //              }
    //          });
    //          var removeValue = [], removeText = [], removeData = [];
    //          $(unSelected).each(function (i, rowdata)
    //          {
    //              p.valueField && removeValue.push(rowdata[p.valueField]);
    //              p.textField && removeText.push(rowdata[p.textField]);
    //              var o = $.extend(true, {}, this);
    //              grid.formatRecord(o, true);
    //              removeData.push(o);
    //          });
    //          p.onSelect({
    //              value: value.join(p.split),
    //              text: text.join(p.split),
    //              data: data,
    //              remvoeValue: removeValue.join(p.split),
    //              remvoeText: removeText.join(p.split),
    //              removeData: removeData
    //          });
    //      }
    //  };

})(jQuery);
/**
* LeeUI-Radio
* 
*/

(function ($) {

    $.fn.leeRadio = function () {
        return $.leeUI.run.call(this, "leeUIRadio", arguments);
    };

    $.fn.leeUIGetRadioManager = function () {
        return $.leeUI.run.call(this, "leeUIGetRadioManager", arguments);
    };

    $.leeUIDefaults.Radio = { disabled: false };

    $.leeUI.controls.Radio = function (element, options) {
        $.leeUI.controls.Radio.base.constructor.call(this, element, options);
    };
    $.leeUI.controls.Radio.leeExtend($.leeUI.controls.Input, {
        __getType: function () {
            return 'Radio';
        },
        __idPrev: function () {
            return 'Radio';
        },
        _extendMethods: function () {
            return [];
        },
        _render: function () {

            var g = this, p = this.options;
            g.input = $(this.element);
            g.link = $('<a href="javascript:void(0)" class="lee-radio"></a>');
            g.wrapper = g.input.addClass('lee-hidden').css("display", "none").wrap('<div class="lee-radio-wrapper"></div>').parent();
            g.wrapper.prepend(g.link);
            g.input.change(function () {
                if (this.checked) {
                    g.link.addClass('lee-radio-checked');
                }
                else {
                    g.link.removeClass('lee-radio-checked');
                }
                return true;
            });
            g.link.click(function () {
                g._doclick();
            });
            g.wrapper.hover(function () {
                if (!p.disabled)
                    $(this).addClass("lee-over");
            }, function () {
                $(this).removeClass("lee-over");
            });
            this.element.checked && g.link.addClass('lee-radio-checked');

            if (this.element.id) {
                $("label[for=" + this.element.id + "]").click(function () {
                    g._doclick();
                });
            }
            g.set(p);
        },
        setValue: function (value) {
            var g = this, p = this.options;
            if (!value) {
                g.input[0].checked = false;
                g.link.removeClass('lee-radio-checked');
            }
            else {
                g.input[0].checked = true;
                g.link.addClass('lee-radio-checked');
            }
        },
        getValue: function () {
            return this.input[0].checked;
        },
        setEnabled: function () {
            this.input.attr('disabled', false);
            this.wrapper.removeClass("l-disabled");
            this.options.disabled = false;
        },
        setDisabled: function () {
            this.input.attr('disabled', true);
            this.wrapper.addClass("l-disabled");
            this.options.disabled = true;
        },
        updateStyle: function () {
            if (this.input.attr('disabled')) {
                this.wrapper.addClass("l-disabled");
                this.options.disabled = true;
            }
            if (this.input[0].checked) {
                this.link.addClass('lee-checkbox-checked');
            }
            else {
                this.link.removeClass('lee-checkbox-checked');
            }
        },
        _doclick: function () {
            var g = this, p = this.options;
            if (g.input.attr('disabled')) { return false; }
            g.input.trigger('click').trigger('change');
            var formEle;
            if (g.input[0].form) formEle = g.input[0].form;
            else formEle = document;
            $("input:radio[name=" + g.input[0].name + "]", formEle).not(g.input).trigger("change");
            return false;
        }
    });


})(jQuery);
/**
* jQuery leeUI 1.3.3
* 
* 
* 
*/
(function ($) {

    $.fn.leeRadioList = function (options) {
        return $.leeUI.run.call(this, "leeUIRadioList", arguments);
    };

    $.leeUIDefaults.RadioList = {
        rowSize: 4,            //每行显示元素数   
        valueField: 'id',       //值成员
        textField: 'text',      //显示成员 
        valueFieldID: null,      //隐藏域
        name: null,            //表单名 
        data: null,             //数据  
        parms: null,            //ajax提交表单 
        url: null,              //数据源URL(需返回JSON)
        urlParms: null,                     //url带参数
        ajaxContentType: null,
        ajaxType: 'post',
        onSuccess: null,
        onError: null,
        onSelect: null,
        css: null,               //附加css  
        value: null,            //值 
        valueFieldCssClass: null
    };


    $.leeUI.controls.RadioList = function (element, options) {
        $.leeUI.controls.RadioList.base.constructor.call(this, element, options);
    };
    $.leeUI.controls.RadioList.leeExtend($.leeUI.controls.Input, {
        __getType: function () {
            return 'RadioList';
        },
        _extendMethods: function () {
            return [];
        },
        _init: function () {
            $.leeUI.controls.RadioList.base._init.call(this);
        },
        _render: function () {
            var g = this, p = this.options;
            g.data = p.data;
            g.valueField = null; //隐藏域(保存值) 

            if ($(this.element).is(":hidden") || $(this.element).is(":text")) {
                g.valueField = $(this.element);
                if ($(this.element).is(":text")) {
                    g.valueField.hide();
                }
            }
            else if (p.valueFieldID) {
                g.valueField = $("#" + p.valueFieldID + ":input,[name=" + p.valueFieldID + "]:input");
                if (g.valueField.length == 0) g.valueField = $('<input type="hidden"/>');
                g.valueField[0].id = g.valueField[0].name = p.valueFieldID;
            }
            else {
                g.valueField = $('<input type="hidden"/>');
                g.valueField[0].id = g.valueField[0].name = g.id + "_val";
            }
            if (g.valueField[0].name == null) g.valueField[0].name = g.valueField[0].id;
            if (p.valueFieldCssClass) {
                g.valueField.addClass(p.valueFieldCssClass);
            }
            g.valueField.attr("data-leeid", g.id);


            if ($(this.element).is(":hidden") || $(this.element).is(":text")) {
                g.radioList = $('<div></div>').insertBefore(this.element);
            } else {
                g.radioList = $(this.element);
            }
            g.radioList.html('<div class="lee-radiolist-inner"><table cellpadding="0" cellspacing="0" border="0" class="lee-radiolist-table"></table></div>').addClass("lee-radiolist").append(g.valueField);
            g.radioList.table = $("table:first", g.radioList);


            p.value = g.valueField.val() || p.value;

            g.set(p);

            g._addClickEven();
        },
        destroy: function () {
            if (this.radioList) this.radioList.remove();
            this.options = null;
            $.leeUI.remove(this);
        },
        clear: function () {
            this._changeValue("");
            this.trigger('clear');
        },
        _setCss: function (css) {
            if (css) {
                this.radioList.addClass(css);
            }
        },
        _setDisabled: function (value) {
            //禁用样式
            if (value) {
                this.radioList.addClass('lee-radiolist-disabled');
                $("input:radio", this.radioList).attr("disabled", true);
            } else {
                this.radioList.removeClass('lee-radiolist-disabled');
                $("input:radio", this.radioList).removeAttr("disabled");
            }
        },
        _setRequired: function (flag) {
        },
        _setWidth: function (value) {
            this.radioList.width(value);
        },
        _setHeight: function (value) {
            this.radioList.height(value);
        },
        indexOf: function (item) {
            var g = this, p = this.options;
            if (!g.data) return -1;
            for (var i = 0, l = g.data.length; i < l; i++) {
                if (typeof (item) == "object") {
                    if (g.data[i] == item) return i;
                } else {
                    if (g.data[i][p.valueField].toString() == item.toString()) return i;
                }
            }
            return -1;
        },
        removeItems: function (items) {
            var g = this;
            if (!g.data) return;
            $(items).each(function (i, item) {
                var index = g.indexOf(item);
                if (index == -1) return;
                g.data.splice(index, 1);
            });
            g.refresh();
        },
        removeItem: function (item) {
            if (!this.data) return;
            var index = this.indexOf(item);
            if (index == -1) return;
            this.data.splice(index, 1);
            this.refresh();
        },
        insertItem: function (item, index) {
            var g = this;
            if (!g.data) g.data = [];
            g.data.splice(index, 0, item);
            g.refresh();
        },
        addItems: function (items) {
            var g = this;
            if (!g.data) g.data = [];
            $(items).each(function (i, item) {
                g.data.push(item);
            });
            g.refresh();
        },
        addItem: function (item) {
            var g = this;
            if (!g.data) g.data = [];
            g.data.push(item);
            g.refresh();
        },
        _setValue: function (value) {
            var g = this, p = this.options;
            g.valueField.val(value);
            p.value = value;
            this._dataInit();
        },
        setValue: function (value) {
            this._setValue(value);
        },
        _setUrl: function (url) {
            var g = this, p = this.options;
            if (!url) return;
            var urlParms = $.isFunction(p.urlParms) ? p.urlParms.call(g) : p.urlParms;
            if (urlParms) {
                for (name in urlParms) {
                    url += url.indexOf('?') == -1 ? "?" : "&";
                    url += name + "=" + urlParms[name];
                }
            }
            var parms = $.isFunction(p.parms) ? p.parms() : p.parms;
            if (p.ajaxContentType == "application/json" && typeof (parms) != "string") {
                parms = liger.toJSON(parms);
            }
            $.ajax({
                type: 'post',
                url: url,
                data: parms,
                cache: false,
                dataType: 'json',
                contentType: p.ajaxContentType,
                success: function (data) {
                    g.setData(data);
                    g.trigger('success', [data]);
                },
                error: function (XMLHttpRequest, textStatus) {
                    g.trigger('error', [XMLHttpRequest, textStatus]);
                }
            });
        },
        setUrl: function (url) {
            return this._setUrl(url);
        },
        setParm: function (name, value) {
            if (!name) return;
            var g = this;
            var parms = g.get('parms');
            if (!parms) parms = {};
            parms[name] = value;
            g.set('parms', parms);
        },
        clearContent: function () {
            var g = this, p = this.options;
            $("table", g.radioList).html("");
        },
        _setData: function (data) {
            this.setData(data);
        },
        setData: function (data) {
            var g = this, p = this.options;
            if (!data || !data.length) return;
            g.data = data;
            g.refresh();
            g.updateStyle();
        },
        refresh: function () {
            var g = this, p = this.options, data = this.data;
            this.clearContent();
            if (!data) return;
            var out = [], rowSize = p.rowSize, appendRowStart = false, name = p.name || g.id;
            for (var i = 0; i < data.length; i++) {
                var val = data[i][p.valueField], txt = data[i][p.textField], id = g.id + "-" + i;
                var newRow = i % rowSize == 0;
                //0,5,10
                if (newRow) {
                    if (appendRowStart) out.push('</tr>');
                    out.push("<tr>");
                    appendRowStart = true;
                }
                out.push("<td><input type='radio' name='" + name + "' value='" + val + "' id='" + id + "'/><label for='" + id + "'>" + txt + "</label></td>");
            }
            if (appendRowStart) out.push('</tr>');
            g.radioList.table.append(out.join(''));
            $("input[type='radio']", g.radioList).leeRadio();
        },
        _getValue: function () {
            var g = this, p = this.options, name = p.name || g.id;
            return $('input:radio[name="' + name + '"]:checked').val();
        },
        getValue: function () {
            var val = this._getValue();
            if (!val) val = "";
            //获取值
            return val;
        },
        updateStyle: function () {
            var g = this, p = this.options;
            g._dataInit();
            $(":radio", g.radioList).change(function () {
                var value = g.getValue();
                g.trigger('select', [{
                    value: value
                }]);
            });
        },
        _dataInit: function () {
            var g = this, p = this.options, name = p.name || g.id;
            var value = g.valueField.val() || g._getValue() || p.value;
            //g._changeValue(value);

            $("input:radio[name='" + name + "']", g.radioList).each(function () {
                $(this).leeUI().setValue(this.checked = this.value == value);
            });
        },
        //设置值到 隐藏域
        _changeValue: function (newValue) {
            var g = this, p = this.options, name = p.name || g.id;
            $("input:radio[name='" + name + "']", g.radioList).each(function () {
                this.checked = this.value == newValue;
            });
            g.valueField.val(newValue);
            g.selectedValue = newValue;
            g.trigger('changeValue', [newValue]);
            //g.element.trigger('change');
        },
        _addClickEven: function () {
            var g = this, p = this.options;
            //选项点击
            g.radioList.click(function (e) {
                var value = g.getValue();
                if (value) {

                    g.trigger('changeValue', [value]);
                    g.valueField.val(value);
                }
            });
        }
    });


})(jQuery);

(function ($) {
    $.fn.leeSpinner = function () {
        return $.leeUI.run.call(this, "leeUISpinner", arguments);
    };
    $.fn.leeGetSpinnerManager = function () {
        return $.leeUI.run.call(this, "leeUIGetSpinnerManager", arguments);
    };

    $.leeUIDefaults.Spinner = {
        type: 'int',     //类型 float:浮点数 int:整数 time:时间
        isNegative: true, //是否负数
        decimalplace: 2,   //小数位 type=float时起作用
        step: 0.1,         //每次增加的值
        interval: 50,      //间隔，毫秒
        value: null,
        onChangeValue: false,    //改变值事件
        minValue: null,        //最小值
        maxValue: null,         //最大值
        disabled: false,
        inline: false,
        readonly: false              //是否只读
    };

    $.leeUI.controls.Spinner = function (element, options) {
        $.leeUI.controls.Spinner.base.constructor.call(this, element, options);
    };
    $.leeUI.controls.Spinner.leeExtend($.leeUI.controls.Input, {
        __getType: function () {
            return 'Spinner';
        },
        __idPrev: function () {
            return 'Spinner';
        },
        _extendMethods: function () {
            return {};
        },
        _init: function () {
            $.leeUI.controls.Spinner.base._init.call(this);
            var p = this.options;
            if (p.type == 'float') {
                //p.step = 0.1;
                p.interval = 50;
            } else if (p.type == 'int') {
                p.step = 1;
                p.interval = 100;
            } else if (p.type == 'time') {
                p.step = 1;
                p.interval = 100;
            } else {
                p.type = "int";
                p.step = 1;
                p.interval = 100;
            }
        },
        _render: function () {
            var g = this, p = this.options;
            g.interval = null;
            g.inputText = null;
            g.value = null;
            g.textFieldID = "";
            if (this.element.tagName.toLowerCase() == "input" && this.element.type && this.element.type == "text") {
                g.inputText = $(this.element);
                if (this.element.id)
                    g.textFieldID = this.element.id;
            }
            else {
                g.inputText = $('<input type="text"/>');
                g.inputText.appendTo($(this.element));
            }
            if (g.textFieldID == "" && p.textFieldID)
                g.textFieldID = p.textFieldID;

            g.link = $('<div class="lee-right"><div class="lee-spinner-up lee-icon lee-angle-up"></div><div class="lee-spinner-split"></div><div class="lee-spinner-down  lee-icon lee-angle-down"></div></div>');
            g.wrapper = g.inputText.wrap('<div class="lee-text lee-text-spinner"></div>').parent();
            //g.wrapper.append('<div class="lee-text-l"></div><div class="lee-text-r"></div>');
            g.wrapper.append(g.link).after(g.selectBox).after(g.valueField);
            g.link.up = $(".lee-spinner-up", g.link);
            g.link.down = $(".lee-spinner-down", g.link);
            g.inputText.addClass("lee-text-field");

            if (p.disabled) {
                g.wrapper.addClass("lee-text-disabled");
            }
            //初始化
            if (!g._isVerify(g.inputText.val())) {
                g.value = g._getDefaultValue();
                g._showValue(g.value);
            }

            g.inputText.on('keydown.spinner', function (e) {
                var dir = {
                    38: 'up',
                    40: 'down'
                }[e.which];
                if (p.disabled) return;
                if (e.which == 38) {
                    g._uping.call(g);

                    //不让选中？
                    $(document).bind("selectstart.spinner", function () { return false; });

                } else if (e.which == 40) {
                    g._downing.call(g);
                    $(document).bind("selectstart.spinner", function () { return false; });
                }

            }).on("keyup.spinner", function (e) {

                g.inputText.trigger("change").focus();
            });
            //事件
            g.link.up.hover(function () {
                if (!p.disabled)
                    $(this).addClass("lee-spinner-up-over");
            }, function () {
                clearInterval(g.interval);
                $(document).unbind("selectstart.spinner");
                $(this).removeClass("lee-spinner-up-over");
            }).mousedown(function () {
                if (!p.disabled) {
                    g._uping.call(g);
                    g.interval = setInterval(function () {
                        g._uping.call(g);
                    }, p.interval);
                    //不让选中？
                    $(document).bind("selectstart.spinner", function () { return false; });
                }
            }).mouseup(function () {
                clearInterval(g.interval);
                g.inputText.trigger("change").focus();
                $(document).unbind("selectstart.spinner");
            });
            g.link.down.hover(function () {
                if (!p.disabled)
                    $(this).addClass("lee-spinner-down-over");
            }, function () {
                clearInterval(g.interval);
                $(document).unbind("selectstart.spinner");
                $(this).removeClass("lee-spinner-down-over");
            }).mousedown(function () {
                if (!p.disabled) {
                    g.interval = setInterval(function () {
                        g._downing.call(g);
                    }, p.interval);
                    $(document).bind("selectstart.spinner", function () { return false; });
                }
            }).mouseup(function () {
                clearInterval(g.interval);
                g.inputText.trigger("change").focus();
                $(document).unbind("selectstart.spinner");
            });

            g.inputText.change(function () {
                var value = g.inputText.val();
                g.value = g._getVerifyValue(value);
                g.trigger('changeValue', [g.value]);
                // 这里需要出发
                g._showValue(g.value);
            }).blur(function () {
                g.wrapper.removeClass("lee-text-focus");
            }).focus(function () {
                g.wrapper.addClass("lee-text-focus");
            });
            g.wrapper.hover(function () {
                if (!p.disabled)
                    g.wrapper.addClass("lee-text-over");
            }, function () {
                g.wrapper.removeClass("lee-text-over");
            });
            g.set(p);
        },
        _setValue: function (value) {
            if (value != null)
                this.inputText.val(value);
        },
        _setWidth: function (value) {
            var g = this;
            if (value > 20) {
                //g.wrapper.css({ width: value });
                //g.inputText.css({ width: value});
            }
        },
        _setInline: function (value) {
            var g = this;
            if (value) {

                g.wrapper.css({ "display": "inline-block" });
            }
        },
        _setHeight: function (value) {
            var g = this;
            if (value > 10) {
                g.wrapper.height(value);
                g.inputText.height(value - 2);
                g.link.height(value - 4);
            }
        },
        _setDisabled: function (value) {
            var g = this, p = this.options;
            p.disabled = value ? true : false;
            if (value) {

                this.inputText.attr("readonly", "readonly");
                this.wrapper.addClass("lee-text-disabled");
            }
            else {
                this.inputText.removeAttr("readonly");
                this.wrapper.removeClass("lee-text-disabled");
            }
        },
        _showValue: function (value) {
            var g = this, p = this.options;
            if (!value || value == "NaN") value = 0;
            if (p.type == 'float') {
                value = parseFloat(value).toFixed(p.decimalplace);
            }
            this.inputText.val(value)
        },
        _setValue: function (value) {
            this._showValue(value);
        },
        setValue: function (value) {
            this._showValue(value);
        },
        getValue: function () {
            return this.inputText.val();
        },
        _round: function (v, e) {
            var g = this, p = this.options;
            var t = 1;
            for (; e > 0; t *= 10, e--) { }
            for (; e < 0; t /= 10, e++) { }
            return Math.round(v * t) / t;
        },
        _isInt: function (str) {
            var g = this, p = this.options;
            var strP = p.isNegative ? /^-?\d+$/ : /^\d+$/;
            if (!strP.test(str)) return false;
            if (parseFloat(str) != str) return false;
            return true;
        },
        _isFloat: function (str) {
            var g = this, p = this.options;
            var strP = p.isNegative ? /^-?\d+(\.\d+)?$/ : /^\d+(\.\d+)?$/;
            if (!strP.test(str)) return false;
            if (parseFloat(str) != str) return false;
            return true;
        },
        _isTime: function (str) {
            var g = this, p = this.options;
            var a = str.match(/^(\d{1,2}):(\d{1,2})$/);
            if (a == null) return false;
            if (a[1] > 24 || a[2] > 60) return false;
            return true;

        },
        _isVerify: function (str) {
            var g = this, p = this.options;
            if (p.type == 'float') {
                if (!g._isFloat(str)) return false;
                var value = parseFloat(str);
                if (p.minValue != undefined && p.minValue > value) return false;
                if (p.maxValue != undefined && p.maxValue < value) return false;
                return true;
            } else if (p.type == 'int') {
                if (!g._isInt(str)) return false;
                var value = parseInt(str);
                if (p.minValue != undefined && p.minValue > value) return false;
                if (p.maxValue != undefined && p.maxValue < value) return false;
                return true;
            } else if (p.type == 'time') {
                return g._isTime(str);
            }
            return false;
        },
        _getVerifyValue: function (value) {
            var g = this, p = this.options;
            var newvalue = null;
            if (p.type == 'float') {
                newvalue = g._round(value, p.decimalplace);
            }
            else if (p.type == 'int') {
                newvalue = parseInt(value);
            } else if (p.type == 'time') {
                newvalue = value;
            }
            if (!g._isVerify(newvalue)) {
                return g.value;
            } else {
                return newvalue;
            }
        },
        _isOverValue: function (value) {
            var g = this, p = this.options;
            if (p.minValue != null && p.minValue > value) return true;
            if (p.maxValue != null && p.maxValue < value) return true;
            return false;
        },
        _getDefaultValue: function () {
            var g = this, p = this.options;
            if (p.type == 'float' || p.type == 'int') { return 0; }
            else if (p.type == 'time') { return "00:00"; }
        },
        _addValue: function (num) {
            var g = this, p = this.options;
            var value = g.inputText.val();
            value = parseFloat(value) + num;
            if (g._isOverValue(value)) return;
            g._showValue(value);
            g.inputText.trigger("change");
        },
        _addTime: function (minute) {
            var g = this, p = this.options;
            var value = g.inputText.val();
            var a = value.match(/^(\d{1,2}):(\d{1,2})$/);
            newminute = parseInt(a[2]) + minute;
            if (newminute < 10) newminute = "0" + newminute;
            value = a[1] + ":" + newminute;
            if (g._isOverValue(value)) return;
            g._showValue(value);
            g.inputText.trigger("change");
        },
        _uping: function () {
            var g = this, p = this.options;
            if (p.type == 'float' || p.type == 'int') {
                g._addValue(p.step);
            } else if (p.type == 'time') {
                g._addTime(p.step);
            }
        },
        _downing: function () {
            var g = this, p = this.options;
            if (p.type == 'float' || p.type == 'int') {
                g._addValue(-1 * p.step);
            } else if (p.type == 'time') {
                g._addTime(-1 * p.step);
            }
        },
        _isDateTime: function (dateStr) {
            var g = this, p = this.options;
            var r = dateStr.match(/^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2})$/);
            if (r == null) return false;
            var d = new Date(r[1], r[3] - 1, r[4]);
            if (d == "NaN") return false;
            return (d.getFullYear() == r[1] && (d.getMonth() + 1) == r[3] && d.getDate() == r[4]);
        },
        _isLongDateTime: function (dateStr) {
            var g = this, p = this.options;
            var reg = /^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2}) (\d{1,2}):(\d{1,2})$/;
            var r = dateStr.match(reg);
            if (r == null) return false;
            var d = new Date(r[1], r[3] - 1, r[4], r[5], r[6]);
            if (d == "NaN") return false;
            return (d.getFullYear() == r[1] && (d.getMonth() + 1) == r[3] && d.getDate() == r[4] && d.getHours() == r[5] && d.getMinutes() == r[6]);
        }
    });


})(jQuery);
$.leeUIDefaults.Grid = {
    title: null,
    width: 'auto',
    height: 'auto',
    columnWidth: null, //默认列宽度
    resizable: true, //table是否可改变宽度
    url: false, //动态取数url
    urlParms: null, //url参数信息
    data: null, //本地存储数据
    usePager: true, //是否分页
    hideLoadButton: false, //是否隐藏刷新按钮
    pagerRender: null, //分页栏自定义渲染函数
    page: 1, //默认当前页 
    pageSize: 100, //每页默认的结果数
    pageSizeOptions: [10, 20, 30, 40, 50], //可选择设定的每页结果数
    parms: [], //提交到服务器的参数 
    columns: [], //列配置信息
    minColToggle: 1, //最小显示的列
    dataType: 'server', //数据源：本地(local)或(server),本地是将读取p.data。不需要配置，取决于设置了data或是url
    dataAction: 'server', //提交数据的方式：本地(local)或(server),选择本地方式时将在客服端分页、排序。 
    showTableToggleBtn: false, //是否显示'显示隐藏Grid'按钮 
    switchPageSizeApplyComboBox: true, //切换每页记录数是否应用ligerComboBox
    allowAdjustColWidth: true, //是否允许调整列宽     
    checkbox: false, //是否显示复选框
    isSingleCheck: false, //复选框选择的时候是否单选模式 方便单选
    allowHideColumn: true, //是否显示'切换列层'按钮
    enabledEdit: false, //是否允许编辑
    isScroll: true, //是否滚动 
    dateFormat: 'yyyy-MM-dd', //默认时间显示格式？
    inWindow: true, //是否以窗口的高度为准 height设置为百分比时可用
    statusName: '__status', //状态名
    method: 'post', //获取数据http方式
    async: true,
    fixedCellHeight: true, //是否固定单元格的高度
    heightDiff: 0, //高度补差,当设置height:100%时，可能会有高度的误差，可以通过这个属性调整
    cssClass: null, //类名
    noborder: null,//不现实边框
    root: 'Rows', //数据源字段名
    record: 'Total', //数据源记录数字段名
    pageParmName: 'page', //页索引参数名，(提交给服务器)
    pagesizeParmName: 'pagesize', //页记录数参数名，(提交给服务器)
    sortnameParmName: 'sortname', //页排序列名(提交给服务器)
    sortorderParmName: 'sortorder', //页排序方向(提交给服务器) 
    allowUnSelectRow: false, //是否允许反选行 
    alternatingRow: false, //奇偶行效果
    mouseoverRowCssClass: 'lee-grid-row-over', //鼠标略过css样式
    enabledSort: true, //是否允许排序
    rowClsRender: null, //行自定义css class渲染器
    rowAttrRender: null, //行自定义属性渲染器(包括style，也可以定义)
    groupColumnName: null, //分组 - 列名
    groupColumnDisplay: '分组', //分组 - 列显示名字
    groupRender: null, //分组 - 渲染器
    totalRender: null, //统计行(全部数据)
    delayLoad: false, //初始化时是否不加载
    where: null, //数据过滤查询函数,(参数一 data item，参数二 data item index)
    selectRowButtonOnly: false, //复选框模式时，是否只允许点击复选框才能选择行 
    selectable: true,
    whenRClickToSelect: false, //右击行时是否选中
    contentType: null, //Ajax contentType参数  
    checkboxColWidth: 27, //复选框列宽度
    detailColWidth: 29, //明细列宽度
    clickToEdit: true, //是否点击单元格的时候就编辑
    detailToEdit: false, //是否点击明细的时候进入编辑
    onEndEdit: null,
    minColumnWidth: 80,
    tree: null, //treeGrid模式
    isChecked: null, //复选框 初始化函数
    isSelected: null, //选择 初始化函数
    frozen: true, //是否固定列
    frozenDetail: false, //明细按钮是否在固定列中
    frozenCheckbox: true, //复选框按钮是否在固定列中
    detail: null,
    detailHeight: 260,
    isShowDetailToggle: null, //是否显示展开/收缩明细的判断函数
    rownumbers: false, //是否显示行序号
    frozenRownumbers: true, //行序号是否在固定列中
    rownumbersColWidth: 26, //序号列宽度  
    colDraggable: false, //是否允许表头拖拽
    rowDraggable: false, //是否允许行拖拽
    rowDraggingRender: null, //拖拽渲染函数
    autoCheckChildren: true, //是否自动选中子节点
    onRowDragDrop: null, //行拖拽事件
    rowHeight: 28, //行默认的高度
    headerRowHeight: 36, //表头行的高度
    minHeight: 63,
    toolbar: null, //工具条,参数同 LeeToolBar,额外参数有title、icon
    toolbarShowInLeft: true, //工具条显示在左边
    headerImg: null, //表格头部图标  
    editorTopDiff: 0, //编辑器top误差
    editorLeftDiff: 1, //编辑器left误差
    editorHeightDiff: -1, //编辑器高度误差
    unSetValidateAttr: true, //是否不设置validate属性到inuput nouse
    autoFilter: false, //自动生成高级查询, 需要filter/toolbar组件支持. 需要引用skins/ligerui-icons.css nouse
    rowSelectable: true, //是否允许选择
    scrollToPage: false, //滚动时分页
    scrollToAppend: true, //滚动时分页 是否追加分页的形式
    noDataRender: null, //没有数据时候的提示
    /*事件接口方法*/
    onDragCol: null, //拖动列事件
    onToggleCol: null, //切换列事件
    onChangeSort: null, //改变排序事件
    onSuccess: null, //成功获取服务器数据的事件
    onDblClickRow: null, //双击行事件
    onSelectRow: null, //选择行事件
    onBeforeSelectRow: null, //选择前事件
    onUnSelectRow: null, //取消选择行事件
    onBeforeCheckRow: null, //选择前事件，可以通过return false阻止操作(复选框)
    onCheckRow: null, //选择事件(复选框)  
    onBeforeCheckAllRow: null, //选择前事件，可以通过return false阻止操作(复选框 全选/全不选)
    onCheckAllRow: null, //选择事件(复选框 全选/全不选)onextend
    onBeforeShowData: null, //显示数据前事件，可以通过reutrn false阻止操作
    onAfterShowData: null, //显示完数据事件
    onError: null, //错误事件
    onSubmit: null, //提交前事件
    onReload: null, //刷新事件，可以通过return false来阻止操作
    onToFirst: null, //第一页，可以通过return false来阻止操作
    onToPrev: null, //上一页，可以通过return false来阻止操作
    onToNext: null, //下一页，可以通过return false来阻止操作
    onToLast: null, //最后一页，可以通过return false来阻止操作
    onAfterAddRow: null, //增加行后事件
    onBeforeEdit: null, //编辑前事件
    onBeforeSubmitEdit: null, //验证编辑器结果是否通过
    onAfterEdit: null, //结束编辑后事件
    onLoading: null, //加载时函数
    onLoaded: null, //加载完函数
    onContextmenu: null, //右击事件
    onBeforeCancelEdit: null, //取消编辑前事件
    onAfterSubmitEdit: null, //提交后事件
    onRowDragDrop: null, //行拖拽后事件
    onGroupExtend: null, //分组展开事件
    onGroupCollapse: null, //分组收缩事件
    onTreeExpand: null, //树展开事件
    onTreeCollapse: null, //树收缩事件
    onTreeExpanded: null, //树展开事件
    onTreeCollapsed: null, //树收缩事件
    onLoadData: null, //加载数据前事件 
    onHeaderCellBulid: null,
    showHeaderFilter: false,
    filterRowHeight: 29,
    virtualScroll: false,
    autoColWidth: false
};

$.leeUIDefaults.GridString = {
    errorMessage: '发生错误',
    pageStatMessage: '共{total}条记录  ， 显示第 {from} 条- 第{to} 条 ',
    pageTextMessage: 'Page',
    loadingMessage: '正在加载中...',
    findTextMessage: '查找',
    noRecordMessage: '没有符合条件的记录存在',
    isContinueByDataChanged: '数据已经改变,如果继续将丢失数据,是否继续?',
    cancelMessage: '取消',
    saveMessage: '保存',
    applyMessage: '应用',
    draggingMessage: '{count}行'
};


//排序器扩展
$.leeUIDefaults.Grid.sorters = $.leeUIDefaults.Grid.sorters || {};

//格式化器扩展
$.leeUIDefaults.Grid.formatters = $.leeUIDefaults.Grid.formatters || {};

//编辑器扩展
$.leeUIDefaults.Grid.editors = $.leeUIDefaults.Grid.editors || {};

$.leeUIDefaults.Grid.sorters['date'] = function (val1, val2) {
    return val1 < val2 ? -1 : val1 > val2 ? 1 : 0;
};
$.leeUIDefaults.Grid.sorters['int'] = function (val1, val2) {
    return parseInt(val1) < parseInt(val2) ? -1 : parseInt(val1) > parseInt(val2) ? 1 : 0;
};
$.leeUIDefaults.Grid.sorters['float'] = function (val1, val2) {
    return parseFloat(val1) < parseFloat(val2) ? -1 : parseFloat(val1) > parseFloat(val2) ? 1 : 0;
};
$.leeUIDefaults.Grid.sorters['string'] = function (val1, val2) {
    if (!val1) return false;
    return val1.localeCompare(val2);
};

$.leeUIDefaults.Grid.formatters['date'] = function (value, column) {
    function getFormatDate(date, dateformat) {
        var g = this,
            p = this.options;
        if (isNaN(date)) return null;
        var format = dateformat;
        var o = {
            "M+": date.getMonth() + 1,
            "d+": date.getDate(),
            "h+": date.getHours(),
            "m+": date.getMinutes(),
            "s+": date.getSeconds(),
            "q+": Math.floor((date.getMonth() + 3) / 3),
            "S": date.getMilliseconds()
        }
        if (/(y+)/.test(format)) {
            format = format.replace(RegExp.$1, (date.getFullYear() + "")
                .substr(4 - RegExp.$1.length));
        }
        for (var k in o) {
            if (new RegExp("(" + k + ")").test(format)) {
                format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] :
                    ("00" + o[k]).substr(("" + o[k]).length));
            }
        }
        return format;
    }
    if (!value) return "";
    // /Date(1328423451489)/
    if (typeof (value) == "string" && /^\/Date/.test(value)) {
        value = value.replace(/^\//, "new ").replace(/\/$/, "");
        eval("value = " + value);
    }
    if (value instanceof Date) {
        var format = column.format || this.options.dateFormat || "yyyy-MM-dd";
        return getFormatDate(value, format);
    } else {
        return value.toString();
    }
}

//引用类型,数据形式表现为[id,text] 
$.leeUIDefaults.Grid.formatters['ref'] = function (value) {
    if ($.isArray(value)) return value.length > 1 ? value[1] : value[0];
    return value;
};
(function ($) {
    $.fn.leeGrid = function (options) {
        return $.leeUI.run.call(this, "leeUIGrid", arguments);
    };
    $.fn.leeGridInstace = function () {
        //获取控件实例
        return $.leeUI.run.call(this, "leeUIGetGridManager", arguments);
    };

    $.leeUI.controls.Grid = function (element, options) {
        $.leeUI.controls.Grid.base.constructor.call(this, element, options);
    };

    $.leeUI.controls.Grid.leeExtend($.leeUI.core.UIComponent, {
        __getType: function () {
            return 'Grid';
        },
        __idPrev: function () {
            return 'grid';
        },
        _extendMethods: function () {
            return {};
        },
        _init: function () {

            //基类构造函数
            $.leeUI.controls.Grid.base._init.call(this);
            var g = this,
                p = this.options;
            p.dataType = p.url ? "server" : "local";
            if (p.dataType == "local") {
                p.data = p.data || [];
                p.dataAction = "local";
            }
            if (!p.frozen) {
                p.frozenCheckbox = false;
                p.frozenDetail = false;
                p.frozenRownumbers = false;
            }
            if (p.tree) //启用分页模式
            {
                p.tree.childrenName = p.tree.childrenName || "children";
                p.tree.isParent = p.tree.isParent || function (rowData) {
                    var exist = p.tree.childrenName in rowData;
                    return exist; //是否父节点
                };
                p.tree.isExtend = p.tree.isExtend || function (rowData) {

                    if ('isextend' in rowData && rowData['isextend'] == false)
                        return false;
                    return true;
                };
                //初始化编辑器
            }
        },
        _render: function () {
            var g = this,
                p = this.options;
            g.grid = $(g.element);
            g.grid.addClass("lee-ui-grid");
            if (p.autoColWidth) g.grid.addClass("lee-ui-grid-colfix");
            if (!p.fixedCellHeight)
                g.grid.addClass("lee-ui-grid-auto");
            var gridhtmlarr = [];
            gridhtmlarr.push("        <div class='lee-panel-header'><span class='lee-panel-header-text'></span></div>");
            gridhtmlarr.push("                    <div class='lee-grid-loading'></div>");
            gridhtmlarr.push("        <div class='lee-panel-toolbar' style='display:none'><div class='lee-grid-toolbar'></div></div><div class='lee-clear'></div>");
            gridhtmlarr.push("        <div class='lee-panel-bwarp'>");
            gridhtmlarr.push("            <div class='lee-panel-body'>");
            gridhtmlarr.push("                <div class='lee-grid'>");
            //拖动显示线
            gridhtmlarr.push("                    <div class='lee-grid-dragging-line'></div>");
            //右键菜单
            gridhtmlarr.push("                    <div class='lee-grid-popup'><table cellpadding='0' cellspacing='0'><tbody></tbody></table></div>");
            //左侧锁定区
            gridhtmlarr.push("                    <div class='lee-grid-left'>");
            gridhtmlarr.push("                        <div class='lee-grid-header lee-grid-header-left'>");
            gridhtmlarr.push("                          <div class='lee-grid-header-inner'><table class='lee-grid-header-table' cellpadding='0' cellspacing='0'><tbody></tbody></table></div>");
            gridhtmlarr.push("                        </div>");
            gridhtmlarr.push("                    <div class='lee-grid-body lee-grid-body-left'></div>");
            gridhtmlarr.push("                  </div>");

            //主滚动区
            gridhtmlarr.push("                  <div class='lee-grid-main'>");
            gridhtmlarr.push("                      <div class='lee-grid-header lee-grid-header-main'>");
            gridhtmlarr.push("                          <div class='lee-grid-header-inner'><table class='lee-grid-header-table' cellpadding='0' cellspacing='0'><tbody></tbody></table></div>");
            gridhtmlarr.push("                      </div>");
            gridhtmlarr.push("                      <div class='lee-grid-body lee-grid-body-main lee-grid-scroll'></div>");
            gridhtmlarr.push("                  </div>");

            //右侧锁定区
            gridhtmlarr.push("                  <div class='lee-grid-right'>");
            gridhtmlarr.push("                      <div class='lee-grid-header lee-grid-header-right'>");
            gridhtmlarr.push("                          <div class='lee-grid-header-inner'><table class='lee-grid-header-table' cellpadding='0' cellspacing='0'><tbody></tbody></table></div>");
            gridhtmlarr.push("                      </div>");
            gridhtmlarr.push("                      <div class='lee-grid-body lee-grid-body-right'></div>");
            gridhtmlarr.push("                  </div>");

            gridhtmlarr.push("                 </div>");
            gridhtmlarr.push("              </div>");
            gridhtmlarr.push("         </div>");

            //底部工具栏
            gridhtmlarr.push("         <div class='lee-panel-footer'>");
            gridhtmlarr.push("            <div class='lee-panel-footer-inner'>");
            gridhtmlarr.push("                <div class='group  lee-bar-message'><span class='lee-bar-text'></span></div>");
            gridhtmlarr.push("            <div class='group selectpagesize'></div>");
            gridhtmlarr.push("                <div class='separator'></div>");
            gridhtmlarr.push("                <div class='group prevgroup'>");
            gridhtmlarr.push("                    <a href='javascript:void(0)' class='lee-icon lee-bar-button lee-bar-btnfirst'><span class='lee-icon'></span></a>");
            gridhtmlarr.push("                    <a href='javascript:void(0)' class='lee-icon lee-bar-button lee-bar-btnprev'><span class='lee-icon'></span></a>");
            gridhtmlarr.push("                </div>");
            gridhtmlarr.push("                <div class='separator'></div>");
            gridhtmlarr.push("                <div class='group changepage'><span class='pcontrol'> <input type='text' size='4' value='1' style='width:20px' maxlength='3' /> / <span></span></span></div>");
            gridhtmlarr.push("                <div class='separator'></div>");
            gridhtmlarr.push("                <div class='group nextgroup'>");
            gridhtmlarr.push("                     <a href='javascript:void(0)' class='lee-bar-button lee-bar-btnnext'><span class='lee-icon'></span></a>");
            gridhtmlarr.push("                     <a href='javascript:void(0)' class='lee-icon lee-bar-button lee-bar-btnlast'><span class='lee-icon'></span></a>");
            gridhtmlarr.push("                </div>");
            gridhtmlarr.push("                <div class='separator'></div>");
            gridhtmlarr.push("                <div class='group refreshgroup'>");
            gridhtmlarr.push("                     <a href='javascript:void(0)' class='lee-icon lee-bar-button lee-bar-btnload'><span class='lee-icon'></span></a>");
            gridhtmlarr.push("                </div>");
            gridhtmlarr.push("                <div class='separator'></div>");

            gridhtmlarr.push("                <div class='lee-clear'></div>");
            gridhtmlarr.push("            </div>");
            gridhtmlarr.push("         </div>");

            g.grid.html(gridhtmlarr.join(''));

            //头部
            g.header = $(".lee-panel-header:first", g.grid);
            //主体
            g.body = $(".lee-panel-body:first", g.grid);
            //底部工具条         
            g.toolbar = $(".lee-panel-footer:first", g.grid);
            //右键菜单
            g.popup = $(".lee-grid-popup:first", g.grid);
            //进度条
            g.gridloading = $(".lee-grid-loading:first", g.grid);
            //调整列宽层 
            g.draggingline = $(".lee-grid-dragging-line", g.grid);
            //顶部工具栏
            g.topbar = $(".lee-grid-toolbar:first", g.grid);

            g.gridview = $(".lee-grid:first", g.grid);
            g.gridview.attr("id", g.id + "$grid");
            g.gridview1 = $(".lee-grid-left:first", g.gridview);
            g.gridview2 = $(".lee-grid-main:first", g.gridview);
            g.gridview3 = $(".lee-grid-right:first", g.gridview);
            g.gridview3.hide();
            //表头     
            g.gridheader = $(".lee-grid-header:first", g.gridview2);
            //表主体     
            g.gridbody = $(".lee-grid-body:first", g.gridview2);

            //frozen
            g.f = {};
            g.f.gridheader = $(".lee-grid-header:first", g.gridview1);
            //表主体     
            g.f.gridbody = $(".lee-grid-body:first", g.gridview1);

            g.currentData = null;
            g.changedCells = {};
            g.editors = {}; //多编辑器同时存在
            g.editor = {
                editing: false
            }; //单编辑器,配置clickToEdit
            g.cacheData = {}; //缓存数据

            if (p.height == "auto") {
                //绑定自动计算高度事件
                g.bind("SysGridHeightChanged", function () {
                    if (g.enabledFrozen()) {
                        g.gridview.height(Math.max(Math.max(g.gridview1.height() + 1, g.gridview2.height() + 1), p.minHeight));
                    }

                });
            }
            var pc = $.extend({}, p);
            this._bulid();
            this._setColumns(p.columns); //设置列
            delete pc['columns'];
            delete pc['data'];
            delete pc['url']; //清空这些属性？ 为啥
            g.set(pc); //重新赋值
            if (!p.delayLoad) {
                if (p.url)
                    g.set({
                        url: p.url
                    });
                else if (p.data)
                    g.set({
                        data: p.data
                    });
            }

            //编辑构造器初始化
            for (var type in leeUI.editors) {
                var editor = leeUI.editors[type];
                //如果没有默认的或者已经定义
                if (!editor || type in p.editors) continue;
                p.editors[type] = leeUI.getEditor($.extend({
                    type: type,
                    master: g
                }, editor));
            }

            if (p.virtualScroll) g.initScrollSetting();
        },
        _setFrozen: function (frozen) {
            if (frozen)
                this.grid.addClass("lee-frozen");
            else
                this.grid.removeClass("lee-frozen");
        },
        _setLoadingMessage: function (value) {
            this.gridloading.html("<div class='lee-grid-loading-inner'><div class='loader'></div><div class='message'>" + value + "</div></div>");
            //this.gridloading.html(value);
        },
        _setToolbar: function (value) {
            //设置工具栏信息
            var g = this,
                p = this.options;
            if (value && $.fn.leeToolBar) {
                g.topbar.parent().show();
                g.topbarManager = g.topbar.leeToolBar(value);

                if (p.toolbarShowInLeft) {
                    g.topbar.addClass("lee-grid-toolbar-left");
                }
            } else {
                g.topbar.parent().remove();
            }
        },
        isHorizontalScrollShowed: function () {
            //是否显示横向滚动条
            var g = this;

            if (g.gridbody.scrollLeft() > 0) return true; //如果有scrollTop
            g.gridbody.scrollLeft(1);
            var c = g.gridbody.scrollLeft();
            g.gridbody.scrollLeft(0);

            return c !== 0;
        },
        isVerticalScrollShowed: function () {
            var g = this;
            //var inner = g.gridbody.find(".lee-grid-body-inner:first");
            if (g.gridbody.scrollTop() > 0) return true; //如果有scrollTop
            g.gridbody.scrollTop(1);
            var c = g.gridbody.scrollTop();
            g.gridbody.scrollTop(0);

            return c !== 0;
            //if(!inner.length) return false;
            //20为横向滚动条的宽度
            //return g.gridbody.height() < inner.height();
        },
        _setHeight: function (h) {
            var g = this,
                p = this.options;
            g.unbind("SysGridHeightChanged"); //解绑事件
            if (h == "auto") {
                g.bind("SysGridHeightChanged", function () {
                    if (g.enabledFrozen()) {
                        g.gridview.height(Math.max(Math.max(g.gridview1.height() + 1, g.gridview2.height() + 1), p.minHeight));
                    }
                });
                return;
            }
            h = g._calculateGridBodyHeight(h); //计算高度
            if (h > 0) {
                g.gridbody.height(h);

                if (p.frozen) {
                    //解决冻结列和活动列由上至下滚动错位的问题
                    var w = g.gridbody.width(),
                        w2 = $(":first-child", g.gridbody).width();
                    if (w2 && (w2 + 18 > w)) {
                        if (h > 18)
                            g.f.gridbody.height(h - 18);
                    } else {
                        g.f.gridbody.height(h);
                    }
                }

                var gridHeaderHeight = p.headerRowHeight * (g._columnMaxLevel - 1) + p.headerRowHeight + 3;

                if (p.showHeaderFilter) {
                    gridHeaderHeight += p.filterRowHeight;
                }
                g.gridview.height(h + gridHeaderHeight);
            }
            g._updateHorizontalScrollStatus.leeDefer(g, 10); //延时执行
        },
        _calculateGridBodyHeight: function (h) {
            var g = this,
                p = this.options;
            if (typeof h == "string" && h.indexOf('%') > 0) {
                if (p.inWindow)
                    h = $(window).height() * parseInt(h) * 0.01;
                else
                    h = g.grid.parent().height() * parseInt(h) * 0.01;
            }
            if (p.title) h -= 24;
            if (p.usePager || (p.pagerRender && p.scrollToPage)) h -= g.toolbar.outerHeight();
            if (p.totalRender) h -= 25;
            if (p.toolbar) h -= g.topbar.outerHeight();
            //alert(g.topbar.outerHeight());
            var gridHeaderHeight = p.headerRowHeight * (g._columnMaxLevel - 1) + p.headerRowHeight + 3;
            if (p.showHeaderFilter) {
                gridHeaderHeight += p.filterRowHeight;
            }
            h -= (gridHeaderHeight || 0);


            return h;
        },
        _updateHorizontalScrollStatus: function () {
            var g = this,
                p = this.options;
            if (g.isHorizontalScrollShowed()) {
                g.gridview.addClass("lee-grid-hashorizontal");
            } else {
                g.gridview.removeClass("lee-grid-hashorizontal");
            }

            if (g.isVerticalScrollShowed()) {
                g.gridview.addClass("lee-grid-hasvertical");
            } else {
                g.gridview.removeClass("lee-grid-hasvertical");
            }
        },
        _updateFrozenWidth: function () {
            var g = this,
                p = this.options;
            if (g.enabledFrozen()) {
                g.gridview1.width(g.f.gridtablewidth); //固定区域宽度
                var view2width = g.gridview.width() - g.f.gridtablewidth;
                g.gridview2.css({
                    left: g.f.gridtablewidth
                }); //左侧偏移
                if (view2width > 0) g.gridview2.css({
                    width: view2width
                }); //高度
            }
        },
        _setWidth: function (value) {
            var g = this,
                p = this.options;
            if (g.enabledFrozen()) g._onResize();
        },
        _setUrl: function (value) {
            this.options.url = value;
            if (value) {
                this.options.dataType = "server";
                if (!this.options.empty)
                    this.loadData(true);
                else
                    this._onResize();
            } else {
                this.options.dataType = "local";
            }
        },
        removeParm: function (name) {
            var g = this;
            var parms = g.get('parms');
            if (!parms) parms = {};
            if (parms instanceof Array) {
                removeArrItem(parms, function (p) {
                    return p.name == name;
                });
            } else {
                delete parms[name];
            }
            g.set('parms', parms);
        },
        setParm: function (name, value) {
            var g = this;
            var parms = g.get('parms');
            if (!parms) parms = {};
            if (parms instanceof Array) {
                removeArrItem(parms, function (p) {
                    return p.name == name;
                });
                parms.push({
                    name: name,
                    value: value
                });
            } else {
                parms[name] = value;
            }
            g.set('parms', parms);
        },
        _setData: function (value) {
            this.loadData(this.options.data);
            this.trigger('afterSetData');
        },
        //刷新数据
        loadData: function (loadDataParm, sourceType) {
            var g = this,
                p = this.options;
            g.loading = true;
            g.trigger('loadData');
            var clause = null;
            var loadServer = true;
            if (typeof (loadDataParm) == "function") {
                clause = loadDataParm;
                if (g.lastData) {
                    g.data = g.lastData;
                } else {
                    g.data = g.currentData;
                    if (!g.data) g.data = {};
                    if (!g.data[p.root]) g.data[p.root] = [];
                    g.lastData = g.data;
                }
                loadServer = false;
            } else if (typeof (loadDataParm) == "boolean") {
                loadServer = loadDataParm;
            } else if (typeof (loadDataParm) == "object" && loadDataParm) {
                loadServer = false;
                p.dataType = "local";
                p.data = loadDataParm;
            } else if (typeof (loadDataParm) == "number") {
                p.newPage = loadDataParm;
            }
            //参数初始化
            if (!p.newPage) p.newPage = 1;
            if (p.dataAction == "server") {
                if (!p.sortOrder) p.sortOrder = "asc"; //默认排序
            }
            var param = [];
            if (p.parms) {
                var parms = $.isFunction(p.parms) ? p.parms() : p.parms;
                if (parms.length) {
                    $(parms).each(function () {
                        param.push({
                            name: this.name,
                            value: this.value
                        });
                    });
                    for (var i = parms.length - 1; i >= 0; i--) {
                        if (parms[i].temp)
                            parms.splice(i, 1);
                    }
                } else if (typeof parms == "object") {
                    for (var name in parms) {
                        param.push({
                            name: name,
                            value: parms[name]
                        });
                    }
                }
            }
            if (p.dataAction == "server") {
                if (p.usePager) {
                    param.push({
                        name: p.pageParmName,
                        value: p.newPage
                    });
                    param.push({
                        name: p.pagesizeParmName,
                        value: p.pageSize
                    });
                }
                if (p.sortName) {
                    param.push({
                        name: p.sortnameParmName,
                        value: p.sortName
                    });
                    param.push({
                        name: p.sortorderParmName,
                        value: p.sortOrder
                    });
                }
            };
            //直接增加不可用状态
            $(".lee-bar-btnload", g.toolbar).addClass("lee-disabled");
            if (p.dataType == "local") {
                //原语句: g.filteredData = p.data || g.currentData;
                //该语句修改了p.data, 导致过滤数据后, 丢失了初始数据.
                g.filteredData = $.extend(true, {}, p.data || g.currentData); //复制当前数据
                if (clause)
                    g.filteredData[p.root] = g._searchData(g.filteredData[p.root], clause); //本地过滤数据
                if (p.usePager)
                    g.currentData = g._getCurrentPageData(g.filteredData); //本地分页
                else {
                    g.currentData = g.filteredData;
                }
                g._convertTreeData();
                g._showData();
                $(".lee-bar-btnload", g.toolbar).removeClass("lee-disabled");
            } else if (p.dataAction == "local" && !loadServer) {
                if (g.data && g.data[p.root]) {
                    g.filteredData = g.data;
                    if (clause)
                        g.filteredData[p.root] = g._searchData(g.filteredData[p.root], clause);
                    g.currentData = g._getCurrentPageData(g.filteredData);
                    g._convertTreeData();
                    g._showData();
                }
            } else {
                g.loadServerData(param, clause, sourceType);
                //g.loadServerData.ligerDefer(g, 10, [param, clause]);
            }
            g.loading = false;
        },
        _convertTreeData: function () {
            var g = this,
                p = this.options;
            if (p.tree && p.tree.idField && p.tree.parentIDField) {
                g.currentData[p.root] = g.arrayToTree(g.currentData[p.root], p.tree.idField, p.tree.parentIDField);
                g.currentData[p.record] = g.currentData[p.root].length;
            } else if (p.tree && p.tree.grade) {
                g.currentData[p.root] = g.gradeToTree(g.currentData[p.root], p.tree.grade.gradeField, p.tree.grade.levelField, p.tree.grade.detailField, p.tree.grade.format);
                g.currentData[p.record] = g.currentData[p.root].length;
            }
        },
        _setLoading: function (flag) {
            var g = this;
            var loadName = flag ? "loading" : "loaded";
            if (g.hasBind(loadName)) {
                g.trigger(loadName);
            } else {
                g.toggleLoading(flag);
            }
        },
        loadCustomServerData: function (param, clause, sourceType) {
            var g = this,
                p = this.options;
            var requestFunc = p.customDataService;
            g._setLoading(true);
            requestFunc.call(this, param).then(function (data) {
                g.trigger('success', [data, g]);
                g.data = data;
                //保存缓存数据-记录总数
                if (g.data[p.record] != null) {
                    g.cacheData.records = g.data[p.record];
                }
                if (p.dataAction == "server") //服务器处理好分页排序数据
                {
                    g.currentData = g.data;
                    //读取缓存数据-记录总数(当没有返回总记录数)
                    if (g.currentData[p.record] == null && g.cacheData.records) {
                        g.currentData[p.record] = g.cacheData.records;
                    }
                }
                g._convertTreeData();
                g._showData.leeDefer(g, 10, [sourceType]);
                $(".lee-bar-btnload", g.toolbar).removeClass("lee-disabled");
                g.trigger('complete', [g]);
                g._setLoading(false);
            });
        },
        loadServerData: function (param, clause, sourceType) {
            var g = this,
                p = this.options;
            if (p.customDataService) {
                g.loadCustomServerData(param, clause, sourceType);
                return;
            }

            var url = p.url;
            if ($.isFunction(url)) url = url.call(g);
            var urlParms = $.isFunction(p.urlParms) ? p.urlParms.call(g) : p.urlParms;
            if (urlParms) {
                for (name in urlParms) {
                    url += url.indexOf('?') == -1 ? "?" : "&";
                    url += name + "=" + urlParms[name];
                }
            }
            var ajaxOptions = {
                type: p.method,
                url: url,
                data: param,
                async: p.async,
                dataType: 'json',
                beforeSend: function () {
                    if (g.hasBind('loading')) {
                        g.trigger('loading');
                    } else {
                        g.toggleLoading(true);
                    }
                },
                success: function (data) {
                    if (data.error) {
                        leeUI.Error(data.mes);
                    }
                    if (data.Code && data.Code != "ok") {
                        g.trigger('loadError', [data, g]);
                    }
                    if (data.Code) {
                        data = data.Data;
                    }
                    g.trigger('success', [data, g]);
                    //没有data 或者没有data.Rows 或者data.Rows.length<=0
                    if (!data || !data[p.root] || !data[p.root].length) {
                        g.currentData = g.data = {};
                        g.currentData[p.root] = g.data[p.root] = [];
                        if (data && data[p.record]) {
                            g.currentData[p.record] = g.data[p.record] = data[p.record];
                        } else {
                            g.currentData[p.record] = g.data[p.record] = 0;
                        }
                        g._convertTreeData();
                        g._showData(sourceType);
                        $(".lee-bar-btnload", g.toolbar).removeClass("lee-disabled");
                        return;
                    }
                    g.data = data;
                    //保存缓存数据-记录总数
                    if (g.data[p.record] != null) {
                        g.cacheData.records = g.data[p.record];
                    }
                    if (p.dataAction == "server") //服务器处理好分页排序数据
                    {
                        g.currentData = g.data;
                        //读取缓存数据-记录总数(当没有返回总记录数)
                        if (g.currentData[p.record] == null && g.cacheData.records) {
                            g.currentData[p.record] = g.cacheData.records;
                        }
                    } else //在客户端处理分页排序数据
                    {
                        g.filteredData = g.data;
                        if (clause) g.filteredData[p.root] = g._searchData(g.filteredData[p.root], clause);
                        if (p.usePager)
                            g.currentData = g._getCurrentPageData(g.filteredData);
                        else
                            g.currentData = g.filteredData;
                    }
                    g._convertTreeData();
                    g._showData.leeDefer(g, 10, [sourceType]);
                    $(".lee-bar-btnload", g.toolbar).removeClass("lee-disabled");
                },
                complete: function () {
                    g.trigger('complete', [g]);
                    if (g.hasBind('loaded')) {
                        g.trigger('loaded', [g]);
                    } else {
                        g.toggleLoading.leeDefer(g, 10, [false]);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    g.currentData = g.data = {};
                    g.currentData[p.root] = g.data[p.root] = [];
                    g.currentData[p.record] = g.data[p.record] = 0;
                    g.toggleLoading.leeDefer(g, 10, [false]);
                    $(".lee-bar-btnload", g.toolbar).removeClass("lee-disabled");
                    g.trigger('error', [XMLHttpRequest, textStatus, errorThrown]);
                }
            };
            //设置请求类型
            if (p.contentType) ajaxOptions.contentType = p.contentType;
            if (p.contentType == "application/json" && typeof (parms) != "string") {
                ajaxOptions.data = converParmJson(param)
            }
            $.ajax(ajaxOptions);

            function converParmJson(parm) {
                if (!parm) return "null";
                var o = parm,
                    result = {};
                if ($.isArray(o)) {
                    for (var i = 0; i < o.length; i++) {
                        result[o[i].name] = o[i].value;
                    }
                } else {
                    result = o;
                }
                return JSON.stringify(result); // liwl
            }
        },
        toggleLoading: function (show) {
            this.gridloading[show ? 'show' : 'hide']();
        },
        _createEditor: function (editorBuilder, container, editParm, width, height) {
            //创建编辑器
            var editor = editorBuilder.create.call(this, container, editParm);
            if (editorBuilder.setValue)
                editorBuilder.setValue.call(this, editor, editParm.value, editParm); //赋值
            if (editParm.column.editor && editParm.column.editor.initSelect) {
                if (editor.element && $(editor.element).is(":text")) {
                    $(editor.element).select();
                    $(editor.element).focus();
                }
                if (editor instanceof jQuery) {
                    if (editor.is(":text")) editor.select();
                    else editor.find(":text").select();
                }
            }
            if (editorBuilder.setText && editParm.column.textField)
                editorBuilder.setText.call(this, editor, editParm.text, editParm);
            if (editorBuilder.resize)
                editorBuilder.resize.call(this, editor, width, height, editParm);
            return editor;
        },
        /*
		@description 使一行进入编辑状态
		@param  {rowParm} rowindex或者rowdata
		@param {containerBulider} 编辑器填充层构造器
		*/
        beginEdit: function (rowParm, containerBulider) {
            //todo

            var g = this, p = this.options;
            if (!p.enabledEdit) return;
            var rowdata = g.getRow(rowParm);
            if (rowdata._editing) return;
            if (g.trigger('beginEdit', { record: rowdata, rowindex: rowdata['__index'] }) == false) return;
            g.editors[rowdata['__id']] = {};
            rowdata._editing = true;
            g.reRender({ rowdata: rowdata });
            containerBulider = containerBulider || function (rowdata, column) {
                var cellobj = g.getCellObj(rowdata, column);
                var container = $(cellobj).html("");
                g.setCellEditing(rowdata, column, true);
                return container;
            };
            for (var i = 0, l = g.columns.length; i < l; i++) {
                var column = g.columns[i];
                if (!column.name || !column.editor || !column.editor.type || !p.editors[column.editor.type]) continue;
                var editor = p.editors[column.editor.type];
                var editParm = {
                    record: rowdata,
                    value: g._getValueByName(rowdata, column.name),
                    column: column,
                    rowindex: rowdata['__index'],
                    grid: g
                };
                var container = containerBulider(rowdata, column);
                var width = container.width(), height = container.height();
                var editorControl = g._createEditor(editor, container, editParm, width, height);
                g.editors[rowdata['__id']][column['__id']] = {
                    editor: editor,
                    input: editorControl,
                    editParm: editParm,
                    container: container
                };
            }
            g.trigger('afterBeginEdit', { record: rowdata, rowindex: rowdata['__index'] });
        },
        cancelEdit: function (rowParm) {
            var g = this;
            if (rowParm == undefined) {
                for (var rowid in g.editors) {
                    //全部取消
                    g.cancelEdit(rowid);
                }
            } else {
                var rowdata = g.getRow(rowParm); //获取行
                if (!g.editors[rowdata['__id']]) return;
                if (g.trigger('beforeCancelEdit', {
                    record: rowdata,
                    rowindex: rowdata['__index']
                }) == false) return;
                for (var columnid in g.editors[rowdata['__id']]) {
                    var o = g.editors[rowdata['__id']][columnid];
                    if (o.editor.destroy) o.editor.destroy(o.input, o.editParm);
                }
                delete g.editors[rowdata['__id']];
                delete rowdata['_editing'];
                g.reRender({
                    rowdata: rowdata
                });
            }
        },
        addEditRow: function (rowdata, containerBulider) {
            this.submitEdit();
            rowdata = this.add(rowdata);
            this.beginEdit(rowdata, containerBulider);
        },
        submitEdit: function (rowParm) {
            var g = this,
                p = this.options;
            if (rowParm == undefined) {
                for (var rowid in g.editors) {
                    g.submitEdit(rowid);
                }
            } else {
                var rowdata = g.getRow(rowParm);
                var newdata = {};
                if (!rowdata || !g.editors[rowdata['__id']]) return;
                for (var columnid in g.editors[rowdata['__id']]) {
                    var o = g.editors[rowdata['__id']][columnid];
                    var column = o.editParm.column;
                    if (column.name) {
                        newdata[column.name] = o.editor.getValue(o.input, o.editParm);
                    }
                    //如果是文本框
                    if (column.textField && o.editor.getText) {
                        newdata[column.textField] = o.editor.getText(o.input, o.editParm);
                    }
                }
                //这里出发 提交前事件
                if (g.trigger('beforeSubmitEdit', {
                    record: rowdata,
                    rowindex: rowdata['__index'],
                    newdata: newdata
                }) == false)
                    return false;
                g.updateRow(rowdata, newdata);
                g.trigger('afterSubmitEdit', {
                    record: rowdata,
                    rowindex: rowdata['__index'],
                    newdata: newdata
                });
            }
        },
        _enabledEditByCell: function (cell) {
            var g = this,
                p = this.options;
            var column = g.getColumn(cell);
            if (!column) return false;
            return column.editor && column.editor.type && !column.readonly;
        },
        //结束当前的编辑 并进入下一个单元格的编辑状态(如果位于最后单元格，进入下一行第一个单元格)
        endEditToNext: function () {
            var g = this,
                p = this.options;
            var editor = g.editor,
                jnext = null,
                jprev = null;
            if (editor) {
                var editParm = editor.editParm;
                var column = editParm.column;
                var columnIndex = $.inArray(column, g.columns);
                var cell = g.getCellObj(editParm.record, editParm.column);
                jprev = $(cell);
                jnext = jprev.next();
                if (!jnext.length) jnext = getNextRowCell(); //已经是当前行最后一个单元格了
                if (jnext.length) {
                    //获取到下一个可编辑的列
                    while (!g._enabledEditByCell(jnext.get(0))) {
                        jprev = jnext;
                        jnext = jnext.next();
                        while (jnext.css("display") == "none") {
                            jnext = jnext.next();
                        }
                        if (!jnext.length) //已经是当前行最后一个单元格了
                        {
                            jnext = getNextRowCell();
                        }
                    }
                }
                //获取下一行第一个列对象
                function getNextRowCell() {
                    return jprev.parent("tr").next(".lee-grid-row").find("td:first");
                }
            }

            g.endEdit();
            if (jnext && jnext.length) {
                g._applyEditor(jnext.get(0));
            }
        },
        endEdit: function (rowParm) {
            var g = this,
                p = this.options;
            if (g.editor.editing) {
                var o = g.editor;
                g.trigger('beforeEndEdit', [o]);
                g.trigger('sysEndEdit', [g.editor.editParm]);
                g.trigger('endEdit', [g.editor.editParm]);
                if (o.editor.destroy) o.editor.destroy(o.input, o.editParm);
                g.editor.container.remove();
                g.reRender({
                    rowdata: g.editor.editParm.record,
                    column: g.editor.editParm.column
                });
                g.trigger('afterEdit', [g.editor.editParm]);
                g.editor = {
                    editing: false
                };
            } else if (rowParm != undefined) {
                var rowdata = g.getRow(rowParm);
                if (!g.editors[rowdata['__id']]) return;
                if (g.submitEdit(rowParm) == false) return false;
                for (var columnid in g.editors[rowdata['__id']]) {
                    var o = g.editors[rowdata['__id']][columnid];
                    if (o.editor.destroy) o.editor.destroy(o.input, o.editParm);
                }
                delete g.editors[rowdata['__id']];
                delete rowdata['_editing'];
                g.trigger('afterEdit', {
                    record: rowdata,
                    rowindex: rowdata['__index']
                });
            } else {
                for (var rowid in g.editors) {
                    g.endEdit(rowid);
                }
            }
            g._fixHeight.leeDefer(g, 10);
        },

        setWidth: function (w) {
            return this._setWidth(w);
        },
        setHeight: function (h) {
            return this._setHeight(h);
        },
        //是否启用复选框列
        enabledCheckbox: function () {
            return this.options.checkbox ? true : false;
        },
        //是否固定列
        enabledFrozen: function () {
            var g = this,
                p = this.options;
            if (!p.frozen) return false;
            var cols = g.columns || [];
            if (g.enabledDetail() && p.frozenDetail || g.enabledCheckbox() && p.frozenCheckbox ||
                p.frozenRownumbers && p.rownumbers) return true;
            for (var i = 0, l = cols.length; i < l; i++) {
                if (cols[i].frozen) {
                    return true;
                }
            }
            this._setFrozen(false);
            return false;
        },
        //是否启用明细编辑
        enabledDetailEdit: function () {
            if (!this.enabledDetail()) return false;
            return this.options.detailToEdit ? true : false;
        },
        //是否启用明细列
        enabledDetail: function () {
            if (this.options.detail && this.options.detail.onShowDetail) return true;
            return false;
        },
        //是否启用分组
        enabledGroup: function () {
            return this.options.groupColumnName ? true : false;
        },
        deleteSelectedRow: function () {
            if (!this.selected) return;
            for (var i in this.selected) {
                var o = this.selected[i];
                if (o['__id'] in this.records)
                    this._deleteData.leeDefer(this, 10, [o]);
            }
            this.reRender.leeDefer(this, 20);
        },
        removeRange: function (rowArr) {
            var g = this,
                p = this.options;
            $.each(rowArr, function () {
                g._removeData(this);
            });
            g.reRender();
        },
        remove: function (rowParm) {
            var g = this,
                p = this.options;
            var rowdata = g.getRow(rowParm);
            g._removeData(rowParm);
            g.reRender();
        },
        deleteRange: function (rowArr) {
            var g = this,
                p = this.options;
            $.each(rowArr, function () {
                g._deleteData(this);
            });
            g.reRender();
        },
        deleteRow: function (rowParm) {
            var g = this,
                p = this.options;
            var rowdata = g.getRow(rowParm);
            if (!rowdata) return;
            g._deleteData(rowdata);
            p.virtualScroll && g.refreshScroll();
            g.reRender();
            g.isDataChanged = true;
        },
        _deleteData: function (rowParm) {
            var g = this,
                p = this.options;
            var rowdata = g.getRow(rowParm);
            rowdata[p.statusName] = 'delete';
            if (p.tree) {
                var children = g.getChildren(rowdata, true);
                if (children) {
                    //删除孩子节点状态
                    for (var i = 0, l = children.length; i < l; i++) {
                        //children[i][p.statusName] = 'delete'; liwl
                        g._deleteData(children[i]);
                    }
                }
            }
            g.deletedRows = g.deletedRows || [];
            g.deletedRows.push(rowdata);
            g._removeSelected(rowdata);
        },
        /*
        @param  {arg} column index、column name、column、单元格
        @param  {value} 值
        @param  {rowParm} rowindex或者rowdata
        */
        updateCell: function (arg, value, rowParm) {
            var g = this,
                p = this.options;
            var column, cellObj, rowdata;
            if (typeof (arg) == "string") //column name
            {
                for (var i = 0, l = g.columns.length; i < l; i++) {
                    if (g.columns[i].name == arg) {
                        g.updateCell(i, value, rowParm);
                    }
                }
                return;
            }
            if (typeof (arg) == "number") {
                column = g.columns[arg];
                rowdata = g.getRow(rowParm);
                cellObj = g.getCellObj(rowdata, column);
            } else if (typeof (arg) == "object" && arg['__id']) {
                column = arg;
                rowdata = g.getRow(rowParm);
                cellObj = g.getCellObj(rowdata, column);
            } else {
                cellObj = arg;
                var ids = cellObj.id.split('|');
                var columnid = ids[ids.length - 1];
                column = g._columns[columnid];
                var row = $(cellObj).parent();
                rowdata = rowdata || g.getRow(row[0]);
            }
            if (value != null && column.name) {
                g._setValueByName(rowdata, column.name, value);
                if (rowdata[p.statusName] != 'add')
                    rowdata[p.statusName] = 'update';
                g.isDataChanged = true;
            }
            g.reRender({
                rowdata: rowdata,
                column: column
            });
        },
        addRows: function (rowdataArr, neardata, isBefore, parentRowData) {
            var g = this,
                p = this.options;
            $(rowdataArr).each(function () {
                g.addRow(this, neardata, isBefore, parentRowData);
            });
        },
        _createRowid: function () {
            return "r" + (1000 + this.recordNumber);
        },
        _isRowId: function (str) {
            return (str in this.records);
        },
        _addNewRecord: function (o, previd, pid) {
            var g = this,
                p = this.options;
            g.recordNumber++;
            o['__id'] = g._createRowid();
            o['__previd'] = previd;
            if (previd && previd != -1) {
                var prev = g.records[previd];
                if (prev['__nextid'] && prev['__nextid'] != -1) {
                    var prevOldNext = g.records[prev['__nextid']];
                    if (prevOldNext)
                        prevOldNext['__previd'] = o['__id'];
                }
                prev['__nextid'] = o['__id'];
                o['__index'] = prev['__index'] + 1;
            } else {
                o['__index'] = 0;
            }
            if (p.tree) {
                if (pid && pid != -1) {
                    var parent = g.records[pid];
                    o['__pid'] = pid;
                    o['__level'] = parent['__level'] + 1;
                } else {
                    o['__pid'] = -1;
                    o['__level'] = 1;
                }
                o['__hasChildren'] = o[p.tree.childrenName] ? true : false;
            }
            o[p.statusName] = o[p.statusName] || "nochanged";
            g.rows[o['__index']] = o;
            g.records[o['__id']] = o;
            return o;
        },
        //将原始的数据转换成适合 grid的行数据 
        _getRows: function (data) {
            var g = this,
                p = this.options;
            var targetData = [];

            function load(data) {
                if (!data || !data.length) return;
                for (var i = 0, l = data.length; i < l; i++) {
                    var o = data[i];
                    targetData.push(o);
                    if (o[p.tree.childrenName]) {
                        load(o[p.tree.childrenName]);
                    }
                }
            }
            load(data);
            return targetData;
        },
        _updateGridData: function () {
            var g = this,
                p = this.options;
            g.recordNumber = 0;
            g.rows = [];
            g.records = {};
            var previd = -1;
            //重新对rowid index 排序
            function load(data, pid) {
                if (!data || !data.length) return;
                for (var i = 0, l = data.length; i < l; i++) {
                    var o = data[i];
                    g.formatRecord(o);
                    if (o[p.statusName] == "delete") continue;
                    g._addNewRecord(o, previd, pid);
                    previd = o['__id'];
                    if (o['__hasChildren']) {
                        load(o[p.tree.childrenName], o['__id']);
                    }
                }
            }
            load(g.currentData[p.root], -1);
            return g.rows;
        },
        _moveData: function (from, to, isAfter) {
            var g = this,
                p = this.options;
            var fromRow = g.getRow(from);
            var toRow = g.getRow(to);
            var fromIndex, toIndex;
            var listdata = g._getParentChildren(fromRow);
            fromIndex = $.inArray(fromRow, listdata);
            listdata.splice(fromIndex, 1);
            listdata = g._getParentChildren(toRow);
            toIndex = $.inArray(toRow, listdata);
            listdata.splice(toIndex + (isAfter ? 1 : 0), 0, fromRow);
        },
        move: function (from, to, isAfter) {
            this._moveData(from, to, isAfter);
            this.reRender();
        },
        moveRange: function (rows, to, isAfter) {
            for (var i in rows) {
                this._moveData(rows[i], to, isAfter);
            }
            this.reRender();
        },
        up: function (rowParm) {
            var g = this,
                p = this.options;
            var rowdata = g.getRow(rowParm);
            var listdata = g._getParentChildren(rowdata);
            var index = $.inArray(rowdata, listdata);
            if (index == -1 || index == 0) return;
            var selected = g.getSelected() || rowdata;
            g.move(rowdata, listdata[index - 1], false);

            g.select(selected);
        },
        down: function (rowParm) {
            var g = this,
                p = this.options;
            var rowdata = g.getRow(rowParm);
            var listdata = g._getParentChildren(rowdata);
            var index = $.inArray(rowdata, listdata);
            if (index == -1 || index == listdata.length - 1) return;
            var selected = g.getSelected() || rowdata;
            g.move(rowdata, listdata[index + 1], true);
            g.select(selected);
        },
        addRow: function (rowdata, neardata, isBefore, parentRowData) {
            var g = this,
                p = this.options;
            rowdata = rowdata || {};
            //重新构造数据
            g._addData(rowdata, parentRowData, neardata, isBefore);
            p.virtualScroll && g.refreshScroll();
            //全量渲染
            g.reRender();
            //标识状态
            rowdata[p.statusName] = 'add';
            //递归设置子节点状态
            if (p.tree) {
                var children = g.getChildren(rowdata, true);
                if (children) {
                    for (var i = 0, l = children.length; i < l; i++) {
                        children[i][p.statusName] = 'add';
                    }
                }
            }
            //标记数据变化
            g.isDataChanged = true;
            //标记数据总数
            p.total = p.total ? (p.total + 1) : 1;
            p.pageCount = Math.ceil(p.total / p.pageSize);
            //构造分页器
            g._buildPager();
            //触发事件
            g.trigger('SysGridHeightChanged');
            g.trigger('afterAddRow', [rowdata]);
            return rowdata;
        },
        updateRow: function (rowDom, newRowData) {
            var g = this,
                p = this.options;
            var rowdata = g.getRow(rowDom);
            //标识状态
            g.isDataChanged = true;
            $.extend(rowdata, newRowData || {});
            if (rowdata[p.statusName] != 'add')
                rowdata[p.statusName] = 'update';
            g.reRender.leeDefer(g, 10, [{
                rowdata: rowdata
            }]);
            return rowdata;
        },
        setCellEditing: function (rowdata, column, editing) {
            var g = this,
                p = this.options;
            var cell = g.getCellObj(rowdata, column);
            var methodName = editing ? 'addClass' : 'removeClass';
            $(cell)[methodName]("lee-grid-row-cell-editing");
            if (rowdata['__id'] != 0) {
                var prevrowobj = $(g.getRowObj(rowdata['__id'])).prev();
                //当使用可编辑的grid带分组时，第一行的prev对象是分组行，不具备id等getRow方法中需要的信息
                if (!prevrowobj.length ||
                    prevrowobj.length <= 0 ||
                    prevrowobj[0].id == null ||
                    prevrowobj[0].id == "") {
                    return;
                }
                var prevrow = g.getRow(prevrowobj[0]);
                var cellprev = g.getCellObj(prevrow, column);
                if (!cellprev) return;
                $(cellprev)[methodName]("lee-grid-row-cell-editing-topcell");
            }
            if (column['__previd'] != -1 && column['__previd'] != null) {
                var cellprev = $(g.getCellObj(rowdata, column)).prev();
                $(cellprev)[methodName]("lee-grid-row-cell-editing-leftcell");
            }
        },
        reRender: function (e) {
            var g = this,
                p = this.options;
            e = e || {};
            var rowdata = e.rowdata,
                column = e.column,
                //只重渲染统计行
                totalOnly = e.totalOnly;
            if (column && (column.isdetail || column.ischeckbox)) return;
            if (rowdata && rowdata[p.statusName] == "delete") return;
            if (totalOnly) {
                $(g.columns).each(function () {
                    reRenderTotal(this);
                });
            } else if (rowdata && column) {
                var cell = g.getCellObj(rowdata, column);
                $(cell).html(g._getCellHtml(rowdata, column));
                if (!column.issystem)
                    g.setCellEditing(rowdata, column, false);
            } else if (rowdata) {
                $(g.columns).each(function () {
                    g.reRender({
                        rowdata: rowdata,
                        column: this
                    });
                });
            } else if (column) {
                for (var rowid in g.records) {
                    g.reRender({
                        rowdata: g.records[rowid],
                        column: column
                    });
                }
                reRenderTotal(column);
            } else {
                g._showData();
            }

            function reRenderTotal(column) {
                if (!column.totalSummary) return;
                for (var i = 0; i < g.totalNumber; i++) {
                    var tobj = document.getElementById(g.id + "|total" + i + "|" + column['__id']);
                    $("div:first", tobj).html(g._getTotalCellContent(column, g.groups && g.groups[i] ? g.groups[i] : g.currentData[p.root]));
                }
            }
        },
        getData: function (status, removeStatus) {
            var g = this,
                p = this.options;
            var data = [];
            if (removeStatus == undefined) removeStatus = true;
            for (var rowid in g.records) {
                var o = $.extend(true, {}, g.records[rowid]);
                if (o[p.statusName] == status || status == undefined) {
                    var res = g.formatRecord(o, removeStatus);
                    if (p.tree)
                        delete res[p.tree.childrenName];
                    data.push(res);
                }
            }
            return data;
        },
        //格式化数据
        formatRecord: function (o, removeStatus) {
            //删除私有状态
            delete o['__id'];
            delete o['__previd'];
            delete o['__nextid'];
            delete o['__index'];
            if (this.options.tree) {
                delete o['__pid'];
                delete o['__level'];
                delete o['__hasChildren'];
            }
            if (removeStatus) delete o[this.options.statusName];
            return o;
        },
        getUpdated: function () {
            return this.getData('update', true);
        },
        getDeleted: function () {
            return this.deletedRows;
        },
        getAdded: function () {
            return this.getData('add', true);
        },
        getChanges: function () {
            //getChanges函数必须保留__status属性,否则根本不知道哪些是新增的,哪些是被删除的.
            //则本函数返回的结果毫无意义.
            var g = this,
                p = this.options;
            var data = [];
            if (this.deletedRows) {
                $(this.deletedRows).each(function () {
                    var o = $.extend(true, {}, this);
                    data.push(g.formatRecord(o, false));
                });
            }
            $.merge(data, g.getData("update", false));
            $.merge(data, g.getData("add", false));
            return data;
        },
        findColumByName: function (name) {
            var g = this,
                res = null;
            for (var i in g._columns) {
                if (g._columns[i].name == name) {
                    res = g._columns[i];
                    break;
                }
            }
            return res;

        },
        getColumn: function (columnParm) {
            var g = this,
                p = this.options;
            if (typeof columnParm == "string") // column id
            {
                if (g._isColumnId(columnParm))
                    return g._columns[columnParm];
                else
                    return g.findColumByName(columnParm) || g.columns[parseInt(columnParm)];
            } else if (typeof (columnParm) == "number") //column index
            {
                return g.columns[columnParm];
            } else if (typeof columnParm == "object" && columnParm.nodeType == 1) //column header cell
            {
                var ids = columnParm.id.split('|');
                var columnid = ids[ids.length - 1];
                return g._columns[columnid];
            }
            return columnParm;
        },
        getColumnByName: function (columnname) {
            var g = this,
                p = this.options;
            for (i = 0; i < g.columns.length; i++) {
                if (g.columns[i].name == columnname) {
                    return g.columns[i];
                }
            }
            return null;
        },
        getColumnType: function (columnname) {
            var g = this,
                p = this.options;
            for (i = 0; i < g.columns.length; i++) {
                if (g.columns[i].name == columnname) {
                    if (g.columns[i].type) return g.columns[i].type;
                    return "string";
                }
            }
            return null;
        },
        //是否包含汇总
        isTotalSummary: function () {
            var g = this,
                p = this.options;
            for (var i = 0; i < g.columns.length; i++) {
                if (g.columns[i].totalSummary) return true;
            }
            return false;
        },

        //根据层次获取列集合
        //如果columnLevel为空，获取叶节点集合
        getColumns: function (columnLevel) {
            var g = this,
                p = this.options;
            var columns = [];
            for (var id in g._columns) {
                var col = g._columns[id];
                if (columnLevel != undefined) {
                    if (col['__level'] == columnLevel) columns.push(col);
                } else {
                    if (col['__leaf']) columns.push(col);
                }
            }
            return columns;
        },
        //改变排序
        changeSort: function (columnName, sortOrder) {
            var g = this,
                p = this.options;
            if (g.loading) return true;
            if (p.dataAction == "local") {
                var columnType = g.getColumnType(columnName);
                if (!g.sortedData)
                    g.sortedData = g.filteredData;
                if (!g.sortedData || !g.sortedData[p.root])
                    return;
                if (p.sortName == columnName) {
                    g.sortedData[p.root].reverse();
                } else {
                    g.sortedData[p.root].sort(function (data1, data2) {
                        return g._compareData(data1, data2, columnName, columnType);
                    });
                }
                if (p.usePager)
                    g.currentData = g._getCurrentPageData(g.sortedData);
                else
                    g.currentData = g.sortedData;
                g._showData();
            }
            p.sortName = columnName;
            p.sortOrder = sortOrder;
            if (p.dataAction == "server") {
                g.loadData(p.where);
            }
        },
        //改变分页
        changePage: function (ctype) {
            var g = this,
                p = this.options;
            if (g.loading) return true;
            if (p.dataAction != "local" && g.isDataChanged && !confirm(p.isContinueByDataChanged))
                return false;
            p.pageCount = parseInt($(".pcontrol span", g.toolbar).html());
            switch (ctype) {
                case 'first':
                    if (p.page == 1) return;
                    p.newPage = 1;
                    break;
                case 'prev':
                    if (p.page == 1) return;
                    if (p.page > 1) p.newPage = parseInt(p.page) - 1;
                    break;
                case 'next':
                    if (p.page >= p.pageCount) return;
                    p.newPage = parseInt(p.page) + 1;
                    break;
                case 'last':
                    if (p.page >= p.pageCount) return;
                    p.newPage = p.pageCount;
                    break;
                case 'input':
                    var nv = parseInt($('.pcontrol input', g.toolbar).val());
                    if (isNaN(nv)) nv = 1;
                    if (nv < 1) nv = 1;
                    else if (nv > p.pageCount) nv = p.pageCount;
                    $('.pcontrol input', g.toolbar).val(nv);
                    p.newPage = nv;
                    break;
            }
            if (p.newPage == p.page) return false;
            if (p.newPage == 1) {
                $(".lee-bar-btnfirst", g.toolbar).addClass("lee-disabled");
                $(".lee-bar-btnprev", g.toolbar).addClass("lee-disabled");
            } else {
                $(".lee-bar-btnfirst", g.toolbar).removeClass("lee-disabled");
                $(".lee-bar-btnprev", g.toolbar).removeClass("lee-disabled");
            }
            if (p.newPage == p.pageCount) {
                $(".lee-bar-btnlast", g.toolbar).addClass("lee-disabled");
                $(".lee-bar-btnnext", g.toolbar).addClass("lee-disabled");
            } else {
                $(".lee-bar-btnlast", g.toolbar).removeClass("lee-disabled");
                $(".lee-bar-btnnext", g.toolbar).removeClass("lee-disabled");
            }
            g.trigger('changePage', [p.newPage]);
            if (p.dataAction == "server") {
                if (!p.parms) {
                    p.parms = [];
                }
                if ($.isArray(p.parms)) {
                    p.parms.push({
                        name: "changepage",
                        value: "1",
                        temp: true
                    });
                } else {
                    p.parms["changepage"] = "1";
                }
                g.loadData(p.where);
            } else {
                g.currentData = g._getCurrentPageData(g.filteredData);
                //增加以下一句调用: 在显示数据之前, 应该先调用转换tree的函数. 
                //否则会导致树的数据重复显示
                if (p.tree) {
                    var childrenName = p.tree.childrenName;
                    $(g.filteredData[p.root]).each(function (index, item) {
                        if (item[childrenName])
                            item[childrenName] = [];
                    });
                    g._convertTreeData();
                }
                g._showData();
            }
        },
        getSelectedRow: function () {
            for (var i in this.selected) {
                var o = this.selected[i];
                if (o['__id'] in this.records)
                    return o;
            }
            return null;
        },
        getSelectedRows: function () {
            var arr = [];
            var g = this,
                p = this.options;
            if (p.virtualScroll) {
                for (var key in this.records) {
                    if (this.records[key]["__selected"]) {
                        arr.push(this.records[key]);
                    }
                }

            } else {
                for (var i in this.selected) {
                    var o = this.selected[i];
                    if (o['__id'] in this.records)
                        arr.push(o);
                }
            }

            return arr;
        },
        getSelectedRowObj: function () {
            for (var i in this.selected) {
                var o = this.selected[i];
                if (o['__id'] in this.records)
                    return this.getRowObj(o);
            }
            return null;
        },
        getSelectedRowObjs: function () {
            var arr = [];
            for (var i in this.selected) {
                var o = this.selected[i];
                if (o['__id'] in this.records)
                    arr.push(this.getRowObj(o));
            }
            return arr;
        },
        getCellObj: function (rowParm, column) {
            var rowdata = this.getRow(rowParm);
            column = this.getColumn(column);
            return document.getElementById(this._getCellDomId(rowdata, column));
        },
        getRowObj: function (rowParm, frozen) {
            var g = this,
                p = this.options;
            if (rowParm == null) return null;
            if (typeof (rowParm) == "string") {
                if (g._isRowId(rowParm))
                    return document.getElementById(g.id + (frozen ? "|1|" : "|2|") + rowParm);
                else
                    return document.getElementById(g.id + (frozen ? "|1|" : "|2|") + g.rows[parseInt(rowParm)]['__id']);
            } else if (typeof (rowParm) == "number") {
                return document.getElementById(g.id + (frozen ? "|1|" : "|2|") + g.rows[rowParm]['__id']);
            } else if (typeof (rowParm) == "object" && rowParm['__id']) //rowdata
            {
                return g.getRowObj(rowParm['__id'], frozen);
            }
            return rowParm;
        },
        getRow: function (rowParm) {
            var g = this,
                p = this.options;
            if (rowParm == null) return null;
            if (typeof (rowParm) == "string") {
                if (g._isRowId(rowParm))
                    return g.records[rowParm];
                else
                    return g.rows[parseInt(rowParm)];
            } else if (typeof (rowParm) == "number") {
                return g.rows[parseInt(rowParm)];
            } else if (typeof (rowParm) == "object" && rowParm.nodeType == 1 && !rowParm['__id']) //dom对象
            {
                return g._getRowByDomId(rowParm.id);
            }
            return rowParm;
        },
        _setColumnVisible: function (column, hide) {
            var g = this,
                p = this.options;
            if (!hide) //显示
            {
                column._hide = false;
                document.getElementById(column['__domid']).style.display = "";
                //判断分组列是否隐藏,如果隐藏了则显示出来
                if (column['__pid'] != -1) {
                    var pcol = g._columns[column['__pid']];
                    if (pcol._hide) {
                        document.getElementById(pcol['__domid']).style.display = "";
                        this._setColumnVisible(pcol, hide);
                    }
                }
            } else {
                column._hide = true;
                document.getElementById(column['__domid']).style.display = "none";
                //判断同分组的列是否都隐藏,如果是则隐藏分组列
                if (column['__pid'] != -1) {
                    var hideall = true;
                    var pcol = this._columns[column['__pid']];
                    for (var i = 0; pcol && i < pcol.columns.length; i++) {
                        if (!pcol.columns[i]._hide) {
                            hideall = false;
                            break;
                        }
                    }
                    if (hideall) {
                        pcol._hide = true;
                        document.getElementById(pcol['__domid']).style.display = "none";
                        this._setColumnVisible(pcol, hide);
                    }
                }
            }
        },
        //显示隐藏列
        toggleCol: function (columnparm, visible, toggleByPopup) {
            var g = this,
                p = this.options;
            var column;
            if (typeof (columnparm) == "number") {
                column = g.columns[columnparm];
            } else if (typeof (columnparm) == "object" && columnparm['__id']) {
                column = columnparm;
            } else if (typeof (columnparm) == "string") {
                if (g._isColumnId(columnparm)) // column id
                {
                    column = g._columns[columnparm];
                } else // column name
                {
                    $(g.columns).each(function () {
                        if (this.name == columnparm)
                            g.toggleCol(this, visible, toggleByPopup);
                    });
                    return;
                }
            }
            if (!column) return;
            var columnindex = column['__leafindex'];
            var headercell = document.getElementById(column['__domid']);
            if (!headercell) return;
            headercell = $(headercell);
            var cells = [];
            for (var i in g.rows) {
                var obj = g.getCellObj(g.rows[i], column);
                if (obj) cells.push(obj);
            }
            for (var i = 0; i < g.totalNumber; i++) {
                var tobj = document.getElementById(g.id + "|total" + i + "|" + column['__id']);
                if (tobj) cells.push(tobj);
            }
            var colwidth = column._width;
            //显示列
            if (visible && column._hide) {
                if (column.frozen)
                    g.f.gridtablewidth += (parseInt(colwidth) + 1);
                else
                    g.gridtablewidth += (parseInt(colwidth) + 1);
                g._setColumnVisible(column, false);
                $(cells).show();
            }
            //隐藏列
            else if (!visible && !column._hide) {
                if (column.frozen)
                    g.f.gridtablewidth -= (parseInt(colwidth) + 1);
                else
                    g.gridtablewidth -= (parseInt(colwidth) + 1);
                g._setColumnVisible(column, true);
                $(cells).hide();
            }
            if (column.frozen) {
                $("div:first", g.f.gridheader).width(g.f.gridtablewidth);
                $("div:first", g.f.gridbody).width(g.f.gridtablewidth);
            } else {
                $("div:first", g.gridheader).width(g.gridtablewidth + 40);
                if (g.gridtablewidth) {
                    $("div:first", g.gridbody).width(g.gridtablewidth);
                } else {
                    $("div:first", g.gridbody).css("width", "auto");
                }
            }
            g._updateFrozenWidth();
            if (!toggleByPopup) {
                $(':checkbox[columnindex=' + columnindex + "]", g.popup).each(function () {
                    this.checked = visible;
                    if ($.fn.leeCheckBox) {
                        var checkboxmanager = $(this).leeUI();
                        if (checkboxmanager) checkboxmanager.updateStyle();
                    }
                });
            }
        },
        //设置列宽
        setColumnWidth: function (columnparm, newwidth) {
            var g = this,
                p = this.options;
            if (!newwidth) return;
            newwidth = parseInt(newwidth, 10);
            var column;
            if (typeof (columnparm) == "number") {
                column = g.columns[columnparm];
            } else if (typeof (columnparm) == "object" && columnparm['__id']) {
                column = columnparm;
            } else if (typeof (columnparm) == "string") {
                if (g._isColumnId(columnparm)) {// column id
                    column = g._columns[columnparm];
                } else { // column name
                    $(g.columns).each(function () {
                        if (this.name == columnparm)
                            g.setColumnWidth(this, newwidth);
                    });
                    return;
                }
            }
            if (!column) return;
            var mincolumnwidth = p.minColumnWidth;
            if (column.minWidth) mincolumnwidth = column.minWidth;
            newwidth = newwidth < mincolumnwidth ? mincolumnwidth : newwidth;
            var diff = newwidth - column._width;
            if (g.trigger('beforeChangeColumnWidth', [column, newwidth]) == false) return;
            column._width = newwidth;
            if (column.frozen) {
                g.f.gridtablewidth += diff;
                $("div:first", g.f.gridheader).width(g.f.gridtablewidth);
                $("div:first", g.f.gridbody).width(g.f.gridtablewidth);
            } else {
                g.gridtablewidth += diff;
                $("div:first", g.gridheader).width(g.gridtablewidth + 40);
                $("div:first", g.gridbody).width(g.gridtablewidth);
            }
            $(document.getElementById(column['__domid'])).css('width', newwidth); //设置td列宽
            if (p.autoColWidth) {
                $(document.getElementById(column['__domid'] + "_col")).css('width', newwidth); //设置td列宽
                $(document.getElementById(column['__domid'] + "_detail")).css('width', newwidth); //设置td列宽
            }
            var cells = [];
            for (var rowid in g.records) {
                var obj = g.getCellObj(g.records[rowid], column);
                if (obj) cells.push(obj);

                if (!g.enabledDetailEdit() && g.editors[rowid] && g.editors[rowid][column['__id']]) {
                    var o = g.editors[rowid][column['__id']];
                    if (o.editor.resize) o.editor.resize(o.input, newwidth, o.container.height(), o.editParm);
                }
            }
            for (var i = 0; i < g.totalNumber; i++) {
                var tobj = document.getElementById(g.id + "|total" + i + "|" + column['__id']);
                if (tobj) cells.push(tobj);
            }
            //设置单元格内部的宽度
            $(cells).css('width', newwidth).find("> div.lee-grid-row-cell-inner:first").css('width', newwidth - 8);

            g._updateFrozenWidth();
            g._updateHorizontalScrollStatus.leeDefer(g, 10);

            g.trigger('afterChangeColumnWidth', [column, newwidth]);
        },
        //改变列表头内容
        changeHeaderText: function (columnparm, headerText) {
            var g = this,
                p = this.options;
            var column;
            if (typeof (columnparm) == "number") {
                column = g.columns[columnparm];
            } else if (typeof (columnparm) == "object" && columnparm['__id']) {
                column = columnparm;
            } else if (typeof (columnparm) == "string") {
                if (g._isColumnId(columnparm)) // column id
                {
                    column = g._columns[columnparm];
                } else // column name
                {
                    $(g.columns).each(function () {
                        if (this.name == columnparm)
                            g.changeHeaderText(this, headerText);
                    });
                    return;
                }
            }
            if (!column) return;
            var columnindex = column['__leafindex'];
            var headercell = document.getElementById(column['__domid']);
            $(".lee-grid-hd-cell-text", headercell).html(headerText);
            if (p.allowHideColumn) {
                $(':checkbox[columnindex=' + columnindex + "]", g.popup).parent().next().html(headerText);
            }
        },
        //改变列的位置
        changeCol: function (from, to, isAfter) {
            var g = this,
                p = this.options;
            if (!from || !to) return;
            var fromCol = g.getColumn(from);
            var toCol = g.getColumn(to);
            fromCol.frozen = toCol.frozen;
            var fromColIndex, toColIndex;
            var fromColumns = fromCol['__pid'] == -1 ? p.columns : g._columns[fromCol['__pid']].columns;
            var toColumns = toCol['__pid'] == -1 ? p.columns : g._columns[toCol['__pid']].columns;
            fromColIndex = $.inArray(fromCol, fromColumns);
            toColIndex = $.inArray(toCol, toColumns);
            var sameParent = fromColumns == toColumns;
            var sameLevel = fromCol['__level'] == toCol['__level'];
            toColumns.splice(toColIndex + (isAfter ? 1 : 0), 0, fromCol);
            if (!sameParent) {
                fromColumns.splice(fromColIndex, 1);
            } else {
                if (isAfter) fromColumns.splice(fromColIndex, 1);
                else fromColumns.splice(fromColIndex + 1, 1);
            }
            g._setColumns(p.columns);
            g.reRender();
        },

        collapseDetail: function (rowParm) {
            var g = this,
                p = this.options;
            var rowdata = g.getRow(rowParm);
            if (!rowdata) return;
            for (var i = 0, l = g.columns.length; i < l; i++) {
                if (g.columns[i].isdetail) {
                    var row = g.getRowObj(rowdata);
                    var cell = g.getCellObj(rowdata, g.columns[i]);
                    $(row).next("tr.lee-grid-detailpanel").hide();
                    $(".lee-grid-row-cell-detailbtn:first", cell).removeClass("lee-open");
                    g.trigger('SysGridHeightChanged');
                    return;
                }
            }
        },
        extendDetail: function (rowParm) {
            var g = this,
                p = this.options;
            var rowdata = g.getRow(rowParm);
            if (!rowdata) return;
            for (var i = 0, l = g.columns.length; i < l; i++) {
                if (g.columns[i].isdetail) {
                    var row = g.getRowObj(rowdata);
                    var cell = g.getCellObj(rowdata, g.columns[i]);
                    $(row).next("tr.lee-grid-detailpanel").show();
                    $(".lee-grid-row-cell-detailbtn:first", cell).addClass("lee-open");
                    g.trigger('SysGridHeightChanged');
                    return;
                }
            }
        },
        getParent: function (rowParm) {
            var g = this,
                p = this.options;
            if (!p.tree) return null;
            var rowdata = g.getRow(rowParm);
            if (!rowdata) return null;
            if (rowdata['__pid'] in g.records) return g.records[rowdata['__pid']];
            else return null;
        },
        getChildren: function (rowParm, deep) {
            var g = this,
                p = this.options;
            if (!p.tree) return null;
            var rowData = g.getRow(rowParm);
            if (!rowData) return null;
            var arr = [];

            function loadChildren(data) {
                if (data[p.tree.childrenName]) {
                    for (var i = 0, l = data[p.tree.childrenName].length; i < l; i++) {
                        var o = data[p.tree.childrenName][i];
                        if (o[p.statusName] == 'delete') continue;
                        arr.push(o);
                        if (deep)
                            loadChildren(o);
                    }
                }
            }
            loadChildren(rowData);
            return arr;
        },
        isLeaf: function (rowParm) {
            var g = this,
                p = this.options;
            var rowdata = g.getRow(rowParm);
            if (!rowdata) return;
            return rowdata['__hasChildren'] ? false : true;
        },
        hasChildren: function (rowParm) {
            var g = this,
                p = this.options;
            var rowdata = this.getRow(rowParm);
            if (!rowdata) return;
            return (rowdata[p.tree.childrenName] && rowdata[p.tree.childrenName].length) ? true : false;
        },
        existRecord: function (record) {
            for (var rowid in this.records) {
                if (this.records[rowid] == record) return true;
            }
            return false;
        },
        _removeSelected: function (rowdata) {
            var g = this,
                p = this.options;
            if (p.tree) {
                var children = g.getChildren(rowdata, true);
                if (children) {
                    for (var i = 0, l = children.length; i < l; i++) {
                        var index2 = $.inArray(children[i], g.selected);
                        if (index2 != -1) g.selected.splice(index2, 1);
                    }
                }
            }
            var index = $.inArray(rowdata, g.selected);
            if (index != -1) g.selected.splice(index, 1);
        },
        _getParentChildren: function (rowParm) {
            var g = this,
                p = this.options;
            var rowdata = g.getRow(rowParm);
            var listdata;
            if (p.tree && g.existRecord(rowdata) && rowdata['__pid'] in g.records) {
                listdata = g.records[rowdata['__pid']][p.tree.childrenName];
            } else {
                listdata = g.currentData[p.root];
            }
            return listdata;
        },
        _removeData: function (rowdata) {
            var g = this,
                p = this.options;
            var listdata = g._getParentChildren(rowdata);
            var index = $.inArray(rowdata, listdata);
            if (index != -1) {
                listdata.splice(index, 1);
            }
            g._removeSelected(rowdata);
        },
        _addData: function (rowdata, parentdata, neardata, isBefore) {
            var g = this,
                p = this.options;
            if (!g.currentData) g.currentData = {};
            if (!g.currentData[p.root]) g.currentData[p.root] = [];
            var listdata = g.currentData[p.root];
            if (neardata) {
                if (p.tree) {
                    if (parentdata)
                        listdata = parentdata[p.tree.childrenName];
                    else if (neardata['__pid'] in g.records)
                        listdata = g.records[neardata['__pid']][p.tree.childrenName];
                }
                var index = $.inArray(neardata, listdata);
                listdata.splice(index == -1 ? -1 : index + (isBefore ? 0 : 1), 0, rowdata);
            } else {
                if (p.tree && parentdata) {
                    if (!parentdata[p.tree.childrenName])
                        parentdata[p.tree.childrenName] = [];
                    listdata = parentdata[p.tree.childrenName];
                }
                listdata.push(rowdata);
            }
        },
        //移动数据(树)
        //parm [parentdata] 附加到哪一个节点下级
        //parm [neardata] 附加到哪一个节点的上方/下方
        //parm [isBefore] 是否附加到上方
        _appendData: function (rowdata, parentdata, neardata, isBefore) {
            var g = this,
                p = this.options;
            rowdata[p.statusName] = "update";
            g._removeData(rowdata);
            g._addData(rowdata, parentdata, neardata, isBefore);
        },
        appendRange: function (rows, parentdata, neardata, isBefore) {
            var g = this,
                p = this.options;
            var toRender = false;
            $.each(rows, function (i, item) {
                if (item['__id'] && g.existRecord(item)) {
                    if (g.isLeaf(parentdata)) g.upgrade(parentdata);
                    g._appendData(item, parentdata, neardata, isBefore);
                    toRender = true;
                } else {
                    g.appendRow(item, parentdata, neardata, isBefore);
                }
            });
            if (toRender) g.reRender();

        },
        appendRow: function (rowdata, parentdata, neardata, isBefore) {
            var g = this,
                p = this.options;
            if ($.isArray(rowdata)) {
                g.appendRange(rowdata, parentdata, neardata, isBefore);
                return;
            }
            if (rowdata['__id'] && g.existRecord(rowdata)) {
                g._appendData(rowdata, parentdata, neardata, isBefore);
                g.reRender();
                return;
            }
            if (parentdata && g.isLeaf(parentdata)) g.upgrade(parentdata);
            g.addRow(rowdata, neardata, isBefore ? true : false, parentdata);
        },

        upgrade: function (rowParm) {
            var g = this,
                p = this.options;
            var rowdata = g.getRow(rowParm);
            if (!rowdata || !p.tree) return;
            rowdata[p.tree.childrenName] = rowdata[p.tree.childrenName] || [];
            rowdata['__hasChildren'] = true;
            var rowobjs = [g.getRowObj(rowdata)];
            if (g.enabledFrozen()) rowobjs.push(g.getRowObj(rowdata, true));
            $("> td > div > .lee-grid-tree-space:last", rowobjs).addClass("lee-grid-tree-link lee-grid-tree-link-open");
        },
        demotion: function (rowParm) {
            var g = this,
                p = this.options;
            var rowdata = g.getRow(rowParm);
            if (!rowdata || !p.tree) return;
            var rowobjs = [g.getRowObj(rowdata)];
            if (g.enabledFrozen()) rowobjs.push(g.getRowObj(rowdata, true));
            $("> td > div > .lee-grid-tree-space:last", rowobjs).removeClass("lee-grid-tree-link lee-grid-tree-link-open lee-grid-tree-link-close");
            if (g.hasChildren(rowdata)) {
                var children = g.getChildren(rowdata);
                for (var i = 0, l = children.length; i < l; i++) {
                    g.deleteRow(children[i]);
                }
            }
            rowdata['__hasChildren'] = false;
        },

        collapseAll: function () {
            var g = this,
                p = this.options;
            $(g.rows).each(function (rowIndex, rowParm) {
                var targetRowObj = g.getRowObj(rowParm);
                var linkbtn = $(".lee-grid-tree-link", targetRowObj);
                if (linkbtn.hasClass("lee-grid-tree-link-close")) return;
                g.toggle(rowParm);
            });
        },
        expandAll: function () {
            var g = this,
                p = this.options;
            $(g.rows).each(function (rowIndex, rowParm) {
                var targetRowObj = g.getRowObj(rowParm);
                var linkbtn = $(".lee-grid-tree-link", targetRowObj);
                if (linkbtn.hasClass("lee-grid-tree-link-open")) return;
                g.toggle(rowParm);
            });
        },

        collapse: function (rowParm) {
            var g = this,
                p = this.options;
            var targetRowObj = g.getRowObj(rowParm);
            var linkbtn = $(".lee-grid-tree-link", targetRowObj);
            if (linkbtn.hasClass("lee-grid-tree-link-close")) return;
            g.toggle(rowParm);
        },
        expand: function (rowParm) {
            var g = this,
                p = this.options;
            var targetRowObj = g.getRowObj(rowParm);
            var linkbtn = $(".lee-grid-tree-link", targetRowObj);
            if (linkbtn.hasClass("lee-grid-tree-link-open")) return;
            g.toggle(rowParm);
        },
        toggle: function (rowParm) {
            if (!rowParm) return;
            var g = this,
                p = this.options;
            var rowdata = g.getRow(rowParm);
            var targetRowObj = [g.getRowObj(rowdata)];
            if (g.enabledFrozen()) targetRowObj.push(g.getRowObj(rowdata, true));
            var level = rowdata['__level'],
                indexInCollapsedRows;
            var linkbtn = $(".lee-grid-tree-link:first", targetRowObj);
            var opening = true;
            g.collapsedRows = g.collapsedRows || [];
            var isToExpanding = linkbtn.hasClass("lee-grid-tree-link-close");

            function toggleChildren(items) {
                for (var i = 0, l = items.length; i < l; i++) {
                    var o = items[i];
                    var subchildren = g.getChildren(o, false);
                    var haschildren = subchildren && subchildren.length >= 0 ? true : false;
                    var currentRow = $([g.getRowObj(o['__id'])]);
                    if (g.enabledFrozen()) currentRow = currentRow.add(g.getRowObj(o['__id'], true));
                    console.log("subchildren:" + subchildren);
                    if (haschildren) {
                        //如果这个子节点原来是折叠状态的,那么子节点的子节点不做处理
                        if ($(".lee-grid-tree-link", currentRow).hasClass("lee-grid-tree-link-close")) {
                            currentRow.show();
                        }
                        //如果是展开状态的
                        else {
                            currentRow.show();
                            toggleChildren(subchildren);
                        }
                    } else {
                        $(".lee-grid-tree-link", currentRow).removeClass("lee-grid-tree-link-close").addClass("lee-grid-tree-link-open");
                        currentRow.show();
                    }

                }
            }

            function update() {
                var children = [];
                //折叠,那么直接隐藏所有子节点即可
                if (!opening) {
                    children = g.getChildren(rowdata, true);
                    $(children).each(function () {
                        $(g.getRowObj(this)).hide();
                        if (g.enabledFrozen()) $(g.getRowObj(this, true)).hide();
                    });
                    g.trigger('treeCollapsed', [rowdata]);
                    return;
                }

                //展开,下面逻辑处理(选择性递归)
                children = g.getChildren(rowdata, false);

                toggleChildren(children);

                g.trigger('treeExpanded', [rowdata]);
            }

            if (isToExpanding) //展开
            {
                function linkbtn_expand() {
                    linkbtn.removeClass("lee-grid-tree-link-close").addClass("lee-grid-tree-link-open");
                    indexInCollapsedRows = $.inArray(rowdata, g.collapsedRows);
                    delete rowdata["__hideChild"]
                    if (indexInCollapsedRows != -1) g.collapsedRows.splice(indexInCollapsedRows, 1);
                }
                var e = {
                    update: function () {
                        linkbtn_expand();
                        update();
                    }
                };
                if (g.hasBind('treeExpand') && g.trigger('treeExpand', [rowdata, e]) == false) return false;
                linkbtn_expand();
            } else { //折叠
                function linkbtn_collapse() {
                    linkbtn.addClass("lee-grid-tree-link-close").removeClass("lee-grid-tree-link-open");
                    indexInCollapsedRows = $.inArray(rowdata, g.collapsedRows);
                    rowdata["__hideChild"] = true;
                    if (indexInCollapsedRows == -1) g.collapsedRows.push(rowdata);
                }
                var e = {
                    update: function () {
                        linkbtn_collapse();
                        update();
                    }
                };
                if (g.hasBind('treeCollapse') && g.trigger('treeCollapse', [rowdata, e]) == false) return false;
                opening = false;
                linkbtn_collapse();
            }

            update();
            if (p.virtualScroll) g.triggerTreeCollapse();

        },
        _bulid: function () {
            var g = this;
            g._clearGrid();
            //创建头部
            g._initBuildHeader();
            //宽度高度初始化
            g._initHeight();
            //创建底部工具条
            g._initFootbar();
            //创建分页
            g._buildPager();
            //创建事件
            g._setEvent();
        },
        _setColumns: function (columns) {
            var g = this;
            //初始化列
            g._initColumns();
            //创建表头
            g._initBuildGridHeader();
            //创建 显示/隐藏 列 列表
            g._initBuildPopup();
        },
        _initBuildHeader: function () {
            var g = this,
                p = this.options;
            if (p.title) {
                $(".lee-panel-header-text", g.header).html(p.title);
                if (p.headerImg)
                    g.header.append("<img src='" + p.headerImg + "' />").addClass("lee-panel-header-hasicon");
            } else {
                g.header.hide();
            }
            //			if(p.toolbar) {
            //				if($.fn.leeToolBar) {
            //					g.topbarManager = g.topbar.leeToolBar(p.toolbar);
            //					//原语句不知道为什么, toolbar的父div高度为0. 导致样式有问题. 
            //					if(g.topbar.height() == 0)
            //						g.topbar.parent().height(25);
            //					else
            //						g.topbar.parent().height(g.topbar.height());
            //				}
            //			} else {
            //				g.topbar.parent().remove();
            //			}
        },
        _createColumnId: function (column) {
            if (column.id != null && column.id != "") return column.id.toString();
            return "c" + (100 + this._columnCount);
        },
        _isColumnId: function (str) {
            return (str in this._columns);
        },
        _initColumns: function () {
            var g = this,
                p = this.options;
            g._columns = {}; //全部列的信息  
            g._columnCount = 0;
            g._columnLeafCount = 0;
            g._columnMaxLevel = 1;
            if (!p.columns) return;

            function removeProp(column, props) {
                for (var i in props) {
                    if (props[i] in column)
                        delete column[props[i]];
                }
            }
            //设置id、pid、level、leaf，返回叶节点数,如果是叶节点，返回1
            function setColumn(column, level, pid, previd) {
                if (column.editorType) {
                    column.editor = column.editor || {};
                    column.editor.type = column.editorType;
                }
                removeProp(column, ['__id', '__pid', '__previd', '__nextid', '__domid', '__leaf', '__leafindex', '__level', '__colSpan', '__rowSpan']);
                if (level > g._columnMaxLevel) g._columnMaxLevel = level;
                g._columnCount++;
                column['__id'] = g._createColumnId(column);
                column['__domid'] = g.id + "|hcell|" + column['__id'];
                g._columns[column['__id']] = column;
                if (!column.columns || !column.columns.length)
                    column['__leafindex'] = g._columnLeafCount++;
                column['__level'] = level;
                column['__pid'] = pid;
                column['__previd'] = previd;
                if (!column.columns || !column.columns.length) {
                    column['__leaf'] = true;
                    return 1;
                }
                var leafcount = 0;
                var newid = -1;
                for (var i = 0, l = column.columns.length; i < l; i++) {
                    var col = column.columns[i];
                    leafcount += setColumn(col, level + 1, column['__id'], newid);
                    newid = col['__id'];
                }
                column['__leafcount'] = leafcount;
                return leafcount;
            }
            var lastid = -1;
            //行序号
            if (p.rownumbers) {
                var frozenRownumbers = g.enabledGroup() ? false : p.frozen && p.frozenRownumbers;
                var col = {
                    isrownumber: true,
                    issystem: true,
                    width: p.rownumbersColWidth,
                    frozen: frozenRownumbers
                };
                setColumn(col, 1, -1, lastid);
                lastid = col['__id'];
            }
            //明细列
            if (g.enabledDetail()) {
                var frozenDetail = g.enabledGroup() ? false : p.frozen && p.frozenDetail;
                var col = {
                    isdetail: true,
                    issystem: true,
                    width: p.detailColWidth,
                    frozen: frozenDetail
                };
                setColumn(col, 1, -1, lastid);
                lastid = col['__id'];
            }
            //复选框列
            if (g.enabledCheckbox()) {
                var frozenCheckbox = g.enabledGroup() ? false : p.frozen && p.frozenCheckbox;
                var col = {
                    ischeckbox: true,
                    issystem: true,
                    width: p.detailColWidth,
                    frozen: frozenCheckbox
                };
                setColumn(col, 1, -1, lastid);
                lastid = col['__id'];
            }
            for (var i = 0, l = p.columns.length; i < l; i++) {
                var col = p.columns[i];
                //增加以下一句. 因为在低版本IE中, 可能因为修改了prototype, 
                //导致这里取出undefined, 并进一步导致后续的函数调用出错. 
                if (!col) continue;
                setColumn(col, 1, -1, lastid);
                lastid = col['__id'];
            }
            //设置colSpan和rowSpan
            for (var id in g._columns) {
                var col = g._columns[id];
                if (col['__leafcount'] > 1) {
                    col['__colSpan'] = col['__leafcount'];
                }
                if (col['__leaf'] && col['__level'] != g._columnMaxLevel) {
                    col['__rowSpan'] = g._columnMaxLevel - col['__level'] + 1;
                }
            }
            //叶级别列的信息  
            g.columns = g.getColumns();
            $(g.columns).each(function (i, column) {
                column.columnname = column.name;
                column.columnindex = i;
                column.type = column.type || "string";
                column.islast = i == g.columns.length - 1;
                column.isSort = column.isSort == false ? false : true;
                column.frozen = column.frozen ? true : false;
                column._width = g._getColumnWidth(column);
                column._hide = column.hide ? true : false;
            });
        },
        _getColumnWidth: function (column) {
            var g = this,
                p = this.options;
            if (column._width) return column._width;
            var colwidth = column.width || p.columnWidth;
            if (!colwidth || colwidth == "auto") {
                var autoColumnNumber = 0,
                    noAutoColumnWidth = 0;
                $(g.columns).each(function (i, col) {
                    var colwidth = col.width || p.columnWidth;
                    var isAuto = (!colwidth || colwidth == "auto") ? true : false;
                    if (isAuto) autoColumnNumber++;
                    else noAutoColumnWidth += (parseInt(g._getColumnWidth(col)) + 1);
                });
                colwidth = parseInt((g.grid.width() - noAutoColumnWidth - 20) / autoColumnNumber) - 1;
            }
            if (typeof (colwidth) == "string" && colwidth.indexOf('%') > 0) {
                /*
				 * 修复行控件宽度被忽略的bug
				 */
                var lwidth = 0;
                if (g.enabledDetail()) {
                    lwidth += p.detailColWidth;
                }
                if (g.enabledCheckbox()) {
                    lwidth += p.checkboxColWidth;
                }
                if (g.options.rownumbers) {
                    lwidth += g.options.rownumbersColWidth;
                }
                column._width = colwidth = parseInt(parseInt(colwidth) * 0.01 * (g.grid.width() - lwidth - (g.columns.length / 2) - 1));
            }
            if (column.minWidth && colwidth < column.minWidth) colwidth = column.minWidth;
            if (column.maxWidth && colwidth > column.maxWidth) colwidth = column.maxWidth;
            return colwidth;
        },
        _createHeaderCell: function (column) {
            var g = this,
                p = this.options;
            var jcell = $("<td class='lee-grid-hd-cell'><div class='lee-grid-hd-cell-inner'><span class='lee-grid-hd-cell-text'></span></div></td>");
            jcell.attr("id", column['__domid']);
            if (!column['__leaf'])
                jcell.addClass("lee-grid-hd-cell-mul");


            else if (p.showHeaderFilter) {
                jcell.find(".lee-grid-hd-cell-inner").css("padding-bottom", p.filterRowHeight + "px");
                var id = g.id + "_filter_" + column["name"];
                jcell.append("<div class='lee-grid-cell-filter' style='border-top: 1px solid #DDD;height: " + p.filterRowHeight + "px;'><input id='" + id + "' class='input_filter' col='" + column["name"] + "' type='text'  style='width: 100%;border:0;height: 100%;outline: none;'></div>");
                jcell.find(".input_filter").keyup(function (e) {
                    if (typeof p.beforeHeaderFilter == "function") p.beforeHeaderFilter(g, p);
                    g.loadLocalData($(this).attr("col"), $(this).val());
                });
                jcell.find(".input_filter").change(function (e) {
                    if (typeof p.beforeHeaderFilter == "function") p.beforeHeaderFilter(g, p);
                    g.loadLocalData($(this).attr("col"), $(this).val());
                });

                //jcell.find(".input_filter").leeDropDown({});
            }
            if (column.columnindex == g.columns.length - 1) {
                jcell.addClass("lee-grid-hd-cell-last");
            }
            if (column.isrownumber) {
                jcell.addClass("lee-grid-hd-cell-rownumbers");
                jcell.html("<div class='lee-grid-hd-cell-inner'></div>");
            }
            if (column.ischeckbox) {
                jcell.addClass("lee-grid-hd-cell-checkbox");
                jcell.html("<div class='lee-grid-hd-cell-inner'><div class='lee-grid-hd-cell-text lee-grid-hd-cell-btn-checkbox'></div></div>");
            }
            if (column.isdetail) {
                jcell.addClass("lee-grid-hd-cell-detail");
                jcell.html("<div class='lee-grid-hd-cell-inner'><div class='lee-grid-hd-cell-text lee-grid-hd-cell-btn-detail'></div></div>");
            }
            if (column.heightAlign) {
                $(".lee-grid-hd-cell-inner:first", jcell).css("textAlign", column.heightAlign);
            }
            if (column['__colSpan']) jcell.attr("colSpan", column['__colSpan']);
            if (column['__rowSpan']) {
                jcell.attr("rowSpan", column['__rowSpan']);
                jcell.height(p.headerRowHeight * column['__rowSpan']);
                var paddingTop = (p.headerRowHeight * column['__rowSpan'] - p.headerRowHeight) / 2 - 5;
                $(".lee-grid-hd-cell-inner:first", jcell).css("paddingTop", paddingTop);
            } else {


                jcell.height(p.headerRowHeight);

            }
            if (column['__leaf']) {
                jcell.width(column['_width']);
                jcell.attr("columnindex", column['__leafindex']);

                if (p.showHeaderFilter)
                    jcell.height(p.headerRowHeight + p.filterRowHeight);

            }
            var cellHeight = jcell.height();
            if (!column['__rowSpan'] && cellHeight > 10)
                //$(">div:first", jcell).height(cellHeight);//设置div的高度
                if (column._hide) jcell.hide();
            if (column.name) jcell.attr({
                columnname: column.name
            });
            var headerText = "";
            if (column.display && column.display != "")
                headerText = column.display;
            else if (column.headerRender)
                headerText = column.headerRender(column);
            else
                headerText = "&nbsp;";
            if (column.required)
                headerText = "<span style='color:red'>*</span>" + headerText;
            $(".lee-grid-hd-cell-text:first", jcell).html(headerText);
            if (!column.issystem && column['__leaf'] && column.resizable !== false && $.fn.leeResizable && p.allowAdjustColWidth) {
                g.colResizable[column['__id']] = jcell.leeResizable({
                    handles: 'e',
                    onStartResize: function (e, ev) {
                        this.proxy.hide();
                        g.draggingline.css({
                            height: g.body.height(),
                            top: 0,
                            left: ev.pageX - g.grid.offset().left + parseInt(g.body[0].scrollLeft)
                        }).show();
                    },
                    onResize: function (e, ev) {
                        g.colresizing = true;
                        g.draggingline.css({
                            left: ev.pageX - g.grid.offset().left + parseInt(g.body[0].scrollLeft)
                        });
                        $('body').add(jcell).css('cursor', 'e-resize');
                    },
                    onStopResize: function (e) {
                        g.colresizing = false;
                        $('body').add(jcell).css('cursor', 'default');
                        g.draggingline.hide();
                        g.setColumnWidth(column, parseInt(column._width) + e.diffX);
                        g._fixHeight.leeDefer(g, 10);
                        return false;
                    }
                });
            }
            g.trigger('headerCellBulid', [jcell, column]);
            return jcell;
        },
        loadLocalData: function (colname, val) {
            var g = this, p = this.options;
            throttle(f_getWhere, 400)(colname, val)
            function f_getWhere(colname, val) {
                for (var i in g.rows) {
                    var rowdata = g.rows[i];
                    var row = $(g.getRowObj(rowdata));
                    var row2 = $(g.getRowObj(rowdata, true));
                    if (String(rowdata[colname]).indexOf(val) > -1) {
                        row.removeClass("row_hide");
                        row2.removeClass("row_hide");
                    } else {
                        row.addClass("row_hide");
                        row2.addClass("row_hide");

                    }
                }
            }
            function throttle(func, wait, options) {
                var context, args, result;
                var timeout = null;
                // 上次执行时间点
                var previous = 0;
                if (!options) options = {};
                // 延迟执行函数
                var later = function () {
                    // 若设定了开始边界不执行选项，上次执行时间始终为0
                    previous = options.leading === false ? 0 : new Date().getTime();
                    timeout = null;
                    result = func.apply(context, args);
                    if (!timeout) context = args = null;
                };
                return function () {
                    var now = new Date().getTime();
                    // 首次执行时，如果设定了开始边界不执行选项，将上次执行时间设定为当前时间。
                    if (!previous && options.leading === false) previous = now;
                    // 延迟执行时间间隔
                    var remaining = wait - (now - previous);
                    context = this;
                    args = arguments;
                    // 延迟时间间隔remaining小于等于0，表示上次执行至此所间隔时间已经超过一个时间窗口
                    // remaining大于时间窗口wait，表示客户端系统时间被调整过
                    if (remaining <= 0 || remaining > wait) {
                        clearTimeout(timeout);
                        timeout = null;
                        previous = now;
                        result = func.apply(context, args);
                        if (!timeout) context = args = null;
                        //如果延迟执行不存在，且没有设定结尾边界不执行选项
                    } else if (!timeout && options.trailing !== false) {
                        timeout = setTimeout(later, remaining);
                    }
                    return result;
                };
            }
        },
        _initBuildGridHeader: function () {
            var g = this,
                p = this.options;
            g.gridtablewidth = 0;
            g.f.gridtablewidth = 0;
            if (g.colResizable) {
                for (var i in g.colResizable) {
                    g.colResizable[i].destroy();
                }
                g.colResizable = null;
            }
            g.colResizable = {};
            $("tbody:first", g.gridheader).html("");
            $("tbody:first", g.f.gridheader).html("");

            if (p.autoColWidth) {
                var cols = g.getColumns();
                var tr = $("<colgroup></colgroup>");
                $(".lee-grid-header-table", g.gridheader).append(tr);
                $(cols).each(function (i, column) {
                    if (!column.frozen) {
                        var colcell = $("<col></col>");
                        colcell.attr("id", column['__domid'] + "_col");
                        tr.append(colcell);
                        colcell.width(column['_width']);
                    }
                });
            }

            for (var level = 1; level <= g._columnMaxLevel; level++) {


                //这里应该增加colgroup
                var columns = g.getColumns(level); //获取level层次的列集合
                var islast = level == g._columnMaxLevel; //是否最末级
                var tr = $("<tr class='lee-grid-hd-row'></tr>");
                var trf = $("<tr class='lee-grid-hd-row'></tr>");
                if (!islast) tr.add(trf).addClass("lee-grid-hd-mul");
                $("tbody:first", g.gridheader).append(tr);
                $("tbody:first", g.f.gridheader).append(trf);



                $(columns).each(function (i, column) {
                    (column.frozen ? trf : tr).append(g._createHeaderCell(column));
                    if (column['__leaf']) {
                        var colwidth = column['_width'];

                        if (!column._hide) {
                            if (!column.frozen)
                                g.gridtablewidth += (parseInt(colwidth) ? parseInt(colwidth) : 0) + 1;
                            else
                                g.f.gridtablewidth += (parseInt(colwidth) ? parseInt(colwidth) : 0) + 1;
                        }
                    }
                });
            }
            if (g._columnMaxLevel > 0) {
                var h = p.headerRowHeight * g._columnMaxLevel;
                if (p.showHeaderFilter) {
                    h += p.filterRowHeight;
                }
                g.gridheader.add(g.f.gridheader).height(h);
                if (p.rownumbers && p.frozenRownumbers) g.f.gridheader.find("td:first").height(h);
            }
            g._updateFrozenWidth();
            $("div:first", g.gridheader).width(g.gridtablewidth + 40);
        },
        _initBuildPopup: function () {
            var g = this,
                p = this.options;
            $(':checkbox', g.popup).unbind();
            $('tbody tr', g.popup).remove();
            $(g.columns).each(function (i, column) {
                if (column.issystem) return;
                if (column.isAllowHide == false) return;
                var chk = 'checked="checked"';
                if (column._hide) chk = '';
                var header = column.display;
                $('tbody', g.popup).append('<tr><td class="l-column-left"\n><input type="checkbox" ' + chk + ' class="l-checkbox" columnindex="' + i + '"/></td><td class="l-column-right">' + header + '</td></tr>');
            });
            if ($.fn.leeCheckBox) {
                $('input:checkbox', g.popup).leeCheckBox({
                    onBeforeClick: function (obj) {
                        if (!obj.checked) return true;
                        if ($('input:checked', g.popup).length <= p.minColToggle)
                            return false;
                        return true;
                    }
                });
            }
            //表头 - 显示/隐藏'列控制'按钮事件
            if (p.allowHideColumn) {
                $('tr', g.popup).hover(function () {
                    $(this).addClass('l-popup-row-over');
                },
                    function () {
                        $(this).removeClass('l-popup-row-over');
                    });
                var onPopupCheckboxChange = function () {
                    if ($('input:checked', g.popup).length + 1 <= p.minColToggle) {
                        return false;
                    }
                    g.toggleCol(parseInt($(this).attr("columnindex")), this.checked, true);
                };
                if ($.fn.leeCheckBox)
                    $(':checkbox', g.popup).bind('change', onPopupCheckboxChange);
                else
                    $(':checkbox', g.popup).bind('click', onPopupCheckboxChange);
            }
        },
        _initHeight: function () {
            var g = this,
                p = this.options;
            if (p.height == 'auto') {
                g.gridbody.height('auto');
                g.f.gridbody.height('auto');
            }
            if (p.width) {
                g.grid.width(p.width);
            }
            g._onResize.call(g);
        },
        _initFootbar: function () {
            var g = this,
                p = this.options;
            if (p.usePager) {
                if (p.pagerRender) {
                    g.toolbar.html(p.pagerRender.call(g));
                    return;
                }
                if (p.scrollToPage) {
                    g.toolbar.hide();
                    return;
                }
                //创建底部工具条 - 选择每页显示记录数
                var optStr = "";
                var selectedIndex = -1;
                $(p.pageSizeOptions).each(function (i, item) {
                    var selectedStr = "";
                    if (p.pageSize == item) selectedIndex = i;
                    optStr += "<option value='" + item + "' " + selectedStr + " >" + item + "</option>";
                });
                //todo
                $('.selectpagesize', g.toolbar).append("<select name='rp'>" + optStr + "</select>");
                if (selectedIndex != -1) $('.selectpagesize select', g.toolbar)[0].selectedIndex = selectedIndex;
                if (p.switchPageSizeApplyComboBox && $.fn.leeDropDown) {
                    $(".selectpagesize select", g.toolbar).leeDropDown({
                        onBeforeSelect: function () {
                            if (p.url && g.isDataChanged && !confirm(p.isContinueByDataChanged))
                                return false;
                            return true;
                        },
                        width: 45
                    });
                }
            } else {
                g.toolbar.hide();
            }
        },
        _searchData: function (data, clause) {
            var g = this,
                p = this.options;
            var newData = new Array();
            for (var i = 0; i < data.length; i++) {
                if (clause(data[i], i)) {
                    newData[newData.length] = data[i];
                }
            }
            return newData;
        },
        _clearGrid: function (sourceType) {
            var g = this,
                p = this.options;
            g._fixRows();
            for (var i in g.rows) {
                var rowobj = $(g.getRowObj(g.rows[i]));
                if (g.enabledFrozen())
                    rowobj = rowobj.add(g.getRowObj(g.rows[i], true));
                rowobj.unbind();
            }
            //清空数据
            g.gridbody.html("");
            g.f.gridbody.html("");
            g.recordNumber = 0;
            g.records = {};
            g.rows = [];
            g._fixRows();
            //清空选择的行
            if (sourceType != "virtualScroll")
                g.selected = [];
            g.totalNumber = 0;
            //编辑器计算器
            g.editorcounter = 0;
        },
        _fixRows: function () {
            var g = this,
                p = this.options;
            if (!g.rows) return;
            for (var i in g.rows) {
                if ($.isFunction(g.rows[i])) {
                    delete g.rows[i];
                }
            }
        },

        _fillGridBody: function (data, frozen, sourceType) {
            var g = this,
                p = this.options;
            //加载数据 
            var gridhtmlarr = sourceType == "scrollappend" ? [] : ['<div class="lee-grid-body-inner"><table class="lee-grid-body-table" cellpadding=0 cellspacing=0><tbody>'];


            if (p.autoColWidth && !frozen) {
                var cols = g.getColumns();
                gridhtmlarr.push("<colgroup>");

                $(cols).each(function (i, column) {
                    if (!column.frozen) {
                        var id = column['__domid'] + "_detail";
                        gridhtmlarr.push("<col id='" + id + "' style='width:" + column['_width'] + "px;'></col>");
                    }
                });
                gridhtmlarr.push("</colgroup>");
            }


            if (g.enabledGroup()) //启用分组模式
            {
                var groups = []; //分组列名数组
                var groupsdata = []; //切成几块后的数据
                g.groups = groupsdata;
                for (var rowparm in data) {
                    var item = data[rowparm];
                    var groupColumnValue = item[p.groupColumnName];
                    var valueIndex = $.inArray(groupColumnValue, groups);
                    if (valueIndex == -1) {
                        groups.push(groupColumnValue);
                        valueIndex = groups.length - 1;
                        groupsdata.push([]);
                    }
                    groupsdata[valueIndex].push(item);
                }
                $(groupsdata).each(function (i, item) {
                    if (groupsdata.length == 1)
                        gridhtmlarr.push('<tr class="lee-grid-grouprow lee-grid-grouprow-last lee-grid-grouprow-first"');
                    if (i == groupsdata.length - 1)
                        gridhtmlarr.push('<tr class="lee-grid-grouprow lee-grid-grouprow-last"');
                    else if (i == 0)
                        gridhtmlarr.push('<tr class="lee-grid-grouprow lee-grid-grouprow-first"');
                    else
                        gridhtmlarr.push('<tr class="lee-grid-grouprow"');
                    gridhtmlarr.push(' groupindex"=' + i + '" >');
                    gridhtmlarr.push('<td colSpan="' + g.columns.length + '" class="lee-grid-grouprow-cell">');
                    gridhtmlarr.push('<span class="lee-icon lee-grid-group-togglebtn"></span>');
                    if (p.groupRender)
                        gridhtmlarr.push(p.groupRender(groups[i], item, p.groupColumnDisplay));
                    else
                        gridhtmlarr.push(p.groupColumnDisplay + ':' + groups[i]);

                    gridhtmlarr.push('</td>');
                    gridhtmlarr.push('</tr>');

                    gridhtmlarr.push(g._getHtmlFromData(item, frozen));
                    //汇总
                    if (g.isTotalSummary())
                        gridhtmlarr.push(g._getTotalSummaryHtml(item, "lee-grid-totalsummary-group", frozen));
                });
            } else {
                gridhtmlarr.push(g._getHtmlFromData(data, frozen));
            }
            if (!sourceType == "scrollappend") {
                gridhtmlarr.push('</tbody></table></div>');
            }

            if (sourceType == "scrollappend") {
                (frozen ? g.f.gridbody : g.gridbody).find("tbody:first").append(gridhtmlarr.join(''));
            } else {
                (frozen ? g.f.gridbody : g.gridbody).html(gridhtmlarr.join(''));
            }

            if (frozen) {
                g.f.gridbody.find(">l-jplace").remove();
                g.f.gridbody.append('<div class="l-jplace"></div>');
            }

            if (p.usePager && p.scrollToPage && !p.scrollToAppend) {
                var jgridbodyinner = frozen ? g.f.gridbody.find("> .lee-grid-body-inner") : g.gridbody.find("> .lee-grid-body-inner");
                var jreplace = jgridbodyinner.find("> .l-scrollreplacetop");
                jreplace = jreplace.length ? jreplace : $('<div class="l-scrollreplacetop"></div>').prependTo(jgridbodyinner);
                jreplace.css("width", "80%").height(g.lastScrollTop);

                jreplace = jgridbodyinner.find("> .l-scrollreplacebottom");
                jreplace = jreplace.length ? jreplace : $('<div class="l-scrollreplacebottom"></div>').appendTo(jgridbodyinner);
                jreplace.css("width", "80%").height((p.pageCount - p.newPage) * g._getOnePageHeight());
            }
            //分组时不需要            
            if (!g.enabledGroup()) {
                //创建汇总行
                g._bulidTotalSummary(frozen);
            }
            $("> div:first", g.gridbody).width(g.gridtablewidth);
            g._onResize();
        },
        _showData: function (sourceType) {
            var g = this,
                p = this.options;
            g.changedCells = {};
            var data = g.currentData[p.root];
            if (p.usePager) {
                //更新总记录数
                if (p.dataAction == "server" && g.data && g.data[p.record])
                    p.total = g.data[p.record];
                else if (g.filteredData && g.filteredData[p.root])
                    p.total = g.filteredData[p.root].length;
                else if (g.data && g.data[p.root])
                    p.total = g.data[p.root].length;
                else if (data)
                    p.total = data.length;

                p.page = p.newPage;
                if (!p.total) p.total = 0;
                if (!p.page) p.page = 1;
                p.pageCount = Math.ceil(p.total / p.pageSize);
                if (!p.pageCount) p.pageCount = 1;
                if (!p.scrollToPage) {
                    //更新分页
                    g._buildPager();
                }
            }
            //加载中
            $('.l-bar-btnloading:first', g.toolbar).removeClass('l-bar-btnloading');
            if (g.trigger('beforeShowData', [g.currentData]) == false) return;
            if (sourceType != "scrollappend") {
                g._clearGrid(sourceType);//不是追加数据则清空
            }
            g.isDataChanged = false;
            if (!data || !data.length) {
                g.gridview.addClass("lee-grid-empty");
                $(g.element).addClass("lee-empty");

                $("<div></div>").addClass("lee-grid-body-inner").appendTo(g.gridbody).css({
                    width: g.gridheader.find(">div:first").width() - 40,
                    height: "100%"
                });
                if (p.pagerRender) {
                    g.toolbar.html(p.pagerRender.call(g));
                    return;
                }
                g._onResize.leeDefer(g, 50);
                if (p.noDataRender) {
                    var top = 37;
                    top = p.headerRowHeight * g._columnMaxLevel + 1;
                    var html = "";
                    html = p.noDataRender.call(g);
                    if ($(".lee-grid-empty-wrap", g.gridview).length > 0) {
                        $(".lee-grid-empty-wrap", g.gridview).html(html).css("top", top);
                    } else {
                        $("<div>" + html + "</div>").addClass("lee-grid-empty-wrap").appendTo(g.gridview).css("top", top);
                    }
                }
                g.trigger('afterShowData', [g.currentData]);
                return;
            } else {
                g.gridview.removeClass("lee-grid-empty");
                $(g.element).removeClass("lee-empty");
                $(".lee-grid-empty-wrap", g.gridview).remove();
            }
            $(".l-bar-btnload:first span", g.toolbar).removeClass("l-disabled");
            g._updateGridData();
            if (g.enabledFrozen())
                g._fillGridBody(g.rows, true, sourceType);
            g._fillGridBody(g.rows, false, sourceType);
            g.trigger('SysGridHeightChanged');
            if (sourceType == "scroll") {
                g.trigger('sysScrollLoaded');
            }
            //汇总行
            if (p.totalRender) {
                $(".l-panel-bar-total", g.element).remove();
                $(".l-panel-bar", g.element).before('<div class="l-panel-bar-total">' + p.totalRender(g.data, g.filteredData) + '</div>');
            }
            //绑定鼠标事件
            if (p.mouseoverRowCssClass) {
                for (var i in g.rows) {
                    var rowobj = $(g.getRowObj(g.rows[i]));
                    if (g.enabledFrozen())
                        rowobj = rowobj.add(g.getRowObj(g.rows[i], true));
                    rowobj.bind('mouseover.gridrow', function () {
                        g._onRowOver(this, true);
                    }).bind('mouseout.gridrow', function () {
                        g._onRowOver(this, false);
                    });
                }
            }
            g._fixHeight();
            if (p.pagerRender) {
                g.toolbar.html(p.pagerRender.call(g));
                return;
            }
            //触发事件
            if (!p.virtualScroll) g.gridbody.trigger('scroll.grid');

            g.trigger('afterShowData', [g.currentData]);
        },
        _fixHeight: function () {
            var g = this,
                p = this.options;
            if (p.fixedCellHeight || !p.frozen) return;
            var column1, column2;
            for (var i in g.columns) {
                var column = g.columns[i];
                if (column1 && column2) break;
                if (column.frozen && !column1) {
                    column1 = column;
                    continue;
                }
                if (!column.frozen && !column2) {
                    column2 = column;
                    continue;
                }
            }
            if (!column1 || !column2) return;
            for (var rowid in g.records) {
                var cell1 = g.getCellObj(rowid, column1),
                    cell2 = g.getCellObj(rowid, column2);
                $(cell1).add(cell2).height("auto");
                var height = Math.max($(cell1).height(), ($(cell2).height()));
                $(cell1).add(cell2).height(height);
            }
        },
        _getRowDomId: function (rowdata, frozen) {
            return this.id + "|" + (frozen ? "1" : "2") + "|" + rowdata['__id'];
        },
        _getCellDomId: function (rowdata, column) {
            return this._getRowDomId(rowdata, column.frozen) + "|" + column['__id'];
        },
        refreshScroll: function () {
            var g = this,
                p = this.options;
            g.setScrollHeight();
            g.mainLength = Math.ceil(g.scrollheight / p.rowHeight);//可视区域
            g.refreshOffset(g.gridbody.scrollTop())
        },
        refreshOffset: function (toph) {
            var g = this,
                p = this.options;
            g.startIndex = Math.floor(toph / p.rowHeight);
            g.startIndex = g.startIndex - g.offsetLen / 2;
            if (g.startIndex < 0) g.startIndex = 0;
            g.endIndex = g.offsetLen + g.mainLength + g.startIndex;
            g.toph = toph;
        },
        onScroll: function (toph) {
            var g = this,
                p = this.options;
            if (!g.offsetLen) g.initScrollSetting();
            var off = (Math.floor(toph / p.rowHeight) - g.startIndex);
            if (off > g.offsetLen || off < 0) {
                g.refreshOffset(toph);
                g._showData("virtualScroll");
                g.gridbody.scrollTop(toph);
                g.f.gridbody.scrollTop(toph);
            }

        },
        setScrollHeight: function () {
            var g = this,
                p = this.options;
            g.scrollheight = g.gridbody.height();
        },
        initScrollSetting: function () {
            var g = this,
                p = this.options;
            g.offsetLen = 10;//显示留白数据
            g.setScrollHeight();
            g.mainLength = Math.ceil(g.scrollheight / p.rowHeight);//可视区域
            g.startIndex = 0;
            g.endIndex = g.offsetLen + g.mainLength;

            g.totalHeight = 0;
            g.toph = 0;
            g.bottomHeight = g.totalHeight - (g.offsetLen + g.mainLength) * p.rowHeight - g.toph;
        },
        triggerTreeCollapse: function () {
            var g = this,
                p = this.options;
            var toph = g.gridbody.scrollTop();
            //var off = (Math.floor(toph / p.rowHeight) - g.startIndex);
            g.startIndex = Math.floor(toph / p.rowHeight);
            g.startIndex = g.startIndex - g.offsetLen / 2;
            if (g.startIndex < 0) g.startIndex = 0;
            g.endIndex = g.offsetLen + g.mainLength + g.startIndex;
            g.toph = toph;
            g._showData();
            g.gridbody.scrollTop(toph);
            g.f.gridbody.scrollTop(toph);
        },
        getCalcShowLength: function (data) {
            var g = this,
                p = this.options;
            var hiddenLength = 0;
            var start = 0;
            var middle = 0;
            var end = 0;
            for (var i = 0; i < data.length; i++) {
                var item = data[i];
                if (this._isRowHide(item)) {
                    hiddenLength++;
                    if (g.startIndex > i - start) {
                        start++;
                    } else if (g.startIndex <= i - start && g.endIndex > i - start - middle) {
                        middle++;
                    } else {
                        end++;
                    }
                }
            }

            return {
                start: start, all: hiddenLength, end: end, middle: middle
            };
        },
        _isRowHide: function (item) {
            var g = this,
                p = this.options;
            if (p.tree && g.collapsedRows && g.collapsedRows.length) {
                var isHide = function () {
                    var pitem = g.getParent(item);
                    while (pitem) {
                        if ($.inArray(pitem, g.collapsedRows) != -1) return true;
                        pitem = g.getParent(pitem);
                    }
                    return false;
                };
                return isHide();
            }
            else if (p.tree && p.tree.isExtend) {
                var isHide = function () {
                    var pitem = g.getParent(item);
                    while (pitem) {
                        if (p.tree.isExtend(pitem) == false) return true;
                        pitem = g.getParent(pitem);
                    }
                    return false;
                };
                return isHide();
            }
            return false;
        },
        _getHtmlFromData: function (data, frozen) {
            if (!data) return "";
            var g = this,
                p = this.options;
            var gridhtmlarr = [];
            var startIndex = 0;
            var endIndex = data.length;


            if (p.virtualScroll) {
                var res = g.getCalcShowLength(data);
                g.totalHeight = (data.length - res.all) * p.rowHeight;//去掉所有隐藏数据的行高
                if (g.endIndex > data.length) g.endIndex = data.length;
                gridhtmlarr.push("<tr style='height:" + g.startIndex * p.rowHeight + "px;'></tr>");

                var hideArr = [];
                var start = g.startIndex + res.start;
                endIndex = g.endIndex + res.middle + res.start;
                if (endIndex > data.length) endIndex = data.length;
                startIndex = g.startIndex + res.start;
            }
            //p.virtualScroll
            for (var i = startIndex; i < endIndex; i++) {
                var item = data[i];
                var rowid = item['__id'];
                if (!item) continue;

                if (p.virtualScroll && p.tree && g._isRowHide(item)) continue;
                gridhtmlarr.push('<tr');
                gridhtmlarr.push(' id="' + g._getRowDomId(item, frozen) + '"');
                gridhtmlarr.push(' class="lee-grid-row'); //class start 
                if ((!frozen && g.enabledCheckbox() && p.isChecked && p.isChecked(item)) || item["__selected"]) {
                    g.select(item);
                    gridhtmlarr.push(' lee-selected');
                } else if (g.isSelected(item)) {
                    gridhtmlarr.push(' lee-selected');
                } else if (p.isSelected && p.isSelected(item)) {
                    g.select(item);
                    gridhtmlarr.push(' lee-selected');
                }
                if (p.alternatingRow && item['__index'] % 2 == 1)
                    gridhtmlarr.push(' lee-grid-row-alt');
                //自定义css class
                if (p.rowClsRender) {
                    var rowcls = p.rowClsRender(item, rowid);
                    rowcls && gridhtmlarr.push(' ' + rowcls);
                }
                gridhtmlarr.push('" '); //class end
                if (p.rowAttrRender) gridhtmlarr.push(p.rowAttrRender(item, rowid));

                if (g._isRowHide(item)) gridhtmlarr.push(' style="display:none;" ');

                gridhtmlarr.push('>');
                $(g.columns).each(function (columnindex, column) {
                    if (frozen != column.frozen) return;
                    gridhtmlarr.push('<td');
                    gridhtmlarr.push(' id="' + g._getCellDomId(item, this) + '"');
                    //如果是行序号(系统列)
                    if (this.isrownumber) {
                        gridhtmlarr.push(' class="lee-grid-row-cell lee-grid-row-cell-rownumbers" style="width:' + this.width + 'px;height:' + p.rowHeight + 'px;"><div class="lee-grid-row-cell-inner"');
                        if (p.fixedCellHeight)
                            gridhtmlarr.push(' style = "" ');
                        else
                            gridhtmlarr.push(' style = "" ');
                        gridhtmlarr.push('>' + (parseInt(item['__index']) + 1) + '</div></td>');
                        return;
                    }
                    //如果是复选框(系统列)
                    if (this.ischeckbox) {
                        gridhtmlarr.push(' class="lee-grid-row-cell lee-grid-row-cell-checkbox" style="width:' + this.width + 'px;height:' + p.rowHeight + 'px;"><div class="lee-grid-row-cell-inner"');
                        if (p.fixedCellHeight)
                            gridhtmlarr.push(' style = "" ');
                        else
                            gridhtmlarr.push(' style = "" ');
                        gridhtmlarr.push('>');
                        gridhtmlarr.push('<span class="lee-grid-row-cell-btn-checkbox"></span>');
                        gridhtmlarr.push('</div></td>');
                        return;
                        //
                    }
                    //如果是明细列(系统列)
                    else if (this.isdetail) {
                        gridhtmlarr.push(' class="lee-grid-row-cell lee-grid-row-cell-detail" style="width:' + this.width + 'px;height:' + p.rowHeight + 'px;"><div class="lee-grid-row-cell-inner"');
                        if (p.fixedCellHeight)
                            gridhtmlarr.push(' style = "" ');
                        else
                            gridhtmlarr.push(' style = "" ');
                        gridhtmlarr.push('>');
                        if (!p.isShowDetailToggle || p.isShowDetailToggle(item)) {
                            gridhtmlarr.push('<span class="lee-grid-row-cell-detailbtn"></span>');
                        }
                        gridhtmlarr.push('</div></td>');
                        return;
                    }
                    var colwidth = this._width;
                    gridhtmlarr.push(' class="lee-grid-row-cell ');
                    if (g.changedCells[rowid + "_" + this['__id']]) gridhtmlarr.push("lee-grid-row-cell-edited ");
                    if (this.islast)
                        gridhtmlarr.push('lee-grid-row-cell-last ');
                    gridhtmlarr.push('"');
                    //if (this.columnname) gridhtmlarr.push('columnname="' + this.columnname + '"');
                    gridhtmlarr.push(' style = "');
                    gridhtmlarr.push('width:' + colwidth + 'px; ');
                    if (p.fixedCellHeight)
                        gridhtmlarr.push('height:' + p.rowHeight + 'px;');
                    //jjii
                    if (column._hide) {
                        gridhtmlarr.push('display:none;');
                    }
                    gridhtmlarr.push(' ">');
                    gridhtmlarr.push(g._getCellHtml(item, column));
                    gridhtmlarr.push('</td>');
                });
                gridhtmlarr.push('</tr>');
            }
            if (p.virtualScroll) {
                g.bottomHeight = g.totalHeight - (g.endIndex) * p.rowHeight;
                gridhtmlarr.push("<tr style='height:" + g.bottomHeight + "px;'></tr>");
            }
            return gridhtmlarr.join('');
        },
        _getCellHtml: function (rowdata, column) {
            var g = this,
                p = this.options;
            if (column.isrownumber)
                return '<div class="lee-grid-row-cell-inner">' + (parseInt(rowdata['__index']) + 1) + '</div>';
            var htmlarr = [];
            htmlarr.push('<div class="lee-grid-row-cell-inner"');
            //htmlarr.push('<div');
            //这里计算单元格内部的高度 暂且不算
            htmlarr.push(' style = "width:' + parseInt(column._width - 8) + 'px;');
            //htmlarr.push(' style = "');
            //if(p.fixedCellHeight) htmlarr.push('height:' + p.rowHeight + 'px;');
            //htmlarr.push('min-height:' + p.rowHeight + 'px; ');
            if (column.align) htmlarr.push('text-align:' + column.align + ';');
            var content = g._getCellContent(rowdata, column);
            htmlarr.push('">' + content + '</div>');
            return htmlarr.join('');
        },
        _setValueByName: function (data, name, value) {
            if (!data || !name) return null;
            if (name.indexOf('.') == -1) {
                data[name] = value;
            } else {
                try {
                    new Function("data,value", "data." + name + "=value;")(data, value);
                } catch (e) { }
            }
        },
        _getValueByName: function (data, name) {
            if (!data || !name) return null;
            if (name.indexOf('.') == -1) {
                return data[name];
            } else {
                try {
                    return new Function("data", "return data." + name + ";")(data);
                } catch (e) {
                    return null;
                }
            }
        },
        _getCellContent: function (rowdata, column) {
            var g = this,
                p = this.options;
            if (!rowdata || !column) return "";
            if (column.isrownumber) return parseInt(rowdata['__index']) + 1;
            var rowid = rowdata['__id'];
            var rowindex = rowdata['__index'];
            var value = g._getValueByName(rowdata, column.name);
            var text = g._getValueByName(rowdata, column.textField);
            var content = "";
            if (column.render) {
                content = column.render.call(g, rowdata, rowindex, value, column, g.rows.length);
            } else if (p.formatters[column.type]) {
                content = p.formatters[column.type].call(g, value, column);
            } else if (text != null) {
                content = text.toString();
            } else if (value != null) {
                content = value.toString();
            }
            if (p.tree && (p.tree.columnName != null && p.tree.columnName == column.name || p.tree.columnId != null && p.tree.columnId == column.id)) {
                content = g._getTreeCellHtml(content, rowdata);
            }
            return content || "";
        },
        _getTreeCellHtml: function (oldContent, rowdata) {
            var level = rowdata['__level'];
            var g = this,
                p = this.options;
            //var isExtend = p.tree.isExtend(rowdata);

            var isExtend = false;
            if (g.collapsedRows == null) {//如果没有收起来的列 那么 是否展开用isExtend函数
                isExtend = p.tree.isExtend(rowdata);
            } else {
                //不然返回当前行是否在收起的列里
                isExtend = $.inArray(rowdata, g.collapsedRows || []) == -1;
            }
            //alert(isExtend + rowdata.name)
            var isParent = p.tree.isParent(rowdata);
            var content = "";
            level = parseInt(level) || 1;
            for (var i = 1; i < level; i++) {
                content += "<div class='lee-grid-tree-space'></div>";
            }
            var hasAsyn = false;
            if (rowdata.children) {
                if (rowdata.children.length == 0) {//有children并且数组长度是0 那么则为异步记载
                    hasAsyn = true;
                }
            }

            if (isExtend && isParent && !hasAsyn)//如果是是父节点 并且是扩展 并且不是异步加载 那么则展开
            {
                //alert(1);
                content += "<div class='lee-icon lee-grid-tree-space lee-grid-tree-link lee-grid-tree-link-open'></div>";
            }
            else if (isParent) {
                //alert(2);
                content += "<div class='lee-icon lee-grid-tree-space lee-grid-tree-link lee-grid-tree-link-close'></div>";
            }
            else
                content += "<div class='lee-grid-tree-space'></div>";

            content += g._getIconHtml(rowdata);
            content += "<span class='lee-grid-tree-content'>" + oldContent + "</span>";
            return content;
        },
        _getIconHtml: function (rowdata) {

            var icon = rowdata['treeicon'];
            if (icon) {
                if (typeof icon == "function") {
                    return icon.call(this, rowdata); //这里直接返回样式
                } else if (typeof icon == "string") {
                    return "<i class='tree-grid-icon icon-img icon-img-" + icon + "'></i>";
                }
            }

            if (this.options.treeIconRender) {
                return this.options.treeIconRender.call(this, rowdata); //这里直接返回样式
            }
            return "";
        },
        _applyEditor: function (obj) {
            var g = this,
                p = this.options;
            var rowcell = obj,
                ids = rowcell.id.split('|');
            var columnid = ids[ids.length - 1],
                column = g._columns[columnid];
            var row = $(rowcell).parent(),
                rowdata = g.getRow(row[0]),
                rowid = rowdata['__id'],
                rowindex = rowdata['__index'];
            if (p.disabled) return;
            if (g.trigger('beforeApplyEditor', [rowindex, rowdata, column]) == false) return false;

            if (!column || !column.editor) return;

            var columnname = column.name,
                columnindex = column.columnindex;

            if (!column || column.readonly || !column.editor || column.editor.readonly) return;
            if (column.editor.type && p.editors[column.editor.type]) {
                var currentdata = g._getValueByName(rowdata, columnname);
                var editParm = {
                    record: rowdata,
                    value: currentdata,
                    column: column,
                    rowindex: rowindex
                };
                if (column.textField) editParm.text = g._getValueByName(rowdata, column.textField);
                if (g.trigger('beforeEdit', [editParm]) == false) return false;
                g.lastEditRow = rowdata;
                leeUI.lastEditGrid = g;
                var editor = p.editors[column.editor.type],
                    jcell = $(rowcell),
                    offset = $(rowcell).offset(),
                    width = $(rowcell).width(),
                    height = $(rowcell).height(),
                    container = $("<div class='lee-grid-editor'></div>").appendTo(jcell),
                    left = 0,
                    top = 0,
                    pc = jcell.position(),
                    pb = g.gridbody.position(),
                    pv = g.gridview2.position(),
                    //加上括号解决不能正常判定topBar的高度。不加括号会忽略后面运算出来的值
                    topbarHeight = (p.toolbar ? g.topbar.parent().outerHeight() : 0) + (p.title ? g.header.outerHeight() : 0),
                    left = pc.left + pb.left + pv.left,
                    top = pc.top + pb.top + pv.top + topbarHeight;

                top = (height - 23) / 2;

                left = 0;
                //编辑模式的编辑器创建在哪里为好？
                //jcell.html("");
                g.setCellEditing(rowdata, column, true);

                var isIE = (!!window.ActiveXObject || "ActiveXObject" in window) ? true : false,
                    isIE8 = false; // $.browser.version.indexOf('8') == 0;
                //				if(isIE) {
                //					height -= isIE8 ? 1 : 2;
                //					top -= 1;
                //					left -= 1;
                //				} else if($.browser.mozilla) {
                //					height -= 1;
                //					top -= 1;
                //					left -= 1;
                //				} else if($.browser.safari) {
                //					top -= 1;
                //				} else {
                //					height -= 1;
                //				}
                left += p.editorLeftDiff || 0;
                top += p.editorTopDiff || 0;
                //height += p.editorHeightDiff || 0;

                container
                    .css({
                        left: left,
                        top: top
                    })
                    .show();
                if (column.textField) editParm.text = g._getValueByName(rowdata, column.textField);
                var editorInput = g._createEditor(editor, container, editParm, width, height - 1);
                g.editor = {
                    editing: true,
                    editor: editor,
                    input: editorInput,
                    editParm: editParm,
                    container: container
                };
                g.unbind('sysEndEdit');
                g.bind('sysEndEdit', function () {
                    var newValue = editor.getValue(editorInput, editParm);
                    if (column.textField && editor.getText) {
                        editParm.text = editor.getText(editorInput, editParm);
                    }
                    if (editor.getSelected) {
                        editParm.selected = editor.getSelected(editorInput, editParm);
                    }
                    if (newValue != currentdata) {
                        $(rowcell).addClass("lee-grid-row-cell-edited");
                        g.changedCells[rowid + "_" + column['__id']] = true;
                        editParm.value = newValue;
                    }
                    if (column.editor.onChange) column.editor.onChange.call(editorInput, editParm, g);
                    if (g._checkEditAndUpdateCell(editParm)) {
                        if (column.editor.onChanged) column.editor.onChanged.call(editorInput, editParm, g); //触发列onchange事件
                    }
                });
                g.trigger('afterApplyEditor', [g.editor]);
            }
        },
        _checkEditAndUpdateCell: function (editParm) {
            //校验并且更新单元格
            var g = this,
                p = this.options;
            if (g.trigger('beforeSubmitEdit', [editParm]) == false) return false;
            var column = editParm.column;
            if (editParm.text != undefined && column.textField) g._setValueByName(editParm.record, column.textField, editParm.text);
            g.updateCell(column, editParm.value, editParm.record);
            if (column.render || g.enabledTotal()) g.reRender({
                column: column
            });
            g.reRender({
                rowdata: editParm.record
            });
            g.trigger('afterSubmitEdit', [editParm]);
            return true;
        },
        _getCurrentPageData: function (source) {
            var g = this,
                p = this.options;
            var data = {};
            data[p.root] = [];
            if (!source || !source[p.root] || !source[p.root].length) {
                data[p.record] = 0;
                return data;
            }
            data[p.record] = source[p.root].length;
            if (!p.newPage) p.newPage = 1;
            for (i = (p.newPage - 1) * p.pageSize; i < source[p.root].length && i < p.newPage * p.pageSize; i++) {
                data[p.root].push(source[p.root][i]);
            }
            return data;
        },
        //比较某一列两个数据
        _compareData: function (data1, data2, columnName, columnType) {
            var g = this,
                p = this.options;
            var val1 = data1[columnName],
                val2 = data2[columnName];
            if (val1 == null && val2 != null) return 1;
            else if (val1 == null && val2 == null) return 0;
            else if (val1 != null && val2 == null) return -1;
            if (p.sorters[columnType])
                return p.sorters[columnType].call(g, val1, val2);
            else
                return val1 < val2 ? -1 : val1 > val2 ? 1 : 0;
        },
        _getTotalInfo: function (column, data) {
            var g = this,
                p = this.options;
            try {
                var totalsummaryArr = [];
                if (!column.totalSummary) return null;
                var sum = 0,
                    count = 0,
                    avg = 0,
                    min = 0,
                    max = 0;
                if (data && data.length) {
                    max = parseFloat(data[0][column.name]);
                    min = parseFloat(data[0][column.name]);
                    for (var i = 0; i < data.length; i++) {
                        if (data[i][p.statusName] == 'delete') continue;
                        count += 1;
                        var value = data[i][column.name];
                        if (typeof (value) == "string") value = value.replace(/\$|\,/g, '');
                        value = parseFloat(value);
                        if (!value) continue;
                        sum += value;
                        if (value > max) max = value;
                        if (value < min) min = value;
                    }
                    avg = sum * 1.0 / data.length;
                }
                return {
                    sum: sum,
                    count: count,
                    avg: avg,
                    min: min,
                    max: max
                };
            } catch (e) {
                return {};
            }
        },
        _getTotalCellContent: function (column, data) {
            var g = this,
                p = this.options;
            var totalsummaryArr = [];
            if (column.totalSummary) {
                var isExist = function (type) {
                    for (var i = 0; i < types.length; i++)
                        if (types[i].toLowerCase() == type.toLowerCase()) return true;
                    return false;
                };
                var info = g._getTotalInfo(column, data);
                if (column.totalSummary.render) {
                    var renderhtml = column.totalSummary.render(info, column, g.data);
                    totalsummaryArr.push(renderhtml);
                } else if (column.totalSummary.type && info) {
                    var types = column.totalSummary.type.split(',');
                    if (isExist('sum'))
                        totalsummaryArr.push("<div>Sum=" + info.sum.toFixed(2) + "</div>");
                    if (isExist('tsum'))
                        totalsummaryArr.push("<div>" + sum.toFixed(0) + "</div>");
                    if (isExist('count'))
                        totalsummaryArr.push("<div>Count=" + info.count + "</div>");
                    if (isExist('max'))
                        totalsummaryArr.push("<div>Max=" + info.max.toFixed(2) + "</div>");
                    if (isExist('min'))
                        totalsummaryArr.push("<div>Min=" + info.min.toFixed(2) + "</div>");
                    if (isExist('avg'))
                        totalsummaryArr.push("<div>Avg=" + info.avg.toFixed(2) + "</div>");
                }
            }
            return totalsummaryArr.join('');
        },
        _getTotalSummaryHtml: function (data, classCssName, frozen) {
            var g = this,
                p = this.options;
            var totalsummaryArr = [];
            if (classCssName)
                totalsummaryArr.push('<tr class="lee-grid-totalsummary ' + classCssName + '">');
            else
                totalsummaryArr.push('<tr class="lee-grid-totalsummary">');
            $(g.columns).each(function (columnindex, column) {
                if (this.frozen != frozen) return;
                //如果是行序号(系统列)
                if (this.isrownumber) {
                    totalsummaryArr.push('<td class="lee-grid-totalsummary-cell lee-grid-totalsummary-cell-rownumbers" style="width:' + this.width + 'px;height:' + p.rowHeight + 'px;"><div>&nbsp;</div></td>');
                    return;
                }
                //如果是复选框(系统列)
                if (this.ischeckbox) {
                    totalsummaryArr.push('<td class="lee-grid-totalsummary-cell lee-grid-totalsummary-cell-checkbox" style="width:' + this.width + 'px;height:' + p.rowHeight + 'px;"><div>&nbsp;</div></td>');
                    return;
                }
                //如果是明细列(系统列)
                else if (this.isdetail) {
                    totalsummaryArr.push('<td class="lee-grid-totalsummary-cell lee-grid-totalsummary-cell-detail" style="width:' + this.width + 'px;height:' + p.rowHeight + 'px;"><div>&nbsp;</div></td>');
                    return;
                }
                totalsummaryArr.push('<td class="lee-grid-totalsummary-cell');
                if (this.islast)
                    totalsummaryArr.push(" lee-grid-totalsummary-cell-last");
                totalsummaryArr.push('" ');

                totalsummaryArr.push(' style="height:' + p.rowHeight + 'px;"');
                totalsummaryArr.push('id="' + g.id + "|total" + g.totalNumber + "|" + column.__id + '" ');
                totalsummaryArr.push('width="' + this._width + '" ');
                columnname = this.columnname;
                if (columnname) {
                    totalsummaryArr.push('columnname="' + columnname + '" ');
                }
                totalsummaryArr.push('columnindex="' + columnindex + '" ');
                totalsummaryArr.push('><div class="lee-grid-totalsummary-cell-inner"');
                if (column.align)
                    totalsummaryArr.push(' style="text-Align:' + column.align + ';width:' + parseInt(column._width - 8) + 'px;"');



                totalsummaryArr.push('>');
                totalsummaryArr.push(g._getTotalCellContent(column, data));
                totalsummaryArr.push('</div></td>');
            });
            totalsummaryArr.push('</tr>');
            if (!frozen) g.totalNumber++;
            return totalsummaryArr.join('');
        },
        _bulidTotalSummary: function (frozen) {
            var g = this,
                p = this.options;
            if (!g.isTotalSummary()) return false;
            if (!g.currentData || g.currentData[p.root].length == 0) return false;
            var totalRow = $(g._getTotalSummaryHtml(g.currentData[p.root], null, frozen));
            $("tbody:first", frozen ? g.f.gridbody : g.gridbody).append(totalRow);
            if (frozen) g.totalRow1 = totalRow;
            else g.totalRow2 = totalRow;
        },
        updateTotalSummary: function () {
            var g = this,
                p = this.options;
            g.reRender({
                totalOnly: true
            });
        },
        setPage: function (val) { //设置分页信息
            var g = this,
                p = this.options;
            p.page = val
            g._buildPager();
        },
        _buildPager: function () {
            var g = this,
                p = this.options;
            if (p.pagerRender) {
                return;
            }
            $('.pcontrol input', g.toolbar).val(p.page);
            if (!p.pageCount) p.pageCount = 1;
            $('.pcontrol span', g.toolbar).html(p.pageCount);
            var r1 = parseInt((p.page - 1) * p.pageSize) + 1.0;
            var r2 = parseInt(r1) + parseInt(p.pageSize) - 1;
            if (!p.total) p.total = 0;
            if (p.total < r2) r2 = p.total;
            if (!p.total) r1 = r2 = 0;
            if (r1 < 0) r1 = 0;
            if (r2 < 0) r2 = 0;
            var stat = p.pageStatMessage;
            stat = stat.replace(/{from}/, r1);
            stat = stat.replace(/{to}/, r2);
            stat = stat.replace(/{total}/, p.total);
            stat = stat.replace(/{pagesize}/, p.pageSize);
            if (p.total == 0) {
                stat = p.noRecordMessage;
            }
            $('.lee-bar-text', g.toolbar).html(stat);
            if (!p.total) {
                $(".lee-bar-btnfirst,.lee-bar-btnprev,.lee-bar-btnnext,.lee-bar-btnlast", g.toolbar)
                    .addClass("lee-disabled");
            }
            if (p.hideLoadButton) {
                $('.lee-bar-btnload:first', g.toolbar).parent().hide();
                $('.lee-bar-btnload:first', g.toolbar).parent().next().hide();
            }
            if (p.page == 1) {
                $(".lee-bar-btnfirst ", g.toolbar).addClass("lee-disabled");
                $(".lee-bar-btnprev ", g.toolbar).addClass("lee-disabled");
            } else if (p.page > p.pageCount && p.pageCount > 0) {
                $(".lee-bar-btnfirst ", g.toolbar).removeClass("lee-disabled");
                $(".lee-bar-btnprev ", g.toolbar).removeClass("lee-disabled");
            }
            if (p.page == p.pageCount) {
                $(".lee-bar-btnlast ", g.toolbar).addClass("lee-disabled");
                $(".lee-bar-btnnext ", g.toolbar).addClass("lee-disabled");
            } else if (p.page < p.pageCount && p.pageCount > 0) {
                $(".lee-bar-btnlast ", g.toolbar).removeClass("lee-disabled");
                $(".lee-bar-btnnext ", g.toolbar).removeClass("lee-disabled");
            }
        },
        _getRowIdByDomId: function (domid) {
            var ids = domid.split('|');
            var rowid = ids[2];
            return rowid;
        },
        _getRowByDomId: function (domid) {
            return this.records[this._getRowIdByDomId(domid)];
        },
        //在外部点击的时候，判断是否在编辑状态，比如弹出的层点击的，如果自定义了编辑器，而且生成的层没有包括在grid内部，建议重载
        _isEditing: function (jobjs) {
            var g = this;
            if (jobjs.hasClass("l-box-dateeditor") || jobjs.hasClass("l-box-select")) return true;

            //判断是否位于编辑器弹出的框
            if (jobjs.hasClass("l-dialog")) {
                var ids = [];
                jobjs.find(".l-dialog").each(function () {
                    var curId = $(this).attr("leeuiid");
                    if (curId) {
                        ids.push(curId);
                    }
                });
                if (g._editorIncludeCotrols(ids)) {
                    return true;
                }
            }
            return false;

        },
        _getSrcElementByEvent: function (e) {
            var g = this;
            var obj = (e.target || e.srcElement);
            var jobjs = $(obj).parents().add(obj);
            var fn = function (parm) {
                for (var i = 0, l = jobjs.length; i < l; i++) {
                    if (typeof parm == "string") {
                        if ($(jobjs[i]).hasClass(parm)) return jobjs[i];
                    } else if (typeof parm == "object") {
                        if (jobjs[i] == parm) return jobjs[i];
                    }
                }
                return null;
            };
            if (fn("lee-grid-editor")) return {
                editing: true,
                editor: fn("lee-grid-editor")
            };
            if (jobjs.index(this.element) == -1) {
                if (g._isEditing(jobjs)) return {
                    editing: true
                };
                else return {
                    out: true
                };
            }
            var indetail = false;
            if (jobjs.hasClass("lee-grid-detailpanel") && g.detailrows) {
                for (var i = 0, l = g.detailrows.length; i < l; i++) {
                    if (jobjs.index(g.detailrows[i]) != -1) {
                        indetail = true;
                        break;
                    }
                }
            }
            var r = {
                grid: fn("lee-panel"),
                indetail: indetail,
                frozen: fn(g.gridview1[0]) ? true : false,
                header: fn("lee-panel-header"), //标题
                gridheader: fn("lee-grid-header"), //表格头 
                gridbody: fn("lee-grid-body"),
                total: fn("lee-panel-bar-total"), //总汇总 
                popup: fn("lee-grid-popup"),
                toolbar: fn("lee-panel-footer")
            };
            if (r.gridheader) {
                r.hrow = fn("lee-grid-hd-row");
                r.hcell = fn("lee-grid-hd-cell");
                r.hcelltext = fn("lee-grid-hd-cell-text");
                r.checkboxall = fn("lee-grid-hd-cell-checkbox");
                if (r.hcell) {
                    var columnid = r.hcell.id.split('|')[2];
                    r.column = g._columns[columnid];
                }
            }
            if (r.gridbody) {
                r.row = fn("lee-grid-row");
                r.cell = fn("lee-grid-row-cell");
                r.checkbox = fn("lee-grid-row-cell-btn-checkbox");
                r.groupbtn = fn("lee-grid-group-togglebtn");
                r.grouprow = fn("lee-grid-grouprow");
                r.detailbtn = fn("lee-grid-row-cell-detailbtn");
                r.detailrow = fn("lee-grid-detailpanel");
                r.totalrow = fn("lee-grid-totalsummary");
                r.totalcell = fn("lee-grid-totalsummary-cell");
                r.rownumberscell = $(r.cell).hasClass("lee-grid-row-cell-rownumbers") ? r.cell : null;
                r.detailcell = $(r.cell).hasClass("lee-grid-row-cell-detail") ? r.cell : null;
                r.checkboxcell = $(r.cell).hasClass("lee-grid-row-cell-checkbox") ? r.cell : null;
                r.treelink = fn("lee-grid-tree-link");
                r.editor = fn("lee-grid-editor");

                if (r.row) r.data = this._getRowByDomId(r.row.id);
                if (r.cell) r.editing = $(r.cell).hasClass("lee-grid-row-cell-editing");
                if (r.editor) r.editing = true;
                if (r.editing) r.out = false;
            }
            if (r.toolbar) {
                r.first = fn("lee-bar-btnfirst");
                r.last = fn("lee-bar-btnlast");
                r.next = fn("lee-bar-btnnext");
                r.prev = fn("lee-bar-btnprev");
                r.load = fn("lee-bar-btnload");
                r.button = fn("lee-bar-button");
            }

            return r;
        },
        _editorIncludeCotrols: function (ids) {
            var g = this,
                p = this.options;
            if (!ids || !ids.length) return false;
            if (g.editor && g.editor.input) {
                if (g._controlIncludeCotrols(g.editor.input, ids)) return true;
            } else if (g.editors) {
                for (var a in g.editors) {
                    if (!g.editors[a]) continue;
                    for (var b in g.editors[a]) {
                        var editor = g.editors[a][b];
                        if (editor && editor.input) {
                            if (g._controlIncludeCotrols(editor.input, ids)) return true;
                        }
                    }
                }
            }
            return false;
        },

        _controlIncludeCotrols: function (control, ids) {
            var g = this,
                p = this.options;
            if (!control || !control.includeControls || !control.includeControls.length) return false;

            for (var i = 0; i < control.includeControls.length; i++) {
                var sub = control.includeControls[i];
                if ($.inArray(sub.id, ids) != -1) return true;
            }
            return false;
        },

        _getOnePageHeight: function () {
            var g = this,
                p = this.options;
            return (parseFloat(p.rowHeight || 24) + 1) * parseInt(p.pageSize);
        },

        _setEvent: function () {
            var g = this,
                p = this.options;
            g.grid.bind("mousedown.grid", function (e) {
                g._onMouseDown.call(g, e);
            });
            g.grid.bind("dblclick.grid", function (e) {
                g._onDblClick.call(g, e);
            });
            g.grid.bind("contextmenu.grid", function (e) {
                return g._onContextmenu.call(g, e);
            });
            $(document).bind("mouseup.grid." + this.id, function (e) {
                g._onMouseUp.call(g, e);
            });
            $(document).bind("click.grid." + this.id, function (e) {
                g._onClick.call(g, e);
            });
            $(window).bind("resize.grid." + this.id, function (e) {
                g._onResize.call(g);
            });
            //$(document).bind("keydown.grid", function (e) {
            //    if (e.ctrlKey) g.ctrlKey = true;
            //});
            //$(document).bind("keyup.grid", function (e) {
            //    delete g.ctrlKey;
            //});

            g.gridbody.bind('keydown.grid', function (event) {
                if (event.keyCode == 9) //enter,也可以改成9:tab
                {
                    g.endEditToNext();
                }
            });
            //表体 - 滚动联动事件 
            g.gridbody.bind('scroll.grid', function () {
                var scrollLeft = g.gridbody.scrollLeft();
                var scrollTop = g.gridbody.scrollTop();
                if (scrollTop != null)
                    g.f.gridbody.scrollTop(scrollTop);
                if (scrollLeft != null)
                    g.gridheader[0].scrollLeft = scrollLeft;

                if (p.scrollToPage && p.usePager && !g.loading) {
                    var innerHeight = g.gridbody.find(".lee-grid-body-inner:first").height();
                    var toHeight = scrollTop + g.gridbody.height();
                    if (p.scrollToAppend) {
                        if (p.newPage != p.pageCount) {
                            if (toHeight >= innerHeight) {
                                g.reload(p.newPage + 1, "scrollappend");
                            }
                        }
                    } else {
                        var topage = toHeight >= innerHeight ? p.pageCount : Math.ceil(toHeight / g._getOnePageHeight());
                        if (!g.scrollLoading) {
                            g.scrollLoading = true;
                            g.lastScrollTop = scrollTop;
                            g.unbind("sysScrollLoaded");
                            g.bind("sysScrollLoaded", function () {
                                g.gridbody.scrollTop(scrollTop);
                                setTimeout(function () {
                                    g.scrollLoading = false;
                                }, 500);
                            });
                            g.scrollLoading = true;
                            g.reload(topage, "scroll");

                        }
                    }
                }


                if (p.virtualScroll) {
                    if (g.scrollTimer) {
                        clearTimeout(g.scrollTimer);
                    }
                    g.scrollTimer = setTimeout(function () {
                        g.onScroll(scrollTop);
                    }, 100);

                }

                g.trigger('SysGridHeightChanged');
            });
            //工具条 - 切换每页记录数事件
            $('select', g.toolbar).change(function () {
                if (g.isDataChanged && p.dataAction != "local" && !confirm(p.isContinueByDataChanged))
                    return false;
                p.newPage = 1;
                p.pageSize = this.value;
                g.loadData(p.dataAction != "local" ? p.where : false);
            });
            //工具条 - 切换当前页事件
            $('span.pcontrol :text', g.toolbar).blur(function (e) {
                g.changePage('input');
            });
            $("div.l-bar-button", g.toolbar).hover(function () {
                $(this).addClass("l-bar-button-over");
            }, function () {
                $(this).removeClass("l-bar-button-over");
            });
            //列拖拽支持
            if ($.fn.leeDrag && p.colDraggable) {
                g.colDroptip = $("<div class='l-drag-coldroptip' style='display:none'><div class='l-drop-move-up'></div><div class='l-drop-move-down'></div></div>").appendTo('body');
                g.gridheader.add(g.f.gridheader).leeDrag({
                    revert: true,
                    animate: false,
                    proxyX: 0,
                    proxyY: 0,
                    proxy: function (draggable, e) {
                        var src = g._getSrcElementByEvent(e);
                        if (src.hcell && src.column) {
                            var content = $(".lee-grid-hd-cell-text:first", src.hcell).html();
                            var proxy = $("<div class='l-drag-proxy' style='display:none'><div class='l-drop-icon l-drop-no'></div></div>").appendTo('body');
                            proxy.append(content);
                            return proxy;
                        }
                    },
                    onRevert: function () {
                        return false;
                    },
                    onRendered: function () {
                        this.set('cursor', 'default');
                        g.children[this.id] = this;
                    },
                    onStartDrag: function (current, e) {
                        if (e.button == 2) return false;
                        if (g.colresizing) return false;
                        this.set('cursor', 'default');
                        var src = g._getSrcElementByEvent(e);
                        if (!src.hcell || !src.column || src.column.issystem || src.hcelltext) return false;
                        if ($(src.hcell).css('cursor').indexOf('resize') != -1) return false;
                        this.draggingColumn = src.column;
                        g.coldragging = true;

                        var gridOffset = g.grid.offset();
                        this.validRange = {
                            top: gridOffset.top,
                            bottom: gridOffset.top + g.gridheader.height(),
                            left: gridOffset.left - 10,
                            right: gridOffset.left + g.grid.width() + 10
                        };
                    },
                    onDrag: function (current, e) {
                        this.set('cursor', 'default');
                        var column = this.draggingColumn;
                        if (!column) return false;
                        if (g.colresizing) return false;
                        if (g.colDropIn == null)
                            g.colDropIn = -1;
                        var pageX = e.pageX;
                        var pageY = e.pageY;
                        var visit = false;
                        var gridOffset = g.grid.offset();
                        var validRange = this.validRange;
                        if (pageX < validRange.left || pageX > validRange.right ||
                            pageY > validRange.bottom || pageY < validRange.top) {
                            g.colDropIn = -1;
                            g.colDroptip.hide();
                            this.proxy.find(".l-drop-icon:first").removeClass("l-drop-yes").addClass("l-drop-no");
                            return;
                        }
                        for (var colid in g._columns) {
                            var col = g._columns[colid];
                            if (column == col) {
                                visit = true;
                                continue;
                            }
                            if (col.issystem) continue;
                            var sameLevel = col['__level'] == column['__level'];
                            var isAfter = !sameLevel ? false : visit ? true : false;
                            if (column.frozen != col.frozen) isAfter = col.frozen ? false : true;
                            if (g.colDropIn != -1 && g.colDropIn != colid) continue;
                            var cell = document.getElementById(col['__domid']);
                            var offset = $(cell).offset();
                            var range = {
                                top: offset.top,
                                bottom: offset.top + $(cell).height(),
                                left: offset.left - 10,
                                right: offset.left + 10
                            };
                            if (isAfter) {
                                var cellwidth = $(cell).width();
                                range.left += cellwidth;
                                range.right += cellwidth;
                            }
                            if (pageX > range.left && pageX < range.right && pageY > range.top && pageY < range.bottom) {
                                var height = p.headerRowHeight;
                                if (col['__rowSpan']) height *= col['__rowSpan'];
                                g.colDroptip.css({
                                    left: range.left + 5,
                                    top: range.top - 9,
                                    height: height + 9 * 2
                                }).show();
                                g.colDropIn = colid;
                                g.colDropDir = isAfter ? "right" : "left";
                                this.proxy.find(".l-drop-icon:first").removeClass("l-drop-no").addClass("l-drop-yes");
                                break;
                            } else if (g.colDropIn != -1) {
                                g.colDropIn = -1;
                                g.colDroptip.hide();
                                this.proxy.find(".l-drop-icon:first").removeClass("l-drop-yes").addClass("l-drop-no");
                            }
                        }
                    },
                    onStopDrag: function (current, e) {
                        var column = this.draggingColumn;
                        g.coldragging = false;
                        if (g.colDropIn != -1) {
                            g.changeCol.leeDefer(g, 0, [column, g.colDropIn, g.colDropDir == "right"]);
                            g.colDropIn = -1;
                        }
                        g.colDroptip.hide();
                        this.set('cursor', 'default');
                    }
                });
            }
            //行拖拽支持
            if ($.fn.leeDrag && p.rowDraggable) {
                g.rowDroptip = $("<div class='l-drag-rowdroptip' style='display:none'></div>").appendTo('body');
                g.gridbody.add(g.f.gridbody).leeDrag({
                    revert: true,
                    animate: false,
                    proxyX: 0,
                    proxyY: 0,
                    proxy: function (draggable, e) {
                        var src = g._getSrcElementByEvent(e);
                        if (src.row) {
                            var content = p.draggingMessage.replace(/{count}/, draggable.draggingRows ? draggable.draggingRows.length : 1);
                            if (p.rowDraggingRender) {
                                content = p.rowDraggingRender(draggable.draggingRows, draggable, g);
                            }
                            var proxy = $("<div class='l-drag-proxy' style='display:none'><div class='l-drop-icon l-drop-no'></div>" + content + "</div>").appendTo('body');
                            return proxy;
                        }
                    },
                    onRevert: function () {
                        return false;
                    },
                    onRendered: function () {
                        this.set('cursor', 'default');
                        g.children[this.id] = this;
                    },
                    onStartDrag: function (current, e) {
                        if (e.button == 2) return false;
                        if (g.colresizing) return false;
                        if (!g.columns.length) return false;
                        this.set('cursor', 'default');
                        var src = g._getSrcElementByEvent(e);
                        if (!src.cell || !src.data || src.checkbox) return false;
                        var ids = src.cell.id.split('|');
                        var column = g._columns[ids[ids.length - 1]];
                        if (src.rownumberscell || src.detailcell || src.checkboxcell || column == g.columns[0]) {
                            if (g.enabledCheckbox()) {
                                this.draggingRows = g.getSelecteds();
                                if (!this.draggingRows || !this.draggingRows.length) return false;
                            } else {
                                this.draggingRows = [src.data];
                            }
                            this.draggingRow = src.data;
                            this.set('cursor', 'move');
                            g.rowdragging = true;
                            this.validRange = {
                                top: g.gridbody.offset().top,
                                bottom: g.gridbody.offset().top + g.gridbody.height(),
                                left: g.grid.offset().left - 10,
                                right: g.grid.offset().left + g.grid.width() + 10
                            };
                        } else {
                            return false;
                        }
                    },
                    onDrag: function (current, e) {
                        var rowdata = this.draggingRow;
                        if (!rowdata) return false;
                        var rows = this.draggingRows ? this.draggingRows : [rowdata];
                        if (g.colresizing) return false;
                        if (g.rowDropIn == null) g.rowDropIn = -1;
                        var pageX = e.pageX;
                        var pageY = e.pageY;
                        var visit = false;
                        var validRange = this.validRange;
                        if (pageX < validRange.left || pageX > validRange.right ||
                            pageY > validRange.bottom || pageY < validRange.top) {
                            g.rowDropIn = -1;
                            g.rowDroptip.hide();
                            this.proxy.find(".l-drop-icon:first").removeClass("l-drop-yes l-drop-add").addClass("l-drop-no");
                            return;
                        }
                        for (var i in g.rows) {
                            var rd = g.rows[i];
                            var rowid = rd['__id'];
                            if (rowdata == rd) visit = true;
                            if ($.inArray(rd, rows) != -1) continue;
                            var isAfter = visit ? true : false;
                            if (g.rowDropIn != -1 && g.rowDropIn != rowid) continue;
                            var rowobj = g.getRowObj(rowid);
                            var offset = $(rowobj).offset();
                            var range = {
                                top: offset.top - 4,
                                bottom: offset.top + $(rowobj).height() + 4,
                                left: g.grid.offset().left,
                                right: g.grid.offset().left + g.grid.width()
                            };
                            if (pageX > range.left && pageX < range.right && pageY > range.top && pageY < range.bottom) {
                                var lineTop = offset.top;
                                if (isAfter) lineTop += $(rowobj).height();
                                g.rowDroptip.css({
                                    left: range.left,
                                    top: lineTop,
                                    width: range.right - range.left
                                }).show();
                                g.rowDropIn = rowid;
                                g.rowDropDir = isAfter ? "bottom" : "top";
                                if (p.tree && pageY > range.top + 5 && pageY < range.bottom - 5) {
                                    this.proxy.find(".l-drop-icon:first").removeClass("l-drop-no l-drop-yes").addClass("l-drop-add");
                                    g.rowDroptip.hide();
                                    g.rowDropInParent = true;
                                } else {
                                    this.proxy.find(".l-drop-icon:first").removeClass("l-drop-no l-drop-add").addClass("l-drop-yes");
                                    g.rowDroptip.show();
                                    g.rowDropInParent = false;
                                }
                                break;
                            } else if (g.rowDropIn != -1) {
                                g.rowDropIn = -1;
                                g.rowDropInParent = false;
                                g.rowDroptip.hide();
                                this.proxy.find(".l-drop-icon:first").removeClass("l-drop-yes  l-drop-add").addClass("l-drop-no");
                            }
                        }
                    },
                    onStopDrag: function (current, e) {
                        var rows = this.draggingRows;
                        g.rowdragging = false;
                        for (var i = 0; i < rows.length; i++) {
                            var children = rows[i].children;
                            if (children) {
                                rows = $.grep(rows, function (node, i) {
                                    var isIn = $.inArray(node, children) == -1;
                                    return isIn;
                                });
                            }
                        }
                        if (g.rowDropIn != -1) {
                            if (p.tree) {
                                var neardata, prow;
                                if (g.rowDropInParent) {
                                    prow = g.getRow(g.rowDropIn);
                                } else {
                                    neardata = g.getRow(g.rowDropIn);
                                    prow = g.getParent(neardata);
                                }
                                g.appendRange(rows, prow, neardata, g.rowDropDir != "bottom");
                                g.trigger('rowDragDrop', {
                                    rows: rows,
                                    parent: prow,
                                    near: neardata,
                                    after: g.rowDropDir == "bottom"
                                });
                            } else {
                                g.moveRange(rows, g.rowDropIn, g.rowDropDir == "bottom");
                                g.trigger('rowDragDrop', {
                                    rows: rows,
                                    parent: prow,
                                    near: g.getRow(g.rowDropIn),
                                    after: g.rowDropDir == "bottom"
                                });
                            }

                            g.rowDropIn = -1;
                        }
                        g.rowDroptip.hide();
                        this.set('cursor', 'default');
                    }
                });
            }
        },
        _onRowOver: function (rowParm, over) {
            if (leeUI.draggable.dragging) return;
            var g = this,
                p = this.options;
            var rowdata = g.getRow(rowParm);
            var methodName = over ? "addClass" : "removeClass";
            if (over && g.editor.editing) {
                $("tr." + p.mouseoverRowCssClass, g.gridview).removeClass(p.mouseoverRowCssClass);
            }
            if (g.enabledFrozen())
                $(g.getRowObj(rowdata, true))[methodName](p.mouseoverRowCssClass);
            $(g.getRowObj(rowdata, false))[methodName](p.mouseoverRowCssClass);
        },
        _onMouseUp: function (e) {
            var g = this,
                p = this.options;
            if (leeUI.draggable.dragging) {
                var src = g._getSrcElementByEvent(e);

                //drop in header cell
                if (src.hcell && src.column) {
                    g.trigger('dragdrop', [{
                        type: 'header',
                        column: src.column,
                        cell: src.hcell
                    }, e]);
                } else if (src.row) {
                    g.trigger('dragdrop', [{
                        type: 'row',
                        record: src.data,
                        row: src.row
                    }, e]);
                }
            }
        },
        _onMouseDown: function (e) {
            var g = this,
                p = this.options;
        },
        _onContextmenu: function (e) {
            var g = this,
                p = this.options;
            var src = g._getSrcElementByEvent(e);
            if (src.row) {
                if (p.whenRClickToSelect)
                    g.select(src.data);
                if (g.hasBind('contextmenu')) {
                    return g.trigger('contextmenu', [{
                        data: src.data,
                        rowindex: src.data['__index'],
                        row: src.row
                    }, e]);
                }
            } else if (src.hcell) {
                if (!p.allowHideColumn) return true;
                var columnindex = $(src.hcell).attr("columnindex");
                if (columnindex == undefined) return true;
                var left = (e.pageX - g.body.offset().left + parseInt(g.body[0].scrollLeft));
                if (columnindex == g.columns.length - 1) left -= 50;
                g.popup.css({
                    left: left,
                    top: g.gridheader.height() + 1
                });
                g.popup.toggle();
                return false;
            }
        },
        _onDblClick: function (e) {
            var g = this,
                p = this.options;
            var src = g._getSrcElementByEvent(e);
            if (src.row) {
                g.trigger('dblClickRow', [src.data, src.data['__id'], src.row]);
            }
        },
        _onClick: function (e) {
            var obj = (e.target || e.srcElement);
            var g = this,
                p = this.options;
            var src = g._getSrcElementByEvent(e);
            if (src.out) {
                if (g.editor.editing && !$.leeUI.win.masking) g.endEdit();
                if (p.allowHideColumn) g.popup.hide();
                return;
            }
            if (src.indetail || src.editing) {
                return;
            }
            if (g.editor.editing) {
                g.endEdit();
            }
            if (p.allowHideColumn) {
                if (!src.popup) {
                    g.popup.hide();
                }
            }
            if (src.checkboxall) //复选框全选
            {
                var row = $(src.hrow);
                var uncheck = row.hasClass("lee-checked");
                if (g.trigger('beforeCheckAllRow', [!uncheck, g.element]) == false) return false;
                if (uncheck) {
                    row.removeClass("lee-checked");
                } else {
                    row.addClass("lee-checked");
                }
                g.selected = [];
                for (var rowid in g.records) {
                    if (uncheck)
                        g.unselect(g.records[rowid]);
                    else
                        g.select(g.records[rowid]);
                }
                g.trigger('checkAllRow', [!uncheck, g.element]);
            } else if (src.hcelltext) { //排序
                var hcell = $(src.hcelltext).parent().parent();
                if (!p.enabledSort || !src.column) return;
                if (src.column.isSort == false) return;
                if (p.url && p.dataAction != "local" && g.isDataChanged && !confirm(p.isContinueByDataChanged)) return;
                var sort = $(".lee-grid-hd-cell-sort:first", hcell);
                var columnName = src.column.name;
                if (!columnName) return;
                if (sort.length > 0) {
                    if (sort.hasClass("lee-grid-hd-cell-sort-asc")) {
                        sort.removeClass("lee-grid-hd-cell-sort-asc").addClass("lee-grid-hd-cell-sort-desc");
                        hcell.removeClass("lee-grid-hd-cell-asc").addClass("lee-grid-hd-cell-desc");
                        g.trigger('ChangeSort', [columnName, 'desc']);
                        g.changeSort(columnName, 'desc');
                    } else if (sort.hasClass("lee-grid-hd-cell-sort-desc")) {
                        sort.removeClass("lee-grid-hd-cell-sort-desc").addClass("lee-grid-hd-cell-sort-asc");
                        hcell.removeClass("lee-grid-hd-cell-desc").addClass("lee-grid-hd-cell-asc");
                        g.trigger('ChangeSort', [columnName, 'asc']);
                        g.changeSort(columnName, 'asc');
                    }
                } else {
                    hcell.removeClass("lee-grid-hd-cell-desc").addClass("lee-grid-hd-cell-asc");
                    $(src.hcelltext).after("<span class='lee-icon lee-grid-hd-cell-sort lee-grid-hd-cell-sort-asc'></span>");
                    g.trigger('ChangeSort', [columnName, 'asc']);
                    g.changeSort(columnName, 'asc');
                }
                //移除其他
                $(".lee-grid-hd-cell-sort", g.gridheader).add($(".lee-grid-hd-cell-sort", g.f.gridheader)).not($(".lee-grid-hd-cell-sort:first", hcell)).remove();
            }
            //明细
            else if (src.detailbtn && p.detail) {
                var item = src.data;
                var row = $([g.getRowObj(item, false)]);
                if (g.enabledFrozen()) row = row.add(g.getRowObj(item, true));
                var rowid = item['__id'];
                if ($(src.detailbtn).hasClass("l-open")) {
                    if (p.detail.onCollapse)
                        p.detail.onCollapse(item, $(".lee-grid-detailpanel-inner:first", nextrow)[0]);
                    row.next("tr.lee-grid-detailpanel").hide();
                    $(src.detailbtn).removeClass("l-open");
                } else {
                    var nextrow = row.next("tr.lee-grid-detailpanel");
                    if (nextrow.length > 0) {
                        nextrow.show();
                        if (p.detail.onExtend)
                            p.detail.onExtend(item, $(".lee-grid-detailpanel-inner:first", nextrow)[0]);
                        $(src.detailbtn).addClass("l-open");
                        g.trigger('SysGridHeightChanged');
                        return;
                    }
                    $(src.detailbtn).addClass("l-open");
                    var frozenColNum = 0;
                    for (var i = 0; i < g.columns.length; i++)
                        if (g.columns[i].frozen) frozenColNum++;
                    var detailRow = $("<tr class='lee-grid-detailpanel'><td><div class='lee-grid-detailpanel-inner' style='display:none'></div></td></tr>");
                    var detailFrozenRow = $("<tr class='lee-grid-detailpanel'><td><div class='lee-grid-detailpanel-inner' style='display:none'></div></td></tr>");
                    detailRow.find("div:first").width(g.gridheader.find("div:first").width() - 50);
                    detailRow.attr("id", g.id + "|detail|" + rowid);
                    g.detailrows = g.detailrows || [];
                    g.detailrows.push(detailRow[0]);
                    g.detailrows.push(detailFrozenRow[0]);
                    var detailRowInner = $("div:first", detailRow);
                    detailRowInner.parent().attr("colSpan", g.columns.length - frozenColNum);
                    row.eq(0).after(detailRow);
                    if (frozenColNum > 0) {
                        detailFrozenRow.find("td:first").attr("colSpan", frozenColNum);
                        row.eq(1).after(detailFrozenRow);
                    }
                    if (p.detail.onShowDetail) {
                        p.detail.onShowDetail(item, detailRowInner[0], function () {
                            g.trigger('SysGridHeightChanged');
                        });
                        $("div:first", detailFrozenRow).add(detailRowInner).show().height(p.detail.height || p.detailHeight);
                    } else if (p.detail.render) {
                        detailRowInner.append(p.detail.render());
                        detailRowInner.show();
                    }
                    g.trigger('SysGridHeightChanged');
                }
            } else if (src.groupbtn) {
                var grouprow = $(src.grouprow);
                var opening = true;
                if ($(src.groupbtn).hasClass("lee-grid-group-togglebtn-close")) {
                    $(src.groupbtn).removeClass("lee-grid-group-togglebtn-close");

                    if (grouprow.hasClass("lee-grid-grouprow-last")) {
                        $("td:first", grouprow).width('auto');
                    }
                } else {
                    opening = false;
                    $(src.groupbtn).addClass("lee-grid-group-togglebtn-close");
                    if (grouprow.hasClass("lee-grid-grouprow-last")) {
                        $("td:first", grouprow).width(g.gridtablewidth);
                    }
                }
                var currentRow = grouprow.next(".lee-grid-row,.lee-grid-totalsummary-group,.lee-grid-detailpanel");
                while (true) {
                    if (currentRow.length == 0) break;
                    if (opening) {
                        currentRow.show();
                        //如果是明细展开的行，并且之前的状态已经是关闭的，隐藏之
                        if (currentRow.hasClass("lee-grid-detailpanel") && !currentRow.prev().find("td.lee-grid-row-cell-detail:first span.lee-grid-row-cell-detailbtn:first").hasClass("l-open")) {
                            currentRow.hide();
                        }
                    } else {
                        currentRow.hide();
                    }
                    currentRow = currentRow.next(".lee-grid-row,.lee-grid-totalsummary-group,.lee-grid-detailpanel");
                }
                g.trigger(opening ? 'groupExtend' : 'groupCollapse');
                g.trigger('SysGridHeightChanged');
            }
            //树 - 伸展/收缩节点
            else if (src.treelink) {
                g.toggle(src.data);
            } else if (src.row && g.enabledCheckbox()) //复选框选择行
            {
                //复选框
                var selectRowButtonOnly = p.selectRowButtonOnly ? true : false;
                if (p.enabledEdit) selectRowButtonOnly = true;
                if (obj.tagName.toLowerCase() == "a") return;
                if ((src.checkbox || !selectRowButtonOnly) && p.selectable != false) {
                    var row = $(src.row);
                    var uncheck = row.hasClass("lee-selected");
                    if (g.trigger('beforeCheckRow', [!uncheck, src.data, src.data['__id'], src.row]) == false)
                        return false;
                    var met = uncheck ? 'unselect' : 'select';
                    g[met](src.data);
                    if (p.tree && p.autoCheckChildren) {
                        var children = g.getChildren(src.data, true);
                        for (var i = 0, l = children.length; i < l; i++) {
                            g[met](children[i]);
                        }
                    }
                    g.trigger('checkRow', [!uncheck, src.data, src.data['__id'], src.row]);
                }
                if (!src.checkbox && src.cell && p.enabledEdit && p.clickToEdit) {
                    g._applyEditor(src.cell);
                }
            } else if (src.row && !g.enabledCheckbox() && p.selectable != false) {
                if (src.cell && p.enabledEdit && p.clickToEdit) {
                    g._applyEditor(src.cell);
                }

                //选择行
                if ($(src.row).hasClass("lee-selected")) {
                    if (!p.allowUnSelectRow) {
                        $(src.row).addClass("lee-selected-again");
                        return;
                    }
                    g.unselect(src.data);
                } else {
                    g.select(src.data);
                }
            } else if (src.toolbar) {
                if (src.first) {
                    if (g.trigger('toFirst', [g.element]) == false) return false;
                    g.changePage('first');
                } else if (src.prev) {
                    if (g.trigger('toPrev', [g.element]) == false) return false;
                    g.changePage('prev');
                } else if (src.next) {
                    if (g.trigger('toNext', [g.element]) == false) return false;
                    g.changePage('next');
                } else if (src.last) {
                    if (g.trigger('toLast', [g.element]) == false) return false;
                    g.changePage('last');
                } else if (src.load) {
                    if ($(src.load).hasClass("lee-disabled")) return false;
                    if (g.trigger('reload', [g.element]) == false) return false;
                    if (p.url && g.isDataChanged && !confirm(p.isContinueByDataChanged))
                        return false;
                    g.loadData(p.where);
                }
            }
        },
        select: function (rowParm) {
            var g = this,
                p = this.options;
            var rowdata = g.getRow(rowParm);
            var rowid = rowdata['__id'];
            var rowobj = g.getRowObj(rowid);
            if (!p.rowSelectable || g.trigger('beforeSelectRow', [rowdata, rowid, rowobj]) == false) return;
            var rowobj1 = g.getRowObj(rowid, true);
            if ((!g.enabledCheckbox() && !g.ctrlKey) || p.isSingleCheck) //单选
            {
                for (var i in g.selected) {
                    var o = g.selected[i];
                    if (o['__id'] in g.records) {
                        $(g.getRowObj(o)).removeClass("lee-selected lee-selected-again");
                        if (g.enabledFrozen())
                            $(g.getRowObj(o, true)).removeClass("lee-selected lee-selected-again");
                    }
                }
                g.selected = [];
            }
            if (rowobj) $(rowobj).addClass("lee-selected");
            if (rowobj1) $(rowobj1).addClass("lee-selected");
            g.selected[g.selected.length] = rowdata;
            if (p.virtualScroll) rowdata["__selected"] = true;
            g.trigger('selectRow', [rowdata, rowid, rowobj]);
        },
        cancelSelect: function () {
            //取消选中行
            var obj = this.getSelectedRow();
            obj && this.unselect(obj);
        },
        unselect: function (rowParm) {
            var g = this,
                p = this.options;
            var rowdata = g.getRow(rowParm);
            var rowid = rowdata['__id'];
            var rowobj = g.getRowObj(rowid);
            var rowobj1 = g.getRowObj(rowid, true);
            $(rowobj).removeClass("lee-selected lee-selected-again");
            if (g.enabledFrozen())
                $(rowobj1).removeClass("lee-selected lee-selected-again");
            g._removeSelected(rowdata);
            if (p.virtualScroll) delete rowdata["__selected"];
            g.trigger('unSelectRow', [rowdata, rowid, rowobj]);
        },
        isSelected: function (rowParm) {
            var g = this,
                p = this.options;
            var rowdata = g.getRow(rowParm);
            for (var i in g.selected) {
                if (g.selected[i] == rowdata) return true;
            }
            return false;
        },
        arrayToTree: function (data, id, pid) //将ID、ParentID这种数据格式转换为树格式
        {
            var g = this,
                p = this.options;
            var childrenName = "children";
            if (p.tree) childrenName = p.tree.childrenName;
            if (!data || !data.length) return [];
            var targetData = []; //存储数据的容器(返回) 
            var records = {};
            var itemLength = data.length; //数据集合的个数
            for (var i = 0; i < itemLength; i++) {
                var o = data[i];
                var key = getKey(o[id]);
                records[key] = o;
            }
            for (var i = 0; i < itemLength; i++) {
                var currentData = data[i];
                var key = getKey(currentData[pid]);
                var parentData = records[key];
                if (!parentData) {
                    targetData.push(currentData);
                    continue;
                }
                parentData[childrenName] = parentData[childrenName] || [];
                parentData[childrenName].push(currentData);
            }
            return targetData;

            function getKey(key) {
                if (typeof (key) == "string") key = key.replace(/[.]/g, '').toLowerCase();
                return key;
            }
        },
        gradeToTree: function (data, grade, level, detail, format) {
            var g = this, p = this.options;
            var itemLength = data.length;
            var idobj = {};
            for (var i = 0; i < itemLength; i++) {
                var levels = Number(data[i][level]);
                var key_id = "__mainid";
                var key_pid = "__parentID";
                var len = this._getLenByFormat(levels, format);
                data[i][key_id] = data[i][grade];
                idobj[data[i][key_id]] = "exit";
                data[i][key_pid] = data[i][grade].substring(0, len);
            }
            //把 没有上级的节点设成空
            for (var i = 0; i < itemLength; i++) {
                if (!idobj[data[i][key_pid]])
                    data[i][key_pid] = "";
            }
            var res = g.arrayToTree(data, "__mainid", "__parentID");
            return res;

        },
        _getLenByFormat: function (level, format) {
            var len = 0;
            for (var i = 0; i < level - 1; i++) {
                var _length = "";
                if (format.length > level) {
                    _length = format.substring(i, i + 1);
                } else {
                    _length = 4;
                }
                len += Number(_length);
            }
            return len;
        },
        _onResize: function () {
            var g = this,
                p = this.options;
            if (p.height && p.height != 'auto') {
                var windowHeight = $(window).height();
                //if(g.windowHeight != undefined && g.windowHeight == windowHeight) return;

                var h = 0;
                var parentHeight = null;
                if (typeof (p.height) == "string" && p.height.indexOf('%') > 0) {
                    var gridparent = g.grid.parent();
                    if (p.inWindow) {
                        parentHeight = windowHeight;
                        parentHeight -= parseInt($('body').css('paddingTop'));
                        parentHeight -= parseInt($('body').css('paddingBottom'));
                    } else {
                        parentHeight = gridparent.height();
                    }
                    h = parentHeight * parseInt(p.height) * 0.01;
                    if (p.inWindow || gridparent[0].tagName.toLowerCase() == "body")
                        h -= (g.grid.offset().top - parseInt($('body').css('paddingTop')));
                } else {
                    h = parseInt(p.height);
                }

                h += p.heightDiff;
                g.windowHeight = windowHeight;
                g._setHeight(h);
            } else {
                g._updateHorizontalScrollStatus.leeDefer(g, 10);
            }
            if (g.enabledFrozen()) {
                var gridView1Width = g.gridview1.width();
                var gridViewWidth = g.gridview.width();
                if (gridViewWidth - gridView1Width <= 0) {
                    g.gridview2.css({
                        width: 'auto'
                    });
                } else {
                    g.gridview2.css({
                        width: gridViewWidth - gridView1Width
                    });
                }
            }
            g.trigger('SysGridHeightChanged');
        },
        _setDisabled: function (flag) {
            var g = this,
                p = this.options;
            p.disabled = flag;
        },
        destroy: function () {
            var g = this;
            if (this.element) {
                $(this.element).remove();
            }
            this.options = null;
            g.grid.unbind("mousedown.grid");
            g.grid.unbind("dblclick.grid");
            g.grid.unbind("contextmenu.grid");
            $(document).unbind("mouseup.grid." + this.id);
            $(document).unbind("click.grid." + this.id);
            $(window).unbind("resize.grid." + this.id);
            leeUI.remove(this);
        }

    });

    //method alias
    $.leeUI.controls.Grid.prototype.update = $.leeUI.controls.Grid.prototype.updateRow;
    $.leeUI.controls.Grid.prototype.append = $.leeUI.controls.Grid.prototype.appendRow;

    $.leeUI.controls.Grid.prototype.enabledTotal = $.leeUI.controls.Grid.prototype.isTotalSummary;
    $.leeUI.controls.Grid.prototype.add = $.leeUI.controls.Grid.prototype.addRow;
    $.leeUI.controls.Grid.prototype.update = $.leeUI.controls.Grid.prototype.updateRow;
    $.leeUI.controls.Grid.prototype.append = $.leeUI.controls.Grid.prototype.appendRow;
    $.leeUI.controls.Grid.prototype.getSelected = $.leeUI.controls.Grid.prototype.getSelectedRow;
    $.leeUI.controls.Grid.prototype.getSelecteds = $.leeUI.controls.Grid.prototype.getSelectedRows;
    $.leeUI.controls.Grid.prototype.getCheckedRows = $.leeUI.controls.Grid.prototype.getSelectedRows;
    $.leeUI.controls.Grid.prototype.getCheckedRowObjs = $.leeUI.controls.Grid.prototype.getSelectedRowObjs;
    $.leeUI.controls.Grid.prototype.setOptions = $.leeUI.controls.Grid.prototype.set;
    $.leeUI.controls.Grid.prototype.reload = $.leeUI.controls.Grid.prototype.loadData;
    $.leeUI.controls.Grid.prototype.refreshSize = $.leeUI.controls.Grid.prototype._onResize;
    $.leeUI.controls.Grid.prototype.append = $.leeUI.controls.Grid.prototype.appendRange;

    function removeArrItem(arr, filterFn) {
        for (var i = arr.length - 1; i >= 0; i--) {
            if (filterFn(arr[i])) {
                arr.splice(i, 1);
            }
        }
    }
})(jQuery);

//todo 
//2.样式优化
//3.编辑器更多类型
//4.计算扩展
//5.键盘事件
leeUI.gridRender = {};

leeUI.gridRender.DropDownRender = function (rowdata, rowindex, value, column) {
    var colname = column.columnname;
    var vsdata = column.editor.data;
    var valuefield = "id";
    if (column.editor.valueField) valuefield = column.editor.valueField;
    if (column.editor.valueColumnName) valuefield = column.editor.valueColumnName;
    var namefield = "text";
    if (column.editor.textField) namefield = column.editor.textField;
    if (column.editor.displayColumnName) namefield = column.editor.displayColumnName;
    if (column.editor.isMultiSelect) {
        var arrdata = [];
        var valuedata = value.split(";");
        for (var n = 0; n < vsdata.length; n++) {
            for (var j = 0; j < vsdata.length; j++) {
                if (valuedata[n] == vsdata[j][valuefield]) {
                    arrdata.push(vsdata[j][namefield]);
                    break;
                }
            }

        }

        return arrdata.join(";");

    } else {
        for (var i = 0; i < vsdata.length; i++) {
            if (value == vsdata[i][valuefield])
                return vsdata[i][namefield];
        }
    }
};

leeUI.gridRender.ToogleRender = function (rowdata, rowindex, value, column) {
    if (typeof value != "boolean") {
        value = value == '1' ? true : false;
    }

    var checked = value ? "checked='checked'" : "";
    var disabled = column.readonly ? "disabled" : "";
    var ctrlid = this.id + '_chk_' + column.name + '_' + rowindex;
    var iconHtml = '<div style="padding:1px;"><input type="checkbox" class="lee-toggle-switch" ' + checked + ' ' + disabled + ' id="' + ctrlid + '"/>';
    iconHtml += "<label for='" + ctrlid + "'/></div>";
    return iconHtml;
}

leeUI.gridRender.bind = function () {

    $("body").on("click", ".lee-grid-row-cell-inner .lee-toggle-switch", function (e) {

        var grid = $(this).closest(".lee-ui-grid").leeUI();
        var cell = $(this).closest(".lee-grid-row-cell");
        var row = $(this).closest(".lee-grid-row");
        var rowobj = grid.getRow(row.attr("id").split("|")[2]);
        console.log(rowobj);
        var column = grid.getColumn(cell.attr("id").split("|")[3]);
        console.log(column);
        var checked = $(this).prop("checked") ? "1" : "0";
        if (typeof rowobj[column.columnname] == "boolean") {
            checked = $(this).prop("checked");
        }
        if (grid.options.onBeforeCheckEditor) {
            if (grid.options.onBeforeCheckEditor.call(this, rowobj, column.columnname)) {

                grid.updateCell(column.columnname, checked, rowobj);
            }
        } else {
            grid.updateCell.leeDefer(grid, 200, [column.columnname, checked, rowobj]);
        }
        e.stopPropagation();

    });


    // $("body").on("click", ".lee-grid-row-cell-inner .grid_up", function (e) {

    //     var grid = $(this).closest(".lee-ui-grid").leeUI();
    //     var cell = $(this).closest(".lee-grid-row-cell");
    //     var row = $(this).closest(".lee-grid-row");
    //     var rowobj = grid.getRow(row.attr("id").split("|")[2]);
    //     grid.up(rowobj);
    //     e.stopPropagation();

    // });


    // $("body").on("click", ".lee-grid-row-cell-inner .grid_down", function (e) {

    //     var grid = $(this).closest(".lee-ui-grid").leeUI();
    //     var cell = $(this).closest(".lee-grid-row-cell");
    //     var row = $(this).closest(".lee-grid-row");
    //     var rowobj = grid.getRow(row.attr("id").split("|")[2]);
    //     grid.down(rowobj);
    //     e.stopPropagation();

    // });
    // $("body").on("click", ".lee-grid-row-cell-inner .grid_remove", function (e) {

    //     var grid = $(this).closest(".lee-ui-grid").leeUI();
    //     var cell = $(this).closest(".lee-grid-row-cell");
    //     var row = $(this).closest(".lee-grid-row");
    //     var rowobj = grid.getRow(row.attr("id").split("|")[2]);
    //     grid.deleteRow(rowobj);
    //     e.stopPropagation();

    // });


    $("body").on("click", ".lee-grid-row-cell-inner .grid_delete", function (e) {

        var grid = $(this).closest(".lee-ui-grid").leeUI();
        var cell = $(this).closest(".lee-grid-row-cell");
        var row = $(this).closest(".lee-grid-row");
        var rowobj = grid.getRow(row.attr("id").split("|")[2]);

        e.stopPropagation();

    });

    $("body").on("click", ".lee-grid-row-cell-inner .lee-checkbox-wrapper", function (e) {


        if ($(this).hasClass("lee-disabled")) return;
        var grid = $(this).closest(".lee-ui-grid").leeUI();

        if (grid.options.disabled) return;
        var cell = $(this).closest(".lee-grid-row-cell");
        var row = $(this).closest(".lee-grid-row");
        var span = $(cell.find("span"));
        var rowobj = grid.getRow(row.attr("id").split("|")[2]);
        console.log(rowobj);
        var column = grid.getColumn(cell.attr("id").split("|")[3]);
        console.log(column);
        span.toggleClass("lee-checkbox-checked");
        var checked = span.hasClass("lee-checkbox-checked") ? "1" : "0";



        if (grid.options.onBeforeCheckEditor) {
            if (grid.options.onBeforeCheckEditor.call(this, rowobj, column.columnname)) {

                grid.updateCell(column.columnname, checked, rowobj);
            }
        } else {
            grid.updateCell.leeDefer(grid, 200, [column.columnname, checked, rowobj]);
        }
        e.stopPropagation();

    });


}
$(function () {
    leeUI.gridRender.bind();
});


leeUI.gridRender.CheckboxRender = function (rowdata, rowindex, value, column) {
    value = value == '1' ? true : false;
    var iconHtml = '<div class="lee-checkbox-wrapper ' + (column.readonly ? "  lee-disabled " : "") + '> ';



    iconHtml += ' rowid = "' + rowdata['__id'] + '"';
    iconHtml += ' gridid = "' + this.id + '"';
    iconHtml += ' columnname = "' + column.name + '"';


    iconHtml += '><span class="lee-checkbox ';

    if (value) iconHtml += ' lee-checkbox-checked ';
    iconHtml += '"></span></div>';
    return iconHtml;
};
/**
 * This jQuery plugin displays pagination links inside the selected elements.
 */
jQuery.fn.leePager = function (maxentries, opt) {
    var ele = this;

    var opts = $.extend({
        items_per_page: 10,
        num_display_entries: 10,
        current_page: 0,
        num_edge_entries: 0,
        link_to: "#",
        prev_text: "上一页",
        next_text: "下一页",
        first_text: "首页",
        last_text: "尾页",
        ellipse_text: "...",
        prev_show_always: true,
        next_show_always: true,
        callback: function () {
            return false;
        }
    }, opt || {});

    return ele.each(function () {
        /**
		 * 计算最大分页显示数目
		 */
        function numPages() {
            return Math.ceil(maxentries / opts.items_per_page);
        }
        /**
		 * 极端分页的起始和结束点，这取决于current_page 和 num_display_entries.
		 * @返回 {数组(Array)}
		 */
        function getInterval() {
            var ne_half = Math.ceil(opts.num_display_entries / 2);
            var np = numPages();
            var upper_limit = np - opts.num_display_entries;
            var start = current_page > ne_half ? Math.max(Math.min(current_page - ne_half, upper_limit), 0) : 0;
            var end = current_page > ne_half ? Math.min(current_page + ne_half, np) : Math.min(opts.num_display_entries, np);
            return [start, end];
        }

        /**
		 * 分页链接事件处理函数
		 * @参数 {int} page_id 为新页码
		 */
        function pageSelected(page_id, evt) {
            current_page = page_id;
            drawLinks();
            var continuePropagation = opts.callback(page_id, panel);
            if (!continuePropagation) {
                if (evt.stopPropagation) {
                    evt.stopPropagation();
                } else {
                    evt.cancelBubble = true;
                }
            }
            return continuePropagation;
        }

        /**
		 * 此函数将分页链接插入到容器元素中
		 */
        function drawLinks() {
            panel.empty();
            var interval = getInterval();
            var np = numPages();
            // 这个辅助函数返回一个处理函数调用有着正确page_id的pageSelected。
            var getClickHandler = function (page_id) {
                return function (evt) {
                    return pageSelected(page_id, evt);
                }
            }
            //辅助函数用来产生一个单链接(如果不是当前页则产生span标签)
            var appendItem = function (page_id, appendopts, tial) {
                page_id = page_id < 0 ? 0 : (page_id < np ? page_id : np - 1); // 规范page id值
                appendopts = jQuery.extend({
                    text: page_id + 1,
                    classes: ""
                }, appendopts || {});
                if (page_id == current_page) {
                    var lnk
                    if (tial) {
                        lnk = jQuery("<span disabled>" + (appendopts.text) + "</span>");
                    } else {
                        lnk = jQuery("<span class='current'>" + (appendopts.text) + "</span>");
                    }

                } else {
                    var lnk = jQuery("<span>" + (appendopts.text) + "</span>")
                        .bind("click", getClickHandler(page_id))
                        .attr('href', opts.link_to.replace(/__id__/, page_id));
                }
                if (appendopts.classes) {
                    lnk.addClass(appendopts.classes);
                }
                panel.append(lnk);
            }
            // 产生"Previous"-链接
            if (opts.prev_text && (current_page > 0 || opts.prev_show_always)) {

                appendItem(0, {
                    text: opts.first_text,
                    classes: "prev"
                }, true);
                appendItem(current_page - 1, {
                    text: opts.prev_text,
                    classes: "prev"
                }, true);

            }
            if (!opts.simple) {
                // 产生起始点
                if (interval[0] > 0 && opts.num_edge_entries > 0) {
                    var end = Math.min(opts.num_edge_entries, interval[0]);
                    for (var i = 0; i < end; i++) {
                        appendItem(i);
                    }
                    if (opts.num_edge_entries < interval[0] && opts.ellipse_text) {
                        jQuery("<span>" + opts.ellipse_text + "</span>").appendTo(panel);
                    }
                }
                // 产生内部的些链接
                for (var i = interval[0]; i < interval[1]; i++) {
                    appendItem(i);
                }
                // 产生结束点
                if (interval[1] < np && opts.num_edge_entries > 0) {
                    if (np - opts.num_edge_entries > interval[1] && opts.ellipse_text) {
                        jQuery("<span>" + opts.ellipse_text + "</span>").appendTo(panel);
                    }
                    var begin = Math.max(np - opts.num_edge_entries, interval[1]);
                    for (var i = begin; i < np; i++) {
                        appendItem(i);
                    }

                }
            }

            // 产生 "Next"-链接
            if (opts.next_text && (current_page < np - 1 || opts.next_show_always)) {
                appendItem(current_page + 1, {
                    text: opts.next_text,
                    classes: "next"

                }, true);

                appendItem(np - 1, {
                    text: opts.last_text,
                    classes: "prev"
                }, true);
            }
        }

        //从选项中提取current_page
        var current_page = opts.current_page;
        //创建一个显示条数和每页显示条数值
        maxentries = (!maxentries || maxentries < 0) ? 1 : maxentries;
        opts.items_per_page = (!opts.items_per_page || opts.items_per_page < 0) ? 1 : opts.items_per_page;
        //存储DOM元素，以方便从所有的内部结构中获取
        var panel = jQuery(this);
        // 获得附加功能的元素
        this.selectPage = function (page_id) {
            pageSelected(page_id);
        }
        this.prevPage = function () {
            if (current_page > 0) {
                pageSelected(current_page - 1);
                return true;
            } else {
                return false;
            }
        }
        this.nextPage = function () {
            if (current_page < numPages() - 1) {
                pageSelected(current_page + 1);
                return true;
            } else {
                return false;
            }
        }
        // 所有初始化完成，绘制链接
        drawLinks();
        // 回调函数
        //opts.callback(current_page, this);
    });
};