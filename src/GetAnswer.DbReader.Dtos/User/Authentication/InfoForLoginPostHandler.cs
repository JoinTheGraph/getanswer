using System;

namespace GetAnswer.DbReader.Dtos.User.Authentication
{
    public class InfoForLoginPostHandler
    {
        public string UserId { get; set; }
        public string HashedPassword { get; set; }
        public string FirstName { get; set; }
        public DateTime AuthTicketInfoLastChangeUtcTime { get; set; }
    }
}