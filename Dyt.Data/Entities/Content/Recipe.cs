using System;
using System.Collections.Generic;
using Dyt.Data.Entities.Common;

namespace Dyt.Data.Entities.Content
{
  /// <summary>
    /// Tarif varlýðý. Saðlýklý yemek tariflerini ve detaylarýný içerir.
    /// </summary>
    public class Recipe : AuditableEntity
    {
        public string Title { get; set; } = string.Empty; // Tarif baþlýðý

    public string Slug { get; set; } = string.Empty; // URL dostu kýsa ad (benzersiz)

   public string Summary { get; set; } = string.Empty; // Kýsa açýklama

  public string? ImageUrl { get; set; } // Tarif görseli

        public string PrepTime { get; set; } = string.Empty; // Hazýrlama süresi (örn: "15 dk")

        public string Calories { get; set; } = string.Empty; // Kalori bilgisi (örn: "320 kcal")

        public string Difficulty { get; set; } = "Kolay"; // Zorluk seviyesi

        public bool IsPublished { get; set; } = true; // Yayýnda mý

    public DateTime? PublishDateUtc { get; set; } // Yayýn tarihi

        // Ýliþkiler
        public ICollection<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();
        public ICollection<RecipeStep> Steps { get; set; } = new List<RecipeStep>();
        public ICollection<RecipeTag> RecipeTags { get; set; } = new List<RecipeTag>();
    }
}
