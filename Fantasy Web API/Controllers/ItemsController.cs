using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fantasy_Web_API.Data;
using Fantasy_Web_API.Models;

namespace Fantasy_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ItemsController(ApplicationDbContext context)
        {
            _db = context;
        }

        // Retrieves a list of all "items" from the database as a JSON response.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetAllItems()
        {
            if (_db.Items == null)
            {
                return NotFound();
            }
            return await _db.Items.ToListAsync();
        }
    }
}
