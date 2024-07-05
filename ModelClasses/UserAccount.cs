using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelClasses
{
    [Table("user_account")]
    public class UserAccount
    {
        [Key] [Column("id")] public long Id { get; set; }

        [Required(ErrorMessage = "User Name is required")]
        [Column("user_name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Steam ID is required")]
        [Column("steam_id")]
        public string SteamId { get; set; }

        [Required(ErrorMessage = "Current MMR is required")]
        [Column("current_mmr")]
        public int CurrentMMR { get; set; }

        [Required(ErrorMessage = "Goal MMR is required")]
        [Column("goal_mmr")]
        public int GoalMMR { get; set; }

        // Additional properties not mapped to database columns
        [NotMapped] public int RegularMatches { get; set; }

        [NotMapped] public int DoubleMatches { get; set; }

        public UserAccount()
        {
            if (CurrentMMR != 0 && GoalMMR != 0)
            {
                RegularMatches = (GoalMMR - CurrentMMR) / 20;
                DoubleMatches = (GoalMMR - CurrentMMR) / 40;
            }
        }
    }
}