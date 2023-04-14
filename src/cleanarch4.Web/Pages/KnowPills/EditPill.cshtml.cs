using System.Security.Cryptography.X509Certificates;
using cleanarch4.Core.ProjectAggregate;
using cleanarch4.Infrastructure.Data;
using cleanarch4.Web.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace cleanarch4.Web.Pages.KnowPills;

public class EditPill : PageModel
{
  private readonly AppDbContext _appDbContext;

  [BindProperty]
  public InputModel Input { get; set; } = new InputModel()
    { KnowledgePill = new KnowledgePillPoco()};
  

  public EditPill(AppDbContext appDbContext)
  {
    _appDbContext = appDbContext;
  }
  
  public void OnGet(int id)
  {
    KnowledgePill? pill = _appDbContext.KnowledgePills.Find(id);
    if (pill != null)
    {
      Input.KnowledgePill = new KnowledgePillPoco()
      {
        Id = pill.Id, Title = pill.Title, Description = pill.Description, Link = pill.Link
      };
    }
    // return NotFound();
  }

  public void OnPost()
  {
    KnowledgePill? pill = _appDbContext.KnowledgePills.Find(Input.KnowledgePill.Id);
    if (pill != null)
    {
      pill.Title = Input.KnowledgePill.Title;
      pill.Link = Input.KnowledgePill.Link;
      pill.Description = Input.KnowledgePill.Description;
      _appDbContext.SaveChanges();
      Input.Message = $"Knowledge pill '{pill.Title}' updated.";
    }
  }
  
  public class InputModel
  {
    public KnowledgePillPoco KnowledgePill { get; set; }
    public string Message { get; set; }
  }
}
