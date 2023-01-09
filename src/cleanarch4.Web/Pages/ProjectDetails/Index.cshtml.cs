using cleanarch4.Core.ProjectAggregate;
using cleanarch4.Core.ProjectAggregate.Specifications;
using cleanarch4.SharedKernel.Interfaces;
using cleanarch4.Web.ApiModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace cleanarch4.Web.Pages.ProjectDetails;

public class IndexModel : PageModel
{
  private readonly IRepository<Project> _repository;
  private readonly IHostEnvironment _hostEnvironment;

  [BindProperty(SupportsGet = true)]
  public int ProjectId { get; set; }

  public string Message { get; set; } = "";

  public string Environment { get; }

  public ProjectDTO? Project { get; set; }

  public IndexModel(IRepository<Project> repository, IHostEnvironment hostEnvironment)
  {
    _repository = repository;
    _hostEnvironment = hostEnvironment;
    this.Environment = _hostEnvironment.EnvironmentName;
  }

  public async Task OnGetAsync()
  {
    var projectSpec = new ProjectByIdWithItemsSpec(ProjectId);
    var project = await _repository.FirstOrDefaultAsync(projectSpec);
    if (project == null)
    {
      Message = "No project found.";

      return;
    }

    Project = new ProjectDTO
    (
        id: project.Id,
        name: project.Name,
        items: project.Items
        .Select(item => ToDoItemDTO.FromToDoItem(item))
        .ToList()
    );
  }
}
