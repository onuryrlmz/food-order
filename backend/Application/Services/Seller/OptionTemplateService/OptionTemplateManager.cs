using AutoMapper;
using Domain.Dto.Seller.OptionTemplate;
using Domain.Entities.Seller;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller.OptionTemplateService;

public class OptionTemplateManager : IOptionTemplateService
{
    private readonly IMapper _mapper;
    private readonly IOptionTemplateRepository _templateRepository;
    private readonly IOptionTemplateValueRepository _templateValueRepository;
    private readonly IOptionTemplateValueOptionRepository _templateValueOptionRepository;
    private readonly IOptionTemplateValueOptionValueRepository _templateValueOptionValueRepository;
    private readonly IMenuOptionRepository _menuOptionRepository;
    private readonly IMenuOptionValueRepository _menuOptionValueRepository;
    private readonly IMenuOptionValueOptionRepository _menuOptionValueOptionRepository;
    private readonly IMenuOptionValueOptionValueRepository _menuOptionValueOptionValueRepository;

    public OptionTemplateManager(
        IMapper mapper,
        IOptionTemplateRepository templateRepository,
        IOptionTemplateValueRepository templateValueRepository,
        IOptionTemplateValueOptionRepository templateValueOptionRepository,
        IOptionTemplateValueOptionValueRepository templateValueOptionValueRepository,
        IMenuOptionRepository menuOptionRepository,
        IMenuOptionValueRepository menuOptionValueRepository,
        IMenuOptionValueOptionRepository menuOptionValueOptionRepository,
        IMenuOptionValueOptionValueRepository menuOptionValueOptionValueRepository)
    {
        _mapper = mapper;
        _templateRepository = templateRepository;
        _templateValueRepository = templateValueRepository;
        _templateValueOptionRepository = templateValueOptionRepository;
        _templateValueOptionValueRepository = templateValueOptionValueRepository;
        _menuOptionRepository = menuOptionRepository;
        _menuOptionValueRepository = menuOptionValueRepository;
        _menuOptionValueOptionRepository = menuOptionValueOptionRepository;
        _menuOptionValueOptionValueRepository = menuOptionValueOptionValueRepository;
    }

    public async Task<ServiceCollectionResult<OptionTemplateResponseDto>> GetTemplatesByRestaurantId(Guid restaurantId)
    {
        var result = new ServiceCollectionResult<OptionTemplateResponseDto>();
        try
        {
            var templates = await _templateRepository.GetListAsync(
                x => x.RestaurantId == restaurantId,
                include: x => x
                    .Include(t => t.OptionTemplateValues)
                        .ThenInclude(v => v.Product)
                    .Include(t => t.OptionTemplateValues)
                        .ThenInclude(v => v.OptionTemplateValueOptions)
                            .ThenInclude(o => o.OptionTemplateValueOptionValues)
                                .ThenInclude(ov => ov.Product),
                size: 999);

            var dtos = templates.Items
                .OrderBy(t => t.OrderIndex)
                .Select(t => _mapper.Map<OptionTemplateResponseDto>(t))
                .ToList();

            result.SetData(dtos);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<OptionTemplateResponseDto>> GetTemplateById(Guid id)
    {
        var result = new ServiceObjectResult<OptionTemplateResponseDto>();
        try
        {
            var template = await _templateRepository.GetAsync(
                x => x.Id == id,
                include: x => x
                    .Include(t => t.OptionTemplateValues)
                        .ThenInclude(v => v.Product)
                    .Include(t => t.OptionTemplateValues)
                        .ThenInclude(v => v.OptionTemplateValueOptions)
                            .ThenInclude(o => o.OptionTemplateValueOptionValues)
                                .ThenInclude(ov => ov.Product));

            if (template == null)
            {
                result.Fail("Template not found");
                return result;
            }

            result.SetData(_mapper.Map<OptionTemplateResponseDto>(template));
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<Guid>> CreateTemplate(CreateOptionTemplateRequestDto request)
    {
        var result = new ServiceObjectResult<Guid>();
        try
        {
            var count = (await _templateRepository.GetListAsync(x => x.RestaurantId == request.RestaurantId)).Count;
            var template = new OptionTemplate
            {
                Id = Guid.NewGuid(),
                RestaurantId = request.RestaurantId,
                Name = request.Name,
                Description = request.Description,
                MinCount = request.MinCount,
                MaxCount = request.MaxCount,
                OrderIndex = count + 1
            };
            await _templateRepository.AddAsync(template);
            result.SetData(template.Id);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdateTemplate(UpdateOptionTemplateRequestDto request)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var template = await _templateRepository.GetAsync(x => x.Id == request.Id && x.RestaurantId == request.RestaurantId);
            if (template == null)
            {
                result.Fail("Template not found");
                return result;
            }

            template.Name = request.Name;
            template.Description = request.Description;
            template.MinCount = request.MinCount;
            template.MaxCount = request.MaxCount;
            await _templateRepository.UpdateAsync(template);

            // Sync name/min/max to all linked MenuOptions
            var linkedOptions = await _menuOptionRepository.GetListAsync(x => x.OptionTemplateId == template.Id, size: 999);
            foreach (var opt in linkedOptions.Items)
            {
                opt.Name = template.Name;
                opt.Description = template.Description;
                opt.MinCount = template.MinCount;
                opt.MaxCount = template.MaxCount;
                await _menuOptionRepository.UpdateAsync(opt);
            }

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> DeleteTemplate(DeleteOptionTemplateRequestDto request)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var template = await _templateRepository.GetAsync(x => x.Id == request.Id && x.RestaurantId == request.RestaurantId);
            if (template == null)
            {
                result.Fail("Template not found");
                return result;
            }

            // Null out all linked MenuOptions before deleting
            var linkedOptions = await _menuOptionRepository.GetListAsync(x => x.OptionTemplateId == template.Id, size: 999);
            foreach (var opt in linkedOptions.Items)
            {
                opt.OptionTemplateId = null;
                await _menuOptionRepository.UpdateAsync(opt);
            }

            await _templateRepository.DeleteAsync(template);
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<Guid>> AddTemplateValue(AddOptionTemplateValueRequestDto request)
    {
        var result = new ServiceObjectResult<Guid>();
        try
        {
            var template = await _templateRepository.GetAsync(x => x.Id == request.OptionTemplateId);
            if (template == null)
            {
                result.Fail("Template not found");
                return result;
            }

            var count = (await _templateValueRepository.GetListAsync(x => x.OptionTemplateId == request.OptionTemplateId)).Count;
            var value = new OptionTemplateValue
            {
                Id = Guid.NewGuid(),
                OptionTemplateId = request.OptionTemplateId,
                ProductId = request.ProductId,
                Price = request.Price,
                OrderIndex = count + 1
            };
            await _templateValueRepository.AddAsync(value);

            // Auto-create MenuOptionValue for all linked MenuOptions
            var linkedOptions = await _menuOptionRepository.GetListAsync(x => x.OptionTemplateId == template.Id, size: 999);
            foreach (var opt in linkedOptions.Items)
            {
                var menuOptionValueCount = (await _menuOptionValueRepository.GetListAsync(x => x.MenuOptionId == opt.Id)).Count;
                var menuOptionValue = new MenuOptionValue
                {
                    Id = Guid.NewGuid(),
                    MenuOptionId = opt.Id,
                    ProductId = request.ProductId,
                    Price = request.Price,
                    OptionTemplateValueId = value.Id,
                    OrderIndex = menuOptionValueCount + 1
                };
                await _menuOptionValueRepository.AddAsync(menuOptionValue);
            }

            result.SetData(value.Id);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdateTemplateValue(UpdateOptionTemplateValueRequestDto request)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var value = await _templateValueRepository.GetAsync(x => x.Id == request.Id);
            if (value == null)
            {
                result.Fail("Template value not found");
                return result;
            }

            value.ProductId = request.ProductId;
            value.Price = request.Price;
            await _templateValueRepository.UpdateAsync(value);

            // Sync to derived MenuOptionValues
            var derived = await _menuOptionValueRepository.GetListAsync(x => x.OptionTemplateValueId == value.Id, size: 999);
            foreach (var mov in derived.Items)
            {
                mov.ProductId = request.ProductId;
                mov.Price = request.Price;
                await _menuOptionValueRepository.UpdateAsync(mov);
            }

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> DeleteTemplateValue(DeleteOptionTemplateValueRequestDto request)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var value = await _templateValueRepository.GetAsync(x => x.Id == request.Id);
            if (value == null)
            {
                result.Fail("Template value not found");
                return result;
            }

            // Delete derived MenuOptionValues (cascade handles sub-items)
            var derived = await _menuOptionValueRepository.GetListAsync(x => x.OptionTemplateValueId == value.Id, size: 999);
            foreach (var mov in derived.Items)
                await _menuOptionValueRepository.DeleteAsync(mov);

            await _templateValueRepository.DeleteAsync(value);
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<Guid>> AddTemplateValueOption(AddOptionTemplateValueOptionRequestDto request)
    {
        var result = new ServiceObjectResult<Guid>();
        try
        {
            var templateValue = await _templateValueRepository.GetAsync(x => x.Id == request.OptionTemplateValueId);
            if (templateValue == null)
            {
                result.Fail("Template value not found");
                return result;
            }

            var count = (await _templateValueOptionRepository.GetListAsync(x => x.OptionTemplateValueId == request.OptionTemplateValueId)).Count;
            var option = new OptionTemplateValueOption
            {
                Id = Guid.NewGuid(),
                OptionTemplateValueId = request.OptionTemplateValueId,
                Name = request.Name,
                Description = request.Description,
                MinCount = request.MinCount,
                MaxCount = request.MaxCount,
                OrderIndex = count + 1
            };
            await _templateValueOptionRepository.AddAsync(option);

            // Auto-create MenuOptionValueOption for all derived MenuOptionValues
            var derivedValues = await _menuOptionValueRepository.GetListAsync(x => x.OptionTemplateValueId == templateValue.Id, size: 999);
            foreach (var mov in derivedValues.Items)
            {
                var movoCount = (await _menuOptionValueOptionRepository.GetListAsync(x => x.MenuOptionValueId == mov.Id)).Count;
                var movo = new MenuOptionValueOption
                {
                    Id = Guid.NewGuid(),
                    MenuOptionValueId = mov.Id,
                    OptionTemplateValueOptionId = option.Id,
                    Name = request.Name,
                    Description = request.Description,
                    MinCount = request.MinCount,
                    MaxCount = request.MaxCount,
                    OrderIndex = movoCount + 1
                };
                await _menuOptionValueOptionRepository.AddAsync(movo);
            }

            result.SetData(option.Id);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdateTemplateValueOption(UpdateOptionTemplateValueOptionRequestDto request)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var option = await _templateValueOptionRepository.GetAsync(x => x.Id == request.Id);
            if (option == null)
            {
                result.Fail("Template value option not found");
                return result;
            }

            option.Name = request.Name;
            option.Description = request.Description;
            option.MinCount = request.MinCount;
            option.MaxCount = request.MaxCount;
            await _templateValueOptionRepository.UpdateAsync(option);

            // Sync to derived MenuOptionValueOptions
            var derived = await _menuOptionValueOptionRepository.GetListAsync(x => x.OptionTemplateValueOptionId == option.Id, size: 999);
            foreach (var movo in derived.Items)
            {
                movo.Name = request.Name;
                movo.Description = request.Description;
                movo.MinCount = request.MinCount;
                movo.MaxCount = request.MaxCount;
                await _menuOptionValueOptionRepository.UpdateAsync(movo);
            }

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> DeleteTemplateValueOption(DeleteOptionTemplateValueOptionRequestDto request)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var option = await _templateValueOptionRepository.GetAsync(x => x.Id == request.Id);
            if (option == null)
            {
                result.Fail("Template value option not found");
                return result;
            }

            var derived = await _menuOptionValueOptionRepository.GetListAsync(x => x.OptionTemplateValueOptionId == option.Id, size: 999);
            foreach (var movo in derived.Items)
                await _menuOptionValueOptionRepository.DeleteAsync(movo);

            await _templateValueOptionRepository.DeleteAsync(option);
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<Guid>> AddTemplateValueOptionValue(AddOptionTemplateValueOptionValueRequestDto request)
    {
        var result = new ServiceObjectResult<Guid>();
        try
        {
            var templateValueOption = await _templateValueOptionRepository.GetAsync(x => x.Id == request.OptionTemplateValueOptionId);
            if (templateValueOption == null)
            {
                result.Fail("Template value option not found");
                return result;
            }

            var count = (await _templateValueOptionValueRepository.GetListAsync(x => x.OptionTemplateValueOptionId == request.OptionTemplateValueOptionId)).Count;
            var optionValue = new OptionTemplateValueOptionValue
            {
                Id = Guid.NewGuid(),
                OptionTemplateValueOptionId = request.OptionTemplateValueOptionId,
                ProductId = request.ProductId,
                Price = request.Price,
                OrderIndex = count + 1
            };
            await _templateValueOptionValueRepository.AddAsync(optionValue);

            // Auto-create MenuOptionValueOptionValue for all derived MenuOptionValueOptions
            var derivedOptions = await _menuOptionValueOptionRepository.GetListAsync(x => x.OptionTemplateValueOptionId == templateValueOption.Id, size: 999);
            foreach (var movo in derivedOptions.Items)
            {
                var movovCount = (await _menuOptionValueOptionValueRepository.GetListAsync(x => x.MenuOptionValueOptionId == movo.Id)).Count;
                var movov = new MenuOptionValueOptionValue
                {
                    Id = Guid.NewGuid(),
                    MenuOptionValueOptionId = movo.Id,
                    ProductId = request.ProductId,
                    Price = request.Price,
                    OptionTemplateValueOptionValueId = optionValue.Id,
                    OrderIndex = movovCount + 1
                };
                await _menuOptionValueOptionValueRepository.AddAsync(movov);
            }

            result.SetData(optionValue.Id);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdateTemplateValueOptionValue(UpdateOptionTemplateValueOptionValueRequestDto request)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var optionValue = await _templateValueOptionValueRepository.GetAsync(x => x.Id == request.Id);
            if (optionValue == null)
            {
                result.Fail("Template value option value not found");
                return result;
            }

            optionValue.ProductId = request.ProductId;
            optionValue.Price = request.Price;
            await _templateValueOptionValueRepository.UpdateAsync(optionValue);

            var derived = await _menuOptionValueOptionValueRepository.GetListAsync(x => x.OptionTemplateValueOptionValueId == optionValue.Id, size: 999);
            foreach (var movov in derived.Items)
            {
                movov.ProductId = request.ProductId;
                movov.Price = request.Price;
                await _menuOptionValueOptionValueRepository.UpdateAsync(movov);
            }

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task<ServiceObjectResult<bool>> DeleteTemplateValueOptionValue(DeleteOptionTemplateValueOptionValueRequestDto request)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var optionValue = await _templateValueOptionValueRepository.GetAsync(x => x.Id == request.Id);
            if (optionValue == null)
            {
                result.Fail("Template value option value not found");
                return result;
            }

            var derived = await _menuOptionValueOptionValueRepository.GetListAsync(x => x.OptionTemplateValueOptionValueId == optionValue.Id, size: 999);
            foreach (var movov in derived.Items)
                await _menuOptionValueOptionValueRepository.DeleteAsync(movov);

            await _templateValueOptionValueRepository.DeleteAsync(optionValue);
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }
        return result;
    }

    public async Task SyncTemplateToMenuOptions(Guid optionTemplateId)
    {
        var template = await _templateRepository.GetAsync(
            x => x.Id == optionTemplateId,
            include: x => x
                .Include(t => t.OptionTemplateValues)
                    .ThenInclude(v => v.OptionTemplateValueOptions)
                        .ThenInclude(o => o.OptionTemplateValueOptionValues));

        if (template == null) return;

        var linkedOptions = await _menuOptionRepository.GetListAsync(x => x.OptionTemplateId == optionTemplateId, size: 999);

        foreach (var menuOption in linkedOptions.Items)
        {
            var existingValues = await _menuOptionValueRepository.GetListAsync(
                x => x.MenuOptionId == menuOption.Id && x.OptionTemplateValueId != null, size: 999);

            // Remove derived values not in template anymore
            foreach (var existing in existingValues.Items)
            {
                if (!template.OptionTemplateValues.Any(tv => tv.Id == existing.OptionTemplateValueId))
                    await _menuOptionValueRepository.DeleteAsync(existing);
            }

            // Upsert template values
            foreach (var tv in template.OptionTemplateValues)
            {
                var existingMov = existingValues.Items.FirstOrDefault(x => x.OptionTemplateValueId == tv.Id);
                if (existingMov == null)
                {
                    var count = (await _menuOptionValueRepository.GetListAsync(x => x.MenuOptionId == menuOption.Id)).Count;
                    existingMov = new MenuOptionValue
                    {
                        Id = Guid.NewGuid(),
                        MenuOptionId = menuOption.Id,
                        ProductId = tv.ProductId,
                        Price = tv.Price,
                        OptionTemplateValueId = tv.Id,
                        OrderIndex = count + 1
                    };
                    await _menuOptionValueRepository.AddAsync(existingMov);
                }
                else
                {
                    existingMov.ProductId = tv.ProductId;
                    existingMov.Price = tv.Price;
                    await _menuOptionValueRepository.UpdateAsync(existingMov);
                }

                // Sync value options
                var existingMovOptions = await _menuOptionValueOptionRepository.GetListAsync(
                    x => x.MenuOptionValueId == existingMov.Id && x.OptionTemplateValueOptionId != null, size: 999);

                foreach (var tvo in tv.OptionTemplateValueOptions)
                {
                    var existingMovo = existingMovOptions.Items.FirstOrDefault(x => x.OptionTemplateValueOptionId == tvo.Id);
                    if (existingMovo == null)
                    {
                        var movoCount = (await _menuOptionValueOptionRepository.GetListAsync(x => x.MenuOptionValueId == existingMov.Id)).Count;
                        existingMovo = new MenuOptionValueOption
                        {
                            Id = Guid.NewGuid(),
                            MenuOptionValueId = existingMov.Id,
                            OptionTemplateValueOptionId = tvo.Id,
                            Name = tvo.Name,
                            Description = tvo.Description,
                            MinCount = tvo.MinCount,
                            MaxCount = tvo.MaxCount,
                            OrderIndex = movoCount + 1
                        };
                        await _menuOptionValueOptionRepository.AddAsync(existingMovo);
                    }
                    else
                    {
                        existingMovo.Name = tvo.Name;
                        existingMovo.Description = tvo.Description;
                        existingMovo.MinCount = tvo.MinCount;
                        existingMovo.MaxCount = tvo.MaxCount;
                        await _menuOptionValueOptionRepository.UpdateAsync(existingMovo);
                    }

                    // Sync value option values
                    var existingMovovs = await _menuOptionValueOptionValueRepository.GetListAsync(
                        x => x.MenuOptionValueOptionId == existingMovo.Id && x.OptionTemplateValueOptionValueId != null, size: 999);

                    foreach (var tvov in tvo.OptionTemplateValueOptionValues)
                    {
                        var existingMovov = existingMovovs.Items.FirstOrDefault(x => x.OptionTemplateValueOptionValueId == tvov.Id);
                        if (existingMovov == null)
                        {
                            var ovovCount = (await _menuOptionValueOptionValueRepository.GetListAsync(x => x.MenuOptionValueOptionId == existingMovo.Id)).Count;
                            await _menuOptionValueOptionValueRepository.AddAsync(new MenuOptionValueOptionValue
                            {
                                Id = Guid.NewGuid(),
                                MenuOptionValueOptionId = existingMovo.Id,
                                ProductId = tvov.ProductId,
                                Price = tvov.Price,
                                OptionTemplateValueOptionValueId = tvov.Id,
                                OrderIndex = ovovCount + 1
                            });
                        }
                        else
                        {
                            existingMovov.ProductId = tvov.ProductId;
                            existingMovov.Price = tvov.Price;
                            await _menuOptionValueOptionValueRepository.UpdateAsync(existingMovov);
                        }
                    }
                }
            }
        }
    }
}
