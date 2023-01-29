using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;
using System.Collections;
using System.Linq;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == id).ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var items = _context.CelestialObjects.Where(obj => obj.Name == name);

            if(!items.Any())
            {
                return NotFound();
            }

            foreach (var item in items)
            {
                item.Satellites = _context.CelestialObjects.Where(obj => obj.OrbitedObjectId == item.Id).ToList();
            }

            return Ok(items.ToList());
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var items = _context.CelestialObjects.ToList();

            foreach (var item in items)
            {
                item.Satellites = _context.CelestialObjects.Where(obj => obj.OrbitedObjectId == item.Id).ToList();
            }

            return Ok(items);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var existingObject = _context.CelestialObjects.Find(id);

            if (existingObject == null)
            {
                return NotFound();
            }

            existingObject.Name = celestialObject.Name;
            existingObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            existingObject.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(existingObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var existingObject = _context.CelestialObjects.Find(id);

            if(existingObject == null)
            {
                return NotFound();
            }

            existingObject.Name = name;
            _context.CelestialObjects.Update(existingObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingObjects = _context.CelestialObjects.Where(e => e.Id == id || e.OrbitedObjectId == id);

            if(!existingObjects.Any())
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(existingObjects);
            _context.SaveChanges();

            return NoContent();
        }
        
    }
}
