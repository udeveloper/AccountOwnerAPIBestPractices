using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities;
using LoggerService;
using Microsoft.AspNetCore.Mvc;
using Repository;

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

        [HttpGet]
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

        [HttpGet("{id}")]
        public IActionResult GetOwnerById(Guid id)
        {
            try
            {
                var owner = _repository.Owner.GetOwnerById(id);

                if (owner == null)
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
    }
}
