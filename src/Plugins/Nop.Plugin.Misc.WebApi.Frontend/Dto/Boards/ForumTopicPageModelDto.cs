﻿using Nop.Plugin.Misc.WebApi.Framework.Dto;

namespace Nop.Plugin.Misc.WebApi.Frontend.Dto.Boards;

public partial class ForumTopicPageModelDto : ModelWithIdDto
{
    public string Subject { get; set; }

    public string SeName { get; set; }

    public string WatchTopicText { get; set; }

    public bool IsCustomerAllowedToEditTopic { get; set; }

    public bool IsCustomerAllowedToDeleteTopic { get; set; }

    public bool IsCustomerAllowedToMoveTopic { get; set; }

    public bool IsCustomerAllowedToSubscribe { get; set; }

    public IList<ForumPostModelDto> ForumPostModels { get; set; }

    public int PostsPageIndex { get; set; }

    public int PostsPageSize { get; set; }

    public int PostsTotalRecords { get; set; }
}