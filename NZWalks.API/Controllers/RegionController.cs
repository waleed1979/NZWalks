using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class RegionController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRespository regionRespository;
        private readonly IMapper mapper;
        private readonly ILogger<RegionController> logger;

        public RegionController(NZWalksDbContext nZWalksDbContext, 
            IRegionRespository regionRespository, 
            IMapper mapper,
            ILogger<RegionController> logger)
        {
            this.dbContext = nZWalksDbContext;
            this.regionRespository = regionRespository;
            this.mapper = mapper;
            this.logger = logger;
        }

        // GET ALL REGIONS
        // GET: https://localhost:portnumber/api/region
        [HttpGet]
        //[Authorize(Roles ="Reader")]
        public async Task<IActionResult> GetAll()
        {
            //logger.LogInformation("GetAll Action Method was invoked");
            
            //Get Data From Database - Domain Models
            var regionsDomain = await regionRespository.GetAllAsync();

            ////Map Domain Models to DTOs
            //var regionsDto = new List<RegionDto>();
            //foreach (var regionDomain in regionsDomain)
            //{
            //    regionsDto.Add(new RegionDto()
            //    {
            //        Id = regionDomain.Id,
            //        Code = regionDomain.Code,
            //        Name = regionDomain.Name,
            //        RegionImageUrl = regionDomain.RegionImageUrl,
            //    });
            //}

            // Map Model to DTOs

            //logger.LogInformation($"Finished GetAllRegions request with data: {JsonSerializer.Serialize(regionsDomain)}");

            var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);

           //Return DTOs
            return Ok(regionsDto);
        }

        //GET SINGLE REGION (Get Region By ID)
        //GET: https://localhost:portnumber/api/region/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById(Guid id) {
            //Get Region Domain Model From Database
            var regionDomain = await regionRespository.GetByIdAsync(id);
            //var regions = dbContext.Regions.Find(id); //This is only for primary key
            if(regionDomain == null)
            {
                return NotFound();
            }

            // Map Region Domain Model to Region DTO

            //var regionsDto = new RegionDto()
            //{
            //    Id = regionDomain.Id,
            //    Code = regionDomain.Code,
            //    Name = regionDomain.Name,
            //    RegionImageUrl = regionDomain.RegionImageUrl,
            //};

            //Map Region Domain Model to DTO
            var regionsDto = mapper.Map<RegionDto>(regionDomain);


            //Return DTOs
            return Ok(regionsDto);
        }

        //POST to Create New Region
        //POST: https://localhost:portnumber/api/regions

        [HttpPost]
        //[Authorize(Roles = "Writer")]

        public async Task<IActionResult> Create([FromBody] AddRegionRequestDTO addRegionRequestDTO)
        {
            if (ModelState.IsValid)
            {
                ////Map DTO to Domain Model
                //var regionDomainModel = new Region
                //{
                //    Code = addRegionRequestDTO.Code,
                //    Name = addRegionRequestDTO.Name,
                //    RegionImageUrl = addRegionRequestDTO.RegionImageUrl,
                //};

                //Map DTO to Domain Model
                var regionDomainModel = mapper.Map<Region>(addRegionRequestDTO);

                //Use Domain Model to create Region
                regionDomainModel = await regionRespository.CreateAsync(regionDomainModel);

                ////Map Domain model back to DTO
                //var regionDto = new RegionDto
                //{
                //    Id = regionDomainModel.Id,
                //    Code = regionDomainModel.Code,
                //    Name = regionDomainModel.Name,
                //    RegionImageUrl = regionDomainModel.RegionImageUrl,
                //};

                //Map Domain Model Back to DTO
                var regionDto = mapper.Map<RegionDto>(regionDomainModel);

                return CreatedAtAction(nameof(GetById), new { id = regionDomainModel.Id }, regionDomainModel);
            }
            else 
            {
                return BadRequest();
            }
        }

        //PUT to Update A Region
        //PUT: https://localhost:portnumber/api/regions/{id}
        [HttpPut]
        [Route("{id:guid}")]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody]UpdateRegionRequestDTO updateRegionRequestDTO )
        {
            if (ModelState.IsValid)
            {
                ////Map DTO to Domain Model
                //var regionDomainModel = new Region 
                //{
                //    Code = updateRegionRequestDTO.Code,
                //    Name = updateRegionRequestDTO.Name,
                //    RegionImageUrl= updateRegionRequestDTO.RegionImageUrl,
                //};

                //Map DTO to Domain Model
                var regionDomainModel = mapper.Map<Region>(updateRegionRequestDTO);
                //Check if region exist
                regionDomainModel = await regionRespository.UpdateAsync(id, regionDomainModel);

                if (regionDomainModel == null)
                {
                    return NotFound();
                }

                ////Convert Domain Model to DTO
                //var regionDto = new RegionDto
                //{
                //    Id = regionDomainModel.Id,
                //    Code = regionDomainModel.Code,
                //    Name = regionDomainModel.Name,
                //    RegionImageUrl = regionDomainModel.RegionImageUrl,
                //};

                var regionDto = mapper.Map<RegionDto>(regionDomainModel);
                return Ok(regionDto);
            }
            else 
            {
                return BadRequest(ModelState);
            }
        }


        //DELETE to Delete a Region
        // DELETE: https://localhost:port/api/regions/{id}

        [HttpDelete]
        [Route("{id:guid}")]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id) 
        {
            var regionDomainModel = await regionRespository.DeleteAsync(id);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

           // Map Domain Model to DTO
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return Ok(regionDto);
        }

    }
}
