namespace DentalCareSystem.Data.DTO
{
    public class Employee
    {
        public string JMBG { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Specialization { get; set; }
        public int SelectedTheme { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public override string ToString()
        {
            return $"{JMBG} {Role} {FirstName} {LastName} {PhoneNumber} {Email}";
        }
    }

}
