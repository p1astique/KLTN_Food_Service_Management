document.addEventListener('submit', function (event) {
    const form = event.target.closest('form[data-confirm]');
    if (form && !window.confirm(form.dataset.confirm || 'Bạn có chắc không?')) event.preventDefault();
});
