using System;
using Microsoft.JSInterop;

namespace BeerDispenser.WebUi.Implementation
{
    public sealed class TimeZoneService
    {
        private readonly IJSRuntime _jsRuntime;

        public TimeSpan? UserOffset;

        public TimeZoneService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async ValueTask<DateTimeOffset> GetLocalDateTime(DateTimeOffset dateTime)
        {
            if (UserOffset == null)
            {
                int offsetInMinutes = await _jsRuntime.InvokeAsync<int>("blazorGetTimezoneOffset");
                UserOffset = TimeSpan.FromMinutes(-offsetInMinutes);
            }

            return dateTime.ToOffset(UserOffset.Value);
        }
    }
}


