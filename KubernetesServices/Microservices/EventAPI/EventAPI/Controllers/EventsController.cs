using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EventAPI.Infrastructure;
using EventAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {

        private EventDbContext db;

        public EventsController(EventDbContext dbContext)
        {
            this.db = dbContext;
        }

        //GET /api/events
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [AllowAnonymous]
        public ActionResult<IEnumerable<EventInfo>> GetEvents()
        {
            return db.Events.ToList();
        }

        //GET /api/events/5
        [HttpGet("{id}", Name = "GetEventById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [AllowAnonymous]
        public ActionResult<EventInfo> GetEventById(int id)
        {
            var item = db.Events.Find(id);
            if (item != null)
                return Ok(item);
            else
                return NotFound();
        }

        //POST /api/events
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]        
        public async Task<ActionResult<EventInfo>> AddEventAsync(EventInfo model)
        {
            TryValidateModel(model);
            if(ModelState.IsValid)
            {
                var result=await db.Events.AddAsync(model);
                await db.SaveChangesAsync();
                //return Created($"/api/events/{result.Entity.Id}", result.Entity);
                //return CreatedAtAction(nameof(GetEventById), new { id = result.Entity.Id }, result.Entity);
                return CreatedAtRoute("GetEventById", new { id = result.Entity.Id }, result.Entity);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        
    }
}