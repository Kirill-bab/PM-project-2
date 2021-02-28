using Library;
using System.Text.Json;
using System;
using System.Net.Http;
using ConsoleClient.Models;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace ConsoleClient
{
    class SessionPerformer
    {
        private UserAccount _account;
        private bool _connect;
        private bool _checkSearch;
        private int _width = 100;
        private ConnectionSettings _connectionSettings;
        private HttpClient _httpClient;
        private int _tries;
        private bool _inGame;
        private bool _inRound;
        public SessionPerformer(UserAccount account)
        {
            _account = account;
            _connectionSettings = JsonSerializer
                .Deserialize<ConnectionSettings>(FileWorker.Read("connectionSettings.json"));
            
            Uri uri = new Uri(_connectionSettings.BaseAddress);
            _httpClient = new HttpClient()
            {
                BaseAddress = uri
            };
            _inGame = false;
            _checkSearch = false;
            _connect = false;
            _inRound = false;
            _tries = 0;
        }
        public void Run()
        {
            var exit = false;
            while (!exit)
            {
                var option = MenuManager.Menu("Main Menu", _width, new string[]
                {
                    "Register",
                    "Authorize",
                    "Global Ratings",
                    "Exit"
                });
                switch(option)
                {
                    case 0:
                        {
                            Register();
                            Console.WriteLine("\n Press any key to continue...");
                            Console.ReadKey(true);
                            break;
                        }
                    case 1:
                        {
                            Authorize();
                            Console.WriteLine("\n Press any key to continue...");
                            Console.ReadKey(true);
                            break;
                        }
                    case 2:
                        {
                            GetGlobalStats();
                            Console.WriteLine("\n Press any key to continue...");
                            Console.ReadKey(true);
                            break;
                        }
                    case 3:
                        exit = true;
                        break;
                }
            }
        }
        private void Register()
        {
            Console.Clear();
            MenuManager.DrawHeader("Registration", _width);
            Console.WriteLine("Enter Login: ");
            var login = Console.ReadLine().Trim();
            while (string.IsNullOrEmpty(login))
            {
                Console.WriteLine("Empty login! Try again!");
                login = Console.ReadLine().Trim();
            }
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Enter Password: ");
            var password = Console.ReadLine().Trim();
            while (string.IsNullOrEmpty(password) || password.Length < 6)
            {
                if (password.Length < 6)
                {
                    Console.WriteLine("Password must contain at least 6 symbols! Try Again!");
                }
                else
                {
                    Console.WriteLine("Empty login! Try again!");
                }
                password = Console.ReadLine().Trim();
            }

            var response = Task.Run<HttpResponseMessage>(async () =>
            {
                return await _httpClient.GetAsync($"user/register?login={login}&password={password}");
            }).Result;
            Console.WriteLine("---------------------------------");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine("Registration Successfull!");
            }
            else
            {
                Console.WriteLine("User with such Login already exists!");
            }           
        }
        private void Authorize()
        {
            Console.Clear();
            MenuManager.DrawHeader("Authorization", _width);
            Console.WriteLine("Enter Login: ");
            var login = Console.ReadLine().Trim();
            while (string.IsNullOrEmpty(login))
            {
                Console.WriteLine("Empty login! Try again!");
                login = Console.ReadLine().Trim();
            }
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Enter Password: ");
            var password = Console.ReadLine().Trim();
            while (string.IsNullOrEmpty(password) || password.Length < 6)
            {
                if (password.Length < 6)
                {
                    Console.WriteLine("Password must contain at least 6 symbols! Try Again!");
                }
                else
                {
                    Console.WriteLine("Empty login! Try again!");
                }
                password = Console.ReadLine().Trim();
            }

            var response = Task.Run<HttpResponseMessage>(async () =>
            {
                return await _httpClient
                .GetAsync($"user/authorize?login={login}&password={password}&tries={_tries}");
            }).Result;
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK :
                    {
                        StartPlatformSession(login, password);
                        break;
                    }
                case HttpStatusCode.NotFound:
                    {
                        Console.WriteLine($"User with login {login} not registered on platform!");
                        break;
                    }
                case HttpStatusCode.Conflict:
                    {
                        Console.WriteLine($"User {login} is already on platform!");
                        break;
                    }
                case HttpStatusCode.NoContent:
                    {
                        Console.WriteLine($"User {login} was recently banned on platform!");
                        Console.WriteLine("Try again later.");
                        break;
                    }
                case HttpStatusCode.BadRequest:
                    {
                        Console.WriteLine("Wrong password!");
                        _tries++;
                        break;
                    }
                case HttpStatusCode.Unauthorized:
                    {
                        Console.WriteLine("You entered wrong password 3 times in a row! \n Now you are banned for 3 minutes!");
                        _tries = 0;
                        break;
                    }
            }
        }
        private void GetGlobalStats()
        {
            var response = Task.Run(async () => await _httpClient.GetAsync("user/stats/global")).Result;
            var stats = JsonSerializer
                .Deserialize<List<Statistics>>(Task
                .Run(async () => await response.Content.ReadAsStringAsync()).Result);
            Stats.DisplayGlobal(stats);
        }
        private void StartPlatformSession(string login, string password)
        {
            _account.Login = login;
            _account.Password = password;
            Console.Clear();
            _connect = true;
            StayConnected();
            var exit = false;
            while (!exit)
            {
                Console.Clear();
                var option = MenuManager.Menu("Welcome to Rpc gaming platform!", _width, new string[]
                    {
                    "See My Stats",
                    "Play Random Game",
                    "Create Private Room",
                    "Connect To Private Room",
                     "See Global Stats",
                    "Exit Platform"
                    });
                switch (option)
                {
                    case 0:
                        {
                            SeeStats();
                            Console.WriteLine("\n Press any key to continue...");
                            Console.ReadKey(true);
                            break;
                        }
                    case 1:
                        {
                            StartRandomGameSearch();
                            Console.WriteLine("\n Press any key to continue...");
                            Console.ReadKey(true);
                            break;
                        }
                    case 2:
                        {
                            OpenPrivateRoom();
                            Console.WriteLine("\n Press any key to continue...");
                            Console.ReadKey(true);
                            break;
                        }
                    case 3:
                        {
                            EnterPrivateRoom();
                            Console.WriteLine("\n Press any key to continue...");
                            Console.ReadKey(true);
                            break;
                        }
                    case 4:
                        {
                            GetGlobalStats();
                            Console.WriteLine("\n Press any key to continue...");
                            Console.ReadKey(true);
                            break;
                        }
                    case 5:
                        exit = true;
                        _connect = false;
                        break;
                }
            }
        }

        private void EnterPrivateRoom()
        {
            throw new NotImplementedException();
        }

        private void OpenPrivateRoom()
        {
            throw new NotImplementedException();
        }

        private void SeeStats()
        {
            var response = Task
                .Run(async () => await _httpClient.GetAsync($"user/stats?login={_account.Login}")).Result;
            var content = Task.Run(async () => await response.Content.ReadAsStringAsync()).Result;
            var userStats = JsonSerializer.Deserialize<Statistics>(content, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            Stats.DisplayUser(userStats, _account.Login);
        }

        private void StartRandomGameSearch()
        {
            Console.Clear();
            MenuManager.DrawHeader("Random Game Search", _width);
            Console.WriteLine("Press ESC to stop search.");
            Console.WriteLine("\n\n");
            _checkSearch = true;            
            Console.WriteLine(TableBuilder.AlignCentre("Searching for second player...", _width));
            CancellationTokenSource cts = new CancellationTokenSource();
            GoInQueue(cts.Token);            
            do
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Escape) 
                {
                    _checkSearch = false;
                    cts.Cancel();
                    var result = Task.Run(async () =>
                    {
                        return await _httpClient
                        .GetAsync($"user/session/stop/search?login={_account.Login}");
                    }).Result;
                    return;
                }
            } while (_checkSearch);
            cts.Cancel();
            Console.WriteLine(TableBuilder.AlignCentre("Match Found!",_width));
            StartSeries();
        }

        private void StartSeries()
        {
            _inGame = true;
            int counter = 0;
            CheckGame();
            while (_inGame)
            {
                StartRoundTimer();
                string figure = "";
                Console.Clear();
                var option = MenuManager.Menu($"Round {counter}", _width, new string[]
                    {
                        "Parer",
                        "Rock",
                        "Scissors",
                        "Quit Match"
                    });
                switch (option)
                {
                    case 0: 
                        figure = "paper";
                        break;
                    case 1:
                        figure = "paper";
                        break;
                    case 2:
                        figure = "paper";
                        break;
                    case 3:
                        {
                            _inGame = false;
                            var r = Task.Run(async () =>
                            {
                                return await _httpClient.GetAsync($"user/quit/game?login={_account.Login}");
                            }).Result;                            
                            break;
                        }             
                }
                var response = Task.Run(async () =>
                {
                    return await _httpClient
                       .GetAsync($"user/session/figure?login={_account.Login}&figure={figure}");
                }).Result;

                _inRound = Task.Run(async () =>
                {
                    return await _httpClient
                       .GetAsync($"user/check/round?login={_account.Login}");
                }).Result.StatusCode ==HttpStatusCode.OK;
                
                while (_inRound)
                {
                    _inRound = Task.Run(async () =>
                    {
                        await Task.Delay(1000);
                        return await _httpClient
                           .GetAsync($"user/check/round?login={_account.Login}");
                    }).Result.StatusCode == HttpStatusCode.OK;
                }
                if (_inGame)
                {
                    var result = Task.Run(async () =>
                    {
                        return await _httpClient
                           .GetAsync($"user/round/result?login={_account.Login}");
                    }).Result.StatusCode;
                    switch (result)
                    {
                        case HttpStatusCode.OK : 
                            {
                                Console.WriteLine(TableBuilder.AlignCentre("You won!",_width));
                                break;
                            }
                        case HttpStatusCode.NotFound:
                            {
                                Console.WriteLine(TableBuilder.AlignCentre("You lost!",_width));
                                break;
                            }
                        case HttpStatusCode.NoContent:
                            {
                                Console.WriteLine(TableBuilder.AlignCentre("Draw!",_width));
                                break;
                            }
                    }
                    var temp = Task.Run(async () =>
                    {
                        await Task.Delay(1000);
                        return true;
                    }).Result;
                }
                counter++;
            }
            Console.WriteLine("\n" + TableBuilder.AlignCentre("Match Ended!",_width));
        }

        private void StayConnected()
        {
            Task.Run(() =>
            {
                var timer = new Timer(async callback =>
                {
                    var response = await _httpClient
                    .GetAsync($"user/confirm/connection?login={_account.Login}");
                    if (response.StatusCode != HttpStatusCode.OK) _connect = false;
                }, null, 0, 1000);

                while (_connect)
                {

                }
                timer.Dispose();
            });
        } 
        private void GoInQueue(CancellationToken ct)
        {
            Task.Run(async () =>
            {
                WaitInQueueAnimation();
                await _httpClient.GetAsync($"user/session/start/random?login={_account.Login}");
                do
                {
                    await Task.Delay(3000);
                    if (ct.IsCancellationRequested) break;
                    _checkSearch = (await _httpClient
                    .GetAsync($"user/check/inqueue?login={_account.Login}")).StatusCode == HttpStatusCode.OK;                  
                } while (_checkSearch);
            }, ct);
            _checkSearch = false;
        }
        private void WaitInQueueAnimation()
        {
            Task.Run(async () =>
            {
                do
                {
                    Console.SetCursorPosition(63, 7);
                    Console.WriteLine("  ");
                    await Task.Delay(1000);
                    Console.SetCursorPosition(63, 7);
                    Console.WriteLine(". ");
                    await Task.Delay(1000);
                    Console.SetCursorPosition(63, 7);
                    Console.WriteLine("..");
                    await Task.Delay(1000);
                } while (_checkSearch);
            });
        }
        private void CheckGame()
        {
            Task.Run(async () =>
            {
                while (_inGame)
                {
                    _inGame = (await _httpClient
                    .GetAsync($"user/check/game?login={_account.Login}")).StatusCode == HttpStatusCode.OK;
                    await Task.Delay(2000);
                }
            });
        }

        private void StartRoundTimer()
        {
            Console.SetCursorPosition(80, 8);
            Console.WriteLine("Time Left:");
            Task.Run(async () =>
            {
                int timer = 20;
                while (_inRound)
                {
                    Console.SetCursorPosition(80, 9);
                    Console.Write("  ");
                    Console.SetCursorPosition(80, 9);
                    Console.Write(timer);
                    timer--;
                    await Task.Delay(1000);
                }
            });
        }
    }
}
