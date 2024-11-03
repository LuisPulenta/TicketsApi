using System;
using System.Threading.Tasks;
using TicketsApi.Web.Data.Entities;
using TicketsApi.Web.Models;

namespace TicketsApi.Web.Helpers
{
    public interface IConverterHelper
    {
        Task<User> ToUserAsync(UserViewModel model, string imageId, bool isNew);

        UserViewModel ToUserViewModel(User user);
    }
}
