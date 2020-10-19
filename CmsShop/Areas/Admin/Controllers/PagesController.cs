using CmsShop.Models.Data;
using CmsShop.Models.ViewModels.Pages;
using System.Collections.Generic;
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

    private string StartWithValidation(string validHelper)
    {
      string validString;
      if (validHelper.StartsWith("-"))
      {
        return validString = validHelper.Remove(0, 1);
      }
      else
      {
        return validString = validHelper;
      }
    }
  }
}