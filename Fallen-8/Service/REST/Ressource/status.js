// 
// status.js
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



var w = 150,
    h = 150,
    r = Math.min(w, h) / 2,
    arc = d3.svg.arc().outerRadius(r);

function showStatus() {
    FetchStatus();
}

function FetchStatus() {

    var statusUri = baseUri + "/Status";
    //production
    d3.json(statusUri, function (json) {

        PrintStatus(json);
        PrintPieCharts(json);
    });
}

function PrintPieCharts(json) {

    PrintMemory(json);
    PrintGraphStats(json);
}

function PrintMemory(json) {
    var freemem = json["FreeMemory"];
    var usedmem = json["UsedMemory"];

    PrintPieChart(freemem, usedmem, "#memstats");
}

function PrintGraphStats(json) {
    var vCount = json["VertexCount"];
    var eCount = json["EdgeCount"];
    if (vCount > 0) {
        PrintPieChart(vCount, eCount, "#graphStats");
    } else {
        PrintPieChart(0, 1, "#graphStats");
    }
}

function PrintPieChart(val1, val2, divName) {
    var data = [val1, val2].sort(d3.descending),
    //data2 = d3.range(2).map(Math.random).sort(d3.descending),
    color = d3.scale.category20(),
    donut = d3.layout.pie();

    var vis = d3.select(divName).append("svg")
    .data([data])
    .attr("width", w)
    .attr("height", h);

    var arcs = vis.selectAll("g.arc")
    .data(donut)
  .enter().append("g")
    .attr("class", "arc")
    .attr("transform", "translate(" + r + "," + r + ")");

    var paths = arcs.append("path")
    .attr("fill", function (d, i) { return color(i); });

    paths.transition()
    .ease("bounce")
    .duration(2000)
    .attrTween("d", tweenPie);
}

function tweenPie(b) {
    b.innerRadius = 0;
    var i = d3.interpolate({ startAngle: 0, endAngle: 0 }, b);
    return function (t) {
        return arc(i(t));
    };
}

function tweenDonut(b) {
    b.innerRadius = r * .6;
    var i = d3.interpolate({ innerRadius: 0 }, b);
    return function (t) {
        return arc(i(t));
    };
}

function PrintStatus(json) {
    d3.select("#statusingeneral")
      .style("width", "0%")
      .style("background-color", "white")
      .html(JSONToText(json))
    .transition()
      .ease("bounce")
      .duration(2000)
      .style("width", "25%")
      .style("background-color", "grey");
}

function PrintList(list, heading) {
    var result = "";

    if (list.length > 0) {

        result += "<p>";

        result += "<font color=darkred>" + heading + "</font><br>";
        for (var i = 0; i < list.length; i++) {
            result += list[i].toString() + "<br>";
        }

        result += "</p>";
    }

    return result;
}

function JSONToText(jsonObject) {
    var result = "<html><head><title>Status</title></head><body>";

    result += "<p><font color=darkred><b>PLUGINS</b></font><br>";
    //idx
    result += PrintList(jsonObject["AvailableIndexPlugins"], "Available indices");
    
    //path
    result += PrintList(jsonObject["AvailablePathPlugins"], "Available path algorithms");

    //servicePlugins
    result += PrintList(jsonObject["AvailableServicePlugins"], "Available services");

    //Free memory
    result += "<font color=darkred>Free memory</font> " + jsonObject["FreeMemory"] + "<br>";

    //used memory
    result += "<font color=darkred>Used memory</font> " + jsonObject["UsedMemory"] + "<br>";

    //#Vertices
    result += "<font color=darkred>|V|</font> " + jsonObject["VertexCount"] + "<br>";

    //#edges
    result += "<font color=darkred>|E|</font> " + jsonObject["EdgeCount"] + "<br>";

    result += "</p></body></html>";

    return result;
}