// script.js
const showRegister = document.getElementById('show-register');
const showLogin = document.getElementById('show-login');
const formWrapper = document.querySelector('.form-wrapper');

showRegister.addEventListener('click', (e) => {
    e.preventDefault();
    formWrapper.style.transform = 'translateX(-100%)';
});

showLogin.addEventListener('click', (e) => {
    e.preventDefault();
    formWrapper.style.transform = 'translateX(0)';
});
