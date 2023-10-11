'use strict';


var stripe = Stripe("pk_test_51NpWMxBwrd9g8wB1Kl1fBZ4P25KtfUwBobeHU8WPORfQBQtdXb446cMP3R7DpSxiNgRSGMPUfu7hdGQpNokBOiRx00VhUbVBJa");
var issubmitting;

function onCreateTokenLoad(pubKey) {

    issubmitting = false;
    //var stripe = Stripe(pubKey);


    var elements = stripe.elements({
        fonts: [
            {
                cssSrc: 'https://fonts.googleapis.com/css?family=Quicksand',
            },
        ],

        locale: window.__exampleLocale,
    });

    var cardNumber = elements.create('cardNumber');
    cardNumber.mount('#custom-card-number');

    var cardExpiry = elements.create('cardExpiry');
    cardExpiry.mount('#custom-card-expiry');
    var cardCvc = elements.create('cardCvc');
    cardCvc.mount('#custom-card-cvc');

   // const form = document.getElementById("payment-form")
    var name = document.getElementById('name');
    var email = document.getElementById('email');


    registerElements([cardNumber, cardExpiry, cardCvc, name, email]);


    function registerElements(elements) {


        var error = document.querySelector('.error');
        var errorMessage = error.querySelector('.message');

        var submitButton = document.getElementById("submit");

        //elements.forEach((element) =>

        //    element.addEventListener("change", (event) =>
        //        onFormElementChange(event, errorMessage, submitButton)));

        submitButton.addEventListener("click", (event) => {
            event.preventDefault();
            //var name = form.querySelector('#name');
            //var email = form.querySelector('#email');
            if (!issubmitting) {

                var additionalData = {
                    name: name ? name.value : undefined,
                    email: email ? email.value : undefined,
                };

                console.log(elements[0]);

                issubmitting = true;
                stripe.createToken(elements[0], additionalData)
                    .then(function (result) {
                        processCardToken(result)
                    })
                    .catch(error => {
                        console.log("ERROR:" + error);
                        errorMessage.style.visibility = "visible"
                        errorMessage.textContent = error.message
                    });
            }
        });
    }
}

    //function onSubmit() {

    //    const form = document.getElementById("payment-form")
    //    var name = form.querySelector('#name');
    //    var email = form.querySelector('#email');

    //    var additionalData = {
    //        name: name ? name.value : undefined,
    //        email: email ? email.value : undefined,
    //    };

        

    //    console.log(this.cardNumber);

    //    stripe.createToken(this.cardNumber, additionalData).then(function (result) {
    //        processCardToken(result)
    //    });
    //}





function onFormElementChange(event, errorMessage, submitButton) {

    if (event.error) {
        errorMessage.textContent = event.error.message;
        errorMessage.style.visibility = "visible"

        document.getElementById('formValidState').value = "false";
       
        console.log(submitButton);
    }
    else {
        errorMessage.style.visibility = "hidden"
        document.getElementById('formValidState').value = "true";
        
        console.log(submitButton);
    }
}


    function processCardToken(result) {
        console.log("processCardToken:" + result);


        if (result.token) {
            document.getElementById('token').value = result.token.id;
            document.getElementById('formValidState').value = "false";
        } else {
            document.getElementById('formValidState').value = "true";
        }
        issubmitting = false;
    }