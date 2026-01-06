namespace Dyt.Contracts.Recipes.Responses
{
    public class RecipeDto
    {
        public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
      public string Summary { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string PrepTime { get; set; } = string.Empty;
   public string Calories { get; set; } = string.Empty;
      public string Difficulty { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public DateTime? PublishDateUtc { get; set; }
        public List<string> Tags { get; set; } = new();
        public List<RecipeIngredientDto> Ingredients { get; set; } = new();
        public List<RecipeStepDto> Steps { get; set; } = new();
    }

    public class RecipeIngredientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }

    public class RecipeStepDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public int StepNumber { get; set; }
    }
}
