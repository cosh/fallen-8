// 
// graph.js
//  
// Author:
//       Sebastian Dechant <s3bbi@fallen-8.com>
//
// Copyright (c) 2012 Sebastian Dechant
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

var w = 1400,
    h = 800;

//var w = window.innerWidth || (window.document.documentElement.clientWidth || window.document.body.clientWidth);
//var h = window.innerHeight || (window.document.documentElement.clientHeight || window.document.body.clientHeight);

var force = d3.layout.force()
    .charge(-820)
    .linkDistance(180)
	.linkStrength(0.5)
	.theta(0.2)
    .size([w, h]);

var vis = d3.select("#graph").append("svg:svg")
    .attr("width", w)
    .attr("height", h);

vis.append("svg:rect")
    .attr("width", w)
    .attr("height", h);

var nodes = [];
var links = [];

function getVertex(vertexId) {
    d3.json(baseUri + "/Vertices/" + vertexId + "/Properties", function (json) {

        DrawVertex(json);
    });
}

function ReturnXY(d) {
   
    return (ReturnWH(d) / 2) * (-1);
}

function ReturnWH(d) {
    
    return 60;
}

function DrawVertex(json) {
    this.nodes = new Array();
    this.links = new Array();

    this.nodes[0] = json;


    vis.selectAll("line").remove();
    var link = vis.selectAll("line")
      .data(links)
    .enter().append("svg:line")
	.style("stroke-width", function (d) { return 5; });

    vis.selectAll("circle").remove();
    var node = vis.selectAll("circle")
      .data(nodes)
    .enter().append("svg:circle")
      .attr("xlink:href", function (d) { return d.image; })
      .attr("x", function (d) { return ReturnXY(d) + "px"; })
      .attr("y", function (d) { return ReturnXY(d) + "px"; })
      .attr("width", function (d) { return ReturnWH(d) + "px"; })
      .attr("height", function (d) { return ReturnWH(d) + "px"; })
      .attr("onclick", function (d) { return "ShowVertexInfo('" + d.Id + "')"; })
	  .call(force.drag);

    force
      .nodes(nodes)
      .links(links)
      .on("tick", tick)
      .start();
}

function ShowVertexInfo(id) {
    alert("asdasd");
}

function tick() {
    vis.selectAll("line")
      .attr("x1", function (d) { return d.source.x; })
      .attr("y1", function (d) { return d.source.y; })
      .attr("x2", function (d) { return d.target.x; })
      .attr("y2", function (d) { return d.target.y; });

    vis.selectAll("circle")
        .attr("transform", function(d) { return "translate(" + d.x + "," + d.y + ")"; });
}