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

        // Retrieves a list of "items" from the database as a JSON response.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDTO>>> GetItems(int pageNumber, int pageSize, string? searchQuery)
        {
            // Check & Set Page Number/Size
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            // Create the Queryable 
            IQueryable<Item> queryItem = _db.Items.AsQueryable();

            // Null check and search for the name
            if (searchQuery != null)
            {
                queryItem = queryItem.Where(item => item.Name.Contains(searchQuery));
            }

            // Apply pagination
            var items = await queryItem.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            if (items.Count == 0)
            {
                return NotFound();
            }

            // Convert each item in the original list to an ItemDTO and add it to a new list.
            var result = items.Select(item => new ItemDTO
            {
                Id = item.Id,
                Name = item.Name,
                Rarity = item.Rarity,
                Price = item.Price,
                Description = item.Description,
                Image = item.Image
            }).ToList();

            return Ok(result);
        }

        // Retrieves a single item by id as a JSON response.
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ItemDTO>> GetItemById(int id)
        {
            var result = await _db.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // Creates a new database entry
        [HttpPost]
        public async Task<ActionResult<ItemDTO>> CreateItem(ItemDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var newItem = new Item()
            {
                Name = item.Name
            };
            _db.Items.Add(newItem);
            await _db.SaveChangesAsync();
            var newItemDTO = new ItemDTO()
            {
                Id = newItem.Id,
                Name = newItem.Name
            };
            return CreatedAtAction(nameof(GetItemById), new { id = newItemDTO.Id }, newItemDTO);
        }

        // Deletes a single item entry by id.
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<ItemDTO>> DeleteItemById(int id)
        {
            var result = await _db.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (result == null)
            {
                return NotFound();
            }
            _db.Items.Remove(result);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // Updates a single item entry by id.
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<ItemDTO>> UpdateItemById(int id, ItemDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var existingItem = await _db.Items.FindAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }
            existingItem.Name = item.Name;
            await _db.SaveChangesAsync();
            var updatedItem = new ItemDTO()
            {
                Id = existingItem.Id,
                Name = existingItem.Name
            };
            return Ok(updatedItem);
        }
    }
}   
