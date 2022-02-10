using System;

namespace DataModel.Client.Provider.Common
{
    public class DateOfBirth
    {
        public string DateOfBirthId { get; set; }
        
        public DateTime Date { get; set; }
        public string? Age { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
