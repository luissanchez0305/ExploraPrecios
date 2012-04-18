using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using Castle.Windsor;
using Explora_Precios.Core;
using Explora_Precios.Data.NHibernateMaps;
using Explora_Precios.Web.Controllers;
using Explora_Precios.Web.CastleWindsor;
using MvcContrib.Castle;
using SharpArch.Core.DomainModel;
using SharpArch.Data.NHibernate;
using SharpArch.Web.NHibernate;
using SharpArch.Web.Castle;
using Microsoft.Practices.ServiceLocation;
using CommonServiceLocator.WindsorAdapter;
using SharpArch.Web.Areas;
using SharpArch.Web.CommonValidator;
using SharpArch.Web.ModelBinder;

namespace Explora_Precios.Web
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : HttpApplication
	{
		protected void Application_Start() {
			log4net.Config.XmlConfigurator.Configure();

			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new AreaViewEngine());

			ModelBinders.Binders.DefaultBinder = new SharpModelBinder();

			InitializeServiceLocator();

			RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);
		}

		/// <summary>
		/// Instantiate the container and add all Controllers that derive from 
		/// WindsorController to the container.  Also associate the Controller 
		/// with the WindsorContainer ControllerFactory.
		/// </summary>
		protected virtual void InitializeServiceLocator() {
			IWindsorContainer container = new WindsorContainer();
			ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));

			container.RegisterControllers(typeof(HomeController).Assembly);
			ComponentRegistrar.AddComponentsTo(container);

			ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));
		}

		public override void Init() {
			base.Init();

			// The WebSessionStorage must be created during the Init() to tie in HttpApplication events
			webSessionStorage = new WebSessionStorage(this);
		}

		/// <summary>
		/// Due to issues on IIS7, the NHibernate initialization cannot reside in Init() but
		/// must only be called once.  Consequently, we invoke a thread-safe singleton class to 
		/// ensure it's only initialized once.
		/// </summary>
		protected void Application_BeginRequest(object sender, EventArgs e) {
			NHibernateInitializer.Instance().InitializeNHibernateOnce(
				() => InitializeNHibernateSession());
		}

		/// <summary>
		/// If you need to communicate to multiple databases, you'd add a line to this method to
		/// initialize the other database as well.
		/// </summary>
		private void InitializeNHibernateSession()
		{
			try { NHibernateSession.Reset(); }
			catch { }
			NHibernateSession.Init(
				webSessionStorage,
				new string[] { Server.MapPath("~/bin/Explora_Precios.Data.dll") },
				new AutoPersistenceModelGenerator().Generate());
		}

		protected void Application_Error(object sender, EventArgs e) {
			// Useful for debugging
			Exception ex = Server.GetLastError();
			ReflectionTypeLoadException reflectionTypeLoadException = ex as ReflectionTypeLoadException;
			bool goOn = true;
			while (goOn && !ex.Source.Contains("Explora_Precios"))
			{
				if (ex.InnerException != null)
					ex = ex.InnerException;
				else
				{
					goOn = false;
				}
			}

			if (goOn)
			{
				try
				{
					var email = new Explora_Precios.ApplicationServices.EmailServices("info@exploraprecios.com",
						"(Global) Error en " + System.Configuration.ConfigurationManager.AppSettings["Enviroment"] + " - " + ex.Source,
						"Detalle: " + ex.StackTrace);
					email.Send();
				}
				catch { }
			}
		}

		private WebSessionStorage webSessionStorage;
	}
}
