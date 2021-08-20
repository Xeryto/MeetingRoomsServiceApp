var editButton = document.querySelector(".edit");
var inputs = document.querySelectorAll(".editInput");

editButton.onclick = function () {
    for (var i = 0; i < inputs.length; i++) {
        inputs[i].removeAttribute('readonly');
    }
}