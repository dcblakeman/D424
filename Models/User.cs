using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SQLite;

namespace C_971.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [SQLite.MaxLength(100)]
        [NotNull]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; } = string.Empty;

        [SQLite.MaxLength(100)]
        [NotNull]
        public string HashedPassword { get; set; } = string.Empty;
    }
}
