# CastThenExpand

I initially raised this issue: https://github.com/OData/AspNetCoreOData/issues/445

Which was supposed to have been fixed, https://github.com/OData/odata.net/pull/2300, but I still cannot do what I want to do.

Have I created the model correctly or is there still an issue?

# Error

https://github.com/OData/odata.net/pull/2300 suggests I should be able to do this: `/default/Emails?$expand=Customer/Default.VipCustomer`, but I get the following error:

```text
An unhandled exception has occurred while executing the request.
      System.InvalidCastException: Unable to cast object of type 'Microsoft.OData.UriParser.TypeSegment' to type 'Microsoft.OData.UriParser.NavigationPropertySegment'.
         at Microsoft.AspNetCore.OData.Query.Validator.SelectExpandQueryValidator.ValidateRestrictions(Nullable`1 remainDepth, Int32 currentDepth, SelectExpandClause selectExpandClause, IEdmNavigationProperty navigationProperty, ODataValidationSettings validationSettings)
         at Microsoft.AspNetCore.OData.Query.Validator.SelectExpandQueryValidator.Validate(SelectExpandQueryOption selectExpandQueryOption, ODataValidationSettings validationSettings)
         at Microsoft.AspNetCore.OData.Query.SelectExpandQueryOption.Validate(ODataValidationSettings validationSettings)
         at Microsoft.AspNetCore.OData.Query.Validator.ODataQueryValidator.Validate(ODataQueryOptions options, ODataValidationSettings validationSettings)
         at Microsoft.AspNetCore.OData.Query.ODataQueryOptions.Validate(ODataValidationSettings validationSettings)
         at Microsoft.AspNetCore.OData.Query.EnableQueryAttribute.ValidateQuery(HttpRequest request, ODataQueryOptions queryOptions)
         at Microsoft.AspNetCore.OData.Query.EnableQueryAttribute.OnActionExecuting(ActionExecutingContext actionExecutingContext)
         at Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeNextActionFilterAsync>g__Awaited|10_0(ControllerActionInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Rethrow(ActionExecutedContextSealed context)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.InvokeInnerFilterAsync()
      --- End of stack trace from previous location ---
         at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeFilterPipelineAsync>g__Awaited|20_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Awaited|17_0(ResourceInvoker invoker, Task task, IDisposable scope)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Awaited|17_0(ResourceInvoker invoker, Task task, IDisposable scope)
         at Microsoft.AspNetCore.Routing.EndpointMiddleware.<Invoke>g__AwaitRequestTask|6_0(Endpoint endpoint, Task requestTask, ILogger logger)
         at Microsoft.AspNetCore.OData.Routing.ODataRouteDebugMiddleware.Invoke(HttpContext context)
         at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware.Invoke(HttpContext context)
```

Originally, https://github.com/OData/AspNetCoreOData/issues/445, I was trying to do filter such that I had the correct types: `/default/Emails?$filter=isof(Customer,'Default.VipCustomer')&$expand=Customer/Default.VipCustomer`, but I end up getting the same error.

So I think it has been partly fixed.

# EDM model

```xml
<?xml version="1.0" encoding="utf-8"?>
<edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
    <edmx:DataServices>
        <Schema Namespace="Default" xmlns="http://docs.oasis-open.org/odata/ns/edm">
            <EntityType Name="Customer">
                <Key>
                    <PropertyRef Name="ID"/>
                </Key>
                <Property Name="ID" Type="Edm.Int32" Nullable="false"/>
                <Property Name="Name" Type="Edm.String" Nullable="false"/>
                <Property Name="EmailID" Type="Edm.Int32"/>
                <NavigationProperty Name="Email" Type="Default.Email">
                    <ReferentialConstraint Property="EmailID" ReferencedProperty="ID"/>
                </NavigationProperty>
            </EntityType>
            <EntityType Name="VipCustomer" BaseType="Default.Customer">
                <Property Name="VipNumber" Type="Edm.Int32" Nullable="false"/>
            </EntityType>
            <EntityType Name="Email">
                <Key>
                    <PropertyRef Name="ID"/>
                </Key>
                <Property Name="ID" Type="Edm.Int32" Nullable="false"/>
                <Property Name="EmailAddress" Type="Edm.String" Nullable="false"/>
                <Property Name="CustomerID" Type="Edm.Int32"/>
                <NavigationProperty Name="Customer" Type="Default.Customer">
                    <ReferentialConstraint Property="CustomerID" ReferencedProperty="ID"/>
                </NavigationProperty>
            </EntityType>
            <EntityContainer Name="Container">
                <EntitySet Name="Customers" EntityType="Default.Customer">
                    <NavigationPropertyBinding Path="Email" Target="Emails"/>
                </EntitySet>
                <EntitySet Name="VipCustomers" EntityType="Default.VipCustomer">
                    <NavigationPropertyBinding Path="Email" Target="Emails"/>
                </EntitySet>
                <EntitySet Name="Emails" EntityType="Default.Email">
                    <NavigationPropertyBinding Path="Customer" Target="Customers"/>
                </EntitySet>
            </EntityContainer>
        </Schema>
    </edmx:DataServices>
</edmx:Edmx>
```
