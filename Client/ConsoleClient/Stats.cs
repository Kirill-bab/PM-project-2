using ConsoleClient.Models;
using System;
using System.Collections.Generic;
using Library;
using System.Linq;

namespace ConsoleClient
{
    public static class Stats
    {
        public static void DisplayUser(Statistics userStats, string login)
        {
            Console.Clear();
            MenuManager.DrawHeader("your statistics", 100);
            Console.WriteLine($"\nTOTAL TIME IN GAME: {userStats.TotalTimeInGame}\n");            
            int session = 0;
            if (userStats.SessionsList.Count == 0)
            {
                Console.WriteLine("Your Session list is empty! Go Play!");
            }
            else 
            {
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine("SESSIONS LiST:");
                userStats.SessionsList.ForEach(Console.WriteLine);
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine($"AVERAGE WIN RATE: {CalculateAverageWinRate(userStats.SessionsList, login)}");

                Console.WriteLine("\n\nIf you want to see datails by each session, press Enter");
                if (Console.ReadKey(true).Key != ConsoleKey.Enter) return;

                var statsList = userStats.SessionsList.Select(s => s.ToString()).ToArray();
                session = MenuManager.Menu("Choose session", 100, statsList);
                var sessionArray = new string[userStats.SessionsList[session].Rounds.Count][];
                for (int i = 0; i < userStats.SessionsList[session].Rounds.Count; i++)
                {
                    if (login == userStats.SessionsList[session].Rounds[i].Winner)
                    {
                        sessionArray[i] = new string[]
                        {
                        userStats.SessionsList[session].Rounds[i].Winner,
                        userStats.SessionsList[session].Rounds[i].Looser,
                        userStats.SessionsList[session].Rounds[i].WinnerFigure.ToString(),
                        userStats.SessionsList[session].Rounds[i].LooserFigure.ToString(),
                        "You won"
                        };
                    }
                    else if(userStats.SessionsList[session].Rounds[i].Winner == "draw")
                    {
                        sessionArray[i] = new string[]
                       {
                        userStats.SessionsList[session].Rounds[i].Winner,
                        userStats.SessionsList[session].Rounds[i].Looser,
                        userStats.SessionsList[session].Rounds[i].WinnerFigure.ToString(),
                        userStats.SessionsList[session].Rounds[i].LooserFigure.ToString(),
                        "Draw"
                       };
                    }
                    else
                    {
                        sessionArray[i] = new string[]
                        {
                        userStats.SessionsList[session].Rounds[i].Looser,
                        userStats.SessionsList[session].Rounds[i].Winner,
                        userStats.SessionsList[session].Rounds[i].LooserFigure.ToString(),
                        userStats.SessionsList[session].Rounds[i].WinnerFigure.ToString(),
                        "You loose"
                        };
                    }
                    
                }
                TableBuilder.DrawTable(new string[]
                {
                "You",
                "Oponent",
                "Your figure",
                "Oponent's figure",
                "Result"
                },sessionArray);                
             }
            
            Console
                .WriteLine($"Your Win Rate in this Session: " +
                $"{CalculateSessionWinRate(userStats.SessionsList[session], login)}%");
        }
        public static void DisplayGlobal(List<Statistics> stats)
        {
            MenuManager.DrawHeader("Global rating", 100);
            Console.WriteLine();
            var GlobalRatings = new List<(string, double)>();
            foreach (var user in stats)
            {
                var userName = (DefineUser(user.SessionsList));
                GlobalRatings.Add((userName, CalculateAverageWinRate(user.SessionsList, userName)));
            }
            GlobalRatings = GlobalRatings.OrderBy(p => p.Item2).ToList();
            foreach (var user in GlobalRatings)
            {
                Console.WriteLine($"{user.Item1,15}{user.Item2}");
            }
        }
        private static int Sigm(int numb)
        {
            if (numb == 0) return 1;
            return numb;
        }
        private static double CalculateSessionWinRate(Session session, string login)
        {
            var winRate = (double)session.Rounds.Where(r => r.Winner == login).Count() /
               (double)Sigm(session.Rounds.Where(r => r.Winner != login).Count()) * 100;
            return winRate;
        }
        private static double CalculateAverageWinRate(List<Session> sessions,string login)
        {
            var avgWinRate = sessions.Select(s => CalculateSessionWinRate(s, login)).Average();
            return avgWinRate;
        }
        private static string DefineUser(List<Session> sessions)
        {
            var player = sessions[0].Player1;
            if (player == sessions[1].Player1) return player;
            else
            {
                 
            }
            return "";
        }
    }
}
