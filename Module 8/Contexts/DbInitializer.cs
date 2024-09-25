using Entities;
using Newtonsoft.Json;

namespace Contexts;

public static class DbInitializer
{
    public static async Task<bool> InitializeDatabase(ProductContext context, bool withSeed = true)
    {
        var ok = await context.Database.EnsureCreatedAsync();
        if (ok && withSeed)
        {
            Seed(context);
        }
        return ok;
    }
    private static T? ReadEmbeddedResource<T>(string resource) where T: class
    {
        var json = new JsonSerializer();
        json.MissingMemberHandling = MissingMemberHandling.Ignore;
        using(var res = typeof(DbInitializer).Assembly.GetManifestResourceStream($"Contexts.Data.{resource}"))
        {
            using (var rdr = new StreamReader(res!))
            {
                using(var jrdr = new JsonTextReader(rdr))
                {
                    try
                    {
                        return json.Deserialize<T>(jrdr);
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                }
            }
        } 
    }
    private static void Seed(ProductContext context)
    {
        var brands = ReadEmbeddedResource<List<Brand>>("brands.json");
        context.Brands.AddRange(brands!);
            
        var products =  ReadEmbeddedResource<List<Product>>("products.json");
        context.Products.AddRange(products!);
        var productGroups = ReadEmbeddedResource<List<ProductGroup>>("productgroups.json");
        context.ProductGroups.AddRange(productGroups!);
         var productGroupProducts = ReadEmbeddedResource<List<ProductGroupProduct>>("productgroupproducts.json");
        context.ProductGroupProducts.AddRange(productGroupProducts!);
        context.SaveChanges();
        
    }
}