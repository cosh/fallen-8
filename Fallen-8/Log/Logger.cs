// 
// Logger.cs
//  
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
// 
// Copyright (c) 2011-2015 Henning Rauch
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

#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace NoSQL.GraphDB.Log
{
    /// <summary>
    ///   The Fallen-8 logger
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// The error log delegates
        /// </summary>
        private static List<LogDelegate> _errorLogs = new List<LogDelegate> { ConsoleLog };

        /// <summary>
        /// The info log delegates
        /// </summary>
        private static List<LogDelegate> _infoLogs = new List<LogDelegate> { ConsoleLog };

        /// <summary>
        /// The console log
        /// </summary>
        /// <param name="toBeloggedString"></param>
        private static void ConsoleLog(String toBeloggedString)
        {
            lock (Console.Title)
            {
                Console.WriteLine(toBeloggedString);
            }
        }

        /// <summary>
        /// The log-delegate
        /// </summary>
        /// <param name="toBeLoggedString">The string that should be logged</param>
        public delegate void LogDelegate(String toBeLoggedString);

        /// <summary>
        /// Registers an error log
        /// </summary>
        /// <param name="logDelegate">The log delegate</param>
        public static void RegisterErrorLog(LogDelegate logDelegate)
        {
            _errorLogs.Add(logDelegate);
        }

        /// <summary>
        /// Registers an info log
        /// </summary>
        /// <param name="logDelegate">The log delegate</param>
        public static void RegisterInfoLog(LogDelegate logDelegate)
        {
            _infoLogs.Add(logDelegate);
        }

        /// <summary>
        /// Log an error
        /// </summary>
        /// <param name="message">Error message</param>
        public static void LogError(String message)
        {
            _errorLogs.ForEach(_ => _(message));
        }

        /// <summary>
        /// Log an info
        /// </summary>
        /// <param name="message">Info message</param>
        public static void LogInfo(string message)
        {
            _infoLogs.ForEach(_ => _(message));
        }
    }
}