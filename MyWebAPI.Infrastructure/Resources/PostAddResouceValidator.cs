using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWebAPI.Infrastructure.Resources
{
    public class PostAddResouceValidator : AbstractValidator<PostAddResource>
    {
        public PostAddResouceValidator()
        {
            RuleFor(x => x.Title)
               .NotNull()
               .WithName("标题")
               .WithMessage("{PropertyName}是必填的")
               .MaximumLength(50)
               .WithMessage("{PropertyName}的最大长度是{MaxLength}");

            RuleFor(x => x.Body)
               .NotNull()
               .WithName("正文")
               .WithMessage("{PropertyName}是必填的")
               .MaximumLength(100)
               .WithMessage("{PropertyName}的最大长度是{MaxLength}");
        }

    }
}
