function blazorGetTimezoneOffset() {
    return new Date().getTimezoneOffset();
}


function showSpinner() {
    console.log("js show spinner");
    document.getElementById("cover-spin").style.display = "block";
}

function hideSpinner(){
    console.log("js hide spinner");
    document.getElementById("cover-spin").style.display = "none";
}

//window.SetBodyCss = function (elementId, classname) {
//    var link = document.getElementById("cover-spin");
//    if (link !== undefined) {
//        link.sty = classname;
//    }
//    return true;
//}