using CmsShop.Models.Data;
using System.Web.Mvc;

namespace CmsShop.Models.ViewModels.Pages
{
  public class SidebarVM
  {
    public int Id { get; set; }

    [AllowHtml]
    public string Body { get; set; }

    public SidebarVM()
    {
    }

    public SidebarVM(SidebarDTO row)
    {
      Id = row.Id;
      Body = row.Body;
    }

  }
}