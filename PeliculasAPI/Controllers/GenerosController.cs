using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Utilidades;
using System.Threading.Tasks;

namespace PeliculasAPI.Controllers
{
    [Route("api/generos")]
    [ApiController] //reglas de validacion
    public class GenerosController : ControllerBase
    {

        private readonly IOutputCacheStore outputCacheStore;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private const string cacheTag = "generos";
        public GenerosController(IOutputCacheStore outputCacheStore,ApplicationDbContext context, IMapper mapper) 
        {   
            this.outputCacheStore = outputCacheStore;
            this.context = context;
            this.mapper = mapper;
        }    

        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<List<GeneroDTO>> Get([FromQuery] PaginacionDTO paginacion)
        {
            var queryable = context.Generos;
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable
                .OrderBy(g => g.Nombre)
                .Paginar(paginacion)
                .ProjectTo<GeneroDTO>(mapper.ConfigurationProvider).ToListAsync(); //ProjectTo -> No realiza consulta de datos que no necesita
        }

        [HttpGet("{id:int}", Name = "ObtenerGeneroPorId")] //Name -> Sirve para que otros métodos puedan referenciarla sin escribir manualmente la URL
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<GeneroDTO>> Get(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            var genero = mapper.Map<Genero>(generoCreacionDTO);

            context.Add(genero); // Marca la entidad para inserción
            await context.SaveChangesAsync(); // Ejecuta el INSERT y genera el Id

            // Devuelve 201 y la URL del recurso creado
            return CreatedAtRoute("ObtenerGeneroPorId", new {id = genero.Id}, genero);
        }

        [HttpPut]
        public void Put()
        {
            throw new NotImplementedException();
        }

        [HttpDelete] 
        public void Delete()
        {
            throw new NotImplementedException();
        }

    }
}
