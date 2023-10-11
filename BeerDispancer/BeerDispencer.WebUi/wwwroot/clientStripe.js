function onPaymentPageLoad(pubKey, clientSecret) {

  const stripe = Stripe(pubKey);

  // On page load, we create a PaymentIntent on the server so that we have its clientSecret to
  // initialize the instance of Elements below. The PaymentIntent settings configure which payment
  // method types to display in the PaymentElement.
  
  // Initialize Stripe Elements with the PaymentIntent's clientSecret,
  // then mount the payment element.
  const elements = stripe.elements({ clientSecret });
  const paymentElement = elements.create('payment');
  paymentElement.mount('#payment-element');
  // Create and mount the linkAuthentication Element to enable autofilling customer payment details
  const linkAuthenticationElement = elements.create("linkAuthentication");
  linkAuthenticationElement.mount("#link-authentication-element");
  // If the customer's email is known when the page is loaded, you can
  // pass the email to the linkAuthenticationElement on mount:
  //
  //   linkAuthenticationElement.mount("#link-authentication-element",  {
  //     defaultValues: {
  //       email: 'jenny.rosen@example.com',
  //     }
  //   })
  // If you need access to the email address entered:
  //
  //  linkAuthenticationElement.on('change', (event) => {
  //    const email = event.value.email;
  //    console.log({ email });
  //  })

  // When the form is submitted...
  const form = document.getElementById('payment-form');
  let submitted = false;
  form.addEventListener('submit', async (e) => {
    e.preventDefault();

    // Disable double submission of the form
    if(submitted) { return; }
    submitted = true;
    form.querySelector('button').disabled = true;

    const nameInput = document.querySelector('#name');

    // Confirm the payment given the clientSecret
    // from the payment intent that was just created on
    // the server.
    const {error: stripeError} = await stripe.confirmPayment({
      elements,
      confirmParams: {
        return_url: `${window.location.origin}/return.html`,
      }
    });

    if (stripeError) {
      addMessage(stripeError.message);

      // reenable the form.
      submitted = false;
      form.querySelector('button').disabled = false;
      return;
    }
  });

}

const addMessage = (message) => {
  const messagesDiv = document.querySelector('#messages');
  messagesDiv.style.display = 'block';
  const messageWithLinks = addDashboardLinks(message);
  messagesDiv.innerHTML += `> ${messageWithLinks}<br>`;
  console.log(`Debug: ${message}`);
};

// Adds links for known Stripe objects to the Stripe dashboard.
const addDashboardLinks = (message) => {
  const piDashboardBase = 'https://dashboard.stripe.com/test/payments';
  return message.replace(
    /(pi_(\S*)\b)/g,
    `<a href="${piDashboardBase}/$1" target="_blank">$1</a>`
  );
};