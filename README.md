[![Build Status](https://travis-ci.org/cosh/fallen-8.png?branch=master)](https://travis-ci.org/cosh/fallen-8)

## Welcome to Fallen-8.

[![Join the chat at https://gitter.im/cosh/fallen-8](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/cosh/fallen-8?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
Fallen-8 is an in-memory [graph database](http://en.wikipedia.org/wiki/Graph_database) implemented in C#. Its focus is to provide raw speed for heavy graph algorithms.

### Key featues
* **Properties** on vertices and edges 
* **Indexes** on vertices and edges
* **Plugins** for indexes, algorithms and services
* Checkpoint **persistency**

### Sweet spots
* **Enterprise Search** (Semantic adhoc queries on multi-dimensional graphs)
* **Lawful Interception** (Mass analysis)
* **E-Commerce** (Bid- and portfolio-management)

## HowTo Clone
Fallen-8 makes use of git submodules, that's why they have to be cloned too.

```
$ git clone --recursive git@github.com:cosh/fallen-8.git
```

## HowTo Use

```
var fallen8 = new Fallen8();

//start the built-in services
fallen8.ServiceFactory.StartGraphService();
fallen8.ServiceFactory.StartAdminService();
```

## Service plugins

Service plugins are the easiest way to add use-case specific functionality on top of Fallen-8. There are two standard plugins available:
* **Admin** REST service
* **Graph** REST service

They are started by default using the StartUp project.
```
[Fallen-8_Graph_Service:config] URIPattern (n/a, using default): Fallen-8_Graph_Service
[Fallen-8_Graph_Service:config] IPAddress (n/a, using default): 127.0.0.1
[Fallen-8_Graph_Service:config] Port (n/a, using default): 9923
Fallen-8 Graph Service Plugin v1.0.0
   -> Service is started at http://127.0.0.1:9923/Fallen-8_Graph_Service/1.0/REST
[Fallen-8_Admin_Service:config] URIPattern (n/a, using default): Fallen-8_Admin_Service
[Fallen-8_Admin_Service:config] IPAddress (n/a, using default): 127.0.0.1
[Fallen-8_Admin_Service:config] Port (n/a, using default): 9923
Fallen-8 Admin Service Plugin v2.3.0
   -> Service is started at http://127.0.0.1:9923/Fallen-8_Admin_Service/1.0/REST
Enter 'shutdown' to initiate the shutdown of this instance.

```
The lovely .Net is able to help you using the REST services. Just take a look at the webservice help:
* Admin service: http://127.0.0.1:9923/Fallen-8_Admin_Service/1.0/REST/help
* Graph service: http://127.0.0.1:9923/Fallen-8_Graph_Service/1.0/REST/help

In order to create your own Service plugin four steps need to be taken. The following example code can be found in Examples/ServicePlugin. There is a slightly bigger "benchmark" service which could act as some kind of bluprint service for you located in Examples/Benchmark.
### Webservice contract

The webservice contract defines the functions which you would like to add on top of Fallen-8. It's a plain old service contract with one extension: you have to implement the **IRESTService** interface.
```
	[ServiceContract (Namespace = "Fallen-8-Training", Name = "Fallen-8 training service")]
    public interface IHelloService : IRESTService
    {
        /// <summary>
        /// Say hello to someone
        /// </summary>
        /// <param name="who">The one you like to say hello to</param>
        /// <returns>Hello to someone</returns>
        [OperationContract (Name = "Hello")]
        [Description ("[F8Training] Hello: Says hello to someone.")]
        [WebGet (UriTemplate = "/hello/{who}", ResponseFormat = WebMessageFormat.Json)]
        String Hello (String who);
    }
```

### Webservice contract implementation
Create a new IHelloService implementation and implement the methods. In this first example don't do anything in the "IRESTService", "IFallen8Serializable" and "IDisposable" implementations.

```
	public class HelloService : IHelloService
    {
   		...
        #region IHelloService implementation

        public string Hello (string who)
        {
            return "hello " + who;
        }

        #endregion
		...
    }
```

### The Fallen-8 plugin
The actual Fallen-8 plugin needs to extend the ARESTServicePlugin abstract class. The most important field is the "PLUGIN_NAME". This is the unique name of your plugin. You'll need it later in order to start the plugin.
Interesting regions:
* **IFallen8Serializable**: in case the plugin needed to persist sth during the "save"-phase of the checkpoint persistency you are able to deserialize the necessary parameters
* **IPlugin**: All methods which are responsible for the plugin mechanics like creation/versioning/nameing etc.

```
public sealed class HelloServicePlugin : ARESTServicePlugin
    {
        public const string PLUGIN_NAME = "Fallen-8_Hello_Service";
        public const string MANUFACTURER = "Henning Rauch";
        public const string PLUGIN_DESCRIPTION = "Fallen-8 Hello Service Plugin";
        public const string VERSION = "1.0.0";
        public const double RESTVERSION = 1.0;

        #region IFallen8Serializable implementation

        public override IRESTService LoadServiceFromSerialization (SerializationReader reader, Fallen8 fallen8)
        {
            return new HelloService ();
        }

        #endregion

        #region IPlugin implementation

        public override IRESTService CreateService (Fallen8 fallen8, IDictionary<string, object> parameter)
        {
            return new HelloService ();
        }

        ...
        #endregion
    }
```

### Start the plugin
This is pretty simple now. Just tell Fallen-8 what you like to have and you'll get it.

```
IService helloService;
            if (fallen8.ServiceFactory.TryAddService (out helloService, "Fallen-8_Hello_Service", "Hello API", null)) {
                if (helloService.TryStart ()) {
                    Console.WriteLine ("Wohooo.");
                }
            }
```
After you started the project you'll see this:
```
[Fallen-8_Hello_Service:config] URIPattern (n/a, using default): Fallen-8_Hello_Service
[Fallen-8_Hello_Service:config] IPAddress (n/a, using default): 127.0.0.1
[Fallen-8_Hello_Service:config] Port (n/a, using default): 9923
Fallen-8 Hello Service Plugin v1.0.0
   -> Service is started at http://127.0.0.1:9923/Fallen-8_Hello_Service/1.0/REST
Wohooo.
Enter 'shutdown' to initiate the shutdown of this instance.
```

just try it: http://127.0.0.1:9923/Fallen-8_Hello_Service/1.0/REST/hello/world

## Benchmarks
There are plenty of benchmarks in the world. This is a puristic one that focusses on the raw traversal speed of Fallen-8. It creates an equally distributed graph with a defined number of vertices and edges. Afterwards you are able to measure the traversals per second for your graph in Fallen-8.
In order to start the benchmark, compile the Examples/Benchmark project and start it with Admin-Rights. It uses the service plugin pattern described in the last point and provides two web-services
* CreateGraph: http://127.0.0.1:9923/Fallen-8_Benchmark_Service/1.0/REST/CreateGraph?nodes=1000&edgesPerNode=500  (creates a graph with 1000 nodes and 500 edges per node)
* algorithm/tps: http://127.0.0.1:9923/Fallen-8_Benchmark_Service/1.0/REST/algorithm/tps?iterations=10000 (takes every node of the graph and iterates over all it's edges in order to "touch" the neighbor vertex)

As always there is a .Net help available: http://127.0.0.1:9923/Fallen-8_Benchmark_Service/1.0/REST/help

## Additional information

[Graph databases - Henning Rauch](http://www.slideshare.net/HenningRauch/graphdatabases)

[Graphendatenbanken - Henning Rauch (visiting lecture)](http://www.slideshare.net/HenningRauch/vorlesung-graphendatenbanken-an-der-universitt-hof)

[Issues on GitHub](https://github.com/cosh/fallen-8/issues)

[Wiki on GitHub](https://github.com/cosh/fallen-8/wiki)

[Google Group](https://groups.google.com/d/forum/fallen-8)

## MIT-License
Copyright (c) 2012-2015 Henning Rauch

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE
