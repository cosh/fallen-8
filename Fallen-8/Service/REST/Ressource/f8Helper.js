// 
// f8Helper.js
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


function getVertex(id)
{
    $.get(f8Uri + "Vertices/" + id + "/Properties",
        function (data) {
            if (data != "") {
                var newNodes = {};
                newNodes.nodes = {};
                newNodes.edges = {};
                newNodes.nodes[data.Id] = { alone: true, 'label': "Id: " + data.Id };
                //
                expandNode(id, newNodes);
                sys.graft();
            }
        });
}

function expandNode(id, toAdd) {
    var tempEdges = [];
    $.get(f8Uri + "Vertices/" + id + "/AvailableOutEdges", function (data) { getAllEdges(data); });

    function getAllEdges(data) {
        for (var count in data) {
            $.get(f8Uri + "Vertices/" + id + "/OutEdges/"+data[count], function (data) { addEdges(data); });
        }
        sys.graft(toAdd);
    }

    function addEdges(data) {
        for (var count in data) {
            sys.addEdge(id, data[count]);
            $.get(f8Uri + "Vertices/" + data[count] + "/Properties",
             function (data) {
                 sys.addNode(data.Id, { 'label': "Id: "+data.Id });
             });
        }
        
    }
        


}