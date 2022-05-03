function Rearrange() {
    var waystations = document.getElementsByClassName("way-station");
    var removeBtns = document.getElementsByClassName("ws-remove-btn");
    var wsCount = waystations.length;
    for (var i = 0; i < wsCount; i++) {
        waystations[i].id = "ws-" + (i + 2);
        document.getElementById(removeBtns[i].id).outerHTML = GetRemoveButtonHtml(waystations[i].id);
    }
}

function AddWayStation() {
    var wsCount = document.getElementsByClassName("way-station").length;
    var newWS = document.createElement("div");
    var template = document.getElementById("ws-template");
    newWS.className = "way-station";
    newWS.id = "ws-" + (wsCount + 2);
    newWS.innerHTML = template.innerHTML + GetRemoveButtonHtml(newWS.id);
    document.getElementById("ws-area").appendChild(newWS);
}

function RemoveWayStation(id) {
    var child = document.getElementById(id);
    document.getElementById("ws-area").removeChild(child);
    Rearrange();
}

function GetRemoveButtonHtml(id) {
    return `<button id=\"ws-remove-${id}\" class=\"btn btn-danger ws-remove-btn\" type=\"button\" onClick=\"RemoveWayStation(\'${id}\')\"> Remove</button></div>`;
}

function ToggleRouteTable(id) {
    var routeTable = document.getElementById(id);
    routeTable.hidden = !routeTable.hidden;
}