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
            var infoRole = context.HttpContext.User.FindFirst("Role").Value;
            if (infoRole == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var Role = (Role)Enum.Parse(typeof(Role), infoRole);


            if (!Roles.Any(r => Role == r))
            {
                context.Result = new ForbidResult();

                throw new Exception("Not enough level of access");
            }

            base.OnActionExecuting(context);
        }
    }
}
