using Dyt.Contracts.Common;
using Dyt.Contracts.Recipes.Requests;
using Dyt.Contracts.Recipes.Responses;

namespace Dyt.Business.Interfaces.Recipes
{
    /// <summary>
    /// Tarif içeriklerinin listelenmesi ve yönetimi için servis sözleþmesi.
    /// </summary>
    public interface IRecipeService
  {
        Task<PagedResult<RecipeDto>> QueryAsync(RecipeQueryRequest request, CancellationToken ct = default);
        Task<int> CreateAsync(RecipeCreateRequest request, CancellationToken ct = default);
 Task<RecipeDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<RecipeDto?> GetBySlugAsync(string slug, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, RecipeUpdateRequest request, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
