namespace DotaWebStats.Models;

public class Profile
{
    public string Personaname { get; set; }
    public string? Name { get; set; } 
    public bool Plus { get; set; }
    public int Cheese { get; set; }
    public string Steamid { get; set; }
    public string Avatar { get; set; }
    public string AvatarMedium { get; set; }
    public string AvatarFull { get; set; }
    public string Profileurl { get; set; }
    public DateTime? LastLogin { get; set; }
    public string? Loccountrycode { get; set; } 
    public string? Status { get; set; }  
    public bool FhUnavailable { get; set; }
    public bool IsContributor { get; set; }
    public bool IsSubscriber { get; set; }
}