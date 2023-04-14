using cleanarch4.Core.ProjectAggregate;
using cleanarch4.Infrastructure.Data;
using cleanarch4.Web.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace cleanarch4.Web.Pages.KnowPills;

public class NewPill : PageModel
{
  private readonly AppDbContext _appDbContext;

  public NewPill(AppDbContext appDbContext)
  {
    _appDbContext = appDbContext;
  }

  [BindProperty] 
  public InputModel Input { get; set; } = new InputModel(); // tihs initializer is important for first generation
  
  public void OnPost()
  {
    EntityEntry<KnowledgePill> entityEntry = _appDbContext.KnowledgePills.Add(new KnowledgePill()
    {
      Title = Input.KnowledgePill.Title,
      Description = Input.KnowledgePill.Description,
      Link = Input.KnowledgePill.Link
    });
    _appDbContext.SaveChanges();
    Input.Message = $"Knowledge pill '{entityEntry.Entity.Title}' created with Id={entityEntry.Entity.Id}.";
  }
  
  public class InputModel
  {
    public KnowledgePillPoco KnowledgePill { get; set; }
    public string Message { get; set; }
  }
}
