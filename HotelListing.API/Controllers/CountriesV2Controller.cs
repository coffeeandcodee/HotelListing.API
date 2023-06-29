using AutoMapper;
using HotelListing.API.Contracts;
using HotelListing.API.Data;
using HotelListing.API.Exceptions;
using HotelListing.API.Models.Country;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Controllers;

//RESUME AT PUT

[Route("api/v{version:apiVersion}/countries")]
[ApiController]
[ApiVersion("2.0")]
public class CountriesV2Controller : ControllerBase
{

    private readonly IMapper _mapper;
    private readonly ICountriesRepository _countriesRepository;
    private readonly ILogger<CountriesV2Controller> _logger;

    //Controller Boiler Plate INJECTS INTERFACE OBJECT into controller seen in below constructor
    //In program.cs, you can see on line 10 we have registered the INTERFACES as part of our services.This gives us the ability to inject it in almost any file.
    //This saves us from instantiating new objects 
    public CountriesV2Controller(IMapper mapper, ICountriesRepository countriesRepository, ILogger<CountriesV2Controller> logger)
    {
        this._mapper = mapper;
        this._countriesRepository = countriesRepository;
        this._logger = logger;
    }

    // GET: api/Countries
    [HttpGet]
    [EnableQuery]
    public async Task<ActionResult<IEnumerable<GetCountriesDto>>> GetCountries()
    {
        //The below is a query.
        //Equivalent to Select * from Countries
        var countries = await _countriesRepository.GetAllAsync();

        var records = _mapper.Map<List<GetCountriesDto>>(countries);
        return Ok(records);
    }

    // GET: api/Countries/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetCountryDto>> GetCountry(int id)
    {

        /* Before implementing repo:
         * var country = await _context.Countries.Include(q => Hotels)
         *      .FirstOrDefaultAsync(q => q.Id == id);
         */

        var country = await _countriesRepository.GetAsync(id);

        if (country == null)
        {
            throw new NotFoundException(nameof(GetCountries), id);
        }

        var countryDto = _mapper.Map<GetCountryDto>(country);

        return Ok(countryDto);
    }

    // PUT: api/Countries/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
    {
        if (id != updateCountryDto.Id)
        {
            return BadRequest("Invalid Record Id");
        }

        ///_context.Entry(country).State = EntityState.Modified;
        ///
        var country = await _countriesRepository.GetAsync(id);
        if (country == null)
        {
            throw new NotFoundException(nameof(GetCountries), id);
        }

        ///Take details of first argument (updateCountryDto) and transfer them to second argument
        _mapper.Map(updateCountryDto, country);

        //RESUME HERE
        try
        {
            await _countriesRepository.UpdateAsync(country);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await CountryExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Countries
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountry)
    {

        //Maps createCountry to Country 
        var country = _mapper.Map<Country>(createCountry);

        await _countriesRepository.AddAsync(country);


        return CreatedAtAction("GetCountry", new { id = country.Id }, country);
    }

    // DELETE: api/Countries/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        var country = await _countriesRepository.GetAsync(id);
        if (country == null)
        {
            throw new NotFoundException(nameof(GetCountries), id);
        }

        await _countriesRepository.DeleteAsync(id);
        return NoContent();

    }

    private async Task<bool> CountryExists(int id)
    {
        return await _countriesRepository.Exists(id);
    }
}
