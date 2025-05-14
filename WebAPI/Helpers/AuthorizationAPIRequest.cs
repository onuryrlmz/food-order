using Base.Enums;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI.Helpers;

public class AuthorizeAPIRequestAttribute : ActionFilterAttribute
{
    public AuthorizeAPIRequestAttribute(bool _requireLogin, bool _requireAnonToken, AuthorizationServiceEnums.UserRoleEnums[] _roleList)
    {
        RequireLogin = _requireLogin;
        RequireAnonToken = _requireAnonToken;
        RoleList = _roleList.ToList();
    }

    private bool RequireLogin { get; set; }
    private bool RequireAnonToken { get; set; }
    private List<AuthorizationServiceEnums.UserRoleEnums> RoleList { get; set; }

    public override void OnActionExecuting(ActionExecutingContext actionContext)
    {
        var failResult = new ServiceObjectResult<bool>();
        try
        {
            failResult.SetData(true);

            //var actionName = (actionContext.ActionDescriptor as ControllerActionDescriptor)?.ActionName;
            var controller = (BaseController)actionContext.Controller;

            if (RequireLogin)
            {
                if (controller.Client?._tokenDto == null)
                {
                    failResult.AddErrorMessage("Lütfen giriş yapınız.");
                    failResult.SetData(false);
                }
                else
                {
                    if (RoleList.Count > 0)
                        if (!RoleList.Contains(controller.Client._tokenDto.Role))
                        {
                            failResult.AddErrorMessage("Yetkisiz işlem.");
                            failResult.SetData(false);
                        }
                }
            }

            if (RequireAnonToken)
                if (controller.Client?._anonymousId == null)
                {
                    failResult.AddErrorMessage("Lütfen giriş yapınız.");
                    failResult.SetData(false);
                }
        }
        catch (Exception ex)
        {
            failResult.Fail("NotAuthorized", ex.Message);
        }

        if (failResult.Data == false)
            actionContext.Result = new JsonResult(failResult);

        base.OnActionExecuting(actionContext);
    }
}