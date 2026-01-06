using Dyt.Data.Context;
using Dyt.Data.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dyt.Web.Infrastructure
{
    /// <summary>
    /// Varsayýlan tarifleri veritabanýna ekler
    /// </summary>
    public static class RecipeSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
       using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

  // Zaten tarif varsa seed yapma
  if (await db.Recipes.AnyAsync())
           return;

            var recipes = new List<Recipe>
            {
    new Recipe
     {
 Title = "Yulaflý Meyveli Kahvaltý Kasesi",
         Slug = "yulafli-meyveli-kahvalti-kasesi",
       Summary = "Lif zengini, hýzlý ve doyurucu kahvaltý",
  PrepTime = "10 dk",
        Calories = "320 kcal",
            Difficulty = "Kolay",
          IsPublished = true,
              PublishDateUtc = DateTime.UtcNow,
    CreatedAtUtc = DateTime.UtcNow,
Ingredients = new List<RecipeIngredient>
         {
                new RecipeIngredient { Name = "1/2 su bardaðý yulaf", DisplayOrder = 0, CreatedAtUtc = DateTime.UtcNow },
       new RecipeIngredient { Name = "1/2 su bardaðý süt/yoðurt", DisplayOrder = 1, CreatedAtUtc = DateTime.UtcNow },
      new RecipeIngredient { Name = "1/2 muz", DisplayOrder = 2, CreatedAtUtc = DateTime.UtcNow },
 new RecipeIngredient { Name = "Çilek/ yaban mersini", DisplayOrder = 3, CreatedAtUtc = DateTime.UtcNow },
  new RecipeIngredient { Name = "1 yemek kaþýðý ceviz", DisplayOrder = 4, CreatedAtUtc = DateTime.UtcNow }
   },
      Steps = new List<RecipeStep>
           {
      new RecipeStep { Description = "Yulafý süt veya yoðurtla karýþtýrýn.", StepNumber = 1, CreatedAtUtc = DateTime.UtcNow },
        new RecipeStep { Description = "Üzerine meyve ve kuruyemiþ ekleyin.", StepNumber = 2, CreatedAtUtc = DateTime.UtcNow },
     new RecipeStep { Description = "Bal veya tarçýn ile tatlandýrýn.", StepNumber = 3, CreatedAtUtc = DateTime.UtcNow }
      }
      },
  new Recipe
         {
               Title = "Izgara Tavuklu Fit Salata",
           Slug = "izgara-tavuklu-fit-salata",
       Summary = "Protein dengeli, pratik öðle yemeði",
            PrepTime = "15 dk",
    Calories = "380 kcal",
                  Difficulty = "Kolay",
       IsPublished = true,
         PublishDateUtc = DateTime.UtcNow,
      CreatedAtUtc = DateTime.UtcNow,
          Ingredients = new List<RecipeIngredient>
       {
            new RecipeIngredient { Name = "120 g tavuk göðüs", DisplayOrder = 0, CreatedAtUtc = DateTime.UtcNow },
                 new RecipeIngredient { Name = "Karýþýk yeþillik", DisplayOrder = 1, CreatedAtUtc = DateTime.UtcNow },
         new RecipeIngredient { Name = "Cherry domates", DisplayOrder = 2, CreatedAtUtc = DateTime.UtcNow },
      new RecipeIngredient { Name = "1/2 avokado", DisplayOrder = 3, CreatedAtUtc = DateTime.UtcNow },
    new RecipeIngredient { Name = "1 yemek kaþýðý zeytinyaðý", DisplayOrder = 4, CreatedAtUtc = DateTime.UtcNow }
  },
             Steps = new List<RecipeStep>
           {
        new RecipeStep { Description = "Tavuðu zeytinyaðý ve baharatla marine edin.", StepNumber = 1, CreatedAtUtc = DateTime.UtcNow },
         new RecipeStep { Description = "Izgarada piþirin ve dilimleyin.", StepNumber = 2, CreatedAtUtc = DateTime.UtcNow },
     new RecipeStep { Description = "Yeþilliklerle karýþtýrýp soslayýn.", StepNumber = 3, CreatedAtUtc = DateTime.UtcNow }
       }
       },
    new Recipe
      {
          Title = "Fýrýnda Sebzeli Somon",
              Slug = "firinda-sebzeli-somon",
        Summary = "Omega-3 kaynaðý, akþam yemeði",
         PrepTime = "20 dk",
     Calories = "420 kcal",
              Difficulty = "Orta",
            IsPublished = true,
 PublishDateUtc = DateTime.UtcNow,
 CreatedAtUtc = DateTime.UtcNow,
         Ingredients = new List<RecipeIngredient>
         {
        new RecipeIngredient { Name = "1 dilim somon", DisplayOrder = 0, CreatedAtUtc = DateTime.UtcNow },
      new RecipeIngredient { Name = "Brokoli, havuç, kabak", DisplayOrder = 1, CreatedAtUtc = DateTime.UtcNow },
         new RecipeIngredient { Name = "1 yemek kaþýðý zeytinyaðý", DisplayOrder = 2, CreatedAtUtc = DateTime.UtcNow },
            new RecipeIngredient { Name = "Limon, tuz, karabiber", DisplayOrder = 3, CreatedAtUtc = DateTime.UtcNow }
              },
    Steps = new List<RecipeStep>
       {
   new RecipeStep { Description = "Somonu limon ve baharatla ovun.", StepNumber = 1, CreatedAtUtc = DateTime.UtcNow },
        new RecipeStep { Description = "Sebzeleri doðrayýp tepsiye dizin.", StepNumber = 2, CreatedAtUtc = DateTime.UtcNow },
     new RecipeStep { Description = "200°C fýrýnda 15-18 dk piþirin.", StepNumber = 3, CreatedAtUtc = DateTime.UtcNow }
            }
     },
     new Recipe
         {
   Title = "Þekersiz Muzlu Yulaflý Kurabiye",
      Slug = "sekersiz-muzlu-yulafli-kurabiye",
                Summary = "Ara öðün için saðlýklý atýþtýrmalýk",
      PrepTime = "18 dk",
    Calories = "90 kcal (adet)",
           Difficulty = "Kolay",
      IsPublished = true,
      PublishDateUtc = DateTime.UtcNow,
        CreatedAtUtc = DateTime.UtcNow,
          Ingredients = new List<RecipeIngredient>
   {
             new RecipeIngredient { Name = "1 olgun muz", DisplayOrder = 0, CreatedAtUtc = DateTime.UtcNow },
        new RecipeIngredient { Name = "1 su bardaðý yulaf", DisplayOrder = 1, CreatedAtUtc = DateTime.UtcNow },
   new RecipeIngredient { Name = "1 yemek kaþýðý damla çikolata/üzüm", DisplayOrder = 2, CreatedAtUtc = DateTime.UtcNow },
       new RecipeIngredient { Name = "Tarçýn", DisplayOrder = 3, CreatedAtUtc = DateTime.UtcNow }
         },
             Steps = new List<RecipeStep>
      {
     new RecipeStep { Description = "Muzu ezip yulafla karýþtýrýn.", StepNumber = 1, CreatedAtUtc = DateTime.UtcNow },
      new RecipeStep { Description = "Damla çikolata/üzüm ekleyin.", StepNumber = 2, CreatedAtUtc = DateTime.UtcNow },
       new RecipeStep { Description = "180°C'de 12-14 dk piþirin.", StepNumber = 3, CreatedAtUtc = DateTime.UtcNow }
         }
          }
            };

            // Tag'leri oluþtur
     var tags = new Dictionary<string, Tag>();
    var tagNames = new[] { "kahvaltý", "lif", "vegan", "öðle", "protein", "glutensiz", "akþam", "omega-3", "düþük-karbonhidrat", "atýþtýrmalýk", "þekersiz" };
   
   foreach (var tagName in tagNames)
     {
   var tag = new Tag
    {
        Name = tagName,
     Slug = tagName.Replace("ý", "i").Replace("ð", "g").Replace("ü", "u").Replace("þ", "s").Replace("ö", "o").Replace("ç", "c"),
        CreatedAtUtc = DateTime.UtcNow
      };
           tags[tagName] = tag;
          await db.Tags.AddAsync(tag);
     }

   // Tariflere tag'leri ekle
            recipes[0].RecipeTags.Add(new RecipeTag { Tag = tags["kahvaltý"] });
     recipes[0].RecipeTags.Add(new RecipeTag { Tag = tags["lif"] });
        recipes[0].RecipeTags.Add(new RecipeTag { Tag = tags["vegan"] });

    recipes[1].RecipeTags.Add(new RecipeTag { Tag = tags["öðle"] });
            recipes[1].RecipeTags.Add(new RecipeTag { Tag = tags["protein"] });
     recipes[1].RecipeTags.Add(new RecipeTag { Tag = tags["glutensiz"] });

            recipes[2].RecipeTags.Add(new RecipeTag { Tag = tags["akþam"] });
    recipes[2].RecipeTags.Add(new RecipeTag { Tag = tags["omega-3"] });
            recipes[2].RecipeTags.Add(new RecipeTag { Tag = tags["düþük-karbonhidrat"] });

        recipes[3].RecipeTags.Add(new RecipeTag { Tag = tags["atýþtýrmalýk"] });
 recipes[3].RecipeTags.Add(new RecipeTag { Tag = tags["þekersiz"] });
      recipes[3].RecipeTags.Add(new RecipeTag { Tag = tags["vegan"] });

            await db.Recipes.AddRangeAsync(recipes);
            await db.SaveChangesAsync();
        }
    }
}
