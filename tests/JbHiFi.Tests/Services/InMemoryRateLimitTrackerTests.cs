using JbHiFi.Interfaces;
using JbHiFi.Services;
using Xunit;

namespace JbHiFi.Tests.Services
{
    public class InMemoryRateLimitTrackerTests
    {
        private readonly IRateLimitTracker _tracker;

        public InMemoryRateLimitTrackerTests()
        {
            _tracker = new InMemoryRateLimitTracker();
        }

        [Fact]
        public void FirstCall_ShouldNotBeLimited()
        {
            // Act
            var result = _tracker.IsLimitExceeded("test-key");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AfterFiveCalls_ShouldBeLimited()
        {
            var key = "test-key";

            // Act
            for (int i = 0; i < 5; i++)
            {
                _tracker.RegisterCall(key);
            }

            var result = _tracker.IsLimitExceeded(key);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void FewerThanLimit_ShouldNotBeLimited()
        {
            var key = "test-key";

            for (int i = 0; i < 4; i++)
            {
                _tracker.RegisterCall(key);
            }

            var result = _tracker.IsLimitExceeded(key);

            Assert.False(result);
        }
    }
}
