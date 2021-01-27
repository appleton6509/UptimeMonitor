using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UptimeAPI.Messaging;

namespace UptimeAPI.Services
{
    /// <summary>
    /// provides authorization for a user to modify an existing resource
    /// </summary>
    public class ModelAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, IBaseModel>
    {
          private readonly Dictionary<Type,string > _types = new Dictionary<Type,string >
            {
                {typeof(EndPoint),nameof(EndPoint)},
                {typeof(ResultData),nameof(ResultData) }
            };
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement, IBaseModel resource)
        {
            bool resultFound = Guid.TryParse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId);
            if (Object.Equals(resource, null) || !resultFound)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            bool isSuccessful = false;
            switch (_types[resource.GetType()])
            {
                case nameof(EndPoint):
                    {
                        if ((resource as EndPoint).UserId == userId)
                            isSuccessful = true;
                        break;
                    }
                default:
                    break;
            }
            if (isSuccessful)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
    public class Operations
    {
        public static OperationAuthorizationRequirement Create =
            new OperationAuthorizationRequirement { Name = nameof(Create) };
        public static OperationAuthorizationRequirement Read =
            new OperationAuthorizationRequirement { Name = nameof(Read) };
        public static OperationAuthorizationRequirement Update =
            new OperationAuthorizationRequirement { Name = nameof(Update) };
        public static OperationAuthorizationRequirement Delete =
            new OperationAuthorizationRequirement { Name = nameof(Delete) };
    }
}
