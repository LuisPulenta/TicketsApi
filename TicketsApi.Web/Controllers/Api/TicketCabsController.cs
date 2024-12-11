using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TicketsApi.Web.Data;
using TicketsApi.Web.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using TicketsApi.Common.Enums;
using System.Linq;
using TicketsApi.Web.Models;
using TicketsApi.Web.Models.Request;
using System.IO;
using TicketsApi.Common.Helpers;

namespace TicketsApi.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TicketCabsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IFilesHelper _filesHelper;

        public TicketCabsController(DataContext context, IFilesHelper filesHelper)
        {
            _context = context;
            _filesHelper = filesHelper;
        }

        //-----------------------------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketCab>>> GetTicketCabs()
        {
            List<TicketCab> ticketCabs = await _context.TicketCabs
              .OrderBy(x => x.CompanyName)
              .OrderBy(x => x.Id)
              .ToListAsync();

            List<TicketCabViewModel> list = new List<TicketCabViewModel>();

            

            foreach (TicketCab ticketCab in ticketCabs)
            {
                User createUser = await _context.Users
                .FirstOrDefaultAsync(p => p.Id == ticketCab.UserId);

                TicketCabViewModel ticketCabViewModel = new TicketCabViewModel
                {
                    Id = ticketCab.Id,
                    CreateDate = ticketCab.CreateDate,
                    CreateUserId = createUser.Id,
                    CreateUserName = createUser.FullName,
                    CompanyId = ticketCab.CompanyId,
                    CompanyName = ticketCab.CompanyName,
                    Title = ticketCab.Title,
                    TicketState = ticketCab.TicketState,
                    AsignDate = ticketCab.AsignDate,
                    InProgressDate = ticketCab.InProgressDate,
                    FinishDate = ticketCab.FinishDate,
                    TicketDets = ticketCab.TicketDets
                };
                list.Add(ticketCabViewModel);
            }
            return Ok(list);



        }

        //-----------------------------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketCab>> GetTicketCab(int id)
        {
            TicketCab ticketCab = await _context.TicketCabs.FindAsync(id);

            if (ticketCab == null)
            {
                return NotFound();
            }

            return ticketCab;
        }

        //-----------------------------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTicketCab(int id, TicketCab ticketCab)
        {
            if (id != ticketCab.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TicketCab oldTicketCab = await _context.TicketCabs.FirstOrDefaultAsync(o => o.Id == ticketCab.Id);

            DateTime ahora = DateTime.Now;

            oldTicketCab.TicketState = ticketCab.TicketState;
            oldTicketCab.CompanyId=ticketCab.CompanyId;
            oldTicketCab.CompanyName = ticketCab.CompanyName;

            _context.Update(oldTicketCab);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbUpdateException)
            {
               return BadRequest(dbUpdateException.InnerException.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }

            return Ok(oldTicketCab);
        }
        
        //-----------------------------------------------------------------------------------
        [HttpPost]
        [Route("PostTicketCab")]
        public async Task<ActionResult<TicketCab>> PostTicketCab(TicketCab ticketCab)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DateTime ahora = DateTime.Now;

            TicketCab newTicketCab = new TicketCab
            {
                Id = 0,
                CreateDate = ahora,
                UserId = ticketCab.UserId,
                UserName = ticketCab.UserName,
                CompanyId = ticketCab.CompanyId,
                CompanyName = ticketCab.CompanyName,
                Title=ticketCab.Title,
                TicketState=TicketState.Enviado,
                AsignDate=null,
                InProgressDate=null,
                FinishDate=null,                
            };
            
            _context.TicketCabs.Add(newTicketCab);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(newTicketCab);
            }
            catch (DbUpdateException dbUpdateException)
            {
               return BadRequest(dbUpdateException.InnerException.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        //-----------------------------------------------------------------------------------
        [HttpPost]
        [Route("PostTicketDet")]
        public async Task<ActionResult<TicketCab>> PostTicketDet(TicketDetRequest ticketDetRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DateTime ahora = DateTime.Now;
            TicketCab ticketCab = await _context.TicketCabs.FirstOrDefaultAsync(o => o.Id == ticketDetRequest.TicketCabId);

            TicketState ticketState = TicketState.Enviado;

            if (ticketDetRequest.TicketState == 1)
            {
                ticketState = TicketState.Devuelto;
            }
            if (ticketDetRequest.TicketState == 2)
            {
                ticketState = TicketState.Asignado;
            }
            if (ticketDetRequest.TicketState == 3)
            {
                ticketState = TicketState.Encurso;
            }
            if (ticketDetRequest.TicketState == 4)
            {
                ticketState = TicketState.Resuelto;
            }


            TicketDet newTicketDet = new TicketDet
            {
                Id = 0,
                TicketCab = ticketCab,
                Description= ticketDetRequest.Description,
                StateDate = ahora,
                TicketState = ticketState,                
                StateUserId= ticketDetRequest.StateUserId,
                StateUserName= ticketDetRequest.StateUserName,
            };

            //Foto
            if (ticketDetRequest.ImageArray != null)
            {
                var stream = new MemoryStream(ticketDetRequest.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot\\images\\Tickets";
                var fullPath = $"~/images/Tickets/{file}";
                var response = _filesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    newTicketDet.Image = fullPath;
                }
            }

            _context.TicketDets.Add(newTicketDet);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(newTicketDet);
            }
            
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        //-----------------------------------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicketCab(int id)
        {
            TicketCab ticketCab = await _context.TicketCabs.FindAsync(id);
            if (ticketCab == null)
            {
                return NotFound();
            }

            _context.TicketCabs.Remove(ticketCab);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
