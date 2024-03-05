
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

      //  DotNet.invokeMethodAsync('BeerDispenser.WebUi', 'FacebookLoginAsync', { UserId: response.authResponse.userID, Token: response.authResponse.accessToken });

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


//async function getUserData(userId, accessToken) {
//    try {
        
//        const response = await fetch(`https://graph.facebook.com/v13.0/${userId}?fields=email,id,picture,first_name,last_name,name&access_token=${accessToken}`);
//        //https://graph.facebook.com/me?fields=email,id,picture,first_name,last_name,name&access_token=EAAGMbWawjqUBOzamreR4hvWmqY9dqTHqpWKrSq087evQzTc2JiRkUFZA080Un8Mil4I3QqRdPbjXzdB2BSlZBOvMMMiiFy9wqxpsGJJP4skPr6ZCnZBiDTpHUZBbuhTzicS5Czb2YxD2cv4R1ev9ZBEZCIPecwXX1ZAZCCnldX8mZABe1J2A4uQlXvpKpR0CeqTlGo6RYiyquTz7G0QPFnAAZDZD
//        // Проверка на успешный ответ
//        if (!response.ok) {
//            console.log('Unable to retrieve user data');
//        }

//        // Парсинг ответа в формат JSON
//        const userData = await response.json();

//        // Вывод данных пользователя в консоль (вы можете использовать их по вашему усмотрению)
//        console.log('User Data:', userData);

//    } catch (error) {
//        console.error('error:', error.message);
//    }
//}
