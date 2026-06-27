using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.Book;
using CODE81_Assessment.Application.Exceptions;
using CODE81_Assessment.Application.Interfaces;
using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Application.Interfaces.Services;
using CODE81_Assessment.Domain.Entities;
using CODE81_Assessment.Domain.Enums;

namespace CODE81_Assessment.Application.Services
{
    public class BookService(
    IBookRepository bookRepository,
    IAuthorRepository authorRepository,
    ICategoryRepository categoryRepository,
    IFileStorageService fileStorageService,
    IUnitOfWork unitOfWork) : IBookService
    {
        private readonly IBookRepository _bookRepository = bookRepository;
        private readonly IAuthorRepository _authorRepository = authorRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IFileStorageService _fileStorageService = fileStorageService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        #region CRUD

        public async Task<BookDto> GetByIdAsync(int id)
        {
            var entity = await _bookRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Book not found");

            return MapToDto(entity);
        }

        public async Task<PaginatedResult<BookDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var (items, totalCount) = await _bookRepository.GetAllAsync(pageNumber, pageSize);

            return new PaginatedResult<BookDto>
            {
                Items = items.Select(MapToDto),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<BookDto> CreateAsync(BookCreateDto dto)
        {
            string? imageUrl = null;

            if (dto.CoverImage != null)
            {
                imageUrl = await _fileStorageService.SaveFileAsync(dto.CoverImage, "books");
            }

            var entity = MapToEntity(dto, imageUrl);

            await AttachRelations(entity, dto.AuthorIds, dto.CategoryIds);

            await _bookRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task UpdateAsync(int id, BookUpdateDto dto)
        {
            var entity = await _bookRepository.GetForUpdateAsync(id)
                ?? throw new EntityNotFoundException("Book not found");

            MapToUpdatedEntity(entity, dto);

            if (dto.AuthorIds is not null)
                await UpdateAuthors(entity, dto.AuthorIds);

            if (dto.CategoryIds is not null)
                await UpdateCategories(entity, dto.CategoryIds);

            _bookRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _bookRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Book not found");

            _bookRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Other Opertaions 
        public async Task<PaginatedResult<BookDto>> SearchAsync(BookSearchDto dto)
        {
            var (books, totalCount) = await _bookRepository.SearchAsync(
                dto.Title,
                dto.Author,
                dto.Category,
                dto.PageNumber,
                dto.PageSize
            );

            var result = books.Select(b => new BookDto
            {
                Id = b.Id,
                Summary = b.Summary,
                Title = b.Title,
                ISBN = b.ISBN,
                PublicationYear = b.PublicationYear,
                Language = b.Language,
                Edition = b.Edition,
                Status = b.Status,
                CoverImageUrl = b.CoverImageUrl,
                PublisherName = b.Publisher.Name,
            });

            return new PaginatedResult<BookDto>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize
            };
        }

        public async Task<PaginatedResult<BookDetailsDto>> GetByStatusAsync(BookByStatusDto dto)
        {
            var (books, totalCount) = await _bookRepository.GetByStatusAsync(
                dto.Status,
                dto.PageNumber,
                dto.PageSize
            );

            var result = books.Select(b => new BookDetailsDto
            {
                Id = b.Id,
                Title = b.Title,
                ISBN = b.ISBN,
                PublicationYear = b.PublicationYear,
                Language = b.Language,
                Edition = b.Edition,
                Summary = b.Summary,
                Status = b.Status.ToString(),
                CoverImageUrl = b.CoverImageUrl,
                PublisherName = b.Publisher.Name,
                Authors = b.Authors.Select(a => a.Name).ToList(),
                Categories = b.Categories.Select(c => c.Name).ToList()
            });

            return new PaginatedResult<BookDetailsDto>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize
            };
        }
        #endregion

        #region Helpers
        private static BookDto MapToDto(Book entity)
            => new()
            {
                Id = entity.Id,
                Title = entity.Title,
                ISBN = entity.ISBN,
                PublicationYear = entity.PublicationYear,
                Language = entity.Language,
                Edition = entity.Edition,
                Summary = entity.Summary,
                Status = entity.Status,
                PublisherName = entity.Publisher.Name,
                Authors = entity.Authors.Select(a => a.Name).ToList(),
                Categories = entity.Categories.Select(c => c.Name).ToList()
            };

        private static Book MapToEntity(BookCreateDto dto, string? imageUrl)
            => new()
            {
                Title = dto.Title,
                ISBN = dto.ISBN,
                PublicationYear = dto.PublicationYear,
                Language = dto.Language,
                Edition = dto.Edition,
                Summary = dto.Summary,
                PublisherId = dto.PublisherId,
                Status = BookStatus.Available,
                CoverImageUrl = imageUrl
            };

        private static void MapToUpdatedEntity(Book entity, BookUpdateDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Title)) entity.Title = dto.Title;
            if (!string.IsNullOrWhiteSpace(dto.ISBN)) entity.ISBN = dto.ISBN;
            if (dto.PublicationYear.HasValue) entity.PublicationYear = dto.PublicationYear.Value;
            if (!string.IsNullOrWhiteSpace(dto.Language)) entity.Language = dto.Language;
            if (!string.IsNullOrWhiteSpace(dto.Edition)) entity.Edition = dto.Edition;
            if (!string.IsNullOrWhiteSpace(dto.Summary)) entity.Summary = dto.Summary;
            if (dto.Status.HasValue) entity.Status = dto.Status.Value;
            if (!string.IsNullOrWhiteSpace(dto.CoverImageUrl)) entity.CoverImageUrl = dto.CoverImageUrl;
            if (dto.PublisherId.HasValue) entity.PublisherId = dto.PublisherId.Value;
        }

        private async Task AttachRelations(Book book, List<int> authorIds, List<int> categoryIds)
        {
            var authors = await _authorRepository.GetByIdsAsync(authorIds);
            var categories = await _categoryRepository.GetByIdsAsync(categoryIds);

            book.Authors = authors;
            book.Categories = categories;
        }

        private async Task UpdateAuthors(Book book, List<int> authorIds)
        {
            var authors = await _authorRepository.GetByIdsAsync(authorIds);

            book.Authors.Clear();
            book.Authors = authors;
        }

        private async Task UpdateCategories(Book book, List<int> categoryIds)
        {
            var categories = await _categoryRepository.GetByIdsAsync(categoryIds);

            book.Categories.Clear();
            book.Categories = categories;
        }

        #endregion
    }
}
