using PixelMart.API.Data;

namespace PixelMart.API.Repositories.Impl;

public class PixelMartRepository : IPixelMartRepository
{
    private readonly PixelMartDbContext _dbContext;

    public PixelMartRepository(PixelMartDbContext context)
    {
        _dbContext = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken) >= 0;    // SaveChangesAsync result contains no of entries written to the DB
    }
}
