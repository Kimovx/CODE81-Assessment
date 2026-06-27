using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.Author;
using CODE81_Assessment.Application.Exceptions;
using CODE81_Assessment.Application.Interfaces;
using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Application.Interfaces.Services;
using CODE81_Assessment.Domain.Entities;

namespace CODE81_Assessment.Application.Services
{
    public class AuthorService(
    IAuthorRepository authorRepository,
    IUnitOfWork unitOfWork) : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository = authorRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        #region CRUD

        public async Task<AuthorDto> GetByIdAsync(int id)
        {
            var entity = await _authorRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Author not found");

            return MapToGetDto(entity);
        }

        public async Task<PaginatedResult<AuthorDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var (items, totalCount) = await _authorRepository.GetAllAsync(pageNumber, pageSize);

            return new PaginatedResult<AuthorDto>
            {
                Items = items.Select(MapToGetDto),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<AuthorDto> CreateAsync(AuthorCreateDto dto)
        {
            var entity = MapToEntity(dto);

            await _authorRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return MapToGetDto(entity);
        }

        public async Task UpdateAsync(int id, AuthorUpdateDto dto)
        {
            var entity = await _authorRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Author not found");

            MapToUpdatedEntity(entity, dto);

            _authorRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _authorRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Author not found");

            _authorRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Helpers

        private static AuthorDto MapToGetDto(Author entity)
            => new()
            {
                Id = entity.Id,
                Name = entity.Name
            };

        private static Author MapToEntity(AuthorCreateDto dto)
            => new()
            {
                Name = dto.Name
            };

        private static void MapToUpdatedEntity(Author entity, AuthorUpdateDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Name))
                entity.Name = dto.Name;
        }

        #endregion
    }
}
