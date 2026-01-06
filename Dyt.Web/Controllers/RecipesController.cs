using Dyt.Business.Interfaces.Recipes;
using Dyt.Contracts.Recipes.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Dyt.Web.Controllers
{
    public class RecipesController : Controller
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? tag, string? difficulty, CancellationToken ct)
        {
            var request = new RecipeQueryRequest
            {
                Tag = tag,
                Difficulty = difficulty,
                IsPublished = true,
                Page = 1,
                PageSize = 100
            };

            var result = await _recipeService.QueryAsync(request, ct);
            return View(result.Items);
        }
    }
}
