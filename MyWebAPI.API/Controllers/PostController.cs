using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Core.Entities;
using MyWebAPI.Core.Interfaces;
using MyWebAPI.Core.Interfaces.IRepositories;
using MyWebAPI.Infrastructure.DataBase;

namespace MyWebAPI.API.Controllers
{
    [Route("api/posts")]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PostController(IPostRepository postRepository,
            IUnitOfWork unitOfWork)
        {
            this._postRepository = postRepository;
            this._unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = await this._postRepository.GetAllPostsAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var post = new Post()
            {
                Author = "Jack",
                Title = "Hello",
                Body = "YYYYYYYYYYYYYYYYYYYY",
                LastModified = DateTime.Now
            };

            this._postRepository.AddPost(post);

            await _unitOfWork.SaveAsync();

            return Ok();
        }


    }
}