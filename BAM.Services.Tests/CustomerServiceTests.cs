using AutoMapper;
using BAM.Contracts.DTO;
using BAM.DataAccessLayer.Interfaces;
using BAM.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace BAM.Services.Tests.Services
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepo> _repo = new(MockBehavior.Strict);
        private readonly Mock<IMapper> _mapper = new(MockBehavior.Strict);
        private readonly Mock<ILogger<CustomerService>> _logger = new();

        private CustomerService CreateSut() => new(_repo.Object, _mapper.Object, _logger.Object);

        [Fact]
        public async Task GetByIdAsync_ReturnsMappedDto()
        {
            var id = 5;
            var entity = new Customer { Id = id, Name = "Alice", CreditRating = 80 };
            var dto = new CustomerDto { Id = id, Name = "Alice", CreditRating = 80 };

            _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mapper.Setup(m => m.Map<CustomerDto>(entity)).Returns(dto);

            var sut = CreateSut();
            var result = await sut.GetByIdAsync(id);

            Assert.Same(dto, result);
            _repo.Verify(r => r.GetByIdAsync(id), Times.Once);
            _mapper.Verify(m => m.Map<CustomerDto>(entity), Times.Once);
        }
    }
}
