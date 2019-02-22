using Contracts;
using Entities.Extensions;
using Entities.Models;
using LoggerService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using AccountOwnerServerAPI.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AccountOwnerServerAPI.Controllers
{
    [Route("api/owner")]
    public class OwnerController : Controller
    {
        private readonly ILoggerManager _loggerManager;
        private readonly IRepositoryWrapper _repository;
        private readonly IDistributedCache _cache;

        public OwnerController(ILoggerManager loggerManager, IRepositoryWrapper repositoryContext, IDistributedCache cache)
        {
            _loggerManager = loggerManager;
            _repository = repositoryContext;
            _cache = cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Entidad o Informacion que acompaña, el codigo HTTP</response>
        [HttpGet]
        //[ResponseCache(Duration =60)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<Owner>), 200)]
        [ProducesResponseType(500)]
        public IActionResult GetAllOwners()
        {
            string cacheKey = $"clinical_owner_list";
           
            IEnumerable<Owner> owners = null;
            
            // Get the cached item
            owners = owners.GetCache<IEnumerable<Owner>>(cacheKey,_cache);

            // If there was a cached item then deserialise this into our contact object
            if (owners==null)
            {               
                owners = _repository.Owner.GetAllOwners();

                owners.SetCache<IEnumerable<Owner>>(cacheKey, _cache);

                _loggerManager.LogInfo($"Returned all owners from database.");
            }

                      
            return Ok(owners);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Entidad o Informacion que acompaña, el codigo HTTP</response>
        /// <response code="400"></response>
        /// <response code="500"></response>
        [HttpGet("{id}", Name = "OwnerById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Owner), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetOwnerById(Guid id)
        {

            var owner = _repository.Owner.GetOwnerById(id);

            if (owner.IsEmptyObject())
            {
                _loggerManager.LogError($"Owner with id: {id}, hasn't been found in db.");
                return NotFound();
            }
            else
            {
                _loggerManager.LogInfo($"Returned owner with id: {id}");
                return Ok(owner);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200"></response>
        /// <response code="400"></response>
        /// <response code="500"></response>
        [HttpGet("{id}/account")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Owner), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetOwnerWithDetails(Guid id)
        {

            var owner = _repository.Owner.GetOwnerWithDetails(id);

            if (owner.IsEmptyObject())
            {
                _loggerManager.LogError($"Owner with id: {id}, hasn't been found in db.");
                return NotFound();
            }
            else
            {
                _loggerManager.LogInfo($"Returned owner with details for id: {id}");
                return Ok(owner);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        /// <response code="201">Entidad o Informacion que acompaña, el codigo HTTP</response>
        /// <response code="400"></response>
        /// <response code="500"></response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Owner), 201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateOwner([FromBody] Owner owner)
        {

            if (owner.IsObjectNull())
            {
                _loggerManager.LogError("Owner object sent from client is null.");
                return BadRequest("Owner object is null");
            }

            if (!ModelState.IsValid)
            {
                _loggerManager.LogError("Invalid owner object sent from client.");
                return BadRequest(ModelState);
            }

            _repository.Owner.CreateOwner(owner);

            return CreatedAtRoute("OwnerById", new { id = owner.Id }, owner);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        /// <response code="204">Entidad o Informacion que acompaña, el codigo HTTP</response>
        /// <response code="400"></response>
        /// <response code="404"></response>
        /// <response code="500"></response>
        [HttpPut("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Owner), 204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateOwner(Guid id, [FromBody] Owner owner)
        {

            if (owner.IsObjectNull())
            {
                _loggerManager.LogError("Owner object sent from client is null.");
                return BadRequest("Owner object is null");
            }
            if (!ModelState.IsValid)
            {
                _loggerManager.LogError("Invalid owner object sent from client.");
                return BadRequest("Invalid model object");
            }

            var dbOwner = _repository.Owner.GetOwnerById(id);

            if (dbOwner.IsEmptyObject())
            {
                _loggerManager.LogError($"Owner with id: {id}, hasn't been found in db.");
                return NotFound();
            }

            _repository.Owner.UpdateOwner(dbOwner, owner);

            return NoContent();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="204">Entidad o Informacion que acompaña, el codigo HTTP</response>        
        /// <response code="400"></response>
        /// <response code="404"></response>
        /// <response code="500"></response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Owner), 204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteOwner(Guid id)
        {

            var owner = _repository.Owner.GetOwnerById(id);

            if (owner.IsEmptyObject())
            {
                _loggerManager.LogError($"Owner with id: {id}, hasn't been found in db.");
                return NotFound();
            }

            if (_repository.Account.AccountsByOwner(id).Any())
            {
                _loggerManager.LogError($"Cannot delete owner with id: {id}. It has related accounts. Delete those accounts first");
                return BadRequest("Cannot delete owner. It has related accounts. Delete those accounts first");
            }

            _repository.Owner.DeleteOwner(owner);

            return NoContent();

        }
    }
}
