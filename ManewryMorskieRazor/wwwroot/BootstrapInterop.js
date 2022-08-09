export function showDropdown(element) {
    bootstrap.Dropdown.getOrCreateInstance(element).show();
}

export function hideDropdown(element) {
    bootstrap.Dropdown.getOrCreateInstance(element).hide();
}

export function updateDropdown(element) {
    bootstrap.Dropdown.getOrCreateInstance(element).update();
}

export function toggleModal(modalId) {
    bootstrap.Modal.getOrCreateInstance(document.getElementById(modalId)).toggle();
}

export function showModal(modalId) {
    bootstrap.Modal.getOrCreateInstance(document.getElementById(modalId)).show();
}

export function hideModal(modalId) {
    bootstrap.Modal.getOrCreateInstance(document.getElementById(modalId)).hide();
}

