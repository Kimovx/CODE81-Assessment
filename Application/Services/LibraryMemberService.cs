using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.LibraryMember;
using CODE81_Assessment.Application.Exceptions;
using CODE81_Assessment.Application.Interfaces;
using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Application.Interfaces.Services;
using CODE81_Assessment.Domain.Entities;
using CODE81_Assessment.Domain.Enums;

namespace CODE81_Assessment.Application.Services
{
    public class LibraryMemberService(
        ILibraryMemberRepository libraryMemberRepository,
        IUnitOfWork unitOfWork) : ILibraryMemberService
    {
        private readonly ILibraryMemberRepository _libraryMemberRepository = libraryMemberRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        #region CRUD
        public async Task<LibraryMemberDto> GetByIdAsync(int id)
        {
            var entity = await _libraryMemberRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Library Member not found");

            return MapToDto(entity);
        }

        public async Task<PaginatedResult<LibraryMemberDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var (items, totalCount) = await _libraryMemberRepository.GetAllAsync(pageNumber, pageSize);

            return new PaginatedResult<LibraryMemberDto>
            {
                Items = items.Select(MapToDto),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<LibraryMemberDto> CreateAsync(LibraryMemberCreateDto dto)
        {
            var entity = MapToEntity(dto);

            await _libraryMemberRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task UpdateAsync(int id, LibraryMemberUpdateDto dto)
        {
            var entity = await _libraryMemberRepository.GetByIdAsync(id, false)
                ?? throw new EntityNotFoundException("Library Member not found");

            MapToUpdatedEntity(entity, dto);

            _libraryMemberRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _libraryMemberRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Library Member not found");

            _libraryMemberRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Helpers
        private static LibraryMemberDto MapToDto(LibraryMember entity)
            => new()
            {
                Id = entity.Id,
                FullName = entity.FullName,
                Email = entity.Email,
                Phone = entity.Phone,
                Address = entity.Address,
                MembershipStartDate = entity.MembershipStartDate,
                MembershipEndDate = entity.MembershipEndDate,
                Status = entity.Status
            };

        private static LibraryMember MapToEntity(LibraryMemberCreateDto dto)
            => new()
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                MembershipStartDate = DateTimeOffset.UtcNow,
                Status = MemberStatus.Active
            };

        private static void MapToUpdatedEntity(LibraryMember entity, LibraryMemberUpdateDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.FullName))
                entity.FullName = dto.FullName;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                entity.Phone = dto.Phone;

            if (!string.IsNullOrWhiteSpace(dto.Address))
                entity.Address = dto.Address;

            if (dto.MembershipEndDate.HasValue)
                entity.MembershipEndDate = dto.MembershipEndDate;

            if (dto.Status.HasValue)
                entity.Status = dto.Status.Value;
        }
        #endregion
    }
}