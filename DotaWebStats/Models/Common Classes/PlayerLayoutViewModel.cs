using DotaWebStats.Services;

namespace DotaWebStats.Models.Common_Classes;

public class PlayerLayoutViewModel
{
    public long PlayerId { get; set; }
    
    public UserDotaStats? UserDotaStats { get; set; }
    
}