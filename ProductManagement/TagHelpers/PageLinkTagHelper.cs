using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ProductManagement.TagHelpers
{
    [HtmlTargetElement("div",Attributes = "paging-info")]
    public class PageLinkTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        [ViewContext] [HtmlAttributeNotBound] public ViewContext ViewContext { get; set; }

        public PagingInfo PagingInfo { get; set; }
        public string PageAction { get; set; }
        public bool PageClassEnabled { get; set; }
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public PageLinkTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);

            TagBuilder result = new TagBuilder("div");
            for (int i = 1; i <= PagingInfo.TotalPage; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                tag.Attributes["href"] = PagingInfo.UrlParam.Replace(":", i.ToString());
                if (PageClassEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(PagingInfo.CurrentPage == i ? PageClassSelected : PageClassNormal);
                }

                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }

            output.Content.AppendHtml(result);
        }
    }
}