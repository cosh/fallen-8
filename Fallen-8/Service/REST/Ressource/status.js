function showStatus() {
    d3.select("#status")
      .style("width", "0%")
      .style("background-color", "white")
      .text(GenerateStatus())
    .transition()
      .ease("bounce")
      .duration(2000)
      .style("width", "100%")
      .style("background-color", "grey");
}

function GenerateStatus() {
    return "blaa";
}