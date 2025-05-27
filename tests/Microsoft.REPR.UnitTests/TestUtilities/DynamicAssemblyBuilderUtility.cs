using System.Reflection;
using System.Reflection.Emit;

namespace Microsoft.REPR.UnitTests.TestUtilities;

internal static class DynamicAssemblyBuilderUtility
{
    public static Assembly CreateTestAssembly(string assemblyName = "TestAssembly")
    {
        var assembly = new AssemblyName(assemblyName);
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assembly, AssemblyBuilderAccess.RunAndCollect);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

        var typeBuilder = moduleBuilder.DefineType("TestType", TypeAttributes.Public);
        var methodBuilder = typeBuilder.DefineMethod("SayHello",
            MethodAttributes.Public | MethodAttributes.Static,
            typeof(void),
            Type.EmptyTypes);

        var il = methodBuilder.GetILGenerator();
        il.EmitWriteLine("Hello from dynamic method!");
        il.Emit(OpCodes.Ret);

        typeBuilder.CreateType();
        AppDomain.CurrentDomain.Load(assemblyBuilder.GetName());
        return assemblyBuilder;
    }

}
