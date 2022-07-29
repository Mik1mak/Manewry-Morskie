export function toggleModal(modalId) {
    bootstrap.Modal.getOrCreateInstance(document.getElementById(modalId)).toggle();
}

export function showModal(modalId) {
    bootstrap.Modal.getOrCreateInstance(document.getElementById(modalId)).show();
}

export function hideModal(modalId) {
    bootstrap.Modal.getOrCreateInstance(document.getElementById(modalId)).hide();
}