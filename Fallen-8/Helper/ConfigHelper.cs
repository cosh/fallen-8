// 
// ConfigHelper.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
// 
// Copyright (c) 2012-2015 Henning Rauch
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#region usings

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

#endregion

namespace NoSQL.GraphDB.Helper
{
    /// <summary>
    ///   A static helper class
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// Fetches a certain configurationSection of a custom appSettings-section in app.config and returns all keys & values as Dictionary
        /// </summary>
        /// <param name="configSection">name of the config section</param>
        /// <returns>Dictionary with all keys and values in this section</returns>
        public static Dictionary<string, object> GetConfigSectionAsDictionary(string configSection)
        {
            return ((NameValueCollection)ConfigurationManager.GetSection(configSection)).ToDictionary();
        }

        /// <summary>
        /// Converts a NameValueCollection (as used in custom appSettings-sections in app.config) to a Dictionary
        /// </summary>
        /// <param name="nvc">NameValueCollection from app.config</param>
        /// <returns>A Dictionary of the values</returns>
        public static Dictionary<string, object> ToDictionary(this NameValueCollection nvc)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            if (nvc == null)
            {
                return dict;
            }
            foreach (var key in nvc.AllKeys)
            {
                dict.Add(key, nvc[key]);
            }
            return dict;
        }

        /// <summary>
        /// Returns configuration values from AppSettings section as an array (values can be comma-separated)
        /// </summary>
        /// <param name="key">the name of the setting as in "key=x"</param>
        /// <returns>the value of the setting as in "value=x"</returns>
        public static string[] GetConfigurationValueArray(string key)
        {
            if (ConfigurationManager.AppSettings[key] != null)
            {
                return ConfigurationManager.AppSettings[key].Split(',');
            }
            else
            {
                throw new IndexOutOfRangeException("Configuration does not contain the requested key: " + key);
            }
        }

        /// <summary>
        /// Nimmt zunächst alle default-Einstellungen (in appSettings-Section) und überschreibt / ergänzt diese mit den Werten aus der pluginName-ConfigSection
        /// </summary>
        /// <param name="pluginName">Name des Plugins (muss mit ConfigSection in app.config übereinstimmen)</param>
        /// <returns>ein Dictionary mit allen für dieses Plugin relevanten Config-Strings</returns>
        public static Dictionary<string, string> GetPluginSpecificConfig(string pluginName)
        {
            Dictionary<string, string> configs = new Dictionary<string, string>();
            NameValueCollection nvc = (NameValueCollection)ConfigurationManager.GetSection("appSettings");
            if (nvc != null)
            {
                foreach (var key in nvc.AllKeys)
                {
                    configs[key] = nvc[key];
                }
            }
            nvc = (NameValueCollection)ConfigurationManager.GetSection(pluginName);
            if (nvc != null)
            {
                foreach (var key in nvc.AllKeys)
                {
                    configs[key] = nvc[key];
                }
            }
            return configs;
        }

        /// <summary>
        /// Sucht nach einem bestimmten Parameter in einem Dictionary/Map von Parametern. Wenn der Parameter mit dem bestimmten Präfix nicht gefunden wird,
        /// wird die Liste von Präfixen rückwärts durchlaufen, bis der Parameter mit dem Präfix gefunden wurde. Normalerweise ist das erste Element der 
        /// Präfix-Liste ein leerer String, so dass der Parameter in Reinform gefunden werden kann (weil er ohne Präfix in appSettings definiert wurde ->
        /// default). So können ein oder mehrere Fallbacks definiert werden (meist wird aber lediglich ein bestimmtes Präfix und der default-Fall definiert).
        /// </summary>
        /// <param name="configs">Die Map von Config-Parametern, die durchsucht werden soll</param>
        /// <param name="param">Name des Parameters (ohne Präfix)</param>
        /// <param name="prefixes">Die verschiedenen möglichen Präfixe des Parameters</param>
        /// <param name="startPlusOne">Ab welchem Präfix rückwärts gesucht werden soll (+1, also 2 für das Präfix foo in {"","foo","bar"})</param>
        /// <returns>den gefundenen Parameter (mit oder ohne Präfix) oder einen leeren String</returns>
        public static string GetInstanceParam(Dictionary<string, string> configs, string param, string[] prefixes, int startPlusOne)
        {
            string value;
            for (int i = startPlusOne; i > 0; i--)
            {
                if (configs.TryGetValue(prefixes[i - 1] + param, out value))
                {
                    return value;
                }
            }
            return "";
        }
    }
}
