using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClientBuilder.RuleSet;
using ClientBuilder.TestAssembly.Controllers;

namespace ClientBuilder.TestAssembly.RuleSet;

public class DecoratedControllerScanningRules : IScanningRules
{
    public IEnumerable<Type> FetchTypes(Assembly assembly) =>
        assembly.GetTypes().Where(x => x == typeof(DecoratedController));
}