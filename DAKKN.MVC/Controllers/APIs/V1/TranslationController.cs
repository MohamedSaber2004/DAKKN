using Asp.Versioning;
using DAKKN.Appearence.Routes;
using DAKKN.Application.Localization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DAKKN.Appearence.Controllers.APIs.V1
{
    [ApiVersion("1.0")]
    public class TranslationController: BaseApiController
    {

        public TranslationController(IMediator mediator): base(mediator)
        {
            
        }

        [HttpGet]
        [Route(ApiRoutes.Translation.Get)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetTranslations(string lang)
        {
            var translations = JsonLocalizationProvider.GetTranslations(lang);
            return Json(translations);
        }
    }
}
