using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TicketsApi.Web.Data.Entities
{
    public class Company
    {
        public int Id { get; set; }

        [Display(Name = "Empresa")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Name { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
