using System;
using Dyt.Data.Entities.Common;

namespace Dyt.Data.Entities.Content
{
    /// <summary>
    /// Tarif malzemelerini saklar.
    /// </summary>
 public class RecipeIngredient : BaseEntity
    {
        public int RecipeId { get; set; } // Hangi tarife ait

        public Recipe Recipe { get; set; } = null!; // Navigation property

        public string Name { get; set; } = string.Empty; // Malzeme adý ve miktarý

        public int DisplayOrder { get; set; } // Sýralama
    }
}
