namespace Dyt.Contracts.Recipes.Requests
{
    public class RecipeQueryRequest
 {
        public string? Search { get; set; }
        public string? Tag { get; set; }
        public string? Difficulty { get; set; }
        public bool? IsPublished { get; set; }
 public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
  }

    public class RecipeCreateRequest
    {
   public string Title { get; set; } = string.Empty;
   public string Summary { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string PrepTime { get; set; } = string.Empty;
     public string Calories { get; set; } = string.Empty;
      public string Difficulty { get; set; } = "Kolay";
   public List<string> Tags { get; set; } = new();
  public List<string> Ingredients { get; set; } = new();
      public List<string> Steps { get; set; } = new();
   public bool IsPublished { get; set; } = true;
  }

    public class RecipeUpdateRequest
    {
        public string Title { get; set; } = string.Empty;
      public string Summary { get; set; } = string.Empty;
      public string? ImageUrl { get; set; }
        public string PrepTime { get; set; } = string.Empty;
        public string Calories { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "Kolay";
        public List<string> Tags { get; set; } = new();
     public List<string> Ingredients { get; set; } = new();
    public List<string> Steps { get; set; } = new();
        public bool IsPublished { get; set; } = true;
    }
}
