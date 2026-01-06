namespace Dyt.Data.Entities.Content
{
    /// <summary>
    /// Tarif ve Tag arasındaki çoka-çok ilişkiyi temsil eder.
    /// </summary>
    public class RecipeTag
    {
 public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;

     public int TagId { get; set; }
        public Tag Tag { get; set; } = null!;
    }
}
