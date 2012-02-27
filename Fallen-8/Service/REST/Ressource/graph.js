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