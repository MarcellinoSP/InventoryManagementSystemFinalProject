// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    const table = document.getElementById("myTable");
    const tbody = table.querySelector("tbody");
    const rows = Array.from(tbody.querySelectorAll("tr"));

    const headers = Array.from(table.querySelectorAll("th"));
    headers.forEach((header, index) => {
        header.addEventListener("click", () => {
            sortTable(index);
            Boolean(index);
        });
    });

    function sortTable(columnIndex) {
        const order = headers[columnIndex].classList.toggle("asc");
        rows.sort((a, b) => {
            const cellA = a.querySelectorAll("td")[columnIndex].textContent.trim();
            const cellB = b.querySelectorAll("td")[columnIndex].textContent.trim();

            return (order ? cellA.localeCompare(cellB) : cellB.localeCompare(cellA));
        });

        while (tbody.firstChild) {
            tbody.removeChild(tbody.firstChild);
        }

        rows.forEach(row => {
            tbody.appendChild(row);
        });
    }
});