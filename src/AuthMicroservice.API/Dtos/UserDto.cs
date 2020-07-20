namespace AuthMicroservice.API.Dtos
{
    public class UserDto
    {
        public int UserID {get; set; }
        public string FirstName {get; set; }
        public string LastName {get; set; }
        public string UserName {get; set; }
        public string Email {get; set; }
        public string Password {get; set; } 
    }
}