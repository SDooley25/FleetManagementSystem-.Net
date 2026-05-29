// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
(function () {
    // Add a red asterisk to labels of required form fields.
    function markRequiredAsterisks(root) {
        root = root || document;
        // Find inputs/selects/textareas with required attribute or data-val-required (for unobtrusive validation)
        var selectors = 'input[required], textarea[required], select[required], [data-val-required]';
        var elements = Array.prototype.slice.call(root.querySelectorAll(selectors));

        elements.forEach(function (el) {
            // Do not add for hidden inputs
            if (el.type === 'hidden') return;

            // Try to find the corresponding label using for attribute
            var id = el.id;
            var label = null;
            if (id) {
                label = root.querySelector('label[for="' + id + '"]');
            }

            // If no label found, maybe it's wrapped by label
            if (!label) {
                var parent = el.parentElement;
                while (parent) {
                    if (parent.tagName && parent.tagName.toLowerCase() === 'label') { label = parent; break; }
                    parent = parent.parentElement;
                }
            }

            if (!label) return;

            // Avoid duplicating the asterisk
            if (label.querySelector('.required-asterisk')) return;

            var span = document.createElement('span');
            span.className = 'required-asterisk text-danger ms-1';
            span.setAttribute('aria-hidden', 'true');
            span.textContent = '*';

            // Append asterisk after label text
            label.appendChild(span);
        });
    }

    // Run on DOMContentLoaded if layout indicates it should
    document.addEventListener('DOMContentLoaded', function () {
        var body = document && document.body;
        if (body && body.getAttribute && body.getAttribute('data-mark-required-asterisk') === 'true') {
            markRequiredAsterisks(document);
        }
    });

    // Expose globally for dynamic content
    window.markRequiredAsterisks = markRequiredAsterisks;
})();
