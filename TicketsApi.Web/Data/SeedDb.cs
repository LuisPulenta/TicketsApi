using System.Threading.Tasks;
using TicketsApi.Web.Data.Entities;
using TicketsApi.Web.Helpers;
using TicketsApi.Common.Enums;

namespace TicketsApi.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckRolesAsycn();
            await CheckUserAsync("Luis", "Núñez", "luis@yopmail.com", "351 681 4963",UserType.Admin);
            await CheckUserAsync("Pablo", "Lacuadri", "pablo@yopmail.com", "351 681 4963", UserType.Admin);
            await CheckUserAsync("Lionel", "Messi", "messi@yopmail.com", "311 322 4620",UserType.User);
            await CheckUserAsync("Diego", "Maradona", "maradona@yopmail.com", "311 322 4620",UserType.User);

        }

        private async Task CheckRolesAsycn()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }
                
     


        private async Task CheckUserAsync(string firstName, string lastName, string email, string phoneNumber, UserType userType)
        {
            User user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phoneNumber,
                    UserName = email,
                    UserType = userType
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);

            }
        }
    }
}
