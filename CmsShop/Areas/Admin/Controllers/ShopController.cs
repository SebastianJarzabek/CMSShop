using CmsShop.Models.Data;
using CmsShop.Models.ViewModels.Shop;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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

    //POST: Admin/Shop/ReorderCategories
    [HttpPost]
    public ActionResult ReorderCategories(int[] id)
    {
      using (Db db = new Db())
      {
        int count = 1;

        CategoryDTO dto = new CategoryDTO();
        foreach (var catId in id)
        {
          dto = db.Categories.Find(catId);
          dto.Sorting = count;
          db.SaveChanges();
          count++;
        }
      }
      return View();
    }

    //GET: Admin/Shop/DeleteCategory
    [HttpGet]
    public ActionResult DeleteCategory(int id)
    {
      using (Db db = new Db())
      {
        CategoryDTO dto = db.Categories.Find(id);
        db.Categories.Remove(dto);
        db.SaveChanges();
      }
      return RedirectToAction("Categories");
    }

    //POST: Admin/Shop/RenameCategory
    [HttpPost]
    public string RenameCategory(string newCatName, int id)
    {
      using (Db db = new Db())
      {
        if (db.Categories.Any(X => X.Name == newCatName))
        {
          return "tytulzajety";
        }

        CategoryDTO dto = db.Categories.Find(id);
        dto.Name = newCatName;
        dto.Slug = newCatName.Replace(" ", "-").ToLower();
        db.SaveChanges();
      }
      return "Ok";
    }

    //GET: Admin/Shop/AddProduct
    [HttpGet]
    public ActionResult AddProduct()
    {
      ProductVM model = new ProductVM();

      using (Db db = new Db())
      {
        model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
      }
      return View(model);
    }

    //POST: Admin/Shop/AddProduct
    [HttpPost]
    public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
    {
      if (!ModelState.IsValid)
      {
        using (Db db = new Db())
        {
          model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
          return View(model);
        }
      }

      using (Db db = new Db())
      {
        if (db.Products.Any(x => x.Name == model.Name))
        {
          model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
          ModelState.AddModelError("", "Nazwa jest już zajęta.");
          return View(model);
        }
      }

      int id;

      using (Db db = new Db())
      {
        ProductDTO product = new ProductDTO();
        product.Name = model.Name;
        product.Slug = model.Name.Replace(" ", "-").ToLower();
        product.Description = model.Description;
        product.Price = model.Price;
        product.CategoryId = model.CategoryId;

        CategoryDTO catDto = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
        product.CategoryName = catDto.Name;
        db.Products.Add(product);
        db.SaveChanges();
        id = product.Id;
      }
      TempData["SM"] = "Dodałeś produkt";

      var orginalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
      var pathString1 = Path.Combine(orginalDirectory.ToString(), "Products");
      var pathString2 = Path.Combine(orginalDirectory.ToString(), "Products\\" + id.ToString());
      var pathString3 = Path.Combine(orginalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
      var pathString4 = Path.Combine(orginalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
      var pathString5 = Path.Combine(orginalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

      if (!Directory.Exists(pathString1))
      {
        Directory.CreateDirectory(pathString1);
      }

      if (!Directory.Exists(pathString2))
      {
        Directory.CreateDirectory(pathString2);
      }

      if (!Directory.Exists(pathString3))
      {
        Directory.CreateDirectory(pathString3);
      }

      if (!Directory.Exists(pathString4))
      {
        Directory.CreateDirectory(pathString4);
      }

      if (!Directory.Exists(pathString5))
      {
        Directory.CreateDirectory(pathString5);
      }

      if (file != null && file.ContentLength > 0)
      {
        string ext = file.ContentType.ToLower();
        if (ext != "image/jpg" && ext != "image/jpeg" && ext != "image/pjpeg" && ext != "image/gif" && ext != "image/x-png" && ext != "image/png")
        {
          using (Db db = new Db())
          {
            model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
            ModelState.AddModelError("", "Obraz nie został przesłany - nieprawidłowe rozszerzenie obrazu");
            return View(model);
          }
        }
        string imageName = file.FileName;
        using (Db db = new Db())
        {
          ProductDTO dto = db.Products.Find(id);
          dto.ImageName = imageName;
          db.SaveChanges();
        }

        var imagePath = string.Format("{0}\\{1}", pathString2, imageName);

        var thumbPath = string.Format("{0}\\{1}", pathString3, imageName);

        file.SaveAs(imagePath);

        WebImage thumb = new WebImage(file.InputStream);
        thumb.Resize(200, 200);
        thumb.Save(pathString2);
      }
      return RedirectToAction("AddProduct");
    }
  }
}