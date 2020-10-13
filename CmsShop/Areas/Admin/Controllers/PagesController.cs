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
    public ActionResult Index()
    {

      // Deklaracja listy stron PAgeVM
      List<PageVM> pagesList = new List<PageVM>();


      //Inicializacja listy PageVM
      using (Db db = new Db())
      {
        pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x=> new PageVM(x)).ToList();
      }

      //Zwracamy strony do widoku 
      return View(pagesList);
    }
  }
}