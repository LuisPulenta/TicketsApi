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

namespace TicketsApi.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TicketCabsController : ControllerBase
    {
        private readonly DataContext _context;

        public TicketCabsController(DataContext context)
        {
            _context = context;
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
                CompanyId = ticketCab.CompanyId,
                CompanyName = ticketCab.CompanyName,
                UserId = ticketCab.UserId,
                UserName = ticketCab.UserName,
                CreateDate = ahora,
                TicketState=TicketState.Enviado,
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
