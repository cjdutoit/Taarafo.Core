﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Taarafo.Core.Models.Comments;
using Taarafo.Core.Models.Comments.Exceptions;
using Xunit;

namespace Taarafo.Core.Tests.Unit.Services.Foundations.Comments
{
	public partial class CommentServiceTests
	{
		[Fact]
		public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
		{
			// given
			Guid someId = Guid.NewGuid();
			SqlException sqlException = GetSqlException();

			var failedCommentStorageException =
				new FailedCommentStorageException(sqlException);

			var expectedCommentDependencyException =
				new CommentDependencyException(failedCommentStorageException);

			this.storageBrokerMock.Setup(broker =>
				broker.SelectCommentByIdAsync(It.IsAny<Guid>()))
					.ThrowsAsync(sqlException);

			// when
			ValueTask<Comment> retrieveCommentByIdTask =
				this.commentService.RetrieveCommentByIdAsync(someId);

			CommentDependencyException actualCommentDependencyException =
				await Assert.ThrowsAsync<CommentDependencyException>(
					retrieveCommentByIdTask.AsTask);

			// then
			actualCommentDependencyException.Should().BeEquivalentTo(
				expectedCommentDependencyException);

			this.storageBrokerMock.Verify(broker =>
				broker.SelectCommentByIdAsync(It.IsAny<Guid>()),
					Times.Once);

			this.loggingBrokerMock.Verify(broker =>
				broker.LogCritical(It.Is(SameExceptionAs(
					expectedCommentDependencyException))),
						Times.Once);

			this.storageBrokerMock.VerifyNoOtherCalls();
			this.loggingBrokerMock.VerifyNoOtherCalls();
			this.dateTimeBrokerMock.VerifyNoOtherCalls();
		}

		[Fact]
		public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
		{
			// given
			Guid someId = Guid.NewGuid();
			var serviceException = new Exception();

			var failedCommentServiceException =
				new FailedCommentServiceException(serviceException);

			var expectedCommentServiceException =
				new CommentServiceException(failedCommentServiceException);

			this.storageBrokerMock.Setup(broker =>
				broker.SelectCommentByIdAsync(It.IsAny<Guid>()))
					.ThrowsAsync(serviceException);

			// when
			ValueTask<Comment> retrieveCommentByIdTask =
				this.commentService.RetrieveCommentByIdAsync(someId);

			CommentServiceException actualCommentServiceException =
				await Assert.ThrowsAsync<CommentServiceException>(
					retrieveCommentByIdTask.AsTask);

			// then
			actualCommentServiceException.Should().BeEquivalentTo(
				expectedCommentServiceException);

			this.storageBrokerMock.Verify(broker =>
				broker.SelectCommentByIdAsync(It.IsAny<Guid>()),
					Times.Once);

			this.loggingBrokerMock.Verify(broker =>
			   broker.LogError(It.Is(SameExceptionAs(
				   expectedCommentServiceException))),
						Times.Once);

			this.storageBrokerMock.VerifyNoOtherCalls();
			this.loggingBrokerMock.VerifyNoOtherCalls();
			this.dateTimeBrokerMock.VerifyNoOtherCalls();
		}
	}
}
