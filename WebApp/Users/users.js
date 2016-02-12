
function setUsername(username) {
    window.sessionStorage.setItem('username', username);
}

function getUsername() {
    return window.sessionStorage.getItem('username');
}

function setToken(token) {
    window.sessionStorage.setItem('token', token);
}

function getToken() {
    return window.sessionStorage.getItem('token');
}

$(document).ajaxSend(
           function (event, jqxhr, settings) {
               jqxhr.setRequestHeader("Authorization", "Bearer " + getToken() );
           });

$(document).ajaxError(
    function (event, jqxhr, settings, message) {
        if (jqxhr.status === 401) {
            alert("You are not authorizedto access this resosurce");
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

    $('#connectButton').on('click', doLogin);
}

function doLogin(event) {

    var username, password;
    event.stopPropagation();

    username = $('#login-username').val();
    password = $('#login-password').val();

    if (!username ) {
        alert("please provide username");
        return;
    }

    if (!password) {
        alert("please provide a password");
        return;
    }

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
                alert( JSON.parse(xhr.responseText)["error_description"] );
            });

    return false;
}

function doLogout() {
    setUsername(null);
    setToken(null);
}


function registerReady() {
    $('#connectButton').on('click', doRegister);
}

function doRegister() {

    var username = $('#newuser-username').val();
    var password = $('#newuser-password').val();
    var confirm = $('#newuser-confirm').val();
    var email = $('#newuser-email').val();

    if (!username) {
        alert("Username required");
        return;
    }

    if (!password) {
        alert("Password required");
        return;
    }

    if (password.length < 8) {
        alert("Min 8 characters please");
        return;
    }

    if (!confirm) {
        alert("Password confirmation required");
        return;
    }

    if (password !== confirm) {
        alert("Entered password does not match confirmed");
        return;
    }

    if (!email) {
        alert("Email is required");
    }

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
                            alert(JSON.parse(xhr.responseText)["error_description"]);
                        });
                 })
        .fail(
            function (xhr, textStatus, error) {
                alert(xhr.statusText);
            });
}