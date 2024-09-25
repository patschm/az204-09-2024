using Contexts;
using Entities;
using Interfaces;

namespace Repository.InMemory;

public class BrandRepository : BaseRepository<Brand>, IBrandRepository
{
    public BrandRepository(ProductContext ctx): base(ctx)
    {

    }
}        
