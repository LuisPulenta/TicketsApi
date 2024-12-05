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
            return await _context.TicketCabs
                .Include(x=>x.TicketState)
                .ToListAsync();
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
            oldTicketCab.Description = ticketCab.Description;
            oldTicketCab.Title = ticketCab.Title;
            oldTicketCab.Company=ticketCab.Company;
            oldTicketCab.StateDate=ahora;
            oldTicketCab.StateUser=ticketCab.StateUser;

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
                Company = ticketCab.Company,
                Description = ticketCab.Description,
                Title = ticketCab.Title,
                CreateUser = ticketCab.CreateUser,
                CreateDate = ahora,
                TicketState=TicketState.Borrador,
                StateUser=ticketCab.StateUser,
                StateDate   =ahora,
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
