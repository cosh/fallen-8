function showCopyright() {
    d3.select("#copyright")
      .style("width", "0%")
      .style("background-color", "white")
      .text(GenerateCopyRight())
    .transition()
      .ease("bounce")
      .duration(2000)
      .style("width", "100%")
      .style("background-color", "black");
}

function GenerateCopyRight() {
    return "Copyright (c) 2012 Henning Rauch";
}