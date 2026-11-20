using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using PeliculasAPI.Entidades;
using System.Threading.Tasks;

namespace PeliculasAPI.Controllers
{
    [Route("api/generos")]
    [ApiController] //reglas de validacion
    public class GenerosController : ControllerBase
    {

        private readonly IOutputCacheStore outputCacheStore;
        private readonly IConfiguration configuration;
        private const string cacheTag = "generos";
        public GenerosController(
            IOutputCacheStore outputCacheStore,
            IConfiguration configuration) 
        {
           
            this.outputCacheStore = outputCacheStore;
           
        }
        

        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public List<Genero> Get()
        {
            return new List<Genero>() { new Genero { Id = 1, Nombre = "Comedia" }, new Genero { Id = 2, Nombre = "Accion" } };
        }

        [HttpGet("{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<Genero>> Get(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Genero genero)
        {
            throw new NotImplementedException();
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
