using System.Reflection;
using System.Runtime.InteropServices;

#if TRIDION2013SP1
[assembly: AssemblyTitle("XView for SDL Tridion 2013 SP1")]
#elif TRIDION2011SP1
[assembly: AssemblyTitle("XView for SDL Tridion 2011 SP1")]
#elif WEB8
[assembly: AssemblyTitle("XView for SDL Web 8")]
#else
[assembly: AssemblyTitle("XView")]
#endif

[assembly: AssemblyDescription("XView SDL TOM.NET Templating Framework")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("XView")]
[assembly: AssemblyCompany("Chimote")]
[assembly: AssemblyCopyright("Copyright © 2010-2015 Chimote")]
[assembly: ComVisible(false)]
[assembly: Guid("93f8e68d-1399-4307-a7fc-39278376fac3")]
[assembly: AssemblyVersion("1.7")]
[assembly: AssemblyFileVersion("1.7")]