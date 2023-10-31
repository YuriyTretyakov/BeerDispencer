
function onPaymentPageInit(pubkey, beAddCardUrl) {
    var stripe = Stripe(pubkey);
    var elements = stripe.elements();

    var style = {
        base: {
            iconColor: '#666EE8',
            color: '#31325F',
            lineHeight: '40px',
            fontWeight: 300,
            fontFamily: 'Helvetica Neue',
            fontSize: '15px',

            '::placeholder': {
                color: '#CFD7E0',
            },
        },
    };

    var options;

    var cardNumberElement = elements.create('cardNumber', {
        style: style
    });
    cardNumberElement.mount('#card-number-element');

    var cardExpiryElement = elements.create('cardExpiry', {
        style: style
    });
    cardExpiryElement.mount('#card-expiry-element');

    var cardCvcElement = elements.create('cardCvc', {
        style: style
    });
    cardCvcElement.mount('#card-cvc-element');


    function setOutcome(result) {
        hideError();
        hideSuccess();
        
        if (result.token) {
            console.log(result);
            showSuccess("Token has been provided by payment gateway");
            sendPaymentDetailsToServer(beAddCardUrl, result)
            
        } else if (result.error) {

            showError(result.error.message);
        }
    }

    cardNumberElement.on('change', function (event) {
        setOutcome(event);
    });

    cardExpiryElement.on('change', function (event) {
        setOutcome(event);
    });

    cardCvcElement.on('change', function (event) {
        setOutcome(event);
    });

    document.querySelector('form').addEventListener('submit', function (e) {

        var email = document.getElementById('email-element').value;
        var name = document.getElementById('accountholder-element').value;

        e.preventDefault();
        options = {
            name: name,
            email: email
        };
        stripe.createToken(cardNumberElement, options).then(setOutcome);
    });

    function doFormSubmit() {
        var form = document.querySelector('form');
        form.submit();
    }

    function showError(errorText) {
        var errorElement = document.querySelector('.error');
        errorElement.textContent = errorText;
        errorElement.classList.add('visible');
    }

    function hideError() {
        var errorElement = document.querySelector('.error');
        errorElement.textContent = "";
        errorElement.classList.remove('visible');
    }

    function showSuccess(success) {
        var successElement = document.querySelector('.success');
        successElement.textContent = success;
        successElement.classList.add('visible');
    }

    function hideSuccess() {
        var successElement = document.querySelector('.success');
        successElement.classList.remove('visible');
        successElement.textContent = "";
    }

    function sendPaymentDetailsToServer(url, tokenResponse) {

        console.log(tokenResponse);
        var token = document.getElementById("jwttoken").value;
        fetch(url, {
            method: "POST",
            body: JSON.stringify(tokenResponse),
            headers: {
                "Content-type": "application/json; charset=UTF-8",
                "Authorization": "Bearer " + token
            }
        })
            .then((response) => {
                hideError();
                console.log(response.json());
                doFormSubmit();
            })
            .catch((error) => {
                hideSuccess();
                showError(error)
            });
    }

}