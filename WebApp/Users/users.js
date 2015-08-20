
function setUsername1(username) {
    window.sessionStorage.setItem('username', username);
}

function getUsername1() {
    return window.sessionStorage.getItem('username');
}

function setPassword1(password) {
    window.sessionStorage.setItem('password', password);
}

function getPassword1() {
    return window.sessionStorage.getItem('password');
}

$(document).ajaxSend(
           function (event, jqxhr, settings) {
               var data = getUsername1() + ":" + getPassword1();
               jqxhr.setRequestHeader("Authorization", "Basic " + btoa( data ));
           });

$(document).ajaxError(
    function (event, jqxhr, settings, message) {
        if (jqxhr.status === 401) {
            alert("authen error ");
        }
    });

function authenticate() {

    if ( getUsername1() && getPassword1() ) {
        window.location = "Commits/commits.html";
    }
    else {
        window.location = "login.html"; // with redirect
    }
}

function showLogin() {
    // get dialog
    // insert form
    // set actions on the buttons
}

function closeLogin() {
    // remove content
    // close dialog
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

    setUsername1(username);
    setPassword1(password);

    $.post('api/users/login')
        .done(
            function () {
                $('#log').html("ok");
                window.location = "Commits/commits.html";
            })
        .fail(
            function (xhr, textStatus, error) {
                // TODO print message and allow retry
                $('#log').html(xhr.statusText);
                setUsername1(null);
                setPassword1(null);
            });
    return false;
}

function doLogout() {
    setUsername1(null);
    setPassword1(null);
}

function createUser() {

    var userName = $('#newuser-username').val();
    var password = $('#newuser-password').val();
    var confirm = $('#newuser-confirm').val();

    alert($('#newuser-username').val());
    if (!userName) {
        alert("username required");
        return;
    }

    if (!password) {
        alert("password required");
        return;
    }

    if (password.length < 8) {
        alert("min 8 caracters please");
        return;
    }

    if (!confirm) {
        alert("password confirmation rquired");
        return;
    }

    if (password !== confirm) {
        alert("entered password does not match confirmed");
        return;
    }

    $.post('api/users/create/' + userName + '?password=' + encodeURIComponent(password))
        .done(
            function () {
                window.location = "Commits/commits.html";
            })
        .fail(
            function (xhr, textStatus, error) {
                alert(xhr.responseJSON.ExceptionMessage);
            });
}