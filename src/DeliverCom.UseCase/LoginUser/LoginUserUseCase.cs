using System.Linq.Expressions;
using DeliverCom.Core.Exception.Impl;
using DeliverCom.Core.Operation.Model;
using DeliverCom.Core.UseCase.Infrastructure;
using DeliverCom.Domain.Context;
using DeliverCom.Domain.User;
using DeliverCom.Domain.User.Error;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace DeliverCom.UseCase.LoginUser
{
    public class LoginUserUseCase : BaseUseCase<LoginUserUseCaseParameters>
    {
        private readonly DeliverComDbContext _dataContext;

        public LoginUserUseCase(DeliverComDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public override void Validate()
        {
            if (Input == null)
                throw new ArgumentNotValidException($"{nameof(Input)} is null");

            if (string.IsNullOrWhiteSpace(Input.Email))
                throw new ArgumentNotValidException($"{nameof(Input.Email)} is not valid");

            if (string.IsNullOrWhiteSpace(Input.Password))
                throw new ArgumentNotValidException($"{nameof(Input.Password)} is not valid");
        }

        protected override async Task<OperationResult> ExecuteInternal()
        {
            Expression<Func<User, bool>> expression = f => f.Email.Trim() == Input.Email.Trim() &&
                                                           f.Password == Input.Password;
            var user = await _dataContext.Users.FirstOrDefaultAsync(expression.Expand());

            if (user == null)
                throw new UserOrPasswordIncorrectException("User or password incorrect");
            return OperationResult.Ok(user);
        }
    }
}