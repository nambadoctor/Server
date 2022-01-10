namespace DataModel.Client.Provider.Common
{
    public class DateOfBirth
    {
        public string DateOfBirthId { get; set; }
        public int? Day { get; set; } //1-31

        public int? Month { get; set; } //1-12

        public int? Year { get; set; } //YYYY
    }
}
