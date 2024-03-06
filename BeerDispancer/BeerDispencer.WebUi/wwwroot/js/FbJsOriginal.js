
let stateProviderInstance = null;

function InitSdk(stateProvider) {
    console.log('statusChangeCallback provider=' + stateProvider); 
    stateProviderInstance = stateProvider;
    var s = 'script';
    var id = 'facebook-jssdk';
    var js, fjs = document.getElementsByTagName(s)[0];
    if (document.getElementById(id)) { return; }
    js = document.createElement(s); js.id = id;
    js.src = "https://connect.facebook.net/en_US/sdk.js";
    fjs.parentNode.insertBefore(js, fjs);
}

function statusChangeCallback(response) {
    
    console.log('statusChangeCallback provider=' + stateProviderInstance); 

    if (response.status === 'connected') {
        console.log("Connected: UserId " + response.authResponse.userID + "Token " + response.authResponse.accessToken);

        stateProviderInstance.invokeMethodAsync("FacebookLoginAsync", { UserId: response.authResponse.userID, Token: response.authResponse.accessToken });   
    } else {
        document.getElementById('status').innerHTML = 'Please log ' +
            'into this app.';
    }
}

function fbAsyncInit() {
    FB.init({
        appId: '435876478881445',
        cookie: true,
        xfbml: true,
        version: 'v7.0'
    });
    document.getElementById('status').innerHTML = 'Inited';
}

function fbLogin() {
    window.FB.login(function (response) {
        statusChangeCallback(response, stateProvider)

    }, { scope: 'public_profile, email' });
}

//Dont delete -referenced from button
function checkLoginState() {
    FB.getLoginStatus(function (response) {
        statusChangeCallback(response);
    });
}
