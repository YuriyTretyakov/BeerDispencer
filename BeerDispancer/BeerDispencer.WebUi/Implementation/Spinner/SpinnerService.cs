
using Microsoft.JSInterop;

namespace BeerDispenser.WebUi.Implementation.Spinner
{



    public class SpinnerService
    {
        private readonly IJSRuntime _jSRuntime;

        public event Action OnShow;
        public event Action OnHide;

        public SpinnerService(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }
        public void Show()
        {
            _jSRuntime.InvokeVoidAsync("showSpinner");
            Console.WriteLine("Spinner show");
            OnShow?.Invoke();
        }

        public void Hide()
        {
            _jSRuntime.InvokeVoidAsync("hideSpinner");
            Console.WriteLine("Spinner Hide");
            OnHide?.Invoke();
        }
    }
}