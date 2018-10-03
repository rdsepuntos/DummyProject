using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Allegro.Framework.Api
{
    public class WebApiSettings
    {
        private readonly Dictionary<AssemblyType, Assembly> _assembly;

        public WebApiSettings(Dictionary<AssemblyType, Assembly> assembly)
        {
            _assembly = assembly;
            ConfigureWebApi();
        }

        public string ControllerNamespace { get; set; }
        public string WebApiName { get; set; }

        public void ConfigureWebApi()
        {
            ControllerNamespace = _assembly[AssemblyType.Controller].Modules.First().Name;
            ControllerNamespace = ControllerNamespace.Replace(".dll", string.Empty);
            WebApiName = ControllerNamespace.Split('.')[1];
        }
    }
}
