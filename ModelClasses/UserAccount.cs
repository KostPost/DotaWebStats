using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelClasses;

[Table("user_account")]
public class UserAccount
{
    [Key] [Column("id")] public long Id { get; set; }
    [Column("user_name")] public string UserName { get; set; }
    [Column("steam_id")] public string SteamId { get; set; }
    [Column("current_mmr")] public int CurrentMMR { get; set; }
    [Column("goal_mmr")] public int GoalMMR { get; set; }
    [NotMapped] public int RegularMatches { get; set; }
    [NotMapped] public int DoubleMatches { get; set; }
}