using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IntakeApi.Features.Intakes
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntakesController : ControllerBase
    {
        private readonly IMediator _application;

        public IntakesController(
            IMediator application
            )
        {
            this._application = application;
        }

        [HttpGet("{id}", Name = "GetIntake")]
        public async Task<IActionResult> Get(string id)
        {
            var intake = await _application.Send(new Get.Request
            {
                Id = id
            });

            if (intake == null)
            {
                return NotFound();
            }

            return Ok(intake);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Post.Request intake)
        {
            var persistedIntake = await _application.Send(intake);
            return CreatedAtRoute(
                "GetIntake",
                new { id = persistedIntake.Id },
                persistedIntake
            );
        }
    }
}