using System;
using System.Collections.Generic;

namespace owasptop10.Models.DB
{
    public partial class Users
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }
}
