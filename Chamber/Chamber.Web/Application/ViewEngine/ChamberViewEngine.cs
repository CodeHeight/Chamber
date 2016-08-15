using System;
using System.Linq;
using System.Web.Mvc;

namespace Chamber.Web.Application.ViewEngine
{
    public class ChamberViewEngine : IViewEngine
    {
        private readonly RazorViewEngine _defaultViewEngine = new RazorViewEngine();
        private string _lastTheme;
        private RazorViewEngine _lastEngine;
        private readonly object _lock = new object();
        private readonly string _defaultTheme;

        public ChamberViewEngine(string defaultTheme)
        {
            _defaultTheme = defaultTheme;
        }

        private RazorViewEngine CreateRealViewEngine()
        {
            lock (_lock)
            {
                string settingsTheme;
                try
                {
                    settingsTheme = _defaultTheme;
                    if (settingsTheme == _lastTheme)
                    {
                        return _lastEngine;
                    }
                }
                catch (Exception)
                {
                    return _defaultViewEngine;
                }

                _lastEngine = new RazorViewEngine();

                _lastEngine.PartialViewLocationFormats =
                    new[]
                    {
                        "~/Views/{1}/{0}.cshtml",
                        "~/Views/Shared/{0}.cshtml",
                        "~/Views/Shared/{1}/{0}.cshtml",
                        "~/Views/Extensions/{1}/{0}.cshtml"
                    }.Union(_lastEngine.PartialViewLocationFormats).ToArray();

                _lastEngine.ViewLocationFormats =
                    new[]
                    {
                        "~/Views/{1}/{0}.cshtml",
                        "~/Views/Extensions/{1}/{0}.cshtml"
                    }.Union(_lastEngine.ViewLocationFormats).ToArray();

                _lastEngine.MasterLocationFormats =
                    new[]
                    {
                        "~/Views/{1}/{0}.cshtml",
                        "~/Views/Shared/{1}/{0}.cshtml",
                        "~/Views/Shared/{0}.cshtml",
                        "~/Views/Extensions/{1}/{0}.cshtml"
                    }.Union(_lastEngine.MasterLocationFormats).ToArray();

                _lastTheme = settingsTheme;

                return _lastEngine;
            }
        }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return CreateRealViewEngine().FindPartialView(controllerContext, partialViewName, useCache);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return CreateRealViewEngine().FindView(controllerContext, viewName, masterName, useCache);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            CreateRealViewEngine().ReleaseView(controllerContext, view);
        }
    }
}