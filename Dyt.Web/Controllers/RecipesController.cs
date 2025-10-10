using Microsoft.AspNetCore.Mvc;

namespace Dyt.Web.Controllers
{
    public class RecipesController : Controller
    {
        public record RecipeVm(string Title, string Summary, string ImageUrl, string PrepTime, string Calories, string Difficulty, string[] Tags, string[] Steps, string[] Ingredients);

        [HttpGet]
        public IActionResult Index()
        {
            var list = new List<RecipeVm>
            {
                new(
                    Title: "Yulaflý Meyveli Kahvaltý Kasesi",
                    Summary: "Lif zengini, hýzlý ve doyurucu kahvaltý",
                    ImageUrl: string.Empty,
                    PrepTime: "10 dk",
                    Calories: "320 kcal",
                    Difficulty: "Kolay",
                    Tags: new[]{"kahvaltý","lif","vegan"},
                    Steps: new[]{"Yulafý süt veya yoðurtla karýþtýrýn.","Üzerine meyve ve kuruyemiþ ekleyin.","Bal veya tarçýn ile tatlandýrýn."},
                    Ingredients: new[]{"1/2 su bardaðý yulaf","1/2 su bardaðý süt/yoðurt","1/2 muz","Çilek/ yaban mersini","1 yemek kaþýðý ceviz"}
                ),
                new(
                    Title: "Izgara Tavuklu Fit Salata",
                    Summary: "Protein dengeli, pratik öðle yemeði",
                    ImageUrl: string.Empty,
                    PrepTime: "15 dk",
                    Calories: "380 kcal",
                    Difficulty: "Kolay",
                    Tags: new[]{"öðle","protein","glutensiz"},
                    Steps: new[]{"Tavuðu zeytinyaðý ve baharatla marine edin.","Izgarada piþirin ve dilimleyin.","Yeþilliklerle karýþtýrýp soslayýn."},
                    Ingredients: new[]{"120 g tavuk göðüs","Karýþýk yeþillik","Cherry domates","1/2 avokado","1 yemek kaþýðý zeytinyaðý"}
                ),
                new(
                    Title: "Fýrýnda Sebzeli Somon",
                    Summary: "Omega-3 kaynaðý, akþam yemeði",
                    ImageUrl: string.Empty,
                    PrepTime: "20 dk",
                    Calories: "420 kcal",
                    Difficulty: "Orta",
                    Tags: new[]{"akþam","omega-3","düþük-karbonhidrat"},
                    Steps: new[]{"Somonu limon ve baharatla ovun.","Sebzeleri doðrayýp tepsiye dizin.","200°C fýrýnda 15-18 dk piþirin."},
                    Ingredients: new[]{"1 dilim somon","Brokoli, havuç, kabak","1 yemek kaþýðý zeytinyaðý","Limon, tuz, karabiber"}
                ),
                new(
                    Title: "Þekersiz Muzlu Yulaflý Kurabiye",
                    Summary: "Ara öðün için saðlýklý atýþtýrmalýk",
                    ImageUrl: string.Empty,
                    PrepTime: "18 dk",
                    Calories: "90 kcal (adet)",
                    Difficulty: "Kolay",
                    Tags: new[]{"atýþtýrmalýk","þekersiz","vegan"},
                    Steps: new[]{"Muzu ezip yulafla karýþtýrýn.","Damla çikolata/üzüm ekleyin.","180°C'de 12-14 dk piþirin."},
                    Ingredients: new[]{"1 olgun muz","1 su bardaðý yulaf","1 yemek kaþýðý damla çikolata/üzüm","Tarçýn"}
                )
            };
            return View(list);
        }
    }
}
