using System.Threading.Tasks;
using TicketsApi.Web.Data.Entities;
using TicketsApi.Web.Helpers;
using TicketsApi.Common.Enums;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
            await CheckCompaniesAsync();
            await CheckRolesAsycn();
            await CheckUserAsync("Luis", "Núñez", "luis@yopmail.com", "351 681 4963",UserType.Admin,1);
            await CheckUserAsync("Pablo", "Lacuadri", "pablo@yopmail.com", "351 681 4963", UserType.Admin,1);
            await CheckUserAsync("Lionel", "Messi", "messi@yopmail.com", "311 322 4620",UserType.User,2);
            await CheckUserAsync("Diego", "Maradona", "maradona@yopmail.com", "311 322 4620",UserType.User,2);

        }

        //--------------------------------------------------------------------------------------------
        private async Task CheckRolesAsycn()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        //--------------------------------------------------------------------------------------------
        private async Task CheckUserAsync(string firstName, string lastName, string email, string phoneNumber, UserType userType,int companyId)
        {
            Company company = await _context.Companies
                .FirstOrDefaultAsync(x => x.Id == companyId);

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
                    UserType = userType,
                    CompanyId =company.Id
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }
        }

        //--------------------------------------------------------------------------------------------
        private async Task CheckCompaniesAsync()
        {
            if (!_context.Companies.Any())
            {
                _context.Companies.Add(new Company { Name = "Keypress" });
                _context.Companies.Add(new Company { Name = "Prueba" });
                await _context.SaveChangesAsync();
            }
        }
    }
}
