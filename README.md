autofac-extensions
==================

Extensions I've found useful working with autofac

These extensions were built to lower the verbosity of registering a large
number of types that have things in common, especially interceptedBy().

The fluent API chain generally starts with 
	builder.enableInterfaceInterceptor<TInterceptor>()
and terminates with 
	.RegisterType<TType,TInterface>

Multiple types can be registered with the same interceptor enabled simply by chaining .RegisterType<>()

Subsequent .enableInterfaceInterceptor<>() calls accumulate and apply only to the RegisterType<>() calls that follow.  The natural use of this combination leads to the pattern of registering types that have the same interceptors.  

See the examples
