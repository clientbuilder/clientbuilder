using System.IO;
using System.Linq;
using ClientBuilder.Core.Modules;
using ClientBuilder.Models;

namespace ClientBuilder.Common;

/// <summary>
/// Helper that maps core models into response ones.
/// </summary>
public static class ResponseMapper
{
    /// <summary>
    /// Maps a specified <see cref="ScaffoldModule"/> into a <see cref="ScaffoldModuleModel"/>.
    /// </summary>
    /// <param name="module"></param>
    /// <returns></returns>
    public static ScaffoldModuleModel MapToModel(ScaffoldModule module)
    {
        return new ScaffoldModuleModel
        {
            Id = module.Id,
            Name = module.Name,
            Order = module.Order,
            ClientId = module.ClientId,
            ClientName = module.ClientName,
            SourceDirectory = module.SourceDirectory,
            Generated = module.Generated,
            Files = module.GetFiles().Select(x => new ScaffoldModuleFileSystemItemModel
            {
                Name = x.Name,
                Path = Path.Combine(x.RelativePath, x.Name),
            }),
            Folders = module.GetFolders().Select(x => new ScaffoldModuleFileSystemItemModel
            {
                Name = x.Name,
                Path = Path.Combine(x.RelativePath, x.Name),
            }),
        };
    }
}