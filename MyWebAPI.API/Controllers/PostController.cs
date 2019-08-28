using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyWebAPI.API.Helpers;
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
    [AllowAnonymous]
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
        [RequestHeaderMatchingMediaType("Accept", new[] { "application/vnd.sen.hateoas+json" })]
        public async Task<IActionResult> GetHateoas(PostParameter postParameters)
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

            var links = this.CreateLinksForPosts(postParameters, postList.HasPrevious, postList.HasNext);

            var shapedResources = postResources.ToDynamicIEnumerable(postParameters.Fields);

            var result = new
            {
                value = shapedResources,
                links
            };

            var meta = new
            {
                postList.PageIndex,
                postList.PageSize,
                postList.TotalItemsCount,
                postList.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(meta, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()//设定返回数据属性首字母小写
                }));

            return Ok(result);

        }

        [HttpGet(Name = "GetPosts")]
        [RequestHeaderMatchingMediaType("Accept", new[] { "application/json" })]
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


            var shapedResources = postResources.ToDynamicIEnumerable(postParameters.Fields);

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

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(meta, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()//设定返回数据属性首字母小写
                    }));

            return Ok(shapedResources);

        }

        [HttpGet("{id}",Name ="GetPost")]
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

            var shapedResource = postResource.ToDynamic(fields);

            var links = this.CreateLinksForPost(id, fields);

            var result = shapedResource as IDictionary<string, object>;

            result.Add("links", links);

            return Ok(result);
        }


        //application/vnd.cgzl.post.create+json
        /// <summary>
        /// 新建文章
        /// </summary>
        /// <returns></returns>
        [HttpPost(Name = "CreatePost")]
        [RequestHeaderMatchingMediaType("Content-Type", new[] { "application/vnd.sen.post.create+json" })]
        [RequestHeaderMatchingMediaType("Accept", new[] { "application/vnd.sen.hateoas+json" })]
        public async Task<IActionResult> Post([FromBody]PostAddResource postAddResource )
        {
            if (postAddResource == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                //return UnprocessableEntity(ModelState);
                return new MyUnprocessableEntityObjectResult(ModelState);
            }

            var newPost = this._mapper.Map<PostAddResource, Post>(postAddResource);

            newPost.Author = "admin";
            newPost.LastModified = DateTime.Now;

            this._postRepository.AddPost(newPost);

            if(!await _unitOfWork.SaveAsync())
            {
                throw new Exception("Save Failed!");
            }    


            var resultResouce = this._mapper.Map<Post, PostResource>(newPost);

            var links = this.CreateLinksForPost(newPost.Id);
            var linkedPostResource = resultResouce.ToDynamic() as IDictionary<string, object>;

            linkedPostResource.Add("links", links);

            return CreatedAtRoute("GetPost", new { id = linkedPostResource["Id"] }, linkedPostResource);
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

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("id",Name ="DeletePost")]
        public async Task<IActionResult> DeletePost(int id) {

            var post = await _postRepository.GetPostByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            _postRepository.Deletet(post);

            if (await this._unitOfWork.SaveAsync())
            {
                throw new Exception($"Deleteing post {id} failed when saving.");
            }

            return NoContent();

        }


        [HttpPut("{id}", Name = "UpdatePost")]
        [RequestHeaderMatchingMediaType("Content-Type", new[] { "application/vnd.sen.post.update+json" })]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] PostUpdateResource postUpdate)
        {
            if (postUpdate == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new MyUnprocessableEntityObjectResult(ModelState);
            }

            var post = await _postRepository.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            post.LastModified = DateTime.Now;
            _mapper.Map(postUpdate, post);

            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception($"Updating post {id} failed when saving.");
            }
            return NoContent();
        }


        [HttpPatch("{id}", Name = "PartiallyUpdatePost")]
        public async Task<IActionResult> PartiallyUpdate(int id,
            [FromBody] JsonPatchDocument<PostUpdateResource> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var post = await _postRepository.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var postToPatch = _mapper.Map<PostUpdateResource>(post);

            patchDoc.ApplyTo(postToPatch, ModelState);

            TryValidateModel(postToPatch);

            if (!ModelState.IsValid)
            {
                return new MyUnprocessableEntityObjectResult(ModelState);
            }

            _mapper.Map(postToPatch, post);
            post.LastModified = DateTime.Now;
            _postRepository.Update(post);

            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception($"Patching city {id} failed when saving.");
            }

            return NoContent();
        }



        /// <summary>
        /// 创建链接(单个post)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private IEnumerable<LinkResource> CreateLinksForPost(int id, string fields = null)
        {
            var links = new List<LinkResource>();

            if (!string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkResource(this._urlHelper.Link("GetPost", new { id }), "self", "GET"));
            }
            else
            {
                links.Add(
                    new LinkResource(this._urlHelper.Link("GetPost", new { id, fields }), "self", "GET"));
            }

            links.Add(
                new LinkResource(this._urlHelper.Link("DeletePost", new { id }), "self", "DELETE"));

            return links;

        }
       


        /// <summary>
        /// 为集合资源创建链接
        /// </summary>
        /// <param name="postResourceParameters"></param>
        /// <param name="hasPrevious"></param>
        /// <param name="hasNext"></param>
        /// <returns></returns>
        private IEnumerable<LinkResource> CreateLinksForPosts(PostParameter postResourceParameters,
          bool hasPrevious, bool hasNext)
        {
            var links = new List<LinkResource>
            {
                new LinkResource(
                    CreatePostUri(postResourceParameters, PaginationResourceUriType.CurrentPage),
                    "self", "GET")
            };

            if (hasPrevious)
            {
                links.Add(
                    new LinkResource(
                        CreatePostUri(postResourceParameters, PaginationResourceUriType.PreviousPage),
                        "previous_page", "GET"));
            }

            if (hasNext)
            {
                links.Add(
                    new LinkResource(
                        CreatePostUri(postResourceParameters, PaginationResourceUriType.NextPage),
                        "next_page", "GET"));
            }

            return links;
        }
    }
}