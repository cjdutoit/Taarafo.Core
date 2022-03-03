﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Moq;
using Taarafo.Core.Models.Profiles;
using Taarafo.Core.Models.Profiles.Exceptions;
using Xunit;

namespace Taarafo.Core.Tests.Unit.Services.Foundations.Profiles
{
    public partial class ProfileServiceTests
    {
        [Fact]
        public async void ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidProfileId = Guid.Empty;

            var invalidProfileException =
                new InvalidProfileException();

            invalidProfileException.AddData(
                key: nameof(Profile.Id),
                values: "Id is required");

            var expectedProfileValidationException =
                new ProfileValidationException(invalidProfileException);

            // when
            ValueTask<Profile> retrieveProfileByIdTask =
                this.profileService.RetrieveProfileByIdAsync(invalidProfileId);

            // then
            await Assert.ThrowsAsync<ProfileValidationException>(() =>
                retrieveProfileByIdTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedProfileValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectProfileByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfProfileIsNotFoundAndLogItAsync()
        {
            // given
            Guid someProfileId = Guid.NewGuid();
            Profile noProfile = null;

            var notFoundProfileException =
                new NotFoundProfileException(someProfileId);

            var expectedProfileValidationException =
                new ProfileValidationException(notFoundProfileException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectProfileByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noProfile);
            // when
            ValueTask<Profile> retrieveProfileByIdTask =
                this.profileService.RetrieveProfileByIdAsync(someProfileId);

            // then
            await Assert.ThrowsAsync<ProfileValidationException>(() =>
                retrieveProfileByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectProfileByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedProfileValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
