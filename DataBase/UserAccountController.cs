using Microsoft.EntityFrameworkCore;
using ModelClasses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataBase
{
    public class UserAccountController
    {
        private readonly DotaWebStatsContext _context;

        public UserAccountController(DotaWebStatsContext context)
        {
            _context = context;
        }

        public async Task<List<UserAccount>> GetAccountsAsync()
        {
            return await _context.UserAccounts.ToListAsync();
        }

        public async Task DeleteAccountAsync(long id)
        {
            var account = await _context.UserAccounts.FindAsync(id);
            if (account != null)
            {
                _context.UserAccounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }

        public int CalculateRegular(UserAccount account)
        {
            return (account.GoalMMR - account.CurrentMMR) / 20;
        }

        public int CalculateDouble(UserAccount account)
        {
            return (account.GoalMMR - account.CurrentMMR) / 40;
        }
    }
}