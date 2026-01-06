using System;
using Dyt.Data.Entities.Common;

namespace Dyt.Data.Entities.Content
{
    /// <summary>
    /// Tarif adýmlarýný saklar.
    /// </summary>
    public class RecipeStep : BaseEntity
    {
      public int RecipeId { get; set; } // Hangi tarife ait

        public Recipe Recipe { get; set; } = null!; // Navigation property

      public string Description { get; set; } = string.Empty; // Adým açýklamasý

        public int StepNumber { get; set; } // Adým numarasý
    }
}
