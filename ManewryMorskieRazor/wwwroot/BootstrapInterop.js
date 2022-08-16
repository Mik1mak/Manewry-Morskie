var controllers = {};

export function showDropdown(element) {
    bootstrap.Dropdown.getOrCreateInstance(element).show();

    controllers[element] = true;
    element.addEventListener('hidden.bs.dropdown', function () {
        if (controllers[element]) {
            bootstrap.Dropdown.getOrCreateInstance(element).show();
        }
    });
}

export function hideDropdown(element) {
    controllers[element] = false;
    bootstrap.Dropdown.getOrCreateInstance(element).hide();
}

export function toggleDropdown(element) {
    bootstrap.Dropdown.getOrCreateInstance(element).toggle();
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

