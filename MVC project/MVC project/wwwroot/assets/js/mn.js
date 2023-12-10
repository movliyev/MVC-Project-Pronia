const addButtons = document.querySelectorAll(".add-basket-button");

addButtons.forEach(btn => {
    btn.addEventListener("click", function (e) {
        e.preventDefault();
        var endpoint = btn.getAttribute("href");
        fetch(endpoint)
            .then(response => response.text())
            .then(data => {

            })
    })
})
