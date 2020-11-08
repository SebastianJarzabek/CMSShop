using CmsShop.Models.Data;

namespace CmsShop.Models.ViewModels.Shop
{
  public class CategoryVM
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public string Slug { get; set; }

    public int Sorting { get; set; }

    public CategoryVM()
    {
    }

    public CategoryVM(CategoryDTO row)
    {
      Id = row.Id;
      Name = row.Name;
      Slug = row.Slug;
      Sorting = row.Sorting;
    }
  }
}