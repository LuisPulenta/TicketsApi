using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TicketsApi.Web.Data;
using TicketsApi.Web.Data.Entities;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using TicketsApi.Common.Helpers;
using TicketsApi.Web.Models.Request;
using Org.BouncyCastle.Asn1.Ocsp;

namespace TicketsApi.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CompaniesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IFilesHelper _filesHelper;

        public CompaniesController(DataContext context, IFilesHelper filesHelper)
        {
            _context = context;
            _filesHelper = filesHelper;
        }

        //-----------------------------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        //-----------------------------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            Company company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return company;
        }

        //-----------------------------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(int id, CompanyRequest companyRequest)
        {
            if (id != companyRequest.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Company oldCompany = await _context.Companies.FirstOrDefaultAsync(o => o.Id == companyRequest.Id);

            //Foto
            string imageUrl = string.Empty;
            if (companyRequest.ImageArray != null && companyRequest.ImageArray.Length > 0)
            {
                imageUrl = string.Empty;
                var stream = new MemoryStream(companyRequest.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot\\images\\Logos";
                var fullPath = $"~/images/Logos/{file}";
                var response = _filesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    imageUrl = fullPath;
                    oldCompany!.Photo = imageUrl;
                }
            }

            DateTime ahora = DateTime.Now;

            oldCompany!.Active = companyRequest.Active;
            oldCompany.LastChangeUser = companyRequest.LastChangeUser;
            oldCompany!.LastChangeDate = ahora;
            oldCompany!.Name = companyRequest.Name;

            _context.Update(oldCompany);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicada"))
                {
                    return BadRequest("Ya existe una empresa con el mismo nombre.");
                }
                else
                {
                    return BadRequest(dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }

            return Ok(oldCompany);
        }
        
        //-----------------------------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany(CompanyRequest companyRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DateTime ahora = DateTime.Now;

            Company newCompany = new Company
            {
                Id = 0,
                Name = companyRequest.Name,
                Active = true,
                CreateUser = companyRequest.CreateUser,
                CreateDate = ahora,
                LastChangeUser = companyRequest.LastChangeUser,
                LastChangeDate = ahora,
                Photo = null
            };


            //Foto


            if (companyRequest.ImageArray != null) {
                var stream = new MemoryStream(companyRequest.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot\\images\\Logos";
                var fullPath = $"~/images/Logos/{file}";
                var response = _filesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    newCompany.Photo = fullPath;
                }
            }
            _context.Companies.Add(newCompany);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(newCompany);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                {
                    return BadRequest("Ya existe esta Empresa.");
                }
                else
                {
                    return BadRequest(dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
        //-----------------------------------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            Company company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        //-----------------------------------------------------------------------------------
        [HttpGet("combo")]
        public async Task<ActionResult> GetCombo()
        {
            return Ok(await _context.Companies
                  .OrderBy(c => c.Name)
                  .Where(c => c.Active && c.Id != 1)
                  .ToListAsync());
        }
    }
}
