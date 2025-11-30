using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace C_971.Models
{
    public abstract class BaseEntity : ObservableObject
    {
        [PrimaryKey, AutoIncrement]

        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
