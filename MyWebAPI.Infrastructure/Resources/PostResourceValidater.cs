﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWebAPI.Infrastructure.Resources
{
    /// <summary>
    /// 文章验证器
    /// </summary>
    public class PostResourceValidater : AbstractValidator<PostResource>
    {
        public PostResourceValidater()
        {
            RuleFor(x => x.Author)
                .NotNull()
                .WithName("作者")
                .WithMessage("{PropertyName}是必填的")
                .MaximumLength(50)
                .WithMessage("{PropertyName}的最大长度是{MaxLength}");
        }
    }
}