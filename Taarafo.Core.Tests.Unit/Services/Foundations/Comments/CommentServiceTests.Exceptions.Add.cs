﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Taarafo.Core.Models.Comments;
using Taarafo.Core.Models.Comments.Exceptions;
using Xunit;

namespace Taarafo.Core.Tests.Unit.Services.Foundations.Comments
{
    public partial class CommentServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Comment someComment = CreateRandomComment();
            SqlException sqlException = GetSqlException();

            var failedCommentStorageException =
                new FailedCommentStorageException(sqlException);

            var expectedCommentDependencyException =
                new CommentDependencyException(failedCommentStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Comment> addCommentTask =
                this.commentService.AddCommentAsync(someComment);

            // then
            await Assert.ThrowsAsync<CommentDependencyException>(() =>
               addCommentTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCommentAsync(It.IsAny<Comment>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCommentDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfCommentAlreadyExsitsAndLogItAsync()
        {
            // given
            Comment randomComment = CreateRandomComment();
            Comment alreadyExistsComment = randomComment;
            string randomMessage = GetRandomMessage();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsCommentException =
                new AlreadyExistsCommentException(duplicateKeyException);

            var expectedCommentDependencyValidationException =
                new CommentDependencyValidationException(alreadyExistsCommentException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(duplicateKeyException);

            // when
            ValueTask<Comment> addCommentTask =
                this.commentService.AddCommentAsync(alreadyExistsComment);

            // then
            await Assert.ThrowsAsync<CommentDependencyValidationException>(() =>
                addCommentTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCommentAsync(It.IsAny<Comment>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameValidationExceptionAs(
                    expectedCommentDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            Comment someComment = CreateRandomComment();

            var databaseUpdateException =
                new DbUpdateException();

            var failedCommentStorageException =
                new FailedCommentStorageException(databaseUpdateException);

            var expectedCommentDependencyException =
                new CommentDependencyException(failedCommentStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<Comment> addCommentTask =
                this.commentService.AddCommentAsync(someComment);

            // then
            await Assert.ThrowsAsync<CommentDependencyException>(() =>
               addCommentTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCommentAsync(It.IsAny<Comment>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCommentDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            Comment someComment = CreateRandomComment();
            var serviceException = new Exception();

            var failedCommentServiceException =
                new FailedCommentServiceException(serviceException);

            var expectedCommentServiceException =
                new CommentServiceException(failedCommentServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<Comment> addCommentTask =
                this.commentService.AddCommentAsync(someComment);

            // then
            await Assert.ThrowsAsync<CommentServiceException>(() =>
                addCommentTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCommentAsync(It.IsAny<Comment>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCommentServiceException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime =
                GetRandomDateTimeOffset();

            Comment randomComment = CreateRandomComment(randomDateTime);
            Comment someComment = randomComment;
            string randomMessage = GetRandomMessage();
            string exceptionMessage = randomMessage;
            var foreignKeyConstraintConflictException = 
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidCommentReferenceException =
                new InvalidCommentReferenceException(foreignKeyConstraintConflictException);

            var expectedCommentValidationException =
                new CommentDependencyValidationException(invalidCommentReferenceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Throws(foreignKeyConstraintConflictException);

            // when
            ValueTask<Comment> addCommentTask =
                this.commentService.AddCommentAsync(someComment);

            // then
            await Assert.ThrowsAsync<CommentDependencyValidationException>(() =>
                addCommentTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(expectedCommentValidationException))),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
