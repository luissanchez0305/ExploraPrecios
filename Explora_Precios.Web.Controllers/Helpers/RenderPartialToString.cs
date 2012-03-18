using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Explora_Precios.Web.Controllers.Helpers
{

    public static class RenderPartialToString
    {
        ///// <summary>Renders a view to string.</summary> 
        //public static string RenderPartialToString(this HtmlHelper html, string viewName, object viewData)
        //{
        //    return RenderViewToString(html.ViewContext.Controller.ControllerContext, viewName, viewData);
        //}
        /// <summary>Renders a view to string.</summary> 
        public static string RenderViewToString(this Controller controller, string viewName, object viewData)
        {
            return RenderViewToString(controller.ControllerContext, viewName, viewData);
        }
        public static string RenderViewToString(this Controller controller, string viewName, object viewData, object model)
        {
            return RenderViewToString(controller.ControllerContext, viewName, viewData, model);
        }
        public static string RenderViewToString(ControllerContext context,
                                            string viewName, object viewData)
        {
            //Create memory writer 
            var sb = new StringBuilder();
            var memWriter = new StringWriter(sb);

            //Create fake http context to render the view 
            var fakeResponse = new HttpResponse(memWriter);
            var fakeContext = new HttpContext(HttpContext.Current.Request, fakeResponse);
            var fakeControllerContext = new ControllerContext(
                new HttpContextWrapper(fakeContext),
                context.RouteData, context.Controller);

            var oldContext = HttpContext.Current;
            HttpContext.Current = fakeContext;

            //Use HtmlHelper to render partial view to fake context 
            var html = new HtmlHelper(new ViewContext(fakeControllerContext,
                new FakeView(), new ViewDataDictionary(), new TempDataDictionary()),
                new ViewPage());

            // copy model state items to the html helper
            foreach (var item in context.Controller.ViewData.ModelState)
                html.ViewData.ModelState.Add(item);

            //added support for typed views - jb
            if (((ViewDataDictionary)viewData).Model == null)
            {
                html.RenderPartial(viewName, viewData);
            }
            else
            {
                html.RenderPartial(viewName, ((ViewDataDictionary)viewData).Model, ((ViewDataDictionary)viewData));
            }

            //Restore context 
            HttpContext.Current = oldContext;

            //Flush memory and return output 
            memWriter.Flush();
            return sb.ToString();
        }

        public static string RenderViewToString(ControllerContext context,
                                            string viewName, object viewData, object Model)
        {
            //Create memory writer 
            var sb = new StringBuilder();
            var memWriter = new StringWriter(sb);

            //Create fake http context to render the view 
            var fakeResponse = new HttpResponse(memWriter);
            var fakeContext = new HttpContext(HttpContext.Current.Request, fakeResponse);
            var fakeControllerContext = new ControllerContext(
                new HttpContextWrapper(fakeContext),
                context.RouteData, context.Controller);

            var oldContext = HttpContext.Current;
            HttpContext.Current = fakeContext;

            //Use HtmlHelper to render partial view to fake context 
            var html = new HtmlHelper(new ViewContext(fakeControllerContext,
                new FakeView(), new ViewDataDictionary(), new TempDataDictionary()),
                new ViewPage());

            // copy model state items to the html helper
            foreach (var item in context.Controller.ViewData.ModelState)
                html.ViewData.ModelState.Add(item);



            html.RenderPartial(viewName, Model, ((ViewDataDictionary)viewData));

            //Restore context 
            HttpContext.Current = oldContext;

            //Flush memory and return output 
            memWriter.Flush();
            return sb.ToString();
        }

        /// <summary>Fake IView implementation, only used to instantiate an HtmlHelper.</summary> 
        public class FakeView : IView
        {
            #region IView Members
            public void Render(ViewContext viewContext, System.IO.TextWriter writer)
            {
                throw new NotImplementedException();
            }
            #endregion
        }
    }
}
