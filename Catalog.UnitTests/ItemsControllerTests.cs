using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Api.Controllers;
using Catalog.Api.Dtos;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using DnsClient.Internal;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Catalog.UnitTests
{
	public class ItemsControllerTests
	{ 
		private readonly Mock<IItemsRepository> repositoryStub = new(); 
		private readonly Mock<ILogger<ItemsController>> loggerStub = new();
		private readonly Random rand = new();
		
		[Fact]
		//public void UnitOfWork_StateUnderTest_ExpectedBehavior()
		public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
		{
			// Arrange
			repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
				.ReturnsAsync((Item)null); //cast to null to Item so that Moq doesn't get confused

			var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

			// Act
			var result = await controller.GetItemAsync(Guid.NewGuid());

			// Assert
			result.Result.Should().BeOfType<NotFoundResult>();
		}
		
		[Fact]
		//public void UnitOfWork_StateUnderTest_ExpectedBehavior()
		public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
		{
			// Arrange
			var expectedItem = CreateRandomItem();
			repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
				.ReturnsAsync(expectedItem);

			var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);
			
			// Act
			var result = await controller.GetItemAsync(Guid.NewGuid());

			// Assert
			Assert.IsType<ItemDto>(result.Value);
			var dto = result.Value;
			
			result.Value.Should().BeEquivalentTo(
				expectedItem,
				options => options.ComparingByMembers<Item>());
			//Good practice is should have only 1 assertion. 
			// Assert.Equal(expectedItem.Id, dto.Id);
			// Assert.Equal(expectedItem.Name, dto.Name);
			// Assert.Equal(expectedItem.Price, dto.Price);
		}

		[Fact]
		public async Task GetItemsAsync_WithExistingItems_ReturnsAllItems()
		{
			// Arrange
			var expectedItems = new[] {CreateRandomItem(), CreateRandomItem(), CreateRandomItem()};
			repositoryStub.Setup(repo => repo.GetItemsAsync())
				.ReturnsAsync(expectedItems);

			var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);
			
			// Act
			var actualItems = await controller.GetItemsAsync();
			
			//Assert
			actualItems.Should().BeEquivalentTo(
				expectedItems,
				options => options.ComparingByMembers<Item>());
		}
		
		[Fact]	
		public async Task CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
		{
			// Arrange
			var itemToCreate = new CreateItemDto()
			{
				Name = Guid.NewGuid().ToString(),
				Price = rand.Next(1000)
			};

			var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);
			
			// Act
			var result = await controller.CreateItemAsync(itemToCreate);
			
			//Assert
			var createdItem = ((CreatedAtActionResult) result.Result).Value as ItemDto;
			itemToCreate.Should().BeEquivalentTo(
				createdItem,
				//compare Item vs ItemDto, different no of prop
				options => options.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
				);
			createdItem.Id.Should().NotBeEmpty();
			createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
		}

		[Fact]
		public async Task UpdateItemAsync_WithExistingItem_ReturnsNoContent()
		{
			//Arrange
			Item exisingItem = CreateRandomItem();
			repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
				.ReturnsAsync(exisingItem);

			var itemId = exisingItem.Id;
			var itemToUpdate = new UpdateItemDto()
			{
				Name = Guid.NewGuid().ToString(),
				Price = exisingItem.Price + 3,
			};

			var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);
			
			//Act
			var result = await controller.UpdateItemAsync(itemId, itemToUpdate);
			
			//Assert
			result.Should().BeOfType<NoContentResult>();
		}
		
		[Fact]
		public async Task DeleteItemAsync_WithExisitingItem_ReturnsNoContent()
		{
			// Arrange
			Item existingItem = CreateRandomItem();
			repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
				.ReturnsAsync(existingItem); 

			var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

			// Act
			var result = await controller.DeleteItemAsync(Guid.NewGuid());

			// Assert
			result.Should().BeOfType<NoContentResult>();
		}

		[Fact]
		public async Task GetItemsAsync_WithMatchingItems_ReturnsMatchingItems()
		{
			//Arrange
			var allItems = new[]
			{
				new Item() {Name = "Potion"},
				new Item() {Name = "Antidote"},
				new Item() {Name = "Hi-Potion"},
			};

			var nameToMatch = "Potion";

			repositoryStub.Setup(repo => repo.GetItemsAsync())
				.ReturnsAsync(allItems);

			var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);
			
			//Act
			IEnumerable<ItemDto> foundItems = await controller.GetItemsAsync(nameToMatch);
			
			//Assert
			foundItems.Should().OnlyContain(
				item => item.Name == allItems[0].Name || item.Name == allItems[2].Name
			);
		}
		

		private Item CreateRandomItem()
		{
			return new()
			{
				Id = Guid.NewGuid(),
				Name = Guid.NewGuid().ToString(),
				Price = rand.Next(1000),
				CreatedDate = DateTimeOffset.UtcNow
			};
		}

	}
}
