using Base.Enums;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace WebAPI.Helpers;

public class AuthorizeAPIRequestAttribute : ActionFilterAttribute
{
    public AuthorizeAPIRequestAttribute(bool requireLogin, bool requireAnonToken, params AuthorizationServiceEnums.UserRoleEnums[] roleList)
    {
        RequireLogin = requireLogin;
        RequireAnonToken = requireAnonToken;
        RoleList = roleList.ToList();
    }

    private bool RequireLogin { get; set; }
    private bool RequireAnonToken { get; set; }
    private List<AuthorizationServiceEnums.UserRoleEnums> RoleList { get; set; }

    public override void OnActionExecuting(ActionExecutingContext actionContext)
    {
        try
        {
            var controller = (BaseController)actionContext.Controller;

            if (RequireLogin)
            {
                if (controller.Client?._tokenDto == null)
                {
                    SetUnauthorized(actionContext, "Lütfen giriş yapınız.");
                    return;
                }

                if (controller.Client._tokenDto.Expiration < DateTime.UtcNow)
                {
                    SetUnauthorized(actionContext, "Oturumunuzun süresi dolmuştur.");
                    return;
                }

                if (RoleList.Count > 0 && !RoleList.Contains(controller.Client._tokenDto.Role))
                {
                    SetForbidden(actionContext, "Bu işlem için yetkiniz bulunmamaktadır.");
                    return;
                }
            }

            if (RequireAnonToken && controller.Client?._anonymousId == null)
            {
                SetUnauthorized(actionContext, "Anonim kimlik gereklidir.");
                return;
            }
        }
        catch
        {
            SetUnauthorized(actionContext, "Yetkilendirme hatası.");
        }

        base.OnActionExecuting(actionContext);
    }

    private static void SetUnauthorized(ActionExecutingContext context, string message)
    {
        var result = new ServiceObjectResult<bool>();
        result.AddErrorMessage(message);
        result.SetData(false);
        context.Result = new JsonResult(result) { StatusCode = (int)HttpStatusCode.Unauthorized };
    }

    private static void SetForbidden(ActionExecutingContext context, string message)
    {
        var result = new ServiceObjectResult<bool>();
        result.AddErrorMessage(message);
        result.SetData(false);
        context.Result = new JsonResult(result) { StatusCode = (int)HttpStatusCode.Forbidden };
    }
}
