using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using EmitMapper.Utils;
using System.Reflection;
using EmitMapper.AST;
using EmitMapper.AST.Nodes;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Helpers;
using EmitMapper.Mappers;
using EmitMapper.MappingConfiguration;
using EmitMapper.Conversion;

namespace EmitMapper.EmitBuilders
{
	class MappingBuilder
	{
		Type from;
		Type to;
		TypeBuilder typeBuilder;
		public List<object> storedObjects;
		IMappingConfigurator mappingConfigurator;
		ObjectMapperManager objectsMapperManager;

		public MappingBuilder(
			ObjectMapperManager objectsMapperManager,
			Type from,
			Type to,
			TypeBuilder typeBuilder,
			IMappingConfigurator mappingConfigurator
			)
		{
			this.objectsMapperManager = objectsMapperManager;
			this.from = from;
			this.to = to;
			this.typeBuilder = typeBuilder;
			this.storedObjects = new List<object>();
			this.mappingConfigurator = mappingConfigurator;
		}

		public void BuildCopyImplMethod()
		{
			if (ReflectionUtils.IsNullable(from))
			{
				from = Nullable.GetUnderlyingType(from);
			}
			if (ReflectionUtils.IsNullable(to))
			{
				to = Nullable.GetUnderlyingType(to);
			}

			MethodBuilder methodBuilder = typeBuilder.DefineMethod(
				"MapImpl",
				MethodAttributes.FamORAssem | MethodAttributes.Virtual,
				typeof(object),
				new Type[] { typeof(object), typeof(object), typeof(object) }
				);

			ILGenerator ilGen = methodBuilder.GetILGenerator();
			CompilationContext compilationContext = new CompilationContext(ilGen);

			AstComplexNode mapperAst = new AstComplexNode();
			var locFrom = ilGen.DeclareLocal(from);
			var locTo = ilGen.DeclareLocal(to);
			var locState = ilGen.DeclareLocal(typeof(object));
			LocalBuilder locException = null;

			mapperAst.nodes.Add(BuilderUtils.InitializeLocal(locFrom, 1));
			mapperAst.nodes.Add(BuilderUtils.InitializeLocal(locTo, 2));
			mapperAst.nodes.Add(BuilderUtils.InitializeLocal(locState, 3));

#if DEBUG
			locException = compilationContext.ilGenerator.DeclareLocal(typeof(Exception));
#endif
			var mappingOperations = mappingConfigurator.GetMappingOperations(from, to);
			StaticConvertersManager staticConverter = mappingConfigurator.GetStaticConvertersManager();
            mapperAst.nodes.Add(
				new MappingOperationsProcessor()
				{
					locException = locException,
					locFrom = locFrom,
					locState = locState,
					locTo = locTo,
					objectsMapperManager = objectsMapperManager,
					compilationContext = compilationContext,
					storedObjects = storedObjects,
					operations = mappingOperations,
					mappingConfigurator = mappingConfigurator,
					rootOperation = mappingConfigurator.GetRootMappingOperation(from, to),
					staticConvertersManager = staticConverter ?? StaticConvertersManager.DefaultInstance
				}.ProcessOperations()
			);
			mapperAst.nodes.Add(
				new AstReturn()
				{
					returnType = typeof(object),
					returnValue = AstBuildHelper.ReadLocalRV(locTo)
				}
			);

			mapperAst.Compile(compilationContext);
		}
	}
}
