﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

using System;
using Taarafo.Core.Models.GroupPosts;
using Taarafo.Core.Models.GroupPosts.Exceptions;

namespace Taarafo.Core.Services.Foundations.GroupPosts
{
    public partial class GroupPostService
    {
        public static void ValidateGroupPostOnAdd(GroupPost groupPost)
        {
            ValidateGroupPostIsNotNull(groupPost);

            Validate(
                (Rule: IsInvalid(groupPost.GroupId), Parameter: nameof(GroupPost.GroupId)),
                (Rule: IsInvalid(groupPost.PostId), Parameter: nameof(GroupPost.PostId)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(object @object) => new
        {
            Condition = @object is null,
            Message = "Object is required"
        };

        private static void ValidateGroupPostIsNotNull(GroupPost groupPost)
        {
            if (groupPost is null)
            {
                throw new NullGroupPostException();
            }
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidGroupPostException = new InvalidGroupPostException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidGroupPostException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidGroupPostException.ThrowIfContainsErrors();
        }
    }
}
