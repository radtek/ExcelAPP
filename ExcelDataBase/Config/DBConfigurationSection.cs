 

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;


namespace ExcelDataBase
{
    /// <summary>
    /// 配置文件入口类
    /// </summary>
    public class DBConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// configSections 下 section 中的name名称
        /// </summary>
        public const string DBConfigSectionElement = "GSYS.Data";

        /// <summary>
        /// 是否重写exception处理，
        /// </summary>
        /// <remarks>是：重写处理函数；否：直接抛出错误。</remarks>
        [ConfigurationProperty("Override-exceptions", DefaultValue = false)]
        public bool OverrideExceptions
        {
            get
            {
                object val = this["Override-exceptions"];
                if (val is bool)
                    return (bool)val;
                else
                    return false;
            }
            set
            {
                this["Override-exceptions"] = value;
            }
        }

        /// <summary>
        /// XML命名空间
        /// </summary>
        [ConfigurationProperty("xmlns")]
        public string xmlns
        {
            get { return this["xmlns"] as string; }
            set { this["xmlns"] = value; }
        }

       

        /// <summary>
        /// 获取数据工厂提供者
        /// </summary>
        [ConfigurationProperty("ProviderSection", Options = ConfigurationPropertyOptions.None)]
        public DBProviderConfigSection ProviderSection
        {
            get
            {
                DBProviderConfigSection _providerSection = this["ProviderSection"] as DBProviderConfigSection;
                if (null == _providerSection)
                {
                    _providerSection = new DBProviderConfigSection();
                    _providerSection.Providers = new DBProviderConfigElementCollection();
                    this.ProviderSection = _providerSection;
                }
                return _providerSection;
            }
            set
            {
                this["ProviderSection"] = value;
            }
        }


        private static DBConfigurationSection _defined;

        /// <summary>
        /// 从congfig文件中获取定义的配置信息
        /// </summary>
        /// <returns></returns>
        public static DBConfigurationSection GetSection()
        {
            if (null == _defined)
            {
                _defined = System.Configuration.ConfigurationManager.GetSection(DBConfigurationSection.DBConfigSectionElement) as DBConfigurationSection;
                if(null == _defined)//没有配置文件则创建一个
                    _defined = CreateDefaultConfiguration();
            }
            return _defined;
        }

        /// <summary>
        /// 创建默认的配置文件
        /// </summary>
        private static DBConfigurationSection CreateDefaultConfiguration()
        {
            DBConfigurationSection defined = new DBConfigurationSection();
            defined.ProviderSection = new DBProviderConfigSection();
            defined.ProviderSection.Providers = new DBProviderConfigElementCollection();
            return defined;
        }
    }
}
