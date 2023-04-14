using cleanarch4.Infrastructure.Data;
using cleanarch4.Web.DTO;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace cleanarch4.Web.Pages.KnowPills;

public class Index : PageModel
{
  private readonly AppDbContext _appDbContext;

  public InputModel Input { get; set; } = new InputModel();

  public Index(AppDbContext appDbContext)
  {
    _appDbContext = appDbContext;
  }
  public void OnGet()
  {
    Input.KnowledgePillsCount = _appDbContext.KnowledgePills.Count();
    Input.Pills = _appDbContext.KnowledgePills.Select(p => new KnowledgePillPoco()
    {
      Title = p.Title, Link = p.Link, Description = p.Description, Id = p.Id
    }).ToList();
  }

  public class InputModel
  {
    public int KnowledgePillsCount { get; set; }
    public List<KnowledgePillPoco> Pills { get; set; }
  }
  
}
