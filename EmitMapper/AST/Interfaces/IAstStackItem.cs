using System;

namespace EmitMapper.AST.Interfaces
{
    interface IAstStackItem: IAstNode
    {
        Type itemType { get; }
    }
}