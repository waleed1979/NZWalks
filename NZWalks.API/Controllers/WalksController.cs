using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // /api/walks
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }
        //Create Walk
        // POST: /api/walks
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDTO addWalkRequestDTO)
        {
            
                //Map DTO to Domain Model
                var walkDomainModel = mapper.Map<Walk>(addWalkRequestDTO);

                await walkRepository.CreateAsync(walkDomainModel);
                //Mao Domain Model to DTO
                var walkDto = mapper.Map<WalkDto>(walkDomainModel);

                return Ok(walkDto);
            
        }

        //Get Walks
        // GET: /api/walks?filterOn=Name&filterQuery=Track&sortBy=Name&isAscending=true&pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var walksDomainModel = await walkRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true, pageNumber, pageSize);

            // Map Domain Model to DTO

            var walkDto = mapper.Map<List<WalkDto>>(walksDomainModel);
            return Ok(walkDto);
        }



        //Get Walk by ID
        // GET: /api/walks/id
        [HttpGet]
        [Route("{id:guid}")]

        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walksDomianModel = await walkRepository.GetByIdAsync(id);

            if (walksDomianModel == null) 
            {
                return NotFound();
            } 

            //Map Domain Model to DTO
            
            var walkDto = mapper.Map<WalkDto>(walksDomianModel);
            return Ok(walkDto);
        }

        //Update Walk By ID
        //PUT: /api/walks/id
        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, UpdateWalkRequestDto updateWalkRequestDto) 
        {
            
                //Map DTO to domain Model
                var walkDominModel = mapper.Map<Walk>(updateWalkRequestDto);

                await walkRepository.UpdateAsync(id, walkDominModel);
                if (walkDominModel == null)
                {
                    return NotFound();
                }
                //Map Domain Model to DTO
                return Ok(mapper.Map<WalkDto>(walkDominModel));
            }
           
        

        //Delete Walk By ID
        //DELETE: /api/walks/id
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id) 
        {
            var deletedWalkDomainModel = await walkRepository.DeleteAsync(id);
            if (deletedWalkDomainModel == null) 
            {
                return NotFound();
            }

            //Map DomainModel to DTO
            return Ok(mapper.Map<WalkDto>(deletedWalkDomainModel));
        }
    }
}

