﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fantasy_Web_API.Data;
using Fantasy_Web_API.Models;
using Shared_Classes.Models;
using System.Text.RegularExpressions;

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

        // Retrieves a paginated list of "items" from the database as a JSON response.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemSearchResponse<ItemDTO>>>> GetItems(int pageNumber, int pageSize, string? searchQuery, int pageId)
        {
            if (pageNumber < 0 || pageSize < 0 || pageId < 0)
            {
                return BadRequest();
            }
            if (searchQuery != null && !Regex.IsMatch(searchQuery, @"^[a-zA-Z0-9\s]+$"))
            {
                return BadRequest();
            }

            // Check Values, Set Minimums
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            IQueryable<Item> queryItem = _db.Items.AsQueryable();

            // Search Query
            if (searchQuery != null)
            {
                queryItem = queryItem.Where(item => item.Name.Contains(searchQuery));
            }

            // Pagination
            queryItem = queryItem.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            List<Item> items = await queryItem.ToListAsync();
            if (items.Count == 0)
            {
                return NotFound();
            }
            var result = new ItemSearchResponse<ItemDTO>
            {
                PageId = items.Max(item => item.Id),
                Data = items.Select(item => new ItemDTO
                {
                    Id = item.Id,
                    Name = item.Name,
                    Rarity = item.Rarity,
                    Price = item.Price,
                    Description = item.Description,
                    Image = item.Image
                }).ToList()
            };
            return Ok(result);
        }

        // Retrieves a single item by id as a JSON response.
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ItemDTO>> GetItemById(int id)
        {
            Item itemGet = await _db.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (itemGet == null)
            {
                return NotFound();
            }
            var result = new ItemDTO()
            {
                Id = itemGet.Id,
                Name = itemGet.Name,
                Rarity = itemGet.Rarity,
                Price = itemGet.Price,
                Description = itemGet.Description,
                Image = itemGet.Image
            };
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

            // Check Values, Set Defaults
            item.Name = item.Name == null ? "Untitled" : item.Name;
            item.Description = item.Description == null ? "N/A" : item.Description;
            item.Price = item.Price == null ? 0 : item.Price;
            item.Rarity = item.Rarity == null ? "Common" : item.Rarity;
            item.Image = item.Image == null ? "./images/Icon/Question_Mark.jpg" : item.Image;

            var newItem = new Item()
            {
                Name = item.Name,
                Rarity = item.Rarity,
                Price = (int)item.Price,
                Description = item.Description,
                Image = item.Image
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
            Item result = await _db.Items.FirstOrDefaultAsync(x => x.Id == id);
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
        public async Task<ActionResult<ItemDTO>> UpdateItemById(ItemDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            Item existingItem = await _db.Items.FindAsync(item.Id);
            if (existingItem == null)
            {
                return NotFound();
            }

            // Check Values, Set Defaults
            item.Name = item.Name == null ? "Untitled" : item.Name;
            item.Description = item.Description == null ? "N/A" : item.Description;
            item.Price = item.Price == null ? 0 : item.Price;
            item.Rarity = item.Rarity == null ? "Common" : item.Rarity;
            item.Image = item.Image == null ? "./images/Icon/Question_Mark.jpg" : item.Image;

            {
                existingItem.Name = item.Name;
                existingItem.Rarity = item.Rarity;
                existingItem.Price = item.Price;
                existingItem.Description = item.Description;
                existingItem.Image = item.Image;
            }
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
