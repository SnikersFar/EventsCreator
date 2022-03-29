using EventsCreator.EfStuff.DbModel;
using EventsCreator.EfStuff.DbModel.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;

namespace EventsCreator.Controllers.Attributes
{
    public class UserAccessAttribute : ActionFilterAttribute
    {
        private Role[] Roles;
        public UserAccessAttribute(params Role[] AccessRoles)
        {
            Roles = AccessRoles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = (User)context.HttpContext.Items["User"];

            if (user == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!Roles.Any(r => user.Role == r))
            {
                context.Result = new ForbidResult();

                throw new Exception("Not enough level of access");
            }

            base.OnActionExecuting(context);
        }
    }
}
