﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicketsApi.Common.Enums;

namespace TicketsApi.Web.Data.Entities
{
    public class TicketCab
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Fecha Creación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Usuario Creación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public User CreateUser { get; set; }

        [Display(Name = "Empresa")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public Company Company { get; set; }

        [Display(Name = "Estado")]
        public TicketState TicketState { get; set; }

        [Display(Name = "Fecha Estado")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime StateDate { get; set; }

        [Display(Name = "Usuario Estado")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string StateUserId { get; set; }

        [Display(Name = "Usuario Estado")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string StateUserName { get; set; }

        [Display(Name = "Fecha Fin")]
        public DateTime? FinishDate { get; set; }

        public ICollection<TicketDet>? TicketDets { get; set; }

        [Display(Name = "Estados")]
        public int TicketDetsNumber => TicketDets == null ? 0 : TicketDets.Count;
}
}
