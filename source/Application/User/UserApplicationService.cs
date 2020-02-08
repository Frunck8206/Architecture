using Architecture.Database;
using Architecture.Domain;
using Architecture.Infra;
using Architecture.Model;
using DotNetCore.Mapping;
using DotNetCore.Objects;
using DotNetCore.Results;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Architecture.Application
{
    public sealed class UserApplicationService : IUserApplicationService
    {
        private readonly IAuthService _authService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserLogApplicationService _userLogApplicationService;
        private readonly IUserRepository _userRepository;

        public UserApplicationService
        (
            IAuthService authService,
            IUnitOfWork unitOfWork,
            IUserLogApplicationService userLogApplicationService,
            IUserRepository userRepository
        )
        {
            _authService = authService;
            _unitOfWork = unitOfWork;
            _userLogApplicationService = userLogApplicationService;
            _userRepository = userRepository;
        }

        public async Task<IDataResult<long>> AddAsync(AddUserModel addUserModel)
        {
            var validation = new AddUserModelValidator().Validate(addUserModel);

            if (validation.Failed)
            {
                return DataResult<long>.Fail(validation.Message);
            }

            var authModel = _authService.CreateAuth(addUserModel.SignIn);

            var userEntity = UserFactory.Create(addUserModel, authModel);

            userEntity.Add();

            await _userRepository.AddAsync(userEntity);

            await _unitOfWork.SaveChangesAsync();

            return DataResult<long>.Success(userEntity.Id);
        }

        public async Task<IResult> DeleteAsync(long id)
        {
            await _userRepository.DeleteAsync(id);

            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public Task<UserModel> GetAsync(long id)
        {
            return _userRepository.GetByIdAsync(id);
        }

        public async Task InactivateAsync(long id)
        {
            var userEntity = UserFactory.Create(id);

            userEntity.Inactivate();

            await _userRepository.UpdateStatusAsync(userEntity);

            await _unitOfWork.SaveChangesAsync();
        }

        public Task<PagedList<UserModel>> ListAsync(PagedListParameters parameters)
        {
            return _userRepository.Queryable.Project<UserEntity, UserModel>().ListAsync(parameters);
        }

        public async Task<IEnumerable<UserModel>> ListAsync()
        {
            return await _userRepository.Queryable.Project<UserEntity, UserModel>().ToListAsync();
        }

        public async Task<IDataResult<TokenModel>> SignInAsync(SignInModel signInModel)
        {
            var validation = new SignInModelValidator().Validate(signInModel);

            if (validation.Failed)
            {
                return DataResult<TokenModel>.Fail(validation.Message);
            }

            var signModel = await _userRepository.SignInAsync(signInModel);

            validation = _authService.Validate(signModel, signInModel);

            if (validation.Failed)
            {
                return DataResult<TokenModel>.Fail(validation.Message);
            }

            var userLogModel = UserLogFactory.Create(signModel);

            await _userLogApplicationService.AddAsync(userLogModel);

            var tokenModel = _authService.CreateToken(signModel);

            return DataResult<TokenModel>.Success(tokenModel);
        }

        public async Task SignOutAsync(SignedInModel signedInModel)
        {
            var userLogModel = UserLogFactory.Create(signedInModel);

            await _userLogApplicationService.AddAsync(userLogModel);
        }

        public async Task<IResult> UpdateAsync(UpdateUserModel updateUserModel)
        {
            var validation = new UpdateUserModelValidator().Validate(updateUserModel);

            if (validation.Failed)
            {
                return Result.Fail(validation.Message);
            }

            var userEntity = await _userRepository.GetAsync(updateUserModel.Id);

            if (userEntity == default)
            {
                return Result.Success();
            }

            userEntity.ChangeFullName(updateUserModel.FullName.Name, updateUserModel.FullName.Surname);

            userEntity.ChangeEmail(updateUserModel.Email);

            await _userRepository.UpdateAsync(userEntity.Id, userEntity);

            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
}
