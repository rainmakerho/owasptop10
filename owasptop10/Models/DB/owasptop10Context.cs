using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace owasptop10.Models.DB
{
    public partial class Owasptop10Context : DbContext
    {
         
        public Owasptop10Context(DbContextOptions<Owasptop10Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Users> Users { get; set; }

        public async Task<bool> LoginByUsernamePasswordAsync(string username, string passwordHash)
        {
            var queryUser = $"SELECT UserName FROM Users Where UserName='{username}' AND PasswordHash='{passwordHash}'";
            //var isExist = await Users.FromSql(queryUser).AnyAsync();
            var isExist = await Users.AnyAsync(u => u.UserName.Equals(username) && u.PasswordHash.Equals(passwordHash));
            return isExist;
        }
    }
}
