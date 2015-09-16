using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using EmitMapper.AST;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.AST.Helpers;
using EmitMapper.Utils;
using EmitMapper.Mappers;
using System.Reflection;
using EmitMapper.EmitInvoker;
using EmitMapper.MappingConfiguration;

namespace EmitMapper.EmitBuilders
{
	class MappingOperationsProcessor
	{
		public LocalBuilder locFrom;
		public LocalBuilder locTo;
		public LocalBuilder locState;
		public LocalBuilder locException;
		public CompilationContext compilationContext;
		public IEnumerable<IMappingOperation> operations = new List<IMappingOperation>();
		public List<object> storedObjects = new List<object>();
		public IMappingConfigurator mappingConfigurator;
		public ObjectMapperManager objectsMapperManager;
		public IRootMappingOperation rootOperation;
		public StaticConvertersManager staticConvertersManager;

		public MappingOperationsProcessor()
		{ 
		}

		public MappingOperationsProcessor(MappingOperationsProcessor prototype)
		{
			locFrom = prototype.locFrom;
			locTo = prototype.locTo;
			locState = prototype.locState;
			locException = prototype.locException;
			compilationContext = prototype.compilationContext;
			operations = prototype.operations;
			storedObjects = prototype.storedObjects;
			mappingConfigurator = prototype.mappingConfigurator;
			objectsMapperManager = prototype.objectsMapperManager;
			rootOperation = prototype.rootOperation;
			staticConvertersManager = prototype.staticConvertersManager;
		}

		public IAstNode ProcessOperations()
		{
			var result = new AstComplexNode();
			foreach (var operation in operations)
			{
				IAstNode completeOperation = null;
				int operationId = AddObjectToStore(operation);

				if (operation is OperationsBlock)
				{
					completeOperation = 
						new MappingOperationsProcessor(this) 
						{ 
							operations = (operation as OperationsBlock).Operations 
						}.ProcessOperations();
				}
                else if (operation is ReadWriteComplex)
                {
                    completeOperation = Process_ReadWriteComplex(operation as ReadWriteComplex, operationId);
                }
                else if (operation is DestSrcReadOperation)
                {
                    completeOperation = ProcessDestSrcReadOperation(operation as DestSrcReadOperation, operationId);
                }
				else if (operation is SrcReadOperation)
				{
					completeOperation = ProcessSrcReadOperation(operation as SrcReadOperation, operationId);
				}
				else if (operation is DestWriteOperation)
				{
					completeOperation = ProcessDestWriteOperation(operation as DestWriteOperation, operationId);
				}
				else if (operation is ReadWriteSimple)
				{
					completeOperation = ProcessReadWriteSimple(operation as ReadWriteSimple, operationId);
				}

				if (completeOperation == null)
				{
					continue;
				}
				if (locException != null)
				{
					var tryCatch = CreateExceptionHandlingBlock(operationId, completeOperation);
					result.nodes.Add(tryCatch);
				}
				else
				{
					result.nodes.Add(completeOperation);
				}
			}
			return result;
		}

		private IAstNode ProcessReadWriteSimple(ReadWriteSimple readWriteSimple, int operationId)
		{
			IAstRefOrValue sourceValue = ReadSrcMappingValue(readWriteSimple, operationId);

            IAstRefOrValue convertedValue;

            if (readWriteSimple.NullSubstitutor != null && (ReflectionUtils.IsNullable(readWriteSimple.Source.MemberType) || !readWriteSimple.Source.MemberType.IsValueType))
            {
                convertedValue = new AstIfTernar(
                    ReflectionUtils.IsNullable(readWriteSimple.Source.MemberType)
                        ? (IAstValue)new AstExprNot(AstBuildHelper.ReadPropertyRV(new AstValueToAddr((IAstValue)sourceValue), readWriteSimple.Source.MemberType.GetProperty("HasValue")))
                        : new AstExprIsNull(sourceValue),
                        GetNullValue(readWriteSimple.NullSubstitutor), // source is null
                        AstBuildHelper.CastClass(
                            ConvertMappingValue(
                                readWriteSimple,
                                operationId,
                                sourceValue
                            ),
                            readWriteSimple.Destination.MemberType
                        )
                );
            }
            else
            {
                convertedValue =
                    ConvertMappingValue(
                        readWriteSimple,
                        operationId,
                        sourceValue
                    );
            }

			return WriteMappingValue(readWriteSimple, operationId, convertedValue);
		}

		private IAstNode ProcessDestWriteOperation(DestWriteOperation destWriteOperation, int operationId)
		{
			LocalBuilder locValueToWrite = null;
			locValueToWrite = this.compilationContext.ilGenerator.DeclareLocal(destWriteOperation.Getter.Method.ReturnType);

			var cmdValue = new AstWriteLocal(
				locValueToWrite,
				AstBuildHelper.CallMethod(
					destWriteOperation.Getter.GetType().GetMethod("Invoke"),
                    new AstCastclassRef(
					    (IAstRef)AstBuildHelper.ReadMemberRV(
						    GetStoredObject(operationId, typeof(DestWriteOperation)),
						    typeof(DestWriteOperation).GetProperty("Getter")
					    ),
                        destWriteOperation.Getter.GetType()
                    ),
                    new List<IAstStackItem>
					{
						AstBuildHelper.ReadLocalRV(locFrom),
						AstBuildHelper.ReadLocalRV(locState)
					}
				)
			);

			return
				new AstComplexNode
				{
					nodes = new List<IAstNode> 
					{ 
						cmdValue,
						new AstIf()
						{
							condition = new AstExprEquals(
								(IAstValue)AstBuildHelper.ReadMembersChain(
									AstBuildHelper.ReadLocalRA(locValueToWrite),
									new[] { (MemberInfo)locValueToWrite.LocalType.GetField("action") }
								),
								new AstConstantInt32() { value = 0 }
							),
							trueBranch = new AstComplexNode
							{
								nodes = new List<IAstNode>
								{
									 AstBuildHelper.WriteMembersChain(
										destWriteOperation.Destination.MembersChain,
										AstBuildHelper.ReadLocalRA(locTo),
										AstBuildHelper.ReadMembersChain(
											AstBuildHelper.ReadLocalRA(locValueToWrite),
											new[] { (MemberInfo)locValueToWrite.LocalType.GetField("value") }
										)
									)
								}
							}
						}
					}
				};
		}

		private IAstNode ProcessSrcReadOperation(SrcReadOperation srcReadOperation, int operationId)
		{
			var value = AstBuildHelper.ReadMembersChain(
				AstBuildHelper.ReadLocalRA(locFrom),
				srcReadOperation.Source.MembersChain
			);

			return WriteMappingValue(srcReadOperation, operationId, value);
		}

        private IAstNode Process_ReadWriteComplex(ReadWriteComplex op, int operationId)
        {
			if (op.Converter != null)
			{
				return
					AstBuildHelper.WriteMembersChain(
						op.Destination.MembersChain,
						AstBuildHelper.ReadLocalRA(locTo),
						AstBuildHelper.CallMethod(
							op.Converter.GetType().GetMethod("Invoke"),
                            new AstCastclassRef(
							    (IAstRef)AstBuildHelper.ReadMemberRV(
								     GetStoredObject(operationId, typeof(ReadWriteComplex)),
								     typeof(ReadWriteComplex).GetProperty("Converter")
							    ),
                                op.Converter.GetType()
                            ),
							new List<IAstStackItem>()
							{
								AstBuildHelper.ReadMembersChain(
									AstBuildHelper.ReadLocalRA(locFrom),
									op.Source.MembersChain
								),
								AstBuildHelper.ReadLocalRV(locState),
							}
						)
					);
			}

			var result = new AstComplexNode();
            LocalBuilder origTempSrc, origTempDst;
            LocalBuilder tempSrc = compilationContext.ilGenerator.DeclareLocal(op.Source.MemberType);
            LocalBuilder tempDst = compilationContext.ilGenerator.DeclareLocal(op.Destination.MemberType);
            origTempSrc = tempSrc;
            origTempDst = tempDst;

            result.nodes.Add(
                new AstWriteLocal(tempSrc, AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(locFrom), op.Source.MembersChain )
                )
            );
            result.nodes.Add(
                new AstWriteLocal(tempDst, AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(locTo), op.Destination.MembersChain))
            );

            var writeNullToDest =
                new List<IAstNode>
                {
                    AstBuildHelper.WriteMembersChain(
                        op.Destination.MembersChain,
                        AstBuildHelper.ReadLocalRA(locTo),
                        GetNullValue(op.NullSubstitutor)
                    )
                };

            // Target construction
			var initDest = new List<IAstNode>();
			var custCtr = op.TargetConstructor;
			if (custCtr != null)
			{
				int custCtrIdx = AddObjectToStore(custCtr);
				initDest.Add(
					new AstWriteLocal(
						tempDst,
						AstBuildHelper.CallMethod(
                            custCtr.GetType().GetMethod("Invoke"),
                            GetStoredObject(custCtrIdx, custCtr.GetType()),
							null
						)
					)
				);
			}
			else
			{
				initDest.Add(
					new AstWriteLocal(tempDst, new AstNewObject(op.Destination.MemberType, null))
				);
			}

			var copying = new List<IAstNode>();

            // if destination is nullable, create a temp target variable with underlying destination type
			if (ReflectionUtils.IsNullable(op.Source.MemberType))
			{
				tempSrc = compilationContext.ilGenerator.DeclareLocal(Nullable.GetUnderlyingType(op.Source.MemberType));
				copying.Add(
					new AstWriteLocal(
						tempSrc,
						AstBuildHelper.ReadPropertyRV(
							AstBuildHelper.ReadLocalRA(origTempSrc),
							op.Source.MemberType.GetProperty("Value")
						)
					)
				);
			}

            // If destination is null, initialize it.
            if (ReflectionUtils.IsNullable(op.Destination.MemberType) || !op.Destination.MemberType.IsValueType)
            {
                copying.Add(
                    new AstIf()
                    {
                        condition = ReflectionUtils.IsNullable(op.Destination.MemberType)
							? (IAstValue) new AstExprNot((IAstValue)AstBuildHelper.ReadPropertyRV(AstBuildHelper.ReadLocalRA(origTempDst), op.Destination.MemberType.GetProperty("HasValue")))
							: new AstExprIsNull(AstBuildHelper.ReadLocalRV(origTempDst)),
                        trueBranch = new AstComplexNode() { nodes = initDest }
                    }
                );
				if (ReflectionUtils.IsNullable(op.Destination.MemberType))
				{
					tempDst = compilationContext.ilGenerator.DeclareLocal(Nullable.GetUnderlyingType(op.Destination.MemberType));
					copying.Add(
						new AstWriteLocal(
							tempDst, 
							AstBuildHelper.ReadPropertyRV(
								AstBuildHelper.ReadLocalRA(origTempDst),
								op.Destination.MemberType.GetProperty("Value")
							)
						)
					);
				}
            }

            // Suboperations
            copying.Add(
                new AstComplexNode()
                {
                    nodes = new List<IAstNode> 
					{ 
						new MappingOperationsProcessor(this) 
						{ 
							operations = op.Operations, 
							locTo = tempDst, 
							locFrom = tempSrc,
							rootOperation = mappingConfigurator.GetRootMappingOperation(op.Source.MemberType, op.Destination.MemberType)
						}.ProcessOperations() 
					}
                }
            );

            IAstRefOrValue processedValue;
            if (ReflectionUtils.IsNullable(op.Destination.MemberType))
            {
                processedValue =
                    new AstNewObject(
                        op.Destination.MemberType,
                        new[] 
                        {
                            AstBuildHelper.ReadLocalRV(tempDst)
                        }
                    );
            }
            else
            {
                processedValue = AstBuildHelper.ReadLocalRV(origTempDst);
            }

            if (op.ValuesPostProcessor != null)
            {
                int postProcessorId = AddObjectToStore(op.ValuesPostProcessor);
                processedValue =
                    AstBuildHelper.CallMethod(
                        op.ValuesPostProcessor.GetType().GetMethod("Invoke"),
                        GetStoredObject(postProcessorId, op.ValuesPostProcessor.GetType()),
                        new List<IAstStackItem>
                        {
                            processedValue,
                            AstBuildHelper.ReadLocalRV(locState)
                        }
                    );
            }

            copying.Add(
                AstBuildHelper.WriteMembersChain(
                    op.Destination.MembersChain,
                    AstBuildHelper.ReadLocalRA(locTo),
                    processedValue
                )
            );

            if (ReflectionUtils.IsNullable(op.Source.MemberType) || !op.Source.MemberType.IsValueType)
            {
                result.nodes.Add(
                    new AstIf()
                    {
                        condition = ReflectionUtils.IsNullable(op.Source.MemberType)
                            ? (IAstValue) new AstExprNot((IAstValue)AstBuildHelper.ReadPropertyRV(AstBuildHelper.ReadLocalRA(origTempSrc), op.Source.MemberType.GetProperty("HasValue")))
							: new AstExprIsNull(AstBuildHelper.ReadLocalRV(origTempSrc)),
						trueBranch = new AstComplexNode() { nodes = writeNullToDest },
						falseBranch = new AstComplexNode() { nodes = copying }
                    }
                );
            }
            else
            {
                result.nodes.AddRange(copying);
            }

            return result;
        }

        private IAstRefOrValue GetNullValue(Delegate nullSubstitutor)
		{
			if (nullSubstitutor != null)
			{
                var substId = AddObjectToStore(nullSubstitutor);
				return
					AstBuildHelper.CallMethod(
                        nullSubstitutor.GetType().GetMethod("Invoke"),
                        GetStoredObject(substId, nullSubstitutor.GetType()),
						new List<IAstStackItem>
						{
							AstBuildHelper.ReadLocalRV(locState)
						}
					);
			}
			else
			{
				return new AstConstantNull();
			}
		}
		private int AddObjectToStore(object obj)
		{
			int objectId = storedObjects.Count();
			storedObjects.Add(obj);
			return objectId;
		}

		private IAstNode CreateExceptionHandlingBlock(int mappingItemId, IAstNode writeValue)
		{
			var handler =
				new AstThrow
				{
					exception =
						new AstNewObject
						{
							objectType = typeof(EmitMapperException),
							ConstructorParams =
								new IAstStackItem[]
								{
									new AstConstantString()
									{
										str = "Error in mapping operation execution: "
									},
									new AstReadLocalRef()
									{
										localIndex = locException.LocalIndex,
										localType = locException.LocalType
									},
									GetStoredObject(mappingItemId, typeof(IMappingOperation))
								}
						},
				};

			var tryCatch = new AstExceptionHandlingBlock(
				writeValue,
				handler,
				typeof(Exception),
				locException
				);
			return tryCatch;
		}

		private IAstNode WriteMappingValue(
			IMappingOperation mappingOperation,
			int mappingItemId,
			IAstRefOrValue value)
		{
			IAstNode writeValue;

			if (mappingOperation is SrcReadOperation)
			{
				writeValue = AstBuildHelper.CallMethod(
					typeof(ValueSetter).GetMethod("Invoke"),
                    new AstCastclassRef(
					    (IAstRef)AstBuildHelper.ReadMemberRV(
					         GetStoredObject(mappingItemId, typeof(SrcReadOperation)),
					         typeof(SrcReadOperation).GetProperty("Setter")
					     ),
                         (mappingOperation as SrcReadOperation).Setter.GetType()
                    ),
					new List<IAstStackItem>()
                        {
							AstBuildHelper.ReadLocalRV(locTo),
                            value,
							AstBuildHelper.ReadLocalRV(locState),
                        }
					);
			}
			else
			{
				writeValue = AstBuildHelper.WriteMembersChain(
					(mappingOperation as IDestOperation).Destination.MembersChain,
					AstBuildHelper.ReadLocalRA(locTo),
					value
				);
			}
			return writeValue;
		}

		private IAstRefOrValue ConvertMappingValue(
			ReadWriteSimple rwMapOp,
			int operationId,
			IAstRefOrValue sourceValue)
		{
			IAstRefOrValue convertedValue = sourceValue;
			if (rwMapOp.Converter != null)
			{
				convertedValue = AstBuildHelper.CallMethod(
					rwMapOp.Converter.GetType().GetMethod("Invoke"),
                    new AstCastclassRef(
					    (IAstRef)AstBuildHelper.ReadMemberRV(
					         GetStoredObject(operationId, typeof(ReadWriteSimple)),
					         typeof(ReadWriteSimple).GetProperty("Converter")
					    ),
                        rwMapOp.Converter.GetType()
                    ),
					new List<IAstStackItem>()
                    {
                        sourceValue,
						AstBuildHelper.ReadLocalRV(locState),
                    }
				);
			}
			else
			{
				if (rwMapOp.ShallowCopy && rwMapOp.Destination.MemberType == rwMapOp.Source.MemberType)
				{
					convertedValue = sourceValue;
				}
				else
				{
					var mi = staticConvertersManager.GetStaticConverter(rwMapOp.Source.MemberType, rwMapOp.Destination.MemberType);
					if (mi != null)
					{
						convertedValue = AstBuildHelper.CallMethod(
							mi,
							null,
							new List<IAstStackItem> { sourceValue }
						);
					}
					else
					{
						convertedValue = ConvertByMapper(rwMapOp);
					}
				}
			}

			return convertedValue;
		}

		private IAstRefOrValue ConvertByMapper(ReadWriteSimple mapping)
		{
			IAstRefOrValue convertedValue;
			ObjectsMapperDescr mapper = objectsMapperManager.GetMapperInt(
				mapping.Source.MemberType,
				mapping.Destination.MemberType,
				mappingConfigurator);
			int mapperId = AddObjectToStore(mapper);

			convertedValue = AstBuildHelper.CallMethod(
				typeof(ObjectsMapperBaseImpl).GetMethod(
					"Map",
					new Type[] { typeof(object), typeof(object), typeof(object) }
					),

				new AstReadFieldRef
				{
					fieldInfo = typeof(ObjectsMapperDescr).GetField("mapper"),
                    sourceObject = GetStoredObject(mapperId, mapper.GetType())
				},

				new List<IAstStackItem>()
                    {
						AstBuildHelper.ReadMembersChain(
							AstBuildHelper.ReadLocalRA(locFrom),
							mapping.Source.MembersChain
						),
						AstBuildHelper.ReadMembersChain(
							AstBuildHelper.ReadLocalRA(locTo),
							mapping.Destination.MembersChain
						),
						(IAstRef)AstBuildHelper.ReadLocalRA(locState)
                    }
				);
			return convertedValue;
		}

		private IAstNode ProcessDestSrcReadOperation(
			DestSrcReadOperation operation,
			int operationId)
		{
			IAstRefOrValue src =
				AstBuildHelper.ReadMembersChain(
					AstBuildHelper.ReadLocalRA(locFrom),
					operation.Source.MembersChain
				);

			IAstRefOrValue dst =
				AstBuildHelper.ReadMembersChain(
					AstBuildHelper.ReadLocalRA(locFrom),
					operation.Destination.MembersChain
				);

			return AstBuildHelper.CallMethod(
				typeof(ValueProcessor).GetMethod("Invoke"),
                new AstCastclassRef(
				    (IAstRef)AstBuildHelper.ReadMemberRV(
					    GetStoredObject(operationId, typeof(DestSrcReadOperation)),
					    typeof(DestWriteOperation).GetProperty("ValueProcessor")
				    ),
                    operation.ValueProcessor.GetType()
                ),
				new List<IAstStackItem> { src, dst, AstBuildHelper.ReadLocalRV(locState) }
			);
		}

		private IAstRefOrValue ReadSrcMappingValue(
			IMappingOperation mapping,
			int operationId)
		{
			var readOp = mapping as ISrcReadOperation;
			if (readOp != null)
			{
				return AstBuildHelper.ReadMembersChain(
					AstBuildHelper.ReadLocalRA(locFrom),
					readOp.Source.MembersChain
				);
			}

			var destWriteOp = (DestWriteOperation)mapping;
			if (destWriteOp.Getter != null)
			{
				return AstBuildHelper.CallMethod(
					destWriteOp.Getter.GetType().GetMethod("Invoke"),
                    new AstCastclassRef(
					    (IAstRef)AstBuildHelper.ReadMemberRV(
						    GetStoredObject(operationId, typeof(DestWriteOperation)),
						    typeof(DestWriteOperation).GetProperty("Getter")
					    ),
                        destWriteOp.Getter.GetType()
                    ),
					new List<IAstStackItem>
					{
						AstBuildHelper.ReadLocalRV(locState)
					}
				);
			}
			throw new EmitMapperException("Invalid mapping operations");
		}

		private static IAstRef GetStoredObject(int objectIndex, Type castType)
		{
			var result = (IAstRef)AstBuildHelper.ReadArrayItemRV(
				(IAstRef)AstBuildHelper.ReadFieldRA(
					new AstReadThis() { thisType = typeof(ObjectsMapperBaseImpl) },
					typeof(ObjectsMapperBaseImpl).GetField(
						"StroredObjects",
						BindingFlags.Instance | BindingFlags.Public
					)
				),
				objectIndex
			);
			if (castType != null)
			{
				result = new AstCastclassRef(result, castType);
			}
			return result;
		}
	}
}
