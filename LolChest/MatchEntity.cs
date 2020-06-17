using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace LolChest
{
    /// <summary>
    /// This represents a single table row of the table with
    /// the name 'registeredmatches'.
    /// </summary>
    public class MatchEntity : TableEntity
    {
        public DateTime GameCreation { get; set; }
        public string MatchJson { get; set; }
    }
}