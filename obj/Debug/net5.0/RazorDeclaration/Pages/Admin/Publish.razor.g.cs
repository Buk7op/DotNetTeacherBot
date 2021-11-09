// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

namespace DotNetTeacherBot.Pages.Admin
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "c:\Users\Viktor\LocalRepos\DotNetTeacherBot\Pages\Admin\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "c:\Users\Viktor\LocalRepos\DotNetTeacherBot\Pages\Admin\_Imports.razor"
using Microsoft.AspNetCore.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "c:\Users\Viktor\LocalRepos\DotNetTeacherBot\Pages\Admin\_Imports.razor"
using Microsoft.AspNetCore.Components.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "c:\Users\Viktor\LocalRepos\DotNetTeacherBot\Pages\Admin\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "c:\Users\Viktor\LocalRepos\DotNetTeacherBot\Pages\Admin\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "c:\Users\Viktor\LocalRepos\DotNetTeacherBot\Pages\Admin\_Imports.razor"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "c:\Users\Viktor\LocalRepos\DotNetTeacherBot\Pages\Admin\_Imports.razor"
using Microsoft.AspNetCore.Components.Web.Virtualization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "c:\Users\Viktor\LocalRepos\DotNetTeacherBot\Pages\Admin\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "c:\Users\Viktor\LocalRepos\DotNetTeacherBot\Pages\Admin\_Imports.razor"
using Microsoft.EntityFrameworkCore;

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "c:\Users\Viktor\LocalRepos\DotNetTeacherBot\Pages\Admin\_Imports.razor"
using DotNetTeacherBot.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 11 "c:\Users\Viktor\LocalRepos\DotNetTeacherBot\Pages\Admin\_Imports.razor"
using DotNetTeacherBot.Data;

#line default
#line hidden
#nullable disable
#nullable restore
#line 12 "c:\Users\Viktor\LocalRepos\DotNetTeacherBot\Pages\Admin\_Imports.razor"
using DotNetTeacherBot;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "c:\Users\Viktor\LocalRepos\DotNetTeacherBot\Pages\Admin\Publish.razor"
using Microsoft.AspNetCore.Mvc;

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/admin/questions/publish/{id:long}")]
    public partial class Publish : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 16 "c:\Users\Viktor\LocalRepos\DotNetTeacherBot\Pages\Admin\Publish.razor"
 
    [Inject]
    public IQuestionRepo Repository { get; set; }
    [Parameter]
    public long Id { get; set; }
    public Question Question { get; set; }
    public string State => Question.Published == true ? "Unpublish" : "Publish";
    [Inject]
    public NavigationManager NavManager { get; set; }
    protected override void OnParametersSet()
    {
        Question = Repository.Questions.FirstOrDefault(p => p.ID == Id);
    }
    public string EditUrl => $"/admin/questions/edit/{Question.ID}";
    public void PublishQuestion()
    {
        Repository.ChangePublish(Question);
        NavManager.NavigateTo("/admin");
    }

#line default
#line hidden
#nullable disable
    }
}
#pragma warning restore 1591
