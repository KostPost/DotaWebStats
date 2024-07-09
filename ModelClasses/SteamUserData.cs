namespace ModelClasses;

public class SteamUserData
{
    public class SteamPlayerSummary
    {
        public Response Response { get; set; }
    }

    public class Response
    {
        public List<Player> Players { get; set; }
    }

    public class Player
    {
        public string Steamid { get; set; }
        public long Dota2Id { get; set; } 
        public string PersonaName { get; set; }
        public string AvatarFull { get; set; }
        public string Rank { get; set; }
        public string ProfileUrl { get; set; }
        public string Avatar { get; set; }
    }
}

public class MatchStats
{
    public int TotalMatches { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public double WinRate => TotalMatches > 0 ? (double)Wins / TotalMatches : 0;
}