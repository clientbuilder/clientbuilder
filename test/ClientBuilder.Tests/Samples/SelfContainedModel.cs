using System.Collections.Generic;

namespace ClientBuilder.Tests.Samples;

public class SelfContainedModel
{
    public SelfContainedModel Child { get; set; }

    public IEnumerable<SelfContainedModel> Children { get; set; }
}