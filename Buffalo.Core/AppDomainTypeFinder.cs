using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;

namespace Buffalo.Core
{
    public class AppDomainTypeFinder : ITypeFinder
    {
        private bool loadAppDomainAssemblies = true;
        private string assemblySkipLoadingPattern = "^System|^mscorlib|^Microsoft|^AjaxControlToolkit|^Antlr3|^Autofac|^AutoMapper|^Castle|^ComponentArt|^CppCodeProvider|^DotNetOpenAuth|^EPPlus|^FluentValidation|^ImageProcessor|^itextsharp|^log4net|^MaxMind|^MbUnit|^MiniProfiler|^Mono.Math|^MongoDB|^MvcContrib|^Newtonsoft|^NHibernate|^nunit|^Org.Mentalis|^PerlRegex|^QuickGraph|^Recaptcha|^Remotion|^RestSharp|^Rhino|^Telerik|^Iesi|^TestDriven|^TestFu|^UserAgentStringLibrary|^VJSharpCodeProvider|^WebActivator|^WebDev|^WebGrease";
        private string assemblyRestrictToLoadingPattern = ".*";
        private IList<string> assemblyNames = new List<string>();

        public bool LoadAppDomainAssemblies { get => loadAppDomainAssemblies; set => loadAppDomainAssemblies = value; }
        public string AssemblySkipLoadingPattern { get => assemblySkipLoadingPattern; set => assemblySkipLoadingPattern = value; }
        public string AssemblyRestrictToLoadingPattern { get => assemblyRestrictToLoadingPattern; set => assemblyRestrictToLoadingPattern = value; }
        public IList<string> AssemblyNames { get => assemblyNames; set => assemblyNames = value; }


        #region Methods
        public IEnumerable<Type> FindClassesOrType(Type assignFrom, bool onlyConcreteClasses = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Type> FindClassesOrType(Type assignFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            var result = new List<Type>();
            try
            {
                foreach (var assembly in assemblies)
                {
                    Type[] types = null;
                    types = assembly.GetTypes();
                    if (types == null)
                    {
                        continue;
                    }

                    foreach (var type in types)
                    {
                        if (!assignFrom.IsAssignableFrom(type) && (!assignFrom.IsGenericTypeDefinition || DoesTypeImplementOpenGeneric(type,assignFrom)))
                        {
                            continue;
                        }

                        if (type.IsInterface)
                        {
                            continue;
                        }

                        if (onlyConcreteClasses)
                        {
                            if (type.IsClass && !type.IsAbstract)
                            {
                                result.Add(type);
                            }
                        }
                        else
                        {
                            result.Add(type);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return result;
        }

        public IEnumerable<Type> FindClassesOrType<T>(bool onlyConcreteClasses = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Type> FindClassesOrType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            throw new NotImplementedException();
        }

        public IList<Assembly> GetAllAssemblies()
        {
            var addedAssemblyNames = new List<string>();
            var assemblies = new List<Assembly>() { Assembly.GetExecutingAssembly() };
            if (loadAppDomainAssemblies)
            {
                AddAssemblierInAppDomain(addedAssemblyNames, assemblies);
            }
            AddConfiguredAssemblies(addedAssemblyNames, assemblies);
            return assemblies;
        }
        #endregion

        #region Utilities

        private void AddAssemblierInAppDomain(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (Matches(assembly.FullName))
                {
                    if (!addedAssemblyNames.Contains(assembly.FullName))
                    {
                        assemblies.Add(assembly);
                        addedAssemblyNames.Add(assembly.FullName);
                    }
                }
            }
        }

        private void AddConfiguredAssemblies(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (var assemblyName in AssemblyNames)
            {
                Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
                if (!addedAssemblyNames.Contains(assembly.FullName))
                {
                    assemblies.Add(assembly);
                    addedAssemblyNames.Add(assembly.FullName);
                }
            }
        }

        protected virtual bool Matches(string assemblyFulllName, string pattern)
        {
            return Regex.IsMatch(assemblyFulllName, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }


        protected virtual bool Matches(string assemblyFullName)
        {
            return !Matches(assemblyFullName, AssemblySkipLoadingPattern) &&
                   Matches(assemblyFullName, AssemblyRestrictToLoadingPattern);
        }


        protected virtual void LoadMatchingAssemblies(string directoryPath)
        {
            var loadedAssemglyName = new List<string>();
            foreach (var assembly in GetAllAssemblies())
            {
                loadedAssemglyName.Add(assembly.FullName);
            }

            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            foreach (var dllPath in Directory.GetFiles(directoryPath, "*.dll"))
            {
                try
                {
                    AssemblyLoadContext.Default.LoadFromAssemblyPath(dllPath);
                }
                catch (BadImageFormatException ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }

        protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            try
            {
                var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
                foreach (var implementInterface in type.FindInterfaces((objType, criteria) => true, null))
                {
                    if (!implementInterface.IsGenericType)
                    {
                        continue;
                    }

                    var isMatch = genericTypeDefinition.IsAssignableFrom(implementInterface.GetGenericTypeDefinition());
                    return isMatch;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion

    }
}
