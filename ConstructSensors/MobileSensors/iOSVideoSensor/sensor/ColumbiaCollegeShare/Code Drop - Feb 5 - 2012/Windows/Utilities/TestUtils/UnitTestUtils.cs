using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestUtilities
{
    public static class UnitTestUtils
    {
        public static Type[] GetTestClasses(Assembly assembly)
        {
            List<Type> testClasses = new List<Type>();
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass == true)
                {
                    object[] attributes = type.GetCustomAttributes(true);
                    foreach (object attribute in attributes)
                    {
                        TestClassAttribute testClassAtrribute = attribute as TestClassAttribute;
                        if (testClassAtrribute != null)
                        {
                            testClasses.Add(type);
                        }
                    }
                }
            }
            return testClasses.ToArray();
        }

        public static MethodInfo[] GetTestMethods(Type testClass)
        {
            return GetMethodsWithAtrributeType(testClass, typeof(TestMethodAttribute));
        }

        public static MethodInfo GetTestSetupMethod(Type testClass)
        {
            MethodInfo[] initMethods = GetMethodsWithAtrributeType(testClass, typeof(TestInitializeAttribute));
            if (initMethods == null || initMethods.Length == 0)
            {
                return null;
            }
            return initMethods[0];
        }

        public static MethodInfo[] GetMethodsWithAtrributeType(Type testClass, Type attributeType)
        {
            List<MethodInfo> testMethods = new List<MethodInfo>();
            MethodInfo[] methods = testClass.GetMethods();
            foreach (MethodInfo method in methods)
            {
                object[] attributes = method.GetCustomAttributes(true);
                foreach (object attribute in attributes)
                {
                    if (attribute.GetType().Name == attributeType.Name)
                    {
                        testMethods.Add(method);
                        break;
                    }
                }
            }
            return testMethods.ToArray();
        }


    }
}
