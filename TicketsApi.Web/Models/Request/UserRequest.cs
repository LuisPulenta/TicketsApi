using TicketsApi.Common.Enums;

namespace TicketsApi.Web.Models.Request
{
    public class UserRequest
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int IdCompany { get; set; }
        public string Company { get; set; }
        public int IdUserType { get; set; }
        public string UserType { get; set; }
        public string CreateUser { get; set; }
        public string LastChangeUser { get; set; }
        public bool Active { get; set; }
    }
}