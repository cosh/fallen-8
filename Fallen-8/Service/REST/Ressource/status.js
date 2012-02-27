function showStatus() {
    FetchStatus();
}

function FetchStatus() {

    var statusUri = baseUri + "/Status";
    //production
    d3.json(statusUri, function (json) {

        PrintStatus(json);
    });
}

function PrintStatus(json) {
    d3.select("#status")
      .style("width", "0%")
      .style("background-color", "white")
      .html(JSONToText(json))
    .transition()
      .ease("bounce")
      .duration(2000)
      .style("width", "100%")
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