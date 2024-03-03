using System.Net.Http.Json;
using BeerDispenser.Shared.Dto;
using BeerDispenser.WebUi.Abstractions;
using Microsoft.JSInterop;

namespace BeerDispenser.WebUi.Implementation
{

    public class AccountStateChangedArgs:EventArgs
    {
        public bool IsLoggedIn { get; set; }
    }

    public class AccountService:IDisposable
    {
        private readonly ILocalStorage _localStorage;
       
        private readonly HttpClient _beClient;

        public delegate void AccountChangedEventHandler(object sender, AccountStateChangedArgs args);
        public event AccountChangedEventHandler OnAccountStateChanged;


        public string Password { private set; get; }
        public string Token { private set; get; }
        public string UserName { get; private set; }
        public UserRolesDto? Role { get; private set; }
        public DateTimeOffset? ValidUntil { get; private set; }
        public DateTimeOffset? Now { get; private set; }

        public string PictureUrl { get; set; }

        public bool IsLoggedIn { private set; get; }

        private bool _previousState;

        public AccountService(ILocalStorage localStorage, IHttpClientFactory httpClientFactory)
        {
            _localStorage = localStorage;
            _beClient = httpClientFactory.CreateClient("ServerAPI");
            _previousState = false;
        }


        public async Task InitializeAsync()
        {
            PictureUrl = "/images/no-avatar.png";

            var token = await ReadTokenasync();

            if (!string.IsNullOrEmpty(token))
            {
                 SetAccountProperties(token);
                RaiseloginEvent();
                Console.WriteLine("Properties set");
            }

            Task.Factory.StartNew(MonitorTokenExpirationAsync, TaskCreationOptions.LongRunning);
        }

        public async Task<(bool, string)> Login(string username, string password)
        {
            var loginModel = new LoginDto
            {
                UserName = username,
                Password = password
            };

            var response = await _beClient.PostAsJsonAsync("/api/Auth/login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                await _localStorage.SaveStringAsync("user", token);

                if (!string.IsNullOrEmpty(token))
                {
                    SetAccountProperties(token);
                    RaiseloginEvent();
                }
                return (true, string.Empty);
            }
            return (false, await response.Content.ReadAsStringAsync());
        }

        [JSInvokable]
        public async Task ProcessExternalUserAsync(string googleJwt)
        {
            var response = await _beClient.GetAsync($"/api/Auth/google-external-user/{googleJwt}");

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                await _localStorage.SaveStringAsync("user", token);

                if (!string.IsNullOrEmpty(token))
                {
                    SetAccountProperties(token);
                    RaiseloginEvent();
                }
            }
            {
                //handle error
            }
        }

        private void RaiseloginEvent()
        {
            OnAccountStateChanged?.Invoke(this, new AccountStateChangedArgs { IsLoggedIn = true });
        }

        private void RaiselogoutEvent()
        {
            OnAccountStateChanged?.Invoke(this, new AccountStateChangedArgs { IsLoggedIn = false });
        }

        private void SetAccountProperties(string jwtToken)
        {
           var (token, role, username, validUntil, validUntilLocalTime, pictureUrl) =   InitClaimsByFromJwt(jwtToken);
            Token = token;
            Role = role;
            UserName = username;
            ValidUntil = validUntil;
            Now = validUntilLocalTime;
            IsLoggedIn = true;
            PictureUrl = pictureUrl;
        }


        public async Task LogoutAsync()
        {
            IsLoggedIn = false;
            await _localStorage.RemoveAsync("user");
            Console.WriteLine("Logout");
            UserName = null;
            Password = null;
            Role = null;
            Token = null;
            ValidUntil = null;
            Now = null;
            PictureUrl = "/images/no-avatar.png";
            RaiselogoutEvent();
        }


        private (string Token,
            UserRolesDto Role,
            string Name,
            DateTimeOffset ValidUntil,
            DateTimeOffset ValidUntilLocalTime,
            string PictureUr)
            InitClaimsByFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);


            var jsonText = System.Text.Encoding.UTF8.GetString(jsonBytes);
            var keyValuePairs = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonText);

            UserRolesDto role = UserRolesDto.Unknown;
            string username = null;
            DateTimeOffset validUntil = default;
            DateTimeOffset localTime = default;
            string picture = null;
            foreach (var kv in keyValuePairs)
            {
                if (kv.Key.EndsWith("role"))
                {
                    role = Enum.Parse<UserRolesDto>(kv.Value.ToString());
                }

                if (kv.Key.EndsWith("nameidentifier"))
                {
                    username = kv.Value.ToString();
                }

                if (kv.Key.EndsWith("picture"))
                {
                    picture = kv.Value.ToString();
                }

                if (kv.Key.EndsWith("exp"))
                {
                    var utcValidUntil =DateTime.UnixEpoch.AddSeconds((long)kv.Value);
                    validUntil = utcValidUntil.ToLocalTime();
                    localTime = new DateTimeOffset(DateTime.Now);
                }
            }

            return (jwt, role, username, validUntil, localTime, picture);
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        private async Task<bool> IsTokenActive()
        {
            var token = await ReadTokenasync();

            if (token is null)
            {
                return false;
            }

             SetAccountProperties(token);

            if (Now is not null && ValidUntil is not null && Now < ValidUntil)
            {
                return true;
            }

            return false;
        }

        //private async Task<DateTimeOffset> ConvertToBrowserLocalTime(DateTime dt)
        //{
        //   var offset =  await _timeZoneService.GetUTCOffset();
        //    return new DateTimeOffset(dt, offset.Value);
        //}

        private async Task<string> ReadTokenasync()
        {
            return await _localStorage.GetStringAsync("user");
        }

        private async Task MonitorTokenExpirationAsync()
        {
            Console.WriteLine("MonitorTokenExpirationAsync job started");
            while (true)
            {
                IsLoggedIn = await IsTokenActive();

                Console.WriteLine($"IsLoggedIn={IsLoggedIn}");

                if (_previousState!= IsLoggedIn)
                {
                    if (!IsLoggedIn)
                    {
                        await LogoutAsync();
                        Console.WriteLine($"Token has expired Now={Now} ValidUntil={ValidUntil}");
                    }
                    else
                    {
                        Console.WriteLine("Token still alive");
                    }

                }

                _previousState = IsLoggedIn;
                await Task.Delay(1000);

            }
        }

        public void Dispose()
        {
            
        }
    }
}

