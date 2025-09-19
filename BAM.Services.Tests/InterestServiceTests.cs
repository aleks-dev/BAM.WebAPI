using BAM.Contracts.Enums;
using BAM.Domain.Calculators;
using Microsoft.Extensions.Logging;
using Moq;

namespace BAM.Services.Tests.Services
{
    public class InterestServiceTests
    {
        private readonly Mock<IInterestRateCalc> _calc = new(MockBehavior.Strict);
        private readonly Mock<ILogger<InterestService>> _logger = new();

        private InterestService CreateSut() => new(_calc.Object, _logger.Object);

        [Theory]
        [InlineData(19)]
        [InlineData(0)]
        public void GetInterestRate_Throws_WhenCreditRatingTooLow(int creditRating)
        {
            var sut = CreateSut();
            Assert.Throws<InvalidOperationException>(() => sut.GetInterestRate(creditRating, (int)LoanDuration.OneYear));
        }

        [Theory]
        [InlineData(20, 2)]
        [InlineData(50, 4)]
        [InlineData(80, 15)]
        public void GetInterestRate_Throws_WhenDurationOutOfRange(int creditRating, int duration)
        {
            var sut = CreateSut();
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.GetInterestRate(creditRating, duration));
        }

        [Theory]
        [InlineData(20, (int)LoanDuration.OneYear, 4.5)]
        [InlineData(55, (int)LoanDuration.ThreeYears, 7.25)]
        [InlineData(90, (int)LoanDuration.FiveYears, 2.0)]
        public void GetInterestRate_ReturnsRate_FromCalculator(int creditRating, int duration, double expected)
        {
            // Arrange
            _calc.Setup(c => c.CalculateInterestRate(creditRating, duration))
                .Returns((decimal)expected);

            var sut = CreateSut();

            // Act
            var rate = sut.GetInterestRate(creditRating, duration);

            // Assert
            Assert.Equal((decimal)expected, rate);
            _calc.Verify(c => c.CalculateInterestRate(creditRating, duration), Times.Once);
        }
    }
}
