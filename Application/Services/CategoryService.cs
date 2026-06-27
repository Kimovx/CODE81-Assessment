using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.Category;
using CODE81_Assessment.Application.Exceptions;
using CODE81_Assessment.Application.Interfaces;
using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Application.Interfaces.Services;
using CODE81_Assessment.Domain.Entities;

namespace CODE81_Assessment.Application.Services
{
    public class CategoryService(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        #region CRUD

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var entity = await _categoryRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Category not found");

            return MapToGetDto(entity);
        }

        public async Task<PaginatedResult<CategoryDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var (items, totalCount) = await _categoryRepository.GetAllAsync(pageNumber, pageSize);

            return new PaginatedResult<CategoryDto>
            {
                Items = items.Select(item => MapToGetDto(item)),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<CategoryDto> CreateAsync(CategoryCreateDto dto)
        {
            var parentCategoryName = string.Empty;

            if (dto.ParentCategoryId.HasValue)
            {
                if (dto.ParentCategoryId <= 0)
                    throw new BadRequestException("Invalid parent category id");

                var exists = await _categoryRepository.ExistsAsync(dto.ParentCategoryId.Value);

                if (!exists)
                    throw new EntityNotFoundException("Parent category not found");
                
                parentCategoryName = await _categoryRepository.GetCategoryNameAsync(dto.ParentCategoryId.Value);
            }

            var entity = MapToEntity(dto);


            await _categoryRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();


            return MapToGetDto(entity, parentCategoryName);
        }

        public async Task UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var entity = await _categoryRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Category not found");

            if (dto.ParentCategoryId.HasValue)
            {
                if (dto.ParentCategoryId == id)
                    throw new BadRequestException("Category cannot be parent of itself");

                var exists = await _categoryRepository.ExistsAsync(dto.ParentCategoryId.Value);

                if (!exists)
                    throw new EntityNotFoundException("Parent category not found");
            }

            MapToUpdatedEntity(entity, dto);

            _categoryRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _categoryRepository.GetWithSubCategoriesAsync(id)
                ?? throw new EntityNotFoundException("Category not found");

            if (entity.SubCategories.Any())
                throw new ConflictException("Cannot delete category with subcategories");

            _categoryRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Helpers

        private static CategoryDto MapToGetDto(Category entity, string? parentCategoryName = null)
            => new()
            {
                Id = entity.Id,
                Name = entity.Name,
                ParentCategoryId = entity.ParentCategoryId,
                ParentCategoryName = parentCategoryName
            };

        private static Category MapToEntity(CategoryCreateDto dto)
            => new()
            {
                Name = dto.Name,
                ParentCategoryId = dto.ParentCategoryId
            };

        private static void MapToUpdatedEntity(Category entity, CategoryUpdateDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Name))
                entity.Name = dto.Name;

            if (dto.ParentCategoryId.HasValue)
                entity.ParentCategoryId = dto.ParentCategoryId;
        }

        #endregion
    }
}
