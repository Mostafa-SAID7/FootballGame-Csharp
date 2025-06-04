// Advanced Console Football Game in C# with Full Features

using System;
using System.Collections.Generic;
using System.IO;

class FootballGame
{
    static Random rnd = new Random();
    static List<string> matchLog = new List<string>();
    static Dictionary<string, int> playerGoals = new();
    static Dictionary<string, int> playerCards = new();

    class Team
    {
        public string Name;
        public List<string> Players;
        public List<string> Substitutes;
        public string Formation;
        public string Strategy;
        public int Score = 0;

        public Team(string name)
        {
            Name = name;
            Players = new();
            Substitutes = new();
        }
    }

    static void Main()
    {
        Console.WriteLine("🏆 Welcome to the Tournament!");
        Console.Write("How many teams? (2 or 4): ");
        int teamCount = int.Parse(Console.ReadLine());

        List<Team> teams = new();
        for (int i = 1; i <= teamCount; i++)
        {
            Console.Write($"Enter name for Team {i}: ");
            Team team = new(Console.ReadLine());

            Console.WriteLine($"Enter 3 players for {team.Name}:");
            for (int j = 1; j <= 3; j++)
            {
                Console.Write($"Player {j}: ");
                string player = Console.ReadLine();
                team.Players.Add(player);
                playerGoals[player] = 0;
                playerCards[player] = 0;
            }

            Console.WriteLine($"Enter 2 substitutes for {team.Name}:");
            for (int j = 1; j <= 2; j++)
            {
                Console.Write($"Sub {j}: ");
                team.Substitutes.Add(Console.ReadLine());
            }

            Console.Write("Choose formation (e.g. 4-4-2, 3-5-2): ");
            team.Formation = Console.ReadLine();
            Console.Write("Choose strategy (Attack, Defend, Balanced): ");
            team.Strategy = Console.ReadLine();

            teams.Add(team);
        }

        Console.Clear();
        if (teamCount == 2)
            PlayMatch(teams[0], teams[1]);
        else
        {
            Console.WriteLine("Semi-Final 1:");
            PlayMatch(teams[0], teams[1]);
            Team winner1 = teams[0].Score > teams[1].Score ? teams[0] : teams[1];

            ResetScores(teams);
            Console.WriteLine("\nSemi-Final 2:");
            PlayMatch(teams[2], teams[3]);
            Team winner2 = teams[2].Score > teams[3].Score ? teams[2] : teams[3];

            ResetScores(teams);
            Console.WriteLine("\nFinal Match:");
            PlayMatch(winner1, winner2);
        }
    }

    static void ResetScores(List<Team> teams)
    {
        foreach (var team in teams) team.Score = 0;
    }

    static void PlayMatch(Team teamA, Team teamB)
    {
        Console.WriteLine($"\n🏟️ {teamA.Name} vs {teamB.Name}");
        Console.WriteLine($"{teamA.Name} Formation: {teamA.Formation}, Strategy: {teamA.Strategy}");
        Console.WriteLine($"{teamB.Name} Formation: {teamB.Formation}, Strategy: {teamB.Strategy}\n");

        matchLog.Clear();

        for (int min = 1; min <= 90; min++)
        {
            if (min == 45)
            {
                Console.WriteLine("\n⏸️ Half-time!\n");
                ShowScore(teamA, teamB);
                HandleSubstitutions(teamA);
                HandleSubstitutions(teamB);
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
            SimulateMinute(min, teamA, teamB);
            System.Threading.Thread.Sleep(50);
        }

        Console.WriteLine("\n🏁 Full Time!");
        ShowScore(teamA, teamB);

        if (teamA.Score == teamB.Score)
        {
            Console.WriteLine("⚔️ Draw! Going to penalties...");
            PenaltyShootout(teamA, teamB);
        }

        Console.WriteLine("\n📊 Player Stats:");
        foreach (var player in playerGoals.Keys)
            Console.WriteLine($"{player} - Goals: {playerGoals[player]}, Cards: {playerCards[player]}");

        SaveMatchLog(teamA, teamB);
    }

    static void SimulateMinute(int min, Team teamA, Team teamB)
    {
        int chance = rnd.Next(100);
        if (chance < 4)
        {
            string p = teamA.Players[rnd.Next(teamA.Players.Count)];
            teamA.Score++;
            playerGoals[p]++;
            Log($"🔥 Goal! {teamA.Name} - {p} at {min}'");
        }
        else if (chance < 8)
        {
            string p = teamB.Players[rnd.Next(teamB.Players.Count)];
            teamB.Score++;
            playerGoals[p]++;
            Log($"🔥 Goal! {teamB.Name} - {p} at {min}'");
        }
        else if (chance < 12)
        {
            string p = RandomPlayer(teamA, teamB);
            playerCards[p]++;
            Log($"🟨 Yellow Card - {p} at {min}'");
        }
        else if (chance < 14)
        {
            string p = RandomPlayer(teamA, teamB);
            playerCards[p] += 2;
            Log($"🟥 Red Card - {p} at {min}'");
        }
        else
        {
            Log($"⚽ Ball in play at {min}'");
        }
    }

    static void HandleSubstitutions(Team team)
    {
        Console.WriteLine($"{team.Name} - Substitution Time:");
        Console.Write("Enter player out: ");
        string outPlayer = Console.ReadLine();
        if (team.Players.Contains(outPlayer))
        {
            Console.Write("Enter sub in: ");
            string inPlayer = Console.ReadLine();
            if (team.Substitutes.Contains(inPlayer))
            {
                team.Players.Remove(outPlayer);
                team.Players.Add(inPlayer);
                team.Substitutes.Remove(inPlayer);
                team.Substitutes.Add(outPlayer);
                playerGoals[inPlayer] = 0;
                playerCards[inPlayer] = 0;
                Log($"🔄 {team.Name} substituted {outPlayer} with {inPlayer}");
            }
        }
    }

    static string RandomPlayer(Team a, Team b)
    {
        List<string> all = new(a.Players);
        all.AddRange(b.Players);
        return all[rnd.Next(all.Count)];
    }

    static void ShowScore(Team a, Team b)
    {
        Console.WriteLine($"Score: {a.Name} {a.Score} - {b.Score} {b.Name}\n");
    }

    static void PenaltyShootout(Team a, Team b)
    {
        int pa = 0, pb = 0;
        for (int i = 1; i <= 5; i++)
        {
            if (rnd.Next(2) == 1) { pa++; Log($"✅ {a.Name} scored penalty {i}"); } else Log($"❌ {a.Name} missed penalty {i}");
            if (rnd.Next(2) == 1) { pb++; Log($"✅ {b.Name} scored penalty {i}"); } else Log($"❌ {b.Name} missed penalty {i}");
        }
        a.Score = pa;
        b.Score = pb;
        Console.WriteLine($"Penalties Result: {a.Name} {pa} - {pb} {b.Name}");
    }

    static void Log(string msg)
    {
        matchLog.Add(msg);
        Console.WriteLine(msg);
    }

    static void SaveMatchLog(Team a, Team b)
    {
        string file = $"Match_{a.Name}_vs_{b.Name}_{DateTime.Now:yyyyMMddHHmmss}.txt";
        File.WriteAllLines(file, matchLog);
        Console.WriteLine($"\n📁 Match log saved to {file}");
    }
}
