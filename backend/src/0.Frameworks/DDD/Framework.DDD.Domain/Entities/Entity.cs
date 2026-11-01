using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Framework.DDD.Domain.ValueObjects;

namespace Framework.DDD.Domain.Entities;

/// <summary>
/// Base implementation for entities with a generic single-value primary key.
/// Provides equality, hash code, and EF Core proxy-aware type comparison.
/// </summary>
public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>, IEquatable<Entity<TPrimaryKey>>
    where TPrimaryKey : struct
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public virtual TPrimaryKey Id { get; set; }

    protected Entity() { }

    protected Entity(TPrimaryKey id)
    {
        if (Equals(id, default(TPrimaryKey)))
            throw new ArgumentException("The Id cannot be the default value.", nameof(id));
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TPrimaryKey> other) return false;
        if (ReferenceEquals(this, other)) return true;
        if (ValueObject.GetUnproxiedType(this) != ValueObject.GetUnproxiedType(other)) return false;
        if (IsTransient() || other.IsTransient()) return false;
        return Id.Equals(other.Id);
    }

    public bool Equals(Entity<TPrimaryKey>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (ValueObject.GetUnproxiedType(this) != ValueObject.GetUnproxiedType(other)) return false;
        if (IsTransient() || other.IsTransient()) return false;
        return Id.Equals(other.Id);
    }

    public bool IsTransient() => EqualityComparer<TPrimaryKey>.Default.Equals(Id, default);

    public static bool operator ==(Entity<TPrimaryKey> a, Entity<TPrimaryKey> b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    public static bool operator !=(Entity<TPrimaryKey> a, Entity<TPrimaryKey> b) => !(a == b);

    public override int GetHashCode() =>
        (ValueObject.GetUnproxiedType(this).ToString() + Id).GetHashCode();

    public override string ToString() => $"Id: '{Id}' -> Name: '{GetType().Name}'";

    private string GetDebuggerDisplay() => ToString();
}

/// <summary>
/// Convenience base for entities with an <see cref="int"/> primary key.
/// </summary>
[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public abstract class Entity : Entity<int>
{
    protected Entity() { }
    protected Entity(int id) : base(id) { }
    private string GetDebuggerDisplay() => ToString();
}
