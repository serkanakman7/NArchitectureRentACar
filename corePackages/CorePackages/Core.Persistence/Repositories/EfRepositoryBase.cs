using Core.Persistence.Dynamic;
using Core.Persistence.Paging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.Persistence.Repositories
{
    public class EfRepositoryBase<TEntity, TEntityId, TContext> : IAsyncRepository<TEntity, TEntityId>, IRepository<TEntity, TEntityId>
        where TEntity : Entity<TEntityId>
        where TContext : DbContext
    {

        protected TContext Context { get; }

        public EfRepositoryBase(TContext context)
        {
            Context = context;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await Context.AddAsync(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        public async Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities)
        {
            foreach (var entity in entities)
                entity.CreatedDate = DateTime.UtcNow;
            await Context.AddRangeAsync(entities);
            await Context.SaveChangesAsync();
            return entities;
        }

        public async Task<bool> Any(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = Query();
            if (withDeleted) queryable = queryable.IgnoreQueryFilters();
            if (!enableTracking) queryable = queryable.AsNoTracking();
            if (predicate != null) queryable = queryable.Where(predicate);
            return await queryable.AnyAsync(cancellationToken);
        }

        public async Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false)
        {
            await SetEntityAsDeletedAsync(entity, permanent);
            await Context.SaveChangesAsync();

            return entity;
        }

        public async Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities, bool permanent = false)
        {
            await SetEntityAsDeletedAsync(entities, permanent);
            await Context.SaveChangesAsync();
            return entities;
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = Query();
            if (enableTracking) queryable = queryable.AsNoTracking();
            if (include != null) queryable = include(queryable);
            if (withDeleted) queryable = queryable.IgnoreQueryFilters();
            return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<Paginate<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? order = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = Query();
            if(!enableTracking) queryable = queryable.AsNoTracking();
            if(include != null) queryable = include(queryable);
            if(withDeleted) queryable = queryable.IgnoreQueryFilters();
            if (predicate != null) queryable = queryable.Where(predicate);
            if(order != null) queryable = order(queryable);
            return await queryable.ToPaginateAsync<TEntity>(index, size, cancellationToken);
        }

        public async Task<Paginate<TEntity>> GetListByDynamicAsync(DynamicQuery dynamic, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? order = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = Query().ToDynamic<TEntity>(dynamic);
            if (!enableTracking) queryable = queryable.AsNoTracking();
            if(include != null) queryable = include(queryable);
            if (withDeleted) queryable = queryable.IgnoreQueryFilters();
            if(predicate != null) queryable = queryable.Where(predicate);
            if (order != null) queryable = order(queryable);
            return await queryable.ToPaginateAsync(index,size,cancellationToken);
        }

        public IQueryable<TEntity> Query() => Context.Set<TEntity>();

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            Context.Update(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        public async Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities)
        {
            foreach(var entity in entities)
                entity.UpdatedDate = DateTime.UtcNow;
            Context.UpdateRange(entities);
            await Context.SaveChangesAsync();
            return entities;
        }

        public TEntity? Get(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool withDeleted = false, bool enableTracking = true)
        {
            throw new NotImplementedException();
        }

        public Paginate<TEntity> GetList(Expression<Func<TEntity, bool>>? prediate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? order = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true)
        {
            throw new NotImplementedException();
        }

        public Paginate<TEntity> GetListByDynamic(DynamicQuery dynamic, Expression<Func<TEntity, bool>>? prediate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? order = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true)
        {
            throw new NotImplementedException();
        }

        public bool Any(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true)
        {
            throw new NotImplementedException();
        }

        public TEntity Add(TEntity entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            Context.AddAsync(entity);
            Context.SaveChangesAsync();
            return entity;
        }

        public ICollection<TEntity> AddRange(ICollection<TEntity> entities)
        {
            foreach(var entity in entities)
                entity.CreatedDate = DateTime.UtcNow;
            Context.AddAsync(entities);
            Context.SaveChangesAsync();
            return entities;
        }

        public TEntity Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public ICollection<TEntity> UpdateRange(ICollection<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public TEntity Delete(TEntity entity, bool permanent = false)
        {
            throw new NotImplementedException();
        }

        public ICollection<TEntity> DeleteRange(ICollection<TEntity> entities, bool permanent = false)
        {
            throw new NotImplementedException();
        }


        private async Task SetEntityAsDeletedAsync(TEntity entity, bool permanent)
        {
            if (!permanent)
            {
                CheckHasEntityHaveOneToOneRelation(entity);
                await SetEntityAsSoftDeletedAsync(entity);
            }
            else
            {
                Context.Remove(entity);
            }
        }


        private async Task SetEntityAsDeletedAsync(ICollection<TEntity> entities, bool permanent)
        {
            foreach (TEntity entity in entities)
                await SetEntityAsDeletedAsync(entity, permanent);
        }

        private void CheckHasEntityHaveOneToOneRelation(TEntity entity)
        {
            //Metadata: Varlıkla ilgili meta verileri alır.Entity Framework gibi veri erişim araçları, meta verileri kullanarak veritabanı tabloları ve varlıklar arasında eşlemeler oluşturur.
            //GetForeignKey: entity'nin foreign keylerini içeren bir koleksiyon alır.
            //x.DependentToPrincipal?.IsCollection == true veya x.PrincipalToDependent?.IsCollection == true: İlişkinin Collection tipi olup olmadığını kontrol eder.Collection tipi bire - bir ilişkiyi temsil etmez.
            //x.DependentToPrincipal?.ForeignKey.DeclaringEntityType.ClrType == GetType(): İlişkinin, varlığın kendi türüyle bire-bir olup olmadığını kontrol eder. (Bunu daha açık bir şekilde açıklamak için kodun öncesi ve sonrası incelenmesi gerekebilir.)
            bool hasEntityHaveOnetoOneRelation = Context.Entry(entity).Metadata.GetForeignKeys().All(
                x => x.DependentToPrincipal?.IsCollection == true || x.PrincipalToDependent?.IsCollection == true
                || x.DependentToPrincipal?.ForeignKey.DeclaringEntityType.ClrType == GetType()
                ) == false;

            if (hasEntityHaveOnetoOneRelation)
                throw new Exception("Entity has one-to-one relationship.Soft delete causes problems if you try to create entry again by same foreign key.");

        }

        private async Task SetEntityAsSoftDeletedAsync(IEntityTimestamps entity)
        {
            if (entity.DeletedDate.HasValue)
                return;
            entity.DeletedDate = DateTime.UtcNow;
            var navigations = Context.Entry(entity).Metadata.GetNavigations().Where(x => x is { IsOnDependent: false, ForeignKey.DeleteBehavior: DeleteBehavior.ClientCascade or DeleteBehavior.Cascade }).ToList();

            foreach (INavigation? navigation in navigations)
            {
                if (navigation.TargetEntityType.IsOwned()) continue;
                if (navigation.PropertyInfo == null) continue;

                object? navValue = navigation.PropertyInfo.GetValue(entity);

                if (navigation.IsCollection)
                {
                    if (navValue == null)
                    {
                        IQueryable query = Context.Entry(entity).Collection(navigation.PropertyInfo.Name).Query();
                        navValue = await GetRealittonLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType()).ToListAsync();

                        if (navValue == null) continue;
                    }

                    foreach (IEntityTimestamps navValueItem in (IEnumerable)navValue)
                        await SetEntityAsSoftDeletedAsync(navValueItem);
                }
                else
                {
                    if (navValue == null)
                    {
                        IQueryable query = Context.Entry(entity).Reference(navigation.PropertyInfo.Name).Query();
                        navValue = await GetRealittonLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType()).FirstOrDefaultAsync();

                        if (navValue == null) continue;
                    }
                    await SetEntityAsSoftDeletedAsync((IEntityTimestamps)navValue);
                }
            }
            Context.Update(entity);
        }

        private IQueryable<object> GetRealittonLoaderQuery(IQueryable query, Type navigationPropertyType)
        {
            Type queryProviderType = query.Provider.GetType();
            MethodInfo createQueryMethod = queryProviderType.GetMethods().First(m => m is { Name: nameof(query.Provider.CreateQuery), IsGenericMethod: true })?.MakeGenericMethod(queryProviderType)
                ?? throw new InvalidOperationException("CreateQuery<TElement> method is not found in IQueryProvider");

            var queryProviderQuery = (IQueryable<object>)createQueryMethod.Invoke(query.Provider, new object[] { query.Expression })!;

            return queryProviderQuery.Where(x => !((IEntityTimestamps)x).DeletedDate.HasValue);
        }
    }
}
