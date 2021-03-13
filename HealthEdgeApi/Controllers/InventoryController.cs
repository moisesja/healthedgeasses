using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HealthEdgeApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HealthEdgeApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, InventoryItem> _inventory;

        private readonly ILogger<InventoryController> _logger;

        static InventoryController()
        {
            _inventory = new ConcurrentDictionary<string, InventoryItem>();

            _inventory.GetOrAdd("apples", new InventoryItem()
                {
                    Name = "Apples",
                    Quantity = 3,
                    CreatedOn = DateTime.Parse("2020-01-01")
                });

            _inventory.GetOrAdd("oranges", new InventoryItem()
                {
                    Name = "Oranges",
                    Quantity = 7,
                    CreatedOn = DateTime.Parse("2020-02-01")
                });

            _inventory.GetOrAdd("pomegranates", new InventoryItem()
            {
                Name = "Pomegranates",
                Quantity = 55,
                CreatedOn = DateTime.Parse("2020-02-10")
            });
        }

        public InventoryController(ILogger<InventoryController> logger)
        {
            _logger = logger;
        }

        #region Pure RESTful

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<InventoryItem>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get()
        {
            try
            {
                return Ok(_inventory.Values.ToList());
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Error while getting inventory.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<InventoryItem>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest();
            }

            try
            {
                var key = name.ToLower();

                if (!_inventory.ContainsKey(key))
                {
                    return NotFound();
                }

                return Ok(_inventory[key]);

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Error while inventory item by name.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(InventoryItem))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Post([FromBody] InventoryItem item)
        {
            try
            {
                var key = item.Name.ToLower();

                if (_inventory.ContainsKey(key))
                {
                    return Conflict();
                }

                _inventory.GetOrAdd(key, item);

                return Created($"/api/inventory/{key}", item);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Error while posting inventory item.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// This really acts as an Upsert operation
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(InventoryItem))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Put([FromBody] InventoryItem item)
        {
            try
            {
                var key = item.Name.ToLower();

                if (_inventory.ContainsKey(key))
                {
                    // Update
                    _inventory[key] = item;
                }
                else
                {
                    _inventory.GetOrAdd(key, item);
                }

                return Accepted($"/api/inventory/{item.Name}", item);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Error while putting inventory item.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{name}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest();
            }

            try
            {
                var key = name.ToLower();

                if (!_inventory.ContainsKey(key))
                {
                    return NotFound();
                }

                _inventory.Remove(key, out InventoryItem item);

                return NoContent();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Error while deleting inventory item {name}.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        #endregion

        #region This is not REST

        /// <summary>
        /// Highest quantity (user queries the inventory for the Highest quantity item)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("~/GetHighestQuantity")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<InventoryItem>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetHighestQuantity()
        {
            try
            {
                // Get the max value first
                // We do this because it is possible to have several items with the
                //  same quantity
                var quantity = _inventory.Values.Max(_ => _.Quantity);

                return Ok(_inventory.Values.Where(_ => _.Quantity == quantity)
                    .ToList());
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Error while getting highest quantity item.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /*
         * - Lowest quantity (user queries the inventory for the Highest quantity item)- Oldest item- Newest item- Any additional functionality you might think will be useful to the user.
         */

        #endregion
    }
}
