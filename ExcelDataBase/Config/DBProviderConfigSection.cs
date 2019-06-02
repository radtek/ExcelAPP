 

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ExcelDataBase
{

    /// <summary>
    /// 提供者集合
    /// </summary>
    public class DBProviderConfigSection : ConfigurationElement
    {

        #region public DBProviderConfigElementCollection Implementations {get;set;}

        /// <summary>
        /// 设置提供者集合
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(DBProviderConfigElement)
            , AddItemName = "Register", ClearItemsName = "Clear", RemoveItemName = "Remove"
            , CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
        public DBProviderConfigElementCollection Providers
        {
            get
            {
                return (DBProviderConfigElementCollection)this[""];
            }
            set
            {
                if (null == value) //允许空
                    value = new DBProviderConfigElementCollection();

                this[""] = value;
            }
        }

        #endregion

        #region public bool HasProviders {get;}

        /// <summary>
        /// 是否有提供者
        /// </summary>
        public bool HasProviders
        {
            get { return this.Providers != null && this.Providers.Count > 0; }
        }

        #endregion

        //
        // 方法
        //

        /// <summary>
        /// 增加方法
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="implemementation"></param>
        internal void Add(string providerName, DBProvider  provider)
        {
            DBProviderConfigElement ele = new DBProviderConfigElement(
                providerName,
                provider.GetType().AssemblyQualifiedName,
                provider);
            this.Providers.Add(ele);
        }

        /// <summary>
        /// 移除方法
        /// </summary>
        /// <param name="providerName"></param>
        internal void Remove(string providerName)
        {
            this.Providers.Remove(providerName);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="providername"></param>
        /// <returns></returns>
        internal DBProvider Get(string providername)
        {
            DBProviderConfigElement ele = this.Providers.Get(providername);
            if (null == ele)
            { 
                throw new Exception(string.Format(DbError.ProviderNameError, providername));  
            }

            return ele.Provider;
        }

        

        /// <summary>
        /// returns true if the configured collection 
        /// </summary>
        /// <param name="providername"></param>
        /// <returns></returns>
        internal bool Contains(string providername)
        {
            return this.Providers.Contains(providername);
        }
       
    }
}
