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
            if (userStats.SessionsList.Count == 0)
            {
                Console.WriteLine("Your Session list is empty! Go Play!");
            }
            else 
            {
                var statsList = userStats.SessionsList.Select(s => s.ToString()).ToArray();
                var session = MenuManager.Menu("Choose session", 100, statsList);
                var sessionArray = new string[userStats.SessionsList.Count][];
                for (int i = 0; i < userStats.SessionsList.Count; i++)
                {
                    if (login == userStats.SessionsList[session].Rounds[i].Winner.Item1)
                    {
                        sessionArray[i] = new string[]
                        {
                        userStats.SessionsList[session].Player1,
                        userStats.SessionsList[session].Player2,
                        userStats.SessionsList[session].Rounds[i].Winner.Item2.ToString(),
                        userStats.SessionsList[session].Rounds[i].Looser.Item2.ToString(),
                        "You won"
                        };
                    }
                    else
                    {
                        sessionArray[i] = new string[]
                        {
                        userStats.SessionsList[session].Player1,
                        userStats.SessionsList[session].Player2,
                        userStats.SessionsList[session].Rounds[i].Looser.Item2.ToString(),
                        userStats.SessionsList[session].Rounds[i].Winner.Item2.ToString(),
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
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }
        public static void DisplayGlobal(List<Statistics> stats)
        {
            var user = true;
        }
    }
}
