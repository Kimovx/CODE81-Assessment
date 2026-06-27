using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.BorrowingTransaction;
using CODE81_Assessment.Application.Exceptions;
using CODE81_Assessment.Application.Interfaces;
using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Application.Interfaces.Services;
using CODE81_Assessment.Domain.Entities;
using CODE81_Assessment.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CODE81_Assessment.Application.Services
{
    public class BorrowingService(
    IBorrowingRepository borrowingRepository,
    IBookRepository bookRepository,
    ILibraryMemberRepository memberRepository,
    IUnitOfWork unitOfWork) : IBorrowingService
    {
        private readonly IBorrowingRepository _repo = borrowingRepository;
        private readonly IBookRepository _bookRepo = bookRepository;
        private readonly ILibraryMemberRepository _memberRepo = memberRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        #region CRUD
        public async Task<BorrowingTransactionDto> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Transaction not found");

            return MapToDto(entity);
        }

        public async Task<PaginatedResult<BorrowingTransactionDto>> GetAllAsync(int page, int size)
        {
            var (items, total) = await _repo.GetAllAsync(page, size);

            return new PaginatedResult<BorrowingTransactionDto>
            {
                Items = items.Select(MapToDto),
                TotalCount = total,
                PageNumber = page,
                PageSize = size
            };
        }

        public async Task<BorrowingTransactionDto> BorrowAsync(BorrowingTransactionCreateDto dto, int userId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var member = await _memberRepo.GetByIdAsync(dto.MemberId)
                ?? throw new EntityNotFoundException("Member not found");

                var book = await _bookRepo.GetByIdAsync(dto.BookId, true)
                ?? throw new EntityNotFoundException("Book not found");

                if (book.Status == BookStatus.Borrowed)
                    throw new BadRequestException("Book already borrowed");

                book.Status = BookStatus.Borrowed;

                var entity = new BorrowingTransaction
                {
                    MemberId = dto.MemberId,
                    BookId = dto.BookId,
                    CreatedById = userId,
                    BorrowDate = DateTimeOffset.UtcNow,
                    DueDate = dto.DueDate,
                    Notes = dto.Notes,
                    Status = TransactionStatus.Active
                };

                await _repo.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();

                return await GetByIdAsync(entity.Id);
            }

            // Handling Race Condition Case
            catch (DbUpdateConcurrencyException)
            {
                await _unitOfWork.RollbackAsync();
                throw new ConflictException("Book was already borrowed by another user.");
            }

            catch
            {
                await _unitOfWork.RollbackAsync();
                throw new UnknownInternalServerError();
            }
        }

        public async Task ReturnAsync(ReturnBookDto dto, int userId)
        {
            var entity = await _repo.GetByIdAsync(dto.TransactionId, false)
                ?? throw new EntityNotFoundException("Transaction not found");

            if (entity.Status != TransactionStatus.Active)
                throw new BadRequestException("Transaction already closed");

            entity.ReturnDate = DateTimeOffset.UtcNow;
            entity.ReturnedById = userId;
            entity.Status = TransactionStatus.Returned;
            entity.Notes = dto.Notes ?? entity.Notes;

            _repo.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }
        #endregion

        #region Helpers
        private static BorrowingTransactionDto MapToDto(BorrowingTransaction x)
            => new()
            {
                Id = x.Id,
                MemberId = x.MemberId,
                MemberName = x.Member.FullName,
                BookId = x.BookId,
                BookTitle = x.Book.Title,
                BorrowDate = x.BorrowDate,
                DueDate = x.DueDate,
                ReturnDate = x.ReturnDate,
                Status = x.Status,
                Notes = x.Notes
            };
        #endregion
    }
}
