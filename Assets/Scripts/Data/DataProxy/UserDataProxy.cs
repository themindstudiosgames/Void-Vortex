using Data.Enums;
using UniRx;

namespace Data.DataProxy
{
    public class UserDataProxy
    {
        private readonly ReactiveProperty<LoginStateType> _loginState = new();
        private readonly ReactiveProperty<string> _nickname = new();
        public IReadOnlyReactiveProperty<LoginStateType> LoginState => _loginState;
        public IReadOnlyReactiveProperty<string> Nickname => _nickname;
        
        public string UserPlayFabID { get; set; }

        public void ChangeLoginState(LoginStateType loginStateType) => _loginState.Value = loginStateType;
        public void ChangeNickname(string newNickname) => _nickname.Value = newNickname;
        
    }
}