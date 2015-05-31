## Welcome to Fallen-8.
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

In order to create your own Service plugin four steps need to be taken. The following example code can be found in Examples/Benchmark. There is a slightly bigger "benchmark" service which could act as some kind of bluprint service for you located in Examples/Benchmark.
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
