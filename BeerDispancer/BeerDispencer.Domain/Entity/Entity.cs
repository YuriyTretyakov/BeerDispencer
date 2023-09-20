using System;
namespace BeerDispencer.Domain.Entity
{
    public abstract class Entity
    {
        public Guid? Id { get; protected set; }

        public override bool Equals(object? obj)
        {
            var instance = (obj as Entity);

            if (instance == null)
            {
                return false;
            }

            return (Id == instance.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}

