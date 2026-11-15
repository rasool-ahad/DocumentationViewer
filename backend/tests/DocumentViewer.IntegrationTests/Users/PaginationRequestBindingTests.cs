using DocumentViewer.Application.Users.Admin.Queries.List;
using System.Globalization;
using Framework.DDD.Domain.Contract.Paginations.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DocumentViewer.IntegrationTests.Users;

public class PaginationRequestBindingTests
{
    [Fact]
    public async Task Admin_user_request_binds_nested_query_fields()
    {
        var services = new ServiceCollection().AddLogging().AddControllers().Services.BuildServiceProvider();
        var metadata = services.GetRequiredService<IModelMetadataProvider>()
            .GetMetadataForType(typeof(ListAdminUsersRequest));
        var binder = services.GetRequiredService<IModelBinderFactory>().CreateBinder(new ModelBinderFactoryContext
        {
            Metadata = metadata,
            BindingInfo = new BindingInfo { BindingSource = BindingSource.Query },
            CacheToken = metadata
        });
        var httpContext = new DefaultHttpContext();
        httpContext.Request.QueryString = new QueryString(
            "?paging.index=2&paging.size=20&filter.search=alice&orders%5BuserName%5D=Descending");
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var valueProvider = new QueryStringValueProvider(BindingSource.Query, httpContext.Request.Query,
            CultureInfo.InvariantCulture);
        var context = DefaultModelBindingContext.CreateBindingContext(actionContext, valueProvider,
            metadata, new BindingInfo { BindingSource = BindingSource.Query }, "");

        await binder.BindModelAsync(context);

        var request = Assert.IsType<ListAdminUsersRequest>(context.Result.Model);
        Assert.Equal(2, request.Paging.Index);
        Assert.Equal(20, request.Paging.Size);
        Assert.Equal("alice", request.Filter.Search);
        Assert.Equal(RequestOrderType.Descending, request.Orders["userName"]);
    }
}
