using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.Publisher;
using CODE81_Assessment.Application.Exceptions;
using CODE81_Assessment.Application.Interfaces;
using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Application.Interfaces.Services;
using CODE81_Assessment.Domain.Entities;

namespace CODE81_Assessment.Application.Services
{
    public class PublisherService(IPublisherRepository publisherRepository, IUnitOfWork unitOfWork) : IPublisherService
    {
        private readonly IPublisherRepository _publisherRepository = publisherRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        #region CRUD
        public async Task<PublisherDto> GetByIdAsync(int id)
        {
            var entity = await _publisherRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Publisher not found");

            return MapToDto(entity);
        }

        public async Task<PaginatedResult<PublisherDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var (items, totalCount) = await _publisherRepository.GetAllAsync(pageNumber, pageSize);

            return new PaginatedResult<PublisherDto>
            {
                Items = items.Select(MapToDto),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PublisherDto> CreateAsync(PublisherCreateDto dto)
        {
            var exists = await _publisherRepository.ExistsByNameAsync(dto.Name);
            if (exists)
                throw new BadRequestException("Publisher already exists");

            var entity = MapToEntity(dto);

            await _publisherRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task UpdateAsync(int id, PublisherUpdateDto dto)
        {
            var entity = await _publisherRepository.GetByIdAsync(id, false)
                ?? throw new EntityNotFoundException("Publisher not found");

            MapToUpdatedEntity(entity, dto);

            _publisherRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _publisherRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Publisher not found");

            _publisherRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }
        #endregion

        #region Helpers
        private static PublisherDto MapToDto(Publisher entity)
            => new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Country = entity.Country,
                Email = entity.Email,
                Phone = entity.Phone
            };

        private static Publisher MapToEntity(PublisherCreateDto dto)
            => new()
            {
                Name = dto.Name,
                Country = dto.Country,
                Email = dto.Email,
                Phone = dto.Phone
            };

        private static void MapToUpdatedEntity(Publisher entity, PublisherUpdateDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Name))
                entity.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Country))
                entity.Country = dto.Country;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                entity.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                entity.Phone = dto.Phone;
        }
        #endregion
    }
}
