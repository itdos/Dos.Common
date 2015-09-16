using System;
using System.Reflection.Emit;
using System.IO;
using System.Reflection;

namespace EmitMapper.AST
{
    class CompilationContext
    {
        public ILGenerator ilGenerator;
        public TextWriter outputCommands;
        private int stackCount = 0;

        public CompilationContext()
        {
            outputCommands = TextWriter.Null;
			//outputCommands = Console.Out;
        }

        public CompilationContext(ILGenerator ilGenerator): this()
        {
            this.ilGenerator = ilGenerator;
        }

        public void ThrowException(Type exType)
        {
            ilGenerator.ThrowException(exType);
        }

        public void Emit(OpCode opCode)
        {
            ProcessCommand(opCode, 0, "");
            ilGenerator.Emit(opCode);
			
        }

        public void Emit(OpCode opCode, string str)
        {
            ProcessCommand(opCode, 0, str);
            ilGenerator.Emit(opCode, str);

        }

        public void Emit(OpCode opCode, int param)
        {
            ProcessCommand(opCode, 0, param.ToString());
            ilGenerator.Emit(opCode, param);
			
        }

        public void EmitNewObject(ConstructorInfo ci)
        {
            ProcessCommand(
                OpCodes.Newobj,
                ci.GetParameters().Length * -1 + 1,
                ci.ToString()
                );

            ilGenerator.Emit(OpCodes.Newobj, ci);
        }

        public void EmitCall(OpCode opCode, MethodInfo mi)
        {
            ProcessCommand(
                opCode, 
                (mi.GetParameters().Length + 1) * -1 + (mi.ReturnType == typeof(void) ? 0 : 1), 
                mi.ToString()
                );

            ilGenerator.EmitCall(opCode, mi, null);
        }

        public void Emit(OpCode opCode, FieldInfo fi)
        {
            ProcessCommand(opCode, 0, fi.ToString());
            ilGenerator.Emit(opCode, fi);
        }

        public void Emit(OpCode opCode, ConstructorInfo ci)
        {
            ProcessCommand(opCode, 0, ci.ToString());
            ilGenerator.Emit(opCode, ci);
        }

        public void Emit(OpCode opCode, LocalBuilder lb)
        {
            ProcessCommand(opCode, 0, lb.ToString());
            ilGenerator.Emit(opCode, lb);

        }

        public void Emit(OpCode opCode, Label lb)
        {
            ProcessCommand(opCode, 0, lb.ToString());
            ilGenerator.Emit(opCode, lb);
        }

        public void Emit(OpCode opCode, Type type)
        {
            ProcessCommand(opCode, 0, type.ToString());
            ilGenerator.Emit(opCode, type);
        }

        private void ProcessCommand(OpCode opCode, int addStack, string comment)
        {
			
            int stackChange = 
                GetStackChange(opCode.StackBehaviourPop) + 
                GetStackChange(opCode.StackBehaviourPush) +
                addStack;

            stackCount += stackChange;
            WriteOutputCommand(opCode.ToString() + " " + comment);
        }

        private int GetStackChange(StackBehaviour beh)
        {
            switch (beh)
            {
                case StackBehaviour.Pop0:
                case StackBehaviour.Push0:
                    return 0;

                case StackBehaviour.Pop1:
                case StackBehaviour.Popi:
                case StackBehaviour.Popref:
                case StackBehaviour.Varpop:
                    return -1;

                case StackBehaviour.Push1:
                case StackBehaviour.Pushi:
                case StackBehaviour.Pushref:
                case StackBehaviour.Varpush:
                    return 1;

                case StackBehaviour.Pop1_pop1:
                case StackBehaviour.Popi_pop1:
                case StackBehaviour.Popi_popi:
                case StackBehaviour.Popi_popi8:
                case StackBehaviour.Popi_popr4:
                case StackBehaviour.Popi_popr8:
                case StackBehaviour.Popref_pop1:
                case StackBehaviour.Popref_popi:
                    return -2;

                case StackBehaviour.Push1_push1:
                    return 2;

                case StackBehaviour.Popref_popi_pop1:
                case StackBehaviour.Popref_popi_popi:
                case StackBehaviour.Popref_popi_popi8:
                case StackBehaviour.Popref_popi_popr4:
                case StackBehaviour.Popref_popi_popr8:
                case StackBehaviour.Popref_popi_popref:
                    return -3;
            }
            return 0;
        }

        private void WriteOutputCommand(string command)
        {
            if (outputCommands != null)
            {
                outputCommands.WriteLine(new string('\t', stackCount >= 0 ? stackCount : 0) + command);
            }
        }

    }
}