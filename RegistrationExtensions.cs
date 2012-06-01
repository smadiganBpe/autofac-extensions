using System;
using System.Collections.Generic;
using System.Linq;
using Antero.Logging;
using Autofac;
using Autofac.Core;
using AutofacContrib.DynamicProxy2;

namespace Constellation.Common.Core.Extensions
{

	public static partial class RegistrationExtensions
	{
		public static BuilderContext EnableInterfaceInterceptor<TInterceptor>(this ContainerBuilder builder,
		                                                                      LifeTime lifetime =
		                                                                      	LifeTime.InstancePerDependency)
		{
			return new BuilderContext(builder, typeof (TInterceptor)) {Lifetime = lifetime};
		}

		public static BuilderContext EnableInterfaceInterceptor<TInterceptor>(this BuilderContext builderContext)
		{
			builderContext.InterceptedByTypes.Add(typeof (TInterceptor));
			return builderContext;
		}

		public static BuilderContext EnableInterfaceInterceptor(this BuilderContext builderContext, params Type[] types)
		{
			builderContext.InterceptedByTypes.AddRange(types);
			return builderContext;
		}

		public static BuilderContext And<TInterceptor>(this BuilderContext builderContext)
		{
			builderContext.InterceptedByTypes.Add(typeof (TInterceptor));
			return builderContext;
		}

		public static BuilderContext WithLifetime(this BuilderContext builderContext, LifeTime lifetime)
		{
			builderContext.Lifetime = lifetime;
			return builderContext;
		}

		public static BuilderContext InstancePerLifetimeScope(this BuilderContext builderContext)
		{
			builderContext.Lifetime = LifeTime.InstancePerLifetimeScope;
			return builderContext;
		}

		public static BuilderContext RegisterType<TClass, TInterface>(this BuilderContext context, string name = null)
		{
			var result =
				context.Builder.RegisterType<TClass>().As<TInterface>().EnableInterfaceInterceptors().InterceptedBy(
					context.InterceptedByTypes.ToArray()).OnRegistered(TraceEvent);
			switch (context.Lifetime)
			{
				case LifeTime.InstancePerDependency:
					break;
				case LifeTime.InstancePerLifetimeScope:
					result.InstancePerLifetimeScope();
					break;
				case LifeTime.Singleton:
					result.SingleInstance();
					break;
			}
			if (name != null)
			{
				result.Named<TInterface>(name);
			}
			return context;
		}

		public static BuilderContext RegisterType<TClass>(this BuilderContext context)
		{
			var result =
				context.Builder.RegisterType<TClass>().EnableInterfaceInterceptors().InterceptedBy(
					context.InterceptedByTypes.ToArray()).OnRegistered(TraceEvent);
			switch (context.Lifetime)
			{
				case LifeTime.InstancePerDependency:
					break;
				case LifeTime.InstancePerLifetimeScope:
					result.InstancePerLifetimeScope();
					break;
				case LifeTime.Singleton:
					result.SingleInstance();
					break;
			}
			return context;
		}
	}



	public static partial class RegistrationExtensions
	{
		private static void TraceEvent(ComponentRegisteredEventArgs args)
		{
			System.Diagnostics.Trace.WriteLine("<<<REGISTER>>> " + args.ComponentRegistration.ToString());
		}
	}



	public static partial class RegistrationExtensions
	{
		public static void EnableILoggable(this ContainerBuilder builder)
		{
			builder.RegisterCallback( obj =>						// on builder contstruction
				obj.Registered += (o, e) =>								// when type is registered
					e.ComponentRegistration.Activated += (sender, args) =>  // set activation handler
					{
						var loggable = args.Instance as ILoggable;
						if (loggable == null) return;
						loggable.SetLogger(args.Context.Resolve<ILogger>());
					}
			);
		}

	}

	public class BuilderContext
	{
		public List<Type> InterceptedByTypes { get; set; }
		public ContainerBuilder Builder { get; set; }
		public LifeTime Lifetime { get; set; }
		public Action<PreparingEventArgs> Preparing;
		public Action<ActivatingEventArgs<string>> Activating;
		public Action<IActivatedEventArgs<string>> Activated { get; set; }

		public BuilderContext(BuilderContext builderContext)
		{
			InterceptedByTypes = builderContext.InterceptedByTypes.ToList();
			Builder = builderContext.Builder;
		}

		public BuilderContext(ContainerBuilder builder, Type type)
		{
			Builder = builder;
			InterceptedByTypes = new List<Type> { type };
		}

		public BuilderContext(ContainerBuilder builder, params Type[] types)
		{
			Builder = builder;
			InterceptedByTypes = new List<Type>();
			InterceptedByTypes.AddRange(types);
		}

	}
	public enum LifeTime
	{
		InstancePerDependency = 0,
		InstancePerLifetimeScope,
		Singleton
	}
}
