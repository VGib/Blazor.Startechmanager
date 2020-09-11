var modalClose = document.getElementById("modalClose");
var modal = document.getElementById("modal");
var modalTitle = document.getElementById("modalTitle");
var modalContent = document.getElementById("modalContent");


modalClose.addEventListener('click', (e) => {
    modal.style.display = "none";
});

function openModalWithMessage(title, message) {
    modalTitle.innerText = title;
    modalContent.innerText = message;
    modal.style.display = "block";
}