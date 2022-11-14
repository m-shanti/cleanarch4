using Autofac;
using cleanarch4.Core.Interfaces;
using cleanarch4.Core.Services;

namespace cleanarch4.Core;

public class DefaultCoreModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    builder.RegisterType<ToDoItemSearchService>()
        .As<IToDoItemSearchService>().InstancePerLifetimeScope();
  }
}
