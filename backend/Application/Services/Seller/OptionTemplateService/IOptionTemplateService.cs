using Domain.Dto.Seller.OptionTemplate;
using Domain.Service;

namespace Application.Services.Seller.OptionTemplateService;

public interface IOptionTemplateService
{
    Task<ServiceCollectionResult<OptionTemplateResponseDto>> GetTemplatesByRestaurantId(Guid restaurantId);
    Task<ServiceObjectResult<OptionTemplateResponseDto>> GetTemplateById(Guid id);
    Task<ServiceObjectResult<Guid>> CreateTemplate(CreateOptionTemplateRequestDto request);
    Task<ServiceObjectResult<bool>> UpdateTemplate(UpdateOptionTemplateRequestDto request);
    Task<ServiceObjectResult<bool>> DeleteTemplate(DeleteOptionTemplateRequestDto request);

    Task<ServiceObjectResult<Guid>> AddTemplateValue(AddOptionTemplateValueRequestDto request);
    Task<ServiceObjectResult<bool>> UpdateTemplateValue(UpdateOptionTemplateValueRequestDto request);
    Task<ServiceObjectResult<bool>> DeleteTemplateValue(DeleteOptionTemplateValueRequestDto request);

    Task<ServiceObjectResult<Guid>> AddTemplateValueOption(AddOptionTemplateValueOptionRequestDto request);
    Task<ServiceObjectResult<bool>> UpdateTemplateValueOption(UpdateOptionTemplateValueOptionRequestDto request);
    Task<ServiceObjectResult<bool>> DeleteTemplateValueOption(DeleteOptionTemplateValueOptionRequestDto request);

    Task<ServiceObjectResult<Guid>> AddTemplateValueOptionValue(AddOptionTemplateValueOptionValueRequestDto request);
    Task<ServiceObjectResult<bool>> UpdateTemplateValueOptionValue(UpdateOptionTemplateValueOptionValueRequestDto request);
    Task<ServiceObjectResult<bool>> DeleteTemplateValueOptionValue(DeleteOptionTemplateValueOptionValueRequestDto request);

    Task SyncTemplateToMenuOptions(Guid optionTemplateId);
}