using System;
using System.Collections.Generic;
using System.Linq;
using Catalog.Dtos;
using Catalog.Entities;
using Catalog.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers
{
	[ApiController]
	[Route("items")]
	public class ItemsController : ControllerBase
	{
		private readonly IInMemItemsRepository repository;

		public ItemsController(InMemItemsRepository repository)
		{
			this.repository = repository;
		}


		//GET /items
		[HttpGet]
		public IEnumerable<ItemDto> GetItems()
		{
			var items = repository.GetItems().Select(item => item.asDto());
			return items;
		}

		//GET /items/{id}
		[HttpGet("{id}")]
		public ActionResult<ItemDto> GetItem(Guid id)
		{
			var item = repository.GetItem(id);

			if (item is null)
			{
				return NotFound();
			}

			return item.asDto();
		}
	}
}