namespace PixelMart.API.Repositories;

public interface IPixelMartRepository
{
    Task<bool> SaveAsync(CancellationToken cancellationToken = default);
}
