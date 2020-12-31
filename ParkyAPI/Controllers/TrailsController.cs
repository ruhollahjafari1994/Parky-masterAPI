using System.Linq;

namespace ParkyAPI.Controllers
{
    //[Route("api/Trails")]
    [Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/trails")]
    [Microsoft.AspNetCore.Mvc.ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecTrails")]
    [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest)]
    public class TrailsController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly ParkyAPI.Repository.IRepository.ITrailRepository _trailRepo;
        private readonly AutoMapper.IMapper _mapper;

        public TrailsController(ParkyAPI.Repository.IRepository.ITrailRepository trailRepo, AutoMapper.IMapper mapper)
        {
            _trailRepo = trailRepo;
            _mapper = mapper;
        }


        /// <summary>
        /// Get list of trails.
        /// </summary>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(200, Type =typeof(System.Collections.Generic.List<ParkyAPI.Models.Dtos.TrailDto>))]
        public Microsoft.AspNetCore.Mvc.IActionResult GetTrails()
        {
            var objList = _trailRepo.GetTrails();
            var objDto = new System.Collections.Generic.List<ParkyAPI.Models.Dtos.TrailDto>();
            foreach (var obj in objList) {
                objDto.Add(_mapper.Map<ParkyAPI.Models.Dtos.TrailDto>(obj));
            }
            return Ok(objDto);
        }

        /// <summary>
        /// Get individual trail
        /// </summary>
        /// <param name="trailId"> The id of the trail </param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpGet("{trailId:int}", Name = "GetTrail")]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(200, Type = typeof(ParkyAPI.Models.Dtos.TrailDto))]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(404)]
        [Microsoft.AspNetCore.Mvc.ProducesDefaultResponseType]
         [Microsoft.AspNetCore.Authorization.Authorize(Roles ="Admin")]
        public Microsoft.AspNetCore.Mvc.IActionResult GetTrail(int trailId)
        {
            var obj = _trailRepo.GetTrail(trailId);
            if (obj == null)
            {
                return NotFound();
            }
            var objDto = _mapper.Map<ParkyAPI.Models.Dtos.TrailDto>(obj);
            
            return Ok(objDto);

        }

        [Microsoft.AspNetCore.Mvc.HttpGet("[action]/{nationalParkId:int}")]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(200, Type = typeof(ParkyAPI.Models.Dtos.TrailDto))]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(404)]
        [Microsoft.AspNetCore.Mvc.ProducesDefaultResponseType]
        public Microsoft.AspNetCore.Mvc.IActionResult GetTrailInNationalPark(int nationalParkId)
        {
            var objList = _trailRepo.GetTrailsInNationalPark(nationalParkId);
            if (objList == null)
            {
                return NotFound();
            }
            var objDto = new System.Collections.Generic.List<ParkyAPI.Models.Dtos.TrailDto>();
            foreach(var obj in objList)
            {
                 objDto.Add(_mapper.Map<ParkyAPI.Models.Dtos.TrailDto>(obj));
            }
            

            return Ok(objDto);

        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(201, Type = typeof(ParkyAPI.Models.Dtos.TrailDto))]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status201Created)]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound)]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
        public Microsoft.AspNetCore.Mvc.IActionResult CreateTrail([Microsoft.AspNetCore.Mvc.FromBody] ParkyAPI.Models.Dtos.TrailCreateDto trailDto)
        {
            if (trailDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_trailRepo.TrailExists(trailDto.Name))
            {
                ModelState.AddModelError("", "Trail Exists!");
                return StatusCode(404, ModelState);
            }
            var trailObj = _mapper.Map<ParkyAPI.Models.Trail>(trailDto);
            if (!_trailRepo.CreateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetTrail", new { trailId= trailObj.Id }, trailObj);
        }

        [Microsoft.AspNetCore.Mvc.HttpPatch("{trailId:int}", Name = "UpdateTrail")]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(204)]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound)]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
        public Microsoft.AspNetCore.Mvc.IActionResult UpdateTrail(int trailId, [Microsoft.AspNetCore.Mvc.FromBody] ParkyAPI.Models.Dtos.TrailUpdateDto trailDto)
        {
            if (trailDto == null || trailId!=trailDto.Id)
            {
                return BadRequest(ModelState);
            }

            var trailObj = _mapper.Map<ParkyAPI.Models.Trail>(trailDto);
            if (!_trailRepo.UpdateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }


        [Microsoft.AspNetCore.Mvc.HttpDelete("{trailId:int}", Name = "DeleteTrail")]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status204NoContent)]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound)]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status409Conflict)]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
        public Microsoft.AspNetCore.Mvc.IActionResult DeleteTrail(int trailId)
        {
            if (!_trailRepo.TrailExists(trailId))
            {
                return NotFound();
            }

            var trailObj = _trailRepo.GetTrail(trailId);
            if (!_trailRepo.DeleteTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

    }
}