using CmsShop.Models.Data;
using CmsShop.Models.ViewModels.Shop;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;

namespace CmsShop.Areas.Admin.Controllers
{
  public class ShopController : Controller
  {
    // GET: Admin/Shop/Categories
    public ActionResult Categories()
    {
      //Deklaracja listy kategori do wyświetlenia.
      List<CategoryVM> categoryVMList;

      using (Db db = new Db())
      {
        categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
      }
      return View(categoryVMList);
    }

    //POST: Admin/Shop/AddNewCategory
    [HttpPost]
    public string AddNewCategory(string catName)
    {
      string id;

      using (Db db = new Db())
      {
        // sprawdzamy czy nazwa kategorii jest unikalna
        if (db.Categories.Any(x => x.Name == catName))
        {
          return "tytulzajety";
        }
        CategoryDTO dto = new CategoryDTO();
        dto.Name = catName;
        dto.Slug = catName.Replace(" ", "-").ToLower();
        dto.Sorting = 1000;

        db.Categories.Add(dto);
        db.SaveChanges();

        // pobranie id 
        id = dto.Id.ToString();
      }
      return id;
    }

    //POST: Admin/Shop/AddNewCategory
    [HttpPost]
    public ActionResult snippet()
    {
      return View();
    }


  }
}