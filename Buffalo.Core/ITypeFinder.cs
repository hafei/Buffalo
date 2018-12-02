using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Buffalo.Core
{
    public interface ITypeFinder
    {
        IList<Assembly> GetAssemblies();

        IEnumerable<Type> FindClassesOrType(Type assignFrom, bool onlyConcreteClasses = true);

        IEnumerable<Type> FindClassesOrType(Type assignFrom, IEnumerable<Assembly> assemblies, bool
            onlyConcreteClasses = true);


        IEnumerable<Type> FindClassesOrType<T>(bool onlyConcreteClasses = true);

        IEnumerable<Type> FindClassesOrType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);
    }
}
