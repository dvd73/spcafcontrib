using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using Microsoft.SharePoint;
using SharePoint.Common.Utilities.Extensions;

namespace SharePoint.Common.Utilities
{
    /// <summary>
    /// This class is implemented as singleton and it should be used for storing application data 
    /// (webparts result or whatever kind of data). The key is based on current url
    /// For storing application data is used inner collection type of dictionary with keys of type string.
    /// </summary>
    public sealed class CacheDispatcher
    {
        private static volatile CacheDispatcher _Instance;
        private static readonly object _Object = new Object();
        private bool _shouldClean;
        private double CACHE_TIME = 10.0;
        private const string KEY_PREFIX = "SharePoint_";
        private const string URL_PREFIX = "_URL_";

        #region Properties

        /// <summary>
        /// Gets the reference to _Instance of CacheDispatcher singleton
        /// </summary>
        public static CacheDispatcher Current
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_Object)
                    {
                        if (_Instance == null)
                            _Instance = new CacheDispatcher();
                    }
                }
                return _Instance;
            }
        }

        /// <summary>
        /// This property is used to explicitly calling the Clear method from CacheCleaningHandler.
        /// It's because the event handler is assynchronous and the HttpContext.Current is null when the event ItemUpdated is fired
        /// </summary>
        public bool ShouldClean
        {
            set
            {
                _shouldClean = value;
            }
        }

        /// <summary>
        /// Verifies that expiration time is different from 0.0
        /// </summary>
        private bool IsCacheSetOn
        {
            get
            {
                return CACHE_TIME != 0.0;
            }
        }

        #endregion

        /// <summary>
        /// Method verifies if url contains 'nocache' parameters and in this case it clears the cache
        /// Method also checks the web.config file and reacts on change of CacheExpiration appSettings key
        /// </summary>
        private void SetExpiration()
        {
            if (_shouldClean)
            {
                Clear();
                _shouldClean = false;
            }
            //allows to clear cache from URL - i.e. when &nocache=1 is set
            if (HttpContext.Current.Request.QueryString["nocache"] != null)
            {
                CACHE_TIME = 0.0;
                Clear();
                return;
            }

             //TODO: value from web.config e.g. Common.GetConfigurationValue("CacheExpiration", false);
            // The default cache time should be configured in web.config 
            // <appSettings><add key="CacheExpiration" value="10.0" /></appSettings>
            string expiration = "10.0";
            if (expiration == string.Empty) return;
            double val;
            Double.TryParse(expiration, out val);
            if (val == CACHE_TIME) return;
            CACHE_TIME = val;
            Clear();
        }

        #region Methods

        /// <summary>
        /// Determines whether the cache contains the specified key
        /// URL of current site is included as a part of key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>returns true if key is contained or false</returns>
        public bool ContainsKey(string key)
        {
            SetExpiration();
            if (IsCacheSetOn)
            {
                string url = SPContext.Current.Web.Url;
                string urlBasedKey = KEY_PREFIX + key + URL_PREFIX + url;
                object o = HttpContext.Current.Cache[urlBasedKey];
                if (o != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the cache contains the specified Key
        /// URL of current site isn't included as a part of key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>returns true if key is contained or false</returns>
        public bool ContainsKeyWithoutUrl(string key)
        {
            SetExpiration();
            if (IsCacheSetOn)
            {
                object o = HttpContext.Current.Cache[KEY_PREFIX + key];
                if (o != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets object value specified by key from the cache. If key doesn't exist in cache returns null
        /// URL of current site is included as a part of key 
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>returns the object value stored in the cache</returns>
        public object Get(string key)
        {
            if (IsCacheSetOn)
            {
                string url = SPContext.Current.Web.Url;
                string urlBasedKey = KEY_PREFIX + key + URL_PREFIX + url;
                return HttpContext.Current.Cache[urlBasedKey];
            }
            return null;
        }

        /// <summary>
        /// Gets object value specified by key from the cache. If key doesn't exist in cache returns null
        /// URL of current site isn't included as a part of key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>returns the object value stored in the cach</returns>
        public object GetWithoutUrl(string key)
        {
            return !IsCacheSetOn ? null : HttpContext.Current.Cache[KEY_PREFIX + key];
        }

        /// <summary>
        /// Adds the specified key and value into the cache
        /// URL of current site is included as a part of key
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public void Add(string key, object value)
        {
            if (!IsCacheSetOn) return;
            string url = SPContext.Current.Web.Url;
            string urlBasedKey = KEY_PREFIX + key + URL_PREFIX + url;
            HttpContext.Current.Cache.Insert(urlBasedKey, value, null, DateTime.Now.AddMinutes(CACHE_TIME), TimeSpan.Zero);
        }

        /// <summary>
        /// Adds the specified key and value into the cache
        /// URL of current site isn't included as a part of key
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public void AddWithoutUrl(string key, object value)
        {
            if (IsCacheSetOn)
            {
                HttpContext.Current.Cache.Insert(KEY_PREFIX + key, value, null, DateTime.Now.AddMinutes(CACHE_TIME), TimeSpan.Zero);
            }
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// URL of current site is included as a part of key
        /// </summary>
        /// <param name="key">key</param>
        public void Remove(string key)
        {
            if (!IsCacheSetOn) return;
            string url = SPContext.Current.Web.Url;
            string urlBasedKey = KEY_PREFIX + key + URL_PREFIX + url;
            HttpContext.Current.Cache.Remove(urlBasedKey);
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// URL of current site isn't included as a part of key
        /// </summary>
        /// <param name="key">key</param>
        public void RemoveWithoutUrl(string key)
        {
            if (IsCacheSetOn)
            {
                HttpContext.Current.Cache.Remove(KEY_PREFIX + key);
            }
        }

        /// <summary>
        /// Clears all Starter Kit entries from the chache
        /// </summary>
        public void Clear()
        {
            try
            {
                List<string> keysToRemove = new List<string>();
                IDictionaryEnumerator cacheEnumerator = HttpContext.Current.Cache.GetEnumerator();
                while (cacheEnumerator.MoveNext())
                {
                    string key = cacheEnumerator.Key.ToString();
                    if (key.StartsWith(KEY_PREFIX))
                    {
                        keysToRemove.Add(key);
                    }
                }
                foreach (string key in keysToRemove)
                {
                    HttpContext.Current.Cache.Remove(key);
                }
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }

        #endregion

    }//class
}//namespace
