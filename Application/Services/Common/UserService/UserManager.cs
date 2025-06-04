using AutoMapper;
using Base.Constant;
using Base.Entities;
using Base.Enums;
using Base.Security;
using Domain.Dto.Common;
using Domain.Entities.Common;
using Domain.Service;
using Newtonsoft.Json;
using Persistence.IRepositories.Common;
using Persistence.IRepositories.Seller;

namespace Application.Services.Common.UserService;

public class UserManager : IUserService
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IRestaurantRepository _restaurantRepository;

    public UserManager(IUserRepository userRepository, IMapper mapper, IRestaurantRepository restaurantRepository)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _restaurantRepository = restaurantRepository;
    }

    public override async Task<ServiceObjectResult<bool>> Register(UserRegisterDto requestDto)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            var checkEmail = await _userRepository.GetAsync(x => x.Email == requestDto.Email, withDeleted: true);
            if (checkEmail != null)
            {
                response.AddErrorMessage("Email already exists.");
            }
            else
            {
                var mappedUser = _mapper.Map<User>(requestDto);
                mappedUser.UserStatusId = (short)AuthorizationServiceEnums.UserStatusEnums.WaitingForActivation;
                mappedUser.Password = requestDto.Password.Encrypt(Global.EncryptionKey);
                mappedUser.ActivationKey = Guid.NewGuid();
                mappedUser.UserRoleId = (short)AuthorizationServiceEnums.UserRoleEnums.User;

                await _userRepository.AddAsync(mappedUser);
                response.SetData(true);
            }
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }

    public override async Task<ServiceObjectResult<bool>> Login(UserLoginDto requestDto)
    {
        var response = new ServiceObjectResult<bool>();
        try
        {
            requestDto.Password = requestDto.Password.Encrypt(Global.EncryptionKey);

            var user = await _userRepository.GetAsync(x => x.Email == requestDto.Email && x.Password == requestDto.Password && x.UserStatusId == (short)AuthorizationServiceEnums.UserStatusEnums.Active);
            if (user == null)
            {
                response.AddErrorMessage("User not found");
            }
            else
            {
                var tokenDto = new TokenDto
                {
                    UserId = user.Id,
                    Token = Guid.NewGuid(),
                    Expiration = DateTime.Now.AddDays(30),
                    Role = AuthorizationServiceEnums.UserRoleEnumList[user.UserRoleId]
                };

                if (user.UserRoleId is (short)AuthorizationServiceEnums.UserRoleEnums.SellerAdmin or (short)AuthorizationServiceEnums.UserRoleEnums.SellerUser)
                {
                    tokenDto.SellerId = user.SellerId;
                    tokenDto.RestaurantIds = _restaurantRepository.GetList(x => x.SellerId == user.SellerId, size: 999)?.Items?.Select(x => x.Id)?.ToList();
                }

                var tokenStringData = JsonConvert.SerializeObject(tokenDto);
                var tokenString = tokenStringData.Encrypt(Global.EncryptionKey);
                response.Token = tokenString;
                response.SetData(true);
            }
        }
        catch (Exception e)
        {
            response.Fail(e);
        }

        return response;
    }
}