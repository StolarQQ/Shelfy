﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Shelfy.Infrastructure.Commands.Review;
using Shelfy.Infrastructure.Services;

namespace Shelfy.API.Controllers
{
    [Route("book/{bookId}/[controller]")]
    [ApiController]
    public class ReviewController : ApiControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        
        [HttpGet(Name = "GetReviewsForBook")]
        public async Task<IActionResult> Get(Guid bookId)
        {
            var reviews = await _reviewService.GetReviewsForBookAsync(bookId);

            return Ok(reviews);
        }

        [HttpPost]
        [Authorize(Policy = "HasUserRole")]
        public async Task<IActionResult> Post([FromBody]CreateBookReview command, Guid bookId)
        {
            await _reviewService.AddAsync(command.Rating, command.Comment, UserId, bookId);

            return Created("", command);
        }

        [HttpPatch]
        [Authorize(Policy = "HasUserRole")]
        public async Task<IActionResult> Patch(Guid bookId, [FromBody]JsonPatchDocument<UpdateReview> review)
        {
            await _reviewService.UpdateAsync(bookId, UserId, review);

            return NoContent();
        }

        [HttpDelete]
        [Authorize(Policy = "HasUserRole")]
        public async Task<IActionResult> Delete(Guid bookId)
        {
            await _reviewService.DeleteAsync(bookId, UserId);

            return NoContent();
        }
    }
}