using SimpleECommerce.vModels;
using SimpleECommerce.DataAndContext;
using SimpleECommerce.DataAndContext.ModelsForEommerce;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace SimpleECommerce.Services
{
    public class CartOrdersService : ICartOrdersService
    {
        private readonly ApplicationDbContext _dbContext;

        public CartOrdersService(ITransferPhotosToPathWithStoreService transferPhoto, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }



    }
}
