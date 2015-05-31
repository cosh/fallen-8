//
// Program.cs
//
// Author:
//       Henning Rauch <Henning@RauchEntwicklung.biz>
//
// Copyright (c) 2015 Henning Rauch
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
using System;
using NoSQL.GraphDB;
using NoSQL.GraphDB.Service;

namespace Benchmark
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            var shutdown = false;

            #region Fallen-8 startup

            var fallen8 = new Fallen8 ();

            #endregion

            #region services

            #region Fallen-8 REST API

            fallen8.ServiceFactory.StartGraphService ();
            fallen8.ServiceFactory.StartAdminService ();

            #endregion

            #region benchmark api

            IService introService;
            if (fallen8.ServiceFactory.TryAddService (out introService, "Fallen-8_Benchmark_Service", "Benchmark API", null)) {
                introService.TryStart ();
            }

            #endregion


            #endregion

            #region shutdown

            Console.WriteLine ("Enter 'shutdown' to initiate the shutdown of this instance.");

            while (!shutdown) {
                var command = Console.ReadLine ();

                if (command == null)
                    continue;

                if (command.ToUpper () == "SHUTDOWN")
                    shutdown = true;
            }

            Console.WriteLine ("Shutting down Fallen-8 startup");
            fallen8.Shutdown ();
            Console.WriteLine ("Shutdown complete");

            #endregion
        }
    }
}
