using Radzen;

namespace BeerDispenser.WebUi.Implementation
{
    public class UserNotificationService
	{
        private readonly NotificationService _notificationService;

        public UserNotificationService(NotificationService NotificationService)
		{
            _notificationService = NotificationService;
        }

        public void ShowUnauthorizedNotification()
        {
            Console.WriteLine($"Unauthorized error");
            var errorMessage = new NotificationMessage
            {
                Summary = "Authentication error",
                Detail = "You have to login to continue",
                Severity = NotificationSeverity.Warning,
                Duration = 4000
            };
            _notificationService.Notify(errorMessage);
            
        }

        public void ShowErrorNotification(string error)
        {
            Console.WriteLine($"Error while proccessing request");

            var errorMessage = new NotificationMessage
            {
                Summary = "Error",
                CloseOnClick = true,
                Detail = error,
                Severity = NotificationSeverity.Error,
                Duration = 4000
            };

            _notificationService.Notify(errorMessage);
        }

        public void ShowSuccessNotification(string message = null)
        {
            Console.WriteLine($"Operation Success");

            var errorMessage = new NotificationMessage
            {
                Summary = message?? "Success",
                CloseOnClick = false,
                Detail = "Opearion successfully completed",
                Severity = NotificationSeverity.Success,
                Duration = 10000,
                
            };
            _notificationService.Notify(errorMessage);
        }
    }
}

