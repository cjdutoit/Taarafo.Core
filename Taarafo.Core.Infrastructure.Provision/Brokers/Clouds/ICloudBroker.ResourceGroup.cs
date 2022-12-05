﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace Taarafo.Core.Infrastructure.Provision.Brokers.Clouds
{
	public partial interface ICloudBroker
	{
		ValueTask<IResourceGroup> CreateResourceGroupAsync(string resourceGroupName);
		ValueTask<bool> CheckResourceGroupExistAsync(string resourceGroupName);
		ValueTask DeleteResourceGroupAsync(string resourceGroupName);
	}
}
