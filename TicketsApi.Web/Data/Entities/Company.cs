using System;
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

        [Display(Name = "Fecha Creación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Usuario Creación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string CreateUser { get; set; }

        [Display(Name = "Fecha Ultima Modificación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime LastChangeDate { get; set; }

        [Display(Name = "Usuario Ultima Modificación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string LastChangeUser { get; set; }

        [Display(Name = "Activo")]
        public bool Active { get; set; }

        [Display(Name = "Logo")]
        public string? Photo { get; set; }
        public string PhotoFullPath => string.IsNullOrEmpty(Photo)
        ? $"http://keypress.serveftp.net:90/Tickets/images/logos/noimage.png"
        : $"http://keypress.serveftp.net:90/Tickets{Photo.Substring(1)}";

        
        

        public ICollection<User>? Users { get; set; }
    }
}
