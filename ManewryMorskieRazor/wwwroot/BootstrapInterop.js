var controllers = {};

export function showDropdown(element) {
    bootstrap.Dropdown.getOrCreateInstance(element).show();
    controllers[element] = new AbortController();

    element.addEventListener('hidden.bs.dropdown', function () {
        bootstrap.Dropdown.getOrCreateInstance(element).show();
    }, { signal: controllers[element].signal });
}

export function hideDropdown(element) {
    controllers[element].abort();
    bootstrap.Dropdown.getOrCreateInstance(element).hide();
    controllers[element] = new AbortController();
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

