using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SQLite;

namespace C_971.Models
{
    [Table("user")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("email")]
        [SQLite.MaxLength(100)]
        [NotNull]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; } = string.Empty;

        [Column("first_name")]
        [SQLite.MaxLength(50)]
        [NotNull]
        public string FirstName { get; set; } = string.Empty;

        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [Column("hashed_password")]
        [SQLite.MaxLength(100)]
        [NotNull]
        public string HashedPassword { get; set; } = string.Empty;
    }
}
