
function onPaymentPageInit(pubkey) {
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
        var successElement = document.querySelector('.success');
        var errorElement = document.querySelector('.error');
        successElement.classList.remove('visible');
        errorElement.classList.remove('visible');

        if (result.token) {
            // In this example, we're simply displaying the token
            successElement.querySelector('.token').textContent = result.token.id;
            successElement.classList.add('visible');

            // In a real integration, you'd submit the form with the token to your backend server
            var form = document.querySelector('form');
            form.querySelector('input[name="token"]').setAttribute('value', result.token.id);
            form.submit();
        } else if (result.error) {
            errorElement.textContent = result.error.message;
            errorElement.classList.add('visible');
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
        var options = {
            name: name,
            email: email
        };
        stripe.createToken(cardNumberElement, options).then(setOutcome);
    });

}