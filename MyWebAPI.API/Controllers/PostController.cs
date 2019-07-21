using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyWebAPI.Core.Entities;
using MyWebAPI.Core.Interfaces;
using MyWebAPI.Core.Interfaces.IRepositories;
using MyWebAPI.Infrastructure.DataBase;
using MyWebAPI.Infrastructure.Resources;

namespace MyWebAPI.API.Controllers
{
    [Route("api/posts")]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PostController> _logger;
        private readonly IMapper _mapper;

        public PostController(IPostRepository postRepository,
            IUnitOfWork unitOfWork,
             ILogger<PostController> logger,
             IMapper mapper)
        {
            this._postRepository = postRepository;
            this._unitOfWork = unitOfWork;
            this._logger = logger;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var post = await this._postRepository.GetAllPostsAsync();

            var postResources = this._mapper.Map<IEnumerable<Post>, IEnumerable<PostResource>>(post);

            //this._logger.LogInformation("get all posts");

            //throw new Exception("error!!!!!!!!!!!!!!!");

            return Ok(postResources);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var post = await this._postRepository.GetPostByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            var postResource = this._mapper.Map<Post, PostResource>(post);

            return Ok(postResource);
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