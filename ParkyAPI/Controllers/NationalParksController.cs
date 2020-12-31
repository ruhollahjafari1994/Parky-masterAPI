using System.Linq;

namespace ParkyAPI.Controllers
{
    // Dynamically Load Version Of Controller Belongs To => If Versin Is Not Define It Is V1.0
    [Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/nationalparks")] 

    //[Route("api/[controller]")]
    [Microsoft.AspNetCore.Mvc.ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecNP")]
    [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest)]
    public class NationalParksController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly ParkyAPI.Repository.IRepository.INationalParkRepository _npRepo;
        private readonly AutoMapper.IMapper _mapper;

        public NationalParksController(ParkyAPI.Repository.IRepository.INationalParkRepository npRepo, AutoMapper.IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }


        /// <summary>
        /// Get list of national parks.
        /// </summary>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(200, Type =typeof(System.Collections.Generic.List<ParkyAPI.Models.Dtos.NationalParkDto>))]
        public Microsoft.AspNetCore.Mvc.IActionResult GetNationalParks()
        {
            var objList = _npRepo.GetNationalParks();
            var objDto = new System.Collections.Generic.List<ParkyAPI.Models.Dtos.NationalParkDto>();
            foreach (var obj in objList) {
                objDto.Add(_mapper.Map<ParkyAPI.Models.Dtos.NationalParkDto>(obj));
            }
            return Ok(objDto);
        }

        /// <summary>
        /// Get individual national park
        /// </summary>
        /// <param name="nationalParkId"> The Id of the national Park </param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpGet("{nationalParkId:int}", Name = "GetNationalPark")]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(200, Type = typeof(ParkyAPI.Models.Dtos.NationalParkDto))]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(404)]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [Microsoft.AspNetCore.Mvc.ProducesDefaultResponseType]
        public Microsoft.AspNetCore.Mvc.IActionResult GetNationalPark(int nationalParkId)
        {
            var obj = _npRepo.GetNationalPark(nationalParkId);
            if (obj == null)
            {
                return NotFound();
            }
            var objDto = _mapper.Map<ParkyAPI.Models.Dtos.NationalParkDto>(obj);
            //var objDto = new NationalParkDto()
            //{
            //    Created = obj.Created,
            //    Id = obj.Id,
            //    Name = obj.Name,
            //    State = obj.State,
            //};
            return Ok(objDto);

        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(201, Type = typeof(ParkyAPI.Models.Dtos.NationalParkDto))]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound)]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status201Created)]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
        public Microsoft.AspNetCore.Mvc.IActionResult CreateNationalPark([Microsoft.AspNetCore.Mvc.FromBody] ParkyAPI.Models.Dtos.NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_npRepo.NationalParkExists(nationalParkDto.Name))
            {
                ModelState.AddModelError("", "National Park Exists!");
                return StatusCode(404, ModelState);
            }
            var nationalParkObj = _mapper.Map<ParkyAPI.Models.NationalPark>(nationalParkDto);
            if (!_npRepo.CreateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetNationalPark", new { nationalParkId= nationalParkObj.Id }, nationalParkObj);
        }

        [Microsoft.AspNetCore.Mvc.HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(204)]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound)]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
        public Microsoft.AspNetCore.Mvc.IActionResult UpdateNationalPark(int nationalParkId, [Microsoft.AspNetCore.Mvc.FromBody] ParkyAPI.Models.Dtos.NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null || nationalParkId!=nationalParkDto.Id)
            {
                return BadRequest(ModelState);
            }

            var nationalParkObj = _mapper.Map<ParkyAPI.Models.NationalPark>(nationalParkDto);
            if (!_npRepo.UpdateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }


        [Microsoft.AspNetCore.Mvc.HttpDelete("{nationalParkId:int}", Name = "DeleteNationalPark")]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status204NoContent)]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound)]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status409Conflict)]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
        public Microsoft.AspNetCore.Mvc.IActionResult DeleteNationalPark(int nationalParkId)
        {
            if (!_npRepo.NationalParkExists(nationalParkId))
            {
                return NotFound();
            }

            var nationalParkObj = _npRepo.GetNationalPark(nationalParkId);
            if (!_npRepo.DeleteNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

    }
}