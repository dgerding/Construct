#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace Construct.Server.Services.Entities.Repositories
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Collections;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using Telerik.OpenAccess;
	using Construct.Server.Entities;

	
	public partial interface IRepository<TEntity>
	{
	    void Add(TEntity entity);
	    void Remove(TEntity entity);
	
	    TEntity Get(ObjectKey objectKey);
	    
	    IQueryable<TEntity> GetAll();
	
	    IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
	}
	
	public partial class Repository<TEntity> : IRepository<TEntity>
	        where TEntity : class
	{
	    protected IUnitOfWork unitOfWork;
	
	    public Repository(IUnitOfWork unitOfWork)
	    {
	        this.unitOfWork = unitOfWork;
	    }
	
	    public void Add(TEntity entity)
	    {
			this.unitOfWork.Add(entity);
	    }
	
	    public void Remove(TEntity entity)
	    {
	        this.unitOfWork.Delete(entity);
	    }
	
	    public IQueryable<TEntity> GetAll()
	    {
	        return this.unitOfWork.GetAll<TEntity>();
	    }
	
	    public TEntity Get(ObjectKey objectKey)
	    {
	        return this.unitOfWork.GetObjectByKey<TEntity>(objectKey);
	    }
	
	    public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
	    {
	        return this.unitOfWork.GetAll<TEntity>().Where(predicate);
	    }
	}
	
	public partial interface IDataTypeRepository : IRepository<DataType>
	{ 
	
	}
	
	public partial class DataTypeRepository : Repository<DataType>, IDataTypeRepository
	{
	    public DataTypeRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface IConstantRepository : IRepository<Constant>
	{ 
	
	}
	
	public partial class ConstantRepository : Repository<Constant>, IConstantRepository
	{
	    public ConstantRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface IDataTypeSourceRepository : IRepository<DataTypeSource>
	{ 
	
	}
	
	public partial class DataTypeSourceRepository : Repository<DataTypeSource>, IDataTypeSourceRepository
	{
	    public DataTypeSourceRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface IItemRepository : IRepository<Item>
	{ 
	
	}
	
	public partial class ItemRepository : Repository<Item>, IItemRepository
	{
	    public ItemRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface IPropertyRepository : IRepository<Property>
	{ 
	
	}
	
	public partial class PropertyRepository : Repository<Property>, IPropertyRepository
	{
	    public PropertyRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface IPropertyParentRepository : IRepository<PropertyParent>
	{ 
	
	}
	
	public partial class PropertyParentRepository : Repository<PropertyParent>, IPropertyParentRepository
	{
	    public PropertyParentRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface IPropertyTypeRepository : IRepository<PropertyType>
	{ 
	
	}
	
	public partial class PropertyTypeRepository : Repository<PropertyType>, IPropertyTypeRepository
	{
	    public PropertyTypeRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	
	
	public partial interface IQuestionParameterRepository : IRepository<QuestionParameter>
	{ 
	
	}
	
	public partial class QuestionParameterRepository : Repository<QuestionParameter>, IQuestionParameterRepository
	{
	    public QuestionParameterRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface ISourceRepository : IRepository<Source>
	{ 
	
	}
	
	public partial class SourceRepository : Repository<Source>, ISourceRepository
	{
	    public SourceRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface ISensorRepository : IRepository<Sensor>
	{ 
	
	}
	
	public partial class SensorRepository : Repository<Sensor>, ISensorRepository
	{
	    public SensorRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface ISensorCommandRepository : IRepository<SensorCommand>
	{ 
	
	}
	
	public partial class SensorCommandRepository : Repository<SensorCommand>, ISensorCommandRepository
	{
	    public SensorCommandRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface ISensorCommandParameterRepository : IRepository<SensorCommandParameter>
	{ 
	
	}
	
	public partial class SensorCommandParameterRepository : Repository<SensorCommandParameter>, ISensorCommandParameterRepository
	{
	    public SensorCommandParameterRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface ISensorHostRepository : IRepository<SensorHost>
	{ 
	
	}
	
	public partial class SensorHostRepository : Repository<SensorHost>, ISensorHostRepository
	{
	    public SensorHostRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface ISensorHostTypeRepository : IRepository<SensorHostType>
	{ 
	
	}
	
	public partial class SensorHostTypeRepository : Repository<SensorHostType>, ISensorHostTypeRepository
	{
	    public SensorHostTypeRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface ISensorRuntimeRepository : IRepository<SensorRuntime>
	{ 
	
	}
	
	public partial class SensorRuntimeRepository : Repository<SensorRuntime>, ISensorRuntimeRepository
	{
	    public SensorRuntimeRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface ISensorTypeSourceRepository : IRepository<SensorTypeSource>
	{ 
	
	}
	
	public partial class SensorTypeSourceRepository : Repository<SensorTypeSource>, ISensorTypeSourceRepository
	{
	    public SensorTypeSourceRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface ISessionRepository : IRepository<Session>
	{ 
	
	}
	
	public partial class SessionRepository : Repository<Session>, ISessionRepository
	{
	    public SessionRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface ISessionDesignRepository : IRepository<SessionDesign>
	{ 
	
	}
	
	public partial class SessionDesignRepository : Repository<SessionDesign>, ISessionDesignRepository
	{
	    public SessionDesignRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface ISessionDesignNodeRepository : IRepository<SessionDesignNode>
	{ 
	
	}
	
	public partial class SessionDesignNodeRepository : Repository<SessionDesignNode>, ISessionDesignNodeRepository
	{
	    public SessionDesignNodeRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface IVisualizationRepository : IRepository<Visualization>
	{ 
	
	}
	
	public partial class VisualizationRepository : Repository<Visualization>, IVisualizationRepository
	{
	    public VisualizationRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
	
	public partial interface IVisualizerRepository : IRepository<Visualizer>
	{ 
	
	}
	
	public partial class VisualizerRepository : Repository<Visualizer>, IVisualizerRepository
	{
	    public VisualizerRepository(IEntitiesModelUnitOfWork unitOfWork)
	        : base(unitOfWork)
	    {
	    }
	}
}
#pragma warning restore 1591
