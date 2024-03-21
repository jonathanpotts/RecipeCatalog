const addCuisineModalElement = document.getElementById("addCuisineModal");

if (addCuisineModalElement) {
    const addCuisineModal = new bootstrap.Modal(addCuisineModalElement);

    window.showAddCuisineModal = () => {
        addCuisineModal.show();
    }

    window.hideAddCuisineModal = () => {
        addCuisineModal.hide();
    };
}
