using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Api.Dtos;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.Api.Controllers
{
	[ApiController]
	[Route("items")]
	public class ItemsController : ControllerBase
	{
		private readonly IItemsRepository repository;

		private readonly ILogger<ItemsController> logger;

		public ItemsController(IItemsRepository repository, ILogger<ItemsController> logger)
		{
			this.repository = repository;
			this.logger = logger;
		}


		//GET /items
		[HttpGet]
		public async Task<IEnumerable<ItemDto>> GetItemsAsync()
		{
			var items = (await repository.GetItemsAsync())
				.Select(item => item.AsDto());
			return items;
		}

		//GET /items/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
		{
			var item = await repository.GetItemAsync(id);

			if (item is null)
			{
				return NotFound();
			}

			return item.AsDto();
		}

		[HttpPost]
		public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
		{
			Item item = new()
			{
				Id = Guid.NewGuid(),
				Name = itemDto.Name,
				Price = itemDto.Price,
				CreatedDate = DateTimeOffset.UtcNow
			};

			await repository.CreateItemAsync(item);

			return CreatedAtAction("GetItem", new { id = item.Id }, item.AsDto());
			//Since GetItemAsync will be trimmed to GetItem controller and Rider doesn't see it
			//return CreatedAtAction(nameof(GetItemAsync), new {id = item.Id}, item.AsDto());
			// options.SuppressAsyncSuffixInActionNames = false; is another option
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
		{
			var existingItem = await repository.GetItemAsync(id);

			if (existingItem is null)
			{
				return NotFound();
			}

			Item updatedItem = existingItem with
			{
				Name = itemDto.Name,
				Price = itemDto.Price
			};

			await repository.UpdateItemAsync(updatedItem);

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteItemAsync(Guid id)
		{
			var exitingItem = await repository.GetItemAsync(id);

			if (exitingItem is null)
			{
				return NotFound();
			}

			await repository.DeleteItemAsync(id);

			return NoContent();
		}

		[HttpGet]
		public async Task<IEnumerable<ItemDto>> GetItemsAsync(string name = null)
		{
			var items = (await repository.GetItemsAsync())
				.Select(item => item.AsDto());

			if (!string.IsNullOrWhiteSpace(name))
			{
				items = items.Where(item => item.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
			}
			return items;
		}
	}
}