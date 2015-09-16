using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmitMapper.Utils;
using System.Reflection;
using EmitMapper.AST.Nodes;
using EmitMapper.AST;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Helpers;
using System.Reflection.Emit;

namespace EmitMapper.EmitInvoker
{
    public static class MethodInvoker
    {
        private static ThreadSaveCache _typesCache = new ThreadSaveCache();

        public static MethodInvokerBase GetMethodInvoker(object targetObject, MethodInfo mi)
        {
            var typeName = "EmitMapper.MethodCaller_" + mi.ToString();

            Type callerType = _typesCache.Get<Type>(
                typeName,
                () =>
                {
                    if (mi.ReturnType == typeof(void))
                    {
                        return BuildActionCallerType(typeName, mi);
                    }
                    else
                    {
                        return BuildFuncCallerType(typeName, mi);
                    }
                }
            );

            MethodInvokerBase result = (MethodInvokerBase)Activator.CreateInstance(callerType);
            result.targetObject = targetObject;
            return result;
        }


        private static Type BuildFuncCallerType(string typeName, MethodInfo mi)
        {
            var par = mi.GetParameters();
            Type funcCallerType = null;
            if (par.Length == 0)
            {
                funcCallerType = typeof(MethodInvokerFunc_0);
            }
            if (par.Length == 1)
            {
                funcCallerType = typeof(MethodInvokerFunc_1);
            }
            if (par.Length == 2)
            {
                funcCallerType = typeof(MethodInvokerFunc_2);
            }
            if (par.Length == 3)
            {
                funcCallerType = typeof(MethodInvokerFunc_3);
            }
            else
            {
                new EmitMapperException("too many method parameters");
            }

            var tb = DynamicAssemblyManager.DefineType(typeName, funcCallerType);

            MethodBuilder methodBuilder = tb.DefineMethod(
                "CallFunc",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(object),
                Enumerable.Range(0, par.Length).Select(i => typeof(object)).ToArray()
            );

            new AstReturn
            {
                returnType = typeof(object),
                returnValue = CreateCallMethod(mi, par)
            }.Compile(new CompilationContext(methodBuilder.GetILGenerator()));

            return tb.CreateType();
        }

        private static Type BuildActionCallerType(string typeName, MethodInfo mi)
        {
            var par = mi.GetParameters();
            Type actionCallerType = null;
            if (par.Length == 0)
            {
                actionCallerType = typeof(MethodInvokerAction_0);
            }
            if (par.Length == 1)
            {
                actionCallerType = typeof(MethodInvokerAction_1);
            }
            if (par.Length == 2)
            {
                actionCallerType = typeof(MethodInvokerAction_2);
            }
            if (par.Length == 3)
            {
                actionCallerType = typeof(MethodInvokerAction_3);
            }
            else
            {
                new EmitMapperException("too many method parameters");
            }

            var tb = DynamicAssemblyManager.DefineType(typeName, actionCallerType);

            MethodBuilder methodBuilder = tb.DefineMethod(
                "CallAction",
                MethodAttributes.Public | MethodAttributes.Virtual,
                null,
                Enumerable.Range(0, par.Length).Select(i => typeof(object)).ToArray()
            );

            new AstComplexNode
            {
                nodes = new List<IAstNode>
                {
                    CreateCallMethod(mi, par),
                    new AstReturnVoid()
                }
            }.Compile(new CompilationContext(methodBuilder.GetILGenerator()));

            return tb.CreateType();
        }

        private static IAstRefOrValue CreateCallMethod(MethodInfo mi, ParameterInfo[] parameters)
        {
            return
                AstBuildHelper.CallMethod(
                    mi,
                    mi.IsStatic ? null : 
                        new AstCastclassRef(
                            AstBuildHelper.ReadFieldRV(
                                new AstReadThis() { thisType = typeof(MethodInvokerBase) },
                                typeof(MethodInvokerBase).GetField("targetObject", BindingFlags.NonPublic | BindingFlags.Instance)
                            ),
                            mi.DeclaringType
                        ),
                    parameters.Select((p, idx) => (IAstStackItem)AstBuildHelper.ReadArgumentRV(idx + 1, typeof(object))).ToList()
                );
        }
    }
}
