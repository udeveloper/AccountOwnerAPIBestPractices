using System;
using System.Collections.Generic;
using Contracts;
using Entities.Models;
using LoggerService;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AccountOwnerServerAPI.Controllers
{
    [Route("api/owner")]    
    public class OwnerController : Controller
    {
        private ILoggerManager _loggerManager;
        private IRepositoryWrapper _repository;

        public OwnerController(ILoggerManager loggerManager,IRepositoryWrapper repositoryContext)
        {
            _loggerManager = loggerManager;
            _repository = repositoryContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Entidad o Informacion que acompaña, el codigo HTTP</response>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<Owner>),200)]
        [ProducesResponseType(500)]        
        public IActionResult GetAllOwners()
        {
            try
            {
                var owners = _repository.Owner.GetAllOwners();

                _loggerManager.LogInfo($"Returned all owners from database.");

                return Ok(owners);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Something went wrong inside GetAllOwners action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
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
            try
            {
                var owner = _repository.Owner.GetOwnerById(id);

                if (owner.Id.Equals(Guid.Empty))
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
            catch (Exception ex)
            {
                _loggerManager.LogError($"Something went wrong inside GetOwnerById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
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
            try
            {
                var owner = _repository.Owner.GetOwnerWithDetails(id);

                if (owner.Id.Equals(Guid.Empty))
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
            catch (Exception ex)
            {
                _loggerManager.LogError($"Something went wrong inside GetOwnerWithDetails action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        ///  /// <response code="200">Entidad o Informacion que acompaña, el codigo HTTP</response>
        /// <response code="400"></response>
        /// <response code="500"></response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Owner), 201]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]        
        public IActionResult CreateOwner([FromBody] Owner owner)
        {
            try
            {
                if (owner == null)
                {
                    _loggerManager.LogError("Owner object sent from client is null.");
                    return BadRequest("Owner object is null");
                }

                if (!ModelState.IsValid)
                {
                    _loggerManager.LogError("Invalid owner object sent from client.");
                    return BadRequest("Invalid model object");
                }

                _repository.Owner.CreateOwner(owner);

                return CreatedAtRoute("OwnerById", new { id = owner.Id }, owner);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
            
        }
    }
}
