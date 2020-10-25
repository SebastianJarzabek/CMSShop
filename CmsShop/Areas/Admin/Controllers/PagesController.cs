using CmsShop.Models.Data;
using CmsShop.Models.ViewModels.Pages;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.MappingViews;
using System.Linq;
using System.Web.Mvc;

namespace CmsShop.Areas.Admin.Controllers
{
  public class PagesController : Controller
  {
    // GET: Admin/Pages
    [HttpGet]
    public ActionResult Index()
    {

      // Deklaracja listy stron PAgeVM
      List<PageVM> pagesList = new List<PageVM>();


      //Inicializacja listy PageVM
      using (Db db = new Db())
      {
        pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
      }

      //Zwracamy strony do widoku 
      return View(pagesList);
    }

    // GET: Admin/Pages/AddPage
    [HttpGet]
    public ActionResult AddPage()
    {
      return View();
    }

    // POST: Admin/Pages/AddPage
    [HttpPost]
    public ActionResult AddPage(PageVM model)
    {
      // Validacja formularza model.state
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      using (Db db = new Db())
      {
        string slug;

        //Inicializacja PAgeDTO
        PageDTO pageDTO = new PageDTO();

        //Jesli slug jest nie wypełniony to przypisujemy do niego tytuł
        if (string.IsNullOrWhiteSpace(model.Slug))
        {
          var validHelper = model.Title.Replace(" ", "-").ToLower();
          slug = StartWithValidation(validHelper);
        }
        else
        {
          var validHelper = model.Slug.Replace(" ", "-").ToLower();
          slug = StartWithValidation(validHelper);
        }

        //Zapobiegamy dodaniu takiej samej nazwy strony
        if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
        {
          ModelState.AddModelError("", "Ten tytuł lub adres strony już istnieje.");
          return View(model);
        }

        pageDTO.Title = model.Title;
        pageDTO.Slug = slug;
        pageDTO.Body = model.Body;
        pageDTO.HasSidebar = model.HasSidebar;
        pageDTO.Sorting = 1000;

        //Zapis dto
        db.Pages.Add(pageDTO);
        db.SaveChanges();
      }
      TempData["SM"] = "Dodałeś nową stronę.";

      return RedirectToAction("AddPAge");
    }

    // GET: Admin/Pages/EditPage
    [HttpGet]
    public ActionResult EditPage(int id)
    {
      //Deklaracja pageVM
      PageVM model;

      using (Db db = new Db())
      {
        // Pobieramy stronę o id
        PageDTO dto = db.Pages.Find(id);
        //czy strona istnieje?
        if (dto == null)
        {
          return Content("Strona nie istnieje.");
        }
        //Przypisujemy i zwracamy do widoku.
        model = new PageVM(dto);
      }
      return View(model);
    }

    // POST: Admin/Pages/EditPage
    [HttpPost]
    public ActionResult EditPage(PageVM model)
    {
      if (!ModelState.IsValid)
      {
        return (View(model));
      }

      using (Db db = new Db())
      {
        // Pobieramy id

        int id = model.Id;
        string slug = string.Empty;

        //pobranie strony do edycji
        PageDTO dto = db.Pages.Find(id);

        dto.Title = model.Title;
        if (model.Slug != "home")
        {
          if (string.IsNullOrWhiteSpace(model.Slug))
          {
            var validHelper = model.Title.Replace(" ", "-").ToLower();
            slug = StartWithValidation(validHelper);
          }
          else
          {
            var validHelper = model.Slug.Replace(" ", "-").ToLower();
            slug = StartWithValidation(validHelper);
          }
        }

        // Sprawdzamy czy nasza strona lub adres juz isnieje 
        if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title)
         || db.Pages.Where(x => x.Id != id).Any(x => x.Title == slug))
        {
          ModelState.AddModelError("", "Strona lub tytuł już istnieje.");
        }

        //Modyfikacja DTO
        dto.Title = model.Title;
        dto.Slug = model.Slug;
        dto.Body = model.Body;
        dto.HasSidebar = model.HasSidebar;

        // Zapis edytowanej strony do bazy
        db.SaveChanges();
      }

      TempData["SM"] = "Strona została wyedytowana";
      //Resirect


      return RedirectToAction("EditPage");
    }

    // GET: Admin/Pages/Details/id
    [HttpGet]
    public ActionResult Details(int id)
    {
      //deklaracja PageVM
      PageVM model;

      using (Db db = new Db())
      {
        // Pobranie strony o id
        PageDTO dto = db.Pages.Find(id);

        // sprawdzenie czy strona o takim id istnieje
        if (dto == null)
        {
          return Content("Strona o podanym id nie istnieje.");
        }

        // inicjalizacja PageVM
        model = new PageVM(dto);
      }

      return View(model);
    }

    // GET: Admin/Pages/Delete/id
    [HttpGet]
    public ActionResult Delete(int id)
    {
      using (Db db = new Db())
      {
        //Pobranie strony do usunięcia
        PageDTO dto = db.Pages.Find(id);

        //Usuwanie wybranej strony z bazy
        db.Pages.Remove(dto);

        db.SaveChanges();
      }
      return RedirectToAction("Index");
    }

    //POST: Admin/Pages/ReorderPages
    [HttpPost]
    public ActionResult ReorderPages(int[] id)
    {
      using (Db db = new Db())
      {
        int counter = 1;
        PageDTO dto;

        //Sortowanie stron 
        foreach (var pageId in id)
        {
          dto = db.Pages.Find(pageId);
          dto.Sorting = counter;

          //Zapis na bazie
          db.SaveChanges();

          counter++;
        }
      }

      return View();
    }

    //GET: Admin/Pages/EditSidebar
    [HttpGet]
    public ActionResult EditSidebar()
    {
      //Deklaracja Sidebar
      SidebarVM model;

      using (Db db = new Db())
      {
        //Pobieramy SidebarDTO
        SidebarDTO dto = db.Sidebar.Find(1);

        //Inicializacja modelu
        model = new SidebarVM(dto);
      }
      return View(model);
    }

    //POST: Admin/Pages/EditSidebar
    [HttpPost]
    public ActionResult EditSidebar(SidebarVM model)
    {
      using (Db db = new Db())
      {
        //Pobieramy SidebarDTO
        SidebarDTO dto = db.Sidebar.Find(1);

        //modyfikacja Sidebar
        dto.Body = model.Body;
        //Zapisujemy na bazie
        db.SaveChanges();
      }

      //Ustawiamy tempdata o zmodyfikowaniu

      TempData["SM"] = "Zmodyfikowano pasek boczny.";

      return RedirectToAction("EditSidebar");
    }


    private string StartWithValidation(string validHelper)
    {
      string _validString = string.Empty;

      if (validHelper.StartsWith("-"))
      {
        return _validString = validHelper.Remove(0, 1);
      }
      else
      {
        return _validString = validHelper;
      }
    }
  }
}