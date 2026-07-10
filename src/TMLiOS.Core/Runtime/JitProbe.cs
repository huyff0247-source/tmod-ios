using System.Linq.Expressions;
using System.Reflection.Emit;

namespace TMLiOS.Core.Runtime;

public static class JitProbe
{
    public static IReadOnlyList<ProbeResult> RunAll()
    {
        var results = new List<ProbeResult>();
        results.Add(TestExpressionCompile());
        results.Add(TestDynamicMethod());
        return results;
    }

    private static ProbeResult TestExpressionCompile()
    {
        try
        {
            var input = Expression.Parameter(typeof(int), "x");
            var body = Expression.Add(input, Expression.Constant(41));
            var lambda = Expression.Lambda<Func<int, int>>(body, input);
            var compiled = lambda.Compile();
            var value = compiled(1);
            return new ProbeResult("Expression.Compile", value == 42, $"returned {value}");
        }
        catch (Exception ex)
        {
            return new ProbeResult("Expression.Compile", false, ex.GetType().Name + ": " + ex.Message);
        }
    }

    private static ProbeResult TestDynamicMethod()
    {
        try
        {
            var method = new DynamicMethod("Return42", typeof(int), Type.EmptyTypes);
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldc_I4, 42);
            il.Emit(OpCodes.Ret);
            var func = (Func<int>)method.CreateDelegate(typeof(Func<int>));
            var value = func();
            return new ProbeResult("Reflection.Emit.DynamicMethod", value == 42, $"returned {value}");
        }
        catch (Exception ex)
        {
            return new ProbeResult("Reflection.Emit.DynamicMethod", false, ex.GetType().Name + ": " + ex.Message);
        }
    }
}
