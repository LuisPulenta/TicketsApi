namespace TicketsApi.Web.Models.Request
{
    public class CompanyRequest
    {
        public string Name { get; set; }
        public string CreateUser { get; set; }
        public string LastChangeUser { get; set; }
        public bool Active { get; set; }
        public byte[] ImageArray { get; set; }
    }
}
