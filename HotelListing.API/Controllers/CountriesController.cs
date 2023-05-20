using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;
using AutoMapper;
using HotelListing.API.Contracts;

namespace HotelListing.API.Controllers;

//RESUME AT PUT

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    
    private readonly IMapper _mapper;
    private readonly ICountriesRepository _countriesRepository;

    //Controller Boiler Plate INJECTS INTERFACE OBJECT into controller seen in below constructor
    //In program.cs, you can see on line 10 we have registered the INTERFACES as part of our services.This gives us the ability to inject it in almost any file.
    //This saves us from instantiating new objects 
    public CountriesController(IMapper mapper, ICountriesRepository countriesRepository)
    {
        this._mapper = mapper;
        this._countriesRepository = countriesRepository;
    }

    // GET: api/Countries
    [HttpGet]
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
        var country = await _countriesRepository.GetAsync(id);

        if (country == null)
        {
            return NotFound();
        }

        var countryDto = _mapper.Map<GetCountryDto>(country);

        return Ok(countryDto);
    }

    // PUT: api/Countries/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
    {
        if (id != updateCountryDto.Id)
        {
            return BadRequest("Invalid Record Id");
        }

        ///_context.Entry(country).State = EntityState.Modified;
        ///
        var country = await _countriesRepository.GetAsync(id);
        if(country == null)
        {
            return NotFound();
        }

        ///Take details of first argument (updateCountryDto) and transfer them to second argument
        _mapper.Map(updateCountryDto, country);
        
        //RESUME HERE
        try
        {   
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CountryExists(id))
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
    public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountry)
    {
       
        //Maps createCountry to Country 
        var country = _mapper.Map<Country>(createCountry);

        _context.Countries.Add(country);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCountry", new { id = country.Id }, country);
    }

    // DELETE: api/Countries/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        var country = await _context.Countries.FindAsync(id);
        if (country == null)
        {
            return NotFound();
        }

        _context.Countries.Remove(country);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CountryExists(int id)
    {
        return _context.Countries.Any(e => e.Id == id);
    }
}
