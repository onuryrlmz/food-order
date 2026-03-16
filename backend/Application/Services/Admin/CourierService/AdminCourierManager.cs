using Base.Enums;
using Domain.Dto.Admin.Courier;
using Domain.Service;
using Persistence.IRepositories.Common;

namespace Application.Services.Admin.CourierService;

public class AdminCourierManager : IAdminCourierService
{
    private readonly IUserRepository _userRepository;

    public AdminCourierManager(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ServiceCollectionResult<AdminCourierListDto>> GetCouriersAsync(int page, int pageSize)
    {
        var result = new ServiceCollectionResult<AdminCourierListDto>();
        try
        {
            var couriers = await _userRepository.GetListAsync(
                u => u.UserRoleId == (short)AuthorizationServiceEnums.UserRoleEnums.Courier,
                orderBy: q => q.OrderByDescending(u => u.CreatedDate),
                index: page - 1,
                size: pageSize);

            var dtos = couriers.Items.Select(u => new AdminCourierListDto
            {
                UserId = u.Id,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserStatusId = u.UserStatusId,
                UserStatusName = ((AuthorizationServiceEnums.UserStatusEnums)u.UserStatusId).ToString(),
                CreatedDate = u.CreatedDate
            }).ToList();

            result.SetData((int)couriers.Count, dtos);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> ApproveCourierAsync(ApproveCourierDto request)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var user = await _userRepository.GetAsync(
                u => u.Id == request.UserId &&
                     u.UserRoleId == (short)AuthorizationServiceEnums.UserRoleEnums.Courier);

            if (user == null)
            {
                result.AddErrorMessage("Kurye bulunamadı.");
                return result;
            }

            if (request.Approve)
            {
                user.UserStatusId = (short)AuthorizationServiceEnums.UserStatusEnums.Active;
                result.AddSuccessMessage("Kurye onaylandı.");
            }
            else
            {
                user.UserStatusId = (short)AuthorizationServiceEnums.UserStatusEnums.Passive;
                result.AddSuccessMessage("Kurye reddedildi.");
            }

            await _userRepository.UpdateAsync(user);
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}
