using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyWebAPI.Core.Entities;
using MyWebAPI.Core.Entities.Enumeration;
using MyWebAPI.Core.EntityParameters;
using MyWebAPI.Core.Interfaces;
using MyWebAPI.Core.Interfaces.IRepositories;
using MyWebAPI.Infrastructure.DataBase;
using MyWebAPI.Infrastructure.Extensions;
using MyWebAPI.Infrastructure.Resources;
using MyWebAPI.Infrastructure.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MyWebAPI.API.Controllers
{
    [Route("api/posts")]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PostController> _logger;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IPropertyMappingContainer _propertyMappingContainer;

        public PostController(IPostRepository postRepository,
            IUnitOfWork unitOfWork,
             ILogger<PostController> logger,
             IMapper mapper,
             IUrlHelper urlHelper,
             ITypeHelperService typeHelperService,
             IPropertyMappingContainer propertyMappingContainer
             )
        {
            this._postRepository = postRepository;
            this._unitOfWork = unitOfWork;
            this._logger = logger;
            this._mapper = mapper;
            this._urlHelper = urlHelper;
            this._typeHelperService = typeHelperService;
            this._propertyMappingContainer = propertyMappingContainer;
        }

        [HttpGet(Name = "GetPosts")]
        public async Task<IActionResult> Get(PostParameter postParameters)
        {
            if (!this._propertyMappingContainer.ValidateMappingExistsFor<PostResource, Post>(postParameters.OrderBy))
            {
                return BadRequest("Can't find fields for sorting");
            }
            if (!this._typeHelperService.TypeHasProperties<PostResource>(postParameters.Fields))
            {
                return BadRequest("Fields not exist.");
            }

            var postList = await this._postRepository.GetAllPostsAsync(postParameters);

            var postResources = this._mapper.Map<IEnumerable<Post>, IEnumerable<PostResource>>(postList);

            var result = postResources.ToDynamicIEnumerable(postParameters.Fields);

            var previousPageLink = postList.HasPrevious ?
                this.CreatePostUri(postParameters, PaginationResourceUriType.PreviousPage) : null;

            var nextPageLink = postList.HasNext ?
                this.CreatePostUri(postParameters, PaginationResourceUriType.NextPage) : null;

            var meta = new
            {
                postList.PageIndex,
                postList.PageSize,
                postList.TotalItemsCount,
                postList.PageCount,
                previousPageLink,
                nextPageLink
            };

            //this._logger.LogInformation("get all posts");
            //throw new Exception("error!!!!!!!!!!!!!!!");

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(meta, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()//设定返回数据属性首字母小写
            }));

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id,string fields)
        {
            if (!_typeHelperService.TypeHasProperties<PostResource>(fields))
            {
                return BadRequest("Fields not exist.");
            }

            var post = await this._postRepository.GetPostByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            var postResource = this._mapper.Map<Post, PostResource>(post);

            var result = postResource.ToDynamic(fields);

            return Ok(result);
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


        /// <summary>
        /// 创建上一页、下一页链接
        /// </summary>
        /// <param name="postParameter"></param>
        /// <returns></returns>
        private string CreatePostUri(PostParameter postParameter, PaginationResourceUriType uriType)
        {
            switch (uriType)
            {
                case PaginationResourceUriType.PreviousPage:
                    var previousParameters = new
                    {
                        pageIndex = postParameter.PageIndex - 1,
                        pageSize = postParameter.PageSize,
                        orderBy = postParameter.OrderBy,
                        fields = postParameter.Fields
                    };
                    return this._urlHelper.Link("GetPosts", previousParameters);
                case PaginationResourceUriType.NextPage:
                    var nextParameters = new
                    {
                        pageIndex = postParameter.PageIndex + 1,
                        pageSize = postParameter.PageSize,
                        orderBy = postParameter.OrderBy,
                        fields = postParameter.Fields
                    };
                    return this._urlHelper.Link("GetPosts", nextParameters);
                default:
                    var currentParameters = new
                    {
                        pageIndex = postParameter.PageIndex,
                        pageSize = postParameter.PageSize,
                        orderBy = postParameter.OrderBy,
                        fields = postParameter.Fields
                    };
                    return this._urlHelper.Link("GetPosts", currentParameters);

            }
        }
    }
}