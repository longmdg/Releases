namespace SparkTech.Helpers
{
    using System;
    using System.Reflection.Emit;

    using static System.String;

    internal static class Initializer
    {
        internal static TType CreateInstance<TType>(Type type) where TType : class
        {
            var target = type?.GetConstructor(Type.EmptyTypes);
            var declaring = target?.DeclaringType;

            if (declaring == null || !type.IsAssignableFrom(typeof(TType)))
            {
                return null;
            }

            var dynamic = new DynamicMethod(Empty, type, new Type[0], declaring);

            var generator = dynamic.GetILGenerator();

            generator.DeclareLocal(declaring);
            generator.Emit(OpCodes.Newobj, target);
            generator.Emit(OpCodes.Stloc_0);
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ret);

            return (dynamic.CreateDelegate(typeof(Func<TType>)) as Func<TType>)?.Invoke();
        }
    }
}