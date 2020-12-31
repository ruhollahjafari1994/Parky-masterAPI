using System.Linq;

namespace ParkyAPI.Controllers
{
    // Dynamically Load Version Of Controller Belongs To => Version 2.0
    [Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/nationalparks")]
    [Microsoft.AspNetCore.Mvc.ApiVersion("2.0")]
    [Microsoft.AspNetCore.Mvc.ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecNP")]
    [Microsoft.AspNetCore.Mvc.ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest)]
    public class NationalParksV2Controller :Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly ParkyAPI.Repository.IRepository.INationalParkRepository _npRepo;
        private readonly AutoMapper.IMapper _mapper;

        public NationalParksV2Controller(ParkyAPI.Repository.IRepository.INationalParkRepository npRepo,AutoMapper.IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }


        /// <summary>
        /// Get First Or Default of national parks.
        /// </summary>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.ProducesResponseType(200, Type =typeof(System.Collections.Generic.List<ParkyAPI.Models.Dtos.NationalParkDto>))]
        public Microsoft.AspNetCore.Mvc.IActionResult GetNationalParks()
        {
            var obj = _npRepo.GetNationalParks().FirstOrDefault();
           
            return Ok(_mapper.Map<ParkyAPI.Models.Dtos.NationalParkDto>(obj));
        }

    }
}