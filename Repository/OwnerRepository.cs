using Contracts;
using Entities;
using Entities.ExtendedModels;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class OwnerRepository: RepositoryBase<Owner>, IOwnerRepository
    {
        public OwnerRepository(RepositoryContext repositoryContext)
            :base(repositoryContext)
        {

        }

        public void CreateOwner(Owner owner)
        {
            owner.Id = Guid.NewGuid();
            Create(owner);
            Save();
        }

        public IEnumerable<Owner> GetAllOwners()
        {
            return FindAll().OrderBy(ow => ow.Name);
        }

        public Owner GetOwnerById(Guid ownerId)
        {
            return FindByCondition(x => x.Id.Equals(ownerId))
                    .DefaultIfEmpty(new Owner())
                    .FirstOrDefault();
        }

        public OwnerExtended GetOwnerWithDetails(Guid ownerId)
        {
            return new OwnerExtended(GetOwnerById(ownerId))
            {
                Accounts = RepositoryContext.Accounts.Where(a => a.OwnerId == ownerId)
            };
        }
    }
}
