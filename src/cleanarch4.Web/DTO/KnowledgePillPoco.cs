using System.ComponentModel.DataAnnotations;

namespace cleanarch4.Web.DTO;

public class KnowledgePillPoco
{
  [Display(Name = "Title")]
  public string Title { get; set; }
  public string Description { get; set; }
  [Url]
  public string Link { get; set; }
  public int Id { get; set; }
}
