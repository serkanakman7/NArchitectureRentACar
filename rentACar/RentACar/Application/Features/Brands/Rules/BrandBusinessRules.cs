using Application.Features.Brands.Constants;
using Application.Services.Repositories;
using Core.CrosscuttingConcerns.Exceptions.Types;
using Core.CrosscuttingConcerns.Rules;
using Domain.Entities;

namespace Application.Features.Brands.Rules
{
    public class BrandBusinessRules : BaseBusinessRules
    {
        private readonly IBrandRepository _brandRepository;

        public BrandBusinessRules(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task BrandNameCannotBeDuplicatedWhenInserted(string name)
        {
            Brand? brand = await _brandRepository.GetAsync(predicate:b=>b.Name.ToLower() == name.ToLower());
            if (brand != null)
            {
                throw new BusinessException(BrandMessages.BrandNameExists);
            }
        }
    }
}
