﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ExcelDataBase
{
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class DbError {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal DbError() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Genersoft.GSIDP.GSYS.Data.DbError", typeof(DbError).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   使用此强类型资源类，为所有资源查找
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 数据库连接字符串不能为空！ 的本地化字符串。
        /// </summary>
        internal static string ConnstrNull {
            get {
                return ResourceManager.GetString("ConnstrNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 配置文件数据库连接串Name属性{0}不能可为空 的本地化字符串。
        /// </summary>
        internal static string ConnstrSetNameNull {
            get {
                return ResourceManager.GetString("ConnstrSetNameNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 配置文件数据库连接串{0}的连接串不能为空！ 的本地化字符串。
        /// </summary>
        internal static string ConnstrSetValueNull {
            get {
                return ResourceManager.GetString("ConnstrSetValueNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 数据库访问出错 的本地化字符串。
        /// </summary>
        internal static string DBErrorCaption {
            get {
                return ResourceManager.GetString("DBErrorCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 未实现的数据库访问类名称：{0}！ 的本地化字符串。
        /// </summary>
        internal static string InvalidProviderNameForConn {
            get {
                return ResourceManager.GetString("InvalidProviderNameForConn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 未实现的数据库访问工厂提供者名称：{0}！ 的本地化字符串。
        /// </summary>
        internal static string InvalidProviderNameForDBFactory {
            get {
                return ResourceManager.GetString("InvalidProviderNameForDBFactory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 参数名不能为空！ 的本地化字符串。
        /// </summary>
        internal static string ParamNameNull {
            get {
                return ResourceManager.GetString("ParamNameNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 参数为空，参数名{0} 的本地化字符串。
        /// </summary>
        internal static string ParamNull {
            get {
                return ResourceManager.GetString("ParamNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 数据库访问类访问工厂类生成出错，ProviderName:{0} 的本地化字符串。
        /// </summary>
        internal static string ProviderFactoryError {
            get {
                return ResourceManager.GetString("ProviderFactoryError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 数据库访问类访问工厂类不能为空！ 的本地化字符串。
        /// </summary>
        internal static string ProviderFactoryNull {
            get {
                return ResourceManager.GetString("ProviderFactoryNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 数据库访问类提供者名称不在配置文件中，ProviderName:{0} 的本地化字符串。
        /// </summary>
        internal static string ProviderNameError {
            get {
                return ResourceManager.GetString("ProviderNameError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 数据库访问类提供者名称不能为空！ 的本地化字符串。
        /// </summary>
        internal static string ProviderNameNull {
            get {
                return ResourceManager.GetString("ProviderNameNull", resourceCulture);
            }
        }
    }
}
