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
        public string Dota2Id { get; set; } 
        public string PersonaName { get; set; }
        public string AvatarFull { get; set; }
        public string Rank { get; set; }
    }
}

