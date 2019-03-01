﻿var previousUrl = "";

function setUsername(username) {
    window.localStorage.setItem('username', username);
}

function getUsername() {
    return window.localStorage.getItem('username');
}

function setToken(token) {
    window.localStorage.setItem('token', token);
}

function getToken() {
    return window.localStorage.getItem('token');
}

$(document).ajaxSend(
           function (event, jqxhr, settings) {
               jqxhr.setRequestHeader("Authorization", "Bearer " + getToken() );
           });

$(document).ajaxError(
    function (event, jqxhr, settings, message) {
        if (jqxhr.status === 401) {
            window.location = "/Users/login.html"; // with redirect
        }
    });

function authenticate() {

    if (getUsername() && getToken()) {
        window.location = "/Commits/commits.html";
    }
    else {
        window.location = "/Users/login.html"; // with redirect
    }
}

function loginReady() {
    previousUrl = document.referrer;
    $('#connectButton').on('click', doLogin);
}

function doLogin(event) {

    var username, password;
    event.stopPropagation();

    username = $('#login-username').val();
    password = $('#login-password').val();

    var alertMsg = "";

    if (!username ) {
        alertMsg = "please provide username";
    }

    if (!password) {
        alertMsg = "please provide a password";
    }

    if (alertMsg) {
        $(".alert").removeClass("hidden");
        $(".alert-text").html(alertMsg);
    }
    else {
        var data = 'username=' + username + '&password=' + password + '&grant_type=password';

        $.post('/token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } })
            .done(
                function (data) {
                    setUsername(username);
                    setToken(data.access_token);
                    if (previousUrl && previousUrl.indexOf('login.html') === -1) {
                        window.location = previousUrl;
                    } else {
                        window.location = "/Commits/commits.html";
                    }
                })
            .fail(
                function (xhr, textStatus, error) {
                    $(".alert").removeClass("hidden");
                    $(".alert-text").html(JSON.parse(xhr.responseText)["error_description"]);
                });
    }
}

function doLogout() {
    setUsername(null);
    setToken(null);
}


function registerReady() {
    $('#connectButton').on('click', doRegister);
}

function doRegister() {
    event.stopPropagation();

    var username = $('#newuser-username').val();
    var password = $('#newuser-password').val();
    var confirm = $('#newuser-confirm').val();
    var email = $('#newuser-email').val();
    var alertMsg = "";

    if (!username) {
        alertMsg = "Username required";
    }

    if (!password) {
        alertMsg = "Password required";
    }

    if (password.length < 8) {
        alertMsg = "Min 8 characters please";
    }

    if (!confirm) {
        alertMsg = "Password confirmation required";
    }

    if (password !== confirm) {
        alertMsg = "Entered password does not match confirmed";
    }

    if (!email) {
        alertMsg = "Email is required";
    }

    if (alertMsg) {
        $(".alert").removeClass("hidden");
        $(".alert-text").html(alertMsg);
    }
    else {

        var form = $('#register');
        $.post('/api/users/register', form.serialize())
            .done(
                function (data) {
                    setUsername(username);
                    var data = 'username=' + username + '&password=' + password + '&grant_type=password';
                    $.post('/token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } })
                        .done(
                            function (data) {
                                setUsername(username);
                                setToken(data.access_token);
                                window.location = "/Commits/commits.html";
                            })
                        .fail(
                        function (xhr, textStatus, error) {
                                $(".alert").removeClass("hidden");
                                $(".alert-text").html(JSON.parse(xhr.responseText)["error_description"]);
                            });
                })
            .fail(
                function (xhr, textStatus, error) {
                    $(".alert").removeClass("hidden");
                    $(".alert-text").html(JSON.parse(xhr.responseText)["error_description"]);
                });
    }
}

$(window).keydown(function (e) {
    switch (e.keyCode) {
        case 13:
            $(".btn-submit").click();
            return;
    }
});