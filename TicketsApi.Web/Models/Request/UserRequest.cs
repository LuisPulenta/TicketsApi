using System.ComponentModel.DataAnnotations;
using TicketsApi.Web.Data.Entities;

namespace TicketsApi.Web.Models.Request
{
    public class UserRequest
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Email { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string LastName { get; set; }

        [MaxLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Empresa")]
        public int CompanyId { get; set; }
    }
}
