using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Contracts.Blog; // Blog DTO'larını kullanmak için ekliyorum
using Dyt.Contracts.Common; // Sayfalama için ortak tipleri kullanmak için ekliyorum
using Dyt.Contracts.Blog.Requests;    // BlogPostCreateRequest, BlogPostQueryRequest burada
using Dyt.Contracts.Blog.Responses;   // BlogPostDto, TagDto burada
using Dyt.Contracts.Common;           // PagedResult<T> burada

namespace Dyt.Business.Interfaces.Blog
{
    /// <summary>
    /// Blog içeriklerinin listelenmesi ve yönetimi için servis sözleşmesi.
    /// </summary>
    public interface IBlogService
    {
        Task<PagedResult<BlogPostDto>> QueryAsync(BlogPostQueryRequest request, CancellationToken ct = default); // Listeleme
        Task<int> CreateAsync(BlogPostCreateRequest request, CancellationToken ct = default);                    // Oluşturma
        Task<BlogPostDto?> GetBySlugAsync(string slug, CancellationToken ct = default);                         // Tekil getir
    }
}


